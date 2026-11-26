/// Worker role: claim, execute through the application's handler, settle (EAR-CMD-006/012/013/014/017/018).
/// The application supplies one Operation per operation name; everything else is provided here.
module Echelon.Execution.Worker

open System.Threading.Tasks

type ExecutionContext<'tx> =
    { Tx: 'tx
      Command: Command
      /// Payload after upcasting to the operation's current contract version.
      Payload: string
      Now: Instant }

/// What a handler reports. Business writes happen through ctx.Tx inside the same transaction.
type HandlerResult =
    | Done of Outcome * effects: (string * string) list
    | Transient of Fault
    | Poison of Fault
    | Ambiguous of obligation: string
    | NeedDependency of CommandId list
    | NeedApproval of approvalRef: string

type Operation<'tx> =
    { Name: OperationName
      CurrentVersion: int
      /// Pure upcaster from an older contract version to the current one (EAR-CMD-005).
      Upcast: int -> string -> Result<string, string>
      /// Execution-time authorization and policy re-check, inside the command transaction (EAR-CMD-017).
      Authorize: ExecutionContext<'tx> -> Task<Result<unit, string>>
      Execute: ExecutionContext<'tx> -> Task<HandlerResult> }

type Settings =
    { Owner: string
      LeaseDuration: System.TimeSpan
      Retry: Policy.RetryPolicy
      BatchSize: int }

/// Stable effect keys: command id + ordinal, so a re-sent effect carries the same provider idempotency key (EAR-CMD-008).
let effectKey (CommandId id) (ordinal: int) = sprintf "%O:%d" id ordinal

let private settle (state: LifecycleState) (attempts: int) (outbox: OutboxMessage list) (reason: string) =
    { NewState = state
      Attempts = attempts
      Outbox = outbox
      Reason = reason }

/// Pure: map a handler result to the next lifecycle state. Ambiguous outcomes are never retried (EAR-CMD-014).
let decide
    (retry: Policy.RetryPolicy)
    (now: Instant)
    (jitter: float)
    (command: Command)
    (result: HandlerResult)
    : Result<Settlement, Lifecycle.IllegalTransition> =
    let attempts = command.Attempts + 1

    let event, outbox, reason =
        match result with
        | Done(outcome, effects) ->
            let outbox =
                effects
                |> List.mapi (fun i (topic, payload) ->
                    { EffectKey = effectKey command.Id i
                      Topic = topic
                      Payload = payload })

            Lifecycle.Complete outcome, outbox, "handler completed"
        | Transient fault ->
            Lifecycle.TransientFailure(fault, Policy.nextAttempt retry attempts now jitter), [], fault.Code
        | Poison fault -> Lifecycle.PoisonDetected fault, [], fault.Code
        | Ambiguous obligation -> Lifecycle.AmbiguousEffect obligation, [], "external outcome unknown"
        | NeedDependency ids -> Lifecycle.WaitForDependency ids, [], "awaiting dependency"
        | NeedApproval approval -> Lifecycle.WaitForApproval approval, [], "awaiting approval"

    Lifecycle.transition command.State event
    |> Result.map (fun next -> settle next attempts outbox reason)

/// Pure: settlement for a handler that threw. Repeated crashes become poison (EAR-CMD-013).
let onCrash (retry: Policy.RetryPolicy) (now: Instant) (jitter: float) (command: Command) (error: exn) : Settlement =
    let attempts = command.Attempts + 1
    let fault = { Code = "HandlerCrashed"; Detail = error.GetType().Name }

    let event =
        if attempts >= retry.QuarantineAfterCrashes then
            Lifecycle.PoisonDetected fault
        else
            Lifecycle.TransientFailure(fault, Policy.nextAttempt retry attempts now jitter)

    match Lifecycle.transition command.State event with
    | Ok next -> settle next attempts [] fault.Code
    | Error _ -> settle (Quarantined fault) attempts [] "crash in unexpected state"

let private runOne
    (operations: Map<OperationName, Operation<'tx>>)
    (settings: Settings)
    (now: unit -> Instant)
    (jitter: unit -> float)
    (tx: 'tx)
    (command: Command)
    : Task<Settlement> =
    task {
        let at = now ()
        let poison code detail = Poison { Code = code; Detail = detail }

        let! result =
            match operations.TryFind command.Operation with
            | None ->
                let (OperationName name) = command.Operation
                Task.FromResult(poison "UnknownOperation" name)
            | Some operation ->
                match operation.Upcast command.Version command.Payload with
                | Error reason -> Task.FromResult(poison "UnsupportedVersion" reason)
                | Ok payload ->
                    task {
                        let ctx =
                            { Tx = tx
                              Command = command
                              Payload = payload
                              Now = at }

                        match! operation.Authorize ctx with
                        | Error reason -> return Done(Declined(sprintf "NotAuthorized: %s" reason), [])
                        | Ok() -> return! operation.Execute ctx
                    }

        match decide settings.Retry at (jitter ()) command result with
        | Ok settlement -> return settlement
        | Error illegal ->
            return
                settle
                    (Quarantined
                        { Code = "IllegalTransition"
                          Detail = sprintf "%s/%s" illegal.From illegal.Event })
                    (command.Attempts + 1)
                    []
                    "illegal transition"
    }

type CycleReport =
    { Claimed: int
      Settled: (CommandId * Settlement) list
      Faults: (CommandId option * StoreFault) list }

/// One worker cycle: expire overdue work, claim a batch, execute each claimed command once.
let cycle
    (store: Store<'tx>)
    (operations: Map<OperationName, Operation<'tx>>)
    (settings: Settings)
    (now: unit -> Instant)
    (jitter: unit -> float)
    : Task<CycleReport> =
    task {
        let at = now ()
        let! _ = store.ExpireDue at

        match!
            store.Claim
                { Owner = settings.Owner
                  Now = at
                  LeaseExpires = at.Add settings.LeaseDuration
                  Limit = settings.BatchSize }
        with
        | Error fault ->
            return
                { Claimed = 0
                  Settled = []
                  Faults = [ None, fault ] }
        | Ok claimed ->
            let! results =
                claimed
                |> List.map (fun c ->
                    task {
                        let! r =
                            store.Execute
                                c.Id
                                settings.Owner
                                (runOne operations settings now jitter)
                                (onCrash settings.Retry (now ()) (jitter ()))

                        return c.Id, r
                    })
                |> List.fold
                    (fun acc next ->
                        task {
                            let! xs = acc
                            let! x = next
                            return xs @ [ x ]
                        })
                    (Task.FromResult [])

            return
                { Claimed = claimed.Length
                  Settled =
                    results
                    |> List.choose (fun (id, r) ->
                        match r with
                        | Ok s -> Some(id, s)
                        | Error _ -> None)
                  Faults =
                    results
                    |> List.choose (fun (id, r) ->
                        match r with
                        | Error f -> Some(Some id, f)
                        | Ok _ -> None) }
    }

type Delivery =
    | Delivered
    | NotDelivered of reason: string

/// Outbox relay: at-least-once delivery using the stable effect key as the provider idempotency key.
let relay (store: Store<'tx>) (limit: int) (send: PendingEffect -> Task<Delivery>) : Task<Result<int, StoreFault>> =
    task {
        match! store.PendingOutbox limit with
        | Error f -> return Error f
        | Ok pending ->
            let! outcomes =
                pending
                |> List.fold
                    (fun acc effect ->
                        task {
                            let! done' = acc
                            let! delivery = send effect
                            let delivered = (delivery = Delivered)
                            let! _ = store.MarkOutbox effect.Message.EffectKey delivered
                            return if delivered then done' + 1 else done'
                        })
                    (Task.FromResult 0)

            return Ok outcomes
    }

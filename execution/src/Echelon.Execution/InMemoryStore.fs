/// Reference adapter for tests and local development. Store semantics are pure functions over an immutable
/// State; the only mutation is swapping the state reference under a lock at the edge.
module Echelon.Execution.InMemoryStore

open System.Threading.Tasks

/// Business writes made through the transaction handle; committed only with the command's settlement.
type Tx = { mutable Writes: string list }

type IdentityKey = TenantId * PrincipalId * OperationName * IdempotencyKey

type State =
    { Commands: Map<CommandId, Command>
      Keys: Map<IdentityKey, CommandId>
      History: HistoryEntry list
      Outbox: Map<string, PendingEffect * bool>
      Pause: Policy.PauseState
      BusinessLog: string list }

let empty =
    { Commands = Map.empty
      Keys = Map.empty
      History = []
      Outbox = Map.empty
      Pause = Policy.noPause
      BusinessLog = [] }

let private identity (c: Command) : IdentityKey =
    c.Tenant, c.Principal, c.Operation, c.IdempotencyKey

let private entry (c: Command) (next: LifecycleState) (at: Instant) (actor: string) (reason: string) =
    { Command = c.Id
      From = Lifecycle.label c.State
      To = Lifecycle.label next
      At = at
      Actor = actor
      Reason = reason }

// ---- pure state functions -------------------------------------------------------------------------------

let insert (c: Command) (s: State) : State * InsertOutcome =
    match s.Keys.TryFind(identity c) with
    | Some existing -> s, Existing s.Commands.[existing]
    | None ->
        { s with
            Commands = s.Commands.Add(c.Id, c)
            Keys = s.Keys.Add(identity c, c.Id)
            History = entry c c.State c.AcceptedAt "intake" "accepted" :: s.History },
        Inserted

let countNonTerminal (tenant: TenantId) (s: State) =
    s.Commands
    |> Map.toList
    |> List.filter (fun (_, c) -> c.Tenant = tenant && not (Lifecycle.isTerminal c.State))
    |> List.length

let claim (r: ClaimRequest) (s: State) : State * Command list =
    let chosen =
        s.Commands |> Map.toList |> List.map snd |> Policy.selectForClaim r.Now s.Pause r.Limit

    let claimed =
        chosen
        |> List.choose (fun c ->
            match Lifecycle.transition c.State (Lifecycle.Claim(r.Owner, r.LeaseExpires)) with
            | Ok next -> Some(c, { c with State = next })
            | Error _ -> None)

    { s with
        Commands = claimed |> List.fold (fun m (_, c) -> Map.add c.Id c m) s.Commands
        History =
            (claimed |> List.map (fun (before, after) -> entry before after.State r.Now r.Owner "claimed"))
            @ s.History },
    claimed |> List.map snd

let holdsLease (owner: string) (c: Command) =
    match c.State with
    | Processing(o, _) -> o = owner
    | _ -> false

let settle (c: Command) (st: Settlement) (writes: string list) (at: Instant) (owner: string) (s: State) : State =
    let updated =
        { c with
            State = st.NewState
            Attempts = st.Attempts }

    { s with
        Commands = s.Commands.Add(c.Id, updated)
        History = entry c st.NewState at owner st.Reason :: s.History
        Outbox =
            st.Outbox
            |> List.fold
                (fun m msg ->
                    Map.add
                        msg.EffectKey
                        ({ Command = c.Id
                           Tenant = c.Tenant
                           Message = msg
                           Attempts = 0 },
                         false)
                        m)
                s.Outbox
        BusinessLog = s.BusinessLog @ writes }

let apply (tenant: TenantId) (id: CommandId) (event: Lifecycle.Event) (at: Instant) (actor: string) (reason: string) (s: State) =
    match s.Commands.TryFind id with
    | Some c when c.Tenant = tenant ->
        match Lifecycle.transition c.State event with
        | Ok next ->
            { s with
                Commands = s.Commands.Add(id, { c with State = next })
                History = entry c next at actor reason :: s.History },
            Ok next
        | Error illegal -> s, Error(Illegal illegal)
    | _ -> s, Error NotFound

// ---- effectful edge ------------------------------------------------------------------------------------

type Handle =
    { Snapshot: unit -> State
      /// Fault injection: when true, every store call reports Unavailable (simulates a database outage).
      SetUnavailable: bool -> unit }

let create (clock: unit -> Instant) : Store<Tx> * Handle =
    let gate = obj ()
    let mutable state = empty
    let mutable unavailable = false

    let atomically (f: State -> State * 'a) : Result<'a, StoreFault> =
        lock gate (fun () ->
            if unavailable then
                Error(Unavailable "simulated outage")
            else
                let next, result = f state
                state <- next
                Ok result)

    let read (f: State -> 'a) = atomically (fun s -> s, f s)

    let execute (id: CommandId) (owner: string) (run: Tx -> Command -> Task<Settlement>) (crash: Command -> exn -> Settlement) =
        task {
            match read (fun s -> s.Commands.TryFind id) with
            | Error f -> return Error f
            | Ok None -> return Error NotFound
            | Ok(Some c) when not (holdsLease owner c) -> return Error LeaseNotHeld
            | Ok(Some c) ->
                let tx = { Writes = [] }

                let! outcome =
                    task {
                        try
                            let! st = run tx c
                            return Choice1Of2 st
                        with error ->
                            return Choice2Of2 error
                    }

                let settlement, writes =
                    match outcome with
                    | Choice1Of2 st -> st, tx.Writes
                    | Choice2Of2 error -> crash c error, [] // rolled back: business writes discarded

                return
                    atomically (fun s ->
                        match s.Commands.TryFind id with
                        | Some current when holdsLease owner current ->
                            settle current settlement writes (clock ()) owner s, Ok settlement
                        | _ -> s, Error LeaseNotHeld)
                    |> Result.bind (fun r -> r)
        }

    let store: Store<Tx> =
        { Insert = fun c -> Task.FromResult(atomically (insert c))
          CountNonTerminal = fun t -> Task.FromResult(read (countNonTerminal t))
          Claim = fun r -> Task.FromResult(atomically (claim r))
          Execute = execute
          Apply =
            fun tenant id event actor reason ->
                Task.FromResult(atomically (apply tenant id event (clock ()) actor reason) |> Result.bind (fun r -> r))
          Get =
            fun tenant id ->
                Task.FromResult(read (fun s -> s.Commands.TryFind id |> Option.filter (fun c -> c.Tenant = tenant)))
          History =
            fun tenant id ->
                Task.FromResult(
                    read (fun s ->
                        match s.Commands.TryFind id with
                        | Some c when c.Tenant = tenant -> s.History |> List.filter (fun h -> h.Command = id) |> List.rev
                        | _ -> [])
                )
          ExpireDue =
            fun now ->
                Task.FromResult(
                    atomically (fun s ->
                        let due = s.Commands |> Map.toList |> List.map snd |> Policy.dueForExpiry now

                        let next =
                            due
                            |> List.fold
                                (fun st c -> fst (apply c.Tenant c.Id Lifecycle.Expire now "system" "processing deadline passed" st))
                                s

                        next, due |> List.map (fun c -> c.Id))
                )
          SetPause =
            fun scope paused ->
                Task.FromResult(
                    atomically (fun s ->
                        let toggle set value = if paused then Set.add value set else Set.remove value set

                        let pause =
                            match scope with
                            | Everything -> { s.Pause with Everything = paused }
                            | Tenant t -> { s.Pause with Tenants = toggle s.Pause.Tenants t }
                            | Operation o -> { s.Pause with Operations = toggle s.Pause.Operations o }

                        { s with Pause = pause }, ())
                )
          PendingOutbox =
            fun limit ->
                Task.FromResult(
                    read (fun s ->
                        s.Outbox |> Map.toList |> List.filter (fun (_, (_, sent)) -> not sent) |> List.map (snd >> fst) |> List.truncate limit)
                )
          MarkOutbox =
            fun key delivered ->
                Task.FromResult(
                    atomically (fun s ->
                        match s.Outbox.TryFind key with
                        | Some(effect, _) ->
                            { s with Outbox = s.Outbox.Add(key, ({ effect with Attempts = effect.Attempts + 1 }, delivered)) }, ()
                        | None -> s, ())
                ) }

    store,
    { Snapshot = fun () -> lock gate (fun () -> state)
      SetUnavailable = fun v -> lock gate (fun () -> unavailable <- v) }

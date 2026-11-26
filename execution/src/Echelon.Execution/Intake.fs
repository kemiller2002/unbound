/// Intake for durable classes (EAR-API-005, -007, -008, -009; EAR-CMD-001, -004).
/// Authentication, submission authorization and structural validation happen before this call, in the API layer.
module Echelon.Execution.Intake

open System
open System.Threading.Tasks

type Submission =
    { Tenant: TenantId
      Principal: PrincipalId
      Operation: OperationName
      Version: int
      Class: RequestClass
      IdempotencyKey: IdempotencyKey
      /// Canonical payload produced by the operation's pure normalizer.
      Payload: string
      OrderingKey: OrderingKey option
      Correlation: CorrelationId
      Causation: string option
      NotBefore: Instant option
      Deadline: Instant option
      Priority: int }

type Rejection =
    | NotDurableClass of RequestClass
    | IdempotencyConflict of original: CommandId
    | Throttled of retryAfter: TimeSpan
    /// The store could not confirm a commit: nothing may be reported as accepted (EAR-API-007, EAR-AVL-010).
    | AcceptanceUnavailable of retryWithSameKey: IdempotencyKey

type Response =
    | NewlyAccepted of Receipt
    | Duplicate of Receipt

type Limits = { QueuedPerTenant: int; RetryAfter: TimeSpan }

let defaultLimits =
    { QueuedPerTenant = 10_000
      RetryAfter = TimeSpan.FromSeconds 30.0 }

/// Pure construction of the command to persist.
let toCommand (id: Guid) (now: Instant) (s: Submission) : Command =
    { Id = CommandId id
      Tenant = s.Tenant
      Principal = s.Principal
      Operation = s.Operation
      Version = s.Version
      Class = s.Class
      IdempotencyKey = s.IdempotencyKey
      Fingerprint = Policy.fingerprint s.Operation s.Version s.Payload
      Payload = s.Payload
      OrderingKey = s.OrderingKey
      Correlation = s.Correlation
      Causation = s.Causation
      AcceptedAt = now
      Deadline = s.Deadline
      Priority = s.Priority
      Attempts = 0
      State =
        match s.NotBefore with
        | Some notBefore when notBefore > now -> Scheduled notBefore
        | _ -> Accepted }

/// Pure: the receipt never claims completion, settlement or a tax-effective date.
let receiptOf (c: Command) : Receipt =
    { Command = c.Id
      IdempotencyKey = c.IdempotencyKey
      Correlation = c.Correlation
      Operation = c.Operation
      AcceptedAt = c.AcceptedAt
      State = Lifecycle.label c.State
      Meaning =
        match c.State with
        | Scheduled notBefore -> ScheduledForProcessing notBefore
        | _ -> AcceptedForProcessing }

/// Pure: resolve a duplicate key against the stored command.
let resolveExisting (fingerprint: string) (existing: Command) : Result<Response, Rejection> =
    if existing.Fingerprint = fingerprint then
        Ok(Duplicate(receiptOf existing))
    else
        Error(IdempotencyConflict existing.Id)

/// Accept a durable command. The receipt is produced only from a committed insert.
let accept
    (store: Store<'tx>)
    (limits: Limits)
    (newId: unit -> Guid)
    (now: unit -> Instant)
    (s: Submission)
    : Task<Result<Response, Rejection>> =
    task {
        if not (RequestClass.isDurable s.Class) then
            return Error(NotDurableClass s.Class)
        else
            match! store.CountNonTerminal s.Tenant with
            | Error _ -> return Error(AcceptanceUnavailable s.IdempotencyKey)
            | Ok queued when queued >= limits.QueuedPerTenant -> return Error(Throttled limits.RetryAfter)
            | Ok _ ->
                let command = toCommand (newId ()) (now ()) s

                match! store.Insert command with
                | Ok Inserted -> return Ok(NewlyAccepted(receiptOf command))
                | Ok(Existing existing) -> return resolveExisting command.Fingerprint existing
                | Error _ -> return Error(AcceptanceUnavailable s.IdempotencyKey)
    }

/// Pure policies: fingerprinting, retry backoff and claim selection. No I/O; time and randomness are inputs.
module Echelon.Execution.Policy

open System
open System.Security.Cryptography
open System.Text

/// SHA-256 over the operation, contract version and the operation's canonical (normalized) payload.
/// Two submissions with the same idempotency key must carry the same fingerprint (EAR-API-005).
let fingerprint (OperationName operation) (version: int) (canonicalPayload: string) =
    sprintf "%s\n%d\n%s" operation version canonicalPayload
    |> Encoding.UTF8.GetBytes
    |> SHA256.HashData
    |> Convert.ToHexStringLower

type RetryPolicy =
    { MaxAttempts: int
      BaseDelay: TimeSpan
      MaxDelay: TimeSpan
      /// Unhandled crashes beyond this count mark the command as poison (EAR-CMD-013).
      QuarantineAfterCrashes: int }

let defaultRetryPolicy =
    { MaxAttempts = 5
      BaseDelay = TimeSpan.FromSeconds 2.0
      MaxDelay = TimeSpan.FromMinutes 5.0
      QuarantineAfterCrashes = 3 }

/// Exponential backoff with caller-supplied jitter in [0, 1). None means the attempt budget is exhausted.
let nextAttempt (policy: RetryPolicy) (attemptsSoFar: int) (now: Instant) (jitter: float) : Instant option =
    if attemptsSoFar >= policy.MaxAttempts then
        None
    else
        let exponential = policy.BaseDelay.TotalMilliseconds * Math.Pow(2.0, float (max 0 (attemptsSoFar - 1)))
        let capped = min exponential policy.MaxDelay.TotalMilliseconds
        let withJitter = capped * (0.5 + 0.5 * (min 0.999999 (max 0.0 jitter)))
        Some(now.AddMilliseconds withJitter)

type PauseState =
    { Everything: bool
      Tenants: Set<TenantId>
      Operations: Set<OperationName> }

let noPause =
    { Everything = false
      Tenants = Set.empty
      Operations = Set.empty }

let isPaused (pause: PauseState) (command: Command) =
    pause.Everything
    || pause.Tenants.Contains command.Tenant
    || pause.Operations.Contains command.Operation

let private orderOf (c: Command) = (c.AcceptedAt, c.Id)

/// Per ordering key, only the oldest non-terminal command is eligible, and only if it is claimable now.
/// A key held by a blocking state (processing under a live lease, quarantine, unknown outcome, waiting) yields nothing.
let headsOfKeys (now: Instant) (commands: Command list) : Command list =
    let keyed, unordered = commands |> List.partition (fun c -> c.OrderingKey.IsSome)

    let keyHeads =
        keyed
        |> List.filter (fun c -> not (Lifecycle.isTerminal c.State))
        |> List.groupBy (fun c -> c.Tenant, c.OrderingKey)
        |> List.choose (fun (_, group) ->
            let head = group |> List.minBy orderOf
            if Lifecycle.isClaimable now head.State then Some head else None)

    let unorderedReady =
        unordered |> List.filter (fun c -> Lifecycle.isClaimable now c.State)

    keyHeads @ unorderedReady

/// Fair selection (EAR-CMD-020): round-robin across tenants, each tenant ordered by priority then age.
let selectForClaim (now: Instant) (pause: PauseState) (limit: int) (commands: Command list) : Command list =
    let byTenant =
        headsOfKeys now commands
        |> List.filter (isPaused pause >> not)
        |> List.groupBy (fun c -> c.Tenant)
        |> List.sortBy fst
        |> List.map (fun (_, cs) -> cs |> List.sortBy (fun c -> -c.Priority, c.AcceptedAt, c.Id))

    let rec interleave (queues: Command list list) =
        match queues |> List.filter (List.isEmpty >> not) with
        | [] -> []
        | nonEmpty -> (nonEmpty |> List.map List.head) @ interleave (nonEmpty |> List.map List.tail)

    interleave byTenant |> List.truncate limit

/// Unstarted commands past their processing deadline (EAR-CMD-016).
let dueForExpiry (now: Instant) (commands: Command list) : Command list =
    commands
    |> List.filter (fun c ->
        match c.Deadline, c.State with
        | Some deadline, (Accepted | Scheduled _ | RetryScheduled _ | AwaitingDependency _) -> deadline < now
        | _ -> false)

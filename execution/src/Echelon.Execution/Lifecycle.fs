/// Pure, exhaustively matched lifecycle transition function (EAR-CMD §2, SEC-LANG-002).
/// Mirrors Ordo's vocabulary: an ambiguous external effect becomes OutcomeUnknown with a reconciliation obligation.
module Echelon.Execution.Lifecycle

type Event =
    | Claim of owner: string * leaseExpires: Instant
    | LeaseLost
    | Complete of Outcome
    | TransientFailure of Fault * nextAttempt: Instant option
    | PoisonDetected of Fault
    | AmbiguousEffect of obligation: string
    | WaitForDependency of CommandId list
    | WaitForApproval of approvalRef: string
    | Resume
    | Cancel of reason: string
    | Expire
    | Replay
    | ReconciledNotExecuted
    | ReconciledExecuted of Outcome
    | ReconciliationFailed of Fault

type IllegalTransition = { From: string; Event: string }

let label =
    function
    | Accepted -> "Accepted"
    | Scheduled _ -> "Scheduled"
    | Processing _ -> "Processing"
    | AwaitingDependency _ -> "AwaitingDependency"
    | AwaitingApproval _ -> "AwaitingApproval"
    | RetryScheduled _ -> "RetryScheduled"
    | Completed _ -> "Completed"
    | Failed _ -> "Failed"
    | Quarantined _ -> "Quarantined"
    | Cancelled _ -> "Cancelled"
    | Expired -> "Expired"
    | OutcomeUnknown _ -> "OutcomeUnknown"

let eventLabel (e: Event) =
    match e with
    | Claim _ -> "Claim"
    | LeaseLost -> "LeaseLost"
    | Complete _ -> "Complete"
    | TransientFailure _ -> "TransientFailure"
    | PoisonDetected _ -> "PoisonDetected"
    | AmbiguousEffect _ -> "AmbiguousEffect"
    | WaitForDependency _ -> "WaitForDependency"
    | WaitForApproval _ -> "WaitForApproval"
    | Resume -> "Resume"
    | Cancel _ -> "Cancel"
    | Expire -> "Expire"
    | Replay -> "Replay"
    | ReconciledNotExecuted -> "ReconciledNotExecuted"
    | ReconciledExecuted _ -> "ReconciledExecuted"
    | ReconciliationFailed _ -> "ReconciliationFailed"

/// Terminal states never transition again except Failed, which an authorized replay may reopen.
let isTerminal =
    function
    | Completed _
    | Failed _
    | Cancelled _
    | Expired -> true
    | Accepted
    | Scheduled _
    | Processing _
    | AwaitingDependency _
    | AwaitingApproval _
    | RetryScheduled _
    | Quarantined _
    | OutcomeUnknown _ -> false

/// Unstarted states: cancellation and expiry are only legal before execution begins.
let private isUnstarted =
    function
    | Accepted
    | Scheduled _
    | RetryScheduled _
    | AwaitingDependency _
    | AwaitingApproval _ -> true
    | Processing _
    | Completed _
    | Failed _
    | Quarantined _
    | Cancelled _
    | Expired
    | OutcomeUnknown _ -> false

let transition (state: LifecycleState) (event: Event) : Result<LifecycleState, IllegalTransition> =
    let illegal () =
        Error
            { From = label state
              Event = eventLabel event }

    match state, event with
    | (Accepted | Scheduled _ | RetryScheduled _), Claim(owner, expires) -> Ok(Processing(owner, expires))
    // Re-claim after a crashed worker's lease expired; the scheduler only offers expired leases (isClaimable).
    | Processing _, Claim(owner, expires) -> Ok(Processing(owner, expires))
    | Processing _, LeaseLost -> Ok Accepted
    | Processing _, Complete outcome -> Ok(Completed outcome)
    | Processing _, TransientFailure(_, Some next) -> Ok(RetryScheduled next)
    | Processing _, TransientFailure(fault, None) -> Ok(Failed fault)
    | Processing _, PoisonDetected fault -> Ok(Quarantined fault)
    | Processing _, AmbiguousEffect obligation -> Ok(OutcomeUnknown obligation)
    | Processing _, WaitForDependency ids -> Ok(AwaitingDependency ids)
    | Processing _, WaitForApproval approval -> Ok(AwaitingApproval approval)
    | (AwaitingDependency _ | AwaitingApproval _), Resume -> Ok Accepted
    | s, Cancel reason when isUnstarted s -> Ok(Cancelled reason)
    | s, Expire when isUnstarted s -> Ok Expired
    | (Failed _ | Quarantined _), Replay -> Ok Accepted
    | OutcomeUnknown _, ReconciledNotExecuted -> Ok Accepted
    | OutcomeUnknown _, ReconciledExecuted outcome -> Ok(Completed outcome)
    | OutcomeUnknown _, ReconciliationFailed fault -> Ok(Failed fault)
    | _ -> illegal ()

/// A command in one of these states holds its ordering key: later commands on the key wait (EAR-CMD-009, -013).
let blocksOrderingKey (now: Instant) =
    function
    | Processing(_, expires) -> expires > now
    | AwaitingDependency _
    | AwaitingApproval _
    | Quarantined _
    | OutcomeUnknown _
    | RetryScheduled _
    | Scheduled _
    | Accepted -> true
    | Completed _
    | Failed _
    | Cancelled _
    | Expired -> false

/// Whether this command can be claimed now, ignoring ordering (EAR-CMD-018: expired leases are reclaimable).
let isClaimable (now: Instant) =
    function
    | Accepted -> true
    | Scheduled notBefore -> notBefore <= now
    | RetryScheduled next -> next <= now
    | Processing(_, expires) -> expires <= now
    | AwaitingDependency _
    | AwaitingApproval _
    | Completed _
    | Failed _
    | Quarantined _
    | Cancelled _
    | Expired
    | OutcomeUnknown _ -> false

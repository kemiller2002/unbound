/// Core vocabulary of the durable execution module (INIT-012 EAR-API, EAR-CMD).
/// Immutable data only; no I/O in this file.
namespace Echelon.Execution

open System

type TenantId = TenantId of string
type PrincipalId = PrincipalId of string
type CommandId = CommandId of Guid
type IdempotencyKey = IdempotencyKey of string
type OrderingKey = OrderingKey of string
type CorrelationId = CorrelationId of string
type OperationName = OperationName of string
type Instant = DateTimeOffset

/// Static request classification (EAR-API-002). Only the durable classes reach the command store.
type RequestClass =
    | ImmediateQuery
    | ImmediateControl
    | ImmediateCommand
    | DurableCommand
    | ScheduledCommand
    | LongRunningOperation
    | HumanApprovalWorkflow
    | ExternalIntegrationOperation

module RequestClass =
    let isDurable =
        function
        | DurableCommand
        | ScheduledCommand
        | LongRunningOperation
        | HumanApprovalWorkflow
        | ExternalIntegrationOperation -> true
        | ImmediateQuery
        | ImmediateControl
        | ImmediateCommand -> false

    let name (c: RequestClass) = sprintf "%A" c

    let parse =
        function
        | "ImmediateQuery" -> Some ImmediateQuery
        | "ImmediateControl" -> Some ImmediateControl
        | "ImmediateCommand" -> Some ImmediateCommand
        | "DurableCommand" -> Some DurableCommand
        | "ScheduledCommand" -> Some ScheduledCommand
        | "LongRunningOperation" -> Some LongRunningOperation
        | "HumanApprovalWorkflow" -> Some HumanApprovalWorkflow
        | "ExternalIntegrationOperation" -> Some ExternalIntegrationOperation
        | _ -> None

type Fault = { Code: string; Detail: string }

/// Business outcome of a command whose transaction committed. A decline is an outcome, not a failure.
type Outcome =
    | Succeeded of detail: string
    | Declined of reason: string

/// Command lifecycle (EAR-CMD §2). Received and intake-Rejected are deliberately not states.
type LifecycleState =
    | Accepted
    | Scheduled of notBefore: Instant
    | Processing of leaseOwner: string * leaseExpires: Instant
    | AwaitingDependency of CommandId list
    | AwaitingApproval of approvalRef: string
    | RetryScheduled of nextAttempt: Instant
    | Completed of Outcome
    | Failed of Fault
    | Quarantined of Fault
    | Cancelled of reason: string
    | Expired
    | OutcomeUnknown of obligation: string

type Command =
    { Id: CommandId
      Tenant: TenantId
      Principal: PrincipalId
      Operation: OperationName
      Version: int
      Class: RequestClass
      IdempotencyKey: IdempotencyKey
      Fingerprint: string
      Payload: string
      OrderingKey: OrderingKey option
      Correlation: CorrelationId
      Causation: string option
      AcceptedAt: Instant
      Deadline: Instant option
      Priority: int
      Attempts: int
      State: LifecycleState }

type OutboxMessage =
    { EffectKey: string
      Topic: string
      Payload: string }

type HistoryEntry =
    { Command: CommandId
      From: string
      To: string
      At: Instant
      Actor: string
      Reason: string }

/// What intake promises. There is no constructor meaning "completed" (EAR-API-008).
type ReceiptMeaning =
    | AcceptedForProcessing
    | ScheduledForProcessing of notBefore: Instant

type Receipt =
    { Command: CommandId
      IdempotencyKey: IdempotencyKey
      Correlation: CorrelationId
      Operation: OperationName
      AcceptedAt: Instant
      State: string
      Meaning: ReceiptMeaning }

type PauseScope =
    | Everything
    | Tenant of TenantId
    | Operation of OperationName

/// The only effectful boundary of the module: a record of functions implemented by an adapter
/// (in-memory for tests, PostgreSQL for production). 'tx is the adapter's transaction handle, passed to
/// application handlers so business writes commit atomically with the command's completion (EAR-CMD-006).
namespace Echelon.Execution

open System.Threading.Tasks

type StoreFault =
    | Unavailable of detail: string
    | NotFound
    | LeaseNotHeld
    | Illegal of Lifecycle.IllegalTransition

type InsertOutcome =
    | Inserted
    | Existing of Command

/// The result of executing one claimed command, computed purely by the worker from the handler's result.
type Settlement =
    { NewState: LifecycleState
      Attempts: int
      Outbox: OutboxMessage list
      Reason: string }

type ClaimRequest =
    { Owner: string
      Now: Instant
      LeaseExpires: Instant
      Limit: int }

type PendingEffect =
    { Command: CommandId
      Tenant: TenantId
      Message: OutboxMessage
      Attempts: int }

type Store<'tx> =
    { /// Atomically insert unless (tenant, principal, operation, idempotency key) exists (EAR-CMD-004).
      Insert: Command -> Task<Result<InsertOutcome, StoreFault>>
      CountNonTerminal: TenantId -> Task<Result<int, StoreFault>>
      /// Claim up to Limit commands under a lease, honouring ordering, fairness and pause (EAR-CMD-009/018/020).
      Claim: ClaimRequest -> Task<Result<Command list, StoreFault>>
      /// Open a transaction, lock the command, verify the caller still holds the lease, run `run` with the
      /// transaction, persist its settlement, lifecycle history and outbox, and commit. If `run` throws, roll back
      /// and persist `onCrash` instead in a fresh transaction.
      Execute:
          CommandId
              -> string
              -> ('tx -> Command -> Task<Settlement>)
              -> (Command -> exn -> Settlement)
              -> Task<Result<Settlement, StoreFault>>
      /// Apply an operator or system lifecycle event (cancel, expire, replay, resume, reconcile) with history.
      Apply: TenantId -> CommandId -> Lifecycle.Event -> string -> string -> Task<Result<LifecycleState, StoreFault>>
      Get: TenantId -> CommandId -> Task<Result<Command option, StoreFault>>
      History: TenantId -> CommandId -> Task<Result<HistoryEntry list, StoreFault>>
      ExpireDue: Instant -> Task<Result<CommandId list, StoreFault>>
      SetPause: PauseScope -> bool -> Task<Result<unit, StoreFault>>
      PendingOutbox: int -> Task<Result<PendingEffect list, StoreFault>>
      MarkOutbox: string -> bool -> Task<Result<unit, StoreFault>> }

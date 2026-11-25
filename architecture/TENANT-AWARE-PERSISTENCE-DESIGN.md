# Tenant-aware persistence architecture and implementation contract
Date: 2026-11-21 | Status: design specification, NOT implemented or validated

## Mandatory Strata integration requirement
**UNB-TEC-STRATA-001 (MANDATORY):** Unbound SHALL use **Strata** as an explicit component of its data/persistence architecture. Strata integration is required, not an optional alternative to be silently replaced by generic database access. Before implementation, discover and verify the actual Strata repository, package/API surface, current release, supported storage backends, append-only guarantees and compatibility with the current EF stack. Record the verified integration contract and pin its version through the approved EF installation/governance process. Do not invent Strata capabilities or claim it is installed before verification.
> Converged 2026-11-24: governed by `CNV-ARC-004` (section 'Converged requirements: master mission' in the domain file named by each clause).

**UNB-TEC-STRATA-002:** The tenant-aware connection/persistence layer SHALL integrate Strata while preserving all three partner isolation profiles (separate schema, separate database, dedicated infrastructure), or document an explicit incompatibility requiring owner review before implementation.
> Converged 2026-11-24: governed by `CNV-ARC-004` (section 'Converged requirements: master mission' in the domain file named by each clause).

**UNB-TEC-STRATA-003:** Strata's role in authoritative event persistence, projection management, schema evolution, integrity verification, and correction handling SHALL be determined from verified functionality. Any capabilities Strata does not provide SHALL be implemented through separately governed components, without relaxing append-only requirements.
> Converged 2026-11-24: governed by `CNV-ARC-004` (section 'Converged requirements: master mission' in the domain file named by each clause).

**UNB-TEC-STRATA-004:** Automated integration and adversarial tests SHALL demonstrate Strata-backed tenant isolation, append-only corrections, bitemporal history, deterministic replay, backup/restore verification, and migration between isolation profiles where supported. Do not mark tests passed without execution evidence.
> Converged 2026-11-24: governed by `CNV-ARC-004` (section 'Converged requirements: master mission' in the domain file named by each clause).

**Architecture gate:** Strata is a binding product selection. Its specific integration design remains pending technical verification; a discovered limitation is a design blocker to escalate, not permission to substitute it without approval.

## Scope and decision
Design one F# modular-monolith persistence boundary supporting per-partner **(A) separate schema in shared database**, **(B) separate database on shared managed infrastructure**, and **(C) dedicated partner database/infrastructure**, without branching domain logic. The actual database engine is **not yet selected**. This specification does not authorize implementation before Unbound's requirements approval and Conditor bootstrap.

The system's authoritative business record is **append-only**. All committed business facts, journals, policy approvals, fund participants, successor-account allocations and decisions are immutable. Changes are represented as new authorized correction, supersession or compensating events. Current-state projections are rebuildable and may be updated.

## Logical architecture
```text
  WASM UI / partner API / authorized agents
                    |
       Authenticated request boundary
                    |
      PartnerContextResolver (trusted)
                    |
        Authorization / sponsor policy
                    |
        Application command handlers
                    |
         ITenantUnitOfWorkFactory
                    |
       TenantConnectionBroker / lease
           /          |           \
      Schema A     Database B    Dedicated C
           \          |           /
           Append-only EventStore
                    |
        Outbox + deterministic projector
                    |
     Tenant-scoped read models / evidence
```

**Never** let a browser, LLM, external partner API or user-provided tenant string choose a database/schema. The broker resolves tenant identity from verified principal + contractual partner/program entitlement and checks the requested fund's owning tenant.

## Proposed F# contracts (illustrative, not compiled)
```fsharp
type TenantId = private TenantId of string
type SponsorId = private SponsorId of string
type FundId = private FundId of string
type EventId = private EventId of string

type IsolationProfile =
    | SharedDatabaseDedicatedSchema
    | DedicatedDatabaseSharedPlatform
    | DedicatedInfrastructure

type VerifiedTenantContext = private {
    Tenant: TenantId
    Sponsor: SponsorId
    PrincipalId: string
    AuthorizationVersion: string
    IsolationProfile: IsolationProfile
}

type ExpectedVersion = NoStream | Exact of int64
type AppendCommand = {
    StreamId: string
    ExpectedVersion: ExpectedVersion
    IdempotencyKey: string
    EventType: string
    SchemaVersion: int
    Payload: byte[]
    EffectiveAt: System.DateTimeOffset
    CausationId: string option
    EvidenceRefs: string list
}
type AppendReceipt = {
    EventId: EventId
    StreamVersion: int64
    TenantSequence: int64
    RecordedAt: System.DateTimeOffset
    IntegrityReference: string
}
type IVerifiedTenantResolver =
    abstract Resolve: principal:obj * fund:FundId option -> Async<Result<VerifiedTenantContext,string>>
type ITenantUnitOfWorkFactory =
    abstract Open: VerifiedTenantContext -> Async<Result<ITenantUnitOfWork,string>>
and ITenantUnitOfWork =
    inherit System.IAsyncDisposable
    abstract Append: AppendCommand -> Async<Result<AppendReceipt,string>>
    abstract ReadStream: streamId:string * asKnownAt:System.DateTimeOffset option -> Async<Result<obj list,string>>
    abstract Commit: unit -> Async<Result<unit,string>>
```
These contracts deliberately omit provider-specific connection strings, schema names and administrative methods from application callers. The exact types, asynchronous patterns and interfaces must be reconciled with current EF libraries.

## Connection broker design
1. Verify authentication, partner membership, sponsor relationship, fund authorization and the scope of delegated agent tools.
2. Resolve immutable TenantId to a versioned, approved deployment binding held in a **minimal control plane**. The control plane stores routing and health metadata, not confidential donor or grant payloads.
3. Acquire a least-privilege partner-scoped connection/transaction. For profile A, bind an explicit, allowlisted schema and partner-specific database role; never interpolate untrusted identifiers or depend on a mutable default search path.
4. Enforce tenant-specific read/write authority again in the persistence layer, not only in UI or API code.
5. Append authorized events with optimistic stream version and idempotency check in the same durable transaction as outbox intent.
6. Emit an immutable receipt and process projections asynchronously or transactionally as appropriate. Side effects are dispatched through an independently authorized payment/trade/notice gateway, never during replay.
7. Release/reset connection state, including session authorization, transaction state and schema/search-path settings, before pooling.

## Append-only physical enforcement
- Application identities have INSERT and constrained SELECT privileges on authoritative event/journal tables, **no UPDATE, DELETE, TRUNCATE or destructive DDL**.
- Corrections reference original event, reason, evidence, authorizer and effective/recorded times.
- Tamper-evident checkpoint digests must be stored independently of the mutable administrative boundary. A database administrator with unrestricted physical access may still rewrite storage, so database permissions alone are not a WORM guarantee.
- Backups, replica logs, event snapshots and evidence artifacts have separately specified retention and integrity requirements.
- Event replay and projection rebuild must be side-effect-free and deterministic; durable external effect receipts and idempotency keys prevent duplicate transfers or payments.

## Data and authority boundaries
- Partner tenant is **not** necessarily the sponsor. Define the legal sponsor, institutional partner, program and fund independently.
- One tenant may contain many funds; a fund may have arbitrarily many participants, role grants and successor accounts. The fund's tenant boundary is immutable unless an approved transfer/migration explicitly changes its placement.
- Cross-partner fund transfers require two independently authorized records (outgoing and incoming) and reconciliation, not an implicit cross-database join or distributed transaction.
- Shared charity reference data may be reused only where licensed/authorized; partner-specific eligibility decisions, private donor records and agent context remain isolated.
- Each tenant's object storage, logs, indexes, embeddings, caches, queues, outbox, projections and backups must be inventoried and isolated according to its deployment profile.

## Migration A → B → C and rollback
1. Append an approved TenantMigrationPlanned record and freeze conflicting topology/policy changes.
2. Provision destination and verify encryption, identity, least-privilege, schema compatibility and append-only constraints.
3. Capture a consistent source event watermark; copy immutable event history and permitted evidence; verify event IDs, sequence, payload digests, corrections and historical query behavior.
4. Catch up the append-only event tail while the source remains sole writer. Verify ledgers, grants, participant/successor relationships and projections.
5. Quiesce writes and external effect dispatch; append migration checkpoint, reconcile outbox and payments, and fence the old writer.
6. Atomically update the trusted tenant routing binding to destination with an explicit generation/lease token. Reject stale leases and enforce **one authoritative writer**.
7. Resume traffic, run negative isolation tests, compare balances and monitor. A rollback is a new controlled routing/migration action, never a history rewrite.
8. Preserve old data for the approved retention/verification window; disposition follows sponsor ownership, contracts and legal holds.

## Required engineering test matrix
| Test | Shared DB / separate schema | Dedicated DB | Dedicated infrastructure |
|---|---|---|---|
| Cross-tenant SQL access denied | Required | Required | Required |
| Spoofed tenant routing denied | Required | Required | Required |
| Pooled connection state leakage | Required | Required | Required |
| UPDATE/DELETE/TRUNCATE of events denied | Required | Required | Required |
| Correction and bitemporal replay | Required | Required | Required |
| Event integrity and restore verification | Required | Required | Required |
| Per-tenant backup and selective restore | Document limitations + test | Required | Required |
| AI retrieval/cache/telemetry isolation | Required | Required | Required |
| 10× surge and noisy-neighbor behavior | Required | Required | Required |
| Partner migration with in-flight payments | A→B | B→C | C→B where permitted |
| Multi-participant and successor-account access | Required | Required | Required |
| Source/destination single-writer fencing | Required | Required | Required |

## Architecture decisions to resolve before implementation
- **ADR-001:** Verify Strata's actual persistence role and supported database engines, then select the managed database engine and true schema isolation capabilities; do not assume a specific provider.
- **ADR-002:** Tenant registry and authorization provenance, including sponsor/partner relationships.
- **ADR-003:** Connection pooling, schema binding, credential rotation, database role provisioning and SQL injection protections.
- **ADR-004:** Event envelope, expected-version concurrency, idempotency index, ordering and outbox transaction.
- **ADR-005:** Independently anchored integrity checkpoints and tenant-specific backup/restore.
- **ADR-006:** Key management, PII vault and lawful erasure/retention across immutable events and backups.
- **ADR-007:** Tenant migration fencing, dual-run validation, export and incident recovery.
- **ADR-008:** EF Registry/Conditor compatible versions and deployment automation for managed infrastructure.
- **ADR-009:** Partner isolation tier economics at 10/100/1,000 tenants.
- **ADR-010:** Sponsor-level authoritative books and reporting when a sponsor serves multiple segregated partners.

## Requirements traceability and gate
Implements the *design intent* of UNB-TEN-001..020, UNB-DATA-001..012, UNB-IMM-001..010, UNB-PART-001..009 and UNB-SUCC-001..014. This is an architecture proposal and test plan, **not evidence of a working persistence implementation**. No database selected, no code compiled, no tests executed, and no Conditor installation performed in this change.

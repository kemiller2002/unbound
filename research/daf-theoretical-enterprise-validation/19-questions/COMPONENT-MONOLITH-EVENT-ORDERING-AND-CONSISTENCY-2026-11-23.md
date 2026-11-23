# Component monolith, event ordering and authoritative consistency

Date: 2026-11-23. **User architecture directive: component monolith, not microservices.** Theoretical requirements only; no implementation, simulation, board approval or provider validation.

## Architecture decision

Unbound is one deployable, modular **component monolith** with explicit in-process component interfaces and one authoritative transactional financial/security core. Modules may have separately owned schemas, commands, policies and tests but do not communicate through internal network APIs, message brokers or independent write databases as a default. Internal modules are not independently deployable services. External partner APIs, payment rails, user-facing APIs/widgets, managed databases and independently durable outage/protective channels are integration and resilience boundaries, not justification for internal microservices.

Candidate modules: Identity/Authority, Sponsor Policy, Common Charity Registry, Grant Orchestration, Contribution Intake, Payment Orchestration, Ledger/Reservations/AUM, Compliance/Holds, Donor and Sponsor Administration, Widget/API Gateway, Evidence and Audit, Notifications, Risk/AI Exception Workbench, DR/Operations. These are logical components in the same deployment and versioned release. In-process events can decouple noncritical work, but no in-process event is allowed to silently bypass transaction safety.

Prefer F# and existing Echelon components where fit is verified; do not commit to a particular WASM server runtime, DB topology or licensing assumption without ADR and validation. Managed databases and minimal operational tooling preferred; no Kubernetes, Terraform or microservice decomposition as architectural defaults.

## One authoritative command boundary

All consequential state changes enter typed domain commands, authenticated and idempotent, through one command pipeline: validate principal and authority -> pin applicable global/sponsor policy -> resolve evidence snapshot -> evaluate mandatory rules -> acquire transaction-scoped locks/version checks -> recheck current safety epochs -> atomically commit authorized state and audit/outbox record -> acknowledge only after quorum-durable commit.

A shared physical database does not mean unrestricted table writes. Component-owned tables/views and domain command interfaces enforce ownership; database privileges, transaction invariants and audit controls must prevent bypass. Cross-component changes that must be atomic (grant approval + reservation, hold publication + release gate visibility, payment release intent + idempotency state) occur in one authoritative transaction. Cross-module read models, UI search and reporting may lag and are never trusted as final release gates.

**Outbox:** The transaction records external dispatch intent durably; a worker may deliver it at least once, so stable idempotency and external reconciliation remain mandatory. Never assume database transaction and external bank transfer are one atomic operation.

## Ordering and safety epochs

Use an authoritative, monotonically ordered commit position (or equivalent serializable transaction order) for all critical grant/payment/hold commands. IDs are globally unique application-generated identifiers but are not themselves ordering clocks. Store separate monotonic epochs/versions for global charity status, global protective holds, sponsor policy, actor authority/revocation, recipient destination and payment-intent state. Every release records the exact versions it checked.

**Hold effectiveness:** A hold becomes effective against new authoritative releases at its durable commit position. A release that serializes *before* the hold may have been authorized under the previously known state; a release that serializes *after* must observe the hold and stop. The system must not claim it can retract an already transmitted external payment. For externally discovered adverse events not yet ingested, the platform can only guarantee action after authoritative ingestion or under a separately specified protective feed/latency contract. This is a real residual risk requiring monitoring, not a false zero-window guarantee.

**Critical serialization:** Release and hold publication must conflict through a shared gate/epoch or equivalent database concurrency control. Snapshot reads alone may allow write skew; enforce serializable isolation, locking or explicit compare-and-swap with tested behavior. Every payment release rechecks current charity/sanctions, destination, authority, sponsor policy, protective holds, funding, payment identity and writer fencing at the commit boundary.

**Outbound race:** Committing a release intent and transmitting to an external rail are different events. A hold committed after release intent but before actual provider submission should stop dispatch where technically possible. Dispatch worker must perform a final authoritative hold/qualification check and atomically claim send eligibility. If a hold arrives after irreversible provider submission, preserve the exact sequence and pursue cancellation/recall only when supported. Define precise dispatch point of no return per rail and never misstate guarantees.

## Example ordering trace

| Order | Event | Required interpretation |
|---|---|---|
| 100 | Grant approved and $100 reserved | Sponsor-authorized grant, not yet payment |
| 101 | Payment destination version 7 verified | Evidence bound to version 7 |
| 102 | Global charity hold committed | Effective for subsequent release/dispatch decisions |
| 103 | Worker attempts payment release using snapshot from 101 | Must reject stale release, no dispatch |
| 104 | New evidence clears hold via independently authorized command | New hold epoch and audit history; old approval not automatically enough |
| 105 | Worker reevaluates all release gates | May proceed only if all current conditions pass |

A separate trace must test the hold arriving after provider submission; correct outcome is evidence-based incident/recall, not a fictitious automatic reversal.

## Consistency classes

| Class | Examples | Required consistency | Failure behavior |
|---|---|---|---|
| C0 mandatory authoritative | Balances/reservations, approvals, sanctions holds, charity eligibility, destination versions, authority revocations, payment send intents | Serializable or proven equivalent invariant-preserving atomic commands; quorum-durable acknowledgments | Fail closed on unsafe quorum or stale gate |
| C1 independently durable protective | Emergency holds, revocations, cancellations during core outage | Separate durable authenticated append-only protective channel, reconciled before replay | Do not report success without durability; no money movement |
| C2 nonauthoritative intake | Outage grant/recurring instructions | Independently durable queue, authenticated, deduplicated, labeled Queued only | Never claim approved/reserved/paid |
| C3 eventual projections | Search, dashboards, notifications, AI case summaries, widgets' nonfinancial cached views | Eventual, with source version and freshness displayed | Never used to authorize payment |
| C4 external uncertain | Bank/provider execution, settlement, nonreceipt, finality | Evidence-based reconciliation; no invented exactly-once external guarantee | Unknown state and no blind retry |

## DR with a monolith

One active authoritative writer at a time, with fenced epochs and quorum durability across appropriate failure domains; warm standby monolith instance(s) may serve failover, but not independent competing write leaders. App-generated IDs avoid site-specific sequence collisions, while idempotency prevents duplicate business effects. A shared deployment does not imply single-site or single-instance availability. Two sites plus witness or three independent voting failure domains are candidate patterns, not selected topology. Loss of safe quorum pauses authoritative financial writes; durable intake and protective-only emergency controls can continue if separately healthy. Safe failback requires replica catch-up, fencing, reconciliation and controlled resumption.

An independently deployed **minimal protective-only plane** is an explicit safety exception to the component-monolith deployment rule, not a second general-purpose business service. It may record holds/revocations and incidents but never approve grants, edit destinations, balances or release money. This exception must be minimized and independently threat-modeled. The independent intake channel may use managed queue/storage without becoming a second financial core.

## Architectural tradeoffs and adversarial challenges

1. **Monolith coupling:** Shared release cadence and database load can create correlated failure. Require module ownership, bounded interfaces, dependency rules, migration discipline, resource budgets and performance isolation for expensive AI/reporting tasks.
2. **Single DB contention:** Global hold gating against high-throughput payments can become a hotspot. Prototype per-charity/per-destination gate versioning and a global hold epoch, prove no write skew, benchmark contention before selecting.
3. **Long-running AI:** AI never holds financial DB locks or spans authoritative transactions. AI works on versioned snapshots; rules revalidate immediately before any consequential commit.
4. **Eventual projections:** Never use UI cache, search index, or asynchronous charity projection to authorize a payment.
5. **External API failures:** Independent evidence unavailable means hold, not model-created proof. Provider Unknown is separate from DB commit.
6. **Emergency split-brain:** Protective plane can add holds during outage but cannot clear them independently; reconcile before core resumes release.
7. **Migration safety:** Schema changes are one coordinated release with expand/contract compatibility and rollback plan; never bypass ownership with ad hoc writes.
8. **Internal event ordering:** In-process events are not automatically durable or globally ordered. Critical commands commit transactionally; only noncritical side effects use asynchronous projection/outbox.
9. **Tenant isolation:** Shared process/database requires tenant-scoped authorization and queries, with cross-tenant shared charity facts separately governed.
10. **Scale assumption:** A component monolith is preferred unless reproducible measurements show an unavoidable limit that cannot be addressed with scaling instances, optimized queries, partitioning or background workers; any exception requires explicit ADR and user approval.

## Adversarial schedules (unexecuted)

| ID | Injection | Safety oracle |
|---|---|---|
| ADV-069 | Hold commits between grant approval and payment release | Release observes hold and stops |
| ADV-070 | Hold commits between durable send intent and external provider dispatch | Dispatch rechecks gate and stops if still cancellable |
| ADV-071 | Hold commits after irreversible provider submission | Record timing, no false rollback claim; pursue supported recall |
| ADV-072 | Two concurrent hold/release commands under snapshot isolation | No write skew or post-hold release |
| ADV-073 | Charity hold update conflicts with sponsor-specific grant approval | Global safety remains binding |
| ADV-074 | AI case runs 15 minutes while authority revoked | Revalidation rejects stale AI recommendation |
| ADV-075 | Reporting query starves payment transactions | Workload controls protect critical processing |
| ADV-076 | Internal non-durable event lost after transaction commit | Outbox preserves required external side effect |
| ADV-077 | Old site restarts monolith after failover | Fencing rejects stale writer |
| ADV-078 | Schema migration changes gate interpretation mid-flight | Compatible version-pinned semantics or safe hold |
| ADV-079 | Shared database query accidentally omits tenant predicate | No donor-private cross-tenant exposure |
| ADV-080 | Emergency hold during quorum outage races with recovery | Protective state reconciles before first release |
| ADV-081 | Outbox worker resubmits after provider success but response loss | Stable payment identity, Unknown and no blind retry |
| ADV-082 | Cached charity status newer than grant snapshot but older than global hold | Authoritative gate wins over projection |
| ADV-083 | Payment destination changed concurrently with release | Version binding and serialization prevent stale payment |
| ADV-084 | Two monolith instances concurrently reserve same funds | Atomic capacity invariant holds |

## Acceptance criteria and next research

- All authoritative commands have declared module ownership, required isolation level, versioned policy/evidence inputs, idempotency, audit and outbox effects.
- A formally specified total order or equivalent serialization point exists for hold versus release; no post-hold release can commit in the defined fault model.
- Outbound payment submission has an explicit cancellability/point-of-no-return model and an Unknown state.
- Run theoretical model checking of interleavings; later execute DB concurrency tests, failover tests and provider-specific contract tests before asserting guarantees.
- Benchmark global hold gating and throughput without prematurely introducing microservices.
- Update the 62+24 decision inventory to remove implied service boundaries and align with component ownership.
- Board/counsel must validate sponsor-delegated approvals and common charity reliance; provider and DR assumptions remain unverified.

**Decision:** Component monolith is user-directed. Exact DB, isolation implementation, hosting topology and provider capabilities remain subject to evidence and ADR. No code or simulations have been executed.

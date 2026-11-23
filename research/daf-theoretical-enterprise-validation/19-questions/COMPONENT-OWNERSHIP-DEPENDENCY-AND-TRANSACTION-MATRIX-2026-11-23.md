# Component ownership, dependency and transaction matrix

Date: 2026-11-23. **User-directed component monolith.** Theoretical requirements only; no implementation or tests executed. Companion to REQ-X-020 and ADV-001..ADV-084.

## Non-negotiable design rules

- One coordinated deployable application with strongly encapsulated in-process domain components, one authoritative transactional financial/security core, and no internal microservice network as default.
- One owner per authoritative aggregate, command, schema namespace and invariant. Shared transaction coordinator can invoke component-owned domain operations but cannot silently rewrite their owned state.
- Components expose explicit F# interfaces, typed commands, read contracts and domain events. No cross-component direct table mutation, ad hoc SQL or shared mutable object access.
- Cyclic *domain* dependencies are prohibited. Orchestration lives in an application-command layer; domain components do not call back into their callers. Read dependencies are distinguished from command dependencies.
- Safety-critical gates are synchronous and authoritative. Noncritical projections, notifications, evidence investigation and provider delivery are asynchronous via durable outbox.
- No AI or human exception path may bypass the same domain commands, authority checks, idempotency or financial transaction rules.
- The minimal independently durable emergency protective plane and outage intake are explicit resilience exceptions, not a migration toward microservices.

## Component ownership matrix

| Component | Owns authoritative entities | Public command contract | Dependencies / boundary | Atomicity requirement | Priority |
|---|---|---|---|---|---|
| Identity & Delegation | Principal, verified identity, authority chain, consent, revocation epoch | Identity/authority commands | All transactional command handlers; no cross-component direct writes | Authority revocation must be visible to financial release | P0 |
| Sponsor Governance | Sponsor configuration, policy bundles, DoA, white-label legal terms, publication history | Independently approved policy publication | Grants, Fees, Charity overlay, UI; reads only pinned signed versions | Policy publication and version activation atomic | P0 |
| Common Charity Registry | Canonical charity identity, verification evidence, eligibility epoch, 180-day review, adverse holds | Charity identity/vetting commands | Grant/Payment/Compliance read via eligibility gate; tenant-private data excluded | Global adverse hold and release serialization | P0 |
| Compliance & Protective Holds | Sanctions state, prohibited-benefit findings, protective hold ledger, risk policy | Hold and compliance disposition commands | Grant/Payment/Authority/Charity via mandatory gate | Hold commit and payment release ordered | P0 |
| Grant Management | Recommendation, recurring/future mandate, donor affirmation, grant lifecycle, external grant IDs | Grant submit/evaluate/approve/cancel commands | Identity, Policy, Charity, Compliance, Finance; orchestrates through command boundary | Approval plus reservation atomic | P0 |
| Financial Ledger & Capacity | Authoritative fund balances, journal, capacity, reservations, fees accrued, financial snapshots | Journal/reservation/reversal commands | Grant, Contribution, Payment, Fees, AUM; owns all balance mutations | Reservation, journal and idempotency atomic | P0 |
| Payment Orchestration | Payment intent, destination authorization binding, provider state, Unknown, settlement/return | Prepare/release/reconcile payment commands | Grant, Charity, Compliance, Ledger, Provider adapters | Release authorization, idempotency and outbox atomic | P0 |
| Contribution Intake | Contribution instructions, recurring funding mandate, received/accepted/finality states | Contribution acceptance/reversal commands | Identity, Ledger, Compliance, custody adapters | Recognized funds and ledger journal atomic | P1 |
| AUM & Investments | Mandates, allocations, holdings, valuation provenance, liquidity, custody reconciliation | Investment/allocation/valuation commands | Ledger, Sponsor Policy, custodians; never direct grant mutation | Authoritative allocation/journal mutations atomic | P1 |
| Fees & Billing | Fee schedules interpreted, charge events, manual-work evidence, waivers | Accrue/charge/waive commands | Sponsor Governance, Ledger, Operations | Fee event and ledger effect idempotent/atomic | P1 |
| Donor & Sponsor Administration | Accounts, advisor succession workflow, closures, sponsor setup and operational cases | Administrative case commands | Identity, Governance, Grant, Ledger; no direct financial edits | Closure/authority changes enforce current holds | P1 |
| Evidence & Audit | Evidence provenance, immutable decision trace, source snapshots, legal retention references | Append evidence/audit commands | All domain components append via transactional interface | Critical audit/outbox persisted with domain commit | P0 |
| Integration Adapters | External bank, charity registry, identity and messaging connectors; contract capability metadata | Provider request/response adapters | Payment, Contribution, Charity, AUM; never owns authoritative financial state | Outbox delivery external; reconcile Unknown | P1 |
| API, Widgets & UX | Typed presentation contracts, authenticated sessions, accessibility, branding, status receipts | Submit domain commands, render read models | Domain application facade only; no DB writes | Queued never shown as Approved/Paid | P1 |
| AI Evidence & Exception Workbench | Case investigation, bounded AI tasks, evidence extraction, human queue | Propose evidence, no consequential command authority by default | Evidence, Policy, Compliance, domain read-only views | No DB lock held across model call | P1 |
| Notifications & Reporting | Email/print tasks, donor statements, reconciled analytics and projections | Projection and notification commands | Transactional outbox and versioned read views | Eventually consistent; never payment gate | P2 |
| Recovery & Operations | Writer fencing, quorum health, intake reconciliation, emergency protective channel | Recovery/protective commands | Command pipeline, Compliance, Payment, Ledger | Protective state reconciles before release | P0 |

## Allowed dependency and transaction matrix

| Caller | Callee | Mode | Required contract |
|---|---|---|---|
| Identity & Delegation | Sponsor Governance | Sync read | Check effective delegation and sponsor scope; no circular command invocation |
| Grant Management | Identity / Governance / Charity / Compliance | Sync safety gate | Evaluate current authoritative snapshots and versions |
| Grant Management | Financial Ledger & Capacity | Same DB transaction | Approve + reserve + audit atomically |
| Payment Orchestration | Grant / Charity / Compliance / Identity / Governance | Sync safety gate | Recheck current eligibility, holds, authority and destination |
| Payment Orchestration | Financial Ledger & Capacity / Evidence & Audit | Same DB transaction | Commit release intent, idempotency, journal effect where applicable and outbox |
| Payment Orchestration | Integration Adapters | Async outbox | Dispatch after durable commit, reconcile external Unknown |
| Contribution Intake | Financial Ledger & Capacity | Same DB transaction | Receipt/acceptance and corresponding journal under approved accounting policy |
| AUM & Investments | Financial Ledger & Capacity | Same DB transaction where financial effect | Allocations, units and journal must reconcile |
| Fees & Billing | Financial Ledger & Capacity | Same DB transaction | One service event, one authorized charge |
| Common Charity Registry | Compliance & Protective Holds | Sync safety gate | Global adverse events publish protective state |
| Recovery & Operations | Compliance / Payment / Ledger | Sync reconciliation barrier | Protective holds and revocations before replay |
| AI Evidence & Exception Workbench | Evidence & Audit / domain views | Async read-only | Structured evidence suggestion only; revalidate before domain command |
| Notifications & Reporting | Domain outbox | Async eventual | Never authoritative for release or balances |
| API, Widgets & UX | Application command facade | Sync command/async status | Client never bypasses authority gates |

## Dependency direction and anti-cycle policy

`UI/Widgets/External APIs -> Application Command Facade -> Domain Command Coordinator -> Owned Domain Components -> Owned Persistence Ports`. The coordinator may call multiple components within one transaction. Domain components depend on shared *types/contracts* and narrowly scoped read ports, not each other's implementation or the coordinator. External adapters implement outbound ports. Evidence/Audit is a transactionally participating port. Reporting/notifications subscribe to durable outbox events.

**Important architectural correction:** An apparent cycle such as Grant -> Ledger -> Grant is resolved by coordinator-owned sequencing, not mutual domain callbacks. Ledger validates financial invariants against authoritative records; Grant provides a grant-intent reference and authorization decision. The coordinator commits both under one transaction. Similarly, Compliance must not synchronously call Payment while Payment is evaluating compliance; compliance publishes holds, and the coordinator serializes hold/release commands.

## Data ownership and cross-component read rules

Each component owns a named logical schema/table group, versioned aggregates and migrations. Other components read only through stable query interfaces or authorized immutable snapshots. A shared physical database does not grant universal table access. Critical cross-component reads must participate in a transaction with sufficient isolation and current epoch checks. Read models can lag and must expose source version and freshness; they never authorize payment. Sponsor-specific confidential data remains tenant-scoped, while the common charity evidence record contains only approved shareable charity facts.

## Transaction classes

**T0: single-aggregate authoritative.** Identity change, sponsor policy publication, charity eligibility update, hold publication, payment destination update. Each commits its aggregate, audit and durable outbox atomically.

**T1: multi-component authoritative.** Grant approval + reservation, contribution acceptance + journal, fee charge + journal, payment release intent + idempotency/outbox and applicable accounting effects. One database transaction, one linearizable or otherwise proven invariant-preserving commit boundary.

**T2: external side effect.** Provider payment submission, external charity registry lookup, custody instructions, email. Not part of a DB atomic transaction; durable outbox and provider-specific idempotency/reconciliation needed. A timeout after submission is Unknown, not Failed.

**T3: eventual projection.** Dashboards, statements, notifications, search, AI summaries. Async, rebuildable, never used for mandatory safety gates.

**T4: independent protective/outage plane.** Durable queue and protective holds when authoritative core unavailable; cannot approve/reserve/pay. Reconcile protective state before T0/T1 resume.

## Ownership conflicts to resolve before implementation

1. **Sanctions and charity status:** Common Charity owns canonical identity/eligibility evidence; Compliance owns sanctions disposition and protective holds. Charity status does not independently clear a Compliance hold.
2. **Grant vs ledger:** Grant owns recommendation and approval intent; Ledger owns authoritative reservations and balance. No duplicate source of truth for available capacity.
3. **Grant vs payment:** Grant owns grant lifecycle; Payment owns payment intent, destination binding and provider status. Grant Paid status is a derived outcome from verified payment evidence, not independent editable state.
4. **Sponsor policy vs fee billing:** Governance owns fee schedule authorization; Fees owns service-event billing; Ledger owns financial charge. No sponsor UI direct fee override.
5. **AI vs evidence:** AI owns no verified authoritative facts by assertion. Evidence/Audit records source and provenance; domain owner validates the evidence before promotion.
6. **Recovery vs financial core:** Recovery can fence, hold and reconcile, but cannot directly rewrite balances or release money. Core resumes only after safety gates and quorum health.
7. **Charity registry vs tenant data:** Global reusable charity evidence is distinct from sponsor-specific grant decisions and donor notes.

## Adversarial module-boundary tests (unexecuted)

| ID | Attempt | Required result |
|---|---|---|
| ADV-085 | Grant component writes ledger balance directly | Block via interface/DB ownership; only Ledger command can mutate |
| ADV-086 | Payment component reads stale charity projection | Reject projection as release authority; use current gate |
| ADV-087 | AI case writes sponsor policy to bypass review | No mutation capability; independent policy publication required |
| ADV-088 | Notifications event arrives before payment finality | Message cannot claim Paid without confirmed domain state |
| ADV-089 | Reporting query causes lock starvation on financial core | Workload isolation/bounded queries preserve financial operations |
| ADV-090 | Sponsor configuration bypasses global hold in UI | Backend authoritative gate blocks command |
| ADV-091 | Domain component callback creates Grant/Ledger cycle | Dependency architecture test fails; coordinator owns orchestration |
| ADV-092 | External provider retries after crash and duplicates payment | Stable intent, Unknown reconciliation, no blind retry |
| ADV-093 | DB operator directly edits reservation table | Privilege controls, append-only audit and reconciliation detect/prevent |
| ADV-094 | Charity global evidence update includes donor-private notes | Reject data classification violation; tenant isolation maintained |
| ADV-095 | Emergency plane calls Payment release command | Capability isolation denies; protective-only operations |
| ADV-096 | One component migration breaks another's read contract | Version compatibility tests and migration gates block deployment |

## F# component-contract direction (illustrative only)

Use F# discriminated unions for Known/Unknown/Disputed/Stale, authorized command outcomes and domain state. Distinct typed IDs prevent mixing SponsorId, CharityId, GrantId, PaymentIntentId and ControllingPrincipalId. Public interfaces are domain command/query ports; private persistence types do not leak. Prefer one solution with domain projects/modules and automated architecture tests, not one independently deployed service per component. Do not commit to specific library/DB engine until design ADR and licensing checks.

## Acceptance gates and next steps

- Every authoritative table/aggregate and command has exactly one owning component, named steward and declared public interface.
- Every dependency has a permitted direction; architecture tests reject cycles and forbidden table writes.
- Every cross-component invariant identifies its coordinator, transaction class, isolation requirement, audit evidence and concurrency test.
- All money-moving paths pass the same domain command pipeline, including administrative and break-glass paths.
- Model hold-vs-release, concurrent reservation, payment Unknown and protective replay using component-owned commands before any production claims.
- Measure DB contention, query load and release latency before considering architectural exceptions. A microservice split is not the default remedy.

**Open:** Exact component count/boundaries, transaction isolation strategy, schema enforcement, DR topology, provider proof, sponsor legal approvals and test execution remain unverified.

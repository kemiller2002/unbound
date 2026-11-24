# EVA: Event-driven enterprise architecture hypothesis (addendum §19)

> **Integrator note (2026-11-24).** The adversarial review corrected the volume test: the outbox figure is inconsistent, and daily per-fund fee accrual rows were omitted. It also qualified the "one transactional boundary" claim, because the hold registry spans two databases. See [12](../12-EVENT-ARCHITECTURE-DECISION.md) §1 and XIF-ADV-006, XIF-ADV-007. The text below is the research pass as delivered.

Domain code EVA. Date 2026-11-23. This is requirements research only: nothing was built, benchmarked or executed. Volume figures are arithmetic on cited inputs; assumptions are marked [A].

**Outputs**

| Item | Count |
|---|---|
| Event-matrix rows | 96 (XEV-001..096) |
| Atlas rows | 27 |
| Findings | 23 |
| New requirements | 16 (UNB-XIN-EVA-001..016) |
| Refinements | 4 (RR-UNB-EVT-001-r1, RR-UNB-EVT-005-r1, RR-UNB-DATA-004-r1, RR-UNB-DATA-005-r1) |
| Scenarios | 22 |
| Rejections | 10 |
| Failure modes | 20 |
| Decisions | 7 |
| Conflicts | 6 |

**Hypothesis under test:** "A DAF enterprise can be modeled as a financial and relationship management system, governed by charitable-purpose rules and coordinated through traceable business events."

**Short answer: partially supported.**

- **Where events work.** Traceable business events are the right coordination layer *downstream* of authoritative commits.
- **Where they fail.** They cannot carry invariant-bearing steps. Reservation, hold vs release, bank change vs release, period close and sanctions blocking must commit synchronously in one PostgreSQL transaction.
- **"Relationship management" is the wrong framing.** The sponsor controls the assets; the donor holds an advisory privilege.
- **External truth arrives as files.** Custodian, bank and IRS data come as batch files to reconcile, not as subscribable events.

**Builds on (not repeated here):**
- REQ-X-020/021/022 and the consistency classes
- TX-01..28, INV-01..18, ADV-069..116, SIM-01..28, PAY-01..09
- GR-EVT-001..060 with UNB-GEX-001..040
- UNB-DATA-001..012, UNB-IMM-001..010, UNB-EVT-001..006
- RR-UNB-PLAT-007-r1, RR-REQ-X-020-r1
- RCF-ARC-001..004, RCC-ARC-001, RCD-004, RCD-012

## Volume test: does one PostgreSQL per sponsor with a transactional outbox suffice?

**Inputs**

| Input | Value | Source |
|---|---|---|
| Unbound expected scale | 100k accounts × 8 grants/yr = 800k grants/yr | unbound:research/daf-theoretical-enterprise-validation/15-economics/economic-model.md:104 |
| December surge | k = 3 over 25 of 250 days | economic-model.md:62 |
| Throughput peaks | 10–30× | economic-model.md:90 (TVC-017) |
| Largest US sponsor | Fidelity Charitable, 3.0m grants in CY2025; DAFgiving360, 1.5m | daf-landscape:research/daf-grant-exception-unit-economics-evidence-and-sensitivity-2026-11-12.md:8-9 |
| Rows per grant lifecycle | ~25 [A] | Derived from the GR-EVT catalog and TX matrix |
| Authoritative transactions per grant | ~8 [A] | Derived |

**Results (one sponsor database)**

| Scenario | Grants/day | Authoritative tx/s | Rows/s |
|---|---|---|---|
| Unbound expected, average day | 3,200 | ~0.9 over 8 h | ~3 |
| Unbound, December day | 8,000 | ~2.2 | ~7 |
| Fidelity scale, December day | 30,000 | ~8 | ~26 |
| Fidelity scale, year-end spike (5× December [A]) | 150,000 | ~85 (incl. contributions [A]) | ~260 |
| Fidelity scale, 30× burst | 12.5 grants/s | ~200 | ~620 |

Outbox load at the Fidelity spike is about 26 messages/s. Storage is about 75–150 GB/yr at Fidelity scale.

**Interpretation**
- A modest managed PostgreSQL primary sustains thousands of short write transactions per second. This is unverified general industry knowledge and must be benchmarked (UNB-IMM-010, UNB-TEN-018, UNB-XIN-EVA-013).
- `FOR UPDATE SKIP LOCKED` is the documented pattern for queue-like tables (https://www.postgresql.org/docs/16/sql-select.html, accessed 2026-11-23).

**Conclusion.** A broker is unnecessary by one to two orders of magnitude. The real risks are not throughput:
- Hot-row contention. Use per-recipient hold rows and read a monotonic version; do not lock one global row.
- Long transactions. No AI or external call may run inside a transaction.
- Reporting load starving transaction processing (ADV-075).

## Architectural decision record

**ADR-EVA-001: Hybrid authoritative-ledger architecture with event-driven workflow coordination inside a component monolith.** Status: **Proposed** (decision XID-EVA-001).

### Context

- Twelve capabilities must coordinate.
- The owner has directed a component monolith (REQ-X-020), F#, a WASM-first UI and managed persistence (UNB-TEC-002, UNB-PLAT-006).
- Strata is a schema-governance CLI, not an event store (RCF-ARC-001).
- Existing requirements already demand:
  - append-only history (UNB-DATA-001)
  - bitemporal reconstruction (UNB-DATA-003, UNB-IMM-004)
  - an outbox (UNB-EVT-002)
  - atomic approval-with-reservation (TX-11)
  - a hold/release serialization point (RR-REQ-X-020-r1)

### Decision

1. **Authority lives in typed tables in one PostgreSQL database per legal sponsor**, confirming RCD-004. The tables fall into four classes:
   - **Financial.** An insert-only double-entry journal plus reservations. **The journal is the financial event log.** There is no event store beneath it, and no balance is derived from any other log (UNB-XIN-EVA-001).
   - **Class 1 (domain facts).** Per-module append-only fact tables. Current state is updated in the same transaction and can be rebuilt from the facts (RR-UNB-DATA-004-r1).
   - **Class 2 (engagement/CRM).** Mutable, with system-versioned history (UNB-XIN-EVA-014).
   - **Class 3 (coordination).** Outbox, process-manager rows, leases and queue assignment. Mutable and audited, but not authoritative.

   AI observations live in a separate schema that only the AI role may write (RR-UNB-EVT-005-r1, UNB-XIN-EVA-016).
2. **Invariant-bearing commands are synchronous.** They use a deterministic lock order, or SERIALIZABLE isolation with retry on SQLSTATE 40001 (https://www.postgresql.org/docs/16/transaction-iso.html, accessed 2026-11-23).
3. **Everything downstream is coordinated by events.** Events are written to a transactional outbox in the same commit.
   - In-process process managers (persisted rows with timers; UNB-XIN-EVA-007) drive long-running workflows.
   - Projections, notifications, published partner events and AI work consume events at least once, deduplicated by business idempotency keys (UNB-XIN-EVA-005).
   - Every event declares an authority class (RR-UNB-EVT-001-r1) and carries a correlation root (UNB-XIN-EVA-006).
4. **External truth enters as completeness-checked ExternalObservation batches** (UNB-XIN-EVA-004). Reconciliation commands create facts; ingestion does not.
5. **Reports come only from sealed snapshots** bound to a ledger commit watermark (UNB-XIN-EVA-003).
6. **Strata declares and drift-checks** the tables, privileges, row-level security and constraints that enforce these classes (RR-UNB-PLAT-007-r1).
7. **No broker, separately deployed services, workflow engine or CDC** unless the UNB-XIN-EVA-013 thresholds are met. The protective plane stays under RCD-012; this analysis found nothing at these volumes that needs it.

### Alternatives

- A. Traditional modular
- B. Event-driven modular monolith
- C. Event-sourced financial core
- D. Fully event-sourced
- F. Distributed services with a broker (reference)

Rejected ideas: XRJ-EVA-001..010.

### Consequences

**Positive**
- One transactional boundary for every invariant.
- Balances and privileges enforced by the database.
- Deterministic reports.
- Recovery from rows, with no added infrastructure.

**Negative**
- Process managers are hand-written F# state machines.
- One database per sponsor multiplies migrations (Strata has no schema templating).
- Payment release reads a cross-database hold registry, so registry unavailability blocks releases.
- The outbox needs claim-and-mark delivery. PostgreSQL sequences have gaps (https://www.postgresql.org/docs/16/functions-sequence.html), and commit order can differ from id order (an inference, not a quote).

### Links

- **Confirms:** REQ-X-020/021/022, REQ-X-057, UNB-EVT-002, UNB-DATA-003, RR-UNB-PLAT-007-r1, RR-REQ-X-020-r1, RCD-004, RCD-012, UNB-RCN-FIN-009, UNB-TEN-019.
- **Refines:** UNB-EVT-001, UNB-EVT-005, UNB-DATA-004, UNB-DATA-005.
- **Contradicts:**
  - XIC-EVA-001: the single EventStore in TENANT-AWARE-PERSISTENCE-DESIGN.md vs erasure
  - XIC-EVA-002: a literal reading of UNB-DATA-001 vs mutable outbox state
  - XIC-EVA-003: the Echelon generator's asynchronous approval coupling (recorded only; Echelon not modified)
  - XIC-EVA-004: GR-EVT vs TVR-EVT-001 event naming
  - XIC-EVA-005: Strata in an event-store role
  - XIC-EVA-006: statements classed T3

### Falsification criteria (any one reopens the ADR)

1. A benchmark shows the largest sponsor's modelled peak needs more than 50% of sustainable commit capacity after tuning, or p99 reservation latency exceeds 500 ms.
2. p99 outbox lag exceeds 60 s on three consecutive peak days after tuning.
3. Contract or regulation requires dedicated infrastructure for a material share of sponsors, so a sponsor's transactions no longer fit in one database.
4. An auditor needs to reconstruct a non-financial module's past state and class-2 history cannot answer. That module alone moves to class 1.
5. Model checking finds an invariant interleaving that cannot be serialized without unacceptable contention.
6. Long-running workflow types exceed about 50 [A].

## Event ownership and authority matrix

`event-matrix.jsonl` holds 96 events. GR-EVT names are reused, and TVR-EVT-001 names are crosswalked to them.

| Capability | Events | Auth.-financial | Auth.-nonfinancial | Derived | Informational | Observation-AI |
|---|---|---|---|---|---|---|
| Contributions | 9 | 6 | 2 | 1 | 0 | 0 |
| Grants and payments | 25 | 11 | 8 | 2 | 4 | 0 |
| Investments | 9 | 4 | 3 | 0 | 2 | 0 |
| Accounting | 9 | 7 | 0 | 1 | 1 | 0 |
| Charity verification | 8 | 0 | 6 | 0 | 2 | 0 |
| Customer service | 4 | 0 | 4 | 0 | 0 | 0 |
| Compliance | 7 | 1 | 6 | 0 | 0 | 0 |
| Relationship management | 7 | 0 | 6 | 0 | 1 | 0 |
| Fees and billing | 4 | 4 | 0 | 0 | 0 | 0 |
| Communications | 5 | 0 | 2 | 1 | 2 | 0 |
| Operational reporting | 4 | 0 | 0 | 2 | 2 | 0 |
| AI automation | 5 | 0 | 1 | 0 | 1 | 3 |

**Q1. Which events are authoritative?** Any event that changes money, capacity, authority, eligibility or legal status:
- journal batches and reversals
- prior-period adjustments
- reservations
- contribution acceptance and liquidation tranches
- payment authorization, settlement, return, partial return and void
- sanctions blocks
- fee accruals
- period close and reopen
- holds
- authority epoch and successor activation
- eligibility evaluations and destination verifications
- policy versions
- issued statements and acknowledgments

**Q2. Which are informational?**
- provider acknowledgments before confirmation
- custodian, bank and IRS file ingestion
- charity source changes
- notification delivery
- interaction logs
- projection rebuilds
- AI context retrieval

An informational event may trigger a command, but that command re-reads authoritative state.

**Q3–Q5. Which systems own which truth?**

| Truth | Owner | Store |
|---|---|---|
| Balances, reservations, capacity, financial reports | Financial ledger and capacity | Sponsor journal (UNB-XIN-EVA-001) |
| Payment knowledge states | Payment orchestration | Payment facts (REQ-X-083) |
| Advisory privilege, successors, contributor of record, anonymity, consent | Identity and delegation | Append-only authority facts (UNB-XIN-EVA-014) |
| Engagement and CRM data | Donor and sponsor administration | Mutable with history; never authorizes money |
| Policy decisions | Sponsor governance (versions); deterministic rules plus authorized humans (decisions) | Signed versions plus decision trace (REQ-X-017/018) |
| Charity eligibility and global holds | Common charity registry and compliance | Platform database with HoldPublicationVersion (RR-REQ-X-020-r1) |

## Consistency and failure-recovery requirements

**Q11. Operations that must be synchronous** (one transaction, or a version check at commit):

| Invariant | Mechanism | IDs |
|---|---|---|
| Approval plus reservation | Lock fund rows in deterministic order; recompute capacity inside the transaction | TX-11, UNB-RCN-FIN-009 |
| Hold vs release and dispatch | Check HoldPublicationVersion at release; dispatcher re-checks; evaluated per item | RR-REQ-X-020-r1, UNB-XIN-EVA-011 |
| Payee bank change vs release | Release pins the destination version | INV-11, TX-19 |
| Sanctions block | Blocked-property posting in the same transaction | TVCTL-13, UNB-XIN-EVA-010 |
| Period close vs posting | Posting takes FOR SHARE on the period row; close takes FOR UPDATE. A late posting becomes a prior-period adjustment | UNB-XIN-EVA-002 |
| Authority change vs command | Authority epoch checked at commit | TX-25, UNB-XIN-EVA-012 |
| Fee accrual vs capacity | Posted in the same transaction | TX-20 |
| Recurring occurrence | Unique occurrence key | TX-17 |
| Conflicting instructions | Conflict-scope check inside the transaction | UNB-XIN-EVA-012 |

**Q12. Where eventual consistency is acceptable**
- Notifications, with revalidation at send time (UNB-XIN-EVA-015)
- Search
- Provisional dashboards
- AI summaries
- Published partner events
- CRM segments
- Service metrics
- Scheduling of charity re-evaluation

**Q6. How events are correlated.** Every record carries `correlation_root`, `causation_id`, typed `business_keys` and `parent_root` (UNB-XIN-EVA-006). The same envelope applies to cases, AI observations, notices and external items, so an exception dossier is a graph query.

**Q7. Duplicate, delayed, missing and out-of-order events**

| Problem | Handling |
|---|---|
| Duplicate | Business key plus payload hash. Same key with a different payload opens a conflict case (UNB-XIN-EVA-005). |
| Delayed | Bitemporal facts. A late fact dated in a closed period becomes a prior-period adjustment (UNB-XIN-EVA-002). |
| Missing | Expected-file calendar, control totals and watermarks; dependent automation is held (UNB-XIN-EVA-004). Database sequence gaps are not data loss (UNB-XIN-EVA-007). |
| Out of order | Per-aggregate versions. External evidence follows monotonic knowledge with explicit reversal, never last-message-wins. |
| Unknown schema version | Authoritative consumers halt and quarantine the event (UNB-XIN-EVA-008). |

**Q8. Correcting transactions without destroying audit history**
- Linked reversal plus re-post (UNB-ACC-003, UNB-IMM-003).
- Every financial fact carries effective_at, recorded_at and a posting period.
- Period locks, with prior-period adjustments for late items.
- As-issued artifacts are kept and amended, never replaced.
- A deterministic correction-impact registry (RR-UNB-DATA-005-r1).

**Q9. Tenant isolation**
- One database per sponsor, with partner schemas.
- The envelope's tenant is verified against the connection (UNB-TEN-004).
- AI retrieval runs under a verified tenant context (UNB-RCN-PLT-004).
- Shared charity and hold data has no donor fields.
- A sponsor-to-sponsor move is a legal transfer, not a tenant migration (UNB-XIN-EVA-009).

**Q10. Keeping AI observations from becoming authoritative records**
- Observation, Proposal, Command and Fact are distinct types, and Fact constructors are private.
- The AI database role can only insert into `ai_observation`.
- Promoting evidence requires a deterministic validator or an authorized human.

**Q13. Workflow recovery**
- Transactional outbox.
- Durable process-manager rows with timers, idempotent steps, and stall detection that routes to a human-resolution queue (UNB-XIN-EVA-007).
- Compensation only for external effects.
- No blind retry when an outcome is Unknown.
- Poison messages quarantined.

**Q14. Reporting without inconsistent financial results.** Reports read only `FinancialSnapshotSealed` (period plus watermark). The same watermark always yields the same figures. Live figures are labelled provisional (UNB-XIN-EVA-003).

## Alternative architecture comparison

Scores are 1–5. For skills and infrastructure, 5 means low demand or a small footprint. "Fit" means fit to F#, WASM-first UI, managed PostgreSQL and minimal infrastructure.

| Criterion | A Traditional | B Event-driven monolith | C Event-sourced financial core | D Fully event-sourced | **E Hybrid (chosen)** | F Distributed + broker |
|---|---|---|---|---|---|---|
| Simplicity | 5 | 4 | 3 | 1 | 4 | 1 |
| Reliability | 3 | 4 | 4 | 3 | 5 | 2 |
| Testability | 4 | 4 | 4 | 3 | 5 | 2 |
| Ops efficiency | 3 | 4 | 3 | 2 | 4 | 1 |
| Auditability | 2 | 3 | 5 | 5 | 5 | 3 |
| Correction handling | 2 | 3 | 4 | 3 | 5 | 2 |
| Tenant migration | 3 | 3 | 3 | 2 | 4 | 2 |
| Skills (5 = low) | 5 | 4 | 3 | 1 | 4 | 1 |
| Infrastructure (5 = small) | 5 | 5 | 4 | 3 | 5 | 1 |
| Fit | 4 | 4 | 3 | 2 | 5 | 1 |
| **Total (/50)** | 36 | 38 | 36 | 25 | **47** | 16 |

**Basis for the scores**
- **A. Traditional.** Familiar CRUD. But dual writes, row diffs rather than intent, and in-place edits are tempting. It conflicts with UNB-DATA-001.
- **B. Event-driven monolith.** The outbox fixes dual writes. But state stays mutable and events are only informational.
- **C. Event-sourced financial core.** Strong core history. But it creates two financial logs (journal and stream) with no defined winner, and replay could re-post. No Echelon or Strata event store exists (RCF-ARC-001).
- **D. Fully event-sourced.** Streams, projections and upcasters everywhere. Invariants spanning aggregates become eventual, and it collides with erasure of personal data.
- **E. Hybrid.** Every invariant commits in one transaction, and process managers resume after failure. Audit comes from the journal, decision traces and bitemporal facts. Corrections use reversal and re-post with period locks and the impact registry. Migration is per sponsor database. Skills needed: SQL plus F# state machines.
- **F. Distributed + broker.** Invariants cross services, contradicting REQ-X-021, and it needs a broker plus an orchestrator.

**Why not C.** The journal already is an append-only, balanced, causal log. Event-sourcing the core adds a second log with no defined winner (XIF-EVA-003) and a risk of re-posting on replay. This contradicts the daf-landscape "event-sourced ledger" recommendation (daf-landscape:research/mission-2026-11-14/strategic-synthesis.md:133). E meets that recommendation's goals without the second log. The landscape's own review warned against prescribing event sourcing as a mechanism (review-arch-econ.md:104).

## Adversarial validation

Each scenario below is an attempt to disprove E.

| # | Scenario | Handled? | What breaks or remains at risk | Requirement | Scenario ID |
|---|---|---|---|---|---|
| 1 | Misposted contribution found after statements and close | Yes | The wrong fund may already have granted; who bears the shortfall is policy | UNB-XIN-EVA-002, RR-UNB-DATA-005-r1, UNB-XIN-EVA-003 | XDS-EVA-001 |
| 2 | Security valued at the wrong date | Yes, with human steps | Acknowledgment amendment; units issued at the wrong strike | RR-UNB-DATA-005-r1 | XDS-EVA-002 |
| 3 | Charity revoked between approval and payment | Yes, once the hold is committed | **Not handled** if the revocation is not yet ingested or the payment is already transmitted | RR-REQ-X-020-r1, UNB-XIN-EVA-004 | XDS-EVA-003 |
| 4 | ACH R03 return after 3 days | Yes | No end-to-end ACH key | UNB-XIN-EVA-005, UNB-XIN-EVA-015 | XDS-EVA-004 |
| 5 | Check uncashed for 180 days | Yes | Needs durable timers | UNB-XIN-EVA-007 | XDS-EVA-005 |
| 6 | In-kind liquidation in tranches, plus a custodian correction | Yes | Naive deduplication drops the correction | UNB-XIN-EVA-005/007 | XDS-EVA-006 |
| 7 | Wire returned net of fees | Yes (accounting) | Who bears the fee is undecided | XID-EVA-003 | XDS-EVA-007 |
| 8 | One of three liquidation legs fails | Yes | Grant is held | UNB-XIN-EVA-006/007 | XDS-EVA-008 |
| 9 | OFAC hit mid-batch | Yes, only if a batch carries no authority | Batch-level approval breaks it | UNB-XIN-EVA-011 | XDS-EVA-009 |
| 10 | Legal hold or dispute freeze | Yes, with typed holds | A generic hold over-blocks | UNB-XIN-EVA-010 | XDS-EVA-010 |
| 11 | 10k funds move to another sponsor | Yes, as legal transfers | **Breaks** if run as a tenant migration | UNB-XIN-EVA-009 | XDS-EVA-011 |
| 12 | Sponsor leaves and returns; event schema drift | Partially | An unknown forked version halts the stream (intended) | UNB-XIN-EVA-008 | XDS-EVA-012 |
| 13–15 | Co-advisors give opposite instructions; standing vs one-off instruction; successor notice arrives while a grant is in flight | Yes | Needs scope keys, policy defaults and the epoch check | UNB-XIN-EVA-012/006 | XDS-EVA-013..015 |
| 16 | AI prompt injection writes "cleared" | Yes | Fails without database-role separation | RR-UNB-EVT-005-r1, UNB-XIN-EVA-016 | XDS-EVA-016 |
| 17–19 | Missing feed; report run during posting; posting vs close race | Yes | Needs feed calendar, snapshots and period lock | UNB-XIN-EVA-004/003/002 | XDS-EVA-017..019 |
| 20 | A gate keyed on a provider acknowledgment | Yes, via a CI check | Needs the authority field | RR-UNB-EVT-001-r1 | XDS-EVA-020 |
| 21 | Personal-data erasure request | Yes, for class 2 | Fully event-sourced variants fail | RR-UNB-DATA-004-r1, UNB-XIN-EVA-014 | XDS-EVA-021 |
| 22 | Someone proposes a broker | Rejected without evidence | — | UNB-XIN-EVA-013 | XDS-EVA-022 |
| 23 | Outbox id 501 commits after id 502 | Yes, with claim-and-mark | An id-watermark cursor would skip it | UNB-XIN-EVA-007 | — |

**Residual risks no architecture removes**
- External facts not yet ingested.
- Settlement that cannot be reversed.
- The same policy-pack misconfiguration replicated across sponsors.
- Year-end hot-row contention, which has not been measured.

## Where shared abstractions become harmful

| Abstraction | Harm | Instead |
|---|---|---|
| One generic Event / EventStore table | Mixes authority classes; cannot grant AI-only privileges; cannot enforce balancing; duplicates the journal | UNB-XIN-EVA-016, UNB-XIN-EVA-001 |
| Generic Case with financial semantics | Closing a case is mistaken for fixing the money | Typed commands kept separate from cases (UNB-GEX-025) |
| Generic Hold | A records hold blocks grants; a freeze blocks inflows; an OFAC block is treated as a release back to the fund | UNB-XIN-EVA-010 |
| Batch that carries approval | A sanctions hit mid-batch is ignored | UNB-XIN-EVA-011 |
| Universal bus coupling reporting to the write model | Reports depend on timing | UNB-XIN-EVA-003 |
| Event schemas as the public API | Partners bind to internal states | UNB-XIN-EVA-008 |
| Shared Party record for CRM and authority | CRM data authorizes actions; erasure becomes impossible | UNB-XIN-EVA-014 |
| Single linear payment status (Echelon `PaymentRecorded`) | A payment shows as paid on acknowledgment | REQ-X-083 |
| "Recorded" suffix on every generated event | Hides the event's authority | RR-UNB-EVT-001-r1 |
| Tenant-migration tooling used for a sponsor transfer | The funds end up on the wrong legal books | UNB-XIN-EVA-009 |
| Database sequence used as a watermark | Gaps read as loss; late commits are skipped | UNB-XIN-EVA-007 |

## Hypothesis verdict

**Partially supported.**

**Supported.** All twelve capabilities can share typed, traceable, at-least-once events from a transactional outbox, consumed by in-process process managers and projections. Volumes are one to two orders of magnitude below broker territory.

**Refuted:**
1. Steps that carry invariants are synchronous, not event-coordinated: reservation, hold vs release, payee change, sanctions block, period close, authority epoch and conflicting instructions.
2. "Relationship management" misstates the legal structure. CRM data must not authorize money.
3. External truth arrives as batches to reconcile, not as events.
4. Charitable-purpose rules are synchronous gates with pinned versions and sponsor or human discretion, not reactions to events.

**Reformulated hypothesis:**

> A DAF enterprise can be modeled as a set of authoritative records of sponsor-controlled charitable assets — a sponsor-entity double-entry ledger, versioned policy and authority decisions, and orthogonal contribution and payment state machines bound to external evidence — committed synchronously in one transactional store per legal sponsor. Every downstream activity (workflow progression, communications, reporting, relationship engagement and AI assistance) is coordinated through traceable, typed, at-least-once business events derived from those commits. External financial truth enters through completeness-checked reconciliation, and no event, projection or AI observation can by itself create or change an authoritative record.

## Automation challenge

- **Deterministic:** deduplication, completeness checks, period locks, holds, impact enumeration, outbox, process managers.
- **AI-assisted:** dossier summaries over the correlation graph, feed-gap diagnosis, drafts of amendment letters, classification proposals.
- **AI never:** posts, promotes evidence, releases holds, chooses between advisors, or marks a source complete.
- **Cost constraint:** per-grant AI must cost less than about $0.31 (economic-model.md §4), so event-triggered AI should run only on exceptions.

## Business-model applicability

The core is universal. Model-specific needs:

| Requirement | Sponsor models |
|---|---|
| UNB-XIN-EVA-009 (bulk transfer) | Institutional, white-label, outsourced-admin, advisor-distribution |
| UNB-XIN-EVA-008 (published events) | Embedded, white-label, advisor-distribution, institutional |
| UNB-XIN-EVA-012 (conflicting instructions) | HNW-family-office, D2C, advisor-distribution, institutional |
| Benchmark thresholds | Approached only by a Fidelity-scale high-volume-low-balance sponsor |

## Evidence not obtained

- No PostgreSQL benchmark; database capacity is unverified general knowledge.
- No hourly year-end arrival data; the 5× spike is an assumption.
- No contribution counts for large sponsors.
- No bank-partner Nacha return specification.
- No custodian file or correction-indicator specifications.
- No legal opinions on XID-EVA-003..005 (bulk sponsor transfer, scope of a dispute freeze, who bears fees on returned wires).
- Fund-administration NAV-error practice is low confidence.
- The claim that commit order can differ from id order is an inference. The PostgreSQL pages actually read were transaction isolation, SELECT / SKIP LOCKED, and sequence functions.

# 12: Event-driven enterprise architecture hypothesis (addendum §19)

**Hypothesis tested:** "A DAF enterprise can be modeled as a financial and relationship management system, governed by charitable-purpose rules and coordinated through traceable business events."

**Verdict: partially supported.** A reformulated hypothesis is given in §8. The full research pass is in [industry/eva.md](industry/eva.md): volume arithmetic, a scored comparison with justification for every cell, and 23 adversarial scenarios. The adversarial review's challenge to this ADR is in [14](14-ADVERSARIAL-REVIEW.md).

## 1. Architectural decision record

**ADR-EVA-001: Hybrid authoritative-ledger architecture with event-driven workflow coordination inside a component monolith.**
**Status:** Proposed. Adoption is decision XID-EVA-001.

### Context

- Twelve capabilities must coordinate: contributions, grants, investments, accounting, charity verification, customer service, compliance, relationship management, fees and billing, communications, operational reporting and AI automation.
- The owner has directed an F# component monolith with WASM-first UI and managed persistence (REQ-X-020, UNB-TEC-002, UNB-PLAT-006).
- Strata is a PostgreSQL schema-governance tool, not an event store (RCF-ARC-001).
- The baseline already requires:
  - append-only history (UNB-DATA-001);
  - bitemporal reconstruction (UNB-DATA-003);
  - an outbox (UNB-EVT-002);
  - atomic approval-with-reservation (TX-11);
  - a hold/release serialization point (RR-REQ-X-020-r1).

### Volume test

| Scenario (one sponsor database) | Grants/day | Authoritative tx/s | Rows/s |
|---|---|---|---|
| Unbound expected, average day (100k accounts × 8 grants/yr) | 3,200 | ~0.9 over 8 h | ~3 |
| Unbound, December day (k = 3) | 8,000 | ~2.2 | ~7 |
| Fidelity scale (3.0m grants, CY2025), December day | 30,000 | ~8 | ~26 |
| Fidelity scale, year-end spike (5× December [A]) | 150,000 | ~85 | ~260 |
| Fidelity scale, 30× burst | 12.5 grants/s | ~200 | ~620 |

Inputs:
- economic-model.md:29, :62, :90, :104;
- daf-landscape grant-exception unit-economics file :8–9;
- about 25 rows and about 8 authoritative transactions per grant lifecycle [A].

**Corrections from the adversarial review (XIF-ADV-006):**
- The research pass gave outbox load at the spike as about 26 messages/s. That contradicts its own ~85 tx/s. Grants alone give 150,000 × 8 / 28,800 ≈ 42 tx/s, so the outbox is of the same order as authoritative transactions, not lower.
- The table **omits the largest row generator: daily per-fund fee accrual and income attribution.** At 100k accounts this is about 110m rows/yr, against about 20m from grants. At 1.42m accounts it is about 1.5bn rows/yr.
- The no-broker conclusion still holds, because these are batch writes. Storage, vacuum and partitioning become first-order concerns, so:
  - compute fee accruals daily but post them at assessment (XID-ADV-007, XIC-ADV-009);
  - capacity = journal balance − reservations − accrued-unposted fees.

Conclusion:
- At these volumes a broker is unnecessary by one to two orders of magnitude.
- The real risks are hot-row contention, long transactions and reporting load.
- Database capacity has **not been benchmarked**. That is the first falsification test.

### Decision

1. **Authority lives in typed tables in one PostgreSQL database per legal sponsor.** This confirms RCD-004. The financial store is an insert-only double-entry journal plus reservations. **The journal is the financial event log.** No event store sits beneath it, and no balance is derived from any other log (UNB-XIN-EVA-001). Other records fall into three storage classes (RR-UNB-DATA-004-r1):
   - *Class 1:* append-only domain facts, with current state updated in the same transaction.
   - *Class 2:* engagement and CRM data, mutable with system-versioned history and erasable.
   - *Class 3:* outbox, process-manager rows, leases and queue assignment. These are mutable, audited and non-authoritative.
2. **AI observations live in a separate schema** that only the AI database role can write. Promoting an observation to evidence requires a deterministic validator or an authorized human (RR-UNB-EVT-005-r1, UNB-XIN-EVA-016).
3. **Invariant-bearing commands are synchronous.** They use a deterministic lock order, or SERIALIZABLE isolation with retry on SQLSTATE 40001.
4. **Everything downstream is coordinated by events.** Events are written to a transactional outbox in the same commit. Consumers include:
   - in-process process managers, persisted with timers (UNB-XIN-EVA-007);
   - projections;
   - notifications, revalidated at send time (UNB-XIN-EVA-015);
   - published partner events, versioned and kept separate from private events (UNB-XIN-EVA-008);
   - AI work.

   Every consumer receives at least once and is keyed by a business idempotency key (UNB-XIN-EVA-005). Every event declares an authority class (RR-UNB-EVT-001-r1) and carries one envelope (UNB-XIN-SAS-005 + UNB-XIN-EVA-006, merged per XIC-INT-001).
5. **External truth enters as completeness-checked observation batches.** Sources include bank, custodian, processor and IRS files (UNB-XIN-EVA-004). Facts are created only by reconciliation commands, never by ingestion.
6. **Reports come only from sealed snapshots** bound to a ledger watermark (UNB-XIN-EVA-003).
7. **No broker, separately deployed services, workflow engine or CDC** unless the measured thresholds in UNB-XIN-EVA-013 are met.

### Alternatives considered

A traditional modular; B event-driven modular monolith; C event-sourced financial core; D fully event-sourced; E hybrid (chosen); F distributed services with broker and sagas (reference). See §5.

### Consequences

**Positive**
- Every invariant *within a sponsor* has one transactional boundary. **Exception (XIF-ADV-007):** hold vs release spans the platform hold registry and the sponsor database. It relies on a version check at commit (HoldPublicationVersion), not a shared transaction, and an unavailable registry stops every sponsor's releases. That is a fail-closed single point of failure, and it needs its own availability objective.
- The database enforces balances and privileges.
- Reports are deterministic.
- Recovery works from rows, with no added infrastructure.

**Negative**
- Process managers are hand-written F# state machines.
- One database per sponsor multiplies migrations, and Strata has no schema templating.
- Payment release reads a cross-database hold registry, so registry unavailability blocks releases.
- The outbox needs claim-and-mark delivery. Sequences have gaps, and commit order can differ from id order (an inference).

### Links

- **Confirms:** REQ-X-020, REQ-X-021, REQ-X-022, REQ-X-057, UNB-EVT-002, UNB-DATA-003, RR-UNB-PLAT-007-r1, RR-REQ-X-020-r1, RCD-004, RCD-012, UNB-RCN-FIN-009, UNB-TEN-019.
- **Refines:** RR-UNB-EVT-001-r1, RR-UNB-EVT-005-r1, RR-UNB-DATA-004-r1, RR-UNB-DATA-005-r1.
- **Contradicts:**
  - XIC-EVA-001: the single EventStore in TENANT-AWARE-PERSISTENCE-DESIGN conflicts with erasure.
  - XIC-EVA-002: a literal reading of UNB-DATA-001 forbids mutable outbox state.
  - XIC-EVA-003: the Echelon generator's asynchronous approval coupling. Recorded only; Echelon was not modified.
  - XIC-EVA-004: GR-EVT naming vs TVR-EVT-001 naming.
  - XIC-EVA-005: Strata cast in an event-store role.
  - XIC-EVA-006: statements classed as T3.
- **Against the landscape:** this also contradicts the `daf-landscape` recommendation of an "event-sourced ledger" (strategic-synthesis.md:133). Its goals are met without a second log.

### Falsification criteria (any one reopens the ADR)

1. A benchmark shows the largest sponsor's modelled peak needs more than 50% of sustainable commit capacity after tuning, or p99 reservation latency exceeds 500 ms.
2. p99 outbox lag exceeds 60 s on three consecutive peak days.
3. Contract or regulation requires dedicated infrastructure for a material share of sponsors.
4. An auditor needs past-state reconstruction for a non-financial module that class-2 history cannot answer. In that case only that module moves to class 1.
5. Model checking finds an invariant interleaving that cannot be serialized without unacceptable contention.
6. Long-running workflow types exceed about 50 [A].

## 2. Event ownership and authority matrix

`data/event-matrix.jsonl` has 96 events (XEV-001..096) covering all twelve capabilities. Each event records its owning module, authority class, source-of-truth store, consistency, ordering scope, idempotency key, correction pattern, tenant scoping, retention class and consumers. Existing GR-EVT names are reused, and TVR-EVT-001 names are crosswalked. Gate D4 enforces two rules: an AI observation is never stored in a ledger, and it is never synchronous.

<!-- BEGIN GENERATED: event-summary -->
| Capability | authoritative-financial | authoritative-nonfinancial | derived | informational | observation-ai | synchronous | eventual |
|---|---|---|---|---|---|---|---|
| contributions | 6 | 2 | 1 |  |  | 8 | 1 |
| grants | 11 | 8 | 2 | 4 |  | 18 | 7 |
| investments | 4 | 3 |  | 2 |  | 7 | 2 |
| accounting | 7 |  | 1 | 1 |  | 8 | 1 |
| charity verification |  | 6 |  | 2 |  | 6 | 2 |
| customer service |  | 4 |  |  |  | 1 | 3 |
| compliance | 1 | 6 |  |  |  | 6 | 1 |
| relationship management |  | 6 |  | 1 |  | 5 | 2 |
| fees and billing | 4 |  |  |  |  | 4 |  |
| communications |  | 2 | 1 | 2 |  | 2 | 3 |
| operational reporting |  |  | 2 | 2 |  |  | 4 |
| AI automation |  | 1 |  | 1 | 3 | 1 | 4 |
<!-- END GENERATED: event-summary -->

### Answers to the architecture questions

| # | Question | Answer | Requirements |
|---|---|---|---|
| 1 | Authoritative events | Any event that changes money, capacity, authority, eligibility or legal status. Examples: journal batches and reversals, prior-period adjustments, reservations, contribution acceptance and liquidation tranches, payment authorization/settlement/return/partial return/void, sanctions blocks, fee accruals, period close/reopen, holds, authority epoch and successor activation, eligibility evaluations, destination verifications, policy versions, issued statements and acknowledgments. | event matrix; RR-UNB-EVT-001-r1 |
| 2 | Informational events | Provider acknowledgments before confirmation; ingestion of custodian, bank and IRS files; charity source changes; notification delivery; interaction logs; projection rebuilds; AI context retrieval. These may trigger a command, but the command re-reads authoritative state. | RR-UNB-EVT-001-r1 |
| 3 | Financial truth | The sponsor journal and reservations (Financial Ledger & Capacity). Payment knowledge states belong to Payment Orchestration (REQ-X-083). | UNB-XIN-EVA-001 |
| 4 | Customer and relationship truth | *Authority facts* (advisory privilege, successors, contributor of record, anonymity, consent) are append-only in Identity & Delegation. *Engagement data* (CRM) is mutable with history and never authorizes money. | UNB-XIN-EVA-014, UNB-XIN-CRM-001 |
| 5 | Policy decisions | Sponsor Governance owns signed policy versions. Decisions are made by deterministic rules or authorized humans and recorded with a decision trace. Discretionary holds cite board-approved criteria. | REQ-X-017, REQ-X-018, UNB-XIN-NPG-006, UNB-XIN-GOV-001 |
| 6 | Correlation | `correlation_root`, `parent_root`, `causation_id`, typed `business_keys` and the tenant envelope, carried on facts, cases, AI observations, notices and external items. | UNB-XIN-EVA-006, UNB-XIN-SAS-005 |
| 7 | Duplicate, delayed, missing and out-of-order events | **Duplicates:** business key plus payload hash; a mismatched payload opens a conflict case. **Delayed:** bitemporal facts and prior-period adjustments. **Missing:** an expected-file calendar, control totals and a watermark; dependent automation is held, and sequence gaps are not treated as loss. **Out of order:** per-aggregate versions and monotonic knowledge with explicit reversal. **Unknown schema:** the event is halted and quarantined. | UNB-XIN-EVA-002, -004, -005, -007, -008 |
| 8 | Corrections without destroying audit history | Linked reversal and re-post. Each fact carries effective_at, recorded_at and posting period. Period locks apply. Issued artifacts are amended, not replaced. A correction-impact registry tracks downstream effects. Journals are keyed by (event, book), never by rule version. | RR-UNB-DATA-005-r1, RR-ACC-MDL-007-r1, UNB-XIN-ACC-003, -004 |
| 9 | Tenant isolation | One database per sponsor. The envelope's tenant is verified against the connection. Correlation never crosses tenants except through declared inter-cell contracts. A sponsor-to-sponsor move is a legal transfer, not a migration. | UNB-XIN-SAS-005, UNB-XIN-EVA-009, UNB-TEN-004 |
| 10 | Keeping AI observations from becoming financial records | Observation, Proposal, Command and Fact are distinct types, and Fact constructors are private. The AI role may only INSERT into `ai_observation`. Promotion requires a validator or an authorized human. | RR-UNB-EVT-005-r1, UNB-XIN-EVA-016 |
| 11 | Operations needing synchronous consistency | Approval with reservation; hold vs release and dispatch (per item); payee bank change vs release; sanctions block; period close vs posting; authority change vs command; fee accrual vs capacity; recurring occurrence; conflicting instructions. | UNB-XIN-EVA-002, -010, -011, -012; TX-11, TX-19, TX-25 |
| 12 | Where eventual consistency is acceptable | Notifications (with send-time revalidation), search, provisional dashboards, AI summaries, published partner events, CRM segments, service metrics, scheduling of charity re-evaluation. | UNB-XIN-EVA-015 |
| 13 | Workflow recovery | Transactional outbox. Durable process-manager rows with timers, step idempotency and stall detection that routes to a human-resolution queue. Compensation only for external effects. No blind retry on Unknown. Poison quarantine. Timers fire exactly once after a restore. | UNB-XIN-EVA-007, UNB-XIN-WFA-001, -002 |
| 14 | Reporting without inconsistent financial results | Reports read only from `FinancialSnapshotSealed` (period, watermark), so the same watermark always yields the same figures. Live figures are labelled provisional. | UNB-XIN-EVA-003 |

### Matrix

<!-- BEGIN GENERATED: events -->
| ID | Event | Capability | Owner | Authority | Store | Consistency | Idempotency key | Correction |
|---|---|---|---|---|---|---|---|---|
| XEV-001 | ContributionIntentRecorded | contributions | Contribution Intake | authoritative-nonfinancial | sponsor DB: contribution.facts + contribution.current (same tx) | eventual | client intent key (sponsor, donor principal, idempotency token) | ContributionIntentWithdrawn; never edited |
| XEV-002 | ContributionFundsReceived | contributions | Contribution Intake + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) (clearing/suspense posting) | synchronous | rail evidence key: ACH (trace number, settlement date, amount); wire (IMAD/OMAD or bank ref); check (bank acc… | Reversal batch linked to original; re-post to correct fund |
| XEV-003 | ContributionAccepted | contributions | Gift Acceptance + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | ContributionId + 'accept' (unique acceptance per contribution) | Accepted gift cannot be refunded to donor (UNB-RCN-FIN-013); errors corrected by linked reversal + re-post un… |
| XEV-004 | ContributionAvailableForGranting | contributions | Ledger & Capacity | derived | computed in-transaction from journal + reservations + holds | synchronous | n/a (derived; not stored as authority) | Recomputed; never corrected directly |
| XEV-005 | ContributionReturned | contributions | Contribution Intake + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | ACH return: (original trace number, return reason code, return trace number) | Compensating debit; capacity deficit -> FundingShortfall/hold, never silent negative |
| XEV-006 | InKindAssetLegallyAccepted | contributions | Gift Acceptance | authoritative-financial | sponsor DB: invest.facts/positions + ledger.journal | synchronous | AssetIntakeId + acceptance decision id | Superseding acceptance record; valuation corrections separate |
| XEV-007 | AssetLiquidationTrancheSettled | contributions | AUM & Investments + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | custodian (account, trade ref, settle date, lot ids) | Trade bust/correction from custodian -> reversal of tranche batch + re-post |
| XEV-008 | ContributionAcknowledgmentIssued | contributions | Documents/Artifact Registry | authoritative-nonfinancial | sponsor DB: artifact_registry (as-issued, hashed) | synchronous | ContributionId + template version + ack version | AcknowledgmentAmended / Superseded artifact; original retained as-issued |
| XEV-009 | ContributionMisapplicationCorrected | contributions | Ledger (correction command) | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | CorrectionCaseId | Is itself the correction: reversal of wrong-fund batch + re-post to right fund in one tx, both linked to orig… |
| XEV-010 | GrantRecommendationCreated | grants | Grant Management | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) | eventual | client request key + recommendation fingerprint (fund, charity, amount, purpose) for EXC-10 review | GrantRecommendationRevised / Withdrawn |
| XEV-011 | GrantRecommendationWithdrawn | grants | Grant Management | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) | synchronous | GrantId + 'withdraw' | none (terminal); if already approved -> cancellation path |
| XEV-012 | SponsorPolicyEvaluated | grants | Sponsor Governance / Grant | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) (decision trace) | synchronous | GrantId + policy version + evaluation seq | New evaluation; never edit prior trace |
| XEV-013 | SponsorApprovalGranted | grants | Grant Management | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) | synchronous | GrantId + decision id | GrantCommitmentReversed (pre-payment) with authority |
| XEV-014 | FundingReservationCreated | grants | Financial Ledger & Capacity | authoritative-financial | sponsor DB: ledger.reservation (insert-only) | synchronous | GrantId + installment no. | FundingReservationReleased (linked) |
| XEV-015 | FundingReservationReleased | grants | Financial Ledger & Capacity | authoritative-financial | sponsor DB: ledger.reservation | synchronous | ReservationId + release reason | none (terminal) |
| XEV-016 | GrantPayableRecognized | grants | Financial Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | GrantId + 'payable' | GrantPayableAdjusted / reversal batch |
| XEV-017 | GrantCommitmentReversed | grants | Grant Management + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) + sponsor DB: grant.facts + grant.current (same t… | synchronous | GrantId + reversal decision id | is the correction |
| XEV-018 | GrantInstallmentDue | grants | Grant Management (schedule process manager) | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) (occurrence row) | synchronous | ScheduleId + schedule version + occurrence date | OccurrenceSkipped with reason |
| XEV-019 | GrantFundingPlanApproved | grants | Grant Management + AUM | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) | synchronous | GrantId + plan version | Superseding plan version |
| XEV-020 | FundTransferredOut | grants | Grant Management (DAF_TRANSFER_OUT) | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | TransferProgramId + FundId | Return of transfer by receiving sponsor = new inbound contribution, not reversal |
| XEV-021 | PaymentInstructionApproved | grants | Payment Orchestration | authoritative-financial | sponsor DB: payment.facts + payment.current (same tx) + outbox | synchronous | PaymentIntentId (stable across retries/failover) | RevokedBeforeDispatch (linked) |
| XEV-022 | PaymentInstructionSubmissionRecorded | grants | Payment Orchestration | authoritative-nonfinancial | sponsor DB: payment.facts + payment.current (same tx) | synchronous | PaymentIntentId + provider request id | DispatchOutcomeUnknown; no edit |
| XEV-023 | PaymentExecutionAcknowledged | grants | Payment Orchestration (adapter) | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | provider event id | Contradictory later evidence recorded, not overwritten |
| XEV-024 | PaymentOutcomeUnknownRecorded | grants | Payment Orchestration | authoritative-nonfinancial | sponsor DB: payment.facts + payment.current (same tx) | synchronous | PaymentIntentId + 'unknown' + dispatch attempt | Resolved only by evidence-based TX08 disposition |
| XEV-025 | PaymentSettlementEvidenceReceived | grants | Payment Orchestration (adapter) | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | bank statement line id or provider settlement id | Later reversal evidence appended |
| XEV-026 | PaymentSettled | grants | Payment Orchestration + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | PaymentIntentId + settlement trace | PaymentReturnReported -> compensating batch |
| XEV-027 | PaymentReturnReported | grants | Payment Orchestration + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | ACH: (original trace, return reason, return trace); check: (account, check no., return item id); wire: bank r… | is compensation; never deletes settlement |
| XEV-028 | PaymentPartiallyReturned | grants | Payment Orchestration + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | bank return ref + amount + currency | Linked compensation for returned amount; fee leg separately classified |
| XEV-029 | CheckVoidedStaleDated | grants | Payment Orchestration + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | account + check number + void confirmation ref | Reissue = new linked PaymentIntent; escheat path per FI-21 |
| XEV-030 | PaymentReconciliationCompleted | grants | Finance Reconciliation | authoritative-financial | sponsor DB: recon.match (insert-only) + ledger | synchronous | statement id + line id + PaymentIntentId | Unmatch record + new match |
| XEV-031 | PaymentReconciliationBreakDetected | grants | Finance Reconciliation | derived | sponsor DB: workqueue.case + case_event (append-only) | eventual | statement line id + rule id | Case closure with proof |
| XEV-032 | GranteeNotified | grants | Notifications | informational | sponsor DB: outbox (mutable delivery status) + subscriber cursor / comm.delivery_log | eventual | PaymentIntentId + template + channel | Suppressed/superseded notice |
| XEV-033 | GrantAcknowledgmentReceived | grants | Grant Management | informational | sponsor DB: grant.facts + grant.current (same tx) | eventual | charity ack id | n/a |
| XEV-034 | GrantClosed | grants | Grant Management | derived | sponsor DB: grant.facts + grant.current (same tx) | eventual | GrantId + 'close' | SM-0: later facts create linked child records |
| XEV-035 | InvestmentRedemptionRequested | investments | AUM & Investments | authoritative-nonfinancial | sponsor DB: invest.facts/positions + ledger.journal | synchronous | GrantId + leg no. | RedemptionCancelled before execution |
| XEV-036 | InvestmentRedemptionRejected | investments | AUM & Investments | authoritative-nonfinancial | sponsor DB: invest.facts/positions + ledger.journal | synchronous | custodian order ref | n/a |
| XEV-037 | InvestmentRedemptionPartiallySettled | investments | AUM & Investments + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | custodian (trade ref, settle date, qty) | Linked reversal on bust |
| XEV-038 | InvestmentRedemptionSettled | investments | AUM & Investments + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | custodian (trade ref, settle date) | Linked reversal on bust |
| XEV-039 | CustodianPositionFileIngested | investments | Integration Adapters | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | custodian + account + as-of date + file hash | Superseding file version recorded; original kept |
| XEV-040 | ValuationCorrectionReceived | investments | AUM & Investments | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | price source + security + as-of + version | Recorded as new version |
| XEV-041 | UnitPriceStruck | investments | AUM & Investments + Ledger | authoritative-financial | sponsor DB: invest.facts/positions + ledger.journal | synchronous | pool + valuation date | NavRestated |
| XEV-042 | NavRestated | investments | AUM & Investments + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | pool + valuation date + restatement version | is the correction; materiality policy decides fund-level reprocessing |
| XEV-043 | FundInvestmentAllocationChanged | investments | AUM & Investments | authoritative-nonfinancial | sponsor DB: invest.facts/positions + ledger.journal | synchronous | fund + allocation version | Superseding allocation |
| XEV-044 | JournalBatchPosted | accounting | Financial Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | SourceAggregateId + SourceEventId (UNIQUE) | JournalBatchReversed + replacement |
| XEV-045 | JournalBatchReversed | accounting | Financial Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | ReversalOfBatchId (UNIQUE: one reversal per batch) | none; reversing a reversal requires new posting |
| XEV-046 | PriorPeriodAdjustmentPosted | accounting | Financial Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) | synchronous | CorrectionCaseId | Further adjustment |
| XEV-047 | AccountingPeriodHardClosed | accounting | Financial Ledger / Close | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) (period state) | synchronous | sponsor + period | AccountingPeriodReopened (dual approval) |
| XEV-048 | AccountingPeriodReopened | accounting | Financial Ledger / Close | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) (period state) | synchronous | sponsor + period + reopen seq | Re-close |
| XEV-049 | BankStatementIngested | accounting | Integration Adapters | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | bank + account + statement id + hash | Corrected statement recorded as new version |
| XEV-050 | AvailableCashReconciled | accounting | Finance Reconciliation | authoritative-financial | sponsor DB: recon.attestation | synchronous | account + date + recon version | Superseding reconciliation |
| XEV-051 | FinancialSnapshotSealed | accounting | Reporting | derived | sponsor DB: reporting.snapshot (sealed, watermark-bound) | synchronous | sponsor + period + watermark | Superseding snapshot version; original retained |
| XEV-052 | CharityEvidenceSnapshotPinned | charity verification | Common Charity Registry | authoritative-nonfinancial | platform DB: charity_registry.facts (shared, no donor data) | synchronous | CharityId + source + retrieval hash | New version |
| XEV-053 | CharityEvidenceChanged | charity verification | Common Charity Registry | informational | platform DB: charity_registry.facts (shared, no donor data) | eventual | source + record key + source version | n/a |
| XEV-054 | EligibilityEvaluationCompleted | charity verification | Common Charity Registry | authoritative-nonfinancial | platform DB: charity_registry.facts (shared, no donor data) | synchronous | CharityId + evidence version + rule version | Superseding evaluation |
| XEV-055 | EligibilityHoldPlaced | charity verification | Compliance & Protective Holds | authoritative-nonfinancial | platform DB: hold_registry (HoldPublicationVersion) | synchronous | hold reason + subject + source evidence id | EligibilityHoldReleased (independent authority) |
| XEV-056 | EligibilityHoldReleased | charity verification | Compliance & Protective Holds | authoritative-nonfinancial | platform DB: hold_registry (HoldPublicationVersion) | synchronous | HoldId + release decision | n/a |
| XEV-057 | RecipientBankDetailsIndependentlyVerified | charity verification | Payment Orchestration (destination) | authoritative-nonfinancial | sponsor DB: payment.facts + payment.current (same tx) (destination version) | synchronous | recipient + destination version | New destination version; never edit |
| XEV-058 | RecipientBankDetailsChangeFlagged | charity verification | Payment Orchestration (destination) | authoritative-nonfinancial | sponsor DB: payment.facts + payment.current (same tx) | synchronous | recipient + submitted change id | Resolution case |
| XEV-059 | IrsExemptOrgFileIngested | charity verification | Integration Adapters | informational | sponsor DB: external_observation.batch/item (raw, hashed, insert-only) | eventual | source + period + file hash | Superseding file version |
| XEV-060 | ServiceCaseOpened | customer service | Work Queue / Donor & Sponsor Administration | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) | eventual | channel + contact id or originating event id | CaseReclassified |
| XEV-061 | StaffAssistedInstructionCaptured | customer service | Donor & Sponsor Administration | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) + originating command | synchronous | channel + staff id + instruction hash | Instruction withdrawn |
| XEV-062 | ReconsiderationRequested | customer service | Donor & Sponsor Administration | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) | eventual | decision id + requester | Outcome record |
| XEV-063 | ServiceCaseResolved | customer service | Work Queue | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) | eventual | CaseId + resolution seq | CaseReopened |
| XEV-064 | SanctionsScreeningCompleted | compliance | Compliance & Protective Holds | authoritative-nonfinancial | sponsor DB: compliance.screening (list version pinned) | synchronous | subject + list version + rule version | Re-screen on list update (CTL-012) |
| XEV-065 | SanctionsBlockApplied | compliance | Compliance + Ledger | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) (blocked-property account) | synchronous | subject + item + block decision id | Unblock only under OFAC licence/authorization record |
| XEV-066 | FundProtectiveFreezePlaced | compliance | Compliance & Protective Holds | authoritative-nonfinancial | platform DB: hold_registry (HoldPublicationVersion) (fund-scoped) or sponsor hold table | synchronous | fund + reason + decision id | FundProtectiveFreezeReleased (independent authority) |
| XEV-067 | RecordsLegalHoldPlaced | compliance | Evidence & Audit / Retention | authoritative-nonfinancial | sponsor DB: retention.legal_hold | synchronous | matter id + scope hash | LegalHoldReleased by counsel |
| XEV-068 | ConflictingInstructionDetected | compliance | Grant / Donor Administration | authoritative-nonfinancial | sponsor DB: grant.facts + grant.current (same tx) | synchronous | fund + conflict scope key | Resolution decision by sponsor |
| XEV-069 | ExceptionCaseOpened | compliance | Exception Workbench | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) | eventual | trigger event id + case class | Merge into parent case |
| XEV-070 | ExceptionResolutionApproved | compliance | Exception Workbench | authoritative-nonfinancial | sponsor DB: workqueue.case + case_event (append-only) | synchronous | CaseId + resolution version | New resolution |
| XEV-071 | AdvisoryPrivilegeGranted | relationship management | Identity & Delegation | authoritative-nonfinancial | sponsor DB: identity.authority_facts (authority epoch) | synchronous | fund + principal + role + grant decision id | AdvisoryPrivilegeRevoked |
| XEV-072 | AuthorityEpochAdvanced | relationship management | Identity & Delegation | authoritative-nonfinancial | sponsor DB: identity.authority_facts (authority epoch) | synchronous | fund + epoch | n/a |
| XEV-073 | SuccessionNoticeReceived | relationship management | Donor & Sponsor Administration | authoritative-nonfinancial | sponsor DB: identity.authority_facts (authority epoch) | synchronous | fund + notice source + document hash | Notice disproved -> hold released |
| XEV-074 | SuccessorActivated | relationship management | Identity & Delegation | authoritative-nonfinancial | sponsor DB: identity.authority_facts (authority epoch) | synchronous | fund + succession case id | Governed reversal |
| XEV-075 | AnonymityElectionRecorded | relationship management | Identity & Delegation (privacy) | authoritative-nonfinancial | sponsor DB: identity.authority_facts (authority epoch) | synchronous | fund/grant + election version | Superseding election (prospective only) |
| XEV-076 | ContactPreferenceUpdated | relationship management | Donor & Sponsor Administration (CRM) | authoritative-nonfinancial | sponsor DB: crm.* mutable rows + system-versioned history table | eventual | party + field + version | In-place update with history; erasable per privacy |
| XEV-077 | InteractionLogged | relationship management | Donor & Sponsor Administration (CRM) | informational | sponsor DB: crm.* mutable rows + system-versioned history table | eventual | channel + external interaction id | Redaction/erasure per R-PII |
| XEV-078 | FeeAccrued | fees and billing | Fees & Billing + Ledger | authoritative-financial | sponsor DB: fees.accrual + ledger.journal | synchronous | fund + fee schedule version + period + basis watermark | FeeAccrualRecomputed delta posting |
| XEV-079 | FeeAccrualRecomputed | fees and billing | Fees & Billing + Ledger | authoritative-financial | sponsor DB: fees.accrual + ledger.journal | synchronous | original accrual id + basis watermark | is correction (delta posting) |
| XEV-080 | FeeWaived | fees and billing | Fees & Billing + Ledger | authoritative-financial | sponsor DB: fees.accrual + ledger.journal | synchronous | fund + period + waiver decision id | Waiver reversal |
| XEV-081 | PartnerRevenueShareInvoiced | fees and billing | Fees & Billing (sponsor operating ledger) | authoritative-financial | sponsor DB: ledger.journal_batch/journal_line (insert-only) (operating entity) | synchronous | partner + period + invoice version | Credit memo |
| XEV-082 | NotificationRequested | communications | Notifications | derived | sponsor DB: outbox (mutable delivery status) + subscriber cursor | eventual | event id + template + recipient (UNB-RCN-PLT-002 dedup key) | Superseded/suppressed |
| XEV-083 | NotificationSuppressedAsSuperseded | communications | Notifications | informational | sponsor DB: comm.delivery_log | eventual | notification id | n/a |
| XEV-084 | NotificationDelivered | communications | Notifications | informational | sponsor DB: comm.delivery_log | eventual | provider message id | Bounce recorded |
| XEV-085 | StatementIssued | communications | Documents/Artifact Registry | authoritative-nonfinancial | sponsor DB: artifact_registry (as-issued, hashed) | synchronous | fund + period + snapshot id | StatementAmendmentIssued; original retained |
| XEV-086 | StatementAmendmentIssued | communications | Documents/Artifact Registry | authoritative-nonfinancial | sponsor DB: artifact_registry (as-issued, hashed) | synchronous | fund + period + amendment no. | Further amendment |
| XEV-087 | ProjectionRebuilt | operational reporting | Reporting | informational | sponsor DB: projection metadata | eventual | projection + source watermark | n/a |
| XEV-088 | ProjectionLagBreached | operational reporting | Reporting / Operations | informational | telemetry | eventual | projection + window | n/a |
| XEV-089 | SourceFeedGapDetected | operational reporting | Integration Adapters / Reconciliation | derived | sponsor DB: workqueue.case + case_event (append-only) | eventual | source + expected date | Late file arrival closes case |
| XEV-090 | ProcessManagerStalled | operational reporting | Operations | derived | sponsor DB: workqueue.case + case_event (append-only) | eventual | process id + stall window | Resolved by human resolution |
| XEV-091 | AiObservationRecorded | AI automation | AI Evidence & Exception Workbench | observation-ai | sponsor DB: ai_observation.* (separate schema; AI role INSERT here only) | eventual | session id + observation hash | Superseded observation; never corrected into facts |
| XEV-092 | ExceptionResolutionProposed | AI automation | AI Evidence & Exception Workbench | observation-ai | sponsor DB: ai_observation.* (separate schema; AI role INSERT here only) + case link | eventual | CaseId + proposal hash | Rejected proposal retained |
| XEV-093 | AiDocumentExtractionRecorded | AI automation | AI Evidence & Exception Workbench | observation-ai | sponsor DB: ai_observation.* (separate schema; AI role INSERT here only) | eventual | document hash + model version + prompt version | Re-extraction |
| XEV-094 | EvidencePromoted | AI automation | Evidence & Audit (owning domain validates) | authoritative-nonfinancial | sponsor DB: evidence.facts | synchronous | observation id + promoter principal | EvidenceDemoted (with reason) |
| XEV-095 | AiContextRetrieved | AI automation | AI Evidence & Exception Workbench | informational | sponsor DB: ai_observation.retrieval_log | eventual | session id + resource id | n/a |
| XEV-096 | TenantMigrationCutoverCompleted | accounting | Recovery & Operations / Migration | authoritative-financial | control plane routing + sponsor DB checkpoint | synchronous | migration run + fencing generation | New migration run (no history rewrite) |
<!-- END GENERATED: events -->

## 3. Consistency and failure-recovery requirements

<!-- BEGIN GENERATED: eva-reqs -->
| ID | P | Requirement | Links |
|---|---|---|---|
| UNB-XIN-EVA-001 | P0 | For each sponsor legal entity and for every function for which Unbound is designated book of record (UNB-XIN-ACC-010), fund balances, reservations, capacity inputs and every financial report figure MUST be computed only from the insert-only journal and reservation tables, less fees computed but not yet posted (XID-ERP-001). Every domain fact with a declared financial effect MUST carry the JournalBatchId (or ReservationId) committed in the same database transaction, and the system MUST NOT maint… | UNB-ACC-002, UNB-ACC-003, UNB-PLAT-007, RR-UNB-PLAT-007-r1, REQ-X-022, UNB-IMM-005, GR-EVT-029 |
| UNB-XIN-EVA-002 | P0 | Every posting command MUST read the accounting period row for its posting date with a shared lock (or run SERIALIZABLE) in the posting transaction; HardClosePeriod MUST take an exclusive lock on that row. An ordinary posting whose posting date falls in a hard-closed period MUST be rejected with PERIOD_CLOSED, and the economic event MUST instead be recorded through a PriorPeriodAdjustment command that posts in the first open period, carries relates_to_period and effective_at of the original even… | UNB-ACC-017, ACC-ADR-011, TX-21, INV-08, EXC-18, GEX-T16 |
| UNB-XIN-EVA-003 | P1 | Every financial report, fund statement, regulatory data package and management financial dashboard figure MUST be computed from a FinancialSnapshotSealed identified by (sponsor legal entity, period or as-of instant, ledger commit watermark) and MUST record that identifier. Computing the same report twice with the same identifier MUST produce identical figures. Reports MUST NOT read event streams, outbox records or asynchronously maintained projections for financial figures. | UNB-ACC-019, UNB-ACC-020, TX-21, INV-08, TVR-DATA-001 |
| UNB-XIN-EVA-004 | P1 | For each external source (bank statement/return files, custodian position and transaction files, card/ACH processor reports, IRS exempt-organization extracts, sanctions list publications) the system MUST maintain an expected-delivery calendar and a per-source completeness watermark (last complete business date or source sequence). A batch MUST be accepted as complete only when its control totals (record count, amount sum, or source-declared trailer) match. When a source is behind its calendar b… | UNB-ACC-018, UNB-EVT-003, UNB-CUST-013, CTL-012, UNB-GEX-032, EXC-13 |
| UNB-XIN-EVA-005 | P0 | Each external observation type MUST have a registered business idempotency key derived from source data (e.g., ACH return: original entry trace number + return reason code + return entry trace number; check: bank account + check number + item sequence; custodian transaction: custodian + account + transaction reference + version/cancel indicator; provider callback: provider event id). Ingestion MUST store the key and a payload hash. A record with an existing key and identical hash MUST be acknow… | UNB-GEX-003, UNB-PAY-004, UNB-EVT-002, PAY-07, SIM-19, GEX-T01, GEX-T14 |
| UNB-XIN-EVA-006 | P1 | Every authoritative fact, derived event, case, AI observation, notification and external observation item MUST carry (a) correlation_root: the business root id (ContributionId, GrantId, FundId-lifecycle id, CorrectionCaseId, TransferProgramId) it belongs to, (b) causation_id: the id of the fact, observation or command that directly caused it, and (c) a business_keys set of typed ids (SponsorId, FundId, CharityId, PaymentIntentId, ContributionId) it touches. Where one root spawns another (a succ… | UNB-GEX-004, UNB-DATA-002, UNB-GEX-021, UNB-EVT-004 |
| UNB-XIN-EVA-007 | P1 | Each long-running business workflow MUST be implemented as a process manager persisted as a versioned row in the owning module's schema with: current step, expected next facts, next_due_at timer, attempt counts, and the ids of commands it has issued. A process manager MUST advance only by consuming authoritative facts or timer expiry, MUST issue commands with stable idempotency keys so re-execution after a crash has no duplicate effect, and MUST NOT hold database locks across external calls or… | UNB-GEX-020, UNB-GEX-014, UNB-ACC-034, INV-13, TX-17, FI-21, CTL-044, EXC-04 |
| UNB-XIN-EVA-008 | P2 | Domain events MUST be classified private (consumed only inside the monolith) or published (delivered to partners, widgets or webhooks). Published integration events MUST be a separately versioned contract derived from authoritative facts by an explicit mapping, MUST expose only partner-safe states (e.g., Submitted, Approved, Paid-settled, Returned; never provider-acknowledged or internal exception states) and MUST be supported for a declared compatibility window. Stored events MUST carry schema… | UNB-RCN-ARC-001, UNB-DATA-010, UNB-GEX-034, UNB-API-STD-001, WG-8 |
| UNB-XIN-EVA-009 | P1 | A bulk transfer of funds from one legal sponsor to another MUST run as a TransferProgram process manager that issues, per fund, an independent DAF_TRANSFER_OUT distribution in the source sponsor ledger and an independent inbound contribution in the destination sponsor ledger, each with its own authorization, idempotency key (TransferProgramId + FundId) and reconciliation. Source history MUST remain in the source sponsor's books; the destination MUST receive a signed, hashed predecessor-history… | UNB-WF-014, RR-UNB-WF-014-r1, UNB-RCN-ENT-007, REQ-X-061, RR-REQ-X-061-r1, UNB-TEN-012, UNB-TEN-016 |
| UNB-XIN-EVA-010 | P0 | The system MUST model at least these distinct hold types with a declared set of blocked commands: ProtectiveHold(charity\|recipient\|destination) blocks release and dispatch to that subject; FundProtectiveFreeze blocks outflow commands from the fund (grants, transfers, and fee accrual only if sponsor policy says so) but MUST NOT block inbound contributions, dividends, valuation, statements or records access; SanctionsBlock moves affected funds to a blocked-property account and blocks unblocking… | UNB-GAP-019, UNB-GEX-038, ADM-023, TVCTL-13, TVS-081, UNB-RCN-PLT-003 |
| UNB-XIN-EVA-011 | P0 | A batch (recurring-grant run, payment file, statement run, fee run) MUST be a container of independent item commands and MUST NOT carry approval, release or dispatch authority itself. Each item MUST be committed in its own transaction with its own idempotency key and MUST record the HoldPublicationVersion it checked at release; the dispatcher MUST re-check each item immediately before transmitting it, including items already placed in an outbound file not yet transmitted. A hold published mid-r… | TVCTL-04, TVCTL-13, CTL-012, RR-REQ-X-020-r1, UNB-RCN-PLT-003, INV-13, ADV-070 |
| UNB-XIN-EVA-012 | P1 | Each advisor instruction MUST carry a conflict scope key (fund, instruction kind, and target: charity, schedule, investment allocation or transfer). On commit, the handler MUST, in the same transaction, check for open instructions with the same scope from a different principal that request an incompatible outcome; if found, both MUST move to HeldConflict and a case opened for the sponsor, unless the fund's versioned instruction-authority policy (any-one, all, designated primary) resolves it det… | ADM-023, TVS-370, TVS-279, UNB-SUCC-008, RR-UNB-SUCC-008-r1, TX-25, UNB-RCN-ENT-008 |
| UNB-XIN-EVA-013 | P2 | A message broker, separately deployed service or second authoritative store MUST NOT be introduced unless an ADR presents reproducible measurements on the selected managed PostgreSQL showing at least one of: (a) required peak authoritative commit rate exceeds 50% of benchmarked sustainable capacity of the largest sponsor database after indexing, partitioning and connection pooling; (b) p99 outbox dispatch lag exceeds 60 seconds for three consecutive peak days after tuning; (c) a contractual or… | REQ-X-020, REQ-X-021, UNB-IMM-010, UNB-TEN-018, RCD-012, UNB-RCN-PLT-012 |
| UNB-XIN-EVA-014 | P1 | Relationship data MUST be classified as either (a) authority facts — advisory privilege grants and revocations, successor designations, contributor of record, anonymity and recognition elections, legal-representative evidence, consent records — owned by Identity & Delegation, stored append-only with effective/recorded times and checked synchronously by command handlers; or (b) engagement data — contact details, preferences, interests, interactions, segments, relationship-manager notes — owned b… | UNB-PART-003, UNB-DATA-007, UNB-IMM-008, INV-09, INV-14, UNB-RCN-ENT-009 |
| UNB-XIN-EVA-015 | P1 | Every notification derived from a fact MUST record the fact id and version it describes. Immediately before external send, the delivery worker MUST re-read the fact's current version and the recipient's anonymity/consent state; if the fact was superseded, reversed or returned, or the election changed so that the message would be false or disclosing, it MUST suppress the message (NotificationSuppressedAsSuperseded) and, for mandatory notices, enqueue the corrected notice instead. | UNB-RCN-PLT-002, INV-03, INV-13, INV-14, ADV-088, UNB-PAY-009 |
| UNB-XIN-EVA-016 | P1 | Records of different authority classes MUST be stored in physically separate tables (or schemas) with distinct database privileges: journal and reservation tables (financial facts, balance CHECK constraints), per-module authoritative fact tables, ai_observation (AI principal INSERT only here), external_observation (raw ingested evidence), outbox and process-manager tables (operational coordination, mutable), and engagement tables (mutable with history). A generic event or JSON-document table MA… | UNB-DATA-001, UNB-DATA-008, RR-UNB-PLAT-007-r1, UNB-TEC-STRATA-003, UNB-EVT-005 |
<!-- END GENERATED: eva-reqs -->

## 4. Can the twelve capabilities share one event-driven coordination architecture?

| Capability | Share the event coordination? | Caveat |
|---|---|---|
| Contributions | Yes, downstream of acceptance | Acceptance and capacity are synchronous; the reversal window comes from the rail table |
| Grants | Yes, downstream of approval | Approval with reservation, hold vs release and sanctions are synchronous, per item |
| Investments | Partially | Truth arrives as custodian files to reconcile, not as events |
| Accounting | The journal *is* the log | No second event store; period lock is synchronous |
| Charity verification | Yes | Eligibility evaluations are authoritative; source changes are informational |
| Customer service | Yes | Cases reference transactions and never carry financial semantics |
| Compliance | Partially | Holds and blocks are synchronous typed records, not event reactions |
| Relationship management | Engagement data only | Authority facts are separate and synchronous |
| Fees and billing | Yes | Only the fee engine emits authoritative fee events |
| Communications | Yes | Send-time revalidation |
| Operational reporting | Yes, from sealed snapshots | Never from raw streams |
| AI automation | Yes, as consumers and observers | Observations are never authoritative |

## 5. Alternative architecture comparison

Scores run from 1 to 5. For skills and infrastructure, 5 means low demand or a small footprint. Justification for every cell is in [industry/eva.md](industry/eva.md).

| Criterion | A Traditional modular | B Event-driven modular monolith | C Event-sourced financial core | D Fully event-sourced | **E Hybrid (chosen)** | F Distributed + broker + saga |
|---|---|---|---|---|---|---|
| Conceptual simplicity | 5 | 4 | 3 | 1 | 4 | 1 |
| Reliability | 3 | 4 | 4 | 3 | 5 | 2 |
| Testability | 4 | 4 | 4 | 3 | 5 | 2 |
| Operational efficiency | 3 | 4 | 3 | 2 | 4 | 1 |
| Auditability | 2 | 3 | 5 | 5 | 5 | 3 |
| Correction handling | 2 | 3 | 4 | 3 | 5 | 2 |
| Tenant migration | 3 | 3 | 3 | 2 | 4 | 2 |
| Skill demand (5 = low) | 5 | 4 | 3 | 1 | 4 | 1 |
| Infrastructure footprint (5 = small) | 5 | 5 | 4 | 3 | 5 | 1 |
| Fit to F#, WASM-first, managed PostgreSQL, minimal infrastructure | 4 | 4 | 3 | 2 | 5 | 1 |
| **Total (/50)** | 36 | 38 | 36 | 25 | **47** | 16 |

**Why not C, the event-sourced financial core.** The journal is already an append-only, balanced, causal log. Adding an event stream under it creates two financial logs with no defined winner, and replay risks re-posting (XIF-EVA-003). The baseline also has no Echelon or Strata event store (RCF-ARC-001).

**Why not B.** In B, events are informational and state is mutable. E keeps B's outbox and adds append-only facts and journal authority, at little extra cost.

**Caveat on the scoring.** The proposing research pass assigned these scores, and they come from analysis, not measurement. The adversarial review notes that this is self-scoring (XIF-ADV-007). The ranking E > B is robust to ±1 on any single criterion. The margin over B (47 vs 38) should not be read as precise.

**Scope correction (XIC-ADV-001, XIC-ADV-002).** UNB-XIN-EVA-001 ("balances only from the journal") applies only to functions for which Unbound is designated book of record under UNB-XIN-ACC-010. Where a sponsor keeps an external book of record (Shadow mode), grantable capacity comes from UNB-XIN-ADV-002: a reconciled external balance at a watermark, minus reservations. The re-derivation control in UNB-XIN-ACC-003 is a comparison whose output never posts, so it does not conflict with EVA-001.

## 6. Adversarial validation

Each scenario below was an attempt to disprove E. The scenario records are XDS-EVA-001..022.

| Scenario | Outcome | What breaks or remains at risk |
|---|---|---|
| Misposted contribution discovered after statements are issued and the period is closed | Handled | The wrong fund may already have made grants. Who bears the shortfall is a policy question. |
| Security contribution valued at the wrong date | Handled with human steps | The acknowledgment must be amended. Units were struck at the wrong price. |
| Charity revoked between approval and payment | Handled once the hold commits | **Not handled** if the revocation has not yet been ingested, or the payment was already transmitted. |
| ACH R03 return after 3 days; check uncashed after 180 days | Handled | There is no end-to-end ACH key. Escheat needs durable timers. |
| In-kind liquidation in tranches, then a custodian correction | Handled | Naive deduplication would drop the correction. |
| Wire partly returned net of fees; one of three liquidation legs fails | Handled for accounting | Who bears the fee is undecided (XID-EVA-003). The grant is held. |
| OFAC hit in the middle of a recurring batch | Handled **only if** batches carry no authority | Batch-level approval breaks it (UNB-XIN-EVA-011). |
| Legal hold on a fund under dispute | Handled with typed holds | A generic hold over-blocks (UNB-XIN-EVA-010). |
| 10,000 funds move to another sponsor | Handled as legal transfers | **Breaks** if run as a tenant migration (UNB-XIN-EVA-009). |
| Sponsor leaves and returns; event schema drift | Partially handled | An unknown forked version halts the stream. That is intended. |
| Co-advisors give opposite instructions; standing vs one-off instruction; successor notice arrives during a grant | Handled | Needs scope keys, policy defaults and an epoch check (UNB-XIN-EVA-012). |
| AI prompt injection writes "cleared" | Handled | Fails without separate database roles (UNB-XIN-EVA-016). |
| Missing feed; report run during posting; posting races close | Handled | Needs a feed calendar, sealed snapshots and a period lock. |
| Erasure request | Handled for class 2 | Fully event-sourced variants fail. |
| Outbox id 501 commits after id 502 | Handled with claim-and-mark | A cursor based on id watermarks would skip it. |

**Residual risks no architecture removes:**
- external facts that have not yet been ingested;
- settlement that cannot be reversed;
- a correlated misconfiguration of a policy pack shared across sponsors;
- year-end hot-row contention, which has not been measured.

## 7. Where shared abstractions become harmful

| Abstraction | Harm | Instead |
|---|---|---|
| One generic Event/EventStore table | Mixes authority classes; cannot enforce AI-only privileges or balancing; duplicates the journal | UNB-XIN-EVA-016, UNB-XIN-EVA-001 |
| Generic Case with financial semantics | Closing the case is mistaken for fixing the money | Typed commands kept separate from cases; case kernel plus profiles (UNB-XIN-SVC-002) |
| Generic Hold | A records hold blocks grants; a freeze blocks inflows; an OFAC block is treated as release | Typed holds (UNB-XIN-EVA-010) |
| Batch that carries approval | A sanctions hit mid-batch is ignored | UNB-XIN-EVA-011 |
| Universal bus coupling reporting to the write model | Reports depend on timing | UNB-XIN-EVA-003 |
| Event schemas used as the public API | Partners bind to internal states | UNB-XIN-EVA-008 |
| One Party record shared by CRM and authority | CRM data authorizes; erasure becomes impossible | UNB-XIN-EVA-014 |
| Single linear payment status | A payment shows as paid on acknowledgment | REQ-X-083 |
| Tenant-migration tooling used for sponsor transfers | Funds land on the wrong legal books | UNB-XIN-EVA-009 |
| Database sequence used as a watermark | Gaps read as loss; late commits are skipped | UNB-XIN-EVA-007 |

## 8. Hypothesis verdict

**Partially supported.**

**Supported.** All twelve capabilities can share typed, traceable, at-least-once events from a transactional outbox, consumed by in-process process managers and projections, at volumes far below where a broker is needed.

**Refuted:**
1. Invariant-bearing steps are synchronous, not event-coordinated.
2. "Relationship management" misstates the legal structure. The sponsor controls the assets, and CRM data must not authorize money.
3. External financial truth arrives as batches that must be reconciled.
4. Charitable-purpose rules are synchronous gates with pinned versions and sponsor or human discretion. They are not reactions to events.

**Reformulated hypothesis:**

> A DAF enterprise can be modeled as a set of authoritative records of sponsor-controlled charitable assets. These are a sponsor-entity double-entry ledger, versioned policy and authority decisions, and orthogonal contribution and payment state machines bound to external evidence, all committed synchronously in one transactional store per legal sponsor. Every downstream activity (workflow progression, communications, reporting, relationship engagement and AI assistance) is coordinated through traceable, typed, at-least-once business events derived from those commits. External financial truth enters through completeness-checked reconciliation. No event, projection or AI observation can by itself create or change an authoritative record.

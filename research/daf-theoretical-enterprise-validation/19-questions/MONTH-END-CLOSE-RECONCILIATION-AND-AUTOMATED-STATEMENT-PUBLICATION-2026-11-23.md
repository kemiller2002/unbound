# Month-end close, reconciliations and automated statement publication
Date: 2026-11-23. Proposed requirements and architecture only; no implementation, simulations, CPA review or executed tests.

## Objective
Automate routine monthly financial close and statement production in an AI-native DAF enterprise while preserving independent accounting authority, sponsor governance and correct as-issued evidence. Human effort focuses on genuine exceptions and legally/contractually mandated oversight. Close is not the same decision as statement approval or distribution. One component monolith with authoritative accounting core, typed in-process contracts, no default microservices.

## Close calendar and scope
Define sponsor-specific fiscal calendar, period start/end, cutoff timezone, holiday/business-day rules, NAV valuation deadlines, custodian feed SLAs, review deadlines, statement issue target and distribution target. Support different sponsors and pooled investment programs, late account openings/closures, funds with zero activity, consolidated statements, cross-month settlement and sponsor-specific accounting policies. Keep the calendar versioned; a deadline is a target, not authorization to issue incomplete statements.

## Close state machine
Open -> CutoffReached -> InputsCollecting -> ReconciliationPending -> ExceptionReview or Reconciled -> Prepared -> FinanceApproved -> ClosedSnapshotCommitted -> StatementBatchReady -> StatementsValidated -> PublicationApproved -> Published -> DistributionQueued -> DistributionReconciled.
Provide explicit Blocked, Failed, ReopenedByControlledCorrection and SupersededCloseVersion branches. Period status is sponsor-scoped, while individual funds can be blocked independently where accounting consistency permits. No implicit jump from 'inputs received' to 'approved'.

## Authoritative snapshots and bitemporal timing
At cutoff capture ledger commit watermark, fund subledger versions, cash/fees/reservations, grants and payments, pooled investment units/NAV, FX, restrictions and approved valuation sources. Preserve business effective date and system known-at timestamp. A late-arriving transaction is classified as an adjusting item for the closed period or a prospective item under approved accounting rules, never silently backdated into an issued snapshot. Reopening or restating creates a new close version and linked statement revisions, not overwrite.

## Mandatory reconciliation gates
- GL trial balance and balanced journal entries, including suspense and clearing.
- Fund subledger total against sponsor control accounts and pooled ownership.
- Custodian holdings, cash, activity, trades and pricing evidence against accounting books.
- Investment NAV/unitization, stale prices, rounding and FX source integrity.
- Contributions, deposits and unsettled donor receipts.
- Grant approvals, reservations, disbursements, returned/voided checks and payment-provider Unknown states.
- Accrued and charged fees, processing and mailing fees, reversals.
- Restricted balances, available-to-grant versus NAV, liquidity and negative balances.
- Interfund transfers and consolidated presentation eliminations.
- Prior-period closing balances roll forward to current opening.
Every assertion stores source refs, calculation version, tolerance, materiality, result Pass/Fail/Unknown, evidence and responsible policy. Unknown critical evidence cannot be treated as zero or passed.

## Exception handling
Classify exceptions by sponsor/fund/period, materiality, reversibility, root cause, evidence completeness and blast radius. Automatically enrich with deterministic source queries and bounded AI-assisted investigation only when necessary; AI may summarize or propose but cannot create financial postings, clear a material exception or approve a close. Route unresolved issues to accountable owner with proposed corrective action, impact and deadline. A sponsor-level control-account mismatch may block the entire sponsor batch; an isolated fund-specific discrepancy may block only that fund if the global books remain reconciled. No silent partial publication: batch UI shows excluded funds and why.

## Delegation and segregation of duties
Routine deterministic reconciliations can auto-pass under versioned finance policy. Controller/preparer, independent finance reviewer, CFO/Audit oversight and sponsor-authorized delegates have distinct roles. Whether period close can auto-approve is an explicit sponsor governance/CPA decision; do not presume it. A statement may auto-validate and auto-publish after a properly approved close only if sponsor policy delegates that action and every document gate passes. No self-approval, no AI override, no waiver of hard regulatory holds.

## Statement generation and issuance
For each eligible fund, construct immutable typed StatementData from the approved close snapshot; include opening/closing balances, contributions, grants, fees, investment activity, valuation, pending items, restrictions and disclosures. Pin Typst template, Folio adapter version, fonts, brand, locale, disclosures and calculation policy. Render PDF, validate content and accessibility, compute canonical evidence digest and final PDF-byte SHA-256, archive exact bytes, verify storage readback, approve issuance and publish through the shared Document Artifact Registry. Keep original and corrections for at least seven years, plus longer holds/requirements.

## Distribution and customer experience
Publish to the authorized Document Center, then enqueue branded availability notices through Communications. Secure portal default; approved mail/attachment options and processing fees by sponsor policy. Distribution outbox is idempotent by recipient, document revision and channel. Distinguish queued, provider accepted, delivered when evidenced, bounced and viewed. Check recipient authority and preferences at dispatch. A bounced email does not invalidate issuance or portal availability; handle notification remediation separately.

## Failure and recovery
Object storage and DB commit cannot be assumed atomically transactional: stage immutable bytes, verify hash, commit issuance metadata with recoverable state machine, reconcile orphans and missing objects. On retries, preserve exact close version, statement revision and issuance idempotency key. Never issue two originals for same sponsor/fund/period/scope/close version. No blind retry of ambiguous payment or document distribution provider outcomes. Recover from crash between close approval, render, object upload, publication and notification without losing provenance or duplicating issuance.

## Administrative UI
Close Command Center: sponsor/period selector, timeline, readiness percentage by required controls (not a fabricated assurance score), control assertion table, source freshness, unreconciled differences, exception owners, blocked funds, deadlines, approvals and close version history. Statement Batch dashboard: generated/validated/approved/published/delivered counts as distinct states, failed render and accessibility tests, missing artifacts, corrections and distribution exceptions. Drill from any displayed amount to authoritative source evidence. Support mobile and accessible layouts; no blank loading state.

## Observability and metrics
Close cycle time, source feed latency, percentage of controls passed deterministically, human touches per fund, funds blocked, exception aging, false holds, reconciliation breaks, close revisions, document generation failures, duplicate prevention, delivery outcomes, integrity failures and per-sponsor unit cost. Optimize exception rate without lowering control standards. Monitor dependency on custodians and external payment providers.

## Proposed acceptance gates
1. No statement issues before applicable close and finance approval.
2. Ledger/subledger/custody/NAV reconciliation ties to immutable approved snapshot.
3. Unknown or stale critical source blocks affected close or statement.
4. Correct sponsor/fund scope for partial batches; no misleading 'all complete'.
5. Late effective-date postings preserve known-at history and trigger governed correction.
6. Routine statements publish automatically only under valid delegated policy.
7. Exact issued PDF and evidence manifest are archived with hashes and seven-year minimum.
8. Crash/retry cannot create duplicate originals or notifications.
9. Recipient revocation before dispatch prevents unauthorized delivery.
10. Separate statuses for accounting close, document issuance and communication delivery.
11. Materiality thresholds cannot suppress a material mismatch or evade independent finance review.
12. Corrected close and statements retain original versions and notify authorized recipients as policy requires.

## Adversarial scenarios (proposed, unexecuted)
- ADV-435: custodian feed missing at deadline; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-436: stale price treated as current; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-437: GL balances but fund subledger differs; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-438: fund NAV mismatches pooled units; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-439: unsettled grant payment marked paid; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-440: check voided after cutoff; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-441: late backdated contribution arrives after issuance; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-442: fee accrual omitted; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-443: FX conversion uses wrong date; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-444: negative available-to-grant hidden; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-445: sponsor-level mismatch masked by fund-only exclusion; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-446: partial batch claims all statements published; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-447: controller approves own close; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-448: AI marks unknown reconciliation as passed; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-449: sponsor delegation expired; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-450: period reopens without close version increment; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-451: template upgraded during batch; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-452: brand revision changes after render; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-453: rendered table truncates financial line; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-454: accessibility validation fails; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-455: object upload succeeds but DB commit fails; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-456: DB issuance exists but object missing; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-457: retry issues duplicate original; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-458: retry sends duplicate notice; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-459: recipient authority revoked before send; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-460: provider accepted mistaken for delivered; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-461: corrected statement overwrites original; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-462: source snapshot expires before seven years; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-463: legal hold not propagated to statement evidence; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-464: one sponsor's close contaminates another; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-465: zero-activity fund omitted; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-466: consolidated statement double counts transfers; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-467: materiality tolerance silently increased; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-468: payment provider Unknown reclassified Failed and retried; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-469: month-end close delayed by irrelevant nonfinancial exception; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.
- ADV-470: PDF hash matches but statement does not reconcile; expected: authoritative controls, explicit exception, correct scope, immutable evidence and governed recovery.

## Open decisions
Define sponsor-specific close schedule and SLA, accounting basis, materiality, adjustment/restatement thresholds, partial-batch eligibility, CPA/CFO oversight, valuation source precedence, required accessibility outputs and distribution preferences. Validate Typst/Folio rendering and exact-byte archival separately. No claim of regulatory approval or operational readiness.

Status: proposed. ADV-435..ADV-470 not executed.

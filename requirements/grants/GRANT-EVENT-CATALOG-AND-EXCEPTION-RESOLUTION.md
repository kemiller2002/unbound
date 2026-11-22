# Grant event catalog and exception-resolution engine
Date: 2026-11-21 | Status: requirements only; no runtime, tests, live integrations or money movement implemented.

## Mission
Treat every grant as a sponsor-governed, evidence-backed series of immutable decisions and financial state changes. Detect deviations, identify safe next actions, resolve routine exceptions automatically within bounded authority, and escalate genuine legal, fiduciary, financial or factual uncertainty to authorized humans. Preserve the outcome, not the process.

## Event envelope
Every event requires eventId, eventType, schemaVersion, aggregateId, correlationId, causationId, idempotencyKey, sponsorId, partnerId, legalEntityId, DAFId, actor/agent identity, authority reference, occurredAt, recordedAt, effectiveAt (where distinct), payload checksum, evidence references, source version, access classification, append-only sequence, and optional reversal/supersession reference. Preserve recorded-time and valid-time semantics. Do not place restricted raw documents in general event payloads.

## Canonical event catalog (60)
**Recommendation and intent**
GR-EVT-001 GrantIntentRecorded
GR-EVT-002 GrantRecommendationCreated
GR-EVT-003 GrantRecommendationRevised
GR-EVT-004 GrantRecommendationWithdrawn
GR-EVT-005 GrantScheduleProposed
GR-EVT-006 GrantInstallmentDue
**Recipient and evidence**
GR-EVT-007 RecipientIdentityResolutionRequested
GR-EVT-008 RecipientIdentityResolved
GR-EVT-009 RecipientIdentityDisputed
GR-EVT-010 CharityEvidenceSnapshotPinned
GR-EVT-011 CharityEvidenceChanged
GR-EVT-012 RecipientBankDetailsSubmitted
GR-EVT-013 RecipientBankDetailsIndependentlyVerified
GR-EVT-014 RecipientBankDetailsChangeFlagged
**Eligibility and policy**
GR-EVT-015 EligibilityEvaluationRequested
GR-EVT-016 EligibilityEvaluationCompleted
GR-EVT-017 EligibilityHoldPlaced
GR-EVT-018 EligibilityHoldReleased
GR-EVT-019 SponsorPolicyEvaluated
GR-EVT-020 SanctionsScreeningCompleted
**Sponsor authority and commitment**
GR-EVT-021 SponsorApprovalRequested
GR-EVT-022 SponsorApprovalGranted
GR-EVT-023 SponsorApprovalDenied
GR-EVT-024 SponsorApprovalExpired
GR-EVT-025 GrantConditionsEstablished
GR-EVT-026 GrantCommitmentRecorded
GR-EVT-027 GrantCommitmentAmended
GR-EVT-028 GrantCommitmentReversed
**Accounting and funding**
GR-EVT-029 GrantPayableRecognized
GR-EVT-030 GrantPayableAdjusted
GR-EVT-031 GrantFundingPlanProposed
GR-EVT-032 GrantFundingPlanApproved
GR-EVT-033 FundingReservationCreated
GR-EVT-034 FundingReservationReleased
GR-EVT-035 InvestmentRedemptionRequested
GR-EVT-036 InvestmentRedemptionAccepted
GR-EVT-037 InvestmentRedemptionRejected
GR-EVT-038 InvestmentRedemptionPartiallySettled
GR-EVT-039 InvestmentRedemptionSettled
GR-EVT-040 ValuationCorrectionReceived
GR-EVT-041 AvailableCashReconciled
**Payment preparation, simulation and evidence**
GR-EVT-042 PaymentInstructionDrafted
GR-EVT-043 PaymentInstructionApproved
GR-EVT-044 PaymentInstructionSimulated
GR-EVT-045 PaymentInstructionSubmissionRecorded
GR-EVT-046 PaymentExecutionAcknowledged
GR-EVT-047 PaymentSettlementEvidenceReceived
GR-EVT-048 PaymentReturnReported
GR-EVT-049 PaymentReconciliationCompleted
GR-EVT-050 PaymentReconciliationBreakDetected
**Recipient relationship and close**
GR-EVT-051 GranteeNotified
GR-EVT-052 GrantAgreementExecuted
GR-EVT-053 GrantAcknowledgmentReceived
GR-EVT-054 GrantReportReceived
GR-EVT-055 GrantMilestoneVerified
GR-EVT-056 GrantClosed
**Exceptions and governance**
GR-EVT-057 ExceptionCaseOpened
GR-EVT-058 ExceptionResolutionProposed
GR-EVT-059 ExceptionResolutionApproved
GR-EVT-060 ExceptionCaseClosed

Events 045–049 model future external-provider evidence and/or synthetic simulation; they do NOT authorize or imply live payment execution.

## Exception case contract
caseId, class, severity, impacted grants/accounts/positions/recipient, trigger events, observed vs expected state, financial exposure, legal/control exposure, affected policies, evidence completeness, owner, assigned authority, proposed safe action, deadline, required approvals, dependencies, retries, compensating actions, decision history, resolution proof, close criteria and recurrence fingerprint.

## Exception classes and mandatory response
- **EXC-01 Eligibility drift:** source change or stale sanctions check after approval -> hold advancement, reevaluate evidence, request compliance decision.
- **EXC-02 Identity ambiguity:** same-name charity, changed EIN, fiscal sponsor confusion -> quarantine recipient association and require steward review.
- **EXC-03 Funding shortfall:** available liquidity below approved funding plan -> do not overdraw; propose approved alternate allocation or defer.
- **EXC-04 Redemption failure/partial fill:** reconcile positions and available cash; propose partial funding or replacement source within sponsor rules.
- **EXC-05 NAV correction:** pin original valuation and correction; calculate impact, accounting adjustment and reapproval threshold.
- **EXC-06 Ledger/custodian mismatch:** quarantine financial close and investigate independent evidence; do not fabricate balancing entry.
- **EXC-07 Payment state mismatch:** distinguish instruction, provider acceptance, bank settlement and grantee receipt; unresolved state is not paid.
- **EXC-08 Returned or recalled payment:** reopen payable/receivable as appropriate under accounting policy, verify beneficiary and seek authorization.
- **EXC-09 Bank detail substitution:** block payment instruction advancement until independently verified by an approved channel.
- **EXC-10 Duplicate/split grants:** detect idempotency/recommendation fingerprint and policy evasion; preserve legitimate recurring installments.
- **EXC-11 Grant conditions unmet:** suspend future installments; obtain sponsor determination and preserve donor communications.
- **EXC-12 Cross-tenant exposure:** deny disclosure, alert security and isolate affected projections and credentials.
- **EXC-13 Source outage or stale evidence:** use explicitly labeled last-known information only where policy permits, otherwise hold.
- **EXC-14 Approval authority conflict:** freeze transition and obtain valid delegation/dual approval.
- **EXC-15 Retroactive legal change:** run historical impact analysis; do not rewrite past approval.
- **EXC-16 Reporting discrepancy:** preserve grantee claim and contradictory evidence, seek clarification; do not auto-assert fraud.
- **EXC-17 Sponsor succession/DAF transfer:** reconcile authority and outstanding grant commitments before continuing.
- **EXC-18 Reconciliation break after close:** create post-close adjustment/reopening case according to policy, preserve original close package.

## Requirements
- **UNB-GEX-001:** Publish typed, versioned event schemas with compatibility tests and deterministic replay.
- **UNB-GEX-002:** All authoritative events append-only with linked corrections/reversals; Strata implementation must be verified.
- **UNB-GEX-003:** Enforce idempotency and per-aggregate ordering; handle late, duplicated and out-of-order external observations.
- **UNB-GEX-004:** Preserve event causation, correlation, evidence, actor authority, tenant and legal entity context.
- **UNB-GEX-005:** Every exception has accountable owner, severity, SLA, escalation, evidence and closure proof.
- **UNB-GEX-006:** Detect expected/actual state divergence across grants, GL, custody, bank, recipient and payment simulation.
- **UNB-GEX-007:** Exception detection never silently creates balancing accounting entries or alters external financial facts.
- **UNB-GEX-008:** Agents may diagnose, retrieve evidence and propose remedies, but execution authority is separately bounded and audited.
- **UNB-GEX-009:** Categorize resolutions as automated-safe, human-approved, counsel/compliance-reviewed or prohibited.
- **UNB-GEX-010:** No automated remedy may override a legal prohibition, sponsor discretion or required financial approval.
- **UNB-GEX-011:** Apply least privilege, partner isolation, PII controls and source-rights restrictions to case evidence and agent context.
- **UNB-GEX-012:** Use policy-driven financial exposure and reversibility thresholds for auto-remediation; default to hold when unknown.
- **UNB-GEX-013:** Every automated resolution produces a reason, evidence chain, policy version and before/after projection.
- **UNB-GEX-014:** Support compensation and reversal workflows instead of assuming distributed actions are atomic.
- **UNB-GEX-015:** Distinguish business completion, accounting recognition, cash settlement and grantee acknowledgment.
- **UNB-GEX-016:** A funding failure cannot be resolved by treating unavailable cash as available.
- **UNB-GEX-017:** NAV corrections trigger impacted grant/fund/accounting review with immutable historical valuations.
- **UNB-GEX-018:** Material recipient bank-detail changes require independent verification and dual control per policy.
- **UNB-GEX-019:** Source evidence changes trigger only impacted eligibility reevaluations with appropriate decision freshness.
- **UNB-GEX-020:** Support retry with bounded attempts/backoff, poison-message quarantine and operator replay.
- **UNB-GEX-021:** Provide case deduplication, dependency graph and parent/child cases to avoid alert storms.
- **UNB-GEX-022:** Provide outcome-based prioritization using legal risk, money exposure, deadline, recipient impact and confidence.
- **UNB-GEX-023:** Track false positives, time to detect, time to resolve, reopened cases and human interventions.
- **UNB-GEX-024:** Support deterministic replay and simulation of exception policy changes before production activation.
- **UNB-GEX-025:** Separate case resolution approval from posting to GL, trading, grant approval and future payment execution.
- **UNB-GEX-026:** Forma UI shows next best action, evidence, comparison, authority, aging and safe available remedies.
- **UNB-GEX-027:** Folio exports complete case dossiers, immutable decision histories and reconciliation evidence.
- **UNB-GEX-028:** All case/event APIs are governed, versioned and accessible without mandating microservices.
- **UNB-GEX-029:** Support sponsor, partner, program and DAF scoped access and multiple advisor/successor roles.
- **UNB-GEX-030:** Prevent automated case closure without explicit verification of expected financial and regulatory outcomes.
- **UNB-GEX-031:** Use provider-neutral payment states and a simulated adapter; no live rail selected or authorized.
- **UNB-GEX-032:** Preserve evidence of upstream system outage and distinguish unknown from confirmed failure.
- **UNB-GEX-033:** Escalate uncertain legal interpretations and suspected fraud rather than automatically deciding culpability.
- **UNB-GEX-034:** Require explicit schema version migration, event catalog crosswalk and event producer ownership.
- **UNB-GEX-035:** Model compensating actions as separately authorized, immutable business events, not edits.
- **UNB-GEX-036:** Validate independent financial sources before reconciling, with no self-confirming ledger/custodian loop.
- **UNB-GEX-037:** Support manual intervention with reason codes, role checks, evidence and immutable audit.
- **UNB-GEX-038:** Define case retention, privacy, legal hold and rights policies compatible with append-only history.
- **UNB-GEX-039:** Require synthetic fault injection and cross-domain tests before any auto-remediation class is activated.
- **UNB-GEX-040:** Never equate event emission with successful external execution or settlement.

## Resolution protocol
Detect -> correlate -> preserve evidence -> classify severity and authority -> determine safe options -> simulate financial/legal effects -> propose next best action -> obtain necessary approval -> execute only permitted internal action -> verify independently -> close with proof -> monitor recurrence. On uncertainty, preserve the current safe state and escalate.

## Illustrative F# contract (conceptual, uncompiled)
```fsharp
type CaseSeverity = Informational | Operational | Material | Critical
type ResolutionAuthority = AutomatedSafe | SponsorApproval | ComplianceReview | CounselReview | Prohibited
type ResolutionStatus = Proposed | Approved | Applied | IndependentlyVerified | Rejected
type ExceptionKind = EligibilityDrift | IdentityAmbiguity | LiquidityShortfall | NavCorrection | ReconciliationBreak | PaymentMismatch | SecurityIsolation | Other of string

type ExceptionProposal = {
    CaseId: string
    Kind: ExceptionKind
    Authority: ResolutionAuthority
    EvidenceIds: string list
    PolicyVersion: string
    ExpectedOutcome: string
}
```

## Adversarial and acceptance scenarios (20)
GEX-T01: Same event delivered three times -> one business effect.
GEX-T02: Redemption rejected after grant approval -> hold, never pretend cash exists.
GEX-T03: Partial redemption and alternate source -> new authorized plan, independent balance check.
GEX-T04: NAV correction retroactive to funding -> linked accounting review, no silent overwrite.
GEX-T05: Charity status changes after approval -> eligibility hold before simulated payment.
GEX-T06: Bank change social engineering -> independent validation, no instruction advancement.
GEX-T07: Provider acknowledgment arrives without settlement -> status remains unsettled.
GEX-T08: Simulated return after settlement -> new case and accounting policy path.
GEX-T09: GL and custodian both agree due to shared bad import -> independent third-source control detects weakness.
GEX-T10: Stale IRS data and unavailable source -> hold per freshness policy, no fabricated verification.
GEX-T11: Two same-name charities -> identity collision case, no automatic merge.
GEX-T12: Partner A private fraud investigation queried by Partner B -> access denied and audited.
GEX-T13: Duplicate grant submission with new request ID -> fingerprint review, no unapproved double grant.
GEX-T14: Recurring grant installment legitimately repeats -> not falsely deduplicated.
GEX-T15: Exception agent recommends legally prohibited remedy -> blocked and escalated.
GEX-T16: Late source event arrives after close -> reopen/review without destructive edits.
GEX-T17: Operator attempts case closure without proof -> denied.
GEX-T18: Changed resolution policy shadow simulation -> effects compared before activation.
GEX-T19: Cross-border grant uncertain -> counsel review, no automatic legal assertion.
GEX-T20: Requirements-stage simulated payment instruction -> no external rail invoked.

## Open decisions
GEX-ADR-001 Event naming alignment with enterprise event catalog and schema registry.
GEX-ADR-002 Strata ordering, durability, replay and projection verification.
GEX-ADR-003 Financial exposure thresholds for automatic safe resolutions.
GEX-ADR-004 Independent reconciliation evidence hierarchy.
GEX-ADR-005 Human approval and escalation matrix.
GEX-ADR-006 Case correlation and deduplication algorithms.
GEX-ADR-007 SLA tiers and queue priorities.
GEX-ADR-008 Error/retry/compensation semantics.
GEX-ADR-009 Security incident versus business exception boundary.
GEX-ADR-010 Test fixture, fault injection and deterministic simulation harness.

## Dependencies and implementation gates
Related: requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md; requirements/grants/MULTI-INVESTMENT-FUNDING-AND-PERCENTAGE-GRANTS.md; requirements/FULL-ACCOUNTING-SYSTEM.md; requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md; requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md; requirements/investments/NAV-UNITIZATION-VALUATION-ENGINE.md.
Before activation: validate actual Strata capabilities; reconcile event names with existing enterprise events; approve authority and accounting policies; build simulated payment adapter; execute deterministic and adversarial tests. None are complete at requirements stage.

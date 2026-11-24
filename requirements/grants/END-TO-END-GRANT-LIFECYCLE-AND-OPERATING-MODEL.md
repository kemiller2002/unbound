# End-to-end grant lifecycle and operating model
Date: 2026-11-21 | Status: requirements only; no implementation, executed tests, legal validation or live payment execution.

## Mission
Design the full donor-advised grant lifecycle as one outcome-oriented, API-governed process that coordinates donor intent, charity intelligence, sponsor discretion, policy, funding, accounting, recipient communications, conditions, monitoring and reconciliation. Preserve the outcome, not the process. The sponsor retains exclusive legal control and final authority; donor/advisor requests remain recommendations.

## Canonical lifecycle
Intent -> Draft recommendation -> Recipient identity resolution -> Eligibility and restrictions assessment -> Grant amount and funding plan -> Sponsor approval -> Commitment/payable assessment -> Investment liquidity and funding -> Payment instruction preparation -> **deferred live payment execution** -> settlement evidence/reconciliation -> grantee acknowledgment/reporting -> completion/monitoring/close.
Stages may run in parallel when independent; no implicit skipping of legal, sponsor or accounting controls. Exceptions and reversals are first-class.

## Domain model
GrantIntent, GrantRecommendation, GrantPurpose, GrantRecipient, FiscalSponsorArrangement, GrantRestriction, GrantSchedule, GrantEligibilitySnapshot, GrantApproval, GrantCommitment, GrantPayable, GrantFundingPlan, GrantFundingSource, RedemptionRequest, GrantAgreement, GrantCondition, GrantMilestone, PaymentInstruction, PaymentExecutionReference, SettlementEvidence, GranteeAcknowledgment, GrantReport, GrantCase, GrantEvent, GrantDecisionSnapshot.
Each entity has immutable ID, sponsor/partner/program/DAF scope, effective/recorded timestamps, actor/authority, version and provenance. Canonical append-only events with rebuildable projections; Strata interfaces remain subject to verification.

## Requirements
- **UNB-GRANT-001:** Recommendations never create donor legal entitlement or unilateral asset movement; sponsor authority governs approval.
- **UNB-GRANT-002:** Capture intent, purpose, beneficiary, suggested amount, anonymity/public recognition preferences and restrictions separately.
- **UNB-GRANT-003:** Support one-time, scheduled, recurring, conditional, multi-year and milestone-based recommendations with explicit approval and revalidation rules.
- **UNB-GRANT-004:** Support fixed-dollar grants and percentages of a precisely defined eligible valuation base, with valuation date/lock and rounding policy.
- **UNB-GRANT-005:** Support multiple investment funding sources with independently defined amount or allocation percentages, liquidity and source restrictions.
- **UNB-GRANT-006:** Separate grant percentage-of-base from investment-source allocation percentages; validate each independently.
- **UNB-GRANT-007:** Resolve canonical charity identity, legal payee, fiscal sponsor, project and payment destination as separate facts.
- **UNB-GRANT-008:** Pin versioned charity evidence, jurisdictional rules, sponsor policy and recipient eligibility to each decision.
- **UNB-GRANT-009:** Recheck time-sensitive identity, sanctions and eligibility conditions at policy-defined stages, including prior to eventual execution.
- **UNB-GRANT-010:** Apply sponsor, program, DAF, donor intent and restricted-fund policies in explicit precedence order with legal prohibitions always controlling.
- **UNB-GRANT-011:** Evaluate prohibited benefits, conflicts of interest, pledges, earmarking and other legal/policy concerns, escalating uncertain cases to qualified review.
> Adopted 2026-11-23: superseded by `RR-UNB-GRANT-011-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-GRANT-012:** Route human approvals according to role, amount, risk, relationship, exception and delegation limits; enforce dual control where required.
> Adopted 2026-11-23: superseded by `RR-UNB-GRANT-012-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-GRANT-013:** AI can prepare recommendations, collect evidence and identify issues but cannot override sponsor authority, legal blocks or financial controls.
- **UNB-GRANT-014:** Keep approval, grant commitment, accounting expense recognition, payable recognition, liquidity allocation, payment instruction and settlement distinct.
- **UNB-GRANT-015:** Recognize grants and payables only under approved accounting policy and applicable obligations, not automatically upon recommendation.
- **UNB-GRANT-016:** Ensure GL/subledger entries are balanced, attributable to DAF/fund/legal entity, append-only and linked to grant events.
- **UNB-GRANT-017:** Reserve or encumber available balances only under explicit accounting and fund-availability policy; prevent double allocation.
- **UNB-GRANT-018:** Model segregated custody, pooled unitized investments, omnibus accounts and external managers without conflating investment preference with trading authority.
- **UNB-GRANT-019:** Reconcile investment redemptions, cash availability, custody settlement, sponsor funding and grant payable independently.
- **UNB-GRANT-020:** Handle partial liquidity, failed redemption, stale NAV, corrected valuation, FX, fees and funding-source substitution with approval and audit trail.
- **UNB-GRANT-021:** Prepare provider-neutral payment instructions, beneficiaries, remittance references, fraud checks, idempotency keys and approval evidence; live execution remains disabled until separately authorized.
- **UNB-GRANT-022:** Distinguish prepared, approved, submitted, accepted, rejected, settled, returned and reconciled payment states; never infer settlement from submission.
- **UNB-GRANT-023:** Support grants to domestic public charities, qualifying fiscal sponsors and other permitted recipients, with legal review for nonstandard and international grants.
> Adopted 2026-11-23: refined by `RR-UNB-GRANT-023-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-GRANT-024:** Support grant agreements, restrictions, milestones, reporting obligations, conditional release and grant purpose amendments.
  - *Refined 2026-11-24 by RR-UNB-GRANT-024-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-NPG-006, decided-provisional):* Each grant MUST be assigned one GrantTermsTier by a counsel-approved deterministic rule: Tier 0 (routine grant to a Permitted recipient with no conditions) — no agreement; sponsor-standard award terms (charitable use, no goods or services to donor, return if not used for charitable purposes or if recipient status is lost before use) printed in remittance; Tier 1 (sponsor-imposed restriction, condition, milestone or multi-year schedule) — unilateral award letter recording the terms, with grantee acceptance optional unless the sponsor requires it; Tier 2 (ERRequired, or any grant counsel designates) — written commitment signed by an authorized grantee officer containing the counsel-approved §53.4945-5(b)(3) terms, captured electronically. Payment for Tier 2 MUST NOT be released before the signed commitment is recorded. Purpose amendments MUST create a new terms version and re-run the BenefitAndPurposeGate. A distribution to a §170(b)(1)(A) organization other than a disqualified supporting organization is not a §4966 taxable distribution whatever its stated purpose; purpose monitoring of such grants is optional sponsor policy, while §4967 benefit screens remain mandatory (XID-NPG-006). The text above is retained as originally written.
- **UNB-GRANT-025:** Support donor anonymity and communications preferences while meeting sponsor, grantee, legal and operational disclosure obligations.
- **UNB-GRANT-026:** Provide grantee onboarding, secure verification of organization and payment details, change-of-bank anti-fraud controls and acknowledgement collection.
- **UNB-GRANT-027:** Handle recurring grants when charity eligibility, DAF funding, donor role or sponsor policy changes between installments.
> Adopted 2026-11-23: refined by `RR-UNB-GRANT-027-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-GRANT-028:** Model cancellation, withdrawal, expiration, declined grants, returns, refunds, clawbacks, disputed grants and corrective regranting without erasing history.
- **UNB-GRANT-029:** Handle duplicate recommendations and replayed requests idempotently; detect suspicious repeated or split grants.
- **UNB-GRANT-030:** Support grant batching and straight-through processing for low-risk cases without bypassing required approval or accounting checks.
- **UNB-GRANT-031:** Maintain accessible donor, sponsor operator and grantee views with role-specific next best action, status and clear explanations.
- **UNB-GRANT-032:** Use Forma mobile-first WCAG 2.2 AA-targeted components and Folio reproducible grant letters, agreements, reports and audit packages.
- **UNB-GRANT-033:** Publish governed APIs for recommendations, eligibility, funding, approvals, commitments, payables, payment instructions, acknowledgments, reporting, exceptions and reconciliation.
- **UNB-GRANT-034:** Support partner/tenant isolation, scoped advisors, multiple participants and successor DAF accounts throughout the grant lifecycle.
- **UNB-GRANT-035:** Preserve donor intent and advisory history when advisor authority changes or DAF succession occurs; outstanding grants require sponsor-governed transition rules.
- **UNB-GRANT-036:** Model service-level objectives, queues, ownership, aging, exception severity, escalation and communications across the entire process.
- **UNB-GRANT-037:** Generate evidence-backed explanations of grant delays and required next steps without revealing restricted intelligence or private partner data.
- **UNB-GRANT-038:** Allow authorized retrospective replay of every decision using pinned rules, policies, evidence, NAV, balances, approvals and timestamps.
- **UNB-GRANT-039:** Distinguish grant award date, accounting recognition date, payment date, settlement date, grantee receipt and completion date.
- **UNB-GRANT-040:** Support tax/regulatory reporting evidence and financial-statement reconciliation without asserting unverified legal reporting obligations.
- **UNB-GRANT-041:** Support international grants only through separately approved due diligence, sanctions, equivalency/expenditure-responsibility and payment/legal frameworks as applicable.
  - *Refined 2026-11-24 by RR-UNB-GRANT-041-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-NPG-001, decided-provisional):* A recommendation to a foreign recipient MUST be routed deterministically to exactly one InternationalRoute: (1) RoutineListed — recipient holds a US IRS determination as §170(b)(1)(A) organization; (2) EquivalencyDetermination — current ED per UNB-XIN-NPG-001; (3) ExpenditureResponsibility — sponsor offers ER and UNB-RCN-GRT-003 case completes; (4) Intermediary — donor accepts an IntermediaryCatalog alternative (UNB-XIN-NPG-002); (5) Decline. Routes 2 and 3 additionally require the ForeignNexusDeclaration (UNB-XIN-NPG-009), sanctions screening of the payment chain (UNB-RCN-GRT-007) and, where a program applies, a SanctionsAuthorization (UNB-XIN-NPG-010). Routes offered are sponsor configuration; route selection is never made by AI. Repository-sourced equivalency determinations follow XID-NPG-001. The text above is retained as originally written.
- **UNB-GRANT-042:** Treat grantee impact reports as evidence with provenance, not automatically verified outcomes.
- **UNB-GRANT-043:** Support grant portfolios, charitable strategies, recurring commitments and longitudinal outcomes across multiple DAFs and participants, subject to permissions.
- **UNB-GRANT-044:** Model grant-specific fraud threats, payment detail substitution, fake charity identity, insider override, social engineering and conflicting approvals.
- **UNB-GRANT-045:** Implement versioned policies for minimum/maximum grant sizes, payout schedules, funding order, restricted assets and fee treatment.
- **UNB-GRANT-046:** Ensure grant workflow can operate with a simulated payment adapter and synthetic settlement evidence while real rails remain deferred.
- **UNB-GRANT-047:** Prevent payment preparation from bypassing accounting/eligibility checks, and prohibit live execution in requirements-stage environments.
- **UNB-GRANT-048:** Require human approval of material grant modifications after sponsor approval, with appropriate re-evaluation and new accounting effects.

## State machine invariants
1. Recommendation != approval; approval != unconditional payable; payable != cash available; payment submission != settlement.
2. Sponsor authorization is required before grant commitment or payment instruction advancement under approved policy.
3. No available balance may be consumed twice; allocations, liquidity and units reconcile to source positions.
4. Every grant decision is tied to sponsor, DAF, recipient legal entity, policy, evidence and effective-time snapshots.
5. Changes are appended as corrections/reversals/supersessions, never destructive edits to authoritative history.
6. Grantee bank changes require independent validation and anti-fraud checks.
7. A restricted or prohibited recipient cannot advance on the strength of AI recommendations.
8. No payment provider selected or live money movement authorized by this document.

## Illustrative cross-domain scenarios
- **GRANT-T01:** $25,000 grant funded from three investment sources, one redemption fails; no premature payable settlement.
- **GRANT-T02:** 2% grant of a valuation-locked eligible base; unrelated 60/40 source weights produce correct dollar amounts.
- **GRANT-T03:** NAV correction after approval changes source liquidity; adjustment and reviewer decision preserved.
- **GRANT-T04:** Charity loses eligibility after approval but before payment; grant held and reviewed.
- **GRANT-T05:** Fiscal sponsor and named project differ; legal payee is validated.
- **GRANT-T06:** Recurring grant installment fails sponsor policy after donor advisor succession.
- **GRANT-T07:** Duplicate request submitted from mobile; one recommendation created.
- **GRANT-T08:** Donor anonymity requested; lawful and operational grantee disclosures still applied.
- **GRANT-T09:** Approved grant becomes conditional agreement; accounting recognition follows approved policy.
- **GRANT-T10:** Bank detail change attempts to redirect payment; independent verification required.
- **GRANT-T11:** Payment instruction prepared and approved in simulation; live rail remains disabled.
- **GRANT-T12:** Simulated payment returned; payable, receivable or reversal entries follow approved accounting policy.
- **GRANT-T13:** Cross-partner access to grant evidence denied.
- **GRANT-T14:** International grant enters counsel/compliance review, no implicit equivalency determination.
- **GRANT-T15:** A grantee report conflicts with external evidence; preserve both as claims.
- **GRANT-T16:** Partial settlement and later reconciliation break remain visible.
- **GRANT-T17:** Grant withdrawn after approval; authority, reversal and recipient communication tracked.
- **GRANT-T18:** A sponsor auditor replays a historical grant decision and funding allocation.
- **GRANT-T19:** Grant schedule survives donor death subject to successor DAF and sponsor policies.
- **GRANT-T20:** Multi-year grant commitments and installment cashflows are separately tracked.

## Operating architecture
Domain services inside an F# modular monolith: GrantIntent, Recommendation, RecipientIntelligence, RegulatoryEligibility, SponsorPolicy, ApprovalAuthority, GrantCommitment, Accounting, PortfolioFunding, Liquidity, PaymentPreparation, GranteeRelationship, Reporting, Reconciliation, Monitoring. API contracts are boundaries, not a mandate for microservices. Strata persistence required, capabilities not independently verified. Forma UX and Folio documents. Governed agents are advisory and bounded.

## Decision points
GRANT-ADR-001: Legal/accounting triggers for unconditional grants and payable recognition.
GRANT-ADR-002: Sponsor discretionary approval and delegation matrix.
GRANT-ADR-003: Funding source priority, reserves and insufficient liquidity.
GRANT-ADR-004: Recurring and multi-year grants, policy rechecks and succession.
GRANT-ADR-005: Grantee identity, fiscal sponsorship and payment-detail verification.
GRANT-ADR-006: International grant policy and external legal requirements.
GRANT-ADR-007: Grant cancellation/return accounting.
GRANT-ADR-008: Charity report and outcome evidence confidence.
GRANT-ADR-009: Cross-domain event naming, replay and close.
GRANT-ADR-010: Payment adapter contract and simulation without live execution.
GRANT-ADR-011: Privacy/anonymity and mandatory disclosure.
GRANT-ADR-012: Sponsor service levels and automation thresholds.

## Dependencies and validation
Trace to requirements/grants/MULTI-INVESTMENT-FUNDING-AND-PERCENTAGE-GRANTS.md, requirements/FULL-ACCOUNTING-SYSTEM.md, requirements/investments/NAV-UNITIZATION-VALUATION-ENGINE.md, requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md, requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md, and architecture/CHARITY-KNOWLEDGE-MODEL-AND-ACQUISITION-PIPELINE.md.
Next artifacts: detailed grant event/state catalog, donor/grantee experience journeys, sponsor authority matrix, grant accounting posting examples, and exception test fixtures. Counsel, accountants and operations owners must validate jurisdiction-specific and accounting rules before activation.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-REQ-X-002-r1 (refines `REQ-X-002`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:28` @ `777a70d` (kept verbatim) · **Decisions:** RCD-017, RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** Grant approval and fund reservation MUST commit in one authoritative transaction against net grantable capacity, serialised on every constrained ancestor (fund, pool, legal entity, custodial account). The reservation unit MUST match the funding source: currency for settled cash, and units with a configured valuation haircut for pooled or positional sources. A grant that needs liquidation MUST be Approved and Reserved and then enter FundingPlanned/LiquidationPending (UNB-RCN-FIN-008 names, RCD-049), which creates no external commitment and no payment instruction, until redemption settles. If proceeds fall short, the sponsor-configured shortfall rule MUST apply and MUST NOT draw on capacity reserved for other grants.
- **Validation still required:** RCD-017: CFO/investment committee confirms buffer default; RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given $100 net capacity and two concurrent $80 cash-funded grants; when both are approved; then exactly one is ApprovedAndReserved and the other is Rejected with no reservation.
  - AC-2: Given a $10,000 grant funded from Pool A units with a 5% haircut; when it is approved; then units worth $10,500 at the pinned NAV are reserved and the grant is ApprovedPendingLiquidity.
  - AC-3: Given redemption proceeds of $9,800 under ShortfallRule=Hold; when settlement is recorded; then the grant is held, no payment instruction exists, and other grants' reservations are unchanged.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-002-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-004-r1 (refines `REQ-X-004`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:50` @ `777a70d` (kept verbatim) · **Decisions:** RCD-028
- **Requirement:** When a payment's dispatch or provider outcome is Unknown, the system MUST block any new dispatch, reissue or settlement posting for that payment chain. The Unknown MUST be resolved only through the rail's published UnknownResolutionProtocol, and declaring NotReceived MUST require the protocol's evidence combination plus approval by a principal independent of the dispatcher. The payment identity, idempotency key and attempt history MUST survive restore and failover. Unknown items MUST escalate through configured aging tiers.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given provider timeout followed by a restore from backup; when an operator retries; then the retry is refused and the original identity remains.
  - AC-2: Given an Unknown ACH past its settlement window with a no-execution attestation and no statement debit; when an independent approver declares NotReceived; then a linked reissue becomes eligible.
  - AC-3: Given an original payment that settles after a reissue was sent; when the settlement is recorded; then a duplicate recovery case opens and both are posted correctly.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-004-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-005-r1 (refines `REQ-X-005`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:61` @ `777a70d` (kept verbatim) · **Decisions:** RCD-028, RCD-048
- **Requirement:** Automated release MUST require destination evidence that meets the rail's minimum combination in the Treasury-published DestinationEvidence table, and the pinned destination version MUST match the version bound at authorization. Any destination change, including a corrected ACH Notification of Change, MUST create a new destination version through an independently authorized verification. The change requester and the verifier MUST be different principals. Rails or providers that cannot meet the minimum MUST be ineligible for automated release.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a provider returns a self-issued fingerprint that conflicts with the sponsor record; when release is evaluated; then release is blocked.
  - AC-2: Given an ACH NOC correcting the account number; when it is received; then a new destination version is created Pending and further entries are held until it is Effective.
  - AC-3: Given the requester tries to verify their own change; when verification is attempted; then it is rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-005-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-083-r1 (refines `REQ-X-083`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:646` @ `777a70d` (kept verbatim) · **Decisions:** RCD-015, RCD-028
- **Requirement:** The payment aggregate MUST persist six orthogonal dimensions (Authorization, Dispatch, ProviderKnowledge, SettlementEvidence, AccountingReconciliation, RecipientAcknowledgement) plus a WorkCase. ProviderKnowledge and SettlementEvidence MUST include Blocked. The check rail MUST include Outstanding, AgedOutreach, StaleDated, VoidRequested, VoidConfirmed and UnclaimedPropertyReportable. Provider callbacks MUST be authenticated and deduplicated on (provider, providerEventId) and on a semantic key. This model supersedes UNB-GRANT-022, UNB-PAY-001 and UNB-PAY-009 and the TX-13 merged state.
- **Validation still required:** RCD-015: Counsel review of ScreeningScope matrix; RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given the same provider event delivered three times with different IDs but the same semantic key; when callbacks are processed; then one accounting effect.
  - AC-2: Given an intermediary bank blocks a wire; when the block notice is recorded; then ProviderKnowledge=Blocked and no capacity is restored.
  - AC-3: Given a check outstanding past the stale threshold; when the void is requested and positive pay acknowledges it; then the state is VoidConfirmed and a reissue becomes eligible after eligibility recheck.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-083-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-GRANT-011-r1 (supersedes `UNB-GRANT-011`) — adopted-provisional
- **Original:** `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:26` @ `777a70d` (kept verbatim) · **Decisions:** RCD-014, RCD-016
- **Requirement:** Before sponsor approval and again at release, every grant MUST pass a BenefitAndPurposeGate evaluating: (1) the recipient class (UNB-RCN-GRT-001); (2) related-party payee or beneficiary (UNB-RCN-GRT-002); (3) the donor's AffirmationEvidence (UNB-RCN-GRT-010); (4) the declared purpose and host context, against a counsel-approved catalogue of benefit types (tickets, tables, auction items, memberships with more-than-incidental benefits, personal goods or services, tuition, political campaign purposes); (5) the sponsor's pledge policy (UNB-RCN-GRT-009). The gate's deterministic outcomes are Pass, Reject or Hold. Only Hold cases go to qualified review.
- **Validation still required:** RCD-014: Counsel Q3, Q4; RCD-016: Counsel confirmation that Notice 2017-73 remains reliance guidance
- **Acceptance criteria:**
  - AC-1: Given a recommendation with HostContext EventTicket; when it is submitted; then it is held or rejected per sponsor policy and is never auto-approved.
  - AC-2: Given a NoticeConditions sponsor and a pledge-linked grant with no other benefit; when evaluated; then Pass, with remittance pledge-suppression flagged.
  - AC-3: Given AI flags 'gala' in the purpose text; when the gate evaluates; then Hold, not Reject.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-GRANT-011-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-GRANT-023-r1 (refines `UNB-GRANT-023`) — adopted-provisional
- **Original:** `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:38` @ `777a70d` (kept verbatim) · **Decisions:** RCD-006, RCD-014
- **Requirement:** Grants MUST be made only to recipients whose RecipientClass maps to Permitted, or to ERRequired with a completed ER case, under the counsel-approved RecipientClass rule. Natural persons and non-charitable purposes MUST be rejected. Disqualified supporting organizations and recipients needing expenditure responsibility MUST NOT use the routine path. Recipients absent from IRS files MUST be verifiable through the EntityTypeEvidence matrix.
- **Validation still required:** RCD-006: Counsel Q2; RCD-014: Counsel Q3, Q4
- **Acceptance criteria:**
  - AC-1: Given a recipient coded as a private non-operating foundation; when a routine grant is submitted; then it is routed to the ER path or rejected per sponsor policy.
  - AC-2: Given a church absent from IRS files with matrix evidence; when evaluated; then Permitted.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-GRANT-023-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-GRANT-027-r1 (refines `UNB-GRANT-027`) — adopted-provisional
- **Original:** `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:42` @ `777a70d` (kept verbatim) · **Decisions:** RCD-028
- **Requirement:** Each recurring occurrence MUST be evaluated as a new grant at its due time against current authority, affirmation validity, common status, sanctions, sponsor policy and capacity, keyed by (scheduleId, scheduleVersion, periodKey). Held occurrences MUST resolve within OccurrenceHoldWindow or become Skipped. There is no catch-up unless policy allows it. After ConsecutiveAdverseHolds the schedule MUST be Suspended.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a revoked charity; when the occurrence is due; then Held, donor notified, no payment.
  - AC-2: Given a hold window that expired before reinstatement; when the next occurrence is due; then one grant only.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-GRANT-027-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-GRANT-012-r1 (supersedes `UNB-GRANT-012`) — adopted-provisional
- **Original:** `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:27` @ `777a70d` (kept verbatim) · **Decisions:** RCD-005
- **Requirement:** Grant approval authority MUST be routed under REQ-X-017. Tier 1 is deterministic board-delegated sponsor policy, permitted only where OD-01 is validated for that sponsor. Tier 3 is the human routing matrix by role, amount, risk, relationship and exception, with dual control where the sponsor policy requires it. The approver MUST be independent of the originator and of anyone who materially edited the grant.
- **Validation still required:** RCD-005: Counsel Q1 + sponsor board resolution
- **Acceptance criteria:**
  - AC-1: Given a sponsor with validated delegation and a complete low-risk grant; when it is evaluated; then approved by policy with zero human touches.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-GRANT-012-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-GRT-001 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-005 · **Parent:** UNB-GRANT-023 · **Decisions:** RCD-006, RCD-014
- **Requirement:** Before sponsor approval and again at payment release, the system MUST classify every grant recipient into a counsel-approved RecipientClass. Classes are: (a) 170(b)(1)(A) public charity not a disqualified supporting organization; (b) disqualified supporting organization; (c) private non-operating foundation or other recipient requiring expenditure responsibility; (d) foreign organization; (e) government unit; (f) natural person; (g) non-charitable or unknown. The system MUST deterministically map each class to Permitted, ERRequired, Prohibited or CounselReview under a versioned rule. It MUST reject classes (f) and (g) in every tier, with no human override unless counsel later approves a lawful exception route. It MUST NOT let AI output set a class.
- **Validation still required:** RCD-006: Counsel Q2; RCD-014: Counsel Q3, Q4
- **Acceptance criteria:**
  - AC-1: Given a recipient whose IRS record shows a Type III non-functionally-integrated supporting organization; when a routine grant is evaluated; then the result is Prohibited or CounselReview per the active rule, no reservation is created, and the rule version is recorded.
  - AC-2: Given a grant whose payee is a natural person; when the grant is submitted by any channel; then it is rejected at TX-10 and no human queue can approve it.
  - AC-3: Given a Type I supporting organization whose supported organization is controlled by the donor per the related-party graph; when evaluated; then classified as disqualified and held for counsel.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-002 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-006 · **Parent:** UNB-GRANT-011 · **Decisions:** RCD-014
- **Requirement:** The system MUST block, as a nonwaivable hold, any grant or payment whose payee legal entity, destination account owner or declared beneficiary matches a donor, advisor or related person in the fund's related-party registry. This applies to reimbursements, tuition, travel and fees paid to an advisor's firm from fund assets, unless counsel has approved that payment class. Evidence of the match MUST be recorded, and only independent compliance authority may release the hold, based on counsel's determination.
- **Validation still required:** RCD-014: Counsel Q3, Q4
- **Acceptance criteria:**
  - AC-1: Given a grant whose destination account owner name and tax ID match the advisor's LLC; when release is evaluated; then release is held, a case is opened, and no dispatch occurs.
  - AC-2: Given an AI agent recommends releasing the hold; when release is requested; then the request is rejected and the attempt is audited.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-002); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-003 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-034 · **Parent:** UNB-GRANT-041 · **Decisions:** RCD-014
- **Requirement:** For every grant classed ERRequired, the system MUST track an ExpenditureResponsibilityCase with: pre-grant inquiry evidence; a signed agreement containing the counsel-approved terms; a reporting schedule; receipt and review of each report; and the reporting inputs for the sponsor's return. It MUST block payment until the inquiry and agreement are complete, and block further grants to that grantee while a report is overdue past the configured period. An ER determination MUST NOT be reused across sponsors unless counsel approves a reliance agreement.
- **Validation still required:** RCD-014: Counsel Q3, Q4
- **Acceptance criteria:**
  - AC-1: Given an ER grant with no executed agreement; when payment release is attempted; then release is held with reason ER_AGREEMENT_MISSING.
  - AC-2: Given an ER grantee with an overdue report; when a new grant is submitted; then the new grant is held and the sponsor is notified.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-004 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-010 · **Parent:** UNB-GRANT-027 · **Decisions:** RCD-028, RCD-043
- **Requirement:** Each recurring occurrence MUST have the idempotency key (scheduleId, scheduleVersion, periodKey). An occurrence held for eligibility, sanctions, authority or funding MUST stay Held until the configured OccurrenceHoldWindow ends. If it clears within the window, it proceeds once. If not, it becomes Skipped with a reason. There is no automatic catch-up payment in a later period unless the schedule's sponsor policy explicitly allows catch-up. After ConsecutiveAdverseHolds held or skipped occurrences for an adverse reason, the schedule MUST move to Suspended and needs donor reconfirmation and a fresh affirmation to resume. The donor MUST be notified on every hold, skip and suspension.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a monthly schedule where the charity is revoked before the March occurrence; when the March occurrence executes; then it is Held, the donor is notified, and no payment is made.
  - AC-2: Given the charity is reinstated after the March hold window has expired; when the April occurrence executes; then exactly one April grant is created and March remains Skipped.
  - AC-3: Given the same occurrence key is processed twice after a crash; when the scheduler replays; then one occurrence and at most one grant exist.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-004); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-008 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-017 · **Parent:** REQ-X-004 · **Decisions:** RCD-028
- **Requirement:** For each rail, the system MUST define an UnknownResolutionProtocol: a provider status query by idempotency key at defined intervals; a reconciliation against bank statements after the rail's settlement window; and the evidence combination that permits declaring NotReceived (for example: provider attests no execution, plus no statement debit after the window, plus recipient confirms non-receipt). Declaring NotReceived requires independent approval and creates the linked reissue chain. Unknown items MUST age into escalation tiers at configured thresholds. They MUST NOT be retried or reissued outside this protocol.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given an ACH dispatch timeout with no statement debit after the settlement window and a provider no-execution attestation; when an independent approver declares NotReceived; then a reissue is created linked to the original, and the original can no longer settle without opening a duplicate case.
  - AC-2: Given an Unknown payment; when an operator attempts a direct resend; then the resend is rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-008); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-009 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-008 · **Parent:** UNB-GRANT-011 · **Decisions:** RCD-006, RCD-016
- **Requirement:** Pledge handling MUST be a sponsor L3 policy with values Prohibited, NonBindingOnly or NoticeConditions, on a counsel-approved baseline. Under NoticeConditions, the system MUST: (1) suppress pledge identifiers and pledge vocabulary from every sponsor-issued outbound artifact (check memo, ACH addenda, remittance advice, recipient portal, webhooks); (2) run the other-benefit gate; (3) include the no-deduction remittance statement. Inbound pledge references MUST be kept only as internal evidence.
- **Validation still required:** RCD-006: Counsel Q2; RCD-016: Counsel confirmation that Notice 2017-73 remains reliance guidance
- **Acceptance criteria:**
  - AC-1: Given a NoticeConditions sponsor and a donor-entered purpose 'pays my 2026 pledge, ref P-123'; when remittance is generated; then the remittance contains neither 'pledge' nor 'P-123' and the internal record keeps both.
  - AC-2: Given a Prohibited sponsor; when the donor affirms the grant pays a binding pledge; then the grant is rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-009); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-010 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-GRT-009 · **Parent:** UNB-GRANT-011 · **Decisions:** —
- **Requirement:** Every grant submission MUST carry an AffirmationEvidence record: actor; authority; sponsor; fund; draft hash; disclosure version and content hash; locale and presented text; channel; time; idempotency key; and explicit affirmative action, with no pre-checked or inferred consent. The server MUST reject submissions, including widget and API submissions, without valid evidence for the current disclosure version. A material change of amount, recipient, purpose or policy version MUST invalidate the affirmation. Recurring schedules MUST use a standing affirmation with a sponsor-configured validity and its own refresh triggers.
- **Acceptance criteria:**
  - AC-1: Given a widget submission whose affirmation token references a superseded disclosure version; when it is submitted; then it is rejected with AFFIRMATION_STALE.
  - AC-2: Given a donor changes the recipient after affirming; when the draft is submitted; then a fresh affirmation is required.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-010); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-011 (P2, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-035 · **Parent:** REQ-X-077 · **Decisions:** RCD-028
- **Requirement:** Sponsor-issued remittance and recipient-portal grant records MUST state the sponsoring organization as the legal donor and MUST include a counsel-approved statement that the donor or advisor may not claim a charitable deduction for the grant. Recipient CRM attribution exports MUST label the donor as an advisor-recommender, not the legal donor.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a grant with a named advisor; when remittance is generated; then the sponsor is the legal donor and the counsel-approved statement is present.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-011); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-012 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-GRT-036 · **Parent:** REQ-X-063 · **Decisions:** —
- **Requirement:** Embedded and API checkout requests MUST declare a typed HostContext: Donation, EventTicket, TableOrSponsorship, Auction, MembershipWithBenefits, Other. Contexts that carry a benefit MUST be rejected or routed to the benefit gate per sponsor policy. Widgets MUST be certified, and the server MUST cross-check the declared context against the host's registered page classification.
- **Acceptance criteria:**
  - AC-1: Given a checkout from a page registered as event ticketing; when a grant recommendation is submitted with context Donation; then it is routed to the benefit gate because the context and registration do not match.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-012); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-015 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-037 · **Parent:** UNB-GRANT-028 · **Decisions:** RCD-028, RCD-045
- **Requirement:** Returned, recalled, refunded or recipient-declined grant funds MUST be re-credited to the originating fund via a linked compensating entry. They MUST NOT generate a contribution acknowledgment or tax receipt. Capacity is restored only after reconciliation, and the donor is notified. A recipient MUST be able to decline a grant before release, which cancels the reservation. Accounting treatment is subject to CPA approval.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a charity returns a settled grant by check; when the return is reconciled; then the originating fund is credited once, no receipt is issued, and the grant shows ReturnedByRecipient.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-015); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-016 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-GRT-040 · **Parent:** REQ-X-020 · **Decisions:** RCD-012
- **Requirement:** When an adverse common-status, sanctions or authority event affects a payment that has been dispatched but not settled, the system MUST automatically: request stop, recall or void where the rail allows; record the request outcome as externally uncertain; open a linked case; and notify the sponsor. The grant MUST NOT be marked complete until the outcome is reconciled.
- **Acceptance criteria:**
  - AC-1: Given a check mailed yesterday whose recipient is revoked today; when the revocation is ingested; then a stop-payment and positive-pay void request is issued and the payment shows VoidRequested.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-016); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-GRT-017 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-018 · **Parent:** REQ-X-005 · **Decisions:** RCD-028, RCD-048
- **Requirement:** Treasury MUST publish a versioned DestinationEvidence table per rail. It names evidence types (for example bank-originated account-owner match, prenote or return absence, independent callback using a contact sourced separately from the change request, positive-pay reconciled clearing), assigns each an evidence class, and sets the minimum combination for automated release. Rails or providers that cannot meet the minimum MUST be ineligible for automated release.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given an ACH destination with a bank-originated owner match and a verified independent callback; when release is evaluated; then automated release passes G08.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-017); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-NPG-001:** For a foreign recipient without an IRS determination, the system MUST permit the routine (non-ER) path only when an EquivalencyDetermination record exists for the sponsor with: practitioner identity and type (attorney, CPA or enrolled agent) and attestation of Circular 230 standing; advice date; grantee taxable-year end; taxable year of the latest factual information relied on; public-support test period where used; law-version reference; and the advice document hash. The system MUST compute CurrentUntil deterministically per a counsel-approved rule encoding Rev. Proc. 2017-53 §3.03(1) (default: end of the grantee taxable year following the year of the factual information; for public-support advice, end of the second grantee taxable year after the five-year test period) and MUST treat the determination as expired on the earlier of CurrentUntil or a recorded law-change event affecting its basis. Approval and release MUST each re-check currency. Advice obtained by another grantor or through a repository MAY be relied on when it is current under §3.03(1), the practitioner or repository has authorized reliance by the sponsor in writing (reliance-letter reference recorded), and the sponsor has no contrary knowledge (XID-NPG-001).
  - Provenance: [UNB-XIN-NPG-001](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-NPG-001 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given an ED based on FY2025 calendar-year grantee data, advice dated 2026-03-01; when a grant is released on 2027-12-30; then release proceeds (current through 2027-12-31).
    - AC-2: Given the same ED; when release is attempted on 2028-01-02; then release is held with reason ED_EXPIRED and an ED-renewal work item is created.
    - AC-3: Given an ED whose practitioner type is 'consultant'; when approval is requested; then approval is rejected with ED_PRACTITIONER_NOT_QUALIFIED.
    - AC-4: Given a recorded law-change event tagged to Rev. Proc. 2017-53; when any grant relying on affected EDs reaches approval; then the grant is held for counsel review.
    - AC-D1: Given repository advice that is current but has no recorded reliance authorization for the sponsor; when a grant is approved; then the routine ED path is refused.
- **UNB-XIN-NPG-002:** When a recommendation names a foreign recipient for which the sponsor has no current EquivalencyDetermination and does not offer expenditure responsibility, the system MUST present the sponsor's configured IntermediaryCatalog entries (each a domestic §170(b)(1)(A) organization with recorded counsel-approved purpose wording) as alternatives, and MUST convert an accepted alternative into a domestic grant to the intermediary whose remittance purpose uses only the catalogue's approved wording (e.g. 'for the intermediary's program supporting <country/cause>'), never language requiring payment to the named foreign organization. The original foreign recommendation MUST be preserved as donor intent evidence. Wording MAY express a non-binding preference for the intermediary's work in a country or program and MUST NOT name a foreign grantee as a required recipient or require pass-through; the intermediary retains discretion and control (XID-NPG-002).
  - Provenance: [UNB-XIN-NPG-002](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-NPG-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given sponsor S has no ED for foreign org F and ER disabled; IntermediaryCatalog has I1; when donor recommends F; then the donor is offered I1 with approved purpose text and no grant to F is created.
    - AC-2: Given donor accepts I1; when the grant reaches release; then the payee is I1, recipient class is (a) and remittance text matches catalogue wording exactly.
    - AC-3: Given donor edits purpose to 'pay to F'; when submission; then rejected with EARMARK_TEXT_NOT_APPROVED.
    - AC-D1: Given remittance wording 'for transfer to Organization X in Kenya'; when the intermediary grant is created; then it is rejected with EARMARK_WORDING_NOT_PERMITTED.
- **UNB-XIN-NPG-005:** The system MUST apply a counsel-approved, versioned OrganizationChangeEffect table to every open grant and active schedule when a change event is recorded for the recipient: NameChangeSameEIN -> continue, update remittance name, no hold; AddressChange -> continue, re-verify destination only if destination changed; EINChange or MergerIntoSuccessor or SuccessorDesignated -> hold, require sponsor approval of payee substitution with new eligibility evaluation of the successor and donor notification; Dissolution or BankruptcyFiled -> cancel unreleased grants and suspend schedules, route dispatched-unsettled payments to UNB-RCN-GRT-016; Revocation -> hold; ReinstatementRetroactive -> re-evaluate held grants, release only if the reinstatement effective date covers the grant date and all other checks pass. Every effect MUST be recorded with the event evidence and table version. A same-purpose eligible merger successor receives open and recurring grants after sponsor approval with donor notice (no reconfirmation); a change of purpose or recipient class, or an ineligible successor, cancels the grant and asks the donor to re-recommend (XID-NPG-005).
  - Provenance: [UNB-XIN-NPG-005](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-NPG-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given charity C renames with same EIN; when event recorded; then open grants continue; no work item.
    - AC-2: Given C merges into D; when event recorded; then open grants Held; payee-substitution work item created; schedule suspended; donor notified.
    - AC-3: Given C revoked 2026-05-15 and grant held; reinstated retroactively to 2026-05-15; when reinstatement recorded; then held grant re-evaluated and released if other checks pass.
    - AC-4: Given C dissolves with a dispatched unsettled ACH; when event recorded; then UNB-RCN-GRT-016 recall flow starts; grant not complete.
    - AC-D1: Given a recurring grant to a charity that merges into an eligible successor with the same purpose; when the sponsor approves substitution; then the next occurrence pays the successor and the donor receives a notice without a reconfirmation task.
- **UNB-XIN-NPG-007:** The RecipientClass rule MUST include a class 'Section530AGeneralFundingContribution', disabled by default per sponsor, mapping to Permitted only when: a Treasury acceptance agreement reference identifying the sponsor as eligible donor is recorded; the qualified or approved class definition is recorded; the grant date falls within the rule's applicability window (taxable years beginning on or after 2026-01-01) and before its expiry (2029-09-30) or a successor rule; and the BenefitAndPurposeGate confirms the class is not defined by reference to the donor's or advisor's family. Absent any condition the class MUST map to CounselReview. Payments MUST go only to the Treasury-designated channel, never to an individual account holder. The class MUST remain disabled for every sponsor until the decision XID-NPG-003 is revisited.
  - Provenance: [UNB-XIN-NPG-007](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-NPG-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given class disabled for sponsor S; when donor selects Trump-account contribution; then option not offered.
    - AC-2: Given enabled, agreement recorded, grant date 2027-03-01; when eligibility runs; then Permitted.
    - AC-3: Given grant date 2029-10-01 with no successor rule; when eligibility runs; then CounselReview with RULE_EXPIRED.
    - AC-4: Given class definition limited to donor's grandchildren; when benefit gate; then Reject.
    - AC-D1: Given a sponsor admin; when they attempt to enable Section530AGeneralFundingContribution; then the setting is unavailable.
- **UNB-XIN-NPG-008:** When any rule, audit or human report identifies a completed payment as a possible taxable distribution (§4966), prohibited benefit (§4967) or excess benefit (§4958), the system MUST open an ExciseTaxIncident with: affected grants and amounts; suspected provision; parties (sponsor, fund managers who approved, donor/advisor, benefited person); knowledge evidence (approval records); recovery actions (repayment request, receivable); and a filing-inputs record for Form 4720 and, for §4958, Form 990 Schedule L Part I. Determinations of whether a violation occurred and who is liable MUST be made only by authorized counsel/CPA roles. The incident MUST block further grants to the same payee from the same fund until counsel disposition and MUST be reported to the sponsor's board-designated committee within a sponsor-configured period. Because §4966 and §4967 first-tier taxes are qualified first tier taxes abatable under §4962 on reasonable cause and timely correction, each incident MUST record reasonable_cause_evidence, correction_steps and correction_deadline, and MUST track filing of Form 4720 by each liable person (XID-NPG-007).
  - Provenance: [UNB-XIN-NPG-008](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-NPG-007 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given an audit finds a grant paid to an advisor's LLC; when incident opened; then further grants from that fund to that payee blocked; counsel task created; Form 4720 inputs draft.
    - AC-2: Given AI flags a possible benefit; when incident proposed; then incident is created in Suspected state; no liability field may be set by AI.
    - AC-3: Given counsel records 'no violation'; when disposition; then block lifted; record retained.
    - AC-D1: Given an incident opened for a possible §4967 benefit; when it is saved; then fields reasonable_cause_evidence, correction_steps and correction_deadline are mandatory and a Form 4720 filing task exists per liable person.
- **UNB-XIN-EVA-010:** The system MUST model at least these distinct hold types with a declared set of blocked commands: ProtectiveHold(charity|recipient|destination) blocks release and dispatch to that subject; FundProtectiveFreeze blocks outflow commands from the fund (grants, transfers, and fee accrual only if sponsor policy says so) but MUST NOT block inbound contributions, dividends, valuation, statements or records access; SanctionsBlock moves affected funds to a blocked-property account and blocks unblocking without a licence/authorization record; RecordsLegalHold blocks only retention disposition and deletion. Each hold MUST record type, scope, authority, reason and release authority, and no hold type MAY be released by AI. During a dispute FundProtectiveFreeze (XID-EVA-004): fees continue to accrue; investments follow the standing policy but advisor-directed allocation changes are rejected; recurring and new grants are held; contributions may be accepted; nothing is distributed until the sponsor resolves the dispute.
  - Provenance: [UNB-XIN-EVA-010](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-EVA-004 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given Fund F under FundProtectiveFreeze for co-advisor dispute; when a dividend and a new contribution arrive; then both post; a grant recommendation is accepted as recommendation but cannot be approved; statement issues normally.
    - AC-2: Given RecordsLegalHold on fund F records; when a grant approval for F is submitted; then the hold does not block it; disposition of F records is blocked.
    - AC-3: Given SanctionsBlock on payment P; when an operator attempts to return funds to fund F; then rejected BLOCKED_PROPERTY; requires authorization record.
    - AC-D1: Given a fund under dispute freeze with a monthly recurring grant; when the occurrence comes due; then it is held, the quarterly fee still computes, and a contribution received that day is accepted.
<!-- END ADOPTED: cross-industry-discovery -->

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-GRT-001 — Board-delegated deterministic approval:** Deterministic grant, renewal and fund-acceptance policies SHALL produce recommendations (ApproveRecommended, AcceptRecommended) until a sponsor board or bylaw-authorized committee resolution is recorded that adopts the version-pinned policy, delegates final approval of conforming items to a named officer or committee and names who reviews exceptions. Only then MAY conforming items be approved by the policy under that delegate's authority. Each policy version SHALL record its §4966(d)(3) fund managers (officers, directors, trustees, the delegate and any employee with final authority). Without a delegation the operating model SHALL be individual accountable batch decisions on AI-prepared dossiers (SPD-CLR-004).
  - Provenance: resolves RCD-005, SPD-ENT-006, RR-ADM-003-r1#Q1, RR-ADM-002-r1#Q1, SPD-GRT-001, RR-REQ-X-007-r2#Q1, SPD-CLR-004, RR-UNB-GRT-003-r1#Q2, RR-UNB-GRANT-030-r1#Q1; amends REQ-X-007, RR-ADM-003-r1, RR-REQ-X-007-r2, RR-UNB-GRT-003-r1, RR-UNB-GRANT-030-r1. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor board (or bylaw-authorized committee) resolution adopting the policy version and naming the delegate.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given no recorded resolution, when a conforming grant is evaluated, then its state is ApproveRecommended and it waits for a named approver.
    - AC-2: Given a recorded resolution delegating to the CFO, when a conforming grant is evaluated, then it is approved with the CFO as accountable authority and the policy version recorded.
    - AC-3: Given a resolution that expires, when the next grant is evaluated, then approval reverts to recommendation.
- **CNV-GRT-002 — Eligible recipient classes:** Release 1 SHALL support grants to domestic §170(b)(1)(A) public charities (including private operating foundations and §170(b)(1)(F) conduit foundations labelled 'legally excepted, unsupported in release 1' until enabled) and SHALL reject grants to natural persons. Governmental units for exclusively public purposes MAY be enabled by sponsor policy. Expenditure-responsibility grants, disqualified supporting organizations and direct foreign grants SHALL be architecturally supported but disabled. Rejection text SHALL say 'not supported by this sponsor or release', never 'prohibited by law', except for natural persons.
  - Provenance: resolves RCD-014, XID-ADV-001; amends UNB-XIN-NPG-003; related findings RCF-GRT-001. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a grant to an individual, when evaluated, then it is rejected with NaturalPersonProhibited.
    - AC-2: Given a grant to a private operating foundation in release 1, when evaluated, then the donor sees 'not supported by this sponsor' and the class is recorded as legally excepted.
- **CNV-GRT-003 — Charity status freshness and deep review:** Status SHALL be checked at approval and at release against the latest published Tax Exempt Organization Search / Pub 78, EO BMF and auto-revocation releases, recording each source's posting date; retrieval within 24 hours of publication is an operational target, not a legal threshold. Grants SHALL be made to the listed legal name; group-exemption subordinates SHALL be flagged for additional verification. Deep review SHALL default to 180 days (sponsor may tighten) with refresh 30 days before expiry and event-triggered invalidation. Cross-sponsor evidence SHALL be reused only as input; any known adverse public fact SHALL be considered. Release-time checks MAY use cached verified evidence within the approved freshness window per source and SHALL fail safe when stale. Informational changes without eligibility, authority, payment or policy impact SHALL NOT block payment.
  - Provenance: resolves RCD-006, RR-REQ-X-007-r3#Q1, SPD-GRT-002, RR-UNB-EXT-028-r1#Q1, RR-UNB-EXT-028-r1#Q2, SPD-ENT-005; amends REQ-X-007, REQ-X-018, RR-UNB-EXT-028-r1; related findings RCF-REG-001, RCF-GRT-001, RCF-GRT-002, RCF-GRT-003, RCF-GRT-024, RCF-GRT-038, RCC-GRT-003, RCC-GRT-005. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a recipient removed from Pub 78 after approval, when release runs, then payment is held with EligibilityChanged.
    - AC-2: Given cached status older than the source's approved window, when release runs, then a fresh check is required before payment.
    - AC-3: Given only a phone number change, when detected, then no payment hold is created.
- **CNV-GRT-004 — Shared public facts versus private investigations:** The common charity registry SHALL propagate only holds derived from verified public facts (IRS revocation, Pub 78 removal, OFAC listing, published judgments) re-verified by the platform from the primary source with source and timestamp. Sponsor-private investigation content, suspicions and decisions SHALL NOT propagate. A neutral 'enhanced review recommended' flag MAY be shared only under a written inter-sponsor information-sharing agreement limited to sponsors with pending or recent grants to the charity and containing no factual allegations. Sponsor-originated corrections SHALL be re-verified before updating the registry.
  - Provenance: resolves RCD-029, SPD-GRT-002; amends —; related findings RCF-GRT-025, RCC-GRT-004. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Inter-sponsor information-sharing agreement only if the neutral flag is enabled.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given sponsor A opens a confidential investigation, when sponsor B views the charity, then no indication of A's investigation is visible.
    - AC-2: Given an IRS revocation, when published, then every sponsor's pending grants to that EIN are held with the public source cited.
- **CNV-GRT-005 — Discretionary holds:** Discretionary holds SHALL be permitted only under a board-approved DiscretionaryGrantPolicy listing criteria (adjudicated findings, regulator actions, official pending matters such as indictment, attorney-general or IRS action or a government investigation the sponsor is informed of; media allegations only with elevated approval and a shorter maximum). Each hold SHALL record reason, evidence, owner, review deadline (maximum 90 days, renewable once on new evidence), escalation and automatic release; legally mandatory holds SHALL be distinguished and SHALL NOT auto-release.
  - Provenance: resolves XID-NPG-004, UNB-XIN-NPG-006#Q1, UNB-XIN-CLM-006#Q1; amends UNB-XIN-NPG-006. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor board resolution approving DiscretionaryGrantPolicy.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a hold based on a media report approved by a single grant officer, when saved, then it is rejected for insufficient authority.
    - AC-2: Given a discretionary hold reaching 90 days without new evidence, when the deadline passes, then it is released and the donor notified.
- **CNV-GRT-006 — Sanctions screening, dispositions and OFAC reporting:** Donors, advisors, successors, grantees and payors SHALL be screened at onboarding, delta-screened automatically on list updates and screened before each grant release against the current list version. A confirmed match SHALL resolve to Block (a blocked person has an interest in property in the sponsor's possession or control), Reject (prohibited transaction, nothing blockable) or Proceed (licence or exemption), never automatically to Blocked. Block or reject actions SHALL produce 31 CFR 501.603/501.604 report data due within 10 business days and blocked property SHALL enter the annual report of property held on 30 June. A true recipient match before approval SHALL refuse the recommendation, open a compliance task due in 10 business days with report data prepared (default: file a precautionary report) and release the reservation only if donor, advisors and successors screen clear. Dispatch SHALL be fenced atomically at the irreversible step so a match found during release prevents transmission. Human false-positive dispositions SHALL be reusable within the deciding sponsor until a subject attribute changes, the list entry changes, a new entry matches, 12 months elapse or the officer's authority is revoked for cause; cross-sponsor reuse SHALL NOT apply to donors or advisors and SHALL apply to recipients only where the relying sponsor has adopted a written reliance policy. Donor-facing status SHALL use the neutral catalog and never show list names or scores.
  - Provenance: resolves RCD-015, RCD-044, SPD-GRT-003, SPD-ADV-002, RR-UNB-EXT-004-r2#Q1, RCD-023; amends UNB-RCN-WF-024, RR-UNB-EXT-004-r2, UNB-EXT-004, UNB-RCN-GRT-007; related findings RCF-GRT-013, RCF-GRT-014, RCF-REG-006. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor compliance officer designation and approval of the ScreeningScope matrix and reuse policy.
  - Ownership: domain owner: Sanctions; financial system of record: Unbound program ledger; policy owner: Sponsor compliance officer; data owner: Sponsor; operational owner: Sponsor compliance; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a potential match found after the payment instruction was staged, when dispatch runs, then nothing is transmitted and the reservation remains.
    - AC-2: Given a disposition reused after the SDN entry's hash changed, when screening runs, then the reuse is refused and a new review opens.
    - AC-3: Given a held grant, when the donor views it, then the page shows 'under review' and no list or score text (Playwright assertion).
- **CNV-GRT-007 — OFAC general licences and FCPA diligence for foreign-related grants:** Reliance on OFAC general licences SHALL be versioned as {program, CFR section or GL number, Federal Register citation, eCFR as-of date, conditions} and re-verified unchanged at release; only targeted-program NGO licences of the December 2022 type SHALL be accepted by default, with comprehensive-embargo programs held for program-specific approval or a specific licence. Foreign-related grants (when enabled) SHALL apply risk-based FCPA diligence: grantee declaration on government ownership and official connections, sanctions and PEP screening, staged payments and use reporting for larger or higher-risk grants, and escalation on any affirmative answer.
  - Provenance: resolves UNB-XIN-NPG-009#Q1, UNB-XIN-NPG-010#Q1; amends UNB-XIN-NPG-009, UNB-XIN-NPG-010. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: deferred; operating models: E1, E2, E3, E4, E5.
  - Activation gate: OFAC specific licence for any activity outside a general licence; sponsor approval of the GL catalog.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a GL reliance whose eCFR text changed since approval, when release runs, then payment is held for re-approval.
- **CNV-GRT-008 — Intermediaries, equivalency determinations and fiscal sponsors:** Outbound wording to an intermediary MAY name its program, country or cause and SHALL NOT name a specific foreign organization, even as a non-binding suggestion; the donor's original recommendation SHALL be retained only as internal evidence. The automatic (non-expenditure-responsibility) foreign path SHALL require current written advice received from a qualified tax practitioner (directly, or through a repository whose practitioner issues the advice or a reliance letter to the sponsor), no contrary knowledge and currency per §53.4945-5(a)(5); other advice SHALL go to sponsor review under the general good-faith standard. No agreement copy SHALL be required for a §170(b)(1)(A) fiscal sponsor that retains discretion and control, evidenced by a published project list or signed attestation; projects that are individuals, for-profits or foreign entities SHALL require agreement review. The proposed §4966 regulations SHALL be tracked as proposed (not effective) on the rule watch list.
  - Provenance: resolves XID-NPG-001, XID-NPG-002, UNB-XIN-NPG-001#Q1, UNB-XIN-NPG-001#Q2, UNB-XIN-NPG-002#Q1, UNB-XIN-NPG-003#Q1; amends UNB-XIN-NPG-001, UNB-XIN-NPG-002, UNB-XIN-NPG-003. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: deferred; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Repository contract under which a qualified tax practitioner issues advice or a reliance letter to the sponsor.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given an intermediary grant whose purpose text names a foreign charity, when submitted, then it is rejected with EarmarkWordingNotPermitted.
    - AC-2: Given ED advice forwarded by another grantor, when evaluated, then the grant routes to sponsor review.
- **CNV-GRT-009 — Prohibited benefits, related parties and compensation from funds:** No compensation, referral payment or other payment SHALL be made from a fund to any donor, donor-advisor (including an advisor the donor designated with investment privileges), family member or 35% controlled entity of that fund. Grants SHALL be blocked to an advising charity from a fund that is a DAF; a fund whose sole beneficiary is the contributing charity SHALL be classified as a designated or agency fund, and reliance on the proposed definition for funds contributed and advised only by public charities SHALL be a recorded sponsor policy choice. Broad charitable classes defined by family SHALL be rejected and class grants SHALL remain disabled in release 1. Sponsor-engaged investment managers MAY be paid only under sponsor contracts with uniform program-wide fees and not designated by the donor for that fund.
  - Provenance: resolves RCD-021, XID-CRM-004, UNB-XIN-CRM-002#Q1, UNB-XIN-NPG-007#Q1, XID-CRM-002, UNB-XIN-CRM-006#Q1, UNB-XIN-ERP-003#Q1; amends UNB-XIN-CRM-002, UNB-XIN-CRM-006, UNB-XIN-NPG-007, REQ-X-076. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor board-approved account-type classification policy; executed investment-management agreements.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a payment from a fund to its donor-advisor's investment firm designated by the donor, when proposed, then it is blocked with AutomaticExcessBenefit.
    - AC-2: Given an organization-advised fund that is a DAF, when it recommends a grant to the advising charity, then the grant is blocked.
- **CNV-GRT-010 — Pledges, payee substitution and taxable-distribution remediation:** Pledge handling SHALL be an explicit sponsor policy choice from {Prohibited, NonBindingOnly, NoticeConditions (IRS Notice 2017-73 §7)} with NonBindingOnly as the onboarding suggestion. Payee substitution to a merger successor SHALL require a verified legal successor passing eligibility on its own EIN in the same recipient class, sponsor approval and donor notice; recurring series SHALL carry a notice-and-opt-out window before the next disbursement and SHALL be reconfirmed when purpose, class or eligibility changes; one succession record MAY drive bulk notices. A taxable distribution or prohibited benefit SHALL open a remediation record (recovery to the sponsor, reasonable-cause evidence, Form 4720 inputs per schedule, optional Form 843 abatement request under §4962, which is discretionary).
  - Provenance: resolves RCD-016, XID-NPG-005, UNB-XIN-NPG-005#Q1, XID-NPG-007, UNB-XIN-NPG-008#Q1; amends UNB-XIN-NPG-005, UNB-XIN-NPG-008; related findings RCF-GRT-028. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a merged recipient with 1,200 recurring series, when the succession is verified, then each donor receives notice and series continue only after the opt-out window passes without objection.
    - AC-2: Given a remediation record, when the tax year closes, then Form 4720 inputs list the sponsor and fund-manager amounts by schedule.
- **CNV-GRT-011 — Tier-2 review triggers and decision quality:** Non-expenditure-responsibility grants SHALL route to enhanced (Tier 2) review when any of these is present: donor, advisor or family affiliation or benefit signals; Type I or II supporting organization; fiscally sponsored separate-entity project; intermediary with a stated program or country preference; equivalency-determination-based foreign recipient; governmental unit; grant large relative to recipient revenue; unusual restrictions; ambiguous or conflicting eligibility evidence. Thresholds SHALL be by risk and decision type, not amount alone; sponsor-specific amount thresholds MAY be configured. Decision quality SHALL be checked by a common risk-stratified, error-bound sampling policy with qualified independent reviewers, increasing after material errors or policy changes.
  - Provenance: resolves RR-UNB-GRANT-024-r1#Q1, RR-UNB-GRANT-030-r1#Q1, RR-UNB-GRANT-030-r1#Q2, XID-CLM-004, RR-UNB-GRT-003-r1#Q2, RR-UNB-CHAR-013-r1#Q1; amends RR-UNB-GRANT-024-r1, RR-UNB-GRANT-030-r1, RR-UNB-CHAR-013-r1. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a grant to a Type I supporting organization, when evaluated, then it routes to Tier 2 regardless of amount.
- **CNV-GRT-012 — Decline reasons and reconsideration:** Declines SHALL use the sponsor-configurable neutral catalog: donor-facing text describes the policy outcome, never facts about a charity; sanctions, investigation and law-enforcement reasons map to COMPLIANCE_REVIEW; charity-facing communication is limited to status codes; free text is optional and drawn from approved phrases. Reconsideration SHALL default to a 60-day filing window and a 30-day response target, configurable with mandatory exceptions, decided by a reviewer independent of the original decision (contracted or board-appointed for single-reviewer sponsors); the original and reconsideration records SHALL both survive with outcomes {Upheld, Reversed, Modified, Withdrawn, Referred}. Charity corrections of shared facts and appeals of sponsor decisions SHALL be distinct paths.
  - Provenance: resolves XID-CLM-003, XID-CLM-001, SPD-ENT-002, UNB-XIN-CLM-001#Q1, UNB-XIN-CLM-004#Q1, XID-CLM-005; amends UNB-XIN-CLM-001, UNB-XIN-CLM-002, UNB-XIN-CLM-004. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a decline for an OFAC match, when shown to the donor, then the text is the COMPLIANCE_REVIEW phrase.
    - AC-2: Given a single-reviewer sponsor, when reconsideration is filed, then it is assigned to an appointed independent reviewer, not the original decider.
- **CNV-GRT-013 — Complex-asset intermediaries and fund-borne charges:** Complex-asset gifts expected to credit a DAF SHALL use the agent model: the sponsor is donee of record (acknowledgment, Form 8283 Part V, Form 8282, §4943 holding) and the intermediary is the sponsor's contracted agent. The donee model SHALL be activated only where the intermediary agreement gives it discretion over proceeds and the donor is told the deduction is for a gift to the intermediary. Service charges to funds (including outsourced vendor fees netted from proceeds) SHALL be listed in the disclosed schedule, reasonable with cost-based evidence and never paid to the donor, advisor, family or 35% entities; a recovery ratio above 1.10 SHALL trigger review, never service denial; fund-borne charges SHALL be reported as bearer shifts, not savings.
  - Provenance: resolves RCD-034, XID-ECO-002, SPD-ECN-005, UNB-XIN-ECO-018#Q1, UNB-SMP-ECN-001#Q1; amends UNB-XIN-ECO-018, UNB-SMP-ECN-001. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Executed intermediary agreement; sponsor-approved fee schedule.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given an intermediary agreement without a title clause, when a complex gift is configured, then activation is refused.
    - AC-2: Given a charge schedule with recovery ratio 1.25, when the annual review runs, then a CFO review task is opened and service continues.
- **CNV-GRT-014 — Tax-fit guidance and statutory disclosures:** The platform SHALL NOT provide individualized tax advice. Donor-facing tax content SHALL be generated from verified authority with citations and effective dates: DAF contributions do not qualify for the §170(p) non-itemizer deduction; the 0.5% itemized floor from 2026; acknowledgments state the sponsor's exclusive legal control and whether goods or services were provided; grants from the fund are not deductible by the donor. Schedule F manner-of-disbursement values SHALL use the IRS wording versioned by form revision.
  - Provenance: resolves RCD-030, UNB-XIN-NPG-004#Q1; amends UNB-XIN-NPG-004; related findings RCF-REG-009. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Grants and eligibility; financial system of record: Unbound program ledger; policy owner: Sponsor board via DoA; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given the donor tax summary, when rendered, then every statement cites its authority and effective year.
- **CNV-GRT-016 — Recorded instruction authority for multi-advisor funds:** Instruction authority SHALL come from the recorded fund agreement, never from marital status. Absent explicit any-one authority, all authorized parties SHALL approve or the ambiguity SHALL be resolved before execution; closure, succession and authority changes SHALL require joint approval; conflicting instructions SHALL be detected and capacity reserved atomically. Changing only the funding source of an unchanged grant SHALL require notice only when recipient, amount, purpose, timing, donor costs, investment exposure, restrictions and tax or accounting consequences are unchanged and the funding instruction is authorized; otherwise approval SHALL be obtained. Fixed and percentage-of-available-capacity grants (frozen at approval), the closing sweep and recurring fixed grants SHALL ship in release 1; cash-first funding SHALL be configurable. Verified ACH SHALL be the default rail, with checks and authorized hand delivery supported; instant rails SHALL be deferred behind the rail adapter boundary.
  - Provenance: resolves XID-ADV-004, XID-EVA-006, XID-TRS-002, RR-ADM-023-r1#Q1, UNB-XIN-EVA-012#Q1, SPD-FIN-004, RR-UNB-GFUND-012-r1#Q1, SPD-FIN-001, SPD-GRT-004, XID-BNK-004; amends RR-ADM-023-r1, UNB-XIN-EVA-012, RR-UNB-GFUND-012-r1. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor fund-agreement wording on authority modes.
  - Ownership: domain owner: Grants; financial system of record: Unbound program ledger; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor grant operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a joint fund with no any-one clause, when one advisor recommends a grant, then it waits for the co-advisor.
    - AC-2: Given a funding-source change that alters investment exposure, when submitted, then advisor approval is required.
<!-- END ADOPTED: convergence-2026-11-24 -->

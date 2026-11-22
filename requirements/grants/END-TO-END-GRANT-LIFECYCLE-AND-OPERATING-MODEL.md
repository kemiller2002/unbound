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
- **UNB-GRANT-012:** Route human approvals according to role, amount, risk, relationship, exception and delegation limits; enforce dual control where required.
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
- **UNB-GRANT-024:** Support grant agreements, restrictions, milestones, reporting obligations, conditional release and grant purpose amendments.
- **UNB-GRANT-025:** Support donor anonymity and communications preferences while meeting sponsor, grantee, legal and operational disclosure obligations.
- **UNB-GRANT-026:** Provide grantee onboarding, secure verification of organization and payment details, change-of-bank anti-fraud controls and acknowledgement collection.
- **UNB-GRANT-027:** Handle recurring grants when charity eligibility, DAF funding, donor role or sponsor policy changes between installments.
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

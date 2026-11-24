# Product owner resolution review

Status: proposed, not approved. Source: [generated register](../OPEN-QUESTIONS.md). Preserve original source records and do not edit generated register manually. This is a preliminary review, not a closure of questions. Underlying source and landscape evidence still require verification.

## Decision-specific recommendations and challenges

### SPD-ENT-002

Which reconciliation window and outcome set apply to reconsideration, and which record survives?

Existing proposal: UNB-XIN-CLM-001 survives; window default 60 days (RCD-046); outcomes {Upheld, Reversed, Modified, Withdrawn, Referred}; response 30 days.

Review: Preserve original and reconsideration records; proposed 60-day default subject to reconciliation with XID-CLM-001.

Authority: Product owner with sponsor operations. Original blocker: Y. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### XID-ADV-001

Which optional money-movement and legal features are in release 1 (card contributions, ACH debit intake, direct foreign grants, call recording, pooled investment vehicles, distribution partners, embedded checkout)?

Existing proposal: Exclude card intake, call recording, direct foreign grants (use intermediaries) and partner exit tooling from R1; this removes ~8 conditional P0s.

Review: Separate architecture, implementation and activation; require baseline exit/export capability.

Authority: Product owner + general counsel. Original blocker: Y. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-ADV-002

Is high-volume-low-balance a release-1 business model, and if so what minimum economics (fee floor, servicing class, digital-only) are acceptable?

Existing proposal: Not an R1 model unless UNB-XIN-ADV-001 is adopted and a fee floor passes the XID-ECO-002 reasonableness review.

Review: Design for low balances; activation conditional on modeled economics.

Authority: Product owner + CFO + counsel. Original blocker: Y. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-ADV-004

Default co-advisor authority mode by instruction kind (resolves XID-TRS-002 vs XID-EVA-006).

Existing proposal: Any-one for grants below threshold; AllRequired for large grants, transfers-out, closure, successor and authority changes.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner + counsel. Original blocker: Y. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-ERP-003

Confirm ERP boundary: AP, procurement, budgeting, HR, payroll, fixed assets are integrations; AR only for partner invoices.

Existing proposal: Adopt (consistent with RCD-013).

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: Y. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-INV-007

Which functions is Unbound book of record for, per sponsor model?

Existing proposal: FundAttribution and FundStatements authoritative always; GL configurable.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner + sponsor CFO. Original blocker: Y. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### SPD-CLR-005

What standing do Clarity and EDF evaluations have in the requirements gate, and what independent (non-same-model) validation is required before their conclusions are relied on?

Existing proposal: Treat both as evaluation vocabulary, not gates; require at least one human or different-model review of any conclusion that changes a P0/P1 requirement.

Review: Use Clarity/EDF as evaluation methods; independent review for consequential changes, not automatic approval.

Authority: Requirements owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-CMP-001

Which segment is Unbound's primary competitive entry point: D2C small-balance donors, the advisor channel, or community-foundation/independent-sponsor administration?

Existing proposal: Treat CF/independent-sponsor administration as the leading hypothesis to test (largest documented cost gap and observed switching), but do not commit until CF requirements (external GL, non-DAF fund types) are scoped and priced; keep D2C small-balance out.

Review: Leading hypothesis: independent sponsors/community foundations; scope non-DAF functionality separately.

Authority: product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-CMP-002

Should Unbound commit to publishing measured service levels (turnaround P50/P90 by rail, response times) and WCAG conformance to donors and charities?

Existing proposal: Yes, once measured; publish only measured figures with method and period.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-CMP-004

Should any external competitive claim require an evidence grade of verified-documented, including logged-in verification for capability-absence claims?

Existing proposal: Yes: no 'only/first provider' claim without logged-in verification and dated evidence.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: product owner / marketing. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ECN-002

How are small balances priced: minimum fee, DigitalMinimal low fee, employer sponsorship, or disclosed cross-subsidy?

Existing proposal: DigitalMinimal class funded by employer fee plus modest asset fee or a disclosed cross-subsidy from balances above a threshold; retain minimum fees only with a cost basis (UNB-SMP-ECN-001). Not an R1 model.

Review: Support disclosed alternative funding models; no hard-coded commercial model.

Authority: Product owner + CFO + counsel. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ENT-001

Should REQ-X-058..077 (and the landscape-wave outcome/hypothesis records) be reclassified as a non-normative capability index, with tests attached only to their carrier records?

Existing proposal: Yes. Keep each ID as an index entry linking to carriers (report section 2); keep deferred status where the feature is out of R1.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Requirements owner (product owner). Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ENT-003

What is the default statement notification and paper policy (RR-REQ-X-032-r1)?

Existing proposal: Generate and publish every period; notify by quarterly digest by default plus correction alerts; monthly notification as donor opt-in; paper on request or without e-consent, free as an accommodation.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner and sponsor CFO. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ENT-007

Apply the RecurringObligation register (UNB-XIN-ADV-001 part 1) to all servicing classes in release 1?

Existing proposal: Yes, as an inventory with legal basis class and interactions per account-year; keep DigitalMinimal budget enforcement deferred.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ENT-008

Move research tasks and open hypotheses (UNB-GAP-001..009, UNB-COM-001, UNB-HYP-003/009) to an evidence backlog with owners and due dates?

Existing proposal: Yes; GAP-001 shadow replay first because it replaces the economic model's assumed exception rate and minutes.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Program owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-FIN-001

Which percentage-grant bases and funding options ship in release 1?

Existing proposal: FixedAmount and Percentage of available-to-recommend only (fixed at approval), ResidualSweep for closing grants, one default cash-first waterfall; defer other bases, weights, templates and recurring percentages.

Review: Support fixed and percentage grants with explicit basis and reservation; retain recurring fixed grants; defer additional percentage modes.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-FIN-003

Confirm XRJ-ERP-002 (no native budgeting) over the adopted wording of RR-UNB-ACC-014-r1?

Existing proposal: Yes; adopt RR-UNB-ACC-014-r2.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-FIN-005

Remove native rebalancing, trade staging and pre/post-trade compliance (UNB-INV-031)?

Existing proposal: Remove; managers/custodians perform these; oversight via RR-UNB-INV-019-r1 drift monitoring and manager reviews.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-FIN-006

Must staff accounting workbenches (close, reconciliation) be phone-first, or WCAG 2.2 AA with reflow only?

Existing proposal: WCAG 2.2 AA including reflow for all surfaces; phone-first design only for donor/advisor journeys and approvals.

Review: Accessible desktop workbenches; mobile exception handling, approvals and escalation.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-GRT-005

Defer 50-state charity registration/standing acquisition and state-rule tracking until a decision consumes them?

Existing proposal: Defer; grantee state standing is not a grantor obligation (XRA-015) and was rejected as a gate (XRJ-NPG-002).

Review: Defer indiscriminate state data acquisition; retain ability to consume state evidence where applicable.

Authority: Product + counsel. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ORG-004

Default operating structure for the first sponsor.

Existing proposal: Solo sponsor on shared services (S1) with outsourced reconciliation, assurance, counsel and tax.

Review: Lean shared-services sponsor as reference model, not mandatory.

Authority: Product owner + sponsor board. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-ORG-005

Default assignment mode per case kind.

Existing proposal: See report 'Assignment models' default table (exception grants SkillBased; succession CradleToGrave; service StickyRelationship for advisor-distributed sponsors else RoundRobin; fraud Escalation-only).

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-PLT-001

In release 1 (one sponsor, platform operated for that sponsor), does any party need a SOC 2 (or SOC 1) report on the platform, and if not, which SOC2-family requirements become conditional?

Existing proposal: Keep SOC2-family evidence engineering (by-product evidence, integrity, missing-evidence detection) — it serves the sponsor's own audit and bank/custodian due diligence. Defer the SOC 2 system description, CUEC/CSOC and audit-period work until a user entity (second sponsor, partner) or counterparty contract requires a report.

Review: Assurance timing driven by auditor reliance and contracts, not sponsor count.

Authority: Product owner with sponsor CFO and CPA. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### SPD-PLT-002

Should delegated customer roles (advisors, viewers) be recertified annually with auto-suspension, or end on events with an annual no-action notice?

Existing proposal: Event-driven expiry (parent change, explicit expiry, inactivity) plus annual informational notice; keep quarterly workforce recertification.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner with sponsor compliance. Original blocker: N. Source: requirements/discovery/2026-11-24-simplification-economics/20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md (Simplification and economics).

### XID-ADV-011

Release-1 scope of the economics capability.

Existing proposal: Six metrics, monthly pack, AI metering and non-posting attribution with GL reconciliation; everything else P3 or Echelon offline.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner + CFO. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-CLM-005

Publish decision-time metrics to donors?

Existing proposal: Sponsor option.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-ECO-004

Allocation method for shared platform costs across sponsors (and whether shown to sponsors)

Existing proposal: Driver-based by weighted events and storage; showback with disclosed method; chargeback only by contract.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Commercial owner + CFO. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-ECO-007

Should donor-facing cost quotes be shown for charged operations, and is quote-overrun re-affirmation required?

Existing proposal: Yes for charged operations.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner + counsel. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-EVA-006

Default instruction-authority policy for multi-advisor funds (any-one, all, designated primary) per instruction kind

Existing proposal: Any-one for grants; all for transfers-out, closure and investment allocation

Review: Consolidate with XID-ADV-004; authority by instruction type and sponsor policy.

Authority: Product owner + sponsor counsel. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-INT-001

Is message legal classification (UNB-XIN-CRM-003 / UNB-XIN-REG-003) P0 for all channels or only for SMS and prerecorded voice?

Existing proposal: P0 for SMS and voice (statutory damages per message); P1 for email.

Review: Classify all outbound channels before activation; counsel validates channel-specific controls.

Authority: Requirements owner with counsel. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-OTH-001

Minimum notice period and maximum length for migration transition windows.

Existing proposal: 30-day notice; max 3 business days freeze; no Dec 15-Jan 5.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Product owner. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

### XID-SAS-003

Metering allocation keys and whether sponsors see their usage.

Existing proposal: Internal only at release 1; keys = accounts, API calls, AI tokens, storage.

Review: Retain as candidate pending source-level challenge and acceptance criteria.

Authority: Economics owner. Original blocker: N. Source: requirements/discovery/2026-11-23-cross-industry/15-OPEN-DECISIONS-AND-SCENARIOS.md (Cross-industry discovery).

## Next gate

Review all remaining product-owner confirmation and requirement questions individually; validate against original source records and daf-landscape; record alternatives, evidence, dependencies, reversibility, design/build/activation gates, acceptance criteria and reconsideration triggers. Do not mark approved without appropriate authority. No implementation in this mission.

# Multi-investment grant funding and percentage allocation
Date: 2026-11-21 | Status: owner-directed candidate requirements; no implementation

## Product outcome
A donor/advisor may recommend a grant expressed as a fixed monetary amount or as a percentage of an explicitly defined eligible investment balance, and may allocate the proposed funding across multiple investment accounts, pools, portfolios or positions. Sponsor retains legal approval, investment, liquidation and payment authority. Do not conflate the grant awarded to a charity with the investments liquidated to finance it.

## Requirement set
- **UNB-GFUND-001:** Grant recommendation supports FixedAmount(currency,amount) and Percentage(rate,baseSpecification,valuationPoint) as mutually explicit amount modes; a percentage without its base is invalid.
- **UNB-GFUND-002:** Percentage bases support approved choices: a named investment/pool, selected eligible investment positions, selected portfolios, total eligible DAF net asset value, or eligible available-to-recommend balance. These bases are not interchangeable; show the exact basis before confirmation.
- **UNB-GFUND-003:** Funding source selections support multiple eligible custody accounts, pool positions, investment portfolios and cash balances, with sponsor policy determining permitted source types.
- **UNB-GFUND-004:** A fixed grant amount may specify allocation by exact monetary amounts or percentages summing to 100%; mixed modes require explicit normalization policy, not implicit guessing.
- **UNB-GFUND-005:** A percentage-based grant may be funded pro rata across selected investments or by independently specified funding weights. The percentage that calculates grant size is separate from the weights used to source it.
- **UNB-GFUND-006:** Show projected charity award, source-level allocations, estimated liquidation proceeds, estimated fees, available settled cash, unsettled proceeds and funding shortfall independently.
- **UNB-GFUND-007:** Define valuation basis, price timestamp, cutoff, rounding, source currency, FX rate and expiration/requote threshold for percentage grants and liquidation estimates.
- **UNB-GFUND-008:** Support dollar amount and percentage input, editing and preview in mobile-first accessible Forma UI, including clear labels for percentage base versus allocation weights.
- **UNB-GFUND-009:** Validate source-level ownership/attribution, participant permissions, fund restrictions, sponsor liquidity policy, grant limits, pending redemptions, existing commitments and sufficient available balance before sponsor approval.
- **UNB-GFUND-010:** An advisory recommendation cannot directly trigger trade execution, grant payable recognition or disbursement; each has distinct authorized states and evidence.
- **UNB-GFUND-011:** When investments must be liquidated, generate a proposed funding plan that accounts for pool redemption cutoffs, liquidity gates, securities settlement, market holidays, fees and tax-lot rules where applicable.
- **UNB-GFUND-012:** Sponsor may approve the proposed allocation, modify it under policy with a recorded explanation, request renewed advisor consent where policy requires, or reject/hold it.
- **UNB-GFUND-013:** Market value movement after recommendation SHALL not silently alter a fixed-dollar grant. Percentage-based grants require an explicit policy deciding whether the amount locks at recommendation, sponsor approval or execution, with clear preview and immutable snapshot.
- **UNB-GFUND-014:** Support insufficient liquidity, price drift, partial redemptions, restricted assets, failed trades, failed settlement, custodian outage and grant cancellation with explicit recovery and reapproval rules.
- **UNB-GFUND-015:** Permit partial funding and staged settlement only under approved grant policy, and distinguish authorized grant amount from paid, payable and pending funding amounts.
- **UNB-GFUND-016:** Source allocation rounding SHALL be deterministic; for a fixed grant, the sum of source allocation amounts equals the intended funding amount, with documented residual handling.
- **UNB-GFUND-017:** Cross-currency allocations must use approved exchange-rate timing and separate FX risk/fees from the charity's award currency and amount.
- **UNB-GFUND-018:** Support reusable allocation templates and sponsor-approved default funding waterfalls (cash-first, pro rata, specified order), without overriding a donor's explicit approved preference silently.
- **UNB-GFUND-019:** Record immutable recommendation, amount calculation, selected source IDs, percentages, valuation evidence, approvals, funding plan, actual redemptions, accounting journals, grant payment and reconciliations.
- **UNB-GFUND-020:** Reconcile fund investment position reductions, unit redemptions, custody proceeds, cash, grants payable, payment settlement and fund available-to-recommend balance, with no double counting.
- **UNB-GFUND-021:** Prevent concurrent recommendations or liquidation requests from committing the same liquidity twice using controlled reservations, authorization and idempotency.
- **UNB-GFUND-022:** Grant funding instructions SHALL remain distinct from investment rebalancing requests; selling a position to finance a grant is not automatically a mandate to rebalance the remaining portfolio.
- **UNB-GFUND-023:** A grant may target one or multiple grantee recipients only through explicitly modeled separate grant awards and payment obligations; do not silently split a single recipient's award.
- **UNB-GFUND-024:** A percentage-based recurring grant requires explicit frequency, valuation point, maximum amount, minimum, renewal, sponsor approval policy and stop conditions; recurring percentages are not assumed from a one-time instruction.
- **UNB-GFUND-025:** Forma shall display source-by-source projected and actual funding, accessible on mobile, with clear estimated/final labels and screen-reader-readable breakdowns.
- **UNB-GFUND-026:** Folio shall generate reproducible grant recommendations, approval notices, funding breakdowns, grant letters, investment liquidation evidence and reconciliation reports with policy/source version references.
- **UNB-GFUND-027:** All funding allocation APIs shall be versioned, tenant/fund scoped, idempotent, independently authorized and auditable; AI can propose a funding mix but cannot self-approve trades or grant payments.
- **UNB-GFUND-028:** Partner isolation applies to selected accounts, positions, prices, allocations, source credentials, evidence, reports and AI context, including multi-custodian structures.
- **UNB-GFUND-029:** The system shall support fund-wide percentage grants when investments include both segregated accounts and unitized pools, with no double-counting of look-through assets.
- **UNB-GFUND-030:** Support in-kind grant proposals only where sponsor policy, recipient acceptance, legal eligibility and custody capability permit; default cash grant settlement must not assume assets can be transferred directly.

## Conceptual API contract
GrantAmountSpec = FixedAmount(currency,amount) | PercentOf(rate, base, valuationPolicy)
GrantFundingSpec = SourceWeights([{sourceId,percent}]) | SourceAmounts([{sourceId,currency,amount}]) | FundingWaterfall(policyId)
GrantRecommendation = {fundId, granteeId, amountSpec, fundingSpec, requestedAt, advisoryActor, evidence}
GrantFundingQuote = {calculatedAward, sourceValuations, proposedSourceAmounts, estimatedFees, estimatedLiquidation, settledCash, validUntil, warnings, policyVersion}
GrantFundingApproval = {sponsorDecisionId, approvedAward, approvedSources, quoteVersion, conditions}
FundingExecution = {approvedRedemptions, orderStatuses, settlements, journalRefs, paymentRefs, reconciliationRefs}
These are conceptual types, not compiled F# or verified provider API endpoints.

## Worked examples
**A. Fixed amount, weighted sources:** $10,000 grant, 60% Pool A and 40% Pool B. Planned source amounts: $6,000 and $4,000. If transaction costs apply, define whether the sponsor absorbs them, charges them separately, or requires a larger gross liquidation; the charity award remains $10,000 unless explicitly reapproved.

**B. Percentage of selected eligible holdings:** 5% of $200,000 eligible selected investment value at an approved valuation point = $10,000 proposed grant. Funding weights may independently be 70% from Pool A and 30% from segregated cash: $7,000/$3,000. If NAV changes before approval, lock/requote policy determines whether the grant remains $10,000.

**C. Percentage of a single investment:** 10% of eligible $50,000 Pool A position = $5,000 proposed grant, subject to pool redemption pricing and liquidity. This does not imply that exactly 10% of units can be liquidated at an earlier quoted NAV.

**D. Source amounts:** $2,500 from Pool A, $1,500 from Pool B, $1,000 from cash = $5,000 fixed award. The system must reject $5,100 allocations for a $5,000 net award unless separately authorized costs explain the difference.

## Acceptance scenarios
GF-T01: Fixed $10,000 split 60/40 across two pools; allocation sums exactly and fund units reconcile.
GF-T02: 5% of selected investments, with independent 70/30 source weights; award base and funding weights remain distinct.
GF-T03: A stale price makes percentage base unreliable; hold/requote rather than invent an amount.
GF-T04: Market moves after recommendation; fixed amount remains fixed, percentage grant follows explicit lock policy.
GF-T05: Restricted or illiquid asset cannot be redeemed; safe alternative or sponsor hold.
GF-T06: Two advisors concurrently recommend grants funded from same pool; no double-commitment.
GF-T07: Partial redemption and delayed settlement do not imply grant paid.
GF-T08: Fees and FX are shown separately from the charity award.
GF-T09: Mixed segregated and unitized holdings avoid look-through double counting.
GF-T10: Sponsor changes allocation; immutable decision and renewed consent where required.
GF-T11: Grant canceled after liquidation; preserve investment and accounting events, and route proceeds according to policy.
GF-T12: Cross-partner source ID injection is denied at API, agent, Forma and Folio boundaries.
GF-T13: Replayed approval/execution does not duplicate redemptions or payments.
GF-T14: Accessible phone experience accurately distinguishes 5% of fund from 5% allocation weight.
GF-T15: Source redemption NAV corrected after payment; financial impact and compensation workflow are explicit.
GF-T16: One-time grant cannot silently become recurring.

## Open decisions
GF-ADR-001: Default percentage base and whether restricted/illiquid assets are eligible.
GF-ADR-002: Lock point and requote policy for percentage amounts.
GF-ADR-003: Funding source default waterfall and donor override rules.
GF-ADR-004: Fee/FX gross-up and charity award amount policy.
GF-ADR-005: Grant funding reservation and release timing.
GF-ADR-006: In-kind grant eligibility and execution.
GF-ADR-007: Recurring percentage grant policy and approval renewal.

## Traceability
Extends UNB-NAV-001..053, UNB-INV-001..042, UNB-CUST-001..028, UNB-ACC-001..036, UNB-SUCC-001..014 and UNB-TEN-001..020. No application code, integration or test execution is claimed.

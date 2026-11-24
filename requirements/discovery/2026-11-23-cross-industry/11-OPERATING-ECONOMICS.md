# 11 — Enterprise operating economics and cost-to-serve (addendum §18)

**Status:** proposed and unvalidated. **No measured operating data exists.** Every unit time and every figure in the worked example below is tagged *Illustrative*. Sourced figures cite `daf-landscape`. The full research pass, including industry assessments and the redesigns of cost attribution, December staffing, the cost-reduction programme, AI spend and international grant pricing, is in [industry/eco.md](industry/eco.md).

**Objective.** Make operating economics a first-class platform capability, not just financial reporting. Every measurable expense should be associated with the business events, activities, customers and tenants that generate it. Where precise attribution is impossible, that should be stated honestly, with an attribution confidence grade.

## 1. Industries the model draws from, and where each analogy breaks

| Industry | Transfers | Does not transfer |
|---|---|---|
| Bank cost-to-serve and product profitability | Activity costing per account and product; multiple denominators; segment losses made visible | Full absorption of overhead into each fund; tiering service by how profitable a fund is |
| **Funds transfer pricing** | **Nothing.** It is not applicable: there is no deposit or loan book and no interest margin to split. A "float credit" would reward slow grant payment (XRJ-ECO-001). | — |
| Cloud FinOps (FOCUS v1.4 columns, read 2026-11-23) | Vendor cost schema; showback by default, chargeback optional; tagging when a resource is created; AI unit economics | Real-time allocation of every cost; separate FinOps tooling |
| Time-driven activity-based costing (Kaplan & Anderson 2004; abstract only) | Capacity cost rate × time equations; reporting unused capacity | Outcome pricing per approved grant, which rewards unsafe approval |
| Insurance loss-adjustment expense | Split between case-specific and pooled cost; a failure-and-recovery cost category | Reserving; combined ratio |
| Contact centre | Cost per resolved case; repeat-contact denominators | Erlang-C applied to multi-day review backlogs |

## 2. Enterprise operating economics requirements

There are 22 requirements: `UNB-XIN-ECO-001..022`. There are also four refinements:
- RR-UNB-ACC-014-r1: allocation in accounting must not post;
- RR-AIN-028-r1: exhausting an AI budget yields Incomplete, never a pass;
- RR-UNB-ENT-003-r1;
- RR-UNB-ECO-001-r1.

Every requirement has Given/When/Then acceptance criteria and is tested by a scenario.

<!-- BEGIN GENERATED: economics -->
| ID | P | Requirement | Acceptance criteria | UI / reporting implications |
|---|---|---|---|---|
| UNB-XIN-ECO-001 | P2 | The platform SHALL maintain operational cost attribution as a management-accounting projection (CostAttribution read model) derived from GL actuals, vendor cost records and domain events. Cost attribution records SHALL NOT create, modify or reverse any journal entry, fund balance, NAV, reservation, fee accrual or available-to-grant amount. Any economic consequence that must reach the books (fee invoice, chargeback, functional expense allocation) SHALL be raised only through the existing authorised accounting command for that purpose (UNB-ACC-010 fee/billing, UNB-ACC-019 functional allocation)… | [AC-1] Given a published AttributionRule version and a closed month; when the attribution run completes; then the GL journal count, every fund balance and every available-to-grant value are byte-identical before and after the run. [AC-2] Given an operator with cost-admin role; when they attempt to invoke a journal-posting command from the cost attribution API; then the request is rejected with COST_PROJECTION_NON_POSTING and an audit record is written. [AC-3] Given a sponsor contract configured for cost-plus chargeback; when the monthly chargeback amount is computed; then a draft invoice requ… | Every cost screen labelled "Management estimate - not financial statement" (REQ-X-043 official vs estimate). Cost reports carry non-posting disclaimer and ReconciliationRun id. |
| UNB-XIN-ECO-002 | P2 | The platform SHALL implement typed entities CostPool (GL account set + period + bearer + pool type in {Labour, Vendor, Infrastructure, AIInference, PaymentRail, CustodyBanking, Verification, FailureAndRecovery, Overhead}), CostDriver, Activity, ActivityEvent (domain event reference + activity + cost-object keys + touch minutes where captured), CostObject (kind in {Tenant, Sponsor, Account, Fund, Grant, Contribution, Case, CharityVerification, AIRun, Contact} with typed id), AttributionRule (versioned, effective-dated, approved), AttributedCost, UnallocatedResidual, ReconciliationRun and Reven… | [AC-1] Given an AttributedCost row; when it is validated; then it references an existing CostPool, CostObject and an AttributionRule version effective on the cost period; otherwise it is rejected. [AC-2] Given an AttributionRule v3 effective 2026-12-01 and v2 effective earlier; when November costs are re-run after v3 publication; then November rows still cite v2; a restatement under v3 is created only on explicit request and is labelled restated. [AC-3] Given a CostObject of kind AIRun; when its parent grant is queried; then the AIRun cost rolls up to the Grant, Fund, Sponsor and Tenant witho… | Rule catalogue screen with version diff. All cost reports cite rule versions. |
| UNB-XIN-ECO-003 | P2 | Every AttributedCost and every derived unit-cost figure SHALL carry method in {DirectMetered, TimeDriven, DriverBased, ResidualPolicy}, confidence grade in {A, B, C, D, U} and a low/expected/high range. Defaults: A DirectMetered by event-linked invoice or meter (range +/-2%); B TimeDriven with measured unit times (sample >= 30 per activity in the last 90 days) (+/-15%); C DriverBased proxy (+/-35%); D ResidualPolicy (+/-60%); U unattributable (reported only in UnallocatedResidual). Derived figures SHALL take the worst component grade unless >= 80% of the value is from better grades, in which… | [AC-1] Given a unit cost composed 90% of grade A and 10% grade C cost; when it is displayed; then it shows grade A with mixed flag and a range equal to the sum of component ranges. [AC-2] Given a TimeDriven activity whose unit-time sample count drops below 30 in 90 days; when the next attribution runs; then its rows are downgraded to grade C and a data-quality alert is raised. [AC-3] Given a cost figure without grade; when an export is requested; then the export fails validation with MISSING_CONFIDENCE. | Grade badge and range on every widget; accessible text, not colour only. Grade distribution per report. |
| UNB-XIN-ECO-004 | P2 | For each closed accounting period and sponsor scope, the platform SHALL execute a ReconciliationRun that compares the sum of AttributedCost plus UnallocatedResidual plus reported UnusedCapacity with GL operating expense actuals for mapped accounts, per CostPool. The run SHALL (a) require the identity sum(attributed)+residual+unused = GL actual for each pool to within $0.01, (b) record UnallocatedResidual explicitly by pool and reason (UnmappedGLAccount, MissingDriverData, NoEligibleCostObject, LateInvoice), (c) never distribute residual to cost objects except through a ResidualPolicy rule who… | [AC-1] Given pool GL actual $100,000, attributed $93,000, unused capacity $5,000; when the run executes; then UnallocatedResidual = $2,000 is recorded with reasons and the identity holds. [AC-2] Given residual 4% of pool exceeds tolerance; when the economics report is requested as final; then publication is blocked until a controller signs off; draft view shows ExceedsTolerance banner. [AC-3] Given a GL account not mapped to any pool; when the run executes; then its full amount appears as residual reason UnmappedGLAccount and a mapping task is opened. [AC-4] Given a late invoice posted after… | Reconciliation view in controller dashboard (Forma); Folio monthly economics pack. Residual trend line. |
| UNB-XIN-ECO-005 | P2 | Every external provider adapter that incurs cost SHALL emit VendorCostRecords in a FOCUS-aligned schema (BilledCost, EffectiveCost, ListCost, BillingCurrency, ChargeCategory, ChargePeriodStart/End, SubAccountId=sponsor/tenant where known, ConsumedQuantity/Unit, Tags including cost-object keys, x_Bearer in {Fund, Sponsor, Platform, Partner}, x_SourceInvoiceRef). Where a provider meters per request, the adapter SHALL attach the domain correlation id at call time so the cost is DirectMetered. Monthly invoices SHALL be ingested and reconciled to metered sums; differences go to residual. | [AC-1] Given a model API call made during a grant benefit-risk run; when the provider returns usage; then a VendorCostRecord with EffectiveCost, token quantity and the grant correlation id is stored within 5 minutes. [AC-2] Given a monthly payment processor invoice of $35,120 and metered sum $35,000; when the invoice is ingested; then $120 is recorded as residual LateOrUnmeteredCharge for the PaymentRail pool. [AC-3] Given a custodian bps fee borne by funds; when it is ingested; then x_Bearer=Fund and it is excluded from sponsor operating cost-to-serve but shown in donor all-in cost views. | Vendor spend widget. Spend by vendor and bearer. |
| UNB-XIN-ECO-006 | P2 | For each Labour CostPool the platform SHALL compute a capacity cost rate = pool cost / practical capacity minutes, where practical capacity = paid hours x configured practical-capacity factor (default 0.80) x 60. Each human Activity SHALL have a versioned time equation (base minutes + sum of driver increments, e.g. +12 international, +25 rework), with unit times sourced from touch-time samples or event-derived estimates and labelled with source. Attributed labour cost = time-equation minutes x capacity cost rate. Unused capacity cost = (practical capacity - used minutes) x rate SHALL be repor… | [AC-1] Given pool $180,000, 12 FTE x 160 h x 0.80; when rate is computed; then rate = $1.953125 per minute. [AC-2] Given 3,200 exception reviews at 18 min, 150 international +12, 400 rework +25; when labour is attributed; then used 69,400 min, attributed $135,546.88, unused capacity $44,453.12, sum equals $180,000. [AC-3] Given a case that waited 3 days on a provider; when its labour cost is computed; then only time-equation minutes are charged; waiting appears in process mining wait metrics, not cost. | Capacity utilisation widget per pool. Unused capacity by month. |
| UNB-XIN-ECO-007 | P2 | The metric registry SHALL contain versioned definitions (numerator, denominator, eligible population, inclusion/exclusion, time basis, grade rule) for at least: cost per account-month; cost per settled grant by rail; cost per settled contribution by type; cost per charity verification (new vs reuse); cost per resolved case (all touches until resolved); cost per contact; cost per manual exception by type; cost per human approval; cost per AI run and per model invocation; cost per automated workflow completion; custody/banking cost per $1m AUM; failure-and-recovery cost per 1,000 settled grants… | [AC-1] Given a request for cost per account for a sponsor; when the widget renders; then it shows cost per account-month by balance band plus cost per settled grant, with grade and population count. [AC-2] Given a grant that settled then returned; when cost per settled grant is computed; then it is counted once in the denominator (settled), and return-handling cost is included in the numerator under FailureAndRecovery. [AC-3] Given the definition of cost per resolved case changes; when a period comparison is shown; then the chart marks the definition version break and does not silently restat… | Unit Cost Explorer. Catalog. |
| UNB-XIN-ECO-008 | P3 | For operations whose estimated cost exceeds a configured threshold (defaults: AI agent run > $2; international grant ED/ER review; wire or check where ACH is available to the recipient; complex-asset liquidation), the platform SHALL produce a CostQuote {operation, estimate low/expected/high, components, bearer, rate-card versions, validUntil} before execution. CostQuotes SHALL be advisory: they SHALL NOT block, skip or reorder any legally or policy-required control step, SHALL NOT change recipient payment method selection, and SHALL NOT alter sponsor decisions. Where a configured service char… | [AC-1] Given an international grant recommendation with ED fee schedule $250-$600 (Illustrative); when the donor reviews before affirmation; then a quote shows range, bearer=fund, and the affirmation evidence records quote id. [AC-2] Given an AI run estimated at $3.40 above $2 threshold for a sanctions-adjacent review; when the run is requested; then the quote is logged and the run proceeds; the control step is not skipped. [AC-3] Given final ED charge $700 exceeding quoted high $600; when the charge is to be applied; then the charge is held and donor re-affirmation or sponsor waiver is requi… | Quote panel in grant flow; accessible range text. Quote accuracy (actual within range %) metric. |
| UNB-XIN-ECO-009 | P1 | Every model invocation SHALL record {AIRunId, model id/version, input/output tokens, tool calls, EffectiveCost, latency, cost-object keys, purpose}. Each AIRun SHALL roll up to its Grant/Case/Contact. When an AIRun reaches its budget the run SHALL stop issuing model calls, persist its partial state, mark result Incomplete-BudgetExhausted (never Pass), and route the work item to the appropriate human queue with the partial evidence; deterministic controls SHALL continue unaffected. Cost per AI run SHALL be alerted when exceeding 3x its 30-day median for the purpose. | [AC-1] Given an AIRun with $1.00 budget performing charity evidence summarisation; when cumulative cost reaches $1.00; then no further model calls occur; status Incomplete-BudgetExhausted; a human task is created with partial output. [AC-2] Given 100 AI runs for grants of sponsor S1; when cost per grant is computed; then AI cost equals the sum of their invocation EffectiveCost with grade A. [AC-3] Given a purpose whose median run cost is $0.10; when a run costs $0.35; then an anomaly alert is raised to the agent owner (CTL-027). | AI Spend widget. AI cost per outcome. |
| UNB-XIN-ECO-010 | P3 | The platform SHALL mine domain events per workflow (grant, contribution, charity verification, case, statement) weekly and report per sponsor and platform-aggregate: variant count and top variants by frequency and attributed cost; rework rate (activity repeated on same object); wait share (elapsed minus touch time / elapsed); touches per outcome; handoffs per case; and conformance to the modelled workflow. Each step SHALL be tagged with its PRINCIPLES classification (statutory/regulatory, fiduciary, contractual, sponsor policy, risk control, convention, unnecessary); steps tagged statutory, f… | [AC-1] Given a synthetic replay with 5% of grants seeded with a duplicated address-correction step; when mining runs; then rework rate for that activity is reported within +/-0.5 points and a LearningSignal is emitted. [AC-2] Given a required second approval for grants over threshold; when mining runs; then it appears as control cost, not as rework. [AC-3] Given a case waiting 2 days on provider confirmation; when wait share is computed; then waiting is attributed to the provider dependency, not to staff. | Process Efficiency screen with variant map (Forma workflow diagram pattern). Weekly variant report. |
| UNB-XIN-ECO-011 | P2 | AI-generated cost improvement recommendations SHALL be created only as REQ-X-042 InterventionProposals containing: target activity/variant, evidence links to mined events and AttributedCost rows, expected saving range with grade, affected counter-metrics and projected direction, PRINCIPLES classification of every step proposed for removal, and an explicit "AI-generated, unverified" label. AI SHALL NOT change any policy, configuration, routing rule, fee, threshold, staffing or control; proposals touching Tier C areas SHALL route to independent governance. | [AC-1] Given an AI proposal to remove a step classified risk control; when it is submitted; then it is routed Tier C and cannot be auto-deployed. [AC-2] Given an AI proposal without evidence links; when submission is attempted; then it is rejected with MISSING_EVIDENCE. [AC-3] Given the AI agent credential; when it calls a configuration-change command; then the call is denied and logged. | Proposal cards in Improvement Loop tracker. Proposal acceptance rate and realised vs projected. |
| UNB-XIN-ECO-012 | P3 | The platform SHALL derive RevenueRecords only from posted revenue (fee revenue, service charges, contract/SaaS fees, revenue share, float income where policy assigns it to the sponsor) with cost-object and service-model keys, and SHALL compute contribution margins CM1 = revenue - grade A/B direct cost; CM2 = CM1 - driver-based cost (grade C); CM3 = CM2 - residual-policy allocated overhead (grade D), by sponsor, service model (the 10 sponsor models), balance band and product. Fund- or donor-level profitability SHALL be visible only to finance roles and SHALL NOT be an input to grant approval,… | [AC-1] Given a sponsor with $520,000 posted fee revenue and attributed costs by grade; when CM levels are computed; then CM1, CM2, CM3 reconcile to revenue minus cost layers and cite the ReconciliationRun. [AC-2] Given the work-queue routing service; when it requests fund profitability; then access is denied by policy. [AC-3] Given a revenue line not yet posted (accrual pending); when profitability is computed; then it is excluded and flagged as pending. | Profitability screen (finance only). Margin by service model. |
| UNB-XIN-ECO-013 | P3 | The platform SHALL provide a ScenarioRun capability that takes a versioned parameter register (each parameter tagged Measured with ReconciliationRun reference, SourcedBenchmark, Assumption or Illustrative, with low/expected/high) and a candidate fee schedule or service model, and reports expected, one-at-a-time sensitivity, adverse/favourable corners, December surge case and a verdict in {robustly-superior, superior-at-expected, inferior-at-expected, robustly-inferior}. Scenario outputs SHALL be labelled Scenario and SHALL NOT activate fees, policies or staffing; activation SHALL go through R… | [AC-1] Given a candidate minimum fee change from $100 to $250; when a ScenarioRun executes; then outputs show revenue and fund-count impact by balance band with verdict and load-bearing parameters. [AC-2] Given a scenario using an Illustrative parameter as load-bearing; when the report renders; then it states the verdict rests on illustrative input and recommends measurement. [AC-3] Given a scenario result; when a user clicks apply; then no apply action exists; a link to start a REQ-X-045 policy change draft is offered. | Scenario Simulator screen. Scenario archive. |
| UNB-XIN-ECO-014 | P2 | Sponsors and the platform SHALL configure CostThresholds {metric, scope, level or trend rule, owner, minimum grade, effectiveDate, approver}. A breach SHALL create a work item for the owner with evidence. Thresholds SHALL only alert or open proposals; they SHALL NOT automatically throttle, defer, skip or downgrade any grant, contribution, verification, control or SLA. Level thresholds SHALL apply only to metrics of grade A or B; C/D metrics SHALL use trend rules. | [AC-1] Given threshold cost per settled ACH grant > $1.50 owner ops lead; when monthly value is $1.72 grade A; then a work item with drill-down link is created for the owner. [AC-2] Given a threshold on a grade C metric configured as level rule; when it is saved; then validation rejects it and suggests a trend rule. [AC-3] Given a breach on AI cost; when it fires; then no AI runs are cancelled; alert only. | Threshold configuration screen. Breach log. |
| UNB-XIN-ECO-015 | P3 | The platform SHALL support CostImprovementInitiatives with states Proposed -> Baselined -> Approved -> InExperiment -> Verified\|Refuted -> Ratcheted\|RolledBack, each with owner (operations), independent verifier (FP&A, not the owner), baseline (metric, period, grade >= B, seasonality-matched comparison period), target, counter-metric floors (ECO-016) and stop conditions. Verification SHALL compare like-for-like periods (same month prior year or matched cohort) and require the saving low bound > 0. Ratchet: next-period target = max(structural floor, min(current target, verified actual x (1 +… | [AC-1] Given baseline cost per exception $42.00 (grade B) and verified actual $36.00 with all floors met for two periods; when ratchet executes; then new target = $37.80. [AC-2] Given verified saving range -$1.00..$5.00; when verification runs; then status Refuted/inconclusive; no ratchet. [AC-3] Given the initiative owner; when they attempt to mark Verified; then denied; only the verifier role may. [AC-4] Given structural floor $30.00 and verified actual $25.00; when ratchet executes; then target = $30.00 and a review flag notes actual is below floor (investigate control erosion). | Improvement Loop tracker. Quarterly improvement report (Folio). |
| UNB-XIN-ECO-016 | P2 | Each CostImprovementInitiative and each CostThreshold-driven proposal SHALL declare counter-metrics from a mandatory set with configured floors: (1) control effectiveness - detection coverage and seeded-control test pass rate (floor default 100% for P0 controls); (2) compliance - sanctions/eligibility false-negative incidents (floor 0 known), §4966 referral rate change within +/-20% unless explained; (3) SLA - p90 grant approval-to-settlement within contracted SLA; (4) accuracy - correction/reversal rate per 1,000; (5) donor friction - task abandonment and repeat contacts; (6) accessibility -… | [AC-1] Given an initiative reducing review minutes; when seeded-control pass rate drops from 100% to 96%; then experiment halts, ratchet blocked, compliance notified. [AC-2] Given exception count falls 30% while detection coverage falls 25%; when savings verification runs; then the saving is not credited and is labelled detection-loss suspected. [AC-3] Given an initiative without counter-metrics; when approval is attempted; then rejected. | Counter-metric strip on every economics screen. Counter-metrics next to every cost KPI. |
| UNB-XIN-ECO-017 | P2 | The platform SHALL produce a rolling 13-week and next-December capacity forecast per sponsor and skill pool: forecast arrivals by work type (daily), expected handle minutes from time equations, eligible-staff hours net of absences, and a discrete daily backlog simulation that reports projected backlog, days to clear, p90 aging and staff-hours needed to meet each configured SLA. Forecast inputs SHALL be labelled (measured history, sponsor giving plan, assumption) and forecast accuracy (MAPE by week) SHALL be tracked. Forecasts SHALL inform staffing decisions by humans only and SHALL NOT reduce… | [AC-1] Given arrivals 640/day for 25 days and capacity 256/day (economic-model expected case); when simulation runs; then backlog ~9,600 and clearance ~223 days at 43/day surplus are reproduced. [AC-2] Given a configured SLA of 10 business days; when staff-for-SLA is computed; then the result (~54 FTE per economic-model review) is shown with grade and inputs. [AC-3] Given forecast vs actual for 8 weeks; when accuracy is computed; then weekly MAPE is displayed and below-threshold forecasts are flagged. | Capacity & Staffing Forecast screen. Staffing and Capacity report (REQ-X-049). |
| UNB-XIN-ECO-018 | P3 | For every configured service charge (e.g. international ED/ER review, wire, stop-payment/reissue, complex-asset handling, expedited grant), the platform SHALL report monthly recovery ratio = posted charge revenue / attributed cost of the corresponding activity, with grade, and SHALL require a recorded cost-basis memo and counsel review flag before a charge to funds is activated or increased where the expected recovery ratio exceeds a configured ceiling (default 1.25). Waivers SHALL be recorded with reason and counted separately. | [AC-1] Given ED review charge $400 and attributed cost $520 over 10 reviews; when recovery ratio is computed; then 0.77 is shown with grade. [AC-2] Given a proposed increase giving expected ratio 1.6; when activation is requested; then blocked pending counsel review flag. [AC-3] Given 5 waived charges; when the report renders; then waivers are shown with reasons and excluded from revenue. | Fee recovery table. Recovery report. |
| UNB-XIN-ECO-019 | P3 | For each automation (deterministic rule, AI agent, workflow change) the platform SHALL compute realised net benefit per period = baseline attributed cost - (post-change labour + AI/compute + vendor + oversight review + rework + failure-and-recovery cost + amortised build cost), using a frozen baseline and matched comparison period, with low/high bounds. An automation SHALL be reported as "saving verified" only when the low bound > 0 and counter-metrics met floors; otherwise "unverified" or "negative". | [AC-1] Given automation with baseline $50,000/month and post-change $38,000 +/- $9,000; when ROI computed; then reported saving verified with range $3,000-$21,000. [AC-2] Given an automation whose rework cost rose $15,000; when ROI computed; then rework is included and verdict negative if low bound < 0. [AC-3] Given no frozen baseline; when ROI requested; then status unverified-no-baseline. | Automation ROI widget. ROI register. |
| UNB-XIN-ECO-020 | P3 | For each SLA tier offered to sponsors/partners the platform SHALL report incremental staffing cost to meet the tier at forecast December peak (from ECO-017) and service-credit exposure; for each sponsor/partner contract it SHALL report contract-level lifetime value = sum of projected CM2 over contract term discounted at a configured rate, labelled Scenario, with retention assumptions sourced. Donor-level LTV SHALL NOT be computed for or exposed to operational services. | [AC-1] Given SLA tiers 5 and 10 business days; when costing runs; then incremental December staff-hours and cost for the 5-day tier are shown. [AC-2] Given a contract with 3-year term; when LTV is requested; then it is labelled Scenario with retention assumption source. [AC-3] Given a request for donor LTV from a routing service; when it is made; then denied. | Commercial view. SLA economics report. |
| UNB-XIN-ECO-021 | P2 | Cost views SHALL be scoped: sponsor users see only their sponsor's attributed costs plus their share of shared-platform costs with the allocation method disclosed; cross-sponsor benchmarks SHALL require at least 5 contributing sponsors and suppress any cell where one sponsor exceeds 60% of the total; labour costs SHALL be shown at pool/team/activity level with minimum group size 5 and SHALL NOT be shown per named worker or used to rank individuals. | [AC-1] Given sponsor S1 admin; when they open shared hosting cost; then they see their allocated share and the driver, not other sponsors' amounts. [AC-2] Given a benchmark with 4 sponsors; when it is requested; then suppressed with explanation. [AC-3] Given a team of 3 reviewers; when labour cost by team is requested; then it is merged into the parent group. | Suppression messaging accessible. Benchmarks with k. |
| UNB-XIN-ECO-022 | P3 | The platform SHALL provide the Operational Efficiency dashboards defined in report section "Operational efficiency dashboard requirements" (Economics Overview, Unit Cost Explorer, Attribution & Reconciliation, Capacity & Staffing, AI Spend, Process Efficiency, Improvement Loop, Profitability, Scenario, Thresholds) using pinned Forma widgets, with every cost figure showing grade, range, as-of, rule version and drill-down to AttributedCost rows and source events within the user's scope; and a Folio monthly Operating Economics pack issued only after the ReconciliationRun is within tolerance or s… | [AC-1] Given a cost KPI tile; when a user activates drill-down; then they reach the contributing AttributedCost rows and the originating domain events within their sponsor scope. [AC-2] Given reconciliation ExceedsTolerance unsigned; when the monthly pack is generated; then it is watermarked Draft and cannot be issued. [AC-3] Given a screen-reader user; when they navigate a cost chart; then an accessible table alternative with grade and range text is available. [AC-4] Given an export; when it is downloaded; then it contains metric definition version, grade, range and disclaimer columns. | See report section. Folio pack. |
<!-- END GENERATED: economics -->

### The seven architectural options (addendum §18, items 1–7)

| # | Option | Decision | Argument for | Argument against | Simplest valid alternative |
|---|---|---|---|---|---|
| 1 | Real-time or near-real-time cost attribution | **Partial.** Near-real-time only for directly metered AI, payment-rail and verification costs. Everything else is attributed monthly, after close. | Fast feedback on AI and rail spend | Pooled labour and infrastructure cannot be attributed honestly before the GL closes | Monthly attribution, plus real-time metering of variable vendor costs |
| 2 | Cost forecast before expensive operations | **Yes, advisory** above thresholds (AI run over $2, international ED/ER review, wire or check where ACH is possible) | Stops runaway AI cost; gives donors a quote for charged operations | Must never skip a control or change the payment rail | Quote plus re-affirmation if the cost overruns (UNB-XIN-ECO-008) |
| 3 | Automatic identification of inefficient workflows | **Yes**, through deterministic weekly process mining of rework loops, wait time and touch count | The event log already exists | Control steps could be mislabelled as waste | Control steps are never classed as waste (UNB-XIN-ECO-010) |
| 4 | AI-driven improvement recommendations | **Advisory only**, as REQ-X-042 intervention proposals. AI has no write access to configuration. | Finds patterns at scale | Policy changes are human authority | UNB-XIN-ECO-011 |
| 5 | Profitability across business models | **Yes**, at sponsor, service-model and balance-band level. Finance-only access. Never used for operational decisions. | Shows whether high-volume, low-balance accounts are viable | Service tiered by fund profitability would be harmful | CM1–CM3 from posted revenue only (UNB-XIN-ECO-012) |
| 6 | Simulation of service and pricing strategies | **Echelon offline first.** Build into the platform only if it is used monthly. | Pricing decisions need sensitivity analysis | An in-platform simulator is costly and rarely used | ScenarioRun over a versioned parameter register; no apply button (UNB-XIN-ECO-013) |
| 7 | Configurable cost and margin thresholds | **Yes, alert-only** | Early warning | Automatic action on a cost breach could degrade controls | UNB-XIN-ECO-014 |

## 3. Cost attribution and profitability model

**Boundary (P1, UNB-XIN-ECO-001).** Cost attribution is a *management-accounting projection*. It never posts to the financial ledger and never changes fund balances or available-to-grant. Book effects happen only in two ways:
- billing (UNB-ACC-010), through approved invoices;
- CPA-approved functional-expense allocation (UNB-ACC-019).

Before this boundary, UNB-ACC-014 put "allocation methodologies" inside the accounting system with nothing stopping them from posting (XIF-ECO-002, XIC-ECO-001).

### Entities (UNB-XIN-ECO-002)

| Entity | Content |
|---|---|
| CostPool | GL account set, period, bearer (Fund, Sponsor, Platform or Partner), pool type (Labour, Vendor, Infrastructure, AIInference, PaymentRail, CustodyBanking, Verification, FailureAndRecovery, Overhead), and practical capacity for labour pools |
| CostDriver / Activity | Each activity carries its PRINCIPLES classification (statutory … convention) and a versioned time equation |
| ActivityEvent | Derived from existing outbox events (UNB-XIN-EVA-006 envelope) |
| CostObject | Tenant, Sponsor, Account, Fund, Grant, Contribution, Case, CharityVerification, AIRun, Contact. Roll-ups never double-count. |
| AttributionRule | Versioned and effective-dated. Method is one of DirectMetered, TimeDriven, DriverBased or ResidualPolicy. |
| AttributedCost | One pool, one object and one rule version per row, with a confidence grade and a range |
| UnallocatedResidual | Has a reason code and is **never silently spread** |
| UnusedCapacity | Never loaded onto cost objects |
| ReconciliationRun | Versioned monthly tie-out to GL actuals |
| RevenueRecord | Built from posted revenue only |
| VendorCostRecord | FOCUS-aligned (UNB-XIN-ECO-005) |
| Metering feeds | UsageRecord (UNB-XIN-SAS-009), CostDriver facts (UNB-XIN-BNK-005) and work-item effort tags (UNB-XIN-ERP-006) all map into the canonical entities (XIF-INT-008) |

### Allocation methods and uncertainty (UNB-XIN-ECO-003)

| Grade | Method | Range |
|---|---|---|
| A | Directly metered | ±2% |
| B | Time-driven, with at least 30 samples in 90 days | ±15% |
| C | Driver-based | ±35% |
| D | Policy allocation | ±60% |
| U | Unattributable | Reported as residual |

Grade rules:
- A derived figure takes the **worst** grade of its inputs, unless at least 80% of its value has a better grade. In that case it takes the dominant grade plus a "mixed" flag.
- A cost figure without a grade is a validation error.

### Formulas

- **Capacity cost rate:** r = pool cost ÷ (paid hours × 0.80 × 60).
- **Labour cost** = Σ (time-equation minutes × r).
- **Unused capacity** = (capacity − used) × r.
- **Driver-based cost** for cost object o = pool × d(o) ÷ Σd.
- **Per-pool reconciliation identity (UNB-XIN-ECO-004):** Σ attributed + unused + residual = GL actual, to $0.01. A run outside tolerance (default 2% or $5k, configurable) keeps the pack in *Draft* until the controller signs off.
- **Contribution margins:**
  - CM1 = revenue − grade A/B costs;
  - CM2 = CM1 − grade C costs;
  - CM3 = CM2 − unused capacity − overhead;
  - CM3 − residual = revenue − GL operating expense.
- **Target ratchet (UNB-XIN-ECO-015):** next target = max(floor, min(target, verified × 1.05)). It applies only after 2 periods within the floors.
- **Backlog:** B(d+1) = max(0, B(d) + arrivals − capacity).

### Worked example (Illustrative)

Synthetic sponsor S1, one month. GL operating expense is $508,000. Mapped pools total $500,000. One unmapped account holds $8,000. Fee revenue is $520,000.

| Pool / item | Method / grade | Attributed | Unused | Unit figures |
|---|---|---|---|---|
| Grant operations labour ($180,000 pool; 92,160 practical minutes; $1.953125/min) | Time-driven, B | $135,546.88 (69,400 min) | $44,453.12 | — |
| Contact centre ($60,000) | Time-driven, B | $46,875.00 | $13,125.00 | $15.63 per contact; $23.44 per resolved enquiry |
| Compliance ($50,000) | Time-driven, B | $49,218.75 | $781.25 | — |
| Verification ($20,000) | Metered, A | $20,000 | — | — |
| Payment rails ($35,000) | Metered, A | $35,000 | — | ACH $0.30, check $6.00, wire $7.00 |
| AI inference ($6,000) | Metered, A | $6,000 | — | — |
| Hosting ($40,000) | Driver, C | $40,000 | — | $0.10 per event |
| Bank charges ($15,000) | Driver, C | $15,000 | — | $0.30 per transaction |
| Custody platform ($30,000) | Driver, C | $30,000 | — | $1.20 per account-month |
| Overhead ($64,000) | Policy, D | Appears only in CM3 | — | — |
| Unmapped GL account | U | — | — | **Residual $8,000 = 1.6% of GL. This exceeds S1's configured 1% tolerance, so the result is ExceedsTolerance and the pack stays Draft.** |

**Results:**
- Contribution margins: CM1 = $227,359.37; CM2 = $142,359.37; CM3 = $20,000.00.
- Operating result after residual: $12,000 ($520k − $508k).
- Unit costs:
  - routine ACH grant: $1.30 (grade C, $0.98–$1.62);
  - domestic exception grant paid by check: $42.79 (grade B, mixed, $37.07–$48.51).

**What the example shows:**
- Unused capacity ($58,359.37) is **about 4.9 times** the $12,000 operating result. The research pass said "about three times"; the adversarial review corrected it (XIF-ADV-008).
- *Erratum (XIF-ADV-008):* the research pass quoted "$12.70 per grant" for S1. That figure cannot be derived from the example's inputs: back-solving implies about 19 grants per account per year, outside the model's 6.2–13 range. It is withdrawn. The point it illustrated still stands: an S1 unit cost is not a benchmark. The Form 990 Part IX figures for large national sponsors ($55–$68 per grant; Fidelity $67.70, DAFgiving360 $54.59, Vanguard $134.50 and NPT $419.24) cover a far wider scope (daf-landscape sponsor-cost-structure-and-sensitivities.md:127–137).
- Cost per account rises with fund size (elasticity 1.22, ibid. :103), so cost per account must never be shown on its own (UNB-XIN-ECO-007).

**Recomputation (adversarial review).** r = $1.953125/min, CM1, CM2 and CM3 were all reproduced exactly. Three problems remain:
- The unit-cost ranges ($0.98–$1.62; $37.07–$48.51) cannot be reproduced without the component breakdown. They are retained only as illustrations of how graded ranges are displayed.
- The tolerance basis in UNB-XIN-ECO-004 (residual as a % of GL, or of the pool) is ambiguous.
- As written, the reconciliation identity is true by construction, because the residual is defined as the difference.

The identity therefore has to be tested on its components: the residual must carry reason codes, and unmapped GL accounts must be enumerated. Owner decision: XID-ADV-011.

## 4. Operational efficiency dashboard requirements (UNB-XIN-ECO-022)

**Screens and roles**

| Screen | Role | Purpose |
|---|---|---|
| Economics Overview | Executives | Margins, unit costs and counter-metrics, each with a grade |
| Unit Cost Explorer | FP&A, operations | Drill from KPI → AttributedCost → ActivityEvent → domain event or vendor line |
| Attribution & Reconciliation | Controller | Per-pool reconciliation identity, residual by reason, sign-off |
| Capacity & Staffing Forecast | COO, supervisors | 13-week and December forecast, backlog simulation, forecast accuracy (MAPE) |
| AI Spend | Agent owner | Cost per run, budget exhaustion, alerts at 3× median |
| Process Efficiency | Operational excellence | Rework loops, wait time, touch counts; control steps marked as such |
| Improvement Loop | Owner, verifier, compliance | Initiative states, verification, ratchet history, floor breaches |
| Profitability | Finance only | CM1–CM3 by sponsor, service model and balance band |
| Scenario | CFO | Echelon-backed sensitivity runs; no apply button |
| Thresholds | Sponsor admin | Alert-only thresholds |

**Metrics.** Each metric is defined with a numerator, denominator, eligible population, time basis, grade rule and counter-metric (UNB-XIN-ECO-007):
- cost per settled grant, by rail;
- cost per account-month, by balance band;
- cost per settled contribution;
- cost per charity verification, new vs reused;
- cost per resolved case;
- cost per contact;
- cost per exception, by type;
- cost per approval;
- cost per AI run;
- cost per automated completion;
- custody cost per $1m AUM;
- failure cost per 1,000 grants;
- residual %;
- unused capacity %;
- cost-recovery ratio of service charges.

**Components**
- Forma provides: KPI tiles, trends, grids, filters, heatmap, workflow diagram, and freshness and Unknown states.
- Unbound adds domain widgets built on Forma patterns: grade-aware margin waterfall, reconciliation identity, ratchet history and scenario sensitivity.
- Folio produces the monthly Operating Economics pack and the quarterly Improvement report, after renderer validation under REQ-X-050.

**Rules on every screen**
- Each figure shows its grade (as text, not colour alone), its range, its as-of time, its rule version and its reconciliation status.
- Cost and counter-metric are always rendered as a pair. Mobile layouts cannot hide counter-metrics.
- Exports carry the definition version, grade, range and "Management estimate, not a financial statement".
- WCAG 2.1 AA, with a table alternative for every chart.
- Scoping (UNB-XIN-ECO-021, P0):
  - Sponsors see only their own costs plus their share of shared platform costs, with the method disclosed.
  - Benchmarks need at least 5 sponsors, with no sponsor contributing more than 60%.
  - There is no per-worker cost view. Staff data appears only in groups of 5 or more.

## 5. Critical question

> *Can we design the system so that reducing operating costs becomes a measurable, continuous process rather than an occasional management initiative?*

**Answer: yes, for attributable variable cost (grades A and B). No, for structural cost.**

- **Structural costs move only through annual decisions.** These include:
  - the staffing floor imposed by separation of duties and legally competent review (6–8 vs about 20 people, unresolved in TVC-018);
  - overhead;
  - the December specialist pool.
- **The loop needs real measurement first.** Today every economics parameter is an assumption (economic-model.md). Until the measurement requirements below are met, the loop can be specified but not operated:
  - UNB-XIN-ECO-003: grades on every figure;
  - UNB-XIN-ECO-004: monthly reconciliation to the GL;
  - UNB-XIN-ECO-009: per-call AI metering;
  - UNB-XIN-SAS-009: per-tenant usage metering.

### The closed loop (UNB-XIN-ECO-015, -016)

| Step | Owner | Cadence | Rule |
|---|---|---|---|
| 1 Measure | Platform data owner, operations | Continuous, plus weekly sampling | Tag coverage ≥ 95% |
| 2 Attribute | FP&A; controller signs off | Monthly, after close | Reconciliation identity holds; residual within tolerance |
| 3 Detect | Operational-excellence analyst | Weekly | Process mining, thresholds, anomalies |
| 4 Hypothesize | Operations owner; AI advisory | As detected | Proposal sets baseline, target, counter-metrics and stop conditions |
| 5 Experiment | Owner, under REQ-X-042 tiers | Per initiative | Floors set by Compliance |
| 6 Verify | FP&A, independent of the owner | Matched periods | Saving credited only if the lower bound is above 0 and the floors held |
| 7 Ratchet | CFO | Quarterly | Target tightens only after 2 periods within the floors |
| 8 Reset | Leadership | Annual | Review the floors and practical capacity |

### Guardrails (counter-metric floors, UNB-XIN-ECO-016)

A breach of any floor halts the initiative, blocks the ratchet and opens a rollback review. The floors are:
- seeded-control pass rate stays at 100% for P0 controls, and detection coverage does not fall;
- zero known false negatives in sanctions or eligibility screening;
- §4966 referral rate stays within ±20% unless explained;
- p90 approval-to-settlement stays within SLA;
- correction rate stays within its band;
- no rise in abandonment or repeat contacts;
- no new accessibility failures;
- backlog per eligible reviewer stays at or below the configured level.

Fewer detected exceptions are credited only if detection coverage was stable over the same period.

### Acceptance criteria: the loop is operating when

1. At least 80% of variable operating expense is graded A or B within 6 months.
2. The residual is within tolerance for 3 consecutive months.
3. Every initiative has a baseline, a target, a verifier who is not the owner, counter-metrics in every guardrail category, and stop conditions.
4. At least one verified → ratcheted cycle completes each year.
5. No ratchet happens in a period with a floor breach.
6. An independent re-run reproduces the totals to $0.01.
7. December forecast accuracy (MAPE) is reported.
8. No configuration change made by AI exists without a human approval record.

**Limits.** Returns diminish as costs approach the control floor. The largest levers are product redesigns, not cost cutting:
- halving the exception share;
- moving December demand earlier through giving plans (economic-model.md:84);
- defaulting grantees to ACH, which removes check fraud, escheat and reissue cost ([industry/bnk.md](industry/bnk.md) §5);
- reusable equivalency determinations and intermediaries for foreign grants ([industry/grt.md](industry/grt.md) §8).

## 6. UI and reporting implications

- **Truth and posting.** There is no new reporting truth store. Dashboards never post.
- **Official status.** A pack is official only after reconciliation. Restatements are visibly marked.
- **Donors** see a quote only for operations that carry a charge, plus their all-in fund-borne cost where the sponsor enables it.
- **Sponsors** get showback with the method disclosed. Chargeback happens only as approved invoices.
- **New Folio reports:**
  - monthly economics pack;
  - quarterly improvement report;
  - annual floor review;
  - an October December-readiness report;
  - service-charge recovery report (UNB-XIN-ECO-018; counsel flag above a 1.25 recovery ratio, XID-ECO-002).
- **APIs:** query APIs, plus role-gated commands for rules, sign-off, thresholds and initiatives. There are no ledger commands.

## 7. Cost drivers gathered from the other research passes (inputs, not results)

| Domain | Sourced drivers | Source |
|---|---|---|
| Grants | Operating cost per grant: $55–$574 across nationals; anchored labour $7.9 per grant at large nationals vs $180–323 at community foundations | industry/grt.md §8 |
| Payments | Card cost 1.58% (PayPal Charitable Giving Fund CY2024); Blackbaud Giving Fund card fees 0.30% plus 0.18% bank/merchant; lockbox $0.65 per check; manual check fee the higher of $35 or 8% (Benevity) | industry/bnk.md §5 |
| Investments | Pool expense 26.3 bps (Fidelity FY2025); advisor fees about 26–27 bps realised, 0–2% observed; DAF operating cost 67–89 bps market-wide | industry/inv.md §7 |
| Service | 0.3 human contacts per account per year (an assumption); AI $0.05 per service interaction and $0.90 per exception dossier (assumption) | industry/svc.md §7 |
| Fees and revenue | Fidelity 0.60% on the first $500k with a $100 minimum; RCF paid Ren $744 per year-end DAF (explicitly *not* ARPU) | industry/erp.md §6 |
| Tenancy | Usage units for storage, API, compute, AI tokens, provider units and work-item minutes | industry/sas.md §8 |

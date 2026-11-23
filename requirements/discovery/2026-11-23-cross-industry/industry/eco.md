# Enterprise operating economics and cost-to-serve — addendum §18 (code ECO)

Status: proposed and unvalidated. Date: 2026-11-23.

Records produced:

| Record type | Count |
|---|---|
| Atlas rows | 36 |
| Findings | 20 |
| New requirements | 22 (UNB-XIN-ECO-001..022) |
| Refinements | 4 (RR-UNB-ACC-014-r1, RR-AIN-028-r1, RR-UNB-ENT-003-r1, RR-UNB-ECO-001-r1) |
| Scenarios | 28 |
| Rejections | 11 |
| Failure modes | 16 |
| Decisions | 9 |
| Conflicts | 3 |

No measured operating data exists. Every unit time and every figure in the worked example is **Illustrative**.

## 0. Existing coverage reused, not duplicated

**Requirements and methods already in place**
- REQ-X-043 names economics KPIs but has no attribution model.
- research/…/15-economics/economic-model.md is a planning model only, with every input assumed:
  - FTE formula (:23);
  - surge backlog (:62, :74);
  - AI cost materiality: about $4 per dossier and $0.31 per routine run (:96, :98);
  - incident cost (:117).
- UNB-ECO-001..007 and UNB-ENT-003 are planning-level only.
- UNB-ACC-014 places allocation inside accounting, with no boundary stopping allocations from posting.
- REQ-X-042 defines an improvement loop, but without cost targets, a ratchet or an independent verifier.
- REQ-X-048/049 cover queues and capacity, but not forecasting.
- REQ-X-067/071 name profitability and cost-to-serve without defining them.
- AIN-028, CTL-027 and TVR-ECON-002/003 cover AI budgets.
- Echelon's economics and continuous-improvement methods apply at design time only.

**daf-landscape evidence** (mostly from sponsor-cost-structure-and-sensitivities.md)
- FY2024 DAF-allocated operating cost: 67 bps of assets, or 89 bps on a grants basis (:10).
- Operating cost per grant (:127–137):

  | Sponsor | Cost per grant |
  |---|---|
  | Fidelity | $67.70 |
  | DAFgiving360 | $54.59 |
  | Vanguard | $134.50 |
  | NPT | $419.24 |

- Cost per account rises with fund size, with elasticity 1.22 (:103).
- Ren/RCF audited payments equal 95–98% of RCF admin-fee revenue, about $683–$745 per year-end fund. The source marks this "NOT ARPU".
- Fee minimums: Fidelity $100; Ren/RCF $250.

## 1. Industry assessments

| Industry | What transfers | What does not, and why |
|---|---|---|
| Bank cost-to-serve | Activity costing per account and product; multiple denominators; showing losses by segment | Absorbing all overhead into each fund; tiering service by profitability. A DAF fund is sponsor property held for charity, not a customer deposit. |
| **Funds transfer pricing** | **Nothing** | **Not applicable.** There is no deposit or loan book, no maturity transformation and no interest margin. FI-36 already assigns float. A "float credit" would reward paying grants slowly (XRJ-ECO-001). |
| Cloud FinOps | A subset of the FOCUS schema (v1.4: BilledCost, EffectiveCost, ListCost, ChargeCategory, Tags, AllocatedMethodId; focus.finops.org, accessed 2026-11-23); showback required, chargeback optional; tagging at creation; AI unit economics | Real-time allocation of every cost; separate tooling (contradicts the monolith preference). Labour and AUM-based custody need extension columns. |
| BPO / time-driven activity-based costing (Kaplan & Anderson, HBR 2004; abstract only read) | Capacity cost rate × time equations; reporting unused capacity; outcome-unit pricing for outsourced administration | BPO firms carry no legal duty for outcomes. Separation of duties and legally competent review cap the savings. Pricing per approved grant creates an incentive toward unsafe approval. |
| Insurance | The split between allocated (case-specific) and pooled cost; versioned classification rules; a cost category for failure and recovery | Reserving; combined ratio |
| Contact centre | Cost per resolved case; repeat-contact denominators; staffing to service level; proactive status updates | Erlang-C applied to multi-day review backlogs; generic seasonal staff doing legal review |
| 529 recordkeeping (unverified) | Fixed per-account cost shown separately | Minimum fees reduce charitable assets, which raises a reasonableness question (XID-ECO-002) |

## 2. Enterprise operating economics requirements

Each requirement has 2–4 Given/When/Then criteria in `gap-requirements.jsonl`.

| ID | Requirement | Priority |
|---|---|---|
| UNB-XIN-ECO-001 | Cost attribution is a projection that never posts. Book effects happen only through UNB-ACC-010 billing or UNB-ACC-019 functional allocation, with approval. | P1 |
| UNB-XIN-ECO-002 | Typed canonical entities; versioned, effective-dated attribution rules | P1 |
| UNB-XIN-ECO-003 | Every figure carries a method, a confidence grade and a range: A metered (±2%); B time-driven, with ≥30 samples in 90 days (±15%); C driver-based (±35%); D policy (±60%); U unattributable | P1 |
| UNB-XIN-ECO-004 | Monthly reconciliation to the general ledger, with a residual by reason, a tolerance gate (default 2% or $5k) and controller sign-off | P1 |
| UNB-XIN-ECO-005 | Vendor cost records aligned to FOCUS, with cost bearer and correlation ID | P2 |
| UNB-XIN-ECO-006 | Time-driven labour costing; unused capacity is never loaded onto cost objects; waiting time is not touch time | P2 |
| UNB-XIN-ECO-007 | Metric catalog; cost per account is never shown alone | P2 |
| UNB-XIN-ECO-008 | Advisory cost quote before an expensive operation. The quote never skips a control or switches a rail; an overrun needs re-affirmation. | P2 |
| UNB-XIN-ECO-009 | Per-call AI metering; budget exhaustion fails safe; alert at 3× the median | P1 |
| UNB-XIN-ECO-010 | Weekly process mining; control steps are never classed as waste | P2 |
| UNB-XIN-ECO-011 | AI improvement recommendations are advisory only; AI has no write access to configuration | P2 |
| UNB-XIN-ECO-012 | Revenue records come only from posted revenue; contribution margins CM1–CM3; profitability isolated from operational decisions | P3 |
| UNB-XIN-ECO-013 | Scenario runs use Echelon verdicts; there is no apply button | P3 |
| UNB-XIN-ECO-014 | Cost and margin thresholds alert only | P2 |
| UNB-XIN-ECO-015 | Improvement-initiative loop with an independent verifier and a target ratchet | P2 |
| UNB-XIN-ECO-016 | Counter-metric floors. A breach halts the change and blocks the ratchet. | P1 |
| UNB-XIN-ECO-017 | 13-week capacity forecast and December forecast, with backlog simulation and accuracy tracking | P1 |
| UNB-XIN-ECO-018 | Cost-recovery ratio for service charges; counsel flag above 1.25 | P2 |
| UNB-XIN-ECO-019 | Realised automation ROI, including rework, oversight, failure and build cost | P2 |
| UNB-XIN-ECO-020 | SLA-tier cost at the December peak; lifetime value at contract level (Scenario) | P3 |
| UNB-XIN-ECO-021 | Scoped cost views. Benchmarks need ≥5 sponsors, with no single sponsor above 60%. No per-worker cost. | P0 |
| UNB-XIN-ECO-022 | Forma dashboards and a monthly Folio economics pack | P2 |

Example acceptance criteria:
- **UNB-XIN-ECO-001.** Given a closed month, when attribution runs, then the general ledger, fund balances and available-to-grant are byte-identical before and after.
- **UNB-XIN-ECO-016.** Given the seeded-control pass rate falls to 96%, when an initiative is running, then the initiative halts and the ratchet is blocked.

**The seven architectural options**

| # | Option | Decision |
|---|---|---|
| 1 | Real-time attribution | **Partial.** Near-real-time only for metered AI, rail and verification costs; everything else monthly. |
| 2 | Cost forecasting before expensive operations | **Yes, advisory**, above configured thresholds. |
| 3 | Automated identification of inefficient workflows | **Yes**, by deterministic process mining. |
| 4 | AI improvement recommendations | **Advisory only**, through the REQ-X-042 tiers. |
| 5 | Profitability by business model | **Yes**, at sponsor, service-model and balance-band level; finance-only access. |
| 6 | Simulation of pricing and service strategies | **Echelon offline first.** Build it into the platform only if it is used monthly. |
| 7 | Cost and margin thresholds | **Yes, alert-only.** |

## 3. Cost attribution and profitability model

**Entities**
- **CostPool**
  - Type: Labour, Vendor, Infrastructure, AIInference, PaymentRail, CustodyBanking, Verification, FailureAndRecovery or Overhead.
  - Also holds the GL account set, the cost bearer (Fund, Sponsor, Platform or Partner) and practical capacity for labour pools.
- **CostDriver** and **Activity.** Each activity carries its PRINCIPLES class and a versioned time equation.
- **ActivityEvent**, built from existing outbox events.
- **CostObject**
  - Kinds: Tenant, Sponsor, Account, Fund, Grant, Contribution, Case, CharityVerification, AIRun, Contact.
  - Roll-ups must not double-count.
- **AttributionRule**
  - Versioned and effective-dated.
  - Method: DirectMetered, TimeDriven, DriverBased or ResidualPolicy.
- **AttributedCost**
  - One pool, one object and one rule version per row.
  - Each row carries a grade and a range.
- **UnallocatedResidual**, with a reason code.
- **UnusedCapacity**.
- **ReconciliationRun**.
- **RevenueRecord**, built from posted revenue only.
- **VendorCostRecord**.

**Formulas**
- **Capacity cost rate.** r = pool cost ÷ (paid hours × 0.80 × 60).
  - Labour cost = Σ (equation minutes × r).
  - Unused capacity = (capacity − used) × r.
- **Driver-based cost** for object o = pool × d(o) ÷ Σd.
- **Per-pool identity:** Σ attributed + unused + residual = GL actual, to $0.01.
- **Ranges** sum linearly.
- **Grades.** A derived figure takes the worst grade among its inputs. Exception: if ≥80% of its value comes from better-graded inputs, it takes the dominant grade plus a "mixed" flag.
- **Contribution margins:**
  - CM1 = revenue − A/B-grade costs.
  - CM2 = CM1 − C-grade costs.
  - CM3 = CM2 − unused capacity − overhead.
  - CM3 − residual = revenue − GL operating expense.
- **Ratchet.** Next target = max(floor, min(target, verified × 1.05)), applied only after 2 periods within the floors.
- **Backlog.** B(d+1) = max(0, B(d) + arrivals − capacity).

**Worked example (Illustrative; synthetic sponsor S1; one month)**

Inputs:
- GL operating expense: $508,000.
- Mapped pools: $500,000.
- One unmapped account: $8,000.
- Fee revenue: $520,000.

Labour pools (time-driven):

| Pool | Pool cost | Attributed | Unused | Notes |
|---|---|---|---|---|
| Grant operations | $180,000 | $135,546.88 to exception grants (69,400 minutes) | $44,453.12 | 92,160 practical minutes, so $1.953125 per minute |
| Contact centre | $60,000 | $46,875 | $13,125 | $15.63 per contact; $23.44 per resolved inquiry |
| Compliance | $50,000 | $49,218.75 | $781.25 | |

Other pools:

| Pool | Amount | Grade | Basis |
|---|---|---|---|
| Verification | $20,000 | A (metered) | |
| Payment rails | $35,000 | A (metered) | ACH $0.30, check $6.00, wire $7.00 |
| AI inference | $6,000 | A (metered) | |
| Hosting | $40,000 | C (driver-based) | $0.10 per event |
| Bank charges | $15,000 | C (driver-based) | $0.30 per transaction |
| Custody platform | $30,000 | C (driver-based) | $1.20 per account-month |
| Overhead | $64,000 | D | Appears only in CM3 |

Reconciliation:
- The $8,000 residual is 1.6% of GL operating expense, above S1's 1% tolerance.
- The run is therefore marked ExceedsTolerance, and the pack stays Draft until the controller signs off.

Unit costs:

| Unit | Cost | Grade and range |
|---|---|---|
| Routine ACH grant | $1.30 | C, $0.98–$1.62 |
| Domestic exception grant paid by check | $42.79 | B (mixed), $37.07–$48.51 |

Margins:

| Measure | Amount |
|---|---|
| CM1 | $227,359.37 |
| CM2 | $142,359.37 |
| CM3 | $20,000.00 |
| Operating result after residual | $12,000 (= $520,000 − $508,000) |

Two observations:
- Unused capacity ($58,359.37) is about three times the operating result.
- S1's $12.70 per grant is not a benchmark. The $55–68 Part IX figures for large national sponsors cover a much wider scope.

## 4. Operational efficiency dashboard requirements

**Screens and roles**

| Screen | Role |
|---|---|
| Economics Overview | Executives |
| Unit Cost Explorer | FP&A, operations |
| Attribution & Reconciliation | Controller |
| Capacity & Staffing Forecast | COO, supervisors |
| AI Spend | Agent owner |
| Process Efficiency | Operational excellence |
| Improvement Loop | Initiative owner, verifier, compliance |
| Profitability | Finance only |
| Scenario | CFO |
| Thresholds | Sponsor admin |

**Components**
- **Forma** provides: KPI tiles, trends, grids, filters, heatmaps, workflow diagrams, and freshness and Unknown states.
- **Unbound domain widgets:**
  - grade-aware margin waterfall;
  - reconciliation identity;
  - ratchet history;
  - scenario sensitivity.
- **Folio** produces the monthly Operating Economics pack and the quarterly Improvement report, validated under REQ-X-050.

**Metrics.** Each is defined with a numerator, a denominator and a counter-metric:
- Per grant and per contribution:
  - cost per settled grant, by rail;
  - cost per settled contribution.
- Per account and asset:
  - cost per account-month, by balance band;
  - custody cost per $1m AUM.
- Per unit of work:
  - cost per charity verification (new vs reused);
  - cost per resolved case;
  - cost per contact;
  - cost per exception, by type;
  - cost per approval.
- Automation:
  - cost per AI run;
  - cost per automated completion.
- Quality and capacity:
  - failure cost per 1,000 grants;
  - residual %;
  - unused capacity %;
  - recovery ratio.

**Rules on every screen**
- Drill-down path: KPI → AttributedCost → ActivityEvent → domain event or vendor line, within the viewer's scope.
- Every figure shows four things:
  - its grade, as text (not by colour alone);
  - its range;
  - its as-of time;
  - its rule version and reconciliation status.
- Exports carry the definition version, grade and range, plus the disclaimer "Management estimate, not a financial statement".
- Accessibility: WCAG 2.1 AA, with table alternatives for every chart.
- Mobile layouts may not hide counter-metrics.

## 5. Critical question answer

**Can reducing operating cost become a measurable, continuous process?**
- **Yes, for attributable variable cost** (grades A and B).
- **No, for structural cost.** These move only through annual structural decisions:
  - the minimum staffing floor imposed by separation of duties (6–8 vs about 20 people; unresolved, TVC-018);
  - overhead;
  - the December specialist pool.
- **A precondition is real measurement.** Today every economics parameter is assumed (EXT-04, TR-5).

**The loop** (owner and cadence):

| Step | Owner | Cadence and rule |
|---|---|---|
| 1. Measure | Platform data owner, operations | Continuous, plus weekly sampling; tag coverage ≥95% |
| 2. Attribute | FP&A; controller signs off | Monthly, after close |
| 3. Detect | Operational-excellence analyst | Weekly: process mining, thresholds, anomalies |
| 4. Hypothesize | Operations owner, with advisory AI | As findings arise |
| 5. Experiment | Operations owner | Under the REQ-X-042 tiers, within floors set by Compliance |
| 6. Verify | FP&A, independent of the owner | Matched periods; credit only if the lower bound of the saving is > 0 and floors held |
| 7. Ratchet | CFO approves | Quarterly |
| 8. Reset | Leadership | Annual review of floors and practical capacity |

**Guardrails.** A floor breach halts the change, blocks the ratchet and opens a rollback review.
- Seeded-control pass rate stays at 100% for P0 controls, and detection coverage does not fall.
- Zero known sanctions or eligibility false negatives.
- The §4966 referral rate moves no more than ±20% unless explained.
- p90 approval-to-settlement stays within SLA.
- The correction rate stays within its band.
- No rise in abandonment or repeat contacts.
- No new accessibility failures.
- Backlog per eligible reviewer stays at or below the configured level.

A drop in detected exceptions counts as a saving only if detection coverage stayed stable.

**Acceptance criteria: the loop is operating when**
- ≥80% of variable operating expense is grade A or B within 6 months.
- The residual is within tolerance for 3 consecutive months.
- Every initiative has:
  - a baseline and a target;
  - a verifier who is not the owner;
  - a counter-metric in every guardrail category;
  - stop conditions.
- At least one verified-then-ratcheted cycle completes each year.
- No ratchet happens in a period with a floor breach.
- An independent re-run reproduces totals to $0.01.
- December forecast accuracy (MAPE) is reported.
- No AI configuration change exists without a human approval record.

**Limits.** Returns diminish as cost approaches the control floor. The largest levers are product redesigns, not cost-cutting: halving the exception share, and moving giving earlier in the year through giving plans (economic-model:84).

## 6. UI/reporting implications

**Reporting truth and integrity**
- No new store of reporting truth, and dashboards never post.
- A cost figure without a grade is a validation error.
- A pack becomes official only after reconciliation, and restatements are marked visibly.
- A cost metric is always shown with its counter-metric.

**Audiences**
- **Donors** see quotes for charged operations. Where the sponsor enables it, they also see the all-in cost borne by their fund.
- **Sponsors** get showback with the method disclosed. Chargeback happens only through approved invoices.
- **Staff data** is shown only in groups of ≥5, with no ranking of individuals.

**New Folio reports**
- Monthly economics pack.
- Quarterly improvement report.
- Annual floor review.
- December-readiness report, issued in October.
- Service-charge recovery report.

**APIs**
- Query APIs.
- Role-gated commands for rules, sign-off, thresholds and initiatives.
- No ledger commands.

## 7. Redesigns, automation, business models, evidence gaps

**Redesigns.** Each compares conventional, proposed and redesigned workflows, covering controls, exceptions, evidence, recovery, cost, latency and customer experience:
- cost attribution;
- December staffing;
- the cost-reduction programme;
- AI spend;
- international grant pricing.

**Automation boundaries**
- **Deterministic:** arithmetic, reconciliation, process mining, forecasting, budgets, access scoping.
- **AI, advisory only:** explaining process variants, drafting hypotheses, suggesting GL-to-pool mappings.
- **Human:** rules, sign-offs, counsel and CPA review, staffing, targets, and any change to a control.

**What release 1 deliberately omits**
- Real-time allocation of pooled costs.
- Donor-level profitability.
- An in-platform simulator (Echelon runs offline first).

Release 1 is six metrics plus the monthly pack.

**Business-model applicability**
- All 10 models: attribution, AI metering, floors and forecasting.
- High-volume-low-balance: forecasting and minimum-fee analysis are critical.
- Outsourced administration: the main user of chargeback.
- D2C, advisor-distribution and HNW: international cost quotes.

**Evidence not obtained**
- Measured exception rates, handling minutes and December volumes.
- Vendor rate cards.
- Full texts of Kaplan & Anderson, FOCUS 1.4 and SSAP 55.
- Contact-centre and 529 recordkeeping benchmarks.
- A reconciled loaded labour rate.
- The scope gaps in Form 990 Part IX: costs borne by affiliates or netted inside funds are invisible.

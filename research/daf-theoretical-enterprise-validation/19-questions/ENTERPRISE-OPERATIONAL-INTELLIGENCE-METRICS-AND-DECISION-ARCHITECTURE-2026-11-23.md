# Enterprise Operational Intelligence: Metrics and Decision Architecture
Date: 2026-11-23. Requirements only; no implementation, data validation or executed tests.

## Decision
Build a unified decision-oriented operational intelligence layer within the Unbound component monolith. Do not create a separate reporting truth store that can authorize money movement or override accounting. Dashboards and reports consume governed versioned read models, with authoritative financial facts from the accounting core and explicit freshness, reconciliation, scope and provenance. **This is the metric/decision foundation, not the final dashboard and report UX design.** A dedicated subsequent phase will define dashboards, interactive widgets, layouts and reports.

## Six enterprise questions and decision domains
1. Financial integrity: Are accounting books, NAV, cash, available-to-grant and statements correct, reconciled and current?
2. Grant execution: Are recommendations, approvals, reservations and payments progressing safely, with accurate provider states?
3. Donor experience: Where do donors abandon, repeat steps, encounter unclear policy or require assistance?
4. Compliance and charity readiness: Are charities verified, policies enforced, sanctions screened, and holds correctly applied without needless delays?
5. Unit economics and operating capacity: What does each transaction, fund, sponsor and exception actually cost, including AI, vendors, custody, mailing, labor and rework?
6. Prevention and learning: Are recurring failures and harm declining while detection coverage and safety remain stable?

## KPI dictionary: definitions, not target promises
Each metric must specify business question, decision owner, canonical numerator/denominator, source component, inclusion/exclusion rules, sponsor/fund scope, time basis (event/effective/known-at), refresh SLA, reconciliation status, drilldown path, confidence/completeness, permitted audience and version.
- Financial: total AUM and valuation basis, fund balances, available-to-grant, reconciliation breaks by materiality, stale valuation exposure, unresolved suspense, month-end close latency, statement issuance completeness, correction rate.
- Grants: recommendation-to-approval and approval-to-settlement distributions (median/p90), policy-safe deterministic approval rate, held amount and duration, grant decline reasons, duplicate prevention, provider Unknown aging, hand-delivered/mailed check processing.
- Contributions: initiated/completed/settled contributions, recurring contribution success by pay-period cadence, abandoned intake, reversals and processing cost.
- Charity: verified population, 180-day re-verification coverage, expired/review-required charities, shared-evidence reuse, global hold propagation, false positives and adverse outcome detection.
- Donor UX: task completion by flow, abandonment by step, accessibility-related failures, support touches, time to usable grant status, repeat-entry rate, channel transitions (portal/widget/IVR).
- Operations: cases by severity and source, safe auto-resolution rate, human touch minutes, case reopening, time waiting on external providers, false hold/unsafe approval incidence, near misses, root-cause prevention impact.
- Communications/documents: monthly statement completeness, archive verification success, corrections, notice accepted/delivered/bounced where evidenced, seven-year financial preservation compliance, authorized access failures.
- Economics: revenue/fees by approved accounting basis, variable cost per settled grant/contribution, sponsor servicing cost, AI inference and verification cost, paper/check handling cost, cost of exception, unit margin with explicit cost allocation assumptions.
- Governance: override requests, independent approvals, audit evidence gaps, policy changes, retention/hold exceptions, security incidents and access violations.

## Avoid misleading aggregate metrics
Separate recommended, approved, reserved, submitted, provider accepted, settled/paid, returned and canceled; never call Accepted Paid. Distinguish AUM from available-to-grant and cash, fund balance from sponsor revenue, 'AI assisted' from independently verified resolution, detected cases from actual failure incidence, and email accepted from delivered/read. Count unique cases and affected transactions separately. Show denominator, period and eligible population. Never infer zero from missing source data. Explicitly distinguish illustrative targets from validated benchmarks.

## Read-model architecture and provenance
Authoritative accounting core owns GL, subledger, fund balances, NAV and financial close. Grants/Payments own workflow and execution states; Charity/Compliance own verified status and holds; Documents/Communications own issuance and delivery; Operational Learning owns proposals/experiments. Publish typed in-process domain facts via transactional outbox into governed read models, with schema versions, event IDs, effective and known-at timestamps, sponsor IDs, deduplication, source watermarks and replay. Dashboards cannot issue domain commands by mutating read models. Cross-domain joins must preserve tenant isolation and policy. Aggregate cross-sponsor benchmarking only under privacy-preserving rules with suppression for small groups.

## Freshness, correctness and reconciliation
Each tile/chart/export must disclose as-of timestamp, source coverage, completeness, known delays and reconciliation/validation state. For finance, use closed/approved accounting snapshots for official reporting, while explicitly marking live estimates and unsettled activity. Support bitemporal 'as originally reported' versus 'currently corrected' views. Period-over-period comparisons require consistent metric definitions; restated history must be visibly identified. Never silently recalculate an issued board report with changed formulas.

## Decision-to-action contract
Every dashboard section must define the intended decision and authorized next action: e.g., reconciliation break -> open verified exception; aged provider Unknown -> investigate status, not retry payment; charity reverification due -> deterministic evidence refresh; statement batch blocked -> source-specific correction; donor abandonment -> approved UX experiment. Actions use authoritative domain commands with existing authorization and protective gates. No dashboard button can bypass sponsor policy or finance segregation.

## Audience and future UX design inputs
Platform executives: enterprise trends and risk-adjusted economics; sponsor executives: sponsor-scoped AUM, grants, costs, compliance; controller/CFO: ledger, close, reconciliation, valuation and finance signoff; compliance: charity verification, sanctions and holds; operations: queues, throughput and external dependencies; donor experience: task friction and accessibility; board/auditors: frozen, approved period reporting with evidence. Future dashboards must support sponsor branding, embedded widgets, mobile/accessibility, drilldowns, customizable layouts with permission limits, and explicit loading/unknown/error states. These are inputs, not completed UI specifications.

## Reports foundation (future phase)
Define canonical report catalog, as-of/frozen snapshots, reproducible metric definitions, approval and distribution policies, export formats, schedules, sponsor white labeling, legal disclosures, immutable issued artifacts and seven-year financial retention. Formal financial/board reports must be versioned and distinguish operational estimates from approved statements. Report authoring UX, role-based layouts, widgets and scheduling remain dedicated next-phase work.

## Measurement governance
Maintain a metric registry and versioned semantic layer with owner, source contract, formula, tests, authorization, freshness and known limitations. Material changes require review; preserve historical definitions for comparison. Add independent tie-out to approved financial statements. Monitor instrumentation coverage and sampling biases. Define minimum useful set first rather than optimizing for hundreds of decorative KPIs.

## Proposed adversarial scenarios (not executed)
- ADV-543: AUM shown as spendable cash; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-544: provider Accepted counted as Paid; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-545: missing custodian feed interpreted as zero; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-546: unreconciled live balance presented as approved; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-547: dashboard crosses sponsor privacy boundary; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-548: historical metric formula silently changes; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-549: denominator excludes failed transactions; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-550: false holds hidden from compliance metric; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-551: AI assisted labeled fully autonomous; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-552: exception count falls because detection stops; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-553: one grant counted twice after correction; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-554: recurring $10 payroll donations counted at schedule rather than settlement; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-555: statement issued mistaken for delivered; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-556: stale charity verification shown valid; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-557: external provider outage masks payment Unknown; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-558: reconciliation mismatch omitted from executive summary; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-559: board report regenerates with new numbers; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-560: fund closure removes history from reporting; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-561: sponsor benchmark reidentifies small donor cohort; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-562: dashboard action bypasses authoritative grant hold; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-563: metric source watermark is stale but hidden; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-564: materiality threshold suppresses incident; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-565: unit cost excludes paper/check overhead; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-566: labor savings omit rework costs; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-567: FX conversion mixes currencies without rate basis; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-568: retention deletion removes audit evidence; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-569: correction history double counts AUM; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-570: donor UX funnel ignores inaccessible flow; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-571: date filter mixes effective and recorded times; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-572: export contains privileged financial details; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-573: report says verified based solely on PDF hash; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-574: operational estimate mislabeled official financial statement; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-575: widget shows confidential cross-sponsor totals; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-576: read model lag creates duplicate financial action; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-577: monthly close SLA incentivizes unsafe approvals; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.
- ADV-578: AI-generated narrative claims unsupported causality; expected: governed metric definition, explicit uncertainty, tenant isolation, authoritative decision gates and traceable evidence.

## Open decisions and challenge
Determine initial executive KPI set, metric refresh and cost budgets, finance-certified versus operational metric boundary, data minimization, benchmark privacy thresholds, reporting approvals, sponsor customizations, accessibility tests and which indicators drive actionable decisions. Challenge: a single composite 'health score' can hide catastrophic financial errors; prefer independent safety gates and visible dimensions. Avoid optimizing processing speed at the expense of compliance or donor trust.

## Explicit follow-on work
After this metrics/decision architecture, return to (A) role-based dashboards and information architecture, (B) interactive widgets and configurable embedded layouts, (C) financial/operational/compliance/board report catalog, (D) report designer and scheduled delivery, (E) drilldown and data provenance UX, (F) dashboard adversarial tests. Do not treat those as completed here.

Status: proposed. ADV-543..ADV-578 not executed.

# 12 — Evidence-phase protocol: shadow replay, time study and benchmark

**Purpose.** Report 10 recommends that the first step after the requirements gate is evidence, not production code:
CNV-ECO-003 forbids prices, savings claims and segment commitments until a shadow replay and a time study exist, and
CNV-ARC-001 requires a benchmark against declared objectives. This report specifies those three studies so a design-partner
sponsor can be approached with a concrete, bounded request. It is a protocol, not results. Nothing here has been run.

## 1. Design-partner sponsor

| Criterion | Requirement | Why |
|---|---|---|
| Size | 5,000–50,000 funds (CNV-ECO-004 entry segment) | Results must represent the segment the business case depends on |
| History | At least 12 months of grant, contribution, return and exception records, ideally 24 to cover two Decembers | December peak drives staffing (TEV-001) |
| Authority | Board or executive resolution authorizing data sharing for research under a data-processing agreement | Donor data is the sponsor's (CNV-TEN-001) |
| Independence | No compensation tied to the results | Avoids biased participation |

Data minimization: replay uses tokenized donor and advisor identifiers; charity EINs and public facts are kept; free-text
notes are excluded unless the sponsor redacts them. Data stays in a sponsor-specific cell and is deleted 90 days after the
study report unless the sponsor elects to retain it (CNV-PLT-004 legal holds override).

## 2. Shadow replay (12 months minimum)

**Question answered.** Of the human work the sponsor actually performed, how much would the converged rules have
eliminated, prevented, automated or still required, and with what error?

| Step | Method | Output |
|---|---|---|
| Reconstruct | Load each historical grant recommendation, contribution and payment with its as-of facts (charity status on that date from IRS archives, OFAC list version on that date, fund balance and reservations) | Bitemporal event history |
| Re-decide | Run the converged deterministic policies (eligibility, Tier-2 triggers, screening with determination reuse, capacity, authority) in recommend-only mode | Per item: auto-approve, exception class, hold, refuse |
| Compare | Join to the sponsor's actual outcome and handling record | Agreement matrix |
| Adjudicate disagreements | Independent qualified reviewer (not the study author) classifies each disagreement: platform stricter and right, platform stricter and wrong, platform looser and right, platform looser and wrong | Error classes with severity |
| Measure | Exception incidence by class; determination-reuse share (exceptions avoided because an earlier determination was reusable); false-approval candidates; false-exception rate | Lever values replacing [A] parameters |

**Acceptance of the study (not of the platform):** every disagreement adjudicated; any "platform looser and wrong" item on
a legal control (eligibility, sanctions, prohibited benefit) is a blocking defect that must be fixed and re-replayed;
results reported with confidence intervals using the error-bound method of CNV-FIN-004 (no fixed sample sizes).

## 3. Time study (90 days)

**Question answered.** How many minutes does each remaining human activity take, and how often does it occur?

| Element | Specification |
|---|---|
| Activities | Exception review by class; complex-gift handling; donor, advisor and charity contacts by reason; reconciliation breaks; returns and unknown outcomes; complaints (with the CNV-PLT-005 classification rule); close and statement preparation; control reviews |
| Instrument | Work-item timestamps where the sponsor's systems record them, plus a structured self-report sample (random 10% of items) with a second coder for 200 items to measure agreement (ADV-T-040) |
| Coverage | Includes leave, training and interruptions so practical capacity per role can be computed (CNV-SVC-002) |
| Output | Distribution (not just mean) of minutes per activity; arrivals per 1,000 funds per month; December uplift if the window includes it, otherwise derived from replayed history |

## 4. Benchmark (architecture acceptance)

| Element | Specification |
|---|---|
| Declared objectives first | RPO/RTO per data class and SLOs per capability (CNV-ARC-001, CNV-SVC-002) recorded before the run |
| Workload | Largest declared sponsor's replayed history scaled to its peak December day; statement generation concurrent with dispatch (ADV-T-036) |
| Topology | One managed PostgreSQL per sponsor cell with synchronous standby; replicated hold publications and beacon (CNV-ARC-003); outbox |
| Fault injection | Standby failover during dispatch; hold-registry outage beyond freshness bound; bank-feed staleness; rail-partner outage (CNV-PLT-012) |
| Pass criteria | Declared objectives met; no duplicate dispatch; no acknowledged financial fact lost; fail-closed behaviour observed exactly where the clauses require it |

## 5. How results feed back

1. Replace the [A] parameters in `model/enterprise_economics.py` with measured distributions, re-run, and replace report 05's
   lever-dependent conclusions; until then report 05 stands as written.
2. Record each measured value as an `evidence-reviewed` event on the questions it settles (for example RR-UNB-GRT-003-r1#Q1,
   UNB-XIN-ECO-006#Q1, UNB-XIN-SVC-008#Q1) and close the corresponding CNV-EVB-001 items.
3. Only then may CNV-ECO-003's gate pass for customer cost-saving claims and segment commitments. Licence prices are not
   gated on this protocol: the board sets them at what the market will bear (OD-M-11, 2026-11-25).

## 6. Owner-side prerequisites

Executed data-processing agreement with the design partner; budget for the independent adjudicator; a managed-database
account for the benchmark. None of these is a paid legal or accounting consultation.

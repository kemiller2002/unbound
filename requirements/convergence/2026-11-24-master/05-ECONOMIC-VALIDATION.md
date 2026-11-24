# 05 — Economic validation and business redesign (Phase 4, revised after adversarial review)

**Status:** theoretical. The model ([`model/enterprise_economics.py`](model/enterprise_economics.py)) imports the
executed simplification model (`requirements/discovery/2026-11-24-simplification-economics/model/model.py`) and
`decision_teams.py` **unchanged**. About 60 of the 73 base parameters are assumptions; the enterprises, approach
overrides, mixes, licence scales and vendor assumptions added here are labelled. Nothing is observed business
performance. Tables: [05a-ECONOMIC-TABLES.md](05a-ECONOMIC-TABLES.md). Reproduce:
`python3 -I -B model/enterprise_economics.py && python3 -I -B model/render_tables.py` (byte-identical; `model/hashes.txt`).

**Revision.** The first version of this report (commit `42d83f0`) priced the Unbound licence at a whole-platform
scale of 1–3 million funds, compared Unbound against a traditional operation given a 30% per-item review rate, and
modelled the community foundation at 1,500 funds. The business adversarial review (report 08, ADV-B-001 to ADV-B-006,
ADV-B-010) showed each of these to be unsupported. I verified the market facts against the repository's own data
(DAFRC FY2024 sponsor panel in `daf-landscape`): **802 community foundations hold 104,425 DAF accounts; median 46,
p75 128, p90 313; only 7 hold 1,500 or more; only 20 US sponsors of any kind hold 5,000 or more accounts; traditional
invested DAF accounts are at most ~737k.** The model and this report were corrected; the earlier figures are
superseded.

## 1. What is modelled

- **Five enterprises** (report 04) at observed or stated sizes: E1 independent sponsor (50k funds), E2 community
  foundation (**128 funds = p75**, also 46, 313, 1,500), E3 advisor-distributed (40k; release-1 revenue is the sponsor
  admin fee only, because fund-paid advisor compensation is blocked by CNV-GRT-009), E4 workplace (500k), E5
  administrator (50 sponsors × 5k).
- **Seven approaches:** A traditional (30% per-item review, an assumption); **A8** traditional with the same 8%
  exception definition as the others; AL traditional workflows on licensed software; B AI-enhanced self-built;
  **C0** Unbound without the unmeasured redesign levers; C AI-native redesign; D C plus elimination.
- **Licence** = Unbound vendor cost per fund at whole-platform scale plus 30% markup: **$49 at 250k funds (favourable),
  $91 at 100k (base), $229 at 25k (adverse)**; $28 at 1M kept only as a labelled market-exceeding reference.
- **Measure:** total economic administration cost per fund-year, whoever bears it, excluding investment management and
  acquisition. Cost transfers never count as savings.

## 2. Results (base licence, expected values, $/fund-year)

Deterministic expected values are shown for comparability across approaches; the skewed parameter distributions put Monte Carlo P50 higher (05a §1 gives P10/P50/P90). Under CNV-ECO-003 any customer-facing or commercial figure must headline P50 with its range.

| Enterprise | A | A8 | AL | B | C0 | C | D |
|---|---|---|---|---|---|---|---|
| E1 independent (50k) | 465 | 320 | 453 | 313 | 301 | 212 | 205 |
| E2 community foundation (128) | 33,561 | 33,561 | 6,361 | 33,557 | 6,356 | 6,671 | 6,669 |
| E3 advisor-distributed (40k) | 888 | 634 | 860 | 622 | 595 | 351 | 338 |
| E4 workplace (500k) | 87 | 62 | 165 | 57 | 135 | 121 | 118 |
| E5 administrator (250k) | 376 | 231 | 446 | 224 | 302 | 255 | 251 |

**Unbound vendor P&L** (report 05a §2b): break-even licence ≈ **$217/fund at 25k platform funds, $131 at 50k, $81 at
100k, $43 at 250k, $31 at 500k, $24 at 1M**, including build amortisation and sales. At $28 Unbound loses money below
about 1M funds, which exceeds the addressable traditional market.

## 3. Findings

| # | Finding | Consequence |
|---|---|---|
| EV-01 | **Unbound's own business case as a vendor is not demonstrated.** Break-even needs roughly $80–130 per fund at 50–100k platform funds, i.e. 10–20 sponsors of 5,000 funds when only 20 such sponsors exist in the US. | Pricing and market entry are gated on evidence (CNV-ECO-003). The platform's viability depends on winning a large share of mid-size and large sponsors or on a different revenue model. |
| EV-02 | **Unbound's advantage over AI-enhanced incumbents rests on redesign levers that are not yet measured.** C0 (Unbound without levers) is about equal to B: E1 $301 vs $313, E3 $595 vs $622; with the levers C is $212 and $351. The levers are determination reuse, shorter exception handling, fewer contacts, fewer reconciliation breaks and complex-gift handling cut from 12 to 2 hours (worth ≈$61/fund for E1 and ≈$244 for E3). Several are policy changes an incumbent could adopt. | Measure the levers first (CNV-EVB-001 extended); headline comparisons use C0 as base and C as upside until measured. |
| EV-03 | **The 30% traditional review rate inflated the saving.** With the same 8% definition (A8), E1 traditional falls from $465 to $320; about 46% of the original A→C gap came from that assumption. | A8 is the fair traditional comparator. |
| EV-04 | **The community-foundation entry hypothesis is not supported.** At observed sizes (46–313 DAFs) every approach is dominated by fixed floors (decision-team floor, audit, entity costs) that the model charges wholly to the DAF program; redesign adds nothing over licensed traditional software (C $6,671 vs AL $6,361 at 128 funds) and incumbent CF software already serves all fund types. The model cannot represent CF economics properly without driver-based allocation of shared foundation staff (TEV-002). | SPD-CMP-001 direction revised: community foundations are not the entry segment; serve them, if at all, through partnership with CF software or as a DAF module. Entry segment = mid-size national and single-issue sponsors with ~5k–50k funds (E1-like) and advisor-distributed sponsors (E3), subject to EV-01. |
| EV-05 | **Workplace and administrator models do not benefit at realistic licence scale.** E4 C $121 vs B $57; E5 C $255 vs B $224. At $28 (1M platform funds) they did; at $91 they do not. | Activate E4/E5 only with tiered licence pricing at measured cost and a positive contribution margin at adverse scale (CNV-ECO-002, CNV-ECO-003). |
| EV-06 | **E3 margin was overstated.** Release 1 cannot use fund-paid advisor fee sharing (CNV-GRT-009). On the sponsor admin fee alone E3 still has the largest margin ($2,172 revenue vs ≈$351 cost). | E3 economics hold without advisor compensation; the advisor-sponsored structure is a later option requiring a sponsor-engaged manager program. |
| EV-07 | **Sponsor margins under asset-based pricing are large** (E1 revenue ≈$693 vs ≈$212). | Incumbents have little cost pressure to switch; Unbound's pitch must be price competition, transparency, controls and evidence, not cost alone. |
| EV-08 | **Activation gates delay automation.** Up to eight sponsor-side authorizations (board delegations, auditor approach, bank agreements, controller policies) must exist before C's levers apply; until then the sponsor operates near C0. | Model results for C are steady-state only; onboarding plans must schedule the gates (report 09 §3). |
| EV-09 | **AI inference cost is immaterial** (≤ $1/fund) in every case; people dominate sensitivity (loaded cost, minutes per exception, exception rate). | Unchanged from the first version. |
| EV-10 | **December peak** lowers exception SLA attainment to ≈0.70 at modelled staffing (TEV-001). | Unchanged. |

## 3a. Evidence update: incumbent prices (report 13)

Public Form 990 data puts incumbent software at $32–$129 per DAF-year for large national sponsors, about $100 per fund-year (all funds) at community foundations and $413–$745 for fully outsourced administration. Unbound's break-even (≈$81 at 100k platform funds, ≈$131 at 50k) is therefore inside or above the large-national band: price competition is viable only against mid-size nationals, community foundations priced per fund, and outsourced-administration buyers. EV-01 stands and is sharper.

## 4. Why each activity exists, and what each approach does with it

Unchanged from the first version and still valid as a classification (law, contract, integrity, value, risk,
convention); see [05a](05a-ECONOMIC-TABLES.md) §3 for activity costs under each approach. Human involvement is retained
wherever judgement, accountability, independence, accessibility or legal authority requires it.

| Activity | Basis | A | B | C | D eliminate |
|---|---|---|---|---|---|
| Acquisition | Value | Marketing | Same | Embedded and partner distribution | — |
| Onboarding and identity | Risk; no CIP mandate (RCD-023) | Manual | Prefill | Verified prefill, risk-based proofing | Re-keying |
| Contributions | Law (§170 substantiation) | Manual matching | Assisted | Typed rails, payroll batching | Paper by default |
| Complex assets | Law, risk | Staff | Staff + AI | Agent-model intermediaries | — |
| Investment administration | Contract, fiduciary | Manual NAV | Sampled | Delegated deterministic NAV | Per-NAV checker |
| Charity vetting | Law (§4966; Rev. Proc. 2018-32) | Per grant | Exceptions | Continuous monitoring, reuse | Manual revalidation |
| Grant decisions | Law (sponsor control) | Every grant | Exceptions | Delegated deterministic approval | Per-grant sign-off where delegated |
| Payments | Contract (Nacha) | Checks | ACH | Verified ACH default | Checks except by request |
| Service | Value, accessibility | Phone | Chat | Proactive, async, accessible human route | Status-chasing contacts |
| Reconciliation | Integrity, law | Monthly manual | Assisted | Continuous, typed items | Per-journal approval |
| Statements | Program terms | Paper | Electronic | On demand + cadence | Paper by default |
| Assurance | Law, contract | Checklists | Assisted | Evidence by design | Checklists once accepted |

## 5. Durable differentiation (revised)

The review's point stands: the cost advantage alone is copyable. What is harder to copy: deterministic controls that
emit audit evidence by design, a shared registry of re-verified public charity facts (network effect), a single program
ledger bridged to Schedule D, and published, measured service levels. Easy to copy: automated revalidation (public
data), embedded checkout, AI-readable documentation, and the policy levers (reuse, sampling, ACH default). The
strategy must therefore lead with control, evidence and transparency for mid-size and large sponsors, and treat cost as
a consequence to be measured, not the headline.

## 6. Distinguishing efficiency, transfer and projection

- **Real efficiency (if measured):** fewer human touches per grant, fewer contacts, continuous reconciliation, shared software.
- **Transfers:** complex-asset fees to funds; acquisition to employers or partners; decision staff to sponsors (SPD-ADV-004).
- **Projections:** every lever, the licence scale and the vendor P&L.

## 7. Failure conditions

The business case fails if: measured levers are small (C converges to C0 ≈ B); Unbound cannot reach roughly 100k funds
across 10–20 sponsors within the funding horizon; auditors or boards do not accept deterministic controls (sponsors
operate near C0 indefinitely); or asset-fee compression outruns the cost advantage.

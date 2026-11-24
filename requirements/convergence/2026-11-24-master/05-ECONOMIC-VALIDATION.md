# 05 — Economic validation and business redesign (Phase 4)

**Status:** theoretical. The model ([`model/enterprise_economics.py`](model/enterprise_economics.py)) imports the
executed simplification model (`requirements/discovery/2026-11-24-simplification-economics/model/model.py`) and
`decision_teams.py` **unchanged**. About 60 of the 73 base parameters are assumptions; the five enterprises, the
approach overrides, the community-foundation and advisor mixes and the licence markup added here are all
assumptions ([A]). Nothing is observed business performance. Tables: [05a-ECONOMIC-TABLES.md](05a-ECONOMIC-TABLES.md).
Reproduce: `python3 -I -B model/enterprise_economics.py && python3 -I -B model/render_tables.py` (byte-identical
outputs; hashes in `model/hashes.txt`).

## 1. What was modelled

- **Five enterprises** (report 04): E1 independent sponsor, E2 community foundation, E3 advisor-distributed,
  E4 workplace giving, E5 outsourced administrator — at three scales each.
- **Five approaches** per enterprise:
  - **A** traditional human-centred operation on self-built systems (30% of grants touched by a person, no AI);
  - **AL** A's workflows on licensed software (isolates *workflow* effects from *build-versus-buy* effects);
  - **B** existing workflow enhanced with AI (exception-based review, AI assistance, self-built systems);
  - **C** AI-native redesign on Unbound (determination reuse, typed exceptions, proactive notices, default ACH,
    per-sponsor decision teams per SPD-ADV-001/004, licence fee);
  - **D** C plus elimination of activities whose only basis is convention.
- **Measure.** *Total economic administration cost per fund-year*: every administration cost line whoever bears it
  (enterprise or funds), excluding investment-management fees and customer acquisition, which are reported
  separately. Moving a cost to funds, donors, employers or Unbound therefore never appears as a saving.
- **Ranges.** Expected value, and Monte Carlo P10 (favourable), P50 (base), P90 (adverse) over 300 runs.
- **Separations.** Fixed vs variable, enterprise vs fund vs acquisition bearer, activity, and the Unbound licence
  (vendor cost at whole-platform scale of 250k / 1M / 3M funds plus 30% markup: about $49 / $28 / $23 per fund-year).

## 2. Results

| Enterprise | A | AL | B | C | D | C range P10–P90 |
|---|---|---|---|---|---|---|
| E1 independent sponsor (50k) | 465 | 390 | 313 | 149 | 142 | 170–253 |
| E2 community foundation (1.5k) | 3,552 | 1,174 | 3,211 | 739 | 737 | 781–1,025 |
| E3 advisor-distributed (40k) | 888 | 797 | 622 | 289 | 275 | 339–531 |
| E4 workplace giving (500k) | 87 | 102 | 57 | 58 | 55 | 56–86 |
| E5 outsourced administrator (250k) | 376 | 383 | 224 | 192 | 188 | 207–285 |

$ per fund-year, total economic administration, expected values. The deterministic expected value can sit below
P10 because the Monte Carlo samples skewed (triangular) parameters.

### Findings

| # | Finding | Evidence in the model | Consequence |
|---|---|---|---|
| EV-01 | **Workflow redesign, not AI spend, produces most of the saving.** For E1, charity vetting and grant decisions fall from $196 (AL) to $17 (C), driven by the exception rate, determination reuse and minutes per exception. AI inference is ≤ $1 per fund-year in every approach and every sensitivity case. | Activity tables §3; sensitivity §5 (AI cost parameters move cost by < $1) | The business case rests on exception incidence and reuse of determinations. These are the first empirical validations (evidence backlog EB-01, EB-02). |
| EV-02 | **Build-versus-buy dominates for small organizations.** E2 on self-built systems (A, B) costs $3,200–3,600 per fund; on licensed software with the same human workflows (AL) $1,174; with redesign (C) $739. | A vs AL vs C for E2 | Community foundations gain mostly from shared software; redesign adds roughly a further third. |
| EV-03 | **AI-enhanced incumbents (B) close much of the gap at scale.** E1 B $313 vs C $149; at 250k funds B $224 vs C $138. | Scale table §2 | A competitor that adopts exception-based review captures part of the benefit without Unbound. The durable remainder is in §4. |
| EV-04 | **For high-volume low-balance books the licence is the largest cost.** E4 C $58, of which the licence is about $28; a self-built AI-enhanced operation at 2M funds is cheaper ($47 vs $57). | E4 activity table; scale table | Unbound must price digital-minimal servicing classes by measured platform cost (TEV-010); otherwise large workplace providers should build. |
| EV-05 | **Per-sponsor decision teams cap the administrator model's advantage.** E5 C $192 vs B $224: 50 sponsors × a 2.5-FTE decision-team floor ≈ 125 of the modelled 141 FTE. | E5 FTE; decision_teams floor | Accepted (SPD-ADV-001). Part-time and contracted authority holders reduce but do not remove the floor (TEV-003, SPD-ORG-002 dossier). |
| EV-06 | **Community-foundation cost is fixed-cost dominated.** E2 C: $553 of $739 is fixed (team floor, audit, entity costs). | Fixed vs variable | A foundation already bears audit and entity costs; charge the DAF program only its driver-based share (TEV-002). The model's $739 overstates the incremental cost. |
| EV-07 | **Complex-asset outsourcing is a transfer, partly offset by real savings.** C moves complex-gift processing to a fund-borne vendor fee (+$17 per E1 fund) while removing $42 of internal effort. | Bearer tables | Counted net. Fund-borne charges need disclosure and authority (SPD-ECN-005 dossier). |
| EV-08 | **Margins under asset-based pricing are large.** E1 revenue about $693 per fund against $171 enterprise cost; E3 $1,654 against about $194. A 30% market decline cuts E1 margin from $582 to $426. | Revenue table | Price is the main competitive lever available to an Unbound-based sponsor; lower fees for donors are an economically real benefit, not a cost transfer. |
| EV-09 | **Workplace giving is thin.** E4 margin about $5 per fund at $12 employer fee plus asset fees above minimums. | Revenue table | Activate only with employer-paid fees or funded subsidy commitments (UNB-SMP-ECN-003; TEV-011). |
| EV-10 | **The most sensitive inputs are people, not machines:** loaded cost per FTE, minutes per exception, exception rate and contact rate. | Sensitivity table | Validate with time studies (UNB-XIN-ECO-006) before any price commitment. |
| EV-11 | **December peak:** exception SLA attainment ≈ 0.70 for E1, E3, E4 and E5 with no cost change (backlog absorbs it). | Stress table | Surge planning requirement (TEV-001). |
| EV-12 | **Regulatory change and sponsor exit are the costliest shocks for small organizations** (E2: +$100 and +$78 per fund). | Stress table | Policy-pack research shared across sponsors (public facts only) is valuable to small sponsors; exit tooling must be cheap. |

## 3. Why each activity exists, and what each approach does with it

| Activity | Basis (law / contract / integrity / value / risk / convention) | A traditional | B AI-enhanced | C AI-native | D eliminate |
|---|---|---|---|---|---|
| Account acquisition | Value | Marketing | Same | Embedded and partner distribution | — (cannot eliminate) |
| Onboarding and identity | Risk (OFAC, fraud); no CIP mandate for sponsors (RCD-023 dossier) | Manual forms | Prefill | Verified prefill, risk-based proofing | Remove re-keying |
| Contributions | Law (§170 substantiation), integrity | Manual matching | Assisted matching | Typed rails, payroll batching | Paper checks by exception |
| Complex assets | Law (valuation, §170(f)(11)), risk | Staff | Staff + AI | Qualified intermediaries, fund-borne disclosed fee | — |
| Investment administration | Contract, fiduciary (UPMIFA) | Manual NAV check | Sampled | Deterministic in-tolerance NAV under delegation (RCD-035) | Per-NAV human checker (convention) |
| Charity vetting | Law (§4966 eligibility; Rev. Proc. 2018-32 reliance) | Per-grant review | Exception review | Continuous monitoring, determination reuse, release-time check | Manual 180-day revalidation (convention) |
| Grant decisions | Law (sponsor control, variance power) | Every grant | Exceptions | Deterministic recommendation; human decision only where delegation absent or exception | Per-grant sign-off where delegation exists |
| Payments | Contract (Nacha), integrity | Checks common | ACH | Verified ACH default, partner rails | Checks except by request |
| Customer service | Value, accessibility | Phone-first | Chat-assisted | Proactive notices, async, accessible human route | Status-chasing contacts |
| Reconciliation and accounting | Integrity, law (Form 990) | Monthly manual | Assisted | Continuous, typed reconciling items | Per-journal approval (convention) |
| Statements | Contract/program terms; no statutory periodic-statement duty found (RR-REQ-X-032 dossier) | Paper quarterly | Electronic | On demand + approved cadence | Paper by default |
| Compliance and assurance | Law, contract (auditor reliance) | Checklists | Assisted | Evidence generated from inception | Manual checklists where generated evidence is accepted |
| Technology | — | Self-built | Self-built | Licensed shared platform | — |
| Management overhead | Organizational | Departments | Same | Small accountable team | Layers without decisions |
| Sponsor onboarding and exit | Contract | Bespoke | Bespoke | Policy packs, outcome questions, exit export | Bespoke configuration |

Human involvement is retained wherever judgement, accountability, independence, accessibility or legal
authority requires it: reserved decisions, exceptions, complex-asset acceptance, independent reconciliation
review, appeals, and an accessible human route for every service.

## 4. Durable differentiation

| Advantage | Mechanism | Ease for competitors to copy | Why |
|---|---|---|---|
| Eliminated per-item review | Deterministic policy + delegation + sampling | Moderate | Needs board delegations and auditor acceptance, not just software |
| Shared verified charity knowledge | Common registry of re-verified public facts, never private evidence (RCD-006, RCD-029) | Hard | Network effect across sponsors; quality compounds |
| Automated revalidation | Continuous monitoring of IRS lists and events | Easy | Data is public (EO BMF, Pub 78, auto-revocation list) |
| Deterministic controls with integrated evidence | Controls emit evidence by design | Hard | Architectural; retrofitting incumbents is expensive |
| Reduced reconciliation | Single program ledger, typed reconciling items, bridge to Schedule D | Hard | Architectural |
| Embedded distribution | Widgets and checkout | Easy | DAF-checkout networks already exist |
| Sponsor self-service configuration | Outcome questions + versioned policy packs | Moderate | Requires disciplined policy model |
| AI-readable documentation | Machine-readable assistance metadata on every field and API | Easy to start, hard to keep complete | Needs governance tooling |
| Accessibility | WCAG 2.2 AA from inception | Moderate | Incumbent UIs need rework |
| Transparency (published service levels, fee quotes) | Measured SLOs, maximum accepted quote | Easy technically, hard commercially | Incumbents resist publishing tail outcomes |

## 5. Distinguishing efficiency from transfer and projection

- **Real efficiency:** fewer human touches per grant (exception rate, determination reuse), fewer contacts through
  proactive notices, continuous reconciliation, shared software.
- **Transfers (not savings):** complex-asset vendor fees moved to funds; acquisition paid by employers (E4) or
  distribution partners; decision staff employed by sponsors rather than Unbound (SPD-ADV-004). All are shown by bearer.
- **Unvalidated projections:** every exception rate, handling time, contact rate and licence scale. The evidence
  backlog (report 08 §6) lists the measurements that would replace them.

## 6. Failure conditions

The business case fails if any of these hold after measurement: exception incidence above roughly 17% of grants
with handling above 90 minutes (C converges toward B); auditors refuse reliance on deterministic controls
(per-item review returns); sponsors cannot lawfully delegate routine approvals (RCD-005 dossier: then C uses
individual accountable batch decisions, SPD-CLR-004); Unbound platform scale stays near 250k funds (licence
≈ $49, eroding E4 and E5); or asset-based fee compression exceeds the cost advantage.

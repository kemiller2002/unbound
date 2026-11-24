# 24: Per-sponsor decision teams and pricing structure

**Date:** 2026-11-24. **Status:** owner decision recorded as `decided-provisional`. All figures are theoretical model outputs from labelled assumptions; none is an observed cost.

## 1. The decision

**SPD-ADV-001**, decided by the product owner on 2026-11-24:

- No team or person may exercise legally reserved (A3+) authority for more than one sponsor. Every sponsor has its own decision team. This covers:
  - exception grants;
  - gift acceptance;
  - treasury exceptions;
  - sanctions dispositions.
- A sponsor may staff that team itself (**SelfStaffed**). Otherwise the operator supplies a team that works for that sponsor alone (**OperatorDedicated**).
- Pricing must reflect which option the sponsor chooses.

**Consequences:**

| Record | Change |
|---|---|
| SPD-ADV-001 | `decided-provisional` (per-sponsor teams) |
| SPD-ECN-003 | `decided-provisional`. Pricing gets a per-sponsor component (section 5). |
| SPR-ADV-008 | New rejected approach. Option (c), board-appointed shared agents serving several sponsors, which the mission had recommended. |
| SPR-ORG-004 | Unchanged. Pooled operator staff deciding for all sponsors was already rejected. |
| UNB-SMP-ORG-003 | New requirement. A3+ authority is exclusive to one sponsor, and reserved work is routed only within the sponsor. |
| UNB-SMP-ECN-004 | New requirement. Three-part tariff with a decision-team component that depends on the mode. |
| SPF-ECN-015 | New finding with the economic results below |
| SPS-ORG-008, SPS-ORG-009, SPS-ECN-007 | New validation scenarios |

Not every kind of work has to be split per sponsor. These can still be pooled across sponsors:
- service;
- operations;
- evidence assembly;
- AI-prepared dossiers.

Only the decision itself must stay with the sponsor's own team. This decision is stricter than the legal minimum, so it weakens no obligation. SPD-ORG-002 stays open: counsel still has to set the appointment form for operator-dedicated principals.

## 2. How the model was re-run

`model/decision_teams.py` imports the executed model `model.py` unchanged. Its reproduction check asserts that the pooled view equals `model.py`'s own figure in every run. It then restructures only the cost lines the decision affects:

1. The pooled staff for the `exceptions` and `complex` queues are removed. Treating both queues as wholly reserved is conservative **[A]**.
2. Each sponsor gets a team sized as follows. The team floor is **2.5 FTE [A]** (range 2.0–3.5): two-person approval for A3+ decisions plus absence cover. This is the same floor as adversarial variant V1 (SPF-ADV-001).

   ```
   team = max(floor, its own reserved workload rounded up to 0.5 FTE)
   ```
3. Costs by mode:
   - **OperatorDedicated.** The operator pays the team, plus 10% management overhead **[A]** (range 5–20%) for appointment, supervision and conflict attestation.
   - **SelfStaffed.** The sponsor pays the team at its own loaded cost, at 1.0× the operator's **[A]** (range 0.8–1.3). The operator pays $15,000 per sponsor per year **[A]** (range $5k–$30k) for authority-register set-up, training, attestation evidence and audit support.

The run uses the same seed (20261124) and 300 Monte Carlo runs per cell. A second run produced a byte-identical `decision-teams-results.json` (`hashes.txt`).

Command: `cd model && python3 -I -B decision_teams.py`

## 3. Cost per account (enterprise operating cost, excluding acquisition)

Values are P10/P50/P90 in USD per account-year **[D]**.
- *Sponsors* is the median number of sponsors across runs; accounts per sponsor is sampled.
- The last two columns split the self-staffed cost between who pays it.

| Model | Accounts | Sponsors | Pooled (pre-decision) | Operator-dedicated | Self-staffed | Self-staffed: operator pays | Self-staffed: sponsor pays |
|---|---:|---:|---|---|---|---:|---:|
| OM2 | 100,000 | 1 | 142/180/224 | 141/179/223 | 141/179/223 | 0 | 179 |
| OM2 | 1,000,000 | 1 | 87/114/158 | 87/114/158 | 87/114/158 | 0 | 114 |
| OM3 | 100,000 | 12 | 155/192/243 | 168/214/306 | 162/211/305 | 121 | 89 |
| OM3 | 1,000,000 | 112 | 94/125/168 | 106/147/223 | 100/142/224 | 58 | 84 |
| OM3 | 5,000,000 | 608 | 91/121/169 | 101/148/230 | 97/143/219 | 53 | 88 |
| OM4 | 100,000 | 3 | 81/95/108 | 86/106/131 | 86/104/129 | 85 | 18 |
| OM4 | 1,000,000 | 25 | 24/29/37 | 30/39/67 | 29/38/65 | 23 | 15 |
| OM4 | 5,000,000 | 124 | 18/23/29 | 23/32/55 | 23/32/54 | 17 | 15 |
| OM5 | 1,000,000 | 10 | 17/20/24 | 20/23/26 | 19/22/26 | 16 | 7 |
| OM5 | 5,000,000 | 50 | 12/14/17 | 14/16/20 | 14/16/19 | 10 | 7 |

In the "sponsor pays" column, each sponsor's entity costs (board, audit, Form 990, D&O; `sponsor_entity_cost`) are included in both modes.

The self-operated models (OM1 and OM2) barely move. They already had one team serving one sponsor. The new floor only rounds their small books up.

**What the numbers say:**

1. **The two modes cost the enterprise almost the same.** At OM3@1M the median is $147 operator-dedicated against $142 self-staffed. The gap is the management overhead less the support fee. The difference is *who pays*, not *how much*. Pricing therefore has to follow the mode.
2. **The shared platform loses most of its advantage over a lean self-operated sponsor.**
   - OM3@1M median: $125 → $147, against OM2@1M at $114.
   - OM3@100k: $214, against OM2@100k at $180.
   - At OM3's sponsor sizes, a shared platform now saves money only on pooled service, operations and technology, not on decisions.
3. **Cost targets reached by the lowest deterministic scale, compared with the pre-decision pooled model.**

   | Target | OM3 | OM4 | OM5 |
   |---|---|---|---|
   | $100 | Reached at 1M before; **now never** (up to 10M) | 300k operator-dedicated, 100k self-staffed (was 100k) | 100k, unchanged |
   | $50 | — | 1M (was 300k) | 300k, unchanged |
   | $25 | — | **Now never** (was 1M) | 1M, unchanged |
   | $10 | — | — | **Now never** (was 10M) |

   OM5 holds up because its sponsors are assumed to be very large (100,000 accounts each **[A]**), so the floor is spread thinly.

## 4. Sponsor size is now the dominant price driver

This table is deterministic and uses OM3 workload: 0.173 reserved FTE per 1,000 accounts **[D]**. The 2.5 FTE floor binds below about **14,500 accounts per sponsor**. With digital-minimal workload (OM4) it binds below about 232,000 accounts.

| Sponsor accounts | Team FTE | Operator-dedicated cost per sponsor | Per account | Self-staffed new-hire cost | Self-staffed using existing staff (time used only) | Operator support per account |
|---:|---:|---:|---:|---:|---:|---:|
| 500 | 2.5 | $453,750 | $907.50 | $412,500 | $14,264 | $30.00 |
| 1,000 | 2.5 | $453,750 | $453.75 | $412,500 | $28,528 | $15.00 |
| 2,500 | 2.5 | $453,750 | $181.50 | $412,500 | $71,320 | $6.00 |
| 5,000 | 2.5 | $453,750 | $90.75 | $412,500 | $142,640 | $3.00 |
| 10,000 | 2.5 | $453,750 | $45.38 | $412,500 | $285,279 | $1.50 |
| 25,000 | 4.5 | $816,750 | $32.67 | $742,500 | $713,198 | $0.60 |
| 100,000 | 17.5 | $3,176,250 | $31.76 | $2,887,500 | $2,852,793 | $0.15 |

**Implications:**

- **Small sponsors cannot afford an operator-dedicated team at market DAF fees.** At a typical blended asset fee, a 1,000-account sponsor would pay over $450 per account for decisions alone.
- **Self-staffing is the natural option for small sponsors.** A community foundation that already employs two people who can be appointed (for example, the executive director and CFO) pays only for the time those people use: about $28,500 a year at 1,000 accounts. That is a lower bound; the new-hire column is the upper bound.
- The entry hypothesis in SPD-CMP-001 (community foundations and independent sponsors) only works with the self-staffed mode. This needs data on sponsor sizes (SPF-ADV-024).

## 5. Pricing structure (UNB-SMP-ECN-004)

Pricing has three components, each shown separately on quotes, invoices and cost-to-serve reports:

| Component | Charged | Covers |
|---|---|---|
| **A. Platform fee** | Per account, by servicing class | Pooled service, operations, technology, AI, payments, insurance, audit and data |
| **B. Sponsor service fee** | Per sponsor, per year | Sponsor configuration, policy-pack upkeep, sponsor reporting and the relationship |
| **C1. Dedicated decision team** | Per sponsor, priced on team FTE | Operator-employed deciders who are board-appointed and exclusive to that sponsor |
| **C2. Self-staffed support** | Per sponsor, per year | Authority register, training, attestation evidence and audit support for the sponsor's own team |

The sponsor pays either C1 or C2, never both.

Cost-recovery floors **[D]** use expected values and no margin. They are not prices: price levels and margin belong to the CFO and commercial lead.

| Model | Accounts | Sponsors | A per account | B per sponsor | C1 per sponsor | C2 per sponsor | Self-staffed sponsor's own staff |
|---|---:|---:|---:|---:|---:|---:|---:|
| OM3 | 100,000 | 20 | $97.90 | $37,000 | $453,750 | $15,000 | $412,500 |
| OM3 | 1,000,000 | 200 | $40.27 | $37,000 | $453,750 | $15,000 | $412,500 |
| OM4 | 1,000,000 | 50 | $17.11 | $37,000 | $453,750 | $15,000 | $412,500 |
| OM5 | 5,000,000 | 50 | $7.14 | $62,000 | $453,750 | $15,000 | $412,500 |

**Rules carried in the requirements:**

- **C is never spread across other sponsors' fees.**
- **Changing mode is gated on coverage.** The incoming team's DoA entries must be active, and AuthorityCoverage (UNB-SMP-ORG-002) must pass for 20 consecutive business days before the switch. Until then the outgoing team keeps deciding and keeps being charged.
- **The mode and the fees never affect grant handling.** They are not inputs to eligibility, sanctions, approval, grant priority or payment timing.

## 6. Limitations

- **The workload model is not sponsor-specific.** Per-sponsor team sizes use the platform-wide average reserved workload per account. Variation between sponsors is not modelled, so teams for sponsors with lumpy workloads are understated.
- **Splitting the pool may cost more than modelled.** Pooled queues absorb random peaks better than many small teams. The model applies a floor and rounding but no queueing penalty per team, so December backlogs per team may be worse than the pooled simulation shows.
- **Seasonal and contract staff for the reserved queues were not restructured.** Under this decision, seasonal staff could only help prepare work for a sponsor's own deciders, not decide.
- **Every new parameter is an assumption [A].** This is the same model family as every other pass, so it is not independent validation.
- **The cooling-off question is open.** It is not decided whether a principal may move from one sponsor's team to another's without a waiting period (open question on UNB-SMP-ORG-003).

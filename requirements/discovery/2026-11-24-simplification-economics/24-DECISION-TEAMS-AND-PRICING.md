# 24: Per-sponsor decisions, a software-only Unbound, and pricing

**Date:** 2026-11-24. **Status:** two owner decisions recorded as `decided-provisional`. All figures are theoretical model outputs from labelled assumptions; none is an observed cost.

## 1. The decisions

The product owner made two decisions on 2026-11-24.

**SPD-ADV-001.** No team or person may exercise legally reserved (A3+) authority for more than one sponsor, so every sponsor decides with its own people. The reserved decisions include:
- exception grants;
- gift acceptance;
- treasury and payment exceptions;
- sanctions dispositions.

**SPD-ADV-004.** For now, Unbound is a software platform only. Unbound builds, hosts and supports the software. The organizations that use it employ everyone who does human work. There are two kinds of user organization:
- a **sponsor** using Unbound directly;
- a **third-party administrator** acting for several sponsors.

Hooks are kept so that Unbound can later offer staffed services without a redesign. Those services could be supplied staff, a managed service, or an affiliated sponsoring charity of the kind Ren has with Renaissance Charitable Foundation.

This matches how existing servicers work. In the daf-landscape research on Ren, the servicer vets grants and prepares a recommendation, and the sponsoring charity keeps final approval (`daf-landscape:research/ren-distribution-network-institutional-program-comparison-2026-11-12.md:33`).

| Record | Change |
|---|---|
| SPD-ADV-001 | `decided-provisional`. Decisions are per sponsor, made by the sponsor's own people. |
| SPD-ADV-004 | New, `decided-provisional`. Unbound is a software platform only, with hooks for services. |
| SPD-ECN-003 | `decided-provisional`. Pricing is a licence: a per-account fee plus a per-sponsor tenant fee. |
| SPR-ADV-008 | Rejected: board-appointed shared agents serving several sponsors. |
| SPR-ADV-009 | Deferred: Unbound supplying staff, managed operations or an affiliated sponsoring charity. |
| UNB-SMP-ORG-003 | Records which organization employs each principal. Only a sponsor's own DecisionTeam may hold A3+ authority. Administrators may prepare work but never decide. A StaffingModel field has disabled future values as a hook. |
| UNB-SMP-ECN-004 | Typed fee components. Only the licence fee (A) and tenant fee (B) are enabled. New component types can be added later by configuration and an owner decision. |
| SPF-ECN-015 | Finding with the economic results below |
| SPS-ORG-008, SPS-ORG-009, SPS-ECN-007 | Validation scenarios |

## 2. Who does what

| Party | Does | Never does |
|---|---|---|
| **Unbound (vendor)** | Builds, hosts, secures and supports the software. Runs AI and data services inside it. Sets up each sponsor tenant, including its authority register. | Makes or approves any grant, gift, payment or sanctions decision. Holds no business role at release 1. |
| **Sponsor** | Employs or board-appoints its DecisionTeam, which makes all reserved decisions. May also staff its own service and operations. | Lets anyone outside its team decide. |
| **Third-party administrator** (optional) | Does non-reserved work for the sponsors it is contracted to: donor service, operations, evidence assembly and case preparation. | Holds A3+ authority for any sponsor. |

## 3. How the model was re-run

`model/decision_teams.py` imports the executed model `model.py` unchanged. Its reproduction check asserts that the pooled view equals `model.py`'s own figure in every run. It then reassigns each cost line to the party that bears it.

**Vendor costs [A].** Unbound bears:
- leadership, engineering, AI agent ownership, security and philanthropic-intelligence staff;
- AI inference;
- infrastructure;
- data providers, eligibility screening and identity checks;
- platform insurance;
- per-tenant support;
- authority-register set-up ($15,000 per sponsor per year [A]).

**Customer costs.** Everything else that is an operating cost:
- all human-work staff;
- payment rails;
- audit and legal filings;
- advisor relationships;
- the sponsor entity's own costs.

**The two routes a customer can take:**

- **Sponsor-direct.** Each sponsor employs everyone it needs: deciders, service and operations. Its staff is the larger of:
  - its workload;
  - the minimum of **2.5 FTE [A]** (range 2.0–3.5). This is five named authority holders at half time each, which covers two-person approval and absence. It is the same floor as report 10's S1 sponsor.
- **Via an administrator.** One administrator pools service and operations across its sponsors. Each sponsor still keeps its own decision team, sized as the larger of its reserved workload and the same 2.5 FTE minimum.
- **Future operator-dedicated (a hook).** This is computed for reference only. It is not offered.

The run uses the same seed (20261124) and 300 Monte Carlo runs per cell. A second run produced a byte-identical `decision-teams-results.json` (`hashes.txt`).

Command: `cd model && python3 -I -B decision_teams.py`

## 4. Results

### 4.1 Total DAF operating cost per account (customer and Unbound together, excluding acquisition)

Values are P10/P50/P90 in USD per account-year **[D]**. *Sponsors* is the median number of sponsors; accounts per sponsor is sampled. The last two columns split the sponsor-direct median between Unbound and the customer.

| Model | Accounts | Sponsors | Pooled (before the decisions) | Sponsor-direct | Via administrator | Unbound's cost | Customer's cost |
|---|---:|---:|---|---|---|---:|---:|
| OM3 full service | 100,000 | 12 | 155/192/243 | 159/204/290 | 162/211/305 | 75 | 129 |
| OM3 full service | 1,000,000 | 112 | 94/125/168 | 99/138/219 | 100/142/224 | 21 | 118 |
| OM3 full service | 5,000,000 | 608 | 91/121/169 | 96/137/209 | 97/143/219 | 16 | 120 |
| OM4 digital-minimal | 100,000 | 3 | 81/95/108 | 81/98/123 | 86/104/129 | 67 | 29 |
| OM4 digital-minimal | 1,000,000 | 25 | 24/29/37 | 27/34/60 | 29/38/65 | 13 | 21 |
| OM4 digital-minimal | 5,000,000 | 124 | 18/23/29 | 20/27/48 | 23/32/54 | 8 | 20 |
| OM5 engineered floor | 1,000,000 | 10 | 17/20/24 | 18/21/24 | 19/22/26 | 10 | 11 |
| OM5 engineered floor | 5,000,000 | 50 | 12/14/17 | 13/15/18 | 14/16/19 | 4 | 10 |

### 4.2 What the numbers say

1. **Most of the cost of running a DAF sits with the customer, not with Unbound.**
   - At OM3@1M, Unbound's software costs about $21 per account, and the customer's people and services cost about $118.
   - Unbound's main lever on its customers' economics is software that removes human work. Fewer exceptions, faster decisions and complete case files matter more than Unbound's own unit cost. This is the same conclusion as report 09: exception share × minutes is the dominant driver.
2. **The cost targets get harder.** Lowest deterministic scale at which each target is reached:

   | Target | OM3 | OM4 | OM5 |
   |---|---|---|---|
   | $100 | Reached at 1M before; **now never** (up to 10M) | 100k, unchanged | 100k, unchanged |
   | $50 | — | 1M (was 300k) | 300k, unchanged |
   | $25 | — | **Now never** (was 1M) | 1M, unchanged |
   | $10 | — | — | **Now never** (was 10M) |

3. **Sponsor-direct comes out cheaper than going through an administrator. Treat that as an artifact of the assumptions, not a finding.**
   - A sponsor-direct team of five people both decides and does the service work within the same 2.5 FTE minimum.
   - Through an administrator, the sponsor still pays a 2.5 FTE decision minimum and also pays for the administrator's staff.
   - A team that only decides probably needs less time from each holder than 0.5 FTE. That value is not set (section 6), and it decides which route is cheaper.

### 4.3 Sponsor size drives the customer's cost

This table is deterministic and uses OM3 workload:
- all human work: 0.234 FTE per 1,000 accounts;
- reserved decisions only: 0.173 FTE per 1,000 accounts.

The 2.5 FTE minimum binds below about **10,700 accounts** for a sponsor-direct sponsor, and below about **14,500 accounts** for a decision-only team.

| Sponsor accounts | Sponsor-direct FTE | Sponsor-direct people cost | Per account | Decision-team FTE (via administrator) | Per account | Using staff already employed (time used only) |
|---:|---:|---:|---:|---:|---:|---:|
| 500 | 2.5 | $412,500 | $825.00 | 2.5 | $825.00 | $14,264 |
| 1,000 | 2.5 | $412,500 | $412.50 | 2.5 | $412.50 | $28,528 |
| 2,500 | 2.5 | $412,500 | $165.00 | 2.5 | $165.00 | $71,320 |
| 5,000 | 2.5 | $412,500 | $82.50 | 2.5 | $82.50 | $142,640 |
| 10,000 | 2.5 | $412,500 | $41.25 | 2.5 | $41.25 | $285,279 |
| 25,000 | 6.0 | $990,000 | $39.60 | 4.5 | $29.70 | $713,198 |
| 100,000 | 23.5 | $3,877,500 | $38.77 | 17.5 | $28.88 | $2,852,793 |

The last column is the lower bound. It applies to a sponsor, such as a community foundation, that already employs the people who will decide, so it pays only for the time they use. The best-fit customers are therefore:
- sponsors that already have their decision-makers on staff;
- administrators that pool non-reserved work across sponsors.

A small sponsor that must hire a team from scratch is a poor fit. This bears on the entry hypothesis in SPD-CMP-001 and needs data on sponsor sizes (SPF-ADV-024).

## 5. Pricing: Unbound's licence (UNB-SMP-ECN-004)

| Component | Charged | Covers | Release 1 |
|---|---|---|---|
| **A. Licence fee** | Per account, by servicing class | Software, hosting, AI and data services, security, engineering | Enabled |
| **B. Tenant fee** | Per sponsor, per year | Tenant set-up and support, authority-register set-up | Enabled |
| Future service components (for example operator staffing) | As configured | Staff or services Unbound might supply later (SPR-ADV-009) | Exists as a type, disabled |

Cost-recovery floors **[D]** use expected values and no margin. They are Unbound's own cost, not prices; price levels and margin belong to the CFO and commercial lead.

| Model | Accounts | Sponsors | A per account | B per sponsor | Unbound's total modelled cost |
|---|---:|---:|---:|---:|---:|
| OM3 | 100,000 | 20 | $59.26 | $52,000 | $7.0M |
| OM3 | 1,000,000 | 200 | $10.96 | $52,000 | $21.4M |
| OM4 | 1,000,000 | 50 | $9.96 | $52,000 | $12.6M |
| OM5 | 5,000,000 | 50 | $2.95 | $77,000 | $18.6M |

**Rules carried in the requirements:**
- **Administrator invoices are itemized by sponsor.**
- **Enabling a new component type needs only configuration and a recorded owner decision.** It never changes an existing contract.
- **Fees never affect grant handling.** They are not inputs to eligibility, sanctions, approval, grant priority or payment timing.

## 6. Future option: Unbound supplies the decision team (economics kept for reference)

This option is not offered at release 1 (SPR-ADV-009). It is kept so the owner can price it if SPD-ADV-004 is reopened.

**How it would work.** Unbound employs a decision team that works for one sponsor only and is appointed by that sponsor's board. The team is sized like a sponsor's own: the larger of the reserved workload and 2.5 FTE [A]. Unbound adds 10% management overhead [A] (range 5–20%) for appointment paperwork, supervision and conflict attestation. The appointment form still needs counsel (SPD-ORG-002). The executed model computes this as mode `future_operator_dedicated`.

### 6.1 Cost per account with Unbound-supplied teams

Values are P10/P50/P90 for the total, then P50 split by who pays, in USD per account-year [D]. Service and operations staff remain with the customer.

| Model | Accounts | Total | Unbound decision teams | Unbound software | Customer's own |
|---|---:|---|---:|---:|---:|
| OM3 | 100,000 | 168/214/306 | 84 | 73 | 57 |
| OM3 | 1,000,000 | 106/147/223 | 81 | 19 | 48 |
| OM3 | 5,000,000 | 101/148/230 | 84 | 14 | 48 |
| OM4 | 1,000,000 | 30/39/67 | 14 | 13 | 12 |
| OM4 | 5,000,000 | 23/32/55 | 13 | 7 | 11 |
| OM5 | 5,000,000 | 14/16/20 | 5 | 4 | 7 |

Compared with sponsor-staffed teams (section 4.1), the enterprise total is about the same: OM3@1M is $147 against $138–142. The difference is mainly who pays. Targets are reached at about the same scales as the per-sponsor routes:
- $100 by OM4 at 300k and OM5 at 100k;
- $50 by OM4 at 1M;
- $25 only by OM5 at 1M;
- never by OM3.

### 6.2 What a dedicated team would cost one sponsor

| Sponsor accounts | Team FTE | Cost per sponsor per year | Per account |
|---:|---:|---:|---:|
| 500 | 2.5 | $453,750 | $907.50 |
| 1,000 | 2.5 | $453,750 | $453.75 |
| 2,500 | 2.5 | $453,750 | $181.50 |
| 5,000 | 2.5 | $453,750 | $90.75 |
| 10,000 | 2.5 | $453,750 | $45.38 |
| 25,000 | 4.5 | $816,750 | $32.67 |
| 100,000 | 17.5 | $3,176,250 | $31.76 |

This uses OM3 workload. The 2.5 FTE minimum binds below about 14,500 accounts.

### 6.3 How it would be priced

The fee would be a third component type, already provided for in UNB-SMP-ECN-004. It would sit alongside the licence fee (A) and the tenant fee (B).

| Component | Charged | Cost-recovery floor [D] |
|---|---|---:|
| C1. Dedicated decision team | Per sponsor, priced on team FTE | $453,750 per sponsor per year at the 2.5 FTE minimum, before margin |

The rules that would apply:
- **C1 is never spread across other sponsors.** A sponsor that staffs its own team pays nothing for C1.
- **Switching to or from an Unbound-supplied team is gated on coverage.** The incoming team's authority entries must be active, and AuthorityCoverage (UNB-SMP-ORG-002) must pass for 20 consecutive business days. Until then the outgoing team keeps deciding and keeps being charged.
- **Small sponsors could not afford it at market DAF fees.** At 1,000 accounts the team alone is about $454 per account. The option fits sponsors above about 10,000–25,000 accounts that do not want to employ deciders.

## 7. Hooks for later (not built as features now)

| Hook | Where | What enabling it would take |
|---|---|---|
| Organization type on every principal (Sponsor, Administrator, Vendor) | UNB-SMP-ORG-003 | Already required at release 1 |
| StaffingModel values OperatorDedicated and AffiliatedSponsor | UNB-SMP-ORG-003 | Release flag plus a recorded owner decision; no schema change |
| Typed fee components | UNB-SMP-ECN-004 | Configuration plus a recorded owner decision; new contracts only |
| Model mode `future_operator_dedicated` | `model/decision_teams.py` | Already computed; economics in section 6 |

## 8. Limitations

- **The workload model is not sponsor-specific.** Per-sponsor teams use the platform-wide average workload per account. Pooling losses from splitting queues are applied only through the minimum and rounding, not through per-team queueing.
- **The minimum for a team that only decides is not set.** It decides which customer route is cheaper (section 4.2).
- **The split between Unbound and the customer is an assumption [A].** Insurance is assigned to Unbound and audit to the customer, so either side's cost may be under- or over-stated.
- **Seasonal and contract staff stay with the customer and are not restructured.**
- **Every new parameter is an assumption [A].** This is the same model family as every other pass, so it is not independent validation.
- **The cooling-off question is open.** It is not decided whether a principal may move between sponsors' teams without a waiting period (open question on UNB-SMP-ORG-003).

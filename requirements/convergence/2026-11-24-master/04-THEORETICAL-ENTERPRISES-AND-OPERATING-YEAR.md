# 04 — Theoretical enterprises and an operating year (Phase 3)

**Status:** theoretical validation. Nothing here is an executed software test or an observation of a real
organization. Staffing and cost figures come from the executed model in [`model/`](model/) (report 05),
which is itself built on labelled assumptions. Legal and regulatory statements cite the evidence dossiers
(report 02) by question ID; where a dossier is an interpretation, so is the statement here.

## 1. Enterprises modelled

The five target enterprises reuse and extend the ten business models recorded by the cross-industry
discovery (`requirements/discovery/2026-11-23-cross-industry/data/business-models.jsonl`). The community
foundation is new: none of the ten covered a DAF program run inside a broader community foundation.

| ID | Enterprise | Extends business model(s) | Route for human work (SPD-ADV-004) | Modelled size |
|---|---|---|---|---|
| E1 | Independent national DAF sponsor (direct to donor) | D2C, hybrid | Sponsor employs its own people | 50,000 funds (10k–250k) |
| E2 | Community foundation | new (CF) | Foundation staff; DAF program is one of several | 1,500 funds (500–5,000) |
| E3 | Wealth-management and advisor-distributed DAF provider | advisor-distribution, investment-centric, HNW-family-office | Sponsor employs; advisors service clients and manage investments | 40,000 funds (10k–200k) |
| E4 | High-volume workplace giving provider | high-volume-low-balance, embedded | Sponsor employs; employers fund acquisition and subsidies | 500,000 funds (100k–2M) |
| E5 | Enterprise outsourced DAF administrator | outsourced-admin, white-label, institutional | Administrator pools service and operations; each sponsor keeps its own decision team | 50 sponsors × 5,000 funds |

Retained as variants rather than separate enterprises: *institutional* (E5 with dedicated deployment,
XID-SAS-001), *embedded* (a channel of E1/E4, widgets in report 06), *HNW family office* (E3 high-balance
servicing class).

## 2. Enterprise structure

### 2.1 Common structure (all five)

| Element | Specification | Basis |
|---|---|---|
| Charitable entity | A §501(c)(3) public charity sponsoring organization (§4966(d)(1)) owns all fund assets and holds variance power. Unbound never owns or holds funds. | §4966; SPD-ADV-004; XID-REG-001 dossier |
| Technology vendor | Unbound: software platform only. No custody, no payment instruction authority of its own, no investment advice, no decision staff. | SPD-ADV-004, XID-REG-001, XID-REG-002 dossiers |
| Decision authority | Sponsor board → delegation of authority (DoA) → named officers and committees; deterministic policy may *recommend* everywhere, and may *decide* only where a recorded board delegation exists. | RCD-005, SPD-ENT-006, SPD-GRT-001 |
| Financial systems of record | Program ledger and fund attribution: Unbound (book of record per function map, XID-INV-007) or the sponsor's external GL in Shadow mode; entity financial statements: sponsor ERP; bank balance: bank; custody positions: custodian; grant-payment status: rail partner or ODFI. | SPD-FIN-002, XID-INV-007 |
| Payments | Sponsor-titled accounts at a bank; grants by verified ACH by default via the sponsor's ODFI or a grant-payment partner acting as the sponsor's agent; checks by exception with positive pay; wires for complex cases. | SPD-GRT-004, SPD-CMP-003, XID-BNK-005 dossier |
| Custody | Sponsor-selected custodian(s) and pool managers; Unbound reads positions and NAVs. | SPD-FIN-005 |
| Assurance | Evidence generated from inception; SOC 1/SOC 2 scope and timing driven by actual user-auditor reliance and contracts. | SPD-PLT-001 dossier |
| Regulatory obligations (sponsor) | §§4966/4967/4958 compliance; contemporaneous written acknowledgments; Form 990 incl. Schedule D Part I; Form 4720 where taxes arise; OFAC; state charitable registration; unclaimed property; privacy and communications law. | Report 02 |

### 2.2 Enterprise-specific structure

| | E1 Independent sponsor | E2 Community foundation | E3 Advisor-distributed | E4 Workplace giving | E5 Outsourced administrator |
|---|---|---|---|---|---|
| Legal entities | Sponsor (501(c)(3)); optional for-profit affiliate for marketing (related-party controls) | Foundation (501(c)(3)) with DAF, field-of-interest, designated and discretionary funds | Sponsor charity affiliated with a wealth manager or independent; RIA firms are distribution partners | Sponsor charity; employers contract with sponsor; payroll providers transmit deductions | Administrator (for-profit or nonprofit service company) + N independent sponsor charities, each a tenant |
| Customers | Donors, successors | Donors, local nonprofits, professional advisors | Advisors' clients; advisors | Employees, employers | Sponsors (and through them donors) |
| Distribution | Direct digital, embedded checkout | Relationship and advisor referrals | RIA/wealth platforms | Employer benefits platforms | Sponsor-branded white label |
| Revenue | Asset-tiered fee with minimum | Asset-tiered fee, higher minimum; investment spread where disclosed | Advisor-managed pools; sponsor admin fee share | Employer per-account fee + asset fee on larger balances | Per-account licence/administration fee from sponsors |
| Cost structure (model C, $/fund-year total economic admin) | ≈149 (range 170–253 P10–P90) | ≈739 (781–1,025) | ≈289 (339–531) | ≈58 (56–86) | ≈192 (207–285) |
| Human work (FTE, decision + service + operations, C) | ≈12 | ≈2.5 (floor) | ≈23 | ≈23 | ≈141 (50 sponsor decision teams + pooled ops) |
| Retained internally | Board, DoA officers, exception decisions, complex-gift acceptance, donor escalations | Same, plus donor relationships (core to CF mission) | Same; advisors keep investment management | Same; very small decision team relative to volume | Each sponsor: reserved decisions; administrator: service, operations, reconciliation preparation |
| Outsourced | Complex-asset liquidation (fund-borne, disclosed), audit, legal research | Investment management, audit | Investment management (advisors), complex assets | Payroll transmission, employer onboarding | Audit, complex assets |
| Shared (with other Unbound customers) | Charity facts registry (public facts only), policy-pack research, software | Same | Same | Same | Same |
| Eliminated | Per-grant human review of routine grants to listed public charities; paper statements by default; manual 180-day revalidation; per-journal approval | Same; manual fund-agreement re-keying | Same; advisor entitlement re-keying | Same; most contacts (digital-only class with accessible human route) | Same; duplicate administrator approval layers |
| Risk appetite | Low for legal/financial integrity; moderate for service latency | Very low; reputational focus | Low; advisor conflict sensitivity | Low for fraud at scale; tolerates digital-only service with accessible alternative | Very low for cross-tenant isolation |
| Escalation | Exception queue → DoA officer → executive → board committee | Program officer → CEO → board | Advisor service desk → sponsor officer → committee | Automated → service → officer | Administrator ops → sponsor's officer (never administrator decides) |

## 3. Operating year (simulated calendar)

The year runs January to December with concurrent events. Each row names the workflow, its financial
effect, controls and the requirement or decision that governs it. Events marked ⚠ produced a finding (section 5).

| Month | Concurrent events (all enterprises unless noted) | Financial effects | Controls exercised | Governing requirements / decisions |
|---|---|---|---|---|
| Jan | Contemporaneous acknowledgments for December gifts; year-end statements; 1099/990 data prep; E4 employer enrolment wave; residual December payment returns | Contribution receivables settle; ACH returns reverse availability, not the completed gift | Completed-gift date vs settlement vs availability kept separate; return handling by reason code | RCD-019, XID-BNK-006, UNB-XIN-PAY-005 |
| Feb | Audit fieldwork (prior year); bridge letter request for vendor report period gap; E2 board meeting approves DoA renewal | Audit adjustments into a controlled audit-adjustment period | Prior-period routing, issued statement versioning | XID-ADV-005, XID-EVA-002, RR-REQ-X-073-r1 |
| Mar | Quarterly statements; market decline of 30% (stress); charity revocation wave (IRS auto-revocation list update) | AUM and asset-based revenue fall; reserved capacity protected | Revoked recipients' pending grants held before release; capacity reservations unaffected | RCD-006, SAS-005 fencing, XID-ADV-003 |
| Apr | ⚠ Donor death notified for an E1 fund with two successors and a pending recurring grant | Fund enters protective state; recurring grants suspended pending successor authority | Credible-notice protection, death verification, successor authority | RCD-022, XID-TRS-003 |
| May | Form 990 preparation (Schedule D Part I counts and values); E3 advisor firm merger changes advisor entitlements | None (reporting) | Schedule D mapping by tax year; advisor delegation re-scoping | XID-INV-005, RR-ADM-002-r1 |
| Jun | ⚠ Bank feed outage for 3 days (E5 several tenants); new Nacha rule phase effective | Spendable capacity cannot be confirmed by feed | Capacity from reservations + last verified balance; fail closed on stale feed beyond window | UNB-XIN-BNK-003 correction, XID-BNK-007 |
| Jul | ⚠ Charity merger: recipient of 1,200 recurring grants (E4) merges into successor | Recurring grants re-pointed only on verified successor; donor notice | Payee substitution policy | XID-NPG-005 |
| Aug | ⚠ Regulatory change: a state amends charitable platform registration rules | Activation gate for a feature in that state | Policy pack versioning, effective dating, re-attestation | SPD-CFG-001, XID-SAS-005 |
| Sep | Data-breach scenario (credential stuffing on donor portal) | Potential fraudulent grant recommendations | Risk-based step-up, recommendation holds, notification duties | XID-ADV-009, SPD-CXP-002 |
| Oct | E5 sponsor gives notice of exit during an open investigation | Bulk transfer to successor sponsor; investigation hold survives | Per-fund transfer under policy; legal hold precedence | XID-SAS-002, XID-EVA-005, RCD-029 |
| Nov | Provider outage (AI inference down 2 days); December readiness | None directly | Deterministic paths continue; AI assistance degraded only | UNB-AIO-003 |
| Dec | Contribution surge (×3 daily arrivals); two critical employees absent 24 Dec–2 Jan; securities gifts late in month; ⚠ sanctions match immediately before dispatch; ⚠ unknown payment outcome past window | Peak receipts; year-end cut-off for completed gifts | Double-absence coverage, surge bench, dispatch fencing, unknown-outcome protocol | SPD-ORG-003, SPD-ECN-004, RCD-044, RCD-042 |

Modelled December results for approach C (report 05, stress `december_peak`): exception SLA attainment
falls to 0.70–0.73 at the default 5-business-day target for E1, E3, E4 and E5 at modelled staffing,
with no change in cost because surge is absorbed by backlog; E2 is unaffected because its floor
staffing exceeds its workload. This is a material finding (TEV-001).

## 4. Adversarial scenarios

Each scenario lists workflow, financial effects, controls, evidence, communications, responsible parties
and recovery. "Sponsor officer" means the person named in the sponsor's DoA; Unbound software performs
no decision.

| # | Scenario | Workflow and financial effect | Controls and evidence | Communications | Responsible | Recovery |
|---|---|---|---|---|---|---|
| S1 | Sanctions match immediately before payment dispatch | Grant approved, reservation held, payment instruction staged; screening at release returns a potential match. Dispatch is fenced: the instruction is not transmitted. Reservation stays (no release to available balance). | Release-time screening against current list version; atomic fence at dispatch (UNB-XIN-SAS-005 correction); disposition by authorized officer distinguishes false positive, reject, block (RCD-044); evidence: list version, match score, reviewer, decision. | Donor: neutral "grant under review" (XID-CLM-003 catalog); no tipping-off detail. OFAC report within 10 business days if blocked or rejected. | Sponsor compliance officer | False positive → release with fresh screen; true match → block (blocked-property accounting and annual report) or reject; reservation disposition per decision. |
| S2 | Two advisors concurrently recommend grants exceeding available capacity | Both requests arrive within milliseconds; capacity reservation is atomic per fund; second fails with "insufficient available balance" and is offered as a pending request or reduction. | Single-writer capacity reservation; authority matrix (XID-ADV-004): joint-required kinds wait for co-approval; conflicting instructions detected. | Both advisors see the outcome and the other pending item. | Software (deterministic); sponsor officer if conflict escalates | No overspend possible; replay test. |
| S3 | Bank outage during year-end processing | Feeds and ODFI submission unavailable 26–30 Dec. Grants approved but not transmitted; contributions received by lockbox but not reported. | Payment instructions queue with idempotency keys; completed-gift dates for contributions determined by delivery evidence, not feed time; statements marked provisional. | Donors: status page and per-grant status; year-end acknowledgment timing note. | Treasury officer; bank | Catch-up with duplicate prevention; reconciliation of in-flight items; no grant transmitted twice. |
| S4 | Approved recipient loses eligible status before payment | IRS revocation or Pub 78 removal detected after approval. Release gate re-checks eligibility evidence freshness; payment held. | Eligibility re-check at release within approved freshness window (RR-UNB-EXT-028-r1); hold reason recorded. | Donor told grant cannot be paid as recommended; options: redirect or wait. | Grant operations; sponsor officer | Grant cancelled or re-pointed with donor consent; reservation released. |
| S5 | Charity merges while recurring grants remain active | Successor verification; recurring series paused at next occurrence. | Payee substitution requires verified legal succession and donor reconfirmation unless approved policy permits substitution (XID-NPG-005 dossier). | Donors with active series notified with one-step confirm. | Charity vetting | Series continues to successor on confirmation; else ends. |
| S6 | Payment remains unknown after ordinary resolution window | ACH or check status unknown (no return, no settlement confirmation). | Aging tiers; no automatic retry; independent decision; trace request; positive-pay status (RCD-042, RCD-028). | Charity contacted through verified channel; donor sees "confirming delivery". | Treasury officer | Confirmed received → close; not received with evidence → reissue under duplicate controls; unresolvable → risk decision and recovery plan. |
| S7 | Two critical employees unavailable in December | Two of five decision-team members absent. | Roster coverage by role and day (SPD-CLR-002); double-absence stress (SPD-ORG-003); contracted qualified reviewers appointed in advance (SPD-ORG-002). | None external unless SLAs at risk. | Sponsor executive | Pre-appointed bench activated; SoD preserved; backlog cleared by mid-January. |
| S8 | Sponsor exits during an unresolved investigation | Exit plan exports funds and records; investigation hold blocks the affected funds' transfer and deletion. | Legal hold precedence over retention and exit deletion (RCD-007); per-fund transfer decision (XID-SAS-002). | Successor sponsor receives fund records; investigation details only as lawfully authorized. | Exiting sponsor board; Unbound as processor | Held funds transfer after release; read-only access continues until holds end. |
| S9 | Regulatory change invalidates an existing policy pack | A state rule changes; policy pack version superseded. | Effective-dated rule versions; emergency mandatory change path; affirmative adoption for material changes (SPD-CFG-001, RR-UNB-RULE-019-r1). | Sponsors notified with impact analysis. | Unbound policy content owner prepares; each sponsor adopts | Shadow replay; rollback evidence. |
| S10 | Compromised operator account attempts cross-sponsor access | Platform operator credential used to query another tenant. | Tenant isolation (DB per sponsor candidate, RCD-004); operator access default-deny, scoped, expiring, sponsor-approved (XID-IAM-001); anomaly alert. | Affected sponsors informed per contract and breach law. | Unbound security | Session revoked; forensic evidence; no data returned. |
| S11 | AI agent recommends an action beyond its authority | Agent proposes releasing a held grant. | Agents hold no human-authority operation; API rejects execution without recorded authority; recommendation logged (UNB-AIO-001..003). | None external. | Agent owner | Recommendation stays a recommendation; misuse pattern reviewed. |
| S12 | Cost-reduction initiative degrades accessibility or mandatory service | Proposal to remove phone channel for digital-minimal class. | Mandatory service floor and accessibility are non-removable (UNB-XIN-SAS-001); economics budgets cannot deny mandatory service (UNB-XIN-ADV-001). | Accessible alternative channel published. | Sponsor executive | Proposal rejected or redesigned with equivalent accessible route. |

## 5. Requirement feedback from the theoretical year

Every failure or weakness produced at least one change. These are incorporated into the authoritative
baseline (report 03) under the IDs below.

| ID | Finding | Kind | Change |
|---|---|---|---|
| TEV-001 | December exception SLA attainment ≈0.70 at modelled staffing for E1/E3/E4/E5. | Revised acceptance criterion + business policy | Surge capacity is planned from forecast arrivals and the legal deadline set, not from average workload; a December readiness gate requires a surge plan (pre-appointed qualified reviewers, SPD-ECN-004) before 1 December; acceptance: simulated peak meets each legal deadline and internal SLA attainment ≥ the sponsor's approved floor. |
| TEV-002 | Community foundation cost per fund is dominated by fixed floors (decision team floor, audit, entity costs) that the foundation already bears for its non-DAF programs. | Business policy + economic method | Economic model and pricing allocate shared foundation costs by documented driver; the DAF program is not charged the whole foundation's fixed cost. |
| TEV-003 | Administrator model (E5) is dominated by per-sponsor decision-team floors (≈2.5 FTE × 50 sponsors). | Architecture decision + accepted limitation | Accepted (SPD-ADV-001 stands). Requirement: the platform shall let a sponsor appoint part-time and contracted authority holders (SPD-ORG-002 dossier) with measured time and conflict attestations, so the floor is satisfied by people who also serve other functions of the same sponsor. |
| TEV-004 | Bank feed outage leaves spendable capacity unverifiable. | Requirement correction | Capacity uses reservations against last verified balance; when feed staleness exceeds the approved window, release of new grants fails closed while already-reserved approvals may proceed only if the bank confirms sufficient collected funds by another verified channel. |
| TEV-005 | Donor death with pending recurring grants and two successors. | New requirement | On credible notice of an advisor's death, suspend only that person's advisory privileges and record the evidence; surviving co-advisors keep their recorded authority. Succession starts only when no advisor with authority remains: then suspend new recommendations and future recurring occurrences, keep approved and released payments, notify successors (default 90-day response window, XID-TRS-003), and create one successor fund per successor per the fund agreement's allocation (RCD-022 dossier, batch E). |
| TEV-006 | Charity merger affects thousands of recurring series at once. | New requirement | Bulk successor handling: one verified succession record drives per-series donor reconfirmation with batch notices; no series re-points without a recorded basis. |
| TEV-007 | Exit during an investigation. | Requirement correction | Exit plans enumerate held funds; holds survive tenant exit, read-only access persists until all holds lift, and transfers of held funds are blocked. |
| TEV-008 | Sanctions screening at release must not leak match detail to donors. | Revised acceptance criterion | Donor-visible status for held grants uses the neutral catalog; tested by Playwright scenario that asserts no list name or score is rendered. |
| TEV-009 | AI agents could stage repeated out-of-authority recommendations to create pressure. | New requirement | Rate-limit and pattern-detect repeated recommendations rejected for lack of authority; report to agent owner; never auto-escalate to a decision. |
| TEV-010 | Digital-minimal (E4) economics: licence fee ≈ half of cost per fund. | Business policy | Licence pricing for digital-minimal classes is tiered by servicing class and measured platform cost; activation of high-volume low-balance needs a positive contribution margin at adverse licence scale (XID-ADV-002). |
| TEV-011 | E4 margin ≈$5 per fund at modelled revenue. | Accepted limitation + evidence task | High-volume workplace giving is viable only with employer-paid fees or subsidy commitments (UNB-SMP-ECN-003); evidence task: obtain employer willingness-to-pay data. |
| TEV-012 | Unknown payment outcome during holiday staffing. | Revised acceptance criterion | Unknown-outcome aging clocks use business calendars; aging tiers cannot auto-close; S6 scenario test included in release acceptance. |

## 6. Re-validation against the converged baseline

After Phase 2 each scenario step was traced to the clause that now governs it. A step with no governing clause would
be an unhandled transition.

| Scenario | Governing clauses (report 03) | Unhandled step found |
|---|---|---|
| S1 sanctions match before dispatch | CNV-GRT-006 (release-time screening, atomic dispatch fence, Block/Reject/Proceed, 10-business-day report data, neutral status), CNV-ARC-003 (fencing token), CNV-PLT-004 (10-year retention) | none |
| S2 concurrent recommendations | CNV-FIN-001 (atomic reservation), CNV-GRT-016 (recorded authority, conflicting instructions) | none |
| S3 bank outage at year end | CNV-PAY-007 (stale-feed fail-closed), CNV-PAY-004 (unknown outcomes), CNV-FIN-003 (completed-gift date independent of feed), CNV-ARC-003 (idempotency) | none |
| S4 recipient loses status | CNV-GRT-003 (release-time status check) | none |
| S5 charity merger | CNV-GRT-010 (successor verification, notice-and-opt-out, bulk notices) | none |
| S6 unknown payment | CNV-PAY-004 | none |
| S7 two critical absences | CNV-GOV-004 (roster coverage, double-absence stress, appointed contracted holders), CNV-SVC-002 (December readiness gate) | none |
| S8 exit during investigation | CNV-ENT-002 (held funds do not transfer, read-only persists), CNV-PLT-004 (legal hold precedence) | none |
| S9 regulatory change | CNV-GOV-005 (effective-dated packs, emergency path, affirmative adoption, independent approval) | none |
| S10 compromised operator | CNV-ARC-002 (default deny, scoped expiring grants, immediate revocation) | none |
| S11 AI beyond authority | CNV-AIO-001 | none |
| S12 cost cut degrades service | CNV-GOV-006 (non-removable capabilities, budgets never deny mandatory service), CNV-ECO-002 | none |
| TEV-005 death with co-advisor | CNV-ENT-001 | none |

TEV-001 (December capacity) and TEV-011 (workplace margin) remain dependent on empirical data (CNV-EVB-001). This is
theoretical validation of requirement coverage; no software was executed.

# 05 — DAF competitive capability coverage matrix

Date: 2026-11-23 | Evidence base: read-only extraction from `daf-landscape` (no new vendor testing; the research never ran a live Ren, TIFIN or Chariot test). Machine-readable data: [`data/competitive-evidence-rows.csv`](data/competitive-evidence-rows.csv) (755 provider × capability rows, 41 capabilities) and [`data/competitive-rollup.csv`](data/competitive-rollup.csv).

## 1. Evidence grading

| Grade | Rows | Meaning |
|---|---:|---|
| publicly-documented | 293 | First-party documentation, program circular, filing or audited statement |
| observed-verified | 37 | Presence observed (app-store listing, live portal host or footer, Form 990, audited statements). **Never** a functional test. |
| vendor-claim | 76 | Marketing, press release or sponsor homepage statement (Fidelity and Vanguard recurring-grant pages are graded here, conservatively) |
| research-inference | 7 | Inferred by the research (for example, Edward Jones administration by Ren) |
| unknown | 342 | Not assessed or not readable. In the 28-sponsor census, "ND" means *not found on the pages read*, not absent. |

**Rule applied:** no vendor claim or inference becomes a Strata/Unbound requirement. A capability classified *competitive parity* or *differentiator* creates a requirement only where the business model needs it, and that requirement cites the business rationale, not the competitor.

## 2. Coverage matrix

Coverage: **Covered** = an existing requirement specifies the capability testably or nearly so · **Partial** = present but vague, compound or missing key behaviour · **Gap** = no requirement · **Excluded** = recommended deliberate exclusion. Strategic importance: H/M/L for an AI-native, B2B, multi-sponsor administration platform.

| Cap | Market capability | Classification (reason) | Evidence quality | Existing Unbound requirements | Coverage | Missing capability | Imp. | Proposed action |
|---|---|---|---|---|---|---|---|---|
| CAP-01 | Online grant recommendation | Baseline: the core transaction at every portal | Strong | UNB-GRT-001, UNB-GRANT-001..010, UNB-GFUND-001, ADM-019, UNB-UX-001, REQ-X-002 | Partial | One canonical recommendation state machine; the grant lifecycle and event catalog use differing state names (see RCC-GRT) | H | Refine (07, RR-UNB-GRANT-*) |
| CAP-02 | Recurring / scheduled grants | Competitive parity: documented at 8 providers; DAFgiving360 reports 38% of grants recurring (sponsor-reported) | Strong | ADM-019, REQ-X-008, TVR-GR-008, UNB-GRANT (schedules), GR-EVT-005/006 | Partial | Execution-time re-evaluation contract, outage catch-up rule, durable scheduler (DG-08) | H | Refine; scenario DKS-GRT |
| CAP-03 | DAF checkout / embedded grant initiation | Potential differentiator: Chariot documents the protocol; no sponsor is publicly confirmed live on the provider API | Moderate | REQ-X-063 | Covered (as draft) | Widget host and token model (DG-03) unverified | M | Keep P2. Do not build ahead of a sponsor commitment. |
| CAP-04 | Publicly traded stock contributions | Baseline | Strong (ACATS/DTC mechanics unknown) | UNB-CUST-008, UNB-AST-001, UNB-ACC-006 | Partial | Lot-level partial receipt, valuation-date rule, acknowledgment content | H | `RR-UNB-ACC-006-r1` |
| CAP-05 | Crypto contributions | Optional: common but cyclical (The Giving Block write-down); partner-delivered | Strong | None specific (UNB-AST-001 generic) | Gap (intentional) | Intermediary-liquidation pattern | L | Exclude from MVP. Accept via intermediary that liquidates before transfer. |
| CAP-06 | Complex / illiquid assets | Parity: frequently via intermediaries (Dechomai, Realty Gift Fund) | Strong | REQ-X-059, UNB-AST-001/002, UNB-ACC-006 | Partial | §4943 holdings test; Form 8282 tracking; intermediary hand-off | M | `UNB-RCN-FIN-004`, `RR-UNB-ACC-006-r1`; partner, do not build valuation |
| CAP-07 | Advisor-managed investment accounts | Parity in wealth/bank channel; the proposed §4966 rules could treat managing advisors as donor-advisors | Strong | UNB-INV-006, UNB-CUST-*, REQ-X-062 | Covered | Regulatory watch on proposed rules | H | `UNB-RCN-REG-001` |
| CAP-08 | Advisor portal / data feeds | Parity; RIA data feeds are a differentiator (Addepar feeds only at 4 wirehouse-linked sponsors) | Strong | REQ-X-062, UNB-ADV-001/002, ADM-006 | Partial | Feed contract and scoped consent | H | Refine REQ-X-062 (ENT domain) |
| CAP-09 | White-label program delivery | Parity for a B2B platform | Moderate | REQ-X-030, REQ-X-045, UNB-PLAT-009, UNB-EXP-002 | Partial | Terminology resolver (Forma leaves terminology to the app) | H | `UNB-RCN-ARC-002` |
| CAP-10 | Family, co-advisor, successor designation | Baseline | Strong | MULTI-PARTICIPANT (UNB-PART-*, UNB-SUCC-*), REQ-X-008, REQ-X-068, ADM-004..007 | Covered | Donor-death evidence standard; successor-limit policy | H | ENT refinements |
| CAP-11 | Native mobile app | Differentiator, low priority: low ratings where present | Strong (presence) | UNB-EXP-001, UNB-UX-001 (phone-sized web flows) | Covered by mobile web | — | L | **Exclude native apps**; responsive WASM web satisfies the need |
| CAP-12 | Donor-facing charity search | Parity: eligibility data is a commodity (IRS files, Candid) | Strong | None explicit (charity registry exists: UNB-CHAR-*) | **Gap** | Search by name/EIN/cause with eligibility status shown | H | Gap `UNB-RCN-GRT` (charity search), see 04 |
| CAP-13 | Charity vetting / ratings shown to donor | Parity; ratings feeds are licence-bound and cover a minority of charities | Strong | UNB-CHAR-*, UNB-EXT-* | Partial | Licence terms for display; distinguishing eligibility from quality ratings | M | Show eligibility; ratings optional, behind licence check |
| CAP-14 | International grantmaking | Optional: direct foreign grants ≈1.9% of dollars | Strong | REQ-X-065, UNB-GRANT-023, UNB-RULE-* | Covered (draft) | ED vs ER legal question (counsel Q4) | L | Defer to post-MVP, or route via intermediary (for example, a US friends-of organization) |
| CAP-15 | Grant tracking and donor impact reporting | Grant status is baseline; donor-level impact is white space with unproven demand | Strong / absent | REQ-X-069, UNB-GRANT (status), GR-EVT | Partial | Five-dimension status vocabulary adopted platform-wide | M | Status: refine. Impact: P2 hypothesis, not a requirement |
| CAP-16 | Public API / agent access | Differentiator: only Daffy, Endaoment, Every.org (plus Chariot) document one | Strong | REQ-X-051, UNB-API-STD-*, UNB-ACC-023 | Partial | Contract source of truth; scoped OAuth for third parties | H | `UNB-RCN-ARC-001` |
| CAP-17 | Self-serve machine-readable export | Differentiator **and** exit control: no top-10 sponsor documents one | Moderate | REQ-X-061, UNB-LS-015 | Partial | Defined export schema and completeness test | H | Refine REQ-X-061 (ENT) |
| CAP-18 | Statements and tax receipts | Baseline | Strong | REQ-X-032, UNB-ACC-019/025, ACC-MDL-* | Partial | Renderer unverified (DG-04); acknowledgment content | H | `RR-UNB-ACC-006-r1`; decision RCD-009 |
| CAP-19 | Fee engine features | Baseline: tiered % converging on 0.60%, minimum fees, flat subscriptions, adviser-fee remittance | Strong | UNB-ACC-010, ADM-027/028, REQ-X-067, UNB-GRANT-045 | Partial | Fee-calculation rule precision (basis, timing, proration) | H | FIN refinements |
| CAP-20 | Account / grant minimums | Optional, program-configurable ($0 to $25k) | Moderate | UNB-GRANT-045, ADM-027 | Covered | — | L | Configuration parameter |
| CAP-21 | Workplace giving / employer matching | Optional: matching policies on DAF grants are mixed and often stale | Strong | REQ-X-070 | Covered (draft) | — | L | Defer |
| CAP-22 | Payroll funding / round-ups | **Deliberately exclude**: no sponsor offers it; corporate micro-giving products closed | Weak | REQ-X-070 (partly) | Excluded | — | L | Remove payroll round-ups from scope (RCD-011) |
| CAP-23 | Giving pages / community campaigns | Optional: digital-first only | Strong | REQ-X-075 | Covered (draft) | Solicitation-registration exposure (state law) | L | Defer; counsel on CA AB 488 |
| CAP-24 | Donor-facing AI assistant / agent | **Unverified** in market (only Daffy MCP live, no documented limits) | Weak | AIN-*, REQ-X-053..055, REQ-X-051 | Partial (and duplicated IDs) | Single AI-assistance contract; read-only default | H | Differentiator *with controls*; refine REQ-X-051/053 (PLT) |
| CAP-25 | Electronic (ACH/EFT) grant payment | Baseline: Fidelity pays 67% of grants by EFT | Strong | UNB-PAY-*, REQ-X-004/005, UNB-CUST-008 | Partial | Rail selection; partner rail (Chariot, PayPal Grant Payments, own ACH) vs build | H | Partner a rail; refine payment state machine (GRT) |
| CAP-26 | Instant / fast approval or payment | **Unverified**: no sponsor publishes measured latency | Weak | None | Gap (intentional) | Measured service levels | M | Set measurable SLOs instead of "instant" claims (`REQ-X-071`) |
| CAP-27 | Charity-facing portal / nonprofit onboarding | Parity: usually via rail partner | Strong | REQ-X-058, REQ-X-077 | Covered (draft) | Bank-detail verification is covered by REQ-X-005 | M | Use rail partner's onboarding where possible |
| CAP-28 | Anonymous grants / donor-information controls | Regulatory/operational control | Strong | UNB-GRANT-002/025, ADM-013 | Partial | Default and per-grant override precedence | H | GRT refinement |
| CAP-29 | Grant purpose / restricted grants | Control for §4966/§4967 | Moderate | UNB-GRANT-002, REQ-X-069, sponsor affirmations | Partial | Prohibited-benefit affirmation as typed data | H | GRT refinement |
| CAP-30 | Outbound transfer and history portability | Control and differentiator: AEF migration kept only two years of history | Moderate | REQ-X-061, UNB-ACC-011, UNB-LS-015 | Partial | Export completeness, outbound DAF-to-DAF transfer as a grant to an eligible sponsor | H | ENT refinement |
| CAP-31 | SOC 2 / independent assurance | Control: required by institutional buyers; no vendor publishes a scoped report | Weak | SOC2-*, UNB-CTRL-*, REQ-X-073 | Covered (design) | Control catalog quality (keyword-generated links) | H | PLT findings |
| CAP-32 | Sponsor-side approval and eligibility screening | Control | Strong | REQ-X-007, REQ-X-017, UNB-CHAR-*, UNB-RULE-* | Partial (incorrect currency rule) | Grant-date reliance | H | `RR-REQ-X-007-r1` |
| CAP-33 | Idempotent APIs / webhooks / status lifecycle | Control (only Chariot documents it publicly) | Moderate | REQ-X-004, REQ-X-019..022, UNB-API-STD-* | Covered | — | H | Keep |
| CAP-34 | Tax intelligence / planning tools | **Unverified** accuracy | Weak | REQ-X-066 | Covered (draft) | OBBBA nonitemizer exclusion of DAFs | M | Constrain REQ-X-066 (06 §2) |
| CAP-35 | Multi-custody / custodian integration | Optional (wealth channel) | Weak | UNB-CUST-*, REQ-X-062 | Covered | — | M | Keep; one custodian at MVP |
| CAP-36 | Dormancy / minimum-activity policy | Control | Moderate | ADM-024, UNB-WF-015, UNB-LS-015 | Partial | Notice cadence and charitable-default policy | M | ENT refinement |
| CAP-37 | Fund accounting, unitization, reconciliation | Baseline (vendor ledger claims lack delivery evidence) | Weak for vendors | UNB-ACC-*, UNB-NAV-*, REQ-X-022/023 | Covered (over-scoped) | — | H | FIN: challenge ERP scope |
| CAP-38 | Specialty funds (single-charity, recoverable grants, non-501(c)(3) grantees) | Optional | Moderate | UNB-GRANT-023 | Partial | QCD single-charity fund question (counsel Q7) | L | Defer |
| CAP-39 | ESG / impact investment options | Parity | Strong | UNB-INV-014 | Covered | — | M | Keep as menu configuration |
| CAP-40 | SSO / MFA / delegated access | Control | Moderate | GOV-AUTH-*, ADM-005, REQ-X-072, UNB-ADV-001 | Covered | Agent credentials barred by some sponsor terms → scoped OAuth | H | Keep |
| CAP-41 | AI in sponsor-side grant review | **Deliberately exclude** autonomous AI approval; permit AI that adds flags, never clears one | Weak | REQ-X-017, GOV-AUTH-003, AIN-003 | Covered (consistent) | — | H | Keep the exclusion |

### Coverage counts (denominator: 41 capabilities in the research rollup)

| Coverage | Count | Capabilities |
|---|---:|---|
| Covered (including draft-only REQ-X) | 16 | CAP-03, 07, 10, 14, 20, 21, 23, 27, 31, 33, 34, 35, 37, 39, 40, 41 |
| Covered by an alternative (native app need met by mobile web) | 1 | CAP-11 |
| Partial | 20 | CAP-01, 02, 04, 06, 08, 09, 13, 15, 16, 17, 18, 19, 24, 25, 28, 29, 30, 32, 36, 38 |
| Gap | 3 | CAP-05 (intentional), CAP-12 (unintentional), CAP-26 (intentional: replace with measured SLOs) |
| Excluded | 1 | CAP-22 |
| **Total** | **41** | |

Counts are computed from the coverage column above. "Covered" means a requirement exists, not that it is validated: every covering requirement is still unapproved and untested (report 15).

## 3. Recommended deliberate exclusions

| Capability | Why exclude | Decision |
|---|---|---|
| Native mobile apps (CAP-11) | Responsive web meets mobile needs. Native apps carry low ratings at incumbents, duplicate the UI, and conflict with WASM-first. | RCD-011 |
| Consumer payroll funding and round-ups (CAP-22) | No sponsor offers them; corporate micro-giving products closed. | RCD-011 |
| Autonomous AI grant approval (CAP-41) | No market precedent; legal liability sits with the sponsor (§4966/§4967). | Already the corpus position (REQ-X-017) |
| Direct crypto custody (CAP-05) | Cyclical; custody risk is the failure mode seen at SDG Impact Fund. | Accept only via an intermediary that liquidates before transfer |
| Building international grant due diligence in-house (CAP-14) | About 1.9% of dollars; ED/ER law is unsettled for DAFs. | Route via intermediaries until counsel answers Q4 |
| Donor-level impact measurement (CAP-15, impact part) | Demand unproven; data quality depends on charities. | Keep as a hypothesis (UNB-HYP), not a requirement |

## 4. Provider history, relationships and evidence conflicts

Paths in this section are relative to `daf-landscape` unless marked otherwise. IR = `research/DAF-INDUSTRY-REPORT-2026-11.md`.

### 4.1 Failed, acquired and discontinued providers, and lessons

Sources: `data/mission-2026-11-14/failed-daf-startups.csv` (FD01-FD50), `research/mission-2026-11-14/failed-daf-startups-deep-research.md` (FD), `data/technology-provider-exits-and-incidents-2026.csv` (X01-X11), `data/historical-daf-lifecycle-events-2011-2026.csv` (EV01-EV24), IR §11.2.

| Case | Type | Event (date) | Evidence | Lesson (as stated in corpus; [I] = inference) |
|---|---|---|---|---|
| National Heritage Foundation | core DAF sponsor | Ch.11 2009; IRS revocation 2011; listing 2014; FY2022 zero assets, grants to United Charitable (EV02, EV04, EV05, EV09; FD02) | observed-verified (court/IRS/990); final-return flag not verified | Sponsor legal survival risk is distinct from vendor risk; no proof DAF sub-accounts transferred 1:1 |
| SDG Impact Fund | core DAF sponsor | Georgia SoS bar 2026-06/07; AG suit + receivership 2026-09 (FD01) | observed-verified (regulator), allegations unadjudicated | Self-reported crypto/illiquid valuations + commingling are a regulator magnet; custody separation is the product |
| Crown/DonorFirst -> RenPSG | DAF tech | agreement 2016-10-24 (EV06; FD08) | vendor-claim (announcement) | Consolidator roll-up; closing/integration unverified |
| Pinkaloo -> RenPSG | DAF tech (white label, bank pilot) | 2021-03-09 (EV08; FD03) | publicly-documented (founder statement) | Small white-label vendors exit to consolidators |
| Stellar/iPhi -> Ren | DAF platform | 2023-01-20 (EV10; FD07) | publicly-documented | Ren iPhi then lost 6 of 57 sponsors 2023-Nov 2026 (10.5%), mostly CFs to Foundant (IR L433-441) |
| Amicus.io -> TIFIN | DAF tech | 2022-03 (FD04) | publicly-documented (press) | Bank-embedded consumer DAF thesis absorbed into advisor play; buyer valued "pipes" |
| Vennfi -> Foundation Source | DAF tech (Charityvest) | 2024-10-17 (X05, EV12; FD05) | publicly-documented (acquirer) | Sponsor/vendor split let charitable assets stay while code changed owner |
| Giving Place: TIFIN -> Foundation Source | PF/family-office software | 2025-04-21 (X06, EV14; FD06) | publicly-documented | Two owners in 14 months; TIFIN Give DAF continues (not a failure) |
| PG Calc -> Foundation Source | planned giving | 2024 (L06) | publicly-documented | Historic SOC 2 not transferable to new owner's DAF stack |
| Blackbaud DAF (Kintera/Giving Capital) -> Renaissance | DAF platform | 2011-02-08 (EV03) | publicly-documented | Technology sale is not sponsor failure |
| Blackbaud FIMS/DonorCentral -> Fusion/NPact | CF fund accounting/portal | 2022 (~$6.4m) (IR L486) | research corpus [V] | AEF left DonorCentral for NPact (2024) then Ren (2026); drafts >90 days removed; only two years visible; DonorCentral closed 2026-10-31 (EV11, EV20, EV22) |
| Network for Good software -> Bonterra | payments + DAF sponsor | 2022-01-04 (FD09) | publicly-documented | Charity can separate cleanly but may buy services from former affiliate |
| The Giving Block -> Shift4 | crypto intake | 2022 acquisition; $47.1M earnout write-down; ~12% staff cut (FD13) | observed-verified (SEC 10-K) | Crypto is cyclical revenue, not a moat |
| Endaoment (control) | crypto-native DAF | survived; Altruist sponsor-of-record 2026-10 (FD15) | vendor-claim (press) | Survived by moving to partner/advisor distribution |
| Flipcause | payment intermediary | CA AG cease order 2025-11-14; Ch.11 2025-12; Ch.7 2026-04 (X01, EV17-EV21; FD16) | observed-verified (regulator) | Never commingle donor money with operating cash; processor reserves/legal title decide who is paid |
| Ammado | payment intermediary | court-ordered liquidation 2018, ~EUR3.8M unaccounted (FD17) | press | Early warning: major network suspended use a year earlier |
| Giving Assistant | donor app | ceased 2021-11, orderly payout (FD18) | press | Ring-fence liabilities to users/charities |
| Everfund; Kambeo; TakeCharge.io; ioby | fundraising SaaS | closures 2024-11-22; 2026-03-30; 2018-06-22; 2024-09 (X02, X03, EV07; FD19-22) | first-party notices | Export deadlines; volume without margin is not a business |
| GoFundMe Charity; Virgin Money Giving; BT MyDonate; Google One Today; AmazonSmile | corporate giving products | closed 2019-2023 (FD23-27) | press | Borrowed/parent-owned channels vanish when the parent reprioritises; habitual micro-giving not created |
| Benevity Alaya | corporate giving product | planned sunset 2026-12-15 (X04, EV24) | publicly-documented (future) | Do not count as closed yet |
| Classy -> GoFundMe Pro; CyberGrants -> Bonterra | rebrand / acquisition | 2025-07-09; 2021-24 (X07, X08) | publicly-documented | Rebrands are not mortality |
| Schwab Charitable -> DAFgiving360 | sponsor rebrand | 2024-06-18 (EV16) | publicly-documented | No structural/platform change stated |
| DonorPerfect; Blackbaud | incidents | 2026-11-11 outage; 2020 breach -> SEC 2023 / FTC 2024 orders (X09, X10) | observed-verified (status page; federal records) | Test queued-receipt reconciliation, breach escalation, retention |

**Cross-case lessons (FD §6):** (P1) money is lost in the payments/custody layer, not the DAF ledger - all three loss cases had one operator controlling unsegregated funds; (P2) acquisition by a consolidator (Ren, Foundation Source, TIFIN, Blackbaud) is the modal DAF-tech exit, with no codebase/migration outcomes disclosed; (P3) borrowed distribution can be withdrawn; (P4) low-take pricing vs CAC is suggestive only; (P5) regulator scrutiny of intermediaries is rising; (P6) crypto is cyclical; (P7) governance failure in mission-branded entities. **Not supported by evidence:** that code quality, AI, or proposed s4966 regs caused any exit. No venture-backed consumer DAF app shutdown found (likely coverage artefact).

**Implication for requirements:** segregated sponsor-of-record money flows (CAP-25/CAP-37), full-history export and migration (CAP-17/CAP-30), and sponsor-governance due diligence are the controls the failure record actually supports.

### 4.2 Technology and administration relationships (who powers whom)

| Sponsor / program | Technology / administrator | Payment rail | Evidence |
|---|---|---|---|
| Renaissance Charitable Foundation + private labels (Edward Jones, Ameriprise, RBC, LPL/Commonwealth, Northwestern Mutual, UBS (one of four), SEI, Fisher, MDRT) | Ren DFX; Ren administers (RCF paid Ren $19.23m FY2024 = 98% of individual-fundholder admin fees) | Ren Rapid Disbursement Engine (vendor claim) | observed-verified (audited RCF statements; live portals) - IR L70-76; `data/rcf-ren-audited-admin-payments-2020-2024.csv`; Edward Jones back-office by Ren is **inferred only** |
| Morgan Stanley GIFT | Ren DFX portal; RenPSG back-office ($14.93m CY2024 990) | ACH claimed (2025 brochure) vs check (2022) | observed-verified (990, portal) - bank-program-terms rows 22-23, 34, 37 |
| Bank of America Charitable Gift Fund | Ren DonorFirst/DFX portal; Ren mailbox for paper forms | Check only | observed-verified (portal) - rows 51-52; **earlier 10-K claim of Ren relationship contradicted** (IR L75; `research/mission-2026-11-14/b4428-04-bofa-cgf-conflict.md`) |
| Goldman Sachs Philanthropy Fund; GS Gives | Ren/Stellar iPhi; Goldman entities administer | Check (2008) | observed-verified (iPhi footer; 990 Stellar payee) - rows 12-18 |
| American Endowment Foundation | Blackbaud DonorCentral -> NPact (2024) -> Ren (Apr 2026) | Ren charity portal + ACH | publicly-documented (AEF notices) - EV11, EV20, EV22; REL L02 |
| National Philanthropic Trust + 13 private-label sites (J.P. Morgan, UBS, BMO, Glenmede, NPT UK/Transatlantic, Rathbones...) | NPT sponsor/operator; GivingPoint built and hosted on Ren (hosting evidence only, no contract disclosed) | PayPal Grant Payments, wire, check | observed-verified (hosting) - IR L425-431 |
| Fidelity Charitable | FMR affiliates (recordkeeping, systems, admin; fees equal to donor admin fees) | Own EFT via Stripe + ACH; 67% EFT | observed-verified (audited statements/990) - IR L65, L450-451 |
| Vanguard Charitable | Vanguard related-party services ($4.24m FY2026); Give Interactive selected for recordkeeping (vendor release; go-live unobserved); Give Interactive is controlled by The Signatry's trust | PayPal Grant Payments | IR L67-68 |
| DAFgiving360 (Schwab Charitable) | Schwab-affiliated; vendor scope not verified; DAF inside Schwab Mobile | Check documented; DAF Direct; Chariot DAFpay access (conflicting) | IR L65; census |
| GiveClear Foundation (TIFIN Give; AssetMark Charitable, BlueGive, SEI Charitable Fund since 2026-10-28) | TIFIN Give platform (acquired Amicus.io 2022) | Chariot Disbursements (Dec 2025) | publicly-documented (joint announcements) - REL L09-L10; IR L96-97 |
| Charityvest (TIAA Wealth Management DAF, Temple, OneAscent OneGive) | Foundation Source (GTCR) since Vennfi acquisition; BridgeFT multi-custody announced; Charityvest president paid by Foundation Source ($163,944 via Vennfi, 2024) | PayPal -> Chariot (from Aug 2026; in transition) | publicly-documented / observed (990) - REL L03-L08, L16-L21; IR L92-94 |
| Daffy | Own platform; public API/MCP | Chariot Disbursements (Column N.A.) | publicly-documented |
| Endaoment | Own platform; sponsor-of-record for Altruist DAFs (Vanguard announced Altruist acquisition 2026-08-26) | Chariot (vendor statement) | vendor-claim - AVD44-45 |
| Givinga Foundation (Janus Henderson Charitable Investment Accounts) | Givinga Inc (related taxable corp) | not documented | vendor-claim - AVD46; `queue-batch30-everyorg-givinga-goodstack.md` |
| Community foundations (803 mapped where possible) | Foundant CommunitySuite 361 CFs (45%; 32.7% of CF DAF assets); Salesforce 19 (23.7% of assets); GiftingNetwork 2 (CCT, FFTC; 11.5%); Ren/Stellar iPhi 22 (5.9%); akoyaGO 48; NPact Foundation Cloud 14; Blackbaud DonorCentral/NetCommunity 7; DonorSphere/FidTech 3; ledgers incl. Blackbaud FIMS/Financial Edge NXT, NetSuite (SVCF), Sage Intacct | mostly own ACH; Chariot (Saint Paul), Blackbaud REPAY (GWCF), Bill.com (Atlanta) | observed-verified (live login hosts, 990 payees) - `research/mission-2026-11-14/queue-cf-vendor-map.md` L127-146; `community-foundation-rails-portals-2026-v2.csv`. "CSuite" in the corpus = Foundant CommunitySuite |
| Benevity -> American Online Giving Foundation; Blackbaud YourCause -> Blackbaud Giving Fund | workplace platforms with captive DAF charities | processor | REL L11-L12 |
| Infrastructure | Chariot (DAFpay, Disbursements; bank Column N.A.); PayPal Grant Payments / PayPal Giving Fund; Stripe (Fidelity EFT); BridgeFT (custody data); Candid GuideStar Charity Check (embedded in NPT, Foundant, Give Interactive, akoyaGO); Charity Navigator (linked only; no top-20 sponsor embeds ratings); IRS BMF / Pub 78 / Auto-Revocation List / OFAC upstream of every eligibility check; non-cash intermediaries Dechomai/Charitable Solutions, Realty Gift Fund, The Giving Block, Engiven | | IR L99-105; IPD rows; NCG rows |

**Shared single points of failure (IR L107-115):** the RCF-Ren contract; Ren's DFX portal family; Foundation Source behind Charityvest programs; Chariot on Column N.A.; GiveClear as TIFIN Give's sole sponsor; the IRS master file. About 28% of FY2024 DAF assets sit on Ren technology in some form (IR L423).

### 4.3 Weak, contradicted or conflicting evidence

| Topic | Problem | Sources |
|---|---|---|
| Ren SOC 2 type | **Contradicted:** Type 1 (Bain, Jul 2023) -> Type II (AEF, Jul 2025) -> Type 1 (AEF/Ren, Apr 2026); no report or trust page | `data/platform-comparison-2026-11-11.csv` row 22; AVD05; IR L197 |
| All SOC 2 claims (Chariot Type II, TIFIN unspecified) | No independent report inspected for any vendor; none of 13 vendors publishes scoped report | README L185-190; IR L489-490 |
| Fidelity "electronic-only from July 2026" | **Contradicted** - stated by Chariot, Fidelity FAQ says EFT not required | AVD35 (refresh: contradicted) |
| BofA CGF-Ren relationship via 10-K | **Contradicted**; only portal evidence stands | IR L75 |
| DAFgiving360 DAFpay integration | Integrator pages/"access method launched 2026-05-31" vs finding that no sponsor is publicly live on the provider API | `b44n28-01-donor-info-defaults.md` L19; `quadrant-v11-rescore.md` L105 |
| Charityvest payment rail | Chariot says Chariot from Aug 2026; Charityvest guide still says PayPal; completion unconfirmed | IR L101; AVD31 |
| MS GIFT payment rail | 2025 brochure ACH claim vs 2022 check guidelines | bank-program-terms rows 24-26 |
| Ren 'instant approval', 'automatic vetting', '98% Charity Check' | Vendor assertions; 98% is RCF 2023 coverage policy, not auto-approval; no benchmark run | `ren-instant-grant-approval-rcf-policy-clarity-audit-2026-11-12.md` L46-51 |
| Ren daily unitization; Foundation Source DAF OS; Giving Compass integration; Ren crypto DAF; Confluence/TIFIN; GiveClear Direct; Chariot FDX v6; Groundswell 24h | Announced, no public delivery evidence | AVD02, AVD07, AVD08, AVD15, AVD17, AVD23, AVD33, AVD38 |
| TIFIN Louise AI; TIFIN community campaigns; Chariot Gift Processing; DAFpay Donor Accounts | Partial: vendor-only or beta | AVD09, AVD12, AVD21, AVD22 |
| Vanguard Charitable - Give Interactive | Vendor release only; not in Vanguard FY2026 audited statements | IR L68; AVD40 |
| RCF donor-information default | Ren guide (2026-10) says full contact incl. email by default; RCF circular states no default | `b44n28-01-donor-info-defaults.md` L63 |
| DAFgiving360 minimums | $100k professionally managed via snippet conflicts with $50 reading | sponsor-investment-menus row 4; sponsor-fee-schedules row 23 |
| Edward Jones admin by Ren | Inferred only | IR L80-84 |
| NPT GivingPoint on Ren | Hosting evidence only; no contract | IR L425-430 |
| Daffy scale ($2B assets, $600M private stock) | Unaudited vendor claims | FD §6 P3; census DFC (Daffy) |
| Candid "used by all major US DAFs" | Vendor claim | IPD-021 |
| App ratings | Popularity, not quality; Schwab/GoFundMe ratings are app-level | census §3 |
| Census absences | 267 of 476 cells U; ND = not found in 16-25 pages; six sponsor hosts unreadable | `direction-donor-feature-census.md` §0, §5 |
| Stale terms | GS circular 2008; MS GIFT 2024-08; BofA 2024 guidelines | bank-program-terms; census recency_ok=no |
| AI economics model | "AI ranks last in 16/16" is assumption-driven | IR L226 |
| K2D/Chariot "DAFs cause increased giving" | Causal overreach; counterfactual unproven | README L159 |


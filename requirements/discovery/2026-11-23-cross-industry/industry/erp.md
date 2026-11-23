# Cross-industry discovery: ERP, CRM, trust/estate/family office, subscription billing (codes ERP, CRM, TRS)

2026-11-23. Requirements research only. All referenced IDs were checked against `inventory.csv` and the reconciliation data files. Outputs: 48 atlas rows, 26 findings, 20 new requirements, 4 refinements, 27 scenarios, 12 rejections, 20 failure modes, 12 decisions, 3 conflicts.

**Bottom line.** Unbound already covers most of what these industries need: authority (GOV-AUTH-*, AUTH-I*), succession (UNB-SUCC-*, RR-UNB-SUCC-*, UNB-RCN-ENT-004..010), party/capacity separation (ONT-1/ONT-2), retention, and the ERP boundary (RR-UNB-ACC-012-r1, RCD-013). The gaps that matter are narrow places where something is undefined:

- fee period segmentation and minimum fees;
- who pays a fee;
- which payee registry a payment may use;
- detection of split approvals;
- the sender and consent rules for CAN-SPAM and TCPA;
- PII scoped to each capacity;
- precedence when instructions conflict;
- re-checking charitable beneficiaries when succession activates;
- the scope of representatives and of entity holders.

## 1. Independent assessments

### G. ERP

An ERP runs an organization's own money: procure-to-pay, order-to-cash, payroll, budgeting, fixed assets and consolidation. A sponsor needs these for its operating budget, but they are not DAF administration. Fund assets are not the sponsor's operating money and cannot be spent through procurement. RCF-FIN-008 and RCD-013 already recorded this, and it holds from first principles.

| ERP idea | Transfers? | Why, and where the analogy breaks | Disposition |
|---|---|---|---|
| Org hierarchy / legal entities | Yes | The DAF hierarchy encodes legal control (IRC §4966(d)(2)), not management accountability. Cost centers do not transfer. | Covered (ADM-001, UNB-RCN-PLT-001) |
| Delegation-of-authority (DoA) with approval limits | Yes | Per-document limits fail because a grant can legitimately be split across funds (ONT-4). Aggregate per approver × counterparty instead. | UNB-XIN-ERP-005 |
| Single vendor master / accounts payable (AP) | **No** | ERP AP treats payees as interchangeable. In a DAF, the source fund fixes the legal character of the payment. | UNB-XIN-ERP-004 (separate registries plus a cross-match) |
| Accounts receivable (AR) / billing | Partially | A fund deducted for fees never owes anything. Only partner and sponsor invoices, and donor-paid memberships, have a payer. | UNB-XIN-ERP-002; XIC-ERP-001/002 |
| Intercompany | Partially | The operator and the sponsor are unrelated (or related-party) entities, not under common control. | UNB-XIN-ERP-003 |
| Vendor SOC reliance, CUECs | Yes | REQ-X-073 is boilerplate. | RR-REQ-X-073-r1: bridge letter after 3 months; report expired after 12 months; each CUEC mapped to an owner |
| Activity-based costing | Analytics only | Cost must never change how grants are handled. | UNB-XIN-ERP-006 |
| Budgeting, procurement, HR, payroll, fixed assets | No | Not DAF outcomes | XRJ-ERP-001..004; XID-ERP-003 |

**ERP boundary (conservative and explicit)**

| Capability | Native, authoritative | Integration | Excluded |
|---|---|---|---|
| Fund subledger, DAF-program general ledger (GL), posting, close, statements | ✓ | | |
| Fund-level fee engine | ✓ | | |
| Partner/sponsor fee invoices (the only AR) | ✓ | Payment collection via PSP/bank | |
| Operator SaaS billing to tenants | | Operator's own billing system | Never in tenant books |
| Sponsor operating AP, procurement, vendor master | | Journal export plus a vendor EIN/bank extract for the cross-match | Native module |
| Budgeting, cost centers | | Export of actuals | Native module |
| HR, payroll | | SCIM/IdP workforce status (an input to authority) | Native module |
| DoA matrix, approval limits | ✓ | | |
| Critical-vendor assurance register | ✓ (small) | GRC tool optional | Full third-party risk management |
| Contract management | Clause references for enforced terms only | Contract lifecycle tool | Negotiation |
| Queues and tasks | ✓ | | |
| Consolidation and eliminations | DAF-program entities only | Sponsor ERP | |

### H. CRM

CRM maximizes revenue per relationship. Three of its core moves become hazards in a DAF:

- **The unified 360-degree view.** One person can be a donor, a charity officer and a professional advisor at once (ONT-1). Merged views expose PII and break anonymity (INV-14).
- **Segmentation.** Grant history reveals religion, health and politics (affinity segments G01–G18, daf-landscape:data/affinity-daf-segments-and-models-2026.csv).
- **Profitability-driven service.** Grant processing cannot depend on how profitable a fund is.

What does transfer: relationship ownership with coverage (UNB-XIN-CRM-005), attribution (UNB-XIN-CRM-006), consent and suppression (UNB-XIN-CRM-003), journeys and service history.

**Shared party model: both sides**

| | Shared core | Separate records per role |
|---|---|---|
| For | Excess-benefit screening (TVCTL-14) sees that a donor is also a charity officer. Sanctions screening runs once. A death propagates to every capacity (UNB-RCN-ENT-004). No duplicate identity verification. | Privacy by default; simpler access rules |
| Against | Over-merging leaks PII. Staff working in one capacity learn that another capacity exists. | Screening misses links. A death leaves rights active on another record. Duplicate KYC. |

Cases where sharing causes harm:
1. A charity that is also a donor. Its organization-advised fund can recommend grants to itself (affinity segment G05).
2. An advisor who is also a donor. Their own fund appears in their client book, or they draw a fee from it (UNB-RCN-ENT-015).
3. A household view that exposes a spouse's contributions.

**Recommendation (UNB-XIN-CRM-001/002, XID-CRM-003):**
- Keep a shared minimal identity core: PartyId, legal name, screening identifiers and verification state.
- Hold every other attribute on a profile scoped to one capacity, with its own visibility rule.
- Expose cross-capacity links only to deterministic screening and to compliance staff.
- Capture affiliations at onboarding and through annual attestation, so that TVCTL-14 has data to work on.

**Communications law** (sources accessed 2026-11-23):
- **CAN-SPAM** (FTC compliance guide, https://www.ftc.gov/business-guidance/resources/can-spam-act-compliance-guide-business): commercial email must carry a postal address. Opt-outs must be honored within 10 business days, and the opt-out mechanism must keep working for 30 days. Liability cannot be contracted away, but a single designated sender is allowed.
- **TCPA** (47 CFR 64.1200(a)(2), https://www.law.cornell.edu/cfr/text/47/64.1200): autodialed or prerecorded advertising calls and texts need "prior express written consent". A tax-exempt nonprofit caller needs only "prior express consent".

So the applicable rules depend on which entity is the sender. UNB-XIN-CRM-003 adds this. In the sales pipeline, a donor prospect is a draft establishment (ADM-002). The B2B pipeline belongs in the operator's own CRM (XRJ-CRM-001).

### K. Trust, estate and family office

Trust administration rests on four things a DAF does not have: a written instrument, a fiduciary with duties, court oversight, and trust-owned property. In a DAF the sponsor owns the assets, advisors only recommend, succession is a privilege set by sponsor policy, and disputes go to a sponsor decision rather than probate.

What transfers is the discipline of continuity:
- versioned designations and contingent chains;
- triggers gated by evidence;
- deadlock rules (co-fiduciaries map to co-advisors);
- a cy-près-like fallback when a charitable beneficiary fails;
- signatory management for entity and trust holders;
- limits on the scope of a representative (POA), and restoration of rights when incapacity ends.

Sponsor policies differ widely, so all of this must be configuration:
- Successor limits: SVCF caps successors at 3 in aggregate; CCT allows 2 generations; FFTC allows 1 generation by default; NPT and Vanguard allow succession in perpetuity (daf-landscape:research/mission-2026-11-14/b49-03-bequests-legacy-dafs.md:42).
- Unplanned balances: 11 of the 15 sponsors whose default is known send them to the sponsor's own pool (:28).
- Fidelity applies its default after 90 days with no successor response (:178).
- MS GIFT pays the named charities after 5 years with no grants (:58).

Beyond existing coverage, donor continuity needs:
- deterministic precedence for conflicting instructions (RR-ADM-023-r1);
- a beneficiary re-check at activation, with a fallback chain (UNB-XIN-TRS-001);
- a successor response window (UNB-XIN-TRS-005);
- entity and trust holders, where a trustee change is not succession but dissolution of the entity is (UNB-XIN-TRS-003);
- representative scope and restoration of rights (UNB-XIN-TRS-004);
- a non-binding record of legacy intent (UNB-XIN-TRS-002).

The family-office data feed (UNB-XIN-TRS-006) is P3.

### M (partial). Subscription billing

From SaaS billing, proration, minimums and tiering transfer. The analogy breaks in three places:
- A SaaS shortfall becomes a receivable. A DAF fund cannot owe anything, and value cannot flow back to the donor (UNB-RCN-FIN-013).
- A SaaS credit is refunded to the customer. A DAF over-collection is re-credited to the fund.
- A donor-paid membership is a payment for services. It must never be acknowledged as a contribution.

Published schedules confirm marginal tiers. For example, Fidelity charges 0.60% on the first $500k with a $100 minimum: $3,300 on a $600k balance and $6,500 on $2M (daf-landscape:data/daf-admin-fee-scenarios-2026.csv:12,16). The tier method was already declared (UNB-RCN-FIN-005). What was missing, now covered by UNB-XIN-ERP-001/002 and RR-ADM-028-r1:
- period segmentation;
- daily accrual;
- minimum-fee proration;
- capping a shortfall at available funds and recording the rest as a waiver;
- the payer source.

## 2. What the four industries agree on

1. **Authority is relational and time-bound.** Unbound lacked aggregation of approval limits (UNB-XIN-ERP-005) and precedence for conflicting instructions (RR-ADM-023-r1).
2. **One identity, many capacities.** Merging capacities is where harm happens. Partition data by capacity and link capacities only for screening.
3. **Money events are distinct from information events.** Only the fee engine emits authoritative fee events (UNB-XIN-ERP-007).
4. **Configuration over code.** Succession caps, minimum fees, response windows and joint-authority modes are all sponsor settings.

## 3. Workflow redesigns

| Workflow | Conventional | Existing proposal | AI-native redesign | Control, evidence, recovery | Cost / CX |
|---|---|---|---|---|---|
| Quarterly fees | Quarter-end balance × rate; minimums handled manually; waivers on spreadsheets | ADM-028; UNB-RCN-FIN-005 | Daily accrual with deterministic segments. A shortfall is waived automatically with a reason. AI only explains. | Idempotent run key; compensating corrections; recompute equality (SCTRL-032) | No true-ups; disputes explained by segment |
| Approvals over limit | Email; limits per document | GOV-AUTH-001 | Approver × counterparty × time-window check runs inside the authorization transaction and returns the escalation target | Denials logged; a DoA becomes invalid when the holder leaves | Milliseconds |
| Vendor/grantee separation | One vendor master | AP excluded | Two registries; a nightly vendor extract feeds a deterministic cross-match into the related-party queue | Match evidence; reviewer clearance | Rare holds |
| Campaign send | ESP list with a global unsubscribe | Catalog with a consent basis | The catalog carries the purpose and the sender entity. Suppression is checked at send time, keyed by sender. AI flags commercial content in "transactional" templates. | Suppression record per send | No per-send legal review |
| Co-advisor conflict | Phone calls; whoever calls last wins | "Policy resolves" | Typed, scoped instructions; overlap detection; fixed precedence; deadlock timer that opens a sponsor case. AI only summarizes. | Both instructions kept; decider recorded | Faster, explainable |
| Death → successor or default | Paper review over months | SuccessionCase: hold, suspend, activate | Adds a response-window timer, a beneficiary re-check and a fallback chain proposed to the sponsor. AI researches what happened to a dissolved charity. | No distribution before sponsor approval | Fewer stuck funds |
| Advisor leaves firm | The firm reassigns the book | Not covered | A firm/SCIM feed ends advisor-pair grants and fee accrual; the donor consents to the next advisor | Grant-end events | One consent step |

## 4. Automation challenge

| Category | Items |
|---|---|
| Deterministic | Fee math, segmentation and caps; DoA limits; payee cross-match; consent and suppression; segment allowlist; precedence rules; timers; beneficiary re-check |
| AI-assisted, with sign-off | SOC report metadata; contract clause extraction; flagging commercial content; summarizing wishes and disputes; researching dissolved charities; suggesting affiliations |
| Human only | POA scope; account-type classification; whether CAN-SPAM/TCPA applies; legality of referral compensation; waivers; disputes |
| Prohibited for AI | Computing or waiving fees; classifying content as transactional without review; converting intent into restrictions; inferring sensitive segments |

## 5. Business-model applicability

| Requirements | Applies to |
|---|---|
| UNB-XIN-ERP-001, 002, 004, 005; UNB-XIN-CRM-001, 002, 003; UNB-XIN-TRS-001..005 | Universal |
| UNB-XIN-ERP-003 | Most important for white-label, outsourced-admin and embedded; also advisor-distribution and hybrid |
| UNB-XIN-ERP-001, 002 | Highest stakes in high-volume-low-balance (minimum fees, memberships) |
| UNB-XIN-CRM-005 | HNW-family-office, institutional, advisor-distribution, hybrid |
| UNB-XIN-CRM-006, 007 | Advisor-distribution, investment-centric |
| UNB-XIN-TRS-006 | HNW-family-office only |

## 6. Inputs for economics: relationship profitability and lifetime value

Admin fee only, at a constant balance (daf-landscape:data/daf-admin-fee-scenarios-2026.csv:2-17):

| Balance | Fidelity | DAFgiving360 (tier only) | Renaissance donor-directed | Ren/RCF advisor-managed |
|---|---|---|---|---|
| $10k | $100 (the minimum binds; effectively 100 bps) | $60 | $250 | $250 (250 bps) |
| $100k | $600 | $600 | $600 | $650 |
| $600k | $3,300 | $3,300 | $3,300 | $3,600 |
| $2M | $6,500 | $6,500 | $6,500 | $7,500 |

- The Fidelity minimum stops binding at about $16.7k.
- Advisory fees: an advisory layer of 55 bps, waivable (daf-landscape:data/onegive-55bps-fee-illustrations-2026.csv); subadvisory fees of 0–100 bps (daf-landscape:research/daf-economics-and-distribution-2026-11-11.md:42).
- Daffy membership: $0–$480 a year, paid outside the fund (daf-landscape:research/challenger-economics-daffy-chariot-charityvest-2026-11-12.md:9).
- RCF paid Ren $19.23M in 2024, 98% of its admin fees. That is $744 per year-end DAF, and it is **not** average revenue per user (daf-landscape:data/rcf-ren-audited-admin-payments-2020-2024.csv:6).
- Retention data exists only as hypothetical cohorts: gross revenue retention 80–100%, net revenue retention 80–120% (daf-landscape:data/revenue-cohort-sensitivity-2026.csv).

**Lifetime value inputs:**
- fee schedule;
- balance trajectory;
- waivers;
- payer mix;
- succession, which extends the fund's life;
- outbound transfers (UNB-RCN-ENT-007) and dormancy defaults (RR-UNB-WF-015-r1), which end it;
- servicing cost.

No public data on servicing cost exists. UNB-XIN-ERP-006 creates it.

## 7. Inputs for event architecture (UNB-XIN-ERP-007)

| Event | Class | Producer |
|---|---|---|
| FeeScheduleApproved / Superseded | Authoritative financial (configuration) | Fee engine, after DoA approval |
| FeeAccrued, FeeAssessed | Authoritative financial | Fee engine |
| FeePosted | Authoritative financial | Ledger |
| FeeWaived, FeeReversed / Corrected | Authoritative financial | Fee engine |
| PartnerInvoiceIssued, PartnerPaymentApplied, CreditNoteIssued | Authoritative financial | Partner AR |
| DonorServiceFeeCollected | Authoritative financial, recorded outside fund ledgers; never a contribution | Fee engine / PSP adapter |
| DelegationOfAuthorityGranted / Invalidated, ApprovalLimitExceeded | Authoritative authority | Authority registry |
| AffiliationDeclared, SignatoryUpdated, RepresentativeScopeVerified, InstructionConflictDetected / Resolved | Authoritative authority | Party / authorization |
| RelationshipOwnerAssigned, SegmentMembershipChanged, ReferralAttributed, CampaignMessageSent, Consent / OptOutRecorded | Informational. Consent events are legal evidence. | CRM-side |
| Inbound "fee waived" or "balance" messages from a CRM or partner | Not events. They become requests or cases. | — |

## 8. Evidence not obtained

- Actual billing basis and practice for prorating minimum fees.
- Servicing cost by segment, for any sponsor.
- Counsel positions on:
  - whether CAN-SPAM applies to sponsor email;
  - whether donor-paid memberships are deductible;
  - whether POA agents may exercise advisory privileges;
  - whether an organization-advised fund may recommend grants to itself;
  - whether DAF referral compensation makes the referrer a paid solicitor.
- How often charitable beneficiaries fail at activation.
- Whether any sponsor aggregates household balances for fee tiers.
- The TCPA wording was read via Cornell LII because eCFR redirected, so confidence in it is medium.

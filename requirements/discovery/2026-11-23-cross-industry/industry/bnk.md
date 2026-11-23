# Cross-industry discovery: Banking (A), Payments (I), Custody (M partial) — codes BNK, PAY

Status: proposed, unvalidated. 2026-11-23. Outputs: 37 atlas rows, 18 findings, 16 new requirements, 3 refinements, 22 scenarios, 9 rejections, 17 failure modes, 8 decisions, 2 conflicts.

## 0. Existing coverage (not duplicated)

Outbound grant payments are already specified in depth: REQ-X-083 / RR-REQ-X-083-r1 (seven state dimensions plus Blocked), UNB-RCN-GRT-005 (check lifecycle), -006 (blocked property), -008 (Unknown-state protocol), -016 (stop/recall) and -017 (destination evidence). Also covered: TVR-PAY-001..007, REQ-X-004/005, REG-003 (OFAC), OB-32 (BSA does not apply), the exception specification and the resilience specification.

The gaps fall in four areas:
- inbound contribution rails;
- binding rail rules the sponsor carries as Originator, payee, merchant or holder;
- detective bank controls;
- treasury and cost data.

## 1. Independent assessments

### A. Banking

| Bank capability | Disposition |
|---|---|
| Customer identification program (CIP) | Not a legal duty for the sponsor (OB-32). Identity proofing and sanctions screening are already covered by RR-UNB-PART-007-r1 and UNB-RCN-ENT-014. Rejected: XRJ-BNK-001. |
| Customer due diligence (CDD) | The direction is reversed. The sponsor is the bank's nonprofit legal-entity customer, and only the control prong applies (31 CFR 1010.230(e)(3)(ii)). FinCEN order FIN-2026-R001 (2026-02-13) removed re-verification at each new account. The sponsor needs only a register of signers and control persons (XCAP-BNK-002). |
| Suspicious activity reports (SAR) | No duty. OFAC blocking and reporting is binding and already covered. A "referral" case outcome replaces SAR filing (XRJ-BNK-002). |
| ACH | The sponsor is a non-consumer Originator: CCD credits for grants; PPD, WEB and CCD debits for contributions. Its duties bind through the bank agreement (ODFI). Gaps: fraud monitoring (UNB-XIN-BNK-002), account validation for WEB debits (UNB-XIN-PAY-002), return rates and reinitiation (UNB-XIN-PAY-005). |
| Wire | Under UCC 4A-211, cancelling an accepted wire needs the receiving bank's agreement. A beneficiary bank may cancel only unauthorized, duplicate or mistaken orders. A recall is therefore only a request, marked RequestOnly in the rail matrix (UNB-XIN-BNK-004). |
| RTP / FedNow | Irrevocable. The FedNow network limit has been $10m since 2025-11-12. Not eligible for grants by default (XID-BNK-004, XRJ-BNK-004). |
| Checks / positive pay | FinCEN alert FIN-2023-Alert003 covers mail-theft check fraud. Treasury reports checks are 16 times more likely than electronic payments to be lost, stolen or altered. Adds a gate on the bank issue file plus payee-name positive pay (UNB-XIN-BNK-006). |
| Escheat | The sponsor is the holder of outstanding checks only. Fund balances are excluded unless counsel advises otherwise (RR-UNB-WF-015-r1). States vary: Virginia has a business-to-business exemption, New York has none. Adds a per-state rule table (UNB-XIN-BNK-007) and question XID-BNK-003. |
| Reconciliation | Gap: daily matching of every bank debit to an instruction (UNB-XIN-BNK-003). |
| Dual control, limits, holds | Covered (UNB-RCN-PLT-005, REQ-X-020, UNB-RCN-PLT-003). |
| Fraud monitoring | Binding under the 2026 Nacha rules (UNB-XIN-BNK-002). |
| Cash forecasting | Grant outflows are scheduled, so a deterministic daily cash ladder is enough (UNB-XIN-BNK-009, P2). |

**Challenge.** A sponsor is a bank customer, not a bank, so most banking workflows do not transfer. These control objectives do:
- money leaves only on authority;
- destinations are verified;
- every debit is explained;
- reversals are anticipated;
- rail rules are met.

The first two are already covered by preventive controls. The cheapest additions are detective (daily debit matching) and evidentiary (a written fraud program and a register of rail obligations).

### I. Payments by transaction class

| Function | Contributions | Grants | Admin fees | Investment transactions |
|---|---|---|---|---|
| Lifecycle | Received → Provisional → Final; disputes; suspense | REQ-X-083 | Internal journal, no rail | Custodian, T+1 settlement |
| Refunds | Only ReturnOfUnacceptedProperty before acceptance (UNB-RCN-FIN-013). Chargebacks are involuntary (XIC-BNK-001). | Compensating credit (UNB-RCN-GRT-015) | Reversal journal | Custodian cancel or correct |
| Retries | Never reinitiate an unauthorized return. A retry must be the same amount and carry "RETRY PYMT". No dunning (XRJ-BNK-003). | Unknown is not Failed (REQ-X-004) | — | Settlement fail |
| Recurring | Reg E §1005.10(b) (UNB-XIN-PAY-001) | UNB-RCN-GRT-004 | Fee schedule | Rebalancing |
| Disputes | Missing (UNB-XIN-PAY-003) | UNB-RCN-GRT-008 | Case | — |
| Fraud | Card testing (UNB-XIN-PAY-004); WEB debit account validation (UNB-XIN-PAY-002) | Business email compromise (UNB-XIN-BNK-002) | — | Custodian movement matching |
| Reconciliation | Processor three-way match (UNB-XIN-BNK-008) | Covered | Journal | Covered |
| Fees | Processor fee per gift; donor-covered fees (UNB-XIN-PAY-006) | Rail fees | FeeBase (UNB-RCN-FIN-005) | Commissions |

### M. Custody

- **ACATS is the wrong mechanism for gifts.** ACATS (FINRA Rule 11870) moves a customer's own account between brokers, with an exception for mismatched account titles. A gift changes the owner, so it moves by non-ACATS DTC delivery. Product copy must not call a gift an "ACATS transfer".
- **Gift deliveries often arrive without the donor's identity.** Deterministic matching on the donor's declared intent (CUSIP, quantity, delivering DTC participant) is preferred over an AI suggestion (UNB-XIN-PAY-007, RR-UNB-GAP-013-r1).
- **T+1 settlement** (compliance date 2024-05-28) feeds the cash ladder.

## 2. What transfers and what does not

**Transfers:**
- Originator duties under the Nacha rules;
- payee duties under Reg E;
- positive pay;
- daily completeness of bank debits;
- merchant dispute handling;
- defense against card testing;
- gross-to-net reconciliation.

**Does not transfer:**
- CIP, CDD, SAR and CTR, because the sponsor is not a BSA financial institution.
- Reg E error resolution and Reg CC. These bind banks; for the sponsor they matter only as return and dispute windows to model.
- Subscription dunning. Donors owe nothing, and retries raise the unauthorized-return rate.
- Contesting chargebacks by default. A charity delivers no goods, and contesting harms trust.
- Bank-style liquidity forecasting.
- Instant rails by default. Speed combined with irrevocability amplifies business email compromise.

## 3. Workflow redesigns

| Workflow | Conventional / existing | Redesign | Effect |
|---|---|---|---|
| Out-of-band payments | Month-end bank reconciliation; TVR-PAY-004 gate | UNB-XIN-BNK-003: every bank debit matched daily. An unmatched debit opens an incident and places a hold. A missing feed is recorded as Unverified, never as zero activity. ACH debit blocks attested quarterly. | Detection in under a day instead of about 30 days |
| ACH fraud program | Policy PDF | UNB-XIN-BNK-002: the existing UNB-RCN-GRT-017 rules are declared as the Nacha program. Alerts are tagged by fraud type. The annual review is recorded, and origination is blocked once it is overdue. AI only ranks alerts. | Low cost; reuses existing controls |
| Reversibility | A single R10 window | RR-TVR-CON-001-r1: a ReversalWindow table keyed by rail, SEC code, account type and direction. A missing row fails closed as Provisional. | Wires and business debits become available in days; cards stay protected for 120 days |
| Card disputes | None | UNB-XIN-PAY-003: two-phase lifecycle tied to fund exposure and to correction of the acknowledgment. A human decides whether to contest above a threshold. | Small disputes need no human touch |
| Securities intake | Manual matching or AI suggestion | UNB-XIN-PAY-007: deterministic intent match; anything else goes to suspense | High straight-through processing |
| Rail choice | Implicit | UNB-XIN-BNK-004: one versioned matrix of payment class × rail × direction × irreversibility | Removes ambiguity |
| Escheat | Annual scramble | UNB-XIN-BNK-007: ACH by default, outreach before a check goes stale, per-state rule table | Volume falls as the ACH share rises |

**AI boundaries.** Every new control is deterministic. AI may:
- rank fraud alerts;
- draft dispute evidence packets;
- explain reconciliation breaks;
- suggest suspense matches.

AI never:
- clears an alert;
- accepts a dispute;
- links an ambiguous delivery;
- classifies an unmatched debit.

## 4. Applicability by sponsor model

| Requirements | Applies to |
|---|---|
| UNB-XIN-BNK-001, 002, 003, 004, 006, 007; UNB-XIN-PAY-007; RR-UNB-PAY-008-r1; RR-UNB-GAP-013-r1 | Universal |
| UNB-XIN-PAY-001, 002, 005 (where debits are accepted) | D2C, white-label, advisor-distribution, embedded, high-volume-low-balance and hybrid; optional elsewhere |
| UNB-XIN-PAY-003, UNB-XIN-PAY-004, UNB-XIN-BNK-008 (where cards are accepted) | D2C, white-label, embedded, high-volume-low-balance |
| UNB-XIN-BNK-009 (cash ladder) | Institutional, investment-centric, white-label, outsourced-admin, HNW-family-office, high-volume-low-balance, hybrid |

## 5. Cost-to-serve inputs

Payment and cash events emit CostDriver facts (UNB-XIN-BNK-005). Each fact records: sponsor, payment class, rail, provider, external fee and its source (actual, schedule or illustrative), physical items, manual touches and minutes, and exception type. One fact is emitted per attempt, void, reissue, return, dispute or exception case. These facts never post to the ledger.

| Rail / event | Sourced | Illustrative (unsourced) |
|---|---|---|
| ACH credit (grant) | — | $0.10–0.50 per entry; $5–25 per destination verification |
| ACH debit (contribution) | — | $0.10–0.50 per entry; $0.10–1.00 per validation; $2–5 plus 10–20 min per return |
| Wire | — | $15–35; 15–30 min of callback labour |
| Check (grant) | Benevity manual check fee: the higher of $35 or 8% (daf-landscape:research/mission-2026-11-14/queue-npt-dafgiving360-misc.md:189) | $2–8 per check; reissue $25+ |
| Check (contribution, lockbox) | Sunwest: $0.65 per check, $0.25 per scanline item, $100 per month | — |
| Card | PayPal Charitable Giving Fund: 1.58% of grants (CY2024). Blackbaud Giving Fund: card fees 0.30% plus bank/merchant fees 0.18% (CY2025). | 2–3.5% of the gift; $15–25 per dispute |
| DTC securities | — | $0–25 per receipt; 15–60 min per suspense item |
| Exception | Cost per resolved case is a metric in the exception specification | Tier 1 ≈ $0; human handling $20–80 |
| Fraud loss | Givinga $316,184; business email compromise $500,845 net and $524,983; ACH redirection $565,787 (daf-landscape vendor profiles) | Model as a reserve, not a per-transaction cost |

**Steering lever.** Making ACH the default for grantees removes check fraud, escheat and reissue costs. Seattle Foundation made ACH mandatory from 2026-06-30.

## 6. Evidence not obtained

- Full Nacha rules text: reinitiation count and window, how long authorizations must be retained, NOC timing.
- Visa and Mastercard rule texts.
- Per-state unclaimed-property statutes for checks payable to charities.
- RTP and FedNow request-for-return mechanics.
- Reg CC, FDIC and PCI requirements (stated from general knowledge).
- Availability of BAI2 and camt.053 statement feeds.
- The text of FIN-2026-R001 itself (read via FinCEN's listing and law-firm summaries).
- Actual unit costs.

## Sources read (accessed 2026-11-23)

Fetched:
- https://www.nacha.org/rules/risk-management-topics-fraud-monitoring-phase-2
- https://www.nacha.org/rules/ach-network-risk-and-enforcement-topics
- https://www.consumerfinance.gov/rules-policy/regulations/1005/10/
- https://www.law.cornell.edu/cfr/text/31/1010.230
- https://fincen.gov/resources/statutes-and-regulations/cdd-final-rule
- https://www.law.cornell.edu/ucc/4A/4A-211
- https://www.finra.org/rules-guidance/rulebooks/finra-rules/11870

Search summaries only (lower confidence):
- Nacha WEB-debit and R10 pages
- Federal Reserve FedNow limit notice
- Visa dispute FAQ
- FinCEN check-fraud alert
- SEC T+1 FAQ
- DLA Piper on FIN-2026-R001
- VSCPA and NY OSC unclaimed-property pages

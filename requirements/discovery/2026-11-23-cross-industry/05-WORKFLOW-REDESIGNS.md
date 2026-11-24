# 05 — Cross-industry workflow redesigns (deliverable 5)

Each redesign follows mission §6 and PRINCIPLES.md. For every workflow it states:
- the required outcome and the constraints that bind it;
- which traditional steps are only convention;
- the lowest-friction valid process;
- what is fully automated, and what needs human judgment or legal authority;
- how exceptions, evidence, recovery and correction work;
- cost, latency and customer experience.

Each one is compared three ways: **conventional** (how the analogous industry does it), **existing proposed** (the Unbound baseline) and **AI-native redesigned**.

AI is used only where deterministic rules are weaker. Every redesign below keeps authorization, posting, release, clearance and legal discretion deterministic or human. The full tables, with evidence, are in the per-industry reports in [`industry/`](industry/).

## 1. Redesigned workflows

| # | Workflow | Conventional | Existing Unbound | AI-native redesign | Fully automated | Human / legal authority | Requirements |
|---|---|---|---|---|---|---|---|
| 1 | Out-of-band payment detection | Month-end bank reconciliation | Preventive gate only (TVR-PAY-004) | Match every bank debit daily to an authorized instruction or an expected-debit class. An unmatched debit opens an incident and places a hold. A missing feed means *Unverified*, never zero activity. | Matching, holds, completeness | Incident disposition | UNB-XIN-BNK-003 |
| 2 | ACH fraud program (Nacha 2026) | Policy PDF | Destination controls (UNB-RCN-GRT-017) | Declare the existing destination controls as the documented program. Alerts are tagged by fraud type. Origination is blocked when the annual review is overdue. AI only ranks alerts. | Tagging, review clock, block | Clearing alerts | UNB-XIN-BNK-002, RR-UNB-PAY-008-r1 |
| 3 | Contribution reversibility | One R10 window | TVR-CON-001 | A ReversalWindow table keyed by rail, SEC code, account type and direction. A missing row fails closed as *Provisional*. | All | Table approval | RR-TVR-CON-001-r1 |
| 4 | Card disputes | None | — | Two-phase lifecycle tied to fund exposure and acknowledgment correction. A human decides whether to contest above a threshold. | Below threshold | Contest decision | UNB-XIN-PAY-003 |
| 5 | Securities gift intake | Manual match | AI suggestion (UNB-GAP-013) | Deterministic match on declared intent (CUSIP, quantity, DTC participant). Anything unmatched goes to suspense. AI may only suggest. | Matching | Ambiguous links | UNB-XIN-PAY-007, RR-UNB-GAP-013-r1 |
| 6 | Quarterly admin fees | Quarter-end balance × rate; spreadsheet waivers | ADM-028; UNB-RCN-FIN-005 | Daily accrual with deterministic segments at open, close, transfer and schedule change. A minimum-fee shortfall is capped at available funds and waived with a reason, never turned into a receivable. | All | Policy waivers | UNB-XIN-ERP-001, -002, RR-ADM-028-r1 |
| 7 | Approvals over limit | Per-document limits, email | GOV-AUTH-001 | Cumulative limit per approver × counterparty × window, checked inside the authorization transaction. | All | Escalated approval | UNB-XIN-ERP-005 |
| 8 | Co-advisor conflicting instructions | Last caller wins | "Policy resolves" | Typed, scoped instructions with overlap detection and fixed precedence. A deadlock timer opens a sponsor case. AI only summarizes. | Detection, precedence | Deadlock decision | RR-ADM-023-r1, UNB-XIN-EVA-012 |
| 9 | Death → successor or default | Paper review over months | SuccessionCase (UNB-SUCC-*) | Adds a response-window timer and re-checks charitable beneficiaries at activation. A fallback chain is proposed to the sponsor. AI researches dissolved charities. | Timers, re-checks | Sponsor activation | UNB-XIN-TRS-001, -005 |
| 10 | Grant status enquiry | Call and look-up | Proactive events and IVR | Reason-coded proactive notice and a portal timeline. AI answers only by citing a state field or clause; otherwise a human answers. | Notices, cited answers | On request | UNB-XIN-SVC-001, -007, UNB-XIN-CLM-002 |
| 11 | Reconsideration of a decline | Written appeal | Case with owner | Reason-coded notice, filing window and independent fresh review under current policy. The original decision record is never edited (child record). | Clocks, routing | Independent reviewer | UNB-XIN-CLM-001, -002 |
| 12 | Evidence collection | Emailed checklist, repeated asks | WaitingForEvidence | Typed InformationRequest, checked against evidence already held. The decision clock pauses while waiting. Reminders run, and expiry is handled. | All but the decision | Decision | UNB-XIN-CLM-003 |
| 13 | Complaint handling | Only labelled complaints logged | WF-09 | AI flags dissatisfaction and staff confirm. Default clocks of 2 and 15 business days. Closure requires a response and a root cause, which feed REQ-X-042. | Flagging, clocks | Classification, closure | UNB-XIN-SVC-005 |
| 14 | Foreign grant routing | Officer chooses ED or ER | Officer-driven | Deterministic router: listed charity → intermediary → ED → ER → decline. ED expiry is computed and renewals are queued. | Routing, expiry | ED advice by a qualified practitioner | UNB-XIN-NPG-001, -002, RR-UNB-GRANT-041-r1 |
| 15 | Grant terms | Agreement on every grant | Agreement on every grant (UNB-GRANT-024) | Three tiers. Only expenditure-responsibility grants block release until signed (§53.4945-5(b)(3)). | Tier selection | Counsel-designed templates | RR-UNB-GRANT-024-r1, -026-r1 |
| 16 | Charity organizational change | Phone the grantee | Revocation only | Change-effect table: benign changes continue; a successor organization needs approval; reinstatement releases held grants. | Table application | Successor approval | UNB-XIN-NPG-005 |
| 17 | Violation found after payment | Counsel memo | Preventive controls only | ExciseTaxIncident case with Form 4720, §4958 correction and board-reporting inputs. AI may flag but never decide. | Case creation, data gathering | Counsel, board | UNB-XIN-NPG-008 |
| 18 | Year-end grant recognition | Manual accrual | Open decision OD-19 | One sponsor-set recognition point with evidence checks and an automatic cutoff register. | All | CPA-approved policy | UNB-XIN-ACC-002 |
| 19 | Advisor fee from DAF assets | Billing file, then debit | Fee cap only | Recompute each period from the mandate and the valuation snapshot, then match to the debit. A mismatch triggers a hold and a refund request. | All | Exceptions | UNB-XIN-INV-001 |
| 20 | Event → journal → report | Subledger interface plus reconciliation | Rebuildable projections | Persisted journals keyed by (event, book). Re-derivation is a comparison control only. Every report pins its snapshot. | All | Change classification | UNB-XIN-ACC-003, -004, UNB-XIN-EVA-001, -003, RR-ACC-MDL-007-r1 |
| 21 | Form 990 Schedule D, I and F | Preparer rebuilds from GL exports | Package from the locked ledger | Bridge with typed reconciling items. Schedule I/F attributes are captured at grant approval. An unexplained difference blocks sign-off. | Bridge | Sign-off | UNB-XIN-ACC-006, UNB-XIN-NPG-004 |
| 22 | Partner exit with funds | Vendor migrates accounts | Single-donor transfers only | PartnerExit case. Each fund elects. Each transfer is an ordinary transfer grant with sponsor discretion. Funds that do not elect stay. | Orchestration | Sponsor approval (batch policy allowed) | UNB-XIN-SAS-007, UNB-XIN-EVA-009 |
| 23 | Records disposition | Per-record review | RR-REQ-X-036-r1 (per record) | Batches by class, trigger and period, with one independent approval and a per-record hold re-check at execution. | Batching, re-checks | Approval | RR-REQ-X-038-r1, UNB-XIN-REC-001 |
| 24 | Cost reduction | Occasional initiative | REQ-X-042 loop without cost data | Closed loop: measure → attribute → detect → hypothesize → experiment → independently verify → ratchet, with counter-metric floors. | Measurement, detection | Targets, experiments, ratchet | UNB-XIN-ECO-010, -015, -016 |
| 25 | December staffing | Annual average FTE | economic-model.md | 13-week and December forecast with backlog simulation and staffing per SLA. Forecast accuracy is tracked. | Forecast | Staffing decisions | UNB-XIN-ECO-017 |
| 26 | ACH return after settlement | Spreadsheet match | Return event (TX-15) | Business-key-plus-hash match. A stale "paid" notice is suppressed at send time. Reissue only after the destination is re-verified. | All | Destination changes | UNB-XIN-EVA-005, -015 |

## 2. Steps removed or simplified because they are only convention

| Step | Removed or simplified | Basis |
|---|---|---|
| Signed agreement and receipt on every grant | Required only for expenditure responsibility; acknowledgments optional | §4966(c)(2)(A) excepts distributions to §170(b)(1)(A) organizations; the sponsor claims no deduction. Counsel confirmation pending (XID-NPG-006). |
| Donor CIP/CDD and SAR modules | Rejected | The sponsor is not a BSA financial institution. The control prong applies only as the bank's customer (XRJ-BNK-001, -002). |
| Dunning on failed contributions | Rejected | Donors owe nothing, and retries raise unauthorized-return rates (XRJ-BNK-003). |
| Contesting chargebacks by default | Human decision above a threshold only | No goods were delivered, and contesting erodes trust. |
| Per-account GIPS performance reporting, tax lots everywhere | Pool time-weighted return vs benchmark; lots only where Form 8282, §4943 or UBTI need them | XRJ-INV-002, -004, -005 |
| Order management system | Monitor drift only; the outsourced CIO trades | XIF-INV-011 |
| Erlang-C workforce machinery, voice biometrics | Not adopted at DAF contact volumes (≈0.3 contacts per account per year, an assumption) | industry/svc.md |
| Workflow engine (BPMN or durable execution) | In-database state machines, outbox and timer table | XRJ-WFA-001, XID-WFA-001 |
| Message broker | Gated on measured thresholds | UNB-XIN-EVA-013 |

## 3. Correction and recovery patterns shared by every redesign

- **No blind retries.** An *Unknown* external outcome is never retried blindly (REQ-X-004, INV-04).
- **Additive corrections.** A correction is a linked reversal and re-post. A late item effective in a closed period becomes a prior-period adjustment (UNB-XIN-EVA-002).
- **Issued artifacts are amended, not replaced.** The correction-impact registry lists every affected statement, acknowledgment and filing (RR-UNB-DATA-005-r1).
- **Recovery is human-visible.** Stalled process managers go to a human-resolution queue (UNB-XIN-EVA-007). Timers fire exactly once after a restore (UNB-XIN-WFA-001).

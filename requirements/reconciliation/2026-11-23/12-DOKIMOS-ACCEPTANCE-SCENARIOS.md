# 12 — Dokimos-ready acceptance scenarios

Date: 2026-11-23 | Machine-readable: [`data/scenarios.csv`](data/scenarios.csv) (`scenario_id, title, requirement_ids, preconditions, stimulus, expected_states, financial_invariants, audit_records, recovery_behavior, mock_data_notes`). **None has been executed.**

## 1. Execution path (verified constraints)

Dokimos 0.11.1 has **no scenario or BDD runner**. It verifies applications against Praxis verification manifests using Playwright results, and it records quality evidence (report 13). The scenarios are therefore written to be runner-neutral:

1. **Financial and state-machine scenarios** run as F# tests (for example Expecto with FsCheck property tests) against the domain components, using the synthetic data and mock providers called for by the existing Strata mock-data interaction requirements (`kemiller2002/strata` `docs/requirements/data-interaction-virtualization.md`, still *proposed*). No separate simulation framework is introduced (DG-10).
2. **UI scenarios** map to `praxis.requirement-catalog/1` types (`state-transition`, `workflow-outcome`, `accessibility`, `data-contract`) and are verified by Dokimos from Playwright runs.
3. **Dokimos records** the outcome, the inputs (synthetic data version, policy version, clock) and the evidence for each run (`completion.verify`, `evidence.provenance-record`).

Every scenario states the expected state-machine states using the repository's state names where they exist (grant, reservation, payment attempt, contribution), the financial invariants that must hold, the audit records that must exist, and the recovery behaviour.

## 2. Mission-required adversarial coverage

| Mission scenario | Scenarios |
|---|---|
| Duplicate contributions | DKS-REG-006, DKS-FIN-020 |
| Partial securities transfers | DKS-REG-006 |
| Grant approval during policy changes | DKS-XD-013 |
| Charity eligibility expiration during processing | DKS-REG-001, DKS-XD-010 |
| Insufficient liquidity | DKS-XD-005, DKS-FIN-009, DKS-FIN-010, DKS-FIN-011, DKS-FIN-022 |
| Recurring grant execution during an outage | DKS-GRT-013, DKS-GRT-011, DKS-GRT-012 |
| Duplicate payment callbacks | DKS-GRT-019, DKS-GRT-020, DKS-GRT-017 |
| Check cancellation, reissue and uncashed checks | DKS-GRT-022, DKS-GRT-023 |
| Reconciliation discrepancies | DKS-FIN-014, DKS-FIN-015, DKS-FIN-013, DKS-FIN-023 |
| Advisor permission revocation | DKS-ENT-023, DKS-PLT-004, DKS-GRT-011, DKS-XD-001 |
| Sponsor migration | DKS-ENT-018, DKS-PLT-007 |
| Cross-tenant access attempts | DKS-PLT-003, DKS-FIN-024, DKS-XD-006, DKS-XD-007 |
| AI-generated incorrect recommendations | DKS-PLT-008, DKS-PLT-009, DKS-ENT-002, DKS-XD-008 |
| External API schema changes | DKS-ARC-002 |
| Partial workflow failure | DKS-PLT-006, DKS-GRT-013, DKS-FIN-010 |
| Delayed or contradictory third-party data | DKS-FIN-006, DKS-FIN-013, DKS-GRT-017, DKS-ENT-008, DKS-XD-007 |
| Sanctions true match | DKS-REG-003, DKS-XD-009 |
| Related-party payee | DKS-GRT-007 |
| Refund of accepted assets | DKS-FIN-016 |
| Persistence integrity | DKS-ARC-001, DKS-FIN-019 |

Each ID above was checked against the scenario titles and stimuli in `data/scenarios.csv`. `tools/reqaudit/validate.py` (gate G11) reports any refinement or gap requirement with no scenario.

## 3. Counts

<!-- BEGIN GENERATED: scenario-counts -->
| Domain | Scenarios |
|---|---|
| ARC | 3 |
| ENT | 24 |
| FIN | 24 |
| GRT | 27 |
| PLT | 27 |
| REG | 6 |
| UI | 20 |
| XD | 13 |
| **Total** | 144 |
<!-- END GENERATED: scenario-counts -->

## 4. Scenarios

<!-- BEGIN GENERATED: scenarios -->
| ID | Scenario | Requirements | Stimulus | Expected states | Financial invariants | Recovery |
|---|---|---|---|---|---|---|
| DKS-ENT-001 | Unverified death notice then credential use | UNB-SUCC-006;UNB-SUCC-008;RR-UNB-SUCC-008-r1 | Third party reports A's death without documents; A's credential then submits a $5,000 recommendation | SuccessionCase=NoticeReceived; ProtectiveHold active for A-derived rights; recommendation rejected AUTH_SUBJECT_HELD; B unaffected | No reservation created; capacity unchanged | If the notice is false, append ProtectiveHoldReleased |
| DKS-ENT-002 | Verified death with delegated AI agent | UNB-SUCC-008;UNB-WF-011;GOV-AUTH-003 | Death verified; P submits a recommendation 1s later | DerivedRightsSuspended precedes P request; request denied | No reservation | None needed |
| DKS-ENT-003 | Approved unpaid grant at death with unset policy (amended 2026-11-23 per RCD-039) | REQ-X-008;RR-REQ-X-008-r1;OD-03 | Death verified | G=Completes per default (RCD-022/RCD-039); HeldForReview only when sponsor policy = hold_for_review | Reservation retained; no payment | Sponsor sets policy; G re-evaluated |
| DKS-ENT-004 | Recurring instance after death | REQ-X-008;INV-13 | Scheduler fires | Instance not created or Held; template Suspended | No reservation | Successor re-affirms to resume |
| DKS-ENT-005 | Five-advisor fund, one dies | UNB-PART-001;REQ-X-008 | Death verified; advisor 1 submits a recommendation | Advisor 1 accepted; advisor 3 rights suspended | Normal reservation for advisor 1 grant | n/a |
| DKS-ENT-006 | 50/30/20 split with odd cent | UNB-SUCC-004;RR-UNB-SUCC-004-r1 | PostSuccessorAllocation | Successors S1=50,000.01, S2=30,000.00, S3=20,000.00; origin fund closed | Sum credits = origin debit; journal balanced | Retry produces the same journal id (idempotent) |
| DKS-ENT-007 | Split with share below minimum | UNB-SUCC-004 | Posting requested | SplitHeld; no journal | No balance change | Sponsor amends or approves an exception |
| DKS-ENT-008 | Conflicting successor claims | UNB-SUCC-007;SIM-005 | Simultaneous activation requests | SuccessionCase=Disputed; no activation | No allocation | Counsel decision appended |
| DKS-ENT-009 | Dormant fund disposition without outreach evidence | UNB-WF-015;RR-UNB-WF-015-r1 | Disposition proposed | Rejected; remains Unresponsive | No grant | Complete outreach |
| DKS-ENT-010 | Dormancy reactivation race | UNB-WF-015 | Donor affirms activity while a disposition grant awaits approval | Fund Active; disposition grant cancelled before approval | Reservation released | n/a |
| DKS-ENT-011 | Escheat attempt on fund balance | ADM-024;CTL-044 | Job selects the fund balance | Rejected: only payment instruments eligible | No movement | n/a |
| DKS-ENT-012 | Fund closure with outstanding check | UNB-RCN-ENT-006;ADM-025 | Closure requested | Closure blocked or Closed with outstanding-instrument liability per policy; never deleted | Outstanding liability equals check amount | Check presented later: liability cleared |
| DKS-ENT-013 | Trailing receipt after closure | OD-11;UNB-RCN-ENT-006 | Dividend $12.34 arrives | Routed per trailing_receipt_destination; if unset, suspense with case | Suspense balances | Sponsor decides destination |
| DKS-ENT-014 | Outbound transfer to ineligible receiving org | UNB-WF-014;RR-UNB-WF-014-r1 | RequestTransferOut(all) | Grant held at eligibility; fund remains Active | No reservation release | Donor chooses another sponsor |
| DKS-ENT-015 | Transfer-out returned by receiving sponsor | UNB-WF-014 | Receiving sponsor returns funds | Payment=Returned; fund reopened Active | Return journal restores capacity | Donor re-recommends |
| DKS-ENT-016 | Identity failure after contribution | UNB-PART-007;TVF-113 | Grant recommendation | Recommendation rejected; capacity not grantable; case opened | Contribution recorded; no grant | Sponsor applies failed_identity_contribution_policy |
| DKS-ENT-017 | KYC regime not applicable | UNB-PART-007 | Onboarding | No customer-identification data requested; role activates on (a)+(b) Pass | n/a | n/a |
| DKS-ENT-018 | Migration imported payee | REQ-X-061;INV-07 | Grant release | Payment blocked PAYEE_UNVERIFIED | No payment | Payee verification completes |
| DKS-ENT-019 | Legacy write after fence | REQ-X-061;INV-07 | Legacy system posts a grant | Incident; Unbound posting paused for affected funds | Tie-out break recorded | Reconcile and compensate under sponsor approval |
| DKS-ENT-020 | Advisor fee payable to donor's wealth manager | REQ-X-062;UNB-RCN-ENT-015 | Activate schedule | Blocked pending counsel rule | No accrual | Counsel rule recorded |
| DKS-ENT-021 | Canary grant in Schedule D package | REQ-X-060 | Build package | Canaries excluded; totals reconcile to ledger minus canaries | n/a | n/a |
| DKS-ENT-022 | White-label sponsor disclosure suppressed | INV-18;UNB-EXP-002 | Render donor portal | Mandatory legal-sponsor disclosure still present | n/a | Theme rejected |
| DKS-ENT-023 | Revoked co-advisor pending recommendation | UNB-ADV-001 | Donor revokes B | Recommendation Held; B denied | Reservation none or released | Donor re-affirms |
| DKS-ENT-024 | Phone-assisted payee change | UNB-RCN-ENT-016 | Staff enters contact/payee change | Pending until verified-channel confirmation | No payment destination change | Expires if unconfirmed |
| DKS-FIN-001 | Cross-fund cash-only transfer rejected | UNB-ACC-002;ACC-MDL-003 | PostJournal Dr 1030[S] 5000 / Cr 1030[O] 5000 | Journal Rejected(Unbalanced fund S +5000, fund O -5000); no posted rows | Per-fund trial balance zero; 3900 nets to zero | Resubmit with 3900 transfer lines; posts once |
| DKS-FIN-002 | ACH contribution funds grant then returns R10 | REQ-X-009;UNB-ACC-006;FI-23 | Return code R10 arrives day 45 | Contribution ReturnedOrReversed exposure; grant record preserved; new reservations blocked; acknowledgment correction task if issued | Capacity reflects -$800 exposure; no cash erased | Case to Controller; loss-bearer per OD-20 |
| DKS-FIN-003 | Acknowledgment template missing exclusive legal control | UNB-RCN-FIN-001;UNB-ACC-006 | IssueAcknowledgment | Issuance blocked; contribution remains Accepted not Acknowledged | No document hash issued | Counsel approves v4; reissue succeeds once |
| DKS-FIN-004 | 8282 obligation on sale of closely held stock | UNB-RCN-FIN-003;UNB-INV-012 | Sale of L1 settles 14 months after receipt | Form8282Obligation Open with due date per rule table; donor-copy task | Proceeds credited to fund; obligation independent of cash | Escalate 10 days before due; Filed with receipt closes |
| DKS-FIN-005 | Excess business holdings at acceptance | UNB-RCN-FIN-004;REQ-X-059 | Accept gift of B interest | Acceptance Held (EBH review); no recognition | No capacity increase | Counsel decision; disposal plan if accepted |
| DKS-FIN-006 | Crypto gift near midnight with conflicting prices | UNB-RCN-FIN-011;UNB-NAV-015 | Transfer confirmed 23:59:59 local; feeds differ 4% | Valuation ExceptionHold; appraisal-required flag if above threshold | No capacity until liquidation settles (FI-24) | Reviewer selects per policy; sale proceeds fund grants |
| DKS-FIN-007 | Duplicate service fee at pool and DAF level | UNB-NAV-033;UNB-RCN-FIN-005 | Monthly accrual | DAF-level accrual Rejected(DuplicateServiceFee) | Fund charged once for the service | Controller corrects schedule |
| DKS-FIN-008 | Fee-on-fee exclusion | UNB-NAV-033 | Accrual | Base excludes $50; FeeAccrued posted | Recompute equals stored amount | not applicable |
| DKS-FIN-009 | Concurrent reservations with lagging projection | UNB-GFUND-021;REQ-X-022 | Two $6,000 reservations concurrently | One SponsorApprovedAndReserved; other InsufficientCapacity | Sum reservations <= capacity | Loser follows OD-06 |
| DKS-FIN-010 | Payment authorization before liquidation settles | UNB-CUST-015;UNB-RCN-FIN-008 | Request PaymentInstructionAuthorized | Refused NotFunded; grant LiquidationPending | No cash from other funds used | Funded after redemption settles |
| DKS-FIN-011 | Pool redemption gate | UNB-GFUND-014;UNB-NAV-026 | Pool P suspends redemptions | Grant FundingShortfall; sponsor case | Reservation retained; no partial pay unless policy | Re-plan to permitted sources or hold |
| DKS-FIN-012 | Omnibus dividend attribution | UNB-INV-002;UNB-RCN-FIN-007 | $100 dividend | A $60, B $40 posted | Sum fund lots = custodian position | Residual to residual account |
| DKS-FIN-013 | Custodian back-dated correction changes struck NAV | UNB-NAV-038;UNB-NAV-010 | Custodian restates day D position | Original NAV Superseded by Corrected publication; original statement preserved; NAV-error case | Units not silently recomputed | Materiality policy decides remediation |
| DKS-FIN-014 | Immaterial unexplained break aging | UNB-ACC-018 | $0.37 unexplained break for 6 days | Close not blocked; escalation case open | Certification discloses open item | Resolved by evidence |
| DKS-FIN-015 | Controller self-certifies reconciliation | REQ-X-010 | Controller attests | Denied; routed to independent reviewer | not applicable | Reviewer attests |
| DKS-FIN-016 | Refund request after acceptance | UNB-RCN-FIN-013 | Operator issues refund to donor | Rejected; counsel case | Fund value unchanged | Grant to eligible charity or counsel exception |
| DKS-FIN-017 | Percentage grant market move | UNB-GFUND-013 | NAV -3% before approval | Fixed amount from approval snapshot $9,700 (5% of base after a 3% fall); preview retained | Single fixing record | not applicable |
| DKS-FIN-018 | Closing 100% grant with trailing dividend | UNB-GFUND-002;FI-15;FI-35 | Dividend arrives after sweep settles | Sweep fixed at last settlement; trailing receipt to policy destination; fund not reopened | No negative balance | Per OD-11 |
| DKS-FIN-019 | Append-only enforcement | UNB-ACC-026;UNB-IMM-001 | UPDATE on posted journal; migration dropping trigger | UPDATE denied; deploy refused | Digest unchanged | not applicable |
| DKS-FIN-020 | Duplicate and out-of-order custodian files | UNB-CUST-009;UNB-CUST-012 | F1 replayed and correction F2 arrives before F1b | One posting per external ID; F2 held until ordering resolved | No duplicate journals | Reconcile after F1b |
| DKS-FIN-021 | Advisor fee for donor's personal adviser | UNB-RCN-FIN-010 | First fee accrual | Accrual held pending counsel | No fee posted | Counsel decision |
| DKS-FIN-022 | Minimum fee exceeds capacity with reserved grant | UNB-NAV-033;OD-10 | Fee accrual | Policy per OD-10 applied; no silent negative | Capacity never negative without exposure fact | Waiver or hold per policy |
| DKS-FIN-023 | Late custody event after close | UNB-ACC-017;REQ-X-040 | Back-dated trade arrives | Adjusting entry in open period; ReopenedByControlledCorrection only if material | Original snapshot preserved | Corrected statement revision if policy requires |
| DKS-FIN-024 | Cross-tenant source injection in funding plan | UNB-GFUND-028 | Funding plan cites partner B pool ID | Rejected not found (no existence leak) | No reservation | not applicable |
| DKS-PLT-001 | Global hold races cross-cell release | REQ-X-020;UNB-KNOW-012;UNB-TEN-002 | Compliance publishes sanctions hold H on CH-001 (v41) concurrently with release in C2 that read v40 before commit | C2 grant ends Held (not Released); C1 grant Held; no PaymentInstruction in Dispatched | Sum dispatched to CH-001 after v41 = 0; reservations unchanged | Lift of H by compliance re-runs gate; dispatch resumes with new idempotency check |
| DKS-PLT-002 | Hold registry outage fails closed | REQ-X-020;REQ-X-006 | Release gate runs | All 10 in ReleaseBlocked(HOLD_REGISTRY_UNAVAILABLE) | No instruction created; reservations intact | On recovery, gate re-evaluates against current registry version |
| DKS-PLT-003 | Forged tenant hint | UNB-TEN-004;UNB-PLAT-008 | GET /funds/F-T2-0001 with header X-Tenant: T2 and schema hint partner_b | 404 RESOURCE_NOT_FOUND; session tenant T1 | No data change | None needed |
| DKS-PLT-004 | Multi-tenant advisor revoked mid-session | UNB-TEN-004;GOV-AUTH-015 | Sponsor revokes T2 delegation; advisor submits grant recommendation in T2 | Command rejected; selection token invalid; no GrantRecommendation in Submitted | No reservation | Advisor re-selects T1 |
| DKS-PLT-005 | Pooled connection leaks search_path | UNB-TEN-005 | Next request for T2 reuses connection | Broker resets role and search_path; query runs as T2 role; zero T1 rows | None | If reset fails, connection discarded |
| DKS-PLT-006 | Cross-partner transfer half-applied | REQ-X-021;UNB-TEN-020 | C2 crashes after TransferOutPending recorded in C1 | C1: TransferOutPending; suspense +10,000; C2 no TransferIn; after recovery TransferInRecorded then Reconciled | Sponsor total balance constant; suspense nets to 0 after reconcile | Idempotent replay of inter-cell message; daily reconciliation flags if pending > threshold |
| DKS-PLT-007 | Tenant migration interrupted | UNB-TEN-013 | Cutover interrupted before tail catch-up; source still accepting writes | Source remains sole writer; destination binding not active; no split brain | Ledger totals equal source | Restart from new watermark |
| DKS-PLT-008 | Agent attempts grant approval | AIN-003;UNB-AI-STD-003;GOV-AUTH-003 | Agent calls ApproveDecision on grant G | Denied AGENT_APPROVAL_PROHIBITED; G stays PendingApproval | No reservation change | None |
| DKS-PLT-009 | Agent closes own finding | UNB-AI-STD-003;GAP-V09 | Same agent calls CloseCase(F) | Denied; F remains Investigating | None | Human closes after evidence |
| DKS-PLT-010 | Assistant asked about undocumented operation | REQ-X-051 | User asks assistant to run op.x | Assistant replies undocumented; no API call | None | Owner adds contract |
| DKS-PLT-011 | Assistance context leakage | UNB-RCN-PLT-004;REQ-X-053 | U asks 'compare my fund with F2' | Context broker retrieves only U's funds; answer states no access | None | None |
| DKS-PLT-012 | Unknown payment displayed | UNB-UX-003;AIN-014 | Donor opens grant page and widget | Status label 'Outcome being confirmed'; not Paid or Failed | Grant not reversed; reservation held | Reconciliation resolves to Settled or Returned |
| DKS-PLT-013 | Locked clause removal in template | RR-REQ-X-029-r1;REQ-X-027 | Sponsor admin deletes clause and publishes | Publish rejected LOCKED_CLAUSE; template stays Draft | None | Admin restores clause |
| DKS-PLT-014 | Disallowed token in template | REQ-X-027;RR-REQ-X-029-r1 | Admin inserts {{fund.balance}} | Save rejected TOKEN_NOT_ALLOWED | None | None |
| DKS-PLT-015 | Legal hold vs disposition batch | REQ-X-036;REQ-X-038 | Batch executes | S retained; others Disposed; batch report lists S skipped | No financial record of current year affected | Re-evaluate S after hold release |
| DKS-PLT-016 | Sponsor offboarding with retention | REQ-X-036;UNB-TEN-016 | Offboarding plan executes | Export delivered; records retained in archive per contract option; access revoked for sponsor staff | Ledger export reconciles to final balances | Re-export on request |
| DKS-PLT-017 | Routine grant creates no work item | REQ-X-048;REQ-X-017 | Recommendation submitted | Approved by deterministic policy (if OD-01 yes) or PendingApproval single human; zero WorkItems for evidence | Reservation equals amount | None |
| DKS-PLT-018 | Duplicate claim of work item | REQ-X-048 | Two reviewers claim concurrently | One Assigned; other ALREADY_CLAIMED | None | None |
| DKS-PLT-019 | Work item auto-closes on evidence | REQ-X-048 | Evidence ingested and rule passes | W Completed -> SourceVerified -> Closed automatically | None | None |
| DKS-PLT-020 | Keyword link excluded from coverage | SOC2-007;MATRIX-001 | Coverage report computed | Candidate links contribute 0; GOV-AUTH-019 shown unmapped until reviewed | None | Owners review |
| DKS-PLT-021 | Heartbeat silence detected independently | UNB-SCTRL-041;SOC2-027 | No ControlHeartbeat for 2 intervals | Independent observer opens deficiency Detected; evaluator result InsufficientEvidence not Pass | None | Producer restored; deficiency to RetestPending |
| DKS-PLT-022 | Control status inflation rejected | SOC2-008;MATRIX-007 | Status report generated | Reported as Proposed (no evidence) | None | Attach review evidence |
| DKS-PLT-023 | Protected term override blocked | UNB-RCN-PLT-007;REQ-X-030 | Publish Brand Kit | Rejected PROTECTED_TERM | None | Edit mapping |
| DKS-PLT-024 | Mandatory notice not suppressible | UNB-RCN-PLT-002;AIN-023 | Contact email changed (security event) | Security notice sent to prior verified channel | None | None |
| DKS-PLT-025 | IVR numeric reference enumeration | REQ-X-025 | Caller enters 50 sequential account references | Neutral responses; rate limit after threshold; no existence disclosure | None | Lockout window |
| DKS-PLT-026 | Embedded widget after revocation | AIN-029;INT-015;TVR-WID-001 | Revocation event | Widget enters ContextRevoked within configured window; cached data cleared | None | User re-authenticates |
| DKS-PLT-027 | Release skew between cells | RR-REQ-X-057-r1 | Inter-cell transfer using v1.4 contract | C2 rejects dependent command; transfer stays Pending | No partial posting | Upgrade C2 then replay |
| DKS-GRT-001 | Day-180 renewal with clean evidence | REQ-X-007;TX-27 | The refresh job runs at the lead time and every source returns unchanged | C1 stays Verified (not ExpiredOrAdverseHeld); both grants remain ApprovedAndReserved and become PaymentPreparationEligible | No change to reservations | Not applicable: no failure path |
| DKS-GRT-002 | Day-180 refresh during source outage | REQ-X-007;UNB-RULE-009 | A release is attempted for a grant to C1 | The IRS-status fact is Stale; release is held with HOLD_MISSING; C1 is not IneligibleCommon | Reservation retained | Release proceeds automatically after source recovery and refresh |
| DKS-GRT-003 | Adverse listing mid-cycle | REQ-X-007;REQ-X-018 | An authenticated OFAC SDN listing of C2 is ingested before dispatch | C2 is HeldAdverse; payment Authorization=Suspended; Dispatch=NotSent | No dispatch; reservation retained | The sanctions officer case decides the outcome; no AI clearance |
| DKS-GRT-004 | Sponsor-private allegation tries to become a global hold | REQ-X-007;UNB-CHAR-036 | A Sponsor A operator issues RaiseGlobalHold | Rejected with HOLD_EVIDENCE_INSUFFICIENT; a Sponsor A overlay hold is created; Sponsor B's grants to C3 proceed | Not applicable: no money movement | Escalation to an independent compliance reviewer is available |
| DKS-GRT-005 | Type III NFI supporting organization on the routine path | UNB-GRANT-023;UNB-RCN-GRT-001 | A $5,000 routine grant is submitted | Recipient class disqualified; grant Held or Rejected; no ApprovedAndReserved | No reservation created | Counsel review case |
| DKS-GRT-006 | Grant to a natural person | UNB-RCN-GRT-001;TX-10 | Submission through the API | Rejected at Draft->Submitted; no queue item | No reservation | Donor guided to a scholarship fund at a charity |
| DKS-GRT-007 | Payee is the advisor's LLC | UNB-RCN-GRT-002;REQ-X-005 | Release evaluated | Release held (nonwaivable); WorkCase Investigating | No dispatch | Counsel determination required |
| DKS-GRT-008 | Pledge reference leaks into remittance | UNB-RCN-GRT-009;REQ-X-077 | Remittance advice is generated | Outbound remittance contains no 'pledge' or 'P-778'; the internal record retains them | Not applicable: no money movement | If a filter error is detected, block remittance generation |
| DKS-GRT-009 | Event-ticket checkout | REQ-X-063;UNB-RCN-GRT-012;INV-16 | Recommendation submitted | Grant Held (benefit gate) due to the context mismatch | No reservation | Donor told that tickets cannot be paid from the DAF |
| DKS-GRT-010 | Stale affirmation version | UNB-RCN-GRT-010;TX-10 | Submission with a v2 token | Rejected AFFIRMATION_STALE | No grant created | Widget re-renders v3 |
| DKS-GRT-011 | Recurring occurrence after revocation | UNB-GRANT-027;REQ-X-008;TX-18 | The scheduler fires the March occurrence | March occurrence Held; donor notified; after the OccurrenceHoldWindow it is Skipped; S1 still Active | No reservation and no payment for March | If C5 is reinstated within the window, March proceeds once; after the window there is no catch-up |
| DKS-GRT-012 | Recurring schedule suspension threshold | UNB-RCN-GRT-004 | Third consecutive occurrence held | S1 Suspended; resume requires donor reconfirmation and a fresh affirmation | No payments | Donor resumes to a different charity by creating a new schedule version |
| DKS-GRT-013 | Scheduler crash replay | TX-17;TX-18 | Scheduler replays | One occurrence; at most one grant | At most one reservation | Not applicable: idempotent replay |
| DKS-GRT-014 | Concurrent final-dollar reservation | REQ-X-002;TX-11 | Two $80 cash-funded grants approved concurrently | One ApprovedAndReserved; one Rejected CAPACITY_INSUFFICIENT | Sum reserved <= $100 | Rejected grant may be resubmitted |
| DKS-GRT-015 | NAV drop before redemption | REQ-X-002;UNB-GFUND-011 | NAV falls 8% before redemption settles; proceeds $9,660 | Grant Held (shortfall); no payment instruction | Other grants' reservations untouched; no negative capacity | Sponsor policy decides reduce/top-up |
| DKS-GRT-016 | Concurrent percentage grants | UNB-GFUND-013;UNB-GFUND-002 | Two 50% grants approved simultaneously | First $10,000; second computed on $10,000 remaining = $5,000, or retried | Total reserved <= base | Recompute on conflict |
| DKS-GRT-017 | Timeout then late acceptance | REQ-X-004;REQ-X-083;PAY-01 | Operator requests a resend; later an accepted callback arrives | Dispatch=DispatchOutcomeUnknown; resend refused; then ProviderKnowledge=Acknowledged | One dispatch only | Normal settlement path continues |
| DKS-GRT-018 | Unknown resolution to NotReceived | UNB-RCN-GRT-008;REQ-X-004 | Independent approver declares NotReceived | ProviderKnowledge=NotReceived (terminal for the attempt); a reissue is created linked to the original | At most one settled payment in the chain; a late original settlement opens a duplicate case | Duplicate recovery workflow if the original later settles |
| DKS-GRT-019 | Duplicate callbacks with new IDs | REQ-X-083;PAY-07 | Provider redelivers the same settlement as E1' with a different event ID | Semantic dedup recognises E1' as a duplicate; SettlementEvidence unchanged | One settlement posting | Not applicable: no failure path |
| DKS-GRT-020 | Unauthenticated callback | REQ-X-083 | Forged callback claiming Executed without a valid signature | Rejected CALLBACK_UNAUTHENTICATED; state unchanged | No posting | Not applicable: callback rejected |
| DKS-GRT-021 | Intermediary bank blocks a wire | UNB-RCN-GRT-006;REQ-X-083 | Blocking notice from the intermediary | ProviderKnowledge=Blocked; SettlementEvidence=Blocked; WorkCase=Investigating | Reservation not restored; no reissue; BlockedProperty classification | Release only with license evidence and independent authority |
| DKS-GRT-022 | Stale-dated check and void | UNB-RCN-GRT-005;UNB-PAY-006 | Aging job runs; Controller requests void | AgedOutreach -> StaleDated -> VoidRequested -> VoidConfirmed (after positive-pay acknowledgment) | Reservation handled per approved policy; no capacity restored before VoidConfirmed | Reissue only after the eligibility recheck |
| DKS-GRT-023 | Voided check presented later | UNB-RCN-GRT-005 | The check is presented to the bank | Positive-pay exception rejected; payment state unchanged | No ledger effect | Not applicable: presentment rejected |
| DKS-GRT-024 | NOC corrects account number | REQ-X-005;TX-19 | NOC received with a corrected account | Destination v2 Pending; next entry held until v2 Effective via the expedited verification | No entry to unverified v2 | Verification completes and entries resume |
| DKS-GRT-025 | Hold races release | REQ-X-020;REQ-X-019;CONC-02 | Adverse hold commits during the release | If the hold commits before the release serialization point, the release aborts; otherwise the payment is dispatched and a post-dispatch stop/recall is requested | No release ordered after the hold | UNB-RCN-GRT-016 protective actions |
| DKS-GRT-026 | Church absent from IRS files | UNB-RCN-GRT-013;UNB-CHAR-016 | Grant evaluated | G03 Pass with evidence class recorded; no human queue | Normal reservation | Not applicable: no failure path |
| DKS-GRT-027 | Charity declines grant before release | UNB-RCN-GRT-015;UNB-GRANT-028 | The recipient declines through the portal | Grant DeclinedByRecipient; reservation released; donor notified | Capacity restored exactly once; no tax receipt | Donor may redirect |
| DKS-REG-001 | Grant-date reliance after mid-window revocation | RR-REQ-X-007-r1;REQ-X-007 | Auto-Revocation List posting for the EIN; payment release runs | Grant: Approved -> EligibilityHold; PaymentAttempt: none created | Reservation unchanged; no payable; no cash movement | Sponsor notified; donor offered redirect; reservation released only on cancellation |
| DKS-REG-002 | Supporting organization without donor-control certification | UNB-RCN-REG-002 | Donor submits grant recommendation | Recommendation: Submitted -> Held(SupportingOrgCertificationMissing) | No reservation | Certification request sent; resumes on receipt |
| DKS-REG-003 | Confirmed sanctions match blocks property and starts OFAC reporting | UNB-RCN-REG-003;UNB-RCN-GRT-006 | Authorized compliance officer confirms true match | Grant: Blocked; Payment: none; ledger: funds moved to blocked-property account | Blocked amount cannot be reserved, returned or regranted; fund capacity excludes it | Only an OFAC licence or delisting event can release funds |
| DKS-ARC-001 | Application role cannot mutate the ledger; drift blocks release | RR-UNB-PLAT-007-r1;UNB-PLAT-007 | (1) the application issues UPDATE on a posted journal line; (2) an operator adds a trigger manually, then a release runs | (1) permission denied; (2) Strata drift 'differs', release Blocked | Posted journal unchanged | Incident opened; schema restored via a Strata plan |
| DKS-ARC-002 | Breaking event-schema change is rejected | UNB-RCN-ARC-001 | A field is removed without a version bump | CI: Failed with breaking-change report | Not applicable | Introduce v2 with dual publication |
| DKS-ARC-003 | Sponsor terminology override cannot alter legal statement | UNB-RCN-ARC-002;UNB-RCN-PLT-007 | Brand Kit publish | Terminology change Published; legal-sentence change Rejected(ProtectedTerm) | Not applicable | Sponsor edits other copy only |
| DKS-REG-004 | Pending DAF regulation becomes final | UNB-RCN-REG-001 | Status updated to Final | ImpactReview: Open, listing linked requirement IDs | Not applicable | Review completes within the configured window |
| DKS-REG-005 | OFAC record retention outlasts platform floor | RR-REQ-X-036-r1;REQ-X-036 | Disposition job runs in 2034 | Record not eligible; effective_until 2036-01-15 | Not applicable | None needed |
| DKS-REG-006 | Partial securities receipt withholds acknowledgment | RR-UNB-ACC-006-r1;UNB-ACC-006 | Custodian delivers 600; duplicate message for same 600 | Contribution PartiallyReceived; one Received event; duplicate linked | One journal entry for 600 shares; grantable capacity unchanged until acceptance and finality | Donor prompted to confirm final quantity |
| DKS-XD-001 | Deceased advisor's delegated agent acts before succession | UNB-RCN-ENT-004;UNB-SUCC-008 | Agent submits a grant recommendation at T+1 minute | Recommendation rejected (PrincipalSuspended); A's sessions revoked; fund ActiveSuccessionPending | No reservation created | Successor activation follows sponsor policy; co-advisors unaffected |
| DKS-XD-002 | Unclassified product type cannot launch | UNB-RCN-ENT-011 | Attempt to open the first account of that type | Configuration Blocked(ClassificationMissing) | None | Counsel adds classification; launch proceeds |
| DKS-XD-003 | Identity regime without counsel status is not labelled law | UNB-RCN-ENT-014;RCF-REG-007 | Donor onboarding displays requirements | Onboarding shows identity proofing as sponsor policy; no 'required by law' copy | None | Copy updates when counsel status changes |
| DKS-XD-004 | Cross-fund cash-only journal is rejected | UNB-RCN-FIN-006;RCF-FIN-010 | Post journal: Dr Cash(Fund B) 500 / Cr Cash(Fund A) 500 with no net-asset transfer lines | Journal Rejected(UnbalancedBySegment: fund) | No posting; fund balances unchanged | Resubmit with internal transfer clearing lines netting to zero per fund |
| DKS-XD-005 | Lagging projection cannot authorize overcommitment | UNB-RCN-FIN-009;REQ-X-002 | Two concurrent $1,500 reservations and one $5,000 reservation | Exactly one $1,500 reservation Committed; others Rejected(InsufficientCapacity) | Sum of reservations ≤ authoritative capacity at every commit | Donor sees authoritative capacity on retry |
| DKS-XD-006 | Isolation map answers for every data class | UNB-RCN-PLT-001 | Isolation map queried; a query from partner P2 of the same sponsor requests P1's note | Map returns isolation key=sponsor, sharing=per-rule; cross-partner read allowed only if rule permits | None | Deny by default when no rule |
| DKS-XD-007 | Release in another cell honours a newer global hold | UNB-RCN-PLT-003;RCF-PLT-002 | Cell 2 release evaluates a payment to X | Release Held(HoldWatermarkStale) until v41 is observed; then Held(GlobalHold) | No payment instruction | Fail closed if registry unreachable |
| DKS-XD-008 | Agent cannot perform an action above its matrix level | UNB-RCN-PLT-005;AIN-003 | AI agent invokes ApproveGrant via API | Denied(AuthorityCeilingExceeded) | No approval, no reservation | Repeated attempts suspend the agent principal |
| DKS-XD-009 | Sanctions list update rescreens in-flight grants | UNB-RCN-GRT-007 | List update ingested | Grant Held(PotentialMatch) before next release window | No dispatch | Human disposition: false positive releases; true match goes to Blocked (DKS-REG-003) |
| DKS-XD-010 | Sponsor can tighten but not loosen freshness | UNB-RCN-GRT-014;RR-REQ-X-007-r1 | Sponsor S1 sets P90D; sponsor S2 attempts P365D | S1 grants require ≤90-day evidence; S2 change Rejected(WeakerThanPlatform) | None | S2 keeps P180D |
| DKS-XD-011 | Destination evidence gates automated release | UNB-RCN-GRT-017;REQ-X-005 | Release evaluated for both | First: automated release passes G08. Second: Held(DestinationEvidenceInsufficient) | No payment to unverified destination | Callback using independently sourced contact |
| DKS-XD-012 | Ambiguous charity name resolves to canonical identity | UNB-RCN-PLT-014 | Donor searches, selects one, submits recommendation | Recommendation bound to selected EIN; no other-sponsor hold reason visible | None until reservation | Unmatched recipient becomes an identity-resolution request |
| DKS-XD-013 | Grant approval races a sponsor policy change | REQ-X-002;REQ-X-019;ADM-027;RR-REQ-X-002-r1 | Approval of a $40,000 grant evaluated with policy snapshot v7 commits at T+1 ms; a second identical grant is evaluated after T | Grant 1: Approved under v7 only if the approval transaction verified v7 was current at commit; otherwise Rejected(PolicyVersionChanged) and re-evaluated under… | No reservation made under a superseded policy version; exactly one reservation per approved grant | Re-evaluation is automatic; the donor sees 'needs review under updated sponsor policy' |
| DKS-UI-001 | Giving home normal: priority and fold | UNB-UI-S-001;UNB-UI-N-006;UNB-UI-F-002 | Open home at 390x844 and 1280x900 | Headline figure and 'Needs your attention' heading visible above the tab bar at 390; figure, both tasks and page actions in first 1280 viewport; reference imag… | Available + committed + settling = total fund value shown | n/a |
| DKS-UI-002 | Loading skeleton geometry | UNB-UI-DS-014;UNB-UI-G-013 | Render loading then normal at 1280 | Panel boxes differ by <= 24 px vertically; aria-busy region with polite status; no animation under reduced motion | No figure shown before data | If data fails, error state replaces skeleton (G-013) |
| DKS-UI-003 | New fund onboarding (no contributions) | UNB-UI-DS-013;UNB-UI-S-001 | Open home | Checklist 1 of 3 done; one primary action 'Add money'; no empty $0.00 hero repeated | Available to grant $0.00 | n/a |
| DKS-UI-004 | Money available, no grants | UNB-UI-DS-013;UNB-UI-S-001 | Open home | Grants panel explains and offers 'Recommend a grant' | Committed $0.00 | n/a |
| DKS-UI-005 | Stale Available to grant | UNB-UI-F-006;UNB-UI-G-003;UNB-UI-DS-010 | Projection older than limit | 'Not current' chip and last confirmed time in the balance panel; one notice explaining what is still safe | Figure is the last confirmed value, never extrapolated | Recommendations validated against confirmed balance at approval |
| DKS-UI-006 | Pricing integration unavailable | UNB-UI-DS-010;UNB-UI-F-007 | Pricing service returns 503 | Total row reads 'Unavailable'; bar omitted; Available to grant unaffected; one notice | Available to grant independent of market value | Values refresh automatically when service returns |
| DKS-UI-007 | Restricted fund: recommending paused | UNB-UI-DS-005;UNB-UI-DS-010 | Sponsor review hold on fund | No 'Recommend a grant' page action; notice says why, how long, what still works; deep link to recommend shows the same explanation | No reservation possible | Contact donor services |
| DKS-UI-008 | Suspended fund | UNB-UI-DS-010;UNB-UI-S-001 | Fund suspended for overdue certification | Notice with date, reason and lifting action; no grant or contribution actions | Balances read-only | Completing certification lifts suspension |
| DKS-UI-009 | Succession pending: successor first view | UNB-UI-S-001;UNB-UI-G-007 | Successor signs in before activation | Condolence, plain explanation, 'Review and accept' primary; steps of what happens next | Fund value shown as at pause | Decline or ask a question available |
| DKS-UI-010 | Read-only advisor | UNB-UI-N-004;UNB-UI-DS-005 | View-only participant opens home and a grant | 'View only' shown; no add-money, recommend, withdraw or redirect controls; access expiry stated | none | n/a |
| DKS-UI-011 | Failed payment: returned grant | UNB-UI-S-004;UNB-UI-S-001;UNB-UI-G-006 | Charity bank returns ACH | Home task 'A payment was returned' with 'Review options'; grant detail explains, money back in Available to grant, wait or redirect with consequences | Available to grant increases by returned amount exactly once | Redirect creates a new recommendation; wait keeps grant open |
| DKS-UI-012 | Reconciliation exception with segregation of duties | UNB-UI-S-008 | Bank-vs-ledger break; preparer proposes resolution then views it | Expected vs actual and difference shown; resolutions with authority; preparer cannot approve own proposal and is told why | Difference computed = actual - expected | Approver can reject with reason |
| DKS-UI-013 | Embedded widget in partner host | UNB-UI-W-001;UNB-UI-R-001 | Host embeds widget at 360 and 600 px; token expires | Widget keeps sponsor brand and disclosure inside host; readable at both widths; expired token clears data and shows safe message; no consequential actions | Amount equals grant record | Host re-requests token server-to-server |
| DKS-UI-014 | White-label sponsor swap | UNB-UI-DS-012;UNB-UI-DS-001 | Render with example-harbor manifest | Only colours, fonts, radius, mark and name differ from unbound-default; legal disclosure verbatim; all checks pass | none | n/a |
| DKS-UI-015 | Keyboard walk and focus visibility | UNB-UI-A-002;UNB-UI-N-003 | Tab through the page | Every stop has a >= 2 px indicator and is not covered at its centre; skip link reaches main; focus order identical at 390 and 1280 | none | n/a |
| DKS-UI-016 | Reflow, zoom and text spacing | UNB-UI-A-004;UNB-UI-F-001;UNB-UI-R-001 | Render at 320 px and 200% zoom; inject WCAG 1.4.12 spacing | No horizontal page overflow; no clipped text; no money or reference wraps | Money tokens intact | n/a |
| DKS-UI-017 | Automated WCAG 2.2 AA | UNB-UI-A-001;UNB-UI-A-003 | Run axe-core wcag2a..wcag22aa, including incomplete contrast | Zero violations; one h1; no skipped heading levels; unique visible nav labels; no console errors | none | n/a |
| DKS-UI-018 | Manual screen-reader review | UNB-UI-A-003 | NVDA+Firefox, VoiceOver+Safari iOS, TalkBack+Chrome | Each task completable; status, money and dates announced correctly; submission result announced | none | n/a |
| DKS-UI-019 | Forced colours and greyscale | UNB-UI-DS-004;UNB-UI-DS-008;UNB-UI-F-007 | Windows high contrast; greyscale filter | Statuses identifiable by glyph and words; allocation segments distinguishable; focus visible | none | n/a |
| DKS-UI-020 | Grant recommendation with validation and single confirmation | UNB-UI-S-003;UNB-UI-DS-017;UNB-UI-DS-015 | Amount above available, then corrected and submitted | Error summary links to field, values kept; review step states change, reversibility, next actor and timing; no second dialog; result announced | Reservation equals amount; no duplicate on resubmit | Unknown outcome shows check-status, not resubmit |
<!-- END GENERATED: scenarios -->

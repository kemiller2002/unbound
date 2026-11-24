# Workflow modelling report — GRT (WFM-06..13)

Files: /home/user/unbound/requirements/reconciliation/2026-11-23/workflows/WFM-06..13-*.json (generator: scratchpad/grt_gen/*.py).
Checker `python3 -I tools/reqaudit/workflows.py .`: **0 ERROR** (all 26 files); my files carry only W11 "no scenario" warnings, on transitions where no DKS/ADV/SIM/TVS scenario exists.

## 1. Per workflow

| Workflow | States | Transitions | Human | Checker |
|---|---|---|---|---|
| WFM-06 charity verification | 18 | 29 | 14 | 0 ERROR, 8 W11 warn |
| WFM-07 grant recommendation | 11 | 17 | 9 | 0 ERROR, 5 warn |
| WFM-08 grant approval and funding | 21 | 35 | 16 | 0 ERROR, 9 warn |
| WFM-09 recurring schedule | 9 | 14 | 6 | 0 ERROR, 5 warn |
| WFM-10 ACH payment | 16 | 21 | 4 | 0 ERROR, 7 warn |
| WFM-11 check payment | 18 | 22 | 8 | 0 ERROR, 10 warn |
| WFM-12 sanctions screening | 10 | 16 | 8 | 0 ERROR, 5 warn |
| WFM-13 destination change | 12 | 15 | 8 | 0 ERROR, 4 warn |

RCD-005 is modelled as follows. WFM-08-T01 is a deterministic step (A2) that outputs ApproveRecommended. WFM-08-T02 is the human A4 batch approval, which commits approval and reservation together. WFM-08-T03 is the delegated A3 rule path; it is marked DELEGATED PATH and guarded by `delegation-approved-for-sponsor`.

## 2. Workflow defects found

1. **Approval without reservation contradicts the atomic rule.** The prior model has `ApprovedAwaitingCapacity` (state-models.md:69, GR-10), which conflicts with RR-REQ-X-002-r1 (approval and reservation in one transaction). Resolution: `GrantCapacityShort` is a not-approved state with no reservation (WFM-08-T12/T13).
2. **Naming conflict.** RR-REQ-X-002-r1 says "ApprovedPendingLiquidity"; UNB-RCN-FIN-008 and RR-UNB-CUST-015-r1 say FundingPlanned/LiquidationPending. The model uses the FIN-008 names. RR-REQ-X-002-r1 wording needs aligning.
3. **Stale interim text on death.** RR-REQ-X-008-r1 says "until OD-03 decided apply hold_for_review", but RCD-022 decided that approved unpaid grants proceed. Resolution: WFM-08-T31 fires only when sponsor policy = hold_for_review. RR-REQ-X-008-r1 text needs updating.
4. **Check re-credit conflicts with escheat.** RCD-028 says void at 180 days and re-credit to the fund. UNB-RCN-GRT-005 says the money MUST NOT be silently released to the fund. Resolution: WFM-11-T14/T15/T16 record the escheat disposition before any re-credit.
5. **NOC versus "payments continue to the previous destination".** RCF-GRT-019 and RR-REQ-X-005-r1 require holding new entries after a NOC, but S-012 and the brief say payments continue to the previous verified destination. Resolution: a NOC-sourced change holds new releases to the old version (WFM-13-T03, WFM-10-T01 `DestinationCorrectionPending`). A requester-initiated change keeps the old version live. Authorized payments always keep their pin.
6. **Unknown payments can become a trap.** In UNB-RCN-GRT-008 and RR-REQ-X-004-r1, the only manual exit (NotReceived) requires the recipient to confirm non-receipt. If that confirmation is unobtainable, there is no exit (PA-2 ResolvedByDecision was never adopted). WFM-10-T09/T10 only escalate. **Needs decision.**
7. **Ambiguous suspension threshold.** UNB-RCN-GRT-004 counts "held or skipped" occurrences, so the threshold can be reached while an occurrence is still held. The model suspends and skips that occurrence (WFM-09-T06). **Needs decision.**
8. **True match before approval.** UNB-RCN-REG-003 covers only property already in the sponsor's possession. A true match on an unapproved grant's recipient has no defined effect (WFM-08-T08 declines). OFAC rejected-transaction reporting is absent (WFM-12-T09). **Needs counsel.**
9. **Charity decline action has no screen.** UNB-RCN-GRT-015 says the recipient may decline before release, but UNB-UI-S-012 has no decline action (WFM-08-T30).
10. **TX-27 jumps from due to held.** TX-27 goes straight from a 180-day "due" to ExpiredOrAdverseHeld (RCF-GRT-003). Resolution: WFM-06 separates refresh, expired, escalated and held-adverse. An expired deep review holds new approvals only.
11. **TX-13 merges Unknown with Accepted.** WFM-10 splits them (RR-REQ-X-083-r1).
12. **Prior model includes AI agents.** The REC model has a confirmation step for agent-originated recommendations, but AI agents acting on accounts are out of scope (RCD-013). WFM-07-T02 is limited to human delegates.

## 3. Requirement gaps (proposed statements)

- **Capacity wait (WFM-08-T12/T13):** "A grant that fails capacity at approval MUST wait in CapacityShort for at most a sponsor-configured CapacityWaitWindow, then be declined with a reason."
- **Unknown fallback (WFM-10-T10):** "Where the NotReceived evidence combination cannot be obtained within the final aging tier, two independent treasury approvers MAY resolve by decision with a recorded duplicate-risk acceptance and recovery plan."
- **Manual release (WFM-10-T03):** "A payment whose destination evidence is ineligible for automated release MAY be released only by an independent A4 treasury approver; holds, sanctions and stale eligibility are never overridable."
- **Rejected transactions (WFM-12-T09, WFM-08-T08):** "A confirmed match without property in possession MUST refuse the transaction and produce OFAC rejected-transaction report data within 10 business days."
- **Windows (WFM-06-T08, WFM-07-T05/T06/T13, WFM-12-T05, WFM-13-T08):** "Draft TTL, confirmation, re-affirmation, evidence-request, sanctions-disposition SLA and destination-verification windows MUST be versioned policy parameters with owners."
- **Shortfall default (WFM-08-T19):** "The CFO MUST approve the default ShortfallRule (reduce / waterfall top-up / hold)."
- **Positive-pay exceptions (WFM-11-T11/T13):** "Positive-pay exception pay/return decisions MUST be in the payment authority model, with default Return."
- **Destination activation (WFM-13-T12):** "Activation MUST follow a configurable cooling-off period and notify both the previous and the new verified channels."

## 4. NEW surfaces

- Charity identity resolution: a charity data analyst matches ambiguous recipients.
- Charity alternate evidence review: a compliance analyst accepts EntityTypeEvidence.
- Recipient class counsel review: counsel decides CounselReview classes.
- Supporting organization certification: a charity officer certifies no donor control.
- Charity hold decision panel: sponsor or platform compliance places and releases holds.
- Grant batch approval: a board-authorized approver batch-approves ApproveRecommended grants.
- Related-party hold decision: independent compliance acts on counsel's determination.
- Liquidity advance approval: a treasury approver approves an interfund advance.
- Recurring schedule management: a donor or successor amends, resumes or cancels a schedule.
- Payment release approval: a treasury approver releases a payment whose destination is ineligible for automated release.
- Unknown payment resolution: an independent treasury approver declares NotReceived.
- Sanctions match decision panel: an analyst or officer dispositions matches and handles licence releases.
- OFAC blocked property report: a compliance officer records report filing.
- Check void and reissue: treasury voids lost checks and grants ops reissues.
- Positive pay exception decision: treasury decides pay or return.
- Unclaimed property disposition: counsel and the Controller decide escheat or re-credit.
- Destination change verification: an independent verifier handles callbacks, fraud review and revocation.
- Assisted change console: reused from WFM-25; staff-assisted destination change intake.

# Workflow modelling report: ENT (WFM-01, 02, 19-26)

Files are in `/home/user/unbound/requirements/reconciliation/2026-11-23/workflows/`. The generator scripts are in `/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/ent/`.

## 1. Models and checker status

| Workflow | File | States | Transitions | Errors | W11 warnings |
|---|---|---|---|---|---|
| WFM-01 | WFM-01-sponsor-activation.json | 22 | 23 | 0 | 7 |
| WFM-02 | WFM-02-fund-opening-participants.json | 21 | 22 | 0 | 6 |
| WFM-19 | WFM-19-authority-delegation.json | 15 | 22 | 0 | 7 |
| WFM-20 | WFM-20-death-succession.json | 17 | 20 | 0 | 4 |
| WFM-21 | WFM-21-dormancy.json | 9 | 11 | 0 | 3 |
| WFM-22 | WFM-22-fund-closure-transfer.json | 13 | 17 | 0 | 5 |
| WFM-23 | WFM-23-exception-work-item.json | 12 | 16 | 0 | 4 |
| WFM-24 | WFM-24-sponsor-policy-change.json | 13 | 16 | 0 | 3 |
| WFM-25 | WFM-25-support-appeal.json | 11 | 18 | 0 | 4 |
| WFM-26 | WFM-26-sponsor-migration.json | 17 | 18 | 0 | 6 |

Final run of `python3 -I tools/reqaudit/workflows.py .` with 18 workflow files present: **0 ERROR in total**, with no W9 label conflicts. Every remaining warning is a W11 warning: a guarded transition with no matching DKS, TVS, ADV or SIM scenario. All state ids are specific to their workflow.

Decisions the models apply:
- RCD-004: one database per legal sponsor, and Profile B unless a TenantIsolationDecision approves another profile (WFM-01-T05).
- RCD-005: the routine approval tier outputs ApproveRecommended, and a human gives A4 approval. Board delegation flips an authority flag (WFM-01-T04/T18, WFM-24-T09/T14).
- RCD-015: a potential sanctions match goes to a human. A true match leads to Blocked and an OFAC case.
- RCD-022: the dormancy clock and outreach run 36 + 12 months, and the charitable default needs A4 approval. OD-03 is applied as "approved unpaid grants proceed unless sponsor policy" (WFM-20-T07).
- RCD-023: identity proofing applies only to advisory-privilege holders and successors. Everyone is screened for sanctions.
- RCD-028: an outstanding check moves to a liability at closure.

## 2. Defects in the requirements

1. **OD-03 contradiction.** RR-REQ-X-008-r1 (data/refinements.jsonl:2) says to apply `hold_for_review` until OD-03 is decided, and its AC-1 expects HeldForReview. DKS-ENT-003 (data/scenarios.csv:4) expects the same. RCD-022 (data/decisions.csv:23) has now decided OD-03 the other way: grants proceed unless sponsor policy says otherwise. RCF-ENT-015 also says "no default". The model follows RCD-022 (WFM-20-T07). RR-REQ-X-008-r1 needs an r2, and DKS-ENT-003 needs its expected state changed.
2. **Transfer and closure happen in opposite orders.** RR-UNB-WF-014-r1 AC-1 (refinements.jsonl:8) says a fund enters closure after a full transfer settles. UNB-RCN-ENT-006 (gap-requirements.jsonl:6) puts ClosureRequested first and the residual later. The model treats the full transfer as the residual disposition inside closure: trigger first, then a DAF_TRANSFER_OUT grant (WFM-22-T08/T09/T11). This needs harmonising.
3. **Returned or voided money has nowhere to go once a fund is closed.** RCD-019 sends returned grant funds back to the originating fund, and RCD-028 re-credits voided checks the same way. When that fund is Closed or Archived, there is no destination. The model routes the money to the OD-11 trailing destination instead (WFM-22-T14/T17; TVS-366, DKS-ENT-015).
4. **The revocation propagation bound has no value.** RR-UNB-ADV-001-r1 (refinements.jsonl:6) gives no number for the bound. UNB-UI-S-006 (ui-requirements.jsonl:24) says revocation is immediate. The model treats it as immediate at the next check (WFM-19-T14/T15/T21). A decision is needed.
5. **Reinstatement after incapacity is undefined.** RR-UNB-SUCC-008-r1 (refinements.jsonl:1) covers a false notice only (AC-3). Once rights are suspended for incapacity, nothing lets them come back. The model adds WFM-20-T06.
6. **Standing contribution debits continue after a death notice.** The ProtectiveHold scope in RR-UNB-SUCC-008-r1 does not cover pulls from the subject's bank account (TVS-334, TVS-522). The model pauses them (WFM-20-T01).
7. **RR-REQ-X-048-r1 (refinements.jsonl:30) has no lifecycle.** It defines no SLA escalation, no rule that the decider must differ from the preparer, no reopen and no rule for flags raised by a model. The model adds all four (WFM-23-T07/T11/T13/T16).
8. **Dormancy thresholds are undecided, and continued holding never ends.** RR-UNB-WF-015-r1 (refinements.jsonl:7) defines Quiet and Unresponsive stages, but RCD-022 fixes only the 36- and 12-month thresholds. The "continued holding" disposition has no exit other than new activity. WFM-21-T01 makes Quiet optional, and WFM-21-T07 leaves continued holding open, so a decision is needed.
9. **REQ-X-045, REQ-X-064 and E2E-WF-01 point to documents instead of stating rules.** There is no normative activation gate, contract-lapse suspension or exit-completion rule (WFM-01-T17/T19/T22). There is also no rule for suspending a policy when its board delegation lapses (WFM-24-T14; TVS-638).
10. **UNB-RCN-ENT-016 (gap-requirements.jsonl:16) leaves reconsideration underspecified.** It gives no outcome set, window, response time or reviewer-independence rule. The model assumes Upheld, Overturned, Modified or Referred, with an independent reviewer (WFM-25-T09/T13).
11. **There is no published A0–A5 action-class matrix.** UNB-RCN-PLT-005 has not been produced, so the authority levels in these models are inferred and must be checked against it.
12. **RR-REQ-X-061-r1 (refinements.jsonl:9) has no exit for some cases.** It defines no state for an Unknown fence outcome and no way to abandon a migration before cutover. The model adds WFM-26-T10/T17.

## 3. Requirement gaps (proposed statements)

- **G1 (WFM-01-T17).** "Program activation MUST be an A4 sponsor-officer command that requires: a current contract; current board delegation; the legal-sponsor disclosure; a passing tenant isolation suite; tied-out opening balances; and finance readiness certified by a different principal."
- **G2 (WFM-01-T19/T20).** "On contract lapse the program MUST suspend new funds, contributions and approvals, while in-flight payments continue under their own workflows."
- **G3 (WFM-20-T06).** "Rights suspended for incapacity MUST be reinstated only by an A4 reviewer on capacity-restoration evidence, with credentials re-issued through the verified channel."
- **G4 (WFM-20-T01).** "A protective hold MUST pause standing contribution debits from accounts held by the subject."
- **G5 (WFM-19-T14/T15/T21).** "Revocation and expiry MUST take effect at the next authorization check, and in no case later than N seconds, including for sessions, widgets and agent tokens."
- **G6 (WFM-22-T01/T10/T15).** "Closure MUST define: the policy for contributions received while closure is pending; withdrawal of a donor-requested closure before the residual is committed; and an A4 default residual disposition when the donor gives no recommendation within the policy window."
- **G7 (WFM-22-T12/T14/T17).** "Returns, voids and receipts for a Closed or Archived fund MUST route to its recorded trailing destination, and MUST NOT reopen the fund."
- **G8 (WFM-24-T14/T15, WFM-01-T18).** "Policy versions that depend on a board delegation MUST suspend when the delegation lapses. Routine approval MUST then revert to human A4."
- **G9 (WFM-23-T07/T11/T13).** "WorkItems MUST escalate on SLA breach. They MUST be decided only by a principal who is not the preparer and who holds authority for the action class. They MUST reopen when execution of the decision fails."
- **G10 (WFM-25-T09/T13).** "Reconsideration MUST have a request window, the outcome set {Upheld, Overturned, Modified, Referred}, a reviewer independent of the original decider, and a response time."
- **G11 (WFM-02-T14/T16).** "Invitations MUST expire after a policy period, and MUST become invalid if the inviter's authority ends before acceptance."
- **G12 (WFM-26-T10/T17).** "If the fence outcome is Unknown, Unbound MUST NOT post until the fence is confirmed. A MigrationRun MAY be abandoned before cutover, and its staging evidence MUST be retained."
- **G13 (WFM-24-T06–T08).** "Each sponsor policy parameter MUST be classified as platform-safety or not. Changes to platform-safety parameters MUST receive platform-risk clearance, which grants no sponsor authority."
- **G14 (WFM-21-T07).** "A fund under continued holding MUST be re-reviewed periodically."

## 4. New surfaces

- **Sponsor contract and activation record** (sponsor signatory and platform contracts officer): record the executed contract, initiate and complete sponsor exit.
- **Board delegation register** (sponsor corporate secretary): record or renew a board delegation, including the RCD-005 flag.
- **Tenant provisioning console** (platform operations): retry failed provisioning.
- **Sponsor implementation readiness checklist** (integration engineer): retry integration checks.
- **Opening balance tie-out workspace** (sponsor controller): explain opening-balance and migration breaks.
- **Sponsor activation gate** (finance officer and authorized officer): certify readiness, activate or reinstate the program.
- **Fund application** (donor): submit the application with the agreement and exclusive-legal-control acknowledgment, or withdraw it.
- **Fund application review** (sponsor acceptance officer): accept or decline the fund.
- **Invitation acceptance** (invitee or delegate): accept or decline a role or delegation.
- **Representative evidence upload** (legal representative): submit the POA, guardianship or letters instrument.
- **Succession allocation review** (succession approver): decide split constraints, activate succession.
- **Charitable default disposition** (board-delegated approver): approve or reject a charitable default for death, dormancy or closure.
- **Fund closure request** (advisory-privilege holder): withdraw a closure request.
- **Fund closure workspace** (finance officer): move an outstanding check to a liability.
- **Outbound transfer request** (advisory-privilege holder): name the receiving sponsor and consent to sending fund history.
- **Platform policy safety review** (platform risk officer): give or refuse hard-control clearance.
- **Assisted change console** (support agent): prepare a staged change.
- **Verified-channel confirmation** (donor): confirm or deny a staged change.
- **Support case status** (requester): provide information, reopen, withdraw.
- **Migration control console** (migration operator, controller, independent reconciler): run inventory, compensation, sign-off and abandonment.
- **Migration mapping review** (sponsor operations lead): approve the mapping.
- **Migration cutover authorization** (CFO with dual approval): authorize cutover or roll back.
- **Imported payee verification queue** (independent verifier): verify imported destinations one at a time.

These existing screens are reused: S-003, S-006, S-008, S-009 (sanctions disposition, evidence and decisions), and S-010.

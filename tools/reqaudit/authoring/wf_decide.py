"""Workflow-validation decisions (RCD-032..049), gap requirements (UNB-RCN-WF-001..030) and findings (RCF-WF-001..003)."""
import csv, json

D = "/home/user/unbound/requirements/reconciliation/2026-11-23/data"
WHO = "Integrating agent under owner delegation (instruction 2026-11-23: 'make best decisions and note')"
H = ["decision_id", "type", "question", "affected_ids", "decision_owner", "recommendation", "alternatives", "rationale",
     "blocks_implementation", "reversible", "status", "decided_option", "decision_basis", "validation_required", "revisit_trigger", "decided_by", "decided_on"]


def dec(i, typ, q, aff, owner, opt, alt, basis, val):
    return dict(decision_id=i, type=typ, question=q, affected_ids=aff, decision_owner=owner, recommendation=opt, alternatives=alt,
                rationale=basis, blocks_implementation="Y", reversible="Y", status="decided-provisional" if val != "none" else "decided",
                decided_option=opt, decision_basis=basis, validation_required=val, revisit_trigger="Validation result or sponsor policy change",
                decided_by=WHO, decided_on="2026-11-23")


NEWD = [
 dec("RCD-032", "finance", "When are acknowledgments issued for contributions on reversible rails (ACH)?", "UNB-RCN-WF-004;RR-UNB-ACC-006-r1;RCF-WF-001", "Sponsor CFO",
     "Sponsor setting OnAcceptance | AfterFinal; default AfterFinal. OnAcceptance requires the void-and-correct path on return.", "Always on acceptance (voids common); always after final (slower)", "Avoids issuing then voiding tax documents", "none"),
 dec("RCD-033", "finance", "Is provisional credit allowed in release 1?", "RR-UNB-ACC-006-r1;RCD-019;RCF-WF-001", "Sponsor CFO",
     "No. RCD-019 prevails: grantable capacity rises only on Final. The provisional-credit clause in RR-UNB-ACC-006-r1 is amended out.", "Provisional credit with clawback", "Simpler, no grants against reversible money", "none"),
 dec("RCD-034", "legal", "Is a complex-asset intermediary an agent of the sponsor or the donee?", "UNB-RCN-WF-005;RCF-WF-001", "Counsel",
     "Donee model for release 1: the intermediary charity is donee of record; it acknowledges the asset and handles 8283/8282 and §4943; the DAF later receives cash from it.", "Agent model (sponsor is donee; sponsor bears 8283/8282/§4943)", "Keeps complex-asset tax duties out of release 1", "Counsel confirmation per intermediary agreement"),
 dec("RCD-035", "finance", "Must every NAV have a human checker?", "UNB-NAV-008;RCF-WF-001", "Sponsor CFO + investment committee",
     "In-tolerance NAVs are published by deterministic policy (A3) under sponsor delegation, with a sampled human review the next business day. Out-of-tolerance NAVs need a human approver (A4).", "Human approval of every NAV (daily A4 work)", "Removes routine human work while keeping exceptions controlled", "Investment committee delegation"),
 dec("RCD-036", "finance", "Minimum fee versus reserved grant capacity (OD-10)", "RCF-WF-001", "Sponsor CFO",
     "Reserved grants always take priority. A fee shortfall is carried as a receivable against future capacity, waivable by A4. It never draws reserved capacity.", "Fee first", "Protects charitable commitments", "Sponsor CFO"),
 dec("RCD-037", "finance", "Does sampled attestation gate close certification?", "RCD-020;RCF-WF-001", "Controller", "Yes. Close certification requires the period's sample to be attested.", "Attestation after close", "Control evidence before publication", "none"),
 dec("RCD-038", "finance", "Do statements need approval separate from close certification?", "REQ-X-032;REQ-X-040;RCF-WF-001", "Controller", "No. Close certification approves publication of that period's statements.", "Per-statement approval", "Avoids redundant human step", "none"),
 dec("RCD-039", "product", "Pending approved grants at donor death", "RR-REQ-X-008-r1;RCD-022;RCF-WF-002;RCF-WF-003", "Sponsor operations",
     "RCD-022 prevails: approved unpaid grants proceed unless sponsor policy is hold_for_review. RR-REQ-X-008-r1 is amended accordingly.", "Always hold", "Consistency with RCD-022", "none"),
 dec("RCD-040", "product", "Closure, outbound transfer and money arriving for closed funds", "UNB-RCN-WF-014;UNB-RCN-WF-015;RCF-WF-002", "Sponsor operations",
     "Outbound transfer is the residual payout inside the closure workflow. Returns, voids and trailing receipts for a closed fund go to its recorded trailing destination (default: the closure residual recipient; otherwise the sponsor's general charitable fund) and never reopen the fund.", "Reopen fund", "Closed means closed", "none"),
 dec("RCD-041", "product", "Revocation time bound", "UNB-RCN-WF-013;RR-UNB-ADV-001-r1;RCF-WF-002", "Security owner",
     "Revocation takes effect at the next authorization check. Sessions, widget tokens and agent tokens are invalidated within 60 seconds.", "Eventual (minutes)", "Prompt, testable bound", "none"),
 dec("RCD-042", "finance", "Unknown payment that cannot obtain NotReceived evidence", "UNB-RCN-WF-022;RCF-WF-003", "Treasury + CFO",
     "After the final aging tier (default 30 days), two independent treasury approvers may resolve it by recorded decision, with a duplicate-risk acceptance and a recovery plan.", "Indefinite escalation", "Prevents permanently stuck payments", "CFO approval of the aging tiers"),
 dec("RCD-043", "product", "Recurring suspension threshold counting a held occurrence", "UNB-RCN-GRT-004;RCF-WF-003", "Product owner", "Suspend when the threshold is reached, including a held occurrence, and skip that occurrence.", "Wait for held occurrence outcome", "Deterministic", "none"),
 dec("RCD-044", "legal", "True sanctions match before grant approval", "UNB-RCN-WF-024;RCF-WF-003", "Counsel",
     "Refuse the grant, release any reservation, produce OFAC rejected-transaction report data within 10 business days, and show the donor a neutral message.", "Hold indefinitely", "No property transferred; reporting per 31 CFR 501.604", "Counsel"),
 dec("RCD-045", "product", "Charity decline of a grant before release", "UNB-RCN-GRT-015;UNB-UI-S-012;RCF-WF-003", "Product owner", "Add a 'Decline this grant' action to S-012, with a reason. Funds return to the originating fund per RCD-040.", "Decline by email to staff", "Self-service", "none"),
 dec("RCD-046", "product", "Default values for workflow timing parameters", "UNB-RCN-WF-002;UNB-RCN-WF-003;UNB-RCN-WF-011;UNB-RCN-WF-016;UNB-RCN-WF-017;UNB-RCN-WF-018;UNB-RCN-WF-021;UNB-RCN-WF-025;UNB-RCN-WF-026;UNB-RCN-WF-028;UNB-RCN-WF-029;UNB-RCN-WF-030;RCF-WF-001;RCF-WF-002;RCF-WF-003", "Sponsor operations + CFO",
     "Defaults (sponsor-configurable, versioned): grant capacity wait 30 days; draft recommendation 30 days; contribution intent lapse 60 days; partial securities close 30 days; invitation expiry 14 days; appeal request window 60 days, response 30 days; destination cooling-off 3 business days; continued-holding re-review 12 months; sanctions disposition SLA 2 business days; funding ShortfallRule 'hold' (no partial payment); positive-pay exception default 'return'.", "Leave unset", "Models need concrete values to be testable", "Sponsor owners confirm defaults"),
 dec("RCD-047", "legal", "Voided check: escheat versus re-credit", "UNB-RCN-GRT-005;RCD-028;RCF-WF-003", "Counsel",
     "An escheat determination step precedes any re-credit; re-credit only where counsel's per-state rule says no escheat duty applies.", "Always re-credit", "Avoids unclaimed-property violations", "Counsel per state"),
 dec("RCD-048", "finance", "ACH Notification of Change versus pinned destinations", "RR-REQ-X-005-r1;UNB-RCN-GRT-017;RCF-WF-003", "Treasury",
     "On a NOC, new releases to the old account are held until the destination update is verified. On a charity-requested change, the old account stays live until the new one activates. Payments already authorized keep their destination.", "Ignore NOC", "Rail rules plus fraud control", "none"),
 dec("RCD-049", "product", "Funding state names", "RR-REQ-X-002-r1;UNB-RCN-FIN-008;RCF-WF-003", "Product owner", "UNB-RCN-FIN-008 names are canonical (FundingPlanned, LiquidationPending, Funded, FundingShortfall). RR-REQ-X-002-r1 is amended to use them; 'approved awaiting capacity' does not exist (approval and reservation are atomic).", "Keep both", "One vocabulary", "none"),
]


def ac(g, w, t):
    return [{"id": "AC-1", "given": g, "when": w, "then": t}]


import re as _re


def expand(parent: str) -> str:
    """'WFM-03-T04,T11-T13; WFM-04-T11' -> explicit transition ids."""
    out = []
    for part in parent.split(";"):
        m = _re.match(r"\s*(WFM-\d\d)-(.*)", part)
        if not m:
            continue
        wf, rest = m.groups()
        for tok in rest.replace(" ", "").split(","):
            r = _re.match(r"T(\d+)(?:-T?(\d+))?$", tok)
            if r:
                a, b = int(r.group(1)), int(r.group(2) or r.group(1))
                out += [f"{wf}-T{i:02d}" for i in range(a, b + 1)]
    return "; ".join(out)


def gap(n, parent, stmt, acs, links, pri="P1", finding="RCF-WF-001"):
    return dict(id=f"UNB-RCN-WF-{n:03d}", origin="gap-analysis", parent=expand(parent), gap_finding=finding,
                why_no_predecessor="Exposed by workflow state-machine validation: the transitions named in 'parent' had no specifying requirement.",
                statement=stmt, rationale="Required for a complete, testable state machine (report 17).",
                evidence=["requirements/reconciliation/2026-11-23/17-WORKFLOW-VALIDATION.md"], acceptance_criteria=acs, links=links, priority=pri)


GAPS = [
 gap(1, "WFM-03-T04,T11-T13; WFM-04-T11,T12", "Each inbound collection rail (ACH debit) and each outbound refund or redelivery of never-accepted property MUST have an Unknown-resolution protocol equivalent to UNB-RCN-GRT-008. Re-initiation after proven NotSubmitted MUST reuse the original idempotency key.",
     ac("an ACH debit request whose provider response times out", "the next attempt is considered", "no new debit is sent until the protocol resolves the first; the same idempotency key is reused"), ["UNB-RCN-GRT-008", "RR-UNB-ACC-006-r1"], "P0"),
 gap(2, "WFM-03-T28; WFM-04-T33", "A contribution intent with no receipt MUST lapse after the sponsor window (default 60 days, RCD-046) and notify the declarant. A receipt after lapse MUST be treated as unannounced.",
     ac("an intent declared 61 days ago with nothing received", "the lapse job runs", "the intent is Lapsed and the donor is notified"), ["RR-UNB-ACC-006-r1"]),
 gap(3, "WFM-04-T06", "A multi-lot securities intent MUST close as partial after the sponsor window (default 30 days) or on donor confirmation. Acknowledgment and Form 8283 then cover only the received lots.",
     ac("600 of 1,000 shares received and 30 days elapsed", "the window closes", "the contribution is ClosedPartial and the acknowledgment describes 600 shares"), ["RR-UNB-ACC-006-r1", "UNB-RCN-FIN-002"]),
 gap(4, "WFM-03-T17,T24,T25", "The sponsor MUST configure acknowledgment timing for reversible rails (OnAcceptance | AfterFinal; default AfterFinal, RCD-032). OnAcceptance requires a void-and-correction path on return.",
     ac("a sponsor with AfterFinal", "an ACH contribution is accepted", "no acknowledgment is issued until Final"), ["RR-UNB-ACC-006-r1", "UNB-RCN-FIN-001"]),
 gap(5, "WFM-05-T18,T19", "Each intermediary agreement MUST record its counsel-approved structure (donee model for release 1, RCD-034), which determines the donee of record and the acknowledgment, Form 8283/8282 and §4943 duties.",
     ac("an intermediary agreement without a recorded structure", "a referral is attempted", "the referral is blocked"), ["UNB-RCN-FIN-004", "REQ-X-059"]),
 gap(6, "WFM-17-T05", "The annual reporting obligation MUST track due date, extension and escalation tiers.",
     ac("a package not signed 30 days before the due date", "the tier is reached", "the work item escalates to the CFO"), ["UNB-RCN-ENT-012"]),
 gap(7, "WFM-17-T18,T19", "An amended annual package MUST pass the same distinct prepare, review and sign states as the original.",
     ac("an amendment", "the preparer attempts to sign", "signing is refused"), ["UNB-RCN-ENT-012"]),
 gap(8, "WFM-18-T09,T10", "A mandatory-legal document whose delivery failed MUST be retried on another verified channel before it is closed as portal-only.",
     ac("an acknowledgment email bounced", "delivery fails", "it is sent by the next verified channel (post) before closing"), ["REQ-X-034", "UNB-RCN-PLT-002"]),
 gap(9, "WFM-01-T17", "Program activation MUST be an A4 sponsor-officer command requiring: a current contract; current board delegation; the legal-sponsor disclosure; a passing tenant isolation suite; tied-out opening balances; and finance readiness certified by a different principal.",
     ac("an isolation suite failure", "activation is attempted", "activation is refused with the failing predicate named"), ["REQ-X-064", "UNB-RCN-PLT-012"], "P0", "RCF-WF-002"),
 gap(10, "WFM-01-T19,T20", "On contract lapse the program MUST suspend new funds, contributions and approvals, while in-flight payments complete under their own workflows.",
     ac("a lapsed contract", "a donor recommends a grant", "the recommendation is refused; an in-flight payment still settles"), ["REQ-X-064"], "P1", "RCF-WF-002"),
 gap(11, "WFM-20-T06", "Rights suspended for incapacity MUST be reinstated only by an A4 reviewer on capacity-restoration evidence, with credentials re-issued through the verified channel.",
     ac("restoration evidence accepted", "the reviewer reinstates", "new credentials are issued; old sessions remain invalid"), ["UNB-RCN-ENT-004"], "P1", "RCF-WF-002"),
 gap(12, "WFM-20-T01", "A protective hold MUST pause standing contribution debits from accounts held by the subject.",
     ac("a death notice for a donor with a monthly ACH debit", "the next debit date arrives", "no debit is initiated"), ["UNB-RCN-ENT-004", "RR-UNB-SUCC-008-r1"], "P0", "RCF-WF-002"),
 gap(13, "WFM-19-T14,T15,T21", "Revocation and expiry MUST take effect at the next authorization check and in no case later than 60 seconds (RCD-041), including sessions, widget tokens and agent tokens.",
     ac("a revoked advisor with an open session", "61 seconds pass", "every request from that session is denied"), ["RR-UNB-ADV-001-r1", "UNB-RCN-PLT-005"], "P0", "RCF-WF-002"),
 gap(14, "WFM-22-T01,T10,T15", "Closure MUST define the policy for contributions received while closure is pending, withdrawal of a donor-requested closure before the residual is committed, and an A4 default residual disposition when the donor gives no recommendation within the policy window.",
     ac("a closure with no residual recommendation after the window", "the window ends", "a default disposition work item opens for A4 approval"), ["UNB-RCN-ENT-006"], "P1", "RCF-WF-002"),
 gap(15, "WFM-22-T12,T14,T17", "Returns, voids and receipts for a Closed or Archived fund MUST route to its recorded trailing destination (RCD-040) and MUST NOT reopen the fund.",
     ac("a grant return for a closed fund", "it is posted", "it credits the trailing destination; the fund stays Closed"), ["UNB-RCN-ENT-006", "UNB-RCN-GRT-015"], "P0", "RCF-WF-002"),
 gap(16, "WFM-24-T14,T15; WFM-01-T18", "Policy versions that depend on a board delegation MUST suspend when the delegation lapses, and routine approval MUST revert to human A4.",
     ac("a lapsed delegation", "a routine grant is evaluated", "the outcome is ApproveRecommended for human approval"), ["RR-AIN-003-r1", "RCD-005"], "P0", "RCF-WF-002"),
 gap(17, "WFM-23-T07,T11,T13", "Work items MUST escalate on SLA breach, MUST be decided only by a principal who is not the preparer and holds authority for the action class, and MUST reopen when execution of the decision fails.",
     ac("a decision whose execution fails", "the failure is recorded", "the work item reopens with the failure attached"), ["RR-REQ-X-048-r1"], "P1", "RCF-WF-002"),
 gap(18, "WFM-25-T09,T13", "Reconsideration MUST have a request window (default 60 days), the outcome set {Upheld, Overturned, Modified, Referred}, a reviewer independent of the original decider, and a response time (default 30 days).",
     ac("an appeal reviewed by the original decider", "they attempt to decide", "the action is refused"), ["UNB-RCN-ENT-016"], "P1", "RCF-WF-002"),
 gap(19, "WFM-02-T14,T16", "Invitations MUST expire after the policy period (default 14 days) and MUST become invalid if the inviter's authority ends before acceptance.",
     ac("an invitation whose inviter was revoked", "the invitee accepts", "acceptance is refused"), ["RR-UNB-PART-007-r1"], "P1", "RCF-WF-002"),
 gap(20, "WFM-26-T10,T17", "If a migration fence outcome is Unknown, Unbound MUST NOT post until the fence is confirmed. A migration run MAY be abandoned before cutover, with staging evidence retained.",
     ac("a fence call that times out", "posting is attempted", "posting is blocked until the fence is confirmed"), ["RR-REQ-X-061-r1"], "P0", "RCF-WF-002"),
 gap(21, "WFM-24-T06-T08", "Each sponsor policy parameter MUST be classified platform-safety or not. Changes to platform-safety parameters MUST receive platform-risk clearance, which grants no sponsor authority.",
     ac("a sponsor change loosening the sanctions SLA", "it is submitted", "it requires platform-risk clearance before approval"), ["REQ-X-045", "UNB-RCN-GRT-014"], "P1", "RCF-WF-002"),
 gap(22, "WFM-10-T10", "Where NotReceived evidence cannot be obtained within the final aging tier, two independent treasury approvers MAY resolve by recorded decision with duplicate-risk acceptance and recovery plan (RCD-042).",
     ac("an Unknown payment 31 days old without evidence", "a single approver attempts resolution", "it is refused; two independent approvers are required"), ["UNB-RCN-GRT-008", "RR-REQ-X-004-r1"], "P0", "RCF-WF-003"),
 gap(23, "WFM-10-T03", "A payment whose destination evidence is ineligible for automated release MAY be released only by an independent A4 treasury approver; holds, sanctions and stale eligibility are never overridable.",
     ac("a payment with an open sanctions hold", "a treasury approver attempts manual release", "release is refused"), ["UNB-RCN-GRT-017", "RR-REQ-X-005-r1"], "P0", "RCF-WF-003"),
 gap(24, "WFM-12-T09; WFM-08-T08", "A confirmed sanctions match without property in possession MUST refuse the transaction and produce OFAC rejected-transaction report data within 10 business days (RCD-044).",
     ac("a true match before approval", "it is confirmed", "the grant is Refused, any reservation released, and a report obligation due in 10 business days exists"), ["UNB-RCN-REG-003", "UNB-RCN-GRT-007"], "P0", "RCF-WF-003"),
 gap(25, "WFM-06-T08; WFM-07-T05,T06,T13; WFM-12-T05; WFM-13-T08", "Draft lifetime, confirmation, re-affirmation, evidence-request, sanctions-disposition SLA and destination-verification windows MUST be versioned policy parameters with owners (defaults RCD-046).",
     ac("a policy version without the sanctions SLA parameter", "it is published", "publication is refused"), ["REQ-X-045"], "P1", "RCF-WF-003"),
 gap(26, "WFM-08-T19", "The CFO MUST approve the default funding ShortfallRule (reduce | waterfall top-up | hold; default hold, RCD-046).",
     ac("no approved ShortfallRule", "a shortfall occurs", "the grant holds and a CFO work item opens"), ["UNB-RCN-FIN-008"], "P1", "RCF-WF-003"),
 gap(27, "WFM-11-T11,T13", "Positive-pay exception pay/return decisions MUST be in the payment authority model, with default Return (RCD-046).",
     ac("an exception not decided by the bank cutoff", "cutoff passes", "the item is returned"), ["UNB-RCN-GRT-005"], "P1", "RCF-WF-003"),
 gap(28, "WFM-13-T12", "Destination activation MUST follow a cooling-off period (default 3 business days) and notify both the previous and the new verified channels.",
     ac("a verified new destination", "it is activated", "both contacts were notified at least 3 business days earlier"), ["UNB-RCN-GRT-017"], "P0", "RCF-WF-003"),
 gap(29, "WFM-08-T12,T13", "A grant that fails capacity at approval MUST wait in CapacityShort for at most the sponsor CapacityWaitWindow (default 30 days), then be declined with a reason.",
     ac("a grant 31 days in CapacityShort", "the window ends", "the grant is declined with reason 'insufficient funds'"), ["RR-REQ-X-002-r1", "UNB-RCN-FIN-009"], "P1", "RCF-WF-003"),
 gap(30, "WFM-21-T07", "A fund under continued holding after dormancy outreach MUST be re-reviewed every 12 months (default, RCD-046).",
     ac("a fund in ContinuedHolding for 12 months", "the review date arrives", "a re-review work item opens"), ["UNB-RCN-ENT-005"], "P2", "RCF-WF-002"),
]
FINDINGS = [
 dict(finding_id="RCF-WF-001", severity="high", dimension="completeness", affected_ids="RR-UNB-ACC-006-r1;UNB-NAV-008;UNB-RCN-FIN-008;REQ-X-032;REQ-X-040", evidence="requirements/reconciliation/2026-11-23/workflows/REPORT-FIN.md",
      title="Finance workflow validation: missing exits and conflicting rules", description="Modelling WFM-03/04/05/14/15/16/17/18 found an acknowledgment-timing conflict, residual provisional credit, no Unknown protocol for inbound money, an undecided intermediary role, human checking of every NAV, an undecided fee-versus-reservation priority, and unspecified intent expiry and partial receipt.", impact="Untestable or contradictory state machines", proposed_resolution="RCD-032..038 and UNB-RCN-WF-001..008", needs_human_decision="Y", decision_owner="Sponsor CFO / counsel", prior_ids="OD-04;OD-10"),
 dict(finding_id="RCF-WF-002", severity="high", dimension="completeness", affected_ids="RR-REQ-X-008-r1;REQ-X-064;REQ-X-045;UNB-RCN-ENT-006;UNB-RCN-ENT-016;RR-REQ-X-048-r1;RR-UNB-ADV-001-r1;RR-REQ-X-061-r1", evidence="requirements/reconciliation/2026-11-23/workflows/REPORT-ENT.md",
      title="Enterprise workflow validation: dead ends, missing lifecycles and stale rules", description="Modelling WFM-01/02/19-26 found a stale death rule (contradicting RCD-022), a closure-versus-transfer order conflict, no routing for money arriving at closed funds, no revocation time bound, no reinstatement after incapacity, no work-item escalation or reopen, no migration fence-Unknown state, and no activation gate rule.", impact="Funds or people stuck in states with no exit", proposed_resolution="RCD-039..041 and UNB-RCN-WF-009..021, 030", needs_human_decision="Y", decision_owner="Sponsor operations", prior_ids="OD-03;OD-11"),
 dict(finding_id="RCF-WF-003", severity="high", dimension="completeness", affected_ids="RR-REQ-X-002-r1;RR-REQ-X-005-r1;UNB-RCN-GRT-004;UNB-RCN-GRT-005;UNB-RCN-GRT-015;UNB-RCN-GRT-017;UNB-UI-S-012", evidence="requirements/reconciliation/2026-11-23/workflows/REPORT-GRT.md",
      title="Grants and payments workflow validation: stuck states and contradictions", description="Modelling WFM-06..13 found approval-without-reservation in the prior model, conflicting funding state names, Unknown payments that can never close, a recurring threshold ambiguity, no rule for a true sanctions match before approval, a void-versus-escheat conflict, a NOC-versus-pinned-destination conflict, and no screen for a charity to decline.", impact="Duplicate-payment risk; payments stuck in Unknown", proposed_resolution="RCD-042..049 and UNB-RCN-WF-022..029", needs_human_decision="Y", decision_owner="Treasury / counsel", prior_ids="TVF-024"),
]
rows = list(csv.DictReader(open(f"{D}/decisions.csv")))
rows = [r for r in rows if r["decision_id"] < "RCD-032"] + NEWD
with open(f"{D}/decisions.csv", "w", newline="") as f:
    w = csv.DictWriter(f, fieldnames=H, quoting=csv.QUOTE_ALL); w.writeheader(); w.writerows(rows)
json.dump({"gaps": GAPS, "findings": FINDINGS}, open("/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad/own/wf_records.json", "w"))
print(len(rows), len(GAPS), len(FINDINGS))

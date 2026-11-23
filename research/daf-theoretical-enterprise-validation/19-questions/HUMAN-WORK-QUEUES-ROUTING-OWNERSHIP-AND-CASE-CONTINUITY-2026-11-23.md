# Human work queues, assignment routing, and cradle-to-grave ownership
Date: 2026-11-23. Proposed requirements only; not implemented or tested.

## Objective
Provide a shared, sponsor-scoped Human Work Orchestration component within the Unbound component monolith for grant processing, charity vetting and other authorized exception workflows. Automatically assign only work that genuinely needs a person. Keep routing separate from decision authority and protective controls. Support round robin, dedicated accounts, cradle-to-grave ownership, specialist routing, capacity-aware balancing and hybrid models.

## Key distinctions
**Relationship owner** manages sponsor/fund/account continuity; **case owner** coordinates a grant or charity review end to end; **task assignee** performs one bounded step; **decision authority** independently authorizes a regulated or financial decision; **backup owner** covers absence; **queue supervisor** manages assignments but does not thereby gain grant/payment/sanctions authority. These roles may coincide only when permitted by segregation of duties. Grant ownership does not mean charity eligibility is sponsor-private: platform-wide shared charity vetting can serve multiple sponsors while sponsor-specific grant restrictions remain separate.

## Routing strategies
- RoundRobin: deterministic rotation across eligible active staff; durable cursor per queue/sponsor/team and fair treatment of concurrent intake.
- StickyAccountOwner: sponsor, fund, donor account, wealth-advisor portfolio or assigned relationship book maps to owner, with explicit inheritance and fallback.
- CradleToGrave: one named case owner remains responsible from submission through settlement/closure, while independent approvals and specialized tasks may route elsewhere.
- SkillBased: charity due diligence, foreign grants, high-risk reviews, complex assets, language/accessibility and payment investigations route to qualified people.
- CapacityWeighted: eligibility, available hours, active weighted workload, case complexity, SLA, timezone, absences and priorities; avoid overloading fastest workers.
- Hybrid: relationship owner as default; specialists for gated tasks; return work to original owner after specialist completion.
- ManualAssignment: permitted supervisor override with reason, policy validation, audit and no self-approval loophole.
Sponsor-level configuration selects strategies by queue and case class, subject to platform controls.

## Typed entities
WorkQueue {QueueId; SponsorScope; Domain; EligibleTeams; RoutingPolicyVersion; SLA; FallbackQueue}.
Worker {UserId; TeamIds; Skills; SponsorAuthorizations; Capacity; Schedule; Absence; Conflicts; Status}.
OwnershipBinding {ScopeType; ScopeId; PrimaryOwner; BackupOwner; EffectiveFrom; EffectiveTo; Reason; PolicyVersion}.
WorkItem {ItemId; SourceDomain; SubjectRef; SponsorId; CaseId; TaskType; RequiredSkills; Risk; Priority; Deadline; CreatedAt; AssignmentVersion; State; ProtectiveGateRefs}.
AssignmentDecision {EligibleWorkerSetDigest; RoutingStrategy; CursorOrScore; SelectedWorker; Explanation; At; PolicyVersion; AuditRef}.
AuthorityCheck {DecisionType; Sponsor/FundScope; RequiredIndependentApprovers; Evidence; Validity}; assignment does not satisfy this check.

## Work lifecycle
NeedsHuman -> Classified -> Queued -> Assigned -> Accepted/InProgress -> WaitingForEvidence/External/IndependentApproval -> ReadyForVerification -> Completed -> SourceVerified -> Closed. Branches: Unassigned, Declined, Reassigned, Escalated, TimedOut, Canceled, Reopened. Distinguish owner and assignee; closing a task cannot approve a grant or release a charity/payment hold. Assignment and claim operations are atomic with version checks to prevent double claims. Durable rotation cursors prevent bias during concurrency; do not promise mathematically perfect fairness across differing complexity.

## Account ownership and continuity
Dedicated account ownership may be mapped by sponsor, fund, account, donor relationship or portfolio. Define precedence and conflict resolution when one fund has multiple advisers or an account moves between sponsors. Cradle-to-grave case ownership follows grant lifecycle from recommendation through approval, payout, return/correction and final settlement, with explicit case transitions. For charity vetting, ownership follows shared charity review and reverification lifecycle; sponsor-specific grant cases reference the shared verified record. Reassignment preserves original owner history, open tasks, evidence and customer communications.

## Staff eligibility and workload
Eligible only when authorized for sponsor scope, qualified for task, active, not on leave, within capacity and free of relevant conflict. Support vacation schedules, holidays, timezones, shift coverage, emergency backup and automatic reassignment on departure. Capacity score can consider estimated complexity, not just count. A dedicated owner may retain relationship ownership while operational tasks temporarily go to a backup. A stalled case should trigger an SLA alert and escalation, not automatic financial approval.

## Fairness and operational economics
Expose workload distribution by complexity, average handling time, aging, rework, reassignment, outcomes and quality, without using unadjusted raw speed to rank employees. Fairness is secondary to eligibility, safety and deadlines. Staff should be able to see why a task was routed to them and supervisors why another worker was excluded. Prevent cherry-picking, starvation of complex cases, and hidden manual overrides.

## UI requirements
Work Queue dashboard: My Work, My Accounts, Team Queue, Unassigned, Waiting on Others, At Risk, Completed. Filter by sponsor, fund, charity, grant, priority, deadline, assignee, age, stage and required skill. Case detail: owner, task assignees, full timeline, evidence, notes, grant/charity status, next best action, approvals, holds, SLA and reassignment history. Queue Supervisor console: strategy editor, round-robin order, eligibility preview, staffing schedules, workload/capacity, reassignment, exceptions, overrides and audit. Sponsor settings configure ownership and routing where allowed. Accessible/mobile, no blank loading state, privacy-safe views. Avoid showing confidential sanctions details to ordinary grant staff.

## AI and automation boundary
AI can classify work, estimate complexity and recommend routing only from permitted facts; deterministic rules must enforce eligibility and sponsor scope. AI cannot independently assign a legally restricted approval role, authorize payment, clear sanctions holds or invent capacity. Automate routine charity reverification and routine grants first, so queues contain genuine unresolved work rather than every transaction.

## Integration
Grant, Charity Vetting, Compliance, Payments, Fund Accounting and Exception Management create typed work requests and retain authoritative domain state. Work Orchestration owns assignments and queue history, not financial decisions. Decision Rights checks authority at actual approval/execution. Communications handles task notifications; Operational Learning detects routing bottlenecks. Sponsor Policy Studio versions routing rules. Retention Policy preserves financially material work records at least seven years.

## Acceptance criteria
1. Concurrent round-robin intake never assigns the same item twice; cursor and eligible roster are auditable.
2. Dedicated fund/account ownership persists across multiple grants unless policy or reassignment changes.
3. Cradle-to-grave case owner remains accountable even while a specialist performs a bounded task.
4. Staff cannot receive tasks for unauthorized sponsors or restricted cases.
5. Vacation, departure and overload trigger policy-governed coverage without losing ownership history.
6. Assignment does not confer grant, sanctions, financial close or payment approval.
7. Shared charity vetting can serve multiple sponsors without leaking sponsor-private grant details.
8. SLA escalation does not release holds or bypass approvals.
9. Staff workload, deadlines, reasons and next actions are visible to permitted roles.
10. Routing changes and manual overrides preserve immutable audit evidence and financial seven-year retention.

## Adversarial scenarios (proposed, unexecuted)
- ADV-619: round-robin cursor lost on concurrent intake; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-620: ineligible worker selected because of lowest workload; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-621: account owner on vacation with no backup; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-622: dedicated owner leaves organization; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-623: same case assigned to two workers; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-624: fund reassigned but open grants lose continuity; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-625: one charity shared across sponsors leaks donor data; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-626: charity vetting owner mistaken for sponsor grant approver; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-627: case owner approves own restricted exception; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-628: queue supervisor bypasses sanctions hold; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-629: capacity balancing starves complex grants; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-630: manual override hides conflict of interest; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-631: staff timezone causes SLA breach; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-632: high-priority cases endlessly preempt ordinary work; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-633: AI routing recommendation ignores sponsor authorization; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-634: work item marked done while payment remains Unknown; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-635: grant case closed before settlement; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-636: charity reverification expires mid-review; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-637: specialist task never returns to case owner; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-638: worker declines case and it disappears; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-639: round robin unfair due to variable case complexity; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-640: absence triggers duplicate customer notifications; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-641: reassignment overwrites audit history; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-642: case notes accessible to wrong sponsor; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-643: worker can view restricted compliance evidence; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-644: manual owner binding conflicts with fund policy; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-645: customer-facing owner changes without continuity; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-646: shared charity vetting duplicated per sponsor; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-647: staff departure leaves orphaned tasks; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-648: routing policy changes mid-case without version pin; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-649: urgent queue bypasses independent approval; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-650: financially material assignment history purged early; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-651: task reopened without restoring accountability; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-652: queue performance optimized by suppressing human-review flags; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-653: work item claimed from stale queue projection; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.
- ADV-654: fallback queue has no eligible worker; expected: authorized deterministic assignment, durable continuity, safe escalation and auditable source-state verification.

## Open decisions
Which account levels support sticky ownership (sponsor, fund, donor, advisor), how to prioritize mixed rules, default staffing capacity model, acceptable supervisor override scope, whether donors can see assigned staff, and which teams may share a charity review. No actual implementation or tests executed.

Status: proposed. ADV-619..ADV-654 unexecuted.

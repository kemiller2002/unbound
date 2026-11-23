# Work Queue Management dashboards, role-based UX and reporting
Date: 2026-11-23. Proposed requirements, not implemented or tested.

## Scope and design intent
Make Work Queue Management a first-class area of Unbound's dashboard/report phase, integrated with REQ-X-048, REQ-X-041 and the enterprise operational intelligence semantic layer. Do not build an isolated ticketing product or second approval system. Work items are actionable projections of authoritative grant, charity vetting, payment, compliance and exception states. The UI must minimize unnecessary human touches while preserving independent authorization.

## Personas and primary decisions
- **Individual reviewer:** What requires my action now, what evidence is missing, what can I safely complete, and what must I escalate?
- **Cradle-to-grave case owner:** Which grants and accounts am I accountable for, what specialists are handling subtasks, and where is a case blocked?
- **Team supervisor:** Who is overloaded or unavailable, which cases risk deadlines, where should reassignment occur, and are routing policies fair?
- **Sponsor administrator:** What is the sponsor's processing status and service performance, without exposure to platform-private compliance details?
- **Compliance/finance approver:** Which decisions require my independent authority, what evidence supports the decision, and what conflicting duties must block it?
- **Platform operations:** Are queues healthy across permitted sponsor scopes, are provider dependencies causing backlogs, and where should workflow redesign be considered?

## Navigation and dashboard sections
1. **My Work:** Actionable Now, Due Soon, Waiting on Others, Escalated, Returned, Completed. Show case type, sponsor/fund (authorized), risk, deadline, owner, task assignee, status, next best safe action and evidence completeness. Saved filters and keyboard-accessible bulk triage; never bulk-approve restricted decisions.
2. **My Accounts:** Sticky sponsor/fund/account ownership, open grant journeys, charity reviews, follow-ups, unresolved payment status, coverage owner and recent communication history.
3. **Team Queue:** Unassigned, Assigned, Blocked, At Risk, Overdue, Specialist Review, Waiting External, Reopened, Closed. Distinguish queue aging from time waiting on a provider or donor.
4. **Supervisor Command Center:** Staffing/absence calendar, eligible workers, skill coverage, weighted workload, routing policy, round-robin cursor state, assignment history, SLA heatmap, safe reassignment preview and escalation.
5. **Case Workspace:** Persistent case header, ownership and task responsibility, state timeline, financial/control status, evidence panel, typed notes, restricted information boundaries, decision authority, related cases, next steps and communication history.
6. **Routing Configuration:** Sponsor- and queue-specific strategy selection, sticky ownership precedence, fallback, capacity, skills, policy simulation, effective date, approvals, audit and rollback.
7. **Performance & Learning:** Arrival/completion rates, median/p90 aging, time by stage, weighted capacity, reassignment, rework, false holds, escalations, safe automation, downstream outcomes and recurring root causes.

## Interaction requirements
- Search and filter by sponsor, fund, account, charity, grant, reviewer, team, state, priority, deadline, skill, origin and policy version subject to permissions.
- Claim/assign/reassign with optimistic concurrency and explanatory outcome; no duplicate claim or lost task.
- Show why an item is assigned, why another worker is ineligible, and who remains accountable after specialist handoff.
- Persistent contextual action panel distinguishes **View**, **Request Evidence**, **Complete Task**, **Recommend**, **Approve** and **Execute** with independent domain authorization.
- Support accessible mobile layout, screen reader, keyboard operation, high contrast, clear loading/empty/error/Unknown states and non-color-only priority cues.
- Allow saved views, user-customizable density and ordering within mandatory safety visibility rules. No hidden financial holds or required approvals.
- Display task age and elapsed business time distinctly; use sponsor-local timezone/calendar and visible cutoff definitions.
- Avoid overwhelming reviewers with dashboards; default to a prioritized actionable list and one-case focus mode.

## Metrics and reports
Define typed metric contracts with source, numerator, denominator, cohort, known-at/effective-at, freshness, coverage and privacy.
- Work arrival, completion and backlog by task type and sponsor.
- Median and p90 handling time, elapsed time, active work time and external waiting time.
- Weighted workload per eligible FTE, skill coverage, unassigned age, absences and reassignment frequency.
- Cradle-to-grave continuity rate and specialist handoff count.
- Deterministic completion rate versus human-required cases, avoiding false automation attribution.
- Grant recommendation-to-approval and approval-to-settlement, with accurate provider Unknown state.
- Charity vetting 180-day refresh timeliness, shared-evidence reuse and adverse hold propagation.
- Reopen/rework rate, preventable exception incidence, false holds, unauthorized action attempts and SLA breaches.
- Report catalog: My Work Summary, Team Backlog and SLA, Staffing and Capacity, Account Ownership, Grant Lifecycle Bottlenecks, Charity Vetting Timeliness, Exception Root Causes, Supervisor Overrides and Audit.
Reports are scoped, versioned and exportable under sponsor/privacy rules; financial evidence inherits seven-year retention and legal holds.

## Data and authority
Queue projections are eventually consistent; show authoritative domain status and last-refreshed watermark when action is contemplated. Every action rechecks authorization, source state, delegation, holds and assignment version. A queue task completion cannot settle a grant or clear sanctions. Sponsor administrators see only their tenant; shared charity vetting does not expose other sponsors' donor information. Use shared metric registry, document evidence and retention engine; no duplicate ledger or approval store.

## Layout wireframe requirements (conceptual)
Desktop: top status/freshness strip; left saved queue filters; center prioritized task list; right contextual next-action and evidence preview; supervisor mode swaps right panel for capacity/assignment insights. Mobile: status summary, filter sheet, task cards and full-screen case focus. Keep global navigation consistent with other Unbound dashboards. A dashboard must remain useful with zero cases, high volume, stale data and offline/degraded modes.

## Adversarial UX scenarios (proposed, not executed)
- ADV-655: unassigned critical case omitted by default filter; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-656: stale queue card enables payment release; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-657: case owner confused with independent approver; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-658: reassignment hides cradle-to-grave accountability; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-659: worker on leave appears eligible; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-660: round-robin cursor appears current despite lag; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-661: supervisor bulk action bypasses segregation; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-662: cross-sponsor search exposes case title; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-663: shared charity record leaks other sponsors' grants; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-664: provider Unknown displayed as failed; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-665: closed task displayed as settled grant; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-666: SLA clock ignores sponsor holiday; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-667: complex cases make staff seem unproductive; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-668: queue count improves by hiding exceptions; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-669: mobile screen conceals legal hold; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-670: screen reader cannot reach reassignment control; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-671: stale claim results in duplicate assignee; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-672: restricted sanctions evidence appears in notes; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-673: case waiting on provider counted as staff idle; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-674: dashboard refresh changes ordering during approval; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-675: export includes unauthorized sponsor columns; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-676: report uses new metric formula on old periods; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-677: staffing heatmap ignores skill eligibility; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-678: fallback queue has no active worker; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-679: specialist subtask orphaned after reassignment; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-680: high-risk queue sorted behind trivial tasks; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-681: AI recommended action rendered as authorized; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-682: task reopened without visible history; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-683: user hides mandatory approval status in saved view; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-684: case list loads blank without status; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-685: sponsor-admin dashboard exposes platform incident; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.
- ADV-686: bulk notification reveals sensitive case metadata; expected: safe UI states, authoritative action checks, accurate metrics, accessibility and sponsor isolation.

## Acceptance criteria
1. My Work and Team Queue separate actionable cases from external waiting and automated completions.
2. Account owners retain end-to-end visibility during specialist routing.
3. Independent decision authority is explicit and cannot be inferred from assignment.
4. Queue action validates current authoritative state, holds, permissions and assignment version.
5. Supervisors can inspect eligibility, capacity, fairness and reassignment reasons.
6. Metrics distinguish queue count from actual harm, and handling time from provider waiting.
7. Views and reports are tenant-scoped, accessible, auditable and versioned.
8. Dashboard/report phase explicitly includes this module and its report catalog.

## Follow-up dashboard work
Integrate into full role-based Unbound information architecture and widget catalog. Define shared Forma components for queue tables, evidence timelines, assignment previews, SLA indicators, approval gates and case cards. Validate with realistic mock data and adversarial user journeys before implementation. No tests performed.

Status: proposed. ADV-655..ADV-686 unexecuted.

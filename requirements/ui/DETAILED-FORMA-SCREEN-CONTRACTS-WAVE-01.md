# Detailed Forma Screen and Interaction Requirements, Wave 1
2026-11-22 | **Requirements only; no implementation authorization.**

## Design intent
Unbound should feel like a decision and outcome system, not a departmental form collection. Organize around the DAF account and the sponsor's responsibilities, show next authorized action, distinguish evidence from inference, and make uncertainty visible. Preserve sponsor asset ownership and donor recommendation rights.

## Twelve screen contracts
### UX-S01: Sponsor Command Center
- **Roles:** Sponsor executive; operations administrator
- **Information hierarchy:** Material exceptions, decisions due, funding/eligibility holds, service outcomes, evidence freshness
- **Primary actions:** Review queue; assign; escalate; inspect risk; open scoped DAF
- **Exceptions and boundary states:** Missing evidence vs confirmed failure; permissions and legal holds
- **Invariant:** No unsupported global health score
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S02: DAF Account Overview
- **Roles:** DAF administrator; donor; advisor
- **Information hierarchy:** Identity and sponsor ownership, participants, grants, contributions, investments, cases, successor plan
- **Primary actions:** Recommend grant; request changes; review documents; open case
- **Exceptions and boundary states:** Recommendation vs approval vs payable vs settlement
- **Invariant:** Never imply donor asset ownership
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S03: Participant Manager
- **Roles:** DAF administrator; sponsor-authorized delegator
- **Information hierarchy:** People, legal identities, role scopes, effective dates, invitation status, conflicts
- **Primary actions:** Invite; change scope; revoke; review access
- **Exceptions and boundary states:** Role conflict, expired delegation, multi-DAF membership
- **Invariant:** Unlimited participants and multiple concurrent advisors
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S04: Succession Designer
- **Roles:** Donor; successor specialist; sponsor reviewer
- **Information hierarchy:** Successor DAF graph, percentages, contingent triggers, generations, legal uncertainty
- **Primary actions:** Add branch; split; simulate; request review; approve within authority
- **Exceptions and boundary states:** Unallocated shares, contradictory contingencies, death/incapacity evidence
- **Invariant:** Graph and accessible textual equivalent
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S05: Case Workspace
- **Roles:** Operations; grants; compliance; finance
- **Information hierarchy:** Outcome, reason, affected DAF, evidence timeline, agent proposal, authority, deadline
- **Primary actions:** Resolve; request evidence; escalate; assign; communicate
- **Exceptions and boundary states:** Disputed facts, overdue evidence, agent uncertainty
- **Invariant:** No self-review or hidden closure
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S06: Grant Recommendation
- **Roles:** Donor; advisor; grants specialist
- **Information hierarchy:** Purpose, recipient, eligibility, amount basis, funding sources, status
- **Primary actions:** Draft; simulate; submit; revise; withdraw
- **Exceptions and boundary states:** Stale valuation, restricted recipient, approval invalidation
- **Invariant:** Approval and cash settlement separated
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S07: Contribution Intake
- **Roles:** Donor; contributions specialist
- **Information hierarchy:** Gift intent, asset type, acceptance criteria, custody, valuation, acknowledgment
- **Primary actions:** Submit offer; request diligence; accept under authority
- **Exceptions and boundary states:** Noncash restrictions, incomplete identity, unsupported value
- **Invariant:** No receipt conflated with acceptance
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S08: Partner Administration
- **Roles:** Partner administrator; sponsor governance
- **Information hierarchy:** Partner scope, inherited sponsor rules, local branding, delegated tasks
- **Primary actions:** Configure permitted options; invite staff; review cases
- **Exceptions and boundary states:** Cross-tenant exposure, conflicting sponsor policies
- **Invariant:** Sponsor-reserved policies not editable
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S09: Account Establishment
- **Roles:** Prospective donor; onboarding specialist
- **Information hierarchy:** Progress, necessary evidence, sponsor review, contribution, agreements
- **Primary actions:** Save/resume; supply documents; submit; correct
- **Exceptions and boundary states:** Duplicate account, abandoned intake, inaccessible forms
- **Invariant:** Do not collect unnecessary information
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S10: Document and Communication Center
- **Roles:** All authorized participants
- **Information hierarchy:** Requested documents, consent, messages, delivery state, accessible output
- **Primary actions:** Upload; preview; sign; send; request alternative format
- **Exceptions and boundary states:** Wrong recipient, stale address, private attachment
- **Invariant:** Accessible Folio artifacts and privacy-safe notices
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S11: Agent Work and Oversight
- **Roles:** Sponsor operations; AI governance
- **Information hierarchy:** Agent proposal, evidence, tool scopes, confidence limits, approval boundary
- **Primary actions:** Approve permitted action; reject; request sources; pause agent
- **Exceptions and boundary states:** Prompt injection, unsupported claims, attempted escalation
- **Invariant:** No agent self-approval
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.

### UX-S12: Financial Operations
- **Roles:** Controller; investment and treasury operations
- **Information hierarchy:** NAV, ledger, reconciliation, grants payable, custodian breaks, deferred payments
- **Primary actions:** Review breaks; approve corrections; simulate instructions
- **Exceptions and boundary states:** Unbalanced journal, mismatched source, live rail attempted
- **Invariant:** Independent evidence and no live payments
- **Responsive requirement:** On mobile, lead with status, next action and material exceptions; secondary history and data behind accessible disclosure. On desktop, expose contextual evidence alongside action without changing permissions.
- **States:** Loading, Empty, Draft, Ready, PendingApproval, Blocked, Stale, Error, Completed, where applicable. A state is not shown as Completed until the authoritative system confirms it.


## Shared interaction contracts
- **INT-001 Global context selector:** Sponsor, partner, program, DAF and role scopes must be visible and reauthorized on change.
- **INT-002 Decision card:** Show objective, material impact, evidence, policy, actor authority and next action.
- **INT-003 Evidence provenance:** Reveal observed facts, inferred conclusions, source date and uncertainty separately.
- **INT-004 Agent proposal:** Explain proposed action, sources, limits and whether human approval is required.
- **INT-005 Undo semantics:** Only reversible drafts may undo; consequential posted events require correction/reversal.
- **INT-006 Optimistic updates:** Do not show final success until authoritative command result and evidence confirmed.
- **INT-007 State transitions:** Show prior, current and permitted next states with reason and authorization.
- **INT-008 Cross-account search:** Scope by effective authority, avoid leaking hidden record existence.
- **INT-009 Responsive layout:** Phone-first progressive disclosure; tablet/desktop additional density without lost functionality.
- **INT-010 Accessible visualizations:** Every graph and relationship visualization has keyboard and structured text alternatives.
- **INT-011 Loading and errors:** No blank WASM shell; skeleton with progress and recoverable errors.
- **INT-012 Offline behavior:** Read cached data only with stale label; draft work allowed; consequential actions reauthorized online.
- **INT-013 Confirmation design:** Consequential actions summarize exact affected entities and require current authority.
- **INT-014 Bulk actions:** Preview population, excluded rows, partial failure behavior and idempotency.
- **INT-015 Embedded widgets:** Scoped read-only defaults, token expiry, origin checks and no inherited host authority.
- **INT-016 Audit parity:** Forma and Folio use same snapshot, period, scope and status semantics.
- **INT-017 Notifications:** No sensitive content in push/email previews; consent and delivery tracking.
- **INT-018 Accessibility accommodations:** Alternative formats and representative assistance without inferring incapacity.
- **INT-019 Design tokens:** Balanced Charcoal and Bronze, component consistency and contrast verification.
- **INT-020 Usability measurement:** Task completion, effort, errors, recontact, abandonment and comprehension.

## Design tensions and resolution
- **AI-native versus autonomy:** Agents may reduce manual research and prepare cases, but authority must be explicit at the final decision boundary.
- **Simplified UI versus financial precision:** Use progressive disclosure, never hide valuation time, funding sources, approval status or reconciliation breaks when relevant.
- **Multiple advisors versus clear responsibility:** Distinct identities and scoped roles; avoid the fiction of a single controlling advisor.
- **Embeddability versus security:** Widget context is independently authorized and revocable; host application cannot silently inherit privileged action authority.
- **Personalization versus accessibility:** Offer alternate formats, large targets, contrast and reduced motion without inferring disability or reducing privileges.
- **Speed versus evidence:** Optimistic drafting is permitted; no optimistic claims that sponsor approval, payment or posting succeeded.

## Design system and integration
Use F#, Limen WASM, Forma components and Folio documents after actual capabilities are verified. Design language follows approved Echelon Balanced Charcoal and Bronze standards. No framework implementation, deployment or database migration is authorized.

## Review requirements
Each screen needs role-based task walkthroughs, content hierarchy, state transition diagram, API and event dependency list, evidence and authority checklist, accessibility acceptance, mobile-first behavior, widget extraction decision and failure-recovery scenarios. Exact interaction designs are proposed, not user-tested.

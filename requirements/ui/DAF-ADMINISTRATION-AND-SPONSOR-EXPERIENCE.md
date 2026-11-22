# DAF Administration and Sponsor Operations: Forma Experience
2026-11-22 | UI requirements only; no implementation.

## Experience objective
Create an exceptionally clear administrative experience where every person sees the next authorized action, its consequences, evidence and status. Reduce unnecessary handoffs and form fields. Preserve sponsor authority and donor dignity. Avoid a traditional grid-heavy back office unless a data table materially improves a task.

## Role-specific experiences
### Sponsor Operations Administrator
Workspace: Sponsor-wide queues, configuration, staffing, service levels and operational exceptions. Authority boundary: Cannot waive legal obligations or self-approve reserved financial decisions.

### DAF Account Administrator
Workspace: Participant access, account details, servicing, documents, grants and contribution status. Authority boundary: Cannot grant sponsor-only authority or alter ledger history.

### Donor / Primary Advisor
Workspace: Intent, recommendations, account activity, documents, authorized invitations and successor wishes. Authority boundary: Cannot trade or move sponsor-owned assets.

### Additional Advisor
Workspace: Scoped recommendations, account visibility and delegated actions. Authority boundary: No authority beyond effective role grant.

### Successor / Contingent Successor
Workspace: Future interests, required evidence and authorized transition actions. Authority boundary: No present rights unless separately granted.

### Partner Administrator
Workspace: Partner-specific service operations, branding and delegated workflows. Authority boundary: No cross-partner data or sponsor-reserved decisions.

### Sponsor Executive
Workspace: Enterprise service, risk, economics, escalations and governance. Authority boundary: Visibility not equivalent to transaction approval.

### Grants Specialist
Workspace: Grant case queue, eligibility evidence, recipient communication. Authority boundary: Cannot replace sponsor grant approval.

### Contributions Specialist
Workspace: Contribution intake, diligence, noncash cases and acknowledgments. Authority boundary: Cannot accept restricted assets without required authority.

### Compliance / Legal Reviewer
Workspace: Identity, legal rules, conflicts, exceptions and escalation. Authority boundary: Legal conclusions require qualified review.

### Finance / Investment Operations
Workspace: Reconciliations, fees, positions, NAV and accounting impact. Authority boundary: No unilateral donor-instructed trade/payment.

### Auditor / Assurance Reviewer
Workspace: Evidence, case history, control results and scoped reports. Authority boundary: Read-only by default, independent of operations.

### AI Servicing Agent
Workspace: Gather facts, classify, propose next action and draft communications. Authority boundary: No reserved approvals, legal publication, trading or money movement.


## Screen inventory
Sponsor Command Center; DAF Portfolio; DAF Account Overview; Establish Account; Participant and Delegation Manager; Successor Plan Designer; Contribution Intake; Noncash Gift Review; Grant Recommendations; Investment Preferences; Account Funding; Service Case Workspace; Exception and Conflict Resolution; Documents and Communications; Sponsor Policy Studio; Partner Administration; Fee and Billing Review; Account Closure; Operational Analytics; Audit Trail and Evidence; Agent Work Queue; Widget Management.

## Key interactions
- **Sponsor Command Center:** prioritized actions, unresolved exceptions, accounts waiting for activation, funding holds, aging cases, independent approvals and evidence freshness; clear ownership and deadline.
- **DAF Account Overview:** account status, sponsor, program, participant graph, current roles, contributions, grant recommendations, investments, successor plan, cases, statements and next steps; do not imply participant asset ownership.
- **Account establishment:** adaptive progressive intake with save/resume, evidence requests, identity and sponsor acceptance checkpoints; visible reason for each required input.
- **Participant management:** graphical role relationships and effective dates, conflict previews, delegated scopes, invitation acceptance and revoke confirmation.
- **Succession designer:** multiple successor DAF branches, split percentages, contingencies, generations, unresolved legal conditions and approval checkpoints; visual preview plus accessible textual equivalent.
- **Case workspace:** unified timeline, evidence, agent proposal, risk, authorization boundary, recommended next action, SLA and communication.
- **Partner console:** inherited sponsor policy vs partner-configurable fields clearly distinguished; preview changes and effects.
- **Audit detail:** source provenance, immutable correction history, owner/reviewer separation and independent evidence.

## Widgets for internal and external hosts
DAFAccountSummary, MyDAFActions, ParticipantPermissions, SuccessorPlanStatus, ContributionStatus, GrantRecommendationStatus, GrantEligibilityHold, ServiceCaseProgress, DocumentRequests, AdvisorWorkQueue, SponsorOperationalRisk, PartnerPortfolioSummary. Each has typed versioned manifest, allowed filters, authorization scopes, tenant/sponsor/account context, accessible state, stale and offline behavior, minimum-data defaults and safe drilldown. Embeds use short-lived audience-bound tokens and never trust the host page for reserved approvals.

## Accessibility and performance
WASM-first Limen with Forma, Folio for accessible reports, mobile-first WCAG 2.2 AA target, keyboard/screen-reader navigation, semantic status announcements, high-contrast themes, reduced motion, generous touch targets, no blank initial load, skeleton/meaningful loading, explicit error and offline states, save/resume, clear privacy notices. Balanced Charcoal and Bronze design standards after approved design-system verification. No assumed integrations.

## User research requirements
Test representative donor, advisor, successor, sponsor admin, grants specialist, finance, compliance, partner and auditor tasks. Include users with visual or motor accessibility needs. Measure task completion, error recovery, comprehension of sponsor ownership, authority confusion, unnecessary fields, repeated contact and trust in AI explanations. A visually polished screen is not accepted without task evidence.

## Guardrails
Show recommendation versus approval versus payable versus settlement as distinct states. Avoid single-click consequential actions without confirmation and current authority. Never expose one partner's information in another partner's session. AI suggestions must be labeled, sourced and editable; no fabricated completion state. All tests remain requirements, not executed.

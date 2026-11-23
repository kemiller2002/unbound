# DAF Administration and Sponsor Operations Requirements
2026-11-22 | Requirements and research only. No implementation authorized.

## Product thesis
Administer donor-advised funds through outcomes, events and policy-bound authority, not by digitizing legacy departmental handoffs. Sponsor owns and controls assets; participants recommend. AI prepares, triages and explains but does not acquire reserved authority. Preserve append-only history and independent financial/legal controls.

## Operating boundaries
Sponsor-wide governance and operations, partner administration, program administration, DAF account administration, donor/advisor self-service and independent assurance are separate authority scopes. A DAF may have unlimited participants, multiple concurrent advisors and multiple contingent or split successor DAFs. Distinguish household display groupings from legal identity, account authority and sponsor asset ownership.

## Capability requirements
- **ADM-001 Sponsor organization hierarchy** (Sponsor Admin): Support sponsor, legal entity, partner, program, DAF and account distinctions, with effective-dated relationships.
- **ADM-002 DAF establishment intake** (Onboarding): Collect intent, eligibility, funding, identity and agreements through progressive intake.
- **ADM-003 Account establishment authority** (Sponsor Governance): Separate drafted account, sponsor acceptance, legal account creation and investment activation.
- **ADM-004 Participant registry** (DAF Admin): Unlimited participants with distinct identities, multiple roles and DAF memberships.
- **ADM-005 Scoped delegation** (Security): Effective-dated permissions for donor, advisor, successor, representative, sponsor staff and partner.
- **ADM-006 Multiple concurrent advisors** (DAF Admin): Support multiple advisors with scoped grant and investment recommendation permissions.
- **ADM-007 Successor planning** (Succession): Split, contingent and multigenerational successor DAFs with sponsor approval.
- **ADM-008 Household and relationship model** (DAF Admin): Household display and collaboration without conflating legal identity or authority.
- **ADM-009 Donor identity and due diligence** (Compliance): Verify identity, entity status, source of funds and risk according to sponsor policy.
- **ADM-010 Contribution acceptance** (Contributions): Differentiate pledged, received, accepted, valued, invested and acknowledged contributions.
- **ADM-011 Noncash contribution workflows** (Contributions): Document valuation, restrictions, custody, liquidation and sponsor acceptance.
- **ADM-012 Account funding and activation** (Operations): Track initial funding requirements, investment allocation and activation gates.
- **ADM-013 Account naming and visibility** (Privacy): Support public/private names, anonymity preferences and controlled disclosures.
- **ADM-014 Account agreements** (Legal): Version, deliver, sign and retain sponsor-approved terms and acknowledgments.
- **ADM-015 Service requests** (Servicing): Unified intake for changes, questions, corrections, documents and exceptional cases.
- **ADM-016 Case orchestration** (Operations): Event-triggered cases, ownership, SLA, evidence, independent review and escalation.
- **ADM-017 Profile and contact updates** (Servicing): Authenticated updates, verification and sensitive-change safeguards.
- **ADM-018 Beneficiary and grantee preferences** (DAF Admin): Record donor intent separately from binding sponsor policy.
- **ADM-019 Grant recommendation servicing** (Grants): Handle recommendations, revisions, recurring intents, eligibility holds and donor communication.
- **ADM-020 Investment preference servicing** (Investments): Capture preferences without conferring trade or asset ownership authority.
- **ADM-021 Advisor and participant changes** (DAF Admin): Invitations, revocations, replacements, effective dates and notice.
- **ADM-022 Incapacity and death events** (Succession): Evidence-driven suspension, succession review and controlled transition.
- **ADM-023 Disputes and conflicting instructions** (Compliance): Detect conflicting directions and hold consequential actions pending sponsor resolution.
- **ADM-024 Dormancy and unclaimed accounts** (Operations): Define inactivity policy, outreach, holds and legally reviewed disposition.
- **ADM-025 Account closure and transfer** (Operations): Sponsor-approved termination, grants, asset liquidation, final accounting and retained history.
- **ADM-026 Partner service boundaries** (Partner Admin): Configurable branding, service responsibilities and tenant isolation without duplicate authority.
- **ADM-027 Sponsor policy configuration** (Sponsor Governance): Versioned sponsor-specific eligibility, fees, limits, permissions and effective dates.
- **ADM-028 Fees and billing administration** (Finance): Review fee schedules, accruals, waivers, exceptions and financial posting.
- **ADM-029 Document and correspondence center** (Communications): Templates, secure delivery, delivery status, retention and accessible formats.
- **ADM-030 Notifications and consent** (Communications): Preference-aware delivery, privacy-safe previews, opt-in and legally required notice.
- **ADM-031 Operational queue management** (Operations): Prioritized tasks by impact, age, owner, deadline, authority and blocked dependency.
- **ADM-032 Bulk operations** (Operations): Scoped bulk review, dry-run, exception isolation, approvals and rollback-by-correction.
- **ADM-033 Sponsor service-level governance** (Operations): Defined SLAs, escalation, holidays, evidence and transparent overdue reporting.
- **ADM-034 Account health and transparency** (UI): Show open cases, pending funding, holds, unresolved statements and next authorized actions.
- **ADM-035 Audit and assurance linkage** (Governance): Each consequential action links to source evidence, control evaluation and reviewer.
- **ADM-036 Accessible mobile-first administration** (UI): WASM-first Forma UI, accessible workflows, loading/offline/stale/error states.
- **ADM-037 Embeddable administration widgets** (UI): Scoped account status, tasks, participant access and case progress for authorized hosts.
- **ADM-038 API-first administration** (API): Versioned commands, events, queries, webhooks, idempotency and least-privilege auth.
- **ADM-039 Agent-assisted servicing** (AI): Agents classify, gather and draft; humans retain reserved approvals and exception authority.
- **ADM-040 Operational resilience** (Operations): Safe manual continuity, durable queues, replay, incident handling and no silent work loss.
- **ADM-041 Sponsor-owned asset clarity** (Legal): Never imply DAF participant legal ownership, unilateral trading or cash movement rights.
- **ADM-042 Service quality feedback** (Analytics): Track user effort, time to resolution, repeated contacts and actual outcome.
- **ADM-043 Data correction and provenance** (Data): Correct append-only records with linked reversals/supersessions and reviewer evidence.
- **ADM-044 Cross-DAF relationship controls** (Security): Permit scoped multi-account views without leaking unrelated donor or partner information.
- **ADM-045 Accessibility accommodation** (UI): Support alternative communications, representatives and accessible documents without assuming incapacity.
- **ADM-046 Financial and legal escalation** (Compliance): Escalate uncertain legal, tax, accounting and sanction outcomes to qualified reviewers.
- **ADM-047 Lifecycle analytics** (Analytics): Measure establishment abandonment, approval bottlenecks, exceptions and servicing resolution.
- **ADM-048 Sponsor configuration safety** (Governance): Policy changes require preview, dependency impact, effective dates, independent approval and audit.

## Reimagined outcome workflows
- **WF-01 Establish DAF:** Intent -> identity and eligibility -> contribution plan -> sponsor acceptance -> account opening -> funding -> activation.
- **WF-02 Add participant:** Invitation -> identity -> scoped role -> authority check -> acceptance -> effective permission -> notice.
- **WF-03 Change advisor:** Request -> conflict check -> authorization -> revoke prior rights -> grant new rights -> confirmation.
- **WF-04 Succession transition:** Event evidence -> protective hold -> successors and shares -> legal/policy review -> sponsor approval -> account split/transition.
- **WF-05 Grant servicing:** Recommendation -> eligibility/funding/authority -> approval -> payable -> deferred payment adapter -> reconciliation.
- **WF-06 Contribution servicing:** Gift intent -> receipt -> acceptance -> valuation -> investment -> acknowledgment.
- **WF-07 Noncash gift:** Offer -> restrictions -> diligence -> sponsor acceptance -> custody/valuation -> liquidation or retention.
- **WF-08 Account change request:** Authenticated request -> risk classification -> agent preparation -> authorized decision -> update -> notice.
- **WF-09 Complaint or dispute:** Intake -> evidence -> conflicting authority hold -> investigation -> sponsor resolution -> review.
- **WF-10 Dormancy review:** Inactivity signal -> outreach -> policy check -> review -> documented disposition.
- **WF-11 Account closure:** Request/event -> obligations and holds -> approval -> final grants/assets/fees -> accounting -> archival.
- **WF-12 Partner onboarding:** Agreement -> legal/tenant structure -> roles -> policy inheritance -> branding -> testable acceptance.
- **WF-13 Fee waiver:** Request -> policy -> independent approval -> accounting impact -> disclosure.
- **WF-14 Sensitive contact change:** Request -> step-up verification -> change -> prior-channel notice -> fraud monitoring.
- **WF-15 Bulk account servicing:** Selection -> preview -> scope/authority checks -> dry-run -> approval -> idempotent execution -> exceptions.
- **WF-16 Escalated agent case:** Agent identifies ambiguity -> cites evidence -> human decision -> controlled continuation -> monitoring.

## Critical invariants
1. Recommendation is not sponsor approval, and sponsor approval is not cash settlement.
2. Sponsor acceptance of a DAF and acceptance of a contribution are separately governed.
3. Advisor access is not trading authority or legal ownership.
4. Participant and successor rights depend on effective authority, not just display labels.
5. A change in legal status, recipient, account authority or investment base can invalidate earlier approvals.
6. No live outbound payment rail is authorized; simulation and interface requirements only.
7. No material legal, tax, accounting or sanctions determination is asserted as verified without appropriate review.
8. Case automation cannot erase evidence or silently bypass independent controls.
9. Strata is required architecturally, but its specific capabilities are not yet verified.
10. Every consequential command has an idempotent API boundary, authorization, attributable evidence and replay-safe behavior.

## API domain inventory
SponsorOrganization, PartnerProgram, DAFAccount, AccountApplication, Participant, Delegation, SuccessorPlan, IdentityVerification, ContributionIntake, GiftAcceptance, AccountFunding, InvestmentPreference, GrantRecommendation, ServiceCase, ExceptionResolution, Document, Notification, FeePolicy, AccountClosure, AccountActivity, AgentWork, AuditEvidence, WidgetGrant, OperationalMetrics.

## Business process metrics
Measure completed outcomes, median time to usable account, participant invitation acceptance, abandonment, handoff count, cases reopened, exceptions per 100 accounts, blocked grant reasons, funding activation lag, ownerless cases, SLA breaches, legal escalations, control evidence completeness, accessibility task success and cost to serve. Avoid optimizing raw automation counts when they increase donor effort or risk.

## Unresolved decisions
Sponsor-specific legal entity arrangements; minimum contribution and grant policies; account agreement terms; KYC/AML applicability and procedures; noncash acceptance standards; donor privacy disclosures; state and federal legal review; fee schedules; dormant-account policy; succession interpretation; partner delegation contracts; exact Forma, Folio and Strata capabilities; retention and service SLAs. Record as decisions, not implied facts.

## Acceptance
See DAF-ADMINISTRATION-ACCEPTANCE-TESTS.csv. Tests are specified, not executed. Requirements are proposals awaiting sponsor/legal/accounting/technical review.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### UNB-RCN-ENT-006 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-022 · **Parent:** ADM-025 · **Decisions:** RCD-022
- **Requirement:** Fund closure MUST be a state machine (ClosureRequested > ObligationsResolved > FinalFeesPosted > ResidualDisposed > Closed > Archived) with typed triggers (donor request, zero balance, dormancy disposition, succession split, full outbound transfer, sponsor exit). Closed MUST require: no open reservations; every payment attempt terminal or transferred to an outstanding-instrument liability; and a recorded destination for trailing receipts (OD-11).
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given an outstanding issued check; when closure is requested; then the fund reaches Closed only with the check carried as a liability, or the request is blocked per policy.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-006); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-006 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-PLT-023 · **Parent:** REQ-X-048 · **Decisions:** RCD-026
- **Requirement:** Unbound MUST measure, per sponsor and workflow, straight-through-processing rate, human touches per completed outcome and WorkItems created per rule, and MUST require each new WorkItem-creating rule to state its expected volume and the decision only a human can make.
- **Acceptance criteria:**
  - AC-1: Given a new rule creating WorkItems without stated human decision; when it is submitted for approval; then rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-006); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-007 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-040 · **Parent:** REQ-X-030 · **Decisions:** RCD-003
- **Requirement:** Brand Kit publication MUST validate terminology overrides against a protected-term registry of legally meaningful terms (e.g. grant recommendation, sponsoring organization legal name, tax acknowledgment wording) whose meaning may not be altered; violations block publication.
- **Validation still required:** RCD-003: Owner may rename the platform; if so, choose a name other than Strata
- **Acceptance criteria:**
  - AC-1: Given sponsor maps 'grant recommendation' to 'donation'; when Brand Kit published; then blocked with PROTECTED_TERM.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-007); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

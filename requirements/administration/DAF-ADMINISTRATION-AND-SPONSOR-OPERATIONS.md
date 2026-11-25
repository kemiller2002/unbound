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

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-SVC-003:** Unbound and its telephony adapter MUST NOT record, transcribe or machine-analyze any voice interaction unless (a) a sponsor-approved disclosure was played or read to every party before recording began and (b) consent was captured (continuation after disclosure only where counsel approves that as consent; otherwise explicit DTMF/verbal acceptance). The same disclosure MUST apply regardless of caller or staff location. A caller who declines MUST be served without recording or offered an equivalent channel. Recording MUST pause during DTMF entry of secrets. Each recording or transcript MUST store disclosure version, consent evidence, parties, start/stop times and retention class. Outbound staff calls to charities or custodians follow the same rule. Release-1 setting (XID-SVC-002): recording and machine transcription of voice interactions are disabled for every sponsor; enabling later requires explicit all-party consent captured at the start of each call and a transcription vendor contracted as a service provider with no training use.
  - Provenance: [UNB-XIN-SVC-003](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-SVC-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given recording enabled for a sponsor; when a call connects; then disclosure plays before the recorder starts and consent evidence is stored.
    - AC-2: Given caller declines recording; when the call continues; then no recording or transcript exists and the Contact notes 'not recorded by request'.
    - AC-3: Given caller enters a one-time code by DTMF; when recording is active; then the recorded audio contains no DTMF tones for that span.
    - AC-D1: Given a release-1 deployment; when a sponsor admin tries to enable call recording; then the setting is unavailable with FEATURE_GATED_SVC_002.
- **UNB-XIN-CLM-002:** Every decline, hold longer than its profile's notice threshold, or conditional approval communicated to a donor, advisor or charity MUST include: a reason code from a counsel-approved public reason catalog mapped from gate outcomes; a plain-language explanation; the public policy reference (clause ID); what evidence or action, if any, could change the outcome; the reconsideration path and deadline where available; and the decision date. Notices MUST NOT disclose sanctions match details, investigation existence, or other restricted reasons; such cases MUST use a neutral catalog code. The notice content and catalog version MUST be stored with the decision record. The public reason catalog version 1 is: ELIGIBILITY_NOT_CONFIRMED, ORGANIZATION_STATUS_CHANGED, PURPOSE_NOT_PERMITTED_BY_POLICY, IMPERMISSIBLE_BENEFIT, INFORMATION_NOT_RECEIVED, PAYMENT_DETAILS_UNVERIFIED, COMPLIANCE_REVIEW (all restricted reasons map here), FUND_BALANCE_INSUFFICIENT, DUPLICATE_REQUEST, SPONSOR_DISCRETION (XID-CLM-003).
  - Provenance: [UNB-XIN-CLM-002](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-CLM-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given grant declined by gate G-eligibility 'charity revoked'; when notice generated; then contains code ELIG-REVOKED, clause ID, reconsideration info.
    - AC-2: Given grant held due to sanctions potential match; when notice generated; then neutral code UNDER-REVIEW; no sanctions wording.
    - AC-D1: Given a grant held for a sanctions potential match; when the donor notice is generated; then it carries COMPLIANCE_REVIEW and no sanctions wording.
- **UNB-XIN-CLM-006:** A representative (agent under power of attorney, guardian, conservator, family delegate) MUST act only within the scope recorded in the delegation (ADM-005). An authorized staff member who records specific facts supporting a reasonable belief that a representative is exploiting or endangering the donor MUST be able to place a ProtectiveSuspension that blocks new recommendations and contact/payee changes by that representative, notifies the sponsor's designated reviewer within 1 business day, expires after a counsel-set maximum unless renewed, and MUST NOT disclose the suspicion to the representative. Pending payments are governed by existing hold rules. Durations (XID-CLM-002): a ProtectiveSuspension expires after 15 business days unless an independent reviewer extends it once by up to 10 business days, after which it must be referred to counsel or the appropriate authority; no money moves during suspension; the principal is notified unless notice would endanger them.
  - Provenance: [UNB-XIN-CLM-006](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-CLM-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given staff records facts of suspected exploitation by POA P; when suspension placed; then P's new recommendations rejected; reviewer notified.
    - AC-2: Given suspension reaches max duration without renewal; when expiry; then suspension lifts and audit records expiry.
    - AC-D1: Given a suspension placed on day 0 and extended once; when business day 26 begins without referral; then the suspension lapses and a counsel-referral task is overdue and escalated.
- **UNB-XIN-CRM-003:** Each message type MUST declare a primary-purpose class (Commercial, TransactionalOrRelationship, MandatoryLegal, Security) and a sender legal entity with its entity type (tax-exempt nonprofit or other). Every Commercial email MUST carry the sender's valid physical postal address and a functioning opt-out that remains operable for at least 30 days after sending; an opt-out MUST be applied to all Commercial sends for that sender entity within 10 business days, and suppression lists MUST be keyed by sender entity. Automated or prerecorded voice or SMS messages that include an advertisement MUST NOT be sent without consent recorded at the level required for the sender's entity type (prior express written consent unless counsel records the nonprofit rule applies). Opt-outs MUST NOT suppress MandatoryLegal, Security or TransactionalOrRelationship messages. Decided defaults: promotional email about DAF services is Commercial; in white-label programs the sender is the entity named in the From line, contractually designated as the single sender (XID-CRM-001). Automated or prerecorded calls and texts sent by or on behalf of the tax-exempt sponsor require at least prior express consent; those sent on behalf of a for-profit partner or advertising a for-profit's services require prior express written consent (XID-REG-003; 47 CFR 64.1200).
  - Provenance: [UNB-XIN-CRM-003](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-CRM-001, XID-REG-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given A donor opts out of partner P's commercial email; when a P commercial campaign is scheduled 8 business days later; then the donor is suppressed.
    - AC-2: Given The same donor; when a grant-paid notice is generated; then it is delivered.
    - AC-3: Given A Commercial template without postal address token; when publication; then rejected with MISSING_POSTAL_ADDRESS.
    - AC-4: Given A promotional SMS from a for-profit partner without written consent record; when send; then blocked with CONSENT_LEVEL_INSUFFICIENT.
    - AC-D1: Given an SMS campaign advertising a white-label partner's own wealth services; when consent is checked; then only recipients with prior express written consent are sent.
<!-- END ADOPTED: cross-industry-discovery -->

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-SVC-001 — Case kernel, assignment, clocks and prioritisation:** Cases SHALL use a common kernel with code-defined typed profiles (F# discriminated unions for invariants) and sponsor-configurable routing, deadlines and parameters. Assignment modes SHALL be configurable by case kind (skill-based for exceptions, cradle-to-grave for succession, relationship or round robin for service, restricted routing for fraud) with backup and escalation. Clocks SHALL use business calendars per channel and case, track end-to-end elapsed time alongside internal SLA, pause only for documented reasons and never pause statutory deadlines; workflow targets SHALL be versioned by step, risk and dependency, distinguishing external wait from internal delay. Prioritisation SHALL treat hard legal deadlines and imminent irreversible harm as simultaneous constraints, escalate collisions and prevent starvation; no opaque weighted score SHALL be used initially. Staff interactions with charities and custodians SHALL be recorded as contacts with audience and access classification. Live phone, callback and asynchronous channels SHALL be supported and activated per sponsor and accessibility needs.
  - Provenance: resolves XID-SVC-001, UNB-XIN-SVC-002#Q1, SPD-ORG-005, RR-REQ-X-048-r3#Q1, UNB-XIN-SVC-005#Q1, UNB-XIN-EVA-007#Q1, UNB-SMP-DSH-001#Q1, UNB-XIN-SVC-001#Q1, XID-SVC-005; amends UNB-XIN-SVC-002, UNB-XIN-SVC-005, UNB-XIN-EVA-007, UNB-SMP-DSH-001, UNB-XIN-SVC-001, RR-REQ-X-048-r3; related findings RCF-PLT-025. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Service operations; financial system of record: n/a; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor or administrator service team; evidence owner: Sponsor operations owner.
  - Acceptance:
    - AC-1: Given a statutory deadline inside a paused case, when the pause is requested, then the statutory clock continues.
    - AC-2: Given a low-priority case waiting beyond its starvation bound, when the queue is ordered, then it is escalated.
- **CNV-SVC-002 — Capacity planning, surge and suspension:** Staffing forecasts SHALL separate incident-driven demand from routine workload while including expected surge capacity; backlog thresholds SHALL be illustrative until calibrated by severity, age and workload in a pilot. A December readiness gate SHALL require a surge plan (pre-contracted, pre-appointed qualified reviewers with activation lead time set from forecast and deadlines, five working days candidate) before 1 December, and simulated peak SHALL meet each legal deadline (TEV-001). During a platform suspension, donors SHALL see read-only access only if confidentiality and integrity remain trustworthy, otherwise a status-only channel, with no unsafe processing promises. Practical capacity SHALL be measured by role including leave, training, controls and incident load; utilization SHALL NOT be treated as productivity.
  - Provenance: resolves UNB-SMP-ORG-001#Q1, UNB-SMP-ORG-001#Q2, SPD-ECN-004, UNB-SMP-ARX-001#Q1, UNB-XIN-ECO-006#Q1, UNB-XIN-ECO-015#Q1, XID-ECO-008, UNB-XIN-SVC-008#Q1, UNB-XIN-ECO-017#Q1; amends UNB-SMP-ORG-001, UNB-SMP-ARX-001, UNB-XIN-ECO-006, UNB-XIN-ECO-015, UNB-XIN-SVC-008, UNB-XIN-ECO-017. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Empirical: December interval data and time studies (evidence backlog).
  - Ownership: domain owner: Service operations; financial system of record: n/a; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor or administrator service team; evidence owner: Sponsor operations owner.
  - Numeric parameters: five working days [policy: surge activation lead time candidate]; 1 December [policy: December readiness gate date].
  - Acceptance:
    - AC-1: Given no recorded surge plan on 1 December, when readiness is assessed, then the gate fails and the sponsor executive is alerted.
    - AC-2: Given the December readiness gate, when a simulation of P90 forecast December arrivals against rostered capacity including double absence shows any legal deadline missed or internal attainment below the sponsor floor, then the gate fails and the plan must be revised.
<!-- END ADOPTED: convergence-2026-11-24 -->

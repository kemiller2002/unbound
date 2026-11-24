# Platform, API, experience and assurance standards
Date: 2026-11-21 | Status: owner-directed mandatory product requirements; implementation deferred

## Non-negotiable platform requirements
- **UNB-PLAT-001:** All business capabilities SHALL expose governed, versioned APIs/contracts, including internal capability boundaries. API-first does not require microservices; F# modular monolith remains preferred.
- **UNB-PLAT-002:** All application user interfaces SHALL use **Forma** and the approved Echelon design system. Do not introduce alternate UI frameworks without an explicit documented exception approved by the owner.
- **UNB-PLAT-003:** All enterprise-generated documents, printable statements, acknowledgments, grant correspondence, reports and export-ready human-readable artifacts SHALL use **Folio** where technically supported. Validate Folio's actual rendering, accessibility and archival capabilities before asserting compliance.
- **UNB-PLAT-004:** Every interactive user journey SHALL be **mobile-first**, including donor, joint-advisor, successor, sponsor, compliance, partner and administrative experiences. A phone must support essential completion, not just viewing.
- **UNB-PLAT-005:** Accessibility SHALL be designed and tested to **WCAG 2.2 Level AA** as a measurable baseline, plus applicable ADA and other jurisdiction-specific legal obligations. Passing automated WCAG checks alone does not establish ADA legal compliance.
- **UNB-PLAT-006:** The backend SHALL be F# and WASM-first UI integration SHALL use current compatible Echelon components, including Limen and Forma, subject to Registry/Conditor verification. No unapproved alternate runtime/framework.
- **UNB-PLAT-007:** **Strata** SHALL be used in the persistence architecture. All authoritative business history is append-only; corrections and reversals append new records. Physical storage guarantees and Strata integration must be verified.
> Adopted 2026-11-23: superseded by `RR-UNB-PLAT-007-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-PLAT-008:** Partner data isolation SHALL support separate schemas, separate databases and dedicated infrastructure, selected per partner; all shared services must preserve tenant boundaries.
> Adopted 2026-11-23: refined by `RR-UNB-PLAT-008-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-PLAT-009:** Forma and Folio SHALL use a coherent branded design system, consistent content semantics, typography, color contrast, error patterns and accessibility tokens; each public-facing application has a nonblank loading experience and clear independent identity.

## API contract standard
- **UNB-API-STD-001:** Every API defines caller roles, sponsor authority boundary, request/response schemas, validation, version, idempotency, optimistic concurrency where relevant, privacy classification, authorization policy, error codes and audit evidence.
- **UNB-API-STD-002:** Every state-changing API defines accepted/requested versus approved/executed states, async completion, retry rules, cancellation and compensation, and a documented side-effect boundary.
- **UNB-API-STD-003:** All APIs and event contracts require backward compatibility strategy, deprecation policy, schema evolution, contract tests, tracing/correlation IDs, rate limits and explicit failure behavior.
- **UNB-API-STD-004:** External APIs cannot directly approve sponsor-reserved decisions, initiate money movement, modify authoritative events or grant privileges outside governed approval boundaries.
- **UNB-API-STD-005:** Every integration documents authentication method, credential rotation, least privilege, webhook signature verification, replay defense, partner context binding, provider outage behavior and test fixtures.
- **UNB-API-STD-006:** API descriptions SHALL be generated from or verified against authoritative contracts, with no undocumented implementation-only business operations.

## Forma interaction and mobile standard
- **UNB-UX-001:** Each essential task SHALL have a phone-sized reference flow, including multi-party participation, successor planning, grant recommendation, account statements, sponsor review and exceptions.
- **UNB-UX-002:** Forms, dialogs, focus, validation, navigation, errors, tables, graphs and notifications SHALL support keyboard and assistive technology; equivalent nonvisual alternatives required for visual succession graphs.
- **UNB-UX-003:** Interfaces SHALL expose pending, completed, blocked, failed and uncertain states distinctly; never imply a recommendation was approved or a payment completed when only requested.
> Adopted 2026-11-23: refined by `RR-UNB-UX-003-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-UX-004:** Slow network, interrupted session, duplicate tap, stale state, expired delegation and offline/poor connectivity SHALL have safe recovery patterns.
- **UNB-UX-005:** Test zoom/reflow, text spacing, contrast, touch target sizing, screen-reader announcements, focus order, error identification and reduced-motion behavior on supported mobile and desktop platforms.
- **UNB-UX-006:** Accessibility testing SHALL include automated checks, keyboard-only operation, manual screen-reader review, and representative assistive-technology task completion, with documented defects and retests.
- **UNB-UX-007:** Avoid exposing internal EF tool names or implementation concepts in donor-facing experiences unless genuinely relevant to the user.

## Folio and evidence standard
- **UNB-DOC-001:** Each generated document SHALL retain source event/version references, effective and recorded times, template version, policy version, generation timestamp, authorizing actor and integrity metadata.
- **UNB-DOC-002:** Statements and acknowledgments SHALL distinguish source evidence, corrections and as-of dates; regeneration must not silently rewrite historical delivered documents.
- **UNB-DOC-003:** Folio outputs SHALL be tested for accessibility, legibility, mobile preview, printing, archival/export requirements and multilingual/locale formatting where supported.
- **UNB-DOC-004:** Document delivery SHALL be role-scoped and tenant-isolated, with immutable delivery receipts, privacy-safe notifications and controlled redaction.
- **UNB-DOC-005:** Templates and document-generation APIs SHALL be versioned, tested against synthetic fixtures, and resistant to untrusted content injection.

## AI, resilience and governance standard
- **UNB-AI-STD-001:** Agent actions SHALL use the same governed APIs and tenant-scoped authorization as humans; no database shortcuts or privileged hidden tools.
- **UNB-AI-STD-002:** Evidence, uncertainty, model/prompt version, policy version and authorized human sign-off SHALL be captured for material AI-assisted decisions.
- **UNB-AI-STD-003:** Deterministic controls govern money, access, eligibility gates and policy publication; agents may not self-approve or clear their own alerts.
> Adopted 2026-11-23: refined by `RR-UNB-AI-STD-003-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-RES-001:** Each critical capability SHALL define failure modes for model, database, partner API, custodian, payment rail, document renderer and identity provider outages.
- **UNB-RES-002:** Replay and recovery SHALL not duplicate external payments, grants, trades, messages or account allocations; test idempotency and single-writer fencing.
- **UNB-RES-003:** Every capability SHALL have defined RTO/RPO, service ownership, operational runbooks, telemetry, incident escalation and recovery validation proportionate to criticality.

## Completion gates
- Every requirement has a stable ID, owner, source/decision, authority boundary, measurable acceptance criteria and normal/exception/adversarial scenarios.
- Every API has a contract and negative authorization tests.
- Every essential Forma journey has a phone-sized interaction specification and accessibility acceptance tests.
- Every Folio document type has a provenance, correction, authorization and accessibility specification.
- Every append-only/tenant-isolation guarantee has a physical enforcement and recovery test.
- No claims of implementation, compliance certification or executed tests until evidence exists.
- Reconcile all standards with current EF Registry/Conditor and actual Forma, Folio, Strata capabilities before code.

## Next requirements artifacts
1. Canonical API inventory and contract matrix, including sponsor authority and tenant context.
2. Persona-by-journey matrix with mobile and accessibility acceptance tests.
3. Folio document inventory and provenance/retention matrix.
4. Threat model and failure-mode matrix covering all partner isolation profiles.
5. Full fund lifecycle and successor-account authority/state matrix.
6. Regulatory-control row-level crosswalk and contradiction register.


## Mandatory SOC 2 evidence-by-design directive (2026-11-21)
All subsequent design, research, agent missions, implementation and operations MUST follow [SOC 2 Controls, Evidence and Enterprise Directive](../requirements/governance/SOC2-CONTROLS-EVIDENCE-AND-ENTERPRISE-DIRECTIVE.md) and [Sponsor Authority and Autonomous Operations](../requirements/governance/SPONSOR-AUTHORITY-AND-AUTONOMOUS-OPERATIONS.md). For every claimed control specify owner, scope, procedure, cadence, expected evidence, monitoring, exception handling, test and retention. Log security-relevant and consequential actions with attributable tamper-evident evidence while excluding secrets and unnecessary personal data. Include negative tests and report gaps truthfully. SOC 2 readiness is a design requirement, not a claim of completed attestation or operational compliance. No live outbound payment execution is authorized.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-PLAT-008-r1 (refines `UNB-PLAT-008`) — adopted-provisional
- **Original:** `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:12` @ `777a70d` (kept verbatim) · **Decisions:** RCD-004
- **Requirement:** Unbound MUST isolate each tenant's authoritative and derived data by binding every data access to exactly one VerifiedTenantContext. The platform MUST support Profile B (dedicated database per tenant on shared managed infrastructure) as the default. Profile A (schema per tenant in a shared database) and Profile C (dedicated deployment cell) MAY be activated per tenant only after an approved TenantIsolationDecision citing a contractual or regulatory requirement and a passing isolation test suite for that profile. All profiles MUST run the identical release of the same codebase.
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given tenant T has no approved decision; when T is provisioned; then T is placed in Profile B.
  - AC-2: Given an approved decision selects Profile A for T and the Profile A isolation suite has failed; when activation is attempted; then activation is rejected and no binding generation is created.
  - AC-3: Given a principal entitled only to tenant T1; when it issues any query with a forged tenant hint for T2; then the broker binds T1 or denies; zero T2 rows are returned and a denial event is logged.
  - AC-4: Given tenants in Profiles B and C; when a release is deployed; then both cells report the same release version before traffic resumes.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-PLAT-008-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-057-r1 (refines `REQ-X-057`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:520` @ `777a70d` (kept verbatim) · **Decisions:** RCD-004, RCD-013
- **Requirement:** Unbound MUST be one codebase implementing one domain model, one ledger and policy engine, one authorization model and one case model, released as one version to every deployment cell. A deployment cell is a running instance of that release with its tenant-bound authoritative stores. Operating models MUST differ only by declarative configuration, role assignments, adapters and view composition. Cross-cell interactions (shared charity facts, global holds, sponsor-level consolidation) MUST use versioned inter-cell contracts with idempotency and reconciliation; no cell may hold a second authoritative copy of another cell's financial state.
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given two cells; when a release is promoted; then both run the same version or the lagging cell rejects dependent commands.
  - AC-2: Given a sponsor uses two operating models; when a grant is recommended in each; then both use the same GrantRecommendation command and state machine.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-057-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-020-r1 (refines `REQ-X-020`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:268` @ `777a70d` (kept verbatim) · **Decisions:** RCD-004, RCD-012
- **Requirement:** Each global hold (charity, sanctions, recipient) MUST be committed in the hold registry with a strictly increasing HoldPublicationVersion. A payment release in any cell MUST, inside the release transaction, obtain the registry's current HoldPublicationVersion and verify that the recipient has no active hold at that version; it MUST record that version on the release. If the registry is unreachable or the cell's replicated watermark is behind the registry version, the release MUST remain in ReleaseBlocked and MUST NOT be dispatched. Dispatch MUST re-check holds immediately before irreversible external submission.
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given hold H on charity C published at version 41; when a release for C commits reading version 41; then the grant is Held and no instruction is dispatched.
  - AC-2: Given release authorized at version 40 and hold H published at 41 before dispatch; when dispatcher runs; then submission is suppressed and the grant becomes Held.
  - AC-3: Given registry unreachable; when a release is attempted; then state is ReleaseBlocked and no instruction exists.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-020-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-051-r1 (refines `REQ-X-051`) — adopted
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:460` @ `777a70d` (kept verbatim) · **Decisions:** RCD-025
- **Requirement:** Every public API operation, configuration property and workflow action MUST publish an AssistanceContract conforming to the Unbound AssistanceContract JSON Schema (draft 2020-12) version-pinned with the API contract. Required core fields: contractId (stable URN), version (semver), ownerComponent, kind (enum: apiOperation|configProperty|workflowAction|uiField|templateToken), purpose (text, max 280 chars), inputSchemaRef, outputSchemaRef, sideEffects (enum list: none|createsDraft|changesConfig|changesAuthoritativeState|externalEffect), authorityClass (A0..A5 per GOV authority tiers), confirmationRequired (bool), reversibility (enum: reversible|compensatable|irreversible), unknownStates (list), errors (list of error codes), visibility (enum: public|tenantScoped|roleScoped|restricted). Editorial annotations (examples, troubleshooting, policy source) are optional. A surface is 'assistant-ready' only when its contract validates and matches the deployed API schema hash. (Integration amendment, mission section 10.) The following fields are also REQUIRED, not optional: validationRules (machine-readable constraints, generated from the input schema plus policy references), businessConstraints (policy IDs and versions that can block the operation), permittedActorTypes (deterministic policy, AI agent, human role; from the authority matrix UNB-RCN-PLT-005), safeActions and prohibitedActions for AI callers, humanApprovalBoundary (text plus authorityClass), outcomeExplanation (template mapping each result and error code to a plain-language explanation with the evidence fields to cite), and at least one example (request, response and one error).
- **Acceptance criteria:**
  - AC-1: Given an API operation with no contract; when an assistant is asked to perform it; then the assistant reports it is undocumented and makes no call.
  - AC-2: Given contract hash differs from deployed OpenAPI; when CI runs; then the build fails.
  - AC-3: Given a restricted contract; when a donor role queries discovery; then the contract is not returned.
  - AC-4: Given An apiOperation contract missing outcomeExplanation or prohibitedActions; when CI validates contracts; then the build fails and the surface is not assistant-ready.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-051-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-AI-STD-003-r1 (refines `UNB-AI-STD-003`) — adopted
- **Original:** `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:42` @ `777a70d` (kept verbatim) · **Decisions:** RCD-025
- **Requirement:** Money movement, access grants, eligibility gates, hold lifting and policy publication MUST be decided only by versioned deterministic rules or authorized humans. AI outputs MUST be typed as Proposal, Finding or Explanation and MUST NOT be accepted as input to these gates except as evidence attached for a human reviewer. No agent may resolve, suppress or close an alert, case or finding that it raised or that concerns its own action.
- **Acceptance criteria:**
  - AC-1: Given an agent raised finding F; when the same agent calls CloseCase(F); then denied and logged.
  - AC-2: Given the model service is down; when a routine eligibility gate evaluates; then gate decides from deterministic inputs only.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-AI-STD-003-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-048-r1 (refines `REQ-X-048`) — adopted
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:442` @ `777a70d` (kept verbatim) · **Decisions:** RCD-026
- **Requirement:** A WorkItem MUST be created only when a deterministic process cannot complete because of (a) a rule-defined hold, (b) missing or conflicting evidence, (c) a decision requiring human authority, or (d) a provider Unknown requiring reconciliation. Each WorkItem MUST reference the blocked outcome, the rule or event that created it and the exit condition. Assignment MUST use the sponsor's routing policy (round robin, sticky owner, skill, capacity, hybrid) and MUST NOT confer decision authority. WorkItems MUST auto-close when their exit condition is met by system events.
- **Acceptance criteria:**
  - AC-1: Given a routine grant passing all rules; when processing completes; then no WorkItem is created.
  - AC-2: Given a grant held for missing charity evidence; when the evidence arrives and rules pass; then the WorkItem closes automatically.
  - AC-3: Given two workers claim the same item; when both submit; then one succeeds, the other gets ALREADY_CLAIMED.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-048-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-UX-003-r1 (refines `UNB-UX-003`) — adopted
- **Original:** `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:26` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** Every screen, widget, notification and document MUST display a subject's status using the authoritative domain state from that subject's state machine, including Unknown/Uncertain and Failed where the state machine defines them. UI technical states (loading, stale, offline, error) MUST be shown separately from domain state. The UI MUST NOT display a terminal success state until the authoritative command result and evidence confirm it.
- **Acceptance criteria:**
  - AC-1: Given payment outcome Unknown; when donor opens grant; then status shows 'Outcome being confirmed' and no Paid label.
  - AC-2: Given command accepted but not approved; when widget shows status; then label is 'Submitted'.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-UX-003-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-029-r1 (supersedes `REQ-X-029`) — adopted
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:322` @ `777a70d` (kept verbatim) · **Decisions:** RCD-008
- **Requirement:** For the initial release, sponsors SHOULD customise communications through a constrained template model: each communication event has a platform-defined block layout; sponsors MAY edit permitted text slots, select approved Brand Kit theme variants, insert typed tokens from the event's allowlist and add locale variants; mandatory clauses are locked. Templates MUST have draft/review/publish, synthetic preview (desktop, mobile, plain text), accessibility checks and version pinning for as-sent reconstruction. A free-form visual designer or raw HTML mode MUST NOT be provided until a documented sponsor demand decision approves it.
- **Acceptance criteria:**
  - AC-1: Given a slot containing {{fund.balance}} not allowed for event GrantPaid; when author saves; then TOKEN_NOT_ALLOWED.
  - AC-2: Given published template v3; when a message is sent; then as-sent record references v3 and Brand Kit version.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-029-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-PLAT-007-r1 (supersedes `UNB-PLAT-007`) — adopted-provisional
- **Original:** `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:11` @ `777a70d` (kept verbatim) · **Decisions:** RCD-027
- **Requirement:** Authoritative business history MUST be stored by an Unbound-owned Ledger/Event Store component on managed PostgreSQL, in which authoritative tables are insert-only for the application role (no UPDATE/DELETE grants), corrections and reversals are new linked records, and each append is assigned a per-stream sequence number checked under optimistic concurrency. Strata MUST be used as the schema governance layer: every schema object for the authoritative store, including grants, triggers and row-level-security policies that enforce append-only and tenant isolation, MUST be declared in Strata source, deployed only through Strata plans, and checked for drift before each release; a 'not-compared' or 'unverifiable' Strata state for any such object MUST block release.
- **Validation still required:** RCD-027: Counsel on EF licence terms (commercial-launch blocker, not a design blocker)
- **Acceptance criteria:**
  - AC-1: Given The application database role; when it issues UPDATE on a journal table; then the database rejects it with permission denied.
  - AC-2: Given A release candidate whose live database has a manually added trigger; when the Strata drift check runs; then state differs; release blocked.
  - AC-3: Given Two concurrent appends to one fund stream at expectedVersion 7; when both commit; then exactly one succeeds; the other receives ConcurrencyConflict and no partial events exist.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-PLAT-007-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-PLT-003 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-002 · **Parent:** REQ-X-020 · **Decisions:** RCD-004, RCD-012
- **Requirement:** Every global hold MUST carry a strictly increasing HoldPublicationVersion, and every payment release and dispatch MUST record and verify the version observed (see RR-REQ-X-020-r1).
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given hold published at v41; when release in another cell reads v41; then release Held.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-004 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-PLT-043 · **Parent:** UNB-AI-STD-001 · **Decisions:** —
- **Requirement:** All context retrieved for an AI assistance session MUST be read through the public query layer under the initiating principal's VerifiedTenantContext and role; retrieved resource IDs MUST be logged with the session.
- **Acceptance criteria:**
  - AC-1: Given user lacks access to fund F; when assistant builds context; then no F data retrieved; attempt logged.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-004); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-009 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-PLT-017 · **Parent:** REQ-X-082 · **Decisions:** —
- **Requirement:** An ADV/DKS scenario MUST specify expected domain state, rejected command and error code where applicable, required audit record and financial invariant before it may be cited as acceptance evidence; CI MUST reject duplicate scenario IDs.
- **Acceptance criteria:**
  - AC-1: Given ADV line with only boilerplate expected text; when traceability check runs; then flagged stimulus-only.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-009); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-010 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-029 · **Parent:** UNB-PLAT-005 · **Decisions:** RCD-009
- **Requirement:** Every issued document MUST be available in an accessible form (tagged PDF or accessible HTML generated from the same snapshot) at issuance, with the same integrity reference.
- **Validation still required:** RCD-009: DG-04 spike: byte-identical re-render and throughput
- **Acceptance criteria:**
  - AC-1: Given statement issued; when screen-reader user opens Document Center; then accessible rendition available with same snapshot ID.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-010); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-011 (P2, origin: gap-analysis) — adopted
- **Exposed by:** RCF-PLT-028 · **Parent:** UNB-PLAT-004 · **Decisions:** RCD-026
- **Requirement:** The persona-by-journey matrix MUST classify each task as phone-complete, phone-review-and-approve or desktop-first, with rationale, and acceptance tests per class.
- **Acceptance criteria:**
  - AC-1: Given task 'approve grant'; when matrix consulted; then phone-complete.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-011); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-012 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-006 · **Parent:** UNB-TEN-002 · **Decisions:** RCD-004
- **Requirement:** A TenantIsolationDecision MUST cite the contractual/regulatory clause requiring a non-default profile, the risk accepted, cost, and the passing isolation test suite before activation.
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given request for Profile C without clause; when decision submitted; then rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-012); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-PLT-001 — Communications law classification and consent:** Every message template SHALL be classified commercial, transactional or relationship, or other (16 CFR 316.3) before activation, including email; promotional DAF email SHALL receive commercial controls with the sponsor as sender in white-label programs, and a single designated sender only where 316.2(m) is met; suppression SHALL be keyed to the sender and opt-outs honored within 10 business days across all platforms that send for that sender. Automated or prerecorded texts and calls SHALL require prior express written consent naming the sponsor (and any for-profit partner advertised); revocation SHALL follow the FCC rules effective 25 November 2026; residential prerecorded calls SHALL be limited to 3 per 30 days with a company-specific do-not-call list.
  - Provenance: resolves XID-CRM-001, UNB-XIN-CRM-003#Q1, XID-REG-003, UNB-XIN-REG-003#Q1, XID-INT-001; amends UNB-XIN-CRM-003, UNB-XIN-REG-003. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Communications; financial system of record: n/a; policy owner: Sponsor marketing and compliance; data owner: Sponsor; operational owner: Sponsor donor services; evidence owner: Sponsor compliance officer.
  - Numeric parameters: 10 business days [authority: CAN-SPAM opt-out (16 CFR 316.5 / 15 U.S.C. 7704)]; 3 per 30 days [authority: 47 CFR 64.1200 residential prerecorded-call limit]; 25 November 2026 [authority: FCC revocation rules effective date (batch F)].
  - Acceptance:
    - AC-1: Given an unclassified template, when activation is requested, then it is refused.
    - AC-2: Given an SMS opt-out reply, when received, then all future automated texts for that sender stop and the revocation is evidenced.
- **CNV-PLT-002 — Call recording disabled with a consent design for later:** Release 1 SHALL NOT record or machine-transcribe calls. When enabled, recording SHALL require an outset disclosure to every party, affirmative consent (keypad or verbal), a no-recording path, re-disclosure and consent for any party joining later (or pausing for that segment), transfer events with consent status, and a transcription vendor contracted as a service provider without training or independent use; the uniform strictest rule SHALL apply, with a jurisdiction table kept for review.
  - Provenance: resolves XID-SVC-002, UNB-XIN-SVC-003#Q1, UNB-XIN-SVC-003#Q2; amends UNB-XIN-SVC-003, REQ-X-025. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: deferred; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Service; financial system of record: n/a; policy owner: Sponsor compliance; data owner: Sponsor; operational owner: Sponsor donor services; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a call transferred to a third party while recording is enabled, when the party joins, then recording pauses until consent is captured.
- **CNV-PLT-003 — Identity proofing, authentication and contact assurance:** Donor and advisor identity proofing SHALL be risk-based: validate core attributes against authoritative sources, match the funding account owner and confirm a validated address or phone; step up to document and selfie or a non-biometric live-video alternative when validation fails or risk is elevated; entities SHALL be verified by registration and signatory authority. Knowledge-based verification SHALL NOT be used for proofing and SHALL never exceed contact assurance level CAL1. Platform and sponsor privileged roles SHALL use AAL3-grade authenticators (hardware keys or device-bound non-exportable keys; no synced passkeys) with two keys per user and identity-proofed recovery; other workforce SHALL use AAL2 phishing-resistant authenticators. Contact assurance SHALL use distinct levels CAL0–CAL3, with only CAL3 mapped to an achieved AAL. Verified prefill SHALL be marked as prefill, require confirmation, never overwrite verified data silently, and never substitute for identity proofing.
  - Provenance: resolves SPD-CXP-002, UNB-XIN-IAM-003#Q1, UNB-XIN-SVC-004#Q1, XID-SVC-004, XID-ADV-008, XID-ADV-009, RR-ADM-002-r1#Q1; amends UNB-XIN-IAM-003, UNB-XIN-SVC-004, RR-ADM-002-r1, RR-UNB-PART-007-r1, UNB-RCN-ENT-014. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor bank confirmation that its agreement imposes no stricter KYC.
  - Ownership: domain owner: Identity; financial system of record: n/a; policy owner: Unbound security; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound security.
  - Internal control: UNB-CTRL-004, UNB-CTRL-019; operator Unbound; service-organization report relevance SOC2; automated.
  - Acceptance:
    - AC-1: Given a caller verified only by knowledge questions, when they request a bank-detail change, then it is refused below CAL3.
    - AC-2: Given a privileged sponsor user with a synced passkey, when they attempt to approve a reserved decision, then step-up to an AAL3 authenticator is required.
- **CNV-PLT-004 — Retention classes:** Transaction-level financial records (contributions, grants, rejected or declined sanctioned transactions) and related screening results, dispositions and licence reliance records SHALL be retained 10 years after the transaction date; records about blocked property while blocked plus 10 years after unblocking; other financial and grant records at least 7 years; sponsor organizing documents and determination letters permanently; ACH authorizations per CNV-PAY-005. Legal holds SHALL override disposition. The sponsor compliance owner SHALL approve the schedule.
  - Provenance: resolves RCD-007; amends REQ-X-036; related findings RCF-REG-003, RCF-PLT-013, RCF-PLT-014, RCF-FIN-035. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor compliance owner approval of the retention schedule.
  - Ownership: domain owner: Records; financial system of record: n/a; policy owner: Sponsor compliance owner; data owner: Sponsor; operational owner: Unbound operations; evidence owner: Sponsor compliance officer.
  - Numeric parameters: 10 years [authority: 31 CFR 501.601 as amended 2025]; 7 years [policy: platform floor].
  - Internal control: UNB-CTRL-074, UNB-CTRL-086; operator Unbound; service-organization report relevance SOC2; automated; complementary user entity control CUEC-CNV-PLT-004.
  - Acceptance:
    - AC-1: Given a grant paid on 1 March 2026, when disposition is evaluated on 2 March 2035, then the record is retained.
- **CNV-PLT-005 — Statements and complaints:** Statements SHALL be generated for every configured period; the default notification SHALL be a quarterly digest plus correction alerts, monthly by opt-in, paper on request or absent electronic consent, and any cadence promised in program terms SHALL override defaults (no statutory periodic-statement duty was found). A complaint SHALL mean any expression of dissatisfaction, justified or not; acknowledgment within 2 business days, substantive response target 15 business days, outer limit 60 calendar days with a holding response; regulator-originated complaints SHALL route to the sponsor's legal function within 1 business day; contractual or legal deadlines SHALL override. A deterministic classification rule set with examples SHALL separate complaints from inquiries, automatic acknowledgment is allowed, and complaint incidence SHALL be measured in the CNV-EVB-001 time study before staffing.
  - Provenance: resolves RR-REQ-X-032-r1#Q1, SPD-ENT-003, SPD-CFG-002, XID-SVC-003; amends RR-REQ-X-032-r1, REQ-X-032. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Service; financial system of record: Unbound program ledger; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor donor services; evidence owner: Sponsor compliance officer.
  - Numeric parameters: 2 business days [policy: complaint acknowledgment (CFPB/FCA benchmarks only)]; 15 business days [policy: complaint response target]; 60 calendar days [policy: complaint outer limit]; 1 business day [policy: regulator-originated routing].
  - Acceptance:
    - AC-1: Given a donor without e-consent, when the quarter closes, then a paper statement is queued.
    - AC-2: Given a complaint unresolved at day 15, when the target passes, then a holding response is sent and the case escalates.
    - AC-3: Given 200 contacts double-coded by two agents, when classification is validated before activation, then agreement reaches the sponsor-set rate.
- **CNV-ARC-001 — Modular monolith on managed PostgreSQL, accepted against declared objectives:** The authoritative ledger and fact tables SHALL live in one managed PostgreSQL database per sponsor tenant (candidate) with a transactional outbox and durable process-manager rows; workflows SHALL use in-database state machines with a timer table, and a durable-workflow engine or message broker SHALL be adopted only on measured complexity or reliability evidence. Before architecture acceptance, capability-specific RTO and RPO, SLOs, workload and cost envelope SHALL be declared and benchmarked on a realistic managed instance including failover and contention; acknowledged financial facts SHALL NOT be lost without explicitly approved recovery semantics. A second durable protective store SHALL be built only if declared protection objectives cannot otherwise be met; the hold registry SHALL sit in a separate schema and role, isolated by failure domain. Event payloads SHALL carry references to a PII vault, with controlled immutable evidence exceptions. One codebase SHALL serve all operating models; dedicated deployments only by isolation, resilience or contract need. Measurable SLOs SHALL be defined per capability and servicing tier before the benchmark; numeric contractual SLA commitments SHALL be made only after benchmark and pilot evidence. Recovery objectives SHALL be one versioned table keyed by data class and isolation profile: authoritative financial data SHALL have RPO 0 within region through synchronous standby; cross-region recovery SHALL declare a nonzero RPO under approved recovery semantics; consistency SHALL be per sponsor cell plus the hold-registry version recorded at each dispatch. Critical command paths (reservation, release, dispatch, hold publication) SHALL have reserved connection pools and worker capacity; batch, reporting, rendering and AI work SHALL read from replicas or snapshots under statement timeouts in separately scaled worker roles of the same build, and the benchmark SHALL run statement generation concurrently with peak dispatch. After any restore or regional failover, dispatch for the affected cell SHALL stay blocked until hold publications after the restore point are re-applied, in-flight instructions are reconciled with the rail, and the cell's ledger is reconciled to the last bank feed; the recovery semantics SHALL be approved by the sponsor before production.
  - Provenance: resolves XID-EVA-001, UNB-XIN-EVA-013#Q1, XID-WFA-001, RR-UNB-RES-003-r1#Q1, SPD-ARX-001, SPD-ARX-002, XID-EVA-007, XID-SAS-001, RCD-004, UNB-XIN-SAS-005#Q1, XID-SAS-004, UNB-XIN-SAS-008#Q1; amends UNB-XIN-EVA-013, RR-UNB-RES-003-r1, REQ-X-047, REQ-X-020, UNB-XIN-SAS-005, UNB-XIN-SAS-008. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: architectural-support; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Empirical: benchmark against declared objectives (DG-09, DG-11).
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Acceptance:
    - AC-1: Given the benchmark report, when architecture acceptance is requested, then each capability's RTO, RPO and SLO is listed with measured results.
    - AC-2: Given an event containing a donor's name, when published to the outbox, then validation rejects it unless it is a registered evidence exception.
    - AC-3: Given a statement run for the largest declared sponsor at peak dispatch load, when benchmarked, then dispatch latency stays within its declared SLO.
    - AC-4: Given a regional failover, when the cell restarts, then dispatch remains blocked until hold re-application and in-flight reconciliation are recorded.
- **CNV-ARC-003 — Global hold fencing at irreversible dispatch:** Protected execution SHALL reference the immutable hold-publication version and SHALL check it atomically with authorization at the irreversible dispatch step (fencing token), with idempotency keys and reconciliation of in-flight payments; freshness checks alone SHALL NOT be relied on; unknown hold state SHALL fail closed. Because each sponsor has its own database, global hold publications SHALL be replicated into each sponsor database as inter-cell contract messages with a strictly increasing version; dispatch SHALL compare, in the same local transaction, the cell's applied version with a signed registry version beacon no older than an approved freshness bound, and SHALL fail closed for new dispatches when the bound is exceeded; a registry outage SHALL therefore stop only dispatches whose freshness bound has expired, never other tenants' reads or recommendations. The residual exposure window SHALL be declared as maximum beacon age plus p99 publication-to-applied lag, both basis-tagged [method: benchmark, CNV-ARC-001]; holds classified legally mandatory (sanctions, revocation, court order) SHALL additionally be confirmed by a synchronous registry check at the irreversible step, failing closed on timeout. A rail table SHALL give, for each rail, the irreversible step, the fence location and the post-hand-off response to a new hold (ACH recall or reversal request, partner cancel, stop payment, positive-pay void) with a deadline. This clause depends on acceptance of the database-per-sponsor architecture decision; if that decision changes, this clause SHALL be re-opened.
  - Provenance: resolves UNB-XIN-SAS-005#Q1; amends UNB-XIN-SAS-005. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Internal control: UNB-CTRL-049, UNB-CTRL-050; operator Unbound; service-organization report relevance SOC1; automated.
  - Acceptance:
    - AC-1: Given a hold published between authorization and dispatch, when dispatch runs, then the fencing check fails and nothing is transmitted.
    - AC-2: Given the hold registry unavailable for longer than the freshness bound, when a sponsor cell attempts dispatch, then dispatch is refused and all non-dispatch functions continue; given a stalled replica in cell A only, other cells continue to dispatch.
    - AC-3: Given a sanctions hold published one second after the latest beacon, when dispatch runs, then the synchronous mandatory-hold check blocks it.
    - AC-4: Given a hold published after an ACH file was transmitted, when detected, then the rail table's recall response is initiated within its deadline.
- **CNV-EXP-001 — Experience, brand and API lifecycle:** Every surface SHALL meet WCAG 2.2 AA with reflow; donor and advisor journeys and urgent approvals SHALL be mobile-first; complex accounting workbenches MAY be desktop-optimized. Phone navigation SHALL use a tab bar with inline actions (candidate) validated by moderated usability testing with assistive technology. The default unbranded theme SHALL be the Echelon brand manifest; sponsor overrides SHALL stay within verified contrast constraints. Forma SHALL be the preferred owner of reusable application-layer components; an Unbound adapter MAY remain indefinitely where the upstream service level is not met, with no duplicated design system, each adopted Echelon component pinned by release-manifest digest and covered by Unbound contract tests. Statements and acknowledgments SHALL be rendered from shared data snapshots to accessible HTML and PDF, tested for semantic determinism, accessibility and throughput; byte-identical re-rendering only where justified (Folio provides markup only; the PDF pipeline is Unbound-owned until an upstream renderer exists). Breaking changes to supported public APIs SHALL receive 18 months' notice with a version lifecycle and migration path, and documented emergency exceptions. Authoritative business facts SHALL be separated from engagement and coordination data with relevant audit history preserved. Published service levels and accessibility conformance SHALL be measured with a stated methodology before publication.
  - Provenance: resolves SPD-FIN-006, RCD-053, RCD-050, RCD-051, RCD-009, RR-UNB-API-STD-003-r1#Q1, RR-UNB-DATA-004-r1#Q1, SPD-CMP-002, SPD-DSH-001; amends RR-UNB-API-STD-003-r1, RR-UNB-DATA-004-r1. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Empirical: moderated usability tests (INT-020) and PDF accessibility verification.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Numeric parameters: 18 months [policy: API deprecation notice (commercial)].
  - Acceptance:
    - AC-1: Given a sponsor brand colour failing contrast, when the brand is compiled, then compilation fails with the failing pair named.
    - AC-2: Given a breaking API change, when released, then the old version remains supported for 18 months unless an emergency exception is recorded.
    - AC-3: Given a surface proposed for activation, when reviewed, then its Accessibility Conformance Report from automated tests plus manual assistive-technology testing shows no open WCAG 2.2 Level A or AA failure without a signed exception, and every Forma component used has a recorded conformance statement or Unbound test.
    - AC-4: Given an issued statement, when published, then an accessible HTML rendition from the same snapshot exists and any PDF passes PDF/UA-1 validation (veraPDF).
- **CNV-EXP-002 — Switching support for donors and charities:** Sponsor migration SHALL import each donor's full fund history (contributions, grants, acknowledgments, documents) from the predecessor platform with a reconciliation report, so switching sponsors' systems does not degrade the donor record. Charities already enrolled with a supported networked grant-payment partner SHALL be paid through that enrolment without a new Unbound enrolment. Lawful distribution-partner incentives (fixed-fee platform partnerships; referral payments only where registered and permitted) SHALL be defined before partner channels launch. A partner rail's enrolment SHALL satisfy CNV-PAY-002 only where the partner contract carries equivalent controls (owner verification, cooling-off at least as long as the sponsor's, notice to the charity's verified channels, and a destination-change event delivered to the sponsor before the next release); a rail lacking them SHALL be subject to CNV-PAY-002 in full.
  - Provenance: resolves SPD-CMP-003; amends UNB-RCN-ENT-007. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given a migrated fund, when the import completes, then contribution and grant totals reconcile to the predecessor's statements with no unexplained difference.
    - AC-2: Given a grantee enrolled with a supported rail partner whose contract carries the CNV-PAY-002 equivalent controls, when a grant is released, then no additional enrolment is requested.
- **CNV-PLT-009 — Service-organization evidence retention (ADV-T-013):** Unbound SHALL keep a 'service-organization evidence' class (operator access, change, deployment and IPE run records) separate from tenant records, holding only minimum tenant identifiers, for at least the longest of the SOC period plus three years, the Nacha audit horizon or the contractual period; it SHALL be listed on the exit certificate and survive tenant deletion.
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Security; financial system of record: n/a; policy owner: Unbound security; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound security.
  - Internal control: UNB-CTRL-074; operator Unbound; service-organization report relevance SOC1+SOC2; automated.
  - Acceptance:
    - AC-1: Given a sponsor exit and deletion of tenant data, when the evidence store is queried for that period, then operator-access and change records remain.
- **CNV-ARC-004 — Strata scope and authoritative history (ADV-T-018):** Strata SHALL be used for declarative schema governance where its supported PostgreSQL versions match production. Authoritative append-only history SHALL be an Unbound-owned ledger component on managed PostgreSQL. Strata version support SHALL be a precondition checked before each database major-version upgrade; deterministic mock interactions SHALL be Unbound-owned provider contracts until Strata virtualization exists.
  - Provenance: resolves Phase 3 feedback; amends UNB-TEC-STRATA-001, UNB-TEC-STRATA-002, UNB-TEC-STRATA-003, UNB-TEC-STRATA-004. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: architectural-support; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Acceptance:
    - AC-1: Given a planned PostgreSQL major upgrade outside Strata's supported range, when the upgrade is scheduled, then it is blocked or Strata is removed from that path by recorded decision.
- **CNV-PLT-010 — Engineering insider and change controls (ADV-T-023, ADV-T-035):** Modules tagged control-relevant (gate, ledger, reservations, holds, authority, dispatch) SHALL require two approvers on every change, neither the author; builds SHALL be signed with recorded provenance; each release SHALL publish to each sponsor a notice listing changed control-relevant modules; production data access by engineers SHALL follow CNV-ARC-002. Every authorization decision, policy evaluation, eligibility gate, capacity or fee computation and state transition SHALL be computed or re-verified on the server inside the command transaction; client-side results SHALL be advisory display only and API contracts SHALL NOT accept client-computed results. Ledger digests SHALL be anchored externally at least daily so tampering is detectable, and production database administrative roles SHALL be break-glass only, with two-person activation.
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Security; financial system of record: n/a; policy owner: Unbound security; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound security.
  - Internal control: UNB-CTRL-070, UNB-CTRL-071, UNB-CTRL-073; operator Unbound; service-organization report relevance SOC1+SOC2; automated.
  - Acceptance:
    - AC-1: Given a change to the dispatch module approved only by its author and one other engineer, when merged, then the release check fails.
    - AC-2: Given an API request carrying a client-computed capacity value, when processed, then the value is ignored and recomputed on the server.
    - AC-3: Given a ledger row altered outside the application, when the daily digest is verified, then the mismatch is detected and alerted.
- **CNV-PLT-011 — Contact-channel changes and accessible assurance (ADV-T-025, ADV-T-026):** A change to any verified contact channel SHALL notify the previous channel with a self-service revert link; for a sponsor-set period (default 72 hours) afterwards, successor and beneficiary changes, authority changes and grant recommendations above a sponsor threshold SHALL be held for confirmation. An AccessibleAssurancePath SHALL offer at least a one-time code mailed to the verified address, an outbound call to a verified number, assisted video with captioning and in-person or notarized verification, each reaching a defined contact assurance level; relay calls SHALL be accepted. Where a sponsor has not configured the recommendation threshold, the hold SHALL apply to all non-recurring recommendations above an Unbound default set by the Unbound product owner [policy default], and activation SHALL record the threshold in force. The post-change hold SHALL also cover bank-detail changes and, for charities, contact and authorised-signatory changes. Contact assurance levels SHALL be defined by factor combination: CAL0 unverified; CAL1 knowledge or caller identification only; CAL2 one-time code to a validated phone or email, or callback to the number on record; CAL3 an in-session authenticator challenge at AAL2 or above; each accessible method SHALL state the CAL it yields, and relay calls SHALL reach the same CAL as voice calls.
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Security; financial system of record: n/a; policy owner: Unbound security; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound security.
  - Numeric parameters: 72 hours [policy: sponsor (post-change hold)].
  - Internal control: UNB-CTRL-019, UNB-CTRL-020, UNB-CTRL-031; operator Unbound; service-organization report relevance SOC2; automated.
  - Acceptance:
    - AC-1: Given an email change followed by a successor change within 72 hours, when submitted, then the successor change is held and the previous email notified.
    - AC-2: Given a deaf donor without a smartphone, when they need CAL3 to change bank details, then a documented accessible path reaches CAL3.
    - AC-3: Given a sponsor without a configured threshold, when a contact change is followed by a large one-off recommendation, then the recommendation is held.
    - AC-4: Given a charity contact change followed by a bank-detail change within the hold period, when release is requested, then it is held.
    - AC-5: Given a relay-service call completing the CAL2 callback method, when assessed, then it reaches CAL2.
- **CNV-EXP-003 — Status projection and policy precedence (ADV-T-028, ADV-T-029):** A normative StatusProjection table SHALL map every reachable combination of payment, grant and contribution state dimensions to a donor label, a charity label, a staff label, an approved explanation and the next expected event or date, with a staff disclosure matrix by role. Policies SHALL follow the precedence law > platform non-removable > sponsor board policy > program terms > fund agreement > donor preference, and a policy pack SHALL be statically checked at adoption against the lattice and other active packs, accepting overlapping rules with different outcomes when the lattice orders their sources (recording the winner) and rejecting them only when both sources sit at the same level. In release 1 the adoption-time conflict check SHALL run against the sponsor's single configuration and the platform's non-removable rules (OD-P-GOV cut I).
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Service; financial system of record: n/a; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor or administrator service team; evidence owner: Sponsor operations owner.
  - Acceptance:
    - AC-1: Given a reachable state combination with no projection row, when the projection check runs, then it fails.
    - AC-2: Given a pack rule that would let program terms override a non-removable control, when adopted, then adoption is rejected.
- **CNV-ARC-005 — Long-running process versioning and database fleet operations (ADV-T-033, ADV-T-005):** Every process instance SHALL record the definition and policy versions it started under; a new definition SHALL apply only to new instances unless an explicit, tested migration function records a MigrationApplied fact per instance and never shortens a statutory or contractual deadline. Fleet provisioning SHALL be declarative and version-controlled (provider-native templates permitted; no Kubernetes or Terraform requirement), schema changes SHALL be expand/contract so release N runs against schema N-1 and N, migration state SHALL be recorded per tenant, and a failed tenant migration SHALL stop the rollout and leave other tenants serving. Security or control fixes that need no schema change SHALL have an emergency release path that continues to unaffected tenants while a failed tenant is tracked as an open risk. Each tenant SHALL have a restore test at least annually and credential and key rotation evidence; a PostgreSQL major-version upgrade drill SHALL be completed before architecture acceptance.
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: architectural-support; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Acceptance:
    - AC-1: Given a successor window shortened in a new policy version, when applied, then existing instances keep their original deadline.
    - AC-2: Given a migration failing for one tenant, when rolling out, then the rollout halts and other tenants remain on the prior compatible release.
    - AC-3: Given a failed migration on one tenant, when a flagged emergency fix is released, then other tenants receive it.
<!-- END ADOPTED: convergence-2026-11-24 -->

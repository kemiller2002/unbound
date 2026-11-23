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

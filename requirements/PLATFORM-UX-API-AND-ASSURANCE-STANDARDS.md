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
- **UNB-PLAT-008:** Partner data isolation SHALL support separate schemas, separate databases and dedicated infrastructure, selected per partner; all shared services must preserve tenant boundaries.
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

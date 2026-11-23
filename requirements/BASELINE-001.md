# Unbound Enterprise Requirements Baseline 001
Status: candidate requirements; NOT approved. Each item needs source validation, scenario coverage and acceptance evidence.

## Enterprise and economic outcomes
- UNB-ENT-001: The enterprise SHALL express donor charitable intent as explicit goals, restrictions, and measurable outcomes independent of legacy forms.
- UNB-ENT-002: Every capability SHALL have an accountable owner, outcome metric, operating cost hypothesis, and documented alternative.
- UNB-ENT-003: The organization SHALL support a documented sponsor business model, revenue/fee policy, and sensitivity-tested unit economics.
- UNB-ENT-004: Enterprise operating decisions SHALL be explainable and reviewable without relying on an agent's private reasoning.

## Legal, authority and governance
- UNB-LEG-001: Every material legal assertion SHALL record jurisdiction, authoritative source, currency, interpretation, and review status.
- UNB-LEG-002: The sponsor SHALL retain required legal ownership/control and final authority over distributions; donor recommendations SHALL not be treated as binding instructions.
- UNB-LEG-003: The enterprise SHALL distinguish donor advice, sponsor decisions, operational execution, and audit evidence.
- UNB-LEG-004: Conflicts, prohibited benefits, sanctions and other applicable restrictions SHALL have documented review and escalation paths, subject to jurisdiction-specific verification.
- UNB-LEG-005: Changes to material policies SHALL be versioned, approved by authorized decision makers, and linked to impacted tests.

## Donor and grant outcomes
- UNB-DON-001: Donors SHALL be able to express charitable goals and recommendations in accessible channels without being forced into a single legacy workflow.
- UNB-DON-002: Contribution status SHALL be transparent while explicitly distinguishing pledged, received, valued, liquidated, and available funds.
- UNB-GRT-001: Every grant recommendation SHALL have a traceable eligibility determination, sponsor decision, execution state and reconciliation evidence.
- UNB-GRT-002: The enterprise SHALL handle incomplete, ambiguous, ineligible, disputed and potentially fraudulent recommendations safely.
- UNB-GRT-003: Charity due diligence SHALL be proportionate to documented risk and applicable requirements, with human escalation for exceptions.

## Assets, finance and advisors
- UNB-AST-001: The enterprise SHALL model cash, public securities and complex noncash contributions without pretending all assets settle identically.
- UNB-AST-002: Asset valuation, restrictions, custody state and liquidation SHALL have distinct, auditable records.
- UNB-FIN-001: All simulated financial events SHALL be recorded in balanced, immutable or correction-traceable accounting entries.
- UNB-FIN-002: Grants, fees, contributions and investment events SHALL reconcile to independent simulated statements; discrepancies SHALL block unsupported finality claims.
- UNB-ADV-001: Advisors SHALL operate only within explicit, revocable, scoped delegated permissions.
> Adopted 2026-11-23: refined by `RR-UNB-ADV-001-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- UNB-ADV-002: Donor, advisor, sponsor and grantee roles SHALL have separately enforced access and information boundaries.

## AI, enterprise operations and technology
- UNB-AIO-001: Every agent action SHALL identify its permissions, inputs, policy version, result, evidence, and escalation route.
- UNB-AIO-002: High-impact, ambiguous, irreversible or legally reserved decisions SHALL require appropriate authorized human review.
- UNB-AIO-003: Agent unavailability, model changes and prompt injection SHALL not bypass deterministic financial or authorization controls.
- UNB-OPS-001: The enterprise SHALL support incident response, vendor failure, disputes, business continuity and recoverable audit trails.
- UNB-TEC-001: Integration SHALL use versioned contracts and the current Echelon Registry capability-discovery model; optional EF providers SHALL not break core operations.
- UNB-TEC-002: The intended implementation SHALL prioritize F#, WASM-first delivery, Limen and Forma where validated, managed persistence, and minimal operational complexity.
- UNB-TEC-003: Secrets, personal information and financial records SHALL have explicit classification, retention, access, and deletion policies.
- UNB-EXP-001: Every user journey SHALL be usable with keyboard, assistive technology, mobile layouts, and documented accessibility tests.
- UNB-EXP-002: Public identity SHALL be independently branded as Unbound, with transparent attribution to EF as its construction platform.
- UNB-VAL-001: Each requirement SHALL link to normal, exception and adversarial acceptance scenarios.
- UNB-VAL-002: Every claimed efficiency gain SHALL distinguish observed test measurements, modeled assumptions and unverified projections.
- UNB-VAL-003: The project SHALL log EF autonomous contributions, manual intervention, failures, cost, and reproducibility.

## Mandatory scenario families
Donor onboarding; restricted gifts; public securities; complex assets; grant to eligible charity; ambiguous charity identity; donor-benefit conflict; rejected recommendation; duplicate disbursement attempt; sanctions alert; market movement; reconciliation discrepancy; advisor revocation; accessibility failure; model outage; policy change; sponsor decision appeal; data correction; privacy incident.

## Open research blockers
Determine legal entity and sponsor model; applicable jurisdictions; public vs private reference scope; exact authoritative legal sources; charity verification methods; custody and banking partner assumptions; investment policy; fees; sponsor oversight model; regulatory obligations by operating geography; third-party assurance requirements. No blocker is silently deemed resolved.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-ADV-001-r1 (refines `UNB-ADV-001`) — adopted
- **Original:** `requirements/BASELINE-001.md:29` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** Every advisory-privilege holder and every professional advisor MUST act only through role grants that are explicit, scoped (fund set, command set, amount limits) and effective-dated. Grants MUST be revocable by the granting party or the sponsor, effective at the next authorization check, at most the policy-defined propagation bound. Advisory privileges MUST NOT confer trading, payment or approval authority (UNB-LEG-002).
- **Acceptance criteria:**
  - AC-1: Given an advisor revoked at T; when it submits at T+1s; then denied.
  - AC-2: Given a revoked advisor's pending recommendation; when revocation occurs; then it is Held and the donor is notified.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ADV-001-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-ENT-001 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-002 · **Parent:** CHARTER Requirements gate · **Decisions:** RCD-001
- **Requirement:** The program MUST maintain a versioned requirements-gate record listing each CHARTER gate criterion with owner, measure, denominator, evidence path and status, a named gate approver, and an overall PASS or FAIL. FAIL is the default until the approver signs.
- **Acceptance criteria:**
  - AC-1: Given any criterion without evidence; when gate status is computed; then the status is FAIL.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-002 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-003 · **Parent:** EXPANSION-ROADMAP · **Decisions:** RCD-013
- **Requirement:** The program MUST define a release-1 scope as an outcome slice, with every requirement family classified as in, deferred or excluded and given an owner. Requirements outside release 1 MUST NOT block the release-1 gate.
- **Acceptance criteria:**
  - AC-1: Given a REQ-X item classified 'deferred'; when the gate runs; then it is excluded from coverage denominators, and the exclusion is shown.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-002); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-003 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-007 · **Parent:** REQ-X-082 · **Decisions:** —
- **Requirement:** Every workflow, failure-mode and gap identifier MUST carry a registered namespace prefix (LS-WF, E2E-WF, ADM-WF, CW, SIM-FM, LSGAP). The traceability checker MUST reject bare WF-nn, FM-nn and GAP-nnn references in new text.
- **Acceptance criteria:**
  - AC-1: Given new text citing 'WF-13'; when the checker runs; then it fails with an ambiguity error.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-REG-001 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-REG-009 · **Parent:** UNB-LEG-001 · **Decisions:** —
- **Requirement:** The sponsor compliance owner MUST maintain a regulatory-change watch list including REG-142338-07 (proposed §4966 regulations), the 2026–27 Priority Guidance Plan item on §§4966/6033 DAF arrangements, OFAC rule changes, and state charitable-solicitation changes. Each item MUST link to the requirement IDs it would affect. A status change MUST open a requirements impact review within 10 business days (sponsor-configurable).
- **Acceptance criteria:**
  - AC-1: Given A watch item changes status to final; when the change is recorded; then an impact review task lists every linked requirement ID.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-REG-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-REG-003 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-REG-006 · **Parent:** UNB-LEG-004 · **Decisions:** RCD-006, RCD-015
- **Requirement:** When an authorized human confirms a sanctions true match involving property in the sponsor's possession, the system MUST record the property as blocked, prevent any movement, generate the blocked-property report data due to OFAC within 10 business days, track the annual report, and retain all related records for at least 10 years.
- **Validation still required:** RCD-006: Counsel Q2; RCD-015: Counsel review of ScreeningScope matrix
- **Acceptance criteria:**
  - AC-1: Given A confirmed true match on a pending grant recipient; when confirmation is recorded; then the grant is Blocked, funds cannot be released, a report obligation due in 10 business days exists, retention class OFAC applied.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-REG-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ARC-002 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ARC-002 · **Parent:** REQ-X-030 · **Decisions:** RCD-003
- **Requirement:** The system MUST resolve all user-facing domain terms (e.g., 'fund', 'grant', 'advisor') through a versioned, sponsor-scoped terminology catalog applied identically in UI, documents, email, IVR prompts and AI assistant output; legally significant phrases (e.g., the exclusive-legal-control acknowledgment statement) MUST be non-overridable.
- **Validation still required:** RCD-003: Owner may rename the platform; if so, choose a name other than Strata
- **Acceptance criteria:**
  - AC-1: Given A sponsor renames 'fund' to 'giving account'; when a statement, an email and the assistant answer are generated; then all three use 'giving account'; the acknowledgment's statutory sentence is unchanged.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ARC-002); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

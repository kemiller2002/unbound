# DAF Landscape conversion, wave 2: capabilities, workflows and interface boundaries
Date: 2026-11-21 | Status: candidate requirements, not approved

## Reviewed sources
- [Capability map](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/02-business-capabilities/capability-map.md): 29 named capabilities in six domains, with accountable human roles and execution modes.
- [Workflow inventory](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/03-workflow-redesign/README.md): WF-01 through WF-24, grouped in six detailed workflow specifications.
- [API catalog](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/04-enterprise-apis/api-catalog.md): 36 interface domains and five illustrative contracts; requirements analysis, not production interfaces.
- [Authority boundaries](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/05-agent-architecture/authority-boundaries.md): AG-01 through AG-19 and invariants INV-A1 through INV-A8.

## Capability coverage requirement
UNB-CAP-001: Every source capability C1.1–C6.4 SHALL map to an enterprise outcome, accountable human role, executor mode, control owner, interface boundary and acceptance scenarios. Six groups:
- C1.1–C1.5 relationship, access, education, advisor integration and servicing.
- C2.1–C2.5 contributions, complex assets, custody, investment and liquidity.
- C3.1–C3.5 recipient registry, recommendation, decision, payment and post-grant obligations.
- C4.1–C4.5 ledger, reconciliation, fees, reporting and evidence.
- C5.1–C5.6 policy, fraud, exception routing, governance, counterparty risk and privacy.
- C6.1–C6.4 philanthropic intelligence, telemetry, digital twin and controlled improvement.
UNB-CAP-002: Accountability and software execution SHALL be separate attributes; no automated executor eliminates the named human accountable for its capability.
UNB-CAP-003: Legacy departmental handoffs, status inquiries and duplicate data entry SHALL be treated as avoidable failure mechanisms, not standalone capabilities.
UNB-CAP-004: Each capability SHALL be classified as sponsor-specific, reusable infrastructure, shared service, or external dependency, with explicit tenant isolation and commercial packaging implications.

## Workflow outcome requirements
UNB-WF-001: Routine cash contribution through domestic grant (WF-01) SHALL trace settlement, acknowledgment, recommendation, sponsor authorization, payment and recipient evidence without unnecessary manual handoffs.
UNB-WF-002: Securities gifts (WF-02) SHALL distinguish transfer intent, orphan deposits, settlement, gift-date determination, valuation and booking.
UNB-WF-003: Complex noncash gifts (WF-03) SHALL require documented acceptance, specialist authority, valuation and tax/reporting checks.
UNB-WF-004: Cryptocurrency gifts (WF-04) SHALL use asset acceptance, chain/source-of-funds controls, custody and disposition states; eligibility is a policy/legal decision, not presumed.
UNB-WF-005: High-volume and recurring grants (WF-05) SHALL preserve idempotency, policy gates, liquidity, bounded fan-out and exception capacity under year-end surge.
UNB-WF-006: Foreign grants (WF-06) SHALL preserve equivalency/expenditure-responsibility review and sanctions controls where applicable.
UNB-WF-007: Domestic non-charity grants (WF-07) SHALL require appropriate expenditure-responsibility decisioning and records.
UNB-WF-008: Fiscal-sponsor grants (WF-08) SHALL resolve legal payee, project relationship, purpose restrictions and recipient evidence.
UNB-WF-009: Mid-flight eligibility changes (WF-09) SHALL trigger holds or controlled post-payment handling based on transaction state.
UNB-WF-010: Returns and payee-bank changes (WF-10) SHALL require payee integrity controls, independent confirmation where risk warrants, and reversible reconciliation.
UNB-WF-011: Donor AI agents (WF-11) SHALL act only within explicit scoped delegation and confirmation thresholds; requests never bypass sponsor authority.
UNB-WF-012: Advisor onboarding (WF-12) SHALL verify institution, individual authority, privileges, revocation and integration contracts.
UNB-WF-013: Donor death and succession (WF-13) SHALL handle document conflicts, authority transitions and dispute holds.
UNB-WF-014: Outbound transfer (WF-14) SHALL preserve history, restrictions, accounting reconciliation and sponsor approval.
> Adopted 2026-11-23: refined by `RR-UNB-WF-014-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
UNB-WF-015: Dormant accounts (WF-15) SHALL follow approved notice, successor and charitable-default policies.
> Adopted 2026-11-23: refined by `RR-UNB-WF-015-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
UNB-WF-016: Documentation requests (WF-16) SHALL produce accurate evidence-grounded documents with authorization and privacy controls.
UNB-WF-017: Liquidity shocks (WF-17) SHALL model available cash, market movements, custodian outages and approvals outside the playbook.
UNB-WF-018: NAV, unitization and fees (WF-18) SHALL track valuation inputs, pricing exceptions, effective schedules and corrections.
UNB-WF-019: Continuous reconciliation (WF-19) SHALL compare ledger, custodian, bank and payment-rail states, and escalate breaks.
UNB-WF-020: Continuous close and filings (WF-20) SHALL produce traceable reports and preserve accountable officer sign-off.
UNB-WF-021: State registration deadlines (WF-21) SHALL track applicability, changes, reminders and authorized submission.
UNB-WF-022: Regulatory change (WF-22) SHALL generate reviewed policy changes, back-tests, approval, canary and rollback evidence.
UNB-WF-023: Agent permission changes (WF-23) SHALL require independent evaluation and approval with explicit rollback.
UNB-WF-024: Board oversight (WF-24) SHALL separate reproducible evidence snapshots from interpretive commentary and preserve board authority.

Each WF requirement must eventually link to its detailed source specification, exact event triggers, state transitions, negative paths, risk metrics and scenario tests. Source latency targets are targets, not measured results.

## API and architectural requirements
UNB-API-001: Evaluate all 36 API domains against capability ownership and consumer needs before freezing interfaces.
UNB-API-002: Externally exposed writes SHALL express requests rather than grant approvals, money instructions, trades, direct ledger postings or policy publication.
UNB-API-003: A shared authorization model SHALL govern humans, institutions and agent principals; no borrowed credentials or self-escalation.
UNB-API-004: Mutating commands SHALL have idempotency, concurrency handling, evidence references and stable machine-readable errors.
UNB-API-005: Tenant boundaries SHALL prevent cross-sponsor personal-data exposure; explicitly shared data must have separate rights and provenance.
UNB-API-006: External partner events SHALL be signed, replay-safe, deduplicated and traceable to authoritative states.
UNB-API-007: Favor one F# modular monolith with clear internal capability boundaries; API-first SHALL NOT imply microservices-first. This remains subject to EF current-version compatibility review.
UNB-API-008: Treat the five illustrative OpenAPI documents as design references, not verified production contracts; defer implementation until requirements approval.

## Agent authority requirements
UNB-AGT-001: Map AG-01..AG-19 to the current EF agent catalog, deduplicating roles and distinguishing deterministic jobs from actual agents.
UNB-AGT-002: AI agents may read scoped information, propose, flag, draft or open cases; they SHALL NOT independently clear flags, write authoritative records, instruct payments/trades or approve their own output.
UNB-AGT-003: Tool broker enforcement SHALL remove prohibited tools rather than rely on prompts.
UNB-AGT-004: The eight source invariants INV-A1..INV-A8 SHALL be translated to explicit negative and property tests, including approval actor type, flag clearance, payment gate, delegation confirmation, segregation of duties and agent-chain quarantine.
UNB-AGT-005: Policy/rule decisions and human decisions SHALL be independently identifiable, versioned and auditable.

## Next conversion and validation actions
1. Reconcile 29 capability IDs and 24 workflow IDs to detailed requirement records; identify shared controls without losing traceability.
2. Review the 36-row API CSV, 19-agent catalog, events CSV, canonical data model, regulatory applicability matrix and controls CSV.
3. Convert each design into outcome, constraint, hypothesis, acceptance test or rejected alternative. Do not automatically mandate an architecture from research.
4. Run contradiction checks between the existing Unbound baseline and source operating model; explicitly document conflicts.
5. Confirm current Conditor installation and EF capability selection before executing any implementation. Requirements analysis and GitHub documentation may continue without falsely claiming installation.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-WF-015-r1 (refines `UNB-WF-015`) — adopted-provisional
- **Original:** `requirements/DAF-LANDSCAPE-CONVERSION-WAVE-02.md:37` @ `777a70d` (kept verbatim) · **Decisions:** RCD-022
- **Requirement:** Fund inactivity MUST be governed by a sponsor-approved, versioned dormancy policy. Parameters: the activity events that reset the clock; Quiet, Inactive and Unresponsive thresholds; required outreach channels and attempts; and the default disposition (charitable default to sponsor-designated purposes or charities, transfer to a donor-named charitable beneficiary under UNB-SUCC-011, or continued holding). The lifecycle is Active > Quiet > Inactive > Unresponsive > DispositionPending > Disposed or Reactivated. Any disposition distributing assets MUST be an A4 sponsor decision run through the normal grant lifecycle. Escheat handling MUST NOT apply to fund balances unless counsel records it as applicable.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given an Inactive fund; when the donor affirms activity; then the fund is Active and the clock resets.
  - AC-2: Given an Unresponsive fund with fewer outreach attempts than the policy minimum; when disposition is proposed; then rejected.
  - AC-3: Given a disposition grant to an ineligible charity; when eligibility runs; then held.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-WF-015-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-WF-014-r1 (refines `UNB-WF-014`) — adopted-provisional
- **Original:** `requirements/DAF-LANDSCAPE-CONVERSION-WAVE-02.md:36` @ `777a70d` (kept verbatim) · **Decisions:** RCD-022
- **Requirement:** A donor-recommended transfer of all or part of a fund to another sponsoring organization MUST be processed as a sponsor distribution through the standard grant lifecycle, with purpose code DAF_TRANSFER_OUT, unless counsel records a different classification. It MUST include: receiving-organization eligibility; the sponsor's discretion to approve or decline; cash as the default, with in-kind only where both custodians accept and policy allows; carrying over donor-stated restrictions as non-binding recommendation text; a transfer history package to the receiving sponsor when the donor consents; and, for full transfers, fund closure under UNB-RCN-ENT-006.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given a full transfer approved and settled; when post-settlement processing runs; then the fund enters the closing workflow.
  - AC-2: Given no consent; when the package is requested; then no history is sent.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-WF-014-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-ENT-005 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-019 · **Parent:** UNB-WF-015 · **Decisions:** RCD-022
- **Requirement:** The system MUST implement the sponsor-parameterized dormancy lifecycle in RR-UNB-WF-015-r1. Escheat processing MUST be limited to payment instruments unless counsel records otherwise.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given no approved dormancy policy; when a fund passes the time thresholds; then no disposition is proposed and the policy gap is reported.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-005); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-007 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-021 · **Parent:** UNB-WF-014 · **Decisions:** RCD-022
- **Requirement:** Outbound transfers to another sponsoring organization MUST follow RR-UNB-WF-014-r1, and inbound transfers MUST be classified by a counsel-approved rule covering contributor of record, acknowledgment, restrictions and successor plans.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given an inbound transfer; when it is received; then classification is recorded before capacity becomes grantable.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-007); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-012 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-029 · **Parent:** REQ-X-060 · **Decisions:** RCD-024
- **Requirement:** The system MUST produce the DAF annual reporting data package defined in RR-REQ-X-060-r1.
- **Validation still required:** RCD-024: Counsel on future product types
- **Acceptance criteria:**
  - AC-1: Given a year with DAF_TRANSFER_OUT grants; when the package is built; then the transfer subset is reported separately and reconciles to the grant total.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-012); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

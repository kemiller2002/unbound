# Proposed canonical cross-domain requirements and acceptance criteria

Date: 2026-11-22. **Draft, not formally approved.** This document is a proposed reconciliation overlay. Original registers remain authoritative until explicit migration. No simulation below has been run. Five substantive challenged directions have been recorded from user decisions, and TVC-034 remains source-verification pending. All remain subject to formal governance, legal and technical validation.

## Reconciliation method
- Requirements describe observable business/control outcomes, not prematurely selected implementation technology.
- Every requirement has stable ID, linked source-question IDs, a falsifiable acceptance example and unresolved dependencies.
- No fixed thresholds, legal interpretations or third-party capabilities are invented.
- A failing safety gate blocks value movement while allowing safe observation and incident handling.

## Proposed canonical requirements

### REQ-X-001: Authority and agent delegation

**Source questions:** OD-01, OD-14, OD-17, OD-21, TVC-003, TVC-026, TVC-041, TVF-047, TVF-153, TVF-251.

**Normative proposed requirement:** Every agent/staff/advisor command has verified principal, scope, policy version and authorization at execution. No person, agent or automated process may approve an item it originated, submitted, prepared or materially changed; separate logins do not establish independence. Donor confirmation authenticates recommendation intent, not sponsor approval. Independently governed sponsor-delegated deterministic policy may authorize routine grants only when board and counsel validate delegation, publication segregation and independent control. Material changes invalidate prior approval. Advisor-originated agent delegation remains disabled until validated.

**Acceptance scenario (not executed):** Given a revoked donor delegation, when an agent submits a new recipient grant, then the command is rejected, no reservation or payment is created, and evidence identifies the revocation.

**Open dependencies:** Board/counsel authorization of deterministic approvals, independence control design and adversarial testing.


### REQ-X-002: Atomic grant approval and reservation

**Source questions:** OD-06, OD-07, OD-08, OD-10, OD-13, TVC-036, TVC-037, TVC-038, TVF-001, TVF-009.

**Normative proposed requirement:** Routine grant approval and currency reservation commit atomically against net capacity under concurrency; exceptional pending-funding approvals cannot create external commitment.

**Acceptance scenario (not executed):** Given two simultaneous grants exceeding available capacity together, when processed, then no overcommitment occurs and failed grant has neither approval nor reservation.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-003: Recognition and temporal reconstruction

**Source questions:** OD-19, TVC-001, TVR-DATA-001-Q, TVF-144.

**Normative proposed requirement:** Approval, funding, communication, payable recognition, payment and finality are separately timestamped, and prior-known state can be reconstructed; accounting recognition requires approved CPA policy.

**Acceptance scenario (not executed):** Given year-end approval and following-year communication and payment, when queried as of each date, then correct contemporaneous events and policy-driven accounting classifications are reproduced.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-004: Payment unknown and non-receipt proof

**Source questions:** OD-20, TVC-024, TVC-035, TVC-039, TVR-PAY-001-Q, TVR-PAY-002-Q, TVR-PAY-007-Q, TVF-003, TVF-024, TVF-231, TVF-249.

**Normative proposed requirement:** Unknown payment state blocks duplicate submission until contractually sufficient independently controlled evidence proves non-receipt; durable payment identity survives restore.

**Acceptance scenario (not executed):** Given provider timeout and subsequent restore, when an operator retries without qualifying proof, then no second instruction is sent and original identity remains intact.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-005: Payee and external destination integrity

**Source questions:** OD-22, TVF-222, TVF-223, TVF-233, TVF-234, TVF-236, TVF-250, TVR-PAY-005-Q.

**Normative proposed requirement:** Every automated payment requires independently verifiable proof both that the destination is authorized and controlled by the intended charity and that the actual execution destination matches the sponsor-controlled authorization, including cryptographically or independently bound tokenized rails. Provider self-attestation alone is insufficient. Changes require independent authorization without self-approval. Missing or conflicting proof blocks automatic execution; noncompliant providers/rails are ineligible for fully automated payment. Reconcile authorization, execution and settlement evidence.

**Acceptance scenario (not executed):** Given a provider-substituted account, when provider returns a self-issued matching fingerprint but sponsor-controlled evidence differs, then release is blocked.

**Open dependencies:** Rail/provider-specific independently verifiable destination proof and contract testing.


### REQ-X-006: Outage, restoration and emergency safeguards

**Source questions:** OD-22, TVF-139, TVF-227, TVF-228, TVF-230, TVF-254, TVR-DR-001-Q, TVR-PAY-004-Q.

**Normative proposed requirement:** Critical financial and security commits must be durably quorum-replicated before success acknowledgment (RPO zero for acknowledged authoritative commits). Use globally collision-resistant business IDs, stable intent/idempotency keys, a fenced single writer, quorum-based leadership and replica catch-up before failback; suspend authoritative financial writes without safe quorum. An independently durable non-authoritative intake plane may acknowledge queued requests, never approvals, reservations or payments; otherwise submissions fail closed. Replay only after reconciliation, deduplication and current-state revalidation, prioritizing protective holds and cancellations. A separately durable protective-only emergency plane may freeze releases, impose holds, revoke access and capture incident evidence but may not approve, change payees/balances or move funds. Independent authorization is required to remove holds after recovery. External payment outcomes remain Unknown until independently reconciled; no automatic duplicate execution.

**Acceptance scenario (not executed):** Given backup predating a hold and cancellation, when restored, then payment release remains disabled until later security events and protective requests are reconciled.

**Open dependencies:** DR quorum topology, RTO target, independent queue durability, emergency-plane threat model, fault injection and provider reconciliation.


### REQ-X-007: Charity verification and sanctions

**Source questions:** OD-02, TVC-006, TVC-007, TVC-040, TVA-009, TVA-028, TVR-SAN-001-Q, TVR-SAN-002-Q, TVF-027, TVF-048.

**Normative proposed requirement:** Unbound maintains one authoritative, standardized charity-vetting framework and reusable evidence/status across sponsors. A vetted charity does not undergo duplicate routine vetting solely because another sponsor uses it. Record scope, evidence provenance, effective date and expiry; reverify at least every 180 days, on material changes, and check current sanctions/eligibility at execution. Adverse findings propagate to all affected sponsors. Sponsor-specific grant restrictions and approvals remain distinct from common charity vetting; no self-approved vetting exceptions. Cross-sponsor legal reliance, liability, jurisdictions and recipient classes require counsel validation; model never clears holds.

**Acceptance scenario (not executed):** Given a recipient verified 30 days ago and sanctioned today, when payment releases, then the current adverse signal holds the grant despite valid periodic review.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-008: Donor death, succession and recurring plans

**Source questions:** OD-03, OD-11, TVC-030, TVC-033, TVC-044, TVR-GR-008-Q.

**Normative proposed requirement:** Donor death does not automatically cancel valid sponsor-approved unpaid grants; future instructions suspend until successor authority verified; each recurring instance rechecks authority and eligibility.

**Acceptance scenario (not executed):** Given notice of donor death before next recurring instance, when scheduler executes, then new instance is held while prior valid approved grant follows its separate policy.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-009: Reversible contribution and collection

**Source questions:** OD-04, TVC-010, TVR-CON-001-Q, TVR-CON-003-Q, TVF-006, EXT-06.

**Normative proposed requirement:** Rail-specific receipt, provisional availability, acknowledgment and finality remain distinct; returned funds trigger receivable, sponsor exposure and recovery classification without erasing disbursements.

**Acceptance scenario (not executed):** Given a provisionally credited ACH that funds a grant then returns, when reconciled, then original payment remains recorded and receivable and loss attribution are opened.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-010: Independent finance review and loss allocation

**Source questions:** OD-05, OD-09, OD-10, OD-13, OD-17, OD-18, OD-20, TVC-004, TVC-005, TVC-028, TVR-ORG-003-Q.

**Normative proposed requirement:** Independent finance reviewer must attest reconciliations outside the payment chain; loss allocations distinguish investment risk from sponsor/vendor operational error and require governed recovery.

**Acceptance scenario (not executed):** Given a Controller-prepared reconciliation, when Controller attempts to self-certify it, then system denies attestation and routes to segregated reviewer.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-011: UI, widgets and accessible confirmation

**Source questions:** OD-15, OD-16, TVC-008, TVC-009, TVC-011, TVC-012, TVC-025, TVC-031, TVC-032, EXT-08.

**Normative proposed requirement:** Donor/partner statuses represent specific domain facts, not generic 'Accepted'; offline financial commands cannot execute, and accessible assisted channels meet equivalent authorization requirements.

**Acceptance scenario (not executed):** Given widget submission acknowledged before sponsor approval, when status displayed, then UI states 'Submitted' and does not claim approved, funded or paid.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-012: Data retention and architectural neutrality

**Source questions:** OD-12, OD-15, OD-16, TVC-001, TVC-002, TVC-015, TVC-016, TVR-GOV-001-Q, TVR-DATA-001-Q.

**Normative proposed requirement:** Business requirements specify reconstructability, evidence provenance, privacy and lawful record-class retention; stack/event-sourcing selection stays ADR-gated.

**Acceptance scenario (not executed):** Given a legal hold and deletion request, when retention workflow evaluates record, then hold prevents unlawful destruction and the disposition is auditable.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-013: Third-party dependency and assumptions

**Source questions:** TVA-002, TVA-003, TVA-004, TVA-005, TVA-006, TVA-014, TVA-018, TVA-020, TVA-021, TVA-022, TVA-024, EXT-01, EXT-02, EXT-03, EXT-04, EXT-05, EXT-09, TVF-237, TVF-238.

**Normative proposed requirement:** Do not treat provider capabilities, economic projections, cross-sponsor legal reliance or deprovisioning latency as facts until supported by primary evidence and enforceable terms. The business direction is common standardized charity vetting with reusable platform-wide evidence; whether sponsors may legally rely on a shared determination requires counsel-reviewed governance and contracts.

**Acceptance scenario (not executed):** Given a provider without independently provable non-receipt, when onboarding is evaluated, then the payment integration is not marked eligible for automatic retry.

**Open dependencies:** Counsel validation of cross-sponsor reliance and liability, plus evidence of provider capabilities.


### REQ-X-014: Regulatory classification and reviewer bias

**Source questions:** TVC-029, TVA-010, TVA-011, TVR-UX-006-Q, TVR-REG-001-Q, TVR-REG-003-Q, TVR-CMP-004-Q, TVR-GR-006-Q, EXT-07.

**Normative proposed requirement:** Risky recipient types, conflicting IRS sources and sanctions outcomes follow legally approved classification; AI risk summaries are evaluated for missed cases and reviewer bias.

**Acceptance scenario (not executed):** Given conflicting eligibility sources and a persuasive AI summary, when reviewer acts, then original evidence and hold remain visible and AI cannot unilaterally clear the issue.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-015: Risk windows, exposure and security change

**Source questions:** TVA-012, TVF-016, TVF-239, TVF-240, TVF-241, TVF-243, TVF-247, TVF-251, TVF-253, TVR-SEC-004-Q.

**Normative proposed requirement:** Risk limits use rolling windows, principal/recipient novelty and cumulative projected exposure; privilege changes and returns cannot reset limits or evade step-up.

**Acceptance scenario (not executed):** Given a sequence of subthreshold grants to one recipient, when cumulative exposure exceeds policy, then next release is blocked or escalated before transfer.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-016: Incident evidence and independent channels

**Source questions:** TVF-131, TVF-229, TVF-235, TVF-236, TVF-242, TVF-244, TVF-245, TVF-246, TVF-248.

**Normative proposed requirement:** Incident records define earliest credible compromise and lookback, evidence provenance, triage owner and aging; sources sharing control are not counted as independent.

**Acceptance scenario (not executed):** Given sponsor email compromise and two vendor feeds under common control, when independent confirmation is required, then those feeds cannot satisfy two-source rule.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


## Final clarification queue (unchanged)

TVC-034 remains unverified as a source citation. User design directions have been recorded for TVC-041, TVA-020, TVR-DR-001-Q, TVF-228 and TVF-234, but implementation, legal/governance authority and test evidence are not thereby approved.

## Canonical migration controls

1. For each requirement, enumerate conflicting original clauses and classify them as superseded, retained, narrowed or still unresolved.
2. Update originals only after authority approval, with bidirectional source links and a dated supersession record.
3. Validate domain state-machine transitions, accessible UI status semantics, and payment/authority controls using cross-domain simulations.
4. Keep legal/accounting/provider evidence gates explicit. A draft acceptance scenario is not evidence of passing a test.


## Traceability coverage

139 of 183 registry questions are directly linked in the 16 proposed canonical requirements. The remaining 44 questions retain their own registry dispositions and are **not** claimed reconciled by this overlay: TVC-013, TVC-014, TVC-017, TVC-018, TVC-019, TVC-020, TVC-021, TVC-022, TVC-023, TVC-027, TVC-034, TVC-042, TVC-043, TVA-001, TVA-007, TVA-008, TVA-013, TVA-015, TVA-016, TVA-017, TVA-019, TVA-023, TVA-025, TVA-026, TVA-027, TVA-029, TVA-030, TVR-GR-001-Q, TVR-FIN-002-Q, TVR-SEC-006-Q, TVR-CMP-003-Q, TVR-WID-001-Q, TVR-WID-002-Q, TVR-WID-006-Q, TVR-UX-003-Q, TVR-PAY-006-Q, TVR-SEC-007-Q, TVR-SEC-008-Q, TVR-SEC-009-Q, TVR-SEC-010-Q, TVF-149, TVF-226, TVF-232, TVF-252.

## Supplemental source-question mapping (traceability only)

The 44 initially unlinked registry questions are additionally mapped below to an existing canonical requirement. **Mapping is not a claim of resolved source-text conflict or validated acceptance.** In particular TVC-034 remains deferred despite being linked to the records/traceability area.

- **REQ-X-001:** TVC-042, TVF-252.
- **REQ-X-002:** TVC-021, TVC-022, TVC-043, TVR-GR-001-Q, TVR-FIN-002-Q.
- **REQ-X-003:** TVC-023, TVA-016.
- **REQ-X-004:** TVF-226.
- **REQ-X-005:** TVF-232, TVR-PAY-006-Q, TVR-SEC-008-Q, TVR-SEC-009-Q.
- **REQ-X-006:** TVR-SEC-006-Q.
- **REQ-X-007:** TVF-149.
- **REQ-X-008:** TVA-027.
- **REQ-X-009:** TVA-007, TVA-008, TVA-015.
- **REQ-X-010:** TVA-017, TVR-CMP-003-Q.
- **REQ-X-011:** TVC-013, TVC-014, TVA-013, TVA-019, TVA-025, TVA-026, TVR-WID-001-Q, TVR-WID-002-Q, TVR-WID-006-Q, TVR-UX-003-Q, TVR-SEC-007-Q, TVR-SEC-010-Q.
- **REQ-X-012:** TVC-027, TVC-034, TVA-023.
- **REQ-X-013:** TVC-017, TVC-018, TVC-019, TVC-020, TVA-001, TVA-029, TVA-030.

All 183 questions now have at least one proposed canonical-requirement linkage across the main mapping and this supplement. This does not constitute full requirement-level source reconciliation.

## 2026-11-23 integrated architecture challenge and validation gates

1. **Single standard versus legal sponsor responsibility:** Share charity evidence and routine determination across sponsors, but do not infer that each sponsor's fiduciary/legal duty disappears. Counsel must define reliance agreements, liability, jurisdictional scope and any category-specific supplements without duplicating routine investigation.
2. **Automation versus independent approval:** The originator cannot approve their own item, even using a different identity or delegated agent. A separately governed board-authorized policy may make routine decisions only with independently controlled policy publication and evidence of delegation; donor confirmation is not sponsor approval.
3. **Zero-loss versus availability:** RPO zero applies to acknowledged authoritative financial/security commits, not every in-flight message or external bank action. Quorum failure pauses financial writes. Test site loss, network partition, witness failure, fencing and return/failback, with measurable RTO and latency costs.
4. **Queue versus commitment:** Durable intake acknowledgment must not masquerade as financial commitment. If the queue lacks independent durability, fail closed. On recovery reconcile protective events first, deduplicate stable intent identities and re-evaluate authority, charity status, funding and policy before execution.
5. **Protective plane versus attack surface:** Emergency capability is protective-only, independently durable and separately authorized; it must not become an alternative payment route. Test compromise of emergency credentials, replay, forged hold release and ordering under partial recovery.
6. **Payment verification versus provider opacity:** Verify charity control of destination and provider execution destination independently. Tokenization requires an independently verifiable binding, not provider-only fingerprints. Test destination substitution, account change, settlement mismatch and unavailable verification; disqualify nonconforming automated rails.
7. **Cross-cutting identity:** Globally collision-resistant IDs prevent ID reuse, not duplicate business effects. Require idempotency identity, replay protection, deterministic ordering rules, and authoritative command outcomes.
8. **Traceability:** TVC-034 currently quotes REG-007 as referring to REG-057 and REG-061, with REG-061 described in the contradiction register as NYC LL144. The original regulatory requirements source has not yet been located/verified. REG-058 is a guess and MUST NOT be substituted without original-source inspection. Treat as an editorial cross-reference defect pending verification, not a substantive policy decision.

**Validation status:** All above are draft proposed requirements. No production implementation, fault-injection test, external provider certification, counsel opinion, board delegation or source-register migration is claimed.

## REQ-X-017: Rules-first, AI-second, independent-human-third decision routing (2026-11-23)

**User-selected principle:** Tier 1 version-pinned deterministic policy evaluates every eligible case first, resolving safely routine approvals/rejections/holds without AI or human intervention. Tier 2 invokes AI only for ambiguous, unstructured or conflicting evidence and only within explicitly delegated authority; default AI output is an evidence-backed recommendation or request for more information, never unilateral approval of consequential grants, payments, sanctions exceptions or hold removal. Tier 3 routes remaining cases to independently authorized human reviewers who cannot approve their own or materially modified work. Hard prohibitions and unsafe states remain held regardless of tier. All tiers preserve rule traces, provenance, authority, status semantics, bounded costs, audit evidence and case-specific escalation reasons. Sponsor-delegated automated approvals require independent board-approved policy governance and legal validation.

**Acceptance (not executed):** A complete low-risk grant passes deterministic rules and incurs zero AI calls and zero human touches; ambiguous recipient documentation invokes a bounded evidence-only AI investigation; a sanctions match remains blocked even when AI confidently recommends release; a human who originated the case cannot approve it; repeated AI failure routes to a safe hold or independently authorized human, never a fail-open decision.

**Sources:** User directive 2026-11-23, OD-01, OD-02, OD-14, OD-17, OD-21, TVC-003, TVC-006, TVC-007, TVC-041, TVF-153. Detailed adversarial specifications: `ADVERSARIAL-VALIDATION-AND-TIERED-DECISION-ARCHITECTURE-2026-11-23.md` (ADV-001 through ADV-030). No tests executed.

## 2026-11-23 enterprise decision inventory

The proposed implementation-level routing catalog is `ENTERPRISE-DECISION-INVENTORY-2026-11-23.md`, covering 62 decision types across nine domains. It applies REQ-X-017 to each process with rules-first behavior, conditional AI evidence work, independently authorized human exceptions, hard stops and acceptance invariants. This is initial inventory coverage, not exhaustive proof or executed tests. Cross-check against `ADVERSARIAL-VALIDATION-AND-TIERED-DECISION-ARCHITECTURE-2026-11-23.md` and original registers before source migration.

## REQ-X-018: Policy precedence, evidence trust and critical workflow safety (2026-11-23)

**Proposed:** Separate authority precedence from factual evidence trust. Nonwaivable legal requirements and platform safety controls prevail over common charity vetting, sponsor-specific rules, donor mandates and operational/AI optimizations; sponsors may impose stricter valid grant restrictions, not weaken common mandatory controls. Evidence is evaluated by fact-specific provenance, directness, independence, jurisdiction and freshness, with explicit Unknown/Disputed/Stale outcomes. Before AI, run bounded deterministic evidence enrichment. No AI/human escalation overrides a hard prohibition. All 16 safety gates and the charity, grant, payment and DR workflow transitions require execution-time rechecks and auditable versions.

**Acceptance (not executed):** A sponsor configuration that bypasses independently verified payment destination is rejected; an adverse charity signal arriving after approval holds payment; two feeds under one provider are not counted as independent corroboration; missing evidence triggers trusted retrieval without AI when available; a policy-complete grant succeeds with AI offline. See `POLICY-PRECEDENCE-EVIDENCE-TRUST-AND-CRITICAL-WORKFLOWS-2026-11-23.md` and ADV-053 through ADV-068.

**Open:** legal classification, sponsor board approval, provider proof, exact typed schemas, source migration and test execution.

## REQ-X-019: Typed critical decision contracts and concurrent-state safety (2026-11-23)

**Proposed:** Every critical charity, grant, payment and DR decision uses a versioned typed envelope, explicit Known/Absent/Unknown/Disputed/Stale evidence states, independently governed authority and auditable transition outcome. Grant approval and reservation are atomic; payment execution is independently reconciled; safety holds, authority changes and charity changes must be rechecked at the relevant authoritative commit/release boundary. No local exactly-once guarantee is misrepresented as external provider exactly-once execution. Quorum and protective intake operate with distinct acknowledgment semantics.

**Acceptance (not executed):** Test the 16 invariants and 12 concurrency schedules in `TYPED-DECISION-CONTRACTS-STATE-TRANSITIONS-AND-CONCURRENCY-2026-11-23.md`, including simultaneous reservations, charity adverse-event race, payment Unknown after crash, revoked recurring mandate, stale failback and emergency hold racing replay. Each transition has a falsifiable oracle and provenance trace. No test or legal/provider validation is claimed completed.

## REQ-X-020: Component-monolith architecture and safety-critical event ordering (2026-11-23)

**User-directed:** Unbound is a component monolith, **not microservices**. Domain components have explicit in-process boundaries and owned authoritative data, with one deployable application and one authoritative transactional financial/security core. Consequential commands use invariant-preserving atomic transactions, versioned evidence/policies, independently governed authority, durable outbox and idempotency. Internal components do not default to separate databases, independent deployments, service meshes or network calls. Managed external payment/evidence integrations, DR replication, independently durable nonauthoritative intake and minimal protective-only emergency plane are genuine distributed boundaries, not internal microservices.

**Ordering:** Global charity/hold publication and payment release must have a provable serialization point. Holds are binding on releases ordered after their durable commit. A preexisting release intent is not the same as irreversible external submission; dispatch rechecks holds where possible. Post-submission holds cannot guarantee reversal. Quorum loss stops authoritative financial writes; replay reconciles protective state before execution. Historical policy/evidence and exact external Unknown status remain auditable.

**Acceptance (not executed):** Test ADV-069..ADV-084 in `COMPONENT-MONOLITH-EVENT-ORDERING-AND-CONSISTENCY-2026-11-23.md`, including hold/release races, outbox loss, stale failback, AI stale snapshots, shared DB tenant leakage and concurrent reservation. No implementation or tests executed. Any future microservice split requires separately approved evidence-based ADR, not an assumed default.

## REQ-X-021: Component ownership, dependencies and atomic transaction contracts (2026-11-23)

**Proposed:** A single deployable component monolith has explicit in-process component ownership of authoritative aggregates, commands, persistence, migrations and invariants. Domain dependencies must be acyclic and pass through typed contracts; multi-component financial operations are coordinated by an application-command layer within one authoritative transaction. Direct cross-component table writes and unauthorized administrative mutation are prohibited. Outbox-driven provider delivery and noncritical projections are asynchronous; payment release and protective safety checks are authoritative synchronous gates. The independently durable protective-only emergency plane and outage intake are narrowly scoped resilience exceptions, not internal microservices.

**Acceptance (not executed):** The 17-component candidate ownership matrix, 14 dependency contracts, five transaction classes and ADV-085..ADV-096 in `COMPONENT-OWNERSHIP-DEPENDENCY-AND-TRANSACTION-MATRIX-2026-11-23.md` are mapped to architecture tests and concurrency invariants before implementation. Exact component boundaries remain candidate designs pending source traceability and validation.

## REQ-X-022: Authoritative data, ledger and transaction integrity (2026-11-23)

**Proposed:** In the component monolith, the Ledger component exclusively owns authoritative balances, reservations and journal entries. Grant authorization, reservation, payment intent, provider submission and settlement/finality remain distinct. Safety-critical cross-component operations use one authoritative invariant-preserving transaction, with durable outbox and independent reconciliation for nontransactional external payments. Financial journals are balanced, append-only and correction-linked; fund capacity is transactionally checked. Evidence and policies are versioned and bitemporally reconstructable. Sponsor-private data is isolated from global charity facts.

**Acceptance (not executed):** Validate the 20 candidate aggregates, 12 transaction boundaries and ADV-097..ADV-116 in `AUTHORITATIVE-DATA-AND-TRANSACTION-MODEL-2026-11-23.md`. Assert no partial approval/reservation, no unbalanced journal, no duplicate payment or fee, no false external finality, no stale release after hold, and complete as-known-at/as-effective-at audit reconstruction. Exact accounting rules, database isolation and provider proof remain open approval/evidence gates.

## REQ-X-023: DAF fund accounting, pooled AUM and reconciliation (2026-11-23)

**Proposed:** Distinguish sponsoring organization's legal-entity books, fund-level attribution and grant capacity, pooled investment units or segregated holdings, and independently verified custodian/payment evidence. Ledger and fund subledger are authoritative for internal financial state; journal, NAV, unit holdings, investment lifecycle, fee accrual, liquidity, FX, restrictions and historical statements require explicit provenance and independent reconciliation. Grant authorization, reservation, payable/expense recognition, disbursement and finality are not interchangeable. Fund unitization is optional by sponsor/vehicle, not universally imposed. Corrections preserve as-published and as-known-at history. No cross-sponsor commingling absent validated legal structure.

**Acceptance (not executed):** Validate ACC-01..ACC-12, seven reconciliation classes and ADV-117..ADV-140 in `DAF-FUND-ACCOUNTING-AUM-VALUATION-AND-RECONCILIATION-2026-11-23.md`. Sponsor CFO/CPA, legal and custody approvals are required before fixing journal recognition, valuation policy, NAV/unitization or financial database schema.

## REQ-X-024: Shared administrative notes on accounts, grants, charities and other entities (2026-11-23)

**Proposed:** Authorized administrators can create, view, revise, classify, search and link internal notes on accounts/funds, grants, charities, payments, contributions, sponsors and other supported subjects. One shared in-process Administrative Notes component owns typed subject references, note revisions, attachments and follow-up references. Internal notes are private by default, with strict sponsor/tenant and restricted-classification isolation; global charity facts cannot leak sponsor-private notes. Notes are distinct from verified evidence, policy, approvals, payment outcomes and ledger entries. All revisions are attributable and historically reconstructable. Record detail UIs include a contextual Notes section and note timeline, accessible on mobile.

**Acceptance (not executed):** ADV-141..ADV-152 and requirements in `ADMINISTRATIVE-NOTES-CROSS-ENTITY-REQUIREMENTS-2026-11-23.md`. A note stating 'approved' must not authorize a grant; private charity notes cannot cross sponsors; edits preserve prior revisions; sensitive exports require authorization. Notes can initiate independently authorized review/referral but never bypass domain safety gates.

## REQ-X-025: IVR telephone self-service and numeric record references (2026-11-23)

**Proposed:** All IVR-eligible account/fund, grant, charity and related records have stable digit-only DTMF-enterable alternate references distinct from immutable internal IDs and separately namespaced external provider IDs. Preserve leading zeros, uniqueness scope, non-reassignment, check-digit/error detection where justified, legacy aliases and sponsor white-label terminology. IVR integrates through the authenticated application facade within the component monolith; no direct ledger or note database access. Numeric references are lookup keys, not secrets. Caller authentication, authorization, privacy, neutral anti-enumeration responses, accessible DTMF fallback, precise authoritative statuses and secure support handoff are mandatory. Initial IVR capabilities are read-only pending separate approval for consequential commands.

**Acceptance (not executed):** ADV-153..ADV-168 and contracts in `IVR-TELEPHONE-SELF-SERVICE-NUMERIC-IDENTIFIERS-2026-11-23.md`. Validate namespace capacity and length, sponsor scoping, fraud resistance, accessibility, status accuracy and vendor security before selecting reference format or IVR provider.

## REQ-X-026: Grant recommender notes, shared messaging and private staff comments (2026-11-23)

**Proposed:** An authorized grant recommender can submit recommendation notes and continue a two-way grant conversation with sponsor administrators. Administrators can reply publicly to the authorized recommender audience, ask questions and request information, while separately creating staff-only and restricted internal notes. Shared messages are not internet-public. Thread audience is immutable; private notes cannot become shared by toggling visibility. Original recommendation notes and all revisions remain historically reconstructable. All reads, notifications, attachments, widgets, IVR and exports enforce sponsor and participant authorization. Messages never constitute grant approvals, payment evidence or compliance decisions.

**Acceptance (not executed):** ADV-169..ADV-184 and the workflows in `GRANT-RECOMMENDER-NOTES-SHARED-CONVERSATIONS-AND-PRIVATE-ADMIN-COMMENTS-2026-11-23.md`. Validate no private-note leakage, clear staff audience controls, authority revocation, cross-sponsor isolation, safe notifications and separate authoritative grant state.

## REQ-X-027: Client-overridable email templates and typed token replacement (2026-11-23)

**Proposed:** Every supported email event has a platform default template; authorized clients can override permitted subject, content, branding, locale and sender presentation through reviewed, versioned templates with deterministic fallback. A typed allowlisted token registry resolves authorized event-specific values, preserves numeric reference leading zeros, formats locale/currency/date correctly and escapes output. Mandatory legal/security clauses, recipient eligibility, transactional correctness and tenant privacy cannot be overridden. Templates have draft/review/publish/rollback, previews, synthetic test sends, history, plain-text alternatives, delivery audit and secure links. Communication triggers and external delivery use a durable outbox and do not change authoritative grant/payment states.

**Acceptance (not executed):** ADV-185..ADV-204 and contracts in `CLIENT-EMAIL-TEMPLATES-TOKEN-REPLACEMENT-AND-COMMUNICATION-GOVERNANCE-2026-11-23.md`. Test missing/unauthorized tokens, cross-tenant leakage, misleading payment statuses, client override safety, queued revision races, recipient revocation and provider delivery uncertainty.

## REQ-X-028: Immutable email template revision and as-sent communication history (2026-11-23)

**Proposed:** Platform defaults and client overrides retain complete immutable published revision history, including author, reviewer, diffs, token schema, mandatory clauses, approval, effective interval and inherited baseline. Administrators can compare, filter and preview prior versions; restoring a version creates a new reviewed revision rather than modifying history. Separately, every email delivery links the exact template revision, authorized historical token/render snapshot, recipient, trigger event, send attempts and provider status. Record pages provide permission-filtered communication history. Privacy, retention and legal redaction controls apply to sensitive rendered messages.

**Acceptance (not executed):** ADV-205..ADV-216 and `EMAIL-TEMPLATE-VERSION-AND-DELIVERY-HISTORY-2026-11-23.md`. Confirm immutable published revisions, accurate as-sent reconstruction, tenant isolation, governed rollback and honest provider delivery status.

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
> Adopted 2026-11-23: refined by `RR-REQ-X-002-r1` (adopted-provisional); the adopted requirement is in `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md` under 'Adopted reconciliation requirements'.

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
> Adopted 2026-11-23: refined by `RR-REQ-X-004-r1` (adopted-provisional); the adopted requirement is in `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md` under 'Adopted reconciliation requirements'.

**Acceptance scenario (not executed):** Given provider timeout and subsequent restore, when an operator retries without qualifying proof, then no second instruction is sent and original identity remains intact.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-005: Payee and external destination integrity

**Source questions:** OD-22, TVF-222, TVF-223, TVF-233, TVF-234, TVF-236, TVF-250, TVR-PAY-005-Q.

**Normative proposed requirement:** Every automated payment requires independently verifiable proof both that the destination is authorized and controlled by the intended charity and that the actual execution destination matches the sponsor-controlled authorization, including cryptographically or independently bound tokenized rails. Provider self-attestation alone is insufficient. Changes require independent authorization without self-approval. Missing or conflicting proof blocks automatic execution; noncompliant providers/rails are ineligible for fully automated payment. Reconcile authorization, execution and settlement evidence.
> Adopted 2026-11-23: refined by `RR-REQ-X-005-r1` (adopted-provisional); the adopted requirement is in `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md` under 'Adopted reconciliation requirements'.

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
> Adopted 2026-11-23: refined by `RR-REQ-X-007-r1` (adopted-provisional); the adopted requirement is in `requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md` under 'Adopted reconciliation requirements'.

**Acceptance scenario (not executed):** Given a recipient verified 30 days ago and sanctioned today, when payment releases, then the current adverse signal holds the grant despite valid periodic review.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-008: Donor death, succession and recurring plans

**Source questions:** OD-03, OD-11, TVC-030, TVC-033, TVC-044, TVR-GR-008-Q.

**Normative proposed requirement:** Donor death does not automatically cancel valid sponsor-approved unpaid grants; future instructions suspend until successor authority verified; each recurring instance rechecks authority and eligibility.
> Adopted 2026-11-23: refined by `RR-REQ-X-008-r1` (adopted-provisional); the adopted requirement is in `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md` under 'Adopted reconciliation requirements'.

**Acceptance scenario (not executed):** Given notice of donor death before next recurring instance, when scheduler executes, then new instance is held while prior valid approved grant follows its separate policy.

**Open dependencies:** Formal owner validation, evidence, test implementation and source migration.


### REQ-X-009: Reversible contribution and collection

**Source questions:** OD-04, TVC-010, TVR-CON-001-Q, TVR-CON-003-Q, TVF-006, EXT-06.

**Normative proposed requirement:** Rail-specific receipt, provisional availability, acknowledgment and finality remain distinct; returned funds trigger receivable, sponsor exposure and recovery classification without erasing disbursements.
> Adopted 2026-11-23: refined by `RR-REQ-X-009-r1` (adopted-provisional); the adopted requirement is in `requirements/FULL-ACCOUNTING-SYSTEM.md` under 'Adopted reconciliation requirements'.

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
> Adopted 2026-11-23: refined by `RR-REQ-X-020-r1` (adopted-provisional); the adopted requirement is in `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md` under 'Adopted reconciliation requirements'.

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

## REQ-X-029: Visual email designer, styling and reusable client brand themes (2026-11-23)

**Proposed:** Authorized clients can style emails through a constrained WYSIWYG/block editor and reusable, versioned brand kit (logos, colors, typography, spacing, header/footer, buttons), responsive layouts and per-event content. Template, layout and theme revisions are independently governed and pinned for as-sent historical reconstruction. Provide desktop/mobile/dark-mode/plain-text previews, accessibility and email-client compatibility validation, safe image hosting, typed tokens, approval workflow and optional restricted sanitized HTML mode. Mandatory clauses, tenant boundaries and verified sender domains cannot be bypassed by styling.
> Adopted 2026-11-23: superseded by `RR-REQ-X-029-r1` (adopted); the adopted requirement is in `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** ADV-217..ADV-232 and `VISUAL-EMAIL-DESIGNER-STYLING-BRAND-THEMES-AND-RENDERING-2026-11-23.md`. Verify no hidden disclosures, script injection, cross-tenant assets, sensitive-data image URL leakage, unreadable mobile/dark-mode output or retroactive historical rendering changes.

## REQ-X-030: Unified client Brand Kit and omnichannel white labeling (2026-11-23)

**Proposed:** Each sponsor has one governed, versioned Brand Kit for identity, logos, accessible design tokens, typography, semantic terminology, approved assets, support contacts and channel policies. Donor portals, administrative interfaces, embedded widgets, email, print/PDF, IVR and SMS consume typed authorized brand snapshots with channel-specific renderers. Platform defaults, sponsor overrides and optional governed variants resolve deterministically. Brand publication includes previews, accessibility checks, approval, history and as-issued version binding. Client-facing labels may differ from canonical business terminology, but cannot alter financial, compliance, payment or authorization semantics. Sponsor asset/domain isolation and mandatory disclosures remain nonwaivable.

**Acceptance (not executed):** ADV-233..ADV-252 and `UNIFIED-CLIENT-BRAND-KIT-OMNICHANNEL-WHITE-LABEL-REQUIREMENTS-2026-11-23.md`. Verify tenant isolation, cross-channel consistency, accessibility, safe fallback, immutable historical rendering and protected legal/financial meaning.

## REQ-X-031: Reusable Forma email designer and Unbound integration (2026-11-23)

**Proposed:** Forma owns the structured, versioned email document model, visual editing primitives and validation interfaces; Forma Studio owns the visual authoring workspace. A separately validated email renderer produces safe compatible HTML and meaningful plain text. Unbound owns sponsor/client templates, token resolution, branding, mandatory disclosures, approval, delivery and as-sent history. The designer cannot directly access donor data or independently publish/send messages. Requirements include mobile/keyboard accessible authoring, email-client compatibility, schema migration, historical reproducibility and adversarial security tests.

**Acceptance (not executed):** Follow the 30 ED-001..030 scenarios, 10 acceptance gates and 15 ADR topics in `kemiller2002/forma` and `kemiller2002/forma-studio` at `docs/requirements/email-designer-2026-11-23.md`, plus `FORMA-EMAIL-DESIGNER-UNBOUND-INTEGRATION-2026-11-23.md`. No implementation or tests authorized or completed.

## REQ-X-032: Monthly fund statement generation, publication and distribution (2026-11-23)

**Proposed:** Unbound generates monthly statements by default for eligible funds, from approved reconciled period-end financial snapshots, with sponsor-configurable schedule, cutoff, disclosures, branding and delivery policy. Statements have distinct generation, approval, publication, availability and delivery states. Support fund-level and authorized consolidated views, secure portal access, email notification, optional physical mail, accessible print/PDF, immutable as-issued history, governed corrections, month-end exception handling, idempotent batch scheduling and full audit. Statement data never derives from unverified portal caches or AI; account NAV and available-to-grant remain distinct.

**Acceptance (not executed):** ADV-253..ADV-272 and `MONTHLY-FUND-STATEMENTS-GENERATION-AND-DISTRIBUTION-2026-11-23.md`. Validate reconciliation, privacy, period cutoffs, batch retries, sponsor template overrides, delivery uncertainty, historical reconstruction and accessible formats. CFO/CPA and counsel must approve accounting presentation and legal disclosures before production.

## REQ-X-033: Typst statement rendering, immutable PDF artifact registry and cryptographic verification (2026-11-23)

**Proposed:** Use Typst as the preferred statement PDF rendering candidate behind a validated Folio-compatible adapter. Archive exact issued PDF bytes with immutable StatementRevisionId, accounting/valuation/template/brand/renderer provenance, canonical evidence-manifest SHA-256, final PDF-byte SHA-256, approvals, access controls and retention. Embed statement ID and short verification reference or QR in the document; calculate final PDF hash only after rendering to avoid circular self-hashing. Support authenticated verification, original download, integrity checks, corrected revisions and signed manifests if independently justified. Do not regenerate a historic PDF as though it were the issued original.

**Acceptance (not executed):** ADV-273..ADV-292 and `TYPST-STATEMENT-ARTIFACT-REGISTRY-AND-CRYPTOGRAPHIC-VERIFICATION-2026-11-23.md`. Validate Typst/Folio fit, PDF accessibility, deterministic provenance, immutable storage and byte verification before implementation.

## REQ-X-034: Shared immutable Document Artifact Registry (2026-11-23)

**Proposed:** Generalize the Statement Artifact Registry into one reusable in-process document issuance and verification capability covering monthly/annual statements, contribution/tax acknowledgments, grant correspondence, fee notices and other formally issued records. Business modules retain authority over content and approvals; registry owns immutable issued bytes, versioned evidence manifests, final byte hashes, controlled retrieval, correction chains, delivery references, legal holds and retention. Typst via validated Folio-compatible adapter remains preferred PDF candidate. Support authenticated verification, accessible format variants, per-type governance, cross-sponsor isolation and recoverable object-store/DB issuance. Do not mistake a valid hash for financial or legal correctness.

**Acceptance (not executed):** ADV-293..ADV-314 and `SHARED-DOCUMENT-ARTIFACT-REGISTRY-ISSUANCE-VERIFICATION-AND-RETENTION-2026-11-23.md`. Prove exact-byte retrieval, authorization, correction preservation, crash recovery, integrity checks, accessibility and retention before production.

## REQ-X-035: Permission-aware Document Center and verification UI (2026-11-23)

**Proposed:** Provide donor/advisor/successor/sponsor-admin Document Center with fund-level and cross-fund search, period/type filters, exact as-issued PDF viewing/downloading, accessible alternatives, revision/correction history, cryptographic integrity verification and communication/delivery status. Scope metadata, search, previews, counts, downloads and embedded access by current and effective-dated authority. Admin actions for correction, reissue, retention and bulk export require distinct permissions and owning-domain workflows. Monthly statements appear automatically after authorized publication and trigger governed notification. No silent document regeneration or claims that a hash proves financial correctness.

**Acceptance (not executed):** ADV-315..ADV-334 and `DOCUMENT-CENTER-PORTAL-ADMIN-SEARCH-ACCESS-AND-VERIFICATION-2026-11-23.md`. Validate access revocation, cross-tenant isolation, byte integrity checks, corrected document lineage, accessible UI and accurate delivery state.

## REQ-X-036: Seven-year financial document retention minimum and legal holds (2026-11-23)

**Owner-directed policy:** Retain all financial documents and substantiating financial evidence for **at least seven years**, with longer applicable legal, regulatory, contractual, sponsor or litigation-hold requirements taking precedence. This is a nonwaivable platform floor, not a legal conclusion that all records can be deleted after seven years. Retention clocks must be approved per record class; fund closure, donor death, advisor change and sponsor offboarding do not shorten retention. Originals and corrections remain immutable and linked; legal holds apply to existing and future matching evidence. Expiry triggers governed disposition review, never automatic deletion.
> Adopted 2026-11-23: refined by `RR-REQ-X-036-r1` (adopted-provisional); the adopted requirement is in `requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** ADV-335..ADV-354 and `FINANCIAL-DOCUMENT-SEVEN-YEAR-RETENTION-LEGAL-HOLDS-AND-LIFECYCLE-2026-11-23.md`. Validate storage retention, backup/restore, legal hold propagation, access revocation, archived retrieval, disposition controls and sponsor offboarding. Counsel/compliance validation required for record-class triggers and jurisdictional minima.

## REQ-X-037: Enterprise document classification and retention matrix (2026-11-23)

**Proposed:** Govern records using a versioned cross-system classification matrix spanning financial documents, governance, legal, compliance, operational/marketing communications, notes, IVR, security, templates, provenance, vendors, drafts and backups. Apply the **nonwaivable seven-year financial document/evidence minimum** to all financially material records regardless of channel or file type, with longer applicable law, contract, sponsor policy and legal holds prevailing. Multiple classifications use the longest obligation. Nonfinancial periods remain TBD pending legal/compliance review, not arbitrary invented defaults. Classify automatically when safe; AI may recommend but not authorize shorter retention. Expiration triggers reviewed disposition, not automatic deletion. Fund closure, sponsor offboarding and access revocation do not erase retained evidence.

**Acceptance (not executed):** ADV-355..ADV-378 and `DOCUMENT-CLASSIFICATION-RETENTION-MATRIX-AND-DISPOSITION-GOVERNANCE-2026-11-23.md`. Verify class overlap, durable financial classification, legal holds, privacy conflicts, retention clock policy, archive integrity, backup coverage and approved disposition.

## REQ-X-038: Deterministic retention-policy engine and governed disposition (2026-11-23)

**Proposed:** Enforce record preservation through one versioned, typed in-process Retention Policy component within the component monolith. Financial records and evidence have a nonwaivable seven-year minimum from an approved class-specific trigger; longer legal, sponsor, contractual, dependency and legal-hold obligations prevail. Unknown or stale safety facts block deletion. Preservation, access and disposition are distinct decisions. Expiration means EligibleForReview, not automatic purge. Hold matching covers existing/future linked records and serializes with disposition; independently approved disposition rechecks authoritative policy, hold and dependency versions immediately before a fenced, auditable storage action. AI may suggest classification, never shorten retention or authorize deletion. Expose reasons, provenance, archive integrity and legal/compliance controls.

**Acceptance (not executed):** ADV-379..ADV-406 and `DETERMINISTIC-RETENTION-POLICY-ENGINE-HOLDS-AND-DISPOSITION-2026-11-23.md`. Test calendar clocks, multi-class precedence, unknown facts, hold/deletion races, policy migrations, object-store failures, backup and privacy conflicts. Counsel approval and technical ADRs required.

## REQ-X-039: Financial document reconciliation, issuance provenance and artifact assurance (2026-11-23)

**Proposed:** Establish three independent assurance dimensions for issued financial documents: (1) authoritative accounting close and fund-subledger reconciliation; (2) versioned as-issued provenance for financial inputs, valuation, policy, approvals, templates and renderer; and (3) exact archived-byte SHA-256 integrity with optional independently validated signatures. Preserve bitemporal as-issued/as-known-at/current-corrected evidence and linked correction revisions. Publication gates reject material reconciliation exceptions, missing evidence, stale valuations and unknown critical statuses. Authorized audit exports include source snapshot, calculation proofs, issuance attestations, original PDF and manifest, with explicit unknowns. The seven-year financial retention floor and legal holds cover supporting evidence as well as PDFs. A valid hash never implies financial correctness.

**Acceptance (not executed):** ADV-407..ADV-434 and `FINANCIAL-DOCUMENT-AUDIT-RECONCILIATION-PROVENANCE-AND-VERIFICATION-2026-11-23.md`. Validate financial tie-out, late corrections, bitemporal reconstruction, original byte preservation, audit bundle integrity, tenant isolation and publication gates; sponsor finance/CPA review required.

## REQ-X-040: Exception-driven month-end close and automated monthly statement publication (2026-11-23)

**Proposed:** Provide sponsor-specific monthly close calendars and a deterministic, versioned close state machine. Reconcile authoritative GL, fund subledger, custody, NAV, fees, contributions, grants, payments and pending items with explicit Pass/Fail/Unknown evidence. Freeze approved close snapshots with effective-date/known-at semantics. Escalate material, stale, missing or uncertain evidence and mandated finance signoffs, while automatically validating and issuing routine statements under approved sponsor delegation. Render pinned Typst/Folio PDFs, retain immutable bytes and hashes with seven-year financial evidence floor, publish to Document Center and send idempotent notifications. Separate accounting close, statement approval, issuance and delivery states. Preserve original and corrected close/document revisions; recover safely from partial failures.

**Acceptance (not executed):** ADV-435..ADV-470 and `MONTH-END-CLOSE-RECONCILIATION-AND-AUTOMATED-STATEMENT-PUBLICATION-2026-11-23.md`. Validate finance authority, cross-sponsor isolation, reconciliation evidence, late transactions, partial batch rules, storage crash recovery and duplicate prevention. CPA/sponsor governance review required.

## REQ-X-041: Shared financial operations exception management and safe automated resolution (2026-11-23)

**Proposed:** Implement a shared, sponsor-isolated in-process Exception Management component across accounting, payments, grants, charity vetting, compliance, documents, communications and retention. Classify and deduplicate issues by authoritative subject, condition and evidence; use deterministic evidence acquisition and allowlisted safe resolution first, bounded AI for cited investigation proposals second, and independent authorized human judgment only where necessary. Keep protective holds, financial approvals, legal holds and provider Unknown states under their owning domain authority; closing an exception case never automatically clears them. Support root-cause grouping, evidence-based resolution verification, risk-based escalation, accessible role-specific command center, full audit and seven-year minimum for financial evidence.

**Acceptance (not executed):** ADV-471..ADV-506 and `FINANCIAL-OPERATIONS-EXCEPTION-MANAGEMENT-AND-AUTOMATED-RESOLUTION-2026-11-23.md`. Validate concurrency, source-of-truth gates, external Unknown handling, sponsor isolation, idempotency, independent financial approvals, retention and AI authority limits.

## REQ-X-085: Shared financial operations exception management and safe automated resolution (2026-11-23) (formerly the 2nd definition of REQ-X-041; renumbered 2026-11-23)

**Proposed:** Provide a unified, sponsor-scoped exception management component for accounting, payments, grants, charity vetting, compliance, document issuance and communications. Distinguish detected operational cases from authoritative financial or legal controls. Resolve safe routine cases deterministically with verified evidence, use bounded AI for investigation and recommendation, and require independent authorized human judgment for material financial, legal and policy decisions. Preserve Unknown external payment states, protective holds, tenant isolation, case provenance, and the seven-year floor for financially material evidence. Case closure never directly releases money, legal holds or accounting gates. Provide an accessible Exception Command Center, deduplication, root-cause grouping, risk-based SLAs and metrics.

**Acceptance (not executed):** ADV-471..ADV-506 and `FINANCIAL-OPERATIONS-EXCEPTION-MANAGEMENT-AND-AUTOMATED-RESOLUTION-2026-11-23.md`. Validate concurrency, authoritative controls, AI boundaries, provider Unknown, tenant isolation, duplicate suppression, financial retention and verified case closure.

## REQ-X-042: Operational learning and upstream exception prevention (2026-11-23)

**Proposed:** Analyze verified recurring exceptions and near misses across sponsors to identify causal failure mechanisms and propose upstream workflow redesign. Track denominators, detection coverage, severity-weighted harm, counterhypotheses and measured outcomes so lower ticket counts cannot masquerade as risk reduction. Allow only narrowly preauthorized reversible low-risk improvements to auto-deploy under test/rollback gates; financial, compliance, money-movement, privacy, retention, legal and authorization changes require independent governance. AI can investigate and propose but cannot self-modify critical policy. Use synthetic replay, adversarial challenge, controlled experiments or shadow mode, with tenant privacy, accessible UI and durable learning provenance.

**Acceptance (not executed):** ADV-507..ADV-542 and `OPERATIONAL-LEARNING-ROOT-CAUSE-AND-UPSTREAM-EXCEPTION-PREVENTION-2026-11-23.md`. Validate causality claims, detection coverage, approval boundaries, regression guardrails, cross-sponsor isolation, rollback and preserved financial controls.

## REQ-X-043: Enterprise operational intelligence and governed decision metrics (2026-11-23)

**Proposed:** Establish a shared decision-oriented operational intelligence layer with versioned metric definitions, authoritative source lineage, sponsor isolation, reconciliation/freshness/coverage disclosure and safe drilldowns. Cover financial integrity/AUM, grants and payments, donor experience, charity/compliance, unit economics, documents and operational prevention. Distinguish approved financial close figures from live estimates; never conflate Approved with Paid, AUM with available-to-grant, or AI-assisted with verified automation. Dashboard actions invoke authoritative domain commands and cannot bypass financial, legal or security controls. Preserve bitemporal report provenance and privacy-safe aggregated benchmarks. Treat role-based dashboard UX, embedded widgets, formal report catalog/designer and scheduling as explicitly deferred dedicated requirements phase.

**Acceptance (not executed):** ADV-543..ADV-578 and `ENTERPRISE-OPERATIONAL-INTELLIGENCE-METRICS-AND-DECISION-ARCHITECTURE-2026-11-23.md`. Validate metric definitions, denominators, financial reconciliation, freshness, sponsor isolation, official versus estimated reporting, privacy and action gates.

## REQ-X-044: Enterprise decision rights and delegated authority (2026-11-23)

**Proposed:** Versioned sponsor/platform authority, delegation scope/expiry, independent approvals, no AI or emergency money-moving override, execution-time recheck.

**Acceptance (not executed):** Relevant ADV-579..ADV-618; `ENTERPRISE-DECISION-RIGHTS-AND-DELEGATED-AUTHORITY-2026-11-23.md`. Cross-package challenge and prioritized dashboard/report handoff: `ENTERPRISE-GOVERNANCE-INTEGRATION-CONSOLIDATION-AND-DASHBOARD-REPORT-ROADMAP-2026-11-23.md`.

## REQ-X-045: Sponsor configuration and policy lifecycle (2026-11-23)

**Proposed:** Declarative validated sponsor configuration, legal-safe white labeling, versioned policy simulation/approval/activation, no bypass of financial seven-year floor or protective holds.

**Acceptance (not executed):** Relevant ADV-579..ADV-618; `SPONSOR-CONFIGURATION-POLICY-LIFECYCLE-AND-ISOLATION-2026-11-23.md`. Cross-package challenge and prioritized dashboard/report handoff: `ENTERPRISE-GOVERNANCE-INTEGRATION-CONSOLIDATION-AND-DASHBOARD-REPORT-ROADMAP-2026-11-23.md`.

## REQ-X-046: Provider data contracts and integration evidence (2026-11-23)

**Proposed:** Typed adapters, provenance, effective/known-at dates, provider Unknown, reconciliation, idempotency, tenant security and comprehensive mock interactions.

**Acceptance (not executed):** Relevant ADV-579..ADV-618; `ENTERPRISE-DATA-CONTRACTS-PROVIDER-INTEGRATION-AND-EVIDENCE-GOVERNANCE-2026-11-23.md`. Cross-package challenge and prioritized dashboard/report handoff: `ENTERPRISE-GOVERNANCE-INTEGRATION-CONSOLIDATION-AND-DASHBOARD-REPORT-ROADMAP-2026-11-23.md`.

## REQ-X-047: Operational resilience and protective continuity (2026-11-23)

**Proposed:** Fenced authoritative financial writer, explicit fault-assumption RPO/RTO, protective-only emergency plane, safe outage queue, recoverable archive and provider-status reconciliation.

**Acceptance (not executed):** Relevant ADV-579..ADV-618; `ENTERPRISE-RESILIENCE-CONTINUITY-AND-PROTECTIVE-OPERATIONS-2026-11-23.md`. Cross-package challenge and prioritized dashboard/report handoff: `ENTERPRISE-GOVERNANCE-INTEGRATION-CONSOLIDATION-AND-DASHBOARD-REPORT-ROADMAP-2026-11-23.md`.

## REQ-X-048: Human work queues, round-robin routing and cradle-to-grave ownership (2026-11-23)

**Proposed:** Provide shared sponsor-scoped human work orchestration for grant and charity vetting cases. Support deterministic round robin, sticky sponsor/fund/account ownership, cradle-to-grave case continuity, specialist/skills routing, capacity-weighted assignment, manual audited overrides and hybrid routing. Distinguish relationship owner, case owner, task assignee, independent decision authority and backup. Preserve eligibility, sponsor authorization, conflicts, segregation of duties, absences, SLA escalation, assignment audit and seven-year retention of financially material work evidence. Case completion never independently approves grants, clears charity/sanctions holds or releases payment. Provide My Work, My Accounts, Team Queue and Supervisor dashboard requirements; routine safe automated cases should not create unnecessary human tasks.
> Adopted 2026-11-23: refined by `RR-REQ-X-048-r1` (adopted); the adopted requirement is in `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** ADV-619..ADV-654 and `HUMAN-WORK-QUEUES-ROUTING-OWNERSHIP-AND-CASE-CONTINUITY-2026-11-23.md`. Validate concurrent fair assignment, sticky ownership, specialist handoffs, authorization, absence coverage, shared charity vetting privacy and immutable history.

## REQ-X-049: Work Queue Management dashboards, role-based UX and reporting (2026-11-23)

**Proposed:** Make Work Queue Management a first-class Unbound dashboard and reporting area for individual reviewers, cradle-to-grave case owners, supervisors, sponsor admins, compliance/finance approvers and platform operations. Include My Work, My Accounts, Team Queue, Supervisor Command Center, Case Workspace, Routing Configuration and Performance/Learning views; authorized claim/reassignment, evidence, ownership continuity, specialist handoffs, capacity/skills, SLA and risk. Keep queue projections separate from authoritative grant/charity/payment states, and distinguish assignment from approval. Provide tenant-scoped accessible responsive layouts, configurable views, safe actions and versioned report catalog with provenance and retention.

**Acceptance (not executed):** ADV-655..ADV-686 and `WORK-QUEUE-MANAGEMENT-DASHBOARDS-ROLE-BASED-UX-AND-REPORTING-2026-11-23.md`. Validate authorization, stale queue actions, case continuity, privacy, accessibility, metric definitions and audit.

## REQ-X-050: Shared dashboard framework and Forma/Folio capability validation (2026-11-23)

**Proposed:** Establish role-based, configurable, sponsor-scoped dashboards using pinned public Forma components and Forma Studio design authoring, with Unbound-owned semantic metrics, versioned widget/layout definitions, safe filters, secure embedding, source freshness and authoritative actions. Reuse Folio's public print primitives for governed financial/operational reports, but validate renderer tiers, multipage financial fidelity, accessible/archival PDF, determinism, immutable as-issued artifact integrity and Typst interoperability before committing to production rendering. Track gaps in owning Forma, Forma Studio and Folio repositories rather than duplicate shared presentation systems. Confirm commercial license suitability before deployment.

**Acceptance (not executed):** ADV-687..ADV-722 and `SHARED-DASHBOARD-FRAMEWORK-FORMA-FOLIO-CAPABILITY-ASSESSMENT-2026-11-23.md`. Require version-pinned consumer fixtures, responsive/accessibility checks, secure widget tests, rendering evidence, archival/provenance verification and documented unsupported capabilities.

## REQ-X-051: AI-discoverable API, input and configuration assistance contracts (2026-11-23)

**Proposed:** Every public API operation, input field, configuration property, workflow action, dashboard widget and email block/token MUST expose authoritative, versioned, machine-readable AI assistance metadata covering purpose, types, dependencies, valid values, policy, permissions, side effects, errors, Unknown states, examples, source provenance and safe guidance. Generate contextual help and agent tool descriptions from typed contracts with reviewed semantic annotations, test for drift, and enforce caller-scoped discovery. AI can explain, guide and propose diffs, but cannot grant itself authority or bypass sponsor rules, approvals, holds or domain commands.
> Adopted 2026-11-23: refined by `RR-REQ-X-051-r1` (adopted); the adopted requirement is in `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** ADV-723..ADV-752 and `AI-DISCOVERABLE-CONTRACTS-AND-SAFE-ASSISTANCE-2026-11-23.md`.

## REQ-X-052: Typed email template expression language and AI-assisted authoring (2026-11-23)

**Proposed:** Extend the existing Forma email document/block model with a versioned, declarative, typed content expression AST supporting event-scoped tokens, formatters, constrained conditionals, bounded repetition, approved fragments and localization. Offer optional readable syntax and visual Studio authoring, with static type checking, contextual encoding, deterministic HTML/plain-text/subject output, synthetic previews, AI explanation and reviewed proposed diffs. Never allow arbitrary code, donor-data queries, unauthorized token access, removal of mandatory disclosures or AI publishing. Unbound owns event data, sponsor policy, publication and delivery; Forma owns reusable document contracts; Studio owns editing UX.

**Acceptance (not executed):** ADV-753..ADV-782 and `TYPED-EMAIL-TEMPLATE-LANGUAGE-AND-AI-AUTHORING-ASSISTANCE-2026-11-23.md`.

## REQ-X-053: Platform-wide AI assistance, contextual guidance and governed action proposals (2026-11-23)

**Proposed:** Provide AI assistance across all Unbound APIs, inputs, configuration, grants, charity vetting, work queues, dashboards, reports, email templates and operational support using the versioned AI-discoverable contracts in REQ-X-051. Use one in-process Assistance Orchestrator for caller-scoped capability discovery, context retrieval, explanations, guided workflows, validated draft proposals, impact previews and narrowly gated domain commands. Separate verified facts, operational observations, inferences and recommendations; label Unknown and stale evidence. Enforce sponsor privacy, authorization, independent approvals, financial/compliance holds, auditable confirmation and human escalation. Proactive checks should propose safe remediation rather than silently change policies or money movement. Reuse Forma, Forma Studio and Folio capabilities without duplicating domain authority.

**Acceptance (not executed):** ADV-783..ADV-842 and `PLATFORM-WIDE-AI-ASSISTANCE-ARCHITECTURE-CONTEXTUAL-GUIDANCE-AND-GOVERNED-ACTIONS-2026-11-23.md`. Validate source/contract versioning, cross-sponsor security, safe proposals, stale/Unknown status handling, AI prompt-injection resistance, accessible contextual UX, regression evaluation and execution-time authorization.

## REQ-X-054: Reactive contextual AI assistance and guided configuration (2026-11-23)

**Proposed:** Provide caller-scoped contextual AI help on every documented input, API, action, configuration and relevant case/report/dashboard/email surface. Support explain, guide, analyze, validated typed draft, impact preview, safe correction, source attribution, uncertainty, accessibility and human escalation. Preserve ordinary direct UI controls. AI cannot self-publish or bypass domain permissions, holds, approval or segregation of duties.

**Acceptance (not executed):** ADV-843..ADV-866 and `REACTIVE-AI-ASSISTANCE-CONTEXTUAL-UX-AND-GUIDED-CONFIGURATION-2026-11-23.md`.

## REQ-X-055: Proactive AI assistance, deterministic detection and preventive operations (2026-11-23)

**Proposed:** Detect known risks through versioned deterministic checks, optionally use bounded AI for explanation/investigation/proposals, and route findings into existing Exception Management and Work Queue components. Cover charity reverification, grant routing, sponsor policy, recurring activity, accounting, payments, retention, integrations and UI/report quality. Require deduplication, risk tiers, evidence, ownership, SLA, false-positive review, expiry-bound suppression and independent resolution verification. Do not create a competing alert/case engine or permit AI independent money/compliance authority.

**Acceptance (not executed):** ADV-867..ADV-890 and `PROACTIVE-AI-ASSISTANCE-DETERMINISTIC-DETECTION-AND-PREVENTIVE-OPERATIONS-2026-11-23.md`.

## REQ-X-086: Contextual AI assistance UX, configuration previews and approval journeys (2026-11-23) (formerly the 2nd definition of REQ-X-054; renumbered 2026-11-23)

**Proposed:** Integrate AI help into every eligible field, configuration panel and workflow without requiring a separate chatbot. Provide source-grounded explanations, valid-value guidance, typed proposed changes, deterministic validation, impact preview, human review, independent approval and execution-time reauthorization. Preserve sponsor scope, case ownership, financial/compliance holds, mobile accessibility and honest Draft/Approved/Applied/Unknown status. No AI-only bypass of domain commands.

**Acceptance (not executed):** ADV-843..ADV-866 and `CONTEXTUAL-AI-ASSISTANT-UX-CONFIGURATION-AND-APPROVAL-JOURNEYS-2026-11-23.md`.

## REQ-X-087: Proactive configuration health, prevention and safe remediation (2026-11-23) (formerly the 2nd definition of REQ-X-055; renumbered 2026-11-23)

**Proposed:** Use deterministic event-driven checks and bounded AI explanation/investigation to identify configuration contradictions, upcoming charity 180-day reverification, staffing gaps, provider uncertainty, finance/report mismatches and document/communication risks. Findings must be evidenced, classified, deduplicated, sponsor-scoped, assigned to an accountable owner and linked to smallest safe remediation. Existing domain controls retain holds and approvals. Prevent alert fatigue, prohibit silent policy changes and measure actual prevented harm rather than warning counts.

**Acceptance (not executed):** ADV-867..ADV-890 and `PROACTIVE-AI-CONFIGURATION-HEALTH-AND-PREVENTION-2026-11-23.md`.

## REQ-X-088: Reactive and proactive AI assistance, health checks and guided configuration UX (2026-11-23) (formerly the 3rd definition of REQ-X-054; renumbered 2026-11-23)

**Proposed:** Provide one coordinated in-process assistance architecture for user-initiated contextual guidance and event-driven proactive health checks. Deterministic domain validators detect known violations before bounded AI investigation of ambiguity. Use typed evidence-based HealthIssues and RemediationProposals, issue deduplication, risk/owner/expiry, sponsor-scoped context, accessible configuration help, preview/diff/simulation, independent approvals, existing work queues, post-action verification and outcome-based measurement. Do not silently change financial/compliance policies, money movement, holds or protected disclosures; AI remains advisory, with narrowly permitted domain-command execution only after independent authorization.

**Acceptance (not executed):** ADV-843..ADV-903 and `REACTIVE-PROACTIVE-AI-ASSISTANCE-HEALTH-CHECKS-AND-INTERACTION-DESIGN-2026-11-23.md`. Test deduplication, false-positive/false-negative coverage, source freshness, prompt injection, role revocation, accessibility, model outage, proposal safety, tenant isolation and verified outcomes.

## REQ-X-089: Contextual AI assistance UX and notification governance (2026-11-23) (formerly the 3rd definition of REQ-X-055; renumbered 2026-11-23)

**Proposed:** Establish a reusable contextual assistance interaction system, using public Forma patterns and Forma Studio authoring, for inline help, evidence-backed explanations, permission-aware guided configuration, structured proposal diffs, simulation previews, authorized confirmations, human escalation and proactive Health Center notifications. Keep source/version/freshness and fact-versus-inference distinctions visible; preserve mandatory financial/compliance holds; deduplicate notifications and measure real-world harm reduction rather than engagement. Enforce accessible mobile/keyboard/screen-reader UX and explicit loading, Unknown, stale, degraded and unauthorized states. Unbound/Limen retains context, business semantics and action authority; Forma only presentation.

**Acceptance (not executed):** ADV-904..ADV-963 and `CONTEXTUAL-AI-ASSISTANCE-UX-FORMA-PATTERNS-AND-NOTIFICATION-GOVERNANCE-2026-11-23.md`. Require realistic role-based prototypes, source-aware proposal review, no hidden holds, no unauthorized evidence leakage, model-outage fallback, controlled notifications and usability validation.

## REQ-X-056: DAF landscape bidirectional capability reconciliation and workflow parity gates (2026-11-23)

**Proposed:** Maintain an evidence-classified, buyer- and operating-model-aware mapping between DAF landscape vendor/sponsor capabilities and Unbound requirements. Every material competitive capability must map to a requirement, explicit Build/Partner/Defer/Exclude decision, accountable owner, evidence provenance, lifecycle workflow and theoretical acceptance scenario. Every Unbound requirement should identify its user outcome and competitive or regulatory rationale. Do not equate public vendor marketing with tested capability or Unbound requirements with implemented parity. Prioritize the 20 initial gaps (GAP-001..020) and ten end-to-end scenarios (SCN-01..10), including nonprofit operations, complex contributions, sponsor reporting, migration, advisor integration, external DAF checkout and legal operating models. Preserve source dates, legal entity distinctions and unknown states.

**Acceptance (not executed):** `DAF-LANDSCAPE-TO-UNBOUND-CAPABILITY-COVERAGE-AUDIT-2026-11-23.md` and `DAF-LANDSCAPE-SECOND-PASS-RECONCILIATION-GAP-REGISTER-AND-WORKFLOW-GATES-2026-11-23.md`. This is a second-pass mapping, not a completed exhaustive file inventory or verified vendor feature parity.

## REQ-X-057: One-system architecture across all DAF operating models (2026-11-23)

**Owner decision:** Unbound MUST remain one component monolith with one authoritative fund accounting/ledger core, sponsor-scoped policy and delegated authority, shared work orchestration, typed provider integration/evidence, semantic reporting, document/communication lifecycle, AI assistance contracts and reusable UI design system. Institutional sponsor SaaS, outsourced administration, turnkey sponsor partnership, digital donor channels, advisor programs, nonprofit-facing experiences and embedded giving MUST reuse these core domain commands and records. Differences are expressed through declarative configuration, legal/contractual role boundaries, permitted adapters and persona-specific views, not cloned products, separate financial state machines or microservices. Preserve legal sponsor asset separation and tenant isolation; the previously specified protective-only DR exception cannot move funds.
> Adopted 2026-11-23: refined by `RR-REQ-X-057-r1` (adopted-provisional); the adopted requirement is in `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** `ONE-SYSTEM-MULTI-OPERATING-MODEL-ARCHITECTURAL-INVARIANT-2026-11-23.md`; trace SCN-01..SCN-10 to single domain owners and test sponsor isolation, unified financial truth and shared authorization.

## DAF landscape P0 operating capability requirements (2026-11-23)

### REQ-X-058: Nonprofit recipient operations and self-service

**Proposed:** Specify the full operating workflow in `NONPROFIT-RECIPIENT-OPERATIONS-AND-SELF-SERVICE-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.

### REQ-X-059: Complex contributions, gift acceptance and liquidation

**Proposed:** Specify the full operating workflow in `COMPLEX-CONTRIBUTIONS-GIFT-ACCEPTANCE-AND-LIQUIDATION-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.

### REQ-X-060: Sponsor regulatory, tax and audit operations

**Proposed:** Specify the full operating workflow in `SPONSOR-REGULATORY-TAX-AND-AUDIT-OPERATIONS-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.
> Adopted 2026-11-23: refined by `RR-REQ-X-060-r1` (adopted-provisional); the adopted requirement is in `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md` under 'Adopted reconciliation requirements'.

### REQ-X-061: Legacy migration, portability and sponsor exit

**Proposed:** Specify the full operating workflow in `LEGACY-MIGRATION-PORTABILITY-AND-SPONSOR-EXIT-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.
> Adopted 2026-11-23: refined by `RR-REQ-X-061-r1` (adopted); the adopted requirement is in `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md` under 'Adopted reconciliation requirements'.

### REQ-X-062: Advisor, wealth manager, custody and CRM integration

**Proposed:** Specify the full operating workflow in `ADVISOR-WEALTH-MANAGER-CUSTODY-AND-CRM-INTEGRATION-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.

### REQ-X-063: External DAF checkout and embedded grant initiation

**Proposed:** Specify the full operating workflow in `EXTERNAL-DAF-CHECKOUT-EMBEDDED-GRANT-INITIATION-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.

### REQ-X-064: Legal operating models, sponsor contracts and responsibility matrix

**Proposed:** Specify the full operating workflow in `LEGAL-OPERATING-MODELS-SPONSOR-CONTRACTS-AND-RESPONSIBILITY-MATRIX-2026-11-23.md`. Reuse REQ-X-057 one-system architecture and the same authoritative ledger, policy, identity, case, evidence, reporting and communication components. Validate role-specific UI, domain commands, provider mock fixtures, sponsor isolation, exceptions, legal authority, audit and end-to-end financial effects. This is theoretical; no software, tests or professional legal/accounting approval.

## DAF landscape remaining 13 capability requirements and reconciliation (2026-11-23)

### REQ-X-065: International grants and cross-border controls

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-008. See the dedicated REQ-X-065 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-066: Donor tax-fit and charitable opportunity guidance

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-009. See the dedicated REQ-X-066 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-067: Partner economics, fee waterfalls and conflicts

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-010. See the dedicated REQ-X-067 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-068: Family governance, collaborative giving and succession

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-011. See the dedicated REQ-X-068 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-069: Nonprofit impact, restrictions and grant outcomes
  - *Refined 2026-11-24 by RR-REQ-X-069-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-NPG-006, decided-provisional):* Grant-outcome and restriction monitoring MUST be sponsor-configurable and OFF by default for grants to §170(b)(1)(A) organizations (other than disqualified supporting organizations), because such distributions are excepted from §4966 taxable distributions whatever their stated purpose (XID-NPG-006). Monitoring MUST be mandatory only for expenditure-responsibility grants (reports per §53.4945-5) and for grants the sponsor itself conditions. §4967 benefit screens apply to every grant regardless of monitoring settings. The text above is retained as originally written.

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-012. See the dedicated REQ-X-069 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-070: Workplace, payroll, matching and corporate giving

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-013. See the dedicated REQ-X-070 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-071: Institution implementation, service levels and operations

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-014. See the dedicated REQ-X-071 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-072: Identity, donor fraud and account recovery

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-015. See the dedicated REQ-X-072 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-073: Vendor assurance, SOC control ownership and evidence

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-016. See the dedicated REQ-X-073 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-074: Private foundations, trusts and planned-giving boundaries

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-017. See the dedicated REQ-X-074 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-075: Community campaigns, peer giving and attribution

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-018. See the dedicated REQ-X-075 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-076: Sponsor conflicts, related-party transactions and board governance

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-019. See the dedicated REQ-X-076 specification dated 2026-11-23. No implementation or tests claimed.

### REQ-X-077: Recipient interoperability, grant IDs and CRM attribution

**Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-020. See the dedicated REQ-X-077 specification dated 2026-11-23. No implementation or tests claimed.

### Reconciliation checkpoint

All 20 initial landscape gaps have proposed requirements REQ-X-058..077, with a cross-component ownership and collision register in `ONE-SYSTEM-CROSS-COMPONENT-RECONCILIATION-ALL-20-DAF-LANDSCAPE-GAPS-2026-11-23.md`. This is high-level design coverage, not exhaustive research-source traceability, verified competitor parity or production readiness.

## REQ-X-078: End-to-end workflow completeness and authoritative transition ownership (2026-11-23)

**Proposed:** Maintain explicit state-machine contracts for the 18 DAF enterprise workflows WF-01..WF-18 spanning sponsor activation, donor/fund onboarding, cash and complex gifts, custody, charity vetting, grants, recurring schedules, payments, checkout, recipient bank changes, fees, close, regulatory filing, cross-border grants, cases, migration and succession. Every transition must have one authoritative component, legal actor/authority, sponsor policy and evidence versions, typed command, financial effects, provider Unknown and compensation behavior, user-facing status, audit, and synthetic fixture. A feature requirement without complete transition coverage is not a workflow pass. Preserve REQ-X-057 one-system invariant.

**Acceptance (not executed):** `END-TO-END-DAF-ENTERPRISE-WORKFLOW-COMPLETENESS-AUDIT-2026-11-23.md`. Current baseline 18 mapped, 18 gaps flagged, 0 executed or certified.

## REQ-X-079: Cross-workflow financial, authority and irreversibility invariants (2026-11-23)

**Proposed:** Enforce INV-01..INV-18 across contribution acceptance, grantable capacity, atomic reservation, payment finality, provider Unknown, sponsor policy changes, sponsor identity, recipient bank changes, sponsor financial close, reporting, migration fencing, fee conflicts, tax receipts, donor anonymity, recurrence, and AI authority. External providers cannot participate in an imaginary atomic database transaction; use durable idempotency, reconciliation and compensation.

**Acceptance (not executed):** `CROSS-WORKFLOW-FINANCIAL-AUTHORITY-AND-IRREVERSIBILITY-INVARIANTS-2026-11-23.md`. Require synthetic before/after balance and authority snapshots, failure injection, concurrency and replay checks, independent legal/accounting review as applicable.

## REQ-X-080: Typed transaction state-transition matrix and authoritative accounting boundaries (2026-11-23)

**Proposed:** Specify and maintain machine-readable transaction transition contracts for contribution, complex asset acceptance, grants, recurring schedules, payment, charity payee change, fees, close, filing, migration, delegated rights, external checkout, charity reverification and sponsor activation. Every transition declares source/target state, authority, single in-process owner, atomicity classification, evidence, accounting effect, stable idempotency, failure/recovery and truthful user-visible state. External banks, custodians and regulators are not participants in a single database transaction.

**Acceptance (not executed):** `TRANSACTION-STATE-TRANSITION-MATRIX-2026-11-23.csv` contains 28 first-pass transitions. Future checks must verify allowed edges, cross-domain invariants, versioned commands, audit, compensation, sponsor isolation and explicit Unknown handling. No generated code or passing tests claimed.

## REQ-X-081: Adversarial transaction simulation and recovery fixture catalog (2026-11-23)

**Proposed:** Maintain deterministic synthetic failure fixtures for concurrent grants, returned payments, delayed provider outcomes, payee fraud, stale custody values, policy/authority changes, migration dual-writer risk, legal acceptance, sponsor reporting, AI overreach and disaster recovery. Each fixture requires before/after balances, authorization/policy snapshot, provider response, audit, UX status and recovery path. Reconciliation must precede retry after ambiguous external outcomes.

**Acceptance (not executed):** `ADVERSARIAL-TRANSACTION-SIMULATIONS-AND-RECOVERY-CONTRACTS-2026-11-23.md` defines 28 first-pass simulations, 0 executed or independently signed off.

## REQ-X-082: Machine-readable bidirectional workflow traceability and static consistency gates (2026-11-23)

**Proposed:** Require a typed, versioned traceability catalog connecting all WF, TX, INV, SIM and canonical REQ identifiers. Every transaction must link its owning workflow, relevant invariants, exact authority/policy guard, financial journal, evidence, user-visible state, idempotency and failure/recovery fixture. CI should reject invalid references, missing mandatory fields, unreachable transitions, orphan scenarios, inconsistent completion semantics and absent financial invariant assertions.

**Baseline actually checked:** `WORKFLOW-TRANSITION-TRACEABILITY-CATALOG-2026-11-23.json` and `STATIC-WORKFLOW-TRACEABILITY-CHECK-AND-CONTRADICTION-REGISTER-2026-11-23.md` report 18 WF, 18 INV, 28 TX, 28 SIM, 0 invalid SIM->TX references and 2 TX lacking explicit SIM references (TX-01, TX-02). This is a static ID-level check only. No runtime tests, automated CI, complete WF/INV links or numerical ledger validation were executed.

**Required follow-up:** split externally ambiguous state combinations, add exact debit/credit journal expectations and compensation, define irreversible boundaries, expand coverage to full requirement corpus and create executable validator only when implementation is authorized.

## REQ-X-083: Orthogonal payment state machine, rail-specific proof and governed recovery (2026-11-23)

**Proposed:** Model payment authorization, dispatch, provider knowledge, settlement proof, accounting reconciliation, recipient acknowledgement and work case as distinct dimensions. Provider timeout produces Unknown, not failure; accepted dispatch is not settlement; recipient handoff is not check clearing. Pin verified payee destination version, retain immutable request/event evidence, deduplicate callbacks, forbid blind retries, and require controlled recall/reissue/return compensation. Reuse one authoritative payment/ledger system for all channels and sponsors.
> Adopted 2026-11-23: refined by `RR-REQ-X-083-r1` (adopted-provisional); the adopted requirement is in `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md` under 'Adopted reconciliation requirements'.

**Acceptance (not executed):** `PRECISE-PAYMENT-STATE-MACHINE-AND-RAIL-PROOF-CONTRACT-2026-11-23.md` defines commands, six payment state dimensions plus work-case state, rail proof hierarchy and PAY-01..09 adversarial fixtures. Exact bank/custodian proof thresholds require provider and sponsor review.

## REQ-X-084: Sponsor-approved double-entry journal contracts, recognition gates and numerical fixtures (2026-11-23)

**Proposed:** Require balanced, sponsor/fund-scoped, immutable, versioned double-entry journal batches, with explicit source evidence, posting/effective/known-at times, corrections and accounting policy. Separate legal gift acceptance, revenue recognition, grant reservation, unconditional grant payable recognition, bank settlement, investment valuation, fund liquidity, fee accrual and late-period returns. Capacity is a derived controlled projection, not a directly editable ledger balance.

**Acceptance (illustrative, not accounting-certified):** `ILLUSTRATIVE-FUND-ACCOUNTING-JOURNAL-CONTRACTS-AND-REVERSALS-2026-11-23.md` and `ILLUSTRATIVE-LEDGER-JOURNAL-FIXTURES-2026-11-23.csv` document ten balanced synthetic journal examples. A qualified nonprofit fund accountant/CPA must approve actual chart of accounts, recognition timing, reversal treatment and regulatory reporting. No accounting engine or runtime tests executed.

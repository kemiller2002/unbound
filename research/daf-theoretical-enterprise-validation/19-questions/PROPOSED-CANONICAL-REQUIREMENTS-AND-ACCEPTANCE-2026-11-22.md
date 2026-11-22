# Proposed canonical cross-domain requirements and acceptance criteria

Date: 2026-11-22. **Draft, not formally approved.** This document is a proposed reconciliation overlay. Original registers remain authoritative until explicit migration. No simulation below has been run. Six challenged questions remain deferred.

## Reconciliation method
- Requirements describe observable business/control outcomes, not prematurely selected implementation technology.
- Every requirement has stable ID, linked source-question IDs, a falsifiable acceptance example and unresolved dependencies.
- No fixed thresholds, legal interpretations or third-party capabilities are invented.
- A failing safety gate blocks value movement while allowing safe observation and incident handling.

## Proposed canonical requirements

### REQ-X-001: Authority and agent delegation

**Source questions:** OD-01, OD-14, OD-17, OD-21, TVC-003, TVC-026, TVC-041, TVF-047, TVF-153, TVF-251.

**Normative proposed requirement:** Every agent/staff/advisor command has a verified principal, scope, policy version and authorization at execution; sponsor approval stays sponsor-controlled. Advisor-originated agent delegation remains disabled until validated.

**Acceptance scenario (not executed):** Given a revoked donor delegation, when an agent submits a new recipient grant, then the command is rejected, no reservation or payment is created, and evidence identifies the revocation.

**Open dependencies:** TVC-041.


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

**Normative proposed requirement:** Before release, sponsor-authorized destination, provider execution destination and relevant custodian/bank settlement instructions must reconcile using independently controlled evidence; mismatches hold payment.

**Acceptance scenario (not executed):** Given a provider-substituted account, when provider returns a self-issued matching fingerprint but sponsor-controlled evidence differs, then release is blocked.

**Open dependencies:** TVF-234.


### REQ-X-006: Outage, restoration and emergency safeguards

**Source questions:** OD-22, TVF-139, TVF-227, TVF-228, TVF-230, TVF-254, TVR-DR-001-Q, TVR-PAY-004-Q.

**Normative proposed requirement:** Direct authoritative edits remain prohibited; while commands are unavailable, no value-moving bypass exists; preserve protective holds, cancellations, credentials, event history and payment IDs across recovery.

**Acceptance scenario (not executed):** Given backup predating a hold and cancellation, when restored, then payment release remains disabled until later security events and protective requests are reconciled.

**Open dependencies:** TVF-228 TVR-DR-001-Q.


### REQ-X-007: Charity verification and sanctions

**Source questions:** OD-02, TVC-006, TVC-007, TVC-040, TVA-009, TVA-028, TVR-SAN-001-Q, TVR-SAN-002-Q, TVF-027, TVF-048.

**Normative proposed requirement:** Each recipient has a recorded periodic reverification no older than sponsor-approved 180-day interval, material-change rechecks and execution-time sanctions/eligibility checks; model never clears holds.

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

**Normative proposed requirement:** Do not treat provider capabilities, economic projections, shared sponsor determinations or deprovisioning latency as facts until supported by primary evidence and enforceable terms.

**Acceptance scenario (not executed):** Given a provider without independently provable non-receipt, when onboarding is evaluated, then the payment integration is not marked eligible for automatic retry.

**Open dependencies:** TVA-020.


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

TVC-034, TVC-041, TVA-020, TVR-DR-001-Q, TVF-228 and TVF-234. These remain unresolved and must not be silently approved.

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

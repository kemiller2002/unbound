# Cross-requirement consistency review
Date: 2026-11-22
Status: **Proposed architectural invariants and unresolved validation**, not formal approval, executed tests or verified legal interpretation.

## Scope and methodology
Nine interdependent control clusters were traced across 128 unique question IDs in the 183-item registry. This is a targeted cross-dependency review, **not** a complete pairwise proof for all 183 items. Existing OD-01–22 decisions are preserved as proposed working policy. Review events and their per-question cross-references are stored in `open-questions.typed.json`. The six challenged questions from the prior batch remain deferred for final user review.

## Cross-cutting invariants
1. **Authority:** Every consequential command is attributable to an authenticated principal with effective, versioned sponsor authority at execution; an agent never creates authority.
2. **Money:** Approval, reservation, funding, accounting recognition, payment initiation, provider receipt and finality are separate facts; no UI or API response conflates them.
3. **Release gate:** A payment requires valid sponsor approval, recipient eligibility, authority, policy, current funding, verified destination, no holds, and reconciled command provenance at the moment of release.
4. **Unknown:** Missing payment evidence is not proof of non-receipt. Unknown cannot be auto-retried or presented as paid.
5. **Reconstruction:** Independent review can reproduce balances, authority, decision inputs, payment identities and what was known at any prior recorded time, including later corrections.
6. **Emergency:** Outage and recovery procedures may preserve evidence and freeze risk, but cannot create an unreviewed alternate path for moving charitable assets.
7. **Separation:** The person or function preparing a financial reconciliation cannot independently certify the same work.
8. **Recipient verification:** Periodic 180-day review, change-triggered review and execution-time screening are distinct layers.
9. **User semantics:** The donor-facing status and embedded-widget receipt must describe the verified domain fact, not merely that a request was accepted by software.
10. **Evidence:** Unverified provider capabilities, legal reliance, economic assumptions, staffing and technical RPO targets remain gated hypotheses.

## 1. Governance, delegated approval and AI confirmation (High)
**Refs:** OD-01, OD-14, OD-17, OD-21, TVC-003, TVC-026, TVC-041, TVF-047, TVF-153, TVF-251.

**Conflict:** Version-pinned board delegation of sponsor routine grant approval is distinct from donor/advisor delegation of recommendations to an agent. A blanket demand that confirmation come from someone other than the delegator may disallow legitimate donor confirmation.

**Resolution:** Keep sponsor approval under board/counsel-validated policy; agent authority remains scoped, revocable and subordinate. At execution check current advisor authority and any recipient/destination changes. Keep advisor delegation gated. **TVC-041 remains deferred.**

**Challenge:** Independence must stop agent self-approval and coercive privilege expansion without introducing arbitrary two-person donor requirements. Test donor-only, successor, bereavement, compromised agent and assisted access.

## 2. Grant reservation, funding and recognition (Critical)
**Refs:** OD-04, OD-06, OD-07, OD-08, OD-10, OD-13, OD-19, TVC-021–023, TVC-036–038, TVC-043, TVF-001, TVF-009, TVF-252.

**Conflict:** Approved/reserved amounts can be mistaken for funded or legally committed grants; accounting recognition can be incorrectly pegged to UI approval or payment.

**Resolution:** Distinct grant request, sponsor decision, currency reservation, funding, external commitment, accounting payable and payment states. Routine approval+reservation atomic. Exceptional pending-funding grants must not imply a payment commitment. Lock ordinary percentage grants to dollar amount at approval with governed residual/recurring exceptions. CPA validates recognition event and cross-year accounting.

**Challenge:** Currency reservations can underfund after NAV moves; pooled liquidity may mask sponsor-specific risk. Simulate NAV shocks, competing reservations, cancellation, year-end notification and reversals.

## 3. Payment integrity, Unknown and retries (Critical)
**Refs:** OD-20, OD-22, TVC-024, TVC-035, TVC-039, TVR-PAY-001/002/005/007-Q, TVF-003, TVF-024, TVF-222, TVF-231, TVF-234, TVF-249, TVF-250.

**Conflict:** A stable idempotency key does not guarantee provider deduplication indefinitely; a provider's own fingerprint is not independent proof of destination. Multiple feeds may have the same controlling party.

**Resolution:** Persist globally unique payment identities across restores; retain Unknown as a hold state; require sufficient independent non-receipt proof before retries. Verify payment destination through an independently controlled source, not provider assertion alone. Investigate and reconcile mismatches before release.

**Challenge:** Providers may not offer the required evidence; design a safe manual exception without unverified retries. **TVF-234 remains deferred** pending an evidence-source choice and provider contract assessment.

## 4. Emergency access, restore and external authority (Critical)
**Refs:** OD-22, TVC-001, TVR-DATA-001-Q, TVR-DR-001-Q, TVR-PAY-004-Q, TVF-139, TVF-144, TVF-223, TVF-226–228, TVF-230, TVF-232–233, TVF-254.

**Conflict:** Domain-command-only emergency policy is incompatible with an unavailable normal command service unless a safe separate mechanism exists. Restores can discard later holds, payment identities, evidence or protective requests. Bank/custody consoles may bypass sponsor command controls.

**Resolution:** Default to freeze/protective observation during command outages. Preserve evidence and security state outside the restore boundary; reconcile all bank/custody/IdP authority changes to approved commands. An independent emergency command path, if adopted, requires prior dual-control validation and may not become a direct-edit bypass.

**Challenge:** Zero-loss RPO may be impossible or disproportionately expensive across independent vendors. **TVF-228 and TVR-DR-001-Q remain deferred**. Test outage, split-brain, replay, revoked credential and incomplete restoration.

## 5. Sanctions, eligibility and 180-day charity reverification (Critical)
**Refs:** OD-02, TVC-006/007/029/040, TVR-SAN-001/002-Q, TVF-027/048/149, TVA-009/028.

**Conflict:** A 180-day charity re-vetting cycle does not establish eligibility on a particular payment date. AI-generated negative signals and deterministic false-positive clearance must not become indistinguishable.

**Resolution:** Three gates: periodic comprehensive charity verification no older than the approved 180-day limit; event-driven review on material changes; execution-time sanctions/eligibility checks. Approved deterministic classes can clear defined false positives, with QA; models cannot clear holds. Failed checks block payment release.

**Challenge:** Excessive rechecks may increase manual overhead; use verified shared evidence and automated deltas where lawful, while retaining sponsor accountability. Verify source freshness, name matching and false-positive performance.

## 6. Finance segregation, loss and liquidity (High)
**Refs:** OD-05/09/10/13/17/18/20, TVC-004/005/028, TVR-ORG-003-Q, TVA-014/016/022.

**Conflict:** Controller can prepare reconciliations but cannot independently attest its own payment/ledger work. Liquidity planning does not determine legal allocation of operational losses.

**Resolution:** Controller preparation, independent reviewer, CFO remediation and Audit Committee material oversight; one versioned DoA. Separate investment loss, sponsor/vendor operational error and provider insolvency; document sponsor-approved recovery/make-whole decisions.

**Challenge:** Small-scale segregation may require contracted independent review; validate independence, cost and legal loss allocation.

## 7. UI, widgets and assisted access (High)
**Refs:** OD-15/16, TVC-002/008/009/011/012/014/025/031/032, TVR-WID-001/002/006-Q, TVR-SEC-007/010-Q, EXT-08.

**Conflict:** Widget receipt 'Accepted' can be misunderstood as sponsor approval or payment. Offline submission and assisted channels may carry stale authorization or weak identity.

**Resolution:** Command-specific receipts with clear domain fact and freshness; five-dimensional grant status; server drafts and online submission with fresh authority checks; partner revocation SLA; accessible security-equivalent assisted channels.

**Challenge:** Restrictive security can exclude users with disabilities; validate screen readers, bereavement workflows, alternative authenticators and low-tech partners.

## 8. Architecture, records and economic claims (High)
**Refs:** OD-12/15/16, TVC-001/002/015–020/027/034, TVR-GOV-001-Q, TVA-021/023.

**Conflict:** Event sourcing and stack choices are mechanisms, not necessary business outcomes. Coverage percentages, FTE floors and economic figures lack verified denominators or baselines. Immutable evidence needs lawful retention and deletion policy.

**Resolution:** Historical reconstruction invariant and ADR-gated architecture; one canonical requirement map; record-class retention; model economics as parameter ranges. **TVC-034 remains deferred** for source verification.

**Challenge:** Technology-neutral requirements can conceal feasibility constraints. Demand architectural proof-of-concept before selecting mechanisms, not implementation commitment.

## 9. Third-party and adoption assumptions (High)
**Refs:** TVA-002/003/004/007/008/018/019/020/025/026/027/030, EXT-01–05/09, TVF-237/238.

**Conflict:** Touchless economics rely on vendor idempotency, sponsor data, recipient acceptance, staff productivity and partner infrastructure that are unverified.

**Resolution:** Record evidence gates, adverse-case cost/surge models, fourth-party dependency concentration and change-of-control monitoring; do not commit staffing or service promises until measured. **TVA-020 remains deferred**.

**Challenge:** A conservative design could overbuild cost; test small sponsor, large sponsor and peak season rather than assuming one operating model.

## Proposed validation sequence
1. **Safety-critical proofs first:** release gate, Unknown payment, destination evidence, recovery/restore and sanctions holds.
2. **Accounting/legal proofs:** ASC 958 grant recognition, board delegation, DoA, loss allocation, record retention.
3. **Human and partner proofs:** accessible confirmation, embedded widgets, provider contracts, custodian/bank evidence, recipient status usability.
4. **Economics:** measured exception rates, volume spikes, SoD staffing and cost sensitivities.
5. **Reconcile sources:** update the canonical requirements and contradictory source documents only after decisions pass review; this report does **not** claim those edits were made.

## Final clarification queue
TVC-034, TVC-041, TVA-020, TVR-DR-001-Q, TVF-228, TVF-234. These are held for the user's end-of-review decision. No speculative legal, vendor or RPO value has been selected.

## Limits
This review covered nine targeted clusters and 128 unique registry questions, not exhaustive all-pairs consistency, source-document edits, completed simulations, regulatory opinions or external evidence verification. Remaining items should be checked during canonical source reconciliation.

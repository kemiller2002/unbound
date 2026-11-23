# Enterprise requirements gap register and next-wave specifications
Date: 2026-11-21 | Status: candidate gap inventory, not verified implementation coverage

## Purpose
Extend Unbound beyond individually specified modules. Cross-domain invariants, exceptions, owners, legal authority, accessibility, financial controls and evidence must be explicit before implementation. Preserve outcome, not legacy process. No app code authorized.

## Next requirements domains and candidates
### A. Reconciliation and financial close
- **UNB-GAP-001:** Reconcile independent custody, bank, payment, investment subledger, pool NAV, fund ledger and sponsor GL sources by entity, partner, fund, currency and as-of time.
- **UNB-GAP-002:** Define break ownership, aging, materiality, auto-match limits, manual override evidence and independent reviewer approval.
- **UNB-GAP-003:** Define daily operational cutoffs, month-end/year-end close, lock, late adjustments, restatements, signoff, auditor evidence and reproducible Folio outputs.
- **UNB-GAP-004:** Prove cash, investments, units, liabilities and fund allocations through independent source totals; never auto-plug unexplained differences.

### B. Grant lifecycle and disbursement
- **UNB-GAP-005:** Define grantee identity, charity verification, eligibility freshness, sanctions, foreign grants, conflicts, donor benefits and sponsor decision authority per jurisdiction.
- **UNB-GAP-006:** Support one-time, recurring, scheduled, conditional, multi-recipient and installment grant programs as distinct authorized obligations; prevent unauthorized automatic recurrence.
- **UNB-GAP-007:** Specify grant request, sponsor approval, liquidity reservation, investment liquidation, payable recognition, payment initiation, settlement, delivery confirmation, return, void, recovery and cancellation separately.
- **UNB-GAP-008:** Define cross-investment partial funding, failed redemption, changed NAV, currency drift and charity payment after correction, with bounded compensation and reapproval.
- **UNB-GAP-009:** Define payment rail integrations, beneficiary/payee verification, change controls, dual approval, positive pay where applicable, bank reconciliation and duplicate prevention.

### C. Contributions and noncash assets
- **UNB-GAP-010:** Define donor intent, conditional gift, receipt, acceptance, sponsor authority, valuation, acknowledgment, settlement and restriction preservation as distinct events.
- **UNB-GAP-011:** Define securities, privately held shares, partnership interests, real estate, restricted stock, digital assets if permitted and other complex gifts with eligibility, diligence, liquidation and costs.
- **UNB-GAP-012:** Separate gift tax documentation, donor-provided valuations, sponsor accounting valuations, appraisals and tax basis; support Form 8283/8282 applicability review.
- **UNB-GAP-013:** Handle unidentified deposits, rejected gifts, returned assets, disputed ownership, incomplete transfers and fraud alerts.

### D. Identity, authority and relationships
- **UNB-GAP-014:** Define identity proofing, invitation, consent, roles, multiple concurrent advisors, conflicting recommendations, delegation, revocation and step-up authentication.
- **UNB-GAP-015:** Define incapacity, death, succession disputes, contingent successor funds, multigenerational governance and legal/sponsor approval gates.
- **UNB-GAP-016:** Define least privilege, dual control, separation of duties, agent identity, sponsor reserved authority and complete negative authorization tests.
- **UNB-GAP-017:** Define tenant-to-tenant and sponsor-to-partner sharing agreements, permissible aggregate reporting, cross-partner privacy and export constraints.

### E. Compliance and case operations
- **UNB-GAP-018:** Map each regulatory assertion to applicability, legal source/date, control, responsible entity, evidence, human authority and negative test; independently verify before claiming compliance.
- **UNB-GAP-019:** Define compliance cases, investigation, legal holds, deadlines, escalation, appeal, closure and immutable case evidence.
- **UNB-GAP-020:** Define policy change proposal, independent approval, effective dating, rollout, rollback via supersession, exception expiry and model/tool governance.
- **UNB-GAP-021:** Define donor privacy, data minimization, retention, deletion/anonymization where legally required, append-only conflict resolution and legal holds.

### F. Reporting, tax and audit
- **UNB-GAP-022:** Specify donor, fund, sponsor, partner, board, auditor, regulator and grantee reporting personas and authority scopes.
- **UNB-GAP-023:** Specify Folio document catalog, versioning, source event high-water mark, policy/template version, accessible delivery, correction notices and retention.
- **UNB-GAP-024:** Specify sponsor nonprofit financial statements, Form 990/DAF schedules, donor acknowledgments, noncash gift documentation and state reporting support with human signoff.

### G. Enterprise administration and service
- **UNB-GAP-025:** Define partner onboarding, contract, branding, policy and fee configuration, sponsor relationship, integration provisioning, migration and termination.
- **UNB-GAP-026:** Define donor support, contact center, accessibility accommodations, case routing, communications preferences, disputes, complaint escalation and service evidence.
- **UNB-GAP-027:** Define sponsor operating AP/AR, billing, fee collection, budgets, procurement, vendor risk, staffing and financial management.
- **UNB-GAP-028:** Define business continuity, support coverage, SLAs, incident escalation, vendor exit, export and recovery responsibility.

### H. Platform, security and assurance
- **UNB-GAP-029:** Define API contracts and threat models for all commands, external integrations, webhooks, retries, idempotency, source provenance and tenant-bound authorization.
- **UNB-GAP-030:** Define Strata transactional guarantees, immutable events, corrections, replay, projections, audit integrity, disaster recovery and retention limitations through verified experiments.
- **UNB-GAP-031:** Define Forma mobile-first and WCAG 2.2 AA test matrix for every essential journey; assess applicable ADA obligations separately.
- **UNB-GAP-032:** Define Folio accessible, reproducible, signed/verified financial and compliance artifacts.
- **UNB-GAP-033:** Define agent identity, delegated tools, no self-approval, deterministic controls, prompt injection defense, monitoring, evaluation and safe no-AI operation.
- **UNB-GAP-034:** Define recovery-point/time objectives, cyber incident response, credential compromise, partner isolation, immutable backup and restoration tests.

## Cross-domain failure scenarios (required executable acceptance specifications)
- **UNB-XSC-001:** Grant funded from three investments; one redemption fails, another settles late and third NAV is corrected after charity payment. Preserve settled payment, compute correction impacts, enforce liquidity and authorized compensations.
- **UNB-XSC-002:** Simultaneous advisors submit percentage grants against the same fund; price shifts and reservations cannot create excess obligations.
- **UNB-XSC-003:** Sponsor employee revocation occurs during an approved but not executed custody transfer; authority is rechecked at execution.
- **UNB-XSC-004:** Donor death during unsettled investment transfer and pending grant; succession and sponsor authority determine each pending action.
- **UNB-XSC-005:** Partner migration between schema/database isolation tiers during active close; no duplicate postings, cross-tenant exposure or missing source evidence.
- **UNB-XSC-006:** A custodian statement conflicts with pool NAV, a grant is pending and an AI proposes an override; deterministic controls hold affected actions until independent review.
- **UNB-XSC-007:** Noncash gift accepted then valuation/appraisal changes after acknowledgment and financial statement publication; original documents preserved and corrected outputs authorized.
- **UNB-XSC-008:** Grantee bank details change just before payment while sanctions feed is unavailable; payment does not proceed without required fresh checks.
- **UNB-XSC-009:** A user requests erasure of personal data tied to immutable accounting evidence; retention, minimization, restricted access and legally permitted redaction are evaluated without destroying required records.
- **UNB-XSC-010:** A partner outage, duplicate webhook and retry race occur during fund split and grant settlement; no split-brain units, payments or journals.
- **UNB-XSC-011:** Year-end close completes; a late custodian correction materially changes fund NAV and fees. Produce governed restatement and preserve originally issued reports.
- **UNB-XSC-012:** AI tool receives malicious content in a grantee document and attempts to approve its own payment. Tool authorization rejects it and records evidence.

## Requirements completeness gate
For each domain and cross-domain scenario, record: owner, sponsor/legal authority, role/tenant scope, source/provenance, status (verified decision, assumption, legal question, design hypothesis), state machine, API and event contracts, accounting posting effect, Forma/Folio surface, failure mode, accessibility criteria, measurable tests and evidence. Track contradiction register and block implementation on unresolved legal, financial-integrity, authorization and data-isolation questions.

## Recommended immediate authoring sequence
1. Financial reconciliation and close state machine plus source-of-truth matrix.
2. End-to-end grant execution/payment state machine and cross-investment funding failure analysis.
3. Contribution/noncash gift acceptance and recognition policy.
4. Identity/authority matrix and compliance case workflow.
5. Tax/reporting and enterprise operations.
6. Final traceability, contradiction and adversarial test matrix.

This document is a prioritized requirements backlog, not a claim that other requirement files are absent or that any test has passed.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### UNB-RCN-ENT-016 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-033 · **Parent:** BASELINE scenario 'sponsor decision appeal' · **Decisions:** —
- **Requirement:** Donors and charities MUST be able to request reconsideration of a sponsor decision through a case with a defined outcome set, owner and response time. Staff-assisted actions MUST record channel, assurance level and staff identity, and MUST be confirmed through the donor's verified channel before any consequential effect.
- **Acceptance criteria:**
  - AC-1: Given a phone-assisted payee change; when it is entered by staff; then it is not effective until confirmed on a verified channel.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-016); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

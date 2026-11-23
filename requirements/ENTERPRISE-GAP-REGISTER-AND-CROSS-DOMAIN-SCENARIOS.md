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

### UNB-RCN-WF-001 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-03-T04; WFM-03-T11; WFM-03-T12; WFM-03-T13; WFM-04-T11; WFM-04-T12 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** Each inbound collection rail (ACH debit) and each outbound refund or redelivery of never-accepted property MUST have an Unknown-resolution protocol equivalent to UNB-RCN-GRT-008. Re-initiation after proven NotSubmitted MUST reuse the original idempotency key.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an ACH debit request whose provider response times out; when the next attempt is considered; then no new debit is sent until the protocol resolves the first; the same idempotency key is reused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-002 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-03-T28; WFM-04-T33 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** A contribution intent with no receipt MUST lapse after the sponsor window (default 60 days, RCD-046) and notify the declarant. A receipt after lapse MUST be treated as unannounced.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an intent declared 61 days ago with nothing received; when the lapse job runs; then the intent is Lapsed and the donor is notified.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-002); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-003 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-04-T06 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** A multi-lot securities intent MUST close as partial after the sponsor window (default 30 days) or on donor confirmation. Acknowledgment and Form 8283 then cover only the received lots.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given 600 of 1,000 shares received and 30 days elapsed; when the window closes; then the contribution is ClosedPartial and the acknowledgment describes 600 shares.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-004 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-03-T17; WFM-03-T24; WFM-03-T25 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** The sponsor MUST configure acknowledgment timing for reversible rails (OnAcceptance | AfterFinal; default AfterFinal, RCD-032). OnAcceptance requires a void-and-correction path on return.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a sponsor with AfterFinal; when an ACH contribution is accepted; then no acknowledgment is issued until Final.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-004); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-005 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-05-T18; WFM-05-T19 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** Each intermediary agreement MUST record its counsel-approved structure (donee model for release 1, RCD-034), which determines the donee of record and the acknowledgment, Form 8283/8282 and §4943 duties.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an intermediary agreement without a recorded structure; when a referral is attempted; then the referral is blocked.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-005); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-006 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-17-T05 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** The annual reporting obligation MUST track due date, extension and escalation tiers.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a package not signed 30 days before the due date; when the tier is reached; then the work item escalates to the CFO.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-006); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-007 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-17-T18; WFM-17-T19 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** An amended annual package MUST pass the same distinct prepare, review and sign states as the original.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an amendment; when the preparer attempts to sign; then signing is refused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-007); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-008 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-001 · **Parent:** WFM-18-T09; WFM-18-T10 · **Decisions:** RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** A mandatory-legal document whose delivery failed MUST be retried on another verified channel before it is closed as portal-only.
- **Validation still required:** RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an acknowledgment email bounced; when delivery fails; then it is sent by the next verified channel (post) before closing.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-008); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-009 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-01-T17 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Program activation MUST be an A4 sponsor-officer command requiring: a current contract; current board delegation; the legal-sponsor disclosure; a passing tenant isolation suite; tied-out opening balances; and finance readiness certified by a different principal.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an isolation suite failure; when activation is attempted; then activation is refused with the failing predicate named.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-009); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-010 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-01-T19; WFM-01-T20 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** On contract lapse the program MUST suspend new funds, contributions and approvals, while in-flight payments complete under their own workflows.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a lapsed contract; when a donor recommends a grant; then the recommendation is refused; an in-flight payment still settles.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-010); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-011 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-20-T06 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Rights suspended for incapacity MUST be reinstated only by an A4 reviewer on capacity-restoration evidence, with credentials re-issued through the verified channel.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given restoration evidence accepted; when the reviewer reinstates; then new credentials are issued; old sessions remain invalid.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-011); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-012 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-20-T01 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** A protective hold MUST pause standing contribution debits from accounts held by the subject.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a death notice for a donor with a monthly ACH debit; when the next debit date arrives; then no debit is initiated.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-012); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-013 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-19-T14; WFM-19-T15; WFM-19-T21 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Revocation and expiry MUST take effect at the next authorization check and in no case later than 60 seconds (RCD-041), including sessions, widget tokens and agent tokens.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a revoked advisor with an open session; when 61 seconds pass; then every request from that session is denied.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-013); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-014 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-22-T01; WFM-22-T10; WFM-22-T15 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Closure MUST define the policy for contributions received while closure is pending, withdrawal of a donor-requested closure before the residual is committed, and an A4 default residual disposition when the donor gives no recommendation within the policy window.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a closure with no residual recommendation after the window; when the window ends; then a default disposition work item opens for A4 approval.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-014); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-015 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-22-T12; WFM-22-T14; WFM-22-T17 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-045, RCD-046
- **Requirement:** Returns, voids and receipts for a Closed or Archived fund MUST route to its recorded trailing destination (RCD-040) and MUST NOT reopen the fund.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a grant return for a closed fund; when it is posted; then it credits the trailing destination; the fund stays Closed.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-015); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-016 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-24-T14; WFM-24-T15; WFM-01-T18 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Policy versions that depend on a board delegation MUST suspend when the delegation lapses, and routine approval MUST revert to human A4.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a lapsed delegation; when a routine grant is evaluated; then the outcome is ApproveRecommended for human approval.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-016); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-017 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-23-T07; WFM-23-T11; WFM-23-T13 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Work items MUST escalate on SLA breach, MUST be decided only by a principal who is not the preparer and holds authority for the action class, and MUST reopen when execution of the decision fails.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a decision whose execution fails; when the failure is recorded; then the work item reopens with the failure attached.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-017); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-018 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-25-T09; WFM-25-T13 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Reconsideration MUST have a request window (default 60 days), the outcome set {Upheld, Overturned, Modified, Referred}, a reviewer independent of the original decider, and a response time (default 30 days).
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an appeal reviewed by the original decider; when they attempt to decide; then the action is refused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-018); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-019 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-02-T14; WFM-02-T16 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Invitations MUST expire after the policy period (default 14 days) and MUST become invalid if the inviter's authority ends before acceptance.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an invitation whose inviter was revoked; when the invitee accepts; then acceptance is refused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-019); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-020 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-26-T10; WFM-26-T17 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** If a migration fence outcome is Unknown, Unbound MUST NOT post until the fence is confirmed. A migration run MAY be abandoned before cutover, with staging evidence retained.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a fence call that times out; when posting is attempted; then posting is blocked until the fence is confirmed.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-020); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-021 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-24-T06; WFM-24-T07; WFM-24-T08 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** Each sponsor policy parameter MUST be classified platform-safety or not. Changes to platform-safety parameters MUST receive platform-risk clearance, which grants no sponsor authority.
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a sponsor change loosening the sanctions SLA; when it is submitted; then it requires platform-risk clearance before approval.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-021); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-022 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-10-T10 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** Where NotReceived evidence cannot be obtained within the final aging tier, two independent treasury approvers MAY resolve by recorded decision with duplicate-risk acceptance and recovery plan (RCD-042).
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given an Unknown payment 31 days old without evidence; when a single approver attempts resolution; then it is refused; two independent approvers are required.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-022); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-023 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-10-T03 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** A payment whose destination evidence is ineligible for automated release MAY be released only by an independent A4 treasury approver; holds, sanctions and stale eligibility are never overridable.
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a payment with an open sanctions hold; when a treasury approver attempts manual release; then release is refused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-023); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-024 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-12-T09; WFM-08-T08 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** A confirmed sanctions match without property in possession MUST refuse the transaction and produce OFAC rejected-transaction report data within 10 business days (RCD-044).
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a true match before approval; when it is confirmed; then the grant is Refused, any reservation released, and a report obligation due in 10 business days exists.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-024); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-025 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-06-T08; WFM-07-T05; WFM-07-T06; WFM-07-T13; WFM-12-T05; WFM-13-T08 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** Draft lifetime, confirmation, re-affirmation, evidence-request, sanctions-disposition SLA and destination-verification windows MUST be versioned policy parameters with owners (defaults RCD-046).
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a policy version without the sanctions SLA parameter; when it is published; then publication is refused.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-025); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-026 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-08-T19 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** The CFO MUST approve the default funding ShortfallRule (reduce | waterfall top-up | hold; default hold, RCD-046).
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given no approved ShortfallRule; when a shortfall occurs; then the grant holds and a CFO work item opens.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-026); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-027 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-11-T11; WFM-11-T13 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** Positive-pay exception pay/return decisions MUST be in the payment authority model, with default Return (RCD-046).
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given an exception not decided by the bank cutoff; when cutoff passes; then the item is returned.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-027); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-028 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-13-T12 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** Destination activation MUST follow a cooling-off period (default 3 business days) and notify both the previous and the new verified channels.
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a verified new destination; when it is activated; then both contacts were notified at least 3 business days earlier.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-028); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-029 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-003 · **Parent:** WFM-08-T12; WFM-08-T13 · **Decisions:** RCD-039, RCD-042, RCD-043, RCD-044, RCD-045, RCD-046, RCD-047, RCD-048, RCD-049
- **Requirement:** A grant that fails capacity at approval MUST wait in CapacityShort for at most the sponsor CapacityWaitWindow (default 30 days), then be declined with a reason.
- **Validation still required:** RCD-042: CFO approval of the aging tiers; RCD-044: Counsel; RCD-046: Sponsor owners confirm defaults; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a grant 31 days in CapacityShort; when the window ends; then the grant is declined with reason 'insufficient funds'.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-029); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-WF-030 (P2, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-WF-002 · **Parent:** WFM-21-T07 · **Decisions:** RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** A fund under continued holding after dormancy outreach MUST be re-reviewed every 12 months (default, RCD-046).
- **Validation still required:** RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given a fund in ContinuedHolding for 12 months; when the review date arrives; then a re-review work item opens.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-WF-030); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

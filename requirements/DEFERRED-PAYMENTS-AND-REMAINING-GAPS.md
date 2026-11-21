# Deferred outbound payments and remaining enterprise requirements
Date: 2026-11-21 | Status: requirements planning only

## Explicit deferral: outbound payments
**Decision PAY-DEF-001:** Outbound grant payment execution SHALL be supported by the eventual Unbound enterprise, but selection of payment rails, banking partners, execution adapters and production payment sending is intentionally deferred. Do not mistake deferral for omission.

- **UNB-PAY-001:** Model grant sponsor approval, accounting recognition, payment obligation, payment instruction, payment authorization, submission, acceptance, settlement, return, void, recovery and reconciliation as separate events.
- **UNB-PAY-002:** Define a provider-neutral payment API and lifecycle, accommodating ACH, wires, checks and other approved rails subject to eligibility, geography, contracts and sponsor policy.
- **UNB-PAY-003:** No grant recommendation, AI proposal or internal accounting journal directly authorizes outbound funds. Enforce sponsor reserved authority, independent release control, payee verification and least privilege.
- **UNB-PAY-004:** Require idempotent external instructions, replay defense, duplicate detection, settlement evidence, reconciliation, safe retry and explicit uncertain-result handling.
- **UNB-PAY-005:** Record immutable payee identity, bank detail change controls, sanctions/eligibility freshness, approval evidence, payment status and correction/recovery history, with appropriate secrets/privacy protection.
- **UNB-PAY-006:** Support bank/rail outages, failed transfers, returned ACH, lost/voided checks, partial payments, payment holds, escheat/unclaimed property applicability and fraud escalation.
- **UNB-PAY-007:** Provide a simulated payment adapter and deterministic contract tests for grant/accounting integration before any real funds can move.
- **UNB-PAY-008:** Payment provider selection, sponsor bank relationships, funds-flow legal review, rail eligibility, limits, fraud operations, security certification and production authorization are explicit future decision gates.
- **UNB-PAY-009:** Forma SHALL display pending/approved/sent/settled/failed/returned states without conflating them. Folio SHALL generate payment advice, remittance notices and reconciliation/audit packs.
- **UNB-PAY-010:** Payment events SHALL reconcile grant payable, sponsor cash, custodian liquidity and bank confirmation, without claiming a payment is complete before external confirmation.

**Deferral boundary:** Now define payment domain contracts, statuses, control requirements, mock provider, test cases and integration seams. Later verify provider entitlements, select rail, build adapter, complete security and legal review, and authorize production sending. No provider selected and no live payments authorized.

## Other priority gaps to flesh out
- **UNB-REM-001 Charity master and grantee identity:** canonical charity identity, legal name/EIN or jurisdictional equivalent, aliases, merger/dissolution, eligibility evidence, sanctions, payment beneficiary and historic status.
- **UNB-REM-002 Charity lifecycle:** grantee onboarding, changes, acknowledgments, restricted-use reporting, communication, dispute and fraud/impersonation protection.
- **UNB-REM-003 Incoming contributions:** cash, stock, in-kind, illiquid, rejected, unidentified, conditional and cross-currency gifts; independent donor tax acknowledgment versus accounting recognition.
- **UNB-REM-004 Sponsor operations:** governance committees, board authority, delegated officers, service teams, finance, compliance, internal control ownership and independent reviews.
- **UNB-REM-005 Communications:** preference center, secure message delivery, accessible content, notices, receipts, opt-outs, bounce handling and immutable delivery evidence.
- **UNB-REM-006 Cases and disputes:** support tickets, privacy inquiries, succession disputes, grant reversals, beneficiary change, complaints, fraud, appeals, escalation and resolution evidence.
- **UNB-REM-007 Sponsor policy and fees:** effective-dated policies, investment menus, fees, waivers, exceptions, approval and rollback-by-supersession.
- **UNB-REM-008 Data migration and portability:** legacy DAF imports, source provenance, historical accounting, investment lots, restrictions, donor intent, advisors, successors, reconciliation, tenant migration and exit/export.
- **UNB-REM-009 Donor intent and strategy:** versioned philanthropic goals, geographic/mission preferences, restrictions, strategy change, impact evaluation and sponsor authority; avoid implying advisory wishes legally bind sponsor.
- **UNB-REM-010 Financial close and assurance:** daily/monthly/yearly reconciliation, audit, statement reproducibility, fraud controls and independent source verification.
- **UNB-REM-011 Organization administration:** billing, contracts, partner onboarding, procurement, vendor risk, staffing, continuity and performance.
- **UNB-REM-012 Tax/regulatory source crosswalk:** row-level verification of source obligations, applicability, control ownership, evidence and counsel review.
- **UNB-REM-013 Beneficiary experience:** charities can safely confirm organizational identity, grant status and restrictions, without seeing donor-private information without authority.
- **UNB-REM-014 Cross-border and geographic scope:** define permitted jurisdictions, grantee eligibility, FX, tax, sanctions, privacy and payment rules; no implied universal support.
- **UNB-REM-015 Financial integrity of grant funding:** simultaneous multi-investment redemptions, fee/FX changes, stale NAV, partial settlement, authorization expiry and source-of-funds provenance.

## Recommended sequence
1. Charity master/eligibility and recipient lifecycle.
2. Contributions and noncash gift acceptance.
3. Sponsor operations, identity and policy authority.
4. Communication, case management and donor/charity experience.
5. Financial close, data migration, tax/regulatory evidence and full traceability.
6. Payment sending implementation only after separately approved gate.

## Acceptance of this note
This document reserves the future payment capability while expressly prohibiting premature provider commitment or live execution. It is a requirements artifact, not a completed integration, legal review or payment implementation.

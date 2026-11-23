# REQ-X-070: Workplace, payroll, matching and corporate giving
Date: 2026-11-23. GAP-013, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Recurring contributions, provider adapters, sponsor contracts.

## Capability requirements
1. Employer/payroll batch import with consent, identity matching, withholding and settlement distinction.
2. Employer matching policy, eligibility, caps, timing, reversals and accounting.
3. Corporate account and employee fund ownership semantics, correct charitable receipt and legal sponsor identity.
4. File reconciliation, rejected rows, refund and wage-deduction exceptions.
5. Accessible employee portal and employer admin reports with privacy segregation.

## Native versus partner boundary
Partner-first for payroll, employer distribution and matching; native authoritative contribution/ledger workflow.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Payroll mismatch, employee termination, employer match reversal, invalid receipt, duplicate batch, employer sees private grant. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

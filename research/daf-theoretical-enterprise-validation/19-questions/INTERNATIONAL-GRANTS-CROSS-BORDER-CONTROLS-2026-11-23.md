# REQ-X-065: International grants and cross-border controls
Date: 2026-11-23. GAP-008, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Grant policy, charity vetting, provider adapters, fund accounting.

## Capability requirements
1. Recipient legal status and country-specific eligibility, sanctions and restricted-party checks with dated evidence.
2. Equivalency determination and expenditure responsibility routes when applicable, including approved legal review and reporting.
3. FX quote, spread, settlement currency, beneficiary identity, wire/return and intermediary fees with reconciliation.
4. Country, purpose and sponsor restrictions; grant-purpose monitoring, anti-diversion evidence and hold lifecycle.
5. Donor/recipient communications, language and privacy, permitted local payout rails.

## Native versus partner boundary
Sponsor/legal counsel owns eligibility; Unbound owns controlled workflow; specialized cross-border diligence and payments may be partners.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Restricted jurisdiction, stale determination, currency move, unknown wire, diversion alert, return, sanctioned intermediary. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

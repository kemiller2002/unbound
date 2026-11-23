# REQ-X-067: Partner economics, fee waterfalls and conflicts
Date: 2026-11-23. GAP-010, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Accounting, fee rules, sponsor contracts, reporting.

## Capability requirements
1. Contract-based payer/payee/service/revenue mapping for sponsor, platform, adviser, custodian, distributor and payment rail.
2. Tiered AUM, fixed, minimum, transaction, pass-through, waiver, retroactive amendment and effective-date fee policies.
3. Accrual, posting, settlement, reversals, reconciliation and independent approval in authoritative ledger.
4. Related-party and model-sponsorship incentives, disclosure, recusal and fee transparency.
5. Profitability analytics distinguish legal sponsor assets, adviser AUM, serviced AUM, flow, platform revenue and cost.

## Native versus partner boundary
Unbound owns contract and accounting evidence; external billing/settlement provider only an adapter.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Fee double charge, rebate, related-party conflict, negative balance, stale AUM, missing invoice, contract change. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

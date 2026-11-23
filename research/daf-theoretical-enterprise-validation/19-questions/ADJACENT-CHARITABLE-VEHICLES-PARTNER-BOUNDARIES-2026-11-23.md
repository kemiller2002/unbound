# REQ-X-074: Private foundations, trusts and planned-giving boundaries
Date: 2026-11-23. GAP-017, P2. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Legal sponsor contracts, contributions, donor guidance.

## Capability requirements
1. Explicit capability registry for DAF versus private foundation, CRT, CLT, charitable gift annuity and single-charity fund.
2. Vehicle-specific legal ownership, tax, payout, accounting, eligibility and distribution rules must not be reused without validation.
3. Referral/partner workflow, consent, lead handoff and no misleading claims of support.
4. Comparative donor guidance includes direct gift and no DAF where appropriate.
5. Separate ADR before extending financial core to non-DAF legal entities.

## Native versus partner boundary
Default partner/defer; one Unbound system does not require supporting every charitable vehicle.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: QCD misdirected to DAF, annuity treated as grant, private foundation self-dealing, unlicensed advice. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

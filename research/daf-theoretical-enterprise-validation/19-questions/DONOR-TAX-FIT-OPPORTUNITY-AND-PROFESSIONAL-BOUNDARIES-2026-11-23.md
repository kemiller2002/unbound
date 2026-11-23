# REQ-X-066: Donor tax-fit and charitable opportunity guidance
Date: 2026-11-23. GAP-009, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Donor onboarding, contributions, AI contracts, advisor views.

## Capability requirements
1. Compare DAF, direct giving, alternative vehicles and no gift with neutral objective and eligibility.
2. Consent-based holdings and tax-lot signals, missing-data/uncertainty checks and advisor planning workflow.
3. Tax-year and jurisdiction versioning, itemization, AGI limits, special 2026 rules and QCD exclusions subject to qualified review.
4. Scenario assumptions, fees, opportunity costs, irrevocability and investment restrictions in transparent comparisons.
5. Escalation to tax professional before individualized irrevocable transfers; no AI tax determination.

## Native versus partner boundary
Partner professional advice and portfolio data; Unbound owns transparent decision support and consent.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Nonitemizer, low income, no DAF recommendation, QCD to DAF attempt, stale tax year, short-held stock, conflicting assumptions. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

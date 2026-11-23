# REQ-X-071: Institution implementation, service levels and operations
Date: 2026-11-23. GAP-014, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Sponsor onboarding, configuration, work queue, operational metrics.

## Capability requirements
1. Standard implementation lifecycle: diligence, contract, sponsor setup, custody/payment integrations, migration, testing, training, activation.
2. Service catalog and tiered SLAs with exact clock definitions, holidays, exclusions and evidence.
3. Customer success, support intake, incident management, escalations and sponsor communication using shared work queue.
4. Configuration templates, readiness gates, synthetic fixtures, parallel-run verification and rollout controls.
5. Cost-to-serve, onboarding duration, failure modes and service-credit/contract implications.

## Native versus partner boundary
Build shared operating process; outsource implementation labor selectively without bypassing control gates.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Incomplete integration, breached SLA, customer-specific fork request, failed cutover, conflicting support ownership. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

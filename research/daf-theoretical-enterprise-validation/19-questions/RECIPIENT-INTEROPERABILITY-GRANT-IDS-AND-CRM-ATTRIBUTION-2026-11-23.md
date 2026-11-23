# REQ-X-077: Recipient interoperability, grant IDs and CRM attribution
Date: 2026-11-23. GAP-020, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** External IDs, recipient portal, API contracts, payment events.

## Capability requirements
1. Canonical grant/payment/recipient identifiers and sponsor/provider external-ID crosswalk with provenance.
2. Structured remittance metadata, donor anonymity, grant purpose, legal restrictions and reconciliation references.
3. Recipient CRM matching, acknowledgement and corrected attribution without rewriting immutable as-issued history.
4. Versioned API/webhooks, idempotency, consent, status timestamps and privacy scopes.
5. Duplicate charity identity merge/split with audit and historical reference stability.

## Native versus partner boundary
Build shared interoperability contracts; partner nonprofit CRM connectors.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: External ID collision, renamed nonprofit, duplicate donor, wrong attribution, webhook replay, privacy leakage. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

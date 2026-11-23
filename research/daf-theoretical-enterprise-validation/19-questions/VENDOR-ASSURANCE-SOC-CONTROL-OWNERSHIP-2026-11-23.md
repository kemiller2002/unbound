# REQ-X-073: Vendor assurance, SOC control ownership and evidence
Date: 2026-11-23. GAP-016, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Security governance, integration evidence, DR.

## Capability requirements
1. SOC 1/2 scope by legal entity/product/service and period, complementary user entity controls and bridge letters.
2. Subprocessor inventory, contracts, data residency, retention, encryption and access obligations.
3. Control ownership, evidence collection, exception tracking, remediation and independent review.
4. Third-party concentration, outage, exit, cyber and incident notification requirements.
5. Audit-ready reports with provenance and explicit unavailable/expired assurance.

## Native versus partner boundary
Unbound owns vendor governance; independent auditors issue assurance, not AI.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Outdated SOC report, wrong legal entity, missing CUEC, critical subprocessor outage, report scope gap. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

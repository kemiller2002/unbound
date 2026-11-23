# REQ-X-069: Nonprofit impact, restrictions and grant outcomes
Date: 2026-11-23. GAP-012, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Grant lifecycle, recipient portal, reports, evidence.

## Capability requirements
1. Optional sponsor-approved purpose/restriction classification without prohibited donor benefit.
2. Recipient acknowledgement, outcomes, milestones and permitted evidence collection with consent.
3. Distinguish donor-facing impact claim from charity self-report and independently verified outcome.
4. Nonprofit reporting burden limits, reusable data and shared charity evidence with sponsor privacy.
5. Reminders, exceptions, late or missing outcomes and escalation via existing case queue.

## Native versus partner boundary
Build lightweight evidence model; partner specialized impact measurement only when justified.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: False impact claim, restricted grant misuse, anonymous donor, sensitive beneficiary data, duplicate outcomes, missing report. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

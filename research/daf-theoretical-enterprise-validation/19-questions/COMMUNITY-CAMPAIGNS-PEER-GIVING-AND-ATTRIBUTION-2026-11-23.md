# REQ-X-075: Community campaigns, peer giving and attribution
Date: 2026-11-23. GAP-018, P2. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Donor collaboration, external widgets, grant lifecycle.

## Capability requirements
1. Optional campaign objects link to sponsor-authorized recommendations, never pool donor assets outside legal rules.
2. Invitations, sharing, anonymity, privacy, attribution and prohibited-benefit restrictions.
3. Campaign goals/pledges distinguish intent, accepted contribution, approved grant and settled payment.
4. Nonprofit recipient and organizer permissions, misuse reporting and takedown.
5. Impact reporting and accessibility with low-cost optional adoption.

## Native versus partner boundary
Defer broad social features pending demand; use existing fund/grant/identity contracts if adopted.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: False campaign goal, donor privacy, prohibited benefit, organizer impersonation, duplicate donation. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

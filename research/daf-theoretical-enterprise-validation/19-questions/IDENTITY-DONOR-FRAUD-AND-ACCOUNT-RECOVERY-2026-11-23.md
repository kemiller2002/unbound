# REQ-X-072: Identity, donor fraud and account recovery
Date: 2026-11-23. GAP-015, P1. Theoretical requirements only.

## Outcome and one-system boundary
Extend the existing Unbound **component monolith** under REQ-X-057. Reuse the authoritative fund ledger, sponsor policy, identity and delegated authority, work queues, documents, provider integration, evidence and semantic reporting. Do not create an independent product, policy engine, payment state machine, ledger or shadow donor account.

**Existing component ownership:** Shared identity, delegation, sponsor policy, audit.

## Capability requirements
1. Identity proofing and fraud signals by donor, charity contact, advisor, employee and external developer role.
2. MFA/passkeys/SSO, session binding, device risk, revocation, consent and least-privilege delegated access.
3. Account recovery with identity re-proofing, dual controls for sensitive changes and social engineering defenses.
4. Suspicious contribution, grant, beneficiary, payee and access change detection with reversible holds.
5. Privacy-preserving evidence, retention, appeals and accessible alternatives.

## Native versus partner boundary
Partner identity verification and fraud feeds; Unbound owns authorization and transaction holds.

## Role-specific UX and data contracts
Provide sponsor-configurable views for the authorized donor, advisor, charity, operator, sponsor and external provider as applicable. Each action requires a typed, versioned input and AI-discoverable contract, validated authorization, explicit authoritative state and provenance. Preserve sponsor isolation, privacy, consent, accessibility, mobile responsiveness, mock-mode provider interactions and nonblank loading/Unknown/error states. Sponsor branding may rename concepts but cannot alter their legal meaning.

## End-to-end workflow and exception acceptance
Define initiation, prerequisites, deterministic policy checks, external dependencies, domain command, accounting/operational effect, independent approval where required, communications, reconciliation, audit evidence and remediation. Synthetic cases: Account takeover, synthetic identity, deepfake support call, lost MFA, stolen token, compromised charity contact. Require positive, negative, concurrency, stale-source and permission-denial cases. Do not equate a theoretical specification with a passing test.

## Challenge and unresolved decisions
Validate buyer demand, legal scope, actual vendor evidence, per-sponsor variations, cost-to-serve and whether Build/Partner/Defer/Exclude is economically justified. Unknown evidence remains Unknown. Specialist legal, tax, custody and assurance judgments must not be replaced by AI.

# REQ-X-061: Legacy migration, portability and sponsor exit
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
An institution can switch to or leave Unbound with reconcilable fund histories, legal ownership, grants, donor access, documents and audit continuity.
## Core requirements
- Versioned import/export contracts for sponsor/fund/party relationships, positions, lots, GL/subledger entries, grants, payments, recipient evidence, fees, documents, policy history and external IDs.
- Stage -> map -> validate -> reconcile -> parallel run -> authorize cutover -> freeze old writes -> establish single writer -> verify -> rollback/compensate. Never silently invent missing historical transactions.
- Support historical known-at/effective-at and as-issued document provenance; unknown legacy details remain Unknown and require evidence or disclosed limitations.
- Separate migration of account relationships, financial assets/custody and legal sponsor authority; platform change does not imply sponsor change or donor ownership of charitable assets.
- Provide sponsor contract offboarding, export rights, secure deletion/retention boundaries, evidence packages and continuity plans.
- Require financial tie-outs by fund, position, period and pending payment, duplicate/retry handling and safe cutover of bank/payment instructions.
## Adversarial acceptance
Cases: missing years of history, duplicate fund IDs, different chart of accounts, open ACH return, stale check, migrated closed statement, dual writer, donor invitation conflict, sponsor exit during legal hold.
## Decision challenge
Migration is a product, not a one-off professional services script; however, individual legacy adapters may be partner-delivered under Unbound-controlled validation.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

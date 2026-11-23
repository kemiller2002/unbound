# REQ-X-062: Advisor, wealth manager, custody and CRM integration
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
Support advisor-led DAF programs with coherent relationship management, delegated access, sponsor-owned investments, multi-custody data and transparent compensation.
## Core requirements
- Distinguish legal sponsor, donor, advisor, broker/custodian, investment manager and technology provider; role grants cannot transfer legal title or sponsor approval.
- Advisor book-of-business and client servicing views are derived from scoped canonical party/fund relationships; SSO/SCIM, CRM sync, revocation and delegated roles are typed adapters.
- Track sponsor-approved investment models, adviser mandates, restrictions, custody accounts, trades, corporate actions, tax lots, valuations, fees, conflicts and rebalancing exceptions.
- Multi-custody reconciliation, freshness, corporate action corrections and historical as-of reporting feed one accounting core; no wealth-manager shadow balances.
- Record fee waterfall and related-party compensation with sponsor approvals and donor-facing disclosures.
- Allow advisor assistance with grant recommendation and funding but preserve donor consent, sponsor decision and restricted legal advice boundaries.
## Adversarial acceptance
Cases: advisor removed, conflicting custodian balance, same donor at two firms, trading restriction, related-party model incentive, stale tax lot, cross-sponsor SSO, portfolio transfer during pending grant.
## Decision challenge
External custodians own actual custody/trading execution; Unbound owns authorization records, evidence, policy and reconciliation.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

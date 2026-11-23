# REQ-X-059: Complex contributions, gift acceptance and liquidation
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
Accept cash, publicly traded securities and sponsor-approved complex assets with correct legal acceptance, valuation, custody, liquidation, tax substantiation, fee accounting and grantable capacity.
## Core requirements
- Separate donor intent, conditional intake, sponsor legal acceptance, custody confirmation, valuation, liquidation, investment allocation, available balance and tax receipt.
- Gift acceptance policy is asset-type/sponsor-specific: private company shares, real estate, restricted stock, partnership interests, cryptocurrency and noncash property are optional, not presumed universally accepted.
- Track appraisal requirements, restrictions, liens, debt, environmental risks, UBIT exposure, unrelated parties, prearranged sale risks and donor benefit concerns for qualified counsel/CPA review.
- Custodian/source positions, lots, corporate actions, fees and liquidation costs reconcile to fund subledger/GL; uncertain valuations do not become spendable cash.
- Record qualified-appraisal and donor/sponsor substantiation dependencies without pretending the platform is a qualified appraiser or tax adviser.
- Provider adapters for brokers, custodians, appraisal firms, liquidation and escrow are scoped, idempotent and mockable; provider Unknown is not failure.
## Adversarial acceptance
Cases: pending asset transfer, restricted shares, stale FMV, conflicting appraisal, liquidation shortfall, corporate action, donor withdrawal before acceptance, donor request after irrevocable acceptance, unrelated debt, duplicate custody feed.
## Decision challenge
Start with asset-class policy capability rather than promising acceptance of every asset. Specialized liquidation is a partner boundary; authoritative acceptance and accounting remain Unbound.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

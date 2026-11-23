# REQ-X-064: Legal operating models, sponsor contracts and responsibility matrix
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
One Unbound system supports sponsor-operated, outsourced administration, turnkey sponsor partnership, digital donor and embedded channels without ambiguous legal authority.
## Core requirements
- Versioned SponsorOperatingAgreement binds sponsor charity, administrator, technology vendor, adviser, custodian, distributor and payments provider to responsibilities, rights, SLAs, data ownership, retention, fee flows, liability and termination.
- Every domain command maps actor -> legal role -> sponsor delegation -> approval authority -> evidence; contractual outsourcing never silently transfers nondelegable sponsor authority.
- Board policies, conflict recusals, sponsor-specific restrictions, legal counsel/CPA approvals and delegated authority remain enforceable in one policy/identity framework.
- Sponsor program lifecycle: prospect -> diligence -> agreement -> configuration -> control validation -> activation -> operations -> amendments -> offboarding; no second onboarding product.
- One branded experience can involve several legal parties; clearly disclose who sponsors the DAF, who advises assets and who processes grants.
- Sponsor contract amendments trigger affected policy/fees/workflow impact analysis and migration-safe versioning.
## Adversarial acceptance
Cases: vendor operator tries to approve own work, conflicting sponsor policies, expired contract, distributor implies ownership, fee beneficiary conflict, legal entity switch, data access after offboarding, sponsor protective hold.
## Decision challenge
Software cannot manufacture a sponsoring charity; any turnkey program needs a real authorized legal sponsor partner. Contract/legal design requires counsel.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

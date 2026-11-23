# REQ-X-058: Nonprofit recipient operations and self-service
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
A charity can safely establish its identity, manage authorized contacts, provide verified payment instructions, view its grants and remittance details, acknowledge receipts, and resolve payment exceptions without a separate grant/payment system.
## Core requirements
- Charity entity is globally identified with evidence provenance and 180-day reverification, while sponsor-specific permissions and restricted evidence remain isolated.
- Authorized nonprofit contacts enroll with strong identity and affiliation proof, MFA, role delegation, recovery and revocation; changes to bank destinations require independent verification, cooling-off or risk-based hold, and separation from payment approval.
- Recipient portal offers grant list, donor anonymity and disclosure choices, remittance details, downloadable documents, acknowledgements, payment state and support cases. No donor-sensitive information without explicit entitlement.
- Grant/payment status comes exclusively from authoritative Unbound grant and payment components; charity-side acknowledgements are evidence events, not sponsor payment finality.
- External nonprofit CRM mapping, charity identifiers, callbacks and APIs use typed scoped contracts and durable idempotency.
- Support no-digital-access charities through checks, verified phone/mail and staff-assisted workflows with cost transparency.
## Adversarial acceptance
Synthetic cases: stolen charity contact, spoofed bank change, duplicate nonprofit records, cross-sponsor privacy, lost check, payment Unknown, donor anonymity, revocation mid-session, unclaimed nonprofit, disputed remittance. Require safe holds and ledger reconciliation.
## Decision challenge
Avoid forcing charity registration before every grant; use low-friction verified disbursement options. Distinguish a charity contact from an authorized bank signer. Partner identity/bank validation; retain Unbound policy and decision authority.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

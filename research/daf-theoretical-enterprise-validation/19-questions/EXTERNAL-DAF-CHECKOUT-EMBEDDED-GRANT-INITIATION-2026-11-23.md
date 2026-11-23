# REQ-X-063: External DAF checkout and embedded grant initiation
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
A nonprofit checkout or wealth application can initiate an authorized DAF grant recommendation using secure embeddable widgets/APIs without bypassing the sponsor's grant process.
## Core requirements
- Consent-based donor authentication and scoped external application identity; no raw donor credentials or fund balances exposed by default.
- External app submits typed recommendation with charity identifier, amount, donor intent, permitted metadata, expiry, origin reference and idempotency key.
- Canonical grant validation, prohibited benefit affirmation, sponsor policy, charity eligibility, reservation, approval, payment, settlement and recipient acknowledgement remain in shared Unbound modules.
- Callbacks/webhooks signed, replay-safe, versioned and privacy-scoped; status distinguishes submitted, approved, payment initiated, processor accepted, recipient available, settled and reconciled.
- Support sponsor terminology, brand, accessibility, mobile, error states and return/cancel flow with safe recovery from abandoned checkout.
- Protect against fraudulent charity substitution, unauthorized balance discovery, sponsor spoofing and retry duplicates; optional Chariot-like partners are adapters, not a second grant engine.
## Adversarial acceptance
Cases: replayed request, revoked donor consent, stale charity status, donor benefit prohibited, sponsor unavailable, partial callback, two simultaneous checkout tabs, unauthorized balance, payment Unknown, external merchant claim of completion.
## Decision challenge
Do not equate 'instant checkout' with instant money arrival; measure the distinct stages.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

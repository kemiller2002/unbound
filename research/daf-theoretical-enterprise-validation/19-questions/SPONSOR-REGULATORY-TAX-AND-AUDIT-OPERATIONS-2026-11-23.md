# REQ-X-060: Sponsor regulatory, tax and audit operations
Date: 2026-11-23. Proposed theoretical requirements, not implementation or legal validation.

**Architectural invariant:** REQ-X-057, one Unbound component monolith, one authoritative accounting, policy, case, document, evidence and AI assistance system. All role-specific UX and external provider integrations reuse domain contracts.

## Outcome
Support sponsor's legal and financial reporting obligations through governed evidence, accurate period close, donor substantiation, audit schedules, regulatory filings and accountable review.
## Core requirements
- Maintain a counsel/CPA-reviewed obligation register by legal sponsor, jurisdiction, filing year, entity type, deadlines, owners and applicability; do not assume every named tax form applies to every sponsor.
- Generate contribution acknowledgement evidence, grants classification, donor records, fee/related-party transactions, grantmaking reports, fund balances and audit support from authoritative ledger and immutable documents.
- Provide Form 990 and applicable schedules data packages, applicable state filings, donor/noncash substantiation, annual audit evidence, board oversight and related-party conflict review.
- Draft, reconcile, independently review, approve, file externally and archive submission/acceptance evidence as distinct states; no AI filing or signature authority.
- Tie annual packages to locked financial periods and bitemporal corrections; explain as-filed versus corrected, retain legal holds and sponsor-specific retention.
- Track external CPA/counsel tasks and exceptions in existing work queue, never separate compliance tracker.
## Adversarial acceptance
Cases: wrong filing-year rule, amended return, unreconciled grant, unapproved related-party fee, corrected statement after filing, missing acknowledgement, late deadline, restricted evidence, no-filing obligation.
## Decision challenge
A reporting engine does not establish legal compliance; qualified professionals own interpretation, signoff and filing.

## Common acceptance and traceability
Map role-based UI, API contracts, domain owner, ledger consequences, policy versions, data provenance, provider adapters, deterministic mocks, work queue exceptions, privacy, accessibility, audit, report implications and cost-to-serve. Preserve Unknown, no blind payment retries, no cross-sponsor data leakage. No tests executed.

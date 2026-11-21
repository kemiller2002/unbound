# Full enterprise and DAF fund accounting requirements
Date: 2026-11-21 | Status: owner-directed mandatory requirements; detailed accounting policy and jurisdictional review pending

## Binding scope
Unbound SHALL include a **complete integrated accounting system**, not just a transaction log, grants subledger, or external GL connector. Accounting is a first-class enterprise capability with governed APIs, Forma interfaces, Folio reporting, and Strata integrated into append-only authoritative history. Do not assume Strata's actual accounting functionality until its APIs and guarantees are verified.

Distinguish (1) **DAF fund accounting** within a sponsoring charity's legally controlled assets, (2) **sponsor operating/general ledger accounting**, and (3) **partner/program reporting and settlement**, which may be informational rather than a separate legal entity's books. Legal entities, funds, donor advisory privileges, institutional partners, custodians, bank accounts and chart-of-accounts dimensions are not interchangeable.

## Core accounting requirements
- **UNB-ACC-001:** Provide a complete configurable, versioned chart of accounts with legal entity, sponsor, partner/program, DAF fund, department, project, restriction, asset, currency and other approved reporting dimensions; enforce dimension validity.
- **UNB-ACC-002:** All accounting postings SHALL be double-entry, balanced by defined accounting entity and currency/measurement rules, with immutable journal headers/lines, unique posting IDs, effective dates, recorded dates, authorizers and provenance.
- **UNB-ACC-003:** Posted journals SHALL never be updated or deleted. Corrections SHALL use linked reversal, adjustment or reclassification entries with approval and preserved original history.
- **UNB-ACC-004:** Maintain integrated general ledger and subsidiary ledgers for DAF balances, cash/banks, custody/securities, grants payable, accounts payable, accounts receivable, contributions, investments, fees, expenses, payroll interface, fixed assets where relevant and inter-entity balances.
- **UNB-ACC-005:** Separate donor-advised fund accounting from sponsor operating activity and enforce restrictions, liquidity and sponsor ownership/control; DAF subfund balances are not personally owned donor assets.
- **UNB-ACC-006:** Record contribution lifecycle from intent through receipt, valuation, restrictions, acceptance, acknowledgment, settlement and disposition, with distinct cash, securities and complex noncash treatments.
- **UNB-ACC-007:** Record grant lifecycle from recommendation through sponsor approval, commitment, payable, payment, clearing, returns, voids and recoveries; no recommendation alone creates a payment.
- **UNB-ACC-008:** Support cash, accrual and required reporting bases through explicit approved policies and reconciled adjustments; do not assume a single basis applies to every entity/report.
- **UNB-ACC-009:** Support investments, units/NAV, gains/losses, interest, dividends, corporate actions, custody reconciliation, valuations, impairments and liquidity, subject to documented accounting policy.
- **UNB-ACC-010:** Support sponsor and partner fee schedules, accruals, waivers, allocations, revenue recognition, billing, collections and intercompany/interfund settlement without double counting.
- **UNB-ACC-011:** Support internal fund allocations, fund mergers, closures, outbound transfers, successor-account splits and multigenerational successor transfers with balanced immutable entries, restrictions and provenance.
- **UNB-ACC-012:** Support accounts payable, purchasing/expense approvals, vendor master controls, reimbursements, accruals, prepaid expenses and payment reconciliation for sponsor operations.
- **UNB-ACC-013:** Support accounts receivable, invoicing, credit adjustments, collections and deferred revenue where applicable.
- **UNB-ACC-014:** Support budgeting, forecasts, actual-versus-budget reporting, cost centers, allocation methodologies and scenario models while distinguishing forecasts from posted actuals.
- **UNB-ACC-015:** Support multiple legal entities and sponsor structures, intercompany/interfund transactions, eliminations and consolidated reporting where authorized and legally applicable.
- **UNB-ACC-016:** Support currencies, FX rates, source currency, functional/reporting currency, remeasurement, translation and realized/unrealized FX results where needed; currency conversion never silently changes source amounts.
- **UNB-ACC-017:** Define accounting periods, soft/hard close, reconciliations, close checklists, adjusting entries, period locks, reopen approvals and late-arriving event treatment without modifying posted journals.
- **UNB-ACC-018:** Reconcile GL and subledgers to independent bank, custodian, payment-rail, investment and partner statements, with aged breaks, ownership, evidence and escalation.
- **UNB-ACC-019:** Provide financial statements including trial balance, balance sheet/statement of financial position, statement of activities, cash flows, functional expenses where required, fund statements, restricted/unrestricted analyses and management reporting, subject to entity-specific reporting rules.
- **UNB-ACC-020:** Support audit schedules, roll-forwards, supporting workpapers, transaction drill-down, external auditor access, evidence export and independently verifiable as-of financial snapshots.
- **UNB-ACC-021:** Support tax and regulatory reporting source data, including applicable Form 990/DAF disclosures and other filings, with jurisdictional applicability and human sign-off; do not assume automatic filing authority.
- **UNB-ACC-022:** Enforce segregation of duties for journal preparation, approval, posting, bank/payee changes, reconciliations, close certification, master data and payment release; AI cannot approve its own accounting actions.
- **UNB-ACC-023:** Every accounting operation SHALL have an authenticated, versioned API with authorization, idempotency, concurrency, error states, traceability and evidence; external callers cannot directly post unrestricted journals or instruct money movement.
- **UNB-ACC-024:** Forma SHALL provide accessible mobile-first accounting experiences, including approval, exceptions, fund statements, reconciliation review, close dashboards and audit drill-down; complex tasks must remain possible on a phone.
- **UNB-ACC-025:** Folio SHALL generate accessible versioned financial statements, fund statements, acknowledgments, reconciliations, audit packages and regulator-ready human-readable reports with source/policy/template provenance.
- **UNB-ACC-026:** Strata integration SHALL preserve immutable posting, correction links, temporal queries, tenant isolation, replay-safe projections and integrity checks; gaps in verified Strata functionality are blockers or require governed companion components.
- **UNB-ACC-027:** Partner segregation SHALL apply to every journal, account, dimension, bank connection, projection, statement, export and AI accounting context, supporting separate schemas, databases and dedicated infrastructure.
- **UNB-ACC-028:** Define controlled accounting imports/exports, partner ERP integrations, banking feeds and custodian feeds; external systems can be reconciliation sources without silently replacing Unbound's authoritative books.
- **UNB-ACC-029:** Implement deterministic posting rules and policy-versioned accounting treatments; AI may propose classification and journal drafts but cannot bypass posting, approval or reconciliation controls.
- **UNB-ACC-030:** Financial integrity invariants SHALL be continuously checkable: balanced journals, valid dimensions, nonduplicated external effects, reconciled control accounts, authorized allocations, preserved restrictions and no unauthorized negative available balance.
- **UNB-ACC-031:** Support tax-lot and cost-basis tracking where relevant, with donor tax acknowledgment values distinguished from sponsor accounting basis, proceeds and valuation.
- **UNB-ACC-032:** Model pledges, conditional gifts, restricted contributions, contingent liabilities and commitments separately from settled available assets, using applicable accounting policies.
- **UNB-ACC-033:** Support nonprofit accounting classifications and net assets with/without donor restrictions where applicable; avoid conflating donor-advised subfund designation with legally donor-restricted net assets.
- **UNB-ACC-034:** Support configurable fiscal years, reporting calendars, accrual schedules, scheduled allocations, recurring journals and authorized close automation with safe retries.
- **UNB-ACC-035:** Support fraud detection, unusual-journal review, duplicate invoice/payee detection, anomalous transfers, break aging, financial control attestations and independently reviewable AI signals.
- **UNB-ACC-036:** Establish performance and recoverability requirements for high-volume posting, year-end grant surges, long historical ledgers, tenant migration, backup/restore and deterministic projection rebuild.

## Accounting capability modules to specify
1. Core chart of accounts, dimensions, posting engine and immutable journal.
2. DAF fund subledger and donor advisory balance reporting.
3. Contribution and gift accounting, valuation and acknowledgment.
4. Investment, securities, custody, NAV and unitization.
5. Grants commitments, payable, disbursements and recovery.
6. Cash management, bank accounts, treasury and reconciliation.
7. Sponsor AP, AR, expenses, purchasing and operational finance.
8. Fees, billing, revenue and cost allocation.
9. Financial close, statements, regulatory reporting and audit.
10. Budgeting, forecasting, management analytics and AI-assisted exception review.
11. Interfund, inter-entity, successor-account and partner settlement.
12. Cross-currency and cross-jurisdiction support where applicable.

## Essential accounting scenarios
- Multiple contributions settle at different times; pledged/received/available balances remain distinct.
- A security is gifted, valued, sold and reconciled; gains and fees post correctly without changing original gift evidence.
- Two advisors recommend grants simultaneously; sponsor authorization and fund availability prevent duplicate or excess payment.
- A grant is approved, payment returned, then reissued to corrected payee; immutable entries and payment controls reconcile.
- A fund splits 50/30/20 into three successor DAF accounts with open grants, restrictions and illiquid holdings; allocation is approved, balanced and replay-safe.
- A fee schedule changes midperiod; as-of reports preserve the previous policy and correction history.
- An invoice is duplicated or a vendor bank account changes; controls stop unsupported disbursement.
- A closed period receives late custody data; authorized adjusting entries preserve closed historical reports and restatements.
- Sponsor operating expense is incorrectly attributed to a DAF; control detects it and correction appends reversal/reclassification.
- A partner migrates from a shared schema to a dedicated database during active accounting activity; totals, event IDs, journals and audit evidence remain consistent.
- An agent attempts to post an unbalanced or unauthorized journal; deterministic validation denies it.
- An auditor reproduces a prior-period financial statement from immutable source and versioned reporting policy.
- FX conversion, consolidation and eliminations reconcile by legal entity and currency when enabled.
- A database outage and event replay cannot cause duplicate postings or external transfers.

## Research and policy blockers
- Confirm actual sponsor entity, accounting framework (e.g., applicable US nonprofit GAAP), tax and regulatory scope, reporting basis and jurisdiction.
- Determine whether Unbound is authoritative for sponsor GL, partner operating books, both, or a reference-mode demonstration for each.
- Confirm sponsor-controlled charitable assets, restricted fund treatment, unitization methodology and investment accounting policies.
- Verify Strata's actual persistence and transactional guarantees; do not invent accounting features.
- Determine whether payroll, tax filing and external AP payment execution are native modules or governed integrations; complete capability and interface requirements regardless.
- Determine reporting currency, consolidation scope, banking/custody partners and account minimums.
- Obtain accounting-professional review of chart of accounts, postings, financial statements and control design before approval.

## Traceability and implementation gate
Extends UNB-FIN-001/002, UNB-DATA-001..012, UNB-IMM-001..010, UNB-WF-018..020, UNB-SUCC-009, UNB-TEN-001..020, UNB-PLAT-001..009 and the API/Forma/Folio standards. All requirements are candidates pending accounting-policy and technology verification. No application code, compiled implementation, executed tests or accounting certification is claimed.

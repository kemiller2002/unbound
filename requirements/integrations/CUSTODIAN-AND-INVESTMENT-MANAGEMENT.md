# Custodian and investment management integration requirements
Date: 2026-11-21 | Status: owner-directed requirements; vendor capabilities, access and legal authority unverified

## Objective
Unbound SHALL support external investment custodians and investment managers through governed APIs and adapters. Charles Schwab is a reference integration target, not a claim of an available, public, approved or contracted Schwab API. Evaluate other custodians and advisors by capability, contract and authorization rather than assuming interchangeable endpoints.

## Institutional roles
- Sponsor: charitable legal owner/controller of DAF assets and final authority under program documents.
- Institutional partner: distribution/relationship/servicing channel, not necessarily custodian or sponsor.
- Custodian: institution holding accounts/assets and reporting transactions/positions.
- Investment manager/RIA: party with independently verified discretionary or advisory authority, potentially distinct from custodian.
- Donor/advisor: may express investment preferences/recommendations but cannot obtain sponsor-reserved trade or transfer authority by virtue of DAF participation.
- Payment/bank provider: may be distinct from securities custodian.

## Required capabilities
- **UNB-CUST-001:** Define a custodian-neutral adapter contract for account registration, identity mapping, account status, positions, cash, transactions, prices, tax lots where available, corporate actions, fees, transfers, statements, reconciliation and exceptions.
- **UNB-CUST-002:** Support multiple custodians per sponsor, partner, program and DAF, with multiple custody accounts per DAF and explicit legal ownership/account title, restrictions, account purpose and authorization.
- **UNB-CUST-003:** Support custody account opening and maintenance through documented provider-specific capabilities; where APIs are unavailable, define governed file/manual exception channels without claiming end-to-end automation.
- **UNB-CUST-004:** Define normalized investment account, instrument/security, position, lot, trade, settlement, cash movement, income, corporate action, price, valuation, fee and statement schemas, retaining raw source evidence and provider identifiers.
- **UNB-CUST-005:** Support read-only holdings and transaction feeds independently of trade execution. Never infer that a provider supporting balances also permits trading, transfer initiation or account opening.
- **UNB-CUST-006:** Define explicit investment authority: sponsor policy, investment committee, authorized manager, discretionary mandate, account-level permissions, trading limits, prohibited assets, concentration and liquidity controls.
- **UNB-CUST-007:** Trade recommendations, orders, approvals, submission, acceptance, execution, partial fill, cancellation, rejection and settlement SHALL be distinct states. Execution APIs require separately validated contractual permissions and human authority.
- **UNB-CUST-008:** Cash transfers, withdrawals, wires, ACH and securities transfers SHALL use independent authorization, payee/beneficiary verification, separation of duties, limits, reconciliation and provider-specific settlement evidence.
- **UNB-CUST-009:** Support scheduled polling, provider webhooks where available, batch files, secure statements and exception ingestion, with signed/verifiable origin, freshness, deduplication, ordering and replay protection.
- **UNB-CUST-010:** Reconcile custodian positions, transactions, cash, valuations and fees to Unbound's investment subledger, GL, bank/payment evidence and fund available-to-recommend calculation.
- **UNB-CUST-011:** Reconciliation exceptions SHALL capture missing trades, orphan deposits, stale prices, mismatched quantities, delayed settlement, rejected transfers, corporate actions, duplicate records and unresolved balances.
- **UNB-CUST-012:** External provider facts SHALL be appended with source, as-of time, received time, normalization version, hash and confidence/status. Corrections append superseding facts; no mutation of authoritative history.
- **UNB-CUST-013:** Apply Strata-backed append-only persistence requirements and tenant isolation to custody data, authorization, raw files, normalized events, ledger postings, outbox, credentials and audit evidence.
- **UNB-CUST-014:** Support advisor-managed accounts, model portfolios, sponsor-approved investment pools, managed portfolios and self-directed recommendations only where program policy and provider contracts permit.
- **UNB-CUST-015:** Model liquidity and unsettled proceeds separately from settled spendable cash; grant approval and payment gating must account for market movement, restrictions and settlement risk.
- **UNB-CUST-016:** Corporate actions, distributions, splits, reorganizations, fees, dividends, interest and realized/unrealized gains must have deterministic posting rules and independent reconciliation.
- **UNB-CUST-017:** Support provider-specific authentication, scoped credentials, key rotation, secure transport, rate limits, service accounts, incident response, data licensing and entitlement verification.
- **UNB-CUST-018:** Provide a partner- and sponsor-aware integration registry recording each provider's verified capability matrix, data freshness, connection status, authority, contract, sandbox, service-level objectives and operational owner.
- **UNB-CUST-019:** Partner isolation SHALL hold even when the same custodian serves multiple partners. Never use a provider account identifier alone as a global security boundary.
- **UNB-CUST-020:** Forma SHALL provide mobile-first accessible portfolio views, holdings, cash availability, trade/request states, discrepancies, investment mandates and approvals with clear source freshness.
- **UNB-CUST-021:** Folio SHALL produce custody reconciliation packs, fund investment statements, portfolio activity reports, manager oversight reports and audit-ready documents with immutable source/provenance references.
- **UNB-CUST-022:** Custodian outages, delayed feeds, revoked permissions, malformed files and account freezes SHALL trigger defined degraded modes, holds, freshness warnings and escalation without inventing balances or executing unsafe transfers.
- **UNB-CUST-023:** Custodian migrations and advisor changes SHALL preserve complete immutable history, account mappings, tax lots where available, open orders, restrictions, settlements and authorized successor-account allocations.
- **UNB-CUST-024:** Integrations SHALL support synthetic fixtures and provider sandboxes where available, contract tests, negative authorization tests, replay/idempotency tests, reconciliation tests and recovery drills before production authorization.
- **UNB-CUST-025:** Document contractual boundaries for asset custody, investment discretion, trade execution, valuation reliance, fiduciary oversight, privacy, incident liability, statement of record and record retention.
- **UNB-CUST-026:** Custodian adapter contracts SHALL be versioned APIs independent of vendor-specific implementation. Core fund accounting and grant workflows shall not require a particular custodian.
- **UNB-CUST-027:** Model investment performance, benchmarks, time-weighted/money-weighted returns and manager attribution with explicit calculation methods and source limitations; distinguish performance reports from accounting valuations.
- **UNB-CUST-028:** Support fund-level and sponsor-level exposure/portfolio aggregation only with authorized access and correct look-through/beneficial-accounting semantics; no unintended cross-partner leakage.

## Proposed provider-neutral API domains
1. CustodyConnection: register, authorize, rotate, suspend, test, revoke.
2. CustodyAccount: discover, map, open/request, verify legal title, close/request.
3. PositionAndPrice: positions, cash, lots, prices, source freshness and corporate actions.
4. ActivityIngestion: raw evidence, normalized events, deduplication and correction.
5. InvestmentMandate: policies, manager authority, approvals and restrictions.
6. OrderRequest: request, risk-check, approve, route, status and cancel; only if permitted.
7. CashAndAssetTransfer: propose, verify, authorize, submit, track, reconcile; only if permitted.
8. CustodyReconciliation: matching, breaks, exceptions, attestation.
9. InvestmentReporting: returns, exposure, fees, statements, provenance.

## Schwab reference integration discovery checklist
Before specifying actual Schwab endpoints, verify the **specific Schwab business channel** (retail brokerage vs institutional advisor/custody), eligibility of sponsoring charities and institutional partners, developer program/API access, contract permissions, read-only positions and transactions, historical data, account registration, trading, cash transfers, statement retrieval, webhooks vs polling, rate limits, authentication, test environment, and authorized account owner. Public Schwab developer offerings must not be assumed to cover institutional DAF custody or all capabilities above.

## Critical acceptance scenarios
1. Sponsor has two custody accounts at Schwab and another at a second custodian; DAF positions aggregate without double counting.
2. Custodian reports cash received but no matched contribution; create an orphan deposit exception rather than attributing it to a donor.
3. A donor requests a trade without delegated sponsor authority; request is recorded but no executable order is sent.
4. Approved investment manager submits an order; policy and account mandate independently enforce authorization, limits and settlement status.
5. Custodian feed is 48 hours stale; dashboard displays as-of timestamp and blocks unsupported available-cash claims.
6. Duplicate transactions and out-of-order corrections arrive; Strata append-only history remains complete and GL posts only the intended effects.
7. Corporate action changes security quantity; positions and valuations reconcile without creating fictitious gain.
8. Trade is partially filled then canceled; cash, holdings and open-order status remain consistent.
9. Payment transfer request is rejected by custodian; Unbound does not show settled funds or silently clear a payable.
10. Same custodian serves BofA and another partner; account IDs, tokens, raw files, logs and reports remain tenant-isolated.
11. DAF divides into successor accounts while holding illiquid securities and unsettled trades; unsupported allocations are held for sponsor decision.
12. A custodian goes offline during year-end grant surge; safe degraded operations and recovery reconcile without duplicate external instructions.

## Design blockers and follow-up
- Which Schwab institutional product and sponsoring charity arrangement is in scope?
- Which provider capabilities are actually accessible under contract and API entitlements?
- Who has investment discretion and authority to place trades or transfer cash?
- Which accounts are legally held by sponsor vs omnibus custodian accounts, and how are DAF subfund allocations represented?
- Which custodians and asset classes must be supported in first release?
- How are fees, performance reporting, pricing and investment-policy exceptions defined?
- How will provider records map to Strata-backed immutable events and the full accounting chart?

## Traceability
Extends UNB-ACC-001..036, ACC-MDL-001..012, UNB-AST-001..002, UNB-ADV-001..002, UNB-API-STD-001..006, UNB-TEN-001..020, UNB-SUCC-001..014 and UNB-PLAT-001..009. These are requirements, not evidence that Schwab or any other provider has granted API access, and no integration was implemented or tested.

# Canonical chart of accounts and accounting posting specification, v0.1
Date: 2026-11-21
Status: candidate requirements and illustrative postings; NOT accounting-policy approval, implementation or test evidence.

## 1. Accounting boundaries and invariants
**Accounting entity** is the legally accountable organization whose books are maintained. **Partner** is an institutional distribution/servicing tenant, not automatically the legal accounting entity. **DAF** is a sponsor-controlled internal fund or fund-account dimension, not an asset personally owned by its donor/advisors. **Operating fund** is the sponsor's operating activity. **Custodian/bank** is the external holder of assets, not necessarily the entity recording them.

Requirements:
- ACC-MDL-001: Define legal entity, partner, program, internal fund, account, currency, period, restriction, counterparty, asset/security, custody account, transaction source, event ID and posting-policy version independently. Every posted line has valid required dimensions.
- ACC-MDL-002: Authoritative journals are strictly append-only. A correction is a new reversal/reclassification/adjustment with explicit linkage, reason, evidence, authorization and bitemporal dates; no update/delete of posted entries.
- ACC-MDL-003: Every journal balances in its defined book, currency and accounting basis. Multicurrency postings must balance in transaction and functional currencies under an approved FX methodology; never pretend two unlike currencies can be summed.
- ACC-MDL-004: Distinguish cash/assets held, fund net asset balances, available-to-recommend amounts, grant commitments, legally restricted net assets, and operational reserves. Available-to-recommend is a controlled projection, **not a general-ledger account**.
- ACC-MDL-005: Enforce fund/account authorization and restrictions at posting and query boundaries, and tenant isolation across schemas, databases, agents, caches and exports.
- ACC-MDL-006: Every business event maps to zero or more authorized journals under a versioned posting policy. Some events (recommendations, invitations, plan approvals) are **nonposting**; posting requires a separate recognized accounting trigger.
- ACC-MDL-007: Posting is idempotent per event, accounting book and posting-rule version, with explicit treatment of restatements; event replay must not create duplicate journals or external effects.
- ACC-MDL-008: Separate financial statement classification from fund advisory designation and reporting dimensions. Accounting-policy review must determine donor restriction classification; DAF subfund designation does not automatically imply donor-restricted net assets.
- ACC-MDL-009: Control accounts reconcile to detailed subsidiary ledgers and independent bank/custody/payment evidence; unbalanced or unexplained breaks block unsupported close certifications.
- ACC-MDL-010: Closed periods remain immutable. Late discoveries are recognized through authorized subsequent-period adjustments and/or formally approved comparative restatement, with preserved originally issued statements.
- ACC-MDL-011: Use Strata in the approved persistence architecture; validate actual capabilities before mapping event store, journals, projections and integrity enforcement to Strata.
- ACC-MDL-012: APIs are the only governed application command surface; Forma handles accessible mobile-first accounting operations; Folio produces traceable, versioned financial and fund reports.

## 2. Chart of accounts structure
Codes below are **illustrative templates**, not an adopted nonprofit GAAP chart. Account codes SHALL be stable and independent of display names; hierarchical reporting groups may change by version without rewriting history.

| Class | Illustrative codes | Account families | Typical balance |
|---|---|---|---|
| 1 Assets | 1000-1999 | Cash and cash equivalents, custodian settlement receivables, contributions receivable where recognized, investments at approved measurement basis, accrued investment income, prepaid expenses, property/equipment | Debit |
| 2 Liabilities | 2000-2999 | Grants payable, trade/settlement payables, AP, accrued expenses, refundable amounts, deferred revenue where applicable, interentity liabilities | Credit |
| 3 Net assets / fund equity | 3000-3999 | Net assets without donor restrictions, net assets with donor restrictions where applicable, opening balances and approved internal fund transfer clearing | Credit |
| 4 Support / revenue / gains | 4000-4999 | Contribution support, investment income, realized/unrealized gains, fees earned, other support | Credit |
| 5 Program and grant expenses | 5000-5999 | Grants expense, direct program expense, approved program allocations | Debit |
| 6 Operating expenses | 6000-6999 | Personnel, professional services, technology, occupancy, fundraising, administration, depreciation | Debit |
| 7 Other / closing and clearing | 7000-7999 | Authorized clearing and reconciliation accounts, FX gains/losses where policy places them, year-end closing accounts | Policy-defined |

### Recommended initial accounts
- 1010 Operating bank cash; 1020 Sponsor charitable cash at bank; 1030 Custodial cash; 1040 Payment-in-transit clearing.
- 1100 Contributions receivable (recognition policy dependent); 1150 Investment income receivable; 1200 Marketable investments; 1250 Nonmarketable investments; 1300 Trade settlement receivable; 1400 Prepaid expenses.
- 2010 Grants payable; 2020 AP; 2030 Custody trade settlement payable; 2040 Accrued expenses; 2050 Refund/return liability; 2100 Due to/from other legal entities (paired subledger).
- 3010 Net assets without donor restrictions; 3020 Net assets with donor restrictions; 3090 Opening balance/control; 3900 Internal fund transfer clearing (must net to zero under policy).
- 4010 Contribution support; 4020 Investment income; 4030 Realized investment gains/losses; 4040 Unrealized valuation gains/losses; 4100 Sponsor fee revenue.
- 5010 Grant expense; 5020 Program expense; 6010 Salaries; 6020 Technology; 6030 Professional fees; 6040 General administration; 6100 Investment/custody fees.
The same nominal account may have fund and entity dimensions; do **not** automatically duplicate charts for every DAF. Distinct sponsor legal entities require separate accounting books. Accounts and dimensions must be versioned and approved.

## 3. Mandatory journal envelope
JournalId; AccountingEntityId; TenantId; BookId; SourceEventId; SourceEventVersion; PostingRuleId/Version; IdempotencyKey; EffectiveAt; RecordedAt; FiscalPeriod; Status; ApprovalDecisionId; CorrectionOfJournalId (optional); EvidenceRefs; Currency; FXRateSource/Version (if needed); Lines[{LineId, AccountId, Debit, Credit, Currency, FunctionalAmount, FundId?, ProgramId?, PartnerId?, RestrictionId?, AssetId?, CustodianId?, CounterpartyId?, Description}]; IntegrityCheckpoint.

Only **posted** journals are authoritative financial entries. Drafts and approvals are separate append-only business events. Reject negative line amounts, both debit and credit populated on the same line, invalid dimension combinations, unbalanced journals, invalid periods, missing approval or unauthorized posting. Account and dimension referential validity must be checked as of the transaction's policy-effective date.

## 4. Illustrative event-to-journal matrix
Entries assume US-dollar illustrative amounts and **one sponsoring charity** unless noted. These are examples for validation, not universal GAAP prescriptions. Exact timing, recognition and presentation require qualified nonprofit accounting review.

| Event and trigger | Debit | Credit | Conditions / controls |
|---|---|---|---|
| Cash contribution **received and accepted** $10,000 | 1020 Charitable cash $10,000 | 4010 Contribution support $10,000 | Verify receipt, acceptance, recognition basis and fund/restriction dimensions. A pledge alone does not necessarily post. |
| Securities gift accepted at recognized fair value $20,000 | 1200 Investments $20,000 | 4010 Contribution support $20,000 | Verify gift date, valuation, recognition and noncash evidence; tax acknowledgment valuation is separate. |
| Securities sale for $21,000 with carrying value $20,000 | 1030 Custodial cash $21,000 | 1200 Investments $20,000; 4030 Realized gain $1,000 | Simplified gross example; settlement timing, transaction fees, realized-gain classification and basis require policy. |
| Investment dividend received $120 | 1030 Custodial cash $120 | 4020 Investment income $120 | Reconcile custodian and tax lot/security records. |
| Period-end investment fair-value increase $500 | 1200 Investments $500 | 4040 Unrealized gain $500 | Only under approved measurement and valuation policy; reversal/correction uses new entry. |
| Grant recommendation submitted $2,000 | **No journal** | **No journal** | Advisory recommendation is not an accounting liability or payment authorization. |
| Sponsor-approved unconditional grant recognized $2,000 | 5010 Grant expense $2,000 | 2010 Grants payable $2,000 | Recognition depends on grant conditions and accounting policy; approval alone may not always trigger recognition. |
| Grant payment of recognized payable $2,000 | 2010 Grants payable $2,000 | 1020 Charitable cash $2,000 | Payment authorization, payee verification and external settlement evidence required. Pending bank settlement may use 1040 clearing. |
| Grant payment returned after settlement $2,000 | 1020 Charitable cash $2,000 | 2010 Grants payable $2,000 | If obligation remains; if canceled, separately reverse/adjust expense/payable under approved policy. Never erase original payment. |
| Sponsor fee assessed to a DAF $100, within same legal entity | Fund A: debit fee expense $100; sponsor operating fund: debit interfund receivable/clearing $100 | Fund A: credit interfund payable/clearing $100; sponsor operating fund: credit fee recovery/revenue $100 | **Illustrative fund-level mechanism only**. On consolidated sponsor books, internal charges may need elimination; do not count as third-party revenue without policy. Must specify transfer of cash and eliminations. |
| Sponsor vendor invoice accrued $300 | 6020 Technology expense $300 | 2020 AP $300 | Vendor validation, invoice matching, duplicate check, approver separation. |
| Vendor invoice paid $300 | 2020 AP $300 | 1010 Operating cash $300 | Payment gateway evidence, bank reconciliation and idempotency. |
| Cash reallocated from original DAF to successor DAF $5,000, same legal entity | **Fund-level** successor: 1030 Custodial cash/fund allocation $5,000 | **Fund-level** origin: 1030 Custodial cash/fund allocation $5,000 | This is a balanced **fund-dimension transfer**, not necessarily an entity-level GL change; preserve custody position and fund attribution, restriction carryover, approved split, no invented external cash movement. |
| Transfer between different sponsoring charities | To be determined under each legal entity's policies | To be determined under each legal entity's policies | Requires separate outgoing/incoming legal-entity transactions, custody and grant/transfer authority; cannot be modeled as simple internal fund split. |
| Reclassify a $250 expense from incorrect program to correct program | Correct program: debit expense $250 | Incorrect program: credit expense $250 | New authorized adjusting journal linked to original; never edit prior dimensions in place. |
| Prior journal was entirely incorrect | Reversal: swap original debits/credits | Reversal: swap original credits/debits | New reversal journal linked to original, then new corrected posting if warranted. Closed-period treatment governed separately. |

**Important modeling warning:** A single journal line must not represent a fund transfer by crediting a cash account without matching fund-specific debits and without a defined accounting-book and dimension-balancing policy. Implement fund allocation as balanced fund subledger entries or paired fund-dimension journals with a zero-net sponsor-wide effect. Similarly, interfund fee charges require explicit consolidated eliminations and approved sponsor accounting policy.

## 5. Financial subledger and posting-policy contracts
- Fund ledger: contribution basis, settled assets, unsettled trades, available cash, grant commitments, payable, fees, restrictions, donor-advised available balance, successor allocations and fund closure.
- Investment ledger: positions, tax lots where applicable, custody location, valuations, realized/unrealized results, corporate actions, income and cash.
- Grant ledger: recommendation (nonposting), sponsor decision, accounting recognition, payable, payment instruction, settlement, return, cancellation and recoveries.
- Sponsor operations: AP, AR, procurement, operating cash, revenue, expense, payroll interface, fixed assets if applicable, accruals and close.
- Control accounts: reconcile subsidiary ledgers to GL by entity, currency, fund and custody/bank account. Exceptions are append-only cases with assigned owners.
- Posting policies are versioned deterministic mappings from validated event + book + entity + accounting basis + policy date to either NoPosting(reason), DraftJournal, or Rejected(reason). Human authorization is independent of the mapping engine.

## 6. Accounting state and approval separation
Business lifecycle: Requested -> Verified -> SponsorDecision -> AccountingRecognition -> Posted -> ExternalSettlement -> Reconciled -> Closed/Reported, with branch-specific exceptions. This is **not** one universal linear workflow: some transactions have no external settlement; some settlement precedes final recognition. Each event must document prerequisites, actors, permitted transitions, compensations and evidence.

A donor/advisor cannot post, release, correct or approve accounting entries merely by making a recommendation. Agents can classify, draft, reconcile and flag within scoped APIs but cannot self-approve posting, payee changes or money movement. Define threshold-based human review and segregation-of-duties exceptions explicitly.

## 7. Reporting and controls
Required outputs: trial balance, GL detail, journal audit trail, DAF activity/balance statement, fund available-to-recommend statement, sponsor statement of financial position, statement of activities, cash flows, functional expenses as applicable, investment/custody positions, grants payable aging, bank/custodian reconciliation, restricted net asset roll-forward, fee reports, interfund transfers, Form 990 support schedules, close certification and auditor evidence packages. Folio must retain source journal/event high-water marks, policy and template versions, and delivery receipts. Forma must provide mobile-first accessible drill-down and approval.

## 8. Required validation cases
- Balance property: sum debits equals credits per book/currency and authorized dimensional balance rules.
- Nonposting recommendation: donor recommendation never creates a GL entry.
- Double-submit and replay: same event/idempotency key produces at most one posted journal per intended book/policy.
- Correction: original remains, reversal links and reconciles, as-of historical query reproduces previous report.
- Three-way reconciliation: GL, bank/custodian and payment rail agree or produce aged break.
- Multiple advisors and simultaneous grants cannot overspend available-to-recommend funds.
- 50/30/20 successor split, open grants, restricted assets and illiquid securities: preserve sponsor totals, restrictions and provenance; hold impossible allocations.
- Cross-tenant SQL, Forma/API, AI retrieval, exports and Folio documents do not leak partner data.
- Shared-schema to dedicated-database migration preserves all posted entries, account IDs, event sequence and independent evidence checkpoints.
- Financial close, late event, comparative restatement and document reissue preserve original published financial statements.
- Currency mismatches, invalid account dimensions, negative amounts, forged authorization and missing evidence are rejected.
- A payment instruction fails after journal posting; reconciliation identifies pending/failed settlement without silent journal deletion.

## 9. Decision register before final posting rules
ACC-ADR-001: Authoritative accounting framework and qualified nonprofit accountant review (US GAAP/other).
ACC-ADR-002: Sponsor/legal-entity topology and whether fund accounting is one GL with dimensions or balanced subledgers.
ACC-ADR-003: Contribution recognition timing, conditional promises, complex gifts and valuation policy.
ACC-ADR-004: Grant expense/payable recognition timing and conditional grants.
ACC-ADR-005: Investment valuation, units/NAV, lot basis and realized/unrealized reporting.
ACC-ADR-006: Internal fee recognition, cost recovery, interfund balancing and consolidated eliminations.
ACC-ADR-007: Restrictions vs internal DAF designation; available-to-recommend calculation.
ACC-ADR-008: Strata verified transaction, append-only, integrity and projection capabilities.
ACC-ADR-009: Multi-currency, FX, settlement and legal-entity consolidation.
ACC-ADR-010: Partner schema/database isolation, sponsor-wide consolidated reporting and access control.
ACC-ADR-011: Close, period locks, late adjustments and restatement policy.
ACC-ADR-012: Tax/reporting support and external accountant/auditor sign-off.

## 10. Next deliverables
1. Machine-readable chart-of-accounts and dimension dictionary, with types and validation constraints.
2. Machine-readable event-to-journal rules, with recognition gates and correction links.
3. Fund balance and available-to-recommend calculation specification.
4. Detailed sponsor fee, investment and successor-account accounting worked examples.
5. Financial close and reconciliation state machine, control owner matrix and acceptance fixtures.
6. Qualified accounting review, then API/Forma/Folio/Strata implementation mapping.

Traceability: UNB-ACC-001..036; UNB-DATA-001..012; UNB-TEN-001..020; UNB-SUCC-001..014; UNB-PLAT-001..009. No application code or tests were executed for this document.

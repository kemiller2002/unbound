# Full enterprise and DAF fund accounting requirements
Date: 2026-11-21 | Status: owner-directed mandatory requirements; detailed accounting policy and jurisdictional review pending

## Binding scope
Unbound SHALL include a **complete integrated accounting system**, not just a transaction log, grants subledger, or external GL connector. Accounting is a first-class enterprise capability with governed APIs, Forma interfaces, Folio reporting, and Strata integrated into append-only authoritative history. Do not assume Strata's actual accounting functionality until its APIs and guarantees are verified.

Distinguish (1) **DAF fund accounting** within a sponsoring charity's legally controlled assets, (2) **sponsor operating/general ledger accounting**, and (3) **partner/program reporting and settlement**, which may be informational rather than a separate legal entity's books. Legal entities, funds, donor advisory privileges, institutional partners, custodians, bank accounts and chart-of-accounts dimensions are not interchangeable.

## Core accounting requirements
- **UNB-ACC-001:** Provide a complete configurable, versioned chart of accounts with legal entity, sponsor, partner/program, DAF fund, department, project, restriction, asset, currency and other approved reporting dimensions; enforce dimension validity.
- **UNB-ACC-002:** All accounting postings SHALL be double-entry, balanced by defined accounting entity and currency/measurement rules, with immutable journal headers/lines, unique posting IDs, effective dates, recorded dates, authorizers and provenance.
> Adopted 2026-11-23: refined by `RR-UNB-ACC-002-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-ACC-003:** Posted journals SHALL never be updated or deleted. Corrections SHALL use linked reversal, adjustment or reclassification entries with approval and preserved original history.
- **UNB-ACC-004:** Maintain integrated general ledger and subsidiary ledgers for DAF balances, cash/banks, custody/securities, grants payable, accounts payable, accounts receivable, contributions, investments, fees, expenses, payroll interface, fixed assets where relevant and inter-entity balances.
- **UNB-ACC-005:** Separate donor-advised fund accounting from sponsor operating activity and enforce restrictions, liquidity and sponsor ownership/control; DAF subfund balances are not personally owned donor assets.
- **UNB-ACC-006:** Record contribution lifecycle from intent through receipt, valuation, restrictions, acceptance, acknowledgment, settlement and disposition, with distinct cash, securities and complex noncash treatments.
> Adopted 2026-11-23: refined by `RR-UNB-ACC-006-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-ACC-007:** Record grant lifecycle from recommendation through sponsor approval, commitment, payable, payment, clearing, returns, voids and recoveries; no recommendation alone creates a payment.
- **UNB-ACC-008:** Support cash, accrual and required reporting bases through explicit approved policies and reconciled adjustments; do not assume a single basis applies to every entity/report.
- **UNB-ACC-009:** Support investments, units/NAV, gains/losses, interest, dividends, corporate actions, custody reconciliation, valuations, impairments and liquidity, subject to documented accounting policy.
- **UNB-ACC-010:** Support sponsor and partner fee schedules, accruals, waivers, allocations, revenue recognition, billing, collections and intercompany/interfund settlement without double counting.
- **UNB-ACC-011:** Support internal fund allocations, fund mergers, closures, outbound transfers, successor-account splits and multigenerational successor transfers with balanced immutable entries, restrictions and provenance.
- **UNB-ACC-012:** Support accounts payable, purchasing/expense approvals, vendor master controls, reimbursements, accruals, prepaid expenses and payment reconciliation for sponsor operations.
> Adopted 2026-11-23: superseded by `RR-UNB-ACC-012-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-ACC-013:** Support accounts receivable, invoicing, credit adjustments, collections and deferred revenue where applicable.
- **UNB-ACC-014:** Support budgeting, forecasts, actual-versus-budget reporting, cost centers, allocation methodologies and scenario models while distinguishing forecasts from posted actuals.
- **UNB-ACC-015:** Support multiple legal entities and sponsor structures, intercompany/interfund transactions, eliminations and consolidated reporting where authorized and legally applicable.
- **UNB-ACC-016:** Support currencies, FX rates, source currency, functional/reporting currency, remeasurement, translation and realized/unrealized FX results where needed; currency conversion never silently changes source amounts.
- **UNB-ACC-017:** Define accounting periods, soft/hard close, reconciliations, close checklists, adjusting entries, period locks, reopen approvals and late-arriving event treatment without modifying posted journals.
- **UNB-ACC-018:** Reconcile GL and subledgers to independent bank, custodian, payment-rail, investment and partner statements, with aged breaks, ownership, evidence and escalation.
> Adopted 2026-11-23: refined by `RR-UNB-ACC-018-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-ACC-019:** Provide financial statements including trial balance, balance sheet/statement of financial position, statement of activities, cash flows, functional expenses where required, fund statements, restricted/unrestricted analyses and management reporting, subject to entity-specific reporting rules.
- **UNB-ACC-020:** Support audit schedules, roll-forwards, supporting workpapers, transaction drill-down, external auditor access, evidence export and independently verifiable as-of financial snapshots.
- **UNB-ACC-021:** Support tax and regulatory reporting source data, including applicable Form 990/DAF disclosures and other filings, with jurisdictional applicability and human sign-off; do not assume automatic filing authority.
- **UNB-ACC-022:** Enforce segregation of duties for journal preparation, approval, posting, bank/payee changes, reconciliations, close certification, master data and payment release; AI cannot approve its own accounting actions.
- **UNB-ACC-023:** Every accounting operation SHALL have an authenticated, versioned API with authorization, idempotency, concurrency, error states, traceability and evidence; external callers cannot directly post unrestricted journals or instruct money movement.
- **UNB-ACC-024:** Forma SHALL provide accessible mobile-first accounting experiences, including approval, exceptions, fund statements, reconciliation review, close dashboards and audit drill-down; complex tasks must remain possible on a phone.
- **UNB-ACC-025:** Folio SHALL generate accessible versioned financial statements, fund statements, acknowledgments, reconciliations, audit packages and regulator-ready human-readable reports with source/policy/template provenance.
- **UNB-ACC-026:** Strata integration SHALL preserve immutable posting, correction links, temporal queries, tenant isolation, replay-safe projections and integrity checks; gaps in verified Strata functionality are blockers or require governed companion components.
> Adopted 2026-11-23: superseded by `RR-UNB-ACC-026-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
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

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-ACC-006-r1 (refines `UNB-ACC-006`) — adopted-provisional
- **Original:** `requirements/FULL-ACCOUNTING-SYSTEM.md:15` @ `777a70d` (kept verbatim) · **Decisions:** RCD-019, RCD-032, RCD-033, RCD-034, RCD-035, RCD-036, RCD-037, RCD-038, RCD-046
- **Requirement:** Unbound MUST record each contribution as distinct, timestamped facts: Intent, Received (by rail), CompletedGiftDetermined, SponsorAccepted, Valued (separate typed values: sponsor book value, custody receipt value, optional informational acknowledgment value, sale proceeds), Acknowledged, Settled/Final, and Disposed. Acknowledged MUST NOT occur before CompletedGiftDetermined and SponsorAccepted. Grantable capacity MUST NOT increase before Final (RCD-019, RCD-033; provisional credit is not offered in release 1).
- **Validation still required:** RCD-019: CPA on completed-gift rule table; RCD-034: Counsel confirmation per intermediary agreement; RCD-035: Investment committee delegation; RCD-036: Sponsor CFO; RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an ACH contribution received but within the return window and no provisional policy; when IssueAcknowledgment is called; then the command is rejected with AcknowledgmentNotPermitted(NotFinal) unless sponsor policy defines ACH completed-gift at receipt, and no document is produced.
  - AC-2: Given a sponsor template lacking the exclusive-legal-control element; when IssueAcknowledgment runs; then issuance is blocked and an exception case is opened.
  - AC-3: Given an acknowledged ACH contribution that returns R10; when the return is recorded; then a ReturnedContributionExposure fact and an acknowledgment correction task are created and the original acknowledgment remains retrievable.
  - AC-4: Given Donor declares 1,000 shares; custodian delivers 600; when the feed is processed; then contribution PartiallyReceived; 600 shares Received; no acknowledgment issued; fund grantable balance unchanged until acceptance and settlement policy satisfied.
  - AC-5: Given The same custodian receipt message delivered twice; when both are processed; then one Received event; second recorded as duplicate; one journal entry.
  - AC-6: Given A cash contribution of $300; when acknowledgment is issued; then document states amount, 'no goods or services were provided' (or estimate), and the exclusive-legal-control statement.
  - AC-7: Given Section B property received on 2026-03-01 and sold 2027-02-01; when the sale settles; then a Form8282Due obligation exists with due date 2027-06-06 and donor-copy task.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ACC-006-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-ACC-002-r1 (refines `UNB-ACC-002`) — adopted-provisional
- **Original:** `requirements/FULL-ACCOUNTING-SYSTEM.md:11` @ `777a70d` (kept verbatim) · **Decisions:** RCD-020
- **Requirement:** Every posted journal MUST balance (sum of debits = sum of credits, in exact decimal) separately for each combination of accounting entity, transaction currency and every balancing segment declared in the posting policy; fund and restriction MUST be declared balancing segments for DAF books. A journal failing any check MUST be rejected, not posted.
- **Validation still required:** RCD-020: Controller
- **Acceptance criteria:**
  - AC-1: Given a journal Dr 1030 fund S 5000 / Cr 1030 fund O 5000; when PostJournal is called; then it is rejected as Unbalanced for groups (fund S, +5000) and (fund O, -5000).
  - AC-2: Given a transfer with Dr 3900 O / Cr 1030 O and Dr 1030 S / Cr 3900 S, 5000 each; when PostJournal is called; then it posts and 3900 nets to zero.
  - AC-3: Given a USD/EUR journal that nets to zero numerically across currencies; when PostJournal is called; then it is rejected per currency group.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ACC-002-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-ACC-026-r1 (supersedes `UNB-ACC-026`) — adopted-provisional
- **Original:** `requirements/FULL-ACCOUNTING-SYSTEM.md:35` @ `777a70d` (kept verbatim) · **Decisions:** RCD-027
- **Requirement:** The ledger component MUST provide immutable posting, correction links, bitemporal (effective and recorded) queries, tenant isolation, replay-safe projections and integrity checkpoints on the approved PostgreSQL persistence. Strata MAY be used only for declarative schema management of these stores, and its deployment MUST refuse destructive changes (drop, truncate, type narrowing, removal of append-only triggers or privilege revocations) to ledger tables. No requirement may claim these guarantees come from Strata.
- **Validation still required:** RCD-027: Counsel on EF licence terms (commercial-launch blocker, not a design blocker)
- **Acceptance criteria:**
  - AC-1: Given a runtime role; when it issues UPDATE on a posted journal row; then the database rejects it and the row digest is unchanged.
  - AC-2: Given a schema change dropping the journal append trigger; when deployment runs; then the deploy is refused as destructive.
  - AC-3: Given a backdated correction recorded today; when the ledger is queried as known at yesterday; then the correction is absent.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ACC-026-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-ACC-012-r1 (supersedes `UNB-ACC-012`) — adopted
- **Original:** `requirements/FULL-ACCOUNTING-SYSTEM.md:21` @ `777a70d` (kept verbatim) · **Decisions:** RCD-013
- **Requirement:** Unbound MUST NOT natively implement sponsor operating accounts payable, purchasing, vendor master, reimbursements or prepaid accounting in the initial scope. It MUST export balanced, summarized, dimension-complete journals for DAF program activity to the sponsor's system of record for operating finance, and reconcile the receiving control accounts. A native AP module MAY be added only by a recorded owner decision with a demonstrated outcome need.
- **Acceptance criteria:**
  - AC-1: Given a closed period with an unmapped account; when export runs; then export is blocked with UnmappedAccount.
  - AC-2: Given an export retried after timeout; when the same close version is exported again; then the ERP receives one logical export (same key) and totals tie once.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ACC-012-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-ACC-018-r1 (refines `UNB-ACC-018`) — adopted-provisional
- **Original:** `requirements/FULL-ACCOUNTING-SYSTEM.md:27` @ `777a70d` (kept verbatim) · **Decisions:** RCD-020
- **Requirement:** Reconciliation differences MUST be classified against three versioned thresholds per reconciliation class and scope: RoundingTolerance (auto-clear with evidence), Materiality (blocks close certification, NAV publication and new reservations for the affected scope only), and an AgingLimit for unexplained immaterial differences (escalate on expiry regardless of amount). Loosening any threshold MUST require a second approver.
- **Validation still required:** RCD-020: Controller
- **Acceptance criteria:**
  - AC-1: Given a $0.37 explained rounding difference under a $1 tolerance; when reconciliation runs; then it auto-clears with evidence.
  - AC-2: Given a $0.37 unexplained difference older than the aging limit; when the aging check runs; then an escalation case opens though the close is not blocked.
  - AC-3: Given a $25,000 custody cash difference above materiality; when close certification is attempted; then certification is denied for the affected sponsor scope.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-ACC-018-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-009-r1 (refines `REQ-X-009`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:105` @ `777a70d` (kept verbatim) · **Decisions:** RCD-019
- **Requirement:** When a contribution that contributed to capacity is returned or reversed, Unbound MUST record a ReturnedContributionExposure fact linked to the original receipt, reduce capacity immediately, preserve all downstream disbursements, and post entries selected by the CPA-approved returned-contribution policy; it MUST NOT presume a donor receivable unless counsel has confirmed an enforceable claim for that rail and case.
- **Validation still required:** RCD-019: CPA on completed-gift rule table
- **Acceptance criteria:**
  - AC-1: Given a $1,000 ACH funded a reserved $800 grant then returned; when the return is recorded; then capacity becomes -$800 equivalent exposure, new reservations block, the grant payment record is preserved and a case opens.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-009-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-FIN-001 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-001 · **Parent:** UNB-ACC-006 · **Decisions:** RCD-009, RCD-019
- **Requirement:** Unbound MUST issue DAF contribution acknowledgments only through a gated IssueAcknowledgment command that validates a counsel-approved, versioned sponsor template containing every element counsel designates as mandatory (including the statement that the sponsoring organization has exclusive legal control over the contributed assets, the goods-or-services statement, cash amount or noncash description, donor and sponsor identity, and contribution date), and MUST block issuance when any element is missing.
- **Validation still required:** RCD-009: DG-04 spike: byte-identical re-render and throughput; RCD-019: CPA on completed-gift rule table
- **Acceptance criteria:**
  - AC-1: Given a template without the exclusive-legal-control element; when IssueAcknowledgment runs; then issuance is blocked and a case opens.
  - AC-2: Given a valid template and accepted completed gift; when IssueAcknowledgment runs twice with the same key; then exactly one acknowledgment exists.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-006 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-010 · **Parent:** ACC-MDL-003 · **Decisions:** RCD-020
- **Requirement:** Posting policies MUST declare balancing segments; for DAF books, fund and restriction MUST be balancing segments, and internal fund transfers MUST use net-asset transfer or internal-transfer clearing lines that net to zero sponsor-wide per currency.
- **Validation still required:** RCD-020: Controller
- **Acceptance criteria:**
  - AC-1: Given a cross-fund cash-only journal; when it is posted; then it is rejected per fund group.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-006); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-015 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-036 · **Parent:** UNB-ACC-006 · **Decisions:** RCD-030
- **Requirement:** Donor-facing annual summaries and AI assistance MUST use counsel-approved, effective-dated tax disclosures and MUST NOT state or imply that DAF contributions qualify for the nonitemizer charitable deduction or any specific deductible amount.
- **Validation still required:** RCD-030: Counsel/CPA approve disclosure text
- **Acceptance criteria:**
  - AC-1: Given a donor asks the assistant whether a DAF gift qualifies for the nonitemizer deduction; when the assistant responds; then it states only the counsel-approved disclosure and refers to a tax adviser.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-015); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ARC-001 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ARC-003 · **Parent:** UNB-API-STD-* · **Decisions:** —
- **Requirement:** Every external and inter-component command, query and event contract MUST be defined once in F# types from which OpenAPI 3.1 (HTTP) and JSON Schema (events) are generated; generated schemas MUST be versioned and checked for breaking changes in CI; the AI-assistance contract for each operation (see RR-REQ-X-051-r1 or its successor) MUST be generated from the same source.
- **Acceptance criteria:**
  - AC-1: Given A field is removed from a published event schema; when CI runs; then the build fails with a breaking-change report unless a new major version is introduced.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ARC-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

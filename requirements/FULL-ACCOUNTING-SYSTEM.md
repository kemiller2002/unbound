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
  - *Refined 2026-11-24 by RR-UNB-ACC-014-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-ECO-005, decided-provisional):* Support budgeting, forecasts, actual-versus-budget reporting, cost centers and scenario models while distinguishing forecasts from posted actuals. Management cost allocation and activity-based attribution SHALL be computed as a non-posting projection (UNB-XIN-ECO-001) that never creates journals or changes fund balances or available-to-grant; only CPA-approved functional expense allocations (UNB-ACC-019) and approved fee/billing entries (UNB-ACC-010) may post. Every management allocation SHALL reconcile monthly to GL actuals with an explicit unallocated residual (UNB-XIN-ECO-004). Functional expense allocation and management attribution share one activity dataset with separately governed mappings (XID-ECO-005). The text above is retained as originally written.
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
  - *Refined 2026-11-24 by RR-UNB-ACC-033-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-INV-002, decided-provisional):* Each program document version and each fund-intake agreement (including legacy imports and sponsor-to-sponsor transfers-in) MUST record a CPA-reviewed VariancePowerEvidence (clause reference, effective date) or its absence. Net asset classification of contributions MUST be derived from it: with variance power and no donor-imposed purpose/time restriction -> without donor restrictions (DAF designation shown as an internal designation); with an explicit donor restriction -> with donor restrictions; without variance power and with a specified beneficiary -> evaluated under UNB-XIN-ACC-001 (agency/pass-through). A contribution whose agreement lacks recorded evidence MUST post to a suspense classification and open an exception before period close. Advisory privileges alone MUST NOT be treated as a donor restriction (XID-INV-002). The text above is retained as originally written.
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

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-ACC-001:** Every account type in the account-type register MUST carry a CPA-approved AccountingTreatment of exactly one of {Contribution-WithoutDonorRestriction, Contribution-WithDonorRestriction, AgencyLiability, PassThroughLiability}. Receipts into an AgencyLiability or PassThroughLiability account MUST post to a liability (funds held for others) and MUST NOT increase contribution revenue, DAF counts or Schedule D Part I aggregates; payouts from such accounts MUST reduce the liability and MUST NOT be recorded as grant expense. An account type without an approved AccountingTreatment MUST NOT accept receipts. Default treatments are fixed by XID-INV-002: a donor-advised account type whose program version records variance power maps to Contribution-WithoutDonorRestriction; an account type where a nonprofit transfers assets naming itself beneficiary without granting variance power maps to AgencyLiability (designated/agency fund, not a DAF, per XID-CRM-004); Contribution-WithDonorRestriction applies only where a gift instrument imposes a time or purpose restriction beyond advisory privileges.
  - Provenance: [UNB-XIN-ACC-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-INV-002, XID-CRM-004 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given an account type 'Agency endowment' with AccountingTreatment AgencyLiability; when a $50,000 receipt is accepted; then the posted journal credits a funds-held-for-others liability, contribution revenue is unchanged and the Schedule D package excludes the receipt.
    - AC-2: Given an account type with no AccountingTreatment; when a receipt is attempted; then the command is rejected with reason MissingAccountingTreatment and no journal posts.
    - AC-3: Given an AgencyLiability account paying $10,000 to its beneficiary nonprofit; when payment settles; then the liability decreases by $10,000 and no grant-expense line exists.
    - AC-D1: Given a charity deposits $50,000 naming itself as sole beneficiary with no variance-power clause; when the receipt posts; then a liability of $50,000 is recorded, contribution revenue and Schedule D DAF aggregates are unchanged, and the account is classified designated/agency, not DAF.
- **UNB-XIN-ACC-002:** Each sponsor book MUST declare one GrantRecognitionPoint from {ApprovalCommunicatedToGrantee (default), PaymentReleased}, effective-dated and CPA-approved; PaymentReleased MAY be declared only where payment is the first communication of the grant to the grantee. The platform MUST send the approval notice to the grantee automatically on sponsor approval, and the posting rule MUST recognize grant expense and grants payable for an unconditional grant only when the communication record (timestamp, channel, recipient) or, for PaymentReleased, the payment release ID exists. Sponsor approval without communication MUST NOT be recognized (an intention, not a promise to give). A grant whose terms contain both a barrier and a right of return or release, or whose terms are ambiguous, MUST be recorded as ConditionalGrant with no expense until a recorded BarrierMet or Waived event. At each period end, Unbound MUST produce a cutoff register listing grants approved but not communicated, communicated but not paid, and conditional, with their recognition status.
  - Provenance: [UNB-XIN-ACC-002](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ADV-006, XID-INV-001 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given GrantRecognitionPoint=ApprovalCommunicatedToGrantee and a grant approved Dec 30 whose award notice is sent Jan 2; when December close runs; then no grant expense posts in December and the cutoff register lists the grant as approved-not-recognized.
    - AC-2: Given GrantRecognitionPoint=SponsorApproval and a $2,000 grant approved Dec 31 and paid Jan 3; when both periods close; then December holds Dr grant expense/Cr grants payable $2,000 and January holds Dr grants payable/Cr cash $2,000.
    - AC-3: Given a grant with a matching-funds barrier and a release clause; when the grant is approved; then state is ConditionalGrant, no journal posts, and capacity is reserved per policy.
    - AC-4: Given a ConditionalGrant; when BarrierMet is recorded with evidence; then expense and payable post effective at the BarrierMet date.
    - AC-D1: Given a grant approved on 30 December whose approval notice fails to send until 2 January, under ApprovalCommunicatedToGrantee; when the December period closes; then no grant expense is recognized in December and the grant appears in the cutoff register as approved-not-communicated.
    - AC-D2: Given a sponsor configuration attempt to declare SponsorApproval; when the book policy is saved; then the save is rejected with RECOGNITION_POINT_NOT_PERMITTED (XID-ADV-006).
- **UNB-XIN-ACC-005:** Each fiscal year MUST have an AuditAdjustment period after period 12 that accepts only journals linked to an AuditFinding (auditor reference, proposer, controller decision). Each auditor-proposed adjustment MUST be recorded as Booked or Passed with amount and rationale; Passed items MUST appear in a summary-of-unadjusted-differences report. Audited financial statements MUST be generated from period 12 plus AuditAdjustment journals, and fund statements already issued from period 12 MUST NOT be reissued unless a Booked adjustment changes a fund balance above the sponsor's restatement threshold, in which case a correction notice per REQ-X-040 MUST be produced. The AuditAdjustment period remains soft-closed until the audited statements are issued; after issuance it is hard-closed and late items follow UNB-XIN-EVA-002 (XID-ADV-005).
  - Provenance: [UNB-XIN-ACC-005](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ADV-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given December closed and fund statements issued; when the auditor proposes a $30,000 accrued-fee adjustment that is booked; then the journal posts to the AuditAdjustment period and the audited trial balance differs from the period-12 balance by $30,000.
    - AC-2: Given a $4,000 proposed adjustment; when the controller passes it; then it appears on the unadjusted-differences report and no journal posts.
    - AC-3: Given a journal without an AuditFinding link; when posting to AuditAdjustment is attempted; then it is rejected.
    - AC-D1: Given the audited statements were issued yesterday; when an AuditFinding-linked journal is submitted to the AuditAdjustment period; then it is rejected and redirected to the PriorPeriodAdjustment route.
- **UNB-XIN-ACC-006:** For each sponsor and fiscal year Unbound MUST produce a DAF bridge that reconciles (a) Schedule D Part I line 4 aggregate value, (b) the sum of year-end fund statement values, and (c) the DAF component of net assets in the audited statements, and similarly line 2 contributions and line 3 grants to the statement of activities, using only typed reconciling items: InternalTransfersAmongFunds, GrantsApprovedUnpaid, AccruedFeesUnpaid, UnsettledTrades, AgencyOrPassThroughExcluded, ValuationBasisDifference, TimingOfLateItems, Other(requires note). Any unexplained residual MUST block sign-off of the package. Line 4 MUST use the same fair-value measurement as the audited statements; line 3 MUST include all distributions from DAFs including transfers to other funds of the organization, on the grant recognition basis of UNB-XIN-ACC-002 (XID-INV-005).
  - Provenance: [UNB-XIN-ACC-006](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-INV-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given $1.0m of DAF-to-DAF internal transfers in the year; when the bridge runs; then line 3 exceeds GAAP grant expense by $1.0m, explained by InternalTransfersAmongFunds.
    - AC-2: Given $250,000 grants approved Dec 30 unpaid; when the bridge runs; then fund statement values exceed GAAP DAF net assets by $250,000 shown as GrantsApprovedUnpaid (when recognition is at approval).
    - AC-3: Given an unexplained $5,000 residual; when package sign-off is requested; then sign-off is blocked.
    - AC-D1: Given a $10,000 transfer from a DAF to another fund of the same organization during the year; when the Schedule D package is built; then line 3 includes the $10,000 and the bridge shows it as InternalTransfersAmongFunds.
- **UNB-XIN-ACC-008:** At acceptance, every noncash asset MUST be classified UBTIExposure in {None, SCorpStock, PassThroughWithUBTI, DebtFinanced, Unknown}; SCorpStock acceptance MUST require a counsel-approved rule reference; for any exposure other than None, the donor's carryover basis and holding-period evidence MUST be recorded. Unbound MUST accrue estimated UBIT quarterly in the sponsor's books in the period the unrelated business taxable income is earned (ASC 740), MUST allocate the estimate to the fund(s) that held the income-producing asset in proportion to attributed UBTI, MUST reduce those funds' capacity by a recorded UBIT hold equal to the allocated estimate before any grant reservation, and MUST true up the accrual, allocation and hold when Form 990-T is filed.
  - Provenance: [UNB-XIN-ACC-008](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-INV-006 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given S-corp stock offered without a counsel rule; when acceptance is attempted; then acceptance is blocked with reason SCorpRuleMissing.
    - AC-2: Given accepted S-corp shares with carryover basis $100,000 sold for $900,000; when the tax-year UBIT estimate runs; then the estimate uses $800,000 gain and the fund's capacity is reduced by the estimated tax.
    - AC-3: Given UBTIExposure=Unknown; when fund capacity is computed; then a configured haircut is applied and an exception is opened.
    - AC-D1: Given a fund holding S-corporation stock with $20,000 of attributed UBTI in Q2; when the Q2 close runs; then an estimated UBIT accrual is posted in Q2, allocated wholly to that fund, and its capacity is reduced by the estimate.
    - AC-D2: Given the filed Form 990-T tax is $1,200 lower than the accrued estimate; when the true-up is recorded; then the accrual, the fund allocation and the UBIT hold are each reduced by $1,200 with links to the filing.
- **UNB-XIN-ERP-001:** The fee engine MUST compute every fund-level fee daily on the daily balance of the declared FeeBase, using actual/365 (actual/366 in leap years) unless the schedule version declares otherwise, and MUST post the computed fee once per assessment period declared in the schedule (monthly or quarterly), not daily. The computation MUST segment at each of: fund activation, schedule-version effective date, program transfer, successor split or merge, closure trigger and migration cutover; each segment uses the schedule version and fee base in effect for that segment. A minimum fee MUST be prorated by the same day count over the days the fund was billable. Grantable capacity MUST subtract fees computed but not yet posted. The assessed amount MUST NOT exceed the fund's available balance net of reservations and approved-unpaid grants at assessment; any shortfall MUST be recorded as a waived amount with reason MinimumShortfall and MUST NOT create a receivable from any donor or participant. Recomputing any assessed fee from stored inputs MUST reproduce it exactly. The daily computation MUST run in chunks so that no transaction locks more than 5,000 fund rows, and concurrent grant approvals MUST stay within their latency SLO while it runs (benchmark report 18: a single-transaction batch stalled approvals up to 347 ms at 100,000 funds and about 12 s at 1.42 million).
  - Provenance: [UNB-XIN-ERP-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ERP-001 (decided-provisional; confirmations in report 16).
  - Revision proposed 2026-11-24 (QR-0090): the text above is kept verbatim; the proposed revised statement is in `requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl` (`revisions` of UNB-XIN-ERP-001). Reason: Do not prorate a minimum fee by default without contract and economics; propose no extra closing minimum unless disclosed and authorized.
  - Acceptance:
    - AC-1: Given A fund opened on day 46 of a 92-day quarter under a schedule with a $100 annual minimum and actual/365; when the quarterly fee is assessed; then the minimum component equals 100 x 47/365 and the accrual covers only days 46-92.
    - AC-2: Given A fund with $40 available and a $25 prorated minimum plus $30 of reservations; when the fee is assessed; then assessed = $10, waived $15 with reason MinimumShortfall, no receivable exists.
    - AC-3: Given A schedule change effective mid-quarter; when the quarter is assessed; then two segments are stored, each citing its schedule version, and their sum equals the assessed fee.
    - AC-4: Given A migrated fund already charged a full quarter by the incumbent; when the cutover segment is assessed; then the cutover-declared prior-billed days are excluded and no day is billed twice.
    - AC-D1: Given a fund under a quarterly schedule; when each day of the quarter passes; then a daily fee computation is stored but no journal posts until the quarter's assessment, which posts once for the quarter's total.
    - AC-D2: Given a fund with $1,000 of fees computed but not yet posted and $50,000 journal balance with no reservations; when an advisor views available-to-grant; then it shows $49,000.
    - AC-D3: Given the daily fee computation for 100,000 funds running while grant approvals arrive at 200 per second; when the computation executes; then no transaction locks more than 5,000 fund rows and approval p99 latency stays within the approval SLO.
- **UNB-XIN-ERP-002:** Every fee schedule MUST declare exactly one payer source from {FundAssets, DonorExternal, PartnerInvoice, SponsorAbsorbed}. FundAssets fees MUST post only as fund-level fee entries under an approved schedule. DonorExternal fees MUST be recorded outside every fund ledger as a payment for services, MUST NOT be accepted through the contribution workflow, MUST NOT appear on any contribution acknowledgment, and MUST NOT be offset against a fund balance. PartnerInvoice fees MUST create an accounts-receivable invoice to the partner legal entity. A fee schedule with a missing or ambiguous payer source MUST NOT activate. DonorExternal fees are payments for services, receipted as service fees and booked as program service revenue outside fund ledgers (XID-ERP-002).
  - Provenance: [UNB-XIN-ERP-002](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ERP-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given A DonorExternal $5 monthly membership schedule; when the donor pays by card; then no contribution record, no acknowledgment and no fund entry are created; a service-fee receipt is issued.
    - AC-2: Given A PartnerInvoice platform fee; when the period closes; then an invoice to the partner legal entity is created and no fund balance changes.
    - AC-3: Given A schedule without payer source; when activation is attempted; then activation fails with PAYER_SOURCE_REQUIRED.
    - AC-D1: Given a donor pays a $480 annual membership; when the receipt is issued; then it is labelled a service fee with no charitable contribution statement.
- **UNB-XIN-EVA-001:** For each sponsor legal entity and for every function for which Unbound is designated book of record (UNB-XIN-ACC-010), fund balances, reservations, capacity inputs and every financial report figure MUST be computed only from the insert-only journal and reservation tables, less fees computed but not yet posted (XID-ERP-001). Every domain fact with a declared financial effect MUST carry the JournalBatchId (or ReservationId) committed in the same database transaction, and the system MUST NOT maintain any other log from which balances can be derived or treated as a source on replay; re-derivation under UNB-XIN-ACC-003 is a comparison whose output never posts. Posting rules MUST be applied once at posting time and MUST NOT be re-applied on replay. Where an external system is book of record (Shadow mode), capacity MUST be computed under UNB-XIN-ADV-002.
  - Provenance: [UNB-XIN-EVA-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ERP-001, XID-ADV-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given A fund with 3 contributions, 2 grants and 1 return posted; when the balance projection is rebuilt from the journal alone; then the rebuilt balance equals the displayed balance and no other table is read.
    - AC-2: Given A PaymentSettled fact whose declared financial effect is 'settlement posting'; when the integrity check runs; then the fact's JournalBatchId exists, was committed in the same transaction and is balanced per entity x currency x balancing segment.
    - AC-3: Given A posting rule version change after go-live; when domain facts are replayed for projection rebuild; then no new journal batches are created and historical batches are unchanged.
    - AC-D1: Given a sponsor whose general ledger is Shadow (external book of record); when a grant reservation is requested; then capacity comes from the reconciled external balance at the latest watermark minus reservations (UNB-XIN-ADV-002), not from Unbound journals.
- **UNB-XIN-EVA-002:** Every posting command MUST read the accounting period row for its posting date with a shared lock (or run SERIALIZABLE) in the posting transaction; HardClosePeriod MUST take an exclusive lock on that row. An ordinary posting whose posting date falls in a hard-closed period MUST be rejected with PERIOD_CLOSED, and the economic event MUST instead be recorded through a PriorPeriodAdjustment command that posts in the first open period, carries relates_to_period and effective_at of the original event, and requires controller approval above a sponsor-configured threshold. Routing is fixed by XID-EVA-002 and XID-ADV-005: before the audited statements for a year are issued, adjustments found in audit post in that year's AuditAdjustment period (UNB-XIN-ACC-005); after issuance, a late item posts as a PriorPeriodAdjustment classified Immaterial (current-period correction with disclosure) or Material (formal restatement of comparatives, originally issued statements preserved) against the sponsor's annual materiality setting agreed with the auditor; a hard-closed period MAY be reopened only before its statements are issued and only with controller and CFO approval. Donor fund statements already issued MUST show the correction as an explained adjustment on the next statement, and an amended statement MUST be issued whenever an acknowledgment or tax-relevant figure changes.
  - Provenance: [UNB-XIN-EVA-002](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-EVA-002, XID-ADV-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given Period 2026-10 open; posting P validating for 2026-10-31; when HardClosePeriod(2026-10) commits while P is in flight; then exactly one outcome: P commits before the close (and is included in the close snapshot) or P fails with PERIOD_CLOSED/serialization failure and is retried as a PriorPeriodAdjustment.
    - AC-2: Given Period 2026-10 hard-closed; when a custodian correction effective 2026-10-15 arrives on 2026-11-20; then a PriorPeriodAdjustment posts in 2026-11 with relates_to_period=2026-10 and effective_at=2026-10-15; the 2026-10 snapshot is unchanged.
    - AC-D1: Given audited statements for 2026 issued on 15 April 2027 and an immaterial 2026 misposting found on 1 May 2027; when the correction is entered; then it posts in the open 2027 period as PriorPeriodAdjustment(Immaterial) with relates_to_period 2026 and no 2026 report changes.
    - AC-D2: Given a hard-closed period whose statements were already issued; when a user requests reopening; then the request is rejected regardless of approvals.
- **UNB-XIN-EVA-005:** Each external observation type MUST have a registered business idempotency key derived from source data (e.g., ACH return: original entry trace number + return reason code + return entry trace number; check: bank account + check number + item sequence; custodian transaction: custodian + account + transaction reference + version/cancel indicator; provider callback: provider event id). Ingestion MUST store the key and a payload hash. A record with an existing key and identical hash MUST be acknowledged without effect; a record with an existing key and a different hash MUST NOT be applied and MUST open a DuplicateKeyConflict case. Internal commands MUST use stable business keys (GrantId + installment, PaymentIntentId, ScheduleId + occurrence) rather than transport retry identifiers. Intermediary fees on partially returned wires and bank fees on returned payments MUST post as sponsor operating expense unless the fee schedule disclosed a pass-through to the fund before the grant was recommended (XID-EVA-003).
  - Provenance: [UNB-XIN-EVA-005](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-EVA-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given An ACH return R03 for trace T delivered three times; when ingested; then one PaymentReturnReported and one compensating journal batch (PAY-07, SIM-19).
    - AC-2: Given Custodian sends transaction ref X qty 100 then ref X qty 60 with correction indicator absent; when second record ingested; then no posting; DuplicateKeyConflict case with both payloads.
    - AC-D1: Given a $10,000 wire returned net of a $35 intermediary fee with no disclosed pass-through; when the return posts; then the fund is re-credited $10,000 and $35 posts to sponsor operating expense.
- **UNB-XIN-ECO-001:** The platform SHALL maintain operational cost attribution as a management-accounting projection (CostAttribution read model) derived from GL actuals, vendor cost records and domain events. Cost attribution records SHALL NOT create, modify or reverse any journal entry, fund balance, NAV, reservation, fee accrual or available-to-grant amount. Any economic consequence that must reach the books (fee invoice, chargeback, functional expense allocation) SHALL be raised only through the existing authorised accounting command for that purpose (UNB-ACC-010 fee/billing, UNB-ACC-019 functional allocation) with its own approval. Management attribution and the GAAP/Form 990 functional expense allocation MUST use the same activity dataset with separately versioned mappings; only the functional mapping, approved annually by the controller and external CPA, may reach financial statements (XID-ECO-005).
  - Provenance: [UNB-XIN-ECO-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ECO-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given a published AttributionRule version and a closed month; when the attribution run completes; then the GL journal count, every fund balance and every available-to-grant value are byte-identical before and after the run.
    - AC-2: Given an operator with cost-admin role; when they attempt to invoke a journal-posting command from the cost attribution API; then the request is rejected with COST_PROJECTION_NON_POSTING and an audit record is written.
    - AC-3: Given a sponsor contract configured for cost-plus chargeback; when the monthly chargeback amount is computed; then a draft invoice request is created in UNB-ACC-010 billing awaiting approval, linked to the ReconciliationRun id, and nothing posts until approved.
    - AC-D1: Given a change to the management activity mapping; when it is saved; then the functional expense allocation and statement of functional expenses are unchanged.
- **UNB-XIN-ECO-004:** For each closed accounting period and sponsor scope, the platform SHALL execute a ReconciliationRun that compares the sum of AttributedCost plus UnallocatedResidual plus reported UnusedCapacity with GL operating expense actuals for mapped accounts, per CostPool. The run SHALL (a) require the identity sum(attributed)+residual+unused = GL actual for each pool to within $0.01, (b) record UnallocatedResidual explicitly by pool and reason (UnmappedGLAccount, MissingDriverData, NoEligibleCostObject, LateInvoice), (c) never distribute residual to cost objects except through a ResidualPolicy rule whose output is grade D and separately labelled, and (d) mark the run ExceedsTolerance when residual exceeds the configured tolerance (default 2% of pool or $5,000, whichever is greater), requiring controller sign-off before economics reports for that period are published as final. Tolerance is fixed by XID-ECO-003: per pool |residual| ≤ max(2% of the pool's GL actual, $5,000) and in aggregate residual ≤ 1% of in-scope GL operating expense; any residual with reason UnmappedGLAccount fails regardless of size until the account is mapped.
  - Provenance: [UNB-XIN-ECO-004](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ECO-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given pool GL actual $100,000, attributed $93,000, unused capacity $5,000; when the run executes; then UnallocatedResidual = $2,000 is recorded with reasons and the identity holds.
    - AC-2: Given residual 4% of pool exceeds tolerance; when the economics report is requested as final; then publication is blocked until a controller signs off; draft view shows ExceedsTolerance banner.
    - AC-3: Given a GL account not mapped to any pool; when the run executes; then its full amount appears as residual reason UnmappedGLAccount and a mapping task is opened.
    - AC-4: Given a late invoice posted after close; when the period is re-reconciled; then a new ReconciliationRun version is created; the prior run is preserved and the restatement is visible.
    - AC-D1: Given a $300 residual coded UnmappedGLAccount in a $200,000 pool; when the ReconciliationRun completes; then the run is ExceedsTolerance and requires controller sign-off despite being below 2%.
- **UNB-XIN-ECO-018:** For every configured service charge (e.g. international ED/ER review, wire, stop-payment/reissue, complex-asset handling, expedited grant), the platform SHALL report monthly recovery ratio = posted charge revenue / attributed cost of the corresponding activity, with grade. A service charge to a DAF fund SHALL be permitted only when it is listed in the fee schedule disclosed before the request, the donor affirms it, and a recorded cost-basis memo supports it. Where the annual recovery ratio of a charge exceeds 1.10, the price SHALL be reduced at the next schedule version (XID-ECO-002). Waivers SHALL be recorded with reason and counted separately.
  - Provenance: [UNB-XIN-ECO-018](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-ECO-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given ED review charge $400 and attributed cost $520 over 10 reviews; when recovery ratio is computed; then 0.77 is shown with grade.
    - AC-2: Given a proposed increase giving expected ratio 1.6; when activation is requested; then blocked pending counsel review flag.
    - AC-3: Given 5 waived charges; when the report renders; then waivers are shown with reasons and excluded from revenue.
    - AC-D1: Given a wire charge with annual recovery ratio 1.18; when the next fee-schedule version is drafted; then the draft is blocked until the wire charge is reduced so the expected ratio is ≤ 1.10.
    - AC-D2: Given a special-request charge not listed in the disclosed fee schedule; when it is applied to a fund; then it is rejected with UNDISCLOSED_CHARGE.
<!-- END ADOPTED: cross-industry-discovery -->

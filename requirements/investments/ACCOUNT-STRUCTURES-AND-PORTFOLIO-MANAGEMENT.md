# Investment account structures and portfolio management requirements
Date: 2026-11-21 | Status: requirements candidate; no implementation, legal sign-off or vendor access claimed

## Scope and principles
Unbound SHALL accommodate sponsor-approved segregated custody accounts, omnibus custody with DAF-level internal accounting, pooled investment funds with unitized interests, model portfolios, and external investment managers. These are distinct operating structures, not interchangeable database configurations. Sponsor holds legal authority and fiduciary oversight. Donors and advisors may express preferences only within sponsor policy. Investment managers may have discretionary mandates only when documented and approved.

## Account structure requirements
- **UNB-INV-001:** Model legal sponsor, legal account holder, custodian, custody account, portfolio, investment pool, internal DAF fund, subfund position, investment mandate and manager as distinct identities with effective-dated relationships.
- **UNB-INV-002:** Support a DAF mapped to one or many segregated custody accounts, and one custody account supporting multiple DAFs via explicitly governed omnibus allocation.
> Adopted 2026-11-23: refined by `RR-UNB-INV-002-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-INV-003:** Support pooled investment vehicles with fund-level units/shares, valuation dates, unit prices, subscriptions, redemptions, rounding, cash buffers and participant-level holdings.
- **UNB-INV-004:** Sponsor-wide pool totals SHALL reconcile to custody assets and fund-level units; every subscription/redemption must have balanced, append-only accounting effects.
- **UNB-INV-005:** Support DAF allocation among approved portfolios/pools with minimums, eligibility, restrictions, target percentages, rebalancing rules and effective dates.
- **UNB-INV-006:** Support separately managed accounts and external advisors/RIA relationships with explicit investment discretion, delegated scope, account identifiers, limits, expiry and revocation.
- **UNB-INV-007:** Support model portfolios as versioned target allocation specifications, separate from actual holdings and separately from executable trade instructions.
- **UNB-INV-008:** Model account structures and migrations as sponsor-approved, auditable changes; no loss of source custody identifiers, tax lots, historical valuations or reconciliation evidence.
- **UNB-INV-009:** Support multiple custodians and multiple asset managers simultaneously, including manager transitions and overlapping read-only reporting periods.
- **UNB-INV-010:** Sponsor can configure allowable structures by partner, program, DAF type, account size, asset class, jurisdiction and legal/contractual eligibility.
- **UNB-INV-011:** Do not infer that a donor-specific custody account is legally donor-owned or that pooled units are legally transferable securities; document actual legal and accounting classification.
- **UNB-INV-012:** Support in-kind contribution and in-kind transfer workflows, including asset acceptance, restrictions, valuation, liquidation policy and custody settlement.
> Adopted 2026-11-23: refined by `RR-UNB-INV-012-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.

## Authority, preferences and policy
- **UNB-INV-013:** Donor/advisor investment preferences SHALL be modeled as recommendations or policy-permitted elections, not automatically as trade authority.
- **UNB-INV-014:** Sponsor policy SHALL define investment menus, prohibited investments, ESG/mission preferences where supported, liquidity thresholds, concentration limits, rebalancing tolerance, manager eligibility and exception escalation.
- **UNB-INV-015:** Every portfolio change distinguishes request, policy evaluation, sponsor decision, approved instruction, execution, settlement and reconciliation.
- **UNB-INV-016:** Delegated investment discretion SHALL be explicit, time-bound where appropriate, auditable, revocable and restricted to named legal accounts/mandates.
- **UNB-INV-017:** Multiple participants with conflicting preferences SHALL follow a documented fund governance rule; no last-write-wins overwrite of approved policy.
- **UNB-INV-018:** Sponsor can freeze trading or allocation changes for compliance holds, donor death/incapacity, disputed succession, valuation uncertainty, liquidity stress or custodian outage.
- **UNB-INV-019:** AI may recommend portfolios, risk checks, liquidity plans and rebalancing, but may not bypass sponsor approval, mandate boundaries, suitability review where applicable, or execution controls.
  - *Refined 2026-11-24 by RR-UNB-INV-019-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-INV-004, decided-provisional):* AI or rule-based features MAY compute drift, risk metrics, liquidity forecasts and conformity to sponsor-approved models, and MAY propose actions that implement a sponsor-approved model or committee-approved parameter. They MUST NOT generate individualized securities selections or allocation advice for a donor, advisor or sponsor beyond approved models unless counsel has recorded the Advisers Act status of the party providing it (sponsor personnel acting under §203(b)(4), a registered adviser under mandate, or another basis). Every proposal MUST name the approved model/parameter it implements and MUST pass deterministic mandate and pre-trade checks before any human approval. The operator MUST NOT provide individualized investment recommendations, and operator compensation MUST NOT vary with assets or trades (XID-INV-004). The text above is retained as originally written.
- **UNB-INV-020:** Investment policies and exceptions SHALL be versioned, effective-dated and linked to event, decision, account and reporting evidence.

## Valuation, unitization and accounting
- **UNB-INV-021:** Define valuation methodology per asset class and investment structure, including market prices, stale pricing, illiquid assets, corporate actions and manual valuation approval.
- **UNB-INV-022:** Pool unit prices SHALL derive from an approved NAV calculation with documented assets, liabilities, fees, valuation time, cutoffs, rounding and exception policy.
- **UNB-INV-023:** Define subscriptions/redemptions using explicit trade date, valuation date, settlement date and recorded date; reject unsupported backdating and preserve bitemporal corrections.
- **UNB-INV-024:** Distinguish settled cash, pending trades, unsettled proceeds, encumbered liquidity, grants payable, reserved commitments and available-to-recommend balance.
- **UNB-INV-025:** Define investment gains/losses, income, dividends, fees, management expenses, FX and tax-lot/cost-basis treatment in versioned deterministic posting rules.
- **UNB-INV-026:** For segregated accounts, reconcile each legal custody account to DAF investment subledger and sponsor GL. For omnibus/pools, reconcile custody -> pool NAV -> units -> DAF holdings -> sponsor GL.
- **UNB-INV-027:** Unit allocations, fee allocations and performance calculations SHALL be reproducible from immutable events and policy versions; corrections append and trigger controlled revaluation/reissue.
- **UNB-INV-028:** Performance reports SHALL specify time-weighted versus money-weighted return methodology, benchmarks, cash-flow treatment, fees and as-of dates; do not equate accounting gain with performance return.
- **UNB-INV-029:** Define rounding residuals and break thresholds so unit balances, NAV and fund totals reconcile deterministically.
- **UNB-INV-030:** Define liquidity planning for grant commitments and stress scenarios; no investment recommendation may imply that unsettled or illiquid assets are immediately spendable.

## Portfolio operations and integrations
- **UNB-INV-031:** Support policy-governed rebalance proposals, drift detection, manager reviews, trade staging, restricted lists, pretrade controls and posttrade compliance.
> Converged 2026-11-24: governed by `CNV-LEG-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-INV-032:** Investment order routing is optional per custodian entitlement. Read-only feeds, file imports, manager-managed execution and API execution are separate capability profiles.
- **UNB-INV-033:** Each custodian/manager integration SHALL declare verified entitlements, account scope, data freshness, order authority, reconciliation sources, failure modes and contractual restrictions.
- **UNB-INV-034:** Support transfers between segregated accounts and pools with explicit sale/transfer, valuation, fees, tax lots, settlement and sponsor approvals; never assume a book transfer moves external assets.
- **UNB-INV-035:** Support investment restrictions and grant-related liquidity holds across multi-custodian, multi-pool portfolios without oversubscription or double counting.
- **UNB-INV-036:** Support investment manager fee schedules, pool-level fees, DAF-level charges, waivers and sponsor fee recovery with no double counting in consolidated financial reporting.
- **UNB-INV-037:** Support custody/manager failures, stale feeds, failed settlement, halted trading, valuation dispute, market closures, liquidity gates and suspended pool redemptions through explicit degraded states.
- **UNB-INV-038:** All investment APIs SHALL be versioned, tenant-bound, authorization-scoped and idempotent for external effects, with append-only evidence and safe retries.
- **UNB-INV-039:** Forma SHALL provide accessible mobile-first portfolio overview, investment preferences, risk/return context, allocation, liquidity, manager oversight and exceptions with source freshness.
- **UNB-INV-040:** Folio SHALL produce reproducible, accessible investment statements, NAV reports, manager reviews, policy exception reports and fund allocation reports with source/provenance metadata.
- **UNB-INV-041:** Partner segregation SHALL cover custody mapping, model portfolios, pool units, pricing, mandates, manager access, derived analytics, documents and AI context.
- **UNB-INV-042:** Strata-backed immutable records SHALL support custody observations, approved mandates, portfolio decisions, unit transactions, valuation snapshots, corrections and reconciliation events, subject to verified Strata capabilities.

## Investment operating structure comparison
| Structure | Key benefit | Key complexity | Required accounting proof |
|---|---|---|---|
| Segregated custody per DAF | Clear external holdings attribution | Many accounts, fees, onboarding, operational scale | Account-to-fund reconciliation |
| Omnibus custody, fund subledger | Operational consolidation | Allocation, beneficial/internal attribution, control | Custody totals = sum fund allocations |
| Unitized investment pools | Shared strategy and scalable fund allocations | NAV, pricing cutoffs, subscriptions/redemptions, liquidity | Pool NAV = outstanding units x price within approved rounding policy |
| Model portfolios | Consistent policy and allocation guidance | Model drift, mapping to actual trades | Models distinct from executed positions |
| External discretionary managers | Specialist expertise and existing relationships | Mandates, oversight, fee layering, access | Manager transactions reconcile to custodian and GL |

## Required acceptance scenarios
1. DAF A has a segregated Schwab account while DAF B holds units in a pooled strategy custodied at another institution; sponsor totals reconcile.
2. A DAF allocates 60/40 across two approved pools; valuation, cutoff and unit pricing are reproducible.
3. Two advisors recommend conflicting portfolio changes; governance rule prevents unapproved last-write-wins.
4. Donor preference conflicts with prohibited asset policy; sponsor control wins and reason is retained.
5. A pooled asset valuation is stale; NAV publication is held or marked provisional per policy, never silently treated as current.
6. A grant is approved before a pooled redemption settles; available cash and grant commitments are distinguished.
7. Fund successor split 50/30/20 with restricted, illiquid and unsettled assets; allocation policy holds impossible transfers and preserves total units.
8. An external RIA loses delegated authority; new trade submissions fail while historical reporting remains accessible.
9. Custodian sends duplicate and out-of-order trade corrections; immutable source history remains intact and only intended journals post.
10. Sponsor moves a DAF from segregated custody to a pool; actual external settlement and internal accounting transitions are separately tracked.
11. Same manager/custodian serves two institutional partners; API, Forma, Folio, exports and AI searches enforce isolation.
12. Market disruption causes redemption gate and delayed prices; grants do not assume immediate liquidity.
13. Rebalancing suggestion by AI is rejected by deterministic pretrade controls.
14. Manager fee plus sponsor fee is charged; fund expense and sponsor-level reporting reconcile without internal revenue double count.
15. An auditor recreates an earlier NAV and DAF investment statement from archived source observations and policy versions.
16. Partner moves from shared schema to dedicated DB during active portfolio settlement; no duplicate units or missing history.

## Unresolved decisions / research
INV-ADR-001: Which structures are mandatory at first release vs supported through extensions?
INV-ADR-002: Sponsor fund subledger accounting and omnibus beneficial/internal attribution.
INV-ADR-003: Unitization NAV, cutoffs, rounding, illiquid valuations and correction policy.
INV-ADR-004: Authorized investment menus, external manager mandates and sponsor fiduciary oversight.
INV-ADR-005: Schwab institutional API/contract entitlements and which account structures it supports.
INV-ADR-006: Cash and liquidity reserves, investment restrictions, fee allocation and grant timing.
INV-ADR-007: Portfolio performance methodology and statement disclosure.
INV-ADR-008: Strata integration, event ordering, projections, isolation and deterministic replay.

## Next specifications
- Investment account/portfolio/pool entity and relationship dictionary.
- Detailed pool NAV/unitization worked examples with rounding and late corrections.
- Investment-policy/mandate authority matrix and portfolio-change state machine.
- Custodian capability profiles and provider-specific verification (begin with Schwab).
- Investment API contract catalog and accounting posting crosswalk.

Traceability: UNB-CUST-001..028; UNB-ACC-001..036; ACC-MDL-001..012; UNB-SUCC-001..014; UNB-TEN-001..020. All examples are design candidates, not proof of vendor entitlement or legal/accounting compliance.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-INV-002-r1 (refines `UNB-INV-002`) — adopted-provisional
- **Original:** `requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md:9` @ `777a70d` (kept verbatim) · **Decisions:** RCD-020
- **Requirement:** Each custody account that supports more than one DAF MUST declare one attribution method, either PerFundLotSubAccounting (every position lot carries exactly one fund owner) or Unitized (governed by UNB-NAV rules). For PerFundLotSubAccounting, income, corporate actions and realized results MUST follow lot ownership, and the sum of fund-owned lots MUST equal the custodian position after known in-transit items.
- **Validation still required:** RCD-020: Controller
- **Acceptance criteria:**
  - AC-1: Given an omnibus account with lot sub-accounting holding 100 shares split 60/40; when a $1.00/share dividend is received; then funds receive $60 and $40.
  - AC-2: Given an account with no declared method; when a second fund allocation is attempted; then it is rejected.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-INV-002-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-INV-012-r1 (refines `UNB-INV-012`) — adopted-provisional
- **Original:** `requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md:19` @ `777a70d` (kept verbatim) · **Decisions:** RCD-014, RCD-019
- **Requirement:** In-kind contribution workflows MUST, before acceptance of a non-publicly-traded or complex asset, record: asset classification, completed-gift rule, gift-acceptance decision, excess-business-holdings test result (UNB-RCN-FIN-004), Form 8283 Section B donee-signature task where requested, and liquidation policy. Any disposition of property for which the sponsor signed a Form 8283 Section B MUST create a Form 8282 obligation record with deadline unless a counsel-approved exception applies.
- **Validation still required:** RCD-014: Counsel Q3, Q4; RCD-019: CPA on completed-gift rule table
- **Acceptance criteria:**
  - AC-1: Given closely held shares accepted with sponsor-signed 8283 Section B; when the shares are sold 14 months later; then an 8282 obligation is created with a due date per the counsel rule table.
  - AC-2: Given a gift of business interest where donor relatives hold related interests; when acceptance is attempted without an EBH test; then acceptance is blocked.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-INV-012-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-FIN-002 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-FIN-003 · **Parent:** UNB-GAP-012 · **Decisions:** —
- **Requirement:** When the sponsor signs a donor's Form 8283 Section B, Unbound MUST record the signature event, the property description, the lot identifiers, and the date of receipt, and MUST record that the signature acknowledges receipt and not value.
- **Acceptance criteria:**
  - AC-1: Given a donor requests a Section B signature; when the authorized signer signs; then a Form8283Signed record links to the custody lots and states receipt-only.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-002); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-003 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-FIN-003 · **Parent:** UNB-INV-012 · **Decisions:** —
- **Requirement:** Any sale, exchange or other disposition of a lot linked to a Form8283Signed record within the counsel-configured reporting window MUST create a Form8282Obligation with due date, owner and donor-copy task, unless a counsel-approved exception rule applies; obligations MUST be tracked to filing-receipt evidence and escalate before the due date.
- **Acceptance criteria:**
  - AC-1: Given a signed Section B lot sold within the window; when the sale settles; then a Form8282Obligation with due date is created.
  - AC-2: Given an obligation 10 days before due with no filing receipt; when the daily check runs; then an escalation is raised to tax compliance.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-003); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-004 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-005 · **Parent:** REQ-X-059 · **Decisions:** RCD-014
- **Requirement:** Unbound MUST maintain business-enterprise identities and related-person holdings declared at gift acceptance, MUST compute combined holdings for each fund and its donors/advisors/related persons per counsel-configured rules before accepting a business interest and on each later related transaction, and MUST track any disposal period with escalation.
- **Validation still required:** RCD-014: Counsel Q3, Q4
- **Acceptance criteria:**
  - AC-1: Given a gift of business interest with related-person holdings above the configured permitted level; when acceptance is attempted; then acceptance is held for counsel decision with a disposal-period plan.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-004); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-007 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-016 · **Parent:** UNB-INV-002 · **Decisions:** RCD-020
- **Requirement:** Every multi-fund custody account MUST declare one attribution method (PerFundLotSubAccounting or Unitized); undeclared accounts MUST NOT accept multi-fund allocations; lot-owned positions MUST reconcile per security to custodian positions.
- **Validation still required:** RCD-020: Controller
- **Acceptance criteria:**
  - AC-1: Given lot sub-accounting with 60/40 ownership; when a dividend arrives; then income splits 60/40 by record-date lots.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-007); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-010 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-006 · **Parent:** UNB-INV-006 · **Decisions:** RCD-021
- **Requirement:** Before any advisor or manager compensation accrues from DAF assets, Unbound MUST record the counsel-approved classification of the advisor's relationship to the fund's donors/advisors and the applicable compensation policy, and MUST hold accrual when the classification is missing or indicates a donor-advisor relationship pending counsel decision.
- **Validation still required:** RCD-021: Counsel Q-advisor (§4958)
- **Acceptance criteria:**
  - AC-1: Given an advisor flagged as the donor's personal investment adviser; when the first fee accrual runs; then accrual is held pending counsel decision.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-010); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-FIN-012 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-FIN-007 · **Parent:** UNB-ACC-006 · **Decisions:** RCD-019
- **Requirement:** Unbound MUST determine the completed-gift date from a versioned, counsel-approved rule table keyed by asset type and rail (e.g., DTC transfer, physical certificate, check, ACH, card, wire, crypto, real property), and MUST hold the contribution when evidence required by the rule is missing.
- **Validation still required:** RCD-019: CPA on completed-gift rule table
- **Acceptance criteria:**
  - AC-1: Given a mailed check dated December 31 received January 3; when completed-gift determination runs; then the date follows the configured rule and records the rule version.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-FIN-012); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-INV-002:** Every investment pool MUST record a counsel-approved LegalBasis (statutory exclusion or registration relied upon), the charitable organization that maintains it, its trustee/administrator and the sponsors whose assets it may hold. Unbound MUST reject subscriptions from a fund of a sponsor not listed, MUST reject creating a pool whose maintainer is not a charitable organization unless the LegalBasis cites a different counsel-approved basis, and MUST re-require approval when the maintainer or sponsor list changes. For release 1 only single-sponsor pools maintained by the sponsor itself under the charitable-organization exclusion are permitted; multi-sponsor pools and pools maintained by the operator MUST be rejected, and donors MUST receive the pool's written disclosure before first allocation (XID-INV-003).
  - Provenance: [UNB-XIN-INV-002](../../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-INV-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given pool P maintained by Sponsor A; when a fund of Sponsor B subscribes; then the subscription is rejected with SponsorNotEligibleForPool.
    - AC-2: Given a pool configured with maintainer = platform operator and no alternative basis; when activation is requested; then activation is rejected.
    - AC-D1: Given a pool definition listing two sponsors; when it is created; then creation is rejected with MULTI_SPONSOR_POOL_NOT_PERMITTED.
<!-- END ADOPTED: cross-industry-discovery -->

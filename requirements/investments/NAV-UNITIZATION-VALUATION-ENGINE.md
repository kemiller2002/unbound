# Investment valuation, NAV, unitization and correction engine
Date: 2026-11-21 | Status: candidate requirements and mathematical examples, not implemented or approved accounting policy

## Purpose and scope
Define the deterministic investment valuation and unitization engine for sponsor-approved pooled investments, omnibus allocations and DAF fund holdings. Preserve the outcome, not legacy workflows. Sponsor owns/controls charitable assets; fund units represent internal allocations under sponsor policy, not automatically legally issued securities or donor-owned property.

## Binding invariants
- **UNB-NAV-001:** Every investment pool has an accounting entity, sponsor, pool ID, functional currency, valuation policy/version, approved calendar/cutoff, custodian mappings and effective-dated eligible DAFs.
- **UNB-NAV-002:** Every authoritative source observation, NAV run, price, unit transaction, allocation, adjustment, approval and publication is append-only. Corrections append linked superseding records, never UPDATE/DELETE.
- **UNB-NAV-003:** Calculate NAV deterministically from eligible assets less liabilities, using approved pricing sources, cutoff, accounting basis, accrued income, fees, expenses, unsettled trades and FX policies. Never treat missing prices as zero without explicit approved policy.
- **UNB-NAV-004:** Unit price = NAV attributable to outstanding units / outstanding units, with approved treatment for first issuance, zero units, negative NAV, suspended valuation and residuals.
- **UNB-NAV-005:** Valuation states SHALL distinguish Draft, InputsValidated, Calculated, ExceptionHold, IndependentlyReviewed, Approved, Published, Corrected/Superseded and Rejected; publishing is a controlled act, not a calculation side effect.
- **UNB-NAV-006:** A valuation SHALL have immutable source high-water marks, as-of/effective time, recorded time, price-source versions, FX rates, posting policy, rounding policy, code/rule version, reviewer and integrity checkpoint.
- **UNB-NAV-007:** Validate custodian cash/positions, security identifiers, quantities, corporate actions, liabilities, accrued fees, unrecognized items and interfund allocations before publication; material unexplained breaks prevent final publication.
- **UNB-NAV-008:** Define independent maker-checker approval for published NAV and privileged overrides; AI may propose resolutions but cannot approve its own calculations or overrides.
- **UNB-NAV-009:** Support daily, periodic and event-driven valuations, with explicit calendars, market closures, holidays, valuation freezes and after-cutoff processing.
- **UNB-NAV-010:** Every published valuation and dependent report must remain reproducible as originally issued, even after a correction.
- **UNB-NAV-011:** Define separate calculated NAV, approved NAV, published NAV and currently effective NAV; API consumers must explicitly request the intended view.
- **UNB-NAV-012:** Pool and DAF unit positions, liability allocations and GL balances SHALL reconcile under an approved tolerance/rounding policy; no unexplained balancing plug.

## Pricing and valuation
- **UNB-NAV-013:** Define pricing hierarchy by asset class: independent market feed, custodian mark, approved valuation service, documented manual appraisal, or explicit hold. Source precedence and freshness limits are policy controlled.
- **UNB-NAV-014:** Support securities, cash, funds, fixed income, illiquid assets, restricted holdings, accrued interest/dividends, corporate actions and currencies where allowed by investment policy.
- **UNB-NAV-015:** Stale, conflicting, missing or outlier prices create exceptions with severity, affected assets/funds, fallback candidates and reviewer decision; never silently forward-fill.
- **UNB-NAV-016:** FX valuation retains original currency, quote time, rate source, conversion method and functional currency; distinguish FX remeasurement from investment performance.
- **UNB-NAV-017:** Illiquid valuations require appraisal methodology, approval, uncertainty range where meaningful, age limits and valuation-event triggers; mark provisional if policy permits.
- **UNB-NAV-018:** Pricing corrections must trigger impact analysis across NAV, units, fees, grants, financial statements and manager performance, not only change the displayed price.
- **UNB-NAV-019:** A provider observation is evidence, not automatically an approved accounting mark; reconcile source conflicts explicitly.

## Unitization, subscriptions and redemptions
- **UNB-NAV-020:** Each pool subscription/redemption has fund, pool, amount or units, requested/approved/accepted timestamps, valuation point, cutoff decision, fees, settlement and external/internal transfer references.
- **UNB-NAV-021:** Define forward-pricing or other approved allocation methodology explicitly. Do not allow a participant or agent to choose a known favorable historical unit price.
- **UNB-NAV-022:** Use fixed-precision decimal/rational arithmetic with specified precision, rounding mode and residual allocation. Never binary floating point for authoritative monetary/unit calculations.
- **UNB-NAV-023:** Unit issuance = accepted net subscription amount / applicable approved unit price, subject to rounding, minimum unit increment and residual handling.
- **UNB-NAV-024:** Redemption proceeds = approved redeemed units × applicable approved unit price, less applicable charges, subject to settlement, cash availability, gating and rounding policy.
- **UNB-NAV-025:** Pending subscriptions and redemptions SHALL not change outstanding units before the policy-defined recognition/strike point; avoid circular NAV calculations.
- **UNB-NAV-026:** Partial redemptions, gates, suspensions, insufficient units, restricted allocations and unsettled transfers require explicit state and deterministic handling.
- **UNB-NAV-027:** Every unit issue/redeem/transfer preserves sponsor-wide pool totals and has balanced fund subledger and GL posting rules; custody movements are tracked separately from internal allocation.
- **UNB-NAV-028:** Prevent oversubscription, duplicate processing and simultaneous redemption races through idempotency, optimistic concurrency/single-writer controls and fund-level reservations.
- **UNB-NAV-029:** A DAF may hold multiple pool positions and separate custody positions; portfolio aggregation must avoid double counting underlying pooled holdings.
- **UNB-NAV-030:** Successor-account splits and DAF mergers preserve total units, restrictions, commitments and audit history; fractional rounding residuals require explicit policy.
- **UNB-NAV-031:** Pool closure, merger, share-class change and migration require governed conversion ratios, notices, approvals and reproducible allocations.

## Fees, performance and liquidity
- **UNB-NAV-032:** Support sponsor, manager, custodian, investment pool and performance-related fees only where policy/contract permits, with explicit assessment base, accrual frequency, tiers, minimums, waivers and effective dates.
- **UNB-NAV-033:** Accrue liabilities before NAV strike when required by approved policy; ensure fees are not charged twice at pool and DAF levels.
- **UNB-NAV-034:** Define distribution/income reinvestment, fund-level allocation and realized/unrealized gain treatment independently of unit price changes.
- **UNB-NAV-035:** Distinguish total NAV, settled cash, unsettled cash, encumbered cash, redemption proceeds pending, grant commitments and available-to-recommend funds.
- **UNB-NAV-036:** Support liquidity buffers, redemption gates, side pockets if approved, market halts and stress tests; do not promise immediate cash from illiquid holdings.
- **UNB-NAV-037:** Performance metrics define TWR/MWR method, benchmark, gross/net fees, cash-flow timing and valuation assumptions; NAV growth alone is not a valid return measure.

## Corrections, replay and financial close
- **UNB-NAV-038:** Corrected NAV is a new approved publication linked to the original, with cause, affected period, affected pool/funds, materiality, responsible reviewer and downstream remediation plan.
- **UNB-NAV-039:** Corrections distinguish source-price error, missed position, late trade, corporate action, FX error, fee accrual error, wrong cutoff, unit allocation error and fraud/unauthorized override.
- **UNB-NAV-040:** Calculate counterfactual units and monetary impacts for each affected fund, transaction, grant, fee, statement and period; preserve original and corrected views.
- **UNB-NAV-041:** Apply a sponsor-approved materiality/remediation policy, including when to compensate affected funds, restate NAV, adjust units, issue corrected statements or escalate to auditor/regulator.
- **UNB-NAV-042:** Never retroactively overwrite settled transfers or historical unit issuance. Append corrective journals/unit events, reconcile to original and approved revised economic outcomes.
- **UNB-NAV-043:** Closed-period corrections require separate accounting-close and restatement authority; originally published Folio reports and their delivery receipts remain preserved.
- **UNB-NAV-044:** Rebuild projections from append-only Strata-integrated evidence deterministically and compare checksums, units, balances and published results.
- **UNB-NAV-045:** Correction propagation must be idempotent and bounded, with explicit causal dependencies, impact report, approval gate and no unbounded automated cascade.
- **UNB-NAV-046:** If source facts or approvals are unavailable, valuation remains on hold or clearly provisional under policy; no fabricated NAV, prices, balances or completed settlements.

## Interfaces and enterprise integration
- **UNB-NAV-047:** Provide governed versioned APIs for pool setup, pricing observations, valuations, approval, publication, subscription, redemption, fee accrual, corrections, impact analysis, reconciliations and as-of queries.
- **UNB-NAV-048:** All APIs enforce sponsor/partner/fund/manager authority, tenant context, idempotency, immutable evidence and contract tests. No privileged bypass for AI.
- **UNB-NAV-049:** Forma provides mobile-first WCAG 2.2 AA-targeted accessible NAV review, price exceptions, pool positions, subscriptions, redemptions, correction impact, approvals and fund statements.
- **UNB-NAV-050:** Folio produces immutable-versioned pool valuation packs, DAF investment statements, fee disclosures, NAV correction notices, reconciliation evidence and auditor workpapers.
- **UNB-NAV-051:** Strata's actual transactional, append-only, ordering, projection and recovery capabilities must be verified; implement necessary companion guarantees only through approved architecture.
- **UNB-NAV-052:** All pricing, units, positions, NAV, evidence, reports and AI retrieval obey partner isolation across shared schemas, separate databases and dedicated infrastructure.
- **UNB-NAV-053:** Independent custody and bank data reconciliation is required; provider feed availability and data freshness are displayed rather than inferred.

## Mathematical worked example (illustrative, not approved pricing policy)
At a validated valuation point: assets $1,050,000; accrued liabilities $10,000; net assets $1,040,000; 100,000 units outstanding. **NAV/unit = $10.40**.
A $5,200 eligible subscription struck at this price issues **500 units**. If the subscription is recognized after the price strike, the resulting assets are $1,045,200 and units 100,500; implied NAV/unit remains $10.40 absent fees/market movements.
A 125-unit redemption at $10.40 yields **$1,300 gross proceeds** before any applicable fees, liquidity gates and settlement. After redemption, assets and units reduce in tandem under the approved recognition policy.
A later discovery of an omitted $1,000 liability would make the original NAV $1,039,000 / 100,000 = **$10.39**. The original $10.40 publication must remain immutable. A correction process determines whether and how the 500-unit subscription and 125-unit redemption are economically adjusted, including materiality, approved remediation, current unit balances and issued statements. Do **not** silently recalculate and overwrite historical units.

## Deterministic acceptance tests
NAV-T01: NAV = eligible assets minus recognized liabilities with explicit inclusion/exclusion and no hidden balancing entries.
NAV-T02: Zero units, zero/negative NAV, missing price, missing FX and stale custody feed enter defined safe states.
NAV-T03: Price-source precedence, age limits, exceptions and manual overrides are reproducible.
NAV-T04: Decimal precision, rounding, residual allocation and multiple sequential subscriptions reconcile.
NAV-T05: Two subscriptions at same cutoff and a late submission use correct approved valuation point.
NAV-T06: Duplicate/replayed subscription and redemption do not create duplicate units or cash movement.
NAV-T07: Concurrent redemptions cannot exceed fund units, liquidity or sponsor limits.
NAV-T08: Partial redemption/gating/market closure preserve requested, approved, executed and settled states.
NAV-T09: Fee accrual reduces NAV correctly and is not charged twice.
NAV-T10: Corporate action, late trade and FX correction trigger correct impact analysis and approved remediation.
NAV-T11: Original NAV and Folio statement remain reproducible after corrected publication.
NAV-T12: Successor split 50/30/20 preserves total units and restrictions within rounding policy.
NAV-T13: Pool NAV, unit allocations, custody positions and GL reconcile or generate controlled breaks.
NAV-T14: Closed-period correction retains original reports and creates authorized restatement artifacts.
NAV-T15: Cross-partner unauthorized price/units/statement queries are denied across all isolation profiles.
NAV-T16: An AI-proposed valuation override cannot publish without independent approval.
NAV-T17: Strata-backed projection rebuild yields identical as-of results and integrity checkpoints.
NAV-T18: Custodian outage and restart cannot publish stale NAV as current or duplicate postings.

## Unresolved policy/architecture decisions
NAV-ADR-001: NAV frequency, strike time, time zone, business-day calendar and cutoff policy.
NAV-ADR-002: Pricing hierarchy, illiquid valuation methods, stale thresholds and provisional valuations.
NAV-ADR-003: Unit precision, money rounding, residual handling, initial unit price and negative NAV.
NAV-ADR-004: Subscription/redemption recognition, forward pricing, settlement and liquidity gates.
NAV-ADR-005: Fee schedule, accrual, allocation and consolidated accounting treatment.
NAV-ADR-006: Materiality, NAV error compensation, reissue/restatement and auditor oversight.
NAV-ADR-007: Unitized fund accounting subledger vs GL dimensions and consolidation.
NAV-ADR-008: Strata integration and independent deterministic replay/transaction guarantees.
NAV-ADR-009: Regulatory/accounting jurisdiction, sponsor approval and external investment provider entitlements.
NAV-ADR-010: Market data licensing, retention and redisplay rights.

## Traceability and status
Extends UNB-INV-001..042, UNB-CUST-001..028, UNB-ACC-001..036, ACC-MDL-001..012, UNB-SUCC-001..014 and UNB-TEN-001..020. These are design candidates. No code compiled, tests executed, market data licensed, Schwab integration confirmed or accounting/legal approval obtained.

# Illustrative fund accounting journal contracts and reversals
Date: 2026-11-23. **Proposed accounting examples only. Not authoritative GAAP, tax advice or a sponsor-approved chart of accounts.** Validate with a nonprofit fund accountant/CPA and actual sponsor accounting policy before implementation.

## Fundamental model
Use balanced double-entry postings with one authoritative sponsor legal-entity ledger and explicit fund dimensions, restrictions, currencies, effective/posted timestamps, evidence and reversal links. Fund reservation is **not** an expense or external cash movement. Maintain available-to-grant as a controlled projection from reconciled assets, obligations, holds, restrictions and liquidity policy, not a free-standing balance users can edit.

Illustrative accounts: Cash—Operating/Custody; Cash—Clearing; Contribution Revenue (or applicable contribution recognition account); Investments—At Carrying Value; Investment Realized Gain/Loss; Investment Unrealized Gain/Loss; Grant Expense; Grants Payable; Payment Clearing; Fee Expense; Fee Payable; Suspense/Unidentified Receipts; and appropriate Fund Net Assets. Actual accounting treatment, restrictions and presentation vary by sponsor and transaction.

## Example A: $100 settled and legally accepted cash gift
Assume a single legal sponsor, unrestricted eligible contribution, no processor fee, no conditional promise and cash actually settled.
- **On settlement into suspense before acceptance (when policy requires):** Dr Cash $100; Cr Suspense/Unidentified Receipts $100. This is a liability/clearing example, not a universal classification.
- **On legal acceptance and recognition:** Dr Suspense/Unidentified Receipts $100; Cr Contribution Revenue $100, subject to the sponsor's recognition policy.
- **Availability:** after reconciliation and restriction checks, grantable projection may increase by $100. No extra journal entry is created solely to make it available.
- **If ACH subsequently returns:** Dr a sponsor-approved return/reversal account $100; Cr Cash $100, with explicit link to original receipt and recalculation of capacity. If revenue reversal is required, use sponsor policy. Do not silently delete the accepted gift or issue a donor refund without legal review.

## Example B: $80 grant from $100 available
Assume donor recommendation approved under valid sponsor delegation, recipient eligible and payment permitted.
- **At atomic reservation:** no GAAP journal required merely for reservation; post an immutable encumbrance/reservation record of $80. Available-to-grant becomes $20, subject to other restrictions.
- **At grant obligation recognition:** illustrative Dr Grant Expense $80; Cr Grants Payable $80, **only when sponsor accounting policy says an unconditional obligation exists**. Sponsor approval may or may not be that point.
- **At verified payment:** Dr Grants Payable $80; Cr Cash $80 (or first use Payment Clearing where bank reconciliation requires it). Release/consume reservation exactly once.
- **If payment returns after settlement:** illustrative Dr Cash $80; Cr Grants Payable $80 if the obligation remains payable. If obligation has been legally canceled, an authorized grant-expense reversal or other adjustment is needed. Do not automatically add $80 to available-to-grant before obligation and holds are resolved.
- **If grant canceled before obligation:** release reservation under authorization and policy, without fictitious cash journal.

## Example C: $100 accepted investment, later sold for $90 net
Assume valuation and accounting policy support initial $100 investment recognition; transfer legally accepted, with no restriction and no fee for simplicity.
- **At recognition:** Dr Investments $100; Cr Contribution Revenue $100 (conditional on accepted gift, valuation evidence and policy).
- **At sale/settlement:** Dr Cash $90; Dr Realized Investment Loss $10; Cr Investments $100. If sale price and fees are separate, split proceeds, fees and gain/loss explicitly.
- **Grantable cash:** no $100 liquid availability while asset is illiquid; after settled sale, the policy-derived liquid capacity may be $90 net of obligations.
- **If value later revised before sale:** valuation adjustments must use appropriate unrealized gain/loss and period/effective date; do not mutate historical as-issued artifacts.

## Example D: $5 administration fee
Assume approved fee contract and no waiver.
- **Accrual:** Dr Administration Fee Expense $5; Cr Fee Payable $5.
- **Settlement:** Dr Fee Payable $5; Cr Cash $5.
- Sponsor/operator/advisor revenue-sharing must be separately attributed; a fee charged to a DAF fund is not automatically platform company revenue. A related-party fee requires governance evidence.

## Example E: $80 grant paid in October and returned in November
- October grant and payment remain in the locked, as-issued October reporting snapshot.
- November: Dr Cash $80; Cr Grants Payable $80 if obligation remains, or the sponsor-approved adjustment account if not. Create a return evidence event and linked case.
- If the original posting was an accounting error rather than a real subsequent return, treatment may instead require prior-period correction or amended reporting under accounting policy.
- Never backdate a hidden mutation to make October appear as though the payment never occurred.

## Required typed journal schema
`JournalBatchId, SponsorLegalEntityId, FundId, PostingDate, EffectiveAt, KnownAt, Currency, JournalLines[{AccountId, Debit, Credit, FundDimension, RestrictionDimension}], SourceAggregateId, SourceEventId, EvidenceHash, PolicyVersion, IdempotencyKey, ReversalOfBatchId?, ApprovalEvidence, ClosePeriodId`.
Reject unbalanced postings per currency, cross-sponsor lines, duplicate source events, unauthorized adjustments, nonexistent accounts and edits to closed periods. Support separate currency translation and FX realization rules under controller approval.

## Critical accounting decision gates
1. When is the gift legally accepted versus recognized under accounting standards?
2. At which point is a grant unconditional and its payable/expense recognized?
3. Is an unsettled receipt cash, a receivable, suspense, or neither under the sponsor's policy?
4. What is the treatment of returned ACH after a legally accepted irrevocable contribution?
5. Which assets count toward spendable/grantable capacity and at what haircut?
6. How are investment NAV changes, accrued fees, investment income, restrictions and custodian differences posted?
7. How are noncash donor acknowledgement amounts distinguished from sponsor book values and appraisal evidence?
8. Which corrections affect current periods, require restatement, or trigger amended tax/reporting packages?

## Nonnegotiable invariants
Balanced entries; one authoritative writer; immutable posted batches with linked corrections; no direct editing of capacity; no ledger mutation from external UI status; no duplicate payment settlement; no sponsor cross-posting; deterministic mockable evidence and numeric reconciliation.

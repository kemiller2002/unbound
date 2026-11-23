# DAF fund accounting, AUM, valuation and reconciliation

Date: 2026-11-23. **Proposed theoretical requirements, not an accounting opinion, legal conclusion, implementation or executed testing.** Component monolith, one sponsor-owned authoritative ledger per legal-entity scope, shared typed accounting contracts. Companion REQ-X-017..022 and ADV-001..116.

## Governing model

Unbound must distinguish (1) legal-entity books of the sponsoring charity, (2) fund-level economic attribution and grant capacity, (3) pooled-investment unit holdings or separately managed assets, and (4) external custodian/provider statements. These are related but not interchangeable. A DAF is generally a fund/account maintained by a sponsoring organization, not an independently owned legal entity. Confirm each sponsor's legal, accounting, custody and regulatory arrangement before applying a generic model.

**Authority:** Sponsor general ledger and fund subledger are internally authoritative for accounting and fund allocation, subject to independent external custody reconciliation. Custodian statements are authoritative evidence of external positions only to the extent verified; they do not grant authority to rewrite the ledger. Payment provider status does not independently prove correct final settlement.

## Domain requirements and challenges

### 1. Accounting boundaries

Sponsoring charity is legal owner; donor-advised fund is an internal advisory/restriction/accounting dimension, not presumed separate legal entity. General ledger records sponsor books; fund subledger records attribution, restrictions and available grant capacity. Custodian positions and pooled vehicle units are external assets, not a second authoritative financial ledger.

### 2. Ledger dimensions

Journal ID, line ID, legal entity/sponsor, fund, account, asset/currency, effective date, recorded date, accounting period, source event, valuation basis, restriction class, counterparty, correction link. Journal lines balance by legal entity and currency with explicit FX bridge, not by naive sum across currencies. Separate memorandum/units from monetary postings.

### 3. Fund unitization

Where a sponsor uses pooled investment vehicles, track units issued/redeemed per fund, pooled NAV, valuation cut-off, trade/settlement timing, fractional precision and rounding residual. Units represent economic participation, not legal ownership of individual securities. Avoid assuming all sponsors use unitization; support segregated portfolios and cash-only funds.

### 4. Contributions

Distinguish pledge/intention, received, accepted by sponsor, cleared/final, restricted, available for investment, and reversible/returned. Noncash assets require valuation, acceptance, custody and liquidation policy. Donor tax acknowledgment is separately governed; platform cannot assume contribution acceptance equals tax-deductibility.

### 5. Grants

Recommendation -> independently authorized grant -> reservation/commitment -> disbursement instruction -> submitted -> reconciled settlement -> returns/reversals. Account for payable/expense recognition according to approved sponsor/CPA policy; avoid declaring one universal accounting event. Partial disbursement consumes only allocated reservation.

### 6. Fees

Contract version, fee basis, assessment period, tier/breakpoint, minimum, waivers, manual processing, paper check and mailing service event, posting time and tax treatment. Fee accrual and capacity calculations must be consistent; duplicate events idempotent. Fees charged to a fund require sponsor authorization and appropriate disclosure.

### 7. Investment lifecycle

Trade intent, execution, settlement, lot/position, corporate action, income, realized/unrealized return, fees, cash, transfers and restrictions. Separate trade-date and settlement-date views, custodial truth, and valuation corrections. Do not use stale NAV for grants without explicit haircuts and liquidity rules.

### 8. Liquidity and capacity

Fund-level available-to-grant differs from accounting net assets, cash available and pooled NAV. Include restricted/unsettled assets, reserves, pending trades, accrued fees, grants already committed, withdrawal limits, FX buffer and liquidity haircuts. Policy defines negative capacity and exception behavior.

### 9. Foreign exchange

Store native currency for every event and explicit rate source/time, base currency equivalent, conversion legs, realized FX and rounding differences. Never net unlike currencies directly. Reserve FX exposure or obtain committed conversion terms for cross-currency grants.

### 10. Valuation and NAV

Document source hierarchy, valuation date/cutoff, stale prices, nonmarketable assets, corporate actions, price challenge, independent review, fund-level NAV publication, revision and historical snapshots. A late correction creates new version with provenance, not silent restatement.

### 11. Reconciliation

Daily/periodic independent reconciliations among sponsor GL, fund subledger, custodian cash, positions, units, provider settlement, bank and grant/payment ledgers. Exceptions classified by timing, valuation, ownership, missing posting or fraud; preserve evidence and segregation of duties.

### 12. Close and reporting

Period close with late-arrival handling, adjustment periods, locked publication snapshots, audit trails, sponsor statements, fund balances, grant activity, fees, AUM and regulatory/tax reporting inputs. A later correction must not erase what was reported previously.

### 13. Tenant and sponsor boundaries

Fund ownership and unitization pools are scoped to sponsor/legal entity. Shared investment service infrastructure does not permit commingling legal assets or cross-sponsor fund allocations without explicit legally validated structure.

## Example accounting event chain (illustrative, not universal postings)

1. A $10,000 donor contribution is proposed. No accepted sponsor asset or available grant capacity is recognized merely because an intent exists.
2. Sponsor accepts and independently verifies receipt; authorized accounting policy determines when cash, contribution revenue, donor restrictions and fund attribution are posted.
3. The fund allocates $8,000 to a sponsor-approved pooled strategy and retains $2,000 cash. Record units at the approved NAV cut-off, without representing the donor as owner of the securities.
4. An approved $500 grant requires a transactionally reserved amount and appropriate recognition of commitment/payable under the sponsor's accounting policy. This does not mean the bank has paid $500.
5. Payment release rechecks sanctions, charity, destination, capacity, holds and provider. External submission is tracked separately from evidence of settlement and finality.
6. If a $25 processing fee is authorized, the service event and fee contract determine its accrual; the ledger records one idempotent financial effect. The grant reservation and fee must not spend the same available funds.
7. At month end, fund subledger, pooled units/NAV, GL, custody cash/positions and provider settlement are reconciled. Differences become exceptions, not undocumented balancing entries.

## Required accounting invariants

| ID | Invariant |
|---|---|
| ACC-01 | Every posted journal balances by legal entity and currency with explicit FX treatment |
| ACC-02 | Fund-level allocations reconcile to sponsor-level control accounts |
| ACC-03 | Pooled units outstanding reconcile to sum of fund holdings plus documented treasury/residual units |
| ACC-04 | Fund grant capacity never exceeds policy-eligible assets less encumbrances under concurrent reservations |
| ACC-05 | Contribution acceptance, tax substantiation, liquidity and grantability remain distinct |
| ACC-06 | Investment trade, settlement, valuation and liquidity have independent timestamps and statuses |
| ACC-07 | Fees have authorized basis, disclosed terms and unique service event; no duplicate postings |
| ACC-08 | Corrections preserve original journal, NAV and published statement versions |
| ACC-09 | External provider or custody evidence is reconciled, not blindly substituted for internal accounting |
| ACC-10 | Pooled investment returns and costs are allocated by independently validated ownership units/rules |
| ACC-11 | Restrictions, donor succession and sponsor authority cannot be overridden by fund balance alone |
| ACC-12 | Sponsor/legal entity assets are never cross-subsidized through an unapproved common pool |

## Required reconciliation controls

| Reconciliation | Comparison | On difference |
|---|---|---|
| Cash | GL cash vs bank/custodian statements | Classify timing, missing, duplicate or unauthorized item; preserve exception |
| Positions | Investment subledger vs custodian positions and corporate actions | Reconcile lot/unit/settlement, no silent adjustment |
| Fund allocation | Sum of fund units and balances vs sponsor control accounts | Hold impacted valuation/disbursement if material |
| Grants | Reservations, approved grants, payment intents and settlement | Detect unpaid, duplicate, returned and Unknown |
| Fees | Sponsor fee contract and service events vs postings | Correct through linked entries, never duplicate |
| AUM/NAV | Pricing sources, holdings, liabilities and published NAV | Governed price challenge, versioned correction |
| FX | Conversion legs, settlement amounts, rates and bridge accounts | Record exposure, gain/loss and rounding |

## Twenty-four adversarial test specifications (unexecuted)

| ID | Injection | Expected control |
|---|---|---|
| ADV-117 | Sponsor and donor fund treated as separate legal owners | Keep sponsor legal entity and fund attribution distinct |
| ADV-118 | Pooled NAV unit issuance after cutoff uses prior price | Enforce published valuation cutoff and correct effective price |
| ADV-119 | Two funds subscribe/redeem concurrently at same NAV | Conserve pool units and cash, deterministic rounding |
| ADV-120 | Illiquid contribution counted as immediately grantable cash | Capacity excludes or haircuts per approved policy |
| ADV-121 | Custodian trade executes but settlement fails | Separate trade/settlement and restrict liquidity |
| ADV-122 | Sponsor posts fee after grant reservation | Prevent double-spend; preserve prior valid commitment |
| ADV-123 | Same manual check fee imported twice | One fee event, one posting |
| ADV-124 | FX rate changes between approval and payout | Bound FX exposure or hold/requote |
| ADV-125 | NAV corrected after published donor statement | Version correction and preserve original report |
| ADV-126 | Corporate action changes positions after price cutoff | Reconcile units/positions with effective dates |
| ADV-127 | Custodian cash differs from GL due to missing transaction | Flag reconciliation exception; no invented balancing entry |
| ADV-128 | Pooled fund distributes gains disproportionately | Allocate using validated unit holdings and policy |
| ADV-129 | Donor contribution reversed after grants committed | Loss allocation/hold according to approved policy, not silent overdraft |
| ADV-130 | Multiple currencies net to zero numerically | Reject invalid balancing across currencies |
| ADV-131 | Cross-sponsor asset pool used without legal approval | Block pooling absent validated legal/custody structure |
| ADV-132 | Investment management fee and platform fee double-count | Separate charge basis and prevent duplicate economic expense |
| ADV-133 | Grant return posted to closed fund | Governed successor/refund disposition and linked reversal |
| ADV-134 | Valuation stale while sponsor asks for high grant | Hold or haircut, no optimistic NAV |
| ADV-135 | Portfolio transfers create phantom income | Transfer/reclassification not recognized as investment return |
| ADV-136 | Late period transaction changes closed AUM | Controlled adjustment and reproducible as-published reporting |
| ADV-137 | Fund redemption exceeds liquid pool assets | Apply liquidity gates and fair allocation |
| ADV-138 | Rounding residue accumulates in unitized pool | Reconcile explicit residual accounts |
| ADV-139 | Restricted gift used for incompatible grant | Enforce donor restrictions and sponsor authority |
| ADV-140 | Provider marks payment settled without cash reconciliation | Preserve uncertain finality and reconciliation exception |

## Open decisions and evidence gates

1. Obtain sponsor CFO/CPA accounting policies for contribution revenue, grant expense/payable recognition, donor restrictions, unitization and AUM calculations. Do not infer universal US GAAP entries from workflow status.
2. Decide whether pooled vehicles are unitized, segregated or mixed, with custody/legal validation and unit rounding rules.
3. Define fair valuation policies for marketable and nonmarketable assets, NAV timing, stale-price haircuts, price challenges and retroactive corrections.
4. Define fees including AUM-based, manual paper check, mailing, investment-management and sponsor fees, with fee priority and disclosure.
5. Define liquidity and fund capacity under multi-currency, settlement delay, returns, donor fund closure and adverse-market scenarios.
6. Establish reconciliations, close calendar, exception ownership, independence, materiality thresholds and audit evidence.
7. Define AUM measurement for sponsor revenue, fee billing, performance reporting and legal reporting separately; a single number may be misleading.
8. Specify conceptual journal examples only after accounting treatment is independently validated. Do not prematurely fix chart of accounts or DB schema.

**Status:** 12 proposed accounting invariants, seven reconciliation classes and ADV-117..ADV-140. No tests executed, no accounting certification.

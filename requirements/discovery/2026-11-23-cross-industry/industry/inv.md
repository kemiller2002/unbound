# Cross-industry discovery: investment and wealth management (B), accounting / GL / fund accounting (C), fund administration (M partial). Codes: INV, ACC

Date: 2026-11-23. Baseline: `777a70d`. Synthetic data only. Nothing in this report is an accounting opinion or legal advice.

**Outputs:**
- 52 atlas rows
- 24 findings
- 18 new requirements
- 4 refinements (RR-ACC-MDL-007-r1, RR-UNB-ACC-033-r1, RR-UNB-ACC-031-r1, RR-UNB-INV-019-r1)
- 22 scenarios
- 8 rejections
- 17 failure modes
- 8 decisions
- 2 conflicts

## 0. Existing coverage (not duplicated)

**Original requirements:**
- Accounting: UNB-ACC-001..036, ACC-MDL-001..012, the REQ-X-084 journals and REQ-X-040 (close).
- Investments: UNB-INV-001..042, UNB-NAV-001..053 and UNB-CUST-001..028.

**Prior reconciliation records:**
- UNB-RCN-FIN-001..015 cover acknowledgment, Forms 8283 and 8282, §4943, FeeBase, balancing segments, funding states, capacity, omnibus attribution, advisor-compensation screening, crypto timestamps, completed-gift date, and no refunds.
- Refinements: RR-UNB-ACC-002/006/012/018/026, RR-UNB-NAV-033, RR-UNB-INV-002/012, RR-UNB-CUST-015 and RR-REQ-X-060-r1.
- Decisions: RCD-017..021 and RCD-024. OD-19 is still open.

**This pass adds four things:**
- ASC 958 presentation mechanics.
- Authority of posted journals over derived views.
- The securities-law structure of pooled and advised assets.
- Operating controls for wealth management.

## 1. Investment and wealth management

| Topic | Industry norm | DAF reality | Verdict |
|---|---|---|---|
| Client of record | Account owner signs the advisory agreement | The **sponsor** is the client; the donor only recommends | Transfers, with the roles inverted (UNB-XIN-INV-001) |
| Advisor fees | Custodian debits the account from the advisor's billing file, unchecked | Charitable assets are debited. Fidelity CIAP adviser fees ≈26–27 bps; observed range 0–2% (daf-landscape:research/DAF-INDUSTRY-REPORT-2026-11.md:730, :566) | **Gap** (UNB-XIN-INV-001: recompute each period from the mandate and the valuation snapshot; match to the debit; hold and refund on a mismatch) |
| Regulatory status | Adviser registered; pool registered or excluded | Investment Company Act §3(c)(10)(B) excludes only pools "maintained by a charitable organization". Advisers Act §203(b)(4) does not cover a for-profit operator (law.cornell.edu 15 USC 80a-3, 80b-3) | **P0 gap** (UNB-XIN-INV-002: record the legal basis per pool). RR-UNB-INV-019-r1 limits AI portfolio recommendations to approved models until counsel decides. |
| Tax lots | Core function | Needed only for Form 8282, §4943 and UBTI. §512(e) S-corporation gains require the donor's carryover basis | Simplify (RR-UNB-ACC-031-r1; XRJ-INV-004) |
| Performance reporting | GIPS; TWR/MWR per account | Pool TWR against a benchmark is enough | Simplify (XRJ-INV-002/005) |
| Order management | Asset manager generates trades | The outsourced CIO or manager trades; the platform monitors drift | Over-scoped (XIF-INV-011) |
| Proxy votes and elections | Manager votes under a policy | The sponsor holds title; a donor directing votes is a red flag | Gap (UNB-XIN-INV-003) |
| Class actions | Allocated to historical holders | No attribution by holding period | Gap, P3 (UNB-XIN-INV-004) |
| Cash sweep | Sweep economics disclosed | Float and sweep income have no owner; one audited sponsor held $13.8m above the FDIC limit | Gap (UNB-XIN-INV-006) |
| Alternative assets | NAV reported on a lag and rolled forward | One audited sponsor uses a 1–3 month lag, with NAV-measured holdings outside the fair-value hierarchy | Gap (UNB-XIN-INV-005, UNB-XIN-ACC-007) |
| Crypto | Fair value | ASU 2023-08 applies to fiscal years beginning after 2024-12-15 (secondary source) | Gap, P2 (UNB-XIN-INV-007) |

**Investment profiles.** Do not assume every sponsor manages its own assets.

| Profile | Platform scope |
|---|---|
| Outsourced CIO or external manager (default) | Ingest NAV and positions, run the unit register, reconcile, monitor drift. No orders. |
| Advisor-managed separate accounts | Record the mandate, verify fee debits, attribute lots, handle elections. |
| Custodian-held cash only | Sweep and liquidity only. |
| Self-managed | Everything above, plus pre-trade checks. |

## 2. Accounting, GL and fund accounting

1. **DAF net assets.** DAF assets are presented as without donor restrictions because "the Foundation has been granted variance power" (Colorado Gives Foundation FY2025 audit, Note 2). The platform records no variance-power evidence for each program version. Fix: RR-UNB-ACC-033-r1.
2. **Grant recognition.** There are three reference points:
   - ASC 720-25-25-1: contributions made are expensed in the period made.
   - ASU 2018-08: a promise is conditional only if it has a barrier plus a right of return; an ambiguous promise is presumed conditional.
   - Observed practice: unconditional grants are expensed "when they are approved".

   Fix: UNB-XIN-ACC-002 turns OD-19 into a sponsor parameter, with evidence checks, a ConditionalGrant state and a cutoff register.
3. **Agency funds are liabilities.** The same audit shows this. Fix: UNB-XIN-ACC-001.
4. **Schedule D Part I.**
   - Line 3 includes transfers within the organization, and no valuation basis is given for line 4.
   - In landscape data, 44 sponsor-years break from net assets.
   - One return understates net assets by $12.2m against its audit (DAF-INDUSTRY-REPORT:1093, :730).
   - Fix: UNB-XIN-ACC-006 builds a bridge with typed reconciling items. An unexplained difference blocks sign-off.
5. **Disclosure data** is required: fair-value level, the NAV practical expedient, a liquidity table, and investment expense (netted under GAAP, added back on the 990). Fix: UNB-XIN-ACC-007.

### Adversarial check: where a derived projection would give the auditor a different answer from the posted ledger

| # | Mechanism | Permitting text | Fix |
|---|---|---|---|
| 1 | Idempotency key includes the posting-rule version, so a replay after a rule change posts a second journal | ACC-MDL-007 | RR-ACC-MDL-007-r1: key by (event, book). P0. |
| 2 | A "deterministic rebuild" regenerates journals for closed years under current code | UNB-NAV-044, UNB-IMM-005, UNB-ACC-036 | UNB-XIN-ACC-003: re-derivation is a check only |
| 3 | A rule change is applied silently to history | ACC-MDL-010 is silent on this | UNB-XIN-ACC-004: register classifying each change as principle, estimate or error |
| 4 | An effective-date view differs from the posting period | Largely covered (REQ-X-040) | Name each view: as-reported or as-corrected |
| 5 | An audit adjustment is made after statements are issued | UNB-ACC-017 | UNB-XIN-ACC-005 |
| 6 | Fund statements, Schedule D and GAAP totals differ | ACC-MDL-004 | UNB-XIN-ACC-006 |
| 7 | NAV corrections, rounding, FX | Covered (UNB-NAV-042, UNB-NAV-029) | — |

**Verdict.** Events, interpretations and reports are separated conceptually, but posted journals are not yet authoritative against re-derivation. Immutable events reduce reconciliation only if all three hold:
- journals are persisted and keyed by (event, book);
- re-derivation is used only to compare;
- every report pins a snapshot and an asset measure (UNB-XIN-INV-008).

## 3. Fund administration and transfer agency (M, partial)

The NAV unit register is the transfer-agency analogue, but the analogy breaks: there are no external unitholders, no KYC and no 1099s. Investor-servicing capabilities are therefore rejected (XRJ-INV-006). What transfers:
- book-of-record designation and a shadow mode (UNB-XIN-ACC-010);
- a SOC 1 Type II report with a user-entity control list (UNB-XIN-ACC-009). TVC-027 and GAP-016 flagged this need, but no requirement existed.

## 4. Workflow redesigns

| Workflow | Conventional | Existing Unbound | Redesigned |
|---|---|---|---|
| Year-end grant recognition | Manual accrual by the controller | Left open by OD-19 | One sponsor-chosen recognition point with evidence checks and an automatic cutoff register. Exceptions: conditional grants and failed communications. Removes the manual accrual. |
| Advisor fee | Billing file, then debit; rarely reviewed | Fee cap plus relationship screen | Recompute from the mandate and the valuation snapshot each period; match to the debit; on an exception, hold and request a refund. Deterministic. |
| Event → journal → report | Subledger interface plus manual reconciliation | Rebuildable projections | Persisted journals keyed by (event, book); re-derivation sampled monthly and run in full at year end, as a control; a change register; every report pins its snapshot. |
| Form 990 / Schedule D | Preparer rebuilds from GL exports | Package built from the locked ledger | Bridge with typed reconciling items; an unexplained difference blocks sign-off. |
| Alternative-asset valuation | Spreadsheet | Staleness limits only | Formula roll-forward plus a recorded check for material later events; stale values marked provisional and haircut. |

## 5. Automation

| Mode | Applies to |
|---|---|
| Deterministic | Recognition, posting, bridges, fee recomputation, sweep allocation, roll-forward |
| AI may flag; a human or CPA decides | Variance power, agency status, conditional-grant classification |
| AI extracts; a human reviews | K-1s and general-partner statements |
| Human or manager decides | Proxy votes and corporate-action elections |
| AI restricted to approved models pending counsel | Portfolio recommendations (a legal limit, not a reliability one) |

## 6. Business-model applicability

| Requirement | Applies to |
|---|---|
| UNB-XIN-ACC-001 | Institutional, embedded, high-volume-low-balance, hybrid |
| UNB-XIN-ACC-002..007 | All models |
| UNB-XIN-ACC-008 (UBTI) | Advisor-distribution, institutional, investment-centric, HNW-family-office, hybrid |
| UNB-XIN-ACC-009 (SOC 1) | White-label, institutional, outsourced-admin, embedded |
| UNB-XIN-ACC-010 (book of record) | White-label, institutional, outsourced-admin, investment-centric, hybrid |
| UNB-XIN-INV-001 | Advisor-distribution, investment-centric, HNW-family-office, hybrid |
| UNB-XIN-INV-002 | White-label, institutional, outsourced-admin, embedded, investment-centric |
| UNB-XIN-INV-003 | Advisor-distribution, institutional, investment-centric, HNW-family-office |
| UNB-XIN-INV-005 | Institutional, investment-centric, HNW-family-office, hybrid |
| UNB-XIN-INV-006 | All models |
| UNB-XIN-INV-007 | D2C, investment-centric, HNW-family-office |

## 7. Cost drivers for the economics model

| Driver | Value | Source |
|---|---|---|
| Sponsor admin fee | 60/30/20/15 bps tiers; minimum $100 (Fidelity) to $250 (RCF). Advisor-managed: 65/35/25/20 bps (RCF) | daf-landscape:research/daf-economics-and-distribution-2026-11-11.md:64-66 |
| Pool expense ratio | 26.3 bps (Fidelity FY2025) | DAF-INDUSTRY-REPORT:730 |
| Advisor fees | ≈26–27 bps realized at CIAP; 0–2% observed; caps of 1.00% | DAF-INDUSTRY-REPORT:730, :124, :566 |
| Investment expense borne by the fund | 40–46 bps; $1,876–2,254 per account (AEF) | DAF-INDUSTRY-REPORT:726 |
| Platform service fee | ≈30 bps up to $1m; 10–20 bps above | daf-economics:38-40 |
| DAF operating cost | 67–89 bps market-wide; nationals 25–32 bps; community foundations 127–177 bps | DAF-INDUSTRY-REPORT:392-393 |
| Custody | 0–5 bps, or $0–100 per account | Illustrative |
| Trading | ≈$0 for listed securities | Illustrative |
| Fund administration / NAV | 1–5 bps | Illustrative |
| Illiquid-asset valuation | $2k–25k per appraisal | Illustrative |
| SOC 1 | Low six figures per year | Illustrative |

Suggested model units: bps of pool assets, per account, per trade, per close exception, per illiquid holding per year, and fixed per entity per year.

## 8. Authoritative vs derived accounting events (input to event architecture)

**Authoritative: persisted, append-only, never regenerated.**
- External evidence: custody, bank and rail observations, general-partner statements, and prices as received.
- Decisions: acceptance; grant approval and communication; condition met; mandate approval; elections; valuation approval and NAV publication.
- Posted journals, keyed by (event, book) and carrying the rule version.
- Unit issues and redemptions at the struck NAV.
- Accounting-change records and audit-adjustment findings.
- Close snapshots and issued document versions.
- Book-of-record designations, account-type treatment and variance-power evidence.
- Fee-debit matches.

**Derived: rebuildable, and never authorizing anything.**
- Trial balance and statements, built from posted journals plus a pinned snapshot.
- Fund views and pre-issuance statements.
- AUM measures.
- The Schedule D package and bridge.
- Performance, drift and liquidity forecasts.
- Disclosure tables.
- Re-derivation comparisons.
- UBIT estimates. These reduce capacity only through a recorded hold.

**Capacity** is computed inside the reservation transaction from authoritative records (UNB-RCN-FIN-009).

**Rule:** once a journal an auditor tests is posted, it is authoritative. Re-running the interpretation that produced it is a check, not a source.

## 9. Evidence not obtained

- National sponsors' audit policy notes on recognition and net-asset classification.
- Schedule D Part I lines 5–6 instructions.
- Whether UPMIFA applies to DAF assets.
- Any SEC position on DAF technology operators as investment advisers. The risk stated above is inferred from the text of §203(b)(4).
- Market prices for custody, trading, fund administration, appraisal and SOC 1.
- Rule 144 detail.
- Direct reads of ASC 958-605-25-25, ASC 250 and ASC 820.
- The T+1 settlement date and ASU 2023-08 were confirmed from search summaries and secondary sources only.

## Sources (accessed 2026-11-23)

- FASB ASU 2018-08: https://storage.fasb.org/ASU_2018-08.pdf
- Colorado Gives Foundation FY2025 audit: https://coloradogivesfoundation.org/wp-content/uploads/2026/05/Colorado-Gives-Foundation-December-31-2025-Audited-Financial-Statement-Final.pdf
- 15 USC 80a-3: https://www.law.cornell.edu/uscode/text/15/80a-3
- 15 USC 80b-3: https://www.law.cornell.edu/uscode/text/15/80b-3
- 26 USC 512: https://www.law.cornell.edu/uscode/text/26/512
- IRS Schedule D instructions: https://www.irs.gov/instructions/i990sd (only lines 1–4 returned)
- SEC T+1 FAQ (search summary)
- Deloitte on ASU 2023-08 (secondary source)

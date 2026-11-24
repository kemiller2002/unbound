# Deferred outbound payments and remaining enterprise requirements
Date: 2026-11-21 | Status: requirements planning only

## Explicit deferral: outbound payments
**Decision PAY-DEF-001:** Outbound grant payment execution SHALL be supported by the eventual Unbound enterprise, but selection of payment rails, banking partners, execution adapters and production payment sending is intentionally deferred. Do not mistake deferral for omission.

- **UNB-PAY-001:** Model grant sponsor approval, accounting recognition, payment obligation, payment instruction, payment authorization, submission, acceptance, settlement, return, void, recovery and reconciliation as separate events.
- **UNB-PAY-002:** Define a provider-neutral payment API and lifecycle, accommodating ACH, wires, checks and other approved rails subject to eligibility, geography, contracts and sponsor policy.
- **UNB-PAY-003:** No grant recommendation, AI proposal or internal accounting journal directly authorizes outbound funds. Enforce sponsor reserved authority, independent release control, payee verification and least privilege.
- **UNB-PAY-004:** Require idempotent external instructions, replay defense, duplicate detection, settlement evidence, reconciliation, safe retry and explicit uncertain-result handling.
- **UNB-PAY-005:** Record immutable payee identity, bank detail change controls, sanctions/eligibility freshness, approval evidence, payment status and correction/recovery history, with appropriate secrets/privacy protection.
- **UNB-PAY-006:** Support bank/rail outages, failed transfers, returned ACH, lost/voided checks, partial payments, payment holds, escheat/unclaimed property applicability and fraud escalation.
> Adopted 2026-11-23: refined by `RR-UNB-PAY-006-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-PAY-007:** Provide a simulated payment adapter and deterministic contract tests for grant/accounting integration before any real funds can move.
- **UNB-PAY-008:** Payment provider selection, sponsor bank relationships, funds-flow legal review, rail eligibility, limits, fraud operations, security certification and production authorization are explicit future decision gates.
- **UNB-PAY-009:** Forma SHALL display pending/approved/sent/settled/failed/returned states without conflating them. Folio SHALL generate payment advice, remittance notices and reconciliation/audit packs.
- **UNB-PAY-010:** Payment events SHALL reconcile grant payable, sponsor cash, custodian liquidity and bank confirmation, without claiming a payment is complete before external confirmation.

**Deferral boundary:** Now define payment domain contracts, statuses, control requirements, mock provider, test cases and integration seams. Later verify provider entitlements, select rail, build adapter, complete security and legal review, and authorize production sending. No provider selected and no live payments authorized.

## Other priority gaps to flesh out
- **UNB-REM-001 Charity master and grantee identity:** canonical charity identity, legal name/EIN or jurisdictional equivalent, aliases, merger/dissolution, eligibility evidence, sanctions, payment beneficiary and historic status.
- **UNB-REM-002 Charity lifecycle:** grantee onboarding, changes, acknowledgments, restricted-use reporting, communication, dispute and fraud/impersonation protection.
- **UNB-REM-003 Incoming contributions:** cash, stock, in-kind, illiquid, rejected, unidentified, conditional and cross-currency gifts; independent donor tax acknowledgment versus accounting recognition.
- **UNB-REM-004 Sponsor operations:** governance committees, board authority, delegated officers, service teams, finance, compliance, internal control ownership and independent reviews.
- **UNB-REM-005 Communications:** preference center, secure message delivery, accessible content, notices, receipts, opt-outs, bounce handling and immutable delivery evidence.
- **UNB-REM-006 Cases and disputes:** support tickets, privacy inquiries, succession disputes, grant reversals, beneficiary change, complaints, fraud, appeals, escalation and resolution evidence.
- **UNB-REM-007 Sponsor policy and fees:** effective-dated policies, investment menus, fees, waivers, exceptions, approval and rollback-by-supersession.
- **UNB-REM-008 Data migration and portability:** legacy DAF imports, source provenance, historical accounting, investment lots, restrictions, donor intent, advisors, successors, reconciliation, tenant migration and exit/export.
- **UNB-REM-009 Donor intent and strategy:** versioned philanthropic goals, geographic/mission preferences, restrictions, strategy change, impact evaluation and sponsor authority; avoid implying advisory wishes legally bind sponsor.
- **UNB-REM-010 Financial close and assurance:** daily/monthly/yearly reconciliation, audit, statement reproducibility, fraud controls and independent source verification.
- **UNB-REM-011 Organization administration:** billing, contracts, partner onboarding, procurement, vendor risk, staffing, continuity and performance.
- **UNB-REM-012 Tax/regulatory source crosswalk:** row-level verification of source obligations, applicability, control ownership, evidence and counsel review.
- **UNB-REM-013 Beneficiary experience:** charities can safely confirm organizational identity, grant status and restrictions, without seeing donor-private information without authority.
- **UNB-REM-014 Cross-border and geographic scope:** define permitted jurisdictions, grantee eligibility, FX, tax, sanctions, privacy and payment rules; no implied universal support.
- **UNB-REM-015 Financial integrity of grant funding:** simultaneous multi-investment redemptions, fee/FX changes, stale NAV, partial settlement, authorization expiry and source-of-funds provenance.

## Recommended sequence
1. Charity master/eligibility and recipient lifecycle.
2. Contributions and noncash gift acceptance.
3. Sponsor operations, identity and policy authority.
4. Communication, case management and donor/charity experience.
5. Financial close, data migration, tax/regulatory evidence and full traceability.
6. Payment sending implementation only after separately approved gate.

## Acceptance of this note
This document reserves the future payment capability while expressly prohibiting premature provider commitment or live execution. It is a requirements artifact, not a completed integration, legal review or payment implementation.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-PAY-006-r1 (refines `UNB-PAY-006`) — adopted-provisional
- **Original:** `requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md:12` @ `777a70d` (kept verbatim) · **Decisions:** RCD-028
- **Requirement:** The payment model MUST define explicit handling for rail outage, failure, ACH return, lost, voided and stale checks, partial payments, holds, Blocked, and unclaimed-property disposition, with states, ledger effects, configured thresholds and owners, as specified in RR-REQ-X-083-r1 and UNB-RCN-GRT-005.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence
- **Acceptance criteria:**
  - AC-1: Given a check outstanding for longer than the configured stale threshold; when the aging job runs; then StaleDated with outreach case.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-PAY-006-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-GRT-005 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-012 · **Parent:** UNB-PAY-006 · **Decisions:** RCD-028, RCD-047
- **Requirement:** The check rail MUST model the states Issued, Outstanding, AgedOutreach (at configurable day thresholds), StaleDated, VoidRequested, VoidConfirmed (positive-pay void acknowledged), Cleared, and UnclaimedPropertyReportable. Each state MUST have a defined grant, reservation and ledger effect. Reissue requires VoidConfirmed, verified recipient eligibility and destination, and a linked replacement chain. When a check reaches the jurisdiction-specific dormancy rule, the system MUST either produce unclaimed-property report and remittance evidence or record the counsel-approved alternative disposition. It MUST NOT silently release the funds to the fund.
- **Validation still required:** RCD-028: Counsel Q11; rail partner due diligence; RCD-047: Counsel per state
- **Acceptance criteria:**
  - AC-1: Given a check outstanding past the AgedOutreach threshold; when the daily aging job runs; then an outreach case is opened for the recipient and the grant stays Outstanding (not Paid).
  - AC-2: Given a voided check is later presented; when the positive-pay exception arrives; then it is rejected and no ledger effect occurs.
  - AC-3: Given a dormant outstanding check; when the dormancy threshold is reached; then the item becomes UnclaimedPropertyReportable with the jurisdiction recorded.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-005); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-PAY-006:** For every processor-settled contribution the system MUST record gross amount charged to the donor (including any donor-elected fee coverage), the processor fee actually assessed, and net settled, linked to the payout batch and bank credit; the contribution credited to the fund and stated on the acknowledgment MUST be determined by a CPA/counsel-approved FeeCoveragePolicy (default: gross charged is the contribution; processor fee posted as sponsor or fund expense per policy); and a payout batch MUST NOT reconcile unless Σgross − Σfees − Σreversals = net payout = bank credit. Default FeeCoveragePolicy (XID-BNK-002): the gross amount charged is the contribution and the acknowledged amount; the fund is credited gross and the processor fee posts as a separate fund expense line.
  - Provenance: [UNB-XIN-PAY-006](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given a $100 gift with donor-covered fee of $3.20 and actual fee $3.20; when settlement arrives net $100; then fund credit and acknowledgment show $103.20 under default policy and fee expense $3.20 posts.
    - AC-2: Given a payout batch where Σgross−Σfees ≠ net; when reconciliation runs; then the batch stays ExceptionOpen with the delta and no gift is marked Settled from that batch.
    - AC-D1: Given a donor gives $100 and elects to cover a $3.20 fee, paying $103.20; when the contribution settles; then the acknowledgment states $103.20, the fund is credited $103.20 and a $3.20 processing-fee expense line posts to the fund.
- **UNB-XIN-BNK-001:** The system MUST maintain a versioned RailObligation register with one row per (rail, direction, sponsor role ∈ {Originator, ThirdPartySender, Payee, Merchant, Holder, BankCustomer}, obligation), each carrying source (statute, regulation, network rule, bank contract, sponsor policy), citation, effective date, binding class, linked control ID, owner and counsel status; the PAY-DEF-001 production gate for a rail MUST fail while any binding row for that rail and role lacks an implemented, tested control. Default roles (XID-BNK-005): the sponsor is Originator of record on sponsor-titled accounts under its own ODFI agreement; the platform operator is a Third-Party Service Provider, never a Third-Party Sender, and holds no funds.
  - Provenance: [UNB-XIN-BNK-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given the ACH-credit rail with role Originator and a binding fraud-monitoring row lacking a control; when production enablement for ACH is requested; then enablement is refused citing the row.
    - AC-2: Given a row with counsel status 'unreviewed'; when the register is published; then the row is shown as 'not yet confirmed' and is not presented as legally required.
    - AC-3: Given Nacha amends a rule with a new effective date; when the row is versioned; then both versions are retained and the control review task is due before the effective date.
    - AC-D1: Given a deployment configuration naming the operator as Third-Party Sender; when the rail is activated; then activation fails with OPERATOR_ROLE_NOT_PERMITTED.
- **UNB-XIN-BNK-003:** Every business day the system MUST ingest bank activity for each sponsor disbursement, clearing and operating account linked to Unbound and match every debit to exactly one authorized platform instruction or to a configured expected-debit class (bank fee, sweep, return); any unmatched debit MUST open a high-severity incident within one hour of ingestion, notify two independent sponsor roles and place a protective hold on further releases from that account until an authorized human classifies it; ACH debit block or filter status for disbursement accounts MUST be recorded and attested quarterly. Grant disbursement accounts MUST carry an ACH debit block allowing only named bank-fee company IDs; processors MUST settle to a separate contribution clearing account where only the processor's company ID may debit (XID-BNK-008).
  - Provenance: [UNB-XIN-BNK-003](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-008 (decided-provisional; confirmations in report 16).
  - Revision proposed 2026-11-24 (QR-0070): the text above is kept verbatim; the proposed revised statement is in `requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl` (`revisions` of UNB-XIN-BNK-003). Reason: Prior-day feed alone insufficient for spendable capacity; require reservation and freshness controls appropriate to rail.
  - Acceptance:
    - AC-1: Given a $48,000 wire debited that has no platform instruction; when bank activity is ingested; then an incident opens, releases from that account are held, and two roles are notified.
    - AC-2: Given a monthly bank service charge; when it is ingested; then it matches the expected-debit class and no incident opens.
    - AC-3: Given bank activity feed missing for a business day; when the cut-off passes; then a FeedMissing exception opens and the account is marked Unverified.
    - AC-D1: Given a processor chargeback debit; when it is presented; then it posts only against the contribution clearing account and any attempt against a disbursement account is returned by the bank block.
- **UNB-XIN-BNK-007:** The system MUST maintain a counsel-approved UnclaimedPropertyRule table per state {owner type (organization/individual), property type (vendor/grant/refund check), dormancy period, business-to-business or charitable exemption flag, due-diligence notice window and content, report and remittance dates}, MUST determine the state by the payee's last known address (with the sponsor's state of incorporation as fallback, per counsel), MUST send due-diligence outreach before StaleDated items become reportable, and MUST either produce report and remittance evidence or record the counsel-approved alternative disposition (e.g., exemption) for each item; an item with no applicable rule row MUST remain in UnclaimedPropertyReview and MUST NOT be re-credited to a fund. Outstanding uncashed grant checks MUST be presumed reportable property owed to the payee charity; before dormancy the system MUST make two outreach attempts including an offer to reissue by ACH; void-and-re-credit to the fund is permitted only if the grant was cancelled before cashing with notice to the charity and the state row records that cancellation defeats the holder obligation (XID-BNK-003).
  - Provenance: [UNB-XIN-BNK-007](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given a stale grant check to a charity in a state whose row marks B2B exemption applicable; when dormancy is reached; then the item records disposition 'Exempt' with rule version and is not reported.
    - AC-2: Given an item for a state with no rule row; when dormancy date passes; then the item stays UnclaimedPropertyReview and a counsel task opens; fund capacity is unchanged.
    - AC-D1: Given a stale grant check in a state whose row records no exemption; when the sponsor attempts to void and re-credit the fund; then the action is rejected and the item proceeds to reporting after due diligence.
- **UNB-XIN-NPG-011:** For every check reaching AgedOutreach, the system MUST determine the reporting jurisdiction by a counsel-approved priority rule (default: state of the payee's last known address in the sponsor's records; if none or foreign, the sponsor's state of incorporation), look up that state's dormancy period, due-diligence-letter window, reporting deadline and any business-to-business or charitable exemption in a versioned UnclaimedPropertyRuleTable, and schedule the due-diligence letter and report accordingly. Where the table records an applicable exemption, the system MUST record the exemption citation and follow the counsel-approved alternative disposition. The priority rule is fixed by XID-BNK-003 (payee's last-known address state, else sponsor state of incorporation).
  - Provenance: [UNB-XIN-NPG-011](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-003 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given check to a charity with Ohio address; when AgedOutreach reached; then jurisdiction Ohio; Ohio rule row applied.
    - AC-2: Given payee address missing; when AgedOutreach; then jurisdiction = sponsor's state of incorporation.
    - AC-3: Given state row marks B2B exemption applicable; when dormancy reached; then exemption recorded; alternative disposition per counsel.
    - AC-D1: Given a check payable to a charity with a last-known address in Ohio, sponsor incorporated in Delaware; when jurisdiction is determined; then Ohio is selected.
- **RR-TVR-CON-001-r1:** Contribution provisional status MUST be governed by a versioned ReversalWindow table keyed by (rail, SEC code or card network, account type consumer/business, direction push/pull) with fields availability delay, basis (settlement date), source citation and owner. Contributions MUST be recognized as revenue when funds settle to the sponsor; reversibility affects grantable availability, not recognition. Default availability: ACH consumer debit 5 banking days after settlement; ACH business (CCD) debit 3 banking days; card on settlement up to the donor's provisional limit, remainder 60 calendar days after settlement; wires, donor push credits and cleared checks on settlement. The per-donor provisional limit defaults to $0 for donors with less than 90 days of history and $10,000 thereafter (illustrative, sponsor-configurable). A return or chargeback after acceptance MUST reverse contribution revenue against the fund up to the fund's available balance; any shortfall MUST post as sponsor operating loss and MUST NOT create a receivable from the donor. A missing table row MUST fail closed as Provisional.
  - Provenance: [RR-TVR-CON-001-r1](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-006 (decided-provisional; confirmations in report 16). Refines **TVR-CON-001** (`research/daf-theoretical-enterprise-validation/16-registers/requirements.csv:10`), which is retained unchanged.
  - Acceptance:
    - AC-1: Given a $5,000 CCD debit from a business account; when 3 banking days pass with no return; then the contribution becomes Final and fully grantable.
    - AC-2: Given a $5,000 PPD debit from a consumer account and OD-04 limit 0; when day 30; then the value remains Provisional and not grantable.
    - AC-3: Given a $5,000 wire contribution; when it is received; then it is Final at receipt.
    - AC-D1: Given a $2,000 card gift from a donor with 30 days of history; when it settles; then revenue of $2,000 is recognized and $0 is grantable until 60 days after settlement.
    - AC-D2: Given a $5,000 ACH gift fully granted out, then an R10 return 40 days later on a fund with $1,000 available; when the return posts; then contribution revenue is reversed by $5,000, the fund is reduced by $1,000 and $4,000 posts as sponsor operating loss; no donor receivable exists.
<!-- END ADOPTED: cross-industry-discovery -->

# 16 — Decisions on the 41 legal and accounting questions

**Date:** 2026-11-24.

**Status:** `decided-provisional`, all 41.

**Who decided:** the integrator, at the product owner's explicit direction ("look through the accounting rules and decide 41 questions").

**What these decisions are, and are not.** They are product and policy decisions about how the platform will behave by default. They are **not legal or accounting opinions**. Each one records:
- its rationale;
- the authority it relied on, separating what was read from what is general knowledge;
- a confidence level;
- the **confirmation it still needs** from counsel, an external CPA or the auditor before production use.

They do not change the reconciliation's requirements gate. The hard boundary in PRINCIPLES.md still applies: synthetic data only.

## Method

1. **Read the accounting rules already in the repository.**
   - `requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md` (ACC-MDL-001..012, event-to-journal matrix, ACC-ADR register).
   - `requirements/FULL-ACCOUNTING-SYSTEM.md` (UNB-ACC-001..036).
   - Owner decisions OD-04 and OD-19.
   - Reconciliation decisions RCD-017..021.
   - The research passes' accounting findings.
2. **Applied the governing standards.** ASC 958 (promises to give, agency transfers, variance power), ASU 2018-08, ASC 250, ASC 740, ASC 855 and ASC 958-720. These rest on secondary knowledge and the ASU text; the FASB Codification itself was not read.
3. **Verified the statutory or regulatory text where a decision depended on it** (read 2026-11-24):
   - IRC §4966(c)(2) and §4962;
   - 47 CFR 64.1200;
   - FINRA Rule 2165;
   - Rev. Proc. 2017-53.
4. **Chose the conservative option where the law is uncertain**, and gated features behind counsel opinions instead of guessing.

## What verification changed

- **§4962 abatement.** §4962(b) defines "qualified first tier tax" to include subchapter G. That means §4966 and §4967 taxes can be abated when the event had reasonable cause and is corrected. The incident workflow (UNB-XIN-NPG-008) must therefore capture reasonable-cause evidence and correction steps. Without the verification, this would have been decided the other way.
- **TCPA nonprofit standard.** The tax-exempt nonprofit standard in 47 CFR 64.1200 covers calls made "by or on behalf of" the nonprofit. So the operator, acting as the sponsor's agent, needs prior express consent, not prior express written consent. A for-profit partner's own marketing does not qualify.
- **The research pass's card default was overruled.** It would have held every card gift for 120 days. The decision instead recognizes card gifts on settlement and contains reversal risk with per-donor exposure limits (XID-BNK-006).
- **SponsorApproval removed as a recognition policy.** An approval nobody has communicated to the charity is an intention, not a promise to give (XID-ADV-006).

## Summary

<!-- BEGIN GENERATED: outcome-summary -->
| Type | high | medium | medium-high | Total |
|---|---|---|---|---|
| accounting | 5 | 6 | 1 | 12 |
| legal | 7 | 14 | 8 | 29 |
<!-- END GENERATED: outcome-summary -->

## Confirmations still required before production

| Confirmation | Decisions |
|---|---|
| **Gating:** the feature stays off until the opinion is on file | XID-REG-001 (multistate money transmission), XID-SAS-005 (California platform, D2C app and charity-side button only), XID-INV-003 and XID-INV-004 (securities counsel), XID-SVC-002 (recording remains off) |
| External CPA or auditor | XID-ADV-006, XID-INV-001, XID-INV-002, XID-INV-005, XID-INV-006, XID-EVA-002, XID-ADV-005, XID-ECO-005 |
| Counsel: wording or table completion | XID-BNK-002, XID-BNK-003, XID-BNK-005, XID-CLM-002, XID-CLM-003, XID-ERP-002, XID-CRM-002, XID-CRM-004, XID-TRS-001, XID-NPG-001, XID-NPG-002, XID-NPG-005, XID-NPG-007, XID-REG-002, XID-SAS-002, XID-REC-001, XID-ECO-002, XID-EVA-004, XID-EVA-005 |
| Owner or treasury only | XID-ERP-001, XID-ECO-003, XID-EVA-003, XID-BNK-006, XID-BNK-008, XID-CRM-001, XID-REG-003, XID-NPG-003, XID-NPG-006 |

**Translated into requirements (2026-11-24).** All 41 decisions have been applied to the requirements they affect:
- 45 mission records were revised;
- one new refinement of a baseline requirement was added: RR-REQ-X-069-r1, which makes outcome monitoring of public-charity grants optional (XID-NPG-006).

Each revised record carries:
- the decided rule in its `statement`;
- its previous wording in `statement_as_proposed`;
- the decision IDs in `decisions_applied`;
- new `AC-D*` Given/When/Then criteria that test the decision.

Six new scenarios (XDS-DEC-001..006) exercise the most consequential changes:
- year-end cutoff;
- card reversal after granting;
- daily fee computation with accrual-aware capacity;
- excise abatement;
- late errors after issuance;
- gated features staying off.

**Adopted (2026-11-24).** At the owner's direction, these 46 records were adopted into the domain files:
- 38 as dated "Adopted requirements" sections in the owning file;
- 8 as inline "Refined … by RR-…" annotations under the originals.

The original text was kept everywhere, and each record now carries `adoption_status: adopted` and `adopted_into`. Because each change is provisional, its confirmation in the table above is still required before production.

## Requirements as revised

<!-- BEGIN GENERATED: applied -->
| Requirement | Decisions applied | Statement as revised |
|---|---|---|
| RR-REQ-X-069-r1 | XID-NPG-006 | Grant-outcome and restriction monitoring MUST be sponsor-configurable and OFF by default for grants to §170(b)(1)(A) organizations (other than disqualified supporting organizations), because such distributions are excepted from §4966 taxable distributions whatever their stated purpose (XID-NPG-006). Monitoring MUST be mandatory only for expenditure-responsibility grants (reports per §53.4945-5) and for grants the sponsor itself conditions. §4967 benefit screens apply to every grant regardless of monitoring settings. |
| RR-TVR-CON-001-r1 | XID-BNK-006 | Contribution provisional status MUST be governed by a versioned ReversalWindow table keyed by (rail, SEC code or card network, account type consumer/business, direction push/pull) with fields availability delay, basis (settlement date), source citation and owner. Contributions MUST be recognized as revenue when funds settle to the sponsor; reversibility affects grantable availability, not recognition. Default availability: ACH consumer debit 5 banking days after settlement; ACH business (CCD) debit 3 banking days; card on settlement up to the donor's provisional limit, remainder 60 calendar days after settlement; wires, donor push credits and cleared checks on settlement. The per-donor provisional limit defaults to $0 for donors with less than 90 days of history and $10,000 thereafter (illustrative, sponsor-configurable). A return or chargeback after acceptance MUST reverse contribution… |
| RR-UNB-ACC-014-r1 | XID-ECO-005 | Support budgeting, forecasts, actual-versus-budget reporting, cost centers and scenario models while distinguishing forecasts from posted actuals. Management cost allocation and activity-based attribution SHALL be computed as a non-posting projection (UNB-XIN-ECO-001) that never creates journals or changes fund balances or available-to-grant; only CPA-approved functional expense allocations (UNB-ACC-019) and approved fee/billing entries (UNB-ACC-010) may post. Every management allocation SHALL reconcile monthly to GL actuals with an explicit unallocated residual (UNB-XIN-ECO-004). Functional expense allocation and management attribution share one activity dataset with separately governed mappings (XID-ECO-005). |
| RR-UNB-ACC-033-r1 | XID-INV-002 | Each program document version and each fund-intake agreement (including legacy imports and sponsor-to-sponsor transfers-in) MUST record a CPA-reviewed VariancePowerEvidence (clause reference, effective date) or its absence. Net asset classification of contributions MUST be derived from it: with variance power and no donor-imposed purpose/time restriction -> without donor restrictions (DAF designation shown as an internal designation); with an explicit donor restriction -> with donor restrictions; without variance power and with a specified beneficiary -> evaluated under UNB-XIN-ACC-001 (agency/pass-through). A contribution whose agreement lacks recorded evidence MUST post to a suspense classification and open an exception before period close. Advisory privileges alone MUST NOT be treated as a donor restriction (XID-INV-002). |
| RR-UNB-DATA-005-r1 | XID-EVA-002 | The system MUST maintain a versioned correction impact registry that maps each correctable fact type to the derived records computed from it (e.g., contribution valuation -> acknowledgment amount, Form 8283/8282 obligations, unit purchase, fee basis; NAV -> unit allocations, fee accruals, statements; journal batch -> snapshots, statements, Schedule D aggregates, capacity; payee destination -> pending payment intents). When a correction (reversal, re-post, restatement) commits, the system MUST deterministically enumerate impacted derived records as of the correction's effective date, create a linked task or automatic recomputation per registry rule (automatic only for deterministic recomputations such as fee delta postings; human for legal documents such as acknowledgments), and MUST NOT mark the correction case closed until every impacted item is resolved or explicitly waived with autho… |
| RR-UNB-GRANT-024-r1 | XID-NPG-006 | Each grant MUST be assigned one GrantTermsTier by a counsel-approved deterministic rule: Tier 0 (routine grant to a Permitted recipient with no conditions) — no agreement; sponsor-standard award terms (charitable use, no goods or services to donor, return if not used for charitable purposes or if recipient status is lost before use) printed in remittance; Tier 1 (sponsor-imposed restriction, condition, milestone or multi-year schedule) — unilateral award letter recording the terms, with grantee acceptance optional unless the sponsor requires it; Tier 2 (ERRequired, or any grant counsel designates) — written commitment signed by an authorized grantee officer containing the counsel-approved §53.4945-5(b)(3) terms, captured electronically. Payment for Tier 2 MUST NOT be released before the signed commitment is recorded. Purpose amendments MUST create a new terms version and re-run the Bene… |
| RR-UNB-GRANT-041-r1 | XID-NPG-001 | A recommendation to a foreign recipient MUST be routed deterministically to exactly one InternationalRoute: (1) RoutineListed — recipient holds a US IRS determination as §170(b)(1)(A) organization; (2) EquivalencyDetermination — current ED per UNB-XIN-NPG-001; (3) ExpenditureResponsibility — sponsor offers ER and UNB-RCN-GRT-003 case completes; (4) Intermediary — donor accepts an IntermediaryCatalog alternative (UNB-XIN-NPG-002); (5) Decline. Routes 2 and 3 additionally require the ForeignNexusDeclaration (UNB-XIN-NPG-009), sanctions screening of the payment chain (UNB-RCN-GRT-007) and, where a program applies, a SanctionsAuthorization (UNB-XIN-NPG-010). Routes offered are sponsor configuration; route selection is never made by AI. Repository-sourced equivalency determinations follow XID-NPG-001. |
| RR-UNB-INV-019-r1 | XID-INV-004 | AI or rule-based features MAY compute drift, risk metrics, liquidity forecasts and conformity to sponsor-approved models, and MAY propose actions that implement a sponsor-approved model or committee-approved parameter. They MUST NOT generate individualized securities selections or allocation advice for a donor, advisor or sponsor beyond approved models unless counsel has recorded the Advisers Act status of the party providing it (sponsor personnel acting under §203(b)(4), a registered adviser under mandate, or another basis). Every proposal MUST name the approved model/parameter it implements and MUST pass deterministic mandate and pre-trade checks before any human approval. The operator MUST NOT provide individualized investment recommendations, and operator compensation MUST NOT vary with assets or trades (XID-INV-004). |
| RR-UNB-TEN-016-r1 | XID-REC-001 | On sponsor exit, Unbound MUST deliver within 30 days of ExitNoticed (or the contract period) an ExitPackage containing: the tenant's complete authoritative event history and current projections in the documented export schema, issued document artifacts with hashes, policy and configuration versions, audit and evidence records, and a manifest signed with SHA-256 digests plus record counts and ledger trial balances per fund and period that reconcile to the live system at the export watermark. The sponsor MUST be able to verify the package independently. After delivery the tenant MUST enter ReadOnlyRetention for the contractually agreed period, during which authorized sponsor users can query and export. Destruction MUST be performed by destroying tenant-scoped encryption keys under dual control after retention and hold checks, and Unbound MUST issue a destruction certificate stating what w… |
| UNB-XIN-ACC-001 | XID-INV-002, XID-CRM-004 | Every account type in the account-type register MUST carry a CPA-approved AccountingTreatment of exactly one of {Contribution-WithoutDonorRestriction, Contribution-WithDonorRestriction, AgencyLiability, PassThroughLiability}. Receipts into an AgencyLiability or PassThroughLiability account MUST post to a liability (funds held for others) and MUST NOT increase contribution revenue, DAF counts or Schedule D Part I aggregates; payouts from such accounts MUST reduce the liability and MUST NOT be recorded as grant expense. An account type without an approved AccountingTreatment MUST NOT accept receipts. Default treatments are fixed by XID-INV-002: a donor-advised account type whose program version records variance power maps to Contribution-WithoutDonorRestriction; an account type where a nonprofit transfers assets naming itself beneficiary without granting variance power maps to AgencyLiabi… |
| UNB-XIN-ACC-002 | XID-ADV-006, XID-INV-001 | Each sponsor book MUST declare one GrantRecognitionPoint from {ApprovalCommunicatedToGrantee (default), PaymentReleased}, effective-dated and CPA-approved; PaymentReleased MAY be declared only where payment is the first communication of the grant to the grantee. The platform MUST send the approval notice to the grantee automatically on sponsor approval, and the posting rule MUST recognize grant expense and grants payable for an unconditional grant only when the communication record (timestamp, channel, recipient) or, for PaymentReleased, the payment release ID exists. Sponsor approval without communication MUST NOT be recognized (an intention, not a promise to give). A grant whose terms contain both a barrier and a right of return or release, or whose terms are ambiguous, MUST be recorded as ConditionalGrant with no expense until a recorded BarrierMet or Waived event. At each period end… |
| UNB-XIN-ACC-005 | XID-ADV-005 | Each fiscal year MUST have an AuditAdjustment period after period 12 that accepts only journals linked to an AuditFinding (auditor reference, proposer, controller decision). Each auditor-proposed adjustment MUST be recorded as Booked or Passed with amount and rationale; Passed items MUST appear in a summary-of-unadjusted-differences report. Audited financial statements MUST be generated from period 12 plus AuditAdjustment journals, and fund statements already issued from period 12 MUST NOT be reissued unless a Booked adjustment changes a fund balance above the sponsor's restatement threshold, in which case a correction notice per REQ-X-040 MUST be produced. The AuditAdjustment period remains soft-closed until the audited statements are issued; after issuance it is hard-closed and late items follow UNB-XIN-EVA-002 (XID-ADV-005). |
| UNB-XIN-ACC-006 | XID-INV-005 | For each sponsor and fiscal year Unbound MUST produce a DAF bridge that reconciles (a) Schedule D Part I line 4 aggregate value, (b) the sum of year-end fund statement values, and (c) the DAF component of net assets in the audited statements, and similarly line 2 contributions and line 3 grants to the statement of activities, using only typed reconciling items: InternalTransfersAmongFunds, GrantsApprovedUnpaid, AccruedFeesUnpaid, UnsettledTrades, AgencyOrPassThroughExcluded, ValuationBasisDifference, TimingOfLateItems, Other(requires note). Any unexplained residual MUST block sign-off of the package. Line 4 MUST use the same fair-value measurement as the audited statements; line 3 MUST include all distributions from DAFs including transfers to other funds of the organization, on the grant recognition basis of UNB-XIN-ACC-002 (XID-INV-005). |
| UNB-XIN-ACC-008 | XID-INV-006 | At acceptance, every noncash asset MUST be classified UBTIExposure in {None, SCorpStock, PassThroughWithUBTI, DebtFinanced, Unknown}; SCorpStock acceptance MUST require a counsel-approved rule reference; for any exposure other than None, the donor's carryover basis and holding-period evidence MUST be recorded. Unbound MUST accrue estimated UBIT quarterly in the sponsor's books in the period the unrelated business taxable income is earned (ASC 740), MUST allocate the estimate to the fund(s) that held the income-producing asset in proportion to attributed UBTI, MUST reduce those funds' capacity by a recorded UBIT hold equal to the allocated estimate before any grant reservation, and MUST true up the accrual, allocation and hold when Form 990-T is filed. |
| UNB-XIN-BNK-001 | XID-BNK-005 | The system MUST maintain a versioned RailObligation register with one row per (rail, direction, sponsor role ∈ {Originator, ThirdPartySender, Payee, Merchant, Holder, BankCustomer}, obligation), each carrying source (statute, regulation, network rule, bank contract, sponsor policy), citation, effective date, binding class, linked control ID, owner and counsel status; the PAY-DEF-001 production gate for a rail MUST fail while any binding row for that rail and role lacks an implemented, tested control. Default roles (XID-BNK-005): the sponsor is Originator of record on sponsor-titled accounts under its own ODFI agreement; the platform operator is a Third-Party Service Provider, never a Third-Party Sender, and holds no funds. |
| UNB-XIN-BNK-003 | XID-BNK-008 | Every business day the system MUST ingest bank activity for each sponsor disbursement, clearing and operating account linked to Unbound and match every debit to exactly one authorized platform instruction or to a configured expected-debit class (bank fee, sweep, return); any unmatched debit MUST open a high-severity incident within one hour of ingestion, notify two independent sponsor roles and place a protective hold on further releases from that account until an authorized human classifies it; ACH debit block or filter status for disbursement accounts MUST be recorded and attested quarterly. Grant disbursement accounts MUST carry an ACH debit block allowing only named bank-fee company IDs; processors MUST settle to a separate contribution clearing account where only the processor's company ID may debit (XID-BNK-008). |
| UNB-XIN-BNK-007 | XID-BNK-003 | The system MUST maintain a counsel-approved UnclaimedPropertyRule table per state {owner type (organization/individual), property type (vendor/grant/refund check), dormancy period, business-to-business or charitable exemption flag, due-diligence notice window and content, report and remittance dates}, MUST determine the state by the payee's last known address (with the sponsor's state of incorporation as fallback, per counsel), MUST send due-diligence outreach before StaleDated items become reportable, and MUST either produce report and remittance evidence or record the counsel-approved alternative disposition (e.g., exemption) for each item; an item with no applicable rule row MUST remain in UnclaimedPropertyReview and MUST NOT be re-credited to a fund. Outstanding uncashed grant checks MUST be presumed reportable property owed to the payee charity; before dormancy the system MUST make… |
| UNB-XIN-CLM-002 | XID-CLM-003 | Every decline, hold longer than its profile's notice threshold, or conditional approval communicated to a donor, advisor or charity MUST include: a reason code from a counsel-approved public reason catalog mapped from gate outcomes; a plain-language explanation; the public policy reference (clause ID); what evidence or action, if any, could change the outcome; the reconsideration path and deadline where available; and the decision date. Notices MUST NOT disclose sanctions match details, investigation existence, or other restricted reasons; such cases MUST use a neutral catalog code. The notice content and catalog version MUST be stored with the decision record. The public reason catalog version 1 is: ELIGIBILITY_NOT_CONFIRMED, ORGANIZATION_STATUS_CHANGED, PURPOSE_NOT_PERMITTED_BY_POLICY, IMPERMISSIBLE_BENEFIT, INFORMATION_NOT_RECEIVED, PAYMENT_DETAILS_UNVERIFIED, COMPLIANCE_REVIEW (all… |
| UNB-XIN-CLM-006 | XID-CLM-002 | A representative (agent under power of attorney, guardian, conservator, family delegate) MUST act only within the scope recorded in the delegation (ADM-005). An authorized staff member who records specific facts supporting a reasonable belief that a representative is exploiting or endangering the donor MUST be able to place a ProtectiveSuspension that blocks new recommendations and contact/payee changes by that representative, notifies the sponsor's designated reviewer within 1 business day, expires after a counsel-set maximum unless renewed, and MUST NOT disclose the suspicion to the representative. Pending payments are governed by existing hold rules. Durations (XID-CLM-002): a ProtectiveSuspension expires after 15 business days unless an independent reviewer extends it once by up to 10 business days, after which it must be referred to counsel or the appropriate authority; no money mo… |
| UNB-XIN-CRM-002 | XID-CRM-004 | Onboarding of every advisory-privilege holder and annual attestation MUST capture declared affiliations (officer, director, employee, substantial contributor) with charities and other entities, as capacity edges with evidence and effective dates. When the contributor or advisory-privilege holder of a fund is itself a charity, the fund MUST carry a counsel-approved account-type classification (UNB-RCN-ENT-011) and grants to that charity or its affiliates MUST follow the classification's rule. Any recommendation whose recipient matches a declared or detected affiliation MUST be routed to TVCTL-14 review. A fund where a charity names itself beneficiary is classified designated/agency (UNB-XIN-ACC-001); if an organization-advised fund is a DAF, grants to the advising organization are permitted when it is a §170(b)(1)(A) organization other than a disqualified supporting organization, with th… |
| UNB-XIN-CRM-003 | XID-CRM-001, XID-REG-003 | Each message type MUST declare a primary-purpose class (Commercial, TransactionalOrRelationship, MandatoryLegal, Security) and a sender legal entity with its entity type (tax-exempt nonprofit or other). Every Commercial email MUST carry the sender's valid physical postal address and a functioning opt-out that remains operable for at least 30 days after sending; an opt-out MUST be applied to all Commercial sends for that sender entity within 10 business days, and suppression lists MUST be keyed by sender entity. Automated or prerecorded voice or SMS messages that include an advertisement MUST NOT be sent without consent recorded at the level required for the sender's entity type (prior express written consent unless counsel records the nonprofit rule applies). Opt-outs MUST NOT suppress MandatoryLegal, Security or TransactionalOrRelationship messages. Decided defaults: promotional email… |
| UNB-XIN-CRM-006 | XID-CRM-002 | Each fund MUST record its referral attribution (referring party and capacity, channel, date) as an immutable informational fact. Unbound MUST NOT compute, accrue or export any referral compensation based on contributions, balances or grants unless a counsel-approved compensation rule for that referral arrangement and jurisdiction set is recorded; until then attribution MUST be usable only for reporting. Decided rule (XID-CRM-002): referral compensation MUST NEVER be paid from fund assets; flat fair-market-value fees from sponsor operating funds are permitted only under a written agreement, only to persons who are not advisors or related parties of the referred fund, only in states recorded as satisfying paid-solicitor rules, and never per grant or per dollar granted. |
| UNB-XIN-ECO-001 | XID-ECO-005 | The platform SHALL maintain operational cost attribution as a management-accounting projection (CostAttribution read model) derived from GL actuals, vendor cost records and domain events. Cost attribution records SHALL NOT create, modify or reverse any journal entry, fund balance, NAV, reservation, fee accrual or available-to-grant amount. Any economic consequence that must reach the books (fee invoice, chargeback, functional expense allocation) SHALL be raised only through the existing authorised accounting command for that purpose (UNB-ACC-010 fee/billing, UNB-ACC-019 functional allocation) with its own approval. Management attribution and the GAAP/Form 990 functional expense allocation MUST use the same activity dataset with separately versioned mappings; only the functional mapping, approved annually by the controller and external CPA, may reach financial statements (XID-ECO-005). |
| UNB-XIN-ECO-004 | XID-ECO-003 | For each closed accounting period and sponsor scope, the platform SHALL execute a ReconciliationRun that compares the sum of AttributedCost plus UnallocatedResidual plus reported UnusedCapacity with GL operating expense actuals for mapped accounts, per CostPool. The run SHALL (a) require the identity sum(attributed)+residual+unused = GL actual for each pool to within $0.01, (b) record UnallocatedResidual explicitly by pool and reason (UnmappedGLAccount, MissingDriverData, NoEligibleCostObject, LateInvoice), (c) never distribute residual to cost objects except through a ResidualPolicy rule whose output is grade D and separately labelled, and (d) mark the run ExceedsTolerance when residual exceeds the configured tolerance (default 2% of pool or $5,000, whichever is greater), requiring controller sign-off before economics reports for that period are published as final. Tolerance is fixed b… |
| UNB-XIN-ECO-018 | XID-ECO-002 | For every configured service charge (e.g. international ED/ER review, wire, stop-payment/reissue, complex-asset handling, expedited grant), the platform SHALL report monthly recovery ratio = posted charge revenue / attributed cost of the corresponding activity, with grade. A service charge to a DAF fund SHALL be permitted only when it is listed in the fee schedule disclosed before the request, the donor affirms it, and a recorded cost-basis memo supports it. Where the annual recovery ratio of a charge exceeds 1.10, the price SHALL be reduced at the next schedule version (XID-ECO-002). Waivers SHALL be recorded with reason and counted separately. |
| UNB-XIN-ERP-001 | XID-ERP-001 | The fee engine MUST compute every fund-level fee daily on the daily balance of the declared FeeBase, using actual/365 (actual/366 in leap years) unless the schedule version declares otherwise, and MUST post the computed fee once per assessment period declared in the schedule (monthly or quarterly), not daily. The computation MUST segment at each of: fund activation, schedule-version effective date, program transfer, successor split or merge, closure trigger and migration cutover; each segment uses the schedule version and fee base in effect for that segment. A minimum fee MUST be prorated by the same day count over the days the fund was billable. Grantable capacity MUST subtract fees computed but not yet posted. The assessed amount MUST NOT exceed the fund's available balance net of reservations and approved-unpaid grants at assessment; any shortfall MUST be recorded as a waived amount… |
| UNB-XIN-ERP-002 | XID-ERP-002 | Every fee schedule MUST declare exactly one payer source from {FundAssets, DonorExternal, PartnerInvoice, SponsorAbsorbed}. FundAssets fees MUST post only as fund-level fee entries under an approved schedule. DonorExternal fees MUST be recorded outside every fund ledger as a payment for services, MUST NOT be accepted through the contribution workflow, MUST NOT appear on any contribution acknowledgment, and MUST NOT be offset against a fund balance. PartnerInvoice fees MUST create an accounts-receivable invoice to the partner legal entity. A fee schedule with a missing or ambiguous payer source MUST NOT activate. DonorExternal fees are payments for services, receipted as service fees and booked as program service revenue outside fund ledgers (XID-ERP-002). |
| UNB-XIN-EVA-001 | XID-ERP-001, XID-ADV-003 | For each sponsor legal entity and for every function for which Unbound is designated book of record (UNB-XIN-ACC-010), fund balances, reservations, capacity inputs and every financial report figure MUST be computed only from the insert-only journal and reservation tables, less fees computed but not yet posted (XID-ERP-001). Every domain fact with a declared financial effect MUST carry the JournalBatchId (or ReservationId) committed in the same database transaction, and the system MUST NOT maintain any other log from which balances can be derived or treated as a source on replay; re-derivation under UNB-XIN-ACC-003 is a comparison whose output never posts. Posting rules MUST be applied once at posting time and MUST NOT be re-applied on replay. Where an external system is book of record (Shadow mode), capacity MUST be computed under UNB-XIN-ADV-002. |
| UNB-XIN-EVA-002 | XID-EVA-002, XID-ADV-005 | Every posting command MUST read the accounting period row for its posting date with a shared lock (or run SERIALIZABLE) in the posting transaction; HardClosePeriod MUST take an exclusive lock on that row. An ordinary posting whose posting date falls in a hard-closed period MUST be rejected with PERIOD_CLOSED, and the economic event MUST instead be recorded through a PriorPeriodAdjustment command that posts in the first open period, carries relates_to_period and effective_at of the original event, and requires controller approval above a sponsor-configured threshold. Routing is fixed by XID-EVA-002 and XID-ADV-005: before the audited statements for a year are issued, adjustments found in audit post in that year's AuditAdjustment period (UNB-XIN-ACC-005); after issuance, a late item posts as a PriorPeriodAdjustment classified Immaterial (current-period correction with disclosure) or Mater… |
| UNB-XIN-EVA-005 | XID-EVA-003 | Each external observation type MUST have a registered business idempotency key derived from source data (e.g., ACH return: original entry trace number + return reason code + return entry trace number; check: bank account + check number + item sequence; custodian transaction: custodian + account + transaction reference + version/cancel indicator; provider callback: provider event id). Ingestion MUST store the key and a payload hash. A record with an existing key and identical hash MUST be acknowledged without effect; a record with an existing key and a different hash MUST NOT be applied and MUST open a DuplicateKeyConflict case. Internal commands MUST use stable business keys (GrantId + installment, PaymentIntentId, ScheduleId + occurrence) rather than transport retry identifiers. Intermediary fees on partially returned wires and bank fees on returned payments MUST post as sponsor operat… |
| UNB-XIN-EVA-009 | XID-SAS-002, XID-EVA-005 | A bulk transfer of funds from one legal sponsor to another MUST run as a TransferProgram process manager that issues, per fund, an independent DAF_TRANSFER_OUT distribution in the source sponsor ledger and an independent inbound contribution in the destination sponsor ledger, each with its own authorization, idempotency key (TransferProgramId + FundId) and reconciliation. Source history MUST remain in the source sponsor's books; the destination MUST receive a signed, hashed predecessor-history package recorded as non-authoritative reference evidence, with a fund-identifier crosswalk. Before a fund's transfer posts, its in-flight items (reserved grants, Unknown payments, open returns, pending contributions) MUST be resolved or explicitly assigned by the program's approved rule; a fund with an Unknown payment MUST NOT transfer. Legal form (XID-EVA-005): each transfer is a grant to the rec… |
| UNB-XIN-EVA-010 | XID-EVA-004 | The system MUST model at least these distinct hold types with a declared set of blocked commands: ProtectiveHold(charity\|recipient\|destination) blocks release and dispatch to that subject; FundProtectiveFreeze blocks outflow commands from the fund (grants, transfers, and fee accrual only if sponsor policy says so) but MUST NOT block inbound contributions, dividends, valuation, statements or records access; SanctionsBlock moves affected funds to a blocked-property account and blocks unblocking without a licence/authorization record; RecordsLegalHold blocks only retention disposition and deletion. Each hold MUST record type, scope, authority, reason and release authority, and no hold type MAY be released by AI. During a dispute FundProtectiveFreeze (XID-EVA-004): fees continue to accrue; investments follow the standing policy but advisor-directed allocation changes are rejected; recurri… |
| UNB-XIN-INV-002 | XID-INV-003 | Every investment pool MUST record a counsel-approved LegalBasis (statutory exclusion or registration relied upon), the charitable organization that maintains it, its trustee/administrator and the sponsors whose assets it may hold. Unbound MUST reject subscriptions from a fund of a sponsor not listed, MUST reject creating a pool whose maintainer is not a charitable organization unless the LegalBasis cites a different counsel-approved basis, and MUST re-require approval when the maintainer or sponsor list changes. For release 1 only single-sponsor pools maintained by the sponsor itself under the charitable-organization exclusion are permitted; multi-sponsor pools and pools maintained by the operator MUST be rejected, and donors MUST receive the pool's written disclosure before first allocation (XID-INV-003). |
| UNB-XIN-NPG-001 | XID-NPG-001 | For a foreign recipient without an IRS determination, the system MUST permit the routine (non-ER) path only when an EquivalencyDetermination record exists for the sponsor with: practitioner identity and type (attorney, CPA or enrolled agent) and attestation of Circular 230 standing; advice date; grantee taxable-year end; taxable year of the latest factual information relied on; public-support test period where used; law-version reference; and the advice document hash. The system MUST compute CurrentUntil deterministically per a counsel-approved rule encoding Rev. Proc. 2017-53 §3.03(1) (default: end of the grantee taxable year following the year of the factual information; for public-support advice, end of the second grantee taxable year after the five-year test period) and MUST treat the determination as expired on the earlier of CurrentUntil or a recorded law-change event affecting it… |
| UNB-XIN-NPG-002 | XID-NPG-002 | When a recommendation names a foreign recipient for which the sponsor has no current EquivalencyDetermination and does not offer expenditure responsibility, the system MUST present the sponsor's configured IntermediaryCatalog entries (each a domestic §170(b)(1)(A) organization with recorded counsel-approved purpose wording) as alternatives, and MUST convert an accepted alternative into a domestic grant to the intermediary whose remittance purpose uses only the catalogue's approved wording (e.g. 'for the intermediary's program supporting <country/cause>'), never language requiring payment to the named foreign organization. The original foreign recommendation MUST be preserved as donor intent evidence. Wording MAY express a non-binding preference for the intermediary's work in a country or program and MUST NOT name a foreign grantee as a required recipient or require pass-through; the int… |
| UNB-XIN-NPG-005 | XID-NPG-005 | The system MUST apply a counsel-approved, versioned OrganizationChangeEffect table to every open grant and active schedule when a change event is recorded for the recipient: NameChangeSameEIN -> continue, update remittance name, no hold; AddressChange -> continue, re-verify destination only if destination changed; EINChange or MergerIntoSuccessor or SuccessorDesignated -> hold, require sponsor approval of payee substitution with new eligibility evaluation of the successor and donor notification; Dissolution or BankruptcyFiled -> cancel unreleased grants and suspend schedules, route dispatched-unsettled payments to UNB-RCN-GRT-016; Revocation -> hold; ReinstatementRetroactive -> re-evaluate held grants, release only if the reinstatement effective date covers the grant date and all other checks pass. Every effect MUST be recorded with the event evidence and table version. A same-purpose e… |
| UNB-XIN-NPG-007 | XID-NPG-003 | The RecipientClass rule MUST include a class 'Section530AGeneralFundingContribution', disabled by default per sponsor, mapping to Permitted only when: a Treasury acceptance agreement reference identifying the sponsor as eligible donor is recorded; the qualified or approved class definition is recorded; the grant date falls within the rule's applicability window (taxable years beginning on or after 2026-01-01) and before its expiry (2029-09-30) or a successor rule; and the BenefitAndPurposeGate confirms the class is not defined by reference to the donor's or advisor's family. Absent any condition the class MUST map to CounselReview. Payments MUST go only to the Treasury-designated channel, never to an individual account holder. The class MUST remain disabled for every sponsor until the decision XID-NPG-003 is revisited. |
| UNB-XIN-NPG-008 | XID-NPG-007 | When any rule, audit or human report identifies a completed payment as a possible taxable distribution (§4966), prohibited benefit (§4967) or excess benefit (§4958), the system MUST open an ExciseTaxIncident with: affected grants and amounts; suspected provision; parties (sponsor, fund managers who approved, donor/advisor, benefited person); knowledge evidence (approval records); recovery actions (repayment request, receivable); and a filing-inputs record for Form 4720 and, for §4958, Form 990 Schedule L Part I. Determinations of whether a violation occurred and who is liable MUST be made only by authorized counsel/CPA roles. The incident MUST block further grants to the same payee from the same fund until counsel disposition and MUST be reported to the sponsor's board-designated committee within a sponsor-configured period. Because §4966 and §4967 first-tier taxes are qualified first t… |
| UNB-XIN-NPG-011 | XID-BNK-003 | For every check reaching AgedOutreach, the system MUST determine the reporting jurisdiction by a counsel-approved priority rule (default: state of the payee's last known address in the sponsor's records; if none or foreign, the sponsor's state of incorporation), look up that state's dormancy period, due-diligence-letter window, reporting deadline and any business-to-business or charitable exemption in a versioned UnclaimedPropertyRuleTable, and schedule the due-diligence letter and report accordingly. Where the table records an applicable exemption, the system MUST record the exemption citation and follow the counsel-approved alternative disposition. The priority rule is fixed by XID-BNK-003 (payee's last-known address state, else sponsor state of incorporation). |
| UNB-XIN-PAY-006 | XID-BNK-002 | For every processor-settled contribution the system MUST record gross amount charged to the donor (including any donor-elected fee coverage), the processor fee actually assessed, and net settled, linked to the payout batch and bank credit; the contribution credited to the fund and stated on the acknowledgment MUST be determined by a CPA/counsel-approved FeeCoveragePolicy (default: gross charged is the contribution; processor fee posted as sponsor or fund expense per policy); and a payout batch MUST NOT reconcile unless Σgross − Σfees − Σreversals = net payout = bank credit. Default FeeCoveragePolicy (XID-BNK-002): the gross amount charged is the contribution and the acknowledged amount; the fund is credited gross and the processor fee posts as a separate fund expense line. |
| UNB-XIN-REG-001 | XID-BNK-005, XID-REG-001 | Every deployment MUST record a FundsFlow declaration naming, for each account that holds contribution or grant money, the legal title holder and every party with instruction authority. Accounts holding charitable money MUST be titled to the legal sponsor (or a custodian for the sponsor); the platform operator MUST have instruction-only authority under the sponsor's delegation. A deployment whose FundsFlow shows the operator or an affiliate as title holder or receiving funds into its own account MUST NOT activate until operator counsel records a money-transmission/MSB analysis covering the states served. Changes to FundsFlow MUST re-trigger this gate. The operator MUST NOT take title to or possession of contribution or grant money in any deployment, and no embedded or white-label deployment that touches money MAY go live until a multistate money-transmission memo is recorded (XID-REG-001… |
| UNB-XIN-REG-002 | XID-INV-004, XID-REG-002 | The system MUST maintain an OperatorStatusRegister mapping status-relevant features to the legal question they raise: (a) operator-provided investment model portfolios, recommendations or advice for compensation -> investment-adviser analysis; (b) any operator compensation tied to securities transactions or order routing -> broker-dealer analysis; (c) donation pages soliciting for multiple charities -> charitable-platform registration (e.g. Cal. Gov. Code §12599.9); (d) operator title or transmission of funds -> UNB-XIN-REG-001; (e) operator processing of consumer financial information as a for-profit -> GLBA/FTC Safeguards analysis including 16 CFR 314.4(j) notification. Activating a mapped feature for a deployment MUST require a recorded counsel determination for that question, deployment and jurisdiction set. Default dispositions (XID-REG-002): investment advice or model construction… |
| UNB-XIN-SAS-001 | XID-SAS-005 | Unbound MUST maintain a versioned, effective-dated CapabilityEntitlement record per tenant and program that lists each optional capability as Entitled or NotEntitled with the contract reference. Authorization MUST deny a command that requires a NotEntitled capability with a stable error code. Removing an entitlement MUST NOT cancel, block or alter any command already accepted, any protective hold, statement, acknowledgment, retention obligation or regulatory output; those fiduciary capabilities MUST be marked NonRemovable in the catalog. Entitlements MAY be scoped by jurisdiction: D2C own-brand app and charity-side giving button capabilities MUST be NotEntitled for California users until a California opinion is recorded (XID-SAS-005). |
| UNB-XIN-SAS-007 | XID-SAS-002 | A distribution-partner exit MUST be processed as a PartnerExit case with states Notified > DonorOutreach > ElectionsCollected > TransfersProposed > SponsorDecided > TransfersExecuted > Reconciled > Closed. Funds MUST move to another sponsor only as individual outbound transfer grants under RR-UNB-WF-014-r1, each with a recorded recommendation from an authorized advisor of that fund and a decision by sponsor A (which MAY be a sponsor-approved batch policy); no fund MUST change LegalSponsorId by data migration. Funds whose advisors do not recommend transfer MUST remain with sponsor A, with access through a sponsor-direct credential provisioned before the partner's federation ends, and with servicing reassigned to sponsor A or a successor partner. Partner access, subscriptions and API clients MUST terminate at the exit effective time while accepted recommendations continue under sponsor re… |
| UNB-XIN-SVC-003 | XID-SVC-002 | Unbound and its telephony adapter MUST NOT record, transcribe or machine-analyze any voice interaction unless (a) a sponsor-approved disclosure was played or read to every party before recording began and (b) consent was captured (continuation after disclosure only where counsel approves that as consent; otherwise explicit DTMF/verbal acceptance). The same disclosure MUST apply regardless of caller or staff location. A caller who declines MUST be served without recording or offered an equivalent channel. Recording MUST pause during DTMF entry of secrets. Each recording or transcript MUST store disclosure version, consent evidence, parties, start/stop times and retention class. Outbound staff calls to charities or custodians follow the same rule. Release-1 setting (XID-SVC-002): recording and machine transcription of voice interactions are disabled for every sponsor; enabling later requi… |
| UNB-XIN-TRS-004 | XID-TRS-001 | A legal representative MUST NOT exercise advisory privileges on behalf of a principal unless (a) the instrument's verified scope covers charitable recommendations or the principal's DAF advisory privileges and (b) sponsor policy permits representative exercise; otherwise the representative receives view-only access where policy allows. Restoration of a principal's authority after incapacity MUST require verified evidence of capacity and MUST end representative grants derived from the incapacity at the same effective time. A power of attorney qualifies only if it expressly grants authority to make gifts or charitable donations; guardians and conservators act only under a court order covering charitable gifts (XID-TRS-001). |
<!-- END GENERATED: applied -->

## Decisions

<!-- BEGIN GENERATED: outcomes -->
### XID-ADV-005 (accounting)

**Question.** Routing of prior-year adjustments discovered during and after the audit (AuditAdjustment period vs PriorPeriodAdjustment).

**Decision.** Before the audited statements are issued, adjustments found during the audit post in a soft-closed AuditAdjustment period (period 13) and accept only journals linked to an AuditFinding. After issuance, the XID-EVA-002 route applies.

**Rationale.** Mirrors ASC 855: conditions existing at the balance-sheet date discovered before issuance adjust those statements. Resolves XIC-ADV-005.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Auditor.
- **Authority:**
  - FASB ASC 855 (recognized subsequent events before issuance adjust the statements) — secondary knowledge
  - FASB ASC 250 (error corrections: restatement if material to issued statements; immaterial corrected in current period) — secondary knowledge
- **Requirement consequences:**
  - UNB-XIN-ACC-005 and UNB-XIN-EVA-002 boundary set at issuance date.

### XID-ADV-006 (accounting)

**Question.** Which GrantRecognitionPoint values are permitted?

**Decision.** Permitted GrantRecognitionPoint values: ApprovalCommunicatedToGrantee (default) and PaymentReleased (only where payment is the first communication to the grantee). SponsorApproval is NOT a selectable policy; it collapses into ApprovalCommunicatedToGrantee when the approval notice is sent in the same period. Conditional grants (barrier plus right of return/release) are recognized when the barrier is overcome.

**Rationale.** Under ASC 958 a contribution made is recognized when an unconditional promise to give exists, and a promise requires communication to the recipient; an internal approval nobody has told the charity about is an intention. Offering SponsorApproval as a free choice would let a sponsor expense uncommunicated approvals. Audited practice ('when approved') is consistent with this because approval and notice are normally contemporaneous.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** External CPA confirms before the first audited year.
- **Authority:**
  - FASB ASC 958-605 / 958-310 (promise to give requires communication to the recipient; an intention to give is not a promise) — secondary knowledge, Codification not read
  - FASB ASU 2018-08 (conditional = barrier + right of return/release; ambiguous presumed conditional) https://storage.fasb.org/ASU_2018-08.pdf (read 2026-11-23)
  - Colorado Gives Foundation FY2025 audited statements, Note 2 (variance power; agency funds as liabilities) — read by the INV pass 2026-11-23
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-ACC-002: remove SponsorApproval from the enumeration; add cutoff rule: approved-but-uncommunicated at period end is not expensed (cutoff register line).

### XID-BNK-006 (accounting)

**Question.** Card reversal window and provisional credit policy for cards and business ACH.

**Decision.** Contributions are recognized as revenue when the funds settle to the sponsor (unconditional gift received); reversibility affects availability, not recognition. Availability for grants: ACH consumer debit — available after 5 banking days, with exposure to late unauthorized returns capped by a per-donor provisional limit; business (CCD) debit — 3 banking days; card — after settlement, up to a per-donor provisional limit, remainder after 60 days; wire/check cleared — on settlement. Per-donor provisional limit defaults to $0 for donors with < 90 days history and $10,000 thereafter (illustrative, sponsor-configurable). A chargeback or return after acceptance reverses contribution revenue against the fund to the extent of its balance; any shortfall is a sponsor operating loss, never a donor receivable.

**Rationale.** A 120-day hold on every card gift (the research pass default) is disproportionate given dispute rates; exposure limits contain the loss. Returns are not 'refunds' under UNB-RCN-FIN-013 because they are involuntary.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Sponsor CFO sets the limits; counsel confirms acknowledgment timing (acknowledge after availability).
- **Authority:**
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
  - Nacha Operating Rules: Third-Party Service Provider vs Third-Party Sender; return timeframes (Nacha web pages, partly search summaries)
  - Visa dispute windows 120/540 days (search summary)
  - OD-04
- **Requirement consequences:**
  - RR-TVR-CON-001-r1 ReversalWindow rows set to these defaults.
  - XIC-BNK-001 resolved: chargeback = involuntary reversal.

### XID-ECO-003 (accounting)

**Question.** Residual tolerance threshold for ReconciliationRun

**Decision.** ReconciliationRun tolerance: per pool, |residual| ≤ max(2% of the pool's GL actual, $5,000); in aggregate, residual ≤ 1% of in-scope GL operating expense. Any residual coded 'unmapped GL account' fails regardless of size until the account is mapped.

**Rationale.** Resolves the ambiguity found by XIF-ADV-008 and makes the identity testable on its components rather than true by construction.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Controller (management policy; no external authority).
- **Authority:**
  - XIF-ADV-008
  - UNB-XIN-ECO-004
- **Requirement consequences:**
  - UNB-XIN-ECO-004 tolerance wording replaced.

### XID-ECO-005 (accounting)

**Question.** Relationship between management ABC and Form 990 functional expense allocation

**Decision.** Management activity-based costing and the GAAP/Form 990 functional expense allocation use the same activity dataset with two separately governed mappings. The functional mapping is accounting policy, approved annually by the controller and external CPA, and is the only one that reaches financial statements. The management mapping never posts.

**Rationale.** Functional expense reporting is required for NFPs; sharing the dataset avoids two time studies, separating the mappings stops management allocations from changing reported expenses.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** External CPA approves the functional mapping annually.
- **Authority:**
  - FASB ASC 958-720-45 (functional expense analysis: program, management and general, fundraising); Form 990 Part IX
  - RR-UNB-ACC-014-r1
- **Requirement consequences:**
  - UNB-XIN-ECO-001 and RR-UNB-ACC-014-r1 reference this split.

### XID-ERP-001 (accounting)

**Question.** Billing basis: average daily balance with daily accrual, or period-end balance?

**Decision.** Fund-level fees are computed daily on the daily balance of the declared FeeBase (actual/365, actual/366 in leap years), but posted once per assessment period (monthly or quarterly per schedule). Capacity = journal balance − reservations − accrued-unposted fees. Segments split at open, close, transfer, schedule change; minimum fee prorated by days.

**Rationale.** Daily computation is fair at opening/closing/transfer; daily posting would generate ~31× the grant row volume (XIF-ADV-006) with no reporting benefit; subtracting accrued-unposted fees keeps capacity honest.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Sponsor CFO adopts schedule wording; none legal.
- **Authority:**
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
  - RCD-018 FeeBase; XIC-ADV-009
- **Requirement consequences:**
  - UNB-XIN-ERP-001: 'accrue daily' becomes 'compute daily, post at assessment'.
  - UNB-XIN-EVA-001 capacity formula includes accrued-unposted fees.

### XID-EVA-002 (accounting)

**Question.** Default treatment for postings whose effective date falls in a hard-closed period: current-period prior-period adjustment, reopen, or restatement, and the materiality threshold for each

**Decision.** A posting whose effective date falls in a hard-closed period posts in the current open period with relates_to_period and a correction class. If the affected statements have not been issued, see XID-ADV-005. If they have been issued: immaterial → current-period correction with disclosure ('little r' revision where comparatives shown); material → formal restatement of comparatives, with originally issued statements preserved. A hard-closed period is reopened only before issuance and only with controller + CFO approval. Materiality is set annually by the CFO with the auditor (illustrative quantitative default 0.5% of total net assets or 5% of the change in net assets, whichever is lower, plus qualitative factors). Donor-facing fund statements already issued are always corrected by an adjustment line with explanation on the next statement, and an amended statement is issued whenever an acknowledgment or a tax-relevant figure changes.

**Rationale.** Follows the error-correction model of ASC 250 and keeps ACC-MDL-010 (closed periods immutable). Donor statements get stricter treatment because donors rely on them individually.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Auditor agrees the materiality defaults.
- **Authority:**
  - FASB ASC 250 (error corrections: restatement if material to issued statements; immaterial corrected in current period) — secondary knowledge
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-EVA-002 PERIOD_CLOSED handling cites this policy.
  - RR-UNB-DATA-005-r1 impact registry drives amended statements.

### XID-EVA-003 (accounting)

**Question.** Who bears intermediary fees on partially returned wires and bank fees on returned payments: the fund, the sponsor operating budget, or the recipient?

**Decision.** Intermediary fees on partially returned wires and bank fees on returned payments are sponsor operating expense by default. They are charged to the fund only if the fee schedule disclosed the pass-through before the grant was recommended. Fees caused by recipient-supplied wrong bank details are still sponsor expense; the sponsor may pursue recovery outside the fund.

**Rationale.** The donor did not cause the failure; charging funds without prior disclosure is an undisclosed service charge (XID-ECO-002).

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Sponsor CFO.
- **Authority:**
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
  - UNB-XIN-ECO-018
- **Requirement consequences:**
  - UNB-XIN-EVA-005 return posting rules.

### XID-INV-001 (accounting)

**Question.** Which GrantRecognitionPoint does each sponsor adopt, and does approval without notice to the charity create a promise to give?

**Decision.** Default recognition point for every sponsor is ApprovalCommunicatedToGrantee; the platform sends the approval notice automatically on approval, so in normal operation expense, payable and approval fall in the same period. Approval without notice does not create a promise to give.

**Rationale.** Same basis as XID-ADV-006; this closes OD-19 for the platform default.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** External CPA.
- **Authority:**
  - FASB ASC 958-605 / 958-310 (promise to give requires communication to the recipient; an intention to give is not a promise) — secondary knowledge, Codification not read
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-ACC-002 default value; OD-19 closed for the default, sponsor may elect PaymentReleased with CPA sign-off.

### XID-INV-002 (accounting)

**Question.** Is the DAF component presented as net assets without donor restrictions for every program version, and which account types are agency/pass-through?

**Decision.** Classification by account type: (1) donor-advised funds whose program documents grant the sponsor variance power: contribution revenue, net assets WITHOUT donor restrictions; (2) funds where a nonprofit transfers assets and names itself beneficiary (agency/designated funds) without granting variance power: LIABILITY, not revenue; (3) only gift instruments imposing a time or purpose restriction beyond advisory privileges create net assets WITH donor restrictions. Each program version must store the variance-power clause; missing evidence blocks classification (1).

**Rationale.** Variance power is what makes DAF contributions unrestricted for the sponsor; agency transfers where the resource provider is the beneficiary are liabilities. Advisory privileges are not donor restrictions (UNB-ACC-033, ACC-MDL-008).

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** External CPA and counsel confirm program-document wording.
- **Authority:**
  - Colorado Gives Foundation FY2025 audited statements, Note 2 (variance power; agency funds as liabilities) — read by the INV pass 2026-11-23
  - FASB ASC 958-605-25 (agency and variance-power guidance) — secondary knowledge
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-ACC-001: AccountingTreatment enumeration fixed as above.
  - RR-UNB-ACC-033-r1: variance-power evidence mandatory per program version.

### XID-INV-005 (accounting)

**Question.** Schedule D line 4 valuation basis and line 3 grant basis (approved vs paid).

**Decision.** Schedule D Part I: line 4 = aggregate fair value of DAF assets at year end, the same measurement as the audited statements; line 3 = all distributions from DAFs including transfers to other funds of the organization, as the instructions require, on the same recognition basis as grant expense (XID-ADV-006). The UNB-XIN-ACC-006 bridge reconciles line 3/4 to GAAP and to fund statements with typed reconciling items (intra-organization transfers, recognition vs payment timing, agency funds, valuation date).

**Rationale.** One measurement basis everywhere eliminates the observed Schedule D/net-asset breaks; the instructions themselves include internal transfers, so they are a bridge item, not an error.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Return preparer/CPA.
- **Authority:**
  - IRS Instructions for Schedule D (Form 990), Part I (lines 1-4 read 2026-11-23)
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-ACC-006: reconciling-item types fixed.

### XID-INV-006 (accounting)

**Question.** UBIT recognition policy (accrue estimate vs recognize on filing) and fund allocation rule.

**Decision.** UBIT is accrued under ASC 740 in the period the unrelated business taxable income is earned (quarterly estimate), trued up on filing Form 990-T. The estimated tax is allocated to the fund(s) that held the income-producing asset in proportion to attributed UBTI, and reduces capacity through a recorded hold at the estimate until the true-up.

**Rationale.** GAAP requires accrual of income tax; allocating to the generating fund keeps other donors' funds from bearing it; the hold prevents granting money owed as tax.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Tax advisor confirms allocation method.
- **Authority:**
  - FASB ASC 740 applies to income taxes of tax-exempt entities, including UBIT — secondary knowledge
  - 26 USC 511-514; §512(e) S-corporation stock (read by INV pass)
- **Requirement consequences:**
  - UNB-XIN-ACC-008: accrual + allocation + capacity hold rule.

### XID-BNK-002 (legal)

**Question.** Is a donor-covered processing fee part of the contribution stated on the acknowledgment?

**Decision.** When a donor elects to cover processing fees, the contribution and the acknowledgment are the gross amount charged. The fund is credited gross and the processing fee is shown as a separate fund expense line, so the fund nets the intended amount.

**Rationale.** The donor parted with the gross amount to a 501(c)(3) and received nothing in return; the processor fee is the sponsor's cost of collection. Gross presentation matches ASC 958 gross reporting.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel confirms acknowledgment wording.
- **Authority:**
  - 26 USC 170(f)(8), (f)(18) (acknowledgment)
  - unbound:requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md (ACC-MDL-002, -006, -008, -010; §4 matrix); unbound:requirements/FULL-ACCOUNTING-SYSTEM.md (UNB-ACC-003, -010, -017, -033)
- **Requirement consequences:**
  - UNB-XIN-PAY-006 recording rule.

### XID-BNK-003 (legal)

**Question.** Is a voided/stale DAF grant check property owed to the charity for unclaimed-property purposes, and which state rules (incl. B2B exemptions) apply?

**Decision.** An outstanding uncashed grant check is presumed property owed to the payee charity and reportable under the unclaimed-property law of the payee's last-known address state, else the sponsor's state of incorporation (priority rule). Before dormancy: two outreach attempts and an offer to reissue by ACH. The sponsor may void and re-credit the fund only where (a) the grant is cancelled before the check is cashed and the charity is notified, and (b) the applicable state table records that cancellation defeats the holder obligation (B2B or charity exemptions). Otherwise report.

**Rationale.** Conservative default; avoids treating voids as a way around escheat; the per-state table (UNB-XIN-BNK-007) encodes exemptions as counsel confirms them.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel completes the per-state table before release.
- **Authority:**
  - Texas v. New Jersey, 379 U.S. 674 (1965) priority rule (general knowledge)
  - OB-45
- **Requirement consequences:**
  - UNB-XIN-BNK-007, UNB-XIN-NPG-011 defaults.

### XID-BNK-005 (legal)

**Question.** Who is Originator of record and does any vendor hold funds (TPS/money transmission)?

**Decision.** The sponsor is the Originator of record on sponsor-titled accounts under its own ODFI agreement. The platform operator is a Third-Party Service Provider that transmits files on the sponsor's instruction; it is not a Third-Party Sender, holds no funds and has no FBO or omnibus account (XRJ-BNK-005 confirmed).

**Rationale.** Keeps the operator out of the funds flow (UNB-XIN-REG-001) and avoids Third-Party Sender registration and liability.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel confirms with the ODFI agreement.
- **Authority:**
  - Nacha Operating Rules: Third-Party Service Provider vs Third-Party Sender; return timeframes (Nacha web pages, partly search summaries)
  - UNB-XIN-REG-001
- **Requirement consequences:**
  - UNB-XIN-BNK-001 role rows; UNB-XIN-REG-001.

### XID-BNK-008 (legal)

**Question.** Do ACH debit blocks on disbursement accounts conflict with any processor model?

**Decision.** Grant disbursement accounts carry an ACH debit block with an allowlist of named company IDs (bank fee and analysis charges only). Card and ACH processors settle to a separate contribution clearing account, where processor debits (chargebacks, fees) are allowed for the processor's company ID.

**Rationale.** Separating clearing from disbursement makes the debit block compatible with every processor model.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Treasury confirms with the bank.
- **Authority:**
  - Nacha Operating Rules: Third-Party Service Provider vs Third-Party Sender; return timeframes (Nacha web pages, partly search summaries)
- **Requirement consequences:**
  - UNB-XIN-BNK-003 account roles.

### XID-CLM-002 (legal)

**Question.** Legal basis, maximum duration and liability for protective suspension of a representative.

**Decision.** The sponsor may place a protective suspension on a representative's advisory privileges when it reasonably believes exploitation or abuse of authority is occurring. Maximum 15 business days, one extension of up to 10 business days by an independent reviewer, then referral to counsel or the appropriate authority. No money moves during suspension; the principal is notified unless notice would put them at risk.

**Rationale.** Advisory privileges exist by program terms under the sponsor's exclusive legal control, so suspension is a contractual protective measure; durations borrow the FINRA 2165 model.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel approves program-terms wording.
- **Authority:**
  - FINRA Rule 2165 temporary hold: 15 business days, extendable 10 (read 2026-11-24) — design analogy only, not binding on a sponsor
  - IRC 4966(d)(2)(A)(ii) exclusive legal control
- **Requirement consequences:**
  - UNB-XIN-CLM-006 durations.

### XID-CLM-003 (legal)

**Question.** Approve the public decline reason catalog and neutral codes.

**Decision.** Adopt a public decline reason catalog of neutral codes: ELIGIBILITY_NOT_CONFIRMED, ORGANIZATION_STATUS_CHANGED, PURPOSE_NOT_PERMITTED_BY_POLICY, IMPERMISSIBLE_BENEFIT, INFORMATION_NOT_RECEIVED, PAYMENT_DETAILS_UNVERIFIED, COMPLIANCE_REVIEW (restricted reasons, e.g. sanctions or investigations, always map here), FUND_BALANCE_INSUFFICIENT, DUPLICATE_REQUEST, SPONSOR_DISCRETION. Free-text explanations are optional and reviewed.

**Rationale.** Specific enough to act on, neutral enough not to disclose investigations or sanctions matches.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** Counsel reviews wording once.
- **Authority:**
  - UNB-XIN-CLM-002
  - NAIC Model 900 §4 (read by SVC pass)
- **Requirement consequences:**
  - UNB-XIN-CLM-002 catalog.

### XID-CRM-001 (legal)

**Question.** Is sponsor email promoting DAF services 'commercial' under CAN-SPAM; which entity is sender in white-label?

**Decision.** Promotional email about DAF services is treated as commercial under CAN-SPAM. In white-label programs the sender is the entity named in the From line, designated contractually as the single sender; suppression is keyed by that sender. Transactional and legally required messages are classified separately.

**Rationale.** CAN-SPAM applies to any person sending commercial messages; nonprofit status is not an exemption.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** None beyond template review.
- **Authority:**
  - FTC CAN-SPAM compliance guide; 16 CFR 316.2(m) designated sender (guide read 2026-11-23)
- **Requirement consequences:**
  - UNB-XIN-CRM-003 (canonical) classification table.

### XID-CRM-002 (legal)

**Question.** Referral compensation to advisors/affiliates from sponsor operating funds: solicitor registration and §4958 exposure.

**Decision.** No referral compensation is ever paid from fund assets. Flat, fair-market-value fees paid from sponsor operating funds under a written agreement are permitted only to persons who are not advisors or related parties of the referred fund, only in states whose paid-solicitor rules the table records as satisfied, and never per grant or per dollar granted.

**Rationale.** Payments from DAF assets to advisors or related persons are automatic excess benefit; per-grant pay creates perverse incentives; state solicitation law varies.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel completes the state table.
- **Authority:**
  - 26 USC 4958(c)(2)
  - RCD-021
- **Requirement consequences:**
  - UNB-XIN-CRM-006 block rules.

### XID-CRM-004 (legal)

**Question.** May a charity that holds an organization-advised fund recommend grants to itself?

**Decision.** A fund established by a charity naming itself as beneficiary is classified as a designated/agency fund, not a DAF, and distributions to that charity follow the agency terms. If an organization-advised fund qualifies as a DAF, grants to the advising organization are permitted when it is a §170(b)(1)(A) organization (not a disqualified supporting organization), with the affiliation recorded and no benefit to any individual.

**Rationale.** §4966(c)(2)(A) permits distributions to public charities; the remaining risk is individual benefit, screened by affiliations (UNB-XIN-CRM-002).

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel.
- **Authority:**
  - 26 USC 4966(c)(2) (text read at law.cornell.edu 2026-11-24)
  - UNB-RCN-ENT-011
- **Requirement consequences:**
  - UNB-XIN-CRM-002 affiliation rule; UNB-XIN-ACC-001 agency class.

### XID-ECO-002 (legal)

**Question.** Which service charges may be charged to DAF funds, on what cost basis and with what disclosure (and is a recovery ceiling appropriate)?

**Decision.** Special-request service charges to DAF funds are permitted only when (a) listed in the fee schedule disclosed before the request, (b) the donor affirms the charge, and (c) the charge is cost-based. The annual cost-recovery ratio per charge must not exceed 1.10; above that the price is reduced at the next schedule version.

**Rationale.** Charging charitable assets above cost is hard to defend; ±15% uncertainty in grade-B cost data justifies a small tolerance rather than 1.25.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel reviews the schedule wording.
- **Authority:**
  - UNB-XIN-ECO-018
  - UNB-ACC-010
- **Requirement consequences:**
  - UNB-XIN-ECO-018 ceiling 1.10.

### XID-ERP-002 (legal)

**Question.** Tax characterization of donor-paid membership fees to a 501(c)(3) sponsor.

**Decision.** A donor-paid membership or account fee is a payment for services, not a charitable contribution. It is never acknowledged as a gift, is receipted as a service fee, and is booked as program service revenue outside fund ledgers.

**Rationale.** The payer receives administration services equal in value to the fee; there is no donative intent in excess of value received.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel and CPA confirm.
- **Authority:**
  - 26 USC 170 (quid pro quo principle; general knowledge)
- **Requirement consequences:**
  - UNB-XIN-ERP-002 DonorExternal payer rule.

### XID-EVA-004 (legal)

**Question.** During a fund dispute freeze, do fee accruals, investment rebalancing and recurring grants continue?

**Decision.** During a dispute freeze on a fund: fees continue to accrue; investments follow the standing investment policy but no new advisor-directed allocation changes are accepted; recurring and new grants are held; contributions may be accepted; nothing is distributed until the dispute is resolved by the sponsor.

**Rationale.** The fund is still administered; only advisor-driven outflows and changes are contested.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel confirms the freeze notice.
- **Authority:**
  - UNB-XIN-EVA-010
- **Requirement consequences:**
  - UNB-XIN-EVA-010 DisputeFreeze blocked-command set.

### XID-EVA-005 (legal)

**Question.** Legal form of a sponsor-initiated bulk transfer of funds to another sponsor (grant to sponsoring organization vs asset transfer agreement) and ownership of historical records

**Decision.** A sponsor-initiated bulk transfer is a series of per-fund grants to the receiving sponsoring organization (DAF_TRANSFER_OUT), under a master agreement that records each fund's advisory terms. The source sponsor keeps its records; the receiving sponsor gets a reference copy of fund history, with donor notice before personal data is transferred.

**Rationale.** Each transfer is a distribution to another sponsoring organization, which is a permitted distribution; an asset assignment of the whole program would blur legal control.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel drafts the master agreement.
- **Authority:**
  - 26 USC 4966(c)(2) (text read at law.cornell.edu 2026-11-24)
  - UNB-XIN-EVA-009
- **Requirement consequences:**
  - UNB-XIN-EVA-009 legal form fixed.

### XID-INV-003 (legal)

**Question.** What securities-law basis supports each pooled option, and may pools hold assets of more than one sponsor?

**Decision.** Release 1: only single-sponsor investment pools maintained by the sponsor itself, relying on the charitable-organization exclusions added by the Philanthropy Protection Act of 1995 (Investment Company Act §3(c)(10)(B); Securities Act §3(a)(4)). Each pool records its legal basis; donors receive the written disclosure about the pool that the Act requires. Multi-sponsor pools and pools maintained by the operator are prohibited.

**Rationale.** The exclusion turns on the pool being maintained by a charitable organization; an operator-maintained or multi-sponsor pool falls outside it.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Securities counsel before any pool goes live.
- **Authority:**
  - 15 USC 80a-3(c)(10)(B) (read by INV pass)
  - Philanthropy Protection Act of 1995 (general knowledge)
- **Requirement consequences:**
  - UNB-XIN-INV-002 values restricted.

### XID-INV-004 (legal)

**Question.** Does any platform recommendation feature make the operator an investment adviser?

**Decision.** The operator provides no individualized investment recommendations. AI and analytics features may only implement sponsor-approved models and display descriptive information. Operator compensation never varies with assets or trades. An affiliate RIA is out of scope unless the owner later chooses it.

**Rationale.** Advisers Act §203(b)(4) covers the charity, not a for-profit vendor; removing advice and asset-based pay removes the status trigger.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Securities counsel confirms feature list.
- **Authority:**
  - 15 USC 80b-3 (read by INV pass)
  - RR-UNB-INV-019-r1
- **Requirement consequences:**
  - RR-UNB-INV-019-r1 adopted; UNB-XIN-REG-002 feature rows.

### XID-NPG-001 (legal)

**Question.** May a sponsor rely on ED written advice procured by another grantor or an ED repository, and on what contractual terms?

**Decision.** A sponsor may rely on equivalency-determination written advice obtained through a repository or another grantor when the advice is current under Rev. Proc. 2017-53 §3.03(1), the practitioner or repository authorizes reliance by the sponsor in writing, and the sponsor has no contrary knowledge. Expiry is computed from the grantee's taxable year.

**Rationale.** Reusing current advice is the cheapest lawful path for foreign grants; written reliance authorization preserves good faith.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel approves the reliance-letter template.
- **Authority:**
  - Rev. Proc. 2017-53 §§2, 3.03(1) (read 2026-11-24)
  - 26 CFR 53.4945-5(a)(5)
- **Requirement consequences:**
  - UNB-XIN-NPG-001; RR-UNB-GRANT-041-r1.

### XID-NPG-002 (legal)

**Question.** What purpose wording to an intermediary avoids an impermissible earmark for DAF grants?

**Decision.** Grants to US intermediaries may carry a non-binding preference ('to support [intermediary]'s work in [country/program]'). They may never name a specific foreign grantee as a required recipient or require pass-through. The intermediary must retain discretion and control. Wording is catalogued per intermediary.

**Rationale.** Discretion and control by the domestic charity is what makes the grant a grant to it rather than an earmarked foreign grant.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel approves the catalogue.
- **Authority:**
  - Rev. Rul. 63-252; Rev. Rul. 66-79 (general knowledge)
- **Requirement consequences:**
  - UNB-XIN-NPG-002 wording rule.

### XID-NPG-003 (legal)

**Question.** Enable §530A general funding contributions? Under what §4967 test for broad classes?

**Decision.** §530A general funding contributions stay disabled for every sponsor.

**Rationale.** Optional (P3), temporary rule expiring 2029-09-30, and the §4967 analysis for broad classes is not documented.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** None.
- **Authority:**
  - Temp. Reg. §1.530A-7T (FR 2026-20026)
- **Requirement consequences:**
  - UNB-XIN-NPG-007 default off.

### XID-NPG-005 (legal)

**Question.** Payee substitution to a merger successor: donor reconfirmation required?

**Decision.** When a grantee merges, a legal successor with the same charitable purpose and an eligible status may receive open and recurring grants with sponsor approval and notice to the donor (no reconfirmation). If the purpose or recipient class changes, or the successor is not eligible, the grant is cancelled and the donor asked to re-recommend.

**Rationale.** A merger successor takes the predecessor's assets and obligations; notice preserves donor awareness without needless friction.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel.
- **Authority:**
  - UNB-XIN-NPG-005
- **Requirement consequences:**
  - UNB-XIN-NPG-005 change-effect table.

### XID-NPG-006 (legal)

**Question.** Confirm that §4966(c)(2)(A) excepts distributions to public charities irrespective of stated purpose, and which sponsor exempt-status purpose limits remain.

**Decision.** Confirmed from statute: a distribution from a DAF to a §170(b)(1)(A) organization (other than a disqualified supporting organization) is not a taxable distribution under §4966 whatever its stated purpose. Purpose monitoring of such grants is optional sponsor policy. §4967 prohibited-benefit screens (tickets, dues with benefits, pledges of individuals, tuition) remain mandatory.

**Rationale.** The §4966(c)(2)(A) exception text has no purpose condition; §4967 is a separate tax.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** None for the statutory reading.
- **Authority:**
  - 26 USC 4966(c)(2) (text read at law.cornell.edu 2026-11-24)
- **Requirement consequences:**
  - RR-UNB-GRANT-024-r1 tiering; REQ-X-069 monitoring optional.

### XID-NPG-007 (legal)

**Question.** Is a §4966 taxable distribution correctable, and what is the incident reporting channel (Form 4720) for sponsor and managers?

**Decision.** §4966 and §4967 first-tier taxes are 'qualified first tier taxes' under §4962 (subchapter G), so they are abatable if the event was due to reasonable cause, not wilful neglect, and is corrected within the correction period. The incident workflow therefore records reasonable-cause evidence and correction steps for every incident. Excise taxes are reported on Form 4720 by each liable person (sponsor; fund manager; donor/advisor for §4967). §4958 excess benefit is corrected within the correction period and also reported on Schedule L.

**Rationale.** Verified §4962(b) text; correction plus reasonable cause is the route to abatement, so the workflow must capture it.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel confirms correction mechanics per tax.
- **Authority:**
  - 26 USC 4962(a)-(b): qualified first tier taxes include subchapter G (§§4966-4967); abatement on reasonable cause + correction (read 2026-11-24)
  - 26 USC 4958, 4966, 4967
- **Requirement consequences:**
  - UNB-XIN-NPG-008 fields: reasonable_cause_evidence, correction_steps, correction_deadline.

### XID-REC-001 (legal)

**Question.** After sponsor exit, how long does the platform keep read-only data and backups, and who is records custodian?

**Decision.** After a sponsor exits, the sponsor is the records custodian. The platform keeps a read-only copy for 90 days after the reconciled export is delivered, then deletes it; backups expire on their normal schedule (no more than 35 days later). Legal holds override deletion. The exit certificate lists any residual copies and their expiry.

**Rationale.** The sponsor owns the records; time-limited residuals are disclosed rather than hidden.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel confirms contract wording.
- **Authority:**
  - RR-UNB-TEN-016-r1
  - RCC-PLT-008
- **Requirement consequences:**
  - RR-UNB-TEN-016-r1 timings.

### XID-REG-001 (legal)

**Question.** Does instruction-only authority over sponsor-titled accounts keep the operator outside state money-transmission laws in target states?

**Decision.** Architecture rule: the operator never takes title to or possession of contribution or grant money; it holds instruction-only authority over sponsor-titled accounts. No embedded or white-label deployment that touches money goes live until a multistate money-transmission memo is on file.

**Rationale.** Licensing and insolvency exposure is avoided structurally rather than by licence.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Multistate counsel memo (gating).
- **Authority:**
  - UNB-XIN-REG-001
  - XRA-008
- **Requirement consequences:**
  - UNB-XIN-REG-001 gate.

### XID-REG-002 (legal)

**Question.** Which operator features create investment-adviser or broker-dealer status?

**Decision.** Operator features are classified as follows: investment advice or model construction for compensation (adviser risk) — disabled; compensation varying with securities transactions or custody of securities (broker-dealer risk) — prohibited, operator fees are fixed SaaS fees; hosting donor solicitations (California platform risk) — see XID-SAS-005. A feature with an unmapped status question cannot be enabled.

**Rationale.** Status follows the operator's own conduct and compensation, not the sponsor's exclusions.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Securities counsel.
- **Authority:**
  - 15 USC 80b-3; XRA-009; XRA-010
- **Requirement consequences:**
  - UNB-XIN-REG-002 register.

### XID-REG-003 (legal)

**Question.** Does the for-profit operator sending SMS on a sponsor's behalf retain any nonprofit TCPA exemption?

**Decision.** Automated or prerecorded calls and texts sent by the operator on behalf of the sponsor (a tax-exempt nonprofit) require at least prior express consent, which the regulation permits for calls 'made by or on behalf of a tax-exempt nonprofit organization'. Messages sent on behalf of a for-profit partner, or that advertise a for-profit's services, require prior express written consent.

**Rationale.** Verified 47 CFR 64.1200 text: the nonprofit standard extends to agents acting on the nonprofit's behalf, but not to a for-profit partner's own marketing.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** None beyond consent-capture wording.
- **Authority:**
  - 47 CFR 64.1200(a)(2), (a)(3)(iv), (f)(15)(iii): 'made by or on behalf of a tax-exempt nonprofit organization' (read 2026-11-24)
- **Requirement consequences:**
  - UNB-XIN-CRM-003 consent classes by sender.

### XID-SAS-002 (legal)

**Question.** In a distribution-partner exit, may the original sponsor approve outbound transfer grants by a batch policy, and may it decline or impose conditions?

**Decision.** In a partner exit the original sponsor may adopt a written exit policy as the exercise of its discretion. The policy is applied per fund: each transfer is a separate decision with its own record citing the policy, hold and sanctions checks. The sponsor may decline or condition individual transfers (open holds, sanctions matches, expenditure-responsibility grants outstanding, unresolved disputes). A contractual pre-commitment to transfer that removes sponsor discretion is not permitted.

**Rationale.** Preserves exclusive legal control while allowing scale; resolves XIC-ADV-008 because the batch never carries the approval.

- **Confidence:** medium-high · **Status:** decided-provisional · **Confirmation required:** Counsel reviews the policy template.
- **Authority:**
  - IRC 4966(d)(2)(A)(ii)
  - UNB-XIN-EVA-011
- **Requirement consequences:**
  - UNB-XIN-SAS-007 and UNB-XIN-EVA-009 per-fund decision records.

### XID-SAS-005 (legal)

**Question.** Which operating models make the platform company a California charitable fundraising platform (Type A/E) and what registration/disclosure follows?

**Decision.** Assume the sponsor's California exclusion does not cover the platform company. D2C own-brand apps and charity-side giving buttons are not launched to California users until a California opinion is on file; other models may launch.

**Rationale.** Conservative gating where the cost of being wrong is registration violations by the vendor.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** California counsel (gating for those two features).
- **Authority:**
  - OB-38; Cal. Gov. Code §12599.9 (search results only)
- **Requirement consequences:**
  - Entitlement flag per model and state (UNB-XIN-SAS-001).

### XID-SVC-002 (legal)

**Question.** Record calls at release 1? If yes, is a uniform all-party disclosure with continuation-as-consent acceptable in all target states, and may an AI vendor transcribe?

**Decision.** No call recording and no machine transcription of voice calls at release 1. If enabled later: a uniform explicit all-party consent prompt at the start of every call, no recording without an affirmative answer, and a transcription vendor contracted as a service provider with no training use.

**Rationale.** Caller location is unknowable and several states require all-party consent; the simplest valid option is not to record.

- **Confidence:** high · **Status:** decided-provisional · **Confirmation required:** None for release 1; counsel before enabling.
- **Authority:**
  - 18 USC 2511(2)(d); Fla. Stat. 934.03 (read by SVC pass)
- **Requirement consequences:**
  - UNB-XIN-SVC-003 release-1 setting: disabled.

### XID-TRS-001 (legal)

**Question.** Can an agent under POA, guardian or conservator exercise DAF advisory privileges?

**Decision.** An agent under a power of attorney may exercise advisory privileges only if the instrument expressly grants authority to make gifts or charitable donations, the sponsor's program terms allow representatives, and the instrument is verified. Guardians and conservators act only under a court order that covers charitable gifts. Otherwise the privileges are suspended pending succession or court order.

**Rationale.** Most states, following the Uniform Power of Attorney Act, require an express grant for gift-making authority.

- **Confidence:** medium · **Status:** decided-provisional · **Confirmation required:** Counsel per target state.
- **Authority:**
  - Uniform Power of Attorney Act §201 (general knowledge)
- **Requirement consequences:**
  - UNB-XIN-TRS-004 rule.

<!-- END GENERATED: outcomes -->

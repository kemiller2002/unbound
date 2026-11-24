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

The requirement consequences listed under each decision are **not yet applied** to the requirement records. They are applied when the records are adopted into the domain files (RCD-001), so that each change carries the decision as its provenance.

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

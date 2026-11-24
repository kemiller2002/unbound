# 15: Open decisions and acceptance scenarios

## Open decisions

Each decision below needs human, legal or accounting authority. No decision here has been made by an agent. Where a recommendation exists, it is advisory. Decisions that block implementation are marked.

<!-- BEGIN GENERATED: decision-types -->
| Type | Open decisions |
|---|---|
| legal | 29 |
| product | 18 |
| governance | 15 |
| accounting | 12 |
| architecture | 12 |
<!-- END GENERATED: decision-types -->

<!-- BEGIN GENERATED: decisions -->
| Decision | Type | Question | Owner | Recommendation | Blocks impl. | Status |
|---|---|---|---|---|---|---|
| XID-BNK-001 | product | Will the sponsor accept card contributions, and with what maximum per gift and contest threshold? | Sponsor CFO | Accept cards only for D2C/embedded/high-volume models; cap per gift (e.g., $10,000 illustrative); contest disputes ≥ $1,000 illustrative. | N | open |
| XID-BNK-002 | legal | Is a donor-covered processing fee part of the contribution stated on the acknowledgment? | Counsel + CPA | Gross charged is the contribution; fee is sponsor expense. | Y | decided-provisional |
| XID-BNK-003 | legal | Is a voided/stale DAF grant check property owed to the charity for unclaimed-property purposes, and which state rules (incl. B2B exemptions) apply? | Counsel | Build the rule table; treat as reportable unless counsel records exemption. | Y | decided-provisional |
| XID-BNK-004 | governance | Are instant payment rails allowed for grants? | Sponsor CFO + compliance | Not in release 1. | N | open |
| XID-BNK-005 | legal | Who is Originator of record and does any vendor hold funds (TPS/money transmission)? | Counsel + sponsor CFO | Sponsor-titled accounts; sponsor as Originator; administrator as TPSP only. | Y | decided-provisional |
| XID-BNK-006 | accounting | Card reversal window and provisional credit policy for cards and business ACH. | Sponsor CFO | Card 120 days default; business debit 2 banking days + 1 buffer; OD-04 limit 0 for new donors. | N | decided-provisional |
| XID-BNK-007 | architecture | Which bank activity feed (BAI2/camt.053/API) per sponsor bank, and daily latency? | CTO + treasury | Require next-morning prior-day feed minimum; intraday where available. | N | open |
| XID-BNK-008 | legal | Do ACH debit blocks on disbursement accounts conflict with any processor model? | Treasury | Block all debits except named company IDs. | N | decided-provisional |
| XID-INV-001 | accounting | Which GrantRecognitionPoint does each sponsor adopt, and does approval without notice to the charity create a promise to give? | Sponsor CFO + external CPA | SponsorApproval default (matches audited practice) with communication captured; CPA to confirm communication requirement. | N | decided-provisional |
| XID-INV-002 | accounting | Is the DAF component presented as net assets without donor restrictions for every program version, and which account types are agency/pass-through? | Sponsor CFO + CPA + counsel | Without donor restrictions where variance power evidenced; agency for nonprofit-beneficiary funds. | Y | decided-provisional |
| XID-INV-003 | legal | What securities-law basis supports each pooled option, and may pools hold assets of more than one sponsor? | Securities counsel | Sponsor-maintained single-sponsor pools only for release 1. | Y | decided-provisional |
| XID-INV-004 | legal | Does any platform recommendation feature make the operator an investment adviser? | Securities counsel | Restrict to approved-model implementation and analytics. | Y | decided-provisional |
| XID-INV-005 | accounting | Schedule D line 4 valuation basis and line 3 grant basis (approved vs paid). | Sponsor CFO + CPA | Same basis as GAAP, bridge differences. | N | decided-provisional |
| XID-INV-006 | accounting | UBIT recognition policy (accrue estimate vs recognize on filing) and fund allocation rule. | Sponsor CFO + tax advisor | Accrue estimate at fund level for capacity; post per CPA. | N | decided-provisional |
| XID-INV-007 | architecture | Which functions is Unbound book of record for, per sponsor model? | Product owner + sponsor CFO | FundAttribution and FundStatements authoritative always; GL configurable. | Y | open |
| XID-INV-008 | governance | Is a SOC 1 Type II required before the first outsourced/white-label sponsor? | Operator executive + compliance | Yes, Type I at launch, Type II within 12 months. | N | open |
| XID-SVC-001 | architecture | Adopt case kernel with per-kind profiles instead of RCF-PLT-025's single shared lifecycle? | Architecture owner | Yes; profiles code-defined (F# types) with sponsor parameters. | Y | open |
| XID-SVC-002 | legal | Record calls at release 1? If yes, is a uniform all-party disclosure with continuation-as-consent acceptable in all target states, and may an AI vendor transcribe? | Counsel | Do not record at release 1; if later, uniform explicit consent. | Y | decided-provisional |
| XID-SVC-003 | product | Complaint definition, acknowledgement and response defaults, and regulator-originated routing. | Head of Client and Partner Operations | Dissatisfaction-based definition; 2/15 business days defaults. | N | open |
| XID-SVC-004 | governance | Values of the ContactAssuranceMatrix; is KBA permitted for anything beyond public info? | Security owner | KBA public-only; possession factor for account data. | Y | open |
| XID-SVC-005 | product | Which sponsor models offer live phone channels at release 1? | Product owner | None; async + scheduled callback. | N | open |
| XID-CLM-001 | governance | Reconsideration defaults: filing window, decision target, independence for single-reviewer sponsors, and whether charities may reconsider shared vetting. | Sponsor governance owner + counsel | 90 days, 30 days, outsourced/board reviewer allowed, charity reconsideration on platform vetting only. | N | open |
| XID-CLM-002 | legal | Legal basis, maximum duration and liability for protective suspension of a representative. | Counsel | Allow with short max (e.g. 15 business days) and reviewer renewal. | Y | decided-provisional |
| XID-CLM-003 | legal | Approve the public decline reason catalog and neutral codes. | Counsel | Approve ~10 codes. | N | decided-provisional |
| XID-CLM-004 | governance | Decision-quality sample sizes and reviewer qualification. | Compliance owner | Risk-stratified; minimum 30/month per stratum or 100% below that. | N | open |
| XID-CLM-005 | product | Publish decision-time metrics to donors? | Product owner | Sponsor option. | N | open |
| XID-ERP-001 | accounting | Billing basis: average daily balance with daily accrual, or period-end balance? | Sponsor CFO | Daily accrual on daily balance; declared day-count. | Y | decided-provisional |
| XID-ERP-002 | legal | Tax characterization of donor-paid membership fees to a 501(c)(3) sponsor. | Counsel + CPA | Treat as payment for services, not acknowledged. | Y | decided-provisional |
| XID-ERP-003 | product | Confirm ERP boundary: AP, procurement, budgeting, HR, payroll, fixed assets are integrations; AR only for partner invoices. | Product owner | Adopt (consistent with RCD-013). | Y | open |
| XID-ERP-004 | product | Allow relationship/household fee-tier aggregation? | Sponsor CFO | Off by default; opt-in with consent. | N | open |
| XID-ERP-005 | governance | Approve cumulative DoA aggregation key and default window (30 days). | Sponsor board/finance committee | Approver x counterparty, 30 days. | N | open |
| XID-CRM-001 | legal | Is sponsor email promoting DAF services 'commercial' under CAN-SPAM; which entity is sender in white-label? | Counsel | Treat as commercial; sender = brand presented in From line, contractually designated. | Y | decided-provisional |
| XID-CRM-002 | legal | Referral compensation to advisors/affiliates from sponsor operating funds: solicitor registration and §4958 exposure. | Counsel | Block until rule recorded. | Y | decided-provisional |
| XID-CRM-003 | architecture | Shared party core with capacity-partitioned profiles vs separate party per role. | Architecture owner | Shared minimal core + partitioned profiles. | Y | open |
| XID-CRM-004 | legal | May a charity that holds an organization-advised fund recommend grants to itself? | Counsel | Classify account type first (UNB-RCN-ENT-011). | Y | decided-provisional |
| XID-TRS-001 | legal | Can an agent under POA, guardian or conservator exercise DAF advisory privileges? | Counsel | Only when instrument covers charitable acts and sponsor policy permits. | Y | decided-provisional |
| XID-TRS-002 | product | Default joint-authority mode for co-advisors when not declared. | Sponsor operations owner | AllRequired (consistent with AUTH-I4). | N | open |
| XID-TRS-003 | governance | Default successor response window. | Sponsor operations owner | 90 days (observed at Fidelity) as default, configurable. | N | open |
| XID-NPG-001 | legal | May a sponsor rely on ED written advice procured by another grantor or an ED repository, and on what contractual terms? | Sponsor counsel | Permit with reliance letter naming the sponsor. | Y | decided-provisional |
| XID-NPG-002 | legal | What purpose wording to an intermediary avoids an impermissible earmark for DAF grants? | Sponsor counsel | Approve catalogue wording per intermediary. | Y | decided-provisional |
| XID-NPG-003 | legal | Enable §530A general funding contributions? Under what §4967 test for broad classes? | Sponsor counsel | Keep disabled until Treasury process and §4967 view are documented. | N | decided-provisional |
| XID-NPG-004 | governance | What criteria and maximum review period govern discretionary holds? | Sponsor board | Adjudicated findings or regulator actions only; 90-day maximum review. | Y | open |
| XID-NPG-005 | legal | Payee substitution to a merger successor: donor reconfirmation required? | Sponsor counsel | Sponsor approval plus donor notice (not reconfirmation) for same-purpose successors. | N | decided-provisional |
| XID-NPG-006 | legal | Confirm that §4966(c)(2)(A) excepts distributions to public charities irrespective of stated purpose, and which sponsor exempt-status purpose limits remain. | Sponsor counsel | Confirm and keep purpose monitoring optional. | N | decided-provisional |
| XID-NPG-007 | legal | Is a §4966 taxable distribution correctable, and what is the incident reporting channel (Form 4720) for sponsor and managers? | Sponsor counsel and CPA | Document per provision. | N | decided-provisional |
| XID-REG-001 | legal | Does instruction-only authority over sponsor-titled accounts keep the operator outside state money-transmission laws in target states? | Operator counsel | Obtain multistate memo before first embedded deployment. | Y | decided-provisional |
| XID-REG-002 | legal | Which operator features create investment-adviser or broker-dealer status? | Operator counsel | Map before enabling advisory features. | Y | decided-provisional |
| XID-REG-003 | legal | Does the for-profit operator sending SMS on a sponsor's behalf retain any nonprofit TCPA exemption? | Counsel | Assume no; obtain prior express consent. | N | decided-provisional |
| XID-GOV-001 | product | Which authorities get golden fixtures first? | Compliance engineering | Rev. Proc. 2017-53 §7; Rev. Proc. 2018-32 timing; §1.530A-7T dates. | N | open |
| XID-SAS-001 | architecture | Does any operating model require Profile C (dedicated cell) or a separate implementation at release 1? | Architecture owner + product owner | No. Serve all 10 models from one release; activate Profile C only on a cited contract clause (UNB-RCN-PLT-012). | N | open |
| XID-SAS-002 | legal | In a distribution-partner exit, may the original sponsor approve outbound transfer grants by a batch policy, and may it decline or impose conditions? | Counsel | Permit batch-policy approval with per-fund recommendation evidence; sponsor retains discretion; document declines. | Y | decided-provisional |
| XID-SAS-003 | product | Metering allocation keys and whether sponsors see their usage. | Economics owner | Internal only at release 1; keys = accounts, API calls, AI tokens, storage. | N | open |
| XID-SAS-004 | governance | Initial SLO targets and contractual SLA ceiling. | Product owner | Three SLOs at 99.5-99.9%; SLAs no tighter. | N | open |
| XID-SAS-005 | legal | Which operating models make the platform company a California charitable fundraising platform (Type A/E) and what registration/disclosure follows? | Counsel | Obtain opinion before D2C own-brand app or charity-side button launch. | N | decided-provisional |
| XID-IAM-001 | governance | Default operator-access delegation categories sponsors pre-approve. | Sponsor + security owner | Read-only diagnostics and document regeneration pre-approved; all writes per-case. | Y | open |
| XID-REC-001 | legal | After sponsor exit, how long does the platform keep read-only data and backups, and who is records custodian? | Counsel + sponsor contract | Sponsor is custodian; platform read-only period 90 days; backups expire on schedule; certificate discloses residuals. | N | decided-provisional |
| XID-WFA-001 | architecture | Adopt a durable-workflow engine or in-database state machines + outbox + timer table? | Architecture owner | In-database; revisit if >50 long-running process types, cross-cell orchestration, or scheduler incidents exceed threshold. | N | open |
| XID-OTH-001 | product | Minimum notice period and maximum length for migration transition windows. | Product owner | 30-day notice; max 3 business days freeze; no Dec 15-Jan 5. | N | open |
| XID-ECO-001 | governance | Who owns the cost attribution model and who verifies savings? | CFO + COO | CFO/FP&A owns rules and verification; COO owns improvement initiatives; Compliance owns counter-metric floors. | Y | open |
| XID-ECO-002 | legal | Which service charges may be charged to DAF funds, on what cost basis and with what disclosure (and is a recovery ceiling appropriate)? | Sponsor counsel | Permit cost-based special-request charges with disclosure; ceiling 1.25 pending counsel. | Y | decided-provisional |
| XID-ECO-003 | accounting | Residual tolerance threshold for ReconciliationRun | Controller | 2% of pool or $5,000, whichever greater. | N | decided-provisional |
| XID-ECO-004 | product | Allocation method for shared platform costs across sponsors (and whether shown to sponsors) | Commercial owner + CFO | Driver-based by weighted events and storage; showback with disclosed method; chargeback only by contract. | N | open |
| XID-ECO-005 | accounting | Relationship between management ABC and Form 990 functional expense allocation | CPA / Controller | Same activity dataset, separate governed mappings; CPA approves functional mapping annually. | N | decided-provisional |
| XID-ECO-006 | governance | Benchmark privacy thresholds (k and dominance) and staff data minimum group size | Privacy officer + HR | k=5, dominance 60%, group size 5. | N | open |
| XID-ECO-007 | product | Should donor-facing cost quotes be shown for charged operations, and is quote-overrun re-affirmation required? | Product owner + counsel | Yes for charged operations. | N | open |
| XID-ECO-008 | governance | Structural cost floors (SoD/minimum staffing) used by the ratchet | COO + Compliance | Set per sponsor from SoD design; revisit annually. | Y | open |
| XID-ECO-009 | architecture | Pin FOCUS version and extension columns for non-technology costs | Architecture owner | FOCUS 1.4 subset + x_Bearer, x_SourceInvoiceRef, x_CostObject. | N | open |
| XID-EVA-001 | architecture | Adopt the hybrid architecture ADR (authoritative ledger and fact tables in one PostgreSQL database per sponsor, transactional outbox, DB-row process managers; no broker)? | Architecture owner | Adopt as Proposed; run the benchmark in UNB-XIN-EVA-013 before release 1 | Y | open |
| XID-EVA-002 | accounting | Default treatment for postings whose effective date falls in a hard-closed period: current-period prior-period adjustment, reopen, or restatement, and the materiality threshold for each | Sponsor controller + CPA | Current-period adjustment with relates_to_period below threshold; reopen only with dual approval | Y | decided-provisional |
| XID-EVA-003 | accounting | Who bears intermediary fees on partially returned wires and bank fees on returned payments: the fund, the sponsor operating budget, or the recipient? | Sponsor CFO + counsel | Sponsor operating expense by default; fund only if disclosed in fee schedule | N | decided-provisional |
| XID-EVA-004 | legal | During a fund dispute freeze, do fee accruals, investment rebalancing and recurring grants continue? | Sponsor counsel | Fees accrue; rebalancing paused unless policy; recurring grants held | N | decided-provisional |
| XID-EVA-005 | legal | Legal form of a sponsor-initiated bulk transfer of funds to another sponsor (grant to sponsoring organization vs asset transfer agreement) and ownership of historical records | Counsel of both sponsors | Per-fund DAF_TRANSFER_OUT distributions with a master agreement; source retains records, destination gets reference copy | Y | decided-provisional |
| XID-EVA-006 | product | Default instruction-authority policy for multi-advisor funds (any-one, all, designated primary) per instruction kind | Product owner + sponsor counsel | Any-one for grants; all for transfers-out, closure and investment allocation | N | open |
| XID-EVA-007 | architecture | Event payload PII policy: reference-only payloads (ids into PII vault) for all authoritative facts? | Architecture owner + privacy counsel | Yes: facts carry party ids, not names/addresses | Y | open |
| XID-ADV-001 | product | Which optional money-movement and legal features are in release 1 (card contributions, ACH debit intake, direct foreign grants, call recording, pooled investment vehicles, distribution partners, embedded checkout)? | Product owner + general counsel | Exclude card intake, call recording, direct foreign grants (use intermediaries) and partner exit tooling from R1; this removes ~8 conditional P0s. | Y | open |
| XID-ADV-002 | product | Is high-volume-low-balance a release-1 business model, and if so what minimum economics (fee floor, servicing class, digital-only) are acceptable? | Product owner + CFO + counsel | Not an R1 model unless UNB-XIN-ADV-001 is adopted and a fee floor passes the XID-ECO-002 reasonableness review. | Y | open |
| XID-ADV-003 | architecture | How is grantable capacity computed when the general ledger or fund attribution is Shadow (external book of record)? | Controller + architecture owner | Adopt UNB-XIN-ADV-002 and scope EVA-001 to Authoritative functions. | Y | open |
| XID-ADV-004 | governance | Default co-advisor authority mode by instruction kind (resolves XID-TRS-002 vs XID-EVA-006). | Product owner + counsel | Any-one for grants below threshold; AllRequired for large grants, transfers-out, closure, successor and authority changes. | Y | open |
| XID-ADV-005 | accounting | Routing of prior-year adjustments discovered during and after the audit (AuditAdjustment period vs PriorPeriodAdjustment). | Controller + auditor | AuditAdjustment period until audit sign-off; PPA thereafter. | Y | decided-provisional |
| XID-ADV-006 | accounting | Which GrantRecognitionPoint values are permitted? | Controller + auditor | Permit ApprovalCommunicatedToGrantee; SponsorApproval only where CPA documents contemporaneous communication; PaymentReleased only where payment is the first communication. | N | decided-provisional |
| XID-ADV-007 | architecture | Are fund-level fee accruals and income attributions posted daily or computed daily and posted at assessment? | Controller + architecture owner | Compute daily, post at assessment; capacity subtracts accrued-unposted. | Y | open |
| XID-ADV-008 | architecture | Are authenticator bindings (verified phone/email for OTP, registered devices) authority facts or engagement data? | Security owner | Authority facts owned by IAM; changes need step-up on an existing factor and notice to prior channel. | N | open |
| XID-ADV-009 | product | What authentication strength applies to donors and advisors for grant recommendations, and may a partner IdP perform step-up? | Security owner + product owner | AAL2 (any authenticator) for grant recommendations; phishing-resistant for payee/authority changes and workforce; partner IdP step-up allowed by contract with verified acr/amr. | N | open |
| XID-ADV-010 | governance | Who provides counsel/CPA-approved default configuration packs (rail obligations, unclaimed property, decline reasons, assurance matrix, recognition point, discretionary policy), and can a sponsor adopt them by attestati… | General counsel (platform) + sponsor counsel | Platform maintains reviewed default packs; sponsors attest adoption and may only tighten; deviations need sponsor counsel. | Y | open |
| XID-ADV-011 | product | Release-1 scope of the economics capability. | Product owner + CFO | Six metrics, monthly pack, AI metering and non-posting attribution with GL reconciliation; everything else P3 or Echelon offline. | N | open |
| XID-INT-001 | product | Is message legal classification (UNB-XIN-CRM-003 / UNB-XIN-REG-003) P0 for all channels or only for SMS and prerecorded voice? | Requirements owner with counsel | P0 for SMS and voice (statutory damages per message); P1 for email. | N | open |
<!-- END GENERATED: decisions -->

## Acceptance and adversarial scenarios

Every new requirement and every refinement is cited by at least one scenario (gate D7). Each scenario records preconditions, stimulus, expected states, financial invariants, audit records, recovery behavior and mock-data notes. The full records are in `data/scenarios.csv`. Scenarios use synthetic data only. None has been executed, because no implementation exists.

<!-- BEGIN GENERATED: scenarios -->
| Scenario | Title | Requirements | Stimulus | Expected states |
|---|---|---|---|---|
| XDS-BNK-001 | Card dispute after gift funded a grant | UNB-XIN-PAY-003;RR-TVR-CON-001-r1;REQ-X-009 | Acquirer webhook DisputeOpened reason 'fraud' on day 40. | Contribution DisputeOpened; ReturnedContributionExposure; fund over-committed $500; new reservations blocked; grant unchanged; acknowledgment correction task. |
| XDS-BNK-002 | Dispute webhook delivered twice and then won | UNB-XIN-PAY-003 | Duplicate DisputeOpened, then DisputeWon. | One exposure; one re-credit. |
| XDS-BNK-003 | Recurring debit without authorization copy | UNB-XIN-PAY-001;RR-UNB-PAY-008-r1 | First occurrence due. | Occurrence Held AuthorizationMissing; donor prompted to authenticate. |
| XDS-BNK-004 | Revocation race with file transmission | UNB-XIN-PAY-001 | Donor revokes at 14:05. | Current entry stays; future occurrences cancelled; donor notified which debit remains. |
| XDS-BNK-005 | WEB debit to unvalidated account | UNB-XIN-PAY-002 | Submit $500 contribution. | AwaitingAccountValidation; no entry. |
| XDS-BNK-006 | R10 retry attempt blocked and rate alert | UNB-XIN-PAY-005 | R10 return plus operator retry click. | Retry rejected; schedule suspended; alert to compliance. |
| XDS-BNK-007 | Card-testing burst on embedded form | UNB-XIN-PAY-004 | 200 $1 attempts across 50 cards in 10 min. | Channel suspended for host; incident; acquirer calls capped. |
| XDS-BNK-008 | Donor-covered fee and net payout | UNB-XIN-PAY-006;UNB-XIN-BNK-008 | Processor payout net. | Batch reconciles only if Σgross−Σfees−reversal = net = bank credit. |
| XDS-BNK-009 | Ambiguous DTC delivery | UNB-XIN-PAY-007;RR-UNB-GAP-013-r1 | One delivery 100 sh. | Unidentified suspense; no ack; both intents awaiting. |
| XDS-BNK-010 | Unreferenced wire contribution ages to return | RR-UNB-GAP-013-r1 | Return threshold reached. | ReturnToSender proposal to originating account only, human approval. |
| XDS-BNK-011 | Out-of-band wire from bank portal | UNB-XIN-BNK-003;RR-UNB-PAY-008-r1 | $48,000 wire debit with no platform instruction. | Incident within 1h; account releases held; two roles notified. |
| XDS-BNK-012 | Bank feed missing | UNB-XIN-BNK-003 | Cut-off passes. | Account Unverified; FeedMissing exception; releases continue only if policy allows with flag. |
| XDS-BNK-013 | Grant attempted over instant rail | UNB-XIN-BNK-004;UNB-XIN-BNK-001 | Release via RTP. | Rejected RailNotEligible. |
| XDS-BNK-014 | Large wire requires independent confirmation | UNB-XIN-BNK-004;UNB-XIN-BNK-002 | $300,000 wire grant release. | Held for independent callback; FalsePretenses alert. |
| XDS-BNK-015 | Fraud program review overdue | UNB-XIN-BNK-002;RR-UNB-PAY-008-r1 | ACH release. | Held FraudProgramReviewOverdue. |
| XDS-BNK-016 | Check printed before issue file acknowledged | UNB-XIN-BNK-006 | Mail release requested. | Refused IssueFileNotAcknowledged. |
| XDS-BNK-017 | Altered payee exception at cut-off | UNB-XIN-BNK-006 | No decision by cut-off. | Default Return; case. |
| XDS-BNK-018 | Stale grant check with no state rule | UNB-XIN-BNK-007 | Dormancy reached. | UnclaimedPropertyReview; counsel task. |
| XDS-BNK-019 | Projected disbursement shortfall | UNB-XIN-BNK-009 | Projection run. | Alert; releases beyond $0.9m held. |
| XDS-BNK-020 | Reissued check emits cost drivers | UNB-XIN-BNK-005 | Reissue dispatched. | CostDriver facts for original check, void, ACH reissue, exception minutes. |
| XDS-BNK-021 | Business debit finalises after R29 window | RR-TVR-CON-001-r1 | 3 banking days, no return. | Final; grantable. |
| XDS-BNK-022 | Rail go-live gate incomplete | RR-UNB-PAY-008-r1;UNB-XIN-BNK-001 | EnableRail(ACH-debit). | Gate fails listing missing items. |
| XDS-ACC-001 | Posting-rule upgrade during outage replay | RR-ACC-MDL-007-r1;UNB-XIN-ACC-003 | Rule v4 deployed; outage replay reprocesses 300 events of Dec 2025. | No new journals; replay log links existing; re-derivation flags 0 diffs under v3, impact analysis under v4. |
| XDS-ACC-002 | Code defect detected by re-derivation | UNB-XIN-ACC-003 | A refactor changes rounding in fee posting rule implementation. | Monthly re-derivation shows 112 line differences; exception opened; ledger unchanged. |
| XDS-ACC-003 | December grant surge with communication-based recognition | UNB-XIN-ACC-002 | 4,000 grants approved Dec 29-31; award emails for 600 sent Jan 2. | 3,400 recognized in Dec; 600 listed on cutoff register, recognized Jan. |
| XDS-ACC-004 | Conditional matching grant | UNB-XIN-ACC-002 | Grantee reports match met in March. | ConditionalGrant until BarrierMet; expense in March. |
| XDS-ACC-005 | Agency endowment receipt and payout | UNB-XIN-ACC-001 | $50k in, $10k out. | Liability $40k; no revenue/expense; excluded from Schedule D. |
| XDS-ACC-006 | Legacy fund import without variance power evidence | RR-UNB-ACC-033-r1;UNB-XIN-ACC-001 | Import posts opening balances. | 15 funds in suspense classification; close exception. |
| XDS-ACC-007 | Recognition point change applied as change in principle | UNB-XIN-ACC-004;UNB-XIN-ACC-002 | CFO adopts SponsorApproval for FY2027 with retrospective application. | Cumulative-effect journal to opening FY2027 net assets... as-adjusted FY2026 comparatives versioned; originals preserved. |
| XDS-ACC-008 | Auditor AJE after statements issued | UNB-XIN-ACC-005 | Auditor proposes $30k fee accrual (booked) and $4k (passed). | AJE in AuditAdjustment period; SUD lists $4k; no fund statements reissued (below threshold). |
| XDS-ACC-009 | Schedule D bridge with internal transfers | UNB-XIN-ACC-006;UNB-XIN-ACC-001 | Bridge generated. | All differences typed; residual 0; sign-off allowed. |
| XDS-ACC-010 | Disclosure tags at year end | UNB-XIN-ACC-007;UNB-XIN-INV-005 | Year-end disclosure run. | PE in NAV-PE column; gross manager fees available for 990. |
| XDS-ACC-011 | S-corp stock gift and sale | UNB-XIN-ACC-008;RR-UNB-ACC-031-r1 | Accept shares basis $100k; sell $900k. | UBIT estimate on $800k gain; fund capacity reduced. |
| XDS-ACC-012 | SOC 1 readiness for outsourced sponsor | UNB-XIN-ACC-009 | March readiness check. | Gap notification 90 days before FYE or report in place; CUEC list generated. |
| XDS-ACC-013 | Shadow GL mode with administrator | UNB-XIN-ACC-010 | Contribution accepted; administrator books it next day with $10 difference. | No Unbound GL posting; break opened. |
| XDS-INV-001 | Advisor fee overbilling | UNB-XIN-INV-001 | Custodian debit $2,600 vs expected $2,500. | Exception; hold; refund request. |
| XDS-INV-002 | Cross-sponsor pool subscription | UNB-XIN-INV-002 | Sponsor B fund subscribes. | Rejected. |
| XDS-INV-003 | AI rebalance proposal outside approved model | RR-UNB-INV-019-r1 | AI proposes buying a stock not in M. | Rejected NoApprovedModel. |
| XDS-INV-004 | Tender offer deadline on gifted shares | UNB-XIN-INV-003 | No decision by T-1. | Default submitted; donor input stored as recommendation. |
| XDS-INV-005 | Class action recovery to closed fund | UNB-XIN-INV-004 | $9k recovery. | A $6k, B share to OD-11 destination. |
| XDS-INV-006 | Lagged PE NAV roll-forward | UNB-XIN-INV-005 | Dec close. | Value 1,050,000 lag 92 days; provisional if >120. |
| XDS-INV-007 | Sweep income attribution | UNB-XIN-INV-006 | Month-end allocation. | Unattributed 0; FDIC exposure reported. |
| XDS-INV-008 | Crypto held over year end | UNB-XIN-INV-007 | 2 BTC received Dec 31 23:00; broker outage. | Measured at cutoff fair value; separate line; exposure exception. |
| XDS-INV-009 | Unlabeled AUM on partner invoice | UNB-XIN-INV-008 | Template field 'assets' lacks AssetMeasure. | Publication rejected. |
| XDS-SVC-001 | Status call does not create a case | UNB-XIN-SVC-001;RR-ADM-042-r1 | D calls asking status | Contact Resolved; no Case; contact counted in contacts-per-1,000 |
| XDS-SVC-002 | Implicit complaint recognized | UNB-XIN-SVC-005;UNB-XIN-SVC-001;RR-REQ-X-048-r2 | Third email: 'this is unacceptable' | Contact classified complaint; Case kind complaint; WorkItem cause e; acknowledgement due in 2 business days |
| XDS-SVC-003 | Regulator complaint cannot be closed by service | UNB-XIN-SVC-005;UNB-XIN-SVC-002 | Service agent issues Close | Denied; routed to counsel queue |
| XDS-SVC-004 | Kernel isolates clocks and access | UNB-XIN-SVC-002;UNB-XIN-CLM-004 | Service agent opens D timeline; SLA job runs | C1 visible with own clock; C2 invisible; C2 has no external SLA breach |
| XDS-SVC-005 | Legal hold binds closed cases | UNB-XIN-SVC-002;UNB-XIN-CLM-007 | Disposition run | Case retained; disposition skipped with hold reason |
| XDS-SVC-006 | Recording consent declined | UNB-XIN-SVC-003 | Caller presses 2 to decline | No audio or transcript stored; Contact flagged not-recorded |
| XDS-SVC-007 | Recorder must not start before disclosure | UNB-XIN-SVC-003 | Call connects | Adapter conformance check fails; recording discarded and incident opened |
| XDS-SVC-008 | KBA-only caller requests balance | UNB-XIN-SVC-004;UNB-XIN-CLM-008 | Agent clicks show balance | Denied; OTP to registered phone offered |
| XDS-SVC-009 | Staff override of assurance matrix | UNB-XIN-SVC-004 | Supervisor forces disclosure | Command does not exist / denied |
| XDS-SVC-010 | Broken callback commitment | UNB-XIN-SVC-006 | Time passes | Backup alerted at 13:00; at breach customer notified with new time; Commitment Breached |
| XDS-SVC-011 | Unsupported payment promise blocked | UNB-XIN-SVC-006 | Agent records commitment 'paid Monday' | Rejected with reason |
| XDS-SVC-012 | Superseded clause recall | UNB-XIN-SVC-007;RR-CTL-040-r1 | Owner marks v3 superseded-in-error | Report lists 40 contacts; QA rubric updated |
| XDS-SVC-013 | December 31 surge | UNB-XIN-SVC-008;RR-ADM-033-r1 | Arrivals exceed capacity | Playbook: in-queue callback offered, year-end gift-date notice sent; callbacks scheduled with clock |
| XDS-SVC-014 | Summary cannot submit grant | UNB-XIN-SVC-009 | Staff attempts to create grant from summary | Only a draft recommendation requiring donor authenticated affirmation can be created |
| XDS-SVC-015 | Customer-initiated WorkItem closure | RR-REQ-X-048-r2 | Owner closes without message | Rejected |
| XDS-CLM-001 | Original reviewer blocked from reconsideration | UNB-XIN-CLM-001;UNB-XIN-CLM-002 | Donor requests reconsideration; R picks it up | Assignment denied; routed to independent reviewer; original decision record unchanged |
| XDS-CLM-002 | Late reconsideration without new evidence | UNB-XIN-CLM-001 | Request on day 120 | Rejected with notice of window |
| XDS-CLM-003 | Charity vetting reversal does not auto-approve grant | UNB-XIN-CLM-001 | Vetting reconsideration reverses C | C eligible; G remains Declined; donor may resubmit or sponsor re-reviews |
| XDS-CLM-004 | Restricted reason never disclosed | UNB-XIN-CLM-002;UNB-XIN-CLM-004 | Decline/hold notice generated; donor calls | Notice uses UNDER-REVIEW; agent script neutral |
| XDS-CLM-005 | RFI duplicate-ask prevention and expiry | UNB-XIN-CLM-003;RR-ADM-033-r1 | Staff RFI includes letter and appraisal; appraisal not returned by due date | Letter removed from RFI; Decision clock paused while RFI open; case closed incomplete with reopen right |
| XDS-CLM-006 | Investigator cannot decide | UNB-XIN-CLM-004 | I issues Decline on G | Denied; routed to other approver; hold remains under payment domain |
| XDS-CLM-007 | Leakage sample finds STP error | UNB-XIN-CLM-005;RR-CTL-040-r1 | Sample review finds one grant to disqualified supporting org | Remediation case; learning signal; error rate with CI reported |
| XDS-CLM-008 | Representative protective suspension expiry | UNB-XIN-CLM-006 | Max duration elapses without renewal | Suspension lifts; reviewer notified; P not told of reason |
| XDS-CLM-009 | Privileged note excluded from audit export | UNB-XIN-CLM-007 | Auditor evidence pack generated | Note excluded; privilege log entry included |
| XDS-CLM-010 | Vendor offboarding revocation | UNB-XIN-CLM-008 | Sponsor terminates V | All sessions revoked within 1 hour; queued items reassigned |
| XDS-CLM-011 | Handoff metric | RR-ADM-042-r1 | Closed | handoffs=2 reported |
| XDS-ERP-001 | Mid-quarter opening with minimum fee | UNB-XIN-ERP-001;RR-ADM-028-r1 | Quarter-end fee run. | Segment days 46-92; computed bps fee $8,000x0.006x47/365=$6.18 < prorated minimum $12.88; assessed $12.88; Posted. |
| XDS-ERP-002 | Minimum exceeds available balance | UNB-XIN-ERP-001 | Fee run. | Assessed $10; waived $15 MinimumShortfall; no receivable. |
| XDS-ERP-003 | Migration cutover after incumbent billed full quarter | UNB-XIN-ERP-001 | First fee run. | Cutover segment excluded; zero fee for overlapped days. |
| XDS-ERP-004 | Donor membership card payment | UNB-XIN-ERP-002 | Card charge succeeds. | Service-fee receipt; no contribution, no acknowledgment, no fund entry. |
| XDS-ERP-005 | Operator invoice and pass-through | UNB-XIN-ERP-003 | Pass-through schedule activated without clause ref, then with clause + disclosure. | First rejected CONTRACT_CLAUSE_REQUIRED; second active; operator invoice absent from tenant ledger. |
| XDS-ERP-006 | Vendor payee on grant path | UNB-XIN-ERP-004 | Grant to V; grant to C. | V rejected PAYEE_NOT_GRANT_RECIPIENT; C held in related-party review. |
| XDS-ERP-007 | Threshold splitting | UNB-XIN-ERP-005 | Three $45k approvals to one recipient within 10 days. | Third escalated. |
| XDS-ERP-008 | Departed approver | UNB-XIN-ERP-005 | Approval at 10:05. | Rejected AUTHORITY_INVALID. |
| XDS-ERP-009 | Cost-to-serve metrics cannot drive routing | UNB-XIN-ERP-006 | Publish. | Rejected PROHIBITED_INPUT. |
| XDS-ERP-010 | CRM webhook claims fee waived | UNB-XIN-ERP-007 | Ingest. | FeeWaiverRequest case; no FeeWaived event. |
| XDS-ERP-011 | Vendor SOC report aging | RR-REQ-X-073-r1 | Daily evaluation. | BridgeRequired; Assured blocked. |
| XDS-ERP-012 | Household does not aggregate tiers | RR-ADM-008-r1 | Fee run. | Each fund tiered separately (0.60% each). |
| XDS-CRM-001 | Donor who is a charity officer | UNB-XIN-CRM-001;UNB-XIN-CRM-002 | C's staff open P's contact profile; P recommends grant from F to C. | Staff see only charity-contact profile; grant held in TVCTL-14 review. |
| XDS-CRM-002 | Organization-advised fund recommending to itself | UNB-XIN-CRM-002 | Activation attempt. | Blocked pending classification. |
| XDS-CRM-003 | Opt-out in white-label program | UNB-XIN-CRM-003 | Campaign 8 business days later; grant-paid notice same day. | Campaign suppresses donor; notice delivered. |
| XDS-CRM-004 | Promotional SMS without written consent | UNB-XIN-CRM-003 | Send. | Blocked CONSENT_LEVEL_INSUFFICIENT. |
| XDS-CRM-005 | Sensitive segment | UNB-XIN-CRM-004 | Publish. | Rejected SENSITIVE_ATTRIBUTE. |
| XDS-CRM-006 | Backup owner coverage | UNB-XIN-CRM-005 | Work item. | Routed to backup. |
| XDS-CRM-007 | Referral without counsel rule | UNB-XIN-CRM-006 | Month-end. | No accrual/export. |
| XDS-CRM-008 | Advisor changes firm | UNB-XIN-CRM-007 | X reports departure; A at Y requests access. | Old grants ended; accrual stopped; new access needs donor consent. |
| XDS-TRS-001 | Named charity merged | UNB-XIN-TRS-001;UNB-XIN-TRS-005 | Response window expires. | Successor org proposed with evidence for sponsor approval. |
| XDS-TRS-002 | Legacy letter vs successor grant | UNB-XIN-TRS-002 | Successor recommends health grant. | No hold; letter shown labelled non-binding. |
| XDS-TRS-003 | Trustee change vs entity dissolution | UNB-XIN-TRS-003 | Trustee replaced; later LLC holder dissolved. | Signatory update without succession; dissolution opens succession case. |
| XDS-TRS-004 | POA agent recommendation | UNB-XIN-TRS-004 | Agent recommends grant; later donor recovers. | Rejected REPRESENTATIVE_SCOPE; restoration ends agent grant. |
| XDS-TRS-005 | Standing vs new instruction | RR-ADM-023-r1 | One-off $2.5k to Y. | Both held; author asked to confirm. |
| XDS-TRS-006 | Co-advisor deadlock | RR-ADM-023-r1 | One approves, one rejects; window passes. | Sponsor case opened. |
| XDS-TRS-007 | Family office feed consent | UNB-XIN-TRS-006 | Feed; then revoke. | Two funds; then one. |
| XDS-NPG-001 | ED expires mid-schedule | UNB-XIN-NPG-001;RR-UNB-GRANT-041-r1 | Occurrences on 2027-12-15 and 2028-01-15. | 2027-12-15 released; 2028-01-15 Held ED_EXPIRED; renewal work item. |
| XDS-NPG-002 | International recommendation converted to intermediary | UNB-XIN-NPG-002;RR-UNB-GRANT-041-r1 | Donor recommends F $5,000. | Donor offered I1; accepts; grant to I1 class (a). |
| XDS-NPG-003 | Fiscal-sponsored for-profit project | UNB-XIN-NPG-003 | Grant to P. | CounselReview hold; payee would be FS. |
| XDS-NPG-004 | Schedule I reconciliation | UNB-XIN-NPG-004 | Generate package. | Lines aggregated per canonical recipient; difference 0. |
| XDS-NPG-005 | Merger during recurring schedule | UNB-XIN-NPG-005 | Merger event ingested. | Schedule Suspended; open grant Held; substitution work item. |
| XDS-NPG-006 | Discretionary hold auto-expiry | UNB-XIN-NPG-006 | Date passes without renewal. | Hold released; affected grants re-evaluated. |
| XDS-NPG-007 | Trump account contribution after expiry | UNB-XIN-NPG-007 | Grant dated 2029-10-01. | CounselReview RULE_EXPIRED. |
| XDS-NPG-008 | Excess benefit discovered post-payment | UNB-XIN-NPG-008 | Audit flag. | ExciseTaxIncident Suspected; payee blocked for fund; counsel task. |
| XDS-NPG-009 | Foreign grantee with official on board | UNB-XIN-NPG-009;RR-UNB-GRANT-041-r1 | Approval. | CounselReview. |
| XDS-NPG-010 | Humanitarian grant under general license | UNB-XIN-NPG-010;RR-UNB-GRANT-041-r1 | Release without authorization; then with. | Held then Released. |
| XDS-NPG-011 | Stale check jurisdiction | UNB-XIN-NPG-011 | Reaches AgedOutreach. | Jurisdiction TX; due-diligence letter scheduled per table. |
| XDS-NPG-012 | Agreement tiers | RR-UNB-GRANT-024-r1 | Approval/release. | Tier0 no agreement; Tier1 award letter; Tier2 held until signed. |
| XDS-NPG-013 | No acknowledgment needed | RR-UNB-GRANT-026-r1 | No ack for 60 days. | Completed; no work item. |
| XDS-NPG-014 | Foreign route precedence | RR-UNB-GRANT-041-r1;UNB-XIN-NPG-001;UNB-XIN-NPG-002 | Recommendation. | Route chosen per sponsor precedence; recorded. |
| XDS-REG-001 | Operator-titled account blocks activation | UNB-XIN-REG-001 | Activation. | Blocked MT_ANALYSIS_REQUIRED. |
| XDS-REG-002 | Feature activation requires status determination | UNB-XIN-REG-002 | Enable. | Blocked STATUS_DETERMINATION_REQUIRED. |
| XDS-REG-003 | SMS revocation honoured | UNB-XIN-REG-003 | Donor replies 'stop texting me' (non-keyword) recorded by staff. | Subsequent SMS suppressed. |
| XDS-GOV-001 | Rule fixture failure blocks publication | UNB-XIN-GOV-001;UNB-XIN-NPG-001 | Publish. | Fixture from Rev. Proc. 2017-53 §7 fails; publication rejected. |
| XDS-SAS-001 | Entitlement removed mid-flight | UNB-XIN-SAS-001 | Contract amendment removes ComplexAssets. | New complex-asset intents rejected ENTITLEMENT_REQUIRED; 4 in-flight contributions continue to Funded. |
| XDS-SAS-002 | Flag used as tenant fork | UNB-XIN-SAS-002 | Registry validation in CI. | Build fails FLAG_SEMANTIC_VARIATION. |
| XDS-SAS-003 | Canary migration breaks ledger in ring 1 | UNB-XIN-SAS-003;UNB-XIN-WFA-002 | Canary cell shows trial balance mismatch after deploy. | Promotion halts; cells 2-3 remain N-1; canary rolled forward with fix; in-flight instances keep v4 until mapped. |
| XDS-SAS-004 | Webhook after partner termination | UNB-XIN-SAS-004 | Grant G status changes 12:05. | No delivery to P; suppressed-delivery record. |
| XDS-SAS-005 | Cross-tenant causation attempt via support tool | UNB-XIN-SAS-005 | Command submitted. | Rejected CROSS_TENANT_CAUSATION. |
| XDS-SAS-006 | Third-party app over-scope | UNB-XIN-SAS-006 | Registration review and sponsor enablement. | payee:write rejected; sponsor enables funds:read for 2 programs only. |
| XDS-SAS-007 | Wealth-firm program leaves sponsor A for sponsor B | UNB-XIN-SAS-007;UNB-XIN-OTH-003;UNB-XIN-IAM-001;RR-UNB-TEN-016-r1 | P gives 90-day exit notice. | PartnerExit case; donors pre-enrol sponsor-direct factor; 700 transfer grants approved by A's batch policy and paid from A's account to B; 300 remain with A; hold funds transfer o… |
| XDS-SAS-008 | Error budget exhausted in December | UNB-XIN-SAS-008;UNB-XIN-SAS-003 | Feature release requested. | Blocked by both budget and peak freeze; security patch allowed. |
| XDS-SAS-009 | Metering reconciliation off by 12% | UNB-XIN-SAS-009;UNB-XIN-SAS-005 | Allocation runs. | Unattributed usage reported; reconciliation exception opened; job fixed. |
| XDS-SAS-010 | Sandbox attempts live payment | UNB-XIN-SAS-010 | Payment dispatch. | Configuration validation rejects; egress blocked; TEST-labelled mock outcome only. |
| XDS-SAS-011 | Emergency subprocessor replacement | UNB-XIN-SAS-011 | Replacement activated. | Notice within 72h to affected tenants with reason. |
| XDS-SAS-012 | Restore with inconsistent store points | RR-UNB-RES-003-r1;UNB-XIN-WFA-001 | Restore completes. | Restore rejected until common consistency point established; payment release blocked; due obligations fire once after reconciliation. |
| XDS-SAS-013 | Suspension during fee dispute | RR-UNB-TEN-011-r1;UNB-XIN-SAS-001 | Platform suspends partner P. | Submitted recommendations continue under sponsor review; new partner commands blocked; statements issued; entitlements unchanged for fiduciary capabilities. |
| XDS-SAS-014 | Sponsor exit with hold and backups | RR-UNB-TEN-016-r1;UNB-XIN-REC-001 | Retention expires; destruction requested. | Destruction blocked for held scope; certificate for rest lists backup expiry dates. |
| XDS-SAS-015 | Partner API sunset with open objects | RR-UNB-API-STD-003-r1 | Sunset executes. | v1 closed for new requests; 3 queryable via v2 and complete. |
| XDS-IAM-001 | Dual-role employee | UNB-XIN-IAM-001 | Logs in as donor; calls staff endpoint. | REALM_MISMATCH. |
| XDS-IAM-002 | Operator SRE edits grant without sponsor delegation | UNB-XIN-IAM-002 | Operator requests JIT to correct grant. | Pending sponsor approval; when approved, change goes through domain command; access visible to sponsor in 15 min. |
| XDS-IAM-003 | Partner-confirmed export | UNB-XIN-IAM-003 | Export requested. | STEP_UP_REQUIRED; sponsor verifier challenge. |
| XDS-IAM-004 | Uncertified bank portal user | UNB-XIN-IAM-004 | Due+14 days. | User suspended in registry and bank notified. |
| XDS-IAM-005 | Break-glass during IdP compromise | UNB-XIN-IAM-005 | Two responders activate sealed account; try payment freeze and authority change. | Freeze succeeds; authority change denied; tenants notified within 1h; auto-expire 4h. |
| XDS-REC-001 | Hold during print-vendor offboarding | UNB-XIN-REC-001;UNB-XIN-OTH-004 | V offboarded. | Preservation notice acknowledged; V deletion of C data blocked. |
| XDS-REC-002 | AG subpoena production | UNB-XIN-REC-002 | Production generated. | Manifest with hashes, scope query, redaction log; no other tenant data. |
| XDS-REC-003 | Hold lands after batch approval | RR-REQ-X-038-r1 | Hold on 3 records before execution. | 3 skipped, 9,997 disposed, verification recorded. |
| XDS-WFA-001 | Scheduler crash at deadline | UNB-XIN-WFA-001 | Scheduler crash at T, restart T+10m. | Obligation fires once; WorkItem if overdue tolerance exceeded. |
| XDS-WFA-002 | State removed with live instances | UNB-XIN-WFA-002 | Release removes state. | CI failure until mapping declared; then migration events per instance. |
| XDS-OTH-001 | Program description drift | UNB-XIN-OTH-001 | Next business day. | ProgramDescription new version equals config. |
| XDS-OTH-002 | Migration freeze in January | UNB-XIN-OTH-002 | Donor submits recommendation on 2 Feb; cutover overruns. | TRANSITION_WINDOW response; overrun notice within 1 business day. |
| XDS-OTH-003 | Partner settlement account inserted | UNB-XIN-OTH-003 | Payment release. | Blocked FUNDS_FLOW_INTERMEDIARY. |
| XDS-OTH-004 | Spoiled check reprinted | UNB-XIN-OTH-004 | Reconciliation. | Spoiled number voided in positive pay before release of reprint; IMb Delivered does not change Outstanding. |
| XDS-ECO-001 | Attribution run cannot post | UNB-XIN-ECO-001;RR-UNB-ACC-014-r1 | Attribution run plus attempted journal call from cost API | Run completes; journal call rejected COST_PROJECTION_NON_POSTING |
| XDS-ECO-002 | Rule version effective dating and restatement | UNB-XIN-ECO-002 | Re-run November after v3 publish | November rows cite v2; optional restatement labelled |
| XDS-ECO-003 | Grade downgrade on stale time sample | UNB-XIN-ECO-003;UNB-XIN-ECO-006;RR-UNB-ECO-001-r1 | Attribution run | Rows grade C; data-quality alert |
| XDS-ECO-004 | Reconciliation residual exceeds tolerance | UNB-XIN-ECO-004;UNB-XIN-ECO-022;RR-UNB-ACC-014-r1 | Monthly ReconciliationRun and pack generation | Residual $8,000 UnmappedGLAccount; ExceedsTolerance (1.6% > configured 1%); pack Draft until controller sign-off |
| XDS-ECO-005 | Late invoice after close | UNB-XIN-ECO-004;UNB-XIN-ECO-005 | Payment processor invoice +$120 posted after close | Run v2 with LateOrUnmeteredCharge residual; v1 retained; report shows restatement |
| XDS-ECO-006 | Fund-borne custody fee excluded from sponsor cost-to-serve | UNB-XIN-ECO-005;UNB-XIN-ECO-012 | Cost-to-serve computed | Excluded from sponsor CM; shown in donor all-in cost view |
| XDS-ECO-007 | TDABC worked example golden test | UNB-XIN-ECO-006 | Labour attribution | Rate $1.953125/min; attributed $135,546.88; unused $44,453.12 |
| XDS-ECO-008 | Waiting is not touch time | UNB-XIN-ECO-006;UNB-XIN-ECO-010 | Labour attribution and mining | Labour = equation minutes; wait share attributed to provider dependency |
| XDS-ECO-009 | Cost per account shown with multiple denominators | UNB-XIN-ECO-007;UNB-XIN-ECO-022 | Open cost per account widget | Shows per account-month by band + per settled grant + grade + population |
| XDS-ECO-010 | International grant cost quote and overrun | UNB-XIN-ECO-008;UNB-XIN-ECO-018 | Donor affirms; final ED cost $700 | Quote id in affirmation; charge held pending re-affirmation or waiver |
| XDS-ECO-011 | Cost quote never skips control | UNB-XIN-ECO-008;RR-AIN-028-r1 | Run requested | Quote logged; run proceeds; control executes |
| XDS-ECO-012 | Rail not switched for cost | UNB-XIN-ECO-008 | Grant payment | Check issued per verified method; quote shows ACH enrollment opportunity as LearningSignal |
| XDS-ECO-013 | AI budget exhaustion fail-safe | UNB-XIN-ECO-009;RR-AIN-028-r1 | Cost reaches $1.00 mid-evidence summary | Incomplete-BudgetExhausted; human task; no Pass |
| XDS-ECO-014 | AI cost anomaly | UNB-XIN-ECO-009;UNB-XIN-ECO-014 | Run costs $0.35 | Anomaly alert to agent owner; no cancellation |
| XDS-ECO-015 | Mining treats second approval as control cost | UNB-XIN-ECO-010;UNB-XIN-ECO-011 | Weekly mining + AI recommendation proposing removal | Reported as control cost; AI proposal routed Tier C |
| XDS-ECO-016 | AI cannot change configuration | UNB-XIN-ECO-011 | Calls routing-rule update | Denied and logged |
| XDS-ECO-017 | Profitability isolated from routing | UNB-XIN-ECO-012;UNB-XIN-ECO-020 | Queue routing service requests margin / donor LTV | Denied |
| XDS-ECO-018 | Minimum fee scenario with illustrative input | UNB-XIN-ECO-013;RR-UNB-ENT-003-r1 | ScenarioRun minimum $100->$250 | Verdict plus statement that it rests on illustrative input; no apply action |
| XDS-ECO-019 | Threshold breach alerts only | UNB-XIN-ECO-014 | Month at $1.72 grade A | Work item for owner; no processing throttled |
| XDS-ECO-020 | Ratchet with verified saving | UNB-XIN-ECO-015;UNB-XIN-ECO-016 | Verified actual $36 | Target $37.80; verifier not owner |
| XDS-ECO-021 | Counter-metric breach halts initiative | UNB-XIN-ECO-016;UNB-XIN-ECO-015 | Seeded-control pass rate 100%->96% | Halt, ratchet blocked, compliance notified, rollback review |
| XDS-ECO-022 | Detection loss masquerading as saving | UNB-XIN-ECO-016;UNB-XIN-ECO-019 | Saving verification | Not credited; labelled detection-loss suspected |
| XDS-ECO-023 | December capacity forecast reproduces backlog arithmetic | UNB-XIN-ECO-017;UNB-XIN-ECO-020 | Forecast and staff-for-SLA | Backlog ~9,600; clearance ~223 days at average staffing; staff-for-10-day-SLA ~54 FTE shown with grade |
| XDS-ECO-024 | Service charge recovery ceiling | UNB-XIN-ECO-018 | Activation request | Blocked pending counsel review flag |
| XDS-ECO-025 | Automation ROI includes rework and failure cost | UNB-XIN-ECO-019 | Post-change labour down, rework +$15k | Negative or unverified verdict when low bound < 0 |
| XDS-ECO-026 | Cross-sponsor and staff privacy in cost views | UNB-XIN-ECO-021 | Benchmark and team cost requested | Benchmark suppressed; team merged into parent |
| XDS-ECO-027 | Drill-down and accessible export | UNB-XIN-ECO-022;UNB-XIN-ECO-007 | Drill-down from KPI; export CSV | Rows and events within scope; table alternative; export with grade, range, definition, disclaimer |
| XDS-ECO-028 | Driver measurement replaces estimate without overwrite | RR-UNB-ECO-001-r1;RR-UNB-ENT-003-r1 | Measured 52 min (n=60) arrives | New parameter version source Observation; estimate retained |
| XDS-EVA-001 | Misposted contribution discovered after statements issued and period closed | UNB-XIN-EVA-001;UNB-XIN-EVA-002;UNB-XIN-EVA-003;RR-UNB-DATA-005-r1;TVS-359;INV-08 | Donor of F2 reports missing contribution on 2026-11-03 | CorrectionCase opened; ContributionMisapplicationCorrected posts in November (open period) as PriorPeriodAdjustment relates_to_period=2026-09: reversal of F1 credit, re-post to F2… |
| XDS-EVA-002 | Contributed security valued on the wrong date | RR-UNB-DATA-005-r1;UNB-XIN-EVA-002;UNB-RCN-FIN-011;UNB-RCN-FIN-012 | Tax reviewer finds the date error on 2026-11-18 | ValuationCorrection fact recorded; contribution revenue adjusted -$2,200 via PriorPeriodAdjustment; impact registry produces: acknowledgment amendment (human, counsel template), u… |
| XDS-EVA-003 | Charity revoked between sponsor approval and payment release | RR-UNB-EVT-001-r1;UNB-XIN-EVA-011;ADV-069;CONC-02;RR-REQ-X-020-r1 | IRS revocation ingested; EligibilityHoldPlaced commits at version 42; release worker holds snapshot from before 42 | Release transaction reads registry version 42 -> Held(ELIGIBILITY_HOLD); reservation stays Active; grant Held; donor sees 'on hold: recipient eligibility review' |
| XDS-EVA-004 | ACH return R03 three business days after settlement | UNB-XIN-EVA-005;UNB-XIN-EVA-015;TVS-016;TX-15;PAY-03 | Return file on 2026-11-19 contains R03 for P's trace number; same file re-delivered on 2026-11-20 | One PaymentReturnReported; compensating journal Dr Cash Cr Grants Payable; grant state part-paid->Returned per policy; queued 'paid' notice suppressed if unsent else 'payment retu… |
| XDS-EVA-005 | Check uncashed for 180 days | UNB-XIN-EVA-007;FI-21;TVS-087;CTL-044 | Application restarted twice during the period; day 180 reached on 2026-10-28 | Single StaleCheckReview item; on bank-confirmed void: CheckVoidedStaleDated posts reversal of liability; reissue creates new linked PaymentIntent after recipient re-verification,… |
| XDS-EVA-006 | In-kind liquidation sold in three tranches | UNB-XIN-EVA-007;UNB-XIN-EVA-005;SIM-12;TX-09 | Tranche 1 settles 4,000 sh; tranche 2 partially fills 2,500 of 3,000; custodian later corrects tranche 1 price under same trade ref | Each settled tranche posts its own journal (proceeds, realized gain/loss by lot); capacity increases only by settled cash; price correction with same ref and different payload ->… |
| XDS-EVA-007 | Wire partially returned net of intermediary fees | UNB-XIN-EVA-005;UNB-XIN-EVA-001;PAY-06;REQ-X-083 | Return of $19,955 arrives (intermediary deducted $45, synthetic) | PaymentPartiallyReturned: Dr Cash 19,955, Dr returned-wire fee account 45 (bearer per XID-EVA-003), Cr Grants Payable 20,000; settlement dimension PartiallyReturned; case opened |
| XDS-EVA-008 | Multi-investment grant where one liquidation leg fails | UNB-XIN-EVA-007;UNB-XIN-EVA-006;EXC-04;UNB-RCN-FIN-008;GEX-T03 | Legs A and C settle; leg B rejected by fund manager | FundingPM -> FundingShortfall; grant Held; proceeds of A and C remain in fund cash (not reversed); EXC-04 case proposes alternate source plan requiring approval; all records share… |
| XDS-EVA-009 | OFAC listing of a recipient mid-way through a recurring-grant run | UNB-XIN-EVA-011;UNB-XIN-EVA-010;TVCTL-13;CTL-012 | Sanctions list update adds C at 10:14 after 2,100 items released; 9 items to C already in an untransmitted ACH file; 2 items to C transmitted at 09:50 | Remaining 26 items Held; 9 items removed from file before transmission; 2 transmitted items -> SanctionsBlock/recall case per counsel; run PartiallyCompleted; fund-level effects r… |
| XDS-EVA-010 | Legal dispute freeze on a fund | UNB-XIN-EVA-010;UNB-XIN-EVA-012;ADM-023;TVS-370 | Dividend $1,200 arrives; advisor A recommends a $5,000 grant; retention engine schedules disposition of 2017 records | Dividend posts; grant recommendation recorded but approval blocked HELD_FREEZE; 2017 disposition blocked by RecordsLegalHold; statement issues |
| XDS-EVA-011 | Sponsor transfers 10,000 funds to another sponsor on the platform | UNB-XIN-EVA-009;UNB-XIN-EVA-007;RR-UNB-WF-014-r1;UNB-TEN-012 | TransferProgram runs; process crashes at fund 4,000 | Per fund: DAF_TRANSFER_OUT in DB_A and inbound contribution in DB_B; 37 remain UNKNOWN_PAYMENT_OPEN; reserved grants paid or cancelled per program rule before transfer; resume ski… |
| XDS-EVA-012 | Sponsor leaves the platform with full history; event schema drift on re-import | UNB-XIN-EVA-008;UNB-XIN-EVA-009;RR-REQ-X-061-r1;UNB-DATA-010 | Re-import runs | Known versions upcast; an authoritative fact type with unknown version 4b (fork) halts that stream and quarantines; imported history is reference evidence; opening balances posted… |
| XDS-EVA-013 | Two co-advisors submit opposite instructions | UNB-XIN-EVA-012 | Advisor A sets allocation to Conservative at 10:00; advisor B sets Growth at 10:03 | Both HeldConflict; case to sponsor; neither applied |
| XDS-EVA-014 | Standing instruction versus one-off skip | UNB-XIN-EVA-012;INV-13;TX-17 | Authorized advisor submits one-off 'skip November' and later a one-off $250 to C in November | November occurrence skipped with reason; $250 grant evaluated independently; December resumes |
| XDS-EVA-015 | Successor notice arrives while a grant is in flight | UNB-XIN-EVA-012;UNB-XIN-EVA-006;RR-UNB-SUCC-008-r1;TX-25 | Death notice received; epoch -> 8; protective hold on donor-derived rights | R1 approval rejected AUTHORITY_EPOCH_ADVANCED and held; G2 follows sponsor in-flight policy (continue, because sponsor already approved); child correlation roots link both to the… |
| XDS-EVA-016 | AI observation attempts to become a financial fact | RR-UNB-EVT-005-r1;UNB-XIN-EVA-016;SIM-22 | Agent calls its tools | Only an Observation/Proposal recorded; INSERT into eligibility facts denied by DB role; security incident; reviewer sees proposal |
| XDS-EVA-017 | Custodian position file missing for two days | UNB-XIN-EVA-004 | Files for 2026-11-19 and 2026-11-20 not delivered; a large grant request arrives depending on C capacity | SourceFeedGapDetected; capacity increases from C held; grant may reserve only against last complete positions per policy |
| XDS-EVA-018 | Statement and Schedule D package generated during heavy posting | UNB-XIN-EVA-003 | Statements regenerated while 500 postings commit | Identical figures; artifacts cite S |
| XDS-EVA-019 | Posting races hard close | UNB-XIN-EVA-002 | Posting and HardClosePeriod(2026-10) concurrently | Either posting included in snapshot or rejected PERIOD_CLOSED and re-routed |
| XDS-EVA-020 | Release handler subscribed to an informational provider acknowledgment | RR-UNB-EVT-001-r1;UNB-XIN-EVA-008 | CI runs | Build fails NON_AUTHORITATIVE_GATE; published contract exposes only settled Paid |
| XDS-EVA-021 | Erasure request for a former co-advisor | RR-UNB-DATA-004-r1;UNB-XIN-EVA-014 | Privacy erasure request | Engagement data redacted with history; authority facts and grant records retained by class; PII vault reference handling per UNB-DATA-007 |
| XDS-EVA-022 | Proposal to add a message broker for partner webhooks | UNB-XIN-EVA-013 | Architecture proposal submitted | Rejected unless benchmarks meet thresholds; per-subscriber cursors over outbox used |
| XDS-ADV-001 | Workplace HVLB program economics at launch | UNB-XIN-ADV-001;UNB-XIN-CRM-002;UNB-XIN-IAM-004;UNB-XIN-ERP-001;UNB-XIN-SAS-009;UNB-XIN-ECO-007;REQ-X-070 | Sponsor publishes the program configuration; one year passes. | Publication rejected until annual attestation is converted to on-change; IAM-004 recert applies only to accounts with delegated roles; statements electronic; ECO-007 shows expecte… |
| XDS-ADV-002 | Trust company in Shadow mode approving December grants with a late custodian file | UNB-XIN-ADV-002;UNB-XIN-ACC-010;UNB-XIN-EVA-001;UNB-XIN-EVA-004;UNB-XIN-BNK-003 | On 30 December the external snapshot for 29 December is missing; 400 recommendations arrive. | Recommendations above de minimis held SHADOW_SNAPSHOT_STALE; SourceFeedGapDetected case open; protective holds still applied; once the snapshot arrives and passes control totals,… |
| XDS-ADV-003 | Auditor finds a misposted prior-year contribution after fund statements were issued | UNB-XIN-ACC-005;UNB-XIN-EVA-002;UNB-XIN-EVA-003;UNB-XIN-ACC-004;RR-UNB-DATA-005-r1 | Auditor proposes an adjustment. | Adjustment posts to the AuditAdjustment period linked to the AuditFinding (not a current-period PPA, per XIC-ADV-005); fund statements corrected by notice per REQ-X-040; fund A sh… |
| XDS-ADV-004 | Spouses co-advise a fund and give conflicting grant instructions | RR-ADM-023-r1;UNB-XIN-EVA-012;UNB-XIN-TRS-004 | Both recommendations commit. | Under the resolved default (XID-ADV-004: Any-one below threshold) the second commit finds a scope conflict on capacity and both move to HeldConflict with a sponsor case; with AllR… |
| XDS-ADV-005 | Account takeover via contact-detail change then phone-assisted payee redirection | UNB-XIN-EVA-014;UNB-XIN-SVC-004;UNB-XIN-IAM-003;UNB-XIN-CRM-001;UNB-XIN-EVA-015;UNB-XIN-SVC-009 | Attacker changes the phone number via a low-assurance web flow, then calls support claiming to be the donor and requests a grant to a charity whose bank details were recently chan… | Phone-number change is an authenticator change requiring step-up on the existing factor and notice to the old number (XIC-ADV-003); support cannot exceed public disclosure without… |
| XDS-ADV-006 | Partner exit of 4,000 funds with an OFAC hit discovered mid-run | UNB-XIN-SAS-007;UNB-XIN-EVA-009;UNB-XIN-EVA-011;UNB-XIN-NPG-010;UNB-XIN-EVA-010 | During execution a successor advisor on fund #2,311 matches the SDN list. | Each transfer is an item decided under the policy version, hold-checked and committed individually (XIC-ADV-008); fund #2,311 receives a SanctionsBlock and is excluded; run ends P… |
| XDS-ADV-007 | Break-glass during a 31 December database outage | UNB-XIN-IAM-005;UNB-XIN-IAM-002;UNB-XIN-SAS-003;UNB-XIN-WFA-001;UNB-XIN-EVA-007 | Two operators activate break-glass to restore and apply a payment freeze. | Break-glass is a recognized basis for operator access (XIC-ADV-007); session visible in transparency view within 15 minutes and tenant notified within 1 hour; only protective acti… |
| XDS-ADV-008 | Onboarding a small community-foundation sponsor in 30 days | UNB-XIN-BNK-001;UNB-XIN-BNK-004;UNB-XIN-BNK-007;UNB-XIN-ACC-001;UNB-XIN-ACC-002;UNB-XIN-ACC-010;UNB-XIN-SVC-004;UNB-XIN-IAM-003;UNB-XIN-NPG-006;UNB-XIN-REG-001;UNB-XIN-REG-002;UNB… | Sponsor attempts to configure required registers and approvals. | With platform default packs adopted by attestation (XID-ADV-010), sponsor counsel reviews only deviations and launch completes; without them, at least 32 counsel/CPA approvals blo… |
| XDS-ADV-009 | Monthly economics pack with an unmapped GL account | UNB-XIN-ECO-004;UNB-XIN-ECO-001;UNB-XIN-ECO-003;UNB-XIN-ECO-012;UNB-XIN-ERP-003 | Attribution run executes. | Residual reported by reason UnmappedGLAccount; tolerance evaluated on the declared basis (total GL); pack Draft until controller sign-off; ratio text states unused capacity is 4.9… |
| XDS-ADV-010 | Card contribution disputed after the fund has granted it out | UNB-XIN-PAY-003;UNB-XIN-ERP-001;RR-TVR-CON-001-r1;UNB-XIN-EVA-005;UNB-RCN-FIN-013 | Processor reports DisputeOpened twice (duplicate file). | One ReturnedContributionExposure recorded (idempotent on business key); fund cannot go negative so the exposure is borne per sponsor policy; no refund to donor; daily fee accrual… |
| XDS-ADV-011 | Daily fee accrual volume at 1.4 million accounts | UNB-XIN-ERP-001;UNB-XIN-EVA-001;UNB-XIN-EVA-013;UNB-XIN-INV-006;UNB-XIN-ADV-001 | Fee and income processes run for 365 days. | With posted daily accruals, ~1.5bn journal rows per year are produced; with compute-daily/post-at-assessment (XID-ADV-007), ~4 posted accrual batches per fund per year; capacity a… |
<!-- END GENERATED: scenarios -->

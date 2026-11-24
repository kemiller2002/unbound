"""Convergence clauses: accounting, financial reporting, banking and payments (batches B and C).

Each clause is normative ("SHALL") wording that incorporates an approved or mission-adopted resolution into the
requirements baseline. Authoring source only; tools/reqaudit/adopt_convergence.py reads CLAUSES.
"""
from .common import clause, OWN

CLAUSES = (
    clause("CNV-FIN-001", "Capacity reservation and liquidity buffer",
           "Grant approval SHALL reserve the approved amount atomically in the same transaction that records the approval, "
           "moving through Reserved, FundingPlanned and Funded. Each investment pool SHALL carry a sponsor-approved liquidity "
           "buffer derived from a documented stress model (net outflow over the settlement horizon including the December peak). "
           "A fixed percentage MAY be used only while labelled as a placeholder and only until twelve months of the pool's own "
           "outflow data exist. A percentage grant SHALL be fixed in dollars at approval and SHALL change only through a new approval.",
           ["Given two approvals for the same fund committed concurrently whose sum exceeds available capacity, when both commit, then exactly one succeeds and the other is rejected with InsufficientAvailableCapacity.",
            "Given a pool whose buffer policy is labelled placeholder and twelve months of outflow history, when the buffer review runs, then a recalibration task is opened for the investment committee.",
            "Given a 10% percentage grant approved when capacity is $50,000, when the market falls before payment, then the payable remains $5,000."],
           ["RCD-017", "RR-UNB-GFUND-012-r1#Q1"], ["RCF-GRT-015", "RCF-FIN-022", "RCC-GRT-007", "RCC-FIN-008", "REQ-X-002"],
           "FIN", "initial-implementation", "Investment committee approval of the buffer policy within the investment policy statement.",
           OWN("Grants and capacity", "Unbound program ledger (capacity); custodian (positions)", "Sponsor investment committee", "Sponsor", "Sponsor treasury", "Sponsor controller"),
           ["EV-B"]),
    clause("CNV-FIN-002", "Versioned fee basis and accounting",
           "Each fee schedule version SHALL declare basis (end-of-day market value default; average daily, period-end or quarter-average "
           "options), day count, accrual and settlement cadence, minimum, payer source and treatment of reserved-but-uncommunicated "
           "grants. Fees SHALL be computed daily and posted per the schedule. Fees a sponsor charges its own funds SHALL be recorded as "
           "internal transfers (fund-level charge and operating-fund recovery) eliminated in entity statements; fees from external payers "
           "SHALL be recognized as revenue as the service period elapses. No closing-period minimum fee or proration SHALL be applied "
           "unless the schedule version states it.",
           ["Given a schedule on average daily balance, when a fund closes mid-quarter, then the fee equals the daily accruals to the closing date and no minimum is added unless the schedule declares one.",
            "Given fees charged to the sponsor's own DAFs, when entity statements are produced, then the fee transfer is eliminated and no external revenue is reported for it.",
            "Given a grant recognized as payable, when the fee base is computed, then the grant amount is excluded."],
           ["RCD-018", "XID-ERP-001", "UNB-XIN-ERP-001#Q1", "UNB-XIN-ERP-001#Q2", "XID-ADV-007"],
           ["RCF-FIN-012", "RCF-FIN-013", "UNB-XIN-ERP-001", "UNB-RCN-FIN-005"],
           "FIN", "initial-implementation", "Executed sponsor fee schedule; sponsor controller-approved fee accounting policy.",
           OWN("Fees", "Unbound program ledger; sponsor ERP for entity statements", "Sponsor CFO", "Sponsor", "Sponsor finance", "Sponsor controller"),
           ["EV-B"]),
    clause("CNV-FIN-003", "Four contribution dates and reversals",
           "Every contribution SHALL record GiftCompletionDate (tax rule by instrument), AcceptanceDate (accounting recognition), "
           "AvailabilityDate (sponsor risk rule, explicitly not finality) and AcknowledgmentDate. A returned ACH debit or card chargeback "
           "SHALL reverse the contribution as never completed and SHALL void or amend the acknowledgment. Availability holds SHALL be "
           "labelled as risk policy; return and claim windows (consumer ACH unauthorized returns 60 days and warranty claims up to two "
           "years; business ACH returns 2 banking days and claims one year; card chargebacks per network, up to 540 days) SHALL drive "
           "exposure limits per donor across all reversible un-matured amounts.",
           ["Given an ACH contribution settled 3 January for a debit initiated 30 December, when the acknowledgment is issued, then GiftCompletionDate follows the approved rule table and the initiation date is retained.",
            "Given an ACH contribution returned R10 on day 45, when the return posts, then the contribution and its acknowledgment are reversed and any shortfall against the fund is recorded as a sponsor loss.",
            "Given a donor whose un-matured reversible contributions exceed the per-donor exposure cap, when another reversible contribution arrives, then it is accepted but its availability is deferred."],
           ["RCD-019", "XID-BNK-006", "RR-TVR-CON-001-r1#Q1", "UNB-XIN-PAY-003#Q1"],
           ["RCF-FIN-001", "RCF-FIN-002", "RCF-FIN-004", "RCF-FIN-007", "RCF-FIN-020", "RCF-FIN-021", "RCF-REG-004"],
           "FIN", "initial-implementation", "Sponsor sign-off of the completed-gift rule table and acknowledgment template (the sponsor is the legal issuer).",
           OWN("Contributions", "Unbound program ledger; bank (settlement)", "Sponsor controller", "Sponsor", "Sponsor treasury", "Sponsor controller"),
           ["EV-B", "EV-C"]),
    clause("CNV-FIN-004", "Continuous reconciliation and risk-based attestation",
           "Reconciliation SHALL run continuously; exact matches SHALL auto-pass; a risk-stratified sample of auto-passed items SHALL be "
           "independently attested at a size set by the controller from an error-bound method (any numeric starting value labelled "
           "configurable and illustrative). Unexplained cash breaks SHALL block statements only for affected funds. Tolerances SHALL be "
           "absolute and relative per source, currency and asset class, calibrated from actual feeds; no silent write-offs. The controller "
           "SHALL sign off reconciliation completeness monthly.",
           ["Given an unexplained $12 break on fund F1, when monthly statements run, then F1's statement is withheld and all other statements publish.",
            "Given an error found in an attested sample, when the next sampling period starts, then the sample size for that stratum increases per the recorded policy.",
            "Given a tolerance configuration with no calibration evidence, when activation is requested, then activation is refused."],
           ["RCD-020", "XID-ECO-003", "UNB-XIN-ECO-004#Q1", "UNB-XIN-EVA-004#Q1", "UNB-XIN-ADV-002#Q2", "XID-CLM-004", "RR-REQ-X-007-r2#Q2", "RR-UNB-GRANT-030-r1#Q2", "UNB-XIN-CLM-005#Q1"],
           ["RCF-FIN-016", "RCF-FIN-018", "RCF-FIN-019", "RCF-FIN-010", "RCF-FIN-034"],
           "FIN", "initial-implementation", "",
           OWN("Reconciliation", "Unbound program ledger vs bank and custodian", "Sponsor controller", "Sponsor", "Independent reconciler (outside payment chain)", "Sponsor controller"),
           ["EV-B"]),
    clause("CNV-FIN-005", "Deterministic NAV publication under delegation",
           "In-tolerance NAVs SHALL publish by deterministic policy only under a recorded investment-committee delegation that approves "
           "tolerances and pricing sources. Out-of-tolerance, stale, missing or manually overridden prices SHALL route to a human approver "
           "before publication; a risk-based independent sample SHALL be reviewed the next business day; unresolved breaks SHALL block "
           "affected publication; the delegation SHALL be reviewed at least annually. Without a delegation, every NAV SHALL require a human checker.",
           ["Given no recorded delegation, when a NAV is computed, then it waits for a human checker.",
            "Given a delegation and a price 3% outside tolerance, when NAV runs, then publication waits for an approver and the exception is evidenced."],
           ["RCD-035"], ["UNB-NAV-008", "RCF-WF-001"],
           "FIN", "sponsor-activation", "Investment committee or board resolution delegating NAV publication.",
           OWN("Investment administration", "Unbound unit ledger; custodian prices", "Sponsor investment committee", "Sponsor", "Sponsor investment operations", "Sponsor controller"),
           ["EV-B"]),
    clause("CNV-FIN-006", "Grant priority over fees",
           "Recognized grants payable and, by default, reserved grants SHALL rank ahead of fees. A fee shortfall SHALL be a fund-level "
           "memo claim against future capacity, not an entity-level asset; it MAY be waived by an authorized approver with a reason and "
           "SHALL be written off on fund closure.",
           ["Given a fund whose capacity is fully reserved by approved grants, when the quarterly fee assesses, then the grants are unaffected and the fee shortfall is recorded as a memo claim."],
           ["RCD-036"], ["RCF-WF-001"],
           "FIN", "initial-implementation", "Sponsor program terms stating priority and waiver authority.",
           OWN("Fees", "Unbound program ledger", "Sponsor CFO", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-B"]),
    clause("CNV-FIN-007", "Grant recognition point as sponsor policy (corrects UNB-XIN-ACC-002)",
           "Each sponsor book SHALL declare an effective-dated GrantRecognitionPoint from {ApprovalCommunicatedToGrantee (platform default; "
           "notice sent atomically with approval), SponsorApproval (permitted only with a documented sponsor accounting policy that approval "
           "creates the obligation and the sponsor reserves no free rescission right), PaymentReleased (only where payment is the first "
           "communication)}. Approval, communication, condition and rescission evidence SHALL always be captured, and a period-end cutoff "
           "register SHALL list approved-but-uncommunicated, communicated-but-unpaid and conditional grants. This supersedes the "
           "prohibition of SponsorApproval in UNB-XIN-ACC-002 AC-D2, which rested on Codification text not verified and conflicts with "
           "audited sponsor practice.",
           ["Given a sponsor book declaring SponsorApproval with an attached policy document, when saved, then it is accepted and recorded with the policy reference.",
            "Given SponsorApproval without a policy document, when saved, then it is rejected with RecognitionPolicyEvidenceMissing.",
            "Given ApprovalCommunicatedToGrantee and an approval on 31 December, when the notice is sent in the same transaction, then expense and payable post in December."],
           ["XID-ADV-006", "XID-INV-001", "UNB-XIN-ACC-002#Q1"], ["UNB-XIN-ACC-002"],
           "FIN", "initial-implementation", "Sponsor's written grant-recognition policy accepted by its external auditor in the first audited year.",
           OWN("Accounting", "Unbound program ledger; sponsor GL", "Sponsor controller", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-B"]),
    clause("CNV-FIN-008", "Accounting treatment by transfer instrument (corrects UNB-XIN-ACC-001)",
           "Each account type SHALL carry one AccountingTreatment determined from its transfer-instrument terms: contribution without donor "
           "restrictions (variance power granted, no redirect or refund right, no restriction beyond advisory privileges); contribution with "
           "donor restrictions (binding time or purpose restriction); liability (resource provider names itself or an affiliate as "
           "beneficiary regardless of variance power, retains a redirect, revocation or refund right, or controls the recipient); equity "
           "transaction (financially interrelated entities). Classification SHALL NOT follow marketing labels.",
           ["Given a charity transferring assets naming itself beneficiary with a variance-power clause, when the account type is classified, then the treatment is Liability.",
            "Given an account type without an approved AccountingTreatment, when a receipt is attempted, then it is rejected."],
           ["XID-INV-002", "UNB-XIN-ACC-001#Q1", "XID-CRM-004", "UNB-XIN-CRM-002#Q1"], ["UNB-XIN-ACC-001", "RR-UNB-ACC-033-r1"],
           "FIN", "initial-implementation", "Sponsor review of each program's transfer-instrument wording.",
           OWN("Accounting", "Unbound program ledger", "Sponsor controller", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-B"]),
    clause("CNV-FIN-009", "Schedule D bridge basis",
           "Schedule D Part I SHALL be produced per tax year: line 2 contributions on the recognition basis; line 3 grants from DAFs on the "
           "grant-expense recognition basis plus transfers within the organization; line 4 year-end value on the books' basis net of "
           "DAF-attributed liabilities. Funds SHALL be classified DAF, similar fund or neither by the statutory definition and exceptions, "
           "not GAAP labels. The typed bridge of UNB-XIN-ACC-006 SHALL reconcile to the audited statements.",
           ["Given a fund excepted from DAF status by statute, when the Schedule D package is built, then it is excluded from column (a) and the bridge shows ExceptedFund."],
           ["XID-INV-005", "UNB-XIN-ACC-006#Q1", "RCD-024"], ["UNB-XIN-ACC-006"],
           "FIN", "initial-implementation", "Signed Form 990 by the sponsor officer.",
           OWN("Tax reporting", "Unbound program ledger", "Sponsor controller", "Sponsor", "Return preparer", "Sponsor controller"), ["EV-B"]),
    clause("CNV-FIN-010", "UBIT accrual default",
           "Estimated UBIT SHALL accrue by default in the period earned with quarterly estimates aligned to Form 990-T installments, true-up on "
           "filing and allocation to generating funds where program terms allow; filing-basis GL recognition MAY be configured only under a "
           "documented immateriality policy; the fund-level capacity hold SHALL apply in both cases.",
           ["Given filing-basis recognition configured, when UBTI is attributed to a fund, then a capacity hold equal to the estimate is still applied."],
           ["XID-INV-006", "UNB-XIN-ACC-008#Q1"], ["UNB-XIN-ACC-008"],
           "FIN", "initial-implementation", "Sponsor program terms permitting UBIT charge to funds.",
           OWN("Tax", "Unbound program ledger; sponsor GL", "Sponsor controller", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-B"]),
    clause("CNV-FIN-011", "Corrections, audit adjustments and materiality",
           "Effective date and posting period SHALL be stored separately; corrections SHALL post in the current open period with "
           "relates_to_period and correction class. A period 13 (AuditAdjustment) SHALL accept only AuditFinding-linked journals until "
           "statements are available to be issued (non-public NFPs) or issued (public entities). After issuance the CFO SHALL determine "
           "materiality with a recorded qualitative assessment; no numeric materiality default SHALL ship except as an illustrative, "
           "non-authoritative label. Functional expense allocation SHALL be a management policy approved annually by the CFO, reconciled "
           "to management activity-based costing from the same source facts.",
           ["Given a non-public sponsor, when the available-to-be-issued date is recorded, then period 13 hard-closes on that date.",
            "Given a post-issuance correction, when it is posted, then the record requires a qualitative materiality assessment before approval."],
           ["XID-EVA-002", "XID-ADV-005", "XID-ECO-005", "UNB-XIN-EVA-002#Q1", "RR-UNB-ACC-017-r1#Q1"], ["UNB-ACC-017", "ACC-ADR-011", "UNB-XIN-EVA-002", "UNB-XIN-ACC-005", "UNB-ACC-019"],
           "FIN", "initial-implementation", "",
           OWN("Accounting", "Unbound program ledger; sponsor GL", "Sponsor CFO", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-B"]),
    clause("CNV-PAY-001", "Payment roles: sponsor originates, Unbound is a TPSP",
           "Each program SHALL record a role table: Originator (the sponsor), ODFI, Third-Party Service Providers (Unbound), any Third-Party "
           "Sender or grant-rail partner and the authorization holder. Unbound SHALL NOT sign an origination agreement, hold funds or execute "
           "or block a payment on its own authority; any configuration that would do so SHALL be rejected. Unbound SHALL accept TPSP "
           "obligations by contract, including Nacha risk-based fraud monitoring (Phase 2 effective 22 June 2026), security and audit cooperation. "
           "Release 1 grant payments SHALL use networked rail partners acting as the sponsor's agent, with sponsor-originated ACH as fallback.",
           ["Given a program configuration naming Unbound as Originator, when it is saved, then it is rejected with ProhibitedRole.",
            "Given a role table without an executed ODFI agreement reference, when payment activation is requested, then activation is refused."],
           ["XID-BNK-005", "UNB-XIN-BNK-001#Q1", "SPD-CMP-003", "XID-REG-001", "UNB-XIN-REG-001#Q1", "UNB-XIN-OTH-003#Q1"],
           ["UNB-XIN-BNK-001", "RR-UNB-PAY-008-r1", "OB-43", "UNB-XIN-OTH-003", "REQ-X-005", "UNB-RCN-GRT-017"],
           "PAY", "initial-implementation", "Each sponsor's executed ODFI origination agreement naming or permitting Unbound as TPSP; executed rail-partner agreement.",
           OWN("Payments", "Bank / rail partner (payment status)", "Sponsor treasurer", "Sponsor", "Sponsor treasury", "Sponsor treasurer"), ["EV-C", "EV-D"]),
    clause("CNV-PAY-002", "Check lifecycle, positive pay, NOCs and destination proof",
           "Every check account SHALL use positive pay with daily issue files including voids. NOC corrections SHALL apply within six banking "
           "days or before the next entry, whichever is later, under change control. Destination evidence (account-ownership verification or "
           "charity-signed bank letter) SHALL exist before the first electronic payment and after any bank-detail change. Checks SHALL void "
           "by default at 90 days (configurable, never later than the shortest applicable state window) with a recorded reason. Voiding SHALL "
           "NOT extinguish the grant obligation and SHALL NOT re-credit the fund automatically.",
           ["Given a charity bank-detail change, when the next grant releases, then release waits for new destination evidence.",
            "Given a check voided at day 90, when the void posts, then the payable remains and a VoidDisposition is opened."],
           ["RCD-028", "UNB-XIN-BNK-007#Q1"], ["RCF-GRT-010", "RCF-GRT-012", "RCF-GRT-017", "RCF-GRT-018", "RCF-GRT-019", "RCF-GRT-035", "RCF-GRT-037", "RCC-GRT-006"],
           "PAY", "initial-implementation", "Sponsor ODFI and positive-pay service agreements; sponsor adoption of void and disposition policy.",
           OWN("Payments", "Bank", "Sponsor treasurer", "Sponsor", "Sponsor treasury", "Sponsor treasurer"), ["EV-C"]),
    clause("CNV-PAY-003", "Void disposition and unclaimed property",
           "A VoidDisposition record SHALL determine Re-credit permitted, Hold as payable or Report from: grant cancellation status and "
           "grant-letter lapse terms, the priority state, a statute-derived state rule row (dormancy period, no-consideration rebuttal, timely-void "
           "rebuttal and window, business-to-business exemption scope, report date, citation, last-verified date) and the contemporaneous void "
           "reason. Unknown inputs SHALL default to Hold and report at dormancy. State rows SHALL be populated from statute text, beginning "
           "with the states where sponsors concentrate; business-to-business exemptions SHALL default to not applicable to grants.",
           ["Given a state row marked unknown, when a voided grant check reaches dormancy, then it is reported.",
            "Given a cancelled grant with a lapse clause and a state allowing the no-consideration rebuttal, when disposition runs, then re-credit is permitted and evidenced."],
           ["RCD-047", "XID-BNK-003", "UNB-XIN-NPG-011#Q1"], ["UNB-RCN-GRT-005", "UNB-XIN-BNK-007", "REG-045", "OB-45"],
           "PAY", "initial-implementation", "Sponsor adoption of the disposition policy and grant-letter lapse clause.",
           OWN("Payments", "Unbound program ledger", "Sponsor CFO", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-C"]),
    clause("CNV-PAY-004", "Unknown payment outcomes",
           "Before the final aging tier an unknown payment SHALL have a bank trace or rail status request (ODFI trace, Fedwire IMAD/OMAD and "
           "beneficiary-bank confirmation, or paid-item file). Outcomes SHALL be classified Settled-and-accepted, Returned or Rejected, Not "
           "settled, or Unobtainable. Only Unobtainable after 30 days SHALL go to two independent approvers who record duplicate-risk "
           "acceptance and a recovery plan. No automatic retry SHALL occur; a check reissue SHALL require a confirmed stop or void first. "
           "Aging clocks SHALL use business calendars.",
           ["Given an ACH grant with no status after the window, when the final tier is reached without a trace request, then escalation is blocked until a trace is recorded.",
            "Given an unobtainable outcome, when one approver attempts reissue alone, then it is rejected."],
           ["RCD-042"], ["UNB-RCN-WF-022", "RCF-WF-003"],
           "PAY", "initial-implementation", "Sponsor CFO approval of aging tiers and delegation for two approvers.",
           OWN("Payments", "Bank / rail partner", "Sponsor CFO", "Sponsor", "Sponsor treasury", "Sponsor treasurer"), ["EV-C"]),
    clause("CNV-PAY-005", "Nacha authorization retention, reinitiation and return matching",
           "ACH authorizations SHALL be retained for their active life plus two years after termination or revocation, and not deleted while "
           "warranty claims may arise. Reinitiation SHALL be limited to R01 and R09 returns, at most twice within 180 days of the original "
           "settlement, in a separate batch described 'RETRY PYMT' with the same company name, ID and amount; R08 entries SHALL be reinitiated "
           "only with new authorization. Returns SHALL match on original entry trace number + return reason code + return trace; NOCs on "
           "original trace + change code + COR trace; when addenda are missing, a fallback match (amount, effective date, company ID, account "
           "last four, individual ID) SHALL route to a queue, never auto-post.",
           ["Given a third R01 return within 180 days, when reinitiation is requested, then it is refused.",
            "Given a return without addenda, when matched by fallback, then it is queued for a person."],
           ["UNB-XIN-PAY-001#Q1", "UNB-XIN-PAY-005#Q1", "UNB-XIN-EVA-005#Q1"], ["UNB-XIN-PAY-001", "UNB-XIN-PAY-005", "UNB-XIN-EVA-005"],
           "PAY", "initial-implementation", "Empirical: confirm each bank partner's return-file layout.",
           OWN("Payments", "Bank", "Sponsor treasurer", "Sponsor", "Sponsor treasury", "Sponsor treasurer"), ["EV-C"]),
    clause("CNV-PAY-006", "Donor-covered fees and service fees in acknowledgments",
           "When fee coverage is voluntary and the sponsor is merchant of record, the acknowledgment amount SHALL be the gross amount charged, "
           "the fund SHALL be credited gross and the processing fee SHALL post as an expense (to the fund only if the fee schedule discloses "
           "pass-through). Amounts paid to a non-charity (platform or mandatory convenience fees) SHALL be excluded from the acknowledgment "
           "and shown separately. Donor-paid service or membership fees SHALL be receipted as service fees with no deductibility statement; "
           "the preferred design is an additional contribution to the fund against which the disclosed fee is charged.",
           ["Given a voluntary $3 fee covered on a $100 card gift with the sponsor as merchant of record, when acknowledged, then the acknowledgment states $103 and the fund is credited $103.",
            "Given a mandatory platform fee, when acknowledged, then the fee is excluded and shown separately."],
           ["XID-BNK-002", "UNB-XIN-PAY-006#Q1", "XID-ERP-002", "UNB-XIN-ERP-002#Q1"], ["UNB-XIN-PAY-006", "UNB-RCN-FIN-001", "UNB-XIN-ERP-002"],
           "PAY", "sponsor-activation", "",
           OWN("Contributions", "Unbound program ledger", "Sponsor controller", "Sponsor", "Sponsor finance", "Sponsor controller"), ["EV-C"]),
    clause("CNV-PAY-007", "Bank activity feeds, debit blocks and fee bearers",
           "Bank adapters SHALL support BAI2/BTRS, ISO 20022 camt.052/053/054 and bank APIs, with prior-day feeds mandatory and intraday "
           "optional; spendable capacity SHALL derive from internal reservations against the last verified balance, never from a feed balance "
           "alone, and new releases SHALL fail closed when feed staleness exceeds the approved window. Disbursement accounts SHALL carry debit "
           "blocks allowing only bank fees and explicitly approved rail partners that fund by ACH debit. Return and intermediary fees SHALL be "
           "borne by the sponsor by default, domestic wires sent with no-deduction instructions and international wires default OUR; any "
           "pass-through SHALL be disclosed.",
           ["Given a feed stale beyond the approved window, when a new grant release is requested, then it is held with reason StaleBankFeed.",
            "Given an ACH debit from an unapproved originator against a disbursement account, when presented, then the bank block rejects it (bank-side test evidence recorded at activation)."],
           ["XID-BNK-007", "UNB-XIN-BNK-003#Q1", "XID-BNK-008", "XID-EVA-003"], ["UNB-XIN-BNK-003", "UNB-XIN-EVA-005", "PAY-06"],
           "PAY", "initial-implementation", "Bank information-reporting enrolment and debit-block service agreement per sponsor account.",
           OWN("Treasury", "Bank", "Sponsor treasurer", "Sponsor", "Sponsor treasury", "Sponsor treasurer"), ["EV-C"]),
    clause("CNV-PAY-008", "Card contributions deferred",
           "Card contribution acceptance SHALL be architecturally supported but disabled in release 1. When enabled for a sponsor it SHALL "
           "require an acquirer merchant agreement, a configurable per-gift cap and contest threshold, acknowledgment after settlement and "
           "correction on chargeback.",
           ["Given release-1 configuration, when a sponsor attempts to enable card intake, then it is refused with FeatureNotActivated."],
           ["XID-BNK-001"], ["UNB-XIN-PAY-003", "UNB-XIN-PAY-004"],
           "PAY", "deferred", "Acquirer merchant agreement.",
           OWN("Contributions", "Acquirer", "Sponsor CFO", "Sponsor", "Sponsor treasury", "Sponsor controller"), ["EV-C"]),
)

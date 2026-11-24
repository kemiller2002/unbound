# 17 — Release-1 scope (first-pass proposal)

**Date:** 2026-11-24.

**Status:** **proposed**. The product owner confirms or amends this scope. Nothing here changes a requirement's text or priority. Scope says *what ships first*, not whether a requirement is valid.

This proposal answers the scope questions the adversarial review raised (XID-ADV-001, -002, -011 and related decisions). It builds on the reconciliation's release-1 recommendation (RCD-013) and its deliberate exclusions (RCD-011, RCD-014).

## 1. Release-1 shape

| Dimension | In release 1 | Out of release 1 | Basis |
|---|---|---|---|
| Sponsors and models | One legal sponsor (the Unbound reference sponsor), D2C self-service plus advisor delegation, sponsor operations | White-label, distribution partners, embedded checkout, outsourced administration, institutional shadow-book sponsors, **high-volume, low-balance** | RCD-013; XID-ADV-002, -003 |
| Contributions | ACH debit (with Reg E authorization, account validation and return-rate monitoring), wire, check, public securities via DTC | **Card intake**, crypto, complex and illiquid assets | XID-ADV-001; RCD-011 |
| Grants | Domestic grants to IRS-listed §170(b)(1)(A) organizations; ACH by default, check otherwise; catalogued domestic intermediaries for foreign causes; fiscal sponsors | **Direct foreign grants (ED/ER)**, expenditure-responsibility grants, §530A | XID-ADV-001; RCD-014; XID-NPG-003 |
| Investments | One custodian. Sponsor-maintained single-sponsor pools **only if** securities counsel's opinion (XID-INV-003) is on file at launch; otherwise a cash sweep only | Advisor-managed accounts, alternatives, directly held positions, individualized advice | XID-INV-003, -004 |
| Service | Asynchronous messaging and scheduled callback; case kernel with about five profiles; complaint, reconsideration and investigation kinds | Live phone queues, IVR, **call recording** (disabled) | RCD-008; XID-SVC-002, -005 |
| Authentication | AAL2 with any authenticator for grant recommendations; phishing-resistant authentication for payee, contact-method and authority changes and for all workforce users | Partner-IdP step-up (no partners) | XID-ADV-009 (proposed) |
| Economics | Non-posting attribution with grades and GL tie-out; six metrics; per-call AI metering; December capacity forecast; monthly pack | Process mining, AI cost recommendations, profitability, thresholds, the improvement loop and its floors, service-charge analytics, in-platform simulation | XID-ADV-011 (proposed) |
| Configuration | Platform-maintained default packs, reviewed by counsel and CPA and adopted by attestation | Sponsor-authored packs | XID-ADV-010 (proposed) |

## 2. Proposed answers to the scope decisions

<!-- BEGIN GENERATED: scope-proposals -->
| Decision | Question | Proposed answer | Effect |
|---|---|---|---|
| XID-ADV-001 | Which optional money-movement and legal features are in release 1 (card contributions, ACH debit intake, direct foreign grants, call recording, pooled investment vehicles, distribution partners, embedded checkout)? | Release 1 includes ACH debit intake (with UNB-XIN-PAY-001/002/005), wires, checks and DTC securities; domestic grants to IRS-listed public charities and to catalogued domestic intermediaries. Excluded: card intake, direct foreign grants (ED/ER), call recording, distribution partners, white-label, embedded checkout, partner-exit tooling. Pooled investment options are included only if securities counsel's opinion is on file at launch; otherwise a cash sweep only. | Removes from release 1 the P0s for card disputes, foreign ED, OFAC general licences and partner funds-flow; keeps ACH-debit P0s. |
| XID-ADV-002 | Is high-volume-low-balance a release-1 business model, and if so what minimum economics (fee floor, servicing class, digital-only) are acceptable? | High-volume-low-balance is not a release-1 model. Revisit when metered cost-to-serve exists (UNB-XIN-SAS-009, UNB-XIN-ECO-004) and a fee floor passes the XID-ECO-002 rule. | UNB-XIN-ADV-001 deferred; minimum-fee shortfall waivers (UNB-XIN-ERP-001) remain as a control. |
| XID-ADV-011 | Release-1 scope of the economics capability. | Release-1 economics = non-posting attribution with grades (ECO-001..004), the six-metric catalog (ECO-007), per-call AI metering (ECO-009), December capacity forecast (ECO-017), scoped views (ECO-021) and the monthly pack only (ECO-022 cut down). Six metrics: cost per settled grant by rail; cost per settled contribution by rail; cost per account-month by balance band; cost per resolved case; cost per exception by type; AI cost per run — each with its counter-metric. Everything else after measurement exists; simulation stays in Echelon offline. | Economics requirements in R1 fall from 22 to 10. |
| XID-ADV-003 | How is grantable capacity computed when the general ledger or fund attribution is Shadow (external book of record)? | Not needed in release 1: Unbound is book of record for every function of the single sponsor. Keep UNB-XIN-ADV-002 for the first institutional sponsor. | UNB-XIN-ACC-010 and UNB-XIN-ADV-002 deferred. |
| XID-ADV-004 | Default co-advisor authority mode by instruction kind (resolves XID-TRS-002 vs XID-EVA-006). | Adopt the recommendation: any one advisor may recommend grants below a sponsor threshold (default $25,000, illustrative); all advisors required for larger grants, transfers out, closure, successor and authority changes; simultaneous conflicting instructions are held (UNB-XIN-EVA-012). | Resolves XIC-ADV-004. |
| XID-ADV-009 | What authentication strength applies to donors and advisors for grant recommendations, and may a partner IdP perform step-up? | AAL2 with any authenticator for grant recommendations; phishing-resistant authenticators for payee, contact-method and authority changes and for all workforce users. Partner-IdP step-up is moot in R1 (no partners). | Reduces donor friction found in XIF-ADV-018. |
| XID-ADV-010 | Who provides counsel/CPA-approved default configuration packs (rail obligations, unclaimed property, decline reasons, assurance matrix, recognition point, discretionary policy), and can a sponsor adopt them by attestation? | Yes: the platform maintains reviewed default packs (rail obligations, unclaimed-property table, decline reasons, contact assurance matrix, recognition point, discretionary policy); the release-1 sponsor adopts them by attestation and may only tighten them. | Cuts configuration load (XIF-ADV-004). |
| XID-ADV-008 | Are authenticator bindings (verified phone/email for OTP, registered devices) authority facts or engagement data? | Authenticator bindings are authority facts owned by IAM; changing one needs step-up on an existing factor and notice to the prior channel. | Resolves XIC-ADV-003. |
<!-- END GENERATED: scope-proposals -->

## 3. Effect on the requirement set

Every new requirement and refinement is classified in `data/release1-scope.jsonl`.

<!-- BEGIN GENERATED: scope-summary -->
| Priority | R1 | R1-conditional | Deferred | Rejected-recommended | Total |
|---|---|---|---|---|---|
| P0 | 29 | 3 | 4 |  | 36 |
| P1 | 60 |  | 15 | 1 | 76 |
| P2 | 29 | 1 | 21 | 1 | 52 |
| P3 | 3 |  | 12 | 2 | 17 |
| refinement | 1 |  |  |  | 1 |
| **Total** | 122 | 4 | 52 | 4 | 182 |
<!-- END GENERATED: scope-summary -->

**Reading the numbers**
- Most deferred P0s are tied to excluded features: card disputes, foreign equivalency determination, OFAC general licences and partner funds flow. Their legal duty disappears with the feature.
- Several P0s and P1s remain in release 1, so it is still substantial. This matches the review's finding that the controls core cannot shrink much without breaking legal or financial integrity.
- **Already-adopted records.** Four records adopted into the domain files on 2026-11-24 are deferred here: UNB-XIN-NPG-001, UNB-XIN-PAY-006, UNB-XIN-SAS-007 and UNB-XIN-EVA-009. Deferral does not withdraw them. They apply as soon as the feature is enabled.

## 4. Launch blockers that remain inside release 1

| Blocker | Why it remains | Fallback if not cleared |
|---|---|---|
| Securities counsel opinion on sponsor-maintained pools (XID-INV-003) | Pools are the core investment experience | Launch with cash sweep only |
| California opinion (XID-SAS-005) | The sponsor's own D2C giving app may still make the platform company a fundraising platform for California users | Launch outside California |
| CPA confirmation of recognition point, net-asset classification and late-correction policy | Needed before the first audited year | None. This is required before production |
| Bank partner's ACH and statement-file specification | Daily debit matching (UNB-XIN-BNK-003) and returns depend on it | Manual daily reconciliation, which is costly |
| Measured performance (report 18) | The architecture decision's falsification criteria | See report 18 |

## 5. Requirements outside release 1

<!-- BEGIN GENERATED: scope-out -->
| Requirement | Priority | Scope | Reason |
|---|---|---|---|
| UNB-XIN-PAY-003 | P1 | Deferred | card intake excluded |
| UNB-XIN-PAY-004 | P1 | Deferred | card intake excluded |
| UNB-XIN-PAY-006 | P1 | Deferred | no processor-settled intake in R1 |
| UNB-XIN-BNK-008 | P1 | Deferred | no processor-settled intake in R1 |
| UNB-XIN-BNK-009 | P2 | Deferred | cash ladder P2; single pool |
| UNB-XIN-ACC-008 | P1 | Deferred | cash and public securities only; UBTI exposure negligible |
| UNB-XIN-ACC-009 | P2 | Deferred | SOC 1 only for outsourced/white-label |
| UNB-XIN-ACC-010 | P1 | Deferred | Unbound is book of record in R1 |
| UNB-XIN-INV-001 | P2 | Deferred | no advisor-managed accounts |
| UNB-XIN-INV-002 | P0 | R1-conditional | pools only if securities counsel opinion (XID-INV-003) is on file; fallback: cash sweep only |
| UNB-XIN-INV-003 | P2 | Deferred | gifted securities liquidated; no directly held positions |
| UNB-XIN-INV-004 | P3 | Deferred | class actions P3 |
| UNB-XIN-INV-005 | P2 | Deferred | no alternative assets |
| UNB-XIN-INV-007 | P3 | Deferred | no crypto (RCD-011) |
| UNB-XIN-SVC-003 | P0 | R1-conditional | satisfied in R1 by keeping recording disabled |
| UNB-XIN-SVC-008 | P3 | Deferred | merged into ECO-017 |
| UNB-XIN-CLM-007 | P2 | Deferred | privileged access class P2 |
| UNB-XIN-CLM-008 | P2 | Deferred | no outsourced service vendor |
| UNB-XIN-ERP-003 | P2 | Deferred | no operator-to-tenant billing with one sponsor |
| UNB-XIN-CRM-005 | P2 | Deferred | servicing models deferred |
| UNB-XIN-CRM-006 | P1 | Deferred | no referral programmes |
| UNB-XIN-TRS-002 | P2 | Deferred | legacy intent P2 |
| UNB-XIN-TRS-003 | P1 | Deferred | natural-person fund holders only in R1 |
| UNB-XIN-TRS-006 | P3 | Deferred | no family-office feeds |
| UNB-XIN-NPG-001 | P0 | Deferred | direct foreign grants excluded; intermediaries used |
| UNB-XIN-NPG-007 | P3 | Deferred | §530A disabled (XID-NPG-003) |
| UNB-XIN-NPG-009 | P2 | Deferred | direct foreign grants excluded |
| UNB-XIN-NPG-010 | P0 | Deferred | direct foreign grants excluded (domestic OFAC screening remains via existing UNB-RCN-GRT-007) |
| UNB-XIN-REG-003 | P1 | Rejected-recommended | XRJ-ADV-005 (merged into CRM-003) |
| UNB-XIN-SAS-001 | P2 | R1-conditional | California D2C giving requires the opinion in XID-SAS-005; fallback: launch outside California |
| UNB-XIN-SAS-004 | P2 | Deferred | no partner webhooks |
| UNB-XIN-SAS-006 | P2 | Deferred | no third-party apps |
| UNB-XIN-SAS-007 | P1 | Deferred | no distribution partners |
| UNB-XIN-SAS-010 | P2 | Deferred | single sponsor; sandbox deferred |
| UNB-XIN-SAS-011 | P2 | Deferred | subprocessor register P2 |
| UNB-XIN-OTH-001 | P3 | Deferred | no white-label programs |
| UNB-XIN-OTH-002 | P1 | Deferred | no migration in R1 |
| UNB-XIN-OTH-003 | P0 | Deferred | no partners in R1 (merge into REG-001 recommended) |
| UNB-XIN-ECO-005 | P2 | Deferred | FOCUS vendor records after R1 |
| UNB-XIN-ECO-006 | P2 | Deferred | time-driven labour costing after measurement exists |
| UNB-XIN-ECO-008 | P3 | Deferred | cost quotes after R1 |
| UNB-XIN-ECO-010 | P3 | Deferred | process mining after R1 |
| UNB-XIN-ECO-011 | P2 | Deferred | AI recommendations after R1 |
| UNB-XIN-ECO-012 | P3 | Deferred | profitability after R1 |
| UNB-XIN-ECO-013 | P3 | Rejected-recommended | XRJ-ADV-001 |
| UNB-XIN-ECO-014 | P2 | Deferred | thresholds after R1 |
| UNB-XIN-ECO-015 | P3 | Deferred | improvement loop after measurement exists |
| UNB-XIN-ECO-016 | P2 | Deferred | floors ship with the loop |
| UNB-XIN-ECO-018 | P3 | Deferred | no special service charges in R1 |
| UNB-XIN-ECO-019 | P3 | Deferred | merge into REQ-X-042 recommended |
| UNB-XIN-ECO-020 | P3 | Rejected-recommended | XRJ-ADV-002 |
| UNB-XIN-EVA-008 | P2 | Deferred | no published partner events |
| UNB-XIN-EVA-009 | P1 | Deferred | no bulk sponsor transfers |
| UNB-XIN-EVA-013 | P2 | Rejected-recommended | XRJ-ADV-003 (move into ADR) |
| UNB-XIN-ADV-001 | P1 | Deferred | high-volume-low-balance not an R1 model |
| UNB-XIN-ADV-002 | P1 | Deferred | Unbound is book of record in R1 |
| RR-UNB-INV-019-r1 | P0 | R1-conditional | with pools |
| RR-UNB-TEN-011-r1 | P1 | Deferred | tenant lifecycle beyond single tenant deferred |
| RR-UNB-TEN-016-r1 | P0 | Deferred | sponsor exit tooling deferred (one sponsor) |
| RR-UNB-API-STD-003-r1 | P1 | Deferred | no external API consumers in R1 |
<!-- END GENERATED: scope-out -->

## 6. Requirements in release 1

<!-- BEGIN GENERATED: scope-in -->
| Requirement | Priority | Statement |
|---|---|---|
| RR-ACC-MDL-007-r1 | P0 | Posting MUST be idempotent per (source event ID, accounting book): at most one original journal may exist for each pair, whatever the posting-rule version. A posting-rule version… |
| RR-ADM-023-r1 | P0 | The system MUST classify instructions as conflicting when their scopes overlap and cannot both execute, and MUST resolve them by the precedence rules: explicit author-confirmed su… |
| RR-ADM-028-r1 | P0 | Fee administration MUST execute as an idempotent run per (fund, schedule, period, segment set) through the states Accrued, Assessed, Posted, and optionally Waived or Reversed; wai… |
| RR-TVR-CON-001-r1 | P0 | Contribution provisional status MUST be governed by a versioned ReversalWindow table keyed by (rail, SEC code or card network, account type consumer/business, direction push/pull)… |
| RR-UNB-ACC-033-r1 | P0 | Each program document version and each fund-intake agreement (including legacy imports and sponsor-to-sponsor transfers-in) MUST record a CPA-reviewed VariancePowerEvidence (claus… |
| RR-UNB-EVT-001-r1 | P0 | The governed event catalog MUST record, for every event type: owning module; authority class (authoritative-financial, authoritative-nonfinancial, derived, informational, observat… |
| RR-UNB-EVT-005-r1 | P0 | The F# domain model MUST define distinct types Observation (any source including AI; no authority), Proposal (a suggested command with evidence; no authority), Command (an authent… |
| RR-UNB-GRANT-041-r1 | P0 | A recommendation to a foreign recipient MUST be routed deterministically to exactly one InternationalRoute: (1) RoutineListed — recipient holds a US IRS determination as §170(b)(1… |
| RR-UNB-PAY-008-r1 | P0 | Production enablement of any rail for any sponsor MUST pass a RailGoLive gate that requires: (a) all binding RailObligation rows for that rail and sponsor role mapped to implement… |
| UNB-XIN-ACC-001 | P0 | Every account type in the account-type register MUST carry a CPA-approved AccountingTreatment of exactly one of {Contribution-WithoutDonorRestriction, Contribution-WithDonorRestri… |
| UNB-XIN-BNK-002 | P0 | For each sponsor that originates ACH entries (directly or via a Third-Party Sender), the system MUST maintain a documented FraudMonitoringProgram covering at least: destination-ch… |
| UNB-XIN-BNK-003 | P0 | Every business day the system MUST ingest bank activity for each sponsor disbursement, clearing and operating account linked to Unbound and match every debit to exactly one author… |
| UNB-XIN-BNK-006 | P0 | A check MUST NOT move from Printed to Released-for-mail until the bank has acknowledged an issue file containing its number, amount, issue date and (where the bank supports it) pa… |
| UNB-XIN-ERP-002 | P0 | Every fee schedule MUST declare exactly one payer source from {FundAssets, DonorExternal, PartnerInvoice, SponsorAbsorbed}. FundAssets fees MUST post only as fund-level fee entrie… |
| UNB-XIN-ERP-005 | P0 | Each delegation-of-authority entry MUST declare decision type, monetary limit per item, cumulative limit, aggregation window, aggregation key (approver x counterparty, where count… |
| UNB-XIN-EVA-001 | P0 | For each sponsor legal entity and for every function for which Unbound is designated book of record (UNB-XIN-ACC-010), fund balances, reservations, capacity inputs and every finan… |
| UNB-XIN-EVA-002 | P0 | Every posting command MUST read the accounting period row for its posting date with a shared lock (or run SERIALIZABLE) in the posting transaction; HardClosePeriod MUST take an ex… |
| UNB-XIN-EVA-005 | P0 | Each external observation type MUST have a registered business idempotency key derived from source data (e.g., ACH return: original entry trace number + return reason code + retur… |
| UNB-XIN-EVA-010 | P0 | The system MUST model at least these distinct hold types with a declared set of blocked commands: ProtectiveHold(charity\|recipient\|destination) blocks release and dispatch to th… |
| UNB-XIN-EVA-011 | P0 | A batch (recurring-grant run, payment file, statement run, fee run) MUST be a container of independent item commands and MUST NOT carry approval, release or dispatch authority its… |
| UNB-XIN-IAM-001 | P0 | Every principal MUST belong to exactly one realm: PlatformWorkforce, SponsorWorkforce (per tenant, federated or local), PartnerWorkforce (per partner, federated), Customer (donor,… |
| UNB-XIN-IAM-002 | P0 | Platform-workforce access to a tenant's non-public data MUST be permitted only (a) under an OperatorAccessDelegation pre-approved by the sponsor for listed task classes, scopes an… |
| UNB-XIN-IAM-003 | P0 | Unbound MUST maintain one StepUpPolicy matrix mapping action classes (authenticator/recovery change, authority or delegation change, payee or destination change, data export, gran… |
| UNB-XIN-IAM-005 | P0 | Break-glass access MUST use sealed credentials independent of the workforce IdP, require two-person activation, expire automatically within 4 hours, page the security owner and no… |
| UNB-XIN-NPG-008 | P0 | When any rule, audit or human report identifies a completed payment as a possible taxable distribution (§4966), prohibited benefit (§4967) or excess benefit (§4958), the system MU… |
| UNB-XIN-PAY-001 | P0 | Before the first recurring debit from a consumer account (ACH or debit card), the system MUST capture an authorization signed or similarly authenticated by the consumer that state… |
| UNB-XIN-PAY-002 | P0 | The system MUST validate a consumer account number with a configured commercially reasonable method (prenote, micro-entry, or third-party/API validation) before the first WEB debi… |
| UNB-XIN-REG-001 | P0 | Every deployment MUST record a FundsFlow declaration naming, for each account that holds contribution or grant money, the legal title holder and every party with instruction autho… |
| UNB-XIN-SVC-004 | P0 | Unbound MUST maintain a versioned ContactAssuranceMatrix mapping (channel x achieved assurance level x principal role) to permitted disclosure classes (public, account-summary, tr… |
| RR-ADM-008-r1 | P1 | A household MUST be a display grouping only: membership MUST NOT grant access to any fund, MUST NOT change authority, contributor of record or acknowledgments, and MUST NOT aggreg… |
| RR-ADM-033-r1 | P1 | Every case profile and decision type MUST declare its clocks (Response, Resolution, Decision) with explicit start, stop and pause events, a sponsor business calendar and a target.… |
| RR-AIN-028-r1 | P1 | Bound token, tool, time and external-service budgets per agent run and per purpose. Each run SHALL meter cost per invocation and attach it to its cost object (UNB-XIN-ECO-009). On… |
| RR-REQ-X-038-r1 | P1 | Disposition MUST operate on DispositionBatches defined by (tenant, record class, retention trigger period) rather than per-record review. When records in a batch reach EligibleFor… |
| RR-REQ-X-048-r2 | P1 | A WorkItem MUST be created only when (a) a rule-defined hold, (b) missing or conflicting evidence, (c) a decision requiring human authority, (d) a provider Unknown requiring recon… |
| RR-REQ-X-073-r1 | P1 | Unbound MUST maintain a critical-vendor register recording assurance type, legal entity and report period; MUST require a bridge letter when the period end is more than 3 months o… |
| RR-UNB-ACC-014-r1 | P1 | Support budgeting, forecasts, actual-versus-budget reporting, cost centers and scenario models while distinguishing forecasts from posted actuals. Management cost allocation and a… |
| RR-UNB-ACC-031-r1 | P1 | Unbound MUST preserve gift-lot identity (contribution ID, quantity, receipt date, sponsor carrying amount at receipt) for every contributed security until disposition. Donor cost… |
| RR-UNB-DATA-004-r1 | P1 | Records MUST be classified into storage classes. (1) Authoritative financial and authority facts (journal, reservations, grant/payment/contribution decisions, holds, policy versio… |
| RR-UNB-DATA-005-r1 | P1 | The system MUST maintain a versioned correction impact registry that maps each correctable fact type to the derived records computed from it (e.g., contribution valuation -> ackno… |
| RR-UNB-GAP-013-r1 | P1 | Every inbound receipt (cash or securities) that cannot be matched deterministically to one contribution intent MUST be held in an Unidentified suspense state with no fund credit,… |
| RR-UNB-GRANT-024-r1 | P1 | Each grant MUST be assigned one GrantTermsTier by a counsel-approved deterministic rule: Tier 0 (routine grant to a Permitted recipient with no conditions) — no agreement; sponsor… |
| RR-UNB-RES-003-r1 | P1 | Unbound MUST define recovery objectives per data class, not per capability, in one versioned table, at minimum: (C0) acknowledged authoritative financial/security commits (ledger,… |
| UNB-XIN-ACC-002 | P1 | Each sponsor book MUST declare one GrantRecognitionPoint from {ApprovalCommunicatedToGrantee (default), PaymentReleased}, effective-dated and CPA-approved; PaymentReleased MAY be… |
| UNB-XIN-ACC-005 | P1 | Each fiscal year MUST have an AuditAdjustment period after period 12 that accepts only journals linked to an AuditFinding (auditor reference, proposer, controller decision). Each… |
| UNB-XIN-ACC-006 | P1 | For each sponsor and fiscal year Unbound MUST produce a DAF bridge that reconciles (a) Schedule D Part I line 4 aggregate value, (b) the sum of year-end fund statement values, and… |
| UNB-XIN-BNK-001 | P1 | The system MUST maintain a versioned RailObligation register with one row per (rail, direction, sponsor role ∈ {Originator, ThirdPartySender, Payee, Merchant, Holder, BankCustomer… |
| UNB-XIN-BNK-004 | P1 | The system MUST maintain a versioned RailEligibility matrix keyed by (payment class ∈ {Grant, SponsorToSponsorTransfer, ReturnOfUnacceptedProperty, ProcessorRefundOfDuplicate, Ven… |
| UNB-XIN-BNK-007 | P1 | The system MUST maintain a counsel-approved UnclaimedPropertyRule table per state {owner type (organization/individual), property type (vendor/grant/refund check), dormancy period… |
| UNB-XIN-CLM-001 | P1 | A reconsideration Case MUST: (a) be accepted only within a sponsor-configured filing window (default 90 days from decision notice) and at most once per decision unless new materia… |
| UNB-XIN-CLM-002 | P1 | Every decline, hold longer than its profile's notice threshold, or conditional approval communicated to a donor, advisor or charity MUST include: a reason code from a counsel-appr… |
| UNB-XIN-CLM-003 | P1 | Every request for information to a donor, advisor, charity or provider MUST be an InformationRequest with: requested facts (typed), reason, due date, reminder schedule, effect on… |
| UNB-XIN-CLM-004 | P1 | Suspected fraud, related-party diversion, account takeover and sanctions potential-match matters MUST be handled in Cases with access class restricted-investigation. Such cases MU… |
| UNB-XIN-CLM-006 | P1 | A representative (agent under power of attorney, guardian, conservator, family delegate) MUST act only within the scope recorded in the delegation (ADM-005). An authorized staff m… |
| UNB-XIN-CRM-001 | P1 | Party identity MUST consist of a minimal core (internal PartyId, legal name, identifiers needed for screening, verification state). All other personal and financial attributes MUS… |
| UNB-XIN-CRM-002 | P1 | Onboarding of every advisory-privilege holder and annual attestation MUST capture declared affiliations (officer, director, employee, substantial contributor) with charities and o… |
| UNB-XIN-CRM-003 | P1 | Each message type MUST declare a primary-purpose class (Commercial, TransactionalOrRelationship, MandatoryLegal, Security) and a sender legal entity with its entity type (tax-exem… |
| UNB-XIN-CRM-007 | P1 | A professional-advisor relationship MUST be scoped to the pair (individual advisor, firm). When the firm or a verified HR/SCIM feed reports the advisor's departure, or the advisor… |
| UNB-XIN-ECO-009 | P1 | Every model invocation SHALL record {AIRunId, model id/version, input/output tokens, tool calls, EffectiveCost, latency, cost-object keys, purpose}. Each AIRun SHALL roll up to it… |
| UNB-XIN-ERP-001 | P1 | The fee engine MUST compute every fund-level fee daily on the daily balance of the declared FeeBase, using actual/365 (actual/366 in leap years) unless the schedule version declar… |
| UNB-XIN-ERP-004 | P1 | Unbound MUST keep grant-recipient payment profiles separate from any operating-vendor payee data it receives or exports, and MUST NOT allow a grant payment instruction to referenc… |
| UNB-XIN-ERP-007 | P1 | The event catalog MUST classify every fee, billing and relationship event as AuthoritativeFinancial, AuthoritativeAuthority, or Informational. Only the fee engine may emit Authori… |
| UNB-XIN-EVA-003 | P1 | Every financial report, fund statement, regulatory data package and management financial dashboard figure MUST be computed from a FinancialSnapshotSealed identified by (sponsor le… |
| UNB-XIN-EVA-004 | P1 | For each external source (bank statement/return files, custodian position and transaction files, card/ACH processor reports, IRS exempt-organization extracts, sanctions list publi… |
| UNB-XIN-EVA-006 | P1 | Every authoritative fact, derived event, case, AI observation, notification and external observation item MUST carry (a) correlation_root: the business root id (ContributionId, Gr… |
| UNB-XIN-EVA-007 | P1 | Each long-running business workflow MUST be implemented as a process manager persisted as a versioned row in the owning module's schema with: current step, expected next facts, ne… |
| UNB-XIN-EVA-012 | P1 | Each advisor instruction MUST carry a conflict scope key (fund, instruction kind, and target: charity, schedule, investment allocation or transfer). On commit, the handler MUST, i… |
| UNB-XIN-EVA-014 | P1 | Relationship data MUST be classified as either (a) authority facts — advisory privilege grants and revocations, successor designations, contributor of record, anonymity and recogn… |
| UNB-XIN-EVA-015 | P1 | Every notification derived from a fact MUST record the fact id and version it describes. Immediately before external send, the delivery worker MUST re-read the fact's current vers… |
| UNB-XIN-EVA-016 | P1 | Records of different authority classes MUST be stored in physically separate tables (or schemas) with distinct database privileges: journal and reservation tables (financial facts… |
| UNB-XIN-IAM-004 | P1 | Access recertification MUST run at least quarterly for privileged and workforce roles and at least annually for delegated customer roles (advisors, authorized viewers, agents), co… |
| UNB-XIN-NPG-003 | P1 | A grant naming a project hosted by a fiscal sponsor MUST record a FiscalSponsorshipEvidence item with: arrangement type (comprehensive/project-of-sponsor, pre-approved grant relat… |
| UNB-XIN-NPG-004 | P1 | Each grant MUST carry, at approval, the attributes needed for Form 990 Schedules I and F as mapped in a CPA-approved, form-year-versioned data dictionary: recipient legal name, ad… |
| UNB-XIN-NPG-005 | P1 | The system MUST apply a counsel-approved, versioned OrganizationChangeEffect table to every open grant and active schedule when a change event is recorded for the recipient: NameC… |
| UNB-XIN-NPG-006 | P1 | Every sponsor discretionary hold or decline not required by law MUST cite a criterion from a board-approved, versioned DiscretionaryGrantPolicy, record the evidence class (allegat… |
| UNB-XIN-NPG-011 | P1 | For every check reaching AgedOutreach, the system MUST determine the reporting jurisdiction by a counsel-approved priority rule (default: state of the payee's last known address i… |
| UNB-XIN-OTH-004 | P1 | Every print/mail batch MUST carry a manifest of piece ids (check number or document artifact id) and the print vendor MUST return a disposition per piece (Printed, Spoiled, Reprin… |
| UNB-XIN-PAY-005 | P1 | The system MUST compute, per Originator identity, rolling 60-day unauthorized (R05, R07, R10, R11, R29, R51), administrative (R02, R03, R04) and overall debit return rates; MUST a… |
| UNB-XIN-PAY-007 | P1 | Securities gift intake MUST collect a GiftIntent (donor, fund, security identifier, quantity, delivering DTC participant number, expected date); incoming custody receipts MUST be… |
| UNB-XIN-REC-001 | P1 | A LegalHold MUST list, at creation and on any scope change, every registered processor, distribution partner and deployment cell holding in-scope data, and MUST send each a preser… |
| UNB-XIN-REG-002 | P1 | The system MUST maintain an OperatorStatusRegister mapping status-relevant features to the legal question they raise: (a) operator-provided investment model portfolios, recommenda… |
| UNB-XIN-SAS-005 | P1 | Every authoritative event, outbox message, inbox record, audit record and telemetry span MUST carry an immutable envelope with TenantId, LegalSponsorId, ProgramId (where applicabl… |
| UNB-XIN-SVC-002 | P1 | Unbound MUST implement one Case kernel shared by all case kinds, holding: case ID, kind, profile version, sponsor scope, subject references, owner, assignees, routing decisions, t… |
| UNB-XIN-SVC-005 | P1 | Unbound MUST classify as a complaint any Contact in which a party expresses dissatisfaction with the sponsor's or platform's service, decision, communication or conduct, regardles… |
| UNB-XIN-SVC-009 | P1 | Transcripts and AI summaries of interactions MUST be labelled machine-generated, MUST be stored as notes linked to the Contact, and MUST NOT be accepted as input to any command th… |
| UNB-XIN-TRS-001 | P1 | Every charitable beneficiary designation MUST be validated against eligibility at designation and re-validated at activation using current registry facts. If at activation a named… |
| UNB-XIN-TRS-004 | P1 | A legal representative MUST NOT exercise advisory privileges on behalf of a principal unless (a) the instrument's verified scope covers charitable recommendations or the principal… |
| UNB-XIN-TRS-005 | P1 | A succession policy MUST declare a successor response window and a default action. After a verified trigger, if no designated successor accepts within the window following documen… |
| UNB-XIN-WFA-001 | P1 | Every deadline-bearing obligation MUST be persisted as a DueObligation record in the tenant's authoritative store, in the same transaction as the state change that creates it, wit… |
| UNB-XIN-WFA-002 | P1 | Each long-running process instance (grant, payment attempt, migration run, partner exit, disposition case, fund closure) MUST record the state-machine definition version at creati… |
| RR-ADM-042-r1 | P2 | Unbound MUST compute, per sponsor and month: contacts per 1,000 accounts by reason and channel; repeat-contact rate (same principal and reason within 7 calendar days); deflection… |
| RR-CTL-040-r1 | P2 | Service QA MUST sample closed Contacts monthly by stratified random selection over channel and answer source with a recorded seed, score them against a versioned rubric (factual,… |
| RR-UNB-ECO-001-r1 | P2 | Model accounts, AUM, average balance, grants (by rail), contributions (by type), exception rate by type, review minutes per exception type, complex-asset volume, service contacts… |
| RR-UNB-ENT-003-r1 | P2 | The organization SHALL maintain a documented sponsor business model and revenue/fee policy, and unit economics that are (a) computed monthly from measured attributed costs reconci… |
| RR-UNB-GRANT-026-r1 | P2 | Grantee onboarding MUST NOT be a precondition for a routine grant when an existing verified destination or a check/approved disbursement-partner path is available. Payment-destina… |
| UNB-XIN-ACC-003 | P2 | Posted journals MUST be persisted as authoritative records and MUST NOT be regenerated from source events. Unbound MUST run a scheduled re-derivation that recomputes, under each j… |
| UNB-XIN-ACC-004 | P2 | Every posting-rule version change and every correction of a posted error MUST carry an AccountingChangeRecord classified by an authorized controller as ChangeInPrinciple, ChangeIn… |
| UNB-XIN-ACC-007 | P2 | Every investment holding at each reporting date MUST carry a FairValueLevel in {Level1, Level2, Level3, NAVPracticalExpedient} with source and approver for Level3/NAV-PE; every fi… |
| UNB-XIN-CLM-005 | P2 | Each month Unbound MUST draw a stratified random sample of closed grant approvals (straight-through and human), charity vetting determinations and gift acceptances, sized by a cou… |
| UNB-XIN-CRM-004 | P2 | Segment definitions used for communications, analytics shared outside compliance, or partner reporting MUST draw attributes only from a versioned allowlist. Attributes derived fro… |
| UNB-XIN-ECO-001 | P2 | The platform SHALL maintain operational cost attribution as a management-accounting projection (CostAttribution read model) derived from GL actuals, vendor cost records and domain… |
| UNB-XIN-ECO-002 | P2 | The platform SHALL implement typed entities CostPool (GL account set + period + bearer + pool type in {Labour, Vendor, Infrastructure, AIInference, PaymentRail, CustodyBanking, Ve… |
| UNB-XIN-ECO-003 | P2 | Every AttributedCost and every derived unit-cost figure SHALL carry method in {DirectMetered, TimeDriven, DriverBased, ResidualPolicy}, confidence grade in {A, B, C, D, U} and a l… |
| UNB-XIN-ECO-004 | P2 | For each closed accounting period and sponsor scope, the platform SHALL execute a ReconciliationRun that compares the sum of AttributedCost plus UnallocatedResidual plus reported… |
| UNB-XIN-ECO-007 | P2 | The metric registry SHALL contain versioned definitions (numerator, denominator, eligible population, inclusion/exclusion, time basis, grade rule) for at least: cost per account-m… |
| UNB-XIN-ECO-017 | P2 | The platform SHALL produce a rolling 13-week and next-December capacity forecast per sponsor and skill pool: forecast arrivals by work type (daily), expected handle minutes from t… |
| UNB-XIN-ECO-021 | P2 | Cost views SHALL be scoped: sponsor users see only their sponsor's attributed costs plus their share of shared-platform costs with the allocation method disclosed; cross-sponsor b… |
| UNB-XIN-GOV-001 | P2 | Every counsel-approved rule version that encodes an authority with worked examples MUST ship with executable golden fixtures transcribing those examples (e.g. Rev. Proc. 2017-53 §… |
| UNB-XIN-INV-006 | P2 | Every cash-bearing account MUST declare its sweep vehicle, yield source and an IncomeAttribution rule (to funds pro rata by daily cash balance, to pool, or to sponsor operating) a… |
| UNB-XIN-INV-008 | P2 | Every report, invoice, filing extract or API field that presents an assets figure MUST reference a named, versioned AssetMeasure definition (inclusion rules, valuation snapshot ty… |
| UNB-XIN-NPG-002 | P2 | When a recommendation names a foreign recipient for which the sponsor has no current EquivalencyDetermination and does not offer expenditure responsibility, the system MUST presen… |
| UNB-XIN-REC-002 | P2 | Unbound MUST produce, on an authorized LegalProductionRequest scoped to one tenant, a production package containing the selected records and documents in original and rendered for… |
| UNB-XIN-SAS-002 | P2 | Every feature flag MUST have an owner, creation date, expiry date no more than 90 days after creation (extension requires architecture approval), and a declared kind (ReleaseToggl… |
| UNB-XIN-SAS-003 | P2 | Each release MUST be deployed in declared rings (synthetic/sandbox cells, then a canary production cell, then remaining cells) with automatic halt when any health or invariant che… |
| UNB-XIN-SAS-008 | P2 | Unbound MUST define SLIs and SLOs for at least: grant recommendation acceptance latency and availability, donor portal availability, partner API availability, statement availabili… |
| UNB-XIN-SAS-009 | P2 | Unbound MUST emit an append-only UsageRecord per tenant and program for: authoritative storage bytes, document/object storage bytes, API calls by tier, background compute seconds,… |
| UNB-XIN-SVC-001 | P2 | Unbound MUST record every inbound or outbound human interaction as a Contact (channel, principal or 'unidentified', assurance level, reason code, linked subjects, outcome, linked… |
| UNB-XIN-SVC-006 | P2 | Any statement to a customer that a person or the system will do something by a time MUST be recorded as a Commitment (what, to whom, owner, due time, channel, linked Contact/Case)… |
| UNB-XIN-SVC-007 | P2 | Unbound MUST maintain a sponsor-scoped, versioned Answer Corpus of approved clauses (ID, version, text, approver, effective dates, topics, locale). Any staff script, help page or… |
| UNB-XIN-BNK-005 | P3 | Every contribution, payment, return, dispute, reissue and payment exception event MUST emit a CostDriver fact {sponsor, payment class, rail, provider, external fee amount and sour… |
| UNB-XIN-ECO-022 | P3 | The platform SHALL provide the Operational Efficiency dashboards defined in report section "Operational efficiency dashboard requirements" (Economics Overview, Unit Cost Explorer,… |
| UNB-XIN-ERP-006 | P3 | Every work item and case MUST carry the tenant, program, servicing model and fund-balance band of its subject at creation, an effort category from a sponsor-independent catalog, a… |
| RR-REQ-X-069-r1 |  | Grant-outcome and restriction monitoring MUST be sponsor-configurable and OFF by default for grants to §170(b)(1)(A) organizations (other than disqualified supporting organization… |
<!-- END GENERATED: scope-in -->

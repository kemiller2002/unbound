# 14 — Final adversarial review (mission §16)

**Method.** After the first pass, a separate agent pass was given the records and the industry reports. It was not given the integrator's conclusions. Its instruction was to **assume the platform fails** and find out why. It tested 14 failure causes, challenged priorities and over-engineering, looked for contradictions between research passes, spot-checked 11 legal claims against primary sources, and recomputed the worked examples.

It is the same model family as the research passes, so this is **not** independent human review. It read the economics and event-architecture reports in full, the SaaS report in part, and the other five reports through their data files only.

**Outcome.** The review changed the findings. The integrator applied every change listed under "Revisions applied" below, and the records keep the original values.

## 1. Verdict by failure cause

| # | Would the platform fail because of… | Verdict | Evidence | Response |
|---|---|---|---|---|
| 1 | Missing business capabilities | Unlikely | The package is broad. Two real gaps: a servicing-cost ceiling per account type, and grantable capacity when an external system is the book of record | UNB-XIN-ADV-001, UNB-XIN-ADV-002 added |
| 2 | Incorrect legal assumptions | Plausible | 9 of 11 spot-checked claims are substantively correct. **Errors found:** the Rev. Proc. 2017-53 currency rule is §3.03(1), not §4.02(1); the integrator verified this against the IRS PDF and corrected it. The Colorado SB 26-189 domain list omitted financial services. GrantRecognitionPoint offered options GAAP does not support as general policy. The real legal risk sits in open counsel questions. | Citation corrected in every record; XRA-020 corrected and downgraded to low confidence; XID-ADV-006 |
| 3 | Inadequate accounting controls | Unlikely | Journal authority, period lock and the ACC set are strong | XIC-ADV-002, XIC-ADV-005 resolutions |
| 4 | Poor customer service | Plausible | Workforce-grade controls are applied to donors: phishing-resistant step-up within 5 minutes, no partner-IdP step-up, phone balance enquiries blocked without a device, advisor auto-suspension after 14 days, annual attestation for every advisor | XIF-ADV-018; XID-ADV-009 (AAL2 for recommendations; phishing-resistant only for payee and authority changes) |
| 5 | **Excessive operational overhead** | **Likely** | 32 of 148 statements need counsel or CPA approval. Independent reviewers are required repeatedly while the staffing floor is 6–8 people (TVC-018). The economics loop runs weekly, monthly and quarterly. | Priority revisions; XID-ADV-010 default packs; economics cut to release-1 scope (XID-ADV-011) |
| 6 | Bad financial data | Unlikely for the ledger; plausible for cost data | Feed completeness and idempotency are good. Economics unit costs could not be reproduced, and the reconciliation identity was true by construction. | Report 11 corrected; XIF-ADV-008 |
| 7 | Inadequate integration architecture | Plausible | P0 controls depend on bank and custodian file specifications nobody has obtained. If the shared hold registry is unavailable, releases stop for every sponsor. | XIF-ADV-020; report 12 qualified (XIF-ADV-007) |
| 8 | **Excessive configuration complexity** | **Likely** | At least 14 new versioned registers or matrices per sponsor database, with no defaults provided | XIF-ADV-004; **XID-ADV-010: the platform maintains counsel- and CPA-reviewed default packs; sponsors attest adoption and may only tighten them** |
| 9 | Weak security boundaries | Unlikely | IAM, contact assurance and separate authority-class stores are strong. Residual risks: contact details serve as authentication factors but are classed as editable engagement data, and the operator-access rule has no break-glass path. | XIC-ADV-003, XIC-ADV-007; XID-ADV-008 |
| 10 | Unreliable AI decisions | Unlikely | AI is tightly fenced. The opposite risk is that it does too little, which feeds cause 5. | Recorded |
| 11 | Inability to support large institutional clients | Plausible | When an external system is the book of record ("shadow" mode, UNB-XIN-ACC-010), the P0 journal-only rule (UNB-XIN-EVA-001) left no lawful source of grantable capacity | UNB-XIN-ADV-002; EVA-001 scoped (report 12 §5) |
| 12 | **Inability to profitably serve small accounts** | **Likely** | Unbound's own model implies about $272 per account per year (economic-model.md:96). The economics example implies about $244, with custody alone at $14.40. Minimum-fee shortfalls must be waived (UNB-XIN-ERP-001). The high-volume/low-balance model has only metering. A cited $1.37-per-account example was a misanalogy, because those processor accounts are a different unit. | **XIF-ADV-001 (critical)**; UNB-XIN-ADV-001; **XID-ADV-002** |
| 13 | Failure to handle unusual transactions | Unlikely | Coverage is wide | Still open: who bears fees on a returned wire (XID-EVA-003), the shortfall after a misposted contribution has already been granted, and a revocation not yet ingested |
| 14 | Failure to support multiple business models | Plausible | Only D2C, hybrid and outsourced administration have no open blocker. High-volume/low-balance fails on economics. HNW evidence is low-confidence. Embedded, investment-centric and advisor-distribution each wait on a legal or identity decision. | XIF-ADV-022; report 13 |

## 2. Revisions applied

1. **Priorities.** 48 challenges applied, so new-requirement P0s fell from 40 to 25:
   - 14 moved from P0 to P1, and one from P0 to P2;
   - 20 moved from P1 to P2;
   - 10 moved from P2 to P3;
   - the remaining three became dispositions: one P0 merged and two P3s rejected.

   Each changed record keeps `priority_as_proposed` and `priority_revision`. See [03](03-MISSING-REQUIREMENTS-REGISTER.md).
2. **Dispositions.** Seven requirements are marked for rejection or merger and stay visibly in the register until the owner decides:
   - reject: UNB-XIN-ECO-013, UNB-XIN-ECO-020;
   - move into the ADR: UNB-XIN-EVA-013;
   - merge: UNB-XIN-OTH-003 into REG-001, UNB-XIN-REG-003 into CRM-003, UNB-XIN-ECO-019 into REQ-X-042;
   - not a platform requirement: UNB-XIN-ACC-009.
3. **Legal corrections.**
   - Rev. Proc. 2017-53 is now cited as §3.03(1) in XRA-001, UNB-XIN-NPG-001, RR-UNB-GRANT-041-r1, the related findings and the atlas.
   - XRA-020 now lists financial services, and its confidence is lowered to low.
4. **Arithmetic and claim corrections.**
   - Report 11: unused capacity is about 4.9× the operating result, not 3×. The $12.70-per-grant figure is withdrawn, and the unit-cost ranges are marked illustrative only.
   - Report 12: outbox and accrual volumes restated; "one transactional boundary" narrowed to *within a sponsor*, with the hold-registry exception; self-scoring disclosed; EVA-001 scoped to the functions where Unbound is the book of record.
   - The industry reports eco.md and eva.md carry integrator notes pointing to these corrections.
5. **New requirements** (both P1; each is tested by a scenario):

<!-- BEGIN GENERATED: adv-requirements -->
| ID | P | Statement | Business problem |
|---|---|---|---|
| UNB-XIN-ADV-001 | P1 | Every account type MUST be assigned exactly one ServicingClass (Standard or DigitalMinimal) by sponsor policy. Every requirement-driven recurring per-account obligation (attestations, recertifications, outreach letters, printed statements, manual reviews not triggered by a legal or risk rule, vendor-priced checks) MUST be registered in a RecurringObligation register stating trigger, frequency, unit cost estimate with grade, legal basis class (statutory/regulatory, fiduciary, contractual, sponsor policy, risk control, convention) and per-class applicability. For DigitalMinimal accounts, obliga… | Fully loaded cost is ~$244-272 per account-year in Unbound's own models while low-balance accounts generate a few dollars of fee revenue; per-account recurring obligations make high-volume-low-balance and workplace programs structurally loss-making. |
| UNB-XIN-ADV-002 | P1 | For a fund whose FundAttribution function is designated Shadow under UNB-XIN-ACC-010, grantable capacity MUST be computed as: the fund's available balance in the latest external snapshot accepted as complete under UNB-XIN-EVA-004 (with its as-of watermark) minus all Unbound reservations and released-unsettled grants committed after that watermark minus accrued-unposted fees reported by the external system. Reservations MUST be committed in Unbound's reservation table under the same locking rules as Authoritative funds. When the snapshot watermark is older than the sponsor-configured staleness… | Institutional and outsourced-admin sponsors with an incumbent fund accountant cannot authorize grants in Unbound without either migrating their book of record or using an ungoverned balance. |
<!-- END GENERATED: adv-requirements -->

## 3. Conflicts between research passes, found by the review

The integrator had already recorded XIC-INT-001..003. The review found these additional conflicts.

<!-- BEGIN GENERATED: adv-conflicts -->
| Conflict | A | B | Incompatibility | Recommended resolution |
|---|---|---|---|---|
| XIC-ADV-001 | UNB-XIN-ACC-010 | UNB-XIN-EVA-001 | ACC-010 (INV group) allows Shadow book of record for GeneralLedger/FundAttribution where Unbound MUST NOT post; EVA-001 (EVA group, P0) requires fund balances, reservations and capacity to be computed only from Unbound's journal and forbids any other balance log. In Shadow mode capacity has no perm… | Scope EVA-001 to functions designated Authoritative under ACC-010; adopt UNB-XIN-ADV-002 for Shadow-mode capacity (reconciled external balance at a watermark minus Unbound reservations since; stale watermark blocks capacity increases). XID-ADV-003. |
| XIC-ADV-002 | UNB-XIN-ACC-003 | UNB-XIN-EVA-001 | EVA-001 says the system MUST NOT maintain any other log from which balances can be derived or re-derived; ACC-003 requires a scheduled re-derivation of expected journals from source events. Domain facts carrying amounts (contributions, grants) necessarily allow re-derivation, so EVA-001's clause is… | Reword EVA-001: no other log may be treated as a source of balances or posted from on replay; re-derivation under ACC-003 is a comparison control whose output never posts. |
| XIC-ADV-003 | UNB-XIN-EVA-014 | UNB-XIN-SVC-004;UNB-XIN-IAM-003 | EVA-014 classes contact details as mutable, erasable engagement data never used to authorize money; SVC-004 and IAM-003 use registered devices and verified channels as possession factors for disclosure and money-direction changes. | Authenticator bindings are IAM credentials stored as authority facts (append-only, step-up to change, notice to prior channel); ordinary contact details stay engagement data. A contact-detail change never changes an authenticator implicitly. |
| XIC-ADV-004 | RR-ADM-023-r1 | UNB-XIN-EVA-012 | RR-ADM-023-r1 and XID-TRS-002 (ERP/TRS group) default co-advisor authority to AllRequired; XID-EVA-006 (EVA group) recommends Any-one for grants and All only for transfers-out, closure and allocation, and EVA-012 resolves conflicts by that policy. | One default table by instruction kind: grants below sponsor threshold Any-one (conflicts -> EVA-012 HeldConflict); grants above threshold, transfers-out, closure, successor and authority changes AllRequired; mode captured at fund opening and shown to all advisors. |
| XIC-ADV-005 | UNB-XIN-ACC-005 | UNB-XIN-EVA-002 | EVA-002 routes any posting dated in a hard-closed period to a PriorPeriodAdjustment in the first open period; ACC-005 posts audit findings into an AuditAdjustment period of the prior fiscal year. XID-EVA-002 is open. | AuditAdjustment period stays soft-closed until audit sign-off and accepts only AuditFinding-linked journals; after the audited statements are issued, EVA-002 PPA applies. Period 13 hard-closes at sign-off. |
| XIC-ADV-006 | UNB-XIN-ERP-003 | UNB-XIN-ECO-012 | ECO-012 derives RevenueRecords only from posted revenue including contract/SaaS fees; ERP-003 forbids posting operator charges to tenant ledgers. Profitability by business model mixes sponsor and operator P&Ls. | Split ECO-012 into sponsor-view (sponsor GL) and operator-view (operator billing system plus SAS-009 usage costs) margins; neither posts to tenant ledgers. |
| XIC-ADV-007 | UNB-XIN-IAM-002 | UNB-XIN-IAM-005 | IAM-002 permits operator access only under sponsor delegation or per-case sponsor approval and requires visibility within 15 minutes; IAM-005 defines break-glass without sponsor approval and tenant notice within 1 hour. | Add break-glass as the third enumerated basis in IAM-002, bounded by IAM-005; transparency view within 15 minutes for all operator sessions including break-glass, active notification within 1 hour. |
| XIC-ADV-008 | UNB-XIN-SAS-007 | UNB-XIN-EVA-011 | SAS-007 allows sponsor A to decide exit transfers by a sponsor-approved batch policy; EVA-011 forbids batches carrying approval authority. | A batch policy is a per-item decision rule: each transfer item is decided, hold-checked and committed individually with its own decision record citing the policy version. |
| XIC-ADV-009 | UNB-XIN-ERP-001 | XID-ERP-001;UNB-XIN-EVA-013 | ERP-001 (P0) mandates daily accrual per fund while the billing basis decision XID-ERP-001 is open; if accruals are posted daily (TX-20 as cited in the EVA consistency table) the EVA volume test that justifies EVA-013's no-broker threshold omits the largest row generator. | Accrue daily by computation; post at assessment; capacity = journal balance - reservations - accrued-unposted fees; restate EVA volume table. |
| XIC-ADV-010 | UNB-XIN-IAM-003 | UNB-XIN-IAM-001;UNB-XIN-SAS-006 | IAM-003 forbids delegating step-up to a partner IdP; IAM-001 defines a federated PartnerWorkforce realm and the advisor-distribution model relies on partner federation, so every advisor action above threshold requires a second sponsor-controlled login. | Permit partner-IdP step-up where the partner agreement asserts the required AAL, the assertion carries amr/acr evidence verified per command, and the sponsor approves the partner; sponsor-controlled verifier remains mandatory for payee and authority changes. |
| XIC-ADV-011 | UNB-XIN-ECO-013 | UNB-XIN-ECO-022;XRJ-ERP-002 | ECO's own report says simulation runs offline in Echelon first and release 1 is six metrics plus a pack; ECO-013 requires an in-platform ScenarioRun and ECO-022 ten dashboards including Scenario; XRJ-ERP-002 rejected native budgeting/forecasting. | Reject ECO-013 (XRJ-ADV-001); cut ECO-022 to the six R1 metrics and monthly pack. |
<!-- END GENERATED: adv-conflicts -->

## 4. Decisions the review raised

<!-- BEGIN GENERATED: adv-decisions -->
| Decision | Question | Recommendation |
|---|---|---|
| XID-ADV-001 | Which optional money-movement and legal features are in release 1 (card contributions, ACH debit intake, direct foreign grants, call recording, pooled investment vehicles, distribution partners, embedded checkout)? | Exclude card intake, call recording, direct foreign grants (use intermediaries) and partner exit tooling from R1; this removes ~8 conditional P0s. |
| XID-ADV-002 | Is high-volume-low-balance a release-1 business model, and if so what minimum economics (fee floor, servicing class, digital-only) are acceptable? | Not an R1 model unless UNB-XIN-ADV-001 is adopted and a fee floor passes the XID-ECO-002 reasonableness review. |
| XID-ADV-003 | How is grantable capacity computed when the general ledger or fund attribution is Shadow (external book of record)? | Adopt UNB-XIN-ADV-002 and scope EVA-001 to Authoritative functions. |
| XID-ADV-004 | Default co-advisor authority mode by instruction kind (resolves XID-TRS-002 vs XID-EVA-006). | Any-one for grants below threshold; AllRequired for large grants, transfers-out, closure, successor and authority changes. |
| XID-ADV-005 | Routing of prior-year adjustments discovered during and after the audit (AuditAdjustment period vs PriorPeriodAdjustment). | AuditAdjustment period until audit sign-off; PPA thereafter. |
| XID-ADV-006 | Which GrantRecognitionPoint values are permitted? | Permit ApprovalCommunicatedToGrantee; SponsorApproval only where CPA documents contemporaneous communication; PaymentReleased only where payment is the first communication. |
| XID-ADV-007 | Are fund-level fee accruals and income attributions posted daily or computed daily and posted at assessment? | Compute daily, post at assessment; capacity subtracts accrued-unposted. |
| XID-ADV-008 | Are authenticator bindings (verified phone/email for OTP, registered devices) authority facts or engagement data? | Authority facts owned by IAM; changes need step-up on an existing factor and notice to prior channel. |
| XID-ADV-009 | What authentication strength applies to donors and advisors for grant recommendations, and may a partner IdP perform step-up? | AAL2 (any authenticator) for grant recommendations; phishing-resistant for payee/authority changes and workforce; partner IdP step-up allowed by contract with verified acr/amr. |
| XID-ADV-010 | Who provides counsel/CPA-approved default configuration packs (rail obligations, unclaimed property, decline reasons, assurance matrix, recognition point, discretionary policy), and can a sponsor adopt them by attestation? | Platform maintains reviewed default packs; sponsors attest adoption and may only tighten; deviations need sponsor counsel. |
| XID-ADV-011 | Release-1 scope of the economics capability. | Six metrics, monthly pack, AI metering and non-posting attribution with GL reconciliation; everything else P3 or Echelon offline. |
<!-- END GENERATED: adv-decisions -->

## 5. Findings

<!-- BEGIN GENERATED: adv-findings -->
| Finding | Severity | Title | Affected | Proposed resolution |
|---|---|---|---|---|
| XIF-ADV-001 | critical | High-volume-low-balance model cannot be served profitably under the proposed requirement set | UNB-XIN-ERP-001;UNB-XIN-CRM-002;UNB-XIN-IAM-004;UNB-XIN-SAS-009;UNB-XIN-WFA-001;UNB-RCN-ENT-011;REQ-X-070 | Adopt UNB-XIN-ADV-001 (account servicing class with per-account recurring-obligation budget); make CRM-002 attestation and IAM-004 customer recertification event-driven or risk-banded rather than annual-for-all; compute fee accrual daily but post at assessmen… |
| XIF-ADV-002 | high | Institutional/outsourced sponsors in Shadow book-of-record mode have no lawful source of grantable capacity | UNB-XIN-ACC-010;UNB-XIN-EVA-001;UNB-XIN-EVA-004 | Adopt UNB-XIN-ADV-002 (Shadow-mode capacity = last reconciled external available balance at a watermark minus Unbound reservations since; stale watermark blocks capacity increases); amend EVA-001 to apply to functions designated Authoritative. |
| XIF-ADV-003 | high | Priority inflation: 40 new P0 plus 11 P0 refinements is not a credible release-1 set | UNB-XIN-BNK-001;UNB-XIN-PAY-003;UNB-XIN-CLM-004;UNB-XIN-ERP-001;UNB-XIN-ERP-004;UNB-XIN-CRM-001;UNB-XIN-CRM-002;UNB-XIN-CRM-003;UNB-XIN-REG-002;UNB-XIN-SAS-005;UNB-XIN-SAS-007;UNB-XIN-REC-001;UNB-XIN-ECO-021;UNB-XIN-EVA-014;UNB-XIN-OTH-003;UNB-XIN-ACC-002 | Apply priority-challenges.jsonl: 16 downgrades from P0 (24 remain), 20 P1->P2, 10 P2->P3; tag PAY-001/002, NPG-001/010, SVC-003, INV-002 as P0-conditional on the R1 scope decision XID-ADV-001. |
| XIF-ADV-004 | high | Configuration and approval load makes sponsor onboarding infeasible without platform default packs | UNB-XIN-BNK-001;UNB-XIN-BNK-004;UNB-XIN-BNK-007;UNB-XIN-ACC-001;UNB-XIN-ACC-002;UNB-XIN-ACC-010;UNB-XIN-INV-002;UNB-XIN-SVC-004;UNB-XIN-SVC-007;UNB-XIN-CLM-002;UNB-XIN-NPG-005;UNB-XIN-NPG-006;UNB-XIN-REG-001;UNB-XIN-REG-002;UNB-XIN-SAS-001;UNB-XIN-ERP-005;UNB… | Require platform-maintained, counsel/CPA-reviewed default packs that a sponsor adopts by attestation and may only tighten; per-sponsor counsel approval only for deviations. Record as XID-ADV-010. |
| XIF-ADV-005 | high | Economics stack is over-engineered and internally inconsistent with its own release-1 scope | UNB-XIN-ECO-001;UNB-XIN-ECO-002;UNB-XIN-ECO-003;UNB-XIN-ECO-004;UNB-XIN-ECO-005;UNB-XIN-ECO-006;UNB-XIN-ECO-007;UNB-XIN-ECO-008;UNB-XIN-ECO-009;UNB-XIN-ECO-010;UNB-XIN-ECO-011;UNB-XIN-ECO-012;UNB-XIN-ECO-013;UNB-XIN-ECO-014;UNB-XIN-ECO-015;UNB-XIN-ECO-016;UNB… | Collapse to five requirements: metering feed (merge ECO-005, ECO-009, SAS-009, BNK-005), non-posting attribution with GL reconciliation (merge ECO-001..004, P2), capacity forecast (ECO-017 absorbing SVC-008, P2), counter-metric floors (ECO-016, P2), six R1 me… |
| XIF-ADV-006 | medium | EVA volume test omits the dominant row generator (daily per-fund accruals) and understates outbox load | UNB-XIN-ERP-001;UNB-XIN-INV-006;UNB-XIN-EVA-013 | Accrue by computation daily and post once per assessment period (capacity formula subtracts accrued-unposted fees); restate the EVA volume table including accruals and income attribution; correct outbox msg/s. |
| XIF-ADV-007 | medium | ADR-EVA-001 claims one transactional boundary for every invariant, but hold-vs-release spans two databases | UNB-XIN-EVA-001;UNB-XIN-EVA-011;UNB-XIN-EVA-013;RR-REQ-X-020-r1 | Correct the ADR text; add an acceptance criterion to EVA-011 for registry unavailability (fail closed, with bounded-staleness cache only for sanctions-neutral holds if counsel approves); have the alternative scoring re-done by a non-proposing reviewer before… |
| XIF-ADV-008 | medium | ECO worked example contains an arithmetic misstatement and non-reproducible unit costs; ECO-004 identity is tautological | UNB-XIN-ECO-004;UNB-XIN-ECO-003;UNB-XIN-ECO-012 | Fix the ratio text; publish the grant count and component breakdown for each unit cost; redefine ECO-004 controls as (a) residual share by reason within tolerance, (b) independent recomputation reproduces totals; define tolerance basis (pool vs total GL) and… |
| XIF-ADV-009 | medium | Rev. Proc. 2017-53 currency rule mis-cited as §4.02 in XRA-001 and UNB-XIN-NPG-001 | UNB-XIN-NPG-001 | Correct the citations to §3.03(1); point XID-NPG-001 at §3.03(5)(c). |
| XIF-ADV-010 | medium | XRA-020 misstates the Colorado SB 26-189 consequential-decision domains | REQ-X-017 | Correct XRA-020's domain list; route the financial-services question to counsel before 2027-01-01; confidence medium (bill text not read). |
| XIF-ADV-011 | medium | GrantRecognitionPoint options include points GAAP does not support as general policies | UNB-XIN-ACC-002 | Restrict to ApprovalCommunicatedToGrantee, with SponsorApproval permitted only where the CPA documents that approval and communication are contemporaneous in that program; drop PaymentReleased except where payment is the first communication. Lower ACC-002 to… |
| XIF-ADV-012 | high | Two agents specify opposite defaults for multi-advisor instruction authority | RR-ADM-023-r1;UNB-XIN-EVA-012;AUTH-I4 | Recorded as XIC-ADV-004; recommend Any-one for grant recommendations below a sponsor threshold, AllRequired for transfers-out, closure, successor and authority changes, with mode captured at fund opening. |
| XIF-ADV-013 | high | Contact details classed as mutable engagement data are used as authentication factors | UNB-XIN-EVA-014;UNB-XIN-SVC-004;UNB-XIN-IAM-003;UNB-XIN-CRM-001 | Recorded as XIC-ADV-003: authenticator bindings (registered device, verified OTP channel) are credentials owned by IAM as authority facts; changing them requires IAM-003 step-up on an existing factor plus out-of-band notice to the old channel; marketing conta… |
| XIF-ADV-014 | medium | Late-found prior-year errors have two contradictory routing rules | UNB-XIN-ACC-005;UNB-XIN-EVA-002 | Recorded as XIC-ADV-005: before audited statements are issued, audit findings post to the AuditAdjustment period (which is not hard-closed until sign-off); after issuance, EVA-002 PPA applies; period 13 hard-closes at audit sign-off. |
| XIF-ADV-015 | medium | Profitability requirement depends on operator revenue that another requirement forbids posting | UNB-XIN-ERP-003;UNB-XIN-ECO-012;UNB-XIN-SAS-009 | Recorded as XIC-ADV-006: split ECO-012 into sponsor-view margin (sponsor GL) and operator-view margin (operator billing system + UsageRecord costs via SAS-009, outside tenant ledgers). |
| XIF-ADV-016 | medium | Operator-access rule has no break-glass exception and different visibility deadlines | UNB-XIN-IAM-002;UNB-XIN-IAM-005 | Recorded as XIC-ADV-007: add break-glass as an enumerated third basis in IAM-002, with the IAM-005 limits; one visibility deadline (15 minutes for the transparency view, 1 hour for active notification). |
| XIF-ADV-017 | medium | Partner exit allows sponsor decision by batch policy while batch authority is prohibited | UNB-XIN-SAS-007;UNB-XIN-EVA-011;UNB-XIN-EVA-009 | Recorded as XIC-ADV-008: a batch policy is a decision rule evaluated per item (each item its own transaction, hold check and decision record); it never approves the batch. |
| XIF-ADV-018 | medium | Customer friction: authentication and attestation rules sized for workforce applied to donors and advisors | UNB-XIN-IAM-003;UNB-XIN-IAM-004;UNB-XIN-SVC-004;UNB-XIN-NPG-009;UNB-XIN-CRM-002 | Phishing-resistant authenticators required for workforce and for payee/authority changes only, AAL2 (any) for grant recommendations; allow partner IdP step-up where the partner contract carries assurance obligations and the sponsor approves; customer-role rec… |
| XIF-ADV-019 | medium | Requirements prejudge decisions that are still open | UNB-XIN-ERP-001;UNB-XIN-ACC-002;UNB-XIN-EVA-002;UNB-XIN-EVA-007;UNB-XIN-EVA-001 | Mark these requirements 'blocked-by <XID>' and exclude them from the P0 baseline until the decision closes. |
| XIF-ADV-020 | medium | Integration-dependent P0 controls rest on feed specifications nobody has obtained | UNB-XIN-EVA-004;UNB-XIN-PAY-002;UNB-XIN-BNK-003;UNB-XIN-BNK-006;UNB-XIN-EVA-005;UNB-XIN-INV-001;UNB-XIN-OTH-004 | Make partner selection gated on a feed conformance test (RR-UNB-PAY-008-r1 RailGoLive gate extended to custodians); obtain two real bank and custodian specs before freezing EVA-005 keys. |
| XIF-ADV-021 | low | Legal spot-check: 11 claims checked, 9 substantively correct, 2 with errors | UNB-XIN-PAY-005;UNB-XIN-BNK-002;UNB-XIN-CRM-003;UNB-XIN-NPG-007;UNB-XIN-REG-002 | Fix the two errors; keep counsel questions XID-REG-001/-002 and XID-SAS-005 as R1 blockers for embedded, white-label and investment-centric models. |
| XIF-ADV-022 | medium | Business-model coverage claims are thin for HVLB, HNW and embedded | UNB-XIN-SAS-009;UNB-XIN-WFA-001;UNB-XIN-IAM-002;UNB-XIN-IAM-003 | Rate each model served / served-with-blocker / not served in R1 (XID-ADV-002); do not market blocked models until their decisions close. |
<!-- END GENERATED: adv-findings -->

## 6. Priority challenges (all applied)

<!-- BEGIN GENERATED: priority-challenges -->
| Requirement | Current | Proposed | Reason |
|---|---|---|---|
| UNB-XIN-BNK-001 | P0 | P1 | A register is documentation; enforcement is the RailGoLive gate in RR-UNB-PAY-008-r1, which is already P0. |
| UNB-XIN-PAY-003 | P0 | P1 | Card disputes are run by the processor; ledger exposure is already handled by RR-REQ-X-009-r1 exposure recording; only relevant if cards are in R1. |
| UNB-XIN-CLM-004 | P0 | P1 | Restricted visibility for investigation cases is good practice, but existing role scoping prevents disclosure; not itself loss-preventing. |
| UNB-XIN-ERP-001 | P0 | P1 | Accrual method is an open decision (XID-ERP-001); monthly accrual is also correct; integrity is ensured by RR-ADM-028-r1 idempotent fee runs. |
| UNB-XIN-ERP-004 | P0 | P1 | Fraud-reduction separation of payee registries; grant payees are already verified destinations under existing payment controls. |
| UNB-XIN-CRM-001 | P0 | P1 | Data-partitioning design preference; disclosure risk is controlled by role scoping and EVA-014. |
| UNB-XIN-CRM-002 | P0 | P1 | No statute requires attestations; detection of §4958/§4967 risk is valuable but declarations are low-reliability; make event-driven, not annual. |
| UNB-XIN-CRM-003 | P0 | P1 | CAN-SPAM email violations carry low per-message harm; keep P0 only for SMS/prerecorded voice (TCPA statutory damages), per XIF-INT-001. |
| UNB-XIN-REG-002 | P0 | P1 | A register of open legal questions is a governance artifact; the gating effect belongs to REG-001 and launch checklists. |
| UNB-XIN-SAS-005 | P0 | P1 | With one database per legal sponsor, isolation is physical; the envelope aids audit and metering but is not the isolation boundary. |
| UNB-XIN-SAS-007 | P0 | P1 | No distribution partners in R1 single-tenant launch; P0 once a partner contract exists. |
| UNB-XIN-REC-001 | P0 | P1 | Legal holds are required, but automated multi-processor notice with 5-day escalation is an operational enhancement over a manual counsel notice. |
| UNB-XIN-ECO-021 | P0 | P2 | Management cost dashboards; cross-tenant leakage is already prevented by tenancy controls; benchmark k-anonymity is a P2 reporting rule. |
| UNB-XIN-EVA-014 | P0 | P1 | Classification of data classes is architecture guidance; the P0 effect (CRM data never authorizes money) is already in RR-UNB-EVT-005-r1 and EVA-016. |
| UNB-XIN-ACC-002 | P0 | P1 | Recognition timing is corrected at audit (ACC-005) and the policy choice is open (XID-INV-001). |
| UNB-XIN-OTH-003 | P0 | merge into UNB-XIN-REG-001 (P0) | Duplicate of REG-001 funds-flow rule (XIF-INT-007); keep one P0 requirement. |
| UNB-XIN-SAS-001 | P1 | P2 | Entitlements matter once there are multiple contracted tenants with different scopes; R1 can run on configuration. |
| UNB-XIN-SAS-002 | P1 | P2 | Flag hygiene is engineering practice; the semantic prohibition can be a code-review rule. |
| UNB-XIN-SAS-003 | P1 | P2 | Ring deployment matters with many cells; R1 has few cells; keep the December freeze as a P1 runbook item. |
| UNB-XIN-SAS-010 | P1 | P2 | Sandboxes are a commercial convenience; mock adapters already exist for testing. |
| UNB-XIN-ECO-001 | P1 | P2 | Management attribution is not an essential customer obligation. |
| UNB-XIN-ECO-002 | P1 | P2 | Same as ECO-001. |
| UNB-XIN-ECO-003 | P1 | P2 | Same as ECO-001. |
| UNB-XIN-ECO-004 | P1 | P2 | Same as ECO-001; control is also tautological as written (XIF-ADV-008). |
| UNB-XIN-ECO-016 | P1 | P2 | Only meaningful once improvement initiatives exist. |
| UNB-XIN-ECO-017 | P1 | P2 | Staffing forecast can live in a spreadsheet fed by queue exports for R1; capacity is decided by humans anyway. |
| UNB-XIN-ACC-003 | P1 | P2 | Comparison control valuable but sample-based re-derivation can follow go-live. |
| UNB-XIN-ACC-004 | P1 | P2 | Accounting change classification is handled by the controller manually at low frequency. |
| UNB-XIN-INV-001 | P1 | P2 | Advisory fee recomputation is an integration reconciliation only for advisor-managed accounts. |
| UNB-XIN-EVA-008 | P1 | P2 | Published integration events only needed for partner/embedded models. |
| UNB-XIN-ERP-003 | P1 | P2 | Operator billing in the operator system is a boundary statement; nothing is built in R1. |
| UNB-XIN-CRM-004 | P1 | P2 | Segmentation allowlist matters once marketing segments exist. |
| UNB-XIN-NPG-009 | P1 | P2 | Grantee declarations add friction with low detection value; only for direct foreign grants (possibly out of R1). |
| UNB-XIN-REC-002 | P1 | P2 | Legal production can be assembled manually in R1. |
| UNB-XIN-SVC-001 | P1 | P2 | Logging every interaction as a Contact is CX instrumentation, not an obligation. |
| UNB-XIN-ACC-009 | P1 | P2 | SOC 1 is a commercial/vendor-assurance commitment of the operator, not platform behaviour; track under RR-REQ-X-073-r1. |
| UNB-XIN-ECO-008 | P2 | P3 | Cost quotes advisory only. |
| UNB-XIN-ECO-010 | P2 | P3 | Weekly process mining before measured data exists. |
| UNB-XIN-ECO-015 | P2 | P3 | Improvement programme governance; merge into REQ-X-042. |
| UNB-XIN-ECO-019 | P2 | P3 | Automation ROI; duplicate of ECO-015 verification. |
| UNB-XIN-ECO-022 | P2 | P3 | Ten dashboards versus the report's own six-metric R1 scope. |
| UNB-XIN-ECO-018 | P2 | P3 | Recovery ratio only once service charges exist. |
| UNB-XIN-INV-007 | P2 | P3 | Crypto is normally liquidated on receipt; holding crypto is an optional sponsor policy. |
| UNB-XIN-ERP-006 | P2 | P3 | Duplicate effort tagging of ECO-006 (XIF-INT-008). |
| UNB-XIN-BNK-005 | P2 | P3 | Duplicate cost feed (XIF-INT-008). |
| UNB-XIN-SVC-008 | P2 | P3 | Merged into ECO-017 (XIF-INT-004). |
| UNB-XIN-ECO-013 | P3 | reject | Contradicts ECO report (Echelon offline first) and XRJ-ERP-002. |
| UNB-XIN-ECO-020 | P3 | reject | SLA-tier lifetime value is a commercial analysis, not platform behaviour. |
<!-- END GENERATED: priority-challenges -->

## 7. What the review did not change, and why

- **Event architecture.** The ADR's direction holds: no broker, and the journal is the financial log. The review corrected the evidence behind it, not the decision.
- **AI boundaries.** The AI boundaries were not loosened. The review's point that labour stays high is answered by default packs and fewer approvals, not by giving AI discretion. Mission §10 forbids AI from exercising legal discretion reserved for the sponsor.
- **Open legal questions.** These stay open. The review spot-checked claims; it did not substitute for counsel.

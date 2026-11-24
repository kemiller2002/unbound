# 04: Requirements refinement records (deliverable 4)

Each refinement is a concrete, testable restatement of an original inbound requirement. The original is never edited. Each record carries:

- the original ID, path and line;
- the baseline revision (`777a70d`);
- the **exact original text**, which gate G4 checks verbatim against the file at that revision. For CSV-defined originals, G4 checks against the row's cells joined with ` | `;
- the relationship, either `refines` or `supersedes`.

Refinement numbering continues after the reconciliation's records. For example, `RR-REQ-X-048-r2` follows `RR-REQ-X-048-r1`, which was itself contradicted by this mission's service findings (XIC-SVC-001).

**Adoption.** Each record has a `decision_status`:

- `autonomous-reversible` records may be adopted by the requirements owner through RCD-001.
- `owner-decision` and `counsel-review` records wait for the linked decision.

## Index

<!-- BEGIN GENERATED: refinements -->
| Refinement | Original | Relationship | Original path:line | Reason for change | Decision status |
|---|---|---|---|---|---|
| RR-TVR-CON-001-r1 | TVR-CON-001 | refines | research/daf-theoretical-enterprise-validation/16-registers/requirements.csv:10 | TVR-CON-001 cites only Nacha R10; reviewers noted R29 and credits differ; cards unmodeled. | owner-decision |
| RR-UNB-PAY-008-r1 | UNB-PAY-008 | refines | requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md:14 | Original lists categories only; binding duties exist from first live transaction. | autonomous-reversible |
| RR-UNB-GAP-013-r1 | UNB-GAP-013 | refines | requirements/ENTERPRISE-GAP-REGISTER-AND-CROSS-DOMAIN-SCENARIOS.md:25 | Original is not testable. | autonomous-reversible |
| RR-ACC-MDL-007-r1 | ACC-MDL-007 | supersedes | requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md:15 | Including the rule version in the idempotency key lets a rule change or rebuild post a second journal for the same event and rewrite tested history. | autonomous-reversible |
| RR-UNB-ACC-033-r1 | UNB-ACC-033 | refines | requirements/FULL-ACCOUNTING-SYSTEM.md:42 | UNB-ACC-033 states the outcome (do not conflate) without the evidence that decides classification. | owner-decision |
| RR-UNB-ACC-031-r1 | UNB-ACC-031 | supersedes | requirements/FULL-ACCOUNTING-SYSTEM.md:40 | 'Where relevant' is untestable; a tax-exempt holder needs lots for disposition reporting, §4943 and UBTI, not for capital-gains tax. | owner-decision |
| RR-UNB-INV-019-r1 | UNB-INV-019 | supersedes | requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md:28 | Platform operator is not within the charitable-organization adviser exemption; unconstrained AI portfolio recommendations may constitute investment advice. | counsel-review |
| RR-REQ-X-048-r2 | REQ-X-048 | refines | research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:440 | r1 restricts creation to four system causes; customer- and counterpart-initiated requests that need a human response have no cause (XIF-SVC-002). | autonomous-reversible |
| RR-ADM-033-r1 | ADM-033 | refines | requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:43 | Vague: no clock start/pause/stop semantics, no decision-time targets (XIF-CLM-003). | owner-decision |
| RR-ADM-042-r1 | ADM-042 | refines | requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:52 | Untestable: no formulas, denominators or windows (XIF-SVC-006, XIF-CLM-011). | autonomous-reversible |
| RR-CTL-040-r1 | CTL-040 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-04.md:121 | Control lacks sample design, rubric versioning and AI role (XIF-SVC-005). | autonomous-reversible |
| RR-ADM-023-r1 | ADM-023 | refines | requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:33 | Defines what a conflict is and a fixed precedence order so outcomes are deterministic. | owner-decision |
| RR-ADM-028-r1 | ADM-028 | refines | requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:38 | Makes fee administration testable and idempotent. | autonomous-reversible |
| RR-REQ-X-073-r1 | REQ-X-073 | refines | research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:588 | Replaces boilerplate with freshness and CUEC rules. | autonomous-reversible |
| RR-ADM-008-r1 | ADM-008 | refines | requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:18 | States explicitly what a household may not do, closing CRM-style aggregation. | autonomous-reversible |
| RR-UNB-GRANT-024-r1 | UNB-GRANT-024 | refines | requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:39 | Original is a capability list without conditions; agreements were neither required where law demands nor limited where unnecessary. | counsel-review |
| RR-UNB-GRANT-026-r1 | UNB-GRANT-026 | refines | requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:41 | Original bundles four controls of different necessity; acknowledgment chasing has no legal function for the sponsor. | autonomous-reversible |
| RR-UNB-GRANT-041-r1 | UNB-GRANT-041 | refines | requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:56 | Original 'as applicable' is untestable and omits the intermediary route and ED currency. | counsel-review |
| RR-UNB-RES-003-r1 | UNB-RES-003 | refines | requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:45 | Per-capability RTO/RPO is untestable and inconsistent (TVS-607: stores restored to different points); objectives belong to data classes that span capabilities. | owner-decision |
| RR-UNB-TEN-011-r1 | UNB-TEN-011 | refines | requirements/PARTNER-SEGREGATION-AND-TENANCY.md:20 | UNB-TEN-011 requires a governed plan but no states or gates; onboarding, suspension and exit are described in three documents (REQ-X-064, REQ-X-071, REQ-X-061) without a shared lifecycle. | autonomous-reversible |
| RR-UNB-TEN-016-r1 | UNB-TEN-016 | refines | requirements/PARTNER-SEGREGATION-AND-TENANCY.md:25 | UNB-TEN-016 is not testable ('evidence-complete', 'portability protocol' undefined) and conflicts with append-only storage until RCC-ENT-005's resolution is written as a requirement; it also names the partner, not the legal sponsor, as recipient. | counsel-review |
| RR-REQ-X-038-r1 | REQ-X-038 | refines | research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:374 | REQ-X-038 and RR-REQ-X-036-r1 ('Expiry MUST create a disposition review task') imply per-record human work, contradicting RCD-007's class-level batch recommendation and RCF-PLT-014; defensible disposition rests on consistent schedule application, not per-item… | owner-decision |
| RR-UNB-API-STD-003-r1 | UNB-API-STD-003 | refines | requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:18 | UNB-API-STD-003 lists topics (deprecation policy, rate limits) without values or obligations, so partner contracts cannot rely on it. | autonomous-reversible |
| RR-UNB-ACC-014-r1 | UNB-ACC-014 | refines | requirements/FULL-ACCOUNTING-SYSTEM.md:23 | Original groups posting-grade accounting with management allocation and does not prevent allocations from posting or being silently spread. | owner-decision |
| RR-AIN-028-r1 | AIN-028 | refines | requirements/ai/AI-NATIVE-INTERACTION-AND-AGENT-COLLABORATION.md:38 | "Without silently abandoning work" is untestable: no exhaustion state, routing or attribution is defined. | autonomous-reversible |
| RR-UNB-ENT-003-r1 | UNB-ENT-003 | refines | requirements/BASELINE-001.md:7 | "Sensitivity-tested unit economics" is satisfied by a one-off model; nothing requires measured inputs or refresh. | owner-decision |
| RR-UNB-ECO-001-r1 | UNB-ECO-001 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:53 | Drivers listed for planning; no runtime measurement or link to cost attribution; rail and exception-type granularity missing though they drive cost by orders of magnitude. | autonomous-reversible |
| RR-UNB-EVT-001-r1 | UNB-EVT-001 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:36 | The existing catalog fields do not say whether an event may be acted upon or whether it is the truth, so informational and AI events can gate money. | autonomous-reversible |
| RR-UNB-EVT-005-r1 | UNB-EVT-005 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:40 | UNB-EVT-005 states the outcome without an enforcement mechanism; type and privilege separation make the prohibition structural. | autonomous-reversible |
| RR-UNB-DATA-004-r1 | UNB-DATA-004 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:14 | Applying projection-of-immutable-records to every record class forces event sourcing where no replay need exists, conflicts with erasure and with the outbox. | owner-decision |
| RR-UNB-DATA-005-r1 | UNB-DATA-005 | refines | requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:15 | UNB-DATA-005 states that effects on derived records are preserved but not how they are found or completed. | autonomous-reversible |
| RR-REQ-X-069-r1 | REQ-X-069 | refines | research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:572 | REQ-X-069 extends monitoring generally; the statute excepts public-charity distributions regardless of purpose, so universal monitoring is unnecessary work. | owner-decision |
<!-- END GENERATED: refinements -->

## Records

<!-- BEGIN GENERATED: refinement-detail -->
### RR-TVR-CON-001-r1 (refines TVR-CON-001)

- **Original** (`research/daf-theoretical-enterprise-validation/16-registers/requirements.csv:10` @ `777a70d`): TVR-CON-001 \| Reversible-rail contributions are Provisional until the return window passes \| Contributions cannot fund grants before finality beyond limit \| TVF-006;Nacha R10 [V] \| Contributions \| Contribution \| Observed \| CON-1;FI-22 \| CW-07 \| P1;P2 \| 'Available for grants on D' \| Debit authorization \| Capacity excludes provisional \| Acknowledgment timing [C] \| TVS-091 \| Capacity calculation shows provisional component \| OD-04 \| TVC-023 \| Ack timing \| Future test with return fixtures \| proposed-theoretical
- **Refined statement:** Contribution provisional status MUST be governed by a versioned ReversalWindow table keyed by (rail, SEC code or card network, account type consumer/business, direction push/pull) with fields availability delay, basis (settlement date), source citation and owner. Contributions MUST be recognized as revenue when funds settle to the sponsor; reversibility affects grantable availability, not recognition. Default availability: ACH consumer debit 5 banking days after settlement; ACH business (CCD) debit 3 banking days; card on settlement up to the donor's provisional limit, remainder 60 calendar days after settlement; wires, donor push credits and cleared checks on settlement. The per-donor provisional limit defaults to $0 for donors with less than 90 days of history and $10,000 thereafter (illustrative, sponsor-configurable). A return or chargeback after acceptance MUST reverse contribution revenue against the fund up to the fund's available balance; any shortfall MUST post as sponsor operating loss and MUST NOT create a receivable from the donor. A missing table row MUST fail closed as Provisional.
- **Reason:** TVR-CON-001 cites only Nacha R10; reviewers noted R29 and credits differ; cards unmodeled.
- **Decision status:** owner-decision · **Confidence:** high · **Findings:** XIF-BNK-005, XIF-PAY-004
- **Acceptance criteria:**
  - **AC-1** Given a $5,000 CCD debit from a business account; when 3 banking days pass with no return; then the contribution becomes Final and fully grantable.
  - **AC-2** Given a $5,000 PPD debit from a consumer account and OD-04 limit 0; when day 30; then the value remains Provisional and not grantable.
  - **AC-3** Given a $5,000 wire contribution; when it is received; then it is Final at receipt.
  - **AC-D1** Given a $2,000 card gift from a donor with 30 days of history; when it settles; then revenue of $2,000 is recognized and $0 is grantable until 60 days after settlement.
  - **AC-D2** Given a $5,000 ACH gift fully granted out, then an R10 return 40 days later on a fund with $1,000 available; when the return posts; then contribution revenue is reversed by $5,000, the fund is reduced by $1,000 and $4,000 posts as sponsor operating loss; no donor receivable exists.

### RR-UNB-PAY-008-r1 (refines UNB-PAY-008)

- **Original** (`requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md:14` @ `777a70d`): - **UNB-PAY-008:** Payment provider selection, sponsor bank relationships, funds-flow legal review, rail eligibility, limits, fraud operations, security certification and production authorization are explicit future decision gates.
- **Refined statement:** Production enablement of any rail for any sponsor MUST pass a RailGoLive gate that requires: (a) all binding RailObligation rows for that rail and sponsor role mapped to implemented, tested controls (UNB-XIN-BNK-001); (b) provider selection evidence of idempotent submission and status query (TVR-PAY-002); (c) documented fraud-monitoring program for ACH origination (UNB-XIN-BNK-002); (d) debit authorization and account validation controls where debits are accepted (UNB-XIN-PAY-001, UNB-XIN-PAY-002); (e) daily unmatched-debit control for linked accounts (UNB-XIN-BNK-003); (f) funds-flow legal review recording Originator of record and whether any vendor holds funds; and (g) sponsor approval of limits and rail eligibility (UNB-XIN-BNK-004). Items not listed remain future decisions.
- **Reason:** Original lists categories only; binding duties exist from first live transaction.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-BNK-010, XIF-BNK-001
- **Acceptance criteria:**
  - **AC-1** Given a provider selected but no fraud-monitoring program; when go-live is requested; then the gate fails listing item (c).
  - **AC-2** Given all items evidenced; when go-live is requested; then the gate passes and records evidence references.

### RR-UNB-GAP-013-r1 (refines UNB-GAP-013)

- **Original** (`requirements/ENTERPRISE-GAP-REGISTER-AND-CROSS-DOMAIN-SCENARIOS.md:25` @ `777a70d`): - **UNB-GAP-013:** Handle unidentified deposits, rejected gifts, returned assets, disputed ownership, incomplete transfers and fraud alerts.
- **Refined statement:** Every inbound receipt (cash or securities) that cannot be matched deterministically to one contribution intent MUST be held in an Unidentified suspense state with no fund credit, no capacity and no acknowledgment; suspense items MUST age through configured thresholds with outreach to the sending institution/remitter; resolution MUST be one of MatchedToIntent (with evidence), ReturnedToSender (pre-acceptance only, as ReturnOfUnacceptedProperty to the originating account), or counsel-approved alternative; AI may propose candidate matches only as A1 suggestions; any match where amount, reference and sender are not all exact MUST require human confirmation.
- **Reason:** Original is not testable.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-BNK-009
- **Acceptance criteria:**
  - **AC-1** Given a $10,000 wire with no reference; when received; then it is in Unidentified suspense, no fund credit, outreach task created.
  - **AC-2** Given an AI-suggested match with exact amount but different sender; when an operator confirms; then confirmation is recorded with evidence; without it no credit occurs.
  - **AC-3** Given suspense item unresolved at return threshold; when threshold passes; then a ReturnToSender proposal is created for human approval to the originating account only.

### RR-ACC-MDL-007-r1 (supersedes ACC-MDL-007)

- **Original** (`requirements/accounting/CANONICAL-CHART-AND-POSTING-RULES.md:15` @ `777a70d`): - ACC-MDL-007: Posting is idempotent per event, accounting book and posting-rule version, with explicit treatment of restatements; event replay must not create duplicate journals or external effects.
- **Refined statement:** Posting MUST be idempotent per (source event ID, accounting book): at most one original journal may exist for each pair, whatever the posting-rule version. A posting-rule version change MUST NOT re-post events already posted; its effect on posted history MUST be handled only by an AccountingChangeRecord (UNB-XIN-ACC-004) producing new adjusting journals. Event replay, outage recovery and projection rebuild MUST locate the existing journal by (source event ID, book) and MUST NOT create journals or external effects.
- **Reason:** Including the rule version in the idempotency key lets a rule change or rebuild post a second journal for the same event and rewrite tested history.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-ACC-001
- **Acceptance criteria:**
  - **AC-1** Given event E posted under rule v3; when rule v4 is deployed and E is replayed; then no new journal posts and the replay log references the existing journal.
  - **AC-2** Given event E; when two concurrent posting attempts; then exactly one journal exists for (E, book).

### RR-UNB-ACC-033-r1 (refines UNB-ACC-033)

- **Original** (`requirements/FULL-ACCOUNTING-SYSTEM.md:42` @ `777a70d`): - **UNB-ACC-033:** Support nonprofit accounting classifications and net assets with/without donor restrictions where applicable; avoid conflating donor-advised subfund designation with legally donor-restricted net assets.
- **Refined statement:** Each program document version and each fund-intake agreement (including legacy imports and sponsor-to-sponsor transfers-in) MUST record a CPA-reviewed VariancePowerEvidence (clause reference, effective date) or its absence. Net asset classification of contributions MUST be derived from it: with variance power and no donor-imposed purpose/time restriction -> without donor restrictions (DAF designation shown as an internal designation); with an explicit donor restriction -> with donor restrictions; without variance power and with a specified beneficiary -> evaluated under UNB-XIN-ACC-001 (agency/pass-through). A contribution whose agreement lacks recorded evidence MUST post to a suspense classification and open an exception before period close. Advisory privileges alone MUST NOT be treated as a donor restriction (XID-INV-002).
- **Reason:** UNB-ACC-033 states the outcome (do not conflate) without the evidence that decides classification.
- **Decision status:** owner-decision · **Confidence:** high · **Findings:** XIF-ACC-003
- **Acceptance criteria:**
  - **AC-1** Given a fund transferred in from another sponsor with a purpose restriction; when acceptance posts; then the contribution posts to with donor restrictions.
  - **AC-2** Given program document v2 with variance power clause 4.1; when a cash gift posts; then it posts without donor restrictions referencing clause 4.1.
  - **AC-D1** Given a DAF program version whose documents grant variance power and a contribution with advisory privileges only; when the contribution posts; then it is classified without donor restrictions and the DAF designation is shown only as an internal designation.

### RR-UNB-ACC-031-r1 (supersedes UNB-ACC-031)

- **Original** (`requirements/FULL-ACCOUNTING-SYSTEM.md:40` @ `777a70d`): - **UNB-ACC-031:** Support tax-lot and cost-basis tracking where relevant, with donor tax acknowledgment values distinguished from sponsor accounting basis, proceeds and valuation.
- **Refined statement:** Unbound MUST preserve gift-lot identity (contribution ID, quantity, receipt date, sponsor carrying amount at receipt) for every contributed security until disposition. Donor cost basis and holding period MUST be recorded only for lots classified with UBTIExposure other than None (UNB-XIN-ACC-008) or where counsel requires it. Lot relief on sale MUST be specific identification for lots linked to a Form8283Signed record or a §4943 business-enterprise holding, and MAY be pool-average otherwise. Sponsor GAAP gain/loss MUST be measured from the sponsor carrying amount, never from donor basis; donor acknowledgment value MUST remain a separate field.
- **Reason:** 'Where relevant' is untestable; a tax-exempt holder needs lots for disposition reporting, §4943 and UBTI, not for capital-gains tax.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-ACC-009, XIF-ACC-010
- **Acceptance criteria:**
  - **AC-1** Given a lot linked to Form8283Signed sold within the reporting window; when sale posts; then specific identification is used and a Form8282Obligation is created.
  - **AC-2** Given publicly traded shares with UBTIExposure=None; when accepted; then no donor basis is required.

### RR-UNB-INV-019-r1 (supersedes UNB-INV-019)

- **Original** (`requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md:28` @ `777a70d`): - **UNB-INV-019:** AI may recommend portfolios, risk checks, liquidity plans and rebalancing, but may not bypass sponsor approval, mandate boundaries, suitability review where applicable, or execution controls.
- **Refined statement:** AI or rule-based features MAY compute drift, risk metrics, liquidity forecasts and conformity to sponsor-approved models, and MAY propose actions that implement a sponsor-approved model or committee-approved parameter. They MUST NOT generate individualized securities selections or allocation advice for a donor, advisor or sponsor beyond approved models unless counsel has recorded the Advisers Act status of the party providing it (sponsor personnel acting under §203(b)(4), a registered adviser under mandate, or another basis). Every proposal MUST name the approved model/parameter it implements and MUST pass deterministic mandate and pre-trade checks before any human approval. The operator MUST NOT provide individualized investment recommendations, and operator compensation MUST NOT vary with assets or trades (XID-INV-004).
- **Reason:** Platform operator is not within the charitable-organization adviser exemption; unconstrained AI portfolio recommendations may constitute investment advice.
- **Decision status:** counsel-review · **Confidence:** medium · **Findings:** XIF-INV-001
- **Acceptance criteria:**
  - **AC-1** Given an AI proposal recommending a stock not in any approved model; when submitted; then it is rejected with NoApprovedModel.
  - **AC-2** Given a drift alert implementing model M v3; when submitted; then it proceeds to deterministic checks and human approval.
  - **AC-D1** Given an AI feature request for a donor-specific allocation not in an approved model; when it is processed; then it is refused and only approved-model options are shown.

### RR-REQ-X-048-r2 (refines REQ-X-048)

- **Original** (`research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:440` @ `777a70d`): ## REQ-X-048: Human work queues, round-robin routing and cradle-to-grave ownership (2026-11-23) **Proposed:** Provide shared sponsor-scoped human work orchestration for grant and charity vetting cases. Support deterministic round robin, sticky sponsor/fund/account ownership, cradle-to-grave case continuity, specialist/skills routing, capacity-weighted assignment, manual audited overrides and hybrid routing. Distinguish relationship owner, case owner, task assignee, independent decision authority and backup. Preserve eligibility, sponsor authorization, conflicts, segregation of duties, absence…
- **Refined statement:** A WorkItem MUST be created only when (a) a rule-defined hold, (b) missing or conflicting evidence, (c) a decision requiring human authority, (d) a provider Unknown requiring reconciliation, or (e) a request, complaint, dispute or reconsideration from an identified external party that requires a human response and was promoted from a Contact under UNB-XIN-SVC-001 prevents an outcome from completing. Each WorkItem MUST reference the blocked or requested outcome, the creating rule or Contact, and its exit condition. Assignment MUST follow the sponsor routing policy and MUST NOT confer decision authority. Items of causes (a)-(d) MUST auto-close when system events meet the exit condition where the case profile permits; items of cause (e) MUST NOT close until the outcome has been communicated to the requesting party and recorded.
- **Reason:** r1 restricts creation to four system causes; customer- and counterpart-initiated requests that need a human response have no cause (XIF-SVC-002).
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-SVC-002, XIF-SVC-004
- **Acceptance criteria:**
  - **AC-1** Given donor complaint promoted from Contact; when WorkItem created; then cause e, Contact link and exit condition recorded.
  - **AC-2** Given cause-e item with resolution recorded but no customer message; when close command; then rejected.

### RR-ADM-033-r1 (refines ADM-033)

- **Original** (`requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:43` @ `777a70d`): - **ADM-033 Sponsor service-level governance** (Operations): Defined SLAs, escalation, holidays, evidence and transparent overdue reporting.
- **Refined statement:** Every case profile and decision type MUST declare its clocks (Response, Resolution, Decision) with explicit start, stop and pause events, a sponsor business calendar and a target. A clock MAY pause only while an InformationRequest to the requesting party is open or the party has requested a delay. Each sponsor-facing decision type MUST have a Decision clock target, and the system MUST report on-time rate and p50/p90 elapsed decision time per type and quarter; publication to donors is a sponsor option.
- **Reason:** Vague: no clock start/pause/stop semantics, no decision-time targets (XIF-CLM-003).
- **Decision status:** owner-decision · **Confidence:** high · **Findings:** XIF-CLM-003, XIF-SVC-005
- **Acceptance criteria:**
  - **AC-1** Given grant review Decision target 5 business days; RFI open days 2-4; when decision on day 7; then elapsed 4 business days; on time.
  - **AC-2** Given sponsor holiday Dec 25; when clock computation; then Dec 25 excluded.

### RR-ADM-042-r1 (refines ADM-042)

- **Original** (`requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:52` @ `777a70d`): - **ADM-042 Service quality feedback** (Analytics): Track user effort, time to resolution, repeated contacts and actual outcome.
- **Refined statement:** Unbound MUST compute, per sponsor and month: contacts per 1,000 accounts by reason and channel; repeat-contact rate (same principal and reason within 7 calendar days); deflection as informational intents resolved without a human contact on the same reason within 7 days; time-to-resolution distribution and handoffs per closed case by case kind; human touches per outcome. Optional effort surveys MUST be reported with response rate. Individual handle time MUST NOT be used for ranking without complexity adjustment.
- **Reason:** Untestable: no formulas, denominators or windows (XIF-SVC-006, XIF-CLM-011).
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-SVC-006, XIF-CLM-011
- **Acceptance criteria:**
  - **AC-1** Given donor uses AI for status then calls next day about same grant; when metrics run; then AI contact not counted as deflected; repeat contact counted.
  - **AC-2** Given case passed grants->compliance->grants; when closed; then handoffs = 2.

### RR-CTL-040-r1 (refines CTL-040)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-04.md:121` @ `777a70d`): \| CTL-040 \| Service QA sampling and complaint taxonomy \| Head of Client and Partner Operations \| QA scores; inter-rater checks \|
- **Refined statement:** Service QA MUST sample closed Contacts monthly by stratified random selection over channel and answer source with a recorded seed, score them against a versioned rubric (factual, policy, disclosure, tax-advice boundary), have humans adjudicate every AI/human score disagreement, and measure inter-rater agreement quarterly on a double-scored subset.
- **Reason:** Control lacks sample design, rubric versioning and AI role (XIF-SVC-005).
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-SVC-005, XIF-CLM-006
- **Acceptance criteria:**
  - **AC-1** Given 1,200 contacts in month; when sampling; then sample reproduces from seed.
  - **AC-2** Given AI pre-score pass, human fail; when adjudication; then human result recorded; disagreement counted.

### RR-ADM-023-r1 (refines ADM-023)

- **Original** (`requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:33` @ `777a70d`): - **ADM-023 Disputes and conflicting instructions** (Compliance): Detect conflicting directions and hold consequential actions pending sponsor resolution.
- **Refined statement:** The system MUST classify instructions as conflicting when their scopes overlap and cannot both execute, and MUST resolve them by the precedence rules: explicit author-confirmed supersession for the same author; the fund's declared joint-authority mode (default AllRequired) for different holders; invalidity after verified death/incapacity; latest sponsor-approved plan version. Unresolvable conflicts MUST hold the consequential effect and open a sponsor case.
- **Reason:** Defines what a conflict is and a fixed precedence order so outcomes are deterministic.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-TRS-007
- **Acceptance criteria:**
  - **AC-1** Given Monthly standing $1k to X; author submits one-off $5k to Y exceeding capacity for next instance; when detection; then both held; author asked to confirm priority.
  - **AC-2** Given Two co-advisors, mode AllRequired, one approves one rejects; when decision; then not submitted; after window case opened.
  - **AC-3** Given Recommendation authored 2 days after verified death effective time; when evaluation; then rejected as AUTHOR_NOT_AUTHORIZED.

### RR-ADM-028-r1 (refines ADM-028)

- **Original** (`requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:38` @ `777a70d`): - **ADM-028 Fees and billing administration** (Finance): Review fee schedules, accruals, waivers, exceptions and financial posting.
- **Refined statement:** Fee administration MUST execute as an idempotent run per (fund, schedule, period, segment set) through the states Accrued, Assessed, Posted, and optionally Waived or Reversed; waivers MUST be approved schedule exceptions with expiry; corrections MUST be compensating entries; replaying a run MUST NOT post additional entries.
- **Reason:** Makes fee administration testable and idempotent.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-ERP-010, XIF-ERP-001
- **Acceptance criteria:**
  - **AC-1** Given A posted Q3 fee run; when the run is replayed; then no new entries.
  - **AC-2** Given A waiver without approver; when posting; then rejected.

### RR-REQ-X-073-r1 (refines REQ-X-073)

- **Original** (`research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:588` @ `777a70d`): ### REQ-X-073: Vendor assurance, SOC control ownership and evidence **Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-016. See the dedicated REQ-X-073 specification dated 2026-11-23. No implementation or tests claimed.
- **Refined statement:** Unbound MUST maintain a critical-vendor register recording assurance type, legal entity and report period; MUST require a bridge letter when the period end is more than 3 months old and mark assurance Expired after 12 months; and MUST map every complementary user entity control to an internal control owner before the vendor is marked Assured. AI may extract report metadata for human confirmation only.
- **Reason:** Replaces boilerplate with freshness and CUEC rules.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-ERP-009
- **Acceptance criteria:**
  - **AC-1** Given SOC 2 period ended 5 months ago, no bridge letter; when evaluation; then status BridgeRequired and work item.
  - **AC-2** Given One unmapped CUEC; when mark Assured; then rejected.

### RR-ADM-008-r1 (refines ADM-008)

- **Original** (`requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:18` @ `777a70d`): - **ADM-008 Household and relationship model** (DAF Admin): Household display and collaboration without conflating legal identity or authority.
- **Refined statement:** A household MUST be a display grouping only: membership MUST NOT grant access to any fund, MUST NOT change authority, contributor of record or acknowledgments, and MUST NOT aggregate balances for fee tiers unless a sponsor-approved schedule rule and each affected fund's holders' consent exist.
- **Reason:** States explicitly what a household may not do, closing CRM-style aggregation.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-CRM-008
- **Acceptance criteria:**
  - **AC-1** Given Spouse added to household; when views portal; then sees no fund without a role grant.
  - **AC-2** Given Household of 3 funds; when fee assessed; then each fund tiered on its own base.

### RR-UNB-GRANT-024-r1 (refines UNB-GRANT-024)

- **Original** (`requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:39` @ `777a70d`): - **UNB-GRANT-024:** Support grant agreements, restrictions, milestones, reporting obligations, conditional release and grant purpose amendments.
- **Refined statement:** Each grant MUST be assigned one GrantTermsTier by a counsel-approved deterministic rule: Tier 0 (routine grant to a Permitted recipient with no conditions) — no agreement; sponsor-standard award terms (charitable use, no goods or services to donor, return if not used for charitable purposes or if recipient status is lost before use) printed in remittance; Tier 1 (sponsor-imposed restriction, condition, milestone or multi-year schedule) — unilateral award letter recording the terms, with grantee acceptance optional unless the sponsor requires it; Tier 2 (ERRequired, or any grant counsel designates) — written commitment signed by an authorized grantee officer containing the counsel-approved §53.4945-5(b)(3) terms, captured electronically. Payment for Tier 2 MUST NOT be released before the signed commitment is recorded. Purpose amendments MUST create a new terms version and re-run the BenefitAndPurposeGate. A distribution to a §170(b)(1)(A) organization other than a disqualified supporting organization is not a §4966 taxable distribution whatever its stated purpose; purpose monitoring of such grants is optional sponsor policy, while §4967 benefit screens remain mandatory (XID-NPG-006…
- **Reason:** Original is a capability list without conditions; agreements were neither required where law demands nor limited where unnecessary.
- **Decision status:** counsel-review · **Confidence:** high · **Findings:** XIF-NPG-010, XIF-REG-004
- **Acceptance criteria:**
  - **AC-1** Given routine grant to public charity; when approval; then Tier 0; no agreement work item.
  - **AC-2** Given ERRequired grant without signature; when release; then Held AGREEMENT_SIGNATURE_REQUIRED.
  - **AC-3** Given restricted multi-year grant; when approval; then Tier 1 award letter issued.
  - **AC-D1** Given a Tier 0 grant to a public charity with a donor-stated purpose and purpose monitoring disabled; when the grant completes; then no monitoring task is created and the §4967 screen result is recorded.

### RR-UNB-GRANT-026-r1 (refines UNB-GRANT-026)

- **Original** (`requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:41` @ `777a70d`): - **UNB-GRANT-026:** Provide grantee onboarding, secure verification of organization and payment details, change-of-bank anti-fraud controls and acknowledgement collection.
- **Refined statement:** Grantee onboarding MUST NOT be a precondition for a routine grant when an existing verified destination or a check/approved disbursement-partner path is available. Payment-destination verification and change controls follow REQ-X-005/RR-REQ-X-005-r1 and UNB-RCN-GRT-017. Grantee acknowledgment of receipt MUST be optional, MUST NOT block grant completion, and MUST NOT be used as payment proof; grant completion MUST rest on settlement evidence. Any acknowledgment content the platform supplies to the grantee MUST state that no goods or services were provided and that the donor may not claim a deduction (UNB-RCN-GRT-011).
- **Reason:** Original bundles four controls of different necessity; acknowledgment chasing has no legal function for the sponsor.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-NPG-009
- **Acceptance criteria:**
  - **AC-1** Given grant settled, no acknowledgment; when 30 days pass; then grant Completed; no work item.
  - **AC-2** Given charity not enrolled, verified destination exists; when routine grant release; then proceeds without onboarding.

### RR-UNB-GRANT-041-r1 (refines UNB-GRANT-041)

- **Original** (`requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md:56` @ `777a70d`): - **UNB-GRANT-041:** Support international grants only through separately approved due diligence, sanctions, equivalency/expenditure-responsibility and payment/legal frameworks as applicable.
- **Refined statement:** A recommendation to a foreign recipient MUST be routed deterministically to exactly one InternationalRoute: (1) RoutineListed — recipient holds a US IRS determination as §170(b)(1)(A) organization; (2) EquivalencyDetermination — current ED per UNB-XIN-NPG-001; (3) ExpenditureResponsibility — sponsor offers ER and UNB-RCN-GRT-003 case completes; (4) Intermediary — donor accepts an IntermediaryCatalog alternative (UNB-XIN-NPG-002); (5) Decline. Routes 2 and 3 additionally require the ForeignNexusDeclaration (UNB-XIN-NPG-009), sanctions screening of the payment chain (UNB-RCN-GRT-007) and, where a program applies, a SanctionsAuthorization (UNB-XIN-NPG-010). Routes offered are sponsor configuration; route selection is never made by AI. Repository-sourced equivalency determinations follow XID-NPG-001.
- **Reason:** Original 'as applicable' is untestable and omits the intermediary route and ED currency.
- **Decision status:** counsel-review · **Confidence:** high · **Findings:** XIF-NPG-001, XIF-NPG-002, XIF-NPG-011, XIF-NPG-012
- **Acceptance criteria:**
  - **AC-1** Given foreign org with US determination; when recommendation; then RoutineListed.
  - **AC-2** Given no ED, ER disabled, catalogue configured; when recommendation; then Intermediary offered.
  - **AC-3** Given nothing available; when recommendation; then Declined with reason.
  - **AC-D1** Given a foreign recipient with a current repository ED and a recorded reliance letter; when the route is selected; then EquivalencyDetermination is chosen.

### RR-UNB-RES-003-r1 (refines UNB-RES-003)

- **Original** (`requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:45` @ `777a70d`): - **UNB-RES-003:** Every capability SHALL have defined RTO/RPO, service ownership, operational runbooks, telemetry, incident escalation and recovery validation proportionate to criticality.
- **Refined statement:** Unbound MUST define recovery objectives per data class, not per capability, in one versioned table, at minimum: (C0) acknowledged authoritative financial/security commits (ledger, reservations, approvals, holds, payee/destination versions, authority, payment send intents): RPO 0, RTO <= 4 hours, restore to a single consistency point across all C0 stores; (C1) issued document artifacts and evidence: RPO 0 for issued artifacts, RTO <= 24 hours; (C2) operational records (cases, notes, communications history): RPO <= 15 minutes, RTO <= 24 hours; (C3) rebuildable projections, search indexes, caches, embeddings: no backup required if rebuild from C0 is tested, RTO <= 24 hours; (C4) telemetry and non-evidentiary logs: best effort. Each tenant contract MUST reference this table; a tenant MAY buy stricter RTO only with a matching isolation profile. Every class MUST have a restore drill at least annually per isolation profile in use, including tenant-selective restore where the profile claims it, and payment release MUST stay blocked after any restore until C0 consistency and provider reconciliation pass (REQ-X-006).
- **Reason:** Per-capability RTO/RPO is untestable and inconsistent (TVS-607: stores restored to different points); objectives belong to data classes that span capabilities.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-SAS-012
- **Acceptance criteria:**
  - **AC-1** Given C0 stores restored; when restore completes; then all C0 stores share one consistency point and payment release is blocked until reconciliation passes.
  - **AC-2** Given a new search index store; when CI runs; then it must declare class C3 and pass rebuild test.
  - **AC-3** Given annual drill for Profile B; when drill runs; then evidence shows achieved RPO/RTO per class.

### RR-UNB-TEN-011-r1 (refines UNB-TEN-011)

- **Original** (`requirements/PARTNER-SEGREGATION-AND-TENANCY.md:20` @ `777a70d`): - **UNB-TEN-011:** Tenant provisioning, migration, upgrade, suspension and offboarding SHALL use a governed, reproducible plan with approvals, inventory, integrity verification, rollback/compensation strategy and evidence.
- **Refined statement:** Each tenant MUST have one TenantLifecycle state machine: Prospect > Contracted > Provisioning > Configured > ReadinessValidated > Live > (Suspended <-> Live) > ExitNoticed > ExportDelivered > ReadOnlyRetention > DestructionAuthorized > Destroyed. Transition to Live MUST require recorded evidence of: executed SponsorOperatingAgreement (REQ-X-064), sponsor board/delegation evidence, isolation profile decision, approved configuration version, provider integrations passing contract tests, and named sponsor approvers. Suspended MUST stop new commands from non-sponsor principals while accepted commands, holds, statements and retention continue. DestructionAuthorized MUST require no active legal hold, expiry of the contractual and legal retention computed by the retention engine, and dual authorization.
- **Reason:** UNB-TEN-011 requires a governed plan but no states or gates; onboarding, suspension and exit are described in three documents (REQ-X-064, REQ-X-071, REQ-X-061) without a shared lifecycle.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-SAS-013
- **Acceptance criteria:**
  - **AC-1** Given tenant with no board delegation evidence; when Live requested; then rejected GATE_EVIDENCE_MISSING.
  - **AC-2** Given tenant Suspended for fee dispute; when donor views statement; then statement available; new recommendations blocked with notice.
  - **AC-3** Given legal hold active; when DestructionAuthorized requested; then rejected.

### RR-UNB-TEN-016-r1 (refines UNB-TEN-016)

- **Original** (`requirements/PARTNER-SEGREGATION-AND-TENANCY.md:25` @ `777a70d`): - **UNB-TEN-016:** Each partner SHALL have an exportable, evidence-complete logical dataset and portability protocol, subject to lawful retention, sponsor ownership, third-party rights and contracts.
- **Refined statement:** On sponsor exit, Unbound MUST deliver within 30 days of ExitNoticed (or the contract period) an ExitPackage containing: the tenant's complete authoritative event history and current projections in the documented export schema, issued document artifacts with hashes, policy and configuration versions, audit and evidence records, and a manifest signed with SHA-256 digests plus record counts and ledger trial balances per fund and period that reconcile to the live system at the export watermark. The sponsor MUST be able to verify the package independently. After delivery the tenant MUST enter ReadOnlyRetention for the contractually agreed period, during which authorized sponsor users can query and export. Destruction MUST be performed by destroying tenant-scoped encryption keys under dual control after retention and hold checks, and Unbound MUST issue a destruction certificate stating what was destroyed, what persists (e.g. immutable backups until their expiry, with dates) and which records were retained by legal obligation. Distribution partners MUST receive only data the sponsor authorizes, never by default. Decided timings (XID-REC-001): the sponsor is records custodian after exit;…
- **Reason:** UNB-TEN-016 is not testable ('evidence-complete', 'portability protocol' undefined) and conflicts with append-only storage until RCC-ENT-005's resolution is written as a requirement; it also names the partner, not the legal sponsor, as recipient.
- **Decision status:** counsel-review · **Confidence:** medium · **Findings:** XIF-SAS-014
- **Acceptance criteria:**
  - **AC-1** Given sponsor exit noticed; when package delivered; then trial balances in package equal live balances at watermark.
  - **AC-2** Given retention expired and no hold; when destruction executed; then certificate lists destroyed keys and residual backups with expiry dates.
  - **AC-3** Given partner P requests export; when sponsor has not authorized; then denied.
  - **AC-D1** Given an ExitPackage delivered on day 0 with no legal hold; when day 91 begins; then read-only access ends and key destruction is scheduled, with backups expiring by day 126.

### RR-REQ-X-038-r1 (refines REQ-X-038)

- **Original** (`research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:374` @ `777a70d`): ## REQ-X-038: Deterministic retention-policy engine and governed disposition (2026-11-23) **Proposed:** Enforce record preservation through one versioned, typed in-process Retention Policy component within the component monolith. Financial records and evidence have a nonwaivable seven-year minimum from an approved class-specific trigger; longer legal, sponsor, contractual, dependency and legal-hold obligations prevail. Unknown or stale safety facts block deletion. Preservation, access and disposition are distinct decisions. Expiration means EligibleForReview, not automatic purge. Hold matchin…
- **Refined statement:** Disposition MUST operate on DispositionBatches defined by (tenant, record class, retention trigger period) rather than per-record review. When records in a batch reach EligibleForReview, the retention engine MUST compute the batch, excluding records under hold, with Unknown facts, or with unexpired dependencies; one authorized records owner (not the requester) MUST approve the batch; execution MUST re-check holds and safety epochs per record in the same authoritative transaction and skip (not fail) any record newly held. Per-record human review MUST be required only for records flagged by a deterministic exception rule. Expired records not yet approved MUST remain preserved; deletion MUST never be automatic.
- **Reason:** REQ-X-038 and RR-REQ-X-036-r1 ('Expiry MUST create a disposition review task') imply per-record human work, contradicting RCD-007's class-level batch recommendation and RCF-PLT-014; defensible disposition rests on consistent schedule application, not per-item review.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-REC-003
- **Acceptance criteria:**
  - **AC-1** Given batch of 10,000 FIN-03 records from FY2018; when records owner approves; then one approval covers the batch and execution logs each record.
  - **AC-2** Given hold placed on 3 records after approval; when execution runs; then the 3 are skipped and logged.
  - **AC-3** Given requester equals approver; when approval submitted; then rejected.

### RR-UNB-API-STD-003-r1 (refines UNB-API-STD-003)

- **Original** (`requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:18` @ `777a70d`): - **UNB-API-STD-003:** All APIs and event contracts require backward compatibility strategy, deprecation policy, schema evolution, contract tests, tracing/correlation IDs, rate limits and explicit failure behavior.
- **Refined statement:** Every API and event contract MUST declare a tier (Internal, Partner, Public) and a semantic version. Breaking changes, detected in CI from generated schemas (UNB-RCN-ARC-001) plus annotated semantic changes, MUST increment the major version. A superseded major version of a Partner or Public contract MUST remain available for at least 12 months after the successor is generally available, with Deprecation and Sunset metadata on every response and notice to every registered consumer at announcement, 90 days and 30 days before sunset. A sunset MUST NOT strand in-flight objects: requests accepted under the old version MUST be completed and their status remain queryable. Rate limits MUST be declared per tier and per tenant and returned in response metadata.
- **Reason:** UNB-API-STD-003 lists topics (deprecation policy, rate limits) without values or obligations, so partner contracts cannot rely on it.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-SAS-015
- **Acceptance criteria:**
  - **AC-1** Given field removed from Partner contract; when CI runs; then build fails unless major version incremented.
  - **AC-2** Given v1 sunset date reached with 3 open recommendations created via v1; when sunset executes; then the 3 remain queryable and complete.
  - **AC-3** Given v2 GA on 1 Jan 2027; when v1 sunset scheduled; then earliest permitted sunset is 1 Jan 2028.

### RR-UNB-ACC-014-r1 (refines UNB-ACC-014)

- **Original** (`requirements/FULL-ACCOUNTING-SYSTEM.md:23` @ `777a70d`): - **UNB-ACC-014:** Support budgeting, forecasts, actual-versus-budget reporting, cost centers, allocation methodologies and scenario models while distinguishing forecasts from posted actuals.
- **Refined statement:** Support budgeting, forecasts, actual-versus-budget reporting, cost centers and scenario models while distinguishing forecasts from posted actuals. Management cost allocation and activity-based attribution SHALL be computed as a non-posting projection (UNB-XIN-ECO-001) that never creates journals or changes fund balances or available-to-grant; only CPA-approved functional expense allocations (UNB-ACC-019) and approved fee/billing entries (UNB-ACC-010) may post. Every management allocation SHALL reconcile monthly to GL actuals with an explicit unallocated residual (UNB-XIN-ECO-004). Functional expense allocation and management attribution share one activity dataset with separately governed mappings (XID-ECO-005).
- **Reason:** Original groups posting-grade accounting with management allocation and does not prevent allocations from posting or being silently spread.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-ECO-002, XIF-ECO-003, XIF-ECO-015
- **Acceptance criteria:**
  - **AC-1** Given a management allocation rule; when it is executed; then no journal is created and the ledger state hash is unchanged.
  - **AC-2** Given the CPA-approved functional allocation for Form 990; when it is posted; then it is posted via UNB-ACC-019 path with approval and references the activity dataset version.
  - **AC-D1** Given the annual functional mapping lacks CPA approval; when the year-end statement of functional expenses is generated; then generation is blocked with FUNCTIONAL_MAPPING_UNAPPROVED.

### RR-AIN-028-r1 (refines AIN-028)

- **Original** (`requirements/ai/AI-NATIVE-INTERACTION-AND-AGENT-COLLABORATION.md:38` @ `777a70d`): - **AIN-028 Agent cost and latency boundaries** (Operations): Bound token, tool, time and external service budgets without silently abandoning work.
- **Refined statement:** Bound token, tool, time and external-service budgets per agent run and per purpose. Each run SHALL meter cost per invocation and attach it to its cost object (UNB-XIN-ECO-009). On budget exhaustion the run SHALL stop model calls, persist partial state, return Incomplete-BudgetExhausted (never a passing result) and route the item to a human queue; deterministic controls continue. Runs estimated above a configured threshold SHALL log a CostQuote before execution (UNB-XIN-ECO-008), which never skips a required control.
- **Reason:** "Without silently abandoning work" is untestable: no exhaustion state, routing or attribution is defined.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-ECO-006
- **Acceptance criteria:**
  - **AC-1** Given a run with $0.50 budget; when cost reaches $0.50; then status Incomplete-BudgetExhausted and human task created.
  - **AC-2** Given a run attempting to raise its own budget; when it calls budget API; then denied and logged.

### RR-UNB-ENT-003-r1 (refines UNB-ENT-003)

- **Original** (`requirements/BASELINE-001.md:7` @ `777a70d`): - UNB-ENT-003: The organization SHALL support a documented sponsor business model, revenue/fee policy, and sensitivity-tested unit economics.
- **Refined statement:** The organization SHALL maintain a documented sponsor business model and revenue/fee policy, and unit economics that are (a) computed monthly from measured attributed costs reconciled to GL (UNB-XIN-ECO-004) once operating data exists, (b) labelled by attribution grade, and (c) stress-tested for December surge, high exception rates and market drawdown via ScenarioRun (UNB-XIN-ECO-013), with illustrative inputs explicitly labelled and replaced by measured values as they become available.
- **Reason:** "Sensitivity-tested unit economics" is satisfied by a one-off model; nothing requires measured inputs or refresh.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-ECO-012, XIF-ECO-001
- **Acceptance criteria:**
  - **AC-1** Given a closed month with reconciled attribution; when unit economics are published; then each figure cites ReconciliationRun id and grade.
  - **AC-2** Given a parameter still illustrative; when stress test runs; then output states which conclusions rest on illustrative inputs.

### RR-UNB-ECO-001-r1 (refines UNB-ECO-001)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:53` @ `777a70d`): **UNB-ECO-001:** Model accounts, AUM, average balance, grants, contributions, exception rate, review time, complex asset volume, service contacts and reconciliation breaks as separate drivers.
- **Refined statement:** Model accounts, AUM, average balance, grants (by rail), contributions (by type), exception rate by type, review minutes per exception type, complex-asset volume, service contacts and resolutions, reconciliation breaks, AI runs and charity verifications (new vs reuse) as separate drivers; at runtime each driver SHALL be measured from domain events (ActivityEvent) and stored in a versioned parameter register with provenance (measured / benchmark / assumption), replacing planning estimates only via an Observation source.
- **Reason:** Drivers listed for planning; no runtime measurement or link to cost attribution; rail and exception-type granularity missing though they drive cost by orders of magnitude.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-ECO-004, XIF-ECO-007
- **Acceptance criteria:**
  - **AC-1** Given one month of synthetic events; when drivers are computed; then each driver equals the event count/sum for its definition and cites the event window.
  - **AC-2** Given a planning estimate for minutes per exception; when a measured value arrives; then a new parameter version with source Observation is created; the estimate remains in history.

### RR-UNB-EVT-001-r1 (refines UNB-EVT-001)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:36` @ `777a70d`): **UNB-EVT-001:** Maintain a governed event catalog with owner, schema, version, producer, authorized actor, consumer, control, evidence and side-effect classification for every event.
- **Refined statement:** The governed event catalog MUST record, for every event type: owning module; authority class (authoritative-financial, authoritative-nonfinancial, derived, informational, observation-ai); source-of-truth store; required consistency with the state it describes (synchronous or eventual); ordering scope (per aggregate, per fund, per sponsor ledger commit position, global hold version, or none); idempotency key formula; correction pattern; tenant scoping; retention class; producers; consumers; and crosswalk to equivalent names in other catalogs. A consumer that issues a consequential command MUST NOT be subscribed to an event whose authority class is derived, informational or observation-ai unless that command re-reads the authoritative fact in its own transaction; CI MUST reject such subscriptions.
- **Reason:** The existing catalog fields do not say whether an event may be acted upon or whether it is the truth, so informational and AI events can gate money.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-EVA-002, XIF-EVA-018
- **Acceptance criteria:**
  - **AC-1** Given Handler ApprovePaymentRelease subscribed to PaymentExecutionAcknowledged (informational); when CI runs; then build fails with NON_AUTHORITATIVE_GATE.
  - **AC-2** Given New event lacking idempotency key formula; when catalog validated; then build fails.

### RR-UNB-EVT-005-r1 (refines UNB-EVT-005)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:40` @ `777a70d`): **UNB-EVT-005:** State-changing events with legal/financial consequences SHALL be emitted only by approved deterministic rules or authorized people, not raw probabilistic agent output.
- **Refined statement:** The F# domain model MUST define distinct types Observation (any source including AI; no authority), Proposal (a suggested command with evidence; no authority), Command (an authenticated principal's request) and Fact (an authoritative recorded outcome). Fact values MUST be constructible only inside owning-module command handlers (private constructors) after deterministic validation of an authorized Command. The AI workbench principal MUST be able to create only Observation and Proposal records, and its database role MUST hold INSERT only on the ai_observation schema. An AI Observation MAY influence a Fact only through an EvidencePromoted record created by a deterministic validator or an authorized human principal, which the Fact then cites.
- **Reason:** UNB-EVT-005 states the outcome without an enforcement mechanism; type and privilege separation make the prohibition structural.
- **Decision status:** autonomous-reversible · **Confidence:** high · **Findings:** XIF-EVA-019
- **Acceptance criteria:**
  - **AC-1** Given AI agent proposes clearing a sanctions hold; when proposal recorded; then no Fact created; hold remains; reviewer sees proposal (SIM-22).
  - **AC-2** Given AI extracts EIN from a charity letter; when validator matches EIN to IRS file and promotes; then EligibilityEvaluationCompleted cites EvidencePromoted, which cites the observation.

### RR-UNB-DATA-004-r1 (refines UNB-DATA-004)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:14` @ `777a70d`): **UNB-DATA-004:** Current state SHALL be computed as a deterministic projection of immutable authoritative records. Projection caches, materialized views, search indexes and read replicas MAY be rebuilt or replaced, but are explicitly non-authoritative. Their updates do not constitute modification of the append-only record.
- **Refined statement:** Records MUST be classified into storage classes. (1) Authoritative financial and authority facts (journal, reservations, grant/payment/contribution decisions, holds, policy versions, authority grants) MUST be append-only; their current state MAY be maintained in current-state tables updated in the same transaction as the fact append and MUST be deterministically rebuildable from the facts. (2) Engagement and relationship data (contact details, preferences, interactions, notes) MAY be stored as mutable rows with system-versioned history and is not required to be rebuildable from events; it is eligible for privacy redaction. (3) Operational coordination state (outbox delivery status, process-manager step and timers, leases, work-queue assignment) MAY be updated in place with audit of material transitions and is not authoritative. Projections, caches and search indexes remain non-authoritative as originally stated.
- **Reason:** Applying projection-of-immutable-records to every record class forces event sourcing where no replay need exists, conflicts with erasure and with the outbox.
- **Decision status:** owner-decision · **Confidence:** medium · **Findings:** XIF-EVA-004, XIF-EVA-021
- **Acceptance criteria:**
  - **AC-1** Given Contact preference changed twice; when history queried; then both prior values with timestamps returned; current row updated in place.
  - **AC-2** Given Fund balance current-state table dropped; when rebuild runs from journal and reservations; then identical current state.

### RR-UNB-DATA-005-r1 (refines UNB-DATA-005)

- **Original** (`requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md:15` @ `777a70d`): **UNB-DATA-005:** Corrections SHALL preserve the original, correction reason, authorized corrector, linked evidence and effect on derived balances, grants, filings and notices. Financial corrections SHALL use balanced reversal/adjustment entries, never silent journal mutation.
- **Refined statement:** The system MUST maintain a versioned correction impact registry that maps each correctable fact type to the derived records computed from it (e.g., contribution valuation -> acknowledgment amount, Form 8283/8282 obligations, unit purchase, fee basis; NAV -> unit allocations, fee accruals, statements; journal batch -> snapshots, statements, Schedule D aggregates, capacity; payee destination -> pending payment intents). When a correction (reversal, re-post, restatement) commits, the system MUST deterministically enumerate impacted derived records as of the correction's effective date, create a linked task or automatic recomputation per registry rule (automatic only for deterministic recomputations such as fee delta postings; human for legal documents such as acknowledgments), and MUST NOT mark the correction case closed until every impacted item is resolved or explicitly waived with authority. Financial corrections continue to use balanced reversal/adjustment entries. For donor fund statements the registry rule MUST produce an amended statement whenever an acknowledgment or tax-relevant figure changes, and otherwise an explained adjustment on the next statement (XID-EVA-002).
- **Reason:** UNB-DATA-005 states that effects on derived records are preserved but not how they are found or completed.
- **Decision status:** autonomous-reversible · **Confidence:** medium · **Findings:** XIF-EVA-017
- **Acceptance criteria:**
  - **AC-1** Given Contributed security valued on the wrong date; correction changes value from $52,000 to $49,800; when correction commits; then impact list includes acknowledgment amendment task, fee basis recompute delta posting, unit purchase adjustment and statement amendment for affected periods.
  - **AC-2** Given Correction case with one open impact item; when closure attempted; then rejected IMPACTS_OPEN.
  - **AC-D1** Given a correction that changes a fund balance but no acknowledgment; when it commits; then the next fund statement carries an explained adjustment line and no amended statement is issued.

### RR-REQ-X-069-r1 (refines REQ-X-069)

- **Original** (`research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:572` @ `777a70d`): ### REQ-X-069: Nonprofit impact, restrictions and grant outcomes **Proposed:** Extend the one Unbound component monolith with the capability's documented operating workflow, domain ownership, sponsor-scoped permissions, legal/partner boundaries, role-specific UX, typed API/data contracts, deterministic provider mocks, evidence, exceptions and audit. Corresponds to GAP-012. See the dedicated REQ-X-069 specification dated 2026-11-23. No implementation or tests claimed.
- **Refined statement:** Grant-outcome and restriction monitoring MUST be sponsor-configurable and OFF by default for grants to §170(b)(1)(A) organizations (other than disqualified supporting organizations), because such distributions are excepted from §4966 taxable distributions whatever their stated purpose (XID-NPG-006). Monitoring MUST be mandatory only for expenditure-responsibility grants (reports per §53.4945-5) and for grants the sponsor itself conditions. §4967 benefit screens apply to every grant regardless of monitoring settings.
- **Reason:** REQ-X-069 extends monitoring generally; the statute excepts public-charity distributions regardless of purpose, so universal monitoring is unnecessary work.
- **Decision status:** owner-decision · **Confidence:** high · **Findings:** 
- **Acceptance criteria:**
  - **AC-1** Given a routine grant to a public charity under default policy; when it is paid; then no outcome-reporting task is created.
  - **AC-2** Given an expenditure-responsibility grant; when it is approved; then annual report obligations are created and release is blocked until the schedule exists.

<!-- END GENERATED: refinement-detail -->

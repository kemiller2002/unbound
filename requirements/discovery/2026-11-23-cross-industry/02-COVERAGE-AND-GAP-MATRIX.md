# 02: Coverage and gap matrix (deliverable 2)

This matrix compares the cross-industry capabilities with the existing Unbound requirements. The coverage classes are defined in [00 §3](00-BASELINE-AND-TRACEABILITY.md). "Covered" means *documented*. Nothing in the baseline is implemented.

## Coverage by mission section

<!-- BEGIN GENERATED: coverage-matrix -->
| Industry | covered | partial | contradictory | missing | Total |
|---|---|---|---|---|---|
| A Banking and financial services | 11 | 21 |  | 4 | 36 |
| B Investment and wealth / M custody, fund admin | 17 | 22 |  | 5 | 44 |
| C Accounting, GL, fund accounting | 11 | 9 | 1 | 3 | 24 |
| D Call centers and customer service | 7 | 20 | 1 | 3 | 31 |
| E Insurance and claims | 8 | 12 | 1 | 1 | 22 |
| F Healthcare administration | 6 | 3 |  |  | 9 |
| G Enterprise resource planning | 24 | 26 | 2 | 5 | 57 |
| H CRM, sales, relationship management | 5 | 9 |  | 2 | 16 |
| I Payments, merchant processing, billing | 3 | 7 |  | 3 | 13 |
| J Nonprofit administration and grantmaking | 13 | 12 | 2 | 4 | 31 |
| K Trust, estate, family office | 5 | 6 |  |  | 11 |
| L Enterprise SaaS and platform operations | 2 | 2 |  |  | 4 |
| M Identity and access management | 3 | 5 |  | 1 | 9 |
| M Legal case mgmt, BPO, fraud investigation |  | 1 |  |  | 1 |
| M Other industries | 7 | 7 | 1 | 1 | 16 |
| M Records management | 1 | 4 | 1 |  | 6 |
| M Subscription billing | 1 | 4 |  | 2 | 7 |
| §18 Operating economics analogues | 1 | 10 |  | 2 | 13 |
| §19 Event architecture analogues |  | 2 | 1 | 1 | 4 |
| §9 / M Regulatory, tax and benefits administration | 7 | 6 |  | 1 | 14 |
| **Total** | 132 | 188 | 10 | 38 | 368 |
<!-- END GENERATED: coverage-matrix -->

## Capabilities that are missing or contradictory

These are the atlas rows with no existing requirement, or with conflicting requirements. Each one produced either a new requirement in [03](03-MISSING-REQUIREMENTS-REGISTER.md), a refinement in [04](04-REFINEMENT-RECORDS.md), a conflict record, or a rejection in [09](09-REJECTED-RECOMMENDATIONS.md).

<!-- BEGIN GENERATED: gaps-attention -->
| ID | Industry | Capability | Coverage | Necessity | Business problem |
|---|---|---|---|---|---|
| XCAP-BNK-008 | Banking and financial services | Instant payments (RTP / FedNow) | missing | commercial | Irrevocable real-time credit push. |
| XCAP-BNK-019 | Banking and financial services | Deposit concentration and FDIC exposure | missing | commercial | Limit uninsured exposure to one bank. |
| XCAP-BNK-026 | Payments, merchant processing and billing | Disputes and chargebacks (card contributions) | missing | legal | Handle involuntary reversals. |
| XCAP-BNK-028 | Payments, merchant processing and billing | Failed payments and retries (contributions) | missing | legal | Collect recurring debits. |
| XCAP-BNK-032 | Payments, merchant processing and billing | Card-testing / enumeration attack defense | missing | operational | Stop fraudsters using donation forms to test stolen cards. |
| XCAP-INV-009 | Investment management and wealth management | Advisor fee debit verification (custodian-debited fees from DAF accounts) | missing | legal | Advisors usually send a billing file to the custodian, which debits the DAF account; the sponsor must authorize and verify each debit. |
| XCAP-INV-012 | Investment management and wealth management | Voluntary corporate actions and proxy voting | missing | operational | Elections (tenders, rights, dividend options) and shareholder votes need an authorized decision by a deadline. |
| XCAP-INV-016 | Investment management and wealth management | Cash sweep and uninvested cash management | missing | operational | Cash awaiting investment or grant payment earns sweep yield and may exceed deposit insurance. |
| XCAP-INV-025 | Investment management and wealth management | Investment Company Act / Advisers Act structure of pooled options | missing | legal | A pooled investment option or a platform-run model could be an unregistered investment company or adviser. |
| XCAP-ACC-002 | Accounting, general ledger and fund accounting | Authoritative journal vs derived projections (event / interpretation / report separation) | contradictory | legal | If journals are re-derived from events under current rules, the books the auditor tested can change. |
| XCAP-ACC-006 | Accounting, general ledger and fund accounting | Agency / pass-through funds (funds held for others) | missing | legal | Agency endowments and donor-designated pass-through gifts are liabilities, not contributions. |
| XCAP-ACC-015 | Accounting, general ledger and fund accounting | Audit adjustments and post-close pre-issuance period | missing | operational | Auditor-proposed entries after close but before statements are issued; passed adjustments. |
| XCAP-ACC-018 | Accounting, general ledger and fund accounting | Accounting policy changes and historical corrections (principle, estimate, error) | missing | legal | A posting-rule change must be applied retrospectively, prospectively or as a restatement. |
| XCAP-SVC-006 | Call centers and customer service | Distinction between contact, case, financial transaction and operational exception | contradictory | operational | Treating every contact as a case inflates queues; treating transactions as cases duplicates state machines. |
| XCAP-SVC-010 | Call centers and customer service | Interaction recording and recording consent | missing | legal | Recordings are needed for QA, dispute evidence and fraud investigation but are unlawful without required consent in all-party consent states. |
| XCAP-SVC-011 | Call centers and customer service | Transcription and AI interaction summaries | missing | operational | Summaries reduce after-call work and preserve context. |
| XCAP-SVC-012 | Call centers and customer service | Sentiment, intent and complaint signal detection | missing | operational | Complaints are often not labeled as complaints by the customer. |
| XCAP-SVC-030 | Cross-industry (unified work management) | Unified work management kernel | contradictory | operational | One case/work model across grants, contributions, vetting, complaints, financial exceptions and compliance investigations. |
| XCAP-CLM-011 | Insurance and claims administration | Case reopening | contradictory | operational | New facts after closure must be handled without rewriting history. |
| XCAP-CLM-013 | Insurance and claims administration | Closed-file decision-quality audit (leakage review) | missing | operational | Straight-through approvals can contain undetected errors. |
| XCAP-ERP-006 | Enterprise resource planning | Billing and accounts receivable | contradictory | operational | Invoice partners/sponsors and collect platform fees; assess DAF admin fees. |
| XCAP-ERP-017 | Enterprise resource planning | Fee/billing event classification (financial vs informational) | missing | operational | Event consumers must know which fee events are authoritative. |
| XCAP-ERP-020 | Subscription billing (M partial) | Proration and billing-period segmentation | missing | contractual | Open/close/transfer/schedule change mid-period. |
| XCAP-ERP-021 | Subscription billing (M partial) | Donor-paid membership fees (flat subscription) | missing | legal | Some challengers charge monthly membership to a card. |
| XCAP-CRM-010 | CRM, sales and relationship management | Segmentation | missing | legal | Group donors for communications and service. |
| XCAP-CRM-015 | CRM, sales and relationship management | Professional advisor change of firm | missing | contractual | Advisor moves firms; who keeps the relationship. |
| XCAP-NPG-003 | Nonprofit administration and grantmaking (private/community foundation practice) | Recipient-class eligibility mapping (Permitted / ERRequired / Prohibited / CounselReview) | contradictory | legal | Determines whether a distribution is taxable. |
| XCAP-NPG-009 | Nonprofit administration and grantmaking (private/community foundation practice) | International giving via US intermediaries (CAF America, GlobalGiving, Give2Asia, 'friends of' organizations) | missing | commercial | Donors want foreign causes without sponsor-level ED/ER cost. |
| XCAP-NPG-013 | Nonprofit administration and grantmaking (private/community foundation practice) | Sponsor information-return data: Form 990 Schedule I and Schedule F grant detail | missing | legal | Sponsor must report grants >$5,000 per recipient (Schedule I) and foreign grants/activities (Schedule F: region, purpose, manner of disbursement, ED/ER procedu… |
| XCAP-NPG-024 | Nonprofit administration and grantmaking (private/community foundation practice) | Foreign-official / anti-bribery diligence for international grants | missing | legal | Grants benefiting foreign officials can violate FCPA. |
| XCAP-NPG-026 | Nonprofit administration and grantmaking (private/community foundation practice) | Excise-tax incident handling (taxable distribution, §4967 benefit, §4958 excess benefit) | missing | legal | Detected violations need correction, return filing and board reporting. |
| XCAP-NPG-027 | Nonprofit administration and grantmaking (private/community foundation practice) | §530A (Trump account) general funding contributions through a DAF | contradictory | commercial | New lawful channel for DAF distributions benefiting broad classes of children. |
| XCAP-REG-001 | Regulatory applicability (cross-industry) | Custody/titling of disbursement funds (no vendor-held float) | missing | legal | If a for-profit vendor holds or moves charitable money, money-transmission and insolvency risk follow. |
| XCAP-SAS-002 | enterprise SaaS platform operations | Isolation profiles (schema, database, dedicated cell) selected per tenant without code fork | contradictory | commercial | Institutional buyers demand stronger isolation; supporting every profile from day one multiplies tests and migrations. |
| XCAP-SAS-007 | enterprise SaaS platform operations | Feature entitlements (contracted capability set per tenant/program) | missing | commercial | What a sponsor or partner has bought (e.g. complex assets, international grants, API access, AI assistance) is not represented; sales promises leak into policy… |
| XCAP-SAS-008 | enterprise SaaS platform operations | Feature flags (release toggles) with expiry and no semantic effect | missing | operational | Shipping one release to all cells needs dark launch and kill switches; unmanaged flags become hidden tenant forks. |
| XCAP-SAS-019 | enterprise SaaS platform operations | Distribution-partner exit with fund portability to a new sponsor | missing | legal | A wealth firm or program leaving one sponsor for another wants its donors' funds to follow; funds are the old sponsor's property. |
| XCAP-SAS-027 | enterprise SaaS platform operations | Per-tenant metering and cost attribution (storage, compute, API, AI, provider calls) | missing | commercial | Cost-to-serve per sponsor/program/model is unknowable, so pricing, isolation-tier economics (ADR-009) and the high-volume-low-balance model cannot be evaluated. |
| XCAP-IAM-005 | identity and access management | Platform-operator (vendor workforce) access to tenant data under sponsor authority | missing | legal | Under outsourced/infrastructure models operator staff are not covered by the sponsor's delegation of authority. |
| XCAP-REC-003 | records management | Defensible disposition by approved class-level batches | contradictory | legal | Per-record human review at expiry does not scale and is not more defensible. |
| XCAP-OTH-002 | 529 college savings plan administration | Standardized machine-readable program description (fees, investment options, roles) per program (MSRB Form G-45 analog) | missing | commercial | Advisors and donors compare programs by marketing pages; fees and options per white-label program are opaque. |
| XCAP-ECO-004 | Cloud FinOps | Normalized cost and usage data schema (FOCUS) for multi-vendor technology spend including AI, SaaS and cloud | missing | operational | Vendor invoices use incompatible schemas, so cost cannot be joined to usage or allocated consistently. |
| XCAP-ECO-033 | Banking / insurance operations | Cost of human approvals and four-eyes controls as an explicit control cost (cost per approval, approvals per outcome) | missing | operational | Controls accumulate; their cost is never weighed against risk reduced. |
| XCAP-ECO-036 | Cloud FinOps | Commitment/discount and vendor rate-card management (effective vs billed cost) | missing | operational | List-price attribution overstates cost; discounts unallocated. |
| XCAP-EVA-006 | Custody/fund admin | Expected-file calendar, control totals and late-data handling | missing | operational | Silent missing data |
| XCAP-EVA-020 | Event-sourced fintech ledgers | Full event sourcing of ledger and domain | contradictory | conventional | Audit and replay |
| XCAP-EVA-022 | Bank M&A | Account portfolio conversion between legal entities | missing | legal | Transfer books between institutions |
| XCAP-EVA-027 | Data platforms | Change-data-capture to analytics | missing | commercial | Analytics without OLTP load |
<!-- END GENERATED: gaps-attention -->

## Findings

Findings cover gaps, defects, contradictions, over-complication and duplicates. Severity follows the reconciliation scale.

<!-- BEGIN GENERATED: findings-severity -->
| Domain | critical | high | medium | low | Total |
|---|---|---|---|---|---|
| BNK |  | 4 | 5 | 2 | 11 |
| PAY | 1 | 2 | 4 |  | 7 |
| ACC | 1 | 6 | 6 |  | 13 |
| INV |  | 3 | 6 | 2 | 11 |
| SVC |  | 6 | 5 | 1 | 12 |
| CLM | 1 | 3 | 6 | 1 | 11 |
| ERP |  | 4 | 6 |  | 10 |
| CRM |  | 3 | 3 | 2 | 8 |
| TRS |  | 2 | 3 | 3 | 8 |
| NPG |  | 5 | 8 | 1 | 14 |
| REG | 1 | 1 | 1 | 2 | 5 |
| GOV |  |  | 1 |  | 1 |
| SAS | 1 | 7 | 8 | 1 | 17 |
| IAM | 2 | 2 | 1 |  | 5 |
| REC |  | 1 | 2 |  | 3 |
| WFA |  | 1 | 1 | 1 | 3 |
| OTH |  | 2 | 1 | 1 | 4 |
| ECO |  | 5 | 13 | 2 | 20 |
| EVA |  | 7 | 14 | 2 | 23 |
| ADV | 1 | 6 | 14 | 1 | 22 |
| INT |  |  | 9 |  | 9 |
| **Total** | 8 | 70 | 117 | 22 | 217 |
<!-- END GENERATED: findings-severity -->

### Critical and high findings

<!-- BEGIN GENERATED: findings-severe -->
| Finding | Severity | Title | Affected | Proposed resolution |
|---|---|---|---|---|
| XIF-BNK-001 | high | Nacha 2026 false-pretenses fraud monitoring is already binding on every non-consumer Originator but is recorded only as a '[S] contractual' row | OB-43;REG-044;UNB-PAY-008;UNB-PAY-002 | Adopt UNB-XIN-BNK-002 (documented program, typology, annual review evidence) and UNB-XIN-BNK-001 (rail obligation register). |
| XIF-PAY-002 | high | Recurring contribution debits from consumer accounts lack Reg E §1005.10(b) authorization and copy obligations, which bind the sponsor as payee | UNB-GRANT-027;UNB-RCN-GRT-004;REQ-X-009 | Adopt UNB-XIN-PAY-001. |
| XIF-PAY-003 | high | WEB debit account validation before first use is a Nacha requirement absent from contribution intake | REQ-X-063;UNB-REM-003;OB-43 | Adopt UNB-XIN-PAY-002. |
| XIF-PAY-004 | critical | Card contribution disputes and chargebacks have no lifecycle; they collide with irrevocable acceptance and issued acknowledgments | REQ-X-009;RR-REQ-X-009-r1;UNB-RCN-FIN-013;UNB-RCN-FIN-001 | Adopt UNB-XIN-PAY-003 and conflict XIC-BNK-001. |
| XIF-BNK-005 | high | Provisional window is defined only by ACH R10 (≈60 days) while actual windows vary by rail, SEC code and account type | TVR-CON-001;OD-04;REQ-X-009 | Refine TVR-CON-001 (RR-TVR-CON-001-r1) into a versioned per-rail ReversalWindow table. |
| XIF-BNK-006 | high | No requirement makes every bank-account debit match an authorized platform instruction | TVR-PAY-004;TVF-223;UNB-GAP-009;EXT-03 | Adopt UNB-XIN-BNK-003. |
| XIF-BNK-010 | high | Payment deferral gate lists 'fraud operations' as a future decision, but several obligations bind from the first live contribution or grant, regardless of provider | UNB-PAY-008;UNB-PAY-002 | Refine UNB-PAY-008 (RR-UNB-PAY-008-r1). |
| XIF-ACC-001 | critical | Posting idempotency keyed by posting-rule version permits re-posting history when a rule changes | ACC-MDL-007;ACC-MDL-006;UNB-NAV-044;UNB-ACC-036 | Refine ACC-MDL-007 (RR-ACC-MDL-007-r1): idempotency per (source event, book); rule-version changes only via the accounting-change protocol (UNB-XIN-ACC-004); add re-derivation as a detective control (UNB-XIN-ACC-003). |
| XIF-ACC-002 | high | Grant recognition point is undecided and conditional grants have no barrier tracking | UNB-ACC-007;OD-19;ACC-ADR-004;TVF-103 | UNB-XIN-ACC-002: sponsor-level recognition-point parameter with communication evidence and a ConditionalGrant state; recommend approval-plus-communication as default for CPA review. |
| XIF-ACC-003 | high | Net-asset classification is not tied to the variance-power evidence that justifies it | UNB-ACC-033;ACC-MDL-008;ACC-ADR-007 | Refine UNB-ACC-033 (RR-UNB-ACC-033-r1). |
| XIF-ACC-004 | high | Agency funds and donor-designated pass-through gifts are not modeled | UNB-RCN-ENT-011 | UNB-XIN-ACC-001. |
| XIF-ACC-005 | high | No post-close audit-adjustment period or passed-adjustment register | UNB-ACC-017;UNB-ACC-020 | UNB-XIN-ACC-005. |
| XIF-ACC-006 | high | Accounting changes (principle, estimate, error) have no classification or mechanism | ACC-MDL-010;UNB-NAV-043;ACC-ADR-011 | UNB-XIN-ACC-004. |
| XIF-ACC-007 | high | No required bridge between Schedule D Part I aggregates, fund statements and audited net assets | RR-REQ-X-060-r1;UNB-RCN-ENT-012;OB-14 | UNB-XIN-ACC-006 with typed reconciling items. |
| XIF-INV-001 | high | Platform-generated portfolio recommendations may make the operator an investment adviser | UNB-INV-019 | RR-UNB-INV-019-r1: recommendations limited to sponsor-approved models and arithmetic checks unless counsel approves. |
| XIF-INV-002 | high | Pooled investment options have no recorded securities-law basis; cross-sponsor or operator-maintained pools are not gated | UNB-INV-003;ADV-131;UNB-INV-011 | UNB-XIN-INV-002. |
| XIF-INV-003 | high | Advisor fees debited by custodians are not verified against the sponsor-approved agreement | UNB-INV-006;UNB-RCN-FIN-010;CTL-025 | UNB-XIN-INV-001. |
| XIF-SVC-001 | high | One case model is right, one case lifecycle is harmful | RCF-PLT-025;REQ-X-048;REQ-X-041;AIN-009;ADM-016;REQ-X-057 | Adopt a case kernel with per-kind policy profiles (UNB-XIN-SVC-002): kernel = identity, kind, subjects, owner/assignee, routing, timeline, evidence refs, commitments, audit; profile = state set and closure rule, clock p… |
| XIF-SVC-002 | high | WorkItem creation rule excludes customer-initiated requests | RR-REQ-X-048-r1;UNB-RCN-ENT-016;ADM-015;UNB-GAP-026 | Refine to RR-REQ-X-048-r2 adding cause (e): a request or complaint from an authenticated or identified external party that requires a human response, still with exit condition and creating source. |
| XIF-SVC-003 | high | No recording or transcription consent requirement | REQ-X-025;TEL-01;ADV-162 | UNB-XIN-SVC-003: universal disclosure and consent before any recording or machine transcription, unrecorded alternative, pause for secrets, vendor contract review; counsel decision XID-SVC-002. |
| XIF-SVC-004 | high | No definition separating contact, case, transaction and exception | ADM-015;ADM-016;AIN-009;REQ-X-041;REQ-X-048;UNB-GAP-026 | UNB-XIN-SVC-001 typed interaction classes and promotion rules. |
| XIF-SVC-009 | high | Transcripts and AI summaries could be mistaken for instructions | AIN-014;UNB-RCN-GRT-010;ADV-376 | UNB-XIN-SVC-009. |
| XIF-SVC-011 | high | No assurance-by-channel disclosure and action matrix for assisted contacts | TVF-132;UNB-RCN-ENT-016;REQ-X-025;REQ-X-072 | UNB-XIN-SVC-004. |
| XIF-CLM-001 | high | Reconsideration lacks independence, filing window, de novo review and scope | UNB-RCN-ENT-016;CHAR-ADR-007;UNB-GAP-019 | UNB-XIN-CLM-001. |
| XIF-CLM-002 | high | No content standard for adverse or conditional decision notices | AIN-025;UNB-GEX-013;UNB-RCN-PLT-002 | UNB-XIN-CLM-002. |
| XIF-CLM-004 | high | Information requests have no deadline, tolling or duplicate-ask prevention | AIN-007;REQ-X-048;RR-REQ-X-048-r1 | UNB-XIN-CLM-003. |
| XIF-CLM-005 | critical | No restricted investigation case kind; investigator and adjudicator not separated | UNB-GEX-033;REQ-X-072;ADV-643;EXC-16 | UNB-XIN-CLM-004. |
| XIF-ERP-001 | high | Fee period segmentation, proration and minimum-fee shortfall unspecified | UNB-RCN-FIN-005;ADM-028;UNB-NAV-032 | UNB-XIN-ERP-001. |
| XIF-ERP-002 | high | No payer-source distinction for fees | UNB-ACC-010;UNB-ACC-013 | UNB-XIN-ERP-002. |
| XIF-ERP-004 | high | No guard separating operating-vendor payees from grant recipients | UNB-ACC-012;RR-UNB-ACC-012-r1;UNB-RCN-FIN-013 | UNB-XIN-ERP-004. |
| XIF-ERP-005 | high | Threshold splitting is a test name without a rule | GOV-AUTH-001;CAG-02 | UNB-XIN-ERP-005. |
| XIF-CRM-001 | high | Shared party model lacks capacity-scoped PII | ONT-1;UNB-PART-006 | UNB-XIN-CRM-001. |
| XIF-CRM-002 | high | Affiliation edges that feed TVCTL-14 are never captured | TVCTL-14;UNB-RCN-ENT-011 | UNB-XIN-CRM-002. |
| XIF-CRM-003 | high | Communications rules omit sender entity, opt-out timing and nonprofit/for-profit regime | UNB-RCN-PLT-002;ADM-030 | UNB-XIN-CRM-003. |
| XIF-TRS-004 | high | Representative scope over advisory privileges and restoration undefined | UNB-RCN-ENT-010;ADM-022 | UNB-XIN-TRS-004. |
| XIF-TRS-007 | high | No precedence rules for conflicting instructions | ADM-023;UNB-PART-005;AUTH-I4 | RR-ADM-023-r1. |
| XIF-NPG-001 | high | Equivalency-determination route is unspecified and its legal basis was recorded as low-confidence | UNB-GRANT-041;REQ-X-065;OB-20 | Add UNB-XIN-NPG-001 (ED case with computed currency) and RR-UNB-GRANT-041-r1; upgrade OB-20 confidence (see XRA-001). |
| XIF-NPG-004 | high | Grant data needed for Form 990 Schedules I and F is not captured | RR-REQ-X-060-r1;REQ-X-060;UNB-ACC-021 | Add UNB-XIN-NPG-004. |
| XIF-NPG-005 | high | Charity organizational changes have no defined effect on open and recurring grants | UNB-CHAR-004;UNB-CHAR-015;RR-REQ-X-007-r1 | Add UNB-XIN-NPG-005 (OrganizationChangeEffect table). |
| XIF-NPG-006 | high | Sponsor discretionary holds lack criteria, expiry and consistency evidence | UNB-CHAR-031;UNB-CHAR-023;UNB-EXT-005 | Add UNB-XIN-NPG-006. |
| XIF-NPG-008 | high | No workflow when a taxable distribution, prohibited benefit or excess-benefit payment is discovered after payment | UNB-GRANT-011;OB-15;OB-05;OB-03 | Add UNB-XIN-NPG-008. |
| XIF-REG-001 | critical | No requirement fixes who holds legal title to disbursement funds; a vendor-held float can make the operator a money transmitter | REQ-X-064;UNB-GRANT-021;OB-32 | Add UNB-XIN-REG-001: sponsor-titled accounts, vendor instruction-only authority; any vendor-custody model blocked pending counsel analysis. |
| XIF-REG-002 | high | Operator regulatory status is not tied to feature activation | REQ-X-064;UNB-RCN-ENT-014;OB-34;OB-38;OB-42 | Add UNB-XIN-REG-002. |
| XIF-SAS-001 | high | No entitlement model separate from sponsor policy and flags | REQ-X-045;REQ-X-064 | Add UNB-XIN-SAS-001 with NonRemovable fiduciary capabilities. |
| XIF-SAS-002 | high | Feature-flag governance absent; flags are the usual path to hidden tenant forks | RR-REQ-X-057-r1 | Add UNB-XIN-SAS-002. |
| XIF-SAS-003 | high | Release rollout and schema migration fan-out across cells unspecified | RR-REQ-X-057-r1;UNB-TEN-002;UNB-TEC-STRATA-002 | Add UNB-XIN-SAS-003. |
| XIF-SAS-005 | high | Tenant envelope and cross-tenant correlation rule not mandated for events and telemetry | UNB-TEN-003;UNB-TEN-009;UNB-DATA-002 | Add UNB-XIN-SAS-005. |
| XIF-SAS-007 | critical | Distribution-partner exit with fund portability is undefined and invites ownership change by data migration | RR-UNB-WF-014-r1;UNB-SUCC-014;UNB-TEN-016;UNB-LS-015 | Add UNB-XIN-SAS-007; refine UNB-TEN-016 so partners receive only sponsor-authorized data. |
| XIF-SAS-009 | high | No per-tenant metering; cost-to-serve and isolation-tier economics cannot be computed | REQ-X-071;UNB-TEN-018 | Add UNB-XIN-SAS-009. |
| XIF-SAS-012 | high | Recovery objectives per capability are untestable; need data-class table | UNB-RES-003;REQ-X-006 | RR-UNB-RES-003-r1. |
| XIF-SAS-014 | high | Sponsor exit export/destruction not reconciled with append-only storage in a requirement | UNB-TEN-016;UNB-DATA-001 | RR-UNB-TEN-016-r1. |
| XIF-IAM-001 | high | No identity realm separation | REQ-X-072;SOC2-010;CTL-033 | UNB-XIN-IAM-001. |
| XIF-IAM-002 | critical | Operator workforce access outside sponsor authority (TVF-134 not adopted) | CTL-031;GOV-AUTH-001 | UNB-XIN-IAM-002. |
| XIF-IAM-003 | high | Step-up rules scattered, partner-delegated and without freshness | CTL-033;AUTH-I7;TVF-136 | UNB-XIN-IAM-003. |
| XIF-IAM-005 | critical | Break-glass controls exist only as findings | CTL-031;GOV-AUTH-024 | UNB-XIN-IAM-005. |
| XIF-REC-001 | high | Legal holds do not propagate to processors, partners or migration targets | REQ-X-038;WG-7 | UNB-XIN-REC-001. |
| XIF-WFA-001 | high | No durable timer/obligation mechanism | TIME-1;ADM-033 | UNB-XIN-WFA-001. |
| XIF-OTH-003 | high | Partners/hosts not structurally excluded from funds flow | REQ-X-063;OB-38 | UNB-XIN-OTH-003. |
| XIF-OTH-004 | high | Print/mail chain of custody missing on check rail | UNB-RCN-GRT-005;TVS-505 | UNB-XIN-OTH-004. |
| XIF-ECO-001 | high | No shared cost attribution model behind the unit-economics KPIs | REQ-X-043;UNB-ENT-003;UNB-ECO-001 | Adopt UNB-XIN-ECO-001..004 (non-posting attribution ledger, canonical entities, method+confidence, GL reconciliation). |
| XIF-ECO-002 | high | Management cost allocation sits inside the financial accounting requirement with no non-posting boundary | UNB-ACC-014 | Refine UNB-ACC-014 (RR-UNB-ACC-014-r1): management attribution is a projection; only approved fee/billing (UNB-ACC-010) or CPA-approved functional allocation entries post. |
| XIF-ECO-003 | high | No reconciliation of attributed cost to GL actuals and no explicit unallocated residual | UNB-ACC-014;REQ-X-043 | UNB-XIN-ECO-004: monthly ReconciliationRun; UnallocatedResidual reported, tolerance-gated, never spread. |
| XIF-ECO-005 | high | Capacity forecasting and SLA-based staffing are still unspecified despite known December backlog arithmetic | TVF-060;TVF-162;UNB-WF-005;REQ-X-048 | UNB-XIN-ECO-017 capacity forecast with skill pools and backlog simulation; forecast accuracy tracked. |
| XIF-ECO-009 | high | Cost targets have no binding counter-metric floors | REQ-X-042;REQ-X-043;ADV-577 | UNB-XIN-ECO-015/016: floors block ratchet and trigger rollback review. |
| XIF-EVA-001 | high | Hypothesis framing 'financial and relationship management system' misstates who owns the assets | REQ-X-057;UNB-ACC-005 | Reformulate the hypothesis (see REPORT 'Hypothesis verdict'): authority facts (advisory privilege, successor designation, anonymity election, contributor of record) belong to Identity & Delegation and are append-only; r… |
| XIF-EVA-002 | high | No event catalog declares authority class or required consistency | UNB-EVT-001;UNB-GEX-001;TVR-EVT-001 | Refine UNB-EVT-001 (RR-UNB-EVT-001-r1) to require authority, consistency, ordering scope, idempotency key formula, correction pattern and retention class per event; event-matrix.jsonl is the first instance (96 events). |
| XIF-EVA-003 | high | Two candidate sources of financial truth: a generic append-only EventStore and the double-entry journal | UNB-DATA-004;UNB-TEC-STRATA-003;UNB-GEX-002;UNB-ACC-002 | Adopt UNB-XIN-EVA-001: the balanced journal is itself the financial event log; financial domain facts reference JournalBatchId; no balance is computed from any other log. |
| XIF-EVA-005 | high | Concurrent posting versus period close is unspecified | UNB-ACC-017;TX-21;ACC-ADR-011 | UNB-XIN-EVA-002: postings lock the period row FOR SHARE; close takes it FOR UPDATE; effective dates in hard-closed periods route to PriorPeriodAdjustment. |
| XIF-EVA-007 | high | Missing external files are indistinguishable from 'no activity' | UNB-ACC-018;UNB-EVT-003;UNB-CUST-013 | UNB-XIN-EVA-004: per-source completeness watermark and gap detection; dependent automation holds when a source is behind. |
| XIF-EVA-011 | high | 'Hold' is used for three legally different things | UNB-GAP-019;UNB-GEX-038;ADM-023 | UNB-XIN-EVA-010: distinct typed hold kinds with declared blocked command sets. |
| XIF-EVA-012 | high | Batch-level approval semantics would release items after a mid-batch sanctions hit | CTL-012;TVCTL-04;INV-13 | UNB-XIN-EVA-011: a batch is a container of independent item commands; each item release and each dispatch checks HoldPublicationVersion. |
| XIF-ADV-001 | critical | High-volume-low-balance model cannot be served profitably under the proposed requirement set | UNB-XIN-ERP-001;UNB-XIN-CRM-002;UNB-XIN-IAM-004;UNB-XIN-SAS-009;UNB-XIN-WFA-001;UNB-RCN-ENT-011;REQ-X-070 | Adopt UNB-XIN-ADV-001 (account servicing class with per-account recurring-obligation budget); make CRM-002 attestation and IAM-004 customer recertification event-driven or risk-banded rather than annual-for-all; compute… |
| XIF-ADV-002 | high | Institutional/outsourced sponsors in Shadow book-of-record mode have no lawful source of grantable capacity | UNB-XIN-ACC-010;UNB-XIN-EVA-001;UNB-XIN-EVA-004 | Adopt UNB-XIN-ADV-002 (Shadow-mode capacity = last reconciled external available balance at a watermark minus Unbound reservations since; stale watermark blocks capacity increases); amend EVA-001 to apply to functions d… |
| XIF-ADV-003 | high | Priority inflation: 40 new P0 plus 11 P0 refinements is not a credible release-1 set | UNB-XIN-BNK-001;UNB-XIN-PAY-003;UNB-XIN-CLM-004;UNB-XIN-ERP-001;UNB-XIN-ERP-004;UNB-XIN-CRM-001;UNB-XIN-CRM-002;UNB-XIN-CRM-003;UNB-XIN-REG-002;UNB-XIN-SAS-005;UNB-XIN-SAS-007;UNB-XIN-REC-001;UNB-XIN-ECO-021;UNB-XIN-EVA… | Apply priority-challenges.jsonl: 16 downgrades from P0 (24 remain), 20 P1->P2, 10 P2->P3; tag PAY-001/002, NPG-001/010, SVC-003, INV-002 as P0-conditional on the R1 scope decision XID-ADV-001. |
| XIF-ADV-004 | high | Configuration and approval load makes sponsor onboarding infeasible without platform default packs | UNB-XIN-BNK-001;UNB-XIN-BNK-004;UNB-XIN-BNK-007;UNB-XIN-ACC-001;UNB-XIN-ACC-002;UNB-XIN-ACC-010;UNB-XIN-INV-002;UNB-XIN-SVC-004;UNB-XIN-SVC-007;UNB-XIN-CLM-002;UNB-XIN-NPG-005;UNB-XIN-NPG-006;UNB-XIN-REG-001;UNB-XIN-REG… | Require platform-maintained, counsel/CPA-reviewed default packs that a sponsor adopts by attestation and may only tighten; per-sponsor counsel approval only for deviations. Record as XID-ADV-010. |
| XIF-ADV-005 | high | Economics stack is over-engineered and internally inconsistent with its own release-1 scope | UNB-XIN-ECO-001;UNB-XIN-ECO-002;UNB-XIN-ECO-003;UNB-XIN-ECO-004;UNB-XIN-ECO-005;UNB-XIN-ECO-006;UNB-XIN-ECO-007;UNB-XIN-ECO-008;UNB-XIN-ECO-009;UNB-XIN-ECO-010;UNB-XIN-ECO-011;UNB-XIN-ECO-012;UNB-XIN-ECO-013;UNB-XIN-ECO… | Collapse to five requirements: metering feed (merge ECO-005, ECO-009, SAS-009, BNK-005), non-posting attribution with GL reconciliation (merge ECO-001..004, P2), capacity forecast (ECO-017 absorbing SVC-008, P2), counte… |
| XIF-ADV-012 | high | Two agents specify opposite defaults for multi-advisor instruction authority | RR-ADM-023-r1;UNB-XIN-EVA-012;AUTH-I4 | Recorded as XIC-ADV-004; recommend Any-one for grant recommendations below a sponsor threshold, AllRequired for transfers-out, closure, successor and authority changes, with mode captured at fund opening. |
| XIF-ADV-013 | high | Contact details classed as mutable engagement data are used as authentication factors | UNB-XIN-EVA-014;UNB-XIN-SVC-004;UNB-XIN-IAM-003;UNB-XIN-CRM-001 | Recorded as XIC-ADV-003: authenticator bindings (registered device, verified OTP channel) are credentials owned by IAM as authority facts; changing them requires IAM-003 step-up on an existing factor plus out-of-band no… |
<!-- END GENERATED: findings-severe -->

## Contradictions

Contradictions are preserved, not silently resolved (mission §1, rule 7). Some lie between the existing requirements and this mission's findings. Others lie between the eight research passes themselves; those carry the `XIC-INT-*` IDs.

<!-- BEGIN GENERATED: conflicts -->
| Conflict | A | B | Incompatibility | Recommended resolution | Human authority |
|---|---|---|---|---|---|
| XIC-BNK-001 | UNB-RCN-FIN-013 | UNB-XIN-PAY-003 | FIN-013 rejects any 'refund' to a donor after acceptance; card network rules let the issuer reverse an accepted gift without sponsor consent. | Classify network-initiated reversals as InvoluntaryReversal outside FIN-013's scope; FIN-013 governs only sponsor-initiated value movement. | Counsel |
| XIC-BNK-002 | TVR-CON-001 | UNB-RCN-FIN-012 | Completed-gift date for cards may be the charge date while TVR-CON-001 keeps value Provisional up to 120+ days; acknowledgment gating (RR-UNB-ACC-006-r1) may then delay acknowledgments past donor filing needs. | Separate acknowledgment eligibility (completed gift + acceptance) from grantable capacity (reversal window); allow acknowledgment before window end with correction workflow per counsel. | Counsel + CFO |
| XIC-INV-001 | ACC-MDL-007 | UNB-NAV-044 | Idempotency keyed by rule version plus mandatory deterministic rebuild permits regenerating posted journals under new rules. | RR-ACC-MDL-007-r1: journals are persisted facts; rebuild applies to projections only; re-derivation is a control (UNB-XIN-ACC-003). | N |
| XIC-INV-002 | UNB-INV-019 | UNB-INV-011 | AI recommending portfolios conflicts with keeping legal/advisory classification explicit; operator lacks charitable adviser exemption. | RR-UNB-INV-019-r1 with counsel decision XID-INV-004. | Y |
| XIC-SVC-001 | RR-REQ-X-048-r1 | UNB-RCN-ENT-016 | r1 permits WorkItems only for four system causes; ENT-016 requires reconsideration cases initiated by donors/charities (also ADM-015 intake). | RR-REQ-X-048-r2 adds cause (e). | N |
| XIC-SVC-002 | RCF-PLT-025 | UNB-GEX-038 | One shared case lifecycle vs per-case retention/legal-hold/privacy policies. | Kernel + CaseProfile (UNB-XIN-SVC-002). | Y |
| XIC-CLM-001 | SM-0 | UNB-GEX-023 | Terminal objects never reopen vs metrics/flows for reopened cases (also EXC-18). | Decisions follow SM-0; work cases reopen within profile window. | N |
| XIC-ERP-001 | UNB-ACC-010 | UNB-RCN-FIN-013 | UNB-ACC-010 requires 'billing, collections' generally; collections from donors against fund fees are impossible because fees are deducted and value cannot return to donors; donor-paid fees are outside fund ledgers. | UNB-XIN-ERP-002 payer-source discriminator; collections apply only to PartnerInvoice and DonorExternal. | N |
| XIC-ERP-002 | UNB-ACC-013 | RCD-013 | UNB-ACC-013 requires native AR/invoicing; RCD-013 recommends integrating AR except partner fee billing. | Narrow UNB-ACC-013 to partner/sponsor invoices (XID-ERP-003). | Y |
| XIC-CRM-001 | ADM-008 | ONT-2 | ADM-008 allows 'household collaboration' without limits; ONT-2 states households confer no access. | RR-ADM-008-r1. | N |
| XIC-NPG-001 | UNB-RCN-GRT-001 | UNB-XIN-NPG-007 | GRT-001 rejects natural-person benefit in every tier with no override absent future counsel route; Temp. Reg. §1.530A-7T makes DAF general funding contributions to Trump accounts lawful and not natural-person distributions. | Add a separate disabled-by-default class (UNB-XIN-NPG-007); keep class (f) unchanged. | Y |
| XIC-NPG-002 | UNB-GRANT-026 | REQ-X-058 | UNB-GRANT-026 implies onboarding and acknowledgment collection for grantees; REQ-X-058 warns against forcing registration before every grant. | RR-UNB-GRANT-026-r1. | N |
| XIC-SAS-001 | RR-UNB-PLAT-008-r1 | RCD-004 | RR-UNB-PLAT-008-r1 makes Profile B (database per tenant) default and Profile A (schema per tenant) exception-only; RCD-004 recommends one database per legal sponsor with distribution partners as separate schemas by default. If tenant = par… | Close RCD-004 with tenant = legal sponsor and amend RR-UNB-PLAT-008-r1 to name partner schemas inside a sponsor database as the default partner partition. | Y |
| XIC-REC-001 | RR-REQ-X-036-r1 | RCD-007 | RR-REQ-X-036-r1: 'Expiry MUST create a disposition review task'; RCD-007 recommends class-level batches, not per-record human review. | Adopt RR-REQ-X-038-r1 (batch per class and trigger period; per-record review only for rule-flagged exceptions). | Y |
| XIC-SAS-002 | UNB-TEN-016 | UNB-SUCC-014 | UNB-TEN-016 gives each partner an exportable dataset and portability protocol; UNB-SUCC-014 and RR-UNB-WF-014-r1 say cross-sponsor portability is never automatic and transfers are sponsor distributions. A partner export of donor data by de… | RR-UNB-TEN-016-r1: export goes to the legal sponsor; partner receives only sponsor-authorized data; fund moves only via UNB-XIN-SAS-007. | Y |
| XIC-ECO-001 | UNB-ACC-014 | UNB-XIN-ECO-001 | UNB-ACC-014 places allocation methodologies inside the full accounting system (implying postable allocations); UNB-XIN-ECO-001 requires management attribution to be non-posting. | Adopt RR-UNB-ACC-014-r1: only CPA-approved functional allocations and approved billing post; management attribution is projection. | Y |
| XIC-ECO-002 | AIN-028 | UNB-XIN-ECO-009 | AIN-028 "without silently abandoning work" can be read as requiring completion beyond budget; ECO-009 stops model calls at budget. | RR-AIN-028-r1: stopping is not abandoning when state persists and work routes to a human queue with Incomplete status. | N |
| XIC-ECO-003 | REQ-X-042 | UNB-XIN-ECO-016 | REQ-X-042 Tier A permits auto-deployment of reversible non-financial UX changes; ECO-016 requires counter-metric floors before any cost-saving credit or ratchet. | Compatible if Tier A deploys may proceed but their savings are credited only after ECO-016 verification; no change to Tier A authority. | N |
| XIC-EVA-001 | UNB-DATA-004 (all current state is a projection of immutable records); architecture/TENANT-AWARE-PERSISTENCE-DESIGN.md:18 (all committed business facts immutable) | UNB-DATA-007 (erasure where lawful) | Engagement data held as immutable events cannot be erased; crypto-shredding becomes mandatory for routine CRM changes | RR-UNB-DATA-004-r1 storage classes; XID-EVA-007 | Y |
| XIC-EVA-002 | UNB-DATA-001 (no UPDATE/DELETE of committed authoritative records) | UNB-EVT-002 (durable outbox) and process-manager state (UNB-XIN-EVA-007) | Outbox delivery status and process steps require updates; literal reading forbids them | Classify as operational coordination state (RR-UNB-DATA-004-r1 class 3) | N |
| XIC-EVA-003 | REQ-X-022 / TX-11 (approval and reservation atomic); REQ-X-083 (orthogonal payment states) | Echelon generated DAF architecture: GrantApprovalRecorded consumed asynchronously by redemption, payment and accounting; PaymentRecorded single event (Echelon-AI-Native-Enterprise-Design-Framework:case-studies/daf/generated/analysis.md:160… | Generator replaces the atomic reservation with event handoffs and collapses payment knowledge states | Unbound requirements prevail; record as Echelon framework feedback: EventContract needs authority and consistency attributes | N |
| XIC-EVA-004 | GR-EVT-045..049 naming | TVR-EVT-001 naming (PaymentSubmitted, PaymentFinal, PaymentStatusUnknown, ContributionReversed) | Two catalogs name the same facts differently | Resolve in GEX-ADR-001 using event-matrix.jsonl crosswalk | N |
| XIC-EVA-005 | UNB-TEC-STRATA-003 / UNB-GEX-002 (Strata role in authoritative event persistence) | UNB-XIN-EVA-001 (journal is the financial event log; no parallel event store) | Already known as RCF-ARC-001; additionally an event store beneath the journal would duplicate financial truth | Adopt RR-UNB-PLAT-007-r1 with the journal declared authoritative | N |
| XIC-EVA-006 | COMPONENT-OWNERSHIP matrix T3 (statements as eventual projections) | UNB-XIN-EVA-003 (statements from sealed snapshots only) | Eventual projection allows non-reproducible statements | Statements are deterministic artifacts of sealed snapshots; dashboards remain T3 | N |
| XIC-ADV-001 | UNB-XIN-ACC-010 | UNB-XIN-EVA-001 | ACC-010 (INV group) allows Shadow book of record for GeneralLedger/FundAttribution where Unbound MUST NOT post; EVA-001 (EVA group, P0) requires fund balances, reservations and capacity to be computed only from Unbound's journal and forbid… | Scope EVA-001 to functions designated Authoritative under ACC-010; adopt UNB-XIN-ADV-002 for Shadow-mode capacity (reconciled external balance at a watermark minus Unbound reservations since; stale watermark blocks capacity increases). XID… | Y |
| XIC-ADV-002 | UNB-XIN-ACC-003 | UNB-XIN-EVA-001 | EVA-001 says the system MUST NOT maintain any other log from which balances can be derived or re-derived; ACC-003 requires a scheduled re-derivation of expected journals from source events. Domain facts carrying amounts (contributions, gra… | Reword EVA-001: no other log may be treated as a source of balances or posted from on replay; re-derivation under ACC-003 is a comparison control whose output never posts. | N |
| XIC-ADV-003 | UNB-XIN-EVA-014 | UNB-XIN-SVC-004;UNB-XIN-IAM-003 | EVA-014 classes contact details as mutable, erasable engagement data never used to authorize money; SVC-004 and IAM-003 use registered devices and verified channels as possession factors for disclosure and money-direction changes. | Authenticator bindings are IAM credentials stored as authority facts (append-only, step-up to change, notice to prior channel); ordinary contact details stay engagement data. A contact-detail change never changes an authenticator implicitl… | N |
| XIC-ADV-004 | RR-ADM-023-r1 | UNB-XIN-EVA-012 | RR-ADM-023-r1 and XID-TRS-002 (ERP/TRS group) default co-advisor authority to AllRequired; XID-EVA-006 (EVA group) recommends Any-one for grants and All only for transfers-out, closure and allocation, and EVA-012 resolves conflicts by that… | One default table by instruction kind: grants below sponsor threshold Any-one (conflicts -> EVA-012 HeldConflict); grants above threshold, transfers-out, closure, successor and authority changes AllRequired; mode captured at fund opening a… | Y |
| XIC-ADV-005 | UNB-XIN-ACC-005 | UNB-XIN-EVA-002 | EVA-002 routes any posting dated in a hard-closed period to a PriorPeriodAdjustment in the first open period; ACC-005 posts audit findings into an AuditAdjustment period of the prior fiscal year. XID-EVA-002 is open. | AuditAdjustment period stays soft-closed until audit sign-off and accepts only AuditFinding-linked journals; after the audited statements are issued, EVA-002 PPA applies. Period 13 hard-closes at sign-off. | Y |
| XIC-ADV-006 | UNB-XIN-ERP-003 | UNB-XIN-ECO-012 | ECO-012 derives RevenueRecords only from posted revenue including contract/SaaS fees; ERP-003 forbids posting operator charges to tenant ledgers. Profitability by business model mixes sponsor and operator P&Ls. | Split ECO-012 into sponsor-view (sponsor GL) and operator-view (operator billing system plus SAS-009 usage costs) margins; neither posts to tenant ledgers. | Y |
| XIC-ADV-007 | UNB-XIN-IAM-002 | UNB-XIN-IAM-005 | IAM-002 permits operator access only under sponsor delegation or per-case sponsor approval and requires visibility within 15 minutes; IAM-005 defines break-glass without sponsor approval and tenant notice within 1 hour. | Add break-glass as the third enumerated basis in IAM-002, bounded by IAM-005; transparency view within 15 minutes for all operator sessions including break-glass, active notification within 1 hour. | N |
| XIC-ADV-008 | UNB-XIN-SAS-007 | UNB-XIN-EVA-011 | SAS-007 allows sponsor A to decide exit transfers by a sponsor-approved batch policy; EVA-011 forbids batches carrying approval authority. | A batch policy is a per-item decision rule: each transfer item is decided, hold-checked and committed individually with its own decision record citing the policy version. | Y |
| XIC-ADV-009 | UNB-XIN-ERP-001 | XID-ERP-001;UNB-XIN-EVA-013 | ERP-001 (P0) mandates daily accrual per fund while the billing basis decision XID-ERP-001 is open; if accruals are posted daily (TX-20 as cited in the EVA consistency table) the EVA volume test that justifies EVA-013's no-broker threshold… | Accrue daily by computation; post at assessment; capacity = journal balance - reservations - accrued-unposted fees; restate EVA volume table. | Y |
| XIC-ADV-010 | UNB-XIN-IAM-003 | UNB-XIN-IAM-001;UNB-XIN-SAS-006 | IAM-003 forbids delegating step-up to a partner IdP; IAM-001 defines a federated PartnerWorkforce realm and the advisor-distribution model relies on partner federation, so every advisor action above threshold requires a second sponsor-cont… | Permit partner-IdP step-up where the partner agreement asserts the required AAL, the assertion carries amr/acr evidence verified per command, and the sponsor approves the partner; sponsor-controlled verifier remains mandatory for payee and… | Y |
| XIC-ADV-011 | UNB-XIN-ECO-013 | UNB-XIN-ECO-022;XRJ-ERP-002 | ECO's own report says simulation runs offline in Echelon first and release 1 is six metrics plus a pack; ECO-013 requires an in-platform ScenarioRun and ECO-022 ten dashboards including Scenario; XRJ-ERP-002 rejected native budgeting/forec… | Reject ECO-013 (XRJ-ADV-001); cut ECO-022 to the six R1 metrics and monthly pack. | N |
| XIC-INT-001 | UNB-XIN-SAS-005 | UNB-XIN-EVA-006;RR-UNB-EVT-001-r1 | Two event envelopes are specified: SAS-005 (TenantId, LegalSponsorId, ProgramId, tenant-scoped sequence, CorrelationId, CausationId) and EVA-006 (correlation_root, causation_id, business_keys, parent_root) plus RR-UNB-EVT-001-r1 (authority… | Adopt one envelope: tenant fields from SAS-005; correlation_root, parent_root, causation_id and business_keys from EVA-006; authority_class from RR-UNB-EVT-001-r1. CorrelationId in SAS-005 is renamed correlation_root. Tenant-scoped sequenc… | N |
| XIC-INT-002 | UNB-XIN-ERP-007 | RR-UNB-EVT-001-r1;UNB-XIN-EVA-016 | ERP-007 classifies events as AuthoritativeFinancial / AuthoritativeAuthority / Informational; the event matrix and RR-UNB-EVT-001-r1 use authoritative-financial / authoritative-nonfinancial / derived / informational / observation-ai. Two v… | Use the event-matrix vocabulary (five classes). Map AuthoritativeAuthority to authoritative-nonfinancial. ERP-007's substantive rule (only the fee engine emits authoritative fee events) is retained. | N |
| XIC-INT-003 | UNB-XIN-SVC-002 | UNB-XIN-EVA-010;UNB-XIN-NPG-006;UNB-XIN-CLM-004 | Holds are specified in three places: EVA-010 typed hold model, NPG-006 discretionary hold governance, CLM-004 restricted investigation cases (which may place holds), and SVC-002's case kernel references holds through case profiles. Without… | Holds are domain records of EVA-010 types only. A case (SVC-002/CLM-004) may request or reference a hold but never is one; NPG-006 discretionary holds are the EVA-010 'DiscretionaryHold' type with NPG-006 governance fields. | N |
<!-- END GENERATED: conflicts -->

## Duplicate requirements reconciled

The eight research passes were independent, so some overlap was expected. Duplicates were found by token similarity screening of every cross-pass pair of statements, followed by manual review by topic. For each set, the first ID listed is canonical. `graph.py` emits a `duplicates` edge from each overlapping ID to the canonical one.

<!-- BEGIN GENERATED: duplicates -->
| Finding | Canonical, then overlapping | Topic | Resolution |
|---|---|---|---|
| XIF-INT-001 | UNB-XIN-CRM-003;UNB-XIN-REG-003 | CAN-SPAM/TCPA message classification | CRM-003 is canonical (it adds sender legal entity and send-time suppression keyed by sender). Fold REG-003's counsel-approval and revocation-handling clauses into CRM-003 at adoption; REG-003 is retained only as a traceability alias. Priority: P0 only for SMS/voice (statutory damages); email classification P1 (see XID… |
| XIF-INT-002 | UNB-XIN-BNK-007;UNB-XIN-NPG-011 | unclaimed property for uncashed grant checks | Keep both: BNK-007 is canonical for the rule table and lifecycle; NPG-011 is canonical for jurisdiction selection and MUST be referenced by BNK-007 as its jurisdiction function. One scenario must exercise both (XDS-BNK and XDS-NPG escheat scenarios are linked). |
| XIF-INT-003 | UNB-XIN-EVA-009;UNB-XIN-SAS-007 | bulk movement of funds between legal sponsors | EVA-009 is the mechanism (process manager, per-fund DAF transfer, no LegalSponsorId update); SAS-007 is the case and donor-election front end that invokes it. Adoption must state SAS-007 depends_on EVA-009 and share one reconciliation identity (elected = transferred + declined + remaining). |
| XIF-INT-004 | UNB-XIN-ECO-017;UNB-XIN-SVC-008 | seasonal capacity and December staffing forecast | ECO-017 is canonical. SVC-008's channel-specific inputs (Erlang-C for live channels, callback conversion) become a method option inside ECO-017 for live-channel pools only. |
| XIF-INT-005 | UNB-XIN-EVA-001;UNB-XIN-ACC-003 | journal authority over derived re-derivation | Complementary, not redundant: EVA-001 canonical for 'single financial log'; ACC-003 canonical for the re-derivation control. Both must be adopted together; neither alone closes the replay-reposting risk (RR-ACC-MDL-007-r1). |
| XIF-INT-006 | UNB-XIN-EVA-007;UNB-XIN-WFA-001;UNB-XIN-WFA-002 | durable process managers, timers and in-flight versioning | EVA-007 canonical for the process-manager model; WFA-001 and WFA-002 are retained as its timer and versioning clauses. Adoption merges them into one specification with one test suite. |
| XIF-INT-007 | UNB-XIN-REG-001;UNB-XIN-OTH-003;UNB-XIN-ERP-004 | funds-flow title and exclusion of partners from the money path | REG-001 canonical for title; OTH-003 is its payment-path corollary and should be merged into REG-001 at adoption. ERP-004 is distinct (payee registry separation) and stays separate. |
| XIF-INT-008 | UNB-XIN-ECO-002;UNB-XIN-SAS-009;UNB-XIN-BNK-005;UNB-XIN-ECO-005;UNB-XIN-ERP-006 | cost and usage metering feeds | ECO-002 is the canonical model. SAS-009, BNK-005, ECO-005 and ERP-006 are feeds that MUST map into ActivityEvent/VendorCostRecord with one shared cost-object key set (tenant, program, fund, grant, case, AI run). Field names must be unified at adoption (XIC-INT-002). |
| XIF-INT-009 | UNB-XIN-EVA-014;UNB-XIN-CRM-001 | party identity and relationship data classes | Complementary: EVA-014 canonical for authority vs engagement classification; CRM-001 canonical for PII partitioning by capacity. Adopt together; the identity core is an authority-class record. |
<!-- END GENERATED: duplicates -->

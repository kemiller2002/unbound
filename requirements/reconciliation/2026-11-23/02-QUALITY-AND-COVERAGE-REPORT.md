# 02 — Requirements quality and coverage report

Date: 2026-11-23 | Denominator: **1,148 canonical requirement identifiers** (`data/requirements-quality.csv`), plus 132 unlabelled normative statements (`data/narrative-requirements.csv`). Findings: [`data/findings.csv`](data/findings.csv).

## 1. How the audit was done

1. **Automated heuristics** over every canonical requirement (`tools/reqaudit/quality.py`): RFC 2119 keyword present; vague terms; ≥8 clauses (compound); acceptance signal (Given/When/Then or test wording, or cited by a test or scenario). These are triage signals only.
2. **Semantic audit** by four role-separated domain passes (GRT grants, charities and payments; FIN finance and investments; PLT platform, AI, UI and governance; ENT enterprise, donor lifecycle and operating model) against a common brief and the 14 mission dimensions. Each pass read its files in full, checked evidence, and recorded only material defects confirmed by reading.
3. **Integrating review** added the regulatory (REG) and architecture (ARC) findings, merged overlapping records, resolved inconsistencies between passes (for example the tenancy recommendation; see [03](03-CONFLICT-REGISTER.md)), and spot-verified every critical finding against the source.

**Limits.** All passes are the same AI model family, so this is not independent human review. No finding has been confirmed by counsel or the owner. A requirement with no finding is *not reviewed as defective*; that is not the same as *validated*.

## 2. Heuristic signals (all 1,148)

| Signal | Count | Share |
|---|---:|---:|
| No RFC 2119 keyword (MUST/SHALL/SHOULD/MAY) | 914 | 79.6% |
| No normative verb of any kind | 734 | 63.9% |
| No acceptance signal (no Given/When/Then, no test wording, not cited by a test) | 902 | 78.6% |
| … and no test record anywhere in the same file | 522 | 45.5% |
| Contains vague terms ("support" ×122 is the most common) | 184 | 16.0% |
| Compound (≥8 clauses: several requirements in one) | 209 | 18.2% |
| Cited by ID in any test or scenario record | 24 | 2.1% |

The prevailing style is an imperative noun phrase ("Support X, Y and Z with …"). Two competent teams could implement such text differently and both claim compliance, which is the precision failure the mission asks about. The `REQ-X` overlay is better on acceptance (82 of 84 carry an acceptance paragraph) but worst on compounding: 89 of 153 research-area requirements exceed 8 clauses.

## 3. Coverage of the charter's twelve domains

Families are mapped to charter domains by name.

| Charter domain | Canonical requirements | Assessment |
|---|---:|---|
| UNB-ENT enterprise, economics, decision rights | 25 | Thin. No release boundary (RCF-ENT-003); the three-operating-model comparison was never done (RCF-ENT-004). |
| UNB-LEG law, authority | 37 | Mostly authority mechanics (GOV-AUTH). Legal obligations live as controls, not requirements (RCF-FIN-001/003/005, RCF-GRT-005/006). |
| UNB-DON donor lifecycle | 25 | **Weak.** Onboarding, dormancy, closure, outbound transfer and households are one-liners or absent (RCF-ENT-019..025). |
| UNB-GRT grants and charities | 261 | Largest, and duplicative. Core §4966/§4958 rules missing. Payment states inconsistent across six vocabularies (RCF-GRT-011). |
| UNB-AST assets and investments | 125 | Strong on NAV and unitization. Weak on complex-asset obligations (8282, §4943) and valuation dates. |
| UNB-FIN accounting | 50 | Over-scoped (full ERP, RCF-FIN-008). Missing per-fund balancing and fee base. |
| UNB-ADV advisors | **2** | **Weakest.** Advisor rules are scattered across ADM, REQ-X-062 and UNB-INV; compensation exposure is unaddressed (RCF-ENT-023, RCF-FIN-006). |
| UNB-AIO AI and agents | 47 | Good intent. Four autonomy scales and duplicated IDs (RCF-PLT-019). |
| UNB-OPS operations and controls | 159 | Large GRC surface (RCF-PLT-035); the control catalog is templated (RCF-PLT-031/032). |
| UNB-TEC architecture, security, data | 121 | Strata category error (RCF-ARC-001). Tenancy contradictions (RCF-PLT-001..006). |
| UNB-EXP experience | 53 | Department-centric; missing Unknown/Failed states; no charity search. |
| UNB-VAL validation | 98 | Traceability requirements exist, but tests do not cite requirements (report 01 §7). |
| REQ-X overlay / TVR | 84 / 48 | Proposed cross-domain requirements, not yet migrated into domains. Three duplicated IDs (REQ-X-041/054/055). |

## 4. Findings by domain and severity

<!-- BEGIN GENERATED: findings-matrix -->
| Domain | critical | high | medium | low | Total |
|---|---|---|---|---|---|
| GRT | 3 | 18 | 18 | 1 | 40 |
| FIN | 1 | 12 | 15 | 8 | 36 |
| PLT | 2 | 21 | 23 | 2 | 48 |
| ENT |  | 18 | 16 | 3 | 37 |
| REG |  | 2 | 5 | 3 | 10 |
| ARC | 1 |  | 3 |  | 4 |
| **Total** | 7 | 71 | 80 | 17 | 175 |
<!-- END GENERATED: findings-matrix -->

409 of the 1,148 canonical requirements (35.6%) are named in at least one finding (ranges such as `UNB-SUCC-001..014` expanded).

## 5. Findings by mission dimension

<!-- BEGIN GENERATED: findings-dimension -->
| Dimension | Findings |
|---|---|
| completeness | 44 |
| consistency | 30 |
| necessity | 23 |
| correctness | 21 |
| precision | 13 |
| financial-integrity | 9 |
| testability | 8 |
| traceability | 7 |
| security | 6 |
| architecture | 6 |
| automation | 4 |
| usability | 3 |
| operability | 1 |
<!-- END GENERATED: findings-dimension -->

How to read the dimensions:
- **Correctness**: legal or factual error, for example the 180-day currency rule, refund paths, and Strata capabilities.
- **Consistency**: contradictions, detailed in report 03.
- **Completeness**: missing scenarios or obligations, answered by the gap requirements in report 04.
- **Necessity**: duplicates and scope that serves no obligation or outcome. Examples: full ERP, GRC product, visual email designer, IVR, 1,892 keyword links.
- **Precision and testability**: vague or compound requirements.
- **Automation and usability**: human work and friction that could be removed (report 09).
- **Security, financial integrity, operability, architecture**: as named.

## 6. Critical and high findings

<!-- BEGIN GENERATED: findings-severe -->
| ID | Sev. | Dimension | Finding | Affected | Proposed resolution |
|---|---|---|---|---|---|
| RCF-ARC-001 | critical | architecture | Strata assigned capabilities it does not have | UNB-PLAT-007;UNB-ACC-026;UNB-KNOW-014;UNB-CHAR-040;UNB-INV-042;UNB-NAV-051 | Adopt RR-UNB-PLAT-007-r1: an Unbound-owned Ledger/Event Store on managed PostgreSQL, with Strata as the schema-governance and drift layer. |
| RCF-FIN-021 | critical | correctness | 'Refund' paths could distribute accepted DAF assets to donors | REQ-X-070;REQ-X-023;ACC-MDL-004 | Add UNB-RCN-FIN-013: after acceptance, value leaves a fund only through the grant lifecycle to an eligible grantee or a governed transfer. 'Refund' is restricted to pre-acceptance returns of non-comp… |
| RCF-GRT-005 | critical | completeness | No requirement defines the recipient classes that make a DAF distribution taxable | UNB-GRANT-023;UNB-CHAR-020;UNB-GRANT-041;REQ-X-007;REQ-X-018 | Add a RecipientClass taxonomy and a deterministic TaxableDistributionGate (see UNB-RCN-GRT-001). Each class maps to one of: route permitted, ER route required, prohibited, or counsel review. Classifi… |
| RCF-GRT-006 | critical | completeness | No requirement blocks payments that benefit a donor, advisor or related person (IRC 4958(c)(2)) | UNB-GRANT-011;UNB-PAY-005;REQ-X-005;REQ-X-018 | Add UNB-RCN-GRT-002: a deterministic gate that compares the payee legal entity, destination owner and declared beneficiaries with the related-party registry. The result is a nonwaivable hold routed t… |
| RCF-GRT-013 | critical | completeness | A true sanctions match has no Blocked outcome, so the money would follow return or retry paths | REQ-X-083;REQ-X-007;UNB-EXT-004;GR-EVT-020 | Add Blocked to the provider-knowledge and settlement dimensions and add a BlockedProperty ledger classification. Release is allowed only by license evidence and independent compliance authority (UNB-… |
| RCF-PLT-001 | critical | consistency | One-system invariant contradicts per-partner dedicated infrastructure | UNB-PLAT-008;UNB-TEN-002;REQ-X-057;REQ-X-020;UNB-ACC-027;UNB-NAV-052 | Define 'one system' as: one codebase, one domain model, one release version applied to all deployments, one command/policy/ledger implementation. Define a 'deployment cell' (one runtime instance plus… |
| RCF-PLT-002 | critical | financial-integrity | Global charity hold has no serialization point across partner databases | REQ-X-020;UNB-KNOW-012;UNB-TEN-002;UNB-PLAT-008 | Add a requirement: every global hold has a monotonically increasing hold-publication version; each cell's release gate must read the hold registry synchronously (fail closed on unavailability) or ver… |
| RCF-ENT-001 | high | consistency | Mandatory Conditor bootstrap never recorded, yet all later mission work proceeded | AGENT-MISSION step 0;AGENT-MISSION step 1 | The owner records one of two decisions: (a) a dated waiver allowing requirements-only work before bootstrap, scoped and justified, with step 0 amended; or (b) a bootstrap that is executed and reporte… |
| RCF-ENT-002 | high | completeness | No requirements-gate PASS/FAIL record; gate criteria have no owner or measurable thresholds | CHARTER Requirements gate;AGENT-MISSION step 10 | Publish a gate record that states the current status (FAIL), has one row per CHARTER criterion (owner, measure, denominator, evidence path, status), and names the gate approver. Add the TR-1..TR-6 ga… |
| RCF-ENT-003 | high | necessity | No MVP or release boundary: the requirement set targets the whole DAF industry at once | EXPANSION roadmap tracks R1 to R8;REQ-X-056;REQ-X-058;REQ-X-063;REQ-X-070;REQ-X-074;REQ-X… | Define a release-1 boundary as an outcome slice: one legal sponsor, cash and public-securities contributions, domestic grants to eligible public charities, multi-participant funds, succession, closur… |
| RCF-ENT-004 | high | consistency | 'Operating model' has three incompatible meanings, and the CHARTER-mandated three-model comparison never happened | CHARTER principle 4;CHARTER backlog 4;UNB-COM-001;REQ-X-057 | Adopt three distinct terms: autonomy model (CHARTER), sponsor business model (UNB-COM), service/channel model (REQ-X-057). The owner then either runs the CHARTER comparison as configurations inside t… |
| RCF-ENT-006 | high | consistency | Unbound-branded public identity contradicts the white-label, multi-sponsor platform and mandatory legal-sponsor disclosure | UNB-EXP-002;REQ-X-057;INV-18;UNB-COM-004 | Split the brand into platform name, reference-sponsor name (synthetic) and partner sponsor brand. Make the legal-sponsor disclosure a non-configurable element. See RCC-ENT-001. |
| RCF-ENT-007 | high | traceability | Workflow IDs WF-nn have at least four incompatible namespaces | UNB-WF-013;UNB-WF-015;UNB-WF-017;REQ-X-078;REQ-X-079;INV-07;ADM WF-01..WF-16 | Introduce qualified namespaces (LS-WF, E2E-WF, ADM-WF, CW) with a crosswalk, and reject bare WF-nn in new text. See UNB-RCN-ENT-003. |
| RCF-ENT-008 | high | necessity | Duplicate: UNB-GAP-001..009 defined twice with different content; GAP-001 has four meanings | UNB-GAP-001;UNB-GAP-002;UNB-GAP-003;UNB-GAP-004;UNB-GAP-005;UNB-GAP-006;UNB-GAP-007;UNB-G… | Renumber Wave 1 items as UNB-VBLK-001..009 (validation blockers). Prefix landscape gaps LSGAP-001..020. Keep UNB-GAP-001..034 for the Enterprise Gap Register. Publish a redirect table. |
| RCF-ENT-009 | high | traceability | Control crosswalks omit the BASELINE, participant, workflow, cross-domain and REQ-X requirement families | UNB-LEG-002;UNB-ENT-001;UNB-PART-001;UNB-WF-013;UNB-XSC-004;REQ-X-057;REQ-X-079 | Extend the crosswalk to every requirement family, with gap_or_review rows where no control exists. Make 'no control mapped' a gate-blocking status for P0 legal and financial requirements. |
| RCF-ENT-010 | high | testability | Most enterprise-domain 'requirements' lack the CHARTER record fields, and many are backlog topics rather than requirements | UNB-LS-001..017;UNB-WF-001..024;UNB-GAP-001..034;UNB-REM-001..015;UNB-HYP-001..010;REQ-X-… | Reclassify UNB-GAP, UNB-REM and UNB-LS items as backlog epics or outcomes with status 'not a requirement'. Require all CHARTER record fields before any item takes the status 'candidate requirement'.… |
| RCF-ENT-011 | high | necessity | Duplicate: succession and donor-death rules are specified in eight places without a canonical owner | UNB-SUCC-001..014;UNB-WF-013;UNB-GAP-015;REQ-X-008;REQ-X-068;ADM-007;ADM-022;UNB-XSC-004 | Make UNB-SUCC the canonical family. Mark the others 'refined-by UNB-SUCC' and remove normative text from them. Adopt RR-UNB-SUCC-008-r1 and RR-REQ-X-008-r1. |
| RCF-ENT-014 | high | correctness | KYC is used as a fund-opening gate without determining whether it is law, contract or sponsor policy | UNB-PART-007;ADM-009;E2E WF-02;UNB-GAP-014;REQ-X-072 | Needs counsel. Replace 'KYC' with four separately classified obligations, each with an authority type (statute/regulation, contract, sponsor policy, risk control) and an owner. Define failure outcome… |
| RCF-ENT-015 | high | correctness | REQ-X-008 adopts as normative the answer to open owner decision OD-03, and is ambiguous for multi-participant funds | REQ-X-008;OD-03 | Make the pending-grant disposition a sponsor policy parameter, death_pending_grant_policy in {complete, hold_for_review, cancel}, with no default until OD-03 is answered. Scope suspension to rights t… |
| RCF-ENT-016 | high | security | Former advisory authority stays live from verified death until succession activation | UNB-SUCC-008;UNB-SUCC-006;UNB-WF-011 | Separate the three steps: protective hold on any notice (new recommendations only), suspension on verified death, and successor activation later. See RR-UNB-SUCC-008-r1 and UNB-RCN-ENT-004. TVF-187 a… |
| RCF-ENT-017 | high | financial-integrity | Fractional successor allocations lack a rounding, valuation-date, unit and residual rule, so 'reconcile exactly' cannot be tested | UNB-SUCC-004;UNB-SUCC-009 | Adopt the deterministic allocation rule in RR-UNB-SUCC-004-r1. Make the valuation point and residual rule explicit sponsor parameters with a CFO owner. |
| RCF-ENT-020 | high | completeness | REQ-X-078's '18 enterprise workflows' omit fund closure, dormancy, outbound and inbound transfer, fund split/merge and sponsor succession | REQ-X-078;UNB-WF-014;UNB-WF-015;ADM-025 | Add E2E workflows for closure, dormancy, outbound transfer, inbound transfer, split/merge and sponsor succession, each to the REQ-X-078 transition standard. See UNB-RCN-ENT-005..008. |
| RCF-ENT-023 | high | correctness | Paying a donor's personal investment advisor from fund assets is treated as a fee-disclosure topic, not a legal-applicability gate | REQ-X-062;CTL-025;UNB-ADV-001;REQ-X-067 | Needs counsel. Do not activate fees paid from fund assets to any party with advisory privileges or a personal advisory relationship with the donor until a counsel-approved rule exists. Track the prop… |
| RCF-ENT-027 | high | financial-integrity | Migration imports standing data (payees, successors, advisors) without required provenance or verification state, and has no rollback boundary | REQ-X-061;INV-07;E2E WF-17 | TVF-208, TVF-217 and TVF-244 remain open and REQ-X-061 has not resolved them. Adopt RR-REQ-X-061-r1. |
| RCF-ENT-029 | high | completeness | Schedule D DAF reporting and account-type classification are not specified; §6033(k) is not cited in any requirement | REQ-X-060;UNB-GAP-024;UNB-WF-020;REQ-X-070;REQ-X-075 | Needs CPA and counsel. See UNB-RCN-ENT-011 and UNB-RCN-ENT-012, and RR-REQ-X-060-r1. |
| RCF-FIN-001 | high | completeness | No normative requirement for DAF contribution acknowledgment content or timing | UNB-ACC-006;UNB-ACC-025;REQ-X-009;REQ-X-059;ADM-010;UNB-GAP-010 | Add UNB-RCN-FIN-001: acknowledgment issuance is a gated command whose content schema is versioned and counsel-approved per sponsor; issuance is blocked when any mandatory element is missing; acknowle… |
| RCF-FIN-003 | high | completeness | Form 8283 donee signature and Form 8282 disposition reporting are not requirements | REQ-X-059;UNB-INV-012;UNB-ACC-006;UNB-GAP-012;UNB-ACC-021 | Add UNB-RCN-FIN-002 and UNB-RCN-FIN-003: signing a Form 8283 Section B creates a tracked donee-acknowledgment record; any disposition event on the same asset lot within the statutory window creates a… |
| RCF-FIN-005 | high | completeness | Excess business holdings (IRC 4943 as applied by 4943(e)) are not monitored | REQ-X-059;UNB-INV-012;UNB-INV-014;UNB-CUST-006 | Add UNB-RCN-FIN-004: maintain business-enterprise identity and ownership for related persons; test at gift acceptance and on every related transaction; track the disposal period with escalation. Coun… |
| RCF-FIN-006 | high | completeness | Advisor compensation paid from DAF assets has no IRC 4958 or proposed-regulation screen | UNB-INV-006;UNB-CUST-014;UNB-INV-036;REQ-X-067 | Add UNB-RCN-FIN-010: advisor engagement requires a recorded relationship classification (donor's personal adviser or not) and a counsel-approved compensation policy before the first fee accrual; stat… |
| RCF-FIN-008 | high | necessity | Full sponsor ERP scope (AP, AR, budgeting, payroll, fixed assets, consolidation, FX translation) is not needed for DAF administration | UNB-ACC-004;UNB-ACC-012;UNB-ACC-013;UNB-ACC-014;UNB-ACC-015;UNB-ACC-016;UNB-ACC-019;UNB-A… | Owner decision: (1) native and authoritative: DAF fund subledger and the DAF-program GL accounts, posting engine, reconciliation, close, fund statements, Schedule D source data; (2) integration: bala… |
| RCF-FIN-009 | high | architecture | Requirements attribute ledger, append-only, temporal and replay guarantees to Strata, a schema management tool | UNB-ACC-026;UNB-CUST-013;UNB-INV-042;UNB-NAV-044;UNB-NAV-051;ACC-MDL-011;UNB-PLAT-007 | Supersede the Strata wording: the ledger component provides append-only posting, correction links, bitemporal queries and integrity checkpoints on PostgreSQL (privilege revocation for UPDATE/DELETE/T… |
| RCF-FIN-010 | high | financial-integrity | Illustrative fund-to-fund transfer is not balanced per fund | ACC-MDL-003;UNB-ACC-002;UNB-ACC-011;UNB-SUCC-009 | Declare fund (and restriction) as balancing segments; each journal must balance per entity x currency x fund. Transfers post Dr origin net assets (or 3900 transfer-out) / Cr origin cash and Dr succes… |
| RCF-FIN-013 | high | financial-integrity | Fee base undefined: fee-on-fee, double charging and fee timing are not testable | UNB-NAV-032;UNB-NAV-033;UNB-INV-036;UNB-ACC-010;REQ-X-067 | Add UNB-RCN-FIN-005 (fee base contract). Every fee accrual records basis type, base amount, exclusions applied, NAV version, tier method and minimum; deterministic recomputation must match within zer… |
| RCF-FIN-014 | high | completeness | Grant lifecycle still has no funding or liquidation states between reservation and payment | UNB-GFUND-011;UNB-GFUND-014;UNB-GFUND-015;UNB-CUST-015;UNB-INV-030 | Add UNB-RCN-FIN-008: states FundingPlanned, LiquidationPending, Funded, FundingShortfall; PaymentInstructionAuthorized requires Funded (settled cash allocated to the grant), unless a sponsor-approved… |
| RCF-FIN-015 | high | architecture | Capacity is defined as a projection but reservations need a transactional check | ACC-MDL-004;REQ-X-084;REQ-X-022;UNB-NAV-044;UNB-GFUND-021 | Add UNB-RCN-FIN-009: the reservation command computes capacity inside the same serializable transaction from authoritative ledger and reservation rows, with deterministic lock order across ancestors.… |
| RCF-FIN-016 | high | completeness | No allocation method for non-unitized omnibus accounts | UNB-INV-002;UNB-INV-026;REQ-X-023 | Add UNB-RCN-FIN-007: an omnibus account must declare its attribution method (per-fund lot sub-accounting, or unitized). Lot sub-accounting requires every position and lot to carry a fund owner and su… |
| RCF-FIN-020 | high | consistency | Returned-contribution treatment contradicts itself and presumes a donor receivable | REQ-X-009;REQ-X-084;FI-23 | Replace 'receivable' with a neutral 'returned-contribution exposure' fact; the posting is chosen by a CPA-approved policy (reversal of contribution, receivable with allowance, or sponsor loss) and th… |
| RCF-GRT-001 | high | correctness | 180-day charity revalidation is stated as a platform minimum although its only source calls it an unvalidated sponsor policy proposal | REQ-X-007;REQ-X-018;REQ-X-058;TX-27;SCV-03 | Classify the interval as platform compliance policy (authority type: policy, not statute). Make it a governed configuration parameter, CommonVerificationMaxAge, defined per evidence fact type, with a… |
| RCF-GRT-002 | high | consistency | One universal 180-day validity contradicts the rule that freshness is fact-, rail- and jurisdiction-specific | REQ-X-018;REQ-X-007;UNB-RULE-015;UNB-KNOW-025 | Replace the single validity with a typed freshness table: fact type, maximum age, monitoring source, execution-time recheck (yes or no) and owner. Then 180 days becomes, at most, the age limit for th… |
| RCF-GRT-003 | high | automation | Calendar expiry of vetting is modelled as a blocking hold rather than an automatic refresh | TX-27;TX-18;REQ-X-018;REQ-X-019 | Separate the states ReverificationDue, RefreshInProgress, Expired and HeldAdverse. Start a deterministic refresh at a configurable lead time. Hold grants only when evidence has passed its maximum age… |
| RCF-GRT-007 | high | correctness | Gate G05 treats every pledge as prohibited, which contradicts IRS reliance guidance and real differences between sponsors | REQ-X-018;UNB-GRANT-011;INV-16 | Move pledge handling from the global G05 to a sponsor L3 policy with values Prohibited, NonBindingOnly or Notice2017-73Conditions. The baseline is approved by counsel. If a sponsor chooses Notice2017… |
| RCF-GRT-008 | high | correctness | Remittance and external-ID features can break the Notice 2017-73 condition that the sponsor must not mention a pledge | REQ-X-077;REQ-X-063;REQ-X-058;UNB-GRANT-021 | Classify outbound remittance fields as sponsor statements. Apply a deterministic filter that drops pledge identifiers and pledge vocabulary from sponsor-issued remittance when sponsor policy relies o… |
| RCF-GRT-009 | high | completeness | The rules for capturing donor affirmations were never carried into the requirements | UNB-GRANT-011;UNB-GRANT-002;REQ-X-063;TX-10;REQ-X-018 | Adopt UNB-RCN-GRT-010, an AffirmationEvidence contract that G05 and TX-10 reference. |
| RCF-GRT-010 | high | completeness | What happens to a recurring occurrence held after an eligibility change is not specified | UNB-GRANT-027;UNB-GRANT-003;REQ-X-008;TX-18;GRANT-T06 | Adopt UNB-RCN-GRT-004. By default a held occurrence is skipped once a configurable hold window expires, with no automatic catch-up. Each occurrence has its own idempotency key (schedule version plus… |
| RCF-GRT-011 | high | consistency | There are six incompatible payment-state vocabularies, and TX-13 merges Unknown with Accepted | UNB-GRANT-022;UNB-PAY-001;UNB-PAY-009;UNB-GEX-031;REQ-X-019;REQ-X-083;TX-13 | Declare REQ-X-083 the canonical model. Mark UNB-GRANT-022, UNB-PAY-001 and UNB-PAY-009 as superseded and TYPED-DECISION contract 3 as derived. Split TX-13 into the transitions DispatchOutcomeUnknown… |
| RCF-GRT-012 | high | completeness | Uncashed, stale-dated, voided and escheated checks have no lifecycle | UNB-PAY-006;REQ-X-083;CTL-044 | Adopt UNB-RCN-GRT-005: the states Outstanding, AgedOutreach, StaleDated, VoidRequested, VoidConfirmed and UnclaimedPropertyReportable. Each state has a ledger effect and a reissue chain, and the agin… |
| RCF-GRT-014 | high | precision | The sanctions screening scope is undefined: which subjects, which lists, when, and what rescreens in-flight items | UNB-EXT-004;REQ-X-007;UNB-RULE-014;UNB-GRANT-009 | Adopt UNB-RCN-GRT-007, a ScreeningScope matrix (subject x list x trigger x threshold owner) approved by counsel. A list update rescreens every unreleased payment chain. |
| RCF-GRT-015 | high | financial-integrity | Atomic 'currency reservation' does not say how to reserve grants that are funded by selling investments | REQ-X-002;UNB-GFUND-011;UNB-GFUND-021;UNB-GRANT-017;TX-11 | Define the reservation unit for each funding source: cash amount, units with a valuation haircut, or a sale before approval. Add an ApprovedPendingLiquidity state that creates no external commitment,… |
| RCF-GRT-017 | high | operability | There is no defined way for an Unknown payment to ever be resolved | REQ-X-004;REQ-X-083;UNB-PAY-004;P-04 | Adopt UNB-RCN-GRT-008: an Unknown resolution protocol with rail-specific evidence combinations, an aging schedule, independent approval to declare NotReceived, and a mandatory linked reissue chain. T… |
| RCF-GRT-018 | high | automation | The destination-proof standard does not name any qualifying evidence, so it may make every automated payment impossible | REQ-X-005;REQ-X-018;P-02;SCV-05 | Publish a per-rail DestinationEvidence table, owned by Treasury, that classifies named evidence types into E-classes and gives the minimum combination for automated release. Treat it as configuration… |
| RCF-GRT-023 | high | consistency | The grant requirements assume a human approves grants and grant modifications; REQ-X-017 assumes rules approve routine cases | UNB-GRANT-012;UNB-GRANT-048;REQ-X-017;REQ-X-001 | Restate UNB-GRANT-012 and UNB-GRANT-048 as: approval authority is either board-delegated policy (once OD-01 is validated) or an independent human. A material modification invalidates approval and is… |
| RCF-GRT-024 | high | consistency | Sponsor eligibility overrides (UNB-CHAR-023) conflict with the rule that sponsors cannot lower common vetting | UNB-CHAR-023;REQ-X-018;REQ-X-007 | See RCC-GRT-003: allow sponsor-supplied evidence to be added to the common record through the alternate-verification path (UNB-RCN-GRT-013). Never allow a sponsor to override a common adverse or Inel… |
| RCF-GRT-025 | high | security | Spreading adverse findings to every sponsor conflicts with keeping sponsor investigations confidential | REQ-X-007;UNB-CHAR-036;UNB-CHAR-037;UNB-EXT-033;UNB-EXT-015 | See RCC-GRT-004. Propagate only a typed status (HeldAdverse with a reason category), never the private evidence. A global hold requires E0 or E1 evidence or independent compliance authority. Other sp… |
| RCF-GRT-027 | high | architecture | Append-only history, temporal queries and replay are assigned to Strata, which is a schema-migration tool | UNB-GEX-002;UNB-CHAR-040;UNB-KNOW-014;UNB-EXT-027 | Restate these as technology-neutral capability requirements: append-only journal tables, bitemporal columns, and replay from the outbox, implemented in the authoritative PostgreSQL core. Limit Strata… |
| RCF-GRT-034 | high | completeness | International grants lack the equivalency-determination and expenditure-responsibility obligations, and ownership of the decision is contradictory | REQ-X-065;UNB-GRANT-041;UNB-REM-014;GRANT-T14 | Adopt UNB-RCN-GRT-003 (ER obligation lifecycle). Mark foreign-recipient decisions as sponsor-scoped (L3) unless counsel approves a reliance agreement. |
| RCF-PLT-003 | high | financial-integrity | Sponsor-level books and cross-partner transfers span databases despite single-transaction rule | REQ-X-021;UNB-TEN-020;REQ-X-057;UNB-TEN-013 | Decide the partition key of the authoritative ledger: legal sponsor (recommended) not distribution partner. If partners must be physically separated, require a two-phase transfer state machine (Trans… |
| RCF-PLT-004 | high | consistency | Row-level tenancy assumed by ordering design but not approved as isolation profile | UNB-TEN-002;UNB-TEN-005;REQ-X-020 | State in REQ-X-020 that tenant scoping is by schema/database binding through the connection broker (UNB-TEN-004/005) and replace ADV-079 with 'connection bound to wrong schema/database' and 'pooled s… |
| RCF-PLT-005 | high | precision | The tenant unit is undefined: partner versus sponsor | UNB-TEN-001;UNB-TEN-020;REQ-X-024;REQ-X-048;REQ-X-057;UNB-KNOW-012 | Add a glossary requirement (gap UNB-RCN-PLT-001) defining Tenant, LegalSponsor, DistributionPartner, Program and Cell, the isolation key for each data class, and which classes may cross partners with… |
| RCF-PLT-006 | high | necessity | Three isolation profiles plus live A->B->C migration mandated before any partner evidence | UNB-PLAT-008;UNB-TEN-002;UNB-TEN-012;UNB-TEN-013;UNB-TEN-018 | Make one profile the default (recommended: database-per-partner on managed infrastructure, Profile B). Keep A and C as gated options activated only by a signed contractual requirement and the UNB-TEN… |
| RCF-PLT-007 | high | correctness | Append-only, bitemporal and replay responsibilities attributed to Strata, a schema-management tool | UNB-PLAT-007;UNB-TEC-STRATA-001;UNB-TEC-STRATA-002;UNB-TEC-STRATA-003;UNB-TEC-STRATA-004;… | Re-scope Strata to schema lifecycle (compile, diff, governed deploy of per-tenant schemas and append-only privileges). Move append-only enforcement, bitemporal history and replay to an ADR-selected p… |
| RCF-PLT-008 | high | consistency | Technology mandates still SHALL despite ADR-gating decision and noncommercial licences | UNB-PLAT-002;UNB-PLAT-003;UNB-PLAT-006;UNB-PLAT-007;REQ-X-012;REQ-X-050 | Resolve OD-16. Rewrite UNB-PLAT-002/003/006/007 as 'preferred candidate, subject to ADR, capability verification and an executed commercial licence', with a named fallback per component. |
| RCF-PLT-009 | high | consistency | Folio mandated for documents while Typst is the preferred PDF renderer | UNB-PLAT-003;REQ-X-034;REQ-X-033;UNB-DOC-003 | Choose one issuance renderer for as-issued financial documents via ADR (Folio P2 Chromium or Typst), and limit the other to non-issued previews. Update UNB-PLAT-003 accordingly. |
| RCF-PLT-011 | high | necessity | Visual WYSIWYG email designer is not needed for transactional DAF notices | REQ-X-029;REQ-X-031;REQ-X-052 | Supersede REQ-X-029/031/052 for the initial scope with a constrained template model: fixed per-event block layouts, Brand Kit theming, editable text slots with typed tokens, locale variants, preview… |
| RCF-PLT-013 | high | correctness | Seven-year retention floor has no stated legal basis and conflicts with the no-invented-periods rule | REQ-X-036;REQ-X-037;REQ-X-038;SOC2-023 | Keep seven years as a sponsor-contract business policy parameter (owner: GC) rather than a platform legal floor; require each record class to cite authority type (statute/regulation/IRS guidance/cont… |
| RCF-PLT-016 | high | traceability | Adversarial scenario IDs ADV-843..ADV-890 defined in several files with different content | REQ-X-054;REQ-X-055;REQ-X-053 | Renumber per file with a file-scoped prefix or a registry allocator; reject duplicate IDs in CI (REQ-X-082 static gate). |
| RCF-PLT-017 | high | testability | About 700 adversarial scenarios share one boilerplate expected result per file | REQ-X-051;REQ-X-053;REQ-X-054;REQ-X-055;REQ-X-037;REQ-X-038;REQ-X-048;REQ-X-049;REQ-X-050 | Require per-scenario expected state, rejected command/error code, audit record and invariant (as in Output 6) before an ADV ID counts as acceptance; mark existing ones 'stimulus only'. |
| RCF-PLT-018 | high | precision | AI assistance contract has a field list but no standard schema | REQ-X-051;REQ-X-053;UNB-API-STD-001;UNB-API-STD-006 | Define one JSON Schema (draft 2020-12) published with the API contract, with a minimal required core (id, version, owner, kind, purpose, inputs schema ref, side effects enum, authority class, confirm… |
| RCF-PLT-019 | high | consistency | Four incompatible AI autonomy taxonomies | UNB-AI-STD-003;AIN-003;REQ-X-017;REQ-X-051;REQ-X-055;GOV-AUTH-010 | Adopt one authority matrix (action class x actor type {deterministic policy, AI agent, human} -> allowed/proposal-only/prohibited) with A0-A5 as the single scale, and map L- and Tier- terms to it or… |
| RCF-PLT-020 | high | consistency | AIN-003 requires humans for all grant decisions; REQ-X-017 permits zero-human routine approval | AIN-003;REQ-X-017;REQ-X-001;GOV-AUTH-010 | Reword AIN-003 to 'reserved decisions not covered by a board-delegated deterministic policy (OD-01) require authorized humans; AI never approves'. Keep OD-01 open with GC. |
| RCF-PLT-023 | high | usability | Staff UI is organised by department and queue rather than by goal and exception | UNB-UX-001;AIN-001;ADM-031;REQ-X-048;REQ-X-049;REQ-X-057 | Make the primary staff surface an exception inbox of WorkItems that exist only for (a) deterministic-policy hold, (b) evidence missing, (c) reserved-authority decision; require each WorkItem to name… |
| RCF-PLT-027 | high | financial-integrity | Screen state set omits Unknown/Uncertain and Failed | UNB-UX-003;AIN-014;REQ-X-011 | Separate UI loading states from domain outcome states; require each screen to render the subject's domain state vocabulary (including Unknown) from the authoritative state machine. |
| RCF-PLT-030 | high | correctness | 90-control master matrix is 30 copies of three templates marked Designed | UNB-CTRL-001;SOC2-008;SOC2-019;MATRIX-007;MATRIX-008 | Reset status to Proposed; drop the 90 templated rows in favour of the 46 specific controls plus risk-driven additions; build a TSC criterion-to-control map with explicit not-applicable decisions. |
| RCF-PLT-031 | high | traceability | The 1,893 keyword-generated requirement-to-control links are mostly not meaningful | SOC2-007;MATRIX-001;OPS-031;UNB-PLAT-002;UNB-PLAT-005;UNB-TEN-020;GOV-AUTH-019 | Quarantine the CSV as 'machine candidates, do not cite'. Replace with control-first mapping: each control owner lists the requirements the control enforces (expected tens, not thousands), plus requir… |
| RCF-PLT-032 | high | completeness | 646 of 1,140 requirement IDs have no control link or non-applicability decision | SOC2-007;MATRIX-001;UNB-PAY-001;UNB-API-STD-001;AIN-003;REQ-X-020 | Prioritise control-first mapping for payment, authority, tenant isolation, AI tool authorization and evidence integrity requirements; record explicit not-applicable for presentation-only requirements. |
| RCF-PLT-035 | high | necessity | A full GRC product is specified inside the DAF platform | OPS-001;OPS-032;MON-001;MON-024;GOV-AUTH-017 | Keep in product: evidence emission, evidence integrity, expected-evidence monitor, deficiency cases in the shared Case model, Folio evidence export. Use an external GRC or spreadsheet register for co… |
| RCF-PLT-048 | high | completeness | Donor-facing charity search is not specified | UNB-CHAR-001;UNB-UX-001;REQ-X-011 | Add UNB-RCN-PLT-014. |
| RCF-REG-001 | high | correctness | 180-day revalidation treated as the currency standard for charity status | REQ-X-007;REQ-X-018 | Adopt RR-REQ-X-007-r1: an IRS-status check at approval and at release, a per-sponsor snapshot, and 180 days as a sponsor-configurable deep-review interval. |
| RCF-REG-005 | high | completeness | Donor control of supporting organizations cannot be determined from IRS data | REQ-X-007;UNB-CHAR-001 | Add UNB-RCN-REG-002. |
<!-- END GENERATED: findings-severe -->

## 7. Medium and low findings

<!-- BEGIN GENERATED: findings-other -->
| ID | Sev. | Dimension | Finding | Affected |
|---|---|---|---|---|
| RCF-ARC-002 | medium | completeness | Dynamic terminology has no owner | REQ-X-030;UNB-PLAT-009 |
| RCF-ARC-003 | medium | precision | No contract source of truth or format | UNB-API-STD-001;UNB-ACC-023;REQ-X-051 |
| RCF-ARC-004 | medium | architecture | Protective emergency plane and outage intake add a second durable system | REQ-X-047;REQ-X-020;REQ-X-021 |
| RCF-ENT-005 | medium | consistency | Program naming collision: Unbound (repo/brand) vs 'Strata' (owner's program name and an EF persistence component) | UNB-EXP-002;UNB-TEC-STRATA-001 |
| RCF-ENT-012 | medium | necessity | Duplicate: portability, transfer and migration requirements overlap and conflate three different processes | UNB-LS-015;UNB-HYP-007;UNB-WF-014;UNB-REM-008;UNB-GAP-025;UNB-GAP-028;REQ-X-061 |
| RCF-ENT-013 | medium | necessity | Duplicate: donor and advisor identity onboarding specified five times with different scopes | UNB-PART-007;UNB-GAP-014;REQ-X-072;UNB-WF-012;ADM-009 |
| RCF-ENT-018 | medium | completeness | No rule says whether a successor account at another sponsor or program is an internal allocation or a distribution | UNB-SUCC-014;UNB-SUCC-003;UNB-WF-014 |
| RCF-ENT-019 | medium | completeness | Dormancy is a one-line requirement framed as 'unclaimed accounts', which confuses sponsor-owned balances with escheat | UNB-WF-015;ADM-024;CTL-044 |
| RCF-ENT-021 | medium | completeness | Outbound sponsor-to-sponsor transfer has no specified treatment | UNB-WF-014;TransferOutRequested |
| RCF-ENT-022 | medium | completeness | Fund closure has no state machine or trigger catalogue in the UNB requirement families | ADM-025;UNB-WF-015;E2E WF-02 |
| RCF-ENT-024 | medium | precision | 'Advisor' is overloaded: donor advisor, professional/investment advisor and successor advisor | UNB-ADV-001;UNB-ADV-002;UNB-PART-003;CHARTER UNB-ADV domain |
| RCF-ENT-025 | medium | completeness | Households and joint donors have no rules for contributor of record, separation or divorce, or household privacy | ADM-008;UNB-PART-001;UNB-PART-003;EXPANSION R1 |
| RCF-ENT-026 | medium | completeness | Role taxonomy omits legal representatives (POA agent, guardian, executor, trustee, entity signatory) | UNB-PART-003;UNB-SUCC-006;UNB-SUCC-007 |
| RCF-ENT-030 | medium | completeness | The legal operating-model matrix omits state charitable-platform and solicitation applicability for the technology-provider role | REQ-X-064;UNB-LEG-004 |
| RCF-ENT-031 | medium | consistency | 'One system, single coordinated deployment' conflicts with the dedicated-infrastructure tenancy profile | REQ-X-057;UNB-TEN-002 |
| RCF-ENT-032 | medium | consistency | Mandatory component selections were made before the CHARTER-required capability discovery and gate | CHARTER EF integration discovery;UNB-TEC-STRATA-001;UNB-TEC-002;REQ-X-057 |
| RCF-ENT-033 | medium | completeness | Customer support has no requirement for appeals of sponsor decisions, assisted-channel attribution or support-channel identity assurance | BASELINE scenario family 'sponsor decision appeal';UNB-GAP-026;UNB-REM-006;E2E WF-16 |
| RCF-ENT-036 | medium | usability | Succession previews and donor wording risk reading as promises; successor-advisor litigation makes the wording legally sensitive | UNB-SUCC-010;UNB-SUCC-013 |
| RCF-ENT-037 | medium | correctness | Tax-fit guidance asserts unspecified 'special 2026 rules' and has the sponsor's fee-earning system compare DAF against direct giving | REQ-X-066 |
| RCF-FIN-002 | medium | consistency | UNB-ACC-006 orders acknowledgment before settlement | UNB-ACC-006;REQ-X-009;ADM-010 |
| RCF-FIN-004 | medium | correctness | Requirements assume the sponsor states a value on noncash acknowledgments | UNB-ACC-031;REQ-X-059;UNB-INV-012 |
| RCF-FIN-007 | medium | correctness | No valuation-date or completed-gift-date rules for contributed securities, crypto or complex assets | UNB-INV-012;UNB-INV-021;UNB-NAV-013;UNB-ACC-031;REQ-X-059 |
| RCF-FIN-011 | medium | consistency | Two incompatible journal envelopes; one puts a single FundId on the batch header | ACC-MDL-003;REQ-X-084;UNB-ACC-002;UNB-ACC-011 |
| RCF-FIN-012 | medium | consistency | Fee posting examples contradict each other on who receives the fee | REQ-X-084;UNB-ACC-010;ACC-MDL-006;REQ-X-067 |
| RCF-FIN-017 | medium | consistency | Capacity and ledger invariants assume unitization, which REQ-X-023 makes optional | REQ-X-023;UNB-INV-003;FI-02;CTL-018 |
| RCF-FIN-018 | medium | consistency | Break blocking rules disagree and thresholds have no owner | ACC-MDL-009;UNB-NAV-007;UNB-NAV-012;UNB-ACC-018;REQ-X-040 |
| RCF-FIN-019 | medium | consistency | Independent attestation of every reconciliation conflicts with auto-pass of routine reconciliations | REQ-X-010;REQ-X-040 |
| RCF-FIN-022 | medium | completeness | Closing (100%) grants, proceeds-linked grants and the percentage fixing point remain unresolved | UNB-GFUND-001;UNB-GFUND-002;UNB-GFUND-013;UNB-GFUND-024 |
| RCF-FIN-023 | medium | consistency | Per-grant liquidation framing persists for unitized pools | UNB-GFUND-011;UNB-GFUND-005;FI-14 |
| RCF-FIN-024 | medium | testability | Canonical text of REQ-X-059, -067 and -070 is boilerplate with no testable obligations | REQ-X-059;REQ-X-067;REQ-X-070 |
| RCF-FIN-025 | medium | testability | ADV-435..ADV-470 all have the same boilerplate expected outcome | REQ-X-040 |
| RCF-FIN-026 | medium | testability | Journal fixtures balance but cannot test fund, entity or currency balancing | REQ-X-084;ACC-MDL-003 |
| RCF-FIN-032 | medium | correctness | Payroll and employer-matching contributions raise unaddressed substantiation and benefit questions | REQ-X-070 |
| RCF-FIN-036 | medium | completeness | Donor annual summaries must not imply nonitemizer deduction eligibility for DAF gifts | UNB-ACC-006;UNB-ACC-021 |
| RCF-GRT-004 | medium | precision | 'Comprehensive revalidation' has no defined content | REQ-X-007;SCV-03;TX-27 |
| RCF-GRT-016 | medium | precision | The result of a percentage grant depends on the order in which concurrent grants and reservations are processed | UNB-GFUND-002;UNB-GFUND-013;UNB-GFUND-021;UNB-GFUND-024 |
| RCF-GRT-019 | medium | consistency | ACH Notification of Change handling is unspecified and may conflict with pinned, independently verified destinations | REQ-X-005;REQ-X-083;TX-19 |
| RCF-GRT-020 | medium | completeness | The check rail proof row leaves out the positive-pay issue and void files | REQ-X-083;UNB-GAP-009 |
| RCF-GRT-021 | medium | precision | Callback deduplication uses only providerEventId, with no authentication or semantic rule | REQ-X-083;UNB-GEX-003;GR-EVT-047;UNB-PAY-004 |
| RCF-GRT-022 | medium | automation | Several exception classes always send work to a human even when a rule could settle it | EXC-01;EXC-02;UNB-GEX-009;UNB-RULE-012;REQ-X-017 |
| RCF-GRT-026 | medium | consistency | 'Eligibility' means two different things and has five different state vocabularies | UNB-CHAR-019;UNB-CHAR-021;REQ-X-019;REQ-X-007 |
| RCF-GRT-028 | medium | completeness | The 60-event grant catalog has no events for Unknown, void, recall, reissue, blocked, escheat, affirmation or held occurrences | GR-EVT-045;GR-EVT-046;GR-EVT-047;GR-EVT-048;UNB-GEX-001;REQ-X-083 |
| RCF-GRT-029 | medium | consistency | Every event must carry sponsor, partner and DAF IDs, which common charity-registry events do not have | UNB-GEX-004;REQ-X-007;GR-EVT-010;GR-EVT-011 |
| RCF-GRT-030 | medium | necessity | Duplicate: bank-detail change control is defined six times at different strengths | UNB-GRANT-026;UNB-GEX-018;UNB-CHAR-032;UNB-PAY-005;REQ-X-005;TX-19 |
| RCF-GRT-031 | medium | necessity | Duplicate: execution-time rechecks are stated seven times, some of them optional | UNB-GRANT-009;UNB-RULE-015;UNB-CHAR-024;REQ-X-007;REQ-X-018;REQ-X-019;UNB-GEX-019 |
| RCF-GRT-032 | medium | testability | The canonical REQ-X-058..077 entries are template text and their detail files list cases without expected results | REQ-X-065;REQ-X-069;REQ-X-077;REQ-X-058;REQ-X-063;G-02;SCV-03 |
| RCF-GRT-033 | medium | testability | The base grant and charity requirements have no RFC 2119 keywords and their scenarios are not traced to IDs | UNB-GRANT-001;UNB-GEX-001;UNB-RULE-001;UNB-CHAR-001;UNB-GFUND-001 |
| RCF-GRT-035 | medium | completeness | Nothing stops a charity from issuing the donor a deductible-gift receipt for a DAF grant | REQ-X-063;REQ-X-077;REQ-X-058;UNB-GRANT-025 |
| RCF-GRT-036 | medium | completeness | Embedded checkout does not capture what is being bought, so ticket purchases cannot be detected | REQ-X-063;TX-26;INV-16 |
| RCF-GRT-037 | medium | financial-integrity | What happens to returned, refunded or declined grant money is undefined | UNB-GRANT-028;TX-15;EXC-08 |
| RCF-GRT-038 | medium | completeness | G03 has no defined alternate-evidence path for legitimate recipients that are not in the IRS files | UNB-CHAR-016;UNB-RULE-003;REQ-X-018 |
| RCF-GRT-040 | medium | completeness | Nothing says what to do when an adverse event arrives after a payment has been dispatched | REQ-X-020;REQ-X-018;UNB-GRANT-009 |
| RCF-PLT-010 | medium | correctness | Forma treated as a widget runtime and brand owner | UNB-PLAT-009;INT-019;REQ-X-050;REQ-X-055;MON-022 |
| RCF-PLT-012 | medium | necessity | Duplicate: six overlapping email template requirements | REQ-X-027;REQ-X-028;REQ-X-029;REQ-X-030;REQ-X-031;REQ-X-052 |
| RCF-PLT-014 | medium | automation | Every expired record becomes a human disposition review | REQ-X-036;REQ-X-037;REQ-X-038 |
| RCF-PLT-015 | medium | necessity | Duplicate: the retention floor and hold semantics restated in three requirements | REQ-X-036;REQ-X-037;REQ-X-038;REQ-X-012 |
| RCF-PLT-021 | medium | necessity | Duplicate: AI assistance specified about nine times | REQ-X-051;REQ-X-053;REQ-X-054;REQ-X-055;AIN-001;AIN-036;UNB-AI-STD-001;UNB-AI-STD-002;UNB… |
| RCF-PLT-022 | medium | necessity | AI help on every input field is unbounded scope | REQ-X-054;REQ-X-053;REQ-X-051 |
| RCF-PLT-024 | medium | necessity | Duplicate: overlapping command centers, dashboards and widgets | REQ-X-049;REQ-X-050;REQ-X-055;UNB-KNOW-039;MON-022 |
| RCF-PLT-025 | medium | necessity | Duplicate: five case/work-item models | AIN-009;ADM-015;ADM-016;REQ-X-048;MON-007;OPS-017 |
| RCF-PLT-026 | medium | consistency | Dashboards deferred in REQ-X-043 but first-class in REQ-X-049/050 | REQ-X-043;REQ-X-049;REQ-X-050 |
| RCF-PLT-028 | medium | usability | Phone completion mandated for every journey including heavy administration | UNB-PLAT-004;UNB-UX-001;OPS-026 |
| RCF-PLT-029 | medium | completeness | Document accessibility escapes through 'where supported' | UNB-PLAT-005;UNB-DOC-003;UNB-PLAT-003;REQ-X-035 |
| RCF-PLT-033 | medium | consistency | Three control lifecycle vocabularies | SOC2-019;MATRIX-007;MON-001;OPS-001;UNB-SCTRL-001 |
| RCF-PLT-034 | medium | security | Missing-evidence control relies on a heartbeat from the monitored system | UNB-SCTRL-041;SOC2-027;MATRIX-004;OPS-010;MON-004 |
| RCF-PLT-036 | medium | completeness | SOC 1 for fund-accounting controls still absent from the governance directive | SOC2-001;MATRIX-008;REQ-X-073 |
| RCF-PLT-037 | medium | necessity | Duplicate: SOC2, MATRIX, OPS, MON and GOV-AUTH families restate the same rules | SOC2-027;MATRIX-004;OPS-010;MON-004;SOC2-036;OPS-022;MON-024;MATRIX-011 |
| RCF-PLT-038 | medium | precision | REQ-X-058..077 normative text is identical boilerplate | REQ-X-071;REQ-X-072;REQ-X-073 |
| RCF-PLT-039 | medium | necessity | IVR channel lacks demand evidence; numeric references are the durable value | REQ-X-025 |
| RCF-PLT-040 | medium | security | White-label and dynamic terminology lack a locked legal-term list and channel consent rules | REQ-X-030;UNB-PLAT-009 |
| RCF-PLT-041 | medium | completeness | No canonical notification event catalog | AIN-023;ADM-030;INT-017;REQ-X-027;REQ-X-035;REQ-X-055 |
| RCF-PLT-042 | medium | precision | API standard lists topics but defines no concrete conventions | UNB-API-STD-001;UNB-API-STD-002;UNB-API-STD-003;REQ-X-051 |
| RCF-PLT-043 | medium | security | Assistance Orchestrator context retrieval is not bound to the caller principal | UNB-AI-STD-001;REQ-X-053;UNB-TEN-009;AIN-012 |
| RCF-PLT-044 | medium | precision | Tenant selection for multi-partner users is not specified | UNB-TEN-004;UNB-TEN-020;INT-001 |
| RCF-PLT-047 | medium | security | Duplicate: embedded widget security restated five times while TVR-WID items stay open | AIN-029;INT-015;ADM-037;MON-022;REQ-X-050 |
| RCF-REG-002 | medium | correctness | Carried REG-001 row says grant approval cannot be vested in software | UNB-TRACE-003 |
| RCF-REG-003 | medium | completeness | Retention floor lacks authority-keyed classes (OFAC 10 years) | REQ-X-036;REQ-X-037;REQ-X-038 |
| RCF-REG-006 | medium | completeness | OFAC blocking and reporting duties are not specified | UNB-LEG-004;REQ-X-007 |
| RCF-REG-009 | medium | completeness | No regulatory-change watch for pending DAF guidance | UNB-LEG-001;UNB-LEG-005 |
| RCF-REG-010 | medium | completeness | No excess-business-holdings computation | REQ-X-059;UNB-AST-001 |
| RCF-ENT-028 | low | completeness | Donor-level data portability is a hypothesis with no requirement | UNB-HYP-007;UNB-LS-015 |
| RCF-ENT-034 | low | precision | The E2E workflow inventory table's column headers do not match its cell contents | REQ-X-078 |
| RCF-ENT-035 | low | traceability | Failure-mode ID FM-12 means 'duplicate delivery' in the simulation taxonomy and 'account takeover' in the validation program | FM-12;FM-05 |
| RCF-FIN-027 | low | correctness | NAV worked example mislabels net assets as assets | UNB-NAV-023 |
| RCF-FIN-028 | low | traceability | DAF fund accounting detail file cites the wrong companion requirements | REQ-X-023 |
| RCF-FIN-029 | low | necessity | Duplicate: REQ-X-041 is defined twice with different text | REQ-X-041 |
| RCF-FIN-030 | low | necessity | Duplicate: the append-only correction rule appears at least eight times with different required attributes | UNB-ACC-003;ACC-MDL-002;UNB-IMM-002;UNB-IMM-003;UNB-NAV-002;UNB-NAV-042;UNB-CUST-012;REQ-… |
| RCF-FIN-031 | low | necessity | Duplicate: cash/liquidity distinctions are restated with inconsistent vocabulary | UNB-INV-024;UNB-NAV-035;UNB-CUST-015;ACC-MDL-004;UNB-GFUND-006 |
| RCF-FIN-033 | low | testability | Performance requirements have no baseline volumes | UNB-IMM-010;UNB-ACC-036 |
| RCF-FIN-034 | low | necessity | Monthly statements by default for every fund is a convention, not an outcome | REQ-X-032 |
| RCF-FIN-035 | low | correctness | Seven-year retention floor is presented as a fixed rule | REQ-X-039;REQ-X-040;REQ-X-041 |
| RCF-GRT-039 | low | consistency | GRANT-T19 says a schedule 'survives donor death'; REQ-X-008 says future instructions are suspended | GRANT-T19;REQ-X-008 |
| RCF-PLT-045 | low | precision | UNB-UX-007 is untestable | UNB-UX-007 |
| RCF-PLT-046 | low | architecture | No owner for executing adversarial scenarios; Dokimos is a code-quality system | UNB-RES-002;REQ-X-082 |
| RCF-REG-004 | low | correctness | Exclusive legal control cited to the wrong section | UNB-ACC-006 |
| RCF-REG-007 | low | correctness | KYC/CIP applicability correctly left open; must not be imported as law | ADM-002;REQ-X-072 |
| RCF-REG-008 | low | traceability | Superseded Rev. Proc. 2011-33 cited in evidence | UNB-CHAR-001 |
<!-- END GENERATED: findings-other -->

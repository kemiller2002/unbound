# 02 — Proposed requirements (UNB-CDD)

Package: [README](README.md) · Source data: [`data/gap-requirements.jsonl`](data/gap-requirements.jsonl) · Status: **proposed**. None of these records is adopted into the authoritative baseline; adoption follows the existing ratification process. Where a converged clause (`CNV-*`) states the behaviour, the clause governs on conflict.

Areas: **CIV** charity intelligence and vetting; **ONB** institutional onboarding gaps (everything else in onboarding is already covered by UNB-ONB-001..033 and UNB-TRN-*); **CMF** commercial and operating-mode flexibility; **OEM** operational economics measurement and prevention.

<!-- BEGIN GENERATED -->
### UNB-CDD-CIV-001 — Typed evidence classes for every charity record

| Field | Value |
|---|---|
| Requirement | Every charity evidence record MUST carry exactly one evidence class from a versioned, closed catalogue: authoritative fact; third-party assertion; licensed third-party verification; charity-provided information; sponsor-specific decision; shared vetting evidence; historical observation; inferred relationship; unresolved conflict; AI-generated assessment. The catalogue MUST state for each class whether it may drive an automated eligibility outcome, who may change it, and whether it may be shared across sponsors. Only authoritative facts (through a deterministic, versioned rule) and sponsor-specific decisions (for that sponsor only) may drive an eligibility outcome; third-party assertions and licensed verifications may do so only where the sponsor's versioned policy names them as sufficient evidence for a stated predicate. |
| Business rationale | Without explicit classes, inferred, asserted or AI-generated records can accumulate into de facto facts; the approved DEC-005 forbids that, and DEC-004 needs a class that carries per-licence sharing flags. |
| Refines / applicable existing requirements | UNB-CHAR-018; UNB-CHAR-012; UNB-KNOW-002; UNB-CNET-002 |
| Origin | refinement (parent UNB-CHAR-018) |
| Approved decisions | DEC-005; DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 17-unbound-implications.md; 20-adversarial-review.md |
| Research ids | XRI-002; SRC-008; SRC-063; CHL-063 |
| Evidence gates | — |
| Acceptance criteria | Given a charity with a Pub 78 listing (authoritative fact), a Charity Navigator rating (third-party assertion) and an AI dossier note, When the sponsor's eligibility rule is evaluated, Then only the Pub 78 fact and any third-party assertion the sponsor's policy names for that predicate are inputs, and the evaluation record lists the class of every input. |
| Dependencies | — |
| Security implications | Class is assigned by the ingesting component and is immutable; a reclassification is a new record with actor and reason, so a privileged user cannot silently upgrade an assertion to a fact. |
| Failure behaviour | A record without a class, or with a class not in the active catalogue version, MUST be excluded from eligibility evaluation and routed to the steward queue; evaluation proceeds as if the record were absent (fail safe: absence never proves eligibility, UNB-CHAR-016). |
| Verification method | Automated (catalogue conformance tests; property test that no evaluation consumes an unclassified record) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-002 — Promotion rules: no authority by accumulation

| Field | Value |
|---|---|
| Requirement | No record MUST become an authoritative fact or a shared record by accumulation, confidence score, repetition or agreement between non-authoritative sources. Promotion MUST occur only through (a) a later authoritative source record processed by a deterministic, versioned rule, or (b) a named sponsor decision, which is effective for that sponsor only. Every promotion MUST be an append-only event naming rule version or decision-maker, inputs and reason. A link, finding or confirmation that used any sponsor-confidential input MUST stay in that sponsor's layer; screening match dispositions, match thresholds and conflict labels MUST remain sponsor-scoped, and only source-attributed public facts re-verified from the primary source MAY propagate (CNV-GRT-004). |
| Business rationale | Approved DEC-005 makes AI and inferred records advisory; CHL-044 and CHL-062 show that shared dispositions and promotions from confidential inputs leak across tenants and create correlated failure and defamation exposure. |
| Refines / applicable existing requirements | UNB-CHAR-037; UNB-CHAR-036; CNV-GRT-004; UNB-CNET-002 |
| Origin | refinement (parent UNB-CHAR-037) |
| Approved decisions | DEC-005; DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 20-adversarial-review.md; 13-compounding-competitive-advantages.md |
| Research ids | XRI-002; CHL-044; CHL-062; CLM-035 |
| Evidence gates | — |
| Acceptance criteria | Given an inferred successor link scored 0.97 that three sponsors' stewards have each viewed, When no authoritative record or named sponsor decision confirms it, Then the link remains an inferred relationship, no eligibility outcome uses it, and no shared record is created; And Given sponsor A confirms it using a board letter it holds privately, When sponsor B queries the charity, Then B sees no confirmation. |
| Dependencies | UNB-CDD-CIV-001 |
| Security implications | Prevents cross-tenant inference and leakage of sponsor-confidential evidence through the shared layer (CNV-TEN-001 SharedStoreRegister). |
| Failure behaviour | A promotion request lacking a rule version or named decision-maker MUST be rejected and logged; a shared-layer write whose lineage includes a sponsor-scoped input MUST be refused. |
| Verification method | Automated (lineage tests over promotion events; negative isolation tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-003 — AI assessments are advisory records

| Field | Value |
|---|---|
| Requirement | Every AI output about a charity (summary, dossier, extraction, classification proposal, risk note) MUST be stored as an AI-generated assessment record carrying model identifier and version, prompt template version, input evidence identifiers and hashes, and time. An AI-generated assessment MUST NOT write or amend a fact, an eligibility decision, a hold, a payment destination, a screening disposition or a regulatory determination, and MUST NOT be promoted; a human or deterministic rule that acts after reading it MUST record its own decision against primary evidence, and the assessment is retained only as context. AI MUST NOT be given proprietary or non-commercial source content as prompt input unless the source's rights record permits AI processing (UNB-CDD-CIV-005). |
| Business rationale | Approved DEC-005: AI assessments and inferred records are advisory and never silently become authoritative facts or financial, regulatory or eligibility decisions. |
| Refines / applicable existing requirements | CNV-AIO-002; CNV-AIO-001; UNB-CHAR-022; UNB-CNET-022 |
| Origin | refinement (parent CNV-AIO-002) |
| Approved decisions | DEC-005; DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 03-charity-data-licensing-and-access.md; 08-frictionless-onboarding.md |
| Research ids | XRI-002; CLM-034; SRC-008 |
| Evidence gates | — |
| Acceptance criteria | Given an AI dossier stating 'website suggests programme activity in a sanctioned region', When a reviewer opens the case, Then the deterministic findings section is unchanged by the note, the note is labelled as an AI-generated assessment with model and inputs, and the case cannot be closed, held or cleared without a recorded human decision citing primary evidence. |
| Dependencies | UNB-CDD-CIV-001; UNB-CDD-CIV-005 |
| Security implications | Removes prompt-injection and hallucination paths from authoritative stores; AI service principals hold no write grant on fact, decision, hold or destination tables. |
| Failure behaviour | If provenance fields cannot be recorded, the AI output MUST be discarded rather than stored without provenance; AI unavailability MUST leave deterministic processing unaffected. |
| Verification method | Automated (authorization tests proving AI principals cannot write authoritative tables) + review of dossier rendering |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-004 — Candidate screening and exclusion sources

| Field | Value |
|---|---|
| Requirement | The source register MUST list, as candidate sources with licence class, access method and constraints, at least: the trade.gov Consolidated Screening List; the FCDO UK Sanctions List (which replaced the OFSI consolidated list, closed 2026-01-28); the Federal Audit Clearinghouse (EIN and UEI bridge); and SAM.gov exclusions (exclusion facts only; D&B-derived fields are not redistributed in bulk). A candidate source MUST NOT be part of legally applicable sanctions or exclusion screening until counsel adds it to the ScreeningScope matrix (RR-UNB-EXT-004-r1); until then a match is advisory evidence routed for review and never an adverse determination. Each adopted source MUST be ingested from bulk extracts (not rate-limited per-query APIs), archived per release like the federal sources in UNB-CNET-003, and treat a missing release as unknown, not as no change. |
| Business rationale | The research verified that these free, commercially usable sources extend screening beyond OFAC and that the OFSI list no longer exists (CLM-006, CLM-009); the existing requirements name OFAC and SAM only in general terms. |
| Refines / applicable existing requirements | UNB-EXT-004; UNB-EXT-003; RR-UNB-EXT-004-r1; UNB-CNET-003 |
| Origin | refinement (parent UNB-EXT-004) |
| Approved decisions | DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 02-charity-intelligence-sources.md; 03-charity-data-licensing-and-access.md; 17-unbound-implications.md |
| Research ids | XRI-004; CLM-006; CLM-009; SRC-064 |
| Evidence gates | — |
| Acceptance criteria | Given the CSL is a candidate not yet in the ScreeningScope matrix, When a charity name matches a CSL entry, Then a review item is opened with the CSL row as advisory evidence, no hold is published and no adverse determination is recorded; And When counsel adds CSL to the matrix version N+1, Then matches under N+1 follow RR-UNB-EXT-004-r1 handling. |
| Dependencies | UNB-CDD-CIV-005 |
| Security implications | Sanctions data is public; the risk is false adverse determinations, which this requirement bounds by counsel-approved scope and name-match rules (OD-M-10). |
| Failure behaviour | If a candidate source is unreachable or its release fails integrity gates (UNB-CNET-021), its evidence is marked unknown and no assurance is derived from it; legally applicable screening continues on the adopted lists. |
| Verification method | Automated (source register conformance; screening scope tests) + counsel review of ScreeningScope |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | Which of the candidate lists (trade.gov CSL, FCDO UK Sanctions List, Federal Audit Clearinghouse, SAM.gov exclusions) does counsel add to the ScreeningScope matrix as legally applicable, and which remain advisory evidence only? |
| Conflicts | — |

### UNB-CDD-CIV-005 — Source rights enforced at query, export, API and AI time

| Field | Value |
|---|---|
| Requirement | Every source MUST have a versioned rights record (licence class: public domain, open with attribution, open no rights reserved, field-restricted, share-alike, non-commercial, proprietary or contractual, no licence found; and the permitted uses: internal use, shared multi-sponsor layer, redistribution, resale, AI processing, attribution text, retention). Every derived fact MUST carry the rights of all its inputs, and the most restrictive applies. Exports, API responses, shared-layer writes, published datasets and AI prompts MUST be filtered by rights at the time of use. Sources whose terms bar redistribution or resale in a commercial product without a licence (as researched: ProPublica Nonprofit Explorer, GivingTuesday 990 data, OpenSanctions, MSRB EMMA, and sources with no licence found such as BBB WGA) MUST NOT enter any shared or resold layer and MAY be used only as link-out or manually cited evidence; proprietary sources (e.g. Candid, Charity Navigator) MAY be attached only to the licensing sponsor's own decisions. State registry data MUST have its own rights row per state (17 USC 105 covers federal works only). Licence readings are research findings and MUST be confirmed by counsel before commercial reliance. |
| Business rationale | Approved DEC-004: exclude non-redistributable information from shared and resold intelligence layers and enforce source-specific rights, access restrictions and provenance. |
| Refines / applicable existing requirements | UNB-CNET-011; UNB-EXT-016; UNB-CHAR-008; UNB-TRN-002 |
| Origin | refinement (parent UNB-CNET-011) |
| Approved decisions | DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 03-charity-data-licensing-and-access.md; 02-charity-intelligence-sources.md; 05-charity-registry-economics.md |
| Research ids | XRI-004; CLM-007; SRC-064; OQ-007; OQ-008 |
| Evidence gates | — |
| Acceptance criteria | Given a fact derived from a ProPublica record and an IRS BMF row, When a shared-layer write or a cross-sponsor API response is attempted, Then the write is refused and the response omits the fact, while the sponsor that holds the record internally still sees it with its licence label; And Given an AI prompt is assembled, Then content from sources whose rights record denies AI processing is excluded. |
| Dependencies | — |
| Security implications | Rights filtering is an access-control decision executed server side; a client cannot request unfiltered data. Licence breach is a legal exposure, so failures default to exclusion. |
| Failure behaviour | If a rights record is missing or ambiguous, the source MUST be treated as no licence found (internal link-out only) until a rights record is approved. |
| Verification method | Automated (rights-filter tests for every egress path) + counsel review of rights records |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Counsel to confirm the licence readings in the research (OQ-007, OQ-008) before any source is classified as redistributable. |
| Conflicts | — |

### UNB-CDD-CIV-006 — Monitoring coverage register

| Field | Value |
|---|---|
| Requirement | Every eligibility-relevant predicate MUST be declared in a monitoring coverage register naming the sources that can change it, their cadence and freshness limit (UNB-CNET-004), and whether the predicate is continuously monitorable. A predicate with no continuously monitored source (e.g. bank letter, officer verification, website programme check, supporting-organisation control facts) is non-monitorable evidence and MUST carry an expiry under UNB-CDD-CIV-007. Absence of a new release MUST be recorded as unknown, never as no change; a lapse beyond the freshness limit MUST be visible per predicate and charity. |
| Business rationale | Change-driven re-vetting is only sound where monitoring is continuous against every source that can change eligibility; the register makes the blind spots explicit. |
| Refines / applicable existing requirements | RR-UNB-EXT-028-r2; UNB-CHAR-015; UNB-CNET-004; UNB-CNET-010 |
| Origin | refinement (parent RR-UNB-EXT-028-r2) |
| Approved decisions | DEC-003 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 03-charity-data-licensing-and-access.md; 05-charity-registry-economics.md |
| Research ids | XRI-003; SRC-001; SRC-063 |
| Evidence gates | — |
| Acceptance criteria | Given the register declares 'exempt status' monitored by BMF, Pub 78 and the revocation list and 'bank ownership' as non-monitorable, When the BMF release is 50 days late against a 45-day limit, Then every charity's exempt-status predicate shows monitoring lapsed and release-time checks fail safe per CNV-GRT-003, and bank ownership evidence is governed only by its expiry. |
| Dependencies | — |
| Security implications | The register is configuration under CNV-GOV-005 (versioned, independently approved); weakening coverage is a material change. |
| Failure behaviour | A predicate missing from the register MUST be treated as non-monitorable with the shortest configured expiry. |
| Verification method | Automated (register completeness check against the eligibility rule inputs) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-007 — Evidence expiry within the 180-day validity ceiling

| Field | Value |
|---|---|
| Requirement | Each evidence type MUST have a validity period set by sponsor policy, and expiry MUST trigger a targeted refresh of that evidence only, not a full re-vet. The validity of a charity eligibility determination MUST NOT exceed the deep-review interval of CNV-GRT-003 (default 180 days; a sponsor may tighten it and may not lengthen it without a later explicit owner decision); evidence relied on by that determination therefore lapses for eligibility purposes no later than the interval. Longer validity periods (e.g. 24 months for bank-account verification) MAY apply only to non-eligibility evidence such as payment-destination verification under CNV-PAY-002. Expired evidence relied on by eligibility MUST hold new approvals as RR-REQ-X-007-r1 specifies and MUST NOT be the sole basis of a payment-time eligibility decision. |
| Business rationale | Approved DEC-003 replaces repetitive full reviews with evidence expiration and targeted re-vetting while preserving the owner requirement that eligibility remain current within the approved 180-day validity policy. |
| Refines / applicable existing requirements | UNB-CHAR-024; CNV-GRT-003; RR-REQ-X-007-r1; UNB-CHAR-019 |
| Origin | refinement (parent UNB-CHAR-024) |
| Approved decisions | DEC-003 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 05-charity-registry-economics.md; 20-adversarial-review.md |
| Research ids | XRI-003; CLM-005; CHL-035 |
| Evidence gates | V-3; V-9 |
| Acceptance criteria | Given a sponsor policy setting website-programme evidence to 365 days and the deep-review interval to 180 days, When the policy is validated, Then activation is refused because eligibility evidence may not outlive the 180-day interval; And Given a bank verification aged 400 days with a 24-month destination policy, When a grant is released, Then destination evidence is current and eligibility is evaluated separately. |
| Dependencies | UNB-CDD-CIV-006 |
| Security implications | Expiry settings are policy configuration under CNV-GOV-005; lengthening validity is a material change requiring independent approval. |
| Failure behaviour | If the refresh of expired evidence fails, the determination falls back to NeedsReview and new approvals hold; release-time checks follow CNV-GRT-003 fail-safe rules. |
| Verification method | Automated (policy validation rules; temporal tests across the 180-day boundary) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-008 — Change-driven targeted re-vetting inside the deep-review interval

| Field | Value |
|---|---|
| Requirement | Between deep reviews, an eligibility-relevant change (classified by the materiality table of RR-UNB-EXT-028-r2 and UNB-CNET-007) MUST trigger re-vetting of only the affected predicates and the decisions, grants and schedules that depend on them; informational changes MUST NOT create work or block payment. Targeted re-vetting MUST NOT extend the deep-review clock (consistent with RR-REQ-X-007-r2 item 3). The deep review at the CNV-GRT-003 interval MUST be retained. Until the owner decides OD-CDD-02, the deep review MUST be performed as RR-REQ-X-007-r1 specifies; the reading under which a renewal that finds monitoring continuous, no eligibility-relevant change and every evidence item unexpired counts as the deep review (RR-REQ-X-007-r2 item 1) is not adopted by this package. |
| Business rationale | DEC-003 approves change-driven re-vetting as the direction; the research estimates full repetition at about 3x the cost (CLM-005), but the 180-day validity is preserved and the ratified wording governs until the owner decides. |
| Refines / applicable existing requirements | RR-UNB-EXT-028-r2; RR-REQ-X-007-r2; CNV-GRT-003; UNB-CNET-007 |
| Origin | refinement (parent RR-UNB-EXT-028-r2) |
| Approved decisions | DEC-003 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 05-charity-registry-economics.md; 17-unbound-implications.md |
| Research ids | XRI-003; CLM-005; HYP-005; HYP-006; CHL-035 |
| Evidence gates | V-3; V-9 |
| Acceptance criteria | Given a charity verified 60 days ago, When the auto-revocation list adds its EIN, Then a hold is published within the detection SLA, only that charity's open grants and schedules are re-evaluated, a single case opens, and the deep-review due date is unchanged; And Given an address change in the same EIN, Then no case and no hold are created. |
| Dependencies | UNB-CDD-CIV-006; UNB-CDD-CIV-007 |
| Security implications | Targeted re-vetting narrows work, not controls: adverse facts still hold payments immediately and release-time checks still run. |
| Failure behaviour | If change detection for a source is lapsed (UNB-CDD-CIV-006), the affected predicates MUST be treated as stale and fail safe, not as unchanged. |
| Verification method | Automated (replay over archived IRS and sanctions releases; V-3 harness) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | May a renewal at the CNV-GRT-003 deep-review interval that finds continuous monitoring, no eligibility-relevant change and all evidence unexpired count as the deep review (OD-CDD-02)? |
| Conflicts | Approved DEC-003 direction vs ratified CNV-GRT-003 'deep review SHALL default to 180 days' and RR-REQ-X-007-r1 'comprehensive charity review'; escalated as OD-CDD-02; ratified text governs now. |

### UNB-CDD-CIV-009 — Statistical sample audit (investigation item)

| Field | Value |
|---|---|
| Requirement | Unbound MUST investigate, and MUST NOT yet adopt, a stratified statistical sample audit in which a sample of charities with no detected change is fully re-reviewed to estimate the monitoring blind-spot rate. The investigation MUST specify strata, sample sizes for a stated detection power, the cost, the evidence to be produced for auditors, and how a detected blind spot changes monitoring coverage; it MUST use the existing public-data replay (requirements/convergence/2026-11-24-master/replay) as the lower-cost route to the V-3 measurements and the V-9 auditor walkthrough. The sample audit MUST NOT replace or lengthen the 180-day validity under CNV-GRT-003 unless the owner explicitly decides so. |
| Business rationale | Approved DEC-003 requires the statistical sample audit to be investigated; HYP-005 and OQ-004 (auditor acceptance) are untested. |
| Refines / applicable existing requirements | UNB-XIN-CLM-005; RR-REQ-X-007-r2 |
| Origin | refinement (parent UNB-XIN-CLM-005) |
| Approved decisions | DEC-003; DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 22-commercial-validation-plan.md; 05-charity-registry-economics.md |
| Research ids | XRI-003; HYP-005; OQ-003; OQ-004 |
| Evidence gates | V-3; V-9 |
| Acceptance criteria | Given the investigation report, When it is reviewed, Then it states strata, sample size and detection power, cost per year, the V-3 replay results it used, and the auditor feedback from V-9, and it makes no change to the deep-review interval. |
| Dependencies | UNB-CDD-CIV-008 |
| Security implications | Sample selection must be unpredictable to staff and charities; the investigation design records the selection method. |
| Failure behaviour | If V-3 or V-9 evidence is unavailable, the investigation records the gap and the item stays open; nothing is adopted by default. |
| Verification method | Investigation report reviewed by the product owner and sponsor compliance |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | deferred |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-010 — Shared public evidence with sponsor-scoped decisions and snapshot pinning

| Field | Value |
|---|---|
| Requirement | The shared layer MUST hold only source-attributed public facts permitted by their rights records (UNB-CDD-CIV-005) and their histories; eligibility decisions, overrides, screening dispositions, match thresholds, identity confirmations and private submissions MUST live in the sponsor's cell. Each sponsor MUST evaluate shared evidence through its own policy version. A sponsor MUST pin a shared-registry snapshot version for non-adverse changes and accept newer versions explicitly; holds derived from re-verified adverse public facts (CNV-GRT-004) MUST apply regardless of the pin. |
| Business rationale | Shared public evidence lowers per-sponsor cost (research: about $18-42k per sponsor at ten sponsors), while pinning limits correlated failure from a bad shared release without delaying adverse facts. |
| Refines / applicable existing requirements | CNV-GRT-004; UNB-CHAR-036; UNB-CNET-010; RR-REQ-X-007-r1 |
| Origin | refinement (parent CNV-GRT-004) |
| Approved decisions | DEC-004; DEC-005 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 05-charity-registry-economics.md; 20-adversarial-review.md |
| Research ids | XRI-002; CLM-035; CHL-062; CHL-063 |
| Evidence gates | — |
| Acceptance criteria | Given sponsor A pinned to snapshot 41 and snapshot 42 renames a charity and adds a revocation for another, When A releases grants, Then the rename is not applied until A accepts snapshot 42 but the revocation hold applies immediately. |
| Dependencies | UNB-CDD-CIV-002; UNB-CDD-CIV-005 |
| Security implications | Pinning is per sponsor cell; the shared layer cannot read which snapshot a sponsor uses beyond routing metadata. |
| Failure behaviour | If a snapshot fails integrity gates (UNB-CNET-021) it MUST NOT be offered for acceptance. |
| Verification method | Automated (isolation and pinning tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-011 — Identity resolution as scored candidate links

| Field | Value |
|---|---|
| Requirement | Identity resolution MUST produce candidate links with scores and the features that produced them; it MUST NOT merge entities that carry financial history, eligibility history or payment destinations without a recorded confirmation, and every merge MUST be reversible. Successor, EIN reuse, group subordinate, fiscal sponsorship and merger MUST be typed, time-bounded relationships. A confirmation is a named decision in the confirming sponsor's layer; it MAY become shared only when it rests solely on public evidence and a registry steward confirms it, and the question of sharing sponsor-confirmed resolutions across sponsors (OQ-010) stays with counsel. |
| Business rationale | AI and similarity scores propose and deterministic gates dispose; financial entities are never auto-merged. |
| Refines / applicable existing requirements | UNB-CHAR-005; UNB-CHAR-009; UNB-CHAR-004 |
| Origin | refinement (parent UNB-CHAR-005) |
| Approved decisions | DEC-005 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 08-frictionless-onboarding.md; 13-compounding-competitive-advantages.md |
| Research ids | XRI-002; CHL-062; OQ-010; CLM-034 |
| Evidence gates | — |
| Acceptance criteria | Given two registry entities with the same name and address and different EINs scored 0.93, When resolution runs, Then a candidate link is recorded with its features, no merge occurs, and grants to either entity are evaluated against that entity's own evidence. |
| Dependencies | UNB-CDD-CIV-002 |
| Security implications | Merges can redirect payments; confirmation requires a permission distinct from data entry (segregation of duties). |
| Failure behaviour | If the resolver is unavailable, existing entities and links are unchanged and new records remain unlinked. |
| Verification method | Automated (resolver tests; merge/unmerge reversibility tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-012 — Typed human review queues for charity intelligence

| Field | Value |
|---|---|
| Requirement | Charity intelligence work that needs a person MUST enter typed review queues (status conflict, identity candidate, evidence refresh failure, eligibility-relevant change, AI-flagged item, sample-audit item, rights-restricted evidence request) with owner, due time, priority and an evidence bundle whose deterministic section AI cannot edit (CNV-AIO-002). Sponsor-scoped items MUST live in the sponsor's cell and any cross-sponsor projection MUST carry routing metadata only (CNV-TEN-001); shared steward items MUST contain public evidence only. Each item MUST record its outcome, handling time and touches as an operational event (UNB-CDD-OEM-001). |
| Business rationale | Typed queues make exception work measurable and keep the human decision point explicit, which DEC-005 and DEC-003 require. |
| Refines / applicable existing requirements | UNB-CHAR-013; UNB-CHAR-030; CNV-TEN-001; CNV-AIO-002 |
| Origin | refinement (parent UNB-CHAR-013) |
| Approved decisions | DEC-003; DEC-005 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 09-event-level-operating-model.md; 05-charity-registry-economics.md |
| Research ids | XRI-002; XRI-006; CLM-012 |
| Evidence gates | — |
| Acceptance criteria | Given a status conflict between Pub 78 and the revocation list for a charity used by two sponsors, When it is detected, Then one shared steward item with public evidence and one item per affected sponsor are created, each sponsor's item is visible only in its cell, and each closure records handling minutes. |
| Dependencies | UNB-CDD-CIV-001; UNB-CDD-OEM-001 |
| Security implications | Queue projections expose no donor PII and no sponsor's interest in a charity to other sponsors. |
| Failure behaviour | If an item passes its due time, it escalates per sponsor policy and the affected eligibility stays held where the item type requires a hold. |
| Verification method | Automated (queue routing and isolation tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CIV-013 — Auditable evidence and decision histories

| Field | Value |
|---|---|
| Requirement | Every evidence record, class assignment, promotion, expiry, refresh, conflict state and eligibility decision MUST be recorded append-only with actor or rule version, reason and the three time axes of UNB-CNET-001, in the Unbound-owned history component (CNV-ARC-004). A decision MUST be replayable as known at decision time; reinstatement, retroactive revocation and corrections MUST be new records, never edits. |
| Business rationale | Auditors and sponsors must be able to reproduce what was relied on; change-driven re-vetting is only defensible if every skipped repetition is explained by recorded evidence. |
| Refines / applicable existing requirements | UNB-CHAR-025; UNB-CHAR-040; CNV-ARC-004; UNB-CNET-001 |
| Origin | refinement (parent UNB-CHAR-025) |
| Approved decisions | DEC-003; DEC-005 |
| Research (daf-landscape research/competitive-displacement/) | 04-charity-intelligence-architecture.md; 20-adversarial-review.md |
| Research ids | XRI-002; XRI-005; CHL-063 |
| Evidence gates | — |
| Acceptance criteria | Given a grant approved on 20 July and a revocation effective 1 May first published 12 August, When the approval is replayed, Then the replay shows the listing as known on 20 July, the later revocation with its valid and publication times, and no edited record. |
| Dependencies | — |
| Security implications | History is tamper-evident; corrections require a distinct permission and leave the original visible. |
| Failure behaviour | If the history write fails, the decision MUST NOT be committed (no decision without its evidence trail). |
| Verification method | Automated (temporal replay tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-ONB-001 — Sponsor-approved validation summary before cutover

| Field | Value |
|---|---|
| Requirement | Cutover authorisation MUST require a validation summary generated from the final rehearsal's evidence (blocking gate results, exception dispositions, disclosed history gaps, attribute-sample results, history depth imported versus available, freeze plan) and approved by a named sponsor authority. The approval MUST be recorded with the summary hash; it MUST NOT waive any blocking gate of UNB-ONB-013, and any change to the evidence after approval MUST invalidate it. |
| Business rationale | Customer acceptance evidence is the payments-industry pattern (customer approves a validation summary) that the existing onboarding records do not yet require. |
| Refines / applicable existing requirements | UNB-ONB-013; UNB-ONB-011; RR-REQ-X-061-r1 |
| Origin | refinement (parent UNB-ONB-013) |
| Approved decisions | DEC-010; DEC-007 |
| Research (daf-landscape research/competitive-displacement/) | 08-frictionless-onboarding.md; 18-transitus-implications.md |
| Research ids | CLM-022; SRC-066 |
| Evidence gates | — |
| Acceptance criteria | Given a final rehearsal with all blocking gates passed and two disclosed history gaps, When the sponsor's controller approves the summary, Then cutover may be authorised; And When a further exception is dispositioned afterwards, Then the approval is invalidated and cutover is blocked until re-approved. |
| Dependencies | UNB-ONB-013 |
| Security implications | The approver must hold the sponsor role for migration acceptance; Unbound staff cannot approve on a sponsor's behalf. |
| Failure behaviour | Without a current approval, cutover authorisation MUST fail closed. |
| Verification method | Automated (cutover gate tests) + rehearsal review |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-ONB-002 — Evidence-selected source cluster and repeatable source packages

| Field | Value |
|---|---|
| Requirement | Vendor-specific source packages MUST be versioned per incumbent platform and report version, carry a layout drift test, and be built only after a customer-authorised sample from that source exists (DEC-011). The first incumbent source cluster on which migrations concentrate MUST be selected by a recorded decision that cites evidence (contract-end calendar per V-5, authorised sample availability, commercial practicality) and MUST NOT be selected arbitrarily. The canonical historical import remains available for every source; adapters over self-service reports MUST be labelled balance-level unless a contracted incumbent export supplies transaction history. |
| Business rationale | Approved DEC-007 concentrates early migrations on one incumbent where commercially practical, preserving V-5; approved DEC-011 defers customer-specific proprietary packages until authorised source data exists. |
| Refines / applicable existing requirements | UNB-ONB-002; UNB-ONB-006; UNB-TRN-006 |
| Origin | refinement (parent UNB-ONB-002) |
| Approved decisions | DEC-007; DEC-011 |
| Research (daf-landscape research/competitive-displacement/) | 08-frictionless-onboarding.md; 18-transitus-implications.md; 22-commercial-validation-plan.md |
| Research ids | HYP-011; HYP-012; OQ-012; SRC-066 |
| Evidence gates | V-5 |
| Acceptance criteria | Given no customer-authorised sample from incumbent X, When a package for X is proposed, Then it is refused and the canonical import is offered; And Given the source-cluster decision record, When it is reviewed, Then it cites the V-5 evidence used. |
| Dependencies | — |
| Security implications | Authorised samples contain donor data and are handled under the sponsor's MSA and DPA only. |
| Failure behaviour | If drift is detected on a report version, the package MUST stop accepting that version until updated. |
| Verification method | Review of the source-cluster decision record + automated drift tests |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-ONB-003 — Process SLAs only after a measured rehearsal

| Field | Value |
|---|---|
| Requirement | Any migration service commitment MUST be a process SLA covering only Unbound-controlled steps, with the clock starting on receipt of a conforming extract against a published exclusions list, fee-credit remedies and liability capped as the contract states. It MUST NOT be offered until at least one measured rehearsal on an authorised real incumbent export has passed every blocking gate (V-4) and the commitment text has passed contractual and legal review. Outcome guarantees (e.g. a guaranteed go-live date independent of incumbent or sponsor inputs) MUST NOT be offered. |
| Business rationale | Approved DEC-010 permits evidence-based process SLAs only after successful rehearsals and validation, preserving V-4 and contractual review. |
| Refines / applicable existing requirements | UNB-ONB-030; UNB-ONB-031 |
| Origin | refinement (parent UNB-ONB-030) |
| Approved decisions | DEC-010 |
| Research (daf-landscape research/competitive-displacement/) | 08-frictionless-onboarding.md; 22-commercial-validation-plan.md |
| Research ids | CLM-022; HYP-003; OQ-005 |
| Evidence gates | V-4 |
| Acceptance criteria | Given no rehearsal on an authorised export has passed, When a proposal includes a migration SLA, Then the commercial record is refused; And Given a passed rehearsal and approved contract text, Then only Unbound-controlled steps carry the SLA. |
| Dependencies | UNB-CDD-ONB-001 |
| Security implications | Not applicable beyond contract confidentiality. |
| Failure behaviour | If the clock's start condition is disputed, the SLA does not start until a conforming extract is acknowledged in writing. |
| Verification method | Review of commercial records against the gate |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-ONB-004 — Onboarding maturity claimed only from measured data

| Field | Value |
|---|---|
| Requirement | Any claim about onboarding maturity or repeatability MUST be computed from measured migrations, each metric stating numerator, denominator, window and minimum sample: share of fields mapped by package defaults; engineering changes per migration; mock-to-live control-total delta; days from conforming extract to go-live; post-go-live defects per 1,000 funds at 30 and 90 days. A level MUST NOT be claimed while the minimum sample is unmet. |
| Business rationale | The research's maturity model is conceptual; at two to four migrations a year higher levels cannot be evidenced for years (CHL-076). |
| Refines / applicable existing requirements | UNB-ONB-030; UNB-ONB-032 |
| Origin | refinement (parent UNB-ONB-030) |
| Approved decisions | DEC-010; DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 08-frictionless-onboarding.md |
| Research ids | CLM-020; SRC-066 |
| Evidence gates | — |
| Acceptance criteria | Given two completed migrations and a minimum sample of five, When a maturity report is produced, Then it shows the metrics and states that no level can yet be claimed. |
| Dependencies | UNB-CDD-OEM-002 |
| Security implications | Metrics are aggregated per sponsor and never disclose another sponsor's data. |
| Failure behaviour | Missing metric inputs make the metric unavailable, never zero. |
| Verification method | Automated (metric computation tests) |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CMF-001 — Operating modes as configuration, not forks

| Field | Value |
|---|---|
| Requirement | The platform MUST support these operating modes by configuration in the Sponsor Configuration Package, without sponsor- or partner-specific code: (a) technology-only licence, the sponsor operating every exception; (b) partner-operated administration under a per-sponsor ServiceDelegation (CNV-TEN-001); (c) a module embedded in a channel partner's offering (white-label); (d) full-platform licence; (e) optional operational services supplied by a separately contracted operator. The mode and its role table MUST be recorded per sponsor; every mode MUST carry the same mandatory controls; a change of mode MUST be a material configuration change under CNV-GOV-005. |
| Business rationale | Approved DEC-001 (partner-first entry, then licensing) and DEC-006 (licensing first) need commercial flexibility without forking the product or weakening controls. |
| Refines / applicable existing requirements | UNB-ONB-023; CNV-TEN-001; CNV-GOV-005; UNB-ONB-024 |
| Origin | refinement (parent UNB-ONB-023) |
| Approved decisions | DEC-001; DEC-006; DEC-002 |
| Research (daf-landscape research/competitive-displacement/) | 14-alternative-enterprise-models.md; 16-commercial-entry-strategy.md; 10-zero-touch-administration-economics.md |
| Research ids | XRI-008; XRI-001; CLM-014; SRC-063 |
| Evidence gates | — |
| Acceptance criteria | Given sponsor S configured in mode (b) with partner P holding a ServiceDelegation for exception handling, When S moves to mode (a), Then the change requires independent approval, P's delegation is revoked at the effective time, and no control changes. |
| Dependencies | — |
| Security implications | Mode determines which principals may act; delegations are scoped and expire (CNV-TEN-001). |
| Failure behaviour | A configuration naming an unsupported mode or omitting the role table MUST fail activation. |
| Verification method | Automated (configuration validation tests per mode) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | architectural-support |
| Open questions | — |
| Conflicts | DEC-001 partner-first entry vs ratified CNV-ECO-004 entry segment (OD-M-05); escalated as OD-CDD-01; the ratified segment governs now, so modes (b) and (c) are architectural support only. |

### UNB-CDD-CMF-002 — Release 1 operable with no Unbound staff in sponsor workflows

| Field | Value |
|---|---|
| Requirement | Release 1 MUST be operable in technology-only licensing mode: every exception queue, review case, reconciliation break and support case MUST be routable to sponsor principals or to a delegated partner, and no sponsor workflow step MUST require an Unbound operator role. Unbound personnel MAY act only in platform operations (security, deployment, incident response) under segregation of duties, and any support access to sponsor data MUST be break-glass, time-bounded and visible to the sponsor in its audit trail. |
| Business rationale | XRI-008: the cheapest self-sustaining modes (MD-6, MD-7) need Release 1 to run with the sponsor or a partner operating exceptions. |
| Refines / applicable existing requirements | CNV-TEN-001; CNV-GOV-004; CNV-SVC-001 |
| Origin | refinement (parent CNV-TEN-001) |
| Approved decisions | DEC-001; DEC-006; DEC-002 |
| Research (daf-landscape research/competitive-displacement/) | 14-alternative-enterprise-models.md; 10-zero-touch-administration-economics.md; 17-unbound-implications.md |
| Research ids | XRI-008; CLM-014; CHL-058 |
| Evidence gates | — |
| Acceptance criteria | Given a sponsor in technology-only mode, When every Release-1 case kind is generated in a test sponsor, Then each routes to a sponsor or delegated principal and none requires an Unbound role; And When Unbound support opens a break-glass session, Then the sponsor sees it in its audit trail. |
| Dependencies | UNB-CDD-CMF-001 |
| Security implications | Removes standing vendor access to sponsor data; break-glass is logged and expires. |
| Failure behaviour | A case kind with no eligible assignee MUST alert the sponsor's configured owner rather than defaulting to Unbound staff. |
| Verification method | Automated (routing coverage test over all case kinds) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CMF-003 — Partner-embedded and white-label delivery without changing funds flow

| Field | Value |
|---|---|
| Requirement | Capabilities offered inside a channel partner's offering MUST be delivered through the versioned APIs and brand tokens of the platform, with the partner's or sponsor's branding and no Unbound brand required. Every partner deployment MUST pass the CNV-LEG-001 funds-flow conformance check and the CNV-PAY-001 role table before activation, and partner isolation MUST follow CNV-TEN-001 (dedicated sponsor cell or separate legal sponsor where a partner needs dedicated infrastructure). A partner MUST NOT see data of sponsors it is not delegated for. |
| Business rationale | Approved DEC-001 enters through a module inside an existing channel partner's offering; that must not alter the ratified legal perimeter. |
| Refines / applicable existing requirements | CNV-LEG-001; CNV-TEN-001; REQ-X-045 |
| Origin | refinement (parent CNV-LEG-001) |
| Approved decisions | DEC-001 |
| Research (daf-landscape research/competitive-displacement/) | 16-commercial-entry-strategy.md; 11-institutional-distribution.md; 14-alternative-enterprise-models.md |
| Research ids | XRI-001; XRI-008; CLM-023; CLM-025 |
| Evidence gates | — |
| Acceptance criteria | Given a partner deployment whose configuration routes donor money through a partner-held pooled account, When activation runs, Then the funds-flow conformance check fails and activation is blocked. |
| Dependencies | UNB-CDD-CMF-001 |
| Security implications | API credentials are per partner and per sponsor delegation; white-label does not relax authentication or audit. |
| Failure behaviour | A failed conformance check blocks activation; no partial activation. |
| Verification method | Automated (conformance and isolation tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | architectural-support |
| Open questions | — |
| Conflicts | Partner-first entry (DEC-001) is not yet reflected in CNV-ECO-004; see OD-CDD-01. |

### UNB-CDD-CMF-004 — Modular offerings with controls that cannot be unbundled

| Field | Value |
|---|---|
| Requirement | Licensable modules (for example charity evidence and eligibility, onboarding and migration, grant workflow, contributions, reconciliation, and the full platform) MUST be declared in a versioned module catalogue with their dependencies and the mandatory controls that travel with each module. A configuration that omits a mandatory control of a licensed module (eligibility checks at approval and release, sanctions screening, segregation of duties, append-only audit, funds-flow conformance, deterministic financial controls) MUST be rejected. A charity evidence module licensed to a non-sponsor MUST expose only rights-permitted shared public facts (UNB-CDD-CIV-005). |
| Business rationale | Modular commercial offerings must not become a route to removing mandatory controls, which the owner's approval does not authorise. |
| Refines / applicable existing requirements | UNB-ONB-024; CNV-GOV-005; CNV-LEG-001 |
| Origin | refinement (parent UNB-ONB-024) |
| Approved decisions | DEC-006; DEC-004 |
| Research (daf-landscape research/competitive-displacement/) | 14-alternative-enterprise-models.md; 16-commercial-entry-strategy.md; 05-charity-registry-economics.md |
| Research ids | XRI-008; CLM-028; HYP-007 |
| Evidence gates | — |
| Acceptance criteria | Given a configuration licensing grant workflow without sanctions screening, When it is validated, Then it is rejected naming the missing control. |
| Dependencies | UNB-CDD-CMF-001; UNB-CDD-CIV-005 |
| Security implications | Control coverage is checked at activation and on every configuration change. |
| Failure behaviour | An unknown module identifier fails activation. |
| Verification method | Automated (catalogue validation tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | architectural-support |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CMF-005 — Multiple independent sponsors and operators; no Echelon Foundry role required

| Field | Value |
|---|---|
| Requirement | Every operating mode MUST support multiple independent legal sponsors, each with its own cell and role table, and multiple administrators or partners, each acting under per-sponsor ServiceDelegations. No mode MUST require Echelon Foundry, or any entity affiliated with it, to act as sponsor, administrator, Originator, ODFI, Third-Party Sender or operator of sponsor workflows; the platform provider's roles are those of CNV-PAY-001 and CNV-LEG-001. Activation MUST reject a configuration that hard-wires a specific operator entity into a sponsor role. |
| Business rationale | Global constraint for this initiative: Unbound must not require Echelon Foundry to become a DAF sponsor or administrator; independent sponsors and partners are the licensing route (DEC-001, DEC-002, DEC-006). |
| Refines / applicable existing requirements | CNV-TEN-001; CNV-PAY-001; CNV-LEG-001 |
| Origin | refinement (parent CNV-TEN-001) |
| Approved decisions | DEC-001; DEC-002; DEC-006 |
| Research (daf-landscape research/competitive-displacement/) | 14-alternative-enterprise-models.md; 19-echelon-foundry-implications.md; 16-commercial-entry-strategy.md |
| Research ids | XRI-008; SRC-063 |
| Evidence gates | — |
| Acceptance criteria | Given two unaffiliated sponsors and one administrator delegated by both, When each sponsor's role table is validated, Then neither names an Echelon Foundry entity, the administrator's access is limited to each delegation, and activation succeeds. |
| Dependencies | — |
| Security implications | Delegations are per sponsor; one administrator cannot correlate sponsors' data outside routing metadata. |
| Failure behaviour | A role table that cannot be validated blocks activation. |
| Verification method | Automated (role-table validation tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CMF-006 — Role and assurance record per operating mode (TPSP and SOC 1)

| Field | Value |
|---|---|
| Requirement | For every operating mode, the CNV-PAY-001 role table MUST name each party that performs an ACH processing function (creating or transmitting entries or files, holding payee bank details, operating release workflow) as a Third-Party Service Provider; the platform provider MUST remain a contracted TPSP wherever the platform creates entries or files, and no mode may assume otherwise from the absence of origination. The CNV-GOV-001 planned-approach record per sponsor or licensee MUST state whether any user auditor intends to rely on platform controls, which is the existing SOC 1 Type 2 planning trigger; this requirement creates no new SOC trigger. Allocation of Nacha fraud-monitoring duties between Unbound and a transmitting partner remains OD-CNO-12. |
| Business rationale | XRI-008 revision 2 requires the licensing mode to treat the platform as a Nacha TPSP and plan for SOC 1 Type II (CHL-037, CHL-066); the ratified clauses already require both, so this records them per mode. |
| Refines / applicable existing requirements | CNV-PAY-001; CNV-GOV-001; UNB-CNET-023 |
| Origin | refinement (parent CNV-PAY-001) |
| Approved decisions | DEC-001; DEC-006 |
| Research (daf-landscape research/competitive-displacement/) | 14-alternative-enterprise-models.md; 05-charity-registry-economics.md; 20-adversarial-review.md |
| Research ids | XRI-008; CHL-037; CHL-066; OQ-009 |
| Evidence gates | — |
| Acceptance criteria | Given a technology-only licence where the sponsor operates exceptions and the platform generates ACH files, When the role table is validated, Then the platform provider is recorded as TPSP with its contract reference; And Given the sponsor's auditor states an intent to rely on platform controls, Then the planned-approach record triggers SOC 1 Type 2 planning under CNV-GOV-001. |
| Dependencies | UNB-CDD-CMF-001 |
| Security implications | Keeps Nacha risk-based fraud monitoring and audit cooperation obligations attached to the party that performs them. |
| Failure behaviour | A role table without a TPSP for a file-creating mode MUST block activation. |
| Verification method | Automated (role-table validation) + counsel review of TPSP allocation |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-CMF-007 — Licence-first pricing basis for initial offerings

| Field | Value |
|---|---|
| Requirement | Economic models, commercial records and requirement packages for initial offerings MUST use licence or subscription pricing (per account, per module or service subscription) as the primary basis; asset-based administration fees (bps) MAY be modelled only for a later administration offer and MUST be labelled as working assumptions, not approved prices. Prices remain set by the Unbound board under OD-M-11; no price MUST be described as validated by buyers until V-2 evidence exists. |
| Business rationale | Approved DEC-006 prioritises licensing or subscription over asset-based administration fees for initial offerings, preserving V-2; licensing is the mode least sensitive to handling minutes in the research. |
| Refines / applicable existing requirements | CNV-ECO-003; UNB-ONB-032; CNV-ECO-002 |
| Origin | refinement (parent CNV-ECO-003) |
| Approved decisions | DEC-006 |
| Research (daf-landscape research/competitive-displacement/) | 16-commercial-entry-strategy.md; 10-zero-touch-administration-economics.md; 22-commercial-validation-plan.md |
| Research ids | CLM-013; CLM-014; OQ-002 |
| Evidence gates | V-2 |
| Acceptance criteria | Given an economics run for an initial licensing offer, When it is published, Then its primary revenue line is licence or subscription, any bps line is labelled 'working assumption (administration offer)', and no buyer-validation claim appears without V-2 evidence. |
| Dependencies | — |
| Security implications | Not applicable. |
| Failure behaviour | A model missing the pricing-basis label fails the economics review checklist. |
| Verification method | Review of economics outputs |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | Should the existing 20 bps working assumptions in OD-CNO-10 and FIL-OD-10 be restated on a licence-first basis (OD-CDD-03)? |
| Conflicts | Existing open or agent-recommended decisions OD-CNO-10 and FIL-OD-10 reason from a ~20 bps servicing fee; escalated as OD-CDD-03, not edited. |

### UNB-CDD-OEM-001 — Operational event records for every event occurrence

| Field | Value |
|---|---|
| Requirement | Unbound MUST maintain a versioned catalogue of operational event types (initially mapped to the research's 25 events and to Unbound workflows), each with expertise class, controls and applicable prevention mechanisms. Every occurrence MUST produce an event record with: event type and catalogue version; sponsor; start and terminal timestamps; outcome (straight-through, touched, exception, failed); touches (count, role class, active minutes); exception reason class; failure lifecycle (detected, contained, corrected, external cost, loss); prevention mechanism applied; AI assistance used; policy and rule versions. Event records MUST be derived from workflow and case state transitions, not entered separately, and MUST be non-posting. |
| Business rationale | XRI-006: all economics rest on unmeasured touch rates; event-level records let measured rates replace modelled ones. |
| Refines / applicable existing requirements | CNV-ECO-001; CNV-ECO-003; UNB-CNET-025; UNB-ING-OPS-002 |
| Origin | refinement (parent CNV-ECO-001) |
| Approved decisions | DEC-006; DEC-008; DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 09-event-level-operating-model.md; 10-zero-touch-administration-economics.md; 17-unbound-implications.md |
| Research ids | XRI-006; CLM-011; CLM-012; SRC-063 |
| Evidence gates | — |
| Acceptance criteria | Given a grant recommendation that passes every deterministic check and releases without a touch, When it reaches a terminal state, Then one event record shows outcome straight-through, zero touches and the rule versions; And Given a contribution that needed attribution by a person for 6 active minutes, Then its record shows outcome touched, one touch of role 'operations', 6 minutes and reason 'unidentified receipt'. |
| Dependencies | — |
| Security implications | Event records reference cases by identifier and hold no donor PII beyond what the case store holds; access follows the case's sponsor scoping. |
| Failure behaviour | A transition without an event record MUST raise an instrumentation exception; missing records make metrics unavailable rather than understated. |
| Verification method | Automated (instrumentation coverage test over all workflow transitions) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-OEM-002 — Versioned operational economics metrics

| Field | Value |
|---|---|
| Requirement | Unbound MUST compute, per sponsor and in aggregate, from event records: straight-through rate; human intervention rate; exception rate; exception handling time (active and elapsed); prevention rate; capacity and utilisation against declared staffing; cost attribution per event (via CNV-ECO-001 attribution); service quality (defects, reversals, rework, SLA attainment); latency from initiating input to terminal outcome; and control effectiveness (control test pass rate, false-hold and false-clear rates, time to detect material adverse events). Each metric MUST be versioned with numerator, denominator, window, minimum sample, exclusions and owner, and MUST be reproducible with information-produced-by-entity support (CNV-GOV-001). |
| Business rationale | XRI-006 and XRI-007 need measured rates for touch, exception, failure and prevention, and the brief requires these specific measures. |
| Refines / applicable existing requirements | UNB-ING-OPS-002; RR-REQ-X-007-r2; CNV-GOV-001; CNV-SVC-002 |
| Origin | refinement (parent UNB-ING-OPS-002) |
| Approved decisions | DEC-006; DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 09-event-level-operating-model.md; 10-zero-touch-administration-economics.md |
| Research ids | XRI-006; XRI-007; CLM-011; CLM-037 |
| Evidence gates | — |
| Acceptance criteria | Given 1,000 grant events in a month with 940 straight-through, 50 touched and 10 failed, When metrics run, Then the straight-through rate is 94.0% under metric version v1, the report states the window and sample, and re-running with the same inputs produces the same hash. |
| Dependencies | UNB-CDD-OEM-001 |
| Security implications | Metrics expose aggregates; drill-down follows sponsor scoping. |
| Failure behaviour | A metric below its minimum sample is reported as 'insufficient sample', never as a value. |
| Verification method | Automated (metric reproducibility tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-OEM-003 — Measured values replace modelled assumptions

| Field | Value |
|---|---|
| Requirement | Every parameter of Unbound's economic models (touch probability, minutes per touch, failure probability, prevention efficacy, AI handling factor, eligibility-relevant change rate) MUST be linked to the metric that measures it and labelled modelled or measured. A parameter becomes measured only when its metric meets the minimum sample over its window; measured values MUST then replace modelled ones in every subsequent economics run, and claims MUST be restated under the CNV-ECO-003 withdrawal rule. A model MUST NOT override a measured value. |
| Business rationale | The research is explicit that every figure is conditional on labelled inputs and none is measured; DEC-008 and DEC-012 require evidence gates rather than modelled assumptions. |
| Refines / applicable existing requirements | CNV-ECO-003; CNV-ECO-005; UNB-ONB-032 |
| Origin | refinement (parent CNV-ECO-003) |
| Approved decisions | DEC-008; DEC-012; DEC-006 |
| Research (daf-landscape research/competitive-displacement/) | 10-zero-touch-administration-economics.md; 09-event-level-operating-model.md; 21-evidence-gaps.md |
| Research ids | XRI-006; HYP-006; HYP-008; OQ-003; OQ-011 |
| Evidence gates | V-3; V-7 |
| Acceptance criteria | Given the minutes-per-touch metric for charity vetting reaches its minimum sample at 31 minutes against a modelled 45, When the next economics run executes, Then it uses 31, labels it measured with sample and window, and the change is listed in the restatement log. |
| Dependencies | UNB-CDD-OEM-002 |
| Security implications | Not applicable beyond sponsor confidentiality of per-sponsor values. |
| Failure behaviour | If the metric falls below minimum sample in a later window, the last measured value stands with its window shown; it does not revert silently to the model. |
| Verification method | Review of economics runs + automated parameter-link check |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-OEM-004 — Handling time captured from work state, not timesheets

| Field | Value |
|---|---|
| Requirement | Active handling minutes MUST be derived from task and case lifecycle events (claimed, active, paused, completed) with idle detection, with an optional recorded adjustment by the worker; self-reported timesheets MUST NOT be the primary source. Economics MUST use role-class aggregates; individual-level timing records MUST be access-restricted under the data classification and MUST NOT be shown outside the sponsor's authorised supervisory roles. |
| Business rationale | Measured minutes per touch are the dominant uncertain input; capture must be low-burden and protect workers' data. |
| Refines / applicable existing requirements | CNV-SVC-002; CNV-ECO-001 |
| Origin | refinement (parent CNV-SVC-002) |
| Approved decisions | DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 09-event-level-operating-model.md; 10-zero-touch-administration-economics.md |
| Research ids | XRI-006; HYP-008 |
| Evidence gates | — |
| Acceptance criteria | Given a case claimed at 10:00, paused 10:05-10:20 and completed at 10:27, When handling time is derived, Then active minutes are 12 and the record shows the derivation. |
| Dependencies | UNB-CDD-OEM-001 |
| Security implications | Individual timing data is personal data; access is role-restricted and audited. |
| Failure behaviour | Gaps in lifecycle events mark the touch's minutes as unknown, excluded from averages and counted in a data-quality metric. |
| Verification method | Automated (derivation tests) |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-OEM-005 — Prevention mechanisms first, each with a measured efficacy

| Field | Value |
|---|---|
| Requirement | Release-1 workflows MUST include these prevention mechanisms ahead of AI exception handling: notification-first contribution intents with unique references (UNB-ING-DAF-001); validated grant recommendation forms (charity selected from the registry, purpose from a sponsor taxonomy, required fields enforced); recommendation-time policy pre-validation (UNB-CDD-OEM-006); and verified payment destinations (CNV-PAY-002, UNB-CNET-014). Each mechanism MUST have an efficacy metric (share of events arriving compliant with the mechanism and exception rate with versus without it) computed from event records. |
| Business rationale | XRI-007: prevention outranks AI speed where an upstream lever exists (CLM-011); its efficacy depends on partner cooperation, so it must be measured rather than assumed (CHL-060). |
| Refines / applicable existing requirements | UNB-ING-DAF-001; CNV-PAY-002; UNB-CNET-014; CNV-GRT-003 |
| Origin | refinement (parent UNB-ING-DAF-001) |
| Approved decisions | DEC-009; DEC-012 |
| Research (daf-landscape research/competitive-displacement/) | 10-zero-touch-administration-economics.md; 09-event-level-operating-model.md; 17-unbound-implications.md |
| Research ids | XRI-007; CLM-011; CHL-060; SRC-004 |
| Evidence gates | — |
| Acceptance criteria | Given contributions over a month where 70% carry an intent reference, When efficacy is computed, Then the attribution exception rate is reported separately for contributions with and without a reference. |
| Dependencies | UNB-CDD-OEM-001; UNB-CDD-OEM-006 |
| Security implications | Prevention never bypasses controls: pre-validation is advisory and approval and release checks still run. |
| Failure behaviour | If a prevention mechanism is unavailable (e.g. no virtual-account product at the bank), the workflow proceeds with downstream controls and the metric records the mechanism as not applied. |
| Verification method | Automated (workflow tests) + metric review |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CDD-OEM-006 — Recommendation-time policy pre-validation

| Field | Value |
|---|---|
| Requirement | When a donor or advisor prepares a grant recommendation, the platform MUST evaluate the sponsor's deterministic policy rules and current charity status against the draft and show any blocking reason before submission. The pre-check MUST be advisory: it MUST NOT approve a grant, and approval-time and release-time checks under CNV-GRT-003 MUST still run. The pre-check result and rule versions MUST be recorded with the recommendation. |
| Business rationale | Approval exceptions are the largest residual cost in the research model; catching them at entry is the prevention lever for grant approval (XRI-007). |
| Refines / applicable existing requirements | CNV-GRT-003; UNB-CHAR-020; CNV-AIO-002 |
| Origin | refinement (parent CNV-GRT-003) |
| Approved decisions | DEC-005 |
| Research (daf-landscape research/competitive-displacement/) | 10-zero-touch-administration-economics.md; 09-event-level-operating-model.md |
| Research ids | XRI-007; CLM-012 |
| Evidence gates | — |
| Acceptance criteria | Given a recommendation to a charity on the sponsor's exclusion list, When the donor enters it, Then the form shows the blocking reason before submission; And Given a recommendation that passes the pre-check, When the charity is revoked before approval, Then the approval-time check holds it. |
| Dependencies | — |
| Security implications | The pre-check reveals only the donor-facing reason text configured by the sponsor, not investigation content (CNV-GRT-005). |
| Failure behaviour | If the pre-check cannot run, submission is allowed with a notice and the approval-time check governs. |
| Verification method | Automated (UI and rule tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |
<!-- END GENERATED -->

# 02 — Charity network and onboarding requirements (UNB-CNET, UNB-ONB)

Mission: [README](README.md) · Source data: [`data/gap-requirements.jsonl`](data/gap-requirements.jsonl) · Status: **proposed**. None of these records is adopted into the authoritative baseline, and none is implemented or validated. Adoption follows the existing ratification process (`requirements/questions/`, convergence clauses).

Every record either refines existing requirements (listed under *Refines*) or is a new requirement from gap analysis. Where a converged clause (`CNV-*`) already states the behaviour, a record adds only what the clause does not cover. On conflict the clause governs (baseline precedence rule), and the conflict is listed as an owner decision in [04](04-DECISIONS-AND-OPEN-QUESTIONS.md).

<!-- BEGIN GENERATED -->
### UNB-CNET-001 — Three time axes on every charity claim

| Field | Value |
|---|---|
| Requirement | Every charity claim and status observation MUST carry valid time (when the fact became true), source-publication time (the posting date of the source release that first contained it, taken from the file's content and Last-Modified/hash, not the web page label) and recorded time (when Unbound learned it). Eligibility evaluations MUST record all three for every claim relied on. |
| Business rationale | IRS reliance under Rev. Proc. 2018-32 §4.02/§4.04 turns on public posting, which lags effective revocation by a median 88-89 days; bitemporal records alone cannot reproduce whether a grantor could rely on a listing. |
| Refines / applicable existing requirements | UNB-KNOW-002; UNB-CHAR-008; RR-REQ-X-007-r1 |
| Origin | refinement (parent UNB-KNOW-002) |
| Supporting research (daf-landscape package) | 05-charity-registry-design.md; 04-authoritative-charity-data-inventory.md; 06-continuous-vetting.md |
| Acceptance criteria | Given a revocation effective 1 May and first posted in the 12 August release, When a grant approved on 20 July is replayed, Then the evaluation shows the organisation listed as of publication time on 20 July, the later-learned revocation with its valid time, and the reliance basis used. |
| Dependencies | UNB-CNET-003 |
| Echelon dependencies | Unbound-owned append-only history component on managed PostgreSQL (CNV-ARC-004); Strata for DDL governance only |
| Security implications | Publication time cannot be supplied by a submitter; it derives only from ingested source releases, preventing back-dating of reliance. |
| Failure behaviour | If a claim lacks a publication time it MUST be treated as unpublished for reliance purposes (not listed), never defaulted to recorded time. |
| Verification method | Automated (temporal replay tests over archived IRS releases) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-002 — Per-predicate source authority lattice

| Field | Value |
|---|---|
| Requirement | Claim precedence MUST be defined per predicate (e.g. exempt status, legal name, subordinate inclusion, contact, website, bank destination) in a versioned authority table. A claim from a lower-authority source for that predicate MAY create a dispute and review task but MUST NOT replace, hide or reorder the current higher-authority claim. |
| Business rationale | The IRS outranks a charity on exempt status; the charity outranks the IRS on its website; a central organisation outranks both on subordinate inclusion. Source-level precedence either over-trusts submitters or blocks legitimate corrections. |
| Refines / applicable existing requirements | UNB-CHAR-012; UNB-EXT-030; UNB-CHAR-014 |
| Origin | refinement (parent UNB-CHAR-012) |
| Supporting research (daf-landscape package) | 05-charity-registry-design.md; 09-charity-network-security.md |
| Acceptance criteria | Given a charity submission stating it is a public charity while the latest BMF shows a private foundation, When the submission is processed, Then the BMF claim remains current, a dispute record links both claims, and no eligibility evaluation changes until a higher-authority source agrees. |
| Dependencies | UNB-CNET-001 |
| Echelon dependencies | — |
| Security implications | Prevents data poisoning by malicious directory submissions (threat model T-series in CNO/09). |
| Failure behaviour | On an authority-table gap (predicate without a defined ranking) the system MUST refuse to auto-resolve and route to stewardship. |
| Verification method | Automated (property tests over the authority table) + Stewardship review sample |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-003 — Release-based ingestion of public charity sources

| Field | Value |
|---|---|
| Requirement | Federal charity sources (EO BMF, Pub 78 data, Automatic Revocation list, Form 990-N, Form 990 e-file indexes, OFAC lists) MUST be ingested from their published fixed file URLs, each release versioned by Last-Modified time and SHA-256 and archived immutably (BMF monthly, 990-N weekly, revocation and Pub 78 monthly) because the sources overwrite history. Ingestion MUST NOT crawl pages disallowed by the publisher's robots rules (e.g. TEOS search). |
| Business rationale | All federal inputs are public-domain, subscription-free bulk files (about 6.3 million rows, about 1 GB per monthly cycle) but none preserves its own history and IRS page dates lag file contents by up to four weeks. |
| Refines / applicable existing requirements | UNB-EXT-001; UNB-KNOW-026; UNB-CHAR-011 |
| Origin | refinement (parent UNB-EXT-001) |
| Supporting research (daf-landscape package) | 04-authoritative-charity-data-inventory.md; 05-charity-registry-design.md |
| Acceptance criteria | Given an unchanged BMF file re-downloaded, When ingestion runs, Then no new release is created; Given a changed file, When ingestion runs, Then a new immutable release with hash, Last-Modified and row counts is recorded and the prior release remains retrievable. |
| Dependencies | UNB-CNET-023 |
| Echelon dependencies | — |
| Security implications | Archived releases are the evidence base for reliance; integrity of archives is protected by hashes recorded at ingestion. |
| Failure behaviour | If a source is unreachable the last release stays current until its freshness limit (UNB-CNET-004) is exceeded; no partial file is ever promoted. |
| Verification method | Automated (ingestion idempotency and archive integrity tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-004 — Freshness measured against the latest publication

| Field | Value |
|---|---|
| Requirement | A source MUST be considered current when Unbound holds its latest published release, and stale only when (a) a newer release has been published and not ingested within the source's freshness limit, or (b) no release has been observed within the source's maximum expected interval. Default limits (policy defaults, owner: sponsor compliance): BMF, Pub 78 and revocation list 45 days; 990-N 14 days; OFAC 24 hours; California Registry of Charities adverse list 21 days. |
| Business rationale | A wall-clock reading of CNV-GRT-003's fail-safe rule would hold every payment whenever the IRS publishes late; staleness is a property of Unbound lagging the publisher, not of the publisher's cadence. |
| Refines / applicable existing requirements | UNB-RCN-GRT-014; UNB-CHAR-011; CNV-GRT-003 |
| Origin | refinement (parent UNB-RCN-GRT-014) |
| Supporting research (daf-landscape package) | 06-continuous-vetting.md; 04-authoritative-charity-data-inventory.md |
| Acceptance criteria | Given the IRS has not published a new Pub 78 release for 40 days and Unbound holds the latest, When a grant is released, Then Pub 78 is treated as current; Given the IRS published a release 50 days ago that Unbound failed to ingest, When a grant is released, Then the check fails safe. |
| Dependencies | UNB-CNET-003 |
| Echelon dependencies | — |
| Security implications | None beyond CNV-GRT-003. |
| Failure behaviour | When a source is stale, release-time checks fail safe per CNV-GRT-003; approvals record the stale state. |
| Verification method | Automated (clock-controlled freshness tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Does the owner accept 'stale = behind the latest publication' as the reading of CNV-GRT-003's fail-safe rule (OD-CNO-04)? |
| Conflicts | Clarifies CNV-GRT-003 'fail safe when stale'; requires owner confirmation (OD-CNO-04). |

### UNB-CNET-005 — Cross-source status conflicts produce a review state

| Field | Value |
|---|---|
| Requirement | When authoritative federal files disagree about an organisation's status (e.g. a revocation without reinstatement on the revocation list while the EIN remains in the current BMF or Pub 78), the registry MUST record a StatusConflict with both claims and MUST NOT resolve it to Eligible or Ineligible automatically; sponsor policy determines whether new approvals hold pending review, using the reliance analysis recorded for the conflict class. |
| Business rationale | About 31,600 EINs whose latest revocation has no reinstatement remain on the BMF (about 19,700 also in Pub 78; exact counts 31,579-31,614 depending on tie-breaking). About 71% carry a later BMF ruling and look re-recognised; about 9,200 remain genuinely ambiguous, so neither 'revoked' nor 'listed' is safe as a default. |
| Refines / applicable existing requirements | UNB-CHAR-016; CNV-GRT-003 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 04-authoritative-charity-data-inventory.md; 06-continuous-vetting.md; 22-adversarial-review.md |
| Acceptance criteria | Given an EIN on the revocation list without reinstatement and present in the latest Pub 78, When eligibility is evaluated, Then the result is NeedsReview with a StatusConflict reason citing both releases, and no automated approval occurs; Given the same EIN with a BMF ruling date later than the revocation, When evaluated under a counsel-approved rule for re-recognition, Then the conflict class is ReRecognised and the rule's outcome is applied automatically. |
| Dependencies | UNB-CNET-002 |
| Echelon dependencies | — |
| Security implications | Avoids both wrongful grants to revoked organisations and wrongful holds that harm legitimate charities. |
| Failure behaviour | Unresolved conflicts older than the sponsor's review SLA escalate; they never auto-clear. |
| Verification method | Automated (fixtures drawn from real conflict classes) + counsel review of reliance analysis |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Does a Pub 78/BMF listing after an unreinstated revocation constitute a 'subsequent listing' under Rev. Proc. 2018-32 §4.06 (OD-CNO-13)? |
| Conflicts | — |

### UNB-CNET-006 — Group-exemption subordinate eligibility evidence

| Field | Value |
|---|---|
| Requirement | Eligibility of a group-exemption subordinate MUST require (a) the central organisation's current status and group exemption number from the latest BMF and (b) inclusion evidence for the subordinate: a central-issued roster or letter with an issue date and expiry (default 12 months, sponsor may shorten). Evidence MUST be cached per group and reused across subordinates and sponsors as public-fact evidence; a Pub 78 lookup alone MUST NOT be used for subordinates. |
| Business rationale | All 237,380 501(c)(3) group subordinates in the BMF are absent from Pub 78 and Rev. Proc. 2018-32 §6.03 excludes subordinates from reliance on the lists, so central-organisation evidence is the normal path, not an exception. |
| Refines / applicable existing requirements | UNB-RCN-GRT-013; CNV-GRT-003 |
| Origin | refinement (parent UNB-RCN-GRT-013) |
| Supporting research (daf-landscape package) | 05-charity-registry-design.md; 04-authoritative-charity-data-inventory.md |
| Acceptance criteria | Given a subordinate listed in the BMF under GEN 1234 whose central is in good standing and a central roster dated 3 months ago that includes it, When a second sponsor evaluates it, Then the cached roster evidence is reused and the evaluation passes without a new human task. |
| Dependencies | UNB-CNET-002 |
| Echelon dependencies | — |
| Security implications | Roster documents are untrusted input: AI may extract names but inclusion is confirmed deterministically against EIN/GEN fields (CNV-AIO-002). |
| Failure behaviour | Expired or missing roster evidence yields InsufficientEvidence, never Ineligible. |
| Verification method | Automated + stewardship sample |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-007 — Field-and-direction materiality table

| Field | Value |
|---|---|
| Requirement | Material-change detection MUST classify each changed source field by both field and direction (adverse, favourable, neutral) using a versioned materiality table; only adverse changes to eligibility-relevant fields (e.g. subsection, foundation code, deductibility, status, group exit, revocation, sanctions) may invalidate assurance or hold payments, and informational changes (financial, filing, address within the same entity) MUST NOT create human tasks or block payment. |
| Business rationale | In one monthly BMF cycle 236,750 501(c)(3) organisations changed something but only 405 changed an eligibility-relevant field and about 90 adversely; field-only rules would generate thousands of needless reviews. |
| Refines / applicable existing requirements | RR-UNB-EXT-028-r2; UNB-CHAR-024; CNV-GRT-003 |
| Origin | refinement (parent RR-UNB-EXT-028-r2) |
| Supporting research (daf-landscape package) | 06-continuous-vetting.md |
| Acceptance criteria | Given a monthly BMF delta in which an organisation's revenue and address change, When monitoring runs, Then no review task is created; Given a public charity reclassified to private foundation, When monitoring runs, Then affected assurance is invalidated and pending grants re-evaluated. |
| Dependencies | UNB-CNET-003 |
| Echelon dependencies | — |
| Security implications | Reduces alert fatigue that masks real adverse events. |
| Failure behaviour | Fields absent from the materiality table are treated as potentially material and routed to stewardship until classified. |
| Verification method | Automated (replay of archived monthly deltas with expected task counts) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-008 — Reinstatement semantics

| Field | Value |
|---|---|
| Requirement | A reinstatement shown on the revocation list MUST trigger re-evaluation of held and declined grants for the organisation but MUST NOT by itself release a hold; release requires a subsequent BMF/Pub 78 listing or a determination letter recorded as evidence. Retroactive reinstatement MUST NOT rewrite the recorded history of what was known when the hold was placed. |
| Business rationale | 66% of reinstatements (122,787 of 184,830) are retroactive to the revocation date; reliance attaches to a subsequent listing or determination letter. |
| Refines / applicable existing requirements | UNB-XIN-NPG-005; UNB-CHAR-004 |
| Origin | refinement (parent UNB-XIN-NPG-005) |
| Supporting research (daf-landscape package) | 06-continuous-vetting.md |
| Acceptance criteria | Given a grant held for revocation and a later revocation-list row with a retroactive reinstatement date, When ingestion runs, Then the grant is re-evaluated and remains held until a subsequent listing or letter is recorded, and the original hold record is unchanged. |
| Dependencies | UNB-CNET-001 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | If neither listing nor letter arrives within a sponsor-set period, a sponsor decision task is created. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | May a reinstatement date on the revocation list alone support releasing a held grant (counsel)? |
| Conflicts | — |

### UNB-CNET-009 — Sanctions list polling and full rescreen

| Field | Value |
|---|---|
| Requirement | OFAC SDN and Consolidated lists MUST be polled at least every 6 hours; a changed digest MUST trigger a full reload (not delta-only) and an automatic rescreen of every charity with a pending, scheduled or recurring grant and every active payment destination, before the next payment release. |
| Business rationale | OFAC updates on about 110-130 days a year without a fixed schedule and advises full refresh; release-time checks must reflect same-day additions. |
| Refines / applicable existing requirements | UNB-EXT-004; UNB-RCN-GRT-007; CNV-GRT-006 |
| Origin | refinement (parent UNB-EXT-004) |
| Supporting research (daf-landscape package) | 04-authoritative-charity-data-inventory.md; 06-continuous-vetting.md |
| Acceptance criteria | Given an OFAC update at 10:00 that adds a name matching a scheduled grantee, When the next release window opens, Then the grant is held pending sanctions disposition per CNV-GRT-006. |
| Dependencies | UNB-CNET-003 |
| Echelon dependencies | — |
| Security implications | Screening is a legally mandatory hold; it never auto-releases (CNV-PLT-007 amendment). |
| Failure behaviour | If the list cannot be refreshed within 24 hours releases fail safe. |
| Verification method | Automated |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-010 — Universe-wide monitoring without portfolio inference

| Field | Value |
|---|---|
| Requirement | Public-source monitoring MUST cover the entire ingested public universe rather than per-sponsor watchlists, and identity-resolution requests, searches and watch relationships MUST be stored only in the requesting sponsor's cell; the shared registry MUST NOT be able to infer which sponsors or donors are interested in which charities. |
| Business rationale | Per-sponsor watchlists and request logs in a shared store would reveal grantee portfolios and donor intentions across tenants. |
| Refines / applicable existing requirements | CNV-TEN-001; UNB-CHAR-036; UNB-CHAR-037 |
| Origin | refinement (parent CNV-TEN-001) |
| Supporting research (daf-landscape package) | 05-charity-registry-design.md; 09-charity-network-security.md |
| Acceptance criteria | Given two sponsors granting to the same charity, When the shared store is inspected, Then it contains no record linking either sponsor to that charity. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Closes a cross-tenant inference channel; SharedStoreRegister entries must list it as a negative test. |
| Failure behaviour | If a feature needs sponsor-specific monitoring it is implemented in the sponsor cell. |
| Verification method | Automated (shared-store inference tests) + architecture review |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-011 — Licence-segregated sources

| Field | Value |
|---|---|
| Requirement | Data obtained under share-alike, non-commercial or no-resale terms (e.g. OpenCorporates free tier under ODbL, ProPublica API) MUST carry its licence on every derived claim and MUST NOT enter shared stores, published datasets or exports whose terms conflict; public-domain federal data and compatible open-government licences (UK OGL v3, Canada OGL) are permitted subject to their attribution conditions. |
| Business rationale | Public accessibility does not grant redistribution rights; mixing licences would contaminate any future open dataset. |
| Refines / applicable existing requirements | UNB-EXT-016; UNB-EXT-018; UNB-CHAR-008 |
| Origin | refinement (parent UNB-EXT-016) |
| Supporting research (daf-landscape package) | 04-authoritative-charity-data-inventory.md |
| Acceptance criteria | Given a claim derived from an ODbL source, When a public-layer export is generated, Then the claim is excluded and the export manifest records the exclusion. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Unknown licence = excluded from shared and published outputs. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-012 — Sponsor recipient coverage and payment-readiness metric

| Field | Value |
|---|---|
| Requirement | For each sponsor the platform MUST report, over trailing-12-month grant recipients by count and by dollars: share resolved to BMF, to Pub 78, to neither (with reason class), share with a payment-ready electronic destination by rail class, and check share. The metric MUST be computed from sponsor-cell data and published to the sponsor, not shared. |
| Business rationale | Directory size is not a competitive measure (all incumbents rest on the same IRS data); resolution of real recipients and electronic payment readiness are. Up to about 7% of recipient EINs at Ren-serviced sponsors are not in the BMF (upper bound). |
| Refines / applicable existing requirements | UNB-CHAR-011; CNV-ECO-001 |
| Origin | refinement (parent UNB-CHAR-011) |
| Supporting research (daf-landscape package) | 02-ren-network-decomposition.md; 03-competitor-capability-matrix.md; 21-competitive-performance-targets.md |
| Acceptance criteria | Given a sponsor's migrated 12-month grant history, When the metric runs, Then counts and dollar shares by resolution and payment-readiness class are produced and reconcile to total recipients and dollars. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Computed in-cell; no cross-sponsor aggregation without CNV-TEN-001 controls. |
| Failure behaviour | If recipient resolution is incomplete the unresolved share is reported, never omitted. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-013 — Rail-class taxonomy; pass-through intermediaries are not grant rails

| Field | Value |
|---|---|
| Requirement | Every grant rail MUST be registered with a rail class: PartnerAccountRail (charity holds a receiving account in its own name at the partner's bank), PassThroughIntermediary (an intermediary charity legally receives and regrants), or MoneyMovementSubstrate (no charity network). Only PartnerAccountRail and sponsor-originated ACH/check MAY carry grants to a named grantee; a PassThroughIntermediary MAY be used only when the intermediary itself is the approved grantee under the applicable intermediary rules (CNV-GRT-008). |
| Business rationale | Market 'networked partners' include intermediaries such as PayPal Giving Fund or Benevity foundations whose use changes the legal payee, contrary to CNV-GRT-003's listed-legal-name rule. |
| Refines / applicable existing requirements | CNV-PAY-001; CNV-GRT-003 |
| Origin | refinement (parent CNV-PAY-001) |
| Supporting research (daf-landscape package) | 07-payment-network-landscape.md |
| Acceptance criteria | Given a rail registered as PassThroughIntermediary, When a grant to charity X is routed to it, Then activation refuses the route and records the reason. |
| Dependencies | CNV-PAY-001 |
| Echelon dependencies | — |
| Security implications | Prevents silent payee substitution through intermediaries. |
| Failure behaviour | Unclassified rails cannot be activated. |
| Verification method | Automated (activation conformance check) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-014 — Destination evidence separated from sponsor acceptance

| Field | Value |
|---|---|
| Requirement | Payment destination records MUST separate DestinationEvidence (method, source, verification result, raw vendor response, cost class, timestamps; created once per destination and change) from SponsorDestinationAcceptance (a sponsor's decision to rely on that evidence under its written reliance policy, with scope and expiry). Evidence combinations MUST be those permitted by CNV-PAY-002 as amended; any additional class (e.g. a partner-bank account opened in the charity's name with partner-attested change controls) MUST NOT be accepted unless the owner ratifies it. |
| Business rationale | Partner rails verify differently (Chariot validates officer identity and opens a charity-titled account at Column N.A.) and the c10 amendment permits exactly two combinations; separating evidence from acceptance makes reuse explicit and auditable without loosening the rule. |
| Refines / applicable existing requirements | CNV-PAY-002; UNB-RCN-GRT-017; UNB-CHAR-026 |
| Origin | refinement (parent CNV-PAY-002) |
| Supporting research (daf-landscape package) | 07-payment-network-landscape.md; 09-charity-network-security.md; 22-adversarial-review.md |
| Acceptance criteria | Given destination evidence created for sponsor A, When sponsor B has no reliance policy covering it, Then sponsor B cannot release a payment to the destination and is offered its own verification. |
| Dependencies | UNB-CNET-019 |
| Echelon dependencies | — |
| Security implications | Bank details remain sponsor-scoped (CNV-TEN-001); evidence never shared without a reliance policy. |
| Failure behaviour | Missing acceptance = payment not releasable; check fallback offered. |
| Verification method | Automated |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Should CNV-PAY-002 admit a third evidence class for partner-bank accounts titled to the charity with partner-attested controls (OD-CNO-02)? |
| Conflicts | CNV-EXP-002 (partner enrolment without re-enrolment) vs CNV-PAY-002 c10 'exactly two' evidence combinations for partners such as Chariot (OD-CNO-02). |

### UNB-CNET-015 — Destination risk tiers, first-payment caps and probation

| Field | Value |
|---|---|
| Requirement | Each new or changed destination MUST be assigned a risk tier (D0 unchanged long-verified; D1 new with bank-originated owner match; D2 new with document plus callback; D3 any change following a contact or signatory change, or anomaly signals). Tiers D1-D3 MUST apply a sponsor-set first-payment cap and probation that ends only on charity confirmation of receipt through a verified channel or after a sponsor-set number of clean payments; the CNV-PAY-002 cooling-off and dual-channel notice always apply. |
| Business rationale | Observed losses come from bank-detail substitution after account takeover (e.g. $1m first payment diverted at a foundation in 2023), not fake charities; no single method proves both account ownership and authority to change it. |
| Refines / applicable existing requirements | CNV-PAY-002; UNB-RCN-WF-028; UNB-CHAR-032 |
| Origin | refinement (parent CNV-PAY-002) |
| Supporting research (daf-landscape package) | 09-charity-network-security.md; data/charity-network-threat-model.csv |
| Acceptance criteria | Given a D2 destination and a first grant above the cap, When release is requested, Then only the capped amount is releasable or the grant routes to check per sponsor policy, and probation persists until receipt confirmation. |
| Dependencies | UNB-CNET-014 |
| Echelon dependencies | — |
| Security implications | Bounds loss from a single successful deception; all holds are time-bounded with sponsor escalation (CNV-LEG-001). |
| Failure behaviour | Unconfirmed receipt after the probation window raises a sponsor task; it never blocks indefinitely. |
| Verification method | Automated + simulation of diversion scenarios |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-016 — Imported legacy bank destinations

| Field | Value |
|---|---|
| Requirement | Bank destinations imported from a predecessor platform MUST enter state LegacyImported, MUST NOT be used for electronic release until destination evidence meeting CNV-PAY-002 is recorded (an owner-match recheck is the default low-friction path), and MUST fall back to check (positive pay) meanwhile. The import of an unchanged record MUST NOT itself be treated as a bank-detail change for cooling-off purposes unless the owner decides otherwise. |
| Business rationale | Incumbent enrolments are not portable in practice (AEF's charities reverted to checks until re-verified on Ren); unverified imported payees are the binding constraint on cutover speed. |
| Refines / applicable existing requirements | UNB-CHAR-041; UNB-REM-008; CNV-PAY-002 |
| Origin | refinement (parent UNB-CHAR-041) |
| Supporting research (daf-landscape package) | 02-ren-network-decomposition.md; 15-migration-factory.md; 17-cutover-and-recovery.md; 07-payment-network-landscape.md |
| Acceptance criteria | Given 5,000 imported destinations, When cutover completes, Then none is electronically payable until evidence is recorded, owner-match rechecks run in bulk before cutover, and unverified grantees are paid by check. |
| Dependencies | UNB-CNET-014; UNB-ONB-021 |
| Echelon dependencies | — |
| Security implications | Prevents migration-file tampering from redirecting payments (threat model). |
| Failure behaviour | Recheck vendor outage leaves destinations LegacyImported; check fallback continues. |
| Verification method | Automated + migration rehearsal |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Is importing an unchanged predecessor bank record a 'bank change' that triggers cooling-off (OD-CNO-03)? |
| Conflicts | — |

### UNB-CNET-017 — Ownership-signal port with multiple vendors

| Field | Value |
|---|---|
| Requirement | Bank account ownership verification MUST use a provider-neutral OwnershipSignalPort supporting at least two vendors (e.g. bank-network account-owner authentication, aggregator identity match), retaining each raw response as destination evidence and recording vendor, coverage outcome and cost class. |
| Business rationale | Owner-match services are cheap (single-digit dollars) but coverage of business and nonprofit accounts is unpublished and per-inquirer; a single vendor creates coverage and lock-in risk. |
| Refines / applicable existing requirements | UNB-RCN-GRT-017; CNV-PAY-002 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 07-payment-network-landscape.md; 20-economic-analysis.md |
| Acceptance criteria | Given vendor A returns no-coverage for a credit-union account, When verification runs, Then vendor B is tried and both raw responses are stored. |
| Dependencies | UNB-CNET-014 |
| Echelon dependencies | — |
| Security implications | Raw responses contain personal data of signatories: retained under retention class rules, sponsor-scoped. |
| Failure behaviour | If no vendor covers the account, the document-plus-callback combination is offered. |
| Verification method | Automated (contract tests with vendor sandboxes) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-018 — Partner rail contract minimums

| Field | Value |
|---|---|
| Requirement | A PartnerAccountRail MUST NOT be activated unless the partner agreement provides: idempotent instruction submission and status query; destination owner/admin-change events delivered to the sponsor before the next release; change controls equivalent to CNV-PAY-002 (per the CNV-EXP-002 amendment); disclosure of the deposit bank and insurance ceiling with a sponsor-approved cap on uninsured balances; export of the sponsor's payee and payment records in a documented format at exit; change-of-control notice and termination right; and a written fee schedule. |
| Business rationale | The leading partner (Chariot) serves direct competitors, uses a single bank (Column N.A.) with $250k FDIC cover per depositor, and its public terms do not document change-control, export or change-of-control provisions. |
| Refines / applicable existing requirements | CNV-PLT-012; CNV-EXP-002; UNB-CHAR-042 |
| Origin | refinement (parent CNV-PLT-012) |
| Supporting research (daf-landscape package) | 07-payment-network-landscape.md; 22-adversarial-review.md |
| Acceptance criteria | Given a partner agreement lacking owner-change events, When payment activation is requested for that rail, Then activation is refused with the missing term listed. |
| Dependencies | UNB-CNET-013 |
| Echelon dependencies | — |
| Security implications | Concentration and counterparty risk are bounded contractually; a second rail is required before scale (see UNB-CNET-026). |
| Failure behaviour | Partner insolvency or outage follows CNV-PLT-012 and CNV-PAY-004. |
| Verification method | Contract review checklist + activation conformance test |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-019 — No-portal charity experience for grant receipt

| Field | Value |
|---|---|
| Requirement | A charity MUST be able to receive grants, remittance detail and acknowledgment requests without creating a portal account: remittance by email or file to verified channels, acknowledgment by a single signed link, and enrolment invitations triggered by grants with at most one open invitation per charity, an expiry, and IRS-keyed claim routes (BMF address mail code, e-filed 990 phone, grant-check code). |
| Business rationale | Grant-triggered enrolment is parity among incumbents; what differs is whether the charity must become a software user. Conversion per invitation, not partner pre-enrolment, dominates network growth (time to 67% electronic: 23-65 months at 10% conversion). |
| Refines / applicable existing requirements | UNB-REM-013; UNB-REM-002; UNB-CHAR-014 |
| Origin | refinement (parent UNB-REM-013) |
| Supporting research (daf-landscape package) | 08-organic-network-growth.md; 02-ren-network-decomposition.md; 19-participant-onboarding.md |
| Acceptance criteria | Given a charity with no account receiving its first grant by check, When the check is printed, Then it carries a single-use claim code; When the charity completes enrolment, Then subsequent grants pay electronically with email remittance and no login. |
| Dependencies | UNB-CNET-014; UNB-CNET-015 |
| Echelon dependencies | — |
| Security implications | Invitation links are single-use, expiring and bound to IRS-keyed channels; bank submission still requires CAL3 (CNV-PAY-002 amendment). |
| Failure behaviour | Expired invitations are not reissued in parallel; the grant continues by check. |
| Verification method | Usability test with charities + automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-020 — Charity-scoped payment profile vault

| Field | Value |
|---|---|
| Requirement | Charity-controlled data (authorised representatives, private uploads, preferred rail) MUST be held in a charity-scoped partition outside the shared public registry, and each sponsor's reliance on it MUST require the charity's explicit authorisation recorded per sponsor. |
| Business rationale | Enables one charity authorisation to serve many sponsors without placing bank or personal data in a shared store. |
| Refines / applicable existing requirements | UNB-CHAR-029; CNV-TEN-001; UNB-CHAR-036 |
| Origin | refinement (parent UNB-CHAR-029) |
| Supporting research (daf-landscape package) | 08-organic-network-growth.md; 05-charity-registry-design.md |
| Acceptance criteria | Given a charity that authorised sponsor A only, When sponsor B queries its payment profile, Then access is denied and no metadata about sponsor A is revealed. |
| Dependencies | UNB-CNET-014 |
| Echelon dependencies | — |
| Security implications | Requires a new partition type in the tenancy model; inference tests required. |
| Failure behaviour | Without the partition type ratified, charity data stays sponsor-scoped (current rule). |
| Verification method | Architecture review + automated access tests |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | deferred |
| Open questions | Should CNV-TEN-001's cell model gain a charity-scoped partition (OD-CNO-06)? |
| Conflicts | Charity-scoped partition is not in the CNV-TEN-001 cell model (OD-CNO-06). |

### UNB-CNET-021 — Source integrity gates and mass-change circuit breaker

| Field | Value |
|---|---|
| Requirement | Each ingested release MUST pass integrity gates (expected format, row-count and churn bounds relative to the prior release, TLS-verified retrieval from the publisher host) before promotion; a release whose adverse-change volume exceeds a configured bound (e.g. more than 3x the trailing maximum outside the known August revocation batch) MUST be quarantined and MUST NOT invalidate assurance or create holds until a steward approves it. |
| Business rationale | IRS bulk pages publish no checksums or signatures; a corrupted or spoofed release could otherwise mass-hold or mass-clear charities. |
| Refines / applicable existing requirements | UNB-KNOW-026; UNB-CHAR-033 |
| Origin | refinement (parent UNB-KNOW-026) |
| Supporting research (daf-landscape package) | 09-charity-network-security.md; 06-continuous-vetting.md |
| Acceptance criteria | Given a BMF release in which 20% of 501(c)(3) organisations change subsection, When ingestion runs, Then the release is quarantined and no eligibility changes occur. |
| Dependencies | UNB-CNET-003 |
| Echelon dependencies | — |
| Security implications | Mitigates compromised-feed and data-poisoning threats. |
| Failure behaviour | Quarantine keeps the prior release current, subject to freshness limits. |
| Verification method | Automated (fault-injection tests) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-022 — AI-extracted contact and payment data never auto-applied

| Field | Value |
|---|---|
| Requirement | Contacts, signatories, bank details and roster memberships extracted by AI from charity websites, documents or emails MUST be stored as untrusted proposals and MUST NOT update the registry, a callback contact or a payment destination without deterministic corroboration or human confirmation. |
| Business rationale | Charity websites and uploads are an injection and poisoning channel; contact data feeds the callback control. |
| Refines / applicable existing requirements | CNV-AIO-002; CNV-AIO-001; UNB-CHAR-022 |
| Origin | refinement (parent CNV-AIO-002) |
| Supporting research (daf-landscape package) | 09-charity-network-security.md |
| Acceptance criteria | Given an uploaded letter containing a new bank account and the instruction to use it, When AI processing completes, Then a proposal exists and no destination or contact has changed. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Closes prompt-injection and document-forgery paths into payment data. |
| Failure behaviour | Unconfirmed proposals expire. |
| Verification method | Automated (injection test corpus) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-023 — Nacha False Pretenses monitoring programme

| Field | Value |
|---|---|
| Requirement | Unbound MUST operate, as a Third-Party Service Provider, a documented risk-based monitoring programme for credit entries authorized under False Pretenses (including misrepresented account ownership), reviewed at least annually, with a recall and return playbook coordinated with the sponsor's ODFI and each rail partner. |
| Business rationale | Nacha's Phase 2 fraud monitoring rules (effective 19/22 June 2026) apply to TPSPs and define False Pretenses to include misrepresenting ownership of the account to be credited. |
| Refines / applicable existing requirements | CNV-PAY-001; CNV-PAY-002 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 09-charity-network-security.md; 07-payment-network-landscape.md |
| Acceptance criteria | Given the annual review date, When the compliance calendar runs, Then a review record with monitoring metrics, incidents and changes exists and is attached to the CNV-PAY-001 role table evidence. |
| Dependencies | UNB-CNET-015 |
| Echelon dependencies | — |
| Security implications | Rule obligation; applicability when partners transmit entries requires counsel confirmation. |
| Failure behaviour | Missed review blocks new payment activations. |
| Verification method | Control test + annual review evidence |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | How do Nacha TPSP monitoring obligations allocate between Unbound and a rail partner that transmits the entries (OD-CNO-12)? |
| Conflicts | — |

### UNB-CNET-024 — Publication-ready public registry layer (no Release 1 publication)

| Field | Value |
|---|---|
| Requirement | The shared public layer MUST be designed so that its contents are limited to verbatim public-source facts with source, posting date and licence, separable from all Unbound judgments, so that it could later be published as an open dataset or API; no external publication, licensing or commercial data service MAY occur without an owner decision. |
| Business rationale | The public directory is already commoditised (IRS bulk files, ProPublica, Candid); publication adds little competitive effect, but keeping the layer publication-ready preserves the option at low cost and limits defamation exposure to republishing government records. |
| Refines / applicable existing requirements | UNB-CHAR-042; UNB-CHAR-036 |
| Origin | refinement (parent UNB-CHAR-042) |
| Supporting research (daf-landscape package) | 05-charity-registry-design.md; 23-strategic-recommendations.md |
| Acceptance criteria | Given the shared public layer, When a test export is produced, Then every field traces to a public source release and no sponsor-derived or Unbound-judgment field is present. |
| Dependencies | UNB-CNET-011 |
| Echelon dependencies | — |
| Security implications | Separation also enforces CNV-GRT-004 (no private findings propagate). |
| Failure behaviour | Fields failing provenance are excluded. |
| Verification method | Automated export conformance test |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | deferred |
| Open questions | Should the public layer ever be openly published, and as a separate repository or product (OD-CNO-09)? |
| Conflicts | — |

### UNB-CNET-025 — Charity network cost and effect metering

| Field | Value |
|---|---|
| Requirement | The platform MUST meter, per sponsor and in aggregate: registry operating cost per grant, human review cases and minutes per case by reason class, destination verification method and cost class, invitation-to-enrolment conversion rate, and check share, so that network economics can be validated against the hypotheses in the economic model. |
| Business rationale | On reconciled assumptions (partner coverage about 8%, conversion p=0.10) modelled e-payment savings are about zero in the base case while the registry costs about $316k a year to run ($107k-673k); the network is a cost of doing business and a switching-cost defence whose value must be measured, not assumed. |
| Refines / applicable existing requirements | CNV-ECO-001; CNV-ECO-003 |
| Origin | refinement (parent CNV-ECO-001) |
| Supporting research (daf-landscape package) | 20-economic-analysis.md; 08-organic-network-growth.md; 22-adversarial-review.md |
| Acceptance criteria | Given a month of operation, When the metering report runs, Then each metric is produced with numerator and denominator and conversion is computed per invitation cohort. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Metrics computed in-cell; aggregates follow CNV-TEN-001. |
| Failure behaviour | Missing data is reported as missing. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-CNET-026 — Second electronic rail before scale

| Field | Value |
|---|---|
| Requirement | Before a sponsor's electronic grant volume through a single partner rail exceeds a sponsor-approved share or dollar threshold, a second electronic path (another PartnerAccountRail or sponsor-originated ACH with verified destinations) MUST be activated and tested, so that a partner outage, change of control or termination does not revert the sponsor to checks. |
| Business rationale | Partner concentration risk: the leading rail is venture-funded, serves competitors and depends on one bank. |
| Refines / applicable existing requirements | CNV-PLT-012; CNV-PAY-001 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 07-payment-network-landscape.md; 22-adversarial-review.md |
| Acceptance criteria | Given partner P carries 80% of electronic grants above the threshold, When the activation review runs, Then a second rail must be certified or the sponsor records an accepted risk. |
| Dependencies | UNB-CNET-018; UNB-ONB-025 |
| Echelon dependencies | — |
| Security implications | Resilience control. |
| Failure behaviour | Without a second rail, the sponsor's accepted risk is recorded and reviewed quarterly. |
| Verification method | Activation review evidence |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-001 — Migration class determination

| Field | Value |
|---|---|
| Requirement | Every onboarding MUST be classified at intake as PlatformReplacement (the sponsor of record stays the same and moves its books from another system), ProgramTransfer (funds move from another legal sponsor, e.g. a vendor-affiliated charity, fund by fund under CNV-ENT-002 without history import) or Greenfield, based on a recorded 'legal sponsor of record' finding. CNV-EXP-002 full-history import obligations MUST apply only to PlatformReplacement. |
| Business rationale | Advisor and bank programmes often run on a vendor-affiliated charity (RCF, NPT, Charityvest, GiveClear); moving them is a series of sponsor-approved grants, not a data migration, and UNB-XIN-EVA-009 keeps history with the source sponsor. |
| Refines / applicable existing requirements | CNV-EXP-002; CNV-ENT-002; UNB-XIN-EVA-009 |
| Origin | refinement (parent CNV-EXP-002) |
| Supporting research (daf-landscape package) | 10-institutional-onboarding-landscape.md; 13-pre-contract-migration-assessment.md; 22-adversarial-review.md |
| Acceptance criteria | Given a prospective programme whose donor accounts are legally held by a vendor-affiliated charity, When intake completes, Then the class is ProgramTransfer, the plan uses CNV-ENT-002 transfers and no history-import obligation is created. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Prevents unauthorised extraction of another sponsor's donor records. |
| Failure behaviour | Unknown sponsor of record blocks the assessment from proceeding beyond schema-only profiling. |
| Verification method | Checklist + automated plan validation |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Confirm that CNV-EXP-002 applies to platform replacement only and that 'full history' means all history the predecessor holds (OD-CNO-07). |
| Conflicts | CNV-EXP-002 'full fund history' vs UNB-XIN-EVA-009 (history stays with source sponsor) for inter-sponsor moves (OD-CNO-07). |

### UNB-ONB-002 — Extraction plan per incumbent

| Field | Value |
|---|---|
| Requirement | Before any migration commitment, an Extraction Plan MUST record the sponsor's contractual export rights, export method, format, cost, achievable cadence, post-termination access window and the incumbent's deletion deadline, and MUST select a cadence variant (repeated full extracts, periodic standard reports, or single extract with retrospective parallel run). |
| Business rationale | Published incumbent terms are restrictive (e.g. login-only access, paid export, deletion 10 business days after hosting ends, download bans); there is no DAF equivalent of FINRA Rule 11870 compelling cooperation. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; UNB-TEN-011 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 15-migration-factory.md; 13-pre-contract-migration-assessment.md; 11-cross-industry-migration-research.md; 10-institutional-onboarding-landscape.md |
| Acceptance criteria | Given an incumbent whose terms permit only a single export at termination, When the plan is created, Then the single-extract variant is selected and the schedule captures the archive before the deletion deadline. |
| Dependencies | UNB-ONB-009 |
| Echelon dependencies | — |
| Security implications | Plans record legal basis for receiving each data class. |
| Failure behaviour | A plan without a confirmed export right blocks contract commitments that depend on history import. |
| Verification method | Checklist review + plan validation |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-003 — Pre-contract assessment with exposure tiers

| Field | Value |
|---|---|
| Requirement | A pre-contract migration assessment MUST be available in exposure tiers: T0 questionnaire and sample report layouts (no personal data); T1 schema plus statistical profiles generated on the sponsor's side from standard reports it can download (no row-level personal data leaves the sponsor); T2 row-level data only after an executed NDA/DPA with deletion commitment. Each tier MUST produce a readiness report covering formats, profiled quality, missing and inconsistent data, proposed mappings, unsupported legacy behaviours, remediation estimate and a simulated conversion with reconciliation results. |
| Business rationale | Tokenised samples remain personal data and differential privacy breaks exact tie-outs; sponsor-side profiling is the practical privacy boundary. Requesting a paid incumbent export can signal intent to leave, so T1 must work from standard reports. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; CNV-TEN-001; CNV-PLT-004 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 13-pre-contract-migration-assessment.md |
| Acceptance criteria | Given a prospective sponsor at T1, When it runs the profiler on its standard reports, Then Unbound receives only schema and statistics, and a readiness report with a remediation estimate is produced. |
| Dependencies | UNB-ONB-004; UNB-ONB-005 |
| Echelon dependencies | — |
| Security implications | Data minimisation by design; T2 data is held in an isolated assessment cell and deleted on the committed date. |
| Failure behaviour | If profiles are insufficient for an estimate the report says so and states what T2 data would resolve it. |
| Verification method | Automated profiler tests + privacy review |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-004 — Deterministic onboarding estimate behind a data-quality gate

| Field | Value |
|---|---|
| Requirement | The readiness report MUST derive an onboarding price band and time-to-production estimate deterministically from declared inputs (accounts, history depth available, transaction volume, data-quality profile, number and type of integrations, extraction cadence variant). A fixed price MAY be offered only when all inputs fall within published gate limits; outside them the out-of-gate work MUST be quoted separately and charged up front, and any early-exit recovery MUST be limited to a published straight-line unamortised onboarding balance with no export fee. |
| Business rationale | Data quality swings onboarding cost 2.5-6.2x and full-history import is 46-58% of cost at 100k+ accounts; including onboarding in servicing fees pays back only for added sponsors (about 3.4 months at 10k accounts and 20 bps on the C0 base), not for the first sponsor. |
| Refines / applicable existing requirements | CNV-ECO-002; UNB-SMP-ECN-002; CNV-ECO-003 |
| Origin | refinement (parent CNV-ECO-002) |
| Supporting research (daf-landscape package) | 20-economic-analysis.md; 22-adversarial-review.md; 13-pre-contract-migration-assessment.md |
| Acceptance criteria | Given identical inputs, When the estimate runs twice, Then identical price band and duration are produced; Given a data-quality profile outside the gate, When a quote is produced, Then the out-of-gate component is itemised. |
| Dependencies | UNB-ONB-003 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Missing inputs yield a range with the missing inputs listed, never a point estimate. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | Approve the onboarding commercial model: included in servicing fee inside the gate for added sponsors; separate fee for first sponsor and out-of-gate work (OD-CNO-10). |
| Conflicts | — |

### UNB-ONB-005 — AI mapping proposals are never authoritative

| Field | Value |
|---|---|
| Requirement | Source-to-target mappings proposed by AI MUST carry a state (Proposed, Tested, SponsorConfirmed, Pinned); only Pinned mappings MAY be used in a trial or final migration, and pinning MUST require a passing round-trip reconciliation, attribute-level sample checks and sponsor confirmation. AI MUST NOT execute or alter transformations. |
| Business rationale | AI mapping speeds assessment but total-preserving mapping errors (e.g. swapped fund attributes) pass aggregate reconciliation. |
| Refines / applicable existing requirements | CNV-AIO-001; CNV-AIO-002; RR-REQ-X-061-r1 |
| Origin | refinement (parent CNV-AIO-001) |
| Supporting research (daf-landscape package) | 13-pre-contract-migration-assessment.md; 15-migration-factory.md; 22-adversarial-review.md |
| Acceptance criteria | Given an AI-proposed mapping that swaps two restriction codes but preserves totals, When pinning is attempted, Then the attribute-level sample or invariant check fails and the mapping stays Tested. |
| Dependencies | UNB-ONB-013 |
| Echelon dependencies | — |
| Security implications | Prevents silent semantic corruption. |
| Failure behaviour | Unpinned mappings block trial migration. |
| Verification method | Automated (mutation tests) |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-006 — Canonical migration package with explicit unknowns

| Field | Value |
|---|---|
| Requirement | Legacy data MUST be transformed into a versioned canonical migration package whose fields distinguish Value, Zero, Unknown, NotProvidedBySource and NotApplicable; every record MUST preserve its source system identifier and a hash link to the original source artefact; no transformation may fabricate a value for Unknown or NotProvided. |
| Business rationale | Incomplete legacy history is normal (2 to 26 years observed); fabricated or zero-filled history corrupts statements and tax records. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; UNB-CHAR-008 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 14-canonical-migration-model.md; data/canonical-migration-entities.csv |
| Acceptance criteria | Given a legacy grant lacking a payment date, When transformed, Then the payment date is NotProvidedBySource and the record links to its source row hash. |
| Dependencies | UNB-ONB-007 |
| Echelon dependencies | — |
| Security implications | Source hashes support chain of custody against tampering. |
| Failure behaviour | Validation rejects packages with unmarked nulls. |
| Verification method | Automated schema validation |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-007 — Legacy history partition, opening-balance epoch and roll-forward proof

| Field | Value |
|---|---|
| Requirement | Imported history MUST be stored in a non-posting Legacy History Partition; the live ledger MUST start from an opening-balance epoch posted in the first open period; and each fund MUST have a roll-forward proof tying legacy opening balance plus legacy activity to the epoch balance. Any unexplained difference MUST be recorded as a HistoryGap disclosed to the sponsor and fund holder, never closed with a plug entry. |
| Business rationale | Append-only financial records cannot accept back-dated postings; history must coexist without contaminating live periods. |
| Refines / applicable existing requirements | CNV-EXP-002; CNV-FIN-011; CNV-ARC-004; CNV-FIN-012 |
| Origin | refinement (parent CNV-EXP-002) |
| Supporting research (daf-landscape package) | 14-canonical-migration-model.md; 16-financial-reconciliation.md |
| Acceptance criteria | Given a fund whose legacy activity does not roll to its legacy closing balance by $12.40, When the epoch is posted, Then a HistoryGap of $12.40 is recorded and disclosed and no balancing entry exists. |
| Dependencies | UNB-ONB-006; UNB-ONB-013 |
| Echelon dependencies | — |
| Security implications | Legacy partition is read-only after load; corrections are new versions. |
| Failure behaviour | Unresolved HistoryGaps above the sponsor's tolerance block cutover (UNB-ONB-013). |
| Verification method | Automated + auditor review |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-008 — History depth: all history the predecessor holds, disclosed

| Field | Value |
|---|---|
| Requirement | For PlatformReplacement, the import MUST include all fund history the predecessor holds and the sponsor is entitled to export; any shortfall against full fund life MUST be disclosed per fund with its reason; importing less than is available MUST require the sponsor's written waiver recorded in the plan. |
| Business rationale | 'Full history' is unachievable when the predecessor no longer holds it (AEF's move to Ren carried 33 months, 2 years visible to donors); vendors do not publish history depth. |
| Refines / applicable existing requirements | CNV-EXP-002 |
| Origin | refinement (parent CNV-EXP-002) |
| Supporting research (daf-landscape package) | 14-canonical-migration-model.md; 10-institutional-onboarding-landscape.md; 11-cross-industry-migration-research.md |
| Acceptance criteria | Given a predecessor that holds 7 years of history for funds opened 12 years ago, When migration completes, Then 7 years are imported and each fund shows a disclosed 5-year shortfall with reason. |
| Dependencies | UNB-ONB-001 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Undisclosed shortfall blocks cutover. |
| Verification method | Automated completeness report |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Confirm the reading of CNV-EXP-002 'full fund history' as all history the predecessor holds (OD-CNO-07). |
| Conflicts | — |

### UNB-ONB-009 — Legacy evidence vault

| Field | Value |
|---|---|
| Requirement | Unbound MUST capture and retain, on the sponsor's behalf, the immutable source extracts and supporting documents received in a migration before the incumbent's deletion deadline, with retention clocks running from each source transaction's date under the applicable retention class, and MUST make them exportable with the sponsor's other records. |
| Business rationale | Incumbents delete data shortly after termination; donors and auditors later need source evidence. |
| Refines / applicable existing requirements | CNV-PLT-004; UNB-TEN-016 |
| Origin | refinement (parent CNV-PLT-004) |
| Supporting research (daf-landscape package) | 15-migration-factory.md; 17-cutover-and-recovery.md |
| Acceptance criteria | Given an incumbent deletion deadline 10 business days after termination, When cutover occurs, Then the vault holds hashed extracts captured before termination and a retention schedule for each record class. |
| Dependencies | UNB-ONB-002 |
| Echelon dependencies | — |
| Security implications | Vault is in the sponsor's cell; access is audited. |
| Failure behaviour | Capture failure before deadline escalates to the sponsor immediately. |
| Verification method | Automated + retention control test |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-010 — Deterministic, reproducible migration runs

| Field | Value |
|---|---|
| Requirement | Each migration run MUST be described by a manifest pinning source extract hashes, adapter version, mapping version, configuration package hash and reference data releases; re-running a manifest MUST produce byte-identical canonical outputs and identical reconciliation results. |
| Business rationale | Reproducibility is what makes rehearsals meaningful and auditor reliance possible. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; CNV-GOV-005 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 15-migration-factory.md |
| Acceptance criteria | Given a completed trial run manifest, When it is executed again in a clean rehearsal cell, Then output hashes match exactly. |
| Dependencies | UNB-ONB-011 |
| Echelon dependencies | — |
| Security implications | Manifests are signed and audit-logged. |
| Failure behaviour | A hash mismatch fails the run and blocks promotion. |
| Verification method | Automated |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-011 — Isolated rehearsals and rehearsal gate

| Field | Value |
|---|---|
| Requirement | Trial migrations MUST run in isolated rehearsal cells that cannot write to production financial records or send external communications or payments; cutover authorisation MUST require a sponsor-set minimum of trial runs (default at least three, the last two clean against blocking gates), with the count reduced only when the Extraction Plan's cadence variant makes repeated extracts impossible, in which case transform re-runs on the single extract count. |
| Business rationale | Costly migration failures were governance and testing failures (e.g. TSB testing completed the day before go decision), not data-copy failures. |
| Refines / applicable existing requirements | UNB-TEN-011; RR-REQ-X-061-r1 |
| Origin | refinement (parent UNB-TEN-011) |
| Supporting research (daf-landscape package) | 11-cross-industry-migration-research.md; 15-migration-factory.md |
| Acceptance criteria | Given two clean runs and one failing run as the latest, When cutover authorisation is requested, Then it is refused. |
| Dependencies | UNB-ONB-010 |
| Echelon dependencies | — |
| Security implications | Rehearsal cells use the same isolation profile as production; no outbound effects. |
| Failure behaviour | Gate failure blocks cutover. |
| Verification method | Automated + governance review |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-012 — Closed migration exception classes

| Field | Value |
|---|---|
| Requirement | Migration exceptions MUST be classified into a closed, versioned list (at minimum: missing required field, referential break, value out of domain, duplicate identity, unsupported legacy behaviour, financial roll-forward break, unverifiable payee, policy conflict), each with owner (Unbound or sponsor), blocking flag and resolution evidence. |
| Business rationale | Exception lists with owners and clocks are what make standardised transfers routine (ACATS reject reasons); open-ended exceptions transfer unbounded work to customers. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 15-migration-factory.md; 11-cross-industry-migration-research.md |
| Acceptance criteria | Given an exception that matches no class, When classification runs, Then the run fails until the taxonomy is extended under change control. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Unclassified exceptions block the run. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-013 — Financial reconciliation as the migration acceptance gate

| Field | Value |
|---|---|
| Requirement | Cutover MUST be blocked unless every blocking reconciliation gate passes: per-fund balances, sponsor totals, cash, holdings and units at source precision, custodian positions, general ledger, contributions, grants, grants payable, accrued fees, pending items and restricted assets, plus attribute-level samples and semantic invariants. Exact equality is required where representations permit; unit-pool value rounding MUST use a documented deterministic allocation (largest remainder) and every non-zero difference MUST be classified as rounding, valuation or timing with evidence. Plug or balancing entries without authorised accounting treatment MUST be impossible. |
| Business rationale | Financial correctness is the primary acceptance criterion; silent plugs hide errors that surface in donor statements and audits. |
| Refines / applicable existing requirements | CNV-FIN-004; CNV-FIN-011; UNB-NAV-012; UNB-NAV-031 |
| Origin | refinement (parent CNV-FIN-004) |
| Supporting research (daf-landscape package) | 16-financial-reconciliation.md; data/reconciliation-gates.csv |
| Acceptance criteria | Given a unitised pool whose migrated value differs by $0.01 due to rounding, When gates run, Then the difference is allocated by largest remainder, classified as rounding and the gate passes; Given an unexplained $500 fund difference, Then cutover is blocked. |
| Dependencies | UNB-ONB-007 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Blocking failure prevents cutover; non-blocking differences are disclosed. |
| Verification method | Automated + sponsor and auditor sign-off |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-014 — Accounting convention continuity at cutover

| Field | Value |
|---|---|
| Requirement | The sponsor's legacy grant-recognition and fee-accrual conventions MUST be preserved through cutover until the sponsor's external auditor accepts a change, by default at the next fiscal year-end; a cutoff register of items straddling the epoch MUST be complete and reconciled before cutover. |
| Business rationale | A recognition-point change mid-year creates cutoff errors and audit findings; CNV-FIN-007 makes the recognition point sponsor policy. |
| Refines / applicable existing requirements | CNV-FIN-007; CNV-FIN-011 |
| Origin | refinement (parent CNV-FIN-007) |
| Supporting research (daf-landscape package) | 14-canonical-migration-model.md; 22-adversarial-review.md |
| Acceptance criteria | Given a sponsor recognising grants at approval in the legacy system, When it cuts over mid-year, Then approval-based recognition continues until the configured change date and the cutoff register lists all grants approved but unpaid at epoch. |
| Dependencies | UNB-ONB-013 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Incomplete cutoff register blocks cutover. |
| Verification method | Automated + auditor confirmation |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-015 — Audit evidence for the first sponsor's period

| Field | Value |
|---|---|
| Requirement | Before the first sponsor's go-live, Unbound MUST provide an auditor evidence pack for the migration (manifests, reconciliation results, gate evidence, exception dispositions) and a plan for service-organisation reports (Type I at go-live; Type II period overlapping the sponsor's fiscal year), and MUST disclose the gap to the sponsor's auditor where no Type II report covers the period. |
| Business rationale | The first sponsor's auditor cannot rely on a Type II report that does not yet exist; the evidence pack reduces but does not replace substantive procedures. |
| Refines / applicable existing requirements | CNV-GOV-001; CNV-GOV-002 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 16-financial-reconciliation.md; 17-cutover-and-recovery.md; 22-adversarial-review.md |
| Acceptance criteria | Given go-live on 1 April for a sponsor with a 31 December year-end, When the evidence plan is approved, Then it shows a Type I report at go-live and a Type II period ending no later than the sponsor's year-end or a disclosed gap. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Missing evidence plan blocks go-live approval. |
| Verification method | Governance review |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-016 — Cutover strategy and freeze limits

| Field | Value |
|---|---|
| Requirement | Cutover MUST be per sponsor with a single-writer switch (no fund cohorts within one sponsor on different systems), at an epoch at the start of a month, avoiding December by default; any operational freeze longer than three business days MUST be noticed to affected donors and advisors 30 to 60 days in advance; grant recommendations received during a freeze MUST be accepted and queued, never discarded; parallel operation MUST cover at least one monthly close, or a retrospective parallel run where only a single extract is available. |
| Business rationale | Observed DAF cutovers froze grant processing 4 days to about 5.5 weeks; the ERISA blackout-notice analogue (30-60 days notice for freezes over 3 business days) is a verified precedent. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; UNB-XIN-OTH-002 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 17-cutover-and-recovery.md; 11-cross-industry-migration-research.md; 10-institutional-onboarding-landscape.md; data/cutover-strategy-comparison.csv |
| Acceptance criteria | Given a planned 5-business-day freeze, When the cutover plan is approved, Then donor notices are scheduled at least 30 days ahead and the intake queue for recommendations is enabled for the freeze window. |
| Dependencies | UNB-ONB-017 |
| Echelon dependencies | — |
| Security implications | Queued recommendations are re-authorised at release (CNV-ARC-003). |
| Failure behaviour | Freeze overrun triggers sponsor decision and donor notice. |
| Verification method | Plan validation + rehearsal |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-017 — Irreversibility boundary and effects ledger

| Field | Value |
|---|---|
| Requirement | The migration rollback boundary MUST be the earlier of the first live ledger posting and the first irreversible external effect (payment release, check print, outbound donor or charity notice, custodian instruction); every external effect after the epoch MUST be recorded in an Effects Ledger, and recovery after the boundary MUST use authorised compensating processes rather than deletion. |
| Business rationale | Technical deployment is reversible; financial execution and communications are not. |
| Refines / applicable existing requirements | RR-REQ-X-061-r1; CNV-ARC-003; CNV-PAY-004 |
| Origin | refinement (parent RR-REQ-X-061-r1) |
| Supporting research (daf-landscape package) | 17-cutover-and-recovery.md |
| Acceptance criteria | Given a check printed before the first live posting, When rollback is requested, Then rollback is refused and the compensating process (void with positive-pay update) is offered. |
| Dependencies | UNB-ONB-018 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Unknown effect state is treated as executed (CNV-PAY-004). |
| Verification method | Automated + rehearsal |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Confirm the rollback boundary as the earlier of first live posting and first irreversible external effect (OD-CNO-08). |
| Conflicts | Moves RR-REQ-X-061-r1's rollback boundary earlier (from first live posting) — owner confirmation (OD-CNO-08). |

### UNB-ONB-018 — In-flight item ownership and duplicate prevention

| Field | Value |
|---|---|
| Requirement | Every in-flight item at cutover (approved unpaid grants, issued uncashed checks, pending contributions, recurring schedules, ACH returns window) MUST appear in an ownership register naming the system that may act on it; payment release MUST check, within the release transaction, a cross-system idempotency key derived from the legacy reference; check-number ranges MUST be partitioned between systems; and ACH return routing after any ODFI change MUST be tested before cutover. |
| Business rationale | Duplicate grants and payments are the characteristic migration loss; legacy and new systems both believe they own pending items. |
| Refines / applicable existing requirements | CNV-ARC-003; CNV-PAY-004; CNV-PAY-002 |
| Origin | refinement (parent CNV-ARC-003) |
| Supporting research (daf-landscape package) | 17-cutover-and-recovery.md |
| Acceptance criteria | Given a grant approved in the legacy system and also present in the migration package, When both systems attempt release, Then the new system refuses because the legacy reference key is owned by the legacy system per the register. |
| Dependencies | UNB-ONB-017 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Ownership ambiguity holds the item for sponsor decision. |
| Verification method | Automated + rehearsal |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-019 — Single issuer for the cutover year

| Field | Value |
|---|---|
| Requirement | For the cutover tax year, a single system MUST issue donor annual statements and any corrected acknowledgments, combining imported and live activity; acknowledgments already issued by the predecessor MUST be imported as issued documents and never reissued as originals. |
| Business rationale | IRS Rev. Proc. 2004-53 allows a single successor employer to issue full-year W-2s; the analogue avoids duplicate or split donor tax documents. |
| Refines / applicable existing requirements | RR-REQ-X-060-r1 |
| Origin | refinement (parent RR-REQ-X-060-r1) |
| Supporting research (daf-landscape package) | 11-cross-industry-migration-research.md; 17-cutover-and-recovery.md |
| Acceptance criteria | Given contributions acknowledged by the predecessor in March and the cutover in June, When the annual statement is produced, Then it includes March contributions marked as previously acknowledged and no new acknowledgment is issued for them. |
| Dependencies | UNB-ONB-007 |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Missing predecessor acknowledgment records are disclosed on the statement. |
| Verification method | Automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-020 — Legacy identifier resolution

| Field | Value |
|---|---|
| Requirement | Legacy fund, donor, grant and charity identifiers MUST remain resolvable (search, statements, support, API lookup) for at least 36 months after cutover, extended until 13 consecutive months pass with no lookups. |
| Business rationale | Donors, advisors and charities reference old numbers on checks, statements and correspondence long after cutover. |
| Refines / applicable existing requirements | CNV-EXP-002 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 11-cross-industry-migration-research.md |
| Acceptance criteria | Given a charity calling with a legacy grant number two years after cutover, When support searches it, Then the current grant record is found. |
| Dependencies | UNB-ONB-006 |
| Echelon dependencies | — |
| Security implications | Lookups are access-controlled like the target record. |
| Failure behaviour | None. |
| Verification method | Automated |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-021 — Pre-cutover payee verification campaign

| Field | Value |
|---|---|
| Requirement | A payee verification campaign MUST run before cutover for the sponsor's active recipients (bulk owner-match rechecks, reuse of qualifying partner enrolments, targeted outreach to the top recipients by dollars), and cutover authorisation MUST report the verified share by count and dollars against a sponsor-set minimum. |
| Business rationale | Unverified imported payees are the binding constraint on post-cutover electronic payment; recipients are concentrated (top ~1,000 recipients carried about two thirds of one sponsor's itemised dollars). |
| Refines / applicable existing requirements | CNV-PAY-002; CNV-EXP-002; RR-REQ-X-061-r1 |
| Origin | refinement (parent CNV-PAY-002) |
| Supporting research (daf-landscape package) | 08-organic-network-growth.md; 17-cutover-and-recovery.md; 11-cross-industry-migration-research.md |
| Acceptance criteria | Given a sponsor minimum of 60% of dollars verified, When cutover authorisation is requested at 52%, Then it is refused or the sponsor records an accepted increase in check volume. |
| Dependencies | UNB-CNET-016 |
| Echelon dependencies | — |
| Security implications | Same evidence rules as UNB-CNET-014/015. |
| Failure behaviour | Below-minimum is a sponsor decision, never silent. |
| Verification method | Automated + rehearsal metrics |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-022 — Identity continuity for migrated participants

| Field | Value |
|---|---|
| Requirement | Migrated donors, advisors and delegates MUST be invited to the new platform with risk-based proofing that reuses verified attributes where CNV-PLT-003 permits (marked as prefill), with existing delegations and successor designations carried over as data pending first-login confirmation; bulk-imported contacts MUST receive contact assurance levels reflecting their migration provenance, and the import itself MUST NOT trigger CNV-PLT-011 change holds unless the owner decides otherwise. |
| Business rationale | Every observed DAF cutover forced re-registration or password resets; delegations lost at cutover cause grant failures. |
| Refines / applicable existing requirements | CNV-PLT-003; CNV-PLT-011; UNB-XIN-IAM-001 |
| Origin | refinement (parent CNV-PLT-003) |
| Supporting research (daf-landscape package) | 19-participant-onboarding.md; 10-institutional-onboarding-landscape.md |
| Acceptance criteria | Given an advisor delegation in the legacy system, When the donor first signs in after proofing, Then the delegation is shown for confirmation and is active only after confirmation or after the sponsor's policy period. |
| Dependencies | UNB-ONB-006 |
| Echelon dependencies | — |
| Security implications | Invitation links are single-use and expire; proofing never relies on knowledge-based questions (CNV-PLT-003). |
| Failure behaviour | Unclaimed invitations fall back to mailed one-time codes (CNV-PLT-011 accessible path). |
| Verification method | Usability test + automated |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | Should bulk-imported contacts be exempt from CNV-PLT-011 change holds at import (OD-CNO-11)? |
| Conflicts | — |

### UNB-ONB-023 — Sponsor Configuration Package as the single activation artefact

| Field | Value |
|---|---|
| Requirement | A sponsor MUST be activated only from one versioned, hashed Sponsor Configuration Package covering branding, terminology, legal entities, policies, restrictions, fees, accounting configuration, sponsor-provided investment options, integration references, identity provider and roles, delegated authority, workflows, templates, reporting, retention and declared service targets; activation MUST run schema, semantic and legal-perimeter validation (including funds-flow conformance under CNV-LEG-001 and the CNV-PAY-001 role table), require independent approval, and run shadow replay against the sponsor's migrated history or, for greenfield sponsors, the synthetic operating year. |
| Business rationale | Configuration requirements exist across about 40 IDs without one binding artefact; comparable practice (GitOps, plan/diff, validate-only deploy) lacks legal-perimeter checks, which is Unbound's differentiator. |
| Refines / applicable existing requirements | REQ-X-045; CNV-GOV-005; RR-UNB-TEN-011-r1; UNB-SMP-CFG-001 |
| Origin | refinement (parent REQ-X-045) |
| Supporting research (daf-landscape package) | 12-declarative-sponsor-configuration.md; data/sponsor-config-schema-outline.json |
| Acceptance criteria | Given a package whose payment role table names Unbound as Originator, When validation runs, Then activation is refused citing CNV-PAY-001 and CNV-LEG-001. |
| Dependencies | UNB-ONB-024 |
| Echelon dependencies | No Echelon component provides sponsor-configuration activation; Strata for schema governance of the package store (CNV-ARC-004); Dokimos for acceptance evidence |
| Security implications | Credentials are never in the package, only references (UNB-API-STD-005). |
| Failure behaviour | Any validation failure blocks activation; the prior package stays in force. |
| Verification method | Automated validation suite + approval workflow test |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | initial-implementation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-024 — Closed extension-point catalogue; no sponsor forks

| Field | Value |
|---|---|
| Requirement | Sponsor variation MUST be expressible only through a closed catalogue of extension points (configuration items, certified adapters, template slots, brand tokens, view composition); a release check MUST fail any build containing sponsor-specific code paths or feature flags that vary financial or legal semantics by tenant. |
| Business rationale | A new sponsor must not require a custom fork; the guarantee needs enforcement, not intent. |
| Refines / applicable existing requirements | RR-REQ-X-057-r1; UNB-XIN-SAS-002 |
| Origin | refinement (parent RR-REQ-X-057-r1) |
| Supporting research (daf-landscape package) | 12-declarative-sponsor-configuration.md |
| Acceptance criteria | Given a commit adding a conditional on sponsor id in grant approval logic, When the release check runs, Then the build fails. |
| Dependencies | UNB-ONB-023 |
| Echelon dependencies | — |
| Security implications | Reduces per-tenant attack surface. |
| Failure behaviour | Requests outside the catalogue become product backlog items for all sponsors. |
| Verification method | Automated (static analysis in CI) |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | architectural-support |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-025 — Integration certification lifecycle

| Field | Value |
|---|---|
| Requirement | Each integration (custodian, bank/ODFI, payment partner, identity provider, accounting export, payroll contribution feed, file or API) MUST pass an automated certification suite (schema and contract tests, authentication, idempotency, failure simulation, replay, reconciliation) before a scoped, expiring certificate is issued, followed by a shadow-live phase with read-only or non-executing access before write or execution access is enabled; certificates MUST be re-run on counterparty or adapter version change. |
| Business rationale | No US custodian or FDX self-service certification is public; the certification runner is Unbound build scope because Echelon components provide almost none of it. |
| Refines / applicable existing requirements | UNB-XIN-SAS-006; UNB-CUST-024; UNB-PAY-007; CNV-ECO-005 |
| Origin | refinement (parent UNB-XIN-SAS-006) |
| Supporting research (daf-landscape package) | 18-integration-onboarding.md; data/integration-certification-suite.csv |
| Acceptance criteria | Given an adapter whose duplicate-submission test fails, When certification runs, Then no certificate is issued and the integration register blocks readiness. |
| Dependencies | UNB-ONB-026 |
| Echelon dependencies | Unbound-owned provider contracts and mock interactions (CNV-ARC-004) until Strata STRATA-DIV-001 virtualization exists (proposed, not implemented); Vigila untagged pilot - no Release 1 dependency |
| Security implications | Certification includes negative authentication and replay tests. |
| Failure behaviour | Expired certificates suspend execution access, not reads. |
| Verification method | Automated certification suite |
| Priority | P0 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-026 — Self-service counterparty readiness assessment

| Field | Value |
|---|---|
| Requirement | Counterparties (custodians, banks, partners, sponsor IT) MUST be able to run the certification suite's non-production tests themselves against a sandbox with synthetic data and receive a readiness report, including for sponsor-originated ACH a per-ODFI profile recording file formats, cut-offs, return handling and the ODFI's fraud-monitoring evidence. |
| Business rationale | Integration readiness is a recurring source of delay that can be moved earlier and off the critical path. |
| Refines / applicable existing requirements | CNV-ECO-005; CNV-PAY-001 |
| Origin | refinement (parent CNV-ECO-005) |
| Supporting research (daf-landscape package) | 18-integration-onboarding.md |
| Acceptance criteria | Given a custodian running the file-feed tests in the sandbox, When all pass, Then a readiness report is generated and linked to the integration register entry. |
| Dependencies | UNB-ONB-025 |
| Echelon dependencies | — |
| Security implications | Sandbox holds synthetic data only. |
| Failure behaviour | None. |
| Verification method | Automated |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-027 — Federated identity assertion reliance record

| Field | Value |
|---|---|
| Requirement | Identity assertions from a sponsor's or advisory firm's identity provider MUST be recorded as evidence with issuer, assurance claims and trust agreement reference, and MUST NOT substitute for CNV-PLT-003 proofing unless counsel and the owner approve a reliance arrangement for that issuer. |
| Business rationale | Federation removes passwords and repeat data entry, but CNV-PLT-003 states prefill never substitutes for proofing. |
| Refines / applicable existing requirements | CNV-PLT-003; UNB-XIN-IAM-001 |
| Origin | refinement (parent CNV-PLT-003) |
| Supporting research (daf-landscape package) | 19-participant-onboarding.md |
| Acceptance criteria | Given an advisor signing in via the firm's SAML provider, When the session starts, Then the assertion is recorded and the proofing state is unchanged unless an approved reliance arrangement exists. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Prevents assertion-based privilege escalation. |
| Failure behaviour | Without approved reliance, standard proofing applies. |
| Verification method | Automated |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | May a federated identity provider's proofing satisfy CNV-PLT-003 under a trust agreement (counsel, OD-CNO-11)? |
| Conflicts | — |

### UNB-ONB-028 — Firm-managed advisor provisioning

| Field | Value |
|---|---|
| Requirement | Advisory firms MUST be able to provision and deprovision advisor access through SCIM (or an equivalent automated directory feed) under a firm-level delegation recorded by the sponsor, with deprovisioning effective before the next session. |
| Business rationale | Advisor-distributed sponsors are an entry segment; manual advisor onboarding and stale access are both friction and risk. |
| Refines / applicable existing requirements | UNB-WF-012; UNB-XIN-CRM-007 |
| Origin | refinement (parent UNB-WF-012) |
| Supporting research (daf-landscape package) | 19-participant-onboarding.md |
| Acceptance criteria | Given an advisor removed from the firm directory, When the next SCIM sync runs, Then the advisor's sessions are revoked and delegated actions blocked. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | Timely deprovisioning control. |
| Failure behaviour | Sync failure alerts the firm and sponsor; access is not extended. |
| Verification method | Automated |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | sponsor-activation |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-029 — Unbound exit terms

| Field | Value |
|---|---|
| Requirement | A sponsor MUST be able to export, without Unbound assistance or fee, all of its records (including system-generated records, audit evidence, legacy vault contents and configuration packages) in the documented canonical package format, and MUST retain read access for at least 90 days after termination; export MUST never be conditioned on payment of disputed amounts. |
| Business rationale | Unbound cannot compel incumbents to cooperate, but it can offer the exit terms it asks the market to accept; restrictive incumbent exit terms are a documented switching barrier. |
| Refines / applicable existing requirements | UNB-TEN-016; UNB-GAP-025; CNV-ENT-002 |
| Origin | refinement (parent UNB-TEN-016) |
| Supporting research (daf-landscape package) | 10-institutional-onboarding-landscape.md; 11-cross-industry-migration-research.md; 20-economic-analysis.md |
| Acceptance criteria | Given a terminated sponsor, When it runs the export on day 60, Then the full package is produced and validates against the published schema. |
| Dependencies | UNB-ONB-006 |
| Echelon dependencies | — |
| Security implications | Export is authenticated at privileged assurance and audit-logged. |
| Failure behaviour | None. |
| Verification method | Automated export round-trip test |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-030 — Migration Service Commitment with published outcome metrics

| Field | Value |
|---|---|
| Requirement | Unbound MUST measure and report for every migration: history depth imported versus available, reconciliation result (gates passed, disclosed gaps), freeze length, carry-over of logins and charity payment enrolments, verified payee share at cutover, and delivery against the committed date counted from receipt of a usable extract; any public commitment MUST cover only Unbound-controlled steps and exclude incumbent delays, late sponsor approvals and ProgramTransfer moves. |
| Business rationale | Competitors already market migration; none publishes these outcomes, which are the only defensible basis for an onboarding advantage. DAFs lack ACATS/CASS-style compulsion, so guarantees cannot extend to incumbent behaviour. |
| Refines / applicable existing requirements | UNB-XIN-OTH-002; CNV-EXP-002 |
| Origin | refinement (parent UNB-XIN-OTH-002) |
| Supporting research (daf-landscape package) | 11-cross-industry-migration-research.md; 10-institutional-onboarding-landscape.md; 21-competitive-performance-targets.md |
| Acceptance criteria | Given a completed migration, When the outcome report is produced, Then each metric is present with its evidence link and the clock start is the recorded usable-extract date. |
| Dependencies | UNB-ONB-002 |
| Echelon dependencies | — |
| Security implications | Report excludes personal data. |
| Failure behaviour | Missing metric = commitment not met. |
| Verification method | Automated report + governance review |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-031 — Migration capacity limit

| Field | Value |
|---|---|
| Requirement | The number of concurrent migrations and cutovers MUST be limited to declared capacity (staff hours, rehearsal cells, reconciliation reviewers), and no cutover MAY be scheduled in a window where declared capacity is exceeded or in the December peak without owner approval. |
| Business rationale | Schedule pressure is the leading cause of costly migrations; capacity is a governance control, not a sales choice. |
| Refines / applicable existing requirements | UNB-SMP-ECN-002; CNV-SVC-002 |
| Origin | refinement (parent UNB-SMP-ECN-002) |
| Supporting research (daf-landscape package) | 11-cross-industry-migration-research.md; 15-migration-factory.md |
| Acceptance criteria | Given two cutovers already scheduled in March at declared capacity, When a third is requested for March, Then scheduling is refused. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Capacity overrun escalates to the owner. |
| Verification method | Automated scheduling rule |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-032 — First-sponsor and added-sponsor economics

| Field | Value |
|---|---|
| Requirement | Economic validation of onboarding and charity-network capabilities MUST report separately (a) the first-sponsor view carrying organisation fixed costs and build amortisation and (b) the added-sponsor marginal view, each on the no-lever C0 base with low/base/high (or P10-P90) ranges and the buyer's total switching cost, before any onboarding price or inclusion-in-fee commitment is approved. |
| Business rationale | On the C0 base an added 10k-account sponsor at 20 bps pays back onboarding in about 3.4 months, but a first sponsor carrying about $3.0M a year of platform fixed cost does not pay back at P50; buyer switching cost ($170k-$846k base) is borne by sponsors. |
| Refines / applicable existing requirements | CNV-ECO-003; CNV-ECO-002 |
| Origin | refinement (parent CNV-ECO-003) |
| Supporting research (daf-landscape package) | 20-economic-analysis.md; 22-adversarial-review.md |
| Acceptance criteria | Given an onboarding price proposal, When it is submitted for approval, Then both views and the buyer switching cost are attached or approval is refused. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Missing view blocks commercial approval. |
| Verification method | Governance review |
| Priority | P1 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |

### UNB-ONB-033 — Remaining competitive barrier register

| Field | Value |
|---|---|
| Requirement | Unbound MUST maintain a register of competitive barriers that remain after its planned capabilities (e.g. incumbent distribution and revenue-sharing arrangements, investment-platform integration, accumulated payee enrolments, audit-accepted history, brand), each with evidence, owner, planned response and review date, reviewed before each segment activation decision. |
| Business rationale | The adversarial review found the binding barrier may be distribution and incentive alignment rather than charity networks or onboarding; that hypothesis needs an owner and evidence. |
| Refines / applicable existing requirements | CNV-ECO-004; CNV-ECO-003 |
| Origin | gap-analysis (new requirement) |
| Supporting research (daf-landscape package) | 22-adversarial-review.md; 23-strategic-recommendations.md |
| Acceptance criteria | Given a segment activation decision, When it is prepared, Then the register's current entries and their evidence are attached. |
| Dependencies | — |
| Echelon dependencies | — |
| Security implications | None. |
| Failure behaviour | Stale register (past review date) blocks the activation decision. |
| Verification method | Governance review |
| Priority | P2 |
| Status | proposed · not implemented · not validated |
| Release scope | commercial-availability |
| Open questions | — |
| Conflicts | — |
<!-- END GENERATED -->

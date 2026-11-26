# 03 — Cross-repository implications XRI-001..XRI-008

Package: [README](README.md) · Source data: [`data/implications.jsonl`](data/implications.jsonl).

The eight implications the Phase II research routed to Unbound, processed one by one. Each record carries the research register row **verbatim** (`research_row`, checked against `research/competitive-displacement/data/registers/cross-repository-implications.csv` by gate A8), the affected existing records, whether they already address it, conflicts, duplication and gaps, the requirements created or refined, one disposition class, Given/When/Then acceptance and validation requirements.

Disposition classes: Already satisfied · Requirements updated · New requirement added · Existing decision superseded · Research-only · Deferred behind an explicit gate · Blocked by unavailable information · Requires a new owner decision.

The owner authorised progression of XRI-001..XRI-018 on 2026-11-26 subject to their documented conditions. That authorises requirements refinement, research and planning; it does not authorise implementation, expenditure, contracts or the removal of mandatory controls.

<!-- BEGIN GENERATED -->
## Summary

| XRI | Area | Disposition | Created | Refined | Owner decisions |
|---|---|---|---|---|---|
| XRI-001 | Commercial entry segment (CNV-ECO-004, ratified by OD-M-05; SPD-CMP-001) | Requires a new owner decision | UNB-CDD-CMF-001; UNB-CDD-CMF-003 | UNB-ONB-023; CNV-TEN-001; CNV-LEG-001 | OD-CDD-01; OD-CDD-05 |
| XRI-002 | Charity knowledge: evidence classes and promotion rules (UNB-KNOW, UNB-CHAR, CNV-GRT-003) | New requirement added | UNB-CDD-CIV-001; UNB-CDD-CIV-002; UNB-CDD-CIV-003; UNB-CDD-CIV-010; UNB-CDD-CIV-011; UNB-CDD-CIV-012; UNB-CDD-CIV-013 | UNB-CHAR-018; UNB-CHAR-037; CNV-AIO-002; CNV-GRT-004; UNB-CHAR-005; UNB-CHAR-013; UNB-CHAR-025 | — |
| XRI-003 | Charity re-vetting policy (CNV-GRT-003; RR-REQ-X-007-r1/-r2; RR-UNB-EXT-028-r2) | Requirements updated | UNB-CDD-CIV-006; UNB-CDD-CIV-007; UNB-CDD-CIV-008; UNB-CDD-CIV-009 | RR-UNB-EXT-028-r2; UNB-CHAR-024; RR-REQ-X-007-r2; UNB-XIN-CLM-005 | OD-CDD-02 |
| XRI-004 | Charity source register and shared-layer rights (UNB-EXT, UNB-CNET-011) | New requirement added | UNB-CDD-CIV-004; UNB-CDD-CIV-005 | UNB-EXT-004; UNB-CNET-011; UNB-EXT-016 | — |
| XRI-005 | Charity knowledge architecture: archive of every IRS and sanctions release | Already satisfied | — | — | — |
| XRI-006 | Operational event instrumentation (CNV-ECO-001, CNV-ECO-003) | New requirement added | UNB-CDD-OEM-001; UNB-CDD-OEM-002; UNB-CDD-OEM-003; UNB-CDD-OEM-004 | CNV-ECO-001; CNV-ECO-003; UNB-ING-OPS-002; UNB-CNET-025 | — |
| XRI-007 | Prevention mechanisms (contributions, recommendations, destinations, policy rules) | Requirements updated | UNB-CDD-OEM-005; UNB-CDD-OEM-006 | UNB-ING-DAF-001; CNV-GRT-003 | — |
| XRI-008 | Operating modes: technology-only licensing; TPSP; SOC 1 Type II | New requirement added | UNB-CDD-CMF-001; UNB-CDD-CMF-002; UNB-CDD-CMF-004; UNB-CDD-CMF-005; UNB-CDD-CMF-006 | CNV-TEN-001; CNV-PAY-001; CNV-GOV-001; UNB-ONB-023 | — |

### XRI-001 — Requires a new owner decision

| Field | Value |
|---|---|
| Research proposal (verbatim) | Raise an owner decision to re-test the ratified entry segment against switching evidence (CLM-015 CLM-016 CLM-027) |
| Research target area | requirements/convergence OD-M-05 / SPD-CMP-001 |
| Research type / status / blocking | open-question / proposed (not approved) / No |
| Affected area | Commercial entry segment (CNV-ECO-004, ratified by OD-M-05; SPD-CMP-001) |
| Research evidence | 17-unbound-implications.md; 07-institutional-switching-incentives.md; 16-commercial-entry-strategy.md; 23-executive-decision-register.md |
| Claim and source ids | CLM-015; CLM-016; CLM-027; SRC-011; SRC-065; DEC-001 (register: SRC-011;SRC-065) |
| Affected existing requirements, clauses, decisions | CNV-ECO-004; CNV-ECO-003; OD-M-05; OD-M-08 |
| Already addressed? | Partly. The owner approved DEC-001 (partner-first entry, OD-M-05 kept as a long-term destination), which answers the research's request to re-test the segment in direction; the ratified CNV-ECO-004 text still names the segment as the initial target. |
| Addressed by | OD-M-05 |
| Conflicts | DEC-001 conflicts with the entry-route wording of CNV-ECO-004 (initial target segment). CNV-ECO-004's 25% concentration rule is silent on revenue arriving through one channel partner. |
| Duplication | None: no Unbound record yet describes partner-embedded delivery. |
| Gaps | No operating mode for a module inside a channel partner's offering; no owner reading of concentration under a partner channel. |
| Requirements created | UNB-CDD-CMF-001; UNB-CDD-CMF-003 |
| Existing requirements refined | UNB-ONB-023; CNV-TEN-001; CNV-LEG-001 |
| Disposition | Requires a new owner decision |
| Why | Amending the entry route in a ratified clause needs the owner; the supporting operating modes are added only as architectural support so nothing depends on the amendment. |
| Owner decisions raised | OD-CDD-01; OD-CDD-05 |
| Approved decisions | DEC-001 |
| Acceptance criteria | (1) Given OD-CDD-01 is open, When the baseline is regenerated, Then CNV-ECO-004 text is unchanged and OD-CDD-01 appears as an open decision in requirements/OPEN-QUESTIONS.md. (2) Given a partner-embedded deployment configured under UNB-CDD-CMF-003, When activation runs, Then it passes the CNV-LEG-001 funds-flow conformance check or is blocked. |
| Validation requirements | (1) Validator gate A6: XRI-001 carries an owner decision that exists and is open. (2) V-1 and V-6 results recorded before OD-CDD-01 is answered. (3) render-questions registers OD-CDD-01 and OD-CDD-05. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-002 — New requirement added

| Field | Value |
|---|---|
| Research proposal (verbatim) | Propose evidence classes: authoritative fact / third-party assertion / charity-provided / sponsor decision / shared vetting evidence / historical observation / inferred relationship / unresolved conflict / AI assessment with promotion rules |
| Research target area | requirements/charities (UNB-KNOW; CNV-GRT-003) |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / No |
| Affected area | Charity knowledge: evidence classes and promotion rules (UNB-KNOW, UNB-CHAR, CNV-GRT-003) |
| Research evidence | 04-charity-intelligence-architecture.md; 17-unbound-implications.md; 20-adversarial-review.md |
| Claim and source ids | SRC-008; SRC-063; CHL-044; CHL-062; CHL-063; DEC-005 (register: SRC-008;SRC-063) |
| Affected existing requirements, clauses, decisions | UNB-CHAR-012; UNB-CHAR-018; UNB-CHAR-022; UNB-CHAR-036; UNB-CHAR-037; UNB-KNOW-002; CNV-AIO-001; CNV-AIO-002; CNV-GRT-004; UNB-CNET-002; UNB-CNET-005; UNB-CNET-022 |
| Already addressed? | Partly. Claims vs facts, per-predicate authority (UNB-CNET-002), conflict states (UNB-CNET-005), AI proposals never applied (UNB-CNET-022) and AI never altering findings (CNV-AIO-002) exist; there is no closed evidence-class catalogue and no general rule that authority cannot arise by accumulation. |
| Addressed by | UNB-CHAR-012; UNB-CNET-002; UNB-CNET-005; UNB-CNET-022; CNV-AIO-002 |
| Conflicts | None with ratified text: CNV-GRT-004 and OD-P-GRT cut F already keep dispositions sponsor-scoped, which the research's revision 2 also requires. |
| Duplication | The research's 'shared vetting evidence ... screening result at time t' overlaps sponsor-scoped dispositions (CHL-063); the new catalogue keeps only public facts shareable to avoid duplicating CNV-GRT-004 with a weaker rule. |
| Gaps | Closed class catalogue (ten classes including licensed third-party verification), promotion rules, AI assessment records, identity candidates, typed review queues, auditable histories. |
| Requirements created | UNB-CDD-CIV-001; UNB-CDD-CIV-002; UNB-CDD-CIV-003; UNB-CDD-CIV-010; UNB-CDD-CIV-011; UNB-CDD-CIV-012; UNB-CDD-CIV-013 |
| Existing requirements refined | UNB-CHAR-018; UNB-CHAR-037; CNV-AIO-002; CNV-GRT-004; UNB-CHAR-005; UNB-CHAR-013; UNB-CHAR-025 |
| Disposition | New requirement added |
| Why | The gap is a missing catalogue and promotion rule; it refines existing records without changing ratified text. |
| Owner decisions raised | — |
| Approved decisions | DEC-005; DEC-004 |
| Acceptance criteria | (1) Given an AI-generated assessment and an inferred successor link, When eligibility is evaluated, Then neither is an input and the evaluation lists the evidence class of every input. (2) Given a link confirmed with sponsor-confidential evidence, When another sponsor queries the charity, Then no confirmation is visible. |
| Validation requirements | (1) Validator gates A2-A4 on UNB-CDD-CIV-001..003, -010..013. (2) Negative isolation tests named in each requirement's verification method (future implementation). |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-003 — Requirements updated

| Field | Value |
|---|---|
| Research proposal (verbatim) | The 180-day full re-review is a policy choice (not law); propose change-driven re-vetting with evidence expiry and sample audit as the policy |
| Research target area | requirements/charities |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / Counsel/auditor view |
| Affected area | Charity re-vetting policy (CNV-GRT-003; RR-REQ-X-007-r1/-r2; RR-UNB-EXT-028-r2) |
| Research evidence | 17-unbound-implications.md; 04-charity-intelligence-architecture.md; 05-charity-registry-economics.md; 22-commercial-validation-plan.md |
| Claim and source ids | CLM-005; CHL-035; HYP-005; HYP-006; OQ-004; SRC-001; SRC-063; DEC-003 (register: SRC-001;SRC-063) |
| Affected existing requirements, clauses, decisions | CNV-GRT-003; RR-REQ-X-007-r1; RR-REQ-X-007-r2; RR-UNB-EXT-028-r2; UNB-XIN-CLM-005; UNB-CNET-004; UNB-CNET-007; OD-CNO-04 |
| Already addressed? | Partly. Event-triggered invalidation (CNV-GRT-003), materiality-classified monitoring (RR-UNB-EXT-028-r2, UNB-CNET-007), automatic renewal when evidence is current and a stratified after-the-fact sample (RR-REQ-X-007-r2, proposed) exist. |
| Addressed by | CNV-GRT-003; RR-UNB-EXT-028-r2; RR-REQ-X-007-r2; UNB-CNET-007 |
| Conflicts | The research proposes replacing the 180-day full re-review; the owner's approval of DEC-003 preserves the 180-day validity, so replacement is not adopted. Whether a change-driven renewal satisfies CNV-GRT-003's 'deep review' and RR-REQ-X-007-r1's 'comprehensive charity review' is unresolved. |
| Duplication | RR-REQ-X-007-r2 items 1 and 5 already propose automatic renewal and a stratified sample; this package does not restate them and cites them instead. |
| Gaps | No monitoring coverage register; no evidence-type expiry capped by the interval; no statement that targeted re-vetting leaves the deep-review clock unchanged as a general rule; sample audit not framed as an investigation with V-3/V-9. |
| Requirements created | UNB-CDD-CIV-006; UNB-CDD-CIV-007; UNB-CDD-CIV-008; UNB-CDD-CIV-009 |
| Existing requirements refined | RR-UNB-EXT-028-r2; UNB-CHAR-024; RR-REQ-X-007-r2; UNB-XIN-CLM-005 |
| Disposition | Requirements updated |
| Why | Change monitoring, expiry and targeted re-vetting are refined inside the preserved 180-day ceiling; the one point that would need ratified wording to move is escalated as OD-CDD-02 and gated on V-3 and V-9. |
| Owner decisions raised | OD-CDD-02 |
| Approved decisions | DEC-003 |
| Acceptance criteria | (1) Given a sponsor policy that lets eligibility evidence outlive 180 days, When it is validated, Then activation is refused. (2) Given a revocation published for a charity verified 60 days earlier, When it is ingested, Then only dependent grants are re-evaluated and the deep-review due date is unchanged. (3) Given OD-CDD-02 is open, When a deep review falls due, Then it is performed as RR-REQ-X-007-r1 specifies. |
| Validation requirements | (1) V-3 public-data replay via the existing replay tool. (2) V-9 auditor walkthrough. (3) Counsel review (XRI-003 blocking condition: counsel/auditor view). (4) Validator gate A5: DEC-003 stays 'Strategically approved'. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-004 — New requirement added

| Field | Value |
|---|---|
| Research proposal (verbatim) | Add trade.gov CSL, UK Sanctions List (replacing OFSI), Federal Audit Clearinghouse and SAM exclusions as candidate sources; exclude non-redistributable sources from shared layer |
| Research target area | requirements/charities source list |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / No |
| Affected area | Charity source register and shared-layer rights (UNB-EXT, UNB-CNET-011) |
| Research evidence | 17-unbound-implications.md; 02-charity-intelligence-sources.md; 03-charity-data-licensing-and-access.md |
| Claim and source ids | CLM-006; CLM-007; CLM-009; SRC-064; OQ-007; OQ-008; DEC-004 (register: SRC-064) |
| Affected existing requirements, clauses, decisions | UNB-EXT-003; UNB-EXT-004; UNB-EXT-016; RR-UNB-EXT-004-r1; UNB-CNET-003; UNB-CNET-009; UNB-CNET-011; CNV-GRT-004; OD-CNO-09 |
| Already addressed? | Partly. OFAC and SAM appear generally (UNB-EXT-003/-004); licence segregation exists for share-alike and non-commercial data (UNB-CNET-011). CSL, the FCDO UK Sanctions List and FAC are not named; OFSI's closure is not recorded; ProPublica, GivingTuesday, OpenSanctions, EMMA and BBB are not named for exclusion. |
| Addressed by | UNB-EXT-004; UNB-CNET-011 |
| Conflicts | Adding lists to legally applicable screening is counsel's decision (RR-UNB-EXT-004-r1 ScreeningScope); the candidates therefore stay advisory until counsel acts. OD-CNO-09 (open publication of the public layer) is constrained by DEC-004 but not answered by it. |
| Duplication | UNB-CNET-011 already bars share-alike/non-commercial data from shared stores; UNB-CDD-CIV-005 refines it with egress-time enforcement and named exclusions rather than duplicating it. |
| Gaps | Candidate source rows; per-source rights records covering AI processing; named exclusions; per-state rights rows. |
| Requirements created | UNB-CDD-CIV-004; UNB-CDD-CIV-005 |
| Existing requirements refined | UNB-EXT-004; UNB-CNET-011; UNB-EXT-016 |
| Disposition | New requirement added |
| Why | New source and rights requirements are needed; legal applicability is routed to counsel as a requirement question. |
| Owner decisions raised | — |
| Approved decisions | DEC-004 |
| Acceptance criteria | (1) Given a CSL match before counsel adds CSL to ScreeningScope, When screening runs, Then a review item opens and no adverse determination is recorded. (2) Given a ProPublica-derived fact, When a shared-layer write or cross-sponsor API response is attempted, Then it is refused or omitted. |
| Validation requirements | (1) Counsel confirmation of licence readings (OQ-007, OQ-008) before any source is classed redistributable. (2) Validator gate A3: every cited source requirement resolves. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-005 — Already satisfied

| Field | Value |
|---|---|
| Research proposal (verbatim) | Archive every monthly IRS release from day one (temporal archive is a time-based asset) |
| Research target area | architecture/CHARITY-KNOWLEDGE-MODEL-AND-ACQUISITION-PIPELINE.md |
| Research type / status / blocking | architecture-note / proposed (not approved) / No |
| Affected area | Charity knowledge architecture: archive of every IRS and sanctions release |
| Research evidence | 17-unbound-implications.md; 04-charity-intelligence-architecture.md; 13-compounding-competitive-advantages.md |
| Claim and source ids | CLM-010; SRC-001; SRC-064; CHL-063 (register: SRC-001;SRC-064) |
| Affected existing requirements, clauses, decisions | UNB-CNET-003; UNB-CNET-009; UNB-TRN-002; UNB-CHAR-008; UNB-KNOW-026 |
| Already addressed? | Yes, at the requirements level. UNB-CNET-003 archives every BMF, Pub 78, revocation, 990-N and OFAC release immutably with Last-Modified and SHA-256; UNB-TRN-002 records RawEvidence for every acquired object; UNB-CHAR-008 preserves the original payload. None is implemented. |
| Addressed by | UNB-CNET-003; UNB-TRN-002; UNB-CHAR-008 |
| Conflicts | None. The research itself corrected the competitive claim (CLM-010 contradicted: NCCS already archives the BMF); the archive is hygiene and provenance, not an advantage. |
| Duplication | The proposal duplicates UNB-CNET-003; no new requirement is added. Candidate lists in UNB-CDD-CIV-004 inherit the same archive rule. |
| Gaps | None. The architecture note target (architecture/CHARITY-KNOWLEDGE-MODEL-AND-ACQUISITION-PIPELINE.md) is not edited: architecture is fixed and the requirement-level rule suffices. |
| Requirements created | — |
| Existing requirements refined | — |
| Disposition | Already satisfied |
| Why | Existing proposed requirements already require an immutable per-release archive from day one. |
| Owner decisions raised | — |
| Approved decisions | DEC-004 |
| Acceptance criteria | (1) Given two monthly BMF releases, When both are ingested, Then both are retrievable by Last-Modified and hash and a decision made under the first replays against it (UNB-CNET-003, UNB-CDD-CIV-013). |
| Validation requirements | (1) Validator gate A6: an 'Already satisfied' disposition creates no requirement and names the satisfying records. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-006 — New requirement added

| Field | Value |
|---|---|
| Research proposal (verbatim) | Instrument every operational event with touch/no-touch minutes and failure lifecycle so modelled rates can be replaced by measured ones |
| Research target area | requirements (event instrumentation) |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / No |
| Affected area | Operational event instrumentation (CNV-ECO-001, CNV-ECO-003) |
| Research evidence | 17-unbound-implications.md; 09-event-level-operating-model.md; 10-zero-touch-administration-economics.md |
| Claim and source ids | CLM-011; CLM-012; CLM-037; HYP-006; HYP-008; SRC-063 (register: SRC-063) |
| Affected existing requirements, clauses, decisions | CNV-ECO-001; CNV-ECO-003; CNV-SVC-002; UNB-CNET-025; UNB-ING-OPS-002; RR-REQ-X-007-r2; UNB-ONB-030 |
| Already addressed? | Partly. Causal cost metering (CNV-ECO-001), a 90-day time study at the first sponsor (CNV-ECO-003), charity-network metering (UNB-CNET-025) and ingestion STP and touches (UNB-ING-OPS-002) exist; there is no platform-wide event record with touch, minutes and failure lifecycle, and no rule that measured values replace modelled ones. |
| Addressed by | CNV-ECO-001; CNV-ECO-003; UNB-CNET-025; UNB-ING-OPS-002 |
| Conflicts | None with ratified text; CNV-ECO-003's restatement rule is the mechanism the new requirements feed. |
| Duplication | UNB-CNET-025 and UNB-ING-OPS-002 meter subsets; UNB-CDD-OEM-001 generalises them so each subset metric becomes a view over one event record rather than a separate meter. |
| Gaps | Event catalogue and records; versioned metric definitions with minimum samples; parameter-to-metric links; low-burden time capture. |
| Requirements created | UNB-CDD-OEM-001; UNB-CDD-OEM-002; UNB-CDD-OEM-003; UNB-CDD-OEM-004 |
| Existing requirements refined | CNV-ECO-001; CNV-ECO-003; UNB-ING-OPS-002; UNB-CNET-025 |
| Disposition | New requirement added |
| Why | No existing record defines event-level measurement across all events. |
| Owner decisions raised | — |
| Approved decisions | DEC-006; DEC-008; DEC-012 |
| Acceptance criteria | (1) Given a straight-through grant, When it completes, Then one event record shows zero touches and its rule versions. (2) Given a measured parameter reaching its minimum sample, When economics re-runs, Then the measured value replaces the modelled one and is logged. |
| Validation requirements | (1) Instrumentation coverage test over every workflow transition (future implementation). (2) V-7 via the internal synthetic operating-year time study. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-007 — Requirements updated

| Field | Value |
|---|---|
| Research proposal (verbatim) | Prioritise prevention mechanisms: notification-first contribution intents, validated recommendation forms, verified destinations, pre-validated policy rules |
| Research target area | requirements (prevention) |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / No |
| Affected area | Prevention mechanisms (contributions, recommendations, destinations, policy rules) |
| Research evidence | 17-unbound-implications.md; 10-zero-touch-administration-economics.md; 09-event-level-operating-model.md |
| Claim and source ids | CLM-011; CHL-060; SRC-004; SRC-063 (register: SRC-004;SRC-063) |
| Affected existing requirements, clauses, decisions | UNB-ING-DAF-001; CNV-PAY-002; UNB-CNET-014; CNV-GRT-003; UNB-CHAR-020 |
| Already addressed? | Partly. Notification-first contribution intents (UNB-ING-DAF-001) and verified destinations (CNV-PAY-002, UNB-CNET-014) exist; validated recommendation forms and recommendation-time policy pre-validation do not, and no prevention mechanism has an efficacy metric. |
| Addressed by | UNB-ING-DAF-001; CNV-PAY-002; UNB-CNET-014 |
| Conflicts | None. Pre-validation is advisory; approval- and release-time checks under CNV-GRT-003 still govern. |
| Duplication | Contribution intents and verified destinations are cited, not restated. |
| Gaps | Recommendation-time pre-validation; validated recommendation forms; prevention efficacy metrics; explicit priority of prevention over AI exception handling. |
| Requirements created | UNB-CDD-OEM-005; UNB-CDD-OEM-006 |
| Existing requirements refined | UNB-ING-DAF-001; CNV-GRT-003 |
| Disposition | Requirements updated |
| Why | Two of four mechanisms exist; the package adds the missing one, the form rules and measurement. |
| Owner decisions raised | — |
| Approved decisions | DEC-009; DEC-005; DEC-012 |
| Acceptance criteria | (1) Given a recommendation to an excluded charity, When the donor enters it, Then the form shows the blocking reason before submission and the approval-time check still runs for others. (2) Given contributions with and without intent references, When efficacy is computed, Then exception rates are reported separately. |
| Validation requirements | (1) Efficacy metrics computed under UNB-CDD-OEM-002 versions. (2) Validator gate A6 on refined ids. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |

### XRI-008 — New requirement added

| Field | Value |
|---|---|
| Research proposal (verbatim) | Ensure Release 1 supports a technology-only licensing operating mode where the sponsor or administrator operates exceptions; treat the platform as a Nacha TPSP and plan for SOC 1 Type II (CHL-037/066) |
| Research target area | requirements (licensing mode) |
| Research type / status / blocking | requirement-candidate / proposed (not approved) / DEC-001 |
| Affected area | Operating modes: technology-only licensing; TPSP; SOC 1 Type II |
| Research evidence | 17-unbound-implications.md; 14-alternative-enterprise-models.md; 16-commercial-entry-strategy.md; 20-adversarial-review.md |
| Claim and source ids | CLM-014; CHL-037; CHL-058; CHL-066; OQ-009; SRC-063; DEC-001 (register: SRC-063) |
| Affected existing requirements, clauses, decisions | CNV-PAY-001; CNV-GOV-001; CNV-TEN-001; CNV-LEG-001; CNV-ECO-004; OD-M-08; UNB-CNET-023; OD-CNO-12 |
| Already addressed? | Partly. CNV-PAY-001 already records Unbound as a Third-Party Service Provider accepting TPSP obligations by contract; CNV-GOV-001 already requires SOC 1 and SOC 2 evidence engineering from inception and a SOC 1 Type 2 when a user auditor intends to rely; CNV-TEN-001 already supports Administrator principals under ServiceDelegation; OD-M-08 already plans software licensed to sponsors and administrators. |
| Addressed by | CNV-PAY-001; CNV-GOV-001; CNV-TEN-001 |
| Conflicts | No conflict on TPSP or SOC 1: both are already ratified. The research's earlier 'not a TPSP if no origination' framing (CHL-037) is rejected by the ratified text. The 'blocking: DEC-001' condition is lifted by the owner's approval. Nacha allocation with a transmitting partner remains OD-CNO-12. |
| Duplication | TPSP and SOC 1 duties are recorded per mode by citation (UNB-CDD-CMF-006), not restated as new obligations. |
| Gaps | No operating-mode catalogue; no rule that Release 1 runs without Unbound staff in sponsor workflows; no explicit check that no mode needs Echelon Foundry as sponsor or administrator; no module catalogue with non-separable controls. |
| Requirements created | UNB-CDD-CMF-001; UNB-CDD-CMF-002; UNB-CDD-CMF-004; UNB-CDD-CMF-005; UNB-CDD-CMF-006 |
| Existing requirements refined | CNV-TEN-001; CNV-PAY-001; CNV-GOV-001; UNB-ONB-023 |
| Disposition | New requirement added |
| Why | The TPSP and SOC 1 portion is already satisfied by ratified clauses; the licensing-mode portion needs new requirements. |
| Owner decisions raised | — |
| Approved decisions | DEC-001; DEC-006; DEC-002 |
| Acceptance criteria | (1) Given a technology-only licence generating ACH files, When the role table is validated, Then the platform provider is recorded as TPSP. (2) Given every Release-1 case kind in a test sponsor, When routing is checked, Then none requires an Unbound operator role. (3) Given any operating mode, When the role table is validated, Then no Echelon Foundry entity is required as sponsor or administrator. |
| Validation requirements | (1) Counsel review of TPSP allocation (OD-CNO-12). (2) CNV-GOV-001 planned-approach record per licensee. (3) Validator gate A9: the Echelon Foundry constraint row states no change. |
| Progression | Progression authorised by the owner on 2026-11-26, subject to its documented conditions; not implementation-authorised |
<!-- END GENERATED -->

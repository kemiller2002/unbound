# 15 — Final reconciliation and readiness assessment

Date: 2026-11-23 | Baseline: `777a70d` (original requirement files verified unchanged) | Gate verdict: **FAIL**. FAIL is the honest result. No readiness score is offered; every quantity below states its denominator.

## 1. Requirements gate

`CHARTER.md` sets the gate: all domains documented; critical scenarios with normal, exception and failure tests; legal assertions traced to authoritative evidence with review flags; decisions owned; contradictions adjudicated; operating models compared; blockers classified.

| Gate criterion | Status | Evidence |
|---|---|---|
| All twelve domains have documented coverage | **Not met.** UNB-ADV has 2 requirements; UNB-DON lifecycle is mostly gap requirements | [02 §3](02-QUALITY-AND-COVERAGE-REPORT.md) |
| Critical scenarios have normal, exception and failure tests | **Partially met on paper, unexecuted.** 124 reconciliation scenarios plus the prior program's 513 and the corpus's 963+ `ADV-*`; none executed; baseline tests do not cite requirement IDs | [12](12-DOKIMOS-ACCEPTANCE-SCENARIOS.md), [01 §7](01-BASELINE-INVENTORY.md) |
| Legal assertions have traceable authoritative evidence and review flags | **Partially met.** 49 obligations typed and cited; 11 counsel questions open; none reviewed by counsel | [06](06-REGULATORY-ASSUMPTION-REGISTER.md) |
| Decisions have owners | **Met for reconciliation decisions** (31 with an owner role); **not met** for baseline requirements, which have no accountable owner | [14](14-OPEN-DECISIONS.md) |
| Contradictions adjudicated | **Not met.** 39 conflicts recorded with recommendations; 11 need human authority | [03](03-CONFLICT-REGISTER.md) |
| Operating models compared | **Not met** (RCF-ENT-004) | — |
| Blockers classified | **Met** for this reconciliation | §8 below |
| Repository prerequisites | **Not met.** Conditor bootstrap report and Echelon capability matrix absent (RCD-002) | [01 §1](01-BASELINE-INVENTORY.md) |

The reconciliation's own quality gates (`tools/reqaudit/validate.py`, gates G1–G11) **pass with 0 errors and 30 warnings**. That says the reconciliation records are internally consistent and traceable. It does not say the requirements are approved.

## 2. How complete are the requirements?

Measured against defined denominators:

| Measure | Value | Denominator |
|---|---:|---|
| Canonical requirements with an RFC 2119 keyword | 234 (20.4%) | 1,148 |
| Canonical requirements with any acceptance signal | 246 (21.4%) | 1,148 |
| Canonical requirements cited by any test or scenario (after reconciliation) | 96 (8.4%) | 1,148 |
| Canonical requirements named in at least one finding | 409 (35.6%) | 1,148 |
| Canonical requirements with a full refinement record | 46 (4.0%) | 1,148 |
| Market capabilities covered (including draft REQ-X) | 16 + 1 by alternative (41.5%) | 41 capabilities |
| Market capabilities partially covered | 20 (48.8%) | 41 |
| Refined or new records with RFC 2119 keyword and Given/When/Then acceptance | 113 (100%) | 113 reconciliation records |

**Assessment.** The corpus is *broad*: it touches nearly every DAF capability and many rare failure modes, and the prior adversarial work is unusually deep. It is *not complete* in the sense that matters for implementation:

- Several statutory controls exist only as prose or controls, not as requirements: §4966 recipient classes, §4958 payee block, §170(f)(18) acknowledgment content, Form 8282, §4943 holdings and OFAC blocking.
- The donor lifecycle edges (dormancy, closure, outbound transfer, death) are thin.
- Roughly four in five requirements are not precise enough to test.

## 3. Which original requirements were incorrect?

| Original | Error | Severity | Correction |
|---|---|---|---|
| UNB-PLAT-007, UNB-ACC-026, UNB-TEC-STRATA-001..004, UNB-KNOW-014, UNB-CHAR-040, UNB-INV-042, UNB-NAV-051 | Assign append-only history, temporal queries, replay and integrity to Strata, a pilot schema-migration tool | Critical | RR-UNB-PLAT-007-r1, RR-UNB-ACC-026-r1 |
| REQ-X-007, REQ-X-018 (L2/G03), REQ-X-058, TX-27 | 180-day revalidation treated as legal currency; reliance is per grant date (Rev. Proc. 2018-32) | High | RR-REQ-X-007-r1 |
| ADV-133 and account 2050 (refund disposition), REQ-X-070 item 4 | Paths that could return accepted DAF assets to donors (§4966 taxable distribution) | Critical | UNB-RCN-FIN-013 |
| CANONICAL-CHART line 66 fund-to-fund example | Not balanced per fund | High | UNB-RCN-FIN-006 |
| ACC-MDL-004 / REQ-X-084 capacity as eventually consistent projection | Permits overcommitment | High | UNB-RCN-FIN-009 |
| UNB-ACC-006 | Orders acknowledgment before settlement; no content rule | Medium | RR-UNB-ACC-006-r1 |
| REQ-X-018 gate G05 | Bans all pledges, stricter than Notice 2017-73 | High | UNB-RCN-GRT-009 |
| Carried REG-001 row (WAVE-04:142) | "Approval cannot be vested in software" stated as law | Medium | RCD-005 |
| REQ-X-008 | Settles open owner decision OD-03 as if decided | High | RCD-022 |
| UNB-EXP-002 | Public "Unbound" branding contradicts the white-label platform | High | RCD-003 |
| REQ-X-036 | 7-year floor lacks the authority mapping needed to honour OFAC's 10 years | Medium | RR-REQ-X-036-r1 |
| UNB-SUCC-008 | Deceased party's authority, and their agents' authority, persists until succession activation | High | RR-UNB-SUCC-008-r1, UNB-RCN-ENT-004 |

## 4. Which requirements conflicted?

39 conflicts ([03](03-CONFLICT-REGISTER.md)). The ones that change the design:
- One system vs three tenancy profiles; global holds vs per-partner databases (critical).
- Delegated deterministic approval vs human approval of every grant.
- 180-day validity vs per-fact freshness.
- Atomic currency reservation vs liquidation-funded grants.
- Append-only history vs deletion at sponsor exit.
- Public brand vs white label.
- Folio vs Typst.
- Attestation vs auto-pass.
- Conditor-first vs "analysis may continue".

The corpus also has **identifier conflicts**: REQ-X-041 is defined twice and REQ-X-054/055 three times each; INV-01..16 and P-01..06 each carry two meanings; bare WF-nn has four meanings and GAP-nnn has four.

## 5. What significant requirements were missing?

67 gap-analysis requirements (30 P0) are listed in [04](04-MISSING-REQUIREMENTS-REGISTER.md). The most consequential:
- §4966 recipient classes and expenditure-responsibility tracking.
- The §4958(c)(2) related-party payee block.
- The sanctions Blocked state and OFAC reporting.
- The rule that accepted assets never return to donors.
- Acknowledgment content; Form 8283 and 8282 obligations; §4943 holdings.
- Per-fund balancing and in-transaction capacity.
- Explicit funding states.
- Resolution of Unknown payments, the check lifecycle, and destination evidence.
- Immediate suspension of authority on death.
- Dormancy, closure and outbound transfer.
- Account-type classification and the Schedule D package.
- The tenant model and global hold watermark.
- A single authority matrix.
- A requirements gate record and the release-1 scope.
- Donor-facing charity search.

## 6. Which market capabilities should be deliberately excluded?

Native mobile apps (responsive web instead); consumer payroll and round-ups; autonomous AI grant approval; direct crypto custody (use a liquidating intermediary); in-house international due diligence until counsel answers Q4; donor-level impact measurement (hypothesis only); and, for release 1, the visual email designer and IVR. Rationale: [05 §3](05-COMPETITIVE-COVERAGE-MATRIX.md); decisions RCD-008 and RCD-011.

## 7. Workflows, AI and determinism

**Traditional workflows to replace** ([09](09-WORKFLOW-IMPROVEMENT-PROPOSALS.md)):

| Replace | With |
|---|---|
| Calendar re-vetting | Change-driven verification with a grant-date IRS check |
| Human approval queues for routine grants | One deterministic decision function, so people see only holds |
| Per-occurrence review of recurring grants | Idempotent occurrence generation |
| Manual cash checks | Explicit funding states |
| Monthly reconciliation projects | Continuous reconciliation, with close as a snapshot |
| Quarterly fee runs | Daily accrual |
| Department queues | One exception inbox |
| Succession case queues | Hold, suspend, activate |
| Paper transfers | Transfer as a grant |
| Re-keyed Schedule D | Query over a locked period |
| In-house sponsor ERP and GRC | Integration |

**AI is genuinely useful** ([11](11-AI-AUTOMATION-OPPORTUNITIES.md)) for:
- extracting facts from charity and asset documents;
- comparing contradictory sources;
- investigating reconciliation breaks;
- explaining configuration and proposing diffs;
- case summaries and evidence requests;
- grounded donor answers;
- migration mappings;
- drafting copy for approval.

AI may add holds, never clear them.

**Deterministic automation must be preferred** for anything that can be written as a rule: eligibility, sanctions thresholds, capacity and reservation, fees, NAV tolerances, retention, dormancy stages, recurring occurrences, notifications, acknowledgments and closure. The corpus's problem was missing rules, not missing intelligence.

## 8. Remaining weaknesses, dependencies and decisions

**UI requirements that remain weak** ([10](10-UI-FORMA-FOLIO-GAP-ANALYSIS.md)):
- Staff experience organised by department.
- No Unknown or Failed states in screen contracts.
- Duplicate dashboards and command centres.
- Document accessibility that escapes through "where supported".
- The phone-complete-everything mandate.
- No terminology resolver or protected legal terms.
- Embedded-widget runtime ownership misattributed to Forma.

**Unverified architectural dependencies** ([13](13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md)):
- **Not present:** Strata event store; Forma charts, configurable dashboards, widget runtime and email designer; Folio tagged PDF, PDF/A, signing and templating; Typst; Dokimos scenario runner; Limen authentication and server; Knowledge Store schema and rules engine.
- **Partial or unknown:** Forma tables (partial), deterministic Folio PDF (planned only), Conditor adoption workflow (unknown), WASM bundle and cold-start budget (unmeasured).
- **Licence:** EF components carry a noncommercial licence, so suitability for commercial use is unconfirmed (RCD-027).

**Decisions requiring sponsor, legal or business review** ([14](14-OPEN-DECISIONS.md)): 31 decisions (10 legal, 8 product, 5 finance, 4 governance, 4 architecture), of which 18 block implementation, plus 11 counsel questions ([06 §5](06-REGULATORY-ASSUMPTION-REGISTER.md)). The legal core:
- Delegated approval and fund-manager identity.
- Cross-sponsor reliance on charity evidence.
- Supporting-organization control certification.
- Equivalency determination vs expenditure responsibility for foreign grants.
- Retention schedule.
- Advisor compensation under §4958.
- Money-transmitter, GLBA, UPMIFA and ICA applicability to the operator.
- Escheat of uncashed checks.

## 9. What would prevent implementation today?

1. No release-1 scope (RCD-013), so there is no bounded set of requirements to implement.
2. The persistence architecture as written depends on a capability that does not exist (Strata). The replacement design (`RR-UNB-PLAT-007-r1`) is unapproved.
3. The tenant unit and isolation profile are undecided (RCD-004), so the schema cannot be designed.
4. Counsel has not answered the questions that determine the grant gates (Q1–Q4), and the deterministic approval tier's authority depends on Q1.
5. The document renderer for statements and acknowledgments is unverified (RCD-009).
6. Tests do not trace to requirements, so there is no executable acceptance baseline. The F# test harness and Dokimos evidence path are unbuilt (DG-10).
7. The repository's own Conditor prerequisite is unmet (RCD-002).
8. The CHARTER and AGENT-MISSION explicitly forbid implementation until the gate passes, and the gate fails.

## 10. What should be refined next?

1. **Decide RCD-013 (scope) and RCD-004 (tenancy)**, then re-run this audit with release-1 as the denominator. Most gates become reachable within a bounded set.
2. **Adopt the 15 `autonomous-reversible` refinements** into the domain files (RCD-001), marking originals `superseded-by`, and fix the identifier collisions (REQ-X-041/054/055, INV, P, WF, GAP namespaces per UNB-RCN-ENT-003).
3. **Put the 11 counsel questions** to qualified counsel and the CPA questions to a CPA; then adopt the owner-decision refinements.
4. **Relink the 272 baseline `*-T*` tests** to the requirement IDs they verify, and convert the release-1 set into a Praxis requirement catalog (RCD-010), so Praxis and Dokimos can check it.
5. **Refine the remaining release-1 requirements** to the record standard in [07](07-REFINEMENT-RECORDS.md). Priority: advisor domain, donor lifecycle, fee engine, payment rail selection.
6. **Close the 29 G11 warnings** with scenarios, and the residual precision items noted in §11.
7. **Run the architecture spikes** (DG-04 renderer, DG-07 WASM budget, DG-09 ledger on PostgreSQL with Strata-governed append-only grants) and record ADRs.

## 11. Pass 8: consistency re-run over the refined set

- All 113 reconciliation requirement records (46 refinements, 67 gap requirements) contain an RFC 2119 keyword and at least one Given/When/Then acceptance criterion (median 2).
- Residual precision items: `support`/`supports` in RR-UNB-INV-002-r1, RR-UNB-PLAT-008-r1, UNB-RCN-ENT-008 and UNB-RCN-PLT-013; "where applicable" in UNB-RCN-PLT-009; an open-ended "etc." list in UNB-RCN-ENT-011, which is acceptable because the register it requires is the closed list. Five other hits are the legal term "supporting organization".
- Inconsistencies between domain passes, found and resolved during integration:
  - Tenancy: database per partner vs a shared cell vs the owner's existing row-level position. Resolved as database per sponsor with partner schemas (RCD-004).
  - Five duplicate refinements of the same original, merged.
  - One duplicate gap requirement, withdrawn (`data/withdrawn-ids.csv`).
  - A superseded Rev. Proc. citation in the grants workflow analysis, corrected.
  - AI-contract fields short of mission §10, amended.
  - A missing scenario for policy-change races (DKS-XD-013), added.
- Critical findings spot-verified against source by the integrating review: RCF-ARC-001, RCF-FIN-021 (ADV-133 and account 2050), RCF-GRT-013 (no Blocked state in the payment state machine), RCF-PLT-002 (serialization requirement coexisting with per-partner databases).

## 12. Provenance

Produced 2026-11-23 by an AI agent: four role-separated domain audit passes plus an integrating review, all from one model family. No human, counsel, CPA or owner has reviewed any finding. `daf-landscape` was not modified. Other Echelon repositories were read only. Commands that reproduce every table: [README](README.md#quality-gates).

## 13. Update after owner instructions (2026-11-23, second pass)

| Owner instruction | Done | Effect on §9 blockers |
|---|---|---|
| Merge | PR #2 merged into `main` | — |
| Make best decisions and note | All 31 decisions recorded: 12 `decided`, 19 `decided-provisional` with named validation; counsel working positions for Q1–Q11 (`data/counsel-positions.csv`) | Blockers 1 (scope), 3 (tenant unit) and 5 (renderer) are resolved for design. Blocker 4 (counsel) is now a production gate, not a design gate. |
| Adopt | 113 records folded into 23 domain files: 37 `adopted`, 76 `adopted-provisional`; originals kept verbatim and annotated (`data/adoption.csv`). Identifier collisions fixed with a register (`data/renamed-ids.csv`). | Blocker 2 is resolved for design (the persistence design is adopted provisionally pending spike DG-09). |
| Refine UI before converting; make sure Forma and Folio can handle everything | 38 UI requirements ([16](16-UI-REQUIREMENTS-AND-DESIGNS.md)); 14 screen and 7 document prototypes built on the real Forma and Folio with no custom CSS; gate G12 | Every screen fits with recorded gaps. The fund statement needs render-time page rules. |
| File the gaps in their repositories | Forma #186–#198, Folio #59–#67, and evidence on folio#47 | — |

**Gate verdict remains FAIL.** What still stands between this and an implementation gate:
- counsel validation of the 19 provisional decisions;
- execution of the scenarios (no test harness yet, DG-10);
- the persistence and renderer spikes (DG-09, DG-04);
- the Conditor prerequisite (RCD-002 waiver covers requirements work only);
- conversion to a Praxis catalog, deferred by the owner (RCD-010).

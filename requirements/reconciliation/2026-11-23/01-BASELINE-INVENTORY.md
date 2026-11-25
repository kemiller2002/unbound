# 01 — Requirements inventory and baseline (pre-change)

Date: 2026-11-23 | Baseline revision: `777a70d27a70e7e6851560ed513a872b5b4d134b` (branch `main` at fork) | Status: observational record of the repository **before** any reconciliation change. Nothing in this file alters an original requirement. `data/summary.json` records `originals_identical_to_baseline: true`, so the counts below describe the baseline even when re-run later on this branch.

## 1. Repository discovery

| Item | Finding | Evidence |
|---|---|---|
| Inbound requirements repository | `kemiller2002/unbound` (local `/home/user/unbound`). It holds the charter, baseline, domain specifications, control catalogs, simulation catalogs and a prior theoretical-validation program. | `requirements/CHARTER.md`, `requirements/BASELINE-001.md` |
| Product name | The repository names the enterprise **Unbound** (`UNB-EXP-002`: "Public identity SHALL be independently branded as Unbound"). The mission brief calls the platform **"Strata"**. In this repository, *Strata* is the name of an Echelon persistence component (`UNB-PLAT-007`, `UNB-ACC-026`). | `requirements/BASELINE-001.md`; `requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md:11` |
| What `kemiller2002/strata` actually is | A **pilot-status declarative PostgreSQL schema-management tool** (v0.1.1 in Registry channel `echelon-current` 1.39.0): compile `.sql` → diff against a live database → judge → deploy. It is not an event store, ledger, or temporal database. | `kemiller2002/strata@c6a384e:README.md`; Registry `echelon-registry@fef1f94` |
| Read-only evidence repository | `kemiller2002/daf-landscape` (1,858 files). Not modified by this mission. | — |
| Echelon repositories inspected (read-only) | strata `c6a384e`, forma `d7c499e`, folio `56a18e1`, limen `743cdb9`, dokimos `6ea8624`, praxis `12bf995`, echelon-knowledge-store `35d9cee`, echelon-diagnostic-framework (EDF) `710d54c`, clarity-framework `200a9eb`, echelon-registry `fef1f94`, Echelon-AI-Native-Enterprise-Design-Framework `763ff8c`. | [13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md](13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md) |
| Repository governance | No `AGENTS.md`, CI workflow, PR template or validation tooling in `unbound`. Governing documents: `CHARTER.md` (requirements gate; "no application implementation until the requirements baseline passes a formal review gate"), `PRINCIPLES.md` ("preserve the outcome, not the process"), `AGENT-MISSION.md`, and the SOC 2 evidence-by-design directive. | `requirements/*.md` |
| Unmet governance prerequisite | `AGENT-MISSION.md` step 0 makes a Conditor bootstrap mandatory before requirements execution and requires `requirements/conditor-installation-report.md`; step 1 requires `requirements/echelon-capability-matrix.md`. **Neither file exists.** This mission did not install Conditor: the work is requirements reconciliation only, and installing tooling is outside its authority. Recorded as decision RCD-002. | `requirements/AGENT-MISSION.md:5-6` |

## 2. Corpus scope and extraction method

The extractor `inventory` (`tools/fs/Unbound.ReqAudit`) (pure functional pipeline, re-runnable) scans 181 files in `requirements/`, `architecture/` and `research/daf-theoretical-enterprise-validation/`. It recognises eight definition styles found in the repository:

| Site | Example | Count of definition sites |
|---|---|---:|
| bold bullet | `- **UNB-ACC-001:** …`, `- **ADM-001 Title** (owner): …` | 1,729 |
| line-start | `GOV-AUTH-001: …`, `CHAR-T01: …`, `**UNB-TEC-STRATA-001 (MANDATORY):**` | 326 |
| heading | `### REQ-X-001: …` (heading plus following block) | 341 |
| table row | `\| INV-01 \| … \|` | 884 |
| catalog line | `GR-EVT-001 GrantIntentRecorded` | 113 |
| CSV first column | `"UNB-CTRL-001", …` | 1,537 |
| **Total definition sites** | | **4,930** |

Crosswalk CSVs whose first column *references* a requirement are excluded from definitions (`REQUIREMENT-TO-CONTROL-CROSSWALK-WAVE-01.csv`, `DETAILED-REQUIREMENT-CONTROL-LINKS-WAVE-02.csv`, `ENTERPRISE-CAPABILITY-TO-CONTROL-CROSSWALK.csv`, the empty simulation template). Bullets in `13-adversarial-reviews/raw/` are reviewer commentary and are excluded. Bullets that are only lists of identifiers are treated as cross-references.

Every definition site records identifier, kind, path, line, git blob SHA at the baseline revision, nearest heading and **exact original text** (`data/inventory.csv`). This is the immutable source reference for every refinement.

## 3. Identifier inventory

4,238 distinct identifiers (`data/summary.json`):

| Kind | Distinct IDs | Notes |
|---|---:|---|
| requirement | 1,176 | 1,148 after excluding `-Q` question restatements (the denominator used in reports 02 and 15) |
| scenario | 1,503 | `ADV-*` (963 numbered), `TVS-*` (513), `SIM-*`, `SC-*`, `XSIM-*`, `UNB-XSC-*` |
| test | 272 | `*-T01..` style acceptance cases embedded in domain files and CSVs |
| finding | 274 | `TVF-*`, `GAP-*` |
| control | 238 | `UNB-CTRL-*` (90), `UNB-SCTRL-*` (46), `CTL-*`, `TVCTL-*`, `VAL-*`, `CTRL-*` |
| decision | 123 | `OD-*`, `*-ADR-*`, `*-DEC-*` |
| invariant | 69 | `FI-*`, `INV-*`, `CONC-*` |
| regulatory assertion | 65 | `REG-*` |
| other | 298 | document-local labels (`ACC-01`, `P-01`, `A-1`, …) |

Largest requirement families: REQ-X 84, UNB-NAV 53, ADM 49, UNB-GRANT 48, UNB-CHAR 42, UNB-INV 42, UNB-GEX 40, UNB-KNOW 40, UNB-EXT 38, SOC2 36, UNB-ACC 36, AIN 36 (full list in `data/requirements-quality-summary.json`).

### Versions, statuses and ownership

- **No requirement carries an individual version or status.** Status is declared per file, and every file declares a non-approved state, e.g. "candidate requirements; NOT approved" (BASELINE-001), "owner-directed mandatory requirements; detailed … review pending" (FULL-ACCOUNTING-SYSTEM), "requirements only; no runtime, tests …" (grant event catalog), "Draft, not formally approved" (REQ-X overlay).
- **Ownership** appears only as "proposed owner" roles in the CSV matrices (ADM, control catalogs) and as bracketed role labels on ADM/AIN bullets. No requirement has an accountable named decision owner, which the charter's requirement record demands.
- **CSV status columns** (where present) are uniformly `Proposed`, `Specified`, `Designed`, `Not executed`, `Not simulated`, with `implemented = No`, `tested = No`, `operating_evidence_confirmed = No`.

## 4. Narrative-only requirements and ID migration strategy

132 normative statements (containing SHALL/MUST/must/never/required/prohibited and similar) have no identifier (`data/narrative-requirements.csv`; `narrative` (`tools/fs/Unbound.ReqAudit`)). 30 files contain requirements **only** in narrative or unlabelled form, including `CHARTER.md`, `PRINCIPLES.md`, `EXPANSION-ROADMAP-AND-GATES.md`, `DAF-LANDSCAPE-CONVERSION-WAVE-02/03.md`, `ui/CONTROL-OPERATIONS-ROLE-WORKSPACES.md` and `ui/DAF-ADMINISTRATION-AND-SPONSOR-EXPERIENCE.md`.

**Migration strategy (adopted, reversible):**
1. Existing identifiers are **never** renumbered or reused, even when they collide. Collisions are disambiguated by qualified reference `ID@path` (for example `INV-01@CROSS-WORKFLOW-FINANCIAL-AUTHORITY…md`).
2. Statements without an identifier receive a content-addressed surrogate `NARR-<8 hex>` = `sha256(path + "\n" + normalized text)[:8]`. The surrogate is stable across line moves and changes if the wording changes, so a reworded statement is never silently treated as the original.
3. Refinements use `RR-<original-id>-r<n>`. New gap-analysis requirements use `UNB-RCN-<domain>-<nnn>` with `origin: gap-analysis`. Neither scheme overlaps any existing family (verified by `validate` (`tools/fs/Unbound.ReqAudit`)).
4. Original files are not edited. Supersession is recorded in the traceability graph (`data/trace-edges.csv`); adoption into the domain files is an owner decision (RCD-001).

## 5. Identifier integrity defects found at baseline

| Class | Count | Examples (verified by reading) |
|---|---:|---|
| Same-file redefinition (true collision) | 3 IDs | `REQ-X-041` defined twice (lines 392, 398, identical titles: probable copy-paste); `REQ-X-054` defined **three** times with different scopes (lines 476, 488, 500); `REQ-X-055` defined three times (lines 482, 494, 506), all in `19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md` |
| Cross-file divergent text (needs human review) | 120 IDs | Confirmed namespace collisions: `INV-01..16` mean different invariants in `CROSS-WORKFLOW-FINANCIAL-AUTHORITY-AND-IRREVERSIBILITY-INVARIANTS-2026-11-23.md` and `TYPED-DECISION-CONTRACTS-STATE-TRANSITIONS-AND-CONCURRENCY-2026-11-23.md`; `P-01..06` differ between `DECISION-INVENTORY-SECOND-PASS` and `ENTERPRISE-DECISION-INVENTORY`. The remainder are mostly terse summaries of longer definitions (for example `TVA-*` in the executive summary). |
| Mirror or summary (benign) | 356 IDs | `OPS-001..032` in both `CONTROL-OPERATIONS-LIFECYCLE.md` and `CONTROL-OPERATIONS-REQUIREMENTS.csv`; REQ-X-058..077 summarised in the overlay and specified in detail files |
| Referenced but never defined in this repository | 605 IDs | 49 requirement-kind: e.g. `AUTH-03/06/07/09/10`, `OPS-01..03`, `TVR-WID-004`, `TVR-GR-007`, `UX-F19`, `ADM-12/20/23`. 25 workflow IDs `CW-21..CW-45` are defined only in `daf-landscape`. 82 `ADV-*` identifiers are cited but not defined. |

## 6. Prior reviews, questions and exceptions located

- **Theoretical enterprise validation program** (`research/daf-theoretical-enterprise-validation/`): 513 scenarios, 242 findings (`TVF-*`), 44 contradictions (`TVC-*`), 30 assumptions (`TVA-*`), 22 owner decisions (`OD-*`), 183 consolidated open questions, readiness report. Its README states that nothing is implemented, legally reviewed or independently validated. The same program also exists in `daf-landscape/research/theoretical-enterprise-validation/`; the Unbound copy adds the 19-questions REQ-X overlay.
- **Gap registers:** `ENTERPRISE-GAP-REGISTER-AND-CROSS-DOMAIN-SCENARIOS.md` (`UNB-GAP-*`), `DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md`, and `DAF-LANDSCAPE-SECOND-PASS-RECONCILIATION-GAP-REGISTER-AND-WORKFLOW-GATES-2026-11-23.md` (20 landscape gaps).
- **Control reconciliation waves 01-03**: 493 + 1,892 requirement-to-control links. Every link is labelled "Keyword and domain candidate; objective-level review required" / "Proposed, not validated".
- **Known exceptions:** "No live outbound payment execution is authorized" (SOC 2 directive); synthetic data only (PRINCIPLES hard boundaries).

## 7. Baseline quality signals

Heuristic triage only (`quality` (`tools/fs/Unbound.ReqAudit`); definitions in report 02). Denominator: the 1,148 canonical requirement identifiers.

| Area | Requirements | No RFC 2119 keyword | No acceptance signal | Vague terms | ≥8 clauses | Cited by any test/scenario |
|---|---:|---:|---:|---:|---:|---:|
| architecture | 44 | 40 | 38 | 6 | 3 | 0 |
| requirements/(root) | 334 | 168 | 277 | 60 | 51 | 0 |
| requirements/accounting | 12 | 12 | 12 | 0 | 2 | 0 |
| requirements/administration | 48 | 48 | 44 | 4 | 0 | 0 |
| requirements/ai | 36 | 36 | 34 | 1 | 0 | 0 |
| requirements/charities | 112 | 107 | 106 | 18 | 11 | 0 |
| requirements/governance | 118 | 94 | 100 | 8 | 8 | 0 |
| requirements/grants | 136 | 133 | 126 | 29 | 6 | 0 |
| requirements/integrations | 28 | 17 | 25 | 8 | 13 | 0 |
| requirements/investments | 95 | 76 | 87 | 20 | 14 | 0 |
| requirements/ui | 32 | 32 | 31 | 12 | 12 | 0 |
| research/…/validation (REQ-X, TVR) | 153 | 151 | 22 | 18 | 89 | 24 |
| **Total** | **1,148** | **914** | **902** | **184** | **209** | **24** |

The structural headline: **acceptance tests exist (272 `*-T*` cases) but do not cite the requirement IDs they verify**. Only 24 requirements are referenced by ID in any test or scenario record. Bidirectional requirement↔test traceability therefore does not exist at baseline.

## 8. Reproduce

```bash
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- inventory . requirements/reconciliation/2026-11-23/data
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- quality requirements/reconciliation/2026-11-23/data .
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- narrative . requirements/reconciliation/2026-11-23/data/narrative-requirements.csv
```
The inventory excludes `requirements/reconciliation/`, so re-running after this mission reproduces the baseline counts as long as the original files are unchanged.

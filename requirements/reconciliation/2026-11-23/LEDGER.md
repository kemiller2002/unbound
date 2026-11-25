# Ledger — requirements reconciliation 2026-11-23

**Branch:** `claude/strata-requirements-audit-kskwzy` (kemiller2002/unbound). **Baseline:** `777a70d`. **Original requirement files modified:** none (`data/summary.json`: `originals_identical_to_baseline: true`). **daf-landscape modified:** no.

## 1. How to resume

1. Read [README](README.md), then [15-READINESS-ASSESSMENT.md](15-READINESS-ASSESSMENT.md) §9–§10 (blockers and next refinements).
2. Records live in `data/`. Reports render their tables from data, so never hand-edit content between `<!-- BEGIN GENERATED -->` markers.
3. To add a finding, refinement, gap requirement, conflict, scenario or decision: append to the matching data file using the schema in the existing rows. Then run the pipeline below and fix every ERROR.
4. ID schemes and the relationship vocabulary are in the README. Never reuse a withdrawn ID (`data/withdrawn-ids.csv`).

```bash
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- inventory . requirements/reconciliation/2026-11-23/data
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- quality requirements/reconciliation/2026-11-23/data .
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- narrative . requirements/reconciliation/2026-11-23/data/narrative-requirements.csv
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- graph requirements/reconciliation/2026-11-23/data
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- render .
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- validate .
```

## 2. Passes completed

| Pass | Work | Outputs |
|---|---|---|
| 1 Inventory and baseline | Extractor for 8 definition styles over 181 files; collision classification; narrative surrogates; heuristic quality signals | 01, `data/inventory.csv` and related files |
| 2 Correctness and conflicts | Four domain audits plus the regulatory evidence check (web-verified 2026-11-23) and Echelon capability verification | 02, 03, 06, 13 |
| 3 Coverage | Competitive evidence extraction (755 graded rows, 41 capabilities); gap requirements | 04, 05 |
| 4 Workflow redesign | 19 workflows challenged | 09 |
| 5 Specification refinement | 46 refinement records, 67 gap requirements | 07, 04 |
| 6 Architecture and UI | Forma, Folio, Limen, Dokimos, Praxis, Strata, Knowledge Store, EDF and Clarity checked; UI gap analysis; AI contract | 10, 11, 13 |
| 7 Adversarial | 124 scenarios, including every mission-required adversarial case | 12 |
| 8 Consistency | Integration review; consolidations; validator re-run; refined-set quality check | 14, 15 §11 |

## 3. Checkpoints (commits on the branch)

| Commit | Content |
|---|---|
| c4df75e | Baseline inventory, extraction tooling, quality signals |
| 24ce2eb | Competitive coverage, regulatory register, architecture dependencies |
| 618f034 | Validator, graph generator, README, REG/ARC records |
| b40ea9b | Four domain audits merged; findings, conflicts, refinements, gaps, scenarios, decisions |
| c2f91a2 | Reports 02–04, 07–12, 14; renderer; integration fixes |
| d46382f | Report 15, ledger, baseline-number refresh, final validation |
| (this) | Narrative surrogates attributed to the baseline revision; ledger hashes |

## 4. Validation results (last run)

See `data/validation-report.json`: **0 errors, 30 warnings**. The warnings are 29 refinements and gap requirements without a scenario (G11) and one reference to the never-defined `TVR-GR-007` (G8u).

## 5. Agent provenance and telemetry

| Pass | Kind | Sub-agent tokens (harness-reported) |
|---|---|---:|
| Echelon capability verification | read-only research | 171,747 |
| Regulatory evidence (incl. web verification) | read-only research | 205,260 |
| Competitive evidence extraction | read-only research | 313,729 |
| Domain audit GRT | role-separated audit | 312,077 |
| Domain audit FIN | role-separated audit | 297,504 |
| Domain audit PLT | role-separated audit | 325,550 |
| Domain audit ENT | role-separated audit | 306,263 |
| **Sub-agent total** | | **1,932,130** |

The integrating session's own token use is not reported by the harness and is not estimated. No human intervention occurred during the run. This is relevant to the EF capability ledger required by `PRINCIPLES.md`.

## 6. Blockers encountered

- Conditor bootstrap (AGENT-MISSION step 0) was not performed: it is outside a requirements-reconciliation mandate. Recorded as RCD-002.
- No Echelon repository was modified. Capability gaps are recorded in 13 §4 only.
- Some vendor sites were unreadable in the original research. These are recorded as "unknown" in 05; no new vendor testing was done.

## 7. Second pass (owner instructions, 2026-11-23)

- PR #2 merged. Branch restarted from `main` at `940b067`.
- Decisions recorded (`data/decisions.csv`, `data/counsel-positions.csv`).
- Adoption: `adopt` (`tools/fs/Unbound.ReqAudit`) (idempotent) folds records into domain files between `BEGIN/END ADOPTED` markers. Re-run it after changing any record.
- Renames recorded in `data/renamed-ids.csv`. The inventory now scans baseline revision `777a70d` by default (`REQAUDIT_REV=WORKTREE` to scan the working tree).
- UI: `data/ui-requirements.jsonl` and `data/ui-component-gaps.csv` are generated by the authoring script; the catalog snapshots are `forma-catalog.csv` and `folio-elements.csv`.
- Prototypes: `requirements/ui/designs/`. Rebuild and verify with `tools/build-screens.mjs` + `tools/verify-screens.mjs` and `tools/build-documents.mjs` + `tools/verify-documents.mjs`. The documents need a local HTTP server for ES modules.
- The Forma CSS was compiled from source with .NET 10 (`npm run tokens:build`, then `brands:build`, `color:build`, `runtime:build` and `assets:build`). It is vendored at `vendor/forma-1.0.2/`.
- Issues filed: Forma #186–#198 (#186 and #187 corrected after the fit check), Folio #59–#67, and a comment on folio#47.
- Sub-agent telemetry: Forma screens 326,621 tokens; Folio documents 207,099 tokens.

## 8. Third pass: UI quality addendum (report 18)

- **Design system baseline.**
  - Forma 1.0.2 tokens verified.
  - Charcoal and Bronze come from `kemiller2002/echelon-design-system` theme 1.1.0 (read-only clone, not modified).
  - The `unbound-default` brand was compiled with Forma's BrandCompiler.
  - Vendored assets: `requirements/ui/designs/vendor/unbound-brands/`, plus the OFL fonts in `vendor/echelon-theme-1.1.0/`.
- **v2 prototypes.**
  - Location: `requirements/ui/designs/v2/`.
  - Rebuild and verify: `SCREENS=S-001,S-003,S-004,S-008,S-011,S-012,W-001 node tools/build.mjs`, then `node tools/verify.mjs`. A partial build keeps the other screens' entries in `pages.json`.
  - Result: 42 of 42 pages pass at five viewports.
- **Data authoring scripts.** Copied to `tools/reqaudit/authoring/`, with their scratchpad inputs in `specs/`. Run them in this order; each is idempotent:
  1. `ui_quality.py`: UI quality requirements, revisions with content-addressed tags, RCD-050..053.
  2. `merge_specs.py`: panel and view specs; gaps FORMA-GAP-24..26 and UNB-GAP-W-02.
  3. `ui_trace.py`: defects, state matrix, coverage, UIR records, DKS-UI scenarios.

  Do **not** re-run the old `ui_build.py` alone: it rewrites `ui-requirements.jsonl` from the first-pass records only.
- **Audit.** 63 v1 defects (`data/ui-defects.csv`); 47 are resolved in v2 or prevented by the gate, and 16 are open on screens not yet redesigned (roadmap waves 2–3).
- **Filed (owner instruction, 2026-11-24).** FORMA-GAP-18..26 are kemiller2002/forma#199–#207; the primary-button finding is a comment on forma#186.
- **Workflow-model issues found while specifying views** (not yet fixed in `workflows/`):
  - WFM-14 has no deterministic "publish in-tolerance NAV by policy" transition and no sampled-review step; the note on WFM-14-T11 is stale against RCD-035.
  - WFM-12 has no human transition for recording an OFAC rejected-transaction report (UNB-RCN-WF-024).
  - WFM-22 and WFM-02 start in their initial state with no human initiation transition.
- **Telemetry.** Sub-agent tokens: v1 audit 286,758; donor redesign 233,915; ops, charity and widget redesign 219,829; view specs 231,939.

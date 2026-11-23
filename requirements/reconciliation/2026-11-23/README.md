# Requirements reconciliation, validation and refinement — 2026-11-23

**Scope.** An evidence-driven audit of every requirement in this repository, checked against the `daf-landscape` research (read-only), current U.S. DAF law and guidance, the competitive landscape, and the verified capabilities of the Echelon components. This is a **requirements engineering mission, not an implementation**. No application code was written.

**Naming.** The mission brief calls the platform "Strata". This repository names the enterprise **Unbound** (`UNB-EXP-002`), and *Strata* is the Echelon PostgreSQL schema-management tool. These reports use "Unbound" for the platform and "Strata" only for the tool. See decision RCD-003.

**Original requirements are unchanged.** Every refinement, finding and new requirement lives in this directory and links back to the original by identifier, path, line, baseline revision (`777a70d`) and exact original text. Adoption into the domain files is an owner decision (RCD-001).

## Deliverables

| # | Deliverable | File | Machine-readable data |
|---|---|---|---|
| 1 | Requirements inventory and baseline | [01-BASELINE-INVENTORY.md](01-BASELINE-INVENTORY.md) | `data/inventory.csv`, `id-index.csv`, `duplicate-ids.csv`, `undefined-references.csv`, `narrative-requirements.csv`, `summary.json` |
| 2 | Requirements quality and coverage report | [02-QUALITY-AND-COVERAGE-REPORT.md](02-QUALITY-AND-COVERAGE-REPORT.md) | `data/requirements-quality.csv`, `findings.csv` |
| 3 | Conflict and contradiction register | [03-CONFLICT-REGISTER.md](03-CONFLICT-REGISTER.md) | `data/conflicts.csv` |
| 4 | Missing requirements register | [04-MISSING-REQUIREMENTS-REGISTER.md](04-MISSING-REQUIREMENTS-REGISTER.md) | `data/gap-requirements.jsonl` |
| 5 | DAF competitive capability coverage matrix | [05-COMPETITIVE-COVERAGE-MATRIX.md](05-COMPETITIVE-COVERAGE-MATRIX.md) | `data/competitive-evidence-rows.csv`, `competitive-rollup.csv` |
| 6 | Regulatory and policy assumption register | [06-REGULATORY-ASSUMPTION-REGISTER.md](06-REGULATORY-ASSUMPTION-REGISTER.md) | `data/regulatory-obligations.csv` |
| 7 | Requirements refinement records | [07-REFINEMENT-RECORDS.md](07-REFINEMENT-RECORDS.md) | `data/refinements.jsonl` |
| 8 | Bidirectional traceability graph | [08-TRACEABILITY-GRAPH.md](08-TRACEABILITY-GRAPH.md) | `data/trace-edges.csv` (generated) |
| 9 | Workflow improvement proposals | [09-WORKFLOW-IMPROVEMENT-PROPOSALS.md](09-WORKFLOW-IMPROVEMENT-PROPOSALS.md) | — |
| 10 | UI and Forma/Folio gap analysis | [10-UI-FORMA-FOLIO-GAP-ANALYSIS.md](10-UI-FORMA-FOLIO-GAP-ANALYSIS.md) | — |
| 11 | AI automation opportunity analysis | [11-AI-AUTOMATION-OPPORTUNITIES.md](11-AI-AUTOMATION-OPPORTUNITIES.md) | — |
| 12 | Dokimos-ready acceptance scenarios | [12-DOKIMOS-ACCEPTANCE-SCENARIOS.md](12-DOKIMOS-ACCEPTANCE-SCENARIOS.md) | `data/scenarios.csv` |
| 13 | Architecture dependency analysis | [13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md](13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md) | — |
| 14 | Open decisions and unresolved questions | [14-OPEN-DECISIONS.md](14-OPEN-DECISIONS.md) | `data/decisions.csv` |
| 15 | Final reconciliation and readiness assessment | [15-READINESS-ASSESSMENT.md](15-READINESS-ASSESSMENT.md) | `data/validation-report.json` |
| — | Checkpoint ledger (resume here) | [LEDGER.md](LEDGER.md) | — |

## Identifier schemes (new; none overlap a baseline family)

| Scheme | Meaning |
|---|---|
| `RR-<original-id>-r<n>` | Refinement record of an original requirement (`refines` or `supersedes`) |
| `UNB-RCN-<DOMAIN>-<nnn>` | New requirement, `origin: gap-analysis` |
| `RCF-<DOMAIN>-<nnn>` | Finding (defect) |
| `RCC-<DOMAIN>-<nnn>` | Conflict |
| `RCD-<nnn>` | Open decision requiring human authority |
| `DKS-<DOMAIN>-<nnn>` | Dokimos-ready acceptance scenario |
| `NARR-<8 hex>` | Content-addressed surrogate for a normative statement that has no identifier |

Withdrawn identifiers are listed with their replacement in `data/withdrawn-ids.csv` and must never be reused (gate G1).

Domains: `GRT` grants, charities and payments · `FIN` finance, contributions and investments · `PLT` platform, AI, UI and governance · `ENT` enterprise, donor lifecycle and operating model · `REG` regulatory · `ARC` architecture.

## Relationship vocabulary (`data/trace-edges.csv`)

`refines`, `supersedes`, `conflicts_with`, `duplicates`, `depends_on`, `derived_from`, `validated_by`, `implemented_by` (reserved), `tested_by`, plus `affects` (finding → requirement) and `decided_by` (record → open decision). The graph is **generated** from the records by `tools/reqaudit/graph.py` and must not be edited by hand.

## Quality gates

```bash
python3 -I tools/reqaudit/inventory.py . requirements/reconciliation/2026-11-23/data   # baseline inventory
python3 -I tools/reqaudit/quality.py requirements/reconciliation/2026-11-23/data .     # heuristic signals
python3 -I tools/reqaudit/graph.py requirements/reconciliation/2026-11-23/data          # regenerate edges
python3 -I tools/reqaudit/render.py .                                                    # refresh generated tables in reports
python3 -I tools/reqaudit/validate.py .                                                  # gates G1-G11; non-zero exit on error
```

Gates: duplicate identifiers (G1); collisions with baseline IDs (G2); originals exist (G3) and are quoted verbatim at the baseline revision (G4); required fields (G5); Given/When/Then acceptance criteria (G6); gap-analysis origin and finding (G7); references resolve (G8); edge vocabulary and endpoints (G9); identifiers cited in these reports resolve (G10); every refinement and gap requirement is tested by a scenario (G11, warning).

## Evidence discipline

- `daf-landscape` is cited as `daf-landscape:<path>` and was not modified.
- Other Echelon repositories were read only. Gaps in them are recorded in [13](13-ARCHITECTURE-DEPENDENCY-ANALYSIS.md), not raised there.
- Legal statements carry an authority type. Anything uncertain is a counsel question in [06](06-REGULATORY-ASSUMPTION-REGISTER.md) §5, not a requirement.
- Domain audits were performed by four role-separated agent passes against a common brief, then merged, checked by the validator and spot-verified by the integrating agent. All passes are the same model family, so this is not independent human review.

# 08 — Bidirectional traceability graph

Date: 2026-11-23 | Graph: [`data/trace-edges.csv`](data/trace-edges.csv) (`source, relationship, target, provenance`). **Generated** by `tools/reqaudit/graph.py` from the record files. Do not edit it by hand.

## 1. Model

Nodes are baseline identifiers (`data/inventory.csv`), narrative surrogates (`NARR-*`), and reconciliation identifiers (`RR-`, `UNB-RCN-`, `RCF-`, `RCC-`, `RCD-`, `DKS-`). Competitive capability (`CAP-*`) and obligation (`OB-*`) identifiers are also valid nodes.

| Relationship | Direction | Produced from |
|---|---|---|
| `refines` / `supersedes` | refinement → original | refinement `relationship` |
| `derived_from` | refinement or gap → finding; finding → prior TVF/TVC/OD; gap → parent | `related_findings`, `gap_finding`, `prior_ids`, `parent` |
| `depends_on` | refinement or gap → requirement | `dependencies`, `links` |
| `affects` | finding → requirement | `affected_ids` (ranges such as `UNB-SUCC-001..014` are expanded) |
| `duplicates` | requirement → canonical duplicate | findings titled "Duplicate:" |
| `conflicts_with` | requirement ↔ requirement | conflict register |
| `tested_by` | requirement or refinement → scenario | scenario `requirement_ids`; a scenario citing an original also tests its refinement |
| `decided_by` | record → open decision | decision `affected_ids` |
| `validated_by` | requirement → evidence of validation | **reserved**: no requirement has been validated |
| `implemented_by` | requirement → code | **reserved**: no implementation exists |

Bidirectionality: every edge can be queried from either end. For example, "which findings affect UNB-GRANT-012?" is `target = UNB-GRANT-012 AND relationship = affects`; "what does RR-REQ-X-007-r1 refine?" is `source = RR-REQ-X-007-r1`.

## 2. Edge counts

<!-- BEGIN GENERATED: trace-summary -->
| Relationship | Edges |
|---|---|
| affects | 822 |
| derived_from | 511 |
| depends_on | 397 |
| tested_by | 307 |
| decided_by | 141 |
| duplicates | 97 |
| conflicts_with | 56 |
| refines | 39 |
| supersedes | 7 |
| **Total** | 2377 |
<!-- END GENERATED: trace-summary -->

## 3. Coverage (denominators stated)

<!-- BEGIN GENERATED: trace-coverage -->
| Measure | Count | Share |
|---|---|---|
| Baseline canonical requirements (denominator) | 1148 |  |
| … affected by at least one finding | 409 | 35.6% |
| … with a refinement record | 46 | 4.0% |
| … tested_by at least one scenario (baseline + reconciliation scenarios) | 96 | 8.4% |
| Reconciliation refinements + gap requirements (denominator) | 113 |  |
| … tested_by at least one scenario | 84 | 74.3% |
<!-- END GENERATED: trace-coverage -->

Reading the coverage numbers:
- "Affected by a finding" counts requirements named in at least one finding. A requirement *not* affected is **unreviewed or acceptable, not validated**. The audit read every file, but it recorded findings only for material defects and did not issue per-requirement verdicts.
- "Tested_by" counts links from scenarios, including the prior program's scenarios where they cite requirement IDs. Baseline tests (`*-T*`) do not cite requirement IDs at all (report 01 §7), so they cannot contribute until they are relinked.

## 4. Validation gates

<!-- BEGIN GENERATED: validation -->
| Record set / result | Count |
|---|---|
| refinements | 46 |
| gaps | 67 |
| findings | 175 |
| conflicts | 39 |
| scenarios | 124 |
| decisions | 31 |
| ui | 38 |
| trace edges | 2377 |
| **errors** |  |
| warnings | 30 |
<!-- END GENERATED: validation -->

Warnings are kept visible on purpose:
- **G11:** refinements and gap requirements that have their own Given/When/Then criteria but no adversarial scenario yet.
- **G8u:** references to identifiers the baseline cites but never defines (for example `TVR-GR-007`).

## 5. Example queries

```bash
# everything that refines or supersedes a requirement
awk -F, '$3=="\"REQ-X-007\"" || $3=="REQ-X-007"' requirements/reconciliation/2026-11-23/data/trace-edges.csv
# all scenarios for a gap requirement
grep '^UNB-RCN-GRT-004,tested_by' requirements/reconciliation/2026-11-23/data/trace-edges.csv
```

# DAF enterprise simplification, economic engineering and theoretical validation — 2026-11-24

**What this is.** An adversarial examination of the Unbound DAF operating model and its requirements. The mission was to design the business first, then refine the software requirements to support it.

**Status.**
- Requirements and theory only. No production code was written.
- All refinements and new requirements are **proposed**. Original requirements and adopted sections are unchanged.
- Economics come from an executed, reproducible **theoretical** model.
- Every pass, including the adversarial one, used the same AI model family. This is **not** independent validation.

**Start here:**
1. [23-EXECUTIVE-FINDINGS.md](23-EXECUTIVE-FINDINGS.md) answers the twelve strategic questions.
2. [22-ADVERSARIAL-REVIEW.md](22-ADVERSARIAL-REVIEW.md) covers passes A–H and the revisions they forced.
3. [24-DECISION-TEAMS-AND-PRICING.md](24-DECISION-TEAMS-AND-PRICING.md) applies the owner's decision on SPD-ADV-001. Its figures supersede the pooled shared-platform (OM3–OM5) figures in reports 06–10 and 23.
3. [00-MISSION-AND-SOURCE-BASELINE.md](00-MISSION-AND-SOURCE-BASELINE.md) records the sources, revisions and the 1,487-record review universe.

## Deliverables

| # | Report | Data |
|---|---|---|
| 00 | [Mission and source baseline](00-MISSION-AND-SOURCE-BASELINE.md) | `second-site-accounting.csv`, `external-ids.csv` |
| 01 | [Reverse requirements analysis](01-REVERSE-REQUIREMENTS-ANALYSIS.md) | `register.jsonl` |
| 02 | [Requirement simplification matrix](02-REQUIREMENT-SIMPLIFICATION-MATRIX.md) | `register.jsonl` |
| 03 | [Enterprise work inventory](03-ENTERPRISE-WORK-INVENTORY.md) | `work-inventory.jsonl` |
| 04 | [Minimum-touch operating model](04-MINIMUM-TOUCH-OPERATING-MODEL.md) | — |
| 05 | [Human-intervention taxonomy](05-HUMAN-INTERVENTION-TAXONOMY.md) | — |
| 06 | [Economic target engineering](06-ECONOMIC-TARGET-ENGINEERING.md) | `model/` |
| 07 | [Theoretical cost floor](07-THEORETICAL-COST-FLOOR.md) | `model/results.json` |
| 08 | [Enterprise simulation specification](08-ENTERPRISE-SIMULATION-SPECIFICATION.md) | `model/model.py`, `model/parameters.json` |
| 09 | [Simulation scenarios and results](09-SIMULATION-SCENARIOS-AND-RESULTS.md) | `model/results.json`, `results.csv` |
| 10 | [Organizational and staffing models](10-ORGANIZATIONAL-AND-STAFFING-MODELS.md) | `roles.jsonl` |
| 11 | [Customer friction analysis](11-CUSTOMER-FRICTION-ANALYSIS.md) | `interactions.jsonl` |
| 12 | [Configuration simplification](12-CONFIGURATION-SIMPLIFICATION.md) | `config-items.jsonl` |
| 13 | [Dashboard and reporting requirements](13-DASHBOARD-AND-REPORTING-REQUIREMENTS.md) | `metrics.jsonl` |
| 14 | [Competitive differentiation](14-COMPETITIVE-DIFFERENTIATION.md) | `comparison.jsonl` |
| 15 | [Clarity evaluation](15-CLARITY-EVALUATION.md) | `clarity-results.jsonl` |
| 16 | [EDF evaluation](16-EDF-EVALUATION.md) | `edf-results.jsonl` |
| 17 | [Architectural implications](17-ARCHITECTURAL-IMPLICATIONS.md) | — |
| 18 | [Refined requirements](18-REFINED-REQUIREMENTS.md) | `gap-requirements.jsonl`, `refinements.jsonl`, `scenarios.csv`, `findings.csv` |
| 19 | [Rejected approaches](19-REJECTED-APPROACHES.md) | `rejected.jsonl` |
| 20 | [Open decisions and evidence gaps](20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md) | `decisions.csv` |
| 21 | [Research transfer candidates](21-RESEARCH-TRANSFER-CANDIDATES.md) | `transfer.jsonl` |
| 22 | [Adversarial review](22-ADVERSARIAL-REVIEW.md) | `disposition-challenges.jsonl` |
| 23 | [Executive findings](23-EXECUTIVE-FINDINGS.md) | — |
| 24 | [Per-sponsor decision teams and pricing](24-DECISION-TEAMS-AND-PRICING.md): owner decision SPD-ADV-001 applied | `model/decision_teams.py`, `model/decision-teams-results.json` |
| — | [Ledger](LEDGER.md) | `validation-report.json` |

Reports 01–22 are **generated** by `tools/reqaudit/assemble_simplification.py` from `data/`. Never edit them by hand.

## Reproduce and validate

```bash
python3 -I -B tools/reqaudit/graph.py requirements/discovery/2026-11-24-simplification-economics/data
python3 -I -B tools/reqaudit/validate_simplification.py .      # G1-G9, D2/D3/D5/D7/D8, S1-S7
python3 -I -B tools/reqaudit/assemble_simplification.py .      # regenerate reports 01-22
python3 -I -B tools/reqaudit/validate_discovery.py .           # previous mission still valid
python3 -I -B tools/reqaudit/validate.py .                     # reconciliation still valid
cd requirements/discovery/2026-11-24-simplification-economics/model && python3 -I -B model.py && sha256sum -c hashes.txt
```

The new mission-specific gates are:

| Gate | Checks |
|---|---|
| S1 | Every record in the review universe is reviewed exactly once |
| S2/S3 | Enumerated register vocabularies; removals justified |
| S4 | Every kept metric names the decision it supports and the action it prompts |
| S5 | Inventory, configuration, interaction, competition and transfer rows are complete; transfer candidates are independent of Unbound |
| S6 | Every identifier cited in the reports resolves |
| S7 | Every consolidation reaches a retained carrier |

A passing validator does not establish regulatory correctness, economic viability or implementation readiness.

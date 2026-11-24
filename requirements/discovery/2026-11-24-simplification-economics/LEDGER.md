# Ledger: simplification, economic engineering and theoretical validation (2026-11-24)

**Branch:** `claude/daf-economics-event-architecture-v1zdti`, restarted from `main` @ `5c04c42`.

## Resume point

1. Read [00](00-MISSION-AND-SOURCE-BASELINE.md).
2. Agent outputs are integrated into `data/` and the numbered reports.
3. Run the validator:

```bash
python3 -I -B tools/reqaudit/graph.py requirements/discovery/2026-11-24-simplification-economics/data
python3 -I -B tools/reqaudit/validate_simplification.py .
```

## Passes

| Pass | Scope | Status |
|---|---|---|
| 0 | Baseline, source revisions, review universe (1,473 records), validator `validate_simplification.py` | done |
| 1–4 | Reverse requirements analysis: finance (342), grants, charities and payments (370), platform and governance (379), enterprise administration (382); plus integrator review of 14 distinct second definition sites | done |
| 5 | Work inventory (143), minimum-touch model, human-intervention taxonomy, staffing (24 roles) | done |
| 6 | Economic targets, cost floor, revenue models, executed simulation model (seed 20261124; integrator re-run byte-identical) | done |
| 7 | Customer friction (90 interactions), configuration (120 items), dashboards (84 metrics) | done |
| 8 | Competitive differentiation (114 rows), research transfer (16 candidates) | done |
| 9 | Clarity and EDF evaluation (frameworks attached read-only at 200a9eb and 710d54c), architecture | done |
| 10 | Adversarial passes A–H (28 findings, 32 disposition challenges, executed model variants) | done |
| 11 | Integrator: applied adversarial revisions (scripted, idempotent; prior values kept as `*_as_proposed` with `adversarial_revision`); added gate S7; assembled reports; wrote 00, 23, README | done |

## Integrator fixups and revisions

| Change | Reason |
|---|---|
| 14 register rows for distinct second definition sites; 168 mirrors accounted for in `second-site-accounting.csv` | Gate S1 universe was keyed on first sites; PR #3 renamed colliding IDs (commit 8f4860c) |
| `external-ids.csv` (48 Forma, Folio and dependency gap IDs, each with its defining file) | Agents cited Echelon component gaps defined outside the requirement corpus |
| SPD-CLR-005 added; SPF-CLR-006, SPF-CLR-007 and SPF-EDF-001 linked to it | Method findings affect no requirement record |
| Duplicate refinement ID RR-REQ-X-007-r2 renumbered | Both the grants pass and the operations pass refined REQ-X-007; the operations pass's version is now RR-REQ-X-007-r3 |
| Adversarial revisions (`fixups3`): survivors for two consolidation cycles and the deferred-carrier cases; four refinements made owner-decision; sanctions reuse limited to the deciding sponsor; three interaction recommendations made risk-triggered; five decision recommendations revised; duplicate $258 findings merged; seven new requirements flagged for merge, rejection or deferral | Report 22 |

The scripts for these changes live in the session scratch area and are **not** in the repository. The merged `data/` is the canonical record, and every change is visible in it through `*_as_proposed`, `adversarial_revision` and `adversarial_disposition`.

## Owner decisions after the mission (2026-11-24)

The product owner made two decisions:
- **SPD-ADV-001:** no team may decide for more than one sponsor; each sponsor decides with its own people.
- **SPD-ADV-004:** Unbound is a software platform only for now, with hooks for staffed services later.

An intermediate version priced an operator-dedicated decision team (commit 6f7bc7b). SPD-ADV-004 deferred that option, and at the owner's request its economics are kept in report 24 section 6 as a future option. The integrating session made these changes:

| Change | Detail |
|---|---|
| Model | `model/decision_teams.py` imports `model.py` unchanged, asserts that it reproduces `model.py`'s pooled figure in every run, and reassigns cost lines to the vendor (Unbound) or the customer. It was run twice with byte-identical output. |
| Model outputs | `decision-teams-results.json`, `decision-teams-results.csv`, `decision-teams-run.log`; hashes in `hashes.txt` |
| Decisions | SPD-ADV-001, SPD-ECN-003 and the new SPD-ADV-004 are `decided-provisional`. SPD-ECN-001's recommendation points to report 24. |
| New records | UNB-SMP-ORG-003, UNB-SMP-ECN-004, SPF-ECN-015, SPS-ORG-008, SPS-ORG-009, SPS-ECN-007, SPR-ADV-008, SPR-ADV-009, and the trace edges for them |
| Reports | Report 24 written by hand. Reports 18, 19, 20 and 22 regenerated. Report 20 distinguishes owner decisions from open ones. Report 23 updated. |
| Correction | Report 23 question 7 cited SPD-ADV-001 for the small-balance channel; it now cites XID-ADV-002 only. |

## Validation (last run)

See `data/validation-report.json`: **0 errors and 3 warnings**. The warnings are S7 notices for obligations carried by deferred requirements, which are deferred with their capability. The validators of the two earlier missions also pass; see the PR description.

## Telemetry

| Pass | Sub-agent tokens (harness-reported) |
|---|---:|
| Reverse analysis: finance | 339,023 |
| Reverse analysis: grants, charities, payments | 311,227 |
| Reverse analysis: platform and governance | 268,352 |
| Reverse analysis: enterprise administration | 391,206 |
| Work inventory and staffing | 336,333 |
| Economics and simulation | 379,180 |
| Friction, configuration and dashboards | 405,989 |
| Competitive and research transfer | 350,015 |
| Clarity, EDF and architecture | 320,633 |
| Adversarial passes A–H | 314,233 |
| **Total** | **3,416,191** |

The integrating session's own token use is not reported. There was no human intervention during the run.

## Limits and resumption

- **Assumptions.** Every hour, interaction, setting and most economic parameters are assumptions.
- **Template text.** Register purpose and consequence fields are partly templated (SPF-ADV-013).
- **No measurement or professional review.** No measurement, counsel, CPA or auditor review exists.
- **Resume point.** Report 23, "Decisions needed first". The next useful work needs measured data (shadow replay, UNB-LSGAP-001; specialist-pool time study) or owner and counsel decisions.

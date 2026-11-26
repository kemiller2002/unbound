# Charity network and onboarding requirements discovery (2026-11-26)

Research: kemiller2002/daf-landscape, `research/competitive-advantage/charity-network-and-onboarding/`. Start with its `00-executive-summary.md`, and see `22-adversarial-review.md` and `23-strategic-recommendations.md`.

Source revision assessed: `e2f6385` (main, 2026-11-26). Status: **proposed**.

- Nothing here amends the authoritative baseline, a convergence clause, or the generated `AUTHORITATIVE-BASELINE.md`.
- No requirement is implemented or validated.
- There is no application code.

## Key reconciliation findings

Much of the mission's ask is **already governed** by owner-ratified clauses. This package refines them rather than restating them.

| Ask | Already governed by | What this package adds |
|---|---|---|
| First-party charity registry, provenance, no silent overwrite | UNB-CHAR-DEC-001, UNB-CHAR-001..042, UNB-KNOW-*, UNB-EXT-* | Three time axes (UNB-CNET-001); per-predicate authority (002); release-based ingestion and freshness (003, 004); conflict states (005); subordinate evidence (006) |
| 180-day vetting, event-driven invalidation, fail safe | CNV-GRT-003, RR-REQ-X-007-r2, RR-UNB-EXT-028-r2 | Materiality by field and direction (007); reinstatement semantics (008); sanctions polling (009); the reading of "stale" (004, OD-CNO-04) |
| Shared public facts vs private findings | CNV-GRT-004, CNV-TEN-001 | Universe-wide monitoring without portfolio inference (010); licence segregation (011); cross-sponsor bank evidence deferred (OD-CNO-05) |
| Payment rails, destination evidence | CNV-PAY-001/002 (with the c10 amendment), CNV-PLT-012, CNV-EXP-002 (c10), CNV-PLT-011 (c11) | Rail-class taxonomy (013); evidence separated from sponsor acceptance (014); risk tiers and caps (015); legacy-imported destinations (016); owner-match port (017); partner contract minimums (018); second rail (026); Nacha programme (023) |
| Migration import with reconciliation | CNV-EXP-002, CNV-ENT-002, RR-REQ-X-061-r1 (deferred in the simplification scope) | Migration classes (UNB-ONB-001); extraction plan (002); canonical package, legacy partition and epoch (006-009); deterministic rehearsals (010-012); reconciliation gates (013); cutover and irreversibility (016-019) |
| Configuration, no forks, integration readiness | REQ-X-045, CNV-GOV-005 (OD-P-GOV cut I), RR-REQ-X-057-r1, CNV-ECO-005 | One Sponsor Configuration Package (023); extension-point catalogue (024); certification lifecycle (025, 026) |

**Four conflicts with ratified text** are escalated as owner decisions and are not resolved here:

- **OD-CNO-01:** migration in Release 1.
- **OD-CNO-02:** a third destination-evidence class for partner-bank accounts.
- **OD-CNO-07:** CNV-EXP-002 scope and "full history".
- **OD-CNO-08:** the rollback boundary.

## Contents

| File | Content |
|---|---|
| [01-TRACEABILITY.md](01-TRACEABILITY.md) | Forward trace (requirement → refined IDs → research) and reverse trace (generated) |
| [02-REQUIREMENTS.md](02-REQUIREMENTS.md) | 26 UNB-CNET and 33 UNB-ONB proposed requirements. Each has an ID, business rationale, statement, supporting research, Given/When/Then acceptance, dependencies, security implications, failure behaviour, verification method and the existing requirements it refines (generated) |
| [03-ECHELON-DEPENDENCIES-SECURITY-AND-COMPLIANCE.md](03-ECHELON-DEPENDENCIES-SECURITY-AND-COMPLIANCE.md) | Echelon component dependencies and gaps; security and compliance implications |
| [04-DECISIONS-AND-OPEN-QUESTIONS.md](04-DECISIONS-AND-OPEN-QUESTIONS.md) | 13 owner decisions (OD-CNO-01..13) and 14 requirement questions, registered in `requirements/OPEN-QUESTIONS.md` (generated) |
| [05-SIMULATION-SCENARIOS.md](05-SIMULATION-SCENARIOS.md) | Future simulation scenarios, in the format of the whole-enterprise simulation framework |
| `data/gap-requirements.jsonl` | Source of truth for the requirements |
| `data/decisions.csv` | Source of truth for the owner decisions |

## Question register integration

- This package is registered as a source of the open-question register. It was added to `QuestionsLib.MISSIONS` and `RenderQuestions.SOURCES`.
- `dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .` now yields 343 questions. The 27 new ones (13 decisions and 14 requirement questions) are open and not approved.
- The regression test `tools/fs/Unbound.Convergence.Tests` was updated to that count. It asserts that all 316 earlier questions remain answered and all 27 new ones are open.
- No resolution events were appended.

## Validation

```bash
python3 -I -B tools/reqaudit/validate_charity_onboarding.py . --check --research-root ../daf-landscape   # gates C1-C8
dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .                     # register (no diff expected)
dotnet test tools/fs/Unbound.Convergence.Tests
```

The validator is pure-function Python, following the precedent of `validate_availability.py`. It checks:

- ID form and uniqueness;
- all mandated fields are present;
- every cited requirement resolves against the baseline or repository definitions;
- Given/When/Then acceptance;
- status is `proposed / not implemented / not validated`;
- every research citation exists in the daf-landscape checkout;
- decision integrity;
- the generated sections are current.

Existing validators were re-run and are unchanged by this package. `ReqAudit validate`, `validate-simplification` and `validate_availability.py` pass. `validate-discovery` reports two errors, and both reproduce on the base commit without this change:

- **D8** requires a clean daf-landscape checkout.
- **D9** is the unknown `RR-ADM-023-r2` in the cross-industry report.

## Not done

- **No adoption into the baseline and no convergence clause.** Both require owner ratification.
- **No implementation and no validation claim.**
- **No changes to Echelon repositories.** Needs found in them are listed in 03.

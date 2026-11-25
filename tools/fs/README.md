# Unbound tooling (F#)

All repository tooling is F#: one language, functional style (pure functions over immutable data; file and console I/O
only at each program's edge). The Python tools were ported on 2026-11-25. Each port was accepted only when it produced
**byte-identical** files, stdout and exit codes to the Python on the same commit. That covered every documented
command, deliberately broken inputs for the validators, and changed parameters for the economics models. The last commit
containing the Python is `e32819f`.

Requires the .NET 10 SDK (`global.json`). Run any tool with `dotnet run --project tools/fs/<Project> -c Release -- <subcommand> <args>`
from the repository root, unless noted.

| Project | Purpose | Subcommands |
|---|---|---|
| `Unbound.Py` | CPython-exact primitives used by the ports: float `repr`/format/`round`, compensated `sum`, `json`, `csv`, `random.Random` (MT19937, `gauss`, `triangular`), `zlib.crc32`, code-point ordering. Tested against CPython 3.13 golden values (`Unbound.Py.Tests/golden.json`). | (library) |
| `Unbound.Convergence` | Question register, evidence dossiers, convergence clauses and baseline, decision and adversarial registers (master mission) | `render-questions`, `review-map`, `integrate-dossiers`, `adopt-convergence`, `render-decision-register`, `render-adversarial`, `render-ratification-packets` |
| `Unbound.ReqAudit` | Reconciliation and discovery: inventory, graph, validators, generated tables and reports | `inventory`, `quality`, `narrative`, `graph`, `render`, `validate`, `workflows`, `ia`, `uiclass`, `adopt`, `validate-discovery`, `render-discovery`, `validate-simplification`, `assemble-simplification`, `benchmark-analyse` |
| `Unbound.Economics` | Simplification Monte Carlo model, decision teams, enterprise economics, report 05a tables | `model`, `decision-teams` (run from the simplification `model/` folder), `enterprise-economics`, `render-tables` |
| `Unbound.Authoring` | UI requirement authoring steps (reconciliation 2026-11-23) | `ui-quality`, `merge-specs`, `ui-trace` (each takes the repository root) |

Each project has a `*.Tests` project: `dotnet test tools/fs/<Project>.Tests`. The public-data replay and synthetic
operating year live with their report, in `requirements/convergence/2026-11-24-master/replay/`.

## Script to command map

| Former script | Now |
|---|---|
| `tools/reqaudit/render_questions.py .` | `Unbound.Convergence render-questions .` (keeps `--allow-discard`) |
| `review_map.py`, `integrate_dossiers.py`, `adopt_convergence.py`, `render_decision_register.py`, `render_adversarial.py`, `render_ratification_packets.py` | the `Unbound.Convergence` subcommand with the same name in kebab case and the same arguments |
| `questions_lib.py` | `Unbound.Convergence/QuestionsLib.fs` |
| `test_question_register.py` | `dotnet test tools/fs/Unbound.Convergence.Tests` |
| `requirements/convergence/2026-11-24-master/clauses/*.py` | `clauses/*.json` (same names) plus `clauses/manifest.json` giving the module order. **Edit clauses in JSON.** |
| `inventory.py`, `quality.py`, `narrative.py`, `graph.py`, `render.py`, `validate.py`, `workflows.py`, `ia.py`, `uiclass.py`, `adopt.py`, `validate_discovery.py`, `render_discovery.py`, `validate_simplification.py`, `assemble_simplification.py` | the `Unbound.ReqAudit` subcommand with the same name in kebab case and the same arguments |
| `requirements/discovery/2026-11-23-cross-industry/benchmark/analyse.py <dir>` | `Unbound.ReqAudit benchmark-analyse <dir>` |
| `model/model.py`, `model/decision_teams.py` (simplification) | `Unbound.Economics model`, `decision-teams`, run from that `model/` folder; `hashes.txt` now covers `Model.fs` and `DecisionTeams.fs` |
| `model/enterprise_economics.py`, `model/render_tables.py` (master mission) | `Unbound.Economics enterprise-economics`, `render-tables` |
| `tools/reqaudit/authoring/ui_quality.py`, `merge_specs.py`, `ui_trace.py` | `Unbound.Authoring ui-quality`, `merge-specs`, `ui-trace`; inputs moved to `Unbound.Authoring/specs/` |

## Retired, not ported

These scripts are in git history at `e32819f`.

| Script | Why |
|---|---|
| `tools/reqaudit/seed_resolutions.py` | One-time seed of `requirements/questions/resolution-events.jsonl`; it refuses to run once the log exists. |
| `tools/reqaudit/apply_revisions_2026_11_24.py` | One-off migration of the 2026-11-24 revisions; already applied (re-running it changes nothing). |
| `tools/reqaudit/authoring/ui_build.py` | Superseded first-pass UI records. Its own instructions say not to re-run it, because it rewrites `ui-requirements.jsonl`. |
| `tools/reqaudit/authoring/decide.py`, `wf_decide.py` | Earlier decision-authoring steps. They no longer run against today's data (a missing record, and a scratch input that was never preserved). The committed data is the source of truth. |

## Known differences from the Python

None of these occur on the repository's data today.

- **Unhandled-exception output.** When a Python tool crashed with a traceback, the F# tool exits with the same code, prints the same stdout and writes the same files, but its stderr is not a CPython traceback.
- **Hash-ordered sets.** A few messages came from iterating a Python `set`, whose order changes between runs: G2 collisions, W1 duplicate transitions, W7 effects and duplicate evidence ids. The F# reports these in a stable order.
- **Unicode corner cases.** These involve Unicode text not present in the data: characters outside the Basic Multilingual Plane in word classes, and invalid UTF-8 (replaced where Python raised).

## Other languages still in the repository

The UI design renderer in `requirements/ui/designs/tools` and `requirements/ui/designs/v2/tools` is JavaScript (`.mjs`,
about 5,900 lines). It renders the mock-ups and takes browser screenshots. `requirements/ui/designs/vendor/folio-0.4.2`
is a vendored library. Neither has been ported. The PostgreSQL benchmark in
`requirements/discovery/2026-11-23-cross-industry/benchmark` is SQL run by `pgbench` from small Bash scripts.

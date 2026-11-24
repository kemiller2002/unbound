# Authoring scripts (reconciliation 2026-11-23)

These scripts write the hand-authored records in `requirements/reconciliation/2026-11-23/data/`. They were run from the session scratchpad and are kept here for reproducibility. The paths at the top of each script are absolute: `DATA`, `SP`/`AUDIT`, `V2`. Adjust them before re-running. `specs/` holds the scratchpad inputs: the panel and view specs, and the v1 audit outputs.

Run order for the UI records, each step idempotent: `ui_build.py` (first-pass records; overwrites the file), then `ui_quality.py`, `merge_specs.py`, `ui_trace.py`. Then run `tools/reqaudit/graph.py`, `render.py` and `validate.py`.

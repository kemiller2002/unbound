#!/usr/bin/env python3
"""Quality gates and renderer for the availability-architecture mission
(requirements/discovery/2026-11-25-availability-architecture).

Every check is a pure function returning a tuple of issue strings; main() is the only function with effects.
References resolve against the authoritative baseline (requirements/baseline/baseline.jsonl) and this
mission's own UNB-AVL records.

Gates:
  V1  operation and requirement IDs are unique and well formed (OPC-nnn, UNB-AVL-nnn)
  V2  every operation row has every matrix field, non-empty
  V3  every cited Unbound requirement ID resolves (baseline or this mission)
  V4  execution models use the INIT-012 request classes; ImmediateControl rows are never queue-eligible
  V5  every UNB-AVL requirement has the normative fields, Given/When/Then acceptance, priority P0-P2,
      status 'proposed', and a resolvable parent (or none, for new requirements from gap analysis)
  V6  the generated Markdown sections are current (only with --check)

Usage:
  python3 -I -B tools/reqaudit/validate_availability.py .           validate + rewrite generated sections
  python3 -I -B tools/reqaudit/validate_availability.py . --check   validate + fail if generated sections are stale
"""
from __future__ import annotations

import json
import re
import sys
from functools import reduce
from pathlib import Path
from typing import Callable, Iterable, Mapping, Sequence, Tuple

MISSION = Path("requirements/discovery/2026-11-25-availability-architecture")
BASELINE = Path("requirements/baseline/baseline.jsonl")
OPERATIONS = MISSION / "data/operation-classification.jsonl"
REQUIREMENTS = MISSION / "data/gap-requirements.jsonl"
MATRIX_MD = MISSION / "01-OPERATION-CLASSIFICATION-MATRIX.md"
REQUIREMENTS_MD = MISSION / "02-REQUIREMENTS.md"
BEGIN, END = "<!-- BEGIN GENERATED -->", "<!-- END GENERATED -->"

OPERATION_FIELDS = ("id", "operation", "requirements", "execution_model", "consistency", "ordering_boundary",
                    "idempotency", "authorization_timing", "queue_eligibility", "cache_eligibility",
                    "failure_behavior", "recovery_behavior", "audit", "user_status", "acceptance")
REQUIREMENT_FIELDS = ("id", "origin", "statement", "rationale", "source", "applicability", "responsible_system",
                      "acceptance", "verification", "priority", "status", "implementation_status", "release_scope")
CLASSES = ("ImmediateQuery", "ImmediateControl", "ImmediateCommand", "DurableCommand", "ScheduledCommand",
           "LongRunningOperation", "HumanApprovalWorkflow", "ExternalIntegrationOperation")
OPC_ID = re.compile(r"^OPC-\d{3}$")
AVL_ID = re.compile(r"^UNB-AVL-\d{3}$")
GWT = re.compile(r"\bGiven\b.*\bWhen\b.*\bThen\b", re.S)

Row = Mapping[str, object]
Issues = Tuple[str, ...]


def read_jsonl(path: Path) -> Tuple[Row, ...]:
    return tuple(json.loads(line) for line in path.read_text(encoding="utf-8").splitlines() if line.strip())


def duplicates(ids: Iterable[str]) -> Tuple[str, ...]:
    seen = reduce(lambda acc, i: {**acc, i: acc.get(i, 0) + 1}, ids, {})
    return tuple(sorted(i for i, n in seen.items() if n > 1))


def gate_ids(ops: Sequence[Row], reqs: Sequence[Row]) -> Issues:
    malformed = tuple(f"V1 malformed operation id {o['id']}" for o in ops if not OPC_ID.match(str(o["id"]))) + \
        tuple(f"V1 malformed requirement id {r['id']}" for r in reqs if not AVL_ID.match(str(r["id"])))
    dups = tuple(f"V1 duplicate id {i}" for i in duplicates([str(x["id"]) for x in (*ops, *reqs)]))
    return malformed + dups


def gate_operation_fields(ops: Sequence[Row]) -> Issues:
    return tuple(f"V2 {o.get('id')} missing or empty field {f}"
                 for o in ops for f in OPERATION_FIELDS if not o.get(f))


def gate_references(ops: Sequence[Row], reqs: Sequence[Row], known: frozenset) -> Issues:
    cited = [(o["id"], ref) for o in ops for ref in o["requirements"]] + \
            [(r["id"], r["parent"]) for r in reqs if r.get("parent")]
    return tuple(f"V3 {owner} cites unknown requirement {ref}" for owner, ref in cited if ref not in known)


def gate_classes(ops: Sequence[Row]) -> Issues:
    named = lambda o: [c for c in CLASSES if c in str(o["execution_model"])]
    unclassified = tuple(f"V4 {o['id']} execution model names no request class" for o in ops if not named(o))
    queued_controls = tuple(
        f"V4 {o['id']} is ImmediateControl but queue eligibility is '{o['queue_eligibility']}'"
        for o in ops
        if str(o["execution_model"]).startswith("ImmediateControl")
        and not str(o["queue_eligibility"]).lower().startswith(("never", "no")))
    return unclassified + queued_controls


def gate_requirements(reqs: Sequence[Row]) -> Issues:
    checks: Tuple[Tuple[str, Callable[[Row], bool]], ...] = (
        ("missing normative field", lambda r: all(r.get(f) for f in REQUIREMENT_FIELDS)),
        ("acceptance is not Given/When/Then", lambda r: bool(GWT.search(str(r.get("acceptance", ""))))),
        ("priority not P0-P2", lambda r: r.get("priority") in ("P0", "P1", "P2")),
        ("status must be 'proposed'", lambda r: r.get("status") == "proposed"),
        ("new requirement without parent must come from gap analysis",
         lambda r: bool(r.get("parent")) or r.get("origin") == "gap-analysis"),
    )
    return tuple(f"V5 {r.get('id')} {label}" for r in reqs for label, ok in checks if not ok(r))


def cell(value: object) -> str:
    text = " ".join(value) if isinstance(value, (list, tuple)) else ("—" if value is None else str(value))
    return text.replace("|", "\\|").replace("\n", " ")


def table(headers: Sequence[str], rows: Iterable[Sequence[object]]) -> str:
    head = "| " + " | ".join(headers) + " |\n|" + "---|" * len(headers) + "\n"
    return head + "".join("| " + " | ".join(cell(c) for c in row) + " |\n" for row in rows)


def render_matrix(ops: Sequence[Row]) -> str:
    headers = ("ID", "Operation", "Original requirements", "Execution model", "Consistency", "Ordering boundary",
               "Idempotency", "Authorization timing", "Queue eligibility", "Future cache eligibility",
               "Failure behaviour", "Recovery behaviour", "Audit", "User-visible status", "Acceptance criteria")
    return table(headers, ([o[f] for f in OPERATION_FIELDS] for o in ops))


def render_requirements(reqs: Sequence[Row]) -> str:
    block = lambda r: (
        f"### {r['id']}\n\n"
        + table(("Field", "Value"), (
            ("Statement", r["statement"]), ("Rationale", r["rationale"]),
            ("Refines / parent", r.get("parent") or "— (new requirement)"), ("Source", r["source"]),
            ("Applicability", r["applicability"]), ("Responsible system", r["responsible_system"]),
            ("Dependencies", r.get("dependencies") or "—"), ("Echelon requirements", r.get("echelon_requirements") or "—"),
            ("Acceptance criteria", r["acceptance"]), ("Verification", r["verification"]),
            ("Priority", r["priority"]), ("Status", f"{r['status']} · {r['implementation_status']}"),
            ("Release scope", r["release_scope"]))))
    return "\n".join(block(r) for r in reqs)


def splice(document: str, generated: str) -> str:
    start, end = document.index(BEGIN) + len(BEGIN), document.index(END)
    return document[:start] + "\n" + generated + document[end:]


def main(argv: Sequence[str]) -> int:
    root = Path(argv[1]) if len(argv) > 1 else Path(".")
    check = "--check" in argv
    ops, reqs = read_jsonl(root / OPERATIONS), read_jsonl(root / REQUIREMENTS)
    known = frozenset(str(r["id"]) for r in read_jsonl(root / BASELINE)) | frozenset(str(r["id"]) for r in reqs)
    issues = (gate_ids(ops, reqs) + gate_operation_fields(ops) + gate_references(ops, reqs, known)
              + gate_classes(ops) + gate_requirements(reqs))
    targets = ((root / MATRIX_MD, render_matrix(ops)), (root / REQUIREMENTS_MD, render_requirements(reqs)))
    rendered = tuple((path, path.read_text(encoding="utf-8"), splice(path.read_text(encoding="utf-8"), body))
                     for path, body in targets)
    stale = tuple(f"V6 {path} generated section is stale; rerun without --check"
                  for path, before, after in rendered if before != after)
    if not check:
        tuple(path.write_text(after, encoding="utf-8") for path, before, after in rendered if before != after)
    errors = issues + (stale if check else ())
    print(json.dumps({"operations": len(ops), "requirements": len(reqs), "errors": list(errors)}, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

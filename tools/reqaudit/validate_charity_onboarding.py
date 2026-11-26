#!/usr/bin/env python3
"""Quality gates and renderer for the charity-network and onboarding discovery package
(requirements/discovery/2026-11-26-charity-network-and-onboarding).

Every check is a pure function returning a tuple of issue strings; main() is the only function with effects.
References resolve against the authoritative baseline (requirements/baseline/baseline.jsonl), this package's
own records, and identifiers defined in repository text that the baseline does not index (e.g. UNB-CHAR-DEC-001).

Gates:
  C1  requirement and decision IDs are unique and well formed (UNB-CNET-nnn, UNB-ONB-nnn, OD-CNO-nn)
  C2  every requirement has every mandated field, non-empty (statement, rationale, supporting research,
      acceptance, security implications, failure behaviour, verification, refines, priority, release scope)
  C3  every cited requirement ID (parent, refines, dependencies, decision affected_ids) resolves
  C4  acceptance is Given/When/Then; priority P0-P2; status 'proposed'; implementation 'not implemented';
      validation 'not validated' (documentation never implies implementation or validation)
  C5  a requirement without a parent comes from gap analysis; a refinement names its parent in 'refines'
  C6  every supporting-research path exists in the daf-landscape checkout (only with --research-root)
  C7  every decision is open, names an owner and a recommendation, and every requirement question or
      conflict that cites an OD-CNO decision cites one that exists
  C8  the generated Markdown sections are current (only with --check)

Usage:
  python3 -I -B tools/reqaudit/validate_charity_onboarding.py .                 validate + rewrite generated sections
  python3 -I -B tools/reqaudit/validate_charity_onboarding.py . --check         validate + fail if generated sections are stale
  add --research-root ../daf-landscape to verify research citations
"""
from __future__ import annotations

import csv
import io
import json
import re
import sys
from functools import reduce
from pathlib import Path
from typing import Callable, Iterable, Mapping, Optional, Sequence, Tuple

MISSION = Path("requirements/discovery/2026-11-26-charity-network-and-onboarding")
BASELINE = Path("requirements/baseline/baseline.jsonl")
REQUIREMENTS = MISSION / "data/gap-requirements.jsonl"
DECISIONS = MISSION / "data/decisions.csv"
REQUIREMENTS_MD = MISSION / "02-REQUIREMENTS.md"
TRACE_MD = MISSION / "01-TRACEABILITY.md"
DECISIONS_MD = MISSION / "04-DECISIONS-AND-OPEN-QUESTIONS.md"
BEGIN, END = "<!-- BEGIN GENERATED -->", "<!-- END GENERATED -->"
RESEARCH_PREFIX = "daf-landscape:"

REQUIRED = ("id", "origin", "title", "statement", "rationale", "supporting_research", "acceptance",
            "security_implications", "failure_behavior", "verification", "refines", "priority", "status",
            "implementation_status", "validation_status", "release_scope")
REQ_ID = re.compile(r"^UNB-(CNET|ONB)-\d{3}$")
DEC_ID = re.compile(r"^OD-CNO-\d{2}$")
OD_REF = re.compile(r"OD-CNO-\d{2}")
GWT = re.compile(r"\bGiven\b.*\bWhen\b.*\bThen\b", re.S)
SCOPES = ("initial-implementation", "sponsor-activation", "commercial-availability", "architectural-support",
          "regulatory-authorization", "deferred")

Row = Mapping[str, object]
Issues = Tuple[str, ...]


def read_jsonl(path: Path) -> Tuple[Row, ...]:
    return tuple(json.loads(line) for line in path.read_text(encoding="utf-8").splitlines() if line.strip())


def read_csv(path: Path) -> Tuple[Row, ...]:
    return tuple(csv.DictReader(io.StringIO(path.read_text(encoding="utf-8"))))


def duplicates(ids: Iterable[str]) -> Tuple[str, ...]:
    seen = reduce(lambda acc, i: {**acc, i: acc.get(i, 0) + 1}, ids, {})
    return tuple(sorted(i for i, n in seen.items() if n > 1))


def split_ids(value: str) -> Tuple[str, ...]:
    return tuple(x.strip() for x in value.split(";") if x.strip())


def repo_defined_ids(root: Path, candidates: frozenset) -> frozenset:
    """IDs that appear as definitions in repository Markdown (e.g. '**UNB-CHAR-DEC-001:**') but not in the baseline."""
    texts = (p.read_text(encoding="utf-8", errors="replace") for p in (root / "requirements").rglob("*.md")
             if MISSION.as_posix() not in p.as_posix())
    blob = "\n".join(texts)
    return frozenset(c for c in candidates if re.search(r"\*\*" + re.escape(c) + r"[:*]", blob))


def cited_ids(reqs: Sequence[Row], decs: Sequence[Row]) -> Tuple[Tuple[str, str], ...]:
    from_reqs = tuple((str(r["id"]), ref) for r in reqs
                      for ref in (*([r["parent"]] if r.get("parent") else []), *r.get("refines", ()), *r.get("dependencies", ())))
    from_decs = tuple((str(d["decision_id"]), ref) for d in decs for ref in split_ids(str(d.get("affected_ids", ""))))
    return from_reqs + from_decs


def gate_ids(reqs: Sequence[Row], decs: Sequence[Row]) -> Issues:
    bad = tuple(f"C1 malformed requirement id {r['id']}" for r in reqs if not REQ_ID.match(str(r["id"]))) + \
        tuple(f"C1 malformed decision id {d['decision_id']}" for d in decs if not DEC_ID.match(str(d["decision_id"])))
    return bad + tuple(f"C1 duplicate id {i}" for i in
                       duplicates([str(r["id"]) for r in reqs] + [str(d["decision_id"]) for d in decs]))


def gate_fields(reqs: Sequence[Row]) -> Issues:
    return tuple(f"C2 {r.get('id')} missing or empty field {f}" for r in reqs for f in REQUIRED if not r.get(f))


def gate_references(cites: Sequence[Tuple[str, str]], known: frozenset) -> Issues:
    return tuple(f"C3 {owner} cites unknown requirement {ref}" for owner, ref in cites if ref not in known)


def gate_status(reqs: Sequence[Row]) -> Issues:
    checks: Tuple[Tuple[str, Callable[[Row], bool]], ...] = (
        ("acceptance is not Given/When/Then", lambda r: bool(GWT.search(str(r.get("acceptance", ""))))),
        ("priority not P0-P2", lambda r: r.get("priority") in ("P0", "P1", "P2")),
        ("status must be 'proposed'", lambda r: r.get("status") == "proposed"),
        ("implementation_status must be 'not implemented'", lambda r: r.get("implementation_status") == "not implemented"),
        ("validation_status must be 'not validated'", lambda r: r.get("validation_status") == "not validated"),
        ("unknown release scope", lambda r: r.get("release_scope") in SCOPES),
    )
    return tuple(f"C4 {r.get('id')} {label}" for r in reqs for label, ok in checks if not ok(r))


def gate_origin(reqs: Sequence[Row]) -> Issues:
    orphan = tuple(f"C5 {r['id']} has no parent but origin is not gap-analysis"
                   for r in reqs if not r.get("parent") and r.get("origin") != "gap-analysis")
    unlisted = tuple(f"C5 {r['id']} parent {r['parent']} not listed in refines"
                     for r in reqs if r.get("parent") and r["parent"] not in r.get("refines", ()))
    return orphan + unlisted


def gate_research(reqs: Sequence[Row], research_root: Optional[Path]) -> Issues:
    if research_root is None:
        return ()
    paths = ((r["id"], str(p)) for r in reqs for p in r.get("supporting_research", ()))
    return tuple(f"C6 {rid} cites missing research {p}" for rid, p in paths
                 if not (research_root / p.removeprefix(RESEARCH_PREFIX)).exists())


def gate_decisions(reqs: Sequence[Row], decs: Sequence[Row]) -> Issues:
    known = frozenset(str(d["decision_id"]) for d in decs)
    weak = tuple(f"C7 {d['decision_id']} must be open with owner and recommendation" for d in decs
                 if d.get("status") != "open" or not d.get("decision_owner") or not d.get("recommendation"))
    texts = ((r["id"], " ".join((*r.get("open_questions", ()), *r.get("conflicts", ())))) for r in reqs)
    dangling = tuple(f"C7 {rid} cites unknown decision {od}" for rid, t in texts for od in OD_REF.findall(t) if od not in known)
    return weak + dangling


def cell(value: object) -> str:
    text = "; ".join(map(str, value)) if isinstance(value, (list, tuple)) else ("—" if value in (None, "") else str(value))
    return text.replace("|", "\\|").replace("\n", " ")


def table(headers: Sequence[str], rows: Iterable[Sequence[object]]) -> str:
    head = "| " + " | ".join(headers) + " |\n|" + "---|" * len(headers) + "\n"
    return head + "".join("| " + " | ".join(cell(c) for c in row) + " |\n" for row in rows)


def research_links(paths: Sequence[str]) -> str:
    return "; ".join(p.removeprefix(RESEARCH_PREFIX + "research/competitive-advantage/charity-network-and-onboarding/")
                     for p in paths)


def render_requirements(reqs: Sequence[Row]) -> str:
    block = lambda r: (
        f"### {r['id']} — {r['title']}\n\n"
        + table(("Field", "Value"), (
            ("Requirement", r["statement"]), ("Business rationale", r["rationale"]),
            ("Refines / applicable existing requirements", r["refines"]),
            ("Origin", f"{r['origin']}" + (f" (parent {r['parent']})" if r.get("parent") else " (new requirement)")),
            ("Supporting research (daf-landscape package)", research_links(r["supporting_research"])),
            ("Acceptance criteria", r["acceptance"]), ("Dependencies", r.get("dependencies") or "—"),
            ("Echelon dependencies", r.get("echelon_dependencies") or "—"),
            ("Security implications", r["security_implications"]), ("Failure behaviour", r["failure_behavior"]),
            ("Verification method", r["verification"]), ("Priority", r["priority"]),
            ("Status", f"{r['status']} · {r['implementation_status']} · {r['validation_status']}"),
            ("Release scope", r["release_scope"]),
            ("Open questions", r.get("open_questions") or "—"), ("Conflicts", r.get("conflicts") or "—"))))
    return "\n".join(block(r) for r in reqs)


def render_trace(reqs: Sequence[Row]) -> str:
    forward = table(("Requirement", "Title", "Refines", "Research", "Priority", "Scope"),
                    ((r["id"], r["title"], r["refines"], research_links(r["supporting_research"]), r["priority"],
                      r["release_scope"]) for r in reqs))
    reverse_map = reduce(lambda acc, pair: {**acc, pair[0]: (*acc.get(pair[0], ()), pair[1])},
                         ((ref, str(r["id"])) for r in reqs for ref in r["refines"]), {})
    reverse = table(("Existing requirement or clause", "Refined by"),
                    ((k, v) for k, v in sorted(reverse_map.items())))
    return "## Forward trace\n\n" + forward + "\n## Reverse trace (existing → proposed)\n\n" + reverse


def render_decisions(decs: Sequence[Row], reqs: Sequence[Row]) -> str:
    questions = tuple((r["id"], q) for r in reqs for q in r.get("open_questions", ()))
    return ("## Owner decisions (registered as open decisions)\n\n"
            + table(("ID", "Type", "Question", "Affected", "Owner", "Recommendation", "Alternatives", "Blocks", "Reversible"),
                    ((d["decision_id"], d["type"], d["question"], d["affected_ids"], d["decision_owner"],
                      d["recommendation"], d["alternatives"], d["blocks_implementation"], d["reversible"]) for d in decs))
            + "\n## Requirement questions (registered as `<id>#Q<n>`)\n\n"
            + table(("Requirement", "Question"), questions))


def splice(document: str, generated: str) -> str:
    start, end = document.index(BEGIN) + len(BEGIN), document.index(END)
    return document[:start] + "\n" + generated + document[end:]


def main(argv: Sequence[str]) -> int:
    root = Path(argv[1]) if len(argv) > 1 and not argv[1].startswith("--") else Path(".")
    check = "--check" in argv
    research_root = Path(argv[argv.index("--research-root") + 1]) if "--research-root" in argv else None
    reqs, decs = read_jsonl(root / REQUIREMENTS), read_csv(root / DECISIONS)
    cites = cited_ids(reqs, decs)
    base = frozenset(str(r["id"]) for r in read_jsonl(root / BASELINE)) | frozenset(str(r["id"]) for r in reqs)
    unresolved = frozenset(ref for _, ref in cites) - base
    known = base | repo_defined_ids(root, unresolved)
    issues = (gate_ids(reqs, decs) + gate_fields(reqs) + gate_references(cites, known) + gate_status(reqs)
              + gate_origin(reqs) + gate_research(reqs, research_root) + gate_decisions(reqs, decs))
    targets = ((root / REQUIREMENTS_MD, render_requirements(reqs)), (root / TRACE_MD, render_trace(reqs)),
               (root / DECISIONS_MD, render_decisions(decs, reqs)))
    rendered = tuple((path, path.read_text(encoding="utf-8")) for path, _ in targets)
    updated = tuple((path, before, splice(before, body)) for (path, before), (_, body) in zip(rendered, targets))
    stale = tuple(f"C8 {path} generated section is stale; rerun without --check"
                  for path, before, after in updated if before != after)
    if not check:
        tuple(path.write_text(after, encoding="utf-8") for path, before, after in updated if before != after)
    errors = issues + (stale if check else ())
    print(json.dumps({"requirements": len(reqs), "decisions": len(decs),
                      "by_prefix": {p: sum(1 for r in reqs if str(r["id"]).startswith(p)) for p in ("UNB-CNET", "UNB-ONB")},
                      "errors": list(errors)}, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

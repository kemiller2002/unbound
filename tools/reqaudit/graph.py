#!/usr/bin/env python3
"""Derive the bidirectional traceability graph from reconciliation records.

Edges are computed, never hand-maintained, so the graph cannot drift from the records.
Relationship vocabulary (mission section 3):
    refines, supersedes, conflicts_with, duplicates, depends_on, derived_from,
    validated_by, implemented_by (reserved), tested_by
plus 'affects' (finding -> requirement) and 'decided_by' (record -> owner decision).

Usage: python3 -I tools/reqaudit/graph.py <reconciliation-data-dir>
Writes trace-edges.csv and prints edge counts by relationship.
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Iterable, Iterator, Tuple

Edge = Tuple[str, str, str, str]  # source, relationship, target, provenance

ID = re.compile(r"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}(?:-r\d+)?\*?\b|\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-\*")


RANGE = re.compile(r"\b([A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-)([A-Z]?)(\d{1,4})\.\.(?:\1)?\2?(\d{1,4})\b")


def expand_ranges(text: str) -> str:
    """UNB-SUCC-001..014 -> UNB-SUCC-001 UNB-SUCC-002 ... (zero padding preserved)."""
    def rep(m):
        width = len(m.group(3))
        return " ".join(f"{m.group(1)}{m.group(2)}{n:0{width}d}" for n in range(int(m.group(3)), int(m.group(4)) + 1))
    return RANGE.sub(rep, text or "")


def ids_in(text: str) -> Tuple[str, ...]:
    return tuple(dict.fromkeys(m.group(0) for m in ID.finditer(expand_ranges(text))))


def jsonl(path: Path) -> Tuple[dict, ...]:
    return tuple(json.loads(l) for l in path.read_text(encoding="utf-8").splitlines() if l.strip()) if path.exists() else ()


def rows(path: Path) -> Tuple[dict, ...]:
    return tuple(csv.DictReader(path.open(encoding="utf-8"))) if path.exists() else ()


def refinement_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for r in recs:
        rid = r["refinement_id"]
        yield (rid, r.get("relationship", "refines"), r["original_id"], "refinements.jsonl")
        yield from ((rid, "derived_from", f, "refinements.jsonl") for f in r.get("related_findings", ()))
        yield from ((rid, "depends_on", d, "refinements.jsonl") for d in r.get("dependencies", ()) if ID.fullmatch(d))


def gap_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for g in recs:
        yield (g["id"], "derived_from", g["gap_finding"], "gap-requirements.jsonl")
        yield from ((g["id"], "derived_from", p, "gap-requirements.jsonl") for p in ids_in(g.get("parent", "")))
        yield from ((g["id"], "depends_on", l, "gap-requirements.jsonl") for l in g.get("links", ()) if ID.fullmatch(l))


def finding_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for f in recs:
        yield from ((f["finding_id"], "affects", a, "findings.csv") for a in ids_in(f.get("affected_ids", "")))
        yield from ((f["finding_id"], "derived_from", p, "findings.csv") for p in ids_in(f.get("prior_ids", "")))


def conflict_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for c in recs:
        a, b = ids_in(c["requirement_a"]), ids_in(c["requirement_b"])
        yield from ((x, "conflicts_with", y, c["conflict_id"]) for x in a for y in b if x != y)


def duplicate_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for f in recs:
        if f.get("title", "").lower().startswith("duplicate"):
            ids = list(ids_in(f["affected_ids"]))
            yield from ((ids[i], "duplicates", ids[0], f["finding_id"]) for i in range(1, len(ids)))


def scenario_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for s in recs:
        yield from ((r, "tested_by", s["scenario_id"], "scenarios.csv") for r in ids_in(s.get("requirement_ids", "")))


def inherited_test_edges(refs: Iterable[dict], scen: Iterable[dict]) -> Iterator[Edge]:
    """A scenario that cites an original requirement also validates its refinement(s)."""
    by_orig = {}
    for r in refs:
        by_orig.setdefault(r["original_id"], []).append(r["refinement_id"])
    for s in scen:
        for rid in ids_in(s.get("requirement_ids", "")):
            yield from ((rr, "tested_by", s["scenario_id"], "scenarios.csv (via original)") for rr in by_orig.get(rid, ()))


def ui_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for u in recs:
        yield from ((u["id"], "derived_from", t, "ui-requirements.jsonl") for t in u.get("traces", ()) if ID.fullmatch(t))
        yield from ((u["id"], "depends_on", g, "ui-requirements.jsonl") for g in u.get("capability_gaps", ()))


def decision_edges(recs: Iterable[dict]) -> Iterator[Edge]:
    for d in recs:
        yield from ((x, "decided_by", d["decision_id"], "decisions.csv") for x in ids_in(d.get("affected_ids", "")))


def build(data: Path) -> Tuple[Edge, ...]:
    edges = (
        *refinement_edges(jsonl(data / "refinements.jsonl")),
        *gap_edges(jsonl(data / "gap-requirements.jsonl")),
        *finding_edges(rows(data / "findings.csv")),
        *duplicate_edges(rows(data / "findings.csv")),
        *conflict_edges(rows(data / "conflicts.csv")),
        *scenario_edges(rows(data / "scenarios.csv")),
        *inherited_test_edges(jsonl(data / "refinements.jsonl"), rows(data / "scenarios.csv")),
        *decision_edges(rows(data / "decisions.csv")),
        *ui_edges(jsonl(data / "ui-requirements.jsonl")),
    )
    return tuple(dict.fromkeys(edges))


def main(argv) -> int:
    data = Path(argv[1])
    edges = build(data)
    with (data / "trace-edges.csv").open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh)
        w.writerow(("source", "relationship", "target", "provenance"))
        w.writerows(edges)
    print(json.dumps(dict(Counter(e[1] for e in edges)), indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

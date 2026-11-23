#!/usr/bin/env python3
"""Quality gates for the requirements reconciliation records (mission section 16).

Each check is a pure function returning a tuple of Issue values; the program
aggregates them, prints a report, and exits non-zero when any ERROR exists.

Checks:
  G1 duplicate identifiers within and across new record sets
  G2 new identifiers colliding with baseline identifiers
  G3 refinement original_id exists in the baseline inventory
  G4 refinement original_text is present verbatim (normalized) in the cited file at baseline
  G5 required fields present and non-empty (refinements, gaps, findings, conflicts, scenarios, decisions)
  G6 acceptance criteria are Given/When/Then objects
  G7 gap requirements declare origin=gap-analysis, a gap finding that exists, and why_no_predecessor
  G8 every reference in records resolves (baseline id, new id, family wildcard, NARR surrogate,
     competitive capability CAP-*, regulatory obligation OB-*, or a withdrawn id
     listed with its replacement in withdrawn-ids.csv)
  G9 trace-edge relationships belong to the vocabulary and endpoints resolve
  G10 identifiers cited in reconciliation Markdown reports resolve
  G11 every refinement and gap requirement has at least one tested_by edge (WARN)

Usage: python3 -I tools/reqaudit/validate.py <repo-root>
"""
from __future__ import annotations

import csv
import json
import re
import subprocess
import sys
from collections import Counter
from dataclasses import dataclass
from pathlib import Path
from typing import Callable, Dict, FrozenSet, Iterable, Tuple

RECON = Path("requirements/reconciliation/2026-11-23")
BASELINE_REV = "777a70d"
REL_VOCAB = frozenset({"refines", "supersedes", "conflicts_with", "duplicates", "depends_on", "derived_from",
                       "validated_by", "implemented_by", "tested_by", "affects", "decided_by"})
NEW_ID = re.compile(r"^(RR-.+-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})$")
CITED = re.compile(r"\b(RR-[A-Z0-9-]+?-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})\b")
ANY_ID = re.compile(r"^[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-(?:[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?|\*)$")

REQUIRED = {
    "refinements": ("refinement_id", "original_id", "original_path", "original_line", "original_revision", "original_text",
                    "relationship", "statement", "business_objective", "actors", "processing_rules", "failure_behavior",
                    "security", "auditability", "acceptance_criteria", "reason_for_change", "evidence", "confidence",
                    "validation_status", "decision_status"),
    "gaps": ("id", "origin", "parent", "gap_finding", "why_no_predecessor", "statement", "rationale", "evidence",
             "acceptance_criteria", "links", "priority"),
    "findings": ("finding_id", "severity", "dimension", "affected_ids", "evidence", "title", "description", "proposed_resolution"),
    "conflicts": ("conflict_id", "requirement_a", "requirement_b", "incompatibility", "recommended_resolution"),
    "scenarios": ("scenario_id", "title", "requirement_ids", "stimulus", "expected_states"),
    "decisions": ("decision_id", "question", "affected_ids", "decision_owner", "recommendation", "status"),
}


@dataclass(frozen=True)
class Issue:
    level: str  # ERROR | WARN
    gate: str
    subject: str
    message: str


def load_jsonl(p: Path) -> Tuple[dict, ...]:
    return tuple(json.loads(l) for l in p.read_text(encoding="utf-8").splitlines() if l.strip()) if p.exists() else ()


def load_csv(p: Path) -> Tuple[dict, ...]:
    return tuple(csv.DictReader(p.open(encoding="utf-8"))) if p.exists() else ()


def normalize(t: str) -> str:
    return re.sub(r"\s+", " ", re.sub(r"[*_`]|^\s*-\s*", "", t or "")).strip().lower()


def baseline_file(root: Path, path: str) -> str:
    r = subprocess.run(["git", "-C", str(root), "show", f"{BASELINE_REV}:{path}"], capture_output=True, text=True)
    return r.stdout if r.returncode == 0 else ""


def resolver(baseline: FrozenSet[str], new: FrozenSet[str], narr: FrozenSet[str],
             known_undefined: FrozenSet[str] = frozenset()) -> Callable[[str], bool]:
    families = frozenset(re.sub(r"-[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?$", "", i) for i in baseline | new)

    def ok(ref: str) -> bool:
        ref = ref.strip().rstrip(".")
        if ref in baseline or ref in new or ref in narr or ref in known_undefined:
            return True
        if ref.endswith("-*"):
            return ref[:-2] in families
        if "@" in ref:  # qualified reference to a colliding id
            return ref.split("@")[0] in baseline
        return False
    return ok


def g_dupes(sets: Dict[str, Tuple[str, ...]]) -> Tuple[Issue, ...]:
    allnew = [i for ids in sets.values() for i in ids]
    return tuple(Issue("ERROR", "G1", i, f"identifier defined {n} times across reconciliation records")
                 for i, n in Counter(allnew).items() if n > 1)


def g_collide(new: FrozenSet[str], baseline: FrozenSet[str]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", "G2", i, "new identifier collides with a baseline identifier") for i in new & baseline) + \
        tuple(Issue("ERROR", "G2", i, "new identifier does not follow a reconciliation ID scheme") for i in new if not NEW_ID.match(i))


def g_refinements(root: Path, refs: Tuple[dict, ...], baseline: FrozenSet[str]) -> Tuple[Issue, ...]:
    def check(r: dict) -> Iterable[Issue]:
        rid = r.get("refinement_id", "?")
        if r.get("original_id") not in baseline:
            yield Issue("ERROR", "G3", rid, f"original_id {r.get('original_id')} not in baseline inventory")
        if not rid.startswith(f"RR-{r.get('original_id')}-r"):
            yield Issue("ERROR", "G3", rid, "refinement_id must be RR-<original_id>-r<n>")
        src = normalize(baseline_file(root, r.get("original_path", "")))
        if not src:
            yield Issue("ERROR", "G4", rid, f"original_path {r.get('original_path')} not found at {BASELINE_REV}")
        elif normalize(r.get("original_text", ""))[:160] not in src:
            yield Issue("ERROR", "G4", rid, "original_text not found verbatim (first 160 normalized chars) in original file")
        if r.get("relationship") not in ("refines", "supersedes"):
            yield Issue("ERROR", "G9", rid, f"relationship {r.get('relationship')} invalid for a refinement")
    return tuple(i for r in refs for i in check(r))


def g_required(name: str, recs: Tuple[dict, ...], key: str) -> Tuple[Issue, ...]:
    def empty(v) -> bool:
        return v is None or (isinstance(v, (str, list, dict)) and len(v) == 0)
    return tuple(Issue("ERROR", "G5", r.get(key, "?"), f"{name}: missing or empty field '{f}'")
                 for r in recs for f in REQUIRED[name] if empty(r.get(f)))


def g_acceptance(recs: Tuple[dict, ...], key: str) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", "G6", r.get(key, "?"), f"acceptance criterion {a!r:.60} lacks given/when/then")
                 for r in recs for a in r.get("acceptance_criteria", ())
                 if not (isinstance(a, dict) and all(a.get(k) for k in ("given", "when", "then"))))


def g_gaps(gaps: Tuple[dict, ...], finding_ids: FrozenSet[str]) -> Tuple[Issue, ...]:
    return tuple(i for g in gaps for i in (
        *((Issue("ERROR", "G7", g["id"], "origin must be gap-analysis"),) if g.get("origin") != "gap-analysis" else ()),
        *((Issue("ERROR", "G7", g["id"], f"gap_finding {g.get('gap_finding')} not in findings register"),)
          if g.get("gap_finding") not in finding_ids else ()),
    ))


def g_refs(label: str, subject: str, refs: Iterable[str], ok: Callable[[str], bool]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", "G8", subject, f"{label}: unresolved reference '{r}'")
                 for r in refs if r.strip() and ANY_ID.match(r.strip()) and not ok(r))


def g_edges(edges: Tuple[dict, ...], ok: Callable[[str], bool]) -> Tuple[Issue, ...]:
    return tuple(i for e in edges for i in (
        *((Issue("ERROR", "G9", f"{e['source']}->{e['target']}", f"relationship '{e['relationship']}' not in vocabulary"),)
          if e["relationship"] not in REL_VOCAB else ()),
        *((Issue("ERROR", "G9", f"{e['source']}->{e['target']}", f"unresolved endpoint ({e['provenance']})"),)
          if not (ok(e["source"]) and ok(e["target"])) else ()),
    ))


def g_reports(root: Path, ok: Callable[[str], bool]) -> Tuple[Issue, ...]:
    docs = sorted((root / RECON).glob("*.md"))
    return tuple(Issue("ERROR", "G10", f"{d.name}", f"cites unknown identifier {m}")
                 for d in docs for m in sorted(set(CITED.findall(d.read_text(encoding="utf-8")))) if not ok(m))


def g_tested(new_reqs: Iterable[str], edges: Tuple[dict, ...]) -> Tuple[Issue, ...]:
    tested = {e["source"] for e in edges if e["relationship"] == "tested_by"}
    return tuple(Issue("WARN", "G11", r, "no tested_by scenario") for r in new_reqs if r not in tested)


def main(argv) -> int:
    root = Path(argv[1]).resolve()
    data = root / RECON / "data"
    inv = load_csv(data / "inventory.csv")
    baseline = frozenset(r["identifier"] for r in inv)
    narr = frozenset(r["surrogate_id"] for r in load_csv(data / "narrative-requirements.csv")) | \
        frozenset(r["capability_id"] for r in load_csv(data / "competitive-rollup.csv")) | \
        frozenset(r["obligation_id"] for r in load_csv(data / "regulatory-obligations.csv")) | \
        frozenset(r["identifier"] for r in load_csv(data / "withdrawn-ids.csv"))
    refs_, gaps = load_jsonl(data / "refinements.jsonl"), load_jsonl(data / "gap-requirements.jsonl")
    finds, confs = load_csv(data / "findings.csv"), load_csv(data / "conflicts.csv")
    scens, decs = load_csv(data / "scenarios.csv"), load_csv(data / "decisions.csv")
    edges = load_csv(data / "trace-edges.csv")
    sets = {
        "refinements": tuple(r["refinement_id"] for r in refs_), "gaps": tuple(g["id"] for g in gaps),
        "findings": tuple(f["finding_id"] for f in finds), "conflicts": tuple(c["conflict_id"] for c in confs),
        "scenarios": tuple(s["scenario_id"] for s in scens), "decisions": tuple(d["decision_id"] for d in decs),
    }
    new = frozenset(i for ids in sets.values() for i in ids)
    known_undefined = frozenset(r["identifier"] for r in load_csv(data / "undefined-references.csv"))
    ok = resolver(baseline, new, narr, known_undefined)
    strict = resolver(baseline, new, narr)
    sys.path.insert(0, str(Path(__file__).parent))
    from graph import ids_in
    split = ids_in
    issues = (
        *g_dupes(sets), *g_collide(new, baseline),
        *g_refinements(root, refs_, baseline),
        *g_required("refinements", refs_, "refinement_id"), *g_required("gaps", gaps, "id"),
        *g_required("findings", finds, "finding_id"), *g_required("conflicts", confs, "conflict_id"),
        *g_required("scenarios", scens, "scenario_id"), *g_required("decisions", decs, "decision_id"),
        *g_acceptance(refs_, "refinement_id"), *g_acceptance(gaps, "id"),
        *g_gaps(gaps, frozenset(sets["findings"])),
        *(i for f in finds for i in g_refs("affected_ids", f["finding_id"], split(f["affected_ids"]), ok)),
        *(i for c in confs for i in g_refs("requirements", c["conflict_id"], split(c["requirement_a"]) + split(c["requirement_b"]), ok)),
        *(i for s in scens for i in g_refs("requirement_ids", s["scenario_id"], split(s["requirement_ids"]), ok)),
        *(i for r in refs_ for i in g_refs("related_findings", r["refinement_id"], r.get("related_findings", ()), ok)),
        *(i for g in gaps for i in g_refs("links", g["id"], g.get("links", ()), ok)),
        *g_edges(edges, ok), *g_reports(root, ok),
        *g_tested(sets["refinements"] + sets["gaps"], edges),
        *(Issue("ERROR", "G1", w["identifier"], "withdrawn id is still used by a live record")
          for w in load_csv(data / "withdrawn-ids.csv") if w["identifier"] in new),
        *(Issue("WARN", "G8u", e["source"], f"references {e['target']}, which the baseline cites but never defines")
          for e in edges if ok(e["target"]) and not strict(e["target"])),
    )
    errors = tuple(i for i in issues if i.level == "ERROR")
    for i in issues:
        print(f"{i.level:5} {i.gate:4} {i.subject}: {i.message}")
    summary = {"records": {k: len(v) for k, v in sets.items()}, "trace_edges": len(edges),
               "errors": len(errors), "warnings": len(issues) - len(errors),
               "errors_by_gate": dict(Counter(i.gate for i in errors))}
    (data / "validation-report.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(summary, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

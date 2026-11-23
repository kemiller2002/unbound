#!/usr/bin/env python3
"""Quality gates for the cross-industry discovery records (requirements/discovery/<mission>/data).

Reuses the pure gate functions of validate.py (G1, G3-G9, G11) and adds gates for record types
that only the discovery mission has. Every check is a pure function returning a tuple of Issue
values; main() is the only function with effects.

References resolve against: the baseline inventory, the reconciliation records (RR/UNB-RCN/RCF/
RCC/RCD/DKS, OB-*, CAP-*, NARR-*) and this mission's own records.

Discovery-specific gates:
  D1  capability-atlas rows use the enumerated coverage, necessity, daf_equivalent and automation values
  D2  new requirements carry the mission section-11 fields (discovery provenance, analogy, priority
      justification, failure conditions, UI/API/reporting/testing implications, challenge fields)
  D3  priorities are P0-P4
  D4  event-matrix rows: authority enumerated; an 'observation-ai' event is never the source of an
      authoritative store and never synchronous-financial; every event names an owning module
  D5  rejected recommendations carry a reason and the strongest argument for them
  D6  failure-catalog rows name detection, recovery and escalation; their ids resolve
  D7  every new requirement and refinement is tested by a scenario (ERROR here; WARN in G11)
  D8  daf-landscape (if present beside this repository) has no working-tree changes
  D9  identifiers cited in the mission Markdown reports resolve

Usage: python3 -I tools/reqaudit/validate_discovery.py <repo-root> [mission-dir]
"""
from __future__ import annotations

import json
import re
import subprocess
import sys
from collections import Counter
from pathlib import Path
from typing import Callable, FrozenSet, Iterable, Tuple

sys.path.insert(0, str(Path(__file__).parent))
import validate as v  # noqa: E402
from graph import ids_in  # noqa: E402

MISSION = Path("requirements/discovery/2026-11-23-cross-industry")
NEW_ID = re.compile(
    r"^(RR-.+-r\d+|UNB-XIN-[A-Z]{3}-\d{3}|XIF-[A-Z]{3}-\d{3}|XIC-[A-Z]{3}-\d{3}|XID-[A-Z]{3}-\d{3}|"
    r"XDS-[A-Z]{3}-\d{3}|XCAP-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|XFC-[A-Z]{3}-\d{3}|XEV-\d{3}|XRA-\d{3}|"
    r"XWF-\d{3}|XADR-\d{3})$")
CITED = re.compile(
    r"\b(UNB-XIN-[A-Z]{3}-\d{3}|XIF-[A-Z]{3}-\d{3}|XIC-[A-Z]{3}-\d{3}|XID-[A-Z]{3}-\d{3}|XDS-[A-Z]{3}-\d{3}|"
    r"XCAP-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|XFC-[A-Z]{3}-\d{3}|XEV-\d{3}|XRA-\d{3}|XWF-\d{3}|XADR-\d{3}|"
    r"RR-[A-Z0-9-]+?-r\d+|UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3})\b")

COVERAGE = frozenset({"covered", "partial", "contradictory", "missing"})
NECESSITY = frozenset({"legal", "operational", "commercial", "conventional"})
EQUIV = frozenset({"yes", "partial", "no"})
AUTOMATION = frozenset({"deterministic", "ai-assisted", "configuration", "human-required", "mixed"})
AUTHORITY = frozenset({"authoritative-financial", "authoritative-nonfinancial", "derived", "informational", "observation-ai"})
PRIORITY = frozenset({"P0", "P1", "P2", "P3", "P4"})
SECTION11 = ("discovery_source", "industry_analogy", "parent_capability", "business_problem", "applicability",
             "failure_conditions", "priority_justification", "security_compliance", "ui_implications",
             "api_implications", "reporting_implications", "testing_requirements", "argue_for", "argue_against",
             "simplest_valid_alternative", "decision_rationale", "confidence")

Issue = v.Issue


def empty(x) -> bool:
    return x is None or (isinstance(x, (str, list, dict, tuple)) and len(x) == 0)


def as_list(x) -> Tuple[str, ...]:
    return tuple(x) if isinstance(x, (list, tuple)) else ids_in(str(x or ""))


def g_scheme(new: FrozenSet[str], baseline: FrozenSet[str], prior: FrozenSet[str]) -> Tuple[Issue, ...]:
    return (tuple(Issue("ERROR", "G2", i, "new identifier collides with a baseline identifier") for i in new & baseline)
            + tuple(Issue("ERROR", "G2", i, "new identifier collides with a reconciliation record") for i in new & prior)
            + tuple(Issue("ERROR", "G2", i, "identifier does not follow a discovery ID scheme") for i in new if not NEW_ID.match(i)))


def g_enum(gate: str, recs: Iterable[dict], key: str, field: str, allowed: FrozenSet[str]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", gate, r.get(key, "?"), f"{field}={r.get(field)!r} not in {sorted(allowed)}")
                 for r in recs if r.get(field) not in allowed)


def g_fields(gate: str, recs: Iterable[dict], key: str, fields: Tuple[str, ...]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", gate, r.get(key, "?"), f"missing or empty field '{f}'")
                 for r in recs for f in fields if empty(r.get(f)))


def g_events(events: Tuple[dict, ...]) -> Tuple[Issue, ...]:
    def check(e: dict) -> Iterable[Issue]:
        i = e.get("id", "?")
        if e.get("authority") not in AUTHORITY:
            yield Issue("ERROR", "D4", i, f"authority {e.get('authority')!r} not enumerated")
        if empty(e.get("owning_module")):
            yield Issue("ERROR", "D4", i, "no owning module")
        if e.get("authority") == "observation-ai" and "ledger" in str(e.get("source_of_truth_store", "")).lower():
            yield Issue("ERROR", "D4", i, "AI observation declared as stored in an authoritative ledger")
        if e.get("authority") == "observation-ai" and e.get("consistency") == "synchronous":
            yield Issue("ERROR", "D4", i, "AI observation declared synchronous-consistent")
    return tuple(x for e in events for x in check(e))


def g_refs_list(gate: str, recs: Iterable[dict], key: str, fields: Tuple[str, ...], ok: Callable[[str], bool]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", gate, r.get(key, "?"), f"{f}: unresolved reference '{ref}'")
                 for r in recs for f in fields for ref in as_list(r.get(f))
                 if ref.strip() and v.ANY_ID.match(ref.strip()) and not ok(ref))


def g_tested(new_reqs: Iterable[str], edges: Tuple[dict, ...]) -> Tuple[Issue, ...]:
    tested = frozenset(e["source"] for e in edges if e["relationship"] == "tested_by")
    return tuple(Issue("ERROR", "D7", r, "no tested_by scenario") for r in new_reqs if r not in tested)


def g_landscape(root: Path) -> Tuple[Issue, ...]:
    other = root.parent / "daf-landscape"
    if not (other / ".git").exists():
        return (Issue("WARN", "D8", "daf-landscape", "clone not found beside repository; unchanged-state not checked"),)
    r = subprocess.run(["git", "-C", str(other), "status", "--porcelain"], capture_output=True, text=True)
    return () if not r.stdout.strip() else (Issue("ERROR", "D8", "daf-landscape", "working tree has changes"),)


def g_reports(mission: Path, ok: Callable[[str], bool]) -> Tuple[Issue, ...]:
    return tuple(Issue("ERROR", "D9", d.name, f"cites unknown identifier {m}")
                 for d in sorted(mission.glob("*.md")) for m in sorted(set(CITED.findall(d.read_text(encoding="utf-8"))))
                 if not ok(m))


def main(argv) -> int:
    root = Path(argv[1]).resolve()
    mission = root / (Path(argv[2]) if len(argv) > 2 else MISSION)
    data, recon = mission / "data", root / v.RECON / "data"
    inv = v.load_csv(recon / "inventory.csv")
    baseline = frozenset(r["identifier"] for r in inv)
    prior = (frozenset(r["refinement_id"] for r in v.load_jsonl(recon / "refinements.jsonl"))
             | frozenset(g["id"] for g in v.load_jsonl(recon / "gap-requirements.jsonl"))
             | frozenset(r["finding_id"] for r in v.load_csv(recon / "findings.csv"))
             | frozenset(r["conflict_id"] for r in v.load_csv(recon / "conflicts.csv"))
             | frozenset(r["scenario_id"] for r in v.load_csv(recon / "scenarios.csv"))
             | frozenset(r["decision_id"] for r in v.load_csv(recon / "decisions.csv")))
    narr = (frozenset(r["surrogate_id"] for r in v.load_csv(recon / "narrative-requirements.csv"))
            | frozenset(r["capability_id"] for r in v.load_csv(recon / "competitive-rollup.csv"))
            | frozenset(r["obligation_id"] for r in v.load_csv(recon / "regulatory-obligations.csv"))
            | frozenset(r["identifier"] for r in v.load_csv(recon / "withdrawn-ids.csv")))
    known_undefined = frozenset(r["identifier"] for r in v.load_csv(recon / "undefined-references.csv"))

    refs_, gaps = v.load_jsonl(data / "refinements.jsonl"), v.load_jsonl(data / "gap-requirements.jsonl")
    finds, confs = v.load_csv(data / "findings.csv"), v.load_csv(data / "conflicts.csv")
    scens, decs = v.load_csv(data / "scenarios.csv"), v.load_csv(data / "decisions.csv")
    atlas, rejected = v.load_jsonl(data / "capability-atlas.jsonl"), v.load_jsonl(data / "rejected.jsonl")
    failures, events = v.load_jsonl(data / "failure-catalog.jsonl"), v.load_jsonl(data / "event-matrix.jsonl")
    regs = v.load_jsonl(data / "regulatory-applicability.jsonl")
    edges = v.load_csv(data / "trace-edges.csv")
    sets = {
        "refinements": tuple(r["refinement_id"] for r in refs_), "gaps": tuple(g["id"] for g in gaps),
        "findings": tuple(f["finding_id"] for f in finds), "conflicts": tuple(c["conflict_id"] for c in confs),
        "scenarios": tuple(s["scenario_id"] for s in scens), "decisions": tuple(d["decision_id"] for d in decs),
        "atlas": tuple(a["id"] for a in atlas), "rejected": tuple(r["id"] for r in rejected),
        "failures": tuple(f["id"] for f in failures), "events": tuple(e["id"] for e in events),
        "regulatory": tuple(r["id"] for r in regs),
    }
    new = frozenset(i for ids in sets.values() for i in ids)
    ok = v.resolver(baseline, new | prior, narr, known_undefined)
    issues = (
        *v.g_dupes(sets), *g_scheme(new, baseline, prior),
        *v.g_refinements(root, refs_, baseline),
        *v.g_required("refinements", refs_, "refinement_id"), *v.g_required("gaps", gaps, "id"),
        *v.g_required("findings", finds, "finding_id"), *v.g_required("conflicts", confs, "conflict_id"),
        *v.g_required("scenarios", scens, "scenario_id"), *v.g_required("decisions", decs, "decision_id"),
        *v.g_acceptance(refs_, "refinement_id"), *v.g_acceptance(gaps, "id"),
        *v.g_gaps(gaps, frozenset(sets["findings"])),
        *(i for f in finds for i in v.g_refs("affected_ids", f["finding_id"], ids_in(f["affected_ids"]), ok)),
        *(i for c in confs for i in v.g_refs("requirements", c["conflict_id"], ids_in(c["requirement_a"]) + ids_in(c["requirement_b"]), ok)),
        *(i for s in scens for i in v.g_refs("requirement_ids", s["scenario_id"], ids_in(s["requirement_ids"]), ok)),
        *(i for d in decs for i in v.g_refs("affected_ids", d["decision_id"], ids_in(d["affected_ids"]), ok)),
        *(i for r in refs_ for i in v.g_refs("related_findings", r["refinement_id"], r.get("related_findings", ()), ok)),
        *(i for g in gaps for i in v.g_refs("links", g["id"], g.get("links", ()), ok)),
        *v.g_edges(edges, ok),
        *g_enum("D1", atlas, "id", "coverage", COVERAGE), *g_enum("D1", atlas, "id", "necessity", NECESSITY),
        *g_enum("D1", atlas, "id", "daf_equivalent", EQUIV), *g_enum("D1", atlas, "id", "automation", AUTOMATION),
        *g_refs_list("D1", atlas, "id", ("existing_ids",), ok),
        *g_fields("D2", gaps, "id", SECTION11),
        *g_enum("D3", gaps, "id", "priority", PRIORITY),
        *g_events(events), *g_refs_list("D4", events, "id", ("existing_ids",), ok),
        *g_fields("D5", rejected, "id", ("idea", "argue_for", "argue_against", "reason_rejected")),
        *g_fields("D6", failures, "id", ("failure_mode", "detection", "recovery", "escalation")),
        *g_refs_list("D6", failures, "id", ("covered_by", "new_ids"), ok),
        *g_refs_list("D1", regs, "id", ("related_OB_ids", "existing_requirement_ids"), ok),
        *g_tested(sets["refinements"] + sets["gaps"], edges),
        *g_landscape(root), *g_reports(mission, ok),
    )
    errors = tuple(i for i in issues if i.level == "ERROR")
    for i in issues:
        print(f"{i.level:5} {i.gate:4} {i.subject}: {i.message}")
    summary = {"records": {k: len(x) for k, x in sets.items()}, "trace_edges": len(edges),
               "errors": len(errors), "warnings": len(issues) - len(errors),
               "errors_by_gate": dict(Counter(i.gate for i in errors))}
    (data / "validation-report.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(summary, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

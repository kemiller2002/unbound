#!/usr/bin/env python3
"""Quality gates for the simplification and economic-engineering mission
(requirements/discovery/2026-11-24-simplification-economics/data).

Reuses the pure gate functions of validate.py and validate_discovery.py; adds gates for the reverse-analysis
register, work inventory, metrics, configuration items, interactions, competitive rows and research-transfer
candidates. main() is the only function with effects.

References resolve against: the baseline inventory, the reconciliation records, the cross-industry discovery
records (all ID-bearing data files) and this mission's own records.

Gates (in addition to G1-G9, D2, D3, D5, D7, D8, D9 reused):
  S1  the register covers every requirement in the review universe exactly once (baseline canonical requirement IDs
      excluding '-Q' restatements, reconciliation UNB-RCN/RR records, discovery UNB-XIN/RR records)
  S2  register dispositions, levels and convention origins use the enumerated vocabularies
  S3  register 'recommend-removal' rows carry an elimination consequence, reasoning and evidence_to_change
  S4  every kept metric names the decision it supports and its expected action; removed metrics give a reason
  S5  work-inventory, configuration, interaction, comparison and transfer rows carry their mandatory fields and
      their referenced IDs resolve; transfer candidates are independent of Unbound
  S6  identifiers cited in the mission Markdown reports resolve
  S7  every 'consolidate' row names a target, and following consolidate links ends at a row that is not itself
      consolidated or deferred (or at a record outside the register); cycles and dead ends would lose the obligation

The review universe also includes the 14 distinct second definition sites listed as 'reviewed-separately' in
data/second-site-accounting.csv. Identifiers defined outside the requirement corpus (Echelon component gap IDs) resolve
through data/external-ids.csv, each with the file that defines it.

Usage: python3 -I -B tools/reqaudit/validate_simplification.py <repo-root> [mission-dir]
"""
from __future__ import annotations

import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Callable, FrozenSet, Iterable, Tuple

sys.path.insert(0, str(Path(__file__).parent))
import validate as v  # noqa: E402
import validate_discovery as vd  # noqa: E402
from graph import ids_in  # noqa: E402

MISSION = Path("requirements/discovery/2026-11-24-simplification-economics")
DISCOVERY = Path("requirements/discovery/2026-11-23-cross-industry")
NEW_ID = re.compile(r"^(RR-.+-r\d+|UNB-SMP-[A-Z]{3}-\d{3}|SP[FDSR]-[A-Z]{3}-\d{3}|SP[WMGCTX]-\d{3}|ORG-R\d{2})$")
CITED = re.compile(
    r"\b(UNB-SMP-[A-Z]{3}-\d{3}|SP[FDSR]-[A-Z]{3}-\d{3}|SP[WMGCTX]-\d{3}|"
    r"UNB-XIN-[A-Z]{3}-\d{3}|XI[FCD]-[A-Z]{3}-\d{3}|XDS-[A-Z]{3}-\d{3}|XRJ-[A-Z]{3}-\d{3}|"
    r"UNB-RCN-[A-Z]+-\d{3}|RCF-[A-Z]+-\d{3}|RCC-[A-Z]+-\d{3}|RCD-\d{3}|DKS-[A-Z]+-\d{3}|RR-[A-Z0-9-]+?-r\d+)\b")
DISPOSITIONS = frozenset({"retain", "simplify", "consolidate", "replace", "automate-deterministic", "automate-ai-assisted",
                          "exception-driven", "conditional-on-operating-model", "defer", "recommend-removal", "need-evidence"})
LEVELS = frozenset({"none", "low", "medium", "high"})
ORIGINS = frozenset({"legacy-systems", "department-silos", "digitized-paper", "integration-gaps", "information-unavailable",
                     "human-compensating-for-technology", "vendor-limitation", "fee-model-dependency",
                     "conservative-interpretation", "policy-accretion", "none"})
Issue = v.Issue


def ids_of(data: Path) -> FrozenSet[str]:
    """Every record identifier defined in a mission data directory (JSONL 'id'-like keys and CSV first columns)."""
    keys = ("id", "refinement_id", "finding_id", "conflict_id", "scenario_id", "decision_id")
    jl = frozenset(r.get(k) for p in data.glob("*.jsonl") for r in v.load_jsonl(p) for k in keys if r.get(k))
    cv = frozenset(r.get(k) for p in data.glob("*.csv") if p.name != "trace-edges.csv" for r in v.load_csv(p) for k in keys if r.get(k))
    return jl | cv


def universe(root: Path, recon: Path, disc: Path, data_dir: Path) -> FrozenSet[str]:
    inv = v.load_csv(recon / "inventory.csv")
    base = frozenset(r["identifier"] for r in inv if r["kind"] == "requirement" and not r["identifier"].endswith("-Q"))
    recs = (frozenset(r["refinement_id"] for r in v.load_jsonl(recon / "refinements.jsonl"))
            | frozenset(g["id"] for g in v.load_jsonl(recon / "gap-requirements.jsonl"))
            | frozenset(r["refinement_id"] for r in v.load_jsonl(disc / "refinements.jsonl"))
            | frozenset(g["id"] for g in v.load_jsonl(disc / "gap-requirements.jsonl")))
    second = frozenset(r["register_id"] for r in v.load_csv(data_dir / "second-site-accounting.csv") if r["action"] == "reviewed-separately")
    return base | recs | second


def g_scheme(new: FrozenSet[str], baseline: FrozenSet[str], prior: FrozenSet[str]) -> Tuple[Issue, ...]:
    return (tuple(Issue("ERROR", "G2", i, "new identifier collides with a baseline identifier") for i in new & baseline)
            + tuple(Issue("ERROR", "G2", i, "new identifier collides with a prior mission record") for i in new & prior)
            + tuple(Issue("ERROR", "G2", i, "identifier does not follow a simplification-mission ID scheme")
                    for i in new if not NEW_ID.match(i)))


def g_register(reg: Tuple[dict, ...], scope: FrozenSet[str]) -> Tuple[Issue, ...]:
    counts = Counter(r.get("id") for r in reg)
    missing = tuple(Issue("ERROR", "S1", i, "requirement not reviewed in the register") for i in sorted(scope - set(counts)))
    extra = tuple(Issue("ERROR", "S1", i, "register row for an identifier outside the review universe") for i in sorted(set(counts) - scope))
    dup = tuple(Issue("ERROR", "S1", i, f"reviewed {n} times") for i, n in counts.items() if n > 1)

    def row(r: dict) -> Iterable[Issue]:
        i = r.get("id", "?")
        if r.get("disposition") not in DISPOSITIONS:
            yield Issue("ERROR", "S2", i, f"disposition {r.get('disposition')!r}")
        for f in ("human_work", "customer_friction", "configuration_burden"):
            if (r.get(f) or {}).get("level") not in LEVELS:
                yield Issue("ERROR", "S2", i, f"{f}.level {(r.get(f) or {}).get('level')!r}")
        bad = [o for o in (r.get("convention_origin") or []) if o not in ORIGINS]
        if bad:
            yield Issue("ERROR", "S2", i, f"convention_origin {bad}")
        for f in ("original_purpose", "reasoning", "confidence"):
            if vd.empty(r.get(f)):
                yield Issue("ERROR", "S2", i, f"missing {f}")
        if r.get("disposition") == "recommend-removal":
            for f in ("elimination_consequence", "evidence_to_change"):
                if vd.empty(r.get(f)):
                    yield Issue("ERROR", "S3", i, f"removal recommended without {f}")
    return missing + extra + dup + tuple(x for r in reg for x in row(r))


def g_chains(reg: Tuple[dict, ...]) -> Tuple[Issue, ...]:
    by = {r["id"]: r for r in reg}
    def end(i: str, seen: Tuple[str, ...]) -> Tuple[bool, str]:
        r = by.get(i)
        if r is None:
            return True, i                       # record outside the register (e.g. a control or decision) carries it
        if r["disposition"] == "recommend-removal" and seen:
            return False, f"{i} (recommend-removal)"
        if r["disposition"] == "defer" and seen:
            return True, f"DEFERRED:{i}"
        if r["disposition"] != "consolidate":
            return True, i
        targets = [t for t in (r.get("consolidate_with") or []) if t != i]
        if not targets:
            return False, f"{i} (no target)"
        if i in seen:
            return False, "cycle " + " -> ".join(seen + (i,))
        results = [end(t, seen + (i,)) for t in targets]
        return (True, results[[ok for ok, _ in results].index(True)][1]) if any(ok for ok, _ in results) else results[0]
    def check(r: dict) -> Iterable[Issue]:
        if r["disposition"] == "consolidate":
            ok, where = end(r["id"], ())
            if not ok:
                yield Issue("ERROR", "S7", r["id"], f"consolidation chain does not reach a retained carrier: {where}")
            elif where.startswith("DEFERRED:"):
                yield Issue("WARN", "S7", r["id"], f"obligation is carried by a deferred requirement ({where[9:]}); it is deferred with that capability")
    return tuple(x for r in reg for x in check(r))


def g_metrics(metrics: Tuple[dict, ...]) -> Tuple[Issue, ...]:
    def row(m: dict) -> Iterable[Issue]:
        i = m.get("id", "?")
        keep = str(m.get("keep", "")).upper().startswith("Y")
        need = ("definition", "decision_supported", "expected_action", "authoritative_source") if keep else ("keep",)
        yield from (Issue("ERROR", "S4", i, f"missing {f}") for f in need if vd.empty(m.get(f)))
    return tuple(x for m in metrics for x in row(m))


def main(argv) -> int:
    root = Path(argv[1]).resolve()
    mission = root / (Path(argv[2]) if len(argv) > 2 else MISSION)
    data, recon, disc = mission / "data", root / v.RECON / "data", root / DISCOVERY / "data"
    inv = v.load_csv(recon / "inventory.csv")
    baseline = frozenset(r["identifier"] for r in inv)
    prior = ids_of(recon) - baseline | ids_of(disc)
    narr = (frozenset(r["surrogate_id"] for r in v.load_csv(recon / "narrative-requirements.csv"))
            | frozenset(r["capability_id"] for r in v.load_csv(recon / "competitive-rollup.csv"))
            | frozenset(r["obligation_id"] for r in v.load_csv(recon / "regulatory-obligations.csv"))
            | frozenset(r["identifier"] for r in v.load_csv(recon / "withdrawn-ids.csv")))
    known_undefined = (frozenset(r["identifier"] for r in v.load_csv(recon / "undefined-references.csv"))
                       | frozenset(r["identifier"] for r in v.load_csv(data / "external-ids.csv")))
    L, C = (lambda n: v.load_jsonl(data / n)), (lambda n: v.load_csv(data / n))
    refs_, gaps, reg = L("refinements.jsonl"), L("gap-requirements.jsonl"), L("register.jsonl")
    finds, confs, scens, decs = C("findings.csv"), C("conflicts.csv"), C("scenarios.csv"), C("decisions.csv")
    rejected, work, metrics = L("rejected.jsonl"), L("work-inventory.jsonl"), L("metrics.jsonl")
    config, inter, comp, transfer = L("config-items.jsonl"), L("interactions.jsonl"), L("comparison.jsonl"), L("transfer.jsonl")
    edges = C("trace-edges.csv")
    sets = {"refinements": tuple(r["refinement_id"] for r in refs_), "gaps": tuple(g["id"] for g in gaps),
            "findings": tuple(f["finding_id"] for f in finds), "conflicts": tuple(c["conflict_id"] for c in confs),
            "scenarios": tuple(s["scenario_id"] for s in scens), "decisions": tuple(d["decision_id"] for d in decs),
            "rejected": tuple(r["id"] for r in rejected), "work": tuple(w["id"] for w in work),
            "metrics": tuple(m["id"] for m in metrics), "config": tuple(c["id"] for c in config),
            "interactions": tuple(x["id"] for x in inter), "comparison": tuple(c["id"] for c in comp),
            "transfer": tuple(t["id"] for t in transfer), "roles": tuple(r.get("id") or r["role_id"] for r in L("roles.jsonl"))}
    new = frozenset(i for ids in sets.values() for i in ids)
    ok = v.resolver(baseline, new | prior, narr, known_undefined)
    issues = (
        *v.g_dupes(sets), *g_scheme(new, baseline, prior),
        *vd.g_refinements_csv_aware(root, refs_, baseline),
        *v.g_required("refinements", refs_, "refinement_id"), *v.g_required("gaps", gaps, "id"),
        *v.g_required("findings", finds, "finding_id"), *v.g_required("conflicts", confs, "conflict_id"),
        *v.g_required("scenarios", scens, "scenario_id"), *v.g_required("decisions", decs, "decision_id"),
        *v.g_acceptance(refs_, "refinement_id"), *v.g_acceptance(gaps, "id"),
        *v.g_gaps(gaps, frozenset(sets["findings"])),
        *(i for f in finds for i in v.g_refs("affected_ids", f["finding_id"], ids_in(f["affected_ids"]), ok)),
        *(i for c in confs for i in v.g_refs("requirements", c["conflict_id"], ids_in(c["requirement_a"]) + ids_in(c["requirement_b"]), ok)),
        *(i for s in scens for i in v.g_refs("requirement_ids", s["scenario_id"], ids_in(s["requirement_ids"]), ok)),
        *(i for d in decs for i in v.g_refs("affected_ids", d["decision_id"], ids_in(d["affected_ids"]), ok)),
        *(i for g in gaps for i in v.g_refs("links", g["id"], g.get("links", ()), ok)),
        *v.g_edges(edges, ok),
        *vd.g_fields("D2", gaps, "id", vd.SECTION11), *vd.g_enum("D3", gaps, "id", "priority", vd.PRIORITY),
        *vd.g_fields("D5", rejected, "id", ("idea", "argue_for", "argue_against", "reason_rejected")),
        *g_register(reg, universe(root, recon, disc, data)), *g_chains(reg),
        *vd.g_refs_list("S2", reg, "id", ("dependencies", "consolidate_with"), ok),
        *g_metrics(metrics),
        *vd.g_fields("S5", work, "id", ("function", "activity", "trigger", "why_exists", "if_not_performed", "automation")),
        *vd.g_refs_list("S5", work, "id", ("existing_requirement_ids", "consolidate_with", "depends_on"), ok),
        *vd.g_fields("S5", config, "id", ("item", "who_needs_it", "why", "disposition")),
        *vd.g_refs_list("S5", config, "id", ("source_ids",), ok),
        *vd.g_fields("S5", inter, "id", ("persona", "step", "information_requested", "recommendation")),
        *vd.g_refs_list("S5", inter, "id", ("existing_ids",), ok),
        *vd.g_fields("S5", comp, "id", ("provider", "dimension", "evidence_grade", "assessment")),
        *vd.g_fields("S5", transfer, "id", ("research_question", "finding_or_hypothesis", "suggested_daf_landscape_destination")),
        *(Issue("ERROR", "S5", t["id"], "transfer candidate not independent of Unbound") for t in transfer
          if str(t.get("independent_of_unbound", "")).upper()[:1] != "Y"),
        *vd.g_tested(sets["refinements"] + sets["gaps"], edges),
        *vd.g_landscape(root),
        *(Issue("ERROR", "S6", d.name, f"cites unknown identifier {m}")
          for d in sorted(mission.rglob("*.md")) for m in sorted(set(CITED.findall(d.read_text(encoding="utf-8")))) if not ok(m)),
    )
    errors = tuple(i for i in issues if i.level == "ERROR")
    for i in issues:
        print(f"{i.level:5} {i.gate:4} {i.subject}: {i.message}")
    summary = {"records": {k: len(x) for k, x in sets.items()}, "register_rows": len(reg), "trace_edges": len(edges),
               "errors": len(errors), "warnings": len(issues) - len(errors),
               "errors_by_gate": dict(Counter(i.gate for i in errors))}
    data.mkdir(parents=True, exist_ok=True)
    (data / "validation-report.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(summary, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

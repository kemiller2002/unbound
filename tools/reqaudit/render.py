#!/usr/bin/env python3
"""Render data-driven tables into the reconciliation Markdown reports.

Hand-written narrative lives in the reports; tables between
    <!-- BEGIN GENERATED: <name> --> ... <!-- END GENERATED: <name> -->
are replaced from the data files, so summaries cannot drift from the records.

Usage: python3 -I tools/reqaudit/render.py <repo-root>
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Callable, Dict, Iterable, Tuple

RECON = Path("requirements/reconciliation/2026-11-23")
SEV = ("critical", "high", "medium", "low")
DOMAINS = ("GRT", "FIN", "PLT", "ENT", "REG", "ARC")


def cell(text: str, limit: int = 220) -> str:
    t = re.sub(r"\s+", " ", str(text or "")).replace("|", "\\|").strip()
    return t if len(t) <= limit else t[: limit - 1].rstrip() + "…"


def table(header: Tuple[str, ...], rows: Iterable[Tuple]) -> str:
    lines = ("| " + " | ".join(header) + " |", "|" + "|".join("---" for _ in header) + "|",
             *("| " + " | ".join(cell(c) for c in r) + " |" for r in rows))
    return "\n".join(lines)


def domain(i: str) -> str:
    return i.split("-")[1] if i.count("-") >= 2 else "?"


def load(data: Path):
    jl = lambda p: tuple(json.loads(l) for l in (data / p).read_text(encoding="utf-8").splitlines() if l.strip())
    cv = lambda p: tuple(csv.DictReader((data / p).open(encoding="utf-8")))
    return dict(F=cv("findings.csv"), C=cv("conflicts.csv"), S=cv("scenarios.csv"), D=cv("decisions.csv"),
                E=cv("trace-edges.csv"), Q=cv("requirements-quality.csv"), R=jl("refinements.jsonl"),
                G=jl("gap-requirements.jsonl"), U=jl("ui-requirements.jsonl") if (data / "ui-requirements.jsonl").exists() else (),
                UG=cv("ui-component-gaps.csv") if (data / "ui-component-gaps.csv").exists() else (), K=cv("counsel-positions.csv") if (data / "counsel-positions.csv").exists() else (), V=json.loads((data / "validation-report.json").read_text()))


def g_findings_matrix(d) -> str:
    c = Counter((domain(f["finding_id"]), f["severity"]) for f in d["F"])
    rows = tuple((dm, *(c[(dm, s)] for s in SEV), sum(c[(dm, s)] for s in SEV)) for dm in DOMAINS)
    tot = ("**Total**", *(sum(c[(dm, s)] for dm in DOMAINS) for s in SEV), len(d["F"]))
    return table(("Domain", *SEV, "Total"), rows + (tot,))


def g_findings_dimension(d) -> str:
    c = Counter(f["dimension"] for f in d["F"])
    return table(("Dimension", "Findings"), sorted(c.items(), key=lambda kv: -kv[1]))


def g_findings_severe(d) -> str:
    rows = sorted((f for f in d["F"] if f["severity"] in ("critical", "high")), key=lambda f: (SEV.index(f["severity"]), f["finding_id"]))
    return table(("ID", "Sev.", "Dimension", "Finding", "Affected", "Proposed resolution"),
                 ((f["finding_id"], f["severity"], f["dimension"], f["title"], cell(f["affected_ids"], 90), cell(f["proposed_resolution"], 200)) for f in rows))


def g_findings_other(d) -> str:
    rows = sorted((f for f in d["F"] if f["severity"] in ("medium", "low")), key=lambda f: (SEV.index(f["severity"]), f["finding_id"]))
    return table(("ID", "Sev.", "Dimension", "Finding", "Affected"),
                 ((f["finding_id"], f["severity"], f["dimension"], f["title"], cell(f["affected_ids"], 90)) for f in rows))


def g_conflicts(d) -> str:
    return table(("ID", "Requirement A", "Requirement B", "Incompatibility", "Recommended resolution", "Human authority"),
                 ((c["conflict_id"], cell(c["requirement_a"], 80), cell(c["requirement_b"], 80), cell(c["incompatibility"], 260),
                   cell(c["recommended_resolution"], 220), c.get("requires_human_authority", "")) for c in d["C"]))


def g_gaps(d) -> str:
    rows = sorted(d["G"], key=lambda g: (g["priority"], g["id"]))
    return table(("ID", "Pri.", "Parent", "Requirement (normative)", "Gap finding", "Why no predecessor"),
                 ((g["id"], g["priority"], cell(g["parent"], 60), cell(g["statement"], 300), g["gap_finding"], cell(g["why_no_predecessor"], 140)) for g in rows))


def g_gap_counts(d) -> str:
    c = Counter((domain(g["id"].replace("UNB-RCN-", "X-")), g["priority"]) for g in d["G"])
    doms = sorted({k[0] for k in c})
    return table(("Domain", "P0", "P1", "P2", "Total"), tuple((dm, c[(dm, "P0")], c[(dm, "P1")], c[(dm, "P2")], sum(c[(dm, p)] for p in ("P0", "P1", "P2"))) for dm in doms))


def g_refinement_index(d) -> str:
    return table(("Refinement", "Original", "Relation", "Domain", "Decision status", "Confidence", "ACs"),
                 ((r["refinement_id"], f'{r["original_id"]} ({r["original_path"].split("/")[-1]}:{r["original_line"]})', r["relationship"],
                   r.get("_domain", ""), r["decision_status"], r["confidence"], len(r["acceptance_criteria"])) for r in d["R"]))


def fmt_list(v) -> str:
    if isinstance(v, dict):
        return "; ".join(f"**{k}:** {fmt_list(x)}" for k, x in v.items())
    if isinstance(v, list):
        return "; ".join(fmt_list(x) for x in v) if v else "—"
    return str(v)


def g_refinement_records(d) -> str:
    def one(r) -> str:
        acs = "\n".join(f"  - **{a['id']}** Given {a['given']}; when {a['when']}; then {a['then']}." for a in r["acceptance_criteria"])
        return "\n".join((
            f"### {r['refinement_id']} — {r['relationship']} `{r['original_id']}`",
            f"- **Source:** `{r['original_path']}:{r['original_line']}` @ `{r['original_revision']}` · domain {r.get('_domain','')} · "
            f"{r['decision_status']} · confidence {r['confidence']} · {r['validation_status']}",
            f"- **Original text:** {cell(r['original_text'], 900)}",
            f"- **Refined statement:** {r['statement']}",
            f"- **Business objective:** {r['business_objective']}",
            f"- **Reason for change:** {r['reason_for_change']}",
            f"- **Processing rules:** {fmt_list(r['processing_rules'])}",
            f"- **Failure behaviour:** {fmt_list(r['failure_behavior'])}",
            f"- **Security / audit:** {fmt_list(r['security'])} / {fmt_list(r['auditability'])}",
            f"- **Configuration:** {fmt_list(r.get('configuration'))}",
            f"- **AI assistance:** {fmt_list(r.get('ai_assistance'))}",
            f"- **UI / API / reporting:** {fmt_list(r.get('ui_implications'))} / {fmt_list(r.get('api_implications'))} / {fmt_list(r.get('reporting_implications'))}",
            f"- **Regulatory:** {fmt_list(r.get('regulatory_implications'))}",
            f"- **Dependencies:** {fmt_list(r.get('dependencies'))} · **Findings:** {fmt_list(r.get('related_findings'))}",
            f"- **Assumptions / alternatives / tradeoffs:** {fmt_list(r.get('assumptions'))} / {fmt_list(r.get('alternatives_considered'))} / {fmt_list(r.get('tradeoffs'))}",
            f"- **Evidence:** {fmt_list(r.get('evidence'))}",
            "- **Acceptance criteria:**", acs, ""))
    return "\n".join(one(r) for r in d["R"])


def g_trace_summary(d) -> str:
    c = Counter(e["relationship"] for e in d["E"])
    return table(("Relationship", "Edges"), sorted(c.items(), key=lambda kv: -kv[1]) + [("**Total**", len(d["E"]))])


def g_trace_coverage(d) -> str:
    tested = {e["source"] for e in d["E"] if e["relationship"] == "tested_by"}
    affected = {e["target"] for e in d["E"] if e["relationship"] == "affects"}
    refined = {r["original_id"] for r in d["R"]}
    base = {q["identifier"] for q in d["Q"]}
    new = [r["refinement_id"] for r in d["R"]] + [g["id"] for g in d["G"]]
    rows = (
        ("Baseline canonical requirements (denominator)", len(base), ""),
        ("… affected by at least one finding", len(base & affected), f"{100*len(base & affected)/len(base):.1f}%"),
        ("… with a refinement record", len(base & refined), f"{100*len(base & refined)/len(base):.1f}%"),
        ("… tested_by at least one scenario (baseline + reconciliation scenarios)", len(base & tested), f"{100*len(base & tested)/len(base):.1f}%"),
        ("Reconciliation refinements + gap requirements (denominator)", len(new), ""),
        ("… tested_by at least one scenario", sum(1 for n in new if n in tested), f"{100*sum(1 for n in new if n in tested)/len(new):.1f}%"),
    )
    return table(("Measure", "Count", "Share"), rows)


def g_scenarios(d) -> str:
    return table(("ID", "Scenario", "Requirements", "Stimulus", "Expected states", "Financial invariants", "Recovery"),
                 ((s["scenario_id"], s["title"], cell(s["requirement_ids"], 70), cell(s["stimulus"], 140), cell(s["expected_states"], 160),
                   cell(s.get("financial_invariants", ""), 120), cell(s.get("recovery_behavior", ""), 110)) for s in d["S"]))


def g_scenario_counts(d) -> str:
    c = Counter(s["scenario_id"].split("-")[1] for s in d["S"])
    return table(("Domain", "Scenarios"), sorted(c.items()) + [("**Total**", len(d["S"]))])


def g_decisions(d) -> str:
    return table(("ID", "Type", "Decision", "Status", "Decided option", "Validation still required", "Owner to confirm", "Linked records"),
                 ((x["decision_id"], x["type"], x["question"], x.get("status", ""), cell(x.get("decided_option", x["recommendation"]), 420),
                   x.get("validation_required", ""), x["decision_owner"], cell(x["affected_ids"], 120)) for x in d["D"]))


def g_counsel(d) -> str:
    return table(("Question", "Topic", "Provisional working position", "Status"),
                 ((q["question_id"], q["topic"], q["provisional_position"], q["status"]) for q in d["K"]))


def g_validation(d) -> str:
    v = d["V"]
    rows = tuple((k, n) for k, n in v["records"].items()) + (("trace edges", v["trace_edges"]), ("**errors**", v["errors"]), ("warnings", v["warnings"]))
    return table(("Record set / result", "Count"), rows)


def g_ui_index(d) -> str:
    return table(("ID", "Kind", "Title", "Forma components", "Folio elements", "Capability gaps", "ACs"),
                 ((u["id"], u["kind"], u["title"], cell(", ".join(u["forma_components"]), 160), cell(", ".join(u["folio_elements"]), 120),
                   ", ".join(u["capability_gaps"]) or "—", len(u["acceptance_criteria"])) for u in d["U"]))


def g_ui_records(d) -> str:
    def one(u) -> str:
        acs = "\n".join(f"  - **{a['id']}** Given {a['given']}; when {a['when']}; then {a['then']}." for a in u["acceptance_criteria"])
        rows = [f"### {u['id']} — {u['title']} ({u['kind']})", f"- **Requirement:** {u['statement']}",
                f"- **Traces to:** {', '.join(u['traces'])}"]
        rows += [f"- **Actors:** {', '.join(u['actors'])}"] if u["actors"] else []
        rows += [f"- **Forma components:** {', '.join(f'`{c}`' for c in u['forma_components'])}"] if u["forma_components"] else []
        rows += [f"- **Folio elements:** {', '.join(f'`{c}`' for c in u['folio_elements'])}"] if u["folio_elements"] else []
        rows += [f"- **States:** {'; '.join(u['states'])}"] if u["states"] else []
        rows += [f"- **Actions:** {'; '.join(u['actions'])}"] if u["actions"] else []
        rows += [f"- **Responsive:** {u['responsive']}"] if u["responsive"] else []
        rows += [f"- **Capability gaps:** {', '.join(u['capability_gaps'])}"] if u["capability_gaps"] else []
        return "\n".join(rows + ["- **Acceptance criteria:**", acs, ""])
    return "\n".join(one(u) for u in d["U"])


def g_ui_gaps(d) -> str:
    return table(("Gap", "Component", "Owner", "Priority (owner's catalog)", "Need", "Release-1 workaround", "Acceptance"),
                 ((g["gap_id"], g["component"], g["owner"], g["forma_priority"], cell(g["need"], 200), cell(g["workaround_r1"], 220), cell(g["acceptance"], 200)) for g in d["UG"]))


GENERATORS: Dict[str, Callable] = {
    "ui-index": g_ui_index, "ui-records": g_ui_records, "ui-gaps": g_ui_gaps,
    "findings-matrix": g_findings_matrix, "findings-dimension": g_findings_dimension,
    "findings-severe": g_findings_severe, "findings-other": g_findings_other,
    "conflicts": g_conflicts, "gaps": g_gaps, "gap-counts": g_gap_counts,
    "refinement-index": g_refinement_index, "refinement-records": g_refinement_records,
    "trace-summary": g_trace_summary, "trace-coverage": g_trace_coverage,
    "scenarios": g_scenarios, "scenario-counts": g_scenario_counts,
    "decisions": g_decisions, "counsel": g_counsel, "validation": g_validation,
}
BLOCK = re.compile(r"(<!-- BEGIN GENERATED: ([a-z-]+) -->)(.*?)(<!-- END GENERATED: \2 -->)", re.S)


def render(text: str, d) -> str:
    return BLOCK.sub(lambda m: f"{m.group(1)}\n{GENERATORS[m.group(2)](d)}\n{m.group(4)}", text)


def main(argv) -> int:
    root = Path(argv[1])
    d = load(root / RECON / "data")
    for p in sorted((root / RECON).glob("*.md")):
        before = p.read_text(encoding="utf-8")
        after = render(before, d)
        if after != before:
            p.write_text(after, encoding="utf-8")
            print(f"rendered {p.name}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

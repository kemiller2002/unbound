#!/usr/bin/env python3
"""Render data-driven tables into the cross-industry discovery Markdown reports.

Same convention as render.py: hand-written narrative lives in the reports; content between
    <!-- BEGIN GENERATED: <name> --> ... <!-- END GENERATED: <name> -->
is replaced from the data files, so summaries cannot drift from the records.

Usage: python3 -I -B tools/reqaudit/render_discovery.py <repo-root> [mission-dir]
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Callable, Dict, Iterable, Tuple

MISSION = Path("requirements/discovery/2026-11-23-cross-industry")
PRIORITIES = ("P0", "P1", "P2", "P3", "P4")
COVERAGE = ("covered", "partial", "contradictory", "missing")
GROUP_NAME = {"bnk": "Banking & payments", "inv": "Investment & accounting", "svc": "Service, claims, healthcare",
              "erp": "ERP, CRM, trust & estate", "grt": "Grantmaking & regulatory", "sas": "SaaS, IAM, records, workflow, other",
              "eco": "Operating economics", "eva": "Event architecture", "adv": "Adversarial review", "int": "Integration"}


def cell(text, limit: int = 200) -> str:
    t = text if isinstance(text, str) else (", ".join(map(str, text)) if isinstance(text, (list, tuple)) else str(text or ""))
    t = re.sub(r"\s+", " ", t).replace("|", "\\|").strip()
    return t if len(t) <= limit else t[: limit - 1].rstrip() + "…"


def table(header: Tuple[str, ...], rows: Iterable[Tuple], limit: int = 200) -> str:
    return "\n".join(("| " + " | ".join(header) + " |", "|" + "|".join("---" for _ in header) + "|",
                      *("| " + " | ".join(cell(c, limit) for c in r) + " |" for r in rows)))


def load(data: Path) -> Dict[str, tuple]:
    jl = lambda p: tuple(json.loads(l) for l in (data / p).read_text(encoding="utf-8").splitlines() if l.strip()) if (data / p).exists() else ()
    cv = lambda p: tuple(csv.DictReader((data / p).open(encoding="utf-8"))) if (data / p).exists() else ()
    return dict(A=jl("capability-atlas.jsonl"), G=jl("gap-requirements.jsonl"), R=jl("refinements.jsonl"),
                F=cv("findings.csv"), C=cv("conflicts.csv"), S=cv("scenarios.csv"), D=cv("decisions.csv"),
                X=jl("rejected.jsonl"), FC=jl("failure-catalog.jsonl"), EV=jl("event-matrix.jsonl"),
                RA=jl("regulatory-applicability.jsonl"), BM=jl("business-models.jsonl"), E=cv("trace-edges.csv"),
                PC=jl("priority-challenges.jsonl"), O=jl("decision-outcomes.jsonl"), SC=jl("release1-scope.jsonl"), SP=jl("scope-proposals.jsonl"), DU=cv("duplicates.csv"),
                V=json.loads((data / "validation-report.json").read_text()) if (data / "validation-report.json").exists() else {})


def grp(r: dict) -> str:
    return GROUP_NAME.get(r.get("_group", ""), r.get("_group", "?"))


# ---- generators (pure: data -> markdown) ----

def g_counts(d) -> str:
    rows = (("Capability atlas rows", len(d["A"])), ("Findings", len(d["F"])), ("New requirements (UNB-XIN-*)", len(d["G"])),
            ("Refinements of originals (RR-*)", len(d["R"])), ("Scenarios", len(d["S"])), ("Conflicts", len(d["C"])),
            ("Open decisions", len(d["D"])), ("Rejected recommendations", len(d["X"])), ("Failure catalog rows", len(d["FC"])),
            ("Event authority matrix rows", len(d["EV"])), ("Regulatory applicability rows", len(d["RA"])),
            ("Business-model rows", len(d["BM"])), ("Trace edges (generated)", len(d["E"])))
    return table(("Record set", "Count"), rows)


def g_by_group(d) -> str:
    keys = tuple(dict.fromkeys(r.get("_group") for r in (*d["A"], *d["G"])))
    cnt = lambda recs, g: sum(1 for r in recs if r.get("_group") == g)
    return table(("Research pass", "Atlas", "Findings", "New reqs", "Refinements", "Scenarios", "Rejected", "Failures"),
                 tuple((GROUP_NAME.get(g, g), cnt(d["A"], g), cnt(d["F"], g), cnt(d["G"], g), cnt(d["R"], g),
                        cnt(d["S"], g), cnt(d["X"], g), cnt(d["FC"], g)) for g in keys))


SECTIONS = (  # free-text industry label -> mission section; first match wins
    (("finops", "tdabc", "management accounting", "lean", "six sigma", "process analytics", "rpa", "pricing", "benchmark"), "§18 Operating economics analogues"),
    (("event", "architecture", "data platform", "model risk"), "§19 Event architecture analogues"),
    (("payment", "merchant", "nacha"), "I Payments, merchant processing, billing"),
    (("subscription", "telecom", "utility billing"), "M Subscription billing"),
    (("bank",), "A Banking and financial services"),
    (("invest", "wealth", "custody", "brokerage", "fund admin", "transfer agency", "market data", "endowment"), "B Investment and wealth / M custody, fund admin"),
    (("accounting", "ledger", "financial reporting"), "C Accounting, GL, fund accounting"),
    (("call cent", "contact cent", "customer service", "service management"), "D Call centers and customer service"),
    (("insurance", "claims"), "E Insurance and claims"),
    (("healthcare",), "F Healthcare administration"),
    (("legal case", "bpo"), "M Legal case mgmt, BPO, fraud investigation"),
    (("erp", "enterprise resource", "shared services", "performance management"), "G Enterprise resource planning"),
    (("crm", "relationship"), "H CRM, sales, relationship management"),
    (("nonprofit", "grantmaking", "cross-border", "charity data"), "J Nonprofit administration and grantmaking"),
    (("trust", "estate", "family"), "K Trust, estate, family office"),
    (("saas", "api platform", "multi-tenant"), "L Enterprise SaaS and platform operations"),
    (("identity",), "M Identity and access management"),
    (("records",), "M Records management"),
    (("workflow",), "M Enterprise workflow automation"),
    (("regulatory", "government", "tax administration"), "§9 / M Regulatory, tax and benefits administration"),
)


def section(industry: str) -> str:
    low = (industry or "").lower()
    return next((name for keys, name in SECTIONS if any(k in low for k in keys)), "M Other industries")


def g_coverage_by_pass(d) -> str:
    keys = tuple(dict.fromkeys(a.get("_group") for a in d["A"]))
    c = Counter((a.get("_group"), a.get("coverage")) for a in d["A"])
    rows = tuple((GROUP_NAME.get(k, k), *(c[(k, v)] for v in COVERAGE), sum(c[(k, v)] for v in COVERAGE)) for k in keys)
    return table(("Research pass", *COVERAGE, "Total"), rows)


def g_coverage_matrix(d) -> str:
    inds = tuple(sorted({section(a.get("industry", "")) for a in d["A"]}))
    c = Counter((section(a.get("industry", "")), a.get("coverage")) for a in d["A"])
    rows = tuple((i, *(c[(i, k)] for k in COVERAGE), sum(c[(i, k)] for k in COVERAGE)) for i in inds)
    tot = ("**Total**", *(sum(c[(i, k)] for i in inds) for k in COVERAGE), len(d["A"]))
    return table(("Industry", *COVERAGE, "Total"), rows + (tot,), 80)


def g_necessity(d) -> str:
    c = Counter((a.get("necessity"), a.get("coverage")) for a in d["A"])
    ns = tuple(sorted({a.get("necessity") for a in d["A"]}))
    return table(("Necessity", *COVERAGE), tuple((n, *(c[(n, k)] for k in COVERAGE)) for n in ns))


def g_automation(d) -> str:
    c = Counter(a.get("automation") for a in d["A"])
    return table(("Automation class", "Capabilities"), sorted(c.items(), key=lambda kv: -kv[1]))


def g_atlas(d) -> str:
    return table(("ID", "Industry", "Capability", "DAF equiv.", "Necessity", "Coverage", "Existing IDs", "Automation", "Analogy breaks down"),
                 tuple((a["id"], a.get("industry"), a.get("capability"), a.get("daf_equivalent"), a.get("necessity"),
                        a.get("coverage"), a.get("existing_ids", ()), a.get("automation"), a.get("analogy_breaks_down"))
                       for a in d["A"]), 140)


def g_gaps_attention(d) -> str:
    rows = tuple(a for a in d["A"] if a.get("coverage") in ("missing", "contradictory"))
    return table(("ID", "Industry", "Capability", "Coverage", "Necessity", "Business problem"),
                 tuple((a["id"], a.get("industry"), a.get("capability"), a.get("coverage"), a.get("necessity"), a.get("business_problem"))
                       for a in rows), 160)


def g_priority_counts(d) -> str:
    keys = tuple(dict.fromkeys(g.get("_group") for g in d["G"]))
    c = Counter((g.get("_group"), g.get("priority")) for g in d["G"])
    rows = tuple((GROUP_NAME.get(k, k), *(c[(k, p)] for p in PRIORITIES), sum(c[(k, p)] for p in PRIORITIES)) for k in keys)
    tot = ("**Total**", *(sum(c[(k, p)] for k in keys) for p in PRIORITIES), len(d["G"]))
    return table(("Research pass", *PRIORITIES, "Total"), rows + (tot,))


def g_register(d) -> str:
    order = {p: i for i, p in enumerate(PRIORITIES)}
    rows = sorted(d["G"], key=lambda g: (order.get(g.get("priority"), 9), g["id"]))
    pri = lambda g: g.get("priority") + (f" (was {g['priority_as_proposed']})" if g.get("priority_as_proposed", g.get("priority")) != g.get("priority") else "")
    return table(("ID", "P", "Parent capability", "Statement", "Applicability", "Discovery source", "Priority justification", "Adversarial disposition", "Links"),
                 tuple((g["id"], pri(g), g.get("parent_capability"), g.get("statement"), g.get("applicability"),
                        g.get("discovery_source"), g.get("priority_revision") or g.get("priority_justification"),
                        g.get("adversarial_disposition", ""), g.get("links", ())) for g in rows), 260)


def g_priority_shift(d) -> str:
    c = Counter((g.get("priority_as_proposed", g.get("priority")), g.get("priority")) for g in d["G"])
    return table(("As proposed by research pass", *PRIORITIES, "Total"),
                 tuple((p, *(c[(p, q)] for q in PRIORITIES), sum(c[(p, q)] for q in PRIORITIES)) for p in PRIORITIES))


def g_refinements(d) -> str:
    return table(("Refinement", "Original", "Relationship", "Original path:line", "Reason for change", "Decision status"),
                 tuple((r["refinement_id"], r["original_id"], r.get("relationship"), f"{r.get('original_path')}:{r.get('original_line')}",
                        r.get("reason_for_change"), r.get("decision_status")) for r in d["R"]), 260)


def g_refinement_detail(d) -> str:
    def one(r) -> str:
        ac = "\n".join(f"  - **{a.get('id', 'AC')}** Given {a.get('given')}; when {a.get('when')}; then {a.get('then')}."
                       for a in r.get("acceptance_criteria", ()))
        return (f"### {r['refinement_id']} ({r.get('relationship')} {r['original_id']})\n\n"
                f"- **Original** (`{r.get('original_path')}:{r.get('original_line')}` @ `{r.get('original_revision')}`): {cell(r.get('original_text'), 600)}\n"
                f"- **Refined statement:** {cell(r.get('statement'), 1200)}\n"
                f"- **Reason:** {cell(r.get('reason_for_change'), 500)}\n"
                f"- **Decision status:** {r.get('decision_status')} · **Confidence:** {r.get('confidence')} · **Findings:** {cell(r.get('related_findings', ()))}\n"
                f"- **Acceptance criteria:**\n{ac}\n")
    return "\n".join(one(r) for r in d["R"])


def g_findings_severity(d) -> str:
    keys = tuple(dict.fromkeys(f["finding_id"].split("-")[1] for f in d["F"]))
    sev = ("critical", "high", "medium", "low")
    c = Counter((f["finding_id"].split("-")[1], f["severity"]) for f in d["F"])
    rows = tuple((k, *(c[(k, s)] for s in sev), sum(c[(k, s)] for s in sev)) for k in keys)
    return table(("Domain", *sev, "Total"), rows + (("**Total**", *(sum(c[(k, s)] for k in keys) for s in sev), len(d["F"])),))


def g_findings_severe(d) -> str:
    rows = tuple(f for f in d["F"] if f["severity"] in ("critical", "high"))
    return table(("Finding", "Severity", "Title", "Affected", "Proposed resolution"),
                 tuple((f["finding_id"], f["severity"], f["title"], f["affected_ids"], f["proposed_resolution"]) for f in rows), 220)


def g_conflicts(d) -> str:
    return table(("Conflict", "A", "B", "Incompatibility", "Recommended resolution", "Human authority"),
                 tuple((c["conflict_id"], c["requirement_a"], c["requirement_b"], c["incompatibility"], c["recommended_resolution"],
                        c.get("requires_human_authority")) for c in d["C"]), 240)


def g_duplicates(d) -> str:
    rows = tuple(f for f in d["F"] if f["title"].lower().startswith("duplicate"))
    return table(("Finding", "Canonical, then overlapping", "Topic", "Resolution"),
                 tuple((f["finding_id"], f["affected_ids"], f["title"].split(":", 1)[-1], f["proposed_resolution"]) for f in rows), 320)


def g_regulatory(d) -> str:
    return table(("ID", "Topic", "Binding", "Authority", "Citation", "Applies to (entity)", "Activity", "Models", "Confidence", "Counsel question"),
                 tuple((r["id"], r.get("topic"), r.get("binding"), r.get("authority_type"), r.get("citation"),
                        r.get("applies_to_entity"), r.get("applies_to_activity"), r.get("business_models"),
                        r.get("confidence"), r.get("counsel_question")) for r in d["RA"]), 160)


def g_failures(d) -> str:
    return table(("ID", "Capability", "Failure mode", "Scenario", "Detection", "Recovery", "Escalation", "Covered by / new"),
                 tuple((f["id"], f.get("capability"), f.get("failure_mode"), f.get("scenario"), f.get("detection"), f.get("recovery"),
                        f.get("escalation"), list(f.get("covered_by", ())) + list(f.get("new_ids", ()))) for f in d["FC"]), 150)


def g_failure_modes(d) -> str:
    c = Counter(str(f.get("failure_mode", "?")).split(";")[0].strip().lower() for f in d["FC"])
    return table(("Failure mode", "Catalogued cases"), sorted(c.items(), key=lambda kv: -kv[1]))


def g_rejected(d) -> str:
    return table(("ID", "Idea", "Source industry", "Argument for", "Argument against", "Reason rejected", "Revisit if"),
                 tuple((r["id"], r.get("idea"), r.get("source_industry"), r.get("argue_for"), r.get("argue_against"),
                        r.get("reason_rejected"), r.get("revisit_if")) for r in d["X"]), 170)


def g_events(d) -> str:
    return table(("ID", "Event", "Capability", "Owner", "Authority", "Store", "Consistency", "Idempotency key", "Correction"),
                 tuple((e["id"], e.get("event"), e.get("business_capability"), e.get("owning_module"), e.get("authority"),
                        e.get("source_of_truth_store"), e.get("consistency"), e.get("idempotency_key"), e.get("correction_pattern"))
                       for e in d["EV"]), 110)


def g_event_summary(d) -> str:
    auth = ("authoritative-financial", "authoritative-nonfinancial", "derived", "informational", "observation-ai")
    caps = tuple(dict.fromkeys(e.get("business_capability") for e in d["EV"]))
    c = Counter((e.get("business_capability"), e.get("authority")) for e in d["EV"])
    s = Counter((e.get("business_capability"), e.get("consistency")) for e in d["EV"])
    return table(("Capability", *auth, "synchronous", "eventual"),
                 tuple((k, *(c[(k, a)] for a in auth), s[(k, "synchronous")], s[(k, "eventual")]) for k in caps))


def g_models(d) -> str:
    return table(("Model", "Distinguishing capabilities", "Configuration needed", "Unsupported or risky", "Confidence"),
                 tuple((m.get("model"), m.get("distinguishing_capabilities"), m.get("configuration_needed"),
                        m.get("unsupported_or_risky"), m.get("confidence")) for m in d["BM"]), 260)


def g_decisions(d) -> str:
    return table(("Decision", "Type", "Question", "Owner", "Recommendation", "Blocks impl.", "Status"),
                 tuple((x["decision_id"], x.get("type"), x["question"], x["decision_owner"], x["recommendation"],
                        x.get("blocks_implementation"), x["status"]) for x in d["D"]), 220)


def g_decision_types(d) -> str:
    c = Counter(x.get("type") for x in d["D"])
    return table(("Type", "Open decisions"), sorted(c.items(), key=lambda kv: -kv[1]))


def g_scenarios(d) -> str:
    return table(("Scenario", "Title", "Requirements", "Stimulus", "Expected states"),
                 tuple((s["scenario_id"], s["title"], s["requirement_ids"], s["stimulus"], s["expected_states"]) for s in d["S"]), 180)


def g_economics(d) -> str:
    rows = tuple(g for g in d["G"] if g["id"].startswith("UNB-XIN-ECO-"))
    def acs(g):
        return " ".join(f"[{a.get('id')}] Given {a.get('given')}; when {a.get('when')}; then {a.get('then')}." for a in g.get("acceptance_criteria", ()))
    return table(("ID", "P", "Requirement", "Acceptance criteria", "UI / reporting implications"),
                 tuple((g["id"], g.get("priority"), g.get("statement"), acs(g), f"{g.get('ui_implications', '')} {g.get('reporting_implications', '')}")
                       for g in rows), 600)


def g_eva_reqs(d) -> str:
    rows = tuple(g for g in d["G"] if g["id"].startswith("UNB-XIN-EVA-"))
    return table(("ID", "P", "Requirement", "Links"), tuple((g["id"], g.get("priority"), g.get("statement"), g.get("links", ())) for g in rows), 500)


def g_priority_challenges(d) -> str:
    return table(("Requirement", "Current", "Proposed", "Reason"),
                 tuple((p.get("id"), p.get("current_priority"), p.get("proposed_priority"), p.get("reason")) for p in d["PC"]), 260)


def g_adv_findings(d) -> str:
    rows = tuple(f for f in d["F"] if f["finding_id"].startswith("XIF-ADV-"))
    return table(("Finding", "Severity", "Title", "Affected", "Proposed resolution"),
                 tuple((f["finding_id"], f["severity"], f["title"], f["affected_ids"], f["proposed_resolution"]) for f in rows), 260)


def g_adv_conflicts(d) -> str:
    return table(("Conflict", "A", "B", "Incompatibility", "Recommended resolution"),
                 tuple((c["conflict_id"], c["requirement_a"], c["requirement_b"], c["incompatibility"], c["recommended_resolution"])
                       for c in d["C"] if c["conflict_id"].startswith("XIC-ADV-")), 300)


def g_adv_decisions(d) -> str:
    return table(("Decision", "Question", "Recommendation"),
                 tuple((x["decision_id"], x["question"], x["recommendation"]) for x in d["D"] if x["decision_id"].startswith("XID-ADV-")), 300)


def g_adv_requirements(d) -> str:
    return table(("ID", "P", "Statement", "Business problem"),
                 tuple((g["id"], g.get("priority"), g.get("statement"), g.get("business_problem")) for g in d["G"] if g["id"].startswith("UNB-XIN-ADV-")), 600)


def g_outcome_summary(d) -> str:
    q = {x["decision_id"]: x for x in d["D"]}
    c = Counter((q.get(o["decision_id"], {}).get("type"), o.get("confidence")) for o in d["O"])
    types = tuple(sorted({k[0] for k in c})); confs = tuple(sorted({k[1] for k in c}))
    return table(("Type", *confs, "Total"), tuple((t, *(c[(t, k)] for k in confs), sum(c[(t, k)] for k in confs)) for t in types))


def g_outcomes(d) -> str:
    q = {x["decision_id"]: x for x in d["D"]}
    def one(o) -> str:
        x = q.get(o["decision_id"], {})
        auth = "\n".join(f"  - {a}" for a in o.get("authority", ()))
        cons = "\n".join(f"  - {a}" for a in o.get("requirement_consequences", ()))
        return (f"### {o['decision_id']} ({x.get('type', '?')})\n\n"
                f"**Question.** {cell(x.get('question'), 600)}\n\n"
                f"**Decision.** {o['decision']}\n\n"
                f"**Rationale.** {o['rationale']}\n\n"
                f"- **Confidence:** {o.get('confidence')} · **Status:** {o.get('status')} · **Confirmation required:** {o.get('confirmation_required')}\n"
                f"- **Authority:**\n{auth}\n"
                f"- **Requirement consequences:**\n{cons}\n")
    order = lambda o: (q.get(o["decision_id"], {}).get("type", ""), o["decision_id"])
    return "\n".join(one(o) for o in sorted(d["O"], key=order))


def g_applied(d) -> str:
    recs = {**{g["id"]: g for g in d["G"]}, **{r["refinement_id"]: r for r in d["R"]}}
    rows = tuple((rid, ", ".join(r.get("decisions_applied", ())), r.get("statement"))
                 for rid, r in sorted(recs.items()) if r.get("decisions_applied"))
    return table(("Requirement", "Decisions applied", "Statement as revised"), rows, 900)


def g_scope_summary(d) -> str:
    scopes = ("R1", "R1-conditional", "Deferred", "Rejected-recommended")
    c = Counter((x["priority"] or "refinement", x["scope"]) for x in d["SC"])
    pris = tuple(sorted({k[0] for k in c}))
    rows = tuple((p, *(c[(p, sc)] for sc in scopes), sum(c[(p, sc)] for sc in scopes)) for p in pris)
    tot = ("**Total**", *(sum(c[(p, sc)] for p in pris) for sc in scopes), len(d["SC"]))
    return table(("Priority", *scopes, "Total"), rows + (tot,))


def g_scope_out(d) -> str:
    rows = tuple(x for x in d["SC"] if x["scope"] != "R1")
    return table(("Requirement", "Priority", "Scope", "Reason"), tuple((x["id"], x["priority"], x["scope"], x["reason"]) for x in rows), 200)


def g_scope_in(d) -> str:
    stmt = {**{g["id"]: g for g in d["G"]}, **{r["refinement_id"]: r for r in d["R"]}}
    rows = tuple(x for x in d["SC"] if x["scope"] == "R1")
    order = {p: i for i, p in enumerate(PRIORITIES)}
    rows = sorted(rows, key=lambda x: (order.get(x["priority"], 9), x["id"]))
    return table(("Requirement", "Priority", "Statement"), tuple((x["id"], x["priority"], stmt[x["id"]].get("statement")) for x in rows), 180)


def g_scope_proposals(d) -> str:
    q = {x["decision_id"]: x for x in d["D"]}
    return table(("Decision", "Question", "Proposed answer", "Effect"),
                 tuple((p["decision_id"], q.get(p["decision_id"], {}).get("question"), p["proposed_answer"], p["effect"]) for p in d["SP"]), 700)


def g_trace(d) -> str:
    c = Counter(e["relationship"] for e in d["E"])
    return table(("Relationship", "Edges"), sorted(c.items(), key=lambda kv: -kv[1]))


def g_validation(d) -> str:
    v = d["V"]
    return table(("Measure", "Value"), (("Errors", v.get("errors")), ("Warnings", v.get("warnings")), ("Trace edges", v.get("trace_edges")),
                                        *((f"Records: {k}", n) for k, n in v.get("records", {}).items())))


GENERATORS: Dict[str, Callable] = {k[2:].replace("_", "-"): f for k, f in globals().items() if k.startswith("g_") and callable(f)}
BLOCK = re.compile(r"(<!-- BEGIN GENERATED: ([a-z0-9-]+) -->\n)(.*?)(<!-- END GENERATED: \2 -->)", re.S)


def render(text: str, d) -> str:
    return BLOCK.sub(lambda m: m.group(1) + GENERATORS[m.group(2)](d) + "\n" + m.group(4), text)


def main(argv) -> int:
    root = Path(argv[1])
    mission = root / (Path(argv[2]) if len(argv) > 2 else MISSION)
    d = load(mission / "data")
    unknown = sorted({m.group(2) for p in mission.glob("*.md") for m in BLOCK.finditer(p.read_text(encoding="utf-8"))} - set(GENERATORS))
    if unknown:
        print("unknown generated blocks:", unknown)
        return 1
    for p in sorted(mission.glob("*.md")):
        old = p.read_text(encoding="utf-8")
        new = render(old, d)
        if new != old:
            p.write_text(new, encoding="utf-8")
            print("rendered", p.name)
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

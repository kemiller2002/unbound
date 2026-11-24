#!/usr/bin/env python3
"""Adopt the master-mission convergence clauses into the requirements baseline (Phase 2).

Rules (same discipline as tools/reqaudit/adopt.py):
  * Original requirement text is never changed. Each amended requirement defined in Markdown gets a one-line
    annotation directly after its definition naming the converging clause(s); each amended JSONL record gets a
    `converged_by` list and a `revision_history` entry (statement text unchanged).
  * Each target domain file receives (or has regenerated) one section between
        <!-- BEGIN ADOPTED: convergence-2026-11-24 --> ... <!-- END ADOPTED: convergence-2026-11-24 -->
  * Every question a clause resolves gets an `applied-to-requirement` event in the resolution log.
  * The authoritative baseline index is regenerated: requirements/baseline/baseline.jsonl and
    requirements/AUTHORITATIVE-BASELINE.md.
  * Idempotent: re-running produces identical files and appends no new events.

Usage: python3 -I -B tools/reqaudit/adopt_convergence.py <repo-root>
"""
import csv
import hashlib
import os
import importlib
import io
import json
import re
import sys
from collections import Counter
from functools import reduce
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import questions_lib as ql  # noqa: E402

MISSION = Path("requirements/convergence/2026-11-24-master")
TAG = "convergence-2026-11-24"
BEGIN, END = f"<!-- BEGIN ADOPTED: {TAG} -->", f"<!-- END ADOPTED: {TAG} -->"
NOTE = "Converged 2026-11-24"
ON = "2026-11-24"
ACTOR = "Master mission agent (convergence; product-owner directions and mission-adopted evidence, owner ratification pending where agent-recommended)"
CLAUSE_MODULES = ("c1_finance_payments", "c2_grants_law", "c3_governance_regulatory", "c4_platform_product",
                  "c5_adversarial_business", "c6_adversarial_technical")
AMENDMENT_MODULES = ("c7_amendments", "c8_owner_decisions")  # applied in order; each records revision history
RECORD_FILES = ("requirements/reconciliation/2026-11-23/data/refinements.jsonl",
                "requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl",
                "requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl",
                "requirements/discovery/2026-11-23-cross-industry/data/refinements.jsonl",
                "requirements/discovery/2026-11-24-simplification-economics/data/refinements.jsonl",
                "requirements/discovery/2026-11-24-simplification-economics/data/gap-requirements.jsonl")


def amend(c, a):
    """Apply one adversarial amendment to a clause (pure); fails loudly if a replacement target is missing."""
    texts = (c["statement"],) + tuple(c["acceptance_criteria"])
    missing = [old for old, _ in a.get("replace", ()) if not any(old in t for t in texts)]
    if missing:
        raise ValueError(f"{c['id']}: amendment target not found: {missing[0][:80]}")
    sub = lambda t: reduce(lambda x, on: x.replace(on[0], on[1]), a.get("replace", ()), t)
    stmt = sub(c["statement"])
    stmt = (stmt + " " + a["append"]) if a.get("append") else stmt
    return dict(c, statement=" ".join(stmt.split()),
                acceptance_criteria=[sub(x) for x in c["acceptance_criteria"]] + list(a.get("acceptance_add", ())),
                amends=list(dict.fromkeys(c["amends"] + list(a.get("amends_add", ())))),
                dependencies=list(dict.fromkeys(c["dependencies"] + list(a.get("dependencies", ())))),
                revision_history=c.get("revision_history", []) + [{"on": ON, "source": "owner decision" if "owner decision" in a["finding"] else "report 08 adversarial review",
                                                                    "findings": a["finding"], "original_statement": c["statement"]}])


def load_clauses(root):
    sys.path.insert(0, str(root / MISSION))
    mods = tuple(importlib.import_module(f"clauses.{m}") for m in CLAUSE_MODULES)
    base = tuple(c for m in mods for c in m.CLAUSES)
    def apply_set(cs, module):
        amendments = importlib.import_module(f"clauses.{module}").AMENDMENTS
        unknown = set(amendments) - {c["id"] for c in cs}
        if unknown:
            raise ValueError(f"{module}: amendments for unknown clauses: {sorted(unknown)}")
        return tuple(amend(c, amendments[c["id"]]) if c["id"] in amendments else c for c in cs)
    return reduce(apply_set, AMENDMENT_MODULES, base)


def cycles(clauses):
    """Clause ids that sit on a dependency cycle (depth-first, pure)."""
    deps = {c["id"]: tuple(c["dependencies"]) for c in clauses}
    def reach(start):
        def go(frontier, seen):
            nxt = tuple(d for f in frontier for d in deps.get(f, ()) if d not in seen)
            return seen if not nxt else go(nxt, seen | set(nxt))
        return go(deps.get(start, ()), set(deps.get(start, ())))
    return tuple(i for i in deps if i in reach(i))


def read_jsonl(p):
    return tuple(json.loads(l) for l in p.read_text(encoding="utf-8").splitlines() if l.strip())


def rid(r):
    return r.get("id") or r.get("refinement_id")


def inventory(root):
    return tuple(csv.DictReader(io.StringIO((root / "requirements/reconciliation/2026-11-23/data/inventory.csv").read_text(encoding="utf-8"))))


def validate(clauses, inv, records, register):
    ids = Counter(c["id"] for c in clauses)
    known = {r["identifier"] for r in inv} | set(records)
    return (tuple(f"duplicate clause {i}" for i, n in ids.items() if n > 1)
            + tuple(f"{c['id']}: unknown amended id {a}" for c in clauses for a in c["amends"] if a not in known)
            + tuple(f"{c['id']}: unknown question {q}" for c in clauses for q in c["resolves"] if q not in register)
            + tuple(f"{c['id']}: unknown dependency {d}" for c in clauses for d in c["dependencies"] if d not in ids)
            + tuple(f"dependency cycle through {i}" for i in cycles(clauses)))


# ---------------------------------------------------------------- markdown sections

def esc(t):
    return " ".join(str(t).split())


def render_clause(c, mission_rel):
    owners = "; ".join(f"{k.replace('_', ' ')}: {v}" for k, v in c["owners"].items())
    return "\n".join([
        f"- **{c['id']} — {esc(c['title'])}:** {esc(c['statement'])}",
        f"  - Provenance: resolves {', '.join(c['resolves']) or 'Phase 3 feedback'}; amends {', '.join(c['amends']) or '—'}"
        + (f"; related findings {', '.join(c['related_findings'])}" if c["related_findings"] else "")
        + f". Evidence and reasoning: [{mission_rel}/02-EVIDENCE-DOSSIERS.md]({mission_rel}/02-EVIDENCE-DOSSIERS.md), "
          f"[{mission_rel}/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md]({mission_rel}/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).",
        f"  - Classification: {c['classification']}; release scope: {c['release_scope']}; operating models: {', '.join(c['operating_models'])}.",
        f"  - Activation gate: {esc(c['activation_gate']).rstrip('.') or 'none beyond release scope'}.",
        f"  - Ownership: {owners}.",
        "  - Acceptance:",
        *[f"    - AC-{i}: {esc(a)}" for i, a in enumerate(c["acceptance_criteria"], 1)],
    ])


def section(clauses, target):
    mission_rel = os.path.relpath(str(MISSION), str(Path(target).parent))
    body = "\n".join(render_clause(c, mission_rel) for c in clauses)
    return "\n".join([BEGIN, f"## Converged requirements: master mission (2026-11-24)", "",
                      "Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are "
                      "binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative "
                      "positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation "
                      "gate names the authorization still required before production use. Where a clause conflicts with an earlier "
                      "requirement in this file, the clause governs and the earlier requirement is annotated.", "", body, END])


def put_section(text, sec):
    if BEGIN in text:
        return re.sub(re.escape(BEGIN) + r".*?" + re.escape(END), lambda _: sec, text, flags=re.S)
    return text.rstrip("\n") + "\n\n" + sec + "\n"


def annotate(lines, line_no, note):
    """Insert note after line_no (1-based) unless the following line already carries a convergence note."""
    i = line_no
    nxt = lines[i] if i < len(lines) else ""
    if nxt.startswith(f"> {NOTE}"):
        return lines[:i] + (note,) + lines[i + 1:]
    return lines[:i] + (note,) + lines[i:]


# ---------------------------------------------------------------- records and events

def record_updates(clauses, records_by_file):
    by_id = reduce(lambda acc, c: {**acc, **{a: acc.get(a, ()) + (c["id"],) for a in c["amends"]}}, clauses, {})
    def upd(r):
        cs = sorted(set(by_id.get(rid(r), ())))
        if not cs:
            return r
        hist = tuple(h for h in r.get("revision_history", ()) if h.get("source") != TAG)
        return dict(r, converged_by=cs, revision_history=list(hist) + [{
            "on": ON, "source": TAG, "change": "Converged clauses govern where they conflict; statement text unchanged",
            "clauses": cs, "provenance": str(MISSION / "03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md")}])
    return {f: tuple(upd(r) for r in rs) for f, rs in records_by_file.items()}


DEFAULT_DISPOSITION = {"evidence-backlog": "empirical-validation-required"}


def applied_events(clauses, states, existing):
    by_q = reduce(lambda acc, c: {**acc, **{q: acc.get(q, ()) + (c,) for q in c["resolves"]}}, clauses, {})
    seen = {(e["question_id"], tuple(e.get("set", {}).get("applied_to", ()))) for e in existing if e["type"] == "applied-to-requirement"}
    def event(q, cs):
        s = states.get(q, {})
        disp = s.get("disposition") or next((DEFAULT_DISPOSITION[c["classification"]] for c in cs if c["classification"] in DEFAULT_DISPOSITION),
                                             "empirical-validation-required" if s.get("review_class") == "Validation" else "business-policy-decision")
        gate = "; ".join(dict.fromkeys(c["activation_gate"] for c in cs if c["activation_gate"]))
        approval = s.get("approval_status") if s.get("approval_status") in ("approved-product-direction", "approved-owner") else "agent-recommended"
        superseded = s.get("resolution_status") == "superseded-by-duplicate"
        ext = s.get("external_authorization_required") or (gate if disp == "external-authorization-required" else "")
        return {"question_id": q, "type": "applied-to-requirement", "set": {
            "applied_to": [c["id"] for c in cs], "disposition": disp,
            "resolution_status": "superseded-by-duplicate" if superseded else "resolved",
            "approval_status": approval, "activation_gate": s.get("activation_gate") or gate,
            "external_authorization_required": ext}}
    cands = tuple(event(q, cs) for q, cs in sorted(by_q.items()))
    fresh = tuple(e for e in cands if (e["question_id"], tuple(e["set"]["applied_to"])) not in seen)
    start = max((int(e["event_id"].split("-")[1]) for e in existing), default=0)
    return tuple(dict(e, event_id=f"QE-{start + i:06d}", on=ON, artifact=str(MISSION / "03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md"),
                      actor=ACTOR) for i, e in enumerate(fresh, 1))


# ---------------------------------------------------------------- baseline index

OWNER_BY_PREFIX = (("UNB-XIN-ACC", "Finance"), ("UNB-ACC", "Finance"), ("UNB-FIN", "Finance"), ("UNB-NAV", "Investments"),
                   ("UNB-INV", "Investments"), ("UNB-XIN-INV", "Investments"), ("UNB-XIN-PAY", "Payments"), ("UNB-XIN-BNK", "Payments"),
                   ("UNB-PAY", "Payments"), ("UNB-GRT", "Grants"), ("UNB-GRANT", "Grants"), ("UNB-XIN-NPG", "Grants"), ("UNB-CHAR", "Charity knowledge"),
                   ("UNB-EXT", "Charity knowledge"), ("UNB-KNOW", "Charity knowledge"), ("UNB-REG", "Regulatory"), ("UNB-XIN-REG", "Regulatory"),
                   ("UNB-LEG", "Regulatory"), ("UNB-SOC2", "Assurance"), ("UNB-CTRL", "Assurance"), ("UNB-GOV", "Governance"),
                   ("UNB-ADM", "Administration"), ("UNB-SUCC", "Accounts and succession"), ("UNB-XIN-TRS", "Accounts and succession"),
                   ("UNB-TEN", "Tenancy"), ("UNB-PART", "Tenancy"), ("UNB-XIN-SAS", "Tenancy"), ("UNB-API", "Platform"), ("UNB-DATA", "Platform"),
                   ("UNB-RES", "Platform"), ("UNB-AIO", "AI"), ("UNB-AI", "AI"), ("UNB-EXP", "Experience"), ("UNB-UI", "Experience"),
                   ("UNB-XIN-ECO", "Economics"), ("UNB-ECO", "Economics"), ("UNB-XIN-SVC", "Service"), ("UNB-XIN-CLM", "Service"),
                   ("UNB-XIN-CRM", "Communications"), ("UNB-SMP", "Operating model"))


def owner_of(i):
    return next((o for p, o in OWNER_BY_PREFIX if i.startswith(p)), "Product (route by domain)")


def classify(i, text):
    t = (text or "").lower()
    if i.startswith(("UNB-HYP", "UNB-GAP", "UNB-COM")):
        return "research-hypothesis"
    if re.match(r"REQ-X-0(5[89]|6\d|7[0-7])$", i):
        return "capability-index-candidate (SPD-ENT-001; normative until carrier coverage verified)"
    if "hypothesis" in t[:80]:
        return "research-hypothesis"
    return "normative"


def text_sha(t):
    return hashlib.sha256((t or "").encode("utf-8")).hexdigest()


def baseline(clauses, inv, records, adoption, release, states):
    conv = reduce(lambda acc, c: {**acc, **{a: acc.get(a, ()) + (c["id"],) for a in c["amends"]}}, clauses, {})
    adopted = {a["original_id"]: a for a in adoption}
    record_status = {a["record_id"]: a["status"] for a in adoption}
    reqs = {r["identifier"]: r for r in inv if r["kind"] == "requirement"}
    def row_req(i, r):
        a = adopted.get(i)
        status = ("refined by " + a["record_id"] + f" ({a['status']})") if a else "original"
        return {"id": i, "kind": "requirement", "source": f"{r['path']}:{r['line']}", "text": esc(r["text"])[:300], "text_sha256": text_sha(r["text"]),
                "status": status, "converged_by": list(conv.get(i, ())), "classification": classify(i, r["text"]),
                "domain_owner": owner_of(i), "release_scope": release.get(i, "unassigned (default initial-implementation unless a clause defers)")}
    def row_rec(i, r, f):
        st = (record_status.get(i) or ("adopted (cross-industry-discovery section)" if "cross-industry" in f and "gap" in f else "")
              or r.get("approval_status") or r.get("validation_status") or "recorded")
        return {"id": i, "kind": "record", "source": f, "text": esc(r.get("statement", ""))[:300], "text_sha256": text_sha(r.get("statement", "")), "status": st,
                "converged_by": list(conv.get(i, ())), "classification": classify(i, r.get("statement", "")),
                "domain_owner": owner_of(i), "release_scope": release.get(i, "unassigned (default initial-implementation unless a clause defers)")}
    rows = tuple(row_req(i, r) for i, r in sorted(reqs.items())) + tuple(row_rec(i, r, f) for i, (r, f) in sorted(records.items()))
    def basis(c):
        appr = {states.get(q, {}).get("approval_status") for q in c["resolves"]}
        return ("mission-adopted from adversarial review (owner ratification pending)" if not c["resolves"]
                else "agent-recommended, owner ratification pending" if "agent-recommended" in appr else "owner-approved product direction")
    cls = tuple({"id": c["id"], "kind": "convergence-clause", "source": c["target_file"], "text": esc(c["statement"])[:300],
                 "text_sha256": text_sha(c["statement"]), "approval_basis": basis(c), "resolves": c["resolves"],
                 "dependencies": c["dependencies"],
                 "status": "adopted (convergence)", "converged_by": [], "classification": c["classification"],
                 "domain_owner": c["owners"]["domain_owner"], "release_scope": c["release_scope"]} for c in clauses)
    return rows + cls


def render_baseline(rows, clauses):
    cnt = Counter(r["kind"] for r in rows)
    st = Counter(r["status"].split(" (")[0] if r["kind"] != "requirement" else ("converged" if r["converged_by"] else r["status"].split(" ")[0]) for r in rows)
    cls = Counter(r["classification"].split(" (")[0] for r in rows)
    def table(h, rs):
        return "\n".join(["| " + " | ".join(h) + " |", "|" + "---|" * len(h)] + ["| " + " | ".join(x) + " |" for x in rs])
    clause_rows = [(c["id"] + (" (amended)" if c.get("revision_history") else ""), esc(c["title"]), c["domain"], c["release_scope"], c["classification"], str(len(c["resolves"])),
                    ", ".join(c["amends"][:6]) + (" …" if len(c["amends"]) > 6 else "")) for c in clauses]
    converged = [(r["id"], r["source"], ", ".join(r["converged_by"])) for r in rows if r["converged_by"]]
    return "\n\n".join([
        "# Authoritative requirements baseline (index)",
        "Generated by `tools/reqaudit/adopt_convergence.py`; do not edit by hand. This index is the single authoritative "
        "entry point to the Unbound requirements baseline as of 2026-11-24. It lists every requirement identified by the "
        "reconciliation inventory, every refinement and gap record from the three requirement missions, and every convergence "
        "clause, with status, governing clause, classification, owner and release scope. Full rows: "
        "[`baseline/baseline.jsonl`](baseline/baseline.jsonl).",
        "**Precedence.** (1) Convergence clauses `CNV-*` (sections `convergence-2026-11-24` in the domain files) govern where they "
        "conflict with earlier text. (2) Adopted reconciliation and cross-industry sections refine the originals they annotate. "
        "(3) Original requirements stand where nothing refines them. Simplification-mission records remain proposed except where a "
        "clause adopts their resolved content. Nothing here authorizes implementation (no application code during this mission) "
        "or production activation (see each clause's activation gate).",
        "## Counts\n\n" + table(("Measure", "Value"), [(k, str(v)) for k, v in sorted(cnt.items())]
                                  + [("status: " + k, str(v)) for k, v in sorted(st.items())]
                                  + [("classification: " + k, str(v)) for k, v in sorted(cls.items())]),
        "## Convergence clauses\n\n" + table(("Clause", "Title", "Domain", "Release scope", "Class", "Questions resolved", "Amends"), clause_rows),
        "## Requirements governed by convergence clauses\n\n" + table(("Requirement", "Defined at", "Clauses"), converged),
    ]) + "\n"


def main(argv):
    root = Path(argv[1])
    clauses = load_clauses(root)
    inv = inventory(root)
    records_by_file = {f: read_jsonl(root / f) for f in RECORD_FILES}
    records = {rid(r): (r, f) for f, rs in records_by_file.items() for r in rs}
    register = {r["question_id"] for r in ql.load_csv(root / "requirements/open-questions.csv")}
    problems = validate(clauses, inv, records, register)
    if problems:
        print("\n".join(problems), file=sys.stderr)
        return 2
    # 1. clause data
    (root / MISSION / "data/convergence-clauses.jsonl").write_text(
        "".join(json.dumps(c, ensure_ascii=False, sort_keys=True) + "\n" for c in clauses), encoding="utf-8")
    # 2. domain sections
    targets = tuple(dict.fromkeys(c["target_file"] for c in clauses))
    for t in targets:
        p = root / t
        p.write_text(put_section(p.read_text(encoding="utf-8"), section(tuple(c for c in clauses if c["target_file"] == t), t)), encoding="utf-8")
    # 3. annotations on Markdown definitions (first definition site per id)
    conv = reduce(lambda acc, c: {**acc, **{a: acc.get(a, ()) + (c["id"],) for a in c["amends"]}}, clauses, {})
    sites = {}
    for r in inv:
        if r["identifier"] in conv and r["kind"] == "requirement" and r["identifier"] not in sites:
            sites[r["identifier"]] = (r["path"], int(r["line"]))
    by_file = reduce(lambda acc, kv: {**acc, kv[1][0]: acc.get(kv[1][0], ()) + ((kv[1][1], kv[0]),)}, sites.items(), {})
    for path, items in by_file.items():
        p = root / path
        lines = tuple(p.read_text(encoding="utf-8").split("\n"))
        # process bottom-up so earlier line numbers stay valid; account for notes already inserted by earlier runs
        def apply(ls, item):
            line_no, ident = item
            idx = next((i for i in range(max(0, line_no - 1), min(len(ls), line_no + 400)) if ident in ls[i]), None)
            if idx is None:
                return ls
            note = f"> {NOTE}: governed by {', '.join(f'`{c}`' for c in sorted(set(conv[ident])))} (section 'Converged requirements: master mission' in the domain file named by each clause)."
            return annotate(ls, idx + 1, note)
        out = reduce(apply, sorted(items, reverse=True), lines)
        p.write_text("\n".join(out), encoding="utf-8")
    # 4. JSONL records
    for f, rs in record_updates(clauses, records_by_file).items():
        (root / f).write_text("".join(json.dumps(r, ensure_ascii=False) + "\n" for r in rs), encoding="utf-8")
    # 5. events
    events = ql.load_events(root)
    add = applied_events(clauses, ql.fold(events), events)
    with open(root / ql.EVENTS, "a", encoding="utf-8") as fh:
        fh.write("".join(json.dumps({k: e[k] for k in ("event_id", "question_id", "on", "type", "artifact", "actor", "set")}, ensure_ascii=False) + "\n" for e in add))
    # 6. baseline index
    adoption = tuple(csv.DictReader(io.StringIO((root / "requirements/reconciliation/2026-11-23/data/adoption.csv").read_text(encoding="utf-8"))))
    release = {json.loads(l)["id"]: json.loads(l)["scope"] for l in (root / "requirements/discovery/2026-11-23-cross-industry/data/release1-scope.jsonl").read_text().splitlines() if l.strip()}
    rows = baseline(clauses, inv, records, adoption, release, ql.fold(ql.load_events(root)))
    (root / "requirements/baseline").mkdir(exist_ok=True)
    (root / "requirements/baseline/baseline.jsonl").write_text("".join(json.dumps(r, ensure_ascii=False, sort_keys=True) + "\n" for r in rows), encoding="utf-8")
    (root / "requirements/AUTHORITATIVE-BASELINE.md").write_text(render_baseline(rows, clauses), encoding="utf-8")
    print(f"clauses {len(clauses)}, files {len(targets)}, annotated {len(sites)}, events {len(add)}, baseline rows {len(rows)}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

#!/usr/bin/env python3
"""Integrate Phase 1 evidence dossiers into the mission evidence register and the resolution log.

Inputs (a directory): dossiers-<B>.jsonl and evidence-<B>.jsonl per research batch (schema in
requirements/convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md, section "Method").

Outputs:
  requirements/convergence/2026-11-24-master/data/evidence.jsonl   merged evidence register
  requirements/convergence/2026-11-24-master/data/dossiers.jsonl   merged dossiers
  requirements/convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md rendered report
  requirements/questions/resolution-events.jsonl                    appended 'evidence-reviewed' events

Idempotent: an event is appended only if no event with the same (question_id, type, artifact,
dossier_sha) exists. Fails (exit 2) on dossiers citing unknown evidence ids, unknown dispositions or
questions that are not in the register.

Usage: python3 -I -B tools/reqaudit/integrate_dossiers.py <repo-root> <input-dir>
"""
import glob
import hashlib
import json
import os
import sys
from functools import reduce
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import questions_lib as ql  # noqa: E402

MISSION = Path("requirements/convergence/2026-11-24-master")
REPORT = MISSION / "02-EVIDENCE-DOSSIERS.md"
ON = "2026-11-24"
ACTOR = "Master mission agent (zero-consultation evidence research; agent-recommended, owner ratification pending)"
BATCH_NAMES = {"A": "Charitable tax law", "B": "Accounting and financial reporting", "C": "Banking and payments",
               "D": "Financial regulation", "E": "Governance, succession and delegated authority",
               "F": "Privacy, communications and sanctions", "G": "Assurance, internal control and board delegation"}


def read_jsonl(path):
    return tuple(json.loads(l) for l in Path(path).read_text(encoding="utf-8").splitlines() if l.strip())


def load_batches(indir):
    batch = lambda p: os.path.basename(p).split("-")[1].split(".")[0]
    dossiers = tuple(dict(d, batch=batch(p)) for p in sorted(glob.glob(os.path.join(indir, "dossiers-*.jsonl"))) for d in read_jsonl(p))
    evidence = tuple(dict(e, batch=batch(p)) for p in sorted(glob.glob(os.path.join(indir, "evidence-*.jsonl"))) for e in read_jsonl(p))
    return dossiers, evidence


def dsha(d):
    return hashlib.sha256(json.dumps({k: v for k, v in d.items() if k != "batch"}, sort_keys=True).encode()).hexdigest()[:16]


def as_list(x):
    return list(x) if isinstance(x, (list, tuple)) else ([x] if x else [])


def check(dossiers, evidence, states):
    ev_ids = {e["evidence_id"] for e in evidence}
    problems = tuple(
        p for d in dossiers for p in (
            (f"{d['question_id']}: unknown disposition {d.get('disposition')!r}",) if d.get("disposition") not in ql.DISPOSITIONS else ()) + (
            (f"{d['question_id']}: not a registered question",) if d["question_id"] not in states else ()) + tuple(
            f"{d['question_id']}: cites unknown evidence {i}" for i in as_list(d.get("evidence_ids")) if i not in ev_ids))
    dup_ev = tuple(f"duplicate evidence id {i}" for i in {e["evidence_id"] for e in evidence}
                   if sum(1 for e in evidence if e["evidence_id"] == i) > 1)
    return problems + dup_ev


def primary_events(d, clusters, states):
    """Events for the dossier's own question and for same-question duplicates it answers."""
    gate = d.get("external_authorization_required") or ""
    empirical = d.get("disposition") == "empirical-validation-required"
    status = "proposed" if d.get("disposition") == "unresolved" else "resolved"
    base = {"answer": d.get("recommended_answer", ""), "disposition": d.get("disposition", ""),
            "confidence": d.get("confidence", ""), "evidence_refs": as_list(d.get("evidence_ids")),
            "conservative_fallback": d.get("conservative_fallback", ""), "remaining_uncertainty": d.get("remaining_uncertainty", ""),
            "acceptance_criteria": as_list(d.get("required_acceptance_criteria")),
            "reconsideration_triggers": as_list(d.get("reconsideration_triggers")),
            "external_authorization_required": gate,
            "activation_gate": gate or ("Empirical validation: " + d.get("remaining_uncertainty", "") if empirical else ""),
            "alternative_design": d.get("alternative_design", ""),
            "resolution_status": status, "approval_status": "agent-recommended",
            "dossier_sha": dsha(d), "dossier_batch": d["batch"]}
    own = ({"question_id": d["question_id"], "set": base},)
    related = {r["id"]: r.get("answer", "") for r in as_list(d.get("related_question_answers")) if isinstance(r, dict) and r.get("id")}
    members = tuple(c for c in clusters if c["canonical_id"] == d["question_id"])
    def member_event(c):
        same = c["relation"] in ("same-question", "sub-question")
        ans = related.get(c["member_id"], "")
        s = ({"resolution_status": "superseded-by-duplicate", "canonical_id": d["question_id"],
              "approval_status": "agent-recommended", "disposition": d.get("disposition", ""),
              "answer": ans or f"Answered by {d['question_id']}: " + d.get("recommended_answer", "")}
             if same else {"related_evidence": f"See {d['question_id']} dossier", **({"related_answer": ans} if ans else {})})
        return {"question_id": c["member_id"], "set": dict(s, dossier_sha=dsha(d), dossier_batch=d["batch"])}
    extra = tuple({"question_id": qid, "set": {"related_evidence": f"See {d['question_id']} dossier", "related_answer": a,
                                               "dossier_sha": dsha(d), "dossier_batch": d["batch"]}}
                  for qid, a in related.items() if qid in states and qid not in {c["member_id"] for c in members} and qid != d["question_id"])
    return own + tuple(member_event(c) for c in members if c["member_id"] in states) + extra


def new_events(dossiers, clusters, states, existing):
    seen = {(e["question_id"], e["type"], e.get("set", {}).get("dossier_sha")) for e in existing}
    cands = tuple(e for d in dossiers for e in primary_events(d, clusters, states))
    fresh = tuple(e for e in cands if (e["question_id"], "evidence-reviewed", e["set"]["dossier_sha"]) not in seen)
    start = max((int(e["event_id"].split("-")[1]) for e in existing), default=0)
    return tuple({"event_id": f"QE-{start + i:06d}", "question_id": e["question_id"], "on": ON, "type": "evidence-reviewed",
                  "artifact": str(REPORT), "actor": ACTOR, "set": e["set"]} for i, e in enumerate(fresh, 1))


# ---------------------------------------------------------------- rendering

def cell(t, n=None):
    s = " ".join(str(t or "").split()).replace("|", "\\|")
    return s if n is None or len(s) <= n else s[: n - 1] + "…"


def bullets(xs):
    return "\n".join(f"- {cell(x)}" for x in as_list(xs)) or "- none recorded"


def render(dossiers, evidence, states):
    ev = {e["evidence_id"]: e for e in evidence}
    by_disp = reduce(lambda acc, d: {**acc, d["disposition"]: acc.get(d["disposition"], 0) + 1}, dossiers, {})
    ext59 = tuple(q for q, s in states.items() if s.get("review_class") == "External")
    covered = {d["question_id"] for d in dossiers}
    summary = "\n".join(["| Question | Batch | Disposition | Confidence | Recommended answer (short) |", "|---|---|---|---|---|"]
                        + [f"| {d['question_id']} | {d['batch']} | {d['disposition']} | {d.get('confidence', '')} | {cell(d.get('recommended_answer'), 220)} |"
                           for d in sorted(dossiers, key=lambda d: (d['batch'], d['question_id']))])
    def finding(f):
        e = ev.get(f.get("evidence_id"), {})
        return f"- *{f.get('classification', '')}* — {cell(f.get('claim'))} [{f.get('evidence_id', '')}{', ' + cell(f.get('pinpoint')) if f.get('pinpoint') else ''}]"
    def section(d):
        rel = "\n".join(f"- **{r.get('id')}**: {cell(r.get('answer'))}" for r in as_list(d.get("related_question_answers")) if isinstance(r, dict)) or "- none"
        corr = "\n".join(f"- {cell(c.get('record_or_artifact'))}: {cell(c.get('issue'))} → {cell(c.get('correction'))}"
                         for c in as_list(d.get("corrections_to_prior_records")) if isinstance(c, dict)) or "- none"
        return "\n\n".join([
            f"### {d['question_id']} — {cell(d.get('exact_question'), 200)}",
            f"**Disposition:** {d['disposition']} · **Confidence:** {d.get('confidence', '')} · **Jurisdiction:** {cell(d.get('jurisdiction'))} · **Batch:** {d['batch']} ({BATCH_NAMES.get(d['batch'], '')})",
            f"**Original provisional answer.** {cell(d.get('original_provisional_answer'))}",
            f"**Relevant requirements.** {cell(', '.join(map(str, as_list(d.get('relevant_requirements')))))}",
            "**Authoritative findings.**\n\n" + ("\n".join(finding(f) for f in as_list(d.get("authoritative_findings")) if isinstance(f, dict)) or "- none"),
            f"**Supporting interpretation.** {cell(d.get('supporting_interpretation'))}",
            f"**Competing interpretation.** {cell(d.get('competing_interpretation'))}",
            f"**Applicability to Unbound.** {cell(d.get('applicability_to_unbound'))}",
            "**Business assumptions.**\n\n" + bullets(d.get("business_assumptions")),
            f"**Recommended answer.** {cell(d.get('recommended_answer'))}",
            f"**Remaining uncertainty.** {cell(d.get('remaining_uncertainty'))}",
            f"**Consequences if wrong.** {cell(d.get('consequences_if_wrong'))}",
            f"**Conservative fallback.** {cell(d.get('conservative_fallback'))}",
            f"**Alternative business or architecture design.** {cell(d.get('alternative_design'))}",
            f"**External authorization required.** {cell(d.get('external_authorization_required')) or 'none'}",
            "**Acceptance criteria.**\n\n" + bullets(d.get("required_acceptance_criteria")),
            "**Reconsideration triggers.**\n\n" + bullets(d.get("reconsideration_triggers")),
            "**Related questions.**\n\n" + rel,
            "**Corrections to prior records.**\n\n" + corr,
        ])
    ev_table = "\n".join(["| ID | Level | Source | Status | Effective | Fetched | Pinpoint |", "|---|---|---|---|---|---|---|"]
                         + [f"| {e['evidence_id']} | {e.get('hierarchy_level', '')} | [{cell(e.get('title'), 90)}]({e.get('url', '')}) ({cell(e.get('issuer'), 40)}) | {e.get('status', '')} | {cell(e.get('effective_date'), 20)} | {'yes' if e.get('verified_by_fetch') else 'no'} | {cell(e.get('pinpoint'), 60)} |"
                            for e in sorted(evidence, key=lambda e: e["evidence_id"])])
    missing = sorted(set(ext59) - covered)
    return "\n\n".join([
        "# 02 — Evidence dossiers (Phase 1, zero-consultation evidence resolution)",
        "Generated by `tools/reqaudit/integrate_dossiers.py` from `data/dossiers.jsonl` and `data/evidence.jsonl`; do not edit by hand. "
        "Research used only publicly available sources retrieved on 2026-11-24. **Research is not authorization**: a dossier never "
        "substitutes for a board resolution, executed contract, bank or acquirer approval, licence, filing or assurance report. "
        "Every answer is *agent-recommended* until the owner ratifies it; conservative fallbacks are adopted into the baseline in the meantime (report 03).",
        "## Method\n\nSeven domain batches researched the 59 externally gated confirmations and the further requirement questions and "
        "decisions whose gates could be answered by research. Evidence hierarchy: statute, effective regulation, official guidance, "
        "case law, accounting standards, official interpretations, public contracts and rules, audited statements, sponsor policies, "
        "professional publications, industry research, commentary. Proposed regulations are recorded as proposed. Each source records "
        "whether it was fetched; unfetched citations are marked. Dispositions: established-by-authoritative-evidence, reasoned-interpretation, "
        "business-policy-decision, empirical-validation-required, external-authorization-required, unresolved.",
        f"## Coverage\n\n- Dossiers: {len(dossiers)} (by disposition: {json.dumps(by_disp, sort_keys=True)})\n"
        f"- Evidence sources: {len(evidence)} ({sum(1 for e in evidence if e.get('verified_by_fetch'))} fetched)\n"
        f"- Externally gated confirmations without a dossier: {', '.join(missing) if missing else 'none'}",
        "## Summary\n\n" + summary,
        "## Dossiers\n\n" + "\n\n".join(section(d) for d in sorted(dossiers, key=lambda d: (d['batch'], d['question_id']))),
        "## Evidence register\n\n" + ev_table,
    ]) + "\n"


def main(argv):
    root, indir = Path(argv[1]), argv[2]
    dossiers, evidence = load_batches(indir)
    events = ql.load_events(root)
    states = ql.fold(events)
    problems = check(dossiers, evidence, states)
    if problems:
        print("\n".join(problems), file=sys.stderr)
        return 2
    clusters = ql.load_csv(root / ql.CLUSTERS)
    add = new_events(dossiers, clusters, states, events)
    (root / MISSION / "data").mkdir(parents=True, exist_ok=True)
    (root / MISSION / "data/evidence.jsonl").write_text("".join(json.dumps(e, ensure_ascii=False, sort_keys=True) + "\n" for e in sorted(evidence, key=lambda e: e["evidence_id"])), encoding="utf-8")
    (root / MISSION / "data/dossiers.jsonl").write_text("".join(json.dumps(d, ensure_ascii=False, sort_keys=True) + "\n" for d in sorted(dossiers, key=lambda d: (d["batch"], d["question_id"]))), encoding="utf-8")
    with open(root / ql.EVENTS, "a", encoding="utf-8") as fh:
        fh.write("".join(json.dumps(e, ensure_ascii=False) + "\n" for e in add))
    (root / REPORT).write_text(render(dossiers, evidence, ql.fold(ql.load_events(root))), encoding="utf-8")
    print(f"dossiers {len(dossiers)}, evidence {len(evidence)}, new events {len(add)}")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

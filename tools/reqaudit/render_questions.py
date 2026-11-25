#!/usr/bin/env python3
"""Consolidate every unanswered question in the requirements missions into one register.

Reads the decision and requirement records of the reconciliation (2026-11-23),
the cross-industry discovery (2026-11-23) and the simplification mission
(2026-11-24) and writes:

  requirements/OPEN-QUESTIONS.md   readable register, grouped by who answers
  requirements/open-questions.csv  the same rows joined with their current resolution

Answers are NOT recorded in the CSV. They live in the append-only, source-linked resolution log
requirements/questions/resolution-events.jsonl (see tools/reqaudit/questions_lib.py and
requirements/questions/README.md). This tool folds that log, validates it against the source records
and joins it to the rows, so regeneration can never discard a decision:

  * a question registered in the log stays in the register even if its source decision is later
    marked decided (its register kind is pinned by the 'registered' event);
  * the tool refuses to write if the existing CSV holds an answer that the log does not (hand edits
    must be moved into the log first);
  * validation errors (orphans, duplicate event ids, missing source records, changed question text,
    invalid closure states) fail the run; warnings are printed.

Pure functions build the rows; main() does the I/O. Re-run after any record or log
change: python3 -I -B tools/reqaudit/render_questions.py .
"""
import csv
import io
import json
import sys
from collections import Counter
from functools import reduce
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import questions_lib as ql  # noqa: E402

RECON = Path("requirements/reconciliation/2026-11-23")
DISC = Path("requirements/discovery/2026-11-23-cross-industry")
SIMP = Path("requirements/discovery/2026-11-24-simplification-economics")
SOURCES = (
    (RECON, "Reconciliation", "14-OPEN-DECISIONS.md"),
    (DISC, "Cross-industry discovery", "15-OPEN-DECISIONS-AND-SCENARIOS.md"),
    (SIMP, "Simplification and economics", "20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md"),
)
# Who answers: the party named first in the owner field (the primary owner).
# A bare "sponsor" only counts when no other party is named ("Sponsor CFO" -> finance).
OWNER_GROUPS = (
    ("Counsel", ("counsel", "legal", " gc ", " gc,", "(gc")),
    ("Finance: CFO, controller, CPA, auditor", ("cpa", "auditor", "cfo", "controller", "finance", "treasury", "audit", "tax advisor", "return preparer")),
    ("Technology: architecture, security, privacy", ("architecture", "cto", "security", "privacy", "engineering", "spike", "forma", "usability")),
    ("Product owner", ("product", "requirements owner", "program owner", "commercial", "marketing", "economics", "owner", "template review", "consent-capture")),
    ("Sponsor governance and operations", ("board", "governance", "compliance", "operations", "coo", "executive", "head of", "crco", "committee")),
)
FALLBACK = ("Sponsor governance and operations", ("sponsor",))
GROUP_ORDER = tuple(g for g, _ in OWNER_GROUPS) + ("Unassigned",)
FIELDS = ("question_id", "kind", "owner_group", "owner", "question", "context", "blocks_implementation",
          "source", "status", "answer", "answered_by", "answered_on",
          "resolution_status", "approval_status", "disposition", "canonical_id", "activation_gate",
          "external_authorization_required", "evidence_refs", "last_event", "resolution_events")


def owner_group(owner):
    o = " " + (owner or "").lower() + " "
    hits = sorted((o.find(k), i, g) for i, (g, keys) in enumerate(OWNER_GROUPS) for k in keys if k in o)
    fallback = FALLBACK[0] if any(k in o for k in FALLBACK[1]) else "Unassigned"
    return hits[0][2] if hits else fallback


def needs_confirmation(text):
    t = str(text).strip().lower().rstrip(".")
    return not (t in ("", "none") or (t.startswith("none") and not any(w in t for w in ("beyond", "before", "counsel", "cpa"))))


def load_csv(path):
    return tuple(csv.DictReader(io.StringIO(path.read_text(encoding="utf-8")))) if path.exists() else ()


def load_jsonl(path):
    return tuple(json.loads(l) for l in path.read_text(encoding="utf-8").splitlines() if l.strip()) if path.exists() else ()


def row(**kw):
    return {f: kw.get(f, "") for f in FIELDS}


def open_decisions(root, base, label, report, pinned=frozenset()):
    """Open decisions, plus decisions pinned as 'open decision' by a registered event (closed since)."""
    return tuple(row(question_id=d["decision_id"], kind="open decision", owner=d["decision_owner"],
                     owner_group=owner_group(d["decision_owner"]), question=d["question"],
                     context="Recommendation: " + d["recommendation"], blocks_implementation=d.get("blocks_implementation", ""),
                     source=f"{base / report} ({label})", status=d["status"])
                 for d in load_csv(root / base / "data/decisions.csv") if d["status"] == "open" or d["decision_id"] in pinned)


def confirmations(root, pinned=frozenset()):
    """Provisional decisions that still need a named professional or owner to confirm them,
    plus decisions pinned as 'awaiting confirmation' by a registered event."""
    recon = tuple(row(question_id=d["decision_id"], kind="awaiting confirmation", owner=d["validation_required"],
                      owner_group=owner_group(d["validation_required"]),
                      question="Confirm provisional decision: " + d["question"],
                      context="Decided: " + d.get("decided_option", ""), blocks_implementation=d.get("blocks_implementation", ""),
                      source=f"{RECON / '14-OPEN-DECISIONS.md'} (Reconciliation)", status=d["status"])
                  for d in load_csv(root / RECON / "data/decisions.csv")
                  if (d["status"] == "decided-provisional" and needs_confirmation(d.get("validation_required", "none")))
                  or d["decision_id"] in pinned)
    questions = {d["decision_id"]: d for d in load_csv(root / DISC / "data/decisions.csv")}
    disc = tuple(row(question_id=o["decision_id"], kind="awaiting confirmation", owner=o["confirmation_required"],
                     owner_group=owner_group(o["confirmation_required"]),
                     question="Confirm provisional decision: " + questions.get(o["decision_id"], {}).get("question", ""),
                     context="Decided: " + o["decision"],
                     blocks_implementation=questions.get(o["decision_id"], {}).get("blocks_implementation", ""),
                     source=f"{DISC / '16-LEGAL-AND-ACCOUNTING-DECISIONS.md'} (Cross-industry discovery)", status=o["status"])
                 for o in load_jsonl(root / DISC / "data/decision-outcomes.jsonl")
                 if needs_confirmation(o.get("confirmation_required", "")) or o["decision_id"] in pinned)
    return recon + disc


def requirement_questions(root):
    def from_record(base, label, name, r):
        rid = r.get("id") or r.get("refinement_id")
        qs = r.get("open_questions") or ()
        qs = (qs,) if isinstance(qs, str) else tuple(qs)
        return tuple(row(question_id=f"{rid}#Q{i}", kind="requirement question", owner="Product owner (route as needed)",
                         owner_group="Product owner", question=q, context=(r.get("statement") or "")[:400],
                         source=f"{base / 'data' / name} ({label})", status=r.get("approval_status") or r.get("validation_status", ""))
                     for i, q in enumerate(qs, 1) if str(q).strip())
    return tuple(q for base, label, _ in SOURCES for name in ("gap-requirements.jsonl", "refinements.jsonl")
                 for r in load_jsonl(root / base / "data" / name) for q in from_record(base, label, name, r))


def status_cell(r):
    parts = (r.get("resolution_status", ""), r.get("disposition", ""), ("dup of " + r["canonical_id"]) if r.get("canonical_id") else "")
    return "; ".join(p for p in parts if p)


def blocks_first(rows):
    return tuple(sorted(rows, key=lambda r: (r["blocks_implementation"] != "Y", r["question_id"])))


def cell(text, limit=None):
    t = " ".join(str(text).split()).replace("|", "\\|")
    return t if limit is None or len(t) <= limit else t[: limit - 1].rstrip() + "…"


def table(headers, rows):
    return "\n".join(["| " + " | ".join(headers) + " |", "|" + "---|" * len(headers)]
                     + ["| " + " | ".join(r) + " |" for r in rows])


def render(rows):
    by_kind = lambda k: tuple(r for r in rows if r["kind"] == k)
    od, ac, rq = by_kind("open decision"), by_kind("awaiting confirmation"), by_kind("requirement question")
    counts = Counter((r["owner_group"], r["kind"]) for r in rows)
    summary = table(("Who answers", "Open decisions", "Awaiting confirmation", "Requirement questions", "Total"),
                    [(g, str(counts[(g, "open decision")]), str(counts[(g, "awaiting confirmation")]),
                      str(counts[(g, "requirement question")]),
                      str(sum(counts[(g, k)] for k in ("open decision", "awaiting confirmation", "requirement question"))))
                     for g in GROUP_ORDER if any(counts[(g, k)] for k in ("open decision", "awaiting confirmation", "requirement question"))]
                    + [("**Total**", f"**{len(od)}**", f"**{len(ac)}**", f"**{len(rq)}**", f"**{len(rows)}**")])
    first = blocks_first(tuple(r for r in od if r["blocks_implementation"] == "Y"))

    def od_section(g):
        rs = blocks_first(tuple(r for r in od if r["owner_group"] == g))
        return f"### {g} ({len(rs)})\n\n" + table(("ID", "Question", "Recommendation", "Blocks", "Owner", "Status", "Current answer"),
                                                  [(r["question_id"], cell(r["question"]), cell(r["context"].removeprefix("Recommendation: "), 300),
                                                    r["blocks_implementation"], cell(r["owner"]), status_cell(r), cell(r.get("answer", ""), 300))
                                                   for r in rs]) if rs else ""

    def ac_section(g):
        rs = tuple(r for r in ac if r["owner_group"] == g)
        return f"### {g} ({len(rs)})\n\n" + table(("ID", "What to confirm", "Provisional decision", "Who confirms", "Status", "Current answer"),
                                                  [(r["question_id"], cell(r["question"].removeprefix("Confirm provisional decision: "), 250),
                                                    cell(r["context"].removeprefix("Decided: "), 250), cell(r["owner"]), status_cell(r), cell(r.get("answer", ""), 300))
                                                   for r in rs]) if rs else ""

    rq_rows = [(r["question_id"], cell(r["question"]), cell(r["source"].split(" (")[1].rstrip(")")), status_cell(r),
                cell(r.get("answer", ""), 300))
               for r in sorted(rq, key=lambda r: r["question_id"])]
    parts = [
        "# Open questions register",
        "Every question in the requirements missions that still needs an answer, in one place. "
        "Generated by `tools/reqaudit/render_questions.py` from the mission data files and the resolution log; do not edit by hand. "
        "Answers, approvals, evidence dispositions and their history are recorded only in the append-only log "
        "[`questions/resolution-events.jsonl`](questions/resolution-events.jsonl) (see [`questions/README.md`](questions/README.md)); "
        "[`open-questions.csv`](open-questions.csv) is an output that joins each question to its current resolution. "
        "The tool refuses to regenerate if the CSV holds an answer the log does not.",
        "**Three kinds of question:**\n\n"
        "- **Open decisions** have not been decided. Each has a recommendation.\n"
        "- **Awaiting confirmation** are decided provisionally and need the named professional or owner to confirm them.\n"
        "- **Requirement questions** are open points on proposed requirements and refinements.",
        "**Who answers** is grouped by keyword from each record's owner field, with counsel taking precedence. "
        "Unbound is a software platform only (SPD-ADV-004), so a question owned by a sponsor role usually means "
        "the product owner decides the default and the setting the sponsor can change.",
        "## Summary\n\n" + summary,
        "## Resolution status\n\n" + table(("Resolution status", "Approval", "Evidence disposition", "Questions"),
                                             [(a, b, c or "—", str(n)) for (a, b, c), n in sorted(Counter(
                                                 (r.get("resolution_status", ""), r.get("approval_status", ""), r.get("disposition", "")) for r in rows).items())]),
        f"## Decide first: open decisions that block implementation ({len(first)})\n\n"
        + table(("ID", "Question", "Who answers"), [(r["question_id"], cell(r["question"]), r["owner_group"]) for r in first]),
        f"## 1. Open decisions ({len(od)})",
        *[s for s in (od_section(g) for g in GROUP_ORDER) if s],
        f"## 2. Provisional decisions awaiting confirmation ({len(ac)})",
        *[s for s in (ac_section(g) for g in GROUP_ORDER) if s],
        f"## 3. Open questions on proposed requirements ({len(rq)})\n\n"
        "Most of these are answered by the product owner, routing to counsel, finance or architecture where the question says so.\n\n"
        + table(("Question ID", "Question", "Mission", "Status", "Current answer"), rq_rows),
        "## Not consolidated here\n\n"
        "- **Earlier research questions:** `research/daf-theoretical-enterprise-validation/19-questions/` and "
        "`research/daf-theoretical-enterprise-validation/16-registers/owner-decisions.md`. They were inputs to the "
        "reconciliation mission; this register does not re-check whether each one was carried forward.\n"
        "- **Evidence gaps** that need measurement rather than a decision: report 20 of the simplification mission.",
    ]
    return "\n\n".join(p for p in parts if p) + "\n"


def to_csv(rows):
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=FIELDS, quoting=csv.QUOTE_ALL, lineterminator="\n")
    w.writeheader()
    w.writerows(rows)
    return buf.getvalue()


def pinned_ids(states, kind):
    return frozenset(q for q, s in states.items() if s.get("register_kind") == kind)


def candidate_rows(root, states):
    """Source-derived rows; each question appears once, under the kind pinned by its registered event."""
    rows = (tuple(r for base, label, report in SOURCES
                  for r in open_decisions(root, base, label, report, pinned_ids(states, "open decision")))
            + confirmations(root, pinned_ids(states, "awaiting confirmation")) + requirement_questions(root))
    keep = lambda r: states.get(r["question_id"], {}).get("register_kind", r["kind"]) == r["kind"]
    first_wins = lambda acc, r: acc if r["question_id"] in acc else {**acc, r["question_id"]: r}
    return tuple(reduce(first_wins, (r for r in rows if keep(r)), {}).values())


def join(r, s):
    """Overlay the folded resolution state on a register row."""
    if not s:
        return dict(r, resolution_status="open", approval_status="not-approved")
    hist = s.get("history", ())
    last = hist[-1] if hist else {}
    return dict(r, answer=s.get("answer", ""), answered_by=s.get("approved_by", "") or (last.get("actor", "") if s.get("answer") else ""),
                answered_on=s.get("approved_on", "") or (last.get("on", "") if s.get("answer") else ""),
                resolution_status=s.get("resolution_status", ""), approval_status=s.get("approval_status", ""),
                disposition=s.get("disposition", ""), canonical_id=s.get("canonical_id", ""),
                activation_gate=s.get("activation_gate", ""),
                external_authorization_required=s.get("external_authorization_required", ""),
                evidence_refs=";".join(s.get("evidence_refs", ()) or ()),
                last_event=f"{last.get('event_id', '')} {last.get('type', '')} {last.get('artifact', '')}".strip(),
                resolution_events=str(len(hist)))


def logged_answers(events):
    """Every answer text ever recorded in the log, per question."""
    return reduce(lambda acc, e: {**acc, e["question_id"]: acc.get(e["question_id"], frozenset()) | {e["set"]["answer"].strip()}}
                  if (e.get("set") or {}).get("answer") else acc, events, {})


def discarded_hand_answers(existing, logged):
    """CSV answers that the log never recorded: hand edits that regeneration would silently discard."""
    return tuple(e["question_id"] for e in existing
                 if (e.get("answer") or "").strip() and e["answer"].strip() not in logged.get(e["question_id"], frozenset()))


def main(argv):
    root = Path(argv[1]) if len(argv) > 1 else Path(".")
    events = ql.load_events(root)
    states = ql.fold(events)
    findings = ql.validate(events, states, ql.source_decisions(root), ql.source_requirement_questions(root),
                           ql.load_csv(root / ql.CLUSTERS))
    for f in findings:
        print(*f, sep=" | ", file=sys.stderr)
    base = candidate_rows(root, states)
    orphans = sorted(set(states) - {r["question_id"] for r in base})
    if ql.errors(findings) or orphans:
        print(f"refusing to write: {len(ql.errors(findings))} validation errors; orphaned resolutions: {orphans}", file=sys.stderr)
        return 2
    rows = tuple(join(r, states.get(r["question_id"])) for r in base)
    csv_path = root / "requirements/open-questions.csv"
    lost = discarded_hand_answers(load_csv(csv_path), logged_answers(events)) if "--allow-discard" not in argv else ()
    if lost:
        print(f"refusing to write: {len(lost)} answers in {csv_path} are not in {ql.EVENTS}: {list(lost)[:10]}", file=sys.stderr)
        return 3
    (root / "requirements/OPEN-QUESTIONS.md").write_text(render(rows), encoding="utf-8")
    csv_path.write_text(to_csv(rows), encoding="utf-8")
    print(dict(Counter(r["kind"] for r in rows)), "total", len(rows))
    print(dict(Counter((r["resolution_status"], r["approval_status"]) for r in rows)))
    print("warnings:", len(findings))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

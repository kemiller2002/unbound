#!/usr/bin/env python3
"""Open-questions register: every question in the requirements missions, joined to its recorded answer.

Questions come from the decision and requirement records of the reconciliation
(2026-11-23), the cross-industry discovery (2026-11-23) and the simplification
mission (2026-11-24). Answers come from resolution records in
requirements/resolutions/*/resolutions.jsonl, keyed by a stable question key:

    <record id>#<first 10 hex of sha256(kind | normalized exact question text)>

so reordering, inserting or removing other questions never moves an answer, and
editing a question's text orphans its answer (reported as an error) instead of
silently re-attaching it.

Writes requirements/OPEN-QUESTIONS.md and requirements/open-questions.csv.
Pure functions build the rows; main() does the I/O.

    python3 -I -B tools/reqaudit/render_questions.py .          # write
    python3 -I -B tools/reqaudit/render_questions.py . --check  # verify only; exit 1 on integrity errors
"""
import csv
import hashlib
import io
import json
import sys
from collections import Counter
from pathlib import Path

RECON = Path("requirements/reconciliation/2026-11-23")
DISC = Path("requirements/discovery/2026-11-23-cross-industry")
SIMP = Path("requirements/discovery/2026-11-24-simplification-economics")
RESOLUTIONS = "requirements/resolutions/*/resolutions.jsonl"
SOURCES = (
    (RECON, "Reconciliation", "14-OPEN-DECISIONS.md"),
    (DISC, "Cross-industry discovery", "15-OPEN-DECISIONS-AND-SCENARIOS.md"),
    (SIMP, "Simplification and economics", "20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md"),
)
KINDS = ("open decision", "awaiting confirmation", "requirement question")
# Who answers or closes a gate: the party named first in the owner text.
# A bare "sponsor" only counts when no other party is named ("Sponsor CFO" -> finance).
OWNER_GROUPS = (
    ("Counsel", ("counsel", "legal", " gc ", " gc,", "(gc")),
    ("Finance: CFO, controller, CPA, auditor", ("cpa", "auditor", "cfo", "controller", "finance", "treasury", "audit", "tax advisor", "return preparer", "acquirer", "bank", "odfi", "custodian")),
    ("Technology: architecture, security, privacy", ("architecture", "cto", "security", "privacy", "engineering", "spike", "forma", "usability", "test", "simulat", "benchmark")),
    ("Product owner", ("product", "requirements owner", "program owner", "commercial", "marketing", "economics", "owner", "template review", "consent-capture", "pilot", "measure", "calibrat", "contract")),
    ("Sponsor governance and operations", ("board", "governance", "compliance", "operations", "coo", "executive", "head of", "crco", "committee", " ops ", " ops.")),
)
FALLBACK = ("Sponsor governance and operations", ("sponsor",))
GROUP_ORDER = tuple(g for g, _ in OWNER_GROUPS) + ("Unassigned",)
FIELDS = ("question_key", "legacy_id", "record_id", "kind", "owner_group", "owner", "question", "context",
          "blocks_implementation", "source", "status", "resolution_id", "disposition", "answer", "gate", "gate_type",
          "gate_group", "revision", "answered_by", "answered_on")
STATES = ("unanswered", "answered, external gate open", "answered, validation gate open", "answered, no gate")


# ----------------------------------------------------------------------------- keys and classification

def normalize(text):
    return " ".join(str(text).split()).lower()


def question_key(record_id, kind, text):
    digest = hashlib.sha256(f"{kind}|{normalize(text)}".encode("utf-8")).hexdigest()[:10]
    return f"{record_id}#{digest}"


def owner_group(owner):
    o = " " + (owner or "").lower() + " "
    hits = sorted((o.find(k), i, g) for i, (g, keys) in enumerate(OWNER_GROUPS) for k in keys if k in o)
    fallback = FALLBACK[0] if any(k in o for k in FALLBACK[1]) else "Unassigned"
    return hits[0][2] if hits else fallback


def needs_confirmation(text):
    t = normalize(text).rstrip(".")
    return not (t in ("", "none") or (t.startswith("none") and not any(w in t for w in ("beyond", "before", "counsel", "cpa"))))


def state_of(row):
    if not row["resolution_id"]:
        return STATES[0]
    return {"external": STATES[1], "validation": STATES[2]}.get(row["gate_type"], STATES[3])


# ----------------------------------------------------------------------------- loading questions

def load_csv(path):
    return tuple(csv.DictReader(io.StringIO(path.read_text(encoding="utf-8")))) if path.exists() else ()


def load_jsonl(path):
    return tuple(json.loads(l) for l in path.read_text(encoding="utf-8").splitlines() if l.strip()) if path.exists() else ()


def row(record_id, kind, source_text, legacy_id, **kw):
    base = {f: "" for f in FIELDS}
    return dict(base, record_id=record_id, kind=kind, legacy_id=legacy_id,
                question_key=question_key(record_id, kind, source_text), **kw)


def open_decisions(root, base, label, report):
    return tuple(row(d["decision_id"], KINDS[0], d["question"], d["decision_id"], owner=d["decision_owner"],
                     owner_group=owner_group(d["decision_owner"]), question=d["question"],
                     context="Recommendation: " + d["recommendation"], blocks_implementation=d.get("blocks_implementation", ""),
                     source=f"{base / report} ({label})", status=d["status"])
                 for d in load_csv(root / base / "data/decisions.csv") if d["status"] == "open")


def confirmations(root):
    """Provisional decisions that still need a named professional or owner to confirm them."""
    recon = tuple(row(d["decision_id"], KINDS[1], d["question"], d["decision_id"], owner=d["validation_required"],
                      owner_group=owner_group(d["validation_required"]), question=d["question"],
                      context="Decided: " + d.get("decided_option", ""), blocks_implementation=d.get("blocks_implementation", ""),
                      source=f"{RECON / '14-OPEN-DECISIONS.md'} (Reconciliation)", status=d["status"])
                  for d in load_csv(root / RECON / "data/decisions.csv")
                  if d["status"] == "decided-provisional" and needs_confirmation(d.get("validation_required", "none")))
    questions = {d["decision_id"]: d for d in load_csv(root / DISC / "data/decisions.csv")}
    disc = tuple(row(o["decision_id"], KINDS[1], questions.get(o["decision_id"], {}).get("question", ""), o["decision_id"],
                     owner=o["confirmation_required"], owner_group=owner_group(o["confirmation_required"]),
                     question=questions.get(o["decision_id"], {}).get("question", ""), context="Decided: " + o["decision"],
                     blocks_implementation=questions.get(o["decision_id"], {}).get("blocks_implementation", ""),
                     source=f"{DISC / '16-LEGAL-AND-ACCOUNTING-DECISIONS.md'} (Cross-industry discovery)", status=o["status"])
                 for o in load_jsonl(root / DISC / "data/decision-outcomes.jsonl")
                 if needs_confirmation(o.get("confirmation_required", "")))
    return recon + disc


def requirement_questions(root):
    def from_record(base, label, name, r):
        rid = r.get("id") or r.get("refinement_id")
        qs = r.get("open_questions") or ()
        qs = (qs,) if isinstance(qs, str) else tuple(qs)
        return tuple(row(rid, KINDS[2], q, f"{rid}#Q{i}", owner="Product owner (route as needed)", owner_group="Product owner",
                         question=q, context=(r.get("statement") or "")[:400],
                         source=f"{base / 'data' / name} ({label})", status=r.get("approval_status") or r.get("validation_status", ""))
                     for i, q in enumerate(qs, 1) if str(q).strip())
    return tuple(q for base, label, _ in SOURCES for name in ("gap-requirements.jsonl", "refinements.jsonl")
                 for r in load_jsonl(root / base / "data" / name) for q in from_record(base, label, name, r))


def collect(root):
    return (tuple(r for base, label, report in SOURCES for r in open_decisions(root, base, label, report))
            + confirmations(root) + requirement_questions(root))


def load_resolutions(root):
    return tuple(r for f in sorted(root.glob(RESOLUTIONS)) for r in load_jsonl(f))


# ----------------------------------------------------------------------------- join and integrity

def join(rows, resolutions):
    """Attach the latest resolution to each question; return (rows, integrity issues)."""
    by_key = {}
    for r in resolutions:
        by_key.setdefault(r["question_key"], []).append(r)
    keys = Counter(r["question_key"] for r in rows)
    issues = tuple(
        [f"duplicate question key {k} ({n} questions)" for k, n in keys.items() if n > 1]
        + [f"duplicate resolution id {i}" for i, n in Counter(r["resolution_id"] for r in resolutions).items() if n > 1]
        + [f"orphan resolution {r['resolution_id']} for {r['question_key']} (question removed or its text changed): {r['question'][:80]}"
           for r in resolutions if r["question_key"] not in keys])

    def attach(q):
        hits = by_key.get(q["question_key"])
        if not hits:
            return q
        r = hits[-1]  # later batches supersede earlier ones
        return dict(q, resolution_id=r["resolution_id"], disposition=r["disposition"], answer=r["answer"],
                    gate=r.get("gate", ""), gate_type=r.get("gate_type", ""), gate_group=owner_group(r.get("gate", "")) if r.get("gate") else "",
                    revision=r.get("revision", ""), answered_by=r["answered_by"], answered_on=r["answered_on"])
    return tuple(attach(q) for q in rows), issues


# ----------------------------------------------------------------------------- rendering

def cell(text, limit=None):
    t = " ".join(str(text).split()).replace("|", "\\|")
    return t if limit is None or len(t) <= limit else t[: limit - 1].rstrip() + "…"


def table(headers, rows):
    return "\n".join(["| " + " | ".join(headers) + " |", "|" + "---|" * len(headers)]
                     + ["| " + " | ".join(r) + " |" for r in rows])


def ordered(rows):
    return tuple(sorted(rows, key=lambda r: (KINDS.index(r["kind"]), r["blocks_implementation"] != "Y", r["record_id"], r["question_key"])))


def render(rows, issues):
    states = Counter((r["kind"], state_of(r)) for r in rows)
    summary = table(("Kind",) + STATES + ("Total",),
                    [(k,) + tuple(str(states[(k, s)]) for s in STATES) + (str(sum(states[(k, s)] for s in STATES)),) for k in KINDS]
                    + [("**Total**",) + tuple(f"**{sum(states[(k, s)] for k in KINDS)}**" for s in STATES) + (f"**{len(rows)}**",)])
    unanswered = ordered(tuple(r for r in rows if state_of(r) == STATES[0]))
    gated = lambda t: ordered(tuple(r for r in rows if state_of(r) == t))

    def gate_section(title, rs, by_group):
        groups = [(g, tuple(r for r in rs if (r["gate_group"] or "Unassigned") == g)) for g in GROUP_ORDER] if by_group else [("", rs)]
        body = [(f"### {g} ({len(x)})\n\n" if g else "") + table(
            ("Record", "Kind", "Question", "Answer (product direction)", "Gate"),
            [(r["record_id"], r["kind"], cell(r["question"], 220), cell(r["answer"], 320), cell(r["gate"], 160)) for r in x])
            for g, x in groups if x]
        return "\n\n".join([f"## {title} ({len(rs)})"] + body)

    parts = [
        "# Open questions register",
        "Every question in the requirements missions, joined to its recorded answer. Generated by "
        "`tools/reqaudit/render_questions.py`; do not edit by hand. Answers live in resolution records under "
        "`requirements/resolutions/` (see its README), keyed by a stable question key, so the same question keeps "
        "its answer when other questions change. [`open-questions.csv`](open-questions.csv) has every column.",
        "**Answers are product directions.** An answer with an external gate still needs the named counsel, CPA, "
        "auditor, bank, board or counterparty before the capability is activated; a validation gate needs the stated "
        "test, pilot or measurement. Answering does not change a decision record's own status.",
        "## Summary\n\n" + summary,
        "## Integrity\n\n" + ("No duplicate keys and no orphaned answers." if not issues else
                              "**Errors:**\n\n" + "\n".join(f"- {i}" for i in issues)),
        f"## Unanswered ({len(unanswered)})\n\n" + (table(("Key", "Kind", "Question", "Who answers", "Blocks"),
                                                         [(r["question_key"], r["kind"], cell(r["question"]), r["owner_group"], r["blocks_implementation"])
                                                          for r in unanswered]) if unanswered else "None."),
        gate_section("Answered, external gate open: who must confirm", gated(STATES[1]), True),
        gate_section("Answered, validation gate open: what must be tested or measured", gated(STATES[2]), False),
        gate_section("Answered, no remaining gate", gated(STATES[3]), False),
        "## Not consolidated here\n\n"
        "- **Earlier research questions:** `research/daf-theoretical-enterprise-validation/19-questions/` and "
        "`research/daf-theoretical-enterprise-validation/16-registers/owner-decisions.md`. They were inputs to the "
        "reconciliation mission; this register does not re-check whether each one was carried forward.\n"
        "- **Evidence gaps** that need measurement rather than a decision: report 20 of the simplification mission.",
    ]
    return "\n\n".join(parts) + "\n"


def to_csv(rows):
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=FIELDS, quoting=csv.QUOTE_ALL, lineterminator="\n")
    w.writeheader()
    w.writerows(ordered(rows))
    return buf.getvalue()


def main(argv):
    args = [a for a in argv[1:] if not a.startswith("--")]
    root = Path(args[0]) if args else Path(".")
    rows, issues = join(collect(root), load_resolutions(root))
    if "--check" not in argv:
        (root / "requirements/OPEN-QUESTIONS.md").write_text(render(rows, issues), encoding="utf-8")
        (root / "requirements/open-questions.csv").write_text(to_csv(rows), encoding="utf-8")
    print(json.dumps({"questions": len(rows), "by_state": dict(Counter(state_of(r) for r in rows)),
                      "integrity_errors": len(issues)}, indent=1))
    for i in issues:
        print("ERROR", i)
    return 1 if issues else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

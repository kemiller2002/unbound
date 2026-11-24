#!/usr/bin/env python3
"""Consolidate every unanswered question in the requirements missions into one register.

Reads the decision and requirement records of the reconciliation (2026-11-23),
the cross-industry discovery (2026-11-23) and the simplification mission
(2026-11-24) and writes:

  requirements/OPEN-QUESTIONS.md   readable register, grouped by who answers
  requirements/open-questions.csv  the same rows with empty answer columns

Pure functions build the rows; main() does the I/O. Re-run after any record
changes: python3 -I -B tools/reqaudit/render_questions.py .
"""
import csv
import io
import json
import sys
from collections import Counter
from pathlib import Path

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
          "source", "status", "answer", "answered_by", "answered_on")


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


def open_decisions(root, base, label, report):
    return tuple(row(question_id=d["decision_id"], kind="open decision", owner=d["decision_owner"],
                     owner_group=owner_group(d["decision_owner"]), question=d["question"],
                     context="Recommendation: " + d["recommendation"], blocks_implementation=d.get("blocks_implementation", ""),
                     source=f"{base / report} ({label})", status=d["status"])
                 for d in load_csv(root / base / "data/decisions.csv") if d["status"] == "open")


def confirmations(root):
    """Provisional decisions that still need a named professional or owner to confirm them."""
    recon = tuple(row(question_id=d["decision_id"], kind="awaiting confirmation", owner=d["validation_required"],
                      owner_group=owner_group(d["validation_required"]),
                      question="Confirm provisional decision: " + d["question"],
                      context="Decided: " + d.get("decided_option", ""), blocks_implementation=d.get("blocks_implementation", ""),
                      source=f"{RECON / '14-OPEN-DECISIONS.md'} (Reconciliation)", status=d["status"])
                  for d in load_csv(root / RECON / "data/decisions.csv")
                  if d["status"] == "decided-provisional" and needs_confirmation(d.get("validation_required", "none")))
    questions = {d["decision_id"]: d for d in load_csv(root / DISC / "data/decisions.csv")}
    disc = tuple(row(question_id=o["decision_id"], kind="awaiting confirmation", owner=o["confirmation_required"],
                     owner_group=owner_group(o["confirmation_required"]),
                     question="Confirm provisional decision: " + questions.get(o["decision_id"], {}).get("question", ""),
                     context="Decided: " + o["decision"],
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
        return tuple(row(question_id=f"{rid}#Q{i}", kind="requirement question", owner="Product owner (route as needed)",
                         owner_group="Product owner", question=q, context=(r.get("statement") or "")[:400],
                         source=f"{base / 'data' / name} ({label})", status=r.get("approval_status") or r.get("validation_status", ""))
                     for i, q in enumerate(qs, 1) if str(q).strip())
    return tuple(q for base, label, _ in SOURCES for name in ("gap-requirements.jsonl", "refinements.jsonl")
                 for r in load_jsonl(root / base / "data" / name) for q in from_record(base, label, name, r))


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
        return f"### {g} ({len(rs)})\n\n" + table(("ID", "Question", "Recommendation", "Blocks", "Owner", "Source"),
                                                  [(r["question_id"], cell(r["question"]), cell(r["context"].removeprefix("Recommendation: "), 400),
                                                    r["blocks_implementation"], cell(r["owner"]), cell(r["source"]))
                                                   for r in rs]) if rs else ""

    def ac_section(g):
        rs = tuple(r for r in ac if r["owner_group"] == g)
        return f"### {g} ({len(rs)})\n\n" + table(("ID", "What to confirm", "Provisional decision", "Who confirms"),
                                                  [(r["question_id"], cell(r["question"].removeprefix("Confirm provisional decision: "), 250),
                                                    cell(r["context"].removeprefix("Decided: "), 300), cell(r["owner"]))
                                                   for r in rs]) if rs else ""

    rq_rows = [(r["question_id"].split("#")[0], cell(r["question"]), cell(r["source"].split(" (")[1].rstrip(")")))
               for r in sorted(rq, key=lambda r: r["question_id"])]
    parts = [
        "# Open questions register",
        "Every question in the requirements missions that still needs an answer, in one place. "
        "Generated by `tools/reqaudit/render_questions.py` from the mission data files; do not edit by hand. "
        "Record answers in [`open-questions.csv`](open-questions.csv) (columns `answer`, `answered_by`, `answered_on`), "
        "then update the source record and re-run the tool.",
        "**Three kinds of question:**\n\n"
        "- **Open decisions** have not been decided. Each has a recommendation.\n"
        "- **Awaiting confirmation** are decided provisionally and need the named professional or owner to confirm them.\n"
        "- **Requirement questions** are open points on proposed requirements and refinements.",
        "**Who answers** is grouped by keyword from each record's owner field, with counsel taking precedence. "
        "Unbound is a software platform only (SPD-ADV-004), so a question owned by a sponsor role usually means "
        "the product owner decides the default and the setting the sponsor can change.",
        "## Summary\n\n" + summary,
        f"## Decide first: open decisions that block implementation ({len(first)})\n\n"
        + table(("ID", "Question", "Who answers"), [(r["question_id"], cell(r["question"]), r["owner_group"]) for r in first]),
        f"## 1. Open decisions ({len(od)})",
        *[s for s in (od_section(g) for g in GROUP_ORDER) if s],
        f"## 2. Provisional decisions awaiting confirmation ({len(ac)})",
        *[s for s in (ac_section(g) for g in GROUP_ORDER) if s],
        f"## 3. Open questions on proposed requirements ({len(rq)})\n\n"
        "Most of these are answered by the product owner, routing to counsel, finance or architecture where the question says so.\n\n"
        + table(("Record", "Question", "Mission"), rq_rows),
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


def main(argv):
    root = Path(argv[1]) if len(argv) > 1 else Path(".")
    rows = (tuple(r for base, label, report in SOURCES for r in open_decisions(root, base, label, report))
            + confirmations(root) + requirement_questions(root))
    (root / "requirements/OPEN-QUESTIONS.md").write_text(render(rows), encoding="utf-8")
    (root / "requirements/open-questions.csv").write_text(to_csv(rows), encoding="utf-8")
    print(dict(Counter(r["kind"] for r in rows)), "total", len(rows))
    print(dict(Counter(r["owner_group"] for r in rows)))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

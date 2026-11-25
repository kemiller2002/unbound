#!/usr/bin/env python3
"""Map every 2026-11-24 decision-review row to an exact register question (Phase 0 verification).

The eleven review artifacts in requirements/discovery/2026-11-24-*.md cite requirement
questions by record ID plus a short label, and decisions by ID plus (claimed) original
question text. This tool joins them to the generated register rows
(requirements/open-questions.csv) and reports:

  * coverage per question (which artifacts proposed, dispositioned, corrected or evidenced it)
  * review rows that cannot be mapped (orphans) and questions no artifact covers (gaps)
  * record-level count mismatches (more or fewer review rows than register questions)
  * decision rows whose quoted "original question" differs from the register text

Pure functions build every result; main() performs the I/O.
Usage: python3 -I -B tools/reqaudit/review_map.py <repo-root> <out-dir>
Writes <out-dir>/review-map.csv and <out-dir>/review-map-summary.json.
"""
import csv
import io
import json
import re
import sys
from collections import Counter
from functools import reduce
from pathlib import Path

DISC = Path("requirements/discovery")
PROPOSAL_ARTIFACTS = (
    "2026-11-24-initial-38-question-resolutions.md",
    "2026-11-24-human-intervention-and-question-resolution.md",
    "2026-11-24-question-resolutions-accounting-banking-economics.md",
    "2026-11-24-final-55-question-resolutions.md",
)
DISPOSITION_ARTIFACT = "2026-11-24-adversarial-approval-review.md"
CORRECTION_ARTIFACT = "2026-11-24-approved-corrections-and-decision-gates.md"
HOLD_ARTIFACT = "2026-11-24-three-evidence-hold-resolutions.md"
DECISION_ARTIFACT = "2026-11-24-open-decision-resolution-review.md"
CONFIRMATION_ARTIFACT = "2026-11-24-confirmation-question-review.md"
EVIDENCE_ARTIFACT = "2026-11-24-zero-consultation-evidence-review-01.md"
PO_ARTIFACT = "2026-11-24-product-owner-resolution-review.md"
OUT_FIELDS = ("question_id", "kind", "register_question", "proposal_artifact", "proposal_label",
              "proposed_answer", "proposal_gate", "disposition", "disposition_note", "correction",
              "hold_resolution", "decision_review_direction", "decision_review_gate",
              "confirmation_class", "confirmation_answer", "evidence_review", "quoted_question_matches",
              "mapping_flags")
ID_RE = re.compile(r"\b(?:RR|UNB|SPD|XID|RCD)-[A-Z0-9-]*\d(?:-r\d+)?\b")


def table_rows(text):
    """Every data row of every pipe table, as a tuple of stripped cells (header and rule rows dropped)."""
    lines = tuple(l.strip() for l in text.splitlines())
    cells = lambda l: tuple(c.strip() for c in re.split(r"(?<!\\)\|", l.strip("|")))
    is_rule = lambda l: bool(re.fullmatch(r"\|?(\s*:?-{3,}:?\s*\|)+\s*:?-*:?\s*\|?", l))
    def step(acc, pair):
        prev, l = pair
        in_table = l.startswith("|") and not is_rule(l)
        header = in_table and (prev is None or not prev.startswith("|"))
        return acc + ((cells(l),) if in_table and not header else ())
    return reduce(step, zip((None,) + lines[:-1], lines), ())


def register(root):
    return tuple(csv.DictReader(io.StringIO((root / "requirements/open-questions.csv").read_text(encoding="utf-8"))))


def record_of(qid):
    return qid.split("#")[0]


def ordered_assignment(review_rows, questions):
    """Assign review rows for one record to that record's questions in source order.

    Returns ((question_id or None, row), ...) plus a flag when the counts differ."""
    qids = tuple(q["question_id"] for q in sorted(questions, key=lambda q: int(q["question_id"].split("#Q")[1])))
    pairs = tuple((qids[i] if i < len(qids) else None, r) for i, r in enumerate(review_rows))
    flag = "" if len(review_rows) == len(qids) else f"count-mismatch:{len(review_rows)}-reviews-vs-{len(qids)}-questions"
    return pairs, flag


def assign_by_record(rows, reg_rq):
    """rows: tuple of (record, payload). Returns {question_id: payload}, orphans, flags."""
    records = tuple(dict.fromkeys(r for r, _ in rows))
    by_record = {rec: tuple(p for r, p in rows if r == rec) for rec in records}
    qs_by_record = {rec: tuple(q for q in reg_rq if record_of(q["question_id"]) == rec) for rec in records}
    results = tuple((rec,) + ordered_assignment(by_record[rec], qs_by_record[rec]) for rec in records)
    mapping = {qid: p for _, pairs, _ in results for qid, p in pairs if qid}
    orphans = tuple((rec, p) for rec, pairs, _ in results for qid, p in pairs if qid is None)
    flags = {rec: f for rec, _, f in results if f}
    return mapping, orphans, flags


def norm(t):
    return re.sub(r"[^a-z0-9]+", " ", (t or "").lower()).strip()


def holds(text):
    """Three evidence-hold sections: {'RR-REQ-X-073-r1': text, ...}."""
    parts = re.split(r"^## ", text, flags=re.M)
    return {p.split("\n", 1)[0].split(" / ")[0].strip(): " ".join(p.split("\n", 1)[1].split())
            for p in parts[1:] if p.split("\n", 1)[0].strip() != "Status"}


def evidence_sections(text):
    parts = re.split(r"^## ", text, flags=re.M)
    heads = tuple((p.split("\n", 1)[0], " ".join(p.split("\n", 1)[1].split()) if "\n" in p else "") for p in parts[1:])
    return {i: body for head, body in heads for i in ID_RE.findall(head)}


def build(root):
    reg = register(root)
    reg_by_id = {r["question_id"]: r for r in reg}
    reg_rq = tuple(r for r in reg if r["kind"] == "requirement question")
    read = lambda name: (root / DISC / name).read_text(encoding="utf-8")

    proposals = tuple((cells[0], {"proposal_artifact": name, "proposal_label": cells[1], "proposed_answer": cells[2],
                                  "proposal_gate": cells[3] if len(cells) > 3 else ""})
                      for name in PROPOSAL_ARTIFACTS for cells in table_rows(read(name)) if ID_RE.match(cells[0]))
    prop_map, prop_orphans, prop_flags = assign_by_record(proposals, reg_rq)

    disp = tuple((c[0], {"disposition": c[2], "disposition_note": c[3], "_label": c[1]})
                 for c in table_rows(read(DISPOSITION_ARTIFACT)) if ID_RE.match(c[0]))
    disp_map, disp_orphans, disp_flags = assign_by_record(disp, reg_rq)

    corr_rows = tuple(c for c in table_rows(read(CORRECTION_ARTIFACT)) if ID_RE.match(c[0]) and len(c) == 4 and c[2] == "revise")
    # Corrections repeat revise rows; key them by (record, label) and join through the disposition label.
    corr_by_label = {(c[0], c[1]): c[3] for c in corr_rows}
    hold_map = holds(read(HOLD_ARTIFACT))

    dec = {c[0]: c for c in table_rows(read(DECISION_ARTIFACT)) if ID_RE.fullmatch(c[0])}
    conf = {c[0]: c for c in table_rows(read(CONFIRMATION_ARTIFACT)) if ID_RE.fullmatch(c[0])}
    ev = evidence_sections(read(EVIDENCE_ARTIFACT))

    def row(q):
        qid, kind = q["question_id"], q["kind"]
        p, d = prop_map.get(qid, {}), disp_map.get(qid, {})
        label_mismatch = bool(p and d and norm(p["proposal_label"]) != norm(d["_label"]))
        hold_key = record_of(qid) if record_of(qid) in hold_map else ""
        dr, cr = dec.get(qid), conf.get(qid)
        quoted = (dr[1] if dr else cr[1] if cr else "")
        reg_q = q["question"].removeprefix("Confirm provisional decision: ")
        matches = "" if not quoted else ("yes" if norm(quoted) == norm(reg_q) else "no")
        flags = tuple(f for f in (
            "no-proposal" if kind == "requirement question" and not p else "",
            "no-disposition" if kind == "requirement question" and not d else "",
            "proposal-disposition-label-differs" if label_mismatch else "",
            prop_flags.get(record_of(qid), "") if kind == "requirement question" else "",
            "no-decision-review" if kind == "open decision" and not dr else "",
            "no-confirmation-review" if kind == "awaiting confirmation" and not cr else "",
            "quoted-question-differs" if matches == "no" else "",
        ) if f)
        return {"question_id": qid, "kind": kind, "register_question": q["question"],
                "proposal_artifact": p.get("proposal_artifact", ""), "proposal_label": p.get("proposal_label", ""),
                "proposed_answer": p.get("proposed_answer", ""), "proposal_gate": p.get("proposal_gate", ""),
                "disposition": d.get("disposition", ""), "disposition_note": d.get("disposition_note", ""),
                "correction": corr_by_label.get((record_of(qid), d.get("_label", "")), "") if d.get("disposition") == "revise" else "",
                "hold_resolution": hold_map.get(hold_key, "") if d.get("disposition") == "hold" else "",
                "decision_review_direction": dr[3] if dr else "", "decision_review_gate": dr[4] if dr else "",
                "confirmation_class": cr[2] if cr else "", "confirmation_answer": cr[3] if cr else "",
                "evidence_review": ev.get(qid, ""), "quoted_question_matches": matches,
                "mapping_flags": ";".join(flags)}

    rows = tuple(row(q) for q in reg)
    reg_ids = set(reg_by_id)
    summary = {
        "register": dict(Counter(r["kind"] for r in reg)), "register_total": len(reg),
        "proposal_rows": len(proposals), "proposal_rows_per_artifact": dict(Counter(p["proposal_artifact"] for _, p in proposals)),
        "proposal_orphans": [r for r, _ in prop_orphans], "proposal_count_mismatches": prop_flags,
        "disposition_rows": len(disp), "dispositions": dict(Counter(p["disposition"] for _, p in disp)),
        "disposition_orphans": [r for r, _ in disp_orphans], "disposition_count_mismatches": disp_flags,
        "correction_rows": len(corr_rows), "corrections_joined": sum(1 for r in rows if r["correction"]),
        "holds_joined": sum(1 for r in rows if r["hold_resolution"]),
        "decision_review_rows": len(dec), "decision_review_ids_not_open_decisions": sorted(i for i in dec if reg_by_id.get(i, {}).get("kind") != "open decision"),
        "confirmation_review_rows": len(conf), "confirmation_classes": dict(Counter(c[2] for c in conf.values())),
        "confirmation_review_ids_not_confirmations": sorted(i for i in conf if reg_by_id.get(i, {}).get("kind") != "awaiting confirmation"),
        "evidence_review_ids": sorted(ev), "evidence_review_ids_unknown": sorted(set(ev) - reg_ids),
        "flag_counts": dict(Counter(f for r in rows for f in r["mapping_flags"].split(";") if f)),
        "questions_with_flags": [r["question_id"] for r in rows if r["mapping_flags"]],
    }
    return rows, summary


def to_csv(rows):
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=OUT_FIELDS, quoting=csv.QUOTE_ALL, lineterminator="\n")
    w.writeheader()
    w.writerows(rows)
    return buf.getvalue()


def main(argv):
    root, out = Path(argv[1]), Path(argv[2])
    rows, summary = build(root)
    out.mkdir(parents=True, exist_ok=True)
    (out / "review-map.csv").write_text(to_csv(rows), encoding="utf-8")
    (out / "review-map-summary.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(summary, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

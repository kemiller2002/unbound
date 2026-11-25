#!/usr/bin/env python3
"""Seed requirements/questions/resolution-events.jsonl from the 2026-11-24 decision-review artifacts.

Run once to create the log (refuses to overwrite an existing log unless --rebuild is given, which is
only safe before any later events have been appended). Every event cites the artifact it came from, so
the log reproduces the review history exactly; nothing is inferred beyond what the artifacts state.

  registered          exact source text + SHA-256, register kind, duplicate canonical (question-clusters.csv)
  proposed            12 product-owner review sections, the four requirement-question proposal tables,
                      the 101-decision review and the 72-confirmation review
  dispositioned       adversarial approval review (approved / revise / hold) for the 143 requirement questions
  corrected           approved corrections (29) replacing the proposed wording
  evidence-reviewed   three evidence-hold resolutions; zero-consultation evidence review 01 (6 questions)

Usage: python3 -I -B tools/reqaudit/seed_resolutions.py <repo-root> [--rebuild]
"""
import json
import re
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import questions_lib as ql  # noqa: E402
import review_map as rmap  # noqa: E402

ON = "2026-11-24"
D = "requirements/discovery/"
ACTOR_REVIEW = "Prior agent session under product-owner delegation (2026-11-24 decision review)"


def po_review_sections(text):
    """{decision_id: review text} from the product-owner resolution review."""
    parts = re.split(r"^### ", text, flags=re.M)[1:]
    def review(body):
        m = re.search(r"^Review:\s*(.+)$", body, flags=re.M)
        return m.group(1).strip() if m else ""
    return {p.split("\n", 1)[0].strip(): review(p) for p in parts}


def events_for(q, reg_row, clusters, po):
    qid, kind = q["question_id"], q["kind"]
    canon = next((c["canonical_id"] for c in clusters if c["member_id"] == qid and c["relation"] == "same-question"), "")
    src = reg_row["source"].split(" (")[0]
    text = reg_row["question"].removeprefix("Confirm provisional decision: ") if kind != "requirement question" else reg_row["question"]
    registered = {"type": "registered", "artifact": "requirements/open-questions.csv", "actor": "tools/reqaudit/render_questions.py (PR #7)",
                  "set": {"register_kind": kind, "source_record": qid.split("#")[0], "source_path": src,
                          "question_text": text, "question_sha256": ql.sha(text), "canonical_id": canon}}
    po_event = ({"type": "proposed", "artifact": D + rmap.PO_ARTIFACT, "actor": ACTOR_REVIEW,
                 "set": {"answer": po[qid], "resolution_status": "proposed"}},) if qid in po and po[qid] else ()
    if kind == "requirement question":
        disp = q["disposition"]
        approval = {"approval_status": "approved-product-direction", "approved_by": "Product owner (delegated adversarial review)",
                    "approved_on": ON}
        rest = (
            {"type": "proposed", "artifact": D + q["proposal_artifact"], "actor": ACTOR_REVIEW,
             "set": {"answer": q["proposed_answer"], "validation_gate": q["proposal_gate"], "resolution_status": "proposed"}},
            {"type": "dispositioned", "artifact": D + rmap.DISPOSITION_ARTIFACT, "actor": ACTOR_REVIEW,
             "set": {"review_disposition": disp, "review_note": q["disposition_note"],
                     **(approval if disp == "approved" else {})}},
        ) + (({"type": "corrected", "artifact": D + rmap.CORRECTION_ARTIFACT, "actor": "Product owner authorization (approved corrections)",
               "set": {"answer": q["correction"], "superseded_answer": q["proposed_answer"], **approval}},) if disp == "revise" else ()) \
          + (({"type": "evidence-reviewed", "artifact": D + rmap.HOLD_ARTIFACT, "actor": "Product-design direction approved under delegated review",
               "set": {"answer": q["hold_resolution"], "superseded_answer": q["proposed_answer"], **approval}},) if disp == "hold" else ())
    elif kind == "open decision":
        rest = ({"type": "proposed", "artifact": D + rmap.DECISION_ARTIFACT, "actor": ACTOR_REVIEW,
                 "set": {"answer": q["decision_review_direction"], "validation_gate": q["decision_review_gate"],
                         "resolution_status": "proposed", "approval_status": "approved-product-direction",
                         "approved_by": "Product owner (provisional acceptance of reviewed directions)", "approved_on": ON}},)
    else:
        rest = ({"type": "proposed", "artifact": D + rmap.CONFIRMATION_ARTIFACT, "actor": ACTOR_REVIEW,
                 "set": {"answer": q["confirmation_answer"], "review_class": q["confirmation_class"],
                         "resolution_status": "proposed"}},)
    ev = ({"type": "evidence-reviewed", "artifact": D + rmap.EVIDENCE_ARTIFACT, "actor": ACTOR_REVIEW,
           "set": {"evidence_note": q["evidence_review"]}},) if q["evidence_review"] else ()
    return tuple(dict(e, question_id=qid, on=ON) for e in (registered,) + po_event + rest + ev)


def build(root):
    rows, _ = rmap.build(root)
    reg = {r["question_id"]: r for r in rmap.register(root)}
    clusters = ql.load_csv(root / "requirements/questions/question-clusters.csv")
    po = po_review_sections((root / D / rmap.PO_ARTIFACT).read_text(encoding="utf-8"))
    evs = tuple(e for q in rows for e in events_for(q, reg[q["question_id"]], clusters, po))
    return tuple(dict(e, event_id=f"QE-{i:06d}") for i, e in enumerate(evs, 1))


def main(argv):
    root = Path(argv[1])
    out = root / ql.EVENTS
    if out.exists() and "--rebuild" not in argv:
        print(f"{out} exists; refusing to overwrite (append events instead)")
        return 1
    out.parent.mkdir(parents=True, exist_ok=True)
    events = build(root)
    out.write_text("".join(json.dumps({k: e[k] for k in ("event_id", "question_id", "on", "type", "artifact", "actor", "set")},
                                      ensure_ascii=False) + "\n" for e in events), encoding="utf-8")
    print(len(events), "events written to", out)
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

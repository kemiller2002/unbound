#!/usr/bin/env python3
"""Apply the revisions from the product-owner review of 2026-11-24 to the source records (additions only).

Input: requirements/resolutions/2026-11-24/revisions.jsonl and resolutions.jsonl.

  * A revision of a refinement (RR-<orig>-r<n>) becomes a new superseding
    refinement RR-<orig>-r<n+1> in the simplification mission's refinements.jsonl,
    copying the predecessor's original_text and fields; the predecessor is unchanged.
  * A revision of a gap requirement (UNB-XIN-*, UNB-SMP-*) is appended to that
    record's `revisions` list; the record's original `statement` is unchanged, so
    text adopted verbatim into domain files stays traceable.
  * Domain files that adopted a revised record get a one-line pointer under its
    provenance line.
  * Trace edges are added for each new refinement.

Idempotent: re-running adds nothing that already exists.

    python3 -I -B tools/reqaudit/apply_revisions_2026_11_24.py .
"""
import csv
import io
import json
import sys
from pathlib import Path

RES = Path("requirements/resolutions/2026-11-24")
SIMP = Path("requirements/discovery/2026-11-24-simplification-economics/data")
DATA_DIRS = (Path("requirements/discovery/2026-11-23-cross-industry/data"), SIMP)
DATE = "2026-11-24"
ADOPTED = {  # record -> domain file that adopted it verbatim
    "UNB-XIN-ERP-001": "requirements/FULL-ACCOUNTING-SYSTEM.md",
    "UNB-XIN-BNK-003": "requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md",
}


def read_jsonl(p):
    return [json.loads(l) for l in p.read_text(encoding="utf-8").splitlines() if l.strip()]


def write_jsonl(p, rows):
    p.write_text("".join(json.dumps(r, ensure_ascii=False) + "\n" for r in rows), encoding="utf-8")


def rid(r):
    return r.get("refinement_id") or r.get("id")


def new_refinement(pred, rev, res, scenarios):
    acs = list(pred.get("acceptance_criteria") or []) + rev["added_acceptance_criteria"]
    return dict(pred, **{
        "validation_scenarios": list(pred.get("validation_scenarios") or scenarios),
        ("refinement_id" if "refinement_id" in pred else "id"): rev["revision_id"],
        **({"id": rev["revision_id"]} if "id" in pred else {}),
        "statement": rev["revised_statement"],
        "acceptance_criteria": acs,
        "supersedes_refinement": rev["target"],
        "reason_for_change": f"Product-owner review {DATE} ({res['resolution_id']}): {res['answer']}",
        "validation_status": "unvalidated",
        "approval_status": "proposed",
        "open_questions": [],
        "resolution_ids": [res["resolution_id"]],
        "_group": "integrator",
    })


def gap_revision(rev, res):
    return {"revision_id": rev["revision_id"], "date": DATE, "resolution_id": res["resolution_id"], "status": "proposed",
            "reason": res["answer"], "statement": rev["revised_statement"],
            "added_acceptance_criteria": rev["added_acceptance_criteria"]}


def with_revision(record, entry):
    have = {r["revision_id"] for r in record.get("revisions", [])}
    return record if entry["revision_id"] in have else dict(record, revisions=list(record.get("revisions", [])) + [entry])


def edges_for(r):
    return ([(rid(r), "refines", r["original_id"], "refinements.jsonl"), (rid(r), "supersedes", r["supersedes_refinement"], "refinements.jsonl")]
            + [(rid(r), "tested_by", s, "scenarios.csv") for s in r.get("validation_scenarios") or []])


def main(argv):
    root = Path(argv[1]) if len(argv) > 1 else Path(".")
    revs = read_jsonl(root / RES / "revisions.jsonl")
    res = {r["revision"]: r for r in read_jsonl(root / RES / "resolutions.jsonl") if r.get("revision")}
    sources = {d: {n: read_jsonl(root / d / n) for n in ("refinements.jsonl", "gap-requirements.jsonl")} for d in DATA_DIRS}
    index = {rid(r): (d, n) for d, files in sources.items() for n, rows in files.items() for r in rows}

    rr = [v for v in revs if v["target"].startswith("RR-")]
    gaps = [v for v in revs if not v["target"].startswith("RR-")]
    preds = {v["target"]: next(r for r in sources[index[v["target"]][0]][index[v["target"]][1]] if rid(r) == v["target"]) for v in rr}
    def tested(d, target):  # predecessor scenarios recorded only as trace edges (cross-industry mission)
        edges = csv.DictReader(io.StringIO((root / d / "trace-edges.csv").read_text(encoding="utf-8")))
        return [e["target"] for e in edges if e["source"] == target and e["relationship"] == "tested_by"]
    new_rr = [new_refinement(preds[v["target"]], v, res[v["revision_id"]], tested(index[v["target"]][0], v["target"]))
              for v in rr if v["revision_id"] not in index]
    simp_ref = sources[SIMP]["refinements.jsonl"] + new_rr
    gap_map = {v["target"]: gap_revision(v, res[v["revision_id"]]) for v in gaps}
    for d, files in sources.items():
        updated = [with_revision(r, gap_map[rid(r)]) if rid(r) in gap_map else r for r in files["gap-requirements.jsonl"]]
        write_jsonl(root / d / "gap-requirements.jsonl", updated)
    write_jsonl(root / SIMP / "refinements.jsonl", simp_ref)

    # trace edges for new refinements (simplification mission)
    te_path = root / SIMP / "trace-edges.csv"
    te = list(csv.DictReader(io.StringIO(te_path.read_text(encoding="utf-8"))))
    fields = list(te[0].keys())
    have = {tuple(r.values()) for r in te}
    add = [dict(zip(fields, e)) for r in simp_ref if r.get("supersedes_refinement") for e in edges_for(r) if e not in have]
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=fields, lineterminator="\r\n")
    w.writeheader()
    w.writerows(te + add)
    te_path.write_bytes(buf.getvalue().encode("utf-8"))

    # one-line pointers in domain files that adopted a revised record verbatim
    for record, path in ADOPTED.items():
        if record not in gap_map:
            continue
        p = root / path
        text = p.read_text(encoding="utf-8")
        marker = f"  - Revision proposed {DATE} ({gap_map[record]['resolution_id']})"
        if marker in text:
            continue
        lines = text.split("\n")
        start = next(i for i, l in enumerate(lines) if l.startswith(f"- **{record}:**"))
        prov = next(i for i in range(start, len(lines)) if lines[i].startswith("  - Provenance:"))
        note = (f"{marker}: the text above is kept verbatim; the proposed revised statement is in "
                f"`requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl` (`revisions` of {record}). "
                f"Reason: {gap_map[record]['reason']}")
        write = "\n".join(lines[:prov + 1] + [note] + lines[prov + 1:])
        p.write_text(write, encoding="utf-8")
    print(f"{len(new_rr)} superseding refinements, {len(gap_map)} gap-requirement revisions, {len(add)} trace edges")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

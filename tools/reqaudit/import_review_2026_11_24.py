#!/usr/bin/env python3
"""One-off import of the product-owner review of 2026-11-24 into keyed resolution records.

Reads the eleven review files under requirements/discovery/2026-11-24-*.md and
requirements/resolutions/2026-11-24/revisions.jsonl, matches each answer to the
exact question it answers, and writes requirements/resolutions/2026-11-24/resolutions.jsonl.

Matching rules:
  * requirement questions: the review lists each record's questions in the same
    order as the record's open_questions; labels are matched by position within
    the record and the count per record must agree;
  * open decisions and confirmations: matched by decision id.
The script refuses to write if any question is unmatched or any answer is left over.

    python3 -I -B tools/reqaudit/import_review_2026_11_24.py .
"""
import json
import re
import sys
from collections import defaultdict
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import render_questions as RQ  # noqa: E402

REVIEW = Path("requirements/discovery")
OUT = Path("requirements/resolutions/2026-11-24")
DECIDED_BY = "Product owner (delegated review, 2026-11-24)"
DATE = "2026-11-24"
PROPOSALS = ("2026-11-24-initial-38-question-resolutions.md", "2026-11-24-final-55-question-resolutions.md",
             "2026-11-24-human-intervention-and-question-resolution.md",
             "2026-11-24-question-resolutions-accounting-banking-economics.md")
EXTERNAL = ("counsel", "cpa", "auditor", "board", "bank", "acquirer", "odfi", "custodian", "securities", "regulator",
            "controller", "cfo", "tax advisor", "investment committee", "contract", "sponsor", "compliance", "treasury",
            "privacy", "security validates", "auditor", "obtain", "ops")
VALIDATION = ("test", "measure", "pilot", "calibrat", "simulat", "validate", "verify", "compare", "stress", "sensitivity",
              "research", "usability", "monitor", "model", "check", "evidence", "benchmark", "back-test", "replay")
HOLDS = {  # condensed from 2026-11-24-three-evidence-hold-resolutions.md
    ("RR-REQ-X-073-r1", "3/12 month thresholds"): (
        "Approve 3-month bridge request and 12-month stale-report alert as provisional monitoring defaults, not universal audit "
        "standards. Bridge letters do not extend independent assurance; track coverage gap, report scope, opinion, exceptions, "
        "complementary controls and time-limited risk acceptance; critical vendor suspension needs a risk-based continuity decision.",
        "Auditor confirms reliance criteria; test bridge letter with unmapped CUEC, adverse opinion, expired risk acceptance and vendor outage."),
    ("RR-UNB-GOV-002-r1", "Auditor acceptance"): (
        "Reject the blanket one-approver tightening exception. Independent approval for material changes to financial controls, "
        "eligibility, permissions or customer rights regardless of direction; streamlined low-risk changes only within an "
        "independently approved change policy; ambiguous or mixed changes are material. The estimated 50 hours/year saving is unvalidated.",
        "Auditor/CPA confirms reliance; back-test representative history plus synthetic edge cases (lowered limit causing deadline "
        "breaches, compound changes, author self-approval, missing evidence)."),
    ("UNB-XIN-ECO-005", "FOCUS version"): (
        "Pin FOCUS 1.4 (ratified 4 June 2026) as the internal mapping target, not a mandatory provider format; version-aware "
        "adapters; preserve native invoice and usage provenance; extensions x_Bearer, x_SourceInvoiceRef, x_CostObject; no "
        "conformance claim for a subset; invoice-only ingestion allowed for low-volume vendors; no automatic upgrade to later versions.",
        "Test older schemas, corrections, credits, currencies, residuals and duplicate invoices."),
}
EVIDENCE = {  # from 2026-11-24-zero-consultation-evidence-review-01.md
    "XID-NPG-007": "Established: 26 USC 4962(b) and 4963(a) include 4966(a) and 4967(a) taxes as first-tier; abatement under 4962(a) needs reasonable cause, no willful neglect and timely correction. Sources: uscode.house.gov t26 s4962, s4963, s4966, s4967 (accessed 2026-11-24).",
    "RCD-044": "Established: OFAC blocking and rejection are distinct; reports within 10 business days of the reportable action; blocked property also reported annually. Sources: ofac.treasury.gov FAQs and OFAC introduction (accessed 2026-11-24).",
    "RCD-014": "Established statutory baseline: 4966(c) treats distributions to natural persons as taxable and sets charitable-purpose and expenditure-responsibility rules, with the 170(b)(1)(A) exception excluding disqualified supporting organizations; 4967 applies separately. Source: 26 USC 4966, 4967.",
    "RCD-016": "Preliminary: IRS Notice 2017-73 (2017-51 IRB) describes pledge treatment; current reliance language and later guidance still to verify.",
    "XID-INV-005": "Preliminary: Schedule D instructions distinguish statutory DAF classification from GAAP fund definitions; line-specific instructions still to verify.",
    "RCD-024": "Preliminary: Schedule D instructions distinguish statutory DAF classification from GAAP fund definitions; line-specific instructions still to verify.",
}
NOTES = {  # integrator check findings: conflicts with recorded owner decisions
    "SPD-ORG-002": "Integrator check: under SPD-ADV-001 a fractional officer may hold A3+ authority for one sponsor only (UNB-SMP-ORG-003 refuses a second); fractional officers serving several sponsors cannot decide for more than one.",
    "SPD-ECN-004": "Integrator check: under SPD-ADV-001 a pre-contracted surge bench may prepare work for several sponsors but a bench member may decide only as an appointed member of one sponsor's DecisionTeam.",
}


def md_tables(text):
    out, hdr = [], None
    for line in text.splitlines():
        if not line.startswith("|"):
            hdr = None
            continue
        cells = [c.strip() for c in re.split(r"(?<!\\)\|", line.strip())[1:-1]]
        if all(re.fullmatch(r":?-+:?", c) for c in cells if c):
            continue
        if hdr is None:
            hdr = cells
        else:
            out.append(dict(zip(hdr, cells)))
    return tuple(out)


def classify_gate(text):
    t = " " + RQ.normalize(text) + " "
    if t.strip() in ("", "none", "none."):
        return "none"
    hits = sorted([(t.find(k), "external") for k in EXTERNAL if k in t] + [(t.find(k), "validation") for k in VALIDATION if k in t])
    return hits[0][1] if hits else "none"


def next_revision(root, target):
    m = re.fullmatch(r"RR-(.+)-r(\d+)", target)
    if not m:
        return f"{target} revision 1"
    rs = [int(x.group(1)) for f in root.glob("requirements/**/data/refinements.jsonl")
          for l in f.read_text(encoding="utf-8").splitlines() if l.strip()
          for x in [re.fullmatch(rf"RR-{re.escape(m.group(1))}-r(\d+)", json.loads(l).get("refinement_id") or json.loads(l).get("id", ""))] if x]
    return f"RR-{m.group(1)}-r{max(rs) + 1}"


def resolution(qrow, n, disposition, answer, gate, gate_type, **extra):
    return dict({"resolution_id": f"QR-{n:04d}", "question_key": qrow["question_key"], "record_id": qrow["record_id"],
                 "kind": qrow["kind"], "question": qrow["question"], "disposition": disposition, "answer": answer,
                 "gate": gate, "gate_type": gate_type, "answered_by": DECIDED_BY, "answered_on": DATE}, **extra)


def build(root):
    tabs = {f: md_tables((root / REVIEW / f).read_text(encoding="utf-8")) for f in
            [p.name for p in sorted((root / REVIEW).glob("2026-11-24-*.md"))]}
    questions = RQ.collect(root)
    revisions = {r["target"]: r for r in RQ.load_jsonl(root / OUT / "revisions.jsonl")}
    req = defaultdict(list)
    for q in (q for q in questions if q["kind"] == RQ.KINDS[2]):
        req[q["record_id"]].append(q)
    adv = defaultdict(list)
    for r in tabs["2026-11-24-adversarial-approval-review.md"]:
        adv[r["Original record"]].append(r)
    prop = {(r["Original record"], r["Question"]): r for f in PROPOSALS for r in tabs[f]}
    mismatched = sorted(k for k in set(req) | set(adv) if len(req[k]) != len(adv[k]))
    if mismatched:
        raise SystemExit(f"question/answer count mismatch for {mismatched}")

    def req_resolution(q, a, n):
        p = prop[(a["Original record"], a["Question"])]
        gate = p.get("Challenge / validation gate") or p.get("Validation / challenge", "")
        label = a["Question"]
        if a["Disposition"] == "hold":
            answer, gate = HOLDS[(a["Original record"], label)]
            disp = "hold-resolved"
        elif a["Disposition"] == "revise":
            answer, disp = a["Review / correction"], "revised"
        else:
            answer, disp = p["Proposed answer"], "approved"
        rev = revisions.get(a["Original record"])
        return resolution(q, n, disp, answer, gate, classify_gate(gate), label=label,
                          superseded_answer=p["Proposed answer"] if disp != "approved" else "",
                          revision=rev["revision_id"] if rev and rev["question_label"] == label else "",
                          review_sources=["2026-11-24-adversarial-approval-review.md"] + [f for f in PROPOSALS if (a["Original record"], label) in {(r["Original record"], r["Question"]) for r in tabs[f]}])

    pairs = [(q, a) for rid in sorted(req) for q, a in zip(req[rid], adv[rid])]
    out = [req_resolution(q, a, i) for i, (q, a) in enumerate(pairs, 1)]

    dec = {r["ID"]: r for r in tabs["2026-11-24-open-decision-resolution-review.md"]}
    cross = {i.strip(): r for r in tabs["2026-11-24-approved-corrections-and-decision-gates.md"] if "Decision IDs" in r
             for i in r["Decision IDs"].split(",")}
    od = [q for q in questions if q["kind"] == RQ.KINDS[0]]
    missing = [q["record_id"] for q in od if q["record_id"] not in dec]
    if missing or len(dec) != len(od):
        raise SystemExit(f"open decisions unmatched: {missing}; review rows {len(dec)} vs questions {len(od)}")

    def dec_resolution(q, n):
        d, c = dec[q["record_id"]], cross.get(q["record_id"])
        answer = c["Product direction"] if c else d["Reviewed direction"]
        gate = c["Remaining gate"] if c else d["Challenge / remaining gate"]
        return resolution(q, n, "direction-accepted", answer, gate, classify_gate(gate),
                          review_challenge=d["Challenge / remaining gate"] if c else "",
                          integrator_note=NOTES.get(q["record_id"], ""),
                          review_sources=["2026-11-24-open-decision-resolution-review.md"]
                          + (["2026-11-24-approved-corrections-and-decision-gates.md"] if c else []))
    out += [dec_resolution(q, len(out) + i) for i, q in enumerate(sorted(od, key=lambda q: q["record_id"]), 1)]

    conf = {r["ID"]: r for r in tabs["2026-11-24-confirmation-question-review.md"]}
    ac = [q for q in questions if q["kind"] == RQ.KINDS[1]]
    missing = [q["record_id"] for q in ac if q["record_id"] not in conf]
    if missing or len(conf) != len(ac):
        raise SystemExit(f"confirmations unmatched: {missing}; review rows {len(conf)} vs questions {len(ac)}")
    kind_of = {"External": "external", "Validation": "validation", "Product": "none"}
    out += [resolution(q, len(out) + i, "direction-accepted", conf[q["record_id"]]["Reviewed answer / safe assumption"],
                       conf[q["record_id"]]["Original confirmer"], kind_of[conf[q["record_id"]]["Classification"]],
                       evidence=EVIDENCE.get(q["record_id"], ""),
                       review_sources=["2026-11-24-confirmation-question-review.md"]
                       + (["2026-11-24-zero-consultation-evidence-review-01.md"] if q["record_id"] in EVIDENCE else []))
            for i, q in enumerate(sorted(ac, key=lambda q: q["record_id"]), 1)]
    return out


def main(argv):
    root = Path(argv[1]) if len(argv) > 1 else Path(".")
    out = build(root)
    (root / OUT / "resolutions.jsonl").write_text("".join(json.dumps(r, ensure_ascii=False) + "\n" for r in out), encoding="utf-8")
    print(len(out), "resolutions;", sum(1 for r in out if r.get("revision")), "linked to revisions")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

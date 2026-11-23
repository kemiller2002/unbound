#!/usr/bin/env python3
"""Deterministic requirement-quality heuristics over the inventory produced by inventory.py.

Pure functions map each canonical requirement record to a tuple of quality signals.
Signals are heuristics for triage, not verdicts: every material defect reported in the
reconciliation findings register was confirmed by reading the requirement.

Usage:
    python3 -I tools/reqaudit/quality.py <inventory-dir> <repo-root>
Writes <inventory-dir>/requirements-quality.csv and prints aggregate counts.
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Dict, Tuple

RFC2119 = re.compile(r"\b(SHALL NOT|SHALL|MUST NOT|MUST|SHOULD NOT|SHOULD|MAY|REQUIRED)\b")
NORMATIVE = re.compile(r"\b(SHALL|MUST|SHOULD|MAY|REQUIRED|never|cannot|must|shall|requires?|prohibit\w*|block\w*|reject\w*)\b")
VAGUE = re.compile(r"\b(support\w*|flexible|appropriate(ly)?|where (relevant|applicable|needed|warranted|justified)|as needed|"
                   r"reasonabl\w*|robust|seamless\w*|user-friendly|comprehensive|etc\.?|various|adequate\w*|"
                   r"sufficient\w*|proportionate|optimi[sz]e\w*|enhanc\w*|intelligent\w*|easily)\b", re.I)
ACCEPTANCE = re.compile(r"\b(Given|acceptance|accept(ed)?|test(s|ed)?|verify|verified|assert|oracle|scenario|ADV-\d+)\b", re.I)
TEST_FILE = re.compile(r"(TESTS?|SCENARIOS?|VALIDATION|scenarios)\.csv$|ADVERSARIAL", re.I)
QUESTION_FAMILIES = re.compile(r"-Q$")


def canonical(rows: Tuple[dict, ...]) -> Dict[str, dict]:
    """Prefer the requirements/ definition, then the longest text, as the canonical site."""
    def rank(r: dict):
        return (0 if r["path"].startswith("requirements/") else 1, -len(r["text"]))
    ids = sorted({r["identifier"] for r in rows})
    return {i: sorted((r for r in rows if r["identifier"] == i), key=rank)[0] for i in ids}


def referenced_by_test_records(all_rows: Tuple[dict, ...]) -> frozenset:
    """Requirement ids cited inside the text of any test or scenario record."""
    return frozenset(i for r in all_rows if r["kind"] in ("test", "scenario")
                     for i in re.findall(r"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}\b", r["text"]))


def files_with_tests(all_rows: Tuple[dict, ...]) -> frozenset:
    return frozenset(r["path"] for r in all_rows if r["kind"] == "test")


def referenced_in_tests(repo: Path) -> frozenset:
    files = tuple(p for p in repo.rglob("*") if p.is_file() and TEST_FILE.search(p.name)
                  and "reconciliation" not in str(p))
    text = " ".join(p.read_text(encoding="utf-8", errors="replace") for p in files)
    return frozenset(re.findall(r"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-\d{1,4}\b", text))


def signals(rec: dict, tested: frozenset, test_files: frozenset = frozenset()) -> dict:
    body = rec["text"]
    clauses = len(re.findall(r";|\band\b|,", body))
    vague = sorted({m.group(0).lower() for m in VAGUE.finditer(body)})
    return {
        "identifier": rec["identifier"],
        "family": rec["family"],
        "path": rec["path"],
        "line": rec["line"],
        "words": len(body.split()),
        "has_normative_modal": bool(NORMATIVE.search(body)),
        "has_rfc2119_keyword": bool(RFC2119.search(body)),
        "vague_terms": ";".join(vague),
        "clause_count": clauses,
        "compound": clauses >= 8,
        "inline_acceptance": bool(ACCEPTANCE.search(body)),
        "referenced_by_test_artifact": rec["identifier"] in tested,
        "acceptance_signal": bool(ACCEPTANCE.search(body)) or rec["identifier"] in tested,
        "file_has_test_records": rec["path"] in test_files,
    }


def main(argv) -> int:
    inv, repo = Path(argv[1]), Path(argv[2])
    all_rows = tuple(csv.DictReader((inv / "inventory.csv").open(encoding="utf-8")))
    rows = tuple(r for r in all_rows
                 if r["kind"] == "requirement" and not QUESTION_FAMILIES.search(r["identifier"]))
    tested = referenced_in_tests(repo) | referenced_by_test_records(all_rows)
    out = tuple(signals(r, tested, files_with_tests(all_rows)) for r in canonical(rows).values())
    with (inv / "requirements-quality.csv").open("w", newline="", encoding="utf-8") as fh:
        w = csv.DictWriter(fh, fieldnames=tuple(out[0]))
        w.writeheader()
        w.writerows(out)
    n = len(out)
    agg = {
        "requirements_canonical": n,
        "no_normative_modal": sum(not o["has_normative_modal"] for o in out),
        "no_rfc2119_keyword": sum(not o["has_rfc2119_keyword"] for o in out),
        "with_vague_terms": sum(bool(o["vague_terms"]) for o in out),
        "compound_ge_8_clauses": sum(o["compound"] for o in out),
        "no_acceptance_signal": sum(not o["acceptance_signal"] for o in out),
        "referenced_by_test_artifact": sum(o["referenced_by_test_artifact"] for o in out),
        "no_acceptance_signal_and_no_colocated_tests": sum(not o["acceptance_signal"] and not o["file_has_test_records"] for o in out),
        "vague_term_frequency": dict(Counter(t for o in out for t in o["vague_terms"].split(";") if t).most_common(15)),
        "by_family": dict(sorted(Counter(o["family"] for o in out).items())),
    }
    (inv / "requirements-quality-summary.json").write_text(json.dumps(agg, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({k: v for k, v in agg.items() if k != "by_family"}, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

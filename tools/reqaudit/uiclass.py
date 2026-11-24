#!/usr/bin/env python3
"""Classify every canonical requirement by whether it implies a user-facing surface.

Classes (first matching rule wins, in this order):
  human-decision   a person must decide, approve, review or sign off -> needs an action surface
  document-output  produces a document a person reads (statement, acknowledgment, letter, package, PDF)
  ui-bearing       explicitly about screens, views, notifications, accessibility or user-visible state
  backend          none of the above: enforced by the platform without a dedicated surface

This is a transparent keyword heuristic (matched terms are recorded per row) used to cross-check
the surface inventory derived from workflow models; it is not a substitute for reading the requirement.

Usage: python3 -I tools/reqaudit/uiclass.py <repo-root>
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import Counter
from pathlib import Path
from typing import Tuple

RECON = Path("requirements/reconciliation/2026-11-23")
RULES: Tuple[Tuple[str, re.Pattern], ...] = (
    ("human-decision", re.compile(r"\b(human|authori[sz]ed (?:person|reviewer|approver|staff)|reviewer|approv(?:e|al|er)s?|sign[- ]?off|certif(?:y|ication)|escalat\w*|adjudicat\w*|committee|override|disposition|attest\w*|manual)\b", re.I)),
    ("document-output", re.compile(r"\b(statement|acknowledg\w*|receipt|(?<!dead-)letter|remittance|report(?:s|ing)? package|PDF|Folio|document(?:s)? (?:issu|generat|render)\w*|audit package|evidence package|Schedule D|Form 8283|Form 8282)\b", re.I)),
    ("ui-bearing", re.compile(r"\b(UI|screen|view|display\w*|show\w*|portal|dashboard|workspace|inbox|queue|Forma|mobile|phone|WCAG|accessib\w*|notif\w*|email|widget|preview|visible|selector|user[- ]visible|donors? (?:see|can)|self[- ]service|(?<!indexes )search(?! index)|wizard|form(?!at)|explain\w*|terminology|brand|ask)\b", re.I)),
)


def classify(text: str) -> Tuple[str, str]:
    for cls, rx in RULES:
        hits = sorted({m.group(0).lower() for m in rx.finditer(text)})
        if hits:
            return cls, ";".join(hits[:6])
    return "backend", ""


def main(argv) -> int:
    root = Path(argv[1])
    data = root / RECON / "data"
    inv = {r["identifier"]: r for r in csv.DictReader((data / "inventory.csv").open(encoding="utf-8"))}
    q = tuple(csv.DictReader((data / "requirements-quality.csv").open(encoding="utf-8")))
    rows = tuple((r["identifier"], r["family"], *classify(inv[r["identifier"]]["text"])) for r in q)
    with (data / "requirement-ui-classification.csv").open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh, quoting=csv.QUOTE_ALL)
        w.writerow(("identifier", "family", "class", "matched_terms"))
        w.writerows(rows)
    by = Counter(r[2] for r in rows)
    fam = {}
    for i, f, c, _ in rows:
        fam.setdefault(f, Counter())[c] += 1
    out = {"denominator": len(rows), "by_class": dict(by),
           "by_family": {f: dict(c) for f, c in sorted(fam.items(), key=lambda kv: -sum(kv[1].values()))}}
    (data / "requirement-ui-classification-summary.json").write_text(json.dumps(out, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({k: v for k, v in out.items() if k != "by_family"}, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

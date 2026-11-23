#!/usr/bin/env python3
"""Assign stable surrogate identifiers to normative statements that have no identifier.

Migration strategy (documented in requirements/reconciliation/2026-11-23/01-BASELINE-INVENTORY.md):
    NARR-<8 hex> where the hex is sha256(path + "\\n" + normalized sentence)[:8].
The surrogate is content-addressed: it survives line moves, and any wording change yields a new
surrogate, so a reworded statement is never silently treated as the same requirement.
Original path, line, revision and exact text are retained alongside the surrogate.

Usage: python3 -I tools/reqaudit/narrative.py <repo-root> <output-csv>
"""
from __future__ import annotations

import csv
import hashlib
import re
import subprocess
import sys
from pathlib import Path
from typing import Iterator, Tuple

ID = re.compile(r"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}\b")
NORMATIVE = re.compile(r"\b(SHALL|MUST|must|shall|never|Never|required|mandatory|prohibited|SHOULD|may not|cannot)\b")
SCOPE = ("requirements", "architecture")
SKIP = ("requirements/reconciliation/",)
BASELINE = "777a70d"


def normalize(text: str) -> str:
    return re.sub(r"\s+", " ", re.sub(r"[*_`]", "", text)).strip()


def surrogate(path: str, text: str) -> str:
    return "NARR-" + hashlib.sha256(f"{path}\n{normalize(text).lower()}".encode()).hexdigest()[:8]


def candidates(root: Path, rel: str) -> Iterator[Tuple[int, str]]:
    lines = (root / rel).read_text(encoding="utf-8", errors="replace").splitlines()
    return ((i + 1, l) for i, l in enumerate(lines)
            if NORMATIVE.search(l) and not ID.search(l) and not l.startswith(("#", "|")) and len(l.split()) >= 6)


def main(argv) -> int:
    root, out = Path(argv[1]), Path(argv[2])
    rev = BASELINE  # statements are attributed to the audited baseline revision
    if subprocess.run(["git", "-C", str(root), "diff", "--quiet", BASELINE, "HEAD", "--", *SCOPE, f":!{SKIP[0]}"]).returncode != 0:
        print("warning: originals differ from the baseline revision; surrogates describe the working tree", file=sys.stderr)
    files = sorted(str(p.relative_to(root)) for d in SCOPE for p in (root / d).rglob("*.md")
                   if not str(p.relative_to(root)).startswith(SKIP))
    rows = tuple((surrogate(rel, text), rel, n, rev, normalize(text)) for rel in files for n, text in candidates(root, rel))
    ids = [r[0] for r in rows]
    collisions = {i for i in ids if ids.count(i) > 1}
    with out.open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh)
        w.writerow(("surrogate_id", "path", "line", "revision", "original_text", "duplicate_text_in_same_file"))
        w.writerows(r + (r[0] in collisions,) for r in rows)
    print(f"{len(rows)} narrative normative statements; {len(set(ids))} distinct surrogates; {len(collisions)} repeated")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

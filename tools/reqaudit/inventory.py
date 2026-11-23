#!/usr/bin/env python3
"""Requirements inventory extractor for the Unbound requirements corpus.

Pure, functional pipeline: files -> lines -> definition/reference records -> CSV/JSON.
No record is mutated after creation; every derived view is computed from the
immutable list of extracted records.

Usage:
    python3 -I tools/reqaudit/inventory.py <repo-root> <output-dir>

Outputs (in <output-dir>):
    inventory.csv           one row per definition site (id may repeat)
    id-index.csv            one row per distinct identifier with definition/reference counts
    duplicate-ids.csv       identifiers defined more than once with differing text
    undefined-references.csv identifiers referenced but never defined in the corpus
    summary.json            counts by family and kind
"""
from __future__ import annotations

import csv
import hashlib
import io
import json
import re
import subprocess
import sys
from collections import Counter
from dataclasses import dataclass, asdict
from functools import reduce
from itertools import groupby
from pathlib import Path
from typing import Iterable, Iterator, Optional, Tuple

ID = r"[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}[A-Za-z]?(?:-Q)?"
ID_RE = re.compile(rf"\b({ID})\b")
BULLET_RE = re.compile(rf"^\s*[-*]\s+(?:\*\*({ID})\b[^*]*\*\*|({ID})\s*:)\s*:?\s*(.*)$")
LINE_RE = re.compile(rf"^({ID}):\s+(\S.*)$")
HEADING_RE = re.compile(rf"^(#{{1,6}})\s+({ID})\s*[:.—-]?\s*(.*)$")
TABLE_RE = re.compile(rf"^\|\s*`?({ID})`?\s*\|(.*)$")
CATALOG_RE = re.compile(rf"^({ID})\s+([A-Z][A-Za-z0-9]+.*)$")

# Corpus scope: files that hold requirement-bearing content in this repository.
SCOPE_GLOBS = (
    "requirements/**/*.md",
    "requirements/**/*.csv",
    "architecture/**/*.md",
    "research/daf-theoretical-enterprise-validation/**/*.md",
    "research/daf-theoretical-enterprise-validation/**/*.csv",
)
# CSVs whose first column is a reference (crosswalk), not a definition.
REFERENCE_ONLY_CSV = (
    "REQUIREMENT-TO-CONTROL-CROSSWALK-WAVE-01.csv",
    "DETAILED-REQUIREMENT-CONTROL-LINKS-WAVE-02.csv",
    "ENTERPRISE-CAPABILITY-TO-CONTROL-CROSSWALK.csv",
    "WORKFLOW-SIMULATION-RECORD-TEMPLATE.csv",
)
# Files produced by this reconciliation are excluded from the baseline inventory.
EXCLUDE_PREFIXES = ("requirements/reconciliation/",)

# Identifier family -> record kind. Families not listed are classified "other".
KIND_BY_FAMILY = {
    **{f: "requirement" for f in (
        "UNB-ENT", "UNB-LEG", "UNB-DON", "UNB-GRT", "UNB-AST", "UNB-FIN", "UNB-ADV", "UNB-AIO",
        "UNB-OPS", "UNB-TEC", "UNB-EXP", "UNB-VAL", "UNB-ACC", "UNB-NAV", "UNB-INV", "UNB-GRANT",
        "UNB-GEX", "UNB-GFUND", "UNB-CHAR", "UNB-EXT", "UNB-RULE", "UNB-CUST", "UNB-TEN", "UNB-SUCC",
        "UNB-PLAT", "UNB-KNOW", "UNB-WF", "UNB-DATA", "UNB-LS", "UNB-REM", "UNB-IMM", "UNB-XSC",
        "UNB-PAY", "UNB-PART", "UNB-API", "UNB-UX", "UNB-GOV", "UNB-ECO", "UNB-EVT", "UNB-TRACE",
        "UNB-DOC", "UNB-COM", "UNB-AGT", "UNB-CAP", "UNB-RES", "UNB-REGMAP", "UNB-API-STD",
        "UNB-AI-STD", "UNB-TEC-STRATA", "UNB-GAP", "UNB-CTL", "UNB-HYP",
        "REQ-X", "ADM", "AIN", "MON", "OPS", "INT", "TVR-GR", "TVR-ORG", "TVR-FIN", "TVR-PAY",
        "TVR-SEC", "TVR-UX", "TVR-WID", "TVR-DATA", "TVR-CMP", "TVR-OPS", "TVR-AI", "TVR-CHR", "UX", "ACC-MDL", "EXC", "AUTH",
        "TVR-CON", "TVR-GOV", "TVR-INV", "TVR-ACC", "GOV-AUTH", "SOC2", "UX-S", "WG",
    )},
    **{f: "control" for f in ("UNB-CTRL", "UNB-SCTRL", "CTL", "TVCTL", "VAL")},
    **{f: "test" for f in ("ADM-T", "CCM-T", "LIFE-T", "UI-T", "GOV-T")},
    **{f: "scenario" for f in ("SIM", "TVS", "SC", "SCN", "XSIM", "ADV", "UNB-XSC")},
    **{f: "event" for f in ("GR-EVT",)},
    **{f: "finding" for f in ("TVF", "GAP")},
    **{f: "decision" for f in ("OD", "ACC-ADR", "NAV-ADR", "GRANT-ADR", "UNB-CHAR-DEC", "UNB-DATA-DEC")},
    **{f: "contradiction" for f in ("TVC",)},
    **{f: "assumption" for f in ("TVA",)},
    **{f: "regulatory-assertion" for f in ("REG",)},
    **{f: "failure-mode" for f in ("FM", "FMEA")},
    **{f: "invariant" for f in ("FI", "INV", "CONC")},
    **{f: "workflow" for f in ("WF", "CW")},
}


@dataclass(frozen=True)
class Record:
    identifier: str
    family: str
    kind: str
    site: str          # bullet | heading | table | catalog | csv
    path: str
    line: int
    blob: str          # git blob sha of the file at baseline revision
    heading: str       # nearest preceding markdown heading
    text: str          # exact original text (single line, or heading + first paragraph)
    text_sha: str      # sha256 of normalized text, for duplicate detection


def family_of(identifier: str) -> str:
    base = re.sub(r"-Q$", "", identifier)
    return re.sub(r"-[A-Z]?\d{1,4}[A-Za-z]?$", "", base)


def kind_of(identifier: str) -> str:
    fam = family_of(identifier)
    if re.search(r"-T\d+$", identifier):
        return "test"
    if fam.endswith("ADR") or fam.startswith("ADR") or fam.endswith("-DEC"):
        return "decision"
    if fam.startswith("CTRL"):
        return "control"
    return KIND_BY_FAMILY.get(fam, "other")


def normalize(text: str) -> str:
    return re.sub(r"[^a-z0-9]+", " ", text.lower()).strip()


def sha(text: str) -> str:
    return hashlib.sha256(normalize(text).encode()).hexdigest()[:16]


def git_blob(root: Path, rel: str) -> str:
    result = subprocess.run(["git", "-C", str(root), "rev-parse", f"HEAD:{rel}"],
                            capture_output=True, text=True)
    return result.stdout.strip() if result.returncode == 0 else "untracked"


def corpus_files(root: Path) -> Tuple[str, ...]:
    found = {str(p.relative_to(root)) for g in SCOPE_GLOBS for p in root.glob(g) if p.is_file()}
    return tuple(sorted(f for f in found if not f.startswith(EXCLUDE_PREFIXES)))


def headings_by_line(lines: Tuple[str, ...]) -> Tuple[str, ...]:
    """Nearest preceding heading for each line (pure fold)."""
    def step(acc: Tuple[Tuple[str, ...], str], line: str):
        out, current = acc
        nxt = line.lstrip("#").strip() if line.startswith("#") else current
        return (out + (current,), nxt)
    return reduce(step, lines, ((), ""))[0]


def paragraph_after(lines: Tuple[str, ...], index: int, limit: int = 12) -> str:
    """Heading definitions take the heading plus the following block up to the next heading."""
    tail = lines[index + 1:index + 1 + limit]
    block = tuple(l for l in _takewhile_not_heading(tail) if l.strip())
    return " ".join(l.strip() for l in block)


def _takewhile_not_heading(lines: Iterable[str]) -> Iterator[str]:
    for l in lines:
        if l.startswith("#"):
            return
        yield l


def is_reference_list(text: str) -> bool:
    """A bullet whose body is essentially a list of identifiers is a cross-reference, not a definition."""
    body = ID_RE.sub("", text.split(":", 1)[-1] if ":" in text else text)
    return len(re.findall(r"[A-Za-z]{3,}", body)) < 3


def tokens(text: str) -> frozenset:
    return frozenset(w for w in normalize(ID_RE.sub(" ", text)).split() if len(w) > 3)


def similarity(a: str, b: str) -> float:
    ta, tb = tokens(a), tokens(b)
    # containment coefficient: tolerant of a short summary restating a longer definition
    return len(ta & tb) / min(len(ta), len(tb)) if ta and tb else 1.0


def collision_class(recs: Tuple[Record, ...]) -> str:
    """Classify a multiply-defined identifier.

    same-file-redefinition: one file defines the id twice with different content (true collision)
    cross-file-divergent-needs-review: different files carry low-overlap text (possible namespace
                           collision, or a terse summary); requires human confirmation
    mirror-or-summary:     same requirement restated elsewhere (md/csv mirror, index summary)
    """
    if len({r.text_sha for r in recs}) < 2:
        return "identical"
    pairs = tuple((a, b) for i, a in enumerate(recs) for b in recs[i + 1:])
    if any(a.path == b.path and a.site == b.site == "heading" and a.text_sha != b.text_sha for a, b in pairs):
        return "same-file-redefinition"
    divergent = tuple((a, b) for a, b in pairs if similarity(a.text, b.text) < 0.34)
    if not divergent:
        return "mirror-or-summary"
    same_file = any(a.path == b.path for a, b in divergent)
    return "same-file-redefinition" if same_file else "cross-file-divergent-needs-review"


def md_records(root: Path, rel: str) -> Tuple[Record, ...]:
    lines = tuple((root / rel).read_text(encoding="utf-8", errors="replace").splitlines())
    heads = headings_by_line(lines)
    blob = git_blob(root, rel)

    def classify(i: int, line: str) -> Optional[Record]:
        b, h, t, c = BULLET_RE.match(line), HEADING_RE.match(line), TABLE_RE.match(line), CATALOG_RE.match(line)
        if b and not is_reference_list(line) and "/13-adversarial-reviews/raw/" not in rel:
            ident, site, text = b.group(1) or b.group(2), "bullet", line.strip()
        elif h:
            ident, site = h.group(2), "heading"
            text = f"{line.strip()} {paragraph_after(lines, i)}".strip()
        elif t and not re.match(r"^\|\s*-", line):
            ident, site, text = t.group(1), "table", line.strip()
        elif LINE_RE.match(line):
            ident, site, text = LINE_RE.match(line).group(1), "line", line.strip()
        elif c:
            ident, site, text = c.group(1), "catalog", line.strip()
        else:
            return None
        return Record(ident, family_of(ident), kind_of(ident), site, rel, i + 1, blob,
                      heads[i], text, sha(text))

    return tuple(r for r in (classify(i, l) for i, l in enumerate(lines)) if r is not None)


def csv_records(root: Path, rel: str) -> Tuple[Record, ...]:
    if Path(rel).name in REFERENCE_ONLY_CSV:
        return ()
    raw = (root / rel).read_text(encoding="utf-8", errors="replace")
    rows = tuple(csv.reader(io.StringIO(raw)))
    blob = git_blob(root, rel)
    if not rows:
        return ()
    header = ",".join(rows[0])

    def to_record(i: int, row: Tuple[str, ...]) -> Optional[Record]:
        first = row[0].strip().strip('"') if row else ""
        if not re.fullmatch(ID, first):
            return None
        text = " | ".join(cell.strip() for cell in row)
        return Record(first, family_of(first), kind_of(first), "csv", rel, i + 1, blob,
                      header[:120], text, sha(" ".join(row[1:3])))

    return tuple(r for r in (to_record(i, tuple(row)) for i, row in enumerate(rows) if i > 0) if r)


def references(root: Path, files: Tuple[str, ...]) -> Counter:
    def ids_in(rel: str) -> Iterator[str]:
        return (m.group(1) for m in ID_RE.finditer((root / rel).read_text(encoding="utf-8", errors="replace")))
    return Counter(i for rel in files for i in ids_in(rel))


def extract(root: Path) -> Tuple[Tuple[Record, ...], Counter]:
    files = corpus_files(root)
    recs = tuple(r for rel in files
                 for r in (csv_records(root, rel) if rel.endswith(".csv") else md_records(root, rel)))
    return recs, references(root, files)


def write_csv(path: Path, header: Tuple[str, ...], rows: Iterable[Tuple]) -> None:
    with path.open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh)
        w.writerow(header)
        w.writerows(rows)


def by_id(records: Tuple[Record, ...]):
    keyed = sorted(records, key=lambda r: r.identifier)
    return {k: tuple(g) for k, g in groupby(keyed, key=lambda r: r.identifier)}


def main(argv: Tuple[str, ...]) -> int:
    root, out = Path(argv[1]).resolve(), Path(argv[2]).resolve()
    out.mkdir(parents=True, exist_ok=True)
    records, refs = extract(root)
    grouped = by_id(records)
    fields = tuple(Record.__dataclass_fields__)

    write_csv(out / "inventory.csv", fields, (tuple(asdict(r).values()) for r in records))
    write_csv(out / "id-index.csv",
              ("identifier", "family", "kind", "definition_sites", "distinct_texts", "reference_count", "first_path", "first_line"),
              ((k, v[0].family, v[0].kind, len(v), len({r.text_sha for r in v}), refs.get(k, 0), v[0].path, v[0].line)
               for k, v in grouped.items()))
    classes = {k: collision_class(v) for k, v in grouped.items()}
    write_csv(out / "duplicate-ids.csv",
              ("identifier", "kind", "collision_class", "path", "line", "site", "text"),
              ((r.identifier, r.kind, classes[k], r.path, r.line, r.site, r.text[:300])
               for k, v in grouped.items() if classes[k] not in ("identical",) and len(v) > 1 for r in v))
    defined = set(grouped)
    undefined = sorted((i, n) for i, n in refs.items() if i not in defined)
    write_csv(out / "undefined-references.csv", ("identifier", "family", "kind", "reference_count"),
              ((i, family_of(i), kind_of(i), n) for i, n in undefined))

    kinds = Counter(v[0].kind for v in grouped.values())
    fams = Counter(v[0].family for v in grouped.values())
    summary = {
        "baseline_commit": subprocess.run(["git", "-C", str(root), "rev-parse", "HEAD"],
                                          capture_output=True, text=True).stdout.strip(),
        "files_scanned": len(corpus_files(root)),
        "definition_sites": len(records),
        "distinct_identifiers": len(grouped),
        "distinct_identifiers_by_kind": dict(sorted(kinds.items())),
        "distinct_identifiers_by_family": dict(sorted(fams.items())),
        "multiply_defined_by_class": dict(Counter(c for k, c in classes.items() if len(grouped[k]) > 1)),
        "true_collisions": sorted(k for k, c in classes.items() if c in ("same-file-redefinition", "cross-file-divergent-needs-review")),
        "undefined_referenced_identifiers": len(undefined),
    }
    (out / "summary.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({k: summary[k] for k in summary if k != "distinct_identifiers_by_family"}, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main(tuple(sys.argv)))

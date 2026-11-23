#!/usr/bin/env python3
"""Adopt reconciliation records into the domain requirement files (owner instruction 2026-11-23).

Rules:
  * Original requirement text is never changed. A one-line annotation is inserted directly after
    the original definition pointing to the adopted record.
  * Each domain file receives (or has regenerated) one section between markers
        <!-- BEGIN ADOPTED: reconciliation-2026-11-23 --> ... <!-- END ADOPTED: reconciliation-2026-11-23 -->
    containing the adopted refined statements and gap-analysis requirements, with acceptance criteria.
  * Adoption status is derived from the decision register: records linked only to `decided` decisions
    (or to none) are `adopted`; records linked to a `decided-provisional` decision are
    `adopted-provisional` and name the validation still required.
  * Idempotent: re-running produces identical files.

Usage: python3 -I tools/reqaudit/adopt.py <repo-root>
Writes requirements/reconciliation/2026-11-23/data/adoption.csv.
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import defaultdict
from pathlib import Path
from typing import Dict, Iterable, List, Tuple

RECON = Path("requirements/reconciliation/2026-11-23")
BEGIN, END = "<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->", "<!-- END ADOPTED: reconciliation-2026-11-23 -->"
NOTE = "Adopted 2026-11-23"
DOMAIN_FILE = {
    "GRT": "requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md",
    "FIN": "requirements/FULL-ACCOUNTING-SYSTEM.md",
    "PLT": "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md",
    "ARC": "requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md",
    "ENT": "requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md",
    "REG": "requirements/charities/JURISDICTION-AWARE-REGULATORY-RULES-AND-ELIGIBILITY.md",
}
ENT_GOVERNANCE = ("UNB-RCN-ENT-001", "UNB-RCN-ENT-002", "UNB-RCN-ENT-003")
ID = re.compile(r"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}(?:-r\d+)?\b")


def jsonl(p: Path) -> Tuple[dict, ...]:
    return tuple(json.loads(l) for l in p.read_text(encoding="utf-8").splitlines() if l.strip())


def rows(p: Path) -> Tuple[dict, ...]:
    return tuple(csv.DictReader(p.open(encoding="utf-8")))


def norm(t: str) -> str:
    return re.sub(r"\s+", " ", re.sub(r"[*_`]|^\s*-\s*", "", t or "")).strip().lower()


def primary_domain(tag: str) -> str:
    return re.split(r"[+/]", tag or "PLT")[0]


def linked_decisions(record_ids: Iterable[str], decisions: Tuple[dict, ...]) -> Tuple[dict, ...]:
    ids = set(record_ids)
    return tuple(d for d in decisions if ids & set(ID.findall(d["affected_ids"])))


def status_of(record_ids: Iterable[str], decisions: Tuple[dict, ...]) -> Tuple[str, str, str]:
    linked = linked_decisions(record_ids, decisions)
    prov = tuple(d for d in linked if d["status"] == "decided-provisional")
    refs = ", ".join(d["decision_id"] for d in linked) or "—"
    if prov:
        return "adopted-provisional", refs, "; ".join(f'{d["decision_id"]}: {d["validation_required"]}' for d in prov)
    return "adopted", refs, ""


def target_for_gap(g: dict, inventory_paths: Dict[str, str]) -> str:
    if g["id"] in ENT_GOVERNANCE:
        return "requirements/BASELINE-001.md"
    for l in [*g.get("links", ()), *ID.findall(g.get("parent", ""))]:
        p = inventory_paths.get(l, "")
        if p.startswith("requirements/") and p.endswith(".md") and "reconciliation" not in p:
            return p
    return DOMAIN_FILE[g["id"].split("-")[2]]


def target_for_refinement(r: dict) -> str:
    p = r["original_path"]
    return p if p.startswith("requirements/") else DOMAIN_FILE[primary_domain(r.get("_domain", "PLT"))]


def ac_lines(acs) -> List[str]:
    return [f"  - {a['id']}: Given {a['given']}; when {a['when']}; then {a['then']}." for a in acs]


def render_refinement(r: dict, status: Tuple[str, str, str]) -> List[str]:
    st, decs, val = status
    head = f"### {r['refinement_id']} ({r['relationship']} `{r['original_id']}`) — {st}"
    meta = f"- **Original:** `{r['original_path']}:{r['original_line']}` @ `777a70d` (kept verbatim) · **Decisions:** {decs}"
    lines = [head, meta, f"- **Requirement:** {r['statement']}"]
    lines += [f"- **Validation still required:** {val}"] if val else []
    lines += ["- **Acceptance criteria:**", *ac_lines(r["acceptance_criteria"]),
              f"- Full record: `{RECON}/data/refinements.jsonl` ({r['refinement_id']}); report `{RECON}/07-REFINEMENT-RECORDS.md`.", ""]
    return lines


def render_gap(g: dict, status: Tuple[str, str, str]) -> List[str]:
    st, decs, val = status
    lines = [f"### {g['id']} ({g['priority']}, origin: gap-analysis) — {st}",
             f"- **Exposed by:** {g['gap_finding']} · **Parent:** {g['parent']} · **Decisions:** {decs}",
             f"- **Requirement:** {g['statement']}"]
    lines += [f"- **Validation still required:** {val}"] if val else []
    lines += ["- **Acceptance criteria:**", *ac_lines(g["acceptance_criteria"]),
              f"- Full record: `{RECON}/data/gap-requirements.jsonl` ({g['id']}); report `{RECON}/04-MISSING-REQUIREMENTS-REGISTER.md`.", ""]
    return lines


def annotate_original(text: str, r: dict, target: str, st: str) -> str:
    """Insert a single annotation line after the original definition (idempotent)."""
    marker = f"{NOTE}: {'superseded' if r['relationship'] == 'supersedes' else 'refined'} by `{r['refinement_id']}`"
    if marker in text:
        return text
    lines = text.split("\n")
    key = norm(r["original_text"])[:80]
    hits = [i for i, l in enumerate(lines) if key and key[:60] in norm(l)] or \
           [i for i, l in enumerate(lines) if r["original_id"] in l and (l.startswith(("#", "-", "**")))]
    if not hits:
        raise SystemExit(f"original not found for {r['refinement_id']} in {r['original_path']}")
    i = hits[0]
    where = "below" if target == r["original_path"] else f"in `{target}`"
    note = f"> {marker} ({st}); the adopted requirement is {where} under 'Adopted reconciliation requirements'."
    return "\n".join(lines[: i + 1] + ([""] if lines[i].startswith("#") else []) + [note] + lines[i + 1:])


def section(blocks: List[List[str]]) -> str:
    body = [BEGIN, "", "## Adopted reconciliation requirements (2026-11-23)", "",
            "Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.", ""]
    return "\n".join(body + [l for b in blocks for l in b] + [END])


def with_section(text: str, sec: str) -> str:
    if BEGIN in text:
        return re.sub(re.escape(BEGIN) + r".*?" + re.escape(END), lambda _: sec, text, flags=re.S)
    return text.rstrip("\n") + "\n\n" + sec + "\n"


def main(argv) -> int:
    root = Path(argv[1])
    data = root / RECON / "data"
    refs, gaps, decs = jsonl(data / "refinements.jsonl"), jsonl(data / "gap-requirements.jsonl"), rows(data / "decisions.csv")
    inv = {}
    for r in rows(data / "inventory.csv"):
        inv.setdefault(r["identifier"], r["path"])
    blocks: Dict[str, List[List[str]]] = defaultdict(list)
    register = []
    for r in refs:
        st = status_of([r["refinement_id"], r["original_id"], *r.get("related_findings", ())], decs)
        tgt = target_for_refinement(r)
        blocks[tgt].append(render_refinement(r, st))
        register.append((r["refinement_id"], "refinement", r["original_id"], tgt, *st))
    for g in gaps:
        st = status_of([g["id"], g["gap_finding"], *g.get("links", ())], decs)
        tgt = target_for_gap(g, inv)
        blocks[tgt].append(render_gap(g, st))
        register.append((g["id"], "gap-requirement", "", tgt, *st))
    files = {}
    for r in refs:
        p = r["original_path"]
        files[p] = annotate_original(files.get(p) or (root / p).read_text(encoding="utf-8"), r,
                                     target_for_refinement(r), status_of([r["refinement_id"], r["original_id"], *r.get("related_findings", ())], decs)[0])
    for tgt, bl in blocks.items():
        files[tgt] = with_section(files.get(tgt) or (root / tgt).read_text(encoding="utf-8"), section(bl))
    for p, t in files.items():
        (root / p).write_text(t, encoding="utf-8")
    with (data / "adoption.csv").open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh, quoting=csv.QUOTE_ALL)
        w.writerow(("record_id", "kind", "original_id", "adopted_into", "status", "decisions", "validation_required"))
        w.writerows(register)
    c = defaultdict(int)
    for x in register:
        c[x[4]] += 1
    print(dict(c), "files touched:", len(files))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

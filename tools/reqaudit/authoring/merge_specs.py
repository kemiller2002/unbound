"""Merge the panel and view specs (scratchpad) into data/ui-requirements.jsonl; register their PROPOSED gaps. Idempotent."""
import json, csv, re
DATA = "/home/user/unbound/requirements/reconciliation/2026-11-23/data"
SP = "/tmp/claude-0/-home-user/179dd429-c49d-5559-a2b4-4866d79585b5/scratchpad"
GAPMAP = {"PROPOSED:Approval quorum tracker": "FORMA-GAP-24", "PROPOSED:Two-person approval capture": "FORMA-GAP-24",
          "PROPOSED:Accounting period context bar": "FORMA-GAP-25", "PROPOSED:Per-item batch outcome summary": "FORMA-GAP-26",
          "PROPOSED:Typed panel slot registry": "UNB-GAP-W-02"}
RENUM = {"UNB-UI-S-099": "UNB-UI-S-061"}
NEWGAPS = [
    ("FORMA-GAP-24", "Approval quorum tracker (two or more distinct approvers)", "Forma", "P1", "Restatement, tolerance override, disposition batches, cutover and two-person approvals need a tracker of required approvers, each with status and independence rule (not the preparer). readiness-checklist only approximates it.", "readiness-checklist with text per approver", "A component listing required approvals with approver, status, time and an explicit 'cannot be the preparer' rule, announced on change"),
    ("FORMA-GAP-25", "Accounting period context bar", "Forma", "P2", "Finance workspaces need a persistent header with period, period state (open, closing, certified), cutoff and snapshot as-of/known-at times; scope-trail and date-range do not carry period state.", "scope-trail plus freshness", "A context bar component with period state and two timestamps, copyable, with forced-colours support"),
    ("FORMA-GAP-26", "Per-item batch outcome summary", "Forma", "P2", "Batch approval commits each item in its own transaction, so results are mixed (approved, in review, waiting for funds, unknown); FORMA-GAP-04 assumed a single outcome.", "Table of items with status chips", "A summary that groups items by outcome with counts and links, and treats Unknown distinctly (G-006)"),
    ("UNB-GAP-W-02", "Typed panel slot registry for the case workspace", "Unbound", "", "The case frame (S-014) hosts 18 typed decision panels; a registry maps case type to panel, required facts and permitted actions.", "Static mapping in the prototype", "Registry data validated against WFM transitions; unknown case types render a safe read-only panel"),
]

def remap(v):
    if isinstance(v, str):
        return RENUM.get(v, v) if v in RENUM else re.sub(r"UNB-UI-S-099", "UNB-UI-S-061", v)
    if isinstance(v, list):
        return [GAPMAP.get(x, remap(x)) if isinstance(x, str) else remap(x) for x in v]
    if isinstance(v, dict):
        return {k: remap(x) for k, x in v.items()}
    return v

def load(p):
    return [json.loads(l) for l in open(p) if l.strip()]

specs = [remap(r) for f in ("ui-spec-panels.jsonl", "ui-spec-views.jsonl") for r in load(f"{SP}/{f}")]
specs = [dict(r, capability_gaps=list(dict.fromkeys(r["capability_gaps"]))) for r in specs]
path = f"{DATA}/ui-requirements.jsonl"
existing = load(path)
ids = {r["id"] for r in specs}
out = [r for r in existing if r["id"] not in ids] + specs
assert len({r["id"] for r in out}) == len(out)
assert not any(g.startswith("PROPOSED") for r in out for g in r["capability_gaps"])
open(path, "w").writelines(json.dumps(r, ensure_ascii=False) + "\n" for r in out)
gp = f"{DATA}/ui-component-gaps.csv"
rows = list(csv.DictReader(open(gp))); f = list(rows[0].keys()); have = {r["gap_id"] for r in rows}
rows += [dict(zip(f, list(g) + ["pending"])) for g in NEWGAPS if g[0] not in have]
w = csv.DictWriter(open(gp, "w", newline=""), fieldnames=f, quoting=csv.QUOTE_ALL); w.writeheader(); w.writerows(rows)
print(len(out), "ui records;", len(specs), "merged;", len(rows), "gaps")

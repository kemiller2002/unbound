#!/usr/bin/env python3
"""Validate workflow state-machine models and derive the UI surface inventory.

Each workflow is one JSON file in requirements/reconciliation/2026-11-23/workflows/ following the schema
in requirements/reconciliation/2026-11-23/workflows/SCHEMA.md. All checks are pure functions over the
parsed models; the program prints issues, writes a JSON report and a surface inventory, and exits
non-zero on any ERROR.

Checks
  W1  identifiers unique; every referenced state exists
  W2  exactly one initial state; every state reachable from it
  W3  every non-terminal state has an outgoing transition and a path to a terminal state (no traps)
  W4  terminal states have no outgoing transitions except to correction states
  W5  every transition names actor_type, authority (A0-A5) and at least one resolvable requirement id
  W6  authority is consistent with actor: ai <= A2, deterministic <= A3, A4 only human, A5 never a transition
  W7  money-moving transitions carry their mandatory guards (see GUARDS)
  W8  transitions that call an external party declare an Unknown/timeout outcome
  W9  every state has a user_label and lozenge; one state id has one label across all workflows
  W10 every human-actor transition names a UI surface (existing UNB-UI-* or NEW:<name>)
  W11 transitions reference scenarios that exist (WARN when none)

Usage: python3 -I tools/reqaudit/workflows.py <repo-root>
"""
from __future__ import annotations

import csv
import json
import re
import sys
from collections import defaultdict, deque
from dataclasses import dataclass
from pathlib import Path
from typing import Dict, FrozenSet, Iterable, Tuple

RECON = Path("requirements/reconciliation/2026-11-23")
AUTH = ("A0", "A1", "A2", "A3", "A4", "A5")
LOZENGE = frozenset({"ok", "attention", "unknown", "blocked", "neutral"})
ACTORS = frozenset({"human", "deterministic", "ai", "external", "timer"})
CEILING = {"ai": "A2", "deterministic": "A3", "timer": "A3", "external": "A3", "human": "A4"}
# effect tag -> guards that must be present on the transition
GUARDS = {
    "external-money-out": {"eligibility-current", "sanctions-clear", "hold-watermark-current", "destination-verified", "funded"},
    "reserve-capacity": {"capacity-in-transaction"},
    "grant-approval": {"eligibility-current", "sanctions-clear", "segregation-of-duties"},
    "credit-grantable": {"contribution-final"},
    "issue-acknowledgment": {"completed-gift-determined", "sponsor-accepted"},
}


@dataclass(frozen=True)
class Issue:
    level: str
    check: str
    subject: str
    message: str


def load(dirp: Path) -> Tuple[dict, ...]:
    return tuple(json.loads(p.read_text(encoding="utf-8")) for p in sorted(dirp.glob("WFM-*.json")))


def states_of(w: dict) -> Dict[str, dict]:
    return {s["id"]: s for s in w["states"]}


def graph(w: dict) -> Dict[str, Tuple[str, ...]]:
    g = defaultdict(list)
    for t in w["transitions"]:
        for f in t["from"] if isinstance(t["from"], list) else [t["from"]]:
            g[f].append(t["to"])
            g[f].extend(x["to_state"] for x in t.get("failure", ()) if x.get("to_state"))
    return {k: tuple(v) for k, v in g.items()}


def reach(g: Dict[str, Tuple[str, ...]], start: str) -> FrozenSet[str]:
    seen, q = {start}, deque([start])
    while q:
        for n in g.get(q.popleft(), ()):
            if n not in seen:
                seen.add(n)
                q.append(n)
    return frozenset(seen)


def check_structure(w: dict) -> Iterable[Issue]:
    wid, S = w["workflow_id"], states_of(w)
    tids = [t["id"] for t in w["transitions"]]
    yield from (Issue("ERROR", "W1", wid, f"duplicate transition id {t}") for t in set(tids) if tids.count(t) > 1)
    froms = lambda t: t["from"] if isinstance(t["from"], list) else [t["from"]]
    for t in w["transitions"]:
        for s in [*froms(t), t["to"], *(x.get("to_state") for x in t.get("failure", ()) if x.get("to_state"))]:
            if s not in S:
                yield Issue("ERROR", "W1", t["id"], f"unknown state '{s}'")
    inits = [s for s in S.values() if s["kind"] == "initial"]
    if len(inits) != 1:
        yield Issue("ERROR", "W2", wid, f"expected exactly one initial state, found {len(inits)}")
        return
    g = graph(w)
    r = reach(g, inits[0]["id"])
    yield from (Issue("ERROR", "W2", f"{wid}:{s}", "state unreachable from initial") for s in S if s not in r)
    terminals = frozenset(s for s, v in S.items() if v["kind"] in ("terminal", "correction"))
    for s, v in S.items():
        if v["kind"] in ("terminal", "correction"):
            bad = [n for n in g.get(s, ()) if S.get(n, {}).get("kind") != "correction"]
            if bad:
                yield Issue("ERROR", "W4", f"{wid}:{s}", f"terminal state transitions to non-correction state(s) {bad}")
            continue
        if not g.get(s):
            yield Issue("ERROR", "W3", f"{wid}:{s}", "non-terminal state has no outgoing transition (dead end)")
        elif not (reach(g, s) & terminals):
            yield Issue("ERROR", "W3", f"{wid}:{s}", "no path to any terminal state (trap)")


def check_transitions(w: dict, resolves) -> Iterable[Issue]:
    for t in w["transitions"]:
        a, auth = t.get("actor_type"), t.get("authority")
        if a not in ACTORS:
            yield Issue("ERROR", "W5", t["id"], f"actor_type '{a}' invalid")
        if auth not in AUTH:
            yield Issue("ERROR", "W5", t["id"], f"authority '{auth}' invalid")
        elif auth == "A5":
            yield Issue("ERROR", "W6", t["id"], "A5 (prohibited) cannot be a transition")
        elif a in CEILING and AUTH.index(auth) > AUTH.index(CEILING[a]):
            yield Issue("ERROR", "W6", t["id"], f"{a} actor exceeds its ceiling {CEILING[a]} with {auth}")
        reqs = t.get("requirement_ids", [])
        if not reqs:
            yield Issue("ERROR", "W5", t["id"], "no requirement_ids")
        yield from (Issue("ERROR", "W5", t["id"], f"requirement '{r}' does not resolve") for r in reqs if not resolves(r))
        effects, guards = set(t.get("effects", [])), set(t.get("guards", []))
        for e in effects & set(GUARDS):
            missing = GUARDS[e] - guards
            if missing:
                yield Issue("ERROR", "W7", t["id"], f"effect '{e}' missing guards {sorted(missing)}")
        if (a == "external" or "external-call" in effects) and not any(x.get("condition", "").lower().startswith(("unknown", "timeout")) for x in t.get("failure", ())):
            yield Issue("ERROR", "W8", t["id"], "external interaction without an Unknown/timeout outcome")
        if a == "human" and not t.get("ui_surface"):
            yield Issue("ERROR", "W10", t["id"], "human transition without ui_surface")


def check_labels(ws: Tuple[dict, ...]) -> Iterable[Issue]:
    labels = defaultdict(set)
    for w in ws:
        for s in w["states"]:
            if not s.get("user_label") or s.get("lozenge") not in LOZENGE:
                yield Issue("ERROR", "W9", f'{w["workflow_id"]}:{s["id"]}', "missing user_label or invalid lozenge")
            labels[s["id"]].add((s.get("user_label"), s.get("lozenge")))
    yield from (Issue("ERROR", "W9", sid, f"same state id with different labels {sorted(v)}") for sid, v in labels.items() if len(v) > 1)


def check_scenarios(ws, scenario_ids: FrozenSet[str]) -> Iterable[Issue]:
    for w in ws:
        for t in w["transitions"]:
            sc = t.get("scenario_ids", [])
            yield from (Issue("ERROR", "W11", t["id"], f"scenario '{s}' not found") for s in sc if s not in scenario_ids)
            if not sc and (t.get("guards") or t.get("failure")):
                yield Issue("WARN", "W11", t["id"], "guarded or failure-bearing transition has no scenario")


def surface_inventory(ws) -> Dict[str, dict]:
    inv: Dict[str, dict] = {}
    for w in ws:
        for t in w["transitions"]:
            s = t.get("ui_surface")
            if not s:
                continue
            e = inv.setdefault(s, {"surface": s, "workflows": set(), "transitions": [], "roles": set(), "max_authority": "A0", "release": set()})
            e["workflows"].add(w["workflow_id"])
            e["transitions"].append(t["id"])
            e["roles"].add(t.get("actor_role", ""))
            e["release"].add(w.get("release", "R1"))
            if AUTH.index(t["authority"]) > AUTH.index(e["max_authority"]):
                e["max_authority"] = t["authority"]
        for s in w.get("observer_surfaces", ()):
            e = inv.setdefault(s, {"surface": s, "workflows": set(), "transitions": [], "roles": set(), "max_authority": "A0", "release": set()})
            e["workflows"].add(w["workflow_id"])
            e["release"].add(w.get("release", "R1"))
    return inv


def main(argv) -> int:
    root = Path(argv[1]).resolve()
    data = root / RECON / "data"
    ws = load(root / RECON / "workflows")
    sys.path.insert(0, str(Path(__file__).parent))
    from validate import load_csv, load_jsonl, resolver  # reuse the reconciliation resolver
    inv = frozenset(r["identifier"] for r in load_csv(data / "inventory.csv"))
    new = frozenset([*(r["refinement_id"] for r in load_jsonl(data / "refinements.jsonl")), *(g["id"] for g in load_jsonl(data / "gap-requirements.jsonl")),
                     *(u["id"] for u in load_jsonl(data / "ui-requirements.jsonl")), *(d["decision_id"] for d in load_csv(data / "decisions.csv"))])
    extra = frozenset(r["new_id"] for r in load_csv(data / "renamed-ids.csv"))
    ok = resolver(inv, new | extra, frozenset())
    scen = frozenset(s["scenario_id"] for s in load_csv(data / "scenarios.csv")) | frozenset(r["identifier"] for r in load_csv(data / "inventory.csv") if r["kind"] == "scenario")
    issues = tuple(i for w in ws for i in (*check_structure(w), *check_transitions(w, ok))) + tuple(check_labels(ws)) + tuple(check_scenarios(ws, scen))
    errors = [i for i in issues if i.level == "ERROR"]
    for i in issues:
        print(f"{i.level:5} {i.check:4} {i.subject}: {i.message}")
    surfaces = surface_inventory(ws)
    with (data / "surface-inventory.csv").open("w", newline="", encoding="utf-8") as fh:
        w = csv.writer(fh, quoting=csv.QUOTE_ALL)
        w.writerow(("surface", "kind", "release", "workflows", "transition_count", "roles", "max_authority"))
        for s in sorted(surfaces):
            e = surfaces[s]
            w.writerow((s, "existing" if s.startswith("UNB-UI-") else "new", ";".join(sorted(e["release"])), ";".join(sorted(e["workflows"])),
                        len(e["transitions"]), ";".join(sorted(r for r in e["roles"] if r)), e["max_authority"]))
    summary = {"workflows": len(ws), "states": sum(len(w["states"]) for w in ws), "transitions": sum(len(w["transitions"]) for w in ws),
               "errors": len(errors), "warnings": len(issues) - len(errors),
               "errors_by_check": dict(sorted({c: sum(1 for i in errors if i.check == c) for c in {i.check for i in errors}}.items())),
               "surfaces": len(surfaces), "new_surfaces": sum(1 for s in surfaces if not s.startswith("UNB-UI-"))}
    (data / "workflow-validation.json").write_text(json.dumps(summary, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(summary, indent=2))
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

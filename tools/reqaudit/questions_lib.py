"""Source-linked, event-sourced question resolutions (pure functions; no I/O except the load_* helpers).

Every question in the requirements missions has a stable ID:
  * decisions: the decision_id from a mission's data/decisions.csv
  * requirement questions: '<record id>#Q<n>', n = 1-based position in the record's open_questions

Resolutions are never stored in the generated register. They live in an append-only event log,
requirements/questions/resolution-events.jsonl, one JSON object per line:

  {"event_id": "QE-000001", "question_id": "...", "on": "YYYY-MM-DD", "type": "<event type>",
   "artifact": "<repo path that records the reasoning>", "actor": "<who>", "set": {<field>: <value>}}

The current resolution of a question is the left fold of its events in log order: each event's
"set" overwrites the named fields. A "registered" event pins the exact source question text and its
SHA-256 so later edits to a source record are detected instead of silently re-keying answers.
"""
import csv
import hashlib
import io
import json
import re
from collections import Counter
from functools import reduce
from pathlib import Path

RECON = Path("requirements/reconciliation/2026-11-23")
DISC = Path("requirements/discovery/2026-11-23-cross-industry")
SIMP = Path("requirements/discovery/2026-11-24-simplification-economics")
MISSIONS = ((RECON, "Reconciliation", "14-OPEN-DECISIONS.md"),
            (DISC, "Cross-industry discovery", "15-OPEN-DECISIONS-AND-SCENARIOS.md"),
            (SIMP, "Simplification and economics", "20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md"))
EVENTS = Path("requirements/questions/resolution-events.jsonl")
CLUSTERS = Path("requirements/questions/question-clusters.csv")

EVENT_TYPES = ("registered", "proposed", "dispositioned", "corrected", "evidence-reviewed", "resolved",
               "approved", "superseded", "reopened", "applied-to-requirement", "text-reconfirmed")
RESOLUTION_STATUSES = ("open", "proposed", "resolved", "superseded-by-duplicate", "withdrawn")
APPROVAL_STATUSES = ("not-approved", "agent-recommended", "approved-product-direction", "approved-owner", "rejected")
DISPOSITIONS = ("established-by-authoritative-evidence", "reasoned-interpretation", "business-policy-decision",
                "empirical-validation-required", "external-authorization-required", "unresolved")
REGISTER_KINDS = ("open decision", "awaiting confirmation", "requirement question")
STATE_FIELDS = ("register_kind", "source_record", "source_path", "question_text", "question_sha256",
                "canonical_id", "resolution_status", "approval_status", "approved_by", "approved_on",
                "disposition", "answer", "confidence", "evidence_refs", "activation_gate",
                "external_authorization_required", "acceptance_criteria", "reconsideration_triggers",
                "conservative_fallback", "remaining_uncertainty", "applied_to", "review_class")


def sha(text):
    return hashlib.sha256((text or "").strip().encode("utf-8")).hexdigest()


def load_csv(path):
    return tuple(csv.DictReader(io.StringIO(path.read_text(encoding="utf-8")))) if path.exists() else ()


def load_jsonl(path):
    return tuple(json.loads(l) for l in path.read_text(encoding="utf-8").splitlines() if l.strip()) if path.exists() else ()


# ---------------------------------------------------------------- source questions

def source_decisions(root):
    """Every decision in every mission (any status): {decision_id: {question, status, path, ...}}."""
    return {d["decision_id"]: dict(d, _path=str(base / "data/decisions.csv"), _mission=label, _report=str(base / report))
            for base, label, report in MISSIONS for d in load_csv(root / base / "data/decisions.csv")}


def source_requirement_questions(root):
    """Every open_questions entry of every record: {qid: {question, record, path, statement}}."""
    def from_record(base, name, r):
        rid = r.get("id") or r.get("refinement_id")
        qs = r.get("open_questions") or ()
        qs = (qs,) if isinstance(qs, str) else tuple(qs)
        return tuple((f"{rid}#Q{i}", {"question": q, "record": rid, "_path": str(base / "data" / name),
                                       "statement": r.get("statement") or "",
                                       "status": r.get("approval_status") or r.get("validation_status", "")})
                     for i, q in enumerate(qs, 1) if str(q).strip())
    return dict(q for base, _, _ in MISSIONS for name in ("gap-requirements.jsonl", "refinements.jsonl")
                for r in load_jsonl(root / base / "data" / name) for q in from_record(base, name, r))


def source_text(qid, decisions, reqqs):
    """The exact current source question text, or None when the source record no longer exists."""
    if "#Q" in qid:
        return reqqs[qid]["question"] if qid in reqqs else None
    return decisions[qid]["question"] if qid in decisions else None


# ---------------------------------------------------------------- event fold

def load_events(root):
    return load_jsonl(root / EVENTS)


def apply_event(state, event):
    merged = {**state, **event.get("set", {})}
    return {**merged, "history": state.get("history", ()) + (
        {k: event[k] for k in ("event_id", "on", "type", "artifact", "actor") if k in event},)}


def fold(events):
    """{question_id: state} from the ordered event log."""
    def step(acc, e):
        q = e["question_id"]
        return {**acc, q: apply_event(acc.get(q, {"question_id": q, "resolution_status": "open",
                                                   "approval_status": "not-approved"}), e)}
    return reduce(step, events, {})


def last_text_event_index(state):
    hist = state.get("history", ())
    return max((i for i, h in enumerate(hist) if h["type"] in ("registered", "text-reconfirmed")), default=-1)


def answer_event_index(state):
    hist = state.get("history", ())
    return max((i for i, h in enumerate(hist) if h["type"] in ("proposed", "corrected", "resolved", "evidence-reviewed")), default=-1)


# ---------------------------------------------------------------- validation

def validate(events, states, decisions, reqqs, clusters):
    """Return a tuple of (severity, code, question_id, message). 'error' rows must fail the build."""
    ids = Counter(e.get("event_id") for e in events)
    dup_events = tuple(("error", "duplicate-event-id", "", f"{i} appears {n} times") for i, n in ids.items() if n > 1)
    bad_types = tuple(("error", "invalid-event-type", e["question_id"], f"{e.get('event_id')}: {e.get('type')}")
                      for e in events if e.get("type") not in EVENT_TYPES)
    unregistered = tuple(("error", "unregistered-question", q, "events exist but no 'registered' event")
                         for q, s in states.items() if not any(h["type"] == "registered" for h in s.get("history", ())))

    def per_question(q, s):
        text = source_text(q, decisions, reqqs)
        status, approval, disp = s.get("resolution_status"), s.get("approval_status"), s.get("disposition", "")
        checks = (
            ("error", "missing-source-record", "source record no longer exists") if text is None else None,
            ("error", "changed-question-text", "source text differs from registered text; re-review and add a 'text-reconfirmed' event")
            if text is not None and sha(text) != s.get("question_sha256") else None,
            ("error", "invalid-status", f"resolution_status {status!r}") if status not in RESOLUTION_STATUSES else None,
            ("error", "invalid-approval", f"approval_status {approval!r}") if approval not in APPROVAL_STATUSES else None,
            ("error", "invalid-disposition", f"disposition {disp!r}") if disp and disp not in DISPOSITIONS else None,
            ("error", "invalid-closure", "resolved without an answer") if status == "resolved" and not s.get("answer") else None,
            ("error", "invalid-closure", "resolved with disposition 'unresolved'") if status == "resolved" and disp == "unresolved" else None,
            ("error", "invalid-closure", "external authorization disposition without naming the authorization")
            if status == "resolved" and disp == "external-authorization-required" and not s.get("external_authorization_required") else None,
            ("error", "invalid-closure", "approved but still open") if approval.startswith("approved") and status == "open" else None,
            ("error", "dangling-duplicate", f"canonical {s.get('canonical_id')!r} has no resolution")
            if status == "superseded-by-duplicate" and s.get("canonical_id") not in states else None,
            ("warning", "stale-resolution", "answer predates the latest text confirmation")
            if s.get("answer") and answer_event_index(s) < last_text_event_index(s) else None,
            ("warning", "no-answer", "no proposed or resolved answer yet") if not s.get("answer") and status != "superseded-by-duplicate" else None,
        )
        return tuple((c[0], c[1], q, c[2]) for c in checks if c)

    def conflict(member, canonical, relation):
        m, c = states.get(member, {}), states.get(canonical, {})
        both = m.get("resolution_status") == "resolved" and c.get("resolution_status") == "resolved"
        differ = m.get("disposition") and c.get("disposition") and m["disposition"] != c["disposition"]
        return (("warning", "conflicting-dispositions", member,
                 f"{relation} of {canonical} but dispositions differ: {m.get('disposition')} vs {c.get('disposition')}"),) \
            if relation == "same-question" and both and differ else ()

    return (dup_events + bad_types + unregistered
            + tuple(r for q, s in sorted(states.items()) for r in per_question(q, s))
            + tuple(r for c in clusters for r in conflict(c["member_id"], c["canonical_id"], c["relation"])))


def errors(findings):
    return tuple(f for f in findings if f[0] == "error")

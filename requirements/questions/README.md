# Question resolutions (source-linked, append-only)

This directory is the **only** place where answers to the questions in
[`../OPEN-QUESTIONS.md`](../OPEN-QUESTIONS.md) are recorded. The register and
[`../open-questions.csv`](../open-questions.csv) are generated outputs.

| File | Role |
|---|---|
| `resolution-events.jsonl` | Append-only event log. One JSON object per line. Never edit or delete a line; append a new event instead. |
| `question-clusters.csv` | Duplicate and related questions: `member_id`, `canonical_id`, `relation` (`same-question`, `sub-question`, `related`), `basis`. |

Tools: `tools/reqaudit/questions_lib.py` (fold and validation), `tools/reqaudit/render_questions.py`
(generator), `tools/reqaudit/seed_resolutions.py` (one-time seed from the 2026-11-24 review artifacts),
`tools/reqaudit/test_question_register.py` (regression tests).

## Stable identifiers

- Decisions: the `decision_id` in a mission's `data/decisions.csv`.
- Requirement questions: `<record id>#Q<n>`, where `n` is the 1-based position in the record's
  `open_questions`. Because the ID is positional, **never reorder or delete `open_questions` entries**;
  the `registered` event pins the exact text and its SHA-256, and the validator fails if the text at that
  position changes.

## Event schema

```json
{"event_id": "QE-000846", "question_id": "XID-REG-001", "on": "2026-11-24", "type": "evidence-reviewed",
 "artifact": "requirements/convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md", "actor": "...",
 "set": {"answer": "...", "disposition": "reasoned-interpretation", "evidence_refs": ["EV-D-003"]}}
```

`type` is one of `registered`, `proposed`, `dispositioned`, `corrected`, `evidence-reviewed`, `resolved`,
`approved`, `superseded`, `reopened`, `applied-to-requirement`, `text-reconfirmed`. The current state is
the left fold of `set` objects in log order; `history` is every event.

State fields with controlled values:

| Field | Values |
|---|---|
| `resolution_status` | `open`, `proposed`, `resolved`, `superseded-by-duplicate`, `withdrawn` |
| `approval_status` | `not-approved`, `agent-recommended`, `approved-product-direction`, `approved-owner`, `rejected` |
| `disposition` | `established-by-authoritative-evidence`, `reasoned-interpretation`, `business-policy-decision`, `empirical-validation-required`, `external-authorization-required`, `unresolved` |

Other fields: `answer`, `canonical_id`, `evidence_refs`, `activation_gate`, `external_authorization_required`,
`acceptance_criteria`, `reconsideration_triggers`, `conservative_fallback`, `remaining_uncertainty`,
`applied_to` (requirement IDs changed), `review_class`, `superseded_answer`.

`resolved` means a design-level answer exists. It does **not** mean an activation gate is satisfied:
`activation_gate` and `external_authorization_required` name what must exist (contract, board resolution,
bank approval, auditor report, empirical measurement) before a capability is switched on.

## Validation (fails the generator)

| Code | Meaning |
|---|---|
| `duplicate-event-id` | Two events share an ID |
| `invalid-event-type` | Unknown event type |
| `unregistered-question` / orphaned resolution | Events for a question that has no `registered` event or no longer appears in any source |
| `missing-source-record` | Registered question's source decision or record was deleted |
| `changed-question-text` | Source text differs from the registered text; review and append `text-reconfirmed` |
| `invalid-status` / `invalid-approval` / `invalid-disposition` | Value outside the controlled list |
| `invalid-closure` | Resolved without answer, resolved as `unresolved`, external-authorization disposition without naming the authorization, or approved while still open |
| `dangling-duplicate` | Superseded by a canonical question that has no resolution |

Warnings: `stale-resolution` (answer older than the latest text confirmation), `no-answer`,
`conflicting-dispositions` (same-question duplicates resolved with different dispositions).

The generator also refuses to write when the existing CSV contains an answer the log does not, so a
hand edit is never silently discarded (`--allow-discard` overrides deliberately).

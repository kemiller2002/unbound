# Resolutions: answers to the open-questions register

This directory holds the answers to the questions in [`../OPEN-QUESTIONS.md`](../OPEN-QUESTIONS.md). The register is generated; the answers here are the source of truth for it.

## How an answer finds its question

Every question has a **stable key**:

```
<record id>#<first 10 hex digits of sha256(kind | question text, whitespace-collapsed and lower-cased)>
```

For example `UNB-SMP-ORG-003#…` for a question on UNB-SMP-ORG-003. Kinds are `open decision`, `awaiting confirmation` and `requirement question`.

The key has three consequences:
- **Moving questions doesn't move answers.** Adding, removing or reordering other questions on the same record never re-attaches an answer to the wrong question.
- **Editing a question's text orphans its answer.** The generator reports the orphan as an error (`--check` exits 1). It never silently re-attaches the answer. Re-answer the edited question, or restore its text.
- **Later answers win.** When two resolution records carry the same key, the later one supersedes. A later dated batch can therefore revise an earlier answer without deleting it.

## Files

| Path | Content |
|---|---|
| `<date>/resolutions.jsonl` | One record per answered question. Fields: `resolution_id` (QR-nnnn, never reused), `question_key`, `record_id`, `kind`, `question` (exact text when answered), `disposition`, `answer` (the product direction), `gate` and `gate_type` (`external`, `validation` or `none`), `revision` (the revised record, if any), `answered_by`, `answered_on`, plus provenance fields. |
| `<date>/revisions.jsonl` | The text of each revision a batch made to a source record. Applied as additions only (see below). |
| `2026-11-24/CHECK.md` | The integrator's check of the 2026-11-24 review: verified claims, conflicts and choices made where the review was silent. |

## Commands

```
python3 -I -B tools/reqaudit/render_questions.py .            # regenerate OPEN-QUESTIONS.md and open-questions.csv
python3 -I -B tools/reqaudit/render_questions.py . --check    # integrity only; exit 1 on duplicate keys or orphaned answers
python3 -I -B tools/reqaudit/test_render_questions.py         # key stability, joins, orphan detection, determinism, committed output
```

## Recording new answers

1. Create a new dated directory with a `resolutions.jsonl`.
2. Take each `question_key` from `open-questions.csv` and continue the QR numbering.
3. Regenerate the register.

Do not edit the generated register or CSV by hand.

## How revisions are applied to source records

`tools/reqaudit/apply_revisions_2026_11_24.py` applies a batch's revisions without editing any existing text:
- **A refinement** `RR-<orig>-r<n>` is superseded by a new `RR-<orig>-r<n+1>` in the simplification mission's `refinements.jsonl`. It copies the original text and adds the revised statement, the added acceptance criteria and `supersedes_refinement`.
- **A gap requirement** (`UNB-XIN-*`, `UNB-SMP-*`) gets an entry in its `revisions` list. The original `statement` stays as it was, because some of these were adopted verbatim into domain files.
- **A domain file that adopted a revised record** gets a one-line pointer under that record's provenance line.

Answering a question does not change a decision record's own `status`. The answers are product directions, and external gates stay open until the named counsel, CPA, auditor, bank, board or counterparty confirms.

# Resolutions of 2026-11-24: applied revisions (historical import)

**Answers to the open questions are recorded only in [`../questions/`](../questions/README.md).** That append-only event log comes from the master mission (PR #8) and feeds `tools/reqaudit/render_questions.py`. This directory was a parallel import of the same 2026-11-24 review, made in PR #9 before the two lines of work were reconciled. It is kept because it is the source of the revisions PR #9 applied to the requirement records. No generator reads `resolutions.jsonl` any more.

| File | Status | Content |
|---|---|---|
| `2026-11-24/revisions.jsonl` | **Live input** to `tools/reqaudit/apply_revisions_2026_11_24.py` | The revised statements and added acceptance criteria. There are 14 superseding refinements and 18 gap-requirement revisions, plus the new requirement UNB-SMP-ORG-004. |
| `2026-11-24/resolutions.jsonl` | Historical | 316 records (QR-0001…) importing the review. The QR numbers are cited by the revisions and by the two domain-file pointers. For the current answer and approval status of any question, use `../open-questions.csv`. |
| `2026-11-24/CHECK.md` | Historical, with a reconciliation note | The integrator's check of the review: facts verified, two conflicts with SPD-ADV-001, and choices made where the review was silent. |

## How the two lines were reconciled (2026-11-25)

**Question system.** The master mission's event log and generator are authoritative. PR #9's stable-hash generator and its importer were removed. Their purpose is covered by `questions_lib.py`, which pins each question's text by SHA-256 and fails on changed text, orphans or invalid closures, and by `test_question_register.py`.

**Requirement records.** Each side's changes were merged field by field, and no field was changed by both:
- The master mission added `converged_by` and `revision_history` to 121 records.
- PR #9 added `revisions` to 18 gap requirements, 14 superseding refinements and UNB-SMP-ORG-004.

**Precedence.** It follows [`../AUTHORITATIVE-BASELINE.md`](../AUTHORITATIVE-BASELINE.md): convergence clauses `CNV-*` govern where they conflict. PR #9's revisions and new records are simplification-mission proposals. They appear in the baseline with status `unvalidated` or `proposed` and do not override a clause.

**Event log.**
- 32 `applied-to-requirement` events add each PR #9 revision to its question's `applied_to`, alongside the existing clause references.
- One `evidence-reviewed` event records the surge-bench constraint on SPD-ECN-004.
- The other conflict in `CHECK.md` (SPD-ORG-002, fractional officers) was already enforced by clause CNV-GOV-004, which refuses a second sponsor's appointment.

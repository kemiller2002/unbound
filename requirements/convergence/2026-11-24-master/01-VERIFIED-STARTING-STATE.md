# 01 — Verified starting state (Phase 0)

Mission: Unbound master agent mission (evidence resolution, requirements convergence and AI-native
enterprise validation). Verified on 2026-11-24 against `main` @ `ab1638d` (branch
`claude/unbound-master-agent-0r5kql` created from it; no other outstanding remote branches exist in
`kemiller2002/unbound`). Reproduce with:

```bash
python3 -I -B tools/reqaudit/review_map.py . requirements/convergence/2026-11-24-master/data
```

Outputs: [`data/review-map.csv`](data/review-map.csv) (one row per register question with every review
that touched it) and [`data/review-map-summary.json`](data/review-map-summary.json).

## 1. Register counts

| Claim | Source record check | Verified |
|---|---|---|
| 143 requirement questions | `open_questions` entries in the four discovery/simplification `gap-requirements.jsonl` and `refinements.jsonl` files (reconciliation records carry none) | 143 |
| 101 open decisions | `status == open` in cross-industry `decisions.csv` (45) and simplification `decisions.csv` (56); reconciliation has none open | 101 |
| 72 confirmation questions | reconciliation `decided-provisional` decisions whose `validation_required` names a confirmer (33 of 34) + cross-industry `decision-outcomes.jsonl` rows whose `confirmation_required` names a confirmer (39 of 41) = 72 | 72 |
| 316 total | | 316 |
| No duplicate IDs | | none |
| Confirmation classes 8 / 5 / 59 | confirmation review table | Product 8, Validation 5, External 59 |

Not in the register: the simplification mission's three `decided-provisional` owner decisions
(SPD-ADV-001, SPD-ADV-004, SPD-ECN-003). Its `decisions.csv` has no confirmation column, so the generator
cannot tell whether they need confirmation. They are owner decisions recorded in that mission's ledger and
are treated here as decided (finding P0-06).

## 2. Review artifacts located and mapped

| Artifact (`requirements/discovery/`) | Rows | Mapped to exact IDs | Orphans |
|---|---|---|---|
| `2026-11-24-product-owner-resolution-review.md` | 32 decision sections | 32 | 0 |
| `2026-11-24-initial-38-question-resolutions.md` | 38 | 38 | 0 |
| `2026-11-24-human-intervention-and-question-resolution.md` | 21 (+1 proposed cross-cutting requirement) | 21 | 0 |
| `2026-11-24-question-resolutions-accounting-banking-economics.md` | 29 | 29 | 0 |
| `2026-11-24-final-55-question-resolutions.md` | 55 | 55 | 0 |
| `2026-11-24-adversarial-approval-review.md` | 143 (111 approved, 29 revise, 3 hold) | 143 | 0 |
| `2026-11-24-approved-corrections-and-decision-gates.md` | 29 corrections + 15 cross-register directions | 29 | 0 |
| `2026-11-24-three-evidence-hold-resolutions.md` | 3 | 3 | 0 |
| `2026-11-24-open-decision-resolution-review.md` | 101 | 101 (quoted text identical to source: 101/101) | 0 |
| `2026-11-24-confirmation-question-review.md` | 72 | 72 (quoted text identical: 72/72) | 0 |
| `2026-11-24-zero-consultation-evidence-review-01.md` | 6 questions (3 substantive, 3 preliminary) | 6 | 0 |

Requirement-question artifacts cite only the record ID and a short label. Rows were mapped to `#Qn` by
order within each record and every pair was read against the register text (table in `review-map.csv`).
Every record had exactly as many review rows as questions; no count mismatches.

## 3. Findings

| ID | Finding | Effect | Action |
|---|---|---|---|
| P0-01 | **Register generator discards answers.** `render_questions.py` rewrote `open-questions.csv` with blank `answer`, `answered_by`, `answered_on`. Reproduced: one hand-entered answer, regenerate, zero answers. | Any answer recorded where the register told reviewers to record it would be lost. No answers had yet been recorded, so nothing was lost. | Fixed (section 4). |
| P0-02 | Requirement-question IDs are positional (`#Qn`). | Reordering `open_questions` silently re-keys answers. | Registered text + SHA-256 now pinned; validator fails on change. |
| P0-03 | RR-UNB-GOV-002-r1#Q1 asks "Auditor/CPA acceptance (SPD-PLT-001)". The hold resolution answers a different question (whether a one-approver tightening exception is allowed). | Answer addresses the parent refinement, not the question. | Hold text retained as a correction to the refinement; the question is a same-question duplicate of SPD-PLT-001 and is answered there. |
| P0-04 | UNB-XIN-PAY-006#Q1 cites XID-BNK-006 (card reversal windows) but asks about donor-covered fees, which is XID-BNK-002. | Wrong cross-reference in source text. | Cluster maps it to XID-BNK-002; source text left unchanged (positional ID) and noted. |
| P0-05 | Several "questions" are routing notes, not questions: RR-UNB-ACC-004-r1#Q1, RR-UNB-ACC-014-r2#Q1, RR-UNB-ACC-008-r1#Q1, RR-REQ-X-048-r3#Q1, RR-UNB-ACC-022-r1#Q1. | Reviews answered the underlying refinement. Acceptable, but each is a duplicate of a decision. | Marked same-question duplicates of their decisions. |
| P0-06 | Three owner decisions (SPD-ADV-001, SPD-ADV-004, SPD-ECN-003) are outside the register. | Register understates decided scope. | Recorded in the decision register (report 07) as owner decisions. |
| P0-07 | 72 same-question and 2 sub-question duplicates across 69 canonical questions, mostly requirement questions that restate a decision (for example UNB-XIN-REG-001#Q1 and XID-REG-001). | 316 questions contain about 240 distinct issues. Duplicates carried different answers before the corrections (for example RR-UNB-EXT-004-r2 "12 month maximum" against SPD-GRT-003 "counsel-set"; RR-ADM-023 and UNB-XIN-EVA-012 "any spouse" against XID-ADV-004 "recorded authority"). | `requirements/questions/question-clusters.csv`; validator warns on conflicting dispositions. |
| P0-08 | Initial-38 artifact records that the previous chat produced 37 answers while claiming 38; it added RR-REQ-X-073-r1. | Verified: 38 rows now present. | None. |
| P0-09 | Confirmations XID-CRM-001 and XID-REG-003 name "None beyond template/consent wording review" as confirmer but were classed External. | Classification inconsistent with the source. | Re-examined in Phase 1 (batch F). |
| P0-10 | "Approved" status in prior artifacts is product-direction approval under delegation; the 72 confirmations were reviewed but never approved. No professional confirmation was claimed anywhere. | Must not be read as legal, CPA or bank approval. | Approval status recorded per question. |
| P0-11 | Prior artifacts state that resolutions had not been applied to source requirement records and the register had not been regenerated. | Verified: no source record or normative file changed after commit `6c5115f`. | Phase 2. |

## 4. Register governance repair

| Item | Location |
|---|---|
| Event-sourced resolution log (845 seed events reproducing every artifact above) | `requirements/questions/resolution-events.jsonl` |
| Duplicate and related clusters | `requirements/questions/question-clusters.csv` |
| Schema and rules | `requirements/questions/README.md` |
| Fold and validation library | `tools/reqaudit/questions_lib.py` |
| Generator joins log, pins kinds, refuses to discard hand answers, fails on validation errors | `tools/reqaudit/render_questions.py` |
| Seed (refuses to overwrite) | `tools/reqaudit/seed_resolutions.py` |
| Regression tests: idempotent regeneration, hand-answer guard, orphan, duplicate event ID, changed text, invalid closure, closed source decision stays registered (7/7 pass) | `tools/reqaudit/test_question_register.py` |

Existing validators after the change: `validate.py` 0 errors, `validate_discovery.py` 0 errors,
`validate_simplification.py` 0 errors.

## 5. Starting status (seeded, before Phase 1)

| Kind | Resolution | Approval | Count |
|---|---|---|---|
| Requirement question | proposed (answer present) | approved-product-direction | 143 |
| Open decision | proposed | approved-product-direction (provisional) | 101 |
| Awaiting confirmation | proposed | not-approved | 72 |

No question had an evidence disposition. Only six had any primary-source research (review 01).

# Check of the 2026-11-24 product-owner review

> **Reconciliation note (2026-11-25).** This check was written in PR #9 before it was reconciled with the master mission (PR #8). Answers and approvals are now recorded only in `requirements/questions/` (see `../README.md`). Section 3's SPD-ORG-002 conflict was already enforced there by clause CNV-GOV-004. The SPD-ECN-004 note is recorded as an `evidence-reviewed` event. The `--check` command and the tests named in this file were replaced by `questions_lib.py` and `test_question_register.py`.

**Scope.** This checks the eleven review files under `requirements/discovery/2026-11-24-*.md` (commits `0cf42ce`..`ab1638d`). It also records how they were turned into resolution records and source-record revisions. It is a consistency and spot-fact check by the integrating agent, not legal, accounting or audit review.

## 1. Coverage

| Kind | Questions | Answered | How matched |
|---|---:|---:|---|
| Open decisions | 101 | 101 | By decision id. For 26 ids (15 rows), the cross-register table in `approved-corrections-and-decision-gates.md` overrides the per-decision direction. |
| Provisional decisions awaiting confirmation | 72 | 72 | By decision id |
| Requirement questions | 143 | 143 | By record, then by position within the record. The question count per record agrees for all 122 records. For the 20 records with more than one question, each short label was checked by eye against the exact question text. |
| **Total** | **316** | **316** | |

The requirement-question outcomes are:

| Outcome | Count |
|---|---:|
| Approved | 111 |
| Revised (the 29 corrections) | 29 |
| Held, then resolved by `three-evidence-hold-resolutions.md` | 3 |

No answer was left unmatched and no question was left unanswered (`render_questions.py --check`: 0 integrity errors).

## 2. Claims checked against primary sources

| Claim in the review | Result |
|---|---|
| FOCUS 1.4 was ratified on 4 June 2026 and is the current version | **Confirmed.** The FinOps Foundation announcement ("Introducing FOCUS 1.4") and focus.finops.org list 1.4 as the latest release, ratified 4 June 2026. Validator support follows later. |
| OFAC records must be kept 10 years (RCD-007 candidate) | **Confirmed.** The OFAC final rule (90 FR 13286, 21 March 2025) adopts the interim rule extending 31 CFR 501.601 recordkeeping from 5 to 10 years. The period runs from the transaction date. |
| Zero errors in a sample of 30 still allows an upper 95% error bound of about 10% | **Correct.** The exact bound is 1 − 0.05^(1/30) ≈ 9.5%, and the "rule of three" gives 3/30 = 10%. |
| Sections 4962, 4963, 4966 and 4967 (XID-NPG-007, RCD-014) | **Consistent with the statute as cited.** The 4963(a) list of first-tier taxes includes 4966(a) and 4967(a). Abatement under 4962 needs reasonable cause and timely correction. 4966(c) treats distributions to natural persons as taxable. I did not re-fetch these sections. |
| OFAC reports within 10 business days, with an annual blocked-property report (RCD-044) | Consistent with 31 CFR 501.603–604 as summarized; not re-fetched. |

## 3. Conflicts with recorded owner decisions

These two answers conflict with SPD-ADV-001 (no person or team may decide for more than one sponsor). They are kept as written, with an `integrator_note` on the resolution record:

| Decision | Review answer | Conflict | Suggested reading |
|---|---|---|---|
| SPD-ORG-002 | Support formally appointed fractional officers with scope, expiry and independence. | Fractional officers usually serve several organizations. | A fractional officer may hold A3+ authority for **one** sponsor only. UNB-SMP-ORG-003 already refuses a second sponsor's A3+ entry. |
| SPD-ECN-004 | Pre-contract a qualified surge bench. | A shared bench would decide for several sponsors. | The bench may prepare work across sponsors, but a member decides only as an appointed member of one sponsor's DecisionTeam. |

No other answer contradicts SPD-ADV-001 or SPD-ADV-004 (software platform only).

## 4. Revisions applied to source records (additions only)

| Count | What | Where |
|---:|---|---|
| 14 | Superseding refinements: the 12 refinement corrections, plus RR-UNB-GOV-002-r2 and RR-REQ-X-073-r2 from the hold resolutions | Simplification mission `data/refinements.jsonl`, with 49 trace edges |
| 18 | Gap-requirement revisions: the 17 corrections to UNB-XIN-* and UNB-SMP-* records, plus UNB-XIN-ECO-005 from the FOCUS hold resolution | Appended to each record's `revisions` list. The original statements are unchanged. |
| 2 | One-line pointers in domain files: UNB-XIN-ERP-001 in `FULL-ACCOUNTING-SYSTEM.md` and UNB-XIN-BNK-003 in `DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md` | Adopted text kept verbatim |
| 1 | New requirement UNB-SMP-ORG-004 (justified human intervention), from the cross-cutting proposal in `human-intervention-and-question-resolution.md` | Scenario SPS-ORG-010 |

**Why RR-UNB-GOV-002 needed a revision.** RR-UNB-GOV-002-r1 says a tightening change needs only one approver. The hold resolution rejects that, so r2 replaces it with independent approval for every material change.

**Choices made where the review was silent.** Each is reversible by a later resolution batch.

- **RR-UNB-EXT-004-r3.** Until counsel records a maximum age, sanctions false-positive dispositions are **not reused**. This is conservative and costs review time. The review said only "counsel sets disposition reuse".
- **UNB-XIN-ERP-001 revision 1.** There is no default proration of a minimum fee, and no closing-period minimum unless the contract discloses one. The opening-period treatment also moves to the schedule version, because the correction says "do not prorate a minimum fee by default".
- **RR-UNB-API-STD-003-r2.** The 18-month rule applies to Public contracts. Partner contracts keep 12 months, or the contract term if that is longer.
- **UNB-XIN-CLM-001 revision 1.** It uses a 60-day window with mandatory exceptions. It keeps the five outcomes of the original record. "Referred" from SPD-ENT-002's earlier proposal was not added, because the cross-register direction did not repeat it.
- **UNB-XIN-ADV-002.** It is cited in `FULL-ACCOUNTING-SYSTEM.md` but has no adopted entry of its own, so no pointer was added there.

## 5. Gate classification

Each answer's gate is classified from its text, by keyword: `external` names a person or organization outside the product team (counsel, CPA, auditor, bank, board, sponsor and similar), `validation` names a test, pilot or measurement, and `none` is everything else. Classification is approximate. The register groups the 209 external gates by who must confirm:

| Who must confirm | External gates |
|---:|---|
| Counsel | 107 |
| Finance | 67 |
| Sponsor governance | 21 |
| Technology | 8 |
| Product owner | 6 |

## 6. Not done

- **No decision record's `status` was changed.** The review accepted directions provisionally and said explicitly that this is not formal closure.
- **No external confirmation was obtained, and none is claimed.**
- **The zero-consultation evidence dossiers have not been extended beyond batch 01.** Batch 01 covers three established findings and three preliminary ones, attached as `evidence` on RCD-014, RCD-044, XID-NPG-007, RCD-016, RCD-024 and XID-INV-005.
- **`daf-landscape` is unchanged.**

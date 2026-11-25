# 25: Test scenario links for the 2026-11-24 revisions

**Date:** 2026-11-25. **Status:** agent-drafted proposals for owner confirmation. Nothing here is approved.

The owner review of 2026-11-24 (`requirements/resolutions/2026-11-24/revisions.jsonl`) added 12 superseding refinements to `data/refinements.jsonl`. No scenario in `data/scenarios.csv` cited them, so gate D7 failed with 12 errors once the trace graph was regenerated.

This draft fixes D7 in two ways:

- Where an existing scenario still tests the revised behaviour unchanged, the revision id is added to that row's `requirement_ids`. The predecessor id stays.
- Where the revision changes behaviour no scenario exercises, one new scenario row (`SPS-REV-001` to `SPS-REV-012`) tests what changed. Each new row's `mock_data_notes` starts with "Drafted 2026-11-25 from ...; owner to confirm".

No refinement, requirement text or other mission's data was changed.

## How to confirm or edit

1. Edit `data/scenarios.csv`. Keep every field quoted and the CRLF line endings. To reject a link, delete the revision id from that row's `requirement_ids`. To reject a new scenario, delete its row. A revision with no scenario left fails D7.
2. When a scenario is confirmed, delete "Drafted 2026-11-25 ...; owner to confirm." from its `mock_data_notes`.
3. Rerun from the repository root:

```
R="dotnet run --project tools/fs/Unbound.ReqAudit -c Release --"
$R graph requirements/discovery/2026-11-24-simplification-economics/data
$R validate-simplification .
$R assemble-simplification .
$R validate-discovery . && $R validate .
dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .
```

## Proposals

| Refinement | What the revision changed | Existing scenarios linked | New scenario and what it tests | Confidence |
|---|---|---|---|---|
| RR-ADM-002-r2 | Prefill must carry source, time, purpose and consent; it never satisfies identity proofing or silently overwrites a verified value. | SPS-ENT-001 | SPS-REV-001: identity-provider address differs from the verified address; both shown with sources, verified value kept, proofing still runs; a value outside its freshness window is not prefilled. | High |
| RR-ADM-023-r2 | Joint authority comes from the fund agreement, never from marital status; capacity is reserved atomically at commit. | None (predecessor tested only by XDS-TRS-005, XDS-TRS-006, XDS-ADV-004) | SPS-REV-002: spouses with no any-one clause wait for each other; two any-one holders commit $8,000 and $6,000 against $10,000; one reserved, one HeldConflict with a sponsor case. | High |
| RR-REQ-X-007-r4 | Post-renewal review sample size follows each stratum's confidence and tolerable error; a fixed 30 is an exploratory pilot only. | SPS-GRT-001, SPS-GRT-004, SPS-GRT-006 (they cite r2, the predecessor; renewal, sanctions hold and outage behaviour is unchanged) | SPS-REV-003: 95% confidence and 3% tolerable error give a sample of 99, not 30; a 30-item pilot is labelled exploratory (upper bound about 9.5%); a 40-item stratum is reviewed in full. | Medium |
| RR-UNB-ACC-008-r2 | One authoritative book per entity and basis, which may be an external ERP; Unbound need not own the general ledger. | SPS-FIN-004 | SPS-REV-004: external ERP is book of record; program report reconciles to it with a typed in-transit item; no parallel authoritative ledger; a second authoritative book is rejected. | High |
| RR-UNB-ACC-022-r2 | External ERP is the default, not mandatory; Unbound may be the authoritative general ledger without generic ERP modules. | SPS-FIN-002 | SPS-REV-005: sponsor without an ERP runs Unbound as its general ledger; close produces program and entity statements; no AP or payroll module. | High |
| RR-UNB-API-STD-003-r2 | Public contracts stay 18 months (was 12); Partner 12 months or the contract term if longer; Internal no minimum; recorded emergency exception. | None (predecessor tested only by XDS-SAS-015) | SPS-REV-006: Public sunset at 12 months rejected and at 18 months accepted; Partner sunset at 12 months rejected when the term is longer; Internal accepted; emergency sunset accepted only with approval, notices and mitigation. | High |
| RR-UNB-EXT-028-r2 | Each source declares freshness; decisions use cached evidence inside the window and hold when evidence is stale or the source is down. | SPS-GRT-003 | SPS-REV-007: 10-hour-old evidence under a 24-hour window releases with no live call; 30-hour-old evidence during an outage holds with StaleEvidence. | High |
| RR-UNB-GFUND-012-r2 | A funding-source swap proceeds on notice only if allocation, restrictions, tax and accounting stay unchanged and policy permits it; otherwise advisor approval. | SPS-FIN-003 (its swap matches the revision's own AC-1, but its preconditions do not state the four conditions) | SPS-REV-008: a swap that keeps the pool at 77.8% (band 60-80%) proceeds on notice; one that moves it to 93.3% waits for advisor approval. | Medium |
| RR-UNB-RES-003-r2 | Recovery objectives must be approved and restore-tested before architecture acceptance; no C0 loss without an approved procedure. | None (predecessor tested only by XDS-SAS-012) | SPS-REV-009: acceptance refused without an approved table, refused without a passing C0 drill, accepted with both; a restore that would lose C0 facts without an approved procedure does not complete. | High |
| RR-UNB-RULE-019-r2 | Promotion past the first stage needs representative volume per affected risk stratum, no trigger and a rehearsed rollback; elapsed time alone is not enough. | SPS-EDF-001, SPS-EDF-002, SPS-CLR-005, SPS-ADV-003 (shadow evaluation, trigger halt and rollback are unchanged) | SPS-REV-010: day 7 with 12 of 50 high-risk items is refused; full volume without a rehearsed rollback is refused; promoted once rollback is rehearsed. | Medium |
| RR-UNB-GOV-002-r2 | Material changes need independent approval whether they tighten or loosen; one approver only for low-risk changes under an approved change policy; mixed changes are material. | None. SPS-PLT-008 is not linked: it expects a tightening to publish with one approver, which r2 forbids. | SPS-REV-011: lowering the ceiling (6 queued grants would miss deadlines) needs an independent approver and the author cannot self-approve; raising it also needs one; a wording change under the change policy needs one approver; a mixed change is material. | High |
| RR-REQ-X-073-r2 | A bridge letter does not extend independent assurance; the register tracks coverage gap, opinion, exceptions and risk acceptance; suspension needs a continuity decision. | None (predecessor tested only by XDS-ERP-011) | SPS-REV-012: vendor with a bridge letter and one unmapped CUEC is not Assured and shows the gap; adverse opinion and expired risk acceptance change status; suspension waits for a recorded continuity decision. | High |

## Points for the owner

- **SPS-PLT-008 now contradicts RR-UNB-GOV-002-r2.** It still cites r1 and expects a tightening to publish with one approver. Decide whether to retire it or rewrite it. Also, the acceptance criterion `RR-UNB-GOV-002-r1-AC1` carried into r2 still says "publishes with one approver"; it conflicts with r2's statement and AC-R1.
- **SPS-EDF-002** keeps its "14-day observation" precondition. Its tested behaviour (halt and rollback on a trigger) matches r2's AC-2, which also mentions 14 days. Under r2, days alone do not allow promotion.
- **XDS-ADV-004** (cross-industry mission, not changed) assumes an "Any-one below threshold" default. RR-ADM-023-r2 makes AllRequired the default unless the agreement grants any-one.
- **SPS-FIN-003** is linked to RR-UNB-GFUND-012-r2 on the strength of the revision's own AC-1 and `validation_scenarios`. If the owner wants the four conditions stated in its preconditions, edit that row.
- **RR-REQ-X-007-r4** supersedes r2. SPS-ORG-005 and SPS-ORG-006 cite r3 and r1 and were not linked to r4. The sample size of 99 assumes zero expected errors; the owner may prefer another sampling plan.

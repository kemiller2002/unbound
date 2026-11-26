# Competitive displacement decisions: requirements discovery (2026-11-26)

Research: kemiller2002/daf-landscape, `research/competitive-displacement/` (Phase II, merged as PR #92 at `10b92f4`). Start with its `17-unbound-implications.md`, `23-executive-decision-register.md` and `22-commercial-validation-plan.md`; the machine-readable rows are `data/registers/decisions.csv` and `data/registers/cross-repository-implications.csv`. Research reports are cited by path, not copied.

Status: **proposed**.

- On 2026-11-26 the owner approved Phase II decisions DEC-001..DEC-012 and authorised progression of XRI-001..XRI-018, subject to their documented conditions. In Unbound every one of those decisions is at **Strategically approved**. None is requirements-incorporated, evidence-gate-satisfied, implementation-authorised, implemented or validated.
- Nothing here amends the authoritative baseline, a convergence clause, an owner decision or the generated `AUTHORITATIVE-BASELINE.md`.
- No requirement is implemented or validated, and there is no application code.
- Nothing here authorises expenditure, contracts, outreach, production deployment or the removal of a mandatory control.

## What this package does

1. **Records the approved decisions** that bear on Unbound (DEC-001..007, 009, 010, and 008, 011 and 012 where they constrain product scope), each with the owner's conditions, preserved validation gates, Unbound effect, requirement IDs and the text that governs now ([04](04-DECISIONS-AND-OPEN-QUESTIONS.md)).
2. **Processes XRI-001..XRI-008** one by one, carrying each research register row verbatim and assigning exactly one disposition class ([03](03-CROSS-REPOSITORY-IMPLICATIONS.md)).
3. **Adds or refines 30 requirements** (UNB-CDD-CIV/ONB/CMF/OEM; [02](02-REQUIREMENTS.md)) covering charity intelligence, the onboarding gaps, commercial flexibility and operational-economics measurement. Onboarding reuses UNB-ONB-001..033 and UNB-TRN-* wherever they already govern; the coverage table in [01](01-TRACEABILITY.md) shows which capability is existing, refined, new or an investigation item.
4. **Checks the architecture constraints** and records "no change" for each ([05](05-RECONCILIATION-AND-ARCHITECTURE.md)).
5. **Reconciles** approved decisions with ratified clauses. Where a decision would need ratified text to change, the ratified text governs now and a new owner decision is raised.

## Dispositions

| XRI | Disposition | Requirements | Owner decisions |
|---|---|---|---|
| XRI-001 entry segment | Requires a new owner decision | UNB-CDD-CMF-001, -003 (architectural support) | OD-CDD-01, OD-CDD-05 |
| XRI-002 evidence classes | New requirement added | UNB-CDD-CIV-001..003, -010..013 | — |
| XRI-003 180-day re-review | Requirements updated (inside the preserved 180-day ceiling) | UNB-CDD-CIV-006..009 | OD-CDD-02 |
| XRI-004 sources and rights | New requirement added | UNB-CDD-CIV-004, -005 | — |
| XRI-005 release archive | Already satisfied (UNB-CNET-003, UNB-TRN-002, UNB-CHAR-008) | — | — |
| XRI-006 event instrumentation | New requirement added | UNB-CDD-OEM-001..004 | — |
| XRI-007 prevention | Requirements updated | UNB-CDD-OEM-005, -006 | — |
| XRI-008 licensing mode, TPSP, SOC 1 | New requirement added (TPSP and SOC 1 Type 2 already required by CNV-PAY-001 and CNV-GOV-001) | UNB-CDD-CMF-001, -002, -004..006 | — |

## Conflicts escalated, not resolved

| Approved decision | Ratified or existing text | Governs now | Raised |
|---|---|---|---|
| DEC-001 partner-first entry | CNV-ECO-004 entry segment (OD-M-05) | CNV-ECO-004 | OD-CDD-01; concentration under a partner channel OD-CDD-05 |
| DEC-003 change-driven re-vetting | CNV-GRT-003 "deep review SHALL default to 180 days"; RR-REQ-X-007-r1 "comprehensive charity review" | CNV-GRT-003, RR-REQ-X-007-r1 | OD-CDD-02 (the 180-day ceiling itself is preserved, as the owner required) |
| DEC-006 licensing first | ~20 bps working assumptions in OD-CNO-10 (open) and FIL-OD-10 (recorded answer) | OD-M-11; those records unedited | OD-CDD-03 |
| DEC-002 defer custodial administration | OD-M-08 staffed-administration second offer; CNV-GOV-001 SOC 1 timing | OD-M-08, CNV-ECO-004, CNV-LEG-001 | OD-CDD-04 |

Open charity-network decisions are **not** answered by the approvals and remain open. These are OD-CNO-04 and -13 (re-vetting), -05 and -09 (sharing and publication), -10 (pricing), -12 (Nacha allocation), -01, -07 and -08 (migration) and -02 (partner-bank destinations).

**Echelon Foundry check.** No ratified clause requires Echelon Foundry to be a DAF sponsor or administrator:

- CNV-PAY-001 names the sponsor as Originator and Unbound as a TPSP.
- CNV-GOV-001 plans a SOC 1 Type 2 when a user auditor intends to rely on Unbound's controls.
- CNV-LEG-001 forbids Unbound from holding funds.

UNB-CDD-CMF-005 makes this explicit.

## Release-1 scope impact

This package proposes 20 requirements with release scope `initial-implementation`, 11 of them P0. They are listed in [05](05-RECONCILIATION-AND-ARCHITECTURE.md), generated from the data.

- They are **proposals pending owner ratification**: they would add to Release-1 scope only once the owner ratifies them into the baseline.
- The ratified Release-1 scope is unchanged until then.
- None was moved to a later scope to avoid this.

## Ownership of what is made (owner decision OD-P3-01)

The owner decided on 2026-11-26, in the owner's words: "EF owns all property we make". It is recorded in kemiller2002/echelon-organization-administration as OD-DAF-IP-001 (`strategy/daf-entry/04-owner-decision-register.md`). Echelon Foundry owns the IP this initiative creates, including Unbound requirements and tooling.

- This is an ownership decision, not a sponsor or administrator role; UNB-CDD-CMF-005 is unaffected.
- No LICENSE file is changed.
- **TODO (counsel):** confirm how copyright in contributed work is assigned to Echelon Foundry (contributor assignment or work-for-hire terms) and whether repository licence notices need to change.

## Kill rule

The DEC-008 kill rule ("no named partner") is read with two owner decisions:

- **OD-P3-04:** the clock starts when DAF commercialization becomes its own cost center at Echelon Foundry. It is recorded as OD-DAF-CAP-001/002 in echelon-organization-administration.
- **OD-P3-02:** the kill rule does not stop Transitus.

Neither changes an Unbound requirement.

## Source-cluster selection (DEC-007)

Unbound owns the single first-incumbent source-cluster selection record (UNB-CDD-ONB-002).

- Transitus consumes that record through its proposed TRN-REQ-MP-010 and does not keep its own.
- Package versioning and the drift test are Transitus TRN-REQ-MP-004. They are cited here, not re-specified.

## Contents

| File | Content |
|---|---|
| [01-TRACEABILITY.md](01-TRACEABILITY.md) | Forward and reverse trace; coverage of the brief's capability lists (generated) |
| [02-REQUIREMENTS.md](02-REQUIREMENTS.md) | 30 proposed UNB-CDD requirements, each with every mandated field (generated) |
| [03-CROSS-REPOSITORY-IMPLICATIONS.md](03-CROSS-REPOSITORY-IMPLICATIONS.md) | XRI-001..008 processing (generated) |
| [04-DECISIONS-AND-OPEN-QUESTIONS.md](04-DECISIONS-AND-OPEN-QUESTIONS.md) | Approved-decision status, validation gates, owner decisions OD-CDD-01..05 and requirement questions (generated) |
| [05-RECONCILIATION-AND-ARCHITECTURE.md](05-RECONCILIATION-AND-ARCHITECTURE.md) | Reconciliation with ratified text and the architecture constraint check (generated) |
| `data/gap-requirements.jsonl` | Source of truth for the requirements |
| `data/implications.jsonl` | Source of truth for the XRI processing (with the verbatim research rows) |
| `data/approved-decisions.csv` | Source of truth for the approved decisions' Unbound status |
| `data/decisions.csv` | Source of truth for the new owner decisions |
| `data/validation-gates.csv`, `data/coverage.csv`, `data/constraint-checks.csv` | DEC-012 gates with lower-cost routes; capability coverage; architecture constraints |

## Question register integration

- This package is registered as a source of the open-question register. It was added to `QuestionsLib.MISSIONS` and `RenderQuestions.SOURCES`.
- `dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .` now yields 374 questions. Nine are new: OD-CDD-01..05 and four requirement questions. All nine are open and not approved.
- The regression test `tools/fs/Unbound.Convergence.Tests` was updated to that count. It asserts that the earlier answered questions stay answered and that the charity-network and competitive-displacement questions are open.
- No resolution events were appended.

## Validation

```bash
dotnet run --project tools/fs/Unbound.CompetitiveDecisions -c Release -- validate . --check --research-root ../daf-landscape   # gates A1-A11
dotnet test tools/fs/Unbound.CompetitiveDecisions.Tests
dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .                                              # register (no diff expected)
dotnet test tools/fs/Unbound.Convergence.Tests
```

The validator is F# (`tools/fs/Unbound.CompetitiveDecisions`), in the functional style of `Unbound.Ingestion`: pure gates over immutable inputs, with I/O only in `Program.fs`. Python was not used because the F# precedent covers the need. It checks:

- ID form and uniqueness, and that exactly XRI-001..008 and DEC-001..012 are present;
- all mandated fields are present;
- every citation resolves against the baseline, sibling packages, owner decisions and decision registers;
- the status ladder: requirements are proposed, not implemented and not validated, and decisions stay at Strategically approved;
- each disposition class is consistent with what it creates, refines and raises;
- owner-decision integrity;
- with `--research-root`: research paths and register IDs exist, and every XRI research row matches the research register verbatim;
- every architecture constraint states "no change", and the Echelon Foundry check is present;
- coverage vocabulary;
- the generated sections are current.

Existing validators were re-run and are unchanged by this package:

- `validate_charity_onboarding.py . --check --research-root ../daf-landscape`, `validate_availability.py . --check`, `Unbound.Ingestion validate . --check`, `ReqAudit validate` and `validate-simplification` all pass. `ReqAudit validate` gate G4 needs the full git history (it reads commit `777a70d`), so a shallow clone must be unshallowed first.
- `validate-discovery` reports one error, D9 (the unknown `RR-ADM-023-r2` in the cross-industry report). It reproduces on the base commit `79b9258` without this change.

## Not done

- **No adoption into the baseline and no convergence clause.** Both require owner ratification.
- **No implementation and no validation claim.** No evidence gate (V-1..V-9) has run.
- **No edits to ratified clauses, owner decisions, OD-CNO records, FIL-OD-10 or other packages' records.**
- **No architecture change.**

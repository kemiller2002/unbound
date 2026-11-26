# 04 — Decisions and open questions (Transitus integration)

## Proposed decisions (for owner ratification; not added to `owner-decisions.jsonl`)

| ID | Proposal | Basis |
|---|---|---|
| TRN-D-1 | Unbound consumes the Transitus Formats, Checks and Evidence libraries for release-1 feeds. Feed modules and domain operations remain Unbound code. | Transitus DF-TRN-2026-0016; adversarial review TRN-REV-01 |
| TRN-D-2 | Echelon.Execution stays incubated in `execution/` during Transitus phase 1. Extraction to a shared component is proposed only when Transitus trigger T2 fires, after the RM-05 benchmark. | Transitus DF-TRN-2026-0006; ADR-EAR-004 ("extraction when a second consumer starts") |
| TRN-D-3 | The IRS and OFAC parsers in `Unbound.Replay/Sources.fs` move to `Transitus.Formats`; the replay tool consumes them. | TRN-REQ-ARC-006 |

## Open questions for the Unbound owner

| ID | Question | Notes |
|---|---|---|
| TRN-Q-1 | Is donor and fund history migration (CNV-EXP-002) in release 1, given the simplification proposal "no migration in R1" and OD-M-14 (no design partner)? | Blocks UNB-TRN-006 and UNB-TRN-011, and the conditional Transitus phase-1 item. Mirrors Transitus OQ-13. |
| TRN-Q-2 | Which bank activity format (BAI2 or camt.053) and which custodian format come first? | Determines the Transitus phase-1 format scope (Transitus OQ-06, OQ-07). |
| TRN-Q-3 | What are the retention periods for raw source files, lineage and evidence per data class? | Transitus OQ-10 |

## Proposed changes to Echelon.Execution (for its owners; not applied)

For Transitus to be its second consumer, Echelon.Execution needs:

- **G1:** lease renewal for long handlers;
- **G2:** automatic dependency resume (EAR-CMD-010);
- **G3:** payload protection and retention purge (EAR-CMD-022), which UNB-AVL-009 also requires;
- **G4:** metrics (EAR-OBS-002).

There is also a reported limitation: the queue-limit check runs before duplicate lookup, so an idempotent retry from a sponsor that is at its limit is throttled.

## Threat-model additions

Transitus threat model TRN-SEC-01 covers malicious files, archive bombs, XXE, formula injection, path traversal, replay and duplicate financial processing for ingestion. Unbound-specific consequences, such as a duplicated ACH return reversing a contribution twice, are mitigated by UNB-TRN-003 and UNB-TRN-004.

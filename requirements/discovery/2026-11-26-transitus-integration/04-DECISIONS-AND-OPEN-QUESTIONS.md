# 04 — Decisions and open questions (Transitus integration)

## Proposed decisions (for owner ratification; not added to `owner-decisions.jsonl`)

| ID | Proposal | Basis |
|---|---|---|
| TRN-D-1 | Unbound runs its release-1 feeds and the canonical historical import as Transitus phase-1 declarative linear pipelines, with basic packages for IRS, NACHA, the first bank format, Schwab and the canonical history. These run on Transitus's Formats, Checks and Evidence libraries and Echelon.Execution. Domain operations, tolerances and the MigrationRun remain Unbound's. | Transitus DF-TRN-2026-0018/0019 (owner-directed, superseding DF-TRN-2026-0016) |
| TRN-D-2 | Echelon.Execution stays incubated in `execution/` during Transitus phase 1. Extraction to a shared component is proposed only when Transitus trigger T2 fires, after the RM-05 benchmark. | Transitus DF-TRN-2026-0006; ADR-EAR-004 ("extraction when a second consumer starts") |
| TRN-D-3 | The IRS and OFAC parsers in `Unbound.Replay/Sources.fs` move to `Transitus.Formats`; the replay tool consumes them. | TRN-REQ-ARC-006 |

## Open questions for the Unbound owner

| ID | Question | Notes |
|---|---|---|
| TRN-Q-1 | Is donor and fund history migration (CNV-EXP-002) in release 1? | **Resolved by the owner (2026-11-26): yes.** Canonical historical import and migration rehearsal are required in release 1 (UNB-TRN-006, -011, -012). OD-M-14 (no design partner) still means there is no real sponsor export to rehearse against. Rehearsal uses synthetic canonical fixtures until one exists. |
| TRN-Q-2 | Which bank activity format and which custodian format come first? | **Owner direction:** the bank format comes from the first bank partner, and Schwab's documented format is the custodian priority. Still open until the partner is chosen and Schwab's documentation is obtained under acceptable terms. |
| TRN-Q-3 | What are the retention periods for raw source files, lineage and evidence per data class? | **Owner direction:** retention comes from approved classification and sponsor policy, with no invented binding durations. Open until those policies are approved. |

## Proposed changes to Echelon.Execution (for its owners; not applied)

Owner direction (OQ-01): extraction is deferred until the shared contract is stable, and Transitus phase 1 uses the module in place.


For Transitus to be its second consumer, Echelon.Execution needs:

- **G1:** lease renewal for long handlers;
- **G2:** automatic dependency resume (EAR-CMD-010);
- **G3:** payload protection and retention purge (EAR-CMD-022), which UNB-AVL-009 also requires;
- **G4:** metrics (EAR-OBS-002).

There is also a reported limitation: the queue-limit check runs before duplicate lookup, so an idempotent retry from a sponsor that is at its limit is throttled.

## Threat-model additions

Transitus threat model TRN-SEC-01 covers malicious files, archive bombs, XXE, formula injection, path traversal, replay and duplicate financial processing for ingestion. Unbound-specific consequences, such as a duplicated ACH return reversing a contribution twice, are mitigated by UNB-TRN-003 and UNB-TRN-004.

# 05 — Financial import and reconciliation requirements

Status: **proposed**. Requirements UNB-ING-FIN-001..010, UNB-ING-RCN-001..004, UNB-ING-DAF-001..011.

## 1. The rule that keeps files out of the ledger

Files never write authoritative financial tables. A command-batch record becomes the same Echelon.Execution operation an
interactive client submits, re-authorized and re-validated at execution inside the command transaction (CNV-PLT-010).
An observation-set record becomes an append-only external observation (UNB-XIN-EVA-016) that can confirm or contradict
ledger facts but never creates them. This is enforced structurally: ingestion modules may not reference ledger-write
APIs (UNB-ING-BND-002, static architecture rule).

## 2. Two-level acceptance

Existing text appeared to conflict: UNB-XIN-EVA-004 accepts a batch only when control totals match; UNB-XIN-EVA-011 makes
each item its own transaction. Both hold under a two-level rule (UNB-ING-FIN-001):

1. **File gate** — structure, schema version, control totals (record count, sums, debit/credit totals, opening/closing
   continuity), expected business date, tenant mapping. Any failure rejects the whole Run. No record is staged as valid.
2. **Record evaluation** — each record gets its own outcome and, if valid, its own command or observation.

## 3. Atomic versus independent

| Atomicity | Used for | Reason |
|---|---|---|
| all-or-nothing | Bank and custodian observation sets; reference datasets; migration waves; opening balances; manual journal batches | The set is only meaningful complete (a statement with a missing line misstates cash; half a journal does not balance). Command batches of this kind execute as **one atomic batch command** so a record declined at execution rolls back the whole batch (UNB-ING-FIN-011) |
| record-independent | Bulk and recurring grant recommendations; allocation changes | Each row is a separate donor request; failing 199 good grants because of one bad EIN helps no one |
| hold-on-threshold | Payroll/matching allocations; partner remittances | Independent rows, but a high error share signals a wrong file; hold for a person |

## 4. Identity, duplicates and idempotency

| Level | Identity | Effect |
|---|---|---|
| Artifact | SHA-256; source file id; source business date + sequence | A renamed re-delivery is detected; no second Run |
| Record | Profile-declared natural key within source and tenant, plus a canonical payload hash | Same key and hash: *Duplicate*, no effect. Same key, different hash: *KeyConflict* with a DuplicateKeyConflict case (UNB-XIN-EVA-005), never silently ignored. The key feeds a principal-independent business idempotency key (`ingest:{tenant}:{source}:{purpose}:{key}`), so a replay by an operator cannot double-post (UNB-ING-FIN-009) |
| Near-duplicate | Declared fields without shared key (same fund, EIN, amount, date) | Held for a decision; never dropped |

## 5. Money, dates and periods

Exact decimals at declared scale, rejected (not rounded) when over-precise; USD only unless declared. Tax-effective and
accounting dates (CNV-FIN-003) come only from instrument evidence referenced by observation id — the bank credit's value
date or the custodian's settlement date — never from dates claimed in a file, receipt time or execution time after a
hold (UNB-ING-FIN-006, revised after review R1-06). Closed periods follow existing late-entry rules and late observations
open current-period reconciliation items (UNB-ING-FIN-007).

## 6. Staging, dry run and approval

Every command-batch Run produces a validated preview with the same code as submission. Principal uploads require the
requester to confirm it. **Files never need approval merely for being files**: approval comes only from the target
command's own policy (e.g. dual control for opening balances and manual journals). This avoids adding a manual
approval to every bulk grant file — the grants' existing policy already decides which need review.

## 7. Corrections and replay

Before submission a corrected artifact supersedes the Run. After submission corrections are compensating commands or
observations linked to the original (ACC-MDL-002). Replay is a new Run under a stated profile version; idempotency
keys resolve previously effected records as duplicates; replaying a Completed run needs dual control.

## 8. Never silently discard

For every Run, `effected + declined + invalid + duplicate + excluded + unknown = input records` is checked
automatically (UNB-ING-FIN-003). Invalid rows are returned with row, field and reason, and the requester can download
only the rows that need fixing.

## 9. Reconciliation

| Scope | Sources | Rule |
|---|---|---|
| Bank account × business date | BAI2 or camt.053 prior-day (governs), camt.052 intraday where delivered (superseded), lockbox sets, payment-partner settlement | A receipt that would credit a fund auto-matches only when reference, amount and sender are all exact (RR-UNB-GAP-013-r1); everything else is a suggestion while the receipt waits in Unidentified suspense. Tolerance and many-to-one rules only confirm existing commands (disbursements, settlements). Every match records its rule version |
| Custody account × as-of date | Manager/custodian positions, transactions, cash, prices | Daily unitization check; missing price blocks valuation (CNV-FIN-005) |
| Contribution intent | Intent reference carried in wire memo, DTC note, check memo, intermediary confirmation | Unmatched receipts enter suspense with ageing (RR-UNB-GAP-013-r1) |
| Migration wave | Predecessor trial balance and fund balances | Ties to the cent before CutoverAuthorized (RR-REQ-X-061-r1) |

Breaks are exception cases with business-calendar ageing, owner role and suggested cause; AI may suggest candidates for
unmatched items but a match needs a human decision or a deterministic rule (UNB-ING-RCN-002). Write-offs are compensating
commands with dual control above a sponsor threshold.

## 10. Preventing exceptions instead of processing them faster

The industry evidence shows the dominant contribution exception is attribution, not parsing: gifts land in a pooled
sponsor account and the fund key travels as free text (FIE-A-131, FIE-A-106, FIE-A-123). UNB-ING-DAF-001 therefore
issues a unique reference with every contribution intent and puts it into the instructions the donor's institution
sends. The economic analysis in daf-landscape *assumes* (it cannot yet measure) intrinsic investigation rates and
minutes per investigation; under those assumptions, preventing investigations moves cost more than raising routine
straight-through processing, and the investigation rate is the parameter to measure first (UNB-ING-OPS-002).

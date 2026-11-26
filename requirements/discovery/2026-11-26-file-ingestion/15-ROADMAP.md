# 15 — Dependency-ordered implementation roadmap

Status: **proposed sequencing only**; no implementation is authorized by this mission. Each step names its entry
dependency and exit evidence.

| # | Step | Depends on | Exit evidence | Requirements |
|---|---|---|---|---|
| 0 | Owner decisions FIL-OD-01 (cloud) and FIL-OD-02 (object storage) | — | Recorded decisions | — |
| 1 | Artifact model, Expectation/Artifact/Run/Retention machines, slot-first storage adapter (with local test implementation), reconciler | 0; Echelon.Execution | Lifecycle property tests; TC-INT-*, TC-STO-* | LCY-*, STO-*, BND-004/005/006 |
| 1a | Local developer path: file-system storage adapter, scanner stub, process sandbox, corpus runner | 1 | Corpus runs offline (TC-TST-001) | BND-009, TST-001 |
| 2 | Supervisor and per-artifact sandbox: structure verification, limits, managed malware verdicts (polled, version-bound), fail-closed release, safe derivatives, download origin, network policy on links | 1, FIL-OD-13 | TC-SEC-*, TC-SIZ-*, TC-MAL-*; egress denial and forged-output tests | SEC-*, SEC-017 |
| 3 | Profile schema, catalogs (transforms, rules, holds), profile lifecycle, generated AI-readable documentation | 1 | Profile validator tests; documentation endpoint | PRF-*, FMT-002 |
| 4 | Reference datasets: IRS BMF, Pub 78, auto-revocation, OFAC SLS, California registry (no partner contract needed — earliest real data) | 2, 3 | Snapshot diff and re-screening tests; expectations live | DAF-003/004/005, OPS-001 |
| 5 | Command batches: bulk and recurring grant recommendations with preview, affirmation and per-row outcomes | 2, 3 | TC-VAL-001, TC-PAR-*, TC-DUP-*, TC-FIN-001; accessibility test | FIN-001..006, FIN-009/010, UI-001 |
| 6 | Evidence documents: upload, review UI, retention and legal hold (AI off) | 2 | TC-SEC-007/008; RET tests | RET-*, UI-003, DAF-009 |
| 7 | Canonical bank-transaction model + launch bank adapter (BAI2 or camt.053), lockbox, ACH returns; reconciliation | 2, 3, FIL-OD-03 (external) | Shadow month-end reconciled (ProvisionalCalibration) | RCN-*, DAF-001/002, DLV-002 |
| 8 | Manager/custodian adapter (positions, transactions, cash, prices) | 7; manager contract | Shadow month-end; unitization reconciled | DAF-011, ONB-002 |
| 9 | Migration waves, crosswalk, dry-run report, opening balances | 5, 7 | Migration rehearsal tied to the cent | DAF-007/008, FIN-002/008 |
| 10 | Operations dashboards and exception queue (measure STP and cost) | 4–9 | Dashboard definitions match test data | OPS-002/003, UI-002 |
| 11 | AI assistance for onboarding and migration mapping (configuration-time only) | 3, 9 | Mapping proposals reviewed field by field | AI-004 |
| 11a | AI proposals for selected evidence documents, behind provider policy, minimisation, evaluation sets and kill criteria (only if FIL-OD-04 approves) | 6, FIL-OD-04 | Evaluation results published; TC-AI-* | AI-001..003, AI-005..007 |
| 12 | Partner onboarding workspace with AI-assisted mapping | 3, 8 | Second institution onboarded via reuse | ONB-001, AI-004 |

**Implement first:** steps 1–4. They are needed by every later step, need no partner contract, and step 4 replaces the
most common charity document (the determination letter) with authoritative public data.

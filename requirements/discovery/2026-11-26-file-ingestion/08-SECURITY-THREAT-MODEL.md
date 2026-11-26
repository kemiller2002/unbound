# 08 — Security threat model for ingestion

Status: **proposed**. Method: threat → invariant → control → evidence (Tutela). Requirements UNB-ING-SEC-*, STO-*, AI-*.
Every file is untrusted, including files from authenticated banks, custodians and government sites.

## 1. Assets and trust boundaries

Assets: donor and fund personal data; financial instructions and observations; tenant separation; the ledger's
integrity; sponsor reputation. Boundaries: (1) the Internet → intake slot; (2) partner network → pull adapter;
(3) quarantine bytes → inspector role (no egress); (4) inspector → staging rows; (5) staged records → command intake
(re-authorized); (6) documents → AI provider (only under policy); (7) records container → download origin.

## 2. Threats and controls

| # | Threat | Invariant | Controls (requirements) | Evidence / test |
|---|---|---|---|---|
| T1 | Malware delivered as a document | No unscanned or malicious byte is readable by the application or users | Managed/licensed engine on exact bytes; fail closed; quarantine container unreadable by app roles; re-scan and revocation (SEC-005, SEC-006, SEC-013, STO-003) | TC-SEC-001, TC-SEC-010 |
| T2 | Type spoofing and polyglots | Type is what the strict parser proves | Declared type ∧ signature ∧ successful parse (SEC-002) | TC-SEC-002, TC-SEC-003 |
| T3 | Resource exhaustion (zip bombs, pixel floods, billion laughs, giant CSVs) | Bounded work per artifact | Streaming limits on bytes, records, depth, pixels, frames, expansion ratio, entries, CPU, memory, time (SEC-003, SEC-008) | TC-SIZ-*, TC-SEC-005 |
| T4 | Parser exploitation (image codecs, PDF, XML) | A compromised parser reaches nothing of value and cannot forge records | Per-artifact sandbox with no network, database or cloud credentials; supervisor validates output and recomputes totals (SEC-004); XXE disabled (SEC-008) | TC-SEC-004, TC-SEC-012, TC-SEC-015 |
| T5 | Active content reaching a reviewer's browser | Reviewers see rasters; originals only as attachments from a separate origin | Safe derivatives; strict download headers; CSP sandbox (SEC-010, SEC-014) | TC-SEC-007 |
| T6 | Formula injection into staff spreadsheets | Exported text cells cannot execute | Neutralisation on every export; no formula evaluation on import (SEC-009) | TC-SEC-006 |
| T7 | Path traversal / filename injection | Names never become paths or keys | Generated keys; sanitised display names (SEC-011) | TC-SEC-009 |
| T8 | Cross-tenant contamination (multiplexed institution files, URL replay, storage listing) | One artifact, one tenant; every access authorized | Adapter splits by authenticated account map before artifact creation; whole-run rejection on foreign accounts; prefix policies + application authorization (SEC-016, STO-004, SEC-014) | TC-TEN-001..004 |
| T9 | Bypassing business controls with a file | A file has no more authority than its submitter | Same operations as UI; re-authorization at execution; no ledger writes from ingestion (BND-002) | TC-INV-003, TC-INV-004 |
| T10 | Replay and duplicate posting | Each record has at most one financial effect | Artifact and record identity; idempotency keys; dual control for replay of completed runs (FIN-004, FIN-009) | TC-DUP-*, TC-REC-001 |
| T11 | Forged institution file | Only authenticated, signed (where offered) sources create artifacts | Pinned hosts, mTLS/OAuth, PGP verification, registered account map (DLV-002) | TC-SEC-013 |
| T12 | Tampering with stored bytes | Stored bytes equal recorded hash | Hash verification on every read; write-once records container (SEC-012, STO-003) | TC-SEC-011 |
| T13 | Prompt injection via documents | Document text never becomes instructions or actions | Tool-less, single-document, schema-constrained extraction; injection flag; corroboration; no authority (AI-001, AI-002, AI-006) | TC-AI-001..004 |
| T14 | Disclosure to AI providers | Documents leave only under an approved policy | Per-sponsor provider policy; off by default (AI-003) | TC-AI-006 |
| T15 | Business email compromise / payment-destination fraud via documents | A document alone never changes a destination | Bank letters are evidence for CNV-PAY-002 cases; call-back or account validation required (DAF-006, AI-006) | TC-AI-004 |
| T16 | Bulk exfiltration through downloads | Unusual volume is stopped and seen | Role, rate and volume limits; step-up; alert (SEC-015) | Security test |
| T17 | Scanner outage used as a bypass | Outage delays, never releases | Fail closed; exception after expectation (SEC-006) | TC-SEC-010 |
| T18 | Retention/legal-hold failure (premature deletion or over-retention) | Disposal only after period, never under hold | Storage-enforced write-once retention and legal hold; reviewed disposal batches; tombstones (RET-001, RET-002) | Integration test |
| T19 | Insider misuse of security override | Nobody can release bytes without a clean verdict | No override transition exists; reinstatement after a false positive requires a second managed engine's clean verdict on the exact hash (lifecycle revision 1) | TC-SEC-014 |
| T21 | Hidden text layer steering extraction | What the model reads is what the reviewer sees | Extraction on the reviewer's raster; deterministic raster/text disagreement flag (AI-001) | TC-AI-002 |
| T22 | Network-policy bypass with storage URLs | Restricted tenants' bytes are reachable only from permitted networks | Slot and link issuance evaluated against network policy; bytes through the application or address-bound URLs (SEC-017) | TC-SEC-018 |
| T23 | Replay or principal change double-posting | Each record has one effect regardless of who replays | Principal-independent business idempotency key; KeyConflict outcome (FIN-009, FIN-004) | TC-REC-001, TC-FIN-003 |
| T20 | Schema drift silently corrupting data | Unknown structure holds, never coerces | Drift detection; refuse-run default (ONB-002) | TC-VER-002, TC-UNX-001 |

## 3. Residual risks

- Native storage scanning has a higher false-negative rate than endpoint scanning (no behavioural context, FIE-C-010);
  safe derivatives and download headers are the second layer.
- On GCP no native scanner exists; ClamAV detection is materially weaker (~60%, FIE-C-012, inferred) — part of
  FIL-OD-01.
- Institution-specific file specifications are contract-only; drift detection reduces but cannot remove the risk of
  semantically wrong but structurally valid files. Reconciliation to an independent source is the final control.
- The isolation primitive is cloud-dependent (FIL-OD-13); its egress denial must be tested, not assumed.

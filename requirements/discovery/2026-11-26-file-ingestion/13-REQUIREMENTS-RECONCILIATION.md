# 13 — Requirements reconciliation

Status: **proposed**. Source: [`data/reconciliation.csv`](data/reconciliation.csv); topic inventory and evidence from a
read-only review of `requirements/baseline/baseline.jsonl` (1,653 rows), the question register (316 questions, all
resolved before this mission), all discovery missions and `architecture/`.

## 1. Classification of findings

The 52 ingestion topics were classified before any requirement was written; the counts in §5 are generated from
`primary_classification`, which takes the weakest class a topic carries (review R2-27 found the earlier hand-written
headline could not be derived from the data). Text-level conflicts and redundancies were resolved by precedence:
convergence clauses govern (`AUTHORITATIVE-BASELINE.md`). Refinements whose parents are proposed availability-mission
requirements (UNB-AVL-*, not in the baseline) take effect only if that mission is adopted (review R2-19).

## 2. Conflicts and how this mission takes a side

| Conflict | Side taken |
|---|---|
| Object store assumed by REQ-X-034, UNB-TEN-009, UNB-XIN-SAS-009 vs CNV-ARC-001 / UNB-XIN-EVA-013 / UNB-AVL-011 | ADR-ING-002: object storage for non-authoritative bytes only, with measurements; owner decision FIL-OD-02 |
| 7-year retention texts vs CNV-PLT-004 10-year R1 floor | CNV-PLT-004 governs (UNB-ING-RET-001) |
| Complex/crypto assets held and valued (UNB-GAP-011 etc.) vs CNV-GRT-013 intermediary | CNV-GRT-013 governs (UNB-ING-DAF-010; FIW-CON-14 deferred) |
| Independent price vendor (UNB-NAV-013) vs CNV-FIN-005 | CNV-FIN-005 governs (no licensed pricing feed; format decision) |
| All-state registries (UNB-CHAR-010, UNB-EXT-002) vs CNV-GRT-015 | California only (UNB-ING-DAF-005; FIW-CHV-13) |
| Batch completeness (UNB-XIN-EVA-004) vs independent items (UNB-XIN-EVA-011) | Two-level rule (UNB-ING-FIN-001) |
| ADM-01 attachment retention "TBD; 7-year" vs CNV-PLT-004 | 10-year floor for financial evidence (UNB-ING-RET-001) |

## 3. What this mission did **not** change

No approved decision, convergence clause, release-scope assignment or requirement text was edited. New requirements are
`proposed`; adoption into the baseline is a separate step (the baseline generator's source list, like the question
tooling's, is hard-coded, which is also why the availability mission's UNB-AVL rows are not in the baseline).

## 4. Questions

Twenty-two questions were added through the append-only process (events QE-001537..QE-001580): 14 decisions
(FIL-OD-01..14) and 8 requirement questions. After the adversarial review, events QE-001581..QE-001585 reopened
FIL-OD-05 and FIL-OD-13 (and their requirement questions) for the owner and corrected FIL-OD-10's answer. Eight remain
resolved on evidence under delegated authority (agent-recommended); fourteen are proposed for owner judgment or external
authorization — see
[14](14-QUESTIONS-AND-OWNER-DECISIONS.md). One pre-existing gap was observed and not fixed here: the availability
mission's questions (U-10..U-12, AVL-Q1, AVL-Q2) are still unregistered because that mission is not in the tooling's
source list.

## 5. Topic register



<!-- BEGIN GENERATED: reconciliation -->
Primary classification takes the weakest class a topic carries (conflicting, then missing, insufficient, deferred, covered); for example "covered (data) / missing (files)" counts as missing.

| Primary classification before this mission | Topics |
|---|---|
| already-covered | 9 |
| conflicting | 1 |
| covered-but-insufficient | 26 |
| deferred | 4 |
| missing | 12 |

| Status after this mission | Topics |
|---|---|
| addressed by proposed requirements (pending adoption) | 40 |
| deferred (unchanged); ingestion rules apply when activated | 2 |
| unchanged (covered) | 6 |
| unchanged (deferred) | 4 |

| # | Topic | Inventory classification | Existing (governing first) | Resolution in this mission | Status |
|---|---|---|---|---|---|
| 1 | Generic upload channel (portal/API) for donors, charities, staff | missing | UX-S10, UX-S09, UNB-EXT-007, UNB-CHAR-014, UNB-XIN-CLM-003, ADM-015, UNB-AVL-002/003 | UNB-ING-BND-001;UNB-ING-DLV-001;UNB-ING-STO-002;UNB-ING-UI-001 | addressed by proposed requirements (pending adoption) |
| 2 | Bulk grants / CSV / spreadsheet import | missing | ADM-032 (capability row: dry-run, exception isolation), INT-014 (preview population, partial failure), UNB-XIN-EVA-011 (batch = independent items), UNB-GRANT-029, UNB-RCN-GRT-010 | UNB-ING-FIN-005;UNB-ING-FIN-010;UNB-ING-FMT-002;UNB-ING-BND-002 | addressed by proposed requirements (pending adoption) |
| 3 | Inbound document storage (blob/object store) | missing / conflicting | UNB-TEN-009, UNB-XIN-SAS-009, REQ-X-034, ARCH-KNOW-RAWEVIDENCE, ARCH-TAPD-OBJECTSTORE vs CNV-ARC-001, UNB-XIN-EVA-013, UNB-AVL-011 | UNB-ING-STO-001;UNB-ING-STO-002;UNB-ING-STO-003;ADR-ING-002;FIL-OD-02 | addressed by proposed requirements (pending adoption) |
| 4 | Issued (outbound) document registry and Document Center | covered | REQ-X-034, REQ-X-035, CNV-EXP-001, RCD-009, UNB-DOC-001..005, UNB-RCN-PLT-010 |  | unchanged (covered) |
| 5 | Retention of ingested files | insufficient (+ redundant / superseded text) | CNV-PLT-004 (R1 10-year floor, cut L), RR-REQ-X-036-r1, REQ-X-036/037/038, ADM-01, UNB-AVL-009, U-12 | UNB-ING-RET-001;FIL-OD-05 | addressed by proposed requirements (pending adoption) |
| 6 | Legal hold | covered (insufficient for blobs) | CNV-PLT-004, UNB-XIN-REC-001, UNB-XIN-REC-002, REQ-X-038, UNB-GAP-019 | UNB-ING-RET-002 | addressed by proposed requirements (pending adoption) |
| 7 | Attachments (notes, messages, cases, complaints) | insufficient | REQ-X-024, REQ-X-026, ADM-01, UNB-XIN-SVC-002, UX-S10 | UNB-ING-SEC-005;UNB-ING-RET-001;UNB-ING-SEC-014 | addressed by proposed requirements (pending adoption) |
| 8 | Evidence storage / provenance | insufficient | UNB-XIN-EVA-016 (external_observation table), CNV-FIN-013 (raw feed preserved), UNB-CHAR-008, UNB-EXT-020, UNB-CUST-004/013, ARCH-KNOW-RAWEVIDENCE, MON-003 | UNB-ING-SEC-012;UNB-ING-STO-001;UNB-ING-AI-002 | addressed by proposed requirements (pending adoption) |
| 9 | Malware / content scanning | missing | SOC2-016 (generic security testing only) | UNB-ING-SEC-005;UNB-ING-SEC-006;UNB-ING-SEC-013;FIL-OD-01 | addressed by proposed requirements (pending adoption) |
| 10 | Quarantine of data and records | covered (data) / missing (files) | UNB-EXT-032, UNB-KNOW-026, UNB-AVL-007, RR-UNB-GAP-013-r1, UNB-XIN-PAY-007, UNB-CUST-022, RR-UNB-CHAR-013-r1 | UNB-ING-LCY-001;UNB-ING-STO-003 | addressed by proposed requirements (pending adoption) |
| 11 | OCR / AI document extraction | insufficient | CNV-AIO-001, CNV-AIO-002, UNB-XIN-SVC-009, UNB-CHAR-022, UNB-KNOW-030, AIN-005, RR-AIN-027-r1, RR-UNB-GAP-013-r1 | UNB-ING-AI-002;UNB-ING-AI-003;UNB-ING-AI-005;UNB-ING-UI-003 | addressed by proposed requirements (pending adoption) |
| 12 | Prompt injection from documents | insufficient | AIN-012, CNV-AIO-002, UNB-AIO-003, UNB-GAP-033, RR-AIN-027-r1, UNB-DOC-005, UX-S11 | UNB-ING-AI-001;UNB-ING-AI-006 | addressed by proposed requirements (pending adoption) |
| 13 | Bank statements (BAI2/BTRS, camt.052/053/054, bank API) | insufficient (decisions made) | CNV-PAY-007, XID-BNK-007, UNB-XIN-BNK-003, UNB-XIN-EVA-004, UNB-XIN-EVA-005, OPC-009, CNV-EVB-001 | UNB-ING-RCN-001;UNB-ING-FIN-004;UNB-ING-DLV-002;FIL-OD-03 | addressed by proposed requirements (pending adoption) |
| 14 | NACHA / ACH returns / NOCs | covered | CNV-PAY-005, CNV-PAY-002, UNB-XIN-EVA-005, UNB-XIN-EVA-005#Q1, UNB-XIN-PAY-005, UNB-RCN-WF-001 |  | unchanged (covered) |
| 15 | Lockbox / check images / remote deposit capture | missing | UNB-XIN-BNK-005 (lockbox as a cost driver only), CNV-PAY-005 (R1 contributions arrive by check), UNB-RCN-FIN-012 | UNB-ING-DAF-002;FIL-OD-03 | addressed by proposed requirements (pending adoption) |
| 16 | Inbound wires (and ACH credits) | insufficient | CNV-PAY-005, UNB-RCN-FIN-012, RR-UNB-GAP-013-r1, UNB-RCN-WF-002, XID-EVA-003 | UNB-ING-DAF-001;UNB-ING-RCN-001 | addressed by proposed requirements (pending adoption) |
| 17 | Securities transfers / DTC | covered (insufficient for donor documents) | UNB-XIN-PAY-007, CNV-FIN-013, UNB-RCN-WF-003, RR-UNB-ACC-031-r1, UNB-XIN-PAY-007#Q1, OPC-002 | UNB-ING-DAF-001 | addressed by proposed requirements (pending adoption) |
| 18 | Custodian feeds / holdings / transactions | insufficient | CNV-LEG-001, OD-M-07, CNV-FIN-013, UNB-CUST-001/003/004/005/009/013/022/024, UNB-INV-032, UNB-NAV-007 | UNB-ING-DAF-011;UNB-ING-ONB-002;UNB-ING-DLV-002 | addressed by proposed requirements (pending adoption) |
| 19 | Pricing / NAV inputs | covered (decision) | CNV-FIN-005, RR-UNB-NAV-008-r1, UNB-NAV-013 (narrowed), UNB-ACC-016 |  | unchanged (covered) |
| 20 | Corporate actions | deferred / insufficient | UNB-XIN-INV-003 (Deferred), UNB-CUST-016, UNB-NAV-007, UNB-ACC-009 |  | unchanged (deferred) |
| 21 | Payroll giving / matching gifts / employer files | deferred | CNV-ECO-004, REQ-X-070 (title-only placeholder), OD-M-02, CNV-FIN-012, XID-ERP-003, UNB-RCN-ENT-011, UNB-RCN-FIN-013 |  | unchanged (deferred) |
| 22 | Noncash assets / appraisals / Form 8283 | covered (decision); insufficient (documents) | CNV-GRT-013, OD-M-09, RCD-034, RR-UNB-INV-012-r1, UNB-RCN-FIN-002/003, UNB-RCN-WF-005, UNB-GAP-012, CNV-FIN-010 | UNB-ING-DAF-010 | addressed by proposed requirements (pending adoption) |
| 23 | Crypto | deferred (via intermediary only) | CNV-GRT-013, XID-ADV-001, UNB-XIN-INV-007 (Deferred), UNB-WF-004, UNB-RCN-FIN-011 |  | unchanged (deferred) |
| 24 | IRS EO BMF / Pub 78 / auto-revocation | insufficient (best-covered source) | CNV-GRT-003, OPC-011, UNB-CHAR-010/011, UNB-EXT-001/017/019/020/032, UNB-KNOW-023/026/027, RR-REQ-X-007-r1, UNB-RCN-GRT-014, ARCH-KNOW-PIPELINE | UNB-ING-DAF-003 | addressed by proposed requirements (pending adoption) |
| 25 | OFAC / sanctions lists | insufficient | CNV-GRT-006, RR-UNB-EXT-004-r1, UNB-RCN-GRT-007, UNB-EXT-004, UNB-XIN-EVA-004 | UNB-ING-DAF-004 | addressed by proposed requirements (pending adoption) |
| 26 | State charity registries | deferred except California | CNV-GRT-015, SPD-GRT-005, UNB-EXT-002, CNV-EVB-001 | UNB-ING-DAF-005 | deferred (unchanged); ingestion rules apply when activated |
| 27 | Charity bank verification / payment enrolment | covered (rules); insufficient (documents) | CNV-PAY-002, UNB-RCN-GRT-017, CNV-EXP-002, RR-UNB-GRANT-026-r1, UNB-RCN-WF-028, UNB-CHAR-029 | UNB-ING-DAF-006 | addressed by proposed requirements (pending adoption) |
| 28 | Returned and failed payments | covered | CNV-PAY-004, CNV-PAY-005, UNB-RCN-GRT-008/015, RR-UNB-PAY-006-r1, RR-REQ-X-004-r1, UNB-XIN-OTH-004 (returned mail) |  | unchanged (covered) |
| 29 | Migration / conversion / historical data | insufficient | CNV-EXP-002, RR-REQ-X-061-r1, UNB-RCN-WF-020, UNB-REM-008, UNB-CHAR-041, UNB-CUST-023, RR-UNB-ACC-033-r1, XID-OTH-001, UNB-SMP-ECN-002; deferred: UNB-XIN-OTH-002, UNB-XIN-EVA-009 | UNB-ING-DAF-007 | addressed by proposed requirements (pending adoption) |
| 30 | Opening balances | insufficient | UNB-RCN-WF-009 ("tied-out opening balances"), CNV-EXP-002 | UNB-ING-DAF-008 | addressed by proposed requirements (pending adoption) |
| 31 | Sponsor onboarding documents | missing | UNB-RCN-WF-009, UNB-GAP-025, RR-UNB-TEN-011-r1 (Deferred), CNV-PLT-004 (organizing documents permanent), CNV-GOV-004, CNV-GOV-008 | UNB-ING-BND-003 (evidence-document profiles); workflow matrix FIW-SPO-* | addressed by proposed requirements (pending adoption) |
| 32 | Advisor onboarding / Form ADV / firm feeds | missing | UNB-XIN-CRM-007 (HR/SCIM firm feed), UNB-XIN-CRM-002, RR-UNB-ADV-001-r1 | workflow matrix FIW-ADV-*; UNB-ING-DAF-012 | addressed by proposed requirements (pending adoption) |
| 33 | Donor identity documents / KYC | insufficient | CNV-PLT-003, RCD-023 (no CIP), SPD-CXP-002, RR-UNB-PART-007-r1, UNB-XIN-CRM-001, UNB-EXT-007 | workflow matrix FIW-DON-02 (eliminated: no ID images) | addressed by proposed requirements (pending adoption) |
| 34 | Trusts / entities / signatories | deferred (trusts) / insufficient (entities) | CNV-PLT-003 ("entities verified by registration and signatory authority", R1), UNB-XIN-TRS-003 (Deferred), REQ-X-074 (placeholder) | UNB-ING-DAF-009 | deferred (unchanged); ingestion rules apply when activated |
| 35 | POA / death certificates / successors | insufficient | CNV-ENT-001, RR-UNB-SUCC-008-r1, UNB-XIN-TRS-004, UNB-XIN-CLM-006, XID-TRS-001, UNB-SUCC-007, UNB-RCN-ENT-004 | UNB-ING-DAF-009 | addressed by proposed requirements (pending adoption) |
| 36 | Correspondence (email / mail / fax inbound) | insufficient | UNB-XIN-SVC-001 (Contact record), CNV-PLT-001 (R1 email/portal outbound), CNV-PLT-002, UNB-XIN-OTH-004 | UNB-ING-AI-006;UNB-ING-DLV-001;FIL-OD-07;FIL-OD-12 | addressed by proposed requirements (pending adoption) |
| 37 | Complaints / disputes | covered (process); missing (attachments) | CNV-PLT-005, UNB-XIN-SVC-005, CNV-GRT-012, UNB-RCN-ENT-016 | UNB-ING-SEC-014 | addressed by proposed requirements (pending adoption) |
| 38 | Vendor SOC reports / bridge letters / audit evidence | covered (decision: annual manual task in R1) | CNV-GOV-002 (cut J), RR-REQ-X-073-r1/r2, CNV-GOV-001, CNV-PLT-009, SOC2-013, UNB-RCN-PLT-013 |  | unchanged (covered) |
| 39 | Reconciliation | covered | CNV-FIN-004 (ProvisionalCalibration for new sources), UNB-XIN-BNK-003, UNB-GAP-001/004, CNV-GOV-003 | UNB-ING-RCN-003 | addressed by proposed requirements (pending adoption) |
| 40 | Control totals / completeness | covered (feeds); missing (uploads) | UNB-XIN-EVA-004, MON-004, UNB-KNOW-026, ARCH-KNOW-ACQRUN | UNB-ING-FIN-001 | addressed by proposed requirements (pending adoption) |
| 41 | Idempotency / duplicates | covered (records); missing (file-level) | UNB-XIN-EVA-005, UNB-KNOW-027, RR-ACC-MDL-007-r1, UNB-GRANT-029, UNB-DATA-009, CNV-PAY-005 | UNB-ING-FIN-004 | addressed by proposed requirements (pending adoption) |
| 42 | Dry-run / staging / preview | insufficient | ADM-032, INT-014, RR-REQ-X-061-r1 (ImportedToStaging), UNB-EXT-037, CNV-FIN-004, UNB-XIN-SAS-010 (sandbox, Deferred) | UNB-ING-FIN-005 | addressed by proposed requirements (pending adoption) |
| 43 | Partial vs atomic processing | insufficient | UNB-XIN-EVA-011 (items independent), UNB-XIN-EVA-004 (batch complete only on control totals), INT-014, CNV-FIN-001 (atomic reservation) | UNB-ING-FIN-001;UNB-ING-FIN-002 | addressed by proposed requirements (pending adoption) |
| 44 | Replay / reprocessing | covered | UNB-DATA-009, UNB-IMM-006, AIN-020, UNB-AVL-004, UNB-KNOW-027, UNB-RES-002 | UNB-ING-FIN-009 | addressed by proposed requirements (pending adoption) |
| 45 | SFTP / MFT / file transport | missing | UNB-CUST-003/009, UNB-API-STD-005, UNB-EVT-003, CNV-ECO-005 (integration register) | UNB-ING-DLV-001;UNB-ING-DLV-002;FIL-OD-06 | addressed by proposed requirements (pending adoption) |
| 46 | Webhooks (inbound) | insufficient | UNB-API-006, UNB-API-STD-005, UNB-CUST-009, UNB-XIN-EVA-005, OPC-009 | UNB-ING-DLV-003 | addressed by proposed requirements (pending adoption) |
| 47 | AI governance | covered | CNV-AIO-001, CNV-AIO-002, CNV-GOV-010, RR-AIN-027-r1, RR-AIN-003-r1, UNB-RCN-PLT-004 |  | unchanged (covered) |
| 48 | Durable queue / PostgreSQL command processing | covered | CNV-ARC-001, ADR-EVA-001, UNB-AVL-001..019, ADR-AVL-001 (proposed), Echelon.Execution | UNB-ING-BND-006 (OPC-035..046) | addressed by proposed requirements (pending adoption) |
| 49 | Tenant isolation | covered (insufficient for blobs) | CNV-ARC-002, CNV-TEN-001, UNB-TEN-009, UNB-TEN-014, UNB-XIN-SAS-005, UNB-DATA-011 | UNB-ING-SEC-016;UNB-ING-STO-004 | addressed by proposed requirements (pending adoption) |
| 50 | Encryption | insufficient | SOC2-014, UNB-AVL-009, UNB-TEN-006/008, RR-UNB-TEN-016-r1 (tenant key destruction), TAPD-ADR-006 (open), UNB-DATA-007 | UNB-ING-STO-003;UNB-ING-STO-004;FIL-OD-09 | addressed by proposed requirements (pending adoption) |
| 51 | Download / view authorization | covered (issued documents); missing (uploaded and raw files) | REQ-X-035, UNB-DOC-004, UNB-XIN-IAM-003 (export step-up), UNB-XIN-REC-002, UNB-XIN-IAM-002, CNV-ARC-002 | UNB-ING-SEC-014;UNB-ING-SEC-015 | addressed by proposed requirements (pending adoption) |
| 52 | Cards / instant rails | deferred | CNV-PAY-008, CNV-PAY-006, XID-BNK-004, UNB-XIN-BNK-008, UNB-XIN-PAY-003/004/006 |  | unchanged (deferred) |
<!-- END GENERATED: reconciliation -->

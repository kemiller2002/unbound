# 01 — Workflow coverage matrix

Status: **proposed**. Source data: [`data/workflow-matrix.json`](data/workflow-matrix.json) (all mission §7 fields for every
workflow) and [`data/workflow-matrix.csv`](data/workflow-matrix.csv). The industry view of the same workflows, without
Unbound requirements, is in daf-landscape
(`data/file-ingestion-2026-11/workflow-inventory.csv`, research package `research/file-ingestion-and-straight-through-processing/`).

**How to read it.**
- *Class* is the artifact class (UNB-ING-BND-003). It decides the controls: command batches become the same commands as the
  interface; observation sets are external facts used for matching; reference datasets are whole snapshots; evidence
  documents attach to a case; configuration goes through configuration-change commands.
- *Disposition*: `keep` (ingest as is), `redesign` (ingest, but change what is asked for or how), `eliminate-file`
  (replace with structured capture or an authoritative source, no upload path), `challenge` (only if a sponsor policy
  requires it), `defer`, `merge` (covered by another workflow).
- *Priority* applies the governing clauses (e.g. CNV-ECO-004 defers workplace giving; CNV-GRT-013 routes complex assets
  through an intermediary; RCD-023 means no customer identification program, so no identity-document images).
- *Confidence* is `confirmed` only where public evidence (FIE-* ids in daf-landscape) documents the practice;
  otherwise `inferred`.
- *Existing requirements* (JSON field `existing_unbound_requirements`) are a topic-level mapping from the reconciliation
  inventory, not proof of coverage; [13](13-REQUIREMENTS-RECONCILIATION.md) classifies coverage.

The matrix stopped growing when further sources produced no materially distinct workflow: the last three research passes
(custodians, community-foundation software, corporate giving) added evidence to existing rows but no new rows except
the California registry, which a governing clause required.

<!-- BEGIN GENERATED: summary -->
| Domain | Workflows | R1 | R1-conditional | R2 | Deferred | Eliminated |
|---|---|---|---|---|---|---|
| sponsor-onboarding | 8 | 7 | 1 | 0 | 0 | 0 |
| advisor-administration | 8 | 4 | 1 | 2 | 1 | 0 |
| donor-administration | 10 | 5 | 2 | 0 | 2 | 1 |
| contributions | 16 | 7 | 4 | 1 | 4 | 0 |
| grants | 8 | 4 | 2 | 2 | 0 | 0 |
| charity-vetting | 13 | 8 | 1 | 2 | 1 | 1 |
| investments | 11 | 6 | 2 | 0 | 1 | 2 |
| accounting-treasury | 11 | 5 | 3 | 2 | 1 | 0 |
| compliance | 9 | 4 | 1 | 3 | 1 | 0 |
| migration | 11 | 8 | 2 | 1 | 0 | 0 |
| customer-service | 6 | 2 | 1 | 1 | 2 | 0 |
| platform | 4 | 1 | 1 | 2 | 0 | 0 |
| total | 115 | 61 | 21 | 16 | 13 | 4 |

| Disposition | Workflows |
|---|---|
| challenge | 4 |
| defer | 9 |
| eliminate-file | 15 |
| keep | 59 |
| merge | 2 |
| redesign | 26 |

| Artifact class | Workflows |
|---|---|
| command-batch | 23 |
| configuration | 6 |
| evidence-document | 47 |
| observation-set | 30 |
| reference-dataset | 9 |
<!-- END GENERATED: summary -->

<!-- BEGIN GENERATED: matrix -->
| ID | Workflow | Class | Delivery | Format | Atomicity | Automation | AI | Disposition | Priority | Confidence | New requirements |
|---|---|---|---|---|---|---|---|---|---|---|---|
| FIW-SPO-01 | Sponsor services agreement and order form | evidence-document | e-signature envelope (preferred); PDF upload | PDF | all-or-nothing | high | low (clause summary for review only) | redesign | R1 | inferred |  |
| FIW-SPO-02 | Board resolutions and delegation-of-authority documents | evidence-document | PDF upload | PDF | all-or-nothing | medium | medium (propose authority matrix from resolution text; human confirms) | redesign | R1 | inferred |  |
| FIW-SPO-03 | Operating policies (grant, gift-acceptance, investment, fee, privacy) | evidence-document | PDF/DOCX upload (DOCX converted to PDF before upload) | PDF | all-or-nothing | medium | high (clause-to-parameter proposals with citations) | redesign | R1 | inferred |  |
| FIW-SPO-04 | Fee schedules (administrative, advisor, pool expense) | configuration | admin UI entry (preferred); CSV template | CSV | all-or-nothing | high | none | eliminate-file | R1 | inferred | UNB-ING-DAF-012 |
| FIW-SPO-05 | Branding assets (logo, colours, typography) | configuration | browser upload | PNG/JPEG (SVG refused: active content) | all-or-nothing | high | none | keep | R1 | inferred |  |
| FIW-SPO-06 | Investment policy statement and pool line-up | evidence-document | PDF upload + pool configuration | PDF | all-or-nothing | medium | low | redesign | R1 | inferred |  |
| FIW-SPO-07 | Sponsor's own compliance documents (IRS determination, 990, state registrations, audited statements) | evidence-document | PDF upload; retrieval from IRS/state registries where available | PDF | all-or-nothing | high | medium (summaries for diligence) | redesign | R1 | inferred |  |
| FIW-SPO-08 | Existing operational configuration from incumbent platform (fund types, grant rules, fee tiers, user roles) | command-batch | CSV/XLSX export upload | CSV, XLSX | record-independent | medium | high (schema discovery and mapping proposals) | keep | R1-conditional | inferred | UNB-ING-AI-004 |
| FIW-ADV-01 | Advisor roster (firm, advisors, CRD numbers, contact) | command-batch | CSV upload or API; SCIM for user provisioning | CSV, JSON | record-independent | high | none | keep | R1 | inferred |  |
| FIW-ADV-02 | Client-to-advisor assignments (household/fund relationships) | command-batch | CSV upload or API | CSV, JSON | record-independent | high | none | redesign | R1 | inferred |  |
| FIW-ADV-03 | Form ADV / CRD registration evidence | reference-dataset | scheduled retrieval / lookup | XML/JSON public data | all-or-nothing | high | none | eliminate-file | R2 | inferred |  |
| FIW-ADV-04 | Advisor insurance certificates (E&O) | evidence-document | PDF upload | PDF | all-or-nothing | medium | low (expiry date proposal) | challenge | Deferred | inferred |  |
| FIW-ADV-05 | Advisor tax documentation (W-9) for advisor fee payment | evidence-document | PDF upload or structured W-9 form | PDF / form | all-or-nothing | high | medium (extract TIN/name proposals) | redesign | R1-conditional | inferred |  |
| FIW-ADV-06 | Delegated authority / limited power of attorney between donor and advisor | evidence-document | e-signature in-app (preferred); PDF upload | PDF | all-or-nothing | high | medium (scope proposals) | redesign | R1 | inferred |  |
| FIW-ADV-07 | Advisor fee schedules for advisor-managed funds | configuration | admin UI; CSV template | CSV | all-or-nothing | high | none | eliminate-file | R2 | inferred |  |
| FIW-ADV-08 | Advisor termination and client reassignment | command-batch | CSV upload or API | CSV, JSON | record-independent | high | none | keep | R1 | inferred |  |
| FIW-DON-01 | Account applications (paper or PDF legacy) | evidence-document | mail, email, PDF upload (legacy); digital onboarding preferred | PDF, image | all-or-nothing | high | high (pre-fill from scanned forms) | redesign | R1 | confirmed |  |
| FIW-DON-02 | Identity documentation (driver licence, passport) | evidence-document | upload to ID-verification provider | image | all-or-nothing | medium | medium (provider does it) | eliminate-file | Eliminated | inferred |  |
| FIW-DON-03 | Trust documentation (certification of trust, trust agreement excerpts) | evidence-document | PDF upload | PDF | all-or-nothing | medium | high (trustee/power extraction with citations) | defer | Deferred | inferred |  |
| FIW-DON-04 | Entity formation documents (corporation, LLC, foundation donors) | evidence-document | PDF upload | PDF | all-or-nothing | medium | medium | keep | R1-conditional | inferred | UNB-ING-DAF-009 |
| FIW-DON-05 | Powers of attorney | evidence-document | PDF upload | PDF | all-or-nothing | low | medium | keep | R1-conditional | inferred | UNB-ING-DAF-009 |
| FIW-DON-06 | Successor and beneficiary designations | command-batch | in-app form (preferred); PDF form legacy | form / PDF | record-independent | high | medium (legacy forms) | redesign | R1 | confirmed | UNB-ING-DAF-009 |
| FIW-DON-07 | Death certificates and succession evidence | evidence-document | PDF/image upload; mail | PDF, image | all-or-nothing | medium | medium (field proposals + match to donor record) | keep | R1 | inferred | UNB-ING-AI-001; UNB-ING-UI-001; UNB-ING-UI-003; UNB-ING-DAF-009 |
| FIW-DON-08 | Account amendments (name, address, fund name, joint holders) | command-batch | in-app; supporting document when legal name changes | form / PDF | record-independent | high | low | eliminate-file | R1 | inferred |  |
| FIW-DON-09 | Signed fund agreements and disclosures | evidence-document | in-app click-through or e-signature | e-sign record / PDF | all-or-nothing | high | none | eliminate-file | R1 | inferred | UNB-ING-DAF-012 |
| FIW-DON-10 | Historical account statements from a prior sponsor (incoming DAF-to-DAF transfer) | evidence-document | PDF upload | PDF | all-or-nothing | medium | low | challenge | Deferred | inferred |  |
| FIW-CON-01 | Bank lockbox data file and check images | observation-set | SFTP or bank API (bank-proprietary layout) | CSV/fixed-width + TIFF/JPEG images (or X9.100-187 via bank) | all-or-nothing | high | medium (remittance-letter classification; memo-line donor/fund proposals) | redesign | R1 | confirmed | UNB-ING-FIN-001; UNB-ING-RCN-002; UNB-ING-RCN-003; UNB-ING-DLV-002; UNB-ING-ONB-002; UNB-ING-DAF-002 |
| FIW-CON-02 | Remote deposit capture by sponsor staff | observation-set | bank RDC scanner/portal (bank system) | X9.100-187 via bank | all-or-nothing | medium | low | challenge | Deferred | inferred |  |
| FIW-CON-03 | Incoming wire and ACH credit details (bank-reported remittance) | observation-set | bank reporting (BAI2/camt.053/camt.054) or bank API | BAI2, ISO 20022 camt | all-or-nothing | high | low (fuzzy-match suggestions for orphans) | redesign | R1 | confirmed | UNB-ING-FIN-001; UNB-ING-RCN-001; UNB-ING-RCN-002; UNB-ING-DAF-001 |
| FIW-CON-04 | Securities received at sponsor's custodian (DTC deliveries) | observation-set | custodian data feed or API; custodian portal export | custodian-proprietary CSV/fixed-width | all-or-nothing | high | low (orphan matching suggestions) | redesign | R1 | confirmed | UNB-ING-RCN-002; UNB-ING-DAF-001 |
| FIW-CON-05 | Incoming gift-of-securities notification forms | evidence-document | PDF form by email/fax; online form | PDF / web form | all-or-nothing | high | none | eliminate-file | R1 | confirmed | UNB-ING-DAF-001; UNB-ING-DAF-012 |
| FIW-CON-06 | Donor's letter of authorization / transfer instruction to delivering broker | evidence-document | generated by sponsor; signed by donor; sent to donor's broker | PDF | all-or-nothing | high | none | eliminate-file | R1 | confirmed | UNB-ING-DAF-001; UNB-ING-DAF-012 |
| FIW-CON-07 | Payroll-deduction contribution allocation files | command-batch | SFTP or API | CSV | hold-on-threshold | high | none | defer | Deferred | confirmed | UNB-ING-FIN-001; UNB-ING-FIN-008 |
| FIW-CON-08 | Employer matching-gift files | command-batch | SFTP, API or portal export | CSV | hold-on-threshold | high | none | defer | Deferred | confirmed |  |
| FIW-CON-09 | Contribution allocation schedules (one gift split across several funds) | command-batch | in-app split (preferred); CSV | form / CSV | record-independent | high | none | eliminate-file | R1 | inferred |  |
| FIW-CON-10 | Noncash contribution documentation (private stock, LP/LLC interests, real estate) | evidence-document | PDF upload to case | PDF, XLSX (cap tables) | all-or-nothing | medium | high (dossier assembly, missing-document detection) | redesign | R1-conditional | confirmed | UNB-ING-DAF-010 |
| FIW-CON-11 | Qualified appraisals | evidence-document | PDF upload | PDF | all-or-nothing | medium | high (field proposals with citations) | keep | R1-conditional | confirmed | UNB-ING-AI-001; UNB-ING-UI-001; UNB-ING-UI-003; UNB-ING-DAF-010 |
| FIW-CON-12 | IRS Form 8283 (donee acknowledgment, Part V signature) | evidence-document | PDF upload; sponsor signs and returns | PDF | all-or-nothing | high | medium (compare donor-prepared fields) | redesign | R1-conditional | confirmed | UNB-ING-DAF-010 |
| FIW-CON-13 | Asset ownership records (certificates, operating agreements, title, transfer restrictions) | evidence-document | PDF upload | PDF | all-or-nothing | low | medium (restriction clause detection) | redesign | R1-conditional | inferred | UNB-ING-DAF-010 |
| FIW-CON-14 | Cryptocurrency contribution evidence (processor reports, on-chain transaction ids) | observation-set | processor API or report | CSV/JSON | all-or-nothing | high | none | defer | Deferred | confirmed |  |
| FIW-CON-15 | Incoming DAF-to-DAF grants and third-party platform remittances (with grant letters) | observation-set | check, ACH or wire + PDF letter or remittance file | bank reporting + PDF/CSV | all-or-nothing | high | medium (letter classification, fund matching) | redesign | R1 | confirmed | UNB-ING-DAF-001 |
| FIW-CON-16 | Estate, bequest and beneficiary-designation distributions | evidence-document | check/wire + documents | PDF | all-or-nothing | low | medium | keep | R2 | inferred |  |
| FIW-GRT-01 | Bulk grant recommendations | command-batch | CSV upload or API | CSV, JSON | record-independent | high | none | keep | R1 | confirmed | UNB-ING-BND-002; UNB-ING-SEC-009; UNB-ING-FIN-002; UNB-ING-FIN-005; UNB-ING-FIN-010; UNB-ING-FMT-002; UNB-ING-UI-001 |
| FIW-GRT-02 | Recurring grant schedules (bulk setup or migration) | command-batch | in-app; CSV for bulk | CSV | record-independent | high | none | keep | R1 | confirmed | UNB-ING-BND-002; UNB-ING-FIN-005; UNB-ING-FIN-010; UNB-ING-FMT-002 |
| FIW-GRT-03 | Grant agreements signed by recipient (restricted, large or expenditure-responsibility grants) | evidence-document | e-signature (preferred); PDF upload | PDF | all-or-nothing | high | medium (clause differences vs template) | redesign | R1-conditional | inferred |  |
| FIW-GRT-04 | Returned, voided and uncashed grant payments | observation-set | bank reporting, ACH returns, check-issue reconciliation, mailed returns | NACHA return entries, BAI2/camt, PDF letters | all-or-nothing | high | low (return-letter classification) | redesign | R1 | confirmed |  |
| FIW-GRT-05 | Grant expenditure and use reports (ER grants, foreign grants) | evidence-document | portal upload | PDF, XLSX | all-or-nothing | medium | high (completeness check, summary) | keep | R2 | inferred |  |
| FIW-GRT-06 | Charity correspondence (declines, refunds, acknowledgments, address changes) | evidence-document | email/mail via mailroom; charity portal | PDF, email | all-or-nothing | high | high (classification, routing, summarization) | redesign | R2 | inferred | UNB-ING-AI-006 |
| FIW-GRT-07 | Grant payment confirmations from payment partner (check vendor, ACH processor, grant-payment network) | observation-set | API/webhook + settlement report | JSON, CSV | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-DLV-003; UNB-ING-DLV-004 |
| FIW-GRT-08 | Paid-check and positive-pay exception files | observation-set | bank portal/API/SFTP | bank-proprietary CSV/BAI2 | all-or-nothing | high | none | keep | R1-conditional | confirmed |  |
| FIW-CHV-01 | IRS Exempt Organizations Business Master File | reference-dataset | scheduled HTTPS download | CSV (by region/state) | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-OPS-001; UNB-ING-ONB-002; UNB-ING-DAF-003 |
| FIW-CHV-02 | IRS Publication 78 data (deductibility) | reference-dataset | scheduled HTTPS download | pipe-delimited text in ZIP | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-DAF-003 |
| FIW-CHV-03 | IRS Automatic Revocation of Exemption list | reference-dataset | scheduled HTTPS download | pipe-delimited text in ZIP | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-DAF-003 |
| FIW-CHV-04 | IRS Form 990-series e-file data and 990-N (e-Postcard) | reference-dataset | scheduled HTTPS download | XML (990), pipe-delimited (990-N) | all-or-nothing | high | medium (risk signal summarization) | keep | R2 | confirmed |  |
| FIW-CHV-05 | IRS determination letters (exception path only) | evidence-document | charity portal upload | PDF | all-or-nothing | high | medium (EIN/date/subsection extraction + BMF corroboration) | eliminate-file | R1 | confirmed | UNB-ING-AI-001; UNB-ING-UI-003 |
| FIW-CHV-06 | Charity W-9 / tax documentation | evidence-document | PDF upload | PDF | all-or-nothing | high | none | eliminate-file | Eliminated | inferred | UNB-ING-DAF-012 |
| FIW-CHV-07 | Fiscal sponsorship agreements | evidence-document | PDF upload | PDF | all-or-nothing | medium | high (clause presence check) | keep | R1-conditional | inferred |  |
| FIW-CHV-08 | Charity banking verification (bank letters, voided checks) | evidence-document | PDF upload | PDF, image | all-or-nothing | high | medium (routing/account proposals, corroborated by validation) | redesign | R1 | confirmed | UNB-ING-AI-006; UNB-ING-DAF-006 |
| FIW-CHV-09 | Charity electronic-payment enrollment forms | command-batch | charity portal form (preferred); PDF | form / PDF | record-independent | high | none | eliminate-file | R1 | confirmed | UNB-ING-DAF-006 |
| FIW-CHV-10 | Evidence for organizations not in IRS data (churches, governmental units, group-exemption subordinates) | evidence-document | portal upload | PDF | all-or-nothing | medium | high (evidence summary) | keep | R1 | inferred | UNB-ING-UI-003 |
| FIW-CHV-11 | Foreign grantee equivalency/ER documentation | evidence-document | portal upload; service report | PDF | all-or-nothing | low | high (translation, summary) | keep | R2 | inferred |  |
| FIW-CHV-13 | California Attorney General Registry of Charities and Fundraisers and FTB exemption status | reference-dataset | scheduled HTTPS download / search | CSV (registry list); status lookup | all-or-nothing | high | none | keep | R1 | inferred | UNB-ING-DAF-005 |
| FIW-CHV-12 | Licensed charity data (e.g., Candid) and other states' charity registry data | reference-dataset | API / download | JSON/CSV | all-or-nothing | high | low | defer | Deferred | inferred |  |
| FIW-INV-01 | Custodian positions/holdings (pool and advisor-managed accounts) | observation-set | custodian data feed/API/SFTP | custodian-proprietary CSV/fixed-width/JSON | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-FIN-001; UNB-ING-FIN-007; UNB-ING-RCN-003; UNB-ING-RCN-004; UNB-ING-DLV-002; UNB-ING-OPS-001; UNB-ING-ONB-002; UNB-ING-DAF-011 |
| FIW-INV-02 | Custodian cash balances | observation-set | feed/API | custodian-proprietary | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-DAF-011 |
| FIW-INV-03 | Investment transactions (trades, settlements, transfers, income) | observation-set | feed/API | custodian-proprietary | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-FIN-007; UNB-ING-DAF-011 |
| FIW-INV-04 | Prices and NAVs | observation-set | feed/API | CSV/JSON | all-or-nothing | high | none | redesign | R1 | inferred | UNB-ING-DAF-011 |
| FIW-INV-05 | Trade confirmations | observation-set | feed/PDF | CSV/PDF | all-or-nothing | high | none | eliminate-file | Eliminated | inferred | UNB-ING-DAF-012 |
| FIW-INV-06 | Corporate actions | observation-set | feed/API | custodian-proprietary (ISO 15022/20022 upstream) | all-or-nothing | high | none | defer | Deferred | inferred |  |
| FIW-INV-07 | Dividend and interest records | observation-set | feed/API (part of transactions) | custodian-proprietary | all-or-nothing | high | none | merge | R1 | inferred |  |
| FIW-INV-08 | Investment manager fee invoices | command-batch | PDF/email or API | PDF, CSV | hold-on-threshold | high | medium (invoice extraction) | keep | R1-conditional | inferred |  |
| FIW-INV-09 | Donor cost basis information for contributed securities | evidence-document | PDF/CSV | PDF/CSV | all-or-nothing | high | none | eliminate-file | Eliminated | inferred | UNB-ING-DAF-012 |
| FIW-INV-10 | Investment allocation instructions (bulk reallocation by advisors) | command-batch | in-app; CSV; API | CSV, JSON | record-independent | high | none | keep | R1 | inferred | UNB-ING-BND-002; UNB-ING-FIN-005; UNB-ING-FMT-002 |
| FIW-INV-11 | Fund administrator NAV/unitization and expense allocations (outsourced pools) | observation-set | SFTP/API | CSV | all-or-nothing | high | none | keep | R1-conditional | inferred |  |
| FIW-ACT-01 | Bank account reporting (prior-day and intraday balances and transactions) | observation-set | bank API, SFTP pull, host-to-host | BAI2, ISO 20022 camt.052/053/054, bank JSON | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-FIN-001; UNB-ING-FIN-007; UNB-ING-RCN-001; UNB-ING-RCN-002; UNB-ING-RCN-003; UNB-ING-RCN-004; UNB-ING-DLV-002; UNB-ING-OPS-001; UNB-ING-ONB-002 |
| FIW-ACT-02 | Payment status acknowledgments (ACH origination acceptance, payment status reports) | observation-set | bank API/SFTP | ISO 20022 pain.002, bank-proprietary ACK, JSON | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-DLV-003 |
| FIW-ACT-03 | ACH returns and notifications of change (NOC) | observation-set | bank API/SFTP/report | NACHA return/COR entries or bank report | all-or-nothing | high | none | keep | R1 | confirmed |  |
| FIW-ACT-04 | Payment-partner and processor settlement reports | observation-set | API/SFTP | CSV/JSON | all-or-nothing | high | none | keep | R1 | inferred | UNB-ING-RCN-004; UNB-ING-DLV-004 |
| FIW-ACT-05 | Vendor invoices (accounts payable) | evidence-document | email/portal/PDF | PDF | all-or-nothing | high | high (invoice extraction; benchmarks exist) | keep | R2 | inferred |  |
| FIW-ACT-06 | Manual journal imports (adjustments from accountants) | command-batch | CSV upload | CSV | all-or-nothing | high | none | keep | R1 | inferred | UNB-ING-BND-002; UNB-ING-FIN-002; UNB-ING-FIN-008; UNB-ING-FMT-002 |
| FIW-ACT-07 | External general-ledger trial balances (sponsor's corporate GL, outsourced accounting) | observation-set | CSV upload/API | CSV | all-or-nothing | high | none | keep | R1-conditional | inferred |  |
| FIW-ACT-08 | Audited financial statements and auditor deliverables | evidence-document | PDF upload | PDF | all-or-nothing | low | low | keep | R2 | inferred |  |
| FIW-ACT-09 | Interfund / intercompany and pool-level reconciliation data | observation-set | CSV/API | CSV | all-or-nothing | high | none | keep | R1-conditional | inferred |  |
| FIW-ACT-10 | Fee and expense allocations from third parties | command-batch | CSV/API | CSV | hold-on-threshold | high | none | redesign | R1-conditional | inferred |  |
| FIW-ACT-11 | Bank fee analysis statements | observation-set | bank portal/EDI 822 | EDI 822 / PDF | all-or-nothing | medium | none | defer | Deferred | inferred |  |
| FIW-CMP-01 | OFAC SDN and Consolidated Sanctions lists | reference-dataset | scheduled HTTPS download (Sanctions List Service) | XML, CSV | all-or-nothing | high | none | keep | R1 | confirmed | UNB-ING-OPS-001; UNB-ING-DAF-004 |
| FIW-CMP-02 | Other screening lists (BIS, State) via Consolidated list | reference-dataset | scheduled download | CSV/XML/JSON | all-or-nothing | high | none | merge | R1-conditional | inferred |  |
| FIW-CMP-03 | Regulatory correspondence (IRS notices, state attorney-general letters) | evidence-document | mail via mailroom; portal | PDF | all-or-nothing | medium | high (classification, deadline extraction) | keep | R2 | inferred |  |
| FIW-CMP-04 | External audit evidence requests (PBC lists) and auditor-provided files | evidence-document | portal upload | XLSX, PDF | all-or-nothing | high | medium (map request to existing evidence) | redesign | R2 | inferred |  |
| FIW-CMP-05 | Vendor SOC reports and bridge letters | evidence-document | portal upload / vendor trust centre | PDF | all-or-nothing | medium | high (CUEC and exception extraction) | keep | R1 | inferred |  |
| FIW-CMP-06 | Security assessment and penetration test reports | evidence-document | portal upload | PDF | all-or-nothing | medium | medium (finding extraction) | keep | R2 | inferred |  |
| FIW-CMP-07 | Grant certifications and compliance attestations from charities | command-batch | in-app attestation | form | record-independent | high | none | eliminate-file | R1 | inferred |  |
| FIW-CMP-08 | IRS TIN matching results | observation-set | bulk TIN matching file / interactive | fixed-width text | all-or-nothing | high | none | defer | Deferred | inferred |  |
| FIW-CMP-09 | Subpoenas, legal process and preservation requests | evidence-document | mail/portal | PDF | all-or-nothing | low | medium (party/date extraction) | keep | R1 | inferred |  |
| FIW-MIG-01 | Customer master records (donors, funds, advisors, authorities) | command-batch | SFTP pull or upload (migration waves) | CSV, JSON | all-or-nothing | medium | high (schema discovery, mapping proposals, anomaly detection) | keep | R1 | confirmed | UNB-ING-BND-002; UNB-ING-SEC-009; UNB-ING-FIN-002; UNB-ING-FIN-005; UNB-ING-AI-004; UNB-ING-FMT-002; UNB-ING-DAF-007 |
| FIW-MIG-02 | Historical grants | observation-set | SFTP/upload | CSV | all-or-nothing | high | medium (mapping) | keep | R1 | confirmed | UNB-ING-DAF-007 |
| FIW-MIG-03 | Historical contributions (with acknowledgment data) | observation-set | SFTP/upload | CSV | all-or-nothing | high | medium (mapping) | keep | R1 | confirmed | UNB-ING-DAF-007 |
| FIW-MIG-04 | Opening balances (units, cash, cost) at cutover | command-batch | SFTP/upload | CSV | all-or-nothing | high | none | keep | R1 | inferred | UNB-ING-BND-002; UNB-ING-FIN-001; UNB-ING-FIN-002; UNB-ING-FIN-008; UNB-ING-DAF-007; UNB-ING-DAF-008 |
| FIW-MIG-05 | Pending transactions in flight at cutover | command-batch | SFTP/upload | CSV | record-independent | medium | low | keep | R1 | inferred |  |
| FIW-MIG-06 | Recurring instructions (recurring grants, contributions) | command-batch | SFTP/upload | CSV | record-independent | high | none | keep | R1 | inferred |  |
| FIW-MIG-07 | Successor and beneficiary information | command-batch | SFTP/upload | CSV | record-independent | high | none | keep | R1 | inferred |  |
| FIW-MIG-08 | Historical statements and documents (bulk PDFs) | observation-set | SFTP/bulk upload with index file | PDF + CSV index | all-or-nothing | high | low | keep | R1-conditional | confirmed | UNB-ING-DAF-007 |
| FIW-MIG-09 | Charity master data from predecessor | observation-set | SFTP/upload | CSV | all-or-nothing | high | medium (entity resolution) | keep | R1 | inferred | UNB-ING-DAF-007 |
| FIW-MIG-10 | Historical audit and approval records | observation-set | SFTP/upload | CSV/JSON | all-or-nothing | high | none | keep | R1-conditional | inferred |  |
| FIW-MIG-11 | Bulk inbound fund transfers from an exiting sponsor (sponsor exit / merger) | command-batch | SFTP/upload + funds | CSV | all-or-nothing | medium | medium | keep | R2 | inferred |  |
| FIW-SVC-01 | Donor and advisor secure messages with attachments | evidence-document | authenticated portal messaging | PDF, JPEG, PNG | all-or-nothing | high | high (classification, routing, drafting) | keep | R1 | inferred | UNB-ING-AI-001; UNB-ING-AI-006 |
| FIW-SVC-02 | Complaint documentation | evidence-document | portal; mail | PDF, email | all-or-nothing | medium | medium (classification, summary) | keep | R1 | inferred |  |
| FIW-SVC-03 | Dispute evidence (contribution disputes, chargebacks if cards enabled) | evidence-document | portal; bank dispute system | PDF | all-or-nothing | medium | low | keep | R2 | inferred |  |
| FIW-SVC-04 | Scanned physical mail (digital mailroom) | evidence-document | vendor API/SFTP: PDF + index metadata | PDF + CSV/JSON index | all-or-nothing | high | high (classification and routing) | redesign | R1-conditional | inferred | UNB-ING-AI-001; UNB-ING-AI-006 |
| FIW-SVC-05 | Fax-derived documents | evidence-document | fax-to-PDF via mailroom vendor | PDF/TIFF | all-or-nothing | medium | high | defer | Deferred | inferred |  |
| FIW-SVC-06 | Email to sponsor mailboxes | evidence-document | email | EML + attachments | all-or-nothing | medium | high (classification) | challenge | Deferred | inferred |  |
| FIW-PLT-01 | Partner sample files and integration documentation (onboarding) | evidence-document | secure upload to integration workspace | any allowed type | all-or-nothing | medium | high (format classification, mapping proposals) | keep | R1 | inferred | UNB-ING-AI-004; UNB-ING-ONB-001 |
| FIW-PLT-02 | Federated identity metadata (SAML metadata XML, OIDC discovery/JWKS) for partner SSO | configuration | admin upload or metadata URL retrieval | XML, JSON | all-or-nothing | high | none | keep | R1-conditional | inferred |  |
| FIW-PLT-03 | Network allowlist import (CIDR ranges for advisor/admin access) | configuration | admin UI; CSV | CSV | all-or-nothing | high | none | keep | R2 | inferred |  |
| FIW-PLT-04 | Sponsor staff user provisioning (bulk) | configuration | SCIM (preferred); CSV | JSON, CSV | all-or-nothing | high | none | redesign | R2 | inferred |  |
<!-- END GENERATED: matrix -->

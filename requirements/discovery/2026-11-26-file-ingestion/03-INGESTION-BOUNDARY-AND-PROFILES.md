# 03 — Common ingestion boundary and ingestion profile specification

Status: **proposed**. Requirements UNB-ING-BND-*, UNB-ING-PRF-*, UNB-ING-DLV-*, UNB-ING-FMT-*.

## 1. One boundary, many adapters

```
 browser upload ─┐
 API upload ─────┤                        ┌── Supervisor (Execution worker) ⇄ per-artifact sandbox (no network, no credentials) ──┐
 API records ────┤   ┌──────────────┐     │  structure + limits + malware verdict + safe derivatives      │
 scheduled pull ─┼──▶│ Intake slot  │──▶ quarantine object ──▶ parse + validate under profile vN ──▶ staged records
 SFTP pull ──────┤   │ (PostgreSQL) │     └────────────────────────────────────────────────────────────────┘
 webhook→fetch ──┤   └──────────────┘                                     │
 mailroom vendor ┘                                                         ▼
                               command batch ──▶ same Echelon.Execution operations as the UI (re-authorized at execution)
                               observation set ──▶ external_observation (append-only) ──▶ matching / reconciliation
                               reference dataset ──▶ versioned snapshot ──▶ diff ──▶ re-screening commands
                               evidence document ──▶ case attachment (no Run) ──▶ optional extraction proposals (never commands)
                               configuration ──▶ configuration-change commands (dual control)
```

Adapters only authenticate the source and move bytes into an intake slot. They never parse business content. That is
what keeps a grant recommendation in a file subject to exactly the controls of a grant recommendation typed into the
interface (UNB-ING-BND-001/002): the file produces `grant.recommend@1` commands, and the command handler — not the
ingestion code — decides eligibility, balance, policy and approval.

**Why not a separate ingestion service?** The component monolith already has a durable command engine
(Echelon.Execution) and PostgreSQL. Ingestion steps are durable commands keyed by artifact or run (rows OPC-035..046
below). The one new runtime element is honest to name: a **second deployment unit** of the same codebase — a sandbox
created per artifact that has no network, no database access and no cloud credentials. A trusted supervisor (an ordinary
Execution worker) fetches the bytes with a tenant- and object-scoped credential, hands them to the sandbox, validates
what comes back and recomputes control totals itself (UNB-ING-SEC-004, revised after review R1-08 and R2-01). It is not
a new codebase or a new service with its own API, but it is a new workload with its own isolation primitive, which is
why FIL-OD-13 is an owner decision.

**Artifact custody versus record processing** (revised after review R2-16). Every byte goes through custody: slot,
quarantine, inspection, storage, retention. Only command batches, observation sets and reference datasets go through a
Run under a profile; an evidence document attaches to its case as soon as it is released. Sources that arrive as
several files (IRS regional BMF files, lockbox data plus images, mailroom PDF plus index, migration file plus manifest)
are **artifact sets** with a manifest, a completeness rule and a set hash; one Run processes the complete set
(UNB-ING-BND-010).

## 2. Ingestion profiles

A profile is a declarative, versioned JSON document (schema: [`data/ingestion-profile.schema.json`](data/ingestion-profile.schema.json)).
Illustrative profiles, one per artifact class plus the hardest financial cases:

| Profile | Class | Atomicity | Why it is illustrative |
|---|---|---|---|
| [`grant.bulk-recommendation.csv`](data/profiles/grant.bulk-recommendation.csv.json) | command-batch | record-independent | Principal upload; preview + confirm; same command as UI |
| [`bank.account-reporting.prior-day`](data/profiles/bank.account-reporting.prior-day.json) | observation-set | all-or-nothing | Institutional file; expectation calendar; trailer control totals |
| [`charity.reference.irs-bmf`](data/profiles/charity.reference.irs-bmf.json) | reference-dataset | all-or-nothing | Government pull; drift hold; tenant-neutral |
| [`contribution.noncash.appraisal`](data/profiles/contribution.noncash.appraisal.json) | evidence-document | n/a | No AI in release 1 (review R2-17); deterministic consistency checks against the gift record |
| [`succession.death-certificate.document`](data/profiles/succession.death-certificate.document.json) | evidence-document | n/a | AI proposals only where enabled; SSN masked before disclosure; independent corroboration against the donor record |
| [`correspondence.scanned-mail.vendor`](data/profiles/correspondence.scanned-mail.vendor.json) | evidence-document | n/a | Deterministic routing of payment, authority and succession items before any AI classification |
| [`charity.determination-letter.document`](data/profiles/charity.determination-letter.document.json) | evidence-document | n/a | Exception path only; proposals must agree with IRS data |
| [`migration.opening-balances.csv`](data/profiles/migration.opening-balances.csv.json) | command-batch | all-or-nothing | Manifest control totals per field; dual approval; one atomic batch command (UNB-ING-FIN-011) |
| [`contribution.payroll-giving.csv`](data/profiles/contribution.payroll-giving.csv.json) | command-batch | hold-on-threshold | Control total = funds actually received; dependency on bank run (deferred workflow, shown for the pattern) |

### 2.1 What a profile may contain — and what it may not

| Profile element | Expressed as | Why not more powerful |
|---|---|---|
| Field mapping | source column / JSON pointer / function-free XPath / fixed position | Positions and names are data; no expressions |
| Normalisation | names from a closed transform catalog (`trim`, `strip-non-digits`, `implied-decimal-2`, `neutralize-formula-prefix`, …) | Each transform is a reviewed, unit-tested F# function |
| Validation | rule ids from the closed catalog [`data/catalogs.json`](data/catalogs.json) (`fund.exists-in-tenant`, `ein.checksum`, …), checked by gate I5 | Business rules stay in command handlers; the preview calls each handler's side-effect-free validate mode, so profiles never copy business rules |
| Holds | metric + comparison + value, with sponsor bounds | A threshold, not a program |
| Output | one operation name and version that already exists for interactive use | No ingestion-only operations (UNB-ING-BND-002) |
| Extraction | field names + corroboration rule ids + provider policy reference | AI output is a proposal, corroborated deterministically |

There is no scripting language, expression evaluator, regular expression supplied by a tenant, or template engine.
Research found no ingestion workflow in the matrix whose transformation could not be expressed with the closed
catalogs plus, for a genuinely new institutional format, a new reviewed parser in the platform release. Review R2-08
argued the opposite risk — profiles too rigid for real partner quirks — and the trade-off is accepted: a quirk costs a
release, not a configuration change ([16](16-ADVERSARIAL-REVIEW.md)). Adding a catalog entry, and every platform
profile version, is a control-relevant change under CNV-PLT-010.

### 2.2 What sponsors may configure

Only the attributes listed in a platform profile's `sponsor_configurable` (UNB-ING-PRF-003): field aliases from an
approved list, notification recipients, expectation windows and grace, hold-rule values within declared bounds, record
limits below the platform limit, longer retention, and activation. Each change is a dual-controlled configuration command
with preview. Sponsors do not author profiles (decision FIL-OD-14, owner decision pending; recommendation: no).

### 2.3 Profile lifecycle

`draft → testing (synthetic corpus) → active → retired`, with **calibration** for new sources and for versions that
change mapping or semantics: live processing at zero tolerance with 100% human review until a month-end reconciles
(CNV-FIN-004; UNB-ING-RCN-003). Tightening-only versions need only the corpus. Sponsor specialisations (`extends`) are
deferred until a second sponsor uses the same platform profile. A version is immutable after draft; each Run records
the profile version (UNB-ING-PRF-002, CNV-ARC-005).

### 2.4 AI-readable documentation

Every profile publishes generated, machine-readable documentation of its columns, types, examples, rule ids that can
reject a value and the authority required (UNB-ING-PRF-004, CNV-AIO-001). This is what lets an advisor's own assistant
prepare a valid bulk-grant file without trial and error.

## 3. Delivery mechanisms (release 1)

| Mechanism | Release 1 | Notes |
|---|---|---|
| Browser upload | Yes | Forma/Limen component; short-lived write-only URL bound to key, size range and checksum |
| API upload / API records | Yes | Same profiles; JSON Schema published |
| Scheduled HTTPS retrieval | Yes | IRS, OFAC, California registry; pinned hosts |
| Outbound SFTP / API pull | Yes | Banks, custodians, partners; pinned host keys; PGP where offered |
| Partner webhook → fetch | Yes (as hint) | Signature-verified; the file is fetched, the webhook body is never the artifact (UNB-ING-DLV-003) |
| Mailroom vendor (PDF + index) | Conditional | Only for a launch sponsor with material paper mail (FIL-OD-12) |
| Unbound-hosted inbound SFTP | No | Until a contracted partner can only push (FIL-OD-06, ADR-ING-004) |
| Email ingestion | No | Not authority-bearing (FIL-OD-07) |
| Fax | No | Via mailroom vendor if ever needed |

Direct APIs are preferred for event-shaped data (payment status, disbursement networks); files remain superior for
institutional end-of-day sets, government datasets and human-prepared bulk batches (UNB-ING-DLV-004).

## 4. Operation classification (extends the availability mission's matrix)

<!-- BEGIN GENERATED: operations -->
| ID | Operation | Execution class | Ordering | Idempotency | Authorization timing | Failure | User status | Requirements |
|---|---|---|---|---|---|---|---|---|
| OPC-035 | Create artifact intake slot (upload or pull) | ImmediateCommand | Artifact | Client idempotency key per upload; one slot per key | Intake: principal or source authorized for the profile purpose and tenant | Database unavailable -> refuse honestly, no URL issued | Uploading... | UNB-ING-BND-001; UNB-ING-STO-002; UNB-ING-BND-005 |
| OPC-036 | Commit received bytes (quarantine) | ImmediateCommand | Artifact | Artifact id; storage checksum must equal declared SHA-256 | Re-check slot owner and expiry | Checksum mismatch -> slot Abandoned, partial object deleted | Received - security check pending | UNB-ING-STO-002; UNB-ING-SEC-012 |
| OPC-037 | Inspect artifact (structure, limits, malware verdict, derivatives) | DurableCommand | Artifact | Artifact id + security policy version | None beyond tenant scope (system command on the inspector role) | Scanner unavailable -> stays Quarantined (fail closed); parser fault -> Rejected with evidence | Received - security check pending / Rejected (reason) / Accepted for processing | UNB-ING-SEC-002; UNB-ING-SEC-003; UNB-ING-SEC-004; UNB-ING-SEC-005; UNB-ING-SEC-006; UNB-ING-SEC-010 |
| OPC-038 | Parse and validate run (dry run) | DurableCommand | Run | Run id; record keys inside the run | Validation uses requester authority as of now; re-checked at execution | Structural failure -> Run Rejected; control-total mismatch -> Rejected | Checking N records... / N valid, M need attention | UNB-ING-FIN-001; UNB-ING-FIN-002; UNB-ING-FIN-003; UNB-ING-FIN-004; UNB-ING-FIN-005 |
| OPC-039 | Submit run records as business commands | DurableCommand | Run (submission); each command keeps its own ordering key (e.g. Fund) | Record identity -> command idempotency key | Each command re-authorizes at execution (CNV-PLT-010) | Partial submission crash -> resumes; idempotency prevents duplicates | Submitted N of M | UNB-ING-BND-002; UNB-ING-FIN-009; UNB-ING-FIN-010 |
| OPC-040 | Record observation set (bank, custodian, partner) | DurableCommand | Source account + business date | Source reference per observation | System (registered source) | Continuity break -> Run Rejected; expectation stays Lapsed | Not user-facing (operations) | UNB-ING-BND-003; UNB-ING-RCN-001; UNB-ING-DAF-011 |
| OPC-041 | Activate reference snapshot (IRS, OFAC, California) | DurableCommand | Dataset | Dataset + content hash | System | Drift hold -> prior snapshot stays active | Not user-facing (operations) | UNB-ING-DAF-003; UNB-ING-DAF-004; UNB-ING-DAF-005 |
| OPC-042 | Scheduled retrieval of dataset or partner file | ScheduledCommand + ExternalIntegrationOperation | Source | Source file id / content hash | System (credential from secret store) | Source unreachable -> retry; expectation may lapse | Not user-facing | UNB-ING-DLV-001; UNB-ING-DLV-002; UNB-ING-OPS-001 |
| OPC-043 | Release held run / reject run | HumanApprovalWorkflow | Run | Decision id | Decision right ingestion.run.release-hold; segregation from uploader where profile requires | Stale decision (run superseded) -> refused | Held - awaiting decision / Released / Rejected | UNB-ING-LCY-001; UNB-ING-UI-002 |
| OPC-044 | Approve run effects (migration, journals) | HumanApprovalWorkflow | Run | Approval id | Target command approval policy (dual control) | Second approver equals first -> refused | Awaiting approval (1 of 2) | UNB-ING-DAF-008; UNB-ING-FIN-002 |
| OPC-045 | Download artifact or derivative | ImmediateQuery | n/a | n/a | Per request: case/fund permission, step-up above bulk threshold | Unauthorized -> refused and audited | Download | UNB-ING-SEC-014; UNB-ING-SEC-015 |
| OPC-046 | Re-inspect released artifacts after policy or signature change | ScheduledCommand | Artifact | Artifact id + policy version | System | Malicious verdict -> Revoked + impact case | Document temporarily unavailable | UNB-ING-SEC-013 |
<!-- END GENERATED: operations -->


## 5. Format decisions

Research inventory with licensing and parser risk: daf-landscape `data/file-ingestion-2026-11/formats.csv`. Unbound's
decision, where it departs from the research recommendation, says so (source: [`data/format-decisions.csv`](data/format-decisions.csv)).

<!-- BEGIN GENERATED: formats -->
| Format | Unbound decision | Basis | Relation to research recommendation |
|---|---|---|---|
| CSV / delimited (RFC 4180) | initial | Unbound-published templates with requester-declared or trailer control totals (UNB-ING-FMT-001) | agree |
| Pipe-delimited ASCII (IRS TEOS) | initial | IRS datasets required by CNV-GRT-003 | agree |
| Fixed-width | conditional | only via reviewed layout definitions for a named partner | agree |
| JSON + JSON Schema | initial | API records and API batch envelopes | agree |
| XML + XSD | initial (named schemas only) | only for OFAC SLS and ISO 20022 camt; identity-provider metadata belongs to the identity component, not ingestion (review R2-21, R1-28); arbitrary XML uploads refused | narrowed: research said initial generically |
| ISO 20022 camt.053 / camt.052 / camt.054 | initial (camt.053 only, if the launch bank uses it instead of BAI2); camt.052/054 conditional | CNV-PAY-007 needs one prior-day format and one bank adapter; intraday and notification messages only if the launch bank delivers them (review R2-21) | raised from conditional: governed by CNV-PAY-007 |
| ISO 20022 pain.001 / pain.002 | conditional | pain.002 status ingestion only if the launch bank reports payment status that way; Unbound never executes payments (CNV-LEG-001) | agree |
| ISO 20022 pacs.008 / pacs.004 (Fedwire, FedNow, CHIPS) | not-applicable | bank customer, not participant | agree |
| NACHA ACH file | conditional | the sponsor originates ACH through its bank (CNV-LEG-001, CNV-PAY-001); ingest NACHA return/NOC entries only if the bank delivers them as NACHA files rather than in bank reporting or API | lowered from initial: Unbound does not originate files to the ACH operator |
| BAI2 | initial (one of BAI2 or camt.053 per launch bank) | CNV-PAY-007 | agree |
| SWIFT MT940 / MT942 | deferred | US domestic | agree |
| OFX / QFX | not-applicable | consumer formats | stronger than research (deferred) |
| X9.37 / ANSI X9.100-187 image cash letter | not-applicable | consume the bank's lockbox/RDC outputs | agree |
| Lockbox transmission (bank-proprietary, BAI lockbox) | initial | checks are an R1 contribution instrument (CNV-PAY-005); one bank lockbox layout via reviewed profile; delivered as an artifact set (data file plus images), images possibly as a bank ZIP or multi-page TIFF package under trusted-package caps (UNB-ING-BND-010) | raised from conditional: governed by CNV-PAY-005 |
| ANSI X12 823 Lockbox | deferred | licensing; only if the launch bank offers nothing else | agree |
| ANSI X12 820 Payment Order/Remittance | deferred | workplace giving deferred (CNV-ECO-004) | agree |
| XBRL | not-applicable |  | agree |
| IRS Form 990 MeF XML | R2 | grantee enrichment and supporting-organization signals | agree |
| XLSX (OOXML) | conditional | migration and onboarding configuration only, values-only reading; financial imports use CSV templates | lowered from initial: XLSX adds a ZIP container, formulas and external links for little benefit over CSV |
| PDF | initial | evidence documents with safe raster derivative | agree |
| TIFF | conditional (lockbox image packages from the registered bank) | only for lockbox images or mailroom vendors that deliver TIFF | agree |
| JPEG / PNG / HEIC | initial for JPEG/PNG; HEIC conditional | mobile browsers commonly transcode HEIC to JPEG on upload (inferred, needs device testing); add server-side HEIC normalisation only on measured need | narrowed |
| ZIP | initial (trusted government datasets only) | refused for user and partner uploads; single-level, entry and ratio caps for IRS archives | narrowed: research said initial generically |
| OFAC SLS (SDN/Consolidated: XML, Advanced XML, CSV, FF, delta) | initial (one representation) | CNV-GRT-006; use one SLS representation (SDN and consolidated lists in one chosen format, e.g. Advanced XML) plus its published deltas, not every variant (review R2-21) | narrowed |
| IRS EO BMF extract (CSV) | initial | CNV-GRT-003 | agree |
| State charity registry data (e.g., CA AG CSV) | initial (California only) | CNV-GRT-015; other states deferred | raised for California: governed by CNV-GRT-015 |
| Candid / GuideStar APIs (JSON) | deferred | licensing; IRS public data suffices for R1 | agree |
| Custodian data feeds (Schwab SDD, Fidelity Integration Xchange, Pershing/Wove) | initial (one manager/custodian adapter) | read-only ingest of the external investment manager's data (CNV-LEG-001, OD-M-07); contract-only specifications | agree with a scope limit |
| DTCC (DTC deliveries, DWAC, ACATS, NSCC) | not-applicable | receipts observed through custodian reporting | agree |
| FIX | not-applicable | Unbound does not route orders (CNV-LEG-001) | stronger than research (deferred) |
| Pricing data (ICE, Bloomberg) | not-applicable | prices from manager/custodian (CNV-FIN-005) | stronger than research (deferred) |
| Corporate actions (ISO 15022 MT564 / ISO 20022 seev) | not-applicable |  | agree |
| ACORD (insurance) / MISMO (mortgage) | not-applicable |  | agree |
<!-- END GENERATED: formats -->

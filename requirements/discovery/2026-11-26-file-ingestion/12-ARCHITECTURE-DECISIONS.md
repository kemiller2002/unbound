# 12 — Architecture decision records (ADR-ING)

Status of every ADR: **proposed**; none authorizes implementation. Format follows ADR-AVL-001.

## ADR-ING-001 — One ingestion boundary producing the same commands as the interface

- **Context.** Inbound information arrives by browser, API, bank and custodian pulls, government downloads and
  (later) mailroom vendors. Channel-specific code would drift into channel-specific rules, and a file could become a
  privileged path around controls (mission §9, §14).
- **Decision (revised after review R2-16).** Two parts. (1) *Artifact custody* — slot, quarantine, inspection,
  storage, retention — is mandatory for every byte from every channel. (2) *Record processing* — a Run under a profile —
  applies only to command batches, observation sets and reference datasets; evidence documents attach to their case
  when released. Command batches produce the existing Echelon.Execution operations at the interactive contract version,
  re-authorized at execution (UNB-ING-BND-001..003).
- **Consequences.** No ingestion-only business operations; ingestion code cannot reference ledger writes (static
  rule); one test corpus covers every channel. Cost: an operation must exist interactively before it can be imported.
- **Alternatives rejected.** Per-channel importers (duplicated rules); a generic ETL tool writing staging-to-ledger
  (bypasses command authorization).

## ADR-ING-002 — Artifact bytes in managed object storage, everything else in PostgreSQL

- **Context.** CNV-ARC-001 permits a second durable store only if objectives cannot otherwise be met; UNB-XIN-EVA-013
  requires an ADR with measurements. Older texts assume an object store. See [07](07-STORAGE-ARCHITECTURE-EVALUATION.md).
- **Decision.** Bytes and safe derivatives are non-authoritative, content-addressed objects (quarantine and records
  containers); identity, hashes, lifecycle, verdicts, retention, holds, staged records, extractions, audit and queue
  entries are PostgreSQL rows. Slot-first protocol and a reconciler keep the two consistent.
- **Measurements.** Native on-upload malware scanning and storage-enforced retention with legal hold exist only on
  object storage (FIE-C-001..031; the Cohasset assessments evidence control strength, not regulatory applicability to
  DAF sponsors). Object storage $0.0208–$0.023/GB-month vs managed PostgreSQL $0.115 (≈$0.23 with HA); bytea limit 1 GB.
  The decision rests on the first point; restore time is a supporting, unmeasured argument to be benchmarked with the
  managed-instance benchmark (CNV-ARC-001; review R2-31).
- **Consequences.** One more managed resource per environment; inventory reconciliation job; a storage adapter with a
  local test implementation. No broker, cache or new service.
- **Owner decision.** FIL-OD-02.

## ADR-ING-003 — Declarative profiles with closed catalogs; no scripting

- **Context.** Sponsors and institutions differ in layouts; a configurable mapping language would become an
  uncontrolled programming environment (mission §10, §22).
- **Decision.** Versioned JSON profiles referencing closed catalogs of F# transforms, rules, hold metrics and
  operations; sponsor-configurable attributes enumerated and bounded; new formats are reviewed platform versions.
- **Consequences.** Some partner quirks need a release (a new catalog entry) rather than configuration; this is
  accepted (review R2-08, [16](16-ADVERSARIAL-REVIEW.md)).

## ADR-ING-004 — Pull, do not host: no inbound SFTP or email in release 1

- **Context.** Banks and custodians publish files on their own SFTP and download services and APIs (FIE-B-055..060);
  email is the main business-email-compromise vector (FIE-B-019).
- **Decision.** Release 1 uses outbound pulls with pinned host keys and signature verification, partner webhooks only
  as fetch hints, and no inbound SFTP or email ingestion (FIL-OD-06, FIL-OD-07).
- **Consequences.** A push-only partner triggers an owner decision for a managed inbound SFTP service rather than a
  bespoke endpoint.

## ADR-ING-005 — Supervisor and per-artifact sandbox

- **Context.** Parser and codec vulnerabilities (FIE-C-033..035) must not reach secrets, the network, the database or
  other tenants; the architecture is a component monolith with minimal services.
- **Decision (revised after reviews R1-08 and R2-01).** A trusted supervisor (an ordinary Echelon.Execution worker)
  claims the command, fetches the object with a tenant- and object-scoped credential, verifies the hash and hands the
  bytes to a sandbox created per artifact with no network, no database access and no cloud credentials. The supervisor
  validates the sandbox's typed output, recomputes control totals in managed code that does not parse the file, and
  writes staging rows under the tenant's database role with row-level security.
- **Consequences.** A second deployment unit (workload) of the same codebase — not a new codebase, but new
  infrastructure, so the isolation primitive is an owner decision (FIL-OD-13). One sandbox path for all parsing (not only
  native codecs) is kept for simplicity; review R2-01 suggested isolating only native-codec work (recorded disagreement).

## ADR-ING-006 — Five small lifecycle machines instead of one 21-state lifecycle

- **Context and decision.** See [04](04-SECURE-INGESTION-LIFECYCLE.md). Expectation, Artifact, Run, Reconciliation and
  Retention machines plus record outcomes; exceptions are work items.
- **Consequences.** Every state is a decision or a fact; the five meanings (receipt, security acceptance, business
  acceptance, posting, reconciliation) are separately visible.

## ADR-ING-007 — AI proposes, deterministic rules and people decide

- **Context.** CNV-AIO-001/002 and unbound#18.
- **Decision (revised after review R2-17).** Release 1 uses AI for configuration assistance (partner mapping,
  migration mapping). Proposals on unstructured evidence are optional, off by default, enabled per sponsor under an
  approved provider policy (FIL-OD-04), run on the reviewer's raster after minimisation, are corroborated by independent
  sources, gated on measured precision, and switched off by a kill criterion if they do not save reviewer time. AI is
  never in the structured-record-to-financial-effect path.
- **Consequences.** Value is measured as reviewer minutes saved on evidence and engineer days saved on onboarding, both
  to be measured (UNB-ING-OPS-002, UNB-ING-AI-005).

## Framework ownership (review R2-22)

| Concern | Owner | Note |
|---|---|---|
| Artifact, run, staging and set tables (DDL) | Strata via the application (CNV-ARC-004) | Schema governance only |
| Transition vocabulary | Ordo | Semantics; Unbound executes |
| Durable steps, leases, fairness, poison isolation | Echelon.Execution (incubated in Unbound) | UNB-ING-BND-006/007 |
| Sandbox faults and parser crashes | Aegis | Unexpected-fault capture and redaction |
| Static rule "ingestion may not reference ledger writes" | Dokimos | Architecture rule |
| Upload, status, review and dashboard components | Forma | Presentation only |
| Offline behaviour of uploads (refuse, never queue bytes) | Limen | UNB-ING-UI-004 |
| Run reports and formula-safe exports (print) | Folio | Print markup |
| Sandbox role flag in the server scaffold | Conditor (proposed RM-12) | Until it exists, the application provides it |
| Security method (threat → invariant → control → evidence) | Tutela | 08 |
| Malware engine, object storage | Cloud provider (FIL-OD-01) | Managed services, no custom engine |
| Reusable requirements (EFI-*) | echelon-organization-administration INIT-030 | Application-neutral |

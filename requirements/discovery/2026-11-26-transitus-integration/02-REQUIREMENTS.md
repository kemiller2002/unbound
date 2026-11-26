# 02 — Transitus integration requirements (UNB-TRN)

Mission: [README](README.md) · Source data: [`data/gap-requirements.jsonl`](data/gap-requirements.jsonl) · Status: **proposed** — none of these records is adopted into the authoritative baseline; adoption follows the existing ratification process. Where a converged clause (`CNV-*`) states the behaviour, the clause governs on conflict.

<!-- BEGIN GENERATED (from data/gap-requirements.jsonl) -->
### UNB-TRN-001

| Field | Value |
|---|---|
| Statement | Unbound's ingestion of external files for its release-1 feeds — IRS EO BMF, Publication 78, auto-revocation list, OFAC SDN, state charity registries, NACHA returns and notifications of change, bank activity, custodian files — MUST be implemented as Unbound-owned F# feed modules that use the Transitus Formats, Checks and Evidence libraries for parsing, validation, reconciliation and evidence, rather than bespoke per-feed parsing. |
| Rationale | One governed implementation of parsing limits, control totals and raw evidence instead of one per feed; Transitus keeps no Unbound business rules. |
| Refines / parent | UNB-EXT-019; UNB-KNOW-027; UNB-CUST-009 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-AD-001 TRN-REQ-ARC-008 |
| Acceptance criteria | Given a new release-1 feed, When it is implemented, Then it parses through Transitus.Formats and its gates use Transitus.Checks. |
| Verification | Code review + test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-002

| Field | Value |
|---|---|
| Statement | Every acquired source object MUST produce an Unbound RawEvidence record (content hash, locator, rights policy, retention) through Transitus.Evidence before parsing; raw bytes MUST be stored only where the source's rights policy permits, otherwise the hash, size and a lawful reference are stored. |
| Rationale | Aligns Transitus raw preservation with the existing knowledge model instead of a parallel one. |
| Refines / parent | UNB-KNOW-023; CNV-FIN-013 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-ST-001 TRN-REQ-ARC-006 |
| Acceptance criteria | Given a source whose rights policy forbids storage, When it is acquired, Then no bytes are stored and the RawEvidence record states why. |
| Verification | Unit test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-003

| Field | Value |
|---|---|
| Statement | Every feed MUST run blocking count and control-total (or balance) reconciliation gates before submitting any domain command; tolerances, matching rules and the consequences of breaks MUST be defined in Unbound-owned profiles, with zero monetary tolerance unless a clause states otherwise. |
| Rationale | Gates before effects; accounting rules stay in Unbound. |
| Refines / parent | CNV-FIN-004; CNV-PAY-005; UNB-CUST-010 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-VR-003 TRN-REQ-VR-004 TRN-REQ-VR-006 |
| Acceptance criteria | Given a NACHA returns file whose entry hash does not match its control record, When the feed runs, Then no domain command is submitted and an exception is raised. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-004

| Field | Value |
|---|---|
| Statement | Ingested data MUST reach Unbound's books only through Unbound domain operations submitted via the durable command path (Echelon.Execution), each carrying the deterministic per-record idempotency key computed by Transitus.Evidence; no ingestion component may write business tables directly. |
| Rationale | Preserves business invariants and authorization; effectively-once outcomes. |
| Refines / parent | UNB-API-002; UNB-API-STD-002 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-AD-008 TRN-REQ-EX-006 |
| Acceptance criteria | Given the same returns file submitted twice, When both runs complete, Then the second produces zero additional business effects. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-005

| Field | Value |
|---|---|
| Statement | Unbound MUST define the domain operations ingestion loads through (e.g. charity registry observation, OFAC list update, ACH return and NOC recording, bank activity recording, custodian transaction and position recording), each with a declared natural key, idempotency behaviour, read-back query and execution class. |
| Rationale | No domain API exists yet; Transitus cannot load without it (Transitus OQ-15). |
| Refines / parent | — |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | UNB-AVL-001 |
| Transitus requirements | TRN-REQ-AD-007 |
| Acceptance criteria | Given the operation registry, When reviewed, Then each ingestion operation lists natural key, idempotency, read-back and class. |
| Verification | Review |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-006

| Field | Value |
|---|---|
| Statement | If CNV-EXP-002 remains in release scope, historical-import operations MAY accept batches only while the sponsor programme is not live (before SingleWriterActive), MUST preserve per-record idempotency keys and per-record receipts, and MUST be unavailable after go-live. |
| Rationale | Pre-go-live batches avoid live-traffic conflicts with per-fund ordering (UNB-AVL-005) and quarantine (UNB-AVL-007); per-record commands remain the rule afterwards. |
| Refines / parent | RR-REQ-X-061-r1; CNV-EXP-002 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | UNB-AVL-005 UNB-AVL-007 |
| Transitus requirements | TRN-REQ-AD-012 |
| Acceptance criteria | Given a live sponsor, When a batch historical import is submitted, Then it is refused. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | conditional (owner decision on CNV-EXP-002 vs simplification REQ-X-061 'Out') |

### UNB-TRN-007

| Field | Value |
|---|---|
| Statement | Bulk ingestion commands MUST use a configurable share of the sponsor's queued-command quota, at lower priority than interactive commands, processed by a separate worker role, so that imports cannot throttle donor and staff operations. |
| Rationale | The per-sponsor queue limit would otherwise let a migration starve interactive work (Transitus review C9). |
| Refines / parent | UNB-AVL-006 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-AD-011 TRN-REQ-PF-002 |
| Acceptance criteria | Given a bulk load in progress, When a donor submits a grant recommendation, Then it is accepted and its latency stays within budget. |
| Verification | Load test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-008

| Field | Value |
|---|---|
| Statement | Ingestion command payloads MUST carry dataset and record references and hashes only, never personal values, consistent with UNB-AVL-009 vault references. |
| Rationale | Prevents the durable command store becoming a copy of imported personal data. |
| Refines / parent | UNB-AVL-009 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-EX-013 |
| Acceptance criteria | Given an ingestion command, When its payload is inspected, Then it contains no field classified restricted-personal. |
| Verification | Unit test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-009

| Field | Value |
|---|---|
| Statement | Bulk donor import MUST provide a dry-run preview, MUST present duplicate candidates for review and MUST NOT auto-merge donor or fund records. |
| Rationale | No dedicated bulk donor import requirement exists (gap found in Transitus use case U9). |
| Refines / parent | ADM-032; INT-014 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-TX-008 TRN-REQ-EX-004 |
| Acceptance criteria | Given an import with two near-identical donors, When previewed, Then a candidate pair is shown and nothing is merged. |
| Verification | Integration test |
| Priority | P2 |
| Status | proposed · not implemented |
| Release scope | deferred (no release-1 consumer identified) |

### UNB-TRN-010

| Field | Value |
|---|---|
| Statement | Ingestion exceptions requiring human resolution (unmatched returns, reconciliation breaks, outcome-unknown loads) MUST surface in Unbound's existing case and work-item handling, not in a separate ingestion queue UI. |
| Rationale | Avoids a duplicate operator surface (Transitus review C10). |
| Refines / parent | — |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-UI-011 |
| Acceptance criteria | Given an unmatched ACH return, When the feed completes, Then a case exists with the evidence package reference. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-TRN-011

| Field | Value |
|---|---|
| Statement | If migration is in scope, the MigrationRun 'Reconciled' state MUST require a Transitus conservation table (every source record loaded, rejected or excluded with reason) and per-fund balance reconciliation (opening plus movements equals closing) with zero unexplained differences. |
| Rationale | Makes the reconciliation step of the migration state machine evidential. |
| Refines / parent | RR-REQ-X-061-r1 |
| Source | Transitus initiative (kemiller2002/transitus, 2026-11-26) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (feed modules, domain operations); Transitus libraries (formats, checks, evidence) |
| Dependencies | — |
| Transitus requirements | TRN-REQ-VR-002 TRN-REQ-VR-007 |
| Acceptance criteria | Given a migration with one unexplained fund difference, When Reconciled is requested, Then the transition is refused. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | conditional (as UNB-TRN-006) |

<!-- END GENERATED -->

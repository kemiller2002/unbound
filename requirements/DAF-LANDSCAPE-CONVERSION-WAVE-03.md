# DAF Landscape conversion, wave 3: append-only data, events, controls, economics
Date: 2026-11-21 | Status: candidate requirements pending review
Primary sources:
- https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/06-data-and-knowledge/canonical-data-model.md
- https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/06-data-and-knowledge/event-catalog.md
- https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/08-governance-and-compliance/governance-architecture.md
- https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/09-financial-models/financial-and-staffing-models.md

## Binding user architectural decision: append-only
**UNB-DATA-001 (MANDATORY):** The authoritative business database SHALL be append-only. No UPDATE or DELETE of committed authoritative business events, decisions, facts, journal entries, policy versions, approvals or evidence. Corrections SHALL append a new correction, reversal, supersession, or compensating record, referencing the original and explaining why. This is a non-negotiable design constraint unless the owner explicitly changes it.

**UNB-DATA-002:** Every authoritative record SHALL have a durable identity, actor/authority, event/recorded time, applicable effective time, provenance, schema version, correlation/causation, and integrity metadata. Where meaningful, record source hash and policy version.
**UNB-DATA-003:** Bitemporal queries SHALL distinguish what was effective in the world from what the enterprise knew at a particular time; backdated corrections must not rewrite knowledge history.
**UNB-DATA-004:** Current state SHALL be computed as a deterministic projection of immutable authoritative records. Projection caches, materialized views, search indexes and read replicas MAY be rebuilt or replaced, but are explicitly non-authoritative. Their updates do not constitute modification of the append-only record.
**UNB-DATA-005:** Corrections SHALL preserve the original, correction reason, authorized corrector, linked evidence and effect on derived balances, grants, filings and notices. Financial corrections SHALL use balanced reversal/adjustment entries, never silent journal mutation.
**UNB-DATA-006:** Published policies and approval records SHALL be immutable; changing a policy appends a newly approved version and explicit effective period.
**UNB-DATA-007:** Legal retention, privacy rights, data minimization and erasure requests SHALL be addressed through reviewed architecture (e.g., segregated encrypted identity vault and documented cryptographic erasure where lawful) without falsely promising that immutable sensitive payloads can be deleted. Avoid putting unnecessary PII into the immutable event payload. Legal counsel must resolve jurisdictional retention/deletion conflicts before production.
**UNB-DATA-008:** The system SHALL prevent ordinary application roles, administrators and agent tools from rewriting history; investigate physical immutability, database permissions, tamper-evident digests, backups and independently verifiable checkpoints. Append-only at the application API alone is insufficient.
**UNB-DATA-009:** Replay SHALL be deterministic and side-effect-free. External effects (payments, emails, orders, acknowledgments) SHALL use durable issuance receipts and idempotency controls so replay cannot repeat them.
**UNB-DATA-010:** Data migration and schema evolution SHALL preserve old events and apply versioned upcasters or equivalent interpretation; no destructive rewrite of historical authoritative payloads.
**UNB-DATA-011:** Multi-sponsor tenancy SHALL be enforced on write, read, replay, export, and evidence access; cross-sponsor shared facts require separate rights and provenance.
**UNB-DATA-012:** Integrity verification SHALL detect omitted, altered, duplicated and reordered authoritative records, including restore and disaster recovery scenarios. The chosen hash-chain, signed checkpoint or other mechanism is a design decision to be benchmarked, not assumed.

### Acceptance tests for append-only
- Attempt direct UPDATE/DELETE of committed events and journal records with application credentials: denied.
- Append a correction to an incorrect grant amount: original persists, corrected projection is accurate, historical as-known-at query returns the earlier view.
- Append a backdated eligibility correction: both effective-at and recorded-at queries remain accurate.
- Replay the complete event history twice: identical projections and zero duplicated external payments or messages.
- Rebuild all projections from scratch and reconcile independently to event/journal evidence.
- Simulate event corruption, truncation, unauthorized insertion, missing outbox dispatch, duplicate webhook and backup restore: detection and recovery evidenced.
- Rotate a policy while a grant is pending: decision references the correct approved policy version.
- Exercise privacy deletion/retention requests without claiming an impossible physical erasure from immutable backups.

## Event catalog conversion
Source events.csv describes 58 events; detailed row-by-row reconciliation remains open.
**UNB-EVT-001:** Maintain a governed event catalog with owner, schema, version, producer, authorized actor, consumer, control, evidence and side-effect classification for every event.
**UNB-EVT-002:** At-least-once event delivery SHALL use idempotent consumers and commands, subject-scoped ordering, durable outbox and recovery for partial failures.
**UNB-EVT-003:** Events from external files/webhooks SHALL carry retrieval evidence and source authenticity checks; ingestion does not by itself grant authoritative sponsor approval.
**UNB-EVT-004:** Causation-depth, fan-out and money movement limits SHALL prevent agent/event loops and uncontrolled cascades.
**UNB-EVT-005:** State-changing events with legal/financial consequences SHALL be emitted only by approved deterministic rules or authorized people, not raw probabilistic agent output.
**UNB-EVT-006:** Every external effect SHALL be reconciled to its originating authorized decision and receipt, including reversals, returns and settlement failures.

## Governance and control conversion
**UNB-GOV-001:** Sponsor board and delegated decision-makers SHALL retain authority over mission, risk appetite, policy, investment oversight and legally reserved decisions.
**UNB-GOV-002:** Every policy parameter that changes approval, eligibility, agent permissions, financial limits or risk SHALL be versioned, reviewed, tested and independently approved.
**UNB-GOV-003:** Enforce separation of duties, conflict recusals and authority limits in the authorization engine rather than relying solely on warnings or written policy.
**UNB-GOV-004:** Preserve independent audit evidence and board-readable reporting; distinguish objective snapshots from agent-generated interpretation.
**UNB-GOV-005:** Related-party infrastructure/sponsor arrangements SHALL be disclosed, independently reviewed and subject to documented commercial reasonableness controls where applicable.
**UNB-GOV-006:** Numerical thresholds and proposed board composition in the landscape governance model are policy assumptions, not automatically applicable legal requirements. Verify jurisdiction and sponsor form before adoption.
**UNB-GOV-007:** Complete the source REG→CTL traceability mapping and document uncovered obligations before regulatory gate approval.

## Financial and operating model conversion
**UNB-ECO-001:** Model accounts, AUM, average balance, grants, contributions, exception rate, review time, complex asset volume, service contacts and reconciliation breaks as separate drivers.
**UNB-ECO-002:** Separate measured inputs, sourced industry benchmarks, scenarios and assumptions; every sensitivity output must expose parameter provenance.
**UNB-ECO-003:** Account for minimum responsible staffing and segregation-of-duties floors even when transactional processing is automated.
**UNB-ECO-004:** Compare against conventional providers with equivalent affiliate-staffing and cost scope, not misleading per-account ratios.
**UNB-ECO-005:** Run stress scenarios for year-end surge, high exception rates, market drawdown, custodian outage, data-rights denial, fraud and policy change.
**UNB-ECO-006:** No claim of demonstrated operational cost superiority before partner shadow replay measures exception frequency, handling minutes, errors and reconciliation costs.
**UNB-ECO-007:** Measure storage growth, replay time, projection rebuild time, integrity-check cost, privacy segregation and retention cost for append-only architecture. The architecture's durability benefits have operational costs.

## Decisions and open questions
1. **Decided:** authoritative data is append-only; corrections are appended, never updates.
2. **Not decided:** storage engine, database vendor, physical WORM/immutable-storage technique, hash-chain scheme, snapshot strategy, tenant partitioning, retention periods and personal-data erasure mechanism.
3. **Not decided:** exact regulatory scope and controls, authority thresholds, commercial launch model, banking/custody integrations.
4. **Pending inventory:** row-level review of 58 events, governance control CSV and regulatory matrix; quantitative scenario and adversarial test coverage.
5. **Architecture gate:** Conditor installation/plan and latest compatible EF system audit remain required before implementation.

## Relationship to existing Unbound requirements
This wave elaborates UNB-FIN-001/002, UNB-AIO-001/003, UNB-TEC-001/003, UNB-VAL-001/002 and UNB-CTL-001..006. The mandatory append-only requirement supersedes any ambiguous reading of 'immutable or correction-traceable' in UNB-FIN-001; authoritative records must be immutable **and** correction-traceable.

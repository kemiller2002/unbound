# Partner segregation and tenant deployment architecture
Date: 2026-11-21 | Status: owner-directed architectural requirements, detailed technology selection pending

## Decision
Unbound SHALL support multiple partner isolation profiles: (A) separate schemas in a shared database, (B) separate databases within shared managed infrastructure, and (C) dedicated infrastructure and database. Deployment choice is per partner and based on contractual, regulatory, risk, data residency, resilience and economic requirements. No partner is assumed to require a specific tier without evidence. Bank of America is an illustrative institutional partner, not an asserted customer or approved deployment.

The **authoritative append-only rule applies identically in every isolation profile**. Tenant isolation is not provided merely by adding a tenant_id column, choosing a schema, or instructing an AI agent to filter data.

## Mandatory requirements
- **UNB-TEN-001:** Every partner SHALL have an immutable tenant identity independent of database name, schema name, hostname and infrastructure location.
- **UNB-TEN-002:** Support shared-database/separate-schema, dedicated-database, and dedicated-infrastructure deployment profiles through explicit configuration and validated compatibility; partner business logic must not fork by tier.
- **UNB-TEN-003:** Every authoritative event, fund, party relationship, grant, journal entry, policy, evidence reference, agent action and projection SHALL have an enforced tenant security boundary. Cross-tenant access is denied by default, including administrator, background job, support, analytics and agent paths.
- **UNB-TEN-004:** Tenant routing SHALL derive from verified identity and authorized tenancy context, never a client-supplied schema/database name or unverified header. Use an independently tested routing and connection-broker boundary.
- **UNB-TEN-005:** For shared databases with separate schemas, partner-specific roles SHALL be unable to read/write other schemas; default schema search paths, SECURITY DEFINER routines, migrations, connection pooling, shared extensions and administrative credentials require threat modeling and negative tests.
- **UNB-TEN-006:** For dedicated databases, credentials, encryption keys, backup/restore policy, resource quotas, monitoring, recovery objectives and access reviews SHALL be partner-scoped to the degree promised by contract.
- **UNB-TEN-007:** Shared services (identity, registry, charity intelligence, orchestration, telemetry, support and model services) SHALL have an explicit classification: shared public reference, permitted aggregated information, partner-confidential information, or prohibited cross-tenant flow. Shared infrastructure must not become an unreviewed data-sharing mechanism.
- **UNB-TEN-008:** Support per-partner retention, residency, encryption-key management, privacy policy, audit export, disaster recovery objectives, data processing agreements and access restrictions where technically and legally applicable.
- **UNB-TEN-009:** Tenant isolation SHALL apply to caches, queues, event streams, outboxes, object storage, search indexes, backups, logs, observability, prompt/context construction, embeddings, evaluation datasets and generated documents, not just relational tables.
- **UNB-TEN-010:** Partner-specific policies and configurations SHALL be versioned immutable records with sponsor/partner authority boundaries; a partner's preferences cannot override the sponsoring charity's reserved legal authority.
- **UNB-TEN-011:** Tenant provisioning, migration, upgrade, suspension and offboarding SHALL use a governed, reproducible plan with approvals, inventory, integrity verification, rollback/compensation strategy and evidence.
- **UNB-TEN-012:** Migrate a tenant between isolation profiles without changing stable tenant/fund/party/event identifiers or losing append-only provenance, corrections, balances, policy versions, consent, grants, audit chain or bitemporal reconstruction.
- **UNB-TEN-013:** Migration SHALL use a consistent event high-water mark, verifiable source/destination record equivalence, independent ledger reconciliation, replay-safe outbox handoff and controlled cutover. Never permit two active authoritative writers for one tenant.
- **UNB-TEN-014:** Backups and restoration SHALL honor tenant boundaries. A shared physical backup does not guarantee independent per-tenant recovery; document and test tenant-selective restore and confidentiality limitations.
- **UNB-TEN-015:** Define explicit partner-specific performance, availability, recovery and capacity targets. Shared-schema tiers must have noisy-neighbor detection and isolation controls.
- **UNB-TEN-016:** Each partner SHALL have an exportable, evidence-complete logical dataset and portability protocol, subject to lawful retention, sponsor ownership, third-party rights and contracts.
- **UNB-TEN-017:** Operations SHALL record which isolation profile and security controls were in effect for a partner at any point in time; changes are append-only authorized events.
- **UNB-TEN-018:** Before selecting a database engine, benchmark schema counts, migration fan-out, connection pools, query planner/locking contention, event replay, integrity checking, tenant restore and cost at realistic partner sizes.
- **UNB-TEN-019:** Unbound's F# modular monolith and managed database preference SHALL remain compatible with per-tenant connection routing. A multi-database deployment does not require a microservices architecture.
- **UNB-TEN-020:** Multi-sponsor, multi-partner and multi-fund are separate dimensions. Model sponsor legal entity, distribution partner, institutional channel, and DAF account distinctly; a partner is not automatically the legal sponsor, and one sponsor may serve several partners.

## Deployment profiles
| Profile | Isolation | Operational considerations |
|---|---|---|
| A | Shared database, separate partner schemas | Independent schema privileges and explicit shared-DB backup/administrator caveats |
| B | Separate partner database on managed platform | Separate credentials and backup schedules; underlying platform and operator may still be shared |
| C | Dedicated partner database and infrastructure | Independent resources, keys and recovery boundaries, with higher operational burden |

An additional shared-schema/row-level-security model is **not** approved as a default isolation profile; it may be evaluated as an explicit fourth option only if required and independently validated.

## Acceptance and adversarial scenarios
1. Partner A staff, advisor and agent cannot enumerate, query, export, infer or correct Partner B events, funds, identities, prompts, search results or backup data.
2. A malicious client supplies a different partner ID, schema name or connection hint; routing ignores untrusted values.
3. Shared connection pool reuses a connection across tenants; security context is reset and negative tests prevent leakage.
4. A privileged support user receives time-limited audited break-glass access; approval and scope are recorded, no silent cross-partner access.
5. A partner moves A→B and B→C while retaining immutable history, stable IDs, bitemporal queries, current balances and legal evidence.
6. Cutover is interrupted after snapshot but before tail catch-up; no split-brain writer, lost event or duplicated payment.
7. An outbox message is replayed after migration; no duplicate payment, trade or notice.
8. A schema migration fails for one partner; other partners continue safely, and the failed partner's history remains intact.
9. A shared database suffers corruption; independently demonstrate what can and cannot be restored for a single tenant.
10. A shared charity registry is updated; public/reference facts can be reused but private partner determinations cannot leak without documented sharing rights.
11. A partner requests an isolated model or telemetry environment; enforce policy or reject the unsupported configuration explicitly.
12. A successor DAF account is created within the same sponsor but different partner channel; cross-partner movement requires an approved transfer/allocation and privacy authorization, never a direct database copy.
13. An attempted direct UPDATE/DELETE against an event store in each profile is denied; append-only corrections and integrity verification work identically.
14. A partner offboards; produce authorized export, retain mandatory sponsor records and perform permissible deletion/cryptographic erasure without pretending immutable backups vanished.

## Design decisions pending
- Actual database engine and schema/namespace support; managed service selection.
- Encryption-key hierarchy, tenant-specific backup and recovery approach, and WORM/immutable evidence anchor.
- Partner/sponsor legal relationship, data controller/processor roles and cross-partner sharing rights.
- Required residency, RPO/RTO, scale tiers and commercial pricing.
- Whether a fourth profile (shared schema with row-level security) is justified; it is not equivalent to a separate schema.
- Conditor and EF Registry current supported capabilities before implementation.

## Traceability
Expands UNB-API-005, UNB-DATA-008/011/012, UNB-IMM-009, UNB-PART and UNB-SUCC requirements, UNB-LEG-002, UNB-TEC-001/002/003 and UNB-COM-002. Candidate requirements must pass architecture, legal and security validation before implementation.

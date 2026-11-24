# Partner segregation and tenant deployment architecture
Date: 2026-11-21 | Status: owner-directed architectural requirements, detailed technology selection pending

## Decision
Unbound SHALL support multiple partner isolation profiles: (A) separate schemas in a shared database, (B) separate databases within shared managed infrastructure, and (C) dedicated infrastructure and database. Deployment choice is per partner and based on contractual, regulatory, risk, data residency, resilience and economic requirements. No partner is assumed to require a specific tier without evidence. Bank of America is an illustrative institutional partner, not an asserted customer or approved deployment.

The **authoritative append-only rule applies identically in every isolation profile**. Tenant isolation is not provided merely by adding a tenant_id column, choosing a schema, or instructing an AI agent to filter data.

## Mandatory requirements
- **UNB-TEN-001:** Every partner SHALL have an immutable tenant identity independent of database name, schema name, hostname and infrastructure location.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-002:** Support shared-database/separate-schema, dedicated-database, and dedicated-infrastructure deployment profiles through explicit configuration and validated compatibility; partner business logic must not fork by tier.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-003:** Every authoritative event, fund, party relationship, grant, journal entry, policy, evidence reference, agent action and projection SHALL have an enforced tenant security boundary. Cross-tenant access is denied by default, including administrator, background job, support, analytics and agent paths.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-004:** Tenant routing SHALL derive from verified identity and authorized tenancy context, never a client-supplied schema/database name or unverified header. Use an independently tested routing and connection-broker boundary.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
> Adopted 2026-11-23: refined by `RR-UNB-TEN-004-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-TEN-005:** For shared databases with separate schemas, partner-specific roles SHALL be unable to read/write other schemas; default schema search paths, SECURITY DEFINER routines, migrations, connection pooling, shared extensions and administrative credentials require threat modeling and negative tests.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-006:** For dedicated databases, credentials, encryption keys, backup/restore policy, resource quotas, monitoring, recovery objectives and access reviews SHALL be partner-scoped to the degree promised by contract.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-007:** Shared services (identity, registry, charity intelligence, orchestration, telemetry, support and model services) SHALL have an explicit classification: shared public reference, permitted aggregated information, partner-confidential information, or prohibited cross-tenant flow. Shared infrastructure must not become an unreviewed data-sharing mechanism.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-008:** Support per-partner retention, residency, encryption-key management, privacy policy, audit export, disaster recovery objectives, data processing agreements and access restrictions where technically and legally applicable.
> Converged 2026-11-24: governed by `CNV-TEN-001` (section 'Converged requirements: master mission' in the domain file named by each clause).
- **UNB-TEN-009:** Tenant isolation SHALL apply to caches, queues, event streams, outboxes, object storage, search indexes, backups, logs, observability, prompt/context construction, embeddings, evaluation datasets and generated documents, not just relational tables.
- **UNB-TEN-010:** Partner-specific policies and configurations SHALL be versioned immutable records with sponsor/partner authority boundaries; a partner's preferences cannot override the sponsoring charity's reserved legal authority.
- **UNB-TEN-011:** Tenant provisioning, migration, upgrade, suspension and offboarding SHALL use a governed, reproducible plan with approvals, inventory, integrity verification, rollback/compensation strategy and evidence.
- **UNB-TEN-012:** Migrate a tenant between isolation profiles without changing stable tenant/fund/party/event identifiers or losing append-only provenance, corrections, balances, policy versions, consent, grants, audit chain or bitemporal reconstruction.
- **UNB-TEN-013:** Migration SHALL use a consistent event high-water mark, verifiable source/destination record equivalence, independent ledger reconciliation, replay-safe outbox handoff and controlled cutover. Never permit two active authoritative writers for one tenant.
- **UNB-TEN-014:** Backups and restoration SHALL honor tenant boundaries. A shared physical backup does not guarantee independent per-tenant recovery; document and test tenant-selective restore and confidentiality limitations.
- **UNB-TEN-015:** Define explicit partner-specific performance, availability, recovery and capacity targets. Shared-schema tiers must have noisy-neighbor detection and isolation controls.
- **UNB-TEN-016:** Each partner SHALL have an exportable, evidence-complete logical dataset and portability protocol, subject to lawful retention, sponsor ownership, third-party rights and contracts.
  - *Refined 2026-11-24 by RR-UNB-TEN-016-r1 (adopted; see `requirements/discovery/2026-11-23-cross-industry`, decisions XID-REC-001, decided-provisional):* On sponsor exit, Unbound MUST deliver within 30 days of ExitNoticed (or the contract period) an ExitPackage containing: the tenant's complete authoritative event history and current projections in the documented export schema, issued document artifacts with hashes, policy and configuration versions, audit and evidence records, and a manifest signed with SHA-256 digests plus record counts and ledger trial balances per fund and period that reconcile to the live system at the export watermark. The sponsor MUST be able to verify the package independently. After delivery the tenant MUST enter ReadOnlyRetention for the contractually agreed period, during which authorized sponsor users can query and export. Destruction MUST be performed by destroying tenant-scoped encryption keys under dual control after retention and hold checks, and Unbound MUST issue a destruction certificate stating what was destroyed, what persists (e.g. immutable backups until their expiry, with dates) and which records were retained by legal obligation. Distribution partners MUST receive only data the sponsor authorizes, never by default. Decided timings (XID-REC-001): the sponsor is records custodian after exit; ReadOnlyRetention lasts 90 days after the reconciled ExitPackage is delivered; backups expire on their normal schedule, no more than 35 days thereafter; legal holds override deletion and are listed on the destruction certificate. The text above is retained as originally written.
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

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-TEN-004-r1 (refines `UNB-TEN-004`) — adopted
- **Original:** `requirements/PARTNER-SEGREGATION-AND-TENANCY.md:13` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** The TenantConnectionBroker MUST derive the tenant for every request from the authenticated principal's server-held entitlement set. A client MAY submit an opaque tenant selection token only if it was issued by the server from that entitlement set for the current session; the broker MUST revalidate membership and delegation validity on every request and MUST ignore any schema, database, host or raw tenant identifier supplied by a client, partner API, widget host or AI agent.
- **Acceptance criteria:**
  - AC-1: Given advisor A entitled to tenants T1 and T2 with T1 selected; when A requests a T2 fund by ID; then response is RESOURCE_NOT_FOUND and a denial event is recorded.
  - AC-2: Given a request carries header X-Tenant: T2 and schema hint 'partner_b'; when the principal is entitled only to T1; then the hints are ignored and the request executes in T1 or is denied.
  - AC-3: Given A's delegation for T2 is revoked; when A sends the next request with a valid T2 selection token; then the request is denied and the token invalidated.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-TEN-004-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-PLT-001 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-005 · **Parent:** UNB-TEN-020 · **Decisions:** RCD-004
- **Requirement:** Unbound MUST define Tenant, LegalSponsor, DistributionPartner, Program, Fund and DeploymentCell as distinct typed entities, and MUST publish for every data class its isolation key (tenant or sponsor) and whether it may be shared across partners of the same sponsor.
- **Validation still required:** RCD-004: Architecture spike DG-09 and DG-11
- **Acceptance criteria:**
  - AC-1: Given data class 'charity review note'; when the isolation map is queried; then it returns its isolation key and cross-partner sharing rule.
  - AC-2: Given a requirement uses 'sponsor/tenant'; when lint runs; then it is flagged.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-001); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-REG-001:** Every deployment MUST record a FundsFlow declaration naming, for each account that holds contribution or grant money, the legal title holder and every party with instruction authority. Accounts holding charitable money MUST be titled to the legal sponsor (or a custodian for the sponsor); the platform operator MUST have instruction-only authority under the sponsor's delegation. A deployment whose FundsFlow shows the operator or an affiliate as title holder or receiving funds into its own account MUST NOT activate until operator counsel records a money-transmission/MSB analysis covering the states served. Changes to FundsFlow MUST re-trigger this gate. The operator MUST NOT take title to or possession of contribution or grant money in any deployment, and no embedded or white-label deployment that touches money MAY go live until a multistate money-transmission memo is recorded (XID-REG-001).
  - Provenance: [UNB-XIN-REG-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-BNK-005, XID-REG-001 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given FundsFlow shows sponsor-titled disbursement account; when activation; then gate passes.
    - AC-2: Given FundsFlow shows operator-titled pooled account; when activation; then blocked MT_ANALYSIS_REQUIRED.
    - AC-3: Given FundsFlow edited to add operator sweep account; when save; then activation status reverts to blocked pending analysis.
    - AC-D1: Given an embedded deployment with sponsor-titled accounts and no multistate memo on file; when go-live is requested; then go-live is blocked with MT_MEMO_REQUIRED.
- **UNB-XIN-REG-002:** The system MUST maintain an OperatorStatusRegister mapping status-relevant features to the legal question they raise: (a) operator-provided investment model portfolios, recommendations or advice for compensation -> investment-adviser analysis; (b) any operator compensation tied to securities transactions or order routing -> broker-dealer analysis; (c) donation pages soliciting for multiple charities -> charitable-platform registration (e.g. Cal. Gov. Code §12599.9); (d) operator title or transmission of funds -> UNB-XIN-REG-001; (e) operator processing of consumer financial information as a for-profit -> GLBA/FTC Safeguards analysis including 16 CFR 314.4(j) notification. Activating a mapped feature for a deployment MUST require a recorded counsel determination for that question, deployment and jurisdiction set. Default dispositions (XID-REG-002): investment advice or model construction for compensation is disabled; compensation varying with securities transactions or custody is prohibited (operator fees are fixed); a feature whose status question is unmapped cannot be enabled.
  - Provenance: [UNB-XIN-REG-002](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-INV-004, XID-REG-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given feature 'operator model portfolios' unmapped determination; when activation; then blocked STATUS_DETERMINATION_REQUIRED.
    - AC-2: Given determination recorded for deployment D1; when activation for D2; then still blocked (determination is per deployment).
    - AC-3: Given new feature added to catalogue without register entry; when release build; then CI check fails.
    - AC-D1: Given an operator pricing plan with a per-trade fee; when it is attached to a deployment; then it is rejected with TRANSACTION_BASED_COMPENSATION_PROHIBITED.
- **UNB-XIN-SAS-001:** Unbound MUST maintain a versioned, effective-dated CapabilityEntitlement record per tenant and program that lists each optional capability as Entitled or NotEntitled with the contract reference. Authorization MUST deny a command that requires a NotEntitled capability with a stable error code. Removing an entitlement MUST NOT cancel, block or alter any command already accepted, any protective hold, statement, acknowledgment, retention obligation or regulatory output; those fiduciary capabilities MUST be marked NonRemovable in the catalog. Entitlements MAY be scoped by jurisdiction: D2C own-brand app and charity-side giving button capabilities MUST be NotEntitled for California users until a California opinion is recorded (XID-SAS-005).
  - Provenance: [UNB-XIN-SAS-001](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-SAS-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given program P has InternationalGrants NotEntitled; when a donor submits an international grant recommendation; then the command is rejected with ENTITLEMENT_REQUIRED and no reservation is created.
    - AC-2: Given a grant accepted while ComplexAssets was Entitled; when the entitlement is removed; then the accepted command proceeds to completion and the audit shows the entitlement version at acceptance.
    - AC-3: Given Statements is catalogued NonRemovable; when an operator attempts to set it NotEntitled; then the change is rejected.
    - AC-D1: Given a California donor and no California opinion recorded; when they open the D2C own-brand app; then the giving capability is unavailable with JURISDICTION_GATED.
- **UNB-XIN-SAS-007:** A distribution-partner exit MUST be processed as a PartnerExit case with states Notified > DonorOutreach > ElectionsCollected > TransfersProposed > SponsorDecided > TransfersExecuted > Reconciled > Closed. Funds MUST move to another sponsor only as individual outbound transfer grants under RR-UNB-WF-014-r1, each with a recorded recommendation from an authorized advisor of that fund and a decision by sponsor A (which MAY be a sponsor-approved batch policy); no fund MUST change LegalSponsorId by data migration. Funds whose advisors do not recommend transfer MUST remain with sponsor A, with access through a sponsor-direct credential provisioned before the partner's federation ends, and with servicing reassigned to sponsor A or a successor partner. Partner access, subscriptions and API clients MUST terminate at the exit effective time while accepted recommendations continue under sponsor review. The case MUST reconcile: count and value of funds elected, transferred, declined and remaining, with zero unexplained difference. A sponsor-approved batch policy is the written exercise of sponsor A's discretion and MUST be applied per fund: each transfer has its own decision record citing the policy and its hold and sanctions checks; sponsor A MAY decline or condition individual transfers; a contractual pre-commitment that removes sponsor discretion MUST NOT be accepted (XID-SAS-002).
  - Provenance: [UNB-XIN-SAS-007](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-SAS-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given partner P with 1,000 funds exits; 700 advisors recommend transfer to sponsor B; 200 silent; 100 object; when the case closes; then 700 transfer grants exist with individual recommendations and sponsor decisions; 300 funds remain with sponsor A with sponsor-direct access; reconciliation difference is zero.
    - AC-2: Given an operator attempts to update LegalSponsorId of a fund; when the command is submitted; then it is rejected; only TransferOut grants can move value.
    - AC-3: Given partner federation ends at T; when a silent donor logs in at T+1 day; then they authenticate with the pre-provisioned sponsor-direct factor and see their fund under sponsor A.
    - AC-D1: Given a batch policy approving 200 transfers, one of which has an open sanctions potential match; when the batch executes; then 199 transfers commit with individual decision records and the matched one is held.
- **UNB-XIN-EVA-009:** A bulk transfer of funds from one legal sponsor to another MUST run as a TransferProgram process manager that issues, per fund, an independent DAF_TRANSFER_OUT distribution in the source sponsor ledger and an independent inbound contribution in the destination sponsor ledger, each with its own authorization, idempotency key (TransferProgramId + FundId) and reconciliation. Source history MUST remain in the source sponsor's books; the destination MUST receive a signed, hashed predecessor-history package recorded as non-authoritative reference evidence, with a fund-identifier crosswalk. Before a fund's transfer posts, its in-flight items (reserved grants, Unknown payments, open returns, pending contributions) MUST be resolved or explicitly assigned by the program's approved rule; a fund with an Unknown payment MUST NOT transfer. Legal form (XID-EVA-005): each transfer is a grant to the receiving sponsoring organization under a master agreement recording each fund's advisory terms; the source keeps its records and the destination receives a reference copy, with donor notice before personal data transfer.
  - Provenance: [UNB-XIN-EVA-009](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-SAS-002, XID-EVA-005 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given Program moving 10,000 funds; 37 have Unknown payments; when program runs; then 9,963 funds transfer; 37 remain with reason UNKNOWN_PAYMENT_OPEN; program status PartiallyComplete.
    - AC-2: Given Fund F transferred; when destination statement generated; then destination shows an opening inbound contribution with predecessor-history reference; no source journal lines appear in destination ledger.
    - AC-3: Given Program crashes after 4,000 funds; when resumed; then no fund transfers twice (TransferProgramId + FundId key).
    - AC-D1: Given a TransferProgram with no master agreement recorded; when the first transfer is proposed; then it is blocked with MASTER_AGREEMENT_REQUIRED.
<!-- END ADOPTED: cross-industry-discovery -->

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-ARC-002 — Tenant isolation and operator access:** The tenant unit SHALL be the legal sponsor; partner and program isolation SHALL be threat-tested. Operator access to a tenant SHALL be denied by default; read diagnostics SHALL be scoped, expiring and pre-approved by the sponsor; writes SHALL require per-case approval; revocation SHALL take effect immediately and every access SHALL be evidenced. A minimal shared party core SHALL be partitioned by role and tenant.
  - Provenance: resolves XID-IAM-001, UNB-XIN-IAM-002#Q1, XID-CRM-003, RCD-004; amends UNB-XIN-IAM-002; related findings RCF-PLT-001. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Acceptance:
    - AC-1: Given an operator without an active grant, when they query tenant B, then the request is refused and alerted.
    - AC-2: Given a revoked grant, when the operator session continues, then the next request is refused.
- **CNV-TEN-001 — Tenant unit, administrator principal and shared stores (ADV-T-006, ADV-T-019, ADV-T-024):** Isolation profiles SHALL be selected per legal sponsor; partner and program isolation SHALL be enforced inside the sponsor cell by typed partitioning and negative tests, and a partner needing dedicated infrastructure SHALL be served by a dedicated sponsor cell or a separate legal sponsor. An Administrator principal type SHALL hold a per-sponsor ServiceDelegation (task classes, data classes, expiry) recorded in each sponsor's cell; a federated queue projection SHALL contain only routing metadata (case id, kind, priority, due time, sponsor id) and no donor PII; opening an item SHALL authenticate into that sponsor's cell under the delegation. A normative SharedStoreRegister SHALL list each cross-tenant store with permitted fields and classification, writers, readers and inference risks; donor and advisor PII SHALL NOT enter any shared store except as a keyed token for authorized checks. The shared party core SHALL hold only a keyed token, role class and sponsor id, never names or contact details. This clause depends on acceptance of the database-per-sponsor architecture decision.
  - Provenance: resolves Phase 3 feedback; amends UNB-TEN-001, UNB-TEN-002, UNB-TEN-003, UNB-TEN-004, UNB-TEN-005, UNB-TEN-006, UNB-TEN-007, UNB-TEN-008. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Architecture; financial system of record: n/a; policy owner: Unbound architecture owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound engineering.
  - Acceptance:
    - AC-1: Given an administrator user without a delegation for sponsor B, when they open a B case from the federated queue, then access is refused.
    - AC-2: Given a new cross-tenant store not in the register, when deployment runs, then the release check fails.
    - AC-3: Given a party-core schema with a column outside its register entry, when the schema check runs, then it fails.
<!-- END ADOPTED: convergence-2026-11-24 -->

# SaaS platform operations, IAM, records, workflow automation, other industries and operating models (codes SAS, IAM, REC, WFA, OTH)

Date 2026-11-23. Baseline `777a70d`. Every ID here is a proposal; none is validated.

**Outputs:**
- 66 atlas rows
- 32 findings
- 24 new requirements
- 5 refinements
- 29 scenarios
- 11 rejections
- 21 failure modes
- 9 decisions
- 3 conflicts
- 10 business-model rows

## 0. What already exists (not duplicated)

**Tenancy.** UNB-TEN-001..020 and architecture/TENANT-AWARE-PERSISTENCE-DESIGN.md cover isolation profiles, routing, migration, backups and export.

**Prior reconciliation.**
- RCD-004 (tenant = legal sponsor; still open).
- RR-REQ-X-057-r1 (one codebase, one release, N cells) and RR-UNB-PLAT-008-r1 (Profile B is the default).
- RR-UNB-TEN-004-r1, UNB-RCN-ARC-001/002 and UNB-RCN-PLT-004/005/007.
- RR-REQ-X-036-r1, RR-REQ-X-061-r1 and RR-UNB-WF-014-r1.

**Configuration, white label, embedding, AI-readable contracts and retention.** Covered by REQ-X-045, REQ-X-030, REQ-X-063, REQ-X-051 and REQ-X-038.

**What was missing** is the operational layer between "one product" and "many tenants":
- entitlements
- feature flags
- release rings
- webhooks
- the event envelope
- metering
- SLOs
- sandboxes
- operator access and break-glass
- partner exit with funds
- hold propagation
- timers
- print and mail chain of custody

## 1. Enterprise SaaS platform operations

### 1.1 Independent assessment

A multi-tenant financial SaaS serving many legal entities needs the following. All of this is industry practice, not law.

1. One tenancy unit stamped on every record, message, log, metric and cost line.
2. Entitlements kept separate from configuration and from release toggles.
3. One release train, with ring rollout, expand/contract schema changes and version-skew limits.
4. Versioned APIs with deprecation windows.
5. Governed webhooks.
6. A tenant lifecycle: provision, suspend, exit, destroy.
7. Sandboxes.
8. Per-tenant metering.
9. SLOs with error budgets.
10. RPO and RTO set per data class.
11. Access transparency for customers.
12. A subprocessor register.

### 1.2 Comparison with Unbound

| Capability | Unbound status | Action |
|---|---|---|
| Tenant model and isolation | Partial or contradictory (RCD-004, RCC-ARC-001) | Close RCD-004. XIC-SAS-001 records a residual inconsistency between RR-UNB-PLAT-008-r1 and RCD-004 |
| White label, terminology, configuration lifecycle, declarative rules | Covered | — |
| Entitlements | Missing | UNB-XIN-SAS-001; fiduciary capabilities are NonRemovable |
| Feature flags | Missing | UNB-XIN-SAS-002; flags expire within 90 days and may not carry tenant-specific behavior |
| Release rings and schema fan-out | Partial | UNB-XIN-SAS-003: ring halt, N/N-1 compatibility, 14-day skew cap, December freeze |
| API deprecation | Vague (UNB-API-STD-003) | RR-UNB-API-STD-003-r1: support tiers, 12-month window, protection of in-flight objects |
| Webhooks and subscriptions | Partial | UNB-XIN-SAS-004: thin events, retry for 72 hours, redelivery for 30 days, termination at the effective time |
| Tenant envelope on events and telemetry | Partial | UNB-XIN-SAS-005 (P0); the basis for isolation proofs, migration equivalence, metering and incident scoping |
| Third-party app registration | Partial | UNB-XIN-SAS-006: registry plus consent, no marketplace |
| Tenant lifecycle | Partial (UNB-TEN-011) | RR-UNB-TEN-011-r1: one state machine with gates |
| Sponsor exit | Partial | RR-UNB-TEN-016-r1: reconciled ExitPackage, read-only retention, key destruction, residuals certificate |
| Partner exit with fund portability | Missing | UNB-XIN-SAS-007 (P0) |
| Disaster recovery by data class | Partial | RR-UNB-RES-003-r1: classes C0 to C4. C0 has RPO 0 and RTO ≤ 4 h, restores to a consistent point, and blocks payments until reconciled |
| SLOs and error budgets | Partial | UNB-XIN-SAS-008; correctness is never budgeted |
| Metering | Missing | UNB-XIN-SAS-009 |
| Sandbox | Partial | UNB-XIN-SAS-010 |
| Subprocessor register | Partial | UNB-XIN-SAS-011 (P2) |
| Audit, security monitoring, observability | Mostly covered (SOC2-*, MON-013) | Operator transparency added via UNB-XIN-IAM-002 |

### 1.3 Serving many sponsors without fragmenting

Each kind of variation gets exactly one mechanism. Anything outside these tiers is a fork and is rejected (XRJ-SAS-001).

| Tier of variation | Mechanism | Owner | Not allowed |
|---|---|---|---|
| Legal and fiduciary | Sponsor policy (REQ-X-045), protected terms | Sponsor and counsel | Weakening platform floors |
| Commercial scope | Entitlements (UNB-XIN-SAS-001) | Contract | Switching off statements or holds |
| Presentation | Brand kit, terminology, view composition | Sponsor or partner | Changing legal meaning |
| Integration | Adapters, app registry, subscriptions | Platform, with sponsor consent | A partner inside the funds flow (UNB-XIN-OTH-003) |
| Isolation | Deployment binding A, B or C | Architecture decision citing the contract | Different code |
| Release mechanics | Expiring feature flags | Engineering | Tenant-specific behavior |

### 1.4 Redesign: partner exit with funds (UNB-XIN-SAS-007, P0)

**Why it matters.** UNB-TEN-016 gives a portability protocol to the partner, not the sponsor. Nothing defines what happens when a whole program exits. That leaves room for an unlawful shortcut: moving funds to a new sponsor by data migration, which breaches the sponsor's exclusive legal control under §4966(d)(2).

| | Conventional | Existing Unbound | Redesigned |
|---|---|---|---|
| Mechanism | The vendor migrates accounts, or makes one bulk grant | Single-donor transfer only | A PartnerExit case. Each fund elects to transfer or stay. Each transfer is an ordinary transfer grant: the advisor recommends and the sponsor decides; a batch policy is allowed. Funds that do not elect stay, with a sponsor-direct credential |
| Control | Contract | Per-transfer sponsor discretion | Sponsor discretion preserved. LegalSponsorId is never updated. No partner in the funds flow |
| Exceptions | Ad hoc | Silent donors and federated-only logins not handled (TVF-135) | Explicit paths for silent, objecting, held and deceased-advisor funds |
| Evidence | Migration log | Per transfer | Per-fund recommendation and decision, plus an exit reconciliation: elected, transferred, declined and remaining must reconcile with zero difference |
| Recovery | Manual | Re-credit | Re-credit per UNB-RCN-GRT-015. Unknown payments resolved before any retry |

Switching happens in practice: Ren iPhi lost 6 of 57 sponsors, and NPT runs 13 programs on one build (daf-landscape:research/DAF-INDUSTRY-REPORT-2026-11.md:425-444).

### 1.5 Automation

This layer is almost entirely deterministic: routing, envelope, entitlements, ring gates, metering and retries.

AI may only explain:
- denials
- release risk
- configuration diffs
- drafts of notices

AI never decides:
- tenant lifecycle transitions
- operator access
- partner-exit transfers, which are a sponsor discretion under §4966(d)(2)

## 2. Identity and access management

**Independent assessment.** A platform like this needs:
- separate workforce and customer realms
- per-tenant SSO with SCIM provisioning
- step-up authentication bound to the specific action
- delegated principals derived from a parent principal
- AI agents as workload principals
- access recertification with automatic revocation
- support access that the customer approves
- break-glass access that is sealed and independent of the identity provider

NIST SP 800-63B-4 is used as a benchmark only (search excerpt).

| Capability | Status | Action |
|---|---|---|
| Delegated and agent principals; propagation of revocation | Covered (UNB-API-003, SOC2-017, GOV-AUTH-003/015, AUTH-I1/I2, UNB-RCN-PLT-004/005, UNB-RCN-ENT-004) | — |
| SSO and SCIM | Covered (CTL-052, AUTH-I8) | — |
| Workforce/customer realm separation | Missing | UNB-XIN-IAM-001 (P0) |
| Operator access under sponsor authority | Exists only as a finding (TVF-134) | UNB-XIN-IAM-002 (P0): sponsor-approved access with a transparency log |
| Step-up matrix | Scattered (CTL-033, AUTH-I7, TVF-136) | UNB-XIN-IAM-003 (P0): sponsor-controlled verifier, bound to the command, maximum age 5 minutes |
| Access reviews | No cadence or consequence | UNB-XIN-IAM-004 (P1): includes bank and custodian signer registers; access auto-suspended 14 days after the due date |
| Break-glass | Exists only as findings (TVS-441/473, TVF-139/228) | UNB-XIN-IAM-005 (P0): protective actions only, two people, expires after 4 hours, tenant notified within 1 hour |

**Where the analogy breaks.** In consumer finance, step-up protects the customer's own money. A DAF donor only recommends, so here step-up protects the integrity of recommendations and the sponsor's payee data. Likewise, a lockbox in cloud services protects confidentiality; here it also protects the sponsor's exclusive legal control.

Buy a managed identity provider. Building in-house customer IAM is rejected (XRJ-IAM-001).

## 3. Records management

**Independent assessment (Sedona practice):**
- an approved schedule of record classes
- legal holds that reach every custodian, including third parties, are monitored, and are released promptly
- disposition by consistently applying the schedule, never ad hoc
- legal production with chain of custody
- crypto-shredding where storage is immutable

**Comparison with Unbound:**
- REQ-X-036/037/038 and RR-REQ-X-036-r1 are strong within the platform.
- Gap: holds stop at the platform boundary (TVS-484). Fixed by UNB-XIN-REC-001 (P0).
- Gap: no profile for legal production. Fixed by UNB-XIN-REC-002.
- Conflict: RR-REQ-X-036-r1 requires per-record review, while RCD-007 batches by class (XIC-REC-001). RR-REQ-X-038-r1 resolves it:
  - batches by class, trigger and period;
  - one independent approval per batch;
  - each record's hold status rechecked at execution, and newly held records skipped.
- Who holds the records after a sponsor exits is undecided (XID-REC-001).

## 4. Workflow automation: workflow engine or in-database state?

| Need | BPMN suite | Durable-execution engine | In-database state machines + outbox + timer table (recommended) |
|---|---|---|---|
| Long-running state | Held by the engine | Held in engine history | Held in the domain aggregate, in the authoritative store |
| Atomic with the ledger | No | No | Yes: one transaction covers state, outbox and the DueObligation row |
| Timers | Timer events | Durable timers | DueObligation rows plus one scheduler per cell (UNB-XIN-WFA-001) |
| Retries and idempotency | Engine | Engine | Outbox plus idempotency keys; an Unknown outcome is never retried blindly (REQ-X-083) |
| Human tasks | Built in | External | Work queue (REQ-X-048) |
| Versioning of in-flight instances | Definition versions | Patching | Definition version per instance plus declared mappings (UNB-XIN-WFA-002) |
| Infrastructure | Engine, its database and its UI | A cluster plus persistence | The managed database only (UNB-TEC-002) |

**Verdict: no engine at release 1** (XID-WFA-001). An engine creates a second durable state that must be reconciled with the ledger, which is the same objection raised in RCF-ARC-004.

Revisit if any of these occur:
- more than 50 long-running process types
- routine orchestration across cells
- scheduler incidents above a threshold

Sponsor-authored BPMN is rejected (XRJ-WFA-002).

## 5. Logistics and fulfillment

Printers reconcile every piece: printed, spoiled, reprinted, inserted, mailed. USPS IV-MTR provides piece-level scans (search excerpt).

The gap is that there is no manifest reconciliation and no rule for returned mail. UNB-XIN-OTH-004 (P1) adds both:
- a spoiled check is voided in positive pay before it is reissued;
- returned mail marks the address as unverified.

**Where the analogy breaks.** A "Delivered" scan proves transit only. It never proves the charity received the check.

## 6. Other industries

| Industry | Valid analogy | Breaks down | Result |
|---|---|---|---|
| 529 plans | Split between trustee, program manager, recordkeeper and distributor; MSRB G-45 disclosure data | The account owner owns the assets and can roll them over; municipal securities regime | REQ-X-064 covers the roles. UNB-XIN-OTH-001 adds a generated program description (P3) |
| HSA | A bank or IRS-approved nonbank trustee holds the assets, with white-label distributors on top; the closest structural twin | The owner has a statutory transfer right; card spend | Supports the "stay unless elected" rule in UNB-XIN-SAS-007. Giving donors a transfer right is rejected (XRJ-OTH-002) |
| Payroll | A new reporting agent ends the prior agent's authority; liability stays with the employer | Statutory remittance schedules | Effective-dated provider authority (AUTH-I6) |
| 401(k) conversions | Blackout window with advance notice (unverified) | ERISA is law; for a DAF this is only a customer obligation | UNB-XIN-OTH-002: 30-day notice, list of frozen actions, notice if the window overruns |
| Escrow and title | Conditional release; call-back verification | Two-party custody | Covered (REQ-X-005, UNB-RCN-GRT-017) |
| Crowdfunding (Reg CF) | The portal never holds funds; a qualified third party does | Securities regime | UNB-XIN-OTH-003 (P0): partners and hosts are never in the funds flow |
| Lottery | Claimant verification; unclaimed prizes | Payees are individuals, with withholding | Nothing new (XRJ-OTH-001) |
| University endowments | Unitized pools; restrictions | Funds are perpetual | Covered (UNB-RCN-FIN-006/007) |

## 7. Operating-model analysis

Legend:
- **C**: common core
- **K**: same code, configured
- **M**: model-specific capability in the same codebase
- **–**: not needed

| Capability | D2C | WL | Adv | Inst | OutAdm | Emb | Inv | HVLB | HNW | Hyb |
|---|---|---|---|---|---|---|---|---|---|---|
| Ledger, grants, payments, holds, retention | C | C | C | C | C | C | C | C | C | C |
| Policy and configuration lifecycle | K | K | K | K | K | K | K | K | K | K |
| Brand kit and terminology | K | M | K | K | K | M | K | K | K | M |
| Entitlements | K | M | K | K | K | K | K | K | K | M |
| Customer realm and step-up | C | C | C | C | C | C | C | C | C | C |
| Partner federation and SCIM | – | M | M | M | K | M | K | K | – | M |
| Advisor delegation across many funds | – | K | M | K | – | K | M | – | K | K |
| Operator access under delegation of authority | K | M | K | M | M | K | K | K | M | M |
| Widgets, API, webhooks | – | K | M | K | – | M | K | K | – | M |
| Custody adapters | K | K | M | K | K | – | M | K | M | K |
| Partner exit with funds | – | M | M | M | – | K | K | – | – | M |
| Metering | C | C | C | C | C | C | C | M (viability) | C | C |
| Complex assets and family governance | K | K | K | K | K | – | K | – | M | K |
| Payroll and batch intake | – | K | – | K | K | – | – | M | – | K |
| Partners excluded from the funds flow | C | C | C | C | C | C | C | C | C | C |

**No operating model needs a separate implementation** (XIF-SAS-017, XID-SAS-001). Isolation profile should not be tied to the operating model.

Model-specific risks:
- **Hybrid** is safe only if tenant = legal sponsor (RCD-004).
- **High-volume, low-balance** viability cannot be judged without per-tenant metering (UNB-XIN-SAS-009).
- **D2C own-brand app** and **embedded charity-side button** may make the platform company itself a California charitable fundraising platform. The sponsor's exclusion does not pass to its vendor (OB-38, XID-SAS-005).
- **Outsourced administration** and **white label** are unlawful without UNB-XIN-IAM-002.

## 8. Input for economics: per-tenant cost attribution

**Source.** The UNB-XIN-SAS-005 envelope (TenantId, ProgramId, partner) on every usage line.

**Metered units (UNB-XIN-SAS-009):**
- authoritative storage
- document bytes by retention class
- API calls by tier
- compute seconds
- AI calls, by model and tokens
- billable provider units: payment rails, sanctions and charity data, custody, print and mail pieces, SMS and email
- human work-item minutes

**Allocation.**
- Shared infrastructure is allocated monthly using versioned keys, and must reconcile to invoices within 5%.
- Isolation tiers B and C are directly attributable.
- Some costs outlive revenue: post-exit read-only retention, partner-exit operations and parallel runs during migration.
- Metering never feeds fee accrual or authorization.

## 9. Input for event architecture: tenant isolation of events

1. **Every event carries an envelope:** TenantId, LegalSponsorId, ProgramId, a tenant-scoped sequence number, CorrelationId and CausationId. An append without a tenant is rejected.
2. **No correlation across tenants.** The only exceptions are declared inter-cell contracts carrying both tenants' references:
   - global holds, with their publication version
   - shared charity facts
   - transfer grants from one sponsor to another

   Tracing and support tools never join data across tenants.
3. **Tenant migration** preserves event IDs, sequences and envelopes byte-for-byte. Outbox handoff is replay-safe at a watermark. Holds propagate, and data in the old cell is preserved while any hold is active. In-flight process instances keep their definition versions.
4. **External subscriptions** receive thin events and terminate together with the subscriber's authority. Nothing is shown as "Settled" before reconciliation.
5. **Timers** (DueObligations) are scoped to a tenant. After a restore, each fires exactly once, and only after reconciliation.

## 10. Priorities

| Priority | Items |
|---|---|
| P0 | UNB-XIN-SAS-005, UNB-XIN-SAS-007, UNB-XIN-IAM-001, UNB-XIN-IAM-002, UNB-XIN-IAM-003, UNB-XIN-IAM-005, UNB-XIN-REC-001, UNB-XIN-OTH-003, RR-UNB-TEN-016-r1 |
| P1 | UNB-XIN-SAS-001, -002, -003, -010; UNB-XIN-IAM-004; UNB-XIN-REC-002; UNB-XIN-WFA-001, -002; UNB-XIN-OTH-002, -004; RR-UNB-RES-003-r1; RR-UNB-TEN-011-r1; RR-REQ-X-038-r1; RR-UNB-API-STD-003-r1 |
| P2 | UNB-XIN-SAS-004, -006, -008, -009, -011 |
| P3 | UNB-XIN-OTH-001 |

## 11. Evidence not obtained

**Read only as search summaries (medium or low confidence):**
- MSRB G-45
- IRS nonbank HSA trustee approval
- NIST 800-63B-4
- the SRE error-budget policy
- USPS IV-MTR
- Sedona legal holds (the edition found is marked superseded)
- IRS reporting agents
- 17 CFR 227.303

**Unverified general knowledge:** ERISA blackout notices, UPMIFA, lottery practice, and whether the Temporal .NET SDK is current.

**No supporting evidence or decision yet:**
- No customer-demand evidence for entitlements, sandboxes or webhooks.
- No counsel input on XID-SAS-002, XID-SAS-005 or XID-REC-001.
- RTO and SLO values are proposals.
- Strata and Forma capabilities were taken from reconciliation report 13 and not re-verified.

# Enterprise governance and integration consolidation, adversarial challenge, dashboard/report handoff
Date: 2026-11-23. Proposed requirements, no implementation or tests executed.

## Four requirements packages
1. ENTERPRISE-DECISION-RIGHTS-AND-DELEGATED-AUTHORITY-2026-11-23.md
2. SPONSOR-CONFIGURATION-POLICY-LIFECYCLE-AND-ISOLATION-2026-11-23.md
3. ENTERPRISE-DATA-CONTRACTS-PROVIDER-INTEGRATION-AND-EVIDENCE-GOVERNANCE-2026-11-23.md
4. ENTERPRISE-RESILIENCE-CONTINUITY-AND-PROTECTIVE-OPERATIONS-2026-11-23.md

## Consolidation and ownership
Keep a single shared authorization/decision-rights evaluator rather than new per-domain approval engines; domain components still own business-specific authorization and state transitions. Keep a shared sponsor configuration registry, but do not centralize all policy semantics into an unbounded generic rule engine. Shared provider integration contracts and mock tooling, but adapters remain owned by domain modules. Shared protective hold semantics and authoritative safety epochs, not a second financial ledger or a general-purpose emergency payment service. Use the existing Exception Management and Operational Learning components rather than duplicating queues and analytics in each new package.

## Explicit challenge and refinements
- **Decision rights:** role-based access alone is insufficient; bind authorization to decision type, tenant, fund, amount, delegation period, evidence and segregation of duties. Emergency privilege must not confer payment authority.
- **Policy precedence:** a simplistic linear priority can be wrong when law and sponsor governing documents conflict; typed conflict detection and authorized resolution are required. Financial seven-year floor and protective holds cannot be shortened.
- **Sponsor customization:** overflexibility creates hidden forks; permit only typed validated choices with versioned migration and explicit platform invariants. White-labeled words must preserve legal meaning.
- **Integration architecture:** 'exactly once' provider execution is not universally achievable; enforce internal idempotency and reconcile ambiguous external status. Distinguish accepted from settled.
- **Resilience:** zero RPO can be promised only within specific durability assumptions, not any conceivable region-wide catastrophe. Availability must not trump safe money/hold state.
- **Cross-sponsor analytics:** aggregated data can still reidentify small cohorts; privacy thresholds and aggregation controls required.
- **Automation:** reduction in human touches is beneficial only if false approvals, false holds and material losses do not increase.
- **Financial truth:** dashboards, AI agents, integrations and case management must not post directly to GL or replace approved close evidence.

## Proposed adversarial tests (not executed)
- ADV-579: expired grant delegation used; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-580: same person prepares and approves close; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-581: platform super-admin releases sponsor payment; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-582: AI overrides compliance decision; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-583: authority revoked between approval and execution; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-584: amount split to evade approval threshold; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-585: sponsor policy overrides global charity hold; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-586: policy conflict resolved by naive numeric priority; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-587: fee change retroactively alters issued statement; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-588: grant terminology hides required affirmation; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-589: recurring grant executes under expired policy; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-590: policy rollback silently changes pending grant; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-591: cross-tenant template asset leaked; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-592: sponsor offboarding deletes seven-year evidence; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-593: external payment timeout retried blindly; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-594: provider webhook replay posts duplicate transaction; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-595: custodian feed overwrites GL without reconciliation; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-596: external ID collision merges two grants; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-597: mock adapter calls live payment endpoint; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-598: provider schema change drops currency; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-599: out-of-order callbacks mark settled payment pending; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-600: signature-invalid webhook accepted; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-601: stale provider data shown as current; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-602: provider credential exposed in admin UI; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-603: database split brain authorizes duplicate payments; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-604: stale replica used to clear hold; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-605: outage queue shown as approved; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-606: emergency plane sends money; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-607: archive restored without decryption keys; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-608: backup restores altered PDF bytes; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-609: regional failover loses acknowledged commit outside declared assumptions; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-610: incident containment leaks cross-sponsor records; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-611: provider Unknown ignored during restart; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-612: protective freeze removed without authorized evidence; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-613: single dashboard metric masks material financial mismatch; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-614: exception queue duplicated across domains; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-615: policy registry permits executable tenant code; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-616: legal hold and payment send race; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-617: financial document purged after seven years despite active hold; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.
- ADV-618: AI changes financial rule after positive experiment; expected: explicit authority, safe policy composition, provider evidence, protective isolation and honest recovery.

## Next phase: dashboards and reports, explicitly prioritized
**Begin after this governance consolidation.** Produce dedicated requirements, not just visual mockups:
1. Role-based dashboard information architecture for platform executive, sponsor executive, CFO/controller, compliance, operations, advisor/donor, board and auditor.
2. Configurable dashboard layout, permissions, persistent filters, accessible responsive widgets, saved views, drilldowns and actionable next-best steps.
3. Reusable Forma widgets and safe external embedding: tenant-scoped token claims, read-only versus domain-command actions, cross-origin isolation, branding and localization.
4. Complete report catalog: financial, AUM, grant activity, charity compliance, sponsor operations, donor engagement, board, regulatory/audit and unit economics.
5. Report designer and scheduling: governed semantic metrics, templates, period snapshots, approvals, distribution channels, recipient authorization, revision/correction history and seven-year financial retention.
6. Reporting correctness: source provenance, as-issued/as-known-at, approved versus estimated figures, freshness, reconciliation and export validation.
7. Adversarial UX review: information overload, empty/loading/error states, mobile and assistive technology, small-cohort privacy, permission changes, misleading status and unsafe actions.

## Status
REQ-X-044..047 proposed. ADV-579..618 proposed and unexecuted. This is requirements documentation, not a production-ready system or legal certification.

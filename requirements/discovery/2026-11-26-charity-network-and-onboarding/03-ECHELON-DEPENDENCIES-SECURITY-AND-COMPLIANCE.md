# 03 — Echelon dependencies, security and compliance implications

Status: **proposed**. Component facts come from `requirements/convergence/2026-11-24-master/07-ECHELON-CAPABILITY-ASSESSMENT.md` and CNV-ARC-004. Echelon repositories were not modified.

## Echelon component dependencies

| Component | Verified state (07 assessment) | Used by | Gap or decision |
|---|---|---|---|
| Strata v0.1.1 (pilot) | Declarative PostgreSQL 16 schema governance. It does **not** provide append-only history, temporal queries or immutable posting. Mock-interaction virtualization (STRATA-DIV-001) is proposed, not implemented | DDL governance for registry, migration, certificate and configuration tables | Temporal claims (UNB-CNET-001), the legacy partition (UNB-ONB-007) and provider mocks (UNB-ONB-025) are **Unbound-owned** under CNV-ARC-004. Adopt STRATA-DIV when it exists |
| Forma 1.1.0 (CSS/HTML design system) | 174 components; 28 required by its own catalogue but not implemented; channel installs 1.0.1 | Charity claim and remittance pages, migration readiness and reconciliation dashboards, configuration review | Repin to 1.1.0 (existing finding). Reconciliation-diff and evidence-viewer patterns may be among the missing components |
| Folio 0.4.x (print) | Passive print elements; 0.4.2 untagged | Readiness reports, migration outcome reports, auditor evidence packs, charity remittance advice | A tagged release is needed before verification by digest |
| Limen 0.9.x | TypeScript kernel with an F# seam; no embed mount; no IdP or session capability | Participant UIs | Embedded charity and advisor widgets are unsupported (existing risk) |
| Fides 0.2.0 | GitHub is the only IdP | — | Not usable for donors, advisors or charities. Federation (UNB-ONB-027) and SCIM (UNB-ONB-028) are Unbound build scope |
| Dokimos 0.11.1 | Manifest-driven verification; no financial-invariant runner | Acceptance evidence for UNB-CNET and UNB-ONB acceptance criteria | Unbound's F# property tests (reconciliation invariants, temporal replay) must feed it |
| Praxis v3.25.0 | Requirements governance | Converting these records to the Praxis catalogue when the baseline moves | No new need |
| Conditor 0.10.x | Installs Echelon components | Not involved in sponsor activation | The Sponsor Configuration Package (UNB-ONB-023) is an Unbound artefact, not a Conditor manifest |
| Vigila, Chrona, Summa | Untagged, greenfield or planned | — | No Release 1 dependency |

**Net:** no new Echelon capability is required for Release 1. These are all Unbound build scope:

- the temporal claim store;
- the migration pipeline;
- the certification runner;
- provider mocks;
- federation and SCIM.

Proposed upstream items, for the Echelon owners and not acted on here:

- STRATA-DIV-001 virtual time and fault injection would replace Unbound-owned mocks.
- Forma reconciliation-diff and evidence-viewer patterns.
- A tagged Folio release.

## Security implications

| Area | Implication | Requirements |
|---|---|---|
| Destination fraud (account takeover followed by a bank change) | The dominant observed loss pattern. Controls stack: the CNV-PAY-002 evidence combinations (c10), contact holds (c11), risk tiers with first-payment caps and probation, and owner-change events from partners | UNB-CNET-014, -015, -016, -017, -018; CNV-PAY-002; CNV-PLT-011 |
| Data poisoning and malicious submissions | Lower-authority claims can dispute but never replace higher-authority ones. Mass-change circuit breaker on source releases | UNB-CNET-002, -021 |
| Prompt injection from charity websites and uploads | AI-extracted contacts, bank details and rosters are proposals only | UNB-CNET-022; CNV-AIO-002 |
| Compromised or spoofed source feeds | Integrity gates; immutable, hashed release archive; quarantine | UNB-CNET-003, -021 |
| Cross-tenant inference | Universe-wide monitoring; requester identity kept in sponsor cells; inference tests in the SharedStoreRegister | UNB-CNET-010; CNV-TEN-001 |
| Correlated risk from shared verification | Bank-destination evidence stays per sponsor in Release 1; sharing is deferred pending counsel and loss allocation | OD-CNO-05; UNB-CNET-014 |
| Migration chain of custody | Hashed source extracts; deterministic manifests; isolated rehearsal cells with no outbound effects; legacy-imported destinations not payable until verified | UNB-ONB-006, -009, -010, -011; UNB-CNET-016 |
| Duplicate external effects at cutover | Ownership register; cross-system idempotency keys; check-range partitioning; irreversibility boundary | UNB-ONB-017, -018 |
| Pre-contract data exposure | Exposure tiers; T1 takes no row-level personal data; T2 only under NDA/DPA with deletion | UNB-ONB-003 |
| Configuration as an attack surface | Credentials are never in the package; independent approval; legal-perimeter validation; no sponsor forks | UNB-ONB-023, -024; UNB-API-STD-005 |
| Integration counterparties | Scoped, expiring certificates; shadow-live before write; negative authentication and replay tests | UNB-ONB-025 |

## Compliance implications

| Obligation | Status of evidence | Requirements |
|---|---|---|
| Nacha 2026 risk-based fraud monitoring (Phase 2, effective 19/22 June 2026) for TPSPs, including False Pretenses credits | Rule text verified. Allocation between Unbound and a transmitting partner is a counsel question (OD-CNO-12) | UNB-CNET-023 |
| IRS reliance: Rev. Proc. 2018-32 (posting dates, subordinates, subsequent listing, §8.01 third-party reports) | Sections verified verbatim. The §4.06 reading for about 19,700 Pub 78 EINs is a counsel question (OD-CNO-13) | UNB-CNET-001, -005, -006, -008 |
| OFAC screening | Cadence verified; full refresh advised | UNB-CNET-009; CNV-GRT-006 |
| Source licensing (US public domain, OGL, ODbL share-alike, no-resale terms) | Verified per source | UNB-CNET-011, -024 |
| Donor privacy in pre-contract assessment (GLBA-like expectations, state privacy laws, DPA) | Design-level | UNB-ONB-003 |
| Records retention for imported history | Retention runs from the source transaction date (CNV-PLT-004) | UNB-ONB-009 |
| Auditor reliance during the first sponsor's period | No Unbound SOC 1 Type II yet; evidence pack and Type I at go-live | UNB-ONB-015 |
| Notice of operational freezes | ERISA blackout-notice analogue (30-60 days for freezes longer than 3 business days) is a precedent, not a DAF rule | UNB-ONB-016 |
| Identity proofing for migrated and federated participants | CNV-PLT-003 governs; federated reliance needs counsel (OD-CNO-11) | UNB-ONB-022, -027 |
| Regulatory perimeter (no custody, no unilateral payment control) | Activation conformance checks | UNB-CNET-013, -018; UNB-ONB-023; CNV-LEG-001 |

Agent research is not legal advice. Every item marked as a counsel question needs qualified review before activation.

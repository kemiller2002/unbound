# Requirements expansion backlog and completion gates
Date: 2026-11-21 | Status: requirements planning, not implementation

## Mission
Continue defining the full AI-native DAF enterprise so implementation can begin when requirements and evidence gates pass. Preserve the outcome, not the legacy workflow. No application scaffold or production code is authorized by this backlog. Do not infer legal conclusions from vendor designs or unverified research.

## Binding decisions already recorded
- Unbound is an independently branded EF-built reference enterprise, with F# modular-monolith, WASM-first design subject to verified EF compatibility.
- Strata is a required persistence architecture component; its actual capabilities must be verified before implementation.
- Authoritative records are append-only, with explicit correction, reversal and supersession events, not in-place changes.
- Partners may use separate schemas, separate databases or dedicated infrastructure; isolation applies to every data surface.
- Funds support more than two participants, independently scoped permissions, and sponsor-governed successor advisors and successor DAF accounts.
- Sponsoring charity retains legal control of assets and distribution decisions; advisors recommend, not instruct.

## Expansion tracks, in priority order

### R1. Parties, funds, family governance and succession
Define natural/legal persons, households, entities, organizations, relationships, legal sponsor, institutional partner, advisor channel, fund and fund family. Include invitation, KYC/identity where applicable, multiple advisory participants, role overlap, delegated permissions, concurrent conflicting recommendations, consent, privacy, beneficiary charity preferences, restrictions, donor anonymity, donor death/incapacity, disputed instructions, contingencies, successor-of-successor, splits, fund closure, consolidation, transfers, dormant funds, historical donor recognition and minors. Model whether a successor fund is an internal allocation versus external sponsoring-organization transfer.

### R2. Partner and sponsor enterprise operating models
Define sponsor of record, partner distributor, sub-advisor, infrastructure provider, custodian, payment provider, grantee and regulator; contractual responsibility, licensing and obligations per role. Define tenant identity, isolation tier, provisioning, sponsor-shared reporting, partner-specific brand/policies/fees, tenant export, migrations, incident boundaries, data controller/processor responsibilities, third-party access and partner exit. Compare models C/D with a full AI-native sponsor; do not prematurely choose.

### R3. Contribution, asset, accounting and disbursement lifecycle
Define cash, public securities, restricted securities, complex/illiquid assets, crypto acceptance policies, valuations, settlement, units/NAV, fees, investments, liquidity, internal transfers, grants, returns, reversals, payment failures, reconciliations, accounting close, fund splits and transfers, idempotency, duplicate payment prevention, restrictions and charity eligibility. Every financial operation needs balanced immutable entries, event causality, independent confirmation, temporal semantics and exceptions.

### R4. Authority, compliance, policy and AI governance
Build exact per-regulatory-assertion mapping to control owner, evidence, applicability, sponsor/provider responsibility, effective date, counsel verification and negative test. Define policy-as-code, version approval, human reserved decisions, delegated agents, model/tool authorization, separation of duties, prompt injection defenses, adverse-event handling, sanctions and benefit conflicts, incident response, appeals, auditing, privacy/retention versus immutable records, board oversight and jurisdiction-specific review.

### R5. Experience, communication and accessibility
Define donor/advisor/sponsor/partner/board/grantee personas and journeys. Cover accessible onboarding, joint participation, mobile, account discovery, contribution tracking, grant recommendations, donor goals, fund statements, investment preferences, successor planning and preview, advisor dashboards, sponsor operations, exception resolution, notifications, documents, privacy settings, donor intent and accessibility for assistive technology. Ensure polished Unbound branding and no exposed internal EF jargon in donor-facing experiences.

### R6. Knowledge, integration and technology
Define Strata's verified integration contract, EF capability discovery, Conditor install gate, append-only event schema, bitemporal corrections, projections, evidence provenance, privacy boundaries, knowledge-store compilation if verified, partner APIs, external charity and custodian integrations, data residency, secrets, backup/restore, event integrity, disaster recovery, performance, observability, versioning and tenant migration. Prefer managed services and simple deployment; document tradeoffs.

### R7. Business, pricing, economics and organizational design
Define sponsor fees, partner pricing, infrastructure licensing, account minimums, unit economics, cost of controls, AI exception costs, liquidity, staffing/accountability, service levels, sales/onboarding, partner contracts, commercialization, customer support, marketing, philanthropy impact, and sponsor/partner operating model economics. Distinguish measured evidence, assumptions, benchmarks and hypotheses.

### R8. AI-native workflow innovation and experiments
For each outcome, identify the legacy process, legal/safety invariant, at least three different mechanisms, potential elimination of handoffs, explicit human authority, observable evidence and quantitative test. Include agent failure, false positives/negatives, vendor outage, counterparty risk, operational surge, cost sensitivity and no-AI fallback. Track EF autonomous build/run capability separately from DAF operating metrics.

## Requirement record template
Each approved candidate SHALL include: stable ID; outcome and affected personas; scope and legal jurisdiction; sponsor/partner responsibility; requirement statement; mandatory versus hypothesis classification; provenance and source freshness; data/event dependencies; authority/control owner; normal, exception and adversarial cases; measurable acceptance test; privacy/accessibility implications; alternatives; contradictions; status; review owner and approval evidence.

## Completeness checks before implementation
1. Every one of the 24 landscape workflows and 29 capabilities maps to at least one Unbound requirement, decision or explicit rejection.
2. All 65 source regulatory assertions have individual applicability and control mappings; unverified legal assertions remain clearly flagged.
3. All 54 source controls and 58 events have mapped disposition; all 22 source scenarios have disposition and acceptance criteria.
4. All fund/participant/successor/partner combinations have positive and negative access and financial-invariant scenarios.
5. Every requirement has an evidence source or is explicitly labeled a user product decision, assumption or hypothesis.
6. All mandatory architecture decisions, including Strata and append-only, have verified compatibility evidence or are blockers.
7. Contradictions are recorded with owners and resolution; no unresolved safety/legal contradiction is silently waived.
8. Implementation begins only after authorized requirements approval, Conditor bootstrap and reproducible technical validation.

## Proposed next authoring wave
**Wave 6: Full fund and relationship lifecycle.** Expand R1 into individually testable requirements and a state/authority matrix, including successor-account splits, joint participants, restrictions, disputed succession and tenant boundaries. In parallel, build the regulatory-control crosswalk and contradiction register. These are requirements artifacts, not executable implementations.

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-ECO-001 — Cost attribution, metering and benchmarks:** Unbound SHALL meter causal cost drivers (events, storage, compute, AI inference) and maintain non-posting cost attribution reconciled to invoices with absolute and relative residual tolerances per source; AUM-based allocation SHALL be used only when justified; showback SHALL be transparent and chargeback activated only for executed contracts, without double allocation. Finance SHALL own the attribution model and savings SHALL be verified independently; moving a cost to another bearer SHALL be reported as a transfer, not a saving. Cost ingestion SHALL map to FOCUS 1.4 as the internal target with version-aware adapters for providers on 1.2 and 1.3, preserving native invoice provenance with extensions x_Bearer, x_SourceInvoiceRef and x_CostObject, without claiming full FOCUS conformance. Cross-sponsor benchmarks SHALL use experimental suppression thresholds (at least five sponsors, at most 60% dominance) labelled as not an anonymity guarantee, with differencing, longitudinal and collusion tests, including staff metrics. Economic ranges SHALL be labelled scenarios until pilot observations replace them. Release-1 economics SHALL include initial metrics, AI metering, a monthly economics pack and reconciled non-posting attribution.
  - Provenance: resolves XID-ECO-001, XID-ECO-004, XID-SAS-003, UNB-XIN-SAS-009#Q1, UNB-XIN-ECO-001#Q1, XID-ECO-009, UNB-XIN-ECO-005#Q1, XID-ECO-006, SPD-DSH-002, UNB-XIN-ECO-021#Q1, UNB-XIN-ECO-003#Q1, XID-ADV-011; amends UNB-XIN-SAS-009, UNB-XIN-ECO-001, UNB-XIN-ECO-005, UNB-XIN-ECO-021, UNB-XIN-ECO-003. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Economics; financial system of record: n/a (non-posting attribution); policy owner: Sponsor CFO / Unbound CFO for platform costs; data owner: Unbound; operational owner: Unbound finance; evidence owner: Unbound finance.
  - Acceptance:
    - AC-1: Given a cost source without calibration, when attribution runs, then residuals are reported and not written off.
    - AC-2: Given a benchmark cell with four sponsors, when rendered, then it is suppressed.
- **CNV-ECO-002 — Pricing, quotes, small balances and economic targets:** Donor-facing quotes SHALL be shown for charged operations; any amount above the donor's accepted maximum SHALL require consent or be absorbed by the sponsor; required accessibility accommodations SHALL carry no fee. Small-balance pricing (minimum fees, digital-minimal fees, employer sponsorship, disclosed cross-subsidy) SHALL be modelled and SHALL never deny mandatory service; minimum grant sizes SHALL be a sponsor policy, not a universal restriction. Employer-subsidized terms SHALL be advertised only after an enforceable funding commitment. High-volume low-balance books SHALL be architected for but activated only with a positive contribution margin at adverse licence scale and a mandatory service floor whose metric and minimum are defined before any number is approved (TEV-010, TEV-011). Cost targets SHALL be expressed per operating model and servicing class as base, favourable and adverse ranges separating marginal, fully allocated and bearer costs; no single universal target SHALL be approved. Unbound licence prices SHALL derive from fully allocated and marginal cost scenarios with transparent tiers by servicing class and sponsor size only where cost supports them. Shared foundation costs SHALL be allocated to a community-foundation DAF program by documented driver (TEV-002). Household fee-tier aggregation SHALL be opt-in with privacy and consent.
  - Provenance: resolves XID-ECO-007, UNB-XIN-ECO-008#Q1, SPD-ECN-002, UNB-SMP-ECN-001#Q2, UNB-SMP-ECN-003#Q1, XID-ADV-002, UNB-SMP-ECN-002#Q1, SPD-ECN-001, SPD-CLR-001, UNB-SMP-ECN-004#Q1, UNB-SMP-ECN-004#Q2, XID-ERP-004, SPD-CMP-001, SPD-CMP-002; amends UNB-XIN-ECO-008, UNB-SMP-ECN-001, UNB-SMP-ECN-002, UNB-SMP-ECN-003, UNB-SMP-ECN-004. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: commercial-availability; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor CFO approval of fee schedules and disclosures.
  - Ownership: domain owner: Economics; financial system of record: n/a (non-posting attribution); policy owner: Sponsor CFO / Unbound CFO for platform costs; data owner: Unbound; operational owner: Unbound finance; evidence owner: Unbound finance.
  - Acceptance:
    - AC-1: Given an operation whose final cost exceeds the donor's accepted maximum, when executed, then consent is requested or the sponsor absorbs the difference.
    - AC-2: Given an employer subsidy without an executed commitment, when a campaign is published, then publication is refused.
- **CNV-EVB-001 — Empirical validation backlog items:** The following SHALL be measured before their dependent values are treated as validated: exception incidence and handling time (90-day time study), December interval arrivals, custodian and bank feed content, bank return-file layouts, practical capacity by role, employer willingness to pay, and California registry data access. Until measured, dependent values SHALL be labelled scenario assumptions and SHALL NOT be presented as observations.
  - Provenance: resolves RR-UNB-GRT-003-r1#Q1, UNB-XIN-SVC-008#Q1, UNB-XIN-ECO-017#Q1, XID-ECO-003; amends —. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: evidence-backlog; release scope: architectural-support; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Empirical measurements listed.
  - Ownership: domain owner: Economics; financial system of record: n/a (non-posting attribution); policy owner: Sponsor CFO / Unbound CFO for platform costs; data owner: Unbound; operational owner: Unbound finance; evidence owner: Unbound finance.
  - Acceptance:
    - AC-1: Given a dashboard figure derived from an unmeasured parameter, when displayed, then it carries the 'scenario assumption' label.
<!-- END ADOPTED: convergence-2026-11-24 -->

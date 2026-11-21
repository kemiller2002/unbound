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

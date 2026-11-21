# Unbound Enterprise Requirements Charter
Status: Draft v0.1 | Date: 2026-11-21 | Phase: Requirements discovery only

## Mission
Design an independently branded, disruptive, AI-native donor-advised fund (DAF) reference enterprise from first principles. Unbound is a working reference enterprise and EF capability experiment, not a claim that a regulated charitable sponsor already exists.

## Nonnegotiable principles
1. Specify business outcomes and constraints before workflows, departments, interfaces, or code.
2. Distinguish legally required controls from sponsor policy, market convention, and unsupported assumptions.
3. Retain lawful sponsor control, fiduciary accountability, authorization, privacy, and auditable financial records. Never treat an AI recommendation as legal authority.
4. Research, validate, and compare at least three genuinely different operating models using identical scenarios and assumptions.
5. Use synthetic data and simulated money movement until external integrations, controls, and authorizations are independently validated.
6. Keep research in daf-landscape; cite its source artifacts and provenance rather than copying unsupported conclusions.
7. Record human interventions, agent actions, failures, cost, time, and evidence as an EF capability experiment.
8. No application implementation until the requirements baseline passes a formal review gate.

## Requirements domains
- UNB-ENT: mission, strategy, markets, business model, economics, organization and decision rights
- UNB-LEG: sponsor status, law, tax, fiduciary obligations, charity eligibility, regulatory change, jurisdiction
- UNB-DON: donor identity, onboarding, contributions, intent, communications, privacy, accessibility
- UNB-GRT: grant recommendations, eligibility, diligence, approval, payment, exceptions, outcomes
- UNB-AST: cash, securities, noncash assets, custody, liquidation, investment operations
- UNB-FIN: double-entry accounting, fees, controls, reconciliation, statements, audit and tax reporting
- UNB-ADV: advisor relationships, delegated authority, consent, integrations
- UNB-AIO: agents, automation boundaries, explainability, model risk, human review and fallback
- UNB-OPS: vendor operations, people, risk, incident response, disputes, continuity
- UNB-TEC: architecture, security, identity, data, APIs, interoperability, resilience
- UNB-EXP: brand, accessibility, donor/advisor/charity/administrator experience
- UNB-VAL: scenarios, tests, evidence, economic benchmarks, failure injection and EF portability

## Requirement record
Each requirement MUST have: stable ID; outcome; rationale; type (law/policy/business/design hypothesis); source citation and freshness; jurisdiction if applicable; assumptions; dependencies; accountable decision owner; measurable acceptance criteria; positive, negative and exception scenarios; risk/severity; validation method; status; and revision history.
Unverified legal claims remain hypotheses pending qualified review.

## Echelon Foundry integration discovery
Before selecting dependencies, inspect the latest authoritative default branch, published releases, current registry catalog/channel, compatibility manifests, AGENTS.md, and relevant tests for every discoverable EF system. Produce a dated capability matrix listing canonical name, repository, installed/released version, maturity, intended role, interface, dependency classification, evidence, and any blockers. Do not assume the newest commit is a stable release.

Use all applicable EF capabilities across discovery, governance, implementation planning, validation, and future operations, but do not force every EF system into the production runtime. Honor Registry's capability-based interoperability and optional-provider isolation; use receiving-system integration boundaries for cross-system writes.

Candidate systems to investigate include Praxis, Registry, Conditor, Ordo, Clarity, EDF, Aegis, Dokimos, Vigila, Fides, Knowledge Store, Limen, Forma/Studio, Folio, Percepta, Iter, Arca, Tutors, Vitium, and other current catalog entries. The catalog, not this illustrative list, determines completeness.

Knowledge Store currently describes its schema as research-stage, with Markdown/front matter canonical and compiled views under investigation. Do not claim a stable F# requirement schema exists. Prefer current supported canonical formats, with future compiler integration behind a compatibility boundary.

## First research backlog
1. Map actual DAF legal structure, sponsor authority, tax treatment, contribution types, prohibited benefits, grant restrictions, oversight and state/federal variation; retain citations and expert-review flags.
2. Inventory every end-to-end business capability and boundary, including rarely occurring exceptions and failure modes.
3. Separate obligations from legacy operating conventions, then propose eliminations and alternative outcome-based mechanisms.
4. Develop three operating models: radically autonomous within lawful boundaries; human-governed AI orchestration; API-first networked philanthropy. Add novel alternatives as evidence warrants.
5. Test models against representative synthetic donor, grant, complex-asset, investment, compliance, fraud, dispute and reconciliation scenarios.
6. Define economics: cost per funded account, contribution, grant, exception and dollar administered; model assumptions and sensitivity.
7. Define the experience contract for donors, advisors, grantees and operators, including WCAG accessibility.
8. Establish an EF capability ledger for autonomy, rework, evidence, runtime cost, human interventions and portability.

## Requirements gate
Baseline is NOT approved until all domains have documented coverage, critical scenarios have normal/exception/failure tests, legal assertions have traceable authoritative evidence and review flags, decisions have owners, contradictions are adjudicated, operating models are compared on common criteria, and unresolved blockers are explicitly classified. Passing the gate authorizes detailed architecture, not automatically production operations.

## Immediate deliverables
- Requirements inventory and domain-specific specifications
- Evidence and assumption register
- Regulatory applicability and human-authority matrix
- Enterprise capability and end-to-end scenario maps
- Three competing operating models and comparative evaluation
- Current EF system/version/capability matrix
- Traceability and validation plan
- Open questions, risks and requirements readiness report

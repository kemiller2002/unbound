# Continuous Assurance UX, Forma Widgets and Folio Reports
2026-11-22 | UI requirements only.

## Design goals
The experience should answer: What needs attention? What changed? What evidence proves it? Who can act? What happens next? Avoid a wall of green compliance badges. Prefer a clear action queue, time-window context, uncertainty states and accessible drilldowns. Use Echelon Balanced Charcoal and Bronze design tokens once verified, responsive mobile-first Forma components, Limen WASM, WCAG 2.2 AA target, keyboard and screen-reader support, low-motion modes, nonblank loading states and meaningful error recovery.

## Primary screens
1. **Executive assurance overview**: material risks, open critical deficiencies, overdue controls, evidence gaps, current reporting period, sponsor/partner selector, trend and last verified timestamp. Never present a single unsupported compliance score.
2. **Control owner workspace**: assigned controls, next evidence due, exception queue, failed checks, remediation plan, required attestations and deadlines.
3. **Compliance investigation workbench**: case timeline, source event chain, contradictory evidence, root-cause grouping, legal escalation, independent review and immutable decisions.
4. **Financial assurance**: grants, NAV, investment positions, GL, custodian/bank reconciliation, payment instruction simulation and unresolved breaks with source independence indicators.
5. **Agent assurance**: tool-call permissions, denied escalations, prompt-injection incidents, model/policy version, human overrides and bounded autonomy.
6. **Charity assurance**: identity uncertainty, source freshness, legal rule applicability, sanctions/eligibility holds and evidence provenance.
7. **Auditor workspace**: scope, periods, evidence request tracker, sampling, access expiry, disclosures, exceptions, reproducible Folio package previews.
8. **Control detail**: objective, obligation, owner, procedure, independent evidence, expected-vs-observed graph, latest evaluations, tests, exceptions, dependencies and policy versions.
9. **Evidence detail**: producer, timestamp, digest verification, lineage, redaction, permitted access, original vs corrected observations.
10. **Exception case**: severity, impact, timeline, accountable owner, allowed next actions, reviewer, remediation evidence and retest.

## Status representation
Show separate pills/text for Design, Operating Evidence, Testing and Auditor Review. Use neutral/accessible colors plus icons and explicit text; no color-only meaning. A missing event must read "Evidence missing", not "Passed". A control with a pending reviewer must read "Awaiting review". Show denominators, period and scope beside every rate.

## Reusable embeddable widgets
ControlHealthSummary, EvidenceCompleteness, MyExceptions, PendingApprovals, RiskTrend, GrantAssurance, NAVReconciliation, CharityEligibilityHold, AgentBoundaryViolations, AuditReadiness, ControlTimeline and EvidenceLineage. Each has typed input/output contracts, explicit scope, loading/error/empty/stale states, accessible actions and drilldowns. Widgets may embed in internal Forma apps or authorized third-party systems; embed tokens must be short-lived, audience-bound, tenant-bound, least-privilege, revocable and non-replayable. Never expose raw evidence by default.

## Widget API requirements
Versioned manifest and schema, capability discovery, allowed filters, date range, refresh cadence, localization, theme adaptation, accessibility labels, permission declarations, host-origin validation, telemetry, correlation IDs and rate limits. Sensitive actions require host-independent reauthorization and never inherit trust solely from the embedding page. Prefer read-only embeds by default. Require contract tests for scope changes and cross-tenant denial.

## Interaction flows
Executive alert -> control detail -> deficiency -> accountable owner -> remediation -> independent retest -> closed with historical evidence.
Auditor request -> scoped search -> selected verified evidence -> redaction preview -> independent approval -> Folio export -> export integrity verification.
Agent failure -> denied tool call -> exception -> risk triage -> sponsor human decision -> controlled restart.
Every flow must have explicit recovery for offline, expired authorization, delayed evidence, partial failures and accessible mobile interaction.

## Folio output packages
Control register, scope and system description draft, risk-to-control matrix, requirements-to-control traceability, evidence inventory, population and sample manifest, exception history, operating effectiveness results, management responses, change log, vendor/subservice boundaries, unresolved gaps and limitation statement. Every page and export must identify period, scope, snapshot version and status; synthetic/test evidence must be segregated. Folio must not issue or simulate an independent CPA's SOC 2 opinion.

## UI acceptance scenarios
- User can distinguish missing evidence from passing evidence without relying on color.
- All headline metrics reveal numerator, denominator, scope, last evaluation and exclusions.
- A reviewer cannot approve their own remediation.
- Switching partner or sponsor clears prior tenant data before new data loads.
- Embed expires safely and does not leak cached evidence.
- Mobile screen reader can navigate from critical alert to evidence and authorized next action.
- Offline or stale data is prominently labeled and cannot masquerade as live.
- Forma dashboard and Folio export reconcile to the same snapshot and counts.

## Research and unresolved choices
Validate Forma component APIs, Folio document capabilities, Strata event storage APIs and the Echelon knowledge compiler before claiming integrations. Resolve retention policy, named control owners, audit period, independent reviewer role, sampling methods, accessibility testing plan and external embedding threat model. No code authorized by this document.

# Continuous Control Monitoring and Evidence Assurance Architecture
2026-11-22 | Requirements specification, not implemented or audited.

## Mission
Build a control monitoring capability where assurance is computed from expected independent evidence and tested operation, never from the absence of alarms or a self-declared success. This is an enterprise capability serving all DAF business domains, agents, APIs, tenants and external integrations.

## Canonical model
ControlDefinition, ControlVersion, ObligationAssertion, Risk, RequirementLink, ControlDependency, EvidenceExpectation, EvidenceObservation, EvidenceSource, EvaluationRun, ControlResult, DeficiencyCase, RemediationPlan, TestPlan, TestSample, TestResult, ReviewerAttestation, AuditPeriod, AuditPackage, AuditorRequest, ControlWidgetGrant. Every object has scoped identity, effective timestamps, provenance, authorization, immutable revision lineage and status. Source facts and reviewer conclusions remain separate.

## Event and evidence pipeline
1. Governed business APIs emit attributable operational events, never self-asserted effectiveness.
2. An independent expectation engine determines which evidence should exist by schedule, action, source and policy.
3. The evidence collector validates schema, signature, tenant and classification; stores append-only observations with content digests and lineage.
4. The evaluator checks both evidence completeness and the control predicate, including independent corroboration and negative evidence.
5. Monitoring produces actionable cases for failures, missing/stale evidence, tampering and conflicting observations.
6. Independent reviewers assess material exceptions and sign off remediation and retest.
7. Forma displays live operational state; Folio produces reproducible, scoped, time-window audit packages.

## Assurance states
NotDesigned, Specified, Implemented, Operating, InsufficientEvidence, StaleEvidence, AwaitingReview, Pass, Fail, Deficient, Remediating, IndependentlyTested, AuditorReviewed. Design maturity, operating status, test status and audit status are **separate dimensions**, not one misleading progression. Do not call a control Pass unless a versioned evaluation over a defined period has sufficient, independently corroborated evidence. NotApplicable requires signed justification.

## Coverage metrics and denominator discipline
- Evidence completeness = verified expected evidence instances / total expected evidence instances for the scoped period. Report excluded, late, unknown and disputed instances separately.
- Control operating coverage = controls with sufficient evidence and evaluated results / all applicable controls; distinguish passing vs failing.
- Testing coverage = independently executed tests / tests due; separate design tests from operating-effectiveness tests.
- Deficiency aging = open days by severity, cause, owner and overdue status.
- Requirement traceability = requirements with reviewed applicable control relationships / applicable requirements, not automatically generated links.
- Risk-weighted coverage is optional and must publish approved weights; never hide raw denominators or high-risk failures behind aggregate scores.
- Evidence collection, operation, testing, auditor review and actual SOC 2 examination are separate claims.

## Missing evidence algorithm
Create a schedule of expected evidence keyed by control version, scope, producer, business event and period. Reconcile against received signed evidence after a configured lateness allowance. On missing expected observations, independently raise a deficiency and preserve it even if evidence later arrives. Late evidence may change evaluation but not erase incident history. Zero business activity requires independently validated zero-activity evidence.

## Integrity and security
Strata is mandatory as an architectural persistence dependency, but its actual API capabilities have not been independently verified. Require append-only authoritative history, linked corrections, deterministic replay, protected evidence storage, source signing or equivalent integrity, least-privilege access, privacy-safe redaction, legal holds, retention schedules, independent monitoring, tenant isolation and recovery testing. Do not place secrets in general logs. No assumption of a specific Strata cryptographic feature until verified.

## API boundaries
ControlCatalog, ControlExpectations, EvidenceIngest, EvidenceQuery, ControlEvaluations, Deficiencies, Remediations, TestPlans, TestExecutions, Attestations, AuditorRequests, AuditPackages, DashboardQueries, WidgetGrants, MonitoringSubscriptions. Read endpoints must enforce sponsor/partner/legal-entity scope. Write commands require idempotency keys, version preconditions, actor authority and complete audit evidence. AI cannot bypass authorization or certify its own results. External webhook subscribers receive minimum necessary event data.

## Operational failure policies
Monitoring failure is a degraded-assurance incident, not a healthy state. If a safety-critical control cannot evaluate, consequential grant/payment/investment actions must follow explicit sponsor-approved fail-closed or manual-continuity policies; never silently continue. Define bounded recovery, escalation, emergency stop and compensating-control requirements before implementation. Live outbound payment execution remains deferred.

## Verification
See CONTINUOUS-CONTROL-MONITORING-TESTS.csv for 20 scenario contracts. These are specified, not executed. Independent CPA scoping and counsel review remain necessary before any SOC 2 or DAF regulatory assurance claims.

## Integration with existing artifacts
Trace to MASTER-ENTERPRISE-CONTROL-MATRIX.csv, SPECIFIC-EXECUTABLE-CONTROLS.csv, HIGH-RISK-CONTROL-VALIDATION-WAVE-03.csv and DETAILED-REQUIREMENT-CONTROL-LINKS-WAVE-02.csv. Existing mappings are provisional and must not be treated as verified control coverage.

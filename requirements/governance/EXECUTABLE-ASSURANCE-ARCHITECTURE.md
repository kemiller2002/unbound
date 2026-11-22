# Executable assurance architecture and specific control specification
Date: 2026-11-21 | Design only, no implemented evidence producers, executed tests or SOC 2 attestation.

## What was added
46 specific, independently testable proposed controls across enterprise capabilities. The companion CSV identifies control ID, domain, objective, required event type, failure condition, negative test, proposed owner, independent reviewer, trigger and current status. These are **new detailed controls** and do not replace the 90 baseline control patterns.

## Control runtime architecture
1. **ControlRegistry**: versioned control objective, risk, owner, procedure, criteria candidate, applicable business commands, evidence contract, expected execution frequency and authorized exception path.
2. **EvidenceCollector**: accepts authenticated, schema-validated evidence events from governed APIs, agent tools, approval systems, accounting, charity acquisition and infrastructure. Enforces classification, minimal data and tenant scope.
3. **EvidenceIntegrity**: append-only records, hash and source identity, protected retention, tamper detection, key management and independent access controls. Verify Strata suitability before implementation.
4. **ExpectedEvidenceMonitor**: computes expected versus observed evidence, including absence of heartbeat, approval, source freshness check, reconciliation or reviewer attestation. Opens cases for missing evidence.
5. **ControlEvaluator**: deterministic control predicates against evidence and independent reference facts. A successful event emission is not itself proof that the control worked.
6. **NegativeTestHarness**: synthetic adversarial fixtures that attempt unauthorized access, stale approvals, unbalanced journals, duplicate grants, prompt injection, altered evidence and forbidden live payment calls.
7. **IndependentReviewer**: checks operating samples, exceptions, reviewer independence, deficiencies, remediation and retest; human attestations where required.
8. **AssuranceDashboard**: controls designed, implemented, expected events, evidence completeness, effectiveness tests, unresolved deficiencies and time-window coverage. Never show SOC 2 'compliant' based solely on design.
9. **FolioAuditPackage**: reproducible, time-bounded export with redaction, provenance, control mapping, test results, deficiencies and missing evidence.

## Required evidence contract
controlId, controlVersion, evidenceId, eventType, schemaVersion, producer, environment, actor, delegatedAuthority, sponsor/partner/legalEntity scope, resourceId, action, result, policyVersion, occurredAt, recordedAt, correlationId, sourceEvidenceRefs, integrityDigest, retentionClass, privacyClass, expectedEvidenceWindow, reviewer, reviewTime, deficiencyCaseId, syntheticFlag.

## Deterministic evaluator output
Pass | Fail | InsufficientEvidence | NotApplicable | AwaitingReview | StaleEvidence. Each result includes the applicable control version, evaluated time window, supporting evidence references, independently verified facts, reasons and next authorized action. Do not infer Pass from a missing event or self-attestation.

## Lifecycle and gates
Specified -> Reviewed -> Implemented -> Operating -> IndependentlyTested -> AuditorReviewed. Deficient and Remediating are tracked separately. Gate transitions require actual source evidence, not documents or synthetic tests. No controls have advanced beyond specification in this work.

## Distinct regulatory crosswalk
SOC 2 Trust Services Criteria scope must be agreed with an independent CPA. Sponsor fiduciary, DAF legal, tax, accounting and sanctions requirements require their own counsel/accountant-approved applicability matrix. Do not conflate a SOC 2 control with proof of regulatory compliance.

## Priority implementation order when authorized
1. Identity/authorization, agent tool guardrails, immutable evidence and missing-evidence detection.
2. Sponsor grant authority, eligibility and sanctions, payment prohibition and separation of duties.
3. Ledger/NAV/custody reconciliation and financial close.
4. Source rights, charity identity and evidence freshness.
5. Vendor risk, privacy, incidents, recovery and auditor export.

## Acceptance gates
- Every specific control must have actual producer and independent reviewer assigned.
- Every event schema must be versioned, access-controlled and tamper-evident.
- Every negative test must demonstrate a denied/held outcome and observable evidence.
- Every missing expected control must produce a monitored deficiency.
- No AI agent may self-authorize or certify its own control effectiveness.
- No live payment rail may be invoked under requirements-stage authorization.
- Every reported pass must identify its independent proof and evaluated period.

## Remaining work
Validate mapping from the existing 494 provisional requirement rows to these specific controls; review all remaining requirement documents and daf-landscape REG/CTL assertions; refine event schemas, owner assignments, retention, test harness and actual operational controls. Nothing here is implemented or audited.

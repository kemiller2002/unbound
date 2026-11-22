# Enterprise SOC 2 readiness directive and evidence architecture
Status: mandatory design directive; **not a claim of SOC 2 compliance, examination or certification**. 2026-11-21.

## Scope and precise language
All Unbound directives, requirements, architecture decisions, API contracts, UI, agents, data pipelines, infrastructure, operational runbooks and vendor integrations MUST be designed to support a future independent SOC 2 examination. SOC 2 is an AICPA attestation report, not a product certification. Trust Services Criteria: Security is mandatory; Availability, Confidentiality, Processing Integrity and Privacy are selected according to service commitments, scope and auditor review. Type I assesses design at a point in time; Type II examines operating effectiveness over a period. A design document or audit log does not establish operating effectiveness.

## Binding directive
SOC2-001: Every claimed control MUST have a unique ID, scope, control owner, objective, risk, procedure, frequency/trigger, evidence schema, source system, retention, reviewer, failure/escalation behavior and test method.
SOC2-002: Every business or security-significant event MUST generate attributable, tamper-evident, time-stamped evidence including actor, authority, action, resource, decision, result, correlation ID and policy/version where applicable.
SOC2-003: Logging MUST be risk-based and privacy-aware; prohibit secrets, credentials, unnecessary sensitive personal data, raw restricted charity documents and unrestricted prompt contents in general logs.
SOC2-004: Control evidence MUST be generated as a byproduct of normal operations wherever feasible, not manually assembled only before audit.
SOC2-005: Evidence MUST show both successful operation and failures, denials, exceptions, remediation, reviews and overdue controls.
SOC2-006: Design immutable/append-only audit records, integrity verification, independent protected storage, restricted access, backup and retention/legal hold; actual implementation to be validated.
SOC2-007: All requirements and PRs MUST map to applicable control IDs, tests, evidence artifacts and owners; absence requires documented non-applicability decision.
SOC2-008: No control may be marked implemented, operating, tested or audit-ready without actual observed evidence and review.
SOC2-009: Control evidence MUST be exportable into auditor-ready, time-bounded, reproducible Folio packages with chain of custody and access redaction.
SOC2-010: Access management MUST cover MFA, provisioning/deprovisioning, least privilege, privileged access, service identities, agent identities and periodic review.
SOC2-011: Change management MUST cover approvals, source provenance, CI checks, testing, deployment records, rollback and emergency changes.
SOC2-012: Security monitoring MUST cover authentication, authorization failures, administrative actions, data access/export, agent activity, configuration changes, anomalous events and incident handling.
SOC2-013: Risk and vendor management MUST inventory critical services, subservice organizations, shared responsibilities, due diligence, contracts and ongoing monitoring.
SOC2-014: Data classification, privacy, encryption, retention, deletion and legal holds MUST be defined and enforced consistently, including sponsor/partner isolation.
SOC2-015: Backup, disaster recovery, availability objectives, incident response, tabletop exercises and recovery tests MUST produce dated evidence.
SOC2-016: Vulnerability, dependency, secret, infrastructure and penetration testing programs MUST record findings, prioritization, fixes, exceptions and retest evidence.
SOC2-017: AI agents MUST be individually attributable, bounded by authorization, monitored, testable and interruptible; prompt/tool changes and sensitive tool invocations MUST be governed.
SOC2-018: Financial and grant controls MUST preserve sponsor authority, approvals, reconciliation, anti-fraud checks and separation of duties.
SOC2-019: A central control register MUST distinguish Proposed, Designed, Implemented, Operating, Tested, Deficient, Remediating and AuditorReviewed; never infer one state from another.
SOC2-020: Control claims in marketing, contracts and reports MUST be reconciled to actual scope, implemented controls, evidence and independent attestation.
SOC2-021: Define system boundaries, commitments, service description, infrastructure, people, processes, data and relevant third parties before selecting final criteria.
SOC2-022: Define management's description and complementary user entity controls and subservice organization controls where relevant.
SOC2-023: Set audit period, evidence retention and review cadence with qualified auditor and counsel; do not invent universal retention periods.
SOC2-024: Control owners MUST periodically attest to operation, and independent reviewers MUST challenge evidence completeness and effectiveness.
SOC2-025: Audit events MUST support secure correlation across API, agent, Strata persistence, Forma, Folio, accounting, investments, grants and integrations.
SOC2-026: Security incidents and exceptions MUST have documented detection, triage, containment, remediation, communication and lessons learned.
SOC2-027: A missing expected log or control heartbeat MUST itself create a monitored control exception.
SOC2-028: Source licensing, third-party data rights and permitted AI use MUST be reflected in access, storage, retrieval and exports.
SOC2-029: Every future agent directive MUST instruct the agent to identify applicable SOC 2 controls, produce evidence, test negative cases and report any gaps honestly.
SOC2-030: Require security/privacy design review before introducing new data categories, external APIs, autonomous permissions or financial workflows.
SOC2-031: Keep production evidence distinct from test/synthetic evidence and identify environment, period and provenance.
SOC2-032: Support retention and lawful deletion policies without pretending append-only logs exempt personal data from privacy obligations.
SOC2-033: Record policy exceptions with risk acceptance authority, expiration, compensating controls and follow-up.
SOC2-034: Use objective coverage metrics: controls with owners, evidence emitted, missed runs, evidence integrity, overdue access reviews, remediation aging and tested effectiveness.
SOC2-035: Maintain a versioned control-to-system-to-evidence-to-test crosswalk.
SOC2-036: Require explicit approval before publicly claiming SOC 2 compliance or a Type I/II report; no such status exists merely from these requirements.

## Minimum evidence envelope
evidenceId, controlId, controlVersion, system, environment, sponsor/partner scope where appropriate, occurredAt, recordedAt, actor/service identity, authorityRef, action, resourceType/resourceId (redacted where necessary), result, correlationId, immutable log reference, source hash, collection method, reviewer, review time, retention class, access class, test/production flag, exceptionId and evidence integrity verification.

## Example control register
| Control | Objective | Evidence |
|---|---|---|
| CTRL-ACCESS-01 | Privilege granted only after approval | request, approver, identity, policy version, activation |
| CTRL-ACCESS-02 | Departures lose access promptly | termination event, revocation, verification |
| CTRL-CHANGE-01 | Changes tested and approved | PR, test results, review, deployment hash |
| CTRL-AI-01 | Agent cannot exceed authority | denied calls, scope policy, adversarial test |
| CTRL-GRANT-01 | Sponsor approval required | decision snapshot, authorized approver, grant event |
| CTRL-FIN-01 | Reconciliation independently reviewed | source statements, break case, signoff |
| CTRL-SEC-01 | Logs complete and tamper-evident | heartbeat, hash validation, alert on gaps |
| CTRL-IR-01 | Incidents tracked and resolved | incident timeline, containment and postmortem |
| CTRL-BCP-01 | Recovery objectives tested | backup manifest, restore test, results |
| CTRL-VENDOR-01 | Critical vendors reviewed | inventory, diligence, follow-up and decision |

## Required directive footer for future agent missions
'Apply Unbound SOC 2 readiness requirements to every changed domain. Identify applicable controls, evidence events, owners, negative tests, access and privacy safeguards, and operational monitoring. Emit and retain verifiable evidence for each control we claim to operate. Never claim compliance, successful tests, or audit readiness without observed evidence. Preserve sponsor authority and defer live payment execution until separately authorized.'

## Acceptance scenarios
SOC-T01 Successful grant approval creates control evidence.
SOC-T02 Unauthorized grant approval denied and logged.
SOC-T03 Agent self-elevation blocked and logged.
SOC-T04 Missing expected control log triggers incident/exception.
SOC-T05 Sensitive tokens are excluded from audit payload.
SOC-T06 Evidence altered after recording fails integrity check.
SOC-T07 Access reviewer attests to privileges with timestamp.
SOC-T08 Emergency deployment records exception and retrospective review.
SOC-T09 Audit export redacts partner-private data.
SOC-T10 Auditor replays control over requested period with evidence.
SOC-T11 Synthetic test evidence cannot masquerade as production evidence.
SOC-T12 Third-party service failure creates monitored incident evidence.
SOC-T13 Control marked 'operating' without actual evidence is rejected.
SOC-T14 Control deficiency remediation includes independent retest.
SOC-T15 Legal deletion/retention request is handled by approved policy.
SOC-T16 AI prompt injection cannot disable logging or approval.
SOC-T17 Sponsor policy update generates approved change record.
SOC-T18 Evidence gaps are included, not hidden, in audit package.

## Implementation and assurance gates
1. Confirm scope and Trust Services Criteria with qualified SOC 2 auditor.
2. Perform risk assessment and system description.
3. Build control inventory, control mapping and evidence contracts.
4. Implement and operate controls, collect evidence over relevant period.
5. Independently test effectiveness, remediate gaps.
6. Engage independent CPA firm for SOC 2 examination and report.
No control is presently asserted operational by this document.

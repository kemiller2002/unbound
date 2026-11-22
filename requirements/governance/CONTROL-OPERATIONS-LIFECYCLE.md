# Enterprise Control Operations Lifecycle Requirements
2026-11-22 | Requirements and research only. **No implementation authorized.**

## Purpose
Define how Unbound operates controls as durable business capabilities rather than as passive compliance documents. Preserve sponsor discretion, legal duties, financial integrity and agent boundaries while minimizing unnecessary administrative work. SOC 2 attestation is not claimed.

## Control lifecycle
1. **PROPOSE: Propose control.** Identify risk, obligation, scope, expected outcome, owner candidate and alternative controls. Proposed accountable role: Governance.
2. **ASSESS: Assess applicability.** Confirm sponsor, tenant, entity, jurisdiction, financial and SOC2 scope; document exclusions. Proposed accountable role: Compliance.
3. **DESIGN: Design control.** Specify predicate, evidence expectation, independence, failure behavior and negative tests. Proposed accountable role: Control Owner.
4. **REVIEW: Independent design review.** Reviewer challenges completeness, conflicts, legal assumptions and evidence sufficiency. Proposed accountable role: Independent Reviewer.
5. **APPROVE: Approve activation.** Authorized governance approval for version, scope and risk acceptance; no agent self-approval. Proposed accountable role: Sponsor Governance.
6. **ASSIGN: Assign accountable owner.** Named accountable owner, backup, independent reviewer and escalation; no orphan controls. Proposed accountable role: Governance.
7. **ACTIVATE: Activate expectation.** Only after authorized implementation; establish expected evidence, effective date and monitoring. Proposed accountable role: Control Owner.
8. **OPERATE: Operate and collect.** Collect source evidence, track missing/stale data and maintain separation of duties. Proposed accountable role: Operations.
9. **EVALUATE: Evaluate control.** Evaluate time-bounded evidence and explicit control outcome; do not infer pass from silence. Proposed accountable role: Assurance.
10. **TEST: Independently test.** Approved sampling population, positive and negative tests, independent reviewer and results. Proposed accountable role: Control Testing.
11. **REMEDIATE: Remediate deficiencies.** Case, risk, accountable owner, due date, compensating control, retest and independent closure. Proposed accountable role: Risk/Compliance.
12. **CHANGE: Manage control changes.** Versioned material changes, impact analysis, reapproval and effective-date split. Proposed accountable role: Governance.
13. **TRANSFER: Transfer ownership.** Approved handoff, access changes, pending case transfer, history and new owner acceptance. Proposed accountable role: Governance.
14. **SUSPEND: Suspend or emergency-stop.** Explicit authority, scope, expiry, safe fallback and monitored exception. Proposed accountable role: Sponsor Governance.
15. **RETIRE: Retire control.** Prove replacement or no longer applicable, resolve open exceptions and retain history. Proposed accountable role: Governance.
16. **AUDIT: Support independent audit.** Time-bounded evidence, limitations, requests, reviewer signoffs and source provenance. Proposed accountable role: Compliance.

## Cross-cutting requirements
- **OPS-001** (Governance): Every control has immutable identity and version history.
- **OPS-002** (Compliance): Applicability decisions are jurisdiction-, sponsor-, partner-, entity- and period-aware.
- **OPS-003** (Governance): Each control records risk, objective, obligations, materiality and dependency relationships.
- **OPS-004** (Governance): Each active control has a named owner, backup and independent reviewer.
- **OPS-005** (Security): Owner and reviewer conflicts are prevented by authorization rules.
- **OPS-006** (Governance): Delegation and reassignment require acceptance, expiry and full history.
- **OPS-007** (AI): Agents may draft controls and cases but cannot activate, self-review or self-certify.
- **OPS-008** (Governance): Control activation requires approved version, scope, evidence producer and failure behavior.
- **OPS-009** (Assurance): Evidence expectation is explicit for transactions, schedules, exceptions and zero activity.
- **OPS-010** (Assurance): Missing or stale evidence never becomes automatic pass.
- **OPS-011** (Assurance): Control results preserve independent evidence, denominator, evaluated period and scope.
- **OPS-012** (Security): Monitoring failures create deficiencies with safe operational fallback.
- **OPS-013** (Governance): Control changes trigger dependency impact assessment and reapproval when material.
- **OPS-014** (Assurance): Historical evaluations remain tied to effective control versions.
- **OPS-015** (Testing): Negative tests and sample populations are defined before assurance claims.
- **OPS-016** (Testing): Testing distinguishes design validation from operating effectiveness.
- **OPS-017** (Risk): Deficiencies have severity, exposure, ownership, due date, remediation and retest.
- **OPS-018** (Risk): Deficiency closure requires independent evidence and reviewer authority.
- **OPS-019** (Risk): Compensating controls have documented expiry, limits and independent approval.
- **OPS-020** (Governance): Emergency suspension preserves mandatory legal and financial safety constraints.
- **OPS-021** (Governance): Retirement requires dependency review and successor/no-longer-applicable justification.
- **OPS-022** (Reporting): Control dashboards cannot label design completion as regulatory compliance.
- **OPS-023** (Reporting): Evidence exports include missing items, redaction, provenance and limitations.
- **OPS-024** (Security): Every material action is auditable and tenant-isolated.
- **OPS-025** (Architecture): Control operations support sponsor and partner variation without duplicating authoritative policy.
- **OPS-026** (UI): Control ownership and reviewer tasks are accessible on mobile and desktop.
- **OPS-027** (UI): Tasks show next authorized action, reason, urgency and evidence required.
- **OPS-028** (Finance): Financial controls bind independent source reconciliation and accounting assertions.
- **OPS-029** (Compliance): Legal assertions require qualified review and versioned applicability.
- **OPS-030** (Finance): Live payment execution remains prohibited until separately authorized.
- **OPS-031** (Governance): Control-to-requirement mappings require semantic review, not keyword coverage alone.
- **OPS-032** (Governance): Sponsor administration cannot bypass independent governance or legal authority.

## Authority and separation
Sponsor operations administrators configure permitted processes, not legal authority. Sponsor governance approves reserved delegations; compliance maintains regulatory interpretation and escalates legal questions; control owners operate controls; independent reviewers validate evidence; auditors assess under their own professional standards. An agent cannot approve its own actions or control effectiveness. Role permissions must be scoped by sponsor, partner, legal entity, DAF and effective time.

## Ownership handoff
Every transfer records outgoing and incoming owner, independent approver, effective time, open deficiencies, overdue evidence, expiring delegations, acknowledgement and escalation. No active control may be ownerless. Automated reassignment may prepare but cannot self-authorize reserved roles.

## Deficiency lifecycle
Detected -> Triaged -> Assigned -> Investigating -> RemediationProposed -> Authorized -> Remediating -> RetestPending -> IndependentlyVerified -> Closed. Reopen on recurrence or contradictory evidence. Record materiality, impact, affected periods, related controls, compensating controls, expiry and communications. Preserve prior history and prohibit silent deletion.

## Assurance and audit
Report design, operating evidence, independent tests and auditor review as separate dimensions. Control health is period- and scope-specific. A positive operational event is not independent proof. Missing evidence, unreviewed exceptions, synthetic tests, unresolved legal claims and unverified source data must remain visible.

## Requirements-stage deliverables
Control operations register, authority matrix, lifecycle state transition contract, evidence expectation contract, deficiency and handoff workflows, UI screen inventory, widget contract inventory, acceptance tests and unresolved decisions. No source code, deployment, database migration or operational control activation.

## Remaining decisions
Actual named owners, sponsor governance delegations, severity thresholds, legal applicability, reviewer independence policy, sampling approach, retention schedules, audit periods, evidence producer verification, accessibility usability research and product-specific Forma/Folio capability validation.

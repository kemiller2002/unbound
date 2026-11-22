# Master enterprise control matrix governance
Date: 2026-11-21. Requirements-stage design only.

## Baseline
30 enterprise capability domains and 90 **proposed** control rows, with explicit proposed role owner, governed API scope, bounded agent scope, control objective, candidate SOC 2 criteria, evidence, trigger, failure response, test ID and status. All rows have **No** for operating evidence, executed tests and auditor review. These are cross-cutting baseline controls, not a substitute for domain-specific control decomposition.

## Mandatory requirements
MATRIX-001 Every requirement, business workflow, API, agent, data source and operational procedure must map to applicable control IDs and a documented nonapplicability decision where appropriate.
MATRIX-002 Controls must have accountable named owners assigned before implementation; current owners are role placeholders.
MATRIX-003 Evidence must identify event, actor, authorization, action, outcome, system, timestamp, source integrity, policy version, environment, tenant scope, retention and reviewer.
MATRIX-004 Monitor missing expected evidence and failed control operations, not just successful logs.
MATRIX-005 Do not log secrets, unnecessary personal data or restricted documents in generalized logs.
MATRIX-006 Preserve segregation of duties, sponsor discretion, independent reconciliation and legal restrictions even when an AI agent recommends an action.
MATRIX-007 Track lifecycle Proposed, Designed, Implemented, Operating, Tested, AuditorReviewed; mark deficiencies and remediation separately. Require observed evidence for every advancement.
MATRIX-008 Scope SOC 2 Trust Services Criteria with qualified auditor; Security baseline and additional criteria only as applicable. Current CC5/CC6/CC7 mappings are **candidates**, not validated AICPA criterion-by-criterion mappings.
MATRIX-009 Track DAF legal, fiduciary, tax and financial reporting obligations separately from SOC 2; overlapping evidence does not establish compliance with all.
MATRIX-010 Require negative/adversarial tests, exception escalation, time-bounded evidence packages and independent review.
MATRIX-011 Preserve production evidence separately from synthetic/test evidence; no claims of SOC 2 attestation, Type I/II or operating effectiveness without independent evidence.
MATRIX-012 Live outbound payment execution remains deferred and prohibited until separate authorization.

## Matrix interpretation
Each capability currently receives three shared control patterns: (1) authority and segregation, (2) audit evidence integrity, (3) independent review and exceptions. These patterns are **starting templates**. Before activation each row must be tailored to its domain: for example charity acquisition needs rights and provenance; grants need sponsor approval and charity eligibility; NAV needs pricing/correction checks; reconciliation needs independent custody/bank/GL evidence; agents need tool-boundary and self-escalation tests; security needs incident and access controls.

## Priority expansion backlog
1. Reconcile research controls CTL-001..054 and regulatory assertions REG-001..065 from daf-landscape with actual Unbound requirements and the new control IDs. Do not assert that reconciliation has been completed.
2. Expand to every requirement ID in the existing requirements corpus and identify uncovered controls and duplicate assertions.
3. Define a detailed evidence event schema and evidence-coverage monitoring.
4. Develop SOC 2 system description, risk register, vendor/subservice boundary, and management commitments.
5. Validate criteria applicability and mapping with an independent SOC 2 CPA and counsel for DAF-specific obligations.
6. Implement controls and collect dated operational evidence before making any audit readiness claim.

## Operational status
90 control designs proposed; 0 operating controls verified; 0 control tests executed; 0 controls auditor reviewed. No SOC 2 report or attestation claimed.

## Source directives
requirements/governance/SOC2-CONTROLS-EVIDENCE-AND-ENTERPRISE-DIRECTIVE.md
requirements/governance/SPONSOR-AUTHORITY-AND-AUTONOMOUS-OPERATIONS.md
requirements/AGENT-MISSION.md

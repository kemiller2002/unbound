# Sponsor authority and autonomous operations
Status: normative requirements, not implemented. 2026-11-21.

## Core boundary
The sponsoring charity retains legal control of DAF assets, investments, grant approvals, disbursements, policies and delegated authority. Donors and advisors recommend; they do not gain unilateral authority to trade, approve grants or move funds. AI agents have no inherent fiduciary or corporate authority. Automation can execute only specifically delegated, policy-approved actions within verified controls. Legal and fiduciary duties cannot be delegated away by software.

## Authority tiers
A0 Observe: read authorized records and explain evidence.
A1 Prepare: draft recommendations, classify, investigate, simulate, propose actions.
A2 Bounded internal action: deterministic, reversible, low-risk tasks under approved controls, e.g. tagging, routing, fetching permitted public records, opening exception cases.
A3 Controlled consequential action: sponsor-authorized actions with explicit policy limits, segregation of duties, identity verification and documented approval.
A4 Reserved human authority: grant approval where sponsor discretion required, material policy changes, high-risk overrides, major financial commitments, unusual investments, suspicious activity judgments, legal interpretations and governance decisions.
A5 Prohibited: bypassing legal restrictions, unauthorized disclosure, falsifying evidence, backdating approvals, unapproved live payments, disabling required logs, or agent self-expansion of privileges.

## Requirements
GOV-AUTH-001: Maintain an enterprise authority registry mapping every command, actor, sponsor legal entity, partner, DAF, risk class, scope, conditions, approval and delegated authority expiration.
GOV-AUTH-002: Explicitly separate recommendation, approval, accounting recognition, trading instruction, payment instruction, payment execution and settlement.
GOV-AUTH-003: No AI agent may grant itself permissions, approve its own consequential action or change its own authority policy.
GOV-AUTH-004: Segregation of duties applies across humans, service accounts and agents; prevent one actor controlling request, approval and reconciliation.
GOV-AUTH-005: Support dual control, committees, quorum, recusals, conflict disclosures, substitute approvers and emergency authority with post-event review.
GOV-AUTH-006: Require cryptographically attributable identity, strong authentication and least privilege for every material command.
GOV-AUTH-007: Model sponsor board, officers, compliance, grants, finance, investment committee, security, charity steward, partner and donor/advisor roles without conflating them.
GOV-AUTH-008: Every action must be checked against policy version, authority limits, effective dates, fund restrictions, available liquidity and regulatory blocks.
GOV-AUTH-009: Human approval must identify the exact proposal and evidence snapshot; material changes invalidate or trigger reapproval.
GOV-AUTH-010: Agents may automate evidence gathering and reversible internal steps only under explicit approved authority and monitored boundaries.
GOV-AUTH-011: Agent-generated claims and legal interpretations cannot become binding controls without authorized review.
GOV-AUTH-012: Log every material authority decision, denied command, override request, approval, delegation and privilege change in tamper-evident records.
GOV-AUTH-013: Route uncertain authority, suspected fraud, conflicts, stale evidence and material exceptions to authorized humans.
GOV-AUTH-014: Require independent financial reconciliation; the agent initiating a transaction cannot attest to its independent settlement.
GOV-AUTH-015: Support temporary delegated authority with expiration, revocation and immediate propagation.
GOV-AUTH-016: Partner isolation applies to policy, approvals, cases and audit evidence.
GOV-AUTH-017: Maintain accessible Forma approval workspaces and reproducible Folio decision packages.
GOV-AUTH-018: Run shadow-mode simulations and adversarial tests before enabling autonomous consequential actions.
GOV-AUTH-019: No production live outbound payment rail is authorized by this requirements document.
GOV-AUTH-020: Produce SOC 2 control/evidence records for authority configuration, changes, execution, exception handling and periodic access review.
GOV-AUTH-021: Keep authorization policy enforcement deterministic and independently testable; AI explanations do not replace enforcement.
GOV-AUTH-022: Preserve append-only event history and replayable decisions; verify actual Strata capabilities before implementation.
GOV-AUTH-023: Use governed F# APIs for all actions, not direct agent database writes.
GOV-AUTH-024: Define emergency stop, agent suspension, safe mode and human takeover, with evidence and recovery approval.
GOV-AUTH-025: Review authority assignments periodically and upon role changes, departures, DAF succession or partner termination.
GOV-AUTH-026: Require explicit control ownership and service-level escalation for unattended operations.

## Decision matrix examples
| Action | Default authority | Guardrail |
|---|---|---|
| Collect approved public charity feed | A2 | source rights, budget, logging |
| Flag conflicting charity identity | A2 | no destructive merge |
| Merge high-risk charity identities | A4 | steward approval and reversible lineage |
| Draft grant recommendation | A1 | advisor permission |
| Approve grant | A4 | sponsor-authorized role, policy and evidence |
| Prepare payment instruction | A3 | approval, payee verification and segregation |
| Execute live payment | A5 currently | deferred pending separate authorization |
| Reconcile bank/custody/GL | A2 analysis, A4 exceptions | independent sources |
| Change sponsor investment mandate | A4 | sponsor authority and committee policy |
| Change production authority rules | A4 | dual control, testing and change approval |
| Export confidential partner evidence | A3/A4 | rights and recipient checks |
| Disable audit logging | A5 | prohibited |

## Tests
AUTH-T01 Agent attempts self-approval -> denied and logged.
AUTH-T02 Advisor attempts trade -> denied.
AUTH-T03 Approval after proposal changed -> requires reapproval.
AUTH-T04 Expired delegation -> denied.
AUTH-T05 Cross-tenant request -> denied, alert and evidence.
AUTH-T06 Same actor initiates and independently reconciles -> segregation failure.
AUTH-T07 Emergency stop -> queued autonomous actions safely suspended.
AUTH-T08 Agent attempts to suppress audit log -> denied and alerted.
AUTH-T09 Committee quorum/recusal -> correctly enforced.
AUTH-T10 Simulation of payment -> no live rail.
AUTH-T11 Approval on stale charity eligibility -> held per policy.
AUTH-T12 Policy rollback -> append-only history retained.

## Open decisions
Legal delegation matrix and board charter; quantitative automation thresholds; committee structure; emergency approval; authority rule compiler; identity provider and access review cadence; incident escalation and evidence retention. Counsel and auditors must review applicability. This document specifies design, not SOC 2 certification.

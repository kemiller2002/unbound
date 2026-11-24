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

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-GOV-001 — Assurance scope set by actual reliance:** Evidence engineering SHALL operate from inception for SOC 1 (AT-C 320, ledger and ICFR objectives) and SOC 2 (2017 Trust Services Criteria, 2022 points of focus) readiness, with complementary user entity controls mapped. Before live reliance by a sponsor, either the sponsor's external auditor's written planned approach, or a recorded 'planned approach pending' with the sponsor CFO's written acceptance of the interim evidence pack, (rely on a SOC 1 type 2, test Unbound directly, or substantive testing) SHALL be recorded and SHALL set report scope and timing; no sponsor-count trigger and no stand-alone type 1 SHALL be required unless a contract requires it. A SOC 1 type 2 covering a full period SHALL be planned when any user auditor states an intent to rely on Unbound controls; planning one earlier for cost spreading across two or more user entities is a business-policy default owned by the Unbound CFO. Contracts SHALL include right-to-audit, an evidence pack and a named cost bearer (default: each party bears its own auditor and staff costs). Manual review procedures SHALL be replaced by continuous or generated evidence only after the auditor confirms reliance; generated reports SHALL carry information-produced-by-entity support (query definition, parameters, population counts, hash, run log). A 'planned approach pending' status SHALL expire after one sponsor audit cycle or 12 months, whichever is sooner [policy default, owner: sponsor CFO]; the interim evidence pack SHALL be generated at least monthly; expiry without a recorded approach SHALL suspend delegated automation for that sponsor.
  - Provenance: resolves SPD-PLT-001, RR-OPS-004-r1#Q1, RR-SOC2-024-r1#Q1, RR-UNB-GOV-002-r1#Q1, RCD-031, XID-INV-008, SPD-ADV-003, RR-UNB-ACC-017-r1#Q1; amends RR-OPS-004-r1, RR-SOC2-024-r1, RR-UNB-GOV-002-r1, RR-UNB-ACC-017-r1. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: commercial-availability; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor external auditor's written planned approach; executed contract assurance clause.
  - Ownership: domain owner: Platform assurance; financial system of record: n/a; policy owner: Unbound security and compliance; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound compliance.
  - Acceptance:
    - AC-1: Given a sponsor without a recorded auditor planned approach, when production reliance is requested, then activation is refused.
    - AC-2: Given a generated close certification, when produced, then it includes population counts, query hash and run log.
- **CNV-GOV-002 — Vendor reports and bridge letters:** Vendor assurance monitoring SHALL compute the gap between each relevant SOC report period end and each relying party's fiscal year-end or reliance date; a bridge letter SHALL be requested at a configurable gap (3-month default labelled practice, not standard) and a stale-report alert raised at 12 months; bridge letters SHALL be recorded as management assertions that do not extend assurance; beyond the tolerance, compensating procedures or a time-limited risk acceptance SHALL be required; coverage gaps, opinion modifications, exceptions and unmapped CUECs SHALL be tracked separately.
  - Provenance: resolves RR-REQ-X-073-r1#Q1; amends RR-REQ-X-073-r1, REQ-X-073. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor external auditor confirms the tolerance used in its reliance.
  - Ownership: domain owner: Platform assurance; financial system of record: n/a; policy owner: Unbound security and compliance; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound compliance.
  - Acceptance:
    - AC-1: Given a vendor report ending 30 June and a sponsor fiscal year ending 31 December, when monitoring runs on 1 October, then a bridge letter request is due.
    - AC-2: Given an adverse opinion, when recorded, then a risk acceptance or compensating procedure is required before continued reliance.
- **CNV-GOV-003 — Segregation of duties with deterministic release:** Segregation of duties SHALL be enforced by the engine. The deterministic release gate SHALL count as the releaser only when it runs a published, independently approved policy version, the human approvers have no authoring, publishing, deployment or administration rights over it, ITGCs are effective, and the external auditor and board or audit committee have accepted the design; until then the three-distinct-person rule SHALL apply. Reconciliation SHALL be performed outside the payment chain (internal or outsourced, chosen by cost) with daily automated matching, exception review within a sponsor-set number of business days and at least monthly independent review. Enforced leave SHALL apply where at least two trained staff exist, anomaly analytics otherwise; the CISO SHALL have direct access to the audit and risk committee; independent review SHALL be required for key and manual controls; each alternative control SHALL be documented with expiry. A change to a gate module SHALL carry a materiality class; a material change SHALL require the sponsor's acceptance (with notice to its auditor) before activation for that sponsor, and until accepted that sponsor SHALL revert to the three-distinct-person rule.
  - Provenance: resolves SPD-ORG-001, UNB-SMP-ORG-002#Q2, SPD-PLT-004, SPD-ORG-006; amends UNB-SMP-ORG-002. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: External auditor acceptance of the SoD design; board or audit committee approval.
  - Ownership: domain owner: Governance; financial system of record: n/a; policy owner: Sponsor board; data owner: Sponsor; operational owner: Sponsor executive; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given an approver who also has deploy rights to the gate, when they approve a PA-2 resolution, then the system requires a third distinct person.
    - AC-2: Given an alternative control past its expiry, when the control register runs, then it is flagged for committee review.
    - AC-3: Given a material gate change released without a sponsor acceptance record, when that sponsor releases a payment, then three distinct persons are required.
- **CNV-GOV-004 — Appointed contracted and fractional authority holders:** Reserved (A3+) authority SHALL be exercisable only by persons appointed by sponsor board resolution as officers or authorized agents with scope by decision class and limit, start and expiry, independence and conflict attestations, D&O or indemnification confirmation and acknowledgment of §4958 and §4966 manager status; contracted and fractional individuals MAY be appointed. Unbound vendor staff SHALL NOT hold A3+ authority. No person SHALL hold reserved authority for more than one sponsor (SPD-ADV-001); team transfers SHALL revoke old access before granting new access. Coverage SHALL be stated as roster coverage by role and day, derived from criticality, peak workload and independence, with double absence tested as a December stress scenario. While SPD-ADV-001 stands, reserved-authority holders, independent reconsideration reviewers and pre-contracted December surge reviewers SHALL each be exclusive to one sponsor; the owner decided (OD-M-01) to keep the absolute rule for release 1 and to revisit it only with evidence from an administrator deployment. The one-sponsor rule SHALL be enforced by a privacy-preserving person token in the shared store, matched exactly, returning only 'already appointed elsewhere' to the appointing sponsor.
  - Provenance: resolves SPD-ORG-002, UNB-SMP-CLR-001#Q1, UNB-SMP-ORG-003#Q2, UNB-SMP-ORG-003#Q1, SPD-ORG-003, UNB-SMP-ORG-002#Q1, SPD-CLR-002, UNB-SMP-CLR-001#Q2, RR-UNB-ECO-003-r1#Q1, UNB-SMP-ORG-003#Q3, SPD-ORG-004; amends UNB-SMP-CLR-001, UNB-SMP-ORG-002, UNB-SMP-ORG-003, RR-UNB-ECO-003-r1. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor board resolution of appointment; bylaws permitting additional officers.
  - Ownership: domain owner: Governance; financial system of record: n/a; policy owner: Sponsor board; data owner: Sponsor; operational owner: Sponsor executive; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given an appointment record without expiry, when saved, then it is rejected.
    - AC-2: Given a person appointed by sponsor A, when sponsor B attempts to appoint them for A3 decisions, then the appointment is refused.
- **CNV-GOV-005 — Policy packs, configuration and change control:** Policy packs SHALL be versioned and effective-dated; material changes SHALL require affirmative sponsor adoption; emergency mandatory changes SHALL use a separately authorized path with impact analysis and independent retrospective review; tightening SHALL NOT be assumed safe. Material changes to financial controls, eligibility, permissions or customer rights SHALL require independent approval regardless of direction; ambiguous or mixed changes SHALL be treated as material; author self-approval SHALL be impossible. Rollout SHALL use representative shadow replay with volume, risk coverage and rollback readiness criteria (not elapsed time alone), and cross-sponsor shadow evaluation only with privacy controls. Liability for platform-maintained content SHALL follow a contractual responsibility matrix; attestation SHALL NOT transfer liability for platform error. Each activation element SHALL map to its decision IDs and evidence dependencies, and unresolved mandatory dependencies SHALL block activation. Sponsor onboarding SHALL use plain-language outcome-based questions with examples and progressive disclosure; wording SHALL be validated by moderated sponsor usability testing with a recorded task-success criterion set before the test. Workflow timing defaults SHALL be versioned candidates overridden by statutory and contractual deadlines.
  - Provenance: resolves SPD-CFG-001, SPD-CFG-003, UNB-SMP-CFG-002#Q1, UNB-SMP-CFG-002#Q2, XID-ADV-010, RR-UNB-RULE-019-r1#Q1, RR-UNB-RULE-019-r1#Q2, SPD-EDF-001, UNB-SMP-CLR-002#Q1, UNB-SMP-CFG-001#Q1, RCD-046, SPD-CLR-003, RR-ADM-002-r1#Q2, UNB-XIN-SAS-003#Q1; amends UNB-SMP-CFG-001, UNB-SMP-CFG-002, UNB-SMP-CLR-002, RR-UNB-RULE-019-r1, RR-UNB-GOV-002-r1, RR-ADM-002-r1, UNB-XIN-SAS-003. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given a pack change that lowers a grant limit, when submitted by its author, then independent approval is required before adoption.
    - AC-2: Given an activation element whose basis decision is unresolved, when activation is requested, then it is refused.
- **CNV-GOV-006 — Justified human intervention and non-removable capabilities:** Every mandatory human step SHALL record its legal, fiduciary, independent-control, uncertainty-resolution or measured economic justification, accountable owner, evidence, review date and safe automation alternative; convention-only steps SHALL be reviewed annually and on material change, and missing or expired justification SHALL trigger review, never automatic bypass. Human queues SHALL be used only for genuine exceptions, reserved authority, unresolved evidence conflicts and mandatory independent controls. Financial integrity, tenant isolation, audit, mandatory compliance and accessibility SHALL be non-removable; other features SHALL be configurable. Every existing obligation SHALL carry a basis (law, contract, policy, convention) and review date. Cost budgets SHALL be diagnostic signals and SHALL never deny mandatory service or fiduciary obligations. Recurring obligations SHALL be inventoried for all servicing classes. Delegated customer roles SHALL expire on events with risk-based recertification. Affiliation attestation SHALL be event-driven with periodic confirmation; decisions affected by a stale mandatory attestation SHALL be blocked.
  - Provenance: resolves UNB-SMP-CXP-001#Q1, UNB-SMP-ARX-001#Q2, UNB-XIN-SAS-001#Q1, UNB-XIN-ADV-001#Q1, UNB-XIN-ADV-001#Q2, SPD-ENT-007, SPD-PLT-002, SPD-ENT-004, SPD-CXP-003, SPD-CXP-001; amends UNB-SMP-CXP-001, UNB-SMP-ARX-001, UNB-XIN-SAS-001, UNB-XIN-ADV-001. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given a mandatory step whose justification expired, when the register runs, then a review task opens and the step remains in force.
    - AC-2: Given a configuration disabling accessibility features, when saved, then it is rejected as non-removable.
- **CNV-GOV-007 — Authoritative control register and requirements governance:** One control register SHALL be authoritative, preserving mappings and evidence of the UNB-CTRL, UNB-SCTRL, CTL and TVCTL catalogs before duplicates are retired. Cumulative delegation-of-authority limits SHALL aggregate transactions that share a fund, a related-party group (donor household or 35% entities), the requesting principal, or a sponsor-operating payable counterparty, across approvers within a 30-day window (candidate), and detect splitting; recipient-level aggregation across unrelated funds SHALL be a detective analytic, not an approval gate. REQ-X-058..077 and landscape-wave outcome records SHALL be reclassified as a non-normative capability index only after carrier coverage is verified, preserving IDs. Research tasks and hypotheses SHALL move to the evidence backlog with owners, dependencies and dates. Golden fixtures SHALL be built first for authorities with the highest decision risk, after verifying current text. Clarity and EDF outputs SHALL be evaluation methods, not proof; material changes SHALL require independent evidence. Competitive claims SHALL carry dated evidence and bounded wording; absence of evidence is not proof of absence. Technology mandates SHALL remain subject to verified capability and licence (the Echelon stack is PolyForm Noncommercial; a commercial licence SHALL be in place before commercial launch). Conditor SHALL be mandatory before implementation; the requirements-only waiver SHALL end at the implementation gate.
  - Provenance: resolves SPD-PLT-003, XID-ERP-005, SPD-ENT-001, SPD-ENT-008, XID-GOV-001, SPD-CLR-005, SPD-CMP-004, RCD-027, RCD-002, RCD-003; amends UNB-GOV-002, REQ-X-058. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Commercial licence for Echelon components before commercial launch.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given two approvals of $9,000 each to the same counterparty within 30 days by different approvers under a $10,000 limit, when the second is approved, then cumulative-limit escalation triggers.
    - AC-2: Given two $9,000 grants from unrelated funds to the same charity within 30 days, when approved, then no cumulative escalation occurs and the analytic records the pattern.
- **CNV-GOV-008 — Board oversight of delegated automation and ratification status:** Where a board has delegated approval to a deterministic policy, the platform SHALL produce a quarterly board report of auto-approved, held, declined and human-reviewed grants with sampled human reviews, evidencing that the sponsor exercises variance power and exclusive legal control, and SHALL refuse to renew a delegation whose last report was not acknowledged by the board or committee. Clauses whose resolving questions are agent-recommended SHALL be shown as unratified in the baseline index and SHALL NOT be cited in customer-facing material until the owner ratifies them. A sponsor that does not adopt a mandatory policy-pack change within its deadline SHALL have the affected feature fail safe. Every delegation SHALL carry an expiry no later than 12 months after adoption [policy default, owner: sponsor board]; board-report samples SHALL be sized by the CNV-GRT-011 error-bound method. The baseline index SHALL show each clause's approval basis, and an approved-claims register keyed to clause ids SHALL be checked before any customer-facing publication.
  - Provenance: resolves SPD-GRT-001; amends REQ-X-007. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: sponsor-activation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor board acknowledgment of quarterly delegation reports.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given a delegation whose quarterly report is unacknowledged, when renewal is requested, then it is refused.
    - AC-2: Given an unratified clause, when marketing copy cites it, then the content check fails.
    - AC-3: Given a delegation recorded without an expiry, when saved, then it is rejected.
- **CNV-GOV-009 — Commercial licence and dependency rights:** No release-1 production dependency SHALL be under a non-commercial licence without an executed commercial licence plus escrow or a fork right; a software bill of materials licence check SHALL fail on PolyForm Noncommercial components lacking such a licence. Dependency versions SHALL be pinned to tagged releases with digests through Conditor.
  - Provenance: resolves RCD-027; amends —. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: commercial-availability; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Executed commercial licence for Echelon components.
  - Ownership: domain owner: Product; financial system of record: n/a; policy owner: Unbound product owner; data owner: Unbound; operational owner: Unbound operations; evidence owner: Unbound product owner.
  - Acceptance:
    - AC-1: Given an SBOM containing a PolyForm Noncommercial component with no licence record, when the release check runs, then it fails.
- **CNV-GOV-010 — One authority matrix (ADV-T-008):** A single normative AuthorityMatrix SHALL map each command class × actor type {deterministic policy, AI agent, staff role, appointed officer, board} × authority level, replacing the coexisting tier vocabularies; 'Tier 2' SHALL mean only enhanced human review (CNV-GRT-011). Every API command SHALL declare its class; commands absent from the matrix SHALL be refused. The matrix SHALL define one level set, map recommend, prepare and execute to levels, classify routine board-delegated grant approval (CNV-GRT-001) as the level deterministic policy may hold under delegation, identify the reserved level that replaces 'A3+' wording elsewhere, and classify every external communication and payment-instruction preparation by actor type; CI SHALL fail when any clause or API contract uses a level outside the set.
  - Provenance: resolves Phase 3 feedback; amends UNB-RCN-PLT-005. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Governance; financial system of record: n/a; policy owner: Sponsor board / Unbound product owner; data owner: Sponsor; operational owner: Sponsor executive; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a command not mapped in the matrix, when invoked, then it is refused with UnclassifiedCommand.
    - AC-2: Given a clause text containing an authority label outside the defined set, when the baseline builds, then it fails.
- **CNV-GOV-011 — Baseline integrity and numeric basis (ADV-T-010, ADV-T-011, ADV-T-032, ADV-T-041):** Each baseline row SHALL carry a SHA-256 of its exact source text plus file and line, its status derived from adoption markers, and, for convergence clauses, its approval basis and resolving questions. Every clause classified as an internal control SHALL map to at least one entry in the authoritative control register, which SHALL record control operator {Unbound, sponsor, administrator, rail partner, custodian}, report relevance {SOC1, SOC2, none}, complementary user entity control id, automated or manual and the evidence query. Clause dependencies SHALL be declared and the dependency graph SHALL be acyclic. Numeric values in normative clauses SHALL carry a basis tag {authority, business-policy default, empirical target, illustrative}. Each clause SHALL carry a numeric-parameter table (value, unit, basis tag, owner) and generation SHALL fail for any number in a clause statement not listed there; counts and decision status quoted in mission reports SHALL be generated from the clause and decision files.
  - Provenance: resolves Phase 3 feedback; amends —. Evidence and reasoning: [../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](../convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](../convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: none beyond release scope.
  - Ownership: domain owner: Governance; financial system of record: n/a; policy owner: Sponsor board / Unbound product owner; data owner: Sponsor; operational owner: Sponsor executive; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given a clause dependency cycle, when the baseline is generated, then generation fails.
    - AC-2: Given a control clause without a control-register mapping, when the baseline validator runs, then it reports the clause.
<!-- END ADOPTED: convergence-2026-11-24 -->

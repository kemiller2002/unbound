# Initial 38 requirement question resolutions

Status: proposed, not approved. Source: [open questions register](../OPEN-QUESTIONS.md), requirement questions section. Original record IDs are preserved; repeated IDs denote distinct questions. The previous chat contained 37 answers despite reporting 38; this file adds the omitted RR-REQ-X-073-r1 threshold question. Source-level reconciliation and landscape validation remain outstanding. Numerical defaults are hypotheses. Professional confirmations remain open. No implementation or production authorization.

| Original record | Question | Proposed answer | Challenge / validation gate |
|---|---|---|---|
| RR-ADM-002-r1 | Identity prefill | Verified IdPs, trusted financial integrations and previously verified records; provenance and correction required. | Test stale and conflicting identity evidence. |
| RR-ADM-002-r1 | Material agreement change | Fees, advisory privileges, succession, privacy permissions and substantive obligations require material-change review. | Counsel confirms classification. |
| RR-ADM-003-r1 | Board delegation | Build deterministic acceptance with human fallback; activate only after lawful delegation. | Counsel and board approval. |
| RR-ADM-023-r1 | Joint mode | Any authorized spouse for routine grants; joint approval for closure, succession and authority changes. | Sponsor agreement and counsel; test conflicting instructions. |
| RR-OPS-004-r1 | Auditor acceptance | Continuous evidence; replace manual procedures only after auditor acceptance. | Compare evidence completeness. |
| RR-REQ-X-007-r2 | Renewal approval | Deterministic approval under authorized sponsor policy. | Counsel and board approval. |
| RR-REQ-X-007-r2 | Sampling size | Provisional risk-stratified 30 per populated stratum monthly or all if fewer; recalibrate by error rates. | Compliance validates statistical adequacy. |
| RR-REQ-X-007-r3 | Cross-sponsor reliance | Share independently verified public facts, not private evidence or sponsor approval decisions. | Counsel; test tenant isolation. |
| RR-REQ-X-032-r1 | Statement delivery | Configurable delivery obligations, paper and accessible alternatives. | Counsel verifies jurisdiction and terms. |
| RR-REQ-X-048-r3 | Assignment mode | Round robin, skill, relationship, cradle-to-grave and escalation modes by case kind. | Test absence, overload and conflicts. |
| RR-REQ-X-073-r1 | 3/12 month thresholds | Treat thresholds as provisional; configurable only within approved control policy. | Auditor validates underlying control objective; source review required. |
| RR-SOC2-024-r1 | Auditor acceptance | Evidence engineering from inception; assurance timing driven by contracts and auditor reliance. | Auditor confirms scope. |
| RR-TVR-CON-001-r1 | Card window | Version rules by network, acquirer and reason code. | Acquirer confirms rules before activation. |
| RR-UNB-ACC-004-r1 | Journal | Common double-entry component with distinct books and entity boundaries. | Controller confirms RCF-FIN-008; test balanced postings and isolation. |
| RR-UNB-ACC-008-r1 | Accounting basis | One authoritative accrual book per entity with reconciled derived bases where appropriate. | CPA confirms. |
| RR-UNB-ACC-014-r2 | Budgeting | External budgeting integration, no general-purpose native budgeting. | Test export and reconciliation. |
| RR-UNB-ACC-017-r1 | Generated evidence | Replace manual checklists only when auditor accepts equivalent generated evidence. | Test completeness and tamper evidence. |
| RR-UNB-ACC-022-r1 | Reporting scope | DAF program reporting, reconciliation and audit exports; consolidated entity statements in ERP. | Controller and auditor confirm. |
| RR-UNB-API-STD-003-r1 | Deprecation | Propose 18 months notice; documented emergency exceptions. | Assess maintenance cost and consumer migration. |
| RR-UNB-CHAR-013-r1 | Precision | Select thresholds from labeled gold set, measured precision and false-approval cost. | Independent validation; no arbitrary confidence threshold. |
| RR-UNB-DATA-004-r1 | Facts | Separate authoritative business facts from engagement and coordination data; preserve relevant audit history. | Test provenance and retention. |
| RR-UNB-ECO-003-r1 | Authority holder time | Coverage and workload based; 0.5 FTE is scenario assumption. | Roster stress test. |
| RR-UNB-EXT-004-r2 | Disposition reuse | Invalidate on identity or list changes; propose 12 month maximum. | Counsel approval. |
| RR-UNB-EXT-028-r1 | Detection SLA | Risk-tiered source freshness; fresh eligibility check before release. | Measure source latency; fail safely when stale. |
| RR-UNB-EXT-028-r1 | Informational class | Do not block solely informational changes absent eligibility, authority, payment or policy impact. | Counsel confirms class list. |
| RR-UNB-GFUND-012-r1 | Funding source consent | Notify rather than reapprove if recipient, amount, purpose, timing and donor costs unchanged. | Counsel confirms agreement terms. |
| RR-UNB-GOV-002-r1 | Auditor acceptance | Single authoritative control register with evidence, ownership, testing and exceptions. | Auditor confirms adequacy; source review required. |
| RR-UNB-GRANT-024-r1 | Tier 2 | Ambiguous eligibility, prohibited benefit risk, conflicting evidence and unusual restrictions. | Counsel confirms non-ER cases. |
| RR-UNB-GRANT-030-r1 | Thresholds | Risk and decision-type thresholds, not amount alone. | Calibrate and obtain delegated approval. |
| RR-UNB-GRANT-030-r1 | Sampling | Risk-stratified independent review; increase after material errors or policy changes. | TB-09 validation. |
| RR-UNB-GRT-003-r1 | Exception incidence | 90-day pilot; label interim scenario assumptions. | Measure incidence and handling time. |
| RR-UNB-GRT-003-r1 | Amount-based review | Allow sponsor-specific thresholds, not universal human review by amount alone. | Sponsor and counsel confirm. |
| RR-UNB-RES-003-r1 | RTO | Capability-specific recovery objectives; prioritize financial integrity and payment protection. | Set numerical targets after impact analysis and recovery tests. |
| RR-UNB-RULE-019-r1 | Emergency changes | Documented emergency authorization, impact analysis and independent retrospective review. | Test rollback and audit. |
| RR-UNB-RULE-019-r1 | Observation | Propose seven days for ordinary material changes, adjusted for volume and risk. | Shadow replay and minimum sample coverage. |
| RR-UNB-TEN-016-r1 | Retention and cost | Complete export; contract specifies read-only duration and payer; mandatory retention overrides deletion. | Counsel and commercial approval. |
| UNB-SMP-ARX-001 | Suspension SLA | Safe read-only access and status; no unsafe processing promises. | Define communication SLA and test outage. |
| UNB-SMP-ARX-001 | HumanQueue | Genuine exceptions, reserved authority, unresolved evidence conflicts and mandatory independent controls only. | Test routing and no bypass. |

## Reconciliation gate

Locate each original source record, verify context, add precise acceptance criteria and dependencies, record alternatives and counterarguments, and update source records only with appropriate approval status. Regenerate the register using tools/reqaudit/render_questions.py rather than hand-editing it. No changes to daf-landscape.

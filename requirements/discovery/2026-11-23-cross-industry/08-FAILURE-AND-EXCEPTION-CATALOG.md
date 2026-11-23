# 08 — Failure and exception catalog (deliverable 8, mission §8)

Each row is one way a significant capability fails. For each, the record gives:
- how the failure is detected;
- how it is recovered;
- where it is escalated;
- the audit evidence it leaves;
- what the customer is told;
- the existing requirements that already cover it (`covered_by`);
- any new requirements it produced (`new_ids`).

Gate D6 checks every reference.

**How this relates to existing coverage.** The baseline already contains deep adversarial work: `ADV-*`, `TVS-*`, `SIM-01..28` and the reconciliation's `DKS-*` scenarios. This catalog adds the failure modes that come from cross-industry patterns, for example:
- out-of-band bank debits;
- card disputes;
- Nacha return-rate thresholds;
- posting racing period close;
- a sanctions hit in the middle of a batch;
- a stale "paid" notice after a return;
- a partner exit done as a data migration;
- holds that never reach processors;
- break-glass access;
- metering gaps;
- an AI budget running out.

## Failure modes covered

<!-- BEGIN GENERATED: failure-modes -->
| Failure mode | Catalogued cases |
|---|---|
| tenant misconfiguration | 10 |
| conflicting policy | 10 |
| unauthorized instruction | 8 |
| missing information | 8 |
| outage | 8 |
| incorrect ai recommendation | 8 |
| other | 8 |
| repeated processing | 7 |
| partial completion | 7 |
| vendor failure | 7 |
| data corruption | 7 |
| changed external information | 7 |
| duplicate request | 6 |
| stale verification | 6 |
| customer dispute | 5 |
| regulatory change | 5 |
| incorrect accounting entry | 5 |
| unavailable employee | 5 |
| incorrect identity | 4 |
| hallucination | 4 |
| compromised credentials | 4 |
| failed payment delivery | 3 |
| delayed approval | 3 |
| failed integration | 3 |
<!-- END GENERATED: failure-modes -->

## Cross-cutting recovery rules

1. **Unknown external outcomes.** Never retry blindly. Resolve the outcome first (REQ-X-004, INV-04).
2. **Missing feeds.** A missing feed means *Unverified*, never zero activity. Automation that depends on the feed is held (UNB-XIN-EVA-004, UNB-XIN-BNK-003).
3. **Duplicates.** Detect them with a business idempotency key plus a payload hash. If the same key arrives with a different payload, open a conflict case. Never drop the second message silently (UNB-XIN-EVA-005).
4. **Restores.** After a restore, payments stay blocked until reconciliation passes, and each timer fires exactly once (RR-UNB-RES-003-r1, UNB-XIN-WFA-001).
5. **AI failures.** These include incorrect recommendations, hallucinations, and running out of budget. An AI failure can never pass a control: the result is "Incomplete" and the work goes to a human (RR-AIN-028-r1, UNB-XIN-ECO-009, RR-UNB-EVT-005-r1).
6. **Credential compromise.** Use step-up re-authentication, revocation that propagates to delegated and agent principals, and break-glass access limited to protective actions (UNB-XIN-IAM-003, -005).
7. **Customer communication.** Before any external send, re-check that the underlying fact is still current. Suppress stale notices (UNB-XIN-EVA-015).

## Catalog

<!-- BEGIN GENERATED: failures -->
| ID | Capability | Failure mode | Scenario | Detection | Recovery | Escalation | Covered by / new |
|---|---|---|---|---|---|---|---|
| XFC-BNK-001 | ACH grant origination | unauthorized instruction | Compromised bank-portal user keys a wire outside Unbound. | Daily unmatched-debit match. | Hold account releases; recall request; insurance claim. | Treasury + CFO within 1h. | TVR-PAY-004, TVF-223, UNB-XIN-BNK-003 |
| XFC-BNK-002 | Destination integrity | incorrect identity | Fraudster impersonates charity and changes bank details. | Destination-change rules; first-payment alert. | Hold; independent callback. | Compliance. | REQ-X-005, UNB-RCN-GRT-017, UNB-XIN-BNK-002 |
| XFC-BNK-003 | Card contributions | customer dispute | Donor disputes card gift after grant. | Dispute webhook. | Exposure; capacity reduction; contest decision. | Finance above threshold. | REQ-X-009, UNB-XIN-PAY-003 |
| XFC-BNK-004 | Card contributions | repeated processing | Card testing burst. | Velocity rules. | Suspend channel. | Sponsor admin. | UNB-XIN-PAY-004 |
| XFC-BNK-005 | Recurring contributions | missing information | No authorization copy. | Pre-origination check. | Hold occurrence. | Donor ops. | UNB-XIN-PAY-001 |
| XFC-BNK-006 | ACH debit contributions | failed payment delivery | R10 return after acknowledgment. | Return file. | Exposure; ack correction; no reinitiation. | Finance. | REQ-X-009, RR-UNB-ACC-006-r1, UNB-XIN-PAY-005 |
| XFC-BNK-007 | Processor settlement | partial completion | Payout batch net differs from gifts. | Three-way rec. | ExceptionOpen; contact processor. | Controller after age. | UNB-PAY-010, UNB-XIN-BNK-008 |
| XFC-BNK-008 | Securities intake | duplicate request | Custodian feed delivers same receipt twice. | Provider event dedup. | One receipt. | None. | RR-UNB-ACC-006-r1, UNB-XIN-PAY-007 |
| XFC-BNK-009 | Securities intake | missing information | Delivery without donor identity. | No deterministic match. | Suspense; outreach. | Ops after aging. | UNB-GAP-013, RR-UNB-GAP-013-r1, UNB-XIN-PAY-007 |
| XFC-BNK-010 | Check rail | failed payment delivery | Check stolen and washed. | Payee-name positive pay exception. | Default Return; void; reissue via ACH. | Treasury. | UNB-RCN-GRT-005, UNB-XIN-BNK-006 |
| XFC-BNK-011 | Check rail | stale verification | Outstanding check passes dormancy. | Aging job. | Rule-table disposition. | Counsel if no row. | UNB-RCN-GRT-005, UNB-XIN-BNK-007 |
| XFC-BNK-012 | Bank integration | vendor failure | Bank statement feed missing. | Cut-off monitor. | Mark Unverified; re-pull. | Platform ops. | UNB-XIN-BNK-003 |
| XFC-BNK-013 | Disbursement liquidity | partial completion | Liquidation fails T+1, ACH file due. | Cash ladder shortfall. | Hold releases or liquidity advance. | Treasury. | UNB-RCN-FIN-008, UNB-XIN-BNK-009 |
| XFC-BNK-014 | Rail compliance | regulatory change | Nacha amends rule. | Watch list / register version. | Control review before effective date. | Compliance owner. | UNB-RCN-REG-001, UNB-XIN-BNK-001 |
| XFC-BNK-015 | Payment dispatch | outage | Provider timeout. | Dispatch Unknown. | Unknown resolution protocol. | Payments ops. | REQ-X-004, UNB-RCN-GRT-008 |
| XFC-BNK-016 | Rail configuration | tenant misconfiguration | Sponsor enables RTP for grants without approval. | Matrix change approval. | Reject change. | Compliance. | UNB-XIN-BNK-004 |
| XFC-BNK-017 | Fraud alerts | incorrect AI recommendation | AI ranks a BEC alert low and analyst skips it. | Deterministic hold independent of AI rank. | Hold stays until cleared with rationale. | Compliance. | REQ-X-072, UNB-XIN-BNK-002 |
| XFC-INV-001 | Posting engine | repeated processing | Replay after rule change posts duplicates. | (event,book) key collision. | Return existing journal. | Controller if collision with different amounts. | ACC-MDL-007, RR-ACC-MDL-007-r1 |
| XFC-INV-002 | Posting engine | data corruption | Code change alters posting outputs. | Re-derivation diff. | Fix code; classify any posted error. | Controller. | UNB-NAV-044, UNB-XIN-ACC-003 |
| XFC-INV-003 | Grant recognition | delayed approval | Award communication fails at year end. | Cutoff register. | Recognize when communicated. | Controller at close. | OD-19, UNB-XIN-ACC-002 |
| XFC-INV-004 | Net asset classification | missing information | Agreement lacks variance power clause. | Suspense classification. | CPA classification. | Controller. | UNB-ACC-033, RR-UNB-ACC-033-r1 |
| XFC-INV-005 | Agency funds | tenant misconfiguration | Agency account set as DAF. | Bridge item / review. | Accounting change + reclass. | CPA. | UNB-RCN-ENT-011, UNB-XIN-ACC-001 |
| XFC-INV-006 | Audit adjustments | incorrect accounting entry | AJE posted to December after close. | Period validation. | Reject; post to AuditAdjustment. | Controller. | UNB-ACC-017, UNB-XIN-ACC-005 |
| XFC-INV-007 | Schedule D bridge | conflicting policy | Line 4 basis differs from GAAP net assets. | Residual. | Typed item or correction. | CFO. | RR-REQ-X-060-r1, UNB-XIN-ACC-006 |
| XFC-INV-008 | UBTI | changed external information | K-1 arrives showing UBTI on an LP thought clean. | K-1 ingestion. | Reclassify exposure; estimate tax. | Tax advisor. | FI-37, UNB-XIN-ACC-008 |
| XFC-INV-009 | Advisor fee debit | unauthorized instruction | Advisor debits after mandate revoked. | No active mandate. | Hold + refund request. | Sponsor investment ops. | CTL-025, UNB-XIN-INV-001 |
| XFC-INV-010 | Advisor fee debit | vendor failure | Custodian does not expose fee as distinct transaction. | Unmatched cash decrease. | Manual classification. | Ops. | UNB-CUST-011, UNB-XIN-INV-001 |
| XFC-INV-011 | Pools | conflicting policy | Partner requests a shared pool across its two sponsors. | Pool eligibility check. | Reject pending counsel. | Counsel. | ADV-131, UNB-XIN-INV-002 |
| XFC-INV-012 | AI investment proposals | incorrect AI recommendation | AI recommends off-model security. | Model reference check. | Reject. | None. | UNB-INV-019, RR-UNB-INV-019-r1 |
| XFC-INV-013 | Corporate action elections | unavailable employee | Decider on leave before deadline. | Deadline monitor. | Policy default. | Investment committee chair. | UNB-XIN-INV-003 |
| XFC-INV-014 | Alternative valuation | stale verification | GP statement 200 days old. | Lag > max. | Provisional + haircut. | Controller. | UNB-NAV-017, UNB-XIN-INV-005 |
| XFC-INV-015 | Crypto | outage | Broker outage prevents liquidation over year end. | Exposure window. | Fair value at cutoff. | Controller. | UNB-RCN-FIN-011, UNB-XIN-INV-007 |
| XFC-INV-016 | Book of record | failed integration | Administrator GL feed missing at close. | Freshness check. | Close blocked for entity. | CFO. | UNB-ACC-028, UNB-XIN-ACC-010 |
| XFC-INV-017 | Sweep income | partial completion | Allocation run crashes midway. | Unattributed != 0. | Idempotent rerun. | Ops. | A-9, UNB-XIN-INV-006 |
| XFC-SVC-001 | Contact authentication | incorrect identity | Impostor passes knowledge questions | Assurance matrix denies disclosure; anomaly on repeated failures | Step-up or callback to registered number | Fraud case if pattern | TVF-132, UNB-RCN-ENT-016, UNB-XIN-SVC-004 |
| XFC-SVC-002 | Assisted change | unauthorized instruction | Staff-entered payee change from phone | Pending verified-channel confirmation | Expires unconfirmed | Fraud case on rejection | UNB-RCN-ENT-016, RR-UNB-PART-007-r1 |
| XFC-SVC-003 | Recording | other | Recording without disclosure due to adapter misconfiguration | Adapter conformance check, consent-evidence missing | Discard recording, disable | Counsel | REQ-X-025, UNB-XIN-SVC-003 |
| XFC-SVC-004 | AI summary | hallucination | Summary invents donor instruction | Non-authority invariant; verification step | Draft only; donor affirmation required | QA | AIN-014, UNB-XIN-SVC-009 |
| XFC-SVC-005 | AI answer | incorrect AI recommendation | Answer cites superseded clause | Validator checks clause effective dates | Correct and recall list | Corpus owner | UNB-XIN-SVC-007 |
| XFC-SVC-006 | Complaint | other | Complaint misclassified as inquiry | QA sample; repeat-contact signal | Reclassify; clock from original contact | Supervisor | CTL-040, UNB-XIN-SVC-005, RR-CTL-040-r1 |
| XFC-SVC-007 | Case routing | unavailable employee | Owner on leave; customer-initiated case unassigned | Backup routing; at-risk clock | Reassign to backup | Supervisor | REQ-X-048, RR-REQ-X-048-r2 |
| XFC-SVC-008 | Commitments | delayed approval | Promised callback missed | Pre-breach alert | Backup handles; proactive update | Supervisor | UNB-XIN-SVC-006 |
| XFC-SVC-009 | Surge | outage | Telephony vendor outage on Dec 31 | Adapter health | Switch to async + banner; extend callbacks | Ops lead | REQ-X-047, UNB-XIN-SVC-008 |
| XFC-SVC-010 | Case kernel | tenant misconfiguration | Profile with no calendar activated | Activation validation | Block activation | Config owner | UNB-XIN-SVC-002, RR-ADM-033-r1 |
| XFC-SVC-011 | Case kernel | data corruption | Restricted case appears in unified search projection | Access-class property test; projection audit | Purge projection; rebuild | Security incident | UNB-GEX-011, UNB-XIN-CLM-004 |
| XFC-SVC-012 | Reconsideration | conflicting policy | Policy changed between decision and reconsideration | Pinned version in original record | Reconsider on current policy; record both | Sponsor reviewer | ADV-015, TVR-DATA-001, UNB-XIN-CLM-001 |
| XFC-SVC-013 | Decision notice | other | Notice discloses sanctions reason | Catalog mapping test; restricted codes neutral | Correct template; incident | Compliance | UNB-XIN-CLM-002 |
| XFC-SVC-014 | RFI | missing information | Charity never answers | Due date | Close incomplete with reopen right | None | UNB-XIN-CLM-003 |
| XFC-SVC-015 | RFI | duplicate request | Re-asking held document | Registry check | Remove item | None | AIN-007, UNB-XIN-CLM-003 |
| XFC-SVC-016 | Investigation | other | Subject tipped off by agent | Neutral script; access class | Incident; adjust | Compliance | UNB-GEX-033, UNB-XIN-CLM-004 |
| XFC-SVC-017 | Decision quality | incorrect accounting entry | STP approval to ineligible grantee discovered by sample | Leakage sample | Remediation case; child record per SM-0 | Compliance + counsel | SM-0, EXC-08, UNB-XIN-CLM-005 |
| XFC-SVC-018 | Representative | compromised credentials | Representative credentials used by third party | Anomaly + exploitation facts | Protective suspension | Sponsor reviewer | REQ-X-072, UNB-XIN-CLM-006 |
| XFC-SVC-019 | BPO | vendor failure | BPO staff departs with access | Vendor offboarding feed | Revocation within 1h | Security | REQ-X-073, UNB-XIN-CLM-008 |
| XFC-SVC-020 | Case reopen | repeated processing | Closed complaint reopened repeatedly creating case chains | Reopen window in profile | Reopen within window; else linked case | Supervisor | SM-0, UNB-GEX-023, UNB-XIN-SVC-002 |
| XFC-ERP-001 | Fee run | repeated processing | Fee run retried after timeout. | Idempotency key hit. | No-op. | None. | UNB-SCTRL-032, RR-ADM-028-r1 |
| XFC-ERP-002 | Fee run | incorrect accounting entry | Wrong tier method. | Recompute mismatch / SCTRL-032. | Compensating entries. | Controller. | UNB-SCTRL-032, UNB-RCN-FIN-005, UNB-XIN-ERP-001 |
| XFC-ERP-003 | Fee run | tenant misconfiguration | Schedule without payer source. | Activation validation. | Blocked. | Sponsor finance. | UNB-XIN-ERP-002 |
| XFC-ERP-004 | Fee run | changed external information | Valuation restated after fee assessed. | NAV restatement event. | Re-run segments with new version; compensating delta. | Controller. | UNB-NAV-033, UNB-XIN-ERP-001 |
| XFC-ERP-005 | Partner billing | customer dispute | Partner disputes invoice. | Dispute case. | Credit note by compensating entry. | Sponsor finance. | REQ-X-067, UNB-XIN-ERP-002 |
| XFC-ERP-006 | Payee separation | incorrect identity | Fiscal sponsor bank account shared by vendor and charity. | Cross-match. | Review. | Compliance. | UNB-RCN-GRT-002, UNB-XIN-ERP-004 |
| XFC-ERP-007 | DoA | stale verification | HR feed delayed 2 days. | Feed freshness monitor. | Protective restriction of high-value approvals when feed stale. | Security. | GOV-AUTH-025, UNB-XIN-ERP-005 |
| XFC-ERP-008 | DoA | unavailable employee | Only approver at limit is on leave. | Escalation SLA. | Substitute approver per GOV-AUTH-005. | Sponsor officer. | GOV-AUTH-005 |
| XFC-ERP-009 | Vendor assurance | vendor failure | Payment provider SOC report qualified. | Metadata review. | Risk acceptance or compensating control. | Vendor risk owner. | REQ-X-073, RR-REQ-X-073-r1 |
| XFC-ERP-010 | Integration | failed integration | Journal export to sponsor ERP fails. | Control-account tie-out break. | Re-export idempotently. | Controller. | UNB-ACC-028, RR-UNB-ACC-012-r1 |
| XFC-CRM-001 | Party model | data corruption | Two parties wrongly merged. | Screening anomaly / user report. | Unmerge by events; re-evaluate visibility. | Privacy officer. | UNB-DATA-001, UNB-XIN-CRM-001 |
| XFC-CRM-002 | Communications | conflicting policy | Sponsor marks commercial content transactional. | AI flag + reviewer. | Reclassify; suppress. | Counsel. | UNB-RCN-PLT-002, UNB-XIN-CRM-003 |
| XFC-CRM-003 | Communications | vendor failure | ESP fails to apply suppression. | Send-time suppression check in platform. | Block sends. | Communications owner. | UNB-XIN-CRM-003 |
| XFC-CRM-004 | Segmentation | incorrect AI recommendation | AI proposes segment using inferred religion. | Allowlist lint. | Reject. | — | UNB-XIN-CRM-004 |
| XFC-CRM-005 | Advisor relationships | compromised credentials | Departed advisor's firm SSO still active. | Firm feed / AUTH-I8. | Revoke. | Security. | AUTH-I8, AUTH-I6, UNB-XIN-CRM-007 |
| XFC-TRS-001 | Succession | missing information | Successor unreachable. | Response window. | Contingent/default. | Sponsor. | UNB-SUCC-007, UNB-XIN-TRS-005 |
| XFC-TRS-002 | Succession | changed external information | Named charity revoked. | Revalidation at activation. | Fallback chain. | Sponsor. | UNB-SUCC-011, UNB-XIN-TRS-001 |
| XFC-TRS-003 | Representatives | unauthorized instruction | POA agent beyond scope. | Scope check. | Reject. | Counsel if disputed. | UNB-RCN-ENT-010, UNB-XIN-TRS-004 |
| XFC-TRS-004 | Conflicting instructions | duplicate request | Same recommendation from two co-advisors. | Scope overlap detection. | Treat as one if identical; else conflict. | — | UNB-PART-005, RR-ADM-023-r1 |
| XFC-TRS-005 | Conflicting instructions | customer dispute | Family members dispute succession plan version. | Dispute case. | Hold; latest approved plan only. | Counsel. | UNB-SUCC-007, RR-UNB-SUCC-008-r1, RR-ADM-023-r1 |
| XFC-NPG-001 | Equivalency determination | stale verification | ED past CurrentUntil used for release. | Currency check at release. | Hold; renew ED. | Compliance if renewal > hold window. | UNB-GRANT-041, UNB-XIN-NPG-001 |
| XFC-NPG-002 | Equivalency determination | incorrect AI recommendation | AI-extracted support figures wrong. | Practitioner confirmation required. | Correct record. | Counsel. | UNB-XIN-NPG-001 |
| XFC-NPG-003 | Intermediary routing | conflicting policy | Donor text creates earmark. | Purpose text must equal catalogue wording. | Reject edit. | None. | UNB-XIN-NPG-002 |
| XFC-NPG-004 | Fiscal sponsorship | incorrect identity | Payee set to project account. | Payee must equal fiscal sponsor. | Reject. | Compliance if repeated. | UNB-GRANT-007, UNB-XIN-NPG-003 |
| XFC-NPG-005 | Schedule I/F | missing information | Foreign grant lacks region. | Approval validation. | Capture region. | None. | UNB-XIN-NPG-004 |
| XFC-NPG-006 | Organization changes | changed external information | Merger detected after grant dispatched. | Change effect table. | Recall flow; substitution approval. | Sponsor. | UNB-RCN-GRT-016, UNB-XIN-NPG-005 |
| XFC-NPG-007 | Organization changes | stale verification | Retroactive reinstatement not applied; grant stays held. | Reinstatement event triggers re-evaluation. | Release if covered. | None. | RR-REQ-X-007-r1, UNB-XIN-NPG-005 |
| XFC-NPG-008 | Discretionary holds | delayed approval | Hold renewal not decided by review date. | Review-date timer. | Auto-release. | Board committee notified before date. | UNB-CHAR-031, UNB-XIN-NPG-006 |
| XFC-NPG-009 | Discretionary holds | hallucination | AI summary asserts indictment that did not occur. | Evidence class must cite source document; AI cannot set class. | Discard summary. | Compliance. | UNB-EXT-030, UNB-XIN-NPG-006 |
| XFC-NPG-010 | Excise-tax incidents | partial completion | Repayment requested but not received before filing deadline. | Incident deadline tracking. | File with correction status. | Counsel/CPA. | UNB-XIN-NPG-008 |
| XFC-NPG-011 | Sanctions authorizations | regulatory change | General license amended. | License version watch. | Re-hold dependent grants. | Counsel. | UNB-RCN-GRT-007, UNB-XIN-NPG-010 |
| XFC-NPG-012 | Unclaimed property | tenant misconfiguration | State table missing row. | Lookup failure. | Hold report task; alert. | Treasury. | UNB-RCN-GRT-005, UNB-XIN-NPG-011 |
| XFC-NPG-013 | Recipient class | regulatory change | Temp reg expires; class still Permitted. | Sunset rule. | CounselReview. | Counsel. | UNB-RCN-GRT-001, UNB-XIN-NPG-007, UNB-XIN-GOV-001 |
| XFC-REG-001 | Funds flow | vendor failure | Processor sweeps funds into operator account; processor insolvency. | FundsFlow declaration + bank statements title check. | Freeze activation; claim funds. | Sponsor board, counsel. | UNB-XIN-REG-001 |
| XFC-REG-002 | Operator status | tenant misconfiguration | Feature enabled for new deployment without determination. | Register gate. | Disable. | Counsel. | REQ-X-064, UNB-XIN-REG-002 |
| XFC-REG-003 | Communications | unauthorized instruction | SMS sent after revocation. | Pre-send consent check. | Suppress; incident. | Counsel. | UNB-RCN-PLT-002, UNB-XIN-REG-003 |
| XFC-GOV-001 | Rule engine | data corruption | Golden fixture edited to match wrong rule. | Fixture changes require counsel approval. | Revert. | Counsel. | UNB-RULE-020, UNB-XIN-GOV-001 |
| XFC-SAS-001 | Tenant routing | tenant misconfiguration | Deployment binding points tenant A to tenant B's database after migration. | Broker verifies tenant id stored in target DB metadata equals context. | Fail closed; revert binding generation. | Security incident P0. | UNB-TEN-004, RR-UNB-TEN-004-r1, UNB-TEN-013, UNB-XIN-SAS-005 |
| XFC-SAS-002 | Release rollout | data corruption | Expand migration corrupts a projection in canary cell. | Invariant checks in promotion gate. | Halt; rebuild projection from events. | Release owner. | UNB-DATA-004, UNB-XIN-SAS-003 |
| XFC-SAS-003 | Feature flags | conflicting policy | Kill switch disables grant intake while hold-release depends on same flag. | Flag dependency lint. | Kill switch only stops new commands. | Architecture. | UNB-XIN-SAS-002 |
| XFC-SAS-004 | Webhooks | repeated processing | Charity system processes 'grant paid' webhook twice and books two gifts. | Event id dedupe guidance; tenant sequence. | Subscriber reconciles by event id. | Partner support. | UNB-API-006, UNB-XIN-SAS-004 |
| XFC-SAS-005 | Webhooks | outage | Subscriber endpoint down 4 days. | Dead-letter after 72h. | Subscriber redelivers within 30 days. | Partner notified. | UNB-XIN-SAS-004 |
| XFC-SAS-006 | Partner exit | partial completion | Half of transfer grants paid when sponsor B's bank rejects incoming wires. | Payment returns; exit reconciliation shows gap. | Returned funds re-credited to fund (UNB-RCN-GRT-015); retry after destination fixed. | Sponsor A treasury. | UNB-RCN-GRT-015, REQ-X-083, UNB-XIN-SAS-007 |
| XFC-SAS-007 | Partner exit | customer dispute | Partner claims donors 'own' funds and demands bulk move. | Case intake. | Explain legal control; offer per-fund election. | Sponsor counsel. | UNB-LEG-002, ADM-041, UNB-XIN-SAS-007 |
| XFC-SAS-008 | Metering | missing information | Job emits usage without tenant. | Unattributed usage report. | Fix producer; reallocate. | Economics owner. | UNB-XIN-SAS-009 |
| XFC-SAS-009 | Sandbox | tenant misconfiguration | Sandbox bound to production adapter. | Config validation + egress block. | Reject config. | Security. | UNB-XIN-SAS-010 |
| XFC-SAS-010 | DR | outage | Region loss; restore to standby. | Health checks. | Restore C0 to common point; block payments until reconciled. | Incident commander. | REQ-X-006, REQ-X-047, RR-UNB-RES-003-r1 |
| XFC-SAS-011 | Sponsor exit | vendor failure | Platform vendor insolvent before export. | Contractual escrow trigger. | Export package and schema escrowed; read-only continuity. | Sponsor board. | UNB-GAP-028, RR-UNB-TEN-016-r1 |
| XFC-IAM-001 | Operator access | unauthorized instruction | Operator with JIT changes payee directly in DB. | Command/state integrity check (TVF-139). | Block payments; revert via correction command. | CISO + sponsor. | CTL-031, TVF-139, UNB-XIN-IAM-002, UNB-XIN-IAM-005 |
| XFC-IAM-002 | Federation | compromised credentials | Partner IdP compromised; attacker logs in as donors. | Anomaly + step-up on sensitive actions. | Sponsor disables federation; donors use sponsor-direct factor. | Security. | REQ-X-072, CTL-052, UNB-XIN-IAM-001, UNB-XIN-IAM-003 |
| XFC-IAM-003 | Access review | unavailable employee | Reviewer on leave; reviews lapse. | Due-date monitor. | Auto-suspend; delegate reviewer. | Security owner. | GOV-AUTH-025, UNB-XIN-IAM-004 |
| XFC-IAM-004 | Agent principal | incorrect AI recommendation | AI agent proposes enabling an app with broad scope. | Prohibited-scope rule. | Proposal rejected. | Sponsor admin. | UNB-AGT-002, UNB-XIN-SAS-006 |
| XFC-REC-001 | Legal hold | stale verification | Hold matched against stale search index; misses records. | Matching against authoritative index only (REQ-X-038). | Re-run matching. | Counsel. | REQ-X-038, UNB-XIN-REC-001 |
| XFC-REC-002 | Disposition | repeated processing | Disposition worker retries ambiguous delete. | Execution token idempotency. | Verify bytes state; record Failed/Unknown honestly. | Records owner. | REQ-X-038, RR-REQ-X-038-r1 |
| XFC-WFA-001 | Timers | outage | Scheduler down during due window. | Overdue obligations monitor. | Fire once in due order after recovery. | Ops. | TIME-1, UNB-XIN-WFA-001 |
| XFC-OTH-001 | Print/mail | failed payment delivery | Check mailed to stale address; returned. | Returned mail event. | Void + reissue after address verification. | Treasury. | UNB-RCN-GRT-005, UNB-XIN-OTH-004 |
| XFC-OTH-002 | Print/mail | duplicate request | Vendor reprints batch after timeout; same check numbers mailed twice. | Manifest duplicate detection; positive pay pays once. | Void duplicates. | Treasury. | TVS-505, UNB-XIN-OTH-004 |
| XFC-OTH-003 | Embedded checkout | unauthorized instruction | Host routes funds through its account. | Payee-graph check. | Block release. | Counsel. | REQ-X-063, UNB-XIN-OTH-003 |
| XFC-ECO-001 | Vendor cost ingestion | failed integration | Model API usage endpoint down; per-call cost missing | Metered sum vs invoice gap at month end; tag coverage metric drops | Backfill from provider usage export; gap to residual MissingDriverData | Controller if residual > tolerance | REQ-X-046, UNB-XIN-ECO-005, UNB-XIN-ECO-004 |
| XFC-ECO-002 | Vendor cost ingestion | duplicate request | Invoice ingested twice | Duplicate x_SourceInvoiceRef check | Reject duplicate idempotently | None | UNB-XIN-ECO-005 |
| XFC-ECO-003 | Attribution rules | tenant misconfiguration | Driver rule maps hosting cost by grants but sponsor S2 has no grants | Reconciliation NoEligibleCostObject residual | Fix rule version; restate | FP&A | UNB-XIN-ECO-002, UNB-XIN-ECO-004 |
| XFC-ECO-004 | Attribution rules | incorrect accounting entry | Developer wires attribution to journal posting | Ledger-hash property test; permission denial | Remove path; incident | Controller, CISO | UNB-ACC-002, UNB-XIN-ECO-001 |
| XFC-ECO-005 | Reconciliation | data corruption | Projection store rows lost | Identity check fails | Deterministic rebuild from events and GL | Platform ops | UNB-ACC-036, UNB-XIN-ECO-004 |
| XFC-ECO-006 | Reconciliation | changed external information | Vendor retroactive volume discount | Invoice credit line | Restate affected months as new run versions | FP&A | UNB-XIN-ECO-004, UNB-XIN-ECO-005 |
| XFC-ECO-007 | AI metering | incorrect AI recommendation | Budget-exhausted run returns partial summary treated as complete | Status check; Incomplete never maps to Pass | Route to human | Agent owner | AIN-028, UNB-XIN-ECO-009 |
| XFC-ECO-008 | AI recommendations | hallucination | AI recommends removing step claiming 40% saving with fabricated variant | Evidence-link validation; verifier replay | Reject proposal | Learning governance | REQ-X-042, ADV-578, UNB-XIN-ECO-011 |
| XFC-ECO-009 | Cost quotes | customer dispute | Donor disputes ED charge above expected | Charge vs quote comparison | Hold charge; re-affirm or waive | Sponsor ops | UNB-ACC-010, UNB-XIN-ECO-008 |
| XFC-ECO-010 | Capacity forecast | unavailable employee | Specialists on leave during December peak | Forecast eligible hours vs required | Reallocate, seasonal contractors with competence, giving-plan deferral offers | COO | REQ-X-048, UNB-XIN-ECO-017 |
| XFC-ECO-011 | Capacity forecast | other | Forecast built on assumed surge k=3 diverges from actual k=4.5 | Weekly MAPE threshold | Re-forecast with measured weeks | COO | TVA-004, UNB-XIN-ECO-017 |
| XFC-ECO-012 | Improvement loop | conflicting policy | Cost initiative reduces review minutes below sponsor policy evidence requirement | Counter-metric floor (seeded control tests) | Halt and roll back | Compliance | REQ-X-042, UNB-XIN-ECO-016 |
| XFC-ECO-013 | Improvement loop | other | Owner self-certifies saving | Role check on Verified transition | Deny; verifier review | CFO | TVR-ORG-004, UNB-XIN-ECO-015 |
| XFC-ECO-014 | Cost dashboards | compromised credentials | Sponsor admin account used to probe other sponsors' costs | Scope enforcement denial spikes | Lock account | Security | UNB-TEN-007, UNB-XIN-ECO-021 |
| XFC-ECO-015 | Service charges | regulatory change | Counsel advises a service charge to funds is impermissible | Legal update | Deactivate via REQ-X-045; refund charged amounts via approved accounting | Counsel, CFO | REQ-X-045, UNB-XIN-ECO-018 |
| XFC-ECO-016 | Process mining | outage | Event outbox lag causes partial week | Watermark check | Mark week incomplete; no signals emitted | Ops | REQ-X-043, UNB-XIN-ECO-010 |
| XFC-EVA-001 | Outbox dispatch | repeated processing | Dispatcher crashes after provider accepted but before marking sent | Unknown on restart: sent-intent without outcome | Query provider by PaymentIntentId; no blind resend | Treasury after SLA | ADV-081, REQ-X-004, INV-04, UNB-XIN-EVA-007 |
| XFC-EVA-002 | Outbox dispatch | other | Cursor skips an id whose transaction committed late (sequence/commit order mismatch) | Reconciliation of outbox rows never delivered older than N minutes | Claim-and-mark delivery; sweep undelivered rows | Ops | UNB-EVT-002, UNB-XIN-EVA-007 |
| XFC-EVA-003 | Custodian ingestion | missing information | Position file not delivered | Expected-file calendar | Hold dependent automation; request resend | Ops then finance | UNB-ACC-018, UNB-XIN-EVA-004 |
| XFC-EVA-004 | Bank return ingestion | duplicate request | Return file re-delivered | Business key + payload hash | Ack without effect | None | PAY-07, SIM-19, UNB-XIN-EVA-005 |
| XFC-EVA-005 | Custodian ingestion | changed external information | Custodian corrects a trade under same reference | Same key, different hash | Conflict case; reversal+re-post when confirmed | Investment ops | UNB-CUST-007, UNB-XIN-EVA-005 |
| XFC-EVA-006 | Period close | incorrect accounting entry | Posting commits into period being closed | Period lock conflict / serialization failure | Retry as PriorPeriodAdjustment | Controller | UNB-ACC-017, UNB-XIN-EVA-002 |
| XFC-EVA-007 | Reporting | data corruption | Statement figures differ from Schedule D package for same period | Snapshot id mismatch check | Regenerate from sealed snapshot | Controller | TX-21, INV-08, UNB-XIN-EVA-003 |
| XFC-EVA-008 | Grant payment | conflicting policy | Hold published between release and dispatch | Dispatcher final hold-version check | Stop dispatch; Held | Compliance | ADV-070, RR-REQ-X-020-r1, UNB-XIN-EVA-011 |
| XFC-EVA-009 | Recurring grants | regulatory change | Sanctions listing mid-batch | Per-item hold check | Hold remaining items; block transmitted | Sanctions officer | TVCTL-13, CTL-012, UNB-XIN-EVA-011 |
| XFC-EVA-010 | Process managers | outage | Application restart during check aging | Timers persisted; stall detector | Resume from row state | Ops | UNB-RES-002, UNB-XIN-EVA-007 |
| XFC-EVA-011 | Process managers | partial completion | One of three redemption legs rejected | FundingPM state | FundingShortfall; alternate plan approval | Investment ops + sponsor | EXC-04, UNB-RCN-FIN-008, UNB-XIN-EVA-007 |
| XFC-EVA-012 | AI automation | incorrect AI recommendation | Agent proposes clearing hold from injected document | Type/role separation; reviewer | Reject proposal | Compliance | SIM-22, INV-15, RR-UNB-EVT-005-r1 |
| XFC-EVA-013 | AI automation | hallucination | Agent summary cites a payment settlement that is only acknowledged | Summary must cite fact ids; validator checks authority class | Regenerate; flag | Case owner | UNB-GEX-008, RR-UNB-EVT-001-r1 |
| XFC-EVA-014 | Event schema | other | Unknown event version encountered by authoritative consumer | Version check | Halt stream; quarantine | Engineering | UNB-DATA-010, UNB-XIN-EVA-008 |
| XFC-EVA-015 | Communications | changed external information | 'Paid' email after R03 return | Send-time fact version check | Suppress; send correction | None | ADV-088, UNB-XIN-EVA-015 |
| XFC-EVA-016 | Relationship management | unauthorized instruction | CRM note used to grant advisor rights | Static dependency test; command uses identity facts | Reject command | Security | INV-09, UNB-XIN-EVA-014 |
| XFC-EVA-017 | Conflicting instructions | conflicting policy | Co-advisors opposite instructions | Conflict scope check | HeldConflict; sponsor case | Sponsor | ADM-023, UNB-XIN-EVA-012 |
| XFC-EVA-018 | Sponsor transfer | partial completion | Bulk program crashes midway | Program PM state | Resume by keys | Ops | RR-UNB-WF-014-r1, UNB-XIN-EVA-009 |
| XFC-EVA-019 | Tenant routing | tenant misconfiguration | Consumer processes outbox of wrong sponsor DB | Tenant id in envelope vs connection binding | Reject; incident | Security | UNB-TEN-004, ADV-079, UNB-XIN-EVA-006 |
| XFC-EVA-020 | Holds | conflicting policy | Records legal hold mistakenly blocks grants | Hold-type command matrix test | Typed holds | Compliance | UNB-GAP-019, UNB-XIN-EVA-010 |
<!-- END GENERATED: failures -->

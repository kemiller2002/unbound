# Adversarial approval review

Scope: 143 persisted requirement-question proposals, reviewed together for internal consistency, risk, economics and testability. Product-direction approval only, not legal, CPA, bank, sponsor or production approval. Source-level reconciliation and independent landscape verification outstanding. Approved rows retain original answer and validation gate. Revise rows require replacement wording below; hold rows lack sufficient evidence. User authorized approval of sound answers.

Counts: {"approved":111,"revise":29,"hold":3}.

| Original record | Question | Disposition | Review / correction |
|---|---|---|---|
| RR-ADM-002-r1 | Identity prefill | revise | Prefill is not proof of identity; enforce purpose, consent, field provenance and freshness; never silently overwrite verified data. |
| RR-ADM-002-r1 | Material agreement change | approved | Approve product direction; retain original validation gate: Counsel confirms classification. |
| RR-ADM-003-r1 | Board delegation | approved | Approve product direction; retain original validation gate: Counsel and board approval. |
| RR-ADM-023-r1 | Joint mode | revise | Use recorded authority, not marital status; default only when agreement explicitly authorizes any-one; detect conflicting instructions and reserve capacity atomically. |
| RR-OPS-004-r1 | Auditor acceptance | approved | Approve product direction; retain original validation gate: Compare evidence completeness. |
| RR-REQ-X-007-r2 | Renewal approval | approved | Approve product direction; retain original validation gate: Counsel and board approval. |
| RR-REQ-X-007-r2 | Sampling size | revise | Replace universal 30 with risk/error-bound sampling; 30 is exploratory only (zero errors in 30 still allows roughly 10% upper 95% error bound). |
| RR-REQ-X-007-r3 | Cross-sponsor reliance | approved | Approve product direction; retain original validation gate: Counsel; test tenant isolation. |
| RR-REQ-X-032-r1 | Statement delivery | approved | Approve product direction; retain original validation gate: Counsel verifies jurisdiction and terms. |
| RR-REQ-X-048-r3 | Assignment mode | approved | Approve product direction; retain original validation gate: Test absence, overload and conflicts. |
| RR-REQ-X-073-r1 | 3/12 month thresholds | hold | Hold answer pending source context; cannot approve unknown control thresholds. |
| RR-SOC2-024-r1 | Auditor acceptance | approved | Approve product direction; retain original validation gate: Auditor confirms scope. |
| RR-TVR-CON-001-r1 | Card window | approved | Approve product direction; retain original validation gate: Acquirer confirms rules before activation. |
| RR-UNB-ACC-004-r1 | Journal | approved | Approve product direction; retain original validation gate: Controller confirms RCF-FIN-008; test balanced postings and isolation. |
| RR-UNB-ACC-008-r1 | Accounting basis | revise | One authoritative book per entity and accounting basis, which may reside externally; do not require Unbound to own GL. |
| RR-UNB-ACC-014-r2 | Budgeting | approved | Approve product direction; retain original validation gate: Test export and reconciliation. |
| RR-UNB-ACC-017-r1 | Generated evidence | approved | Approve product direction; retain original validation gate: Test completeness and tamper evidence. |
| RR-UNB-ACC-022-r1 | Reporting scope | revise | External ERP is default, not mandatory; support sponsor program reporting and authoritative GL configuration without building generic ERP. |
| RR-UNB-API-STD-003-r1 | Deprecation | revise | 18 months for breaking supported public APIs only; version lifecycle and migration path; emergency exceptions. |
| RR-UNB-CHAR-013-r1 | Precision | approved | Approve product direction; retain original validation gate: Independent validation; no arbitrary confidence threshold. |
| RR-UNB-DATA-004-r1 | Facts | approved | Approve product direction; retain original validation gate: Test provenance and retention. |
| RR-UNB-ECO-003-r1 | Authority holder time | approved | Approve product direction; retain original validation gate: Roster stress test. |
| RR-UNB-EXT-004-r2 | Disposition reuse | revise | Do not conflate sanctions false-positive disposition with charity eligibility; full charity review at most 180 days, event-triggered invalidation and current release screening; counsel sets disposition reuse. |
| RR-UNB-EXT-028-r1 | Detection SLA | revise | Freshness must be explicit by source; use cached verified evidence within approved window, not mandatory live calls on every payment; fail safely. |
| RR-UNB-EXT-028-r1 | Informational class | approved | Approve product direction; retain original validation gate: Counsel confirms class list. |
| RR-UNB-GFUND-012-r1 | Funding source consent | revise | Also require unchanged investment exposure, restrictions, tax/accounting consequences and authorized funding instruction; otherwise obtain approval. |
| RR-UNB-GOV-002-r1 | Auditor acceptance | hold | Hold exact answer pending source review; control register alone does not establish auditor acceptance. |
| RR-UNB-GRANT-024-r1 | Tier 2 | approved | Approve product direction; retain original validation gate: Counsel confirms non-ER cases. |
| RR-UNB-GRANT-030-r1 | Thresholds | approved | Approve product direction; retain original validation gate: Calibrate and obtain delegated approval. |
| RR-UNB-GRANT-030-r1 | Sampling | revise | Use common risk/error-bound sampling policy; independent review and escalation after errors. |
| RR-UNB-GRT-003-r1 | Exception incidence | approved | Approve product direction; retain original validation gate: Measure incidence and handling time. |
| RR-UNB-GRT-003-r1 | Amount-based review | approved | Approve product direction; retain original validation gate: Sponsor and counsel confirm. |
| RR-UNB-RES-003-r1 | RTO | revise | Define RTO and RPO before architecture acceptance, then test; no loss of acknowledged financial facts without explicit approved recovery semantics. |
| RR-UNB-RULE-019-r1 | Emergency changes | approved | Approve product direction; retain original validation gate: Test rollback and audit. |
| RR-UNB-RULE-019-r1 | Observation | revise | Seven days is illustrative; require representative volume, risk coverage and rollback readiness, not elapsed time alone. |
| RR-UNB-TEN-016-r1 | Retention and cost | approved | Approve product direction; retain original validation gate: Counsel and commercial approval. |
| UNB-SMP-ARX-001 | Suspension SLA | revise | Read-only access only if confidentiality and integrity remain trustworthy; otherwise status-only channel. |
| UNB-SMP-ARX-001 | HumanQueue | approved | Approve product direction; retain original validation gate: Test routing and no bypass. |
| UNB-SMP-CFG-001 | Question wording | approved | Approve product direction; retain original validation gate: Sponsor usability research; avoid leading questions. |
| UNB-SMP-CFG-002 | Liability | approved | Approve product direction; retain original validation gate: Counsel approval. |
| UNB-SMP-CFG-002 | Pack adoption | approved | Approve product direction; retain original validation gate: Counsel; test impact and rollback. |
| UNB-SMP-CLR-001 | Outsourced roles | approved | Approve product direction; retain original validation gate: Counsel and board approval. |
| UNB-SMP-CLR-001 | Two-deep coverage | approved | Approve product direction; retain original validation gate: Simulate simultaneous absences. |
| UNB-SMP-CLR-002 | Basis mapping | approved | Approve product direction; retain original validation gate: Validate graph completeness. |
| UNB-SMP-CXP-001 | Convention expiry | approved | Approve product direction; retain original validation gate: Test overdue review escalation. |
| UNB-SMP-DSH-001 | Harm/deadline weights | revise | Hard legal deadlines and imminent harm are simultaneous constraints; escalate collisions, prevent starvation. |
| UNB-SMP-ECN-001 | 1.5 ceiling | approved | Approve product direction; retain original validation gate: CFO reasonableness and sensitivity analysis. |
| UNB-SMP-ECN-001 | Minimum grant sizes | approved | Approve product direction; retain original validation gate: Model donor harm and alternatives. |
| UNB-SMP-ECN-002 | 90% floor | revise | Do not replace an internal minimum service floor with contract tiers; define metric and mandatory minimum before approving number. |
| UNB-SMP-ECN-003 | Employer subsidy | approved | Approve product direction; retain original validation gate: Contract and funding verification. |
| UNB-SMP-ECN-004 | Price/margin | approved | Approve product direction; retain original validation gate: CFO approval. |
| UNB-SMP-ECN-004 | Scale | approved | Approve product direction; retain original validation gate: Sensitivity analysis. |
| UNB-SMP-ORG-001 | System failures | approved | Approve product direction; retain original validation gate: Stress tests. |
| UNB-SMP-ORG-001 | 25 threshold | approved | Approve product direction; retain original validation gate: Pilot measurement. |
| UNB-SMP-ORG-002 | Absence coverage | revise | Derive double absence from criticality and peak workload, not calendar alone; year-end is stress scenario. |
| UNB-SMP-ORG-002 | Deterministic releaser | approved | Approve product direction; retain original validation gate: Auditor and counsel. |
| UNB-SMP-ORG-003 | Team transfers | approved | Approve product direction; retain original validation gate: Access transition test. |
| UNB-SMP-ORG-003 | Appointment | approved | Approve product direction; retain original validation gate: Counsel and board. |
| UNB-SMP-ORG-003 | Minimum time share | approved | Approve product direction; retain original validation gate: Compare administrator and direct models. |
| UNB-XIN-ACC-001 | Pass-through versus variance power | approved | Approve product direction; retain original validation gate: CPA reviews agreements; no classification by marketing label. |
| UNB-XIN-ACC-002 | Promise to give | approved | Approve product direction; retain original validation gate: CPA determines recognition trigger; do not assume internal approval alone suffices. |
| UNB-XIN-ACC-006 | Line 4 valuation | approved | Approve product direction; retain original validation gate: Verify form year and CPA interpretation. |
| UNB-XIN-ACC-008 | UBIT recognition | approved | Approve product direction; retain original validation gate: CPA confirms tax and accounting treatment. |
| UNB-XIN-ADV-001 | Account cost budgets | approved | Approve product direction; retain original validation gate: Counsel reviews fee fairness and service parity. |
| UNB-XIN-ADV-001 | Convention versus policy | approved | Approve product direction; retain original validation gate: Counsel validates classifications; no automatic removal. |
| UNB-XIN-ADV-002 | Shadow attribution demand | revise | Support comparison/import of external attribution, not optional duplicate ledger by default; demand gate for shadow book. |
| UNB-XIN-ADV-002 | De minimis and staleness | approved | Approve product direction; retain original validation gate: Controller calibrates from actual feed data. |
| UNB-XIN-BNK-001 | ACH originator | approved | Approve product direction; retain original validation gate: Bank and counsel confirm contractual roles. |
| UNB-XIN-BNK-002 | ODFI monitoring | approved | Approve product direction; retain original validation gate: Obtain ODFI contract and test alerts. |
| UNB-XIN-BNK-003 | Bank exports | revise | Prior-day feed alone insufficient for spendable capacity; require reservation and freshness controls appropriate to rail. |
| UNB-XIN-BNK-007 | Voided check ownership | approved | Approve product direction; retain original validation gate: Counsel determines unclaimed property and charity rights. |
| UNB-XIN-CLM-001 | Reconsideration window | revise | Choose 60-day product default, 30-day response target, configurable with mandatory exceptions; supersede competing 90-day default only after source reconciliation. |
| UNB-XIN-CLM-004 | Independent investigators | approved | Approve product direction; retain original validation gate: Verify independence and availability. |
| UNB-XIN-CLM-005 | Sampling | revise | Same risk/error-bound policy; avoid separate inconsistent sample rules. |
| UNB-XIN-CLM-006 | Legal basis and duration | approved | Approve product direction; retain original validation gate: Counsel confirms hold-specific maximums. |
| UNB-XIN-CRM-002 | Charity self-recommendation | approved | Approve product direction; retain original validation gate: Do not presume permissibility or prohibition. |
| UNB-XIN-CRM-003 | Commercial email | approved | Approve product direction; retain original validation gate: Verify sender and CAN-SPAM applicability. |
| UNB-XIN-CRM-006 | Referral payments | approved | Approve product direction; retain original validation gate: Counsel researches state solicitation laws before activation. |
| UNB-XIN-ECO-001 | Cost-plus chargeback | approved | Approve product direction; retain original validation gate: Validate reconciliation and avoid double billing. |
| UNB-XIN-ECO-003 | Default ranges | approved | Approve product direction; retain original validation gate: Sensitivity analysis and CFO approval. |
| UNB-XIN-ECO-004 | Tolerance | approved | Approve product direction; retain original validation gate: Calibrate from actual invoices; no invented universal threshold. |
| UNB-XIN-ECO-005 | FOCUS version | hold | Hold exact version pending specification verification; approve adapter approach only. |
| UNB-XIN-ECO-006 | Practical capacity | approved | Approve product direction; retain original validation gate: Pilot time study; avoid treating utilization as productivity. |
| UNB-XIN-ECO-008 | Donor quote | revise | Any increase above accepted maximum requires consent or sponsor absorbs difference; no fee for required accessibility accommodation. |
| UNB-XIN-ECO-015 | Capacity buffer | approved | Approve product direction; retain original validation gate: Stress-test peak and absence scenarios. |
| UNB-XIN-ECO-017 | December volumes | approved | Approve product direction; retain original validation gate: If unavailable use labeled synthetic scenarios, not claimed observations. |
| UNB-XIN-ECO-018 | Ceiling | approved | Approve product direction; retain original validation gate: CFO and counsel review fairness. |
| UNB-XIN-ECO-021 | Privacy thresholds | revise | Five/60% are experimental minimums, not release authorization; require differencing, longitudinal and collusion tests. |
| UNB-XIN-ERP-001 | Billing basis | approved | Approve product direction; retain original validation gate: Obtain sponsor contracts. |
| UNB-XIN-ERP-001 | Closing minimum fee | revise | Do not prorate a minimum fee by default without contract and economics; propose no extra closing minimum unless disclosed and authorized. |
| UNB-XIN-ERP-002 | Membership deductibility | approved | Approve product direction; retain original validation gate: Counsel determines treatment. |
| UNB-XIN-ERP-003 | Related parties | approved | Approve product direction; retain original validation gate: Verify actual relationships. |
| UNB-XIN-ERP-004 | Vendor extracts | approved | Approve product direction; retain original validation gate: Confirm sponsor availability. |
| UNB-XIN-EVA-001 | Journal component | approved | Approve product direction; retain original validation gate: Test isolation and balancing. |
| UNB-XIN-EVA-002 | Controller threshold | approved | Approve product direction; retain original validation gate: Controller approves thresholds. |
| UNB-XIN-EVA-004 | Source tolerances | approved | Approve product direction; retain original validation gate: Calibrate on feeds. |
| UNB-XIN-EVA-005 | Return matching | approved | Approve product direction; retain original validation gate: Verify partner specifications. |
| UNB-XIN-EVA-007 | Workflow SLA | approved | Approve product direction; retain original validation gate: Measure pilot latency. |
| UNB-XIN-EVA-009 | Bulk transfer legal form | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-EVA-010 | Dispute fee accrual | approved | Approve product direction; retain original validation gate: CFO and counsel. |
| UNB-XIN-EVA-012 | Instruction authority | revise | Same recorded authority matrix as joint mode; no implied any-one authority. |
| UNB-XIN-EVA-013 | Benchmark instance | revise | Define workload, SLO, RPO/RTO and cost envelope before selecting instance; include failover and contention. |
| UNB-XIN-IAM-002 | Delegation | approved | Approve product direction; retain original validation gate: Test revocation and tenant isolation. |
| UNB-XIN-IAM-003 | Workforce AAL3 | approved | Approve product direction; retain original validation gate: Security validates NIST mapping and availability. |
| UNB-XIN-INV-001 | Advisor fee feeds | approved | Approve product direction; retain original validation gate: Verify custodian feeds. |
| UNB-XIN-INV-002 | Multi-sponsor pools | approved | Approve product direction; retain original validation gate: Securities counsel. |
| UNB-XIN-NPG-001 | Third-party written advice | approved | Approve product direction; retain original validation gate: Counsel approves. |
| UNB-XIN-NPG-001 | Proposed rule currency | approved | Approve product direction; retain original validation gate: Counsel monitors rulemaking. |
| UNB-XIN-NPG-002 | Intermediary earmark | approved | Approve product direction; retain original validation gate: Counsel determines permissibility. |
| UNB-XIN-NPG-003 | Agreement evidence | approved | Approve product direction; retain original validation gate: Counsel defines exceptions. |
| UNB-XIN-NPG-004 | Schedule F categories | approved | Approve product direction; retain original validation gate: Verify current IRS instructions. |
| UNB-XIN-NPG-005 | Merger successor | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-NPG-006 | Indictment holds | revise | No automatic hold from allegation; temporary hold only for documented credible risk under approved policy, time-bound independent review; reconcile with adjudicated-only proposal. |
| UNB-XIN-NPG-007 | Geographic class benefit | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-NPG-008 | Taxable distribution correction | approved | Approve product direction; retain original validation gate: Counsel verifies section 4966. |
| UNB-XIN-NPG-009 | Domestic concern | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-NPG-010 | General licenses | approved | Approve product direction; retain original validation gate: Sanctions counsel. |
| UNB-XIN-NPG-011 | B2B check exemption | approved | Approve product direction; retain original validation gate: Counsel verifies states. |
| UNB-XIN-OTH-002 | Notice period | approved | Approve product direction; retain original validation gate: Sponsor and counsel. |
| UNB-XIN-OTH-003 | Partner merchant account | approved | Approve product direction; retain original validation gate: Verify custody and funds flow. |
| UNB-XIN-PAY-001 | ACH retention | approved | Approve product direction; retain original validation gate: Confirm current Nacha rules. |
| UNB-XIN-PAY-003 | Card dispute windows | approved | Approve product direction; retain original validation gate: Acquirer confirmation. |
| UNB-XIN-PAY-005 | ACH reinitiation | approved | Approve product direction; retain original validation gate: Confirm Nacha rules. |
| UNB-XIN-PAY-006 | Donor-covered fee | approved | Approve product direction; retain original validation gate: Counsel and CPA. |
| UNB-XIN-PAY-007 | Custodian memo | approved | Approve product direction; retain original validation gate: Test actual custodian feeds. |
| UNB-XIN-REG-001 | Money transmission | approved | Approve product direction; retain original validation gate: State regulatory counsel. |
| UNB-XIN-REG-002 | Investment adviser | approved | Approve product direction; retain original validation gate: Securities counsel. |
| UNB-XIN-REG-003 | TCPA exemption | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-SAS-001 | Nonremovable capabilities | approved | Approve product direction; retain original validation gate: Product and counsel classify catalog. |
| UNB-XIN-SAS-003 | Peak freeze | approved | Approve product direction; retain original validation gate: Validate sponsor seasonality. |
| UNB-XIN-SAS-005 | Global hold watermark | revise | Freshness alone does not close race; atomic authorization/fencing at irreversible dispatch, idempotency and reconciliation of in-flight payments. |
| UNB-XIN-SAS-007 | Batch transfer approval | approved | Approve product direction; retain original validation gate: Counsel and sponsor. |
| UNB-XIN-SAS-007 | Exit fees and timing | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-SAS-008 | Tier targets | approved | Approve product direction; retain original validation gate: Pilot and commercial approval. |
| UNB-XIN-SAS-009 | Allocation keys | revise | Unify with ECO-001 and XID-ECO-004; separate causal cost attribution from contract billing and prevent double allocation. |
| UNB-XIN-SVC-001 | Internal contacts | approved | Approve product direction; retain original validation gate: Test confidentiality. |
| UNB-XIN-SVC-002 | Case profiles | approved | Approve product direction; retain original validation gate: Test invalid configurations. |
| UNB-XIN-SVC-003 | Continuation consent | approved | Approve product direction; retain original validation gate: Counsel by jurisdiction. |
| UNB-XIN-SVC-003 | Third-party transfer | approved | Approve product direction; retain original validation gate: Counsel. |
| UNB-XIN-SVC-004 | Assurance naming | approved | Approve product direction; retain original validation gate: Security validates. |
| UNB-XIN-SVC-005 | Service clocks | revise | Track end-to-end elapsed time alongside internal SLA; pause only with documented reason, no pause of statutory deadline. |
| UNB-XIN-SVC-008 | December interval data | approved | Approve product direction; retain original validation gate: Validate coverage. |
| UNB-XIN-TRS-004 | POA advisory authority | approved | Approve product direction; retain original validation gate: Counsel and sponsor. |

## Cross-cutting acceptance scenarios

- Concurrent co-advisor recommendations cannot overspend or bypass recorded authority.
- Charity eligibility expires at 180 days or earlier invalidation; current sanctions evidence and payment fencing prevent stale approval execution.
- Bank return, void, hold and reissue preserve obligations and balanced journals without duplicate payment.
- Sponsor exit exports complete authorized records without leaking another tenant's data.
- December surge and simultaneous absences preserve independent controls and mandatory deadlines.
- Cost ceiling never denies mandatory service; quotes cannot exceed accepted maximum without consent.
- Policy changes retain historical versions, representative replay and rollback evidence.

Next: apply revisions to original source records, preserve professional gates, reconcile generated CSV and regenerate register. No implementation or changes to daf-landscape.

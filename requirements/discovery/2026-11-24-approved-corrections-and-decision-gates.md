# Reviewed corrections and decision gates

Product-owner authorization to proceed with review recommendations. These corrections supersede conflicting discovery proposals at product-design level, not original normative source records until reconciled. Professional gates remain open.

## 29 corrections
| Record | Question | Disposition | Replacement direction |
|---|---|---|---|
| RR-ADM-002-r1 | Identity prefill | revise | Prefill is not proof of identity; enforce purpose, consent, field provenance and freshness; never silently overwrite verified data. |
| RR-ADM-023-r1 | Joint mode | revise | Use recorded authority, not marital status; default only when agreement explicitly authorizes any-one; detect conflicting instructions and reserve capacity atomically. |
| RR-REQ-X-007-r2 | Sampling size | revise | Replace universal 30 with risk/error-bound sampling; 30 is exploratory only (zero errors in 30 still allows roughly 10% upper 95% error bound). |
| RR-UNB-ACC-008-r1 | Accounting basis | revise | One authoritative book per entity and accounting basis, which may reside externally; do not require Unbound to own GL. |
| RR-UNB-ACC-022-r1 | Reporting scope | revise | External ERP is default, not mandatory; support sponsor program reporting and authoritative GL configuration without building generic ERP. |
| RR-UNB-API-STD-003-r1 | Deprecation | revise | 18 months for breaking supported public APIs only; version lifecycle and migration path; emergency exceptions. |
| RR-UNB-EXT-004-r2 | Disposition reuse | revise | Do not conflate sanctions false-positive disposition with charity eligibility; full charity review at most 180 days, event-triggered invalidation and current release screening; counsel sets disposition reuse. |
| RR-UNB-EXT-028-r1 | Detection SLA | revise | Freshness must be explicit by source; use cached verified evidence within approved window, not mandatory live calls on every payment; fail safely. |
| RR-UNB-GFUND-012-r1 | Funding source consent | revise | Also require unchanged investment exposure, restrictions, tax/accounting consequences and authorized funding instruction; otherwise obtain approval. |
| RR-UNB-GRANT-030-r1 | Sampling | revise | Use common risk/error-bound sampling policy; independent review and escalation after errors. |
| RR-UNB-RES-003-r1 | RTO | revise | Define RTO and RPO before architecture acceptance, then test; no loss of acknowledged financial facts without explicit approved recovery semantics. |
| RR-UNB-RULE-019-r1 | Observation | revise | Seven days is illustrative; require representative volume, risk coverage and rollback readiness, not elapsed time alone. |
| UNB-SMP-ARX-001 | Suspension SLA | revise | Read-only access only if confidentiality and integrity remain trustworthy; otherwise status-only channel. |
| UNB-SMP-DSH-001 | Harm/deadline weights | revise | Hard legal deadlines and imminent harm are simultaneous constraints; escalate collisions, prevent starvation. |
| UNB-SMP-ECN-002 | 90% floor | revise | Do not replace an internal minimum service floor with contract tiers; define metric and mandatory minimum before approving number. |
| UNB-SMP-ORG-002 | Absence coverage | revise | Derive double absence from criticality and peak workload, not calendar alone; year-end is stress scenario. |
| UNB-XIN-ADV-002 | Shadow attribution demand | revise | Support comparison/import of external attribution, not optional duplicate ledger by default; demand gate for shadow book. |
| UNB-XIN-BNK-003 | Bank exports | revise | Prior-day feed alone insufficient for spendable capacity; require reservation and freshness controls appropriate to rail. |
| UNB-XIN-CLM-001 | Reconsideration window | revise | Choose 60-day product default, 30-day response target, configurable with mandatory exceptions; supersede competing 90-day default only after source reconciliation. |
| UNB-XIN-CLM-005 | Sampling | revise | Same risk/error-bound policy; avoid separate inconsistent sample rules. |
| UNB-XIN-ECO-008 | Donor quote | revise | Any increase above accepted maximum requires consent or sponsor absorbs difference; no fee for required accessibility accommodation. |
| UNB-XIN-ECO-021 | Privacy thresholds | revise | Five/60% are experimental minimums, not release authorization; require differencing, longitudinal and collusion tests. |
| UNB-XIN-ERP-001 | Closing minimum fee | revise | Do not prorate a minimum fee by default without contract and economics; propose no extra closing minimum unless disclosed and authorized. |
| UNB-XIN-EVA-012 | Instruction authority | revise | Same recorded authority matrix as joint mode; no implied any-one authority. |
| UNB-XIN-EVA-013 | Benchmark instance | revise | Define workload, SLO, RPO/RTO and cost envelope before selecting instance; include failover and contention. |
| UNB-XIN-NPG-006 | Indictment holds | revise | No automatic hold from allegation; temporary hold only for documented credible risk under approved policy, time-bound independent review; reconcile with adjudicated-only proposal. |
| UNB-XIN-SAS-005 | Global hold watermark | revise | Freshness alone does not close race; atomic authorization/fencing at irreversible dispatch, idempotency and reconciliation of in-flight payments. |
| UNB-XIN-SAS-009 | Allocation keys | revise | Unify with ECO-001 and XID-ECO-004; separate causal cost attribution from contract billing and prevent double allocation. |
| UNB-XIN-SVC-005 | Service clocks | revise | Track end-to-end elapsed time alongside internal SLA; pause only with documented reason, no pause of statutory deadline. |

## Cross-register decisions

| Decision IDs | Product direction | Remaining gate |
|---|---|---|
| SPD-ENT-002, XID-CLM-001 | 60-day filing, 30-day response default; preserve original and reconsideration; independent reviewer; mandatory exceptions. | Sponsor counsel |
| XID-ADV-004, XID-EVA-006, XID-TRS-002 | Recorded authority controls; absent explicit any-one authority require all authorized parties or resolve ambiguity; sensitive changes require stronger approval. | Sponsor counsel |
| SPD-FIN-004 | No reconfirmation only if all material donor and financial consequences unchanged and existing authority permits. | Counsel |
| SPD-GRT-003, SPD-ADV-002, RCD-006 | 180-day charity deep review, earlier invalidation, fresh release screening; false-positive dispositions distinct and sponsor-scoped by default. | Counsel sets reuse age |
| XID-CLM-004 | Risk/error-bound sample sizing, not universal 30. | Compliance |
| XID-NPG-004 | No automatic allegation hold; approved credible-risk temporary holds with review deadlines, escalation and independent review; distinguish mandatory holds. | Board and counsel |
| SPD-FIN-002, SPD-FIN-007, XID-INV-007 | Authoritative function map by sponsor; external GL supported, no duplicate authoritative books; program reporting and reconciliation required. | Controller |
| SPD-ORG-001 | Do not count deterministic release as independent person without auditor acceptance. | Auditor and board |
| SPD-ORG-003 | Risk-based roster coverage; double absence stress scenario. | Board |
| SPD-DSH-002, XID-ECO-006 | Experimental privacy thresholds only; test differencing and reidentification. | Privacy owner |
| XID-ECO-007 | Maximum accepted fee; any overrun requires consent or sponsor absorption. | Counsel |
| XID-ECO-004, XID-SAS-003 | Common causal attribution; distinguish showback, billing and internal metering. | CFO |
| XID-EVA-001, SPD-ARX-002 | PostgreSQL hybrid candidate; set recovery objectives before acceptance; benchmark before infrastructure expansion. | Architecture |
| SPD-CFG-001 | Affirmative adoption for material changes; emergency mandatory controls separately authorized. | Counsel |
| SPD-PLT-001, SPD-ADV-003, XID-INV-008 | Assurance scope and timing depend on actual auditor and contractual reliance, not sponsor count alone. | Auditor and counterparties |

## Register durability blocker

tools/reqaudit/render_questions.py currently generates blank answer columns and assigns requirement question IDs by ordinal. Do not remove/reorder open_questions entries until durable mapping is established. Proposed change: source-linked resolution records keyed by original record, exact question text and stable ID; validate uniqueness and stale references; generator joins resolution records and preserves professional gates. Test regeneration twice, orphan detection and question edits. Do not hand-edit generated Markdown.

## Outstanding three holds

RR-REQ-X-073-r1: retrieve exact control context and verify threshold rationale.
RR-UNB-GOV-002-r1: retrieve underlying auditor acceptance criterion; control catalog consolidation is not assurance.
UNB-XIN-ECO-005 / XID-ECO-009: verify official FOCUS specification and provider compatibility before version pin.

No implementation, no source-record mutation, no register regeneration, no professional confirmation claimed. No changes to daf-landscape.

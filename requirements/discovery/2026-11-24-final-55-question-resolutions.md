# Remaining 55 requirement question resolutions

Status: proposed, not approved. Source: [open questions register](../OPEN-QUESTIONS.md), section 3. Preserve original IDs; repeated IDs represent distinct questions. Legal and external evidence not independently verified. No implementation or activation authorized.

| Original record | Question | Proposed answer | Challenge / validation gate |
|---|---|---|---|
| UNB-XIN-ERP-001 | Billing basis | Support average daily and period-end balances under versioned contracts; reconcile calculations. | Obtain sponsor contracts. |
| UNB-XIN-ERP-001 | Closing minimum fee | Prorate by default proposal; disclose and configure contractual exceptions. | CFO and counsel approve. |
| UNB-XIN-ERP-002 | Membership deductibility | Separate fee and contribution receipts; no automatic deduction representation. | Counsel determines treatment. |
| UNB-XIN-ERP-003 | Related parties | Record ownership, control and conflicts by effective date. | Verify actual relationships. |
| UNB-XIN-ERP-004 | Vendor extracts | Support scoped vendor master import with provenance and validation. | Confirm sponsor availability. |
| UNB-XIN-EVA-001 | Journal component | Reuse journal engine with separate entity books and permissions. | Test isolation and balancing. |
| UNB-XIN-EVA-002 | Controller threshold | Risk-based approval matrix including cumulative amounts and unusual adjustments. | Controller approves thresholds. |
| UNB-XIN-EVA-004 | Source tolerances | Absolute and relative tolerances by source, currency and asset; no silent writeoffs. | Calibrate on feeds. |
| UNB-XIN-EVA-005 | Return matching | Bank-specific mapping of original trace, amount, account reference and return identifiers; ambiguous matches queue. | Verify partner specifications. |
| UNB-XIN-EVA-007 | Workflow SLA | Version targets by step, risk and dependency; distinguish external wait from internal delay. | Measure pilot latency. |
| UNB-XIN-EVA-009 | Bulk transfer legal form | Represent sponsor authorization separately from donor recommendations; block execution pending approved legal form. | Counsel. |
| UNB-XIN-EVA-010 | Dispute fee accrual | Separate accrual, collection and waiver; apply approved contract policy during freeze. | CFO and counsel. |
| UNB-XIN-EVA-012 | Instruction authority | One matrix: any authorized advisor for routine grants, stronger joint approval for sensitive instructions, configurable within legal authority. | Resolve XID-ADV-004 and XID-EVA-006. |
| UNB-XIN-EVA-013 | Benchmark instance | Use smallest realistic managed PostgreSQL production candidate and larger comparison; record specs and workload. | Measure peak load and recovery. |
| UNB-XIN-IAM-002 | Delegation | Default deny; scoped time-limited read diagnostics only after sponsor approval; writes per case. | Test revocation and tenant isolation. |
| UNB-XIN-IAM-003 | Workforce AAL3 | Require phishing-resistant workforce authentication; reserve AAL3 for justified high-risk roles with compliant authenticators. | Security validates NIST mapping and availability. |
| UNB-XIN-INV-001 | Advisor fee feeds | Support distinct fee transactions and reconciliation exceptions when unavailable. | Verify custodian feeds. |
| UNB-XIN-INV-002 | Multi-sponsor pools | Do not activate pooled structure relying on unconfirmed exemption. | Securities counsel. |
| UNB-XIN-NPG-001 | Third-party written advice | Store provenance, scope, currency and reliance rights; no automatic reliance. | Counsel approves. |
| UNB-XIN-NPG-001 | Proposed rule currency | Track proposed versus effective rules separately; update only after verified legal change. | Counsel monitors rulemaking. |
| UNB-XIN-NPG-002 | Intermediary earmark | Capture intermediary discretion and donor suggestion separately; route ambiguous restrictions. | Counsel determines permissibility. |
| UNB-XIN-NPG-003 | Agreement evidence | Require agreement or approved equivalent evidence by arrangement type. | Counsel defines exceptions. |
| UNB-XIN-NPG-004 | Schedule F categories | Version reporting mappings by tax year. | Verify current IRS instructions. |
| UNB-XIN-NPG-005 | Merger successor | Verify legal succession and recipient identity; reconfirm material changes unless approved policy permits substitution. | Counsel. |
| UNB-XIN-NPG-006 | Indictment holds | Allow narrowly scoped temporary risk holds with documented evidence, review and appeal; allegation is not finding. | Sponsor board and counsel approve. |
| UNB-XIN-NPG-007 | Geographic class benefit | Evaluate incidental versus prohibited benefit and related parties; no automatic conclusion. | Counsel. |
| UNB-XIN-NPG-008 | Taxable distribution correction | Support remediation records, recovery and tax reporting without implying statutory correction relief. | Counsel verifies section 4966. |
| UNB-XIN-NPG-009 | Domestic concern | Record jurisdiction and applicable anti-bribery controls; do not assume exemption for nonprofit. | Counsel. |
| UNB-XIN-NPG-010 | General licenses | Version approved license scope, conditions and expiry; block unsupported reliance. | Sanctions counsel. |
| UNB-XIN-NPG-011 | B2B check exemption | Maintain state-specific unclaimed property treatment; no blanket exemption. | Counsel verifies states. |
| UNB-XIN-OTH-002 | Notice period | Propose 30 days for ordinary material changes, subject to contract and law; emergency exception documented. | Sponsor and counsel. |
| UNB-XIN-OTH-003 | Partner merchant account | Default direct sponsor or approved processor settlement; partner pass-through disabled until legal and bank approval. | Verify custody and funds flow. |
| UNB-XIN-PAY-001 | ACH retention | Configurable rail-specific retention; no deletion before verified minimum and holds expire. | Confirm current Nacha rules. |
| UNB-XIN-PAY-003 | Card dispute windows | Version by network, reason and acquirer; preserve evidence and deadlines. | Acquirer confirmation. |
| UNB-XIN-PAY-005 | ACH reinitiation | Enforce verified reason-specific retry rules and authorization; no hardcoded unverified counts. | Confirm Nacha rules. |
| UNB-XIN-PAY-006 | Donor-covered fee | Separate gross contribution, fee and net settlement; receipt amount follows approved tax treatment. | Counsel and CPA. |
| UNB-XIN-PAY-007 | Custodian memo | Preserve raw feed and normalized identifiers; unresolved delivery matches queue. | Test actual custodian feeds. |
| UNB-XIN-REG-001 | Money transmission | Map funds flow, control and jurisdictions; no activation of unapproved models. | State regulatory counsel. |
| UNB-XIN-REG-002 | Investment adviser | Separate sponsor-selected execution from discretionary advice; disable unapproved advisory activities. | Securities counsel. |
| UNB-XIN-REG-003 | TCPA exemption | Capture consent and sender/principal classification; conservative controls until exemption confirmed. | Counsel. |
| UNB-XIN-SAS-001 | Nonremovable capabilities | Financial integrity, tenant isolation, audit, mandatory compliance and accessibility cannot be disabled; optional features configurable. | Product and counsel classify catalog. |
| UNB-XIN-SAS-003 | Peak freeze | Propose Dec 15-Jan 5 for disruptive planned changes; emergency security and legal exceptions. | Validate sponsor seasonality. |
| UNB-XIN-SAS-005 | Global hold watermark | Reference immutable hold publication version and check freshness before protected execution; fail closed on unknown state. | Test races and outage. |
| UNB-XIN-SAS-007 | Batch transfer approval | Support deterministic batch recommendation with per-fund eligibility and recorded delegation; no execution absent authority. | Counsel and sponsor. |
| UNB-XIN-SAS-007 | Exit fees and timing | Disclose contractual fees and deadlines; never obstruct mandatory portability or lawful transfer. | Counsel. |
| UNB-XIN-SAS-008 | Tier targets | Define measurable SLOs by capability and tier; contractual commitments after benchmark. | Pilot and commercial approval. |
| UNB-XIN-SAS-009 | Allocation keys | Use causal drivers such as events, storage and compute; AUM only when justified; reconcile shared costs. | CFO validates fairness. |
| UNB-XIN-SVC-001 | Internal contacts | Record staff interactions with charities and custodians as contacts with audience and access classification. | Test confidentiality. |
| UNB-XIN-SVC-002 | Case profiles | Code-defined F# discriminated unions for invariants; sponsor-configurable routing, deadlines and parameters. | Test invalid configurations. |
| UNB-XIN-SVC-003 | Continuation consent | Do not assume continued participation constitutes consent; support explicit consent and no-recording alternative. | Counsel by jurisdiction. |
| UNB-XIN-SVC-003 | Third-party transfer | Re-disclose recording and data sharing when required; preserve consent scope and transfer evidence. | Counsel. |
| UNB-XIN-SVC-004 | Assurance naming | Separate contact assurance from authentication AAL; map only verified equivalences. | Security validates. |
| UNB-XIN-SVC-005 | Service clocks | Configure channel and case clocks with business calendars, pause reasons and escalation; accessibility exceptions. | Pilot targets. |
| UNB-XIN-SVC-008 | December interval data | Obtain anonymized interval arrivals, handle times and abandonment; synthetic scenarios explicitly labeled. | Validate coverage. |
| UNB-XIN-TRS-004 | POA advisory authority | Record instrument, scope, expiry and sponsor acceptance; no automatic authority from generic POA. | Counsel and sponsor. |

## Cross-cutting review

Reconcile authority matrices, fee policies, hold semantics, accounting boundaries and external rail rules with all prior proposals. Verify source records and landscape evidence, define acceptance tests, record counterarguments and decision owners. Regenerate the register from source after approval; do not edit generated Markdown manually. No landscape changes.

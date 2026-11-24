# Accounting, banking, claims, CRM and economics question resolutions

Status: proposed. Source: [open questions register](../OPEN-QUESTIONS.md), section 3. Original record IDs preserved. Each answer requires source-level reconciliation; legal, accounting and bank confirmations are not represented as obtained. No implementation or landscape changes.

| Original record | Question | Proposed answer | Challenge / validation gate |
|---|---|---|---|
| UNB-XIN-ACC-001 | Pass-through versus variance power | Classify each program by actual contractual control and variance power; maintain distinct accounting treatments. | CPA reviews agreements; no classification by marketing label. |
| UNB-XIN-ACC-002 | Promise to give | Record approval, communication, conditions and enforceability separately; recognize liability under approved accounting policy. | CPA determines recognition trigger; do not assume internal approval alone suffices. |
| UNB-XIN-ACC-006 | Line 4 valuation | Retain gross fair value, liabilities and net assets separately; map to applicable form instructions. | Verify form year and CPA interpretation. |
| UNB-XIN-ACC-008 | UBIT recognition | Support accrual estimates, adjustments and settlement; sponsor controller sets recognition policy. | CPA confirms tax and accounting treatment. |
| UNB-XIN-ADV-001 | Account cost budgets | Use budgets as diagnostic signals, never as automatic denial of mandatory service or fiduciary obligations. | Counsel reviews fee fairness and service parity. |
| UNB-XIN-ADV-001 | Convention versus policy | Inventory each obligation with legal, contractual, policy or convention basis and review date. | Counsel validates classifications; no automatic removal. |
| UNB-XIN-ADV-002 | Shadow attribution demand | Support reconciliation interfaces and optional shadow attribution; defer full duplicate ledger until demonstrated demand. | Interview institutional sponsors; avoid redundant books. |
| UNB-XIN-ADV-002 | De minimis and staleness | Configure absolute and relative tolerances by asset and source; stale or unexplained balances cannot authorize unsafe spending. | Controller calibrates from actual feed data. |
| UNB-XIN-BNK-001 | ACH originator | Record actual originator, ODFI, processor and authorization responsibilities per program. | Bank and counsel confirm contractual roles. |
| UNB-XIN-BNK-002 | ODFI monitoring | Version bank-specific monitoring overlays above mandatory baseline; block activation until documented. | Obtain ODFI contract and test alerts. |
| UNB-XIN-BNK-003 | Bank exports | Support normalized API and file adapters with completeness checks; require prior-day coverage at minimum as proposed target. | Validate actual bank exports and latency. |
| UNB-XIN-BNK-007 | Voided check ownership | Separate instrument voiding from extinguishment of underlying obligation; retain payable or hold until disposition authorized. | Counsel determines unclaimed property and charity rights. |
| UNB-XIN-CLM-001 | Reconsideration window | Propose 60-day filing and 30-day response target, configurable subject to law and sponsor policy; reconcile 90-day alternative. | Counsel and sponsor approve; test versioned deadlines. |
| UNB-XIN-CLM-004 | Independent investigators | Support contracted investigators and board-appointed reviewers with conflict screening and scoped access. | Verify independence and availability. |
| UNB-XIN-CLM-005 | Sampling | Risk-stratified provisional 30 per populated stratum monthly or all below that; increase for errors and changes. | Validate statistical power and cost; not a regulatory minimum. |
| UNB-XIN-CLM-006 | Legal basis and duration | Require documented authority, reason, owner, review deadline and escalation; no automatic release of legally required holds. | Counsel confirms hold-specific maximums. |
| UNB-XIN-CRM-002 | Charity self-recommendation | Support identification of organization-advised funds and related recipients; route unresolved self-benefit cases to counsel-approved policy. | Do not presume permissibility or prohibition. |
| UNB-XIN-CRM-003 | Commercial email | Classify promotional, transactional and legally required messages; apply conservative commercial controls pending counsel. | Verify sender and CAN-SPAM applicability. |
| UNB-XIN-CRM-006 | Referral payments | Maintain jurisdiction-specific referral compensation rules; disable unapproved arrangements. | Counsel researches state solicitation laws before activation. |
| UNB-XIN-ECO-001 | Cost-plus chargeback | Implement non-posting cost attribution and contract-configured billing interface; activate chargeback only for executed contracts. | Validate reconciliation and avoid double billing. |
| UNB-XIN-ECO-003 | Default ranges | Use explicitly labeled scenario ranges until pilot observations replace them. | Sensitivity analysis and CFO approval. |
| UNB-XIN-ECO-004 | Tolerance | Set absolute and relative reconciliation tolerances per cost source; investigate unexplained residuals. | Calibrate from actual invoices; no invented universal threshold. |
| UNB-XIN-ECO-005 | FOCUS version | Propose 1.4 as candidate pin; verify published specification and provider compatibility before adoption. | Version schema and test migration. |
| UNB-XIN-ECO-006 | Practical capacity | Measure productive capacity by role, including leave, training, controls and incident load. | Pilot time study; avoid treating utilization as productivity. |
| UNB-XIN-ECO-008 | Donor quote | Require clear advance quote for charged operations and renewed consent for material increases; accessibility exceptions supported. | Counsel and sponsor approve disclosures. |
| UNB-XIN-ECO-015 | Capacity buffer | Model buffer by demand variability, deadline and criticality; no fixed universal percentage. | Stress-test peak and absence scenarios. |
| UNB-XIN-ECO-017 | December volumes | Use anonymized historical sponsor arrival and completion data by day, channel and case type. | If unavailable use labeled synthetic scenarios, not claimed observations. |
| UNB-XIN-ECO-018 | Ceiling | Treat cost ceiling as a review trigger rather than service denial; calibrate by class and cost basis. | CFO and counsel review fairness. |
| UNB-XIN-ECO-021 | Privacy thresholds | Propose at least five sponsors and 60% maximum dominance, with complementary suppression and reidentification review. | Privacy owner validates; thresholds alone do not guarantee anonymity. |

## Cross-cutting challenges

- Separate accounting recognition from operational workflow state and legal entitlement.
- Never use economic budgets to bypass mandatory service or controls.
- Separate shared factual evidence from sponsor-specific authority and decisions.
- Reconcile proposed 60-day reconsideration window with XID-CLM-001 and RCD-046.
- Treat numerical sampling and privacy thresholds as provisional; validate independently.

Next: reconcile source records, add acceptance tests, verify external evidence and regenerate register through existing tooling only after appropriate approval.

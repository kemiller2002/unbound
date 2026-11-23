# Unbound one-system cross-component reconciliation after 20 landscape gaps
Date: 2026-11-23. Design-level synthesis; not exhaustive source-to-requirement traceability or executed validation.

## Reconciliation result
The 20 initial DAF landscape gaps now each have a dedicated proposed requirement (REQ-X-058..077), following REQ-X-057's one-system constraint. **This closes initial high-level specification omissions, not evidence or implementation gaps.** None of the requirements imply vendor parity, legal certification or production readiness.

## Component ownership map
| Domain capability | Authoritative in-process component(s) | Relevant requirements | Boundary to verify |
|---|---|---|---|
| Sponsor governance, delegated authority | Decision rights + Sponsor Policy | REQ-X-044/045/057/064/076 | Board conflict controls need specialist validation |
| Financial accounting, fund assets and custody | Ledger, Fund Accounting, Provider Integration | REQ-X-022/023/039/040/059/062/067 | Gift acceptance vs accounting event boundary |
| Donor/family/advisor identity and relationships | Identity/Delegation, Fund Relationship | REQ-X-044/057/062/068/072 | Donor, adviser, sponsor and charity distinct principals |
| Grant recommendation and approval | Grant Domain, Policy | REQ-X-018/019/020/063 | Embedded initiation cannot create second decision path |
| Charity vetting and cross-border | Charity Registry, Policy, Case | REQ-X-048/058/065 | Shared evidence versus sponsor-private judgments |
| Payment, check, remittance and returns | Payment Domain, Provider Adapters | REQ-X-022/041/058/063/077 | Recipient status derived, no second settlement ledger |
| Nonprofit contact and recipient experience | Identity, Charity, Grant, Payment | REQ-X-058/069/077 | No charity portal shadow records |
| Recurring, payroll and employer matching | Contribution/Grant Scheduler, Ledger | REQ-X-022/070 | Every occurrence revalidates sponsor rules |
| Communications, email, brand and documents | Communication, Brand, Artifact Registry | REQ-X-027..038/051/052 | Protected disclosures cannot be edited by model |
| Work queues, cases and operational learning | Work Orchestration | REQ-X-041/042/048/049/054 | No separate compliance or recipient ticket system |
| Dashboards, reports, tax and audit | Semantic Reporting, Artifact Registry | REQ-X-039/040/043/050/060 | Legal filing signoff outside model |
| AI assistance and contextual UX | Assistance Orchestrator, Forma | REQ-X-051..055 | No AI financial or legal authority |
| Institution onboarding, migration and offboarding | Sponsor Configuration, Data Contracts, Cases | REQ-X-045/046/061/064/071 | Single-writer cutover and portable evidence |
| Partner fees, billing, conflicts | Fee Engine, Ledger, Governance | REQ-X-023/064/067/076 | Legal payee and fee recipient distinct |
| Fraud, IAM, vendor assurance, DR | Identity, Risk, Evidence, Resilience | REQ-X-044/046/047/072/073 | Independent assurance and safe recovery |
| Optional campaigns and adjacent vehicles | Donor UI, Grants, Partner Adapters | REQ-X-066/074/075 | No expansion of legal entity model without ADR |

## Cross-component collision and ambiguity register
| Collision | Failure mode | Recommended resolution |
|---|---|---|
| Cross-sponsor charity evidence | Shared evidence may be reusable, but private sponsor-specific eligibility decisions and restricted investigations cannot be exposed. | Charity Registry owns evidence; Sponsor Policy owns sponsor decision; authorized cross-sponsor reuse only. |
| Donor vs charity vs advisor identity | A party can occupy multiple roles, but legal rights differ. | One identity service with scoped role relationships, no merged authorization. |
| Sponsor-operated vs turnkey | Software product may not be the legal sponsor. | Sponsor contract records legal party, delegation and operating role before activation. |
| Advisor investment vs fund ledger | Advisor/custodian positions can conflict with sponsor accounting. | Custody is external source; reconciled sponsor ledger authoritative for statements. |
| External DAFpay vs internal grants | Checkout may claim donation success before sponsor approval/payment. | One grant state machine, externally scoped status events. |
| Complex asset intent vs accepted contribution | Premature receipt/available balance could be created. | Separate intake, legal acceptance, custody, valuation, liquidity and grantable balance. |
| Tax guidance vs compliance | Model advice could be mistaken for professional tax determination. | Transparent comparison with qualified review and safe no-DAF outcome. |
| Regulatory filing vs document generation | Producing PDF does not establish filing acceptance. | Separate prepared/reviewed/signed/submitted/accepted states with external evidence. |
| Migration vs live writer | Parallel operations risk duplicate financial posting. | One fenced writer at cutover, dual read-only reconciliation, rollback/compensation. |
| Campaign intent vs grant payment | Pledge may be represented as money settled. | Distinct commitment, contribution, grant recommendation and settlement. |
| Related-party fee vs policy | Sponsor/vendor conflicts can be hidden in billing. | Independent approval, recusal and ledger-linked disclosure. |
| Notification vs human case | Proactive AI can create parallel exception queues. | Single work case with deduplicated health issue reference. |

## Design decisions challenged
1. **Do not create a universal Party record that conflates legal authority.** Share identifiers, keep explicit relationship/role/effective date.
2. **Do not duplicate payment state for nonprofit UX or checkout.** Read the same grant/payment timeline and disclose Unknown honestly.
3. **Do not require every nonprofit to onboard digitally.** Low-friction verified checks and staff assistance remain available.
4. **Do not accept every complex asset.** Sponsor-specific gift acceptance is a policy and specialist review boundary.
5. **Do not treat automated tax guidance as professional advice.** The correct outcome may be no DAF.
6. **Do not turn optional workplace/campaign/other-vehicle capabilities into default platform complexity.** Use explicit buyer-value and legal gates.
7. **Do not mistake configuration for legal outsourcing.** Sponsor contract and actual board delegation must be evidenced.
8. **Do not interpret AI-generated filing/report as completed legal filing.** Independent signature/submission/acceptance evidence required.
9. **Do not assume migration is just an import.** Financial tie-out, parallel run and single-writer cutover are acceptance gates.
10. **Do not equate completed requirements with validated parity.** All vendor demos, contract evidence and executable end-to-end tests remain outstanding.

## Proposed build / partner / defer disposition
**Build as native orchestration:** recipient self-service state, gift acceptance workflow, regulatory evidence packages, migration reconciliation, advisor permissions, DAF checkout API, sponsor contract/delegation, fee waterfall, family relationships, grant impact evidence, institutional onboarding, identity authority, vendor assurance evidence, related-party governance and recipient interoperability.

**Partner with controlled adapters:** bank/identity verification, custody and securities execution, complex appraisal/liquidation, cross-border due diligence and FX, tax advice, payroll/matching infrastructure, assurance audits and specialist impact measurement.

**Defer or condition on buyer evidence:** general-purpose private foundation/trust/annuity support, broad social campaigns and full employer-giving marketplace. A partner may still be supported through a bounded workflow without expanding Unbound's authoritative financial semantics.

## Missing decision inventory (owner validation needed)
- Is Unbound only technology/administration, or will an affiliated sponsoring charity exist? Do not assume.
- Which legal sponsor and jurisdiction combinations are in launch scope?
- Which complex assets are accepted initially?
- Which noncash tax substantiation and sponsor filing packages are essential for first buyers?
- Which custody/CRM/payments providers must be supported first?
- Is direct DAFpay interoperability a launch requirement or an integration phase?
- Are nonprofit portal and recipient bank changes part of the launch critical path?
- Which migration source platforms are in scope and how much history is contractually guaranteed?
- Which international grant countries and compliance pathways are permitted?
- What customer segments justify workplace, family or campaign modules?
- What sponsor/operator/advisor fee flows are legally and commercially allowed?
- Which decisions require sponsor board, independent finance, legal counsel or CPA approval?

## Follow-up verification gates
**Gate A:** inventory all `daf-landscape` documents, data tables and source registers with evidence dates.
**Gate B:** inventory all Unbound requirements, decisions, UI requirements and acceptance scenarios.
**Gate C:** produce machine-readable bidirectional mapping; detect unreferenced research claims and orphaned requirements.
**Gate D:** adversarially simulate SCN-01..10 end to end using synthetic data for all external interactions.
**Gate E:** review P0 legal/accounting/security assumptions and obtain external signoff before implementation.
**Gate F:** measure buyer-specific value and real-world operating economics before declaring competitive advantage.

## Definition of complete
'High-level coverage documented' is defensible after all gap specifications and cross-component mapping. 'Exhaustively reconciled' requires full repository inventory and source-by-source mapping. 'Competitively validated' requires vendor evidence and actual product tests. 'Production ready' requires implementation, security/financial tests and applicable approvals. None of the latter three are claimed here.

# DAF landscape ↔ Unbound: requirements reconciliation and prioritized gap register
Date: 2026-11-23. **Second-pass evidence-backed design audit, not exhaustive repository inventory, independent vendor verification or executed product test.**

## Decision
Compare **end-to-end sponsor outcomes** across the buyer categories and vendor roles in `daf-landscape`. Do not copy competitor UIs, conflate legal sponsor with technology operator, or treat an unverified feature claim as a delivered product. Status describes *theoretical requirement coverage* only.

## Additional source families inspected
- `daf-landscape/research/README.md` research workstream index
- `research/expanded-daf-ecosystem-2026-11-11.md`
- `research/capabilities-2026-11-11.md`
- `research/ren-tifin-chariot-deep-dive-2026-11-11.md`
- `data/platform-comparison-2026-11-11.csv`
- `research/platform-evaluation-protocol.md`
- `research/migration-runbook-2026.md`
- `research/tax-opportunity-lifecycle-2026-11-11.md`
- `research/daf-economics-and-distribution-2026-11-11.md`
- The index identifies additional tax, governance, assurance, donor behavior, switching and source datasets. Those linked files have not all been individually audited in this pass. No claim of complete file coverage.

## Reconciliation method
For each capability capture: source path, provider/legal role, source date, evidence grade (technical spec / official disclosure / marketing / independent observed / unknown), buyer, user journey, Unbound requirement links, theoretical status, gap ID, priority, owner, disposition, synthetic test, legal/accounting review need. A requirement match is **not** implementation evidence. An unassessed competitor is **not** a competitor failure.

## Priority gap register
| Gap | Priority | Capability | Coverage | Landscape anchor | Existing Unbound anchor | Required end-to-end verification | Proposed disposition |
|---|---|---|---|---|---|---|---|
| GAP-001 | P0 | Nonprofit self-service and grant receipt | Potential gap | Ren for Charities, Chariot nonprofit dashboard | REQ-X-022/048, payments, Document Center | Nonprofit portal, bank ownership proof, remittance and acknowledgment | Build core recipient workflow; partner bank verification |
| GAP-002 | P0 | Complex gift acceptance and liquidation | Partial | Ren complex gifts; Fidelity/NPT | REQ-X-022/023 | Gift acceptance board policy, appraisal, restrictions, liquidation, receipt, custody | Build acceptance orchestration; partner specialized valuation |
| GAP-003 | P0 | Sponsor regulatory and tax reporting | Potential gap | Sponsors' 990/audit workflows | REQ-X-034..040, REQ-X-044 | 990/schedules, substantiation, donor receipt, state filings, audit package, controls | Build data/evidence pipeline; professional review |
| GAP-004 | P0 | Migration and sponsor exit | Partial | AEF migration; platform switching research | REQ-X-022/034/037/046 | Historic balance/transaction/doc migration, dual run, cutover, export rights | Build mandatory core capability |
| GAP-005 | P0 | Advisor identity, custody, CRM and SMA operations | Partial | TIFIN Give, Foundation Source/BridgeFT, NPT | REQ-X-023/045/046/050 | Advisor books, delegated rights, trade authority, holdings freshness, CRM and SSO | Build role/control model; partner adapters |
| GAP-006 | P0 | Nonprofit checkout / DAFpay initiation | Potential gap | Chariot DAFpay, Daffy API | REQ-X-045/046/051 | Third-party grant initiation, consent, verification, webhooks and fraud | Build standard API and embed; optional provider partner |
| GAP-007 | P0 | Legal operating model and sponsor-vendor contracting | Partial | Ren/RCF, GiveClear/TIFIN/Chariot, NPT | REQ-X-044/045/047 | Contract role matrix, outsourcing, fiduciary controls, insurance, liabilities | Build operating-model templates, counsel gate |
| GAP-008 | P1 | International grants and cross-border controls | Potential gap | Major sponsors, specialized partners | Grant vetting and payments | Equivalency/expenditure responsibility, sanctions, FX, reporting | Build eligibility boundary; partner specialist diligence |
| GAP-009 | P1 | Tax-fit donor and advisor guidance | Potential gap | TIFIN Tax Intelligence, Ren estimator | REQ-X-051..055 | DAF vs direct giving, tax lot triggers, qualified review, no-DAF recommendation | Partner tax professional; constrained guidance |
| GAP-010 | P1 | Partner economics and fee waterfall | Partial | TIFIN ADV, RCF-Ren audited payments | REQ-X-023/045 | Sponsor/operator/advisor/custodian/rail fee attribution and conflicts | Build contract and accounting controls |
| GAP-011 | P1 | Family governance and collaborative giving | Partial | TIFIN family giving, national sponsors | Succession, roles, portals | Family roles, invited collaborators, pooled recommendations, governance | Build donor-facing module |
| GAP-012 | P1 | Nonprofit impact and grant restrictions | Partial | Charity-facing and donor engagement programs | Reports and notes | Grant restrictions, outcome evidence, acknowledgments, follow-up | Build optional sponsor-scoped reporting |
| GAP-013 | P1 | Workplace/payroll/matching giving | Strategic adjacency | Benevity, Givinga, Blackbaud Giving Fund | Recurring contributions | Employer files, matching, corporate programs, refund and receipts | Partner first; assess buyers |
| GAP-014 | P1 | Implementation, service tiers and SLAs | Potential gap | NPT private label and vendors | REQ-X-045/047/049 | Institution onboarding, training, SLA, help desk, pricing, rollout | Build operational requirements |
| GAP-015 | P1 | Identity, donor fraud and account recovery | Partial | Ren SSO, Chariot OAuth, NPT SSO | REQ-X-044/046 | KYC/identity, fraud, MFA, delegated recovery, compromised account | Build cross-role identity controls |
| GAP-016 | P1 | Vendor assurance and SOC control ownership | Partial | Ren, Chariot, TIFIN assurances | REQ-X-044/047 | SOC1/2 scope, CUECs, subprocessor assurance, evidence | Build assurance program; audit independently |
| GAP-017 | P2 | Private foundations, trusts and planned giving | Strategic adjacency | Foundation Source, PG Calc, Ren | Succession, estate | Vehicle choice, legal entity, separate accounting and tax rules | Partner/defer pending buyer demand |
| GAP-018 | P2 | Community campaigns and peer giving | Strategic adjacency | TIFIN and digital-first sponsors | Donor collaboration | Campaign ownership, permissions, attribution, no prohibited benefit | Validate demand before build |
| GAP-019 | P1 | Sponsor conflict-of-interest and related-party governance | Partial | Charityvest, RCF and OneGive disclosures | REQ-X-044, fees | Board recusals, vendor incentives, model ranking, disclosure evidence | Build control model |
| GAP-020 | P1 | Grant recipient data interoperability | Potential gap | Chariot, Ren for Charities | REQ-X-046 | Charity CRM matching, standardized grant IDs, nonprofit attribution | Build event and data contracts |

## Buyer operating models: distinct product contracts
1. **Sponsor-operated SaaS:** charity owns sponsor, board, custody, tax receipts, compliance; Unbound supplies technology and configurable controls. Test sponsor legal delegation and offboarding.
2. **Outsourced administration:** sponsor retains fiduciary authority while operator performs defined service work. Test contractual role, service-level evidence, separation of duties and delegated permissions.
3. **Turnkey sponsor + technology + distribution:** a wealth manager wants a branded program without becoming sponsor. Requires explicit legal sponsor partnership, advisor/custodian economics, marketing and consent; do not imply Unbound itself is a 501(c)(3).
4. **Embedded/transaction infrastructure:** external platform initiates a grant recommendation or payment instruction; sponsor still approves. Distinguish donor authorization, provider grant decision, money movement and settlement.
5. **Digital donor-led sponsor:** low-balance recurring donor economics and mobile self-service differ from institutional white label.
6. **Adjacent workplace/corporate and planned-giving:** partnership or separate vehicle decisions, not default DAF parity.

## Ten end-to-end theoretical acceptance scenarios
- **SCN-01 Institution launches branded sponsor program:** contract/board -> SSO -> config -> donor -> custody -> first statement. Related GAP-007,014,015. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-02 Complex appreciated asset:** intent -> acceptance -> appraisal -> custody -> liquidation -> receipt -> NAV. Related GAP-002,003,005. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-03 Advisor-managed multi-custody family fund:** delegation -> trade/positions -> corporate action -> fee -> succession -> grant. Related GAP-005,010,011. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-04 Nonprofit checkout and grant payment:** external consent -> recommendation -> sponsor decision -> payee verification -> settlement -> receipt. Related GAP-001,006,020. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-05 Recipient bank change fraud:** charity login -> change request -> proof -> independent verification -> hold -> safe release. Related GAP-001,015,020. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-06 180-day review and recurring grant:** scheduled recurrence -> fresh charity evidence -> adverse hold -> human case -> outcome. Related GAP-001,008. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-07 Payroll and matching:** employer batch -> allocation -> acceptance -> match -> reversal -> substantiation. Related GAP-003,013. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-08 Sponsor platform migration:** legacy history -> import -> parallel ledger -> reconcile -> cutover -> donor continuity -> export. Related GAP-004,014. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-09 Cross-border grant:** recipient eligibility -> legal controls -> FX -> sponsor approval -> remittance -> reporting. Related GAP-008,020. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.
- **SCN-10 Audit-ready close and tax year:** GL close -> statement -> regulatory data -> board review -> 990/audit package -> retention. Related GAP-003,016,019. Require UI/API, owner, authoritative state, exception paths, mock provider data, cost and evidence.

## Additional structural conclusions
**Recipient is a separate persona:** donor portal and internal grant review do not substitute for charity bank onboarding, payment visibility, remittance reconciliation and nonprofit CRM integration.

**Tax and gift acceptance are separate from grantmaking:** DAF tax opportunity, legal acceptance of a contributed asset, investment custody, donor substantiation and subsequent grant recommendations have different authorities. AI should sometimes recommend no DAF, with clear professional advice boundaries.

**Migration is a product capability:** an institution will not switch platforms merely because the new system is better if historic balances, audit trails, records and identity relationships cannot be migrated and reconciled.

**Financial economics span several entities:** platform revenue, sponsor fees, advisor fees, custody costs, payment fees and donor account balances are different measures. Require typed attribution and conflict disclosures.

**Potential competitive advantage:** an AI-native component monolith with typed policy, provenance, deterministic automation and user-guided workflows could reduce cost and manual touches, but no benchmark supports a quantified advantage yet. Compare real settled outcomes and cost-to-serve, not feature counts.

## Verification rules
- Do not declare parity until source-backed vendor feature is product/SKU scoped and Unbound workflow has a complete theoretical contract, and later a passing executable test.
- Do not auto-promote gaps to P0 implementation commitments; P0 means prioritize requirements validation.
- Counsel/CPA must determine sponsor tax/legal duties and cross-border details; source statements are research inputs, not legal advice.
- Sponsor and vendor commercial models require independent contractual diligence.
- Each gap must eventually have explicit decision Build / Partner / Defer / Exclude with rationale and owner.

## Remaining audit work
1. Machine-inventory every relevant `daf-landscape` research and data file, not only the index and selected deep dives.
2. Machine-inventory all Unbound requirements and trace each exact REQ-X and domain acceptance scenario.
3. Expand the 20 gaps into sponsor/provider/role evidence rows, including Foundation Source, NPT, GivingTech, Fidelity, DAFgiving360, Vanguard, Daffy, Chariot, Benevity and others.
4. Create a canonical typed CSV/JSON evidence matrix with source identifiers and lifecycle status; detect unmapped research claims and orphaned requirements.
5. Add detailed P0 requirements and run ten synthetic end-to-end theoretical simulations before software implementation.
6. Obtain live vendor demonstrations/quotes, external legal/CPA reviews and real operating metrics before claiming commercial superiority.

Status: second-pass gap register and operating-model synthesis. No vendor demos, legal signoff, software or tests performed.

# DAF landscape -> Unbound high-level capability coverage audit
Date: 2026-11-23. **First-pass requirements mapping, not a completed full repository-by-repository or vendor-live verification.** Sources are the connected `daf-landscape` research files reviewed this session, dated November 11, 2026. Unbound references are conceptual requirements, not implemented functionality.

## Why this audit is needed
Unbound has substantial financial, grant, policy, AI, work queue, document, brand and UX requirements. That does not establish full sponsor operating capability or parity with competitor products. Compare **outcomes and complete workflows**, not marketing feature counts, and distinguish technology vendor, legal sponsoring charity, advisor, custodian, payments infrastructure and nonprofit-facing service.

## Research baseline examined
- `daf-landscape/research/README.md`: security, tax, economics, migration, vendor, sponsor and broader research workstreams.
- `daf-landscape/research/expanded-daf-ecosystem-2026-11-11.md`: 25 entities, five market categories, Foundation Source/Charityvest, NPT private label, GivingTech, national sponsors, workplace and API providers.
- `daf-landscape/research/ren-tifin-chariot-deep-dive-2026-11-11.md`: legal/technology/payment role separation and architecture.
- `daf-landscape/research/capabilities-2026-11-11.md` and `data/platform-comparison-2026-11-11.csv`: vendor evidence states and 25 comparison dimensions.
- `daf-landscape/data/expanded-daf-market-map-2026.csv`: market universe (partially reviewed).
Research claims are dated, scoped and frequently marketing-only; no live vendor product tests were performed here.

## Coverage taxonomy
**Strong theoretical**: multiple detailed requirement packages exist, but no proof of implementation.
**Covered conceptually**: specified at a meaningful level; full operational walkthrough unverified.
**Partial**: some requirements exist, critical workflow dimensions not traced.
**Potential gap**: no sufficiently specific coverage established in this review; may exist elsewhere.
**Strategic adjacency**: decide explicitly whether product supports, partners or excludes.

## Preliminary coverage matrix
| Capability | Status | Current Unbound evidence | Verify or add |
|---|---|---|---|
| Legal sponsor, delegation, fiduciary governance | Covered conceptually | REQ-X-044, policy and grant authority | Validate sponsor vs administrator vs software contractual responsibilities |
| Donor onboarding, KYC, eligibility and identity | Partial | Onboarding and enterprise data contracts | Identity proofing, beneficial ownership, fraud and accessibility journey |
| Contributions cash/securities/complex assets | Partial | Authoritative transactions, AUM and provider adapters | Closely-held business, real estate, restricted shares, appraisal, acceptance and liquidation |
| Fund GL, subledger, NAV, SMA, corporate actions | Strong theoretical | REQ-X-022/023, close and statements | Corporate action, tax-lot and custody feeds as runnable end-to-end fixtures |
| Custody, investment models and advisor controls | Partial | AUM, sponsor configuration | Advisor trading authority, investment restrictions, corporate actions and multi-custody reconciliation |
| Grant vetting, 180-day review and approval | Strong theoretical | Grant policy, REQ-X-048/049, 180-day verification | Charity-side exception and sanctions data quality |
| Payments, ACH, checks, returns and external IDs | Strong theoretical | Payment unknown/idempotency, check fees, external IDs | Positive pay, payee change fraud, nonprofit delivery evidence and settlement benchmarks |
| Nonprofit self-service portal and onboarding | Potential gap | Recipient/payment verification discussed | Charity self-service, bank verification, grant acknowledgement, remittance data, disputes |
| Donor/advisor portal, family and successors | Partial | Document Center, grants, succession, widgets | Multi-generational collaboration, invited roles, family governance and shared giving |
| Advisor CRM and wealth-management distribution | Potential gap | White label, embed, APIs | Advisor books of business, CRM workflows, SSO, relationship ownership and custody economics |
| Institutional white label and multi-sponsor | Strong theoretical | REQ-X-045, brand kit, embedded widgets | Sponsor onboarding/migration and contractual offboarding demonstration |
| DAFpay, checkout and external donation initiation | Potential gap | Embedded widgets and API contracts | Third-party nonprofit checkout, external grant initiation, auth/consent, callback and fraud |
| Recurring grants, future grants and payroll giving | Covered conceptually | Recurring grants and contributions requirements | Payroll employer file/reconciliation, calendar edge cases, recurring revocation |
| Tax planning, DAF vs direct giving | Potential gap | Research tax personas and AI assistance | Product-level tax-fit guidance, limits and referral to licensed professionals |
| Family giving, campaigns and community giving | Potential gap | Donor collaboration, widgets | Shared campaigns, invitations, permissions and impact tracking |
| Planned giving, trusts and private foundations | Strategic adjacency | Estate/succession concepts | Decide explicitly build/partner/out of scope; do not confuse DAF with foundation |
| Nonprofit grants reporting and impact | Partial | Reports and documents | Nonprofit acknowledgements, impact evidence, outcomes, restricted reporting |
| Sponsor compliance, 990, 8283, 1099 and tax operations | Potential gap | Compliance/financial docs broadly | Tax form inventory, sponsor annual returns, contribution substantiation and counsel/CPA signoff |
| Customer service, communications, complaints | Partial | Email, IVR, notes, work queue | Omnichannel intake, service SLAs, dispute handling, account recovery |
| Fee waterfall and partner revenue sharing | Partial | AUM/fees, research economics | Contract-level revenue splits, advisor fees, sponsor subsidies and billing reconciliation |
| Migration, portability, retention and exit | Partial | Data contracts, retention, artifacts | Full legacy import, history continuity, rights, sponsor switching and parallel-run cutover |
| Security, SOC2, fraud, IAM, operational resilience | Strong theoretical | REQ-X-044..047, evidence, DR | Independent control evidence, penetration testing, exact scope and compliance mappings |
| AI-native assistant and operational learning | Strong theoretical | REQ-X-051..055 and learning | Real-world model evaluation, calibrated uncertainty and measurable cost-to-serve |
| Workplace/corporate giving and matching | Strategic adjacency | Recurring contributions | Decide if employer giving, matching, payroll and corporate charitable accounts are target |
| International grants and cross-border controls | Potential gap | General charity compliance | Equivalency determination/expenditure responsibility, FX, sanctions and jurisdictional restrictions |
| Buyer economics, procurement, implementation and service model | Potential gap | Market/economics research | Productized onboarding, SLAs, support tiers, pricing, contractual liability, implementation timeline |

## Highest-priority suspected omissions
1. **Nonprofit-facing experience:** verified charity identity, self-service bank updates, remittance detail, grants search/acknowledgements, issue resolution, charity fraud and recipient-facing APIs. Ren for Charities and Chariot nonprofit surfaces make this a real competitive dimension.
2. **Complex contribution acceptance and investment operations:** separate gift acceptance/valuation/tax substantiation, liquidation, custody, cost basis and investment authority from generic contribution intake.
3. **Advisor distribution and external checkout:** wealth-manager CRM/SSO/custody data and Chariot-like embedded grant initiation are different user journeys from a donor portal.
4. **Sponsor statutory/tax operations:** tax acknowledgements, required reporting, Form 990 and sponsor legal governance should have explicit workflow and evidence owners; confirm form applicability with counsel/CPA.
5. **Migration and implementation:** historical transactions and documents, fund continuity, parallel runs, financial reconciliation, cutover, rollback, offboarding and sponsor portability.
6. **Tax-fit and donor acquisition:** tax-opportunity guidance must compare DAF versus direct giving and sometimes advise against DAF; avoid unlicensed personalized tax advice.
7. **Family/community and workplace giving:** important for some buyers, but decide scope and economics instead of indiscriminate parity.
8. **International grants and charitable compliance:** material for enterprise sponsors; jurisdiction-specific obligations need counsel.
9. **Commercial model and partner fee waterfall:** provider/sponsor/custodian/advisor economics, pricing and revenue allocation.
10. **Non-DAF adjacent products:** private foundations, planned giving, trusts, charitable gift annuities. Make explicit build/partner/out-of-scope decisions.

## Competitor universe and evaluation method
- **Core platform:** Ren, Foundation Source, TIFIN Give, GivingTech DAFtech.
- **Sponsorship/white label:** NPT, RCF, GiveClear, Charityvest, AEF, Greater Horizons.
- **National donor experiences:** Fidelity Charitable, DAFgiving360, Vanguard Charitable.
- **Digital and embedded:** Daffy, Chariot DAFpay and Disbursements.
- **Workplace/corporate:** Benevity, Givinga, Blackbaud Giving Fund.
- **Infrastructure/adjacent:** BridgeFT, PG Calc.
Do not double-count acquired Vennfi or Giving Place as standalone present competitors; do not confuse Foundation Source cross-product AUM with DAF-only assets.

For each provider, capture legal role, product/SKU, buyer, evidence source/date, capability claim, proof class (official technical docs, independent demonstration, marketing, unassessed), Unbound requirement IDs, end-to-end test, unresolved question, product priority and proposed disposition (match, exceed, partner, defer, exclude). A blank or unassessed vendor cell does NOT mean vendor lacks the feature.

## Workflow-based parity gates
A. New institutional sponsor from contract through brand/SSO/configuration, donor onboarding, custody and first reconciled statement.
B. Complex security gift through appraisal, sponsor acceptance, custody, liquidation, tax receipt, NAV and grantable capacity.
C. Donor/wealth advisor family fund with successors, advisor-managed SMA, multi-custody valuation and fee waterfall.
D. Third-party charity checkout/DAFpay initiation through sponsor approval, payment, charity acknowledgement, returns and reconciled donor status.
E. Nonprofit recipient onboarding, identity/bank verification, remittance, disputes and changed-bank fraud.
F. Sponsor 180-day charity reverification, adverse event, recurring grant and immutable hold.
G. Payroll recurring contribution, employer match, reversal, tax substantiation and correct DAF tax guidance.
H. Legacy sponsor migration: years of financial records, immutable documents, reconciliation, parallel run, cutover and offboarding.
I. Cross-border grant with sanctions, legal analysis, currency, reporting and recipient evidence.
J. Month-end close, audited statement, sponsor annual regulatory/tax reporting and independent governance.
Each must map UI, APIs, authoritative data, roles, third parties, exception branches, reports, costs, mock fixtures and acceptance evidence.

## Adversarial challenge
- Parity is not a goal by itself: preserving outcomes with fewer steps matters more than copying vendor workflows.
- Some vendors are legal sponsors, some software, some payments, some investment custody. A missing feature may be a deliberate partner boundary.
- No unverified market claim should become a hard requirement without buyer value and supporting evidence.
- The presence of an Unbound requirement does not imply tested readiness or a complete workflow.
- Research is dated November 11; revalidate high-impact claims before purchasing, competing or making public claims.
- Compliance interpretations and tax form applicability require counsel/CPA.
- Prioritize outcomes by buyer segment, frequency, risk, economic leverage and differentiation; do not simply maximize the feature count.

## Recommended follow-up
1. Inventory all `daf-landscape` research and datasets (not only reviewed files) and all Unbound requirements; build machine-readable bidirectional mapping.
2. Add evidence-coded provider-by-provider rows, with citations and claim date.
3. Mark every missing/partial capability with owner, priority, design/partner decision, acceptance scenario and dependency.
4. Use the ten workflow parity gates and synthetic mock interactions to test theoretical completeness.
5. Run adversarial reviews and independent legal/accounting checks; do not claim vendor feature parity until evidence supports it.

Status: **first-pass audit**, not full exhaustive verification. No competitor live tests, implementation or legal certification.

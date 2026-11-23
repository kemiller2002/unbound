# Cross-industry discovery: nonprofit grantmaking (J), regulatory validation (§9), government and regtech analogies (M partial)

Codes: NPG, REG, GOV. Status: proposed, unvalidated. Date: 2026-11-23.

| Output | Count |
|---|---|
| Atlas rows | 42 |
| Findings | 20 |
| New requirements | 15 |
| Refinements | 3: RR-UNB-GRANT-024-r1, RR-UNB-GRANT-026-r1, RR-UNB-GRANT-041-r1 |
| Scenarios | 18 |
| Rejections | 10 |
| Failure modes | 17 |
| Decisions | 11 |
| Conflicts | 2 |
| Regulatory rows | 20 (XRA-001..020) |

The prior reconciliation already covers most of the grant lifecycle: UNB-RCN-GRT-001..017, UNB-RCN-REG-001..003, RR-REQ-X-007-r1, and the RR-UNB-GRANT-011/012/023/027 r1 refinements. This pass adds only what is missing.

## 1. Independent assessment: foundation and community-foundation grantmaking

A foundation grants office works through these steps:
1. Verify the grantee's status.
2. Classify the grantee.
3. Choose a route: routine, expenditure responsibility (ER) or equivalency determination (ED).
4. Set the terms.
5. Approve under delegated authority.
6. Pay.
7. Monitor.
8. Close.
9. File: Schedules I and F, plus ER statements.

**What transfers to a DAF sponsor:**
- Reliance rules:
  - Rev. Proc. 2018-32.
  - Rev. Proc. 2017-53, which expressly covers DAF sponsors.
  - Notice 2006-109 for supporting organizations, as continued by Notice 2014-4 §3.02.
- ER mechanics (§53.4945-5).
- Fiscal-sponsor payee logic.
- Schedules I and F.
- Escheat of uncashed grant checks.
- Payee-fraud controls.

**What does not transfer:**
- **An agreement and a receipt on every grant.**
  - §4966(c)(2)(A) excepts any distribution to a §170(b)(1)(A) organization, whatever its stated purpose.
  - The sponsor claims no deduction for a grant, so it needs no receipt.
  - Counsel should confirm (XID-NPG-006).
- **Monitoring of ongoing grantee relationships.** DAF grants are mostly one-off.
- **Grants to individuals.** These are barred, apart from the new §530A route.

**Where the analogy breaks:** a DAF sponsor refuses grants that donors expected to go through, and those refusals now draw attorney-general scrutiny (UNB-XIN-NPG-006).

## 2. Most consequential discoveries

1. **Who holds title to charitable funds is unconstrained.**
   - Records: XIF-REG-001 (critical) → UNB-XIN-REG-001 (P0).
   - Nothing stops the for-profit platform operator from holding a float of charitable money. That raises state money-transmission and FinCEN money-services-business questions.
   - FinCEN's payment-processor exemption covers payments for goods or services. A grant is neither.
   - The Flipcause and SDG Impact Fund failures in the corpus both involved commingled funds.
   - Fix: accounts are titled to the sponsor, and the operator has instruction-only authority. Any deployment where the operator holds title is blocked until a counsel analysis is recorded.
2. **Equivalency determination is settled enough to build.**
   - Records: XIF-NPG-001 → UNB-XIN-NPG-001 and RR-UNB-GRANT-041-r1. OB-20 moves from low to high confidence.
   - Rev. Proc. 2017-53 §2, read directly: "sponsoring organizations of donor advised funds may also use the guidelines" for the §4966(c)(2)(A) exception.
   - §4.02(1) makes currency computable: up to about two years, tied to the grantee's taxable year.
   - Written advice must come from an attorney, CPA or enrolled agent.
   - Routing becomes deterministic, tried in this order: listed charity → US intermediary → ED → ER → decline.
3. **A 2026 temporary regulation contradicts UNB-RCN-GRT-001.**
   - Records: XIC-NPG-001 → UNB-XIN-NPG-007 (P3).
   - Temp. Reg. §1.530A-7T(e)(2) (FR 2026-20026, published 2026-09-30) treats a DAF distribution that funds a Trump-account general funding contribution as "neither … a distribution to a natural person for purposes of section 4966" and as ER exercised.
   - It applies to taxable years beginning on or after 2026-01-01 and expires 2029-09-30.
   - Fix: a separate recipient class, disabled by default. It does not override the natural-person class.
4. **No workflow exists for a violation found after payment.**
   - Record: UNB-XIN-NPG-008 (P0).
   - Every existing control is preventive. Nothing covers:
     - Form 4720 inputs;
     - §4958 correction;
     - Schedule L;
     - the knowing-manager analysis;
     - board reporting.
5. **The operator's regulatory status is not tied to the features it switches on.**
   - Record: UNB-XIN-REG-002 (P0).
   - Status by feature:
     - Model portfolios or advice can make it an investment adviser.
     - Per-transaction pay can make it a broker-dealer.
     - Hosting solicitations can trigger California AB 488 platform registration.
     - It is a GLBA financial institution only if its own activities are financial. Even then, FTC Safeguards Rule §314.4(j) requires notice to the FTC within 30 days for 500+ consumers (effective 2024-05-13).
   - The sponsor's statutory exclusions do not extend to a for-profit vendor.

## 3. Gap table

| New item | Priority | Note |
|---|---|---|
| UNB-XIN-NPG-001 ED; RR-UNB-GRANT-041-r1 | P0 | Every foreign grant made without ER |
| UNB-XIN-NPG-002 US intermediaries | P2 | Efficiency: $215.6m to nine intermediaries and $432.9m to "friends of" organizations in FY2023–24 |
| UNB-XIN-NPG-003 fiscal sponsorship | P1 | Evidence plus a conduit test |
| UNB-XIN-NPG-004 Schedules I/F | P1 | Capture filing data at approval. The current package covers Schedule D only. |
| UNB-XIN-NPG-005 charity organizational changes | P1 | Change-effect table, including retroactive reinstatement under Rev. Proc. 2014-11 |
| UNB-XIN-NPG-006 discretionary holds | P1 | Criteria, automatic expiry and a consistency report. In 2026, 16 state attorneys general wrote to three national sponsors about restricting payments to one charity. |
| UNB-XIN-NPG-007 §530A | P3 | Optional |
| UNB-XIN-NPG-008 excise-tax incidents | P0 | |
| UNB-XIN-NPG-009 FCPA | P1 | A nonprofit corporation is a "domestic concern" (15 USC §78dd-2(h)(1)) |
| UNB-XIN-NPG-010 OFAC general licenses | P0 | Record the general-license basis on each screening decision |
| UNB-XIN-NPG-011 escheat jurisdiction | P1 | Priority: payee's address state, then holder's state |
| UNB-XIN-REG-001 title to funds | P0 | |
| UNB-XIN-REG-002 operator regulatory status | P0 | |
| UNB-XIN-REG-003 CAN-SPAM/TCPA classification | P1 | Classify each message type; handle opt-out and revocation |
| UNB-XIN-GOV-001 rules-as-code | P2 | Golden tests derived from the authorities; temporary rules sunset automatically |
| RR-UNB-GRANT-024-r1 | P1 | Three agreement tiers. A signed commitment is required only for ER, as §53.4945-5(b)(3) requires. |
| RR-UNB-GRANT-026-r1 | P2 | Grantee acknowledgments become optional and never block |

## 4. Workflow redesigns

| Workflow | Today | Redesign |
|---|---|---|
| Foreign grant | Officer chooses ED or ER | Deterministic router; ED expiry computed; renewal queue |
| Grant terms | Agreement on every grant | Three tiers; for Tier 2 (ER), release is blocked until the agreement is signed |
| Charity change | Staff phone the grantee | Change-effect table: benign events continue; a successor organization needs approval; reinstatement releases held grants |
| Discretionary hold | Board minute | Hold coded with its criterion, with automatic expiry |
| Violation found after payment | Counsel memo | ExciseTaxIncident case; AI may flag it but not decide |

**Division of work:**
- **AI** may extract, suggest matches, and raise signals. A signal can only raise a Hold.
- **Humans** decide:
  - ED advice, which by law must come from a qualified tax practitioner;
  - counsel classes;
  - discretionary holds;
  - whether a general license applies;
  - liability.

## 5. Business-model applicability

| Requirements | Applies to |
|---|---|
| ED, FCPA, OFAC general licenses | Institutional, outsourced-admin, investment-centric, HNW-family-office, hybrid. Optional elsewhere. |
| UNB-XIN-NPG-002 to -006, -008, -011 | Universal |
| §530A | Optional |
| UNB-XIN-REG-001 | Universal; most critical for embedded, white-label and high-volume-low-balance |
| UNB-XIN-REG-002 | Universal. Investment-adviser and broker-dealer risk is concentrated in investment-centric and advisor-distribution; platform registration risk in embedded. |

## 6. Regulatory applicability matrix (mission §9)

Full rows are in `regulatory-applicability.jsonl` (XRA-001..020). Rows that already exist as OB-* obligations are linked rather than duplicated.

| Obligation | Binding | Status and confidence |
|---|---|---|
| §4966, §4967, §170(f)(18), §4958(c)(2) | Y | OB-01, OB-03, OB-09, OB-05 |
| Supporting organizations | Y | Notice 2014-4; OB-19 raised to high |
| Schedule D | Y | OB-14 |
| Schedules I and F | Y | XRA-005, medium |
| Forms 8283 and 8282 | Y | OB-12, OB-13 |
| REG-142338-07 (proposed DAF regulations) | N | **Still proposed as of 2026-11-23.** The Federal Register API returns only the 2024-04-19 hearing notice, and Council on Foundations (2026-05-08) says the rules are not final. XRA-002, high. |
| Rev. Proc. 2017-53 ED | Reliance safe harbour | XRA-001, high |
| ER, §53.4945-5 | Y | XRA-018, high; OB-02 raised |
| §1.530A-7T | Y | XRA-004 |
| State solicitation registration; CA AB 488 | Y | OB-37, OB-38 |
| Grantee's own state registration standing | N for the grantor | XRA-015; rejected as a grant gate (XRJ-NPG-002) |
| Unclaimed property | Y | XRA-014. No general exemption for nonprofit holders found. Priority: payee's state, then holder's state. OB-45 raised from low to medium. |
| GLBA / FTC Safeguards | Uncertain; doubtful for a nonprofit sponsor | Applies to a for-profit vendor only if it is itself a financial institution. XRA-007. |
| State privacy laws and breach notification | Y where thresholds are met | OB-35, OB-36 |
| OFAC | Y; strict liability, including US-person nonprofits | OB-29, OB-30. Humanitarian general licenses: XRA-017. |
| FCPA | Y | XRA-006 |
| Money transmission / money-services business | Uncertain; the vendor only if it holds funds | XRA-008 |
| Investment adviser | Uncertain; a vendor giving advice for compensation | XRA-009 |
| Broker-dealer | Uncertain; per-transaction compensation | XRA-010 |
| E-SIGN | Y | Consumer-consent duties apply to donor acknowledgments only. XRA-019, OB-47. |
| Records retention | Mixed | ER grantee records 4 years; OFAC 10 years |
| CAN-SPAM, TCPA | Y | XRA-011, XRA-012, medium |
| ADA / WCAG | Uncertain | Case law. The DOJ Title II rule binds governments only. XRA-013. |
| Colorado automated decision-making (SB 26-189) | Uncertain | Effective 2027-01-01; charitable grants not listed. OB-48 raised from low to medium. XRA-020. |
| §6033(j) automatic revocation; Rev. Proc. 2014-11 | Y | XRA-016 |

**Entity status.** The sponsor is not a bank, broker-dealer, investment adviser or payment processor. The operator may become one, depending on its custody, advice, compensation and solicitation features. The UNB-XIN-REG-001 and UNB-XIN-REG-002 gates control that.

**Prior obligations re-checked:**
- OB-02, OB-19 and OB-20 raised to high.
- OB-45 and OB-48 raised to medium.
- OB-21 confirmed still proposed.

## 7. Government and regtech analogies

| Analogy | Assessment |
|---|---|
| Information-return matching | Applies: reconcile Schedules I and F to the ledger. TIN Matching is rejected (XRJ-GOV-002). |
| Ex parte redetermination | Already the design |
| Change reporting | Applies to ER grantees only |
| Fair hearing | Rejected (XRJ-GOV-001); the reconsideration case is sufficient |
| Do-Not-Pay | Already covered by the recheck at payment release |
| Rules-as-code | The gap is golden tests derived from authorities, plus automatic sunset (UNB-XIN-GOV-001) |

The analogies break down in two places: the sponsor has no data-gathering powers, and charities have no due-process rights.

## 8. Cost drivers for the economics model

**Operating cost per grant** (daf-landscape:research/…/sponsor-cost-structure-and-sensitivities.md:12-14):

| Sponsor | Cost per grant |
|---|---|
| Fidelity | $68 |
| DAFgiving360 | $55 |
| Vanguard | $134 |
| NPT | $419 |
| Goldman Sachs Philanthropy Fund | $574 |

**Diligence:**
- Routine eligibility check for a listed domestic public charity: about $0.14 per grant ($139k pool per 1M grants).
- Non-standard diligence (ER, disqualified supporting organizations, international ED): $1.25M per 1M grants.
- RCF's 2023 policy cites about $4k as the typical international cost, with a $50k minimum.
- ED advice stays current for about 2 years, so its cost spreads across every grant to that grantee in the window. Practitioner fees were not obtained.
- ER cost recurs each year the grant funds remain unspent. No public data.

**Volumes and shares:**
- Non-standard grants: 2.5–3% of dollars.
- Review-need proxy: 10–11% of dollars.
- Broad share of grants needing review: about 28–31% by count.
- Foreign grants: 1.92% of dollars, paid 77.1% by wire, 18.9% by ACH and 3.7% by check.

**Other cost pools (per 1M grants):**
- Checks and returns: $348k.
- Disbursement: $174k.

**Anchored labour:** $7.9 per grant at large nationals, $180–323 at community foundations.

**Implication.** Savings come from four places. Automating routine verification further adds little.
- Routing exceptions to cheaper lawful paths: intermediaries and reusable EDs.
- Eliminating checks.
- Dropping agreements from routine grants.
- Dropping receipts.

## 9. Evidence not obtained

- 11 CCR §999.9.3 and §312 text.
- Current Schedule I and F instructions.
- Full text of FinCEN ruling FIN-2014-R009, and current eCFR text; eCFR redirects from this environment.
- 2026 status of the TCPA consent-revocation rule. The CAN-SPAM 10-day opt-out rule was not re-read.
- State escheat business-to-business exemptions.
- ED and ER fee and labour data.
- Whether the 2026 discretionary holds persisted.
- Any §4966 rule published after the 2026-11-23 API check.

## Sources (accessed 2026-11-23)

**Primary, read directly:**
- Rev. Proc. 2017-53: https://www.irs.gov/pub/irs-drop/rp-17-53.pdf
- Notice 2014-4: https://www.irs.gov/pub/irs-drop/n-14-04.pdf
- 2023 DAF proposed regulations (NPRM): https://www.govinfo.gov/content/pkg/FR-2023-11-14/html/2023-24982.htm
- Federal Register API: https://www.federalregister.gov/api/v1/documents.json
- Trump Accounts temporary regulations: https://www.govinfo.gov/content/pkg/FR-2026-09-30/html/2026-20026.htm
- FTC Safeguards amendment: https://www.govinfo.gov/content/pkg/FR-2023-11-13/html/2023-24412.htm
- 26 CFR 53.4945-5: https://www.law.cornell.edu/cfr/text/26/53.4945-5
- 15 USC 78dd-2: https://www.law.cornell.edu/uscode/text/15/78dd-2

**Search summaries only:**
- Council on Foundations: https://cof.org/node/20145
- FinCEN FIN-2014-R009
- OFAC general-license rule (FR 2022-27639)
- CA AG delinquency page
- Consumer Finance Monitor on SB 26-189
- IOFM escheat Q&A

**General knowledge, medium or low confidence:**
- CAN-SPAM and TCPA
- ADA
- Investment-adviser and broker-dealer analysis
- Texas v. New Jersey escheat priority rule

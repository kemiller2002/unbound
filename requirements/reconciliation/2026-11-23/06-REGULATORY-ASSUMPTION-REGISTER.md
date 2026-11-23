# 06 — Regulatory and policy assumption register

Date: 2026-11-23 | Status: desk research, **not legal advice**. Every item marked "counsel" requires qualified review before it can constrain or relax a requirement. Machine-readable register: [`data/regulatory-obligations.csv`](data/regulatory-obligations.csv) (49 obligations: authority type, citation, jurisdiction, who it applies to, status with verification date, evidence location, confidence, counsel question).

## 1. Method and evidence currency

- Primary input: `daf-landscape/research/ai-native-daf-enterprise/08-governance-and-compliance/regulatory-applicability-map.md` and `regulatory-applicability-matrix.csv` (65 `REG-*` rows, dated 2026-11-20). Unbound carries those rows in `requirements/DAF-LANDSCAPE-CONVERSION-WAVE-04.md` (`UNB-TRACE-003`).
- Statements were checked against the requirement files and, where currency mattered, against primary sources fetched on **2026-11-23**: IRB 2018-23 (Rev. Proc. 2018-32), the Federal Register API, the IRS 2026–2027 Priority Guidance Plan, IR-2026-135, and the OFAC final rule (FR 2025-04864).
- Authority types used: statute · final regulation · proposed regulation · official IRS guidance · case law · sponsor policy · contractual · operational best practice · research inference.
- Thresholds verified as stated correctly in the corpus: $250 written acknowledgment (§170(f)(8)); more than $75 quid pro quo (§6115); Form 8283 at over $500, qualified appraisal at over $5,000 (not for publicly traded securities), appraisal attached at over $500,000 (§170(f)(11)); Form 8282 for dispositions within 3 years, filed within 125 days (§6050L); §4966 20% sponsor / 5% knowing manager (cap $10,000); §4967 125% / 10%; §4943 applies to DAFs through §4943(e); §4944 does not apply to DAFs.

## 2. Current status of moving law (verified 2026-11-23)

| Item | Status | Requirement impact |
|---|---|---|
| Proposed §4966 regulations, REG-142338-07 (88 FR, 2023-11-14) | **Still proposed**: not final, not withdrawn. The 2026–27 Priority Guidance Plan (2026-09-29) lists "§§4966 and 6033 … certain DAF arrangements". IRS officials said in October 2026 that guidance is coming "soon". | Requirements MUST NOT depend on the proposed definitions (for example "personal investment adviser" as donor-advisor, or deemed distributions). Add a regulatory-change watch item ([UNB-RCN-REG-001](04-MISSING-REQUIREMENTS-REGISTER.md)). |
| OBBBA (P.L. 119-21) nonitemizer deduction, §170(p) | In force from tax year 2026: up to $1,000 ($2,000 MFJ), cash only, **DAF and supporting-organization contributions excluded** (IR-2026-135, 2026-11-10). | Donor tax-fit guidance (`REQ-X-066`) MUST NOT present a DAF contribution as qualifying for the nonitemizer deduction. |
| OBBBA 0.5% itemizer floor; 2/37 top-bracket reduction; 1% corporate floor | In force 2026. Confirmed from secondary sources only; carryover mechanics conflict between sources. | Bunching calculators need CPA sign-off (counsel question Q6). The research labels OBBBA as "convention" (REG-023); it is statute on the donor side. |
| OFAC recordkeeping, 31 CFR 501.601 | Extended to **10 years**, effective 2025-03-21. | Conflicts with the "seven-year minimum" retention defaults (finding RCF-REG-003). |

## 3. Material corrections to requirement assumptions

| ID | Requirement / source | Assumption as written | Correct position | Authority type | Severity | Disposition |
|---|---|---|---|---|---|---|
| RCF-REG-001 | `REQ-X-007` (overlay line 83); `19-questions/POLICY-PRECEDENCE-EVIDENCE-TRUST-AND-CRITICAL-WORKFLOWS-2026-11-23.md:20,49,68` | Charity status "re-verified at least every 180 days" is a non-downgradable platform standard. Evidence ≤180 days old satisfies the charity-status gate. The execution-time check is mainly a sanctions check. Vetting is not repeated per sponsor. | **No law sets a 180-day period.** The source decision record says so (`daf-landscape:docs/automated-charity-assurance-180-day-decision-record-v0.1.md:4,51`). Reliance on IRS classification under **Rev. Proc. 2018-32** (which supersedes 2011-33) is judged **as of each grant date**, lasts only until the IRS publicly announces a change, and is lost if the grantor knew of the change. A third-party BMF report must carry specified fields and a timestamp, and **the grantor must retain a copy** (§8.01). | official IRS guidance; 180 days is sponsor policy | High | Refined in `RR-REQ-X-007-r1` ([07](07-REFINEMENT-RECORDS.md)). Each grant date requires an IRS-status check and a per-sponsor snapshot. 180 days stays a configurable *deep review* interval set by the sponsor. |
| RCF-REG-002 | `DAF-LANDSCAPE-CONVERSION-WAVE-04.md:142` (carried REG-001) | Grant approval "cannot be vested in software"; officer sign-off is required for every grant. | Overstated. §4966 taxes a manager who *knowingly* agrees to a taxable distribution; it does not prescribe a human approval of each grant. Delegating approval to rules is a governance and state-law fiduciary question. Already raised as TVC-003 and OD-01; the carried row text was never corrected. | statute (overstated) | Medium | Record as counsel question Q1 and owner decision RCD-005. The requirement stays: routine approval through a sponsor-board-adopted deterministic policy is permitted only after counsel validation. |
| RCF-REG-003 | `19-questions/FINANCIAL-DOCUMENT-AUDIT-RECONCILIATION-PROVENANCE-AND-VERIFICATION-2026-11-23.md:47`; `DETERMINISTIC-RETENTION-POLICY-ENGINE-HOLDS-AND-DISPOSITION-2026-11-23.md:32`; `REQ-X-036` | "At least seven years" retention for financial documents, "longer when required by law, sponsor policy or legal hold". | The text is correctly framed as an owner-directed floor that longer obligations override. The defect is that **no record class maps to a known longer obligation**: OFAC-related records require **10 years** (31 CFR 501.601, effective 2025-03-21), and §6001 basis and carryover records can be "material" well beyond 7 years. Without classes keyed to authority, the "longer when required" clause cannot be executed deterministically. | sponsor policy (correctly labelled); missing authority mapping | Medium | Refined in `RR-REQ-X-036-r1`: retention classes keyed to authority, including an OFAC class of at least 10 years and a "material until basis is exhausted" class. |
| RCF-REG-004 | `research/.../03-organization/organization-and-authority.md:98` | "Exclusive legal control" cited to §4966(d)(2). | That wording is from **§170(f)(18)(B)**, the donor-acknowledgment requirement. §4966(d)(2) defines a DAF as "owned and controlled" by the sponsor. Already TVF-174, still open. | citation error | Low | Correct the citation when adopted. Acknowledgment templates MUST carry the §170(f)(18) statement (`RR-UNB-ACC-006-r1`). |
| RCF-REG-005 | Disqualified supporting organizations (`TVF`, compliance line 170) | Marked "[unverified detail]". | §4966(d)(4) does cover non-functionally-integrated Type III organizations and donor-controlled Type I/II/FI-III organizations. **TEOS/BMF cannot reveal donor control**, so Pub 78 reliance alone does not settle eligibility. A donor or grantee certification is still required (Notice 2006-109: status to be confirmed by counsel). | statute + guidance | Medium | Gap requirement `UNB-RCN-GRT` (supporting-organization control certification). |
| RCF-REG-006 | Sanctions requirements (`UNB-CHAR-*`, `REQ-X-007`, TVC-006) | Screening treated as a legal duty. | The legal duties are the OFAC prohibitions, blocking, and reporting (blocked-property reports within 10 business days). Civil liability is strict. **OFAC prescribes no screening method**, so screening is a risk-based means of complying. | statute + regulation; screening is best practice | Low | Keep screening as a control. Label its authority as best practice serving a statutory prohibition. Add blocked-property reporting (gap). |
| RCF-REG-007 | KYC/CIP for donor onboarding (`ADM-*`, `DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md:97`) | Left open. | Correctly open. The research finds no BSA/CIP mandate for a charity paying its own grants; the answer may change if a for-profit administrator **holds or moves funds** (money-transmitter question). Identity verification remains a fraud and authorization control, not a legal KYC obligation. | research inference | — | Counsel question Q8. Do not import bank KYC as law. |
| RCF-REG-008 | `daf-landscape:research/mission-2026-11-14/b43n26-03-charity-data-terms-ai.md:145,203,333` | Rev. Proc. 2011-33 cited. | Superseded by Rev. Proc. 2018-32 (substance unchanged). | citation currency | Low | Evidence-only note. daf-landscape is read-only, so it is recorded here and not corrected there. |

## 4. Classification of corpus assumptions by authority (selected)

| Topic | Correct authority type | Requirements it constrains | Note |
|---|---|---|---|
| Sponsor ownership and control; recommendations are advisory | statute (§4966(d)(2); §170(f)(18)) | UNB-LEG-002, UNB-GRANT-001, UNB-ACC-005 | Sound |
| Taxable distributions, expenditure responsibility | statute + final regulation by reference | UNB-GRANT, UNB-RULE, REQ-X-065 | Expenditure-responsibility mechanics as adapted to DAFs → counsel |
| Prohibited benefits, pledges | statute (§4967); Notice 2017-73 (interim guidance) | UNB-GEX affirmations, sponsor white-label affirmations | Ticket and pledge edge cases → counsel |
| Automatic excess benefit (grants, loans, compensation to donors or advisors) | statute (§4958(c)(2)) | Payee integrity (REQ-X-005), UNB-GRANT | Hard prohibition. Not configurable. |
| Excess business holdings | statute (§4943(e)) | REQ-X-059 complex contributions | Requires a holdings test that includes holdings of related parties (gap) |
| Acknowledgment and noncash substantiation | statute + IRS guidance | UNB-ACC-006, REQ-X-059 | Form 8283 Section B donee signature; Form 8282 tracking (gap where absent) |
| Form 990 Schedule D Part I | statute (§6033(k)) | UNB-ACC-021, REQ-X-060 | Watch the PGP §6033 item |
| 180-day revalidation | sponsor policy | REQ-X-007, UNB-CHAR | Configurable, sponsor-owned |
| 7-year retention | sponsor policy | REQ-X-036..038 | Replace with authority-keyed classes |
| BSA/AML, GLBA, NY DFS 500, Reg S-P | non-applicable as federal law to a nonprofit sponsor (research inference) | — | May reach a for-profit provider → counsel |
| State charitable solicitation, CA AB 488 platform registration | state statute | REQ-X-063 external checkout, REQ-X-075 campaigns | Per-state nexus → counsel |
| State privacy and breach notification | state statute | UNB-TEC-003, REQ-X-072 | Nonprofit coverage varies by state |
| UPMIFA; ICA §3(c)(10) prong | state uniform act / federal statute | UNB-INV, UNB-NAV | Applicability uncertain → counsel |
| Nacha 2026 fraud monitoring; PCI DSS | contractual | UNB-PAY, contribution intake | Via ODFI and acquirer agreements |
| Colorado automated-decision law (eff. 2027-01-01) | state statute | AIN, REQ-X-017 | Low; donor-account decisions only |

## 5. Questions for qualified counsel (reference from [14-OPEN-DECISIONS.md](14-OPEN-DECISIONS.md))

1. **Q1** Can grant approval be delegated to deterministic rules adopted by the sponsor board? Who is the §4966/§4967 "fund manager" when a policy auto-approves?
2. **Q2** Can a sponsor rely on charity evidence shared across sponsors under Rev. Proc. 2018-32? What IRS-status check and snapshot are required at each grant date?
3. **Q3** What certification establishes the absence of donor control of a Type I/II supporting organization, and is Notice 2006-109 reliance still available?
4. **Q4** Are equivalency determinations available for DAF grants to foreign organizations, or is expenditure responsibility required?
5. **Q5** What retention schedule applies across §6001, Schedule D support, Rev. Proc. 2018-32 snapshots, OFAC (10 years), state charity and records laws, and privacy deletion rights?
6. **Q6** How do OBBBA floor carryovers interact with bunching models in donor-facing calculators? (CPA)
7. **Q7** Is a QCD to a single-charity fund valid under §408(d)(8) and the §4966(d)(2)(B) exception?
8. **Q8** Does a for-profit administrator that holds or moves funds become a money transmitter or a GLBA financial institution? Does UPMIFA apply to DAF pools? Which ICA §3(c)(10) prong applies?
9. **Q9** Do administrator revenue-share arrangements (for example, the reported Ren/RCF arrangement in which the administrator receives 98% of fundholder fees and its staff sit on the sponsor board) raise §4958 or Schedule L questions for a sponsor contracting with Unbound's operator?
10. **Q10** Is a DAF acknowledgment delivered electronically valid without E-SIGN §7001(c) consumer consent?
11. **Q11** How should an uncashed grant check be treated under state unclaimed-property law, and is a sponsor's check void-and-recredit policy sufficient?

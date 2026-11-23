# 07 — Regulatory applicability matrix (deliverable 7, mission §9)

**Status.** None of this is legal advice. Each row separates three things:
- **binding obligations:** statute and final regulation;
- **guidance and reliance safe harbours;**
- **proposed rules and industry practice.**

Each row also records:
- applicability by entity, activity, jurisdiction, contractual role and business model;
- the citation and the URL actually read, or that the source was a search summary only;
- confidence;
- the counsel question it raises.

This matrix **supplements** the reconciliation's `OB-01..49` ([reconciliation 06](../../reconciliation/2026-11-23/06-REGULATORY-ASSUMPTION-REGISTER.md)). It does not restate them. `related_OB_ids` links each new row to the obligation it extends.

## Entity analysis: who is what

| Entity | Bank? | Broker-dealer? | Investment adviser? | Payment processor / money transmitter? | Basis |
|---|---|---|---|---|---|
| Sponsoring organization (501(c)(3)) | No. It is the bank's nonprofit legal-entity customer; only the control prong of beneficial-ownership CDD applies. | No | No for its own pools: Advisers Act §203(b)(4) and Investment Company Act §3(c)(10)(B) cover a charity's own activities | No. It is an ACH Originator, a Reg E payee for recurring consumer debits, and a card merchant. | 31 CFR 1010.230(e)(3)(ii); 15 USC 80a-3, 80b-3; Reg E §1005.10(b) (industry/bnk.md, industry/inv.md) |
| For-profit platform operator | No | **Possible** if it is paid per transaction | **Possible** if it provides model portfolios or advice; the sponsor's exclusions do not extend to a vendor | **Possible** if it holds title to, or a float of, charitable money. FinCEN's payment-processor exemption covers payments for goods or services, and a grant is neither. | XRA-008..010; gates UNB-XIN-REG-001, -002 |
| Distribution partner or host | — | — | — | Must be structurally excluded from the funds flow (pattern borrowed from Reg CF qualified third parties) | UNB-XIN-OTH-003 |
| Platform company as a charitable fundraising platform (California AB 488) | — | — | — | Possible for a D2C own-brand app or an embedded charity-side button | OB-38; XID-SAS-005 |

## Binding obligations newly evidenced by this mission

These are cited in the individual requirements and are not limited to `XRA-*`.

| Obligation | Binds | Requirement |
|---|---|---|
| Nacha 2026 risk-based fraud monitoring, Phase 2 (effective 2026-06-19/22) for non-consumer Originators; annual review | Sponsor as ACH Originator (contractual via ODFI) | UNB-XIN-BNK-002, RR-UNB-PAY-008-r1 |
| Reg E §1005.10(b) authorization for preauthorized transfers, with a copy to the consumer (the payee's duty) | Sponsor as payee | UNB-XIN-PAY-001 |
| UCC 4A-211 cancellation limits on wires | Payment rail design | UNB-XIN-BNK-004 |
| Rev. Proc. 2017-53 equivalency determination (explicitly available to DAF sponsors; currency computable) | Sponsor, foreign grants | UNB-XIN-NPG-001, RR-UNB-GRANT-041-r1 |
| §53.4945-5(b)(3) signed commitment for expenditure-responsibility grants | Sponsor | RR-UNB-GRANT-024-r1 |
| Temp. Reg. §1.530A-7T(e)(2) (FR 2026-20026, 2026-09-30; sunset 2029-09-30) | Sponsor, optional class | UNB-XIN-NPG-007; XIC-NPG-001 |
| FCPA: a nonprofit corporation is a "domestic concern" (15 USC §78dd-2(h)(1)) | Sponsor, foreign grants | UNB-XIN-NPG-009 |
| FTC Safeguards Rule §314.4(j) breach notice (if the operator is a financial institution) | Operator | UNB-XIN-REG-002 |
| CAN-SPAM opt-out within 10 business days; TCPA consent (tax-exempt callers need only "prior express consent") | Sender legal entity | UNB-XIN-CRM-003 |
| All-party consent to call recording in some states (e.g. Fla. Stat. 934.03) | Sponsor/operator if recording | UNB-XIN-SVC-003 |
| ASC 958 net-asset classification depends on variance power; agency funds are liabilities (observed audit practice) | Sponsor financial statements | RR-UNB-ACC-033-r1, UNB-XIN-ACC-001 |
| Investment Company Act §3(c)(10)(B): pool "maintained by" a charitable organization | Pools | UNB-XIN-INV-002 |

## Proposed rules and status checks

- **2023 DAF proposed regulations (REG-142338-07):** **still proposed** as of 2026-11-23. The Federal Register API returns only the 2024 hearing notice. Not binding. Requirements must not encode them as law (XRA-002).
- **OB-19 and OB-20:** confidence raised to high.
- **OB-21:** confirmed still proposed.
- **OB-45 and OB-48:** confidence raised to medium.
- **OB-02:** confidence raised to high ([industry/grt.md](industry/grt.md) §6).

## Matrix (XRA rows)

<!-- BEGIN GENERATED: regulatory -->
| ID | Topic | Binding | Authority | Citation | Applies to (entity) | Activity | Models | Confidence | Counsel question |
|---|---|---|---|---|---|---|---|---|---|
| XRA-001 | Equivalency determinations for DAF sponsors | uncertain (reliance safe harbor, not mandate) | official IRS guidance | Rev. Proc. 2017-53 §§2, 4.02 | sponsor | direct foreign grants | sponsor-dependent; models offering international | high | May advice procured by another grantor be relied on? |
| XRA-002 | 2023 proposed DAF regulations status (late 2026) | N | proposed regulation | REG-142338-07; 88 FR 77922 (2023-11-14) | sponsor, advisor | all DAF distributions | universal | high | Applicability date and transition when final. |
| XRA-003 | Supporting-organization reliance (Notice 2006-109 via Notice 2014-4) | uncertain (reliance) | official IRS guidance | Notice 2014-4 §3.02; Notice 2006-109 §3 | sponsor | grants to Type I/II/FI-III supporting organizations | universal | high | Certification form wording. |
| XRA-004 | Trump account general funding contributions via DAF | Y | final regulation (temporary) | Temp. Treas. Reg. §1.530A-7T(e)(2), (f); FR Doc. 2026-20026 | sponsor, donor | distributions to Treasury-administered Trump accounts | D2C; HNW-family-office; institutional; hybrid | high | §4967 where the class includes the donor's family? |
| XRA-005 | Form 990 Schedules I and F grant reporting | Y | statute + official IRS guidance | IRC §6033; Form 990 Schedules I and F instructions (not re-read) | sponsor | annual return | universal | medium | Current-year line mapping (CPA). |
| XRA-006 | FCPA anti-bribery | Y | statute | 15 USC §78dd-2(h)(1) | sponsor, platform vendor | international grants | international-capable models | medium | Does a charitable grant 'obtain or retain business'? Diligence standard? |
| XRA-007 | FTC Safeguards Rule breach notification | uncertain | final regulation | 16 CFR 314.4(j); FR 2023-24412 | platform vendor | processing donor financial data | all models with for-profit operator | medium | Is the operator a 'financial institution' or only a service provider? |
| XRA-008 | Money transmission / MSB status of platform operator | uncertain | statute + regulation | 31 CFR 1010.100(ff)(5); FIN-2014-R009; state money-transmission acts | platform vendor | holding or moving funds | embedded; white-label; high-volume-low-balance; D2C | medium | Multistate analysis of instruction-only model. |
| XRA-009 | Investment adviser status of platform operator | uncertain | statute | Advisers Act §§202(a)(11), 203(b)(4); state adviser laws | platform vendor, advisor | investment features | investment-centric; advisor-distribution; white-label | medium | Does executing sponsor-selected models without advice avoid status? |
| XRA-010 | Broker-dealer status of platform operator | uncertain | statute | Exchange Act §§3(a)(4), 3(e), 15(a) | platform vendor | securities liquidation and order routing | investment-centric; HNW-family-office | low | Fee structures that avoid transaction-based compensation. |
| XRA-011 | CAN-SPAM | Y | statute + regulation | 15 USC §§7701-7713; 16 CFR Part 316 | sponsor, platform vendor | email | universal | medium | Classify fundraising appeals sent by sponsor to donors. |
| XRA-012 | TCPA | Y | statute + regulation | 47 USC §227; 47 CFR 64.1200 | sponsor, platform vendor | SMS/voice | universal where SMS used | medium | Status of FCC revocation-rule provisions in 2026. |
| XRA-013 | ADA/WCAG | uncertain | case law + industry practice | ADA Title III; 28 CFR Part 35 (Title II) | sponsor, platform vendor | donor and charity UI | universal | medium | Exposure for white-label sponsor sites. |
| XRA-014 | Unclaimed property for grant checks | Y | statute (state) + case law | State UPA/RUUPA; Texas v. New Jersey, 379 U.S. 674 | sponsor | uncashed grant checks | universal where checks used | medium | B2B exemption applicability to charity payees. |
| XRA-015 | State charity registry standing of grantees | N (for grantor) | statute (state) + regulation | Cal. Gov. Code §12585 et seq.; 11 CCR §312 (not read) | grantee | grantee solicitation | universal | medium | None (policy signal only). |
| XRA-016 | Auto-revocation and retroactive reinstatement | Y | statute + official IRS guidance | IRC §6033(j); Rev. Proc. 2014-11 | sponsor | eligibility holds | universal | medium | Is a grant made during the gap covered by retroactive reinstatement for §4966? |
| XRA-017 | OFAC humanitarian/NGO general licenses | Y | final regulation | 87 FR 78470, 87 FR 78484 (2022-12-21); 31 CFR ch. V | sponsor, platform vendor | grants touching sanctioned jurisdictions | international-capable models | medium | Which GLs apply to DAF grants? |
| XRA-018 | Expenditure responsibility mechanics | Y | final regulation (by reference) | Treas. Reg. §53.4945-5(b)(3), (c); §4966(c)(1)(B) | sponsor, grantee | ER grants | institutional; HNW-family-office; investment-centric; hybrid | high | DAF-specific modifications under NPRM. |
| XRA-019 | E-SIGN scope for grantee agreements | Y | statute | 15 USC §7001(c); UETA | sponsor, grantee | electronic grant agreements | universal | medium | Confirm for unincorporated associations. |
| XRA-020 | Colorado ADMT law re-check | uncertain | statute (state) | Colo. SB 26-189 | sponsor, platform vendor | automated decisions about donors | universal | medium | Does any donor-account decision fall within 'financial or lending'? |
<!-- END GENERATED: regulatory -->

## Questions for qualified legal or accounting review

All are recorded as open decisions in [15](15-OPEN-DECISIONS-AND-SCENARIOS.md) with type `legal` or `accounting`.

<!-- BEGIN GENERATED: decision-types -->
| Type | Open decisions |
|---|---|
| legal | 29 |
| product | 14 |
| governance | 13 |
| accounting | 10 |
| architecture | 9 |
<!-- END GENERATED: decision-types -->

# 00: Existing baseline, capability taxonomy and traceability matrix (mission §2)

Date: 2026-11-23. Baseline: `777a70d`. The reconciliation mission inventoried this baseline in [reconciliation 01](../../reconciliation/2026-11-23/01-BASELINE-INVENTORY.md). This report does not redo that work. It reuses that inventory (`data/inventory.csv`: 4,238 distinct identifiers, of which 1,148 are canonical requirement IDs) and maps it onto a capability taxonomy, which is the frame used for cross-industry comparison.

## 1. What the baseline is, and what it is not

| Question | Answer | Evidence |
|---|---|---|
| Where are the canonical inbound requirements? | `kemiller2002/unbound`, under `requirements/`, `architecture/` and `research/daf-theoretical-enterprise-validation/`. The REQ-X overlay, the TVR registers and about 80 detailed topic specifications sit in `19-questions/`. | [reconciliation 01 §1](../../reconciliation/2026-11-23/01-BASELINE-INVENTORY.md) |
| Status of the requirements | Every file declares itself non-approved ("candidate", "draft", "review pending"). No individual requirement carries a version, a status or an accountable owner. | reconciliation 01 §3 |
| **Documented vs implemented** | **Nothing is implemented.** No application code exists in `unbound`. The Echelon framework's DAF case study (`Echelon-AI-Native-Enterprise-Design-Framework/case-studies/daf/`) is a typed *model* that generates analysis artifacts. It is not a running platform. Every capability marked "covered" below means *documented*, not built, tested or operated. | `unbound` tree; Echelon `case-studies/daf/README.md`; `progress/LEDGER.md` |
| Requirements gate | **FAIL** at the reconciliation's readiness assessment. This mission does not change that verdict. | [reconciliation 15](../../reconciliation/2026-11-23/15-READINESS-ASSESSMENT.md) |

## 2. Capability taxonomy, with the requirement families that own each capability

The taxonomy follows the twelve capabilities named in addendum §19, plus the enterprise-wide platform concerns. Family counts come from `inventory.csv`, requirement kind only.

| Capability domain | Functional owner (proposed) | Baseline requirement families | Reconciliation additions |
|---|---|---|---|
| Contributions and gift acceptance | Contributions / Gift Acceptance | UNB-GAP (part), REQ-X complex contributions, TVR-CON, UNB-REM | UNB-RCN-FIN-001..004, -011..013 |
| Grants and grant payments | Grant Operations; Payment Orchestration | UNB-GRANT (48), UNB-GEX (40), EXC (18), UNB-GFUND (30), UNB-PAY (10), TVR-PAY, TVR-GR, REQ-X-083 | UNB-RCN-GRT-001..017 |
| Charity verification and knowledge | Charity Registry / Compliance | UNB-CHAR (42), UNB-KNOW (40), UNB-EXT (38), UNB-RULE (32) | UNB-RCN-GRT (part), UNB-RCN-REG-* |
| Investments, custody, NAV | Investment Operations | UNB-INV (42), UNB-NAV (53), UNB-CUST (28) | UNB-RCN-FIN-006..010 |
| Accounting and close | Finance / Controller | UNB-ACC (36), ACC-MDL (12), REQ-X-084, REQ-X-040 | RR-UNB-ACC-*, UNB-RCN-FIN-005 |
| Compliance, controls, assurance | Compliance; Control Operations | SOC2 (36), OPS (64), MON (24), GOV-AUTH (26), UNB-CTL, UNB-REGMAP | UNB-RCN-REG-*, UNB-RCN-PLT-005 |
| Relationship, participants, succession | Donor and Sponsor Administration | UNB-PART (9), UNB-SUCC (14), UNB-DON, UNB-ADV, ADM (97, part) | UNB-RCN-ENT-004..016 |
| Customer service and work management | Service Operations | REQ-X-048/049 work queues, REQ-X-025 IVR, ADM (part) | RR-REQ-X-048-r1 |
| Fees and billing | Finance / Partner Economics | UNB-ACC-010, ADM-028, PARTNER-ECONOMICS specification | UNB-RCN-FIN-005, UNB-RCN-ENT-015 |
| Communications | Communications Governance | Email template specifications, UNB-DOC, REQ-X-028 | UNB-RCN-PLT-002 |
| Operational reporting and economics | Operations; FP&A | REQ-X-043, UNB-ECO (7), UNB-ENT-003, economic-model.md (assumption-driven) | — |
| AI automation | AI Governance | AIN (72), UNB-AI-STD, UNB-AGT, UNB-AIO | UNB-RCN-PLT-004 |
| Tenancy, platform, API, UI | Platform | UNB-TEN (20), UNB-PLAT, UNB-API, UNB-API-STD, UX (24), INT (40), WG, REQ-X-045/057 | RR-REQ-X-057-r1, UNB-RCN-ARC-* |
| Event and data architecture | Architecture | UNB-EVT (6), UNB-DATA (12), UNB-IMM (10), REQ-X-020/021/022 | RCF-ARC-001..004, RCD-012 |

**Other baseline areas mapped:**
- **Workflows and their dependencies:** `UNB-WF` (24), `LS-WF`/`E2E-WF`/`ADM-WF`/`CW`, and `TRANSACTION-STATE-TRANSITION-MATRIX-2026-11-23.csv` (TX-01..28).
- **API requirements:** `UNB-API`, `UNB-API-STD`, `UNB-RCN-ARC-001`.
- **AI-native requirements:** the `AIN` family, `UNB-AI-STD`, and `PLATFORM-WIDE-AI-ASSISTANCE…`.
- **Tenant-specific configuration:** `UNB-TEN`, `REQ-X-045`, `SPONSOR-CONFIGURATION-POLICY-LIFECYCLE…`.
- **Customer-facing and administrative interfaces:** `UX`, `INT`, `ADM-036`, and `ui/*`.
- **Compliance and operational assumptions:** `OB-01..49` in [reconciliation 06](../../reconciliation/2026-11-23/06-REGULATORY-ASSUMPTION-REGISTER.md).

## 3. Traceability matrix: cross-industry capabilities against the baseline

Every one of the atlas's capability rows was checked against `inventory.csv` and the reconciliation records before a gap was claimed. Each row lists the existing IDs that cover it (`existing_ids`, resolved by gate D1).

**Coverage classes:**
- **Covered:** an existing requirement states the capability concretely enough to test.
- **Partial:** a requirement exists but is vague, incomplete or untestable.
- **Contradictory:** existing requirements conflict with each other or with the industry obligation.
- **Missing:** no existing requirement.

### By mission industry section

<!-- BEGIN GENERATED: coverage-matrix -->
| Industry | covered | partial | contradictory | missing | Total |
|---|---|---|---|---|---|
| A Banking and financial services | 11 | 21 |  | 4 | 36 |
| B Investment and wealth / M custody, fund admin | 17 | 22 |  | 5 | 44 |
| C Accounting, GL, fund accounting | 11 | 9 | 1 | 3 | 24 |
| D Call centers and customer service | 7 | 20 | 1 | 3 | 31 |
| E Insurance and claims | 8 | 12 | 1 | 1 | 22 |
| F Healthcare administration | 6 | 3 |  |  | 9 |
| G Enterprise resource planning | 24 | 26 | 2 | 5 | 57 |
| H CRM, sales, relationship management | 5 | 9 |  | 2 | 16 |
| I Payments, merchant processing, billing | 3 | 7 |  | 3 | 13 |
| J Nonprofit administration and grantmaking | 13 | 12 | 2 | 4 | 31 |
| K Trust, estate, family office | 5 | 6 |  |  | 11 |
| L Enterprise SaaS and platform operations | 2 | 2 |  |  | 4 |
| M Identity and access management | 3 | 5 |  | 1 | 9 |
| M Legal case mgmt, BPO, fraud investigation |  | 1 |  |  | 1 |
| M Other industries | 7 | 7 | 1 | 1 | 16 |
| M Records management | 1 | 4 | 1 |  | 6 |
| M Subscription billing | 1 | 4 |  | 2 | 7 |
| §18 Operating economics analogues | 1 | 10 |  | 2 | 13 |
| §19 Event architecture analogues |  | 2 | 1 | 1 | 4 |
| §9 / M Regulatory, tax and benefits administration | 7 | 6 |  | 1 | 14 |
| **Total** | 132 | 188 | 10 | 38 | 368 |
<!-- END GENERATED: coverage-matrix -->

### By research pass

<!-- BEGIN GENERATED: coverage-by-pass -->
| Research pass | covered | partial | contradictory | missing | Total |
|---|---|---|---|---|---|
| Banking & payments | 13 | 19 |  | 5 | 37 |
| Investment & accounting | 25 | 19 | 1 | 7 | 52 |
| Service, claims, healthcare | 21 | 32 | 3 | 4 | 60 |
| ERP, CRM, trust & estate | 20 | 22 | 1 | 5 | 48 |
| Grantmaking & regulatory | 20 | 15 | 2 | 5 | 42 |
| SaaS, IAM, records, workflow, other | 26 | 32 | 2 | 6 | 66 |
| Operating economics | 2 | 31 |  | 3 | 36 |
| Event architecture | 5 | 18 | 1 | 3 | 27 |
<!-- END GENERATED: coverage-by-pass -->

### By necessity (legal, fiduciary, contractual, operational, commercial or conventional)

<!-- BEGIN GENERATED: necessity -->
| Necessity | covered | partial | contradictory | missing |
|---|---|---|---|---|
| commercial | 12 | 29 | 2 | 7 |
| contractual | 3 | 2 |  | 2 |
| conventional | 13 | 3 | 1 |  |
| fiduciary |  | 2 |  |  |
| legal | 42 | 73 | 3 | 16 |
| operational | 62 | 79 | 4 | 13 |
<!-- END GENERATED: necessity -->

Most capabilities are partially covered. The baseline is *broad*: it names almost every capability that other industries need. It is *shallow* on the mechanics that make a capability testable: periods, payer sources, precedence rules, windows, deadlines and authority classes. That pattern matches the reconciliation's finding that 79% of canonical requirements lack an acceptance signal. Most of this mission's new requirements therefore sharpen a partial capability rather than add a new one. Each one names its parent capability and links to the partial requirement it sharpens.

## 4. How traceability is maintained

- `data/trace-edges.csv` is generated by `graph` (`tools/fs/Unbound.ReqAudit`) from the records. Never edit it by hand. It holds these edge types: `refines`/`supersedes` (RR → original), `derived_from` (requirement → finding, parent), `depends_on` (links), `affects` (finding → requirement), `duplicates`, `conflicts_with`, `tested_by` (→ scenario) and `decided_by` (→ open decision).
- Every new requirement has a `discovery_source` (industry plus specific source), an `industry_analogy`, a `gap_finding` (XIF) and `why_no_predecessor`. A new requirement is never linked to an unrelated original. Where no predecessor exists, provenance comes from the discovery record alone (mission §1).

<!-- BEGIN GENERATED: trace -->
| Relationship | Edges |
|---|---|
| depends_on | 757 |
| affects | 611 |
| derived_from | 454 |
| tested_by | 397 |
| decided_by | 184 |
| conflicts_with | 50 |
| refines | 29 |
| duplicates | 14 |
| supersedes | 3 |
<!-- END GENERATED: trace -->

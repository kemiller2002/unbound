# DAF Landscape → Unbound requirements conversion, wave 1
Date: 2026-11-21 | Status: candidate / unapproved | Source repository: kemiller2002/daf-landscape, main

## Source documents reviewed
- [Executive report](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/14-executive-conclusions/executive-report.md)
- [18-outcome model](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/01-enterprise-outcomes/outcome-model.md)
- [Operating model comparison](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/10-market-and-business-model/models-A-B-C-D-comparison.md)
- [Innovation prioritization](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/workflow-innovation/10-prioritization/prioritization.md)
- [Outstanding work](https://github.com/kemiller2002/daf-landscape/blob/main/research/ai-native-daf-enterprise/OUTSTANDING.md)

This is a **first conversion pass**, not an exhaustive review of the 1,900-entry repository tree. Evidence labels [V], [I], [S], [A] in source documents are preserved as claims of the source, not independently revalidated here. Every item below is a candidate requiring detailed acceptance cases and legal review where applicable.

## Business outcome requirements (source: outcome-model.md O-01..O-18)
- UNB-LS-001: Support eligible contributions and accurate acknowledgments, including distinct asset-settlement and gift-completion states (O-01).
- UNB-LS-002: Maintain identifiable donor-advised accounts and scoped, revocable advisory privileges (O-02).
- UNB-LS-003: Preserve sponsor ownership, legal control, and accountable distribution decisions (O-03).
- UNB-LS-004: Safeguard and invest charitable assets within approved policy and authority (O-04).
- UNB-LS-005: Maintain reconcilable books, independent evidence and corrections (O-05).
- UNB-LS-006: Resolve grant recommendations into eligible, approved, executed, rejected or escalated outcomes (O-06).
- UNB-LS-007: Satisfy applicable reporting, oversight, and regulatory requirements with verified jurisdictional scope (O-07, O-12).
- UNB-LS-008: Detect and control fraud, prohibited benefit and conflicts (O-08).
- UNB-LS-009: Provide accurate donor/advisor information and role-specific access (O-09).
- UNB-LS-010: Preserve independent governance and named human fiduciary accountability (O-10).
- UNB-LS-011: Govern vendors, custodians and counterparties with contractual rights and continuity plans (O-11).
- UNB-LS-012: Enable measurable philanthropic impact without conflating correlation with causality (O-13).
- UNB-LS-013: Demonstrate a sustainable, sensitivity-tested revenue and cost model (O-14).
- UNB-LS-014: Give recipients useful grant identity, remittance, reconciliation and support information (O-15).
- UNB-LS-015: Support succession, inactivity, transfers, record export and portability (O-16).
- UNB-LS-016: Protect confidential donor, advisor and grantee information (O-17).
- UNB-LS-017: Evaluate changes and continuously improve without weakening controls (O-18).

## Novel mechanism candidates: do not freeze these as mandatory solutions
- UNB-HYP-001: Continuous charity eligibility and diff-triggered rescreening may replace per-grant repetitive lookup. Measure false holds, missed changes, staleness and latency (INN-01/03/34).
- UNB-HYP-002: Sponsor-approved policy-as-code and policy compiler with back-testing may eliminate routine manual approval work; require safe rollout, rollback, rule versioning and independent approval (INN-05/21).
- UNB-HYP-003: Reusable, time-bounded determinations may reduce specialist effort; test invalidation, reuse factor and cross-sponsor legal/data rights (INN-04/02).
- UNB-HYP-004: A shared grant state machine and grant identifier carried through remittance may improve recipient reconciliation (INN-08).
- UNB-HYP-005: Continuous three-way reconciliation and event-ledger architecture may reduce break handling; test duplicated, missing, out-of-order and reversed events (INN-20/22/39).
- UNB-HYP-006: Advisor-embedded DAF interaction and narrowly scoped delegations may reduce donor friction; prove explicit consent, revocation, sponsor control and integration economics (INN-15/26).
- UNB-HYP-007: Sponsor-owned portable records and a documented transfer protocol may reduce lock-in; assess confidentiality, retention and interoperability (INN-17/18/44).
- UNB-HYP-008: Recipient-side status and shared verification services may improve nonprofit experience; test cold-start, fraud and network governance (INN-07/38).
- UNB-HYP-009: Donor-selected execution windows and spend-down horizons may improve operations and philanthropic outcomes; validate donor acceptance and avoid unauthorized delays or coercion (INN-37/19).
- UNB-HYP-010: Shared policy packs and specialist decision-right routing may improve scaling; test correlated errors, credentialing and segregation of duties (INN-36/47).

## Commercial model requirements, not premature commitments
- UNB-COM-001: Evaluate sponsor model A (traditional enhanced), B (AI-native sponsor), C (infrastructure provider), D (partner sponsor-of-record), and Dc (captive sponsor) using common economics, legal risk, capital, evidence time, distribution and reversibility criteria.
- UNB-COM-002: Preserve a capability architecture that can support C and D without making sponsor asset ownership a technical prerequisite.
- UNB-COM-003: Require real customer discovery, executed data rights and measured pilot outcomes before asserting that C→D is commercially superior.
- UNB-COM-004: Distinguish the **full synthetic sponsor reference enterprise** needed to demonstrate EF from the **commercial market-entry model**. The former does not decide the latter.

## Safety and control requirements
- UNB-CTL-001: Classify decisions as deterministic (D), probabilistic recommendation/flag (P), or human-authorized (H). P alone must not clear controls or move money.
- UNB-CTL-002: Donor/advisor/agent requests never become binding sponsor instructions without valid sponsor policy and authority.
- UNB-CTL-003: Every material capability has exactly one accountable human role even when its executor is automated.
- UNB-CTL-004: Cross-system business events must be idempotent, provenance-linked and correctable, with reconciliation before finality claims.
- UNB-CTL-005: Prohibit pay-first/verify-later, sampling that replaces legally necessary review, and model-only auto-clear approaches unless an independently reviewed lawful control design establishes otherwise.
- UNB-CTL-006: Model year-end surge, custodian outage, liquidity stress, vendor data restrictions and correlated AI failures.

## Outstanding research converted into explicit validation blockers
- UNB-LSGAP-001: Sponsor grant-history shadow replay to measure exception rates, human minutes and errors (source OUTSTANDING C1).
- UNB-LSGAP-002: Independent reconciliation break-frequency and handling-cost measurement (C2).
- UNB-LSGAP-003: Verify sponsor/vendor API access, export and usage rights in actual agreements (C3).
- UNB-LSGAP-004: Counsel review of instruction-only provider status and inter-sponsor determination reuse (C4).
- UNB-LSGAP-005: Customer willingness to pay, partner sponsor terms and donor acceptance of execution windows (C5).
- UNB-LSGAP-006: Verify restricted custodian/bank and identity-provider integrations rather than assuming availability (C6–C7).
- UNB-LSGAP-007: Charity identity, sanctions matching, fairness and tax-filing extraction evaluations (B5–B8).
- UNB-LSGAP-008: Resolve legal-to-control mapping, agent naming, workflow ID reconciliation and proposed event/API catalog decisions (A1–A6).
- UNB-LSGAP-009: Reproduce and extend adversarial experiments for poisoned data, adaptive fraud, correlated misses, reviewer automation bias and registry compromise (B3).

## Conversion method for next wave
For every source capability, workflow, API, agent, event, research conclusion and proposed innovation:
1. Link exact source path and source-local ID; preserve evidence confidence and dates.
2. Classify as outcome, legal constraint, operational control, design hypothesis, commercial option, research question, or rejected idea.
3. Map to existing UNB requirement; avoid duplicate IDs. Record conflicts rather than silently merging.
4. Add measurable acceptance criteria and normal/exception/adversarial tests.
5. Challenge the incumbent mechanism with at least three alternatives; never promote a design hypothesis to a mandatory implementation without decision evidence.
6. Track reviewed/unreviewed inventory and completion counts. Do not claim full conversion before every source artifact is reconciled.

## Immediate next sources
- 29-capability map and accountability matrix
- 24 workflow redesigns and enterprise integration journey
- API catalog, completeness matrix, agent catalog, authority boundaries
- Canonical data model and event catalog
- Regulatory applicability matrix and control framework
- Financial models and scenario tests
- Workflow innovation design catalog, adversarial verdicts and experiment results
- Charity-vetting AI evidence and evaluation results
- Commercial opportunity and market research workstreams

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### UNB-RCN-ENT-013 (P2, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-028 · **Parent:** UNB-HYP-007 · **Decisions:** —
- **Requirement:** An advisory-privilege holder MUST be able to export the fund's grant, contribution and statement history in a documented machine-readable format, limited to data they are authorized to see.
- **Acceptance criteria:**
  - AC-1: Given a co-advisor with a restricted view; when it exports; then the export excludes other participants' private contribution data.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-013); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

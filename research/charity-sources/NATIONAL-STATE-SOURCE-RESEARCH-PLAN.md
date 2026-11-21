# Nationwide state charity data source architecture and research matrix
Date: 2026-11-21 | Research stage, not implementation

## Scope
Six independent source families across 50 states and DC: **306 source-category slots**. This is a coverage and verification matrix, **not** 306 verified datasets. Indiana contains a few discovered official landing pages; every other jurisdiction remains to be researched at the specific-source level. No claim of bulk/API availability, licensing permission or registration applicability is made.

### Source families
- **CORP:** Secretary of State or equivalent corporate entity registration, legal standing, mergers, dissolution, annual reports, filing images, identifiers.
- **SOL:** Charitable solicitation registration, exemptions, professional fundraising firms/consultants, solicitation campaign disclosures. Do not assume charities register in every state.
- **ENF:** Attorney general oversight, charity fraud enforcement, charitable trust/institutional fund filings, disciplinary and court actions. Distinguish allegation, investigation, settlement and judgment.
- **FIN:** State-required financial statements, audits, annual reports, solicitation financial filings and program data. Distinguish IRS filings from state-specific disclosures.
- **AWD:** State grants, contracts, award recipients and payment data. Government awards are contextual evidence, not grant eligibility.
- **RES:** Optional academic, research, public nonprofit directories and third-party derived sources. Require original provenance and license analysis.

## Verification requirements
**NAT-001:** For each slot, record whether the category is legally applicable, whether a regulator exists, and whether a searchable/public dataset actually exists. These are separate questions.
**NAT-002:** Capture official agency, exact dataset endpoint, source owner, publication authority, public search, API, bulk, record-level export, historical depth, update cadence, schema, identity keys and coverage.
**NAT-003:** Record terms, licensing, automated access permissions, commercial-use rights, raw retention, derivative rights, redistribution, costs, rate limits, authentication and legal review.
**NAT-004:** Distinguish no registry exists, registry not mandatory, source not publicly accessible, source not found, and not yet researched. Never treat unverified as absent.
**NAT-005:** Preserve separate temporal and provenance data for state incorporation, tax exemption, charitable solicitation and sponsor-specific grant eligibility.
**NAT-006:** Use official state resources and agency documents as primary verification; NASCO directory is a discovery index, not proof of a source feed.
**NAT-007:** For each source, log researcher, checkedAt, evidence URL, page/document date, source version and confidence, with recheck interval.
**NAT-008:** Rank acquisition options: lawful official bulk/API, official open data, permitted record lookup, public records request, direct charity submissions, manual steward workflow.
**NAT-009:** Do not scrape portals against restrictions, infer rights from public access, or buy paid data without approval.
**NAT-010:** Establish per-state quality checks for nonprofit identity collision, fiscal sponsorship, chapter networks, names, EIN gaps and historical corporate changes.
**NAT-011:** Keep sponsor-private compliance determinations outside shared/public state facts and outside any future commercial data product.
**NAT-012:** Every acquired source must have an owner, freshness objective, exception route, schema-change monitor, and measurable coverage and cost.

## Research execution
1. **Pilot Indiana:** validate INBiz bulk schema/license/pricing, corporate records, AG professional fundraiser and trust oversight, STATS Indiana provenance; complete missing FIN and AWD slots.
2. **Pattern comparison:** research states with different regulatory models and publishing methods, beginning California, New York, Florida, Texas and Pennsylvania. This is a suggested sequence, not a claim about their data availability.
3. **All remaining states/DC:** assign each slot and verify official source links and access rights individually.
4. **Cross-source quality:** map every state field to canonical charity claim predicates, effective dates, source authority and rights.
5. **Acquisition decision:** prioritize by expected data value, geographic coverage, legal feasibility, cost, freshness, operational risk and incremental information gain.

## Progress metrics
Track (a) slots researched, (b) applicable source types, (c) official endpoint verified, (d) public records accessible, (e) bulk/API verified, (f) rights approved, (g) acquisition tested, (h) source in production, and (i) quality validated. Do not aggregate these into a misleading single percent-complete metric.

## Required outputs for each jurisdiction
Source registry with six families, legal applicability notes, acquisition feasibility, terms/rights matrix, fields and identifier mapping, source freshness, ingestion test plan, ambiguity cases and source-specific ADRs.

## Links
NASCO state survey discovery: https://www.nasconet.org/survey-map
Indiana pilot: https://github.com/kemiller2002/unbound/blob/main/research/charity-sources/INDIANA-SOURCE-PILOT.md
Canonical model: https://github.com/kemiller2002/unbound/blob/main/architecture/CHARITY-KNOWLEDGE-MODEL-AND-ACQUISITION-PIPELINE.md

No ingestion, API validation, paid purchase, source license approval or implementation is claimed.

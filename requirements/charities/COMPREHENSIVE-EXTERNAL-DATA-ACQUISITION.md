# Comprehensive external charity intelligence acquisition
Date: 2026-11-21 | Status: requirements and research inventory, no ingestion implemented

## Binding acquisition directive
**UNB-DATA-DEC-001:** Acquire and internally retain the broadest feasible, lawful, useful range of external charity information, not merely data required for current eligibility decisions. Unbound owns the canonical charity registry, ingestion processes, normalization, historical record, source attribution and internal eligibility judgments. No Candid/GuideStar or comparable commercial charity-data aggregator is a required source, platform dependency or decisioning authority. Prioritize primary sources and direct collection. Public availability does not by itself establish scraping, redistribution, retention, commercial use or automated access rights.

## External source families to inventory
- **UNB-EXT-001:** IRS tax-exempt organization datasets: EO Business Master File, determination letters where available, Pub 78, automatic revocations, Form 990/990-EZ/990-PF/990-N data, machine-readable filings, group exemptions and status histories, with documented gaps.
- **UNB-EXT-002:** State charity solicitation registration, secretary of state corporate status, attorney general charity enforcement, nonprofit annual reports and relevant state licensing, with per-state coverage maps.
- **UNB-EXT-003:** Federal and state grants, contracts, awards and spending sources (e.g., USAspending.gov, SAM.gov where relevant), distinguishing nonprofit recipient identity from legal grant eligibility.
- **UNB-EXT-004:** OFAC and other legally applicable sanctions/debarment/exclusion lists, with source freshness and exact legal screening applicability; do not equate name similarity with a sanctions match.
- **UNB-EXT-005:** Federal/state court dockets, bankruptcy and public enforcement actions where lawfully accessible, distinguishing allegation, pending matter, disposition and appeal.
- **UNB-EXT-006:** Charity official websites, annual reports, audited statements, program descriptions, leadership, contact details, service areas, public grant reports and impact claims, subject to website terms and robots/access constraints.
- **UNB-EXT-007:** Direct charity submissions and attestations, uploaded financials, identity documents and grant-use reporting, treated as claims pending verification.
- **UNB-EXT-008:** Foreign government charity registers, international NGO accreditation and cross-border compliance sources, with jurisdiction, language, reliability, permitted use and coverage.
- **UNB-EXT-009:** Public academic studies, reputable journalism, independent watchdog research and community reports, categorized as secondary evidence rather than authoritative status.
- **UNB-EXT-010:** Geospatial and demographic context, disaster areas, census/community needs and service coverage where relevant to philanthropic strategy; avoid implying an organization serves an area based solely on its registered address.
- **UNB-EXT-011:** Public tax filings and nonprofit financials, including revenue, expense, assets, liabilities, compensation, fundraising, program service categories, grants and multi-year trends, with accounting definitions and filing year.
- **UNB-EXT-012:** Entity network evidence: affiliates, parent/subsidiary, fiscal sponsorship, shared leadership, mergers, chapter networks, grantee/recipient networks and government award relationships; distinguish observed link from inferred link.
- **UNB-EXT-013:** Historical snapshots, archives and previously acquired versions where lawful, preserving changes over time rather than only latest records.
- **UNB-EXT-014:** Accessibility and public service availability signals from official sources and direct organization claims, with validation and age limits.
- **UNB-EXT-015:** Public adverse events, disciplinary actions and verified fraud notices with careful attribution, due process, confidence and anti-defamation safeguards.
- **UNB-EXT-016:** Third-party datasets may be evaluated opportunistically only when licensed and useful; no commercial aggregator may become mandatory, and provenance/rights must be retained per field.

## Ingestion, rights and provenance
- **UNB-EXT-017:** Maintain a machine-readable SourceRegistry with publisher, URL/API/feed, dataset, jurisdiction, schema, license, terms, robots/access restrictions, permitted commercial use, permitted storage, permitted redistribution, PII handling, retention, cadence, coverage, cost, credentials and contact.
- **UNB-EXT-018:** Require legal/contractual access review before automated collection; prohibit bypassing authentication, rate limits, paywalls, anti-bot restrictions or other access controls.
- **UNB-EXT-019:** Use approved bulk feeds, open data APIs and official downloads before web scraping; support incremental sync, full snapshots, change feeds and direct submissions.
- **UNB-EXT-020:** Preserve source payload, retrieval and source-effective timestamps, checksum, parser version, license, claim lineage and transformation errors when permitted; otherwise preserve lawful derived metadata with restrictions.
- **UNB-EXT-021:** Track per-source freshness, completeness, coverage, outages, field definitions, schema changes and known data quality defects.
- **UNB-EXT-022:** Maintain raw observation -> normalized claim -> identity resolution -> verified fact -> sponsor-specific decision as separate states.
- **UNB-EXT-023:** Use deterministic exact identifier matching first; probabilistic entity matching generates reviewable candidates, not irreversible merges.
- **UNB-EXT-024:** Handle duplicates, conflicting identifiers, stale information, false positives, conflicting dates, foreign-language transliteration and merged/dissolved entities with evidence and human stewardship.
- **UNB-EXT-025:** Source-specific legal restrictions flow through to downstream search, AI retrieval, APIs, Folio reports, exports and commercial licensing.
- **UNB-EXT-026:** Do not ingest unnecessary personal data; apply field-level privacy classification, access controls, deletion/retention review and legal hold constraints.
- **UNB-EXT-027:** Maintain append-only observations and linked corrections; rebuild normalized projections and retain historical eligibility snapshots.
- **UNB-EXT-028:** Implement monitoring for source updates, status changes, leadership changes, financial filing changes, enforcement events and entity relationships, with severity and review routing.
- **UNB-EXT-029:** Track source-level acquisition costs, compute, storage, human review, licensing and business value; broad acquisition must remain economically sustainable.
- **UNB-EXT-030:** Record evidence confidence and source authority separately; an AI summary, journalist allegation, charity self-report and IRS record cannot be treated as equivalent.
- **UNB-EXT-031:** Support independent verification of high-impact claims and a dispute/correction channel for charities and sponsors.
- **UNB-EXT-032:** Define ingestion APIs, job checkpoints, retries, backpressure, deduplication, versioning, tenant scoping, quarantine and safe failure without contaminating approved eligibility.
- **UNB-EXT-033:** Keep internally generated sponsor-private assessments separate from lawful common-source data; no cross-partner sharing without authority.
- **UNB-EXT-034:** Maintain a searchable, evidence-linked historical knowledge graph and source lineage usable by deterministic eligibility policies and bounded AI assistance.
- **UNB-EXT-035:** Any future external charity-data API product requires explicit licensing, provenance, confidentiality, security, quality and commercial review; no assumption all collected data can be resold.
- **UNB-EXT-036:** Support an offline or source-outage degraded mode using last-known data clearly marked as such; freshness-sensitive approvals may be held.
- **UNB-EXT-037:** Each data source requires a measured ingestion/quality acceptance gate before being treated as production authoritative evidence.
- **UNB-EXT-038:** Source registry and coverage map must be exportable, reviewable and versioned for audit.

## Priority waves
**Wave 1: identity and eligibility.** IRS core datasets, state registrations, corporate status, relevant sanctions and primary official charity records.
**Wave 2: financial and operational intelligence.** IRS filings, audited financials, annual reports, government awards and charity submissions.
**Wave 3: entity relationships and longitudinal change.** Affiliations, fiscal sponsorship, historic filings, leadership, mergers, dissolution and program changes.
**Wave 4: contextual and secondary intelligence.** Court/enforcement, public journalism, research, geography, community needs, outcomes and international sources.
Waves are sequencing preferences, not limits on eventual acquisition.

## Research tasks before implementation
1. Build a jurisdiction-by-jurisdiction primary source catalog, actual dataset endpoints, access method, license/terms, historical depth, refresh rate and cost. Verify, do not assume.
2. Produce source coverage matrix for IRS, all 50 states plus DC, federal sanctions/awards and prioritized international jurisdictions.
3. Benchmark identity matching on common-name collisions, EIN reuse/errors, fiscal sponsorship, chapters, foreign entities, merged charities and conflicting filings.
4. Define extraction, claim schema, source reliability and contradiction policies.
5. Define charity-facing corrections and sponsor stewardship workload model.
6. Model acquisition cost, quality improvement, grant decision efficiency and optional commercial API economics.

## Acceptance tests
EXT-T01: Official IRS source contradicts charity website; preserve both, show authority and initiate review.
EXT-T02: Source has restrictive redistribution terms; internal use and exports respect rights.
EXT-T03: Two nonprofits share names; identifiers prevent false merge.
EXT-T04: Charity has no EIN; identity and eligibility remain reviewable.
EXT-T05: Feed unavailable; freshness and approval state are accurate.
EXT-T06: A 990 filing is amended; both versions and corrected financial metrics remain reproducible.
EXT-T07: Sanctions name match is a false positive; evidence and review prevent automatic adverse decision.
EXT-T08: News allegation is not promoted into a legal finding or automatic exclusion.
EXT-T09: Partner-private investigation does not leak into common registry or external API.
EXT-T10: Source parser schema changes; quarantine protects existing verified records.
EXT-T11: Historical charity name and merger chain correctly link to prior grants.
EXT-T12: A prohibited scraping target is excluded pending lawful access.
EXT-T13: Charity disputes a claim; case and source evidence persist, corrected outputs are versioned.
EXT-T14: AI cannot create source evidence or silently change eligibility.
EXT-T15: Cross-jurisdiction source dates and currencies are retained without false normalization.
EXT-T16: 10 million source observations replay deterministically within an architecture-defined performance budget (target to be set through testing).

## Architectural boundary
This is first-party charity intelligence, not a mandate to indiscriminately collect every available personal record. Maximize useful evidence coverage while honoring privacy, lawful access, rights, accuracy, cost and sponsor authority. No acquisition, source verification, legal review, commercial license or implementation is claimed.

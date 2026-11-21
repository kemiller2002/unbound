# Verified federal source discovery and 51-jurisdiction state research queue
Date: 2026-11-21

## Verification boundaries
Official source landing pages were inspected through public web research. **Verified official page** means existence and advertised access format were corroborated, not that a bulk download succeeded, that API entitlements were tested, or that licensing was approved. No external datasets were ingested. Every state row remains **unverified** for regulator-specific dataset endpoints, bulk/API access, historical depth, frequency and terms.

## Federal source inventory
- **IRS-01 Pub 78 eligibility data** (IRS): bulk downloadable. Not exhaustive for all eligible recipients; verify rights and current refresh. https://www.irs.gov/charities-non-profits/tax-exempt-organization-search-bulk-data-downloads
- **IRS-02 Automatic revocation** (IRS): bulk downloadable. Presence does not establish current revocation; check reinstatement. https://www.irs.gov/charities-non-profits/tax-exempt-organization-search-bulk-data-downloads
- **IRS-03 Form 990-N** (IRS): bulk downloadable. Limited information compared with full 990. https://www.irs.gov/charities-non-profits/tax-exempt-organization-search-bulk-data-downloads
- **IRS-04 Form 990 series XML** (IRS): year/month ZIP XML. E-filed subset; filing year distinct from tax year; amended returns. https://www.irs.gov/charities-non-profits/form-990-series-downloads
- **IRS-05 EO Business Master File extract** (IRS): state/region CSV. Determination records; not sole current eligibility decision. https://www.irs.gov/charities-non-profits/public-disclosure-datasets-and-downloads
- **IRS-06 Determination letters and exemption approvals** (IRS): search/download varies. Letters available from 2014 onward in TEOS; bulk access needs checking. https://www.irs.gov/charities-non-profits/tax-exempt-organization-search
- **FED-01 SDN and consolidated non-SDN sanctions** (Treasury OFAC): downloadable lists. Match review required; legal applicability and update timing. https://ofac.treasury.gov/sanctions-list-service
- **FED-02 entity and exclusion extracts** (GSA SAM.gov): download/API advertised. Credentials, entitlements and permissible use to verify. https://sam.gov/entity-information
- **FED-03 federal awards and recipient data** (USAspending): API and bulk download. Award recipient is not necessarily charitable entity; identity matching. https://api.usaspending.gov/docs/endpoints
- **STATE-DISC state charity regulator contacts and registration survey** (NASCO): directory/survey. Not a 51-jurisdiction bulk data source. https://www.nasconet.org/survey-map

## Critical distinctions
1. Pub 78 is not exhaustive of eligible donees. Absence cannot be a universal denial.
2. Automatic revocation history is not equivalent to current exemption status. Reinstatement and effective dates matter.
3. XML 990 filings represent available e-file records, not a complete historical record of every organization.
4. Sanctions fuzzy matches are candidates for human/legal evaluation, not automatic confirmed matches.
5. Government-award recipient identity is not equivalent to sponsor-approved DAF grant eligibility.
6. NASCO is a useful state regulator discovery index, not evidence of standardized public bulk feeds across jurisdictions.
7. Public availability is not proof of unrestricted scraping, commercial use, retention, redistribution or AI training rights.

## State research protocol
For each of 50 states and DC, identify separate charity solicitation regulator, corporation registry, charity-specific filings, enforcement notices, data download, search portal, API, licensing, terms, historical coverage, update frequency and lawful acquisition method. Record a precise official URL and retrieval evidence for each verified attribute. Research a second source for entity corporate standing where distinct. Never mark an entire state verified because a NASCO contact exists.

## Prioritization
Wave A: IRS, OFAC, SAM, USAspending, then Indiana as a state-specific pilot. Wave B: high-volume states and reusable state patterns. Wave C: remaining states/DC, nonstandard portals and manual stewardship. Expand later to territories and international sources. Priority is not a claim of completeness.

## Implementation gates
No connector before source rights/access review, parser schema validation, idempotent snapshot/checkpoint design, source freshness SLA, entity-match test cases and sponsor policy controls. Maintain original source, as-of time and recorded time, and do not overwrite historical claims. The first source-inventory CSV and state-queue CSV are research planning assets only.

## Source citations
IRS disclosure: https://www.irs.gov/charities-non-profits/public-disclosure-datasets-and-downloads
IRS 990 XML: https://www.irs.gov/charities-non-profits/form-990-series-downloads
IRS status caveats: https://www.irs.gov/charities-non-profits/search-for-tax-exempt-organizations
NASCO 2026 survey: https://www.nasconet.org/survey-map
OFAC: https://ofac.treasury.gov/sanctions-list-service
SAM: https://sam.gov/entity-information
USAspending API: https://api.usaspending.gov/docs/endpoints

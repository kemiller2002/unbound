# Five-state charity source comparison: California, New York, Florida, Texas, Pennsylvania
Research date: 2026-11-21. Source landing pages and published capabilities reviewed. **No data downloaded, connector built, bulk rights confirmed or legal applicability opinion issued.**

## Findings
### California
AG Registry of Charities and Fundraisers provides public filings and registration documents. Its Registry Reports page advertises a downloadable CSV of organizations that may not operate or solicit for charitable purposes, refreshed on the first and third Wednesday of each month. The official registry search is more current and controls if it differs from the CSV. Source: https://oag.ca.gov/charities/reports and https://www.oag.ca.gov/charities

### New York
AG Charities Bureau searchable registry includes registration, annual financial reports, approved transactions (including dissolutions/asset dispositions), and professional fundraiser reports. Registry financial reports from the last five years are available after review; older records require a FOIL request. Certain religious organizations are exempt and may not appear. Sources: https://charities-search.ag.ny.gov/RegistrySearch and https://charities-search.ag.ny.gov/

### Florida
FDACS Check-A-Charity publishes registration and financial information. State solicitation rules include exemptions and provisions for small charities; public registration absence must be interpreted with the applicable rule and charity facts. No bulk/API feed verified. Sources: https://www.fdacs.gov/business-services/solicitation-of-contributions and https://csapp.fdacs.gov/CSPublicApp/CheckACharity/CheckACharity.aspx

### Texas
Texas AG states most charities and nonprofits are not required to register with the state. Registration obligations are limited to certain law-enforcement telephone solicitation, public safety and veterans organization categories. Corporate formation and IRS exemption are separate. Source: https://www.texasattorneygeneral.gov/divisions/charitable-trusts/registration-and-filings

### Pennsylvania
PA DOS Bureau of Corporations and Charitable Organizations maintains charity registration/financial information and business entity records. The corporate search does not index by federal ID/EIN, making cross-source identity resolution important. Source: https://www.pa.gov/agencies/dos/programs/charities and https://www.pa.gov/agencies/dos/programs/business/information-services/record-searches

## Cross-state requirements
- **FIVE-001:** Maintain jurisdiction-specific registration obligation logic and exemption categories, never a universal missing-registration rejection rule.
- **FIVE-002:** Distinguish official status snapshot from authoritative current search; attach effective/recorded timestamps.
- **FIVE-003:** Treat historical records available only by public-records request as separate acquisition channels with cost, lead time and rights.
- **FIVE-004:** Track different record classes: charity filings, fundraiser filings, corporate records, AG-approved transactions and enforcement.
- **FIVE-005:** Support state-specific field matching where EIN is absent or not indexed.
- **FIVE-006:** Store statutory interpretation and source access as versioned, separately verified claims, subject to counsel review.
- **FIVE-007:** Prefer official downloadable CSV/API/bulk where available, but do not assume a public search page permits scraping.
- **FIVE-008:** Maintain historical status changes, amendments, corrections and source priority policies per jurisdiction.
- **FIVE-009:** State charity regulatory standing is distinct from IRS eligibility and sponsor-specific grant approval.
- **FIVE-010:** Track data rights, automated access, reuse, freshness and commercial API rights separately for each dataset.

## Remaining research
For each of these five states: identify the corporation registry URL and data rights; financial reports; enforcement public records; government awards; optional academic datasets; dataset schemas; bulk/API offerings; historical depth; update cadence; terms and commercial rights. These are source leads, not completed six-category coverage. Repeat with remaining 45 states and DC.

## Immediate acquisition candidates
California adverse-status CSV: request/sample download and validate actual schema, current dates, license and parser behavior before ingestion.
New York registry financials: determine permissible bulk or FOIL acquisition of older records.
Florida: confirm whether FDACS offers authorized exports.
Texas: model categorical registration rather than false missing-registration flags.
Pennsylvania: investigate data access and identity matching without indexed EIN.

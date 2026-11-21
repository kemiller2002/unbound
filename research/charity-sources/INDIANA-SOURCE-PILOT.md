# Indiana charity data acquisition pilot
Date: 2026-11-21 | Status: researched source discovery; no dataset downloaded or rights approved

## Key regulatory finding
Indiana's Attorney General states that charitable organizations soliciting through their own bona fide employees or volunteers generally do not register their fundraising campaigns with the AG. Registration applies to professional fundraising consultants/solicitors. Therefore **absence from an Indiana AG charity registration database must never be treated as disqualifying**. Distinguish corporate standing, federal tax status, fundraiser registration, trust oversight and grant eligibility.

## Verified source leads
| ID | Authority | Data | Access | Official source | Verification and open questions |
|---|---|---|---|---|---|
| IN-01 | Indiana Secretary of State / INBiz | Business entity legal names, entity type, status, IDs and filings | Public entity search | https://inbiz.in.gov/public-search | Search page confirmed; query limits, automation terms and full historical depth unverified |
| IN-02 | Indiana Secretary of State / INBiz | Bulk business entity records and monthly updates | Paid download, account required | https://inbiz.in.gov/inbiz/bulkdataservices/index | State page advertises bulk data and monthly updates; lists $9,500 bulk and $500 per monthly file; verify current prices, license, historical content and fields before purchase |
| IN-03 | Indiana Secretary of State | Certificates of existence, document copies, historical due-and-diligent searches | Document request | https://inbiz.in.gov/business-filings/information-requests | Historical microfilm/archive search mentioned; pricing and machine-readable access unverified |
| IN-04 | Indiana AG Consumer Protection | Professional fundraiser consultant and solicitor registration | Registration portal and oversight | https://secure.in.gov/attorneygeneral/consumer-protection-division/charities-and-donors/charitable-fundraising/ | **Not** a general charity registry; public dataset/export and history unverified |
| IN-05 | Indiana AG | Charitable trusts and institutional funds, modifications, court matters | Submission portal and official information | https://www.in.gov/attorneygeneral/consumer-protection-division/charities-and-donors/charitable-trusts-and-institutional-funds/ | Oversight workflow, not universal charity eligibility register; public bulk records unverified |
| IN-06 | STATS Indiana / Indiana Business Research Center | Nonprofit organization listing, name/address/source | Public research database | https://www.stats.indiana.edu/about/nonprofits.asp | States underlying sources include IRS, SOS and IBRC subscription directories; assess redistribution rights and underlying source provenance |
| IN-07 | IRS | Federal tax status, filings, exemption/revocation | Official IRS bulk datasets | https://www.irs.gov/charities-non-profits/tax-exempt-organization-search-bulk-data-downloads | Separate from state standing; verify current status and exceptions |

## Architecture implications
- **IN-REQ-001:** Distinguish nonprofit incorporation/standing from federal exemption and grant eligibility.
- **IN-REQ-002:** Do not infer lack of charity eligibility from missing Indiana AG registration.
- **IN-REQ-003:** Maintain separate records for fundraiser/solicitor registration and its relationship to charities.
- **IN-REQ-004:** Preserve original SOS business ID, effective-dated corporate status and document lineage.
- **IN-REQ-005:** Evaluate paid SOS bulk acquisition against public search, frequency, license, costs and coverage; no purchase without approval.
- **IN-REQ-006:** Treat STATS Indiana as a discovery/triangulation source; verify subscription-derived source rights before storing/redistributing its content.
- **IN-REQ-007:** Join Indiana corporate entity data to IRS EIN and exemption data only through reviewable identity resolution, not name-only merge.
- **IN-REQ-008:** Record verified access method, license, retention, cost, history, cadence, machine-readability and source terms separately for every source.

## Next verification experiments
1. Obtain current official bulk data sample/schema and licensing terms without buying; assess whether $9,500+ update pricing remains current.
2. Inspect INBiz search field coverage, record history, document retrieval and automation permissions.
3. Inspect STATS Indiana actual download interface and data reuse terms.
4. Determine whether Indiana AG professional fundraiser registration lists and enforcement actions are machine-readable.
5. Create a small consented/public-record gold-standard matching fixture across IRS, SOS and STATS without importing restricted data.
6. Calculate acquisition options: licensed bulk, permitted incremental lookup, state public records request and primary IRS feeds. Record legal and operational tradeoffs.

## Generalization for 51-jurisdiction inventory
Each jurisdiction should have separate source records for (a) corporate registry, (b) charity solicitation regulator, (c) enforcement and trust oversight, (d) financial/annual filings, (e) government grants and (f) optional research datasets. A missing source category is not evidence of missing charity status.

## Source references
https://secure.in.gov/attorneygeneral/consumer-protection-division/charities-and-donors/charitable-fundraising/
https://www.in.gov/attorneygeneral/consumer-protection-division/charities-and-donors/charitable-giving/
https://inbiz.in.gov/public-search
https://inbiz.in.gov/inbiz/bulkdataservices/index
https://inbiz.in.gov/business-filings/information-requests
https://www.stats.indiana.edu/about/nonprofits.asp

No dataset ingested, no bulk service purchased, no connector built, and no legal/contractual approval claimed.

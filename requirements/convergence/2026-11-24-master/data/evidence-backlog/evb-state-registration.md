# EVB: State registration exposure for a software-only DAF platform (CNV-EVB-002)

Accessed 2026-11-24. Data: `evb-state-registration.jsonl` (51 rows). Interpretation, not legal advice.

## Bottom line

1. **Platform-specific laws exist in two states only: California and Hawaii.**
   - **California:** Gov. Code §12599.9, in force since 2023. Its implementing regulations, 11 CCR §§314-322, are in force from 2025.
   - **Hawaii:** HRS §467B-2.3, effective 2026-07-01. Registration with the Attorney General costs $250 and renews every year.
   - **What they cover:** Both statutes expressly reach a "customizable" website/SaaS platform that charities use to solicit or receive donations. They also reach platforms that list charities by name.
   - **Exclusions:** Both exclude a DAF sponsor, but only the sponsor itself. Both exclude a vendor that "solely provides technical or supportive services" (hosting, development, payment processing) to a charitable fundraising platform (CFP). That wording does not clearly protect a white-label operator.
   - **No other state has a similar law.** No third state's law was found. Secondary sources dated 2025-07 and 2026-08 confirm only CA and HI.
   - **Open point:** CA AB 2221 (signed 2026) moves good-standing deadlines to 2028-01-01; bill text not read.
2. **Commercial co-venturer (CCV) registration does not fit Unbound.** It applies in AL, CA (conditional), HI, MA, MS and SC, and Illinois handles CCVs informally. A CCV is a business that advertises that buying its goods or services benefits a charity. Unbound sells software, so this applies only if Unbound runs cause-marketing ("x% of our fee goes to charity").
3. **Professional solicitor / fundraiser registration does not fit Unbound.** It exists in about 41 jurisdictions. These definitions turn on two things: requesting contributions for compensation, or receiving or controlling the funds. Unbound does neither, so it is outside them. Example:
   - **Washington (RCW 19.09.020):** a commercial fund-raiser is one who "solicits or receives contributions".
4. **Fundraising counsel / consultant registration is the main residual risk.** It exists in about 25 jurisdictions.
   - **What counsel is:** typical definitions cover a person paid to "plan, manage, advise, consult or prepare material" for solicitation, without soliciting or holding funds.
   - **Why it matters for Unbound:** a flat fee and no custody move a vendor *into* this category, not out of it.
     - In FL, NC and VA, a fixed fee is part of the counsel definition.
     - In OH and CO, counsel registers only if it has custody of contributions.
     - New York's definition includes anyone who "assists" with solicitation. That is the broadest wording found, so I rate NY "uncertain".
     - Georgia folds advisers into "paid solicitor". I did not read the text, so GA is also "uncertain".
5. **States with no registration categories:** DE, ID, MT, NE and WY have none. AZ repealed most of its regime in 2013; sources disagree on whether contracted fundraisers still register there. TX limits registration to public-safety and veterans solicitation.

## Verification status

**Read in primary text:**
- CA §§12599 and 12599.2, through a mirror. §12599.9 was read earlier as EV-D-027.
- HI §§467B-1 and 467B-2.3.
- NY Exec. Law §§171-a and 173-a.
- FL §496.404, OH R.C. 1716.01, NC §131F-2, VA §57-48, WA RCW 19.09.020, MI MCL 400.272, MA c.68 §18, AZ §44-6551, TX Occ. Code §1803.051.
- IL Attorney General rules: a $10,000 bond if the fundraiser controls funds.

**Not fetched (403/TLS blocks or unretrievable):** leginfo, oag.ca.gov, ilga.gov, Justia, GA/TN/NJ regulator sites, PA Act 202 text.
- **Search snippets only:** PA (DOS guidance: $25,000 solicitor bond), GA ($10,000 bond only with custody), TN, NJ and MD ($25,000 solicitor bond).

**Rows built from the chart:** the other 35 rows rely on the Perlman & Perlman 2025 chart, a secondary source that I did fetch. Their statute citations come from general knowledge and are marked "not verified". SD has no citation.

## Recommended registration plan

1. **California: register now for white-label/embedded or D2C deployments.**
   - Register Unbound with the Attorney General's Registry as a CFP before enabling those deployments for California users. They are type E (customizable SaaS) or type A (listing charities by name).
   - Then follow §12599.9 duties: the annual report, good-standing checks, disclosures, and charity consent.
   - Watch the AB 2221 effective dates in 2027-2028.
   - Sponsor-direct deployments may rely on the sponsor's own-platform exclusion (moderate confidence), as XID-SAS-005 found. Keep the evidence: sponsor ownership, branding, and domain records.
2. **Hawaii: register before serving Hawaii users through any CFP-type model.** The law has been in effect since 2026-07-01. Track HB 992 (2026), which may revise registration and fees.
   - Hawaii requires written consent from every recipient charity, with no exception. So in Hawaii, restrict charity listing and recommendation to advisory-privileged users of a sponsor deployment. Otherwise, build a consent workflow.
3. **No registration as professional solicitor or CCV anywhere,** provided the design constraints below hold. Record this as a decision with evidence ids.
4. **Fundraising-counsel opinion as a single conditional item.**
   - Before Unbound offers authored solicitation content, campaign management, or fundraising advice, get one multistate counsel memo. Prioritize NY, PA, FL, NC, VA, MA, MD, NJ, GA and IL.
   - If that memo concludes coverage, registering as counsel is a ministerial filing with no bond in most states. The NY, FL, NC, PA and MA definitions require no custody.
5. **Re-check every six months.** Watch for new state CFP bills, since NASCO and the states are watching CA/HI. Also verify the "not verified" rows against primary text before any external reliance.

## Design choices that keep Unbound outside the categories

**Brand and content**
- **Sponsor-branded, sponsor-owned experience.** The sponsor is the soliciting charity on sponsor domains, and Unbound's contract is with the sponsor. This supports the CA/HI "charity's own platform" and technical-vendor exclusions. It also means Unbound is not a person who "solicits".
- **No Unbound-branded solicitation.** Unbound makes no "Give via Unbound" appeals, sends no Unbound emails asking for gifts, and runs no Unbound D2C app listing charities. Solicitation content is authored and approved by the sponsor. Unbound supplies neutral templates only, and the sponsor edits and adopts them. This reduces the counsel risk where definitions include "prepares material", "advises" or "assists".
- **No fundraising strategy or advisory services.** Unbound does not plan, manage or run campaigns. Professional services are limited to technical configuration.

**Fees**
- **Flat SaaS fee with no per-gift or percentage compensation.** This avoids percentage-based definitions, such as HI and NY, where counsel paid on contributions is deemed a solicitor. It also keeps the broker-dealer conclusion in XID-REG-002.
- **No cause-marketing claims about Unbound's own fees.** This avoids CCV status.

**Funds**
- **Never touch funds.** Use sponsor merchant accounts and the sponsor's processor or custodian, and Unbound never acts as payee. This avoids "receives or controls contributions", custody-triggered counsel registration (OH, CO, MA), and bonding.

**Charity directory and consent**
- **Gate the charity directory.** Show charity search and recommendations only to authenticated advisory-privileged DAF holders, inside the sponsor's platform. This keeps the deployment within the DAF-sponsor framing in CA and HI and away from type A listing for the general public.
- **Use charities' names only with sponsor-side consent records in Hawaii.**

**Record-keeping**
- **Keep a per-deployment classification record.** It should cover branding owner, domain, who authored the content, fee basis, and funds flow.

## Key gaps

CA AB 2221 text; HI AG forms/rules and HB 992 status; full PA, GA, NJ, TN and IL definitions; regulator guidance on SaaS vendors as counsel (none found).

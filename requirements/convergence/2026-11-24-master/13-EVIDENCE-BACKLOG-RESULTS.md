# 13 — Evidence-backlog results (public research)

Two CNV-EVB-002 items that public sources can answer were researched after the mission report. Data and full summaries:
[`data/evidence-backlog/`](data/evidence-backlog/). Research is not authorization. The other backlog items need a
design partner or measurement (report 12) and remain open.

## 1. State platform and fundraising registration (50 states + DC)

| Result | Evidence status | Requirement change |
|---|---|---|
| Only **California** (Gov. Code §12599.9) and **Hawaii** (HRS §467B-2.3, effective 1 July 2026, $250 a year) have rules aimed at fundraising platforms. Both expressly cover customizable SaaS and white-label platforms, and their DAF-sponsor exclusions protect the sponsor, not Unbound. | Hawaii text re-fetched and verified by the integrating agent (2026-11-24); California from batch D. 15 statutes plus Illinois rules read in primary text; 5 states from search snippets; 35 rows from a secondary 2025 chart marked "not verified". | CNV-LEG-002 amended: register in both before enabling white-label, embedded or own-brand products there; re-verify the table annually. |
| Commercial co-venturer and professional-solicitor regimes do not reach a flat-fee platform that never solicits and never touches funds (47 states "no"; New York and Georgia "uncertain"). | Interpretation | None beyond existing design rules (CNV-LEG-001). |
| Fundraising-counsel registration (about 25 jurisdictions) applies if Unbound writes appeals or advises on fundraising; a flat fee does not avoid it. | Interpretation | CNV-LEG-002 amended: Unbound authors no appeals and gives no fundraising advice unless each affected jurisdiction's requirements are first determined from statute text and registrations made. |

Uncertain items: California AB 2221 (signed 2026, moves some deadlines to 2028) was seen only in a secondary summary;
New York and Georgia coverage. These stay on the backlog for statute-text verification.

## 2. Incumbent software prices

Form 990 Part VII-B contractor payments divided by the same return's Schedule D DAF count (amounts are floors; only the
five largest contractors over $100k are listed). The integrating agent independently re-parsed one filing (National
Christian Foundation, TY2024: Stellar Technology Solutions $2,495,829 ÷ 31,943 DAFs = **$78.13 per DAF-year**), matching the row.

| Tier | $ per DAF-year (or as stated) |
|---|---|
| Ren/Stellar software at national and faith sponsors (Thrivent, T. Rowe, Ayco, NCF, ImpactAssets, Goldman Sachs) | $32–$129 (median about $70); one at $226 |
| Community-foundation platforms (Foundant), all funds | about $100 per fund-year |
| Community-foundation platforms, per DAF | $195–$603 |
| Fully outsourced administration (Renaissance Administration) | $413–$745 |
| Daffy for Work | $36 per employee-fund-year |

**Consequence (report 05 EV-01 sharpened).** At 100k platform funds Unbound's break-even (≈$81) sits *inside* the
incumbent software band for large national sponsors, not below it; at 50k (≈$131) it is above every large-national data
point. Unbound cannot win large nationals on price at cost recovery; it can on price only against mid-size nationals,
community foundations (per fund, not per DAF) and buyers of fully outsourced administration — and otherwise must sell on
value (less staff time, fewer exceptions, evidence), which the shadow replay must demonstrate (CNV-ECO-003). This also
partly re-opens the community-foundation question on *price* grounds (they pay $195–$603 per DAF), though not on the
workflow grounds of report 05 EV-04; any community-foundation offer must price per fund and cover non-DAF fund types
(CNV-ECO-004).

## 3. Grant-payment rail coverage

Open networked rails (PayPal Grant Payments, Chariot) reach an estimated **15–25% of US public charities by count but
55–75% of DAF grant dollars** (inference, low confidence; nothing public measures it directly). Fidelity's enrolled base is
closed. **Consequence:** the release-1 default (OD-M-03) holds, with a mandatory check fallback and a stored verified
destination record (CNV-PAY-001 amended); adapter priority Chariot, then PayPal; electronic-share projections should start
low and be weighted by dollars.

## 4. Backlog status after this report

| Item | Status |
|---|---|
| Incumbent price points | Done (public floors); contract terms still unknown |
| Grantee rail enrolment share | Estimated (low confidence) |
| 50-state platform registration survey | Done for platform-specific rules; 35 rows need primary verification |
| Shadow replay, time study, December data, feed content, capacity, willingness to pay, sales cycles, mobile and PDF/UA measurement, benchmark | Open — require a design partner or measurement (report 12) |

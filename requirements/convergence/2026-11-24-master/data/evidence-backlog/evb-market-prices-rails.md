# CNV-EVB-002 evidence: incumbent prices and grant-payment rail coverage

Prepared 2026-11-24. Data: `evb-market-prices-rails.jsonl` in this folder (42 rows: 27 price, 15 rail). I used public sources only. Rows marked `fetched: true` were re-read today. For 990 rows, that means I re-parsed the IRS e-file XML (the contractor line and `DonorAdvisedFundsHeldCnt`). The other rows come from the daf-landscape research repo and were not re-fetched.

## A. What sponsors pay incumbents

No vendor publishes a list price. Foundant CommunitySuite is quote-only, and Givinga and Chariot Enterprise are custom-priced. The best evidence is therefore Form 990 Part VII-B (independent contractors). I divided each vendor line by the same return's Schedule D DAF count.

| Tier | Examples (TY2022–24) | $ per DAF-year |
|---|---|---|
| Ren/Stellar software at national/faith sponsors | Thrivent $32; T. Rowe Price $60; Ayco $64; NCF $78 (31,943 DAFs, $2.50m); ImpactAssets $106; GS DAPF $129; GS CGF $226 | **$32–$129** (median about $70) at 2k–32k funds |
| Community-foundation platforms, per total fund | Columbus/Foundant $344,614 ÷ about 3,400 funds; Winston-Salem/Foundant $187,344 ÷ about 1,857 funds | **about $100 per fund-year** |
| Community-foundation platforms, per DAF (upper bound) | Des Moines/Ren $195; Memphis/Ren $221; Denver/Foundant $260; Greater Washington/Blackbaud $308; Santa Barbara/NPact $522; Cleveland/Ren $603 | $195–$603 |
| Fully outsourced administration | US Charitable Gift Trust/Renaissance Admin $413 (37 bps); RCF/Renaissance Admin $745 (33 bps) | **$413–$745** |
| Consumer or employer price points | Daffy for Work $36 per employee-fund-year; Daffy membership $36–$480 per fund-year (donor-paid) | $36–$480 |

Caveats: Part VII-B lists only top-five contractors over $100k, so amounts are floors. Lines may bundle payments, consulting or implementation (Winston-Salem likely includes migration). The sample favours larger sponsors.

### What this means for Unbound licence pricing

1. **At 100k platform funds (break-even about $81 per fund-year), Unbound sits inside the incumbent software band, not below it.** Ren already charges large national sponsors $32–$78 per DAF-year (Thrivent, T. Rowe, Ayco, NCF). Unbound cannot win those accounts on price at cost recovery. It can win on price against:
   - mid-size nationals ($106–$226);
   - community foundations paying $195–$600 per DAF, or about $100 per fund when all funds are counted;
   - sponsors buying fully outsourced administration ($400–$750).
2. **At 50k funds (break-even about $131 per fund-year), a cost-plus price is above every large-national software data point.** It competes only for community foundations and outsourced-admin buyers. In the first years, pricing therefore has to rest on value (less staff time, fewer exceptions), not cost-plus. Alternatively, volume could be seeded where incumbents charge most.
3. **Community foundations need per-fund, not per-DAF, pricing.** Their platforms cover endowment, scholarship and designated funds too. A per-DAF price of $81–$131 looks cheap next to $200–$600 per DAF. Applied to every fund, it lands at or above the about $100 per fund that Foundant charges.
4. **A bps-of-assets option is worth offering to national sponsors.** Ren/Stellar software runs at 1–10 bps of DAF assets, and outsourced administration at 33–37 bps. Sponsors' own fees are 60–90 bps (Columbus: 0.9%, with a $600 minimum per DAF), so a $100 per fund licence is a small share of the revenue each fund earns.
5. **Gap:** no public contract terms (minimums, implementation fees, escalators). Next step: RFP or board-minute search.

## B. Grant-payment rail coverage

| Rail | Coverage fact | Open to other sponsors? |
|---|---|---|
| Fidelity EFT (Stripe/ACH) | 67% of about 3M grants in 2025 paid by EFT; 226,823 charities funded; EFT "not currently" required | No (closed) |
| PayPal Grant Payments / PPGF | "200,000+ PayPal confirmed charities" (2022). PPGF: over 336,000 enrolled across 5 countries; 61,099 new in 2024 | Yes (NPT, Vanguard) |
| Chariot Disbursements | Claims reach to "1.4M+" nonprofits by claim-or-check; 94.57% paid electronically; $2bn a year; 100,000+ DAFpay nonprofits | Yes (Daffy, TIFIN, Founders Pledge, Charityvest, SPMF) |
| Ren Rapid Disbursement | "1.8M reachable" (vendor claim); no ACH-enrolled count | Only to Ren clients |
| Benevity | over 2M validated nonprofits worldwide (eligibility, not bank details); 2.99% fee charged to nonprofits | Corporate giving only |
| For Good, DAFgiving360, NPT/GivingData, Candid | No enrolled count published. For Good pays by check by default; Candid has no bank-verification product | n/a |

**Estimate (inference, low confidence).**
- **By count:** about 15–25% (central 20%) of about 1.5M US 501(c)(3)s already hold an enrolment on an open networked rail (PayPal and/or Chariot). The Chariot figure of 1.4M "reach" is not pre-enrolment: the charity must claim its account, or it receives a check.
- **By grant dollars:** probably about 55–75%. Dollars are concentrated:
  - the top 1,000 recipients take 48.8% of itemised top-50 DAF dollars (FY2024);
  - 64.7% of non-Fidelity top-50 dollars went to charities Fidelity also funds;
  - large charities are the ones most likely to be enrolled on several rails.

  Nothing public measures this.

### What this means for the release-1 default rail

- **The default (partner rail where the sponsor has an agreement, otherwise sponsor ACH) holds up.** No open rail pre-enrols a majority of charities by count. Fidelity, the largest enrolled base, is closed.
- **Sponsor ACH means a new enrolment for each charity.** The sponsor must collect and verify bank details itself, and Fidelity warns its own ACH queue "may take several weeks". So sponsor ACH works for a sponsor's repeat grantees but is a poor fallback for the long tail. Release 1 therefore also needs:
  - a **check fallback**, because every incumbent (PPGF, Chariot, For Good, Ren) still mails checks to unenrolled charities;
  - a stored, portable verified-bank-details record that a later rail change does not strand.
- **Partner adapters, in priority order:**
  1. Chariot Disbursements: open; its claim-or-check model covers the long tail without Unbound holding bank data.
  2. PayPal Grant Payments: about 200k+ confirmed charities; no charity fee.
- **Low uptake when rails are new.** Vanguard paid only 400 charities electronically in its first 4 months. Release-1 projections of the electronic share should start low, rise over time, and be weighted by dollars.

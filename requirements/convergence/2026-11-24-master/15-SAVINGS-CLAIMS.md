# 15 — Savings claims

**Decision.** OD-M-12 (2026-11-25): customer cost-saving claims may be made before a real sponsor's replay, on modelled
or public-data evidence. CNV-ECO-003 was amended so that each claim must:

1. state its basis (modelled, public data or measured) and its comparator;
2. give the median (P50) with its P10–P90 range, or a stated conservative bound;
3. carry the disclosure "modelled estimate, not measured at a customer" when it is modelled;
4. be limited to the enterprise types where the model shows the saving at P50;
5. be approved by the Unbound board.

A claim that later measurement contradicts must be withdrawn or restated within 30 days. Segment commitments remain
gated on the real-sponsor replay.

US advertising law requires a reasonable basis for an objective claim at the time it is made, which is why each claim
carries its source and disclosure. This is a design rule, not legal advice; counsel should review the wording before
publication.

Machine-readable register: [`data/claims-register.jsonl`](data/claims-register.jsonl) (CNV-GOV-008). **Approved by the
board on 2026-11-25 (OD-M-13)**: CLM-02 to CLM-05 as written; CLM-01 as a template, usable once the price is set.

## 1. Approved claims

| ID | Audience | Claim | Basis |
|---|---|---|---|
| CLM-01 | Buyers of fully outsourced administration | "Unbound's administered DAF service is priced at $*price* per fund-year, *n*% below the lowest fully outsourced administration fee disclosed in public Form 990 filings ($413 per DAF-year)." | Public Form 990 fees plus the board's price. Modelled Unbound cost: $150–$299 per fund-year. For example, a $349 price is 15.5% below $413. |
| CLM-02 | National DAF sponsors (E1) | "About 30% less on administration than your current workflow enhanced with AI: $245 per fund-year at the median (range $209–$288) against $351 (range $287–$480)." | Modelled. Savings by lever: platform 3.4%, redesign levers 27.7%. |
| CLM-03 | Advisor-distributed providers (E3) | "About 36% less: $375 per fund-year at the median (range $299–$480) against $589 (range $448–$786)." | Modelled. Savings by lever: platform 10.2%, redesign levers 29.1%. |
| CLM-04 | All sponsors | "About 1 in 9 recipients needed a person to confirm eligibility, but 64% of those had been granted to the year before; by reusing earlier determinations, Unbound reduces first-time eligibility reviews to about 4% of recipients." | Public data (report 14), recipients over $5,000 a year. |
| CLM-05 | E1 and E3 sponsors | "Staff step in about half as often: 526 human interventions per 1,000 funds a year against 1,013 (928 against 1,740 for advisor-distributed providers)." | Modelled |

## 2. Claims that may not be made

The model and the evidence do not support these claims, so the amended CNV-ECO-003 rejects them:

| Claim | Why not |
|---|---|
| Unbound software is cheaper than incumbent DAF software (iPhi/Ren licences) | Unbound's break-even (about $81 per fund at 100,000 funds) sits inside the incumbent band of $32–$129 (report 13). |
| Savings for high-volume workplace giving (E4) or outsourced administrators buying software only (E5) | The model's median for C is above B: $129 against $78 (E4) and $294 against $268 (E5). |
| Savings for community foundations (E2) | Licensed traditional software (AL) costs less than C; CNV-ECO-004 excludes the segment. |
| "Every ineligible or sanctioned recipient is caught" | The reference sanctions matcher missed 12 of 100 altered listed names (report 14 §3.4; OD-M-10). |
| Any figure without its disclosure, or with the median shown but not the range | CNV-ECO-003 |

## 3. Risks to these claims

- **Modelled claims (CLM-02, CLM-03, CLM-05).** These rest on assumed levers. Only determination reuse has public-data support; handling minutes, contacts and reconciliation breaks are unmeasured. The first sponsor's data may move these figures. The 30-day withdrawal rule then applies.
- **Fee comparison (CLM-01).** It depends on Form 990 contractor payments, which are floors and may bundle services Unbound does not provide. Ren's actual list prices are not public.

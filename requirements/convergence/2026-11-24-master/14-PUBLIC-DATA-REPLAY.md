# 14 — Public-data replay

**Question.** No real sponsor's records are available for the shadow replay that CNV-ECO-003 requires. Which of the
replay's measurements can public data supply, and what do they show?

**Method.** The F# tool in [`replay/`](replay/README.md) parses 47 Form 990 e-file returns for the 21 sponsors in the
evidence backlog. For each sponsor it takes the latest tax year (TY2024, or TY2023 for Fidelity) and the year before.
It classifies every Schedule I recipient from IRS data (Pub 78, the EO Business Master File and the automatic-revocation
list), screens every recipient name against the OFAC SDN list and aliases, and routes each row with the reference
screening policy (`Screening.fs`). It also injects known-bad cases. The run is deterministic: two runs were
byte-identical, and input hashes are in `replay/out/replay-results.json`. Generated tables are in
[report 14a](14a-PUBLIC-DATA-REPLAY-TABLES.md). The tool has 20 fixture tests.

**Status.** This is evidence: measured on public data, with the interpretation labelled. It does **not** satisfy
CNV-ECO-003, which still needs a real sponsor's 12-month history and a 90-day time study. Neither does it give legal
authorization: counsel confirms the class rules (OD-P-LEG).

## 1. What public data can and cannot measure

| Replay measurement (report 12) | Public data? | Why |
|---|---|---|
| Eligibility-exception incidence | **Yes, partly** | Schedule I lists every recipient that received more than $5,000 in the year, with the total per recipient. Smaller grants and per-grant counts are not visible. |
| Determination reuse | **Yes** | Recipient overlap between consecutive years at the same sponsor, and across sponsors. |
| Revocation events and timing | **Yes** | IRS revocation list against recipient EINs, plus posting lag for the whole list. |
| Sanctions false positives and recall | **Yes** | Real recipient names are presumed true negatives (IRS-recognised US recipients); recall is measured on injected listed names. |
| Safety (bad cases never pass straight through) | **Yes** | Injected cases drawn from the IRS and OFAC lists. |
| Incumbent staffing and cost per fund | **Partly** | Form 990 figures cover the whole organisation. National sponsors report 0 employees because affiliates do the work. |
| Handling minutes, contacts, complex-gift hours, non-eligibility exceptions (purpose, benefit, §4958) | **No** | These need a real sponsor's records and a time study. |

## 2. Results

Measured over 20 sponsors with itemised Schedule I: 151,567 recipient rows and $22.1 billion in the latest year.

| Measure | Result | Model assumption it informs |
|---|---|---|
| Rows routed to a person (standard / conservative policy) | **11.2% / 12.8%**; dollars 6.0% / 11.3%. By sponsor: 3.2% to 23.8%, median 10.8%. | `exception_rate` base 8% (range 2.8–17%) |
| Exception rows whose recipient the same sponsor granted to the year before | **64.4%** (70.7% dollar-weighted) | `determination_reuse` 0.5 (C), 0.6 (D) |
| … seen at this sponsor or any of the 20 sampled sponsors in either year | **72.3%** (a lower bound for a platform registry) | Platform-wide reuse (D) |
| Effective first-time exception incidence (rate × (1 − reuse)) | **4.0%** within each sponsor; **3.1%** across the platform | C assumes 8% × 0.5 = 4.0%; D assumes 8% × 0.4 = 3.2% |
| Largest exception classes | Houses of worship absent from IRS lists 6.6%; EIN in no IRS list 2.0%; no EIN 1.0%; supporting organisations 1.0%; government units 0.5%; private foundations 0.04% | — |
| Recipients revoked during the grant year | 1.7 per 1,000 rows | Monitoring workload |
| Revocation posting lag (all revocations effective since 2020, n = 369,247) | Median 89 days, mean 110 days | Re-screening at disbursement |
| Rows granted to a recipient already revoked at year start, not in Pub 78 today | 72 rows (0.5 per 1,000), 71 posted before year end | Unexplained; see §3.3 |
| Sanctions alerts, token-weighted matcher at 0.92 | 0.9 per 1,000 names; recall on perturbed listed names 94.8% | `exception_rate` component |
| Sanctions alerts, naive whole-string matcher at 0.92 | 10.5 per 1,000 names; recall 95.2% | — |
| Safety: revoked, private foundation, Type III NFI, non-501(c)(3), unknown EIN, no EIN | **100% routed correctly** (600 cases); 100 clean public-charity controls all pass straight through | Safety requirement |
| Safety: listed name with a clean public-charity EIN | **88 of 100 caught; 12 missed** | Gap; see §3.4 |

## 3. Interpretation

### 3.1 The reuse lever is real, and the raw exception rate is higher than assumed (evidence)

The eligibility-exception rate in the public data (11.2%) is above the model's 8% base. Determination reuse (64%) is
above the 0.5 that approach C assumes. The two effects offset: the effective first-time incidence (4.0%) matches C's
assumption, and platform-wide reuse (3.1%) matches D's (3.2%). For the approaches without reuse (A8, B, C0) the
model's 8% *understates* eligibility workload. The replay therefore strengthens the case for reuse rather than
weakening it. One caveat: these figures cover the eligibility component only, measured over recipients above $5,000.

### 3.2 Most exceptions are "verify once" recipients (evidence and interpretation)

Most exceptions are not risky recipients. They are recipients that IRS lists do not cover: churches (which need not
apply for recognition), school districts and municipalities (government units), and subordinates of group rulings.
Private foundations are 0.04% of rows. A person verifies each such recipient once, from evidence such as a
denomination directory, a group-exemption letter or a government-unit record, and the determination is then reused.
Interpretation: a shared, evidenced recipient registry across sponsors is a core requirement for Unbound. Its legal
status is public, so sharing it does not breach sponsor isolation (OD-P-PLT). This supports CNV clauses on determination
reuse as written.

### 3.3 Revocation timing needs re-screening at disbursement (evidence and interpretation)

An automatically revoked organisation stays on Pub 78 for a median of 89 days after the revocation takes effect. The
grantor-reliance rule (Rev. Proc. 2011-33, already in the dossiers as a counsel item) is why screening is tied to the
IRS status at the time of the grant. Interpretation: screen at approval *and* again at disbursement, and monitor the
weekly revocation list. That is about 1.7 events per 1,000 recipients a year, a negligible workload.

72 rows went to recipients that were revoked at the start of the year and are not on Pub 78 today. These are not
findings against any sponsor. EIN typos on Schedule I and reinstatements missing from the data could explain them.
The tool therefore emits this count only pooled, never per sponsor.

### 3.4 Sanctions screening: the matcher design is a requirement, not a detail (evidence)

- **The naive matcher is unworkable.** Whole-string fuzzy matching alerts on 1 in 100 recipient names because shared
  generic words drive the score ("AL NOOR FOUNDATION" vs a listed "AL-RAHMA FOUNDATION"). Names with Arabic-origin or
  Islamic tokens alert at **4.2%**, against 1.0% for all other names.
- **Rarity-weighted token matching** cuts false alerts tenfold (0.9 per 1,000 names) at the same recall. That is about
  9 alerts a year for a sponsor with 10,000 grantees. The subgroup gap narrows to 0.3% against 0.1% but does not close.
- **Recall is not sufficient.** Dropped-character and punctuation perturbations of listed names are missed 9–17% of
  the time. In the safety test, 12 of 100 listed names attached to a clean EIN passed straight through.

Interpretation (proposal, not yet a clause): a production sanctions control should (a) meet a stated recall target on
this harness's perturbation suite before release; (b) report false-alert rates by name subgroup; and (c) treat an alert
as a prompt for review, never as evidence. The reference matcher here does not meet (a). Production should use a tested
matcher or vendor, with this harness as its acceptance test.

### 3.5 Incumbent cost per fund (evidence, with heavy caveats)

Form 990 non-grant expense per DAF, for the whole organisation:

- National commercial sponsors: $420–$1,958 (T. Rowe $420, Ayco $605, Fidelity $1,119, Renaissance Charitable $1,207, US Charitable Gift Trust $1,793, Thrivent $1,866, Goldman Sachs $1,958).
- Faith and national sponsors with their own staff: about $3,000–$3,700 (NCF, AEF), at 107–112 DAFs per employee.
- Community foundations: $9,500–$46,000, which includes non-DAF programmes.

These figures sit well above the model's per-account operating costs and above the Ren staffed-offer comparator
($413–$745). This is consistent with report 05, but it does not test it: 990 expenses also include marketing,
investment and programme costs. The comparisons in report 05 stay relative, not absolute.

## 4. Consequences

| Item | Status |
|---|---|
| Model levers `determination_reuse` 0.5 / 0.6 and effective exception incidence | **Supported by public data** for the eligibility component. Evidence label stays "A" until a real sponsor's replay confirms it (CNV-ECO-003 unchanged). |
| Model base `exception_rate` 8% for approaches without reuse | Public data suggests about 11% for eligibility alone. Recommend a sensitivity run at 11.2% for A8, B and C0 at the next model revision. |
| Recipient registry with reusable, evidenced determinations shared across sponsors | Supported (§3.2) |
| Re-screening at disbursement plus revocation monitoring | Supported (§3.3) |
| Sanctions matcher recall target, subgroup reporting, alert-is-not-evidence | **Proposed** for owner ratification (§3.4); not recorded as a clause |
| Gate change ("public-data replay sets pilot prices; first staffed client validates with a 12-month price review") | **Not recorded.** Awaiting the owner's explicit confirmation. |

## 5. Limits

- **Schedule I coverage.** Schedule I shows recipients above $5,000 with annual totals, not individual grants. Small grants probably go more often to listed public charities, so the per-grant exception rate is probably *lower* than 11.2%.
- **Current snapshots.** Pub 78 and the BMF are snapshots taken in November 2026, not as of the grant dates. Recipients that lost status after the grant year are classified by today's status.
- **Classification by name.** Churches and government units are recognised by keywords in their names. Misclassification moves rows between exception reasons, not between exception and straight-through.
- **Sample.** 20 itemising sponsors, chosen from the evidence backlog, not at random. Fidelity contributes 53% of rows. The per-sponsor range is reported for that reason.
- **Sanctions false positives.** These assume no real recipient is a sanctioned party; any true match would be counted as a false alert. Alerts were not individually reviewed.
- **Eligibility only.** Exceptions for grant purpose, donor benefit and disqualified persons cannot be seen in public data.

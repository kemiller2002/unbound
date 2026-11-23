# 09 — Workflow improvement proposals

Date: 2026-11-23 | Status: proposals, not approved. Every legal step cited below carries an authority type and needs counsel confirmation where noted. Principle applied: **preserve the outcome, not the process** (`requirements/PRINCIPLES.md`).

## 1. Cross-cutting conclusions

1. **Most human queues in the corpus exist because rules were never written down, not because judgment is needed.** Benefit checks, pledge policy, recipient classes, destination evidence, freshness and fee bases appear as human review steps or prose. Once each is a typed, versioned rule (UNB-RCN-GRT-001/009/010/014/017, UNB-RCN-FIN-005), the routine path needs no person. People handle only typed holds, plus a statistically sampled assurance set.
2. **Replace calendar-driven work with change-driven work.** The 180-day full re-vetting cycle, monthly-only reconciliation, quarterly fee runs, annual dormancy letters and periodic access reviews are proxies for change detection that older systems could not do. The replacements are per-fact freshness with event feeds (IRS revocations, sanctions list deltas, custodian messages), continuous reconciliation, daily accrual, and event-driven revalidation of roles.
3. **One command per outcome, every channel.** Grant recommendation from the portal, an advisor platform, an embedded checkout or a recurring schedule must be the same command with a typed context. Separate per-channel engines are how controls drift (WF-F, REQ-X-057).
4. **Funding and payment become explicit state machines.** Approved → Reserved → FundingPlanned → Funded → payment states (including Unknown and Blocked). This removes the manual "is there cash?" checks and the ambiguous "processing" status (RCF-FIN-014, RCF-GRT-011/013/017).
5. **Month-end is a snapshot, not a project.** With continuous reconciliation, close is a certification event over a locked snapshot. Statements and Schedule D packages are pure queries over that snapshot (W6, W8).
6. **Integrate instead of build where the outcome is not DAF-specific.** Sponsor ERP (AP, AR, payroll, budgeting, fixed assets, consolidation), GRC control-test tooling, international equivalency determinations, complex-asset liquidation and payment rails are partner or integration boundaries (RCD-013, RCD-028, report 05 §3).
7. **AI prepares, explains and flags. It never clears, approves, classifies recipients, values assets or moves money.** It is permitted only to *add* holds, never to remove them (report 11).

## 2. Workflow redesign table

Authority types: S = statute, R = regulation, G = IRS guidance, C = contract, P = sponsor policy, B = best practice.

| Workflow | Required outcome | Legal steps (authority) | Inherited steps to eliminate | Deterministic automation | Human judgment retained | AI role | Replacement model | Key records |
|---|---|---|---|---|---|---|---|---|
| Donor onboarding | Identified participants with sponsor-accepted rights | OFAC prohibitions (S/R; screening is B); exclusive-legal-control acknowledgment (S §170(f)(18)); CIP/AML only if counsel says so (open) | Bank-style "KYC" by analogy; identity collection from view-only participants; wet signatures | Regime applicability register; per-participant activation | Refer outcomes; minors; representatives | Explain, classify documents | Participant-centric onboarding; each role activates on its own checks | RR-UNB-PART-007-r1, UNB-RCN-ENT-014, RCD-023 |
| Fund creation | Fund exists with no grantable capacity until accepted contributions are final | Sponsor ownership and control (S §4966(d)(2)) | Separate "account approval" queue for routine applications | Sponsor acceptance on policy match | Non-standard fund types | — | Open on policy match; capacity gated by contribution finality | UNB-RCN-ENT-011, RCD-024 |
| Contributions | Completed gifts credited once, lawful acknowledgment issued | §170(f)(8), (f)(18) acknowledgment (S); Form 8283 donee signature (S/G); Nacha returns (C) | Periodic acknowledgment mail runs; manual deposit matching | Rail evidence matching; completed-gift rule table; idempotent issuance on `CompletedGiftDetermined ∧ Accepted` | Unidentified deposits above policy; third-party payers | Suggest matches with citations | Single contribution aggregate with typed facts | RR-UNB-ACC-006-r1, UNB-RCN-FIN-001/012/013 |
| Noncash and complex assets | Accept lawful assets, convert to grantable value, meet donee reporting | Form 8282 within 125 days for dispositions within 3 years (S §6050L); §4943(e) holdings (S) | Paper transfer packets; human handling of DTC-eligible securities | Asset-class policy table; 8282 obligations as derived events; four-value valuation record | Complex-asset acceptance; contested valuation; §4943 and UBIT analysis (counsel) | Assemble diligence dossier | Straight-through for public securities; committee only for complex assets; intermediaries for crypto and real estate | UNB-RCN-FIN-002/003/004 |
| Investment management | Assets invested within sponsor policy; advisors act within scope | §4958 for compensated advisors (S); proposed regulations on advisors (proposed, not law) | Manual mandate checks | Policy-band monitoring; mandate checks before trade instruction | Exceptions to investment policy | Explain drift | Menu plus mandate configuration; advisor scope from authority matrix | UNB-INV-014, UNB-RCN-FIN-010, RCD-021 |
| Liquidity management | Approved grants are paid from settled cash, never overcommitted | None beyond sponsor control | Per-grant trades in pooled vehicles; manual cash checks | In-transaction capacity; funding plan per source type; pool liquidity buffer | FundingShortfall above limit; gated pools | Propose funding mixes | Reservation → FundingPlanned → Funded → payment | UNB-RCN-FIN-008/009, RCD-017 |
| Charity vetting and "180-day" revalidation | Each grant goes to a lawful, non-adverse recipient at payment time, and each sponsor can prove reliance | §4966 recipient classes (S); Rev. Proc. 2018-32 reliance per grant date (G); OFAC (S/R). **No legal 180-day period** (P) | Calendar-expiry holds; per-sponsor duplicate vetting; manual alternate verification without an evidence matrix | Per-fact FreshnessPolicy; IRS revocation feed; sanctions deltas; entity-type evidence matrix; release-time recheck | Disputed identity; novel entity types; ER classes | Extract facts from documents; summarize contradictions | Change-driven verification. The comprehensive review becomes an audit sample, not a gate. | RR-REQ-X-007-r1, UNB-RCN-GRT-001/013/014, RCD-006 |
| Grant recommendation and approval | Lawful, sponsor-authorized grant, funded exactly once | §4966, §4967, §4958(c)(2) (S); Notice 2017-73 (G). Per-grant human sign-off is **not** established as law (RCD-005). | Human approval queue for routine public-charity grants; human re-approval of modifications; blanket pledge prohibition | Recipient-class, benefit, related-party and sanctions gates in parallel; atomic approve-and-reserve | Conditional-benefit and ER cases; sponsors without validated delegation | Classify free-text purpose into candidate benefits (may only raise a hold) | One deterministic decision function; humans see holds plus a sample | RR-REQ-X-002-r1, UNB-RCN-GRT-001/002/009/010, RCD-005 |
| Recurring grants | Intent carried out each period without paying an ineligible recipient or paying twice | Same as grant approval, per occurrence | Manual per-occurrence review; improvised catch-up payments | Occurrence idempotency key `(scheduleId, scheduleVersion, periodKey)`; hold window then skip; suspension threshold; durable scheduler (DG-08) | Resuming a suspended schedule (donor) | Suggest alternatives after suspension | A schedule generates independent recommendations under a bounded standing affirmation | UNB-RCN-GRT-004 |
| Payment processing | The right amount reaches the verified destination once, with known finality | OFAC at release (S/R); Nacha rules incl. NOC (C); state unclaimed-property law (S) | Callback on every payment rather than only destination changes; blanket manual handling of Unknown | DestinationEvidence table per rail; outbox; authenticated deduplicated callbacks; positive pay; Unknown resolution protocol; check aging | NotReceived declarations; failed destination evidence; Blocked items | Anomaly detection in bank letters | Partner rail plus six-dimension payment state; humans see only Unknown, Blocked and contradictions | RR-REQ-X-004-r1, RR-REQ-X-005-r1, UNB-RCN-GRT-005/006/008/017, RCD-028 |
| Financial reconciliation | Books agree with independent evidence; statements are reproducible | Schedule D source data (S); audits (state law, C) | Monthly-only cadence; human attestation of deterministic passes | Continuous reconciliation with tolerance, materiality and aging | Material breaks; close certification | Investigate breaks with cited evidence | Continuous reconciliation; close is a snapshot and certification | RR-UNB-ACC-018-r1, RCD-020 |
| Fee assessment | Disclosed fees, computed correctly, never on fees or twice | Agreement disclosure (C); §4958 for related parties (S) | Quarterly runs on period-end balances; manual invoices to funds | FeeBase contract; daily accrual reducing capacity; monthly settlement | Schedule changes; waivers | Anomaly detection | Daily accrual, monthly settlement | UNB-RCN-FIN-005, RCD-018 |
| Account servicing and statements | Donors see truthful status; statements are issued and reproducible | §170(f)(8) receipts (S) | Monthly statements for every fund by default (P) | Event-driven notices; statements per sponsor cadence from locked snapshot | — | Plain-language explanations of status | Self-service status. Statement cadence is sponsor policy. | RCF-FIN-034, REQ-X-032 |
| Advisor and household management | Every action is attributable to a scoped, current role; revocation is immediate | Sponsor control (S); §4958 (S) | Two-person cap; spouse-as-secondary; address-based households; periodic donor-side access reviews | Authorization check per command; hold pending recommendations of revoked advisors; contributor of record | Representative authority; family conflicts | Explain denials | One role catalogue. A household is a view, never an authority. | RR-UNB-PART-003-r1, UNB-RCN-ENT-009/010 |
| Succession | The deceased's authority ends promptly; survivors keep their rights; the plan is applied | State law on proof of death (S); program terms (C/P) | Successor re-application; mailed evidence; one long succession case | Hold on notice; suspend on verification; apply plan; allocation algorithm | Contested evidence; pending-grant policy (OD-03); split approval | Extract document data for comparison | Three events: hold, suspend, activate | RR-UNB-SUCC-008-r1, UNB-RCN-ENT-004, RCD-022 |
| Dormancy and closure | Abandoned funds are distributed charitably under disclosed policy; closure settles obligations | Inactivity rules are P; escheat of balances doubtful (counsel); transfer to another sponsor is a distribution (S, counsel) | Annual letters to everyone; manual closure checklists; forced liquidation where in-kind transfer is possible | Clock and stage transitions; closure preconditions; transfer as a grant with a purpose code | Final disposition (A4); transfer approval | Suggest charities only from the donor's own history | RR-UNB-WF-015-r1; closure state machine | UNB-RCN-ENT-005/006/007 |
| Sponsor administration | A sponsor configures policy correctly and knows consequences before approval | Board delegation (state law; counsel) | Long implementation projects; separate help docs | Validation, dependency checks and impact preview generated from contracts | Approving policy diffs | Explain contracts; propose diffs | One gated activation command; configuration diff and approve | RR-REQ-X-051-r1, UNB-RCN-PLT-005 |
| Communications | Accurate, branded, accessible message about a real state change, with proof of what was sent | Acknowledgment content (S); CAN-SPAM, TCPA (S/R) | WYSIWYG design per email; per-sponsor HTML | Notification event catalog; typed tokens; locked legal clauses; as-sent snapshot | Approving sponsor wording | Draft slot text and alt text for approval | Constrained per-event templates plus Brand Kit | RR-REQ-X-029-r1, UNB-RCN-PLT-002/007, RCD-008 |
| Regulatory reporting | Accurate annual DAF data per legal sponsor, reproducible as filed | Form 990 Schedule D Part I (S §6033(k)); pending §§4966/6033 guidance (watch) | Re-keyed aggregates; spreadsheets | Package as a pure query over the locked period plus the classification register | Recognition policy; classification; signature | Variance narratives | RR-REQ-X-060-r1 | UNB-RCN-ENT-011/012, UNB-RCN-REG-001 |
| Customer support | Questions answered from authoritative state; decisions contestable; support cannot bypass authorization | Accessibility (counsel); privacy requests (S, state) | Status-inquiry calls; staff entry of consequential changes | Routing; assisted changes gated on verified-channel confirmation | Appeals; hardship | Grounded answers from authoritative state | Self-service status plus one appeal case type | UNB-RCN-ENT-016 |
| Migration and conversion | Single authoritative writer; reconciled history; no redirected payments | Retention (S/R/C); data-processing termination (C, privacy law) | Charities re-submitting bank details; big-bang cutover | Tie-outs; fencing; blocking `imported_unverified` payees | Accepting breaks; authorizing cutover | Propose field mappings; explain breaks | Parallel run with fenced cutover; exit through tenant-scoped key destruction | RR-REQ-X-061-r1, RCC-ENT-005 |

## 3. Challenges to these proposals (self-review)

- **"Humans see only holds" depends on RCD-005.** If counsel rejects delegated approval, the deterministic tier still runs. Its output becomes a pre-approved batch for one-click human sign-off, so the engineering is unchanged and only the authority flag differs.
- **Change-driven vetting depends on feed quality.** The IRS auto-revocation list and BMF are published periodically, not in real time. The release-time recheck still compares against the newest available revision, and the snapshot records that revision date. Reliance is defined by that date, not by "real time".
- **Database per sponsor (RCD-004) multiplies migrations.** Strata deploys one artifact to many databases (verified partial capability), but fleet orchestration and per-partner schema templating are not Strata features, so Unbound must build or adopt them (DG-09, DG-11). Row-level security between partners would be simpler but reverses the current owner position (`PARTNER-SEGREGATION-AND-TENANCY.md:38`), so it is offered only as an alternative.
- **Integrating instead of building ERP depends on sponsors having an ERP.** Small community foundations may not. The summarized journal export must be consumable by common small-nonprofit accounting packages, which is a product-discovery question for RCD-013.

## 4. Appendix: domain workflow challenges (from the role-separated audits)

The sections below are the domain auditors' detailed analyses, merged unchanged except for one correction: the grants analysis cited Rev. Proc. 2011-33 for IRS-status reliance, which is superseded by **Rev. Proc. 2018-32** (report 06, RCF-REG-008).

### Appendix GRT


#### WF-A Charity verification and reverification

- **Required outcome:** Each grant goes only to an identifiable, lawful recipient that is not adverse at the moment of payment, and every sponsor can rely on that result.
- **Why the current process exists:** Sponsors each re-vet charities by hand, case by case. The research then added a fixed 180-day revalidation cycle to make that reuse auditable.
- **Steps that are legal obligations (authority type):**
  - Sanctions screening and blocking. OFAC regulations; scope needs counsel.
  - Confirming the recipient is not a taxable-distribution class. Statute IRC 4966(c) and (d)(4); application needs counsel.
  - Reliance on IRS status data. Rev. Proc. 2018-32 (supersedes 2011-33, which vendors still cite per daf-landscape batch-2a); counsel must confirm cross-sponsor reliance.
  - No periodic interval was found in any statute or regulation. The 180-day cycle is policy.
- **Steps that exist only because of legacy technology, poor data or convention:**
  - A periodic full re-review of every charity, standing in for change detection the systems could not do.
  - Re-vetting the same charity for each sponsor.
  - Manual alternate verification for churches and government units, done because no evidence matrix exists.
- **Steps that need human judgment:**
  - Disputed identity between two E0 sources.
  - Novel entity types.
  - Raising a global hold on non-authoritative evidence.
  - Counsel-review recipient classes.
- **Steps to eliminate:**
  - Calendar-expiry holds (TX-27 'Scheduled 180-day trigger -> ExpiredOrAdverseHeld').
  - Compliance sign-off on drift that evidence refresh resolves (EXC-01).
  - Per-sponsor duplicate vetting.
- **Steps that can run concurrently:** Per-fact refreshes, sanctions delta screening and the identity-resolution re-check run in parallel. Grant drafting continues while refresh is in progress; only release depends on current facts.
- **Deterministic automation:**
  - The freshness table drives refresh scheduling.
  - IRS revocation and reinstatement feeds give an immediate status change.
  - The entity-type evidence matrix covers recipients not in IRS files.
  - Recipient-class mapping comes from IRS codes plus the related-party graph.
- **Where AI genuinely helps:**
  - Extracting facts from determination letters, audited statements and foreign registry documents.
  - Comparing contradictory sources and drafting reviewer summaries with citations.
- **Where AI introduces unacceptable uncertainty:**
  - Setting a recipient class.
  - Clearing a sanctions or adverse hold.
  - Promoting E4 web content to evidence.
  - Defamation-sensitive allegations.
- **Simpler replacement model:** Change-driven verification. Each fact has its own maximum age and monitoring source. Status changes only on evidence. Release always rechecks critical facts. The 'comprehensive review' becomes an audit sample, not a gate that blocks grants.

#### WF-B Grant recommendation, benefit checks, approval and reservation

- **Required outcome:** A lawful, sponsor-authorized grant with funding committed exactly once.
- **Why the current process exists:** The sponsor keeps legal control (donor advice is advisory). Historically every grant was reviewed by a person because benefits, pledges and recipient classes were not encoded as rules.
- **Steps that are legal obligations (authority type):**
  - Sponsor control over distributions. Statute: the DAF definition and the 170(f)(18) acknowledgement wording; needs counsel.
  - Prohibiting more-than-incidental benefits. IRC 4967 statute, with Notice 2017-73 as IRS reliance guidance on pledges and tickets.
  - 4958(c)(2) payee block (statute).
  - Taxable-distribution classes (statute).
  - Per-grant human sign-off is not established as a legal requirement (TVC-003; OD-01 still open).
- **Steps that exist only because of legacy technology, poor data or convention:**
  - Human approval queues for routine public-charity grants.
  - Human re-approval of modifications (UNB-GRANT-048) when re-running the rules is enough.
  - Blanket pledge prohibition: a sponsor policy that the requirements present as law.
- **Steps that need human judgment:**
  - Conditional-benefit cases.
  - Counsel-review recipient classes.
  - Sponsors without validated delegation (OD-01).
- **Steps to eliminate:**
  - Manual approval for grants where every deterministic gate passes and the sponsor has validated delegation.
  - Separate compliance queues for pledge cases that the sponsor's policy already decides.
- **Steps that can run concurrently:** Recipient classification, benefit gate, sanctions screen and funding quote run in parallel. Approval and reservation are one atomic step.
- **Deterministic automation:**
  - The affirmation evidence contract.
  - The benefit catalogue plus host-context declaration.
  - The recipient-class gate and related-party gate.
  - The atomic reservation, with per-source units and haircut.
- **Where AI genuinely helps:**
  - Classifying free-text purposes into candidate benefit categories, which can only raise a hold.
  - Explaining holds to donors.
- **Where AI introduces unacceptable uncertainty:**
  - Affirming on the donor's behalf.
  - Passing a held benefit case.
  - Classifying recipients.
- **Simpler replacement model:** One decision function run by the rules (REQ-X-017 Tier 1) with typed holds. A person sees only grants on hold, plus a sample taken for assurance.

#### WF-C Recurring and scheduled grants

- **Required outcome:** Donor intent is carried out across periods, never paying an ineligible recipient and never paying twice.
- **Why the current process exists:** Schedules were treated as standing approvals, or were re-reviewed by hand each time.
- **Steps that are legal obligations (authority type):**
  - Rechecking eligibility, sanctions and benefit for each occurrence. These are the same statutory and regulatory bases as WF-B.
  - How long a standing affirmation stays valid is sponsor policy; counsel must confirm.
- **Steps that exist only because of legacy technology, poor data or convention:** Manual review of every occurrence.
- **Steps that need human judgment:** None, routinely. Resuming a suspended schedule needs the donor's action.
- **Steps to eliminate:**
  - Manual handling of occurrences.
  - Improvised catch-up payments.
- **Steps that can run concurrently:** Occurrence gates run independently of other occurrences.
- **Deterministic automation:**
  - Occurrence idempotency key.
  - Hold window, then skip.
  - Suspension threshold.
  - Notifications.
- **Where AI genuinely helps:** Suggesting alternative charities after a suspension, as an advisory only.
- **Where AI introduces unacceptable uncertainty:** Resuming a schedule or re-affirming for the donor.
- **Simpler replacement model:** A schedule is a generator of independent grant recommendations under a bounded standing affirmation (UNB-RCN-GRT-004).

#### WF-D Payment release, rails and settlement

- **Required outcome:** The right amount reaches the verified charity destination once, and its finality is known.
- **Why the current process exists:** The rails are opaque. People confirm payments by hand because providers do not give independent proof.
- **Steps that are legal obligations (authority type):**
  - Sanctions screening at release (OFAC regulations).
  - ACH network rules, including NOC handling and returns. These are private rules enforced by contract.
  - Unclaimed-property law for uncashed checks (state statutes).
- **Steps that exist only because of legacy technology, poor data or convention:**
  - Manual callback verification on every payment, rather than only on destination changes.
  - Blanket manual handling of every Unknown result.
- **Steps that need human judgment:**
  - Declaring NotReceived.
  - Destination changes that fail the evidence table.
  - Blocked items.
- **Steps to eliminate:**
  - Manual release when the destination evidence table is satisfied.
  - Manual handling of known benign callback orderings.
- **Steps that can run concurrently:** Destination verification and grant approval. Rescreening on list updates runs alongside the outstanding-check aging job.
- **Deterministic automation:**
  - The DestinationEvidence table.
  - Outbox dispatch.
  - Authenticated callbacks with semantic deduplication.
  - Positive-pay issue and void files.
  - The Unknown resolution protocol schedule.
  - Check aging.
- **Where AI genuinely helps:**
  - Detecting anomalies in bank letters.
  - Summarising contradictory callbacks for the reviewer.
- **Where AI introduces unacceptable uncertainty:** Any money-state transition.
- **Simpler replacement model:** Six-dimension payment state (REQ-X-083) plus rail evidence tables. People handle only Unknown, Blocked and contradictions.

#### WF-E International and expenditure-responsibility grants

- **Required outcome:** Lawful cross-border grants, with obligations tracked to completion.
- **Why the current process exists:** Equivalency determinations and ER are labour-intensive and specialist, so the work is outsourced or done case by case.
- **Steps that are legal obligations (authority type):**
  - ER and equivalency-determination routes (statute and regulations).
  - Sanctions screening (OFAC regulations).
  - Country restrictions.
  - All of these need counsel.
- **Steps that exist only because of legacy technology, poor data or convention:** Re-collecting the same foreign registry documents for every grant.
- **Steps that need human judgment:**
  - The equivalency determination itself.
  - ER report review.
  - Anti-diversion red flags.
- **Steps to eliminate:** Re-reviewing ER reports that a deterministic completeness check already accepts. Substantive review stays.
- **Steps that can run concurrently:** Inquiry, agreement drafting, FX quote and screening.
- **Deterministic automation:**
  - The ER case tracker.
  - Report due dates.
  - Holds on further grants while a report is overdue.
  - Rescreening of intermediary banks.
- **Where AI genuinely helps:**
  - Translating foreign documents and extracting facts, with provenance.
  - Drafting ER agreements from templates.
- **Where AI introduces unacceptable uncertainty:** Equivalency conclusions.
- **Simpler replacement model:** Partner for the equivalency determination, and track it inside the shared grant and ER case model.

#### WF-F External checkout and recipient self-service

- **Required outcome:** Charity-initiated or embedded DAF grants carry the same controls, and charities can see status and manage verified destinations.
- **Why the current process exists:** Third-party checkout widgets sit outside sponsor controls.
- **Steps that are legal obligations (authority type):**
  - Benefit rules (statute and guidance).
  - Sponsor identification as the legal donor (best practice backed by IRS guidance).
- **Steps that exist only because of legacy technology, poor data or convention:** Charities re-registering with every sponsor.
- **Steps that need human judgment:** Context mismatches and benefit holds.
- **Steps to eliminate:** Separate per-channel grant engines.
- **Steps that can run concurrently:** Widget consent and pre-validation of the charity's status.
- **Deterministic automation:**
  - HostContext declaration.
  - Affirmation token checks.
  - Remittance statement and pledge-suppression filter.
- **Where AI genuinely helps:** Matching recipient CRMs, as an advisory only.
- **Where AI introduces unacceptable uncertainty:** Attributing a grant to a donor where the donor chose anonymity.
- **Simpler replacement model:** One canonical recommendation command for all channels, with the context typed at intake.


### Appendix FIN


#### W1. Cash contribution intake and acknowledgment
- **Required outcome:** sponsor-accepted, completed gifts are credited to the correct fund, and donors get a lawful acknowledgment.
- **Why the current process exists:** rails have different finality; tax law conditions deductions on acknowledgment content.
- **Legal-obligation steps:** acknowledgment content and timing (IRC 170(f)(8), 170(f)(18): statute); NACHA return handling (private network rules); completed-gift date (regulation and counsel).
- **Legacy-only steps:** batching acknowledgments into periodic mail runs; manual matching of deposits by name.
- **Human judgment:** unidentified deposits above policy; third-party payer acceptance (CON-REC-16); template approval (counsel, once per version).
- **Eliminate:** per-gift human acceptance of routine cash from verified donors (deterministic acceptance policy); manual acknowledgment drafting.
- **Run concurrently:** fund attribution, ledger posting and acknowledgment eligibility evaluation once the receipt is final.
- **Deterministic automation:** rail evidence matching, completed-gift rules, template validation, idempotent issuance.
- **AI helps:** suggesting matches for unidentified receipts with citations.
- **AI unacceptable:** wording or issuing acknowledgments; deductibility statements.
- **Simpler model:** a single contribution aggregate with typed facts; acknowledgment generated automatically on `CompletedGiftDetermined AND Accepted`.

#### W2. Securities, crypto and complex assets
- **Required outcome:** accept only lawful, policy-permitted assets, convert them to grantable value, and meet donee reporting duties.
- **Legal-obligation steps:** Form 8283 Section B donee signature (statute and IRS guidance); Form 8282 within 125 days on disposition within three years (IRC 6050L); excess business holdings (IRC 4943(e)); UBIT exposure (counsel).
- **Legacy-only:** paper transfer packets; re-keying custodian confirmations.
- **Human judgment:** complex-asset acceptance (committee), EBH and UBIT analysis (counsel), contested valuation.
- **Eliminate:** human handling of DTC-eligible public securities, using straight-through acceptance plus a liquidation policy.
- **Concurrent:** custody confirmation and valuation evidence collection; 8282 obligation creation runs alongside sale settlement.
- **Deterministic automation:** 8282 obligation creation and deadline tracking; four-value valuation records (FI-24).
- **AI helps:** assembling dossiers (liens, debt, prearranged-sale indicators) for reviewers.
- **AI unacceptable:** acceptance decisions, valuation selection for books, filing exception determinations.
- **Simpler model:** asset-class policy table drives straight-through or committee paths; obligations are derived events, not checklists.

#### W3. Grant funding, liquidity and insufficient cash
- **Required outcome:** an approved grant is paid exactly once from settled cash attributable to its fund.
- **Legal-obligation steps:** none beyond grant eligibility (other domain); fiduciary sponsor control (statute/sponsor policy).
- **Legacy-only:** per-grant security sales in pooled vehicles (FI-14 shows pools trade net).
- **Human judgment:** FundingShortfall decisions, liquidity advances above limit, gated pools.
- **Eliminate:** per-grant trades for pooled sources; manual cash-availability checks.
- **Concurrent:** unit redemption and payee verification proceed together; payment authorization waits only for Funded.
- **Deterministic automation:** serializable reservation, funding plan by source type, Funded gate.
- **AI helps:** proposing funding mixes within donor preferences.
- **AI unacceptable:** approving advances, trades or payments.
- **Simpler model:** reservation, then a FundingPlanned/Funded state machine, then the payment state machine (REQ-X-083).

#### W4. NAV strike and unitization
- **Required outcome:** fair, reproducible unit prices; no participant gains from stale prices.
- **Legal-obligation steps:** none statutory for internal DAF pools; sponsor fiduciary policy and audit expectations.
- **Legacy-only:** manual price spreadsheets and email sign-off.
- **Human judgment:** price challenges, illiquid valuations, NAV error remediation above materiality.
- **Eliminate:** human approval of NAVs that pass all deterministic tolerance checks (CTL-017), if the sponsor delegates.
- **Concurrent:** price validation per asset; custody reconciliation per account.
- **Deterministic automation:** forward pricing, decimal arithmetic, residual accounting, publication gate.
- **AI helps:** explaining price outliers.
- **AI unacceptable:** approving overrides or publishing NAVs.
- **Simpler model:** for sponsors with few pools, consider lot-level sub-accounting instead of unitization (RCF-FIN-016); keep unitization optional.

#### W5. Fee assessment
- **Required outcome:** disclosed fees, correctly computed, never charged on fees or twice.
- **Legal-obligation steps:** sponsor agreement disclosure (contract); related-party governance (IRC 4958 where applicable; counsel).
- **Legacy-only:** quarterly fee runs on period-end balances.
- **Human judgment:** schedule changes, waivers, OD-10 priority policy (decided in advance).
- **Eliminate:** manual fee invoices to funds; manual fee reconciliation.
- **Concurrent:** daily accrual alongside valuation.
- **Deterministic automation:** FeeBase contract, daily accrual reducing capacity, recompute checks.
- **AI helps:** anomaly detection on fee changes.
- **AI unacceptable:** waivers and schedule edits.
- **Simpler model:** daily accrual, monthly settlement; fee recipient type selects the posting rule.

#### W6. Reconciliation and month-end close
- **Required outcome:** books agree with independent evidence; issued statements are reproducible.
- **Legal-obligation steps:** Form 990/Schedule D source data (statute); audit (state law and contract for many sponsors).
- **Legacy-only:** a monthly-only cadence; checklist-driven sign-off of items that passed deterministically.
- **Human judgment:** material breaks, threshold policy, close certification.
- **Eliminate:** human attestation of each deterministic pass (attest the rules instead, RCC-FIN-004); forced monthly statements for all funds (REQ-X-032).
- **Concurrent:** reconciliations by class and account; statement rendering per fund after snapshot.
- **Deterministic automation:** continuous daily reconciliation with tolerance/materiality/aging; snapshot commit.
- **AI helps:** investigating breaks with cited evidence.
- **AI unacceptable:** clearing breaks or marking Unknown as Pass (ADV-448).
- **Simpler model:** continuous reconciliation makes month-end a snapshot and certification event, not a batch project.

#### W7. Sponsor operating finance (AP/AR/payroll/budget)
- **Required outcome:** sponsor operating books are correct and include DAF program totals.
- **Why it exists in the requirements:** owner-directed 'complete integrated accounting system'.
- **Eliminate or replace:** native AP, AR (except partner fee billing), payroll, budgeting, fixed assets and consolidation; replace with a summarized, reconciled journal export to the sponsor ERP (RR-UNB-ACC-012-r1). Owner decision required.



### Appendix PLT

#### W1. Partner onboarding and tenant isolation provisioning (WF-12, UNB-TEN-*)
- **Required outcome:** a partner's donors and staff can transact under the right legal sponsor with data verifiably segregated from every other partner.
- **Why the current process exists:** institutional partners (banks, wealth managers) ask for contractual segregation; the requirements pre-emptively support every tier.
- **Legal-obligation steps:** data processing agreement and controller/processor roles (contract; needs counsel); sponsor legal-control of assets (IRC 4966 DAF definition, statute; needs counsel on how delegation is evidenced); privacy law obligations per residency (regulation; counsel).
- **Legacy/convention-only steps:** choosing among three isolation tiers per partner before any contract demands it; manual approval chains for routing changes.
- **Human judgment:** whether a contract clause actually requires dedicated infrastructure; risk acceptance for shared-backup limitations.
- **Eliminate:** default availability of Profiles A and C; A->B->C migration readiness for tenants that never asked for it.
- **Run concurrently:** schema/database provisioning, Brand Kit setup, role import and isolation test suite execution.
- **Deterministic automation:** provisioning plan, per-tenant roles and keys, isolation test suite, binding generation, evidence bundle.
- **Where AI helps:** extracting isolation, residency and audit clauses from the partner contract into a proposed TenantIsolationDecision with citations.
- **Where AI is unacceptable:** selecting the profile, assembling cross-tenant context, or reading partner data during onboarding.
- **Simpler model:** one default profile (database per partner), one release, cells only by contract (RR-UNB-PLAT-008-r1, RR-REQ-X-057-r1). **Integrating note:** superseded by RCD-004 (database per legal sponsor, partner schemas inside it), because per-partner databases split a sponsor's books (RCF-PLT-003).

#### W2. Control operations and SOC evidence (SOC2-*, MON-*, OPS-*, UNB-SCTRL-*)
- **Required outcome:** an independent auditor can rely on evidence that each in-scope control operated over the period, and gaps are visible.
- **Why the current process exists:** a SOC readiness directive translated into a full in-product GRC lifecycle (16 steps, 18 screens).
- **Legal-obligation steps:** none statutory for SOC 2 (AICPA attestation is contractual/market); SOC 1 for fund accounting reliance (auditor practice; CFO decision, TVC-027); sponsor fiduciary record duties (counsel).
- **Legacy/convention-only steps:** quarterly manual evidence collection; control designer screens; risk heatmaps; keyword crosswalks.
- **Human judgment:** control design adequacy, sampling, deficiency severity, independent review.
- **Eliminate:** the 90 templated controls and 1,893 keyword links as coverage; in-product Control Designer/Test Planner until justified.
- **Run concurrently:** evidence emission runs inline with business commands; expected-evidence monitor runs continuously and independently.
- **Deterministic automation:** evidence envelopes on every consequential command, integrity hashing, expected-vs-observed computation, Folio evidence export, deficiency case creation.
- **Where AI helps:** drafting control narratives, proposing candidate requirement links with rationale for human validation, summarising deficiency evidence.
- **Where AI is unacceptable:** marking links validated, changing control status, closing deficiencies, attesting effectiveness.
- **Simpler model:** evidence-by-design in product, control register and testing in an external GRC tool or simple register, control-first mapping (RR-SOC2-007-r1).

#### W3. Communications: templates, branding, notifications (REQ-X-027..031, 052, AIN-023, ADM-030)
- **Required outcome:** the right recipient receives an accurate, branded, accessible message about a real state change, and the sponsor can prove what was sent.
- **Why the current process exists:** white-label sponsors want their brand and wording; incumbents expose template editors.
- **Legal-obligation steps:** contribution acknowledgment content (IRC 170(f)(8) substantiation, statute; counsel on sponsor wording); CAN-SPAM for commercial email and TCPA for SMS (statute/regulation; counsel); privacy notices (regulation).
- **Legacy/convention-only steps:** WYSIWYG design of each email; per-sponsor HTML; manual test sends.
- **Human judgment:** wording approval of sponsor-specific text; whether an optional notice is enabled.
- **Eliminate:** visual designer, raw HTML mode, expression-language conditionals beyond typed tokens (initial scope).
- **Run concurrently:** rendering of email, in-app notice and Document Center entry from the same event and snapshot.
- **Deterministic automation:** event catalog decides recipients and class; token resolution; locked clauses; delivery outbox; as-sent snapshot.
- **Where AI helps:** drafting slot text, plain-language rewrites, accessibility suggestions (alt text) for human approval.
- **Where AI is unacceptable:** publishing templates, choosing recipients, generating financial figures or status text.
- **Simpler model:** constrained per-event templates plus Brand Kit (RR-REQ-X-029-r1) and a notification catalog (UNB-RCN-PLT-002).

#### W4. Document issuance, Document Center and retention (REQ-X-034..038, UNB-DOC-*)
- **Required outcome:** every issued financial document is reproducible byte-for-byte, accessible, findable by authorized people, and kept as long as obligations require and no longer.
- **Why the current process exists:** audit and tax substantiation; legacy practice of keeping everything forever.
- **Legal-obligation steps:** record retention for exempt organizations (IRC 6001 and regulations, statute/regulation; counsel on period and trigger); OFAC recordkeeping (regulation; counsel on period); litigation holds (case law/procedure; counsel); privacy deletion rights (regulation; counsel on precedence).
- **Legacy/convention-only steps:** per-record human disposition review; regenerating documents on demand.
- **Human judgment:** class rules and periods (GC), hold placement/release, disposition batch approval.
- **Eliminate:** per-record review queue; duplicate retention requirements.
- **Run concurrently:** accessible rendition and PDF generation from one snapshot; delivery and registry write via outbox.
- **Deterministic automation:** classification by document type, retention clock, hold propagation, batch disposition with pre-deletion recheck, integrity verification.
- **Where AI helps:** suggesting a class for unstructured uploads, flagging misclassified records for review.
- **Where AI is unacceptable:** shortening retention, approving disposition, determining legal periods.
- **Simpler model:** class-rule table with authority citations and batch disposition (RR-REQ-X-036-r1).

#### W5. Exceptions and human work (REQ-X-048/049, ADM-015/016/031, AIN-009)
- **Required outcome:** every blocked outcome is unblocked by the right person quickly, and routine outcomes never wait for a person.
- **Why the current process exists:** incumbent DAF operations run department queues (grants, contributions, compliance).
- **Legal-obligation steps:** sponsor approval of grants where not delegated (statute/sponsor policy; OD-01); sanctions match review (regulation).
- **Legacy/convention-only steps:** department-specific queues, supervisor command centers, manual round-robin assignment of routine work, re-keying evidence.
- **Human judgment:** ambiguous charity evidence, reserved approvals, fraud and conflict calls.
- **Eliminate:** WorkItems without a creating rule and exit condition; separate case models; duplicate dashboards.
- **Run concurrently:** evidence acquisition, eligibility, funding check and fraud scoring for a grant in parallel before any human sees it.
- **Deterministic automation:** creation and auto-closure of WorkItems by rules; routing; SLA escalation.
- **Where AI helps:** summarising the case, drafting evidence requests, proposing next action with sources.
- **Where AI is unacceptable:** approving, lifting holds, closing decision-bearing items, reassigning authority.
- **Simpler model:** one exception inbox of rule-created WorkItems (RR-REQ-X-048-r1) with STP metrics (UNB-RCN-PLT-006).

#### W6. AI-assisted configuration and help (REQ-X-051, 053..055, AIN-*)
- **Required outcome:** administrators configure sponsor policy correctly the first time and understand consequences before approval.
- **Why the current process exists:** complex configuration surfaces and long implementation projects at incumbents.
- **Legal-obligation steps:** none directly; policy content may embody legal rules requiring counsel (GOV-AUTH-011).
- **Legacy/convention-only steps:** separate help docs drifting from product; free-form chatbots.
- **Human judgment:** approving policy diffs, interpreting law into policy.
- **Eliminate:** AI on every field; four autonomy taxonomies; duplicate REQ-X-054/055 variants.
- **Run concurrently:** contract generation in CI alongside API schema generation.
- **Deterministic automation:** validation, dependency checks, impact preview from contracts.
- **Where AI helps:** explaining contracts, proposing configuration diffs, finding contradictions between settings.
- **Where AI is unacceptable:** publishing policy, granting permissions, using retrieved content as instructions.
- **Simpler model:** contract-generated help everywhere; AI only on configuration/policy/case surfaces (RR-REQ-X-051-r1, UNB-RCN-PLT-005).

#### W7. Telephone self-service (REQ-X-025)
- **Required outcome:** a caller can learn the status of their grant or account, or reach a person, without exposing private data.
- **Why the current process exists:** call centers at large sponsors.
- **Legal-obligation steps:** call recording consent (state law; counsel); accessibility of alternative channels (ADA; counsel).
- **Legacy/convention-only steps:** IVR menu trees.
- **Human judgment:** identity verification exceptions, sensitive changes.
- **Eliminate (pending demand evidence):** IVR channel; keep numeric references for human-assisted phone and mail support.
- **Deterministic automation:** reference validation (check digit), rate limits, status lookup through query facade.
- **Where AI helps:** agent-assist for human support staff with scoped context.
- **Where AI is unacceptable:** voice agents authenticating callers or performing consequential actions.
- **Simpler model:** numeric references plus authenticated callback to a human; IVR only after demand evidence.


### Appendix ENT


Authority types used: **statute**, **regulation**, **proposed regulation**, **IRS guidance**, **sponsor policy**, **contract**, **best practice**. Every legal item below needs counsel to confirm it; none of them is settled by this audit.

#### W1. Sponsor activation (E2E WF-01, REQ-X-064)
- **Required outcome:** a legally existing sponsoring charity has approved the program and delegated authority, has opening balances reconciled, and has disclosures in place before any donor can transact.
- **Why the current process exists:** sponsor diligence and contracting, plus vendor onboarding projects.
- **Legal-obligation steps:** sponsor exempt status and its board's delegation of authority (governing law and documents; counsel). Applicability of state charitable-platform or solicitation rules to each legal role (statute, varies by state; counsel; RCF-ENT-030). Data-processing terms (contract).
- **Legacy-only steps:** separate implementation projects per channel, and manual brand QA.
- **Human judgment:** board delegation, contract terms, risk appetite.
- **Eliminate:** a second onboarding product per channel. REQ-X-064 already forbids this; keep it.
- **Run concurrently:** configuration drafting, integration validation and the opening-balance tie-out.
- **Deterministic automation:** the activation gate is a conjunction of evidence predicates: agreement executed, delegation recorded, controls validated, opening GL reconciled, legal-sponsor disclosure present (INV-18).
- **Where AI helps:** comparing contract clauses against the responsibility-matrix template.
- **Where AI is unacceptable:** interpreting delegation or legal applicability.
- **Simpler model:** activation is one gated command with an evidence checklist, not a project-management queue.

#### W2. Fund establishment and participant onboarding (E2E WF-02, ADM WF-01/02, UNB-PART-007)
- **Required outcome:** an identified, authorized set of participants whose rights the sponsor has accepted, on a fund that has no grantable capacity until a contribution is accepted and settled (INV-01).
- **Why the current process exists:** paper applications, plus bank-style "KYC" carried over by analogy.
- **Legal-obligation steps:** sanctions screening (regulation, OFAC; scope for counsel). The fund agreement and the donor's written acknowledgment that the sponsor has exclusive legal control (statute, substantiation context; counsel). Customer-identification and AML programs apply **only if** counsel records that they do, whether by law or by custodian or payment-provider contract (RCF-ENT-014). Source-of-funds diligence is **sponsor policy**.
- **Legacy-only steps:** collecting full identity data from view-only participants; wet signatures; re-keying data supplied by a partner.
- **Human judgment:** resolving Refer outcomes; edge cases such as minors, entity donors and representatives.
- **Eliminate:** identity collection beyond what the enabled regimes require; a separate "account approval" queue for routine applications that meet policy.
- **Run concurrently:** invitations to every participant, identity checks per regime, and agreement e-signature.
- **Deterministic automation:** regime resolution from the applicability register, role activation, and capacity gating.
- **Where AI helps:** document classification for representative authority; plain-language explanations.
- **Where AI is unacceptable:** clearing sanctions or identity mismatches.
- **Simpler model:** participant-centric onboarding. The fund opens when the sponsor accepts the agreement, and each participant's rights activate on their own checks (RR-UNB-PART-007-r1).

#### W3. Advisor and participant management, households (UNB-PART, UNB-ADV, REQ-X-062, ADM-008)
- **Required outcome:** every action is attributable to a scoped, current, evidence-backed role, and revocation is effective immediately.
- **Why the current process exists:** primary/secondary account models and advisor-firm attestations.
- **Legal-obligation steps:** the sponsor's control over distributions (statute: the DAF definition; counsel). Excess-benefit rules on payments to donors, donor advisors and related investment advisors (statute; proposed regulation pending; RCF-ENT-023).
- **Legacy-only steps:** a two-person cap; spouse-as-secondary; household defined by mailing address.
- **Human judgment:** representative authority; conflicting family instructions; advisor conflicts.
- **Eliminate:** periodic manual "access reviews" of donor-side roles. Replace them with expiry and event-driven revalidation; keep the GOV-AUTH-025 sponsor-staff review.
- **Run concurrently:** not applicable.
- **Deterministic automation:** an authorization check on every command; holding a revoked advisor's pending recommendations; computing contributor of record (UNB-RCN-ENT-009).
- **Where AI helps:** explaining a denial and drafting role-change notices.
- **Where AI is unacceptable:** granting roles or interpreting POA scope.
- **Simpler model:** one role catalogue with fixed semantics (RR-UNB-PART-003-r1). A household is a view, never an authority.

#### W4. Donor death, incapacity and succession (UNB-SUCC, REQ-X-008, E2E WF-18, LS WF-13)
- **Required outcome:** a deceased or incapacitated party's authority ends promptly, survivors keep their own rights, the sponsor's plan is applied, and pending items are handled by the sponsor's policy.
- **Why the current process exists:** mailed death certificates, legal review of paper designations, and limbo for pending grants.
- **Legal-obligation steps:** sponsor control over pending grants (statute plus sponsor policy; OD-03). Proof of death and representative authority (state law; counsel). Program terms on successors (contract, sponsor policy).
- **Legacy-only steps:** re-applying for successor privileges; manual suspension; mailing paper evidence.
- **Human judgment:** contested evidence, disputes, minors, the pending-grant policy (board), and approval of the split.
- **Eliminate:** successor re-application when the plan has already been approved (apply the plan deterministically after identity verification).
- **Run concurrently:** document verification, successor identity proofing and the allocation preview.
- **Deterministic automation:** ProtectiveHold on notice, suspension on verification, plan application, and the allocation algorithm (RR-UNB-SUCC-008-r1, RR-UNB-SUCC-004-r1).
- **Where AI helps:** extracting data from documents for human comparison.
- **Where AI is unacceptable:** declaring a death verified or choosing among competing claims.
- **Simpler model:** three events (hold, suspend, activate) instead of one long "succession case" queue.

#### W5. Dormancy (UNB-WF-015, ADM WF-10, LS WF-15)
- **Required outcome:** funds that are truly abandoned are distributed charitably under disclosed policy, and active donors are never surprised.
- **Why the current process exists:** annual outreach letters, with inconsistent enforcement.
- **Legal-obligation steps:** none identified for the fund balance itself. Inactivity rules are **sponsor policy** disclosed in program documents. Escheat applicability to balances is doubtful and needs counsel. Uncashed grant checks are a separate case (state unclaimed-property statutes; counsel).
- **Legacy-only steps:** annual letters to everyone; manual list reviews.
- **Human judgment:** the final disposition (A4).
- **Eliminate:** the manual dormancy review list. The clock and stage transitions are deterministic.
- **Run concurrently:** outreach across all verified channels and to all successors.
- **Deterministic automation:** clock resets, stage transitions, outreach scheduling and evidence capture.
- **Where AI helps:** drafting outreach and suggesting charities **only from the donor's own history**.
- **Where AI is unacceptable:** recipient steering in which the sponsor has a financial interest; deciding the disposition.
- **Simpler model:** RR-UNB-WF-015-r1.

#### W6. Fund closure and outbound transfer (ADM-025, UNB-WF-014, LS WF-14)
- **Required outcome:** every obligation is settled or carried as a liability, the residual goes to a determined destination, and history is preserved. A transfer reaches an eligible receiving sponsor and is reported as such.
- **Why the current process exists:** paper transfer forms, liquidation to cash, and history lost on transfer.
- **Legal-obligation steps:** a transfer to another sponsor is a distribution subject to the sponsor's grant controls (statute: the taxable-distribution regime; counsel to confirm classification). Annual reporting of grants (statute; Schedule D form).
- **Legacy-only steps:** forced liquidation when both custodians would accept in-kind; separate "closure" and "transfer" departments.
- **Human judgment:** approving the transfer (sponsor discretion); the trailing-receipt destination policy (OD-11).
- **Eliminate:** manual closure checklists. Closure becomes a state machine with machine-checked preconditions (UNB-RCN-ENT-006).
- **Run concurrently:** liquidation, history packaging and final fee calculation.
- **Deterministic automation:** DAF_TRANSFER_OUT tagging, closure preconditions and the outstanding-instrument liability.
- **Where AI helps:** assembling the history package.
- **Where AI is unacceptable:** approving the transfer.
- **Simpler model:** an outbound transfer is a grant with a purpose code, and a full transfer chains into closure.

#### W7. Institutional migration and sponsor exit (REQ-X-061, E2E WF-17)
- **Required outcome:** a single authoritative writer, reconciled history, no fabricated records, and no redirected payments.
- **Why the current process exists:** professional-services scripts.
- **Legal-obligation steps:** record retention (statute and regulation for tax records; contract; counsel). Data-processing termination duties (contract and privacy law).
- **Legacy-only steps:** charities re-submitting bank details; big-bang cutover without a parallel run.
- **Human judgment:** accepting breaks and authorizing cutover.
- **Eliminate:** charity re-submission when verification can reuse held evidence (TVF-244).
- **Run concurrently:** parallel run and payee re-verification.
- **Deterministic automation:** tie-outs, fencing and blocking imported_unverified payees.
- **Where AI helps:** proposing field mappings and explaining breaks.
- **Where AI is unacceptable:** accepting breaks.
- **Simpler model:** RR-REQ-X-061-r1, with exit through tenant-scoped key destruction (RCC-ENT-005).

#### W8. Regulatory reporting (REQ-X-060, E2E WF-14)
- **Required outcome:** accurate annual DAF data for each legal sponsor, reproducible as filed.
- **Why the current process exists:** spreadsheets pulled from ledgers and manual CPA tie-outs.
- **Legal-obligation steps:** the annual return with DAF reporting (statute and IRS form instructions). Signer responsibility (statute). Pending guidance under §§4966 and 6033 (IRS priority guidance; proposed regulation; watch item).
- **Legacy-only steps:** re-keying aggregates; separate compliance trackers (REQ-X-060 already forbids these).
- **Human judgment:** recognition policy (OD-19), account-type classification (counsel) and signature.
- **Eliminate:** manual aggregation.
- **Run concurrently:** state filings and federal packages from the same locked snapshot.
- **Deterministic automation:** the package build (RR-REQ-X-060-r1).
- **Where AI helps:** variance narratives.
- **Where AI is unacceptable:** classification, signature or filing.
- **Simpler model:** the package is a pure query over a locked period plus the classification register.

#### W9. Customer support and appeals (E2E WF-16, UNB-GAP-026)
- **Required outcome:** questions answered from authoritative state, decisions open to contest, and support unable to serve as an authorization bypass.
- **Why the current process exists:** contact centers re-keying requests.
- **Legal-obligation steps:** accessibility accommodation (statute applicability: counsel); privacy-request handling (statute, varies by state).
- **Legacy-only steps:** status-inquiry calls. These disappear when status is truthful and visible (UNB-CAP-003).
- **Human judgment:** appeals and hardship.
- **Eliminate:** staff entry of consequential changes without confirmation on the donor's verified channel.
- **Run concurrently:** not applicable.
- **Deterministic automation:** routing, and gating assisted changes on verified-channel confirmation (UNB-RCN-ENT-016).
- **Where AI helps:** grounded answers drawn from authoritative state.
- **Where AI is unacceptable:** closing cases on proposed actions, or identity decisions on voice or video (deepfake risk).
- **Simpler model:** self-service status plus a single appeal case type.



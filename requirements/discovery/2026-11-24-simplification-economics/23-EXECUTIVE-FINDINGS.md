# 23 — Executive findings

**Date:** 2026-11-24. **Status:** proposed. Nothing here is accepted, implemented, measured in operation, or reviewed by counsel, a CPA or an auditor.

**How the findings were produced**
- Ten role-separated passes by the same AI model family, including an independent adversarial pass.
- The economics come from an **executed, reproducible theoretical model**. Its outputs are not observations. About 60 of its 73 parameters are assumptions.
- The requirements gate remains **FAIL**.

## Update after the mission: SPD-ADV-001 decided

On 2026-11-24 the product owner decided SPD-ADV-001 (report 24):
- No team may exercise legally reserved authority for more than one sponsor.
- Every sponsor has its own decision team. It is either staffed by the sponsor or supplied by the operator and dedicated to that sponsor alone.
- Pricing carries a per-sponsor decision-team component that depends on which mode the sponsor chooses.

The executed re-run (`model/decision_teams.py`) changes the platform figures below:
- OM3@1M median: $125 → $147 per account.
- OM4@1M median: $29 → $39 per account.
- $100 is no longer reached by the full-service shared platform (OM3) at any modelled scale.
- $25 is no longer reached by the digital-minimal class (OM4).
- The two modes cost the enterprise about the same. They differ in who pays.

Where this report and reports 06–10 quote pooled OM3–OM5 figures, report 24 supersedes them.

## Bottom line

**What holds up**
- **The work.** Most of the work a DAF organization does can be eliminated, prevented or automated, but the remaining human work is narrow, legally required and stubborn.
- **The requirements.** They are mostly sound in *what* they protect, and heavy in *how often they say it*:
  - 475 of 1,487 records are restatements to consolidate;
  - only 9 are recommended for outright removal.
- **The largest operational saving.** It comes from redesigned charity vetting and from replacing per-item review with exceptions and sampling, not from AI.

**What does not hold up**
- **The economic targets are only partly reachable.**
  - $258 per account per year is reachable by a lean sponsor at about 100k accounts.
  - $100 is not reachable by any full-service model up to 5 million accounts. With per-sponsor decision teams (report 24), the shared platform does not reach it up to 10 million.
  - $25–50 is reachable only by a digital-minimal account class at very large scale. With per-sponsor teams, OM4 reaches $50 at 1M and no longer reaches $25.
  - $10 is not reachable at all in the modelled range.
- **The adversarial review weakened the best cases further.** Some of the strongest economic results depended on pooling legally reserved decision-makers across sponsors. The owner has since ruled that out (SPD-ADV-001, report 24).

## Answers to the twelve strategic questions

**1. What work can a DAF organization eliminate entirely?**
- **Restated requirements.** 475 records consolidate into surviving statements (report 02). One pass found only 156 of 382 records carry obligations of their own (SPF-ENT-001).
- **Per-item human review where a deterministic gate already decides.** This includes per-journal approval, NAV maker-checker on unchanged prices, and quarterly attestation of machine-checked controls. Removing NAV maker-checker or AI sign-off now needs an owner decision (SPF-ADV-012).
- **Annual re-attestations and re-acceptances that duplicate information already held.** Affiliation attestation becomes event-driven.
- **Manual 180-day charity revalidation.**
- **Configuration surfaced to sponsors.** 110 items shown at onboarding become 13 outcome questions plus 14 explicit values (report 12).
- **14 metrics with no decision behind them** (report 13).
- **Operator-ERP scope** that duplicates the sponsor's own systems (SPF-FIN-004). That work moves; it does not disappear.

**2. What work can be prevented through better design?**
- **Charity-vetting exceptions.** Prevent them by continuous monitoring and reused determinations (report 04):
  - evidence reuse across grants;
  - materiality-classified monitoring events;
  - reuse of sanctions false-positive decisions, limited to the deciding sponsor (SPF-ADV-007);
  - US intermediaries instead of per-grant equivalency determinations.
- **Payment exceptions.** Prevent them with ACH-by-default and typed return handling.
- **Contact demand.** Prevent it with proactive, reason-coded notices.
- **Configuration conflicts.** Prevent them with reviewed policy packs.
- **What matters most.** The executed model's sensitivity analysis ranks **exception share × exception minutes** as the dominant cost driver (report 09). Preventing exceptions matters more than processing them faster.

**3. What work can be automated safely?**
- **Deterministically, with no AI:**
  - eligibility gates for listed public charities;
  - payment and reconciliation matching;
  - period locks;
  - fee computation, chunked as measured;
  - evidence generation for controls;
  - most onboarding derivations.
- **With AI, and only where it explains, extracts, drafts or ranks:**
  - complaint and sentiment flags;
  - document extraction;
  - dossier summaries;
  - configuration explanations.
- **Never by AI** (reports 05 and 17):
  - posting;
  - clearing a hold;
  - approving a non-routine grant;
  - exercising sponsor discretion;
  - changing configuration without a human approval record.

**4. What work genuinely requires human judgment?** The taxonomy in report 05 defines nine intervention classes. The irreducible core is:
- non-standard recipients (expenditure responsibility, equivalency determination, foreign grants);
- true or unresolved sanctions matches;
- related-party and conflict holds;
- complex-asset acceptance;
- fraud and identity ambiguity;
- disputes and reconsiderations;
- discretionary board-policy holds;
- excise-tax determinations;
- period close and policy changes under dual control;
- incidents.

For charity vetting at 10k accounts, about 770 staff hours a year remain against about 10,700 implied by the requirements as written (revB). That is valued at $55.45/h; the adversarial review notes these dollar figures are about 1.86× too low at the $103/h rate used elsewhere (SPF-ADV-015).

**5. What is the theoretical minimum cost of operating a DAF organization?** These are executed model outputs, theoretical marginal floors (report 07):

| Additional unit | Theoretical floor | Practical floor | Driven by |
|---|---|---|---|
| Account | about $0.30 a year | about $3–20 a year | Expected human contacts and recurring obligations, not technology |
| ACH grant | about $0.46 | about $1.5–8 | Expected exception labour |
| Routine charity verification | $0.14 [S] | $0.14 | Reusable for every grant to the same organization |
| Non-standard verification | — | about $2,000 practical; about $160 per grant if reused | Reuse of the determination |
| Sponsor | about $5,000 a year | about $37,000–62,000 a year | Sponsor-level governance |

Fully allocated, a lean full-service model reaches about $110–121 per account at 5M accounts (P50). The adversarial variant puts a shared platform at $274 at 100k (report 22).

**6. What prevents us from reaching that cost?**
- **Sponsor fragmentation.** Accounts per sponsor is the largest swing factor in platform models: at 5,000 accounts per sponsor, sponsor-level cost is about $19 per account.
- **Irreducible exceptions.** Non-standard grants and complex gifts remain.
- **Per-sponsor decision authority.** Decided: legally reserved decisions are never pooled across sponsors (SPD-ADV-001).
  - Each sponsor carries a decision-team floor (2.5 FTE [A]). It binds below about 14,500 accounts per sponsor.
  - Small sponsors are therefore only economic if they staff the team themselves (report 24).
- **Acquisition.** About $60 per account, outside every target.
- **Service level.** Base figures assume about 80% SLA attainment.
- **Unmeasured inputs.** The two specialist pools, exception rates and minutes, and complex-gift hours have never been measured.

**7. Can small-balance DAF accounts become commercially sustainable?**
- **Only narrowly, on present evidence.** It requires a digital-minimal servicing class at several hundred thousand to a million accounts or more. That class needs at least about $25 per account of revenue, and adversarially about $53 of cost at 1M.
- **Acquisition breaks the case.** Its cost (about $60) is not covered by $9–30 of revenue.
- **A $12 employer fee alone never breaks even** in the model.
- **Tax law works against it.** The 2025 non-itemizer deduction excludes DAF gifts, so much small-balance growth would take share rather than expand the market (SPF-CMP-002).
- **Answer:** not as a standalone direct-to-consumer business. Possibly as a subsidised or embedded channel (employer or advisor) with explicit cross-subsidy. That is decision XID-ADV-002.

**8. Can we improve customer experience while reducing administrative costs?** Yes, where the improvement comes from not asking:
- prefilled, event-driven confirmations;
- grouped affirmations with evidence kept per statement, subject to sponsor counsel (SPD-CXP-001);
- a quarterly digest instead of monthly notices;
- proactive grant status.

No, where cost is cut by letting service levels slip. The base economics assume about 20% of items miss their SLA (SPF-ADV-005). The adversarial review also restored risk-triggered step-up for two interactions the friction pass had eliminated (SPX-003, SPX-027).

**9. What organizational structures become possible through AI-native operations?** A sponsor on shared services needs authority holders per reserved decision class, not departments:
- a cross-trained pool of 4–5 people covers every segregation-of-duties pair with absence cover;
- that is about 2.5 sponsor staff at 1k accounts (report 10);
- minimum-touch direct work at 100k accounts is about 34.5 FTE against 237 conventional ([D] from [A]).

Two caveats:
- The earlier "6–8 floor (TVC-018)" was a mis-citation. The mission carries several floors, and one roster-derived definition is required (SPF-ADV-014; UNB-SMP-ORG-002).
- Whether contracted or fractional people may hold legally reserved authority is open (SPD-ORG-002).

**10. Which advantages are inherent rather than temporary technology advantages?** Almost none are inherent today (report 14, Pass G):
- **Easily copied:** every documented technical advantage, including the ledger, contracts, AI fencing, SLOs, accessibility and portability.
- **Graded defensible:** only a pricing posture of not being paid on assets or through affiliates (SPC-114). That is an inference, and it holds only against incumbents.
- **Durable but not held:** distribution contracts, an assured operating record, executed migrations and scale.
- **Competitors are superior** in distribution, scale economics, live APIs and charity payment networks.

**11. Which existing requirements should be challenged, simplified, or removed?**

Register dispositions (report 02):

| Disposition | Records |
|---|---:|
| Retain | 560 |
| Consolidate | 475 |
| Simplify | 149 |
| Defer | 112 |
| Automate deterministically | 70 |
| Conditional on operating model | 68 |
| Exception-driven | 23 |
| Need evidence | 12 |
| Recommend removal | 9 |
| Replace | 6 |
| Automate with AI assistance | 3 |

The highest-value changes:
- **Collapse the control catalogs into one register** (SPF-PLT-002).
- **Make the SOC 2 report conditional** on an outside user entity existing (SPD-PLT-001).
- **Replace the 20 REQ-X scope statements with their carriers.**
- **Move per-item review to exceptions and samples.**
- **Apply operating-model conditions:** defer partner, white-label, multi-custodian and native-ERP scope.
- **Fix five contradictions:**
  - reconsideration window, 60 vs 90 days;
  - a reversed contribution creating a donor receivable;
  - statement cadence;
  - RR-REQ-X-051-r1 scope;
  - REQ-X-085..089 renumbering duplicates.

**12. What evidence would demonstrate that our entire proposed business model is wrong?**
- **Shadow replay of a real sponsor's grant history.** It would show exception share × minutes well above the model's lean assumptions (e·m above about 3.6, the baseline). That would put staffing at 100k near 69 FTE and remove the cost advantage (UNB-LSGAP-001).
- **Counsel answers.** Tier-1 policy approval is not permitted (RCD-005 Q1), or operator-dedicated principals cannot be appointed in any workable form (SPD-ORG-002). Minimum-touch then collapses toward conventional staffing. Pooling reserved decisions across sponsors is already ruled out by the owner (SPD-ADV-001).
- **No small-balance channel.** No channel covers acquisition plus about $25–53 of servicing cost per account.
- **Competitor parity.** A competitor with distribution reaches the same technology. The analysis finds nothing inherent to defend.
- **Measured database performance.** Production-like storage falsifies the architecture's falsification criteria. Measured headroom is currently 10–20×, so this is the least likely failure.

## What changed because of the adversarial review

Applied (report 22):
- **Orphaned consolidations fixed.** A validator gate (S7) now proves that every consolidation reaches a retained carrier. Three obligations are explicitly deferred with their capability.
- **Four control-weakening refinements** now require an owner decision.
- **Sanctions-disposition reuse** is limited to the deciding sponsor.
- **Automatic policy-pack updates** are limited to strictly tightening changes (SPR-ADV-001).
- **Duplicate $258 findings** are merged.
- **This mission's own additions are flagged for cuts:** 13 new requirements down to 7, about 15 fewer decisions, and release-1 metrics limited to the adopted six-metric catalog.
- **Platform cost bands** were marked not decision-grade until SPD-ADV-001 was decided. It is now decided, and report 24 gives the re-run figures.

## Corrections to earlier work

- **"$258" appears in no source.** It is the midpoint of an illustrative $243.84 and an assumption-driven $272 (SPF-ECN-001).
- **"6–8 (TVC-018)" was mis-cited** in the cross-industry reports 10, 11 and 14 (SPF-CLR-003, SPF-ADV-014).
- **REQ-X-085..089 and UNB-LSGAP-001..009** are post-baseline renamings of colliding definitions. All 14 distinct second sites are now reviewed (report 02, "Second definition sites").

## Decisions needed first

1. **SPD-ADV-001:** decided-provisional on 2026-11-24. Reserved decisions are never pooled; each sponsor has an operator-dedicated or self-staffed team. SPD-ECN-003 (per-sponsor pricing component) followed. Next comes setting the price levels and margin for components A, B, C1 and C2 (CFO and commercial lead, report 24).
2. **SPD-ECN-001 / XID-ADV-002:** target segment and the small-balance channel (employer, advisor or none).
3. **SPD-PLT-001:** is a SOC 2 report needed at release 1?
4. **SPD-ORG-001 / -002:** may the automated release gate count as a segregation-of-duties actor, and may fractional or contracted people hold reserved authority?
5. **SPD-ADV-002:** cross-sponsor reuse of sanctions dispositions.
6. **Measurement before more modelling:** a shadow replay of a real sponsor's grant history (UNB-LSGAP-001) and a time study of the two specialist pools.

Further investigation now hits diminishing returns. Every remaining material uncertainty needs either measured operating data or a decision by counsel, a CPA, the auditor or the owner.

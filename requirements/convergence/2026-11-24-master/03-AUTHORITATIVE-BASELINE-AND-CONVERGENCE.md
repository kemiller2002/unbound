# 03 — Authoritative baseline and requirements convergence (Phase 2)

**Outcome.** One authoritative baseline now exists: [`requirements/AUTHORITATIVE-BASELINE.md`](../../AUTHORITATIVE-BASELINE.md)
(index) with full rows in [`requirements/baseline/baseline.jsonl`](../../baseline/baseline.jsonl). Approved and
mission-adopted resolutions are incorporated into the normative requirement files as 63 convergence clauses
(`CNV-*`) in sections marked `<!-- BEGIN ADOPTED: convergence-2026-11-24 -->` in 11 domain files. All 316 original
questions are accounted for in the resolution log. No application code was written.

Reproduce (idempotent; a second run changes nothing):

```bash
python3 -I -B tools/reqaudit/adopt_convergence.py .
python3 -I -B tools/reqaudit/render_questions.py .
python3 -I -B tools/reqaudit/validate.py . && python3 -I -B tools/reqaudit/validate_discovery.py . && python3 -I -B tools/reqaudit/validate_simplification.py .
python3 -I -B tools/reqaudit/test_question_register.py .
```

All three earlier mission validators report 0 errors after adoption; the register reports 0 validation errors and
0 warnings; the regression tests pass.

## 1. How resolutions were incorporated (2.1)

| Step | Mechanism | Preserves |
|---|---|---|
| Author normative wording | 63 clauses in [`clauses/`](clauses/) (source) → [`data/convergence-clauses.jsonl`](data/convergence-clauses.jsonl). Each clause has SHALL statements, Given/When/Then acceptance criteria, the questions it resolves, the requirement IDs it amends, related reconciliation findings, owners, classification, release scope and activation gate. | Decision provenance (question IDs → resolution log → artifacts) |
| Place in normative files | One generated section per domain file; clauses govern where they conflict with earlier text in that file. | Original text unchanged; earlier adopted sections unchanged |
| Annotate originals | 21 Markdown definitions carry `> Converged 2026-11-24: governed by CNV-…` directly after the original line. | Original identifiers and locations |
| Update source records | 130 JSONL records in the three missions gained `converged_by` and a `revision_history` entry; their `statement` text is unchanged. | Original requirement text and revision history |
| Close questions | 316 `applied-to-requirement` events (one per resolved question) in `requirements/questions/resolution-events.jsonl`. | Reason for change, evidence references, supersession |

**Approval semantics.** 168 questions carry the product owner's delegated product-direction approval from the
2026-11-24 reviews; 113 carry *agent-recommended* evidence answers adopted as conservative positions pending owner
ratification; 35 are superseded by their canonical duplicate. No clause claims legal, CPA, bank, acquirer or auditor
approval; where such approval is required it is the clause's activation gate.

## 2. Conflicts resolved explicitly

Research contradicted several earlier records. Each correction is in a clause and supersedes the earlier wording.

| Earlier record | Problem found (evidence) | Resolution |
|---|---|---|
| UNB-XIN-ACC-002 AC-D2 (SponsorApproval not permitted) | Rested on Codification text no one read; four audited sponsors recognize grants at approval (batch B) | CNV-FIN-007: SponsorApproval permitted with a documented, auditor-accepted policy; communicated-at-approval stays default |
| UNB-XIN-ACC-001 (agency liability only without variance power) | FAS 136 treats a transfer naming the provider as beneficiary as a liability regardless of variance power | CNV-FIN-008 |
| RCD-018 (fee revenue when settled) | Fees charged to own funds are internal transfers; external fees accrue over the service period | CNV-FIN-002 |
| RCD-017 2% buffer, RCD-020 5% sample, XID-EVA-002 0.5%/5% materiality | No authority supports the numbers | CNV-FIN-001, CNV-FIN-004, CNV-FIN-011: method-based, numbers labelled illustrative |
| RCD-019 (5-day ACH hold as finality) | Consumer unauthorized returns 60 days; warranty claims up to two years | CNV-FIN-003 |
| RCD-028 (void after 180 days and re-credit) | Conflicts with RCD-047 and UCC 3-310(b); narrowest RUUPA void window 90 days | CNV-PAY-002, CNV-PAY-003 |
| RCD-015 (every true match → Blocked) | OFAC FAQ 5: block, reject or proceed under licence | CNV-GRT-006 |
| RCD-044 (automatic reject report and reservation release) | Pre-approval refusal resembles FAQ 53; release only if related parties clear | CNV-GRT-006 |
| RCD-007 OFAC retention from screening decision | 31 CFR 501.601 (as amended 2025): 10 years from transaction; blocked property until 10 years after unblocking | CNV-PLT-004 |
| XID-NPG-007 review ("do not assert abatement") | §4962(b) and Form 4720 instructions make abatement available on request (discretionary) | CNV-GRT-010 |
| XID-INV-003 (multi-sponsor pools outside the exclusion) | ICA §3(c)(10)(B)(i) covers funds of one or more charitable organizations | CNV-LEG-001: lawful; deferred as a business choice |
| XID-SAS-005 (white-label may launch in California) | Gov. Code §12599.9 Type E covers SaaS and white-label platforms | CNV-LEG-002: register the operator (actual external authorization) |
| XID-CLM-002 (FINRA 2165 allows 15+10 days) | Rule allows a further 30 business days since 2022 | CNV-ENT-001 (analogy only; legal basis is program terms) |
| RCD-022 (suspend fund on death) | Major sponsors suspend only the deceased's privileges | CNV-ENT-001 |
| XID-CRM-001 (sender = From line) | Sender is the party whose product is promoted; 316.2(m) only for multiple advertisers | CNV-PLT-001 |
| RCD-014 (private foundations as ER-required) | Private operating foundations are excepted §170(b)(1)(A) organizations | CNV-GRT-002 |
| XID-INV-008 (type 1 at launch, type 2 within 12 months) | No source; reliance follows the user auditor's approach | CNV-GOV-001 |
| RR-ADM-023, UNB-XIN-EVA-012 (any spouse may instruct) | Corrected in the adversarial review; authority comes from the recorded agreement | CNV-GRT-016 |

Conflicts **not** resolved and explicitly retained: none at requirement level. Three items remain dependent on
external facts rather than in conflict: whether the California Registry exposes data for CNV-GRT-015, whether each
sponsor's auditor accepts deterministic release (CNV-GOV-003), and FOCUS provider versions (CNV-ECO-001).

## 3. Register governance (2.2)

Implemented and tested (report 01 section 4): stable IDs; exact text pinned by SHA-256; source record and path; resolution
status; recommended answer; approval status; evidence references; remaining dependencies (`activation_gate`,
`external_authorization_required`); full revision history (event log); reconsideration triggers (from dossiers); and
validation for orphaned answers, duplicate IDs, missing sources, changed text, stale resolutions, conflicting
dispositions and invalid closure. Final register state:

| Resolution status | Approval | Questions |
|---|---|---|
| resolved | approved-product-direction | 168 |
| resolved | agent-recommended (owner ratification pending) | 113 |
| superseded-by-duplicate | agent-recommended | 35 |

| Evidence disposition (all 316) | Count |
|---|---|
| business-policy-decision | 211 |
| reasoned-interpretation | 54 |
| established-by-authoritative-evidence | 24 |
| external-authorization-required | 15 |
| empirical-validation-required | 12 |
| unresolved | 0 |

"Business-policy decision" dominates because most register questions asked for a product default, not a legal fact.
The 15 external-authorization questions each name the specific authorization (board resolution, auditor planned approach,
California registration, executed agreements), never a generic "consult counsel". Live table: [`../../OPEN-QUESTIONS.md`](../../OPEN-QUESTIONS.md).

## 4. Ownership (2.3)

Every clause names a domain owner, financial system of record, policy owner, data owner, operational owner and evidence
owner (`owners` in `data/convergence-clauses.jsonl`, rendered under each clause). Principles applied:

- **Policy owner** is a sponsor role wherever the decision is legally the sponsor's (board, DoA officer, controller, CFO,
  treasurer, compliance officer); Unbound owns platform policy only (architecture, security, product, regulatory perimeter).
- **Financial system of record** is stated per function: Unbound program ledger, the sponsor GL in Shadow mode, the bank,
  the custodian, the rail partner (CNV-FIN-012 function map).
- **Data owner** is the sponsor for tenant data; Unbound for platform metering and the public-fact registry.
- **Operational owner** is whoever employs the people (sponsor or administrator, SPD-ADV-004).
- **Evidence owner** is the role accountable for producing evidence to auditors (usually controller or compliance officer).

The baseline index adds a domain owner to all 1,619 rows by identifier family.

## 5. Normalization (2.4)

- Clauses are atomic per capability with testable acceptance criteria; where the source had invented numbers, the clause
  defines the method for determining the number and labels any shipped value illustrative (buffer, sampling, materiality,
  tolerances, privacy thresholds, SLOs).
- Numbers that rest on authority are stated with that authority (Nacha 2 years and 2×/180 days; OFAC 10 years and 10 business
  days; NOC 6 banking days; 90-day void window as the narrowest RUUPA bracket).
- Numbers that are business-policy defaults are labelled as such and configurable (60/30-day reconsideration; 90-day successor
  window; 15/10/30 business-day protective suspension; 2/15/60-day complaints; 90-day maximum discretionary hold; 18-month API
  deprecation; 12-month false-positive reuse backstop).
- Separation of normative requirements from other material: the baseline classifies rows as normative, research hypothesis
  (UNB-HYP/GAP/COM), capability-index candidate (REQ-X-058..077, pending carrier verification per SPD-ENT-001) and evidence
  backlog (CNV-EVB-001). Design alternatives and rejected options remain in their mission reports (report 09 consolidates).

## 6. Release scope (2.5)

| Release scope | Meaning | Clauses |
|---|---|---|
| architectural-support | Designed and built into the architecture; not a user-facing commitment | 2 |
| initial-implementation | First implementation | 47 |
| sponsor-activation | Built, switched on per sponsor once its authorization exists | 7 |
| commercial-availability | Offered commercially only after contract and assurance conditions | 2 |
| regulatory-authorization | Requires an actual external authorization before activation | 1 |
| deferred | Architecturally supported, disabled (cards, foreign grants and ED, call recording, multi-sponsor pools) | 4 |

The expansion path keeps adapters and disabled states for deferred features (card rail, instant rails, direct foreign
grants, call recording, multi-sponsor pools, staffed services) so that enabling one needs configuration, authorization and
tests, not re-architecture.

## 7. Phase 2 gate

| Criterion | Status |
|---|---|
| One authoritative baseline | Met (`AUTHORITATIVE-BASELINE.md`, `baseline/baseline.jsonl`) |
| All 316 questions accounted for | Met (resolution log; 0 open) |
| Approved resolutions incorporated into source requirements | Met (63 clauses; 21 annotated definitions; 130 records revised) |
| Conflicts resolved or identified | Met (section 2) |
| Register reflects actual decision status | Met (generated from the log; approval status distinguishes owner, delegated and agent-recommended) |
| No application implementation | Met |

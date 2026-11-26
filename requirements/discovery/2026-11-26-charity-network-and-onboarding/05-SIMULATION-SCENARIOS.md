# 05 — Future simulation scenarios

Status: **proposed**, not simulated. Format follows `requirements/simulation/INITIAL-WORKFLOW-FAILURE-SCENARIOS.csv`, and failure modes use `requirements/simulation/FAILURE-MODE-TAXONOMY.csv`. Source: [`data/simulation-scenarios.csv`](data/simulation-scenarios.csv).

These scenarios belong in the whole-enterprise simulation framework (`requirements/simulation/WHOLE-ENTERPRISE-WORKFLOW-SIMULATION-FRAMEWORK.md`) once adopted. Many can run before any customer exists:
- public-data replay over archived IRS and OFAC releases;
- synthetic legacy books with seeded errors;
- red-team runs drawn from the daf-landscape threat model.

| ID | Workflow | Simulated fault | FM | Expected handling | Requirements |
|---|---|---|---|---|---|
| SIM-CNO-001 | Charity monitoring | August auto-revocation batch posts about 40k revocations, 88 days after their effective dates; pending and recurring grants exist | FM-18 | Re-evaluate affected grants before next release; hold adverse ones; reliance reasoning uses publication time; no mass false holds outside the affected EINs | UNB-CNET-001, UNB-CNET-007 |
| SIM-CNO-002 | Charity monitoring | BMF release corrupted (20% of c3 subsection codes changed) | FM-02 | Release quarantined by circuit breaker; prior release stays current within freshness limit; no eligibility changes | UNB-CNET-021, UNB-CNET-003 |
| SIM-CNO-003 | Charity monitoring | IRS publishes late; no new Pub 78 for 40 days; Unbound holds latest | FM-03 | Treated as current; payments not held; staleness only when Unbound lags a newer release | UNB-CNET-004 |
| SIM-CNO-004 | Eligibility | EIN on revocation list without reinstatement but present in Pub 78 and BMF with later ruling date | FM-02 | StatusConflict; ReRecognised class handled by approved rule; ambiguous class held for review | UNB-CNET-005 |
| SIM-CNO-005 | Eligibility | Grant to a group-exemption subordinate whose central roster has expired | FM-01 | InsufficientEvidence, not Ineligible; roster request to central; cached evidence reused once renewed | UNB-CNET-006 |
| SIM-CNO-006 | Eligibility | Retroactive reinstatement row arrives for a charity with held grants | FM-06 | Re-evaluation triggered; hold remains until subsequent listing or determination letter; original hold history unchanged | UNB-CNET-008 |
| SIM-CNO-007 | Sanctions | OFAC update at 10:00 adds a name matching a scheduled grantee | FM-18 | Full reload; rescreen before next release; grant held under sanctions disposition; never auto-released | UNB-CNET-009 |
| SIM-CNO-008 | Charity data | Charity website contains instructions to change bank details; AI extraction runs | FM-08 | Proposal only; no destination or contact change; deterministic corroboration required | UNB-CNET-022 |
| SIM-CNO-009 | Payments | Charity email compromised; contact changed then bank change requested within 30 days | FM-04 | Changed contact ineligible for callback (c10); post-change hold (c11); D3 tier; first-payment cap; probation | UNB-CNET-015, CNV-PAY-002, CNV-PLT-011 |
| SIM-CNO-010 | Payments | Partner rail (Chariot) outage with instructions in flight; partner later announces change of control | FM-10 | Unsent instructions move to fallback only after non-execution confirmed (CNV-PLT-012/CNV-PAY-004); second rail certified; change-of-control termination right exercisable | UNB-CNET-018, UNB-CNET-026 |
| SIM-CNO-011 | Payments | Grant routed to a pass-through intermediary rail for a named grantee | FM-26 | Activation refuses route; legal payee preserved | UNB-CNET-013 |
| SIM-CNO-012 | Tenancy | Shared-store inspection while two sponsors fund the same charity | FM-15 | No record links sponsors to charity in shared stores; inference tests pass | UNB-CNET-010 |
| SIM-CNO-013 | Migration | Incumbent permits a single export at termination with deletion 10 business days later | FM-10 | Single-extract variant; vault capture before deadline; retrospective parallel run; clock starts at usable extract | UNB-ONB-002, UNB-ONB-009 |
| SIM-CNO-014 | Migration | AI-proposed mapping swaps two restriction codes but preserves totals | FM-07 | Attribute-level sample and invariants fail; mapping not pinned; trial blocked | UNB-ONB-005, UNB-ONB-013 |
| SIM-CNO-015 | Migration | Re-run of identical manifest produces different output hash | FM-13 | Run fails; promotion blocked; nondeterminism investigated | UNB-ONB-010 |
| SIM-CNO-016 | Migration | Fund legacy activity does not roll to closing balance by $12.40 | FM-17 | HistoryGap recorded and disclosed; no plug entry; blocks cutover if above tolerance | UNB-ONB-007, UNB-ONB-013 |
| SIM-CNO-017 | Migration | Unit pool value differs by $0.01 after migration | FM-17 | Largest-remainder allocation; classified as rounding; gate passes with evidence | UNB-ONB-013 |
| SIM-CNO-018 | Cutover | Grant approved in legacy system and present in package; both systems attempt release | FM-12 | Ownership register and legacy-reference idempotency key prevent duplicate; one release only | UNB-ONB-018 |
| SIM-CNO-019 | Cutover | Check printed before first live posting; rollback requested | FM-27 | Rollback refused past irreversibility boundary; void with positive-pay update as compensating process | UNB-ONB-017 |
| SIM-CNO-020 | Cutover | Imported bank destinations (5,000) with 52% verified by dollars against a 60% minimum | FM-01 | Cutover authorisation refused or sponsor accepts higher check volume; unverified paid by check | UNB-ONB-021, UNB-CNET-016 |
| SIM-CNO-021 | Cutover | Freeze overruns planned 3 business days; donors submit recommendations during freeze | FM-20 | Recommendations queued, not discarded; sponsor decision and donor notice on overrun; re-authorisation at release | UNB-ONB-016 |
| SIM-CNO-022 | Configuration | Configuration package names Unbound as payment Originator | FM-06 | Validation refuses activation citing CNV-PAY-001/CNV-LEG-001; prior package remains | UNB-ONB-023 |
| SIM-CNO-023 | Integration | Custodian adapter fails duplicate-submission test after version change | FM-12 | Certificate not reissued; execution access suspended; reads continue | UNB-ONB-025 |
| SIM-CNO-024 | Participants | Migrated advisor delegation and donor first login after cutover | FM-05 | Delegation shown pending confirmation; active after confirmation or policy period; no knowledge-based proofing | UNB-ONB-022 |
| SIM-CNO-025 | Exit | Terminated sponsor exports all records on day 60 with an unpaid disputed invoice | FM-25 | Full export produced; not conditioned on payment; validates against schema | UNB-ONB-029 |

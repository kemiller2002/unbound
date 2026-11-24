# 17 — Workflow validation and surface inventory

Date: 2026-11-23 | Owner question: *"Did you validate all the workflows?"* Answer before this report: no; workflows had been challenged (report 09) and scenarios written (report 12), but no state machine had been checked for completeness. This report closes that for the **26 release-1 workflows** (RCD-013) and derives the screen inventory from them.

Models: [`workflows/WFM-*.json`](workflows/) (schema: [`workflows/SCHEMA.md`](workflows/SCHEMA.md)) · checker: `tools/reqaudit/workflows.py` · results: `data/workflow-validation.json` · surfaces: `data/surface-inventory.csv`, `data/surface-ia.csv` · modelling reports: [`workflows/REPORT-GRT.md`](workflows/REPORT-GRT.md), [`REPORT-FIN.md`](workflows/REPORT-FIN.md), [`REPORT-ENT.md`](workflows/REPORT-ENT.md)

## 1. Method

Each workflow is modelled as data from the adopted requirements, the decided options (report 14), the prior state models and the transition matrix (TX-01..27). Each state carries its user label and status lozenge. Each transition records the actor type and role, the authority level (A0–A4), guards, effects, the idempotency key, failure outcomes, and the requirement and scenario it traces to. For human transitions it also names the UI surface. The checker then tests every model:

| Check | Rule |
|---|---|
| W1 | Identifiers unique; referenced states exist |
| W2 | One initial state; every state reachable |
| W3 | No dead ends; every non-terminal state can reach a terminal state |
| W4 | Terminal states change only through linked corrections |
| W5 | Every transition has an actor, an authority level and a requirement that resolves |
| W6 | Authority within actor ceiling (AI ≤ A2; deterministic, timer and external ≤ A3; A4 human only; A5 never) |
| W7 | Money-moving and approval effects carry mandatory guards. Payment out requires current eligibility, sanctions clear, current hold watermark, verified destination and funded. Approval requires eligibility, sanctions and segregation of duties. Credit requires a final contribution. Acknowledgment requires a completed gift and sponsor acceptance. |
| W8 | Every external interaction has an Unknown or timeout outcome |
| W9 | Consistent state labels across workflows |
| W10 | Every human transition names a surface |
| W11 | Scenarios referenced exist (warning when a guarded transition has none) |

Three role-separated agents (grants and payments, finance, enterprise and platform) wrote the models against one brief. The integrating review ran the checker across all 26 models together.

## 2. Result

**26 workflows · 401 states · 519 transitions · 0 errors · 128 warnings.** All warnings are W11: guarded or failure-bearing transitions without an adversarial scenario yet. 301 of 519 transitions (58.0%) cite at least one scenario.

<!-- BEGIN GENERATED: workflows -->
| Workflow | Name | States | Transitions | Human | Deterministic/timer | External | AI | With scenario |
|---|---|---|---|---|---|---|---|---|
| WFM-01 | Legal sponsor onboarding, tenant provisioning and program activation | 22 | 23 | 13 | 9 | 1 |  | 11 |
| WFM-02 | Fund application, sponsor acceptance and participant onboarding | 21 | 22 | 14 | 6 | 2 |  | 10 |
| WFM-03 | Cash contribution (ACH, wire, check) | 22 | 29 | 9 | 13 | 7 |  | 27 |
| WFM-04 | Publicly traded securities contribution (DTC) | 25 | 34 | 10 | 17 | 7 |  | 26 |
| WFM-05 | Complex asset via partner intermediary (cash delivered) | 19 | 20 | 8 | 7 | 5 |  | 7 |
| WFM-06 | Charity identity, status verification and holds | 18 | 29 | 14 | 13 | 2 |  | 16 |
| WFM-07 | Grant recommendation (donor and advisor side) | 11 | 17 | 9 | 8 |  |  | 7 |
| WFM-08 | Grant approval, reservation and funding (sponsor side) | 21 | 35 | 16 | 18 | 1 |  | 20 |
| WFM-09 | Recurring grant schedule and occurrences | 9 | 14 | 6 | 8 |  |  | 9 |
| WFM-10 | ACH grant payment via partner rail | 16 | 22 | 5 | 11 | 6 |  | 12 |
| WFM-11 | Check grant payment with positive pay | 18 | 23 | 9 | 11 | 3 |  | 10 |
| WFM-12 | Sanctions screening, disposition and blocking | 10 | 16 | 8 | 8 |  |  | 7 |
| WFM-13 | Charity payment destination change | 12 | 15 | 8 | 5 | 2 |  | 7 |
| WFM-14 | Pool valuation and unit pricing | 17 | 18 | 7 | 8 | 3 |  | 17 |
| WFM-15 | Fee accrual and settlement | 9 | 13 | 6 | 7 |  |  | 10 |
| WFM-16 | Reconciliation, month-end close and fund statements | 13 | 16 | 5 | 10 | 1 |  | 16 |
| WFM-17 | Annual DAF reporting (Schedule D Part I support) | 16 | 19 | 12 | 7 |  |  | 8 |
| WFM-18 | Document issuance, delivery and retention | 15 | 18 | 6 | 11 | 1 |  | 16 |
| WFM-19 | Delegated authority: grant, change scope, revalidate, revoke and expire | 15 | 22 | 12 | 9 | 1 |  | 11 |
| WFM-20 | Death or incapacity notice, rights suspension and succession | 17 | 20 | 10 | 9 | 1 |  | 14 |
| WFM-21 | Fund dormancy, outreach and charitable default | 9 | 11 | 4 | 7 |  |  | 5 |
| WFM-22 | Fund closure, residual disposition and outbound transfer to another sponsor | 13 | 17 | 5 | 12 |  |  | 11 |
| WFM-23 | Rule-created exception work item | 12 | 16 | 6 | 8 | 1 | 1 | 4 |
| WFM-24 | Sponsor policy version lifecycle | 13 | 16 | 10 | 6 |  |  | 6 |
| WFM-25 | Support case, assisted change and reconsideration of a sponsor decision | 11 | 18 | 12 | 5 |  | 1 | 4 |
| WFM-26 | Inbound migration of an existing sponsor book | 17 | 18 | 10 | 7 | 1 |  | 11 |
<!-- END GENERATED: workflows -->

Actors across all 519 transitions:

| Actor | Count |
|---|---:|
| Human | 232 |
| Deterministic | 182 |
| Timer | 58 |
| External party | 45 |
| AI | 2 |

By authority level: A0 1, A1 35, A2 96, A3 264, A4 123. Only 4 transitions move money out of the platform, and each carries all five mandatory guards (W7).

**What "0 errors" does and does not mean.** Every modelled workflow is structurally complete, authority-consistent and guarded. That is not the same as correct. The models encode the requirements and decisions as they stand; legal positions remain provisional (report 14). Only executing the scenarios against an implementation can show behaviour. Models are only as complete as the transitions the agents found, and gaps found while modelling are recorded in §4.

**Design pressure.** 232 of 519 transitions (44.7%) are performed by people, and 123 of them are A4. Most sit on exception paths: holds, matches, unknown outcomes, disputes. Some sit on routine paths only because a decision keeps a human there until validation. Examples are batch grant approval until delegation (RCD-005) and NAV checking until the investment committee delegates (RCD-035). The next refinement pass should split human transitions into routine and exception, and challenge every routine one.

## 3. Workflow defects found and how they were resolved

The modelling reports give full evidence. Each defect is a finding (RCF-WF-001..003) and is resolved by a decision (RCD-032..049, report 14) and/or a gap requirement (§4).

| Defect | Resolution |
|---|---|
| Death rule still held pending grants, contradicting RCD-022 | RCD-039: `RR-REQ-X-008-r1` amended; scenario DKS-ENT-003 expected state amended |
| Provisional credit still allowed by `RR-UNB-ACC-006-r1`, contradicting RCD-019 | RCD-033: amended out |
| Prior model allowed "approved awaiting capacity" (approval without reservation) | RCD-049: removed. Approval and reservation are atomic. `RR-REQ-X-002-r1` uses the canonical funding state names. |
| ACH acknowledgment timing conflict | RCD-032: sponsor setting, default after final |
| Intermediary role undecided | RCD-034: donee model for release 1 (counsel to confirm) |
| Every NAV needed a human checker | RCD-035: in-tolerance NAVs published by delegated policy with sampled review |
| Fee versus reserved capacity undecided | RCD-036: reserved grants first; fee shortfall carried |
| Attestation and statement approval ambiguity | RCD-037, RCD-038 |
| Closure versus transfer order; money arriving for closed funds | RCD-040 |
| No revocation time bound | RCD-041: next authorization check, at most 60 seconds |
| Unknown payments could never close | RCD-042: dual treasury resolution after the final aging tier |
| Recurring threshold ambiguity | RCD-043 |
| True sanctions match before approval | RCD-044: refuse, release reservation, report |
| Charity could not decline a grant | RCD-045: action added to S-012 |
| Timing parameters unset (capacity wait, drafts, intents, invitations, appeals, cooling-off, sanctions SLA) | RCD-046: defaults |
| Void versus escheat | RCD-047: escheat determination precedes re-credit |
| NOC versus pinned destinations | RCD-048 |
| Prior-model errors: deep-review expiry going straight to a hold (TX-27); Unknown merged with Accepted (TX-13) | Corrected in WFM-06 and WFM-10 |

## 4. Requirement gaps added

30 gap requirements were created and adopted provisionally into the domain files: `UNB-RCN-WF-001..030`, origin gap-analysis, each with transitions, Given/When/Then criteria and links. P0 items:
- Unknown protocol for inbound money (WF-001)
- Activation gate (WF-009)
- Pausing contribution debits on a death notice (WF-012)
- Revocation bound (WF-013)
- Routing money for closed funds (WF-015)
- Reverting to human approval when a board delegation lapses (WF-016)
- Migration fence Unknown (WF-020)
- Unknown payment fallback (WF-022)
- Manual release limits (WF-023)
- OFAC rejected-transaction reporting (WF-024)
- Destination cooling-off (WF-028)

The full register is in [04](04-MISSING-REQUIREMENTS-REGISTER.md).

## 5. Surface inventory and information architecture

Every human transition names a surface. The workflows need **66 surfaces**: 13 existing screens plus the widget, and 53 new. Treating 66 surfaces as 66 screens would recreate the department-and-queue sprawl report 10 warned against. They are therefore organised into **22 workspaces**, in which most new staff surfaces become typed decision panels inside the case workspace or a small number of operational workspaces:

<!-- BEGIN GENERATED: ia -->
| Workspace | Name | Audience | Responsive | Views and panels (surface → transitions, max authority) |
|---|---|---|---|---|
| WS-01 | Giving home and fund | Donor, advisor, successor | phone-complete | Giving home (0, A0) |
| WS-02 | Grant recommendation and grants | Donor, advisor | phone-complete | Recurring schedules (4, A2); Charity search (0, A0); Recommend a grant (4, A2); Grant detail (5, A2) |
| WS-03 | Contributions | Donor, advisor | phone-complete | Add money and contribution status (6, A1) |
| WS-04 | People, access and succession | Donor, representatives | phone-complete | Accept invitation (3, A1); Representative evidence (1, A1); Confirm a change on your verified channel (2, A2); Participants and successors (6, A2) |
| WS-05 | Documents and notices | Donor, advisor, charity | phone-complete | Document center (0, A0); Notification preferences (0, A0) |
| WS-06 | Fund opening and lifecycle requests | Applicant, donor | phone-complete | Open a fund (2, A1); Close a fund (1, A1); Transfer to another sponsor (1, A1) |
| WS-07 | Charity portal | Charity finance user | phone-complete | Grants received and destinations (3, A2) |
| WS-08 | Help and support status | Donor, advisor, charity | phone-complete | My requests and appeals (3, A1) |
| WS-09 | Exception inbox | Sponsor operations, compliance, finance, treasury | phone-review-and-approve | Inbox (4, A2) |
| WS-10 | Case workspace with typed decision panels | Reviewers and approvers | desktop-first; approve/decline on phone | Panel: charitable default (5, A4); Panel: alternate eligibility evidence (2, A3); Panel: charity hold (6, A4); Panel: charity identity (2, A3); Panel: destination verification (5, A3); Panel: fee waiver (4, A4); Panel:… |
| WS-11 | Grant approval | Grants approver | phone-review-and-approve | Batch approval (1, A4) |
| WS-12 | Treasury and payments | Treasury | desktop-first | Checks: void and reissue (2, A3); OFAC reports (1, A3); Manual release approval (1, A4); Checks: unclaimed property (3, A3) |
| WS-13 | Fund accounting and close | Controller, fund accountant | desktop-first | Noncash forms: 8282 (1, A4); Noncash forms: 8283 (2, A4); Close certification (3, A4); Unit pricing (4, A4); Pool liquidity (1, A4); Reconciliation and attestation (1, A4) |
| WS-14 | Regulatory reporting | Controller, CFO, signer | desktop-first | Account-type classification (1, A4); Annual DAF package (10, A4) |
| WS-15 | Records and retention | Records owner, counsel | desktop-first | Legal holds (2, A4); Disposition batches (1, A4) |
| WS-16 | Fund lifecycle operations | Sponsor operations | desktop-first | Closures and transfers (1, A4) |
| WS-17 | Sponsor implementation and activation | Implementation lead, sponsor officer | desktop-first | Board delegations (2, A4); Opening balance tie-out (2, A4); Activation gate (3, A4); Contract and activation record (5, A4); Readiness checklist (1, A2); Tenant provisioning (1, A2) |
| WS-18 | Migration | Migration lead, controller | desktop-first | Imported payee verification (1, A4); Migration runs (5, A4); Cutover authorization (2, A4); Mapping review (1, A4) |
| WS-19 | Sponsor policy and configuration | Sponsor administrator, approver, platform risk | desktop-first | Policy draft, diff and approval (9, A4) |
| WS-20 | Support console | Support agent | desktop-first | Assisted change (2, A2) |
| WS-21 | Sponsor overview | Sponsor executive | phone-review-and-approve | Outcomes at risk (0, A0) |
| WS-22 | Embedded status widget | Partner host end users | phone-complete | Grant status card (0, A0) |
<!-- END GENERATED: ia -->

Reading the table:
- **Typed panels inside the case workspace (WS-10).** Eighteen decision surfaces (sanctions match, charity hold, unknown payment, gift acceptance and others) are panels inside one case frame (S-009). Each needs its own evidence layout and decision options, which S-009 alone did not specify. The case workspace carries 64 transitions from 16 workflows, which confirms it must be a frame with typed panels, not one generic screen.
- **Operational workspaces (WS-12..WS-18).** These group month-end, treasury, reporting, retention, lifecycle, implementation and migration work by goal and period, not by department.
- **Donor, advisor and charity side (WS-01..WS-08, WS-22).** These stay on the screens already specified. New additions: recurring schedules, invitation acceptance, representative evidence, verified-channel confirmation, fund opening, closure and transfer requests, and support status.

## 6. Next

1. Specify the new workspaces and panels to the UI requirement standard (report 16): Forma components, states, actions with authority, responsive class and acceptance criteria. Then check them with gate G12.
2. Prototype the release-1 subset on Forma, checked with axe and screenshots, and file any new Forma gaps.
3. Write scenarios for the 128 W11 transitions, starting with the guarded money paths.
4. Split human transitions into routine and exception, and challenge every routine one (§2).

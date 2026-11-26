# 05 — Reconciliation and architecture check

Package: [README](README.md) · Source data: [`data/approved-decisions.csv`](data/approved-decisions.csv), [`data/constraint-checks.csv`](data/constraint-checks.csv).

## Rules applied

- A ratified convergence clause, an owner decision in `requirements/convergence/2026-11-24-master/data/owner-decisions.jsonl` and the generated `AUTHORITATIVE-BASELINE.md` are **not** edited by this package. Where an approved Phase II decision would need ratified text to change, the ratified text governs now and a new owner decision (OD-CDD-*) is raised.
- Historical records are preserved: open decisions (OD-CNO-*) and recorded answers (e.g. FIL-OD-10) are cited, not rewritten.
- **No change to architecture is made.** Every constraint below is checked against the requirements in this package; each states its effect as "no change".
- No requirement here is implemented, validated or implementation-authorised, and nothing here authorises expenditure, contracts, production deployment or the removal of a mandatory control.

## Points of note outside the approved decisions

- **UNB-ONB-015 vs CNV-GOV-001.** The charity-network package proposes a service-organisation report "Type I at go-live", while ratified CNV-GOV-001 requires no stand-alone type 1 unless a contract requires it. This package does not resolve that pre-existing tension; CNV-GOV-001 governs.
- **CNV-ECO-004 concentration.** The research notes that every single-customer start breaches the 25% cap without a transition policy. The clause applies the cap after year 3 with a board-approved exception record, so it already has a mechanism; the partner-channel variant is raised as OD-CDD-05.

<!-- BEGIN GENERATED -->
## Reconciliation with ratified clauses and owner decisions

| Decision | Ratified or existing text touched | Text that governs now | Reconciliation | Owner decisions raised | Related open decisions |
|---|---|---|---|---|---|
| DEC-001 | CNV-ECO-004 | CNV-ECO-004 as ratified (OD-M-05) | Conflicts with the CNV-ECO-004 entry-route wording; escalated, not amended. Concentration under a partner channel escalated separately. | OD-CDD-01; OD-CDD-05 | — |
| DEC-002 | CNV-ECO-004; CNV-GOV-001; CNV-LEG-001 | CNV-LEG-001 (Unbound holds no funds); OD-M-08 and CNV-ECO-004 as ratified | Consistent with CNV-LEG-001 (Unbound is never custodial). Overlap with the OD-M-08 staffed-administration second offer escalated. | OD-CDD-04 | — |
| DEC-003 | CNV-GRT-003; RR-REQ-X-007-r1 | CNV-GRT-003 as ratified; RR-REQ-X-007-r1 (adopted-provisional) | The 180-day ceiling is preserved. Whether a change-driven renewal counts as the deep review is escalated; until decided the deep review follows RR-REQ-X-007-r1. | OD-CDD-02 | OD-CNO-04; OD-CNO-13 |
| DEC-004 | — | CNV-GRT-004 and UNB-CNET-011 (consistent) | Consistent with CNV-GRT-004 and OD-P-GRT cut F; refines UNB-CNET-011. | — | OD-CNO-05; OD-CNO-09 |
| DEC-005 | — | CNV-AIO-001 and CNV-AIO-002 (consistent) | Consistent with CNV-AIO-001/002, UNB-CHAR-022 and UNB-CNET-022; refines them. | — | — |
| DEC-006 | — | OD-M-11 (board sets licence prices); OD-CNO-10 open; FIL-OD-10 as recorded | No ratified clause sets a 20 bps Unbound fee; existing open and agent-recommended records that reason from ~20 bps are escalated, not edited. | OD-CDD-03 | OD-CNO-10 |
| DEC-007 | — | UNB-ONB-002 and UNB-TRN-006 (proposed) | No conflict; refines UNB-ONB-002. | — | OD-CNO-01; OD-CNO-07 |
| DEC-008 | — | Not applicable (business-plan decision outside product requirements) | Capital decisions are outside Unbound requirements; Release-1 scope is unchanged by this package. | — | — |
| DEC-009 | — | CNV-PAY-001, CNV-EXP-002 and CNV-LEG-001 (consistent); UNB-CNET-019 (proposed) | Already satisfied by ratified CNV-PAY-001 (partner rail or sponsor ACH, check fallback) and UNB-CNET-019 (no-portal receipt). | — | OD-CNO-02 |
| DEC-010 | — | UNB-ONB-030 (proposed) | Refines UNB-ONB-030; no ratified text touched. | — | OD-CNO-08 |
| DEC-011 | — | UNB-TRN-001..014 (proposed) | Consistent with the 2026-11-26 Transitus integration package. | — | — |
| DEC-012 | — | Not applicable | No conflict. | — | — |

## Architecture and platform constraints

| Constraint | Governing ids | Source | Effect of this package | Evidence |
|---|---|---|---|---|
| F# implementation language | UNB-TEC-002; UNB-PLAT-006; CNV-SVC-001 | requirements/BASELINE-001.md; requirements/PLATFORM-UX-API-AND-ASSURANCE-STANDARDS.md | no change | No requirement names another language; the validator is F#. |
| Component (modular) monolith on managed PostgreSQL | CNV-ARC-001; REQ-X-020 | baseline | no change | Operating modes and modules are configuration of one deployable (UNB-CDD-CMF-001, -004). |
| WASM-first user interface | UNB-TEC-002; UNB-PLAT-006 | requirements/BASELINE-001.md | no change | Recommendation-time pre-validation is a UI behaviour, not a new client technology. |
| Limen, Forma and Folio | REQ-X-055; REQ-X-002; REQ-X-033; UNB-PLAT-006 | baseline | no change | No new UI or document component. |
| Strata for schema governance; Unbound-owned append-only history | CNV-ARC-004 | baseline | no change | Evidence histories use the existing history component (UNB-CDD-CIV-013). |
| Vigila and Conditor | CNV-GOV-007; CNV-GOV-009 | requirements/CHARTER.md; baseline | no change | Metrics feed existing assurance; no new monitoring product. |
| DB-backed queues where approved; no message broker without evidence | CNV-ARC-001 | baseline | no change | Review queues and event records are database-backed. |
| Sponsor segregation | CNV-TEN-001 | baseline | no change | Every new requirement keeps decisions in the sponsor's cell; shared layer holds public facts only. |
| Append-only financial and audit records | CNV-ARC-004; REQ-X-022 | baseline | no change | Event records are non-posting and append-only. |
| API-mediated access | CNV-AIO-001; UNB-CHAR-034 | baseline | no change | Partner-embedded delivery uses the versioned APIs. |
| Deterministic financial controls | CNV-LEG-001; CNV-PAY-001 | baseline | no change | Modules cannot unbundle mandatory controls (UNB-CDD-CMF-004). |
| SOC 2 and SOC 1 evidence | CNV-GOV-001 | baseline | no change | Per-mode assurance record only (UNB-CDD-CMF-006); no new SOC trigger. |
| Accessibility and mobile-first | CNV-EXP-001; REQ-X-011 | baseline | no change | No UI pattern change. |
| Disaster recovery | CNV-ARC-001; REQ-X-081 | baseline | no change | No new store or RPO/RTO change. |
| Segregation of duties | CNV-GOV-003; REQ-X-048 | baseline | no change | Merge confirmation and promotion need a distinct permission. |
| No microservices, Kubernetes or new infrastructure | REQ-X-020; CNV-ARC-005 | baseline | no change | No new service, cluster or store is introduced. |
| Unbound does not require Echelon Foundry to be a DAF sponsor or administrator | CNV-PAY-001; CNV-GOV-001; CNV-LEG-001 | baseline | no change | CNV-PAY-001 names the sponsor as Originator and Unbound as TPSP; CNV-GOV-001 plans SOC 1 Type 2 on user-auditor reliance; neither names Echelon Foundry as sponsor or administrator. UNB-CDD-CMF-005 makes the absence explicit. |
<!-- END GENERATED -->

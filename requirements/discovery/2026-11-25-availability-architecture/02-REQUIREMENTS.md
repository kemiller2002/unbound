# 02 — Availability, durable execution and messaging requirements (UNB-AVL)

Mission: [README](README.md) · Source data: [`data/gap-requirements.jsonl`](data/gap-requirements.jsonl) · Status: **proposed** — none of these records is adopted into the authoritative baseline; adoption follows the existing ratification process (`requirements/questions/`, convergence clauses).

Every record either refines an existing baseline requirement (named in *Refines / parent*) or is a new requirement
discovered from [unbound#12](https://github.com/kemiller2002/unbound/issues/12) (parent "—"). Where a converged clause
(`CNV-*`) already states the behaviour, these records only add what the clause does not cover; the clause governs on
conflict (baseline precedence rule). *Echelon requirements* link to the portfolio specification in
[INIT-012](https://github.com/kemiller2002/echelon-organization-administration/blob/claude/echelon-availability-architecture-ezhitq/application-governance/availability-architecture/00-INDEX.md).

<!-- BEGIN GENERATED -->
### UNB-AVL-001

| Field | Value |
|---|---|
| Statement | Every Unbound operation MUST declare one execution class (ImmediateQuery, ImmediateControl, ImmediateCommand, DurableCommand, ScheduledCommand, LongRunningOperation, HumanApprovalWorkflow, ExternalIntegrationOperation) as recorded in the operation classification matrix, and its accepted/executed state model MUST follow from that class. |
| Rationale | Makes UNB-API-STD-002's accepted-versus-executed distinction uniform and reviewable; prevents queuing of controls. |
| Refines / parent | UNB-API-STD-002 |
| Source | unbound#12 ('classify each operation as queueable, read-only, or unavailable'); UNB-API-STD-002 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-002 EAR-API-003 |
| Acceptance criteria | Given the operation registry, When the build runs, Then every operation has exactly one class matching data/operation-classification.jsonl. |
| Verification | Automated |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-002

| Field | Value |
|---|---|
| Statement | A receipt for an accepted operation MUST state the acceptance time and that the request is received for processing, and MUST NOT state or imply approval, payment, settlement, a completed gift, or a gift/tax-effective date; those dates come only from instrument rules (CNV-FIN-003, UNB-RCN-FIN-012). |
| Rationale | Year-end disputes and acknowledgment correctness. |
| Refines / parent | RR-UNB-UX-003-r1 |
| Source | unbound#12 ('never promise settlement or tax-effective date at receipt'); mission §15 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-008 |
| Acceptance criteria | Given a contribution intent accepted at 23:59 on Dec 31 whose ACH settles Jan 3, When the donor views the receipt and later the acknowledgment, Then the receipt shows only 'received Dec 31 23:59' and the acknowledgment shows the instrument-rule gift date. |
| Verification | Integration test + Architectural review |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-003

| Field | Value |
|---|---|
| Statement | An operation in a durable class MUST be committed to the authoritative database before any receipt is returned; if commit cannot be confirmed the API MUST return a retryable unavailability response carrying the idempotency key and MUST NOT report acceptance. |
| Rationale | No silent work loss (ADM-040); honest rejection (unbound#12). |
| Refines / parent | ADM-040 |
| Source | unbound#12 ('receipt only after durable commit … reject honestly with retry guidance') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-007 EAR-CMD-001 |
| Acceptance criteria | Given the database fails after the request is parsed and before commit, When the client receives a response, Then it is 'not accepted, retry' and a retry after recovery creates exactly one command. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-004

| Field | Value |
|---|---|
| Statement | At execution, inside the command transaction, the system MUST re-verify principal authority and delegation, holds (fencing per CNV-ARC-003), balances and reservations, charity eligibility within the approved freshness window, applicable deadlines and outstanding mandatory acknowledgments, in addition to the checks CNV-PLT-010 already requires. |
| Rationale | Adds acknowledgments and deadlines explicitly to the re-verification set. |
| Refines / parent | CNV-PLT-010 |
| Source | unbound#12 ('revalidate permissions, balances, restrictions, charity eligibility, deadlines and required acknowledgments at execution') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-017 EAR-MSG-008 |
| Acceptance criteria | Given a recommendation accepted before its submitter's authority is revoked, When it executes, Then it completes as not authorized with no reservation and a notification. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-005

| Field | Value |
|---|---|
| Statement | Durable commands MUST be ordered per fund (or per payment chain for dispatch) with unrelated funds processed in parallel; operations spanning several funds or a shared sponsor liquidity resource MUST update all affected aggregates in one database transaction under optimistic concurrency, without global sequential processing or distributed locks. |
| Rationale | Correct ordering without throughput collapse; consistent multi-fund effects. |
| Refines / parent | REQ-X-002 |
| Source | unbound#12 ('ordered execution within relevant account/aggregate consistency boundary, parallel unrelated work, explicit cross-account dependencies') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-009 EAR-CMD-010 EAR-CMD-011 |
| Acceptance criteria | Given interleaved commands on funds A and B, When processed, Then per-fund order equals acceptance order and A and B progress concurrently; Given a transfer from A to B conflicting with a grant on A, Then balances never go negative. |
| Verification | Integration test (property) |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-006

| Field | Value |
|---|---|
| Statement | The durable command path MUST enforce per-sponsor queued-command quotas, a bounded total backlog with intake throttling, per-sponsor fair share, and priority for deadline-bound and control-adjacent operations over batch work; ImmediateControl operations MUST remain available under throttling. |
| Rationale | December surge and tenant fairness (UNB-TEN-015). |
| Refines / parent | ADM-040 |
| Source | unbound#12 ('bounded backlog, expiry, fairness, backpressure and tenant quotas'); daf-landscape December surge model |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-009 EAR-CMD-020 |
| Acceptance criteria | Given sponsor A floods recommendations at 10x quota-share, When sponsor B submits, Then B's oldest-command age stays within objective and A receives throttling responses, not false receipts. |
| Verification | Integration test (load) |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-007

| Field | Value |
|---|---|
| Statement | Poison commands MUST be quarantined after a bounded number of attempts, blocking only their ordering key, never skipping a command on which later commands depend, and alerting operations. |
| Rationale | Isolation without violating dependencies. |
| Refines / parent | UNB-GEX-020 |
| Source | unbound#12 ('poison-message isolation without violating dependencies') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-013 |
| Acceptance criteria | Given a command on fund A that fails deterministically, When retried N times, Then it is quarantined, fund A stops, fund B continues and an alert is raised. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-008

| Field | Value |
|---|---|
| Statement | Each durable operation MUST declare its cancellation window and its processing deadline; cancellation requests MUST be authorized commands; unstarted commands past deadline MUST expire with notification; deadlines MUST use business calendars and never shorten statutory or contractual deadlines. |
| Rationale | Honest cancellation and stale-intent protection. |
| Refines / parent | UNB-API-STD-002 |
| Source | unbound#12 ('expiry … cancellation semantics'); CNV-ARC-005 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-015 EAR-CMD-016 |
| Acceptance criteria | Given a grant recommendation withdrawn before dispatch, When processed, Then it is cancelled; Given withdrawal after dispatch, Then the request is refused with the reason. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-009

| Field | Value |
|---|---|
| Statement | Queued command payloads MUST reference protected personal data (PII vault references per CNV-ARC-001) rather than copy it, MUST be encrypted at rest, and terminal payloads MUST be reduced after a retention period approved by the sponsor compliance owner while lifecycle metadata follows CNV-PLT-004. |
| Rationale | Protect queued PII (unbound#12). |
| Refines / parent | CNV-PLT-004 |
| Source | unbound#12 ('protect queued PII, retention and audit evidence') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-022 |
| Acceptance criteria | Given a terminal command older than the approved period, When retention runs, Then the payload is reduced and lifecycle history remains. |
| Verification | Integration test + Architectural review |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-010

| Field | Value |
|---|---|
| Statement | Operations MUST be able to pause, drain and resume processing globally, per sponsor and per operation, and MUST see per-sponsor queue depth, oldest-command age, retries, quarantines and OutcomeUnknown aging with alerts linked to runbooks. |
| Rationale | Operational control (unbound#12). |
| Refines / parent | MON-018 |
| Source | unbound#12 ('operational controls for pause/resume/drain, queue age/depth, alerting, recovery and reconciliation') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-AVL-004 EAR-OBS-002 EAR-OBS-003 |
| Acceptance criteria | Given processing is paused for sponsor A, When commands arrive, Then they are accepted, not claimed, sponsor A users see a delay notice, and resume processes them in order. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-011

| Field | Value |
|---|---|
| Statement | When the authoritative database is unavailable the platform MUST refuse new submissions with a retryable 'not accepted' response, MUST NOT buffer work outside the database, MUST present clients a clear unavailable state preserving drafts locally, and MUST publish outage state on an independent status surface; acceptance of durable work during a database outage is out of scope for release 1 (ADR-EAR-008, owner decision U-02). |
| Rationale | A database-backed queue cannot accept while its database is down. |
| Refines / parent | UNB-RES-001 |
| Source | Issue #12 ('define honest degraded behavior'); unbound#12 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-AVL-010 EAR-MSG-010 |
| Acceptance criteria | Given a database failover in progress, When donors submit, Then no receipts are issued, the status surface reports the outage, and no receipted request is lost. |
| Verification | Integration test + Operational evidence |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-012

| Field | Value |
|---|---|
| Statement | For deadline-sensitive operations (year-end contributions, grant deadlines, investment cut-offs, settlement dates) the platform MUST record and display server acceptance time, MUST publish per sponsor a year-end calendar of approval, funding and payment cut-offs per rail, and MUST explain on receipts submitted near a cut-off that completion and gift dates depend on instrument rules. |
| Rationale | Year-end peak and settlement/holiday gaps (daf-landscape research). |
| Refines / parent | CNV-FIN-003 |
| Source | daf-landscape industry-incentives-structural-analysis.md:218-225; review-fin-ops.md:262-272; UNB-RCN-FIN-012; UNB-XIN-SVC-008 surge playbook |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-008 EAR-CMD-016 |
| Acceptance criteria | Given a securities transfer initiated Dec 29 and received in custody Jan 4, When the donor views status, Then the receipt shows Dec 29 receipt time, the gift shows Jan 4 as completed-gift date, and the year-end calendar had shown the custody cut-off. |
| Verification | Integration test + Architectural review (copy) |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-013

| Field | Value |
|---|---|
| Statement | December readiness load tests MUST include the durable command path (acceptance, claim, outbox relay) at the declared December peak multiple, measuring acceptance latency, oldest-command age and outbox lag, before each December. |
| Rationale | Validates the database-backed queue against real seasonal load (CNV-EVB-001 unmeasured). |
| Refines / parent | UNB-IMM-010 |
| Source | ADR-EVA-001 benchmark; CNV-SVC-002 December readiness gate |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-AVL-013 |
| Acceptance criteria | Given the declared peak multiple, When the load test runs, Then acceptance latency, processing delay and outbox lag stay within declared objectives or the readiness gate fails. |
| Verification | Operational evidence |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-014

| Field | Value |
|---|---|
| Statement | SLO declarations MUST add, per operation class and priority, a processing-delay objective (acceptance to claim) and a maximum acceptable queue delay, separately from intake availability, query availability and end-to-end completion. |
| Rationale | Availability of intake is not availability of business completion. |
| Refines / parent | UNB-XIN-SAS-008 |
| Source | Mission §9; CNV-ARC-001 (SLOs per capability before benchmark) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-AVL-013 |
| Acceptance criteria | Given the SLO table, When reviewed, Then intake, query, processing and end-to-end objectives are distinct for each class. |
| Verification | Architectural review |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-015

| Field | Value |
|---|---|
| Statement | The release pipeline MUST test: deployment during submission, crash before and after commit, duplicate retries, command schema version change across N/N-1, intake and storage outage, concurrent grants on one fund, authorization revoked while queued, backlog recovery after pause, and mandatory notice published while commands are queued. |
| Rationale | The test list in unbound#12. |
| Refines / parent | CNV-ARC-005 |
| Source | unbound#12 (test list) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-AVL-003 EAR-AVL-006 EAR-CMD-004 EAR-CMD-017 EAR-CMD-018 |
| Acceptance criteria | Given the release pipeline, When a release candidate is built, Then each listed scenario has a passing automated test or the release is blocked. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-016

| Field | Value |
|---|---|
| Statement | Every persisted command MUST carry its contract version and remain executable by the next release; a release MUST NOT drop support for a version with non-terminal commands. |
| Rationale | Commands accepted by N may execute on N+1. |
| Refines / parent | CNV-ARC-005 |
| Source | unbound#12 ('versioned command contracts') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-CMD-005 EAR-AVL-014 |
| Acceptance criteria | Given non-terminal v1 commands exist, When a release removing v1 support is proposed, Then the release gate fails. |
| Verification | Automated |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-017

| Field | Value |
|---|---|
| Statement | Every query MUST declare a consistency class; values used in decisions (available-to-grant, holds, permissions, eligibility decisions, acknowledgment status, deduplication) MUST be Authoritative; stale-capable displays MUST show an as-of time and MUST NOT present a misleading balance or execution confirmation. |
| Rationale | Stale reads labelled (unbound#12). |
| Refines / parent | UNB-DATA-004 |
| Source | unbound#12 ('stale reads clearly labeled, no misleading balance'); issue #14 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-QCS-001 EAR-QCS-002 EAR-QRY-003 EAR-QRY-004 |
| Acceptance criteria | Given a projection lagging by 2 minutes, When a donor views market value, Then the as-of time is shown; available-to-grant is computed authoritatively. |
| Verification | Automated + Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-018

| Field | Value |
|---|---|
| Statement | Release 1 MUST NOT include a server-side cache; queries MUST carry descriptors (identity, version, consistency class, sensitivity, normalizer, scope) so that a cache can be added later as a decorator without changing domain logic or contracts. |
| Rationale | Cache-ready, not cache-dependent (issue #14). |
| Refines / parent | UNB-DATA-004 |
| Source | Issue #14 |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-QRY-001 EAR-QRY-008 |
| Acceptance criteria | Given the dependency manifest, When scanned, Then no cache provider package is present; Given the query registry, Then every query has a descriptor. |
| Verification | Automated |
| Priority | P2 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-019

| Field | Value |
|---|---|
| Statement | The durable execution capability MUST be built as a separate F# project in the Unbound solution with no reference to Unbound domain projects, with its own tests and documented contract, so it can be extracted when a second Echelon transactional application needs it. |
| Rationale | Avoids a premature shared framework while keeping extraction cheap (ADR-EAR-004). |
| Refines / parent | UNB-PLAT-001 |
| Source | ADR-EAR-004 (proposed; owner decision U-01) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (Execution module incubated per ADR-EAR-004) |
| Dependencies | — |
| Echelon requirements | EAR-API-001 |
| Acceptance criteria | Given the solution, When Dokimos reference-boundary rules run, Then the execution project references no Unbound domain project. |
| Verification | Automated |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-020

| Field | Value |
|---|---|
| Statement | Unbound MUST keep a central message registry per platform and sponsor with lifecycle draft, in review, approved, scheduled, active, withdrawn, expired and superseded; versioned structured content per locale; severity, priority, effective window; and white-label presentation through the sponsor brand manifest. |
| Rationale | Platform messaging (unbound#12). |
| Refines / parent | — (new requirement) |
| Source | unbound#12 §Messaging (new requirement; no predecessor in baseline) |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound (application messaging, ADR-EAR-006); Forma (presentation) |
| Dependencies | — |
| Echelon requirements | EAR-MSG-001 EAR-MSG-005 |
| Acceptance criteria | Given a scheduled message, When its effective time arrives, Then it becomes active for its audience and expires at its end time without manual action. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-021

| Field | Value |
|---|---|
| Statement | Message publication MUST be authorized by kind and audience scope (platform authority for platform/cross-sponsor, sponsor authority for sponsor), MUST require a second approver for mandatory acknowledgments and policy changes, MUST allow single-person emergency publication with review within one business day, MUST target by platform, sponsor, partner/program, role, user/account, functional area, surface and locale with sponsor isolation, and MUST accept only structured content (no raw HTML, scripts or non-allow-listed links). |
| Rationale | Publication abuse and tenant isolation. |
| Refines / parent | — (new requirement) |
| Source | unbound#12 §Messaging |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-002 EAR-MSG-003 EAR-MSG-004 EAR-MSG-013 EAR-SEC-008 |
| Acceptance criteria | Given a sponsor administrator, When targeting another sponsor's users or embedding HTML, Then publication is refused and the attempt audited. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-022

| Field | Value |
|---|---|
| Statement | Acknowledgments MUST be durable server records per (sponsor, user, message, version) with timestamp, surface and authentication context, synchronized across devices; material revisions MUST require renewed acknowledgment; non-material corrections approved as such MUST preserve prior acknowledgments. |
| Rationale | Acknowledgment evidence. |
| Refines / parent | — (new requirement) |
| Source | unbound#12 §Messaging |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-006 EAR-MSG-007 |
| Acceptance criteria | Given a user acknowledged v1 on a phone, When v2 is a material revision, Then the user must acknowledge v2 on any device; a non-material v2 does not prompt. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-023

| Field | Value |
|---|---|
| Statement | Mandatory acknowledgments MUST be enforced by the server at intake and at execution for the operations each message gates; acknowledgment, sign-out, message history, support and emergency assistance, accessibility settings, risk-reducing security actions and read-only access to the user's own statements and tax documents MUST never be blocked. |
| Rationale | Server-enforced gating without trapping users. |
| Refines / parent | — (new requirement) |
| Source | unbound#12 ('server-enforced gating … never block acknowledgment, sign-out, emergency support or essential safety actions') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-008 EAR-MSG-009 |
| Acceptance criteria | Given an outstanding mandatory notice gating grants, When the user calls the grant API directly, Then it is refused with AcknowledgmentRequired; When the user signs out or opens a statement, Then it succeeds. |
| Verification | Integration test |
| Priority | P0 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-024

| Field | Value |
|---|---|
| Statement | Delivery, display, acknowledgment and legally binding acceptance MUST be recorded and reported as distinct facts; an acknowledgment MUST NOT constitute legal assent unless the message is an acceptance instrument governed by the consent/e-sign process (owner/legal decision U-11). |
| Rationale | Distinguish acknowledgment from legal assent. |
| Refines / parent | — (new requirement) |
| Source | unbound#12 ('distinguish acknowledgment from legal assent'; 'delivery/read/acknowledgment distinct') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-006 |
| Acceptance criteria | Given a policy-change notice, When a user acknowledges, Then the record is an acknowledgment and no consent record is created. |
| Verification | Integration test + Architectural review (legal) |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-025

| Field | Value |
|---|---|
| Statement | Authorized sponsor and platform administrators MUST be able to report outstanding acknowledgments by message version, sponsor and role; active notices and outstanding acknowledgments MUST be available through a versioned machine-readable API; widgets MUST show a compact notice linking to the full application; refused API calls MUST reference the gating message. |
| Rationale | API, widget and reporting equivalents. |
| Refines / parent | — (new requirement) |
| Source | unbound#12 ('reporting for outstanding acknowledgments … API and embedded widget equivalents, machine-readable notice and acknowledgment contracts') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-011 EAR-MSG-012 |
| Acceptance criteria | Given 3 of 10 users acknowledged, When the report runs, Then it lists the 7 outstanding users of that sponsor only. |
| Verification | Integration test |
| Priority | P2 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-026

| Field | Value |
|---|---|
| Statement | Maintenance and degradation notices MUST be generated from actual operational state (worker pause, readiness, dependency outage, read-only/status-only suspension per CNV-SVC-002), linked to the scheduled window with updates and resolution, and mirrored on an independent status surface. |
| Rationale | Announcements must match reality. |
| Refines / parent | CNV-SVC-002 |
| Source | unbound#12 ('maintenance announcements linked to actual service state … independent status surface') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-AVL-011 EAR-MSG-010 |
| Acceptance criteria | Given processing is paused for sponsor A, When a sponsor A user signs in, Then a degraded notice appears within 60 seconds and clears on resume. |
| Verification | Integration test + Operational evidence |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |

### UNB-AVL-027

| Field | Value |
|---|---|
| Statement | AI agents MUST surface mandatory notices to their principal and MUST NOT acknowledge on a human's behalf; agent-submitted durable commands MUST carry idempotency keys and MUST NOT be blindly replayed (AIN-020). |
| Rationale | Agents are a primary source of retries and gating bypass. |
| Refines / parent | CNV-AIO-001 |
| Source | CNV-AIO-001; AIN-020; unbound#12 ('API/AI assistance documentation') |
| Applicability | Unbound platform (all sponsor tenants) |
| Responsible system | Unbound |
| Dependencies | — |
| Echelon requirements | EAR-MSG-012 EAR-API-011 |
| Acceptance criteria | Given an agent principal, When it calls the acknowledgment API, Then the response is HumanActionRequired. |
| Verification | Integration test |
| Priority | P1 |
| Status | proposed · not implemented |
| Release scope | initial-implementation |
<!-- END GENERATED -->

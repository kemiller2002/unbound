# End-to-end DAF enterprise workflow completeness audit
Date: 2026-11-23. **Design review, not an executed test.** Grounded in Unbound REQ-X-018..077 and initial DAF landscape coverage/GAP-001..020; full repository/source inventory remains outstanding.

## Test criterion
A workflow is complete only if every transition has a single authoritative component owner, allowed actor and delegation, sponsor policy, canonical write, evidence and time semantics, financial effects, provider failure/Unknown behavior, user-facing status, exception owner, reversible/irreversible boundary and audit trail. Existing feature documentation alone does not pass.

## Preliminary result
**18 cross-enterprise workflows traced; 18 transition-level gaps or unresolved contracts identified.** These are design questions, not proven implementation defects. No workflows have been marked validated, because no synthetic test execution or independent legal/accounting review was performed.

## Workflow inventory
| ID | Journey | In-process owner(s) | Requirement anchors | Most important missing transition |
|---|---|---|---|---|
| WF-01 | Institutional sponsor launch | Agreement execution and sponsor board/delegation are independently evidenced; activation is not merely brand publication. | Contract expired; failed custody feed; missing opening balances; conflicting brand disclosure; sponsor switch | REQ-X-045/057/061/064/071 |
| WF-02 | Donor identity and fund opening | No grantable balance from merely opening fund; no unauthorized donor/advisor rights. | Failed KYC; minor; duplicate identity; donor revocation; inaccessible consent | REQ-X-044/057/068/072 |
| WF-03 | Cash and payroll contribution | Donor intention, payment settlement, legal gift acceptance and available fund capacity differ. | ACH return; payroll reversal; wrong fund; duplicate batch; unidentified funds | REQ-X-022/023/059/070 |
| WF-04 | Complex noncash contribution | No donor receipt or available balance before applicable legal acceptance/evidence; valuation is not cash. | Restricted shares; conflicting appraisal; debt/UBIT; abandoned transfer; price gap | REQ-X-059/060/062 |
| WF-05 | Investment custody and AUM | Ledger is authoritative for financial reporting; raw custody feed is external evidence, not independent fund balance. | Stale feed; missing corporate action; unpriced asset; out-of-balance SMA; fee timing | REQ-X-022/023/040/062 |
| WF-06 | Charity vetting and 180-day refresh | Shared charity evidence does not imply every sponsor grants eligibility; adverse holds persist. | False match; source outage; charity merger; 180-day expiry; conflicting sponsor policy | REQ-X-018/048/054/058/065 |
| WF-07 | Grant recommendation and approval | Donor recommendation is not sponsor approval; reservation atomic; no prohibited benefit. | Two simultaneous grants; donor death; policy change; charity hold; cancellation after payment | REQ-X-018/019/022/048/063 |
| WF-08 | Recurring and future grants | Every occurrence revalidates current policy, charity evidence and fund capacity; schedule creation not perpetual approval. | Pay period change; DST; no funds; charity expires; donor revokes; duplicate trigger | REQ-X-019/022/058/063 |
| WF-09 | Payment execution and charity receipt | Provider Accepted != paid; recipient acknowledged != ledger settlement; Unknown never blind retry. | Payee bank changed; lost check; positive pay rejection; returned ACH; no charity portal | REQ-X-022/041/058/063/077 |
| WF-10 | External embedded checkout | Third-party success screen cannot claim paid until authoritative state; no shadow grant ledger. | Replay; consent revoked; webhook out of order; sponsor outage; recipient substitution | REQ-X-051/057/063/077 |
| WF-11 | Nonprofit recipient account and bank change | Contact affiliation is not bank authority; changed bank cannot silently reroute pending payment. | Stolen contact; bank mismatch; disputed charity; pending check; merged EIN | REQ-X-058/072/077 |
| WF-12 | Fee accrual and revenue sharing | Sponsor fund charge, operator revenue and advisor/custodian pass-through are distinct ledger facts. | Waiver; contract retroactivity; AUM correction; conflict; insufficient cash | REQ-X-023/040/067/076 |
| WF-13 | Month-end close and donor statements | Immutable as-issued statement and locked accounting period; correction never overwrites historical issuance. | Late corporate action; stale custody; missing document; failed email; audit adjustment | REQ-X-032..040/050/060 |
| WF-14 | Annual regulatory reporting and audit | Prepared PDF != filed return; qualified CPA/counsel/authorized signer retains legal responsibility. | Wrong sponsor filing; deadline missed; post-close correction; missing acknowledgment; no filing required | REQ-X-039/044/060/073/076 |
| WF-15 | International grant | Sanctions/legal route and currency conversion remain governed; intermediary acceptance not recipient receipt. | Sanctions change; currency moves; wire returned; missing expenditure report | REQ-X-058/065 |
| WF-16 | Support, complaints and operational health | Assistant cannot close case based on proposed action; financial holds remain authoritative. | Duplicate alert; false positive; overdue case; employee departure; model outage | REQ-X-041/042/048/049/053..055/071 |
| WF-17 | Legacy migration and sponsor exit | No dual financial writers; no fabricated historical records; legal sponsor change is separate transaction. | Pending ACH; mismatched opening NAV; revoked user; lost document; rollback after first live posting | REQ-X-046/057/061/071 |
| WF-18 | Family succession and advisor revocation | Revocation affects live sessions, delegated approvals and pending instructions; not fund legal ownership. | Donor death; contested successor; adviser termination; pending grant; minor beneficiary | REQ-X-044/062/068/072 |

## Workflow detail and challenge
### WF-01 Institutional sponsor launch
**Canonical aggregate:** SponsorContract

**Lifecycle:** Prospect > Diligence > Agreement > AuthorityVerified > ConfigurationDraft > IntegrationsValidated > FinancialOpeningReconciled > ApprovedForActivation > Active

**Authoritative boundary:** Agreement execution and sponsor board/delegation are independently evidenced; activation is not merely brand publication.

**Owner(s):** Sponsor Policy + Identity + Accounting + Integration + Work Queue

**Exception paths:** Contract expired; failed custody feed; missing opening balances; conflicting brand disclosure; sponsor switch

**Existing requirements:** REQ-X-045/057/061/064/071

**Transition gap:** Gap: explicit activation gate joining legal agreement, control signoff, opening GL, donor/charity role availability and fallback.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-02 Donor identity and fund opening
**Canonical aggregate:** Fund

**Lifecycle:** Invitation > IdentityPending > EligibilityValidated > TermsAccepted > FundConfigured > Authorized > Open > Suspended/Closed

**Authoritative boundary:** No grantable balance from merely opening fund; no unauthorized donor/advisor rights.

**Owner(s):** Identity + Fund + Sponsor Policy

**Exception paths:** Failed KYC; minor; duplicate identity; donor revocation; inaccessible consent

**Existing requirements:** REQ-X-044/057/068/072

**Transition gap:** Gap: fund opening requires explicit sponsor acceptance, capacity and relationship evidence.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-03 Cash and payroll contribution
**Canonical aggregate:** Contribution

**Lifecycle:** Intent > SourceVerified > Submitted > ProviderPending > Received > Accepted > Reconciled > Available/Restricted

**Authoritative boundary:** Donor intention, payment settlement, legal gift acceptance and available fund capacity differ.

**Owner(s):** Contribution + Ledger + Provider Adapter

**Exception paths:** ACH return; payroll reversal; wrong fund; duplicate batch; unidentified funds

**Existing requirements:** REQ-X-022/023/059/070

**Transition gap:** Gap: cross-period reversals and receipts must follow legal acceptance state.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-04 Complex noncash contribution
**Canonical aggregate:** AssetGift

**Lifecycle:** Intent > EligibilityReview > SponsorAcceptancePending > CustodyPending > ValuationPending > LiquidationPending/Retained > Reconciled > CapacityEstablished

**Authoritative boundary:** No donor receipt or available balance before applicable legal acceptance/evidence; valuation is not cash.

**Owner(s):** Gift Acceptance + Custody + Ledger

**Exception paths:** Restricted shares; conflicting appraisal; debt/UBIT; abandoned transfer; price gap

**Existing requirements:** REQ-X-059/060/062

**Transition gap:** Gap: explicit conditional acceptance and donation irrevocability clock, with specialist authority.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-05 Investment custody and AUM
**Canonical aggregate:** CustodyPosition

**Lifecycle:** FeedReceived > Validated > Matched > Reconciled > Valued > PublishedAsOf > CorrectedWithHistory

**Authoritative boundary:** Ledger is authoritative for financial reporting; raw custody feed is external evidence, not independent fund balance.

**Owner(s):** Custody Adapter + Accounting + Reporting

**Exception paths:** Stale feed; missing corporate action; unpriced asset; out-of-balance SMA; fee timing

**Existing requirements:** REQ-X-022/023/040/062

**Transition gap:** Gap: same-day vs closed-period valuation corrections and grantable liquidity distinctions.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-06 Charity vetting and 180-day refresh
**Canonical aggregate:** CharityEvidence

**Lifecycle:** Identified > EvidenceCollected > Eligible/Unknown/Adverse > ApprovedWithinScope > Expiring > Recheck > Held/Refreshed

**Authoritative boundary:** Shared charity evidence does not imply every sponsor grants eligibility; adverse holds persist.

**Owner(s):** Charity Registry + Policy + Work Queue

**Exception paths:** False match; source outage; charity merger; 180-day expiry; conflicting sponsor policy

**Existing requirements:** REQ-X-018/048/054/058/065

**Transition gap:** Gap: policy-effective evidence reliance and pending grant behavior during refresh.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-07 Grant recommendation and approval
**Canonical aggregate:** Grant

**Lifecycle:** Draft > Affirmed > Submitted > Validated > SponsorApproved > FundsReserved > PaymentAuthorized > Paid/Returned/Cancelled

**Authoritative boundary:** Donor recommendation is not sponsor approval; reservation atomic; no prohibited benefit.

**Owner(s):** Grant + Policy + Ledger + Work Queue

**Exception paths:** Two simultaneous grants; donor death; policy change; charity hold; cancellation after payment

**Existing requirements:** REQ-X-018/019/022/048/063

**Transition gap:** Gap: exactly defined amendment, cancellation and reapproval rules by state.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-08 Recurring and future grants
**Canonical aggregate:** ScheduleOccurrence

**Lifecycle:** ScheduleCreated > Due > EligibilityRechecked > CapacityReserved > Approved > Sent > Reconciled/Failed

**Authoritative boundary:** Every occurrence revalidates current policy, charity evidence and fund capacity; schedule creation not perpetual approval.

**Owner(s):** Scheduler + Grant + Policy + Ledger

**Exception paths:** Pay period change; DST; no funds; charity expires; donor revokes; duplicate trigger

**Existing requirements:** REQ-X-019/022/058/063

**Transition gap:** Gap: durable occurrence ID and schedule versioning under concurrent updates.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-09 Payment execution and charity receipt
**Canonical aggregate:** Payment

**Lifecycle:** InstructionCreated > Queued > ProviderAccepted/Unknown > InTransit > Settled/Returned > Reconciled > RecipientAcknowledged

**Authoritative boundary:** Provider Accepted != paid; recipient acknowledged != ledger settlement; Unknown never blind retry.

**Owner(s):** Payment + Ledger + Provider Adapter + Recipient

**Exception paths:** Payee bank changed; lost check; positive pay rejection; returned ACH; no charity portal

**Existing requirements:** REQ-X-022/041/058/063/077

**Transition gap:** Gap: standardized proof of receipt by rail and explicit replacement/reissue compensations.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-10 External embedded checkout
**Canonical aggregate:** ExternalGrantIntent

**Lifecycle:** Consent > ScopedIntent > Validated > GrantCreated > SponsorDecision > PaymentOutcome > CallbackDelivered

**Authoritative boundary:** Third-party success screen cannot claim paid until authoritative state; no shadow grant ledger.

**Owner(s):** API/Widget + Grant + Identity + Payment

**Exception paths:** Replay; consent revoked; webhook out of order; sponsor outage; recipient substitution

**Existing requirements:** REQ-X-051/057/063/077

**Transition gap:** Gap: external app liability and retry/status contract at each boundary.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-11 Nonprofit recipient account and bank change
**Canonical aggregate:** RecipientProfile

**Lifecycle:** Unclaimed > AffiliationVerified > Authorized > BankChangeRequested > IndependentlyVerified > Effective > Monitored

**Authoritative boundary:** Contact affiliation is not bank authority; changed bank cannot silently reroute pending payment.

**Owner(s):** Identity + Charity + Payment + Work Queue

**Exception paths:** Stolen contact; bank mismatch; disputed charity; pending check; merged EIN

**Existing requirements:** REQ-X-058/072/077

**Transition gap:** Gap: effective-at semantics for pending payments and independent payee callback.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-12 Fee accrual and revenue sharing
**Canonical aggregate:** FeeEvent

**Lifecycle:** ContractActive > Assessed > Accrued > Reviewed > Posted > Settled > Reversed/Adjusted

**Authoritative boundary:** Sponsor fund charge, operator revenue and advisor/custodian pass-through are distinct ledger facts.

**Owner(s):** Fee + Ledger + Governance

**Exception paths:** Waiver; contract retroactivity; AUM correction; conflict; insufficient cash

**Existing requirements:** REQ-X-023/040/067/076

**Transition gap:** Gap: cross-sponsor fee attribution and fee waterfall dispute resolution.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-13 Month-end close and donor statements
**Canonical aggregate:** ReportingPeriod

**Lifecycle:** Open > DataComplete > Reconciled > ExceptionCleared > Closed > ArtifactIssued > Delivered > CorrectedByAmendment

**Authoritative boundary:** Immutable as-issued statement and locked accounting period; correction never overwrites historical issuance.

**Owner(s):** Ledger + Close + Artifact Registry + Communications

**Exception paths:** Late corporate action; stale custody; missing document; failed email; audit adjustment

**Existing requirements:** REQ-X-032..040/050/060

**Transition gap:** Gap: independent signoff criteria and as-of vs known-at display on every report.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-14 Annual regulatory reporting and audit
**Canonical aggregate:** AnnualObligation

**Lifecycle:** ApplicabilityConfirmed > EvidenceGathered > DraftPrepared > Reconciled > ProfessionallyReviewed > Authorized > Filed > Accepted/Amended

**Authoritative boundary:** Prepared PDF != filed return; qualified CPA/counsel/authorized signer retains legal responsibility.

**Owner(s):** Compliance Evidence + Ledger + Documents + Work Queue

**Exception paths:** Wrong sponsor filing; deadline missed; post-close correction; missing acknowledgment; no filing required

**Existing requirements:** REQ-X-039/044/060/073/076

**Transition gap:** Gap: per-jurisdiction applicability matrix and proof of submission/acceptance.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-15 International grant
**Canonical aggregate:** CrossBorderGrant

**Lifecycle:** Eligibility > LegalPathway > SponsorDecision > FXQuoted > PaymentAuthorized > IntermediaryConfirmed > Reconciled > MonitoringClosed

**Authoritative boundary:** Sanctions/legal route and currency conversion remain governed; intermediary acceptance not recipient receipt.

**Owner(s):** Policy + Grant + FX Adapter + Payment

**Exception paths:** Sanctions change; currency moves; wire returned; missing expenditure report

**Existing requirements:** REQ-X-058/065

**Transition gap:** Gap: country-specific sponsor restrictions, monitoring deadlines and FX reapproval thresholds.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-16 Support, complaints and operational health
**Canonical aggregate:** WorkCase

**Lifecycle:** Detected/Reported > Triaged > Assigned > Investigated > Proposed > Authorized > Executed > Verified > Closed

**Authoritative boundary:** Assistant cannot close case based on proposed action; financial holds remain authoritative.

**Owner(s):** Work Orchestration + AI Assistance + Audit

**Exception paths:** Duplicate alert; false positive; overdue case; employee departure; model outage

**Existing requirements:** REQ-X-041/042/048/049/053..055/071

**Transition gap:** Gap: customer-facing complaint appeals and closure proof distinct from SLA closure.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-17 Legacy migration and sponsor exit
**Canonical aggregate:** MigrationRun

**Lifecycle:** Contracted > Inventoried > Mapped > ImportedToStaging > Reconciled > ParallelValidated > CutoverAuthorized > SingleWriterActive > PostCutoverVerified > Archived/Exited

**Authoritative boundary:** No dual financial writers; no fabricated historical records; legal sponsor change is separate transaction.

**Owner(s):** Migration + Ledger + Identity + Artifact Registry

**Exception paths:** Pending ACH; mismatched opening NAV; revoked user; lost document; rollback after first live posting

**Existing requirements:** REQ-X-046/057/061/071

**Transition gap:** Gap: hard rollback boundary and exception compensation once live posting begins.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.

### WF-18 Family succession and advisor revocation
**Canonical aggregate:** RelationshipChange

**Lifecycle:** EvidenceReceived > AuthorityChecked > PendingReview > Approved > RightsEffective > ExistingWorkRevalidated > Notified

**Authoritative boundary:** Revocation affects live sessions, delegated approvals and pending instructions; not fund legal ownership.

**Owner(s):** Identity + Fund Relationship + Grant + Policy

**Exception paths:** Donor death; contested successor; adviser termination; pending grant; minor beneficiary

**Existing requirements:** REQ-X-044/062/068/072

**Transition gap:** Gap: precedence and timing for in-flight grants, records and communications.

**Theoretical acceptance:** for each transition capture actor, authority, preconditions, input schema, expected authoritative write, source evidence, ledger impact, idempotency, time semantics, reversal/compensation, status in each UX channel, notification, audit and synthetic mock fixture. Exercise the exception paths, concurrency, stale/Unknown provider states and sponsor isolation. No execution performed.


## Cross-workflow invariants
1. One component monolith, one authoritative fund accounting ledger, policy engine, identity/delegation, work queue, document registry and provider adapter layer (REQ-X-057).
2. Legal sponsor remains distinct from technology operator, administrator, adviser, custodian, donor and charity recipient.
3. Every money-related action has explicit legal acceptance, accounting recognition, reservation, release and settlement semantics; never infer finality from a UI message.
4. Sponsor-scoped authority and policy version must be captured at decision time; data sharing requires explicit scope.
5. Unknown provider outcome is a separate durable state with reconciliation, not a failure that can be retried blindly.
6. Event delivery, callbacks and schedules are replay-safe and idempotent with durable occurrence/command IDs.
7. Bitemporal evidence distinguishes effective-at, observed-at, known-at, posted-at and as-issued reporting.
8. Model inference may prepare evidence-backed proposals, not create independent money movement or legal decisions.
9. External providers and all data interactions must have deterministic mock fixtures for offline theoretical simulations and later testing.
10. Every role-specific UX uses the same authoritative state, with accessible mobile, loading, stale, denied, Unknown and recovery states.

## First-priority design repairs
**T1:** Build an explicit transaction and authority boundary matrix across WF-03/04/05/07/09/12/13/17: gift acceptance, capacity, approval/reservation, payment, fees, close and migration. This is where a single-writer system could otherwise generate inconsistent results.

**T2:** Specify full provider-outcome and compensation matrices for ACH, wire, check, securities and FX, including duplicate messages, unknown delivery, returns, payee changes and late events.

**T3:** Model temporal policy and rights changes: charity 180-day refresh, sponsor contract change, donor death, advisor revocation, fund closure and schedule edits while operations are in flight.

**T4:** Define sponsor activation, annual filing and migration cutover as independently reviewed release gates, not ordinary workflow status changes.

**T5:** Validate cross-persona status semantics across donor, nonprofit, advisor, sponsor, administrator, embedded checkout and communications.

## Evidence needed before declaring pass
- Exact REQ-X clause-to-transition bidirectional mapping, not just anchor IDs.
- Per-transition typed state machine and invariant list.
- Synthetic fixture data and expected outputs, including balance and authority snapshots.
- Independent sponsor legal/CPA confirmation of tax, gift acceptance, filing and delegation duties.
- Real vendor product and integration contract evidence before claiming competitive parity.
- Measured operating economics and accessibility/usability validation.

**Status:** 18 mapped, 0 executed, 0 certified as complete.

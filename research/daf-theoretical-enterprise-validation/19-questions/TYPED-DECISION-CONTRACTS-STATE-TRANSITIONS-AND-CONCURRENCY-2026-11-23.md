# Typed decision contracts, state transitions and concurrency challenges

Date: 2026-11-23. **Theoretical specification only**. No executable implementation, tests, formal approval or provider verification. Complements REQ-X-017/018 and ADV-001..ADV-068.

## Shared decision envelope

Every decision receives a versioned typed envelope, not an unstructured prompt. Fields:
- decisionId: application-generated globally collision-resistant ID; intentId: stable client/business idempotency identity with namespace, sponsor, action and origin
- action: enumerated domain command; tenantId, sponsorId, subjectId and controllingPrincipalId: separately verified, scoped references
- actor: authenticated identity, delegation chain, material editor history, action scopes, authentication freshness and revocation epoch
- policy: global safety policy version, shared charity policy version, sponsor policy version, applicable effective interval and independently signed publication evidence
- evidence: typed evidence references with issuer, asserted fact, subject identity, jurisdiction, observedAt, effectiveAt, expiresAt, verification method, independent controlling source and contradictory-source links
- time: requestAt, evaluationAt, executionAt and authoritative commit sequence; distinguish event occurrence time from when platform learned it
- state: authoritative aggregate version, current holds/revocations, external payment state and causal dependencies
- trace: ordered gate outcomes, reason codes, evaluated fact values, provenance hashes, decision authority and attempted escalation history
- output: one typed result, not a boolean; next permissible actions; required evidence; expiration; client-visible status; signed decision receipt

**Fact type:** Known(value, provenance, validInterval) | Absent(proven absence) | Unknown(reason) | Disputed(conflicting evidence IDs) | Stale(lastValidEvidence, expiredAt). An absent fact is not the same as unknown. Unknown/Disputed/Stale must not be coerced to clear.

**Gate type:** Pass | Reject(nonwaivableReason) | Hold(reason, evidenceNeeded) | NotApplicable(justification). Every applicable gate has a typed version and evaluation timestamp. A lower-priority Pass cannot clear a higher-priority Hold or Reject.

**Decision output:** ResolvedByRule | RejectedByRule | HeldForEvidence | HeldForRisk | AIInvestigationRequested | IndependentHumanRequired | ApprovedByIndependentAuthority | QueuedIntakeOnly | UnknownExternalOutcome. Domain-specific state is separate from routing status.

## Domain contract 1: Shared charity eligibility

**Inputs:** charity stable legal identity and alias history; entity type/jurisdiction; common platform-wide vetting evidence; independent authoritative status/sanctions sources; source timestamps; latest material change; global hold; sponsor context only for later grant-policy overlay; verification policy version.

**Pure decision:** Eligibility = EligibleCommon | IneligibleCommon | HoldEvidence | HoldAdverse | DisputedIdentity | VerificationExpired. Verification validity is no longer than 180 days, but current adverse signals and execution-time checks can invalidate earlier approval.

**Transitions:** Unverified -> EvidenceCollecting -> EligibleCommon / IneligibleCommon / Held; EligibleCommon -> ReverificationDue -> EligibleCommon / Held / IneligibleCommon; any -> HeldAdverse on authenticated material adverse event. A sponsor's stricter grant restriction does not mutate global charity eligibility. A global correction must preserve history, provenance and impacted sponsor notification.

**Authority:** Common independently governed vetting standard; legal reliance across sponsors requires counsel. AI can extract or compare evidence, never self-clear adverse holds. Human review only for legally permitted disputed classifications.

**Concurrency invariant:** A payment release uses a current signed charity eligibility snapshot and verifies no later effective adverse event or global hold in authoritative release gate. Avoid time-of-check/time-of-use gaps by binding release to an atomic eligibility/hold epoch or a release barrier that invalidates stale decisions.

## Domain contract 2: Grant recommendation and authorization

**Inputs:** donor/advisor authority chain and revocation epoch; sponsor/account ID; charity identity; purpose and donor affirmation; requested currency/amount; external grant ID namespace; recurring schedule/mandate; sponsor policy version; current charity/sanctions state; available capacity, fee accruals, existing reservations; self-approval lineage.

**Outputs:** Draft | Submitted | Held | Rejected | PendingFundingException | ApprovedAndReserved | Cancelled | PaymentPreparationEligible. Approval and currency reservation must be atomic where authorized; PendingFundingException is not a financial commitment.

**Transitions:** Draft -> Submitted -> PolicyEvaluated -> Rejected / Held / ApprovedAndReserved. ApprovedAndReserved -> PaymentPreparationEligible or Held or CancelledIfStillReversible. Recurring instances are independent domain commands with stable schedule-instance IDs and current execution-time authorization.

**Authority:** Donor confirmation proves recommendation intent only; sponsor approval requires independently board-delegated policy or independent human approval. No self-approval by originating or materially editing controlling principal, including agent or policy publication. Sponsor policy can restrict beyond global controls but cannot waive them.

**Concurrency invariant:** Two approvals racing for the same capacity serialize at authoritative reservation boundary. Any policy or authority change during evaluation either commits against an explicitly permitted pinned version with no new hard hold or retries with new state; never mix versions inside one commit.

## Domain contract 3: Payment release and external observation

**Inputs:** approved/reserved grant ID, payment intent ID and rail; independently verified charity control of destination; independently verifiable provider execution-destination binding; sponsor-controlled authorized destination version; current sanctions, charity and holds; quorum/fencing status; provider qualification and contract; unknown/prior payment outcome; reconciliation evidence.

**Outputs:** Prepared | DestinationVerificationPending | DestinationVerified | ReleaseAuthorized | SubmittedToProvider | UnknownExternalOutcome | ProviderRejected | SettledEvidenceObserved | FinalityConfirmed | Returned | Held.

**Transitions:** Prepared -> DestinationVerified -> ReleaseAuthorized -> SubmittedToProvider -> SettledEvidenceObserved / UnknownExternalOutcome / ProviderRejected. FinalityConfirmed requires rail-specific sufficient evidence; a return can occur according to rail rules. A destination change invalidates DestinationVerified and requires re-authorization. Provider timeout after possible submission transitions to UnknownExternalOutcome, not retryable failure.

**Authority:** Provider self-issued fingerprint cannot independently establish correct destination. No AI authorization of money movement. Human cannot bypass nonwaivable independent destination verification. Provider/rail not capable of independently verifiable binding is ineligible for fully automated payment.

**Concurrency invariant:** Reserve one stable payment identity, then commit an immutable send intent before external submission. After crash/recovery, reconcile whether external effect occurred before retry. A local exactly-once state transition alone does not prove exactly-once external bank execution; require idempotent provider semantics or independent nonreceipt evidence and governed handling of Unknown.

## Domain contract 4: Disaster recovery and protective intake

**Inputs:** writer lease/fencing epoch; quorum acknowledgments and commit index; replica state; independent queue durability receipt; authenticated queued command; emergency hold/revocation/cancellation log; outstanding payment Unknown; recovery plan and reconciliation status.

**Outputs:** CoreWritable | CoreReadOnly | IntakeQueued | ProtectiveOnly | ReconciliationRequired | ReplayEligible | CoreRestored | FailedClosed.

**Transitions:** CoreWritable -> CoreReadOnly on unsafe quorum; CoreReadOnly -> ProtectiveOnly/IntakeQueued only when those independently durable planes are healthy; -> ReconciliationRequired after safe quorum; -> ReplayEligible after protective events, payment identities, and authoritative state reconciled; -> CoreRestored after leader fencing, catch-up and safe failback.

**Authority:** Emergency plane may only freeze releases, revoke access, impose holds and record incidents; cannot approve, change balances/destinations or move money. Removal of protective holds requires independently authorized core operation after integrity restoration.

**Concurrency invariant:** No acknowledged authoritative financial/security commit is lost under defined failure-domain assumptions. No two writable leaders at once. Queued receipt is not approval. Protective holds/revocations/cancellations reconcile before any queued replay. Queue durability and quorum assumptions must be explicitly stated; correlated failure beyond assumptions is not guaranteed zero loss.

## Cross-workflow invariant catalog

| ID | Property | Forbidden counterexample |
|---|---|---|
| TDC-INV-01 | No self-approval across controlling principals, agents and policy authors | Same operator approves through another login |
| TDC-INV-02 | No grant reservation over net available capacity | Two simultaneous grants both reserve final dollar |
| TDC-INV-03 | No money movement while a current mandatory hold applies | Hold arrives after approval but before release |
| TDC-INV-04 | No automated payment without independent destination control and execution binding | Provider substitutes payee with self-fingerprint |
| TDC-INV-05 | No duplicate external effect from Unknown payment retry | Timeout after bank execution causes blind resend |
| TDC-INV-06 | No loss of acknowledged authoritative commit within defined fault model | Primary acknowledges before quorum durability |
| TDC-INV-07 | No split-brain authoritative writes | Partitioned old leader accepts payment |
| TDC-INV-08 | No approval from missing, stale or disputed critical evidence | Null sanction result treated as clean |
| TDC-INV-09 | No cross-sponsor donor data disclosure via shared charity evidence | Sponsor B reads Sponsor A grant notes |
| TDC-INV-10 | No stale recurring authorization | Revoked mandate continues to generate payments |
| TDC-INV-11 | No retroactive rewrite of policy/evidence history | Policy version backdated to legalize grant |
| TDC-INV-12 | No fee duplicate from replay | Fee service event charged twice |
| TDC-INV-13 | No UI false finality | Queue receipt shown as Paid |
| TDC-INV-14 | No AI-induced policy/authority bypass | Injected charity PDF tells model to clear hold |
| TDC-INV-15 | No unauthorized emergency financial commands | Protective plane edits bank destination |
| TDC-INV-16 | No automated grant to prohibited private benefit | Ticket purchase disguised as donation |

## Theoretical concurrency schedules to challenge

| ID | Schedule | Required result |
|---|---|---|
| CONC-01 | Grant A reads $100, Grant B reads $100, both request $80 | At most one atomic reservation succeeds |
| CONC-02 | Charity clean at t1; adverse notice recorded t2; payment release at t3 | Release sees hold and does not submit |
| CONC-03 | Destination verified v1; change requested v2; worker attempts v1 release | Stale destination authorization rejected or explicitly held |
| CONC-04 | Donor revokes agent authority while recurring instance queued | Instance revalidates and holds |
| CONC-05 | Provider executes, response lost, primary fails, worker resumes | Payment remains Unknown until independent reconciliation |
| CONC-06 | Network partition; two sites try to elect writer | Only quorum-fenced leader may commit; other fails closed |
| CONC-07 | Emergency hold arrives during outage, backlog replay begins | Hold reconciles before release; no bypass |
| CONC-08 | Sponsor policy version changes during grant approval | One versioned atomic decision, or retry; no mixed-version approval |
| CONC-09 | Global charity evidence correction and sponsor-specific denial race | Common eligibility and sponsor overlay independently evaluated at execution |
| CONC-10 | Payment provider contract expires after grant approval | Payment rail requalified at release; unqualified automation blocked |
| CONC-11 | Two tenants share charity identity but not donor information | Shared facts propagate, tenant-private fields remain isolated |
| CONC-12 | Intake queue acknowledges then loses primary queue node | Independently durable queued instruction recoverable, otherwise no acknowledgment |

## Proposed model-checking harness (theoretical)

Model finite sets of two sponsors, two grants, two sites, two agents, two payment intents and three evidence states. Generate interleavings of policy updates, holds, approval, reservation, payment submission, provider acknowledgment loss, revocation, quorum loss and recovery. Assert TDC-INV-01..TDC-INV-16 after every transition and check liveness under fair recovery (held requests eventually resolve or receive explicit terminal disposition, not silently disappear). Counterexamples must record minimal event trace, initial snapshot, rule version and expected fix. Include negative tests that prove safety holds remain effective even when AI/human attempts an override.

**Important:** The model can prove invariants only under explicitly declared assumptions. It cannot establish real bank finality, external provider truth, legal compliance or resilience to correlated failure without additional evidence.

## Remaining specification decisions

1. Define authoritative sequencing and freshness semantics for global holds versus payment release, including what happens when an adverse event is known externally but not yet ingested.
2. Define provider-specific independently verifiable execution binding, nonreceipt proof and finality thresholds, without assuming providers support them.
3. Define sponsor policy version migration for in-flight grants, recurring schedules and revocation.
4. Define failover quorum and witness topology, maximum RTO, operational latency and intake queue acknowledgment guarantees.
5. Define the minimum evidence classes for legal entity identity, charity status, donor affirmation and recipient bank control with counsel/compliance.
6. Define immutable audit/retention classes and permissible privacy redaction without destroying financial reconstruction.
7. Define liveness and fairness for queued, held and recurring items without permitting timeout-based fail-open.
8. Confirm original REG-007 cross-reference TVC-034 from primary source.

## Acceptance criteria before implementation

A requirement is implementation-ready only when its input schema, applicable policy, evidence class, state transition, authority, failure status, audit fields, test oracle and source traceability are documented. A test is PASS only when executed reproducibly with artifacts; nothing here is asserted to have passed. Board, counsel, CPA, payment-provider and DR architecture validation remain explicit gates.

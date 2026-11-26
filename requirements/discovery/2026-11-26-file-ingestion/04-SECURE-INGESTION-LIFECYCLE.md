# 04 — Secure ingestion lifecycle

Status: **proposed**. Source: [`data/lifecycle.json`](data/lifecycle.json). Requirements UNB-ING-LCY-001..004.

## Why the 21-state lifecycle was split

The mission proposed one 21-state lifecycle for a file. Testing it against the workflows showed three problems:

1. **It mixes independent questions.** "Is it safe?", "is it valid?", "has it posted?", "does the bank agree?" and "may we
   delete it?" change at different times, for different reasons, under different authority. In one machine every
   combination needs its own state, and the states multiply.
2. **Several states admit no decision.** *Received* (the same commit as quarantine), *Security inspection pending/in
   progress* (a lease on a durable command), *Structurally validated* and *Business validation pending* (one pass, no
   decision between them) and *Ready for processing* (approval satisfied means submit) are operational detail that
   Echelon.Execution already tracks.
3. **Two "states" are not states.** *Exception* is a work item about whatever state was interrupted; *Partially
   processed* is a property of the per-record outcomes. *Awaiting reconciliation* and *Reconciled* belong to an account
   and business date, not a file: one bank file feeds several accounts, one account-date can need two files.

The result is six small machines (Expectation, Artifact, Run, Reconciliation, Retention and Record). Each state is
either a decision point or an externally meaningful fact, and the five meanings the mission asked to distinguish map to
distinct places. Revision 1 (adversarial review R1/R2, [16](16-ADVERSARIAL-REVIEW.md)) closed the paths reviewers found:
releasing a hold returns the Run to *Validated* so approval and requester confirmation are evaluated again; a revoked
artifact *Suspends* every run that has not submitted; there is no human override that releases unscanned bytes; an
expectation is *Satisfied* only when its run completes; runs can no longer get stuck in Pending, Parsing or Submitting;
and records have their own machine so a held record can never be lost when its run completes.

| Meaning | Where it is recorded |
|---|---|
| File receipt | Artifact leaves *Receiving* (bytes committed, hash recorded) — the receipt the user sees (UNB-ING-BND-004) |
| Security acceptance | Artifact *Released* under a recorded policy version |
| Business acceptance | Record outcome *Valid* and command receipted (*Submitted*) |
| Financial posting | Record outcome *Effected* (the command completed) |
| Reconciliation | Reconciliation scope *Matched* or *Resolved* |

**Integration with Echelon capabilities.** Each transition is executed by one durable command (operations OPC-035..046)
or by a human decision command; Echelon.Execution supplies leases, retries, poison isolation, ordering and history, and
Ordo supplies the transition vocabulary. Naming collision: an Artifact in *Quarantined* is unrelated to an
Echelon.Execution command in *Quarantined* (poison); user interfaces and logs always qualify the machine.

**Terminal conditions and recovery.** Terminal states never transition again. Recovery creates new instances: a new Run
(replay) from a released Artifact, a superseding Run before submission, compensating commands after it. *Revoked*
(a released artifact later found unsafe) is deliberately non-terminal: it suspends derived runs and carries an impact
case until a person decides whether the derived records stand; reinstatement after a false positive needs a second
managed engine's clean verdict on the exact hash.

<!-- BEGIN GENERATED: lifecycle -->
**6 machines, 44 states** replace the 21-state single lifecycle (the Record machine holds per-record outcomes).

### Mapping of the 21 proposed states

| Proposed state (mission §11) | Disposition | Maps to | Reason |
|---|---|---|---|
| Expected | kept | Expectation.Open | An expectation exists before any file; it is its own entity |
| Receiving | kept | Artifact.Receiving | Needed for interrupted-upload and orphan reconciliation |
| Received | merged | Artifact.Quarantined | Receipt and quarantine are the same commit; a received-but-not-quarantined state would be a window where unsafe bytes are addressable |
| Quarantined | kept | Artifact.Quarantined | Owner requirement (unbound#18) |
| Security inspection pending | projection | Artifact.Quarantined + inspection attempt status | Queue position is operational detail of the inspection command, already tracked by Echelon.Execution |
| Security inspection in progress | projection | Artifact.Quarantined + inspection attempt status | As above; a lease, not a business state |
| Security rejected | kept | Artifact.Rejected | Terminal security outcome |
| Structurally validated | merged | Run.Validated | Structure and business validation run in one isolated pass; splitting them adds a state without adding a decision |
| Business validation pending | merged | Run.Parsing | No decision happens between structure and business validation |
| Business validation failed | split | record outcome Invalid, or Run.Held when a threshold is breached, or Run.Rejected for structural failure | Failure is usually per record; whole-run failure is a threshold or structural decision |
| Awaiting approval | kept | Run.AwaitingApproval | Only when the business commands themselves require approval; files never require approval merely for being files |
| Ready for processing | removed | Run.Submitting | A state with no possible decision; approval satisfied means submit |
| Processing | renamed | Run.Submitting / Run.Submitted | Distinguishes 'commands being created' from 'commands executing' |
| Partially processed where permitted | removed | Run.Completed (or CompletedWithUnknowns) with per-record outcome counts | Partial is a property of outcomes, not a phase |
| Processed | renamed | Run.Completed | Completed means every record outcome is final |
| Awaiting reconciliation | moved | Reconciliation.Open | Reconciliation scope is an account-date, not a file; one file may feed several scopes and one scope several files |
| Reconciled | moved | Reconciliation.Matched / Reconciliation.Resolved | As above |
| Exception | removed | exception case (work item) referencing any entity; Run.Held where a person must decide before processing continues | An exception is a work item about a state, not a state; modelling it as a state loses which state it interrupted |
| Superseded | kept | Run.Superseded | Pre-submission correction; post-submission correction is by reversal |
| Archived | projection | storage tier attribute under Retention.Retained | Archive tier is a cost optimisation, not a business state |
| Disposed under authorized retention policy | kept | Retention.Disposed | Terminal with tombstone |

### Expectation

A promise that a source will deliver something by a deadline (daily bank report, monthly BMF release, payroll file). Exists before any bytes; drives missing and late detection. Initial state: **Open**.

| State | Terminal | Meaning |
|---|---|---|
| Open | no | Awaiting delivery for an expected business date inside or before its window |
| Lapsed | no | Window plus grace passed without an accepted delivery; an exception case is open |
| Delivered | no | A matching artifact (or artifact set) for the expected business date was released; its Run has not yet completed |
| Suspended | no | An authorized pause covers the source; the window does not run |
| Satisfied | yes | A Run for the expected business date and sequence Completed; the source's completeness watermark advanced |
| Waived | yes | An authorized person declared the delivery not required, with reason; a later delivery opens an 'unexpected delivery' exception |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Open | ArtifactMatched | Delivered | system | profile expectation rule (business date and sequence must match) | artifact or set id |
| Lapsed | ArtifactMatched | Delivered | system | profile expectation rule | artifact or set id; lateness |
| Delivered | RunAccepted | Satisfied | system | Run Completed for the expected business date | run id; watermark advanced |
| Delivered | RunRejected | Lapsed | system | Run Rejected, Cancelled or Superseded without a replacement | run id; exception opened |
| Open | WindowElapsed | Lapsed | system (scheduled command) | profile schedule | schedule version, evaluation time |
| Open | Pause | Suspended | human | ingestion.source.pause | pause id, reason |
| Lapsed | Pause | Suspended | human | ingestion.source.pause | pause id, reason |
| Suspended | Resume | Open | human | ingestion.source.resume | pause id |
| Open | Waive | Waived | human | ingestion.expectation.waive in sponsor scope | reason, actor, time |
| Lapsed | Waive | Waived | human | ingestion.expectation.waive in sponsor scope | reason; exception closed |

### Artifact

Custody and security of one immutable byte sequence (a file, an API record batch, a scanned page set, or a member of an artifact set). Identity = generated ArtifactId; content identity = SHA-256 bound to the stored object version. Initial state: **Receiving**.

| State | Terminal | Meaning |
|---|---|---|
| Receiving | no | An intake slot exists (row committed) and bytes are being written to quarantine storage; nothing may read them |
| Abandoned | yes | Upload interrupted or never completed within the slot lifetime; any partial object is deleted by the reconciler |
| Quarantined | no | Bytes complete, hashed and durable; awaiting or undergoing security inspection. Inspection attempts are a projection, not states |
| Rejected | yes | Security policy refused the artifact (type not permitted, structure mismatch, malware, limits, encryption or password protection, failed source authentication or integrity). Never-released bytes are kept only for the security-evidence period; PDFs with active content are not rejected but released only as derivatives (UNB-ING-SEC-010) |
| Released | no | Inspection passed under a recorded policy version; the artifact may be parsed or presented through safe derivatives |
| Revoked | no | A released artifact was found unsafe or altered (re-inspection, threat report, integrity failure); downloads are blocked, every non-submitted Run derived from it is Suspended, and an impact case lists derived records and commands |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Receiving | BytesCommitted | Quarantined | system | intake authorization already checked for source, tenant and profile purpose | size, SHA-256, storage checksum and version id, channel evidence |
| Receiving | SourceAuthenticationFailed | Rejected | system | profile source authentication (signature, host key, client credential) | failed check |
| Receiving | SlotExpired | Abandoned | system (reconciler) | storage reconciliation policy | slot age, object presence |
| Quarantined | InspectionPassed | Released | system | security policy version; clean verdict for exact hash and version | structure verdict, scanner engine and signature version, limits observed, derivative ids |
| Quarantined | InspectionFailed | Rejected | system | security policy version | failed rule ids, scanner verdict |
| Quarantined | IntegrityFailed | Rejected | system | hash verification | expected vs actual hash |
| Released | ReleaseInvalidated | Revoked | system | re-inspection verdict, threat report or integrity failure under the security policy | verdict or report, impacted runs |
| Revoked | ImpactResolved | Rejected | human | ingestion.security.resolve; retention follows the highest derived class (UNB-ING-RET-001) | impact case outcome, corrective commands |
| Revoked | FalsePositiveConfirmed | Released | human (security owner) with system evidence | second managed engine's clean verdict on the exact hash and version recorded as evidence; not available for integrity failures | second verdict, decision |

### Run

One processing of one released artifact or complete artifact set (or one API record batch) under one profile version. Re-processing is a new run. Evidence documents do not use Runs. Initial state: **Pending**.

| State | Terminal | Meaning |
|---|---|---|
| Pending | no | Created for a released artifact; waiting for dependencies declared by the profile (e.g. prior day's statement) |
| Parsing | no | Isolated parser converts bytes to typed staged records and computes control totals; validation runs in the same step |
| CompletedWithUnknowns | no | Every command reached a terminal state but some records have an ambiguous external outcome (reconciliation obligations open) |
| Completed | yes | Every record outcome is final (none Valid, Held, Submitted, KeyConflict or OutcomeUnknown). Partial processing is Completed with non-zero rejected counts |
| Rejected | yes | Structural failure (schema version unknown, control totals disagree, header/trailer invalid) or an authorized decision rejected a held run; no record was submitted |
| Validated | no | Every record has an outcome (valid, invalid, duplicate, held). This is the dry-run result; nothing has been submitted |
| Held | no | A hold rule fired (invalid share, value limit, volume, drift, dependency timeout or inspection fault); releasing it returns the Run to Validated, where hold rules, approval policy and submission mode are evaluated again in that order |
| AwaitingApproval | no | Business policy (not the file itself) requires approval of the effects, e.g. migration opening balances or a manual journal batch |
| Suspended | no | The source artifact was revoked while the Run had not submitted; nothing may submit until the artifact is reinstated or the Run is rejected |
| Submitting | no | Valid records are being submitted as business commands through the same intake as the interface, using principal-independent business idempotency keys; guard: artifact still Released, source not paused, approval and submission-mode conditions satisfied |
| Submitted | no | Every valid record has a durable command receipt; outcomes pending |
| Superseded | yes | A corrected artifact replaced this run before submission; after submission, corrections are reversal commands, not supersession |
| Cancelled | yes | An authorized person cancelled before submission |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Pending | DependenciesMet | Parsing | system | profile dependency rule; source not paused | dependency run or observation ids |
| Pending | DependencyTimeout | Held | system | profile dependency timeout | missing dependency |
| Pending | Cancel | Cancelled | human | ingestion.run.cancel | reason |
| Pending | ArtifactRevoked | Suspended | system | artifact revocation | revocation id |
| Pending | SupersededBy | Superseded | system or human | correction rule: same source, same business date, later sequence; serialised with submission on the source ordering key | superseding run id |
| Parsing | StructureInvalid | Rejected | system | profile version | structural errors with positions |
| Parsing | RecordsEvaluated | Validated | system | profile version; supervisor-recomputed control totals | record outcome counts, totals declared vs computed |
| Parsing | DriftDetected | Held | system | schema drift rule | drift report |
| Parsing | InspectionFault | Held | system | infrastructure or sandbox fault (never a terminal rejection) | fault id; retry decision |
| Parsing | ArtifactRevoked | Suspended | system | artifact revocation | revocation id |
| Validated | ThresholdBreached | Held | system | profile hold rules (evaluated first) | rule id, measured value |
| Validated | ApprovalRequired | AwaitingApproval | system | approval policy of the target commands (evaluated second) | policy id |
| Validated | AutoSubmit | Submitting | system | submission mode automatic (institution and government sources only); guards: artifact Released at current policy version; source not paused; hold rules clear; approval satisfied or not required; submission mode satisfied | submission decision record |
| Validated | SubmitRequested | Submitting | human | requester confirms the preview and holds authority for every target command; guards: artifact Released at current policy version; source not paused; hold rules clear; approval satisfied or not required; submission mode satisfied | requester, preview hash, per-command evidence |
| Validated | Cancel | Cancelled | human | ingestion.run.cancel | reason |
| Validated | SupersededBy | Superseded | system or human | correction rule, serialised with submission on the source ordering key | superseding run id |
| Validated | ArtifactRevoked | Suspended | system | artifact revocation | revocation id |
| Held | ReleaseHold | Validated | human | ingestion.run.release-hold; cannot be the uploader when the profile demands segregation; releases the hold only | decision, reason; hold cleared flag |
| Held | RejectRun | Rejected | human | ingestion.run.reject | reason |
| Held | Cancel | Cancelled | human | ingestion.run.cancel | reason |
| Held | SupersededBy | Superseded | human | ingestion.run.supersede (a held run is never superseded automatically) | superseding run id |
| Held | ArtifactRevoked | Suspended | system | artifact revocation | revocation id |
| AwaitingApproval | Approved | Submitting | human | approval policy of the target commands (dual control where required); guards: artifact Released at current policy version; source not paused; hold rules clear; approval satisfied or not required; submission mode satisfied | approvers bound to the content hash |
| AwaitingApproval | ApprovalRefused | Rejected | human | approval policy | reason |
| AwaitingApproval | Cancel | Cancelled | human | ingestion.run.cancel | reason |
| AwaitingApproval | ArtifactRevoked | Suspended | system | artifact revocation | revocation id |
| Suspended | ArtifactReinstated | Pending | system | artifact returned to Released (false positive confirmed); processing restarts from the beginning | reinstatement id |
| Suspended | RejectRun | Rejected | human | ingestion.run.reject | impact case outcome |
| Submitting | AllReceiptedOrRefused | Submitted | system | Echelon.Execution receipts; intake refusals become record outcomes (KeyConflict, Declined) | command id or refusal per record |
| Submitted | RecordReleased | Submitted | human | a record-level hold is released and the record submitted as its own command linked to the run | record id, command id |
| Submitted | AllCommandsTerminal | Completed | system | no record Valid, Held, Submitted, KeyConflict or OutcomeUnknown | outcome per record |
| Submitted | TerminalWithUnknowns | CompletedWithUnknowns | system | every command terminal; some outcomes ambiguous | open reconciliation obligations |
| CompletedWithUnknowns | UnknownsResolved | Completed | system | reconciliation resolved every ambiguous outcome | resolution per record |

### Reconciliation

Agreement between Unbound's ledger and an external observation set for one scope (bank account and business date; custodian account and as-of date; payment partner and settlement date). Matched and Resolved scopes are immutable; a later restatement, return or ledger correction for a closed scope opens a new reconciliation item in the current period linked to the closed scope (UNB-ING-FIN-007), and the closed scope shows the link. Initial state: **Open**.

| State | Terminal | Meaning |
|---|---|---|
| Open | no | Observation set and ledger period present; matching not finished |
| Matched | yes | Every item matched by deterministic rules within tolerance |
| Break | no | Unmatched or mismatched items remain; each is an exception case with ageing |
| Resolved | yes | Every break closed by a correcting command, a timing explanation that later matched, or an authorized write-off with evidence |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Open | AllMatched | Matched | system | matching rule version | match pairs, rule ids |
| Open | UnmatchedRemain | Break | system | matching rule version | break items |
| Break | LaterObservationMatched | Matched | system | matching rule version | late item pairs |
| Break | BreaksResolved | Resolved | human or system | reconciliation.break.resolve (write-off requires dual control and threshold) | per-break resolution |

### Retention

Lifecycle of stored bytes and derived personal data independent of processing state. Initial state: **Retained**.

| State | Terminal | Meaning |
|---|---|---|
| Retained | no | Within the retention period of the artifact's record class |
| Held | no | A legal hold applies; disposal is impossible regardless of period |
| Eligible | no | Retention period ended and no hold; disposal scheduled |
| Disposed | yes | Bytes and safe derivatives destroyed; a tombstone keeps id, hash, class, dates and the disposal authority. Ledger entries derived from the artifact are never disposed with it |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Retained | LegalHoldApplied | Held | human | legal.hold.apply | hold id, matter |
| Eligible | LegalHoldApplied | Held | human | legal.hold.apply | hold id, matter |
| Held | LegalHoldReleased | Retained | human | legal.hold.release | hold id, release reason |
| Retained | PeriodEnded | Eligible | system | retention schedule version | record class, trigger date |
| Eligible | DisposalExecuted | Disposed | system | retention schedule version; disposal batch approved by records owner | storage deletion confirmation, tombstone |

### Record

Outcome of one input record of a Run. Invariant (UNB-ING-FIN-003): outcome counts equal input count; at Completed no record is Valid, Held, Submitted, KeyConflict or OutcomeUnknown. Initial state: **Staged**.

| State | Terminal | Meaning |
|---|---|---|
| Staged | no | Parsed and awaiting evaluation |
| Valid | no | Passed validation; eligible for submission |
| Invalid | yes | Failed validation; reasons and source position retained; corrections arrive as new records in a new run |
| Duplicate | yes | Same natural key and same payload hash already accepted; reference to the original; no effect |
| KeyConflict | no | Same natural key with a different payload hash, or an intake idempotency conflict; not applied; DuplicateKeyConflict case open |
| Held | no | Individually above a value or risk threshold, a near-duplicate, or awaiting per-record affirmation |
| Excluded | yes | An authorized person excluded it, with reason |
| Submitted | no | Command receipted; carries the command id |
| Effected | yes | Command completed successfully |
| Declined | yes | Command refused by business rules at execution, exactly as for an interactive request |
| OutcomeUnknown | no | External effect ambiguous; reconciliation obligation open (CNV-PAY-004) |

| From | Event | To | Actor | Authorization | Evidence |
|---|---|---|---|---|---|
| Staged | Validate | Valid | system | profile rules and handler validate mode | rule results |
| Staged | Reject | Invalid | system | profile rules | reasons, position |
| Staged | MatchExisting | Duplicate | system | record identity and payload hash | original record id |
| Staged | KeyCollision | KeyConflict | system | record identity, different payload hash | both payload hashes; case id |
| Staged | Hold | Held | system | record-level hold rule | rule id |
| Held | Release | Valid | human | ingestion.record.release or per-record affirmation | decision or affirmation evidence |
| Held | Exclude | Excluded | human | ingestion.record.exclude | reason |
| Held | Expire | Invalid | system | profile hold expiry | expiry reason |
| KeyConflict | ResolveAsCorrection | Excluded | human | conflict case decision: the correcting record is processed through a compensating command or observation | case outcome |
| KeyConflict | ResolveAsError | Invalid | human | conflict case decision | case outcome |
| Valid | Submit | Submitted | system or human | run submission (guards as for Run Submitting) | command id |
| Valid | Exclude | Excluded | human | ingestion.record.exclude before submission | reason |
| Valid | IntakeConflict | KeyConflict | system | intake idempotency conflict | conflict detail |
| Submitted | Effect | Effected | system | command completed | outcome |
| Submitted | Decline | Declined | system | command refused at execution | reason |
| Submitted | Ambiguous | OutcomeUnknown | system | external effect ambiguous | obligation |
| OutcomeUnknown | ReconciledEffected | Effected | system | reconciliation | evidence |
| OutcomeUnknown | ReconciledNotEffected | Declined | system | reconciliation | evidence |
<!-- END GENERATED: lifecycle -->

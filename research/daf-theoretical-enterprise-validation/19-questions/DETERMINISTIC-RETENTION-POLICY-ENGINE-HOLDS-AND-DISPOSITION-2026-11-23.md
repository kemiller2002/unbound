# Deterministic retention-policy engine, legal holds and controlled disposition
Date: 2026-11-23. Proposed architecture and requirements; no software built, formal verification run or legal opinion obtained.

## Decision
Create one in-process Retention Policy component within the Unbound component monolith. It calculates preservation obligations across financial artifacts, communications, notes, IVR, compliance, logs, templates and source evidence. Financial documents and supporting evidence have a **nonwaivable seven-year platform minimum**; longer legal, contractual, sponsor and hold requirements prevail. The engine is deterministic and versioned; AI may propose classifications but cannot grant deletion authority.

## Three independent decisions
- Preservation: MustPreserve / EligibleForReview / UnknownRequiresHold, with reasons and effective policy snapshot.
- Access: separately authorized under current and effective-dated permissions; retention never grants access.
- Disposition: Denied / AwaitingApproval / AuthorizedForExecution / Completed / Failed, never inferred from a date alone.

## Typed contract proposal
RecordRef = {Component; RecordId; SponsorId; SubjectRefs; SourceVersion}.
RecordClass = canonical stable ID (FIN-01..FIN-08, GOV, LEG, CMP, COM, ADM, TEL, SEC, TEC, SYS, HR, VND, TMP, BAK), with multiple assignments.
RetentionTrigger = IssuedAt | FiscalYearEnd | AccountingPeriodClose | LastMaterialCorrection | FinalSettlement | MatterClosed | ContractTerminated | CustomApprovedTrigger.
RetentionObligation = {PolicyId; Version; Jurisdiction; Class; StartEvidence; MinimumUntil; LegalBasis; Priority; EvidenceRef}.
Hold = {HoldId; ScopePredicate; EffectiveFrom; ReleasedAt?; Owner; Authority; Reason; ReviewEvidence; Version}.
Evaluation = {RecordRef; AsOf; RecordVersion; ClassificationVersions; PolicyVersions; HoldEpoch; DependencyEpoch; Obligations; LongestUntil?; Unknowns; Decision; Explanation; Digest}.
DispositionCase = {CaseId; CandidateRecordRefs; EvaluationDigest; Requester; IndependentApprovals; PlannedAction; ExecutionToken; Status; Evidence}.
Use explicit Known/Unknown/Disputed/Stale values for missing triggers, jurisdictions, source classifications and hold state. Unknown or stale safety-critical facts fail closed for destruction.

## Policy precedence
1. Active legal preservation holds and court/regulatory obligations.
2. Applicable law/regulation and contractual obligations, validated by counsel.
3. Nonwaivable seven-year platform floor for financial evidence.
4. Longer sponsor-specific or approved class-specific schedules.
5. Dependencies: artifacts, source records, correction chains, signatures, templates and evidentiary attachments that must remain interpretable.
6. Disposition governance and privacy requirements. Privacy erasure requests are evaluated, not automatically ignored: retain only what legal obligation requires and restrict access where appropriate.
Never let lower-priority rules reduce a higher-priority minimum. For multiple obligations use the latest valid deadline. If a deadline cannot be calculated, no deletion authorization.

## Retention computation and time
A typed per-class trigger must be approved and supported by immutable evidence. Seven years means at least seven calendar years from approved trigger, not a hard-coded number of days; calendar/year-end conventions and jurisdiction-specific clocks are policy decisions. Store the triggering event, timezone/calendar interpretation, resulting date and policy version. Re-evaluate when corrections, settlements, holds, classification changes or legal changes occur. Existing protected obligations cannot silently shorten under policy migration.

## Legal-hold matching
Hold scopes may target sponsor, fund, account, person, charity, case, record class, event period and linked subject; apply to present and future matching records. Evaluate hold matching against authoritative versioned index, not an eventually consistent search projection. A hold must serialize with disposition authorization and execution through a monotonic hold/safety epoch or equivalent authoritative lock. A hold committed before deletion authorization/execution prevents destruction. Once physical destruction is committed, later discovery cannot retroactively restore it; maintain restoration policy and incident handling. Hold release requires authorized legal/compliance reviewer, segregation of duties and audit.

## Disposition state machine
Classified -> Preserved -> EligibleForReview -> ReviewPending -> ApprovedForDisposition -> ExecutionPending -> Disposed -> Verified, with HoldBlocked/UnknownBlocked/Failed branches. Eligibility never auto-deletes. Approval captures immutable policy/classification/hold/dependency versions and independent reviewer(s). Execution rechecks all safety facts and epochs in the same authoritative transaction that grants a short-lived fenced execution authorization. Storage deletion may be external/nontransactional; use an idempotent recoverable worker with verification, no blind retries of ambiguous external state, and an auditable execution journal. If a new hold arrives after authorization but before external delete, recheck at the last controllable boundary; acknowledge race limitations and define stronger provider retention locks where required. No disposition while object-lock retention remains active.

## Cross-component integration
Accounting, Grants, Contributions, Compliance, Communications, Administrative Notes, IVR, Document Artifact Registry and audit components register record classifications, lineage and retention triggers through typed in-process commands. No independent component can directly delete a protected record; use a mediated retention gate for application and storage operations. For third-party processors, enforce contractual retention/deletion and obtain evidence, but do not claim technical enforcement where not possible. Maintain classification/dependency references without exposing tenant-private contents. The shared registry owns immutable issued bytes, not retention decisions.

## Storage and recovery
Versioned immutable storage/object lock for issued financial documents; backups and encryption keys must preserve availability and exact hashes for full minimum period. Separate hot, warm and archive tier transitions from deletion. Monitor retention-policy drift between database, object metadata, provider lifecycle and backups. Recoverability must be tested. Maintain controlled audit/tombstone evidence of lawful disposal, with privacy and legal review.

## Administration and UX
Retention Console: searchable record class and evidence, computed retention start/deadline, policy precedence explanation, active holds, dependencies, archive location, pending disposition cases, approvals, execution receipts, integrity status and exception queue. Offer hold creation/release to legal/compliance only; sponsor admins may request extensions but cannot shorten seven-year floor. Document Center shows archived availability without revealing privileged legal hold details. Provide machine-readable explanation for each decision and reason codes suitable for audits.

## Operational safeguards
Version and sign policy releases; require test cases and dual control for changes that can shorten preservation. No direct database deletes or bypass via scripts. Alert on unknown classification, missing source event, failed hold sync, object lock mismatch, orphaned archive, pending execution with stale evaluation, and unauthorized deletion attempts. Metrics: protected records, hold coverage, TBD classes, classification uncertainty, eligible cases, approval turnaround, failed deletions, archive retrieval success, integrity drift, policy changes.

## Acceptance criteria (proposed)
- Financial record cannot be eligible before seven-year minimum from approved trigger.
- Multi-class record uses latest deadline and honors dependencies.
- Unknown legal/classification/trigger/hold facts block disposition.
- Sponsor cannot override minimum or legal hold.
- Hold matching covers existing and future linked records.
- Access revocation does not delete preserved records.
- EligibleForReview never directly deletes.
- Independent approvals and execution-time revalidation are mandatory.
- Policy migrations and clock changes produce audited, reproducible results.
- External object-store failure yields honest Failed/Unknown state, not success.
- Archived artifact bytes, hashes and evidence survive restore and renderer changes.
- A later legal hold racing with deletion cannot be silently ignored; defined serialization and provider limits are documented.

## Adversarial scenarios (proposed, not executed)
- ADV-379: seven-year clock mistakenly uses 2555 days. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-380: record assigned financial and short-lived marketing class. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-381: unknown issue timestamp. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-382: unapproved jurisdictional trigger. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-383: sponsor attempts 3-year override. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-384: legal hold inserted during approval. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-385: hold inserted after authorization before storage delete. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-386: hold scope includes future corrections. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-387: hold matching uses stale search index. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-388: hold released by same requester. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-389: AI recommendation removes FIN class. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-390: reclassification drops linked attachment. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-391: policy upgrade retroactively shortens expiry. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-392: dependency on template/signature omitted. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-393: privacy request bypasses financial hold. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-394: archive tier migration triggers provider purge. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-395: backup expires before original. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-396: encryption key deleted early. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-397: duplicate disposition worker retries ambiguous delete. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-398: object store says delete succeeded but bytes remain. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-399: object store says failed but bytes absent. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-400: direct SQL deletion bypasses engine. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-401: cross-sponsor record classified under wrong tenant. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-402: legal hold is active but access is revoked. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-403: eligible record deleted without approval. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-404: record's disposition approved but a new correction arrives. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-405: provider object lock prevents authorized disposal. Expected: conservative preservation, authoritative revalidation and audited handling.
- ADV-406: audit evidence lost after deletion. Expected: conservative preservation, authoritative revalidation and audited handling.

## Unresolved decisions
Counsel/compliance approve precise per-class triggers, legal minima, cross-jurisdiction conflicts and privacy handling. Technical ADRs select storage lock semantics, hold/disposition concurrency, signed policy releases, external delete recovery and backup lifecycle. Prove failure/race behavior before production.

Status: proposed; no implementation or tests executed.

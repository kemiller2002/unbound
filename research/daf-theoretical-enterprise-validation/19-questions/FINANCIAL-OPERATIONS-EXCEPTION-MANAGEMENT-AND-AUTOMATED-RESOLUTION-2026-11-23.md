# Financial Operations Exception Management and Automated Resolution
Date: 2026-11-23. Proposed requirements only; no implementation, simulations or tests executed.

## Decision
Create one shared in-process Financial Operations Exception Management component inside the Unbound component monolith. It coordinates exceptions across accounting close, reconciliations, contributions, grants, payments, charity vetting, sanctions, document generation, retention, communications and external providers. It does not become a parallel source of truth or an independent authority to move money, clear legal holds or approve regulated decisions. Preserve the outcome, not the existing manual process.

## Exception taxonomy and priority
Classify by origin domain; affected sponsor/fund/transaction/recipient; safety class (financial integrity, legal/compliance, privacy/security, operational availability, presentation); root-cause category (missing evidence, conflicting evidence, stale evidence, deterministic rule failure, external Unknown, provider outage, data-quality issue, policy ambiguity, user action, render/delivery failure); reversibility; materiality; customer impact; blast radius; and deadline. Prioritize by potential harm and urgency, not only ticket age. Sponsor-level GL breaks can block a close; isolated rendering errors need not block unrelated funds.

## Typed contract
ExceptionId, SourceComponent, SourceSubjectRef, SponsorId, FundScope, DetectionRuleVersion, FirstSeenAt, LastObservedAt, EvidenceRefs, Facts(Known/Absent/Unknown/Disputed/Stale), Severity, MaterialityPolicyRef, ProtectiveGateRefs, RootCauseHypotheses, ResolutionCandidates, AssignedOwner, RequiredAuthority, SLA, State, StateVersion, RelatedExceptions, ResolutionProof, AuditTrail. Source component owns authoritative underlying condition and protective gates. Exceptions link to immutable accounting/document evidence and communications without copying sensitive data into cross-tenant indexes.

## Lifecycle
Detected -> Classified -> EvidenceGathering -> AutoResolutionCandidate -> VerificationPending -> ResolvedByAuthoritativeEvidence OR HumanReviewRequired -> AuthorizedActionPending -> ResolutionVerified -> Closed.
Branches: Blocked, AwaitingExternalProvider, Escalated, Reopened, DuplicateLinked, Superseded, FailedRecovery. Case closure requires verified source-state change or explicit evidence-backed 'no action required' disposition. It must never automatically release a financial/legal hold. Distinguish operational case Closed from financial control Pass.

## Resolution tiers
Tier 1 deterministic: retry safe read-only source retrieval, compare provider records, validate formats, resolve stale cache, reconcile identical duplicates, request missing non-sensitive fields, check published charity data, re-render document from unchanged approved snapshot, and requeue idempotent communications when provider state proves no prior delivery. All automated actions must be allowlisted, versioned, idempotent and bounded.
Tier 2 bounded AI: summarize conflicting evidence, identify likely causes, recommend next verification query or case owner, extract structured fields from documents with provenance. AI cannot declare a bank payment failed based on timeout, post ledger adjustments, clear sanctions holds, approve exceptions, change retention or fabricate evidence.
Tier 3 authorized human: independent accounting adjustments, policy interpretation, suspicious activity/sanctions decisions, disputed recipient control, material write-offs, close signoffs, privacy/legal disposition and other nondelegable judgments. Preserve segregation of duties and documented rationale.

## Protective gates and concurrency
Use authoritative domain commands and safety epochs. Case actions cannot bypass atomic grant reservations, global charity holds, provider payment Unknown, close reconciliation gates or legal-hold blocks. Hold release only after source component verifies the condition and applicable authorization. Concurrent exception closure, sponsor policy change, payment send, period close or charity suspension must serialize at relevant authoritative boundaries. External irreversible actions require separate proof and state handling.

## Detection and grouping
Sources emit typed exception facts from failed/unknown reconciliation assertions, stale valuation, payment execution uncertainty, duplicate grant, missing charity reverification, sanctions flags, inaccessible PDFs, failed Typst render, artifact hash mismatch, bounced email, and retention-policy unknowns. Deduplicate by stable subject + condition + evidence version, not only message text. Group related symptoms under a root-cause incident without merging distinct sponsor-private evidence or weakening per-record controls. Maintain reopening history.

## Automated investigation
Evidence acquisition plan with permitted sources, freshness thresholds, privacy constraints, retry budgets, source confidence, deterministic comparison, discrepancy explanations and explicit Unknown states. Prefer source-system evidence over AI conclusions. If repeated exceptions recur, propose reviewed changes to intake forms, validation rules or upstream workflow; never self-modify financial policy. Human interventions should be measured as avoidable operational cost, not a target to minimize regardless of risk.

## Case UI
Unified Exception Command Center: sponsor/period filters, open/blocked/awaiting-external queues, severity, financial exposure, affected funds, protective holds, deadline, evidence freshness, owner, next best safe action, AI recommendation clearly marked, authoritative status, audit history and cross-linked financial/document records. Detail shows observed facts versus hypotheses, required authority, proposed action, simulation of expected impact where supported, approvals, verification and communications. Role-specific views for controller, compliance, payments, donor operations, platform support and sponsor admin. No blank loading state; accessible and mobile-friendly. Customer-facing status must be appropriately generalized without revealing investigations or other sponsors.

## Workflow and notification
Use event/outbox and in-process domain command contracts, not internal microservices. Notify owners only for actionable exceptions, aggregate duplicates and suppress noisy repeats. SLA escalation is risk-aware and sponsor-configurable within platform safety. Avoid sending sensitive details in ordinary email. Record all assignments, acknowledgments, actions, provider outcomes and case closure evidence. Support manual external actions such as check investigation with documented evidence.

## Audit and retention
Link cases to authoritative source facts, close snapshots, payment provider attempts, Document Artifact Registry and retention classes. Financially material cases, messages, attachments and resolution evidence inherit the nonwaivable seven-year minimum or longer legal hold. Preserve original case state history, AI model/version and cited evidence where relevant, decision authority, and corrected source records. AI outputs are proposals, not verified facts.

## Operational metrics
Exception incidence per 1,000 transactions/funds, percentage resolved deterministically, AI touches, human touches, time to detection, time to safe resolution, repeated root causes, prevented duplicate payments, false holds, unsafe auto-resolution attempts, unresolved financial exposure, external-provider waiting time, SLA breaches and cost per resolved case. Distinguish fewer exceptions due to upstream design improvement from suppressed detection.

## Acceptance criteria
1. An exception case cannot directly release a payment or legal hold.
2. Unknown external payment execution cannot be classified Failed and blindly retried.
3. Routine evidence retrieval and safe deterministic fixes can resolve without human touch when independently verified.
4. Material accounting changes require authorized domain action and independent review.
5. Cross-sponsor case metadata and evidence remain isolated.
6. Duplicate symptoms can be grouped without losing distinct financial controls.
7. Case closure is distinct from financial reconciliation Pass and payment Paid.
8. Reopening preserves prior decisions and evidence.
9. AI output cannot modify authoritative state without validated deterministic/domain authorization.
10. Financial case evidence survives seven years and legal holds.
11. Concurrency with holds, close and payment release cannot bypass authoritative gates.
12. UI makes facts, uncertainty, proposed action and final decision distinguishable.

## Adversarial scenarios (proposed, unexecuted)
- ADV-471: payment provider timeout treated as failed and retried; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-472: case closed while global charity hold remains; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-473: GL discrepancy resolved by unsupported AI adjustment; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-474: sponsor A exception leaks sponsor B evidence; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-475: duplicate cases trigger duplicate payment actions; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-476: missing custodian feed auto-cleared at deadline; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-477: stale valuation reclassified current; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-478: charity reverification expired during case review; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-479: sanctions false positive cleared without approved evidence; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-480: financial case deleted before seven years; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-481: rendering failure silently regenerates altered historic statement; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-482: PDF hash mismatch marked cosmetic; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-483: bounced email interpreted as failed statement issuance; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-484: provider accepted interpreted as delivered; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-485: human reviewer approves own material journal correction; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-486: AI model fabricates source citation; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-487: external data enrichment exceeds privacy scope; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-488: case deduplication merges unrelated funds; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-489: parent incident closure clears child protective gates; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-490: risk severity downgraded to meet SLA; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-491: customer sees confidential investigation reason; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-492: concurrent payment send occurs while hold commits; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-493: source fact changes after exception verification; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-494: auto-retry exhausts provider rate limit; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-495: manual check investigation lacks delivery evidence; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-496: repeated exceptions hidden instead of redesigned; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-497: AI recommendation automatically changes policy; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-498: closed case reopened but history overwritten; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-499: missing evidence classified absent; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-500: sponsor offboarding destroys unresolved exception; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-501: legal hold released by support staff; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-502: stale case status shown as current; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-503: exception notification exposes PII; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-504: retry of statement notice duplicates recipients; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-505: one fund issue blocks all sponsors unnecessarily; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.
- ADV-506: material discrepancy split into small cases to evade threshold; expected: fail-safe domain authority, independently verified resolution, correct scope and immutable evidence.

## Open decisions
Sponsor-specific materiality and escalation policies; who can approve which corrections; external provider evidence freshness; grouping boundaries; bounded AI deployment and provenance; case SLAs and privacy; whether separate nonfinancial operational exceptions share this component or only its generic case primitives. No implementation or tests have been performed.

Status: proposed. ADV-471..ADV-506 not executed.

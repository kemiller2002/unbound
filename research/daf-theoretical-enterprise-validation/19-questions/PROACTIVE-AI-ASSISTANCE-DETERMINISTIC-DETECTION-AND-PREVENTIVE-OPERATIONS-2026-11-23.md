# Proactive AI Assistance: deterministic detection and preventive operations
Date: 2026-11-23. Proposed requirements only. No implementation, monitoring or tests.

## Core decision
Use deterministic, versioned checks for known conditions. Invoke AI selectively for interpretation, uncertain causal investigation, communication and proposed workflow improvements. Feed findings to the existing in-process Exception Management and Work Orchestration components, not a new case system. Domain components own holds and authoritative state.

## Detection inventory
- Charity verification approaching/exceeding 180 days, material adverse changes, missing evidence and contradictory sponsor reliance rules.
- Unassigned/stalled grants, missing backup owners, approaching SLA, orphaned specialist subtasks, conflicts of interest and staff overload.
- Sponsor policy contradictions, invalid delegation, expired authority, dangerous rule changes, incomplete required disclosures, inconsistent white-label terminology.
- Recurring grants/contributions: upcoming revalidation, insufficient available-to-grant capacity, changed charity status, schedule collision and missing confirmation.
- Accounting: out-of-balance ledger/subledger, stale custodian feeds, reconciliation mismatch, period close blockers, anomalous fee configuration, duplicate external references.
- Payments: provider Unknown, stale acknowledgment, mismatched destination ownership, duplicate retry risk, mailed/hand-delivered check evidence gap.
- Documents: missing final PDF hash, seven-year retention misclassification, legal hold conflicts, stale as-issued reference, inaccessible statements.
- Integration: schema/version drift, provider outage, mock/production boundary violation, credential expiry (never expose secrets), webhook delivery failure.
- UI and reports: inaccessible template, missing mandatory text, stale KPI watermark, report distribution misconfiguration, insufficiently scoped embedded widget.

## Finding lifecycle
Observed -> DeterministicallyValidated -> RiskClassified -> Correlated/Deduplicated -> AssignedToOwner -> (OptionalAIInvestigation) -> ProposedRemediation -> ApprovedIfRequired -> AppliedByDomain -> IndependentlyVerified -> Resolved. Alternate states: FalsePositive, SuppressedWithExpiry, WaitingEvidence, Unknown, Escalated, Reopened. Never infer resolution merely from alert dismissal or lower queue count.

## Finding contract
PreventiveFinding {FindingId, DetectorId/Version, SourceDomain, SponsorScope, SubjectRef, FirstObserved, LastObserved, EffectiveAt, KnownAt, EvidenceRefs, Severity, Confidence, Impact, Deadline, Owner, State, DedupKey, RemediationProposal, VerificationCriteria, PolicyVersion, SuppressionReason/Expiry, Audit}. Dedupe by root condition and affected scope, not arbitrary text similarity. Protect sensitive compliance detail.

## Risk-tiered action model
Tier 0 Inform: contextual notice or digest, no case if low impact.
Tier 1 Safe deterministic enrichment: refresh authorized evidence, rerun validation, add non-authoritative notes; no money/approval change.
Tier 2 Proposal: AI drafts policy/config/workflow/email change for human or delegated approval.
Tier 3 Protective escalation: domain-authorized hold/freeze when an existing deterministic safety rule mandates it; AI cannot independently impose or lift holds.
Tier 4 Restricted/irreversible: never autonomously execute grant approval, payout, sanctions release, financial close, destructive retention or policy waiver.

## Operational integration
Use shared exception/case registry, Work Queue routing, SLA and dedicated account ownership, existing communications templates, Operational Learning prevention, and shared KPI dictionary. Sponsor admins see scoped findings; platform/compliance may see authorized aggregate; cross-sponsor correlation must not reveal other clients' data. Route findings to accountable owner or qualified backup. Deduplicate notifications and suppress low-value noise, but never suppress mandatory protective holds.

## Proactive UX
A Preventive Insights panel embedded in relevant dashboards, not an extra mandatory inbox. Show What happened, Why it matters, Evidence/freshness, Who owns it, Next safe action, Deadline, Confidence and How to verify. Filter by actionable, waiting, snoozed, escalated, resolved. Explain false-positive and suppression controls with expiry. Summaries should distinguish estimated from proven savings.

## Measurement
Detection coverage, false positive/negative rate where measurable, time to identify, time to resolve, recurrence, harm prevented with defensible counterfactual, cost per finding, AI invocation rate, human touches, backlog, suppression and sponsor impact. Avoid optimizing solely for fewer alerts; shadow and replay tests must verify no missed harms.

## Adversarial scenarios (proposed, unexecuted)
- ADV-867: detector misses expired charity because of timezone; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-868: material change arrives between scheduled checks; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-869: alert duplicates across shared charity sponsors; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-870: false positive causes unauthorized hold; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-871: AI suppresses critical finding; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-872: stale reconciliation data appears healthy; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-873: provider Unknown triggers blind retry; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-874: orphaned case has no eligible owner; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-875: snooze hides mandatory deadline; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-876: remediation changes policy without approval; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-877: finding marked resolved after notification only; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-878: dedupe merges unrelated sponsor incidents; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-879: AI cites other sponsor's case; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-880: financial issue hidden by lower queue count; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-881: model provider outage stops deterministic checks; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-882: false negative never enters evaluation; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-883: alert flood overwhelms reviewers; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-884: severity model underrates legal hold; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-885: staff vacation leaves unresolved escalation; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-886: configuration drift after deployment; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-887: mock data detector hits production; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-888: email draft contains unauthorized PII; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-889: retention engine deletes held document; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
- ADV-890: AI claims savings without baseline; expected: deterministic detection, bounded AI, safe escalation and evidence-based verification.
Status: proposed. ADV-867..890 unexecuted.

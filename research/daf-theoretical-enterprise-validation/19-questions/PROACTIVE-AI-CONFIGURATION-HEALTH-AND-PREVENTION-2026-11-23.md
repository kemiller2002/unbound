# Proactive AI configuration health, prevention and operational learning
Date: 2026-11-23. Requirements only; no implementation or executed testing.

## Objective
Identify preventable problems before they become grant failures, compliance exposure, reconciliation breaks or unnecessary human work. Use deterministic rules first, bounded AI second for explanation and uncertain causal hypotheses, human authority for governed changes. Integrate with existing Operational Learning, exception case management, routing, metric registry and sponsor policy lifecycle, without creating a parallel alerting or policy engine.

## Trigger architecture
Deterministic checks at draft validation, publish approval, scheduled policy activation, recurring grant execution, charity 180-day reverification window, expiring delegation, staffing absence, provider mapping changes, report publication, financial close, new evidence, incident recovery and release/schema migration. Periodic checks only when a defined obligation cannot be covered by event triggers. Check results include contract/policy version, scope, freshness, source IDs and deduplication fingerprint.

## Detection catalog
- **Grant rules:** impossible combinations, missing affirmative prohibited-benefit disclosures, recurring grant revalidation gaps, unapproved exception route, fee surprises, recipient control mismatch.
- **Charity vetting:** upcoming/expired 180-day checks, adverse evidence, stale external source, duplicate reviews across sponsors, missing reliable verification.
- **Staffing and ownership:** no eligible reviewer, uncovered leave, orphaned cradle-to-grave case, overload, round-robin fairness anomaly, expired delegation, segregation-of-duties conflict.
- **Finance/AUM:** reconciliation breaks, unexplained balance differences, stale NAV, report mismatches, fund liquidity and fee configuration conflicts; never treat AI inference as accounting fact.
- **Provider integration:** repeated Unknown responses, unsafe retry configuration, schema mismatch, webhook replay risk, missing mock coverage.
- **Documents and communications:** missing protected disclaimer, invalid token, wrong sponsor theme, inaccessible template, failed distribution, as-issued integrity mismatch, legal hold and retention conflict.
- **Dashboard/report:** metric version mismatch, misleading freshness, privacy cohort risk, scheduled report lacking approver or recipient entitlement.

## Finding model
FindingId, Fingerprint, Domain, Scope, Severity, Confidence, EvidenceClass, DetectionRuleVersion, SourceWatermarks, FirstSeen, LastSeen, BusinessImpact, DecisionOwner, Status, SuggestedRemediation, ValidationPlan, SuppressionReason, Expiry and LinkedCases. Distinguish VerifiedViolation, EvidenceGap, Anomaly, Hypothesis, Informational. Missing evidence is not proof of wrongdoing.

## Workflow
Detect -> corroborate -> deduplicate -> classify -> route to responsible owner -> propose smallest safe remedy -> validate and simulate -> authorize -> apply via domain command -> verify downstream outcome -> close with evidence -> feed Operational Learning. Critical holds remain with authoritative domain. Avoid alerting all staff; route to accountable role and summarize in dashboard. Suppression requires time-bound, audited reason and cannot suppress nonwaivable holds. Incident states may allow protective-only escalation but no AI financial authority.

## Prioritization and noise controls
Rank by verified harm, legal deadline, reversibility, exposure and confidence, not just model urgency. Bundle related symptoms by root cause, enforce rate limits and digest defaults, show why this matters and next best action. Measure false-positive rate, ignored recommendations, time to remediation, repeat incidents and actual downstream harm, not alert volume or task count alone. Protect against alert suppression hiding rising harm.

## UX
Proactive Findings Inbox, configuration health panel, inline pre-publish checks, supervisor staffing risk panel, charity refresh horizon, provider degradation and financial-close readiness. Every finding shows observation versus hypothesis, source freshness, exact affected sponsor/fund, action owner, impact, proposed remedy and explicit approval boundary. One-click navigation to the correct configuration or case, not one-click bypass.

## Test and evaluation plan
Use deterministic synthetic scenarios and mocked providers for each trigger; test no-op on healthy cases, idempotent detection, deduplication, escalation and timezones. Adversarially test false positives, missing data, source conflict, cross-tenant leakage, suppression abuse, stale approval, policy changes and AI prompt injection. Run shadow evaluation before live notification; thresholds and production release gates require actual empirical evidence.

## Adversarial cases (proposed, unexecuted)
- ADV-867: healthy sponsor receives warning storm; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-868: expired charity evidence falsely considered current; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-869: missing evidence labeled legal violation; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-870: two sponsors share charity evidence and private data leaks; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-871: repeated event creates duplicate case; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-872: suppression hides nonwaivable hold; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-873: owner on leave never receives escalation; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-874: provider Unknown causes blind retry; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-875: policy activation timezone miscalculated; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-876: AI proposes retroactive fee adjustment; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-877: reconciliation discrepancy called resolved without GL proof; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-878: AI recommends lowering verification to reduce workload; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-879: notification fatigue masks high-severity issue; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-880: source watermarks absent; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-881: false positive triggers automatic financial freeze; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-882: cross-sponsor aggregate reveals tiny donor cohort; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-883: AI interprets adversarial external website as instruction; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-884: alert closes on ticket closure not verified remedy; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-885: expiring delegation not rechecked at action; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-886: new release invalidates validator silently; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-887: recurring grant deadline ignored during weekend; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-888: risk score lacks explanation; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-889: AI confidently attributes root cause without evidence; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
- ADV-890: protective-only incident plane used to approve payment; expected evidenced, deduplicated, safely routed prevention with no unauthorized action.
Status: proposed. ADV-867..ADV-890.

# Operational Learning, Root-Cause Analysis and Upstream Exception Prevention
Date: 2026-11-23. Requirements and proposed validation only. No software implementation or tests executed.

## Purpose
Unbound must learn from recurring operational exceptions to redesign upstream workflows rather than merely accelerate downstream ticket resolution. Preserve the outcome, not the process. Operate as an in-process component within the component monolith, consuming governed exception facts and producing versioned proposals, experiments and approved changes. Do not introduce microservices or independent financial truth.

## Distinct decision layers
1. **Observe** verified exception events, near misses, failed validations, holds and false positives.
2. **Diagnose** causal hypotheses, distinguishing correlation from demonstrated mechanism and upstream cause from downstream symptom.
3. **Design** candidate prevention interventions, including eliminating unnecessary fields or steps, earlier deterministic validation, better evidence capture, clearer user interfaces, provider-contract improvements, reconciliation redesign and policy simplification.
4. **Challenge** each proposal against counterexamples, sponsor variation, accessibility, privacy, failure modes, financial control integrity and operational cost.
5. **Authorize** via risk-tiered policy, independent finance/compliance review when material, and versioned approval.
6. **Experiment** using mock interactions, synthetic data, replay and controlled rollout where permitted; never perform live money movement in a test.
7. **Measure** real prevention outcomes and regressions, with rollback and durable learning record.

## Typed contracts
LearningSignal = {SignalId; ExceptionRefs; SourceComponent; SponsorScope; PatternKey; ObservedAt; EvidenceRefs; Denominator; Severity; DetectionConfidence; DataQuality}.
RootCauseHypothesis = {HypothesisId; Mechanism; SupportingEvidence; ContradictingEvidence; AlternativeExplanations; ConfidenceCalibration; AffectedWorkflow; ImpactEstimate; Unknowns}.
InterventionProposal = {ProposalId; TargetComponent; TargetContractOrUI; CurrentBehavior; ProposedBehavior; RiskClass; ExpectedBenefit; PotentialHarm; Dependencies; SponsorApplicability; SecurityPrivacyReview; Tests; Rollback; Owner}.
ExperimentPlan = {Hypothesis; Baseline; Counterfactual; Metrics; Guardrails; SyntheticReplaySet; RolloutPopulation; StopConditions; IndependentApprovals}.
LearningDecision = {ProposalId; PolicyVersion; Decision; Authority; Evidence; ApprovedScope; Expiry; AuditRef}.
Use explicit Unknown and Disputed states, not fabricated confidence scores.

## Signal capture and root-cause grouping
Capture incident, exception and near-miss events from grants, payments, charity vetting, accounting close, contribution intake, statements, communications, IVR, document registry, retention, access controls and external providers. Track denominator and detection coverage to avoid declaring improvement merely because monitoring was reduced. Deduplicate by causal event and evidence lineage, while retaining each affected financial record and tenant authorization. Support cross-sponsor aggregated patterns only with privacy-preserving, access-controlled metrics; never expose sponsor-specific donor or financial records to another sponsor.

## Prevention intervention catalog
- **Intake and UI:** remove unnecessary fields, prefill trustworthy known facts, progressively disclose complexity, validate identifiers early, explain grant restrictions before confirmation, accessibility and localization.
- **Charity vetting:** reuse platform-wide verified evidence with 180-day reverification and material-change triggers; distinguish charity eligibility from payment destination control and sponsor grant policy.
- **Payments:** verify recipient account ownership and method eligibility before submission, preserve provider Unknown state, prevent duplicate execution with idempotency and atomic reservations.
- **Accounting:** detect stale valuations, missing custody feeds and mismatched fund postings before close cutoff; improve source mapping rather than waive reconciliations.
- **Documents:** pin approved templates and render assets, detect unsupported content early, validate accessibility and source tie-outs before batch issuance.
- **Communications:** improve recipient data and delivery preferences at intake, not repeated manual email repair.
- **Administration:** replace repetitive notes and handoffs with structured decisions, typed workflows and clear next-best-action UI.
- **External providers:** improve contracts, freshness monitoring, fallback evidence paths and safe retry behavior.
A proposed intervention must specify which failure mechanism it prevents, not merely claim to 'use AI.'

## Authorization tiers
Tier A: preapproved reversible nonfinancial UX/content improvements may be automatically deployed only within a narrow approved policy, automated tests, accessibility checks and rollback gates.
Tier B: changes to data capture, operational routing or external integrations require designated component-owner approval and security/privacy review.
Tier C: financial calculation, grants, sanctions, charity eligibility, money movement, retention, authorization, accounting recognition, legal disclosures and audit controls require independent domain governance, counsel/CPA when applicable, and explicit sponsor applicability. No autonomous AI approval or silent self-modification. Even Tier A changes cannot weaken disclosures or consent.

## Evaluation and experiments
For each proposal, maintain prechange baseline, incident rate per eligible transaction, severity-weighted harm, detection sensitivity, false holds, financial exposure, human minutes, donor friction, accessibility, processing latency, operating cost and reversals. Use historical replay and mock data with seeded edge cases; synthetic tests are not proof of production safety. Where valid, run controlled sponsor/fund cohort rollout with guardrail thresholds, power/sample-size awareness and explicit stop conditions. If experimentation on financial outcomes is inappropriate, use shadow mode or simulation. Track displaced failures and downstream costs.

## Continuous improvement workflow
PatternDetected -> EvidenceGathered -> CausalHypothesis -> InterventionDesigned -> AdversarialChallenge -> ApprovedExperiment -> Tested -> ApprovedRelease -> Monitored -> Accepted/RolledBack. Proposals may be rejected or parked. Record why and preserve alternatives. No deployment until required approvals and gates pass. When evidence does not establish causation, label the proposal exploratory.

## UI requirements
Operational Learning Console displays top recurring failure mechanisms, denominators, affected workflow, severity, cost, evidence quality, counterhypotheses, candidate redesigns, projected tradeoffs, approval authority, experiment state, actual results and regression alerts. Sponsor admins see their own scope; platform operators may see appropriately aggregated cross-sponsor insights. Every proposal links to its originating exceptions, requirements and tests. Accessible mobile-friendly UI with honest loading/unknown states.

## Governance and audit
All decisions, proposed AI changes, experiments, approvals, code/policy versions, results and rollback events have immutable provenance. Financially material evidence inherits the seven-year minimum or longer hold. Never use raw customer financial records to train third-party models without explicit authorized governance. Retain model identity and prompt/evidence references where necessary to audit an AI recommendation. Prevention proposals must not rewrite prior financial decisions.

## Acceptance criteria
1. Learning cannot mutate authoritative financial or compliance policies without required independent approval.
2. Reduction in detected incidents is not credited if detection coverage declined.
3. Each proposal includes mechanism, evidence, alternatives, risk and measurable guardrails.
4. Recurring cross-sponsor patterns cannot leak sponsor-specific data.
5. Safe low-risk changes remain within preapproved scope and have rollback.
6. Replays include adverse and counterfactual cases, not only previously observed successes.
7. Changes that move failures downstream are treated as regressions.
8. Human review is reduced only when underlying risk is genuinely reduced.
9. AI cannot directly issue grants, move funds, clear holds or alter retention.
10. A failed experiment is preserved as useful negative evidence, not erased.

## Adversarial scenarios (proposed, unexecuted)
- ADV-507: exception count falls because logging was disabled; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-508: provider outage falsely blamed on donor form; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-509: correlation presented as established root cause; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-510: AI proposes removing sanctions check to speed grants; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-511: reduced fields omit legally required affirmation; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-512: cross-sponsor pattern exposes donor details; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-513: synthetic data misses real check-handling edge case; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-514: auto-deployed UI change blocks screen reader; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-515: reversible label change alters legal meaning; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-516: charity vetting shortcut bypasses 180-day refresh; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-517: payment retry proposal ignores provider Unknown; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-518: fee policy changed without sponsor approval; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-519: reconciliation tolerance widened to hide mismatch; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-520: template change breaks as-issued evidence; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-521: experiment cohort includes protected financial decisions; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-522: small sample declared statistically conclusive; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-523: success metric ignores severity-weighted harm; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-524: fix reduces tickets but increases false approvals; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-525: AI-generated root cause lacks source evidence; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-526: rollback does not restore prior policy version; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-527: experiment silently becomes permanent; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-528: reviewer approves own material financial change; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-529: missing baseline denominator; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-530: near misses excluded from analysis; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-531: cross-tenant aggregation allows reidentification; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-532: AI recommendation used to train external model without authorization; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-533: proposed fix creates inaccessible embedded widget; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-534: provider contract change lacks audit trail; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-535: historical evidence overwritten by new workflow; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-536: failed intervention removed from learning history; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-537: financial records deleted before retention floor; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-538: approval expires but change continues deploying; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-539: upstream validation shifts cost to charity staff; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-540: false holds hidden to improve automation metric; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-541: customer friction reduced by suppressing disclosures; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.
- ADV-542: sponsor-specific restriction ignored in shared rule; expected: independent evidence, appropriate approval, preserved controls and measured safety outcomes.

## Open decisions
Determine threshold for recurring-pattern investigation, policy tier definitions, deployment governance, experiment ethics, privacy-preserving aggregation, measurement windows, minimum evidence for causal claims and whether the learning component owns a generic proposal registry shared with other business domains. All require validation before implementation.

Status: proposed. ADV-507..ADV-542 not executed.

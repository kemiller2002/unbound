# Whole-Enterprise Workflow Simulation and Failure Mapping Framework
2026-11-22 | **Specification for a later simulation pass, not simulation execution.**

## Goal
When the requirements corpus is sufficiently stable, simulate **every workflow**, every material state transition and every high-risk interaction between workflows. The goal is to discover what can go wrong, determine where the platform detects and handles it, and identify missing controls, requirements and user experiences before implementation.

## Coverage inventory and entry criteria
Create a canonical workflow registry across onboarding, participant authority, succession, contributions, grants, charity eligibility, investments, NAV, accounting, custody, fees, partner tenancy, service cases, agent collaboration, security, control assurance, documents, audit and account closure. Deduplicate overlapping workflow names but preserve all source references. Assign stable workflow IDs and versions. No workflow is declared simulated until all transitions, authority boundaries, external dependencies and relevant failure classes have reviewed scenarios. The initial 24 scenarios are **seed cases**, not a claim of coverage.

## Five simulation layers
1. **Happy path:** valid data, authorized actors, available dependencies, complete evidence, correct outcome.
2. **Local failures:** bad/missing/stale inputs, invalid permissions, timeouts, duplicates, accessibility failures and erroneous agent claims at each step.
3. **Cross-workflow interactions:** two concurrent changes, grants vs funding, succession vs advisor roles, legal status vs approval, custody vs ledger and partner scope vs shared infrastructure.
4. **Systemic failures:** partial outage, evidence collector failure, policy rollout, model regression, external source drift, disaster recovery and compromised host.
5. **Adversarial and organizational failures:** insider misuse, colluding roles/agents, prompt injection, forged evidence, unavailable reviewer, incentives to bypass controls and silent control erosion.

## Per-transition simulation contract
1. `workflow_id`
2. `workflow_version`
3. `business_outcome`
4. `trigger`
5. `initiating_actor`
6. `sponsor_scope`
7. `partner_scope`
8. `daf_scope`
9. `preconditions`
10. `state_before`
11. `transition`
12. `expected_state_after`
13. `authority_required`
14. `input_sources`
15. `assumptions`
16. `failure_mode_id`
17. `injection_point`
18. `fault_payload`
19. `detection_signal`
20. `detection_owner`
21. `containment_action`
22. `continue_pause_or_stop`
23. `human_escalation`
24. `user_visible_message`
25. `retry_or_compensation`
26. `reconciliation_method`
27. `recovery_evidence`
28. `independent_reviewer`
29. `audit_event`
30. `privacy_impact`
31. `financial_impact`
32. `legal_impact`
33. `acceptance_assertion`
34. `test_data`
35. `execution_status`
36. `unresolved_decision`

## Failure handling proof
A scenario passes design review only when it identifies (a) detection signal and independent evidence, (b) actor or system responsible for detection, (c) safe containment/continuation, (d) authorized resolution path, (e) visible user state and communications, (f) reconciliation or compensation for partial effects, (g) independent recovery verification, and (h) regression acceptance assertions. 'Log an error' alone is not adequate handling. 'Escalate to human' is incomplete without routing, deadline, authority, fallback and outcome.

## Coverage measures
Workflow inventory coverage = reviewed workflows / known workflows.
Transition coverage = reviewed transitions / identified transitions.
Failure injection coverage = reviewed applicable failure-mode injections / required injections.
Recovery completeness = scenarios with verified detection, containment, resolution and recovery contract / all reviewed failure scenarios.
Authority coverage = transitions with explicit actor and delegation policy / consequential transitions.
Cross-workflow coverage = reviewed high-risk dependency pairs / identified high-risk dependency pairs.
Always show denominators and exclusions. Counts are specification coverage, **not executed test coverage**.

## Required later pass procedure
1. Freeze a versioned requirements baseline and gather all workflow sources from the repository.
2. Normalize workflow IDs, state machines, triggers, owners, API/event boundaries and dependencies.
3. Build workflow dependency graph and risk-ranked transition inventory.
4. Generate systematic fault injections for every transition, including timing and concurrency.
5. Run tabletop simulations and model-based reasoning with independent reviewer challenge.
6. Record detection, containment, authorized resolution, evidence and user-visible states.
7. Create new or corrected requirements for every gap, contradiction or unhandled failure.
8. Re-run simulations after revisions until high-risk scenarios have no unexplained outcomes.
9. Publish a traceable coverage report with untested areas and unresolved decisions.
10. Only after separate implementation authorization, translate scenarios into executable tests and validate against real systems.

## Design principles
Never confuse requirements-level simulation with running code, testing integrations or proving SOC 2 operating effectiveness. Do not treat generated scenarios as independently verified. Failure scenarios should challenge sponsor authority, asset ownership, privacy, data rights, accounting balance, NAV integrity and live payment prohibition.

## Initial fault taxonomy
- **FM-01 Missing or incomplete input:** Request minimal evidence; park task without inventing defaults.
- **FM-02 False or contradictory input:** Preserve source claims, detect conflict and escalate material ambiguity.
- **FM-03 Stale external data:** Label freshness, reevaluate applicability and block consequential decisions where required.
- **FM-04 Identity mismatch:** Hold permissions and sensitive actions until verified.
- **FM-05 Authority revoked mid-flight:** Cancel or suspend pending actions; reconcile any committed effects.
- **FM-06 Policy or law changes:** Version applicability, reassess affected decisions and seek qualified review.
- **FM-07 Agent hallucination:** Require source verification and independent decision gates.
- **FM-08 Prompt injection:** Ignore untrusted instructions, record attempt and isolate tool authority.
- **FM-09 Conflicting agents:** Create discrepancy with evidence, no silent consensus.
- **FM-10 External service outage:** Circuit break, queue safely, show degraded status and retry within bounds.
- **FM-11 Timeout with unknown outcome:** Reconcile using idempotency/correlation before retry.
- **FM-12 Duplicate delivery:** Deduplicate commands and notifications; preserve delivery evidence.
- **FM-13 Partial multi-step success:** Identify committed effects and compensate by authorized correction.
- **FM-14 Concurrent workflow collision:** Detect optimistic version conflicts and coordinate ownership.
- **FM-15 Tenant or scope leakage:** Deny, contain, audit and escalate privacy/security incident.
- **FM-16 Evidence collector failure:** Independent heartbeat and missing-evidence deficiency.
- **FM-17 Accounting or custody mismatch:** Hold dependent decisions and independently reconcile.
- **FM-18 Recipient eligibility drift:** Reevaluate before approval and execution, preserve prior decision.
- **FM-19 Approval invalidated by mutation:** Require renewed authorization on material change.
- **FM-20 Human reviewer unavailable:** Reassign through approved delegation and preserve deadline.
- **FM-21 Compensating control expires:** Enforce fail-closed or approved continuity route.
- **FM-22 Accessibility task failure:** Provide equivalent alternative and track usability defect.
- **FM-23 Widget host compromise:** Revoke embed grant and contain scoped exposure.
- **FM-24 Unsafe or misleading notification:** Suppress sensitive content, revoke recipient access and remediate.
- **FM-25 Audit export inconsistency:** Block publication until snapshot and denominator reconcile.
- **FM-26 Live payment attempt:** Deny; outbound live execution remains deferred.
- **FM-27 Recovery replays stale command:** Reauthorize and reevaluate before resumption.
- **FM-28 Unknown unknown / unclassified event:** Default to bounded safe state, open incident and learn through governed change.

## Required outputs of later pass
Canonical workflow registry, per-workflow transition graph, fault injection matrix, cross-workflow collision graph, exception and recovery maps, role/authority decision table, UI state/error contract, requirements gap register, reviewed simulation transcripts and honest coverage metrics.

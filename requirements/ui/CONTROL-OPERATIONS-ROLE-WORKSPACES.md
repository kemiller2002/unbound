# Control Operations UX and Role-Based Workspaces
2026-11-22 | Forma/Limen/Folio requirements only.

## UX principles
Design around the next authorized decision, not a generic dashboard. Show cause, evidence, risk, consequence, authority and next step in one coherent flow. Every view must distinguish specified, implemented, operating, tested and auditor-reviewed. Mobile-first, WCAG 2.2 AA target, nonblank WASM loading, clear empty/error/offline states, screen reader and keyboard navigation, accessible contrast and touch targets. Use approved Echelon design tokens after verification.

## Role workspaces
### Sponsor Administrator
Primary needs: Operations setup, membership, workflow configuration, case routing. Authority boundary: Cannot change control authority or waive reserved approvals.

### Control Owner
Primary needs: Evidence due, failed checks, action queue, operating procedures, remediation. Authority boundary: Cannot independently certify own operating effectiveness.

### Independent Reviewer
Primary needs: Review queue, evidence comparisons, sampling, retest, signoff. Authority boundary: Cannot review own actions or accept undocumented exclusions.

### Compliance Officer
Primary needs: Applicability, regulatory changes, legal escalation, deficiency oversight. Authority boundary: Cannot unilaterally replace sponsor reserved decisions.

### Sponsor Executive
Primary needs: Risk exposure, material exceptions, control trends, approved delegations. Authority boundary: Executive visibility does not imply permission to alter records.

### Internal Auditor
Primary needs: Scope, population, evidence provenance, samples, exceptions, findings. Authority boundary: Read-only by default, cannot edit operational evidence.

### External Auditor
Primary needs: Time-limited requests, evidence packages, limitations and activity trail. Authority boundary: No blanket tenant access, no inferred SOC 2 opinion.

### Partner Administrator
Primary needs: Scoped local operations, partner-specific obligations and exceptions. Authority boundary: Cannot see other partners or sponsor-private intelligence.

### AI Operations Agent
Primary needs: Prepare control drafts, collect evidence, triage and recommend actions. Authority boundary: Cannot self-approve, conceal failure or mark itself effective.


## Primary flows
1. **Create control:** identify risk -> discover existing coverage -> select applicable obligation -> design evidence/negative test -> assign owner/reviewer -> independent review -> sponsor authorization -> scheduled activation when implementation is separately authorized.
2. **Operate control:** assigned work -> expected evidence -> observation -> evaluation -> exception or documented result -> independent review where needed.
3. **Remediate:** alert -> impact analysis -> accountable owner -> proposed correction -> authority approval -> independent retest -> documented closure.
4. **Transfer ownership:** departing owner -> open work summary -> candidate owner -> segregation check -> acceptance -> governance approval -> effective transfer -> access review.
5. **Audit request:** scoped period -> evidence search -> completeness and provenance checks -> redaction -> reviewer approval -> Folio export -> access expiration.
6. **Retire control:** reason -> dependency impact -> replacement/obligation review -> governance authorization -> historical retention.

## Screens and components
Control Operations Home; My Work; Control Catalog; Control Detail; Control Designer; Evidence Expectations; Evidence Explorer; Control Evaluation; Test Planner; Deficiency Case; Ownership and Delegation; Exception and Compensating Controls; Regulatory Applicability; Risk Heatmap; Auditor Requests; Folio Package Preview; Monitoring Configuration; Role and Permission Review.

## Widget contracts
MyControlTasks, MissingEvidence, ControlHealth, DeficiencyAging, IndependentReviewQueue, ControlOwnerCoverage, AssuranceCoverage, GrantControlStatus, FinancialReconciliationStatus, CharityRuleReview, AgentAuthorizationFailures, AuditorRequestProgress. Embeddable widgets use short-lived tenant- and audience-bound tokens, minimum data, scoped filters, safe stale states, revocation, no unauthorized cross-origin messaging, read-only defaults and reauthorization for consequential actions.

## Visual and interaction requirements
Show separate indicators for design, evidence completeness, testing and auditor review. Every metric exposes numerator, denominator, scope and evaluation period. No misleading overall compliance percentage. Show owner and reviewer side by side with conflict warnings. Provide direct accessible drilldown from alert to supporting evidence and authorized action. Preserve draft work offline but never approve offline or from stale authority. Avoid exposing sensitive evidence in notifications or widget previews.

## Folio output requirements
Control lifecycle register, owner handoff history, independent reviews, exception aging, sample populations, failed tests, evidence gaps, compensating controls, retirement decisions and audit-period limitations. Generate a source manifest and consistent snapshot so Forma and Folio agree.

## User research and acceptance
Test each role with realistic grants, NAV, eligibility, agent, accounting and evidence scenarios. Verify mobile accessibility, task completion, comprehension of uncertainty, ability to distinguish risk from administrative backlog, tenant switching and independent review. No interface is accepted on visual polish alone.

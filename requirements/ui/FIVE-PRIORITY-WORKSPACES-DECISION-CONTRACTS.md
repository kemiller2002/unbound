# Five Priority Workspaces: Detailed UI Decision Contracts
2026-11-22 | Requirements only.

## 1. Sponsor Command Center
**Top:** current sponsor/legal-entity context, data freshness, critical safety/financial holds, pending decisions and time-window selector.
**Middle:** prioritized exception and action queue, overdue cases, grant eligibility holds, reconciliation breaks and missing evidence; each card includes impact, accountable role and next authorized action.
**Bottom:** operating trends, service outcomes, control coverage with explicit denominators, and improvement opportunities.
**Decision contract:** staff may triage and assign within delegation; sponsor-reserved decisions require separate review. Never show 'all healthy' because evidence is missing.
**Widget candidates:** SponsorOperationalRisk, MyControlTasks, DeficiencyAging, GrantEligibilityHold, FinancialReconciliationStatus.

## 2. DAF Account Overview
**Top:** account identity, sponsor ownership statement, status, current authorized viewer role, next required action.
**Primary navigation:** Activity, Participants, Grants, Contributions, Investments, Successors, Documents, Cases, History.
**Action rail:** contextual permitted recommendations, service requests and document tasks; disallowed actions are either absent or explained.
**Critical state distinctions:** gift offered/received/accepted; grant recommended/approved/payable/settled; NAV current/stale/corrected.
**Widget candidates:** DAFAccountSummary, MyDAFActions, ContributionStatus, GrantRecommendationStatus, ServiceCaseProgress.

## 3. Participant Manager
**Primary view:** relationship graph or structured list of people, role scopes, effective dates and membership across DAFs.
**Flows:** invite, verify identity, request permission, approve delegation, accept, revoke, review access.
**Conflicts:** multiple advisors, revoked users, dual-role approval conflicts, inherited partner permissions and succession overlap.
**Security:** role change preview identifies affected actions and disclosures; prior permissions are invalidated immediately when effective.
**Widget candidates:** ParticipantPermissions, AdvisorWorkQueue.

## 4. Succession Designer
**Primary view:** editable graph of successor DAF accounts and shares, contingencies, generations and evidence requirements; accessible equivalent table and narrative.
**Simulation:** preview how a triggering event would affect distributions, account identities, governance, fees and unresolved conditions. Label as hypothetical, not legal determination.
**Checks:** total shares, missing successor identity, circular references, conflicting contingencies, revoked roles and sponsor policy.
**Decision boundary:** donor records wishes; sponsor reviews and authorizes binding transitions under governing documents and applicable law.
**Widget candidates:** SuccessorPlanStatus, DocumentRequests.

## 5. Case Workspace
**Primary view:** outcome sought, current blocker, urgency, owner, due date, next authorized action and case timeline.
**Evidence rail:** verified facts, source claims, contradictory evidence, agent hypotheses and required reviews.
**Action panel:** request information, propose resolution, assign, escalate, authorize if permitted, retest, close independently.
**Closure rule:** case resolution requires outcome proof and any mandatory independent review; never accept agent self-attestation.
**Widget candidates:** ServiceCaseProgress, MyControlTasks, MissingEvidence.

## Cross-workspace design requirements
Context must persist only within authorized scope. Switching sponsor, partner, DAF or role clears prior sensitive state. Navigation must be deep-linkable through scoped opaque identifiers and reauthorization. Empty states provide meaningful next steps. Data visualization must have semantic accessible alternatives. Never expose private information in notifications or widget previews. All important interactions must be keyboard operable and mobile touch friendly.

## Research questions
Validate actual donor and sponsor workflows through interviews; test whether account-first navigation reduces effort; compare queue-first versus account-first administration; determine which widgets third-party advisors and partners need most; evaluate how to explain AI confidence without misleading certainty; verify Forma and Folio APIs before any implementation.

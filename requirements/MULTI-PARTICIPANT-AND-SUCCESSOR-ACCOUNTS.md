# Multi-person DAF participation and successor accounts
Date: 2026-11-21 | Status: product requirements directed by owner; detailed legal/policy validation pending

## Product decisions
A donor-advised fund must support **more than two people**. There SHALL be no hard-coded primary/secondary-person pair, fixed spouse model, or two-person cap. A fund must also support **successor DAF accounts**, which are distinct from merely assigning a successor advisor to the original fund.

This design preserves the sponsor's legal ownership and control. Donor, advisor, participant, successor and beneficiary labels must never imply that individuals legally own the charitable assets. Actual role availability and succession outcomes depend on sponsor program documents and applicable law.

## Candidate requirements
- **UNB-PART-001:** A fund SHALL support an arbitrary number of participant-to-fund relationships, subject only to explicit, versioned sponsor policy and operational limits, not schema or UI cardinality.
- **UNB-PART-002:** A person or organization SHALL be able to participate in multiple funds, each with independent scoped roles and permissions.
- **UNB-PART-003:** Relationships SHALL distinguish contributor/donor, original advisor, additional advisor, authorized viewer, administrative contact, professional advisor, successor advisor, successor-designate, and sponsor staff; these are independently assignable and may overlap.
- **UNB-PART-004:** Every relationship SHALL record its own effective period, recorded time, source of authority, grantor, scope, expiry, revocation and verification state. Changes append events; no in-place mutation.
- **UNB-PART-005:** The fund SHALL support multiple concurrent recommendation-makers with separately scoped permissions; sponsor-approved policy SHALL resolve conflicts, simultaneous recommendations, limits and revocation.
- **UNB-PART-006:** Access and communications SHALL be role- and relationship-specific; adding a participant does not automatically grant access to other participants' personal data, contribution history, or unrelated funds.
- **UNB-PART-007:** Onboarding SHALL support invitation, acceptance, identity verification, declined invitation, inactive status and removal without deleting history.
- **UNB-PART-008:** Fund governance SHALL support configurable consent and approval patterns where permitted (individual recommendation, joint consent, designated lead, sponsor review), without representing donor voting as binding sponsor control.
- **UNB-PART-009:** The experience SHALL support many participants accessibly on mobile, including search, grouping, delegated administration, clear permissions and conflict explanation.

## Successor account requirements
- **UNB-SUCC-001:** A fund SHALL support a versioned succession plan with zero, one or many designated successors, priority/order, conditions, contingencies, activation triggers and sponsor approval status.
- **UNB-SUCC-002:** Succession SHALL distinguish **continuing the original fund with successor advisory privileges** from **creating one or more distinct successor DAF accounts** under sponsor-approved policy.
- **UNB-SUCC-003:** A successor DAF account SHALL have a distinct fund identifier, sponsor/program association, its own participant/privilege graph, policy, reporting and accounting projections, and an immutable link to the originating fund and succession decision.
- **UNB-SUCC-004:** A succession plan MAY specify fractional or amount-based allocations to multiple successor accounts, subject to approved minimums, restrictions, liquidity and valuation rules. Allocations must be deterministic, auditable and reconcile exactly; no assumed automatic division.
- **UNB-SUCC-005:** Succession SHALL support successive generations, successor-of-successor designations, substitute/contingent successors and configurable generation or time limits defined by sponsor policy.
- **UNB-SUCC-006:** Triggers MAY include verified death, incapacity, resignation, expiration, and other sponsor-defined events. Allegations alone SHALL NOT automatically activate privileges or transfer assets.
- **UNB-SUCC-007:** The system SHALL handle contested documents, inconsistent instructions, simultaneous deaths, unavailable or ineligible successors, minors, incapacitated successors, and no surviving designee through holds and authorized human resolution.
- **UNB-SUCC-008:** Succession activation SHALL atomically or recoverably suspend/revoke former advisory authority, establish successor permissions, enforce open-grant and standing-instruction policies, and preserve pending commitments and restrictions.
- **UNB-SUCC-009:** Successor account allocations SHALL use append-only balanced internal fund allocation entries, referencing the original and new accounts, sponsor decision, valuation basis, and effective/recorded times. No direct mutation of prior balances or history.
- **UNB-SUCC-010:** Donor/advisor-facing succession planning SHALL explain that a DAF is not personally owned or inherited, and that sponsor approval and program rules govern any successor account or advisory designation.
- **UNB-SUCC-011:** The system SHALL support sponsor-approved charitable beneficiaries or spend-down instructions when no successor account is created, without conflating these with private beneficiaries.
- **UNB-SUCC-012:** Every activation, dispute, amendment, allocation, reversal and correction SHALL be represented by immutable authoritative records; reconstruction must answer what instructions and evidence were known at any point.
- **UNB-SUCC-013:** A succession plan SHALL be previewable as a graph or allocation simulation showing successors, fallback paths, restrictions, account minimums and potential conflicts before approval.
- **UNB-SUCC-014:** Succession and successor-account creation SHALL be tenant-scoped and policy-governed, including transfers between sponsor programs and any cross-sponsor case; never assume cross-sponsor portability is automatic.

## Proposed conceptual model, subject to architecture review
- Party and Fund remain separate entities.
- FundParticipantRelationship is a many-to-many, temporal, permission-scoped relationship.
- SuccessionPlan is a versioned sponsor-approved instruction set, not a mutable field on Fund.
- SuccessionDesignation identifies the person/entity, priority, condition, allocation, and desired outcome.
- SuccessorFundLink relates origin and successor fund IDs, activation decision, allocation and provenance.
- SuccessionCase handles trigger verification, conflict resolution and approvals.
- Current access, successor hierarchy and balances are **projections** of append-only events.

## Candidate events
`FundParticipantInvited`, `FundParticipantAccepted`, `FundParticipantRoleGranted`, `FundParticipantRoleRevoked`, `SuccessionPlanProposed`, `SuccessionPlanApproved`, `SuccessionPlanSuperseded`, `SuccessionTriggerReported`, `SuccessionTriggerVerified`, `SuccessionCaseOpened`, `SuccessionActivated`, `SuccessorFundCreated`, `SuccessorFundAllocationPosted`, `SuccessionDisputeOpened`, `SuccessionDecisionCorrected`.

These are **proposed extensions**, not already present in the 58-event source catalog. Reconcile with existing `PrivilegeGranted`, `PrivilegeRevoked`, `DonorDeceased`, `JournalPosted` and `TransferOutRequested` before finalizing names or schemas.

## Acceptance and adversarial scenarios
1. Five adults have independent advisory privileges on one fund; each may act within assigned scope without a two-person cap.
2. A participant advises on three different funds; revoking one relationship does not affect the other two.
3. Two advisors submit conflicting recommendations concurrently; deterministic sponsor policy and audit trail govern the result.
4. A donor designates three successor DAF accounts with a 50/30/20 allocation; posted internal fund allocations balance and preserve all original records.
5. A donor names successors to continue the original fund rather than create new funds; no duplicate fund is created.
6. A primary successor is unavailable; the contingent successor follows the approved fallback path.
7. A disputed death notice or conflicting plan versions triggers a hold, not automatic activation.
8. A minor or incapacitated successor cannot acquire privileges outside approved policy and lawful representative authority.
9. A successor account later designates its own successors; the graph handles multiple generations without cycles or unintended reactivation.
10. A succession activation is retried or replayed; no duplicate successor fund, allocation, or external financial effect occurs.
11. An incorrectly verified trigger is corrected later; original evidence and correction remain visible, and remedial action is approved rather than history overwritten.
12. An advisor or agent attempts to change allocations, approve its own succession or view another fund's participant records; authorization denies.
13. Accessibility tests cover multi-person invitation, roles, succession graph alternatives, and dispute workflows on mobile and assistive technology.
14. A successor-account split with illiquid assets, open grant commitments, and fund minimums is held for sponsor decision rather than silently allocated.

## Research and policy decisions still open
- Which sponsor program policies allow successor advisory privileges, separate successor DAF accounts, splits and multigenerational succession?
- What legal distinction applies between an internal successor-account allocation and a transfer to another sponsoring organization?
- Are fractional splits supported for restricted or illiquid assets, and which valuation date controls?
- What happens to recurring recommendations, donor recognition, anonymity, contribution attribution and grant history?
- How are account minimums, fees, restrictions and legacy donor intent carried forward?
- Which identity verification and documentary standards establish death, incapacity, or authorized representatives?

## Traceability
Expands UNB-WF-013 (death and succession), UNB-WF-015 (dormancy), UNB-PART/UNB-SUCC above, UNB-DATA-001..012 (append-only), UNB-ADV-001..002 (delegated authority), UNB-LEG-002..003 (sponsor control), source DAF landscape WF-13, C1.1/C1.2, and the canonical Party/Relationship/Account/PrivilegeGrant model.

**Implementation gate:** requirements and conceptual design only. Conditor installation, current EF system discovery, legal review and acceptance validation remain prerequisites for application development.

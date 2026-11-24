# Multi-person DAF participation and successor accounts
Date: 2026-11-21 | Status: product requirements directed by owner; detailed legal/policy validation pending

## Product decisions
A donor-advised fund must support **more than two people**. There SHALL be no hard-coded primary/secondary-person pair, fixed spouse model, or two-person cap. A fund must also support **successor DAF accounts**, which are distinct from merely assigning a successor advisor to the original fund.

This design preserves the sponsor's legal ownership and control. Donor, advisor, participant, successor and beneficiary labels must never imply that individuals legally own the charitable assets. Actual role availability and succession outcomes depend on sponsor program documents and applicable law.

## Candidate requirements
- **UNB-PART-001:** A fund SHALL support an arbitrary number of participant-to-fund relationships, subject only to explicit, versioned sponsor policy and operational limits, not schema or UI cardinality.
- **UNB-PART-002:** A person or organization SHALL be able to participate in multiple funds, each with independent scoped roles and permissions.
- **UNB-PART-003:** Relationships SHALL distinguish contributor/donor, original advisor, additional advisor, authorized viewer, administrative contact, professional advisor, successor advisor, successor-designate, and sponsor staff; these are independently assignable and may overlap.
> Adopted 2026-11-23: refined by `RR-UNB-PART-003-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-PART-004:** Every relationship SHALL record its own effective period, recorded time, source of authority, grantor, scope, expiry, revocation and verification state. Changes append events; no in-place mutation.
- **UNB-PART-005:** The fund SHALL support multiple concurrent recommendation-makers with separately scoped permissions; sponsor-approved policy SHALL resolve conflicts, simultaneous recommendations, limits and revocation.
- **UNB-PART-006:** Access and communications SHALL be role- and relationship-specific; adding a participant does not automatically grant access to other participants' personal data, contribution history, or unrelated funds.
- **UNB-PART-007:** Onboarding SHALL support invitation, acceptance, identity verification, declined invitation, inactive status and removal without deleting history.
> Adopted 2026-11-23: refined by `RR-UNB-PART-007-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-PART-008:** Fund governance SHALL support configurable consent and approval patterns where permitted (individual recommendation, joint consent, designated lead, sponsor review), without representing donor voting as binding sponsor control.
- **UNB-PART-009:** The experience SHALL support many participants accessibly on mobile, including search, grouping, delegated administration, clear permissions and conflict explanation.

## Successor account requirements
- **UNB-SUCC-001:** A fund SHALL support a versioned succession plan with zero, one or many designated successors, priority/order, conditions, contingencies, activation triggers and sponsor approval status.
- **UNB-SUCC-002:** Succession SHALL distinguish **continuing the original fund with successor advisory privileges** from **creating one or more distinct successor DAF accounts** under sponsor-approved policy.
- **UNB-SUCC-003:** A successor DAF account SHALL have a distinct fund identifier, sponsor/program association, its own participant/privilege graph, policy, reporting and accounting projections, and an immutable link to the originating fund and succession decision.
- **UNB-SUCC-004:** A succession plan MAY specify fractional or amount-based allocations to multiple successor accounts, subject to approved minimums, restrictions, liquidity and valuation rules. Allocations must be deterministic, auditable and reconcile exactly; no assumed automatic division.
> Adopted 2026-11-23: refined by `RR-UNB-SUCC-004-r1` (adopted-provisional); the adopted requirement is below under 'Adopted reconciliation requirements'.
- **UNB-SUCC-005:** Succession SHALL support successive generations, successor-of-successor designations, substitute/contingent successors and configurable generation or time limits defined by sponsor policy.
- **UNB-SUCC-006:** Triggers MAY include verified death, incapacity, resignation, expiration, and other sponsor-defined events. Allegations alone SHALL NOT automatically activate privileges or transfer assets.
- **UNB-SUCC-007:** The system SHALL handle contested documents, inconsistent instructions, simultaneous deaths, unavailable or ineligible successors, minors, incapacitated successors, and no surviving designee through holds and authorized human resolution.
- **UNB-SUCC-008:** Succession activation SHALL atomically or recoverably suspend/revoke former advisory authority, establish successor permissions, enforce open-grant and standing-instruction policies, and preserve pending commitments and restrictions.
> Adopted 2026-11-23: superseded by `RR-UNB-SUCC-008-r1` (adopted); the adopted requirement is below under 'Adopted reconciliation requirements'.
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

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### RR-UNB-SUCC-008-r1 (supersedes `UNB-SUCC-008`) — adopted
- **Original:** `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md:28` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** The system MUST split authority transition on a death or incapacity notice into three independently recorded steps. (1) ProtectiveHold, on any notice: blocks new recommendations, recurring-instance releases and contact or payee changes for rights derived from the subject party. (2) DerivedRightsSuspended, on verified evidence: suspends every session, credential binding, delegated agent principal and privilege derived from the subject party. (3) SuccessionActivated, on sponsor approval: grants successor rights. Steps 2 and 3 MUST each be atomic within the authoritative store. Rights of other participants that do not derive from the subject MUST remain in force.
- **Acceptance criteria:**
  - AC-1: Given a fund with advisors A (donor) and B (independent co-advisor) and an unverified death notice for A; when A's credential submits a recommendation; then it is rejected with AUTH_SUBJECT_HELD, and B can still submit within scope.
  - AC-2: Given verified death of A, who delegated an AI agent; when the agent requests a grant; then it is denied, and DerivedRightsSuspended predates the request.
  - AC-3: Given a notice later proven false; when the reviewer records the correction; then RightsReinstated is appended and the original hold remains in history.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-SUCC-008-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-008-r1 (refines `REQ-X-008`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:94` @ `777a70d` (kept verbatim) · **Decisions:** RCD-022, RCD-039, RCD-040, RCD-041, RCD-046
- **Requirement:** On verified death of a party, the disposition of that party's pending items MUST follow the sponsor's versioned death_pending_grant_policy. The policy sets one value for sponsor-approved unpaid grants (complete | hold_for_review | cancel_by_sponsor_decision), one for unapproved recommendations (lapse | hold), and one for recurring templates (suspend). Each recurring instance MUST re-check current authority and eligibility at release. Rights of other participants that do not derive from the deceased MUST NOT be suspended. Default (RCD-022, RCD-039): sponsor-approved unpaid grants complete unless the sponsor policy value is hold_for_review or cancel_by_sponsor_decision. (Integration amendment 2026-11-23; replaces the interim hold_for_review default.)
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11); RCD-046: Sponsor owners confirm defaults
- **Acceptance criteria:**
  - AC-1: Given an approved unpaid grant and an unset policy; when the death is verified; then the grant moves to HeldForReview and is not paid.
  - AC-2: Given policy=complete and a charity that became ineligible; when release runs; then the grant is held by the eligibility check, not paid.
  - AC-3: Given a 5-advisor fund and the death of advisor 3; when verification occurs; then advisors 1, 2, 4 and 5 keep their scoped rights.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-008-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-SUCC-004-r1 (refines `UNB-SUCC-004`) — adopted-provisional
- **Original:** `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md:24` @ `777a70d` (kept verbatim) · **Decisions:** RCD-022
- **Requirement:** A successor allocation MUST be computed by a deterministic, versioned algorithm. Inputs: the approved plan; one sponsor-designated valuation point; the fund's available capacity net of reservations, accrued fees and approved-unpaid grants; and per-position divisibility. The algorithm MUST express shares in the fund's unit of account. It MUST assign any rounding residual by a declared rule. It MUST post one balanced allocation journal whose successor credits sum exactly to the origin debit, and MUST hold the split for sponsor decision when any constraint (minimum, restriction, illiquidity, open commitment) cannot be met.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given C=$100,000.01 and a 50/30/20 plan with residual to priority 1; when preview is computed; then the shares are 50,000.01, 30,000.00 and 20,000.00, summing exactly to C.
  - AC-2: Given one successor share below the minimum; when posting is requested; then SplitHeld is recorded and no journal is posted.
  - AC-3: Given posting is retried after a timeout; when a duplicate command arrives; then exactly one journal exists.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-SUCC-004-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-PART-007-r1 (refines `UNB-PART-007`) — adopted-provisional
- **Original:** `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md:16` @ `777a70d` (kept verbatim) · **Decisions:** RCD-023
- **Requirement:** Participant onboarding MUST record identity-related checks as four separately classified obligations, each with an authority type and owner: (a) sanctions screening; (b) identity proofing at a sponsor-configured assurance level for authentication and fraud control; (c) any customer-identification or AML program, enabled only when counsel records that it applies, by law or by contract with a custodian or payment provider; (d) source-of-funds or gift-acceptance diligence under sponsor policy. The system MUST NOT label any check 'KYC' or 'legally required' without a linked applicability record.
- **Validation still required:** RCD-023: Counsel Q8
- **Acceptance criteria:**
  - AC-1: Given a regime (c) applicability record that is absent; when onboarding runs; then no CIP-style data is requested.
  - AC-2: Given the sanctions provider is unavailable; when activation is attempted; then the role remains pending and is not activated.
  - AC-3: Given contribution received, then identity Fail; when the fund state is evaluated; then capacity is not grantable and the case follows policy.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-PART-007-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-UNB-PART-003-r1 (refines `UNB-PART-003`) — adopted
- **Original:** `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md:12` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** The relationship model MUST provide distinct role types, each independently assignable, overlapping only by explicit grant: contributor; advisory-privilege holder (original or additional); authorized viewer; administrative contact; professional advisor; successor advisor; successor-designate; legal representative (subtypes: agent under power of attorney, guardian/conservator, personal representative/executor, trustee, entity authorized signatory); and sponsor staff. Each role type MUST declare its permitted commands, the evidence required to grant it, an expiry rule, and whether it can delegate.
- **Acceptance criteria:**
  - AC-1: Given a POA document; when an agent is granted the representative role; then the agent may act only within the principal's scope, and the grant expires on revocation or death of the principal.
  - AC-2: Given a professional advisor; when it attempts a grant recommendation without an advisory-privilege role; then the attempt is denied.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-UNB-PART-003-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-061-r1 (refines `REQ-X-061`) — adopted
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:540` @ `777a70d` (kept verbatim) · **Decisions:** —
- **Requirement:** Institutional migration MUST run as a MigrationRun through Contracted > Inventoried > Mapped > ImportedToStaging > Reconciled > ParallelValidated > CutoverAuthorized > SingleWriterActive > PostCutoverVerified > Archived. Every imported standing-data record (payee and bank, successor, advisor, contact) MUST carry source system, source record id, import batch, and verification_status=imported_unverified. A payment MUST NOT be released to an imported_unverified destination. Cutover MUST fence the legacy writer before Unbound posts. Every migration MUST declare in advance a rollback boundary and the compensation procedure that applies after the first live posting.
- **Acceptance criteria:**
  - AC-1: Given an imported payee; when a grant release is attempted; then blocked until verified.
  - AC-2: Given a legacy write after fencing; when it is detected; then an incident is raised and Unbound posting pauses.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-061-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### RR-REQ-X-060-r1 (refines `REQ-X-060`) — adopted-provisional
- **Original:** `research/daf-theoretical-enterprise-validation/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md:536` @ `777a70d` (kept verbatim) · **Decisions:** RCD-024
- **Requirement:** For each legal sponsor and tax year, the system MUST produce a reproducible DAF annual reporting data package from the locked ledger. Contents: count of donor-advised funds held at year end; aggregate contributions to, grants from, and year-end value of those funds (each defined in a CPA-approved data dictionary that maps to the current form lines); the DAF_TRANSFER_OUT subset; and evidence supporting the donor written-disclosure questions. Synthetic and canary items MUST be excluded. Which account types count as donor-advised funds MUST come from a counsel-approved account-type classification register. The package MUST carry an as-of and known-at snapshot. Preparation, review, signature, filing and acceptance MUST be distinct states, with no AI signing or filing authority.
- **Validation still required:** RCD-024: Counsel on future product types
- **Acceptance criteria:**
  - AC-1: Given canary grants in the period; when the package is built; then they are excluded.
  - AC-2: Given a correction after filing; when the package is rebuilt; then a new version links to as-filed and as-filed is unchanged.
- Full record: `requirements/reconciliation/2026-11-23/data/refinements.jsonl` (RR-REQ-X-060-r1); report `requirements/reconciliation/2026-11-23/07-REFINEMENT-RECORDS.md`.

### UNB-RCN-ENT-004 (P0, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-016 · **Parent:** UNB-SUCC-008 · **Decisions:** —
- **Requirement:** On verified death or incapacity of a party, the system MUST suspend, in one transaction, all rights, sessions and delegated agent principals that derive from that party, independently of and before succession activation.
- **Acceptance criteria:**
  - AC-1: Given verified death; when a derived agent principal acts; then it is denied.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-004); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-008 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-017 · **Parent:** UNB-SUCC-004 · **Decisions:** RCD-022
- **Requirement:** The system MUST support sponsor-approved fund split and merge outside succession (for example, co-donor separation or family division), reusing the RR-UNB-SUCC-004-r1 allocation algorithm. A dispute between participants MUST hold consequential actions until sponsor resolution.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given two co-donors with conflicting split requests; when both are submitted; then the fund is held and no allocation is posted.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-008); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-009 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-025 · **Parent:** UNB-PART-003 · **Decisions:** RCD-022
- **Requirement:** Each contribution MUST record a contributor of record (the acknowledgment recipient), determined by a CPA- and counsel-approved rule for joint accounts, third-party payers and entity payers. It MUST NOT default to the fund's advisory-privilege holder.
- **Validation still required:** RCD-022: Sponsor operations owner; counsel on escheat of balances (Q11)
- **Acceptance criteria:**
  - AC-1: Given a third-party payer contribution; when the acknowledgment is generated; then it is addressed to the payer per rule, not to the advisor.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-009); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-010 (P1, origin: gap-analysis) — adopted
- **Exposed by:** RCF-ENT-026 · **Parent:** UNB-PART-003 · **Decisions:** —
- **Requirement:** The role catalogue MUST include legal-representative subtypes with evidence, scope and expiry, per RR-UNB-PART-003-r1.
- **Acceptance criteria:**
  - AC-1: Given an expired guardianship order; when the guardian acts; then denied.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-010); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-011 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-029 · **Parent:** REQ-X-060 · **Decisions:** RCD-011, RCD-024
- **Requirement:** The system MUST maintain a counsel-approved register classifying every account and product type (standard DAF, workplace account, campaign pool, designated or field-of-interest fund, etc.) as a donor-advised fund or not. That classification MUST drive excise-tax control applicability and annual reporting.
- **Validation still required:** RCD-024: Counsel on future product types
- **Acceptance criteria:**
  - AC-1: Given a new product type; when it is configured; then it cannot go live without a classification entry.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-011); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-ENT-014 (P0, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-ENT-014 · **Parent:** UNB-PART-007 · **Decisions:** RCD-023
- **Requirement:** The system MUST maintain an applicability register for identity-related regimes (sanctions, identity proofing, customer-identification/AML, source-of-funds) per sponsor, role and channel, each with authority type and counsel status, as specified in RR-UNB-PART-007-r1.
- **Validation still required:** RCD-023: Counsel Q8
- **Acceptance criteria:**
  - AC-1: Given a regime without counsel status; when onboarding runs; then the regime is not presented as legally required.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-ENT-014); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

<!-- BEGIN ADOPTED: cross-industry-discovery -->
## Adopted requirements: cross-industry discovery (2026-11-24)

Status: adopted at the product owner's direction from [requirements/discovery/2026-11-23-cross-industry](../requirements/discovery/2026-11-23-cross-industry/README.md) (RCD-001 adoption step). Each requirement applies one or more decided-provisional legal or accounting decisions; it is binding for design but each listed confirmation (counsel, CPA or auditor) is still required before production use. Earlier requirements in this file are unchanged.

- **UNB-XIN-CRM-002:** Onboarding of every advisory-privilege holder and annual attestation MUST capture declared affiliations (officer, director, employee, substantial contributor) with charities and other entities, as capacity edges with evidence and effective dates. When the contributor or advisory-privilege holder of a fund is itself a charity, the fund MUST carry a counsel-approved account-type classification (UNB-RCN-ENT-011) and grants to that charity or its affiliates MUST follow the classification's rule. Any recommendation whose recipient matches a declared or detected affiliation MUST be routed to TVCTL-14 review. A fund where a charity names itself beneficiary is classified designated/agency (UNB-XIN-ACC-001); if an organization-advised fund is a DAF, grants to the advising organization are permitted when it is a §170(b)(1)(A) organization other than a disqualified supporting organization, with the affiliation recorded (XID-CRM-004).
  - Provenance: [UNB-XIN-CRM-002](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-CRM-004 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given Advisor declares she is a director of charity C; when she recommends a grant to C; then the grant is held in TVCTL-14 review.
    - AC-2: Given A church holds an organization-advised fund; when account classification missing; then fund activation is blocked.
    - AC-3: Given Annual attestation not completed within the grace period; when a new recommendation is submitted; then it is accepted as draft but held pending attestation per sponsor policy.
    - AC-D1: Given an organization-advised DAF recommending a grant to its own §170(b)(1)(A) advising charity; when the recommendation is screened; then it proceeds with the affiliation recorded and no TVCTL-14 block unless an individual benefit is detected.
- **UNB-XIN-CRM-006:** Each fund MUST record its referral attribution (referring party and capacity, channel, date) as an immutable informational fact. Unbound MUST NOT compute, accrue or export any referral compensation based on contributions, balances or grants unless a counsel-approved compensation rule for that referral arrangement and jurisdiction set is recorded; until then attribution MUST be usable only for reporting. Decided rule (XID-CRM-002): referral compensation MUST NEVER be paid from fund assets; flat fair-market-value fees from sponsor operating funds are permitted only under a written agreement, only to persons who are not advisors or related parties of the referred fund, only in states recorded as satisfying paid-solicitor rules, and never per grant or per dollar granted.
  - Provenance: [UNB-XIN-CRM-006](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-CRM-002 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given A referral without counsel rule; when month end; then no compensation accrual or export exists.
    - AC-2: Given Attribution recorded; when corrected later; then a superseding fact is appended, original retained.
    - AC-D1: Given a referral agreement priced at 0.10% of contributions; when it is activated; then activation fails with REFERRAL_COMPENSATION_FORM_NOT_PERMITTED.
- **UNB-XIN-TRS-004:** A legal representative MUST NOT exercise advisory privileges on behalf of a principal unless (a) the instrument's verified scope covers charitable recommendations or the principal's DAF advisory privileges and (b) sponsor policy permits representative exercise; otherwise the representative receives view-only access where policy allows. Restoration of a principal's authority after incapacity MUST require verified evidence of capacity and MUST end representative grants derived from the incapacity at the same effective time. A power of attorney qualifies only if it expressly grants authority to make gifts or charitable donations; guardians and conservators act only under a court order covering charitable gifts (XID-TRS-001).
  - Provenance: [UNB-XIN-TRS-004](../requirements/discovery/2026-11-23-cross-industry/README.md); decisions XID-TRS-001 (decided-provisional; confirmations in report 16).
  - Acceptance:
    - AC-1: Given POA instrument silent on charitable acts; when agent recommends grant; then rejected with REPRESENTATIVE_SCOPE.
    - AC-2: Given Donor recovers; when evidence verified; then donor authority restored; agent grant ended.
    - AC-D1: Given a verified durable power of attorney with general financial powers but no express gift authority; when the agent submits a grant recommendation; then it is denied with REPRESENTATIVE_SCOPE_INSUFFICIENT.
<!-- END ADOPTED: cross-industry-discovery -->

<!-- BEGIN ADOPTED: convergence-2026-11-24 -->
## Converged requirements: master mission (2026-11-24)

Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation gate names the authorization still required before production use. Where a clause conflicts with an earlier requirement in this file, the clause governs and the earlier requirement is annotated.

- **CNV-ENT-001 — Death, succession, representatives and protective suspension:** On credible notice of an advisor's death the platform SHALL hold new actions by that person; on verification it SHALL suspend only that person's privileges; succession SHALL start only when no living, qualified advisor remains. Successor allocations SHALL create one new fund per successor by percentage (cents rounding, remainder per sponsor rule) with a successor-response window (default 90 days after documented notice, configurable 90–365) and at least three contact attempts through every verified channel; nonresponse SHALL route to the sponsor's configured default. Approved unpaid grants SHALL proceed unless sponsor policy says otherwise. An agent under a power of attorney MAY exercise advisory privileges only if program terms permit representatives, the instrument is acknowledged and verified and it expressly grants gift or charitable authority (or references the advisory privileges); conservators act only under a court order specifically authorizing the act; guardians of the person have no authority; otherwise the instrument SHALL be treated as evidence of incapacity. A presented power of attorney SHALL be answered within 7 business days (accept, request certification or refuse on a listed ground). Protective suspension of a representative SHALL rest on program terms: trigger is reasonable belief of exploitation; up to 15 business days, one 10-business-day extension after documented independent review, and up to 30 further business days only after a report to adult protective services, law enforcement, a regulator or a court; notice within 2 business days unless it would endanger the principal; expiry without escalation SHALL release or convert to a recorded sponsor decision. Installments already approved and reserved SHALL proceed; future occurrences of a recurring series whose only authorizing advisor has died SHALL be suspended from the date of credible notice until a successor confirms, unless program terms say otherwise.
  - Provenance: resolves RCD-022, XID-TRS-001, UNB-XIN-TRS-004#Q1, XID-TRS-003, XID-CLM-002, UNB-XIN-CLM-006#Q1; amends UNB-XIN-TRS-004, UNB-XIN-CLM-006; related findings RCF-ENT-015, RCF-ENT-017, RCF-ENT-019, RCF-ENT-021, RCF-ENT-022, RCF-ENT-025, RCF-ENT-036. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Sponsor-approved program terms on succession, dormancy, representatives and protective suspension.
  - Ownership: domain owner: Accounts and succession; financial system of record: Unbound program ledger; policy owner: Sponsor operations owner; data owner: Sponsor; operational owner: Sponsor donor services; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given two co-advisors and one verified death, when the survivor recommends a grant, then it proceeds under the survivor's recorded authority.
    - AC-2: Given a presented POA with no gift authority, when reviewed, then the response within 7 business days is a refusal citing the listed ground and succession review is opened.
    - AC-3: Given a protective suspension at day 15 with no independent review, when the deadline passes, then it is released or converted with a recorded decision.
- **CNV-ENT-002 — Disputes, exits, bulk transfers and records custody:** During a fund dispute freeze fees SHALL accrue and be collected (waiver is a separate decision), investments SHALL follow the standing policy including scheduled rebalancing, advisor-directed allocation changes and grants SHALL be held, contributions accepted, and no distribution or transfer made until the sponsor resolves the dispute or a court orders otherwise. Sponsor-initiated bulk transfers and partner-exit transfers SHALL be executed fund by fund as separate sponsor-authorized grants under a written exit policy and master agreement, each with eligibility, sanctions and hold checks; the sponsor MAY decline or condition individual transfers; a Schedule N flag SHALL be raised when the series exceeds 25% of net assets and a state attorney-general notice check when substantially all assets move. Fees and deadlines SHALL be disclosed and SHALL NOT exceed the fully allocated cost of the transfer. The sponsor SHALL be records custodian; after exit the platform SHALL keep a read-only copy 90 days after the sponsor confirms the reconciled export, backups SHALL expire no later than 35 days after that, legal holds SHALL override, held funds SHALL NOT transfer and read-only access SHALL persist until holds lift, and an exit certificate SHALL list residuals.
  - Provenance: resolves XID-EVA-004, UNB-XIN-EVA-010#Q1, XID-EVA-005, UNB-XIN-EVA-009#Q1, XID-SAS-002, UNB-XIN-SAS-007#Q1, UNB-XIN-SAS-007#Q2, XID-REC-001, RR-UNB-TEN-016-r1#Q1, XID-OTH-001, UNB-XIN-OTH-002#Q1; amends UNB-XIN-EVA-010, UNB-XIN-EVA-009, UNB-XIN-SAS-007, RR-UNB-TEN-016-r1, RR-UNB-WF-014-r1, UNB-XIN-OTH-002; related findings RCC-PLT-008. Evidence and reasoning: [convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md](convergence/2026-11-24-master/02-EVIDENCE-DOSSIERS.md), [convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md](convergence/2026-11-24-master/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md).
  - Classification: normative; release scope: initial-implementation; operating models: E1, E2, E3, E4, E5.
  - Activation gate: Executed master agreement; board-adopted exit policy; contract clause on custody and exit.
  - Ownership: domain owner: Tenancy and exit; financial system of record: Unbound program ledger; policy owner: Sponsor board; data owner: Sponsor; operational owner: Unbound operations; evidence owner: Sponsor compliance officer.
  - Acceptance:
    - AC-1: Given an exit while an investigation hold exists on fund F, when the exit runs, then F is not transferred and remains readable.
    - AC-2: Given transfers totalling 30% of net assets, when the series completes, then a Schedule N flag is recorded.
<!-- END ADOPTED: convergence-2026-11-24 -->

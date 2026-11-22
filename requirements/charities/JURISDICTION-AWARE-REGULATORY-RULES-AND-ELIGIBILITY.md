# Jurisdiction-aware charity regulatory rules and eligibility engine
Date: 2026-11-21
Status: requirements specification only; no legal validation, engine implementation, test execution or live decisions.

## Objective
Turn first-party charity intelligence into reproducible, legally scoped, evidence-backed, sponsor-authorized grant eligibility assessments. A charity's legal standing, IRS classification, solicitation registration, sanctions screening, grant-purpose restrictions and sponsor approval are distinct determinations. Preserve the outcome, not the historical administrative workflow.

## Decision architecture
1. **LegalApplicability**: determine which jurisdiction(s), laws, rules, exemptions and effective dates potentially apply to the organization, transaction, sponsor and recipient.
2. **EvidenceAssessment**: evaluate first-party evidence claims, authoritative source priority, contradictory or stale records, and identity certainty. Unknown does not mean false.
3. **SponsorPolicyAssessment**: apply sponsor/program restrictions, risk thresholds, grant purpose and approval policies. Sponsor policies cannot override legal prohibitions.
4. **AuthorityAndDisposition**: identify delegated approver, dual controls, escalation, decision expiry and whether grant can advance.
5. **ImmutableDecisionRecord**: capture input snapshots, rule/policy versions, source evidence, jurisdiction, explanation, reviewer and outcome.

### Determination vocabulary
Legal applicability: Applicable | Exempt | NotApplicable | Indeterminate | RequiresCounselReview.
Evidence status: Verified | Contradictory | Insufficient | Stale | Unavailable | Disputed.
Sponsor policy: Permitted | Prohibited | Conditional | RequiresReview.
Final operational disposition: ProceedToSponsorApproval | HoldForEvidence | HoldForReview | Block | Expired.
Do not collapse intermediate states into a single boolean.

## Requirements
- **UNB-RULE-001**: Version rules by jurisdiction, issuing authority, citation, source publication date, effective date, applicability scope, expiration and review owner.
- **UNB-RULE-002**: Distinguish federal tax deductibility, IRS exemption, state corporate standing, solicitation obligations, sanctions restrictions, international grant rules and sponsor policy.
- **UNB-RULE-003**: Represent statutory exemptions explicitly and require evidence supporting applicability; absence from a registry is not automatically disqualifying.
- **UNB-RULE-004**: Evaluate relevant jurisdictions based on legal entity, solicitation activities, sponsor domicile, grant location, recipient location and applicable transaction facts; do not infer a universal jurisdiction from mailing address.
- **UNB-RULE-005**: Support effective-date and recorded-date history for rules, source facts, interpretations and decisions.
- **UNB-RULE-006**: Maintain separately legal applicability confidence, evidence sufficiency, identity certainty and operational risk; avoid a misleading single charity score.
- **UNB-RULE-007**: Do not permit AI-generated interpretations to become binding rules without authorized legal/compliance review.
- **UNB-RULE-008**: Preserve authoritative government source citations and source-specific freshness/rights restrictions in every decision.
- **UNB-RULE-009**: Treat source absence, access outage, stale data, exemption and negative regulatory findings as different states.
- **UNB-RULE-010**: Require sponsor policy versions, program context, grant purpose, amount and recipient identity to evaluate a grant.
- **UNB-RULE-011**: Separate charity identity, legal recipient/payee, fiscal sponsor and charitable project in evaluation.
- **UNB-RULE-012**: Provide bounded deterministic rules for well-supported cases; uncertain legal or conflicting factual questions route to humans.
- **UNB-RULE-013**: Enforce sponsor approval authority and delegation limits; AI can recommend, explain and gather evidence but cannot authorize prohibited grants.
- **UNB-RULE-014**: Block downstream grant execution when legally prohibited, sanctions status unresolved at required threshold, identity uncertain or approval absent.
- **UNB-RULE-015**: Define source and decision freshness thresholds by jurisdiction and risk, including rechecks before money movement where required.
- **UNB-RULE-016**: Record explicit reasons and evidence for both approval recommendations and denials/holds.
- **UNB-RULE-017**: Allow sponsor-specific policies while preserving shared legally sourced facts and tenant isolation.
- **UNB-RULE-018**: Evaluate legal changes prospectively and retroactively when applicable, with impact analysis on prior grants; never silently rewrite historical decisions.
- **UNB-RULE-019**: Support policy simulation and shadow evaluation before activating changed rules.
- **UNB-RULE-020**: Require authorized dual control for high-risk policy/rule changes and material overrides.
- **UNB-RULE-021**: No manual override may bypass an actual legal prohibition; permitted exception paths require documented authority.
- **UNB-RULE-022**: Maintain legal interpretation review queues and track unresolved ambiguity without asserting certainty.
- **UNB-RULE-023**: Expose explainable eligibility decisions through governed APIs, accessible Forma stewardship UI and reproducible Folio dossiers.
- **UNB-RULE-024**: Support recurring monitoring, change-triggered reevaluation and sponsor notification when material status changes.
- **UNB-RULE-025**: Track jurisdiction-specific regulatory requirements independently from whether a particular state publishes a usable machine-readable dataset.
- **UNB-RULE-026**: Preserve rule and evidence lineage into accounting, grant approvals and payment instruction controls without granting execution authority.
- **UNB-RULE-027**: Test source conflict, outdated rules, exemptions, identity collisions, cross-border cases and adversarial AI recommendations.
- **UNB-RULE-028**: Implement source-rights, privacy and confidentiality controls on evidence exposed to partners, advisors, agents and downstream APIs.
- **UNB-RULE-029**: Distinguish counsel-reviewed legal assertions from research hypotheses and vendor/agency guidance; no research note automatically becomes production law.
- **UNB-RULE-030**: Provide deterministic replay of past decisions from pinned source, rule, policy and identity snapshots.
- **UNB-RULE-031**: Design a typed F# rule AST and compiler/evaluator contract, integrating Echelon Knowledge Store only after capability verification.
- **UNB-RULE-032**: Maintain independent operational controls for grant approval, payable creation, payment instruction and deferred live payment execution.

## Conceptual F# rule model
```fsharp
type Jurisdiction = Jurisdiction of string
type RuleId = RuleId of string
type RuleVersion = RuleVersion of string
type EvidenceRef = EvidenceRef of string

type Applicability =
    | Applicable
    | Exempt
    | NotApplicable
    | Indeterminate
    | RequiresCounselReview

type EvidenceState =
    | Verified
    | Contradictory
    | Insufficient
    | Stale
    | Unavailable
    | Disputed

type Disposition =
    | ProceedToSponsorApproval
    | HoldForEvidence
    | HoldForReview
    | Block
    | Expired

type RuleDefinition = {
    Id: RuleId
    Version: RuleVersion
    Jurisdiction: Jurisdiction
    Citation: string
    EffectiveFrom: System.DateTimeOffset
    EffectiveTo: System.DateTimeOffset option
    LegalApprovalRef: string option
}
```
Illustrative only; not compiled or tested.

## Jurisdiction-specific seed cases
**Indiana:** The Attorney General's fundraiser registration oversight is not a universal charity registration requirement. Absence from AG records is not automatic ineligibility.
**Texas:** Most charities do not register with the state; limited solicitation categories have requirements. Require categorical applicability evaluation.
**Florida:** Evaluate solicitation registration exemptions and entity-specific obligations; Check-A-Charity absence is not conclusive.
**California:** Distinguish periodically published adverse-status CSV from more current official registry search; preserve source timestamp and conflict.
**New York:** Registration/filing history may have limited online lookback, with older records requiring FOIL. Missing historical documents is not proof of compliance or noncompliance.
**Pennsylvania:** Charity registration and corporate status are distinct; lack of EIN search in corporate portal must not force name-only merges.
All legal interpretations are research candidates pending jurisdiction-specific counsel review.

## Acceptance scenarios
- **RULE-T01:** Indiana nonprofit not in AG fundraiser registry is not automatically blocked.
- **RULE-T02:** Texas nonprofit with no general registration remains reviewable against applicable specific categories.
- **RULE-T03:** Florida charity qualifies for a documented statutory exemption; record exemption evidence.
- **RULE-T04:** California adverse-status CSV conflicts with newer official search; flag source precedence and effective dates.
- **RULE-T05:** New York filing older than public portal lookback is unknown, not absent or noncompliant.
- **RULE-T06:** Pennsylvania corporate entity matched to IRS record only by similar name requires identity review.
- **RULE-T07:** Charity appears IRS exempt but sponsor policy prohibits specific grant purpose; sponsor policy blocks.
- **RULE-T08:** A legal prohibition cannot be overridden by sponsor policy or AI.
- **RULE-T09:** AI fabricates statutory citation; unverified rule rejected.
- **RULE-T10:** Retroactive regulatory status change triggers impacted-decision review, preserving prior decision.
- **RULE-T11:** Fiscal-sponsored project and legal grantee are separately evaluated.
- **RULE-T12:** Stale sanctions data causes required hold rather than false clearance.
- **RULE-T13:** Partner-specific policy does not leak to another partner.
- **RULE-T14:** Same inputs and pinned versions replay identical disposition and explanation.
- **RULE-T15:** Rule update shadow evaluation identifies changed dispositions before activation.
- **RULE-T16:** Approved grant does not imply authorization to issue or execute a payment.
- **RULE-T17:** Source access rights prohibit exporting evidence; generated dossier redacts restricted material.
- **RULE-T18:** Multi-jurisdiction facts produce distinct applicability outcomes and explicit unresolved issues.

## APIs and workflows
Domains: RuleRegistry, LegalAssertionReview, ApplicabilityEvaluation, EvidenceEvaluation, SponsorPolicy, GrantEligibility, DecisionExplanation, DecisionReplay, ChangeImpact, ExceptionCase, RuleSimulation, RegulatoryMonitoring. Commands are governed and auditable; no implied microservices.
Workflow: Grant proposal -> identify entity/payee/purpose -> determine jurisdictions -> select effective rules -> evaluate evidence -> apply sponsor policy -> route uncertainty -> sponsor approval -> record immutable decision -> downstream grant lifecycle. Monitoring and changed facts may reopen review.

## Open decisions
ADR-RULE-001: Counsel-approved legal rule taxonomy and ownership.
ADR-RULE-002: Jurisdiction selection and cross-border rules.
ADR-RULE-003: Source precedence and freshness thresholds.
ADR-RULE-004: Rule AST/compiler integration with Echelon Knowledge Store.
ADR-RULE-005: Sponsor policy layering and permitted overrides.
ADR-RULE-006: Retroactive rule changes and historical grant response.
ADR-RULE-007: Confidence/ambiguity thresholds for human escalation.
ADR-RULE-008: Rule lifecycle, legal review, activation and rollback.
ADR-RULE-009: Performance and deterministic replay targets.
ADR-RULE-010: Disclosure and data-rights constraints for decisions and dossiers.

## Traceability and gates
Related: requirements/charities/FIRST-PARTY-CHARITY-REGISTRY-AND-ELIGIBILITY.md; requirements/charities/COMPREHENSIVE-EXTERNAL-DATA-ACQUISITION.md; architecture/CHARITY-KNOWLEDGE-MODEL-AND-ACQUISITION-PIPELINE.md; research/charity-sources/FIVE-STATE-COMPARATIVE-FINDINGS.md.
Before implementation: counsel-reviewed legal assertion matrix; verified source data access; approved rule schema; privacy and tenant controls; scenario fixtures; governance ownership; audit/replay design. Payment execution remains deferred.

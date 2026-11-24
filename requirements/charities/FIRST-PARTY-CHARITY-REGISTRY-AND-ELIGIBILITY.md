# First-party charity registry, eligibility and intelligence
Date: 2026-11-21 | Status: requirements specification; not implemented or legally validated

## Foundational decision
**UNB-CHAR-DEC-001:** Unbound SHALL own, operate and maintain its authoritative internal charity registry and eligibility intelligence. Do not make Candid, GuideStar or any other commercial charity-data aggregator a runtime, data-model, decisioning or contractual dependency. Source evidence may be collected from primary authoritative public records, direct charity submissions, sponsors and independent investigation, subject to source terms, lawful use and verification. Internal authority means Unbound controls records and decisions; it does not mean it can ignore government source changes or certify charities without evidence.

## Canonical identity and organizational history
- **UNB-CHAR-001:** Assign each organization an immutable Unbound Charity ID independent of EIN, name, address or external identifiers.
- **UNB-CHAR-002:** Track legal name, DBA/aliases, historic names, EIN and jurisdiction-specific identifiers, incorporation, tax classification, registered address, operating locations, contacts and websites with effective dates and provenance.
- **UNB-CHAR-003:** Support charities with no US EIN, fiscal sponsorship, religious organizations, government entities, schools, foreign organizations and other legally relevant entity types; do not assume all are US 501(c)(3)s.
- **UNB-CHAR-004:** Model merger, dissolution, reinstatement, name change, EIN change, fiscal-sponsor relationship and successor entity without overwriting history.
- **UNB-CHAR-005:** Detect potential duplicates through deterministic identifiers and probabilistic similarity, but require controlled review before merging; preserve split/unmerge and source lineage.
- **UNB-CHAR-006:** Keep identity confidence separate from grant eligibility, fraud risk and payment beneficiary verification.
- **UNB-CHAR-007:** Record ownership, legal control, contacts, officers and leadership only when lawful and relevant; minimize personal information and protect sensitive records.
- **UNB-CHAR-008:** Preserve immutable evidence documents, original source payload, retrieval timestamp, effective date, checksum, licensing/redistribution terms and validation method.
- **UNB-CHAR-009:** Provide entity relationship graph for affiliates, fiscal sponsors, related organizations, chapters and renamed entities, with typed, time-bounded, evidence-backed edges.

## In-house source acquisition and stewardship
- **UNB-CHAR-010:** Maintain first-party ingestion connectors for applicable primary sources, including IRS Tax Exempt Organization Search datasets, IRS exemption/revocation records, government registration records and direct charity attestations; verify actual availability, completeness, permitted use and refresh frequency.
- **UNB-CHAR-011:** Track source freshness, last successful refresh, source coverage, known gaps, exceptions, licensing and retention rules. Never label a stale source current.
- **UNB-CHAR-012:** Normalize raw observations into canonical organization claims while retaining conflicting claims and their provenance; no silent last-write-wins.
- **UNB-CHAR-013:** Support internal stewardship queues for ambiguous identities, conflicting registrations, suspect documents, stale attestations and unmatched grantees.
- **UNB-CHAR-014:** Maintain controlled verification workflows for charities to submit evidence, confirm details and dispute incorrect records; submissions are claims until validated.
- **UNB-CHAR-015:** Support scheduled and event-driven monitoring of revocation, reinstatement, address changes, organization dissolution, sanction changes and other policy-relevant events.
- **UNB-CHAR-016:** Do not treat absence from a dataset as proof of ineligibility; apply source coverage and entity-type-aware rules.
- **UNB-CHAR-017:** Establish retention, correction, right-of-response and dispute policies for organization data, including audit trails and privacy obligations.

## Eligibility and decisioning
- **UNB-CHAR-018:** Separate factual organization status, sponsor-specific grant eligibility, jurisdiction-specific legal applicability, recipient due diligence, payment-readiness and risk assessment.
- **UNB-CHAR-019:** Represent eligibility as a time-bound, evidence-backed decision for a specific sponsor, program, grant purpose, geography, amount and date; not a universal boolean.
- **UNB-CHAR-020:** Sponsor policy engine SHALL evaluate tax classification, public charity status where required, private foundation and supporting organization considerations, foreign grant requirements, prohibited benefits, conflicts, sanctions and grant restrictions as applicable, with legal review of rules.
- **UNB-CHAR-021:** Record decision result as Eligible, Ineligible, NeedsReview or InsufficientEvidence with reasons, rule versions, source observations and authorized reviewer.
- **UNB-CHAR-022:** Use deterministic policy gates for eligibility and disbursement authority; AI may extract, reconcile, flag and propose, but may not fabricate evidence or self-approve exceptions.
- **UNB-CHAR-023:** Allow sponsor-specific overrides only within lawful and approved policy boundaries, with reason, evidence, expiry, scope and independent review.
- **UNB-CHAR-024:** Trigger reevaluation when primary-source status changes, policy changes, grant purpose changes, relevant amount thresholds change or evidence expires.
- **UNB-CHAR-025:** Historical eligibility decisions and the exact source evidence relied upon remain reproducible, even when later evidence contradicts them.
- **UNB-CHAR-026:** Distinguish identity verification, charity eligibility, payment beneficiary authentication and sanctions checks; passing one never implies passing all.
- **UNB-CHAR-027:** Maintain grantee-specific grant history, restrictions, prior diligence, complaints and sponsor interactions with strict confidentiality and partner scoping.

## Risk and fraud controls
- **UNB-CHAR-028:** Detect impersonation, suspicious address/bank changes, duplicate entities, charity hijacking, mismatched EIN/name and anomalous grant patterns; flag as evidence-based signals, not automatic guilt.
- **UNB-CHAR-029:** Payment details are separately verified, protected and access controlled; internal charity registry ownership does not justify exposing bank details broadly.
- **UNB-CHAR-030:** Support risk-based review, escalation, case management, evidence preservation and human resolution.
- **UNB-CHAR-031:** Maintain restricted lists, sponsor-specific exclusions, investigation holds and legally required screening evidence with effective dates.
- **UNB-CHAR-032:** Prevent unverified changes to legal identity or payee data from propagating into approved payment instructions without renewed authorization.
- **UNB-CHAR-033:** Define monitoring alerts with materiality, confidence, freshness and review ownership; no black-box AI risk score as sole disqualification basis.

## Internal APIs, UX, documents and isolation
- **UNB-CHAR-034:** Versioned internal APIs SHALL cover organization search, identity resolution, source observations, verification cases, status history, eligibility evaluation, evidence, monitoring, disputes, and grant linkage.
- **UNB-CHAR-035:** All read/write APIs enforce sponsor/partner/program roles, purpose limitation, field-level protection where needed, rate controls, immutable audit and idempotency.
- **UNB-CHAR-036:** Maintain shared non-sensitive canonical identity facts where legally permissible, while segregating sponsor-specific eligibility decisions, investigations, private submissions, banking details, donor/grant relationships and partner-specific intelligence.
- **UNB-CHAR-037:** The sharing model must explicitly govern whether sponsor-derived intelligence may improve common registry quality; no cross-partner private-data leakage or unauthorized reuse.
- **UNB-CHAR-038:** Forma SHALL provide accessible mobile-first organization search, identity disambiguation, source freshness, eligibility reasons, stewardship queues, direct charity verification and sponsor review.
- **UNB-CHAR-039:** Folio SHALL produce reproducible organization verification packs, eligibility decision records, due-diligence summaries, charity correspondence and audit evidence with source/version references.
- **UNB-CHAR-040:** Strata-backed immutable observations and decisions SHALL support linked corrections, temporal queries, source replay and integrity verification, subject to actual Strata capability validation.
- **UNB-CHAR-041:** Support bulk import and migration of sponsor charity records without treating unverified legacy labels as current authoritative status.
- **UNB-CHAR-042:** Internal data must remain exportable under authorized sponsor contracts and legal requirements; avoid lock-in via undocumented proprietary identifiers.

## Entity and evidence model
Canonical concepts: CharityIdentity, LegalEntityIdentifier, Alias, OrganizationalRelationship, SourceAuthority, SourceObservation, EvidenceArtifact, VerifiedClaim, IdentityResolutionDecision, EligibilityPolicyVersion, EligibilityEvaluation, RiskSignal, ReviewCase, SanctionsObservation, PaymentBeneficiaryVerification, CharityAttestation, Dispute, MonitoringSubscription, SponsorCharityRelationship and GrantCharitySnapshot.
Each claim should include subject, predicate, value, valid-time interval, observed/recorded time, source, collection method, confidence, status, evidence link, supersession and permitted-use constraints. An observed claim is not automatically a verified fact or sponsor approval.

## Required acceptance scenarios
CHAR-T01: Two charities share a similar name but have different EINs; search does not merge them.
CHAR-T02: Charity changes name and address while retaining legal identity; old grant decisions reproduce historic data.
CHAR-T03: IRS status changes after a grant recommendation but before approval; eligibility is reevaluated.
CHAR-T04: An organization absent from one IRS dataset may still be eligible under applicable alternative rules; no automatic false rejection.
CHAR-T05: Fiscal sponsor receives grant on behalf of project; legal payee and project beneficiary remain distinct.
CHAR-T06: Foreign grantee requires jurisdiction-specific review; US-only classification cannot falsely approve.
CHAR-T07: Two sources disagree about exemption status; show conflict and route to stewardship.
CHAR-T08: Source feed is stale/outage; eligibility freshness state is explicit, not falsely current.
CHAR-T09: Charity representative submits new bank details; identity and payment beneficiary changes are independently verified.
CHAR-T10: Sponsor A confidential investigation is not visible to Sponsor B despite shared public identity.
CHAR-T11: AI-generated suspicious activity alert includes evidence and can be challenged; cannot itself disqualify charity.
CHAR-T12: Duplicate records merged in error; historical grant references and provenance survive controlled unmerge.
CHAR-T13: Sponsor policy changes; new decisions use new policy, prior decisions remain reproducible.
CHAR-T14: Revoked organization reinstated; status history and effective dates preserved.
CHAR-T15: Direct charity attestation contradicts public filing; conflict is preserved, not silently resolved.
CHAR-T16: Migrated sponsor list says Eligible but primary evidence is missing; mark as legacy assertion requiring review.
CHAR-T17: Cross-partner query for private charity case or payee credentials is denied.
CHAR-T18: Charity dissolved after grant approval but before payment; sponsor rechecks payment authorization.
CHAR-T19: Data ingestion duplicates/out-of-order changes; identity history and status remain deterministic.
CHAR-T20: Source licensing prohibits redistribution; internal use and exports obey source terms.

## Decisions to research and approve
CHAR-ADR-001: Scope of primary authoritative datasets and ingestion cadence; verify source terms and coverage.
CHAR-ADR-002: First-party legal entity matching rules and steward review thresholds.
CHAR-ADR-003: Shared canonical registry versus sponsor-specific private claims and legal data-sharing.
CHAR-ADR-004: Eligibility policies for domestic public charities, other permissible recipients, foreign grants and special classes.
CHAR-ADR-005: Verification service levels, source freshness and exception policy.
CHAR-ADR-006: Charity self-service portal identity proofing and dispute workflow.
CHAR-ADR-007: Risk signals, human review, appeals and retention.
CHAR-ADR-008: Whether and how verified primary-source facts may be published or licensed externally.
CHAR-ADR-009: Source and case data privacy, security and legally mandated retention/deletion.
CHAR-ADR-010: Crosswalk of each eligibility rule to current legal sources and independent counsel review.

## Sequencing and implementation gate
1. Primary-source inventory and license/accessibility verification.
2. Canonical entity, claim, source and temporal model.
3. Identity resolution, ingestion and stewardship acceptance tests.
4. Sponsor-specific eligibility policy engine and human review.
5. Grantee portal and change monitoring.
6. Payment-beneficiary verification integration seam, with actual outbound payment execution explicitly deferred.

This file specifies a first-party internal charity information capability, not proof that source data has been acquired, an eligibility rule legally verified, a registry implemented or a provider integrated.

<!-- BEGIN ADOPTED: reconciliation-2026-11-23 -->

## Adopted reconciliation requirements (2026-11-23)

Adopted on owner instruction (2026-11-23). Original requirements above are unchanged; each is annotated where a record below refines or supersedes it. `adopted-provisional` records are binding for design but need the named validation before production. Source of truth for every field: the reconciliation data files.

### UNB-RCN-GRT-013 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-GRT-038 · **Parent:** UNB-CHAR-016 · **Decisions:** RCD-006
- **Requirement:** The system MUST maintain a counsel-approved EntityTypeEvidence matrix, giving accepted evidence for: government units; churches and conventions not required to file; group-exemption subordinates; disregarded entities of an eligible parent; and newly recognized organizations not yet in bulk files. Satisfying the matrix MUST produce a common verification status equivalent to an IRS-listed entity, recorded with its evidence class.
- **Validation still required:** RCD-006: Counsel Q2
- **Acceptance criteria:**
  - AC-1: Given a public school district absent from the EO BMF with verified government-unit evidence; when G03 is evaluated; then it passes with evidence class recorded and no human queue.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-GRT-013); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

### UNB-RCN-PLT-014 (P1, origin: gap-analysis) — adopted-provisional
- **Exposed by:** RCF-PLT-048 · **Parent:** CAP-12 donor-facing charity search; UNB-CHAR-001 · **Decisions:** RCD-029
- **Requirement:** Donors and advisors MUST be able to search the canonical charity registry by name (including known aliases), EIN, city/state and cause category, with results showing legal name, EIN, location and the current platform eligibility state (Eligible, Held, Ineligible or Unknown) with its as-of date. Selecting a result MUST bind the recommendation to the canonical charity identity, never to free text. Results MUST NOT show sponsor-private notes or holds of other sponsors. A recipient not found MUST be submittable as an identity-resolution request, not as a free-text grant.
- **Validation still required:** RCD-029: Counsel on defamation-sensitive facts
- **Acceptance criteria:**
  - AC-1: Given Two charities named 'Hope Foundation' in different states; when a donor searches 'Hope Foundation'; then both appear with EIN, city/state and eligibility state; the recommendation binds to the chosen EIN.
  - AC-2: Given A charity held by sponsor S1 for a private investigation; when a donor of sponsor S2 searches it; then the result shows only platform-level eligibility; S1's hold reason is not visible.
  - AC-3: Given No match; when the donor submits the recipient; then an identity-resolution request is created; no grant recommendation exists until it resolves.
- Full record: `requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl` (UNB-RCN-PLT-014); report `requirements/reconciliation/2026-11-23/04-MISSING-REQUIREMENTS-REGISTER.md`.

<!-- END ADOPTED: reconciliation-2026-11-23 -->

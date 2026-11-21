# Canonical charity knowledge model and source acquisition architecture
Date: 2026-11-21 | Status: requirements/architecture candidate; not implemented or source-verified

## Mission
Build Unbound's first-party, evidence-backed, historical charity intelligence system from the broadest feasible lawful external data. No Candid/GuideStar or comparable aggregator is a required runtime, data model, or eligibility decision dependency. Primary sources and direct charity evidence are preferred. External data is not automatically truth; internal knowledge must preserve who claimed what, when, with what evidence and permitted use.

## Canonical layered model
1. **SourceRegistry:** sourceId, publisher, authorityType, jurisdiction, dataset, retrievalMethod, license/terms, access restrictions, commercialUse, retention, redistribution, cadence, coverage, freshnessSLA, schemaVersion, cost, riskTier, legalReviewStatus.
2. **AcquisitionRun:** runId, sourceId, requestedAt, startedAt, completedAt, sourceSnapshotVersion, cursor, counts, errors, retries, checksum, accessPolicyVersion, workerVersion, provenance.
3. **RawEvidence:** evidenceId, sourceId, acquisitionRunId, nativeRecordId, contentHash, mediaType, permittedStoragePointer, observedAt, sourcePublishedAt, validFrom/To, rightsPolicyId, piiClass, retentionPolicy, integrityProof. Store raw bytes only when permitted; otherwise lawful references/derived claims.
4. **ObservedClaim:** claimId, subjectCandidateId, predicateId, typedValue, unit/currency, locale, sourceRecordId, evidenceId, assertionTime, observedAt, validTime, sourceConfidence, parserVersion, extractionMethod, rightsPolicyId. A claim is not a verified fact.
5. **CanonicalOrganization:** immutable Unbound charity ID, legalEntityType, jurisdiction, lifecycleStatus and identity links. Names, identifiers, addresses, affiliations and statuses are separately effective-dated claims, not destructive mutable fields.
6. **IdentifierAssertion:** namespace, value, issuingAuthority, validInterval, verificationState, evidence; EIN, state registration, foreign registration, government award ID, etc. Do not assume EIN globally unique across all time or every entity has one.
7. **OrganizationRelationship:** typed source/target entity, role (fiscal sponsor, project, chapter, parent, successor, affiliate, shared officer, grantee), evidence, valid interval, confidence, authorization to expose. Do not infer legal control from shared address.
8. **ResolutionCase/Decision:** candidate entities, deterministic matches, probabilistic signals, collision risk, steward reviewer, disposition, rationale, effective time, reversible merge/split lineage.
9. **VerifiedFact:** claim set, adjudication policy version, status (verified, disputed, stale, unknown, superseded), authorized steward, evidence and contradiction references; verification is domain/temporal scoped.
10. **EligibilityEvaluation:** sponsorId, partnerId, programId, charityId, grantPurpose, geography, amount, evaluatedAt, policyVersion, factsSnapshot, result (Eligible/Ineligible/NeedsReview/InsufficientEvidence), reasons, human approval, expiry.
11. **RiskSignal/Investigation:** typed signal, evidence, severity, confidence, due process, reviewer, disposition, expiry and partner-private scope; allegations not legal findings.
12. **CharityAttestation/Correction:** organization claimant, identity proofing, asserted field, evidence, review, accepted/rejected status, notification and audit trail.
13. **KnowledgeSnapshot:** as-of valid/recorded time, source high-water marks, claim/fact graph digest, policy and parser versions, generated reports and immutable publication ID.

## Semantic design rules
- **UNB-KNOW-001:** All canonical entities, claims, relations and decisions have stable opaque IDs and explicit namespaces.
- **UNB-KNOW-002:** Support bitemporal semantics: when a fact was true/effective versus when Unbound learned/recorded it.
- **UNB-KNOW-003:** Preserve conflicting claims and differing source authority; do not collapse into one unqualified truth field.
- **UNB-KNOW-004:** Explicitly distinguish unknown, absent from source, not applicable, stale, disputed, and verified false.
- **UNB-KNOW-005:** Confidence applies to a specific claim/matching inference, not a universal charity trust score; separate statistical score from deterministic legal eligibility.
- **UNB-KNOW-006:** Maintain typed ontology/controlled predicates with versioned definitions, provenance and unit validation, including financial periods and fiscal-year basis.
- **UNB-KNOW-007:** Support legal entity vs program/project vs fiscal sponsor vs local chapter as separate nodes and roles.
- **UNB-KNOW-008:** Reversible identity merges/splits preserve original records, external IDs, prior grants, references and history.
- **UNB-KNOW-009:** Model financial observations with reporting period, filing version/amendment, currency, measurement definition and source, not only amount.
- **UNB-KNOW-010:** Model privacy, licensing, redistribution, AI-use and retention permissions as enforceable attributes inherited from evidence into derived outputs.
- **UNB-KNOW-011:** Preserve source language and normalized translations separately, with translation provenance.
- **UNB-KNOW-012:** Cross-partner canonical public facts may be shared only when lawful; sponsor-private claims, reviews, bank details and grant relationships remain isolated.
- **UNB-KNOW-013:** An organization may dispute an internal claim; adjudication produces linked correction, not deletion of immutable required history.
- **UNB-KNOW-014:** All authoritative history is append-only, using Strata where verified and approved; projections/search indexes can be rebuilt.
- **UNB-KNOW-015:** Every externally surfaced claim supports a traceable evidence chain, freshness status, rights status and correction path.
- **UNB-KNOW-016:** Define a knowledge compiler/export contract for deterministic typed F# models and agent-readable projections, evaluating integration with echelon-knowledge-store rather than assuming its implementation or capabilities.

## Acquisition pipeline
Source discovery -> legal/access approval -> connector configuration -> snapshot/change retrieval -> immutable ingestion manifest -> validation/quarantine -> raw evidence capture if permitted -> parser/extractor -> typed observed claims -> identity candidates -> matching/review -> conflict resolution -> verified facts -> sponsor eligibility evaluation -> searchable projection/API/Folio -> monitoring/reverification.
Each stage emits append-only evidence and idempotent, resumable checkpoints. A source failure cannot silently corrupt approved decisions.

### Source adapter contract (conceptual F# types)
```fsharp
type SourceId = SourceId of string
type EvidenceId = EvidenceId of string
type CharityId = CharityId of string
type AcquisitionCursor = AcquisitionCursor of string
type RightsPolicyId = RightsPolicyId of string

type SourceManifest = {
    SourceId: SourceId
    SchemaVersion: string
    RightsPolicy: RightsPolicyId
    Jurisdiction: string
    SnapshotVersion: string option
}
type AcquisitionResult = {
    Cursor: AcquisitionCursor option
    EvidenceIds: EvidenceId list
    QuarantinedCount: int
    SourceHighWatermark: string option
}
type ClaimStatus = Observed | Verified | Disputed | Stale | Superseded
```
Types illustrate design only. No code compiled.

## Identity resolution
- **UNB-KNOW-017:** Exact identifier match is a strong candidate signal, not unconditional auto-merge; consider issuer, jurisdiction, validity and conflicting legal entities.
- **UNB-KNOW-018:** Use normalized names, aliases, addresses, websites, officers and historical relationships as reviewable probabilistic evidence; never merge solely on similarity.
- **UNB-KNOW-019:** Maintain gold-standard matching evaluation sets with false merge, missed match, precision/recall, ambiguity and high-risk collision measures.
- **UNB-KNOW-020:** High-risk merges and splits require steward review, dual control where material, and reversible lineage.
- **UNB-KNOW-021:** Fiscal sponsor/project identity and legal payee identity must remain distinguishable for grant eligibility and future payment execution.
- **UNB-KNOW-022:** Cross-jurisdiction entity matching must retain original language, transliteration, country, issuer and registration validity.

## Source orchestration and economics
- **UNB-KNOW-023:** Prefer official bulk downloads and APIs, then lawful structured feeds, then permitted web extraction, then direct charity submission; avoid bypassing technical or contractual controls.
- **UNB-KNOW-024:** Each connector declares retrieval limits, rights, costs, authentication, retry, backoff, source update cadence, schema version and failure handling.
- **UNB-KNOW-025:** Schedule acquisition by volatility and decision criticality, not one global refresh cadence; support event-driven recheck for high-risk grant decisions.
- **UNB-KNOW-026:** Source drift, parser regression, empty feeds, implausible record deltas and stale records trigger quarantine/alerts.
- **UNB-KNOW-027:** Every ingestion is resumable and idempotent; duplicate/out-of-order observations do not overwrite prior evidence.
- **UNB-KNOW-028:** Maintain source coverage matrix across US federal, 50 states, DC, territories and selected foreign jurisdictions with verified access and gaps.
- **UNB-KNOW-029:** Track acquisition/compute/storage/review cost per source and downstream decision utility, without discarding strategically valuable long-tail data solely for short-term ROI.
- **UNB-KNOW-030:** Distinguish machine extraction from human attestation and independent verification; AI may extract candidate claims but not fabricate provenance.
- **UNB-KNOW-031:** Provide freshness and confidence propagation into downstream APIs and decision policies.
- **UNB-KNOW-032:** Use deterministic reconciliation to detect contradictory government records, amendments and retroactive status changes.

## Interfaces and operations
- **UNB-KNOW-033:** Versioned APIs for source registry, acquisition jobs, evidence lookup, claims, canonical identities, relationship graph, verification cases, snapshots and eligibility evaluation.
- **UNB-KNOW-034:** Forma provides accessible mobile-first stewardship workspace with source comparisons, merge/split previews, conflicts, queues, evidence and rights/freshness badges.
- **UNB-KNOW-035:** Folio produces reproducible charity dossier, source coverage, identity decision, eligibility audit and contradiction reports.
- **UNB-KNOW-036:** All tools enforce field-level privacy, source-rights controls, tenant/partner authorization, least privilege, immutable audit and explicit external disclosure policy.
- **UNB-KNOW-037:** The graph/search/semantic index is a derived projection, not the authoritative source of truth.
- **UNB-KNOW-038:** Preserve interoperability through versioned schema exports and authorized data portability without redistributing restricted source content.
- **UNB-KNOW-039:** Source and knowledge health dashboards measure coverage, freshness, unresolved conflicts, review queues, false merges, data quality and decision impact.
- **UNB-KNOW-040:** Do not enable a public/commercial intelligence API until licensing, privacy, reliability, security and governance gates are independently approved.

## Critical acceptance scenarios
KNOW-T01: Conflicting IRS and state records remain visible with valid and recorded timestamps; sponsor decision identifies applicable authority.
KNOW-T02: Two same-name charities with distinct IDs do not merge.
KNOW-T03: A charity project under a fiscal sponsor has separate project, legal payee and eligible entity records.
KNOW-T04: An amended filing changes historical figures; both filings and downstream analyses remain reproducible.
KNOW-T05: A later-received revocation with retroactive effective date triggers time-aware reevaluation, not silent historical rewrite.
KNOW-T06: A source's rights prohibit redistribution; exports and AI responses enforce it.
KNOW-T07: An AI extraction invents an officer name; unsupported claim is rejected/quarantined.
KNOW-T08: A bad identity merge is reversed while preserving grants and historical references.
KNOW-T09: Source outage leaves known-last data marked stale; freshness-sensitive approvals are held.
KNOW-T10: Private sponsor investigation cannot be accessed by another partner through graph search, embeddings or exports.
KNOW-T11: Direct charity correction is reviewed with original claim preserved.
KNOW-T12: Replaying duplicate and out-of-order source updates yields identical canonical projection.
KNOW-T13: Cross-language transliteration matches candidates but does not force an irreversible merge.
KNOW-T14: A source parser changes field meaning; versioned definitions prevent silent corruption.
KNOW-T15: An auditor recreates eligibility at the historical decision point using recorded-time and valid-time snapshots.
KNOW-T16: A large source ingestion resumes after failure without duplicate evidence or skipped records.

## Decisions and research gates
KNOW-ADR-001: Strata capabilities for event history, ordering, bitemporal records and projections, verified experimentally.
KNOW-ADR-002: Semantic schema/ontology governance and F# compiler integration with echelon-knowledge-store.
KNOW-ADR-003: Entity resolution thresholds, human review capacity and quality targets.
KNOW-ADR-004: Rights policy enforcement for raw storage, derived claims, AI processing and API redistribution.
KNOW-ADR-005: Primary-source acquisition priority and verified dataset inventory, jurisdiction by jurisdiction.
KNOW-ADR-006: Common global public graph versus sponsor-private overlay, with data-sharing agreements.
KNOW-ADR-007: Retention/privacy reconciliation with append-only audit history.
KNOW-ADR-008: Freshness thresholds and legal source applicability by grant context.
KNOW-ADR-009: Performance, storage and acquisition cost budgets.
KNOW-ADR-010: Future external data product legal and commercial boundaries.

## Deliverables before implementation
Produce machine-readable entity/claim/relationship schema, source registry, coverage map, source-rights matrix, source-to-predicate mapping, conflict and resolution decision tables, identity matching evaluation set, bitemporal snapshot examples, role/privacy matrix, API contract catalog and adversarial test fixtures. Verify actual source availability and terms; none are claimed complete here.

Traceability: UNB-CHAR-001..042 and UNB-EXT-001..038. Payment sending remains deferred. This is a design artifact only.

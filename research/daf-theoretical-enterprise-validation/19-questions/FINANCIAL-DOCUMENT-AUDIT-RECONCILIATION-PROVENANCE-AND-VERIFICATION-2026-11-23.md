# Financial document audit, reconciliation, provenance and verification
Date: 2026-11-23. Proposed requirements and adversarial tests; not implemented or executed.

## Decision and assurance boundaries
Financial document assurance has three independently verifiable claims:
A. **Accounting tie-out**: statement data reconcile to a specifically approved accounting close, valuation snapshot and fund subledger, with documented exceptions.
B. **Issuance provenance**: approved data, policy, calculations, disclosures, template, branding, renderer and approval evidence are preserved as they existed when issued.
C. **Artifact integrity**: exact archived issued bytes recompute to stored final SHA-256; optional signatures authenticate provenance when validated.
None implies the other two. A matching hash cannot prove accounting correctness, and a balanced ledger cannot prove a PDF was unchanged.

## Core typed evidence model
FinancialCloseRef = {SponsorId; CloseId; Period; CloseVersion; LedgerCommitWatermark; SubledgerSnapshotId; ValuationSnapshotId; ReconciliationSetId; ApprovalSetId; AsKnownAt; AsEffectiveAt}.
StatementInputRef = {DocumentRevisionId; FundIds; CloseRef; FinancialDataSchemaVersion; StatementDataDigest; DisclosurePolicyVersion; BrandKitRevision; TypstTemplateRevision; CompilerVersion; FontAssetDigests; Locale; RenderManifestDigest}.
ReconciliationProof = {AssertionId; InputRefs; FormulaVersion; ComputedValues; TolerancePolicy; Result Pass/Fail/Unknown; EvidenceRefs; EvaluatedAt; EvaluatorVersion}.
IssuanceAttestation = {DocumentRevisionId; CloseRef; ValidationDigest; ApprovalRefs; IssuedAt; IssuerIdentity; ArtifactId; FinalPdfSha256; SignedManifestRef?; PublicationEventId}.
VerificationResult = {ArtifactHashMatch; ManifestIntegrity; IssuanceAuthorized; CloseReferenceValid; ReconciliationEvidenceStatus; SignatureStatus; CorrectionStatus; AsOf; EvaluatorVersion; Unknowns}. Do not compress these independent dimensions into a single misleading 'Verified' flag.

## Financial reconciliation invariants
- Statement opening balance + contributions + investment income/loss + transfers/adjustments - grants - fees = closing balance, subject to an explicit documented accounting basis and separately displayed components. Validate against authoritative fund subledger and GL control totals.
- Fund-level activity and pooled investment unit/NAV valuations reconcile to approved custody and valuation evidence; timing differences and unsettled items explicitly classified.
- Available-to-grant is not assumed identical to NAV or closing balance; explain liquidity, reservations, restrictions and pending activity.
- Every displayed transaction references stable authoritative ledger/transaction IDs; no projection-only or AI-estimated amounts.
- Zero-activity, closed, newly opened and consolidated statements still reconcile, with elimination/double-counting checks for consolidated presentations.
- Currency conversions, rounding, fractional units, fees, reversals and pending external payments have versioned calculations and tolerance policies. Tolerances cannot silently mask material discrepancies.
- Missing custodian data, stale pricing, unreconciled ledger, unknown payment execution or unresolved material exceptions block routine auto-approval/publication. Defined immaterial exception handling requires sponsor-authorized policy and independent oversight.

## Close and publication gates
1. Freeze/identify authoritative ledger and subledger watermarks for period, with effective-time and known-at-time semantics.
2. Validate approved NAV/pricing, fees, cash and custody reconciliations, outstanding checks and grants, materiality and completeness.
3. Produce immutable typed statement input snapshot and manifest; independently run statement-specific tie-outs.
4. Render via pinned Typst/Folio-compatible adapter; validate required disclosures, formatting, accessibility, and PDF content consistency against approved inputs.
5. Record preparer/approver/independent review evidence where required by sponsor governance and accounting controls. Safe deterministic cases may be automatically validated under delegated authority; do not assume statutory finance signoff can be eliminated.
6. Issue and register immutable artifact bytes with final PDF SHA-256 and versioned evidence manifest.
7. Publish only after approved close and document issuance gates; Communications notifies authorized recipients with exact revision reference.
8. Reconcile issued artifact registry, object store, publication and delivery outbox, including crash recovery and idempotency.

## Bitemporal audit and late corrections
Store both business effective date and system known/recorded date for transactions, corrections, valuations and approvals. Original statement remains immutable. A later discovery can require a new close version, restatement or prospective adjustment depending on materiality, accounting policy and sponsor governance. Never rewrite historical as-issued inputs or pretend a later-known fact was known at issuance. Link correction to original, changed facts, reasons, affected balances, approver and recipient notifications. Provide AsIssued, AsKnownAt(time), and CurrentCorrected views; the first is historical evidence, not current truth.

## Cryptographic evidence
Use versioned canonical serialization for data and evidence manifests; SHA-256 digest with algorithm agility, stored separately from exact final PDF hash. Embed statement ID and short verification reference inside PDF, not its own final-byte digest. Optional signing of the combined manifest and final PDF digest, with managed keys, key IDs, timestamp policy, rotation and archived verification evidence. Full verification recomputes exact bytes, validates manifest and authorization chain, and checks close and reconciliation evidence; cryptographic integrity alone is not a substitute for financial audit. Secure authenticated verification endpoint must not disclose balances or recipient details to unauthorized users.

## Audit UX and export
Document Center shows original issued PDF, corrected revisions, integrity state, issue date and a plain-language accounting-source summary without exposing privileged workpapers. Sponsor Finance Audit Console provides CloseId, ledger watermark, NAV snapshot, control totals, reconciliation assertions, evidence digests, policy/template revisions, approvals, issuer, artifact hashes, delivery chain and exception history. Authorized auditors can export a sealed evidence bundle with manifest, original artifact, structured input snapshot, reconciliation proof, approval events and verification instructions, subject to data minimization and retention. Bundle must distinguish verified claims, unverified claims and unavailable external evidence.

## Retention and recovery
Financial documents, as-issued source snapshots and supporting evidence must survive at least seven years, longer when required by law, sponsor policy or legal hold. Preserve hashes, signatures, keys/certificates necessary for validation, provenance and corrections. Archived PDF is retrieved as stored, never silently regenerated with newer data or templates. Verify readback after write, periodically and during restore. Report missing bytes, mismatch, invalid signature or missing evidence as separate incidents; no fabricated verification success.

## Component ownership
Fund Accounting owns ledger/close/valuation and reconciliation proof. Statement Management owns financial statement content and gating. Document Artifact Registry owns immutable bytes, digests and lineage. Folio/Typst adapter renders pinned structured inputs. Retention Policy enforces preservation. Communications records distribution. Audit/Document Center expose scoped evidence. All inside component monolith with authoritative in-process commands, no extra microservices or independent financial truth store.

## Acceptance (proposed, unexecuted)
- Given CloseId and DocumentRevisionId, authorized auditor can trace every displayed amount to authoritative approved snapshot and calculation version.
- Original document and its as-issued facts remain retrievable after correction and renderer upgrades.
- Altered PDF bytes fail hash verification even when accounting inputs still match.
- Hash-valid PDF with incorrect ledger total fails financial reconciliation assurance.
- Late transactions preserve both as-issued and as-known-at evidence.
- Stale valuation, missing source, unknown provider status or material unreconciled item blocks routine publication.
- Statement corrections link new close version, new artifact revision, approvals and notification history.
- Sealed audit bundle is reproducibly verifiable with independent checks and explicit unknowns.
- Seven-year floor and holds cover supporting evidence, not just PDF bytes.
- Cross-sponsor audit export cannot reveal unauthorized financial details.

## Adversarial scenarios (proposed, not executed)
- ADV-407: PDF hash matches but fund closing balance is wrong; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-408: GL balances but subledger fund attribution is wrong; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-409: stale NAV used for issued statement; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-410: available-to-grant misrepresented as NAV; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-411: late check clearing misclassified as paid at close; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-412: rounding differences hide material error; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-413: zero-activity fund omitted; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-414: consolidated statement double-counts transfers; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-415: renderer drops a financial table row; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-416: template silently changes required disclosure; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-417: new template replaces historical issued PDF; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-418: close version changes after statement approval; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-419: transaction backdated after issuance; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-420: known-at timestamp overwritten; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-421: corrected statement erases original; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-422: document hash altered but manifest remains valid; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-423: manifest valid but approval signature missing; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-424: signing key rotated without archive trust; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-425: unreconciled custodian amount auto-approved; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-426: unknown payment status labeled completed; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-427: external bank feed absent but statement says reconciled; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-428: auditor export omits source snapshot; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-429: sponsor A auditor sees sponsor B evidence; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-430: object restore changes bytes; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-431: PDF issued before close approval; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-432: delivery points to superseded revision; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-433: retention deletes source evidence before PDF; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.
- ADV-434: AI narrative asserts unverified accounting correctness; expected: independent reconciliation, provenance and artifact-integrity checks with fail-closed publication and auditable correction.

## Open decisions
Sponsor board/finance governance must define independent close signoff, materiality, restatement thresholds and reconciliation tolerance. CPA/counsel review financial representation and retention rules. ADRs cover canonical manifest serialization, signature/timestamping, Typst PDF accessibility, source snapshot storage, verification UX and audit bundle portability. No claim of audited financial accuracy before independent validation.

Status: proposed. ADV-407..ADV-434 unexecuted.

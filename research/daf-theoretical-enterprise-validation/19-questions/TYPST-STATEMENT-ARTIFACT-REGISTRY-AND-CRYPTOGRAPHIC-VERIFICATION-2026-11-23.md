# Typst statement rendering, immutable artifacts and cryptographic verification
Date: 2026-11-23. Proposed requirements and architecture only; no implementation or tests executed.
Extends REQ-X-032. Typst is the preferred candidate, not a verified production dependency.

## Decision
Use Typst as the preferred candidate for generating versioned, sponsor-branded financial statement PDFs behind a Folio-compatible rendering adapter, subject to ADR and proof-of-concept validation. Store the actual approved and issued PDF bytes immutably in a Statement Artifact Registry. The system must retrieve and display the exact issued PDF, never silently substitute a newly rendered approximation.

## Distinct identities and hashes
1. StatementId: stable opaque unique ID for a statement lineage (fund, period, sponsor, scope).
2. StatementRevisionId: immutable issue/correction version; revisions never overwrite earlier issued statements.
3. CanonicalEvidenceDigest: SHA-256 over an explicitly canonicalized, versioned manifest containing authoritative accounting snapshot IDs/hashes, sponsor/fund/period, template revision, Brand Kit revision, Typst template and renderer/compiler version, disclosure revision, locale, valuation and approval references, and deterministic rendering inputs. Do not hash secrets unnecessarily; store manifest under protected access.
4. Visible verification reference: short, non-secret, collision-resistant-enough lookup code derived from StatementRevisionId or digest, plus statement ID, printed in PDF footer; optionally QR code pointing to authenticated verification endpoint. A short digest is a locator, not a full cryptographic proof.
5. FinalPdfSha256: SHA-256 of the **exact finished PDF bytes**, computed after embedding visible identifiers, QR, fonts and metadata. Store digest in artifact registry, never attempt to embed its own full final hash inside those bytes.
6. Optional signed manifest: digital signature of evidence digest and final PDF digest using a managed, rotated signing key. Define key IDs, signature algorithm, verification, revocation and archival trust. This is distinct from PDF digital signatures, which require an explicit signature/ByteRange workflow and validation.

## Artifact registry record
ArtifactId, StatementId, StatementRevisionId, SponsorId, FundIds/authorized consolidated scope, period start/end, accounting close version, source snapshot references, canonical manifest digest, template/layout/brand/disclosure/compiler versions, object storage location/version, byte length, MIME, final SHA-256, optional signature/key ID, creation/approval/publication timestamps, approver identity, status, supersession/correction links, retention/legal hold, access policy and delivery references. Maintain append-only evidence of reads/downloads/verification and correction publication. Object storage must be immutable or enforce retention locks appropriate to policy; no public bucket access.

## Generation pipeline
1. Close period and validate reconciliation; snapshot authoritative financial values, status and disclosure requirements.
2. Construct canonical typed StatementData and render manifest. Validate accounting rollforward, permitted scope and required fields.
3. Obtain version-pinned Typst source, theme/assets, fonts, renderer toolchain and compiler configuration; build in isolated deterministic environment with no arbitrary external network/file access.
4. Calculate canonical evidence digest; embed visible statement ID and verification code/QR in document source.
5. Compile Typst to PDF. Validate PDF structure, page count, embedded fonts, readable tables, legal text, accessibility and print properties.
6. Hash final PDF bytes; write immutable object and registry metadata; verify stored bytes by readback/hash.
7. Independent approval gate where sponsor policy requires; publish through authoritative command and secure access.
8. Notify recipient via separate Communications outbox; record provider delivery status, not presumed reading.
9. On request, serve original stored bytes; recompute SHA-256 periodically/on access when appropriate and flag integrity failures. Verification endpoint requires scoped authentication for sensitive content; public checks, if offered, disclose only minimum nonsensitive validity result.

## Correction and reproduction
Never mutate issued PDF. Correction creates a new StatementRevisionId and new artifact, linked to previous version with reason, evidence and approver; show 'Corrected' or 'Superseded' in portal and notify authorized recipients under policy. Retain original and both hashes. Reproducibility tests compare canonical financial content and rendering semantics; byte-for-byte determinism may require controlling timestamps, PDF object ordering, fonts, Typst/compiler version and metadata. Do not assert byte-identical recompilation without tests.

## Folio and Forma boundary
Folio may own reusable print/document orchestration and Typst adapter if capability audit confirms fit. Forma owns reusable visual design primitives but is not required to render regulated statements. Unbound owns statement data, legal templates, approval, artifact registry and distribution. A structured statement template editor can be considered later; prohibit untrusted executable Typst authored by ordinary client admins without sandboxing, static validation and governance. Preserve sponsor-controlled safe branding and mandatory financial sections.

## UI
Fund > Statements: period, issue date, status, revision, accessible PDF viewer, original PDF download, evidence/verification panel, delivery history, corrections and permissions. Admin > Statement Operations: batch readiness, close/reconciliation, render/validation diagnostics, approval, publish, exception queue, storage integrity checks, audit and retention. Verification panel shows exact stored SHA-256, statement revision, generation and approval times, verified/unverified state and correction chain. Never label 'cryptographically verified' merely because a stored hash exists; actual byte comparison and provenance checks required.

## Security and privacy
Protect sensitive financial PDFs at rest and in transit, enforce fund-level authorization on download and verification, minimize QR payloads and avoid permanent bearer tokens. Fonts, images, Typst packages and templates are supply-chain inputs; pin and review them. Enforce resource limits, isolated compilation, no network by default, path sandboxing, input sanitization, immutable storage policies, backup/restore, legal hold and audit logging. Signing keys remain outside app data and require controlled rotation.

## Acceptance (proposed, unexecuted)
- Retrieve exact issued bytes and verify final SHA-256.
- A single-byte mutation fails integrity verification.
- Visible code resolves only to authorized, exact statement revision.
- Issued PDF remains available after theme, font, template or Typst upgrade.
- Corrected statements preserve originals and link revisions.
- Two tenants cannot view or verify private content across scopes.
- Generated financial totals tie to authoritative snapshot and evidence manifest.
- Renderer errors, invalid assets or accessibility failures prevent publication.
- Large tables, pagination, locale, negative values and long sponsor names remain readable.
- Storage tampering or missing object raises incident, not silent regeneration.
- Optional digital signatures validate with archived key/certificate policy.
- Restoration from backup preserves original bytes, metadata and audit evidence.

## Adversarial cases (not executed)
ADV-273 PDF footer claims hash equal to final PDF digest (circularity); ADV-274 stored bytes changed after issue; ADV-275 template upgraded before historic download; ADV-276 brand asset removed; ADV-277 sponsor A verification leaks sponsor B statement; ADV-278 QR code becomes unauthorized bearer link; ADV-279 statement correction overwrites original; ADV-280 evidence manifest serialized nondeterministically; ADV-281 PDF contains dynamic timestamp defeating byte reproduction; ADV-282 Typst template accesses filesystem/network; ADV-283 untrusted font/package changed; ADV-284 huge transaction table truncates rows; ADV-285 required disclosure omitted in print; ADV-286 PDF is unreadable by screen reader; ADV-287 verification checks database hash but not object bytes; ADV-288 object restore loses version/retention; ADV-289 signed manifest key rotated; ADV-290 PDF downloaded before approval; ADV-291 stale accounting snapshot reused after correction; ADV-292 verification short code collides or guesses.

## ADRs requiring validation
- Typst runtime and F# integration strategy, version pinning, licensing and build/release supply chain.
- Folio adapter ownership versus dedicated Unbound document adapter.
- Typst PDF accessibility/tagging capabilities for target requirements, including accessible HTML/large-print fallback where needed.
- Canonical manifest schema, hash algorithm agility, signing and timestamping policy.
- Immutable storage mechanism, retention lock, cost and backup/restore proof.
- Public vs authenticated verification endpoint and privacy threat model.
- Exact PDF-byte determinism feasibility versus manifest-based semantic reproducibility.

Status: Proposed. No Typst integration, proof-of-concept, signature or tests completed.

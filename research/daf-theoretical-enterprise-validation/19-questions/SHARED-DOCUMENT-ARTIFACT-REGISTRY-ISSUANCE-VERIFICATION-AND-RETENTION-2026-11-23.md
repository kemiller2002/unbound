# Shared Document Artifact Registry: issuance, verification, history and retention
Date: 2026-11-23. Proposed requirements only; no implementation or executed tests.

## Core decision
Generalize Statement Artifact Registry into a shared, in-process Document Artifact Registry for formally issued Unbound documents. Business modules own content and legal/financial decisions; a common artifact capability owns immutable bytes, hashes, metadata, version lineage, access enforcement hooks, verification, retention and evidence. Do not make this a microservice or independent financial authority.

## Supported document families
Monthly/annual fund statements, contribution acknowledgments, grant approval/decline/disbursement letters, grant check remittance and mailing correspondence, fee notices, tax acknowledgments (not assumed equivalent to statements), regulatory notices, donor/account governance correspondence, beneficiary/successor notices and sponsor-defined formal correspondence. Not all emails or informal messages require immutable PDF issuance; ordinary communication history remains in Communications. Explicit classification decides which records are formally issued artifacts.

## Document type registry
DocumentTypeKey; owning domain component; permitted sponsor/client scope; required input schema; mandatory content/disclosures; legal identity requirements; renderer/template family; approval policy; distribution channels; retention/legal hold; recipient authorization policy; correction policy; accessibility outputs; archival metadata and verification policy. Canonical keys independent of sponsor labels and white-label wording. Version all policies.

## Lifecycle
Draft -> RenderedCandidate -> Validated -> Approved -> Issued/Published -> Distributed/Available -> Superseded/Corrected or RetiredUnderPolicy. Validation and approval are independent of rendering. Rejected drafts are not issued documents. Distinguish generated, approved, published, provider accepted, delivered and viewed; none implies another without evidence. Issuance is idempotent, scoped and an authoritative transaction. Corrections create new revisions with reason, approver and links, never overwrite original bytes.

## Artifact record
ArtifactId, DocumentType, BusinessSubjectRef, SponsorId, recipient scope, DocumentLineageId, DocumentRevisionId, document period/as-of where applicable, source snapshot references, immutable evidence manifest and SHA-256, exact issued bytes and final SHA-256, media type, object version, size, render/template/brand/asset/font/toolchain versions, required disclosures, approvals, issuance timestamp, distribution references, signature metadata if any, retention/legal hold and correction chain. Support one logical issued document with multiple immutable format variants (PDF, accessible HTML, large print), each with its own byte hash and content-equivalence validation.

## Cryptographic model
Canonicalize a versioned evidence manifest and hash it. Include visible document ID and short lookup/verification reference in the rendered PDF, optionally an authenticated QR URL. Compute final SHA-256 after rendering and store it outside PDF bytes to avoid circularity. Optional signed manifest with managed key rotation and audit; don't conflate hash integrity with provenance/authenticity or financial correctness. Verification requires checking stored bytes against expected digest and authorized provenance, not merely displaying a database value.

## Rendering adapters
Typst preferred candidate for precise PDF output behind Folio-compatible print adapter, pending accessibility, F# integration, supply-chain and operational validation. Email designer/Forma has different HTML email requirements and should not be forced to share renderer. Permit additional renderers under typed contracts, deterministic input manifests, sandboxed execution, approved assets/fonts and resource limits. Never silently regenerate an original issued document when retrieved.

## Distribution
Communications orchestrates notices and provider status; artifact registry provides authorized immutable document references. Secure portal by default; email notification with authenticated access link, approved direct attachment only under sponsor policy, physical mail with print/mailing fees, returns and delivery uncertainty. Record exactly which artifact revision each recipient received. Verify recipient authorization at access and dispatch; no publicly enumerable document IDs granting access.

## UI
Document Center for donor/advisor/sponsor authorized views with filters for type, fund, period, issued date, status and revisions. View exact issued PDF and accessible alternative; verification details, correction chain, delivery history, support request and download. Admin Document Operations: pending approval, failed validation, rendering exceptions, missing artifact, integrity alerts, delivery queue, retention/legal hold and audits. Show 'superseded' clearly but preserve original for authorized historical review.

## Reuse and boundaries
- Statement Management owns accounting/period-close decisions.
- Contribution/Grant/Compliance modules own their respective business content and approvals.
- Shared Document Artifact Registry owns immutable artifact evidence and controlled retrieval.
- Folio/Typst adapter renders approved structured inputs.
- Client Brand Kit supplies pinned identity and styles.
- Communications owns notices and delivery attempts.
- Administrative Notes and audit timelines reference ArtifactId/RevisionId without mutating issued records.
- One component monolith, typed in-process interfaces and authoritative DB transaction for issuance, with durable object storage and reconciliation of DB/object consistency.

## Failure and recovery
Stage bytes to immutable storage, verify readback hash, then commit authoritative issuance metadata using a recoverable state machine. Handle orphaned staged objects and registry records whose objects are missing. Never report Issued if bytes unavailable or unverified. Use outbox for distribution, idempotent retries, periodic integrity scans, restore tests, and incident escalation. Document exactly which atomicity properties are and are not possible across DB/object store boundaries.

## Privacy and legal
Encrypt sensitive documents, enforce sponsor/fund/recipient scope, audit access, prevent unauthorized indexing, avoid sensitive QR payloads and public links, support legal holds, retention and controlled deletion. Retention periods and tax/legal representations must be set by sponsor policy and counsel, not inferred globally. Protect against signing-key compromise, document substitution and asset supply-chain tampering.

## Acceptance tests (proposed)
- Issued document retrieved byte-for-byte unchanged after years, upgrades and brand changes.
- Mutation or missing object detected, not silently repaired via regeneration.
- Cross-sponsor/fund access denied even with known ArtifactId.
- Correction preserves original and new revision with auditable chain.
- Email/print/portal delivery records exact revision and accurate provider status.
- Statement figures remain sourced from reconciled accounting, while tax acknowledgment semantics are separately governed.
- Multiple format variants remain semantically equivalent and independently hashed.
- Idempotent issuance survives crash between object write and DB commit.
- Retention/legal hold cannot be bypassed by ordinary administrators.
- A verified hash alone never labels document legally valid or financially correct.

## Adversarial scenarios (unexecuted)
ADV-293 generated draft mistakenly presented as issued; ADV-294 object write succeeds but DB commit fails; ADV-295 DB says issued but object missing; ADV-296 altered stored bytes pass UI check without recomputation; ADV-297 cross-tenant document ID enumeration; ADV-298 corrected document overwrites original; ADV-299 recipient authority revoked after dispatch queued; ADV-300 email references obsolete revision; ADV-301 tax receipt incorrectly uses statement disclosure; ADV-302 inaccessible PDF lacks alternative; ADV-303 PDF and accessible HTML disagree on amounts; ADV-304 malicious Typst template reads secrets; ADV-305 signing key compromised; ADV-306 legal hold document deleted; ADV-307 retention policy shortened retroactively; ADV-308 provider accepted mistaken for delivered; ADV-309 brand change rewrites historical artifact; ADV-310 QR code leaks account details; ADV-311 duplicate issuance from retried job; ADV-312 archived document restored without provenance; ADV-313 cross-document search exposes private snippets; ADV-314 audit note changes document without revision.

## Decisions to resolve
Select canonical manifest format, object immutability mechanism, signature/timestamp strategy, supported format variants and accessible PDF target; validate Typst/Folio adapter; decide public minimal validity checks versus authenticated verification; classify document types and per-type retention/approval rules; establish large-scale storage cost and recovery tests.

Status: proposed only. ADV-293..ADV-314 not executed.

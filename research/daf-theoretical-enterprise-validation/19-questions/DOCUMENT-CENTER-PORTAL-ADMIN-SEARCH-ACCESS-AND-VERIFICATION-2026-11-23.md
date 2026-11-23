# Document Center: portal, administration, search, access and verification
Date: 2026-11-23. Proposed requirements only; no implementation or executed tests.

## Purpose
Provide a shared, permission-aware Document Center for donors, advisors, successors, sponsor administrators and platform operators. Surface exact issued artifacts from the shared Document Artifact Registry, not independently regenerated documents. Present fund-context document tabs and a cross-fund consolidated view. Maintain sponsor isolation, least privilege and evidence of each retrieval.

## Personas and scope
- Donor/fund advisor: only documents for funds and periods for which current and historical entitlement policy grants access. Being an advisor today does not automatically grant access to every historical document, nor does losing an account relationship necessarily mandate destroying legally required records.
- Successor/estate representative: effective-dated, explicitly granted permissions, with restricted or staged visibility when appropriate.
- Sponsor administrator: sponsor-scoped search and casework, with separate permissions for viewing, download, reissue requests, approval and retention actions.
- Platform support/compliance: purpose-bound, audited and time-bounded access; no unrestricted universal document search by default.
- Charities: only correspondence expressly intended for the charity, not donor account statements or private fund history.
- Embedded widgets/API: only authorized document metadata and secure retrieval handoff, never public unguarded links.

## Primary user experience
Document Center with All Documents, Statements, Contributions, Grants, Notices and Archived/Corrected views. Search by authorized fund, document type, period, issue date, recipient, reference and status. Sort by issue date, period or fund. Group monthly statements by year and fund. Display meaningful empty, loading, permission-denied and partial-availability states.

Each document row/card shows title, document type, sponsor/fund, statement period or event date, issue date, revision, original/corrected/superseded status, delivery availability and accessible download/view actions. A document detail view offers inline original PDF viewer, original download, accessible alternate format, issuance and verification evidence, revision chain, delivery attempts and support/report-error action. Mobile, keyboard, screen reader, high contrast and large text supported.

## Verification UX
Show Statement/Document Revision ID, as-issued timestamp, SHA-256 of exact artifact, verified/unverified/verification-failed status based on an actual byte comparison, optional signed-manifest provenance and correction chain. Provide copyable full hash, short reference lookup and optional QR scanning with authenticated verification. A green indicator is only shown after integrity and provenance checks; do not claim a hash alone proves the document's financial or legal correctness. Never expose sensitive data through QR URLs, unauthenticated previews or search snippets.

## Access control and discovery
Query only within authorized sponsor/fund/document/recipient scopes, enforcing access before metadata, counts, search results, thumbnails, download and alternate-format retrieval. Prevent ID enumeration, cross-tenant caching, stale entitlement leakage and overbroad consolidated views. Download uses short-lived scoped authorization, not permanent public bearer links. Re-check authorization at open/download, not merely when list loads. Support delegation, revocation, deceased donors, successors, joint advisors and sponsor transfers through explicit effective-dated rules. Avoid indexing private document content into unscoped search or AI retrieval.

## Administrator operations
Admin Document Operations dashboard: pending/failed generation, approval, publication, distribution, returned mail, missing artifacts, failed integrity checks, corrected documents, legal holds and retention review. Search across permitted sponsor scope with audit reason where sensitive. Correction/reissue launches originating business-domain workflow; Document Center cannot mutate accounting or directly overwrite artifact. Admin notes, communication audit and cases link to document revisions. Export and bulk download require explicit permissions, rate limits, logging and privacy checks.

## Document retrieval and lifecycle
- Registry returns exact immutable issued bytes and immutable revision metadata.
- Superseded documents remain available to appropriately authorized users with conspicuous labels and links to current revision; no silent substitution.
- Drafts and render candidates are excluded from donor view until issuance; an approved-but-unpublished artifact is not automatically visible.
- Reissue/correction is a new revision with reason, approval and notification policy.
- Document visibility and distribution preferences are distinct: choosing paper delivery does not necessarily remove portal access.
- Retention and legal hold govern deletion/archival; no user interface promise of perpetual access unless policy supports it.

## Notifications and recurring statements
Monthly statement issuance creates Document Center availability and triggers sponsor-specific Communications template according to preference and eligibility. Email links direct to authenticated document detail, not an exposed static PDF. Track notification queued, provider accepted, delivered when evidenced, bounced and opened only if reliably supported; do not equate message delivery with document reading. Support monthly reminder/digest policy and avoid duplicate notices from retries.

## Search and performance
Use an authorization-filtered metadata index with indexed fund, type, period, date, status, recipient scope and revision; do not assume full-text PDF indexing is appropriate. Paginate and stream results; handle many years of monthly statements and large consolidated account portfolios. Cache only within scope and invalidate on access revocation. Partial index outages must not bypass source-of-truth authorization. Provide transparent stale/partial results warnings.

## Integration boundaries
Shared Document Artifact Registry owns immutable bytes and verification; owning business components control issuance and correction; Fund Accounting controls financial source data; Brand Kit supplies approved presentation; Folio/Typst renders issued PDF; Communications owns delivery; Document Center is read/access UI and orchestration. One component monolith with typed in-process contracts, not separate microservices.

## Acceptance criteria
1. Authorized users can find and download exact issued PDF across years and funds.
2. Users cannot infer unauthorized documents from counts, metadata, thumbnails, search or direct links.
3. Revoked access is enforced at download time even with stale list page.
4. Historical original and corrected versions are clearly labeled and preserved.
5. Integrity verification recomputes artifact hash and identifies mismatch/missing object.
6. Portal, admin and embedded experiences use consistent document identity and permissions.
7. Screen readers, keyboard and mobile users can search, view and download.
8. No draft or merely rendered document is shown as officially issued.
9. Monthly statement notification links to correct authorized revision.
10. Bulk exports, sensitive admin access and legal holds are audited.

## Adversarial scenarios (unexecuted)
ADV-315 user guesses document ID from another sponsor; ADV-316 search count leaks private records; ADV-317 access revoked after listing but before download; ADV-318 successor receives predecessor-only documents; ADV-319 corrected document silently replaces original; ADV-320 PDF hash displayed without checking bytes; ADV-321 cached thumbnail from another sponsor; ADV-322 notification links to superseded revision; ADV-323 PDF viewer exposes signed URL in referrer; ADV-324 legal-hold document hidden/deleted by bulk action; ADV-325 admin bulk export crosses permission boundary; ADV-326 no accessible alternative for screen-reader user; ADV-327 old fund statement omitted from cross-fund view; ADV-328 full-text search indexes private grant correspondence; ADV-329 document marked issued though storage object missing; ADV-330 retry sends duplicate monthly notices; ADV-331 revoked advisor still downloads via browser cache; ADV-332 document status says delivered when provider only accepted; ADV-333 QR verification reveals private fund balance; ADV-334 partial index outage falsely implies no documents.

## Decisions and challenges
Define historical access rules after advisor changes/death/sponsor transfer; decide whether recipient-specific copies or one shared document revision; establish permitted full-text search scope; specify downloadable alternate formats and accessible PDF compliance; determine offline/download revocation limitations (cannot retract previously downloaded bytes); define audit retention, bulk export caps and notification digest policies. Avoid claiming cryptographic verification proves correctness or that portal access can revoke a PDF already saved to someone's device.

Status: proposed. ADV-315..ADV-334 not executed.

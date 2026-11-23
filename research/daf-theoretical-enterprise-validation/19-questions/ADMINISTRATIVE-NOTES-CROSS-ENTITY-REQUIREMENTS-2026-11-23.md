# Administrative notes across Unbound records

Date: 2026-11-23. Proposed requirements, not implemented or tested. Architecture: shared in-process component within the component monolith, not a microservice.

## Product requirement

Authorized administrators must be able to create and view notes in the context of an account, grant, charity and other supported business records. The same shared notes capability should serve all entity types, with consistent access, audit, search and retention. Do not duplicate notes engines across modules.

## Initial supported subjects

- Donor account / fund: service history, preferences, operational follow-up, contact history, unresolved account issues.
- Grant recommendation / grant instance: review context, documentation requests, exception explanations, processing and payment follow-up.
- Charity: verification research, identity conflicts, outreach, address/bank verification follow-up, review history; distinguish global charity facts from sponsor-private notes.
- Contribution / asset / investment / fee / payment: processing exceptions and reconciliation follow-up.
- Sponsor, administrative case and donor/advisor relationship: operational context and escalations.

Use a typed SubjectRef containing tenant/sponsor scope, subject kind, subject ID and subject version where necessary. Do not allow a client to attach a note to a record outside its authorized scope.

## Note types and lifecycle

Types: General, ContactLog, FollowUp, ReviewContext, OperationalException, ComplianceReferral, PaymentInvestigation, DecisionRationaleReference and InternalHandoff. Types are classifications, not evidence or automatic decision authority.

Fields: NoteId, SubjectRef, author principal and controlling identity, createdAt, recordedAt, lastEditedAt, title optional, rich/plain text, classification, visibility scope, linked evidence references, attachments metadata, mentions, follow-up owner and due date, resolution status, immutable revision history, retention/legal-hold category, audit sequence. Use clear timestamps and author attribution. Editing creates a new version, never erases the earlier version; deletion is governed soft-hide/redaction where permitted, not destruction of required audit history.

Support multiple notes per subject, newest-first timeline, pinning, filters, text search, optional attachments, note templates for routine work, and linked notes where a case involves several records. Keep notes usable on mobile and accessible with keyboard/screen readers.

## Permissions and information boundaries

- Internal-only by default. Donor-facing and sponsor-facing publication requires explicit separately governed workflow, not an accidental visibility toggle.
- Role- and attribute-based permissions by sponsor, account, note classification and need-to-know. High-sensitivity compliance/legal notes require narrower access; privilege claims are not automatically established by labeling a note privileged.
- Shared charity registry must not expose one sponsor's private donor/account/grant notes to another sponsor. Distinguish global platform charity notes, sponsor-private charity notes and restricted compliance investigations.
- Author identity, read/write privileges, export privileges, retention and redaction authority are independently governed. A note cannot grant financial permissions or approve a grant.
- Minimize unnecessary personal and sensitive data; apply approved retention, redaction and legal-hold policy.

## Workflow and AI integration

A note can be tagged as follow-up, assigned to an authorized administrator, linked to a case and surfaced in a work queue. AI may summarize or propose structured follow-ups only within the user's permission scope; AI-generated content is labeled and must not silently replace original notes. Avoid automatic AI processing for sensitive notes without a permitted purpose and explicit data-governance approval.

A note alleging sanctions, charity ineligibility, fraud or bank destination compromise may initiate a structured ComplianceReferral command and protective hold assessment. It is not itself a verified finding. Similarly, a note saying 'approved' is never a substitute for the signed GrantAuthorization decision; a note saying 'paid' is not payment evidence. Preserve links to authoritative decisions and their versions.

## UI requirements

Every supported record detail page includes a Notes tab or section with note count, newest activity, add note action, author/date, classification, visibility label and optional follow-up indicator. Users can add a note without leaving the record, review prior versions, filter by note type, search and link a note to an existing case. Show prominent separation between notes and verified status/decision history. For charity pages, make the scope Global / Sponsor-private / Restricted unmistakable and default conservatively.

On a grant detail page, show notes alongside but visually separate from approvals, reservations and payment states. On an account page, include a chronological administrative timeline without merging donor-visible communications or authoritative financial ledger entries into editable notes.

## Component and data ownership

Introduce one **Administrative Notes** component, owning Note, NoteRevision, NoteSubjectLink, NoteAttachmentReference and NoteFollowUp. The component exposes typed in-process commands CreateNote, ReviseNote, ClassifyNote, LinkNote, ResolveFollowUp and authorized queries. It depends on Identity/Authority and SubjectAuthorization interfaces, with evidence/audit transactional participation. Account, Grant, Charity, Payment and Administration modules expose subject access checks but do not own separate notes tables. Note creation is normally its own authoritative transaction; note follow-up notifications and search indexing are asynchronous outbox effects. A referral into Compliance is a distinct authorized domain command, not a side effect that bypasses review.

## Acceptance scenarios (proposed, unexecuted)

| ID | Scenario | Expected result |
|---|---|---|
| ADV-141 | Admin adds note to account | Authorized note visible on account, attributed and audited |
| ADV-142 | Admin adds note to grant and charity | Each attached to correct typed subject; no ID collision |
| ADV-143 | Sponsor A adds private charity note | Sponsor B cannot see, search or export it |
| ADV-144 | Admin edits earlier note | Original revision remains reconstructable |
| ADV-145 | Admin writes 'grant approved' in note | No grant authorization or reservation created |
| ADV-146 | Note flags suspect bank destination | Referral workflow available; no unverified status mutation |
| ADV-147 | Unauthorized user guesses NoteId | Access denied independent of subject ID enumeration |
| ADV-148 | Search index lags note creation | Authoritative note still retrievable; no false missing-data claim |
| ADV-149 | Restricted legal note exported in general account report | Deny unless specifically authorized |
| ADV-150 | AI summarizes note across tenants | Scope isolation blocks leakage |
| ADV-151 | Account closed or grant archived | Notes retained/accessible under approved retention and authority |
| ADV-152 | Attachment contains malicious instructions | Treat as untrusted content; no command or policy execution |

## Open product decisions

1. Whether donor-facing shared notes/comments should be a separate product capability rather than a visibility setting on internal notes. Recommendation: separate.
2. Which note classes support attachments and maximum sizes; integrate malware scanning and document retention.
3. Which note classes require immutable append-only behavior versus revision with full audit; recommendation: revisions always preserved, stricter rules for compliance/legal notes.
4. Whether @mentions and task assignment belong to Notes or a reusable Work Queue component. Recommendation: Notes owns reference, Work Queue owns assigned task lifecycle.
5. Whether a global charity note can be shared across sponsors. Recommendation: only platform-governed non-private facts, never automatic sponsor-private sharing.

**Status:** Candidate requirements and 12 unexecuted adversarial scenarios. No implementation, testing or compliance approval claimed.

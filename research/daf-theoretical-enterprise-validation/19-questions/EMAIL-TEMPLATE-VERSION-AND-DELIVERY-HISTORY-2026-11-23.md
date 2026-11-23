# Email template version history and delivery history

Date: 2026-11-23. Proposed requirements, not implemented or tested. Extends REQ-X-027. Component monolith.

## Two separate histories

**Template history** records the complete immutable lifecycle of platform defaults and each client override. **Delivery history** records which approved version was actually rendered and sent to each authorized recipient. Both are required, linked, auditable and subject to sponsor-specific retention and privacy rules.

## Template revision history

Each revision records TemplateTypeId, SponsorId/ClientId or Platform scope, locale, RevisionId, parent revision, author and controlling principal, createdAt, change summary, field-level diff, token schema version, mandatory disclosure version, branding version, draft/review/approved/published/retired status, reviewer and approval time, effective start/end, and rollback source if applicable.

Retain immutable published subject, preheader, HTML, plain text, sender presentation, reply-to policy, token declarations, validation results and approval evidence. Draft revisions may be edited with revision logging; publication creates a fixed immutable snapshot. Client-specific history must be separate from the platform default history, while showing the inherited baseline and changes.

Allow view, compare side-by-side, filter by author/date/status/locale, inspect field and token changes, preview historical revision with synthetic data, inspect publication/retirement intervals, and create a new draft from an old version. **Rollback is a new approved published revision**, not erasure or in-place reactivation of an earlier historical record.

## Delivery/render history

Every intended message links EventId, recipient scope and verified address reference, selected template revision, token schema version, approved mandatory-clause versions, source event/evidence versions, exact rendered subject and bodies (or an equivalently reconstructable encrypted snapshot), locale, send eligibility/suppression decision, provider identity, dispatch attempt IDs, accepted/bounced/delivered/Unknown observations, timestamps and applicable audit references.

Delivery history must distinguish Queued, Rendered, Submitted, ProviderAccepted, DeliveredReported, Bounced, Suppressed, FailedBeforeSubmission and Unknown. Provider delivery reporting does not prove recipient read the email. A resend creates a linked new delivery intent/attempt according to idempotency rules and never overwrites the prior record.

## Administrator UI

Client Settings > Communications > Email Templates > Template > **History**:
- Revision timeline with version number, date, author, change summary, reviewer, status and effective period.
- Compare any two revisions, including inherited platform default versus client override.
- Preview historical subject/body in desktop/mobile/plain-text form using safe sample data.
- Show linked delivery count and individual delivery records only for authorized roles and scope.
- Restore from revision creates a draft with clear source version, then follows current validation and approval.
- Search/filter and export of audit evidence under role-based permissions.

Record detail pages (grant, account, charity) include **Communication History** showing authorized messages, delivery status and the exact template revision used. This must not expose other tenants' addresses or private content.

## Security and governance

No deletion or silent mutation of published versions or actual sent content; retention expiry and legally required redaction must follow approved policy with audit metadata. Access to historical rendered messages is more sensitive than access to template designs because it contains recipient-specific data. Token values and private information must be encrypted/protected, and historical previews must not resolve against today's live data. A legal disclaimer change does not rewrite previously sent messages. A client cannot alter platform mandatory clauses or other sponsors' revisions.

## Adversarial specifications (unexecuted)

| ID | Scenario | Expected outcome |
|---|---|---|
| ADV-205 | Published template edited directly | Reject; new version required |
| ADV-206 | Client restores older template with obsolete legal disclaimer | Current validation blocks unsafe publication |
| ADV-207 | Template changes while email queued | Deterministic bound revision or audited emergency rerender |
| ADV-208 | Historical preview resolves current grant status instead of historical snapshot | Preserve historical event and render evidence |
| ADV-209 | Sponsor A views sponsor B's template history | Deny |
| ADV-210 | Template token registry changes after email sent | Exact historical render remains reconstructable |
| ADV-211 | Administrator deletes a delivered email record | Retention/audit rules prevent silent erasure |
| ADV-212 | Provider accepted message but delivery unknown | Do not claim Delivered or Read |
| ADV-213 | Restore action overwrites original revision | Reject; create new linked draft |
| ADV-214 | Legal disclosure revised after previous send | Preserve as-sent text and effective history |
| ADV-215 | Historic email export exposes unverified recipient address | Authorization and sensitive-data controls apply |
| ADV-216 | Repeated resend obscures original delivery | Linked distinct attempts/intents with original retained |

## Acceptance

Every published template has a permanent version identity and reconstructable change/approval/effective history. Every actual communication is traceable to a precise template revision, token snapshot and provider outcome. Users can compare, preview and restore via new revision without mutating prior history. Both histories enforce sponsor and recipient privacy. No tests executed.

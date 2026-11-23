# Client email templates, token replacement and communications governance

Date: 2026-11-23. Theoretical product requirements, not implemented or tested. Architecture: shared in-process Communications component within Unbound's component monolith, external email delivery adapter.

## Core product decision

Every defined email event has a platform-owned default template. A client (sponsoring organization or white-label tenant) may override the permitted presentation and content for that event. The platform retains nonwaivable security, privacy, legal and transactional correctness requirements. A client override never changes the triggering event, its authoritative status or who is authorized to receive it.

Resolution order: explicit client-specific published template for event + locale -> client default-locale published template -> platform locale template -> platform default-locale template. A missing or invalid override falls back only when fallback is allowed by policy; legally mandatory content missing from every version blocks sending and opens an operational exception. No silent fallback to the wrong language for legally sensitive disclosures without approved policy.

## Template taxonomy and triggers

- Donor onboarding, account activation, authentication/security notifications, authority/delegation changes.
- Contribution received, accepted, rejected, pending documentation, recurring contribution schedule and failure.
- Grant recommendation received, clarification requested, recommender reply available, approved, declined, on hold, scheduled, payment submitted, payment confirmed, returned or unable to confirm.
- Charity verification requests, 180-day reverification, missing documentation and approved outreach.
- Fee disclosures and notices for paper checks, mailing, manual processing and AUM-based charges.
- Fund statements, tax acknowledgment notifications (only with independently approved legal wording), investment and account updates.
- Administrator assignment, follow-up, unresolved case, compliance alert and reconciliation exception.
- Operational notices, outage intake acknowledgment (Queued only), maintenance and support correspondence.

Each trigger has a stable EventType, EventVersion, recipient role, delivery eligibility rule, idempotency identity, sensitivity class and mandatory disclosure obligations. Never treat an event name as evidence that a bank payment has legally settled.

## Template model

TemplateDefinition: TemplateTypeId, event contract version, supported channels, platform default, required tokens, optional tokens, mandatory clauses, permitted override fields, permitted recipients and data classification.

TemplateRevision: TenantId, TemplateTypeId, Locale, Subject, Preheader, HtmlBody, PlainTextBody, SenderDisplayName, ReplyToPolicy, BrandingVersion, TokenSchemaVersion, Draft/Review/Published/Retired status, Author, Reviewer, Approval, EffectiveAt, Revision, ChangeReason and audit.

TemplateBinding: EventId, RecipientPrincipalId, TenantId, selected revision, locale, authoritative source version, rendered token snapshot, delivery state, provider message identity, consent/suppression decision, timestamp and audit reference.

Use a rich text editor with a constrained safe markup subset, plain-text alternative, accessible layout, responsive mobile rendering, sponsor logo/colors and approved footer. No arbitrary scripts, external dynamic template execution or raw SQL.

## Token replacement engine

Token syntax recommendation: {{grant.reference}}, {{grant.status}}, {{grant.amount}}, {{charity.name}}, {{account.reference}}, {{recipient.first_name}}, {{sponsor.display_name}}, {{support.phone}}, {{message.secure_link}}. This is illustrative, not a final syntax commitment.

- Maintain a **versioned typed token registry**, grouped by event and recipient role. Tokens declare data type, formatting (money/currency, date/time/time zone, number, name), sensitivity, source component, permitted recipient classes, null/Unknown behavior and whether required.
- A template can only reference allowlisted tokens for its event. No arbitrary expressions, code execution, cross-tenant lookup, free-form database paths or access to internal notes.
- Resolve tokens from an authenticated, authorized **event snapshot**, not from a mutable read model or arbitrary client-supplied data. Escape/encode output by context (HTML text, attribute, subject, URL); reject unsafe raw HTML or header injection.
- Missing required token, unresolved mandatory disclosure, unauthorized sensitive token or stale source status must block sending and create a traceable exception. Optional missing tokens use explicitly approved omission/fallback, never misleading guessed values.
- Preserve leading zeros in numeric IVR references and external grant IDs as strings; support sponsor-specific vocabulary ('grant', 'distribution', etc.) without altering the underlying typed event.
- Format currency, dates, names, pronouns and localization using recipient preferences and sponsor policy. Do not fabricate donor name, estimated payment date or bank finality.
- Secure links are short-lived, scoped and authenticated; never embed private notes, sensitive identifiers or permanent bearer tokens in email.
- Tokens representing legal disclaimers/mandatory clauses cannot be removed by client override. Use a governed compliance-fragment mechanism with approved locale versions.
- Store a delivery-time immutable render snapshot or encrypted/auditable equivalent so staff can reconstruct exactly what was sent, subject to retention/privacy controls.

## Override and governance lifecycle

Platform creates and versions default. Client authorized editor clones or customizes allowed fields into Draft, previews against synthetic fixtures, validates token and mandatory content, requests independent review where required, then publishes effective revision. Allow test send to authorized internal addresses with clear TEST labeling and no live customer data. Provide side-by-side default vs override diff, token browser, validation diagnostics, preview in desktop/mobile/plain text, rollback and scheduled effective dates.

Changes to mandatory legal/security wording, recipient eligibility, financial statuses and suppression logic require platform governance and possibly counsel approval. Client customization may be stricter but cannot weaken nonwaivable controls. Every published revision is immutable; changes create a new version. Existing queued messages bind to a deterministic version policy, with emergency security revocation and rerender rules explicitly defined.

## Delivery and recipient governance

Email send eligibility is independent of template content. Enforce sponsor scope, recipient identity/delegation, verified destination, consent/preferences, transactional versus marketing classification, bounce/suppression, security and legal restrictions. Separate email reply routing from grant shared conversation: inbound replies may create a scoped conversation message only after authenticated sender validation and anti-spoofing controls; otherwise route to secure portal or staff triage.

Transactional outbox records intended notification atomically with triggering domain event where needed. Workers resolve an approved template, render, validate, dispatch through an external provider and reconcile accepted/delivered/bounced/complained states. Provider Accepted is not proof the recipient read the email. Stable message idempotency prevents duplicate sends on retries where possible; provider uncertainty is tracked honestly.

## Administrator UI

Client settings > Communications > Email Templates: searchable list by category, event, locale, status and last published revision. Each row shows Default / Customized, editor permissions, preview and test. Editor has subject, preheader, body, token picker, mandatory-content indicators, sender/reply-to settings, live sample preview, accessible plain-text preview, validation and change history. Include Reset to platform default (future sends only) and Rollback to prior approved revision.

On account/grant/charity detail, staff can view a permission-filtered communication timeline: triggering event, template revision, recipient, send time, delivery evidence, failure and related shared conversation, without exposing private addresses or messages across sponsors. Administrative notes and shared conversations remain separate data and access domains.

## Architectural ownership

Communications component owns TemplateDefinition, TemplateRevision, TokenSchema, RenderedMessage, DeliveryIntent and DeliveryAttempt. Domain components own source facts and authoritative event contracts. Identity owns recipient authorization, Governance owns sponsor settings, Compliance/Legal owns mandatory clauses, Evidence/Audit owns trace, Integration Adapters own provider-specific transport. Communications does not own grant approval or financial state. No microservices introduced.

## Adversarial specifications (not executed)

| ID | Fault injection | Expected outcome |
|---|---|---|
| ADV-185 | Client override removes mandatory prohibited-benefit disclaimer | Publish blocked or immutable clause appended under approved rule |
| ADV-186 | Client uses token for another sponsor's account | Token access denied |
| ADV-187 | Missing required grant status token | Send blocked, exception recorded |
| ADV-188 | Grant externally Unknown but template claims Paid | Event/status invariant blocks misleading send |
| ADV-189 | Token value contains HTML/script or email header injection | Contextual escaping and safe renderer |
| ADV-190 | Client changes template while message is queued | Deterministic version binding and audited emergency handling |
| ADV-191 | Two sponsors use same event type with different branding | Correct tenant revision and isolation |
| ADV-192 | Recipient revokes authority before dispatch | Recheck eligibility, suppress private message |
| ADV-193 | Duplicate outbox replay | Stable delivery identity, no avoidable duplicate |
| ADV-194 | Template includes internal compliance note token | Disallowed token, publication blocked |
| ADV-195 | Numeric account reference begins with zero | Preserve all digits |
| ADV-196 | Client deletes all templates | Platform defaults remain; critical missing legal content fails closed |
| ADV-197 | Email secure link forwarded to unauthorized person | Reauthentication and subject authorization required |
| ADV-198 | Translation omits mandatory legal text | Locale validation and governed fallback/hold |
| ADV-199 | External email provider times out after acceptance | Unknown delivery state, no false delivered claim |
| ADV-200 | Inbound reply spoofed as grant recommender | Do not auto-post to shared conversation without sender validation |
| ADV-201 | Unverified address receives account balance notification | Disclosure gate blocks sensitive content |
| ADV-202 | Test send accidentally uses production donor data | Synthetic fixtures and test-only delivery restrictions |
| ADV-203 | Published revision edited in place | Reject; new reviewed revision required |
| ADV-204 | HTML-only email unreadable in accessibility client | Plain-text alternative and accessible markup enforced |

## Open decisions

1. Exact client hierarchy: sponsor, sponsor program, intermediary/advisor firm and branded channel. Recommendation: sponsor-scoped override initially; nested overrides only when governance requires them.
2. Token syntax, max depth, optional conditional blocks and pluralization. Recommendation: constrained declarative formatting, no general-purpose scripting.
3. Who can publish: sponsor administrator alone for ordinary branding, independent reviewer for financial/security/legal content.
4. Whether emails may contain balances/amounts versus directing recipients to secure portal; recommend minimal sensitive data by default.
5. Locale fallback and legally approved translations.
6. Inbound email replies, consent, unsubscribe classification and deliverability governance.
7. Whether a sponsor may completely disable particular event emails. Recommendation: optional notices configurable; security/mandatory notices protected.
8. Vendor selection, retention, encryption and cross-region delivery constraints.

**Status:** Proposed template and token governance, ADV-185..ADV-204. No implementation or tests executed.

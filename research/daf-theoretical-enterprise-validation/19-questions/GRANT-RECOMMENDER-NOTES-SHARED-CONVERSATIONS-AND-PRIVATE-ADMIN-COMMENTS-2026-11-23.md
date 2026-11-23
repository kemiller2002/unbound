# Grant recommender notes, shared conversations and private administrator comments

Date: 2026-11-23. Proposed requirements only. Component monolith, shared Notes/Conversations component. No implementation or executed tests.

## Product model

A grant recommendation has three distinct communication contexts, all accessible from the grant detail page but governed separately.

1. **Recommendation notes**: submitted by the authorized person recommending the grant, explaining purpose, instructions or context. These are part of the versioned recommendation submission record, not administrator notes. Preserve original submitted content. Later clarifications are appended, not silently substituted.
2. **Shared conversation**: an authenticated two-way conversation between authorized recommenders/advisors and sponsor administrators. Supports administrator questions, donor responses, attachments, timestamps, read indicators and follow-up tasks. It is visible only to the explicitly authorized audience, not publicly searchable and not automatically visible to the charity.
3. **Internal administrative notes**: staff-only collaboration for processing, research, compliance, payment and case handling. Restricted internal notes can be further limited to specific roles or investigations. Internal notes are not exposed through donor portals, embedded widgets, IVR, exports or general-purpose APIs.

Terminology: in the UI, 'Shared with recommender' is preferable to 'Public', since these messages are not public on the internet. Sponsor white-label wording may differ, but permissions must not.

## Authoritative entities and ownership

Shared Notes/Conversations component owns ConversationThread, ConversationMessage, MessageRevision, AudiencePolicy, AttachmentReference, Mention and FollowUpReference. Grant owns the submitted RecommendationNote snapshot and links to thread IDs. Identity/Authority owns participant delegation and revocation. Evidence/Audit owns immutable decision and communication traces. Work Queue owns tasks, not the text of notes.

Every message carries ThreadId, SubjectRef, AuthorPrincipalId, ControllingPrincipalId, SponsorId, AudienceScope, AllowedParticipantSet or role policy version, MessageType, CreatedAt, EditedAt, Revision, AttachmentRefs, ModerationState, RetentionClass and audit reference. The system must distinguish the donor, advisor, successor, sponsor administrator, charity and other participants, and recheck current authorization on every read and write.

**Visibility is fixed at thread creation** as SharedRecommender or InternalStaff or RestrictedInternal. Messages cannot be moved or flipped between visibility classes through a casual edit. To share internal information, an administrator must author a **new deliberate shared reply**, with audience preview and confirmation where sensitive content may be present. Do not copy private text automatically. A thread with restricted data cannot be made shared by changing a flag.

## Required workflows

- Recommender can enter a note during initial grant submission; validation and submission confirmation show exactly what was included.
- Recommender can add clarifications and respond to administrator questions after submission, with a preserved chronology.
- Administrator can ask a question in the shared thread, request documents, and mark an inquiry Resolved/WaitingForRecommender/WaitingForStaff. These workflow statuses do not equal grant approval or payment status.
- Administrator can create staff-only notes and restricted compliance notes on the same grant, with distinct tabs or strong visual separation.
- Administrator can choose to post a **shared reply** with explicit audience display; default new administrator note remains internal unless the user deliberately enters the shared conversation.
- Authorized advisors and successor representatives may participate only under sponsor policy and current delegation; revoked access immediately blocks new reads/replies, subject to retention and legal disclosure rules.
- Optional notifications by email/SMS/portal announce that a message exists, but avoid placing sensitive grant details in unverified notification channels.
- Attachments require malware scanning, access control, retention and independent document provenance. Attachments supplied by recommender are untrusted evidence until reviewed.
- A shared conversation can generate a follow-up case/task without automatically holding or approving the grant. A compliance concern can trigger a separately authorized referral/hold workflow.
- White-labeled sponsor UI, embedded widgets and APIs show the same scoped conversation with accessible mobile layouts and explicit timestamps.

## UI recommendations

Grant detail: Overview | Activity | Messages | Internal Notes | Documents. The recommender sees Messages and their original recommendation notes, never Internal Notes. Staff sees all permitted sections but shared Messages and Internal Notes have visually distinct headers, audience labels and separate composition fields. Provide a preview of the audience before sending a shared reply. On staff interfaces, use explicit 'Reply to recommender' versus 'Add internal note' actions, never one composer with a mutable public/private toggle.

An unread shared message and an unresolved staff follow-up are different indicators. The status of the grant remains separately sourced from authoritative grant/payment decisions.

## Security and compliance

- Default-deny visibility; authorization enforced on every API/query and attachment URL, not just UI tab hiding.
- Never leak internal note text in search, activity feeds, notification previews, exports, analytics, AI summaries, IVR, browser caches or cross-tenant projections.
- Sponsor A cannot see Sponsor B's grant conversations even if both concern the same charity.
- A recommender's message does not itself constitute a grant approval, change of payment destination or compliance evidence; typed domain commands govern all consequential actions.
- Internal compliance/legal notes can be more restrictive than ordinary staff notes; labeling does not itself establish attorney-client privilege.
- Message edits preserve revisions; deletion/redaction follows retention and legal hold rules. An already delivered notification cannot be retroactively recalled.
- Accessibility, multilingual support, communication consent and archival/export rules must be sponsor-governed.

## Adversarial tests (unexecuted)

| ID | Scenario | Required outcome |
|---|---|---|
| ADV-169 | Admin accidentally composes sensitive note while viewing shared thread | Clear audience indicator and deliberate shared-send action; no auto-post |
| ADV-170 | Internal note changed to Shared by API patch | Reject immutable audience scope transition |
| ADV-171 | Recommender attempts to fetch internal notes by guessed ID | Deny at query and attachment layers |
| ADV-172 | Admin copies private note into shared reply through automation | No automatic copying; explicit authored new message |
| ADV-173 | Advisor delegation revoked mid-conversation | Subsequent reads/writes denied per current authority |
| ADV-174 | Two sponsors share charity but not grant | Conversations remain sponsor scoped |
| ADV-175 | Search index or AI summary includes restricted staff note in donor result | Deny; projection and model context scoped |
| ADV-176 | Notification sends full sensitive grant details to unverified email | Minimal notification without private content |
| ADV-177 | Recommender edits original submission note after approval | Original preserved; append versioned clarification |
| ADV-178 | Shared reply says 'approved' | Does not change authoritative grant state |
| ADV-179 | Charity representative accesses donor-admin thread | Deny absent separately granted charity-specific conversation |
| ADV-180 | Admin uploads confidential attachment into shared thread | Audience confirmation and attachment-specific access controls |
| ADV-181 | Recommender uses IVR numeric grant ID to fetch private staff notes | Deny; ID is lookup key, not authority |
| ADV-182 | Staff member without restricted role searches compliance notes | No existence/content leakage beyond policy |
| ADV-183 | Concurrent staff and recommender replies arrive out of order | Durable message order and timestamps; no lost messages |
| ADV-184 | Thread archived with unresolved document request | Preserve audit and task state; do not falsely mark resolved |

## Open decisions

1. Can multiple authorized advisors/recommenders participate in one shared thread, or is it restricted to the initiating recommender? Recommendation: policy-driven participant sets with explicit sponsor and fund authority.
2. Can the charity be invited into a distinct conversation? Recommendation: separate charity-facing thread, never reuse donor/staff shared thread.
3. Should donor responses be permitted after grant completion or account closure? Recommendation: bounded retention and support windows, policy-driven.
4. Should all grant recommendations require a note? Recommendation: optional by default, mandatory only under sponsor-specific policy or certain grant types.
5. Should an administrator edit someone else's message? Recommendation: no silent edits; append correction or governed moderation with original preserved.
6. Should AI draft replies? Recommendation: optional staff-reviewed drafting only, no automatic sending of sensitive replies without separately approved policy.

**Status:** Proposed cross-entity conversations extension and ADV-169..ADV-184; no implementation or tests executed.

# IVR, telephone self-service and numeric identifiers

Date: 2026-11-23. Proposed requirements only, no implementation, telephony vendor selection, security certification or tests. Component monolith with external telephony adapter.

## Architecture and identifier decision

Unbound must support an IVR integration through the same authenticated application command/query facade used by web, widgets and other channels. The IVR provider is an external integration adapter, **not an internal microservice**. All financial and privacy controls remain owned by the component monolith.

Each telephone-accessible record has two separate identifiers:
1. **Internal immutable typed ID** (non-human database/domain identity, not intended for DTMF).
2. **Public numeric reference** (digits 0-9, stable, human-readable and enterable by DTMF). This is an alternate lookup key, never a password, authentication factor or authorization grant.

Support numeric references for accounts/funds, grants, charities, contributions, payments and administrative cases where telephone access is permitted. Sponsor-specific display labels are white-labeled, but numeric input behavior must be consistent. Numeric references are not required to expose all records over IVR.

## Numeric reference governance

- Preserve leading zeros by treating reference as a string of digits, never an integer. Input normalization may remove spaces or spoken grouping but must not silently strip meaningful digits.
- Use a configurable, documented reference length with sufficient namespace capacity, collision resistance and a check-digit/error-detection scheme where appropriate. **Do not finalize a length or checksum until expected volume, sponsor scope, legacy references and caller usability are modeled.**
- Numeric references are unique within a clearly specified scope: preferably globally unique platform-wide for unambiguous support and cross-sponsor routing, or sponsor-scoped with a mandatory numeric sponsor prefix/selection. Do not rely on a caller knowing which tenant a duplicate reference belongs to.
- Once issued, do not reassign a reference to another record. Retired references resolve to authorized historical status or an uninformative response; do not leak a new owner's data.
- Preserve external grant IDs as separately namespaced identifiers, since existing providers may use letters or non-numeric formats. Add an IVR reference rather than rewriting external IDs.
- Maintain aliases for imported legacy numeric identifiers with explicit scope, collision checks and provenance; aliases must not create ambiguous lookups.
- Check-digit failures prompt re-entry; do not enumerate valid records or reveal whether a private account exists to unauthenticated callers.
- Printed and digital communications may include the numeric reference subject to disclosure policy. It must never be treated as a secret.

## Telephone lookup requirements

Authorized callers may retrieve:
- Account/fund: status, permitted available-to-grant balance, recent contributions and grants, next actions, relevant notices.
- Grant: recommendation/approval/payment state, amount where authorized, charity name, expected next step, hold/return status with carefully limited explanation.
- Charity: public verification or eligibility status only where publication is approved; private compliance reasons are not automatically disclosed.
- Contribution/payment/case: appropriate status and support instructions; provider Unknown must not be spoken as Paid or Failed.

The system must distinguish donor, authorized advisor, successor, sponsor staff, charity representative and anonymous public caller. Their data scopes and permissible actions differ. A charity representative may receive only charity-authorized information and not donor fund balances or donor-private notes.

Initial IVR actions should be **read-only status and secure handoff**. New grant recommendations, destination changes, beneficiary changes, payment release, sensitive identity changes and financial approvals must remain disabled unless separately designed with strong authorization, fraud controls and independent sponsor approval.

## Authentication and privacy

- Phone number/ANI caller ID is a weak signal, not proof of identity. Numeric record ID is also not authentication.
- Prefer authenticated callback/session binding or strong step-up methods appropriate for caller risk and accessibility. Do not rely on a reusable account PIN alone for sensitive information without a threat model.
- DTMF capture must not expose secrets in call recordings, logs, analytics or vendor transcripts. Payment card or highly sensitive credentials should not be solicited through an unqualified IVR.
- Rate-limit and detect enumeration, repeated failed lookups, spoofed caller ID, account takeover and abusive call transfers; protect against differential disclosure through timing and prompts.
- Separate public status from private data. After failed authentication, give neutral responses that do not confirm existence of a donor account or grant.
- Call recordings/transcripts, voice biometrics and AI voice agents require explicit legal, consent, retention, accessibility and provider reviews before activation.
- The IVR provider receives only the minimum scoped data necessary; require contract, security, data residency and incident response review.

## Voice UX and accessibility

Support DTMF as the guaranteed fallback, even if speech recognition or an AI voice agent is offered. Read back entered digits in groups with confirmation, allow repeat/correct/back, tolerate pauses, provide clear timeout/retry limits, and offer transfer to an accessible human support route without dead ends. Never require a caller to visually inspect a phone screen to complete an essential lookup. Offer multilingual prompts per sponsor where configured, avoid long menus, and allow callers to enter a known reference immediately. TTS must use sponsor-approved terminology, including white-labeled alternatives to 'grant'.

A caller should hear precise state labels: Received, Under Review, Approved, Scheduled, Submitted, Confirmed Paid, On Hold, Returned or Unable to Confirm, as supported by authoritative state. Do not expose internal sanctions triggers or private investigative notes. Announce source freshness when status is not live.

## Components and API contracts

Introduce **Telephone/IVR Channel Adapter** within Integration Adapters and a **Numeric Reference Registry** logical capability owned by Identity/Reference Management or a dedicated shared component. Registry owns ReferenceId, SubjectRef, SponsorScope, NumericReference, IssuedAt, RetiredAt, AliasOf, Version, CheckDigitSchemeVersion and audit trail. The facade exposes ResolveNumericReference, AuthenticateCaller, AuthorizeSubjectAccess, GetPermittedAccountSummary, GetPermittedGrantStatus, GetPermittedCharityStatus and CreateSupportHandoff. These are typed internal contracts, not public unrestricted database lookup APIs.

No IVR endpoint can query the ledger, grant or notes tables directly. Authorization is checked after reference resolution and before disclosure. Phone-specific presentation transforms authorized results into safe speech/DTMF prompts. Status reads may use projections only with explicit freshness and must not imply payment finality unsupported by the authoritative system.

## Adversarial scenarios (not executed)

| ID | Fault | Required result |
|---|---|---|
| ADV-153 | Caller guesses valid account number | No private disclosure without authorization |
| ADV-154 | Two sponsors import same numeric legacy grant ID | Explicit alias namespace; no wrong-tenant resolution |
| ADV-155 | Leading zeros lost by telephony vendor | String-preserving contract or reject ambiguous lookup |
| ADV-156 | Caller ID spoofed as donor phone | Caller ID not accepted as authentication |
| ADV-157 | Rapid sequential number enumeration | Rate limit and neutral responses |
| ADV-158 | Grant payment externally Unknown | IVR does not announce Paid or Failed |
| ADV-159 | Charity representative requests donor account balance | Deny and protect donor privacy |
| ADV-160 | Retired numeric ID is reassigned | Reject reassignment |
| ADV-161 | AI voice agent follows malicious note content | Notes treated as untrusted; no privilege escalation |
| ADV-162 | Call recording captures authentication secrets | Sensitive capture suppressed and retention controlled |
| ADV-163 | Caller with visual impairment cannot use screen prompt | DTMF/voice-only accessible completion |
| ADV-164 | Sponsor labels grants as distributions | Sponsor-specific wording applied without changing semantics |
| ADV-165 | Caller attempts destination change through read-only IVR | Deny; require separately authorized workflow |
| ADV-166 | Check digit mistyped | Safe correction without existence disclosure |
| ADV-167 | Stale IVR cache contradicts current protective hold | Authoritative status wins; do not promise payment |
| ADV-168 | Legacy external grant ID contains letters | Preserve external ID and issue distinct numeric IVR reference |

## Open decisions

1. Forecast maximum reference issuance across sponsors, funds and grants, then choose numeric length, optional grouping and checksum. Balance usability with collision capacity.
2. Decide platform-global versus sponsor-scoped references; recommendation: globally unique canonical telephone reference, sponsor-specific display grouping optional.
3. Select authentication and step-up strategy for donors, advisors, charities and staff with privacy/accessibility testing.
4. Define per-role voice disclosure matrix and whether account balances are ever read aloud by default.
5. Determine telephony vendor API capabilities, caller handoff, outage behavior, call-recording consent, multilingual TTS and pricing.
6. Decide whether future IVR should support transactions; default read-only until risk and governance analysis.
7. Decide how to map historical sponsor account/grant numbers and avoid ambiguity.

**Status:** Proposed requirements and ADV-153..ADV-168. No implementation, telephony selection or tests performed.

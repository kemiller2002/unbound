# Unified Client Brand Kit and omnichannel white labeling

Date: 2026-11-23. Proposed theoretical requirements, not implemented or tested. Component monolith; no independent branding microservice.

## Core decision

Create one authoritative, versioned Client Brand Kit per sponsor/approved brand scope. Email, donor portal, sponsor administration, embedded widgets, print/PDF correspondence, IVR, SMS and other channels consume a shared brand configuration but use independent, channel-specific renderers and accessibility controls. Do not build separate unrelated brand settings per channel.

## Brand configuration layers

1. **Identity:** sponsor legal name, approved public display name, brand names, logos, monochrome/light/dark marks, alt text, icon, sender/display identity, verified domains and support contact details.
2. **Design tokens:** semantic colors (primary, surface, text, success, warning, error), typography and fallback stacks, spacing, radius, border, elevation and iconography. Tokens describe intent, not a universal CSS implementation.
3. **Content and terminology:** client-specific business labels, e.g. Grant -> Distribution, Grant recommendation -> Giving request, Fund -> Giving account; pluralization, language variants, contextual grammar and approved definitions. Canonical domain IDs, financial semantics and legal disclosures remain unchanged.
4. **Channel policies:** permitted customization per email/web/widget/print/IVR/SMS, logo variant, footer, legal notices, supported languages, contrast rules, size constraints and fallback.
5. **Asset registry:** immutable versioned assets, provenance, license, sponsor scope, media type, accessible alternative, approved CDN/hosting and retirement policy.
6. **Publication governance:** Draft, Validating, Approved, Published, Superseded, Retired; effective intervals, owner, approver, diffs, audit and rollback-as-new-revision.

## Channel contracts

| Channel | Shared inputs | Channel-specific behavior |
|---|---|---|
| Donor portal | Colors, typography, logo, terminology, support links | Responsive WASM/Forma UI, WCAG validation, accessible contrast, no cross-tenant CSS bleed |
| Sponsor administration | Sponsor identity and optional brand accent | Keep critical safety/status semantics consistent; never obscure holds or alerts |
| Embedded widgets | Safe scoped tokens, terminology, approved assets | Host-page isolation, CSP, responsive embedding, fallback and no privilege from visual branding |
| Email | Theme, header/footer, logo, terminology, sender | Email-safe HTML, plain-text alternative, dark-mode and client compatibility |
| Print/PDF/letters | Identity, approved fonts, legal footer, address | Print margins, monochrome support, page breaks, archival PDF, immutable as-issued snapshot |
| IVR | Spoken sponsor name, localized terms, support contact | Pronunciation lexicon, concise TTS prompts, DTMF fallback; no CSS assumptions |
| SMS | Sender/brand short name, terminology, support links | Plain text, length and opt-in rules, minimal sensitive data |
| APIs | Brand configuration and canonical semantic keys | Typed authorized configuration; never change canonical domain states |

## Hierarchy and inheritance

Platform default -> sponsor-approved brand -> optionally sponsor program/campaign/channel variant. A child scope can override only fields permitted by parent governance and must not impersonate another sponsor. Resolve deterministic scope + locale + channel + effective version. Avoid unbounded nesting or arbitrary override precedence. Explicitly separate **sponsor legal identity** from **marketing display name**; transactional and regulatory documents may need both.

## Branding versus business rules

A client may relabel 'grant' but cannot redefine prohibited-benefit restrictions, grant approval, sanctions, fund accounting, legal entity ownership or payment finality. Canonical semantic keys remain stable for APIs, audit and internal data; only localized presentation changes. Mandatory legal notices and accessibility requirements cannot be disabled by theme. Client branding is not a basis for access control; all authorization remains sponsor/principal scoped.

## Design studio requirements

Client Settings > Brand Kit:
- Identity and assets, color palette, typography, terminology dictionary, contact details, channel previews, governance/history.
- Live previews for donor portal, embedded widget, email, printed letter, SMS and sample IVR script/audio where supported.
- Upload/replace logo with crop/size checks, alt text and dark/light variants; validate contrast and minimum dimensions.
- Publish with impact report listing affected templates/channels and immutable revision bindings; stage preview before go-live.
- Side-by-side revision comparison, synthetic data fixtures, scheduled publish, rollback as a new version and client permissions.
- Offer starter themes and safe defaults so sponsors can launch with minimal setup; avoid mandatory manual customization.

## Version binding and history

BrandKitRevisionId and resolved token/asset versions must be recorded with email deliveries, printed artifacts and other immutable communications. Dynamic portal/widget rendering may use current published theme but must not silently rewrite historic financial statements, screenshots or delivered messages. Queued communications follow a deterministic version-binding policy. Preserve historical assets and evidence under approved retention/legal hold rules.

## Architecture

Brand Governance component owns BrandKit, BrandRevision, TokenDefinition, TerminologyDictionary, AssetReference, ChannelPolicy and Publication. Communication, Portal, Widgets, Folio/Print and IVR adapters request scoped resolved brand snapshots through typed in-process contracts; they do not write sponsor branding tables. Existing client email template overrides remain distinct from global brand themes. Outbox invalidates caches safely after publication; stale caches cannot weaken authorization or mandatory disclosures.

## Adversarial scenarios (not executed)

| ID | Fault | Required outcome |
|---|---|---|
| ADV-233 | Sponsor A logo appears in sponsor B portal | Strict tenant scope and cache isolation |
| ADV-234 | Client relabels grant as purchase | Canonical legal/benefit rules remain and misleading label blocked by governance |
| ADV-235 | Theme makes required hold warning invisible | Semantic status/contrast gate |
| ADV-236 | Client changes brand after email sent | As-sent email remains reconstructable |
| ADV-237 | Brand change while queued print batch | Deterministic version pinning |
| ADV-238 | Embedded widget inherits hostile host CSS | Style isolation and safe fallback |
| ADV-239 | Brand asset URL includes donor personal data | Reject external data leakage |
| ADV-240 | Sponsor uploads logo without alt text | Publication validation blocks inaccessible output |
| ADV-241 | Dark mode logo disappears | Variant/fallback validation |
| ADV-242 | IVR mispronounces sponsor brand name | Pronunciation override and spoken preview |
| ADV-243 | Sponsor claims another sponsor's verified sending domain | Domain ownership check |
| ADV-244 | Program override conflicts with mandatory sponsor identity | Governance rejects identity misrepresentation |
| ADV-245 | Translated label contradicts canonical financial status | Localization validation and protected semantics |
| ADV-246 | Brand publication races with active widget sessions | Versioned safe transition; no broken critical controls |
| ADV-247 | Old brand revision deleted but needed for historical letter | Retention and immutable asset reference preserve evidence |
| ADV-248 | Client adds inaccessible custom font | Fallback and readability checks |
| ADV-249 | Theme overrides legal disclosure formatting to hide it | Nonwaivable visibility rules |
| ADV-250 | SMS exceeds length due to long sponsor name | Channel-safe formatting and fallback |
| ADV-251 | Admin console theme obscures fraud alerts | Protected safety indicators |
| ADV-252 | Default brand missing for newly onboarded sponsor | Platform-safe starter kit applied |

## Open decisions

1. Decide whether white-label brands can differ by sponsor program, advisor firm or campaign; start with sponsor and explicitly governed variants.
2. Define token naming and theme migration/version compatibility with Forma, Folio, email renderer and IVR.
3. Decide asset hosting, signed upload, content scanning, copyright/license validation and archival policy.
4. Establish accessible contrast and print standards with independent usability testing.
5. Determine client domain verification and email authentication flows.
6. Govern semantic terminology and translations so labels never imply legally inaccurate rights or payment states.
7. Define which admin surfaces may be heavily white-labeled versus retaining standardized operational safety UI.

**Status:** Proposed unified brand system and ADV-233..ADV-252. No implementation or tests executed.

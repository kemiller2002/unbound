# Contextual AI Assistance UX, reusable Forma patterns and notification governance
Date: 2026-11-23. Proposed requirements only. No UI implementation or usability testing.

## Product decision
Use a consistent contextual assistance system throughout Unbound, not a universal chat overlay. The user is already engaged with a field, configuration, case, queue, dashboard, email or report; help should inherit only authorized, current context and offer progressive disclosure. Every visible suggestion distinguishes explanation, observation, inference, proposed change and completed authoritative action.

## Interaction vocabulary and information architecture
**Inline help:** beside each input and action: What is this?, Why disabled?, Valid values, What changes?, Fix issue, Ask assistant. Short accessible hint first; detailed rationale and contract references on demand.
**Assistance panel:** docked side panel on desktop; non-obscuring sheet or full-screen accessible workspace on narrow mobile. Header shows current sponsor, object, role, policy version and whether data are live, stale or synthetic. Tabs/sections: Answer, Evidence, Options, Proposal, History (only when applicable). Avoid unnecessary persistent chat transcripts.
**Guided configuration:** user intent -> scoped discovery -> essential clarifying question -> typed draft -> validation -> simulation -> side-by-side diff -> explicit confirmation -> independent approval/publish -> verified result. Show downstream effects and irreversible consequences.
**Issue/Health Center:** grouped actionable now, due soon, monitoring and history; sort by actual risk, deadline, confidence/evidence completeness and owner. Mandatory financial/compliance holds are always visible. Route work to existing queue; never create duplicate ticketing.
**Proactive nudges:** quiet inline indicators for nonurgent suggestions, digest for low risk, targeted interruption only for time-critical or material risk. Each notice says what happened, why it matters, who owns it, when action is needed and what next safe action exists.
**Evidence inspector:** distinguish source documents, typed contracts, policy versions, effective/known-at timestamps, model inferences and missing evidence. Source links recheck authorization on open.
**Proposal review:** structured before/after diff, affected records, policy checks, simulation, confidence and uncertainty, rollback/compensation, expiry, required decision authority and explicit statuses Draft/Validated/AwaitingApproval/Applied/Verified/Failed/Unknown. No conversational 'yes' as a substitute for privileged confirmation.

## Reusable Forma capability proposal
Reuse public Forma patterns (field help, work queue, alert, operation status, freshness, skeleton, dialogs, drawer, diff viewer, navigation and responsive layout) wherever they exist and validate pinned versions. Add only generalizable missing patterns:
- contextual help trigger and associated accessible panel;
- assistant answer with source/evidence annotations;
- uncertainty and freshness presentation;
- structured proposal/diff review;
- simulation result and risk/impact summary;
- next-safe-action controls with permission/approval semantics;
- notification grouping, digest and escalation presentation;
- model unavailable/degraded states and safe fallback.
Forma owns visual/accessibility presentation only; Unbound/Limen owns context, state, policy and execution. Forma Studio must be able to compose/preview these patterns through public contracts, with no private CSS.

## Detailed UX requirements
1. **Discoverability without clutter:** help on focus, error and relevant action; optional Ask AI; do not place chat bubbles over primary controls.
2. **Progressive disclosure:** one useful answer first, expandable reasoning/evidence; experts can open policy/version details.
3. **Truthful status:** never conflate Proposed, Approved, Queued, Paid, Delivered or Unknown. Completion feedback requires authoritative receipt.
4. **User control:** user can dismiss noncritical suggestions, stop generation, cancel drafts, inspect changes and escalate to human. Critical holds cannot be hidden by preference.
5. **Safe action friction:** minimize unnecessary confirmations for reversible nonmaterial edits; require deliberate review for money, policy, legal, disclosure, account access and irreversible changes. Approval and execution remain separate.
6. **Privacy:** show only caller-authorized context; redact sensitive evidence; no automatic copy of PII into prompts, logs, clipboard or exports.
7. **Accessibility:** keyboard, screen reader, 200–400% zoom, touch, focus management, reduced motion, forced colors, meaningful text not color alone, readable live announcements and localization/RTL. Do not obscure mandatory notices on mobile.
8. **Reliability:** nonblank loading, offline/model unavailable, stale, partial failure, permission denied, expired proposal and conflict states. Preserve form edits and provide deterministic help fallback.
9. **History and audit:** separate chat transcript from durable proposal/decision/action history; retain necessary provenance, not indiscriminate raw conversations.
10. **Evaluation:** task completion, errors prevented, comprehension, time-to-safe-action, notification precision, alert fatigue, accessibility success, human escalation quality and real-world outcomes, not clicks or assistant engagement.

## Notification policy
Critical: financial/compliance hold, unauthorized access, unreconciled material movement, urgent sanctions concern. Show persistent, role-scoped and route to owner; never silently suppress.
High: approaching deadline with material exposure, unresolved provider Unknown, expired delegation blocking work. Timely targeted notification and escalation.
Medium: configuration contradiction, workload imbalance, stale template, missing backup. Batch where safe with actionable digest.
Low: optimization suggestion or informational trend. Quiet recommendation; user preference controls.
Severity is determined by governed rules/evidence, not arbitrary model confidence. Deduplicate by issue fingerprint, preserve material escalation, respect local calendars and work shifts without delaying urgent controls. Dismissal/snooze requires reason and expiry for material items.

## Interaction examples
**Routing setup:** administrator selects Help configure; assistant explains StickyAccountOwner vs RoundRobin and distinct decision authority; proposes sponsor-scoped routing diff; simulator shows reassignment and backup; authorized supervisor approves.
**Charity review:** reviewer sees evidence freshness and a 180-day expiry; assistant identifies missing documents but cannot clear an adverse hold; case routes to authorized reviewer.
**Email template:** editor asks for a clearer confirmation email; assistant drafts typed token and block changes, shows mandatory disclosures and synthetic preview; publish requires ordinary review.
**Finance report:** controller asks why AUM differs from prior report; assistant displays live vs closed snapshot and valuation timestamps, not a fabricated reconciliation; immutable as-issued statement remains unchanged.

## Dependencies and acceptance
Use REQ-X-048/049 work queues, REQ-X-050 dashboard framework, REQ-X-051 documentation, REQ-X-052 typed email templates, REQ-X-053 assistance architecture and REQ-X-054 reactive/proactive checks. Validate design in Forma/Studio before implementation. Build realistic role-based prototypes and synthetic tests; no tests claimed executed.
- ADV-904: help control missing accessible name; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-905: assistant panel obscures financial hold; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-906: mobile keyboard covers confirmation; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-907: focus lost when assistant drawer opens; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-908: screen reader cannot read evidence source; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-909: assistant implies action completed when draft only; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-910: AI-generated diff omits deleted restriction; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-911: user cannot distinguish model inference from fact; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-912: permission change while review panel open; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-913: assistant action enabled despite expired proposal; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-914: confirmation lacks affected sponsor and accounts; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-915: approval action available to proposal author; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-916: sensitive notes leaked in explanation; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-917: citation opens unauthorized source; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-918: stale source shown without timestamp; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-919: Unknown provider status styled as failure; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-920: success toast shown before authoritative acknowledgement; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-921: multiple alerts for same root cause; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-922: critical alert hidden by notification preference; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-923: dismissal treated as remediation; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-924: snooze outlives legal deadline; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-925: low-severity alerts overwhelm work queue; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-926: assistant proactively interrupts active financial close; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-927: alert severity uses model confidence alone; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-928: critical risk shown only as color; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-929: keyboard-only user cannot inspect diff; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-930: touch target too small for mobile; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-931: RTL layout reverses financial meaning; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-932: AI help available only with model online; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-933: unsupported setting suggested as available; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-934: model text overrides protected disclosure; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-935: inline help changes input unexpectedly; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-936: draft conflicts with concurrent config edit; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-937: proposal replays after policy update; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-938: evidence links lack version; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-939: assistant references inaccessible private tenant data; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-940: synthetic preview not labeled; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-941: AI asks for unnecessary PII; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-942: assistant copies private data into export; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-943: user cannot recover original form after preview; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-944: error panel traps focus; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-945: auto-expanded help creates excessive scroll; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-946: empty state implies no compliance issues; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-947: stale data badge omitted on compact mobile; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-948: notification digest delays urgent hold; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-949: false positive cannot be challenged; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-950: human escalation loses issue context; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-951: AI recommendation not auditable; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-952: assistant offers unsafe quick fix; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-953: report preview labeled issued; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-954: widget action bypasses domain command; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-955: approval button placed near destructive control; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-956: assistive voice reads private data aloud; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-957: role switch retains previous sponsor context; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-958: localization misstates grant status; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-959: model output too verbose for urgent decision; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-960: high contrast mode hides provenance icon; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-961: reduced motion ignored in drawer; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-962: deep link to issue bypasses authorization; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
- ADV-963: assistant summary hides unresolved uncertainty; expected: accessible, accurate, permission-scoped, nonintrusive interaction.
Status: proposed. ADV-904..ADV-963 unexecuted.

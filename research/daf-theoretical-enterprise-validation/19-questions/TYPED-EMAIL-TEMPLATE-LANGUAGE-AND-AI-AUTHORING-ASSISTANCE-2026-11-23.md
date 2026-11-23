# Typed Email Template Language (ETL) and AI-assisted authoring
Date: 2026-11-23. Proposed design only; no compiler, parser or tests implemented.

## Decision
Use a small **typed declarative template AST** as canonical persisted representation, with optional friendly text syntax compiled into it and visual authoring in Forma Studio. Do not create a Turing-complete scripting language. Do not adopt Liquid/Handlebars blindly: compare syntax familiarity, tooling and safe semantic restrictions in an ADR. The structured document/block schema from Forma Email Designer remains canonical for visual composition; ETL is the *content expression layer*, not a competing email layout schema. Distinguish content tokens from layout blocks, delivery policy and protected disclosures.

## Language capabilities
1. Typed placeholders: donor.displayName, fund.displayName, grant.amount, grant.status, charity.displayName, sponsor.contactEmail. Names illustrative; actual tokens come from event-scoped registry.
2. Contextual formatters: currency, date, percent, number, localized list and safe link. Formatters require declared types and locale, never arbitrary code.
3. Safe conditionals over explicitly declared boolean/enum states, e.g. grant.status == Approved. Unknown is a first-class state and cannot default to Paid.
4. Bounded iteration over approved collections with explicit maximum cardinality, e.g. statement.items, no arbitrary joins, sorting scripts or database calls.
5. Reusable approved fragments, localization, sponsor terminology and protected compliance/legal fragments.
6. Deterministic HTML/subject/plain-text encoding by context, no raw unescaped HTML or arbitrary URL concatenation.
7. Required token policies: fail validation for missing/unauthorized, or use explicitly approved optional fallback. No silent empty financial values.
8. Versioned AST, schema migration, static type checker, source spans, diagnostic codes, synthetic preview fixtures, renderer compatibility and semantic diff.

## Illustrative authoring syntax, not finalized
```text
Hello {{ donor.displayName }},

Your {{ terminology.grant }} recommendation for
{{ grant.amount | currency("USD") }} to {{ charity.displayName }}
has status {{ grant.status | label }}.

{% if grant.status == "NeedsInformation" %}
Please review the secure request: {{ links.secureAction }}
{% endif %}

{% include "sponsor.requiredDisclosure" %}
```
The example is intentionally non-executable. Token identifiers, enum values, syntax and includes require an ADR and event-scoped schema. Mandatory legal fragments should be injected/enforced by publication policy rather than relying on authors to include them.

## Static contracts and AI help
Every token, formatter, condition, collection, reusable fragment and visual block exposes AssistantCapabilityContract metadata: purpose, type, sensitivity, allowed events, dependencies, examples, formatting rules, missing/Unknown behavior, constraints, accessibility and permission. The editor exposes a searchable token explorer and contextual AI help: "What can I insert here?", "Why does this fail validation?", "Rewrite this for clarity without changing legal meaning", "Preview for grant pending and Unknown." AI outputs a structured proposed diff and diagnostics, not a directly published template. It must preserve protected clauses and sponsor rules.

## Authoring pipeline
Event schema + token registry + sponsor brand + terminology + protected fragments -> editor or constrained text syntax -> typed AST -> static validation -> authorized synthetic preview -> review/approval -> immutable published revision -> at event time resolve authorized token snapshot -> deterministic email HTML + plain text + subject -> archive as-sent evidence and delivery outcome. Never let designer query donor DB. Rendering requires bounded resources and immutable inputs.

## Formal boundaries
Forma: public email block schema, presentation primitives, token insertion/help UI, AST/renderer adapter contracts where reusable.
Forma Studio: visual and optional syntax editor, diagnostics, accessible authoring, AI suggested diff preview and approval UX.
Unbound: event/token semantics, sponsor-scoped values, policy/authorization, mandatory disclosures, revision publication, delivery and retention.
Folio: only after separate email-client compatibility ADR, not default email renderer.
F# domain: typed AST/compiler/interpreter and validation as reusable package subject to ownership ADR; no general scripting execution.

## Security and correctness
No arbitrary function invocation, loops without bounds, runtime network/file access, user-supplied SQL, reflection, JS, raw HTML or untrusted partials. Escape according to output context; verify secure links, prevent header injection, URL exfiltration and HTML injection. Enforce max nesting, output length and iteration. Keep sponsor data isolated, synthetic previews and historical version pinning. Accessible HTML and plain-text equivalent; localized date/time/currency; sponsor language cannot change canonical financial meaning. AI suggestions must be validated identically to human edits.

## Decisions to challenge
- Syntax vs AST: syntax is optional; visual designers should not need to learn a language. Prefer AST as source of truth for fidelity.
- Conditional complexity: allow simple typed predicates, but prevent a second policy engine in templates. Eligibility and approval decisions happen upstream.
- Token flexibility: arbitrary field access is tempting but risks leakage. Require event-scoped allowlists.
- Legal content: author-editable include syntax is insufficient; publication pipeline must force protected fragments.
- Email client constraints: rich layout needs client-tested renderer, not browser HTML assumptions.
- Multi-language: translation is reviewed and versioned, not autonomous production rewrite.

## Acceptance
- Parse/type-check/serialize roundtrip stable with canonical visual document.
- Unknown, missing, wrong-type, cross-sponsor and prohibited tokens fail safely.
- Conditional branches and bounded loops tested across every permitted enum state.
- AI can discover and explain every token/block/formatter, propose validated changes and show exact diff without publishing.
- Legal disclosures survive theme edits, conditional branches and AI rewrites.
- Historic emails can be reconstructed with pinned template, token, brand, policy and renderer versions.
- Rendered HTML, plain text and subject are deterministic for pinned inputs and context-safe.

## Adversarial scenarios (proposed, not executed)
- ADV-753: token accesses another sponsor; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-754: template includes raw script; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-755: conditional hides mandatory disclaimer; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-756: Unknown status rendered Paid; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-757: missing amount silently empty; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-758: currency formatter applied to text; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-759: date rendered wrong timezone; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-760: loop expands million rows; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-761: recursive include cycles; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-762: AI changes legal meaning; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-763: AI publishes without reviewer; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-764: HTML context escaped but URL context not; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-765: header injection via donor name; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-766: unapproved token discovered through autocomplete; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-767: private token appears in preview; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-768: theme revision changes as-sent reconstruction; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-769: visual editor roundtrip loses conditional; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-770: syntax editor produces AST drift; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-771: localization fallback removes required notice; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-772: secure link points to attacker domain; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-773: nested conditional bypasses type checker; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-774: template evaluation queries database; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-775: plain-text message omits CTA; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-776: reusable fragment changed after approval; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-777: event type mismatch uses wrong token registry; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-778: AI hallucinated token accepted; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-779: sponsor term replacement changes legal classification; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-780: preview shows synthetic but is labeled live; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-781: renderer truncates long compliance text; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.
- ADV-782: unbounded recursion consumes renderer resources; expected: typed, bounded, tenant-scoped deterministic rendering and approved publishing.

Status: proposed. ADV-753..ADV-782 unexecuted.

# Unbound integration requirements for Forma Email Designer
Date: 2026-11-23. Requirements only, no implementation/tests.

Authoritative reusable requirements:
- https://github.com/kemiller2002/forma/blob/main/docs/requirements/email-designer-2026-11-23.md
- https://github.com/kemiller2002/forma-studio/blob/main/docs/requirements/email-designer-2026-11-23.md

## Integration responsibilities
Unbound owns sponsor-scoped EmailTemplateDefinition, TemplateRevision, BrandKitRevision, EventType, TokenRegistry, recipient eligibility, mandatory legal/disclosure fragments, review/publish, delivery intent, provider attempts, audit and as-sent history. Forma owns portable structured document and designer UI; Studio owns authoring workspace. A dedicated validated email renderer produces HTML/plain text. Folio reuse requires ADR evidence.

## Acceptance
1. Every email event has a platform default and optional sponsor override; approved locale fallback.
2. Unbound provides typed, event-scoped tokens; synthetic preview; no arbitrary donor data queries from Forma.
3. Sponsor brand/terminology drives designer; cross-tenant data/assets never cross boundaries.
4. Client editors can change safe styling and content; protected legal fragments and truthful financial statuses cannot be removed or misrepresented.
5. Client template revisions pin document, layout, theme, token schema, mandatory clauses and renderer versions.
6. History reconstructs exactly what was sent, including visual styling and delivery state.
7. Host checks author/reviewer/publisher permissions; Forma never publishes on its own.
8. Responsive/mobile/plain-text and accessibility previews pass; unknown status and payment Unknown are never presented as Paid.
9. Test-send uses synthetic data and authorized internal destinations.
10. External provider uncertainty, retries and recipient revocation are handled by Unbound's Communications component.

## Boundaries and open questions
Do not duplicate the renderer or document schema inside Unbound. Determine sender-domain validation, supported locales, compliance fragment policy, whether client custom HTML is permitted, and email-client support matrix before implementation. Existing REQ-X-027..030 remain authoritative for Unbound-specific template governance.

Status: proposed. No code, tests or production rollout.

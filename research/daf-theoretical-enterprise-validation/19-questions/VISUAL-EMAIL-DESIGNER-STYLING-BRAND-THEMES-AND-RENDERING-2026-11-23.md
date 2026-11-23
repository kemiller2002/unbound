# Visual email designer, client styling and brand themes

Date: 2026-11-23. Proposed requirements, not implemented or tested. Extends REQ-X-027 and REQ-X-028.

## Design goal

Clients can visually style transactional and operational emails while inheriting safe platform defaults. Styling is not merely editable HTML: it is a governed combination of reusable client brand tokens, approved layout templates, editable content blocks, tokenized data and immutable published revisions.

## Three-layer design

1. **Client brand theme**: logo/alt text, optional dark/light logo variants, approved colors, type stacks, spacing scale, border radius, header/footer styles, buttons, link appearance, social/contact details, sender identity and accessibility defaults. Theme versioned and independently publishable.
2. **Email layout**: reusable responsive structure (single-column default, constrained multi-column where safe), branded header, body region, footer and mandatory compliance fragments. Layout versioned; client can select from permitted layouts or create approved custom layouts.
3. **Template content**: subject, preheader, text, headings, images, calls to action, dividers, lists, cards, tables where necessary and typed dynamic tokens. Template revision binds to specific layout/theme versions at send time.

A client-wide brand change may optionally update future emails using that theme, but each sent message must preserve the exact resolved style and rendered content used at delivery. Historic emails never change retroactively.

## Editor requirements

- WYSIWYG drag-and-drop editor with block palette: text, heading, image, button, spacer, divider, columns, informational card, contact block, legal disclosure, dynamic token and secure portal link.
- Direct manipulation of alignment, spacing, background, text color, font stack, emphasis, button style and border within constrained responsive design tokens.
- Brand kit editor for logo upload, alt text, approved palette, font fallback and global header/footer.
- Starter templates and client-specific reusable sections, including mandatory nonremovable compliance blocks.
- Token picker with validation and synthetic sample data. Optional conditional visibility only through constrained, versioned declarative rules, never arbitrary scripts.
- Undo/redo, autosaved drafts, revision comparison, reusable layouts, duplicate, preview, test send, approvals, publish, schedule and rollback-as-new-revision.
- Desktop, mobile, dark-mode, plain-text and common email-client preview. Visual editor output is accessible HTML with a readable plain-text alternative.
- Allow advanced HTML import/editing only through a separately authorized and sanitized mode, with the same safety, token and approval checks. Do not promise pixel-perfect rendering across clients.

## Rendering and delivery constraints

Email HTML has inconsistent CSS support. Use a constrained rendering pipeline producing compatible table-based or equivalent robust email markup, safe inline styles, image dimensions, fallback fonts, mobile stacking and high-contrast defaults. Strip scripts, forms, executable markup, untrusted remote styles and unsupported features. Avoid tracking pixels unless explicitly justified by privacy and consent policy. Host images on an approved HTTPS asset service with immutable asset versions and stable retention. Block unauthorized external images and leakage via URL query parameters.

Never place sensitive grant/account data in externally hosted image URLs. Links must be verified, sponsor-domain-aware and phishing-resistant. Accessibility checks include meaningful alt text, logical reading order, sufficient contrast, descriptive links, text scaling and a plain-text body. Required legal disclosures remain legible and cannot be hidden through CSS, low contrast or mobile-only suppression.

## White-label branding and hierarchy

Support platform default theme and client override; optional program/brand variants only if explicitly authorized and unambiguous. Sponsor terminology (including alternative labels for grant), approved sender domains, reply-to and contact information must be coordinated with the same client configuration. Validate SPF/DKIM/DMARC and domain ownership before enabling custom sender domains; no assumption that changing the visible From label proves email authentication.

Theme and layout permissions separate Designer, Reviewer, Publisher and Compliance Approver. Publishing a theme cannot silently alter already queued communications outside a documented binding policy.

## History and evidence

Each delivery captures TemplateRevisionId, LayoutRevisionId, BrandThemeRevisionId, asset references, token schema, mandatory disclosure version, exact rendered HTML/plain text/subject or securely reconstructable snapshot, preview validation results and delivery state. Preserve as-sent rendering even if a client later replaces its logo or changes colors. Historical preview must use immutable referenced assets and original event/token data, not current client settings.

## UI structure

Client Settings > Communications > Brand Kit / Layouts / Email Templates / History / Delivery History. Brand Kit sets global visual defaults; Layouts manage reusable structures; Email Templates customize each event. Editor has left block palette, center canvas, right properties and top preview/test/validate/publish actions. Provide a simple mode for nontechnical admins and advanced mode for authorized designers.

## Adversarial specifications (unexecuted)

| ID | Scenario | Expected outcome |
|---|---|---|
| ADV-217 | Client makes mandatory disclaimer white text on white background | Contrast/visibility validation blocks publication |
| ADV-218 | Client adds malicious script to custom HTML | Sanitize/reject |
| ADV-219 | Logo replaced after email sent | Historical render uses immutable original asset |
| ADV-220 | Theme updated while email queued | Deterministic bound theme revision or audited rerender |
| ADV-221 | Layout hides content on mobile | Responsive preview/validation blocks critical omissions |
| ADV-222 | Image URL leaks donor identifier to third party | Reject sensitive URL/data exfiltration |
| ADV-223 | Font unavailable in recipient client | Safe fallback and readable layout |
| ADV-224 | Client selects low-contrast brand palette | Accessibility gate or approved safe substitution |
| ADV-225 | Dark mode inverts button into unreadable state | Cross-mode checks and fallback |
| ADV-226 | Client uses another sponsor's asset | Tenant asset authorization denies |
| ADV-227 | HTML editor strips required token or clause | Validation prevents publication |
| ADV-228 | Email body has only images and no meaningful text | Plain-text and accessibility checks prevent unusable email |
| ADV-229 | Client configures spoofed sender domain | Domain verification required |
| ADV-230 | Email-client strips CSS | Content remains understandable and actionable |
| ADV-231 | Reusable footer changed after publication | Published revision binds fixed footer version |
| ADV-232 | Designer uses unsupported external font | Approved fallback and warning |

## Open decisions

1. Select constrained block schema and email rendering engine after cross-client tests, not by web-app CSS assumptions.
2. Decide whether advanced custom HTML is allowed for every client or restricted to trained/publisher roles.
3. Define accessibility standards and validation thresholds with sponsor and counsel review.
4. Decide branding inheritance: automatic for future unsent messages versus pinned revision per published template.
5. Define approved image hosting, CDN, retention and third-party tracking policy.
6. Determine supported languages, RTL layouts, text expansion and locale-specific compliance fragments.
7. Define whether reusable section edits automatically publish new dependent template revisions. Recommendation: no silent changes; explicit version binding.

**Status:** Proposed visual design and rendering requirements, ADV-217..ADV-232. No implementation or tests executed.

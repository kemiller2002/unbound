# 16 — UI requirements and concrete designs

Date: 2026-11-23 | Owner instruction: *"Come up with UI requirements, make designs more concrete. Make sure Forma and Folio can handle everything."* | Machine-readable: [`data/ui-requirements.jsonl`](data/ui-requirements.jsonl), [`data/ui-component-gaps.csv`](data/ui-component-gaps.csv) | Design prototypes: [`requirements/ui/designs/`](../../ui/designs/)

## 1. What this adds

Report 10 diagnosed the UI requirements. This report replaces diagnosis with specification:

- **18 global rules** (`UNB-UI-G-*`): status vocabulary, five-dimension grant progress, freshness, authoritative amounts, confirmation, unknown outcomes, terminology, accessibility, responsiveness, scope, AI placement, validation, loading and empty states, brand and legal disclosure, notifications, references, money, dates.
- **13 screens** (`UNB-UI-S-*`), covering every release-1 actor goal (RCD-013) with the exception-first model (RCD-026).
- **6 documents** (`UNB-UI-D-*`), rendered by Folio and the Unbound render service (RCD-009).
- **1 embedded widget** (`UNB-UI-W-001`), read-only in release 1.

Every record names the exact Forma components or Folio elements it uses, its states, its actions with authority level, its responsive class, and Given/When/Then acceptance criteria, and traces to the requirements it realises.

## 2. How "Forma and Folio can handle everything" is checked

Three layers, from cheapest to strongest:

1. **Name check (automated, gate G12).** Every component named in a UI requirement must exist in the catalog snapshot taken at the inspected commit: `data/forma-catalog.csv` (170 components, Forma `d7c499e`, v1.0.2) and `data/folio-elements.csv` (27 elements, Folio `56a18e1`, v0.4.2). Anything a requirement needs that does not exist must cite a registered capability gap. Current result: all 103 component references resolve.
2. **Fit check (design prototypes).** Every screen and document is built as a static prototype from the real Forma build (compiled locally from source with its F# token compiler) and Folio components, **with no custom CSS and no inline styles**. Anything that cannot be expressed is recorded as a gap rather than patched. Results: §5.
3. **Quality check (measured).**
   - Screens: axe-core WCAG 2.2 AA at 320 and 1280 px, plus a page-level horizontal-scroll check.
   - Documents: PDF page counts, repeating headers, page numbering, protected legal text, determinism of re-renders, and tagged-PDF status.

What this does **not** prove: application behaviour that Forma leaves to the application through Limen (sorting, async submission, focus management, offline drafts). Those are Unbound engine requirements and are marked as such in the verification files.

## 3. Requirements index

<!-- BEGIN GENERATED: ui-index -->
| ID | Kind | Title | Forma components | Folio elements | Capability gaps | ACs |
|---|---|---|---|---|---|---|
| UNB-UI-G-001 | global | Status vocabulary and visual mapping | status-lozenge, operation-status |  | — | 2 |
| UNB-UI-G-002 | global | Five-dimension grant progress | steps, status-lozenge, timeline, freshness |  | FORMA-GAP-09 | 2 |
| UNB-UI-G-003 | global | Freshness of evidence and projections | freshness, status-lozenge |  | — | 1 |
| UNB-UI-G-004 | global | Authoritative amounts versus informational values | metric-card, key-value-list, freshness |  | — | 1 |
| UNB-UI-G-005 | global | Consequential action confirmation | dialog, key-value-list, callout, button |  | FORMA-GAP-01, FORMA-GAP-14 | 1 |
| UNB-UI-G-006 | global | Unknown command outcome handling | operation-status, status-lozenge, recovery-actions |  | — | 1 |
| UNB-UI-G-007 | global | Sponsor terminology and protected legal text | prose, text-roles |  | — | 1 |
| UNB-UI-G-008 | global | Accessibility conformance | skip-link, visually-hidden, validation-summary, landmark-region |  | — | 1 |
| UNB-UI-G-009 | global | Responsive classification | mobile-action-bar, bounded-overflow, data-grid, responsive-grid |  | — | 1 |
| UNB-UI-G-010 | global | Scope and tenant context | scope-trail, select, dialog |  | — | 1 |
| UNB-UI-G-011 | global | AI assistance placement and labelling | flyout, conversation, composer, evidence-relationship, callout |  | FORMA-GAP-16, FORMA-GAP-17 | 1 |
| UNB-UI-G-012 | global | Validation and fault presentation | validation-summary, validation-message, fault-inline, fault-banner, fault-reference, form, form-row |  | FORMA-GAP-02 | 1 |
| UNB-UI-G-013 | global | Loading and empty states | skeleton, empty-state, spinner |  | — | 1 |
| UNB-UI-G-014 | global | Brand and legal-sponsor disclosure | site-footer, app-identity, navigation-shell | ef-print-footer | FORMA-GAP-06 | 1 |
| UNB-UI-G-015 | global | In-app notification behaviour | toast, feed-item, timeline |  | — | 1 |
| UNB-UI-G-016 | global | Human-readable record references | identifier | ef-print-header | FORMA-GAP-07 | 1 |
| UNB-UI-G-017 | global | Money and percentage display | measure, key-value-list |  | FORMA-GAP-08 | 1 |
| UNB-UI-G-018 | global | Dates and time zones | timeline, facts |  | — | 1 |
| UNB-UI-S-001 | screen | Giving home (donor, advisor, successor) | navigation-shell, scope-trail, metric-card, key-value-list, freshness, work-queue, feed-item, mobile-action-bar, status-lozenge, empty-state, site-footer, disc… |  | FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-13 | 2 |
| UNB-UI-S-002 | screen | Charity search and selection | search, collection-toolbar, active-filter-summary, data-grid, status-lozenge, freshness, pagination, empty-state, form |  | FORMA-GAP-06, FORMA-GAP-08 | 2 |
| UNB-UI-S-003 | screen | Grant recommendation flow | wizard, steps, form, form-row, text-field, textarea, choice-group, checkbox, select, date-time-field, callout, validation-summary, validation-message, dialog,… |  | FORMA-GAP-01, FORMA-GAP-02, FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-09, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-004 | screen | Grant detail | record-header, identifier, steps, status-lozenge, facts, key-value-list, timeline, freshness, operation-status, dialog, provenance-trail |  | FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-09, FORMA-GAP-14 | 2 |
| UNB-UI-S-005 | screen | Add money (contribution intent) and contribution status | choice-group, form, form-row, text-field, numeric-stepper, identifier, callout, steps, status-lozenge, data-grid, key-value-list, timeline, dialog |  | FORMA-GAP-02, FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-09 | 2 |
| UNB-UI-S-006 | screen | Participants, delegation and successors | data-grid, status-lozenge, dialog, form, select, allocation, hierarchy-tree, validation-summary |  | FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-12, FORMA-GAP-14 | 1 |
| UNB-UI-S-007 | screen | Document center | data-grid, collection-toolbar, select, identifier, status-lozenge, empty-state, pagination |  | FORMA-GAP-06 | 1 |
| UNB-UI-S-008 | screen | Staff exception inbox | workspace-shell, work-queue, data-grid, collection-toolbar, active-filter-summary, status-lozenge, metric-card, empty-state, pagination |  | FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15 | 1 |
| UNB-UI-S-009 | screen | Case workspace | record-header, facts, evidence-relationship, conflict-review, provenance-trail, obligation-panel, readiness-checklist, dialog, textarea, flyout, conversation,… |  | FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-11, FORMA-GAP-13, FORMA-GAP-15, FORMA-GAP-16, FORMA-GAP-17 | 2 |
| UNB-UI-S-010 | screen | Sponsor policy configuration and approval | form, form-row, rule-builder, diff-viewer, comparison-pairs, readiness-checklist, dialog, date-time-field, validation-summary, flyout |  | FORMA-GAP-02, FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-11 | 1 |
| UNB-UI-S-011 | screen | Sponsor overview | dashboard-grid, metric-card, metric-value-change, data-grid, status-lozenge, freshness |  | FORMA-GAP-05, FORMA-GAP-06, FORMA-GAP-08 | 1 |
| UNB-UI-S-012 | screen | Charity view | data-grid, status-lozenge, identifier, key-value-list, form, callout, timeline |  | FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08 | 3 |
| UNB-UI-S-013 | screen | Notification preferences | switch, form, form-row, callout |  | FORMA-GAP-06, FORMA-GAP-11 | 1 |
| UNB-UI-D-001 | document | Contribution acknowledgment |  | ef-print-document, ef-print-header, ef-print-footer, ef-print-section, ef-print-table, ef-print-note, ef-print-page-num… | FOLIO-GAP-01, FOLIO-GAP-02, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-07 | 1 |
| UNB-UI-D-002 | document | Fund statement |  | ef-print-document, ef-print-title-page, ef-print-header, ef-print-footer, ef-print-page-number, ef-print-section, ef-pr… | FOLIO-GAP-03, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-08, FOLIO-GAP-09, FOLIO-GAP-11 | 1 |
| UNB-UI-D-003 | document | Grant confirmation and remittance advice |  | ef-print-document, ef-print-header, ef-print-footer, ef-print-section, ef-print-table, ef-print-note | FOLIO-GAP-01, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-07 | 1 |
| UNB-UI-D-004 | document | Annual DAF reporting package (Schedule D Part I support) |  | ef-print-document, ef-print-title-page, ef-print-section, ef-print-metric, ef-print-table, ef-print-integrity, ef-print… | FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-12 | 1 |
| UNB-UI-D-005 | document | Case decision record |  | ef-print-document, ef-print-header, ef-print-section, ef-print-table, ef-print-finding, ef-print-callout, ef-print-foot… | FOLIO-GAP-04, FOLIO-GAP-06, FOLIO-GAP-11, FOLIO-GAP-12 | 1 |
| UNB-UI-D-006 | document | Audit evidence package |  | ef-print-document, ef-print-title-page, ef-print-toc, ef-print-section, ef-print-table, ef-print-integrity, ef-print-pa… | FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-10 | 1 |
| UNB-UI-W-001 | widget | Embedded grant status widget (partner host) | surface, status-lozenge, key-value-list, identifier |  | UNB-GAP-W-01, FORMA-GAP-08 | 1 |
| UNB-UI-DS-001 | design-system | Token source and brand layering | app-identity |  | FORMA-GAP-20 | 3 |
| UNB-UI-DS-002 | design-system | Application type scale | text-roles, section-heading |  | FORMA-GAP-18 | 2 |
| UNB-UI-DS-003 | design-system | Spacing and density | stack, cluster |  | FORMA-GAP-18 | 2 |
| UNB-UI-DS-004 | design-system | Colour roles and semantic status | status-lozenge |  | FORMA-GAP-20 | 2 |
| UNB-UI-DS-005 | design-system | Interaction states | button |  | FORMA-GAP-01, FORMA-GAP-11 | 2 |
| UNB-UI-DS-006 | design-system | Buttons and action emphasis | button |  | FORMA-GAP-01 | 2 |
| UNB-UI-DS-007 | design-system | Panels and grouping | surface, section |  | FORMA-GAP-20 | 1 |
| UNB-UI-DS-008 | design-system | Status chip | status-lozenge, icon |  | FORMA-GAP-21 | 2 |
| UNB-UI-DS-009 | design-system | Task list (things that need the user) | work-queue, priority-stack |  | FORMA-GAP-22 | 1 |
| UNB-UI-DS-010 | design-system | Notices for restrictions, staleness and outages | callout, fault-banner |  | — | 2 |
| UNB-UI-DS-011 | design-system | Iconography | icon |  | FORMA-GAP-23 | 1 |
| UNB-UI-DS-012 | design-system | Sponsor white-labelling | app-identity |  | FORMA-GAP-20 | 2 |
| UNB-UI-DS-013 | design-system | Empty and onboarding states | empty-state, readiness-checklist |  | — | 1 |
| UNB-UI-DS-014 | design-system | Skeleton loading and layout stability | skeleton |  | — | 2 |
| UNB-UI-DS-015 | design-system | Dialogs | dialog |  | FORMA-GAP-14 | 1 |
| UNB-UI-DS-016 | design-system | Notifications and task resolution | feed-item |  | — | 1 |
| UNB-UI-DS-017 | design-system | Forms and validation | text-field, validation-summary, validation-message, form |  | FORMA-GAP-02 | 1 |
| UNB-UI-DS-018 | design-system | Charts and reporting | disclosure |  | FORMA-GAP-05 | 1 |
| UNB-UI-N-001 | navigation | Application frame | skip-link, app-identity, navigation-shell |  | FORMA-GAP-06 | 1 |
| UNB-UI-N-002 | navigation | Primary destinations by audience | navigation-shell |  | — | 1 |
| UNB-UI-N-003 | navigation | Responsive navigation pattern | navigation-shell |  | FORMA-GAP-06 | 2 |
| UNB-UI-N-004 | navigation | Scope and fund context | scope-trail |  | — | 2 |
| UNB-UI-N-005 | navigation | Page header | record-header, section-heading |  | — | 1 |
| UNB-UI-N-006 | navigation | Information architecture of the donor home | priority-stack |  | — | 2 |
| UNB-UI-F-001 | financial-presentation | Money never wraps | identifier |  | FORMA-GAP-08 | 1 |
| UNB-UI-F-002 | financial-presentation | Headline figure | metric-card |  | FORMA-GAP-08 | 1 |
| UNB-UI-F-003 | financial-presentation | Signs and negative values |  |  | FORMA-GAP-08 | 1 |
| UNB-UI-F-004 | financial-presentation | Tables of money | dense-ledger, data-grid |  | FORMA-GAP-03 | 2 |
| UNB-UI-F-005 | financial-presentation | Dates and times |  |  | — | 1 |
| UNB-UI-F-006 | financial-presentation | Freshness wording | freshness |  | — | 1 |
| UNB-UI-F-007 | financial-presentation | Fund composition | allocation |  | FORMA-GAP-12 | 1 |
| UNB-UI-R-001 | responsive | Breakpoints and viewports |  |  | FORMA-GAP-19 | 1 |
| UNB-UI-R-002 | responsive | Container-driven components |  |  | FORMA-GAP-15 | 1 |
| UNB-UI-R-003 | responsive | Source order equals priority | priority-stack |  | — | 1 |
| UNB-UI-A-001 | accessibility | WCAG 2.2 AA automated gate |  |  | — | 1 |
| UNB-UI-A-002 | accessibility | Keyboard and focus | skip-link |  | — | 1 |
| UNB-UI-A-003 | accessibility | Screen-reader semantics | landmark-region, visually-hidden |  | — | 1 |
| UNB-UI-A-004 | accessibility | Targets, reflow, spacing and motion |  |  | — | 1 |
| UNB-UI-A-005 | accessibility | Accessible authentication and redundant entry |  |  | — | 1 |
| UNB-UI-V-001 | visual-regression | Reference images and comparison |  |  | — | 1 |
| UNB-UI-V-002 | visual-regression | Scenario coverage matrix |  |  | — | 1 |
| UNB-UI-S-014 | screen | Case workspace frame (typed panels) | record-header, status-lozenge, facts, evidence-relationship, conflict-review, freshness, provenance-trail, obligation-panel, readiness-checklist, timeline, dia… |  | FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-11, FORMA-GAP-13, FORMA-GAP-14, FORMA-GAP-15, FORMA-GAP-16, FORMA-GAP-17, UNB-GAP-W-02 | 5 |
| UNB-UI-P-001 | panel | Charitable default disposition panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, timeline, callout, identi… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 4 |
| UNB-UI-P-002 | panel | Alternate eligibility evidence panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, identifier, file-upload, provenance-tra… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-P-003 | panel | Charity hold panel - global (platform-wide) | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, select, provenance-trail, timeline, ide… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 4 |
| UNB-UI-P-004 | panel | Charity identity panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, comparison-grid, choice-group, identifi… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-07 | 3 |
| UNB-UI-P-005 | panel | Destination verification panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, diff-viewer, identifier, security-block… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 4 |
| UNB-UI-P-006 | panel | Fee waiver panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, identifier, text-field, v… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-007 | panel | Fund application panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, identifier, security-evidence-age |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-008 | panel | Gift acceptance panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, measure, identifier |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-009 | panel | Liquidity advance panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, identifier, measure |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-010 | panel | Platform-safety policy change panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, diff-viewer, callout, security-invarian… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-P-011 | panel | Positive-pay exception panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, comparison-pairs, identifier, obligatio… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-012 | panel | Price exception panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, identifier, provenance-tr… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 3 |
| UNB-UI-P-013 | panel | Recipient class (counsel) panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, file-upload, identifier |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-P-014 | panel | Related-party payee panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, comparison-pairs, identifier |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-P-015 | panel | Sanctions potential match panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, comparison-pairs, identifier, dense-led… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 4 |
| UNB-UI-P-016 | panel | Succession allocation panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, dense-ledger, identifier, security-bloc… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08, FORMA-GAP-12 | 3 |
| UNB-UI-P-017 | panel | Supporting-organization certification review panel (staff, read-only) | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, identifier, timeline |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 2 |
| UNB-UI-P-018 | panel | Unknown payment panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, operation-status, timeline, identifier,… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08, FORMA-GAP-24 | 5 |
| UNB-UI-P-019 | panel | Sanctions post-block release panel | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, identifier, dense-ledger, obligation-pa… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08 | 4 |
| UNB-UI-P-020 | panel | Charity hold panel - sponsor-only | facts, key-value-list, evidence-relationship, freshness, status-lozenge, readiness-checklist, dialog, textarea, button, callout, file-upload, timeline, identif… |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-061 | screen | Supporting-organization certification (charity) | form, form-row, checkbox, text-field, validation-message, validation-summary, dialog, button, status-lozenge, freshness, key-value-list, operation-status |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-015 | screen | Fund opening and lifecycle requests (workspace frame) | navigation-shell, scope-trail, work-queue, status-lozenge, record-header, identifier, mobile-action-bar, empty-state, skeleton, site-footer |  | FORMA-GAP-06, FORMA-GAP-11 | 3 |
| UNB-UI-S-016 | screen | Help and support status (workspace frame) | navigation-shell, scope-trail, work-queue, status-lozenge, identifier, empty-state, skeleton, mobile-action-bar, site-footer |  | FORMA-GAP-06 | 2 |
| UNB-UI-S-017 | screen | Grant approval (workspace frame) | navigation-shell, scope-trail, callout, metric-card, work-queue, status-lozenge, freshness, empty-state, skeleton, mobile-action-bar |  | FORMA-GAP-06, FORMA-GAP-08 | 2 |
| UNB-UI-S-018 | screen | Treasury and payments (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, dashboard-grid, metric-card, data-grid |  | FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15 | 2 |
| UNB-UI-S-019 | screen | Fund accounting and close (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, dashboard-grid, metric-card, select |  | FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15, FORMA-GAP-25 | 2 |
| UNB-UI-S-020 | screen | Regulatory reporting (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, steps, metric-card |  | FORMA-GAP-06, FORMA-GAP-25 | 2 |
| UNB-UI-S-021 | screen | Records and retention (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, metric-card, data-grid |  | FORMA-GAP-06 | 2 |
| UNB-UI-S-022 | screen | Fund lifecycle operations (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, data-grid |  | FORMA-GAP-06, FORMA-GAP-15 | 2 |
| UNB-UI-S-023 | screen | Sponsor implementation and activation (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, readiness-checklist, steps |  | FORMA-GAP-06, FORMA-GAP-11 | 2 |
| UNB-UI-S-024 | screen | Migration (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, steps, metric-card, alert |  | FORMA-GAP-06 | 2 |
| UNB-UI-S-025 | screen | Support console (workspace frame) | workspace-shell, navigation-shell, scope-trail, tabs, work-queue, status-lozenge, freshness, empty-state, skeleton, key-value-list |  | FORMA-GAP-06, FORMA-GAP-15 | 2 |
| UNB-UI-S-026 | screen | Recurring schedule management | master-detail, data-grid, record-header, identifier, status-lozenge, key-value-list, timeline, form, text-field, select, date-time-field, checkbox, dialog, val… |  | FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-027 | screen | Invitation acceptance | record-header, identifier, key-value-list, prose, checkbox, button, dialog, status-lozenge, alert, validation-summary, mobile-action-bar, wizard |  | FORMA-GAP-01, FORMA-GAP-14 | 3 |
| UNB-UI-S-028 | screen | Representative evidence upload | form, select, readiness-checklist, file-upload, operation-status, validation-summary, status-lozenge, button, mobile-action-bar |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-029 | screen | Verified-channel confirmation | verification-frame, diff-viewer, key-value-list, identifier, status-lozenge, button, dialog, alert, operation-status, mobile-action-bar |  | FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-030 | screen | Fund application | wizard, form, form-row, text-field, select, checkbox, prose, validation-summary, validation-message, steps, status-lozenge, dialog, operation-status, mobile-ac… |  | FORMA-GAP-09, FORMA-GAP-14 | 3 |
| UNB-UI-S-031 | screen | Fund closure request | record-header, steps, obligation-panel, status-lozenge, freshness, key-value-list, dialog, button, mobile-action-bar |  | FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14 | 2 |
| UNB-UI-S-032 | screen | Outbound transfer request | form, search, combobox, segmented-control, checkbox, textarea, prose, freshness, dialog, validation-summary, status-lozenge, mobile-action-bar |  | FORMA-GAP-11, FORMA-GAP-14 | 3 |
| UNB-UI-S-033 | screen | Support case status | master-detail, data-grid, record-header, identifier, status-lozenge, timeline, textarea, file-upload, dialog, mobile-action-bar, empty-state |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-034 | screen | Grant batch approval | data-grid, collection-toolbar, checkbox, disclosure, path-summary, status-lozenge, freshness, identifier, dialog, textarea, operation-status, mobile-action-bar… |  | FORMA-GAP-04, FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-26 | 3 |
| UNB-UI-S-035 | screen | Check void and reissue | data-grid, collection-toolbar, active-filter-summary, status-lozenge, freshness, identifier, key-value-list, readiness-checklist, dialog, textarea, operation-s… |  | FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-15 | 3 |
| UNB-UI-S-036 | screen | OFAC blocked property report | data-grid, status-lozenge, identifier, key-value-list, file-upload, dialog, validation-summary, preview-surface, empty-state | ef-print-document, ef-print-header, ef-print-section, ef-print-table, ef-print-footer | FORMA-GAP-08, FOLIO-GAP-02 | 2 |
| UNB-UI-S-037 | screen | Payment release approval | master-detail, data-grid, readiness-checklist, security-blockers, evidence-relationship, freshness, status-lozenge, identifier, dialog, textarea, operation-sta… |  | FORMA-GAP-08, FORMA-GAP-11 | 3 |
| UNB-UI-S-038 | screen | Unclaimed property disposition | data-grid, status-lozenge, identifier, key-value-list, file-upload, textarea, dialog, validation-summary, empty-state |  | FORMA-GAP-08, FORMA-GAP-11 | 2 |
| UNB-UI-S-039 | screen | Form 8282 obligation tracker | data-grid, readiness-checklist, status-lozenge, identifier, file-upload, dialog, empty-state |  | FORMA-GAP-08, FORMA-GAP-11 | 2 |
| UNB-UI-S-040 | screen | Form 8283 donee signature | data-grid, key-value-list, identifier, status-lozenge, dialog, prose, readiness-checklist, empty-state | ef-print-document, ef-print-section, ef-print-footer | FORMA-GAP-11, FOLIO-GAP-12 | 2 |
| UNB-UI-S-041 | screen | Month-end close certification | record-header, dense-ledger, status-lozenge, freshness, metric-card, progress-bar, readiness-checklist, identifier, key-value-list, dialog, textarea, operation… |  | FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-25, FORMA-GAP-24 | 3 |
| UNB-UI-S-042 | screen | NAV review and approval | master-detail, data-grid, dense-ledger, record-header, status-lozenge, freshness, key-value-list, provenance-trail, readiness-checklist, dialog, textarea, oper… |  | FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-24 | 3 |
| UNB-UI-S-043 | screen | Pool liquidity buffer | data-grid, metric-card, freshness, status-lozenge, key-value-list, dialog, operation-status |  | FORMA-GAP-08, FORMA-GAP-11 | 2 |
| UNB-UI-S-044 | screen | Reconciliation attestation review | data-grid, comparison-pairs, evidence-relationship, progress-bar, status-lozenge, segmented-control, textarea, dialog, operation-status |  | FORMA-GAP-08, FORMA-GAP-11 | 2 |
| UNB-UI-S-045 | screen | Account-type classification register | data-grid, diff-viewer, status-lozenge, key-value-list, file-upload, dialog |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-046 | screen | Annual DAF reporting workspace | record-header, steps, dense-ledger, status-lozenge, key-value-list, file-upload, dialog, textarea, preview-surface, operation-status | ef-print-document, ef-print-table, ef-print-footer | FORMA-GAP-08, FORMA-GAP-09, FORMA-GAP-11, FOLIO-GAP-12 | 2 |
| UNB-UI-S-047 | screen | Legal hold management | data-grid, rule-builder, status-lozenge, key-value-list, dialog, textarea, empty-state |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-048 | screen | Retention disposition batch approval | data-grid, key-value-list, status-lozenge, readiness-checklist, dialog, operation-status |  | FORMA-GAP-11, FORMA-GAP-24 | 2 |
| UNB-UI-S-049 | screen | Fund closure workspace | master-detail, data-grid, obligation-panel, status-lozenge, timeline, key-value-list, dialog |  | FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-15 | 2 |
| UNB-UI-S-050 | screen | Board delegation register | data-grid, status-lozenge, key-value-list, file-upload, date-time-field, dialog, validation-summary |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-051 | screen | Opening balance tie-out workspace | dense-ledger, status-lozenge, freshness, file-upload, textarea, dialog |  | FORMA-GAP-08, FORMA-GAP-11 | 2 |
| UNB-UI-S-052 | screen | Sponsor activation gate | readiness-checklist, security-blockers, status-lozenge, freshness, key-value-list, dialog, operation-status |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-053 | screen | Sponsor contract and activation record | record-header, timeline, key-value-list, file-upload, readiness-checklist, dialog |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-054 | screen | Sponsor implementation readiness checklist | readiness-checklist, status-lozenge, freshness, diagnostic-status, button |  | — | 2 |
| UNB-UI-S-055 | screen | Tenant provisioning console | key-value-list, status-lozenge, operation-status, diagnostic-status, textarea, dialog |  | — | 2 |
| UNB-UI-S-056 | screen | Imported payee verification queue | work-queue, record-header, identifier, evidence-relationship, readiness-checklist, status-lozenge, freshness, key-value-list, metric-card, dialog, empty-state |  | FORMA-GAP-07, FORMA-GAP-11 | 3 |
| UNB-UI-S-057 | screen | Migration control console | record-header, identifier, steps, dense-ledger, data-grid, status-lozenge, freshness, key-value-list, alert, dialog, textarea, operation-status |  | FORMA-GAP-08, FORMA-GAP-11 | 3 |
| UNB-UI-S-058 | screen | Migration cutover authorization | record-header, readiness-checklist, freshness, status-lozenge, key-value-list, alert, dialog, textarea, operation-status |  | FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-24 | 4 |
| UNB-UI-S-059 | screen | Migration mapping review | data-grid, diff-viewer, readiness-checklist, status-lozenge, key-value-list, dialog |  | FORMA-GAP-11 | 2 |
| UNB-UI-S-060 | screen | Assisted change console | wizard, form, select, text-field, diff-viewer, key-value-list, status-lozenge, readiness-checklist, dialog, validation-summary |  | FORMA-GAP-11 | 3 |
<!-- END GENERATED: ui-index -->

## 4. Capability gaps (Forma, Folio and Unbound-owned)

Gaps were filed as issues in the Forma and Folio repositories on owner instruction (2026-11-23); see the "Filed as" column. No Forma or Folio code was changed. Each gap has a release-1 workaround that keeps designs buildable, and an acceptance criterion the owning component can adopt.

<!-- BEGIN GENERATED: ui-gaps -->
| Gap | Component | Owner | Filed as | Need | Release-1 workaround | Acceptance |
|---|---|---|---|---|---|---|
| FORMA-GAP-01 | Button variants (primary, destructive) and async action button | Forma | kemiller2002/forma#186 (narrowed: busy and disabled states already exist; primary variant is marketing-scoped only, comment added 2026-11-24) | Partly implemented: disabled and aria-busy button states exist (components.css ~5751); primary (outside marketing scope) and destructive variants missing. | Use native button with text labels; confirm destructive actions via dialog (G-005); disable the button and show operation-status while pending. Acceptable for design; not for production visual hierarchy. | data-forma-variant=primary\|destructive styles exist in components.css outside the marketing scope; an aria-busy button state is styled and announced; axe passes. |
| FORMA-GAP-02 | Field invalid, disabled and read-only styling | Forma | kemiller2002/forma#187 (corrected: styles exist at d7c499e; contrast test and inventory correction remain) | Implemented at d7c499e (foundations.css 160-177); remaining: contrast browser test and correcting COMPONENT-INVENTORY, which still lists it as unimplemented. | Use as shipped. | :disabled, :read-only, [aria-invalid=true] and :user-invalid styles exist in foundations.css with 3:1 non-text contrast; a browser test proves each. |
| FORMA-GAP-03 | Data table styling for plain table | Forma | not filed: not needed for release 1 | Not required by release-1 designs: data-grid and dense-ledger cover all tables. | Use data-grid or dense-ledger. | n/a for release 1 |
| FORMA-GAP-04 | Bulk action bar | Forma | not filed: not needed for release 1 | Not required by release 1: the exception inbox has no bulk actions by design (each decision is individual). One-click batch approval (RCD-005) is a single dialog listing items. | dialog + data-grid | n/a for release 1 |
| FORMA-GAP-05 | Visualization primitives and chart accessibility contract | Forma | kemiller2002/forma#188 | Trend views for sponsor overview and investment performance (post release 1). | Tables and metric-value-change only (S-011). | A chart primitive with a mandatory text-table equivalent and axe-tested contrast. |
| FOLIO-GAP-01 | Letter primitives (letterhead, address block, signature block) | Folio | kemiller2002/folio#62 | Acknowledgments and remittances are letters. | Compose from ef-print-header and ef-print-section with semantic address and paragraph markup (proven in design prototype D-001). | Documented recipe or primitives with print-position rules for window envelopes (address block at a fixed position). |
| FOLIO-GAP-02 | Verification code and document identity block | Unbound (document registry) + Folio presentation | kemiller2002/folio#67 | Each issued document shows its reference and a verification code or URL that resolves in the document registry (REQ-X-034). | Text verification code inside ef-print-footer; QR code generation is Unbound-owned (SVG). | Footer shows the reference and the verification URL; the registry resolves the hash. |
| FOLIO-GAP-03 | Deterministic server-side PDF rendering (P2) and batch generation | Unbound (render service), Folio (tier P2 contract) | kemiller2002/folio#47 (existing; evidence comment added) | Monthly statements for every fund, re-renderable byte-for-byte or with a stable content hash. | Headless Chromium print-to-PDF from the Folio HTML (~0.25 s per document). Measured: per-page text hash stable 7/7; raw bytes identical 7/7 once /CreationDate and /ModDate are stripped, so the render service MUST set fi… | Re-rendering the same snapshot yields an identical normalized content hash; throughput is measured. |
| UNB-GAP-W-01 | Widget host, frame protocol and scoped tokens | Unbound (security-owned; Limen effects) | Unbound-owned: not filed externally | Read-only embedded status (W-001). | Plain iframe with server-issued short-lived token; no postMessage commands in release 1. | CSP frame-ancestors restricted to registered partner origins; token TTL ≤ 10 minutes; no PII beyond the card. |
| FOLIO-GAP-04 | ef-print-page-number / P1-P2 page counters | Folio | kemiller2002/folio#59 | 'Page n of N' on every page (D-002, all multi-page documents). | Unbound render service injects the @page margin-box counter rule at render time (measured: passes 7/7 documents). Remove when Folio ships the rule. | With no consumer CSS, ef-print-footer[repeat=page] containing ef-print-page-number[format=page-of-pages] yields 'Page n of N' on every body page in P1/P2 Chromium; P0 strict mode fails export instead… |
| FOLIO-GAP-05 | Running header/footer (repeat=page) and per-section running content | Folio | kemiller2002/folio#60 | Reference (G-016), legal-sponsor disclosure (G-014) and verification code on every page; correct identity per document unit. | Render-time @page margin boxes for reference and page number (measured 7/7); disclosure repeated in a final section; one document per PDF (no multi-unit PDFs) in release 1. | Running slots print on every page of their profile, never fragment, reserve margin space, and can be set per section; pdftotext shows reference and disclosure on every page of a 70-row statement. |
| FOLIO-GAP-06 | Unbroken identifier / reference text treatment | Folio | kemiller2002/folio#61 | References, EINs, URLs and hashes must print and extract intact. | Shorten verification URL to a short code plus registry host; put hashes in their own full-width row; accept wrapping of hashes in release 1. | A primitive keeps identifiers of 32 characters or fewer and dates unbroken; pdftotext output of each identifier equals the source string. |
| FOLIO-GAP-07 | Letter address block at fixed window-envelope position | Folio | kemiller2002/folio#62 | Mailed acknowledgments and remittances (D-001, D-003). | Release 1 delivers acknowledgments electronically (E-SIGN consent, counsel Q10); mailed letters use a fixed-height letterhead so the address lands in the window, verified per brand. | An address primitive places its box at a configurable fixed page position (e.g. #10 window 0.875in/2.0in, 4.0x1.125in); pdftotext -bbox confirms for any letterhead length. |
| FOLIO-GAP-08 | Multi-page table totals printed once | Folio | kemiller2002/folio#63 | Statement grand totals must not repeat on every page (D-002). | Move totals out of tfoot into a following summary block (ef-print-keep). | ef-print-table[footer=final] prints tfoot only on the last fragment; carried-forward subtotal pattern documented and tested. |
| FOLIO-GAP-09 | Heading keep-with-next by default | Folio | kemiller2002/folio#64 | No orphaned headings above page breaks. | Use the .ef-dense recipe on all Unbound documents (measured: no orphan). | Component layer applies break-after: avoid to h1-h6 before blocks and tables; fixture shows no heading in the last 10% of a page. |
| FOLIO-GAP-10 | ef-print-toc target page numbers | Folio | kemiller2002/folio#65 | Tables of contents with page numbers (D-002, D-006). | Two-pass render in the Unbound render service fills data-page from the first-pass PDF; or omit page numbers in release 1. | P2 export fills [data-page] from a first-pass layout; numbers match the PDF. |
| FOLIO-GAP-11 | Tagged PDF in the P2 export contract | Folio + Unbound render service | kemiller2002/folio#66 | Distributed PDFs should be tagged; RCD-009 makes the HTML twin the accessible rendition. | Render with Chromium tagged:true and outline:true (measured: Tagged: yes); HTML twin remains the conformance rendition. | P2 pins tagged:true; structure validated by a PDF/UA checker (veraPDF or PAC) with documented limits. |
| FOLIO-GAP-12 | Signature / approval block recipe | Folio | kemiller2002/folio#62 | Preparer/signer (D-004) and decider (D-005) blocks. | ef-meta-list inside ef-print-keep. | Recipe with name, role, date and signature evidence, kept together, semantic dl, three documented examples. |
| FORMA-GAP-06 | Application shell (.forma-app-shell) with footer legal-note slot | Forma | kemiller2002/forma#189 | One application frame with gutters, header, nav, main and a footer holding the legal-sponsor disclosure (G-014). | Compose .forma-site + .forma-site__main + site-footer around navigation-shell (all 14 prototypes). | .forma-app-shell ships with regions and a legal-note slot, works without .forma-site, 0 axe violations and no horizontal scroll at 320/1280. |
| FORMA-GAP-07 | Copyable identifier | Forma | kemiller2002/forma#193 | Copy control for record references (G-016). | Plain Copy button beside forma-identifier; announcement via application. | Button named 'Copy <label> <value>', status announcement, 24x24 target, wraps at 320 px. |
| FORMA-GAP-08 | Numeric/money value presentation | Forma | kemiller2002/forma#190 | Amounts intact, tabular numerals, end-aligned numeric columns, labelled negatives (G-017). | Avoid 3-column metric grids for large amounts; use key-value-list and dense-ledger; accept proportional digits in release 1 design review only. | Numeric value hook with tabular numerals and no mid-number break in metric-card, data-grid (end-aligned) and key-value dd. |
| FORMA-GAP-09 | Multi-dimension status track | Forma | kemiller2002/forma#191 | Five grant dimensions each with state and as-of time plus headline (G-002). | steps items each containing status-lozenge and time (composition proven in S-004, S-005). | Per-item data-state ok/attention/unknown/blocked/neutral with label and time, headline slot, list semantics, forced-colors proof. |
| FORMA-GAP-11 | Unavailable-action pattern (disabled control with visible reason) | Forma | kemiller2002/forma#192 | Segregation-of-duties and required-notice reasons visible (S-009, S-010, S-013). | Ad-hoc reason text linked via aria-describedby; keep reason outside disabled switch description. | Disabled control plus visible reason at >= 4.5:1, linked by aria-describedby, in button rows, record-header, obligation-panel and switch. |
| FORMA-GAP-12 | Read-only allocation summary | Forma | kemiller2002/forma#193 | Text equivalent of a successor percentage split (S-006). | Ordered list with names and shares. | Read-only allocation summary with names, shares, total and as-of version, list semantics. |
| FORMA-GAP-13 | feed-item canonical markup defect (data-state=warning) | Forma | kemiller2002/forma#194 | Canonical patterns must use the lozenge vocabulary. | Unbound uses data-state=attention. | Pattern fixed; catalog test rejects unknown data-state values. |
| FORMA-GAP-14 | Dialog confirmation variant with pinned header and actions | Forma | kemiller2002/forma#195 | Consequence preview usable on a phone (G-005). | Limit phone confirmation to 4 key consequence rows with a 'Show all details' disclosure. | Pinned title + key consequence and pinned action row; tested at 320x568 and 320x800. |
| FORMA-GAP-15 | Container-query reflow (collection-toolbar, obligation-panel, shells) | Forma | kemiller2002/forma#196 | Components inside panes reflow by container width (S-008, S-009). | Place toolbar above workspace-shell and obligation-panel in the primary column (working layouts used in prototypes). | Container queries; screenshot tests in a 20rem pane at 1280 px show no clipped or single-word lines. |
| FORMA-GAP-16 | AI output provenance label and action-free handoff-summary | Forma | kemiller2002/forma#197 | AI output labelled with sources; no consequential controls in AI panels (G-011). | callout with the label plus evidence-relationship for sources; handoff-summary without actions. | Label and sources slot in conversation turns; action-free handoff-summary variant documented. |
| FORMA-GAP-17 | Flyout keyboard-scrollable body (axe scrollable-region-focusable) | Forma | kemiller2002/forma#198 | Keyboard access to overflowing flyout content. | role=region tabindex=0 aria-label on .forma-flyout__body. | Canonical markup fixed; axe test with overflowing text passes. |
| FORMA-GAP-18 | Application type scale and density switch | Forma | kemiller2002/forma#199 | Forma's heading sizes are marketing display sizes (fluid-display-1 to 7.25rem, h1 measure 14ch, h2 18ch); there is no application type scale and no density token (comfortable/compact). | Unbound layer sets --unb-type-* and component sizes (v2/unbound-app.css) | An application scope (e.g. data-forma-context=app) applies a fixed type scale with no heading measure, and a density attribute switches row heights and paddings for data components |
| FORMA-GAP-19 | Breakpoint tokens | Forma | kemiller2002/forma#200 | Components hard-code media queries (48rem); no shared breakpoint tokens for applications to align layout ranges. | Unbound uses 48rem/64rem constants | Documented breakpoint values exported as tokens and used by Forma's own components |
| FORMA-GAP-20 | Raised/sunken/chrome surface roles and elevation tokens | Forma | kemiller2002/forma#201 | Forma has surface primary/secondary/inverse only; applications need raised panel, sunken and chrome roles plus elevation, derived per brand. The Echelon theme defines them only for data-forma-brand=e… | Unbound derives them with color-mix from --forma-color-* tokens | Brand manifests compile raised/sunken/chrome and elevation roles with contrast checks; a panel component uses them |
| FORMA-GAP-21 | Sentence-case status chip | Forma | kemiller2002/forma#202 | status-lozenge renders uppercase monospace at 11px; on donor surfaces the most important fact becomes the smallest, most technical text (v1 audit UXD-012). | Unbound .unb-chip (glyph + words, body face 13px) | A status-lozenge variant in body face and sentence case, keeping glyph, border and forced-colours behaviour |
| FORMA-GAP-22 | Task list (needs-your-attention) pattern | Forma | kemiller2002/forma#203 | work-queue targets staff queues; donors need a compact list of tasks each with icon, title, reason and one action. | Unbound .unb-task | A task-list pattern with exactly-one-primary-action guidance and resolution semantics |
| FORMA-GAP-23 | Domain icons for giving and payments | Forma | kemiller2002/forma#204 | The 80-icon registry has no glyphs for grant/gift, contribution/deposit, charity/organization, bank or certificate. | Closest registry icons (send, add, globe) | Registry adds reviewed domain-neutral icons: gift, bank, building, receipt, certificate |
| FORMA-GAP-24 | Approval quorum tracker (two or more distinct approvers) | Forma | kemiller2002/forma#205 | Restatement, tolerance override, disposition batches, cutover and two-person approvals need a tracker of required approvers, each with status and independence rule (not the preparer). readiness-check… | readiness-checklist with text per approver | A component listing required approvals with approver, status, time and an explicit 'cannot be the preparer' rule, announced on change |
| FORMA-GAP-25 | Accounting period context bar | Forma | kemiller2002/forma#206 | Finance workspaces need a persistent header with period, period state (open, closing, certified), cutoff and snapshot as-of/known-at times; scope-trail and date-range do not carry period state. | scope-trail plus freshness | A context bar component with period state and two timestamps, copyable, with forced-colours support |
| FORMA-GAP-26 | Per-item batch outcome summary | Forma | kemiller2002/forma#207 | Batch approval commits each item in its own transaction, so results are mixed (approved, in review, waiting for funds, unknown); FORMA-GAP-04 assumed a single outcome. | Table of items with status chips | A summary that groups items by outcome with counts and links, and treats Unknown distinctly (G-006) |
| UNB-GAP-W-02 | Typed panel slot registry for the case workspace | Unbound | pending | The case frame (S-014) hosts 18 typed decision panels; a registry maps case type to panel, required facts and permitted actions. | Static mapping in the prototype | Registry data validated against WFM transitions; unknown case types render a safe read-only panel |
<!-- END GENERATED: ui-gaps -->

## 5. Fit evidence from the prototypes

**Verdict:** both component systems can carry the release-1 UI, **with recorded gaps and workarounds**. No screen fails outright. One document, the fund statement, does not fit Folio 0.4.2 unaided: it has no "Page n of N" and no running header or footer. It does fit once the Unbound render service injects the page-margin rules, which was measured as working on 7 of 7 documents.

### Screens (Forma 1.0.2, compiled from source)

[Fit report](../../ui/designs/screens/FORMA-FIT-REPORT.md) · [verification.json](../../ui/designs/screens/verification.json) · screenshots in `requirements/ui/designs/rendered/`

| Result | Screens |
|---|---|
| Fits fully | W-001 (apart from the money-format gap FORMA-GAP-08 and the Unbound-owned widget runtime) |
| Fits with recorded gaps | S-001 to S-013 |
| Most affected | S-004 and S-005: the five-dimension status track is a composition (FORMA-GAP-09). S-008 and S-009: components break inside panes (FORMA-GAP-15). The prototypes keep the broken layouts as evidence and use working layouts instead. |

Measured:
- 0 axe violations (WCAG 2.2 AA tag sets) on all 14 pages, and in 10 opened dialogs and flyouts, at 320 px and 1280 px.
- No page-level horizontal scroll.
- No `<style>`, `style=` or `<script>` in any page.

Not tested: keyboard walkthroughs, focus order after submission, high-contrast mode and zoom. axe does not check contrast on disabled controls.

The checks distinguish what markup demonstrates from what depends on the Unbound engine. The markup demonstrates 12 global and 11 screen-level acceptance criteria. Eleven of each depend on application behaviour: sorting, staleness rules, focus, segregation-of-duties enforcement, idempotent status checks and tokens.

### Documents (Folio 0.4.2)

[Fit report](../../ui/designs/documents/FOLIO-FIT-REPORT.md) · [verification.json](../../ui/designs/documents/verification.json) · PDFs in `requirements/ui/designs/rendered/`

| Document | Pages | Verdict |
|---|---:|---|
| D-001 acknowledgment (cash, securities) | 1 each | Fits with gaps. The legal sentences print verbatim, and the securities variant states no dollar value. |
| D-002 fund statement | 8 | **Does not fit unaided.** It fits with the render-time page-margin injection (FOLIO-GAP-04/05/08). |
| D-003 remittance | 2 | Fits with gaps. No pledge text anywhere in the PDF, its metadata or the HTML. |
| D-004 DAF reporting package | 3 | Fits with gaps |
| D-005 case decision record | 2 | Fits |
| D-006 audit evidence package | 6 | Fits with gaps |

Measured:
- **Text determinism:** the per-page text hash is stable for 7 of 7 documents.
- **Raw PDF bytes:** identical for 7 of 7 once `/CreationDate` and `/ModDate` are fixed, so the render service MUST stamp them from the snapshot.
- **Render time:** about 0.25 s per document.
- **Accessibility:** 0 axe violations on the HTML renditions.
- **Tagging:** PDFs are untagged by default; tagged with `tagged:true` (structure not yet validated).

### Correction recorded

The first issue filing for FORMA-GAP-01 and FORMA-GAP-02 relied on Forma's `docs/COMPONENT-INVENTORY.md`. The fit work then showed that at the same commit Forma already styles disabled, read-only and invalid fields (`foundations.css` lines 160–177) and busy buttons (`components.css` ~5751). forma#186 and forma#187 were rewritten on GitHub to ask only for what is still missing: the primary and destructive variants, a contrast test, and an inventory correction.

### Where the gaps were filed

- **Forma:** #186–#198.
- **Folio:** #59–#67, plus evidence added to the existing #47 (server-side P2 renderer).

The "Filed as" column in §4 maps each gap to its issue. Gaps that release 1 does not need (FORMA-GAP-03/04) and Unbound-owned work (UNB-GAP-W-01) were not filed.

## 6. Requirement records

<!-- BEGIN GENERATED: ui-records -->
### UNB-UI-G-001 — Status vocabulary and visual mapping (global)
- **Requirement:** Every domain state MUST render as a text label from the versioned status catalog, wrapped in a status chip (UNB-UI-DS-008; Forma status-lozenge remains allowed in staff dense ledgers) whose data-state is derived by rule: ok = authoritative final success (Settled, Final, Approved, Accepted); attention = waiting on a named party or action; unknown = outcome or evidence not determinable (PaymentOutcomeUnknown, EvidenceStale, Disputed); blocked = held, blocked, rejected, failed or ineligible. No state may be conveyed by colour alone, and no record may display a final-success label until the authoritative component has recorded the final state.
- **Traces to:** UNB-UX-003, RR-UNB-UX-003-r1, REQ-X-011, RCF-PLT-027
- **Forma components:** `status-lozenge`, `operation-status`
- **Acceptance criteria:**
  - **AC-1** Given a payment attempt whose provider outcome is Unknown; when any screen renders it; then the label reads 'Payment outcome unknown' with data-state=unknown, and no screen shows 'Paid'.
  - **AC-2** Given a grant in state EligibilityHold; when the giving home renders it; then the label reads 'On hold: charity eligibility' with data-state=blocked and a text reason.

### UNB-UI-G-002 — Five-dimension grant progress (global)
- **Requirement:** In staff views, grant status MUST be shown as five ordered dimensions; donor, advisor and charity views show a four-milestone track (Submitted, Approved, Paid, Acknowledged) with dates, derived from the same dimensions, (Recommendation, Eligibility, Approval, Funding, Payment), each with its own label and as-of time, using Forma steps for the sequence and status-lozenge for each dimension. A single headline status MUST be derived deterministically: the first dimension that is not final, except that any Unknown or Blocked dimension becomes the headline.
- **Traces to:** REQ-X-011, RR-REQ-X-002-r1, RR-REQ-X-004-r1, UNB-RCN-FIN-008
- **Forma components:** `steps`, `status-lozenge`, `timeline`, `freshness`
- **Capability gaps:** FORMA-GAP-09
- **Acceptance criteria:**
  - **AC-1** Given Eligibility = Eligible, Approval = Approved, Funding = FundingPlanned, Payment = not started; when the headline is computed; then headline is 'Funding in progress' (attention).
  - **AC-2** Given Payment = Unknown while Funding = Funded; when the headline is computed; then headline is 'Payment outcome unknown' (unknown).

### UNB-UI-G-003 — Freshness of evidence and projections (global)
- **Requirement:** Every value derived from external evidence or from an asynchronous projection MUST show a Forma freshness element with its as-of time. When the value is older than its configured freshness limit it MUST be labelled 'Not current' (unknown) and MUST NOT be offered as the basis for a consequential action.
- **Traces to:** UNB-RCN-FIN-009, UNB-RCN-GRT-014
- **Forma components:** `freshness`, `status-lozenge`
- **Acceptance criteria:**
  - **AC-1** Given a market value as of yesterday 16:00 with a 1-business-day limit; when viewed the next business day at 17:00; then it shows 'Not current' and the Recommend grant action uses authoritative available-to-grant instead.

### UNB-UI-G-004 — Authoritative amounts versus informational values (global)
- **Requirement:** Fund screens MUST show 'Available to grant' (authoritative capacity computed by the ledger, UNB-RCN-FIN-009) separately from 'Market value' (informational, with freshness) and 'Pending' (reserved or in-flight), and MUST never sum them into a single unlabeled balance.
- **Traces to:** UNB-RCN-FIN-009, UNB-ACC-005
- **Forma components:** `metric-card`, `key-value-list`, `freshness`
- **Acceptance criteria:**
  - **AC-1** Given market value $120,000, reserved grants $15,000, unsettled ACH contribution $5,000; when the giving home renders; then Available to grant shows $100,000 with a 'How this is calculated' disclosure listing the three components.

### UNB-UI-G-005 — Consequential action confirmation (global)
- **Requirement:** Every action of authority class A2 or higher MUST present, before submission, a confirmation that states what will change, whether it is reversible, who acts next and the expected time to the next state. The confirm control MUST name the action (for example 'Submit grant recommendation'), not 'OK'.
- **Traces to:** RR-REQ-X-051-r1, AIN-006, UNB-RCN-PLT-005
- **Forma components:** `dialog`, `key-value-list`, `callout`, `button`
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a donor submits a $5,000 grant; when they press Submit; then the confirmation (the review step, or a dialog for record actions) lists amount, charity, fund, 'Your sponsor approves this grant; you can withdraw until approval', and the button reads 'Submit grant recommendation'.

### UNB-UI-G-006 — Unknown command outcome handling (global)
- **Requirement:** When a command returns no definite outcome (Limen OutcomeUnknown, timeout or 5xx after submission), the screen MUST show a Forma operation-status in the unknown state with a 'Check status' action that queries by idempotency key; donor wording is 'We can't confirm your recommendation was received' and the key is never shown to donors (support sees it). It MUST NOT automatically retry and MUST NOT show success or failure.
- **Traces to:** RR-REQ-X-004-r1, UNB-RCN-GRT-008
- **Forma components:** `operation-status`, `status-lozenge`, `recovery-actions`
- **Acceptance criteria:**
  - **AC-1** Given a grant submission whose HTTP response times out; when the client receives OutcomeUnknown; then the operation-status reads 'We cannot confirm your recommendation was received', and pressing Check status returns the recorded outcome for that idempotency key without creating a second recommendation.

### UNB-UI-G-007 — Sponsor terminology and protected legal text (global)
- **Requirement:** All domain nouns in UI, documents, email and assistant output MUST resolve through the sponsor terminology catalog (UNB-RCN-ARC-002). Protected legal statements (the exclusive-legal-control acknowledgment sentence, the advisory-only nature of recommendations, the legal-sponsor disclosure) MUST render verbatim and cannot be overridden.
- **Traces to:** UNB-RCN-ARC-002, UNB-RCN-PLT-007, RCD-003
- **Forma components:** `prose`, `text-roles`
- **Acceptance criteria:**
  - **AC-1** Given sponsor terminology maps 'fund' to 'giving account'; when the giving home and statement render; then both use 'giving account', and the statement's exclusive-legal-control sentence is unchanged.

### UNB-UI-G-008 — Accessibility conformance (global)
- **Requirement:** Every screen and document design MUST conform to WCAG 2.2 AA: zero axe-core violations with the wcag2a, wcag2aa, wcag21aa and wcag22aa tag sets at 320 CSS px and 1280 CSS px widths; full keyboard operation; visible focus; target size at least 24 by 24 CSS px; and focus moved to the result or error summary after every asynchronous submission.
- **Traces to:** UNB-EXP-001, UNB-PLAT-005
- **Forma components:** `skip-link`, `visually-hidden`, `validation-summary`, `landmark-region`
- **Acceptance criteria:**
  - **AC-1** Given any design prototype in requirements/ui/designs; when the axe check runs at 320 and 1280 px; then zero violations are reported.

### UNB-UI-G-009 — Responsive classification (global)
- **Requirement:** Each screen MUST be classified phone-complete, phone-review-and-approve, or desktop-first (UNB-RCN-PLT-011). Donor, advisor, successor and approval journeys MUST be phone-complete. No screen may scroll horizontally at 320 CSS px except inside a bounded-overflow or data-grid region.
- **Traces to:** UNB-RCN-PLT-011, UNB-UX-001, RCD-026
- **Forma components:** `mobile-action-bar`, `bounded-overflow`, `data-grid`, `responsive-grid`
- **Acceptance criteria:**
  - **AC-1** Given the grant recommendation flow at 320 px; when a donor completes it; then every step is usable with no page-level horizontal scroll.

### UNB-UI-G-010 — Scope and tenant context (global)
- **Requirement:** Every authenticated screen MUST make its scope unambiguous: staff screens show sponsor, program and fund or case in a Forma scope-trail; donor, advisor and charity screens show the sponsor brand in the app bar and the fund as the page title and switcher (UNB-UI-N-004). Users with access to more than one tenant MUST choose from a server-issued list, and changing scope MUST re-authorize and reload all data.
- **Traces to:** INT-001, RR-UNB-TEN-004-r1, RCF-PLT-044
- **Forma components:** `scope-trail`, `select`, `dialog`
- **Acceptance criteria:**
  - **AC-1** Given an advisor with clients at two sponsors; when they switch sponsor; then the page reloads under the new server-issued token, and no data from the previous sponsor remains in the DOM.

### UNB-UI-G-011 — AI assistance placement and labelling (global)
- **Requirement:** AI assistance MUST appear only on surfaces with a published assistance contract, opened from an 'Explain' or 'Help me' control into a Forma flyout containing conversation output. Every AI output MUST be labelled 'AI-generated, not a decision' with its sources, and MUST NOT contain controls that submit consequential actions.
- **Traces to:** RR-REQ-X-051-r1, RR-UNB-AI-STD-003-r1, UNB-RCN-PLT-004
- **Forma components:** `flyout`, `conversation`, `composer`, `evidence-relationship`, `callout`
- **Capability gaps:** FORMA-GAP-16, FORMA-GAP-17
- **Acceptance criteria:**
  - **AC-1** Given a donor asks why a grant is on hold; when the assistant answers; then the answer cites the hold reason record and freshness, is labelled as AI-generated, and offers no Approve or Release control.

### UNB-UI-G-012 — Validation and fault presentation (global)
- **Requirement:** Forms MUST show a validation-summary linked to each invalid field and an inline validation-message per field, preserving entered values. System faults MUST use the Forma fault family with a fault-reference identifier that support can look up. Field invalid and disabled states MUST be visually distinct (FORMA-GAP-02).
- **Traces to:** REQ-X-011, UNB-RES-001
- **Forma components:** `validation-summary`, `validation-message`, `fault-inline`, `fault-banner`, `fault-reference`, `form`, `form-row`
- **Capability gaps:** FORMA-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given a grant amount below the sponsor minimum and a missing purpose; when the donor presses Continue; then focus moves to a summary listing both errors, each linking to its field, and entered values remain.

### UNB-UI-G-013 — Loading and empty states (global)
- **Requirement:** Every list and panel MUST define loading (Forma skeleton), empty (Forma empty-state with the next sensible action) and error states. No indeterminate spinner may show for longer than 1 second without explanatory text.
- **Traces to:** ADM-036
- **Forma components:** `skeleton`, `empty-state`, `spinner`
- **Acceptance criteria:**
  - **AC-1** Given a new fund with no grants; when the grants panel renders; then an empty-state reads 'No grants yet' with a 'Recommend a grant' action.

### UNB-UI-G-014 — Brand and legal-sponsor disclosure (global)
- **Requirement:** Each sponsor's brand MUST be applied through a compiled Forma Brand Manifest. Every donor-facing page and document MUST show the legal-sponsor disclosure (legal name, EIN, statement that the sponsor holds legal control) in the footer.
- **Traces to:** RCD-003, REQ-X-030
- **Forma components:** `site-footer`, `app-identity`, `navigation-shell`
- **Folio elements:** `ef-print-footer`
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given sponsor brand 'Harbor Giving' operated by legal sponsor 'Example Charitable Foundation, EIN 00-0000000'; when any donor page renders; then the footer shows the legal sponsor name, EIN and control statement.

### UNB-UI-G-015 — In-app notification behaviour (global)
- **Requirement:** Transient toasts MAY confirm reversible actions only. Consequential outcomes (approved, paid, held, returned) MUST appear in the record timeline and in the notification list (Forma feed-item), sourced from the notification event catalog (UNB-RCN-PLT-002).
- **Traces to:** UNB-RCN-PLT-002, AIN-023
- **Forma components:** `toast`, `feed-item`, `timeline`
- **Acceptance criteria:**
  - **AC-1** Given a grant moves to Paid; when the donor opens the app; then a feed-item and a timeline entry show it; no toast was used for it.

### UNB-UI-G-016 — Human-readable record references (global)
- **Requirement:** Every fund, grant, contribution, case and document detail view and every document MUST show its numeric reference using the Forma identifier component, with a copy control; donor summary lists (home, activity) omit references (UNB-UX-007). The same reference MUST print on every document and remittance.
- **Traces to:** REQ-X-025, RCD-008
- **Forma components:** `identifier`
- **Folio elements:** `ef-print-header`
- **Capability gaps:** FORMA-GAP-07
- **Acceptance criteria:**
  - **AC-1** Given grant 2026-004817; when the grant screen, remittance advice and statement render; then all three show 2026-004817.

### UNB-UI-G-017 — Money and percentage display (global)
- **Requirement:** Amounts MUST display with currency, two decimals and tabular numerals, never rounded below cents, except chart value labels, which MAY be abbreviated ($512.3k) when the exact amounts are in the chart's data table. Negative values MUST carry a text label as well as a sign. Percentage grants MUST show the percentage, its base, the fixing point and, once fixed, the fixed currency amount.
- **Traces to:** RR-UNB-GFUND-013-r1
- **Forma components:** `measure`, `key-value-list`
- **Capability gaps:** FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a 10% grant of fund value fixed at approval; when the grant renders after approval; then it shows '10% of fund value at approval → $12,345.67 (fixed 24 Nov 2026)'.

### UNB-UI-G-018 — Dates and time zones (global)
- **Requirement:** Dates of legal significance (gift date, grant approval, payment date) MUST show as absolute dates in the sponsor's time zone, with UTC in documents and staff details (not in donor UI). Relative time ('2 hours ago') MAY be used only for activity feeds.
- **Traces to:** UNB-RCN-FIN-012, REQ-X-003
- **Forma components:** `timeline`, `facts`
- **Acceptance criteria:**
  - **AC-1** Given a gift completed at 2026-12-31T23:30 America/Chicago; when the contribution renders; then the gift date reads 31 Dec 2026, not 1 Jan 2027.

### UNB-UI-S-001 — Giving home (donor, advisor, successor) (screen)
- **Requirement:** The giving home MUST answer, on one phone screen: how much is available to grant, what is pending and why, and what the user should do next. Regions: application frame with fund title and switcher (UNB-UI-N-004); fund summary (Available to grant as the one headline figure with a composition breakdown of committed, settling and total market value with freshness: UNB-UI-F-002, UNB-UI-F-007); 'Needs your attention' items created by rules (for example, a held grant or a certification request); recent activity; primary actions 'Recommend a grant' and 'Add money'.
- **Traces to:** UNB-UX-001, UX-S02, RCD-026
- **Actors:** Donor, Advisor (scoped), Successor (after activation)
- **Forma components:** `navigation-shell`, `scope-trail`, `metric-card`, `key-value-list`, `freshness`, `work-queue`, `feed-item`, `mobile-action-bar`, `status-lozenge`, `empty-state`, `site-footer`, `disclosure`
- **States:** Loading; Empty (no contributions yet); Normal; Capacity not current; Fund suspended (death or hold)
- **Actions:** Recommend a grant (navigate); Add money (navigate); Open attention item
- **Responsive:** phone-complete: metric cards stack; actions in mobile-action-bar
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-13
- **Acceptance criteria:**
  - **AC-1** Given a fund in Suspended state after a verified death; when a successor opens it before activation; then the page shows 'This fund is suspended pending succession' (blocked) and offers no grant action.
  - **AC-2** Given a donor with 2 rule-created attention items; when they open the giving home; then both items appear directly after the Available to grant panel, each with one named action, and at 390x844 the figure and the 'Needs your attention' heading are both in the first viewport (UNB-UI-N-006).

### UNB-UI-S-002 — Charity search and selection (screen)
- **Requirement:** Charity search MUST implement UNB-RCN-PLT-014: a search field (name, alias, EIN); filters for state and cause; results as a list that recomposes on phones showing legal name, EIN, city/state and platform eligibility (status-lozenge with as-of date). Selecting a result binds the canonical identity. 'Can't find it?' MUST open an identity-resolution request form.
- **Traces to:** UNB-RCN-PLT-014, REQ-X-011
- **Actors:** Donor, Advisor
- **Forma components:** `search`, `collection-toolbar`, `active-filter-summary`, `data-grid`, `status-lozenge`, `freshness`, `pagination`, `empty-state`, `form`
- **States:** Idle; Searching; Results; No results; Search unavailable (fault-inline)
- **Actions:** Search; Filter; Select charity; Request a charity we don't list
- **Responsive:** phone-complete: data-grid recomposes into labelled records at 30rem
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given two 'Hope Foundation' results; when the donor views results on a phone; then each row shows EIN and city/state so they are distinguishable.
  - **AC-2** Given a charity on hold; when it appears in results; then it shows 'On hold' with no reason text from another sponsor and cannot be selected.

### UNB-UI-S-003 — Grant recommendation flow (screen)
- **Requirement:** One page with progressive disclosure, then one review step: Charity (search within the page; S-002 for the full directory); Amount (fixed amount, or percentage with base and fixing point; recurring schedule optional); 3 Purpose and acknowledgements (purpose text; recognition: name, anonymous or custom; the sponsor's required affirmations as individual checkboxes stating the benefit rules verbatim; the pledge question per sponsor policy); Review and submit (the review step itself meets G-005; no second dialog). It MUST validate against sponsor policy at each step, show 'Available to grant' beside the amount, and after submission show the donor milestone track with expected dates (G-002).
- **Traces to:** UNB-GRANT-002, RR-UNB-GRANT-011-r1, UNB-RCN-GRT-009, UNB-RCN-GRT-010, UNB-RCN-GRT-004, RR-UNB-GFUND-013-r1, REQ-X-011
- **Actors:** Donor, Advisor (scoped)
- **Forma components:** `wizard`, `steps`, `form`, `form-row`, `text-field`, `textarea`, `choice-group`, `checkbox`, `select`, `date-time-field`, `callout`, `validation-summary`, `validation-message`, `dialog`, `key-value-list`, `operation-status`, `mobile-action-bar`
- **States:** Draft (saved locally via Limen IndexedDB pack); Validation errors; Submitting; Submitted; Outcome unknown; Rejected by policy (with reason)
- **Actions:** Next/Back; Save draft; Submit grant recommendation (A1 by donor; creates RecommendationSubmitted); Withdraw (until approval)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-02, FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-09, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given an amount above Available to grant; when the donor continues from step 2; then an inline error states the available amount and the recommendation cannot be reviewed or submitted until it is corrected, and entered values are kept.
  - **AC-2** Given a sponsor whose pledge policy is Prohibited; when the donor reaches step 3; then a required question 'Does this grant fulfil a pledge?' appears; answering Yes blocks submission with an explanation.
  - **AC-3** Given submission times out; when the response is unknown; then G-006 applies and no duplicate recommendation is created.

### UNB-UI-S-004 — Grant detail (screen)
- **Requirement:** Grant detail MUST show: record-header with numeric reference and headline status; progress per G-002 (donor milestone track with dates; five dimensions in staff views); charity identity and eligibility snapshot (as-of date); amount (G-017); purpose and recognition; timeline of all events; documents (grant confirmation, remittance); for a returned payment, the donor's choice to wait for corrected bank details or redirect, each with its consequence and the default applied on a stated date; actions allowed in the current state only (Withdraw before approval; Request cancellation after approval, which explains that payment may already be in progress).
- **Traces to:** REQ-X-011, RR-REQ-X-004-r1, TVR-GR-004
- **Actors:** Donor, Advisor, Sponsor staff (read)
- **Forma components:** `record-header`, `identifier`, `steps`, `status-lozenge`, `facts`, `key-value-list`, `timeline`, `freshness`, `operation-status`, `dialog`, `provenance-trail`
- **States:** Each grant state, including Payment outcome unknown, Returned, Voided check, Blocked (sanctions)
- **Actions:** Withdraw recommendation; Request cancellation; Download confirmation
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-09, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a grant whose check was voided after 180 days and the amount re-credited; when the donor views it; then the timeline shows Issued → Outstanding → Voided → Re-credited, with dates and amounts.
  - **AC-2** Given a grant Blocked by a confirmed sanctions match; when the donor views it; then it shows 'This grant cannot be paid' (blocked) without disclosing screening details.

### UNB-UI-S-005 — Add money (contribution intent) and contribution status (screen)
- **Requirement:** The contribution flow MUST offer cash (ACH from a linked account, wire instructions, check instructions) and publicly traded securities (transfer instructions with sponsor custodian DTC details and an intent form: security, quantity, broker). Complex assets show the partner intermediary hand-off only. Contribution detail MUST show distinct Declared, Received (lot by lot), Accepted, Valued, Acknowledged and Final states, and when the amount becomes available to grant.
- **Traces to:** RR-UNB-ACC-006-r1, UNB-RCN-FIN-012, RCD-013, RCD-019
- **Actors:** Donor, Advisor (scoped)
- **Forma components:** `choice-group`, `form`, `form-row`, `text-field`, `numeric-stepper`, `identifier`, `callout`, `steps`, `status-lozenge`, `data-grid`, `key-value-list`, `timeline`, `dialog`
- **States:** Declared; Partially received; Received; Accepted; Valued; Acknowledged; Final; Returned (ACH); Rejected
- **Actions:** Declare contribution (A1); Link bank account (via rail partner); Download acknowledgment
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-02, FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-09
- **Acceptance criteria:**
  - **AC-1** Given 1,000 shares declared and 600 received; when the donor opens the contribution; then it shows 'Partially received: 600 of 1,000 shares' and no acknowledgment.
  - **AC-2** Given an ACH contribution inside the return window; when the donor views Available to grant; then the amount appears under Pending with the date it becomes available.

### UNB-UI-S-006 — Participants, delegation and successors (screen)
- **Requirement:** The people screen MUST list each participant with role (from the single role catalogue), scope, effective dates and status, and support invite, change scope, revoke (immediate, with G-005 confirmation) and designate successors with percentages that must total 100%. The successor plan MUST show its text equivalent, not only a diagram.
- **Traces to:** RR-UNB-PART-003-r1, RR-UNB-PART-007-r1, RR-UNB-SUCC-008-r1, UNB-RCN-ENT-010
- **Actors:** Donor (primary), Sponsor staff
- **Forma components:** `data-grid`, `status-lozenge`, `dialog`, `form`, `select`, `allocation`, `hierarchy-tree`, `validation-summary`
- **States:** Invited; Active; Revoked; Suspended (verified death); Successor designated; Successor active
- **Actions:** Invite participant; Change scope; Revoke access (A2, immediate); Designate successors
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-12, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given successor allocations of 60% and 30%; when the donor saves; then validation blocks saving and states the 10% remainder.

### UNB-UI-S-007 — Document center (screen)
- **Requirement:** The document center MUST list issued documents (acknowledgments, statements, grant confirmations) with type, period, issue date, numeric reference and verification status. Each opens the accessible HTML rendition, with a PDF download, from the same snapshot. Corrected documents MUST show 'Superseded by' links. Filters: type and year.
- **Traces to:** REQ-X-035, RCD-009, UNB-RCN-PLT-010
- **Actors:** Donor, Advisor, Sponsor staff
- **Forma components:** `data-grid`, `collection-toolbar`, `select`, `identifier`, `status-lozenge`, `empty-state`, `pagination`
- **Actions:** Open (HTML); Download PDF; Verify document
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given an acknowledgment corrected once; when the donor lists documents; then the original shows 'Superseded by <ref>' and both remain downloadable.

### UNB-UI-S-008 — Staff exception inbox (screen)
- **Requirement:** A single inbox of rule-created work items (RR-REQ-X-048-r1). Each row shows creating rule, subject, age against SLA, owner and next action, sorted by SLA risk. Filters: rule family and owner, plus sponsor only for operators who serve more than one sponsor. An item the viewer proposed shows 'Waiting for approval' and no approve action (segregation of duties). Routine work never appears. Items close automatically when their exit condition becomes true. A summary row shows straight-through rate and open items by age (UNB-RCN-PLT-006).
- **Traces to:** RR-REQ-X-048-r1, UNB-RCN-PLT-006, RCD-026
- **Actors:** Sponsor operations, Compliance, Finance
- **Forma components:** `workspace-shell`, `work-queue`, `data-grid`, `collection-toolbar`, `active-filter-summary`, `status-lozenge`, `metric-card`, `empty-state`, `pagination`
- **States:** Open; Assigned; Waiting on external party; Resolved (auto-closed); Breaching SLA
- **Actions:** Open case; Assign to me; Reassign (lead only)
- **Responsive:** phone-review-and-approve
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given an eligibility hold resolved by a fresh IRS observation; when the observation arrives; then the work item closes automatically with reason 'Exit condition met'.

### UNB-UI-S-009 — Case workspace (screen)
- **Requirement:** The case workspace MUST show, for one exception: the facts (typed evidence with source and freshness); conflicting sources side by side; the rule that raised it; permitted actions for the user's authority (A0 to A4) with consequences; an AI summary (G-011); and the decision record. Approve, Release or Disposition actions MUST be disabled for the originator or preparer, with the reason shown (segregation of duties).
- **Traces to:** RR-AIN-003-r1, RR-UNB-AI-STD-003-r1, UNB-RCN-GRT-007, REQ-X-017
- **Actors:** Compliance reviewer, Grants approver
- **Forma components:** `record-header`, `facts`, `evidence-relationship`, `conflict-review`, `provenance-trail`, `obligation-panel`, `readiness-checklist`, `dialog`, `textarea`, `flyout`, `conversation`, `handoff-summary`, `split-pane`
- **States:** Open; Awaiting evidence; Decided; Escalated
- **Actions:** Request evidence (A1); Record decision: false positive / true match (A4); Approve grant (A4 or A3 per RCD-005); Escalate
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-11, FORMA-GAP-13, FORMA-GAP-15, FORMA-GAP-16, FORMA-GAP-17
- **Acceptance criteria:**
  - **AC-1** Given a reviewer who prepared the case; when they open it; then Record decision is disabled with the text 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a potential sanctions match; when the AI summary recommends release; then no release control appears in the AI panel; the human decision dialog requires a written rationale.

### UNB-UI-S-010 — Sponsor policy configuration and approval (screen)
- **Requirement:** Policy changes MUST be drafted, previewed as a diff against the active version with an impact preview (for example 'affects 214 open recurring schedules'), and approved by an authorized approver other than the author before publication with an effective date. Validation from the assistance contracts runs as the user edits.
- **Traces to:** RR-REQ-X-051-r1, REQ-X-045, ADM-027, UNB-RCN-PLT-005
- **Actors:** Sponsor administrator, Sponsor approver
- **Forma components:** `form`, `form-row`, `rule-builder`, `diff-viewer`, `comparison-pairs`, `readiness-checklist`, `dialog`, `date-time-field`, `validation-summary`, `flyout`
- **States:** Draft; In review; Approved (scheduled); Active; Superseded
- **Actions:** Save draft; Submit for approval; Approve and schedule (A4); Reject
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-02, FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given an author; when they open their own draft in review; then Approve is unavailable with the reason shown.

### UNB-UI-S-011 — Sponsor overview (screen)
- **Requirement:** A role-composed overview of outcomes at risk, without a composite health score: holds by reason and age; straight-through rate by workflow; reconciliation status by account; evidence freshness; statement publication status. Release 1 MAY use one trend chart per view only if it meets UNB-UI-DS-018 (title, labelled axes, value labels, text summary, keyboard-reachable data table); otherwise tables and metric cards (FORMA-GAP-05). No total is shown as reconciled while any account is stale.
- **Traces to:** REQ-X-043, UX-S01, RCD-026, UNB-RCN-PLT-006
- **Actors:** Sponsor executive, Operations lead
- **Forma components:** `dashboard-grid`, `metric-card`, `metric-value-change`, `data-grid`, `status-lozenge`, `freshness`
- **Responsive:** phone-review-and-approve
- **Capability gaps:** FORMA-GAP-05, FORMA-GAP-06, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given reconciliation evidence for one account is 2 days stale; when the overview renders; then that account shows 'Not current' and no green total is shown.

### UNB-UI-S-012 — Charity view (screen)
- **Requirement:** Charities MUST see grants addressed to them (sponsor as legal donor; donor name only where the donor chose recognition), remittance details, payment status and their verified payment destinations. Destination changes MUST follow the DestinationEvidence process, without self-service activation.
- **Traces to:** REQ-X-058, UNB-RCN-GRT-011, UNB-RCN-GRT-017, RR-REQ-X-005-r1, UNB-RCN-GRT-015, RCD-045
- **Actors:** Charity finance user
- **Forma components:** `data-grid`, `status-lozenge`, `identifier`, `key-value-list`, `form`, `callout`, `timeline`
- **Actions:** Decline this grant (before release; reason required; RCD-045); Request destination change; Download remittance
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given an anonymous grant; when the charity views it; then the donor field reads 'Anonymous donor via <sponsor>'.
  - **AC-2** Given a destination change request; when submitted; then it shows 'Pending independent verification' and payments continue to the previous verified destination.
  - **AC-3** Given an approved grant not yet released; when the charity declines it with a reason; then the grant shows 'Declined by charity', no payment is released, and the funds return to the originating fund or its trailing destination (RCD-040).

### UNB-UI-S-013 — Notification preferences (screen)
- **Requirement:** Users MUST be able to switch optional notification classes on and off per channel. Mandatory-legal and security classes are shown as always on, with the reason.
- **Traces to:** UNB-RCN-PLT-002, AIN-023
- **Actors:** Donor, Advisor
- **Forma components:** `switch`, `form`, `form-row`, `callout`
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given the acknowledgment notice class; when the user views preferences; then its switch is shown on and disabled, with the text 'Required: tax acknowledgment'.

### UNB-UI-D-001 — Contribution acknowledgment (document)
- **Requirement:** The acknowledgment MUST contain: sponsor letterhead and legal name and EIN; donor name; contribution reference; date of gift; cash amount, or a description of noncash property with no value; the goods-or-services statement; the exclusive-legal-control sentence verbatim; and the document reference with verification code. It is produced as an accessible HTML rendition and a PDF from one snapshot (RCD-009).
- **Traces to:** RR-UNB-ACC-006-r1, UNB-RCN-FIN-001, RCD-019
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-footer`, `ef-print-section`, `ef-print-table`, `ef-print-note`, `ef-print-page-number`
- **Capability gaps:** FOLIO-GAP-01, FOLIO-GAP-02, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-07
- **Acceptance criteria:**
  - **AC-1** Given a securities contribution of 600 shares of XYZ; when the acknowledgment renders; then it describes '600 shares of XYZ Corp. common stock' and states no value.

### UNB-UI-D-002 — Fund statement (document)
- **Requirement:** The statement MUST contain: a period summary (opening value, contributions, grants paid, fees, investment change, closing value; available to grant at period end); contributions, grants and fees tables that repeat column headers across pages; pending grants; the fee basis; disclosures and the legal-sponsor statement; page x of y. Generated from the locked period snapshot.
- **Traces to:** REQ-X-032, RCD-009, RCD-018, RCD-020
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-header`, `ef-print-footer`, `ef-print-page-number`, `ef-print-section`, `ef-print-metric`, `ef-print-table`, `ef-print-keep`, `ef-print-note`, `ef-print-toc`
- **Capability gaps:** FOLIO-GAP-03, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-08, FOLIO-GAP-09, FOLIO-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a statement with 70 grant rows; when rendered to PDF; then the grants table spans pages with its header row repeated, and every page shows 'Page n of N'.

### UNB-UI-D-003 — Grant confirmation and remittance advice (document)
- **Requirement:** The donor confirmation and the charity remittance MUST state the sponsor as the legal donor, the grant reference, amount, payment method and date, the purpose, and recognition per the donor's choice. Remittance MUST contain no pledge reference (UNB-RCN-GRT-011). One remittance may list several grants paid together.
- **Traces to:** UNB-RCN-GRT-011, RCD-016
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-footer`, `ef-print-section`, `ef-print-table`, `ef-print-note`
- **Capability gaps:** FOLIO-GAP-01, FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-06, FOLIO-GAP-07
- **Acceptance criteria:**
  - **AC-1** Given a grant whose donor recorded a pledge ID internally; when the remittance renders; then no pledge reference appears anywhere in the document or its metadata.

### UNB-UI-D-004 — Annual DAF reporting package (Schedule D Part I support) (document)
- **Requirement:** The package MUST contain, per legal sponsor and tax year: the number of DAFs, aggregate value at year end, aggregate contributions and aggregate grants, each with a reconciliation to the ledger, the classification register version and preparer and signer fields.
- **Traces to:** UNB-RCN-ENT-012, RR-REQ-X-060-r1, RCD-024
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-section`, `ef-print-metric`, `ef-print-table`, `ef-print-integrity`, `ef-print-note`
- **Capability gaps:** FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given a locked tax year; when the package renders twice; then both renditions carry identical figures and the same source snapshot hash.

### UNB-UI-D-005 — Case decision record (document)
- **Requirement:** The decision record MUST capture the case reference, rule that raised it, evidence list with sources and freshness, decision, rationale, decider and preparer identities, and timestamps, for audit evidence packages.
- **Traces to:** RR-AIN-003-r1, UNB-RCN-REG-003
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-section`, `ef-print-table`, `ef-print-finding`, `ef-print-callout`, `ef-print-footer`
- **Capability gaps:** FOLIO-GAP-04, FOLIO-GAP-06, FOLIO-GAP-11, FOLIO-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given a confirmed sanctions match; when the decision record renders; then it lists preparer and decider as different people and includes the OFAC report due date.

### UNB-UI-D-006 — Audit evidence package (document)
- **Requirement:** The audit evidence package MUST be a period-bounded export for an auditor: a control list, evidence items with hashes, exceptions and their resolution, and a table of contents.
- **Traces to:** SOC2-009, RCD-031
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-toc`, `ef-print-section`, `ef-print-table`, `ef-print-integrity`, `ef-print-page-number`
- **Capability gaps:** FOLIO-GAP-04, FOLIO-GAP-05, FOLIO-GAP-10
- **Acceptance criteria:**
  - **AC-1** Given an evidence package for Q4; when rendered; then every evidence row shows its SHA-256 and the table of contents lists each control section.

### UNB-UI-W-001 — Embedded grant status widget (partner host) (widget)
- **Requirement:** Partner hosts MAY embed a read-only grant-status card in an iframe served from the sponsor's Unbound origin, authorized by a short-lived, sponsor-scoped token issued server-to-server. It shows headline status, amount and charity, and offers no consequential actions in release 1 (DAF checkout deferred, CAP-03). The card carries the sponsor brand and the legal-sponsor sentence inside the frame, stays readable at 320-600 px frame widths, reports its content height to the host by a postMessage limited to the registered host origin (UNB-GAP-W-01), and on token expiry clears all grant data and shows a safe message.
- **Traces to:** INT-015, REQ-X-063, RCD-011, RCD-013
- **Forma components:** `surface`, `status-lozenge`, `key-value-list`, `identifier`
- **Capability gaps:** UNB-GAP-W-01, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given an expired token; when the widget loads; then it shows 'Sign in again on your provider's site' and no grant data.

### UNB-UI-DS-001 — Token source and brand layering (design-system)
- **Requirement:** All colour, type family, radius and focus values in Unbound UI MUST come from Forma semantic tokens (--forma-color-*, --forma-type-*, --forma-space-*, --forma-primitive-radius-*, --forma-primitive-size-target-min) as set by a brand manifest compiled with Forma's BrandCompiler. Application CSS MAY derive values (color-mix) from those tokens but MUST NOT declare --forma-* properties or hard-code hex colours. The Unbound default brand ('unbound-default') MUST use the Balanced Charcoal and Bronze values of the Echelon theme 1.1.0 manifest (kemiller2002/echelon-design-system theme/echelon.brand.json): charcoal #202421 chrome/heading, carbon #171a18 text, parchment #f2efe7 canvas, oxide bronze #905831 accent (#a87e5e dark), with Newsreader, Manrope and IBM Plex Mono self-hosted under SIL OFL 1.1. The README candidate palette (#20252A, #B7844B, ...) MUST NOT be used: it is unverified.
- **Traces to:** INT-019, UNB-EXP-002, UNB-UI-G-014, RCD-050
- **Forma components:** `app-identity`
- **Capability gaps:** FORMA-GAP-20
- **Acceptance criteria:**
  - **AC-1** Given the v2 prototype CSS; when it is scanned for '--forma-' declarations and hex colour literals outside documented derivations; then there are none; every colour resolves from a --forma-color-* token.
  - **AC-2** Given the unbound-default brand manifest; when it is compiled with Forma's BrandCompiler; then compilation passes and the output values equal echelon.brand.json for every colour role.
  - **AC-3** Given a sponsor manifest (example-harbor); when it is swapped in without other changes; then every v2 screen restyles, and the verification harness passes at all five viewports.

### UNB-UI-DS-002 — Application type scale (design-system)
- **Requirement:** Application screens MUST use a fixed application type scale, not Forma's marketing display sizes (fluid-display-1 up to 7.25rem with a 14ch heading measure): page title clamp(1.5rem, 1.3rem + 0.8vw, 1.875rem) weight 650; section title 1.125rem; body 1rem/1.5; small 0.875rem; micro 0.8125rem (minimum for any text). Headings MUST NOT have a character measure that wraps short titles. The display face (Newsreader in the default brand) is reserved for the one headline money figure per screen; all other text uses the sans face. Labels and chips use sentence case in the body face; uppercase monospace labels MUST NOT be used for status or navigation.
- **Traces to:** INT-019, UNB-UX-007, UX-S02
- **Forma components:** `text-roles`, `section-heading`
- **Capability gaps:** FORMA-GAP-18
- **Acceptance criteria:**
  - **AC-1** Given any v2 page at 1280 px; when computed font sizes of all visible text are collected; then no text is smaller than 13 px and the h1 is between 24 and 30 px.
  - **AC-2** Given the page title 'Lindqvist Family Fund'; when it renders at 320, 390 and 1280 px; then it occupies one line at 390 and 1280 px.

### UNB-UI-DS-003 — Spacing and density (design-system)
- **Requirement:** Spacing MUST use Forma's eight-step scale (--forma-space-1..8 = 0.25, 0.5, 0.75, 1, 1.25, 1.5, 2, 3 rem): 4 within controls, 12-16 between related items, 20-24 between panels, 32-48 between page regions. Two densities are defined: comfortable (donor, advisor, charity, widget; table rows at least 44 px) and compact (staff work queues and ledgers; rows at least 36 px with 24 px targets). Density MUST be a single switch per surface class, not per component; Forma has no density token (FORMA-GAP-18).
- **Traces to:** INT-009, INT-019
- **Forma components:** `stack`, `cluster`
- **Capability gaps:** FORMA-GAP-18
- **Acceptance criteria:**
  - **AC-1** Given the giving home at 1280 px; when gaps between panels are measured; then every gap equals 20 px (space-5).
  - **AC-2** Given a staff queue in compact density; when row heights and target sizes are measured; then rows are at least 36 px tall and every interactive target is at least 24 by 24 px.

### UNB-UI-DS-004 — Colour roles and semantic status (design-system)
- **Requirement:** Colour roles are: canvas (surface-primary), raised panel (derived, lighter than canvas), sunken (derived), chrome (surface-inverse) with text-inverse, accent (accent-primary) for the single primary action, the active navigation rule and the 'available' allocation segment only, and four status roles (success, warning, danger, info). Status colour MUST always be paired with a glyph and words. Accent MUST NOT be used for status. Secondary text (text-secondary) MUST NOT be placed on surface-secondary or sunken surfaces (Echelon AUDIT §6: it fails 4.5:1 there); use text-on-secondary-surface.
- **Traces to:** INT-019, ADV-928, ADV-692
- **Forma components:** `status-lozenge`
- **Capability gaps:** FORMA-GAP-20
- **Acceptance criteria:**
  - **AC-1** Given every v2 page; when axe color-contrast runs at all five viewports; then there are no violations.
  - **AC-2** Given a page rendered with a greyscale filter; when a reviewer identifies each status; then every status is identifiable from its glyph and words.

### UNB-UI-DS-005 — Interaction states (design-system)
- **Requirement:** Every interactive component MUST define rest, hover, focus-visible, active, disabled-with-reason, busy and error states. Focus-visible is a 3 px focus-ring outline with 2 px offset plus a 2 px focus-gap halo, so it is visible on canvas, raised and chrome surfaces. Disabled controls that the user might expect to use MUST show why (FORMA-GAP-11); controls the user can never use MUST be omitted. Busy state uses aria-busy on the control and keeps its label.
- **Traces to:** UNB-UX-002, UNB-UI-G-008
- **Forma components:** `button`
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a keyboard user tabbing through any v2 page; when each focus stop is inspected; then each has an outline of at least 2 px or a box-shadow ring, and none is covered by a fixed bar.
  - **AC-2** Given a donor in a restricted fund; when the home renders; then 'Recommend a grant' is absent from page actions and the notice says why recommending is paused and what still works.

### UNB-UI-DS-006 — Buttons and action emphasis (design-system)
- **Requirement:** Each region MUST have at most one primary (filled accent) action; other actions are secondary (outlined) or quiet (underlined text with chevron for navigation). Button labels are verbs naming the outcome ('Recommend a grant', 'Review and confirm'), never 'OK', 'Submit' alone or 'Click here'. Buttons are at least 44 px tall in comfortable density. Forma 1.0.2 styles data-forma-variant=primary only inside the marketing .forma-site frame, so the application primary variant is a Forma gap (forma#186).
- **Traces to:** INT-013, UNB-UI-G-005
- **Forma components:** `button`
- **Capability gaps:** FORMA-GAP-01
- **Acceptance criteria:**
  - **AC-1** Given any v2 page; when primary-variant buttons are counted per panel and page header; then no region has more than one.
  - **AC-2** Given the giving home; when button heights are measured at 390 px; then every button is at least 44 px tall.

### UNB-UI-DS-007 — Panels and grouping (design-system)
- **Requirement:** Related content is grouped in raised panels with a header (title, optional count or total), body and optional footer for navigation links. Panels MUST NOT nest. A panel needing attention uses a 4 px warning rule on its leading edge plus a text title ('Needs your attention'); colour alone MUST NOT signal it.
- **Traces to:** INT-019, UX-S02
- **Forma components:** `surface`, `section`
- **Capability gaps:** FORMA-GAP-20
- **Acceptance criteria:**
  - **AC-1** Given the giving home; when panels are inspected; then no panel contains another panel and each panel has an accessible name.

### UNB-UI-DS-008 — Status chip (design-system)
- **Requirement:** Status MUST render as a sentence-case chip: status glyph from the Forma icon registry (success, pending, error, info, help-circle), the status words from the status catalog, 1 px border in the status colour, body face at 13 px semibold. This replaces Forma's uppercase monospace status-lozenge in donor, advisor and charity surfaces (it read as developer output in v1). Staff dense ledgers MAY keep status-lozenge.
- **Traces to:** UNB-UX-003, UNB-UI-G-001, UNB-UX-007
- **Forma components:** `status-lozenge`, `icon`
- **Capability gaps:** FORMA-GAP-21
- **Acceptance criteria:**
  - **AC-1** Given the grant 'On hold' chip; when it is rendered in forced-colours mode; then the glyph, border and words remain visible.
  - **AC-2** Given any chip; when its text is read by a screen reader; then the words alone state the status.

### UNB-UI-DS-009 — Task list (things that need the user) (design-system)
- **Requirement:** Items that need the user MUST be presented as a task list: icon in a status-coloured ring, a title that names the subject in plain words, one sentence of reason and consequence, and exactly one primary next action (secondary actions allowed). Informational items with no user action MUST NOT appear in the task list; they belong in activity or record status.
- **Traces to:** UNB-UI-S-001, UX-S02, ADV-925
- **Forma components:** `work-queue`, `priority-stack`
- **Capability gaps:** FORMA-GAP-22
- **Acceptance criteria:**
  - **AC-1** Given the giving home with a certification due and a held grant; when the attention panel renders; then each item has a title, a reason sentence and one named action, and the panel precedes grants in progress and activity in source order.

### UNB-UI-DS-010 — Notices for restrictions, staleness and outages (design-system)
- **Requirement:** Account-level conditions (restricted, suspended, succession, stale balance, integration unavailable) MUST use one notice pattern at the top of the content: leading glyph, a title stating the condition in plain words, a body saying why, since when, what still works and what to expect, and at most two actions. The same root cause MUST produce one notice, not several (ADV-921).
- **Traces to:** INT-012, UNB-UX-004, ADV-921, ADV-946
- **Forma components:** `callout`, `fault-banner`
- **Acceptance criteria:**
  - **AC-1** Given a fund whose pricing integration is down; when the home renders; then one notice titled 'Investment values are temporarily unavailable' states that Available to grant is unaffected.
  - **AC-2** Given a suspended fund; when the home renders; then the notice gives the date, the reason and the action that lifts the suspension.

### UNB-UI-DS-011 — Iconography (design-system)
- **Requirement:** Icons MUST come from the Forma icon registry (80 icons, 24 px grid, 1.8 stroke, round caps), rendered at 16, 20 or 24 px, decorative (aria-hidden) beside visible text, or named by the parent control's aria-label when standalone. Domain concepts with no registry icon (grant, contribution, charity, bank) use the closest registry icon ('send' for grant, 'add' for add money) until FORMA-GAP-23 adds domain glyphs. Icons MUST NOT be the only carrier of meaning.
- **Traces to:** INT-019, ADV-960
- **Forma components:** `icon`
- **Capability gaps:** FORMA-GAP-23
- **Acceptance criteria:**
  - **AC-1** Given the v2 build; when every icon name is resolved; then all resolve in the Forma registry (the helper throws otherwise).

### UNB-UI-DS-012 — Sponsor white-labelling (design-system)
- **Requirement:** A sponsor brand MUST be a Forma brand manifest (identity, fonts, radius, light and dark colour maps, terms) compiled with Forma's BrandCompiler and passing its contrast checks. Every application-layer component MUST be brand-agnostic (derived from --forma-* tokens, no brand selector), so a sponsor brand restyles the whole product. The Echelon theme's application layer (echelon-app.css: app bar, nav tabs, panels, chips) is scoped to data-forma-brand='echelon' and MUST NOT be used for Unbound, because it would not apply to sponsor brands. Sponsor terms (Fund, Donor, Grant) resolve through the terminology catalog (G-007); protected legal text cannot be re-branded.
- **Traces to:** UNB-UI-G-014, UNB-UI-G-007, INT-019, ADV-716
- **Forma components:** `app-identity`
- **Capability gaps:** FORMA-GAP-20
- **Acceptance criteria:**
  - **AC-1** Given the giving home under example-harbor; when it is compared with unbound-default; then only colours, fonts, radius, mark and brand name differ; structure, text and the legal disclosure are identical.
  - **AC-2** Given a sponsor manifest whose accent fails 4.5:1 on its canvas; when it is compiled; then compilation fails with a contrast finding and it is not deployable.

### UNB-UI-DS-013 — Empty and onboarding states (design-system)
- **Requirement:** An empty state MUST say what will appear, why it is empty and offer the single next sensible action. A new fund MUST show an ordered getting-started checklist (open fund, first contribution, first grant) with done/current/todo states instead of separate empty panels and a $0.00 card. Empty compliance or work views MUST state what was checked and when ('Nothing needs you. Last checked 9:12 AM'), never imply 'no issues' without scope (ADV-946).
- **Traces to:** UNB-UI-G-013, UX-S09, ADV-946
- **Forma components:** `empty-state`, `readiness-checklist`
- **Acceptance criteria:**
  - **AC-1** Given a fund opened today with no contributions; when the home renders; then a 'Get your fund started' checklist shows 1 of 3 done with 'Add money' as the only primary action.

### UNB-UI-DS-014 — Skeleton loading and layout stability (design-system)
- **Requirement:** Loading states MUST render skeletons with the geometry of the final layout (same panels, headings and row count), keep real headings visible, set aria-busy on the loading region with a polite status message, and stop animating under prefers-reduced-motion. Content arrival MUST NOT shift layout above the fold by more than 0.1 CLS. No blank shell (INT-011).
- **Traces to:** INT-011, UNB-UI-G-013, UI-T20
- **Forma components:** `skeleton`
- **Acceptance criteria:**
  - **AC-1** Given the home loading state and the normal state at 1280 px; when panel bounding boxes are compared; then panel positions differ by no more than 24 px vertically.
  - **AC-2** Given reduced motion requested; when the loading state renders; then skeletons do not animate.

### UNB-UI-DS-015 — Dialogs (design-system)
- **Requirement:** Dialogs are reserved for consequential confirmation (G-005), unknown-outcome resolution and destructive actions. A dialog MUST be a native modal dialog with a title naming the action, the facts that will change, reversibility, who acts next and timing, a confirm control naming the action and a cancel control; focus moves to the dialog title and returns to the invoking control. One confirmation per action: no 'Are you sure?' after a review step.
- **Traces to:** INT-013, UNB-UI-G-005, ADV-906, ADV-944
- **Forma components:** `dialog`
- **Capability gaps:** FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a grant recommendation reviewed on a review step; when the donor presses 'Submit grant recommendation'; then no second confirmation dialog opens.

### UNB-UI-DS-016 — Notifications and task resolution (design-system)
- **Requirement:** The bell shows the count of unread notifications in its accessible name. A notification that asks for action links to the task and disappears from 'needs attention' only when the underlying task is resolved, never on dismissal (ADV-923). Snooze MUST NOT outlive a legal deadline (ADV-924). Push and email previews contain no amounts or charity names (INT-017).
- **Traces to:** INT-017, UNB-UI-G-015, ADV-922, ADV-923, ADV-924, ADV-948
- **Forma components:** `feed-item`
- **Acceptance criteria:**
  - **AC-1** Given a certification task that the donor dismisses from notifications; when the home reloads; then the task is still listed under 'Needs your attention'.

### UNB-UI-DS-017 — Forms and validation (design-system)
- **Requirement:** Forms are single-column with labels above fields, help text tied by aria-describedby, required fields marked in words, inline validation on blur or submit (never while typing a number), an error summary at the top linking to each field, and entered values preserved. Amount fields accept '$', commas and spaces, use inputmode=decimal and echo the parsed amount in words beside the field. Redundant entry is avoided (WCAG 3.3.7).
- **Traces to:** UNB-UX-002, UNB-UI-G-012, UXA-2, TVR-UX-003
- **Forma components:** `text-field`, `validation-summary`, `validation-message`, `form`
- **Capability gaps:** FORMA-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given an amount above Available to grant; when the donor submits; then the error summary links to the amount field and the message states the available amount and how to proceed.

### UNB-UI-DS-018 — Charts and reporting (design-system)
- **Requirement:** Charts MUST have a visible title, labelled axes with units, direct value labels or a data table equivalent reachable by keyboard, a text summary of the main finding, series distinguishable by pattern or label as well as colour, and freshness. A chart MUST NOT be the only presentation of a financial figure. Forma has no chart primitives (FORMA-GAP-05).
- **Traces to:** INT-010, ADV-692, ADV-693, ADV-721
- **Forma components:** `disclosure`
- **Capability gaps:** FORMA-GAP-05
- **Acceptance criteria:**
  - **AC-1** Given the sponsor overview chart; when a screen reader user navigates it; then a summary and a data table with every plotted value are available.

### UNB-UI-N-001 — Application frame (navigation)
- **Requirement:** Every authenticated surface uses one application frame: skip link; chrome app bar with sponsor brand mark and name, product context, Help, Notifications (with unread count) and account menu; primary navigation; main region; legal-sponsor footer. The marketing site frame (.forma-site) MUST NOT be used for application screens (v1 did, which produced marketing-scale headings).
- **Traces to:** INT-001, UNB-UI-G-014, UX-S02
- **Forma components:** `skip-link`, `app-identity`, `navigation-shell`
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given any v2 page; when landmarks are listed; then there is exactly one main, one banner, one visible primary navigation and a footer with the legal disclosure.

### UNB-UI-N-002 — Primary destinations by audience (navigation)
- **Requirement:** Donor and advisor: Home, Grants, Contributions, Documents, People (Notifications and Settings in the account menu). Staff: Work queue, Cases, Reconciliation, Funds, Policy. Charity: Grants received, Organization. Destinations are places named by nouns in the sponsor's terms, never actions ('Add money' is a page action, not a destination); no more than five primary destinations per audience.
- **Traces to:** UNB-UX-001, UNB-UI-G-007
- **Forma components:** `navigation-shell`
- **Acceptance criteria:**
  - **AC-1** Given each audience's frame; when primary destinations are counted; then there are at most five, each with an icon and visible text.

### UNB-UI-N-003 — Responsive navigation pattern (navigation)
- **Requirement:** From 64rem: persistent side navigation (15.5rem) with the current page marked by an accent rule, weight and aria-current. Below 64rem: a fixed bottom tab bar with at most four destinations, visible labels and 56 px tall targets, plus a Menu button for the rest. Page scroll-padding MUST keep focused elements clear of the tab bar (WCAG 2.4.11). No sticky action bar is stacked on top of the tab bar.
- **Traces to:** INT-009, UNB-UX-001, ADV-930
- **Forma components:** `navigation-shell`
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given the home at 390 px; when fixed bars are measured; then fixed chrome occupies at most 10% of the 844 px viewport height.
  - **AC-2** Given a keyboard walk at 390 px; when each focused element is hit-tested at its centre; then no focused element is covered by the tab bar.

### UNB-UI-N-004 — Scope and fund context (navigation)
- **Requirement:** The current fund is the page title and doubles as the fund switcher (button with aria-haspopup) when the user has more than one fund. The sponsor is identified by the brand in the app bar and the legal footer. Internal references (F-2026-000418) are not shown in donor summaries; they appear in fund details and on documents. Staff surfaces keep the full scope trail (sponsor, program, fund or case) because staff switch scopes.
- **Traces to:** INT-001, UNB-UI-G-010, UNB-UX-007
- **Forma components:** `scope-trail`
- **Acceptance criteria:**
  - **AC-1** Given a donor with two funds; when they open the fund switcher and pick the other fund; then all data reloads under the new fund after re-authorization (G-010).
  - **AC-2** Given the donor home; when visible text is scanned; then no internal fund or rule identifier appears.

### UNB-UI-N-005 — Page header (navigation)
- **Requirement:** Each page has one header: optional context line, h1 naming the subject (fund, grant, case), one line of role or status context, and the page's actions (at most one primary). On phones the actions sit full width under the title, not in a sticky bar.
- **Traces to:** UX-S02, INT-002
- **Forma components:** `record-header`, `section-heading`
- **Acceptance criteria:**
  - **AC-1** Given any v2 page; when headings are listed; then there is exactly one h1 and no heading level is skipped.

### UNB-UI-N-006 — Information architecture of the donor home (navigation)
- **Requirement:** The donor home answers in order: what can I give (Available to grant with breakdown), what needs me (attention tasks), what is in flight (grants in progress), what happened (recent activity). Source order follows that priority at every width; on wide screens the balance takes the main column and attention sits beside it. On a 390x844 phone the headline figure and the 'Needs your attention' heading MUST both be visible in the first viewport above the tab bar; breakdown explanations move behind 'How we calculate this' on phones. Market value is one line of the breakdown, not a peer card. Recent activity and grants in progress show at most three items with a link to the full list.
- **Traces to:** UX-S02, UNB-UI-S-001, UNB-UI-G-004
- **Forma components:** `priority-stack`
- **Acceptance criteria:**
  - **AC-1** Given the home at 390x844 with two attention items; when the first viewport is captured; then the Available to grant figure and the 'Needs your attention' heading are both fully visible above the tab bar.
  - **AC-2** Given the home at 1280 px; when the first viewport is captured; then Available to grant, both attention items and the page actions are visible without scrolling.

### UNB-UI-F-001 — Money never wraps (financial-presentation)
- **Requirement:** A money value (sign, currency symbol, digits, separators, cents) MUST render as one unbreakable token (white-space: nowrap) with tabular lining figures; containers reflow around it. This applies at 320 px and with WCAG 1.4.12 text spacing. v1 broke '$100,000.00' across two lines in three equal metric cards at 1280 px.
- **Traces to:** UNB-UI-G-017, UNB-UX-005
- **Forma components:** `identifier`
- **Capability gaps:** FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given every v2 page at 320, 390, 640, 768 and 1280 px, with and without text-spacing overrides; when each .unb-money element's line boxes are counted; then every money value occupies exactly one line.

### UNB-UI-F-002 — Headline figure (financial-presentation)
- **Requirement:** Each fund screen has exactly one headline figure: Available to grant, in the display face at clamp(2rem, 1.6rem + 1.6vw, 2.75rem) with cents at 62% size raised (still read as one number), followed by its confirmation source and absolute update time. Other amounts use body size.
- **Traces to:** UNB-UI-G-004, UX-S02
- **Forma components:** `metric-card`
- **Capability gaps:** FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given the home; when elements using the display size are counted; then exactly one money value uses it, and its accessible text reads as one amount.

### UNB-UI-F-003 — Signs and negative values (financial-presentation)
- **Requirement:** Outflows show a true minus (U+2212) and a text label naming the movement ('Grant paid'); inflows show '+' only in mixed activity lists. Colour MUST NOT be the only indicator of sign; red is not used for ordinary outflows (a grant is not a loss).
- **Traces to:** UNB-UI-G-017
- **Capability gaps:** FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given recent activity with a grant payment; when it renders; then the amount reads '−$2,500.00' beside the label 'Grant paid', in the normal text colour.

### UNB-UI-F-004 — Tables of money (financial-presentation)
- **Requirement:** Amount columns are right-aligned, tabular, nowrap, with the currency symbol on every value. Date columns use absolute dates. Below a container width of 34rem, tables become two-line records (subject and amount on line one; status and date on line two) without dropping columns; the header row stays available to assistive technology.
- **Traces to:** UNB-UI-G-017, UNB-UI-G-018, INT-009
- **Forma components:** `dense-ledger`, `data-grid`
- **Capability gaps:** FORMA-GAP-03
- **Acceptance criteria:**
  - **AC-1** Given recent activity in the 1280 px side column; when it renders; then it uses the two-line record layout and no cell wraps mid-word.
  - **AC-2** Given recent activity at 1280 px in a wide panel; when amount cells are measured; then their right edges align within 1 px.

### UNB-UI-F-005 — Dates and times (financial-presentation)
- **Requirement:** Dates use the sponsor locale's medium format (en-US: 'Nov 30, 2026'); times add the sponsor time zone abbreviation ('4:58 PM CT'). 'Today'/'Yesterday' MAY replace the date for update times only. Legally significant dates (gift, approval, payment) are always absolute (G-018). Every date is a <time datetime> element.
- **Traces to:** UNB-UI-G-018, RCD-052
- **Acceptance criteria:**
  - **AC-1** Given any v2 page; when time elements are listed; then every visible date is inside a time element with an ISO datetime.

### UNB-UI-F-006 — Freshness wording (financial-presentation)
- **Requirement:** Freshness is stated once per figure in plain words ('Updated today at 4:58 PM CT', 'Market value, Nov 30 close'). When a figure is not current, it shows a 'Not current' chip, the last confirmed time, and a notice explaining what is still safe to do. The words 'Current', 'Calculated' or 'Ledger confirmed' as standalone badges are not used.
- **Traces to:** UNB-UI-G-003, INT-012, ADV-918, ADV-947
- **Forma components:** `freshness`
- **Acceptance criteria:**
  - **AC-1** Given a stale Available to grant; when the home renders at 390 px; then the 'Not current' chip and the last confirmed time are visible in the balance panel.

### UNB-UI-F-007 — Fund composition (financial-presentation)
- **Requirement:** Fund composition is a horizontal proportion bar (Available, Committed, Settling) with a legend list that carries the amounts and plain explanations, ending in a ruled 'Total fund value' row. The bar is decorative (aria-hidden); the list is the accessible source. Segments differ by pattern as well as colour. When fund value is unavailable the bar is omitted and the total row says 'Unavailable'.
- **Traces to:** UNB-UI-G-004, INT-010
- **Forma components:** `allocation`
- **Capability gaps:** FORMA-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given the home in forced-colours mode; when the bar renders; then segments remain distinguishable and the legend carries all amounts.

### UNB-UI-R-001 — Breakpoints and viewports (responsive)
- **Requirement:** Three layout ranges: phone below 48rem, tablet 48-64rem, desktop from 64rem. Every screen MUST be verified at 320 (400% reflow), 390 (phone), 640x450 (1280 at 200% zoom), 768 (tablet) and 1280 (desktop) CSS px, and embedded widgets at 360 and 600 px frame widths. Forma has no breakpoint tokens (FORMA-GAP-19).
- **Traces to:** INT-009, UNB-UX-005, UNB-UI-G-009
- **Capability gaps:** FORMA-GAP-19
- **Acceptance criteria:**
  - **AC-1** Given each v2 page; when the verification harness runs; then results exist for all five viewports and none has horizontal page overflow.

### UNB-UI-R-002 — Container-driven components (responsive)
- **Requirement:** Components that sit in columns of varying width (tables, legends, task lists) MUST adapt to their container width (container queries), not the viewport, so the same component works in a main column, a side column and a phone.
- **Traces to:** INT-009
- **Capability gaps:** FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given the activity table in the 1280 px side column and in a 390 px phone; when both render; then both use the two-line record layout.

### UNB-UI-R-003 — Source order equals priority (responsive)
- **Requirement:** DOM order MUST equal priority order; wider layouts may place regions with grid areas but MUST NOT reorder the reading or focus sequence relative to the narrow layout.
- **Traces to:** INT-009, UNB-UX-002
- **Forma components:** `priority-stack`
- **Acceptance criteria:**
  - **AC-1** Given the home at 390 and 1280 px; when the keyboard focus order is recorded; then the sequence of focusable elements is identical.

### UNB-UI-A-001 — WCAG 2.2 AA automated gate (accessibility)
- **Requirement:** Every screen state MUST pass axe-core with tags wcag2a, wcag2aa, wcag21a, wcag21aa and wcag22aa at the five viewports of UNB-UI-R-001 with zero violations, and MUST have no console errors or failed resource loads.
- **Traces to:** UNB-UX-005, UNB-UX-006, UNB-UI-G-008
- **Acceptance criteria:**
  - **AC-1** Given the v2 prototype set; when tools/verify.mjs runs; then every state passes at every viewport.

### UNB-UI-A-002 — Keyboard and focus (accessibility)
- **Requirement:** Every function is operable by keyboard in a logical order; focus is always visible (2.4.7, 2.4.13 target: 3 px outline) and never obscured by fixed chrome (2.4.11); a skip link reaches main; dialogs trap and return focus; no keyboard traps.
- **Traces to:** UNB-UX-002, UNB-UX-005, ADV-907, ADV-944
- **Forma components:** `skip-link`
- **Acceptance criteria:**
  - **AC-1** Given a Tab walk of each state at each viewport; when each stop is probed; then each stop has a visible indicator and is not covered at its centre.

### UNB-UI-A-003 — Screen-reader semantics (accessibility)
- **Requirement:** One h1 per page, no skipped heading levels, named landmarks with unique labels, every panel named, every status in words, tables with captions and header cells, money read as one amount, live status messages for loading and submission. Manual screen-reader review (NVDA + Firefox, VoiceOver + Safari iOS, TalkBack + Chrome) is required before release; automated checks do not replace it.
- **Traces to:** UNB-UX-002, UNB-UX-006, UXA-1, UI-T21
- **Forma components:** `landmark-region`, `visually-hidden`
- **Acceptance criteria:**
  - **AC-1** Given each v2 page; when headings and navigation landmarks are inspected; then one h1, no skipped levels, and visible nav landmarks have unique labels.

### UNB-UI-A-004 — Targets, reflow, spacing and motion (accessibility)
- **Requirement:** Targets at least 24x24 px (2.5.8) everywhere and 44x44 px for primary controls on phones; content reflows at 320 px without horizontal scroll (1.4.10); text-spacing overrides cause no clipping or overflow (1.4.12); animation stops under prefers-reduced-motion; forced-colours mode keeps borders, focus and status glyphs.
- **Traces to:** UNB-UX-005, UI-T20, ADV-930, ADV-961
- **Acceptance criteria:**
  - **AC-1** Given each v2 page with the WCAG 1.4.12 overrides injected at 320 px; when overflow and clipping are measured; then there is none.

### UNB-UI-A-005 — Accessible authentication and redundant entry (accessibility)
- **Requirement:** Sign-in and step-up MUST offer a method that needs no cognitive test (passkey; WCAG 3.3.8), and multi-step grant and contribution flows MUST not ask again for information already given (3.3.7).
- **Traces to:** UXA-2, UXA-3
- **Acceptance criteria:**
  - **AC-1** Given a grant recommendation reviewed on a review step; when the donor goes back to edit the amount; then all other fields keep their values.

### UNB-UI-V-001 — Reference images and comparison (visual-regression)
- **Requirement:** Each screen state has committed reference images at 390 and 1280 px (requirements/ui/designs/v2/rendered/<screen>/<state>-<width>.png) produced by tools/verify.mjs with deviceScaleFactor 1, reduced motion and fonts loaded. A visual change is accepted only by replacing the reference in a reviewed change; comparisons use a per-pixel threshold of 0.1 and fail above 0.5% changed pixels.
- **Traces to:** UNB-UX-005, INT-020, ADV-700, ADV-701
- **Acceptance criteria:**
  - **AC-1** Given a CSS change that moves the balance panel 8 px; when the comparison runs against references; then it fails and reports the states and widths affected.

### UNB-UI-V-002 — Scenario coverage matrix (visual-regression)
- **Requirement:** The reference set MUST cover these scenarios for the donor experience: normal, loading, no contributions, no grants, pending grants, attention required, stale balances, integration unavailable, restricted fund, suspended fund, succession pending, read-only permissions, failed payment, mobile viewport, white-labelled sponsor; plus reconciliation exception (staff) and embedded widget (partner host). data/ui-state-matrix.csv lists the state that covers each.
- **Traces to:** UNB-UX-001, UNB-UX-003, INT-015
- **Acceptance criteria:**
  - **AC-1** Given data/ui-state-matrix.csv; when it is checked against the 17 scenarios; then every scenario has at least one state with reference images and a passing verification.

### UNB-UI-S-014 — Case workspace frame (typed panels) (screen)
- **Requirement:** UNB-UI-S-009 MUST be implemented as one frame with these regions in order: (1) Case header - work-item reference (G-016), subject reference and subject status lozenge, work-item status, owner, preparer, age against SLA, and the blocked outcome; (2) Rule that raised it - creating rule id and version, the condition that fired, and the exit condition that will close the item automatically; (3) Evidence region - typed evidence from the selected panel, each fact with source and freshness (G-003), conflicting sources side by side, and an evidence snapshot id; (4) Panel slot - exactly one typed decision panel (UNB-UI-P-*); (5) Decision record - proposed and recorded decisions with decider, authority, rationale, evidence snapshot and resulting subject transition, append-only; (6) AI summary flyout - opened by 'Explain', labelled 'AI-generated, not a decision' with sources, with no consequential controls (G-011); (7) Handoff - a handoff summary of what was done, what is pending and who acts next, plus reassignment. The panel MUST be selected deterministically from the work-item type, defined as (creating rule family, subject aggregate and workflow id, subject's current state), through a versioned panel registry; when the subject's state changes, the panel MUST re-resolve. If no panel is registered for a type, the slot MUST show 'No decision panel for this case type' and offer only Request evidence and Escalate; it MUST NOT fall back to a generic approve control. Every panel inherits from the frame: segregation of duties (decision controls disabled with a visible reason for the preparer, the proposer, the originator of the subject action and any user the subject transition's guards exclude, WFM-23-T11 decider-not-preparer); authority revalidation at submit, so a revocation within 60 seconds (RCD-041) rejects the decision; the G-005 confirmation naming the subject transition, its reversibility and who acts next; and G-006 unknown-outcome handling (no automatic retry). Recording a decision writes the WFM-23 decision record and issues the panel's subject transition; if execution fails the item shows 'Reopened' with the failure reason. Request evidence and Escalate are frame actions available in every panel; the frame MUST NOT offer bulk decisions across cases except the RCD-005 batch grant approval, which lives in the inbox (UNB-UI-S-008).
- **Traces to:** UNB-UI-S-009, UNB-UI-S-008, WFM-23-T03, WFM-23-T04, WFM-23-T08, WFM-23-T09, WFM-23-T10, WFM-23-T11, WFM-23-T13, UNB-RCN-WF-017, RR-REQ-X-048-r1, RR-AIN-003-r1, RR-UNB-AI-STD-003-r1, UNB-RCN-GRT-007, REQ-X-017, UNB-RCN-PLT-006, RCD-005, RCD-026, RCD-041, UNB-UI-G-001, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-010, UNB-UI-G-011, UNB-UI-G-016
- **Actors:** Compliance reviewer, Grants approver, Treasury approver, Sponsor counsel, Platform compliance officer, Sponsor operations staff
- **Forma components:** `record-header`, `status-lozenge`, `facts`, `evidence-relationship`, `conflict-review`, `freshness`, `provenance-trail`, `obligation-panel`, `readiness-checklist`, `timeline`, `dialog`, `textarea`, `button`, `flyout`, `conversation`, `handoff-summary`, `split-pane`, `operation-status`, `identifier`, `empty-state`
- **States:** Open; Assigned; In review; Waiting on others; Escalated; Decision proposed; Decision recorded; Reopened; Closed; Closed automatically; Cancelled; Follow-up item linked
- **Actions:** Request evidence (A1; WFM-23-T04); Escalate for authority (A1; WFM-23-T08); Propose decision with evidence snapshot (A1; WFM-23-T09); Record decision through the selected panel - decider holds authority and is not the preparer (A4; WFM-23-T11); Reassign with reason (A2; WFM-23-T03); Explain - open AI summary, read-only (A0; drafted by WFM-23-T10)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-06, FORMA-GAP-07, FORMA-GAP-11, FORMA-GAP-13, FORMA-GAP-14, FORMA-GAP-15, FORMA-GAP-16, FORMA-GAP-17, UNB-GAP-W-02
- **Acceptance criteria:**
  - **AC-1** Given a work item raised by the sanctions match rule on a subject in 'Potential match – on hold'; when the case opens; then the panel slot shows the sanctions potential match panel (UNB-UI-P-015) and no other decision controls.
  - **AC-2** Given a work-item type with no registered panel; when the case opens; then the slot reads 'No decision panel for this case type' and only Request evidence and Escalate are offered.
  - **AC-3** Given the reviewer who proposed the decision; when they open the case; then Record decision is disabled with the text 'You prepared this case; another reviewer must decide'.
  - **AC-4** Given the decider's authority is revoked while the confirmation dialog is open; when they confirm; then the decision is rejected within 60 seconds of revocation with a reason and no decision record is written.
  - **AC-5** Given the AI summary recommends release; when the flyout renders; then it is labelled 'AI-generated, not a decision', lists sources and contains no release or decision control.

### UNB-UI-P-001 — Charitable default disposition panel (panel)
- **Requirement:** The panel MUST open on one of three subtypes named in its header (Succession, Dormancy, Closure residual) and MUST show: (1) the program terms version pinned on the case with the named charitable default recipients and their shares (source: program terms store; immutable on the case); (2) the trigger evidence for the subtype: Succession - the verified death notice and the successor-outreach log showing no eligible successor (WFM-20); Dormancy - the last qualifying activity date (activity projection, with freshness) against the inactivity window (default 36 months) and the outreach evidence per verified channel with delivery outcome (notification delivery records); Closure - the date the recommendation window elapsed; (3) the proposed distribution as a table of recipient, share and amount computed from available balance net of reservations, accrued fees and approved-unpaid grants (ledger, with as-of time); (4) each default recipient's current common charity status lozenge with its IRS-observation freshness (WFM-06). An ineligible or held recipient MUST be flagged with the text 'This charity will be held in the grant lifecycle; you will be asked to choose an alternative' because distributions run through the normal grant lifecycle. Decision options MUST be exactly: Approve charitable default (subtype transition), and for Dormancy only, Hold disposition and, from 'Disposition held', Resume outreach. The panel MUST NOT offer: editing recipients, shares or amounts (program terms change only through UNB-UI-S-010); any escheat or transfer of the fund balance to a state; direct payment (approval only creates grant recommendations); approval while the activity projection is not current (G-003) or the outreach evidence is incomplete, shown as blockers in the readiness checklist. If qualifying activity is detected at commit, the decision MUST NOT be recorded and the panel MUST show 'Active - activity detected since proposal'. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-20-T19, WFM-21-T08, WFM-21-T09, WFM-21-T10, WFM-22-T10, UNB-SUCC-011, RR-UNB-WF-015-r1, UNB-RCN-ENT-005, UNB-RCN-ENT-006, RCD-022, RCD-046, UNB-UI-G-017, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor charitable-default approver (board-delegated), Sponsor operations staff
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `timeline`, `callout`, `identifier`
- **States:** Charitable default pending; Charitable default approved; Disposition pending; Charitable default applied; Disposition held; Inactive - contacting you; Active; Choose where the balance goes; Residual grants in progress
- **Actions:** Approve charitable default - succession; not the preparer (A4; WFM-20-T19); Approve charitable default - dormancy; not the preparer; no escheat (A4; WFM-21-T08); Hold disposition - dormancy (A4; WFM-21-T09); Resume outreach (A2; WFM-21-T10); Approve charitable default - closure residual; not the preparer (A4; WFM-22-T10)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a dormancy disposition prepared by operations staff member X; when X opens the panel; then Approve charitable default is disabled with the reason 'You prepared this case; another approver must decide'.
  - **AC-3** Given a dormancy disposition pending approval; when a donor contribution posts before the approver confirms; then the decision is not recorded, the status reads 'Active' and the panel states that activity was detected since the proposal.
  - **AC-4** Given a charitable default recipient whose common status is 'On hold'; when the panel renders the distribution table; then that row shows the 'On hold' lozenge and the held-in-grant-lifecycle notice, and no control to replace the recipient appears in the panel.

### UNB-UI-P-002 — Alternate eligibility evidence panel (panel)
- **Requirement:** The panel MUST show: the canonical recipient identity (legal name, EIN, legal payee; WFM-06 identity record); the IRS observation proving absence from IRS bulk files (TEOS/Pub 78 and EO BMF revision dates); the claimed entity type (church, government unit, group subordinate, disregarded entity, newly recognised); and the EntityTypeEvidence matrix version as a readiness checklist listing every required evidence item for that type, each with document, source (charity upload, group-exemption letter, government registry, IRS determination letter), evidence class, received date and freshness. Decision options MUST be exactly: Accept evidence - continue classification, enabled only when every required matrix item is present and current; and Determine not a permitted entity. Missing items are requested through the frame's Request evidence action. The panel MUST NOT offer: setting the recipient Eligible (acceptance only writes a common verification status with its evidence class and returns the recipient to deterministic classification); choosing a recipient class; acceptance on AI-extracted evidence that a human has not opened; acceptance with any matrix item missing or stale. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T07, WFM-06-T09, UNB-RCN-GRT-013, RR-UNB-GRANT-023-r1, UNB-CHAR-016, UNB-RCN-GRT-001, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Charity compliance analyst
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `identifier`, `file-upload`, `provenance-trail`
- **States:** Evidence review; Classifying recipient; Not eligible
- **Actions:** Accept evidence - continue classification (A3; WFM-06-T07); Determine not a permitted entity - not the preparer (A3; WFM-06-T09)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a church claim with the group-exemption letter missing from the matrix checklist; when the analyst opens the panel; then Accept evidence is disabled with the reason 'Required evidence missing: group-exemption letter' and the checklist item reads Incomplete.
  - **AC-3** Given all matrix items present; when the analyst accepts; then the status changes to 'Classifying recipient', not 'Eligible', and the decision record names the evidence class.

### UNB-UI-P-003 — Charity hold panel - global (platform-wide) (panel)
- **Requirement:** The panel MUST be visible only to platform compliance roles and MUST show, for the recipient: the common status lozenge; the global hold registry entry (HoldPublicationVersion, reason class, date, placer role); public-source evidence with evidence class E0/E1, source (IRS Auto-Revocation List, state attorney-general action, court record) and retrieval time; the count of affected sponsors and of in-flight payment cases opened. RCD-029 partition: the panel MUST NOT show any sponsor-only hold, its existence, reason or investigation notes, and MUST NOT accept or propagate sponsor-private content; the global reason is a fixed reason-class selection with no free text, and notices to sponsors carry reason class and date only. Options MUST be: Place global hold, Release global hold (requires E0/E1 cure evidence; result is 'Checking IRS status', never 'Eligible'), Confirm not eligible, Correct status determination (linked correction). The panel MUST NOT offer: release directly to Eligible; release by the user who placed the hold; editing or deleting earlier reliance snapshots; any hold or release control in the AI flyout. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T23, WFM-06-T26, WFM-06-T28, WFM-06-T29, RCD-029, RR-REQ-X-020-r1, UNB-EXT-015, RR-AIN-003-r1, RR-UNB-AI-STD-003-r1, RR-REQ-X-007-r1, UNB-GRANT-028, UNB-UI-G-010, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Platform compliance officer (independent)
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `select`, `provenance-trail`, `timeline`, `identifier`
- **States:** Eligible; Review overdue; On hold; Checking IRS status; Not eligible; Status corrected
- **Actions:** Place global hold - reason class only (A4; WFM-06-T23); Release global hold - E0/E1 cure evidence; not the placer (A4; WFM-06-T26); Confirm not eligible (A4; WFM-06-T28); Correct status determination - correction evidence attached (A4; WFM-06-T29)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a recipient that sponsor A holds privately; when the platform compliance officer opens the global panel; then the recipient shows its common status with no indication of sponsor A's hold, reason or notes.
  - **AC-3** Given a global hold with cure evidence attached; when the platform compliance officer releases it; then the status changes to 'Checking IRS status', not 'Eligible'.
  - **AC-4** Given a user without a platform compliance role; when they open a charity hold case; then the global panel is not selectable.

### UNB-UI-P-004 — Charity identity panel (panel)
- **Requirement:** The panel MUST show the request as received (requested name, EIN if given, requester and channel) and every candidate identity from automatic matching side by side: legal name, EIN, address, aliases, and fiscal sponsor or project as separate facts, each with its source (IRS EO BMF, Pub 78, sponsor registry) and source revision date. Conflicting values (same name with different EINs, EIN with a different legal name) MUST be highlighted with text, not colour alone. Decision options MUST be exactly: Confirm identity match - selecting exactly one candidate and attaching source evidence; Reject identity request - with a reason from a fixed list that is sent to the requester. The panel MUST NOT offer: creating a free-text charity or grant recipient; merging two canonical identities; setting or implying eligibility (status is observed after binding); confirming without source evidence attached. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T02, WFM-06-T03, UNB-RCN-PLT-014, UNB-GRANT-007, RR-REQ-X-048-r1, UNB-CHAR-016, UNB-UI-G-016, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Charity data analyst
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `comparison-grid`, `choice-group`, `identifier`, `select`
- **States:** Identity review; Identity confirmed; Could not verify
- **Actions:** Confirm identity match - one candidate, source evidence attached (A3; WFM-06-T02); Reject identity request - reason sent to requester (A3; WFM-06-T03)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-07
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given two candidates with the same legal name and different EINs; when the analyst opens the panel; then both appear side by side with the EIN row labelled 'Conflict' and Confirm is disabled until one candidate is selected and source evidence attached.
  - **AC-3** Given an identity confirmed; when the panel shows the result; then the status reads 'Identity confirmed' and no eligibility label is shown until the IRS observation completes.

### UNB-UI-P-005 — Destination verification panel (panel)
- **Requirement:** The panel MUST show: the proposed destination against the current active version as a field-by-field difference (bank name, routing number, masked account number with last four digits, account owner name); the requester, request channel and time; the callback contact sourced independently from the charity registry, with its source and last-verified date, while any contact supplied in the change request is shown labelled 'Not usable for callback'; the DestinationEvidence table version and the rail minimum as a readiness checklist (bank-originated account-owner match, prenote or return absence, independent callback), each with result, source and freshness; fraud signals (owner mismatch, related-party owner match, callback denial); and, once verified, the effective time after the cooling-off period (default 3 business days, RCD-046) and the old- and new-channel notices. Options by state MUST be: Awaiting callback - Confirm independent callback, Record callback denial; Under security review - Reject destination, Clear fraud signal; Active - Revoke destination. The panel MUST NOT offer: any decision by the requester; a callback to the request-supplied contact; clearing a related-party owner match (the control is absent and the panel links to the related-party determination, UNB-RCN-GRT-002); activation before the cooling-off period ends; rerouting payments already authorized on an earlier version. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-13-T06, WFM-13-T07, WFM-13-T09, WFM-13-T10, WFM-13-T15, RR-REQ-X-005-r1, UNB-RCN-GRT-017, UNB-GEX-018, UNB-CHAR-032, UNB-RCN-GRT-002, UNB-RCN-WF-028, RCD-046, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Destination verifier (not the requester), Sponsor security or compliance reviewer, Sponsor treasury operations
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `diff-viewer`, `identifier`, `security-blockers`, `security-evidence-age`
- **States:** Awaiting callback; Verified – pending activation; Under security review; Rejected; Verifying account; Active; Revoked
- **Actions:** Confirm independent callback - not the requester; registry contact only (A3; WFM-13-T06); Record callback denial (A3; WFM-13-T07); Reject destination - not the requester (A3; WFM-13-T09); Clear fraud signal - explanation evidence; not a related-party match (A3; WFM-13-T10); Revoke destination (A3; WFM-13-T15)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a destination change requested by staff member Z; when Z opens the panel; then Confirm independent callback is disabled with the reason 'You requested this change; another verifier must call back'.
  - **AC-3** Given a fraud signal of type related-party owner match; when the reviewer opens the panel in 'Under security review'; then no Clear fraud signal control is shown; the panel states counsel's determination is required.
  - **AC-4** Given a confirmed callback; when the decision is recorded; then the status reads 'Verified – pending activation' with the effective time three business days later, and no Activate now control exists.

### UNB-UI-P-006 — Fee waiver panel (panel)
- **Requirement:** The panel MUST show the fee period from the ledger with as-of time: fee base (end-of-day fund market value, including reserved unpaid amounts, excluding fees embedded in pool NAV, RCD-018), accrual method (daily, actual/365), accrued amount, minimum fee applied, fee schedule version; for a shortfall, available capacity, reserved grants and the amount by which the fee exceeds capacity; the requester's rationale; and the approver's delegated waiver limit with the requested amount compared against it. Options MUST be: Request fee waiver (relationship manager, from 'Accrued, awaiting settlement'); Approve waiver; Reject or partially waive (amount between zero and accrued); Waive shortfall. The panel MUST NOT offer: any decision by the requester; approval above the approver's delegated limit (the control is disabled with the limit shown and Escalate offered by the frame); editing the fee schedule or accruals; a partial amount greater than the accrued fee. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-15-T05, WFM-15-T06, WFM-15-T07, WFM-15-T12, UNB-ACC-010, UNB-ACC-022, RR-UNB-NAV-033-r1, RCD-018, UNB-UI-G-017, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor relationship manager, Sponsor finance approver independent of requester
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `identifier`, `text-field`, `validation-message`
- **States:** Accrued, awaiting settlement; Waiver requested; Waived; Fee exceeds available capacity
- **Actions:** Request fee waiver (A2; WFM-15-T05); Approve fee waiver - not the requester; within delegated limit (A4; WFM-15-T06); Reject or partially waive - not the requester (A4; WFM-15-T07); Waive shortfall - within delegated limit (A4; WFM-15-T12)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a waiver of $12,000.00 and an approver limit of $10,000.00; when the approver opens the panel; then Approve is disabled with the reason 'Above your waiver limit of $10,000.00' and Escalate is available.
  - **AC-3** Given a waiver requested by relationship manager R; when R opens the panel; then Approve and Reject are disabled with the segregation-of-duties reason.

### UNB-UI-P-007 — Fund application panel (panel)
- **Requirement:** The panel MUST show: applicant identity-proofing result (assurance level, provider, completion time, freshness limit); sanctions screening result (list versions, screening time); agreement version accepted by the applicant against the current version; fund type classification; sponsor program status; and any contribution received before acceptance, labelled 'Held - not accepted, not grantable' with amount and receipt date. Options MUST be exactly Accept fund and Decline application (decision record and applicant notice). The panel MUST NOT offer: acceptance when identity or screening checks are past their freshness limit (on commit the application returns to 'Verifying identity'); acceptance on a superseded agreement version; crediting the held contribution as grantable from this panel; editing applicant data. Release 1 keeps a human acceptance (RCD-023 controls). The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-02-T10, WFM-02-T11, ADM-003, UNB-LEG-002, REQ-X-064, RCD-023, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor account acceptance officer
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `identifier`, `security-evidence-age`
- **States:** In sponsor review; Open; Not accepted; Verifying identity
- **Actions:** Accept fund - checks current (A4; WFM-02-T10); Decline application (A4; WFM-02-T11)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given identity proofing completed beyond its freshness limit; when the officer opens the panel; then Accept is disabled with the reason 'Identity check not current' and the check shows 'Not current'.
  - **AC-3** Given a contribution received before acceptance; when the officer accepts the fund; then the contribution remains labelled 'Held - not accepted, not grantable' until the contribution workflow accepts it.

### UNB-UI-P-008 — Gift acceptance panel (panel)
- **Requirement:** The panel MUST show: the asset description and proposed intermediary with the counsel-approved structure (donee model for release 1, RCD-034); sanctions screening and source-of-funds review results; the excess business holdings computation (combined holdings of fund, donors, advisors and related persons against the counsel-configured limits, limit version, and data completeness per party, with missing data named); and, where recorded, the disposal-period plan. Options MUST be: from 'Held for review' only Decline gift; from 'Excess business holdings review' Decline gift or Approve within holdings plan (only with a recorded disposal-period plan and an agent structure where the sponsor is the holder). The panel MUST NOT offer: approval while a potential sanctions match or source-of-funds review is unresolved; approval when combined holdings cannot be computed; any valuation or acknowledgment action; a decision by the preparer. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-05-T05, WFM-05-T06, UNB-RCN-FIN-004, RR-UNB-INV-012-r1, RCD-014, RCD-034, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor gift acceptance committee member
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `measure`, `identifier`
- **States:** Held for review; Excess business holdings review; Declined by sponsor; Cleared for referral
- **Actions:** Decline gift - not the preparer (A4; WFM-05-T05); Approve within holdings plan - disposal plan recorded; not the preparer (A4; WFM-05-T06)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a gift in 'Held for review' for a potential sanctions match; when a committee member opens the panel; then only Decline gift is offered; no approval control appears.
  - **AC-3** Given related-person holdings data missing for one advisor; when the panel computes combined holdings; then the computation reads 'Cannot compute: holdings missing for <advisor>' and Approve within holdings plan is disabled.

### UNB-UI-P-009 — Liquidity advance panel (panel)
- **Requirement:** The panel MUST show: the grant (reference, recipient, fixed amount, approval date); the funding plan (pool cash buffer available, liquidation order status and expected settlement date from the custodian, with as-of time); the requested advance amount; the approved advance limit (treasury policy version) and current utilisation; paying-account cash balance from the bank with as-of time; and the interfund receivable that approval will book. The only decision option MUST be Approve liquidity advance; not approving leaves the grant on its normal funding path. The panel MUST NOT offer: approval above the remaining advance limit; approval when capacity changed since the preview; any release, eligibility or sanctions override (payment release gates still apply after funding); approval by the preparer or the requester. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-08-T18, RR-UNB-CUST-015-r1, UNB-RCN-FIN-008, UNB-RCN-FIN-009, UNB-UI-G-017, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor treasury approver
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `identifier`, `measure`
- **States:** Funding planned; Selling investments; Funded – ready to pay
- **Actions:** Approve liquidity advance - within limit; not the preparer (A4; WFM-08-T18)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given an advance that exceeds the remaining approved limit; when the treasury approver opens the panel; then Approve is disabled with the reason naming the limit and remaining amount.
  - **AC-3** Given an approved advance; when the decision commits; then the grant reads 'Funded – ready to pay' and the payment still passes every release gate before dispatch.

### UNB-UI-P-010 — Platform-safety policy change panel (panel)
- **Requirement:** The panel MUST show: the proposed sponsor policy version against the current version as a field-by-field difference; the platform-safety-classified parameters touched, each with its hard-control floor or ceiling and the proposed value; the author and sponsor; and a fixed notice 'Clearance confirms no hard control is weakened. It does not approve this policy and gives no sponsor authority.' Options MUST be exactly Record platform-safety clearance (policy moves to 'Awaiting approval' by the sponsor's approver) and Reject on platform safety. The panel MUST NOT offer: approving or activating the sponsor's policy; editing the policy; any decision by its author. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-24-T07, WFM-24-T08, REQ-X-045, GOV-AUTH-016, UNB-RCN-WF-021, UNB-UI-S-010, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Platform risk officer
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `diff-viewer`, `callout`, `security-invariant-result`
- **States:** Platform safety review; Awaiting approval; Rejected
- **Actions:** Record platform-safety clearance - reviewer not author (A4; WFM-24-T07); Reject on platform safety (A4; WFM-24-T08)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a platform risk officer who authored the policy version; when they open the panel; then both decision controls are disabled with the reason 'You authored this version'.
  - **AC-3** Given clearance recorded; when the sponsor views the policy; then it reads 'Awaiting approval', not 'Approved'.

### UNB-UI-P-011 — Positive-pay exception panel (panel)
- **Requirement:** The panel MUST show: the check (number, payee, amount, issue date, days outstanding against the stale threshold, default 180 days, RCD-028); the presentment from the bank positive-pay exception file (presented amount, payee, presentment time) compared field by field with the issued check; void confirmation where present; current recipient eligibility and sanctions status with freshness; and the bank decision cutoff time in the sponsor's time zone. The default decision is Return (UNB-RCN-WF-027) and the panel MUST state what happens at cutoff if no decision is recorded. Options MUST be: Stale-dated - Pay stale presentment (only when sanctions are clear and eligibility is current) or leave the default Return; Voided - Return presented voided check. The panel MUST NOT offer: paying a voided check; paying when presented amount or payee differ from the issued check; paying when sanctions or eligibility are not current; a decision by the person who issued the check. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-11-T11, WFM-11-T13, RR-REQ-X-083-r1, UNB-RCN-GRT-005, UNB-RCN-WF-027, RCD-028, RCD-046, UNB-UI-G-018, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor treasury operations
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `comparison-pairs`, `identifier`, `obligation-panel`
- **States:** Stale-dated; Cashed; Voided
- **Actions:** Pay stale presentment - sanctions clear, eligibility current; not the issuer (A3; WFM-11-T11); Return presented voided check (A3; WFM-11-T13)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a voided check presented for payment; when treasury opens the panel; then only Return is offered.
  - **AC-3** Given a stale-dated presentment whose recipient eligibility observation is 'Not current'; when treasury opens the panel; then Pay is disabled with the reason 'Eligibility not current' and the cutoff notice states the check will be returned.

### UNB-UI-P-012 — Price exception panel (panel)
- **Requirement:** The panel MUST state its context (Gift valuation, WFM-04, or NAV input, WFM-14) and show: the security identifier; the exception reason (stale, missing, conflicting, outlier, feed missing at cutoff); every price observed from each source in the pricing hierarchy with source, timestamp, price convention and hierarchy level; the prior price and tolerance; and the proposed fallback price with its hierarchy level and preparer. The only decision option MUST be Approve fallback price; a price that cannot be approved stays in exception and evidence is requested through the frame. The panel MUST NOT offer: entering a free price outside the pricing hierarchy; forward-filling a previous price; approval by the preparer; publishing a NAV (NAV publication follows RCD-035). The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-04-T15, WFM-14-T05, UNB-NAV-015, UNB-NAV-013, UNB-NAV-008, UNB-RCN-FIN-011, RCD-035, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Pricing reviewer (independent of preparer)
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `identifier`, `provenance-trail`
- **States:** Valuation exception; Valued; Input exception; Inputs validated
- **Actions:** Approve fallback price - gift valuation; not the preparer (A4; WFM-04-T15); Approve fallback price - NAV input; not the preparer (A4; WFM-14-T05)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a fallback price prepared by reviewer P; when P opens the panel; then Approve fallback price is disabled with the segregation-of-duties reason.
  - **AC-3** Given a proposed price whose source is not in the pricing hierarchy; when the reviewer opens the panel; then Approve is disabled with the reason 'Source not in pricing hierarchy' and no price entry field is shown.

### UNB-UI-P-013 — Recipient class (counsel) panel (panel)
- **Requirement:** The panel MUST show: the recipient identity; the IRS observation (subsection, foundation code, private-foundation status, observation date); the RecipientClass rule version and the reason it returned CounselReview; and the counsel memo slot. Options MUST be exactly Determine permitted and Determine not permitted, both requiring an attached counsel memo. The panel MUST NOT offer: a recipient class set or pre-selected from AI output; an 'Expenditure responsibility required' or foreign-recipient approval at release 1 (RCD-014); a decision by any role other than sponsor counsel. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T11, WFM-06-T12, UNB-RCN-GRT-001, RCD-014, RCD-013, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor counsel
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `file-upload`, `identifier`
- **States:** Counsel review; Eligible; Not eligible
- **Actions:** Determine permitted - counsel memo attached (A4; WFM-06-T11); Determine not permitted - counsel memo attached (A4; WFM-06-T12)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given no counsel memo attached; when counsel opens the decision dialog; then both decision controls are disabled with the reason 'Attach the counsel memo'.
  - **AC-3** Given a compliance analyst without the counsel role; when they open the panel; then the evidence is visible and no decision control is shown.

### UNB-UI-P-014 — Related-party payee panel (panel)
- **Requirement:** The panel MUST show the match side by side: the grant's payee legal entity, destination account owner or declared beneficiary against the matching related-party registry entry (name, relationship to the donor or advisor, source, recorded date); the payment class (reimbursement, tuition, travel, fees to an advisor's firm) and whether counsel has approved that class; and counsel's determination record when present. Options MUST be: Release hold on counsel determination (returns the grant to 'In review' and re-runs every rule) and Decline grant with explanation. The hold is nonwaivable: the panel MUST NOT offer any override or waiver, release without a recorded counsel determination, release by a fund participant or the grant's preparer, or release straight to 'Approved'. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-08-T09, WFM-08-T10, UNB-RCN-GRT-002, RR-AIN-003-r1, RCD-014, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Independent compliance officer (on counsel determination)
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `comparison-pairs`, `identifier`
- **States:** Blocked – related party; In review; Declined
- **Actions:** Release hold on counsel determination - independent; not the preparer (A4; WFM-08-T09); Decline grant with explanation (A3; WFM-08-T10)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given no counsel determination recorded; when the compliance officer opens the panel; then Release is disabled with the reason 'Counsel determination required'.
  - **AC-3** Given a release recorded; when the grant updates; then its status reads 'In review', not 'Approved'.

### UNB-UI-P-015 — Sanctions potential match panel (panel)
- **Requirement:** The panel MUST show: the screened subject's attributes against the list entry's attributes side by side (name, aliases, date of birth, nationality, address, identifiers) with the match rule and score; list name, list version and publication date; the screening trigger (onboarding, grant approval, payment release, list update); prior good-guy suppressions for the subject; every dependent grant, ACH payment and check held by the match, with its current status, read-only; and whether property is in the sponsor's possession (ledger, with as-of time), which decides between the two true-match outcomes. Options MUST be: from 'Potential match – on hold' or 'Escalated to compliance officer' - Dispose as false positive (distinguishing identifiers required), Escalate to compliance officer, Confirm true match - block property, Confirm true match - refuse; Request evidence comes from the frame; from 'Confirmed match – refused' - Correct determination (linked correction). Licence and delisting release of blocked items is NOT in this panel; it is the separate Sanctions post-block release panel (UNB-UI-P-019), which resolves only once the subject is blocked. The panel MUST NOT offer: any release of a dependent grant, payment or check while the match is unresolved (the controls are absent, not disabled); a false-positive disposition without recorded distinguishing identifiers; true-match confirmation by the analyst (A3); any disposition from the AI flyout. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-12-T06, WFM-12-T07, WFM-12-T08, WFM-12-T09, WFM-12-T16, WFM-23-T04, RCD-015, RCD-044, UNB-RCN-GRT-007, UNB-RCN-REG-003, UNB-RCN-WF-024, RR-UNB-EXT-004-r1, RR-REQ-X-036-r1, RR-AIN-003-r1, RR-UNB-AI-STD-003-r1, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sanctions analyst, Sanctions compliance officer, Independent compliance officer
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `comparison-pairs`, `identifier`, `dense-ledger`, `timeline`
- **States:** Potential match – on hold; Escalated to compliance officer; Clear; Blocked – OFAC report due; Confirmed match – refused; Determination corrected
- **Actions:** Dispose as false positive - distinguishing identifiers recorded; not the preparer (A3; WFM-12-T06); Escalate to compliance officer (A2; WFM-12-T07); Confirm true match - block property; not the preparer (A4; WFM-12-T08); Confirm true match - refuse; not the preparer (A4; WFM-12-T09); Correct determination - correction evidence attached (A4; WFM-12-T16); Request evidence (A1; WFM-23-T04)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a subject in 'Potential match – on hold' with two held payments; when any user opens the panel; then the payments are listed read-only with their held status and no release control exists in this panel or in the post-block release panel.
  - **AC-3** Given a sanctions analyst (A3); when they open a potential match; then Confirm true match controls are not offered; Dispose as false positive and Escalate are.
  - **AC-4** Given the analyst submits false positive without distinguishing identifiers; when they confirm; then the dialog shows a validation message 'Record at least one distinguishing identifier' and nothing is submitted.

### UNB-UI-P-016 — Succession allocation panel (panel)
- **Requirement:** The panel MUST show: the verified death or incapacity notice; each successor with identity verification and sanctions status and their freshness; open contests; the program rules version; and the allocation preview at the sponsor-designated valuation point (fund value, reservations, accrued fees, approved-unpaid grants, net allocable) with each successor's share and amount as a read-only allocation summary; for 'Split held', the constraint that failed (minimum, restriction, illiquidity, open commitment). Options MUST be: Split held - Decide split constraint (within program rules; recomputes the allocation); Allocation proposed - Activate succession. The panel MUST NOT offer: activation while a contest is open, a successor is unverified or not sanctions-clear, or the allocation is unbalanced; activation by break-glass access; editing shares outside program rules; allocation to another legal sponsor (that is an outbound transfer, WFM-22). If capacity changed since the preview, the decision is not recorded and the panel shows a new preview in 'Allocation proposed'. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-20-T17, WFM-20-T18, RR-UNB-SUCC-004-r1, UNB-SUCC-004, RR-UNB-SUCC-008-r1, UNB-SUCC-002, UNB-SUCC-003, UNB-SUCC-009, UNB-RCN-FIN-009, RCD-022, UNB-UI-G-017, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor succession reviewer, Sponsor succession approver
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `dense-ledger`, `identifier`, `security-blockers`
- **States:** Split held; Allocation proposed; Succession active; Contested
- **Actions:** Decide split constraint - preparer not decider (A4; WFM-20-T17); Activate succession - not the preparer; no break-glass (A4; WFM-20-T18)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08, FORMA-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a contest received; when the approver opens the panel; then Activate succession is disabled with the reason 'Contest open' and the status reads 'Contested'.
  - **AC-3** Given capacity changed after the preview; when the approver activates; then the activation is not recorded, the status remains 'Allocation proposed' and a new preview with its as-of time is shown.

### UNB-UI-P-017 — Supporting-organization certification review panel (staff, read-only) (panel)
- **Requirement:** The panel MUST be read-only for sponsor staff and show: the supporting-organisation type (Type I, Type II or functionally integrated Type III) from the IRS observation with its date; the counsel-approved certification form version; the certification status, signer and signer-authority verification result; submission date and expiry date; and the grants held by 'Certification needed'. The only action MUST be Request renewal, which asks the charity officer for a new certification through the charity screen (UNB-UI-S-061). The panel MUST NOT offer: submitting or editing the certification on the charity's behalf; overriding an expired or missing certification; approving grants while the status reads 'Certification needed'. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T13, WFM-23-T04, UNB-RCN-REG-002, RCD-014, UNB-UI-S-012, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Charity compliance analyst
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `identifier`, `timeline`
- **States:** Certification needed; Eligible
- **Actions:** Request renewal (A1; WFM-23-T04)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a sponsor staff user; when they open the panel; then the certification status, signer and expiry are shown and no submit or edit control appears.
  - **AC-2** Given a certification past its expiry date; when the panel renders; then the status reads 'Certification needed' and Request renewal is available.

### UNB-UI-P-018 — Unknown payment panel (panel)
- **Requirement:** The panel MUST show: the payment chain (grant reference, amount, pinned destination version, provider idempotency key, dispatch time, dispatcher); the provider status-query history (each query time and response); the rail settlement window and when it elapsed; bank-statement reconciliation (statement date, debit found or not); recipient confirmation of non-receipt (source, channel, date); and the aging tier with days in 'Status unknown' against the final tier (default 30 days, RCD-042). The four NotReceived evidence items (settlement window elapsed, provider attests no execution, no statement debit, recipient confirms non-receipt) MUST be a readiness checklist with source and freshness each. Options MUST be: Check status (G-006, query by idempotency key); Declare not received - create replacement, enabled only when all four items are present and current and the decider is not the dispatcher; and, only after the final aging tier, Resolve by recorded decision requiring two independent treasury approvers, a duplicate-risk acceptance and a recovery plan (RCD-042). The panel MUST NOT offer: resend, retry or reissue of any kind outside these options; 'Mark paid' or 'Mark failed'; a single-approver override; both approvals by the same person; approval by the dispatcher. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-10-T10, WFM-10-T08, WFM-10-T09, RR-REQ-X-004-r1, UNB-RCN-GRT-008, UNB-RCN-WF-022, RCD-042, RCD-028, RR-AIN-003-r1, UNB-UI-G-006, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-011
- **Actors:** Treasury approver independent of the dispatcher, Second independent treasury approver
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `operation-status`, `timeline`, `identifier`, `security-unknown`, `state-survivability`
- **States:** Status unknown; Not received – replacement created; Accepted by bank partner; Payment sent
- **Actions:** Check status by idempotency key (A1; WFM-10-T08); Declare not received - create replacement; four evidence items; not the dispatcher (A4; WFM-10-T10); Resolve by recorded decision - two independent approvers after final aging tier (A4 + A4; RCD-042, UNB-RCN-WF-022 - no WFM-10 transition yet)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08, FORMA-GAP-24
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a payment in 'Status unknown' with no recipient confirmation of non-receipt; when the treasury approver opens the panel; then Declare not received is disabled with the reason 'Missing: recipient confirms non-receipt' and no resend or reissue control exists.
  - **AC-3** Given the treasury approver who dispatched the payment; when they open the panel; then Declare not received is disabled with the segregation-of-duties reason.
  - **AC-4** Given a payment past the final aging tier and a first approval recorded by approver A; when approver A tries to give the second approval; then the control is disabled with the reason 'A second, different approver is required'; status remains 'Status unknown'.
  - **AC-5** Given Check status returns no definite outcome; when the panel updates; then operation-status shows unknown, nothing is retried automatically and no success or failure is shown.

### UNB-UI-P-019 — Sanctions post-block release panel (panel)
- **Requirement:** The panel MUST resolve only when the subject's screening state is 'Blocked – OFAC report due' or 'Blocked – reported', or a dependent grant, ACH payment or check is 'Blocked'; for any other state, including 'Potential match – on hold' and 'Escalated to compliance officer', it MUST NOT be selectable and no release control exists. It MUST show: the confirmed match and its decision record (read-only); the blocked property items with ledger amounts and as-of time; the OFAC report due date (10 business days), filing status and annual-report tracking; licence evidence (OFAC licence number, issue and expiry dates, scope: named parties, property and transactions covered) with each blocked item marked 'Covered' or 'Not covered' by that scope; delisting evidence (list name, delisting publication date and list version, source); and for an ACH payment whether it was dispatched. Options MUST be: Release licensed property (subject stays a confirmed match; only covered items move); Record delisting (re-screen queued); Release blocked grant under licence or delisting; Release blocked ACH payment under licence (not yet dispatched only); Record blocked funds returned under licence; Release blocked check under licence (void confirmed). Every release re-enters all release gates. The panel MUST NOT offer: release of any item not covered by the licence scope; release with an expired licence; retry or reissue of a blocked payment; release by anyone other than an independent compliance officer at A4, or by the person who confirmed the match; any release control in the AI flyout. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-12-T13, WFM-12-T14, WFM-08-T34, WFM-10-T19, WFM-10-T20, WFM-11-T21, RCD-015, UNB-RCN-GRT-006, UNB-RCN-REG-003, RR-AIN-003-r1, RR-UNB-AI-STD-003-r1, UNB-UI-G-018, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Independent compliance officer
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `identifier`, `dense-ledger`, `obligation-panel`, `file-upload`, `provenance-trail`
- **States:** Blocked – OFAC report due; Blocked – reported; Screening; Blocked; Approved; Preparing payment; Returned; Voided
- **Actions:** Release licensed property - licence covers transaction; independent (A4; WFM-12-T13); Record delisting - delisting evidence; independent (A4; WFM-12-T14); Release blocked grant under licence or delisting (A4; WFM-08-T34); Release blocked ACH payment under licence - not yet dispatched (A4; WFM-10-T19); Record blocked funds returned under licence (A4; WFM-10-T20); Release blocked check under licence - void confirmed (A4; WFM-11-T21)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a subject in 'Potential match – on hold'; when a compliance officer looks for the post-block release panel; then the panel is not selectable and no release control appears anywhere in the case.
  - **AC-3** Given a licence whose scope covers one of three blocked items; when the officer opens the panel; then only the covered item has an enabled release control; the other two read 'Not covered by licence'.
  - **AC-4** Given a blocked ACH payment already dispatched; when the officer opens the panel; then Release blocked ACH payment is disabled with the reason 'Already sent to bank partner'; Record blocked funds returned under licence remains available.

### UNB-UI-P-020 — Charity hold panel - sponsor-only (panel)
- **Requirement:** The panel MUST be visible only to the holding sponsor's compliance reviewers, within that sponsor's tenant context (G-010), and MUST show: the recipient's common status (read-only, as every sponsor sees it); the sponsor-only hold entry (HoldPublicationVersion, date, placer); the sponsor-private investigation notes and attachments, labelled 'Visible only to <sponsor>'; and whether a global hold also exists (status and reason class only). RCD-029 partition: nothing entered here may reach another sponsor or the global hold; other sponsors and platform roles see the recipient as 'Eligible' unless a global hold exists. Options MUST be: Place sponsor-only hold (from 'Eligible'); Release sponsor-only hold (another compliance officer, not the placer); the record returns to 'Eligible' for this sponsor only. The panel MUST NOT offer: placing or releasing a global hold (escalation to platform compliance goes through the frame's Escalate, carrying reason class only); sharing or exporting notes to another sponsor; release by the placer; any hold or release control in the AI flyout. The panel renders in the panel slot of the case workspace frame and inherits its segregation-of-duties, confirmation (G-005), unknown-outcome (G-006) and AI-labelling (G-011) rules.
- **Traces to:** WFM-06-T24, WFM-06-T25, RCD-029, UNB-CHAR-036, UNB-EXT-033, RR-UNB-AI-STD-003-r1, UNB-UI-G-010, UNB-UI-S-009, WFM-23-T11, UNB-RCN-WF-017, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-011
- **Actors:** Sponsor compliance officer
- **Forma components:** `facts`, `key-value-list`, `evidence-relationship`, `freshness`, `status-lozenge`, `readiness-checklist`, `dialog`, `textarea`, `button`, `callout`, `file-upload`, `timeline`, `identifier`
- **States:** Eligible; On hold (this sponsor)
- **Actions:** Place sponsor-only hold (A3; WFM-06-T24); Release sponsor-only hold - not the placer (A3; WFM-06-T25)
- **Responsive:** desktop-first (phone-review-and-approve for approve and decline only)
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a user who prepared or proposed this decision in WFM-23; when they open the panel; then every decision control is disabled with the visible reason 'You prepared this case; another reviewer must decide'.
  - **AC-2** Given a sponsor-only hold placed by sponsor compliance officer Y; when Y opens the panel; then Release sponsor-only hold is disabled with the reason 'You placed this hold; another compliance officer must release it'.
  - **AC-3** Given a compliance reviewer from sponsor B; when they open a case on a recipient sponsor A holds privately; then the sponsor-only panel for sponsor A is not selectable and the recipient shows 'Eligible'.

### UNB-UI-S-061 — Supporting-organization certification (charity) (screen)
- **Requirement:** The charity screen MUST present to a charity officer of a Type I, Type II or functionally integrated Type III supporting organisation, reached from the charity view (UNB-UI-S-012): the reason certification is needed and the grants waiting on it; the counsel-approved donor-control certification form (version shown) as a form with required attestations; the signer's name, title and signer-authority verification result; and, after submission, the stored certification with its expiry date. The single action MUST be Submit donor-control certification, which on success moves the recipient from 'Certification needed' to 'Eligible'. The screen MUST NOT offer: submission when signer authority is not verified (control disabled with the reason); submission on a superseded form version; certification for a non-functionally integrated Type III organisation (shown as not eligible, no form); any view of sponsor-internal notes or of which donors recommended grants beyond what the charity view already shows. Submission uses the G-005 confirmation and G-012 validation; it is phone-complete.
- **Traces to:** WFM-06-T13, UNB-RCN-REG-002, RCD-014, UNB-UI-S-012, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-012, UNB-UI-G-018
- **Actors:** Charity officer (grantee certification)
- **Forma components:** `form`, `form-row`, `checkbox`, `text-field`, `validation-message`, `validation-summary`, `dialog`, `button`, `status-lozenge`, `freshness`, `key-value-list`, `operation-status`
- **States:** Certification needed; Eligible
- **Actions:** Submit donor-control certification - verified signer authority (A2; WFM-06-T13)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a charity officer whose signer authority is not verified; when they open the form; then Submit is disabled with the reason 'Signer authority not verified'.
  - **AC-2** Given a complete form with all attestations checked; when the officer submits and confirms; then the status changes from 'Certification needed' to 'Eligible' and the expiry date is shown.
  - **AC-3** Given a submission that returns no definite outcome; when the screen updates; then operation-status shows unknown with 'Check status'; nothing is resubmitted automatically.

### UNB-UI-S-015 — Fund opening and lifecycle requests (workspace frame) (screen)
- **Requirement:** WS-06 MUST be the applicant's and donor's single place to start and follow requests that change a fund's existence: opening a fund, closing it and transferring its balance to another sponsor. Layout: scope-trail (sponsor > program > fund, G-010) above a navigation-shell with three views: 'Open a fund', 'Close a fund', 'Transfer to another sponsor'. The landing view MUST list the user's open requests first (RCD-026), ordered by what needs the user: (1) items waiting on the user with a due date, e.g. 'Choose where the balance goes' with the residual-recommendation window end date, an application draft with its agreement version, an expired-agreement re-acknowledgment; (2) requests in progress with their current status label and the party acting next; then (3) start actions. 'Close a fund' MUST be offered only to an advisory-privilege holder of an Open fund; 'Transfer to another sponsor' MUST be reachable only from a closure in 'Choose where the balance goes' (RCD-040: outbound transfer is the residual payout inside closure). For other participants these entries are shown unavailable with the reason (FORMA-GAP-11). The frame MUST carry the legal-sponsor disclosure footer (G-014) and render protected legal text verbatim (G-007). There is no period context; dates are absolute in the sponsor's time zone (G-018).
- **Traces to:** RCD-026, RCD-040, ADM-002, UNB-RCN-ENT-006, UNB-RCN-ENT-007, UNB-RCN-WF-014, UNB-UI-G-007, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-G-014, UNB-UI-G-018
- **Actors:** Applicant, Donor, Advisory-privilege holder
- **Forma components:** `navigation-shell`, `scope-trail`, `work-queue`, `status-lozenge`, `record-header`, `identifier`, `mobile-action-bar`, `empty-state`, `skeleton`, `site-footer`
- **States:** Application draft; Application submitted; Verifying identity; Check result unknown; In sponsor review; Open; Not accepted; Withdrawn; Closure requested; Settling open items; Closure blocked; Choose where the balance goes; Checking receiving sponsor; Residual grants in progress; Closed; Closure cancelled
- **Actions:** Open view: Open a fund (A0; navigation only); Open view: Close a fund (A0; navigation only); Open view: Transfer to another sponsor (A0; navigation only)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a donor with a fund in 'Choose where the balance goes' and an application draft for a second fund; when they open WS-06; then the residual decision is listed first with its window end date, then the draft; start actions follow.
  - **AC-2** Given a participant who is not an advisory-privilege holder; when they open WS-06 for an Open fund; then 'Close a fund' is shown unavailable with the text 'Only an advisory-privilege holder can request closure' and no request can be started.
  - **AC-3** Given a fund not in 'Choose where the balance goes'; when the user navigates directly to the transfer view URL; then the view shows that a transfer is chosen as part of closing the fund and offers no transfer form.

### UNB-UI-S-016 — Help and support status (workspace frame) (screen)
- **Requirement:** WS-08 MUST show donors, advisors and charity users every support request and reconsideration request they made, in one list. Landing (RCD-026): first, requests 'Waiting on you' with the reply due date; second, 'Awaiting your confirmation' changes (link to the verified-channel confirmation view, S-029, only when opened from the verified channel); third, open requests with status, the party acting next and the response due date (default 30 days for reconsideration, RCD-046); last, resolved and closed requests with the reopen window end. Each row shows the case reference (G-016), subject, the original decision it concerns (for reconsideration), and the status label. A 'Start a request' action is always available. The frame MUST NOT show internal triage notes, the AI classification or the reviewer's identity beyond role; a case in 'Security review' shows only that the account is protected and how to reach support. Scope trail shows sponsor and, for charity users, the charity organisation (G-010).
- **Traces to:** RCD-026, RCD-046, WFM-25-T02, UNB-RCN-ENT-016, UNB-RCN-WF-018, ADM-015, ADM-016, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-G-016
- **Actors:** Donor, Advisor, Charity finance user
- **Forma components:** `navigation-shell`, `scope-trail`, `work-queue`, `status-lozenge`, `identifier`, `empty-state`, `skeleton`, `mobile-action-bar`, `site-footer`
- **States:** Received; In progress; Waiting on you; Awaiting your confirmation; Under reconsideration; Decision made; Resolved; Closed; Withdrawn; Security review
- **Actions:** Open view: My requests and appeals (A0; navigation only); Start a request (A1; creates a support case, then WFM-25-T02)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given a donor with one case 'Waiting on you' and two cases 'In progress'; when they open WS-08; then the 'Waiting on you' case is first with its reply due date.
  - **AC-2** Given a case in 'Security review' after the donor denied an assisted change; when the donor opens WS-08; then the row shows 'Security review' (blocked) and support contact guidance, and no internal investigation detail.

### UNB-UI-S-017 — Grant approval (workspace frame) (screen)
- **Requirement:** WS-11 MUST be the grants approver's place for routine approvals that the board has not delegated (RCD-005). Header: scope-trail (sponsor > program) and an authority banner sourced from the board delegation register (S-050) stating 'Routine approval: not delegated - each grant needs your approval' or, once delegated, 'Routine approval delegated to policy version <v> (effective <date>)'; when delegation lapses the banner MUST show that routine approval has reverted to people (WFM-01-T18, UNB-RCN-WF-016). Landing (RCD-026): (1) the current batch of grants in 'Approval recommended' with count, total amount (G-017), oldest age and the policy version that recommended them; (2) items that left the batch since the last view with the reason (policy version changed, eligibility or sanctions changed, waiting for funds); (3) a link to grants needing an individual decision ('Awaiting approval'), which open in the case workspace (S-009), not here. Freshness of the eligibility and sanctions projections is shown once for the batch (G-003). No configurable dashboard (RCD-026).
- **Traces to:** RCD-005, RCD-026, WFM-08-T01, WFM-01-T18, UNB-RCN-WF-016, RR-REQ-X-002-r1, UNB-UI-G-003, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-S-009
- **Actors:** Grants approver
- **Forma components:** `navigation-shell`, `scope-trail`, `callout`, `metric-card`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `mobile-action-bar`
- **States:** Approval recommended; Awaiting approval; Approved; In review; Waiting for funds
- **Actions:** Open view: Batch approval (A0; navigation only); Open individual approvals in the case workspace (A0; navigation only)
- **Responsive:** phone-review-and-approve
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-08
- **Acceptance criteria:**
  - **AC-1** Given routine approval not delegated and 42 grants in 'Approval recommended'; when the approver opens WS-11; then the banner reads 'Routine approval: not delegated' and the landing shows 42 grants, their total and the oldest age.
  - **AC-2** Given the board delegation for routine approval lapsed overnight; when the approver opens WS-11; then the banner states routine approval has reverted to people, with the lapse date, and newly evaluated grants appear in the batch.

### UNB-UI-S-018 — Treasury and payments (workspace frame) (screen)
- **Requirement:** WS-12 MUST group treasury work by goal for one sponsor and business date: views 'Manual release approval', 'Checks: void and reissue', 'Checks: unclaimed property', 'OFAC reports'. Header: scope-trail (sponsor), business date in the sponsor time zone, and freshness for the bank statement feed, partner-rail callbacks and positive-pay acknowledgements (G-003). Landing (RCD-026), exceptions and due items first, each with count, oldest age and SLA or legal due date: OFAC reports due (business-day deadline per UNB-RCN-REG-003) and confirmed-match refusals awaiting report data (UNB-RCN-WF-024); payments 'Status unknown' by aging tier (links to the unknown payment panel in WS-10; RCD-042 dual resolution); 'Release blocked' items eligible for manual release (only those whose sole blocker is destination evidence, UNB-RCN-WF-023); checks 'Void requested' without bank acknowledgement; voided checks in 'Unclaimed property review' with state report dates; checks 'Not yet cashed - contacting charity' and 'Stale-dated'. Routine settled payments do not appear. Positive-pay exceptions link to the WS-10 panel. Amounts follow G-017 with tabular numerals.
- **Traces to:** RCD-026, RCD-028, RCD-015, RCD-042, RCD-047, UNB-RCN-REG-003, UNB-RCN-WF-023, UNB-RCN-WF-024, WFM-11-T09, WFM-11-T15, WFM-12-T10, UNB-UI-G-003, UNB-UI-G-009, UNB-UI-G-017
- **Actors:** Sponsor treasury operations, Treasury release approver, Sanctions compliance officer, Sponsor counsel, Controller
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `dashboard-grid`, `metric-card`, `data-grid`
- **States:** Status unknown; Release blocked; Void requested; Voided; Unclaimed property review; Reportable as unclaimed property; Not yet cashed – contacting charity; Stale-dated; Blocked – OFAC report due; Confirmed match – refused
- **Actions:** Open view: Manual release approval (A0; navigation only); Open view: Checks: void and reissue (A0; navigation only); Open view: Checks: unclaimed property (A0; navigation only); Open view: OFAC reports (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given an OFAC report due in 2 business days and 300 settled payments today; when treasury opens WS-12; then the OFAC item is first with its due date; settled payments are not listed.
  - **AC-2** Given the bank statement feed is older than its freshness limit; when the landing renders; then the feed shows 'Not current' (unknown) and counts derived from it are labelled as not current.

### UNB-UI-S-019 — Fund accounting and close (workspace frame) (screen)
- **Requirement:** WS-13 MUST organise accounting work around one accounting period and its close. Header: scope-trail (sponsor > accounting period) and a period context bar showing period, cutoff date, close state label (Open, Reconciling, ..., Statements published), snapshot as-of and known-at once frozen, and a period selector limited to open and the last 12 closed periods. Views: 'Reconciliation and attestation', 'Close certification', 'Unit pricing', 'Pool liquidity', 'Noncash forms: 8283', 'Noncash forms: 8282'. Landing (RCD-026), exceptions and due items first: unexplained breaks blocking certification (class, amount, age against AgingLimit, affected funds); 'Evidence missing' sources; NAV runs in 'Exception hold', 'Input exception' or 'Valuation suspended'; pools 'Liquidity buffer below floor'; attestation sample outstanding (n of N, RCD-037); Form 8283 signatures pending and Form 8282 filings due with their due dates. Then progress: reconciliation classes passing, NAVs published today (policy-published and human-approved counted separately, RCD-035). Matched auto-passed items are counted, never listed (RCD-020).
- **Traces to:** RCD-026, RCD-020, RCD-035, RCD-037, REQ-X-040, WFM-16-T01, WFM-16-T03, WFM-14-T08, UNB-UI-G-003, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-G-017
- **Actors:** Controller, Fund accountant, Independent finance reviewer, Independent NAV checker, Sponsor investment operations
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `dashboard-grid`, `metric-card`, `select`
- **States:** Open; Reconciling; Evidence missing; Breaks open; Reconciled; Awaiting sampled attestation; Snapshot frozen; Certified; Statements published; Exception hold; Input exception; Valuation suspended; Liquidity buffer below floor; Form 8282 due
- **Actions:** Open view: Reconciliation and attestation (A0; navigation only); Open view: Close certification (A0; navigation only); Open view: Unit pricing (A0; navigation only); Open view: Pool liquidity (A0; navigation only); Open view: Noncash forms: 8283 (A0; navigation only); Open view: Noncash forms: 8282 (A0; navigation only); Change period (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-08, FORMA-GAP-15, FORMA-GAP-25
- **Acceptance criteria:**
  - **AC-1** Given an unexplained bank cash break of $1,250.00 aged 6 days and 12,400 auto-passed matches; when the controller opens WS-13; then the break is listed first with amount, age and affected funds; the matches appear only as a count.
  - **AC-2** Given the custody feed did not arrive by cutoff; when the landing renders; then the custody class shows 'Evidence missing' (unknown), never 'Pass'.

### UNB-UI-S-020 — Regulatory reporting (workspace frame) (screen)
- **Requirement:** WS-14 MUST organise annual regulatory reporting by tax year. Header: scope-trail (sponsor > tax year), filing due date, extension status and current escalation tier (UNB-RCN-WF-006). Views: 'Account-type classification' and 'Annual DAF package'. Landing (RCD-026): first, blockers and due items: 'Blocked: classification register', 'Does not reconcile', 'Rejected by IRS', 'Filed, awaiting acceptance' past its expected date, and the next deadline; then the package step track (register check > locked year > built > reconciled > prepared > reviewed > signed > filed > accepted) with the named preparer, reviewer and signer and their dates. RCD-024: release 1 offers only the standard DAF product; any other product type appears as blocked until classified. No AI signing or filing control exists anywhere in this workspace.
- **Traces to:** RCD-026, RCD-024, UNB-RCN-WF-006, UNB-RCN-WF-007, UNB-RCN-ENT-011, UNB-RCN-ENT-012, RR-REQ-X-060-r1, WFM-17-T05, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-G-011
- **Actors:** Controller, CFO, Tax preparer, Qualified reviewer, Authorized officer (signer), Sponsor counsel, Sponsor tax staff
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `steps`, `metric-card`
- **States:** Not started; Checking classification register; Blocked: classification register; Awaiting locked year; Package built; Does not reconcile; Reconciled to ledger; Prepared; Reviewed; Signed; Filed, awaiting acceptance; Rejected by IRS; Accepted; Not applicable; Amendment in progress; Amended return filed
- **Actions:** Open view: Account-type classification (A0; navigation only); Open view: Annual DAF package (A0; navigation only); Change tax year (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-25
- **Acceptance criteria:**
  - **AC-1** Given a package 'Rejected by IRS' and a filing deadline in 20 days for another year; when the controller opens WS-14; then the rejection is listed first, then the deadline with its escalation tier.
  - **AC-2** Given any user; when they look for an AI-assisted sign or file action; then none exists; AI explanation, where offered, is labelled 'AI-generated, not a decision' and has no controls (G-011).

### UNB-UI-S-021 — Records and retention (workspace frame) (screen)
- **Requirement:** WS-15 MUST let records owners and counsel manage legal holds and class-level disposition (RCD-007). Header: scope-trail (sponsor or platform), retention catalog version and effective date. Views: 'Legal holds' and 'Disposition batches'. Landing (RCD-026): first, disposition batches awaiting the second approver with record counts and age; 'Disposition outcome unknown' batches; holds with no review date or whose matter is closed; then retention classes reaching review in the next 90 days. The frame MUST state that expiry creates review, never deletion, and MUST NOT offer any per-record delete control anywhere (RR-REQ-X-036-r1).
- **Traces to:** RCD-007, RCD-026, RR-REQ-X-036-r1, REQ-X-036, REQ-X-038, WFM-18-T12, UNB-UI-G-009, UNB-UI-G-010
- **Actors:** Records manager, Legal counsel
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `metric-card`, `data-grid`
- **States:** In retention; Superseded, retained; Legal hold; Eligible for disposition review; Disposition approved; Disposition outcome unknown; Disposed
- **Actions:** Open view: Legal holds (A0; navigation only); Open view: Disposition batches (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given a batch approved by one records manager awaiting a second approver; when counsel opens WS-15; then the batch is listed first with 'Waiting for second approver' and its record count.
  - **AC-2** Given any user; when they open a record from WS-15; then no delete control is shown; only hold, release and batch actions defined in the views exist.

### UNB-UI-S-022 — Fund lifecycle operations (workspace frame) (screen)
- **Requirement:** WS-16 MUST give sponsor operations one view of funds that are closing or transferring out. Header: scope-trail (sponsor > program). View: 'Closures and transfers'; dormancy, succession and charitable-default decisions open in the WS-10 case workspace panels and are linked, not duplicated. Landing (RCD-026): first, 'Closure blocked' funds with the blocking obligation (outstanding check, open reservation, provisional contribution, open case); residual-recommendation windows ending within 7 days (UNB-RCN-WF-014: an A4 default disposition follows); transfers 'Checking receiving sponsor'; late items after archive routed to the trailing destination (RCD-040, UNB-RCN-WF-015). Then funds in progress by state with age.
- **Traces to:** RCD-026, RCD-040, RCD-022, UNB-RCN-ENT-006, UNB-RCN-ENT-007, UNB-RCN-WF-014, UNB-RCN-WF-015, WFM-22-T02, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-S-009
- **Actors:** Sponsor operations, Sponsor finance officer
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `data-grid`
- **States:** Closure requested; Settling open items; Closure blocked; Open items settled; Final fees posted; Choose where the balance goes; Checking receiving sponsor; Residual grants in progress; Balance distributed; Closed; Archived; Late item after archive
- **Actions:** Open view: Closures and transfers (A0; navigation only); Open charitable default panel in the case workspace (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given a fund 'Closure blocked' by one outstanding check and 30 funds settling normally; when operations opens WS-16; then the blocked fund is first with 'Outstanding check' as reason.
  - **AC-2** Given a receipt arrives for an archived fund; when the landing renders; then it shows under 'Late item after archive' with its trailing destination and no control to reopen the fund.

### UNB-UI-S-023 — Sponsor implementation and activation (workspace frame) (screen)
- **Requirement:** WS-17 MUST take one legal sponsor from prospect to active program and, later, through suspension and exit (WFM-01). Header: scope-trail (platform > sponsor in onboarding); platform staff choose the sponsor from a server-issued list (G-010). Views: 'Contract and activation record', 'Board delegations', 'Tenant provisioning', 'Readiness checklist', 'Opening balance tie-out', 'Activation gate'. Landing (RCD-026): the activation path as an ordered checklist of WFM-01 states with blockers first ('Setup failed', 'Setup status unknown', 'Configuration blocked', 'Integration checks failed', 'Opening balance break'), each with the failing check and its owner; then due items: contract expiry date and board-delegation expiry date (both re-checked at activation), and on an active sponsor the days to contract lapse (UNB-RCN-WF-010). Platform staff hold A2 here; only sponsor officers hold A4 actions, which are shown unavailable to platform staff with the reason (FORMA-GAP-11).
- **Traces to:** RCD-026, RCD-004, RCD-005, REQ-X-064, UNB-RCN-WF-009, UNB-RCN-WF-010, UNB-RCN-WF-016, UNB-RCN-PLT-005, UNB-UI-G-009, UNB-UI-G-010
- **Actors:** Platform implementation lead, Platform operations engineer, Sponsor authorized officer, Sponsor finance officer, Sponsor corporate secretary, Sponsor controller
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `readiness-checklist`, `steps`
- **States:** Prospect; In diligence; Contract signed; Board delegation recorded; Setting up workspace; Setup status unknown; Setup failed; Workspace ready; Configuration in progress; Configuration blocked; Configuration validated; Integration checks failed; Integrations validated; Opening balance break; Opening balances tied out; Ready to activate; Active; Suspended; Exit in progress; Offboarded; Withdrawn
- **Actions:** Open view: Contract and activation record (A0; navigation only); Open view: Board delegations (A0; navigation only); Open view: Tenant provisioning (A0; navigation only); Open view: Readiness checklist (A0; navigation only); Open view: Opening balance tie-out (A0; navigation only); Open view: Activation gate (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a sponsor whose integration checks failed on the positive-pay file; when the implementation lead opens WS-17; then 'Integration checks failed' is first with 'Positive-pay file accepted: failed' and its owner.
  - **AC-2** Given a platform implementation lead; when they view the Activation gate; then 'Activate program' is unavailable with the reason 'Only the sponsor's authorized officer can activate'.

### UNB-UI-S-024 — Migration (workspace frame) (screen)
- **Requirement:** WS-18 MUST run one inbound migration of an existing sponsor book (WFM-26). Header: scope-trail (sponsor > migration run), the declared rollback boundary, the fencing epoch once allocated, and which system is the writer ('Legacy is the only writer' or 'Unbound is the only writer'). Views: 'Migration runs', 'Mapping review', 'Cutover authorization', 'Imported payee verification'. Landing (RCD-026): first, posting-stopping conditions: 'Fence status unknown' with the banner 'Unbound is not posting until the fence is confirmed' (UNB-RCN-WF-020), 'Compensating' with affected funds, 'Tie-out break' with the failing tie-out; then due items: imported payees still unverified (count and payments they block), parallel-run cycles remaining, post-cutover reconciliation sign-off pending. Then the run step track.
- **Traces to:** RCD-026, RCD-020, RR-REQ-X-061-r1, UNB-RCN-WF-020, WFM-26-T09, WFM-26-T10, WFM-26-T13, UNB-UI-G-006, UNB-UI-G-009, UNB-UI-G-010
- **Actors:** Migration operator, Sponsor controller, Sponsor CFO, Sponsor operations lead, Independent reconciler, Sponsor payee verifier
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `steps`, `metric-card`, `alert`
- **States:** Contracted; Inventoried; Mapped; Staged; Tie-out break; Reconciled; Parallel run; Parallel validated; Cutover authorized; Fence status unknown; Live on Unbound; Compensating; Verified; Archived; Rolled back; Abandoned; Migration correction
- **Actions:** Open view: Migration runs (A0; navigation only); Open view: Mapping review (A0; navigation only); Open view: Cutover authorization (A0; navigation only); Open view: Imported payee verification (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06
- **Acceptance criteria:**
  - **AC-1** Given the legacy fencing request returned no definite result; when the migration lead opens WS-18; then 'Fence status unknown' is first with the banner 'Unbound is not posting until the fence is confirmed' and a 'Check fence status' action; no posting-dependent action is enabled.
  - **AC-2** Given 1,140 imported payees unverified; when the landing renders; then the count is shown with the number of payments they block, and links to the verification queue.

### UNB-UI-S-025 — Support console (workspace frame) (screen)
- **Requirement:** WS-20 MUST give support agents the cases assigned to them and the assisted-change tool. Header: scope-trail (sponsor > case) and the authenticated caller panel (caller identity, assurance level achieved, channel, time). Views: 'Assisted change'; answering, reconsideration and security review open in the case workspace (S-009). Landing (RCD-026): first, the agent's triaged cases by SLA risk; then staged changes 'Awaiting your confirmation' with the confirmation expiry; then changes the customer denied ('Security review'), which are read-only for the agent. Agents hold at most A2 here; nothing in this workspace applies a change to a fund, participant or payee without verified-channel confirmation (UNB-RCN-ENT-016).
- **Traces to:** RCD-026, RCD-046, UNB-RCN-ENT-016, ADM-017, REQ-X-072, UNB-UI-G-009, UNB-UI-G-010, UNB-UI-S-009
- **Actors:** Support agent, Sponsor operations (staff-assisted)
- **Forma components:** `workspace-shell`, `navigation-shell`, `scope-trail`, `tabs`, `work-queue`, `status-lozenge`, `freshness`, `empty-state`, `skeleton`, `key-value-list`
- **States:** Received; In progress; Awaiting your confirmation; Security review; Resolved; Change requested
- **Actions:** Open view: Assisted change (A0; navigation only); Open case in the case workspace (A0; navigation only)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-06, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given an agent with a staged change whose confirmation expires in 2 hours; when they open WS-20; then the change appears with its expiry time after any SLA-breaching cases.
  - **AC-2** Given a case in 'Security review'; when the agent opens it; then it is read-only for the agent with the reason 'Under security review'.

### UNB-UI-S-026 — Recurring schedule management (screen)
- **Requirement:** Within WS-02, the 'Recurring schedules' view MUST list the user's schedules for the fund in a master-detail layout. List rows: charity (canonical name and EIN), amount (G-017; percentage schedules show percentage and base), cadence, next occurrence date (G-018), schedule version, date of the standing affirmation, and status. Detail: schedule facts; the occurrence history as a timeline with each created grant linked to grant detail (S-004); the adverse-occurrence counter against the suspension threshold, where a held occurrence counts and is skipped (RCD-043). Actions by state: Active -> Amend, Cancel; 'Suspended – needs your confirmation' -> Resume (requires an unticked explicit reconfirmation and the advisory-only statement verbatim, G-007), Cancel; 'This month's grant on hold' -> Cancel (the held occurrence is skipped by the system; there is no 'skip' or 'release hold' control); 'Suspended – authority review' -> Resume only for a successor with activated rights, read-only for others with the reason. Amending amount, recipient or purpose MUST show that a new schedule version and a new standing affirmation are created and take effect from the next occurrence. Cancel MUST state that grants already created continue and can be cancelled only by request from grant detail.
- **Traces to:** WFM-09-T07, WFM-09-T08, WFM-09-T09, WFM-09-T12, UNB-RCN-GRT-004, UNB-RCN-GRT-010, RR-UNB-GRANT-027-r1, UNB-GRANT-028, UNB-GRANT-003, RR-UNB-SUCC-008-r1, RR-REQ-X-008-r1, RCD-043, RCD-041, UNB-UI-G-005, UNB-UI-G-007, UNB-UI-G-017, UNB-UI-S-003, UNB-UI-S-004
- **Actors:** Donor, Advisory-privilege holder, Successor with activated rights
- **Forma components:** `master-detail`, `data-grid`, `record-header`, `identifier`, `status-lozenge`, `key-value-list`, `timeline`, `form`, `text-field`, `select`, `date-time-field`, `checkbox`, `dialog`, `validation-summary`, `mobile-action-bar`, `empty-state`
- **States:** Schedule submitted; Active; Occurrence due; This month's grant on hold; Suspended – needs your confirmation; Suspended – authority review; Completed; Canceled; Not accepted
- **Actions:** Amend schedule (A2; WFM-09-T08); Resume schedule (A2; WFM-09-T07); Cancel schedule (A2; WFM-09-T09); Resume as successor (A2; WFM-09-T12)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-07, FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a schedule 'Suspended – needs your confirmation' after the threshold was reached by a held occurrence; when the donor opens it; then the counter shows the held occurrence counted and skipped, and Resume requires ticking an unticked reconfirmation.
  - **AC-2** Given an advisor whose privilege was revoked 30 seconds ago; when they submit an amendment; then the command is refused ('Your access to this fund has changed'), no schedule version is created and the screen reloads scope.
  - **AC-3** Given a schedule in 'Suspended – authority review'; when a non-successor participant opens it; then Resume is unavailable with the reason 'Waiting for the authority review after a notice about the account holder'.

### UNB-UI-S-027 — Invitation acceptance (screen)
- **Requirement:** Opened from an invitation link, the 'Accept invitation' view MUST show, before any account step: who invited the person (name and role), the fund (name and reference, G-016), the offered role from the single role catalogue with what it permits and what it does not (for a delegation: scope and limits), the invitation expiry date (default 14 days, RCD-046), and the role terms with an unticked acknowledgment. Decision options: 'Accept invitation' (then identity proofing begins; status 'Verifying participant') or 'Decline'. For a delegation request the delegate sees 'Decline delegation request', which closes it. If the token is expired or revoked, or the inviter's authority ended, the view MUST show 'Invitation expired' or 'Invitation withdrawn' with 'Ask the inviter to send a new invitation' and MUST NOT offer accept. The invitee never sees fund balances or grant history before the role is active.
- **Traces to:** WFM-02-T14, WFM-02-T15, WFM-19-T06, UNB-PART-007, RR-UNB-PART-007-r1, ADM-014, UNB-RCN-WF-019, RCD-046, RR-UNB-ADV-001-r1, UNB-UI-G-005, UNB-UI-G-007, UNB-UI-S-006
- **Actors:** Invitee, Delegate, Legal representative
- **Forma components:** `record-header`, `identifier`, `key-value-list`, `prose`, `checkbox`, `button`, `dialog`, `status-lozenge`, `alert`, `validation-summary`, `mobile-action-bar`, `wizard`
- **States:** Invited; Verifying participant; Declined; Invitation expired; Invitation withdrawn; Requested; Awaiting documents; Request closed
- **Actions:** Accept invitation (A1; WFM-02-T14); Decline invitation (A1; WFM-02-T15); Decline delegation request (A1; WFM-19-T06)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a valid invitation for the role 'Advisor'; when the invitee ticks the role terms and accepts; then status becomes 'Verifying participant' and the next identity step opens.
  - **AC-2** Given an invitation whose inviter's authority ended before acceptance; when the invitee opens the link; then the view shows 'Invitation withdrawn' (neutral) and no accept control.
  - **AC-3** Given role terms not acknowledged; when the invitee presses Accept; then a validation summary links to the acknowledgment and nothing is submitted.

### UNB-UI-S-028 — Representative evidence upload (screen)
- **Requirement:** The 'Representative evidence' view MUST let a legal representative (agent under power of attorney, guardian, executor, trustee, entity signatory) supply the instrument that evidences their authority. It MUST show the required document list for the selected representative type (UNB-RCN-ENT-010) as a checklist, a file-upload queue with per-file progress, success, failure and unknown outcome, and a summary of what the representative will be able to do once the documents are accepted. Submit is enabled only when every required document is uploaded with a definite outcome. After submission the status reads 'Documents in review' and the view shows the expected review step. The representative MUST NOT see fund data or act on the fund until the delegation is Active.
- **Traces to:** WFM-19-T03, UNB-RCN-ENT-010, RR-UNB-PART-003-r1, UNB-UI-G-006, UNB-UI-G-012, UNB-UI-S-006
- **Actors:** Legal representative
- **Forma components:** `form`, `select`, `readiness-checklist`, `file-upload`, `operation-status`, `validation-summary`, `status-lozenge`, `button`, `mobile-action-bar`
- **States:** Awaiting documents; Documents in review; Verifying delegate; Verification result unknown; Not approved; Active
- **Actions:** Submit documents (A1; WFM-19-T03)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given an executor with letters testamentary uploaded; when they submit; then status becomes 'Documents in review'.
  - **AC-2** Given an upload whose outcome is unknown after a timeout; when the queue renders; then the file shows unknown with 'Check status', and Submit stays unavailable with the reason until resolved.

### UNB-UI-S-029 — Verified-channel confirmation (screen)
- **Requirement:** The 'Confirm a change on your verified channel' view MUST open only from the link sent to a verified channel on file, never from the channel on which the change was requested. It MUST show: the staged change as a before/after diff; the change reference and the hash it was staged under; who staged it (support agent role and staff reference), the channel and time of the request; for sensitive changes, the cooling-off end date (default 3 business days, RCD-046) and that the previous channel will be notified; for a payee or destination change, that the new destination still needs independent verification before any payment uses it (RR-REQ-X-005-r1). Options: 'Confirm this change' and 'I did not ask for this'. Confirm MUST be unavailable when the confirming person is the one who requested a payee change, with the reason. A change hash mismatch MUST result in 'No change was made' and the case returning to 'In progress'. 'I did not ask for this' places a protective hold and opens a security review.
- **Traces to:** WFM-25-T04, WFM-25-T06, UNB-RCN-ENT-016, ADM-017, RR-REQ-X-005-r1, REQ-X-072, UNB-RCN-WF-028, RCD-046, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-S-006
- **Actors:** Donor, Advisory-privilege holder
- **Forma components:** `verification-frame`, `diff-viewer`, `key-value-list`, `identifier`, `status-lozenge`, `button`, `dialog`, `alert`, `operation-status`, `mobile-action-bar`
- **States:** Awaiting your confirmation; Resolved; In progress; Security review
- **Actions:** Confirm this change (A2; WFM-25-T04); I did not ask for this (A1; WFM-25-T06)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given an email-address change staged by support; when the donor confirms from the verified phone channel; then the case becomes 'Resolved' and the previous channel is notified.
  - **AC-2** Given the staged change was edited after the link was sent; when the donor confirms; then the view states 'No change was made' and the case returns to 'In progress'.
  - **AC-3** Given the donor chooses 'I did not ask for this'; when the confirmation dialog is accepted; then status becomes 'Security review' (blocked) and the fund is protected.

### UNB-UI-S-030 — Fund application (screen)
- **Requirement:** The 'Open a fund' view MUST be a resumable wizard: applicant identity; fund name and product (release 1: standard donor-advised fund only, RCD-024); optional co-advisors and successors (invited after opening); agreement review showing the program agreement version; the exclusive-legal-control acknowledgment rendered verbatim (G-007) with an unticked checkbox (no pre-checked consent); review and submit. After submission it shows the status track: 'Application submitted' -> 'Verifying identity' -> 'In sponsor review' -> 'Open' or 'Not accepted'. 'Check result unknown' shows as unknown with the explanation that the sponsor is confirming, never as failure. If the agreement version is superseded before submission, the wizard returns to the agreement step and requires a fresh acknowledgment. Contributions and grants are not available until 'Open'. Withdraw is available until the sponsor decides.
- **Traces to:** WFM-02-T01, WFM-02-T02, ADM-002, ADM-014, UNB-RCN-ARC-002, UNB-SUCC-010, UNB-PART-007, RCD-024, UNB-UI-G-005, UNB-UI-G-007, UNB-UI-G-012, UNB-UI-G-014
- **Actors:** Applicant
- **Forma components:** `wizard`, `form`, `form-row`, `text-field`, `select`, `checkbox`, `prose`, `validation-summary`, `validation-message`, `steps`, `status-lozenge`, `dialog`, `operation-status`, `mobile-action-bar`
- **States:** Application draft; Application submitted; Verifying identity; Check result unknown; Under compliance review; In sponsor review; Open; Not accepted; Withdrawn; Blocked
- **Actions:** Submit application (A1; WFM-02-T01); Withdraw application (A1; WFM-02-T02)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-09, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given an applicant who has not ticked the exclusive-legal-control acknowledgment; when they press Submit; then nothing is submitted and the validation summary links to the acknowledgment.
  - **AC-2** Given the agreement version changed while the draft was open; when the applicant submits; then the wizard returns to the agreement step showing the new version and requires a fresh acknowledgment.
  - **AC-3** Given an identity check with no definite result; when the applicant views status; then it reads 'Check result unknown' (unknown), not 'Not accepted'.

### UNB-UI-S-031 — Fund closure request (screen)
- **Requirement:** The 'Close a fund' view MUST let an advisory-privilege holder request closure of an Open fund and follow it. Request step: the consequences (no new grant recommendations or contributions accepted per sponsor policy while closing; in-flight grants and payments complete; final fees posted before the residual; the balance must go to charities or another sponsor), and the current list of open obligations from the ledger (open reservations, payments in progress, outstanding checks, contributions not yet final) with freshness (G-003). Follow-up: the closure status track with each obligation's state, the date final fees post, and when 'Choose where the balance goes' opens, its window end date and the two choices (residual grants via Recommend a grant, S-003; or transfer to another sponsor, S-032). 'Cancel closure' is available only for a donor-requested closure and only until the residual is committed; it is never shown for dormancy, succession or sponsor-exit closures.
- **Traces to:** WFM-22-T15, WFM-22-T01, UNB-RCN-ENT-006, ADM-025, UNB-RCN-WF-014, RCD-040, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-S-003
- **Actors:** Advisory-privilege holder
- **Forma components:** `record-header`, `steps`, `obligation-panel`, `status-lozenge`, `freshness`, `key-value-list`, `dialog`, `button`, `mobile-action-bar`
- **States:** Closure requested; Settling open items; Closure blocked; Open items settled; Final fees posted; Choose where the balance goes; Residual grants in progress; Balance distributed; Closed; Closure cancelled
- **Actions:** Request closure (A1; creates the closure request accepted by WFM-22-T01); Cancel closure (A1; WFM-22-T15)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a donor-requested closure in 'Settling open items'; when the donor cancels; then status becomes 'Closure cancelled' and new recommendations are possible again.
  - **AC-2** Given a closure triggered by dormancy; when an advisor opens it; then no cancel control is shown and the reason names the trigger.

### UNB-UI-S-032 — Outbound transfer request (screen)
- **Requirement:** The 'Transfer to another sponsor' view MUST be reachable only while the closing fund is in 'Choose where the balance goes'. Form: receiving sponsoring organisation (search by EIN with IRS status and its as-of date), receiving fund reference and contact; transfer form: cash (default), in-kind only where both custodians accept and policy allows; consent to send the fund history package (unticked); restrictions and donor intent carried as non-binding text with the statement that the receiving sponsor is not bound by them; the advisory-only statement verbatim (G-007). Confirmation (G-005) states the amount is the residual after final fees, that the sponsor approves the transfer as a grant and checks the receiving sponsor's status and sanctions, and the next state 'Checking receiving sponsor'. Not available: choosing a non-sponsoring organisation, or in-kind where not permitted (shown with the reason).
- **Traces to:** WFM-22-T08, RR-UNB-WF-014-r1, UNB-RCN-ENT-007, UNB-RCN-GRT-010, RCD-040, UNB-UI-G-005, UNB-UI-G-007, UNB-UI-G-017
- **Actors:** Advisory-privilege holder
- **Forma components:** `form`, `search`, `combobox`, `segmented-control`, `checkbox`, `textarea`, `prose`, `freshness`, `dialog`, `validation-summary`, `status-lozenge`, `mobile-action-bar`
- **States:** Choose where the balance goes; Checking receiving sponsor; Residual grants in progress; Balance distributed; Closed
- **Actions:** Recommend outbound transfer (A1; WFM-22-T08)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-11, FORMA-GAP-14
- **Acceptance criteria:**
  - **AC-1** Given a fund in 'Choose where the balance goes'; when the donor recommends a cash transfer with history consent; then status becomes 'Checking receiving sponsor'.
  - **AC-2** Given a receiving organisation that is not a sponsoring organisation; when the donor selects it; then it cannot be chosen; the reason states only sponsoring organisations can receive a transfer.
  - **AC-3** Given history-package consent not ticked; when the donor submits; then the validation summary links to the consent; nothing is submitted.

### UNB-UI-S-033 — Support case status (screen)
- **Requirement:** The 'My requests and appeals' view MUST show each request with reference, subject, status, next acting party, and dates (received; reply due when 'Waiting on you'; response due for reconsideration, default 30 days; reopen window end after 'Resolved'). Detail: the timeline of public events, the original decision for a reconsideration and, once decided, the outcome from the defined set (Upheld, Overturned, Modified, Referred) with the explanation. Actions by state: 'Waiting on you' -> Provide information (text plus file upload); 'Resolved' within the reopen window -> Reopen; any open state -> Withdraw. Not available: editing the original request, choosing the reviewer, or reopening after 'Closed' (shown with the reason and 'Start a new request').
- **Traces to:** WFM-25-T11, WFM-25-T16, WFM-25-T17, UNB-RCN-ENT-016, UNB-RCN-WF-018, ADM-015, ADM-016, RCD-046, UNB-UI-G-016
- **Actors:** Donor, Advisor, Charity finance user
- **Forma components:** `master-detail`, `data-grid`, `record-header`, `identifier`, `status-lozenge`, `timeline`, `textarea`, `file-upload`, `dialog`, `mobile-action-bar`, `empty-state`
- **States:** Received; In progress; Under reconsideration; Waiting on you; Decision made; Resolved; Closed; Withdrawn
- **Actions:** Provide information (A1; WFM-25-T11); Reopen request (A1; WFM-25-T16); Withdraw request (A1; WFM-25-T17)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a reconsideration 'Waiting on you'; when the donor provides the requested document; then status becomes 'Under reconsideration'.
  - **AC-2** Given a case 'Closed' after the reopen window; when the requester opens it; then Reopen is unavailable with the reason and 'Start a new request' is offered.

### UNB-UI-S-034 — Grant batch approval (screen)
- **Requirement:** The 'Batch approval' view MUST list every grant in 'Approval recommended' for the scope, one row per grant: grant reference, fund, charity (name and EIN), amount (G-017; percentage grants show the amount that will be fixed at approval), recommended date, and the rule trace per item: recommending policy version and each gate outcome with its as-of time (recipient class permitted, benefit and purpose gate, related-party clear, supporting-organisation certification, eligibility current, sanctions clear, deep review current). Each row expands to the full trace. Independence (RCD-005, segregation of duties): grants where the approver is the originator, the recommender, a fund participant or a related party are excluded from the approver's selectable batch and listed separately with the reason. The approver may deselect items or decline an item (with reason) before approving. 'Approve N grants' opens a confirmation listing count, total amount and policy version (G-005); on submit each grant commits its own approval-and-reservation transaction, the batch is not all-or-nothing, and the result lists per item: Approved, returned to 'In review' (policy version or eligibility/sanctions changed) or 'Waiting for funds' (capacity). Not available: editing amounts, overriding a gate, or approving grants not in 'Approval recommended' (individual approvals use the case workspace).
- **Traces to:** WFM-08-T02, WFM-08-T07, WFM-08-T01, RCD-005, RR-REQ-X-002-r1, UNB-RCN-FIN-009, RR-UNB-GFUND-021-r1, RR-UNB-GFUND-013-r1, RR-AIN-003-r1, RR-UNB-GRANT-012-r1, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-017, UNB-UI-S-009
- **Actors:** Sponsor grants approver (board-authorized, independent of originator)
- **Forma components:** `data-grid`, `collection-toolbar`, `checkbox`, `disclosure`, `path-summary`, `status-lozenge`, `freshness`, `identifier`, `dialog`, `textarea`, `operation-status`, `mobile-action-bar`, `empty-state`
- **States:** Approval recommended; Approved; In review; Waiting for funds; Declined
- **Actions:** Approve selected grants (A4; WFM-08-T02); Decline grant with reason (A3; WFM-08-T07); Deselect grant from batch (A0; no transition)
- **Responsive:** phone-review-and-approve
- **Capability gaps:** FORMA-GAP-04, FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-26
- **Acceptance criteria:**
  - **AC-1** Given 40 recommended grants, one of which the approver recommended as an advisor; when the approver opens the batch; then 39 are selectable; the other is listed under 'You cannot approve' with the reason.
  - **AC-2** Given a batch of 10 where the policy version changed for 2 and capacity is short for 1; when the approver approves; then 7 show 'Approved', 2 'In review' with 'Policy changed', 1 'Waiting for funds'; nothing else is rolled back.
  - **AC-3** Given the submission returns no definite outcome; when the result renders; then each item shows unknown with 'Check status'; no item is shown approved and nothing is retried automatically.

### UNB-UI-S-035 — Check void and reissue (screen)
- **Requirement:** The 'Checks: void and reissue' view MUST list checks in 'Issue status unknown', 'Mailed – not yet cashed', 'Not yet cashed – contacting charity', 'Stale-dated', 'Void requested' and 'Voided', with check number, grant reference, payee, amount, issue date, days outstanding, and positive-pay acknowledgement freshness. Request void (lost or unmailed check) MUST capture the reason and is unavailable to the person who prepared the check run (segregation of duties). 'Void requested' is shown as unknown until the bank acknowledges the void file; reissue is unavailable until 'Voided'. For each voided check the view MUST show the escheat determination (RCD-047): jurisdiction, counsel rule version, and result ('No unclaimed-property duty' or 'Unclaimed property review'); this view has no re-credit control - re-credit happens only after the determination (automatically for a recorded not-reportable disposition, or by counsel in 'Checks: unclaimed property'). Reissue creates a linked replacement payment (check or ACH) that re-runs every release gate; the dialog shows the current eligibility, sanctions and destination checks with freshness and refuses if any fails. A check with unknown issue status is never reprinted under a new number until resolved.
- **Traces to:** WFM-11-T06, WFM-11-T19, WFM-11-T12, WFM-11-T14, WFM-11-T15, UNB-RCN-GRT-005, UNB-RCN-GRT-008, RR-REQ-X-004-r1, RR-REQ-X-005-r1, RCD-028, RCD-047, UNB-UI-G-003, UNB-UI-G-006, UNB-UI-G-017
- **Actors:** Sponsor treasury operations, Sponsor grants operations
- **Forma components:** `data-grid`, `collection-toolbar`, `active-filter-summary`, `status-lozenge`, `freshness`, `identifier`, `key-value-list`, `readiness-checklist`, `dialog`, `textarea`, `operation-status`, `pagination`
- **States:** Issue status unknown; Mailed – not yet cashed; Not yet cashed – contacting charity; Stale-dated; Void requested; Voided; Unclaimed property review; Voided – returned to fund; Replaced
- **Actions:** Request void (A3; WFM-11-T06); Reissue payment (A3; WFM-11-T19); Check void status (A0; query by idempotency key)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given a voided check whose escheat determination is pending; when treasury looks for a re-credit action; then none exists; the row shows 'Unclaimed property review' and links to the unclaimed property view.
  - **AC-2** Given a void file with no bank acknowledgement; when the operator opens the check; then it shows 'Void requested' (unknown) and Reissue is unavailable with the reason 'Waiting for bank to confirm the void'.
  - **AC-3** Given the operator prepared the check run; when they try to request a void; then the action is unavailable with the segregation-of-duties reason.

### UNB-UI-S-036 — OFAC blocked property report (screen)
- **Requirement:** The 'OFAC reports' view MUST list confirmed true matches in 'Blocked – OFAC report due' with the case reference, blocked property (amount, account, payment chain), the date blocked, and the report due date in business days (UNB-RCN-REG-003), sorted by due date; and items 'Blocked – reported' with the annual-report date. Report data is prefilled from the case and printable (Folio). Recording the filing requires the submission evidence (file and reference) and schedules the annual report; all report records are retained 10 years (RCD-007). Confirmed matches refused with no property ('Confirmed match – refused') appear in a separate section with rejected-transaction report data and its due date (UNB-RCN-WF-024). No control here releases property; release under licence happens only in the sanctions panel (WS-10).
- **Traces to:** WFM-12-T11, WFM-12-T10, WFM-12-T12, UNB-RCN-REG-003, UNB-RCN-GRT-006, UNB-RCN-WF-024, RCD-015, RCD-044, RCD-007, UNB-UI-G-016, UNB-UI-G-018
- **Actors:** Sanctions compliance officer
- **Forma components:** `data-grid`, `status-lozenge`, `identifier`, `key-value-list`, `file-upload`, `dialog`, `validation-summary`, `preview-surface`, `empty-state`
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-section`, `ef-print-table`, `ef-print-footer`
- **States:** Blocked – OFAC report due; Blocked – reported; Confirmed match – refused
- **Actions:** Record OFAC report filed (A3; WFM-12-T11); Print report data (A0; no transition)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FOLIO-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given a report due in 1 business day; when the officer opens the view; then it is first with 'Due in 1 business day' (attention) and the prefilled report data.
  - **AC-2** Given no submission evidence attached; when the officer records the filing; then the action is refused with a validation message; status stays 'Blocked – OFAC report due'.

### UNB-UI-S-037 — Payment release approval (screen)
- **Requirement:** The 'Manual release approval' view MUST list ACH payments in 'Release blocked' whose only failing gate is destination evidence that is ineligible for automated release (UNB-RCN-WF-023). For each item: grant and payment references, charity, amount, destination version and its evidence against the DestinationEvidence table, and a gate checklist showing every other release gate passing with its as-of time (eligibility current, sanctions clear, hold watermark current, funded). Items blocked by any hold, sanctions or stale eligibility appear read-only with the blocker and no approve control - a human can never override those. The approver MUST be independent of the payment preparer and of the destination requester; otherwise the action is unavailable with the reason. Authorize release records the hold publication version and eligibility snapshot; if any gate fails at commit the item stays 'Release blocked' with the reason. RCD-048: a destination with an unverified Notification of Change is not releasable.
- **Traces to:** WFM-10-T03, RR-REQ-X-005-r1, UNB-RCN-GRT-017, UNB-PAY-003, RR-AIN-003-r1, UNB-RCN-WF-023, RCD-048, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006
- **Actors:** Sponsor treasury release approver (independent of preparer)
- **Forma components:** `master-detail`, `data-grid`, `readiness-checklist`, `security-blockers`, `evidence-relationship`, `freshness`, `status-lozenge`, `identifier`, `dialog`, `textarea`, `operation-status`
- **States:** Release blocked; Release authorized
- **Actions:** Authorize release (A4; WFM-10-T03)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a blocked payment whose sole blocker is destination evidence ineligible for automation; when an independent approver authorizes; then status becomes 'Release authorized'.
  - **AC-2** Given a payment also blocked by a charity hold; when the approver opens it; then no approve control exists; the hold is shown as the blocker.
  - **AC-3** Given the approver prepared the payment; when they open it; then Authorize release is unavailable with the segregation-of-duties reason.

### UNB-UI-S-038 — Unclaimed property disposition (screen)
- **Requirement:** The 'Checks: unclaimed property' view MUST list voided checks in 'Unclaimed property review' and 'Reportable as unclaimed property' with check number, payee, amount, payee state, dormancy period and report date for that state, counsel rule version, and days to the report date. Counsel actions: record an alternative disposition (re-credit to the fund) with an attached counsel memo stating why no escheat duty applies (RCD-047), which requires sanctions clear and notifies charity and donor; or determine reportable with memo. The controller then records report and remittance with evidence; the controller cannot be the person who determined reportability. Not available: re-credit without a memo, batch disposition across checks, or any disposition of a sanctions-blocked check.
- **Traces to:** WFM-11-T16, WFM-11-T17, WFM-11-T18, UNB-RCN-GRT-005, RCD-028, RCD-047, UNB-RCN-ENT-005, CTL-044, RR-UNB-PAY-006-r1, UNB-UI-G-005, UNB-UI-G-017, UNB-UI-G-018
- **Actors:** Sponsor counsel, Controller
- **Forma components:** `data-grid`, `status-lozenge`, `identifier`, `key-value-list`, `file-upload`, `textarea`, `dialog`, `validation-summary`, `empty-state`
- **States:** Unclaimed property review; Reportable as unclaimed property; Voided – returned to fund; Sent to state as unclaimed property
- **Actions:** Record alternative disposition (re-credit) (A3; WFM-11-T16); Determine reportable (A3; WFM-11-T17); Record report and remittance (A3; WFM-11-T18)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a voided check in 'Unclaimed property review' with no counsel memo; when counsel tries to re-credit; then the action is refused with 'Attach counsel memo'.
  - **AC-2** Given the controller who determined reportability; when they record the remittance; then the action is unavailable with the segregation-of-duties reason.

### UNB-UI-S-039 — Form 8282 obligation tracker (screen)
- **Requirement:** The 'Noncash forms: 8282' view MUST list each noncash gift in 'Form 8282 due' with gift reference, property description, date received, date disposed, proceeds (G-017), donor, the filing due date and days remaining, and checklist items: acknowledgment issued, donor copy sent, filing receipt evidence. 'Record filed' is enabled only when all three are present and moves the gift to 'Complete'. Overdue obligations are listed first.
- **Traces to:** WFM-04-T31, UNB-RCN-FIN-003, UNB-UI-G-017, UNB-UI-G-018
- **Actors:** Sponsor tax compliance officer
- **Forma components:** `data-grid`, `readiness-checklist`, `status-lozenge`, `identifier`, `file-upload`, `dialog`, `empty-state`
- **States:** Form 8282 due; Complete
- **Actions:** Record Form 8282 filed (A4; WFM-04-T31)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a 8282 obligation with the donor copy not yet sent; when the officer tries to record filing; then the action is unavailable with 'Donor copy not sent'.
  - **AC-2** Given all evidence attached; when the officer records filing; then the gift becomes 'Complete'.

### UNB-UI-S-040 — Form 8283 donee signature (screen)
- **Requirement:** The 'Noncash forms: 8283' view MUST list noncash gifts accepted by the sponsor ('Settled in custody' or 'Held in kind') that need a Section B donee signature, with property description, lot identifiers, date of receipt and donor. The signing dialog MUST state verbatim that the signature acknowledges receipt of the property described on the date shown and is not agreement with the claimed value, and MUST NOT display or ask to confirm any appraised value. Sign is available only to an authorized donee signatory and only when sponsor acceptance and lot identifiers are recorded; the signed record (signatory, date, property, lots) prints via Folio.
- **Traces to:** WFM-04-T26, WFM-04-T27, UNB-RCN-FIN-002, RR-UNB-INV-012-r1, UNB-UI-G-005, UNB-UI-G-007
- **Actors:** Sponsor authorized donee signatory
- **Forma components:** `data-grid`, `key-value-list`, `identifier`, `status-lozenge`, `dialog`, `prose`, `readiness-checklist`, `empty-state`
- **Folio elements:** `ef-print-document`, `ef-print-section`, `ef-print-footer`
- **States:** Settled in custody; Held in kind
- **Actions:** Sign Form 8283 Section B (settled) (A4; WFM-04-T26); Sign Form 8283 Section B (held in kind) (A4; WFM-04-T27)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11, FOLIO-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given a gift without lot identifiers; when the signatory opens it; then Sign is unavailable with 'Lot identifiers not recorded'.
  - **AC-2** Given a signable gift; when the signing dialog opens; then it contains the receipt-not-value statement and no value field.

### UNB-UI-S-041 — Month-end close certification (screen)
- **Requirement:** The 'Close certification' view MUST show, for the selected period, a table of every reconciliation class - GL to fund subledger, custody positions, bank cash, NAV and units, fees, contributions, grants and payments - with status (Pass, Unknown/'Evidence missing', Break), matched count, unexplained difference amount and age, owner and evidence link. Below it: unexplained breaks with amount, age and affected funds, and any isolated scope (funds excluded from certification and statement publication, RCD-020); sampled-attestation completion (attested n of N, sample at least 5% of the month's automated passes, defects found) - certification is blocked until the sample is attested (RCD-037); the frozen snapshot (as-of, known-at, hash). The certifier MUST NOT be the preparer of the close: for the preparer 'Certify close' is unavailable with the reason. Certify close approves publication of the period's statements (RCD-038) and locks the period; the dialog lists funds whose statements will be withheld. Reject returns the period to 'Reconciling' with a reason. After publication: Approve closed-period restatement (requires a second approver and materiality exceeded) and Issue corrected statements (reason required).
- **Traces to:** WFM-16-T11, WFM-16-T15, WFM-16-T16, WFM-16-T07, WFM-16-T10, REQ-X-040, REQ-X-032, UNB-ACC-022, UNB-ACC-017, REQ-X-010, RR-UNB-ACC-018-r1, UNB-NAV-043, RCD-020, RCD-037, RCD-038, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-017, UNB-UI-S-007
- **Actors:** Controller (not the preparer), Second approver (restatement)
- **Forma components:** `record-header`, `dense-ledger`, `status-lozenge`, `freshness`, `metric-card`, `progress-bar`, `readiness-checklist`, `identifier`, `key-value-list`, `dialog`, `textarea`, `operation-status`
- **States:** Reconciling; Evidence missing; Breaks open; Reconciled; Awaiting sampled attestation; Snapshot frozen; Certified; Statements generating; Statements partly published; Statements published; Period restated; Corrected statement issued
- **Actions:** Certify close (A4; WFM-16-T11); Reject certification (A4; WFM-16-T11 failure CertifierRejects); Approve closed-period restatement (A4; WFM-16-T15); Issue corrected statements (A4; WFM-16-T16)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-25, FORMA-GAP-24
- **Acceptance criteria:**
  - **AC-1** Given the sampled attestation is 180 of 210 complete; when the controller opens certification; then Certify close is unavailable with 'Sampled attestation incomplete (180 of 210)'.
  - **AC-2** Given the controller prepared the close; when they open certification; then Certify close is unavailable with 'You prepared this close; another controller must certify'.
  - **AC-3** Given a bank cash break isolated to 3 funds; when the certifier certifies; then the dialog lists the 3 funds whose statements are withheld, and certification proceeds for the rest.

### UNB-UI-S-042 — NAV review and approval (screen)
- **Requirement:** The 'Unit pricing' view MUST list valuation runs by pool and valuation date with status. Run detail: inputs (positions, cash, prices with source, pricing-hierarchy level and freshness; stale, missing, conflicting or outlier prices flagged, never forward-filled); calculated NAV and unit price in fixed precision; tolerance checks each with result and values - day-over-day change versus tolerance, reconciliation breaks in scope, units reconcile - from the deterministic check (WFM-14-T08). The run header MUST distinguish how a NAV became publishable: 'Published by policy' (in tolerance, delegated policy version and the next-business-day sampled review status, RCD-035) versus 'Approved by <checker>' (human A4). Out-of-tolerance runs are in 'Exception hold' and need a human. Actions: Approve NAV (independent checker, not the preparer; unavailable while any material break is in scope); Override tolerance (requires a written market-move explanation and a second approver; both shown); Suspend valuation (gates unit transactions); Approve restatement of a corrected NAV (impact per fund, grant, fee and statement shown; closed periods need restatement authority). Not available: editing a price or NAV value in this view.
- **Traces to:** WFM-14-T06, WFM-14-T10, WFM-14-T11, WFM-14-T18, WFM-14-T08, WFM-14-T12, UNB-NAV-004, UNB-NAV-046, UNB-INV-018, UNB-NAV-008, UNB-NAV-005, UNB-NAV-011, UNB-NAV-038, UNB-NAV-041, UNB-NAV-043, UNB-NAV-010, RR-UNB-ACC-018-r1, RCD-035, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-017
- **Actors:** Independent NAV checker, Pool valuation committee delegate, Second approver
- **Forma components:** `master-detail`, `data-grid`, `dense-ledger`, `record-header`, `status-lozenge`, `freshness`, `key-value-list`, `provenance-trail`, `readiness-checklist`, `dialog`, `textarea`, `operation-status`
- **States:** Scheduled; Awaiting inputs; Input exception; Inputs validated; Calculated; Exception hold; Awaiting approval; Approved; Published; Valuation suspended; No valuation (calendar); Correction opened; Impact assessed; Correction below materiality; Restated
- **Actions:** Approve NAV (A4; WFM-14-T11); Override tolerance (A4, second approver; WFM-14-T10); Suspend valuation (A4; WFM-14-T06); Approve NAV restatement (A4; WFM-14-T18)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-24
- **Acceptance criteria:**
  - **AC-1** Given an in-tolerance run published under delegated policy; when the controller opens it; then the header reads 'Published by policy' with the policy version and the sampled-review status, not a checker name.
  - **AC-2** Given a run with a 3.1% day-over-day move against a 2% tolerance; when the checker opens it; then it is in 'Exception hold' and Approve is unavailable; Override requires an explanation and a second approver.
  - **AC-3** Given the checker calculated the run; when they open it; then Approve NAV is unavailable with the segregation-of-duties reason.

### UNB-UI-S-043 — Pool liquidity buffer (screen)
- **Requirement:** The 'Pool liquidity' view MUST show each pool's settled cash, the buffer floor (sponsor-set, default 2% of pool value, RCD-017) with the pool value as-of the last published NAV, the shortfall, and the grants waiting on liquidity (count and amount). For a pool in 'Liquidity buffer below floor' it shows the proposed replenishment (sale or rebalance instruction) and whether it is within mandate boundaries. Approve is unavailable if outside mandate or for the proposer (segregation of duties). Grants never draw on other grants' reservations; NAV is not changed here.
- **Traces to:** WFM-14-T14, WFM-14-T13, RCD-017, UNB-INV-015, UNB-NAV-036, RR-REQ-X-002-r1, UNB-UI-G-003, UNB-UI-G-017
- **Actors:** Sponsor investment operations (treasury)
- **Forma components:** `data-grid`, `metric-card`, `freshness`, `status-lozenge`, `key-value-list`, `dialog`, `operation-status`
- **States:** Liquidity buffer below floor; Cycle complete
- **Actions:** Approve buffer replenishment (A4; WFM-14-T14)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a pool at 1.2% cash against a 2% floor; when treasury opens it; then it shows the shortfall, waiting grants and the proposed instruction.
  - **AC-2** Given a proposal outside mandate boundaries; when the approver opens it; then Approve is unavailable with 'Outside investment mandate'.

### UNB-UI-S-044 — Reconciliation attestation review (screen)
- **Requirement:** The 'Reconciliation and attestation' view MUST show the period's attestation sample (at least 5% of automated passes, RCD-020) with the selection method and seed, progress n of N, and for each sampled item the two independent sources side by side (e.g. bank statement line and ledger entry) with the matching rule applied. The reviewer attests each item Pass or records a defect with a note; accepting the full sample freezes the snapshot in the same transaction. A defect returns the period to 'Breaks open'. The reviewer MUST be outside the payment chain and not the preparer; otherwise the view is read-only with the reason. Continuous reconciliation break cases are listed above the sample with links to the case workspace.
- **Traces to:** WFM-16-T09, WFM-16-T08, RCD-020, RCD-037, REQ-X-010, GOV-AUTH-014, UNB-UI-G-003, UNB-UI-S-009
- **Actors:** Independent finance reviewer
- **Forma components:** `data-grid`, `comparison-pairs`, `evidence-relationship`, `progress-bar`, `status-lozenge`, `segmented-control`, `textarea`, `dialog`, `operation-status`
- **States:** Awaiting sampled attestation; Snapshot frozen; Breaks open
- **Actions:** Attest sample (A4; WFM-16-T09); Record sample defect (A4; WFM-16-T09 failure SampleDefectFound)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a reviewer who approved payments this month; when they open the sample; then the view is read-only with 'You are in the payment chain for this period'.
  - **AC-2** Given a sampled item whose sources disagree; when the reviewer records a defect; then the period becomes 'Breaks open' and the snapshot is not frozen.

### UNB-UI-S-045 — Account-type classification register (screen)
- **Requirement:** The 'Account-type classification' view MUST list each product or account type with its classification (donor-advised fund or not), the counsel basis memo, register version and effective date, and which tax year it applies to. Release 1: only the standard DAF product is offered (RCD-024); any other product type shows 'Not classified' and blocks annual reporting. The diff from the prior register version is shown. Approve register is available to sponsor counsel who did not author the version.
- **Traces to:** WFM-17-T03, UNB-RCN-ENT-011, RCD-024, UNB-UI-G-005
- **Actors:** Sponsor counsel
- **Forma components:** `data-grid`, `diff-viewer`, `status-lozenge`, `key-value-list`, `file-upload`, `dialog`
- **States:** Blocked: classification register; Checking classification register
- **Actions:** Approve classification register (A4; WFM-17-T03)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given counsel who authored the register version; when they open it; then Approve is unavailable with the segregation-of-duties reason.
  - **AC-2** Given a product type not classified; when the annual run checks the register; then the year shows 'Blocked: classification register'.

### UNB-UI-S-046 — Annual DAF reporting workspace (screen)
- **Requirement:** The 'Annual DAF package' view MUST show, for one tax year, the Schedule D Part I support package (UNB-UI-D-004): number of DAFs, aggregate value at year end, aggregate contributions and grants, each with the ledger tie-out (package value, ledger value, difference) and the locked periods it draws from. A step track shows preparer, reviewer and signer (three distinct people) with dates. Actions by state: Record not applicable (with applicability evidence); Prepare (preparer, CPA-approved data dictionary required); Review or Return to preparer (not the preparer); Sign (authorized officer of the legal sponsor, not the preparer, signer authority current); Record filing (submission evidence); Record acceptance or rejection (evidence); Reopen for correction after rejection; Open amendment (reason) and Record amended filing accepted. A restatement of a locked period invalidates the package back to 'Awaiting locked year'. No AI signing or filing.
- **Traces to:** WFM-17-T04, WFM-17-T09, WFM-17-T10, WFM-17-T11, WFM-17-T12, WFM-17-T14, WFM-17-T15, WFM-17-T17, WFM-17-T18, WFM-17-T19, WFM-17-T13, RR-REQ-X-060-r1, UNB-RCN-ENT-012, UNB-ACC-021, UNB-ACC-022, UNB-NAV-043, UNB-RCN-WF-007, UNB-UI-D-004, UNB-UI-G-011, UNB-UI-G-017
- **Actors:** Tax preparer, Qualified reviewer, Authorized officer of the legal sponsor, Sponsor tax staff, Tax director, Sponsor counsel
- **Forma components:** `record-header`, `steps`, `dense-ledger`, `status-lozenge`, `key-value-list`, `file-upload`, `dialog`, `textarea`, `preview-surface`, `operation-status`
- **Folio elements:** `ef-print-document`, `ef-print-table`, `ef-print-footer`
- **States:** Checking classification register; Awaiting locked year; Package built; Does not reconcile; Reconciled to ledger; Prepared; Reviewed; Signed; Filed, awaiting acceptance; Rejected by IRS; Accepted; Not applicable; Amendment in progress; Amended return filed
- **Actions:** Record not applicable (A4; WFM-17-T04); Prepare package (A3; WFM-17-T09); Review package (A4; WFM-17-T10); Sign return (A4; WFM-17-T11); Record filing (A3; WFM-17-T12); Record acceptance (A3; WFM-17-T14); Record rejection (A3; WFM-17-T15); Reopen for correction (A3; WFM-17-T17); Open amendment (A4; WFM-17-T18); Record amended filing accepted (A4; WFM-17-T19)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-09, FORMA-GAP-11, FOLIO-GAP-12
- **Acceptance criteria:**
  - **AC-1** Given the preparer of the package; when they open the reviewed package; then Sign is unavailable with the segregation-of-duties reason.
  - **AC-2** Given a restatement of March after the package was prepared; when the restatement is approved; then the package returns to 'Awaiting locked year' and the prior preparation is shown as superseded.

### UNB-UI-S-047 — Legal hold management (screen)
- **Requirement:** The 'Legal holds' view MUST list holds with matter reference, requester, scope, record count affected, date placed and review date. Placing a hold requires a recorded scope built from parties, funds, record classes and date range, and states that the hold also covers future corrections and linked evidence. Release is available only to legal counsel other than the hold requester and states that retention expiry is recomputed; already-expired records re-enter disposition review. Not available: release by the requester, or deleting any held record.
- **Traces to:** WFM-18-T13, WFM-18-T14, RR-REQ-X-036-r1, RCD-007, REQ-X-038, REQ-X-036, UNB-UI-G-005
- **Actors:** Legal counsel
- **Forma components:** `data-grid`, `rule-builder`, `status-lozenge`, `key-value-list`, `dialog`, `textarea`, `empty-state`
- **States:** Legal hold; In retention; Eligible for disposition review; Disposition approved
- **Actions:** Place legal hold (A4; WFM-18-T13); Release legal hold (A4; WFM-18-T14)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given counsel who requested the hold; when they open it; then Release is unavailable with the segregation-of-duties reason.
  - **AC-2** Given a hold placed on records in 'Disposition approved'; when the hold is recorded; then those records show 'Legal hold' and are excluded from execution.

### UNB-UI-S-048 — Retention disposition batch approval (screen)
- **Requirement:** The 'Disposition batches' view MUST present class-level batches (RCD-007), never per-record review: retention class, catalog version, record count, effective retention rule (maximum of all classes, 7-year floor, OFAC 10 years), exclusions (holds, dependencies, unknown safety facts) and the sampled verification result. Approval requires a records manager and a second approver, both shown with their decision state; neither may be the batch preparer. After approval, execution is deterministic; 'Disposition outcome unknown' shows with 'Check status'.
- **Traces to:** WFM-18-T15, WFM-18-T17, WFM-18-T18, RR-REQ-X-036-r1, RCD-007, REQ-X-038, UNB-UI-G-006
- **Actors:** Records manager, Second approver
- **Forma components:** `data-grid`, `key-value-list`, `status-lozenge`, `readiness-checklist`, `dialog`, `operation-status`
- **States:** Eligible for disposition review; Disposition approved; Disposition outcome unknown; Disposed
- **Actions:** Approve disposition batch (A4, second approver; WFM-18-T15)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11, FORMA-GAP-24
- **Acceptance criteria:**
  - **AC-1** Given a batch whose sampled verification is not done; when the records manager opens it; then Approve is unavailable with 'Sampled verification not done'.
  - **AC-2** Given one approver has approved; when the same person tries to approve again; then the action is unavailable with 'A different second approver is required'.

### UNB-UI-S-049 — Fund closure workspace (screen)
- **Requirement:** The 'Closures and transfers' view MUST list closing funds with trigger (donor request, dormancy, succession, sponsor exit), state, age, residual window end and trailing destination. Detail: obligation checklist (open reservations, payment attempts, outstanding checks, provisional contributions, blocking cases), final fee posting, residual disposition, transfer review result, and late events. For 'Closure blocked' by an outstanding check, a finance officer may move the check to the outstanding-instrument liability, which explains that a later void re-credit routes to the trailing destination (RCD-040). The default residual disposition opens the charitable-default panel (WS-10). Not available: reopening a closed fund.
- **Traces to:** WFM-22-T04, UNB-RCN-ENT-006, RCD-028, RCD-040, RR-REQ-X-083-r1, UNB-RCN-WF-015, UNB-UI-G-005, UNB-UI-S-009
- **Actors:** Sponsor finance officer, Sponsor operations
- **Forma components:** `master-detail`, `data-grid`, `obligation-panel`, `status-lozenge`, `timeline`, `key-value-list`, `dialog`
- **States:** Closure requested; Settling open items; Closure blocked; Open items settled; Final fees posted; Choose where the balance goes; Checking receiving sponsor; Residual grants in progress; Balance distributed; Closed; Archived; Late item after archive
- **Actions:** Transfer outstanding check to liability (A4; WFM-22-T04)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11, FORMA-GAP-15
- **Acceptance criteria:**
  - **AC-1** Given a fund blocked only by an outstanding check; when a finance officer transfers it to liability; then status becomes 'Settling open items'.
  - **AC-2** Given the finance officer issued that check; when they open the action; then it is unavailable with the segregation-of-duties reason.

### UNB-UI-S-050 — Board delegation register (screen)
- **Requirement:** The 'Board delegations' view MUST list each board resolution with delegated authority (grant approval, gift acceptance, policy approval, charitable default), delegate role, limits, effective and expiry dates, evidence and the verified-channel confirmation by a second sponsor officer. It MUST show the routine-approval delegation flag (RCD-005), default 'Not delegated', and the policy versions that depend on each delegation. Expiring delegations (within 60 days) are listed first. Recording requires the resolution evidence, effective dating and a within-bylaws check. When a delegation lapses, dependent policies show 'Suspended' and can be reinstated after renewal by a principal other than the one who recorded the renewal.
- **Traces to:** WFM-01-T04, WFM-24-T15, WFM-01-T18, REQ-X-064, RCD-005, GOV-AUTH-001, UNB-RCN-PLT-005, UNB-LEG-005, UNB-RCN-WF-016, UNB-UI-G-005
- **Actors:** Sponsor corporate secretary
- **Forma components:** `data-grid`, `status-lozenge`, `key-value-list`, `file-upload`, `date-time-field`, `dialog`, `validation-summary`
- **States:** Contract signed; Board delegation recorded; Suspended; Active
- **Actions:** Record board delegation (A4; WFM-01-T04); Reinstate policy after delegation renewed (A4; WFM-24-T15)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a resolution without second-officer confirmation; when the secretary records it; then it is refused; state stays 'Contract signed'.
  - **AC-2** Given routine-approval delegation not recorded; when anyone views the register; then the flag reads 'Not delegated'.

### UNB-UI-S-051 — Opening balance tie-out workspace (screen)
- **Requirement:** The 'Opening balance tie-out' view MUST show per account (custody positions, bank cash, GL, and for migrating sponsors the WFM-26 staged balances) the opening balance, the external statement value, the difference and status. A break is resolved only by a correction at source import or an explained opening adjustment with evidence, approved by the sponsor controller (not the preparer). There is no field to edit a balance directly (no fabricated history).
- **Traces to:** WFM-01-T15, WFM-26-T05, WFM-01-T14, WFM-26-T04, RCD-020, RR-REQ-X-061-r1, UNB-UI-G-017
- **Actors:** Sponsor controller
- **Forma components:** `dense-ledger`, `status-lozenge`, `freshness`, `file-upload`, `textarea`, `dialog`
- **States:** Opening balance break; Integrations validated; Opening balances tied out; Tie-out break; Staged
- **Actions:** Approve opening balance explanation (A4; WFM-01-T15); Resolve migration tie-out break (A4; WFM-26-T05)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a break without explanation evidence; when the controller approves; then the action is refused.
  - **AC-2** Given the controller prepared the adjustment; when they open it; then Approve is unavailable with the segregation-of-duties reason.

### UNB-UI-S-052 — Sponsor activation gate (screen)
- **Requirement:** The 'Activation gate' view MUST show the activation preconditions as a checklist with evidence and as-of: contract current, board delegation current, legal-sponsor disclosure present, tenant isolation suite passed, opening balances tied out, finance readiness certified (by whom), staff roles assigned (UNB-RCN-WF-009). 'Certify finance readiness' is for the sponsor finance officer. 'Activate program' is for the sponsor's authorized officer, who MUST differ from the finance certifier. Activation failure sends the sponsor back with the reason (contract expired, delegation expired, configuration changed since validation). For a suspended sponsor, 'Reinstate program' requires current contract and delegation.
- **Traces to:** WFM-01-T16, WFM-01-T17, WFM-01-T20, REQ-X-064, REQ-X-071, RR-UNB-PLAT-008-r1, UNB-RCN-WF-009, UNB-UI-G-014, RCD-005, UNB-UI-G-005
- **Actors:** Sponsor finance officer, Sponsor authorized officer
- **Forma components:** `readiness-checklist`, `security-blockers`, `status-lozenge`, `freshness`, `key-value-list`, `dialog`, `operation-status`
- **States:** Opening balances tied out; Ready to activate; Active; Suspended
- **Actions:** Certify finance readiness (A4; WFM-01-T16); Activate program (A4; WFM-01-T17); Reinstate program (A4; WFM-01-T20)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given the officer who certified finance readiness; when they try to activate; then Activate is unavailable with 'The finance certifier cannot activate'.
  - **AC-2** Given a contract that expired after certification; when the officer activates; then activation is refused and the sponsor returns to 'In diligence' with the reason.

### UNB-UI-S-053 — Sponsor contract and activation record (screen)
- **Requirement:** The 'Contract and activation record' view MUST show the sponsor lifecycle record: diligence, executed contract (counterparties, effective and expiry dates, responsibility matrix, exemption-status evidence and as-of), exit notice and offboarding. Actions: start diligence; record executed contract; withdraw onboarding; initiate exit (each fund then enters closure with trigger 'sponsor exit'); complete exit (only when every fund is closed or transferred, governed export delivered and retention preserved). The contract MUST name the legal sponsor as holder of exclusive legal control.
- **Traces to:** WFM-01-T01, WFM-01-T02, WFM-01-T03, WFM-01-T21, WFM-01-T22, REQ-X-064, UNB-LEG-002, ADM-001, UNB-TEN-011, UNB-TEN-016, UNB-RCN-ENT-006
- **Actors:** Platform implementation lead, Sponsor authorized signatory, Sponsor authorized officer
- **Forma components:** `record-header`, `timeline`, `key-value-list`, `file-upload`, `readiness-checklist`, `dialog`
- **States:** Prospect; In diligence; Contract signed; Active; Suspended; Exit in progress; Offboarded; Withdrawn
- **Actions:** Start diligence (A2; WFM-01-T01); Record executed contract (A4; WFM-01-T02); Withdraw onboarding (A2; WFM-01-T03); Initiate sponsor exit (A4; WFM-01-T21); Complete sponsor exit (A4; WFM-01-T22)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given 12 funds still open; when the officer completes exit; then it is refused with 'Funds remain open (12)'.
  - **AC-2** Given exemption status evidence not current; when the contract is recorded; then it is refused and the sponsor stays 'In diligence'.

### UNB-UI-S-054 — Sponsor implementation readiness checklist (screen)
- **Requirement:** The 'Readiness checklist' view MUST show configuration and integration checks with result and as-of: brand manifest accessible, protected terms unaltered, legal-sponsor disclosure present, initial policy approved, custody feed reconciles, payment-rail sandbox round trip, positive-pay file accepted, screening provider reachable. When 'Integration checks failed', the engineer may retry validation after fixing; results never auto-pass.
- **Traces to:** WFM-01-T13, WFM-01-T10, WFM-01-T12, REQ-X-046, REQ-X-071
- **Actors:** Platform integration engineer
- **Forma components:** `readiness-checklist`, `status-lozenge`, `freshness`, `diagnostic-status`, `button`
- **States:** Configuration validated; Integration checks failed; Integrations validated; Configuration blocked
- **Actions:** Retry integration validation (A2; WFM-01-T13)
- **Responsive:** desktop-first
- **Acceptance criteria:**
  - **AC-1** Given the screening provider unreachable; when checks run; then the item shows failed and status 'Integration checks failed'.
  - **AC-2** Given an engineer retries; when validation runs; then the result reflects the new check, never the old pass.

### UNB-UI-S-055 — Tenant provisioning console (screen)
- **Requirement:** The 'Tenant provisioning' view MUST show provisioning status, isolation profile (RCD-004 Profile B), release, isolation suite results and the idempotency key. 'Setup status unknown' is resolved by query only; no blind re-run control exists. Retry from 'Setup failed' requires the failure cause to be recorded.
- **Traces to:** WFM-01-T08, WFM-01-T07, RR-UNB-PLAT-008-r1, UNB-TEN-011, RCD-004, UNB-UI-G-006
- **Actors:** Platform operations engineer
- **Forma components:** `key-value-list`, `status-lozenge`, `operation-status`, `diagnostic-status`, `textarea`, `dialog`
- **States:** Setting up workspace; Setup status unknown; Setup failed; Workspace ready
- **Actions:** Retry tenant provisioning (A2; WFM-01-T08)
- **Responsive:** desktop-first
- **Acceptance criteria:**
  - **AC-1** Given 'Setup status unknown'; when the engineer opens it; then only 'Check status' is offered.
  - **AC-2** Given 'Setup failed' with no cause recorded; when the engineer retries; then it is refused until a cause is recorded.

### UNB-UI-S-056 — Imported payee verification queue (screen)
- **Requirement:** The 'Imported payee verification' view MUST present imported payee destinations in status 'imported_unverified' as a work queue, one record at a time: charity (canonical identity and EIN), legacy destination, the evidence imported with it and the evidence still required by the DestinationEvidence table (RR-REQ-X-005-r1), and the payments and schedules this destination currently blocks. Imported payees remain blocked from any payment until verified. The verifier MUST be independent of whoever requested or imported the destination; for them 'Verify payee' is unavailable with the reason. Verifying creates a new verified destination version; it never edits the imported record. There is deliberately no select-all, multi-select or bulk approve (bulk approval of thousands of records is not allowed). The queue header shows remaining count and payments blocked.
- **Traces to:** WFM-26-T12, RR-REQ-X-061-r1, RR-REQ-X-005-r1, RCD-028, UNB-UI-G-003, UNB-UI-G-005
- **Actors:** Sponsor payee verifier (independent of requester)
- **Forma components:** `work-queue`, `record-header`, `identifier`, `evidence-relationship`, `readiness-checklist`, `status-lozenge`, `freshness`, `key-value-list`, `metric-card`, `dialog`, `empty-state`
- **States:** Live on Unbound
- **Actions:** Verify imported payee (A4; WFM-26-T12)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-07, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a verifier who imported the destination file; when they open a record from it; then 'Verify payee' is unavailable with 'You imported this destination; another verifier must verify it'.
  - **AC-2** Given a record missing the bank-letter evidence required by the table; when the verifier opens it; then the evidence checklist shows the missing item and Verify is unavailable.
  - **AC-3** Given 1,140 unverified payees; when the verifier looks for a bulk action; then none exists; records are verified one at a time and the count decreases by one per verification.

### UNB-UI-S-057 — Migration control console (screen)
- **Requirement:** The 'Migration runs' view MUST show one migration run: record header with run reference and state; declared rollback boundary and compensation procedure (both required before inventory); inventory of legacy data classes and in-flight items (pending ACH, outstanding checks, Unknown payments, open flags, documents) with counts; tie-out results per class (fund balances to legacy statements, pending items mapped, open flags imported, custody and bank) with differences; parallel-run variance per cycle against tolerance; after cutover, the fence epoch, writer status and post-cutover reconciliation. Actions by state: Inventory legacy book (migration operator); Approve compensating entries while 'Compensating' (sponsor controller, not the preparer; entries must balance; forward compensation only, no history rewrite); Sign off post-cutover reconciliation (an independent reconciler who did not operate the migration; balances tie and provider acknowledgements received; an unresolved discrepancy moves the run to 'Compensating'); Abandon migration (sponsor CFO, only before cutover; staging evidence retained, UNB-RCN-WF-020); Record post-archive mapping defect (linked correction, compensating entries). While 'Fence status unknown', every posting-dependent action is unavailable with the banner 'Unbound is not posting until the fence is confirmed'.
- **Traces to:** WFM-26-T01, WFM-26-T14, WFM-26-T15, WFM-26-T17, WFM-26-T18, WFM-26-T04, WFM-26-T07, WFM-26-T13, RR-REQ-X-061-r1, UNB-RCN-WF-020, GOV-AUTH-014, UNB-TEN-011, UNB-UI-G-005, UNB-UI-G-006, UNB-UI-G-017
- **Actors:** Migration operator, Sponsor controller, Independent reconciler, Sponsor CFO
- **Forma components:** `record-header`, `identifier`, `steps`, `dense-ledger`, `data-grid`, `status-lozenge`, `freshness`, `key-value-list`, `alert`, `dialog`, `textarea`, `operation-status`
- **States:** Contracted; Inventoried; Mapped; Staged; Tie-out break; Reconciled; Parallel run; Parallel validated; Fence status unknown; Live on Unbound; Compensating; Verified; Archived; Abandoned; Migration correction
- **Actions:** Inventory legacy book (A2; WFM-26-T01); Approve compensating entries (A4; WFM-26-T14); Sign off post-cutover reconciliation (A4; WFM-26-T15); Abandon migration (A4; WFM-26-T17); Record post-archive mapping defect (A4; WFM-26-T18)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-08, FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given no rollback boundary declared; when the operator inventories the legacy book; then the action is refused with 'Declare the rollback boundary and compensation procedure first'.
  - **AC-2** Given a reconciler who ran the import; when they open the post-cutover sign-off; then Sign off is unavailable with 'You operated this migration; an independent reconciler must sign off'.
  - **AC-3** Given a run 'Live on Unbound'; when the CFO looks for Abandon; then it is not offered; the reason states that after cutover only compensation is possible.

### UNB-UI-S-058 — Migration cutover authorization (screen)
- **Requirement:** The 'Cutover authorization' view MUST show, before authorization, a precondition checklist with evidence and as-of: parallel run validated (cycles and variance within tolerance); tie-outs passed and current (each tie-out class with its as-of; a tie-out older than the latest staged change is 'Not current' and blocks); rollback boundary declared; pending-items routing agreement recorded (pending ACH, outstanding checks, Unknown payments); sponsor program active or activating. Authorization is dual: sponsor CFO, finance and operations approvers, all distinct, each shown with status; the cutover is authorized only when all have approved. The confirmation (G-005) states that the legacy writer will be fenced at the allocated epoch, opening balances post exactly once, and the rollback boundary. After authorization the view MUST show fence status: 'Cutover authorized' (fence pending), 'Fence status unknown' with 'Check fence status' (query only) and the statement that Unbound does not post until the fence is confirmed (UNB-RCN-WF-020), or 'Live on Unbound' only once the fence is confirmed. 'Roll back' is available to the CFO only before the first live Unbound posting and within the rollback boundary; afterwards it is not offered and the view directs to compensation.
- **Traces to:** WFM-26-T08, WFM-26-T11, WFM-26-T09, WFM-26-T10, WFM-26-T04, WFM-26-T07, RR-REQ-X-061-r1, INV-07, GOV-AUTH-005, UNB-RCN-WF-020, UNB-UI-G-003, UNB-UI-G-005, UNB-UI-G-006
- **Actors:** Sponsor CFO, Finance approver, Operations approver
- **Forma components:** `record-header`, `readiness-checklist`, `freshness`, `status-lozenge`, `key-value-list`, `alert`, `dialog`, `textarea`, `operation-status`
- **States:** Parallel validated; Cutover authorized; Fence status unknown; Live on Unbound; Rolled back
- **Actions:** Authorize cutover (A4, dual approval; WFM-26-T08); Roll back before first live posting (A4; WFM-26-T11); Check fence status (A0; query by idempotency key, WFM-26-T10)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11, FORMA-GAP-14, FORMA-GAP-24
- **Acceptance criteria:**
  - **AC-1** Given a staging correction after the last tie-out; when the CFO opens cutover; then the tie-out shows 'Not current' and Authorize is unavailable with 'Tie-out must be re-run'.
  - **AC-2** Given the CFO and finance approved but operations has not; when the view renders; then status remains 'Parallel validated' with 'Waiting for operations approval'.
  - **AC-3** Given the fencing request returned no definite result; when the view renders; then it shows 'Fence status unknown' (unknown), 'Check fence status', and no 'Live on Unbound' label until the fence is confirmed.
  - **AC-4** Given a live Unbound posting has occurred; when the CFO looks for Roll back; then it is not offered; the view directs to compensating entries.

### UNB-UI-S-059 — Migration mapping review (screen)
- **Requirement:** The 'Mapping review' view MUST show the proposed mapping version as field, role, restriction and charity-identity mappings, each with legacy value, target value, record count and sample records; unmapped and conflicting items first. Checks shown with results: roles derived from signed agreements, restrictions mapped to structured fields (free text kept as non-binding text), charity identity canonical (EIN match), with counts that fail. A diff against the previous mapping version is shown. Approval by the sponsor operations lead, who MUST NOT be the mapping author; a failing check blocks approval.
- **Traces to:** WFM-26-T02, RR-REQ-X-061-r1, REQ-X-061, UNB-UI-G-005
- **Actors:** Sponsor operations lead
- **Forma components:** `data-grid`, `diff-viewer`, `readiness-checklist`, `status-lozenge`, `key-value-list`, `dialog`
- **States:** Inventoried; Mapped
- **Actions:** Approve migration mapping (A4; WFM-26-T02)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given 14 charities without a canonical identity match; when the lead opens the mapping; then the check shows 14 failures first and Approve is unavailable.
  - **AC-2** Given the lead authored the mapping; when they open it; then Approve is unavailable with the segregation-of-duties reason.

### UNB-UI-S-060 — Assisted change console (screen)
- **Requirement:** The 'Assisted change' view MUST let a support agent stage, never apply, a change for an authenticated caller. Steps: record caller authentication with the assurance level achieved against the level the change requires (insufficient assurance blocks the change), the channel and the staff identity; choose the change type and enter the new values; review a before/after diff; send the confirmation to a verified channel on file (the agent chooses among verified channels only, never a channel supplied on the call). The case then shows 'Awaiting your confirmation' with expiry. For a charity payment destination change, the agent records a staff-assisted change that creates a new destination version 'Change requested', which must be confirmed via the charity's verified channel and independently verified before use; the previous channel is notified. Not available: apply change, mark confirmed, or verify a destination.
- **Traces to:** WFM-25-T03, WFM-13-T02, UNB-RCN-ENT-016, ADM-017, REQ-X-072, RR-REQ-X-005-r1, UNB-RCN-WF-028, UNB-UI-G-005, UNB-UI-G-012
- **Actors:** Support agent, Sponsor operations (staff-assisted)
- **Forma components:** `wizard`, `form`, `select`, `text-field`, `diff-viewer`, `key-value-list`, `status-lozenge`, `readiness-checklist`, `dialog`, `validation-summary`
- **States:** In progress; Awaiting your confirmation; Draft; Change requested
- **Actions:** Prepare assisted change (A1; WFM-25-T03); Record staff-assisted destination change (A2; WFM-13-T02)
- **Responsive:** desktop-first
- **Capability gaps:** FORMA-GAP-11
- **Acceptance criteria:**
  - **AC-1** Given a caller authenticated at a lower assurance than the change requires; when the agent tries to stage it; then staging is refused with 'Caller assurance too low for this change'.
  - **AC-2** Given a staged change; when the agent looks for an apply action; then none exists; the case shows 'Awaiting your confirmation'.
  - **AC-3** Given a caller asks to send confirmation to a new email; when the agent opens the channel list; then only verified channels on file are offered.

<!-- END GENERATED: ui-records -->

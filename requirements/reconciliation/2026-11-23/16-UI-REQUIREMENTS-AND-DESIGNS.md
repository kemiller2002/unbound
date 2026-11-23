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
| UNB-UI-G-002 | global | Five-dimension grant progress | steps, status-lozenge, timeline, freshness |  | — | 2 |
| UNB-UI-G-003 | global | Freshness of evidence and projections | freshness, status-lozenge |  | — | 1 |
| UNB-UI-G-004 | global | Authoritative amounts versus informational values | metric-card, key-value-list, freshness |  | — | 1 |
| UNB-UI-G-005 | global | Consequential action confirmation | dialog, key-value-list, callout, button |  | FORMA-GAP-01 | 1 |
| UNB-UI-G-006 | global | Unknown command outcome handling | operation-status, status-lozenge, recovery-actions |  | — | 1 |
| UNB-UI-G-007 | global | Sponsor terminology and protected legal text | prose, text-roles |  | — | 1 |
| UNB-UI-G-008 | global | Accessibility conformance | skip-link, visually-hidden, validation-summary, landmark-region |  | — | 1 |
| UNB-UI-G-009 | global | Responsive classification | mobile-action-bar, bounded-overflow, data-grid, responsive-grid |  | — | 1 |
| UNB-UI-G-010 | global | Scope and tenant context | scope-trail, select, dialog |  | — | 1 |
| UNB-UI-G-011 | global | AI assistance placement and labelling | flyout, conversation, composer, evidence-relationship, callout |  | — | 1 |
| UNB-UI-G-012 | global | Validation and fault presentation | validation-summary, validation-message, fault-inline, fault-banner, fault-reference, form, form-row |  | FORMA-GAP-02 | 1 |
| UNB-UI-G-013 | global | Loading and empty states | skeleton, empty-state, spinner |  | — | 1 |
| UNB-UI-G-014 | global | Brand and legal-sponsor disclosure | site-footer, app-identity, navigation-shell | ef-print-footer | — | 1 |
| UNB-UI-G-015 | global | In-app notification behaviour | toast, feed-item, timeline |  | — | 1 |
| UNB-UI-G-016 | global | Human-readable record references | identifier | ef-print-header | — | 1 |
| UNB-UI-G-017 | global | Money and percentage display | measure, key-value-list |  | — | 1 |
| UNB-UI-G-018 | global | Dates and time zones | timeline, facts |  | — | 1 |
| UNB-UI-S-001 | screen | Giving home (donor, advisor, successor) | navigation-shell, scope-trail, metric-card, key-value-list, freshness, work-queue, feed-item, mobile-action-bar, status-lozenge, empty-state, site-footer, disc… |  | FORMA-GAP-01 | 2 |
| UNB-UI-S-002 | screen | Charity search and selection | search, collection-toolbar, active-filter-summary, data-grid, status-lozenge, freshness, pagination, empty-state, form |  | — | 2 |
| UNB-UI-S-003 | screen | Grant recommendation flow | wizard, steps, form, form-row, text-field, textarea, choice-group, checkbox, select, date-time-field, callout, validation-summary, validation-message, dialog,… |  | FORMA-GAP-01, FORMA-GAP-02 | 3 |
| UNB-UI-S-004 | screen | Grant detail | record-header, identifier, steps, status-lozenge, facts, key-value-list, timeline, freshness, operation-status, dialog, provenance-trail |  | — | 2 |
| UNB-UI-S-005 | screen | Add money (contribution intent) and contribution status | choice-group, form, form-row, text-field, numeric-stepper, identifier, callout, steps, status-lozenge, data-grid, key-value-list, timeline, dialog |  | FORMA-GAP-02 | 2 |
| UNB-UI-S-006 | screen | Participants, delegation and successors | data-grid, status-lozenge, dialog, form, select, allocation, hierarchy-tree, validation-summary |  | — | 1 |
| UNB-UI-S-007 | screen | Document center | data-grid, collection-toolbar, select, identifier, status-lozenge, empty-state, pagination |  | — | 1 |
| UNB-UI-S-008 | screen | Staff exception inbox | workspace-shell, work-queue, data-grid, collection-toolbar, active-filter-summary, status-lozenge, metric-card, empty-state, pagination |  | — | 1 |
| UNB-UI-S-009 | screen | Case workspace | record-header, facts, evidence-relationship, conflict-review, provenance-trail, obligation-panel, readiness-checklist, dialog, textarea, flyout, conversation,… |  | FORMA-GAP-01 | 2 |
| UNB-UI-S-010 | screen | Sponsor policy configuration and approval | form, form-row, rule-builder, diff-viewer, comparison-pairs, readiness-checklist, dialog, date-time-field, validation-summary, flyout |  | FORMA-GAP-02 | 1 |
| UNB-UI-S-011 | screen | Sponsor overview | dashboard-grid, metric-card, metric-value-change, data-grid, status-lozenge, freshness |  | FORMA-GAP-05 | 1 |
| UNB-UI-S-012 | screen | Charity view | data-grid, status-lozenge, identifier, key-value-list, form, callout, timeline |  | — | 2 |
| UNB-UI-S-013 | screen | Notification preferences | switch, form, form-row, callout |  | — | 1 |
| UNB-UI-D-001 | document | Contribution acknowledgment |  | ef-print-document, ef-print-header, ef-print-footer, ef-print-section, ef-print-table, ef-print-note, ef-print-page-num… | FOLIO-GAP-01, FOLIO-GAP-02 | 1 |
| UNB-UI-D-002 | document | Fund statement |  | ef-print-document, ef-print-title-page, ef-print-header, ef-print-footer, ef-print-page-number, ef-print-section, ef-pr… | FOLIO-GAP-03 | 1 |
| UNB-UI-D-003 | document | Grant confirmation and remittance advice |  | ef-print-document, ef-print-header, ef-print-footer, ef-print-section, ef-print-table, ef-print-note | FOLIO-GAP-01 | 1 |
| UNB-UI-D-004 | document | Annual DAF reporting package (Schedule D Part I support) |  | ef-print-document, ef-print-title-page, ef-print-section, ef-print-metric, ef-print-table, ef-print-integrity, ef-print… | — | 1 |
| UNB-UI-D-005 | document | Case decision record |  | ef-print-document, ef-print-header, ef-print-section, ef-print-table, ef-print-finding, ef-print-callout, ef-print-foot… | — | 1 |
| UNB-UI-D-006 | document | Audit evidence package |  | ef-print-document, ef-print-title-page, ef-print-toc, ef-print-section, ef-print-table, ef-print-integrity, ef-print-pa… | — | 1 |
| UNB-UI-W-001 | widget | Embedded grant status widget (partner host) | surface, status-lozenge, key-value-list, identifier |  | UNB-GAP-W-01 | 1 |
<!-- END GENERATED: ui-index -->

## 4. Capability gaps (Forma, Folio and Unbound-owned)

These are recorded here only; no Forma or Folio repository was changed. Each gap has a release-1 workaround that keeps designs buildable, and an acceptance criterion the owning component can adopt.

<!-- BEGIN GENERATED: ui-gaps -->
| Gap | Component | Owner | Priority (owner's catalog) | Need | Release-1 workaround | Acceptance |
|---|---|---|---|---|---|---|
| FORMA-GAP-01 | Button variants (primary, destructive) and async action button | Forma | P0 (catalog/planned.json) | Distinguish the one primary action per view and destructive actions (Revoke, Void) in application UI; show in-progress state on submit without double submission. | Use native button with text labels; confirm destructive actions via dialog (G-005); disable the button and show operation-status while pending. Acceptable for design; not for production visual hierarchy. | data-forma-variant=primary\|destructive styles exist in components.css outside the marketing scope; an aria-busy button state is styled and announced; axe passes. |
| FORMA-GAP-02 | Field invalid, disabled and read-only styling | Forma | P0 (catalog/planned.json) | Forms must show invalid fields and read-only policy values distinctly (G-012). | validation-message text plus aria-invalid; the visual distinction relies on adjacent message text only. | :disabled, :read-only, [aria-invalid=true] and :user-invalid styles exist in foundations.css with 3:1 non-text contrast; a browser test proves each. |
| FORMA-GAP-03 | Data table styling for plain table | Forma | P0 | Not required by release-1 designs: data-grid and dense-ledger cover all tables. | Use data-grid or dense-ledger. | n/a for release 1 |
| FORMA-GAP-04 | Bulk action bar | Forma | P1 | Not required by release 1: the exception inbox has no bulk actions by design (each decision is individual). One-click batch approval (RCD-005) is a single dialog listing items. | dialog + data-grid | n/a for release 1 |
| FORMA-GAP-05 | Visualization primitives and chart accessibility contract | Forma | P1/P2 | Trend views for sponsor overview and investment performance (post release 1). | Tables and metric-value-change only (S-011). | A chart primitive with a mandatory text-table equivalent and axe-tested contrast. |
| FOLIO-GAP-01 | Letter primitives (letterhead, address block, signature block) | Folio | — | Acknowledgments and remittances are letters. | Compose from ef-print-header and ef-print-section with semantic address and paragraph markup (proven in design prototype D-001). | Documented recipe or primitives with print-position rules for window envelopes (address block at a fixed position). |
| FOLIO-GAP-02 | Verification code and document identity block | Unbound (document registry) + Folio presentation | — | Each issued document shows its reference and a verification code or URL that resolves in the document registry (REQ-X-034). | Text verification code inside ef-print-footer; QR code generation is Unbound-owned (SVG). | Footer shows the reference and the verification URL; the registry resolves the hash. |
| FOLIO-GAP-03 | Deterministic server-side PDF rendering (P2) and batch generation | Unbound (render service), Folio (tier P2 contract) | planned (Folio §15 P2) | Monthly statements for every fund, re-renderable byte-for-byte or with a stable content hash. | Headless Chromium print-to-PDF from the Folio HTML; determinism measured on the page-content hash, because PDF metadata dates vary (see report 17). | Re-rendering the same snapshot yields an identical normalized content hash; throughput is measured. |
| UNB-GAP-W-01 | Widget host, frame protocol and scoped tokens | Unbound (security-owned; Limen effects) | — | Read-only embedded status (W-001). | Plain iframe with server-issued short-lived token; no postMessage commands in release 1. | CSP frame-ancestors restricted to registered partner origins; token TTL ≤ 10 minutes; no PII beyond the card. |
<!-- END GENERATED: ui-gaps -->

## 5. Fit evidence from the prototypes

FIT-EVIDENCE-PLACEHOLDER

## 6. Requirement records

<!-- BEGIN GENERATED: ui-records -->
### UNB-UI-G-001 — Status vocabulary and visual mapping (global)
- **Requirement:** Every domain state MUST render as a text label from the versioned status catalog, wrapped in a Forma status-lozenge whose data-state is derived by rule: ok = authoritative final success (Settled, Final, Approved, Accepted); attention = waiting on a named party or action; unknown = outcome or evidence not determinable (PaymentOutcomeUnknown, EvidenceStale, Disputed); blocked = held, blocked, rejected, failed or ineligible. No state may be conveyed by colour alone, and no record may display a final-success label until the authoritative component has recorded the final state.
- **Traces to:** UNB-UX-003, RR-UNB-UX-003-r1, REQ-X-011, RCF-PLT-027
- **Forma components:** `status-lozenge`, `operation-status`
- **Acceptance criteria:**
  - **AC-1** Given a payment attempt whose provider outcome is Unknown; when any screen renders it; then the label reads 'Payment outcome unknown' with data-state=unknown, and no screen shows 'Paid'.
  - **AC-2** Given a grant in state EligibilityHold; when the giving home renders it; then the label reads 'On hold: charity eligibility' with data-state=blocked and a text reason.

### UNB-UI-G-002 — Five-dimension grant progress (global)
- **Requirement:** Grant status MUST be shown as five ordered dimensions (Recommendation, Eligibility, Approval, Funding, Payment), each with its own label and as-of time, using Forma steps for the sequence and status-lozenge for each dimension. A single headline status MUST be derived deterministically: the first dimension that is not final, except that any Unknown or Blocked dimension becomes the headline.
- **Traces to:** REQ-X-011, RR-REQ-X-002-r1, RR-REQ-X-004-r1, UNB-RCN-FIN-008
- **Forma components:** `steps`, `status-lozenge`, `timeline`, `freshness`
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
- **Requirement:** Every action of authority class A2 or higher MUST present, before submission, a confirmation dialog that states what will change, whether it is reversible, who acts next and the expected time to the next state. The confirm control MUST name the action (for example 'Submit grant recommendation'), not 'OK'.
- **Traces to:** RR-REQ-X-051-r1, AIN-006, UNB-RCN-PLT-005
- **Forma components:** `dialog`, `key-value-list`, `callout`, `button`
- **Capability gaps:** FORMA-GAP-01
- **Acceptance criteria:**
  - **AC-1** Given a donor submits a $5,000 grant; when they press Submit; then a dialog lists amount, charity, fund, 'Your sponsor approves this grant; you can withdraw until approval', and the button reads 'Submit grant recommendation'.

### UNB-UI-G-006 — Unknown command outcome handling (global)
- **Requirement:** When a command returns no definite outcome (Limen OutcomeUnknown, timeout or 5xx after submission), the screen MUST show a Forma operation-status in the unknown state with a 'Check status' action that queries by idempotency key. It MUST NOT automatically retry and MUST NOT show success or failure.
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
- **Requirement:** Every authenticated screen MUST show its scope (sponsor, program and fund or case) in a Forma scope-trail. Users with access to more than one tenant MUST choose from a server-issued list, and changing scope MUST re-authorize and reload all data.
- **Traces to:** INT-001, RR-UNB-TEN-004-r1, RCF-PLT-044
- **Forma components:** `scope-trail`, `select`, `dialog`
- **Acceptance criteria:**
  - **AC-1** Given an advisor with clients at two sponsors; when they switch sponsor; then the page reloads under the new server-issued token, and no data from the previous sponsor remains in the DOM.

### UNB-UI-G-011 — AI assistance placement and labelling (global)
- **Requirement:** AI assistance MUST appear only on surfaces with a published assistance contract, opened from an 'Explain' or 'Help me' control into a Forma flyout containing conversation output. Every AI output MUST be labelled 'AI-generated, not a decision' with its sources, and MUST NOT contain controls that submit consequential actions.
- **Traces to:** RR-REQ-X-051-r1, RR-UNB-AI-STD-003-r1, UNB-RCN-PLT-004
- **Forma components:** `flyout`, `conversation`, `composer`, `evidence-relationship`, `callout`
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
- **Acceptance criteria:**
  - **AC-1** Given sponsor brand 'Harbor Giving' operated by legal sponsor 'Example Charitable Foundation, EIN 00-0000000'; when any donor page renders; then the footer shows the legal sponsor name, EIN and control statement.

### UNB-UI-G-015 — In-app notification behaviour (global)
- **Requirement:** Transient toasts MAY confirm reversible actions only. Consequential outcomes (approved, paid, held, returned) MUST appear in the record timeline and in the notification list (Forma feed-item), sourced from the notification event catalog (UNB-RCN-PLT-002).
- **Traces to:** UNB-RCN-PLT-002, AIN-023
- **Forma components:** `toast`, `feed-item`, `timeline`
- **Acceptance criteria:**
  - **AC-1** Given a grant moves to Paid; when the donor opens the app; then a feed-item and a timeline entry show it; no toast was used for it.

### UNB-UI-G-016 — Human-readable record references (global)
- **Requirement:** Every fund, grant, contribution, case and document MUST show its numeric reference using the Forma identifier component, with a copy control. The same reference MUST print on every document and remittance.
- **Traces to:** REQ-X-025, RCD-008
- **Forma components:** `identifier`
- **Folio elements:** `ef-print-header`
- **Acceptance criteria:**
  - **AC-1** Given grant 2026-004817; when the grant screen, remittance advice and statement render; then all three show 2026-004817.

### UNB-UI-G-017 — Money and percentage display (global)
- **Requirement:** Amounts MUST display with currency, two decimals and tabular numerals, never rounded below cents. Negative values MUST carry a text label as well as a sign. Percentage grants MUST show the percentage, its base, the fixing point and, once fixed, the fixed currency amount.
- **Traces to:** RR-UNB-GFUND-013-r1
- **Forma components:** `measure`, `key-value-list`
- **Acceptance criteria:**
  - **AC-1** Given a 10% grant of fund value fixed at approval; when the grant renders after approval; then it shows '10% of fund value at approval → $12,345.67 (fixed 24 Nov 2026)'.

### UNB-UI-G-018 — Dates and time zones (global)
- **Requirement:** Dates of legal significance (gift date, grant approval, payment date) MUST show as absolute dates in the sponsor's time zone, with UTC in details. Relative time ('2 hours ago') MAY be used only for activity feeds.
- **Traces to:** UNB-RCN-FIN-012, REQ-X-003
- **Forma components:** `timeline`, `facts`
- **Acceptance criteria:**
  - **AC-1** Given a gift completed at 2026-12-31T23:30 America/Chicago; when the contribution renders; then the gift date reads 31 Dec 2026, not 1 Jan 2027.

### UNB-UI-S-001 — Giving home (donor, advisor, successor) (screen)
- **Requirement:** The giving home MUST answer, on one phone screen: how much is available to grant, what is pending and why, and what the user should do next. Regions: scope-trail; fund summary (Available to grant, Market value with freshness, Pending); 'Needs your attention' items created by rules (for example, a held grant or a certification request); recent activity; primary actions 'Recommend a grant' and 'Add money'.
- **Traces to:** UNB-UX-001, UX-S02, RCD-026
- **Actors:** Donor, Advisor (scoped), Successor (after activation)
- **Forma components:** `navigation-shell`, `scope-trail`, `metric-card`, `key-value-list`, `freshness`, `work-queue`, `feed-item`, `mobile-action-bar`, `status-lozenge`, `empty-state`, `site-footer`, `disclosure`
- **States:** Loading; Empty (no contributions yet); Normal; Capacity not current; Fund suspended (death or hold)
- **Actions:** Recommend a grant (navigate); Add money (navigate); Open attention item
- **Responsive:** phone-complete: metric cards stack; actions in mobile-action-bar
- **Capability gaps:** FORMA-GAP-01
- **Acceptance criteria:**
  - **AC-1** Given a fund in Suspended state after a verified death; when a successor opens it before activation; then the page shows 'This fund is suspended pending succession' (blocked) and offers no grant action.
  - **AC-2** Given a donor with 2 rule-created attention items; when they open the giving home; then both items appear first, each with one named action.

### UNB-UI-S-002 — Charity search and selection (screen)
- **Requirement:** Charity search MUST implement UNB-RCN-PLT-014: a search field (name, alias, EIN); filters for state and cause; results as a list that recomposes on phones showing legal name, EIN, city/state and platform eligibility (status-lozenge with as-of date). Selecting a result binds the canonical identity. 'Can't find it?' MUST open an identity-resolution request form.
- **Traces to:** UNB-RCN-PLT-014, REQ-X-011
- **Actors:** Donor, Advisor
- **Forma components:** `search`, `collection-toolbar`, `active-filter-summary`, `data-grid`, `status-lozenge`, `freshness`, `pagination`, `empty-state`, `form`
- **States:** Idle; Searching; Results; No results; Search unavailable (fault-inline)
- **Actions:** Search; Filter; Select charity; Request a charity we don't list
- **Responsive:** phone-complete: data-grid recomposes into labelled records at 30rem
- **Acceptance criteria:**
  - **AC-1** Given two 'Hope Foundation' results; when the donor views results on a phone; then each row shows EIN and city/state so they are distinguishable.
  - **AC-2** Given a charity on hold; when it appears in results; then it shows 'On hold' with no reason text from another sponsor and cannot be selected.

### UNB-UI-S-003 — Grant recommendation flow (screen)
- **Requirement:** A four-step wizard: 1 Charity (S-002); 2 Amount (fixed amount, or percentage with base and fixing point; recurring schedule optional); 3 Purpose and acknowledgements (purpose text; recognition: name, anonymous or custom; the sponsor's required affirmations as individual checkboxes stating the benefit rules verbatim; the pledge question per sponsor policy); 4 Review and submit (G-005 dialog). It MUST validate against sponsor policy at each step, show 'Available to grant' throughout, and after submission show the five-dimension status (G-002).
- **Traces to:** UNB-GRANT-002, RR-UNB-GRANT-011-r1, UNB-RCN-GRT-009, UNB-RCN-GRT-010, UNB-RCN-GRT-004, RR-UNB-GFUND-013-r1, REQ-X-011
- **Actors:** Donor, Advisor (scoped)
- **Forma components:** `wizard`, `steps`, `form`, `form-row`, `text-field`, `textarea`, `choice-group`, `checkbox`, `select`, `date-time-field`, `callout`, `validation-summary`, `validation-message`, `dialog`, `key-value-list`, `operation-status`, `mobile-action-bar`
- **States:** Draft (saved locally via Limen IndexedDB pack); Validation errors; Submitting; Submitted; Outcome unknown; Rejected by policy (with reason)
- **Actions:** Next/Back; Save draft; Submit grant recommendation (A1 by donor; creates RecommendationSubmitted); Withdraw (until approval)
- **Responsive:** phone-complete
- **Capability gaps:** FORMA-GAP-01, FORMA-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given an amount above Available to grant; when the donor continues from step 2; then an inline error states the available amount and the step does not advance.
  - **AC-2** Given a sponsor whose pledge policy is Prohibited; when the donor reaches step 3; then a required question 'Does this grant fulfil a pledge?' appears; answering Yes blocks submission with an explanation.
  - **AC-3** Given submission times out; when the response is unknown; then G-006 applies and no duplicate recommendation is created.

### UNB-UI-S-004 — Grant detail (screen)
- **Requirement:** Grant detail MUST show: record-header with numeric reference and headline status; five-dimension progress; charity identity and eligibility snapshot (as-of date); amount (G-017); purpose and recognition; timeline of all events; documents (grant confirmation, remittance); actions allowed in the current state only (Withdraw before approval; Request cancellation after approval, which explains that payment may already be in progress).
- **Traces to:** REQ-X-011, RR-REQ-X-004-r1, TVR-GR-004
- **Actors:** Donor, Advisor, Sponsor staff (read)
- **Forma components:** `record-header`, `identifier`, `steps`, `status-lozenge`, `facts`, `key-value-list`, `timeline`, `freshness`, `operation-status`, `dialog`, `provenance-trail`
- **States:** Each grant state, including Payment outcome unknown, Returned, Voided check, Blocked (sanctions)
- **Actions:** Withdraw recommendation; Request cancellation; Download confirmation
- **Responsive:** phone-complete
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
- **Capability gaps:** FORMA-GAP-02
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
- **Acceptance criteria:**
  - **AC-1** Given successor allocations of 60% and 30%; when the donor saves; then validation blocks saving and states the 10% remainder.

### UNB-UI-S-007 — Document center (screen)
- **Requirement:** The document center MUST list issued documents (acknowledgments, statements, grant confirmations) with type, period, issue date, numeric reference and verification status. Each opens the accessible HTML rendition, with a PDF download, from the same snapshot. Corrected documents MUST show 'Superseded by' links. Filters: type and year.
- **Traces to:** REQ-X-035, RCD-009, UNB-RCN-PLT-010
- **Actors:** Donor, Advisor, Sponsor staff
- **Forma components:** `data-grid`, `collection-toolbar`, `select`, `identifier`, `status-lozenge`, `empty-state`, `pagination`
- **Actions:** Open (HTML); Download PDF; Verify document
- **Responsive:** phone-complete
- **Acceptance criteria:**
  - **AC-1** Given an acknowledgment corrected once; when the donor lists documents; then the original shows 'Superseded by <ref>' and both remain downloadable.

### UNB-UI-S-008 — Staff exception inbox (screen)
- **Requirement:** A single inbox of rule-created work items (RR-REQ-X-048-r1). Each row shows creating rule, subject, age against SLA, owner and next action, sorted by SLA risk. Filters: rule family, sponsor, owner. Routine work never appears. Items close automatically when their exit condition becomes true. A summary row shows straight-through rate and open items by age (UNB-RCN-PLT-006).
- **Traces to:** RR-REQ-X-048-r1, UNB-RCN-PLT-006, RCD-026
- **Actors:** Sponsor operations, Compliance, Finance
- **Forma components:** `workspace-shell`, `work-queue`, `data-grid`, `collection-toolbar`, `active-filter-summary`, `status-lozenge`, `metric-card`, `empty-state`, `pagination`
- **States:** Open; Assigned; Waiting on external party; Resolved (auto-closed); Breaching SLA
- **Actions:** Open case; Assign to me; Reassign (lead only)
- **Responsive:** phone-review-and-approve
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
- **Capability gaps:** FORMA-GAP-01
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
- **Capability gaps:** FORMA-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given an author; when they open their own draft in review; then Approve is unavailable with the reason shown.

### UNB-UI-S-011 — Sponsor overview (screen)
- **Requirement:** A role-composed overview of outcomes at risk, without a composite health score: holds by reason and age; straight-through rate by workflow; reconciliation status by account; evidence freshness; statement publication status. Release 1 uses tables and metric cards only (charts deferred, FORMA-GAP-05).
- **Traces to:** REQ-X-043, UX-S01, RCD-026, UNB-RCN-PLT-006
- **Actors:** Sponsor executive, Operations lead
- **Forma components:** `dashboard-grid`, `metric-card`, `metric-value-change`, `data-grid`, `status-lozenge`, `freshness`
- **Responsive:** phone-review-and-approve
- **Capability gaps:** FORMA-GAP-05
- **Acceptance criteria:**
  - **AC-1** Given reconciliation evidence for one account is 2 days stale; when the overview renders; then that account shows 'Not current' and no green total is shown.

### UNB-UI-S-012 — Charity view (screen)
- **Requirement:** Charities MUST see grants addressed to them (sponsor as legal donor; donor name only where the donor chose recognition), remittance details, payment status and their verified payment destinations. Destination changes MUST follow the DestinationEvidence process, without self-service activation.
- **Traces to:** REQ-X-058, UNB-RCN-GRT-011, UNB-RCN-GRT-017, RR-REQ-X-005-r1
- **Actors:** Charity finance user
- **Forma components:** `data-grid`, `status-lozenge`, `identifier`, `key-value-list`, `form`, `callout`, `timeline`
- **Responsive:** phone-complete
- **Acceptance criteria:**
  - **AC-1** Given an anonymous grant; when the charity views it; then the donor field reads 'Anonymous donor via <sponsor>'.
  - **AC-2** Given a destination change request; when submitted; then it shows 'Pending independent verification' and payments continue to the previous verified destination.

### UNB-UI-S-013 — Notification preferences (screen)
- **Requirement:** Users MUST be able to switch optional notification classes on and off per channel. Mandatory-legal and security classes are shown as always on, with the reason.
- **Traces to:** UNB-RCN-PLT-002, AIN-023
- **Actors:** Donor, Advisor
- **Forma components:** `switch`, `form`, `form-row`, `callout`
- **Responsive:** phone-complete
- **Acceptance criteria:**
  - **AC-1** Given the acknowledgment notice class; when the user views preferences; then its switch is shown on and disabled, with the text 'Required: tax acknowledgment'.

### UNB-UI-D-001 — Contribution acknowledgment (document)
- **Requirement:** The acknowledgment MUST contain: sponsor letterhead and legal name and EIN; donor name; contribution reference; date of gift; cash amount, or a description of noncash property with no value; the goods-or-services statement; the exclusive-legal-control sentence verbatim; and the document reference with verification code. It is produced as an accessible HTML rendition and a PDF from one snapshot (RCD-009).
- **Traces to:** RR-UNB-ACC-006-r1, UNB-RCN-FIN-001, RCD-019
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-footer`, `ef-print-section`, `ef-print-table`, `ef-print-note`, `ef-print-page-number`
- **Capability gaps:** FOLIO-GAP-01, FOLIO-GAP-02
- **Acceptance criteria:**
  - **AC-1** Given a securities contribution of 600 shares of XYZ; when the acknowledgment renders; then it describes '600 shares of XYZ Corp. common stock' and states no value.

### UNB-UI-D-002 — Fund statement (document)
- **Requirement:** The statement MUST contain: a period summary (opening value, contributions, grants paid, fees, investment change, closing value; available to grant at period end); contributions, grants and fees tables that repeat column headers across pages; pending grants; the fee basis; disclosures and the legal-sponsor statement; page x of y. Generated from the locked period snapshot.
- **Traces to:** REQ-X-032, RCD-009, RCD-018, RCD-020
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-header`, `ef-print-footer`, `ef-print-page-number`, `ef-print-section`, `ef-print-metric`, `ef-print-table`, `ef-print-keep`, `ef-print-note`, `ef-print-toc`
- **Capability gaps:** FOLIO-GAP-03
- **Acceptance criteria:**
  - **AC-1** Given a statement with 70 grant rows; when rendered to PDF; then the grants table spans pages with its header row repeated, and every page shows 'Page n of N'.

### UNB-UI-D-003 — Grant confirmation and remittance advice (document)
- **Requirement:** The donor confirmation and the charity remittance MUST state the sponsor as the legal donor, the grant reference, amount, payment method and date, the purpose, and recognition per the donor's choice. Remittance MUST contain no pledge reference (UNB-RCN-GRT-011). One remittance may list several grants paid together.
- **Traces to:** UNB-RCN-GRT-011, RCD-016
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-footer`, `ef-print-section`, `ef-print-table`, `ef-print-note`
- **Capability gaps:** FOLIO-GAP-01
- **Acceptance criteria:**
  - **AC-1** Given a grant whose donor recorded a pledge ID internally; when the remittance renders; then no pledge reference appears anywhere in the document or its metadata.

### UNB-UI-D-004 — Annual DAF reporting package (Schedule D Part I support) (document)
- **Requirement:** The package MUST contain, per legal sponsor and tax year: the number of DAFs, aggregate value at year end, aggregate contributions and aggregate grants, each with a reconciliation to the ledger, the classification register version and preparer and signer fields.
- **Traces to:** UNB-RCN-ENT-012, RR-REQ-X-060-r1, RCD-024
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-section`, `ef-print-metric`, `ef-print-table`, `ef-print-integrity`, `ef-print-note`
- **Acceptance criteria:**
  - **AC-1** Given a locked tax year; when the package renders twice; then both renditions carry identical figures and the same source snapshot hash.

### UNB-UI-D-005 — Case decision record (document)
- **Requirement:** The decision record MUST capture the case reference, rule that raised it, evidence list with sources and freshness, decision, rationale, decider and preparer identities, and timestamps, for audit evidence packages.
- **Traces to:** RR-AIN-003-r1, UNB-RCN-REG-003
- **Folio elements:** `ef-print-document`, `ef-print-header`, `ef-print-section`, `ef-print-table`, `ef-print-finding`, `ef-print-callout`, `ef-print-footer`
- **Acceptance criteria:**
  - **AC-1** Given a confirmed sanctions match; when the decision record renders; then it lists preparer and decider as different people and includes the OFAC report due date.

### UNB-UI-D-006 — Audit evidence package (document)
- **Requirement:** The audit evidence package MUST be a period-bounded export for an auditor: a control list, evidence items with hashes, exceptions and their resolution, and a table of contents.
- **Traces to:** SOC2-009, RCD-031
- **Folio elements:** `ef-print-document`, `ef-print-title-page`, `ef-print-toc`, `ef-print-section`, `ef-print-table`, `ef-print-integrity`, `ef-print-page-number`
- **Acceptance criteria:**
  - **AC-1** Given an evidence package for Q4; when rendered; then every evidence row shows its SHA-256 and the table of contents lists each control section.

### UNB-UI-W-001 — Embedded grant status widget (partner host) (widget)
- **Requirement:** Partner hosts MAY embed a read-only grant-status card in an iframe served from the sponsor's Unbound origin, authorized by a short-lived, sponsor-scoped token issued server-to-server. It shows headline status, amount and charity, and offers no consequential actions in release 1 (DAF checkout deferred, CAP-03).
- **Traces to:** INT-015, REQ-X-063, RCD-011, RCD-013
- **Forma components:** `surface`, `status-lozenge`, `key-value-list`, `identifier`
- **Capability gaps:** UNB-GAP-W-01
- **Acceptance criteria:**
  - **AC-1** Given an expired token; when the widget loads; then it shows 'Sign in again on your provider's site' and no grant data.

<!-- END GENERATED: ui-records -->

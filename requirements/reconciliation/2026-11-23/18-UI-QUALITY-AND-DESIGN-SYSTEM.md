# 18 — UI quality, design system and experience refinement

Date: 2026-11-23 | Owner addendum: *"Transform the current functional but visually underdeveloped interfaces into polished, professional, accessible financial-services experiences ... Continue until all discovered UI requirements have been evaluated and the major design deficiencies have concrete, testable refinements."*

This report covers the UI quality addendum. It preserves the business functionality and state coverage of reports 16 and 17. It changes how those states look, how they are prioritised and how they are verified. Nothing here is production UI: the v2 pages are static design prototypes built to be measured.

## 0. Deliverables

| Required output | Where |
|---|---|
| UI design quality audit | §2 and [`ui-quality/V1-DESIGN-CRITIQUE.md`](ui-quality/V1-DESIGN-CRITIQUE.md); defects in [`data/ui-defects.csv`](data/ui-defects.csv) (63, with resolution) |
| Screen inventory and state matrix | §6; [`data/ui-state-matrix.csv`](data/ui-state-matrix.csv), [`data/ui-scenario-coverage.csv`](data/ui-scenario-coverage.csv) |
| Forma component gap analysis | §7; [`data/ui-component-gaps.csv`](data/ui-component-gaps.csv) (FORMA-GAP-18..26 new) |
| Design token requirements | §4.1; `UNB-UI-DS-001..018` in [`data/ui-requirements.jsonl`](data/ui-requirements.jsonl) |
| Navigation and information architecture requirements | §4.2; `UNB-UI-N-001..006` |
| Accessibility acceptance criteria | §4.5; `UNB-UI-A-001..005` |
| Responsive layout requirements | §4.4; `UNB-UI-R-001..003` |
| Financial presentation requirements | §4.3; `UNB-UI-F-001..007` |
| Visual regression specifications | §8; `UNB-UI-V-001..002`, scenarios `DKS-UI-001..020` in [`data/scenarios.csv`](data/scenarios.csv) |
| UI refinement traceability records | §9; [`data/ui-refinement-trace.csv`](data/ui-refinement-trace.csv) (`UIR-001..`) |
| Prioritised design improvement roadmap | §10 |

Prototypes: [`requirements/ui/designs/v2/`](../../ui/designs/v2/) (one HTML page per state, `index.html` lists them). Reference images are in `v2/rendered/<screen>/<state>-390.png` and `-1280.png`. Measurements are in `v2/verification.json`. To rebuild: `node tools/build.mjs`, then `node tools/verify.mjs`, from `requirements/ui/designs/v2`.

## 1. Method

1. **Verify the material first.** Read Forma at the inspected commit (tokens, components, icon registry, themes), the Echelon design-system repository (theme 1.1.0, brand audit), and the existing UI requirements. Nothing below assumes a token or component that was not found in source (§3).
2. **Audit v1 with measurements and a critique.**
   - An independent audit pass measured all 14 v1 pages at 320, 768, 1280, 1280 at 200% and 1280 at 400%.
   - Measurements: axe, money and word wrapping, overflow, keyboard walk with focus visibility and obscuring, heading and landmark semantics, target size, and WCAG 1.4.12 text spacing.
   - The same pass then reviewed every state's screenshot as a designer would (§2).
3. **Challenge, then specify.** Each defect asks whether the information is needed, whether it is correctly prioritised, whether the action belongs there, and whether the flow can be shorter. The answers became requirements (§4) and revisions to existing rules (§5).
4. **Redesign and iterate** with the loop build → verify (5 viewports) → look → fix, until every state passed and the critique items were addressed (§6).
   - Giving home was redesigned first, in 12 states.
   - S-003, S-004, S-008, S-011, S-012 and W-001 followed on the same system.
5. **Trace** every refinement to the defects it fixes, the original inbound requirements it refines, and the scenarios that test it (§9).

Limits:
- All checks are automated or design reviews by agents of the same model family. Manual screen-reader testing with real assistive technology and usability testing with donors have not been done. They are required (`DKS-UI-018`, `INT-020`) and listed in §10.
- Forced-colours behaviour is specified, but it is not yet emulated by the harness.

## 2. Audit of v1: what was wrong

v1 was semantically careful: 0 axe violations on all 14 pages at all 5 viewports, visible focus everywhere and no page-level horizontal scroll. Its problems were **priority, density and polish**, which an automated gate does not see.

| Severity | Count | Resolved in v2 or prevented by the new gate | Open (screens not yet redesigned) |
|---|---|---|---|
| Critical | 4 | 4 | 0 |
| Major | 35 | 27 | 8 |
| Minor | 24 | 16 | 8 |

The four critical defects:
- **UXD-001 — money broke mid-value.** "$100,000.|00" and "$20,000.0|0" broke inside three equal cards at 768 and 1280 px.
- **UXD-002 — current page number was nearly invisible.** It measured 1.63:1, but axe filed it as "incomplete", so the G-008 gate passed it.
- **UXD-003 — sticky phone bar hid focused controls.** The 138 px bar covered them at 320 px (WCAG 2.4.11).
- **UXD-004 — grant detail named the wrong grant.** Its scope trail showed a different grant from the record on screen.

The pattern behind the majors:
- **Everything at the same weight.** Three equal metric cards made the informational market value as large as the authoritative Available to grant. No button looked primary, and Reject looked like Continue.
- **Status as developer output.** 11 px uppercase Courier lozenges made the most important fact on a record the smallest and most technical text.
- **Repetition.** S-004 showed the same events twice, as a five-row tracker and as a timeline. S-003 confirmed the same data on a review step and again in a modal. S-002 had three controls that cleared one query.
- **Marketing frame for an application.** v1 used Forma's `.forma-site` marketing shell, so headings used marketing display sizes with a 14ch measure ("Normal: two / attention items first"). There was no account menu, no fund switcher and no indication of who was signed in.
- **System vocabulary.** "Created by sponsor rules", "Ledger confirmed", `F-2026-000418`, `IDK-…` and "assistance contract" leaked into the UI.
- **Order.** At 320 px the balance started about 981 px down, below a 710 px attention queue. S-001 is supposed to answer its three questions on one phone screen.

v1's full list, with screens, evidence and the expected behaviour: [`data/ui-defects.csv`](data/ui-defects.csv). Per-screen critique (keep, remove, merge, reprioritise): [`ui-quality/V1-DESIGN-CRITIQUE.md`](ui-quality/V1-DESIGN-CRITIQUE.md).

## 3. Verified design-system baseline

| Item | Verified fact | Source |
|---|---|---|
| Forma version used | 1.0.2 build (vendored `all.css`) and source at `d7c499e` | `requirements/ui/designs/vendor/forma-1.0.2/` |
| Spacing | 8 steps: 0.25, 0.5, 0.75, 1, 1.25, 1.5, 2, 3 rem (`--forma-space-1..8`) | `all.css` |
| Type | Sizes 2xs–3xl (0.6875–2.5 rem) plus fluid display (to 7.25 rem). The h1 measure is 14ch and the h2 measure 18ch. There is **no application type scale** and **no density token**. | `all.css` |
| Radius and targets | 0 / 4 / 8 px / pill; `--forma-primitive-size-target-min` 2.75 rem | `all.css` |
| Breakpoints | None as tokens; components hard-code 48rem | `all.css` |
| Themes | 19 (`light`, `dark`, `high-contrast`, …); brands compiled to `[data-forma-brand]` | `all.css`, `tools/BrandCompiler` |
| Primary button | `data-forma-variant="primary"` is styled **only inside `.forma-site`** (marketing) | `all.css` (finding for forma#186) |
| Application shell | `workspace-shell` exists (four regions, no compact navigation); `.forma-app-shell` is planned | catalog |
| Icons | 80 icons, 24 px grid, 1.8 stroke; none for gift, bank or charity | `icons/registry.json` |
| Charcoal and Bronze | Forma 1.0 **removed** the Echelon primitives (brand neutrality, BNR-001). The approved values now live in `kemiller2002/echelon-design-system` theme 1.1.0. | Forma CHANGELOG; `theme/echelon.brand.json` |
| Echelon values | charcoal `#202421`, carbon `#171a18`, parchment `#f2efe7`, oxide bronze `#905831` (dark `#a87e5e`), verdigris `#47756b`; Newsreader / Manrope / IBM Plex Mono (OFL) | `brand/AUDIT.md` (verified against Forma 0.14 sources) |
| Unverified | The README "candidate palette" (`#20252A`, `#B7844B`, …): *"Do not publish candidate values as final brand tokens."* | echelon-design-system README |
| Echelon app layer | `echelon-app.css` (app bar, nav tabs, panels, chips) is scoped to `[data-forma-brand="echelon"]` | `dist/echelon-app.css` |

Consequences:
- **Default brand (RCD-050).** Unbound's default theme is a separate Forma brand manifest, `unbound-default`. It copies the Echelon colours, fonts and radius and changes only identity and terms. It was compiled with Forma's own BrandCompiler; the same build reproduces `example-harbor.css` byte for byte, which confirms the toolchain.
- **Application layer (RCD-051).** The Echelon app layer cannot serve a white-label product, because it does not apply under a sponsor brand. Unbound therefore keeps a **brand-agnostic** interim application layer, [`v2/unbound-app.css`](../../ui/designs/v2/unbound-app.css). It only reads `--forma-*` tokens, never defines them, has no brand selectors, and names the Forma gap for every block. The long-term owner is Forma.

## 4. Requirements added (41)

All are `status: proposed` with Given/When/Then acceptance criteria. Each traces to inbound requirements such as `INT-009`, `INT-010`, `INT-011`, `INT-013`, `INT-019`, `UNB-UX-001..007`, `UX-S02`, `UXA-1..3`, `UI-T20/T21` and `ADV-*`.

<!-- BEGIN GENERATED: uiq-requirements -->
| ID | Kind | Title | Traces (inbound and governing) | Gaps | ACs |
|---|---|---|---|---|---|
| UNB-UI-DS-001 | design-system | Token source and brand layering | INT-019, UNB-EXP-002, UNB-UI-G-014, RCD-050 | FORMA-GAP-20 | 3 |
| UNB-UI-DS-002 | design-system | Application type scale | INT-019, UNB-UX-007, UX-S02 | FORMA-GAP-18 | 2 |
| UNB-UI-DS-003 | design-system | Spacing and density | INT-009, INT-019 | FORMA-GAP-18 | 2 |
| UNB-UI-DS-004 | design-system | Colour roles and semantic status | INT-019, ADV-928, ADV-692 | FORMA-GAP-20 | 2 |
| UNB-UI-DS-005 | design-system | Interaction states | UNB-UX-002, UNB-UI-G-008 | FORMA-GAP-01, FORMA-GAP-11 | 2 |
| UNB-UI-DS-006 | design-system | Buttons and action emphasis | INT-013, UNB-UI-G-005 | FORMA-GAP-01 | 2 |
| UNB-UI-DS-007 | design-system | Panels and grouping | INT-019, UX-S02 | FORMA-GAP-20 | 1 |
| UNB-UI-DS-008 | design-system | Status chip | UNB-UX-003, UNB-UI-G-001, UNB-UX-007 | FORMA-GAP-21 | 2 |
| UNB-UI-DS-009 | design-system | Task list (things that need the user) | UNB-UI-S-001, UX-S02, ADV-925 | FORMA-GAP-22 | 1 |
| UNB-UI-DS-010 | design-system | Notices for restrictions, staleness and outages | INT-012, UNB-UX-004, ADV-921, ADV-946 | — | 2 |
| UNB-UI-DS-011 | design-system | Iconography | INT-019, ADV-960 | FORMA-GAP-23 | 1 |
| UNB-UI-DS-012 | design-system | Sponsor white-labelling | UNB-UI-G-014, UNB-UI-G-007, INT-019, ADV-716 | FORMA-GAP-20 | 2 |
| UNB-UI-DS-013 | design-system | Empty and onboarding states | UNB-UI-G-013, UX-S09, ADV-946 | — | 1 |
| UNB-UI-DS-014 | design-system | Skeleton loading and layout stability | INT-011, UNB-UI-G-013, UI-T20 | — | 2 |
| UNB-UI-DS-015 | design-system | Dialogs | INT-013, UNB-UI-G-005, ADV-906, ADV-944 | FORMA-GAP-14 | 1 |
| UNB-UI-DS-016 | design-system | Notifications and task resolution | INT-017, UNB-UI-G-015, ADV-922, ADV-923, ADV-924, ADV-948 | — | 1 |
| UNB-UI-DS-017 | design-system | Forms and validation | UNB-UX-002, UNB-UI-G-012, UXA-2, TVR-UX-003 | FORMA-GAP-02 | 1 |
| UNB-UI-DS-018 | design-system | Charts and reporting | INT-010, ADV-692, ADV-693, ADV-721 | FORMA-GAP-05 | 1 |
| UNB-UI-N-001 | navigation | Application frame | INT-001, UNB-UI-G-014, UX-S02 | FORMA-GAP-06 | 1 |
| UNB-UI-N-002 | navigation | Primary destinations by audience | UNB-UX-001, UNB-UI-G-007 | — | 1 |
| UNB-UI-N-003 | navigation | Responsive navigation pattern | INT-009, UNB-UX-001, ADV-930 | FORMA-GAP-06 | 2 |
| UNB-UI-N-004 | navigation | Scope and fund context | INT-001, UNB-UI-G-010, UNB-UX-007 | — | 2 |
| UNB-UI-N-005 | navigation | Page header | UX-S02, INT-002 | — | 1 |
| UNB-UI-N-006 | navigation | Information architecture of the donor home | UX-S02, UNB-UI-S-001, UNB-UI-G-004 | — | 2 |
| UNB-UI-F-001 | financial-presentation | Money never wraps | UNB-UI-G-017, UNB-UX-005 | FORMA-GAP-08 | 1 |
| UNB-UI-F-002 | financial-presentation | Headline figure | UNB-UI-G-004, UX-S02 | FORMA-GAP-08 | 1 |
| UNB-UI-F-003 | financial-presentation | Signs and negative values | UNB-UI-G-017 | FORMA-GAP-08 | 1 |
| UNB-UI-F-004 | financial-presentation | Tables of money | UNB-UI-G-017, UNB-UI-G-018, INT-009 | FORMA-GAP-03 | 2 |
| UNB-UI-F-005 | financial-presentation | Dates and times | UNB-UI-G-018, RCD-052 | — | 1 |
| UNB-UI-F-006 | financial-presentation | Freshness wording | UNB-UI-G-003, INT-012, ADV-918, ADV-947 | — | 1 |
| UNB-UI-F-007 | financial-presentation | Fund composition | UNB-UI-G-004, INT-010 | FORMA-GAP-12 | 1 |
| UNB-UI-R-001 | responsive | Breakpoints and viewports | INT-009, UNB-UX-005, UNB-UI-G-009 | FORMA-GAP-19 | 1 |
| UNB-UI-R-002 | responsive | Container-driven components | INT-009 | FORMA-GAP-15 | 1 |
| UNB-UI-R-003 | responsive | Source order equals priority | INT-009, UNB-UX-002 | — | 1 |
| UNB-UI-A-001 | accessibility | WCAG 2.2 AA automated gate | UNB-UX-005, UNB-UX-006, UNB-UI-G-008 | — | 1 |
| UNB-UI-A-002 | accessibility | Keyboard and focus | UNB-UX-002, UNB-UX-005, ADV-907, ADV-944 | — | 1 |
| UNB-UI-A-003 | accessibility | Screen-reader semantics | UNB-UX-002, UNB-UX-006, UXA-1, UI-T21 | — | 1 |
| UNB-UI-A-004 | accessibility | Targets, reflow, spacing and motion | UNB-UX-005, UI-T20, ADV-930, ADV-961 | — | 1 |
| UNB-UI-A-005 | accessibility | Accessible authentication and redundant entry | UXA-2, UXA-3 | — | 1 |
| UNB-UI-V-001 | visual-regression | Reference images and comparison | UNB-UX-005, INT-020, ADV-700, ADV-701 | — | 1 |
| UNB-UI-V-002 | visual-regression | Scenario coverage matrix | UNB-UX-001, UNB-UX-003, INT-015 | — | 1 |
<!-- END GENERATED: uiq-requirements -->

### 4.1 Design tokens and components: what the numbers are

- **Colour roles:**
  - canvas = `surface-primary`;
  - raised panel = canvas mixed 35% with white;
  - sunken = canvas mixed with `surface-secondary`;
  - chrome = `surface-inverse` with `text-inverse` and a 3 px accent rule;
  - accent only for the single primary action, the current navigation item and the "available" allocation segment;
  - four status roles, always with a glyph and words.
- **Contrast rule carried from the Echelon audit:** no `text-secondary` on `surface-secondary` or sunken surfaces.
- **Application type scale:**
  - page title `clamp(1.5rem, 1.3rem + 0.8vw, 1.875rem)` at weight 650;
  - section title 1.125 rem;
  - body 1 rem / 1.5;
  - small 0.875 rem;
  - micro 0.8125 rem (the minimum).
  - The display face is used for the one headline money figure only.
- **Spacing:** Forma's 8 steps. Panels are 20 px apart and pages 32–48 px between regions. Comfortable density for donors (rows ≥ 44 px) and compact density for staff (rows ≥ 36 px, targets ≥ 24 px).
- **Interaction states:** rest, hover, focus-visible (3 px ring, 2 px offset, 2 px gap halo), active, disabled-with-reason, busy and error.
- **Status chip:** sentence case, registry glyph, words, 1 px border and 13 px semibold body face. It replaces the uppercase monospace lozenge on donor, advisor and charity surfaces.

### 4.2 Navigation and IA

- **One application frame:** skip link; chrome app bar (sponsor mark and name, product context, Help, Notifications with count, account menu showing who is signed in and their role); primary navigation; main; legal footer.
- **Primary destinations:**
  - Donor: Home, Grants, Contributions, Documents, People.
  - Staff: Work queue, Cases, Reconciliation, Funds, Policy.
  - Charity: Grants received, Organization.
  - Destinations are places, never actions. "Add money" is a page action.
- **Navigation by width:** side navigation from 64 rem; below that, a bottom tab bar with four destinations and visible labels, plus a Menu button. Scroll padding keeps focus clear of the tab bar. Page actions sit under the title on phones; there is no sticky action bar.
- **Fund context:** the fund is the page title and also the switcher. Internal references leave donor summaries. Staff keep the full scope trail.
- **Donor home order:** what can I give → what needs me → what is in flight → what happened.

### 4.3 Financial presentation

- **Money never wraps.** It uses tabular figures, a true minus with a text label, and amounts right-aligned in tables.
- **One headline figure**, with cents at 62% size. It is still one text token.
- **Fund composition:** a proportion bar with patterns, plus a legend list that holds the amounts (the bar is `aria-hidden`).
- **Freshness** is stated once, in words.
- **Dates** use the sponsor-locale medium format ("Nov 30, 2026", RCD-052), always inside `<time>`.

### 4.4 Responsive

- **Ranges:** phone below 48 rem, tablet 48–64 rem, desktop from 64 rem.
- **Mandatory verification viewports:** 320, 390, 640×450 (200% zoom), 768 and 1280, plus 360 and 600 px widget frames.
- Tables and legends respond to their **container**, not the viewport.
- **DOM order equals priority order**; grid areas place regions without reordering focus.

### 4.5 Accessibility acceptance

WCAG 2.2 AA is the baseline. Automated gate per state and viewport:
- zero axe violations, including "incomplete" contrast below its required ratio;
- a keyboard walk where every stop is visible and not covered at its centre;
- one h1, no skipped heading levels and unique navigation labels;
- targets at least 24 px, and 44 px for primary controls on phones;
- 320 px reflow without horizontal scroll;
- 1.4.12 text spacing without clipping;
- no console errors.

Manual review with NVDA, VoiceOver and TalkBack is required before release. Passkey-capable step-up authentication (3.3.8) and no redundant entry (3.3.7) are also required.

## 5. Revisions to existing UI rules

These records were `proposed`, so they are revised in place. The replaced text is kept in each record's `revisions` field.

| Record | Change | Why |
|---|---|---|
| `UNB-UI-G-001` | Status renders as the sentence-case chip (`UNB-UI-DS-008`); `status-lozenge` stays allowed in staff dense ledgers | UXD-012 |
| `UNB-UI-G-010` | Donor, advisor and charity screens identify scope by brand plus fund title and switcher; staff keep the scope-trail | UXD-016, UXD-017 |
| `UNB-UI-G-016` | References shown on detail views and documents, not in donor summary lists | UXD-014, UNB-UX-007 |
| `UNB-UI-S-001` | Attention items come directly after the balance; at 390×844 the figure and the attention heading are both in the first viewport; regions restated | UXD-008 (measured), UXD-009 |
| `UNB-UI-S-003` | One page with progressive disclosure, then one review step (was a four-step wizard plus a dialog); the review step meets G-005; a milestone track after submission | UXD-023, UXD-024, UXD-043 |
| `UNB-UI-G-005` | Confirmation means the review step for flows and a dialog for record actions (withdraw, cancel) | UXD-024 |
| `UNB-UI-G-002` | Five dimensions appear in staff views; donor, advisor and charity views show a four-milestone track with dates derived from them | UXD-018, UXD-019 |
| `UNB-UI-G-018` | UTC appears in documents and staff details only | UXD-014 |
| `UNB-UI-G-006` | Donor wording is "We can't confirm your recommendation was received"; the idempotency key is never shown to donors | UXD-014 |
| `UNB-UI-S-004` | Progress per G-002; a returned payment offers wait or redirect, each with its consequence and the default date | failed-payment scenario |
| `UNB-UI-S-011` | One accessible trend chart is allowed under DS-018 (it previously said "charts deferred"); no reconciled total while any account is stale | owner addendum: charts and reporting |
| `UNB-UI-G-017` | Chart value labels may be abbreviated when exact amounts are in the data table | S-011 chart |
| `UNB-UI-W-001` | Brand and legal sentence inside the frame; readable at 320–600 px frames; height reported to the registered host origin only; expiry clears data | UXD-061 |
| `UNB-UI-S-008` | Sponsor filter only for multi-sponsor operators; a proposer sees "Waiting for approval" and no approve action | v2 work queue |

## 6. v2 designs: screen inventory and state matrix

<!-- BEGIN GENERATED: uiq-coverage -->
| Required scenario | v2 states (passing all checks) | Count |
|---|---|---|
| normal | S-001:normal, S-001:white-label, S-003:choose-charity, S-003:details, S-004:in-review, S-008:queue, S-011:normal, S-012:grant-detail, S-012:grants-received, W-001:host-page, W-001:widget | 11 |
| loading | S-001:loading, S-008:loading | 2 |
| no-contributions | S-001:new-fund | 1 |
| no-grants | S-001:new-fund, S-001:no-grants | 2 |
| pending-grants | S-001:normal, S-003:submitted, S-004:in-review | 3 |
| attention-required | S-001:failed-payment, S-001:normal, S-004:on-hold, S-004:payment-returned, S-008:queue, S-008:reconciliation-break, S-012:bank-reverify, S-012:grants-received | 8 |
| stale-balances | S-001:stale-balance, S-011:integration-unavailable, S-011:normal | 3 |
| integration-unavailable | S-001:integration-unavailable, S-003:outcome-unknown, S-011:integration-unavailable | 3 |
| restricted-fund | S-001:restricted, S-003:restricted, S-012:bank-reverify | 3 |
| suspended-fund | S-001:succession, S-001:suspended | 2 |
| succession-pending | S-001:succession | 1 |
| read-only | S-001:read-only, S-004:advisor-view, S-008:proposed-by-me | 3 |
| failed-payment | S-001:failed-payment, S-004:payment-returned, S-012:bank-reverify | 3 |
| reconciliation-exception | S-008:reconciliation-break | 1 |
| mobile | S-001:failed-payment, S-001:integration-unavailable, S-001:loading, S-001:new-fund, S-001:no-grants, S-001:normal, S-001:read-only, S-001:restricted, S-001:stale-balance, S-001:succession, S-001:suspended, S-001:white-l… | 42 |
| embedded-widget | W-001:expired, W-001:host-page, W-001:widget-harbor, W-001:widget | 4 |
| white-label | S-001:white-label, S-003:white-label, W-001:host-page, W-001:widget-harbor | 4 |
<!-- END GENERATED: uiq-coverage -->

<!-- BEGIN GENERATED: uiq-states -->
| Screen | State | Title | Persona | Scenarios | Verified |
|---|---|---|---|---|---|
| S-001 | failed-payment | Failed payment: grant payment returned by charity's bank | Donor | attention-required, failed-payment, mobile | pass |
| S-001 | integration-unavailable | Investment pricing unavailable (integration down) | Donor | integration-unavailable, mobile | pass |
| S-001 | loading | Loading: skeleton matches the final layout | Donor | loading, mobile | pass |
| S-001 | new-fund | New fund: no contributions yet (onboarding) | Donor, fund opened today | mobile, no-contributions, no-grants | pass |
| S-001 | no-grants | Money available, no grants yet | Donor, first contribution settled | mobile, no-grants | pass |
| S-001 | normal | Normal: pending grants and two attention items | Donor (primary advisor) | attention-required, mobile, normal, pending-grants | pass |
| S-001 | read-only | Read-only: financial advisor with view access | Financial advisor (view only) | mobile, read-only | pass |
| S-001 | restricted | Restricted: grant recommendations paused by sponsor review | Donor | mobile, restricted-fund | pass |
| S-001 | stale-balance | Available to grant cannot be confirmed (stale) | Donor | mobile, stale-balances | pass |
| S-001 | succession | Succession pending: successor's first view | Successor advisor before activation | mobile, succession-pending, suspended-fund | pass |
| S-001 | suspended | Fund suspended by sponsor | Donor | mobile, suspended-fund | pass |
| S-001 | white-label | White-labelled sponsor (Harbor Giving brand manifest) | Donor (primary advisor) | mobile, normal, white-label | pass |
| S-003 | choose-charity | Choose a charity: search results with eligibility in plain words | Donor (primary advisor) | mobile, normal | pass |
| S-003 | details | Amount and details: one column, progressive disclosure | Donor (primary advisor) | mobile, normal | pass |
| S-003 | more-options | More options open: percentage of fund value and repeat (phone reference) | Donor on a phone | mobile | pass |
| S-003 | outcome-unknown | Outcome unknown after submit timeout (G-006) | Donor (primary advisor) | integration-unavailable, mobile | pass |
| S-003 | restricted | Restricted: recommending paused, draft still possible | Donor (primary advisor) | mobile, restricted-fund | pass |
| S-003 | review | Review and submit: the single G-005 confirmation | Donor (primary advisor) | mobile | pass |
| S-003 | submitted | Submitted: what happens next | Donor (primary advisor) | mobile, pending-grants | pass |
| S-003 | validation-errors | Validation: amount above Available to grant, missing purpose, charity on hold | Donor (primary advisor) | mobile | pass |
| S-003 | white-label | Amount and details under the Harbor Giving brand | Donor (primary advisor) | mobile, white-label | pass |
| S-004 | advisor-view | Read-only: financial advisor with view access | Financial advisor (view only) | mobile, read-only | pass |
| S-004 | approved | Approved, payment scheduled; withdraw unavailable with reason | Donor (primary advisor) | mobile | pass |
| S-004 | in-review | In review: progress with expected dates, withdraw available | Donor (primary advisor) | mobile, normal, pending-grants | pass |
| S-004 | on-hold | On hold: charity eligibility | Donor (primary advisor) | attention-required, mobile | pass |
| S-004 | paid | Paid: proof of payment and charity acknowledgment (percentage grant) | Donor (primary advisor) | mobile | pass |
| S-004 | payment-returned | Failed payment: returned by the charity's bank, wait or redirect | Donor (primary advisor) | attention-required, failed-payment, mobile | pass |
| S-004 | withdraw-confirm | Withdraw: consequential confirmation dialog | Donor (primary advisor) | mobile | pass |
| S-008 | empty | Empty queue: nothing needs you | Sponsor operations preparer | mobile | pass |
| S-008 | loading | Loading: skeleton matches the queue layout | Sponsor operations preparer | loading, mobile | pass |
| S-008 | proposed-by-me | Same break after proposing: preparer cannot approve own proposal | Sponsor operations preparer who proposed | mobile, read-only | pass |
| S-008 | queue | Exception-first queue sorted by SLA risk | Sponsor operations preparer | attention-required, mobile, normal | pass |
| S-008 | reconciliation-break | Reconciliation break selected: bank vs ledger, candidates and who can resolve | Sponsor operations preparer (owner of the item) | attention-required, mobile, reconciliation-exception | pass |
| S-011 | integration-unavailable | Bank feed unavailable: what is stale, what still works | Operations lead | integration-unavailable, mobile, stale-balances | pass |
| S-011 | normal | Overview with grants-paid chart, holds and reconciliation | Operations lead / sponsor executive | mobile, normal, stale-balances | pass |
| S-012 | bank-reverify | Bank details need re-verification after a returned payment | Charity finance user | attention-required, failed-payment, mobile, restricted-fund | pass |
| S-012 | grant-detail | One grant's detail with acknowledgment | Charity finance user | mobile, normal | pass |
| S-012 | grants-received | Grants received, one waiting for acknowledgment | Charity finance user | attention-required, mobile, normal | pass |
| W-001 | expired | Expired or invalid token: safe message, no grant data | Donor whose partner session expired | embedded-widget, mobile | pass |
| W-001 | host-page | Partner host page embedding the widget at 360 px and 600 px | Donor on the partner's client portal | embedded-widget, mobile, normal, white-label | pass |
| W-001 | widget-harbor | Widget under the Harbor Giving sponsor brand | Donor viewing a partner site (valid token) | embedded-widget, mobile, white-label | pass |
| W-001 | widget | Widget document: approved grant, read-only card | Donor viewing a partner site (valid token) | embedded-widget, mobile, normal | pass |
<!-- END GENERATED: uiq-states -->

### 6.1 Giving home, before and after

| v1 | v2 |
|---|---|
| Marketing frame with a 104 px title; no user or fund context | Application frame: brand chrome, signed-in user and role, notifications count, fund title that is also the switcher |
| Three equal cards; market value as large as Available to grant; "$100,000.\|00" wrapped | One headline figure (display face, never wraps). Committed, settling and total market value are ruled rows of a composition list with a patterned proportion bar. |
| "Created by sponsor rules" kicker; uppercase lozenges; "Ledger confirmed", "Calculated", "Current" | Plain sentences: "Confirmed by Example Community Foundation · Updated today at 4:58 PM CT"; sentence-case chips |
| Attention queue first, 710 px tall at 320; balance at about 981 px | Balance first, then attention. At 390×844 the figure is at 264 px and the attention heading at 711 px, above the 787 px tab bar (measured). |
| 138 px sticky action bar that hid focused controls, also stretched at the bottom of desktop | Actions in the page header (one primary); bottom tab bar on phones with scroll padding; no focus obscured at any viewport |
| Two empty-state boxes with "□" glyphs and a $0.00 card | One getting-started checklist (1 of 3 done) with one primary action |
| Restricted fund, read-only participant, stale Available to grant, integration down and failed payment: missing | Each designed as its own state with one notice: why, since when, what still works, what to do |

The S-003, S-004, S-008, S-011, S-012 and W-001 redesigns are described in §6.2.

### 6.2 Other redesigned screens

All states below pass the full gate at five viewports (42 of 42 pages). Each was also reviewed by eye at 390 and 1280 px.

**S-003 Grant recommendation (9 states).**
- **Layout:** no wizard. One page reveals the details once a charity is chosen. Percentage and repeat grants sit in a collapsed "More options" section.
- **Single confirmation:** one review page answers the four G-005 questions, and its button is "Submit grant recommendation".
- **Charity results:** eligibility is explained per result in plain words ("Not eligible", "On hold", each with why).
- **Validation:** the error summary links to each field and keeps entered values. A held charity offers "Choose another charity" or "keep as draft".
- **Unknown outcome:** "Check status", which never resubmits, plus how the outcome will be resolved.
- **Restricted fund:** "Save draft" becomes the primary action and submitting is explained as paused.
- **Removed:** the scope trail, internal request references, the second confirmation and the five-dimension grid.

**S-004 Grant detail (7 states).**
- **Order:** record header (charity, amount, status, reference once with copy), then a progress track with dates and the one available action, then history. Details and documents sit in a side column.
- **Withdraw:** a native modal dialog that names the action, offered only while the grant is still withdrawable.
- **After approval:** "Withdraw" is shown as unavailable with its reason, and "Request cancellation" explains that payment may already be under way.
- **Returned payment:** a choice between wait and redirect, each with its consequence and the default on Dec 16, 2026.
- **Advisor view:** no actions, and it says why.

**S-008 Work queue (5 states).**
- **Queue:** four summary figures, filter buttons with counts per type, and one sortable table with right-aligned amounts. On phones each row becomes a card and sorting moves to a select.
- **Selected item:** opens as its own page ("Item 2 of 9"). For the reconciliation break it reads in this order: what happened in plain words; bank vs ledger side by side with the difference; matching candidates ranked strong or weak with evidence; the resolutions with who may choose each; propose then approve by two named people; and what is blocked while the break is open (November close, statements for 412 funds, new grants to the affected charity).
- **Preparer's view:** after proposing, the preparer sees "Waiting for approval" and has no approve control.
- **Empty and loading:** "Nothing needs you" with the last-checked time, and a skeleton loading state.

**S-011 Sponsor overview (2 states).**
- Totals, an accessible 12-month grants-paid chart (SVG with a title, axis labels, value labels, a text summary and a data table), holds, reconciliation and freshness. There is no composite score.
- **Bank feed down:** says what is stale and what still works, and shows no reconciled total while an account is stale.

**S-012 Charity view (3 states).**
- A records list replaces the data grid, and the acknowledgment is the single to-do.
- An anonymous donor is shown with the sponsor's recognition wording.
- After a returned payment, a three-step bank re-verification explainer includes account-number re-entry.

**W-001 Embedded widget (4 states).**
- A read-only card with the sponsor brand bar, status chip, amount, milestones, source fund, last update and the legal-sponsor sentence inside the frame.
- Shown on a fictional partner host (a different brand) in 600 and 360 px slots. axe was run inside each iframe at five host widths, with no violations.
- On token expiry the card clears its data and shows a safe message. A Harbor-branded variant is included.

**Patterns proposed for the shared layer.** The redesign passes created screen-local components that should move into `unbound-app.css`, and later into Forma:
- copyable reference (`identifier` lacks copy: FORMA-GAP-07);
- compact metrics row;
- container-responsive toolbar and filters (FORMA-GAP-15);
- queue table with a phone card layout;
- bank-vs-ledger comparison with a difference row;
- decision options with consequence text and a status per option;
- propose and approve duties (two named people);
- milestone track with dates and a failed state (FORMA-GAP-09);
- event timeline;
- currency input with a prefix;
- unavailable action with reason (FORMA-GAP-11);
- choice cards whose hidden radio fills the card (2.5.8).

Roadmap wave 2 includes this consolidation.

## 7. Forma component gap analysis

Shared interaction patterns that should be Forma components (not one-off Unbound CSS):

| Gap | Pattern | Why the closest Forma component is not enough | Unbound interim |
|---|---|---|---|
| FORMA-GAP-06 (forma#189) | Application frame (app bar, account menu, side navigation, phone tab bar, legal footer slot) | `navigation-shell` hides destinations behind a disclosure on phones; `workspace-shell` has no compact navigation; `.forma-app-shell` is only planned | `.unb-appbar`, `.unb-sidenav`, `.unb-tabbar` |
| FORMA-GAP-01 (forma#186) | Primary and destructive button in applications | Primary is styled only inside `.forma-site`; there is no destructive variant | `.unb-app .forma-button[data-forma-variant=primary]` |
| FORMA-GAP-08 (forma#190) | Money value (nowrap, tabular, minus with label, headline variant) | `identifier` is for codes; nothing prevents wrapping | `.unb-money` |
| FORMA-GAP-12 (forma#193) | Read-only fund composition (proportion bar + legend) | `allocation` is an input control | `.unb-alloc` |
| **FORMA-GAP-18** | Application type scale and density switch | Heading sizes are marketing display sizes with character measures | `--unb-type-*` |
| **FORMA-GAP-19** | Breakpoint tokens | Media queries are hard-coded inside components | 48 / 64 rem constants |
| **FORMA-GAP-20** | Raised, sunken and chrome surface roles and elevation, per brand | Only primary, secondary and inverse surfaces exist; the Echelon equivalents are scoped to one brand | `color-mix` derivations |
| **FORMA-GAP-21** | Sentence-case status chip | `status-lozenge` is uppercase monospace at 11 px | `.unb-chip` |
| **FORMA-GAP-22** | Task list ("needs your attention") | `work-queue` targets staff queues | `.unb-task` |
| **FORMA-GAP-23** | Domain icons (gift, bank, building, receipt, certificate) | Not in the 80-icon registry | closest registry icon |
| **FORMA-GAP-24** | Approval quorum tracker (two or more distinct approvers) | `readiness-checklist` has no approver-independence rule | checklist text |
| **FORMA-GAP-25** | Accounting period context bar | `scope-trail` and `date-range` carry no period state | scope-trail + freshness |
| **FORMA-GAP-26** | Per-item batch outcome summary | FORMA-GAP-04 assumed one outcome per batch | table of chips |

Equivalent business operations share one implementation:
- **Status presentation:** one chip.
- **Restrictions:** one notice.
- **Money:** one money token.
- **Grant status:** shown the same way on Giving home, grant detail, the charity view and the widget, using the same chip and the same words from the status catalog.

The widget reuses the application layer's tokens inside its iframe rather than a separate implementation.

Filing: FORMA-GAP-18..26 are recorded with `filed_as = pending`. Like the earlier gaps, they are filed in the Forma repository only on the owner's instruction (the owner asked for that explicitly last time). The finding that the primary button is marketing-only should be added to forma#186.

## 8. Visual regression and interaction test specification

- **Reference images.** Each v2 state has committed reference images at 390 and 1280 px (`v2/rendered/<screen>/<state>-<w>.png`). They are rendered with device scale 1, reduced motion and fonts loaded, from a local HTTP server so fonts and axe behave as in production. Images at 320, 640 and 768 are generated during each run and not committed (size).
- **Comparison rule (UNB-UI-V-001).** Per-pixel threshold 0.1; a run fails above 0.5% changed pixels. A reference changes only in a reviewed change.
- **Gates run per state and viewport** (`tools/verify.mjs`): axe (WCAG 2.0–2.2 A/AA, including "incomplete" contrast measured below its required ratio), page overflow, money and reference wrapping, 24 px targets, keyboard walk (focus visible, not obscured, wrap detection), heading outline, unique navigation labels, text spacing (clipping and overflow) and console or resource errors.
- **Dokimos-ready scenarios:** `DKS-UI-001..020` in `data/scenarios.csv`. They cover the 17 required scenarios plus the keyboard, reflow, WCAG, forced-colours, manual screen-reader and grant-validation flows.
- **Not yet automated:** pixel comparison against references (needs a CI image-diff step), forced-colours emulation and manual screen-reader runs. DKS-UI-018 and DKS-UI-019 are marked "Not executed".

## 9. Traceability

Each `UIR-nnn` record in [`data/ui-refinement-trace.csv`](data/ui-refinement-trace.csv) links:
- the refined UI requirement;
- the v1 defects it resolves;
- the **original inbound requirements** it ultimately refines, reached through UI traces, refinement originals (`RR-*`) and gap parents;
- the v2 evidence images;
- the `DKS-UI` scenarios that test it.

Every UIR record reaches at least one inbound original. Defects with no v2 design yet are marked `open` with the roadmap wave that will close them.

<!-- BEGIN GENERATED: uiq-trace -->
| UIR | Refined requirement | Defects | Original inbound | Status | Tested by |
|---|---|---|---|---|---|
| UIR-001 | UNB-UI-A-001 | UXD-002 | UNB-UX-005, UNB-UX-006, UNB-EXP-001, UNB-PLAT-005 | resolved-in-v2 | DKS-UI-017 |
| UIR-002 | UNB-UI-A-002 | UXD-003 | UNB-UX-002, UNB-UX-005, ADV-907, ADV-944 | resolved-in-v2 | DKS-UI-015 |
| UIR-003 | UNB-UI-A-003 | UXD-038, UXD-050, UXD-051 | UNB-UX-002, UNB-UX-006, UXA-1, UI-T21 | resolved-in-v2 | DKS-UI-017, DKS-UI-018 |
| UIR-004 | UNB-UI-A-004 | UXD-057 | UNB-UX-005, UI-T20, ADV-930, ADV-961 | resolved-in-v2 | DKS-UI-016 |
| UIR-005 | UNB-UI-DS-001 | UXD-040 | INT-019, UNB-EXP-002, REQ-X-030 | resolved-in-v2 | DKS-UI-014 |
| UIR-006 | UNB-UI-DS-002 | UXD-006, UXD-011 | INT-019, UNB-UX-007, UX-S02 | resolved-in-v2 | — |
| UIR-007 | UNB-UI-DS-005 | UXD-025 | UNB-UX-002, UNB-EXP-001, UNB-PLAT-005 | resolved-in-v2 | DKS-UI-007, DKS-UI-010 |
| UIR-008 | UNB-UI-DS-006 | UXD-010, UXD-057 | INT-013, AIN-006, REQ-X-051, GOV-AUTH-001 | resolved-in-v2 | — |
| UIR-009 | UNB-UI-DS-008 | UXD-012, UXD-047 | UNB-UX-003, UNB-UX-007, REQ-X-011 | resolved-in-v2 | DKS-UI-019 |
| UIR-010 | UNB-UI-DS-010 | UXD-028 | INT-012, UNB-UX-004, ADV-921, ADV-946 | resolved-in-v2 | DKS-UI-005, DKS-UI-006, DKS-UI-007, DKS-UI-008 |
| UIR-011 | UNB-UI-DS-013 | UXD-041 | UX-S09, ADV-946, ADM-036 | resolved-in-v2 | DKS-UI-003, DKS-UI-004 |
| UIR-012 | UNB-UI-DS-014 | UXD-013 | INT-011, UI-T20, ADM-036 | resolved-in-v2 | DKS-UI-002 |
| UIR-013 | UNB-UI-DS-015 | UXD-011, UXD-024 | INT-013, ADV-906, ADV-944, AIN-006, REQ-X-051, GOV-AUTH-001 | resolved-in-v2 | DKS-UI-020 |
| UIR-014 | UNB-UI-DS-017 | UXD-022, UXD-023, UXD-026, UXD-035, UXD-042, UXD-044, UXD-058 | UNB-UX-002, UXA-2, TVR-UX-003, REQ-X-011, UNB-RES-001 | partly-open | DKS-UI-020 |
| UIR-015 | UNB-UI-F-001 | UXD-001, UXD-005 | UNB-UX-005, UNB-GFUND-013 | resolved-in-v2 | DKS-UI-016 |
| UIR-016 | UNB-UI-F-002 | UXD-009 | UX-S02, UNB-ACC-005, REQ-X-022 | resolved-in-v2 | DKS-UI-001 |
| UIR-017 | UNB-UI-F-004 | UXD-007, UXD-029, UXD-048 | INT-009, REQ-X-003, UNB-GFUND-013, UNB-ACC-006 | partly-open | — |
| UIR-018 | UNB-UI-F-005 | UXD-059 | REQ-X-003, UNB-ACC-006 | resolved-in-v2 | — |
| UIR-019 | UNB-UI-F-006 | UXD-047 | INT-012, ADV-918, ADV-947, REQ-X-022, REQ-X-007 | resolved-in-v2 | DKS-UI-005 |
| UIR-020 | UNB-UI-F-007 | UXD-009 | INT-010, UNB-ACC-005, REQ-X-022 | resolved-in-v2 | DKS-UI-006, DKS-UI-019 |
| UIR-021 | UNB-UI-G-002 | UXD-018 | REQ-X-011, REQ-X-002, REQ-X-004, UNB-GFUND-014 | resolved-in-v2 | — |
| UIR-022 | UNB-UI-G-005 | UXD-024 | AIN-006, REQ-X-051, GOV-AUTH-001 | resolved-in-v2 | — |
| UIR-023 | UNB-UI-G-006 | UXD-020 | REQ-X-004 | resolved-in-v2 | DKS-UI-011 |
| UIR-024 | UNB-UI-G-007 | UXD-014, UXD-060 | REQ-X-030 | resolved-in-v2 | DKS-UI-009 |
| UIR-025 | UNB-UI-G-010 | UXD-017 | INT-001, UNB-TEN-004 | resolved-in-v2 | — |
| UIR-026 | UNB-UI-G-011 | UXD-053 | REQ-X-051, UNB-AI-STD-003, UNB-AI-STD-001 | partly-open | — |
| UIR-027 | UNB-UI-G-016 | UXD-005, UXD-021, UXD-037 | REQ-X-025 | partly-open | — |
| UIR-028 | UNB-UI-G-017 | UXD-001 | UNB-GFUND-013 | resolved-in-v2 | — |
| UIR-029 | UNB-UI-N-001 | UXD-016, UXD-062 | INT-001, UX-S02, REQ-X-030 | resolved-in-v2 | — |
| UIR-030 | UNB-UI-N-003 | UXD-003 | INT-009, UNB-UX-001, ADV-930 | resolved-in-v2 | DKS-UI-015 |
| UIR-031 | UNB-UI-N-004 | UXD-004, UXD-014, UXD-015, UXD-016, UXD-017 | INT-001, UNB-UX-007, UNB-TEN-004 | resolved-in-v2 | DKS-UI-010 |
| UIR-032 | UNB-UI-N-005 | UXD-004, UXD-015, UXD-021, UXD-048 | UX-S02, INT-002 | resolved-in-v2 | — |
| UIR-033 | UNB-UI-N-006 | UXD-008 | UX-S02, UNB-UX-001, UNB-ACC-005, REQ-X-022 | resolved-in-v2 | DKS-UI-001 |
| UIR-034 | UNB-UI-R-001 | UXD-061 | INT-009, UNB-UX-005, UNB-UX-001, UNB-PLAT-004 | resolved-in-v2 | DKS-UI-013, DKS-UI-016 |
| UIR-035 | UNB-UI-R-002 | UXD-006 | INT-009 | resolved-in-v2 | — |
| UIR-036 | UNB-UI-S-002 | UXD-027, UXD-045, UXD-046 | REQ-X-011, UNB-CHAR-001 | partly-open | — |
| UIR-037 | UNB-UI-S-003 | UXD-019, UXD-043 | UNB-GRANT-002, REQ-X-011, UNB-GRANT-011, UNB-GRANT-027, UNB-GFUND-013 | resolved-in-v2 | DKS-UI-020 |
| UIR-038 | UNB-UI-S-004 | UXD-018, UXD-019, UXD-020 | REQ-X-011, TVR-GR-004, REQ-X-004 | resolved-in-v2 | DKS-UI-011 |
| UIR-039 | UNB-UI-S-005 | UXD-056 | UNB-ACC-006 | partly-open | — |
| UIR-040 | UNB-UI-S-007 | UXD-030, UXD-049 | REQ-X-035, UNB-PLAT-005 | partly-open | — |
| UIR-041 | UNB-UI-S-008 | UXD-031, UXD-032 | REQ-X-048 | resolved-in-v2 | DKS-UI-012 |
| UIR-042 | UNB-UI-S-009 | UXD-033, UXD-052 | REQ-X-017, AIN-003, UNB-AI-STD-003, UNB-EXT-004 | partly-open | — |
| UIR-043 | UNB-UI-S-010 | UXD-034, UXD-054 | REQ-X-045, ADM-027, REQ-X-051, GOV-AUTH-001 | partly-open | — |
| UIR-044 | UNB-UI-S-012 | UXD-035, UXD-055 | REQ-X-058, REQ-X-077, REQ-X-005, UNB-GRANT-028 | resolved-in-v2 | — |
| UIR-045 | UNB-UI-S-013 | UXD-036 | AIN-023, ADM-030 | partly-open | — |
| UIR-046 | UNB-UI-V-001 | UXD-051, UXD-063 | UNB-UX-005, INT-020, ADV-700, ADV-701 | resolved-in-v2 | — |
| UIR-047 | UNB-UI-V-002 | UXD-039 | UNB-UX-001, UNB-UX-003, INT-015 | resolved-in-v2 | — |
| UIR-048 | UNB-UI-W-001 | UXD-061 | INT-015, REQ-X-063 | resolved-in-v2 | DKS-UI-013 |
<!-- END GENERATED: uiq-trace -->

## 10. Prioritised design improvement roadmap

| Wave | Work | Closes | Exit criterion |
|---|---|---|---|
| 0 (owner) | Approve RCD-050..053 (default brand, Forma ownership of the application layer, date format, phone navigation); file FORMA-GAP-18..26; add the primary-button finding to forma#186 | Decisions | Owner sign-off; issues filed |
| 1 (done in this pass) | Design system layer; Giving home in 12 states; S-003, S-004, S-008, S-011, S-012 and W-001 in v2; verification harness | 47 of 63 v1 defects; all 17 required scenarios | Every v2 state passes `verify.mjs` at 5 viewports |
| 2a | Consolidate the redesign passes' screen-local components (`v2/proposed/*.css`) into the shared layer; one helper per pattern | Duplication between screens | No pattern defined twice |
| 2 | Redesign S-002 charity search, S-005 add money, S-006 people and successors, S-007 documents, S-009 case workspace (with the 18 typed panels of S-014/P-*) | UXD-026, 027, 029, 030, 033, 044, 045, 046, 049, 052, 053, 056 | Same gate; critique items closed |
| 3 | S-010 policy configuration, S-013 notification preferences; workspace views S-015..S-061 for finance and treasury (compact density) | UXD-034, 036, 054 | Same gate in compact density |
| 4 | Pixel-diff CI step; forced-colours emulation; manual screen-reader review (NVDA, VoiceOver, TalkBack); moderated usability tests on phones (INT-020) with task success, time and errors | DKS-UI-018, DKS-UI-019, UNB-UX-006 | Recorded results; defects triaged |
| 5 | Move the interim layer into Forma once the gap issues land; delete `unbound-app.css` blocks one by one | RCD-051 | Every v2 page renders with Forma alone |

Still open by design:
- **Dark theme.** The derived tokens have dark values, but no v2 state was verified in dark.
- **High-contrast theme.** Forma ships one, but it was not tested with the application layer.
- **RTL (ADV-931).** Not tested.

## 11. What this does not claim

- That Forma supports the proposed components: it does not. Each is a recorded gap.
- That the v2 pages are production UI. They are static prototypes with synthetic data and no behaviour.
- That the Echelon theme's candidate palette is approved: it is not, and it is not used.
- That passing automated checks equals accessibility. Manual assistive-technology testing is still required.

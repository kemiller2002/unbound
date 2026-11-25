/// The records ui_quality.py constructs (report 18): design-system, navigation, financial-presentation, responsive,
/// accessibility and visual-regression requirements, the revisions of existing UNB-UI-* rules and decisions RCD-050..053.
/// Each binding is a transcription of the Python constant named in its comment, in the same order and wording.
module Unbound.Authoring.UiQualityData

/// The arguments of ui_quality.py's req(): one UI requirement before it is built into its JSON record.
type Spec =
    { Id: string
      Kind: string
      Title: string
      Statement: string
      Traces: string list
      Forma: string list
      Gaps: string list
      /// Acceptance criteria as (given, when, then).
      Acs: (string * string * string) list
      Detail: (string * string) list
      Folio: string list }

/// req(id_, kind, title, statement, traces, forma=(), gaps=(), acs=(), detail=None, folio=()), defaults written out.
let req id kind title statement traces forma gaps acs detail folio : Spec =
    { Id = id; Kind = kind; Title = title; Statement = statement; Traces = traces; Forma = forma; Gaps = gaps
      Acs = acs; Detail = detail; Folio = folio }

/// A text revision of an existing record: (old, new, why).
type Revision = string * string * string

let V2 = "requirements/ui/designs/v2"

/// ui_quality.py: ORIGIN
let origin : string =
    "ui-quality 2026-11-23 (owner addendum: UI quality, design system and experience refinement; report 18)"

/// ui_quality.py: FAMILIES
let families : string list =
    [ "UNB-UI-DS-"; "UNB-UI-N-"; "UNB-UI-F-"; "UNB-UI-R-"; "UNB-UI-A-"; "UNB-UI-V-" ]

/// ui_quality.py: DS
let ds : Spec list =
    [ req "UNB-UI-DS-001" "design-system" "Token source and brand layering"
          "All colour, type family, radius and focus values in Unbound UI MUST come from Forma semantic tokens (--forma-color-*, --forma-type-*, --forma-space-*, --forma-primitive-radius-*, --forma-primitive-size-target-min) as set by a brand manifest compiled with Forma's BrandCompiler. Application CSS MAY derive values (color-mix) from those tokens but MUST NOT declare --forma-* properties or hard-code hex colours. The Unbound default brand ('unbound-default') MUST use the Balanced Charcoal and Bronze values of the Echelon theme 1.1.0 manifest (kemiller2002/echelon-design-system theme/echelon.brand.json): charcoal #202421 chrome/heading, carbon #171a18 text, parchment #f2efe7 canvas, oxide bronze #905831 accent (#a87e5e dark), with Newsreader, Manrope and IBM Plex Mono self-hosted under SIL OFL 1.1. The README candidate palette (#20252A, #B7844B, ...) MUST NOT be used: it is unverified."
          [ "INT-019"; "UNB-EXP-002"; "UNB-UI-G-014"; "RCD-050" ]
          [ "app-identity" ]
          [ "FORMA-GAP-20" ]
          [ ("the v2 prototype CSS", "it is scanned for '--forma-' declarations and hex colour literals outside documented derivations", "there are none; every colour resolves from a --forma-color-* token")
            ("the unbound-default brand manifest", "it is compiled with Forma's BrandCompiler", "compilation passes and the output values equal echelon.brand.json for every colour role")
            ("a sponsor manifest (example-harbor)", "it is swapped in without other changes", "every v2 screen restyles, and the verification harness passes at all five viewports") ]
          ([ ("evidence", V2 + "/unbound-app.css; requirements/ui/designs/vendor/unbound-brands/README.md") ])
          []
      req "UNB-UI-DS-002" "design-system" "Application type scale"
          "Application screens MUST use a fixed application type scale, not Forma's marketing display sizes (fluid-display-1 up to 7.25rem with a 14ch heading measure): page title clamp(1.5rem, 1.3rem + 0.8vw, 1.875rem) weight 650; section title 1.125rem; body 1rem/1.5; small 0.875rem; micro 0.8125rem (minimum for any text). Headings MUST NOT have a character measure that wraps short titles. The display face (Newsreader in the default brand) is reserved for the one headline money figure per screen; all other text uses the sans face. Labels and chips use sentence case in the body face; uppercase monospace labels MUST NOT be used for status or navigation."
          [ "INT-019"; "UNB-UX-007"; "UX-S02" ]
          [ "text-roles"; "section-heading" ]
          [ "FORMA-GAP-18" ]
          [ ("any v2 page at 1280 px", "computed font sizes of all visible text are collected", "no text is smaller than 13 px and the h1 is between 24 and 30 px")
            ("the page title 'Lindqvist Family Fund'", "it renders at 320, 390 and 1280 px", "it occupies one line at 390 and 1280 px") ]
          []
          []
      req "UNB-UI-DS-003" "design-system" "Spacing and density"
          "Spacing MUST use Forma's eight-step scale (--forma-space-1..8 = 0.25, 0.5, 0.75, 1, 1.25, 1.5, 2, 3 rem): 4 within controls, 12-16 between related items, 20-24 between panels, 32-48 between page regions. Two densities are defined: comfortable (donor, advisor, charity, widget; table rows at least 44 px) and compact (staff work queues and ledgers; rows at least 36 px with 24 px targets). Density MUST be a single switch per surface class, not per component; Forma has no density token (FORMA-GAP-18)."
          [ "INT-009"; "INT-019" ]
          [ "stack"; "cluster" ]
          [ "FORMA-GAP-18" ]
          [ ("the giving home at 1280 px", "gaps between panels are measured", "every gap equals 20 px (space-5)")
            ("a staff queue in compact density", "row heights and target sizes are measured", "rows are at least 36 px tall and every interactive target is at least 24 by 24 px") ]
          []
          []
      req "UNB-UI-DS-004" "design-system" "Colour roles and semantic status"
          "Colour roles are: canvas (surface-primary), raised panel (derived, lighter than canvas), sunken (derived), chrome (surface-inverse) with text-inverse, accent (accent-primary) for the single primary action, the active navigation rule and the 'available' allocation segment only, and four status roles (success, warning, danger, info). Status colour MUST always be paired with a glyph and words. Accent MUST NOT be used for status. Secondary text (text-secondary) MUST NOT be placed on surface-secondary or sunken surfaces (Echelon AUDIT §6: it fails 4.5:1 there); use text-on-secondary-surface."
          [ "INT-019"; "ADV-928"; "ADV-692" ]
          [ "status-lozenge" ]
          [ "FORMA-GAP-20" ]
          [ ("every v2 page", "axe color-contrast runs at all five viewports", "there are no violations")
            ("a page rendered with a greyscale filter", "a reviewer identifies each status", "every status is identifiable from its glyph and words") ]
          []
          []
      req "UNB-UI-DS-005" "design-system" "Interaction states"
          "Every interactive component MUST define rest, hover, focus-visible, active, disabled-with-reason, busy and error states. Focus-visible is a 3 px focus-ring outline with 2 px offset plus a 2 px focus-gap halo, so it is visible on canvas, raised and chrome surfaces. Disabled controls that the user might expect to use MUST show why (FORMA-GAP-11); controls the user can never use MUST be omitted. Busy state uses aria-busy on the control and keeps its label."
          [ "UNB-UX-002"; "UNB-UI-G-008" ]
          [ "button" ]
          [ "FORMA-GAP-01"; "FORMA-GAP-11" ]
          [ ("a keyboard user tabbing through any v2 page", "each focus stop is inspected", "each has an outline of at least 2 px or a box-shadow ring, and none is covered by a fixed bar")
            ("a donor in a restricted fund", "the home renders", "'Recommend a grant' is absent from page actions and the notice says why recommending is paused and what still works") ]
          []
          []
      req "UNB-UI-DS-006" "design-system" "Buttons and action emphasis"
          "Each region MUST have at most one primary (filled accent) action; other actions are secondary (outlined) or quiet (underlined text with chevron for navigation). Button labels are verbs naming the outcome ('Recommend a grant', 'Review and confirm'), never 'OK', 'Submit' alone or 'Click here'. Buttons are at least 44 px tall in comfortable density. Forma 1.0.2 styles data-forma-variant=primary only inside the marketing .forma-site frame, so the application primary variant is a Forma gap (forma#186)."
          [ "INT-013"; "UNB-UI-G-005" ]
          [ "button" ]
          [ "FORMA-GAP-01" ]
          [ ("any v2 page", "primary-variant buttons are counted per panel and page header", "no region has more than one")
            ("the giving home", "button heights are measured at 390 px", "every button is at least 44 px tall") ]
          []
          []
      req "UNB-UI-DS-007" "design-system" "Panels and grouping"
          "Related content is grouped in raised panels with a header (title, optional count or total), body and optional footer for navigation links. Panels MUST NOT nest. A panel needing attention uses a 4 px warning rule on its leading edge plus a text title ('Needs your attention'); colour alone MUST NOT signal it."
          [ "INT-019"; "UX-S02" ]
          [ "surface"; "section" ]
          [ "FORMA-GAP-20" ]
          [ ("the giving home", "panels are inspected", "no panel contains another panel and each panel has an accessible name") ]
          []
          []
      req "UNB-UI-DS-008" "design-system" "Status chip"
          "Status MUST render as a sentence-case chip: status glyph from the Forma icon registry (success, pending, error, info, help-circle), the status words from the status catalog, 1 px border in the status colour, body face at 13 px semibold. This replaces Forma's uppercase monospace status-lozenge in donor, advisor and charity surfaces (it read as developer output in v1). Staff dense ledgers MAY keep status-lozenge."
          [ "UNB-UX-003"; "UNB-UI-G-001"; "UNB-UX-007" ]
          [ "status-lozenge"; "icon" ]
          [ "FORMA-GAP-21" ]
          [ ("the grant 'On hold' chip", "it is rendered in forced-colours mode", "the glyph, border and words remain visible")
            ("any chip", "its text is read by a screen reader", "the words alone state the status") ]
          []
          []
      req "UNB-UI-DS-009" "design-system" "Task list (things that need the user)"
          "Items that need the user MUST be presented as a task list: icon in a status-coloured ring, a title that names the subject in plain words, one sentence of reason and consequence, and exactly one primary next action (secondary actions allowed). Informational items with no user action MUST NOT appear in the task list; they belong in activity or record status."
          [ "UNB-UI-S-001"; "UX-S02"; "ADV-925" ]
          [ "work-queue"; "priority-stack" ]
          [ "FORMA-GAP-22" ]
          [ ("the giving home with a certification due and a held grant", "the attention panel renders", "each item has a title, a reason sentence and one named action, and the panel precedes grants in progress and activity in source order") ]
          []
          []
      req "UNB-UI-DS-010" "design-system" "Notices for restrictions, staleness and outages"
          "Account-level conditions (restricted, suspended, succession, stale balance, integration unavailable) MUST use one notice pattern at the top of the content: leading glyph, a title stating the condition in plain words, a body saying why, since when, what still works and what to expect, and at most two actions. The same root cause MUST produce one notice, not several (ADV-921)."
          [ "INT-012"; "UNB-UX-004"; "ADV-921"; "ADV-946" ]
          [ "callout"; "fault-banner" ]
          []
          [ ("a fund whose pricing integration is down", "the home renders", "one notice titled 'Investment values are temporarily unavailable' states that Available to grant is unaffected")
            ("a suspended fund", "the home renders", "the notice gives the date, the reason and the action that lifts the suspension") ]
          []
          []
      req "UNB-UI-DS-011" "design-system" "Iconography"
          "Icons MUST come from the Forma icon registry (80 icons, 24 px grid, 1.8 stroke, round caps), rendered at 16, 20 or 24 px, decorative (aria-hidden) beside visible text, or named by the parent control's aria-label when standalone. Domain concepts with no registry icon (grant, contribution, charity, bank) use the closest registry icon ('send' for grant, 'add' for add money) until FORMA-GAP-23 adds domain glyphs. Icons MUST NOT be the only carrier of meaning."
          [ "INT-019"; "ADV-960" ]
          [ "icon" ]
          [ "FORMA-GAP-23" ]
          [ ("the v2 build", "every icon name is resolved", "all resolve in the Forma registry (the helper throws otherwise)") ]
          []
          []
      req "UNB-UI-DS-012" "design-system" "Sponsor white-labelling"
          "A sponsor brand MUST be a Forma brand manifest (identity, fonts, radius, light and dark colour maps, terms) compiled with Forma's BrandCompiler and passing its contrast checks. Every application-layer component MUST be brand-agnostic (derived from --forma-* tokens, no brand selector), so a sponsor brand restyles the whole product. The Echelon theme's application layer (echelon-app.css: app bar, nav tabs, panels, chips) is scoped to data-forma-brand='echelon' and MUST NOT be used for Unbound, because it would not apply to sponsor brands. Sponsor terms (Fund, Donor, Grant) resolve through the terminology catalog (G-007); protected legal text cannot be re-branded."
          [ "UNB-UI-G-014"; "UNB-UI-G-007"; "INT-019"; "ADV-716" ]
          [ "app-identity" ]
          [ "FORMA-GAP-20" ]
          [ ("the giving home under example-harbor", "it is compared with unbound-default", "only colours, fonts, radius, mark and brand name differ; structure, text and the legal disclosure are identical")
            ("a sponsor manifest whose accent fails 4.5:1 on its canvas", "it is compiled", "compilation fails with a contrast finding and it is not deployable") ]
          []
          []
      req "UNB-UI-DS-013" "design-system" "Empty and onboarding states"
          "An empty state MUST say what will appear, why it is empty and offer the single next sensible action. A new fund MUST show an ordered getting-started checklist (open fund, first contribution, first grant) with done/current/todo states instead of separate empty panels and a $0.00 card. Empty compliance or work views MUST state what was checked and when ('Nothing needs you. Last checked 9:12 AM'), never imply 'no issues' without scope (ADV-946)."
          [ "UNB-UI-G-013"; "UX-S09"; "ADV-946" ]
          [ "empty-state"; "readiness-checklist" ]
          []
          [ ("a fund opened today with no contributions", "the home renders", "a 'Get your fund started' checklist shows 1 of 3 done with 'Add money' as the only primary action") ]
          []
          []
      req "UNB-UI-DS-014" "design-system" "Skeleton loading and layout stability"
          "Loading states MUST render skeletons with the geometry of the final layout (same panels, headings and row count), keep real headings visible, set aria-busy on the loading region with a polite status message, and stop animating under prefers-reduced-motion. Content arrival MUST NOT shift layout above the fold by more than 0.1 CLS. No blank shell (INT-011)."
          [ "INT-011"; "UNB-UI-G-013"; "UI-T20" ]
          [ "skeleton" ]
          []
          [ ("the home loading state and the normal state at 1280 px", "panel bounding boxes are compared", "panel positions differ by no more than 24 px vertically")
            ("reduced motion requested", "the loading state renders", "skeletons do not animate") ]
          []
          []
      req "UNB-UI-DS-015" "design-system" "Dialogs"
          "Dialogs are reserved for consequential confirmation (G-005), unknown-outcome resolution and destructive actions. A dialog MUST be a native modal dialog with a title naming the action, the facts that will change, reversibility, who acts next and timing, a confirm control naming the action and a cancel control; focus moves to the dialog title and returns to the invoking control. One confirmation per action: no 'Are you sure?' after a review step."
          [ "INT-013"; "UNB-UI-G-005"; "ADV-906"; "ADV-944" ]
          [ "dialog" ]
          [ "FORMA-GAP-14" ]
          [ ("a grant recommendation reviewed on a review step", "the donor presses 'Submit grant recommendation'", "no second confirmation dialog opens") ]
          []
          []
      req "UNB-UI-DS-016" "design-system" "Notifications and task resolution"
          "The bell shows the count of unread notifications in its accessible name. A notification that asks for action links to the task and disappears from 'needs attention' only when the underlying task is resolved, never on dismissal (ADV-923). Snooze MUST NOT outlive a legal deadline (ADV-924). Push and email previews contain no amounts or charity names (INT-017)."
          [ "INT-017"; "UNB-UI-G-015"; "ADV-922"; "ADV-923"; "ADV-924"; "ADV-948" ]
          [ "feed-item" ]
          []
          [ ("a certification task that the donor dismisses from notifications", "the home reloads", "the task is still listed under 'Needs your attention'") ]
          []
          []
      req "UNB-UI-DS-017" "design-system" "Forms and validation"
          "Forms are single-column with labels above fields, help text tied by aria-describedby, required fields marked in words, inline validation on blur or submit (never while typing a number), an error summary at the top linking to each field, and entered values preserved. Amount fields accept '$', commas and spaces, use inputmode=decimal and echo the parsed amount in words beside the field. Redundant entry is avoided (WCAG 3.3.7)."
          [ "UNB-UX-002"; "UNB-UI-G-012"; "UXA-2"; "TVR-UX-003" ]
          [ "text-field"; "validation-summary"; "validation-message"; "form" ]
          [ "FORMA-GAP-02" ]
          [ ("an amount above Available to grant", "the donor submits", "the error summary links to the amount field and the message states the available amount and how to proceed") ]
          []
          []
      req "UNB-UI-DS-018" "design-system" "Charts and reporting"
          "Charts MUST have a visible title, labelled axes with units, direct value labels or a data table equivalent reachable by keyboard, a text summary of the main finding, series distinguishable by pattern or label as well as colour, and freshness. A chart MUST NOT be the only presentation of a financial figure. Forma has no chart primitives (FORMA-GAP-05)."
          [ "INT-010"; "ADV-692"; "ADV-693"; "ADV-721" ]
          [ "disclosure" ]
          [ "FORMA-GAP-05" ]
          [ ("the sponsor overview chart", "a screen reader user navigates it", "a summary and a data table with every plotted value are available") ]
          []
          [] ]

/// ui_quality.py: NAV
let nav : Spec list =
    [ req "UNB-UI-N-001" "navigation" "Application frame"
          "Every authenticated surface uses one application frame: skip link; chrome app bar with sponsor brand mark and name, product context, Help, Notifications (with unread count) and account menu; primary navigation; main region; legal-sponsor footer. The marketing site frame (.forma-site) MUST NOT be used for application screens (v1 did, which produced marketing-scale headings)."
          [ "INT-001"; "UNB-UI-G-014"; "UX-S02" ]
          [ "skip-link"; "app-identity"; "navigation-shell" ]
          [ "FORMA-GAP-06" ]
          [ ("any v2 page", "landmarks are listed", "there is exactly one main, one banner, one visible primary navigation and a footer with the legal disclosure") ]
          []
          []
      req "UNB-UI-N-002" "navigation" "Primary destinations by audience"
          "Donor and advisor: Home, Grants, Contributions, Documents, People (Notifications and Settings in the account menu). Staff: Work queue, Cases, Reconciliation, Funds, Policy. Charity: Grants received, Organization. Destinations are places named by nouns in the sponsor's terms, never actions ('Add money' is a page action, not a destination); no more than five primary destinations per audience."
          [ "UNB-UX-001"; "UNB-UI-G-007" ]
          [ "navigation-shell" ]
          []
          [ ("each audience's frame", "primary destinations are counted", "there are at most five, each with an icon and visible text") ]
          []
          []
      req "UNB-UI-N-003" "navigation" "Responsive navigation pattern"
          "From 64rem: persistent side navigation (15.5rem) with the current page marked by an accent rule, weight and aria-current. Below 64rem: a fixed bottom tab bar with at most four destinations, visible labels and 56 px tall targets, plus a Menu button for the rest. Page scroll-padding MUST keep focused elements clear of the tab bar (WCAG 2.4.11). No sticky action bar is stacked on top of the tab bar."
          [ "INT-009"; "UNB-UX-001"; "ADV-930" ]
          [ "navigation-shell" ]
          [ "FORMA-GAP-06" ]
          [ ("the home at 390 px", "fixed bars are measured", "fixed chrome occupies at most 10% of the 844 px viewport height")
            ("a keyboard walk at 390 px", "each focused element is hit-tested at its centre", "no focused element is covered by the tab bar") ]
          []
          []
      req "UNB-UI-N-004" "navigation" "Scope and fund context"
          "The current fund is the page title and doubles as the fund switcher (button with aria-haspopup) when the user has more than one fund. The sponsor is identified by the brand in the app bar and the legal footer. Internal references (F-2026-000418) are not shown in donor summaries; they appear in fund details and on documents. Staff surfaces keep the full scope trail (sponsor, program, fund or case) because staff switch scopes."
          [ "INT-001"; "UNB-UI-G-010"; "UNB-UX-007" ]
          [ "scope-trail" ]
          []
          [ ("a donor with two funds", "they open the fund switcher and pick the other fund", "all data reloads under the new fund after re-authorization (G-010)")
            ("the donor home", "visible text is scanned", "no internal fund or rule identifier appears") ]
          []
          []
      req "UNB-UI-N-005" "navigation" "Page header"
          "Each page has one header: optional context line, h1 naming the subject (fund, grant, case), one line of role or status context, and the page's actions (at most one primary). On phones the actions sit full width under the title, not in a sticky bar."
          [ "UX-S02"; "INT-002" ]
          [ "record-header"; "section-heading" ]
          []
          [ ("any v2 page", "headings are listed", "there is exactly one h1 and no heading level is skipped") ]
          []
          []
      req "UNB-UI-N-006" "navigation" "Information architecture of the donor home"
          "The donor home answers in order: what can I give (Available to grant with breakdown), what needs me (attention tasks), what is in flight (grants in progress), what happened (recent activity). Source order follows that priority at every width; on wide screens the balance takes the main column and attention sits beside it. On a 390x844 phone the headline figure and the 'Needs your attention' heading MUST both be visible in the first viewport above the tab bar; breakdown explanations move behind 'How we calculate this' on phones. Market value is one line of the breakdown, not a peer card. Recent activity and grants in progress show at most three items with a link to the full list."
          [ "UX-S02"; "UNB-UI-S-001"; "UNB-UI-G-004" ]
          [ "priority-stack" ]
          []
          [ ("the home at 390x844 with two attention items", "the first viewport is captured", "the Available to grant figure and the 'Needs your attention' heading are both fully visible above the tab bar")
            ("the home at 1280 px", "the first viewport is captured", "Available to grant, both attention items and the page actions are visible without scrolling") ]
          []
          [] ]

/// ui_quality.py: FIN
let fin : Spec list =
    [ req "UNB-UI-F-001" "financial-presentation" "Money never wraps"
          "A money value (sign, currency symbol, digits, separators, cents) MUST render as one unbreakable token (white-space: nowrap) with tabular lining figures; containers reflow around it. This applies at 320 px and with WCAG 1.4.12 text spacing. v1 broke '$100,000.00' across two lines in three equal metric cards at 1280 px."
          [ "UNB-UI-G-017"; "UNB-UX-005" ]
          [ "identifier" ]
          [ "FORMA-GAP-08" ]
          [ ("every v2 page at 320, 390, 640, 768 and 1280 px, with and without text-spacing overrides", "each .unb-money element's line boxes are counted", "every money value occupies exactly one line") ]
          []
          []
      req "UNB-UI-F-002" "financial-presentation" "Headline figure"
          "Each fund screen has exactly one headline figure: Available to grant, in the display face at clamp(2rem, 1.6rem + 1.6vw, 2.75rem) with cents at 62% size raised (still read as one number), followed by its confirmation source and absolute update time. Other amounts use body size."
          [ "UNB-UI-G-004"; "UX-S02" ]
          [ "metric-card" ]
          [ "FORMA-GAP-08" ]
          [ ("the home", "elements using the display size are counted", "exactly one money value uses it, and its accessible text reads as one amount") ]
          []
          []
      req "UNB-UI-F-003" "financial-presentation" "Signs and negative values"
          "Outflows show a true minus (U+2212) and a text label naming the movement ('Grant paid'); inflows show '+' only in mixed activity lists. Colour MUST NOT be the only indicator of sign; red is not used for ordinary outflows (a grant is not a loss)."
          [ "UNB-UI-G-017" ]
          []
          [ "FORMA-GAP-08" ]
          [ ("recent activity with a grant payment", "it renders", "the amount reads '−$2,500.00' beside the label 'Grant paid', in the normal text colour") ]
          []
          []
      req "UNB-UI-F-004" "financial-presentation" "Tables of money"
          "Amount columns are right-aligned, tabular, nowrap, with the currency symbol on every value. Date columns use absolute dates. Below a container width of 34rem, tables become two-line records (subject and amount on line one; status and date on line two) without dropping columns; the header row stays available to assistive technology."
          [ "UNB-UI-G-017"; "UNB-UI-G-018"; "INT-009" ]
          [ "dense-ledger"; "data-grid" ]
          [ "FORMA-GAP-03" ]
          [ ("recent activity in the 1280 px side column", "it renders", "it uses the two-line record layout and no cell wraps mid-word")
            ("recent activity at 1280 px in a wide panel", "amount cells are measured", "their right edges align within 1 px") ]
          []
          []
      req "UNB-UI-F-005" "financial-presentation" "Dates and times"
          "Dates use the sponsor locale's medium format (en-US: 'Nov 30, 2026'); times add the sponsor time zone abbreviation ('4:58 PM CT'). 'Today'/'Yesterday' MAY replace the date for update times only. Legally significant dates (gift, approval, payment) are always absolute (G-018). Every date is a <time datetime> element."
          [ "UNB-UI-G-018"; "RCD-052" ]
          []
          []
          [ ("any v2 page", "time elements are listed", "every visible date is inside a time element with an ISO datetime") ]
          []
          []
      req "UNB-UI-F-006" "financial-presentation" "Freshness wording"
          "Freshness is stated once per figure in plain words ('Updated today at 4:58 PM CT', 'Market value, Nov 30 close'). When a figure is not current, it shows a 'Not current' chip, the last confirmed time, and a notice explaining what is still safe to do. The words 'Current', 'Calculated' or 'Ledger confirmed' as standalone badges are not used."
          [ "UNB-UI-G-003"; "INT-012"; "ADV-918"; "ADV-947" ]
          [ "freshness" ]
          []
          [ ("a stale Available to grant", "the home renders at 390 px", "the 'Not current' chip and the last confirmed time are visible in the balance panel") ]
          []
          []
      req "UNB-UI-F-007" "financial-presentation" "Fund composition"
          "Fund composition is a horizontal proportion bar (Available, Committed, Settling) with a legend list that carries the amounts and plain explanations, ending in a ruled 'Total fund value' row. The bar is decorative (aria-hidden); the list is the accessible source. Segments differ by pattern as well as colour. When fund value is unavailable the bar is omitted and the total row says 'Unavailable'."
          [ "UNB-UI-G-004"; "INT-010" ]
          [ "allocation" ]
          [ "FORMA-GAP-12" ]
          [ ("the home in forced-colours mode", "the bar renders", "segments remain distinguishable and the legend carries all amounts") ]
          []
          [] ]

/// ui_quality.py: RESP
let resp : Spec list =
    [ req "UNB-UI-R-001" "responsive" "Breakpoints and viewports"
          "Three layout ranges: phone below 48rem, tablet 48-64rem, desktop from 64rem. Every screen MUST be verified at 320 (400% reflow), 390 (phone), 640x450 (1280 at 200% zoom), 768 (tablet) and 1280 (desktop) CSS px, and embedded widgets at 360 and 600 px frame widths. Forma has no breakpoint tokens (FORMA-GAP-19)."
          [ "INT-009"; "UNB-UX-005"; "UNB-UI-G-009" ]
          []
          [ "FORMA-GAP-19" ]
          [ ("each v2 page", "the verification harness runs", "results exist for all five viewports and none has horizontal page overflow") ]
          []
          []
      req "UNB-UI-R-002" "responsive" "Container-driven components"
          "Components that sit in columns of varying width (tables, legends, task lists) MUST adapt to their container width (container queries), not the viewport, so the same component works in a main column, a side column and a phone."
          [ "INT-009" ]
          []
          [ "FORMA-GAP-15" ]
          [ ("the activity table in the 1280 px side column and in a 390 px phone", "both render", "both use the two-line record layout") ]
          []
          []
      req "UNB-UI-R-003" "responsive" "Source order equals priority"
          "DOM order MUST equal priority order; wider layouts may place regions with grid areas but MUST NOT reorder the reading or focus sequence relative to the narrow layout."
          [ "INT-009"; "UNB-UX-002" ]
          [ "priority-stack" ]
          []
          [ ("the home at 390 and 1280 px", "the keyboard focus order is recorded", "the sequence of focusable elements is identical") ]
          []
          [] ]

/// ui_quality.py: A11Y
let a11y : Spec list =
    [ req "UNB-UI-A-001" "accessibility" "WCAG 2.2 AA automated gate"
          "Every screen state MUST pass axe-core with tags wcag2a, wcag2aa, wcag21a, wcag21aa and wcag22aa at the five viewports of UNB-UI-R-001 with zero violations, and MUST have no console errors or failed resource loads."
          [ "UNB-UX-005"; "UNB-UX-006"; "UNB-UI-G-008" ]
          []
          []
          [ ("the v2 prototype set", "the v2 verifier (verify-v2, tools/fs/Unbound.Designs.Verify) runs", "every state passes at every viewport") ]
          []
          []
      req "UNB-UI-A-002" "accessibility" "Keyboard and focus"
          "Every function is operable by keyboard in a logical order; focus is always visible (2.4.7, 2.4.13 target: 3 px outline) and never obscured by fixed chrome (2.4.11); a skip link reaches main; dialogs trap and return focus; no keyboard traps."
          [ "UNB-UX-002"; "UNB-UX-005"; "ADV-907"; "ADV-944" ]
          [ "skip-link" ]
          []
          [ ("a Tab walk of each state at each viewport", "each stop is probed", "each stop has a visible indicator and is not covered at its centre") ]
          []
          []
      req "UNB-UI-A-003" "accessibility" "Screen-reader semantics"
          "One h1 per page, no skipped heading levels, named landmarks with unique labels, every panel named, every status in words, tables with captions and header cells, money read as one amount, live status messages for loading and submission. Manual screen-reader review (NVDA + Firefox, VoiceOver + Safari iOS, TalkBack + Chrome) is required before release; automated checks do not replace it."
          [ "UNB-UX-002"; "UNB-UX-006"; "UXA-1"; "UI-T21" ]
          [ "landmark-region"; "visually-hidden" ]
          []
          [ ("each v2 page", "headings and navigation landmarks are inspected", "one h1, no skipped levels, and visible nav landmarks have unique labels") ]
          []
          []
      req "UNB-UI-A-004" "accessibility" "Targets, reflow, spacing and motion"
          "Targets at least 24x24 px (2.5.8) everywhere and 44x44 px for primary controls on phones; content reflows at 320 px without horizontal scroll (1.4.10); text-spacing overrides cause no clipping or overflow (1.4.12); animation stops under prefers-reduced-motion; forced-colours mode keeps borders, focus and status glyphs."
          [ "UNB-UX-005"; "UI-T20"; "ADV-930"; "ADV-961" ]
          []
          []
          [ ("each v2 page with the WCAG 1.4.12 overrides injected at 320 px", "overflow and clipping are measured", "there is none") ]
          []
          []
      req "UNB-UI-A-005" "accessibility" "Accessible authentication and redundant entry"
          "Sign-in and step-up MUST offer a method that needs no cognitive test (passkey; WCAG 3.3.8), and multi-step grant and contribution flows MUST not ask again for information already given (3.3.7)."
          [ "UXA-2"; "UXA-3" ]
          []
          []
          [ ("a grant recommendation reviewed on a review step", "the donor goes back to edit the amount", "all other fields keep their values") ]
          []
          [] ]

/// ui_quality.py: VR
let vr : Spec list =
    [ req "UNB-UI-V-001" "visual-regression" "Reference images and comparison"
          "Each screen state has committed reference images at 390 and 1280 px (requirements/ui/designs/v2/rendered/<screen>/<state>-<width>.png) produced by the v2 verifier (verify-v2, tools/fs/Unbound.Designs.Verify) with deviceScaleFactor 1, reduced motion and fonts loaded. A visual change is accepted only by replacing the reference in a reviewed change; comparisons use a per-pixel threshold of 0.1 and fail above 0.5% changed pixels."
          [ "UNB-UX-005"; "INT-020"; "ADV-700"; "ADV-701" ]
          []
          []
          [ ("a CSS change that moves the balance panel 8 px", "the comparison runs against references", "it fails and reports the states and widths affected") ]
          []
          []
      req "UNB-UI-V-002" "visual-regression" "Scenario coverage matrix"
          "The reference set MUST cover these scenarios for the donor experience: normal, loading, no contributions, no grants, pending grants, attention required, stale balances, integration unavailable, restricted fund, suspended fund, succession pending, read-only permissions, failed payment, mobile viewport, white-labelled sponsor; plus reconciliation exception (staff) and embedded widget (partner host). data/ui-state-matrix.csv lists the state that covers each."
          [ "UNB-UX-001"; "UNB-UX-003"; "INT-015" ]
          []
          []
          [ ("data/ui-state-matrix.csv", "it is checked against the 17 scenarios", "every scenario has at least one state with reference images and a passing verification") ]
          []
          [] ]

/// ui_quality.py: REVISIONS
let revisions : (string * Revision) list =
    [ ("UNB-UI-S-001", ("both items appear first, each with one named action", "both items appear directly after the Available to grant panel, each with one named action, and at 390x844 the figure and the 'Needs your attention' heading are both in the first viewport (UNB-UI-N-006)", "Attention-first pushed the balance ~981 px down at 320 px (v1 audit UXD-008); measured v2 fold check"))
      ("UNB-UI-G-001", ("wrapped in a Forma status-lozenge", "wrapped in a status chip (UNB-UI-DS-008; Forma status-lozenge remains allowed in staff dense ledgers)", "v1 lozenges render uppercase monospace, read as developer output (report 18)"))
      ("UNB-UI-G-010", ("Every authenticated screen MUST show its scope (sponsor, program and fund or case) in a Forma scope-trail.", "Every authenticated screen MUST make its scope unambiguous: staff screens show sponsor, program and fund or case in a Forma scope-trail; donor, advisor and charity screens show the sponsor brand in the app bar and the fund as the page title and switcher (UNB-UI-N-004).", "A three-level trail with an internal fund id was noise for a donor with one fund (report 18)"))
      ("UNB-UI-G-016", ("Every fund, grant, contribution, case and document MUST show its numeric reference using the Forma identifier component, with a copy control.", "Every fund, grant, contribution, case and document detail view and every document MUST show its numeric reference using the Forma identifier component, with a copy control; donor summary lists (home, activity) omit references (UNB-UX-007).", "References in donor summaries added noise without a task (report 18)")) ]

/// ui_quality.py: DECISIONS
let decisions : (string * string) list list =
    [ [ ("decision_id", "RCD-050")
        ("type", "design")
        ("question", "Which palette and typefaces form Unbound's default (unbranded sponsor) theme?")
        ("affected_ids", "UNB-UI-DS-001;UNB-UI-DS-012;INT-019")
        ("decision_owner", "Product owner (design)")
        ("recommendation", "Use the Echelon theme 1.1.0 brand values (verified, owner-approved 'foundry ledger' direction) compiled as a separate 'unbound-default' Forma brand manifest; not the unverified README candidate palette.")
        ("alternatives", "Use the README candidate palette (unverified); use Forma's neutral default (no Charcoal and Bronze); use data-forma-brand='echelon' directly (brands donors' UI as Echelon)")
        ("rationale", "Only verified values; keeps Unbound's identity distinct from Echelon's (UNB-EXP-002) while following the approved direction")
        ("blocks_implementation", "N")
        ("reversible", "Y")
        ("status", "decided-provisional")
        ("decided_option", "unbound-default manifest = echelon.brand.json colours, fonts and radius; identity and terms changed")
        ("decision_basis", "Echelon brand AUDIT.md verification; owner addendum asks for Balanced Charcoal and Bronze")
        ("validation_required", "Owner design approval; contrast verification by Forma BrandCompiler (passed)")
        ("revisit_trigger", "Echelon releases frozen v1 tokens")
        ("decided_by", "Integrating agent under owner delegation ('make best decisions and note')")
        ("decided_on", "2026-11-23") ]
      [ ("decision_id", "RCD-051")
        ("type", "design")
        ("question", "Where do application-layer components (app frame, panel, status chip, task list, money) live so sponsor brands get them?")
        ("affected_ids", "UNB-UI-DS-012;UNB-UI-N-001;FORMA-GAP-06;FORMA-GAP-20;FORMA-GAP-21;FORMA-GAP-22")
        ("decision_owner", "Design system owner (Forma)")
        ("recommendation", "Brand-neutral components in Forma; brand values only in manifests. Until Forma ships them, Unbound keeps a brand-agnostic application layer derived from --forma-* tokens.")
        ("alternatives", "Reuse the Echelon app layer (does not apply to sponsor brands); per-sponsor CSS (duplicated, unverifiable)")
        ("rationale", "White labelling requires that structure and behaviour be brand-independent")
        ("blocks_implementation", "N")
        ("reversible", "Y")
        ("status", "decided-provisional")
        ("decided_option", "Forma owns; Unbound interim layer")
        ("decision_basis", "echelon-app.css is scoped to data-forma-brand='echelon' (verified)")
        ("validation_required", "Forma maintainers accept the gap issues")
        ("revisit_trigger", "Forma releases an application shell")
        ("decided_by", "Integrating agent under owner delegation")
        ("decided_on", "2026-11-23") ]
      [ ("decision_id", "RCD-052")
        ("type", "design")
        ("question", "Date format for donor surfaces")
        ("affected_ids", "UNB-UI-F-005;UNB-UI-G-018")
        ("decision_owner", "Product owner")
        ("recommendation", "Sponsor-locale medium date (en-US 'Nov 30, 2026') with time zone abbreviation for times; v1 used '30 Nov 2026'.")
        ("alternatives", "Day-month-year everywhere; ISO dates")
        ("rationale", "U.S. sponsors and donors; unambiguous month names")
        ("blocks_implementation", "N")
        ("reversible", "Y")
        ("status", "decided-provisional")
        ("decided_option", "Sponsor locale medium format")
        ("decision_basis", "Sponsor locale is already a tenant setting")
        ("validation_required", "none")
        ("revisit_trigger", "Non-U.S. sponsor")
        ("decided_by", "Integrating agent under owner delegation")
        ("decided_on", "2026-11-23") ]
      [ ("decision_id", "RCD-053")
        ("type", "design")
        ("question", "Phone navigation and action placement")
        ("affected_ids", "UNB-UI-N-003;UNB-UI-N-005")
        ("decision_owner", "Product owner (design)")
        ("recommendation", "Bottom tab bar (four destinations + Menu) and page actions inline under the title; no sticky action bar.")
        ("alternatives", "Forma navigation-shell compact disclosure only (destinations hidden behind 'Navigation'); sticky action bar above the tab bar (13% of an 844 px screen)")
        ("rationale", "Destinations visible with one thumb; fixed chrome under 10% of the viewport; no focus obscured")
        ("blocks_implementation", "N")
        ("reversible", "Y")
        ("status", "decided-provisional")
        ("decided_option", "Tab bar + inline actions")
        ("decision_basis", "Prototype measurement (report 18)")
        ("validation_required", "Moderated usability test with donors on phones (INT-020)")
        ("revisit_trigger", "Usability evidence")
        ("decided_by", "Integrating agent under owner delegation")
        ("decided_on", "2026-11-23") ] ]

/// ui_quality.py: EXTRA_REVISIONS
let extraRevisions : (string * Revision) list =
    [ ("UNB-UI-S-001", ("Regions: scope-trail; fund summary (Available to grant, Market value with freshness, Pending);", "Regions: application frame with fund title and switcher (UNB-UI-N-004); fund summary (Available to grant as the one headline figure with a composition breakdown of committed, settling and total market value with freshness: UNB-UI-F-002, UNB-UI-F-007);", "v2 layout (report 18)")) ]

/// ui_quality.py: MORE_REVISIONS
let moreRevisions : (string * string * string * string) list =
    [ ("UNB-UI-S-003", "A four-step wizard: 1 Charity (S-002); 2 Amount", "One page with progressive disclosure, then one review step: Charity (search within the page; S-002 for the full directory); Amount", "v2: most grants are one fixed amount, one time; v1 needed four Continue presses plus a dialog (UXD-023, UXD-024, UXD-043)")
      ("UNB-UI-S-003", "4 Review and submit (G-005 dialog)", "Review and submit (the review step itself meets G-005; no second dialog)", "UXD-024")
      ("UNB-UI-S-003", "show 'Available to grant' throughout, and after submission show the five-dimension status (G-002)", "show 'Available to grant' beside the amount, and after submission show the donor milestone track with expected dates (G-002)", "UXD-019")
      ("UNB-UI-G-005", "MUST present, before submission, a confirmation dialog that states", "MUST present, before submission, a confirmation that states", "A review page answering all four questions is the confirmation for flows; a modal dialog remains for actions taken from a record (withdraw, cancel) (UXD-024)")
      ("UNB-UI-G-002", "Grant status MUST be shown as five ordered dimensions", "In staff views, grant status MUST be shown as five ordered dimensions; donor, advisor and charity views show a four-milestone track (Submitted, Approved, Paid, Acknowledged) with dates, derived from the same dimensions,", "Five rows of mostly 'Not started' took 870-1,030 px (UXD-018, UXD-019)")
      ("UNB-UI-G-018", "with UTC in details.", "with UTC in documents and staff details (not in donor UI).", "UTC times confused donors (UXD-014)")
      ("UNB-UI-G-006", "with a 'Check status' action that queries by idempotency key.", "with a 'Check status' action that queries by idempotency key; donor wording is 'We can't confirm your recommendation was received' and the key is never shown to donors (support sees it).", "UXD-014 (IDK-... reference shown to donors in v1)")
      ("UNB-UI-G-005", "a dialog lists amount, charity, fund", "the confirmation (the review step, or a dialog for record actions) lists amount, charity, fund", "Consistent with the revised statement")
      ("UNB-UI-S-003", "the step does not advance", "the recommendation cannot be reviewed or submitted until it is corrected, and entered values are kept", "No steps in the v2 flow")
      ("UNB-UI-S-011", "Release 1 uses tables and metric cards only (charts deferred, FORMA-GAP-05).", "Release 1 MAY use one trend chart per view only if it meets UNB-UI-DS-018 (title, labelled axes, value labels, text summary, keyboard-reachable data table); otherwise tables and metric cards (FORMA-GAP-05). No total is shown as reconciled while any account is stale.", "v2 sponsor overview: accessible SVG chart with a data table (owner addendum requires charts and reporting)")
      ("UNB-UI-G-017", "never rounded below cents.", "never rounded below cents, except chart value labels, which MAY be abbreviated ($512.3k) when the exact amounts are in the chart's data table.", "v2 sponsor overview chart")
      ("UNB-UI-W-001", "and offers no consequential actions in release 1 (DAF checkout deferred, CAP-03).", "and offers no consequential actions in release 1 (DAF checkout deferred, CAP-03). The card carries the sponsor brand and the legal-sponsor sentence inside the frame, stays readable at 320-600 px frame widths, reports its content height to the host by a postMessage limited to the registered host origin (UNB-GAP-W-01), and on token expiry clears all grant data and shows a safe message.", "v2 host page at 360 and 600 px; UXD-061")
      ("UNB-UI-S-008", "Filters: rule family, sponsor, owner.", "Filters: rule family and owner, plus sponsor only for operators who serve more than one sponsor. An item the viewer proposed shows 'Waiting for approval' and no approve action (segregation of duties).", "v2 work queue")
      ("UNB-UI-S-004", "five-dimension progress;", "progress per G-002 (donor milestone track with dates; five dimensions in staff views);", "Consistent with revised G-002")
      ("UNB-UI-S-004", "actions allowed in the current state only (Withdraw before approval;", "for a returned payment, the donor's choice to wait for corrected bank details or redirect, each with its consequence and the default applied on a stated date; actions allowed in the current state only (Withdraw before approval;", "v2 payment-returned state (failed-payment scenario)") ]

/// ui_quality.py: NEW
let newSpecs : Spec list = ds @ nav @ fin @ resp @ a11y @ vr

/// The constants of ui_trace.py: v1 defect resolutions, the 17 required scenarios, tag aliases and the DKS-UI scenario
/// rows built in dks(). Transcribed in the same order and wording; ev, page and S are the script's lambdas.
module Unbound.Authoring.UiTraceData

let REL = "requirements/ui/designs/v2/rendered"

/// ev(screen, state): the two reference images of a v2 state.
let ev (screen: string) (state: string) = $"{REL}/{screen}/{state}-390.png; {REL}/{screen}/{state}-1280.png"

let OPEN = "open"
let RES = "resolved-in-v2"
let GATE = "prevented-by-gate"

/// page(s, st) in dks().
let page (s: string) (st: string) = $"requirements/ui/designs/v2/screens/{s}/{st}.html"

/// VIEW in dks().
let VIEW = "viewports 390x844, 320x800, 640x450, 768x1024, 1280x900"

/// The DKS columns, in S()'s order.
let dksFields =
    [ "scenario_id"; "title"; "requirement_ids"; "preconditions"; "stimulus"; "expected_states"; "financial_invariants"
      "audit_records"; "recovery_behavior"; "mock_data_notes" ]

/// S(*a) in dks(): dict(zip(fields, a)).
let S a b c d e f g h i j : (string * string) list = List.zip dksFields [ a; b; c; d; e; f; g; h; i; j ]

/// ui_trace.py: RESOLUTION
let resolution : (string * (string * string * string)) list =
    [ ("UXD-001", (RES, "UNB-UI-F-001;UNB-UI-G-017", "verify.mjs money-wrap check passes on every v2 page at 5 viewports and under text spacing; " + (ev "S-001" "normal")))
      ("UXD-002", (GATE, "UNB-UI-A-001", "verify.mjs now fails on axe 'incomplete' color-contrast nodes below their required ratio; v2 has no single-page pagination"))
      ("UXD-003", (RES, "UNB-UI-N-003;UNB-UI-A-002", "No sticky action bar; scroll-padding clears the tab bar; focus-obscured check passes at 390/320/640; " + (ev "S-001" "normal")))
      ("UXD-004", (RES, "UNB-UI-N-004;UNB-UI-N-005", "S-004 v2 record header names the grant shown in each state; " + (ev "S-004" "payment-returned")))
      ("UXD-005", (RES, "UNB-UI-F-001;UNB-UI-G-016", "EINs and references use .unb-num (nowrap), checked by the money/number wrap check"))
      ("UXD-006", (RES, "UNB-UI-DS-002;UNB-UI-R-002", "Application type scale and container-driven tables; text-spacing check passes"))
      ("UXD-007", (RES, "UNB-UI-F-004", "Ledger tables right-align tabular amounts; " + (ev "S-008" "queue")))
      ("UXD-008", (RES, "UNB-UI-N-006", "Fold check at 390x844: figure at 264 px, attention heading at 711 px, tab bar at 787 px; " + (ev "S-001" "normal")))
      ("UXD-009", (RES, "UNB-UI-F-002;UNB-UI-F-007", "One headline figure; market value is the ruled total row of the breakdown; " + (ev "S-001" "normal")))
      ("UXD-010", (RES, "UNB-UI-DS-006", "One filled primary per region; " + (ev "S-003" "review")))
      ("UXD-011", (RES, "UNB-UI-DS-002;UNB-UI-DS-015", "Application type scale applies to dialogs; " + (ev "S-004" "withdraw-confirm")))
      ("UXD-012", (RES, "UNB-UI-DS-008", "Sentence-case status chip; " + (ev "S-001" "normal")))
      ("UXD-013", (RES, "UNB-UI-DS-014", "Skeleton matches final panels; " + (ev "S-001" "loading") + "; " + (ev "S-008" "loading")))
      ("UXD-014", (RES, "UNB-UI-N-004;UNB-UI-G-007", "No internal ids or process names on donor and charity v2 pages; staff references kept and labelled"))
      ("UXD-015", (RES, "UNB-UI-N-004;UNB-UI-N-005", "One page header per page; no second trail"))
      ("UXD-016", (RES, "UNB-UI-N-001;UNB-UI-N-004", "App bar shows signed-in user and role; fund title is the switcher; " + (ev "S-001" "read-only")))
      ("UXD-017", (RES, "UNB-UI-N-004;UNB-UI-G-010", "Donor screens drop the three-level trail (G-010 revised)"))
      ("UXD-018", (RES, "UNB-UI-S-004;UNB-UI-G-002", "S-004 v2 merges tracker and history; " + (ev "S-004" "in-review")))
      ("UXD-019", (RES, "UNB-UI-S-004;UNB-UI-S-003", "Compact progress track; " + (ev "S-004" "in-review") + "; " + (ev "S-003" "submitted")))
      ("UXD-020", (RES, "UNB-UI-S-004;UNB-UI-G-006", "Explanation directly under the record header; " + (ev "S-004" "payment-returned")))
      ("UXD-021", (RES, "UNB-UI-G-016;UNB-UI-N-005", "Reference shown once with copy; charity and amount in the header; " + (ev "S-004" "approved")))
      ("UXD-022", (RES, "UNB-UI-DS-017", "Single-column form; " + (ev "S-003" "details")))
      ("UXD-023", (RES, "UNB-UI-DS-017", "Progressive disclosure for percentage and recurring options; " + (ev "S-003" "more-options")))
      ("UXD-024", (RES, "UNB-UI-DS-015;UNB-UI-G-005", "G-005 met on the review step; no second dialog; " + (ev "S-003" "review")))
      ("UXD-025", (RES, "UNB-UI-DS-005", "Unavailable actions omitted or explained; " + (ev "S-003" "restricted") + "; " + (ev "S-004" "approved")))
      ("UXD-026", (OPEN, "UNB-UI-DS-017", "S-002 charity search is not yet redesigned in v2 (roadmap wave 2); S-003 v2 embeds a simpler search"))
      ("UXD-027", (OPEN, "UNB-UI-S-002", "S-002 redesign (roadmap wave 2)"))
      ("UXD-028", (RES, "UNB-UI-DS-010", "One notice pattern with state glyph and rule; " + (ev "S-001" "suspended")))
      ("UXD-029", (OPEN, "UNB-UI-F-004", "S-006 participants redesign (roadmap wave 2)"))
      ("UXD-030", (OPEN, "UNB-UI-S-007", "S-007 document centre redesign (roadmap wave 2)"))
      ("UXD-031", (RES, "UNB-UI-S-008", "Counts and list share one scope statement; " + (ev "S-008" "queue")))
      ("UXD-032", (RES, "UNB-UI-S-008", "Selection model and SLA wording; " + (ev "S-008" "queue")))
      ("UXD-033", (OPEN, "UNB-UI-S-009", "S-009 case workspace redesign (roadmap wave 2; S-008 v2 shows the reconciliation decision)"))
      ("UXD-034", (OPEN, "UNB-UI-S-010", "S-010 policy configuration redesign (roadmap wave 3)"))
      ("UXD-035", (RES, "UNB-UI-S-012;UNB-UI-DS-017", "Account number re-entry on re-verification; " + (ev "S-012" "bank-reverify")))
      ("UXD-036", (OPEN, "UNB-UI-S-013", "S-013 notification preferences redesign (roadmap wave 3)"))
      ("UXD-037", (OPEN, "UNB-UI-G-016", "Copy feedback needs application behaviour (FORMA-GAP-07); specify announcement text in S-004/S-008"))
      ("UXD-038", (RES, "UNB-UI-A-003", "Submission result in a status region; " + (ev "S-003" "submitted")))
      ("UXD-039", (RES, "UNB-UI-V-002", "All 17 required scenarios covered (data/ui-state-matrix.csv)"))
      ("UXD-040", (RES, "UNB-UI-DS-001", "One brand manifest per sponsor; one type system"))
      ("UXD-041", (RES, "UNB-UI-DS-013", "Getting-started checklist; no placeholder glyph; " + (ev "S-001" "new-fund")))
      ("UXD-042", (RES, "UNB-UI-DS-017", "Amount field with currency prefix and parsed echo; " + (ev "S-003" "details")))
      ("UXD-043", (RES, "UNB-UI-S-003", "One progress indicator; " + (ev "S-003" "details")))
      ("UXD-044", (OPEN, "UNB-UI-DS-017", "S-006 and S-010 not redesigned (roadmap); rule fixed in DS-017"))
      ("UXD-045", (OPEN, "UNB-UI-S-002", "Pagination hidden for one page: apply in S-002/S-007 redesign"))
      ("UXD-046", (OPEN, "UNB-UI-S-002", "S-002 redesign (roadmap wave 2)"))
      ("UXD-047", (RES, "UNB-UI-DS-008;UNB-UI-F-006", "Freshness stated once in words; " + (ev "S-001" "normal")))
      ("UXD-048", (RES, "UNB-UI-F-004;UNB-UI-N-005", "Actions in the page header in the first viewport; activity as a ledger; " + (ev "S-001" "normal")))
      ("UXD-049", (OPEN, "UNB-UI-S-007", "S-007 redesign (roadmap wave 2)"))
      ("UXD-050", (RES, "UNB-UI-A-003", "Heading outline check passes on S-008 v2"))
      ("UXD-051", (RES, "UNB-UI-A-003;UNB-UI-V-001", "One page per state; unique nav labels check passes"))
      ("UXD-052", (OPEN, "UNB-UI-S-009", "S-009 redesign (roadmap wave 2)"))
      ("UXD-053", (OPEN, "UNB-UI-G-011", "S-009 assistance flyout redesign (roadmap wave 2)"))
      ("UXD-054", (OPEN, "UNB-UI-S-010", "S-010 redesign (roadmap wave 3)"))
      ("UXD-055", (RES, "UNB-UI-S-012", "Consistent action labels; " + (ev "S-012" "grant-detail")))
      ("UXD-056", (OPEN, "UNB-UI-S-005", "S-005 add money redesign (roadmap wave 2)"))
      ("UXD-057", (RES, "UNB-UI-A-004;UNB-UI-DS-006", "Buttons 44 px; tab bar 56 px"))
      ("UXD-058", (RES, "UNB-UI-DS-017", "Full-width single-column fields on S-003 v2"))
      ("UXD-059", (RES, "UNB-UI-F-005", "Sponsor-locale dates in time elements"))
      ("UXD-060", (RES, "UNB-UI-G-007", "Successor notice opens with condolence and plain explanation; " + (ev "S-001" "succession")))
      ("UXD-061", (RES, "UNB-UI-W-001;UNB-UI-R-001", "Widget shown in 360 and 600 px frames on a partner host; " + (ev "W-001" "host-page")))
      ("UXD-062", (RES, "UNB-UI-N-001", "Application frame on every v2 page"))
      ("UXD-063", (RES, "UNB-UI-V-001", "One page per state, no harness chrome inside the product UI")) ]

/// ui_trace.py: SCENARIOS
let scenarios : string list =
    [ "normal"
      "loading"
      "no-contributions"
      "no-grants"
      "pending-grants"
      "attention-required"
      "stale-balances"
      "integration-unavailable"
      "restricted-fund"
      "suspended-fund"
      "succession-pending"
      "read-only"
      "failed-payment"
      "reconciliation-exception"
      "mobile"
      "embedded-widget"
      "white-label" ]

/// ui_trace.py: ALIASES
let aliases : (string * string) list =
    [ ("embedded", "embedded-widget")
      ("restricted", "restricted-fund")
      ("phone", "mobile") ]

/// ui_trace.py: rows in dks()
let dksRows : (string * string) list list =
    [ S "DKS-UI-001" "Giving home normal: priority and fold" "UNB-UI-S-001;UNB-UI-N-006;UNB-UI-F-002" ("Fund with $100,000.00 available, 3 grants in progress, 2 attention tasks; " + (page "S-001-giving-home" "normal")) "Open home at 390x844 and 1280x900" "Headline figure and 'Needs your attention' heading visible above the tab bar at 390; figure, both tasks and page actions in first 1280 viewport; reference images match within 0.5% pixels" "Available + committed + settling = total fund value shown" "none (read)" "n/a" "Synthetic Lindqvist Family Fund"
      S "DKS-UI-002" "Loading skeleton geometry" "UNB-UI-DS-014;UNB-UI-G-013" (page "S-001-giving-home" "loading") "Render loading then normal at 1280" "Panel boxes differ by <= 24 px vertically; aria-busy region with polite status; no animation under reduced motion" "No figure shown before data" "none" "If data fails, error state replaces skeleton (G-013)" "Delay projection 2 s"
      S "DKS-UI-003" "New fund onboarding (no contributions)" "UNB-UI-DS-013;UNB-UI-S-001" (page "S-001-giving-home" "new-fund") "Open home" "Checklist 1 of 3 done; one primary action 'Add money'; no empty $0.00 hero repeated" "Available to grant $0.00" "none" "n/a" "Fund opened today"
      S "DKS-UI-004" "Money available, no grants" "UNB-UI-DS-013;UNB-UI-S-001" (page "S-001-giving-home" "no-grants") "Open home" "Grants panel explains and offers 'Recommend a grant'" "Committed $0.00" "none" "n/a" "First contribution settled"
      S "DKS-UI-005" "Stale Available to grant" "UNB-UI-F-006;UNB-UI-G-003;UNB-UI-DS-010" (page "S-001-giving-home" "stale-balance") "Projection older than limit" "'Not current' chip and last confirmed time in the balance panel; one notice explaining what is still safe" "Figure is the last confirmed value, never extrapolated" "none" "Recommendations validated against confirmed balance at approval" "Projection lag 4 days"
      S "DKS-UI-006" "Pricing integration unavailable" "UNB-UI-DS-010;UNB-UI-F-007" (page "S-001-giving-home" "integration-unavailable") "Pricing service returns 503" "Total row reads 'Unavailable'; bar omitted; Available to grant unaffected; one notice" "Available to grant independent of market value" "none" "Values refresh automatically when service returns" "Pricing stub down"
      S "DKS-UI-007" "Restricted fund: recommending paused" "UNB-UI-DS-005;UNB-UI-DS-010" (page "S-001-giving-home" "restricted" + "; " + (page "S-003-grant-recommendation" "restricted")) "Sponsor review hold on fund" "No 'Recommend a grant' page action; notice says why, how long, what still works; deep link to recommend shows the same explanation" "No reservation possible" "none" "Contact donor services" "Account-change review"
      S "DKS-UI-008" "Suspended fund" "UNB-UI-DS-010;UNB-UI-S-001" (page "S-001-giving-home" "suspended") "Fund suspended for overdue certification" "Notice with date, reason and lifting action; no grant or contribution actions" "Balances read-only" "none" "Completing certification lifts suspension" "Certification 90+ days overdue"
      S "DKS-UI-009" "Succession pending: successor first view" "UNB-UI-S-001;UNB-UI-G-007" (page "S-001-giving-home" "succession") "Successor signs in before activation" "Condolence, plain explanation, 'Review and accept' primary; steps of what happens next" "Fund value shown as at pause" "none" "Decline or ask a question available" "Verified death notice"
      S "DKS-UI-010" "Read-only advisor" "UNB-UI-N-004;UNB-UI-DS-005" (page "S-001-giving-home" "read-only" + "; " + (page "S-004-grant-detail" "advisor-view")) "View-only participant opens home and a grant" "'View only' shown; no add-money, recommend, withdraw or redirect controls; access expiry stated" "none" "none" "n/a" "Financial advisor with view access"
      S "DKS-UI-011" "Failed payment: returned grant" "UNB-UI-S-004;UNB-UI-S-001;UNB-UI-G-006" (page "S-001-giving-home" "failed-payment" + "; " + (page "S-004-grant-detail" "payment-returned")) "Charity bank returns ACH" "Home task 'A payment was returned' with 'Review options'; grant detail explains, money back in Available to grant, wait or redirect with consequences" "Available to grant increases by returned amount exactly once" "Return recorded in grant timeline" "Redirect creates a new recommendation; wait keeps grant open" "R01 return"
      S "DKS-UI-012" "Reconciliation exception with segregation of duties" "UNB-UI-S-008" (page "S-008-work-queue" "reconciliation-break" + "; " + (page "S-008-work-queue" "proposed-by-me")) "Bank-vs-ledger break; preparer proposes resolution then views it" "Expected vs actual and difference shown; resolutions with authority; preparer cannot approve own proposal and is told why" "Difference computed = actual - expected" "Proposal and approval by distinct users" "Approver can reject with reason" "Synthetic bank statement line"
      S "DKS-UI-013" "Embedded widget in partner host" "UNB-UI-W-001;UNB-UI-R-001" (page "W-001-grant-status-widget" "host-page" + "; " + (page "W-001-grant-status-widget" "expired")) "Host embeds widget at 360 and 600 px; token expires" "Widget keeps sponsor brand and disclosure inside host; readable at both widths; expired token clears data and shows safe message; no consequential actions" "Amount equals grant record" "none" "Host re-requests token server-to-server" "Fictional partner host"
      S "DKS-UI-014" "White-label sponsor swap" "UNB-UI-DS-012;UNB-UI-DS-001" (page "S-001-giving-home" "white-label" + "; " + (page "S-003-grant-recommendation" "white-label")) "Render with example-harbor manifest" "Only colours, fonts, radius, mark and name differ from unbound-default; legal disclosure verbatim; all checks pass" "none" "none" "n/a" "example-harbor brand"
      S "DKS-UI-015" "Keyboard walk and focus visibility" "UNB-UI-A-002;UNB-UI-N-003" ("Every v2 page; " + VIEW) "Tab through the page" "Every stop has a >= 2 px indicator and is not covered at its centre; skip link reaches main; focus order identical at 390 and 1280" "none" "none" "n/a" "verify.mjs walkKeyboard"
      S "DKS-UI-016" "Reflow, zoom and text spacing" "UNB-UI-A-004;UNB-UI-F-001;UNB-UI-R-001" ("Every v2 page; " + VIEW) "Render at 320 px and 200% zoom; inject WCAG 1.4.12 spacing" "No horizontal page overflow; no clipped text; no money or reference wraps" "Money tokens intact" "none" "n/a" "verify.mjs"
      S "DKS-UI-017" "Automated WCAG 2.2 AA" "UNB-UI-A-001;UNB-UI-A-003" ("Every v2 page; " + VIEW) "Run axe-core wcag2a..wcag22aa, including incomplete contrast" "Zero violations; one h1; no skipped heading levels; unique visible nav labels; no console errors" "none" "none" "n/a" "axe-core 4.10.2"
      S "DKS-UI-018" "Manual screen-reader review" "UNB-UI-A-003" "S-001 normal, S-003 review and submitted, S-004 payment-returned, S-008 reconciliation-break" "NVDA+Firefox, VoiceOver+Safari iOS, TalkBack+Chrome" "Each task completable; status, money and dates announced correctly; submission result announced" "none" "none" "n/a" "Not executed: requires human testers (UNB-UX-006)"
      S "DKS-UI-019" "Forced colours and greyscale" "UNB-UI-DS-004;UNB-UI-DS-008;UNB-UI-F-007" "S-001 normal and failed-payment" "Windows high contrast; greyscale filter" "Statuses identifiable by glyph and words; allocation segments distinguishable; focus visible" "none" "none" "n/a" "Not yet automated: add forced-colors emulation to verify.mjs"
      S "DKS-UI-020" "Grant recommendation with validation and single confirmation" "UNB-UI-S-003;UNB-UI-DS-017;UNB-UI-DS-015" (page "S-003-grant-recommendation" "validation-errors" + "; " + (page "S-003-grant-recommendation" "review") + "; " + (page "S-003-grant-recommendation" "submitted")) "Amount above available, then corrected and submitted" "Error summary links to field, values kept; review step states change, reversibility, next actor and timing; no second dialog; result announced" "Reservation equals amount; no duplicate on resubmit" "Recommendation submitted event" "Unknown outcome shows check-status, not resubmit" "Synthetic" ]

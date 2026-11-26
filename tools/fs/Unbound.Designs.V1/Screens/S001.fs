/// S-001 Giving home (port of tools/screens/S-001.mjs).
module Unbound.Designs.V1.Screens.S001

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope (who: string) =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; $"Lindqvist Family Fund ({who})" ]

let private actionsBar (_p: string) =
    mobileBar
        "Giving home actions"
        """<a class="forma-button forma-mobile-action-bar__secondary" href="S-005-add-money.html">Add money</a><a class="forma-button forma-mobile-action-bar__primary" href="S-003-grant-recommendation.html">Recommend a grant</a>"""

let private calcDisclosure =
    let rows =
        kv [ "Market value", money "$120,000.00"
             "Less: reserved for 3 grants awaiting payment (deducted)", money "−$15,000.00"
             "Less: ACH contribution still inside its return window (deducted)", money "−$5,000.00"
             "Available to grant", critical "$100,000.00" ]
    disclosure
        "How this is calculated"
        $$"""{{rows}}<p>Calculated by the Example Charitable Foundation ledger. Market value is informational; only Available to grant is used when you recommend a grant.</p>"""

let private summary (p: string) (stale: bool) =
    let avail =
        Ui.metric(
            id = $"{p}-avail",
            label = "Available to grant",
            value = "$100,000.00",
            context = loz "ok" "Ledger confirmed",
            extra = Ui.fresh(label = "Calculated", iso = "2026-12-01T16:58:00-06:00", shown = "1 Dec 2026, 4:58 PM CT", stateText = "Current") + calcDisclosure
        )
    let market =
        Ui.metric(
            id = $"{p}-market",
            label = "Market value (informational)",
            value = "$120,000.00",
            context = (if stale then loz "unknown" "Not current" else loz "" "Informational"),
            extra =
                (if stale then
                     Ui.fresh(label = "Priced", iso = "2026-11-27T16:00:00-06:00", shown = "27 Nov 2026, 4:00 PM CT", stateText = "Not current", state = "unknown")
                     + "<p>Older than the 1-business-day pricing limit. It is not used for grant decisions; Recommend a grant uses Available to grant instead.</p>"
                 else
                     Ui.fresh(label = "Priced", iso = "2026-11-30T16:00:00-06:00", shown = "30 Nov 2026, 4:00 PM CT", stateText = "Current"))
        )
    let pending =
        Ui.metric(
            id = $"{p}-pending",
            label = "Pending",
            value = "$20,000.00",
            context = loz "attention" "In progress",
            extra =
                kv [ "Reserved for 3 grants awaiting payment", money "$15,000.00"
                     "ACH contribution received 24 Nov 2026", money "$5,000.00" + "<br>Available to grant on <time datetime=\"2026-12-08\">8 Dec 2026</time>" ]
        )
    tag
        "dashboard-grid"
        $$"""<section class="forma-dashboard-grid" aria-label="Fund summary">
{{avail}}
{{market}}
{{pending}}
</section>"""

let private attention (p: string) =
    Ui.workQueue(
        id = $"{p}-attention",
        kicker = "Created by sponsor rules",
        title = "Needs your attention",
        count = "2 items",
        countState = "attention",
        items =
            [ Ui.work(
                  title = "Grant 2026-004811 to Northgate Literacy Project is on hold",
                  state = "blocked",
                  label = "On hold: charity eligibility",
                  text = "The charity's IRS status could not be confirmed on 27 Nov 2026. Your sponsor is checking; you can redirect the grant if you prefer.",
                  action = "View held grant",
                  actionLabel = "View held grant 2026-004811"
              )
              Ui.work(
                  title = "Annual donor certification is due 15 Dec 2026",
                  state = "attention",
                  label = "Waiting on you",
                  text = "Confirm that no grant from this fund gave you or your family a personal benefit in 2026.",
                  action = "Complete certification"
              ) ]
    )

let private activity (p: string) =
    let f1 =
        Ui.feedItem(id = $"{p}-f1", source = "Grant 2026-004790", title = "Grant to Riverside Food Bank was paid", state = "ok", label = "Paid", iso = "2026-11-30T10:12:00-06:00", shown = "Yesterday, 10:12 AM", summary = "$2,500.00 sent by ACH to the charity's verified account.")
    let f2 =
        Ui.feedItem(id = $"{p}-f2", source = "Contribution C-2026-000377", title = "ACH contribution received", state = "attention", label = "Inside return window", iso = "2026-11-24T09:03:00-06:00", shown = "24 Nov, 9:03 AM", summary = "$5,000.00 from Lakeshore Credit Union ••••4821. Available to grant on 8 Dec 2026.")
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-activity">
  <h3 id="{{p}}-activity">Recent activity</h3>
  {{f1}}
  {{f2}}
</section>"""

let private st1 =
    state
        "Normal: two attention items first"
        "Donor"
        "UNB-UI-S-001 AC-2, UNB-UI-G-004 AC-1, UNB-UI-G-001 AC-2, UNB-UI-G-015 AC-1, UNB-UI-G-010"
        (fun p ->
            $$"""{{scope "F-2026-000418"}}
{{attention p}}
{{summary p false}}
{{activity p}}
{{actionsBar p}}""")

let private st2 =
    state "Loading" "Donor" "UNB-UI-G-013 loading state (skeleton with busy semantics)" (fun _ ->
        $$"""{{scope "F-2026-000418"}}
{{skeleton "Loading fund summary"}}
{{skeleton "Loading items that need your attention"}}""")

let private st3 =
    state
        "Empty: no contributions or grants yet"
        "Donor, new fund"
        "UNB-UI-G-013 AC-1 (empty-state 'No grants yet' with 'Recommend a grant')"
        (fun p ->
            let card =
                Ui.metric(label = "Available to grant", value = "$0.00", context = loz "ok" "Ledger confirmed", extra = Ui.fresh(label = "Calculated", iso = "2026-12-01T16:58:00-06:00", shown = "1 Dec 2026, 4:58 PM CT", stateText = "Current"))
            let e1 =
                Ui.emptyState(id = $"{p}-e1", title = "No contributions yet", text = "Add money by bank transfer, wire, check or securities. It becomes available to grant once received and settled.", action = """<a class="forma-button" href="S-005-add-money.html">Add money</a>""")
            let e2 =
                Ui.emptyState(id = $"{p}-e2", title = "No grants yet", text = "You can draft a recommendation now; it is checked against Available to grant before submission.", action = """<a class="forma-button" href="S-003-grant-recommendation.html">Recommend a grant</a>""")
            $$"""{{scope "F-2026-000512"}}
{{tag "dashboard-grid" ("<section class=\"forma-dashboard-grid\" aria-label=\"Fund summary\">" + card + "</section>")}}
{{e1}}
{{e2}}
{{actionsBar p}}""")

let private st4 =
    state
        "Capacity evidence not current"
        "Donor"
        "UNB-UI-G-003 AC-1 (market value 'Not current'; authoritative Available to grant used instead), UNB-UI-G-004"
        (fun p ->
            $$"""{{scope "F-2026-000418"}}
{{summary p true}}
{{actionsBar p}}""")

let private st5 =
    state "Fund suspended pending succession" "Successor before activation" "UNB-UI-S-001 AC-1 (blocked, no grant action)" (fun p ->
        let op =
            Ui.opStatus(
                id = $"{p}-susp",
                state = "blocked",
                label = "Suspended pending succession",
                symbol = "×",
                title = "This fund is suspended pending succession",
                body = "<p>Example Charitable Foundation recorded a verified death on 18 Nov 2026. No grants or contributions can be made until the successor plan is activated. Your successor invitation is being prepared; we will email you when you can accept it.</p>",
                actions = [ """<a class="forma-button" href="S-006-participants-successors.html">View succession status</a>"""; """<button type="button">Contact your sponsor</button>""" ]
            )
        $$"""{{scope "F-2026-000418"}}
{{op}}
{{kv [ "Fund value when suspended", money "$120,000.00" + " (informational)"; "Next step", "Sponsor activates the successor plan; no action is needed from you yet" ]}}""")

let screen: Screen =
    { Id = "S-001"
      File = "S-001-giving-home.html"
      Title = "Giving home"
      ReqIds = [ "UNB-UI-S-001" ]
      Audience = "donor"
      Brand = true
      Lead = "How much is available to grant, what is pending and why, and what to do next, on one phone screen."
      States = [ st1; st2; st3; st4; st5 ]
      Coverage =
        [ cov "UNB-UI-S-001" "AC-1" Yes "State 5: operation-status with blocked lozenge 'Suspended pending succession' and heading 'This fund is suspended pending succession'; no Recommend a grant action is rendered. The sentence is the heading, not the lozenge, because lozenge text does not wrap (lozenges do wrap at 320 px inside Forma containers, verified)."
          cov "UNB-UI-S-001" "AC-2" Yes "State 1: work-queue 'Needs your attention' is first in source order with 2 items, each with exactly one named action. Ordering and rule creation are application behaviour."
          cov "UNB-UI-G-004" "AC-1" Yes "Separate metric cards for Available to grant ($100,000.00), Market value (informational) and Pending; native disclosure 'How this is calculated' lists the three components with deducted values labelled in text."
          cov "UNB-UI-G-003" "AC-1" NeedsApplicationBehaviour "State 4 shows 'Not current' (unknown) with as-of time and states that Available to grant is used instead. Computing staleness against the business-day limit is application behaviour."
          cov "UNB-UI-G-001" "AC-2" Yes "Held grant item reads 'On hold: charity eligibility' (data-state=blocked) with a text reason."
          cov "UNB-UI-G-013" "AC-1" Yes "State 3 empty-state 'No grants yet' with a 'Recommend a grant' action; state 2 skeleton loading with aria-busy."
          cov "UNB-UI-G-015" "AC-1" NeedsApplicationBehaviour "Paid grant shown as feed-item; no toast used. Sourcing from the notification catalog is application behaviour."
          cov "UNB-UI-G-014" "AC-1" Yes "site-footer shows legal sponsor name, EIN and control statement (requires the marketing .forma-site frame; see gap on application footer)."
          cov "UNB-UI-G-010" "AC-1" NeedsApplicationBehaviour "scope-trail shows sponsor, program and fund. Re-authorizing and reloading on scope change is application behaviour." ]
      Gaps = [ "FORMA-GAP-01"; "FORMA-GAP-06"; "FORMA-GAP-07"; "FORMA-GAP-08"; "FORMA-GAP-13" ]
      Render = None }

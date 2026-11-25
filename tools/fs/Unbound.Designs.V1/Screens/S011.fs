/// S-011 Sponsor overview (port of tools/screens/S-011.mjs).
module Unbound.Designs.V1.Screens.S011

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope = scopeTrail [ "Example Charitable Foundation"; "Sponsor operations"; "Overview" ]

let private metrics (p: string) =
    let holds = Ui.metric(label = "Grants on hold", value = "19", context = "<p>Oldest 6 days. See holds by reason below.</p>")
    let stp =
        tag
            "metric-value-change"
            $$"""<article class="forma-metric-card" aria-labelledby="{{p}}-stp"><div class="forma-metric-card__label" id="{{p}}-stp">Straight-through rate, last 7 days</div><div class="forma-metric-card__value" data-forma-value-change>94.2%</div><div class="forma-metric-card__context"><p>Up from 93.8% the previous 7 days.</p></div><p class="forma-visually-hidden" aria-live="polite">Straight-through rate updated to 94.2%, up from 93.8%.</p></article>"""
    let reconciled =
        Ui.metric(
            label = "Accounts reconciled",
            value = "Not determinable",
            context = loz "unknown" "1 account not current" + "<p>No total is shown while any account's evidence is not current.</p>"
        )
    tag
        "dashboard-grid"
        $$"""<section class="forma-dashboard-grid" aria-label="Outcomes at risk">
{{holds}}
{{stp}}
{{reconciled}}
</section>"""

let private overview (p: string) =
    let holds =
        Ui.dataGrid(
            label = "Holds by reason and age",
            cols = [ Ui.col "Reason"; Ui.col "Under 1 day"; Ui.col "1 to 3 days"; Ui.col "Over 3 days"; Ui.col "Total" ],
            rows =
                [ [ "Charity eligibility not confirmed"; "7"; "3"; "1"; "11" ]
                  [ "Potential sanctions match"; "1"; "2"; "1"; "4" ]
                  [ "Payment outcome unknown"; "2"; "2"; "0"; "4" ] ]
        )
    let stp =
        Ui.dataGrid(
            label = "Straight-through rate by workflow",
            cols = [ Ui.col "Workflow"; Ui.col "Last 7 days"; Ui.col "Previous 7 days" ],
            rows =
                [ [ "Grant recommendation to approval"; "96.1%"; "95.7%" ]
                  [ "Approval to payment"; "92.4%"; "92.0%" ]
                  [ "Contribution to available"; "88.9%"; "90.3%, down 1.4 points" ] ]
        )
    let statement iso shown stateText state = Ui.fresh(label = "Statement", iso = iso, shown = shown, stateText = stateText, state = state)
    let rec' =
        Ui.dataGrid(
            label = "Reconciliation status by account",
            cols = [ Ui.col "Account"; Ui.col "Status"; Ui.col "Evidence as of"; Ui.col "Difference" ],
            rows =
                [ [ "Operating cash ••••0012"; loz "ok" "Reconciled"; statement "2026-11-30" "30 Nov 2026" "Current" ""; money "$0.00" ]
                  [ "Grant disbursement ••••0034"; loz "ok" "Reconciled"; statement "2026-11-30" "30 Nov 2026" "Current" ""; money "$0.00" ]
                  [ "Custody ••••0056"; loz "unknown" "Not current"; statement "2026-11-28" "28 Nov 2026" "Not current" "unknown"; "Not determinable" ] ]
        )
    let freshness =
        kv [ "IRS eligibility data", Ui.fresh(label = "Loaded", iso = "2026-11-30", shown = "30 Nov 2026", stateText = "Current")
             "Sanctions lists", Ui.fresh(label = "Loaded", iso = "2026-12-01T06:00:00-06:00", shown = "1 Dec 2026, 6:00 AM CT", stateText = "Current")
             "Market prices", Ui.fresh(label = "Priced", iso = "2026-11-30T16:00:00-06:00", shown = "30 Nov 2026, 4:00 PM CT", stateText = "Current")
             "Q3 2026 statements", loz "ok" "Published" + " 4,812 of 4,812 on 9 Oct 2026"
             "November 2026 monthly statements", loz "attention" "Waiting on period lock" + " Due 7 Dec 2026" ]
    let charts =
        callout $"{p}-charts" "note" "Charts deferred" "<p>Trend charts are deferred to a later release (FORMA-GAP-05). Every trend here is a table or a sentence.</p>"
    $$"""{{scope}}{{metrics p}}
<section class="forma-stack" aria-labelledby="{{p}}-holds"><h3 id="{{p}}-holds">Holds by reason and age</h3>{{holds}}</section>
<section class="forma-stack" aria-labelledby="{{p}}-stp-t"><h3 id="{{p}}-stp-t">Straight-through rate by workflow</h3>{{stp}}</section>
<section class="forma-stack" aria-labelledby="{{p}}-rec"><h3 id="{{p}}-rec">Reconciliation by account</h3>{{rec'}}<p><strong>All accounts:</strong> not determinable until Custody ••••0056 evidence is current.</p></section>
<section class="forma-stack" aria-labelledby="{{p}}-fresh"><h3 id="{{p}}-fresh">Evidence freshness and statement publication</h3>{{freshness}}</section>
{{charts}}"""

let screen: Screen =
    { Id = "S-011"
      File = "S-011-sponsor-overview.html"
      Title = "Sponsor overview"
      ReqIds = [ "UNB-UI-S-011"; "UNB-UI-G-003" ]
      Audience = "staff"
      Brand = false
      Lead = "Outcomes at risk without a composite health score: holds, straight-through rate, reconciliation, evidence freshness and statement publication. Tables and metric cards only in release 1."
      States = [ state "Overview with one stale reconciliation" "Operations lead" "UNB-UI-S-011 AC-1 (stale account shows 'Not current'; no green total)" overview ]
      Coverage =
        [ cov "UNB-UI-S-011" "AC-1" NeedsApplicationBehaviour "Custody account shows 'Not current' (unknown) with as-of; the all-accounts total reads 'Not determinable'; no ok total. Freshness computation is application behaviour."
          cov "UNB-UI-S-011" "statement (no charts)" Yes "Tables and metric cards only; FORMA-GAP-05 recorded." ]
      Gaps = [ "FORMA-GAP-05"; "FORMA-GAP-06"; "FORMA-GAP-08" ]
      Render = None }

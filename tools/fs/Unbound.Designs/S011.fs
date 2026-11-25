/// S-011 Sponsor overview, v2. One page per state; port of v2/tools/screens/S-011.mjs.
/// Data first, then pure renderers. Synthetic data only.
module Unbound.Designs.S011

open System
open System.Globalization
open Unbound.Designs.JsCompat
open Unbound.Designs.Ui
open Unbound.Designs.Screen
open Unbound.Designs.S008

let private LEAD : User = { Name = "Sam Okafor"; Initials = "SO"; Role = Some "Operations lead" }
let private NOW = "2026-12-01T16:55:00-06:00", "Dec 1, 2026 at 4:55 PM CT"

type Month = { Iso: string; Long: string; Short: string; Amount: float }

// Grants paid per month (cash out, by payment date in CT), Dec 2025 to Nov 2026.
let private MONTHS =
    [ "2025-12", "Dec 2025", "Dec ’25", 498200.0
      "2026-01", "Jan 2026", "Jan ’26", 281400.0
      "2026-02", "Feb 2026", "Feb ’26", 302150.0
      "2026-03", "Mar 2026", "Mar ’26", 355900.0
      "2026-04", "Apr 2026", "Apr ’26", 341275.0
      "2026-05", "May 2026", "May ’26", 389600.0
      "2026-06", "Jun 2026", "Jun ’26", 402880.0
      "2026-07", "Jul 2026", "Jul ’26", 318450.0
      "2026-08", "Aug 2026", "Aug ’26", 296700.0
      "2026-09", "Sep 2026", "Sep ’26", 377125.0
      "2026-10", "Oct 2026", "Oct ’26", 436470.0
      "2026-11", "Nov 2026", "Nov ’26", 512300.0 ]
    |> List.map (fun (iso, long, short, amount) -> { Iso = iso; Long = long; Short = short; Amount = amount })

let private UNCONFIRMED = 38750.0 // November payments sent but not yet seen on a bank statement (bank feed down)

let private sum (xs: Month list) = xs |> List.fold (fun s m -> s + m.Amount) 0.0
let private fmt (n: float) = formatEnUs2 n
let private short (n: float) = "$" + toFixed 1 (n / 1000.0) + "k"
let private pct (n: float) (max: float) = toFixed 2 ((n / max) * 56.0) // bars use at most 56% of the plot width
/// Unary + on a toFixed string.
let private number (s: string) = Double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture)

// ---------- chart ----------
let private ROW = 30
let private X0 = 19.0 // percent: month labels sit left of this

let private chartSvg (months: Month list) (unconfirmed: float) =
    let max = months |> List.map (fun m -> m.Amount) |> List.max
    let lo = months |> List.reduce (fun a b -> if b.Amount < a.Amount then b else a)
    let hi = months |> List.reduce (fun a b -> if b.Amount > a.Amount then b else a)
    let height = months.Length * ROW
    let bars =
        months
        |> List.mapi (fun i m ->
            let y = i * ROW
            let w = pct m.Amount max
            let isLast = i = months.Length - 1
            let split = isLast && unconfirmed <> 0.0
            let confirmedW = if split then pct (m.Amount - unconfirmed) max else w
            let part =
                if split then
                    $"""<rect class="unb-x-chart__part" x="{toFixed 2 (X0 + number confirmedW)}%%" y="{y + 6}" width="{toFixed 2 (number w - number confirmedW)}%%" height="{ROW - 12}"/>"""
                else ""
            $"""<g><text class="unb-x-chart__month" x="0" y="{y + ROW / 2}" dy="0.35em">{m.Short}</text>
<rect class="unb-x-chart__bar" x="{numberToString X0}%%" y="{y + 6}" width="{confirmedW}%%" height="{ROW - 12}" rx="2"/>{part}
<text class="unb-x-chart__value" x="{toFixed 2 (X0 + number w)}%%" dx="6" y="{y + ROW / 2}" dy="0.35em">{short m.Amount}</text></g>""")
        |> String.concat ""
    let first, last = List.head months, List.last months
    let unconfirmedText =
        if unconfirmed <> 0.0 then $" November includes ${fmt unconfirmed} sent but not yet confirmed by the bank, drawn hatched." else ""
    let desc =
        $"Horizontal bar chart, one bar per month from {first.Long} to {last.Long}. Lowest month {lo.Long}, ${fmt lo.Amount}. Highest month {hi.Long}, ${fmt hi.Amount}. Total ${fmt (sum months)}.{unconfirmedText} Exact values are in the data table below the chart."
    $"""<svg role="img" aria-labelledby="chart-t chart-d" height="{height}" focusable="false">
<title id="chart-t">Grants paid per month, last 12 months</title><desc id="chart-d">{desc}</desc>
<defs><pattern id="unb-x-hatch" width="6" height="6" patternUnits="userSpaceOnUse" patternTransform="rotate(45)"><line class="unb-x-chart__hatch-line" x1="0" y1="0" x2="0" y2="6"/></pattern></defs>
{bars}</svg>"""

let private dataTable (months: Month list) (unconfirmed: float) =
    let tr (i: int) (m: Month) =
        let note =
            if i = months.Length - 1 && unconfirmed <> 0.0 then
                $"""<br><span class="unb-small unb-muted">incl. {money unconfirmed} not yet confirmed</span>"""
            else ""
        $"""<tr><th scope="row">{time m.Iso m.Long}</th><td data-col="num">{money m.Amount}{note}</td></tr>"""
    $"""<table class="unb-x-table"><caption class="forma-visually-hidden">Grants paid per month</caption>
<thead><tr><th scope="col">Month</th><th scope="col" data-col="num">Grants paid</th></tr></thead>
<tbody>{months |> List.mapi tr |> String.concat ""}</tbody>
<tfoot><tr><th scope="row">Total, 12 months</th><td data-col="num">{money (sum months)}</td></tr></tfoot></table>"""

let private chartPanel (unconfirmed: float) =
    let legend =
        if unconfirmed <> 0.0 then
            """<ul class="unb-x-chart__legend" aria-label="Chart key"><li><span class="unb-x-chart__key" aria-hidden="true"></span>Confirmed by bank</li><li><span class="unb-x-chart__key" data-part="unconfirmed" aria-hidden="true"></span>Sent, not yet confirmed (bank feed down)</li></ul>"""
        else ""
    panel
        { P with
            Id = "chart"
            Title = "Grants paid per month"
            Meta = """<span class="unb-small unb-muted">Last 12 months, by payment date</span>"""
            Body =
                $"""<figure class="unb-x-chart">{chartSvg MONTHS unconfirmed}
{legend}
<figcaption class="unb-small unb-muted">November is the highest month in a year: year-end giving starts early. Values are rounded to the nearest $100 on the chart; exact amounts are in the table.</figcaption></figure>
<details class="unb-x-disclosure"><summary>Show the data as a table{icon "chevron-down"}</summary>{dataTable MONTHS unconfirmed}</details>""" }

// ---------- risk ----------
let private HOLDS =
    [ "Charity status not confirmed", 7, 3, 1
      "Possible sanctions match", 1, 2, 1
      "Payment outcome unknown", 2, 2, 0
      "Bank details being verified", 1, 1, 0 ]

let private holdsTable () =
    let tr (r: string, a: int, b: int, c: int) =
        let over = if c <> 0 then $"<strong>{c}</strong>" else string c
        $"""<tr><th scope="row">{r}</th><td data-col="num">{a}</td><td data-col="num">{b}</td><td data-col="num">{over}</td></tr>"""
    $"""<table class="unb-x-table"><caption class="forma-visually-hidden">Grants on hold by reason and age</caption>
<thead><tr><th scope="col">Reason</th><th scope="col" data-col="num">Under 1 day</th><th scope="col" data-col="num">1–3 days</th><th scope="col" data-col="num">Over 3 days</th></tr></thead>
<tbody>{HOLDS |> List.map tr |> String.concat ""}</tbody></table>"""

let private ACCOUNTS (down: bool) =
    [ "Operating cash ••••0012", (if down then chip "unknown" "Not current" else chip "ok" "Reconciled"), (if down then "Nov 30" else "Dec 1")
      "Grant disbursement ••••0034", (if down then chip "unknown" "Not current" else chip "blocked" "1 break open"), "Nov 30"
      "Custody ••••0056", chip "unknown" "Not current", "Nov 28" ]

let private reconPanel (down: bool) =
    let rows = ACCOUNTS down |> List.map (fun (a, c, d) -> $"""{a}<br><span class="unb-small">Bank evidence as of {d}</span>""", c)
    let all = if down then "can't be confirmed while the bank feed is down." else "not confirmed. One break is open and custody evidence is 3 days old."
    panel
        { P with
            Id = "recon"
            Title = "Reconciliation by account"
            Body = $"""{dl rows}
<p class="unb-small"><strong>All accounts:</strong> {all}</p>"""
            Foot = quiet "Open reconciliation" "#recon" "chevron-right" }

let private holdsPanel () =
    panel
        { P with
            Id = "holds"
            Title = "Grants on hold"
            Meta = """<span class="unb-small unb-muted">21 grants · oldest 6 days</span>"""
            Body = holdsTable ()
            Foot = quiet "Open work queue" "#inbox" "chevron-right" }

let private freshness (down: bool) =
    panel
        { P with
            Id = "fresh"
            Title = "Data freshness"
            Body =
                dl
                    [ "Bank statements", (if down then chip "unknown" "Not current: Nov 30" else chip "ok" "Dec 1, 6:02 AM")
                      "IRS charity records", chip "ok" "Nov 30"
                      "Sanctions lists", chip "ok" "Dec 1, 6:00 AM"
                      "Market prices", chip "ok" "Nov 30 close" ] }

let private totals (down: bool) =
    metrics
        "Key totals"
        [ { Label = "Grants paid, 12 months"; Value = money (sum MONTHS); Context = "4,906 grants to 1,240 charities"; State = "" }
          { Label = "Paid in November"
            Value = money 512300.0
            Context = (if down then $"Includes {money UNCONFIRMED} not yet confirmed by the bank" else "Up 17.4% on October")
            State = (if down then "unknown" else "") }
          { Label = "Grants on hold"; Value = "21"; Context = "2 over 3 days old"; State = "attention" }
          { Label = "Straight-through, 7 days"; Value = "94.2%"; Context = "Previous 7 days 93.8%"; State = "" } ]

let private head (sub: string) = pagehead { H with Title = "Overview"; Sub = sub; Actions = [ link "#report" "Download monthly report" ] }

let private baseP =
    { page with
        Title = "Overview"
        Nav = "funds"
        Audience = "staff"
        User = LEAD
        Brand = "unbound-default"
        Notifications = 2
        ExtraCss = Some CSS
        AppContext = "Sponsor operations" }

let private FRESH_SUB = $"Example Community Foundation · All funds · Updated {time (fst NOW) (snd NOW)}"

let private feedDown =
    notice
        { N with
            State = "info"
            IconName = Some "wifi-off"
            Title = "The bank feed has been down since 6:00 AM CT"
            Body =
                $"""<p>We haven't received statements from Prairie Community Bank since {time "2026-11-30T23:59:00-06:00" "the close of Nov 30, 2026"}. Reconciliation and November's confirmed-paid figures are therefore not current.</p>
<p><strong>Still working:</strong> grant approvals, payment sending, donor balances and the work queue. Payments sent today are tracked and will be matched when the feed returns. We retry every 15 minutes; last try {time "2026-12-01T16:45:00-06:00" "4:45 PM CT"}.</p>"""
            Actions = [ quiet "Bank feed status" "#status" "chevron-right" ] }

let states : State list =
    [ app "normal" "Overview with grants-paid chart, holds and reconciliation" "Operations lead / sponsor executive" [ "normal"; "chart"; "stale-balances" ]
          { baseP with
              Pagehead = head FRESH_SUB
              Body = $"""{totals false}<div class="unb-x-overview"><div>{chartPanel 0.0}</div><div>{holdsPanel ()}{reconPanel false}{freshness false}</div></div>""" }
      app "integration-unavailable" "Bank feed unavailable: what is stale, what still works" "Operations lead" [ "integration-unavailable"; "stale-balances" ]
          { baseP with
              Pagehead = head FRESH_SUB
              Body =
                  $"""{feedDown}
{totals true}<div class="unb-x-overview"><div>{chartPanel UNCONFIRMED}</div><div>{reconPanel true}{freshness true}{holdsPanel ()}</div></div>""" } ]

let screen : Screen = { Id = "S-011"; Slug = "S-011-sponsor-overview"; Title = "Sponsor overview"; Requirement = "UNB-UI-S-011"; States = states }

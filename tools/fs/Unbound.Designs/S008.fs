/// S-008 Sponsor work queue, v2. One page per state; port of v2/tools/screens/S-008.mjs.
/// Data first, then pure renderers. Synthetic data only.
/// Local helpers (ref, metrics, field) are exported for the other ops/charity modules and are
/// proposed for promotion to Ui (see proposed/ops-charity-widget.css).
module Unbound.Designs.S008

open Unbound.Designs.JsCompat
open Unbound.Designs.Ui
open Unbound.Designs.Screen

let CSS = [ "proposed/ops-charity-widget.css" ]
let private PREPARER : User = { Name = "Dana Ruiz"; Initials = "DR"; Role = Some "Preparer" }
let private NOW = "2026-12-01T16:55:00-06:00", "today at 4:55 PM CT"

/// Facts list for side rails: the term column keeps a readable width.
let facts (rows: (string * string) list) = dl rows |> replaceFirst "class=\"unb-dl\"" "class=\"unb-dl unb-x-facts\""

// ---------- local helpers (promote to shared layer) ----------

/// Copyable staff reference: monospace identifier + 24px+ copy button (Forma identifier has no copy affordance).
let ref (id: string) (label: string) =
    $"""<span class="unb-x-ref"><code>{id}</code><button class="unb-x-ref__copy" type="button" aria-label="Copy {label} {id}">{icon "copy"}</button></span>"""

/// A metric: State and Context "" are absent.
type Metric = { Label: string; Value: string; Context: string; State: string }

/// Metric strip: label, value, one line of context.
let metrics (label: string) (items: Metric list) =
    let li (m: Metric) =
        let state = if m.State <> "" then $" data-state=\"{m.State}\"" else ""
        let context = if m.Context <> "" then $"""<span class="unb-x-metric__context">{m.Context}</span>""" else ""
        $"""<li class="unb-x-metric"{state}><span class="unb-x-metric__label">{m.Label}</span><span class="unb-x-metric__value">{m.Value}</span>{context}</li>"""
    $"""<ul class="unb-x-metrics" aria-label="{label}">{items |> List.map li |> String.concat ""}</ul>"""

/// Labelled native select.
let select (id: string) (label: string) (options: string list) (selected: string) =
    let opt (o: string) = $"""<option{(if o = selected then " selected" else "")}>{o}</option>"""
    $"""<div class="unb-x-field"><label for="{id}">{label}</label><select id="{id}">{options |> List.map opt |> String.concat ""}</select></div>"""

// ---------- queue data ----------
let private TYPES = [ "Reconciliation"; "Payments"; "Eligibility"; "Sanctions"; "Contributions"; "Bank details" ]

/// A queue item; Href "" means none, Amount None shows no amount.
type Item =
    { Sla: string; SlaText: string; Title: string; Subject: string; Ref: string; Type: string; Owner: string; Age: string; Amount: float option; Href: string }

let private ITEMS =
    [ { Sla = chip "blocked" "Over SLA"; SlaText = "1 d 2 h over"; Title = "Possible sanctions match on payee"; Subject = "Grant to City Arts Council"; Ref = "SAN-2026-00231"; Type = "Sanctions"; Owner = "Unassigned"; Age = "4 d 2 h"; Amount = Some 7500.0; Href = "" }
      { Sla = chip "blocked" "Over SLA"; SlaText = "6 h over"; Title = "Bank and ledger differ by $2,500.00"; Subject = "Grant disbursement account ••••0034"; Ref = "REC-2026-00117"; Type = "Reconciliation"; Owner = "You"; Age = "3 d 6 h"; Amount = Some 2500.0; Href = "#rec-2026-00117" }
      { Sla = chip "attention" "Due today"; SlaText = "4 h left"; Title = "Charity status not confirmed"; Subject = "Grant to Northgate Literacy Project"; Ref = "ELG-2026-00240"; Type = "Eligibility"; Owner = "Unassigned"; Age = "1 d 20 h"; Amount = Some 2500.0; Href = "" }
      { Sla = chip "attention" "Due today"; SlaText = "7 h left"; Title = "Payment returned by charity's bank"; Subject = "Grant to Riverside Food Bank"; Ref = "PAY-2026-00233"; Type = "Payments"; Owner = "Lee Tran"; Age = "1 d 2 h"; Amount = Some 2500.0; Href = "" }
      { Sla = chip "attention" "Due tomorrow"; SlaText = "1 d left"; Title = "Deposit with no matching contribution"; Subject = "Operating cash account ••••0012"; Ref = "REC-2026-00119"; Type = "Reconciliation"; Owner = "Unassigned"; Age = "1 d 1 h"; Amount = Some 5000.0; Href = "" }
      { Sla = chip "attention" "Waiting on custodian"; SlaText = "4 d left"; Title = "Securities only partly received"; Subject = "Contribution to Lindqvist Family Fund"; Ref = "CON-2026-00391"; Type = "Contributions"; Owner = "Lee Tran"; Age = "6 d"; Amount = Some 18240.55; Href = "" }
      { Sla = chip "neutral" "Within SLA"; SlaText = "4 d 20 h left"; Title = "Charity asked to change bank details"; Subject = "Riverside Food Bank"; Ref = "DST-2026-00412"; Type = "Bank details"; Owner = "Unassigned"; Age = "4 h"; Amount = None; Href = "" }
      { Sla = chip "neutral" "Within SLA"; SlaText = "2 d left"; Title = "Custody fee posted twice"; Subject = "Custody account ••••0056"; Ref = "REC-2026-00121"; Type = "Reconciliation"; Owner = "Unassigned"; Age = "3 h"; Amount = Some 35.0; Href = "" }
      { Sla = chip "neutral" "Within SLA"; SlaText = "2 d 18 h left"; Title = "Payment outcome unknown"; Subject = "Grant to City Arts Council"; Ref = "PAY-2026-00236"; Type = "Payments"; Owner = "You"; Age = "2 h"; Amount = Some 12345.67; Href = "" } ]

let private countBy (items: Item list) = TYPES |> List.map (fun t -> t, items |> List.filter (fun i -> i.Type = t) |> List.length)

let private metric (label: string) (value: string) (context: string) (state: string) : Metric =
    { Label = label; Value = value; Context = context; State = state }

// ---------- regions ----------
let private summary () =
    metrics
        "Queue summary"
        [ metric "Over SLA" "2" "Oldest 1 d 2 h over" "blocked"
          metric "Due today" "2" "Both due by 9:00 PM CT" "attention"
          metric "Unassigned" "5" "Of 9 open items" ""
          metric "Straight-through, last 7 days" "94.2%" "1,812 of 1,923 grants needed no one" "" ]

let private toolbar (items: Item list) =
    let filters =
        countBy items
        |> List.map (fun (t, n) -> $"""<button class="unb-x-filter" type="button" aria-pressed="false">{t} <span class="unb-num">{n}</span></button>""")
        |> String.concat ""
    $"""<div class="unb-x-toolbar">
<div class="unb-x-filters" role="group" aria-label="Show items of type">
<button class="unb-x-filter" type="button" aria-pressed="true">All <span class="unb-num">{items.Length}</span></button>
{filters}
</div>
<div class="unb-x-toolbar__end">{select "owner" "Owner" [ "Anyone"; "Me"; "Unassigned" ] "Anyone"}{select "sort" "Sort by" [ "SLA risk"; "Age"; "Amount" ] "SLA risk"}</div>
</div>"""

let private th (label: string) (col: string) (sorted: bool) =
    $"""<th scope="col" data-col="{col}"{(if sorted then " aria-sort=\"ascending\"" else "")}><button class="unb-x-sort" type="button">{label}{icon "sort"}</button></th>"""

let private queueTable (items: Item list) =
    let tr (i: Item) =
        let href = if i.Href <> "" then i.Href else "#" + i.Ref.ToLowerInvariant()
        let owner = if i.Owner = "Unassigned" then """<span class="unb-muted">Unassigned</span>""" else i.Owner
        let amount =
            match i.Amount with
            | None -> """<span aria-hidden="true">—</span><span class="forma-visually-hidden">No amount</span>"""
            | Some a -> money a
        $"""<tr{(if i.Href <> "" then " data-mine=\"true\"" else "")}><td data-col="sla">{i.Sla}<span class="unb-x-queue__sub">{i.SlaText}</span></td>
<td data-col="item"><a class="unb-x-queue__title" href="{href}">{i.Title}</a><span class="unb-x-queue__sub">{i.Subject} · <span class="unb-x-mono">{i.Ref}</span></span></td>
<td data-col="type">{i.Type}</td><td data-col="owner">{owner}</td>
<td data-col="age"><span class="unb-num">{i.Age}</span></td>
<td data-col="amount">{amount}</td></tr>"""
    $"""<table class="unb-x-queue"><caption class="forma-visually-hidden">Open work items, most at risk of missing the SLA first</caption>
<thead><tr>{th "SLA" "sla" true}{th "Item" "item" false}{th "Type" "type" false}{th "Owner" "owner" false}{th "Age" "age" false}{th "Amount" "amount" false}</tr></thead>
<tbody>{items |> List.map tr |> String.concat ""}</tbody></table>"""

let private queuePanel (items: Item list) =
    panel
        { P with
            Id = "q"
            Title = "Open items"
            Meta = $"""<span class="unb-small unb-muted">{items.Length} items · updated {time (fst NOW) (snd NOW)}</span>"""
            Body = toolbar items + queueTable items
            Foot =
                $"""<span class="unb-small unb-muted">Items close on their own when their exit condition is met. Routine work never appears here.</span>{quiet "Recently closed" "#closed" "chevron-right"}""" }

let private queueHead (sub: string option) =
    let sub = defaultArg sub ("Example Community Foundation · " + time (fst NOW) ("Updated " + snd NOW))
    pagehead { H with Title = "Work queue"; Sub = sub; Actions = [ primaryLink "#next" "Take next item" ] }

let private baseP =
    { page with
        Title = "Work queue"
        Nav = "inbox"
        Audience = "staff"
        User = PREPARER
        Brand = "unbound-default"
        Notifications = 3
        ExtraCss = Some CSS
        Counts = [ "inbox", 9 ] }

// ---------- reconciliation break detail ----------
let private BREAK_REF = "REC-2026-00117"
let private BREAK_ACCOUNT = "Grant disbursement account ••••0034"
let private BANK_CLOSING, BANK_OUT = 1204330.12, 38750.0
let private LEDGER_CLOSING, LEDGER_OUT = 1201830.12, 41250.0

let private compare () =
    let bank = dl [ "Balance at Nov 30 close", money BANK_CLOSING; "Payments out on Nov 30", money BANK_OUT; "Payments counted", "<span class=\"unb-num\">14</span>" ]
    let ledgerSide = dl [ "Balance at Nov 30 close", money LEDGER_CLOSING; "Payments out on Nov 30", money LEDGER_OUT; "Payments counted", "<span class=\"unb-num\">15</span>" ]
    $"""<div class="unb-x-compare">
<section class="unb-x-compare__side" aria-labelledby="cmp-bank"><h3 id="cmp-bank">{icon "database"}Bank statement</h3><p class="unb-small unb-muted">Prairie Community Bank, received {time "2026-12-01T06:02:00-06:00" "Dec 1, 6:02 AM CT"}</p>
{bank}</section>
<section class="unb-x-compare__side" aria-labelledby="cmp-ledger"><h3 id="cmp-ledger">{icon "file-text"}Our ledger</h3><p class="unb-small unb-muted">Grant payments recorded through Nov 30</p>
{ledgerSide}</section>
<p class="unb-x-compare__diff"><span>Difference</span>{money 2500.0}<span class="unb-small">The bank holds more than our ledger shows: one payment we recorded did not leave the account.</span></p>
</div>"""

type Candidate = { Id: string; Title: string; Text: string; Strength: string; Checked: bool }

let private CANDIDATES =
    [ { Id = "c1"; Title = "Returned payment: grant to Riverside Food Bank"; Text = $"""Bank entry “RETURN R02 ACCOUNT CLOSED”, {money 2500.0}, value date {time "2026-11-30" "Nov 30, 2026"}. Same amount and payee reference as grant 2026-004790."""; Strength = chip "ok" "Strong match"; Checked = true }
      { Id = "c2"; Title = "Grant to Lakeview Animal Rescue, not yet debited"; Text = $"""Same amount ({money 2500.0}), but scheduled for {time "2026-12-03" "Dec 3"}; the bank shows no entry for it yet."""; Strength = chip "neutral" "Weak match"; Checked = false } ]

let private candidates (proposed: bool) =
    let choice (c: Candidate) =
        $"""<div class="unb-x-choice"><input type="radio" name="cand" id="{c.Id}"{(if c.Checked then " checked" else "")}><label for="{c.Id}"><span class="unb-x-choice__title">{c.Title}</span>{c.Strength}<span class="unb-x-choice__text">{c.Text}</span></label></div>"""
    $"""<fieldset class="unb-x-choices"{(if proposed then " disabled" else "")}><legend>Matching candidates</legend>{CANDIDATES |> List.map choice |> String.concat ""}</fieldset>"""

/// A duty step; Note "" is absent.
type Duty = { Step: string; Role: string; Who: string; Note: string }

/// Who does what: two duties, two different people (segregation of duties made visible).
let private authority (steps: Duty list) =
    let li (s: Duty) =
        let current = if s.Step = "current" then " aria-current=\"step\"" else ""
        let note = if s.Note <> "" then $"""<span class="unb-x-duties__note">{s.Note}</span>""" else ""
        $"""<li data-step="{s.Step}"{current}><span class="unb-x-duties__role">{s.Role}</span><span class="unb-x-duties__who">{s.Who}</span>{note}</li>"""
    $"""<ol class="unb-x-duties">{steps |> List.map li |> String.concat ""}</ol>"""

let private RESOLUTIONS =
    [ "Match to the returned payment", chip "ok" "You can propose", "Reverses the Nov 30 grant payment in the ledger. The $2,500.00 goes back to the Lindqvist Family Fund's Available to grant and the donor is told."
      "Mark as bank error and query the bank", chip "ok" "You can propose", "Keeps the ledger as is and opens a query with Prairie Community Bank. Use this only if no candidate fits."
      "Write off the difference", chip "blocked" "Controller only", "Write-offs over $100.00 need a controller to propose them. You can't choose this." ]

let private resolvePanel () =
    let options =
        RESOLUTIONS
        |> List.map (fun (t, c, d) -> $"""<div role="listitem" class="unb-x-option"><p class="unb-x-option__title">{t}</p>{c}<p class="unb-x-option__text">{d}</p></div>""")
        |> String.concat ""
    let duties =
        authority
            [ { Step = "current"; Role = "1 · Propose"; Who = "You (Dana Ruiz, preparer)"; Note = "" }
              { Step = "todo"; Role = "2 · Approve"; Who = "A different approver"; Note = "Sam Okafor or Priya Nair. The person who proposes can never approve." } ]
    panel
        { P with
            Id = "res"
            Title = "Resolve this break"
            Body =
                $"""{duties}
<div class="unb-x-options" role="list" aria-label="Allowed resolutions">{options}</div>
<div class="unb-x-actions">{primary "Propose match for approval"}{button "Query the bank instead"}</div>
<p class="unb-small unb-muted">Nothing changes in the ledger until an approver accepts. You can withdraw a proposal until then.</p>""" }

let private proposedPanel () =
    let duties =
        authority
            [ { Step = "done"; Role = "1 · Propose"; Who = "You (Dana Ruiz, preparer)"; Note = $"""Done {time "2026-12-01T17:12:00-06:00" "Dec 1, 5:12 PM CT"}""" }
              { Step = "current"; Role = "2 · Approve"; Who = "Sam Okafor or Priya Nair"; Note = "Asked to approve. Usually within 4 business hours." } ]
    let n =
        notice
            { N with
                State = "info"
                IconName = Some "lock"
                Title = "You can't approve your own proposal"
                Body = "<p>You prepared this resolution, so a different approver has to check and accept it. This separation of duties is required for every ledger change; it protects you as well as the fund.</p>" }
    panel
        { P with
            Id = "res"
            Title = "Resolve this break"
            Tone = "attention"
            Body =
                $"""<p><strong>You proposed:</strong> match to the returned payment to Riverside Food Bank, on {time "2026-12-01T17:12:00-06:00" "Dec 1, 2026 at 5:12 PM CT"}.</p>
{duties}
{n}
<div class="unb-x-actions">{button "Withdraw proposal"}</div>""" }

let private blockedPanel () =
    panel
        { P with
            Id = "blk"
            Title = "While this is open"
            Body =
                dl
                    [ "Close November books", chip "blocked" "Blocked"
                      "Publish November statements for 412 funds", chip "blocked" "Blocked"
                      "New grants to Riverside Food Bank", chip "attention" "Held for bank details"
                      "Grant payments from this account", chip "ok" "Continue"
                      "Donor balances and grant approvals", chip "ok" "Continue" ]
            Foot = $"""<span class="unb-small unb-muted">November close is due {time "2026-12-07" "Dec 7, 2026"}.</span>""" }

let private factsPanel () =
    panel
        { P with
            Id = "facts"
            Title = "Details"
            Body =
                facts
                    [ "Reference", ref BREAK_REF "reference"
                      "Created by rule", "REC-01 daily bank match"
                      "Created", time "2026-11-28T10:55:00-06:00" "Nov 28, 2026, 10:55 AM CT"
                      "SLA", "3 business days · 6 h over"
                      "Owner", "Dana Ruiz (you)"
                      "Closes when", "Bank and ledger agree for Nov 30" ] }

let private explain () =
    panel
        { P with
            Id = "what"
            Title = "What happened"
            Body =
                $"""<p>On Nov 30 the bank shows {money 2500.0} less going out of the grant disbursement account than our ledger recorded. The most likely reason: the payment to Riverside Food Bank was returned because the charity's account is closed, so the money never left.</p>{compare ()}{candidates false}""" }

let private explainProposed () =
    panel
        { P with
            Id = "what"
            Title = "What happened"
            Body =
                $"""<p>On Nov 30 the bank shows {money 2500.0} less going out of the grant disbursement account than our ledger recorded. The payment to Riverside Food Bank was returned because the charity's account is closed.</p>{compare ()}{candidates true}""" }

let private detailHead (status: string) =
    pagehead
        { H with
            Context = [ """<a href="#inbox">Work queue</a>"""; "Reconciliation"; "Item 2 of 9" ]
            Title = "Bank and ledger differ by $2,500.00"
            Sub = $"""<span class="unb-x-headmeta">{status}<span>{BREAK_ACCOUNT} · Nov 30 statement</span></span>"""
            Actions = [ quiet "Next item" "#next" "chevron-right" ] }

let private detail (main: string) (side: string) =
    $"""<div class="unb-x-detail"><div class="unb-x-detail__main">{main}</div><div class="unb-x-detail__side">{side}</div></div>"""

// ---------- states ----------
let private emptyBody =
    panel
        { P with
            Id = "empty"
            Label = "Queue status"
            Body =
                $"""<div class="unb-x-empty">{icon "success"}<p class="unb-x-empty__title">Nothing needs you right now</p><p class="unb-muted">Every item the rules created is resolved. Last checked {time (fst NOW) (snd NOW)}; we check again every 5 minutes and new items appear here.</p></div>""" }
    + panel
        { P with
            Id = "closed"
            Title = "Closed automatically today"
            Meta = """<span class="unb-small unb-muted">3 items</span>"""
            Body =
                $"""<ul class="unb-x-closed">
<li><span class="unb-x-closed__title">Charity status confirmed: Northgate Literacy Project</span>{chip "ok" "Closed: exit condition met"}<span class="unb-small unb-muted">New IRS record confirmed public-charity status at {time "2026-12-01T16:40:00-06:00" "4:40 PM CT"}. No one had to act.</span></li>
<li><span class="unb-x-closed__title">Deposit matched to contribution: Lindqvist Family Fund</span>{chip "ok" "Closed: exit condition met"}<span class="unb-small unb-muted">Contribution record arrived at {time "2026-12-01T14:05:00-06:00" "2:05 PM CT"} and matched {money 5000.0}.</span></li>
<li><span class="unb-x-closed__title">Payment outcome confirmed: City Arts Council</span>{chip "ok" "Closed: exit condition met"}<span class="unb-small unb-muted">Bank confirmed the payment at {time "2026-12-01T11:30:00-06:00" "11:30 AM CT"}.</span></li>
</ul>"""
            Foot = quiet "All recently closed" "#closed" "chevron-right" }

let private loadingBody =
    let metricsSkel = [ 1..4 ] |> List.map (fun _ -> $"""<li class="unb-x-metric">{skel "line-short"}{skel "value"}{skel "line"}</li>""") |> String.concat ""
    let rows = [ 1..6 ] |> List.map (fun _ -> skel "row") |> String.concat ""
    let queue = panel { P with Id = "q"; Title = "Open items"; Body = $"""<div class="unb-x-toolbar" aria-hidden="true">{skel "line"}</div>{rows}""" }
    $"""<div class="unb-stack" aria-busy="true" aria-describedby="loading-msg"><p id="loading-msg" class="forma-visually-hidden" role="status">Loading the work queue</p>
<ul class="unb-x-metrics" aria-hidden="true">{metricsSkel}</ul>
{queue}</div>"""

let states : State list =
    [ app "queue" "Exception-first queue sorted by SLA risk" "Sponsor operations preparer" [ "normal"; "attention-required"; "sla-breach" ]
          { baseP with Pagehead = queueHead None; Body = summary () + queuePanel ITEMS }
      app "reconciliation-break" "Reconciliation break selected: bank vs ledger, candidates and who can resolve" "Sponsor operations preparer (owner of the item)" [ "reconciliation-exception"; "segregation-of-duties"; "attention-required" ]
          { baseP with
              Pagehead = detailHead (chip "blocked" "Over SLA by 6 h")
              Body = detail (explain () + resolvePanel ()) (blockedPanel () + factsPanel ()) }
      app "proposed-by-me" "Same break after proposing: preparer cannot approve own proposal" "Sponsor operations preparer who proposed" [ "segregation-of-duties"; "pending-approval"; "read-only" ]
          { baseP with
              Pagehead = detailHead (chip "attention" "Waiting for approval")
              Body = detail (proposedPanel () + explainProposed ()) (blockedPanel () + factsPanel ()) }
      app "empty" "Empty queue: nothing needs you" "Sponsor operations preparer" [ "empty" ]
          { baseP with
              Counts = []
              Notifications = 0
              Pagehead = pagehead { H with Title = "Work queue"; Sub = "Example Community Foundation" }
              Body = emptyBody }
      app "loading" "Loading: skeleton matches the queue layout" "Sponsor operations preparer" [ "loading" ]
          { baseP with Counts = []; Pagehead = queueHead (Some "Example Community Foundation"); Body = loadingBody } ]

let screen : Screen = { Id = "S-008"; Slug = "S-008-work-queue"; Title = "Sponsor work queue"; Requirement = "UNB-UI-S-008"; States = states }

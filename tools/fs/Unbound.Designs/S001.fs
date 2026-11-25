/// S-001 Giving home, v2 (report 18). One page per state; port of v2/tools/screens/S-001.mjs.
/// Data first, then pure renderers. Synthetic data only.
module Unbound.Designs.S001

open System.Text.RegularExpressions
open Unbound.Designs.JsCompat
open Unbound.Designs.Ui
open Unbound.Designs.Screen

let private DONOR : User = { Name = "Elin Lindqvist"; Initials = "EL"; Role = None }
let private FUND = "Lindqvist Family Fund"

type Fund =
    { Available: float; Committed: float; Settling: float; Value: float; Updated: string * string; Priced: string * string }

let private fund =
    { Available = 100000.0
      Committed = 15000.0
      Settling = 5000.0
      Value = 120000.0
      Updated = "2026-12-01T16:58:00-06:00", "today at 4:58 PM CT"
      Priced = "2026-11-30T15:00:00-06:00", "Nov 30, 2026 market close" }

let private pct (part: float) (whole: float) = toFixed 1 ((part / whole) * 100.0) + "%"

// ---------- regions ----------
let private balance (f: Fund) (valueState: string) (availableState: string) (sponsor: string) =
    let stale = availableState = "stale"
    let meta =
        if stale then
            $"""<span>{chip "unknown" "Not current"}</span><span>Last confirmed {time "2026-11-27T16:58:00-06:00" "Nov 27, 4:58 PM CT"}</span>"""
        else
            $"""<span>{icon "success"} Confirmed by {sponsor}</span><span>Updated {time (fst f.Updated) (snd f.Updated)}</span>"""
    let valueRow =
        if valueState = "unavailable" then
            $"""<li data-part="total"><span class="unb-alloc__key" data-part="total"></span><span>Total fund value<small>Investment values are temporarily unavailable</small></span>{chip "unknown" "Unavailable"}</li>"""
        else
            $"""<li data-part="total"><span class="unb-alloc__key" data-part="total"></span><span>Total fund value<small>Market value, {time (fst f.Priced) (snd f.Priced)}</small></span>{money f.Value}</li>"""
    let bar =
        if valueState = "unavailable" || f.Value = 0.0 then ""
        else
            $"""<div class="unb-alloc__bar" aria-hidden="true"><span class="unb-alloc__seg" data-part="available" style="inline-size:{pct f.Available f.Value}"></span><span class="unb-alloc__seg" data-part="committed" style="inline-size:{pct f.Committed f.Value}"></span><span class="unb-alloc__seg" data-part="settling" style="inline-size:{pct f.Settling f.Value}"></span></div>"""
    panel
        { P with
            Id = "bal"
            Label = "Fund balance"
            Body =
                $"""<div class="unb-balance">
<div><p class="unb-balance__label" id="bal-label">Available to grant</p>
<p class="unb-balance__value" aria-describedby="bal-label">{moneyHero f.Available}</p>
<p class="unb-balance__meta">{meta}</p></div>
<div class="unb-alloc">{bar}
<ul class="unb-alloc__legend" aria-label="How your fund is made up">
<li><span class="unb-alloc__key" data-part="available"></span><span>Available to grant</span>{money f.Available}</li>
<li><span class="unb-alloc__key" data-part="committed"></span><span>Committed to grants in progress<small>Leaves your fund when each grant is paid</small></span>{money f.Committed}</li>
<li><span class="unb-alloc__key" data-part="settling"></span><span>Contributions still settling<small>Available to grant on {time "2026-12-08" "Dec 8, 2026"}</small></span>{money f.Settling}</li>
{valueRow}
</ul></div></div>"""
            Foot =
                $"""{quiet "How we calculate this" "#calc" "chevron-right"}<span class="unb-small unb-muted unb-hide-phone">Only Available to grant can be recommended.</span>""" }

let private attention (items: Task list) =
    panel
        { P with
            Id = "att"
            Title = "Needs your attention"
            Meta = $"""<span class="unb-small unb-muted">{items.Length} {(if items.Length = 1 then "item" else "items")}</span>"""
            Tone = "attention"
            Body = tasks items }

let private certify : Task =
    { State = "attention"
      IconName = Some "clipboard"
      Title = "Confirm your 2026 donor certification"
      Text = $"""Due {time "2026-12-15" "Dec 15, 2026"}. Confirm that no grant from this fund gave you or your family a personal benefit."""
      Actions = [ link "#certify" "Review and confirm" ] }

let private hold : Task =
    { State = "blocked"
      IconName = None
      Title = "A grant is on hold: Northgate Literacy Project"
      Text = $"""We couldn't confirm the charity's tax-exempt status on {time "2026-11-27" "Nov 27"}. You can wait while we check, or choose a different charity."""
      Actions = [ link "#grant-004811" "Review options" ] }

let private returned : Task =
    { State = "blocked"
      IconName = Some "refresh"
      Title = "A payment was returned: Riverside Food Bank"
      Text = $"""The charity's bank returned the {money 2500.0} payment on {time "2026-12-02" "Dec 2"}. The money is back in Available to grant. Choose whether to wait for new bank details or send it elsewhere."""
      Actions = [ link "#grant-004790" "Review options" ] }

type Grant = { To: string; Id: string; Amount: float; Chip: string; Next: string; Steps: (string * string) list }

let private GRANTS =
    [ { To = "Riverside Food Bank"; Id = "Grant 2026-004822"; Amount = 5000.0; Chip = chip "ok" "Approved"; Next = $"""Payment scheduled {time "2026-12-03" "Dec 3"}"""; Steps = [ "Submitted", "done"; "Approved", "done"; "Payment scheduled", "current" ] }
      { To = "City Arts Council"; Id = "Grant 2026-004830"; Amount = 7500.0; Chip = chip "attention" "In review"; Next = "Usually approved within 2 business days"; Steps = [ "Submitted", "done"; "In review", "current"; "Paid", "todo" ] }
      { To = "Northgate Literacy Project"; Id = "Grant 2026-004811"; Amount = 2500.0; Chip = chip "blocked" "On hold"; Next = "Charity status being checked"; Steps = [ "Submitted", "done"; "On hold", "failed"; "Paid", "todo" ] } ]

let private grantsInProgress (list: Grant list) =
    let li (g: Grant) =
        $"""<li class="unb-task unb-grant-row"><span class="unb-task__icon unb-task__icon--neutral">{icon "send"}</span>
<div class="unb-split"><p class="unb-task__title"><a href="#{Regex.Replace(g.Id, @"\s", "-").ToLowerInvariant()}">{g.To}</a></p>{money g.Amount}</div>
<div class="unb-split"><span>{g.Chip}</span><span class="unb-small unb-muted">{g.Next}</span></div></li>"""
    panel
        { P with
            Id = "gip"
            Title = "Grants in progress"
            Meta = $"""<span class="unb-small unb-muted">{money (list |> List.fold (fun s g -> s + g.Amount) 0.0)} committed</span>"""
            Body = $"""<ul class="unb-tasks">{list |> List.map li |> String.concat ""}</ul>"""
            Foot = quiet "All grants" "#grants" "chevron-right" }

let private ACTIVITY : LedgerRow list =
    [ { What = "Grant paid"; Sub = "Riverside Food Bank"; Status = chip "ok" "Paid"; Date = time "2026-11-30" "Nov 30, 2026"; Amount = money -2500.0 }
      { What = "Contribution received"; Sub = "Bank transfer from ••••4821"; Status = chip "attention" "Settling"; Date = time "2026-11-24" "Nov 24, 2026"; Amount = moneySigned 5000.0 }
      { What = "Grant paid"; Sub = "Lakeview Animal Rescue"; Status = chip "ok" "Paid"; Date = time "2026-11-12" "Nov 12, 2026"; Amount = money -1000.0 } ]

let private activity (rows: LedgerRow list) =
    panel
        { P with
            Id = "act"
            Title = "Recent activity"
            Body = ledger "Recent activity, newest first" true rows
            Foot = quiet "All activity and statements" "#activity" "chevron-right" }

let private ACTIONS = [ link "#add-money" "Add money"; primaryLink "#recommend" "Recommend a grant" ]

let private headWith (sub: string option) (actions: string list option) =
    pagehead
        { H with
            Title = FUND
            FundSwitch = true
            Sub = defaultArg sub "Donor-advised fund · You are the primary advisor"
            Actions = defaultArg actions ACTIONS }

let private head () = headWith None None

// Priority order in the source: balance (what can I give), attention (what needs me), then the rest.
// On a 390x844 phone the headline figure and the attention heading both fall in the first viewport.
let private home (att: string) (left: string) (main: string) (right: string) =
    let attHtml = if att <> "" then $"""<div data-area="attention">{att}</div>""" else ""
    let mainHtml = if main <> "" then $"""<div data-area="main">{main}</div>""" else ""
    $"""<div class="unb-grid-home"><div data-area="balance">{left}</div>{attHtml}{mainHtml}<div data-area="side">{right}</div></div>"""

let private sponsorOf (brand: string) =
    if brand = "example-harbor" then "Example Charitable Foundation" else "Example Community Foundation"

let private ECF = "Example Community Foundation"

// ---------- states ----------
let private baseOf (brand: string) =
    { page with Title = "Home"; Nav = "home"; Audience = "donor"; User = DONOR; Brand = brand; Notifications = 2 }

let private normal (brand: string) =
    { baseOf brand with
        Pagehead = head ()
        Body = home (attention [ certify; hold ]) (balance fund "current" "current" (sponsorOf brand)) (grantsInProgress GRANTS) (activity ACTIVITY) }

let private skelPanels =
    let left =
        panel { P with Id = "bal"; Label = "Fund balance, loading"; Body = $"""<div class="unb-balance"><div>{skel "line-short"}<p>{skel "value"}</p>{skel "line-short"}</div><div class="unb-alloc">{skel "bar"}{skel "line"}{skel "line"}{skel "line"}</div></div>""" }
        + panel { P with Id = "gip"; Title = "Grants in progress"; Body = skel "row" + skel "row" + skel "row" }
    let right = panel { P with Id = "act"; Title = "Recent activity"; Body = skel "row" + skel "row" + skel "row" }
    left, right

let private loading =
    let left, right = skelPanels
    { baseOf "unbound-default" with
        Notifications = 0
        Pagehead = head ()
        Body = $"""<div aria-busy="true" aria-describedby="loading-msg"><p id="loading-msg" class="forma-visually-hidden" role="status">Loading your fund</p>{home "" left "" right}</div>""" }

let private newFund =
    let left =
        panel
            { P with
                Id = "start"
                Title = "Get your fund started"
                Meta = """<span class="unb-small unb-muted">1 of 3 done</span>"""
                Body =
                    $"""<ol class="unb-checklist">
<li data-step="done"><p class="unb-checklist__title">Open your fund</p><p class="unb-small unb-muted">Opened {time "2026-12-01" "Dec 1, 2026"}. Your fund agreement is in Documents.</p></li>
<li data-step="current" aria-current="step"><p class="unb-checklist__title">Add your first contribution</p><p class="unb-small unb-muted">Bank transfer, wire, check or securities. Bank transfers are available to grant about 10 business days after they arrive.</p><div class="unb-task__action">{primaryLink "#add-money" "Add money"}</div></li>
<li><p class="unb-checklist__title">Recommend your first grant</p><p class="unb-small unb-muted">You can look up charities now; recommendations open once money is available.</p><div class="unb-task__action">{quiet "Find a charity" "#charities" "search"}</div></li>
</ol>""" }
    let right =
        panel { P with Id = "bal0"; Title = "Available to grant"; Body = $"""<p class="unb-balance__value">{moneyHero 0.0}</p><p class="unb-small unb-muted">Contributions appear here once they settle.</p>""" }
        + panel { P with Id = "who"; Title = "Your sponsor"; Body = """<p class="unb-small">Example Community Foundation approves grants and holds legal control of your fund. Questions? <a href="#contact">Contact donor services</a>.</p>""" }
    { baseOf "unbound-default" with
        Notifications = 0
        Pagehead = headWith None (Some [ primaryLink "#add-money" "Add money" ])
        Body = home "" left "" right }

let private noGrants =
    { baseOf "unbound-default" with
        Notifications = 0
        Pagehead = head ()
        Body =
            home
                ""
                (balance { fund with Available = 50000.0; Committed = 0.0; Settling = 0.0; Value = 50250.0 } "current" "current" ECF)
                ""
                (panel
                    { P with
                        Id = "nog"
                        Title = "Grants"
                        Body = $"""<div class="unb-empty"><p class="unb-task__title">You haven't recommended a grant yet</p><p class="unb-small unb-muted">Search U.S. charities by name, cause or location. We check eligibility before your recommendation is submitted.</p><div class="unb-task__action">{primaryLink "#recommend" "Recommend a grant"}</div></div>""" }) }

let private staleBalance =
    { baseOf "unbound-default" with
        Pagehead = head ()
        Body =
            notice { N with State = "info"; IconName = Some "clock"; Title = "We can't confirm today's balance yet"; Body = $"""<p>The amounts below are from the last confirmed update on {time "2026-11-27T16:58:00-06:00" "Nov 27 at 4:58 PM CT"}. You can still recommend grants; each one is checked against the confirmed balance before it is approved.</p>""" }
            + "\n"
            + home (attention [ certify ]) (balance fund "current" "stale" ECF) (grantsInProgress GRANTS) (activity ACTIVITY) }

let private integrationUnavailable =
    { baseOf "unbound-default" with
        Pagehead = head ()
        Body =
            home
                (attention [ certify ])
                (balance fund "unavailable" "current" ECF)
                (grantsInProgress GRANTS)
                (notice { N with State = "info"; IconName = Some "wifi-off"; Title = "Investment values are temporarily unavailable"; Body = "<p>We can't reach the investment pricing service. Your Available to grant amount isn't affected and you can keep recommending grants. We'll update values automatically.</p>" }
                 + activity ACTIVITY) }

let private restricted =
    { baseOf "unbound-default" with
        Pagehead = headWith None (Some [ link "#add-money" "Add money" ])
        Body =
            notice { N with State = "attention"; IconName = Some "lock"; Title = "Grant recommendations are paused"; Body = "<p>Example Community Foundation is reviewing a recent change to your account details. Reviews usually finish within 2 business days. Contributions, documents and grants already approved are not affected.</p>"; Actions = [ link "#contact" "Contact donor services" ] }
            + "\n"
            + home
                ""
                (balance fund "current" "current" ECF)
                (grantsInProgress GRANTS)
                (panel { P with Id = "why"; Title = "What you can still do"; Body = dl [ "Add money", chip "ok" "Available"; "View and download documents", chip "ok" "Available"; "Recommend a grant", chip "blocked" "Paused"; "Change advisors or successors", chip "blocked" "Paused" ] }
                 + activity ACTIVITY) }

let private suspended =
    { baseOf "unbound-default" with
        Notifications = 1
        Pagehead = headWith None (Some [])
        Body =
            notice { N with State = "blocked"; Title = "This fund is suspended"; Body = $"""<p>Example Community Foundation suspended this fund on {time "2026-11-20" "Nov 20, 2026"} because the annual donor certification is more than 90 days overdue. No grants or contributions can be made until it is complete.</p>"""; Actions = [ primaryLink "#certify" "Complete certification"; link "#contact" "Contact donor services" ] }
            + "\n"
            + home "" (balance fund "current" "current" ECF) "" (activity ACTIVITY) }

let private succession =
    { baseOf "unbound-default" with
        User = { Name = "Maya Lindqvist"; Initials = "ML"; Role = Some "Successor" }
        Notifications = 1
        Pagehead = headWith (Some "Donor-advised fund · You are named as successor advisor") (Some [])
        Body =
            notice { N with State = "info"; IconName = Some "users"; Title = "You've been named successor advisor"; Body = $"""<p>We're sorry for your loss. Example Community Foundation recorded Elin Lindqvist's death on {time "2026-11-18" "Nov 18, 2026"}, and the fund's succession plan names you as successor. Grants and contributions are paused until you accept.</p>"""; Actions = [ primaryLink "#accept" "Review and accept"; link "#contact" "Decline or ask a question" ] }
            + "\n"
            + home
                ""
                (panel { P with Id = "next"; Title = "What happens next"; Body = $"""<ol class="unb-checklist"><li data-step="done"><p class="unb-checklist__title">Sponsor verifies the death notice</p><p class="unb-small unb-muted">Completed {time "2026-11-18" "Nov 18"}</p></li><li data-step="current" aria-current="step"><p class="unb-checklist__title">You review and accept the successor role</p><p class="unb-small unb-muted">About 10 minutes. You'll confirm your identity and agree to the program terms.</p></li><li><p class="unb-checklist__title">The fund reopens with you as advisor</p><p class="unb-small unb-muted">You can then recommend grants from the fund's balance.</p></li></ol>""" })
                ""
                (panel { P with Id = "val"; Title = "Fund at a glance"; Body = dl [ "Fund value when paused", money 120000.0; "Grants in progress", "3, paused"; "Successors named", "1 (you)" ] }) }

let private readOnly =
    { baseOf "unbound-default" with
        User = { Name = "Jordan Patel"; Initials = "JP"; Role = Some "View only" }
        Notifications = 0
        Pagehead = headWith (Some $"""Donor-advised fund · {chip "neutral" "View only"} Shared with you by Elin Lindqvist""") (Some [ link "#statement" "Download statement" ])
        Body =
            home
                ""
                (balance fund "current" "current" ECF)
                (grantsInProgress GRANTS)
                (panel { P with Id = "acc"; Title = "Your access"; Body = $"""<p class="unb-small">You can see balances, grants and statements. Only the fund's advisors can add money or recommend grants. Access expires {time "2027-06-30" "Jun 30, 2027"}.</p>""" }
                 + activity ACTIVITY) }

let private failedPayment =
    let grants =
        [ { To = "Riverside Food Bank"; Id = "Grant 2026-004790"; Amount = 2500.0; Chip = chip "blocked" "Payment returned"; Next = "Waiting for your choice"; Steps = [ "Submitted", "done"; "Approved", "done"; "Payment returned", "failed" ] }
          GRANTS.[1]
          { GRANTS.[2] with Amount = 2500.0 } ]
    let returnedRow = { What = "Grant payment returned"; Sub = "Riverside Food Bank"; Status = chip "blocked" "Returned"; Date = time "2026-12-02" "Dec 2, 2026"; Amount = moneySigned 2500.0 }
    { baseOf "unbound-default" with
        Notifications = 3
        Pagehead = head ()
        Body =
            home
                (attention [ returned; certify ])
                (balance { fund with Available = 102500.0; Committed = 12500.0 } "current" "current" ECF)
                (grantsInProgress grants)
                (activity (returnedRow :: ACTIVITY)) }

let states : State list =
    [ app "normal" "Normal: pending grants and two attention items" "Donor (primary advisor)" [ "normal"; "pending-grants"; "attention-required" ] (normal "unbound-default")
      app "white-label" "White-labelled sponsor (Harbor Giving brand manifest)" "Donor (primary advisor)" [ "white-label"; "normal" ] (normal "example-harbor")
      app "loading" "Loading: skeleton matches the final layout" "Donor" [ "loading" ] loading
      app "new-fund" "New fund: no contributions yet (onboarding)" "Donor, fund opened today" [ "no-contributions"; "no-grants" ] newFund
      app "no-grants" "Money available, no grants yet" "Donor, first contribution settled" [ "no-grants" ] noGrants
      app "stale-balance" "Available to grant cannot be confirmed (stale)" "Donor" [ "stale-balances" ] staleBalance
      app "integration-unavailable" "Investment pricing unavailable (integration down)" "Donor" [ "integration-unavailable" ] integrationUnavailable
      app "restricted" "Restricted: grant recommendations paused by sponsor review" "Donor" [ "restricted-fund" ] restricted
      app "suspended" "Fund suspended by sponsor" "Donor" [ "suspended-fund" ] suspended
      app "succession" "Succession pending: successor's first view" "Successor advisor before activation" [ "succession-pending"; "suspended-fund" ] succession
      app "read-only" "Read-only: financial advisor with view access" "Financial advisor (view only)" [ "read-only" ] readOnly
      app "failed-payment" "Failed payment: grant payment returned by charity's bank" "Donor" [ "failed-payment"; "attention-required" ] failedPayment ]

let screen : Screen = { Id = "S-001"; Slug = "S-001-giving-home"; Title = "Giving home"; Requirement = "UNB-UI-S-001"; States = states }

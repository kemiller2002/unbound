/// S-004 Grant detail, v2. One page per state; port of v2/tools/screens/S-004.mjs.
/// Data first, then pure renderers. Synthetic data only (same fund and charities as S-001).
/// Local components (proposed, see proposed/donor-grants.css): record header, milestone
/// track, event timeline, decision options, copyable reference, document list,
/// unavailable-action-with-reason. track and kv are also used by S-003.
module Unbound.Designs.S004

open Unbound.Designs.Ui
open Unbound.Designs.Screen

let private DONOR : User = { Name = "Elin Lindqvist"; Initials = "EL"; Role = None }
let private ADVISOR : User = { Name = "Jordan Patel"; Initials = "JP"; Role = Some "View only" }
let private SPONSOR = "Example Community Foundation"
let private FUND = "Lindqvist Family Fund"

// ---------- local helpers (promote to shared layer) ----------

/// A milestone: step is done|current|todo|failed.
type Milestone = { Label: string; When: string; Step: string }

/// Milestone track -> ordered list with a date line each.
let track (label: string) (items: Milestone list) : string =
    let li (m: Milestone) =
        let hidden =
            match m.Step with
            | "done" -> """<span class="forma-visually-hidden">, done</span>"""
            | "failed" -> """<span class="forma-visually-hidden">, problem</span>"""
            | "current" -> """<span class="forma-visually-hidden">, current step</span>"""
            | _ -> ""
        let current = if m.Step = "current" then " aria-current=\"step\"" else ""
        $"""<li data-step="{m.Step}"{current}><span class="unb-x-track__label">{m.Label}{hidden}</span><span class="unb-x-track__when">{m.When}</span></li>"""
    $"""<ol class="unb-x-track" aria-label="{label}">{items |> List.map li |> String.concat ""}</ol>"""

/// Stacked key-value list for long values.
let kv (rows: (string * string) list) : string =
    let items = rows |> List.map (fun (k, v) -> $"<div><dt>{k}</dt><dd>{v}</dd></div>") |> String.concat ""
    $"""<dl class="unb-x-kv">{items}</dl>"""

/// A timeline event; When is (iso, shown).
type Event = { State: string; IconName: string option; When: string * string; Title: string; Text: string }

let private ev (state: string) (iconName: string option) (wn: string * string) (title: string) (text: string) : Event =
    { State = state; IconName = iconName; When = wn; Title = title; Text = text }

/// Event timeline, newest first.
let private timeline (label: string) (events: Event list) =
    let li (e: Event) =
        let text = if e.Text <> "" then $"""<p class="unb-x-timeline__text">{e.Text}</p>""" else ""
        $"""<li data-state="{e.State}"><span class="unb-x-timeline__mark" aria-hidden="true">{icon (defaultArg e.IconName (stateIcon e.State))}</span>
<p class="unb-x-timeline__when">{time (fst e.When) (snd e.When)}</p><p class="unb-x-timeline__title">{e.Title}</p>{text}</li>"""
    $"""<ol class="unb-x-timeline" aria-label="{label}">{events |> List.map li |> String.concat ""}</ol>"""

let private ref (n: string) =
    $"""<span class="unb-x-ref">Grant <span class="unb-num">{n}</span><button class="unb-x-copy" type="button" aria-label="Copy grant number {n}">{icon "copy"}</button></span>"""

/// A document; Href "" means not yet available.
type Doc = { Href: string; Title: string; Text: string }

let private docs (items: Doc list) =
    let li (d: Doc) =
        if d.Href <> "" then $"""<li>{icon "file-text"}<a href="{d.Href}">{d.Title}</a><span>{d.Text}</span></li>"""
        else $"""<li data-state="pending">{icon "clock"}<p>{d.Title}</p><span>{d.Text}</span></li>"""
    $"""<ul class="unb-x-docs">{items |> List.map li |> String.concat ""}</ul>"""

let private unavailable (title: string) (text: string) (action: string) =
    let act = if action <> "" then $"<div>{action}</div>" else ""
    $"""<div class="unb-x-unavailable">{icon "lock"}<p class="unb-x-unavailable__title">{title}</p><p>{text}</p>{act}</div>"""

let private d (iso: string) (shown: string) = time iso shown
let private ct (iso: string) (shown: string) = iso, shown
let private ms (label: string) (wn: string) (step: string) : Milestone = { Label = label; When = wn; Step = step }

// ---------- data ----------
type Grant =
    { No: string
      Charity: string
      Place: string
      Ein: string
      Amount: float
      AmountText: string option
      Chip: string
      Next: string
      Track: Milestone list
      Purpose: string
      Recognition: string
      Events: Event list }

let private review =
    { No = "2026-004830"
      Charity = "City Arts Council"
      Place = "Chicago, IL"
      Ein = "00-0000207"
      Amount = 7500.0
      AmountText = None
      Chip = chip "attention" "In review"
      Next = $"""{SPONSOR} usually decides within 2 business days, by {d "2026-12-02" "Dec 2, 2026"}."""
      Track =
        [ ms "Submitted" (d "2026-11-30" "Nov 30, 2026") "done"
          ms "Sponsor review" $"""Decision expected by {d "2026-12-02" "Dec 2"}""" "current"
          ms "Approved" "Not yet" "todo"
          ms "Paid" $"""Expected by {d "2026-12-09" "Dec 9"}""" "todo" ]
      Purpose = "2027 youth theater season"
      Recognition = "Elin Lindqvist"
      Events =
        [ ev "ok" None (ct "2026-11-30T15:13:00-06:00" "Nov 30, 2026, 3:13 PM CT") "Charity confirmed eligible" "City Arts Council is a registered public charity (IRS records of Nov 30, 2026)."
          ev "neutral" (Some "send") (ct "2026-11-30T15:12:00-06:00" "Nov 30, 2026, 3:12 PM CT") "Recommendation submitted by Elin Lindqvist" $"{money 7500.0} moved from Available to grant into grants in progress." ] }

let private approved =
    { No = "2026-004822"
      Charity = "Riverside Food Bank"
      Place = "Springfield, IL"
      Ein = "00-0000103"
      Amount = 5000.0
      AmountText = None
      Chip = chip "ok" "Approved"
      Next = $"""Payment scheduled for {d "2026-12-03" "Dec 3, 2026"} by bank transfer."""
      Track =
        [ ms "Submitted" (d "2026-11-24" "Nov 24, 2026") "done"
          ms "Sponsor review" (d "2026-11-26" "Nov 26, 2026") "done"
          ms "Approved" (d "2026-11-26" "Nov 26, 2026") "done"
          ms "Paid" $"""Scheduled {d "2026-12-03" "Dec 3"}""" "current" ]
      Purpose = "General operating support"
      Recognition = "Elin Lindqvist"
      Events =
        [ ev "info" (Some "calendar") (ct "2026-11-26T09:31:00-06:00" "Nov 26, 2026, 9:31 AM CT") "Payment scheduled for Dec 3, 2026" "By bank transfer to Riverside Food Bank's verified account."
          ev "ok" None (ct "2026-11-26T09:30:00-06:00" "Nov 26, 2026, 9:30 AM CT") $"Approved by {SPONSOR}" ""
          ev "ok" None (ct "2026-11-24T10:05:00-06:00" "Nov 24, 2026, 10:05 AM CT") "Charity confirmed eligible" "Registered public charity (IRS records of Nov 23, 2026)."
          ev "neutral" (Some "send") (ct "2026-11-24T10:02:00-06:00" "Nov 24, 2026, 10:02 AM CT") "Recommendation submitted by Elin Lindqvist" "" ] }

let private paidGrant =
    { No = "2026-004760"
      Charity = "Riverside Food Bank"
      Place = "Springfield, IL"
      Ein = "00-0000103"
      Amount = 2500.0
      AmountText = Some $"""2.5%% of fund value at approval → {money 2500.0}<small>Fixed {d "2026-11-20" "Nov 20, 2026"}, when the fund was worth {money 100000.0}</small>"""
      Chip = chip "ok" "Paid"
      Next = $"""Paid {d "2026-11-30" "Nov 30, 2026"}. Riverside Food Bank acknowledged it on {d "2026-12-01" "Dec 1"}."""
      Track =
        [ ms "Submitted" (d "2026-11-18" "Nov 18, 2026") "done"
          ms "Sponsor review" (d "2026-11-20" "Nov 20, 2026") "done"
          ms "Approved" (d "2026-11-20" "Nov 20, 2026") "done"
          ms "Paid" (d "2026-11-30" "Nov 30, 2026") "done" ]
      Purpose = "Holiday meal program"
      Recognition = "Anonymous"
      Events =
        [ ev "ok" (Some "email-open") (ct "2026-12-01T11:20:00-06:00" "Dec 1, 2026, 11:20 AM CT") "Riverside Food Bank acknowledged the grant" ""
          ev "ok" None (ct "2026-11-30T14:00:00-06:00" "Nov 30, 2026, 2:00 PM CT") $"Paid: {money 2500.0} received by the charity's bank" "Bank transfer to the account ending 2210, confirmed by the receiving bank."
          ev "ok" None (ct "2026-11-20T09:15:00-06:00" "Nov 20, 2026, 9:15 AM CT") $"Approved by {SPONSOR}" $"Amount fixed at {money 2500.0} (2.5%% of {money 100000.0})."
          ev "neutral" (Some "send") (ct "2026-11-18T16:40:00-06:00" "Nov 18, 2026, 4:40 PM CT") "Recommendation submitted by Elin Lindqvist" "" ] }

let private returnedGrant =
    { No = "2026-004790"
      Charity = "Riverside Food Bank"
      Place = "Springfield, IL"
      Ein = "00-0000103"
      Amount = 2500.0
      AmountText = None
      Chip = chip "blocked" "Payment returned"
      Next = $"""Waiting for your choice by {d "2026-12-16" "Dec 16, 2026"}."""
      Track =
        [ ms "Submitted" (d "2026-11-19" "Nov 19, 2026") "done"
          ms "Approved" (d "2026-11-23" "Nov 23, 2026") "done"
          ms "Payment sent" (d "2026-11-25" "Nov 25, 2026") "done"
          ms "Payment returned" (d "2026-12-02" "Dec 2, 2026") "failed" ]
      Purpose = "General operating support"
      Recognition = "Elin Lindqvist"
      Events =
        [ ev "info" (Some "refresh") (ct "2026-12-02T10:16:00-06:00" "Dec 2, 2026, 10:16 AM CT") $"{money 2500.0} returned to Available to grant" ""
          ev "blocked" None (ct "2026-12-02T10:15:00-06:00" "Dec 2, 2026, 10:15 AM CT") "Payment returned by the charity's bank" "The bank account it was sent to has been closed."
          ev "ok" None (ct "2026-11-25T14:00:00-06:00" "Nov 25, 2026, 2:00 PM CT") $"Payment of {money 2500.0} sent by bank transfer" ""
          ev "ok" None (ct "2026-11-23T09:00:00-06:00" "Nov 23, 2026, 9:00 AM CT") $"Approved by {SPONSOR}" ""
          ev "neutral" (Some "send") (ct "2026-11-19T13:22:00-06:00" "Nov 19, 2026, 1:22 PM CT") "Recommendation submitted by Elin Lindqvist" "" ] }

let private holdGrant =
    { No = "2026-004811"
      Charity = "Northgate Literacy Project"
      Place = "Gary, IN"
      Ein = "00-0000311"
      Amount = 2500.0
      AmountText = None
      Chip = chip "blocked" "On hold: charity eligibility"
      Next = $"""We expect to finish checking by {d "2026-12-04" "Dec 4, 2026"}."""
      Track =
        [ ms "Submitted" (d "2026-11-25" "Nov 25, 2026") "done"
          ms "Charity check" $"""On hold since {d "2026-11-27" "Nov 27"}""" "failed"
          ms "Approved" "Not yet" "todo"
          ms "Paid" "Not yet" "todo" ]
      Purpose = "Summer reading program"
      Recognition = "Elin Lindqvist"
      Events =
        [ ev "blocked" None (ct "2026-11-27T08:40:00-06:00" "Nov 27, 2026, 8:40 AM CT") "On hold: we couldn't confirm the charity's tax-exempt status" "Northgate Literacy Project no longer appears in the latest IRS list of public charities. This is often a filing delay."
          ev "neutral" (Some "send") (ct "2026-11-25T19:05:00-06:00" "Nov 25, 2026, 7:05 PM CT") "Recommendation submitted by Elin Lindqvist" "" ] }

// ---------- regions ----------
let private header (g: Grant) =
    $"""<header class="unb-x-record">
<nav class="unb-x-record__crumbs" aria-label="Breadcrumb"><a href="#grants">Grants</a><span aria-hidden="true">/</span>{ref g.No}</nav>
<div class="unb-x-record__row"><div class="unb-x-record__who"><h1>{g.Charity}</h1><p class="unb-small unb-muted">{g.Place} · From {FUND}</p></div>
<p class="unb-x-record__amount"><span class="forma-visually-hidden">Amount: </span>{moneyHero g.Amount}</p></div>
<div class="unb-x-record__status">{g.Chip}<p class="unb-x-record__next">{g.Next}</p></div>
</header>"""

let private progress (g: Grant) (foot: string) =
    panel { P with Id = "prog"; Title = "Progress"; Body = track $"Progress of grant {g.No}" g.Track; Foot = foot }

let private details (g: Grant) =
    let recognitionNote =
        if g.Recognition = "Anonymous" then "The charity sees “Anonymous donor via " + SPONSOR + "”" else "Name shown to the charity"
    panel
        { P with
            Id = "det"
            Title = "Grant details"
            Body =
                kv
                    [ "Amount", defaultArg g.AmountText (money g.Amount)
                      "Charity", $"""{g.Charity}<small>{g.Place} · EIN <span class="unb-num">{g.Ein}</span></small>"""
                      "Purpose", g.Purpose
                      "Recognition", $"{g.Recognition}<small>{recognitionNote}</small>"
                      "From", FUND ] }

let private history (g: Grant) =
    panel
        { P with
            Id = "hist"
            Title = "History"
            Meta = """<span class="unb-small unb-muted">Newest first · Central Time</span>"""
            Body = timeline $"History of grant {g.No}" g.Events }

let private documents (items: Doc list) = panel { P with Id = "docs"; Title = "Documents"; Body = docs items }

let private docConfirm (g: Grant) (date: string) : Doc =
    { Href = $"#doc-confirm-{g.No}"; Title = "Grant confirmation (PDF)"; Text = $"Issued {date}" }

let private docRemitPending : Doc = { Href = ""; Title = "Payment remittance"; Text = "Available once the grant is paid" }

let private layout (main: string) (side: string) =
    $"""<div class="unb-x-flow" data-layout="record"><div>{main}</div><div>{side}</div></div>"""

let private pageOf (g: Grant) (user: User option) (main: string) (side: string) : Page =
    let b = { page with Nav = "grants"; Audience = "donor"; User = DONOR; Brand = "unbound-default"; Notifications = 2 }
    let b = match user with Some u -> { b with User = u; Notifications = 0 } | None -> b
    { b with Title = $"{g.Charity}, grant {g.No}"; Pagehead = header g; Body = layout main side }

// Withdraw is offered only while the sponsor has not approved (A2: confirmation dialog).
let private withdrawFoot () =
    """<p class="unb-small unb-muted">You can withdraw this recommendation until it's approved.</p>"""
    + buttonWith { B with Id = "withdraw" } "Withdraw recommendation"

let private withdrawDialog (g: Grant) =
    $"""<dialog class="forma-dialog" id="withdraw-dialog" aria-labelledby="wd-title" aria-describedby="wd-desc">
<form method="dialog">
<div class="forma-dialog__body">
<h2 id="wd-title">Withdraw your {money g.Amount} grant to {g.Charity}?</h2>
<p id="wd-desc">The recommendation closes now and the money goes back to your fund.</p>
<dl class="unb-x-consequences">
<div>{icon "refresh"}<dt>What changes</dt><dd>{money g.Amount} returns to Available to grant right away. {g.Charity} hasn't been told about this grant, so nothing is sent to them.</dd></div>
<div>{icon "warning"}<dt>Can it be undone?</dt><dd>No. You can recommend a new grant to {g.Charity} at any time.</dd></div>
<div>{icon "users"}<dt>Who acts next</dt><dd>No one. {SPONSOR} stops reviewing it.</dd></div>
</dl>
</div>
<div class="forma-dialog__actions"><button class="forma-button" type="submit" value="keep" autofocus>Keep the grant</button><button class="forma-button" type="submit" value="withdraw" data-unb-x-tone="danger">Withdraw recommendation</button></div>
</form>
</dialog>
<script>document.getElementById("withdraw-dialog").showModal();</script>"""

let private approvedFoot =
    unavailable
        "Withdraw is no longer available"
        $"""{SPONSOR} approved this grant on {d "2026-11-26" "Nov 26, 2026"}. You can still ask them to cancel it before {d "2026-12-03" "Dec 3"}, but payment may already be in progress and can't always be stopped."""
        (quiet "Request cancellation" "#request-cancellation" "chevron-right")

// ---------- states ----------
let private inReview (extra: string) =
    pageOf
        review
        None
        (progress review (withdrawFoot ()) + history review + extra)
        (details review + documents [ docConfirm review (d "2026-11-30" "Nov 30, 2026"); docRemitPending ])

let private approvedPage (who: User option) =
    let foot =
        match who with
        | Some _ ->
            unavailable
                "View only"
                $"""Elin Lindqvist shared this fund with you. Only the fund's advisors can withdraw, cancel or change grants. Your access ends {d "2027-06-30" "Jun 30, 2027"}."""
                ""
        | None -> approvedFoot
    pageOf
        approved
        who
        (progress approved foot + history approved)
        (details approved + documents [ docConfirm approved (d "2026-11-26" "Nov 26, 2026"); docRemitPending ])

let private paid =
    let proof =
        panel
            { P with
                Id = "proof"
                Title = "Proof of payment"
                Body =
                    kv
                        [ "Paid on", $"""{d "2026-11-30" "Nov 30, 2026"}<small>Confirmed by the charity's bank at 2:00 PM CT</small>"""
                          "Paid by", """Bank transfer<small>To Riverside Food Bank's verified account ending <span class="unb-num">2210</span></small>"""
                          "Acknowledgment", $"""Received {d "2026-12-01" "Dec 1, 2026"}<small>Riverside Food Bank confirmed it received the grant</small>""" ]
                Foot = """<span class="unb-small unb-muted">This grant isn't tax-deductible for you; your deduction was for the original contribution to the fund.</span>""" }
    pageOf
        paidGrant
        None
        (progress paidGrant "" + proof + history paidGrant)
        (details paidGrant
         + documents
             [ { Href = "#doc-remit-2026-004760"; Title = "Payment remittance (PDF)"; Text = $"""Sent to the charity {d "2026-11-30" "Nov 30, 2026"}""" }
               { Href = "#doc-ack-2026-004760"; Title = "Charity acknowledgment (PDF)"; Text = $"""From Riverside Food Bank, {d "2026-12-01" "Dec 1, 2026"}""" }
               docConfirm paidGrant (d "2026-11-20" "Nov 20, 2026") ])

let private returned =
    let choose =
        panel
            { P with
                Id = "choose"
                Title = "The charity's bank returned this payment"
                Tone = "attention"
                Body =
                    $"""<p>On {d "2026-12-02" "Dec 2, 2026"} Riverside Food Bank's bank sent the {money 2500.0} back because the account had been closed. The money is back in your Available to grant. Choose what should happen next.</p>
<ul class="unb-x-options">
<li class="unb-x-option"><h3>Wait for new bank details</h3><p>We ask Riverside Food Bank for a new account and pay it once it's verified, usually within 10 business days. {money 2500.0} is set aside for this grant again until then. You can change your mind while you wait.</p>{link "#wait" "Wait for new details"}</li>
<li class="unb-x-option"><h3>Send it to a different charity</h3><p>This grant closes and {money 2500.0} stays in Available to grant. You then choose a charity in a new recommendation, which {SPONSOR} reviews as usual.</p>{link "#redirect" "Choose a different charity"}</li>
</ul>
<p class="unb-small unb-muted">If you don't choose by {d "2026-12-16" "Dec 16, 2026"}, we'll wait for new bank details.</p>""" }
    pageOf
        returnedGrant
        None
        (choose + progress returnedGrant "" + history returnedGrant)
        (details returnedGrant
         + documents
             [ docConfirm returnedGrant (d "2026-11-23" "Nov 23, 2026")
               { Href = "#doc-return-2026-004790"; Title = "Return notice (PDF)"; Text = $"""From the charity's bank, {d "2026-12-02" "Dec 2, 2026"}""" } ])

let private hold =
    let n =
        notice
            { N with
                State = "blocked"
                IconName = Some "pending"
                Title = "On hold while we check the charity"
                Body =
                    $"""<p>On {d "2026-11-27" "Nov 27, 2026"} we couldn't confirm that Northgate Literacy Project is still a tax-exempt public charity. {SPONSOR} is checking with the charity and IRS records. Nothing has been paid, and you don't need to do anything to wait.</p>
<p>If it's confirmed, the grant continues to approval. If it can't be confirmed by {d "2026-12-11" "Dec 11, 2026"}, the grant is declined and {money 2500.0} returns to Available to grant.</p>""" }
    let foot =
        """<p class="unb-small unb-muted">Rather not wait? Withdraw it and choose another charity.</p>"""
        + buttonWith { B with Id = "withdraw" } "Withdraw recommendation"
    pageOf holdGrant None (n + progress holdGrant foot + history holdGrant) (details holdGrant + documents [ docRemitPending ])

// The JavaScript also set docTitle "Withdraw confirmation" here; the build always replaces docTitle.
let private withdrawConfirm = inReview (withdrawDialog review)

let states : State list =
    [ app "in-review" "In review: progress with expected dates, withdraw available" "Donor (primary advisor)" [ "pending-grants"; "normal" ] (inReview "")
      app "withdraw-confirm" "Withdraw: consequential confirmation dialog" "Donor (primary advisor)" [ "withdraw"; "confirmation" ] withdrawConfirm
      app "approved" "Approved, payment scheduled; withdraw unavailable with reason" "Donor (primary advisor)" [ "approved"; "action-unavailable" ] (approvedPage None)
      app "paid" "Paid: proof of payment and charity acknowledgment (percentage grant)" "Donor (primary advisor)" [ "paid"; "percentage-grant" ] paid
      app "payment-returned" "Failed payment: returned by the charity's bank, wait or redirect" "Donor (primary advisor)" [ "failed-payment"; "attention-required" ] returned
      app "on-hold" "On hold: charity eligibility" "Donor (primary advisor)" [ "eligibility-hold"; "attention-required" ] hold
      app "advisor-view" "Read-only: financial advisor with view access" "Financial advisor (view only)" [ "read-only" ] (approvedPage (Some ADVISOR)) ]

let screen : Screen =
    { Id = "S-004"
      Slug = "S-004-grant-detail"
      Title = "Grant detail"
      Requirement = "UNB-UI-S-004"
      States =
        states
        |> List.map (fun s ->
            match s.Content with
            | App p -> { s with Content = App { p with ExtraCss = Some [ "proposed/donor-grants.css" ] } }
            | Html _ -> s) }

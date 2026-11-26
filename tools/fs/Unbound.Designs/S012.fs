/// S-012 Charity view, v2. One page per state; port of v2/tools/screens/S-012.mjs.
/// Data first, then pure renderers. Synthetic data only.
module Unbound.Designs.S012

open Unbound.Designs.Ui
open Unbound.Designs.Screen
open Unbound.Designs.S008

let private USER : User = { Name = "Jamie Cole"; Initials = "JC"; Role = Some "Riverside Food Bank" }
let private SPONSOR = "Example Community Foundation"
let private ORG_SUB = "Riverside Food Bank · Springfield, IL"

/// A grant received; Ack "" is absent.
type Record = { Id: string; From: string; By: string; Amount: float; Status: string; When: string; Ack: string }

let private GRANTS =
    [ { Id = "g-004822"; From = "Lindqvist Family Fund"; By = "Recommended by Elin Lindqvist"; Amount = 5000.0; Status = chip "ok" "Approved"; When = $"""Payment scheduled {time "2026-12-03" "Dec 3, 2026"}"""; Ack = "" }
      { Id = "g-004790"; From = "Lindqvist Family Fund"; By = "Recommended by Elin Lindqvist"; Amount = 2500.0; Status = chip "ok" "Paid"; When = $"""Paid {time "2026-11-30" "Nov 30, 2026"}"""; Ack = chip "attention" "Not acknowledged" }
      { Id = "g-004702"; From = $"Anonymous donor via {SPONSOR}"; By = "The donor chose not to be named"; Amount = 10000.0; Status = chip "ok" "Paid"; When = $"""Paid {time "2026-11-20" "Nov 20, 2026"}"""; Ack = chip "neutral" "Acknowledged Nov 21" } ]

let private records (list: Record list) =
    let li (g: Record) =
        $"""<li class="unb-x-record"><span class="unb-x-record__title"><a href="#{g.Id}">{g.From}</a><br><span class="unb-small unb-muted">{g.By}</span></span>{money g.Amount}
<span class="unb-x-record__meta">{g.Status}{g.Ack}<span>{g.When}</span></span></li>"""
    $"""<ul class="unb-x-records">{list |> List.map li |> String.concat ""}</ul>"""

let private baseP =
    { page with
        Title = "Grants received"
        Nav = "received"
        Audience = "charity"
        User = USER
        Brand = "unbound-default"
        Notifications = 1
        ExtraCss = Some CSS }

let private listState =
    let todo =
        panel
            { P with
                Id = "todo"
                Title = "To do"
                Tone = "attention"
                Body =
                    tasks
                        [ { State = "attention"
                            IconName = Some "email-open"
                            Title = $"Acknowledge {money 2500.0} from the Lindqvist Family Fund"
                            Text = $"""Paid {time "2026-11-30" "Nov 30, 2026"}. Acknowledging lets {SPONSOR} and the donor know it arrived."""
                            Actions = [ link "#g-004790" "Review and acknowledge" ] } ] }
    let list =
        panel
            { P with
                Id = "list"
                Title = "Grants"
                Meta = $"""<span class="unb-small unb-muted">{money 17500.0} in 2026 · 3 grants</span>"""
                Body = records GRANTS
                Foot = $"""{quiet "Download 2026 remittances" "#remit" "download"}<span class="unb-small unb-muted">{SPONSOR} is the legal donor of every grant.</span>""" }
    let pay =
        panel { P with Id = "pay"; Title = "Where we pay you"; Body = dl [ "ACH to Prairie Community Bank ••••7710", chip "ok" "Verified" ]; Foot = quiet "Change bank details" "#bank" "chevron-right" }
    { baseP with
        Pagehead = pagehead { H with Title = "Grants received"; Sub = ORG_SUB }
        Body = $"{todo}\n{list}\n{pay}" }

let private detailState =
    let track = steps [ "Approved", "done"; "Paid", "done"; "Your acknowledgment", "current" ]
    let summary =
        panel
            { P with
                Id = "sum"
                Label = "Grant summary"
                Body =
                    $"""<div class="unb-cluster">{chip "ok" "Paid"}<span class="unb-small unb-muted">Paid {time "2026-11-30" "Nov 30, 2026"} to ACH ••••7710</span></div>
<p class="unb-x-amount">{moneyHero 2500.0}</p>
{track}""" }
    let ack =
        panel
            { P with
                Id = "ack"
                Title = "Acknowledge this grant"
                Body =
                    $"""<p class="unb-small">This tells {SPONSOR} and Elin Lindqvist that the grant arrived. It is not a tax receipt: {SPONSOR} is the legal donor and has already received what it needs.</p>
<div class="unb-x-field"><label for="ack-msg">Message to the donor (optional)</label><textarea id="ack-msg" aria-describedby="ack-hint"></textarea><span class="unb-x-field__hint" id="ack-hint">Shared with Elin Lindqvist through {SPONSOR}. Don't include goods or services offered in return.</span></div>
<label class="unb-x-check" for="ack-use"><input type="checkbox" id="ack-use" checked><span>We'll use this grant for general operating support, as recommended.</span></label>
<div class="unb-x-actions">{primary "Acknowledge receipt"}</div>""" }
    let factsP =
        panel
            { P with
                Id = "facts"
                Title = "Grant details"
                Body =
                    facts
                        [ "Legal donor", $"""{SPONSOR}<br><span class="unb-small unb-muted">EIN <span class="unb-num">00-0000001</span></span>"""
                          "Recommended by", "Elin Lindqvist, Lindqvist Family Fund"
                          "Purpose", "General operating support"
                          "Approved", time "2026-11-26" "Nov 26, 2026"
                          "Paid", time "2026-11-30" "Nov 30, 2026"
                          "On your bank statement", """<span class="unb-x-mono">ECF GRANT 2026-004790</span>""" ]
                Foot = quiet "Download remittance (PDF)" "#remit" "download" }
    { baseP with
        Notifications = 0
        Pagehead =
            pagehead
                { H with
                    Context = [ """<a href="#received">Grants received</a>"""; "Lindqvist Family Fund" ]
                    Title = "Grant from the Lindqvist Family Fund"
                    Sub = ORG_SUB }
        Body =
            $"""<div class="unb-x-detail"><div class="unb-x-detail__main">
{summary}
{ack}
</div><div class="unb-x-detail__side">
{factsP}
</div></div>""" }

let private reverifyState =
    let n =
        notice
            { N with
                State = "blocked"
                IconName = Some "refresh"
                Title = "Your bank returned a payment. Please confirm your bank details"
                Body =
                    $"""<p>On {time "2026-12-02" "Dec 2, 2026"} Prairie Community Bank returned our {money 2500.0} payment with the reason “account closed”. To protect your grants we've paused payments to account ••••7710 until new details are verified.</p>"""
                Actions = [ primaryLink "#bank-new" "Update bank details" ] }
    let waiting =
        panel
            { P with
                Id = "waiting"
                Title = "Grants waiting for verified bank details"
                Meta = $"""<span class="unb-small unb-muted">{money 7500.0} in total</span>"""
                Body =
                    records
                        [ { Id = "g-004790"; From = "Lindqvist Family Fund"; By = "Returned by your bank"; Amount = 2500.0; Status = chip "blocked" "Payment returned"; When = "Will be sent again once verified"; Ack = "" }
                          { Id = "g-004822"; From = "Lindqvist Family Fund"; By = "Recommended by Elin Lindqvist"; Amount = 5000.0; Status = chip "attention" "Waiting for bank details"; When = $"""Was scheduled for {time "2026-12-03" "Dec 3, 2026"}"""; Ack = "" } ]
                Foot = """<span class="unb-small unb-muted">Nothing is lost. The donor's fund keeps the money set aside for you while we verify, for up to 60 days.</span>""" }
    let how =
        panel
            { P with
                Id = "how"
                Title = "How verification works"
                Body =
                    $"""<ol class="unb-checklist">
<li data-step="current" aria-current="step"><p class="unb-checklist__title">You send new bank details</p><p class="unb-small unb-muted">With a bank letter or voided check. About 5 minutes.</p></li>
<li><p class="unb-checklist__title">We call you to confirm</p><p class="unb-small unb-muted">{SPONSOR} calls the phone number on your IRS record, never a number from the request.</p></li>
<li><p class="unb-checklist__title">Payments resume</p><p class="unb-small unb-muted">Usually within 3 business days of your request.</p></li>
</ol>""" }
    let pay = panel { P with Id = "pay"; Title = "Where we pay you"; Body = dl [ "ACH to Prairie Community Bank ••••7710", chip "blocked" "Paused: payment returned" ] }
    { baseP with
        Notifications = 2
        Pagehead = pagehead { H with Title = "Grants received"; Sub = ORG_SUB }
        Body =
            $"""{n}
<div class="unb-x-detail"><div class="unb-x-detail__main">
{waiting}
</div><div class="unb-x-detail__side">
{how}
{pay}
</div></div>""" }

let states : State list =
    [ app "grants-received" "Grants received, one waiting for acknowledgment" "Charity finance user" [ "normal"; "attention-required"; "anonymous-donor" ] listState
      app "grant-detail" "One grant's detail with acknowledgment" "Charity finance user" [ "normal"; "acknowledgment" ] { detailState with Title = "Grant detail" }
      app "bank-reverify" "Bank details need re-verification after a returned payment" "Charity finance user" [ "failed-payment"; "restricted"; "attention-required" ] reverifyState ]

let screen : Screen = { Id = "S-012"; Slug = "S-012-charity-view"; Title = "Charity view"; Requirement = "UNB-UI-S-012"; States = states }

/// S-003 Grant recommendation, v2. One page per state; port of v2/tools/screens/S-003.mjs.
/// Design decision: one page with progressive disclosure (charity, then amount and
/// details) followed by ONE review-and-confirm page, instead of v1's four-step wizard
/// plus a confirmation dialog. See the summary in the build report for why.
/// Data first, then pure renderers. Synthetic data only (same fund as S-001).
module Unbound.Designs.S003

open Unbound.Designs.JsCompat
open Unbound.Designs.Ui
open Unbound.Designs.Screen
open Unbound.Designs.S004

let private DONOR : User = { Name = "Elin Lindqvist"; Initials = "EL"; Role = None }
let private SPONSOR = "Example Community Foundation"
let private FUND = "Lindqvist Family Fund"
let private AVAILABLE = 100000.0
let private UPDATED = time "2026-12-01T16:58:00-06:00" "today at 4:58 PM CT"

// ---------- local form helpers (Forma field / choice / checkbox markup) ----------

/// aria-describedby over the non-empty ids.
let private describedBy (ids: string list) =
    match ids |> List.filter ((<>) "") with
    | [] -> ""
    | list -> " aria-describedby=\"" + String.concat " " list + "\""

let private vmsg (id: string) (text: string) =
    $"""<p class="forma-validation-message" id="{id}"><span class="forma-validation-message__mark" aria-hidden="true">!</span><span>{text}</span></p>"""

let private orEmpty (c: bool) (s: string) = if c then s else ""

/// Field arguments; "" stands for an absent desc or error.
type Field = { Id: string; Label: string; Desc: string; Value: string; Error: string; Type: string; Attrs: string }

let private F : Field = { Id = "undefined"; Label = "undefined"; Desc = ""; Value = ""; Error = ""; Type = "text"; Attrs = "" }

let private invalid (f: Field) = orEmpty (f.Error <> "") " aria-invalid=\"true\""
let private errorMsg (f: Field) = orEmpty (f.Error <> "") (vmsg $"{f.Id}-e" f.Error)

let private moneyField (f: Field) =
    $"""<div class="forma-field">
<label class="forma-field__label" for="{f.Id}">{f.Label}</label>
<span class="forma-field__description" id="{f.Id}-d">{f.Desc}</span>
<div class="unb-x-money-input"><span aria-hidden="true">$</span><input id="{f.Id}" name="{f.Id}" type="text" inputmode="decimal" autocomplete="off" value="{f.Value}" required{describedBy [ $"{f.Id}-d"; orEmpty (f.Error <> "") $"{f.Id}-e" ]}{invalid f}></div>
{errorMsg f}</div>"""

let private textField (f: Field) =
    let desc = orEmpty (f.Desc <> "") $"""<span class="forma-field__description" id="{f.Id}-d">{f.Desc}</span>"""
    $"""<div class="forma-field">
<label class="forma-field__label" for="{f.Id}">{f.Label}</label>
{desc}
<input id="{f.Id}" name="{f.Id}" type="{f.Type}" value="{f.Value}"{f.Attrs}{describedBy [ orEmpty (f.Desc <> "") $"{f.Id}-d"; orEmpty (f.Error <> "") $"{f.Id}-e" ]}{invalid f}>
{errorMsg f}</div>"""

let private textarea (f: Field) =
    $"""<div class="forma-field">
<label class="forma-field__label" for="{f.Id}">{f.Label}</label>
<span class="forma-field__description" id="{f.Id}-d">{f.Desc}</span>
<textarea id="{f.Id}" name="{f.Id}" rows="2" maxlength="200" required{describedBy [ $"{f.Id}-d"; orEmpty (f.Error <> "") $"{f.Id}-e" ]}{invalid f}>{f.Value}</textarea>
{errorMsg f}</div>"""

/// choices: (value, title, text, checked)
let private choices (name: string) (legend: string) (items: (string * string * string * bool) list) =
    let item (v: string, t: string, s: string, c: bool) =
        $"""<label class="forma-choice"><input type="radio" name="{name}" value="{v}"{orEmpty c " checked"}><span class="forma-choice__content"><strong>{t}</strong>{orEmpty (s <> "") $"<span>{s}</span>"}</span></label>"""
    $"""<fieldset class="forma-choice-group">
<legend class="forma-choice-group__legend">{legend}</legend>
{items |> List.map item |> String.concat ""}
</fieldset>"""

let private checkbox (id: string) (label: string) (isChecked: bool) =
    $"""<label class="forma-checkbox"><input class="forma-checkbox__input" type="checkbox" id="{id}" name="{id}" value="yes" required{orEmpty isChecked " checked"}><span class="forma-checkbox__box" aria-hidden="true"></span><span class="forma-checkbox__text"><span class="forma-checkbox__label">{label}</span></span></label>"""

let private select (id: string) (label: string) (options: (string * string * bool) list) =
    let opt (v: string, l: string, s: bool) = $"""<option value="{v}"{orEmpty s " selected"}>{l}</option>"""
    $"""<label class="forma-select-field"><span class="forma-field__label">{label}</span><span class="forma-select"><select class="forma-select__input" id="{id}" name="{id}">{options |> List.map opt |> String.concat ""}</select><span class="forma-select__indicator" aria-hidden="true"></span></span></label>"""

let private errorSummary (items: (string * string) list) =
    let list = items |> List.map (fun (href, text) -> $"""<li><a href="{href}">{text}</a></li>""") |> String.concat ""
    let count = if items.Length = 1 then "is 1 problem" else $"are {items.Length} problems"
    $"""<div class="forma-validation-summary" role="alert" tabindex="-1" aria-labelledby="err-t">
<div class="forma-validation-summary__icon" aria-hidden="true">!</div>
<div><h2 class="forma-validation-summary__title" id="err-t">There {count} to fix before you review</h2>
<ul class="forma-validation-summary__list">{list}</ul></div></div>"""

// ---------- data ----------
let private CHECKED = time "2026-11-30" "Nov 30, 2026"

/// A search result; Chip and Why are shown when not Ok.
type Charity = { Name: string; Place: string; Ein: string; Ok: bool; Chip: string; Why: string }

let private RESULTS =
    [ { Name = "Riverside Food Bank"; Place = "Springfield, IL"; Ein = "00-0000103"; Ok = true; Chip = "undefined"; Why = "undefined" }
      { Name = "Riverside Food Bank of Central Ohio"; Place = "Columbus, OH"; Ein = "00-0000418"; Ok = true; Chip = "undefined"; Why = "undefined" }
      { Name = "Riverside Community Food Pantry"; Place = "Peoria, IL"; Ein = "00-0000522"; Ok = false; Chip = chip "blocked" "Not eligible"; Why = "It isn't registered as a public charity, so it can't receive grants from a donor-advised fund." }
      { Name = "Riverside Hunger Relief"; Place = "Decatur, IL"; Ein = "00-0000609"; Ok = false; Chip = chip "blocked" "On hold"; Why = "It can't receive grants while its status is being checked. Try again in a few days." } ]

let private CHARITY = RESULTS.[0]
let private HELD = { Name = "Northgate Literacy Project"; Place = "Gary, IN"; Ein = "00-0000311"; Ok = false; Chip = "undefined"; Why = "undefined" }

let private AFFIRM =
    [ "I and my family will receive no goods, services or other benefit in return for this grant, including tickets, dinners or memberships."
      "This grant will not be used to pay for any part of an event, membership or purchase that benefits me." ]

// ---------- regions ----------
let private headWith (title: string) (sub: string option) =
    pagehead
        { H with
            Context = [ """<a href="#grants">Grants</a>"""; "New recommendation" ]
            Title = title
            Sub = defaultArg sub $"From {FUND} · {money AVAILABLE} available to grant" }

let private head () = headWith "Recommend a grant" None

let private result (r: Charity) =
    let elig =
        if r.Ok then $"""{chip "ok" "Eligible"}<span>Can receive grants. Checked {CHECKED}.</span>"""
        else $"{r.Chip}<span>{r.Why}</span>"
    let action =
        orEmpty r.Ok $"""<div class="unb-x-result__action"><button class="forma-button" type="button">Select<span class="forma-visually-hidden"> {r.Name}, {r.Place}</span></button></div>"""
    $"""<li class="unb-x-result">
<div><p class="unb-x-result__name">{r.Name}</p><p class="unb-small unb-muted">{r.Place} · EIN <span class="unb-num">{r.Ein}</span></p></div>
<p class="unb-x-result__elig">{elig}</p>
{action}
</li>"""

let private searchPanel () =
    panel
        { P with
            Id = "charity"
            Title = "Charity"
            Body =
                $"""<div class="forma-search" role="search" aria-label="Charities">
<label class="forma-search__label" for="q">Search by charity name, city or EIN</label>
<div class="forma-search__control"><input id="q" type="search" value="Riverside food" autocomplete="off"><button class="forma-button" type="button">Search</button></div>
<p class="forma-search__status" role="status">4 charities found. Eligibility is checked against IRS records.</p></div>
<ul class="unb-x-results" aria-label="Search results">{RESULTS |> List.map result |> String.concat ""}</ul>"""
            Foot = quiet "Can't find the charity? Ask us to add it" "#request-charity" "chevron-right" }

let private pickedCharity (c: Charity) (held: bool) =
    panel
        { P with
            Id = "charity"
            Title = "Charity"
            Body =
                if held then
                    $"""<div class="unb-x-picked" data-state="blocked" id="charity-held"><span class="unb-task__icon unb-task__icon--neutral">{icon "globe"}</span>
<p class="unb-x-result__name">{c.Name}</p><p class="unb-small unb-muted">{c.Place} · EIN <span class="unb-num">{c.Ein}</span></p>
<p class="unb-x-result__elig">{chip "blocked" "On hold"}<span>Since {time "2026-11-27" "Nov 27, 2026"} it can't receive grants while its tax-exempt status is checked.</span></p>
<div class="unb-task__action">{link "#search" "Choose another charity"}{quiet "Keep as a draft until it's confirmed" "#save" ""}</div></div>"""
                else
                    $"""<div class="unb-x-picked"><span class="unb-task__icon unb-task__icon--neutral">{icon "globe"}</span>
<p class="unb-x-result__name">{c.Name}</p><p class="unb-small unb-muted">{c.Place} · EIN <span class="unb-num">{c.Ein}</span></p>
<p class="unb-x-result__elig">{chip "ok" "Eligible"}<span>Can receive grants. Checked {CHECKED}.</span></p>
<div class="unb-task__action">{quiet "Change<span class=\"forma-visually-hidden\"> charity</span>" "#search" ""}</div></div>""" }

let private lockedDetails () =
    panel { P with Id = "details"; Title = "Amount and details"; Body = """<p class="unb-muted">Choose a charity first. You'll then enter the amount and purpose here.</p>""" }

let private moreOptions (isOpen: bool) =
    let pctField = textField { F with Id = "pct"; Label = "Percentage of fund value"; Desc = "For example 5 for 5%."; Value = "5"; Attrs = " inputmode=\"decimal\"" }
    let fix = select "fix" "Fix the dollar amount" [ "approval", "When the grant is approved", true; "payment", "On the payment date", false ]
    let basis = choices "basis" "How do you want to set the amount?" [ "fixed", "A fixed dollar amount", "", not isOpen; "percent", "A percentage of fund value", "The dollar amount is fixed on the date you choose below.", isOpen ]
    let repeat = choices "repeat" "How often?" [ "once", "One time", "", true; "quarterly", "Every 3 months", "Each payment is checked against Available to grant first.", false; "yearly", "Once a year", "", false ]
    let opened =
        orEmpty
            isOpen
            ($"""{pctField}
{fix}
<p class="unb-x-derived" id="pct-est">At today's fund value of {money 120000.0} (market close {time "2026-11-30" "Nov 30, 2026"}), 5%% would be <strong>{money 6000.0}</strong>. The exact amount is fixed at approval and must not be more than your Available to grant.</p>""")
    $"""<details class="forma-disclosure"{orEmpty isOpen " open"}>
<summary>Give a percentage of the fund or repeat this grant</summary>
<div class="forma-disclosure__content">
{basis}
{opened}
{repeat}
</div></details>"""

/// detailsForm arguments; errors "" are absent, Action None is the default pair of actions.
type DetailsForm = { Amount: string; Purpose: string; AmountError: string; PurposeError: string; Open: bool; Action: string option }

let private DF = { Amount = "5,000.00"; Purpose = "General operating support"; AmountError = ""; PurposeError = ""; Open = false; Action = None }

let private detailsForm (o: DetailsForm) =
    let affirm = AFFIRM |> List.mapi (fun i t -> checkbox $"aff-{i + 1}" t true) |> String.concat ""
    let action = defaultArg o.Action (primary "Review grant" + quiet "Save draft" "#save" "save")
    let amount =
        moneyField { F with Id = "amount"; Label = "Amount in US dollars"; Desc = $"You have {money AVAILABLE} available to grant. The minimum grant is {money 50.0}."; Value = (if o.Open then "" else o.Amount); Error = o.AmountError }
    let purpose =
        textarea { F with Id = "purpose"; Label = "Purpose"; Desc = $"{SPONSOR} requires a purpose. It's shown to the charity, for example “General operating support”."; Value = o.Purpose; Error = o.PurposeError }
    let recognition =
        choices "recognition" "How should the charity see your name?" [ "name", "Elin Lindqvist", "Your name as the recommending advisor", true; "fund", FUND, "The fund's name only", false; "anon", "Anonymous", $"The charity sees “Anonymous donor via {SPONSOR}”", false ]
    panel
        { P with
            Id = "details"
            Title = "Amount and details"
            Body =
                $"""<form class="unb-x-form" id="grant-form" novalidate aria-labelledby="details-t">
{amount}
{moreOptions o.Open}
{purpose}
{recognition}
<fieldset><legend class="forma-choice-group__legend">{SPONSOR} asks you to confirm</legend>
<p class="unb-x-fieldset-note">Grants from a donor-advised fund can't give you anything in return.</p>
{affirm}</fieldset>
<fieldset class="forma-binary-choice" aria-describedby="pledge-d"><legend class="forma-binary-choice__legend">Does this grant pay a pledge you made personally?</legend>
<p class="unb-x-fieldset-note" id="pledge-d">{SPONSOR} can't use grants to pay personal pledges.</p>
<div class="forma-binary-choice__options"><label class="forma-binary-option"><input type="radio" name="pledge" value="no" checked required><span>No</span></label><label class="forma-binary-option"><input type="radio" name="pledge" value="yes"><span>Yes</span></label></div></fieldset>
<div class="unb-x-actions">{action}</div>
</form>""" }

let private summaryAside (amount: float) (note: bool) =
    let rows =
        [ "Available to grant", money AVAILABLE ]
        @ (if amount <> 0.0 then [ "This grant", money -amount; "Left after this grant", $"<strong>{money (AVAILABLE - amount)}</strong>" ] else [])
    panel
        { P with
            Id = "sum"
            Title = "Your fund"
            Body = $"""{dl rows}<p class="unb-small unb-muted">Updated {UPDATED}.</p>"""
            Foot = orEmpty note $"""<p class="unb-small unb-muted">{SPONSOR} reviews every recommendation, usually within 2 business days. You can withdraw it until it's approved.</p>""" }

let private flow (main: string) (aside: string) =
    $"""<div class="unb-x-flow"><div>{main}</div>{orEmpty (aside <> "") $"<div>{aside}</div>"}</div>"""

let private baseP =
    { page with
        Title = "Recommend a grant"
        Nav = "grants"
        Audience = "donor"
        User = DONOR
        Brand = "unbound-default"
        Notifications = 2
        ExtraCss = Some [ "proposed/donor-grants.css" ] }

// ---------- states ----------
let private choose =
    { baseP with Pagehead = head (); Body = flow (searchPanel () + lockedDetails ()) (summaryAside 0.0 true) }

let private sponsorOf (brand: string) = if brand = "example-harbor" then "Example Charitable Foundation" else SPONSOR

let private details (brand: string) =
    { baseP with
        Brand = brand
        Pagehead = head ()
        Body = flow (pickedCharity CHARITY false + detailsForm DF) (summaryAside 5000.0 true) |> replaceAll SPONSOR (sponsorOf brand) }

let private errors =
    { baseP with
        Pagehead = head ()
        Body =
            errorSummary
                [ "#charity-held", "Northgate Literacy Project can't receive grants right now. Choose another charity or keep this as a draft"
                  "#amount", $"Enter {money AVAILABLE} or less, your Available to grant"
                  "#purpose", "Enter a purpose for this grant" ]
            + "\n"
            + flow
                (pickedCharity HELD true
                 + detailsForm
                     { DF with
                         Amount = "150,000.00"
                         Purpose = ""
                         AmountError = $"Enter {money AVAILABLE} or less. That's your Available to grant today."
                         PurposeError = $"Enter a purpose. {SPONSOR} requires one for every grant." })
                (summaryAside 0.0 true) }

let private CONSEQUENCES =
    $"""<dl class="unb-x-consequences">
<div>{icon "refresh"}<dt>What changes</dt><dd>{money 5000.0} moves from Available to grant into grants in progress. Available to grant becomes {money 95000.0}.</dd></div>
<div>{icon "arrow-left"}<dt>Can you undo it?</dt><dd>Yes. You can withdraw it until {SPONSOR} approves it.</dd></div>
<div>{icon "users"}<dt>Who acts next</dt><dd>{SPONSOR} checks the charity and approves or declines your recommendation.</dd></div>
<div>{icon "calendar"}<dt>When</dt><dd>A decision usually by {time "2026-12-03" "Dec 3, 2026"}. Payment within 5 business days of approval.</dd></div>
</dl>"""

let private reviewKv =
    kv [ "Purpose", "General operating support"; "Recognition", "Elin Lindqvist"; "Your confirmations", "No benefit to you or your family; doesn't pay a personal pledge" ]

let private review =
    { baseP with
        Pagehead = headWith "Review and submit" (Some $"Nothing is sent until you submit · From {FUND}")
        Body =
            flow
                (panel
                    { P with
                        Id = "rev"
                        Title = "Your grant recommendation"
                        Meta = quiet "Edit<span class=\"forma-visually-hidden\"> grant details</span>" "#edit" "edit"
                        Body =
                            $"""<div class="unb-x-commit"><p class="unb-x-commit__amount">{moneyHero 5000.0}</p><p class="unb-x-commit__to">to {CHARITY.Name}, {CHARITY.Place}</p><p class="unb-small unb-muted">One time · EIN <span class="unb-num">{CHARITY.Ein}</span></p></div>
{reviewKv}
<h3>What happens when you submit</h3>
{CONSEQUENCES}"""
                        Foot =
                            $"""<div class="unb-x-actions">{primary "Submit grant recommendation"}{link "#edit" "Edit details"}<p class="unb-x-actions__note">Your recommendation is advisory: {SPONSOR} makes the final decision.</p></div>""" })
                (summaryAside 5000.0 false) }

let private submitted =
    let progressTrack =
        track
            "Progress of grant 2026-004835"
            [ { Label = "Submitted"; When = time "2026-12-01" "Dec 1, 2026"; Step = "done" }
              { Label = "Sponsor review"; When = $"""Decision expected by {time "2026-12-03" "Dec 3"}"""; Step = "current" }
              { Label = "Approved"; When = "Not yet"; Step = "todo" }
              { Label = "Paid"; When = $"""Expected by {time "2026-12-10" "Dec 10"}"""; Step = "todo" } ]
    { baseP with
        Notifications = 3
        Pagehead = headWith "Grant recommendation submitted" (Some $"From {FUND}")
        Body =
            flow
                (panel
                    { P with
                        Id = "done"
                        Label = "Result"
                        Body =
                            $"""<div class="unb-x-outcome" data-state="ok"><span class="unb-x-outcome__icon">{icon "success"}</span>
<h2>{SPONSOR} received your recommendation</h2>
<p>{money 5000.0} to {CHARITY.Name} on {time "2026-12-01T17:02:00-06:00" "Dec 1, 2026 at 5:02 PM CT"}. Grant <span class="unb-num">2026-004835</span>.</p></div>
{progressTrack}
<p class="unb-small">We'll email you when it's approved. You can withdraw it from the grant page until then.</p>"""
                        Foot = $"""<div class="unb-x-actions">{primaryLink "#grant-2026-004835" "View grant"}{link "#recommend" "Recommend another grant"}</div>""" })
                (panel
                    { P with
                        Id = "sum"
                        Title = "Your fund"
                        Body = $"""{dl [ "Available to grant", money 95000.0; "In review", money 5000.0 ]}<p class="unb-small unb-muted">Updated {time "2026-12-01T17:02:00-06:00" "today at 5:02 PM CT"}.</p>""" }) }

let private unknown =
    let sent =
        kv
            [ "What you sent", $"""{money 5000.0} to {CHARITY.Name}<small>{time "2026-12-01T17:02:00-06:00" "Dec 1, 2026 at 5:02 PM CT"}</small>"""
              "Status", chip "unknown" "Not confirmed yet"
              "How we'll resolve it", "We're checking automatically and will show the result here and by email within 15 minutes. If it wasn't received, nothing is taken from Available to grant." ]
    { baseP with
        Pagehead = headWith "Checking your recommendation" (Some $"From {FUND}")
        Body =
            flow
                (panel
                    { P with
                        Id = "unk"
                        Label = "Result"
                        Body =
                            $"""<div class="unb-x-outcome" data-state="unknown"><span class="unb-x-outcome__icon">{icon "help-circle"}</span>
<h2>We can't confirm your recommendation was received</h2>
<p>The connection ended before {SPONSOR} replied. Please don't submit it again: if it did arrive, a second one could create a duplicate grant.</p></div>
{sent}"""
                        Foot =
                            $"""<div class="unb-x-actions">{primary "Check status"}{quiet "Go to grants" "#grants" "chevron-right"}<p class="unb-x-actions__note">Check status only looks up this recommendation. It never sends it again.</p></div>""" })
                "" }

let private restricted =
    { baseP with
        Pagehead = head ()
        Body =
            notice
                { N with
                    State = "attention"
                    IconName = Some "lock"
                    Title = "Submitting grant recommendations is paused"
                    Body =
                        $"""<p>{SPONSOR} is reviewing a recent change to your account details. Reviews usually finish within 2 business days. Until then you can't submit new recommendations.</p>
<p>You can still find charities and save this recommendation as a draft to submit later. Grants already approved will be paid as scheduled, and adding money and documents aren't affected.</p>"""
                    Actions = [ link "#contact" "Contact donor services" ] }
            + "\n"
            + flow
                (pickedCharity CHARITY false
                 + detailsForm { DF with Action = Some(primary "Save draft" + """<p class="unb-x-actions__note">We'll email you when you can submit it.</p>""") })
                (summaryAside 5000.0 true) }

let private options =
    { baseP with Pagehead = head (); Body = flow (pickedCharity CHARITY false + detailsForm { DF with Open = true }) (summaryAside 6000.0 true) }

let states : State list =
    [ app "choose-charity" "Choose a charity: search results with eligibility in plain words" "Donor (primary advisor)" [ "normal"; "charity-search"; "ineligible-charity" ] choose
      app "details" "Amount and details: one column, progressive disclosure" "Donor (primary advisor)" [ "normal" ] (details "unbound-default")
      app "validation-errors" "Validation: amount above Available to grant, missing purpose, charity on hold" "Donor (primary advisor)" [ "validation-error"; "ineligible-charity"; "eligibility-hold" ] errors
      app "review" "Review and submit: the single G-005 confirmation" "Donor (primary advisor)" [ "confirmation" ] review
      app "submitted" "Submitted: what happens next" "Donor (primary advisor)" [ "success"; "pending-grants" ] submitted
      app "outcome-unknown" "Outcome unknown after submit timeout (G-006)" "Donor (primary advisor)" [ "outcome-unknown"; "integration-unavailable" ] unknown
      app "restricted" "Restricted: recommending paused, draft still possible" "Donor (primary advisor)" [ "restricted-fund" ] restricted
      app "more-options" "More options open: percentage of fund value and repeat (phone reference)" "Donor on a phone" [ "phone"; "percentage-grant"; "recurring" ] options
      app "white-label" "Amount and details under the Harbor Giving brand" "Donor (primary advisor)" [ "white-label" ] (details "example-harbor") ]

let screen : Screen = { Id = "S-003"; Slug = "S-003-grant-recommendation"; Title = "Recommend a grant"; Requirement = "UNB-UI-S-003"; States = states }

/// S-006 People, delegation and successors (port of tools/screens/S-006.mjs).
module Unbound.Designs.V1.Screens.S006

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Lindqvist Family Fund (F-2026-000418)"; "People" ]

let private revoke (p: string) =
    Ui.dialog(
        id = $"{p}-revoke",
        invoker = "Revoke…",
        invokerAttrs = "aria-label=\"Revoke access for Jordan Pike\"",
        title = "Revoke Jordan Pike's access now?",
        desc = "Jordan Pike loses access to this fund immediately, including any session that is open now.",
        body =
            kv [ "What changes", "Advisor access (view and recommend grants) ends at once; drafts Jordan started are closed"
                 "Can it be undone?", "No. To restore access you must invite Jordan again"
                 "Who acts next", "No one. Example Charitable Foundation is notified for its records"
                 "Grants already submitted", "Unaffected; they continue through approval" ],
        actions = """<button type="submit" value="cancel" autofocus>Keep access</button><button type="submit" value="revoke">Revoke Jordan Pike's access</button>"""
    )

let private invite (p: string) =
    let email = Ui.field(id = $"{p}-email", label = "Email address (required)", ``type`` = "email", attrs = "required autocomplete=\"off\"")
    let role =
        Ui.select(
            id = $"{p}-role",
            label = "Role",
            options = [ "advisor", "Advisor: view and recommend grants", true; "viewer", "Viewer: view only", false; "co-donor", "Co-donor", false ],
            desc = "Roles come from Example Charitable Foundation's role catalogue."
        )
    let scopeSel = Ui.select(id = $"{p}-scope", label = "Scope", options = [ ("fund", "This fund only", true) ])
    Ui.dialog(
        id = $"{p}-invite",
        invoker = "Invite participant…",
        title = "Invite someone to this fund",
        desc = "They receive an email invitation that expires in 7 days.",
        body = $"{email}\n{role}\n{scopeSel}",
        actions = """<button type="submit" value="cancel">Cancel</button><button type="submit" value="invite">Send invitation</button>"""
    )

let private people (p: string) =
    Ui.dataGrid(
        label = "Participants",
        cols = [ Ui.col "Name"; Ui.col "Role"; Ui.col "Scope"; Ui.col "Effective"; Ui.col "Status"; Ui.col("Actions", plain = true) ],
        rows =
            [ [ "Avery Lindqvist"; "Primary donor"; "Fund F-2026-000418"; "Since 3 Feb 2025"; loz "ok" "Active"; "—" ]
              [ "Jordan Pike, Pike Wealth Partners"
                "Advisor"
                "Fund F-2026-000418"
                "Since 12 Mar 2025"
                loz "ok" "Active"
                """<span class="forma-cluster"><button type="button" aria-label="Change scope for Jordan Pike">Change scope</button>""" + revoke p + "</span>" ]
              [ "Sam Okafor"; "Viewer"; "Fund F-2026-000418"; "Invited 28 Nov 2026"; loz "attention" "Invited: waiting on Sam"; """<button type="button" aria-label="Cancel invitation for Sam Okafor">Cancel invitation</button>""" ]
              [ "Morgan Hale"; "Advisor"; "Fund F-2026-000418"; "12 Mar 2025 to 30 Sep 2026"; loz "blocked" "Revoked"; "—" ] ]
    )

let private allocation (p: string) (a: int) (b: int) (invalid: bool) =
    let errRef = if invalid then $" {p}-alloc-error" else ""
    let inv = if invalid then " aria-invalid=\"true\"" else ""
    let stateAttr = if invalid then "invalid" else "valid"
    let msg =
        if invalid then validationMessage $"{p}-alloc-error" $"Allocations total {a + b}%%. Assign the remaining {100 - a - b}%% before saving." else ""
    tag
        "allocation"
        $$"""<fieldset class="forma-allocation" aria-describedby="{{p}}-alloc-help{{errRef}}">
  <legend class="forma-allocation__legend">Divide 100% of the fund between your successors</legend>
  <div class="forma-allocation__items">
    <label class="forma-allocation__item"><span>Riley Lindqvist (successor advisor, new fund)</span><input type="number" name="{{p}}-riley" min="0" max="100" step="1" value="{{a}}" inputmode="numeric" id="{{p}}-riley"{{inv}}></label>
    <label class="forma-allocation__item"><span>Hope Foundation, EIN 00-0000101 (charity beneficiary)</span><input type="number" name="{{p}}-hope" min="0" max="100" step="1" value="{{b}}" inputmode="numeric"{{inv}}></label>
  </div>
  <div class="forma-allocation__total" data-forma-state="{{stateAttr}}" aria-live="polite" aria-atomic="true"><span>Total</span><output>{{a + b}}% of 100%</output></div>
  <p class="forma-field__description" id="{{p}}-alloc-help">Percentages must total exactly 100%.</p>
  {{msg}}
</fieldset>"""

let private planText (p: string) (a: int) (b: int) =
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-plan"><h3 id="{{p}}-plan">Successor plan in words</h3>
<p>When Example Charitable Foundation verifies the death of Avery Lindqvist, the fund is suspended and then divided:</p>
<ol>
  <li>{{a}}% goes to a new fund advised by Riley Lindqvist (successor advisor).</li>
  <li>{{b}}% is granted to Hope Foundation, EIN {{ident "00-0000101"}}, Springfield, IL.</li>
</ol>
<p>Plan version 3, saved 14 Oct 2026. Example Charitable Foundation must approve the plan when it takes effect.</p></section>"""

let private successorsInvalid (p: string) =
    let vs =
        Ui.validationSummary(id = $"{p}-vs", title = "The plan cannot be saved yet", items = [ ($"#{p}-riley", "Allocations total 90%. Assign the remaining 10% before saving.") ])
    $$"""{{scope}}<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-ft"><h3 id="{{p}}-ft">Designate successors</h3>
{{vs}}
{{allocation p 60 30 true}}
<div class="forma-actions"><button type="submit">Save successor plan</button><button type="button">Cancel</button></div></form>"""

let private suspended (p: string) =
    let op =
        Ui.opStatus(
            id = $"{p}-s",
            state = "blocked",
            label = "Suspended: verified death",
            symbol = "×",
            title = "Fund suspended on 18 Nov 2026",
            body = "<p>Grants and contributions are stopped until the successor plan is activated.</p>"
        )
    let grid =
        Ui.dataGrid(
            label = "Successors",
            cols = [ Ui.col "Successor"; Ui.col "Share"; Ui.col "Status" ],
            rows =
                [ [ "Riley Lindqvist"; "60%"; loz "attention" "Successor designated: invitation sent" ]
                  [ "Hope Foundation, EIN 00-0000101"; "40%"; loz "ok" "Successor active: grant approved" ] ]
        )
    $"{scope}{op}\n{grid}"

let screen: Screen =
    { Id = "S-006"
      File = "S-006-participants-successors.html"
      Title = "People, delegation and successors"
      ReqIds = [ "UNB-UI-S-006"; "UNB-UI-G-005" ]
      Audience = "donor"
      Brand = true
      Lead = "Who can act on this fund, in what role and scope, and who succeeds you."
      States =
        [ state
              "Participants and revoke confirmation"
              "Primary donor"
              "Role, scope, effective dates and status per participant; invite; revoke (A2, immediate) with G-005 dialog"
              (fun p -> scope + "<div class=\"forma-actions\">" + invite p + "</div>" + people p)
          state
              "Successors: allocations do not total 100%"
              "Primary donor"
              "UNB-UI-S-006 AC-1 (validation blocks saving and states the 10% remainder)"
              successorsInvalid
          state "Successors designated, with the text equivalent" "Primary donor" "Successor plan shown as text, not only a diagram" (fun p ->
              scope + allocation p 60 40 false + planText p 60 40)
          state "Fund suspended: successor activation" "Sponsor staff" "Suspended (verified death), Successor designated, Successor active statuses" suspended ]
      Coverage =
        [ cov "UNB-UI-S-006" "AC-1" NeedsApplicationBehaviour "State 2: allocation total data-forma-state=invalid, message 'Allocations total 90%. Assign the remaining 10% before saving.' and summary. Computing the total live and blocking save are Limen/application behaviour."
          cov "UNB-UI-G-005" "revoke" Yes "Revoke dialog states what changes, irreversibility, who acts next; confirm names the action. No destructive visual variant (FORMA-GAP-01)."
          cov "UNB-UI-S-006" "statement (text equivalent)" Yes "State 3 'Successor plan in words' ordered list. Forma has no read-only allocation summary; composed from plain list (FORMA-GAP-12)." ]
      Gaps = [ "FORMA-GAP-01"; "FORMA-GAP-06"; "FORMA-GAP-12"; "FORMA-GAP-14" ]
      Render = None }

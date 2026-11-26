/// S-003 Recommend a grant (port of tools/screens/S-003.mjs).
module Unbound.Designs.V1.Screens.S003

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Lindqvist Family Fund (F-2026-000418)"; "Recommend a grant" ]

let private steps = [ "Charity"; "Amount"; "Purpose and acknowledgements"; "Review and submit" ]

let private available =
    kv [ ("Available to grant", critical "$100,000.00" + Ui.fresh(label = "Calculated", iso = "2026-12-01T16:58:00-06:00", shown = "1 Dec 2026, 4:58 PM CT", stateText = "Current")) ]

let private wizard (p: string) (step: int) (title: string) (body: string) (actions: string) =
    let stepItem i s =
        if i + 1 < step then $"<li data-state=\"complete\"><span>{i + 1}</span>{s}</li>"
        elif i + 1 = step then $"<li data-state=\"current\" aria-current=\"step\"><span>{i + 1}</span>{s}</li>"
        else $"<li><span>{i + 1}</span>{s}</li>"
    let items = steps |> List.mapi stepItem |> String.concat ""
    tag
        "wizard"
        $$"""<section class="forma-wizard" aria-labelledby="{{p}}-wt">
  <header class="forma-wizard__header">
    <div><span class="forma-wizard__position">Step {{step}} of 4</span><h3 id="{{p}}-wt">{{title}}</h3></div>
    <progress value="{{step}}" max="4" aria-label="Grant recommendation progress">{{step}} of 4</progress>
  </header>
  <ol class="forma-wizard__steps" aria-label="Grant recommendation steps">
    {{items}}
  </ol>
  <div class="forma-wizard__body">{{available}}{{body}}</div>
  <footer class="forma-wizard__actions">{{actions}}</footer>
</section>"""

let private amountForm (p: string) (amount: string) (error: string) =
    let summary =
        if error <> "" then Ui.validationSummary(id = $"{p}-vs", title = "There is 1 problem to fix", items = [ $"#{p}-amt", error ]) else ""
    let kind =
        Ui.choiceGroup(
            name = $"{p}-kind",
            legend = "How do you want to give?",
            choices =
                [ "fixed", "Fixed amount", "A dollar amount, for example $5,000.00.", true
                  "percent", "Percentage of fund value", "A percentage fixed at a point you choose.", false ]
        )
    let amt =
        Ui.field(id = $"{p}-amt", label = "Grant amount in US dollars (required)", desc = "Minimum $50.00. You can recommend up to your Available to grant.", value = amount, attrs = "inputmode=\"decimal\" required", error = error)
    let pct = Ui.field(id = $"{p}-pct", label = "Percentage (percentage grants only)", desc = "For example 10.", attrs = "inputmode=\"decimal\"")
    let baseSel = Ui.select(id = $"{p}-base", label = "Percentage of", options = [ ("fund-value", "Fund value (market value)", true) ])
    let fix = Ui.select(id = $"{p}-fix", label = "Amount is fixed", options = [ "approval", "When your sponsor approves", true; "payment", "On the payment date", false ])
    let recur = Ui.checkbox(id = $"{p}-rec", label = "Make this a recurring grant", desc = "Optional. Each payment is checked against Available to grant and sponsor policy.")
    let freq =
        Ui.select(id = $"{p}-freq", label = "Repeat", options = [ "", "Choose how often", false; "monthly", "Monthly", false; "quarterly", "Quarterly", false; "annually", "Annually", false ])
    let start = tag "date-time-field" (Ui.field(id = $"{p}-start", label = "First payment date", ``type`` = "date", desc = "Dates are in Central Time (America/Chicago)."))
    $$"""<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-wt" novalidate>
{{summary}}
{{kind}}
{{amt}}
<div class="forma-form-row" data-forma-form-layout="grid">
{{pct}}
{{baseSel}}
{{fix}}
</div>
{{recur}}
<div class="forma-form-row" data-forma-form-layout="grid">
{{freq}}
{{start}}
</div>
</form>"""

let private navActions (p: string) =
    $$"""<button type="button">Back</button><button type="button">Save draft</button><button type="submit" form="{{p}}-f">Continue</button>"""

let private confirmDialog (p: string) =
    Ui.dialog(
        id = $"{p}-confirm",
        invoker = "Submit…",
        title = "Submit grant recommendation?",
        desc = "Your sponsor approves this grant; you can withdraw until approval.",
        body =
            kv [ "Amount", criticalVerify true "$5,000.00"
                 "Charity", $"""Riverside Food Bank, EIN {ident "00-0000103"}, Springfield, IL"""
                 "From fund", $"""Lindqvist Family Fund {ident "F-2026-000418"}"""
                 "What changes", "$5,000.00 is reserved from Available to grant, which becomes $95,000.00"
                 "Can it be undone?", "Yes. You can withdraw the recommendation until Example Charitable Foundation approves it"
                 "Who acts next", "Example Charitable Foundation grants team checks eligibility and approves or declines"
                 "Expected next step", "Decision usually within 2 business days; payment within 5 business days of approval" ],
        actions = """<button type="submit" value="cancel" autofocus>Go back and edit</button><button type="submit" value="submit">Submit grant recommendation</button>"""
    )

let private review (_p: string) =
    kv [ "Charity", $"""Riverside Food Bank, EIN {ident "00-0000103"}"""
         "Amount", critical "$5,000.00"
         "Schedule", "One time"
         "Purpose", "General operating support"
         "Recognition", "Use my name: Avery Lindqvist"
         "Fulfils a pledge?", "No" ]

let private step1 (p: string) =
    let eligibility = loz "ok" "Eligible" + Ui.fresh(label = "IRS data as of", iso = "2026-11-30", shown = "30 Nov 2026", stateText = "Current")
    let selected = kv [ "Selected charity", $"""Riverside Food Bank, EIN {ident "00-0000103"}, Springfield, IL"""; "Platform eligibility", eligibility ]
    let body = $$"""<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-wt">{{selected}}<p><a href="S-002-charity-search.html">Choose a different charity</a></p></form>"""
    scope + wizard p 1 "Charity" body (navActions p)

let private step3 (p: string) =
    let purpose =
        Ui.textarea(id = $"{p}-purpose", label = "Purpose (required)", desc = "Shown to the charity on the remittance. Up to 200 characters.", value = "General operating support", attrs = "maxlength=\"200\" required")
    let recog =
        Ui.choiceGroup(
            name = $"{p}-recog",
            legend = "How should the charity recognize this grant?",
            choices =
                [ "name", "Use my name", "Avery Lindqvist", true
                  "anon", "Anonymous", "The charity sees “Anonymous donor via Example Charitable Foundation”.", false
                  "custom", "Custom name", "For example “The Lindqvist Family”.", false ]
        )
    let a1 =
        Ui.checkbox(id = $"{p}-a1", label = "I and my family will receive no goods, services or other benefit in return for this grant, including tickets, dinners or memberships.", ``checked`` = true, attrs = "required")
    let a2 =
        Ui.checkbox(id = $"{p}-a2", label = "This grant will not be used to pay for any part of an event, membership or purchase that benefits me.", ``checked`` = true, attrs = "required")
    let pledgeError =
        validationMessage
            $"{p}-pledge-error"
            "This grant cannot be submitted. Example Charitable Foundation does not allow grants that fulfil a personal pledge. You can pay the pledge personally, or answer No if this grant is not tied to a pledge."
    let pledge =
        tag
            "binary-choice"
            $$"""<fieldset class="forma-binary-choice" aria-describedby="{{p}}-pledge-error">
  <legend class="forma-binary-choice__legend">Does this grant fulfil a pledge? (required)</legend>
  <p class="forma-field__description">A pledge is a promise you made to the charity to give a specific amount.</p>
  <div class="forma-binary-choice__options">
    <label class="forma-binary-option"><input type="radio" name="{{p}}-pledge" value="yes" checked required><span>Yes</span></label>
    <label class="forma-binary-option"><input type="radio" name="{{p}}-pledge" value="no"><span>No</span></label>
  </div>
  {{pledgeError}}
</fieldset>"""
    let body =
        $$"""<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-wt">
{{purpose}}
{{recog}}
<fieldset class="forma-stack"><legend>Required affirmations from Example Charitable Foundation</legend>
{{a1}}
{{a2}}
</fieldset>
{{pledge}}
</form>"""
    let actions =
        $$"""<button type="button">Back</button><button type="button">Save draft</button><button type="submit" form="{{p}}-f" disabled aria-describedby="{{p}}-pledge-error">Continue</button>"""
    scope + wizard p 3 "Purpose and acknowledgements" body actions

let private submitted (p: string) =
    let op =
        Ui.opStatus(
            id = $"{p}-ok",
            state = "attention",
            label = "Eligibility check in progress",
            symbol = "!",
            title = "Recommendation received: grant 2026-004817",
            body = "<p>We reserved $5,000.00. Example Charitable Foundation checks the charity's eligibility next.</p>",
            actions = [ """<a class="forma-button" href="S-004-grant-detail.html">View grant 2026-004817</a>""" ]
        )
    let gp =
        grantProgress
            $"{p}-gp"
            [ Ui.dim("Recommendation", "ok", "Submitted", "2026-12-01T17:02:00-06:00", "1 Dec 2026, 5:02 PM CT")
              Ui.dim("Eligibility", "attention", "Checking", "2026-12-01T17:02:00-06:00", "1 Dec 2026, 5:02 PM CT")
              Ui.dim("Approval", "", "Not started")
              Ui.dim("Funding", "", "Not started")
              Ui.dim("Payment", "", "Not started") ]
    scope + op + gp

let private unknownOutcome (p: string) =
    let details = kv [ "Request reference", ident "IDK-7f3a-91c2-4d0e"; "Amount", money "$5,000.00"; "Charity", "Riverside Food Bank" ]
    scope
    + Ui.opStatus(
        id = $"{p}-unk",
        state = "unknown",
        label = "Outcome unknown",
        title = "We cannot confirm your recommendation was received",
        body = "<p>The connection ended before Example Charitable Foundation replied. Do not submit again: that could create a second grant. Check status asks for the recorded outcome of this exact request.</p>" + details,
        actions = [ """<button type="button">Check status</button>"""; """<a class="forma-button" href="S-001-giving-home.html">Return to giving home</a>""" ]
    )

let screen: Screen =
    { Id = "S-003"
      File = "S-003-grant-recommendation.html"
      Title = "Recommend a grant"
      ReqIds = [ "UNB-UI-S-003"; "UNB-UI-G-005"; "UNB-UI-G-006"; "UNB-UI-G-012" ]
      Audience = "donor"
      Brand = true
      Lead = "Four steps: charity, amount, purpose and acknowledgements, review and submit. Available to grant stays visible throughout."
      States =
        [ state "Step 1: charity selected" "Donor" "Step 1 binds the charity chosen in S-002" step1
          state "Step 2: amount (normal)" "Donor on a phone" "Fixed amount or percentage with base and fixing point; optional recurring schedule (UNB-UI-G-017)" (fun p ->
              scope + wizard p 2 "Amount" (amountForm p "5,000.00" "") (navActions p))
          state "Step 2: amount above Available to grant" "Donor" "UNB-UI-S-003 AC-1; UNB-UI-G-012 summary linked to the field, entered value kept" (fun p ->
              scope + wizard p 2 "Amount" (amountForm p "150,000.00" "Enter $100,000.00 or less. Your Available to grant is $100,000.00.") (navActions p))
          state
              "Step 3: purpose, recognition and a pledge answered Yes"
              "Donor, sponsor pledge policy Prohibited"
              "UNB-UI-S-003 AC-2 (required pledge question; Yes blocks submission with an explanation); sponsor affirmations verbatim"
              step3
          state
              "Step 4: review and confirmation dialog"
              "Donor"
              "UNB-UI-G-005 AC-1 (dialog lists amount, charity, fund, reversibility, who acts next; confirm reads 'Submit grant recommendation')"
              (fun p -> scope + wizard p 4 "Review and submit" (review p) ("<button type=\"button\">Back</button><button type=\"button\">Save draft</button>" + confirmDialog p))
          state "Submitting" "Donor" "Pending submission: busy confirm button, fieldset disabled (no double submission)" (fun p ->
              scope
              + $$"""<form class="forma-form" aria-busy="true" aria-labelledby="{{p}}-sub-title"><h3 id="{{p}}-sub-title">Review and submit</h3><fieldset disabled class="forma-stack"><legend>Grant recommendation</legend>{{review p}}<div class="forma-actions"><button type="button">Back</button><button type="submit" aria-busy="true">Submitting grant recommendation</button></div></fieldset></form>""")
          state "Submitted: five-dimension status" "Donor" "UNB-UI-G-002 (five ordered dimensions, each with label and as-of time; derived headline)" submitted
          state
              "Outcome unknown after a timeout"
              "Donor"
              "UNB-UI-S-003 AC-3 and UNB-UI-G-006 AC-1 (unknown, Check status by idempotency key, no retry, no success or failure shown)"
              unknownOutcome
          state "Rejected by policy" "Donor" "Policy rejection with reason (UNB-UI-G-001 blocked)" (fun p ->
              scope
              + Ui.opStatus(
                  id = $"{p}-rej",
                  state = "blocked",
                  label = "Rejected by policy",
                  symbol = "×",
                  title = "This recommendation was not accepted",
                  body = "<p>Example Charitable Foundation's policy sets a minimum grant of $50.00; you entered $25.00. Nothing was reserved. Change the amount to continue.</p>",
                  actions = [ """<button type="button">Change amount</button>""" ]
              )) ]
      Coverage =
        [ cov "UNB-UI-S-003" "AC-1" NeedsApplicationBehaviour "State 3 shows the inline error stating $100,000.00 and a linked validation-summary with the value kept. Not advancing the step and moving focus to the summary are application behaviour."
          cov "UNB-UI-S-003" "AC-2" NeedsApplicationBehaviour "State 4 shows the required pledge question, the Yes answer, the explanation and a disabled Continue. Showing the question per sponsor policy and blocking server-side are application behaviour."
          cov "UNB-UI-S-003" "AC-3" NeedsApplicationBehaviour "State 8 shows the unknown operation-status with Check status and the request reference. Idempotent lookup and not creating a duplicate are server behaviour."
          cov "UNB-UI-G-005" "AC-1" Yes "Dialog s003-5-confirm lists amount, charity, fund, reversibility ('you can withdraw until approval'), who acts next, expected time; confirm button reads 'Submit grant recommendation'. Rendered open in rendered/S-003-grant-recommendation-s003-5-confirm-*.png."
          cov "UNB-UI-G-006" "AC-1" NeedsApplicationBehaviour "Text and Check status action are in markup; the query by idempotency key is application behaviour."
          cov "UNB-UI-G-012" "AC-1" NeedsApplicationBehaviour "Summary + inline message + aria-invalid + kept value shown for one field. Focus movement on Continue is application behaviour. Field invalid styling exists in the 1.0.2 build ([aria-invalid], :user-invalid), which appears to close FORMA-GAP-02; contrast proof still owed."
          cov "UNB-UI-G-002" "AC-1/AC-2" NeedsApplicationBehaviour "Five dimensions rendered with steps + lozenge + as-of. Headline derivation is application logic; see S-004 for both AC examples."
          cov "UNB-UI-G-009" "AC-1" Yes "Every step renders at 320 px with no page-level horizontal scroll (verification.json)." ]
      Gaps = [ "FORMA-GAP-01"; "FORMA-GAP-02"; "FORMA-GAP-06"; "FORMA-GAP-08"; "FORMA-GAP-09"; "FORMA-GAP-11"; "FORMA-GAP-14" ]
      Render = None }

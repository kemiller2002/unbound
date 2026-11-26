/// S-009 Case workspace (port of tools/screens/S-009.mjs).
module Unbound.Designs.V1.Screens.S009

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope = scopeTrail [ "Example Charitable Foundation"; "Sponsor operations"; "Case CASE-2026-000231" ]

let private crumbs =
    """<a href="S-008-exception-inbox.html">Exception inbox</a> <span aria-hidden="true">/</span> <a href="#cases">Cases</a>"""

let private evidence (p: string) =
    let screened = Ui.fresh(label = "Screened", iso = "2026-11-27T08:14:00-06:00", shown = "27 Nov 2026, 8:14 AM CT", stateText = "Current", state = "ok")
    let observed = Ui.fresh(label = "Observed", iso = "2026-11-18", shown = "18 Nov 2026", stateText = "Current", state = "ok")
    let ev1 =
        tag
            "evidence-relationship"
            $$"""<article class="forma-evidence-relationship" aria-labelledby="{{p}}-ev1"><header><p class="forma-component-kicker">Raises</p><h5 id="{{p}}-ev1">Name similarity to a listed party</h5></header><div class="forma-evidence-relationship__body"><p><strong>Evidence:</strong> Recipient name scored 0.91 against a listed entity.</p><p><strong>Source:</strong> Screening provider result SR-88123</p><p><strong>Scope:</strong> Recipient legal name only</p></div><footer>{{screened}}</footer></article>"""
    let ev2 =
        tag
            "evidence-relationship"
            $$"""<article class="forma-evidence-relationship" aria-labelledby="{{p}}-ev2"><header><p class="forma-component-kicker">Weakens</p><h5 id="{{p}}-ev2">Address and registration differ</h5></header><div class="forma-evidence-relationship__body"><p><strong>Evidence:</strong> Recipient is a domestic public charity with IRS registration since 2009.</p><p><strong>Source:</strong> IRS Business Master File</p><p><strong>Scope:</strong> EIN 00-0000210</p></div><footer>{{observed}}</footer></article>"""
    $$"""<section class="forma-stack" aria-labelledby="{{p}}-facts"><h4 id="{{p}}-facts">Facts</h4>
{{ev1}}
{{ev2}}
</section>"""

let private conflict (p: string) =
    tag
        "comparison-pairs"
        $$"""<section class="forma-comparison-pairs" aria-labelledby="{{p}}-cmp">
  <h4 id="{{p}}-cmp">Conflicting sources side by side</h4>
  <div class="forma-comparison-pairs__item"><div class="forma-comparison-pairs__label">Country</div><div class="forma-comparison-pairs__value" data-side="before" data-label="Screening list">Outside the United States</div><div class="forma-comparison-pairs__value" data-side="after" data-label="IRS record">Illinois, United States</div></div>
  <div class="forma-comparison-pairs__item"><div class="forma-comparison-pairs__label">Entity type</div><div class="forma-comparison-pairs__value" data-side="before" data-label="Screening list">Trading company</div><div class="forma-comparison-pairs__value" data-side="after" data-label="IRS record">501(c)(3) public charity</div></div>
</section>"""

let private rule (p: string) =
    tag
        "provenance-trail"
        $$"""<section class="forma-provenance-trail" aria-labelledby="{{p}}-rule"><h4 id="{{p}}-rule">Rule that raised this case</h4><ol>
  <li><span class="forma-provenance-trail__kind">Source</span><strong>Screening result SR-88123</strong><small>27 Nov 2026</small></li>
  <li><span class="forma-provenance-trail__kind">Rule</span><strong>SAN-02 Potential sanctions match</strong><small>Rule version 4: score at least 0.85</small></li>
  <li><span class="forma-provenance-trail__kind">Effect</span><strong>Grant 2026-004102 held</strong><small>Payment blocked until a decision</small></li>
</ol></section>"""

let private permitted (p: string) (preparer: bool) =
    let severity = if preparer then "blocking" else "warning"
    let sod = if preparer then $"<p id=\"{p}-sod\"><strong>You prepared this case; another reviewer must decide.</strong></p>" else ""
    let decide = if preparer then "<span>Not available</span>" else $"<a href=\"#{p}-decide\">Decide</a>"
    tag
        "obligation-panel"
        $$"""<aside class="forma-obligation-panel" aria-labelledby="{{p}}-perm">
  <header class="forma-obligation-panel__header"><div><h4 id="{{p}}-perm">Actions you can take</h4><p>Your authority: A4 compliance reviewer.</p></div><span class="forma-obligation-panel__count" aria-hidden="true">3</span></header>
  <ul class="forma-obligation-list">
    <li class="forma-obligation" data-forma-severity="information"><span class="forma-obligation__status">A1</span><div><strong>Request evidence</strong><p>Asks the charity for documents. The grant stays held; SLA clock pauses.</p></div><a href="#{{p}}-req">Request</a></li>
    <li class="forma-obligation" data-forma-severity="{{severity}}"><span class="forma-obligation__status">A4</span><div><strong>Record decision: false positive or true match</strong><p>False positive releases the hold; true match blocks the grant permanently and starts the OFAC report (due in 10 business days).</p>{{sod}}</div>{{decide}}</li>
    <li class="forma-obligation" data-forma-severity="information"><span class="forma-obligation__status">A2</span><div><strong>Escalate</strong><p>Sends the case to the compliance lead with your notes.</p></div><a href="#{{p}}-esc">Escalate</a></li>
  </ul>
</aside>"""

let private decisionDialog (p: string) =
    let outcome =
        Ui.choiceGroup(
            name = $"{p}-outcome",
            legend = "Decision (required)",
            choices =
                [ "fp", "False positive", "Release the hold. Payment continues to the verified destination.", false
                  "tm", "True match", "Block the grant permanently and start the OFAC report.", false ]
        )
    let rationale =
        Ui.textarea(id = $"{p}-rationale", label = "Written rationale (required)", desc = "Explain which evidence you relied on. At least 50 characters.", attrs = "required minlength=\"50\"")
    let consequences =
        kv [ "Who acts next", "False positive: payments team within 1 business day. True match: compliance lead files the report"
             "Can it be undone?", "No. A new case is needed to change it" ]
    tag
        "dialog"
        $$"""<button type="button" commandfor="{{p}}-decide" command="show-modal" id="{{p}}-decide-open">Record decision…</button>
<dialog class="forma-dialog" id="{{p}}-decide" data-forma-motion-weight="heavy" aria-labelledby="{{p}}-decide-title" aria-describedby="{{p}}-decide-description">
  <form method="dialog">
    <div class="forma-dialog__body">
      <h2 id="{{p}}-decide-title">Record sanctions decision for grant 2026-004102</h2>
      <p id="{{p}}-decide-description">This decision is final and recorded with your name. The AI summary is not part of the record.</p>
      {{outcome}}
      {{rationale}}
      {{consequences}}
    </div>
    <div class="forma-dialog__actions"><button type="submit" value="cancel" autofocus>Cancel</button><button type="submit" value="record">Record decision</button></div>
  </form>
</dialog>"""

let private aiFlyout (p: string) =
    let caution =
        callout $"{p}-ai-label" "caution" "AI-generated, not a decision" "<p>This summary cannot approve, release or decide anything. A reviewer must decide using the evidence.</p>"
    let conversation =
        tag
            "conversation"
            $$"""<section class="forma-conversation" aria-labelledby="{{p}}-conv"><header class="forma-conversation__header"><span class="forma-component-kicker">AI-generated, not a decision</span><h3 id="{{p}}-conv">Summary of CASE-2026-000231</h3></header>
      <ol class="forma-conversation__turns">
        <li class="forma-turn" data-forma-speaker="user"><p class="forma-turn__speaker">You</p><div class="forma-turn__content"><p>Should this be released?</p></div></li>
        <li class="forma-turn" data-forma-speaker="assistant"><p class="forma-turn__speaker">Assistant (AI-generated)</p><div class="forma-turn__content"><p>The evidence leans toward a false positive: the IRS record shows a domestic public charity since 2009, while the listed entity is a foreign trading company. Release would be consistent with SAN-02 guidance, but the decision is yours.</p><p>Sources: screening result SR-88123 (27 Nov 2026, current); IRS Business Master File (18 Nov 2026, current).</p></div></li>
      </ol></section>"""
    let handoff =
        tag
            "handoff-summary"
            $$"""<section class="forma-handoff-summary" aria-labelledby="{{p}}-ho"><header><p class="forma-component-kicker">Assistant proposal</p><h3 id="{{p}}-ho">Suggested outcome: false positive</h3><p>Agent: Harbor Giving case assistant</p></header><dl><div><dt>Uncertainty</dt><dd>Screening provider has not returned the secondary identifiers</dd></div><div><dt>Human authority</dt><dd>A reviewer other than the preparer must record the decision outside this panel</dd></div></dl></section>"""
    tag
        "flyout"
        $$"""<button type="button" commandfor="{{p}}-ai" command="show-modal">Explain this case</button>
<dialog class="forma-flyout" id="{{p}}-ai" data-forma-side="right" data-forma-motion-weight="heavy" aria-labelledby="{{p}}-ai-title" aria-describedby="{{p}}-ai-label">
  <div class="forma-flyout__surface">
    <header class="forma-flyout__header">
      <div><p class="forma-component-kicker">Assistant</p><h2 id="{{p}}-ai-title">Case summary</h2></div>
      <button class="forma-flyout__close" type="button" commandfor="{{p}}-ai" command="close" aria-label="Close case summary">Close</button>
    </header>
    <div class="forma-flyout__body" role="region" tabindex="0" aria-label="Case summary content">
      {{caution}}
      {{conversation}}
      {{handoff}}
    </div>
  </div>
</dialog>"""

let private header (_p: string) (lozenge: string) (actions: string) =
    Ui.recordHeader(
        ``type`` = "Case",
        title = "CASE-2026-000231",
        lozenge = lozenge,
        sub = "Potential sanctions match · Grant " + ident "2026-004102" + " · Opened 27 Nov 2026 · Prepared by Dana Ruiz",
        crumbs = crumbs,
        actions = actions
    )

let private workspace (p: string) (preparer: bool) =
    tag
        "split-pane"
        $$"""<section class="forma-split-pane" aria-label="Case workspace">
  <div class="forma-split-pane__primary">{{evidence p}}{{conflict p}}{{permitted p preparer}}</div>
  <aside class="forma-split-pane__secondary" aria-label="How this case was raised">{{rule p}}</aside>
</section>"""

let private decided (p: string) =
    let record =
        kv [ "Decision", "False positive; hold released"
             "Rationale", "IRS record shows a domestic public charity registered since 2009; listed entity is a foreign trading company with different identifiers."
             "Decided by", "Priya Nair, 1 Dec 2026, 3:12 PM CT (21:12 UTC)"
             "Prepared by", "Dana Ruiz, 27 Nov 2026"
             "Evidence considered", "SR-88123 (27 Nov 2026); IRS Business Master File (18 Nov 2026)" ]
    scope + header p (loz "ok" "Decided: false positive") "" + "\n" + callout $"{p}-dec" "decision" "Decision record" record

let screen: Screen =
    { Id = "S-009"
      File = "S-009-case-workspace.html"
      Title = "Case workspace"
      ReqIds = [ "UNB-UI-S-009"; "UNB-UI-G-011" ]
      Audience = "staff"
      Brand = false
      Lead = "One exception: typed evidence with source and freshness, conflicting sources, the rule that raised it, permitted actions with consequences, AI summary and the decision record."
      States =
        [ state
              "Reviewer who did not prepare the case"
              "Compliance reviewer (Priya Nair)"
              "UNB-UI-S-009 AC-2 (no release control in the AI flyout; decision dialog requires written rationale), UNB-UI-G-011 AC-1"
              (fun p -> scope + header p (loz "attention" "Awaiting decision") (decisionDialog p + aiFlyout p) + workspace p false)
          state
              "Preparer opens their own case"
              "Compliance reviewer who prepared it (Dana Ruiz)"
              "UNB-UI-S-009 AC-1 (Record decision disabled with 'You prepared this case; another reviewer must decide')"
              (fun p ->
                  let actions = $"<button type=\"button\" disabled aria-describedby=\"{p}-sod\">Record decision</button>" + aiFlyout p
                  scope + header p (loz "attention" "Awaiting decision") actions + workspace p true)
          state "Decided: decision record" "Compliance reviewer" "Decision record with decider and preparer as different people" decided ]
      Coverage =
        [ cov "UNB-UI-S-009" "AC-1" NeedsApplicationBehaviour "State 2: native disabled Record decision button described by the text 'You prepared this case; another reviewer must decide'. Segregation of duties must be enforced server-side."
          cov "UNB-UI-S-009" "AC-2" Yes "AI flyout (s009-1-ai) contains conversation and handoff-summary with no Approve/Release controls (the canonical handoff-summary action cluster was deliberately omitted). Decision dialog has a required rationale textarea (required minlength); enforcement is application behaviour."
          cov "UNB-UI-S-009" "statement (layout)" Yes "Permitted actions (obligation-panel) are placed in the split-pane primary column: in the secondary column at 1280 px the obligation grid crushes its text to one word per line (rendered/gap-evidence/FORMA-GAP-15-*.png). FORMA-GAP-15."
          cov "UNB-UI-G-011" "AC-1" Yes "Flyout opened from 'Explain this case', labelled 'AI-generated, not a decision', cites sources with freshness, offers no consequential control." ]
      Gaps = [ "FORMA-GAP-01"; "FORMA-GAP-06"; "FORMA-GAP-11"; "FORMA-GAP-13"; "FORMA-GAP-15"; "FORMA-GAP-16"; "FORMA-GAP-17" ]
      Render = None }

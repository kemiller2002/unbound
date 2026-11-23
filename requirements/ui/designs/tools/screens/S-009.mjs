import { tag, loz, fresh, kv, ident, recordHeader, textarea, scopeTrail, callout, choiceGroup } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Sponsor operations", "Case CASE-2026-000231"]);
const crumbs = `<a href="S-008-exception-inbox.html">Exception inbox</a> <span aria-hidden="true">/</span> <a href="#cases">Cases</a>`;

const evidence = (p) => `<section class="forma-stack" aria-labelledby="${p}-facts"><h4 id="${p}-facts">Facts</h4>
${tag("evidence-relationship", `<article class="forma-evidence-relationship" aria-labelledby="${p}-ev1"><header><p class="forma-component-kicker">Raises</p><h5 id="${p}-ev1">Name similarity to a listed party</h5></header><div class="forma-evidence-relationship__body"><p><strong>Evidence:</strong> Recipient name scored 0.91 against a listed entity.</p><p><strong>Source:</strong> Screening provider result SR-88123</p><p><strong>Scope:</strong> Recipient legal name only</p></div><footer>${fresh({ label: "Screened", iso: "2026-11-27T08:14:00-06:00", shown: "27 Nov 2026, 8:14 AM CT", stateText: "Current", state: "ok" })}</footer></article>`)}
${tag("evidence-relationship", `<article class="forma-evidence-relationship" aria-labelledby="${p}-ev2"><header><p class="forma-component-kicker">Weakens</p><h5 id="${p}-ev2">Address and registration differ</h5></header><div class="forma-evidence-relationship__body"><p><strong>Evidence:</strong> Recipient is a domestic public charity with IRS registration since 2009.</p><p><strong>Source:</strong> IRS Business Master File</p><p><strong>Scope:</strong> EIN 00-0000210</p></div><footer>${fresh({ label: "Observed", iso: "2026-11-18", shown: "18 Nov 2026", stateText: "Current", state: "ok" })}</footer></article>`)}
</section>`;

const conflict = (p) =>
  tag(
    "comparison-pairs",
    `<section class="forma-comparison-pairs" aria-labelledby="${p}-cmp">
  <h4 id="${p}-cmp">Conflicting sources side by side</h4>
  <div class="forma-comparison-pairs__item"><div class="forma-comparison-pairs__label">Country</div><div class="forma-comparison-pairs__value" data-side="before" data-label="Screening list">Outside the United States</div><div class="forma-comparison-pairs__value" data-side="after" data-label="IRS record">Illinois, United States</div></div>
  <div class="forma-comparison-pairs__item"><div class="forma-comparison-pairs__label">Entity type</div><div class="forma-comparison-pairs__value" data-side="before" data-label="Screening list">Trading company</div><div class="forma-comparison-pairs__value" data-side="after" data-label="IRS record">501(c)(3) public charity</div></div>
</section>`
  );

const rule = (p) =>
  tag(
    "provenance-trail",
    `<section class="forma-provenance-trail" aria-labelledby="${p}-rule"><h4 id="${p}-rule">Rule that raised this case</h4><ol>
  <li><span class="forma-provenance-trail__kind">Source</span><strong>Screening result SR-88123</strong><small>27 Nov 2026</small></li>
  <li><span class="forma-provenance-trail__kind">Rule</span><strong>SAN-02 Potential sanctions match</strong><small>Rule version 4: score at least 0.85</small></li>
  <li><span class="forma-provenance-trail__kind">Effect</span><strong>Grant 2026-004102 held</strong><small>Payment blocked until a decision</small></li>
</ol></section>`
  );

const permitted = (p, preparer) =>
  tag(
    "obligation-panel",
    `<aside class="forma-obligation-panel" aria-labelledby="${p}-perm">
  <header class="forma-obligation-panel__header"><div><h4 id="${p}-perm">Actions you can take</h4><p>Your authority: A4 compliance reviewer.</p></div><span class="forma-obligation-panel__count" aria-hidden="true">3</span></header>
  <ul class="forma-obligation-list">
    <li class="forma-obligation" data-forma-severity="information"><span class="forma-obligation__status">A1</span><div><strong>Request evidence</strong><p>Asks the charity for documents. The grant stays held; SLA clock pauses.</p></div><a href="#${p}-req">Request</a></li>
    <li class="forma-obligation" data-forma-severity="${preparer ? "blocking" : "warning"}"><span class="forma-obligation__status">A4</span><div><strong>Record decision: false positive or true match</strong><p>False positive releases the hold; true match blocks the grant permanently and starts the OFAC report (due in 10 business days).</p>${preparer ? `<p id="${p}-sod"><strong>You prepared this case; another reviewer must decide.</strong></p>` : ""}</div>${preparer ? `<span>Not available</span>` : `<a href="#${p}-decide">Decide</a>`}</li>
    <li class="forma-obligation" data-forma-severity="information"><span class="forma-obligation__status">A2</span><div><strong>Escalate</strong><p>Sends the case to the compliance lead with your notes.</p></div><a href="#${p}-esc">Escalate</a></li>
  </ul>
</aside>`
  );

const decisionDialog = (p) =>
  tag(
    "dialog",
    `<button type="button" commandfor="${p}-decide" command="show-modal" id="${p}-decide-open">Record decision…</button>
<dialog class="forma-dialog" id="${p}-decide" data-forma-motion-weight="heavy" aria-labelledby="${p}-decide-title" aria-describedby="${p}-decide-description">
  <form method="dialog">
    <div class="forma-dialog__body">
      <h2 id="${p}-decide-title">Record sanctions decision for grant 2026-004102</h2>
      <p id="${p}-decide-description">This decision is final and recorded with your name. The AI summary is not part of the record.</p>
      ${choiceGroup({ name: `${p}-outcome`, legend: "Decision (required)", choices: [["fp", "False positive", "Release the hold. Payment continues to the verified destination."], ["tm", "True match", "Block the grant permanently and start the OFAC report."]] })}
      ${textarea({ id: `${p}-rationale`, label: "Written rationale (required)", desc: "Explain which evidence you relied on. At least 50 characters.", attrs: 'required minlength="50"' })}
      ${kv([["Who acts next", "False positive: payments team within 1 business day. True match: compliance lead files the report"], ["Can it be undone?", "No. A new case is needed to change it"]])}
    </div>
    <div class="forma-dialog__actions"><button type="submit" value="cancel" autofocus>Cancel</button><button type="submit" value="record">Record decision</button></div>
  </form>
</dialog>`
  );

const aiFlyout = (p) =>
  tag(
    "flyout",
    `<button type="button" commandfor="${p}-ai" command="show-modal">Explain this case</button>
<dialog class="forma-flyout" id="${p}-ai" data-forma-side="right" data-forma-motion-weight="heavy" aria-labelledby="${p}-ai-title" aria-describedby="${p}-ai-label">
  <div class="forma-flyout__surface">
    <header class="forma-flyout__header">
      <div><p class="forma-component-kicker">Assistant</p><h2 id="${p}-ai-title">Case summary</h2></div>
      <button class="forma-flyout__close" type="button" commandfor="${p}-ai" command="close" aria-label="Close case summary">Close</button>
    </header>
    <div class="forma-flyout__body">
      ${callout(`${p}-ai-label`, "caution", "AI-generated, not a decision", "<p>This summary cannot approve, release or decide anything. A reviewer must decide using the evidence.</p>")}
      ${tag("conversation", `<section class="forma-conversation" aria-labelledby="${p}-conv"><header class="forma-conversation__header"><span class="forma-component-kicker">AI-generated, not a decision</span><h3 id="${p}-conv">Summary of CASE-2026-000231</h3></header>
      <ol class="forma-conversation__turns">
        <li class="forma-turn" data-forma-speaker="user"><p class="forma-turn__speaker">You</p><div class="forma-turn__content"><p>Should this be released?</p></div></li>
        <li class="forma-turn" data-forma-speaker="assistant"><p class="forma-turn__speaker">Assistant (AI-generated)</p><div class="forma-turn__content"><p>The evidence leans toward a false positive: the IRS record shows a domestic public charity since 2009, while the listed entity is a foreign trading company. Release would be consistent with SAN-02 guidance, but the decision is yours.</p><p>Sources: screening result SR-88123 (27 Nov 2026, current); IRS Business Master File (18 Nov 2026, current).</p></div></li>
      </ol></section>`)}
      ${tag("handoff-summary", `<section class="forma-handoff-summary" aria-labelledby="${p}-ho"><header><p class="forma-component-kicker">Assistant proposal</p><h3 id="${p}-ho">Suggested outcome: false positive</h3><p>Agent: Harbor Giving case assistant</p></header><dl><div><dt>Uncertainty</dt><dd>Screening provider has not returned the secondary identifiers</dd></div><div><dt>Human authority</dt><dd>A reviewer other than the preparer must record the decision outside this panel</dd></div></dl></section>`)}
    </div>
  </div>
</dialog>`
  );

const header = (p, lozenge, actions) =>
  recordHeader({ type: "Case", title: "CASE-2026-000231", lozenge, sub: `Potential sanctions match · Grant ${ident("2026-004102")} · Opened 27 Nov 2026 · Prepared by Dana Ruiz`, crumbs, actions });

const workspace = (p, preparer) =>
  tag(
    "split-pane",
    `<section class="forma-split-pane" aria-label="Case workspace">
  <div class="forma-split-pane__primary">${evidence(p)}${conflict(p)}${rule(p)}</div>
  <aside class="forma-split-pane__secondary" aria-label="Decision and assistance">${permitted(p, preparer)}</aside>
</section>`
  );

export default {
  id: "S-009",
  file: "S-009-case-workspace.html",
  title: "Case workspace",
  reqIds: ["UNB-UI-S-009", "UNB-UI-G-011"],
  audience: "staff",
  brand: false,
  lead: "One exception: typed evidence with source and freshness, conflicting sources, the rule that raised it, permitted actions with consequences, AI summary and the decision record.",
  states: [
    {
      title: "Reviewer who did not prepare the case",
      persona: "Compliance reviewer (Priya Nair)",
      demonstrates: "UNB-UI-S-009 AC-2 (no release control in the AI flyout; decision dialog requires written rationale), UNB-UI-G-011 AC-1",
      body: (p) => `${scope}${header(p, loz("attention", "Awaiting decision"), `${decisionDialog(p)}${aiFlyout(p)}`)}${workspace(p, false)}`,
    },
    {
      title: "Preparer opens their own case",
      persona: "Compliance reviewer who prepared it (Dana Ruiz)",
      demonstrates: "UNB-UI-S-009 AC-1 (Record decision disabled with 'You prepared this case; another reviewer must decide')",
      body: (p) => `${scope}${header(p, loz("attention", "Awaiting decision"), `<button type="button" disabled aria-describedby="${p}-sod">Record decision</button>${aiFlyout(p)}`)}${workspace(p, true)}`,
    },
    {
      title: "Decided: decision record",
      persona: "Compliance reviewer",
      demonstrates: "Decision record with decider and preparer as different people",
      body: (p) => `${scope}${header(p, loz("ok", "Decided: false positive"), "")}
${callout(`${p}-dec`, "decision", "Decision record", kv([["Decision", "False positive; hold released"], ["Rationale", "IRS record shows a domestic public charity registered since 2009; listed entity is a foreign trading company with different identifiers."], ["Decided by", "Priya Nair, 1 Dec 2026, 3:12 PM CT (21:12 UTC)"], ["Prepared by", "Dana Ruiz, 27 Nov 2026"], ["Evidence considered", "SR-88123 (27 Nov 2026); IRS Business Master File (18 Nov 2026)"]]))}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-009", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 2: native disabled Record decision button described by the text 'You prepared this case; another reviewer must decide'. Segregation of duties must be enforced server-side." },
    { requirement_id: "UNB-UI-S-009", acceptance_id: "AC-2", satisfied_by_markup: true, note: "AI flyout (s009-1-ai) contains conversation and handoff-summary with no Approve/Release controls (the canonical handoff-summary action cluster was deliberately omitted). Decision dialog has a required rationale textarea (required minlength); enforcement is application behaviour." },
    { requirement_id: "UNB-UI-G-011", acceptance_id: "AC-1", satisfied_by_markup: true, note: "Flyout opened from 'Explain this case', labelled 'AI-generated, not a decision', cites sources with freshness, offers no consequential control." },
  ],
  gaps: ["FORMA-GAP-01", "FORMA-GAP-06", "FORMA-GAP-11"],
};

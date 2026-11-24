import { tag, loz, kv, field, select, validationSummary, validationMessage, dataGrid, recordHeader, scopeTrail, callout } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Sponsor policy", "Grant policy v8 (draft)"]);
const crumbs = `<a href="#policy">Sponsor policy</a> <span aria-hidden="true">/</span> <a href="#grant-policy">Grant policy</a>`;

const rules = (p, invalid) =>
  tag(
    "rule-builder",
    `<section class="forma-rule-builder" aria-labelledby="${p}-rb">
  <header class="forma-rule-builder__header"><div><h4 id="${p}-rb">Grant minimum rule</h4><p>Applies to every new grant recommendation.</p></div><button type="button">Add condition</button></header>
  <div class="forma-rule-group" data-forma-operator="and">
    <div class="forma-rule-group__operator" aria-label="All conditions must be true">AND</div>
    <div class="forma-rule-clause">
      <label><span>Field</span><select name="${p}-field"><option>Grant amount</option></select></label>
      <label><span>Operator</span><select name="${p}-op"><option>is at least</option></select></label>
      <label><span>Value</span><input type="text" name="${p}-val" id="${p}-val" value="${invalid ? "25" : "100.00"}" inputmode="decimal"${invalid ? ` aria-invalid="true" aria-describedby="${p}-val-error"` : ""}></label>
      <button type="button" aria-label="Remove condition grant amount is at least">Remove</button>
    </div>
  </div>
  ${invalid ? validationMessage(`${p}-val-error`, "Enter an amount with cents, from $50.00 to $10,000.00. The assistance contract for grant minimums does not allow $25.") : ""}
  <p class="forma-rule-builder__summary"><strong>Meaning:</strong> A grant is accepted only when its amount is at least $${invalid ? "25" : "100.00"}.</p>
</section>`
  );

const diff = (p) =>
  tag(
    "diff-viewer",
    `<section class="forma-diff-viewer" aria-labelledby="${p}-diff">
  <header class="forma-diff-viewer__header"><h4 id="${p}-diff">Changes against active version 7</h4><p>2 fields changed</p></header>
  <div class="forma-diff-viewer__grid">
    <section class="forma-diff-viewer__side" aria-label="Active version 7"><h5>Active (version 7)</h5><dl>
      <div class="forma-diff-row" data-change="changed"><dt>Minimum grant</dt><dd>$50.00</dd></div>
      <div class="forma-diff-row" data-change="changed"><dt>Recurring grants</dt><dd>Allowed, monthly or less often</dd></div>
      <div class="forma-diff-row"><dt>Pledge policy</dt><dd>Prohibited</dd></div>
    </dl></section>
    <section class="forma-diff-viewer__side" aria-label="Draft version 8"><h5>Draft (version 8)</h5><dl>
      <div class="forma-diff-row" data-change="changed"><dt>Minimum grant</dt><dd>$100.00</dd></div>
      <div class="forma-diff-row" data-change="changed"><dt>Recurring grants</dt><dd>Allowed, quarterly or less often</dd></div>
      <div class="forma-diff-row"><dt>Pledge policy</dt><dd>Prohibited</dd></div>
    </dl></section>
  </div>
</section>`
  );

const impact = (p) =>
  tag(
    "readiness-checklist",
    `<section class="forma-readiness-checklist" aria-labelledby="${p}-impact">
  <header>${loz("attention", "Impact preview")}<h4 id="${p}-impact">What this change affects</h4></header>
  <ul class="forma-readiness-checklist__items">
    <li data-state="incomplete"><span aria-hidden="true">!</span><span>Affects 214 open recurring schedules (monthly schedules move to quarterly at their next run)</span></li>
    <li data-state="incomplete"><span aria-hidden="true">!</span><span>Affects 37 draft recommendations below $100.00</span></li>
    <li data-state="complete"><span aria-hidden="true">✓</span><span>No submitted or approved grant changes</span></li>
  </ul>
</section>`
  );

const approveDialog = (p) =>
  tag(
    "dialog",
    `<button type="button" commandfor="${p}-approve" command="show-modal">Approve and schedule…</button>
<dialog class="forma-dialog" id="${p}-approve" data-forma-motion-weight="heavy" aria-labelledby="${p}-approve-title" aria-describedby="${p}-approve-description">
  <form method="dialog">
    <div class="forma-dialog__body">
      <h2 id="${p}-approve-title">Approve grant policy version 8?</h2>
      <p id="${p}-approve-description">Version 8 replaces version 7 on the effective date. It affects 214 open recurring schedules and 37 drafts.</p>
      ${tag("date-time-field", field({ id: `${p}-eff`, label: "Effective date (required)", type: "date", desc: "Midnight Central Time (America/Chicago) at the start of this date.", value: "2027-01-01", attrs: "required" }))}
      ${kv([["Can it be undone?", "Until the effective date you can withdraw approval; afterwards a new version is needed"], ["Who acts next", "No one; donors with affected schedules are notified 14 days before"]])}
    </div>
    <div class="forma-dialog__actions"><button type="submit" value="cancel" autofocus>Cancel</button><button type="submit" value="approve">Approve and schedule version 8</button></div>
  </form>
</dialog>`
  );

export default {
  id: "S-010",
  file: "S-010-policy-configuration.html",
  title: "Sponsor policy configuration and approval",
  reqIds: ["UNB-UI-S-010"],
  audience: "staff",
  brand: false,
  lead: "Draft a policy change, see it as a diff with its impact, and have a different authorized approver schedule it.",
  states: [
    {
      title: "Draft with live validation",
      persona: "Sponsor administrator (author: Lee Tran)",
      demonstrates: "Validation from assistance contracts as the user edits (UNB-UI-G-012)",
      body: (p) => `${scope}${recordHeader({ type: "Grant policy", title: "Version 8", lozenge: loz(null, "Draft"), sub: "Author Lee Tran · last saved 1 Dec 2026, 3:40 PM CT", crumbs, actions: '<button type="button">Save draft</button><button type="button" disabled aria-describedby="' + p + '-why">Submit for approval</button>' })}
<p id="${p}-why">Fix the problem below before submitting for approval.</p>
<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft"><h4 id="${p}-ft">Edit grant policy</h4>
${validationSummary({ id: `${p}-vs`, title: "There is 1 problem to fix", items: [[`#${p}-val`, "Minimum grant must be from $50.00 to $10,000.00."]] })}
${rules(p, true)}
<div class="forma-form-row" data-forma-form-layout="grid">
${select({ id: `${p}-recur`, label: "Recurring grants", options: [["monthly", "Allowed, monthly or less often"], ["quarterly", "Allowed, quarterly or less often", true], ["none", "Not allowed"]] })}
${select({ id: `${p}-pledge`, label: "Pledge policy", options: [["prohibited", "Prohibited", true], ["allowed", "Allowed with disclosure"]] })}
</div></form>`,
    },
    {
      title: "In review: the author views their own draft",
      persona: "Sponsor administrator who authored it",
      demonstrates: "UNB-UI-S-010 AC-1 (Approve unavailable with the reason shown); diff and impact preview",
      body: (p) => `${scope}${recordHeader({ type: "Grant policy", title: "Version 8", lozenge: loz("attention", "In review: waiting on an approver"), sub: "Author Lee Tran · submitted 1 Dec 2026, 4:05 PM CT", crumbs, actions: `<button type="button" disabled aria-describedby="${p}-why">Approve and schedule</button><button type="button">Withdraw from review</button>` })}
<p id="${p}-why"><strong>You wrote this draft, so another approver must approve it.</strong></p>
${diff(p)}${impact(p)}`,
    },
    {
      title: "In review: a different approver",
      persona: "Sponsor approver (Morgan Ellis)",
      demonstrates: "Approve and schedule (A4) with effective date and consequences; Reject",
      body: (p) => `${scope}${recordHeader({ type: "Grant policy", title: "Version 8", lozenge: loz("attention", "In review: waiting on you"), sub: "Author Lee Tran · submitted 1 Dec 2026, 4:05 PM CT", crumbs, actions: `${approveDialog(p)}<button type="button">Reject…</button>` })}${diff(p)}${impact(p)}`,
    },
    {
      title: "Version history",
      persona: "Sponsor administrator",
      demonstrates: "Approved (scheduled), Active and Superseded states",
      body: (p) => `${scope}${dataGrid({ label: "Grant policy versions", cols: [["Version"], ["Status"], ["Effective"], ["Approved by"]], rows: [
        ["8", loz("attention", "Approved: scheduled"), "1 Jan 2027", "Morgan Ellis"],
        ["7", loz("ok", "Active"), "1 Jul 2026", "Morgan Ellis"],
        ["6", loz(null, "Superseded"), "1 Jan 2026 to 30 Jun 2026", "Casey Wong"],
      ] })}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-010", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 2: Approve and schedule is a native disabled button described by 'You wrote this draft, so another approver must approve it.' Author/approver separation must be enforced server-side." },
    { requirement_id: "UNB-UI-S-010", acceptance_id: "statement (diff + impact)", satisfied_by_markup: true, note: "diff-viewer against version 7 and readiness-checklist impact preview 'Affects 214 open recurring schedules'. Forma has no dedicated impact-preview primitive; readiness-checklist items used with incomplete state (semantic stretch, see FORMA-GAP-11 note)." },
    { requirement_id: "UNB-UI-S-010", acceptance_id: "statement (validation as user edits)", satisfied_by_markup: "needs-application-behaviour", note: "Inline validation-message inside rule-builder shown; running assistance-contract validation on edit is application behaviour." },
  ],
  gaps: ["FORMA-GAP-01", "FORMA-GAP-06", "FORMA-GAP-11"],
};

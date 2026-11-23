import { tag, loz, kv, ident, field, select, dialog, dataGrid, validationSummary, validationMessage, opStatus, scopeTrail } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Lindqvist Family Fund (F-2026-000418)", "People"]);

const revoke = (p) =>
  dialog({
    id: `${p}-revoke`,
    invoker: "Revoke…",
    invokerAttrs: 'aria-label="Revoke access for Jordan Pike"',
    title: "Revoke Jordan Pike's access now?",
    desc: "Jordan Pike loses access to this fund immediately, including any session that is open now.",
    body: kv([
      ["What changes", "Advisor access (view and recommend grants) ends at once; drafts Jordan started are closed"],
      ["Can it be undone?", "No. To restore access you must invite Jordan again"],
      ["Who acts next", "No one. Example Charitable Foundation is notified for its records"],
      ["Grants already submitted", "Unaffected; they continue through approval"],
    ]),
    actions: `<button type="submit" value="cancel" autofocus>Keep access</button><button type="submit" value="revoke">Revoke Jordan Pike's access</button>`,
  });

const invite = (p) =>
  dialog({
    id: `${p}-invite`,
    invoker: "Invite participant…",
    title: "Invite someone to this fund",
    desc: "They receive an email invitation that expires in 7 days.",
    body: `${field({ id: `${p}-email`, label: "Email address (required)", type: "email", attrs: 'required autocomplete="off"' })}
${select({ id: `${p}-role`, label: "Role", options: [["advisor", "Advisor: view and recommend grants", true], ["viewer", "Viewer: view only"], ["co-donor", "Co-donor"]], desc: "Roles come from Example Charitable Foundation's role catalogue." })}
${select({ id: `${p}-scope`, label: "Scope", options: [["fund", "This fund only", true]] })}`,
    actions: `<button type="submit" value="cancel">Cancel</button><button type="submit" value="invite">Send invitation</button>`,
  });

const people = (p) =>
  dataGrid({
    label: "Participants",
    cols: [["Name"], ["Role"], ["Scope"], ["Effective"], ["Status"], ["Actions", null, true]],
    rows: [
      ["Avery Lindqvist", "Primary donor", "Fund F-2026-000418", "Since 3 Feb 2025", loz("ok", "Active"), "—"],
      ["Jordan Pike, Pike Wealth Partners", "Advisor", "Fund F-2026-000418", "Since 12 Mar 2025", loz("ok", "Active"), `<span class="forma-cluster"><button type="button" aria-label="Change scope for Jordan Pike">Change scope</button>${revoke(p)}</span>`],
      ["Sam Okafor", "Viewer", "Fund F-2026-000418", "Invited 28 Nov 2026", loz("attention", "Invited: waiting on Sam"), `<button type="button" aria-label="Cancel invitation for Sam Okafor">Cancel invitation</button>`],
      ["Morgan Hale", "Advisor", "Fund F-2026-000418", "12 Mar 2025 to 30 Sep 2026", loz("blocked", "Revoked"), "—"],
    ],
  });

const allocation = (p, a, b, invalid) =>
  tag(
    "allocation",
    `<fieldset class="forma-allocation" aria-describedby="${p}-alloc-help${invalid ? ` ${p}-alloc-error` : ""}">
  <legend class="forma-allocation__legend">Divide 100% of the fund between your successors</legend>
  <div class="forma-allocation__items">
    <label class="forma-allocation__item"><span>Riley Lindqvist (successor advisor, new fund)</span><input type="number" name="${p}-riley" min="0" max="100" step="1" value="${a}" inputmode="numeric" id="${p}-riley"${invalid ? ' aria-invalid="true"' : ""}></label>
    <label class="forma-allocation__item"><span>Hope Foundation, EIN 00-0000101 (charity beneficiary)</span><input type="number" name="${p}-hope" min="0" max="100" step="1" value="${b}" inputmode="numeric"${invalid ? ' aria-invalid="true"' : ""}></label>
  </div>
  <div class="forma-allocation__total" data-forma-state="${invalid ? "invalid" : "valid"}" aria-live="polite" aria-atomic="true"><span>Total</span><output>${a + b}% of 100%</output></div>
  <p class="forma-field__description" id="${p}-alloc-help">Percentages must total exactly 100%.</p>
  ${invalid ? validationMessage(`${p}-alloc-error`, `Allocations total ${a + b}%. Assign the remaining ${100 - a - b}% before saving.`) : ""}
</fieldset>`
  );

const planText = (p, a, b) => `<section class="forma-stack" aria-labelledby="${p}-plan"><h3 id="${p}-plan">Successor plan in words</h3>
<p>When Example Charitable Foundation verifies the death of Avery Lindqvist, the fund is suspended and then divided:</p>
<ol>
  <li>${a}% goes to a new fund advised by Riley Lindqvist (successor advisor).</li>
  <li>${b}% is granted to Hope Foundation, EIN ${ident("00-0000101")}, Springfield, IL.</li>
</ol>
<p>Plan version 3, saved 14 Oct 2026. Example Charitable Foundation must approve the plan when it takes effect.</p></section>`;

export default {
  id: "S-006",
  file: "S-006-participants-successors.html",
  title: "People, delegation and successors",
  reqIds: ["UNB-UI-S-006", "UNB-UI-G-005"],
  audience: "donor",
  brand: true,
  lead: "Who can act on this fund, in what role and scope, and who succeeds you.",
  states: [
    {
      title: "Participants and revoke confirmation",
      persona: "Primary donor",
      demonstrates: "Role, scope, effective dates and status per participant; invite; revoke (A2, immediate) with G-005 dialog",
      body: (p) => `${scope}<div class="forma-actions">${invite(p)}</div>${people(p)}`,
    },
    {
      title: "Successors: allocations do not total 100%",
      persona: "Primary donor",
      demonstrates: "UNB-UI-S-006 AC-1 (validation blocks saving and states the 10% remainder)",
      body: (p) => `${scope}<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft"><h3 id="${p}-ft">Designate successors</h3>
${validationSummary({ id: `${p}-vs`, title: "The plan cannot be saved yet", items: [[`#${p}-riley`, "Allocations total 90%. Assign the remaining 10% before saving."]] })}
${allocation(p, 60, 30, true)}
<div class="forma-actions"><button type="submit">Save successor plan</button><button type="button">Cancel</button></div></form>`,
    },
    {
      title: "Successors designated, with the text equivalent",
      persona: "Primary donor",
      demonstrates: "Successor plan shown as text, not only a diagram",
      body: (p) => `${scope}${allocation(p, 60, 40, false)}${planText(p, 60, 40)}`,
    },
    {
      title: "Fund suspended: successor activation",
      persona: "Sponsor staff",
      demonstrates: "Suspended (verified death), Successor designated, Successor active statuses",
      body: (p) => `${scope}${opStatus({ id: `${p}-s`, state: "blocked", label: "Suspended: verified death", symbol: "×", title: "Fund suspended on 18 Nov 2026", body: "<p>Grants and contributions are stopped until the successor plan is activated.</p>" })}
${dataGrid({ label: "Successors", cols: [["Successor"], ["Share"], ["Status"]], rows: [
  ["Riley Lindqvist", "60%", loz("attention", "Successor designated: invitation sent")],
  ["Hope Foundation, EIN 00-0000101", "40%", loz("ok", "Successor active: grant approved")],
] })}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-006", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 2: allocation total data-forma-state=invalid, message 'Allocations total 90%. Assign the remaining 10% before saving.' and summary. Computing the total live and blocking save are Limen/application behaviour." },
    { requirement_id: "UNB-UI-G-005", acceptance_id: "revoke", satisfied_by_markup: true, note: "Revoke dialog states what changes, irreversibility, who acts next; confirm names the action. No destructive visual variant (FORMA-GAP-01)." },
    { requirement_id: "UNB-UI-S-006", acceptance_id: "statement (text equivalent)", satisfied_by_markup: true, note: "State 3 'Successor plan in words' ordered list. Forma has no read-only allocation summary; composed from plain list (FORMA-GAP-12)." },
  ],
  gaps: ["FORMA-GAP-01", "FORMA-GAP-06", "FORMA-GAP-12"],
};

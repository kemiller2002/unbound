// S-008 Sponsor work queue, v2. One page per state.
// Data first, then pure renderers. Synthetic data only.
// Local helpers (ref, metrics, field) are exported for the other ops/charity modules and are
// proposed for promotion to tools/lib/ui.mjs (see proposed/ops-charity-widget.css).
import { money, chip, panel, notice, button, quiet, skel, icon, time, pagehead, dl } from "../lib/ui.mjs";

export const CSS = ["proposed/ops-charity-widget.css"];
const PREPARER = { name: "Dana Ruiz", initials: "DR", role: "Preparer" };
const NOW = ["2026-12-01T16:55:00-06:00", "today at 4:55 PM CT"];

/** Facts list for side rails: the term column keeps a readable width. */
export const facts = (rows) => dl(rows).replace('class="unb-dl"', 'class="unb-dl unb-x-facts"');

// ---------- local helpers (promote to shared layer) ----------
/** Copyable staff reference: monospace identifier + 24px+ copy button (Forma identifier has no copy affordance). */
export const ref = (id, label = "reference") =>
  `<span class="unb-x-ref"><code>${id}</code><button class="unb-x-ref__copy" type="button" aria-label="Copy ${label} ${id}">${icon("copy")}</button></span>`;

/** Metric strip: label, value, one line of context. items: {label, value, context, state} */
export const metrics = (items, label) =>
  `<ul class="unb-x-metrics" aria-label="${label}">${items
    .map((m) => `<li class="unb-x-metric"${m.state ? ` data-state="${m.state}"` : ""}><span class="unb-x-metric__label">${m.label}</span><span class="unb-x-metric__value">${m.value}</span>${m.context ? `<span class="unb-x-metric__context">${m.context}</span>` : ""}</li>`)
    .join("")}</ul>`;

/** Labelled native select. */
export const select = (id, label, options, selected) =>
  `<div class="unb-x-field"><label for="${id}">${label}</label><select id="${id}">${options.map((o) => `<option${o === selected ? " selected" : ""}>${o}</option>`).join("")}</select></div>`;

// ---------- queue data ----------
const TYPES = ["Reconciliation", "Payments", "Eligibility", "Sanctions", "Contributions", "Bank details"];

const ITEMS = [
  { sla: chip("blocked", "Over SLA"), slaText: "1 d 2 h over", title: "Possible sanctions match on payee", subject: "Grant to City Arts Council", ref: "SAN-2026-00231", type: "Sanctions", owner: "Unassigned", age: "4 d 2 h", amount: 7500 },
  { sla: chip("blocked", "Over SLA"), slaText: "6 h over", title: "Bank and ledger differ by $2,500.00", subject: "Grant disbursement account ••••0034", ref: "REC-2026-00117", type: "Reconciliation", owner: "You", age: "3 d 6 h", amount: 2500, href: "#rec-2026-00117" },
  { sla: chip("attention", "Due today"), slaText: "4 h left", title: "Charity status not confirmed", subject: "Grant to Northgate Literacy Project", ref: "ELG-2026-00240", type: "Eligibility", owner: "Unassigned", age: "1 d 20 h", amount: 2500 },
  { sla: chip("attention", "Due today"), slaText: "7 h left", title: "Payment returned by charity's bank", subject: "Grant to Riverside Food Bank", ref: "PAY-2026-00233", type: "Payments", owner: "Lee Tran", age: "1 d 2 h", amount: 2500 },
  { sla: chip("attention", "Due tomorrow"), slaText: "1 d left", title: "Deposit with no matching contribution", subject: "Operating cash account ••••0012", ref: "REC-2026-00119", type: "Reconciliation", owner: "Unassigned", age: "1 d 1 h", amount: 5000 },
  { sla: chip("attention", "Waiting on custodian"), slaText: "4 d left", title: "Securities only partly received", subject: "Contribution to Lindqvist Family Fund", ref: "CON-2026-00391", type: "Contributions", owner: "Lee Tran", age: "6 d", amount: 18240.55 },
  { sla: chip("neutral", "Within SLA"), slaText: "4 d 20 h left", title: "Charity asked to change bank details", subject: "Riverside Food Bank", ref: "DST-2026-00412", type: "Bank details", owner: "Unassigned", age: "4 h", amount: null },
  { sla: chip("neutral", "Within SLA"), slaText: "2 d left", title: "Custody fee posted twice", subject: "Custody account ••••0056", ref: "REC-2026-00121", type: "Reconciliation", owner: "Unassigned", age: "3 h", amount: 35 },
  { sla: chip("neutral", "Within SLA"), slaText: "2 d 18 h left", title: "Payment outcome unknown", subject: "Grant to City Arts Council", ref: "PAY-2026-00236", type: "Payments", owner: "You", age: "2 h", amount: 12345.67 },
];

const countBy = (items) => TYPES.map((t) => [t, items.filter((i) => i.type === t).length]);

// ---------- regions ----------
const summary = () =>
  metrics(
    [
      { label: "Over SLA", value: "2", context: "Oldest 1 d 2 h over", state: "blocked" },
      { label: "Due today", value: "2", context: "Both due by 9:00 PM CT", state: "attention" },
      { label: "Unassigned", value: "5", context: "Of 9 open items" },
      { label: "Straight-through, last 7 days", value: "94.2%", context: "1,812 of 1,923 grants needed no one" },
    ],
    "Queue summary"
  );

const toolbar = (items) =>
  `<div class="unb-x-toolbar">
<div class="unb-x-filters" role="group" aria-label="Show items of type">
<button class="unb-x-filter" type="button" aria-pressed="true">All <span class="unb-num">${items.length}</span></button>
${countBy(items)
  .map(([t, n]) => `<button class="unb-x-filter" type="button" aria-pressed="false">${t} <span class="unb-num">${n}</span></button>`)
  .join("")}
</div>
<div class="unb-x-toolbar__end">${select("owner", "Owner", ["Anyone", "Me", "Unassigned"], "Anyone")}${select("sort", "Sort by", ["SLA risk", "Age", "Amount"], "SLA risk")}</div>
</div>`;

const th = (label, col, sorted) =>
  `<th scope="col" data-col="${col}"${sorted ? ' aria-sort="ascending"' : ""}><button class="unb-x-sort" type="button">${label}${icon("sort")}</button></th>`;

const queueTable = (items) =>
  `<table class="unb-x-queue"><caption class="forma-visually-hidden">Open work items, most at risk of missing the SLA first</caption>
<thead><tr>${th("SLA", "sla", true)}${th("Item", "item")}${th("Type", "type")}${th("Owner", "owner")}${th("Age", "age")}${th("Amount", "amount")}</tr></thead>
<tbody>${items
    .map(
      (i) => `<tr${i.href ? ' data-mine="true"' : ""}><td data-col="sla">${i.sla}<span class="unb-x-queue__sub">${i.slaText}</span></td>
<td data-col="item"><a class="unb-x-queue__title" href="${i.href ?? `#${i.ref.toLowerCase()}`}">${i.title}</a><span class="unb-x-queue__sub">${i.subject} · <span class="unb-x-mono">${i.ref}</span></span></td>
<td data-col="type">${i.type}</td><td data-col="owner">${i.owner === "Unassigned" ? `<span class="unb-muted">Unassigned</span>` : i.owner}</td>
<td data-col="age"><span class="unb-num">${i.age}</span></td>
<td data-col="amount">${i.amount === null ? `<span aria-hidden="true">—</span><span class="forma-visually-hidden">No amount</span>` : money(i.amount)}</td></tr>`
    )
    .join("")}</tbody></table>`;

const queuePanel = (items) =>
  panel({
    id: "q",
    title: "Open items",
    meta: `<span class="unb-small unb-muted">${items.length} items · updated ${time(NOW[0], NOW[1])}</span>`,
    body: `${toolbar(items)}${queueTable(items)}`,
    foot: `<span class="unb-small unb-muted">Items close on their own when their exit condition is met. Routine work never appears here.</span>${quiet("Recently closed", { href: "#closed", iconName: "chevron-right" })}`,
  });

const queueHead = (sub = `Example Community Foundation · ${time(NOW[0], "Updated " + NOW[1])}`) =>
  pagehead({ title: "Work queue", sub, actions: [button("Take next item", { primary: true, href: "#next" })] });

const base = (extra = {}) => ({ title: "Work queue", nav: "inbox", audience: "staff", user: PREPARER, brand: "unbound-default", notifications: 3, extraCss: CSS, counts: { inbox: 9 }, ...extra });

// ---------- reconciliation break detail ----------
const BREAK = {
  ref: "REC-2026-00117",
  account: "Grant disbursement account ••••0034",
  bank: { closing: 1204330.12, out: 38750 },
  ledger: { closing: 1201830.12, out: 41250 },
};

const compare = () =>
  `<div class="unb-x-compare">
<section class="unb-x-compare__side" aria-labelledby="cmp-bank"><h3 id="cmp-bank">${icon("database")}Bank statement</h3><p class="unb-small unb-muted">Prairie Community Bank, received ${time("2026-12-01T06:02:00-06:00", "Dec 1, 6:02 AM CT")}</p>
${dl([["Balance at Nov 30 close", money(BREAK.bank.closing)], ["Payments out on Nov 30", money(BREAK.bank.out)], ["Payments counted", '<span class="unb-num">14</span>']])}</section>
<section class="unb-x-compare__side" aria-labelledby="cmp-ledger"><h3 id="cmp-ledger">${icon("file-text")}Our ledger</h3><p class="unb-small unb-muted">Grant payments recorded through Nov 30</p>
${dl([["Balance at Nov 30 close", money(BREAK.ledger.closing)], ["Payments out on Nov 30", money(BREAK.ledger.out)], ["Payments counted", '<span class="unb-num">15</span>']])}</section>
<p class="unb-x-compare__diff"><span>Difference</span>${money(2500)}<span class="unb-small">The bank holds more than our ledger shows: one payment we recorded did not leave the account.</span></p>
</div>`;

const CANDIDATES = [
  { id: "c1", title: "Returned payment: grant to Riverside Food Bank", text: `Bank entry “RETURN R02 ACCOUNT CLOSED”, ${money(2500)}, value date ${time("2026-11-30", "Nov 30, 2026")}. Same amount and payee reference as grant 2026-004790.`, strength: chip("ok", "Strong match"), checked: true },
  { id: "c2", title: "Grant to Lakeview Animal Rescue, not yet debited", text: `Same amount (${money(2500)}), but scheduled for ${time("2026-12-03", "Dec 3")}; the bank shows no entry for it yet.`, strength: chip("neutral", "Weak match") },
];

const candidates = (proposed = false) =>
  `<fieldset class="unb-x-choices"${proposed ? " disabled" : ""}><legend>Matching candidates</legend>${CANDIDATES.map(
    (c) => `<div class="unb-x-choice"><input type="radio" name="cand" id="${c.id}"${c.checked ? " checked" : ""}><label for="${c.id}"><span class="unb-x-choice__title">${c.title}</span>${c.strength}<span class="unb-x-choice__text">${c.text}</span></label></div>`
  ).join("")}</fieldset>`;

/** Who does what: two duties, two different people (segregation of duties made visible). */
const authority = (steps) =>
  `<ol class="unb-x-duties">${steps
    .map((s) => `<li data-step="${s.step}"${s.step === "current" ? ' aria-current="step"' : ""}><span class="unb-x-duties__role">${s.role}</span><span class="unb-x-duties__who">${s.who}</span>${s.note ? `<span class="unb-x-duties__note">${s.note}</span>` : ""}</li>`)
    .join("")}</ol>`;

const RESOLUTIONS = [
  ["Match to the returned payment", chip("ok", "You can propose"), "Reverses the Nov 30 grant payment in the ledger. The $2,500.00 goes back to the Lindqvist Family Fund's Available to grant and the donor is told."],
  ["Mark as bank error and query the bank", chip("ok", "You can propose"), "Keeps the ledger as is and opens a query with Prairie Community Bank. Use this only if no candidate fits."],
  ["Write off the difference", chip("blocked", "Controller only"), "Write-offs over $100.00 need a controller to propose them. You can't choose this."],
];

const resolvePanel = () =>
  panel({
    id: "res",
    title: "Resolve this break",
    body: `${authority([
      { step: "current", role: "1 · Propose", who: "You (Dana Ruiz, preparer)" },
      { step: "todo", role: "2 · Approve", who: "A different approver", note: "Sam Okafor or Priya Nair. The person who proposes can never approve." },
    ])}
<div class="unb-x-options" role="list" aria-label="Allowed resolutions">${RESOLUTIONS.map(([t, c, d]) => `<div role="listitem" class="unb-x-option"><p class="unb-x-option__title">${t}</p>${c}<p class="unb-x-option__text">${d}</p></div>`).join("")}</div>
<div class="unb-x-actions">${button("Propose match for approval", { primary: true })}${button("Query the bank instead")}</div>
<p class="unb-small unb-muted">Nothing changes in the ledger until an approver accepts. You can withdraw a proposal until then.</p>`,
  });

const proposedPanel = () =>
  panel({
    id: "res",
    title: "Resolve this break",
    tone: "attention",
    body: `<p><strong>You proposed:</strong> match to the returned payment to Riverside Food Bank, on ${time("2026-12-01T17:12:00-06:00", "Dec 1, 2026 at 5:12 PM CT")}.</p>
${authority([
  { step: "done", role: "1 · Propose", who: "You (Dana Ruiz, preparer)", note: `Done ${time("2026-12-01T17:12:00-06:00", "Dec 1, 5:12 PM CT")}` },
  { step: "current", role: "2 · Approve", who: "Sam Okafor or Priya Nair", note: "Asked to approve. Usually within 4 business hours." },
])}
${notice({ state: "info", iconName: "lock", title: "You can't approve your own proposal", body: `<p>You prepared this resolution, so a different approver has to check and accept it. This separation of duties is required for every ledger change; it protects you as well as the fund.</p>` })}
<div class="unb-x-actions">${button("Withdraw proposal")}</div>`,
  });

const blockedPanel = () =>
  panel({
    id: "blk",
    title: "While this is open",
    body: dl([
      ["Close November books", chip("blocked", "Blocked")],
      ["Publish November statements for 412 funds", chip("blocked", "Blocked")],
      ["New grants to Riverside Food Bank", chip("attention", "Held for bank details")],
      ["Grant payments from this account", chip("ok", "Continue")],
      ["Donor balances and grant approvals", chip("ok", "Continue")],
    ]),
    foot: `<span class="unb-small unb-muted">November close is due ${time("2026-12-07", "Dec 7, 2026")}.</span>`,
  });

const factsPanel = () =>
  panel({
    id: "facts",
    title: "Details",
    body: facts([
      ["Reference", ref(BREAK.ref)],
      ["Created by rule", "REC-01 daily bank match"],
      ["Created", time("2026-11-28T10:55:00-06:00", "Nov 28, 2026, 10:55 AM CT")],
      ["SLA", "3 business days · 6 h over"],
      ["Owner", "Dana Ruiz (you)"],
      ["Closes when", "Bank and ledger agree for Nov 30"],
    ]),
  });

const explain = () =>
  panel({
    id: "what",
    title: "What happened",
    body: `<p>On Nov 30 the bank shows ${money(2500)} less going out of the grant disbursement account than our ledger recorded. The most likely reason: the payment to Riverside Food Bank was returned because the charity's account is closed, so the money never left.</p>${compare()}${candidates()}`,
  });

const explainProposed = () =>
  panel({
    id: "what",
    title: "What happened",
    body: `<p>On Nov 30 the bank shows ${money(2500)} less going out of the grant disbursement account than our ledger recorded. The payment to Riverside Food Bank was returned because the charity's account is closed.</p>${compare()}${candidates(true)}`,
  });

const detailHead = (status) =>
  pagehead({
    context: [`<a href="#inbox">Work queue</a>`, "Reconciliation", "Item 2 of 9"],
    title: "Bank and ledger differ by $2,500.00",
    sub: `<span class="unb-x-headmeta">${status}<span>${BREAK.account} · Nov 30 statement</span></span>`,
    actions: [quiet("Next item", { href: "#next", iconName: "chevron-right" })],
  });

const detail = (main, side) => `<div class="unb-x-detail"><div class="unb-x-detail__main">${main}</div><div class="unb-x-detail__side">${side}</div></div>`;

// ---------- states ----------
export const states = [
  {
    id: "queue",
    title: "Exception-first queue sorted by SLA risk",
    persona: "Sponsor operations preparer",
    scenarios: ["normal", "attention-required", "sla-breach"],
    page: { ...base(), pagehead: queueHead(), body: `${summary()}${queuePanel(ITEMS)}` },
  },
  {
    id: "reconciliation-break",
    title: "Reconciliation break selected: bank vs ledger, candidates and who can resolve",
    persona: "Sponsor operations preparer (owner of the item)",
    scenarios: ["reconciliation-exception", "segregation-of-duties", "attention-required"],
    page: {
      ...base(),
      pagehead: detailHead(chip("blocked", "Over SLA by 6 h")),
      body: detail(`${explain()}${resolvePanel()}`, `${blockedPanel()}${factsPanel()}`),
    },
  },
  {
    id: "proposed-by-me",
    title: "Same break after proposing: preparer cannot approve own proposal",
    persona: "Sponsor operations preparer who proposed",
    scenarios: ["segregation-of-duties", "pending-approval", "read-only"],
    page: {
      ...base(),
      pagehead: detailHead(chip("attention", "Waiting for approval")),
      body: detail(`${proposedPanel()}${explainProposed()}`, `${blockedPanel()}${factsPanel()}`),
    },
  },
  {
    id: "empty",
    title: "Empty queue: nothing needs you",
    persona: "Sponsor operations preparer",
    scenarios: ["empty"],
    page: {
      ...base({ counts: {}, notifications: 0 }),
      pagehead: pagehead({ title: "Work queue", sub: "Example Community Foundation" }),
      body: `${panel({
        id: "empty",
        label: "Queue status",
        body: `<div class="unb-x-empty">${icon("success")}<p class="unb-x-empty__title">Nothing needs you right now</p><p class="unb-muted">Every item the rules created is resolved. Last checked ${time(NOW[0], NOW[1])}; we check again every 5 minutes and new items appear here.</p></div>`,
      })}${panel({
        id: "closed",
        title: "Closed automatically today",
        meta: `<span class="unb-small unb-muted">3 items</span>`,
        body: `<ul class="unb-x-closed">
<li><span class="unb-x-closed__title">Charity status confirmed: Northgate Literacy Project</span>${chip("ok", "Closed: exit condition met")}<span class="unb-small unb-muted">New IRS record confirmed public-charity status at ${time("2026-12-01T16:40:00-06:00", "4:40 PM CT")}. No one had to act.</span></li>
<li><span class="unb-x-closed__title">Deposit matched to contribution: Lindqvist Family Fund</span>${chip("ok", "Closed: exit condition met")}<span class="unb-small unb-muted">Contribution record arrived at ${time("2026-12-01T14:05:00-06:00", "2:05 PM CT")} and matched ${money(5000)}.</span></li>
<li><span class="unb-x-closed__title">Payment outcome confirmed: City Arts Council</span>${chip("ok", "Closed: exit condition met")}<span class="unb-small unb-muted">Bank confirmed the payment at ${time("2026-12-01T11:30:00-06:00", "11:30 AM CT")}.</span></li>
</ul>`,
        foot: quiet("All recently closed", { href: "#closed", iconName: "chevron-right" }),
      })}`,
    },
  },
  {
    id: "loading",
    title: "Loading: skeleton matches the queue layout",
    persona: "Sponsor operations preparer",
    scenarios: ["loading"],
    page: {
      ...base({ counts: {} }),
      pagehead: queueHead("Example Community Foundation"),
      body: `<div class="unb-stack" aria-busy="true" aria-describedby="loading-msg"><p id="loading-msg" class="forma-visually-hidden" role="status">Loading the work queue</p>
<ul class="unb-x-metrics" aria-hidden="true">${[1, 2, 3, 4].map(() => `<li class="unb-x-metric">${skel("line-short")}${skel("value")}${skel("line")}</li>`).join("")}</ul>
${panel({ id: "q", title: "Open items", body: `<div class="unb-x-toolbar" aria-hidden="true">${skel("line")}</div>${[1, 2, 3, 4, 5, 6].map(() => skel("row")).join("")}` })}</div>`,
    },
  },
];

export default {
  id: "S-008",
  slug: "S-008-work-queue",
  title: "Sponsor work queue",
  requirement: "UNB-UI-S-008",
  states,
};

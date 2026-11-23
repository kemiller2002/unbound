// S-012 Charity view, v2. One page per state.
// Data first, then pure renderers. Synthetic data only.
import { money, chip, panel, notice, button, quiet, icon, time, pagehead, dl, tasks, steps } from "../lib/ui.mjs";
import { CSS, facts } from "./S-008.mjs";

const USER = { name: "Jamie Cole", initials: "JC", role: "Riverside Food Bank" };
const SPONSOR = "Example Community Foundation";
const ORG_SUB = "Riverside Food Bank · Springfield, IL";

const GRANTS = [
  { id: "g-004822", from: "Lindqvist Family Fund", by: "Recommended by Elin Lindqvist", amount: 5000, status: chip("ok", "Approved"), when: `Payment scheduled ${time("2026-12-03", "Dec 3, 2026")}`, ack: null },
  { id: "g-004790", from: "Lindqvist Family Fund", by: "Recommended by Elin Lindqvist", amount: 2500, status: chip("ok", "Paid"), when: `Paid ${time("2026-11-30", "Nov 30, 2026")}`, ack: chip("attention", "Not acknowledged") },
  { id: "g-004702", from: `Anonymous donor via ${SPONSOR}`, by: "The donor chose not to be named", amount: 10000, status: chip("ok", "Paid"), when: `Paid ${time("2026-11-20", "Nov 20, 2026")}`, ack: chip("neutral", "Acknowledged Nov 21") },
];

const records = (list) =>
  `<ul class="unb-x-records">${list
    .map(
      (g) => `<li class="unb-x-record"><span class="unb-x-record__title"><a href="#${g.id}">${g.from}</a><br><span class="unb-small unb-muted">${g.by}</span></span>${money(g.amount)}
<span class="unb-x-record__meta">${g.status}${g.ack ?? ""}<span>${g.when}</span></span></li>`
    )
    .join("")}</ul>`;

const base = (extra = {}) => ({ title: "Grants received", nav: "received", audience: "charity", user: USER, brand: "unbound-default", notifications: 1, extraCss: CSS, ...extra });

const listState = {
  ...base(),
  pagehead: pagehead({ title: "Grants received", sub: ORG_SUB }),
  body: `${panel({
    id: "todo",
    title: "To do",
    tone: "attention",
    body: tasks([
      {
        state: "attention",
        iconName: "email-open",
        title: `Acknowledge ${money(2500)} from the Lindqvist Family Fund`,
        text: `Paid ${time("2026-11-30", "Nov 30, 2026")}. Acknowledging lets ${SPONSOR} and the donor know it arrived.`,
        actions: [button("Review and acknowledge", { href: "#g-004790" })],
      },
    ]),
  })}
${panel({
  id: "list",
  title: "Grants",
  meta: `<span class="unb-small unb-muted">${money(17500)} in 2026 · 3 grants</span>`,
  body: records(GRANTS),
  foot: `${quiet("Download 2026 remittances", { href: "#remit", iconName: "download" })}<span class="unb-small unb-muted">${SPONSOR} is the legal donor of every grant.</span>`,
})}
${panel({ id: "pay", title: "Where we pay you", body: dl([["ACH to Prairie Community Bank ••••7710", chip("ok", "Verified")]]), foot: quiet("Change bank details", { href: "#bank", iconName: "chevron-right" }) })}`,
};

const detailState = {
  ...base({ notifications: 0 }),
  pagehead: pagehead({ context: [`<a href="#received">Grants received</a>`, "Lindqvist Family Fund"], title: "Grant from the Lindqvist Family Fund", sub: ORG_SUB }),
  body: `<div class="unb-x-detail"><div class="unb-x-detail__main">
${panel({
  id: "sum",
  label: "Grant summary",
  body: `<div class="unb-cluster">${chip("ok", "Paid")}<span class="unb-small unb-muted">Paid ${time("2026-11-30", "Nov 30, 2026")} to ACH ••••7710</span></div>
<p class="unb-x-amount">${money(2500, { hero: true })}</p>
${steps([["Approved", "done"], ["Paid", "done"], ["Acknowledged", "current"]])}`,
})}
${panel({
  id: "ack",
  title: "Acknowledge this grant",
  body: `<p class="unb-small">This tells ${SPONSOR} and Elin Lindqvist that the grant arrived. It is not a tax receipt: ${SPONSOR} is the legal donor and has already received what it needs.</p>
<div class="unb-x-field"><label for="ack-msg">Message to the donor (optional)</label><textarea id="ack-msg" aria-describedby="ack-hint"></textarea><span class="unb-x-field__hint" id="ack-hint">Shared with Elin Lindqvist through ${SPONSOR}. Don't include goods or services offered in return.</span></div>
<label class="unb-x-check" for="ack-use"><input type="checkbox" id="ack-use" checked><span>We'll use this grant for general operating support, as recommended.</span></label>
<div class="unb-x-actions">${button("Acknowledge receipt", { primary: true })}</div>`,
})}
</div><div class="unb-x-detail__side">
${panel({
  id: "facts",
  title: "Grant details",
  body: facts([
    ["Legal donor", `${SPONSOR}<br><span class="unb-small unb-muted">EIN <span class="unb-num">00-0000001</span></span>`],
    ["Recommended by", "Elin Lindqvist, Lindqvist Family Fund"],
    ["Purpose", "General operating support"],
    ["Approved", time("2026-11-26", "Nov 26, 2026")],
    ["Paid", time("2026-11-30", "Nov 30, 2026")],
    ["On your bank statement", `<span class="unb-x-mono">ECF GRANT 2026-004790</span>`],
  ]),
  foot: quiet("Download remittance (PDF)", { href: "#remit", iconName: "download" }),
})}
</div></div>`,
};

const reverifyState = {
  ...base({ notifications: 2 }),
  pagehead: pagehead({ title: "Grants received", sub: ORG_SUB }),
  body: `${notice({
    state: "blocked",
    iconName: "refresh",
    title: "Your bank returned a payment. Please confirm your bank details",
    body: `<p>On ${time("2026-12-02", "Dec 2, 2026")} Prairie Community Bank returned our ${money(2500)} payment with the reason “account closed”. To protect your grants we've paused payments to account ••••7710 until new details are verified.</p>`,
    actions: [button("Update bank details", { primary: true, href: "#bank-new" })],
  })}
<div class="unb-x-detail"><div class="unb-x-detail__main">
${panel({
  id: "waiting",
  title: "Grants waiting for verified bank details",
  meta: `<span class="unb-small unb-muted">${money(7500)} in total</span>`,
  body: records([
    { id: "g-004790", from: "Lindqvist Family Fund", by: "Returned by your bank", amount: 2500, status: chip("blocked", "Payment returned"), when: "Will be sent again once verified" },
    { id: "g-004822", from: "Lindqvist Family Fund", by: "Recommended by Elin Lindqvist", amount: 5000, status: chip("attention", "Waiting for bank details"), when: `Was scheduled for ${time("2026-12-03", "Dec 3, 2026")}` },
  ]),
  foot: `<span class="unb-small unb-muted">Nothing is lost. The donor's fund keeps the money set aside for you while we verify, for up to 60 days.</span>`,
})}
</div><div class="unb-x-detail__side">
${panel({
  id: "how",
  title: "How verification works",
  body: `<ol class="unb-checklist">
<li data-step="current" aria-current="step"><p class="unb-checklist__title">You send new bank details</p><p class="unb-small unb-muted">With a bank letter or voided check. About 5 minutes.</p></li>
<li><p class="unb-checklist__title">We call you to confirm</p><p class="unb-small unb-muted">${SPONSOR} calls the phone number on your IRS record, never a number from the request.</p></li>
<li><p class="unb-checklist__title">Payments resume</p><p class="unb-small unb-muted">Usually within 3 business days of your request.</p></li>
</ol>`,
})}
${panel({ id: "pay", title: "Where we pay you", body: dl([["ACH to Prairie Community Bank ••••7710", chip("blocked", "Paused: payment returned")]]) })}
</div></div>`,
};

export const states = [
  { id: "grants-received", title: "Grants received, one waiting for acknowledgment", persona: "Charity finance user", scenarios: ["normal", "attention-required", "anonymous-donor"], page: listState },
  { id: "grant-detail", title: "One grant's detail with acknowledgment", persona: "Charity finance user", scenarios: ["normal", "acknowledgment"], page: { ...detailState, title: "Grant detail" } },
  { id: "bank-reverify", title: "Bank details need re-verification after a returned payment", persona: "Charity finance user", scenarios: ["failed-payment", "restricted", "attention-required"], page: reverifyState },
];

export default {
  id: "S-012",
  slug: "S-012-charity-view",
  title: "Charity view",
  requirement: "UNB-UI-S-012",
  states,
};

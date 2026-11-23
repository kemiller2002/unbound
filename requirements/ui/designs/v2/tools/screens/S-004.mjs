// S-004 Grant detail, v2. One page per state.
// Data first, then pure renderers. Synthetic data only (same fund and charities as S-001).
// Local components (proposed, see proposed/donor-grants.css): record header, milestone
// track, event timeline, decision options, copyable reference, document list,
// unavailable-action-with-reason. track() and kv() are also used by S-003.
import { money, chip, panel, notice, button, quiet, icon, time } from "../lib/ui.mjs";

const DONOR = { name: "Elin Lindqvist", initials: "EL", role: null };
const ADVISOR = { name: "Jordan Patel", initials: "JP", role: "View only" };
const SPONSOR = "Example Community Foundation";
const FUND = "Lindqvist Family Fund";

// ---------- local helpers (promote to shared layer) ----------
const STATE_ICON = { ok: "success", attention: "pending", blocked: "error", info: "info", unknown: "help-circle", neutral: "clock" };

/** Milestone track: [{label, when, step: done|current|todo|failed}] -> ordered list with a date line each. */
export const track = (items, label = "Progress") =>
  `<ol class="unb-x-track" aria-label="${label}">${items
    .map(
      (m) => `<li data-step="${m.step}"${m.step === "current" ? ' aria-current="step"' : ""}><span class="unb-x-track__label">${m.label}${
        m.step === "done" ? '<span class="forma-visually-hidden">, done</span>' : m.step === "failed" ? '<span class="forma-visually-hidden">, problem</span>' : m.step === "current" ? '<span class="forma-visually-hidden">, current step</span>' : ""
      }</span><span class="unb-x-track__when">${m.when}</span></li>`
    )
    .join("")}</ol>`;

/** Stacked key-value list for long values. rows: [key, value] */
export const kv = (rows) => `<dl class="unb-x-kv">${rows.map(([k, v]) => `<div><dt>${k}</dt><dd>${v}</dd></div>`).join("")}</dl>`;

/** Event timeline, newest first. events: {state, iconName?, when:[iso, shown], title, text?} */
const timeline = (events, label) =>
  `<ol class="unb-x-timeline" aria-label="${label}">${events
    .map(
      (e) => `<li data-state="${e.state}"><span class="unb-x-timeline__mark" aria-hidden="true">${icon(e.iconName ?? STATE_ICON[e.state])}</span>
<p class="unb-x-timeline__when">${time(e.when[0], e.when[1])}</p><p class="unb-x-timeline__title">${e.title}</p>${e.text ? `<p class="unb-x-timeline__text">${e.text}</p>` : ""}</li>`
    )
    .join("")}</ol>`;

const ref = (n) =>
  `<span class="unb-x-ref">Grant <span class="unb-num">${n}</span><button class="unb-x-copy" type="button" aria-label="Copy grant number ${n}">${icon("copy")}</button></span>`;

const docs = (items) =>
  `<ul class="unb-x-docs">${items
    .map((d) =>
      d.href
        ? `<li>${icon("file-text")}<a href="${d.href}">${d.title}</a><span>${d.text}</span></li>`
        : `<li data-state="pending">${icon("clock")}<p>${d.title}</p><span>${d.text}</span></li>`
    )
    .join("")}</ul>`;

const unavailable = ({ title, text, action = "" }) =>
  `<div class="unb-x-unavailable">${icon("lock")}<p class="unb-x-unavailable__title">${title}</p><p>${text}</p>${action ? `<div>${action}</div>` : ""}</div>`;

const d = (iso, shown) => time(iso, shown);
const ct = (iso, shown) => [iso, shown];

// ---------- data ----------
const G = {
  review: {
    no: "2026-004830",
    charity: "City Arts Council",
    place: "Chicago, IL",
    ein: "00-0000207",
    amount: 7500,
    chip: chip("attention", "In review"),
    next: `${SPONSOR} usually decides within 2 business days, by ${d("2026-12-02", "Dec 2, 2026")}.`,
    track: [
      { label: "Submitted", when: d("2026-11-30", "Nov 30, 2026"), step: "done" },
      { label: "Sponsor review", when: `Decision expected by ${d("2026-12-02", "Dec 2")}`, step: "current" },
      { label: "Approved", when: "Not yet", step: "todo" },
      { label: "Paid", when: `Expected by ${d("2026-12-09", "Dec 9")}`, step: "todo" },
    ],
    purpose: "2027 youth theater season",
    recognition: "Elin Lindqvist",
    events: [
      { state: "ok", when: ct("2026-11-30T15:13:00-06:00", "Nov 30, 2026, 3:13 PM CT"), title: "Charity confirmed eligible", text: "City Arts Council is a registered public charity (IRS records of Nov 30, 2026)." },
      { state: "neutral", iconName: "send", when: ct("2026-11-30T15:12:00-06:00", "Nov 30, 2026, 3:12 PM CT"), title: "Recommendation submitted by Elin Lindqvist", text: `${money(7500)} moved from Available to grant into grants in progress.` },
    ],
  },
  approved: {
    no: "2026-004822",
    charity: "Riverside Food Bank",
    place: "Springfield, IL",
    ein: "00-0000103",
    amount: 5000,
    chip: chip("ok", "Approved"),
    next: `Payment scheduled for ${d("2026-12-03", "Dec 3, 2026")} by bank transfer.`,
    track: [
      { label: "Submitted", when: d("2026-11-24", "Nov 24, 2026"), step: "done" },
      { label: "Sponsor review", when: d("2026-11-26", "Nov 26, 2026"), step: "done" },
      { label: "Approved", when: d("2026-11-26", "Nov 26, 2026"), step: "done" },
      { label: "Paid", when: `Scheduled ${d("2026-12-03", "Dec 3")}`, step: "current" },
    ],
    purpose: "General operating support",
    recognition: "Elin Lindqvist",
    events: [
      { state: "info", iconName: "calendar", when: ct("2026-11-26T09:31:00-06:00", "Nov 26, 2026, 9:31 AM CT"), title: "Payment scheduled for Dec 3, 2026", text: "By bank transfer to Riverside Food Bank's verified account." },
      { state: "ok", when: ct("2026-11-26T09:30:00-06:00", "Nov 26, 2026, 9:30 AM CT"), title: `Approved by ${SPONSOR}` },
      { state: "ok", when: ct("2026-11-24T10:05:00-06:00", "Nov 24, 2026, 10:05 AM CT"), title: "Charity confirmed eligible", text: "Registered public charity (IRS records of Nov 23, 2026)." },
      { state: "neutral", iconName: "send", when: ct("2026-11-24T10:02:00-06:00", "Nov 24, 2026, 10:02 AM CT"), title: "Recommendation submitted by Elin Lindqvist" },
    ],
  },
  paid: {
    no: "2026-004760",
    charity: "Riverside Food Bank",
    place: "Springfield, IL",
    ein: "00-0000103",
    amount: 2500,
    amountText: `2.5% of fund value at approval → ${money(2500)}<small>Fixed ${d("2026-11-20", "Nov 20, 2026")}, when the fund was worth ${money(100000)}</small>`,
    chip: chip("ok", "Paid"),
    next: `Paid ${d("2026-11-30", "Nov 30, 2026")}. Riverside Food Bank acknowledged it on ${d("2026-12-01", "Dec 1")}.`,
    track: [
      { label: "Submitted", when: d("2026-11-18", "Nov 18, 2026"), step: "done" },
      { label: "Sponsor review", when: d("2026-11-20", "Nov 20, 2026"), step: "done" },
      { label: "Approved", when: d("2026-11-20", "Nov 20, 2026"), step: "done" },
      { label: "Paid", when: d("2026-11-30", "Nov 30, 2026"), step: "done" },
    ],
    purpose: "Holiday meal program",
    recognition: "Anonymous",
    events: [
      { state: "ok", iconName: "email-open", when: ct("2026-12-01T11:20:00-06:00", "Dec 1, 2026, 11:20 AM CT"), title: "Riverside Food Bank acknowledged the grant" },
      { state: "ok", when: ct("2026-11-30T14:00:00-06:00", "Nov 30, 2026, 2:00 PM CT"), title: `Paid: ${money(2500)} received by the charity's bank`, text: "Bank transfer to the account ending 2210, confirmed by the receiving bank." },
      { state: "ok", when: ct("2026-11-20T09:15:00-06:00", "Nov 20, 2026, 9:15 AM CT"), title: `Approved by ${SPONSOR}`, text: `Amount fixed at ${money(2500)} (2.5% of ${money(100000)}).` },
      { state: "neutral", iconName: "send", when: ct("2026-11-18T16:40:00-06:00", "Nov 18, 2026, 4:40 PM CT"), title: "Recommendation submitted by Elin Lindqvist" },
    ],
  },
  returned: {
    no: "2026-004790",
    charity: "Riverside Food Bank",
    place: "Springfield, IL",
    ein: "00-0000103",
    amount: 2500,
    chip: chip("blocked", "Payment returned"),
    next: `Waiting for your choice by ${d("2026-12-16", "Dec 16, 2026")}.`,
    track: [
      { label: "Submitted", when: d("2026-11-19", "Nov 19, 2026"), step: "done" },
      { label: "Approved", when: d("2026-11-23", "Nov 23, 2026"), step: "done" },
      { label: "Payment sent", when: d("2026-11-25", "Nov 25, 2026"), step: "done" },
      { label: "Payment returned", when: d("2026-12-02", "Dec 2, 2026"), step: "failed" },
    ],
    purpose: "General operating support",
    recognition: "Elin Lindqvist",
    events: [
      { state: "info", iconName: "refresh", when: ct("2026-12-02T10:16:00-06:00", "Dec 2, 2026, 10:16 AM CT"), title: `${money(2500)} returned to Available to grant` },
      { state: "blocked", when: ct("2026-12-02T10:15:00-06:00", "Dec 2, 2026, 10:15 AM CT"), title: "Payment returned by the charity's bank", text: "The bank account it was sent to has been closed." },
      { state: "ok", when: ct("2026-11-25T14:00:00-06:00", "Nov 25, 2026, 2:00 PM CT"), title: `Payment of ${money(2500)} sent by bank transfer` },
      { state: "ok", when: ct("2026-11-23T09:00:00-06:00", "Nov 23, 2026, 9:00 AM CT"), title: `Approved by ${SPONSOR}` },
      { state: "neutral", iconName: "send", when: ct("2026-11-19T13:22:00-06:00", "Nov 19, 2026, 1:22 PM CT"), title: "Recommendation submitted by Elin Lindqvist" },
    ],
  },
  hold: {
    no: "2026-004811",
    charity: "Northgate Literacy Project",
    place: "Gary, IN",
    ein: "00-0000311",
    amount: 2500,
    chip: chip("blocked", "On hold: charity eligibility"),
    next: `We expect to finish checking by ${d("2026-12-04", "Dec 4, 2026")}.`,
    track: [
      { label: "Submitted", when: d("2026-11-25", "Nov 25, 2026"), step: "done" },
      { label: "Charity check", when: `On hold since ${d("2026-11-27", "Nov 27")}`, step: "failed" },
      { label: "Approved", when: "Not yet", step: "todo" },
      { label: "Paid", when: "Not yet", step: "todo" },
    ],
    purpose: "Summer reading program",
    recognition: "Elin Lindqvist",
    events: [
      { state: "blocked", when: ct("2026-11-27T08:40:00-06:00", "Nov 27, 2026, 8:40 AM CT"), title: "On hold: we couldn't confirm the charity's tax-exempt status", text: "Northgate Literacy Project no longer appears in the latest IRS list of public charities. This is often a filing delay." },
      { state: "neutral", iconName: "send", when: ct("2026-11-25T19:05:00-06:00", "Nov 25, 2026, 7:05 PM CT"), title: "Recommendation submitted by Elin Lindqvist" },
    ],
  },
};

// ---------- regions ----------
const header = (g) => `<header class="unb-x-record">
<nav class="unb-x-record__crumbs" aria-label="Breadcrumb"><a href="#grants">Grants</a><span aria-hidden="true">/</span>${ref(g.no)}</nav>
<div class="unb-x-record__row"><div class="unb-x-record__who"><h1>${g.charity}</h1><p class="unb-small unb-muted">${g.place} · From ${FUND}</p></div>
<p class="unb-x-record__amount"><span class="forma-visually-hidden">Amount: </span>${money(g.amount, { hero: true })}</p></div>
<div class="unb-x-record__status">${g.chip}<p class="unb-x-record__next">${g.next}</p></div>
</header>`;

const progress = (g, foot = "") => panel({ id: "prog", title: "Progress", body: track(g.track, `Progress of grant ${g.no}`), foot });

const details = (g) =>
  panel({
    id: "det",
    title: "Grant details",
    body: kv([
      ["Amount", g.amountText ?? money(g.amount)],
      ["Charity", `${g.charity}<small>${g.place} · EIN <span class="unb-num">${g.ein}</span></small>`],
      ["Purpose", g.purpose],
      ["Recognition", `${g.recognition}<small>${g.recognition === "Anonymous" ? "The charity sees “Anonymous donor via " + SPONSOR + "”" : "Name shown to the charity"}</small>`],
      ["From", FUND],
    ]),
  });

const history = (g) => panel({ id: "hist", title: "History", meta: `<span class="unb-small unb-muted">Newest first · Central Time</span>`, body: timeline(g.events, `History of grant ${g.no}`) });

const documents = (items) => panel({ id: "docs", title: "Documents", body: docs(items) });

const DOC_CONFIRM = (g, date) => ({ href: `#doc-confirm-${g.no}`, title: "Grant confirmation (PDF)", text: `Issued ${date}` });
const DOC_REMIT_PENDING = { title: "Payment remittance", text: "Available once the grant is paid" };

const layout = (main, side) => `<div class="unb-x-flow" data-layout="record"><div>${main}</div><div>${side}</div></div>`;

const base = (brand = "unbound-default") => ({ nav: "grants", audience: "donor", user: DONOR, brand, notifications: 2 });

const page = ({ g, title, main, side, user, brand }) => ({
  ...base(brand),
  ...(user ? { user, notifications: 0 } : {}),
  title: `${g.charity}, grant ${g.no}`,
  docTitle: title,
  pagehead: header(g),
  body: layout(main, side),
});

// Withdraw is offered only while the sponsor has not approved (A2: confirmation dialog).
const withdrawFoot = () =>
  `<p class="unb-small unb-muted">You can withdraw this recommendation until it's approved.</p>${button(`Withdraw recommendation`, { id: "withdraw" })}`;

const withdrawDialog = (g) => `<dialog class="forma-dialog" id="withdraw-dialog" aria-labelledby="wd-title" aria-describedby="wd-desc">
<form method="dialog">
<div class="forma-dialog__body">
<h2 id="wd-title">Withdraw your ${money(g.amount)} grant to ${g.charity}?</h2>
<p id="wd-desc">The recommendation closes now and the money goes back to your fund.</p>
<dl class="unb-x-consequences">
<div>${icon("refresh")}<dt>What changes</dt><dd>${money(g.amount)} returns to Available to grant right away. ${g.charity} hasn't been told about this grant, so nothing is sent to them.</dd></div>
<div>${icon("warning")}<dt>Can it be undone?</dt><dd>No. You can recommend a new grant to ${g.charity} at any time.</dd></div>
<div>${icon("users")}<dt>Who acts next</dt><dd>No one. ${SPONSOR} stops reviewing it.</dd></div>
</dl>
</div>
<div class="forma-dialog__actions"><button class="forma-button" type="submit" value="keep" autofocus>Keep the grant</button><button class="forma-button" type="submit" value="withdraw" data-unb-x-tone="danger">Withdraw recommendation</button></div>
</form>
</dialog>
<script>document.getElementById("withdraw-dialog").showModal();</script>`;

const approvedFoot = unavailable({
  title: "Withdraw is no longer available",
  text: `${SPONSOR} approved this grant on ${d("2026-11-26", "Nov 26, 2026")}. You can still ask them to cancel it before ${d("2026-12-03", "Dec 3")}, but payment may already be in progress and can't always be stopped.`,
  action: quiet("Request cancellation", { href: "#request-cancellation", iconName: "chevron-right" }),
});

// ---------- states ----------
const inReview = (extra = "") =>
  page({
    g: G.review,
    title: "In review",
    main: `${progress(G.review, withdrawFoot())}${history(G.review)}${extra}`,
    side: `${details(G.review)}${documents([DOC_CONFIRM(G.review, d("2026-11-30", "Nov 30, 2026")), DOC_REMIT_PENDING])}`,
  });

const approvedPage = (who) =>
  page({
    g: G.approved,
    title: "Approved",
    user: who,
    main: `${progress(G.approved, who ? unavailable({ title: "View only", text: `Elin Lindqvist shared this fund with you. Only the fund's advisors can withdraw, cancel or change grants. Your access ends ${d("2027-06-30", "Jun 30, 2027")}.` }) : approvedFoot)}${history(G.approved)}`,
    side: `${details(G.approved)}${documents([DOC_CONFIRM(G.approved, d("2026-11-26", "Nov 26, 2026")), DOC_REMIT_PENDING])}`,
  });

const paid = page({
  g: G.paid,
  title: "Paid",
  main: `${progress(G.paid)}${panel({
    id: "proof",
    title: "Proof of payment",
    body: kv([
      ["Paid on", `${d("2026-11-30", "Nov 30, 2026")}<small>Confirmed by the charity's bank at 2:00 PM CT</small>`],
      ["Paid by", `Bank transfer<small>To Riverside Food Bank's verified account ending <span class="unb-num">2210</span></small>`],
      ["Acknowledgment", `Received ${d("2026-12-01", "Dec 1, 2026")}<small>Riverside Food Bank confirmed it received the grant</small>`],
    ]),
    foot: `<span class="unb-small unb-muted">This grant isn't tax-deductible for you; your deduction was for the original contribution to the fund.</span>`,
  })}${history(G.paid)}`,
  side: `${details(G.paid)}${documents([
    { href: "#doc-remit-2026-004760", title: "Payment remittance (PDF)", text: `Sent to the charity ${d("2026-11-30", "Nov 30, 2026")}` },
    { href: "#doc-ack-2026-004760", title: "Charity acknowledgment (PDF)", text: `From Riverside Food Bank, ${d("2026-12-01", "Dec 1, 2026")}` },
    DOC_CONFIRM(G.paid, d("2026-11-20", "Nov 20, 2026")),
  ])}`,
});

const returned = page({
  g: G.returned,
  title: "Payment returned",
  main: `${panel({
    id: "choose",
    title: "The charity's bank returned this payment",
    tone: "attention",
    body: `<p>On ${d("2026-12-02", "Dec 2, 2026")} Riverside Food Bank's bank sent the ${money(2500)} back because the account had been closed. The money is back in your Available to grant. Choose what should happen next.</p>
<ul class="unb-x-options">
<li class="unb-x-option"><h3>Wait for new bank details</h3><p>We ask Riverside Food Bank for a new account and pay it once it's verified, usually within 10 business days. ${money(2500)} is set aside for this grant again until then. You can change your mind while you wait.</p>${button("Wait for new details", { href: "#wait" })}</li>
<li class="unb-x-option"><h3>Send it to a different charity</h3><p>This grant closes and ${money(2500)} stays in Available to grant. You then choose a charity in a new recommendation, which ${SPONSOR} reviews as usual.</p>${button("Choose a different charity", { href: "#redirect" })}</li>
</ul>
<p class="unb-small unb-muted">If you don't choose by ${d("2026-12-16", "Dec 16, 2026")}, we'll wait for new bank details.</p>`,
  })}${progress(G.returned)}${history(G.returned)}`,
  side: `${details(G.returned)}${documents([DOC_CONFIRM(G.returned, d("2026-11-23", "Nov 23, 2026")), { href: "#doc-return-2026-004790", title: "Return notice (PDF)", text: `From the charity's bank, ${d("2026-12-02", "Dec 2, 2026")}` }])}`,
});

const hold = page({
  g: G.hold,
  title: "On hold for charity eligibility",
  main: `${notice({
    state: "blocked",
    iconName: "pending",
    title: "On hold while we check the charity",
    body: `<p>On ${d("2026-11-27", "Nov 27, 2026")} we couldn't confirm that Northgate Literacy Project is still a tax-exempt public charity. ${SPONSOR} is checking with the charity and IRS records. Nothing has been paid, and you don't need to do anything to wait.</p>
<p>If it's confirmed, the grant continues to approval. If it can't be confirmed by ${d("2026-12-11", "Dec 11, 2026")}, the grant is declined and ${money(2500)} returns to Available to grant.</p>`,
  })}${progress(G.hold, `<p class="unb-small unb-muted">Rather not wait? Withdraw it and choose another charity.</p>${button("Withdraw recommendation", { id: "withdraw" })}`)}${history(G.hold)}`,
  side: `${details(G.hold)}${documents([DOC_REMIT_PENDING])}`,
});

const withdrawConfirm = { ...inReview(withdrawDialog(G.review)), docTitle: "Withdraw confirmation" };

export const states = [
  { id: "in-review", title: "In review: progress with expected dates, withdraw available", persona: "Donor (primary advisor)", scenarios: ["pending-grants", "normal"], page: inReview() },
  { id: "withdraw-confirm", title: "Withdraw: consequential confirmation dialog", persona: "Donor (primary advisor)", scenarios: ["withdraw", "confirmation"], page: withdrawConfirm },
  { id: "approved", title: "Approved, payment scheduled; withdraw unavailable with reason", persona: "Donor (primary advisor)", scenarios: ["approved", "action-unavailable"], page: approvedPage() },
  { id: "paid", title: "Paid: proof of payment and charity acknowledgment (percentage grant)", persona: "Donor (primary advisor)", scenarios: ["paid", "percentage-grant"], page: paid },
  { id: "payment-returned", title: "Failed payment: returned by the charity's bank, wait or redirect", persona: "Donor (primary advisor)", scenarios: ["failed-payment", "attention-required"], page: returned },
  { id: "on-hold", title: "On hold: charity eligibility", persona: "Donor (primary advisor)", scenarios: ["eligibility-hold", "attention-required"], page: hold },
  { id: "advisor-view", title: "Read-only: financial advisor with view access", persona: "Financial advisor (view only)", scenarios: ["read-only"], page: approvedPage(ADVISOR) },
];

export default {
  id: "S-004",
  slug: "S-004-grant-detail",
  title: "Grant detail",
  requirement: "UNB-UI-S-004",
  states: states.map((s) => ({ ...s, page: { ...s.page, extraCss: ["proposed/donor-grants.css"] } })),
};

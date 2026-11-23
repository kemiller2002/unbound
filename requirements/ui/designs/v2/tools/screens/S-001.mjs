// S-001 Giving home, v2 (report 18). One page per state.
// Data first, then pure renderers. Synthetic data only.
import { money, chip, panel, notice, button, quiet, tasks, ledger, steps, skel, icon, time, pagehead, dl } from "../lib/ui.mjs";

const DONOR = { name: "Elin Lindqvist", initials: "EL", role: null };
const FUND = "Lindqvist Family Fund";

const fund = {
  available: 100000,
  committed: 15000,
  settling: 5000,
  value: 120000,
  updated: ["2026-12-01T16:58:00-06:00", "today at 4:58 PM CT"],
  priced: ["2026-11-30T15:00:00-06:00", "Nov 30, 2026 market close"],
};

const pct = (part, whole) => `${((part / whole) * 100).toFixed(1)}%`;

// ---------- regions ----------
const balance = ({ f = fund, valueState = "current", availableState = "current", sponsor }) => {
  const stale = availableState === "stale";
  const meta = stale
    ? `<span>${chip("unknown", "Not current")}</span><span>Last confirmed ${time("2026-11-27T16:58:00-06:00", "Nov 27, 4:58 PM CT")}</span>`
    : `<span>${icon("success")} Confirmed by ${sponsor}</span><span>Updated ${time(f.updated[0], f.updated[1])}</span>`;
  const valueRow =
    valueState === "unavailable"
      ? `<li data-part="total"><span class="unb-alloc__key" data-part="total"></span><span>Total fund value<small>Investment values are temporarily unavailable</small></span>${chip("unknown", "Unavailable")}</li>`
      : `<li data-part="total"><span class="unb-alloc__key" data-part="total"></span><span>Total fund value<small>Market value, ${time(f.priced[0], f.priced[1])}</small></span>${money(f.value)}</li>`;
  const bar =
    valueState === "unavailable" || f.value === 0
      ? ""
      : `<div class="unb-alloc__bar" aria-hidden="true"><span class="unb-alloc__seg" data-part="available" style="inline-size:${pct(f.available, f.value)}"></span><span class="unb-alloc__seg" data-part="committed" style="inline-size:${pct(f.committed, f.value)}"></span><span class="unb-alloc__seg" data-part="settling" style="inline-size:${pct(f.settling, f.value)}"></span></div>`;
  return panel({
    id: "bal",
    label: "Fund balance",
    body: `<div class="unb-balance">
<div><p class="unb-balance__label" id="bal-label">Available to grant</p>
<p class="unb-balance__value" aria-describedby="bal-label">${money(f.available, { hero: true })}</p>
<p class="unb-balance__meta">${meta}</p></div>
<div class="unb-alloc">${bar}
<ul class="unb-alloc__legend" aria-label="How your fund is made up">
<li><span class="unb-alloc__key" data-part="available"></span><span>Available to grant</span>${money(f.available)}</li>
<li><span class="unb-alloc__key" data-part="committed"></span><span>Committed to grants in progress<small>Leaves your fund when each grant is paid</small></span>${money(f.committed)}</li>
<li><span class="unb-alloc__key" data-part="settling"></span><span>Contributions still settling<small>Available to grant on ${time("2026-12-08", "Dec 8, 2026")}</small></span>${money(f.settling)}</li>
${valueRow}
</ul></div></div>`,
    foot: `${quiet("How we calculate this", { href: "#calc", iconName: "chevron-right" })}<span class="unb-small unb-muted unb-hide-phone">Only Available to grant can be recommended.</span>`,
  });
};

const attention = (items) =>
  panel({
    id: "att",
    title: "Needs your attention",
    meta: `<span class="unb-small unb-muted">${items.length} ${items.length === 1 ? "item" : "items"}</span>`,
    tone: "attention",
    body: tasks(items),
  });

const T = {
  certify: {
    state: "attention",
    iconName: "clipboard",
    title: "Confirm your 2026 donor certification",
    text: `Due ${time("2026-12-15", "Dec 15, 2026")}. Confirm that no grant from this fund gave you or your family a personal benefit.`,
    actions: [button("Review and confirm", { href: "#certify" })],
  },
  hold: {
    state: "blocked",
    title: "A grant is on hold: Northgate Literacy Project",
    text: `We couldn't confirm the charity's tax-exempt status on ${time("2026-11-27", "Nov 27")}. You can wait while we check, or choose a different charity.`,
    actions: [button("Review options", { href: "#grant-004811" })],
  },
  returned: {
    state: "blocked",
    iconName: "refresh",
    title: "A payment was returned: Riverside Food Bank",
    text: `The charity's bank returned the ${money(2500)} payment on ${time("2026-12-02", "Dec 2")}. The money is back in Available to grant. Choose whether to wait for new bank details or send it elsewhere.`,
    actions: [button("Review options", { href: "#grant-004790" })],
  },
};

const GRANTS = [
  { to: "Riverside Food Bank", id: "Grant 2026-004822", amount: 5000, chip: chip("ok", "Approved"), next: `Payment scheduled ${time("2026-12-03", "Dec 3")}`, steps: [["Submitted", "done"], ["Approved", "done"], ["Payment scheduled", "current"]] },
  { to: "City Arts Council", id: "Grant 2026-004830", amount: 7500, chip: chip("attention", "In review"), next: "Usually approved within 2 business days", steps: [["Submitted", "done"], ["In review", "current"], ["Paid", "todo"]] },
  { to: "Northgate Literacy Project", id: "Grant 2026-004811", amount: 2500, chip: chip("blocked", "On hold"), next: "Charity status being checked", steps: [["Submitted", "done"], ["On hold", "failed"], ["Paid", "todo"]] },
];

const grantsInProgress = (list = GRANTS) =>
  panel({
    id: "gip",
    title: "Grants in progress",
    meta: `<span class="unb-small unb-muted">${money(list.reduce((s, g) => s + g.amount, 0))} committed</span>`,
    body: `<ul class="unb-tasks">${list
      .map(
        (g) => `<li class="unb-task unb-grant-row"><span class="unb-task__icon unb-task__icon--neutral">${icon("send")}</span>
<div class="unb-split"><p class="unb-task__title"><a href="#${g.id.replace(/\s/g, "-").toLowerCase()}">${g.to}</a></p>${money(g.amount)}</div>
<div class="unb-split"><span>${g.chip}</span><span class="unb-small unb-muted">${g.next}</span></div></li>`
      )
      .join("")}</ul>`,
    foot: quiet("All grants", { href: "#grants", iconName: "chevron-right" }),
  });

const ACTIVITY = [
  { what: "Grant paid", sub: "Riverside Food Bank", status: chip("ok", "Paid"), date: time("2026-11-30", "Nov 30, 2026"), amount: money(-2500) },
  { what: "Contribution received", sub: "Bank transfer from ••••4821", status: chip("attention", "Settling"), date: time("2026-11-24", "Nov 24, 2026"), amount: money(5000, { signed: true }) },
  { what: "Grant paid", sub: "Lakeview Animal Rescue", status: chip("ok", "Paid"), date: time("2026-11-12", "Nov 12, 2026"), amount: money(-1000) },
];

const activity = (rows = ACTIVITY) =>
  panel({
    id: "act",
    title: "Recent activity",
    body: ledger({ caption: "Recent activity, newest first", rows }),
    foot: quiet("All activity and statements", { href: "#activity", iconName: "chevron-right" }),
  });

const ACTIONS = [button("Add money", { href: "#add-money" }), button("Recommend a grant", { primary: true, href: "#recommend" })];
const head = (extra = {}) => pagehead({ title: FUND, fundSwitch: true, sub: extra.sub ?? "Donor-advised fund · You are the primary advisor", actions: extra.actions ?? ACTIONS });

// Priority order in the source: balance (what can I give), attention (what needs me), then the rest.
// On a 390x844 phone the headline figure and the attention heading both fall in the first viewport.
const home = ({ attention: att = "", left, main = "", right }) =>
  `<div class="unb-grid-home"><div data-area="balance">${left}</div>${att ? `<div data-area="attention">${att}</div>` : ""}${main ? `<div data-area="main">${main}</div>` : ""}<div data-area="side">${right}</div></div>`;

const sponsorOf = (brand) => (brand === "example-harbor" ? "Example Charitable Foundation" : "Example Community Foundation");

// ---------- states ----------
const base = (brand) => ({ title: "Home", nav: "home", audience: "donor", user: DONOR, brand, notifications: 2 });

const normal = (brand) => ({
  ...base(brand),
  pagehead: head(),
  body: home({ left: balance({ sponsor: sponsorOf(brand) }), main: grantsInProgress(), attention: attention([T.certify, T.hold]), right: `${activity()}` }),
  mobileActions: ACTIONS.join(""),
});

export const states = [
  {
    id: "normal",
    title: "Normal: pending grants and two attention items",
    persona: "Donor (primary advisor)",
    scenarios: ["normal", "pending-grants", "attention-required"],
    page: normal("unbound-default"),
  },
  {
    id: "white-label",
    title: "White-labelled sponsor (Harbor Giving brand manifest)",
    persona: "Donor (primary advisor)",
    scenarios: ["white-label", "normal"],
    page: normal("example-harbor"),
  },
  {
    id: "loading",
    title: "Loading: skeleton matches the final layout",
    persona: "Donor",
    scenarios: ["loading"],
    page: {
      ...base("unbound-default"),
      notifications: 0,
      pagehead: head(),
      body: `<div aria-busy="true" aria-describedby="loading-msg"><p id="loading-msg" class="forma-visually-hidden" role="status">Loading your fund</p>${home({
        left: `${panel({ id: "bal", label: "Fund balance, loading", body: `<div class="unb-balance"><div>${skel("line-short")}<p>${skel("value")}</p>${skel("line-short")}</div><div class="unb-alloc">${skel("bar")}${skel("line")}${skel("line")}${skel("line")}</div></div>` })}${panel({ id: "gip", title: "Grants in progress", body: `${skel("row")}${skel("row")}${skel("row")}` })}`,
        right: `${panel({ id: "act", title: "Recent activity", body: `${skel("row")}${skel("row")}${skel("row")}` })}`,
      })}</div>`,
      mobileActions: ACTIONS.join(""),
    },
  },
  {
    id: "new-fund",
    title: "New fund: no contributions yet (onboarding)",
    persona: "Donor, fund opened today",
    scenarios: ["no-contributions", "no-grants"],
    page: {
      ...base("unbound-default"),
      notifications: 0,
      pagehead: head({ actions: [button("Add money", { primary: true, href: "#add-money" })] }),
      body: home({
        left: panel({
          id: "start",
          title: "Get your fund started",
          meta: `<span class="unb-small unb-muted">1 of 3 done</span>`,
          body: `<ol class="unb-checklist">
<li data-step="done"><p class="unb-checklist__title">Open your fund</p><p class="unb-small unb-muted">Opened ${time("2026-12-01", "Dec 1, 2026")}. Your fund agreement is in Documents.</p></li>
<li data-step="current" aria-current="step"><p class="unb-checklist__title">Add your first contribution</p><p class="unb-small unb-muted">Bank transfer, wire, check or securities. Bank transfers are available to grant about 10 business days after they arrive.</p><div class="unb-task__action">${button("Add money", { primary: true, href: "#add-money" })}</div></li>
<li><p class="unb-checklist__title">Recommend your first grant</p><p class="unb-small unb-muted">You can look up charities now; recommendations open once money is available.</p><div class="unb-task__action">${quiet("Find a charity", { href: "#charities", iconName: "search" })}</div></li>
</ol>`,
        }),
        right: `${panel({ id: "bal0", title: "Available to grant", body: `<p class="unb-balance__value">${money(0, { hero: true })}</p><p class="unb-small unb-muted">Contributions appear here once they settle.</p>` })}${panel({ id: "who", title: "Your sponsor", body: `<p class="unb-small">Example Community Foundation approves grants and holds legal control of your fund. Questions? <a href="#contact">Contact donor services</a>.</p>` })}`,
      }),
      mobileActions: button("Add money", { primary: true, href: "#add-money" }),
    },
  },
  {
    id: "no-grants",
    title: "Money available, no grants yet",
    persona: "Donor, first contribution settled",
    scenarios: ["no-grants"],
    page: {
      ...base("unbound-default"),
      notifications: 0,
      pagehead: head(),
      body: home({
        left: balance({ f: { ...fund, available: 50000, committed: 0, settling: 0, value: 50250 }, sponsor: "Example Community Foundation" }),
        right: panel({
          id: "nog",
          title: "Grants",
          body: `<div class="unb-empty"><p class="unb-task__title">You haven't recommended a grant yet</p><p class="unb-small unb-muted">Search U.S. charities by name, cause or location. We check eligibility before your recommendation is submitted.</p><div class="unb-task__action">${button("Recommend a grant", { primary: true, href: "#recommend" })}</div></div>`,
        }),
      }),
      mobileActions: ACTIONS.join(""),
    },
  },
  {
    id: "stale-balance",
    title: "Available to grant cannot be confirmed (stale)",
    persona: "Donor",
    scenarios: ["stale-balances"],
    page: {
      ...base("unbound-default"),
      pagehead: head(),
      body: `${notice({ state: "info", iconName: "clock", title: "We can't confirm today's balance yet", body: `<p>The amounts below are from the last confirmed update on ${time("2026-11-27T16:58:00-06:00", "Nov 27 at 4:58 PM CT")}. You can still recommend grants; each one is checked against the confirmed balance before it is approved.</p>` })}
${home({ left: balance({ availableState: "stale", sponsor: "Example Community Foundation" }), main: grantsInProgress(), attention: attention([T.certify]), right: `${activity()}` })}`,
      mobileActions: ACTIONS.join(""),
    },
  },
  {
    id: "integration-unavailable",
    title: "Investment pricing unavailable (integration down)",
    persona: "Donor",
    scenarios: ["integration-unavailable"],
    page: {
      ...base("unbound-default"),
      pagehead: head(),
      body: home({ left: balance({ valueState: "unavailable", sponsor: "Example Community Foundation" }), main: grantsInProgress(), right: `${notice({ state: "info", iconName: "wifi-off", title: "Investment values are temporarily unavailable", body: "<p>We can't reach the investment pricing service. Your Available to grant amount isn't affected and you can keep recommending grants. We'll update values automatically.</p>" })}${activity()}`, attention: attention([T.certify]) }),
      mobileActions: ACTIONS.join(""),
    },
  },
  {
    id: "restricted",
    title: "Restricted: grant recommendations paused by sponsor review",
    persona: "Donor",
    scenarios: ["restricted-fund"],
    page: {
      ...base("unbound-default"),
      pagehead: head({ actions: [button("Add money", { href: "#add-money" })] }),
      body: `${notice({ state: "attention", iconName: "lock", title: "Grant recommendations are paused", body: `<p>Example Community Foundation is reviewing a recent change to your account details. Reviews usually finish within 2 business days. Contributions, documents and grants already approved are not affected.</p>`, actions: [button("Contact donor services", { href: "#contact" })] })}
${home({ left: balance({ sponsor: "Example Community Foundation" }), main: grantsInProgress(), right: `${panel({ id: "why", title: "What you can still do", body: dl([["Add money", chip("ok", "Available")], ["View and download documents", chip("ok", "Available")], ["Recommend a grant", chip("blocked", "Paused")], ["Change advisors or successors", chip("blocked", "Paused")]]) })}${activity()}` })}`,
      mobileActions: button("Add money", { href: "#add-money" }),
    },
  },
  {
    id: "suspended",
    title: "Fund suspended by sponsor",
    persona: "Donor",
    scenarios: ["suspended-fund"],
    page: {
      ...base("unbound-default"),
      notifications: 1,
      pagehead: head({ actions: [] }),
      body: `${notice({ state: "blocked", title: "This fund is suspended", body: `<p>Example Community Foundation suspended this fund on ${time("2026-11-20", "Nov 20, 2026")} because the annual donor certification is more than 90 days overdue. No grants or contributions can be made until it is complete.</p>`, actions: [button("Complete certification", { primary: true, href: "#certify" }), button("Contact donor services", { href: "#contact" })] })}
${home({ left: balance({ sponsor: "Example Community Foundation" }), right: activity() })}`,
    },
  },
  {
    id: "succession",
    title: "Succession pending: successor's first view",
    persona: "Successor advisor before activation",
    scenarios: ["succession-pending", "suspended-fund"],
    page: {
      ...base("unbound-default"),
      user: { name: "Maya Lindqvist", initials: "ML", role: "Successor" },
      notifications: 1,
      pagehead: head({ sub: "Donor-advised fund · You are named as successor advisor", actions: [] }),
      body: `${notice({ state: "info", iconName: "users", title: "You've been named successor advisor", body: `<p>We're sorry for your loss. Example Community Foundation recorded Elin Lindqvist's death on ${time("2026-11-18", "Nov 18, 2026")}, and the fund's succession plan names you as successor. Grants and contributions are paused until you accept.</p>`, actions: [button("Review and accept", { primary: true, href: "#accept" }), button("Decline or ask a question", { href: "#contact" })] })}
${home({ left: panel({ id: "next", title: "What happens next", body: `<ol class="unb-checklist"><li data-step="done"><p class="unb-checklist__title">Sponsor verifies the death notice</p><p class="unb-small unb-muted">Completed ${time("2026-11-18", "Nov 18")}</p></li><li data-step="current" aria-current="step"><p class="unb-checklist__title">You review and accept the successor role</p><p class="unb-small unb-muted">About 10 minutes. You'll confirm your identity and agree to the program terms.</p></li><li><p class="unb-checklist__title">The fund reopens with you as advisor</p><p class="unb-small unb-muted">You can then recommend grants from the fund's balance.</p></li></ol>` }), right: panel({ id: "val", title: "Fund at a glance", body: dl([["Fund value when paused", money(120000)], ["Grants in progress", "3, paused"], ["Successors named", "1 (you)"]]) }) })}`,
    },
  },
  {
    id: "read-only",
    title: "Read-only: financial advisor with view access",
    persona: "Financial advisor (view only)",
    scenarios: ["read-only"],
    page: {
      ...base("unbound-default"),
      user: { name: "Jordan Patel", initials: "JP", role: "View only" },
      notifications: 0,
      pagehead: head({ sub: `Donor-advised fund · ${chip("neutral", "View only")} Shared with you by Elin Lindqvist`, actions: [button("Download statement", { href: "#statement" })] }),
      body: home({ left: balance({ sponsor: "Example Community Foundation" }), main: grantsInProgress(), right: `${panel({ id: "acc", title: "Your access", body: `<p class="unb-small">You can see balances, grants and statements. Only the fund's advisors can add money or recommend grants. Access expires ${time("2027-06-30", "Jun 30, 2027")}.</p>` })}${activity()}` }),
    },
  },
  {
    id: "failed-payment",
    title: "Failed payment: grant payment returned by charity's bank",
    persona: "Donor",
    scenarios: ["failed-payment", "attention-required"],
    page: {
      ...base("unbound-default"),
      notifications: 3,
      pagehead: head(),
      body: home({
        left: balance({ f: { ...fund, available: 102500, committed: 12500 }, sponsor: "Example Community Foundation" }), main: grantsInProgress([{ to: "Riverside Food Bank", id: "Grant 2026-004790", amount: 2500, chip: chip("blocked", "Payment returned"), next: "Waiting for your choice", steps: [["Submitted", "done"], ["Approved", "done"], ["Payment returned", "failed"]] }, GRANTS[1], { ...GRANTS[2], amount: 2500 }]),
        attention: attention([T.returned, T.certify]), right: `${activity([{ what: "Grant payment returned", sub: "Riverside Food Bank", status: chip("blocked", "Returned"), date: time("2026-12-02", "Dec 2, 2026"), amount: money(2500, { signed: true }) }, ...ACTIVITY])}`,
      }),
      mobileActions: ACTIONS.join(""),
    },
  },
];

export default {
  id: "S-001",
  slug: "S-001-giving-home",
  title: "Giving home",
  requirement: "UNB-UI-S-001",
  states,
};

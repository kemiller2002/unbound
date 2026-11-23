// S-011 Sponsor overview, v2. One page per state.
// Data first, then pure renderers. Synthetic data only.
import { money, chip, panel, notice, quiet, icon, time, pagehead, dl, button } from "../lib/ui.mjs";
import { CSS, metrics } from "./S-008.mjs";

const LEAD = { name: "Sam Okafor", initials: "SO", role: "Operations lead" };
const NOW = ["2026-12-01T16:55:00-06:00", "Dec 1, 2026 at 4:55 PM CT"];

// Grants paid per month (cash out, by payment date in CT), Dec 2025 to Nov 2026.
const MONTHS = [
  ["2025-12", "Dec 2025", "Dec ’25", 498200],
  ["2026-01", "Jan 2026", "Jan ’26", 281400],
  ["2026-02", "Feb 2026", "Feb ’26", 302150],
  ["2026-03", "Mar 2026", "Mar ’26", 355900],
  ["2026-04", "Apr 2026", "Apr ’26", 341275],
  ["2026-05", "May 2026", "May ’26", 389600],
  ["2026-06", "Jun 2026", "Jun ’26", 402880],
  ["2026-07", "Jul 2026", "Jul ’26", 318450],
  ["2026-08", "Aug 2026", "Aug ’26", 296700],
  ["2026-09", "Sep 2026", "Sep ’26", 377125],
  ["2026-10", "Oct 2026", "Oct ’26", 436470],
  ["2026-11", "Nov 2026", "Nov ’26", 512300],
].map(([iso, long, short, amount]) => ({ iso, long, short, amount }));

const UNCONFIRMED = 38750; // November payments sent but not yet seen on a bank statement (bank feed down)

const sum = (xs) => xs.reduce((s, m) => s + m.amount, 0);
const fmt = (n) => new Intl.NumberFormat("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 }).format(n);
const short = (n) => `$${(n / 1000).toFixed(1)}k`;
const pct = (n, max) => ((n / max) * 56).toFixed(2); // bars use at most 56% of the plot width

// ---------- chart ----------
const ROW = 30;
const X0 = 19; // percent: month labels sit left of this
const chartSvg = (months, unconfirmed = 0) => {
  const max = Math.max(...months.map((m) => m.amount));
  const lo = months.reduce((a, b) => (b.amount < a.amount ? b : a));
  const hi = months.reduce((a, b) => (b.amount > a.amount ? b : a));
  const height = months.length * ROW;
  const bars = months
    .map((m, i) => {
      const y = i * ROW;
      const w = pct(m.amount, max);
      const isLast = i === months.length - 1;
      const confirmedW = isLast && unconfirmed ? pct(m.amount - unconfirmed, max) : w;
      const part = isLast && unconfirmed ? `<rect class="unb-x-chart__part" x="${(X0 + +confirmedW).toFixed(2)}%" y="${y + 6}" width="${(w - confirmedW).toFixed(2)}%" height="${ROW - 12}"/>` : "";
      return `<g><text class="unb-x-chart__month" x="0" y="${y + ROW / 2}" dy="0.35em">${m.short}</text>
<rect class="unb-x-chart__track" x="${X0}%" y="${y + 6}" width="56%" height="${ROW - 12}" rx="2"/>
<rect class="unb-x-chart__bar" x="${X0}%" y="${y + 6}" width="${confirmedW}%" height="${ROW - 12}" rx="2"/>${part}
<text class="unb-x-chart__value" x="${(X0 + +w).toFixed(2)}%" dx="6" y="${y + ROW / 2}" dy="0.35em">${short(m.amount)}</text></g>`;
    })
    .join("");
  const desc = `Horizontal bar chart, one bar per month from ${months[0].long} to ${months.at(-1).long}. Lowest month ${lo.long}, $${fmt(lo.amount)}. Highest month ${hi.long}, $${fmt(hi.amount)}. Total $${fmt(sum(months))}.${unconfirmed ? ` November includes $${fmt(unconfirmed)} sent but not yet confirmed by the bank, drawn hatched.` : ""} Exact values are in the data table below the chart.`;
  return `<svg role="img" aria-labelledby="chart-t chart-d" height="${height}" focusable="false">
<title id="chart-t">Grants paid per month, last 12 months</title><desc id="chart-d">${desc}</desc>
<defs><pattern id="unb-x-hatch" width="6" height="6" patternUnits="userSpaceOnUse" patternTransform="rotate(45)"><line class="unb-x-chart__hatch-line" x1="0" y1="0" x2="0" y2="6"/></pattern></defs>
${bars}</svg>`;
};

const dataTable = (months, unconfirmed = 0) =>
  `<table class="unb-x-table"><caption class="forma-visually-hidden">Grants paid per month</caption>
<thead><tr><th scope="col">Month</th><th scope="col" data-col="num">Grants paid</th></tr></thead>
<tbody>${months
    .map((m, i) => `<tr><th scope="row">${time(m.iso, m.long)}</th><td data-col="num">${money(m.amount)}${i === months.length - 1 && unconfirmed ? `<br><span class="unb-small unb-muted">incl. ${money(unconfirmed)} not yet confirmed</span>` : ""}</td></tr>`)
    .join("")}</tbody>
<tfoot><tr><th scope="row">Total, 12 months</th><td data-col="num">${money(sum(months))}</td></tr></tfoot></table>`;

const chartPanel = ({ unconfirmed = 0 } = {}) =>
  panel({
    id: "chart",
    title: "Grants paid per month",
    meta: `<span class="unb-small unb-muted">Last 12 months, by payment date</span>`,
    body: `<figure class="unb-x-chart">${chartSvg(MONTHS, unconfirmed)}
${unconfirmed ? `<ul class="unb-x-chart__legend" aria-label="Chart key"><li><span class="unb-x-chart__key" aria-hidden="true"></span>Confirmed by bank</li><li><span class="unb-x-chart__key" data-part="unconfirmed" aria-hidden="true"></span>Sent, not yet confirmed (bank feed down)</li></ul>` : ""}
<figcaption class="unb-small unb-muted">November is the highest month in a year: year-end giving starts early. Values are rounded to the nearest $100 on the chart; exact amounts are in the table.</figcaption></figure>
<details class="unb-x-disclosure"><summary>Show the data as a table${icon("chevron-down")}</summary>${dataTable(MONTHS, unconfirmed)}</details>`,
  });

// ---------- risk ----------
const HOLDS = [
  ["Charity status not confirmed", 7, 3, 1],
  ["Possible sanctions match", 1, 2, 1],
  ["Payment outcome unknown", 2, 2, 0],
  ["Bank details being verified", 1, 1, 0],
];

const holdsTable = () =>
  `<table class="unb-x-table"><caption class="forma-visually-hidden">Grants on hold by reason and age</caption>
<thead><tr><th scope="col">Reason</th><th scope="col" data-col="num">Under 1 day</th><th scope="col" data-col="num">1–3 days</th><th scope="col" data-col="num">Over 3 days</th></tr></thead>
<tbody>${HOLDS.map(([r, a, b, c]) => `<tr><th scope="row">${r}</th><td data-col="num">${a}</td><td data-col="num">${b}</td><td data-col="num">${c ? `<strong>${c}</strong>` : c}</td></tr>`).join("")}</tbody></table>`;

const ACCOUNTS = (down) => [
  ["Operating cash ••••0012", down ? chip("unknown", "Not current") : chip("ok", "Reconciled"), down ? "Nov 30" : "Dec 1"],
  ["Grant disbursement ••••0034", down ? chip("unknown", "Not current") : chip("blocked", "1 break open"), "Nov 30"],
  ["Custody ••••0056", chip("unknown", "Not current"), "Nov 28"],
];

const reconPanel = (down = false) =>
  panel({
    id: "recon",
    title: "Reconciliation by account",
    body: `${dl(ACCOUNTS(down).map(([a, c, d]) => [`${a}<br><span class="unb-small">Bank evidence as of ${d}</span>`, c]))}
<p class="unb-small"><strong>All accounts:</strong> ${down ? "can't be confirmed while the bank feed is down." : "not confirmed. One break is open and custody evidence is 3 days old."}</p>`,
    foot: quiet("Open reconciliation", { href: "#recon", iconName: "chevron-right" }),
  });

const holdsPanel = () =>
  panel({
    id: "holds",
    title: "Grants on hold",
    meta: `<span class="unb-small unb-muted">21 grants · oldest 6 days</span>`,
    body: holdsTable(),
    foot: quiet("Open work queue", { href: "#inbox", iconName: "chevron-right" }),
  });

const freshness = (down = false) =>
  panel({
    id: "fresh",
    title: "Data freshness",
    body: dl([
      ["Bank statements", down ? chip("unknown", "Not current: Nov 30") : chip("ok", "Dec 1, 6:02 AM")],
      ["IRS charity records", chip("ok", "Nov 30")],
      ["Sanctions lists", chip("ok", "Dec 1, 6:00 AM")],
      ["Market prices", chip("ok", "Nov 30 close")],
    ]),
  });

const totals = (down = false) =>
  metrics(
    [
      { label: "Grants paid, 12 months", value: money(sum(MONTHS)), context: "4,906 grants to 1,240 charities" },
      { label: "Paid in November", value: money(512300), context: down ? `Includes ${money(UNCONFIRMED)} not yet confirmed by the bank` : "Up 17.4% on October", state: down ? "unknown" : undefined },
      { label: "Grants on hold", value: "21", context: "2 over 3 days old", state: "attention" },
      { label: "Straight-through, 7 days", value: "94.2%", context: "Previous 7 days 93.8%" },
    ],
    "Key totals"
  );

const head = (sub) => pagehead({ title: "Overview", sub, actions: [button("Download monthly report", { href: "#report" })] });
const base = { title: "Overview", nav: "funds", audience: "staff", user: LEAD, brand: "unbound-default", notifications: 2, extraCss: CSS, appContext: "Sponsor operations" };
const FRESH_SUB = `Example Community Foundation · All funds · Updated ${time(NOW[0], NOW[1])}`;

export const states = [
  {
    id: "normal",
    title: "Overview with grants-paid chart, holds and reconciliation",
    persona: "Operations lead / sponsor executive",
    scenarios: ["normal", "chart", "stale-balances"],
    page: {
      ...base,
      pagehead: head(FRESH_SUB),
      body: `${totals()}<div class="unb-x-overview"><div>${chartPanel()}</div><div>${holdsPanel()}${reconPanel()}${freshness()}</div></div>`,
    },
  },
  {
    id: "integration-unavailable",
    title: "Bank feed unavailable: what is stale, what still works",
    persona: "Operations lead",
    scenarios: ["integration-unavailable", "stale-balances"],
    page: {
      ...base,
      pagehead: head(FRESH_SUB),
      body: `${notice({
        state: "info",
        iconName: "wifi-off",
        title: "The bank feed has been down since 6:00 AM CT",
        body: `<p>We haven't received statements from Prairie Community Bank since ${time("2026-11-30T23:59:00-06:00", "the close of Nov 30, 2026")}. Reconciliation and November's confirmed-paid figures are therefore not current.</p>
<p><strong>Still working:</strong> grant approvals, payment sending, donor balances and the work queue. Payments sent today are tracked and will be matched when the feed returns. We retry every 15 minutes; last try ${time("2026-12-01T16:45:00-06:00", "4:45 PM CT")}.</p>`,
        actions: [quiet("Bank feed status", { href: "#status", iconName: "chevron-right" })],
      })}
${totals(true)}<div class="unb-x-overview"><div>${chartPanel({ unconfirmed: UNCONFIRMED })}</div><div>${reconPanel(true)}${freshness(true)}${holdsPanel()}</div></div>`,
    },
  },
];

export default {
  id: "S-011",
  slug: "S-011-sponsor-overview",
  title: "Sponsor overview",
  requirement: "UNB-UI-S-011",
  states,
};

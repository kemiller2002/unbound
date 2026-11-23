// W-001 Embedded grant-status widget, v2. The widget is its own document (body.unb-widget)
// served from the sponsor's Unbound origin; a partner host iframes it.
// Data first, then pure renderers. Synthetic data only.
import { money, chip, steps, dl, icon, time, BRANDS } from "../lib/ui.mjs";

// Brand marks: same artwork as the app bar (ui.mjs keeps MARKS private; promote an export).
const MARKS = {
  "unbound-default": `<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M6 5h5v13a5 5 0 0 0 10 0V5h5v13a10 10 0 0 1-20 0Z"/></svg>`,
  "example-harbor": `<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M16 3 4 12v3h24v-3Zm-9 14h4v8H7Zm7 0h4v8h-4Zm7 0h4v8h-4ZM4 27h24v3H4Z"/></svg>`,
};
const EIN = { "unbound-default": "00-0000001", "example-harbor": "00-0000000" };

const GRANT = {
  charity: "Riverside Food Bank",
  fund: "Lindqvist Family Fund",
  amount: 5000,
  status: chip("ok", "Approved"),
  next: `Payment scheduled for ${time("2026-12-03", "Dec 3, 2026")}`,
  steps: [["Submitted", "done"], ["Approved", "done"], ["Paid", "current"]],
  updated: time("2026-12-01T16:58:00-06:00", "Dec 1, 2026, 4:58 PM CT"),
};

// ---------- widget document ----------
const doc = ({ brand, title, card }) => {
  const b = BRANDS[brand];
  return `<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>${title} · ${b.name}</title>
<link rel="stylesheet" href="../../../vendor/forma-1.0.2/all.css">
${b.css.map((h) => `<link rel="stylesheet" href="${h}">`).join("\n")}
<link rel="stylesheet" href="../../unbound-app.css">
<link rel="stylesheet" href="../../proposed/ops-charity-widget.css">
</head>
<body class="unb-widget" data-forma-brand="${brand}">
<main class="unb-widget__card unb-x-wcard">
<p class="unb-x-wcard__brand"><span class="unb-x-wcard__mark">${MARKS[brand]}</span><span class="unb-x-wcard__name">${b.name}</span><span>Read only</span></p>
<div class="unb-x-wcard__body">${card}</div>
<footer class="unb-x-wcard__foot"><p>Powered by <strong>${b.name}</strong>, a donor-advised fund program of ${b.sponsor} (EIN <span class="unb-num">${EIN[brand]}</span>), which has exclusive legal control over contributed assets.</p></footer>
</main>
</body>
</html>
`;
};

const grantCard = (g = GRANT) =>
  `<h1>Grant to ${g.charity}</h1>
<div class="unb-x-wcard__status">${g.status}<p>${g.next}</p></div>
<p class="unb-x-wcard__amount">${money(g.amount, { hero: true })}</p>
${steps(g.steps)}
${dl([["From", g.fund], ["Last updated", g.updated]])}`;

const expiredCard = () =>
  `<h1>Grant status isn't available right now</h1>
<div class="unb-x-wcard__error">${icon("lock")}<p><strong>Sign in again on your provider's site</strong> to see this grant.</p><p>For your security this view closes after 15 minutes. No grant details are shown until you sign in again.</p></div>`;

// ---------- partner host page (fictional, the partner's own CSS) ----------
const HOST_CSS = `
:root{--h-ink:#14202e;--h-muted:#4a5868;--h-navy:#12324a;--h-line:#d5dde5;--h-bg:#f4f7f9;--h-accent:#00695c}
*{box-sizing:border-box}
body{margin:0;font-family:Georgia,"Times New Roman",serif;color:var(--h-ink);background:var(--h-bg);line-height:1.5}
a{color:var(--h-accent)}
:focus-visible{outline:3px solid #b3541e;outline-offset:2px}
.h-bar{display:flex;flex-wrap:wrap;align-items:center;gap:8px 24px;padding:12px clamp(16px,4vw,48px);background:var(--h-navy);color:#fff}
.h-bar strong{font-size:1.2rem;letter-spacing:.02em}
.h-bar nav ul{display:flex;flex-wrap:wrap;gap:4px 16px;margin:0;padding:0;list-style:none}
.h-bar nav a{color:#fff;display:inline-block;padding:6px 0;font-family:system-ui,sans-serif;font-size:.9375rem}
.h-main{max-width:1100px;margin:0 auto;padding:24px clamp(16px,4vw,48px) 48px;display:grid;gap:24px}
h1{margin:0;font-size:clamp(1.6rem,1.3rem + 1vw,2.2rem);font-weight:400}
h2{margin:0 0 8px;font-size:1.15rem;font-family:system-ui,sans-serif}
.h-lead{margin:0;color:var(--h-muted);font-family:system-ui,sans-serif;max-width:60ch}
.h-grid{display:grid;gap:24px;grid-template-columns:minmax(0,1fr)}
@media (min-width:1060px){.h-grid{grid-template-columns:minmax(0,634px) 394px;justify-content:space-between}}
.h-slot{background:#fff;border:1px solid var(--h-line);border-radius:2px;padding:16px;min-width:0}
.h-slot p{margin:0 0 12px;font-family:system-ui,sans-serif;font-size:.875rem;color:var(--h-muted)}
.h-slot iframe{display:block;border:0;width:100%;max-width:100%}
.h-slot--narrow iframe{width:360px}
.h-slot--wide iframe{width:600px}
.h-foot{font-family:system-ui,sans-serif;font-size:.8125rem;color:var(--h-muted);border-top:1px solid var(--h-line);padding-top:16px}
`;

const host = () => `<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>Your giving · Fairway Wealth Partners (fictional partner host)</title>
<style>${HOST_CSS}</style>
</head>
<body>
<header class="h-bar"><strong>Fairway Wealth Partners</strong><nav aria-label="Client portal"><ul><li><a href="#accounts">Accounts</a></li><li><a href="#giving" aria-current="page">Giving</a></li><li><a href="#documents">Documents</a></li></ul></nav></header>
<main class="h-main" id="main">
<div><h1>Your charitable giving</h1><p class="h-lead">Your donor-advised fund is held by your giving sponsor. The cards below come straight from the sponsor and update on their own.</p></div>
<div class="h-grid">
<section class="h-slot h-slot--wide" aria-labelledby="wide-t"><h2 id="wide-t">Latest grant</h2><p>Wide slot, 600 px.</p><iframe src="widget.html" title="Grant status from Unbound Giving (wide)" height="392" tabindex="-1"></iframe></section>
<section class="h-slot h-slot--narrow" aria-labelledby="narrow-t"><h2 id="narrow-t">Grant status</h2><p>Sidebar slot, 360 px.</p><iframe src="widget.html" title="Grant status from Unbound Giving (sidebar)" height="452" tabindex="-1"></iframe></section>
</div>
<p class="h-foot">Fairway Wealth Partners does not hold or control your donor-advised fund. Grant information is provided read-only by your sponsor.</p>
</main>
</body>
</html>
`;

export const states = [
  { id: "widget", kind: "widget", title: "Widget document: approved grant, read-only card", persona: "Donor viewing a partner site (valid token)", scenarios: ["normal", "embedded"], html: doc({ brand: "unbound-default", title: "Grant to Riverside Food Bank", card: grantCard() }) },
  { id: "host-page", kind: "host", title: "Partner host page embedding the widget at 360 px and 600 px", persona: "Donor on the partner's client portal", scenarios: ["embedded", "white-label", "normal"], html: host() },
  { id: "expired", kind: "widget", title: "Expired or invalid token: safe message, no grant data", persona: "Donor whose partner session expired", scenarios: ["error", "embedded"], html: doc({ brand: "unbound-default", title: "Grant status unavailable", card: expiredCard() }) },
  { id: "widget-harbor", kind: "widget", title: "Widget under the Harbor Giving sponsor brand", persona: "Donor viewing a partner site (valid token)", scenarios: ["white-label", "embedded"], html: doc({ brand: "example-harbor", title: "Grant to Riverside Food Bank", card: grantCard() }) },
];

export default {
  id: "W-001",
  slug: "W-001-grant-status-widget",
  title: "Embedded grant-status widget",
  requirement: "UNB-UI-W-001",
  states,
};

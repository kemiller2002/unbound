import { tag, loz, kv, money, ident } from "../lib/markup.mjs";

// The widget is the document a partner iframes; it has no application navigation or site footer.
// Each design state shows one card as it renders inside the frame.
const card = (p, title, inner) =>
  tag(
    "surface",
    `<section class="forma-surface" aria-labelledby="${p}-card">
  <h3 id="${p}-card">${title}</h3>
  ${inner}
  <p><small>Grant made by Example Charitable Foundation (EIN 00-0000000), which holds exclusive legal control over donor-advised fund assets. Read only.</small></p>
</section>`
  );

const grant = (p, lozenge) =>
  card(p, "Grant to Riverside Food Bank", `${lozenge}${kv([["Amount", money("$5,000.00")], ["Charity", "Riverside Food Bank, Springfield, IL"], ["Grant reference", ident("2026-004817")], ["Status as of", "1 Dec 2026, 4:58 PM CT"]])}`);

const render = (states) => `<!doctype html>
<html lang="en" data-forma-theme="light">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>W-001 Grant status widget | Harbor Giving design prototype</title>
<meta name="description" content="Static Forma 1.0.2 design prototype for UNB-UI-W-001: read-only grant status card for partner iframes. Synthetic data.">
<link rel="stylesheet" href="../vendor/forma-1.0.2/all.css">
<link rel="stylesheet" href="../vendor/forma-1.0.2/brands/example-harbor.css">
</head>
<body class="forma-site" data-forma-brand="example-harbor">
<main class="forma-site__main" id="main-content">
  <header class="forma-section-heading"><div class="forma-section-heading__text">
    <p class="forma-eyebrow">Design prototype · UNB-UI-W-001</p>
    <h1>Embedded grant status widget</h1>
    <p class="forma-lead">Read-only card served from the sponsor's origin inside a partner iframe. No consequential actions in release 1.</p>
  </div></header>
  ${states.map((s) => s.html).join("\n")}
</main>
</body>
</html>
`;

export default {
  id: "W-001",
  file: "W-001-grant-status-widget.html",
  title: "Embedded grant status widget",
  reqIds: ["UNB-UI-W-001"],
  render,
  states: [
    { title: "Normal: funding in progress", persona: "Partner host, valid token", demonstrates: "Headline status, amount and charity; no actions", body: (p) => grant(p, loz("attention", "Funding in progress")) },
    { title: "Payment outcome unknown", persona: "Partner host, valid token", demonstrates: "UNB-UI-G-001 AC-1 in the widget (never 'Paid' while unknown)", body: (p) => grant(p, loz("unknown", "Payment outcome unknown")) },
    { title: "Expired token", persona: "Partner host, expired token", demonstrates: "UNB-UI-W-001 AC-1 ('Sign in again on your provider's site' and no grant data)", body: (p) => card(p, "Grant status unavailable", `${loz("blocked", "Session expired")}<p>Sign in again on your provider's site.</p>`) },
  ],
  coverage: [
    { requirement_id: "UNB-UI-W-001", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 3 shows 'Sign in again on your provider's site' and no grant data. Token validation, CSP frame-ancestors and TTL are server behaviour (UNB-GAP-W-01)." },
    { requirement_id: "UNB-UI-W-001", acceptance_id: "statement (no actions)", satisfied_by_markup: true, note: "No interactive controls in any widget state." },
  ],
  gaps: ["UNB-GAP-W-01", "FORMA-GAP-08"],
};

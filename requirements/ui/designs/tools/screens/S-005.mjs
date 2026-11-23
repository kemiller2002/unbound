import { tag, loz, fresh, kv, money, critical, ident, refWithCopy, field, select, choiceGroup, callout, dialog, dataGrid, timeline, opStatus, recordHeader, scopeTrail } from "../lib/markup.mjs";

const scope = (last) => scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Lindqvist Family Fund (F-2026-000418)", last]);
const crumbs = `<a href="S-001-giving-home.html">Giving home</a> <span aria-hidden="true">/</span> <a href="#contributions">Contributions</a>`;

/** Contribution lifecycle as ordered dimensions (same composition as G-002). */
const lifecycle = (id, items) =>
  tag(
    "steps",
    `<ol class="forma-steps" role="list" aria-label="Contribution progress" id="${id}">
  ${items.map(([name, state, label, when]) => `<li><div class="forma-stack"><p><strong>${name}</strong></p>${loz(state, label)}<p>${when}</p></div></li>`).join("\n  ")}
</ol>`
  );

const methods = (p, checked) =>
  choiceGroup({
    name: `${p}-method`,
    legend: "How do you want to add money?",
    choices: [
      ["ach", "Bank transfer (ACH) from a linked account", "Available to grant after the 5-business-day return window.", checked === "ach"],
      ["wire", "Wire transfer", "Instructions for your bank. Usually received the same day.", checked === "wire"],
      ["check", "Check", "Mail a check payable to Example Charitable Foundation.", checked === "check"],
      ["securities", "Publicly traded securities", "Transfer shares from your broker by DTC.", checked === "securities"],
      ["complex", "Complex assets", "Private stock, real estate and other assets through our partner intermediary.", checked === "complex"],
    ],
  });

const linkBank = (p) =>
  dialog({
    id: `${p}-link`,
    invoker: "Link a bank account…",
    title: "Continue to Lakeshore Rails to link your bank?",
    desc: "You will sign in to your bank on Lakeshore Rails, our payment partner. Harbor Giving never sees your bank password.",
    body: kv([["What changes", "A verified account is added to your fund for ACH contributions"], ["Who acts next", "You, on the partner's site; you return here when done"]]),
    actions: `<button type="submit" value="cancel" autofocus>Stay here</button><button type="submit" value="go">Continue to Lakeshore Rails</button>`,
  });

export default {
  id: "S-005",
  file: "S-005-add-money.html",
  title: "Add money and contribution status",
  reqIds: ["UNB-UI-S-005", "UNB-UI-G-004"],
  audience: "donor",
  brand: true,
  lead: "Declare a contribution by cash or securities, then follow it through Declared, Received, Accepted, Valued, Acknowledged and Final.",
  states: [
    {
      title: "Bank transfer (ACH) from a linked account",
      persona: "Donor",
      demonstrates: "Contribution intent form for ACH; link bank account hand-off dialog",
      body: (p) => `${scope("Add money")}${methods(p, "ach")}${tag(
        "form",
        `<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft">
<h3 id="${p}-ft">Bank transfer details</h3>
<div class="forma-form-row" data-forma-form-layout="grid">
${select({ id: `${p}-acct`, label: "From account", options: [["4821", "Lakeshore Credit Union checking ••••4821 (verified)", true]] })}
${field({ id: `${p}-amt`, label: "Amount in US dollars (required)", value: "5,000.00", attrs: 'inputmode="decimal" required' })}
</div>
<div class="forma-actions"><button type="submit">Declare contribution</button>${linkBank(p)}</div>
</form>`
      )}`,
    },
    {
      title: "Securities: DTC instructions and intent",
      persona: "Donor",
      demonstrates: "Transfer instructions with custodian DTC details and an intent form (security, quantity, broker)",
      body: (p) => `${scope("Add money")}${methods(p, "securities")}
<section class="forma-stack" aria-labelledby="${p}-dtc"><h3 id="${p}-dtc">Give these instructions to your broker</h3>${kv([
  ["Receiving custodian", "Example Custody Bank, N.A."],
  ["DTC participant number", refWithCopy("DTC participant number", "0000")],
  ["Account name", "Example Charitable Foundation"],
  ["Account number", refWithCopy("account number", "00000000")],
  ["Reference for this fund", refWithCopy("fund reference", "F-2026-000418")],
])}</section>
${tag(
  "form",
  `<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft">
<h3 id="${p}-ft">Tell us what is coming</h3>
<div class="forma-form-row" data-forma-form-layout="grid">
${field({ id: `${p}-sec`, label: "Security name or ticker (required)", value: "XYZ Corp. common stock (XYZ)", attrs: "required" })}
${tag("numeric-stepper", `<label class="forma-numeric-stepper"><span class="forma-numeric-stepper__label">Number of shares (required)</span><input type="number" name="${p}-qty" min="1" step="1" inputmode="numeric" required value="1000"><span class="forma-field__description">Whole shares only.</span></label>`)}
${field({ id: `${p}-broker`, label: "Your broker (required)", value: "Lakeside Brokerage", attrs: "required" })}
</div>
<div class="forma-actions"><button type="submit">Declare contribution</button></div>
</form>`
)}`,
    },
    {
      title: "Wire, check and complex assets",
      persona: "Donor",
      demonstrates: "Wire and check instructions; complex assets show the partner intermediary hand-off only",
      body: (p) => `${scope("Add money")}
<section class="forma-stack" aria-labelledby="${p}-w"><h3 id="${p}-w">Wire instructions</h3>${kv([["Bank", "Example Custody Bank, N.A."], ["Routing number", refWithCopy("routing number", "000000000")], ["Account", refWithCopy("account number", "00000001")], ["Memo", refWithCopy("memo", "F-2026-000418 Lindqvist")]])}</section>
<section class="forma-stack" aria-labelledby="${p}-c"><h3 id="${p}-c">Check instructions</h3>${kv([["Payable to", "Example Charitable Foundation"], ["Memo line", ident("F-2026-000418")], ["Mail to", "Example Charitable Foundation, PO Box 0000, Springfield, IL 00000"]])}</section>
${callout(`${p}-cx`, "note", "Complex assets", `<p>Private company stock, real estate and other complex assets are handled by our partner, Example Asset Intermediary. They review the asset and contact you. <a href="#partner">Start with Example Asset Intermediary</a></p>`)}`,
    },
    {
      title: "Securities partially received",
      persona: "Donor",
      demonstrates: "UNB-UI-S-005 AC-1 ('Partially received: 600 of 1,000 shares', no acknowledgment); lot-by-lot receipt",
      body: (p) => `${scope("Contribution C-2026-000391")}${recordHeader({ type: "Contribution", title: "C-2026-000391", lozenge: loz("attention", "Partially received: 600 of 1,000 shares"), sub: `1,000 shares of XYZ Corp. common stock declared 20 Nov 2026<br>Reference ${refWithCopy("contribution reference", "C-2026-000391")}`, crumbs })}
${lifecycle(`${p}-lc`, [
  ["Declared", "ok", "Declared", "20 Nov 2026: 1,000 shares"],
  ["Received", "attention", "Partially received: 600 of 1,000", "Lot 1 received 25 Nov 2026"],
  ["Accepted", null, "Not started", "After all lots are received"],
  ["Valued", null, "Not started", "Mean of high and low on the receipt date"],
  ["Acknowledged", null, "Not issued", "Issued once received and accepted"],
  ["Final", null, "Not started", "Available to grant after sale"],
])}
${dataGrid({ label: "Lots received", cols: [["Lot"], ["Shares"], ["Received"], ["Status"]], rows: [
  ["1", money("600"), "25 Nov 2026", loz("ok", "Received")],
  ["2", money("400"), "Not yet received", loz("attention", "Waiting on your broker")],
] })}
${kv([["Acknowledgment", "Not issued. We issue it after all shares are received; it will describe the shares and state no value."]])}`,
    },
    {
      title: "ACH inside its return window",
      persona: "Donor",
      demonstrates: "UNB-UI-S-005 AC-2 (amount under Pending with the date it becomes available), UNB-UI-G-004",
      body: (p) => `${scope("Contribution C-2026-000377")}${recordHeader({ type: "Contribution", title: "C-2026-000377", lozenge: loz("attention", "Received: in return window"), sub: `$5,000.00 by ACH from Lakeshore Credit Union ••••4821<br>Reference ${refWithCopy("contribution reference", "C-2026-000377")}`, crumbs })}
${kv([
  ["Available to grant", `${critical("$100,000.00")} (does not include this contribution)`],
  ["Pending", `${money("$5,000.00")}: ACH contribution received 24 Nov 2026, available to grant on <time datetime="2026-12-08">8 Dec 2026</time>`],
  ["Gift date", `<time datetime="2026-11-24">24 Nov 2026</time> (CT)`],
])}
${lifecycle(`${p}-lc`, [
  ["Declared", "ok", "Declared", "24 Nov 2026"],
  ["Received", "ok", "Received", "24 Nov 2026"],
  ["Accepted", "attention", "In return window", "Until 8 Dec 2026"],
  ["Valued", "ok", "Valued", "Cash: $5,000.00"],
  ["Acknowledged", "ok", "Acknowledged", "25 Nov 2026"],
  ["Final", null, "Not started", "8 Dec 2026"],
])}`,
    },
    {
      title: "ACH returned",
      persona: "Donor",
      demonstrates: "Returned (ACH) state with reason; Rejected uses the same pattern",
      body: (p) => `${scope("Contribution C-2026-000355")}${opStatus({ id: `${p}-ret`, state: "blocked", label: "Returned (ACH)", symbol: "×", title: "Your bank returned this contribution", body: "<p>Lakeshore Credit Union returned the $2,000.00 ACH contribution on 21 Nov 2026 (insufficient funds). It was removed from Pending and no acknowledgment was issued.</p>", actions: ['<button type="button">Try another way to add money</button>'] })}
${timeline("Contribution C-2026-000355 history", [
  { iso: "2026-11-21T09:00:00-06:00", shown: "21 Nov 2026", state: "blocked", label: "Returned", title: "Returned by the bank: insufficient funds" },
  { iso: "2026-11-17T09:00:00-06:00", shown: "17 Nov 2026", state: "attention", label: "Received", title: "$2,000.00 received by ACH" },
])}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-005", acceptance_id: "AC-1", satisfied_by_markup: true, note: "State 4: record-header lozenge 'Partially received: 600 of 1,000 shares'; Acknowledged dimension 'Not issued'." },
    { requirement_id: "UNB-UI-S-005", acceptance_id: "AC-2", satisfied_by_markup: true, note: "State 5: Pending row shows $5,000.00 and 'available to grant on 8 Dec 2026'; Available to grant excludes it." },
    { requirement_id: "UNB-UI-S-005", acceptance_id: "statement", satisfied_by_markup: "needs-application-behaviour", note: "Distinct lifecycle states composed from steps + lozenge (same gap as G-002). Rail-partner linking is an external hand-off." },
  ],
  gaps: ["FORMA-GAP-01", "FORMA-GAP-06", "FORMA-GAP-07", "FORMA-GAP-08", "FORMA-GAP-09", "FORMA-GAP-10"],
};

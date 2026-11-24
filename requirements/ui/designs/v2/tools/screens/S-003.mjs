// S-003 Grant recommendation, v2. One page per state.
// Design decision: one page with progressive disclosure (charity, then amount and
// details) followed by ONE review-and-confirm page, instead of v1's four-step wizard
// plus a confirmation dialog. See the summary in the build report for why.
// Data first, then pure renderers. Synthetic data only (same fund as S-001).
import { money, chip, panel, notice, button, quiet, icon, time, pagehead, dl } from "../lib/ui.mjs";
import { track, kv } from "./S-004.mjs";

const DONOR = { name: "Elin Lindqvist", initials: "EL", role: null };
const SPONSOR = "Example Community Foundation";
const FUND = "Lindqvist Family Fund";
const AVAILABLE = 100000;
const UPDATED = time("2026-12-01T16:58:00-06:00", "today at 4:58 PM CT");

// ---------- local form helpers (Forma field / choice / checkbox markup) ----------
const describedBy = (...ids) => {
  const list = ids.filter(Boolean);
  return list.length ? ` aria-describedby="${list.join(" ")}"` : "";
};
const vmsg = (id, text) => `<p class="forma-validation-message" id="${id}"><span class="forma-validation-message__mark" aria-hidden="true">!</span><span>${text}</span></p>`;

const moneyField = ({ id, label, desc, value, error }) => `<div class="forma-field">
<label class="forma-field__label" for="${id}">${label}</label>
<span class="forma-field__description" id="${id}-d">${desc}</span>
<div class="unb-x-money-input"><span aria-hidden="true">$</span><input id="${id}" name="${id}" type="text" inputmode="decimal" autocomplete="off" value="${value}" required${describedBy(`${id}-d`, error && `${id}-e`)}${error ? ' aria-invalid="true"' : ""}></div>
${error ? vmsg(`${id}-e`, error) : ""}</div>`;

const textField = ({ id, label, desc, value = "", error, type = "text", attrs = "" }) => `<div class="forma-field">
<label class="forma-field__label" for="${id}">${label}</label>
${desc ? `<span class="forma-field__description" id="${id}-d">${desc}</span>` : ""}
<input id="${id}" name="${id}" type="${type}" value="${value}"${attrs}${describedBy(desc && `${id}-d`, error && `${id}-e`)}${error ? ' aria-invalid="true"' : ""}>
${error ? vmsg(`${id}-e`, error) : ""}</div>`;

const textarea = ({ id, label, desc, value = "", error }) => `<div class="forma-field">
<label class="forma-field__label" for="${id}">${label}</label>
<span class="forma-field__description" id="${id}-d">${desc}</span>
<textarea id="${id}" name="${id}" rows="2" maxlength="200" required${describedBy(`${id}-d`, error && `${id}-e`)}${error ? ' aria-invalid="true"' : ""}>${value}</textarea>
${error ? vmsg(`${id}-e`, error) : ""}</div>`;

/** choices: [value, title, text, checked] */
const choices = ({ name, legend, items }) => `<fieldset class="forma-choice-group">
<legend class="forma-choice-group__legend">${legend}</legend>
${items.map(([v, t, s, c]) => `<label class="forma-choice"><input type="radio" name="${name}" value="${v}"${c ? " checked" : ""}><span class="forma-choice__content"><strong>${t}</strong>${s ? `<span>${s}</span>` : ""}</span></label>`).join("")}
</fieldset>`;

const checkbox = ({ id, label, checked }) => `<label class="forma-checkbox"><input class="forma-checkbox__input" type="checkbox" id="${id}" name="${id}" value="yes" required${checked ? " checked" : ""}><span class="forma-checkbox__box" aria-hidden="true"></span><span class="forma-checkbox__text"><span class="forma-checkbox__label">${label}</span></span></label>`;

const select = ({ id, label, options }) => `<label class="forma-select-field"><span class="forma-field__label">${label}</span><span class="forma-select"><select class="forma-select__input" id="${id}" name="${id}">${options.map(([v, l, s]) => `<option value="${v}"${s ? " selected" : ""}>${l}</option>`).join("")}</select><span class="forma-select__indicator" aria-hidden="true"></span></span></label>`;

const errorSummary = (items) => `<div class="forma-validation-summary" role="alert" tabindex="-1" aria-labelledby="err-t">
<div class="forma-validation-summary__icon" aria-hidden="true">!</div>
<div><h2 class="forma-validation-summary__title" id="err-t">There ${items.length === 1 ? "is 1 problem" : `are ${items.length} problems`} to fix before you review</h2>
<ul class="forma-validation-summary__list">${items.map(([href, text]) => `<li><a href="${href}">${text}</a></li>`).join("")}</ul></div></div>`;

// ---------- data ----------
const CHECKED = time("2026-11-30", "Nov 30, 2026");
const RESULTS = [
  { name: "Riverside Food Bank", place: "Springfield, IL", ein: "00-0000103", ok: true },
  { name: "Riverside Food Bank of Central Ohio", place: "Columbus, OH", ein: "00-0000418", ok: true },
  { name: "Riverside Community Food Pantry", place: "Peoria, IL", ein: "00-0000522", ok: false, chip: chip("blocked", "Not eligible"), why: "It isn't registered as a public charity, so it can't receive grants from a donor-advised fund." },
  { name: "Riverside Hunger Relief", place: "Decatur, IL", ein: "00-0000609", ok: false, chip: chip("blocked", "On hold"), why: "It can't receive grants while its status is being checked. Try again in a few days." },
];
const CHARITY = RESULTS[0];
const HELD = { name: "Northgate Literacy Project", place: "Gary, IN", ein: "00-0000311" };

const AFFIRM = [
  "I and my family will receive no goods, services or other benefit in return for this grant, including tickets, dinners or memberships.",
  "This grant will not be used to pay for any part of an event, membership or purchase that benefits me.",
];

// ---------- regions ----------
const head = (title = "Recommend a grant", sub) =>
  pagehead({
    context: [`<a href="#grants">Grants</a>`, "New recommendation"],
    title,
    sub: sub ?? `From ${FUND} · ${money(AVAILABLE)} available to grant`,
  });

const result = (r) => `<li class="unb-x-result">
<div><p class="unb-x-result__name">${r.name}</p><p class="unb-small unb-muted">${r.place} · EIN <span class="unb-num">${r.ein}</span></p></div>
<p class="unb-x-result__elig">${r.ok ? `${chip("ok", "Eligible")}<span>Can receive grants. Checked ${CHECKED}.</span>` : `${r.chip}<span>${r.why}</span>`}</p>
${r.ok ? `<div class="unb-x-result__action">${`<button class="forma-button" type="button">Select<span class="forma-visually-hidden"> ${r.name}, ${r.place}</span></button>`}</div>` : ""}
</li>`;

const searchPanel = () =>
  panel({
    id: "charity",
    title: "Charity",
    body: `<div class="forma-search" role="search" aria-label="Charities">
<label class="forma-search__label" for="q">Search by charity name, city or EIN</label>
<div class="forma-search__control"><input id="q" type="search" value="Riverside food" autocomplete="off"><button class="forma-button" type="button">Search</button></div>
<p class="forma-search__status" role="status">4 charities found. Eligibility is checked against IRS records.</p></div>
<ul class="unb-x-results" aria-label="Search results">${RESULTS.map(result).join("")}</ul>`,
    foot: quiet("Can't find the charity? Ask us to add it", { href: "#request-charity", iconName: "chevron-right" }),
  });

const pickedCharity = (c = CHARITY, { held = false } = {}) =>
  panel({
    id: "charity",
    title: "Charity",
    body: held
      ? `<div class="unb-x-picked" data-state="blocked" id="charity-held"><span class="unb-task__icon unb-task__icon--neutral">${icon("globe")}</span>
<p class="unb-x-result__name">${c.name}</p><p class="unb-small unb-muted">${c.place} · EIN <span class="unb-num">${c.ein}</span></p>
<p class="unb-x-result__elig">${chip("blocked", "On hold")}<span>Since ${time("2026-11-27", "Nov 27, 2026")} it can't receive grants while its tax-exempt status is checked.</span></p>
<div class="unb-task__action">${button("Choose another charity", { href: "#search" })}${quiet("Keep as a draft until it's confirmed", { href: "#save" })}</div></div>`
      : `<div class="unb-x-picked"><span class="unb-task__icon unb-task__icon--neutral">${icon("globe")}</span>
<p class="unb-x-result__name">${c.name}</p><p class="unb-small unb-muted">${c.place} · EIN <span class="unb-num">${c.ein}</span></p>
<p class="unb-x-result__elig">${chip("ok", "Eligible")}<span>Can receive grants. Checked ${CHECKED}.</span></p>
<div class="unb-task__action">${quiet(`Change<span class="forma-visually-hidden"> charity</span>`, { href: "#search" })}</div></div>`,
  });

const lockedDetails = () =>
  panel({ id: "details", title: "Amount and details", body: `<p class="unb-muted">Choose a charity first. You'll then enter the amount and purpose here.</p>` });

const moreOptions = ({ open = false } = {}) => `<details class="forma-disclosure"${open ? " open" : ""}>
<summary>Give a percentage of the fund or repeat this grant</summary>
<div class="forma-disclosure__content">
${choices({ name: "basis", legend: "How do you want to set the amount?", items: [["fixed", "A fixed dollar amount", "", !open], ["percent", "A percentage of fund value", "The dollar amount is fixed on the date you choose below.", open]] })}
${open ? `${textField({ id: "pct", label: "Percentage of fund value", desc: "For example 5 for 5%.", value: "5", attrs: ' inputmode="decimal"' })}
${select({ id: "fix", label: "Fix the dollar amount", options: [["approval", "When the grant is approved", true], ["payment", "On the payment date"]] })}
<p class="unb-x-derived" id="pct-est">At today's fund value of ${money(120000)} (market close ${time("2026-11-30", "Nov 30, 2026")}), 5% would be <strong>${money(6000)}</strong>. The exact amount is fixed at approval and must not be more than your Available to grant.</p>` : ""}
${choices({ name: "repeat", legend: "How often?", items: [["once", "One time", "", true], ["quarterly", "Every 3 months", "Each payment is checked against Available to grant first.", false], ["yearly", "Once a year", "", false]] })}
</div></details>`;

const detailsForm = ({ amount = "5,000.00", purpose = "General operating support", errors = {}, open = false, action }) =>
  panel({
    id: "details",
    title: "Amount and details",
    body: `<form class="unb-x-form" id="grant-form" novalidate aria-labelledby="details-t">
${moneyField({ id: "amount", label: "Amount in US dollars", desc: `You have ${money(AVAILABLE)} available to grant. The minimum grant is ${money(50)}.`, value: open ? "" : amount, error: errors.amount })}
${moreOptions({ open })}
${textarea({ id: "purpose", label: "Purpose", desc: `${SPONSOR} requires a purpose. It's shown to the charity, for example “General operating support”.`, value: purpose, error: errors.purpose })}
${choices({ name: "recognition", legend: "How should the charity see your name?", items: [["name", "Elin Lindqvist", "Your name as the recommending advisor", true], ["fund", FUND, "The fund's name only", false], ["anon", "Anonymous", `The charity sees “Anonymous donor via ${SPONSOR}”`, false]] })}
<fieldset><legend class="forma-choice-group__legend">${SPONSOR} asks you to confirm</legend>
<p class="unb-x-fieldset-note">Grants from a donor-advised fund can't give you anything in return.</p>
${AFFIRM.map((t, i) => checkbox({ id: `aff-${i + 1}`, label: t, checked: true })).join("")}</fieldset>
<fieldset class="forma-binary-choice" aria-describedby="pledge-d"><legend class="forma-binary-choice__legend">Does this grant pay a pledge you made personally?</legend>
<p class="unb-x-fieldset-note" id="pledge-d">${SPONSOR} can't use grants to pay personal pledges.</p>
<div class="forma-binary-choice__options"><label class="forma-binary-option"><input type="radio" name="pledge" value="no" checked required><span>No</span></label><label class="forma-binary-option"><input type="radio" name="pledge" value="yes"><span>Yes</span></label></div></fieldset>
<div class="unb-x-actions">${action ?? `${button("Review grant", { primary: true })}${quiet("Save draft", { href: "#save", iconName: "save" })}`}</div>
</form>`,
  });

const summaryAside = ({ amount = 5000, note = true } = {}) =>
  panel({
    id: "sum",
    title: "Your fund",
    body: `${dl([
      ["Available to grant", money(AVAILABLE)],
      ...(amount ? [["This grant", money(-amount)], ["Left after this grant", `<strong>${money(AVAILABLE - amount)}</strong>`]] : []),
    ])}<p class="unb-small unb-muted">Updated ${UPDATED}.</p>`,
    foot: note ? `<p class="unb-small unb-muted">${SPONSOR} reviews every recommendation, usually within 2 business days. You can withdraw it until it's approved.</p>` : "",
  });

const flow = (main, aside) => `<div class="unb-x-flow"><div>${main}</div>${aside ? `<div>${aside}</div>` : ""}</div>`;

const base = { title: "Recommend a grant", nav: "grants", audience: "donor", user: DONOR, brand: "unbound-default", notifications: 2, extraCss: ["proposed/donor-grants.css"] };

// ---------- states ----------
const choose = { ...base, pagehead: head(), body: flow(`${searchPanel()}${lockedDetails()}`, summaryAside({ amount: 0 })) };

const SPONSOR_OF = { "unbound-default": SPONSOR, "example-harbor": "Example Charitable Foundation" };
const details = (brand = "unbound-default") => ({
  ...base,
  brand,
  pagehead: head(),
  body: flow(`${pickedCharity()}${detailsForm({})}`, summaryAside()).replaceAll(SPONSOR, SPONSOR_OF[brand]),
});

const errors = {
  ...base,
  pagehead: head(),
  body: `${errorSummary([
    ["#charity-held", "Northgate Literacy Project can't receive grants right now. Choose another charity or keep this as a draft"],
    ["#amount", `Enter ${money(AVAILABLE)} or less, your Available to grant`],
    ["#purpose", "Enter a purpose for this grant"],
  ])}
${flow(
  `${pickedCharity(HELD, { held: true })}${detailsForm({
    amount: "150,000.00",
    purpose: "",
    errors: { amount: `Enter ${money(AVAILABLE)} or less. That's your Available to grant today.`, purpose: `Enter a purpose. ${SPONSOR} requires one for every grant.` },
  })}`,
  summaryAside({ amount: 0 })
)}`,
};

const CONSEQUENCES = `<dl class="unb-x-consequences">
<div>${icon("refresh")}<dt>What changes</dt><dd>${money(5000)} moves from Available to grant into grants in progress. Available to grant becomes ${money(95000)}.</dd></div>
<div>${icon("arrow-left")}<dt>Can you undo it?</dt><dd>Yes. You can withdraw it until ${SPONSOR} approves it.</dd></div>
<div>${icon("users")}<dt>Who acts next</dt><dd>${SPONSOR} checks the charity and approves or declines your recommendation.</dd></div>
<div>${icon("calendar")}<dt>When</dt><dd>A decision usually by ${time("2026-12-03", "Dec 3, 2026")}. Payment within 5 business days of approval.</dd></div>
</dl>`;

const review = {
  ...base,
  pagehead: head("Review and submit", `Nothing is sent until you submit · From ${FUND}`),
  body: flow(
    panel({
      id: "rev",
      title: "Your grant recommendation",
      meta: quiet(`Edit<span class="forma-visually-hidden"> grant details</span>`, { href: "#edit", iconName: "edit" }),
      body: `<div class="unb-x-commit"><p class="unb-x-commit__amount">${money(5000, { hero: true })}</p><p class="unb-x-commit__to">to ${CHARITY.name}, ${CHARITY.place}</p><p class="unb-small unb-muted">One time · EIN <span class="unb-num">${CHARITY.ein}</span></p></div>
${kv([
  ["Purpose", "General operating support"],
  ["Recognition", "Elin Lindqvist"],
  ["Your confirmations", "No benefit to you or your family; doesn't pay a personal pledge"],
])}
<h3>What happens when you submit</h3>
${CONSEQUENCES}`,
      foot: `<div class="unb-x-actions">${button("Submit grant recommendation", { primary: true })}${button("Edit details", { href: "#edit" })}<p class="unb-x-actions__note">Your recommendation is advisory: ${SPONSOR} makes the final decision.</p></div>`,
    }),
    summaryAside({ note: false })
  ),
};

const submitted = {
  ...base,
  notifications: 3,
  pagehead: head("Grant recommendation submitted", `From ${FUND}`),
  body: flow(
    panel({
      id: "done",
      label: "Result",
      body: `<div class="unb-x-outcome" data-state="ok"><span class="unb-x-outcome__icon">${icon("success")}</span>
<h2>${SPONSOR} received your recommendation</h2>
<p>${money(5000)} to ${CHARITY.name} on ${time("2026-12-01T17:02:00-06:00", "Dec 1, 2026 at 5:02 PM CT")}. Grant <span class="unb-num">2026-004835</span>.</p></div>
${track([
  { label: "Submitted", when: time("2026-12-01", "Dec 1, 2026"), step: "done" },
  { label: "Sponsor review", when: `Decision expected by ${time("2026-12-03", "Dec 3")}`, step: "current" },
  { label: "Approved", when: "Not yet", step: "todo" },
  { label: "Paid", when: `Expected by ${time("2026-12-10", "Dec 10")}`, step: "todo" },
], "Progress of grant 2026-004835")}
<p class="unb-small">We'll email you when it's approved. You can withdraw it from the grant page until then.</p>`,
      foot: `<div class="unb-x-actions">${button("View grant", { primary: true, href: "#grant-2026-004835" })}${button("Recommend another grant", { href: "#recommend" })}</div>`,
    }),
    panel({ id: "sum", title: "Your fund", body: `${dl([["Available to grant", money(95000)], ["In review", money(5000)]])}<p class="unb-small unb-muted">Updated ${time("2026-12-01T17:02:00-06:00", "today at 5:02 PM CT")}.</p>` })
  ),
};

const unknown = {
  ...base,
  pagehead: head("Checking your recommendation", `From ${FUND}`),
  body: flow(
    panel({
      id: "unk",
      label: "Result",
      body: `<div class="unb-x-outcome" data-state="unknown"><span class="unb-x-outcome__icon">${icon("help-circle")}</span>
<h2>We can't confirm your recommendation was received</h2>
<p>The connection ended before ${SPONSOR} replied. Please don't submit it again: if it did arrive, a second one could create a duplicate grant.</p></div>
${kv([
  ["What you sent", `${money(5000)} to ${CHARITY.name}<small>${time("2026-12-01T17:02:00-06:00", "Dec 1, 2026 at 5:02 PM CT")}</small>`],
  ["Status", `${chip("unknown", "Not confirmed yet")}`],
  ["How we'll resolve it", `We're checking automatically and will show the result here and by email within 15 minutes. If it wasn't received, nothing is taken from Available to grant.`],
])}`,
      foot: `<div class="unb-x-actions">${button("Check status", { primary: true })}${quiet("Go to grants", { href: "#grants", iconName: "chevron-right" })}<p class="unb-x-actions__note">Check status only looks up this recommendation. It never sends it again.</p></div>`,
    }),
    ""
  ),
};

const restricted = {
  ...base,
  pagehead: head(),
  body: `${notice({
    state: "attention",
    iconName: "lock",
    title: "Submitting grant recommendations is paused",
    body: `<p>${SPONSOR} is reviewing a recent change to your account details. Reviews usually finish within 2 business days. Until then you can't submit new recommendations.</p>
<p>You can still find charities and save this recommendation as a draft to submit later. Grants already approved will be paid as scheduled, and adding money and documents aren't affected.</p>`,
    actions: [button("Contact donor services", { href: "#contact" })],
  })}
${flow(
  `${pickedCharity()}${detailsForm({ action: `${button("Save draft", { primary: true })}<p class="unb-x-actions__note">We'll email you when you can submit it.</p>` })}`,
  summaryAside()
)}`,
};

const options = { ...base, pagehead: head(), body: flow(`${pickedCharity()}${detailsForm({ open: true })}`, summaryAside({ amount: 6000 })) };

export const states = [
  { id: "choose-charity", title: "Choose a charity: search results with eligibility in plain words", persona: "Donor (primary advisor)", scenarios: ["normal", "charity-search", "ineligible-charity"], page: choose },
  { id: "details", title: "Amount and details: one column, progressive disclosure", persona: "Donor (primary advisor)", scenarios: ["normal"], page: details() },
  { id: "validation-errors", title: "Validation: amount above Available to grant, missing purpose, charity on hold", persona: "Donor (primary advisor)", scenarios: ["validation-error", "ineligible-charity", "eligibility-hold"], page: errors },
  { id: "review", title: "Review and submit: the single G-005 confirmation", persona: "Donor (primary advisor)", scenarios: ["confirmation"], page: review },
  { id: "submitted", title: "Submitted: what happens next", persona: "Donor (primary advisor)", scenarios: ["success", "pending-grants"], page: submitted },
  { id: "outcome-unknown", title: "Outcome unknown after submit timeout (G-006)", persona: "Donor (primary advisor)", scenarios: ["outcome-unknown", "integration-unavailable"], page: unknown },
  { id: "restricted", title: "Restricted: recommending paused, draft still possible", persona: "Donor (primary advisor)", scenarios: ["restricted-fund"], page: restricted },
  { id: "more-options", title: "More options open: percentage of fund value and repeat (phone reference)", persona: "Donor on a phone", scenarios: ["phone", "percentage-grant", "recurring"], page: options },
  { id: "white-label", title: "Amount and details under the Harbor Giving brand", persona: "Donor (primary advisor)", scenarios: ["white-label"], page: details("example-harbor") },
];

export default { id: "S-003", slug: "S-003-grant-recommendation", title: "Recommend a grant", requirement: "UNB-UI-S-003", states };

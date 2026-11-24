import { tag, loz, fresh, ident, dataGrid, emptyState, skeleton, scopeTrail, faultInline, field, select } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Lindqvist Family Fund (F-2026-000418)", "Recommend a grant: choose a charity"]);

const toolbar = (p, query, count) =>
  tag(
    "collection-toolbar",
    `<section class="forma-collection-toolbar" aria-label="Charity search controls">
  <form class="forma-search forma-collection-toolbar__search" role="search">
    <label class="forma-search__label" for="${p}-q">Search by charity name, alias or EIN</label>
    <div class="forma-search__control">
      <input id="${p}-q" name="q" type="search" autocomplete="off"${query ? ` value="${query}"` : ""}>
      <button type="button" class="forma-search__clear">Clear</button>
    </div>
  </form>
  <div class="forma-collection-toolbar__controls">
    <details class="forma-collection-toolbar__filters">
      <summary>Filters ${loz(null, "1 active")}</summary>
      <div class="forma-collection-toolbar__filter-panel">
        <label>State <select name="state"><option>Any state</option><option selected>Illinois</option><option>Oregon</option></select></label>
        <label>Cause <select name="cause"><option selected>Any cause</option><option>Education</option><option>Human services</option></select></label>
      </div>
    </details>
    <label class="forma-collection-toolbar__sort">Sort
      <select name="sort"><option>Best match</option><option>Legal name</option></select>
    </label>
  </div>
  <p class="forma-collection-toolbar__count" role="status">${count}</p>
</section>`
  );

const activeFilters = (p) =>
  tag(
    "active-filter-summary",
    `<section class="forma-active-filter-summary" aria-labelledby="${p}-af">
  <div class="forma-active-filter-summary__heading">
    <h3 id="${p}-af">Active filters</h3>
    <button type="button">Clear all</button>
  </div>
  <ul class="forma-active-filter-summary__items">
    <li><span>Search: Hope Foundation</span><button type="button" aria-label="Remove Search: Hope Foundation filter">Remove</button></li>
  </ul>
</section>`
  );

const elig = (state, label, iso, shown) =>
  `${loz(state, label)}${fresh({ label: "IRS data as of", iso, shown, stateText: "Current" })}`;

const results = (p) =>
  dataGrid({
    label: "Charity search results",
    cols: [["Legal name"], ["EIN"], ["City and state"], ["Platform eligibility"], ["Actions", null, true]],
    rows: [
      ["Hope Foundation", ident("00-0000101"), "Springfield, IL", elig("ok", "Eligible", "2026-11-30", "30 Nov 2026"), `<button type="button" aria-label="Select Hope Foundation, EIN 00-0000101, Springfield, IL">Select</button>`],
      ["Hope Foundation", ident("00-0000102"), "Portland, OR", elig("ok", "Eligible", "2026-11-30", "30 Nov 2026"), `<button type="button" aria-label="Select Hope Foundation, EIN 00-0000102, Portland, OR">Select</button>`],
      ["Hope Foundation of Northgate", ident("00-0000104"), "Northgate, IL", elig("blocked", "On hold", "2026-11-27", "27 Nov 2026"), `<p>Cannot be selected while on hold</p>`],
      ["Riverside Food Bank (also known as Riverside Pantry)", ident("00-0000103"), "Springfield, IL", elig("ok", "Eligible", "2026-11-30", "30 Nov 2026"), `<button type="button" aria-label="Select Riverside Food Bank, EIN 00-0000103">Select</button>`],
    ],
  });

const pagination = tag(
  "pagination",
  `<nav class="forma-pagination" aria-label="Search result pages">
  <a class="forma-pagination__control" href="?page=1" rel="prev">Previous</a>
  <ol class="forma-pagination__pages"><li><a href="?page=1" aria-current="page">1</a></li></ol>
  <a class="forma-pagination__control" href="?page=1" rel="next">Next</a>
  <span class="forma-pagination__summary">Page 1 of 1</span>
</nav>`
);

const cantFind = `<p>Can't find it? <a href="#s002-5">Ask us to add a charity</a>.</p>`;

export default {
  id: "S-002",
  file: "S-002-charity-search.html",
  title: "Charity search and selection",
  reqIds: ["UNB-UI-S-002"],
  audience: "donor",
  brand: true,
  lead: "Find a charity by name, alias or EIN, see its platform eligibility with an as-of date, and bind its canonical identity.",
  states: [
    {
      title: "Results: two charities with the same name and one on hold",
      persona: "Donor on a phone",
      demonstrates: "UNB-UI-S-002 AC-1 (EIN and city/state per record), AC-2 ('On hold', no reason text, not selectable)",
      body: (p) => `${scope}${toolbar(p, "Hope Foundation", "4 charities")}${activeFilters(p)}${results(p)}${pagination}${cantFind}`,
    },
    {
      title: "Searching",
      persona: "Donor",
      demonstrates: "Searching state with busy semantics (UNB-UI-G-013)",
      body: (p) => `${scope}${toolbar(p, "Hope", "Searching")}${skeleton("Searching charities")}`,
    },
    {
      title: "No results",
      persona: "Donor",
      demonstrates: "Empty result with the next sensible action (UNB-UI-G-013)",
      body: (p) => `${scope}${toolbar(p, "Hopewel Trust", "0 charities")}${emptyState({ id: `${p}-empty`, title: "No charities match “Hopewel Trust”", text: "Check the spelling, search by EIN, or ask us to add the charity. New charities are verified before you can recommend a grant.", action: '<a class="forma-button" href="#s002-5">Request a charity we don\'t list</a>' })}`,
    },
    {
      title: "Search unavailable",
      persona: "Donor",
      demonstrates: "UNB-UI-G-012 fault presentation with a support reference",
      body: (p) => `${scope}${toolbar(p, "Hope Foundation", "Search unavailable")}${faultInline({ id: `${p}-fault`, title: "Charity search is unavailable right now.", message: "Your draft grant is kept. Try again in a few minutes, or quote the reference if you contact support.", reference: "AG-7Q3KD", actions: '<button type="button" data-forma-aegis-capability="CanRetry">Try search again</button>' })}`,
    },
    {
      title: "Request a charity we don't list",
      persona: "Donor",
      demonstrates: "Identity-resolution request form opened from “Can't find it?”",
      body: (p) => `${scope}${tag(
        "form",
        `<form class="forma-form" id="${p}-form" method="get" action="#${p}-done" aria-labelledby="${p}-form-title">
<h3 id="${p}-form-title">Ask us to add a charity</h3>
<p>We verify the charity's identity and IRS status before it appears in search. This usually takes 3 business days.</p>
<div class="forma-form-row" data-forma-form-layout="grid">
${field({ id: `${p}-name`, label: "Charity legal name (required)", attrs: 'required autocomplete="organization"', value: "Hopewell Community Trust" })}
${field({ id: `${p}-ein`, label: "EIN, if you know it", desc: "Nine digits, for example 00-0000000.", attrs: 'inputmode="numeric"' })}
</div>
<div class="forma-form-row" data-forma-form-layout="grid">
${field({ id: `${p}-city`, label: "City (required)", attrs: "required", value: "Decatur" })}
${select({ id: `${p}-state`, label: "State (required)", options: [["", "Choose a state"], ["IL", "Illinois", true], ["OR", "Oregon"]] })}
${field({ id: `${p}-web`, label: "Website", type: "url", attrs: 'autocomplete="url"' })}
</div>
<div class="forma-actions"><button type="submit">Send request</button><button type="reset">Clear form</button></div>
<p class="forma-form__outcome" id="${p}-done" role="status" aria-label="Request status">Request sent. Reference REQ-2026-00071. We will email you when the charity is verified.</p>
</form>`
      )}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-002", acceptance_id: "AC-1", satisfied_by_markup: true, note: "Each record prints Legal name, EIN and City and state via data-grid data-label projection at 320 px (rendered/S-002-charity-search-320.png)." },
    { requirement_id: "UNB-UI-S-002", acceptance_id: "AC-2", satisfied_by_markup: "needs-application-behaviour", note: "Markup shows 'On hold' (blocked) with no reason text and no Select control. Withholding another sponsor's reason and enforcing non-selection server-side are application behaviour." },
    { requirement_id: "UNB-UI-S-002", acceptance_id: "statement", satisfied_by_markup: "needs-application-behaviour", note: "Search, filtering, sort and binding the canonical identity on Select are Limen/application behaviour; the data-grid sort buttons are hidden on phones so sort is offered in collection-toolbar." },
  ],
  gaps: ["FORMA-GAP-06", "FORMA-GAP-08"],
};

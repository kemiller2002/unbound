import { tag, loz, kv, ident, dataGrid, emptyState, metric, scopeTrail, feedItem } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Sponsor operations", "Exception inbox"]);

const summary = (p) =>
  tag(
    "dashboard-grid",
    `<section class="forma-dashboard-grid" aria-label="Inbox summary">
${metric({ label: "Straight-through rate, last 7 days", value: "94.2%", context: "<p>1,812 of 1,923 grants needed no human touch.</p>" })}
${metric({ label: "Open items by age", value: "19", context: kv([["Under 1 day", "12"], ["1 to 3 days", "5"], ["Over 3 days", "2, both breaching SLA"]]) })}
${metric({ label: "Breaching SLA", value: "2", context: loz("blocked", "Breaching SLA") })}
</section>`
  );

const toolbar = (p) =>
  tag(
    "collection-toolbar",
    `<section class="forma-collection-toolbar" aria-label="Inbox controls">
  <form class="forma-search forma-collection-toolbar__search" role="search"><label class="forma-search__label" for="${p}-q">Search subject or reference</label><input id="${p}-q" type="search"></form>
  <div class="forma-collection-toolbar__controls">
    <details class="forma-collection-toolbar__filters">
      <summary>Filters ${loz(null, "1 active")}</summary>
      <div class="forma-collection-toolbar__filter-panel">
        <label>Rule family <select name="${p}-family"><option>Any rule family</option><option>Eligibility</option><option>Sanctions screening</option><option>Payments</option></select></label>
        <label>Sponsor program <select name="${p}-sponsor"><option>Harbor Giving</option></select></label>
        <label>Owner <select name="${p}-owner"><option>Anyone</option><option selected>Unassigned</option><option>Me</option></select></label>
      </div>
    </details>
    <label class="forma-collection-toolbar__sort">Sort <select name="${p}-sort"><option>SLA risk, highest first</option><option>Age, oldest first</option></select></label>
  </div>
  <p class="forma-collection-toolbar__count" role="status">5 open items</p>
</section>`
  );

const active = (p) =>
  tag(
    "active-filter-summary",
    `<section class="forma-active-filter-summary" aria-labelledby="${p}-af"><div class="forma-active-filter-summary__heading"><h3 id="${p}-af">Active filters</h3><button type="button">Clear all</button></div>
<ul class="forma-active-filter-summary__items"><li><span>Sponsor program: Harbor Giving</span><button type="button" aria-label="Remove Sponsor program: Harbor Giving filter">Remove</button></li></ul></section>`
  );

const grid = (p) =>
  dataGrid({
    label: "Open exceptions sorted by SLA risk",
    cols: [["SLA risk", "descending"], ["Rule"], ["Subject"], ["Age against SLA"], ["Owner"], ["Next action", null, true]],
    rows: [
      [loz("blocked", "Breaching SLA"), "SAN-02 Potential sanctions match", `Grant ${ident("2026-004102")}`, "4 d 2 h of 3 d", "Unassigned", `<button type="button" aria-label="Open case CASE-2026-000231">Open case</button>`],
      [loz("blocked", "Breaching SLA"), "PAY-07 Payment outcome unknown", `Grant ${ident("2026-004817")}`, "3 d 6 h of 3 d", "Dana Ruiz", `<button type="button" aria-label="Open case CASE-2026-000228">Open case</button>`],
      [loz("attention", "Due today"), "ELG-03 Charity eligibility not confirmed", `Grant ${ident("2026-004811")}`, "1 d 20 h of 2 d", "Unassigned", `<button type="button" aria-label="Assign case CASE-2026-000240 to me">Assign to me</button>`],
      [loz("attention", "Waiting on external party"), "CON-04 Securities partially received", `Contribution ${ident("C-2026-000391")}`, "6 d of 10 d", "Lee Tran", `<button type="button" aria-label="Open case CASE-2026-000219">Open case</button>`],
      [loz(null, "Within SLA"), "DST-01 Charity destination change", `Charity ${ident("00-0000103")}`, "4 h of 5 d", "Unassigned", `<button type="button" aria-label="Assign case CASE-2026-000245 to me">Assign to me</button>`],
    ],
  });

const pagination = tag("pagination", `<nav class="forma-pagination" aria-label="Inbox pages"><a class="forma-pagination__control" href="?page=1" rel="prev">Previous</a><ol class="forma-pagination__pages"><li><a href="?page=1" aria-current="page">1</a></li></ol><a class="forma-pagination__control" href="?page=1" rel="next">Next</a><span class="forma-pagination__summary">Page 1 of 1</span></nav>`);

const inspector = (p) => tag("surface", `<section class="forma-surface" aria-labelledby="${p}-insp"><h4 id="${p}-insp">Selected: SAN-02 Potential sanctions match</h4>${kv([["Created by rule", "SAN-02 v4, 27 Nov 2026"], ["Exit condition", "A reviewer other than the preparer records a decision"], ["SLA", "3 business days"]])}</section>`);

const shell = (p, primary, inspector) =>
  tag(
    "workspace-shell",
    `<div class="forma-workspace-shell">
  <header class="forma-workspace-shell__landmark"><h3>Harbor Giving exceptions</h3><p role="status">Updated 1 Dec 2026, 4:55 PM CT</p></header>
  <nav class="forma-workspace-shell__navigation" aria-label="Saved views"><a href="#s008-1" aria-current="page">All open</a><a href="#s008-3">Recently closed</a></nav>
  <section class="forma-workspace-shell__primary" aria-label="Exception list">${primary}</section>
  <aside class="forma-workspace-shell__inspector" aria-label="Selected exception">${inspector}</aside>
</div>`
  );

export default {
  id: "S-008",
  file: "S-008-exception-inbox.html",
  title: "Staff exception inbox",
  reqIds: ["UNB-UI-S-008"],
  audience: "staff",
  brand: false,
  lead: "One inbox of rule-created work items sorted by SLA risk. Routine work never appears; items close when their exit condition becomes true.",
  states: [
    {
      title: "Open items sorted by SLA risk",
      persona: "Sponsor operations",
      demonstrates: "Rule, subject, age against SLA, owner and next action per row; summary row with straight-through rate and open items by age",
      body: (p) => `${scope}${summary(p)}${toolbar(p)}${active(p)}${grid(p)}${pagination}${inspector(p)}`,
    },
    {
      title: "Gap evidence: workspace-shell nested in the application frame",
      persona: "Sponsor operations (composition that does not fit)",
      demonstrates: "FORMA-GAP-15: workspace-shell, collection-toolbar and data-grid adapt by viewport, not container; inside the navigation-shell content column the toolbar collapses at 1280 px",
      body: (p) => `${shell(p, `${toolbar(p)}${grid(p)}`, `<h4>SAN-02 Potential sanctions match</h4>${kv([["Created by rule", "SAN-02 v4, 27 Nov 2026"], ["SLA", "3 business days"]])}`)}`,
    },
    {
      title: "Item closed automatically",
      persona: "Sponsor operations",
      demonstrates: "UNB-UI-S-008 AC-1 (work item closes with reason 'Exit condition met')",
      body: (p) => `${scope}${feedItem({ id: `${p}-closed`, source: "ELG-03 Charity eligibility not confirmed · Grant 2026-004811", title: "Closed automatically: Exit condition met", state: "ok", label: "Resolved (auto-closed)", iso: "2026-12-01T16:40:00-06:00", shown: "1 Dec 2026, 4:40 PM CT", summary: "A new IRS observation dated 1 Dec 2026 confirmed Northgate Literacy Project as a public charity. The hold was released; no one acted on this item." })}`,
    },
    {
      title: "Inbox empty",
      persona: "Sponsor operations",
      demonstrates: "Empty state (UNB-UI-G-013)",
      body: (p) => `${scope}${emptyState({ id: `${p}-e`, title: "No open exceptions", text: "Every item created by a rule is resolved. Checked 1 Dec 2026, 4:55 PM CT.", action: '<a class="forma-button" href="#s008-3">View recently closed items</a>' })}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-008", acceptance_id: "AC-1", satisfied_by_markup: "needs-application-behaviour", note: "State 2 shows the closed item with 'Resolved (auto-closed)' and 'Exit condition met'. Closing on the IRS observation is rule-engine behaviour." },
    { requirement_id: "UNB-UI-S-008", acceptance_id: "statement (workspace-shell)", satisfied_by_markup: false, note: "State 2 nests workspace-shell inside the application frame: at 1280 px its primary column is ~330 px and collection-toolbar collapses (labels set vertically). State 1 therefore omits workspace-shell and stacks toolbar, grid and an inspector surface. FORMA-GAP-15." },
    { requirement_id: "UNB-UI-S-008", acceptance_id: "statement (sort by SLA risk)", satisfied_by_markup: "needs-application-behaviour", note: "aria-sort=descending on SLA risk and a sort select for phones; sorting itself is Limen/application." },
  ],
  gaps: ["FORMA-GAP-06", "FORMA-GAP-08", "FORMA-GAP-15"],
};

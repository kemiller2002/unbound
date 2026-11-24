import { tag, loz, ident, dataGrid, emptyState, scopeTrail, field, opStatus, kv } from "../lib/markup.mjs";

const scope = scopeTrail(["Example Charitable Foundation", "Harbor Giving donor-advised funds", "Lindqvist Family Fund (F-2026-000418)", "Documents"]);

const toolbar = (p, year, count) =>
  tag(
    "collection-toolbar",
    `<section class="forma-collection-toolbar" aria-label="Document filters">
  <div class="forma-collection-toolbar__controls">
    <label class="forma-collection-toolbar__view">Type <select name="${p}-type"><option>All types</option><option>Acknowledgments</option><option>Statements</option><option>Grant confirmations</option></select></label>
    <label class="forma-collection-toolbar__view">Year <select name="${p}-year"><option${year === 2026 ? " selected" : ""}>2026</option><option>2025</option><option${year === 2024 ? " selected" : ""}>2024</option></select></label>
  </div>
  <p class="forma-collection-toolbar__count" role="status">${count}</p>
</section>`
  );

const open = (ref) => `<span class="forma-cluster"><a href="#${ref}" aria-label="Open ${ref} as a web page">Open</a><a href="#${ref}-pdf" aria-label="Download ${ref} as PDF">Download PDF</a></span>`;

export default {
  id: "S-007",
  file: "S-007-document-center.html",
  title: "Document center",
  reqIds: ["UNB-UI-S-007", "UNB-UI-G-016"],
  audience: "donor",
  brand: true,
  lead: "Acknowledgments, statements and grant confirmations, each with its reference, verification status, accessible web version and PDF from the same snapshot.",
  states: [
    {
      title: "Documents including a corrected acknowledgment",
      persona: "Donor",
      demonstrates: "UNB-UI-S-007 AC-1 (original shows 'Superseded by <ref>' and both stay downloadable)",
      body: (p) => `${scope}${toolbar(p, 2026, "4 documents")}${dataGrid({
        label: "Documents for 2026",
        cols: [["Type"], ["Period"], ["Issued", "descending"], ["Reference"], ["Verification"], ["Actions", null, true]],
        rows: [
          ["Contribution acknowledgment (corrected)", "Gift of 24 Nov 2026", "28 Nov 2026", ident("DOC-2026-019002"), loz("ok", "Verified"), open("DOC-2026-019002")],
          ["Contribution acknowledgment", "Gift of 24 Nov 2026", "25 Nov 2026", ident("DOC-2026-018950"), `${loz("attention", "Superseded")}<p>Superseded by <a href="#DOC-2026-019002">${ident("DOC-2026-019002")}</a></p>`, open("DOC-2026-018950")],
          ["Grant confirmation", "Grant 2026-004817", "24 Nov 2026", ident("DOC-2026-018833"), loz("ok", "Verified"), open("DOC-2026-018833")],
          ["Quarterly statement", "Q3 2026", "9 Oct 2026", ident("DOC-2026-014120"), loz("ok", "Verified"), open("DOC-2026-014120")],
        ],
      })}${tag("pagination", `<nav class="forma-pagination" aria-label="Document pages"><a class="forma-pagination__control" href="?page=1" rel="prev">Previous</a><ol class="forma-pagination__pages"><li><a href="?page=1" aria-current="page">1</a></li></ol><a class="forma-pagination__control" href="?page=1" rel="next">Next</a><span class="forma-pagination__summary">Page 1 of 1</span></nav>`)}`,
    },
    {
      title: "Verify a document",
      persona: "Donor or third party",
      demonstrates: "Verify document action: reference and verification code resolve in the document registry",
      body: (p) => `${scope}<form class="forma-form" id="${p}-f" aria-labelledby="${p}-ft"><h3 id="${p}-ft">Verify a document</h3>
<div class="forma-form-row" data-forma-form-layout="grid">${field({ id: `${p}-ref`, label: "Document reference (required)", value: "DOC-2026-019002", attrs: "required" })}${field({ id: `${p}-code`, label: "Verification code (required)", desc: "Printed in the document footer.", value: "7KQ2-M9XD", attrs: "required" })}</div>
<div class="forma-actions"><button type="submit">Verify document</button></div></form>
${opStatus({ id: `${p}-v`, state: "ok", label: "Verified", symbol: "✓", title: "This document is authentic and current", body: kv([["Reference", ident("DOC-2026-019002")], ["Issued by", "Example Charitable Foundation, EIN 00-0000000"], ["Issued", "28 Nov 2026"]]) })}`,
    },
    {
      title: "No documents for the chosen year",
      persona: "Donor",
      demonstrates: "Empty state with the next sensible action (UNB-UI-G-013)",
      body: (p) => `${scope}${toolbar(p, 2024, "0 documents")}${emptyState({ id: `${p}-e`, title: "No documents for 2024", text: "This fund opened in February 2025. Choose 2025 or 2026 to see its documents.", action: '<button type="button">Show 2026</button>' })}`,
    },
  ],
  coverage: [
    { requirement_id: "UNB-UI-S-007", acceptance_id: "AC-1", satisfied_by_markup: true, note: "Original row shows 'Superseded' and 'Superseded by DOC-2026-019002'; both rows keep Open and Download PDF." },
    { requirement_id: "UNB-UI-S-007", acceptance_id: "statement (filters)", satisfied_by_markup: "needs-application-behaviour", note: "Type and year filters are native selects in collection-toolbar; filtering is application behaviour." },
  ],
  gaps: ["FORMA-GAP-06"],
};

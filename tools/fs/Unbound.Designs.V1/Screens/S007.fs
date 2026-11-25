/// S-007 Document center (port of tools/screens/S-007.mjs).
module Unbound.Designs.V1.Screens.S007

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Lindqvist Family Fund (F-2026-000418)"; "Documents" ]

let private toolbar (p: string) (year: int) (count: string) =
    let sel y = if year = y then " selected" else ""
    tag
        "collection-toolbar"
        $$"""<section class="forma-collection-toolbar" aria-label="Document filters">
  <div class="forma-collection-toolbar__controls">
    <label class="forma-collection-toolbar__view">Type <select name="{{p}}-type"><option>All types</option><option>Acknowledgments</option><option>Statements</option><option>Grant confirmations</option></select></label>
    <label class="forma-collection-toolbar__view">Year <select name="{{p}}-year"><option{{sel 2026}}>2026</option><option>2025</option><option{{sel 2024}}>2024</option></select></label>
  </div>
  <p class="forma-collection-toolbar__count" role="status">{{count}}</p>
</section>"""

let private ``open`` (docRef: string) =
    $$"""<span class="forma-cluster"><a href="#{{docRef}}" aria-label="Open {{docRef}} as a web page">Open</a><a href="#{{docRef}}-pdf" aria-label="Download {{docRef}} as PDF">Download PDF</a></span>"""

let private documents (p: string) =
    let grid =
        Ui.dataGrid(
            label = "Documents for 2026",
            cols = [ Ui.col "Type"; Ui.col "Period"; Ui.col("Issued", "descending"); Ui.col "Reference"; Ui.col "Verification"; Ui.col("Actions", plain = true) ],
            rows =
                [ [ "Contribution acknowledgment (corrected)"; "Gift of 24 Nov 2026"; "28 Nov 2026"; ident "DOC-2026-019002"; loz "ok" "Verified"; ``open`` "DOC-2026-019002" ]
                  [ "Contribution acknowledgment"
                    "Gift of 24 Nov 2026"
                    "25 Nov 2026"
                    ident "DOC-2026-018950"
                    loz "attention" "Superseded" + "<p>Superseded by <a href=\"#DOC-2026-019002\">" + ident "DOC-2026-019002" + "</a></p>"
                    ``open`` "DOC-2026-018950" ]
                  [ "Grant confirmation"; "Grant 2026-004817"; "24 Nov 2026"; ident "DOC-2026-018833"; loz "ok" "Verified"; ``open`` "DOC-2026-018833" ]
                  [ "Quarterly statement"; "Q3 2026"; "9 Oct 2026"; ident "DOC-2026-014120"; loz "ok" "Verified"; ``open`` "DOC-2026-014120" ] ]
        )
    let pagination =
        tag
            "pagination"
            """<nav class="forma-pagination" aria-label="Document pages"><a class="forma-pagination__control" href="?page=1" rel="prev">Previous</a><ol class="forma-pagination__pages"><li><a href="?page=1" aria-current="page">1</a></li></ol><a class="forma-pagination__control" href="?page=1" rel="next">Next</a><span class="forma-pagination__summary">Page 1 of 1</span></nav>"""
    scope + toolbar p 2026 "4 documents" + grid + pagination

let private verify (p: string) =
    let refField = Ui.field(id = $"{p}-ref", label = "Document reference (required)", value = "DOC-2026-019002", attrs = "required")
    let code = Ui.field(id = $"{p}-code", label = "Verification code (required)", desc = "Printed in the document footer.", value = "7KQ2-M9XD", attrs = "required")
    let result =
        Ui.opStatus(
            id = $"{p}-v",
            state = "ok",
            label = "Verified",
            symbol = "✓",
            title = "This document is authentic and current",
            body = kv [ "Reference", ident "DOC-2026-019002"; "Issued by", "Example Charitable Foundation, EIN 00-0000000"; "Issued", "28 Nov 2026" ]
        )
    $$"""{{scope}}<form class="forma-form" id="{{p}}-f" aria-labelledby="{{p}}-ft"><h3 id="{{p}}-ft">Verify a document</h3>
<div class="forma-form-row" data-forma-form-layout="grid">{{refField}}{{code}}</div>
<div class="forma-actions"><button type="submit">Verify document</button></div></form>
{{result}}"""

let private noDocuments (p: string) =
    scope
    + toolbar p 2024 "0 documents"
    + Ui.emptyState(
        id = $"{p}-e",
        title = "No documents for 2024",
        text = "This fund opened in February 2025. Choose 2025 or 2026 to see its documents.",
        action = """<button type="button">Show 2026</button>"""
    )

let screen: Screen =
    { Id = "S-007"
      File = "S-007-document-center.html"
      Title = "Document center"
      ReqIds = [ "UNB-UI-S-007"; "UNB-UI-G-016" ]
      Audience = "donor"
      Brand = true
      Lead = "Acknowledgments, statements and grant confirmations, each with its reference, verification status, accessible web version and PDF from the same snapshot."
      States =
        [ state
              "Documents including a corrected acknowledgment"
              "Donor"
              "UNB-UI-S-007 AC-1 (original shows 'Superseded by <ref>' and both stay downloadable)"
              documents
          state "Verify a document" "Donor or third party" "Verify document action: reference and verification code resolve in the document registry" verify
          state "No documents for the chosen year" "Donor" "Empty state with the next sensible action (UNB-UI-G-013)" noDocuments ]
      Coverage =
        [ cov "UNB-UI-S-007" "AC-1" Yes "Original row shows 'Superseded' and 'Superseded by DOC-2026-019002'; both rows keep Open and Download PDF."
          cov "UNB-UI-S-007" "statement (filters)" NeedsApplicationBehaviour "Type and year filters are native selects in collection-toolbar; filtering is application behaviour." ]
      Gaps = [ "FORMA-GAP-06" ]
      Render = None }

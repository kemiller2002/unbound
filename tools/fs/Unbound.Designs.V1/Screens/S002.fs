/// S-002 Charity search and selection (port of tools/screens/S-002.mjs).
module Unbound.Designs.V1.Screens.S002

open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let private scope =
    scopeTrail [ "Example Charitable Foundation"; "Harbor Giving donor-advised funds"; "Lindqvist Family Fund (F-2026-000418)"; "Recommend a grant: choose a charity" ]

let private toolbar (p: string) (query: string) (count: string) =
    let value = if query <> "" then $" value=\"{query}\"" else ""
    tag
        "collection-toolbar"
        $$"""<section class="forma-collection-toolbar" aria-label="Charity search controls">
  <form class="forma-search forma-collection-toolbar__search" role="search">
    <label class="forma-search__label" for="{{p}}-q">Search by charity name, alias or EIN</label>
    <div class="forma-search__control">
      <input id="{{p}}-q" name="q" type="search" autocomplete="off"{{value}}>
      <button type="button" class="forma-search__clear">Clear</button>
    </div>
  </form>
  <div class="forma-collection-toolbar__controls">
    <details class="forma-collection-toolbar__filters">
      <summary>Filters {{loz "" "1 active"}}</summary>
      <div class="forma-collection-toolbar__filter-panel">
        <label>State <select name="state"><option>Any state</option><option selected>Illinois</option><option>Oregon</option></select></label>
        <label>Cause <select name="cause"><option selected>Any cause</option><option>Education</option><option>Human services</option></select></label>
      </div>
    </details>
    <label class="forma-collection-toolbar__sort">Sort
      <select name="sort"><option>Best match</option><option>Legal name</option></select>
    </label>
  </div>
  <p class="forma-collection-toolbar__count" role="status">{{count}}</p>
</section>"""

let private activeFilters (p: string) =
    tag
        "active-filter-summary"
        $$"""<section class="forma-active-filter-summary" aria-labelledby="{{p}}-af">
  <div class="forma-active-filter-summary__heading">
    <h3 id="{{p}}-af">Active filters</h3>
    <button type="button">Clear all</button>
  </div>
  <ul class="forma-active-filter-summary__items">
    <li><span>Search: Hope Foundation</span><button type="button" aria-label="Remove Search: Hope Foundation filter">Remove</button></li>
  </ul>
</section>"""

let private elig state label iso shown =
    loz state label + Ui.fresh(label = "IRS data as of", iso = iso, shown = shown, stateText = "Current")

let private results (_p: string) =
    Ui.dataGrid(
        label = "Charity search results",
        cols = [ Ui.col "Legal name"; Ui.col "EIN"; Ui.col "City and state"; Ui.col "Platform eligibility"; Ui.col("Actions", plain = true) ],
        rows =
            [ [ "Hope Foundation"; ident "00-0000101"; "Springfield, IL"; elig "ok" "Eligible" "2026-11-30" "30 Nov 2026"; """<button type="button" aria-label="Select Hope Foundation, EIN 00-0000101, Springfield, IL">Select</button>""" ]
              [ "Hope Foundation"; ident "00-0000102"; "Portland, OR"; elig "ok" "Eligible" "2026-11-30" "30 Nov 2026"; """<button type="button" aria-label="Select Hope Foundation, EIN 00-0000102, Portland, OR">Select</button>""" ]
              [ "Hope Foundation of Northgate"; ident "00-0000104"; "Northgate, IL"; elig "blocked" "On hold" "2026-11-27" "27 Nov 2026"; "<p>Cannot be selected while on hold</p>" ]
              [ "Riverside Food Bank (also known as Riverside Pantry)"; ident "00-0000103"; "Springfield, IL"; elig "ok" "Eligible" "2026-11-30" "30 Nov 2026"; """<button type="button" aria-label="Select Riverside Food Bank, EIN 00-0000103">Select</button>""" ] ]
    )

let private pagination =
    tag
        "pagination"
        """<nav class="forma-pagination" aria-label="Search result pages">
  <a class="forma-pagination__control" href="?page=1" rel="prev">Previous</a>
  <ol class="forma-pagination__pages"><li><a href="?page=1" aria-current="page">1</a></li></ol>
  <a class="forma-pagination__control" href="?page=1" rel="next">Next</a>
  <span class="forma-pagination__summary">Page 1 of 1</span>
</nav>"""

let private cantFind = """<p>Can't find it? <a href="#s002-5">Ask us to add a charity</a>.</p>"""

let private requestForm (p: string) =
    let name = Ui.field(id = $"{p}-name", label = "Charity legal name (required)", attrs = "required autocomplete=\"organization\"", value = "Hopewell Community Trust")
    let ein = Ui.field(id = $"{p}-ein", label = "EIN, if you know it", desc = "Nine digits, for example 00-0000000.", attrs = "inputmode=\"numeric\"")
    let city = Ui.field(id = $"{p}-city", label = "City (required)", attrs = "required", value = "Decatur")
    let st = Ui.select(id = $"{p}-state", label = "State (required)", options = [ "", "Choose a state", false; "IL", "Illinois", true; "OR", "Oregon", false ])
    let web = Ui.field(id = $"{p}-web", label = "Website", ``type`` = "url", attrs = "autocomplete=\"url\"")
    tag
        "form"
        $$"""<form class="forma-form" id="{{p}}-form" method="get" action="#{{p}}-done" aria-labelledby="{{p}}-form-title">
<h3 id="{{p}}-form-title">Ask us to add a charity</h3>
<p>We verify the charity's identity and IRS status before it appears in search. This usually takes 3 business days.</p>
<div class="forma-form-row" data-forma-form-layout="grid">
{{name}}
{{ein}}
</div>
<div class="forma-form-row" data-forma-form-layout="grid">
{{city}}
{{st}}
{{web}}
</div>
<div class="forma-actions"><button type="submit">Send request</button><button type="reset">Clear form</button></div>
<p class="forma-form__outcome" id="{{p}}-done" role="status" aria-label="Request status">Request sent. Reference REQ-2026-00071. We will email you when the charity is verified.</p>
</form>"""

let screen: Screen =
    { Id = "S-002"
      File = "S-002-charity-search.html"
      Title = "Charity search and selection"
      ReqIds = [ "UNB-UI-S-002" ]
      Audience = "donor"
      Brand = true
      Lead = "Find a charity by name, alias or EIN, see its platform eligibility with an as-of date, and bind its canonical identity."
      States =
        [ state
              "Results: two charities with the same name and one on hold"
              "Donor on a phone"
              "UNB-UI-S-002 AC-1 (EIN and city/state per record), AC-2 ('On hold', no reason text, not selectable)"
              (fun p -> scope + toolbar p "Hope Foundation" "4 charities" + activeFilters p + results p + pagination + cantFind)
          state "Searching" "Donor" "Searching state with busy semantics (UNB-UI-G-013)" (fun p ->
              scope + toolbar p "Hope" "Searching" + skeleton "Searching charities")
          state "No results" "Donor" "Empty result with the next sensible action (UNB-UI-G-013)" (fun p ->
              scope
              + toolbar p "Hopewel Trust" "0 charities"
              + Ui.emptyState(
                  id = $"{p}-empty",
                  title = "No charities match “Hopewel Trust”",
                  text = "Check the spelling, search by EIN, or ask us to add the charity. New charities are verified before you can recommend a grant.",
                  action = """<a class="forma-button" href="#s002-5">Request a charity we don't list</a>"""
              ))
          state "Search unavailable" "Donor" "UNB-UI-G-012 fault presentation with a support reference" (fun p ->
              scope
              + toolbar p "Hope Foundation" "Search unavailable"
              + Ui.faultInline(
                  id = $"{p}-fault",
                  title = "Charity search is unavailable right now.",
                  message = "Your draft grant is kept. Try again in a few minutes, or quote the reference if you contact support.",
                  reference = "AG-7Q3KD",
                  actions = """<button type="button" data-forma-aegis-capability="CanRetry">Try search again</button>"""
              ))
          state "Request a charity we don't list" "Donor" "Identity-resolution request form opened from “Can't find it?”" (fun p -> scope + requestForm p) ]
      Coverage =
        [ cov "UNB-UI-S-002" "AC-1" Yes "Each record prints Legal name, EIN and City and state via data-grid data-label projection at 320 px (rendered/S-002-charity-search-320.png)."
          cov "UNB-UI-S-002" "AC-2" NeedsApplicationBehaviour "Markup shows 'On hold' (blocked) with no reason text and no Select control. Withholding another sponsor's reason and enforcing non-selection server-side are application behaviour."
          cov "UNB-UI-S-002" "statement" NeedsApplicationBehaviour "Search, filtering, sort and binding the canonical identity on Select are Limen/application behaviour; the data-grid sort buttons are hidden on phones so sort is offered in collection-toolbar." ]
      Gaps = [ "FORMA-GAP-06"; "FORMA-GAP-08" ]
      Render = None }

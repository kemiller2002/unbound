/// Pure markup helpers for the Unbound screen prototypes (port of tools/lib/markup.mjs).
/// Every helper returns a string built only from documented Forma 1.0.2 classes and canonical markup
/// (catalog/components/<id>.mjs, dist/patterns/<id>.html). No helper emits a style attribute or page-level CSS.
///
/// Convention: an optional string argument that JavaScript treated by truthiness (null, undefined or "") is the empty
/// string here, so `loz "" "Draft"` is `loz(null, "Draft")`. Helpers that took a JavaScript options object are static
/// members of `Ui` with named, optional arguments, so call sites read like the original object literals.
module Unbound.Designs.V1.Markup

let private ifs (cond: bool) (s: string) = if cond then s else ""
let private nonEmpty (s: string) = s <> ""

/// Public inert authoring tag around a canonical pattern (docs/AGENT-USAGE.md).
let tag (slug: string) (html: string) = $"<forma-{slug} class=\"forma-component-tag\">{html}</forma-{slug}>"

/// Status lozenge (G-001): the text is the authority, data-state only picks the glyph. state "" = no data-state.
let loz (state: string) (text: string) =
    $$"""<span class="forma-status-lozenge"{{ifs (nonEmpty state) $" data-state=\"{state}\""}}>{{text}}</span>"""

/// Key-value list. rows: (label, valueHtml) list.
let kvAttrs (attrs: string) (rows: (string * string) list) =
    let items = rows |> List.map (fun (k, v) -> $"<div><dt>{k}</dt><dd>{v}</dd></div>") |> String.concat ""
    $$"""<dl class="forma-key-value-list"{{attrs}}>{{items}}</dl>"""

let kv rows = kvAttrs "" rows

/// Low-context identifier (G-016).
let ident (v: string) = $"<span class=\"forma-identifier\">{v}</span>"

/// Money in tabular, slashed-zero numerals (G-017).
let money (v: string) = $"<span class=\"forma-identifier\">{v}</span>"

/// Consequential value marked for deliberate verification.
let criticalVerify (verify: bool) (v: string) =
    $$"""<span class="forma-critical-value"{{ifs verify " data-forma-verify=\"true\""}}>{{v}}</span>"""

let critical v = criticalVerify false v

/// Identifier with an adjacent copy control (G-016). Copy behaviour is application-owned.
let refWithCopy (label: string) (value: string) =
    $$"""<span class="forma-cluster">{{ident value}} <button type="button" aria-label="Copy {{label}} {{value}}">Copy</button></span>"""

/// Callout: kind = note | caution | evidence | decision.
let callout (id: string) (kind: string) (label: string) (body: string) =
    tag "callout" $$"""<aside class="forma-callout" data-forma-callout="{{kind}}" aria-labelledby="{{id}}"><p class="forma-callout__label" id="{{id}}">{{label}}</p>{{body}}</aside>"""

let validationMessage (id: string) (text: string) =
    $$"""<p class="forma-validation-message" id="{{id}}"><span class="forma-validation-message__mark" aria-hidden="true">!</span>{{text}}</p>"""

/// One timeline entry; body and source are optional ("").
type TimelineItem = { Iso: string; Shown: string; State: string; Label: string; Title: string; Body: string; Source: string }

/// One grant-progress dimension; Iso "" renders "No activity yet".
type Dimension = { Name: string; State: string; Label: string; Iso: string; Shown: string }

/// One work-queue item.
type WorkItem = { Title: string; State: string; Label: string; Text: string; Action: string; ActionLabel: string }

/// One data-grid column: label, sort state ("" = none), plain (visually hidden header, no sort button).
type Column = { Label: string; Sort: string; Plain: bool }

/// Timeline. items: TimelineItem list.
let timeline (label: string) (items: TimelineItem list) =
    let item (it: TimelineItem) =
        $$"""<li class="forma-timeline__item">
    <div class="forma-timeline__marker" aria-hidden="true"></div>
    <div class="forma-timeline__content">
      <div class="forma-timeline__meta"><time datetime="{{it.Iso}}">{{it.Shown}}</time>{{ifs (nonEmpty it.Label) (loz it.State it.Label)}}</div>
      <h4>{{it.Title}}</h4>
      {{ifs (nonEmpty it.Body) $"<p>{it.Body}</p>"}}
      {{ifs (nonEmpty it.Source) $"<small>{it.Source}</small>"}}
    </div>
  </li>"""
    tag
        "timeline"
        $$"""<ol class="forma-timeline" aria-label="{{label}}">
  {{items |> List.map item |> String.concat "\n  "}}
</ol>"""

/// Five-dimension grant progress (G-002) composed from Forma steps + status lozenge + freshness.
let grantProgress (id: string) (dims: Dimension list) =
    let dim (d: Dimension) =
        let at = if nonEmpty d.Iso then $"<p>As of <time datetime=\"{d.Iso}\">{d.Shown}</time></p>" else "<p>No activity yet</p>"
        $$"""<li><div class="forma-stack"><p><strong>{{d.Name}}</strong></p>{{loz d.State d.Label}}{{at}}</div></li>"""
    tag
        "steps"
        $$"""<ol class="forma-steps" role="list" aria-label="Grant progress by dimension" id="{{id}}">
  {{dims |> List.map dim |> String.concat "\n  "}}
</ol>"""

let skeleton (label: string) =
    tag
        "skeleton"
        $$"""<section class="forma-skeleton" aria-busy="true" aria-label="{{label}}">
  <span class="forma-visually-hidden">{{label}}</span>
  <div class="forma-skeleton__line forma-skeleton__line--title"></div>
  <div class="forma-skeleton__line"></div>
  <div class="forma-skeleton__line"></div>
  <div class="forma-skeleton__line forma-skeleton__line--short"></div>
</section>"""

let scopeTrail (crumbs: string list) =
    let last = List.length crumbs - 1
    let items =
        crumbs
        |> List.mapi (fun i c -> if i = last then $"<li aria-current=\"page\">{c}</li>" else $"<li><a href=\"#scope-{i}\">{c}</a></li>")
        |> String.concat ""
    tag "scope-trail" $$"""<nav class="forma-scope-trail" aria-label="Current scope"><ol>{{items}}</ol></nav>"""

let disclosure (summary: string) (body: string) =
    tag
        "disclosure"
        $$"""<details class="forma-disclosure" data-forma-motion-weight="light">
  <summary>{{summary}}</summary>
  <div class="forma-disclosure__content">{{body}}</div>
</details>"""

let mobileBar (label: string) (children: string) =
    tag "mobile-action-bar" $$"""<nav class="forma-mobile-action-bar" aria-label="{{label}}">{{children}}</nav>"""

let private describedBy (id: string) (desc: string) (error: string) =
    [ ifs (nonEmpty desc) $"{id}-description"; ifs (nonEmpty error) $"{id}-error" ]
    |> List.filter nonEmpty
    |> String.concat " "

let private attr (name: string) (value: string) = ifs (nonEmpty value) $" {name}=\"{value}\""
let private raw (attrs: string) = ifs (nonEmpty attrs) $" {attrs}"

/// Helpers that took a JavaScript options object, with the same names and defaults as named optional arguments.
[<AbstractClass; Sealed>]
type Ui =
    /// Freshness line (G-003). `state` is a lozenge state, or "" for plain state text.
    static member fresh(iso: string, shown: string, stateText: string, ?label: string, ?state: string) =
        let label = defaultArg label "As of"
        let state = defaultArg state ""
        let st = if nonEmpty state then loz state stateText else $"<span class=\"forma-freshness__state\">{stateText}</span>"
        $$"""<p class="forma-freshness"><span class="forma-freshness__label">{{label}}</span> <time datetime="{{iso}}">{{shown}}</time> {{st}}</p>"""

    /// Timeline entry.
    static member event(iso, shown, state, label, title, ?body, ?source) : TimelineItem =
        { Iso = iso; Shown = shown; State = state; Label = label; Title = title; Body = defaultArg body ""; Source = defaultArg source "" }

    /// Grant-progress dimension.
    static member dim(name, state, label, ?iso, ?shown) : Dimension =
        { Name = name; State = state; Label = label; Iso = defaultArg iso ""; Shown = defaultArg shown "" }

    /// Work-queue item.
    static member work(title, text, ?state, ?label, ?action, ?actionLabel) : WorkItem =
        { Title = title; State = defaultArg state ""; Label = defaultArg label ""; Text = text
          Action = defaultArg action ""; ActionLabel = defaultArg actionLabel "" }

    /// Data-grid column.
    static member col(label, ?sort, ?plain) : Column = { Label = label; Sort = defaultArg sort ""; Plain = defaultArg plain false }

    /// Operation status (G-006).
    static member opStatus(id: string, state: string, label: string, title: string, body: string, ?actions: string list, ?symbol: string, ?level: int) =
        let actions = defaultArg actions []
        let symbol = defaultArg symbol "↻"
        let level = defaultArg level 3
        let joined = String.concat "" actions
        let acts = ifs (not actions.IsEmpty) $"<div class=\"forma-operation-status__actions\">{joined}</div>"
        tag
            "operation-status"
            $$"""<section class="forma-operation-status" aria-labelledby="{{id}}">
  <div class="forma-operation-status__symbol" aria-hidden="true">{{symbol}}</div>
  <div>
    {{loz state label}}
    <h{{level}} id="{{id}}">{{title}}</h{{level}}>
    {{body}}
  </div>
  {{acts}}
</section>"""

    /// Text field in the .forma-field structure.
    static member field(id: string, label: string, ?``type``: string, ?desc: string, ?value: string, ?attrs: string, ?error: string) =
        let typ = defaultArg ``type`` "text"
        let desc = defaultArg desc ""
        let value = defaultArg value ""
        let attrs = defaultArg attrs ""
        let error = defaultArg error ""
        let db = describedBy id desc error
        $$"""<div class="forma-field">
  <label class="forma-field__label" for="{{id}}">{{label}}</label>
  {{ifs (nonEmpty desc) $"<span class=\"forma-field__description\" id=\"{id}-description\">{desc}</span>"}}
  <input id="{{id}}" name="{{id}}" type="{{typ}}"{{attr "value" value}}{{attr "aria-describedby" db}}{{ifs (nonEmpty error) " aria-invalid=\"true\""}}{{raw attrs}}>
  {{ifs (nonEmpty error) (validationMessage $"{id}-error" error)}}
</div>"""

    static member textarea(id: string, label: string, ?desc: string, ?value: string, ?attrs: string, ?error: string) =
        let desc = defaultArg desc ""
        let value = defaultArg value ""
        let attrs = defaultArg attrs ""
        let error = defaultArg error ""
        let db = describedBy id desc error
        $$"""<div class="forma-field">
  <label class="forma-field__label" for="{{id}}">{{label}}</label>
  {{ifs (nonEmpty desc) $"<span class=\"forma-field__description\" id=\"{id}-description\">{desc}</span>"}}
  <textarea id="{{id}}" name="{{id}}" rows="3"{{attr "aria-describedby" db}}{{ifs (nonEmpty error) " aria-invalid=\"true\""}}{{raw attrs}}>{{value}}</textarea>
  {{ifs (nonEmpty error) (validationMessage $"{id}-error" error)}}
</div>"""

    /// Native select in the Forma select-field structure. options: (value, label, selected) list.
    static member select(id: string, label: string, options: (string * string * bool) list, ?desc: string, ?attrs: string) =
        let desc = defaultArg desc ""
        let attrs = defaultArg attrs ""
        let opts =
            options
            |> List.map (fun (v, l, s) -> $"""<option value="{v}"{ifs s " selected"}>{l}</option>""")
            |> String.concat ""
        tag
            "select"
            $$"""<label class="forma-select-field">
  <span class="forma-field__label">{{label}}</span>
  <span class="forma-select" data-forma-motion-weight="standard">
    <select class="forma-select__input" id="{{id}}" name="{{id}}"{{ifs (nonEmpty desc) $" aria-describedby=\"{id}-description\""}}{{raw attrs}}>
      {{opts}}
    </select>
    <span class="forma-select__indicator" aria-hidden="true"></span>
  </span>
  {{ifs (nonEmpty desc) $"<span class=\"forma-field__description\" id=\"{id}-description\">{desc}</span>"}}
</label>"""

    static member checkbox(id: string, label: string, ?desc: string, ?``checked``: bool, ?attrs: string) =
        let desc = defaultArg desc ""
        let chk = defaultArg ``checked`` false
        let attrs = defaultArg attrs ""
        tag
            "checkbox"
            $$"""<label class="forma-checkbox" data-forma-motion-weight="standard">
  <input class="forma-checkbox__input" type="checkbox" id="{{id}}" name="{{id}}" value="yes"{{ifs chk " checked"}}{{ifs (nonEmpty desc) $" aria-describedby=\"{id}-description\""}}{{raw attrs}}>
  <span class="forma-checkbox__box" aria-hidden="true"></span>
  <span class="forma-checkbox__text">
    <span class="forma-checkbox__label">{{label}}</span>
    {{ifs (nonEmpty desc) $"<span class=\"forma-checkbox__description\" id=\"{id}-description\">{desc}</span>"}}
  </span>
</label>"""

    /// Choice group. choices: (value, title, text, checked) list.
    static member choiceGroup(name: string, legend: string, choices: (string * string * string * bool) list, ?attrs: string) =
        let attrs = defaultArg attrs ""
        let choice (v: string, t: string, d: string, c: bool) =
            $$"""<label class="forma-choice">
    <input type="radio" name="{{name}}" value="{{v}}"{{ifs c " checked"}}>
    <span class="forma-choice__content"><strong>{{t}}</strong><span>{{d}}</span></span>
  </label>"""
        tag
            "choice-group"
            $$"""<fieldset class="forma-choice-group"{{raw attrs}}>
  <legend class="forma-choice-group__legend">{{legend}}</legend>
  {{choices |> List.map choice |> String.concat ""}}
</fieldset>"""

    /// Validation summary (G-012). items: (href, text) list.
    static member validationSummary(id: string, title: string, items: (string * string) list) =
        let lis = items |> List.map (fun (h, t) -> $"<li><a href=\"{h}\">{t}</a></li>") |> String.concat ""
        tag
            "validation-summary"
            $$"""<div class="forma-validation-summary" role="alert" tabindex="-1" aria-labelledby="{{id}}">
  <div class="forma-validation-summary__icon" aria-hidden="true">!</div>
  <div>
    <h3 class="forma-validation-summary__title" id="{{id}}">{{title}}</h3>
    <ul class="forma-validation-summary__list">{{lis}}</ul>
  </div>
</div>"""

    /// Modal dialog with invoker (G-005). The dialog is closed in the static page; verify-screens opens it.
    static member dialog(id: string, invoker: string, title: string, desc: string, actions: string, ?body: string, ?invokerAttrs: string) =
        let body = defaultArg body ""
        let invokerAttrs = defaultArg invokerAttrs ""
        tag
            "dialog"
            $$"""<button type="button" commandfor="{{id}}" command="show-modal"{{raw invokerAttrs}}>{{invoker}}</button>
<dialog class="forma-dialog" id="{{id}}" data-forma-motion-weight="heavy" aria-labelledby="{{id}}-title" aria-describedby="{{id}}-description">
  <form method="dialog">
    <div class="forma-dialog__body">
      <h2 id="{{id}}-title">{{title}}</h2>
      <p id="{{id}}-description">{{desc}}</p>
      {{body}}
    </div>
    <div class="forma-dialog__actions">{{actions}}</div>
  </form>
</dialog>"""

    /// Data grid with record projection on phones. rows: cell html lists.
    static member dataGrid(label: string, cols: Column list, rows: string list list) =
        let colArr = List.toArray cols
        let th (c: Column) =
            if c.Plain then $"<th scope=\"col\"><span class=\"forma-visually-hidden\">{c.Label}</span></th>"
            else $"""<th scope="col"{attr "aria-sort" c.Sort}><button type="button">{c.Label}</button></th>"""
        let tr (r: string list) =
            let cells = r |> List.mapi (fun i cell -> $"<td data-label=\"{colArr.[i].Label}\">{cell}</td>") |> String.concat ""
            $"<tr>{cells}</tr>"
        tag
            "data-grid"
            $$"""<div class="forma-data-grid" role="region" tabindex="0" aria-label="{{label}}">
  <table>
    <caption class="forma-visually-hidden">{{label}}</caption>
    <thead><tr>{{cols |> List.map th |> String.concat ""}}</tr></thead>
    <tbody>
      {{rows |> List.map tr |> String.concat "\n      "}}
    </tbody>
  </table>
</div>"""

    /// Work queue.
    static member workQueue(id: string, title: string, count: string, items: WorkItem list, ?kicker: string, ?countState: string, ?level: int) =
        let kicker = defaultArg kicker ""
        let countState = defaultArg countState ""
        let level = defaultArg level 3
        let item (it: WorkItem) =
            let lz = ifs (nonEmpty it.Label) $"{loz it.State it.Label} "
            let btn = ifs (nonEmpty it.Action) $"""<button type="button"{attr "aria-label" it.ActionLabel}>{it.Action}</button>"""
            $$"""<li class="forma-work-queue__item">
      <div><strong>{{it.Title}}</strong><p>{{lz}}{{it.Text}}</p></div>
      {{btn}}
    </li>"""
        tag
            "work-queue"
            $$"""<section class="forma-work-queue" aria-labelledby="{{id}}">
  <header class="forma-work-queue__header">
    <div>{{ifs (nonEmpty kicker) $"<span class=\"forma-component-kicker\">{kicker}</span>"}}<h{{level}} id="{{id}}">{{title}}</h{{level}}></div>
    {{loz countState count}}
  </header>
  <ol class="forma-work-queue__list">
    {{items |> List.map item |> String.concat "\n    "}}
  </ol>
</section>"""

    /// Metric card.
    static member metric(label: string, value: string, ?context: string, ?extra: string, ?id: string) =
        let context = defaultArg context ""
        let extra = defaultArg extra ""
        let id = defaultArg id ""
        tag
            "metric-card"
            $$"""<article class="forma-metric-card"{{attr "aria-labelledby" id}}>
  <div class="forma-metric-card__label"{{attr "id" id}}>{{label}}</div>
  <div class="forma-metric-card__value">{{value}}</div>
  {{ifs (nonEmpty context) $"<div class=\"forma-metric-card__context\">{context}</div>"}}
  {{extra}}
</article>"""

    static member emptyState(id: string, title: string, text: string, action: string, ?level: int) =
        let level = defaultArg level 3
        tag
            "empty-state"
            $$"""<section class="forma-empty-state" aria-labelledby="{{id}}">
  <div class="forma-empty-state__symbol" aria-hidden="true">□</div>
  <h{{level}} id="{{id}}">{{title}}</h{{level}}>
  <p>{{text}}</p>
  {{action}}
</section>"""

    static member faultInline(id: string, title: string, message: string, reference: string, actions: string) =
        tag
            "fault-inline"
            $$"""<div class="forma-fault forma-fault--inline" id="{{id}}" data-forma-intent="inline" data-forma-severity="error" aria-labelledby="{{id}}-title">
  <div class="forma-fault__marker" aria-hidden="true">!</div>
  <div class="forma-fault__body">
    <p class="forma-fault__severity">Error</p>
    <p class="forma-fault__title" id="{{id}}-title"><strong>{{title}}</strong></p>
    <p class="forma-fault__message">{{message}}</p>
    <p class="forma-fault-reference">Reference <code>{{reference}}</code></p>
  </div>
  <div class="forma-recovery-actions" role="group" aria-label="Recovery actions">{{actions}}</div>
</div>"""

    static member recordHeader(``type``: string, title: string, lozenge: string, sub: string, crumbs: string, ?actions: string, ?more: string) =
        let actions = defaultArg actions ""
        let more = defaultArg more ""
        let menu = ifs (nonEmpty more) $"<details><summary>More</summary><div class=\"forma-record-header__menu\">{more}</div></details>"
        tag
            "record-header"
            $$"""<header class="forma-record-header">
  <div class="forma-record-header__main">
    <nav aria-label="Breadcrumb">{{crumbs}}</nav>
    <div class="forma-record-header__title">
      <div><span class="forma-record-header__type">{{``type``}}</span><h3>{{title}}</h3></div>
      {{lozenge}}
    </div>
    <p>{{sub}}</p>
  </div>
  <div class="forma-record-header__actions">
    {{actions}}
    {{menu}}
  </div>
</header>"""

    static member feedItem(id: string, source: string, title: string, state: string, label: string, iso: string, shown: string, summary: string, ?action: string) =
        let action = defaultArg action ""
        tag
            "feed-item"
            $$"""<article class="forma-feed-item" aria-labelledby="{{id}}">
  <div class="forma-feed-item__primary">
    <p class="forma-feed-item__source">{{source}}</p>
    <h4 id="{{id}}">{{title}}</h4>
  </div>
  <div class="forma-feed-item__meta">{{loz state label}}<time datetime="{{iso}}">{{shown}}</time></div>
  <p class="forma-feed-item__summary">{{summary}}</p>
  {{ifs (nonEmpty action) $"<div class=\"forma-feed-item__actions\">{action}</div>"}}
</article>"""

    /// A labelled design-state section on a prototype page.
    static member stateSection(id: string, n: int, total: int, persona: string, title: string, demonstrates: string, body: string) =
        $$"""<section class="forma-section" id="{{id}}" aria-labelledby="{{id}}-title">
  <header class="forma-section-heading">
    <div class="forma-section-heading__text">
      <p class="forma-eyebrow">Design state {{n}} of {{total}} · {{persona}}</p>
      <h2 id="{{id}}-title">{{title}}</h2>
      <p>Demonstrates: {{demonstrates}}</p>
    </div>
  </header>
  {{body}}
</section>"""

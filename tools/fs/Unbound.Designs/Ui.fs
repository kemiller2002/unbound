/// Pure markup helpers for the v2 Unbound prototypes (report 18); port of v2/tools/lib/ui.mjs.
/// Data in, HTML string out. No helper reads state or writes files.
/// Classes: Forma 1.0.2 (forma-*) where a verified component fits, otherwise the
/// proposed application layer (unb-*, ../unbound-app.css) whose blocks name their gap.
///
/// Icons: `icon name` emits a placeholder; `resolveIcons registry html` expands every placeholder into the Forma SVG.
/// The registry (vendor/forma-1.0.2/icons-registry.json) is read at the program edge, so every helper stays pure.
module Unbound.Designs.Ui

open System.Text.RegularExpressions
open Unbound.Designs.JsCompat

let private ifs (c: bool) (s: string) = if c then s else ""

// ---------- icons ----------

/// Icon name -> shapes; each shape is the registry object ({element, ...attributes}) in key order.
type IconRegistry = Map<string, (string * JsValue) list list>

/// Builds the registry from the parsed icons-registry.json.
let iconRegistry (json: JsValue) : IconRegistry =
    match prop "icons" json with
    | Some(JArr icons) ->
        icons
        |> List.map (fun i ->
            let shapes =
                match prop "shapes" i with
                | Some(JArr ss) -> ss |> List.map (function JObj kvs -> kvs | _ -> [])
                | _ -> []
            interpolate (prop "name" i), shapes)
        |> Map.ofList
    | _ -> failwith "icons-registry.json has no icons array"

let private attrs (kvs: (string * JsValue) list) : string =
    kvs
    |> List.choose (fun (k, v) ->
        match v with
        | JNull
        | JBool false -> None
        | JBool true -> Some(" " + k)
        | v -> Some($" {k}=\"" + interpolate (Some v) + "\""))
    |> String.concat ""

let private shape (kvs: (string * JsValue) list) : string =
    let element = kvs |> List.tryFind (fst >> (=) "element") |> Option.map snd
    let rest = kvs |> List.filter (fst >> (<>) "element")
    $"<{interpolate element}{attrs rest}/>"

let private iconSvg (registry: IconRegistry) (name: string) : string =
    match registry.TryFind name with
    | None -> failwith $"icon not in Forma registry: {name}"
    | Some shapes ->
        """<svg class="forma-icon__svg" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true" focusable="false">"""
        + (shapes |> List.map shape |> String.concat "")
        + "</svg>"

/// Forma icon (registry-backed, decorative: the adjacent text carries meaning). A placeholder until resolveIcons.
let icon (name: string) : string = "\u0001icon:" + name + "\u0002"

let private placeholder = Regex("\u0001icon:([^\u0002]*)\u0002", RegexOptions.CultureInvariant)

/// Expands every icon placeholder into its SVG; fails on a name not in the registry.
let resolveIcons (registry: IconRegistry) (html: string) : string =
    placeholder.Replace(html, MatchEvaluator(fun m -> iconSvg registry m.Groups.[1].Value))

// ---------- numbers, money, dates (UNB-UI-F-*) ----------

/// Money: one unbreakable token, tabular figures, true minus sign, text label for negatives.
let moneyWith (hero: bool) (signed: bool) (amount: float) : string =
    let negative = amount < 0.0
    let parts = (formatEnUs2 (abs amount)).Split('.')
    let whole, cents = parts.[0], parts.[1]
    let sign = if negative then "−" elif signed && amount > 0.0 then "+" else ""
    let centsHtml = if hero then $"""<span class="unb-money__cents">.{cents}</span>""" else $".{cents}"
    $"""<span class="unb-money"{ifs negative " data-sign=\"negative\""}>{sign}${whole}{centsHtml}</span>"""

let money (amount: float) = moneyWith false false amount
let moneyHero (amount: float) = moneyWith true false amount
let moneySigned (amount: float) = moneyWith false true amount

let num (v: string) = $"""<span class="unb-num">{v}</span>"""
let time (iso: string) (shown: string) = $"""<time datetime="{iso}">{shown}</time>"""

// ---------- status ----------
let stateIcon (state: string) : string =
    match state with
    | "ok" -> "success"
    | "attention" -> "pending"
    | "blocked" -> "error"
    | "info" -> "info"
    | "unknown" -> "help-circle"
    | "neutral" -> "clock"
    | s -> failwith $"unknown state: {s}"

/// Status chip: sentence-case words + glyph; state never by colour alone (G-001 r1).
let chip (state: string) (text: string) =
    $"""<span class="unb-chip" data-state="{state}">{icon (stateIcon state)}{text}</span>"""

// ---------- structure ----------

/// panel arguments; "" stands for an absent (falsy) value.
type Panel =
    { Id: string; Title: string; TitleTag: string; Meta: string; Body: string; Foot: string; Tone: string; Label: string }

let P : Panel = { Id = "undefined"; Title = ""; TitleTag = "h2"; Meta = ""; Body = "undefined"; Foot = ""; Tone = ""; Label = "" }

let panel (p: Panel) : string =
    let labelled =
        if p.Title <> "" then $" aria-labelledby=\"{p.Id}-t\""
        elif p.Label <> "" then $" aria-label=\"{p.Label}\""
        else ""
    let head = ifs (p.Title <> "") $"""<header class="unb-panel__head"><{p.TitleTag} id="{p.Id}-t">{p.Title}</{p.TitleTag}>{p.Meta}</header>"""
    let foot = ifs (p.Foot <> "") $"""<footer class="unb-panel__foot">{p.Foot}</footer>"""
    $"""<section class="unb-panel"{ifs (p.Tone <> "") $" data-tone=\"{p.Tone}\""}{labelled}>
{head}
<div class="unb-panel__body">{p.Body}</div>{foot}
</section>"""

/// notice arguments; IconName None falls back to the state's glyph, Role "" is absent.
type Notice = { State: string; IconName: string option; Title: string; Body: string; Actions: string list; Role: string }

let N : Notice = { State = "undefined"; IconName = None; Title = "undefined"; Body = "undefined"; Actions = []; Role = "" }

let notice (n: Notice) : string =
    let actions = ifs (not n.Actions.IsEmpty) $"""<div class="unb-notice__actions">{String.concat "" n.Actions}</div>"""
    $"""<section class="unb-notice" data-state="{n.State}"{ifs (n.Role <> "") $" role=\"{n.Role}\""} aria-label="{n.Title}">{icon (defaultArg n.IconName (stateIcon n.State))}
<p class="unb-notice__title">{n.Title}</p>
<div class="unb-notice__body">{n.Body}</div>{actions}
</section>"""

/// button options; "" stands for an absent value.
type Button = { Primary: bool; Href: string; Id: string; DescribedBy: string }

let B : Button = { Primary = false; Href = ""; Id = ""; DescribedBy = "" }

let buttonWith (o: Button) (label: string) : string =
    let primary = ifs o.Primary " data-forma-variant=\"primary\""
    if o.Href <> "" then
        $"""<a class="forma-button"{primary} href="{o.Href}"{ifs (o.Id <> "") $" id=\"{o.Id}\""}>{label}</a>"""
    else
        $"""<button class="forma-button" type="button"{primary}{ifs (o.DescribedBy <> "") $" aria-describedby=\"{o.DescribedBy}\""}>{label}</button>"""

/// button(label) with no options.
let button (label: string) = buttonWith B label
/// button(label, { href }).
let link (href: string) (label: string) = buttonWith { B with Href = href } label
/// button(label, { primary: true, href }).
let primaryLink (href: string) (label: string) = buttonWith { B with Primary = true; Href = href } label
/// button(label, { primary: true }).
let primary (label: string) = buttonWith { B with Primary = true } label

/// quiet(label, { href, iconName }); iconName "" for none.
let quiet (label: string) (href: string) (iconName: string) =
    $"""<a class="unb-btn-quiet" href="{href}">{label}{ifs (iconName <> "") (icon iconName)}</a>"""

let dl (rows: (string * string) list) : string =
    let items = rows |> List.map (fun (k, v) -> $"<div><dt>{k}</dt><dd>{v}</dd></div>") |> String.concat ""
    $"""<dl class="unb-dl">{items}</dl>"""

/// A thing that needs the user.
type Task = { State: string; IconName: string option; Title: string; Text: string; Actions: string list }

let tasks (items: Task list) : string =
    let li (t: Task) =
        $"""<li class="unb-task" data-state="{t.State}"><span class="unb-task__icon">{icon (defaultArg t.IconName (stateIcon t.State))}</span>
<p class="unb-task__title">{t.Title}</p><p class="unb-task__text">{t.Text}</p>
<div class="unb-task__action">{String.concat "" t.Actions}</div></li>"""
    $"""<ul class="unb-tasks">{items |> List.map li |> String.concat ""}</ul>"""

/// Ledger row; amounts right-aligned tabular.
type LedgerRow = { What: string; Sub: string; Status: string; Date: string; Amount: string }

let ledger (caption: string) (hideCaption: bool) (rows: LedgerRow list) : string =
    let tr (r: LedgerRow) =
        $"""<tr><td data-col="what"><span class="unb-ledger__primary">{r.What}</span><br><span class="unb-ledger__secondary">{r.Sub}</span></td>
<td data-col="status">{r.Status}</td><td data-col="date">{r.Date}</td><td data-col="amount">{r.Amount}</td></tr>"""
    $"""<table class="unb-ledger"><caption{ifs hideCaption " class=\"forma-visually-hidden\""}>{caption}</caption>
<thead><tr><th scope="col">Activity</th><th scope="col">Status</th><th scope="col" data-col="date">Date</th><th scope="col" data-col="amount">Amount</th></tr></thead>
<tbody>{rows |> List.map tr |> String.concat ""}</tbody></table>"""

let steps (list: (string * string) list) : string =
    let li (label: string, st: string) =
        let hidden =
            match st with
            | "done" -> """<span class="forma-visually-hidden"> (done)</span>"""
            | "failed" -> """<span class="forma-visually-hidden"> (failed)</span>"""
            | _ -> ""
        $"""<li data-step="{st}"{ifs (st = "current") " aria-current=\"step\""}>{label}{hidden}</li>"""
    $"""<ol class="unb-steps">{list |> List.map li |> String.concat ""}</ol>"""

let skel (shape: string) = $"""<span class="unb-skel" data-shape="{shape}"></span>"""

// ---------- frame ----------
let marks (brand: string) : string =
    match brand with
    | "unbound-default" ->
        """<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M6 5h5v13a5 5 0 0 0 10 0V5h5v13a10 10 0 0 1-20 0Z"/></svg>"""
    | "example-harbor" ->
        """<svg viewBox="0 0 32 32" aria-hidden="true" focusable="false"><path fill="currentColor" d="M16 3 4 12v3h24v-3Zm-9 14h4v8H7Zm7 0h4v8h-4Zm7 0h4v8h-4ZM4 27h24v3H4Z"/></svg>"""
    | b -> failwith $"unknown brand: {b}"

type Brand = { Name: string; Css: string list; Sponsor: string; Ein: string; Tz: string }

let brands (brand: string) : Brand =
    match brand with
    | "unbound-default" ->
        { Name = "Unbound Giving"
          Css = [ "../../../vendor/unbound-brands/unbound-default.css"; "../../../vendor/echelon-theme-1.1.0/echelon-fonts.css" ]
          Sponsor = "Example Community Foundation"
          Ein = "00-0000001"
          Tz = "CT" }
    | "example-harbor" ->
        { Name = "Harbor Giving"
          Css = [ "../../../vendor/forma-1.0.2/brands/example-harbor.css" ]
          Sponsor = "Example Charitable Foundation"
          Ein = "00-0000000"
          Tz = "CT" }
    | b -> failwith $"unknown brand: {b}"

let private nav (audience: string) : (string * string * string) list =
    match audience with
    | "donor" ->
        [ "home", "Home", "home"
          "grants", "Grants", "send"
          "money", "Contributions", "download"
          "documents", "Documents", "file-text"
          "people", "People", "users" ]
    | "staff" ->
        [ "inbox", "Work queue", "inbox"
          "cases", "Cases", "folder"
          "recon", "Reconciliation", "workflow"
          "funds", "Funds", "database"
          "policy", "Policy", "settings" ]
    | "charity" -> [ "received", "Grants received", "inbox"; "org", "Organization", "globe" ]
    | a -> failwith $"unknown audience: {a}"

let private tabKeys (audience: string) : string list =
    match audience with
    | "donor" -> [ "home"; "grants"; "money"; "documents" ]
    | "staff" -> [ "inbox"; "cases"; "recon"; "funds" ]
    | "charity" -> [ "received"; "org" ]
    | a -> failwith $"unknown audience: {a}"

let private currentAttr (key: string) (current: string) = ifs (key = current) " aria-current=\"page\""

let private navItems (audience: string) (current: string) (counts: (string * int) list) : string =
    nav audience
    |> List.map (fun (key, label, ic) ->
        let count =
            match counts |> List.tryFind (fst >> (=) key) with
            | Some(_, n) when n <> 0 ->
                $"""<span class="unb-sidenav__count">{n}<span class="forma-visually-hidden"> items</span></span>"""
            | _ -> ""
        $"""<li><a href="#{key}"{currentAttr key current}>{icon ic}<span>{label}</span>{count}</a></li>""")
    |> String.concat ""

let private tabbar (audience: string) (current: string) : string =
    let keys = tabKeys audience
    let links =
        nav audience
        |> List.filter (fun (k, _, _) -> List.contains k keys)
        |> List.map (fun (key, label, ic) -> $"""<a href="#{key}"{currentAttr key current}>{icon ic}<span>{label}</span></a>""")
        |> String.concat ""
    $"""<nav class="unb-tabbar" aria-label="Primary" data-tabs="{keys.Length}">{links}</nav>"""

type User = { Name: string; Initials: string; Role: string option }

/// Full application page for one state. ("" stands for an absent appContext; mobile action bars of the JavaScript
/// source were never rendered and are not carried.)
type Page =
    { Title: string
      Brand: string
      Audience: string
      Nav: string
      User: User
      AppContext: string
      Notifications: int
      Pagehead: string
      Body: string
      Counts: (string * int) list
      ExtraCss: string list option
      Theme: string option
      Footer: bool
      DocTitle: string option }

let page : Page =
    { Title = "undefined"
      Brand = "unbound-default"
      Audience = "donor"
      Nav = ""
      User = { Name = "undefined"; Initials = "undefined"; Role = None }
      AppContext = ""
      Notifications = 0
      Pagehead = "undefined"
      Body = "undefined"
      Counts = []
      ExtraCss = None
      Theme = None
      Footer = true
      DocTitle = None }

/// Legal-sponsor disclosure (G-014), protected text (G-007). Quiet, always present.
let legal (b: Brand) : string =
    $"""<footer class="unb-legal unb-small unb-muted" aria-label="Legal sponsor disclosure">
<p>{b.Name} is a donor-advised fund program of {b.Sponsor} (EIN <span class="unb-num">{b.Ein}</span>), a public charity. {b.Sponsor} has exclusive legal control over contributed assets. Grant recommendations are advisory and are approved or declined by {b.Sponsor}.</p>
<p><a href="#terms">Program terms</a> · <a href="#privacy">Privacy</a> · <a href="#accessibility">Accessibility</a></p>
</footer>"""

let appPage (cfg: Page) : string =
    let b = brands cfg.Brand
    let contextLabel =
        if cfg.AppContext <> "" then cfg.AppContext
        elif cfg.Audience = "staff" then "Sponsor operations"
        elif cfg.Audience = "charity" then "Charity portal"
        else "Donor-advised funds"
    let n = cfg.Notifications
    let css = b.Css |> List.map (fun h -> $"""<link rel="stylesheet" href="{h}">""") |> String.concat "\n"
    let extra =
        defaultArg cfg.ExtraCss []
        |> List.map (fun h -> $"""
<link rel="stylesheet" href="../../{h}">""")
        |> String.concat ""
    let role = cfg.User.Role |> Option.filter ((<>) "") |> Option.map (fun r -> $"""<span class="unb-user__role"> · {r}</span>""")
    $"""<!doctype html>
<html lang="en" data-forma-theme="{defaultArg cfg.Theme "light"}">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>{defaultArg cfg.DocTitle cfg.Title} · {b.Name}</title>
<link rel="stylesheet" href="../../../vendor/forma-1.0.2/all.css">
{css}
<link rel="stylesheet" href="../../unbound-app.css">{extra}
</head>
<body class="unb-app" data-forma-brand="{cfg.Brand}">
<a class="unb-skip" href="#main">Skip to main content</a>
<header class="unb-appbar">
  <a class="unb-appbar__brand" href="#home"><span class="unb-appbar__mark">{marks cfg.Brand}</span>{b.Name}</a>
  <span class="unb-appbar__context">{contextLabel}</span>
  <div class="unb-appbar__end">
    <button class="unb-iconbtn unb-hide-narrow" type="button" aria-label="Help">{icon "help-circle"}</button>
    <button class="unb-iconbtn" type="button" aria-label="Notifications{ifs (n <> 0) $", {n} unread"}">{icon "bell"}{ifs (n <> 0) $"<span class=\"unb-iconbtn__dot\" aria-hidden=\"true\">{n}</span>"}</button>
    <button class="unb-user" type="button" aria-label="Account menu for {cfg.User.Name}"><span class="unb-avatar" aria-hidden="true">{cfg.User.Initials}</span><span>{cfg.User.Name}{defaultArg role ""}</span>{icon "chevron-down"}</button>
    <button class="unb-iconbtn unb-only-compact" type="button" aria-label="Menu" data-unb-menu>{icon "menu"}</button>
  </div>
</header>
<div class="unb-frame">
  <nav class="unb-sidenav" aria-label="Primary"><ul class="unb-sidenav__list">{navItems cfg.Audience cfg.Nav cfg.Counts}</ul></nav>
  <main class="unb-main" id="main" tabindex="-1">
    <div class="unb-main__inner">
{cfg.Pagehead}
{cfg.Body}
{ifs cfg.Footer (legal b)}
    </div>
  </main>
</div>
{tabbar cfg.Audience cfg.Nav}
</body>
</html>
"""

/// pagehead arguments; Sub "" is absent.
type Pagehead = { Context: string list; Title: string; Sub: string; Actions: string list; FundSwitch: bool }

let H : Pagehead = { Context = []; Title = "undefined"; Sub = ""; Actions = []; FundSwitch = false }

/// Page header: context line, title, actions (desktop/tablet; phone uses the sticky bar).
let pagehead (h: Pagehead) : string =
    let context =
        ifs (not h.Context.IsEmpty) $"""<p class="unb-pagehead__context">{String.concat "<span aria-hidden=\"true\">/</span>" h.Context}</p>"""
    let title =
        if h.FundSwitch then
            $"""<h1><button class="unb-fundswitch" type="button" aria-haspopup="listbox" aria-label="{h.Title}. Switch fund">{h.Title}{icon "chevron-down"}</button></h1>"""
        else $"<h1>{h.Title}</h1>"
    let sub = ifs (h.Sub <> "") $"""<p class="unb-muted unb-small">{h.Sub}</p>"""
    let actions = ifs (not h.Actions.IsEmpty) $"""<div class="unb-pagehead__actions">{String.concat "" h.Actions}</div>"""
    $"""<header class="unb-pagehead">
{context}
<div class="unb-pagehead__row"><div>{title}{sub}</div>{actions}</div>
</header>"""

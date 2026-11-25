/// verify-screens: verifies the Forma screen prototypes in screens/ (port of tools/verify-screens.mjs).
///
/// For every page, at 320 and 1280 CSS px (deviceScaleFactor 1):
///   - full-page screenshot to rendered/<page>-<width>.png
///   - page-level horizontal scroll check: documentElement.scrollWidth <= viewport width
///   - axe-core (wcag2a, wcag2aa, wcag21a, wcag21aa, wcag22aa)
///   - every <dialog> on the page is opened as a modal by this harness (the pages themselves carry
///     no script), screenshotted to rendered/<page>-<dialog-id>-<width>.png and axe-checked while open
/// plus static rule checks: no <style>, no style= attributes, no <script>.
/// Results are merged with screens/page-manifest.json into screens/verification.json.
module Unbound.Designs.Verify.Screens

open System
open System.IO
open System.Text.RegularExpressions
open Unbound.Designs.Verify.JsCompat

let widths = [ 320; 1280 ]
let axeTags = [ "wcag2a"; "wcag2aa"; "wcag21a"; "wcag21aa"; "wcag22aa" ]

/// Class roots that do not share their catalog id's spelling.
let classAliases =
    Map [
        "forma-field", "text-field"
        "forma-choice", "choice-group"
        "forma-turn", "conversation"
        "forma-rule-group", "rule-builder"
        "forma-rule-clause", "rule-builder"
        "forma-select-field", "select"
        "forma-critical-value", "identifier"
        "forma-site", "marketing-shell"
        "forma-site__main", "marketing-shell"
        "forma-overflow-commands", "overflow-command-disclosure"
        "forma-obligation", "obligation-panel"
        "forma-obligation-list", "obligation-panel"
        "forma-fault-reference", "fault-reference"
        "forma-diff-row", "diff-viewer"
        "forma-binary-option", "binary-choice"
        "forma-status-lozenge-set", "status-lozenge"
        "forma-eyebrow", "text-roles"
        "forma-lead", "text-roles"
        "forma-component-kicker", "text-roles"
        "forma-actions", "button"
        "forma-button", "button"
    ]

// ---------- pure helpers ----------

let parseCatalogIds (csv: string) : string list =
    csv.Split '\n'
    |> Array.skip 1
    |> Array.choose (fun line -> let m = Regex.Match(line, "^\"([^\"]+)\"") in if m.Success then Some m.Groups.[1].Value else None)
    |> List.ofArray

let classRoot (cls: string) = Regex.Replace(Regex.Replace(cls, "__.*$", ""), "--.*$", "")

let private ws = Regex("[" + jsWs + "]+")

let componentsUsed (html: string) (catalogIds: string list) : string list =
    let known = Set.ofList catalogIds
    let fromTags = [ for m in Regex.Matches(html, "<forma-([a-z0-9-]+) class=\"forma-component-tag\"") -> m.Groups.[1].Value ]
    let classes = [ for m in Regex.Matches(html, "class=\"([^\"]+)\"") do yield! ws.Split m.Groups.[1].Value ]
    let fromClasses =
        classes
        |> List.filter (fun c -> c.StartsWith "forma-" && c <> "forma-component-tag")
        |> List.collect (fun c ->
            [ classAliases.TryFind c; classAliases.TryFind(classRoot c); Some(Regex.Replace(classRoot c, "^forma-", "")) ])
        |> List.choose id
        |> List.filter (fun id -> id <> "" && known.Contains id)
    let buttonsPresent = if Regex.IsMatch(html, @"<button\b", RegexOptions.ECMAScript) then [ "button" ] else []
    (fromTags |> List.filter known.Contains) @ fromClasses @ buttonsPresent
    |> List.distinct
    |> List.sortWith (fun a b -> String.CompareOrdinal(a, b))

let staticRuleViolations (html: string) : string list =
    [ if Regex.IsMatch(html, "<style[" + jsWs + ">]", RegexOptions.IgnoreCase) then "page contains a <style> element"
      if Regex.IsMatch(html, "[" + jsWs + "]style[" + jsWs + "]*=", RegexOptions.IgnoreCase) then "page contains a style attribute"
      if Regex.IsMatch(html, "<script[" + jsWs + ">]", RegexOptions.IgnoreCase) then "page contains a <script> element" ]

/// axe violations as recorded: id, impact, help, node count and the first five targets.
let summarizeViolations (violations: Json list) : Json list =
    violations
    |> List.map (fun v ->
        objOpt [
            "id", prop "id" v
            "impact", prop "impact" v
            "help", prop "help" v
            "nodes", Some(jint (asList (v ? nodes)).Length)
            "targets", Some(JArr(asList (v ? nodes) |> List.truncate 5 |> List.map (fun n -> JStr(jsJoin " " (asList (n ? target))))))
        ])

let pageName (file: string) = Regex.Replace(file, @"\.html$", "")

let private ruleCount (w: Json) = asList (w ? axe_violations) |> List.length

let totals (results: Json list) : Json =
    let flat = results |> List.collect (fun r -> widths |> List.map (fun w -> r ? widths ? (string w)))
    let dialogViolations = flat |> List.collect (fun w -> asList (w ? dialogs) |> List.collect (fun d -> asList (d ? axe_violations)))
    JObj [
        "pages", jint results.Length
        "page_width_checks", jint flat.Length
        "h_scroll_failures", jint (flat |> List.filter (fun w -> truthy (w ? h_scroll)) |> List.length)
        "axe_violation_rules_page", jint (flat |> List.sumBy ruleCount)
        "axe_violation_nodes_page", num (flat |> List.sumBy (fun w -> asList (w ? axe_violations) |> List.sumBy (fun v -> asNum (v ? nodes))))
        "dialog_checks", jint (flat |> List.sumBy (fun w -> asList (w ? dialogs) |> List.length))
        "axe_violation_rules_dialogs", jint dialogViolations.Length
        "static_rule_violations", jint (results |> List.sumBy (fun r -> asList (r ? static_rule_violations) |> List.length))
    ]

let pageRecord (file: string) (manifestEntry: Json option) (components: string list) (staticViolations: string list) (byWidth: (int * Json) list) : Json =
    let field k = manifestEntry |> Option.bind (prop k) |> Option.filter (fun j -> j <> JNull) |> Option.defaultValue (JArr [])
    JObj [
        "page", str file
        "requirement_ids", field "requirement_ids"
        "forma_components_used", strs components
        "static_rule_violations", strs staticViolations
        "states", field "states"
        "widths", obj (byWidth |> List.map (fun (w, j) -> string w, j))
        "requirement_coverage", field "requirement_coverage"
        "gaps", field "gaps"
    ]

let report (results: Json list) : Json =
    JObj [
        "generated_by", str "tools/verify-screens.mjs"
        "forma", JObj [ "version", str "1.0.2"; "commit", str "d7c499e"; "stylesheet", str "vendor/forma-1.0.2/all.css" ]
        "axe", JObj [ "version", str "4.10.2"; "tags", strs axeTags ]
        "widths", JArr(widths |> List.map jint)
        "totals", totals results
        "pages", JArr results
    ]

let isPageFile (f: string) = Regex.IsMatch(f, @"^(S|W)-[0-9]{3}.*\.html$")

// ---------- effectful steps ----------

let runAxeScript = """(tags) => window.axe.run(document, { runOnly: { type: "tag", values: tags }, resultTypes: ["violations"] }).then((r) => r.violations.map((v) => ({ id: v.id, impact: v.impact, help: v.help, nodes: v.nodes.map((n) => ({ target: n.target })) })))"""

type Settings = { Designs: string; Axe: string; Only: string list }

let private runAxe (page: Cdp.Page) (axeSource: string) =
    task {
        do! Cdp.addScriptTag page axeSource
        let! v = Cdp.call page runAxeScript [ strs axeTags ]
        return summarizeViolations (asList v)
    }

let private checkDialogs (page: Cdp.Page) (renderedDir: string) (file: string) (width: int) (axeSource: string) =
    task {
        let! ids = Cdp.evaluate page """[...document.querySelectorAll("dialog[id]")].map((d) => d.id)"""
        let results = ResizeArray()
        for id in asList ids |> List.map asStr do
            do! Cdp.call page "(dialogId) => { document.getElementById(dialogId).showModal(); }" [ str id ] :> Threading.Tasks.Task
            do! Cdp.waitForTimeout 400
            let shot = $"{pageName file}-{id}-{width}.png"
            let! png = Cdp.screenshot page false
            Cdp.save (Path.Combine(renderedDir, shot)) png
            let! violations = runAxe page axeSource
            do! Cdp.call page "(dialogId) => { document.getElementById(dialogId).close(); }" [ str id ] :> Threading.Tasks.Task
            results.Add(JObj [ "dialog", str id; "screenshot", str ("rendered/" + shot); "axe_violations", JArr violations ])
        return List.ofSeq results
    }

let private checkWidth (browser: Cdp.Browser) (s: Settings) (file: string) (width: int) (axeSource: string) =
    task {
        let screensDir = Path.Combine(s.Designs, "screens")
        let renderedDir = Path.Combine(s.Designs, "rendered")
        let! page = Cdp.newPage browser { Width = width; Height = 800; DeviceScaleFactor = 1.0; ReducedMotion = Some "reduce" }
        do! Cdp.goto page (Uri(Path.GetFullPath(Path.Combine(screensDir, file)))).AbsoluteUri
        let shot = $"{pageName file}-{width}.png"
        let! png = Cdp.screenshot page true
        Cdp.save (Path.Combine(renderedDir, shot)) png
        let! sizes = Cdp.evaluate page "({ scrollWidth: document.documentElement.scrollWidth, clientWidth: document.documentElement.clientWidth })"
        let! axeViolations = runAxe page axeSource
        let! dialogs = checkDialogs page renderedDir file width axeSource
        do! Cdp.closePage page
        let pageErrors = Cdp.events page |> List.choose (function Cdp.PageError t -> Some t | _ -> None)
        return
            JObj [
                "h_scroll", JBool(asNum (sizes ? scrollWidth) > float width)
                "scroll_width", sizes ? scrollWidth
                "client_width", sizes ? clientWidth
                "screenshot", str ("rendered/" + shot)
                "axe_violations", JArr axeViolations
                "dialogs", JArr dialogs
                "page_errors", strs pageErrors
            ]
    }

let run (s: Settings) : int =
    try
        let screensDir = Path.Combine(s.Designs, "screens")
        let axeSource = File.ReadAllText s.Axe
        let csv = File.ReadAllText(Path.Combine(s.Designs, "../../reconciliation/2026-11-23/data/forma-catalog.csv"))
        let manifest = asList (parse (File.ReadAllText(Path.Combine(screensDir, "page-manifest.json"))))
        let catalogIds = parseCatalogIds csv
        let pages =
            Directory.GetFiles screensDir
            |> Array.map Path.GetFileName
            |> Array.filter (fun f -> isPageFile f && (s.Only.IsEmpty || List.contains f s.Only))
            |> Array.sortWith (fun a b -> String.CompareOrdinal(a, b))
            |> List.ofArray
        Directory.CreateDirectory(Path.Combine(s.Designs, "rendered")) |> ignore
        let browser = (Cdp.launch ()).Result
        let results =
            pages
            |> List.map (fun f ->
                let html = File.ReadAllText(Path.Combine(screensDir, f))
                let byWidth = widths |> List.map (fun w -> w, (checkWidth browser s f w axeSource).Result)
                let entry = manifest |> List.tryFind (fun m -> m ? page = JStr f)
                let r = pageRecord f entry (componentsUsed html catalogIds) (staticRuleViolations html) byWidth
                Console.WriteLine $"verified {f}"
                r)
        (Cdp.close browser).Wait()
        let rep = report results
        let out = Path.Combine(screensDir, (if s.Only.IsEmpty then "verification.json" else "verification.partial.json"))
        File.WriteAllText(out, stringify 2 rep + "\n")
        Console.WriteLine(stringify 0 (rep ? totals))
        0
    with e ->
        eprintfn "%O" e
        1

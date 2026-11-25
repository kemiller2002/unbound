/// Builds the v2 prototypes: one HTML page per screen state, plus an index (port of v2/tools/build.mjs).
/// Everything here is pure; Program.fs reads the registry and pages.json and writes the files.
module Unbound.Designs.BuildV2

open Unbound.Designs.JsCompat
open Unbound.Designs.Screen

/// Every screen module, by id (the JavaScript imported tools/screens/<id>.mjs).
let screens : Screen list = [ S001.screen; S003.screen; S004.screen; S008.screen; S011.screen; S012.screen; W001.screen ]

let screenById (id: string) : Screen option = screens |> List.tryFind (fun s -> s.Id = id)

/// `(process.env.SCREENS ?? "S-001").split(",")`.
let selected (env: string option) : string list = (defaultArg env "S-001").Split(',') |> List.ofArray

/// A page to write: its path under designs/v2, its HTML and its pages.json entry.
type Built = { File: string; Html: string; Meta: JsValue }

let pagesOf (registry: Ui.IconRegistry) (screen: Screen) : Built list =
    screen.States
    |> List.map (fun s ->
        let file = $"screens/{screen.Slug}/{s.Id}.html"
        let html =
            match s.Content with
            | Html h -> h
            | App p -> Ui.appPage { p with DocTitle = Some $"{screen.Title}: {s.Title}" }
        { File = file
          Html = Ui.resolveIcons registry html
          Meta =
            JObj
                [ "file", JStr file
                  "screen", JStr screen.Id
                  "requirement", JStr screen.Requirement
                  "state", JStr s.Id
                  "title", JStr s.Title
                  "persona", JStr s.Persona
                  "scenarios", JArr(s.Scenarios |> List.map JStr)
                  "kind", JStr(defaultArg s.Kind "app") ] })

let private fileOf (m: JsValue) : string =
    match prop "file" m with
    | Some(JStr f) -> f
    | _ -> failwith "TypeError: m.file.localeCompare is not a function"

/// Prior entries of screens not rebuilt this run, then this run's pages, sorted by file (localeCompare, stable).
/// `prior` is the parsed pages.json (None when it is missing or not JSON).
let merge (ids: string list) (prior: JsValue option) (pages: Built list) : JsValue list =
    let kept =
        match prior with
        | None -> []
        | Some(JArr ms) ->
            ms
            |> List.filter (fun m ->
                match m with
                | JNull -> failwith "TypeError: Cannot read properties of null (reading 'screen')"
                | _ ->
                    match prop "screen" m with
                    | Some(JStr s) -> not (List.contains s ids)
                    | _ -> true)
        | Some _ -> failwith "TypeError: prior.filter is not a function"
    kept @ (pages |> List.map (fun p -> p.Meta)) |> List.sortWith (fun a b -> localeCompare (fileOf a) (fileOf b))

let index (metas: JsValue list) : string =
    let li (m: JsValue) =
        let scenarios =
            match prop "scenarios" m with
            | Some(JArr xs) -> join ", " xs
            | _ -> failwith "TypeError: p.meta.scenarios.join is not a function"
        $"""<li><a href="{fileOf m}">{interpolate (prop "screen" m)} {interpolate (prop "title" m)}</a> ({scenarios})</li>"""
    $"""<!doctype html><html lang="en"><head><meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1"><title>Unbound v2 design prototypes</title><link rel="stylesheet" href="../vendor/forma-1.0.2/all.css"></head>
<body class="forma-site"><main class="forma-site__main" id="main"><div class="forma-container forma-prose"><h1>Unbound v2 design prototypes</h1><p>Static design prototypes for report 18. Synthetic data. One page per state.</p>
<ul>{metas |> List.map li |> String.concat ""}</ul></div></main></body></html>"""

let pagesJson (metas: JsValue list) : string = stringify (JArr metas) + "\n"

/// Generates the static screen prototypes in screens/ from the per-screen modules (port of tools/build-screens.mjs).
/// Pure rendering (`renderPage`, `manifestEntry`, `mergeManifest`); `run` is the only part that touches the file system.
/// Usage (from requirements/ui/designs): Unbound.Designs.V1 build-screens [S-001 ...]
module Unbound.Designs.V1.BuildScreens

open System
open System.IO
open Unbound.Designs.V1.JsCompat
open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.Model

let screens: Screen list =
    [ Screens.S001.screen
      Screens.S002.screen
      Screens.S003.screen
      Screens.S004.screen
      Screens.S005.screen
      Screens.S006.screen
      Screens.S007.screen
      Screens.S008.screen
      Screens.S009.screen
      Screens.S010.screen
      Screens.S011.screen
      Screens.S012.screen
      Screens.S013.screen
      Screens.W001.screen ]

/// "S-001", 0 -> "s001-1".
let prefixFor (screenId: string) (i: int) =
    let id = screenId.ToLowerInvariant() |> replaceFirst "-" ""
    $"{id}-{i + 1}"

/// Expand state definitions into rendered, uniquely prefixed state sections.
let renderStates (screen: Screen) : RenderedState list =
    let total = List.length screen.States
    screen.States
    |> List.mapi (fun i def ->
        let id = prefixFor screen.Id i
        { Id = id
          Title = def.Title
          Html = Ui.stateSection(id = id, n = i + 1, total = total, persona = def.Persona, title = def.Title, demonstrates = def.Demonstrates, body = def.Body id) })

let renderPage (screen: Screen) =
    let states = renderStates screen
    match screen.Render with
    | Some render -> render states
    | None -> Shell.page screen states

let private satisfiedJson =
    function
    | Yes -> JsBool true
    | No -> JsBool false
    | NeedsApplicationBehaviour -> JsString "needs-application-behaviour"

let manifestEntry (screen: Screen) =
    JsObject
        [ "page", JsString screen.File
          "requirement_ids", JsArray(screen.ReqIds |> List.map JsString)
          "states",
          JsArray(
              screen.States
              |> List.mapi (fun i s ->
                  JsObject
                      [ "id", JsString(prefixFor screen.Id i)
                        "title", JsString s.Title
                        "persona", JsString s.Persona
                        "demonstrates", JsString s.Demonstrates ])
          )
          "requirement_coverage",
          JsArray(
              screen.Coverage
              |> List.map (fun c ->
                  JsObject
                      [ "requirement_id", JsString c.RequirementId
                        "acceptance_id", JsString c.AcceptanceId
                        "satisfied_by_markup", satisfiedJson c.Satisfied
                        "note", JsString c.Note ])
          )
          "gaps", JsArray(screen.Gaps |> List.map (fun id -> JsObject [ "gap_id_or_NEW", JsString id; "description", JsString(Gaps.describe id) ])) ]

let private pageOf (v: JsValue) =
    match prop "page" v with
    | Some(JsString s) -> Some s
    | _ -> None

/// [...previous.filter(p => !fresh.some(f => f.page === p.page)), ...fresh].sort((a, b) => a.page.localeCompare(b.page)).
/// Page names are ASCII file names, for which ordinal order equals localeCompare order.
let mergeManifest (previous: JsValue list) (fresh: JsValue list) =
    let freshPages = fresh |> List.map pageOf |> Set.ofList
    previous
    |> List.filter (fun p -> not (freshPages.Contains(pageOf p)))
    |> fun kept -> kept @ fresh
    |> List.sortWith (fun a b -> String.CompareOrdinal(defaultArg (pageOf a) "undefined", defaultArg (pageOf b) "undefined"))

/// The screens to build: all of them, or those whose ids were requested (in the fixed order).
let selected (requested: string list) =
    if List.isEmpty requested then screens else screens |> List.filter (fun s -> List.contains s.Id requested)

/// The manifest text written for the chosen screens on top of the previous manifest entries.
let manifestText (previous: JsValue list) (chosen: Screen list) =
    stringify (Some 2) (JsArray(mergeManifest previous (chosen |> List.map manifestEntry))) + "\n"

/// Writes screens/*.html and screens/page-manifest.json under baseDir (the working directory when run from the
/// command line) and prints the summary line; returns the exit code.
let run (baseDir: string) (args: string list) =
    let outDir = Path.GetFullPath(Path.Combine(baseDir, "screens"))
    let chosen = selected args
    Directory.CreateDirectory outDir |> ignore
    for s in chosen do
        File.WriteAllText(Path.Combine(outDir, s.File), renderPage s)
    let manifestPath = Path.Combine(outDir, "page-manifest.json")
    // A partial build merges into the existing manifest; a missing or unreadable file counts as "[]".
    let previous =
        if List.isEmpty args then []
        else
            let text = try File.ReadAllText manifestPath with _ -> "[]"
            match parse text with
            | JsArray xs -> xs
            | _ -> failwith "TypeError: previous.filter is not a function"
    File.WriteAllText(manifestPath, manifestText previous chosen)
    Console.Out.Write($"wrote {List.length chosen} page(s) to {outDir}\n")
    0

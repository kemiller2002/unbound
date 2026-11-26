/// Competitive-displacement decisions package tool. Effects (file reads/writes, console) happen only here.
///   dotnet run --project tools/fs/Unbound.CompetitiveDecisions -c Release -- validate <root> [--check] [--research-root <path>]
///     validates gates A1-A10 and rewrites the generated Markdown sections; with --check it fails instead of rewriting
///     when a generated section is stale (A11). --research-root points at a daf-landscape checkout for gate A8.
module Unbound.CompetitiveDecisions.Program

open System.IO
open Unbound.CompetitiveDecisions

/// (document path, current text, regenerated text) for every generated document, or the missing-marker issues.
let regenerate (root: string) (inputs: Data.Inputs) : Result<(string * string * string) list, string list> =
    let docs =
        Render.SECTIONS
        |> List.map (fun (doc, render) ->
            let path = Path.Combine(root, Data.MISSION, doc)
            let current = if File.Exists path then File.ReadAllText path else ""
            match Render.splice current (render inputs) with
            | Some next -> Ok(path, current, next)
            | None -> Error $"A11 {doc}: generated section markers are missing")
    match docs |> List.choose (function Error e -> Some e | Ok _ -> None) with
    | [] -> Ok(docs |> List.choose (function Ok d -> Some d | Error _ -> None))
    | errors -> Error errors

let validate (root: string) (check: bool) (researchRoot: string option) : int =
    let inputs = Data.load root researchRoot
    let results = Gates.run inputs
    results
    |> List.iter (fun (code, issues) ->
        let skipped = code = "A8" && researchRoot.IsNone
        printfn "%s %s (%d issue(s))" code (if skipped then "skipped (no --research-root)" elif issues.IsEmpty then "pass" else "FAIL") issues.Length)
    let issues = results |> List.collect snd
    issues |> List.iter (eprintfn "%s")
    printfn "requirements %d, implications %d, owner decisions %d, approved decisions %d" inputs.Requirements.Length inputs.Implications.Length inputs.Decisions.Length inputs.Approved.Length
    match regenerate root inputs with
    | Error e ->
        e |> List.iter (eprintfn "%s")
        1
    | Ok docs ->
        let stale = docs |> List.filter (fun (_, cur, next) -> cur <> next)
        if check then
            stale |> List.iter (fun (p, _, _) -> eprintfn "A11 stale generated section: %s" p)
            printfn "A11 %s (%d stale document(s))" (if stale.IsEmpty then "pass" else "FAIL") stale.Length
            if issues.IsEmpty && stale.IsEmpty then 0 else 1
        else
            stale |> List.iter (fun (p, _, next) -> File.WriteAllText(p, next))
            printfn "A11 rewrote %d document(s)" stale.Length
            if issues.IsEmpty then 0 else 1

let rec private option (name: string) (args: string list) : string option =
    match args with
    | n :: v :: _ when n = name -> Some v
    | _ :: rest -> option name rest
    | [] -> None

[<EntryPoint>]
let main argv =
    match List.ofArray argv with
    | "validate" :: root :: rest -> validate root (List.contains "--check" rest) (option "--research-root" rest)
    | _ ->
        eprintfn "usage: Unbound.CompetitiveDecisions validate <repository root> [--check] [--research-root <daf-landscape checkout>]"
        2

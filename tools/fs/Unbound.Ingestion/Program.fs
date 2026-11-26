/// File-ingestion mission tool. Effects (file reads/writes, console) happen only here.
///   dotnet run --project tools/fs/Unbound.Ingestion -c Release -- validate <root> [--check]
///     validates gates I1-I8 and rewrites the generated Markdown sections; with --check it fails instead of rewriting
///     when a generated section is stale.
module Unbound.Ingestion.Program

open System.IO
open Unbound.Ingestion

let private documentPath (root: string) (doc: string) = Path.Combine(root, Data.MISSION, doc)

/// (document path, current text, regenerated text) for every document with generated sections.
let regenerate (root: string) (inputs: Data.Inputs) : Result<(string * string * string) list, string list> =
    Render.SECTIONS
    |> List.groupBy (fun (doc, _, _) -> doc)
    |> List.map (fun (doc, sections) ->
        let path = documentPath root doc
        let current = if File.Exists path then File.ReadAllText path else ""
        let folded =
            sections
            |> List.fold
                (fun acc (_, name, render) ->
                    match acc with
                    | Error e -> Error e
                    | Ok text ->
                        match Render.splice text name (render inputs) with
                        | Some t -> Ok t
                        | None -> Error [ $"V9 {doc}: generated section markers for '{name}' are missing" ])
                (Ok current)
        folded |> Result.map (fun next -> path, current, next))
    |> List.fold
        (fun acc r ->
            match acc, r with
            | Ok xs, Ok x -> Ok(xs @ [ x ])
            | Error a, Error b -> Error(a @ b)
            | Error a, _ -> Error a
            | _, Error b -> Error b)
        (Ok [])

let validate (root: string) (check: bool) : int =
    let inputs = Data.load root
    let results = Gates.run inputs
    results |> List.iter (fun (code, issues) -> printfn "%s %s (%d issue(s))" code (if issues.IsEmpty then "pass" else "FAIL") issues.Length)
    let issues = results |> List.collect snd
    issues |> List.iter (eprintfn "%s")
    match regenerate root inputs with
    | Error e ->
        e |> List.iter (eprintfn "%s")
        1
    | Ok docs ->
        let stale = docs |> List.filter (fun (_, cur, next) -> cur <> next)
        if check then
            stale |> List.iter (fun (p, _, _) -> eprintfn "V9 stale generated sections: %s" p)
            printfn "V9 %s (%d stale document(s))" (if stale.IsEmpty then "pass" else "FAIL") stale.Length
            if issues.IsEmpty && stale.IsEmpty then 0 else 1
        else
            stale |> List.iter (fun (p, _, next) -> File.WriteAllText(p, next))
            printfn "V9 rewrote %d document(s)" stale.Length
            if issues.IsEmpty then 0 else 1

[<EntryPoint>]
let main argv =
    match List.ofArray argv with
    | "validate" :: root :: rest -> validate root (List.contains "--check" rest)
    | _ ->
        eprintfn "usage: Unbound.Ingestion validate <repository root> [--check]"
        2

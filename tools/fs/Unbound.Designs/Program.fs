/// Entry point and the program edge: environment, file reads and writes, console output.
/// `build-v2 [dir]` runs v2/tools/build.mjs; dir defaults to the working directory (requirements/ui/designs/v2).
module Unbound.Designs.Program

open System
open System.IO
open System.Text
open Unbound.Designs.JsCompat

let private utf8 = UTF8Encoding(false)

/// Node's readFile(p, "utf8"): bytes decoded as UTF-8, invalid sequences replaced, a BOM kept.
let private readText (path: string) : string = utf8.GetString(File.ReadAllBytes path)

let private writeText (path: string) (text: string) : unit = File.WriteAllBytes(path, utf8.GetBytes text)

let private buildV2 (root: string) : int =
    let ids = BuildV2.selected (Option.ofObj (Environment.GetEnvironmentVariable "SCREENS"))
    let screens =
        ids
        |> List.map (fun id ->
            match BuildV2.screenById id with
            | Some s -> s
            | None -> failwith ("Cannot find module '" + Path.Combine(root, "tools", "screens", id + ".mjs") + "'"))
    let registryPath = Path.GetFullPath(Path.Combine(root, "..", "vendor", "forma-1.0.2", "icons-registry.json"))
    let registry =
        match parseJson (readText registryPath) with
        | Some json -> Ui.iconRegistry json
        | None -> failwith $"SyntaxError: {registryPath} is not JSON"
    let pages = screens |> List.collect (BuildV2.pagesOf registry)
    for p in pages do
        let path = Path.Combine(root, p.File)
        Directory.CreateDirectory(Path.GetDirectoryName path) |> ignore
        writeText path p.Html
    // Merge with pages of screens not rebuilt this run, so partial builds do not drop other screens.
    let pagesJsonPath = Path.Combine(root, "pages.json")
    let prior = try parseJson (readText pagesJsonPath) with _ -> None
    let metas = BuildV2.merge ids prior pages
    writeText (Path.Combine(root, "index.html")) (BuildV2.index metas)
    writeText pagesJsonPath (BuildV2.pagesJson metas)
    let out = Console.OpenStandardOutput()
    let line = utf8.GetBytes $"built {pages.Length} pages\n"
    out.Write(line, 0, line.Length)
    out.Flush()
    0

[<EntryPoint>]
let main argv =
    let run root =
        try
            buildV2 root
        with e ->
            // Node prints the uncaught error with a stack trace and exits 1; only the message is reproduced.
            eprintfn "%s" e.Message
            1
    match List.ofArray argv with
    | [ "build-v2" ] -> run (Directory.GetCurrentDirectory())
    | [ "build-v2"; dir ] -> run (Path.GetFullPath dir)
    | _ ->
        eprintfn "usage: Unbound.Designs build-v2 [designs/v2 directory]"
        2

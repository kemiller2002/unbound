/// Browser verifiers for the UI design prototypes (F# port of the Node + Playwright verifiers).
///
///   verify-v2        [--axe <axe.min.js>]                      run from requirements/ui/designs/v2
///                    env OUT_EXTRA (non-reference screenshots; default <temp>/unbound-v2-extra), ONLY=S-001,S-003
///   verify-screens   [--axe <axe.min.js>] [page.html ...]      run from requirements/ui/designs
///   verify-documents [--axe <axe.min.js>] [--work <dir>]       run from requirements/ui/designs
///   audit-v1         [--axe <axe.min.js>] [--screens <dir>] [--out <dir>]   run from requirements/ui/designs
///
/// axe defaults to the vendored, unmodified vendor/axe-core-4.10.2/axe.min.js. Chromium is the headless shell
/// Playwright uses (override with UNBOUND_CHROMIUM).
module Unbound.Designs.Verify.Program

open System
open System.IO

let private flag (name: string) (args: string list) =
    args |> List.tryFindIndex ((=) name) |> Option.bind (fun i -> List.tryItem (i + 1) args)

let private env (name: string) = Environment.GetEnvironmentVariable name |> Option.ofObj

let private temp (name: string) = Path.Combine(Path.GetTempPath(), name)

let private usage () =
    eprintfn "usage: Unbound.Designs.Verify (verify-v2 | verify-screens | verify-documents | audit-v1) [options]"
    2

[<EntryPoint>]
let main argv =
    let args = List.ofArray argv
    let vendoredAxe (designs: string) = Path.Combine(designs, "vendor", "axe-core-4.10.2", "axe.min.js")
    match args with
    | "verify-v2" :: rest ->
        V2.run
            { Root = "."
              Axe = flag "--axe" rest |> Option.defaultValue (vendoredAxe "..")
              OutExtra = env "OUT_EXTRA" |> Option.defaultValue (temp "unbound-v2-extra")
              Only = env "ONLY" |> Option.filter (fun s -> s <> "") |> Option.map (fun s -> s.Split ',' |> List.ofArray) }
    | "verify-screens" :: rest ->
        let axeIdx = rest |> List.tryFindIndex ((=) "--axe")
        let only = rest |> List.indexed |> List.filter (fun (i, a) -> a.EndsWith(".html", StringComparison.Ordinal) && axeIdx <> Some(i - 1)) |> List.map snd
        Screens.run { Designs = "."; Axe = flag "--axe" rest |> Option.defaultValue (vendoredAxe "."); Only = only }
    | "verify-documents" :: rest ->
        Documents.run
            { Root = "."
              Axe = flag "--axe" rest |> Option.defaultValue (vendoredAxe ".")
              Work = flag "--work" rest |> Option.defaultValue (temp "unbound-verify-work") }
    | "audit-v1" :: rest ->
        AuditV1.run
            { Screens = flag "--screens" rest |> Option.defaultValue "screens"
              Out = flag "--out" rest |> Option.defaultValue (temp "unbound-audit-v1")
              Axe = flag "--axe" rest |> Option.defaultValue (vendoredAxe ".") }
    | _ -> usage ()

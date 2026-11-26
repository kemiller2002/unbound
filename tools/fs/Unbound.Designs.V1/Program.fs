/// Entry point: the v1 design generators, run from requirements/ui/designs.
///   build-screens [S-001 ...]   screens/*.html and screens/page-manifest.json (was tools/build-screens.mjs)
///   build-documents             documents/*.html (was tools/build-documents.mjs)
module Unbound.Designs.V1.Program

open System

let private usage = "usage: Unbound.Designs.V1 build-screens [S-001 ...] | build-documents\n"

[<EntryPoint>]
let main argv =
    try
        match List.ofArray argv with
        | "build-screens" :: ids -> BuildScreens.run (Environment.CurrentDirectory) ids
        | [ "build-documents" ] -> BuildDocuments.run Environment.CurrentDirectory
        | _ ->
            Console.Error.Write usage
            2
    with e ->
        // Node prints the uncaught error and exits 1; the message text is .NET's, not V8's.
        Console.Error.Write(e.Message + "\n")
        1

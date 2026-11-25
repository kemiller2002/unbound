/// Full builds reproduce the committed prototypes byte for byte.
module Unbound.Designs.V1.Tests.BuildTests

open System
open System.IO
open Xunit
open Unbound.Designs.V1

/// The repository root: the first ancestor of the test assembly that contains tools/fs.
let private repoRoot =
    let rec up (d: DirectoryInfo) =
        if isNull d then failwith "repository root not found"
        elif Directory.Exists(Path.Combine(d.FullName, "tools", "fs", "Unbound.Designs.V1")) then d.FullName
        else up d.Parent
    up (DirectoryInfo AppContext.BaseDirectory)

let private designs = Path.Combine(repoRoot, "requirements", "ui", "designs")
let private committed (rel: string) = File.ReadAllText(Path.Combine(designs, rel))

let private scratch () =
    let d = Path.Combine(Path.GetTempPath(), "unbound-designs-v1-" + Guid.NewGuid().ToString("N"))
    Directory.CreateDirectory d |> ignore
    d

let screenFiles () : obj[] seq = BuildScreens.screens |> Seq.map (fun s -> [| box s.Id |])

[<Theory>]
[<MemberData(nameof screenFiles)>]
let ``each screen renders the committed page`` (id: string) =
    let s = BuildScreens.screens |> List.find (fun s -> s.Id = id)
    Assert.True((committed ("screens/" + s.File) = BuildScreens.renderPage s), $"{s.File} differs")

[<Fact>]
let ``the full manifest equals the committed page-manifest.json`` () =
    Assert.True((committed "screens/page-manifest.json" = BuildScreens.manifestText [] BuildScreens.screens))

let documentFiles () : obj[] seq = BuildDocuments.documents |> Seq.map (fun (n, _) -> [| box n |])

[<Theory>]
[<MemberData(nameof documentFiles)>]
let ``each document renders the committed file`` (name: string) =
    let build = BuildDocuments.documents |> List.find (fun (n, _) -> n = name) |> snd
    Assert.True((committed ("documents/" + name) = build ()), $"{name} differs")

let private sameFiles (dir: string) (sub: string) (names: string list) =
    for n in names do
        let a = File.ReadAllBytes(Path.Combine(designs, sub, n))
        let b = File.ReadAllBytes(Path.Combine(dir, sub, n))
        Assert.True((a = b), $"{sub}/{n} differs")

[<Fact>]
let ``build-screens into an empty folder writes the committed files`` () =
    let dir = scratch ()
    try
        Assert.Equal(0, BuildScreens.run dir [])
        sameFiles dir "screens" ("page-manifest.json" :: (BuildScreens.screens |> List.map (fun s -> s.File)))
    finally
        Directory.Delete(dir, true)

[<Fact>]
let ``a partial build-screens merges into the existing manifest`` () =
    let dir = scratch ()
    try
        Assert.Equal(0, BuildScreens.run dir [])
        Assert.Equal(0, BuildScreens.run dir [ "W-001"; "S-003"; "not-a-screen" ])
        sameFiles dir "screens" [ "page-manifest.json"; "W-001-grant-status-widget.html"; "S-003-grant-recommendation.html" ]
    finally
        Directory.Delete(dir, true)

[<Fact>]
let ``build-documents into an empty folder writes the committed files`` () =
    let dir = scratch ()
    try
        Assert.Equal(0, BuildDocuments.run dir)
        sameFiles dir "documents" (BuildDocuments.documents |> List.map fst)
    finally
        Directory.Delete(dir, true)

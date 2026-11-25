/// The build reproduces the committed v2 prototypes (requirements/ui/designs/v2) exactly, in memory.
module Unbound.Designs.Tests.BuildV2Tests

open System.IO
open Xunit
open Unbound.Designs
open Unbound.Designs.JsCompat

let private v2 = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, "../../../requirements/ui/designs/v2"))

let private registry () =
    File.ReadAllText(Path.Combine(v2, "../vendor/forma-1.0.2/icons-registry.json")) |> parseJson |> Option.get |> Ui.iconRegistry

let private committed (file: string) = File.ReadAllText(Path.Combine(v2, file))

let private all = [ "S-001"; "S-003"; "S-004"; "S-008"; "S-011"; "S-012"; "W-001" ]

let private build (ids: string list) (prior: JsValue option) =
    let pages = ids |> List.collect (fun id -> BuildV2.pagesOf (registry ()) (BuildV2.screenById id |> Option.get))
    let metas = BuildV2.merge ids prior pages
    pages, BuildV2.index metas, BuildV2.pagesJson metas

[<Fact>]
let ``the documented full build reproduces every committed file`` () =
    let pages, index, pagesJson = build all (parseJson (committed "pages.json"))
    Assert.Equal(42, pages.Length)
    for p in pages do
        Assert.True((committed p.File = p.Html), $"{p.File} differs")
    Assert.Equal(committed "index.html", index)
    Assert.Equal(committed "pages.json", pagesJson)
    // Every committed page is one the build writes.
    let written = pages |> List.map (fun p -> p.File) |> Set.ofList
    for f in Directory.GetFiles(Path.Combine(v2, "screens"), "*.html", SearchOption.AllDirectories) do
        Assert.Contains(Path.GetRelativePath(v2, f).Replace('\\', '/'), written)

[<Fact>]
let ``a full build without a prior pages.json gives the same index`` () =
    let _, index, pagesJson = build all None
    Assert.Equal(committed "index.html", index)
    Assert.Equal(committed "pages.json", pagesJson)

[<Fact>]
let ``a partial build keeps the other screens' entries`` () =
    let others =
        match parseJson (committed "pages.json") with
        | Some(JArr ms) -> JArr(ms |> List.filter (fun m -> prop "screen" m <> Some(JStr "S-003")))
        | _ -> failwith "pages.json is not an array"
    let pages, index, pagesJson = build [ "S-003" ] (Some others)
    Assert.Equal(9, pages.Length)
    Assert.Equal(committed "index.html", index)
    Assert.Equal(committed "pages.json", pagesJson)

[<Fact>]
let ``SCREENS defaults to S-001 and splits on commas`` () =
    Assert.Equal<string list>([ "S-001" ], BuildV2.selected None)
    Assert.Equal<string list>([ "S-003"; "W-001" ], BuildV2.selected (Some "S-003,W-001"))
    Assert.Equal<string list>([ "" ], BuildV2.selected (Some ""))

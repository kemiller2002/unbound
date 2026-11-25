module Unbound.Authoring.Tests.AuthoringTests

open System
open System.IO
open Xunit
open Unbound.Py.PyJson
open Unbound.Authoring
open Unbound.Authoring.PyCompat

/// The repository root: the first ancestor of the test assembly that contains tools/fs.
let private repoRoot =
    let rec up (d: DirectoryInfo) =
        if isNull d then failwith "repository root not found"
        elif Directory.Exists(Path.Combine(d.FullName, "tools", "fs", "Unbound.Authoring")) then d.FullName
        else up d.Parent
    up (DirectoryInfo AppContext.BaseDirectory)

let private copyDir (src: string) (dst: string) =
    Directory.CreateDirectory dst |> ignore
    for f in Directory.GetFiles src do
        File.Copy(f, Path.Combine(dst, Path.GetFileName f))

/// A scratch copy of the inputs and outputs of the three commands.
let private scratchRepo () =
    let root = Path.Combine(Path.GetTempPath(), "unbound-authoring-" + Guid.NewGuid().ToString("N"))
    let rel = [ "requirements/reconciliation/2026-11-23/data"; "tools/fs/Unbound.Authoring/specs" ]
    for r in rel do
        copyDir (Path.Combine(repoRoot, r)) (Path.Combine(root, r))
    let v2 = "requirements/ui/designs/v2"
    Directory.CreateDirectory(Path.Combine(root, v2)) |> ignore
    for f in [ "pages.json"; "verification.json" ] do
        File.Copy(Path.Combine(repoRoot, v2, f), Path.Combine(root, v2, f))
    root

let private snapshot (root: string) =
    let data = Path.Combine(root, "requirements/reconciliation/2026-11-23/data")
    Directory.GetFiles data |> Array.sort |> Array.map (fun f -> Path.GetFileName f, File.ReadAllBytes f)

[<Fact>]
let ``the three commands in order leave the committed data unchanged`` () =
    let root = scratchRepo ()
    try
        let before = snapshot root
        let paths = Io.paths root
        Assert.Equal(0, UiQuality.main paths)
        Assert.Equal(0, MergeSpecs.main paths)
        Assert.Equal(0, UiTrace.main paths)
        let after = snapshot root
        Assert.Equal(before.Length, after.Length)
        for (n1, b1), (n2, b2) in Array.zip before after do
            Assert.Equal(n1, n2)
            Assert.True((b1 = b2), $"{n1} changed")
    finally
        Directory.Delete(root, true)

[<Fact>]
let ``removed UI quality records are regenerated identically`` () =
    let root = scratchRepo ()
    try
        let paths = Io.paths root
        let path = Path.Combine(paths.Data, "ui-requirements.jsonl")
        let original = File.ReadAllText path
        let kept =
            original.Split('\n')
            |> Array.filter (fun l -> not (l.Contains "\"id\": \"UNB-UI-DS-" || l.Contains "\"id\": \"UNB-UI-N-"))
        File.WriteAllText(path, String.Join("\n", kept))
        Assert.Equal(0, UiQuality.main paths)
        Assert.Equal(0, MergeSpecs.main paths)
        Assert.Equal(original, File.ReadAllText path)
    finally
        Directory.Delete(root, true)

[<Fact>]
let ``req builds the record with the script's key order and numbered criteria`` () =
    let r = UiQuality.build (UiQualityData.ds |> List.head)
    match r with
    | JObj kvs ->
        Assert.Equal<string list>(
            [ "id"; "kind"; "title"; "statement"; "traces"; "forma_components"; "folio_elements"; "capability_gaps"; "actors"
              "states"; "actions"; "responsive"; "acceptance_criteria"; "detail"; "status"; "origin" ],
            kvs |> List.map fst
        )
    | _ -> failwith "not an object"
    Assert.Equal(JStr "UNB-UI-DS-001", item "id" r)
    Assert.Equal(JStr "proposed", item "status" r)
    Assert.Equal(JObj [ "evidence", JStr "requirements/ui/designs/v2/unbound-app.css; requirements/ui/designs/vendor/unbound-brands/README.md" ], item "detail" r)
    let acIds = itemList "acceptance_criteria" r |> List.map (itemStr "id")
    Assert.Equal<string list>([ "AC-1"; "AC-2"; "AC-3" ], acIds)

[<Fact>]
let ``NEW has 41 records with unique ids in family order`` () =
    let ids = UiQuality.newRecords |> List.map (itemStr "id")
    Assert.Equal(41, ids.Length)
    Assert.Equal(ids.Length, (List.distinct ids).Length)
    Assert.Equal("UNB-UI-DS-001", List.head ids)
    Assert.Equal("UNB-UI-V-002", List.last ids)

[<Fact>]
let ``a revision replaces the escaped text once and records itself`` () =
    let r = JObj [ "id", JStr "X-1"; "statement", JStr "say \"old\" twice: \"old\"" ]
    let once = UiQuality.reviseOne r ("\"old\"", "new −", "because") "t1"
    Assert.Equal(JStr "say new − twice: new −", item "statement" once)
    Assert.Equal(1, (itemList "revisions" once).Length)
    Assert.Equal(once, UiQuality.reviseOne once ("\"old\"", "new −", "because") "t1")
    Assert.Equal("report-18-" + (sha1Hex "abc").Substring(0, 8), UiQuality.moreTag "abc")
    Assert.Equal("a9993e364706816aba3e25717850c26c9cd0d89d", sha1Hex "abc")

[<Fact>]
let ``remap renumbers S-099 and maps PROPOSED gap names`` () =
    let r = JObj [ "id", JStr "UNB-UI-S-099"; "capability_gaps", JArr [ JStr "PROPOSED:Approval quorum tracker"; JStr "PROPOSED:Two-person approval capture" ]; "note", JStr "see UNB-UI-S-099." ]
    let merged = MergeSpecs.specRecords [ r ] |> List.head
    Assert.Equal(JStr "UNB-UI-S-061", item "id" merged)
    Assert.Equal(JArr [ JStr "FORMA-GAP-24" ], item "capability_gaps" merged)
    Assert.Equal(JStr "see UNB-UI-S-061.", item "note" merged)

[<Fact>]
let ``originals are found through UI traces, refinements and gap parents`` () =
    let ui = Map [ "UI-1", JObj [ "id", JStr "UI-1"; "traces", JArr [ JStr "R-1"; JStr "G-1"; JStr "B-1" ] ] ]
    let inp : UiTrace.Inputs =
        { Ui = ui
          Base = Set [ "B-1"; "B-2"; "B-3" ]
          Refinements = Map [ "R-1", [ "B-2" ] ]
          GapParents = Map [ "G-1", [ "B-3"; "B-1" ] ] }
    Assert.Equal<string list>([ "B-1"; "B-2"; "B-3" ], UiTrace.originalsOf inp [ "UI-1" ])
    Assert.Equal<string list>([ "B-3"; "B-1" ], UiTrace.parents (JObj [ "parent", JStr " B-3, B-1;; " ]))

[<Fact>]
let ``DKS-UI rows have the scenario columns in order`` () =
    let rows = UiTraceData.dksRows
    Assert.Equal(20, rows.Length)
    Assert.All(rows, fun r -> Assert.Equal<string list>(UiTraceData.dksFields, r |> List.map fst))
    Assert.Equal("requirements/ui/designs/v2/rendered/S-001/normal-390.png; requirements/ui/designs/v2/rendered/S-001/normal-1280.png", UiTraceData.ev "S-001" "normal")

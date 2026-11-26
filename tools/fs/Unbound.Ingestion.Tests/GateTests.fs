module Unbound.Ingestion.Tests.GateTests

open System.IO
open Xunit
open Unbound.Ingestion
open Unbound.Ingestion.Json

/// Repository root: walk up from the test binary until requirements/ exists.
let root =
    let rec up (d: DirectoryInfo) =
        if isNull d then failwith "repository root not found"
        elif Directory.Exists(Path.Combine(d.FullName, "requirements")) then d.FullName
        else up d.Parent
    up (DirectoryInfo(System.AppContext.BaseDirectory))

let inputs = lazy (Data.load root)

let set (name: string) (value: J) (j: J) =
    match j with
    | JObj ps -> JObj((ps |> List.filter (fst >> (<>) name)) @ [ name, value ])
    | _ -> j

let withRequirement (f: J -> J) (i: Data.Inputs) = { i with Requirements = (f i.Requirements.Head) :: i.Requirements.Tail }

[<Fact>]
let ``repository data passes every gate`` () =
    let failures = Gates.run inputs.Value |> List.collect snd
    Assert.True(failures.IsEmpty, System.String.Join("\n", failures))

[<Fact>]
let ``generated sections are current`` () =
    match Program.regenerate root inputs.Value with
    | Error e -> failwith (System.String.Join("\n", e))
    | Ok docs ->
        let stale = docs |> List.filter (fun (_, cur, next) -> cur <> next) |> List.map (fun (p, _, _) -> p)
        Assert.True(stale.IsEmpty, "stale: " + System.String.Join(", ", stale))

[<Fact>]
let ``I2 rejects acceptance without Given When Then`` () =
    let i = inputs.Value |> withRequirement (set "acceptance" (JStr "It works."))
    Assert.Contains(Gates.i2 i, fun s -> s.Contains "acceptance is not Given/When/Then")

[<Fact>]
let ``I2 rejects an unresolvable parent`` () =
    let i = inputs.Value |> withRequirement (set "origin" (JStr "refinement") >> set "parent" (JStr "CNV-NOPE-999"))
    Assert.Contains(Gates.i2 i, fun s -> s.Contains "does not resolve")

[<Fact>]
let ``I1 rejects duplicate requirement ids`` () =
    let i = { inputs.Value with Requirements = inputs.Value.Requirements.Head :: inputs.Value.Requirements }
    Assert.Contains(Gates.i1 i, fun s -> s.Contains "duplicate id")

[<Fact>]
let ``I4 detects an unreachable state and a transition out of a terminal state`` () =
    let m =
        parse
            """{"id":"M","initial":"A","states":[{"id":"A","terminal":false},{"id":"B","terminal":true},{"id":"C","terminal":false}],
                "transitions":[{"from":"A","event":"e","to":"B","actor":"x","authorization":"x","evidence":"x"},
                               {"from":"B","event":"e","to":"A","actor":"x","authorization":"x","evidence":"x"}]}"""
    let issues = Gates.machineIssues m
    Assert.Contains(issues, fun s -> s.Contains "terminal state B")
    Assert.Contains(issues, fun s -> s.Contains "'C' unreachable")

[<Fact>]
let ``I5 refuses an observation set that is not all-or-nothing`` () =
    let file, p = inputs.Value.Profiles |> List.find (fun (_, p) -> fs "artifact_class" p = "observation-set")
    Assert.Contains(Gates.profileIssues inputs.Value.Catalogs (file, set "atomicity" (JStr "record-independent") p), fun s -> s.Contains "must be all-or-nothing")

[<Fact>]
let ``I5 requires requester confirmation for principal command uploads`` () =
    let file, p = inputs.Value.Profiles |> List.find (fun (f, _) -> f = "grant.bulk-recommendation.csv.json")
    let changed = set "output" (parse """{"kind":"command","operation":"grant.recommend@1","submission":"automatic"}""") p
    Assert.Contains(Gates.profileIssues inputs.Value.Catalogs (file, changed), fun s -> s.Contains "requester confirmation")

[<Fact>]
let ``I7 flags an uncovered P0 security requirement`` () =
    let i = { inputs.Value with TestCases = [] }
    Assert.Contains(Gates.i7 i, fun s -> s.Contains "UNB-ING-SEC-005")

[<Fact>]
let ``splice refuses a document without markers`` () =
    Assert.True((Render.splice "no markers here" "x" "body").IsNone)

[<Fact>]
let ``csv reader handles quotes, commas and embedded newlines`` () =
    let rows = Data.parseCsv "a,b\n\"x, y\",\"line1\nline2 \"\"q\"\"\"\n"
    Assert.Equal<string list list>([ [ "a"; "b" ]; [ "x, y"; "line1\nline2 \"q\"" ] ], rows)

[<Fact>]
let ``I5 refuses a validation rule outside the catalog`` () =
    let file, p = inputs.Value.Profiles |> List.find (fun (f, _) -> f = "grant.bulk-recommendation.csv.json")
    let changed = set "validation_rules" (JArr [ JStr "fund.requester-has-grant-authority" ]) p
    Assert.Contains(Gates.profileIssues inputs.Value.Catalogs (file, changed), fun s -> s.Contains "not in catalog")

[<Fact>]
let ``I5 requires service identities to act on behalf of a principal`` () =
    let file, p = inputs.Value.Profiles |> List.find (fun (f, _) -> f = "grant.bulk-recommendation.csv.json")
    let changed = set "sources" (parse """[{"kind":"service-identity","authentication":["oauth-client-credentials"]}]""") p
    Assert.Contains(Gates.profileIssues inputs.Value.Catalogs (file, changed), fun s -> s.Contains "on behalf of a principal")

[<Fact>]
let ``lifecycle has no path from Held to Submitting that skips Validated`` () =
    let run = items (field "machines" inputs.Value.Lifecycle) |> List.find (fs "id" >> (=) "Run")
    let edges = items (field "transitions" run) |> List.map (fun t -> fs "from" t, fs "to" t)
    Assert.DoesNotContain(("Held", "Submitting"), edges)
    Assert.Contains(("Held", "Validated"), edges)

[<Fact>]
let ``lifecycle has no release of a quarantined artifact without inspection`` () =
    let art = items (field "machines" inputs.Value.Lifecycle) |> List.find (fs "id" >> (=) "Artifact")
    let into = items (field "transitions" art) |> List.filter (fs "to" >> (=) "Released") |> List.map (fun t -> fs "from" t, fs "event" t)
    Assert.Equal<(string * string) list>([ "Quarantined", "InspectionPassed"; "Revoked", "FalsePositiveConfirmed" ], into)

[<Fact>]
let ``I8 refuses a review finding without a disposition`` () =
    let r = inputs.Value.Review.Head |> Map.add "disposition" "later"
    let i = { inputs.Value with Review = r :: inputs.Value.Review.Tail }
    Assert.Contains(Gates.i8 i, fun s -> s.Contains "disposition")

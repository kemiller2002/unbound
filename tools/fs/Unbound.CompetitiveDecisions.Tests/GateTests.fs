module Unbound.CompetitiveDecisions.Tests.GateTests

open System.IO
open Xunit
open Unbound.CompetitiveDecisions
open Unbound.CompetitiveDecisions.Json

/// Repository root: walk up from the test binary until requirements/ exists.
let root =
    let rec up (d: DirectoryInfo) =
        if isNull d then failwith "repository root not found"
        elif Directory.Exists(Path.Combine(d.FullName, "requirements")) then d.FullName
        else up d.Parent
    up (DirectoryInfo(System.AppContext.BaseDirectory))

let inputs = lazy (Data.load root None)

let set (name: string) (value: J) (j: J) =
    match j with
    | JObj ps -> JObj((ps |> List.filter (fst >> (<>) name)) @ [ name, value ])
    | _ -> j

let withRequirement (f: J -> J) (i: Data.Inputs) = { i with Requirements = (f i.Requirements.Head) :: i.Requirements.Tail }
let withImplication (id: string) (f: J -> J) (i: Data.Inputs) =
    { i with Implications = i.Implications |> List.map (fun x -> if fs "id" x = id then f x else x) }
let withApproved (f: Map<string, string> -> Map<string, string>) (i: Data.Inputs) = { i with Approved = (f i.Approved.Head) :: i.Approved.Tail }

let has (prefix: string) (xs: string list) = xs |> List.exists (fun s -> s.StartsWith prefix)

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
let ``A1 requires every implication XRI-001..008`` () =
    let i = { inputs.Value with Implications = inputs.Value.Implications.Tail }
    Assert.True(has "A1 missing implication XRI-001" (Gates.a1 i))

[<Fact>]
let ``A3 rejects an unknown citation`` () =
    let i = inputs.Value |> withRequirement (set "refines" (JArr [ JStr "CNV-NOPE-999" ]) >> set "parent" (JStr "CNV-NOPE-999"))
    Assert.True(has "A3" (Gates.a3 i))

[<Fact>]
let ``A4 rejects acceptance without Given When Then`` () =
    let i = inputs.Value |> withRequirement (set "acceptance" (JStr "It works."))
    Assert.True(has "A4" (Gates.a4 i))

[<Fact>]
let ``A4 rejects an implemented requirement`` () =
    let i = inputs.Value |> withRequirement (set "implementation_status" (JStr "implemented"))
    Assert.True(has "A4" (Gates.a4 i))

[<Fact>]
let ``A5 rejects a decision advanced beyond strategic approval`` () =
    let i = inputs.Value |> withApproved (Map.add "unbound_status" "Implementation authorized")
    let found = Gates.a5 i
    Assert.True(has "A5" found)
    Assert.Contains(found, fun s -> s.Contains "ladder status 'Implementation authorized'")

[<Fact>]
let ``A5 rejects claiming requirements incorporated before ratification`` () =
    // DEC-001's requirements reverted to proposed: its 'Requirements incorporated' status is no longer supported
    let reverted = inputs.Value.Requirements |> List.map (fun r -> if fs "id" r = "UNB-CDD-CMF-001" then set "status" (JStr "proposed") r else r)
    let i = { inputs.Value with Requirements = reverted }
    Assert.True(has "A5 DEC-001" (Gates.a5 i))

[<Fact>]
let ``A5 rejects incorporation for a decision with no requirements`` () =
    let i = { inputs.Value with Approved = inputs.Value.Approved |> List.map (fun r -> if Data.col "decision_id" r = "DEC-012" then Map.add "unbound_status" "Requirements incorporated" r else r) }
    Assert.True(has "A5 DEC-012" (Gates.a5 i))

[<Fact>]
let ``A4 rejects an adopted requirement without a ratifying owner decision`` () =
    let i = inputs.Value |> withRequirement (set "ratified_by" (JStr "OD-M-99"))
    Assert.True(has "A4" (Gates.a4 i))

[<Fact>]
let ``A7 rejects an incomplete resolution`` () =
    let i = { inputs.Value with Decisions = (inputs.Value.Decisions.Head |> Map.add "owner_decision" "") :: inputs.Value.Decisions.Tail }
    Assert.True(has "A7" (Gates.a7 i))

[<Fact>]
let ``A6 rejects an unknown disposition class`` () =
    let i = inputs.Value |> withImplication "XRI-005" (set "disposition" (JStr "Done"))
    Assert.True(has "A6 XRI-005 disposition" (Gates.a6 i))

[<Fact>]
let ``A6 rejects 'Already satisfied' that creates requirements`` () =
    let i = inputs.Value |> withImplication "XRI-005" (set "requirements_created" (JArr [ JStr "UNB-CDD-CIV-001" ]))
    Assert.True(has "A6 XRI-005 'Already satisfied'" (Gates.a6 i))

[<Fact>]
let ``A6 rejects 'Requires a new owner decision' without one`` () =
    let i = inputs.Value |> withImplication "XRI-001" (set "owner_decisions" (JArr []))
    Assert.True(has "A6 XRI-001 'Requires a new owner decision'" (Gates.a6 i))

[<Fact>]
let ``A7 rejects a dangling owner decision reference`` () =
    let i = inputs.Value |> withRequirement (set "conflicts" (JArr [ JStr "see OD-CDD-99" ]))
    Assert.True(has "A7" (Gates.a7 i))

[<Fact>]
let ``A9 rejects an architecture change`` () =
    let i = { inputs.Value with Constraints = (inputs.Value.Constraints.Head |> Map.add "package_effect" "adds a message broker") :: inputs.Value.Constraints.Tail }
    Assert.True(has "A9" (Gates.a9 i))

[<Fact>]
let ``A9 requires the Echelon Foundry check`` () =
    let i = { inputs.Value with Constraints = inputs.Value.Constraints |> List.filter (fun c -> not ((Data.col "constraint" c).Contains "Echelon Foundry")) }
    Assert.True(has "A9 the Echelon Foundry" (Gates.a9 i))

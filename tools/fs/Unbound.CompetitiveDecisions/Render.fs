/// Generated Markdown sections. Pure: data in, text out. Each document has one section between
/// "<!-- BEGIN GENERATED -->" and "<!-- END GENERATED -->".
module Unbound.CompetitiveDecisions.Render

open System
open Unbound.CompetitiveDecisions.Json
open Unbound.CompetitiveDecisions.Data

let BEGIN, END = "<!-- BEGIN GENERATED -->", "<!-- END GENERATED -->"

let cell (s: string) =
    let t = s.Replace("\r", " ").Replace("\n", " ").Replace("|", "\\|").Trim()
    if t = "" then "—" else t

let table (header: string list) (rows: string list list) : string =
    let line (cells: string list) = "| " + String.Join(" | ", cells |> List.map cell) + " |"
    String.Join("\n", line header :: ("|" + String.replicate header.Length "---|") :: (rows |> List.map line)) + "\n"

let joined (j: J) = String.Join("; ", strs j)
let semi (cell: string) = String.Join("; ", ids cell)

let private researchPrefix = RESEARCH_PREFIX + RESEARCH + "/"
let research (j: J) = String.Join("; ", strs j |> List.map (fun p -> if p.StartsWith researchPrefix then p.Substring researchPrefix.Length else p))

let requirements (i: Inputs) : string =
    i.Requirements
    |> List.map (fun r ->
        let origin =
            match fs "parent" r with
            | "" -> $"""{fs "origin" r} (new requirement)"""
            | p -> $"""{fs "origin" r} (parent {p})"""
        $"""### {fs "id" r} — {fs "title" r}""" + "\n\n"
        + table
            [ "Field"; "Value" ]
            [ [ "Requirement"; fs "statement" r ]
              [ "Business rationale"; fs "rationale" r ]
              [ "Refines / applicable existing requirements"; joined (field "refines" r) ]
              [ "Origin"; origin ]
              [ "Approved decisions"; joined (field "approved_decisions" r) ]
              [ "Research (daf-landscape research/competitive-displacement/)"; research (field "supporting_research" r) ]
              [ "Research ids"; joined (field "research_ids" r) ]
              [ "Evidence gates"; joined (field "evidence_gates" r) ]
              [ "Acceptance criteria"; fs "acceptance" r ]
              [ "Dependencies"; joined (field "dependencies" r) ]
              [ "Security implications"; fs "security_implications" r ]
              [ "Failure behaviour"; fs "failure_behavior" r ]
              [ "Verification method"; fs "verification" r ]
              [ "Priority"; fs "priority" r ]
              [ "Status"; $"""{fs "status" r} · {fs "implementation_status" r} · {fs "validation_status" r}""" ]
              [ "Release scope"; fs "release_scope" r ]
              [ "Open questions"; joined (field "open_questions" r) ]
              [ "Conflicts"; joined (field "conflicts" r) ] ])
    |> fun blocks -> String.Join("\n", blocks)

let trace (i: Inputs) : string =
    let forward =
        table
            [ "Requirement"; "Title"; "Refines"; "Decisions"; "Implications"; "Priority"; "Scope" ]
            (i.Requirements
             |> List.map (fun r ->
                 [ fs "id" r; fs "title" r; joined (field "refines" r); joined (field "approved_decisions" r)
                   String.Join("; ", strs (field "research_ids" r) |> List.filter (fun s -> s.StartsWith "XRI-"))
                   fs "priority" r; fs "release_scope" r ]))
    let reverse =
        i.Requirements
        |> List.collect (fun r -> strs (field "refines" r) |> List.map (fun e -> e, fs "id" r))
        |> List.groupBy fst
        |> List.sortBy fst
        |> List.map (fun (e, xs) -> [ e; String.Join("; ", xs |> List.map snd) ])
    let coverage =
        table [ "Area"; "Capability"; "Requirements"; "Coverage"; "Note" ]
            (i.Coverage |> List.map (fun c -> [ col "area" c; col "capability" c; semi (col "requirement_ids" c); col "coverage" c; col "note" c ]))
    "## Forward trace\n\n" + forward + "\n## Reverse trace (existing → proposed)\n\n"
    + table [ "Existing requirement or clause"; "Refined by" ] reverse
    + "\n## Coverage of the owner brief's capability lists\n\n" + coverage

let implications (i: Inputs) : string =
    let summary =
        table [ "XRI"; "Area"; "Disposition"; "Created"; "Refined"; "Owner decisions" ]
            (i.Implications
             |> List.map (fun x ->
                 [ fs "id" x; fs "affected_area" x; fs "disposition" x; joined (field "requirements_created" x)
                   joined (field "requirements_refined" x); joined (field "owner_decisions" x) ]))
    let detail (x: J) =
        let row = field "research_row" x
        $"""### {fs "id" x} — {fs "disposition" x}""" + "\n\n"
        + table
            [ "Field"; "Value" ]
            [ [ "Research proposal (verbatim)"; fs "proposed_change" row ]
              [ "Research target area"; fs "target_area" row ]
              [ "Research type / status / blocking"; $"""{fs "type" row} / {fs "proposal_status" row} / {fs "blocking" row}""" ]
              [ "Affected area"; fs "affected_area" x ]
              [ "Research evidence"; research (field "research_evidence" x) ]
              [ "Claim and source ids"; joined (field "research_ids" x) + " (register: " + fs "source_ids" row + ")" ]
              [ "Affected existing requirements, clauses, decisions"; joined (field "affected_existing" x) ]
              [ "Already addressed?"; fs "already_addressed" x ]
              [ "Addressed by"; joined (field "addressed_by" x) ]
              [ "Conflicts"; fs "conflicts" x ]
              [ "Duplication"; fs "duplication" x ]
              [ "Gaps"; fs "gaps" x ]
              [ "Requirements created"; joined (field "requirements_created" x) ]
              [ "Existing requirements refined"; joined (field "requirements_refined" x) ]
              [ "Disposition"; fs "disposition" x ]
              [ "Why"; fs "disposition_rationale" x ]
              [ "Owner decisions raised"; joined (field "owner_decisions" x) ]
              [ "Approved decisions"; joined (field "approved_decisions" x) ]
              [ "Acceptance criteria"; String.Join(" ", strs (field "acceptance" x) |> List.mapi (fun n a -> $"({n + 1}) {a}")) ]
              [ "Validation requirements"; String.Join(" ", strs (field "validation" x) |> List.mapi (fun n a -> $"({n + 1}) {a}")) ]
              [ "Progression"; fs "progression" x ] ]
    "## Summary\n\n" + summary + "\n" + String.Join("\n", i.Implications |> List.map detail)

let decisions (i: Inputs) : string =
    let approved =
        table [ "Decision"; "Owner wording"; "Conditions"; "Gates"; "Unbound status"; "Requirements stage"; "Requirements"; "Owner decisions raised"; "Related open decisions" ]
            (i.Approved
             |> List.map (fun r ->
                 [ col "decision_id" r; col "owner_wording" r; col "conditions" r; semi (col "preserved_gates" r); col "unbound_status" r
                   col "requirements_stage" r; semi (col "requirement_ids" r); semi (col "owner_decisions_raised" r); semi (col "related_open_decisions" r) ]))
    let gates =
        table [ "Gate"; "Test"; "Informs"; "Unbound requirements"; "Lower-cost route"; "Status" ]
            (i.Gates |> List.map (fun g -> [ col "gate" g; col "test" g; semi (col "informs" g); semi (col "unbound_requirements" g); col "lower_cost_route" g; col "status" g ]))
    let ods =
        table [ "ID"; "Type"; "Question"; "Affected"; "Owner"; "Recommendation"; "Alternatives"; "Governs now"; "Blocks"; "Reversible" ]
            (i.Decisions
             |> List.map (fun d ->
                 [ col "decision_id" d; col "type" d; col "question" d; semi (col "affected_ids" d); col "decision_owner" d; col "recommendation" d
                   col "alternatives" d; col "governing_text_now" d; col "blocks_implementation" d; col "reversible" d ]))
    let questions =
        table [ "Requirement"; "Question" ]
            (i.Requirements |> List.collect (fun r -> strs (field "open_questions" r) |> List.map (fun q -> [ fs "id" r; q ])))
    "## Approved Phase II decisions: Unbound status\n\n" + approved
    + "\n## Validation gates (DEC-012)\n\n" + gates
    + "\n## Owner decisions (registered as open decisions)\n\n" + ods
    + "\n## Requirement questions (registered as `<id>#Q<n>`)\n\n" + questions

let reconciliation (i: Inputs) : string =
    let recon =
        table [ "Decision"; "Ratified or existing text touched"; "Text that governs now"; "Reconciliation"; "Owner decisions raised"; "Related open decisions" ]
            (i.Approved
             |> List.map (fun r ->
                 [ col "decision_id" r; semi (col "ratified_text_touched" r); col "governing_text_now" r; col "reconciliation" r
                   semi (col "owner_decisions_raised" r); semi (col "related_open_decisions" r) ]))
    let constraints =
        table [ "Constraint"; "Governing ids"; "Source"; "Effect of this package"; "Evidence" ]
            (i.Constraints |> List.map (fun c -> [ col "constraint" c; semi (col "governing_ids" c); col "governing_source" c; col "package_effect" c; col "evidence" c ]))
    let release1 = i.Requirements |> List.filter (fun r -> fs "release_scope" r = "initial-implementation")
    let p0 = release1 |> List.filter (fun r -> fs "priority" r = "P0") |> List.length
    let scope =
        $"This package proposes {release1.Length} requirements with release scope initial-implementation ({p0} P0). They would add to Release-1 scope only if the owner ratifies them into the baseline; until then the ratified Release-1 scope is unchanged and none of them is moved to another scope.\n\n"
        + table [ "Requirement"; "Title"; "Priority"; "Status" ]
            (release1 |> List.map (fun r -> [ fs "id" r; fs "title" r; fs "priority" r; "proposed, pending owner ratification" ]))
    "## Reconciliation with ratified clauses and owner decisions\n\n" + recon
    + "\n## Release-1 scope impact (proposals pending owner ratification)\n\n" + scope
    + "\n## Architecture and platform constraints\n\n" + constraints

/// (document, renderer) pairs.
let SECTIONS: (string * (Inputs -> string)) list =
    [ "01-TRACEABILITY.md", trace
      "02-REQUIREMENTS.md", requirements
      "03-CROSS-REPOSITORY-IMPLICATIONS.md", implications
      "04-DECISIONS-AND-OPEN-QUESTIONS.md", decisions
      "05-RECONCILIATION-AND-ARCHITECTURE.md", reconciliation ]

/// Replace the generated section of a document; None when the markers are missing.
let splice (document: string) (generated: string) : string option =
    let s = document.IndexOf BEGIN
    let e = document.IndexOf END
    if s < 0 || e < s then None
    else Some(document.Substring(0, s + BEGIN.Length) + "\n" + generated + document.Substring e)

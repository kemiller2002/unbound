/// Generated Markdown sections. Pure: data in, text out. A section lives between
/// "<!-- BEGIN GENERATED: name -->" and "<!-- END GENERATED: name -->" in a mission document.
module Unbound.Ingestion.Render

open System
open Unbound.Ingestion.Json
open Unbound.Ingestion.Data

let cell (s: string) = s.Replace("\r", " ").Replace("\n", " ").Replace("|", "\\|").Trim()

let table (header: string list) (rows: string list list) : string =
    let line (cells: string list) = "| " + String.Join(" | ", cells |> List.map cell) + " |"
    String.Join("\n", line header :: ("|" + String.replicate header.Length "---|") :: (rows |> List.map line))

let joined (j: J) = String.Join("; ", strs j)

let countBy (key: J -> string) (xs: J list) = xs |> List.countBy key |> List.sortBy fst

let matrixSummary (i: Inputs) : string =
    let priorities = [ "R1"; "R1-conditional"; "R2"; "Deferred"; "Eliminated" ]
    let domains = i.Workflows |> List.map (fs "domain") |> List.distinct
    let row d =
        let ws = i.Workflows |> List.filter (fs "domain" >> (=) d)
        d :: string ws.Length :: (priorities |> List.map (fun p -> ws |> List.filter (fs "release_priority" >> (=) p) |> List.length |> string))
    let total = "total" :: string i.Workflows.Length :: (priorities |> List.map (fun p -> i.Workflows |> List.filter (fs "release_priority" >> (=) p) |> List.length |> string))
    let disp = countBy (fs "disposition") i.Workflows |> List.map (fun (k, n) -> [ k; string n ])
    let cls = countBy (fs "artifact_class") i.Workflows |> List.map (fun (k, n) -> [ k; string n ])
    String.Join(
        "\n\n",
        [ table ("Domain" :: "Workflows" :: priorities) ((domains |> List.map row) @ [ total ])
          table [ "Disposition"; "Workflows" ] disp
          table [ "Artifact class"; "Workflows" ] cls ]
    )

let matrix (i: Inputs) : string =
    table
        [ "ID"; "Workflow"; "Class"; "Delivery"; "Format"; "Atomicity"; "Automation"; "AI"; "Disposition"; "Priority"; "Confidence"; "New requirements" ]
        (i.Workflows
         |> List.map (fun w ->
             [ fs "id" w; fs "name" w; fs "artifact_class" w; fs "delivery_mechanism" w; fs "expected_format" w; fs "atomicity" w
               fs "automation_potential" w; fs "ai_applicability" w; fs "disposition" w; fs "release_priority" w; fs "confidence" w
               joined (field "new_requirements" w) ]))

let AREA_TITLES =
    [ "BND", "Common ingestion boundary"; "PRF", "Ingestion profiles"; "LCY", "Lifecycle"; "SEC", "Security"
      "STO", "Storage"; "FIN", "Financial import"; "RCN", "Reconciliation"; "AI", "AI processing"; "DLV", "Delivery mechanisms"
      "FMT", "Formats"; "RET", "Retention and legal hold"; "UI", "Administrative and user interface"; "OPS", "Operations and dashboards"
      "ONB", "Partner onboarding"; "DAF", "DAF workflow requirements"; "TST", "Synthetic test corpus" ]

let requirement (r: J) : string =
    let parent =
        match field "parent" r with
        | JStr p -> $"refines {p}"
        | _ -> "new"
    let questions =
        match strs (field "open_questions" r) with
        | [] -> ""
        | qs -> "\n- **Open questions:** " + String.Join(" ", qs |> List.mapi (fun n q -> $"({n + 1}) {q}"))
    $"""#### {fs "id" r} ({fs "priority" r}, {parent})

{fs "statement" r}

- **Acceptance:** {fs "acceptance" r}
- **Rationale:** {fs "rationale" r}
- **Source:** {fs "source" r}
- **Verification:** {fs "verification" r} · **Release:** {fs "release_scope" r} · **Responsible:** {fs "responsible_system" r}
- **Echelon:** {joined (field "echelon_requirements" r)} · **Workflows:** {joined (field "workflows" r)}{questions}"""

let requirements (i: Inputs) : string =
    let counts =
        table [ "Area"; "Requirements"; "P0"; "P1"; "P2"; "Refinements"; "New" ]
            (AREA_TITLES
             |> List.map (fun (a, t) ->
                 let rs = i.Requirements |> List.filter (fs "area" >> (=) a)
                 let n f = rs |> List.filter f |> List.length |> string
                 [ $"{a} — {t}"; string rs.Length; n (fs "priority" >> (=) "P0"); n (fs "priority" >> (=) "P1"); n (fs "priority" >> (=) "P2")
                   n (fs "origin" >> (=) "refinement"); n (fs "origin" >> (=) "new") ]))
    let sections =
        AREA_TITLES
        |> List.map (fun (a, t) ->
            let rs = i.Requirements |> List.filter (fs "area" >> (=) a)
            $"### {a} — {t}\n\n" + String.Join("\n\n", rs |> List.map requirement))
    String.Join("\n\n", counts :: sections)

let operations (i: Inputs) : string =
    table [ "ID"; "Operation"; "Execution class"; "Ordering"; "Idempotency"; "Authorization timing"; "Failure"; "User status"; "Requirements" ]
        (i.Operations
         |> List.map (fun o ->
             [ fs "id" o; fs "operation" o; fs "execution_model" o; fs "ordering_boundary" o; fs "idempotency" o
               fs "authorization_timing" o; fs "failure_behavior" o; fs "user_status" o; joined (field "requirements" o) ]))

let formats (i: Inputs) : string =
    table [ "Format"; "Unbound decision"; "Basis"; "Relation to research recommendation" ]
        (i.Formats |> List.map (fun f -> [ f.["format"]; f.["unbound_decision"]; f.["basis"]; f.["relation_to_research_recommendation"] ]))

let lifecycle (i: Inputs) : string =
    let machine (m: J) =
        let states =
            table [ "State"; "Terminal"; "Meaning" ]
                (items (field "states" m) |> List.map (fun s -> [ fs "id" s; (if field "terminal" s = JBool true then "yes" else "no"); fs "meaning" s ]))
        let transitions =
            table [ "From"; "Event"; "To"; "Actor"; "Authorization"; "Evidence" ]
                (items (field "transitions" m)
                 |> List.map (fun t -> [ fs "from" t; fs "event" t; fs "to" t; fs "actor" t; fs "authorization" t; fs "evidence" t ]))
        $"""### {fs "id" m}

{fs "purpose" m} Initial state: **{fs "initial" m}**.

{states}

{transitions}"""
    let mapping =
        table [ "Proposed state (mission §11)"; "Disposition"; "Maps to"; "Reason" ]
            (items (field "proposed_state_mapping" i.Lifecycle)
             |> List.map (fun r -> [ fs "proposed" r; fs "disposition" r; fs "maps_to" r; fs "reason" r ]))
    let machines = items (field "machines" i.Lifecycle)
    let stateCount = machines |> List.sumBy (field "states" >> items >> List.length)
    String.Join(
        "\n\n",
        [ $"**{machines.Length} machines, {stateCount} states** replace the 21-state single lifecycle (the Record machine holds per-record outcomes)."
          "### Mapping of the 21 proposed states"
          mapping ]
        @ (machines |> List.map machine)
    )

let corpus (i: Inputs) : string =
    let byCat = countBy (fs "category") i.TestCases |> List.map (fun (k, n) -> [ k; string n ])
    String.Join(
        "\n\n",
        [ table [ "Category"; "Cases" ] byCat
          table [ "ID"; "Category"; "Title"; "Profile"; "Fixture (synthetic)"; "Expected result"; "Requirements" ]
              (i.TestCases
               |> List.map (fun t ->
                   [ fs "id" t; fs "category" t; fs "title" t; fs "profile" t; fs "fixture" t; fs "expected" t; joined (field "requirements" t) ])) ]
    )

let decisions (i: Inputs) : string =
    table [ "ID"; "Question"; "Owner"; "Route"; "Recommendation"; "Alternatives"; "Rationale"; "Blocks implementation" ]
        (i.Decisions
         |> List.map (fun d ->
             [ d.["decision_id"]; d.["question"]; d.["decision_owner"]; d.["resolution_route"]; d.["recommendation"]; d.["alternatives"]
               d.["rationale"]; d.["blocks_implementation"] ]))

let reconciliation (i: Inputs) : string =
    let counts = i.Reconciliation |> List.countBy (fun r -> r.["status_after_mission"]) |> List.sortBy fst |> List.map (fun (k, n) -> [ k; string n ])
    let primary = i.Reconciliation |> List.countBy (fun r -> r.["primary_classification"]) |> List.sortBy fst |> List.map (fun (k, n) -> [ k; string n ])
    String.Join(
        "\n\n",
        [ "Primary classification takes the weakest class a topic carries (conflicting, then missing, insufficient, deferred, covered); for example \"covered (data) / missing (files)\" counts as missing."
          table [ "Primary classification before this mission"; "Topics" ] primary
          table [ "Status after this mission"; "Topics" ] counts
          table [ "#"; "Topic"; "Inventory classification"; "Existing (governing first)"; "Resolution in this mission"; "Status" ]
              (i.Reconciliation
               |> List.map (fun r -> [ r.["topic_no"]; r.["topic"]; r.["inventory_classification"]; r.["existing_ids"]; r.["resolution"]; r.["status_after_mission"] ])) ]
    )

let review (i: Inputs) : string =
    let counts =
        i.Review |> List.countBy (fun r -> r.["severity"], r.["disposition"]) |> List.sortBy fst
        |> List.map (fun ((sev, d), n) -> [ sev; d; string n ])
    String.Join(
        "\n\n",
        [ table [ "Severity"; "Disposition"; "Findings" ] counts
          table [ "Finding"; "Severity"; "Issue"; "Disposition"; "Resulting change or reason" ]
              (i.Review |> List.map (fun r -> [ r.["finding_id"]; r.["severity"]; r.["finding"]; r.["disposition"]; r.["resulting_change"] ])) ]
    )

/// (document, section, renderer)
let SECTIONS : (string * string * (Inputs -> string)) list =
    [ "01-WORKFLOW-COVERAGE-MATRIX.md", "summary", matrixSummary
      "01-WORKFLOW-COVERAGE-MATRIX.md", "matrix", matrix
      "02-REQUIREMENTS.md", "requirements", requirements
      "03-INGESTION-BOUNDARY-AND-PROFILES.md", "operations", operations
      "03-INGESTION-BOUNDARY-AND-PROFILES.md", "formats", formats
      "04-SECURE-INGESTION-LIFECYCLE.md", "lifecycle", lifecycle
      "11-SYNTHETIC-TEST-CORPUS.md", "corpus", corpus
      "13-REQUIREMENTS-RECONCILIATION.md", "reconciliation", reconciliation
      "14-QUESTIONS-AND-OWNER-DECISIONS.md", "decisions", decisions
      "16-ADVERSARIAL-REVIEW.md", "review", review ]

let BEGIN (name: string) = $"<!-- BEGIN GENERATED: {name} -->"
let END (name: string) = $"<!-- END GENERATED: {name} -->"

/// Replace the body of a named generated section; None when the markers are missing.
let splice (doc: string) (name: string) (body: string) : string option =
    let b, e = BEGIN name, END name
    let i, j = doc.IndexOf(b, StringComparison.Ordinal), doc.IndexOf(e, StringComparison.Ordinal)
    if i < 0 || j < i then None
    else Some(doc.Substring(0, i + b.Length) + "\n" + body + "\n" + doc.Substring(j))

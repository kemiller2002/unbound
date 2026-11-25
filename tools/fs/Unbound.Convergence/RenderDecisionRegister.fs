/// Render the master-mission decision register and rejected-alternatives report from the resolution log
/// (port of tools/reqaudit/render_decision_register.py).
module Unbound.Convergence.RenderDecisionRegister

open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat
module QL = Unbound.Convergence.QuestionsLib

let MISSION = "requirements/convergence/2026-11-24-master"

let OWNER_DECISIONS =
    [ "SPD-ADV-001", "No team exercises reserved authority for more than one sponsor; each sponsor decides with its own people.", "Product owner, 2026-11-24"
      "SPD-ADV-004",
      "Unbound is a software platform only; customers employ everyone who does human work; hooks kept for staffed services. AMENDED by OD-M-08: software first, Unbound-operated administration as a planned second offer.",
      "Product owner, 2026-11-24"
      "SPD-ECN-003", "Platform pricing includes a per-sponsor component in addition to per-account fees (decided-provisional).", "Product owner, 2026-11-24" ]

/// cell(t, n): str(t or ""), whitespace collapsed, pipes escaped, truncated to n code points with an ellipsis.
let cell (n: int option) (t: Json option) : string =
    let x = (joinWs (str (t |> Option.defaultValue JNull |> orElse (JStr "")))).Replace("|", "\\|")
    match n with
    | Some n when cpLen x > n -> cpTake (n - 1) x + "…"
    | _ -> x

let table (h: string list) (rows: string list list) : string =
    [ "| " + String.concat " | " h + " |"; "|" + String.replicate h.Length "---|" ] @ (rows |> List.map (fun r -> "| " + String.concat " | " r + " |"))
    |> String.concat "\n"

/// Every answer text this question held before its current one.
let supersededAnswers (state: Json) : Json list =
    match get "superseded_answer" state with
    | Some a when truthy a && Some a <> get "answer" state -> [ a ]
    | _ -> []

let private readLines (p: string) = Io.readJsonl p

let main (io: Io.Console) (argv: string list) : int =
    let root = argv.[1]
    let states = QL.fold (QL.loadEvents root)
    let reg = QL.loadCsv (Io.path root "requirements/open-questions.csv") |> List.map (fun r -> itemStr "question_id" r, r) |> Map.ofList
    let dossiers = readLines (Io.path root (MISSION + "/data/dossiers.jsonl")) |> List.map (fun d -> itemStr "question_id" d, d) |> toDict
    let clauses = readLines (Io.path root (MISSION + "/data/convergence-clauses.jsonl"))
    let od = Io.path root (MISSION + "/data/owner-decisions.jsonl")
    let ownerDecisions = if Io.exists od then readLines od else []
    let sorted = QL.sortedStates states
    let gateOf (st: Json) = Some(getOr "external_authorization_required" JNull st |> orElse (getOr "activation_gate" JNull st))
    let rows =
        sorted
        |> List.map (fun (q, st) ->
            let r = reg.[q]
            let appliedTo = getOr "applied_to" (JArr []) st |> orElse (JArr []) |> strs |> String.concat ", "
            let incorporated =
                if appliedTo <> "" then appliedTo
                elif truthyOpt (get "canonical_id" st) then "dup of " + getStr "canonical_id" st
                else ""
            [ q; str (item "kind" r); cell (Some 140) (Some(JStr(removePrefix "Confirm provisional decision: " (itemStr "question" r))))
              cell (Some 260) (get "answer" st); getStr "disposition" st; getStr "approval_status" st; incorporated; cell (Some 160) (gateOf st) ])
    let rejected =
        sorted |> List.collect (fun (q, st) -> supersededAnswers st |> List.map (fun a -> [ q; cell (Some 220) (Some a); cell (Some 200) (get "answer" st) ]))
    let competing =
        dossiers
        |> sortByStrs (fun (q, _) -> [ q ])
        |> List.filter (fun (_, d) -> truthyOpt (get "competing_interpretation" d) || truthyOpt (get "alternative_design" d))
        |> List.map (fun (q, d) -> [ q; cell (Some 260) (get "competing_interpretation" d); cell (Some 260) (get "alternative_design" d) ])
    let priorRejected =
        [ "requirements/discovery/2026-11-23-cross-industry/data/rejected.jsonl"
          "requirements/discovery/2026-11-24-simplification-economics/data/rejected.jsonl" ]
        |> List.sumBy (fun p -> splitLines (Io.readText (Io.path root p)) |> List.filter (fun l -> strip l <> "") |> List.length)
    let gates =
        sorted
        |> List.filter (fun (_, st) -> get "disposition" st = Some(JStr "external-authorization-required") && get "resolution_status" st = Some(JStr "resolved"))
        |> List.map (fun (q, st) -> [ q; strOpt (get "disposition" st); cell (Some 300) (gateOf st) ])
    let activation =
        clauses |> List.filter (fun c -> truthy (item "activation_gate" c))
        |> List.map (fun c -> [ itemStr "id" c; str (item "release_scope" c); cell (Some 300) (get "activation_gate" c) ])
    let statusCounts =
        QL.stateItems states
        |> List.map (fun (_, st) -> [ strOpt (get "resolution_status" st); strOpt (get "approval_status" st) ])
        |> counter
        |> List.sortWith (fun (a, _) (b, _) -> cmpStrs a b)
        |> List.map (fun (k, v) -> [ $"{k.[0]} / {k.[1]}"; string v ])
    let dispCounts =
        QL.stateItems states
        |> List.map (fun (_, st) -> getStr "disposition" st)
        |> counter
        |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b)
        |> List.map (fun (k, v) -> [ "disposition: " + k; string v ])
    let text =
        ([ "# 09 — Decision register, rejected alternatives and activation gates"
           "Generated by `tools/fs/Unbound.Convergence` (`render-decision-register`) from the resolution log, dossiers and convergence clauses; do not edit by hand."
           "## 1. Owner decisions outside the question register\n\n"
           + table
               [ "Decision"; "Decision text"; "Authority" ]
               ((OWNER_DECISIONS |> List.map (fun (a, b, c) -> [ a; b; c ]))
                @ (ownerDecisions |> List.map (fun d -> [ itemStr "id" d; cell (Some 600) (get "decision" d); cell (Some 160) (get "decided_by" d) ])))
           "## 2. Summary\n\n" + table [ "Measure"; "Count" ] (statusCounts @ dispCounts)
           $"## 3. Regulatory and external activation gates ({gates.Length} questions; {activation.Length} clauses)\n\n"
           + "These are the only places where an external party's act is required. Each names the authorization; none is a generic consultation.\n\n"
           + table [ "Question"; "Disposition"; "Exact authorization required" ] gates
           + "\n\n"
           + table [ "Clause"; "Release scope"; "Activation gate" ] activation
           $"## 4. Decision register ({rows.Length} questions)\n\n"
           + table [ "ID"; "Kind"; "Question"; "Decision"; "Disposition"; "Approval"; "Incorporated as"; "Gate" ] rows
           $"## 5. Rejected or replaced directions ({rejected.Length})\n\nAnswers that an earlier review proposed and a later review or evidence replaced.\n\n"
           + table [ "Question"; "Rejected or replaced direction"; "Current decision" ] rejected
           $"## 6. Competing interpretations and alternative designs considered ({competing.Length})\n\n"
           + table [ "Question"; "Competing interpretation (not adopted)"; "Alternative design (recorded for reconsideration)" ] competing
           $"## 7. Earlier missions' rejected approaches\n\n{priorRejected} rejected approaches are recorded in "
           + "`requirements/discovery/2026-11-23-cross-industry/data/rejected.jsonl` and `requirements/discovery/2026-11-24-simplification-economics/data/rejected.jsonl` "
           + "(report 19 of the simplification mission). None was reinstated by this mission." ]
         |> String.concat "\n\n")
        + "\n"
    Io.writeText (Io.path root (MISSION + "/09-DECISION-REGISTER-AND-REJECTED-ALTERNATIVES.md")) text
    io.Out($"{rows.Length} decisions; {rejected.Length} rejected; {gates.Length} gates\n")
    0

/// Consolidate every unanswered question into one register (port of tools/reqaudit/render_questions.py).
/// Pure functions build the rows; main does the I/O.
module Unbound.Convergence.RenderQuestions

open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat
module QL = Unbound.Convergence.QuestionsLib

let RECON, DISC, SIMP, CNO = QL.RECON, QL.DISC, QL.SIMP, QL.CNO

let SOURCES =
    [ RECON, "Reconciliation", "14-OPEN-DECISIONS.md"
      DISC, "Cross-industry discovery", "15-OPEN-DECISIONS-AND-SCENARIOS.md"
      SIMP, "Simplification and economics", "20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md"
      QL.FIL, "File ingestion and STP", "14-QUESTIONS-AND-OWNER-DECISIONS.md"
      CNO, "Charity network and onboarding", "04-DECISIONS-AND-OPEN-QUESTIONS.md"
      QL.CDD, "Competitive displacement decisions", "04-DECISIONS-AND-OPEN-QUESTIONS.md" ]

let OWNER_GROUPS =
    [ "Counsel", [ "counsel"; "legal"; " gc "; " gc,"; "(gc" ]
      "Finance: CFO, controller, CPA, auditor",
      [ "cpa"; "auditor"; "cfo"; "controller"; "finance"; "treasury"; "audit"; "tax advisor"; "return preparer" ]
      "Technology: architecture, security, privacy",
      [ "architecture"; "cto"; "security"; "privacy"; "engineering"; "spike"; "forma"; "usability" ]
      "Product owner",
      [ "product"; "requirements owner"; "program owner"; "commercial"; "marketing"; "economics"; "owner"; "template review"
        "consent-capture" ]
      "Sponsor governance and operations",
      [ "board"; "governance"; "compliance"; "operations"; "coo"; "executive"; "head of"; "crco"; "committee" ] ]

let FALLBACK = "Sponsor governance and operations", [ "sponsor" ]
let GROUP_ORDER = (OWNER_GROUPS |> List.map fst) @ [ "Unassigned" ]

let FIELDS =
    [ "question_id"; "kind"; "owner_group"; "owner"; "question"; "context"; "blocks_implementation"; "source"; "status"
      "answer"; "answered_by"; "answered_on"; "resolution_status"; "approval_status"; "disposition"; "canonical_id"
      "activation_gate"; "external_authorization_required"; "evidence_refs"; "last_event"; "resolution_events" ]

type Row = Map<string, Json>

let ownerGroup (owner: Json) : string =
    let o = " " + lower (str (owner |> orElse (JStr ""))) + " "
    let hits =
        OWNER_GROUPS
        |> List.mapi (fun i (g, ks) -> ks |> List.filter (fun k -> contains k o) |> List.map (fun k -> find k o, i, g))
        |> List.concat
    let fallback = if snd FALLBACK |> List.exists (fun k -> contains k o) then fst FALLBACK else "Unassigned"
    match hits with
    | [] -> fallback
    | _ -> hits |> List.minBy (fun (pos, i, _) -> pos, i) |> fun (_, _, g) -> g

let needsConfirmation (text: Json) : bool =
    let t = strip (str text) |> lower |> rstripChars "."
    not (t = "" || t = "none" || (startsWith "none" t && not ([ "beyond"; "before"; "counsel"; "cpa" ] |> List.exists (fun w -> contains w t))))

let row (kw: (string * Json) list) : Row =
    let m = Map.ofList kw
    FIELDS |> List.map (fun f -> f, m.TryFind f |> Option.defaultValue (JStr "")) |> Map.ofList

let private s (x: string) = JStr x

let openDecisions (root: string) (b: string) (label: string) (report: string) (pinned: Set<string>) : Row list =
    QL.loadCsv (Io.path root (b + "/data/decisions.csv"))
    |> List.filter (fun d -> item "status" d = s "open" || (match item "decision_id" d with JStr i -> pinned.Contains i | _ -> false))
    |> List.map (fun d ->
        row [ "question_id", item "decision_id" d; "kind", s "open decision"; "owner", item "decision_owner" d
              "owner_group", s (ownerGroup (item "decision_owner" d)); "question", item "question" d
              "context", s ("Recommendation: " + itemStr "recommendation" d)
              "blocks_implementation", getOr "blocks_implementation" (s "") d
              "source", s $"{pathStr [ b; report ]} ({label})"; "status", item "status" d ])

let confirmations (root: string) (pinned: Set<string>) : Row list =
    let isPinned (d: Json) = match item "decision_id" d with JStr i -> pinned.Contains i | _ -> false
    let recon =
        QL.loadCsv (Io.path root (RECON + "/data/decisions.csv"))
        |> List.filter (fun d ->
            (item "status" d = s "decided-provisional" && needsConfirmation (getOr "validation_required" (s "none") d)) || isPinned d)
        |> List.map (fun d ->
            row [ "question_id", item "decision_id" d; "kind", s "awaiting confirmation"; "owner", item "validation_required" d
                  "owner_group", s (ownerGroup (item "validation_required" d))
                  "question", s ("Confirm provisional decision: " + itemStr "question" d)
                  "context", s ("Decided: " + str (getOr "decided_option" (s "") d))
                  "blocks_implementation", getOr "blocks_implementation" (s "") d
                  "source", s $"""{pathStr [ RECON; "14-OPEN-DECISIONS.md" ]} (Reconciliation)"""; "status", item "status" d ])
    let questions = QL.loadCsv (Io.path root (DISC + "/data/decisions.csv")) |> List.map (fun d -> str (item "decision_id" d), d) |> Map.ofList
    let q (o: Json) = questions.TryFind(str (item "decision_id" o)) |> Option.defaultValue (JObj [])
    let disc =
        QL.loadJsonl (Io.path root (DISC + "/data/decision-outcomes.jsonl"))
        |> List.filter (fun o -> needsConfirmation (getOr "confirmation_required" (s "") o) || isPinned o)
        |> List.map (fun o ->
            row [ "question_id", item "decision_id" o; "kind", s "awaiting confirmation"; "owner", item "confirmation_required" o
                  "owner_group", s (ownerGroup (item "confirmation_required" o))
                  "question", s ("Confirm provisional decision: " + str (getOr "question" (s "") (q o)))
                  "context", s ("Decided: " + itemStr "decision" o)
                  "blocks_implementation", getOr "blocks_implementation" (s "") (q o)
                  "source", s $"""{pathStr [ DISC; "16-LEGAL-AND-ACCOUNTING-DECISIONS.md" ]} (Cross-industry discovery)"""
                  "status", item "status" o ])
    recon @ disc

let requirementQuestions (root: string) : Row list =
    SOURCES
    |> List.collect (fun (b, label, _) ->
        [ "gap-requirements.jsonl"; "refinements.jsonl" ]
        |> List.collect (fun name ->
            QL.loadJsonl (Io.path root (b + "/data/" + name))
            |> List.collect (fun r ->
                let rid = str (QL.recordId r)
                QL.openQuestions r
                |> List.mapi (fun i q -> i + 1, q)
                |> List.filter (fun (_, q) -> strip (str q) <> "")
                |> List.map (fun (i, q) ->
                    row [ "question_id", s $"{rid}#Q{i}"; "kind", s "requirement question"; "owner", s "Product owner (route as needed)"
                          "owner_group", s "Product owner"; "question", q
                          "context", s (cpTake 400 (str (getOr "statement" JNull r |> orElse (s ""))))
                          "source", s $"""{pathStr [ b; "data"; name ]} ({label})"""
                          "status", (getOr "approval_status" JNull r |> orElse (getOr "validation_status" (s "") r)) ]))))

let private f (k: string) (r: Row) : Json = r.TryFind k |> Option.defaultValue (s "")
let private fs (k: string) (r: Row) : string = str (f k r)

let statusCell (r: Row) : string =
    [ fs "resolution_status" r; fs "disposition" r; (if truthy (f "canonical_id" r) then "dup of " + fs "canonical_id" r else "") ]
    |> List.filter ((<>) "")
    |> String.concat "; "

let blocksFirst (rows: Row list) : Row list =
    rows |> List.sortWith (fun a b ->
        match compare (fs "blocks_implementation" a <> "Y") (fs "blocks_implementation" b <> "Y") with
        | 0 -> cmpStr (fs "question_id" a) (fs "question_id" b)
        | c -> c)

let cell (limit: int option) (text: string) : string =
    let t = (joinWs text).Replace("|", "\\|")
    match limit with
    | Some n when cpLen t > n -> rstrip (cpTake (n - 1) t) + "…"
    | _ -> t

let table (headers: string list) (rows: string list list) : string =
    [ "| " + String.concat " | " headers + " |"; "|" + String.replicate headers.Length "---|" ]
    @ (rows |> List.map (fun r -> "| " + String.concat " | " r + " |"))
    |> String.concat "\n"

let render (rows: Row list) : string =
    let byKind k = rows |> List.filter (fun r -> fs "kind" r = k)
    let od, ac, rq = byKind "open decision", byKind "awaiting confirmation", byKind "requirement question"
    let counts = rows |> List.map (fun r -> fs "owner_group" r, fs "kind" r) |> counter |> Map.ofList
    let count g k = counts.TryFind((g, k)) |> Option.defaultValue 0
    let kinds = [ "open decision"; "awaiting confirmation"; "requirement question" ]
    let summary =
        table [ "Who answers"; "Open decisions"; "Awaiting confirmation"; "Requirement questions"; "Total" ]
            ((GROUP_ORDER
              |> List.filter (fun g -> kinds |> List.exists (fun k -> count g k <> 0))
              |> List.map (fun g ->
                  [ g; string (count g "open decision"); string (count g "awaiting confirmation"); string (count g "requirement question")
                    string (kinds |> List.sumBy (count g)) ]))
             @ [ [ "**Total**"; $"**{od.Length}**"; $"**{ac.Length}**"; $"**{rq.Length}**"; $"**{rows.Length}**" ] ])
    let first = blocksFirst (od |> List.filter (fun r -> fs "blocks_implementation" r = "Y"))
    let odSection g =
        let rs = blocksFirst (od |> List.filter (fun r -> fs "owner_group" r = g))
        if rs.IsEmpty then ""
        else
            $"### {g} ({rs.Length})\n\n"
            + table [ "ID"; "Question"; "Recommendation"; "Blocks"; "Owner"; "Status"; "Current answer" ]
                (rs |> List.map (fun r ->
                    [ fs "question_id" r; cell None (fs "question" r); cell (Some 300) (removePrefix "Recommendation: " (fs "context" r))
                      fs "blocks_implementation" r; cell None (fs "owner" r); statusCell r; cell (Some 300) (fs "answer" r) ]))
    let acSection g =
        let rs = ac |> List.filter (fun r -> fs "owner_group" r = g)
        if rs.IsEmpty then ""
        else
            $"### {g} ({rs.Length})\n\n"
            + table [ "ID"; "What to confirm"; "Provisional decision"; "Who confirms"; "Status"; "Current answer" ]
                (rs |> List.map (fun r ->
                    [ fs "question_id" r; cell (Some 250) (removePrefix "Confirm provisional decision: " (fs "question" r))
                      cell (Some 250) (removePrefix "Decided: " (fs "context" r)); cell None (fs "owner" r); statusCell r
                      cell (Some 300) (fs "answer" r) ]))
    let rqRows =
        rq
        |> sortByStrs (fun r -> [ fs "question_id" r ])
        |> List.map (fun r ->
            [ fs "question_id" r; cell None (fs "question" r); cell None ((splitOn " (" (fs "source" r)).[1] |> rstripChars ")"); statusCell r
              cell (Some 300) (fs "answer" r) ])
    let statusRows =
        rows
        |> List.map (fun r -> [ fs "resolution_status" r; fs "approval_status" r; fs "disposition" r ])
        |> counter
        |> List.sortWith (fun (a, _) (b, _) -> cmpStrs a b)
        |> List.map (fun (k, n) -> [ k.[0]; k.[1]; (if k.[2] = "" then "—" else k.[2]); string n ])
    let parts =
        [ "# Open questions register"
          "Every question in the requirements missions that still needs an answer, in one place. "
          + "Generated by `tools/fs/Unbound.Convergence` (`render-questions`) from the mission data files and the resolution log; do not edit by hand. "
          + "Answers, approvals, evidence dispositions and their history are recorded only in the append-only log "
          + "[`questions/resolution-events.jsonl`](questions/resolution-events.jsonl) (see [`questions/README.md`](questions/README.md)); "
          + "[`open-questions.csv`](open-questions.csv) is an output that joins each question to its current resolution. "
          + "The tool refuses to regenerate if the CSV holds an answer the log does not."
          "**Three kinds of question:**\n\n"
          + "- **Open decisions** have not been decided. Each has a recommendation.\n"
          + "- **Awaiting confirmation** are decided provisionally and need the named professional or owner to confirm them.\n"
          + "- **Requirement questions** are open points on proposed requirements and refinements."
          "**Who answers** is grouped by keyword from each record's owner field, with counsel taking precedence. "
          + "Unbound is a software platform only (SPD-ADV-004), so a question owned by a sponsor role usually means "
          + "the product owner decides the default and the setting the sponsor can change."
          "## Summary\n\n" + summary
          "## Resolution status\n\n" + table [ "Resolution status"; "Approval"; "Evidence disposition"; "Questions" ] statusRows
          $"## Decide first: open decisions that block implementation ({first.Length})\n\n"
          + table [ "ID"; "Question"; "Who answers" ] (first |> List.map (fun r -> [ fs "question_id" r; cell None (fs "question" r); fs "owner_group" r ]))
          $"## 1. Open decisions ({od.Length})" ]
        @ (GROUP_ORDER |> List.map odSection |> List.filter ((<>) ""))
        @ [ $"## 2. Provisional decisions awaiting confirmation ({ac.Length})" ]
        @ (GROUP_ORDER |> List.map acSection |> List.filter ((<>) ""))
        @ [ $"## 3. Open questions on proposed requirements ({rq.Length})\n\n"
            + "Most of these are answered by the product owner, routing to counsel, finance or architecture where the question says so.\n\n"
            + table [ "Question ID"; "Question"; "Mission"; "Status"; "Current answer" ] rqRows
            "## Not consolidated here\n\n"
            + "- **Earlier research questions:** `research/daf-theoretical-enterprise-validation/19-questions/` and "
            + "`research/daf-theoretical-enterprise-validation/16-registers/owner-decisions.md`. They were inputs to the "
            + "reconciliation mission; this register does not re-check whether each one was carried forward.\n"
            + "- **Evidence gaps** that need measurement rather than a decision: report 20 of the simplification mission." ]
    (parts |> List.filter ((<>) "") |> String.concat "\n\n") + "\n"

let toCsv (rows: Row list) : string = csvQuoteAll "\n" ([ FIELDS ] @ (rows |> List.map (fun r -> FIELDS |> List.map (fun k -> fs k r))))

let pinnedIds (states: QL.States) (kind: string) : Set<string> =
    QL.stateItems states |> List.filter (fun (_, st) -> get "register_kind" st = Some(JStr kind)) |> List.map fst |> Set.ofList

/// Source-derived rows; each question appears once, under the kind pinned by its registered event.
let candidateRows (root: string) (states: QL.States) : Row list =
    let rows =
        (SOURCES |> List.collect (fun (b, label, report) -> openDecisions root b label report (pinnedIds states "open decision")))
        @ confirmations root (pinnedIds states "awaiting confirmation")
        @ requirementQuestions root
    let keep (r: Row) =
        let k = f "kind" r
        (match states.ById.TryFind(fs "question_id" r) with
         | Some st -> getOr "register_kind" k st
         | None -> k) = k
    rows
    |> List.filter keep
    |> List.fold (fun (seen: Set<string>, acc) r -> let q = fs "question_id" r in if seen.Contains q then seen, acc else seen.Add q, r :: acc) (Set.empty, [])
    |> snd
    |> List.rev

/// Overlay the folded resolution state on a register row.
let join (r: Row) (st: Json option) : Row =
    match st with
    | None -> r |> Map.add "resolution_status" (s "open") |> Map.add "approval_status" (s "not-approved")
    | Some st ->
        let hist = QL.history st
        let last = List.tryLast hist |> Option.defaultValue (JObj [])
        let answered = truthyOpt (get "answer" st)
        let lastEvent = $"""{getStr "event_id" last} {getStr "type" last} {getStr "artifact" last}""" |> strip
        [ "answer", getOr "answer" (s "") st
          "answered_by", (getOr "approved_by" (s "") st |> orElse (if answered then getOr "actor" (s "") last else s ""))
          "answered_on", (getOr "approved_on" (s "") st |> orElse (if answered then getOr "on" (s "") last else s ""))
          "resolution_status", getOr "resolution_status" (s "") st
          "approval_status", getOr "approval_status" (s "") st
          "disposition", getOr "disposition" (s "") st
          "canonical_id", getOr "canonical_id" (s "") st
          "activation_gate", getOr "activation_gate" (s "") st
          "external_authorization_required", getOr "external_authorization_required" (s "") st
          "evidence_refs", s (getOr "evidence_refs" (JArr []) st |> orElse (JArr []) |> strs |> String.concat ";")
          "last_event", s lastEvent
          "resolution_events", s (string hist.Length) ]
        |> List.fold (fun (acc: Row) (k, v) -> acc.Add(k, v)) r

/// Every answer text ever recorded in the log, per question.
let loggedAnswers (events: Json list) : Map<string, Set<string>> =
    events
    |> List.fold
        (fun (acc: Map<string, Set<string>>) e ->
            let st = getOr "set" JNull e |> orElse (JObj [])
            match get "answer" st with
            | Some a when truthy a ->
                let q = itemStr "question_id" e
                acc.Add(q, (acc.TryFind q |> Option.defaultValue Set.empty).Add(strip (str a)))
            | _ -> acc)
        Map.empty

/// CSV answers that the log never recorded.
let discardedHandAnswers (existing: Json list) (logged: Map<string, Set<string>>) : string list =
    existing
    |> List.filter (fun e ->
        let a = strip (str (getOr "answer" JNull e |> orElse (s "")))
        a <> "" && not ((logged.TryFind(str (item "question_id" e)) |> Option.defaultValue Set.empty).Contains a))
    |> List.map (fun e -> str (item "question_id" e))

let main (io: Io.Console) (argv: string list) : int =
    let root = if argv.Length > 1 then argv.[1] else "."
    let events = QL.loadEvents root
    let states = QL.fold events
    let findings =
        QL.validate events states (QL.sourceDecisions root) (QL.sourceRequirementQuestions root) (QL.loadCsv (Io.path root QL.CLUSTERS))
    findings |> List.iter (fun (a, b, c, d) -> io.Err($"{a} | {b} | {c} | {d}\n"))
    let b = candidateRows root states
    let baseIds = b |> List.map (fs "question_id") |> Set.ofList
    let orphans = states.Order |> List.filter (fun q -> not (baseIds.Contains q)) |> sortStrs
    let errs = QL.errors findings
    if not errs.IsEmpty || not orphans.IsEmpty then
        io.Err($"refusing to write: {errs.Length} validation errors; orphaned resolutions: {reprStrList orphans}\n")
        2
    else
        let rows = b |> List.map (fun r -> join r (states.ById.TryFind(fs "question_id" r)))
        let csvRel = "requirements/open-questions.csv"
        let csvPath = Io.path root csvRel
        let lost =
            if List.contains "--allow-discard" argv then []
            else discardedHandAnswers (QL.loadCsv csvPath) (loggedAnswers events)
        if not lost.IsEmpty then
            io.Err(
                $"refusing to write: {lost.Length} answers in {pathStr [ root; csvRel ]} are not in {QL.EVENTS}: {reprStrList (List.truncate 10 lost)}\n"
            )
            3
        else
            Io.writeText (Io.path root "requirements/OPEN-QUESTIONS.md") (render rows)
            Io.writeText csvPath (toCsv rows)
            let kinds = rows |> List.map (fs "kind") |> counter
            io.Out("{" + (kinds |> List.map (fun (k, n) -> $"{reprStr k}: {n}") |> String.concat ", ") + "}" + $" total {rows.Length}\n")
            let pairs = rows |> List.map (fun r -> fs "resolution_status" r, fs "approval_status" r) |> counter
            io.Out("{" + (pairs |> List.map (fun ((a, b), n) -> $"({reprStr a}, {reprStr b}): {n}") |> String.concat ", ") + "}\n")
            io.Out($"warnings: {findings.Length}\n")
            0

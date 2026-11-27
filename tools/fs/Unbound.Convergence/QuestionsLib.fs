/// Source-linked, event-sourced question resolutions (port of tools/reqaudit/questions_lib.py). Pure functions
/// except the load* helpers.
module Unbound.Convergence.QuestionsLib

open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat

let RECON = "requirements/reconciliation/2026-11-23"
let DISC = "requirements/discovery/2026-11-23-cross-industry"
let SIMP = "requirements/discovery/2026-11-24-simplification-economics"
let FIL = "requirements/discovery/2026-11-26-file-ingestion"
let CNO = "requirements/discovery/2026-11-26-charity-network-and-onboarding"
let CDD = "requirements/discovery/2026-11-26-competitive-displacement-decisions"

let MISSIONS =
    [ RECON, "Reconciliation", "14-OPEN-DECISIONS.md"
      DISC, "Cross-industry discovery", "15-OPEN-DECISIONS-AND-SCENARIOS.md"
      SIMP, "Simplification and economics", "20-OPEN-DECISIONS-AND-EVIDENCE-GAPS.md"
      FIL, "File ingestion and STP", "14-QUESTIONS-AND-OWNER-DECISIONS.md"
      CNO, "Charity network and onboarding", "04-DECISIONS-AND-OPEN-QUESTIONS.md"
      CDD, "Competitive displacement decisions", "04-DECISIONS-AND-OPEN-QUESTIONS.md" ]

let EVENTS = "requirements/questions/resolution-events.jsonl"
let CLUSTERS = "requirements/questions/question-clusters.csv"

let EVENT_TYPES =
    [ "registered"; "proposed"; "dispositioned"; "corrected"; "evidence-reviewed"; "resolved"; "approved"; "superseded"
      "reopened"; "applied-to-requirement"; "text-reconfirmed" ]

let RESOLUTION_STATUSES = [ "open"; "proposed"; "resolved"; "superseded-by-duplicate"; "withdrawn" ]
let APPROVAL_STATUSES = [ "not-approved"; "agent-recommended"; "approved-product-direction"; "approved-owner"; "rejected" ]

let DISPOSITIONS =
    [ "established-by-authoritative-evidence"; "reasoned-interpretation"; "business-policy-decision"
      "empirical-validation-required"; "external-authorization-required"; "unresolved" ]

/// sha256 of the stripped text.
let sha (text: string) : string = sha256Hex (strip text)

/// load_csv: rows of a CSV file, or none when the file is absent.
let loadCsv (p: string) : Json list = if Io.exists p then Io.readCsvDicts p else []

/// load_jsonl
let loadJsonl (p: string) : Json list = if Io.exists p then Io.readJsonl p else []

// ---------------------------------------------------------------- source questions

/// Every decision in every mission: decision_id -> row (only the question text is consulted).
let sourceDecisions (root: string) : Map<string, Json> =
    MISSIONS
    |> List.collect (fun (b, _, _) -> loadCsv (Io.path root (b + "/data/decisions.csv")))
    |> List.map (fun d -> itemStr "decision_id" d, d)
    |> Map.ofList

/// rid of a record: r.get("id") or r.get("refinement_id")
let recordId (r: Json) : Json = (getOr "id" JNull r) |> orElse (getOr "refinement_id" JNull r)

/// The open_questions of a record as a list (a bare string is one question).
let openQuestions (r: Json) : Json list =
    match getOr "open_questions" JNull r |> orElse (JArr []) with
    | JStr s -> [ JStr s ]
    | qs -> asList qs

/// Every open_questions entry of every record: qid -> question.
let sourceRequirementQuestions (root: string) : Map<string, Json> =
    MISSIONS
    |> List.collect (fun (b, _, _) ->
        [ "gap-requirements.jsonl"; "refinements.jsonl" ]
        |> List.collect (fun name -> loadJsonl (Io.path root (b + "/data/" + name))))
    |> List.collect (fun r ->
        let rid = str (recordId r)
        openQuestions r
        |> List.mapi (fun i q -> i + 1, q)
        |> List.filter (fun (_, q) -> strip (str q) <> "")
        |> List.map (fun (i, q) -> $"{rid}#Q{i}", q))
    |> Map.ofList

let sourceText (qid: string) (decisions: Map<string, Json>) (reqqs: Map<string, Json>) : Json option =
    (if contains "#Q" qid then reqqs.TryFind qid
     else decisions.TryFind qid |> Option.map (item "question"))
    |> Option.filter ((<>) JNull)

// ---------------------------------------------------------------- event fold

/// Folded resolution states in first-event order.
type States = { Order: string list; ById: Map<string, Json> }

let loadEvents (root: string) : Json list = loadJsonl (Io.path root EVENTS)

let history (state: Json) : Json list =
    match get "history" state with
    | Some h -> asList h
    | None -> []

let applyEvent (state: Json) (event: Json) : Json =
    let merged = merge state (getOr "set" (JObj []) event)
    let h = JObj([ "event_id"; "on"; "type"; "artifact"; "actor" ] |> List.choose (fun k -> get k event |> Option.map (fun v -> k, v)))
    set "history" (JArr(history state @ [ h ])) merged

let fold (events: Json list) : States =
    let step (order: string list, byId: Map<string, Json>) (e: Json) =
        let q = itemStr "question_id" e
        let current =
            byId.TryFind q
            |> Option.defaultValue (JObj [ "question_id", JStr q; "resolution_status", JStr "open"; "approval_status", JStr "not-approved" ])
        (if byId.ContainsKey q then order else q :: order), byId.Add(q, applyEvent current e)
    let order, byId = events |> List.fold step ([], Map.empty)
    { Order = List.rev order; ById = byId }

let stateItems (s: States) : (string * Json) list = s.Order |> List.map (fun q -> q, s.ById.[q])
let stateOf (q: string) (s: States) : Json = s.ById.TryFind q |> Option.defaultValue (JObj [])
let sortedStates (s: States) : (string * Json) list = stateItems s |> sortByStrs (fun (q, _) -> [ q ])

let private histType (h: Json) = itemStr "type" h

let lastTextEventIndex (state: Json) : int =
    history state |> List.mapi (fun i h -> i, h) |> List.filter (fun (_, h) -> List.contains (histType h) [ "registered"; "text-reconfirmed" ])
    |> List.fold (fun _ (i, _) -> i) -1

let answerEventIndex (state: Json) : int =
    history state
    |> List.mapi (fun i h -> i, h)
    |> List.filter (fun (_, h) -> List.contains (histType h) [ "proposed"; "corrected"; "resolved"; "evidence-reviewed" ])
    |> List.fold (fun _ (i, _) -> i) -1

// ---------------------------------------------------------------- validation

/// (severity, code, question_id, message)
type Finding = string * string * string * string

let private inList (xs: string list) (v: Json option) =
    match v with
    | Some(JStr s) -> List.contains s xs
    | _ -> false

let validate (events: Json list) (states: States) (decisions: Map<string, Json>) (reqqs: Map<string, Json>) (clusters: Json list) : Finding list =
    let ids = events |> List.map (fun e -> get "event_id" e) |> counter
    let dupEvents = ids |> List.filter (fun (_, n) -> n > 1) |> List.map (fun (i, n) -> "error", "duplicate-event-id", "", $"{strOpt i} appears {n} times")
    let badTypes =
        events
        |> List.filter (fun e -> not (inList EVENT_TYPES (get "type" e)))
        |> List.map (fun e -> "error", "invalid-event-type", itemStr "question_id" e, $"""{strOpt (get "event_id" e)}: {strOpt (get "type" e)}""")
    let unregistered =
        stateItems states
        |> List.filter (fun (_, s) -> not (history s |> List.exists (fun h -> histType h = "registered")))
        |> List.map (fun (q, _) -> "error", "unregistered-question", q, "events exist but no 'registered' event")
    let perQuestion (q: string) (s: Json) : Finding list =
        let text = sourceText q decisions reqqs
        let status, approval = get "resolution_status" s, get "approval_status" s
        let disp = getOr "disposition" (JStr "") s
        let statusIs (v: string) = status = Some(JStr v)
        let dispIs (v: string) = disp = JStr v
        let approvalStr = match approval with Some(JStr a) -> a | Some v -> str v | None -> raise (System.InvalidCastException "'NoneType' object has no attribute 'startswith'")
        let checks =
            [ (if text.IsNone then Some("error", "missing-source-record", "source record no longer exists") else None)
              (match text with
               | Some t when Some(JStr(sha (str t))) <> get "question_sha256" s ->
                   Some("error", "changed-question-text", "source text differs from registered text; re-review and add a 'text-reconfirmed' event")
               | _ -> None)
              (if not (inList RESOLUTION_STATUSES status) then Some("error", "invalid-status", $"resolution_status {reprOpt status}") else None)
              (if not (inList APPROVAL_STATUSES approval) then Some("error", "invalid-approval", $"approval_status {reprOpt approval}") else None)
              (if truthy disp && not (inList DISPOSITIONS (Some disp)) then Some("error", "invalid-disposition", $"disposition {repr disp}") else None)
              (if statusIs "resolved" && not (truthyOpt (get "answer" s)) then Some("error", "invalid-closure", "resolved without an answer") else None)
              (if statusIs "resolved" && dispIs "unresolved" then Some("error", "invalid-closure", "resolved with disposition 'unresolved'") else None)
              (if statusIs "resolved" && dispIs "external-authorization-required" && not (truthyOpt (get "external_authorization_required" s)) then
                   Some("error", "invalid-closure", "external authorization disposition without naming the authorization")
               else None)
              (if startsWith "approved" approvalStr && statusIs "open" then Some("error", "invalid-closure", "approved but still open") else None)
              (if statusIs "superseded-by-duplicate"
                  && not (match get "canonical_id" s with Some(JStr c) -> states.ById.ContainsKey c | _ -> false) then
                   Some("error", "dangling-duplicate", $"""canonical {reprOpt (get "canonical_id" s)} has no resolution""")
               else None)
              (if truthyOpt (get "answer" s) && answerEventIndex s < lastTextEventIndex s then
                   Some("warning", "stale-resolution", "answer predates the latest text confirmation")
               else None)
              (if not (truthyOpt (get "answer" s)) && not (statusIs "superseded-by-duplicate") then
                   Some("warning", "no-answer", "no proposed or resolved answer yet")
               else None) ]
        checks |> List.choose id |> List.map (fun (a, b, c) -> a, b, q, c)
    let conflict (memberId: string) (canonical: string) (relation: string) : Finding list =
        let m, c = stateOf memberId states, stateOf canonical states
        let both = get "resolution_status" m = Some(JStr "resolved") && get "resolution_status" c = Some(JStr "resolved")
        let differ = truthyOpt (get "disposition" m) && truthyOpt (get "disposition" c) && get "disposition" m <> get "disposition" c
        if relation = "same-question" && both && differ then
            [ "warning", "conflicting-dispositions", memberId,
              $"""{relation} of {canonical} but dispositions differ: {strOpt (get "disposition" m)} vs {strOpt (get "disposition" c)}""" ]
        else []
    dupEvents @ badTypes @ unregistered
    @ (sortedStates states |> List.collect (fun (q, s) -> perQuestion q s))
    @ (clusters |> List.collect (fun c -> conflict (itemStr "member_id" c) (itemStr "canonical_id" c) (itemStr "relation" c)))

let errors (findings: Finding list) : Finding list = findings |> List.filter (fun (s, _, _, _) -> s = "error")

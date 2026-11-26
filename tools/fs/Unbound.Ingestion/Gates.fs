/// Quality gates for the file-ingestion mission. Every gate is a pure function from the loaded inputs to the
/// issues it finds; an empty list means the gate passes.
///
///   I1  identifiers unique and well formed (UNB-ING, FIW, OPC, TC, FIL-OD, EFI)
///   I2  requirements: normative fields, Given/When/Then, P0-P2, status proposed, resolvable parent and citations,
///       workflow references resolve
///   I3  workflow matrix: every mission §7 field present; references resolve; release priority vocabulary
///   I4  lifecycle: states and transitions consistent, reachable, terminals final, 21 proposed states mapped
///   I5  profiles: closed vocabularies and cross-field rules (class/output, atomicity, submission, zip use)
///   I6  operation classification rows complete, classes valid, requirements resolve
///   I7  test corpus: unique ids, requirements resolve, every P0 SEC/FIN/STO/AI requirement covered
///   I8  decisions and format decisions: unique ids, resolution route, affected ids resolve
module Unbound.Ingestion.Gates

open System.Text.RegularExpressions
open Unbound.Ingestion.Json
open Unbound.Ingestion.Data

type Issue = string

let private rx (p: string) = Regex(p, RegexOptions.CultureInvariant)
let private matches (r: Regex) (s: string) = r.IsMatch s

let ING_ID = rx @"^UNB-ING-(BND|PRF|LCY|SEC|STO|FIN|RCN|AI|UI|OPS|ONB|DLV|FMT|RET|DAF|TST)-\d{3}$"
let FIW_ID = rx @"^FIW-[A-Z]{2,3}-\d{2}$"
let OPC_ID = rx @"^OPC-\d{3}$"
let TC_ID = rx @"^TC-[A-Z]{2,3}-\d{3}$"
let OD_ID = rx @"^FIL-OD-\d{2}$"
let EFI_ID = rx @"^EFI-[A-Z]{2,3}-\d{3}$"
let FIE_ID = rx @"^FIE-[ABC]-\d{3}$"
let GWT = rx @"(?s)\bGiven\b.*\bWhen\b.*\bThen\b"
let ING = rx @"\b(?:UNB-ING-[A-Z]+-\d{3}|FIL-OD-\d{2}|ADR-ING-\d{3})\b"
let CITED = rx @"\b(?:CNV|RR|UNB|REQ|ADM|INT|ACC)-[A-Z0-9-]*\d(?:-r\d)?\b"

let duplicates (ids: string list) : string list =
    ids |> List.countBy id |> List.filter (fun (_, n) -> n > 1) |> List.map fst |> List.sort

let private wellFormed (code: string) (r: Regex) (ids: string list) : Issue list =
    (ids |> List.filter (matches r >> not) |> List.map (sprintf "%s malformed id '%s'" code))
    @ (duplicates ids |> List.map (sprintf "%s duplicate id '%s'" code))

let requirementIds (i: Inputs) = i.Requirements |> List.map (fs "id")
let workflowIds (i: Inputs) = i.Workflows |> List.map (fs "id") |> Set.ofList

/// Every id this mission may cite: baseline, availability mission, this mission's requirements.
let known (i: Inputs) : Set<string> =
    Set.unionMany [ i.BaselineIds; i.AvailabilityIds; Set.ofList (requirementIds i) ]

let i1 (i: Inputs) : Issue list =
    wellFormed "I1" ING_ID (requirementIds i)
    @ wellFormed "I1" FIW_ID (i.Workflows |> List.map (fs "id"))
    @ wellFormed "I1" OPC_ID (i.Operations |> List.map (fs "id"))
    @ wellFormed "I1" TC_ID (i.TestCases |> List.map (fs "id"))
    @ wellFormed "I1" OD_ID (i.Decisions |> List.map (fun d -> d.["decision_id"]))
    @ (i.Requirements
       |> List.collect (fun r -> strs (field "echelon_requirements" r))
       |> List.distinct
       |> List.filter (matches EFI_ID >> not)
       |> List.map (sprintf "I1 malformed Echelon requirement id '%s'"))

let REQUIREMENT_FIELDS =
    [ "id"; "area"; "origin"; "statement"; "rationale"; "source"; "applicability"; "responsible_system"; "acceptance"
      "verification"; "priority"; "status"; "implementation_status"; "release_scope"; "echelon_requirements"; "workflows" ]

let private requirementIssues (k: Set<string>) (wf: Set<string>) (r: J) : Issue list =
    let id = fs "id" r
    let missing = REQUIREMENT_FIELDS |> List.filter (fun f -> not (nonEmpty (field f r))) |> List.map (sprintf "I2 %s missing field %s" id)
    let gwt = if matches GWT (fs "acceptance" r) then [] else [ sprintf "I2 %s acceptance is not Given/When/Then" id ]
    let prio = if List.contains (fs "priority" r) [ "P0"; "P1"; "P2" ] then [] else [ sprintf "I2 %s priority '%s'" id (fs "priority" r) ]
    let status = if fs "status" r = "proposed" then [] else [ sprintf "I2 %s status must be 'proposed'" id ]
    let origin =
        match fs "origin" r, field "parent" r with
        | "refinement", JStr p when k.Contains p -> []
        | "refinement", JStr p -> [ sprintf "I2 %s parent '%s' does not resolve" id p ]
        | "refinement", _ -> [ sprintf "I2 %s refinement without parent" id ]
        | "new", JNull -> []
        | "new", _ -> [ sprintf "I2 %s new requirement must not name a parent" id ]
        | o, _ -> [ sprintf "I2 %s origin '%s'" id o ]
    let cited =
        CITED.Matches(fs "statement" r + " " + fs "source" r)
        |> Seq.map (fun m -> m.Value)
        |> Seq.filter (fun c -> not (k.Contains c))
        |> Seq.distinct
        |> Seq.map (sprintf "I2 %s cites unknown id '%s'" id)
        |> List.ofSeq
    let flows =
        strs (field "workflows" r) |> List.filter (fun w -> w <> "*" && not (wf.Contains w)) |> List.map (sprintf "I2 %s unknown workflow '%s'" id)
    missing @ gwt @ prio @ status @ origin @ cited @ flows

let i2 (i: Inputs) : Issue list =
    let k, wf = known i, workflowIds i
    i.Requirements |> List.collect (requirementIssues k wf)

let WORKFLOW_FIELDS =
    [ "id"; "domain"; "business_purpose"; "source_organization"; "source_identity"; "delivery_mechanism"; "expected_format"
      "schema_availability"; "frequency"; "estimated_volume"; "expected_file_size"; "data_sensitivity"; "tenant_ownership"
      "authentication"; "authorization"; "security_inspection"; "business_validation"; "duplicate_detection"; "reconciliation"
      "approval"; "atomicity"; "effective_date"; "retention"; "legal_hold"; "dependencies"; "exception_handling"
      "recovery_replay"; "automation_potential"; "ai_applicability"; "estimated_human_effort"; "existing_unbound_requirements"
      "confidence"; "release_priority" ]

let PRIORITIES = [ "R1"; "R1-conditional"; "R2"; "Deferred"; "Eliminated" ]

let i3 (i: Inputs) : Issue list =
    let k = known i
    i.Workflows
    |> List.collect (fun w ->
        let id = fs "id" w
        (WORKFLOW_FIELDS |> List.filter (fun f -> not (nonEmpty (field f w))) |> List.map (sprintf "I3 %s missing field %s" id))
        @ (if List.contains (fs "release_priority" w) PRIORITIES then [] else [ sprintf "I3 %s release_priority '%s'" id (fs "release_priority" w) ])
        @ (if List.contains (fs "confidence" w) [ "confirmed"; "inferred"; "proposed" ] then [] else [ sprintf "I3 %s confidence '%s'" id (fs "confidence" w) ])
        @ (if fs "confidence" w = "confirmed" && not (nonEmpty (field "evidence" w)) then [ sprintf "I3 %s confirmed without evidence" id ] else [])
        @ (strs (field "existing_unbound_requirements" w) @ strs (field "new_requirements" w)
           |> List.filter (fun x -> not (k.Contains x))
           |> List.map (sprintf "I3 %s unknown requirement '%s'" id))
        @ (strs (field "evidence" w) |> List.filter (matches FIE_ID >> not) |> List.map (sprintf "I3 %s malformed evidence id '%s'" id)))

/// States reachable from the initial state.
let reachable (initial: string) (edges: (string * string) list) : Set<string> =
    let rec go (seen: Set<string>) (frontier: string list) =
        match frontier with
        | [] -> seen
        | s :: rest ->
            let next = edges |> List.filter (fst >> (=) s) |> List.map snd |> List.filter (seen.Contains >> not) |> List.distinct
            go (Set.union seen (Set.ofList next)) (rest @ next)
    go (Set.singleton initial) [ initial ]

let machineIssues (m: J) : Issue list =
    let name = fs "id" m
    let states = items (field "states" m)
    let ids = states |> List.map (fs "id")
    let terminal = states |> List.filter (field "terminal" >> (=) (JBool true)) |> List.map (fs "id") |> Set.ofList
    let edges = items (field "transitions" m) |> List.map (fun t -> fs "from" t, fs "to" t)
    let undefined =
        edges |> List.collect (fun (a, b) -> [ a; b ]) |> List.distinct |> List.filter (fun s -> not (List.contains s ids))
        |> List.map (sprintf "I4 %s transition references undefined state '%s'" name)
    let init = fs "initial" m
    let initIssue = if List.contains init ids then [] else [ sprintf "I4 %s initial state '%s' undefined" name init ]
    let fromTerminal = edges |> List.filter (fun (a, _) -> terminal.Contains a) |> List.map (fun (a, b) -> sprintf "I4 %s terminal state %s has transition to %s" name a b)
    let deadEnds =
        ids |> List.filter (fun s -> not (terminal.Contains s) && not (edges |> List.exists (fst >> (=) s)))
        |> List.map (sprintf "I4 %s non-terminal state '%s' has no outgoing transition" name)
    let reach = reachable init edges
    let unreachable = ids |> List.filter (reach.Contains >> not) |> List.map (sprintf "I4 %s state '%s' unreachable" name)
    let noTerminal = if terminal.IsEmpty then [ sprintf "I4 %s has no terminal state" name ] else []
    let incomplete =
        items (field "transitions" m)
        |> List.filter (fun t -> [ "event"; "actor"; "authorization"; "evidence" ] |> List.exists (fun f -> not (nonEmpty (field f t))))
        |> List.map (fun t -> sprintf "I4 %s transition %s->%s lacks event/actor/authorization/evidence" name (fs "from" t) (fs "to" t))
    (wellFormed "I4" (rx @"^[A-Z][A-Za-z]+$") ids) @ undefined @ initIssue @ fromTerminal @ deadEnds @ unreachable @ noTerminal @ incomplete

let PROPOSED_STATES = 21

let i4 (i: Inputs) : Issue list =
    let machines = items (field "machines" i.Lifecycle)
    let mapping = items (field "proposed_state_mapping" i.Lifecycle)
    let count = if mapping.Length = PROPOSED_STATES then [] else [ sprintf "I4 proposed_state_mapping has %d rows, expected %d" mapping.Length PROPOSED_STATES ]
    let unexplained = mapping |> List.filter (fun r -> not (nonEmpty (field "reason" r)) || not (nonEmpty (field "maps_to" r))) |> List.map (fs "proposed" >> sprintf "I4 mapping for '%s' lacks reason or target")
    (machines |> List.collect machineIssues) @ count @ unexplained

let OUTPUT_FOR_CLASS =
    Map.ofList
        [ "command-batch", "command"; "observation-set", "observation"; "reference-dataset", "reference-snapshot"
          "evidence-document", "evidence-attachment"; "configuration", "configuration-change" ]

let keys (j: J) : string list =
    match j with
    | JObj ps -> ps |> List.map fst
    | _ -> []

let RECORD_CLASSES = [ "command-batch"; "observation-set"; "reference-dataset" ]

let profileIssues (catalogs: J) (file: string, p: J) : Issue list =
    let pid = fs "profile_id" p
    let cls = fs "artifact_class" p
    let output = field "output" p
    let kind = fs "kind" output
    let sources = items (field "sources" p)
    let principal = sources |> List.exists (fs "kind" >> (=) "principal-role")
    let formats = items (field "formats" p) |> List.map (fs "type")
    let crossTenant = field "cross_tenant_source" (field "tenant_scope" p) = JBool true
    let purpose = fs "purpose" p
    let purposeFormats = strs (field purpose (field "purposes" catalogs))
    let rules = keys (field "validation_rules" catalogs)
    let corroborations = keys (field "corroborations" catalogs)
    let operations = field "operations" catalogs
    let declared = items (field "fields" (field "record_schema" p)) |> List.map (fs "name")
    let single (cond: bool) (msg: string) = if cond then [ msg ] else []
    [ single (file <> pid + ".json") (sprintf "I5 %s file name must be <profile_id>.json" file)
      (match OUTPUT_FOR_CLASS.TryFind cls with
       | Some k when k = kind -> []
       | Some k -> [ sprintf "I5 %s class %s must output %s, not %s" pid cls k kind ]
       | None -> [ sprintf "I5 %s unknown artifact_class '%s'" pid cls ])
      single (List.contains cls [ "observation-set"; "reference-dataset" ] && fs "atomicity" p <> "all-or-nothing")
          (sprintf "I5 %s %s must be all-or-nothing" pid cls)
      single (List.contains cls RECORD_CLASSES && not (has "record_schema" p)) (sprintf "I5 %s record-processing class needs record_schema" pid)
      single (cls = "evidence-document" && (has "atomicity" p || has "duplicate_detection" p)) (sprintf "I5 %s evidence documents carry no record-processing fields" pid)
      single (principal && kind = "command" && fs "submission" output <> "requester-confirms") (sprintf "I5 %s principal uploads must require requester confirmation" pid)
      single (cls = "command-batch" && sources |> List.exists (fun s -> fs "kind" s = "service-identity" && field "on_behalf_of_principal" s <> JBool true))
          (sprintf "I5 %s service identities must act on behalf of a principal (UNB-ING-BND-008)" pid)
      single (List.contains "zip-trusted-dataset" formats && not (cls = "reference-dataset" && crossTenant))
          (sprintf "I5 %s zip-trusted-dataset only for tenant-neutral reference datasets" pid)
      single (has "control_totals" p && fs "on_mismatch" (field "control_totals" p) <> "reject-run") (sprintf "I5 %s control-total mismatch must reject the run" pid)
      single (fs "unknown_fields" (field "record_schema" p) = "ignore-and-report" && cls <> "reference-dataset")
          (sprintf "I5 %s ignore-and-report is permitted only for reference datasets" pid)
      single (fs "mode" (field "extraction" p) = "ai-proposal" && not (nonEmpty (field "provider_policy_ref" (field "extraction" p))))
          (sprintf "I5 %s ai-proposal requires provider_policy_ref" pid)
      single (fs "mode" (field "extraction" p) = "ai-proposal" && not (nonEmpty (field "corroboration" (field "extraction" p))))
          (sprintf "I5 %s ai-proposal requires deterministic corroboration" pid)
      single (purposeFormats.IsEmpty) (sprintf "I5 %s purpose '%s' is not in the catalog" pid purpose)
      (formats |> List.filter (fun f -> not purposeFormats.IsEmpty && not (List.contains f purposeFormats))
       |> List.map (sprintf "I5 %s format '%s' not permitted for purpose" pid))
      (strs (field "validation_rules" p) |> List.filter (fun r -> not (List.contains r rules)) |> List.map (sprintf "I5 %s rule '%s' not in catalog" pid))
      (strs (field "corroboration" (field "extraction" p)) |> List.filter (fun r -> not (List.contains r corroborations))
       |> List.map (sprintf "I5 %s corroboration '%s' not in catalog" pid))
      (match fs "operation" output with
       | "" -> []
       | op when field "interactive" (field op operations) = JBool true -> []
       | op -> [ sprintf "I5 %s operation '%s' is not a catalogued interactive operation" pid op ])
      (strs (field "record_key" (field "duplicate_detection" p)) |> List.filter (fun k -> not (List.contains k declared))
       |> List.map (sprintf "I5 %s record_key '%s' is not a declared field" pid)) ]
    |> List.concat

let i5 (i: Inputs) : Issue list =
    (i.Profiles |> List.collect (profileIssues i.Catalogs))
    @ (duplicates (i.Profiles |> List.map (snd >> fs "profile_id")) |> List.map (sprintf "I5 duplicate profile '%s'"))

let OPERATION_FIELDS =
    [ "id"; "operation"; "requirements"; "execution_model"; "consistency"; "ordering_boundary"; "idempotency"
      "authorization_timing"; "queue_eligibility"; "cache_eligibility"; "failure_behavior"; "recovery_behavior"; "audit"
      "user_status"; "acceptance" ]

let CLASSES =
    [ "ImmediateQuery"; "ImmediateControl"; "ImmediateCommand"; "DurableCommand"; "ScheduledCommand"; "LongRunningOperation"
      "HumanApprovalWorkflow"; "ExternalIntegrationOperation" ]

let i6 (i: Inputs) : Issue list =
    let k = known i
    i.Operations
    |> List.collect (fun o ->
        let id = fs "id" o
        (OPERATION_FIELDS |> List.filter (fun f -> not (nonEmpty (field f o))) |> List.map (sprintf "I6 %s missing field %s" id))
        @ ((fs "execution_model" o).Split('+') |> Array.map (fun s -> s.Trim()) |> Array.filter (fun c -> not (List.contains c CLASSES))
           |> Array.map (sprintf "I6 %s unknown execution class '%s'" id) |> List.ofArray)
        @ (if fs "execution_model" o = "ImmediateControl" && fs "queue_eligibility" o <> "No" then [ sprintf "I6 %s ImmediateControl must not be queue-eligible" id ] else [])
        @ (strs (field "requirements" o) |> List.filter (k.Contains >> not) |> List.map (sprintf "I6 %s unknown requirement '%s'" id))
        @ (if matches GWT (fs "acceptance" o) then [] else [ sprintf "I6 %s acceptance is not Given/When/Then" id ]))

/// Every P0 requirement must be exercised by at least one synthetic test case unless its only verification is an
/// architectural review (an explicit, visible exemption).
let reviewOnly (r: J) = (fs "verification" r).Trim() = "Architectural review"

let i7 (i: Inputs) : Issue list =
    let k = known i
    let referenced = i.TestCases |> List.collect (field "requirements" >> strs) |> Set.ofList
    let unknown =
        i.TestCases
        |> List.collect (fun t -> strs (field "requirements" t) |> List.filter (k.Contains >> not) |> List.map (sprintf "I7 %s unknown requirement '%s'" (fs "id" t)))
    let incomplete =
        i.TestCases
        |> List.filter (fun t -> [ "fixture"; "expected"; "profile"; "category" ] |> List.exists (fun f -> not (nonEmpty (field f t))))
        |> List.map (fs "id" >> sprintf "I7 %s lacks fixture, expected result, profile or category")
    let uncovered =
        i.Requirements
        |> List.filter (fun r -> fs "priority" r = "P0" && not (reviewOnly r) && not (referenced.Contains(fs "id" r)))
        |> List.map (fs "id" >> sprintf "I7 P0 requirement %s has no synthetic test case")
    unknown @ incomplete @ uncovered

let ROUTES = [ "owner"; "agent"; "external" ]

let i8 (i: Inputs) : Issue list =
    let k = known i
    let decisions =
        i.Decisions
        |> List.collect (fun d ->
            let id = d.["decision_id"]
            (if List.contains (Map.tryFind "resolution_route" d |> Option.defaultValue "") ROUTES then [] else [ sprintf "I8 %s resolution_route" id ])
            @ ((Map.tryFind "affected_ids" d |> Option.defaultValue "").Split(';')
               |> Array.filter (fun x -> x <> "" && not (k.Contains x))
               |> Array.map (sprintf "I8 %s affects unknown id '%s'" id) |> List.ofArray)
            @ ([ "question"; "recommendation"; "decision_owner"; "alternatives"; "rationale" ]
               |> List.filter (fun f -> (Map.tryFind f d |> Option.defaultValue "").Trim() = "")
               |> List.map (sprintf "I8 %s missing %s" id)))
    let decisionValues = [ "initial"; "conditional"; "deferred"; "not-applicable"; "R2" ]
    let formats =
        i.Formats
        |> List.filter (fun f -> not (decisionValues |> List.exists (fun v -> f.["unbound_decision"].StartsWith v)))
        |> List.map (fun f -> sprintf "I8 format '%s' has unknown decision '%s'" f.["format"] f.["unbound_decision"])
    let decisionIds = i.Decisions |> List.map (fun d -> d.["decision_id"]) |> Set.ofList
    let resolvable (x: string) = k.Contains x || decisionIds.Contains x || x.StartsWith "ADR-ING-"
    let reconciliation =
        i.Reconciliation
        |> List.collect (fun r ->
            ING.Matches(r.["resolution"]) |> Seq.map (fun m -> m.Value) |> Seq.filter (resolvable >> not)
            |> Seq.map (sprintf "I8 reconciliation topic %s cites unknown '%s'" r.["topic_no"]) |> List.ofSeq)
    let review =
        i.Review
        |> List.collect (fun r ->
            (if List.contains r.["disposition"] [ "accepted"; "partially-accepted"; "rejected" ] then [] else [ sprintf "I8 review %s disposition '%s'" r.["finding_id"] r.["disposition"] ])
            @ (if r.["resulting_change"].Trim() = "" then [ sprintf "I8 review %s has no recorded change or reason" r.["finding_id"] ] else [])
            @ (ING.Matches(r.["resulting_change"]) |> Seq.map (fun m -> m.Value) |> Seq.filter (resolvable >> not)
               |> Seq.map (sprintf "I8 review %s cites unknown '%s'" r.["finding_id"]) |> List.ofSeq))
    decisions @ formats @ reconciliation @ review
    @ (duplicates (i.Review |> List.map (fun r -> r.["finding_id"])) |> List.map (sprintf "I8 duplicate review finding '%s'"))
    @ (duplicates (i.Formats |> List.map (fun f -> f.["format"])) |> List.map (sprintf "I8 duplicate format '%s'"))

let all : (string * (Inputs -> Issue list)) list =
    [ "I1", i1; "I2", i2; "I3", i3; "I4", i4; "I5", i5; "I6", i6; "I7", i7; "I8", i8 ]

let run (i: Inputs) : (string * Issue list) list = all |> List.map (fun (code, gate) -> code, gate i)

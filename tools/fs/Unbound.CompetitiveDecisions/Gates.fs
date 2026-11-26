/// Quality gates for the competitive-displacement decisions package. Every gate is a pure function from the loaded
/// inputs to the issues it finds; an empty list means the gate passes.
///
///   A1  identifiers: UNB-CDD-(CIV|ONB|CMF|OEM)-nnn, OD-CDD-nn; exactly XRI-001..008 and DEC-001..012; unique
///   A2  requirements carry every mandated field; a refinement lists its parent in refines
///   A3  every cited requirement, clause or decision id resolves (baseline, sibling packages, owner decisions,
///       discovery decision registers, this package)
///   A4  status ladder: requirements are proposed / not implemented / not validated; Given/When/Then; P0-P2; known
///       release scope; approved decisions DEC-001..012; evidence gates V-1..V-9
///   A5  approved decisions stay at 'Strategically approved' dated 2026-11-26, with conditions, an effect and a
///       governing-text statement; no later ladder status is claimed anywhere in the package data
///   A6  implications: allowed disposition; evidence, ids, Given/When/Then acceptance and validation present;
///       disposition consistent with requirements created, refined and decisions raised; every created requirement
///       cites its implication
///   A7  owner decisions: open, owner, recommendation, governing text; every OD-CDD reference resolves; every raised
///       decision is traced from an approved decision
///   A8  research (only with --research-root): cited paths exist; claim/source/challenge/hypothesis/question/decision
///       ids exist in the registers; each implication's research_row equals the register row
///   A9  constraint checks: every architecture constraint states 'no change'; the Echelon Foundry check is present
///   A10 coverage rows use the closed vocabulary and every new, refined or investigation row cites this package
module Unbound.CompetitiveDecisions.Gates

open System.IO
open System.Text.RegularExpressions
open Unbound.CompetitiveDecisions.Json
open Unbound.CompetitiveDecisions.Data

type Issue = string

let private rx (p: string) = Regex(p, RegexOptions.CultureInvariant)
let private matches (r: Regex) (s: string) = r.IsMatch s

let REQ_ID = rx @"^UNB-CDD-(CIV|ONB|CMF|OEM)-\d{3}$"
let OD_ID = rx @"^OD-CDD-\d{2}$"
let OD_REF = rx @"\bOD-CDD-\d{2}\b"
let GWT = rx @"(?s)\bGiven\b.*\bWhen\b.*\bThen\b"
let XRIS = [ 1..8 ] |> List.map (sprintf "XRI-%03d")
let DECS = [ 1..12 ] |> List.map (sprintf "DEC-%03d")
let VGATES = [ 1..9 ] |> List.map (sprintf "V-%d")

let SCOPES =
    set [ "initial-implementation"; "sponsor-activation"; "commercial-availability"; "architectural-support"
          "regulatory-authorization"; "deferred" ]

let DISPOSITIONS =
    set [ "Already satisfied"; "Requirements updated"; "New requirement added"; "Existing decision superseded"
          "Research-only"; "Deferred behind an explicit gate"; "Blocked by unavailable information"
          "Requires a new owner decision" ]

/// Ladder statuses after 'Strategically approved' and 'Requirements incorporated'; none may be claimed here.
let LATER_LADDER = [ "Evidence gate satisfied"; "Implementation authorized"; "Implementation authorised"; "Implemented"; "Validated" ]

let REQUIRED =
    [ "id"; "origin"; "title"; "statement"; "rationale"; "supporting_research"; "research_ids"; "approved_decisions"
      "acceptance"; "security_implications"; "failure_behavior"; "verification"; "refines"; "priority"; "status"
      "implementation_status"; "validation_status"; "release_scope" ]

let duplicates (xs: string list) : string list =
    xs |> List.countBy id |> List.filter (fun (_, n) -> n > 1) |> List.map fst |> List.sort

let requirementIds (i: Inputs) = i.Requirements |> List.map (fs "id")
let decisionIds (i: Inputs) = i.Decisions |> List.map (col "decision_id")

/// Every id the package may cite.
let known (i: Inputs) : Set<string> =
    Set.unionMany [ i.ExternalIds; Set.ofList (requirementIds i); Set.ofList (decisionIds i) ]

let private exactly (code: string) (what: string) (expected: string list) (actual: string list) : Issue list =
    let missing = expected |> List.filter (fun e -> not (List.contains e actual))
    let extra = actual |> List.filter (fun a -> not (List.contains a expected))
    (missing |> List.map (sprintf "%s missing %s %s" code what)) @ (extra |> List.map (sprintf "%s unexpected %s %s" code what))

let a1 (i: Inputs) : Issue list =
    let reqs = requirementIds i
    let ods = decisionIds i
    (reqs |> List.filter (matches REQ_ID >> not) |> List.map (sprintf "A1 malformed requirement id '%s'"))
    @ (ods |> List.filter (matches OD_ID >> not) |> List.map (sprintf "A1 malformed decision id '%s'"))
    @ (duplicates (reqs @ ods) |> List.map (sprintf "A1 duplicate id '%s'"))
    @ exactly "A1" "implication" XRIS (i.Implications |> List.map (fs "id"))
    @ (duplicates (i.Implications |> List.map (fs "id")) |> List.map (sprintf "A1 duplicate implication '%s'"))
    @ exactly "A1" "approved decision" DECS (i.Approved |> List.map (col "decision_id"))
    @ (duplicates (i.Approved |> List.map (col "decision_id")) |> List.map (sprintf "A1 duplicate approved decision '%s'"))

let a2 (i: Inputs) : Issue list =
    i.Requirements
    |> List.collect (fun r ->
        let rid = fs "id" r
        let missing = REQUIRED |> List.filter (fun f -> not (nonEmpty (field f r))) |> List.map (sprintf "A2 %s missing or empty field %s" rid)
        let parent = fs "parent" r
        let origin =
            match parent, fs "origin" r with
            | "", "gap-analysis" -> []
            | "", o -> [ $"A2 {rid} has no parent but origin is '{o}'" ]
            | p, _ when not (List.contains p (strs (field "refines" r))) -> [ $"A2 {rid} parent {p} not listed in refines" ]
            | _ -> []
        missing @ origin)

/// (owner, cited id) pairs across the package.
let citations (i: Inputs) : (string * string) list =
    let fromReqs =
        i.Requirements
        |> List.collect (fun r ->
            let rid = fs "id" r
            ((if fs "parent" r = "" then [] else [ fs "parent" r ]) @ strs (field "refines" r) @ strs (field "dependencies" r))
            |> List.map (fun c -> rid, c))
    let fromImplications =
        i.Implications
        |> List.collect (fun x ->
            [ "affected_existing"; "addressed_by"; "requirements_created"; "requirements_refined"; "owner_decisions" ]
            |> List.collect (fun f -> strs (field f x))
            |> List.map (fun c -> fs "id" x, c))
    let fromCsv (rows: Csv) (key: string) (columns: string list) =
        rows |> List.collect (fun r -> columns |> List.collect (fun c -> ids (col c r)) |> List.map (fun c -> col key r, c))
    fromReqs
    @ fromImplications
    @ fromCsv i.Approved "decision_id" [ "requirement_ids"; "ratified_text_touched"; "owner_decisions_raised"; "related_open_decisions" ]
    @ fromCsv i.Decisions "decision_id" [ "affected_ids" ]
    @ fromCsv i.Coverage "capability" [ "requirement_ids" ]
    @ fromCsv i.Constraints "constraint" [ "governing_ids" ]
    @ fromCsv i.Gates "gate" [ "unbound_requirements" ]

let a3 (i: Inputs) : Issue list =
    let k = known i
    citations i |> List.filter (fun (_, c) -> not (k.Contains c)) |> List.map (fun (o, c) -> $"A3 {o} cites unknown id {c}")

let a4 (i: Inputs) : Issue list =
    let checks: (string * (J -> bool)) list =
        [ "acceptance is not Given/When/Then", (fun r -> matches GWT (fs "acceptance" r))
          "priority not P0-P2", (fun r -> List.contains (fs "priority" r) [ "P0"; "P1"; "P2" ])
          "status must be 'proposed'", (fun r -> fs "status" r = "proposed")
          "implementation_status must be 'not implemented'", (fun r -> fs "implementation_status" r = "not implemented")
          "validation_status must be 'not validated'", (fun r -> fs "validation_status" r = "not validated")
          "unknown release scope", (fun r -> SCOPES.Contains(fs "release_scope" r))
          "approved_decisions outside DEC-001..012", (fun r -> strs (field "approved_decisions" r) |> List.forall (fun d -> List.contains d DECS))
          "evidence_gates outside V-1..V-9", (fun r -> strs (field "evidence_gates" r) |> List.forall (fun g -> List.contains g VGATES)) ]
    i.Requirements |> List.collect (fun r -> checks |> List.filter (fun (_, ok) -> not (ok r)) |> List.map (fun (l, _) -> $"""A4 {fs "id" r} {l}"""))

/// Every text value in the package data, with its owner, for ladder scans.
let private statusTexts (i: Inputs) : (string * string) list =
    (i.Approved |> List.collect (fun r -> [ col "decision_id" r, col "unbound_status" r; col "decision_id" r, col "requirements_stage" r ]))
    @ (i.Requirements |> List.collect (fun r -> [ "status"; "implementation_status"; "validation_status" ] |> List.map (fun f -> fs "id" r, fs f r)))
    @ (i.Gates |> List.map (fun g -> col "gate" g, col "status" g))

let a5 (i: Inputs) : Issue list =
    let rowIssues =
        i.Approved
        |> List.collect (fun r ->
            let d = col "decision_id" r
            [ (col "unbound_status" r <> "Strategically approved"), $"""A5 {d} status must be 'Strategically approved' (got '{col "unbound_status" r}')"""
              (col "approval_date" r <> "2026-11-26"), $"A5 {d} approval date must be 2026-11-26"
              (col "conditions" r = ""), $"A5 {d} has no conditions"
              (col "unbound_effect" r = ""), $"A5 {d} has no Unbound effect"
              (col "governing_text_now" r = ""), $"A5 {d} does not state which text governs now"
              ((col "requirements_stage" r).StartsWith "Requirements incorporated"), $"A5 {d} claims 'Requirements incorporated' before ratification"
              (ids (col "preserved_gates" r) |> List.exists (fun g -> not (List.contains g VGATES))), $"A5 {d} preserved gate outside V-1..V-9" ]
            |> List.filter fst
            |> List.map snd)
    let ladder =
        statusTexts i
        |> List.collect (fun (owner, text) ->
            LATER_LADDER
            |> List.filter (fun s -> Regex.IsMatch(text, @"^\s*" + Regex.Escape s + @"\b", RegexOptions.IgnoreCase))
            |> List.map (fun s -> $"A5 {owner} claims ladder status '{s}'"))
    let gateStatus =
        i.Gates |> List.filter (fun g -> col "status" g <> "not run") |> List.map (fun g -> $"""A5 {col "gate" g} status must be 'not run'""")
    rowIssues @ ladder @ gateStatus @ exactly "A5" "validation gate" VGATES (i.Gates |> List.map (col "gate"))

let a6 (i: Inputs) : Issue list =
    let own = Set.ofList (requirementIds i)
    let byId = i.Requirements |> List.map (fun r -> fs "id" r, r) |> Map.ofList
    i.Implications
    |> List.collect (fun x ->
        let xid = fs "id" x
        let disp = fs "disposition" x
        let created = strs (field "requirements_created" x)
        let refined = strs (field "requirements_refined" x)
        let ods = strs (field "owner_decisions" x)
        let present =
            [ "affected_area"; "research_evidence"; "research_ids"; "affected_existing"; "already_addressed"; "conflicts"
              "duplication"; "gaps"; "disposition_rationale"; "approved_decisions"; "acceptance"; "validation"; "progression" ]
            |> List.filter (fun f -> not (nonEmpty (field f x)))
            |> List.map (sprintf "A6 %s missing or empty field %s" xid)
        let rules =
            [ not (DISPOSITIONS.Contains disp), $"A6 {xid} disposition '{disp}' is not an allowed class"
              (fs "target_repository" (field "research_row" x) <> "kemiller2002/unbound"), $"A6 {xid} research row does not target kemiller2002/unbound"
              (strs (field "acceptance" x) |> List.exists (matches GWT >> not)), $"A6 {xid} acceptance is not Given/When/Then"
              (disp = "New requirement added" && created.IsEmpty), $"A6 {xid} 'New requirement added' creates no requirement"
              (disp = "Requirements updated" && refined.IsEmpty), $"A6 {xid} 'Requirements updated' refines nothing"
              (disp = "Requires a new owner decision" && ods.IsEmpty), $"A6 {xid} 'Requires a new owner decision' raises none"
              (disp = "Already satisfied" && (not created.IsEmpty || strs (field "addressed_by" x) |> List.isEmpty)),
              $"A6 {xid} 'Already satisfied' must create nothing and name the satisfying records"
              (strs (field "approved_decisions" x) |> List.exists (fun d -> not (List.contains d DECS))), $"A6 {xid} cites an unknown approved decision" ]
            |> List.filter fst
            |> List.map snd
        let foreign = created |> List.filter (own.Contains >> not) |> List.map (sprintf "A6 %s creates %s, which is not in this package" xid)
        let untraced =
            created
            |> List.filter own.Contains
            |> List.filter (fun c -> not (List.contains xid (strs (field "research_ids" byId.[c]))))
            |> List.map (fun c -> $"A6 {c} is created by {xid} but does not cite it in research_ids")
        present @ rules @ foreign @ untraced)

let a7 (i: Inputs) : Issue list =
    let own = Set.ofList (decisionIds i)
    let weak =
        i.Decisions
        |> List.filter (fun d -> col "status" d <> "open" || col "decision_owner" d = "" || col "recommendation" d = "" || col "governing_text_now" d = "")
        |> List.map (fun d -> $"""A7 {col "decision_id" d} must be open with owner, recommendation and governing text""")
    let badDec =
        i.Decisions
        |> List.filter (fun d -> ids (col "approved_decisions" d) |> List.exists (fun x -> not (List.contains x DECS)))
        |> List.map (fun d -> $"""A7 {col "decision_id" d} cites an unknown approved decision""")
    let texts =
        (i.Requirements |> List.map (fun r -> fs "id" r, String.concat " " (strs (field "open_questions" r) @ strs (field "conflicts" r))))
        @ (i.Implications |> List.map (fun x -> fs "id" x, String.concat " " (strs (field "owner_decisions" x) @ [ fs "conflicts" x ])))
        @ (i.Approved |> List.map (fun r -> col "decision_id" r, col "owner_decisions_raised" r + " " + col "reconciliation" r))
    let dangling =
        texts
        |> List.collect (fun (o, t) -> [ for m in OD_REF.Matches t -> m.Value ] |> List.filter (own.Contains >> not) |> List.map (fun od -> $"A7 {o} cites unknown decision {od}"))
    let raised = i.Approved |> List.collect (fun r -> ids (col "owner_decisions_raised" r)) |> Set.ofList
    let untraced = own |> Set.filter (raised.Contains >> not) |> Set.toList |> List.map (sprintf "A7 %s is not raised by any approved decision")
    weak @ badDec @ dangling @ untraced

let a8 (i: Inputs) : Issue list =
    match i.Research with
    | None -> []
    | Some res ->
        let paths =
            (i.Requirements |> List.collect (fun r -> strs (field "supporting_research" r) |> List.map (fun p -> fs "id" r, p)))
            @ (i.Implications |> List.collect (fun x -> strs (field "research_evidence" x) |> List.map (fun p -> fs "id" x, p)))
        let missingPaths =
            paths
            |> List.filter (fun (_, p) -> not (p.StartsWith RESEARCH_PREFIX) || not (File.Exists(Path.Combine(res.Root, p.Substring RESEARCH_PREFIX.Length))))
            |> List.map (fun (o, p) -> $"A8 {o} cites missing research {p}")
        let researchIds =
            (i.Requirements |> List.collect (fun r -> strs (field "research_ids" r) |> List.map (fun c -> fs "id" r, c)))
            @ (i.Implications |> List.collect (fun x -> strs (field "research_ids" x) |> List.map (fun c -> fs "id" x, c)))
        let missingIds =
            researchIds |> List.filter (fun (_, c) -> not (res.RegisterIds.Contains c)) |> List.map (fun (o, c) -> $"A8 {o} cites {c}, absent from the research registers")
        let rows = res.Implications |> List.map (fun r -> col "implication_id" r, r) |> Map.ofList
        let drift =
            i.Implications
            |> List.collect (fun x ->
                let xid = fs "id" x
                match rows.TryFind xid with
                | None -> [ $"A8 {xid} not in cross-repository-implications.csv" ]
                | Some row ->
                    match field "research_row" x with
                    | JObj ps -> ps |> List.filter (fun (k, v) -> col k row <> str v) |> List.map (fun (k, _) -> $"A8 {xid} research_row.{k} differs from the register")
                    | _ -> [ $"A8 {xid} has no research_row" ])
        let decs = res.Decisions |> List.map (col "decision_id") |> Set.ofList
        let missingDecs = DECS |> List.filter (decs.Contains >> not) |> List.map (sprintf "A8 %s absent from the research decision register")
        missingPaths @ missingIds @ drift @ missingDecs

let a9 (i: Inputs) : Issue list =
    let changed =
        i.Constraints |> List.filter (fun c -> col "package_effect" c <> "no change") |> List.map (fun c -> $"""A9 constraint '{col "constraint" c}' must state 'no change'""")
    let unsupported =
        i.Constraints |> List.filter (fun c -> col "governing_ids" c = "" || col "evidence" c = "") |> List.map (fun c -> $"""A9 constraint '{col "constraint" c}' lacks governing ids or evidence""")
    let foundry =
        if i.Constraints |> List.exists (fun c -> (col "constraint" c).Contains "Echelon Foundry") then []
        else [ "A9 the Echelon Foundry sponsor/administrator check is missing" ]
    changed @ unsupported @ foundry

let a10 (i: Inputs) : Issue list =
    let own = Set.ofList (requirementIds i)
    i.Coverage
    |> List.collect (fun c ->
        let cov = col "coverage" c
        let cap = col "capability" c
        let vocabulary = if List.contains cov [ "existing"; "refined"; "new"; "investigation" ] then [] else [ $"A10 '{cap}' coverage '{cov}' unknown" ]
        let cited =
            if cov <> "existing" && not (ids (col "requirement_ids" c) |> List.exists own.Contains) then [ $"A10 '{cap}' is {cov} but cites no requirement of this package" ]
            else []
        vocabulary @ cited)

let all: (string * (Inputs -> Issue list)) list =
    [ "A1", a1; "A2", a2; "A3", a3; "A4", a4; "A5", a5; "A6", a6; "A7", a7; "A8", a8; "A9", a9; "A10", a10 ]

let run (i: Inputs) : (string * Issue list) list = all |> List.map (fun (code, gate) -> code, gate i)

/// Adopt the master-mission convergence clauses into the requirements baseline (port of
/// tools/reqaudit/adopt_convergence.py). Clauses, amendments, numeric parameters and control links are JSON data
/// under requirements/convergence/2026-11-24-master/clauses/, in the order named by clauses/manifest.json.
module Unbound.Convergence.AdoptConvergence

open System.Text.RegularExpressions
open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat
module QL = Unbound.Convergence.QuestionsLib

let MISSION = "requirements/convergence/2026-11-24-master"
let TAG = "convergence-2026-11-24"
let BEGIN, END = $"<!-- BEGIN ADOPTED: {TAG} -->", $"<!-- END ADOPTED: {TAG} -->"
let NOTE = "Converged 2026-11-24"
let ON = "2026-11-24"

let ACTOR =
    "Master mission agent (convergence; product-owner directions and mission-adopted evidence, owner ratification pending where agent-recommended)"

let CONTROL_MATRIX = "requirements/governance/MASTER-ENTERPRISE-CONTROL-MATRIX.csv"
let CROSSWALK = "requirements/governance/CONVERGENCE-CLAUSE-TO-CONTROL-CROSSWALK.csv"

let RECORD_FILES =
    [ "requirements/reconciliation/2026-11-23/data/refinements.jsonl"
      "requirements/reconciliation/2026-11-23/data/gap-requirements.jsonl"
      "requirements/discovery/2026-11-23-cross-industry/data/gap-requirements.jsonl"
      "requirements/discovery/2026-11-23-cross-industry/data/refinements.jsonl"
      "requirements/discovery/2026-11-24-simplification-economics/data/refinements.jsonl"
      "requirements/discovery/2026-11-24-simplification-economics/data/gap-requirements.jsonl"
      // Owner-ratified 2026-11-27 (OD-M-16; PR #22 direction): the 30 UNB-CDD requirements.
      "requirements/discovery/2026-11-26-competitive-displacement-decisions/data/gap-requirements.jsonl" ]

let private s (x: string) = JStr x
let private valueError (msg: string) = raise (PyError("ValueError", msg))

/// The clause data as stored: module name -> parsed JSON file.
type ClauseData =
    { Manifest: Json
      Modules: Map<string, Json> }

let clausesDir = MISSION + "/clauses"

/// Reads manifest.json, common.json and every module the manifest names.
let readClauseData (root: string) : ClauseData =
    let read (name: string) = loads (Io.readText (Io.path root $"{clausesDir}/{name}.json"))
    let manifest = read "manifest"
    let names =
        strs (item "clause_modules" manifest) @ strs (item "amendment_modules" manifest)
        @ [ itemStr "numeric_parameters_module" manifest; itemStr "control_links_module" manifest; "common" ]
    { Manifest = manifest; Modules = names |> List.map (fun n -> n, read n) |> Map.ofList }

let private moduleValue (data: ClauseData) (name: string) (key: string) : Json = item key data.Modules.[name]

/// The assertions common.clause() made when the clauses were Python modules.
let checkClause (common: Json) (c: Json) : unit =
    let assertIn (k: string) (allowed: Json) =
        let v = item k c
        let ok = match allowed with JObj kvs -> kvs |> List.exists (fun (key, _) -> JStr key = v) | JArr xs -> List.contains v xs | _ -> false
        if not ok then raise (PyError("AssertionError", str v))
    assertIn "domain" (item "DOMAINS" common)
    assertIn "release_scope" (item "RELEASE_SCOPES" common)
    assertIn "classification" (item "CLASSIFICATIONS" common)
    if not (contains "SHALL" (itemStr "statement" c)) && itemStr "classification" c = "normative" then
        raise (PyError("AssertionError", itemStr "id" c))
    if not (truthy (item "acceptance_criteria" c)) then raise (PyError("AssertionError", itemStr "id" c))

let private listOf (k: string) (j: Json) : Json list = getOr k (JArr []) j |> asList

/// Apply one amendment to a clause (pure); fails if a replacement target is missing.
let amend (c: Json) (a: Json) : Json =
    let statement = itemStr "statement" c
    let acceptance = strs (item "acceptance_criteria" c)
    let texts = statement :: acceptance
    let replacements = listOf "replace" a |> List.map (fun p -> match asList p with [ o; n ] -> str o, str n | _ -> failwith "replace pair")
    match replacements |> List.filter (fun (o, _) -> not (texts |> List.exists (contains o))) with
    | (o, _) :: _ -> valueError $"""{itemStr "id" c}: amendment target not found: {cpTake 80 o}"""
    | [] ->
        let sub (t: string) = replacements |> List.fold (fun x (o, n) -> replace o n x) t
        let stmt = sub statement
        let stmt = if truthyOpt (get "append" a) then stmt + " " + itemStr "append" a else stmt
        let finding = itemStr "finding" a
        let source =
            if contains "owner decision" finding then "owner decision"
            elif startsWith "EVB-" finding then "evidence backlog (report 13)"
            elif contains "ADV-R2" finding then "report 08 round 2"
            else "report 08 adversarial review"
        let entry = JObj [ "on", getOr "on" (s ON) a; "source", s source; "findings", item "finding" a; "original_statement", s statement ]
        c
        |> set "statement" (s (joinWs stmt))
        |> set "acceptance_criteria" (JArr((acceptance |> List.map (sub >> s)) @ listOf "acceptance_add" a))
        |> set "amends" (JArr(dedupe (listOf "amends" c @ listOf "amends_add" a)))
        |> set "dependencies" (JArr(dedupe (listOf "dependencies" c @ listOf "dependencies" a)))
        |> set "release_scope" (getOr "release_scope" (item "release_scope" c) a)
        |> set "revision_history" (JArr(listOf "revision_history" c @ [ entry ]))

let private W = @"[\p{L}\p{N}_]"
let private sp = @"[\s\x1c-\x1f]"
let private cnvRef = Regex(@"CNV-[A-Z]+-\d{3}")

let private unitRe =
    Regex(@"\$?\d[\d,.]*" + sp + @"?(?:%|-?(?:business|banking|calendar|working)?" + sp + @"?(?:days?|hours?|months?|years?)(?!" + W + "))")

let private reprList (xs: string list) = reprStrList xs

let loadClauses (data: ClauseData) : Json list =
    let common = data.Modules.["common"]
    let baseClauses =
        strs (item "clause_modules" data.Manifest)
        |> List.collect (fun m -> moduleValue data m "CLAUSES" |> asList |> List.map (fun c -> checkClause common c; c))
    let applySet (cs: Json list) (m: string) =
        let amendments = moduleValue data m "AMENDMENTS"
        let ids = cs |> List.map (itemStr "id") |> Set.ofList
        match keys amendments |> List.filter (fun k -> not (ids.Contains k)) |> List.distinct |> sortStrs with
        | _ :: _ as unknown -> valueError $"{m}: amendments for unknown clauses: {reprList unknown}"
        | [] -> cs |> List.map (fun c -> match get (itemStr "id" c) amendments with Some a -> amend c a | None -> c)
    let amended = strs (item "amendment_modules" data.Manifest) |> List.fold applySet baseClauses
    let ids = amended |> List.map (itemStr "id") |> Set.ofList
    let refs (c: Json) =
        cnvRef.Matches(itemStr "statement" c) |> Seq.map (fun m -> m.Value) |> List.ofSeq |> dedupe
        |> List.filter (fun r -> r <> itemStr "id" c && ids.Contains r)
    let pars = moduleValue data (itemStr "numeric_parameters_module" data.Manifest) "PARAMETERS" |> asList |> List.map asList
    let byClause =
        pars
        |> List.map (fun t -> str t.[0], JObj [ "text", t.[1]; "basis", t.[2]; "source", t.[3] ])
        |> List.fold (fun (acc: (string * Json list) list) (cid, q) ->
            if acc |> List.exists (fst >> (=) cid) then acc |> List.map (fun (k, v) -> if k = cid then k, v @ [ q ] else k, v)
            else acc @ [ cid, [ q ] ]) []
    let byClauseM = Map.ofList byClause
    let stmt = amended |> List.map (fun c -> itemStr "id" c, itemStr "statement" c) |> Map.ofList
    let stale =
        byClause
        |> List.collect (fun (cid, qs) ->
            qs
            |> List.filter (fun q -> not (contains (itemStr "text" q) (stmt.TryFind cid |> Option.defaultValue "")))
            |> List.map (fun q -> $"""{cid}: '{itemStr "text" q}'"""))
    if not stale.IsEmpty then valueError $"numeric parameters not found in clause text: {reprList stale}"
    let covered (cid: string) (m: string) =
        let token = (splitWs (stripChars " -" m)).[0] |> rstripChars ",.%"
        byClauseM.TryFind cid |> Option.defaultValue [] |> List.exists (fun q -> contains token (itemStr "text" q))
    let untagged =
        amended
        |> List.collect (fun c ->
            unitRe.Matches(itemStr "statement" c)
            |> Seq.map (fun m -> m.Value)
            |> Seq.filter (fun m -> not (covered (itemStr "id" c) m))
            |> Seq.map (fun m -> $"""{itemStr "id" c}: '{m}'""")
            |> List.ofSeq)
    if not untagged.IsEmpty then valueError $"numbers without a basis tag: {reprList untagged}"
    let links = moduleValue data (itemStr "control_links_module" data.Manifest) "CONTROL_LINKS"
    match keys links |> List.filter (fun k -> not (ids.Contains k)) |> List.distinct |> sortStrs with
    | _ :: _ as unknown -> valueError $"control links for unknown clauses: {reprList unknown}"
    | [] ->
        amended
        |> List.map (fun c ->
            let cid = itemStr "id" c
            c
            |> set "dependencies" (JArr(dedupe (listOf "dependencies" c @ (refs c |> List.map s))))
            |> set "internal_control" (
                match get cid links with
                | Some l -> l |> set "cuec_id" (s (if truthyOpt (get "cuec" l) then $"CUEC-{cid}" else ""))
                | None -> JNull
            )
            |> set "numeric_parameters" (JArr(byClauseM.TryFind cid |> Option.defaultValue [])))

/// Clause ids that sit on a dependency cycle.
let cycles (clauses: Json list) : string list =
    let deps = clauses |> List.map (fun c -> itemStr "id" c, strs (item "dependencies" c)) |> toDict
    let depM = Map.ofList deps
    let depsOf i = depM.TryFind i |> Option.defaultValue []
    let reach (start: string) =
        let rec go (frontier: string list) (seen: Set<string>) =
            let nxt = frontier |> List.collect depsOf |> List.filter (fun d -> not (seen.Contains d))
            if nxt.IsEmpty then seen else go nxt (Set.union seen (Set.ofList nxt))
        go (depsOf start) (Set.ofList (depsOf start))
    deps |> List.map fst |> List.filter (fun i -> (reach i).Contains i)

let rid (r: Json) : string = str (QL.recordId r)

let validate (clauses: Json list) (inv: Json list) (records: Set<string>) (register: Set<string>) (controlIds: Set<string>) : string list =
    let ids = clauses |> List.map (itemStr "id") |> counter
    let idSet = ids |> List.map fst |> Set.ofList
    let known = Set.union (inv |> List.map (itemStr "identifier") |> Set.ofList) records
    let each f = clauses |> List.collect f
    (ids |> List.filter (fun (_, n) -> n > 1) |> List.map (fun (i, _) -> $"duplicate clause {i}"))
    @ each (fun c -> strs (item "amends" c) |> List.filter (known.Contains >> not) |> List.map (fun a -> $"""{itemStr "id" c}: unknown amended id {a}"""))
    @ each (fun c -> strs (item "resolves" c) |> List.filter (register.Contains >> not) |> List.map (fun q -> $"""{itemStr "id" c}: unknown question {q}"""))
    @ each (fun c -> strs (item "dependencies" c) |> List.filter (idSet.Contains >> not) |> List.map (fun d -> $"""{itemStr "id" c}: unknown dependency {d}"""))
    @ (cycles clauses |> List.map (fun i -> $"dependency cycle through {i}"))
    @ each (fun c ->
        (getOr "internal_control" JNull c |> orElse (JObj []) |> listOf "controls" |> List.map str)
        |> List.filter (controlIds.Contains >> not)
        |> List.map (fun k -> $"""{itemStr "id" c}: unknown control {k}"""))

// ---------------------------------------------------------------- markdown sections

let esc (t: Json) : string = joinWs (str t)

let renderClause (c: Json) (missionRel: string) : string =
    let owners = items (item "owners" c) |> List.map (fun (k, v) -> $"{k.Replace('_', ' ')}: {str v}") |> String.concat "; "
    let joined k = strs (item k c) |> String.concat ", "
    let orDefault d x = if x = "" then d else x
    let numeric =
        match getOr "numeric_parameters" JNull c with
        | p when truthy p ->
            [ "  - Numeric parameters: "
              + (asList p |> List.map (fun q -> $"""{getStr "text" q} [{getStr "basis" q}: {getStr "source" q}]""") |> String.concat "; ")
              + "." ]
        | _ -> []
    let control =
        match getOr "internal_control" JNull c with
        | ic when truthy ic ->
            let cuec = getOr "cuec_id" JNull ic
            [ $"""  - Internal control: {strs (item "controls" ic) |> String.concat ", "}; operator {str (item "operator" ic)}; """
              + $"""service-organization report relevance {str (item "report" ic)}; {(if truthy (item "automated" ic) then "automated" else "manual or mixed")}"""
              + (if truthy cuec then $"; complementary user entity control {str cuec}" else "")
              + "." ]
        | _ -> []
    let related = strs (item "related_findings" c)
    [ $"""- **{itemStr "id" c} — {esc (item "title" c)}:** {esc (item "statement" c)}"""
      $"""  - Provenance: resolves {joined "resolves" |> orDefault "Phase 3 feedback"}; amends {joined "amends" |> orDefault "—"}"""
      + (if related.IsEmpty then "" else $"""; related findings {String.concat ", " related}""")
      + $". Evidence and reasoning: [{missionRel}/02-EVIDENCE-DOSSIERS.md]({missionRel}/02-EVIDENCE-DOSSIERS.md), "
      + $"[{missionRel}/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md]({missionRel}/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md)."
      $"""  - Classification: {str (item "classification" c)}; release scope: {str (item "release_scope" c)}; operating models: {joined "operating_models"}."""
      $"""  - Activation gate: {esc (item "activation_gate" c) |> rstripChars "." |> orDefault "none beyond release scope"}."""
      $"  - Ownership: {owners}." ]
    @ numeric
    @ control
    @ [ "  - Acceptance:" ]
    @ (strs (item "acceptance_criteria" c) |> List.mapi (fun i a -> $"    - AC-{i + 1}: {joinWs a}"))
    |> String.concat "\n"

let section (clauses: Json list) (target: string) : string =
    let missionRel = relpath MISSION (parent target)
    let body = clauses |> List.map (fun c -> renderClause c missionRel) |> String.concat "\n"
    [ BEGIN
      "## Converged requirements: master mission (2026-11-24)"
      ""
      "Status: adopted into the baseline by the master mission. Clauses resolving product-owner-approved directions are "
      + "binding for design; clauses resting on agent-recommended evidence answers are binding for design as conservative "
      + "positions pending owner ratification. No clause is a legal, accounting, bank or auditor approval: each activation "
      + "gate names the authorization still required before production use. Where a clause conflicts with an earlier "
      + "requirement in this file, the clause governs and the earlier requirement is annotated."
      ""
      body
      END ]
    |> String.concat "\n"

let private sectionRe = Regex(Regex.Escape BEGIN + ".*?" + Regex.Escape END, RegexOptions.Singleline)

let putSection (text: string) (sec: string) : string =
    if contains BEGIN text then sectionRe.Replace(text, MatchEvaluator(fun _ -> sec))
    else (text.TrimEnd('\n')) + "\n\n" + sec + "\n"

/// Insert note after line_no (1-based) unless the following line already carries a convergence note.
let annotate (lines: string list) (lineNo: int) (note: string) : string list =
    let i = lineNo
    let nxt = if i < lines.Length then lines.[i] else ""
    if startsWith $"> {NOTE}" nxt then List.take i lines @ [ note ] @ List.skip (i + 1) lines
    else List.take i lines @ [ note ] @ List.skip i lines

// ---------------------------------------------------------------- records and events

/// amended requirement id -> converging clause ids (clause order, once per clause).
let convergedBy (clauses: Json list) : Map<string, string list> =
    clauses
    |> List.fold
        (fun (acc: Map<string, string list>) c ->
            strs (item "amends" c) |> List.distinct
            |> List.fold (fun (m: Map<string, string list>) a -> m.Add(a, (m.TryFind a |> Option.defaultValue []) @ [ itemStr "id" c ])) acc)
        Map.empty

let recordUpdates (clauses: Json list) (recordsByFile: (string * Json list) list) : (string * Json list) list =
    let byId = convergedBy clauses
    let upd (r: Json) =
        match byId.TryFind(rid r) |> Option.defaultValue [] |> List.distinct |> sortStrs with
        | [] -> r
        | cs ->
            let hist = listOf "revision_history" r |> List.filter (fun h -> get "source" h <> Some(s TAG))
            r
            |> set "converged_by" (jstrs cs)
            |> set "revision_history" (
                JArr(
                    hist
                    @ [ JObj
                            [ "on", s ON; "source", s TAG; "change", s "Converged clauses govern where they conflict; statement text unchanged"
                              "clauses", jstrs cs; "provenance", s (MISSION + "/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md") ] ]
                )
            )
    recordsByFile |> List.map (fun (f, rs) -> f, rs |> List.map upd)

let DEFAULT_DISPOSITION = Map.ofList [ "evidence-backlog", "empirical-validation-required" ]

let appliedEvents (clauses: Json list) (states: QL.States) (existing: Json list) : Json list =
    let byQ =
        clauses
        |> List.fold
            (fun (acc: (string * Json list) list) c ->
                strs (item "resolves" c) |> List.distinct
                |> List.fold (fun acc q ->
                    if acc |> List.exists (fst >> (=) q) then acc |> List.map (fun (k, v) -> if k = q then k, v @ [ c ] else k, v)
                    else acc @ [ q, [ c ] ]) acc)
            []
    let seen =
        existing
        |> List.filter (fun e -> item "type" e = s "applied-to-requirement")
        |> List.map (fun e -> itemStr "question_id" e, getOr "set" (JObj []) e |> getOr "applied_to" (JArr []) |> asList)
        |> Set.ofList
    let event (q: string) (cs: Json list) =
        let st = QL.stateOf q states
        let disp =
            getOr "disposition" JNull st
            |> orElse (
                cs
                |> List.tryPick (fun c -> DEFAULT_DISPOSITION.TryFind(itemStr "classification" c))
                |> Option.defaultValue (if get "review_class" st = Some(s "Validation") then "empirical-validation-required" else "business-policy-decision")
                |> s
            )
        let gate = cs |> List.map (item "activation_gate") |> List.filter truthy |> dedupe |> List.map str |> String.concat "; "
        let approval =
            match get "approval_status" st with
            | Some(JStr a) when a = "approved-product-direction" || a = "approved-owner" -> s a
            | _ -> s "agent-recommended"
        let superseded = get "resolution_status" st = Some(s "superseded-by-duplicate")
        let ext = getOr "external_authorization_required" JNull st |> orElse (s (if disp = s "external-authorization-required" then gate else ""))
        q,
        JObj
            [ "applied_to", JArr(cs |> List.map (item "id"))
              "disposition", disp
              "resolution_status", s (if superseded then "superseded-by-duplicate" else "resolved")
              "approval_status", approval
              "activation_gate", (getOr "activation_gate" JNull st |> orElse (s gate))
              "external_authorization_required", ext ]
    let cands = byQ |> sortByStrs (fun (q, _) -> [ q ]) |> List.map (fun (q, cs) -> event q cs)
    let fresh = cands |> List.filter (fun (q, st) -> not (seen.Contains(q, asList (item "applied_to" st))))
    let start = existing |> List.map (fun e -> int (splitOn "-" (itemStr "event_id" e)).[1]) |> List.fold max 0
    fresh
    |> List.mapi (fun i (q, st) ->
        JObj
            [ "event_id", s ("QE-" + pad6 (start + i + 1)); "question_id", s q; "on", s ON; "type", s "applied-to-requirement"
              "artifact", s (MISSION + "/03-AUTHORITATIVE-BASELINE-AND-CONVERGENCE.md"); "actor", s ACTOR; "set", st ])

// ---------------------------------------------------------------- baseline index

let OWNER_BY_PREFIX =
    [ "UNB-XIN-ACC", "Finance"; "UNB-ACC", "Finance"; "UNB-FIN", "Finance"; "UNB-NAV", "Investments"; "UNB-INV", "Investments"
      "UNB-XIN-INV", "Investments"; "UNB-XIN-PAY", "Payments"; "UNB-XIN-BNK", "Payments"; "UNB-PAY", "Payments"; "UNB-GRT", "Grants"
      "UNB-GRANT", "Grants"; "UNB-XIN-NPG", "Grants"; "UNB-CHAR", "Charity knowledge"; "UNB-EXT", "Charity knowledge"
      "UNB-KNOW", "Charity knowledge"; "UNB-REG", "Regulatory"; "UNB-XIN-REG", "Regulatory"; "UNB-LEG", "Regulatory"
      "UNB-SOC2", "Assurance"; "UNB-CTRL", "Assurance"; "UNB-GOV", "Governance"; "UNB-ADM", "Administration"
      "UNB-SUCC", "Accounts and succession"; "UNB-XIN-TRS", "Accounts and succession"; "UNB-TEN", "Tenancy"; "UNB-PART", "Tenancy"
      "UNB-XIN-SAS", "Tenancy"; "UNB-API", "Platform"; "UNB-DATA", "Platform"; "UNB-RES", "Platform"; "UNB-AIO", "AI"; "UNB-AI", "AI"
      "UNB-EXP", "Experience"; "UNB-UI", "Experience"; "UNB-XIN-ECO", "Economics"; "UNB-ECO", "Economics"; "UNB-XIN-SVC", "Service"
      "UNB-XIN-CLM", "Service"; "UNB-XIN-CRM", "Communications"; "UNB-SMP", "Operating model"
      "UNB-CDD-CIV", "Charity knowledge"; "UNB-CDD-ONB", "Administration"; "UNB-CDD-CMF", "Operating model"; "UNB-CDD-OEM", "Economics" ]

let ownerOf (i: string) : string =
    OWNER_BY_PREFIX |> List.tryFind (fun (p, _) -> startsWith p i) |> Option.map snd |> Option.defaultValue "Product (route by domain)"

let private capabilityIndex = Regex(@"\AREQ-X-0(5[89]|6\d|7[0-7])\n?\z")

let classify (i: string) (text: Json) : string =
    let t = lower (str (text |> orElse (s "")))
    if [ "UNB-HYP"; "UNB-GAP"; "UNB-COM" ] |> List.exists (fun p -> startsWith p i) then "research-hypothesis"
    elif capabilityIndex.IsMatch i then "capability-index-candidate (SPD-ENT-001; normative until carrier coverage verified)"
    elif contains "hypothesis" (cpTake 80 t) then "research-hypothesis"
    else "normative"

let textSha (t: Json) : string = sha256Hex (str (t |> orElse (s "")))

let private defaultScope = s "unassigned (default initial-implementation unless a clause defers)"

let baseline
    (clauses: Json list)
    (inv: Json list)
    (records: (string * (Json * string)) list)
    (adoption: Json list)
    (release: Map<Json, Json>)
    (states: QL.States)
    (ratified: Map<string, Json>)
    : Json list =
    let conv = convergedBy clauses
    let convOf i = jstrs (conv.TryFind i |> Option.defaultValue [])
    let adopted = adoption |> List.map (fun a -> str (item "original_id" a), a) |> Map.ofList
    let recordStatus = adoption |> List.map (fun a -> str (item "record_id" a), item "status" a) |> Map.ofList
    let reqs = inv |> List.filter (fun r -> item "kind" r = s "requirement") |> List.map (fun r -> itemStr "identifier" r, r) |> toDict
    let scopeOf i = release.TryFind(s i) |> Option.defaultValue defaultScope
    /// A record that states its own release scope keeps it unless the release-1 scope list names it.
    let recordScopeOf i (r: Json) = release.TryFind(s i) |> Option.orElse (get "release_scope" r) |> Option.defaultValue defaultScope
    let rowReq (i: string) (r: Json) =
        let status =
            match adopted.TryFind i with
            | Some a -> "refined by " + itemStr "record_id" a + $""" ({str (item "status" a)})"""
            | None -> "original"
        JObj
            [ "id", s i; "kind", s "requirement"; "source", s $"""{str (item "path" r)}:{str (item "line" r)}"""; "text", s (esc (item "text" r))
              "text_sha256", s (textSha (item "text" r)); "status", s status; "converged_by", convOf i
              "classification", s (classify i (item "text" r)); "domain_owner", s (ownerOf i); "release_scope", scopeOf i ]
    let rowRec (i: string) (r: Json) (f: string) =
        let st =
            (recordStatus.TryFind i |> Option.defaultValue JNull)
            |> orElse (s (if contains "cross-industry" f && contains "gap" f then "adopted (cross-industry-discovery section)" else ""))
            |> orElse (getOr "approval_status" JNull r)
            |> orElse (getOr "validation_status" JNull r)
            |> orElse (s "recorded")
        let stmt = getOr "statement" (s "") r
        JObj
            [ "id", s i; "kind", s "record"; "source", s f; "text", s (esc stmt); "text_sha256", s (textSha stmt); "status", st
              "converged_by", convOf i; "classification", s (classify i stmt); "domain_owner", s (ownerOf i); "release_scope", recordScopeOf i r ]
    let rows =
        (reqs |> sortByStrs (fun (i, _) -> [ i ]) |> List.map (fun (i, r) -> rowReq i r))
        @ (records |> sortByStrs (fun (i, _) -> [ i ]) |> List.map (fun (i, (r, f)) -> rowRec i r f))
    let basis (c: Json) =
        let resolves = strs (item "resolves" c)
        let appr = resolves |> List.map (fun q -> QL.stateOf q states |> get "approval_status") |> Set.ofList
        let cid = itemStr "id" c
        match ratified.TryFind cid with
        | Some d -> $"owner-ratified ({str d})"
        | None when resolves.IsEmpty -> "mission-adopted from adversarial review (owner ratification pending)"
        | None when appr.Contains(Some(s "agent-recommended")) -> "agent-recommended, owner ratification pending"
        | None -> if appr.Contains(Some(s "approved-owner")) then "owner-ratified (packet review)" else "owner-approved product direction"
    let cls =
        clauses
        |> List.map (fun c ->
            JObj
                [ "id", item "id" c; "kind", s "convergence-clause"; "source", item "target_file" c; "text", s (esc (item "statement" c))
                  "text_sha256", s (textSha (item "statement" c)); "approval_basis", s (basis c); "resolves", item "resolves" c
                  "dependencies", item "dependencies" c; "status", s "adopted (convergence)"; "converged_by", JArr []
                  "classification", item "classification" c; "domain_owner", item "owners" c |> item "domain_owner"
                  "release_scope", item "release_scope" c ])
    rows @ cls

let private table (h: string list) (rs: string list list) =
    [ "| " + String.concat " | " h + " |"; "|" + String.replicate h.Length "---|" ] @ (rs |> List.map (fun x -> "| " + String.concat " | " x + " |"))
    |> String.concat "\n"

let renderBaseline (rows: Json list) (clauses: Json list) : string =
    let sortedCounts (xs: string list) = counter xs |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b)
    let cnt = sortedCounts (rows |> List.map (fun r -> itemStr "kind" r))
    let st =
        sortedCounts (
            rows
            |> List.map (fun r ->
                if itemStr "kind" r <> "requirement" then (splitOn " (" (str (item "status" r))).[0]
                elif truthy (item "converged_by" r) then "converged"
                else (splitOn " " (str (item "status" r))).[0])
        )
    let cls = sortedCounts (rows |> List.map (fun r -> (splitOn " (" (str (item "classification" r))).[0]))
    let basisOf = rows |> List.filter (fun r -> itemStr "kind" r = "convergence-clause") |> List.map (fun r -> itemStr "id" r, getStr "approval_basis" r) |> Map.ofList
    let clauseRows =
        clauses
        |> List.map (fun c ->
            let amends = strs (item "amends" c)
            [ itemStr "id" c + (if truthyOpt (get "revision_history" c) then " (amended)" else "")
              esc (item "title" c)
              str (item "domain" c)
              str (item "release_scope" c)
              str (item "classification" c)
              basisOf.TryFind(itemStr "id" c) |> Option.defaultValue ""
              string (strs (item "resolves" c)).Length
              String.concat ", " (List.truncate 6 amends) + (if amends.Length > 6 then " …" else "") ])
    let converged =
        rows |> List.filter (fun r -> truthy (item "converged_by" r)) |> List.map (fun r -> [ str (item "id" r); str (item "source" r); strs (item "converged_by" r) |> String.concat ", " ])
    ([ "# Authoritative requirements baseline (index)"
       "Generated by `tools/fs/Unbound.Convergence` (`adopt-convergence`); do not edit by hand. This index is the single authoritative "
       + "entry point to the Unbound requirements baseline as of 2026-11-24. It lists every requirement identified by the "
       + "reconciliation inventory, every refinement and gap record from the three requirement missions, and every convergence "
       + "clause, with status, governing clause, classification, owner and release scope. Full rows: "
       + "[`baseline/baseline.jsonl`](baseline/baseline.jsonl)."
       "**Precedence.** (1) Convergence clauses `CNV-*` (sections `convergence-2026-11-24` in the domain files) govern where they "
       + "conflict with earlier text. (2) Adopted reconciliation and cross-industry sections refine the originals they annotate. "
       + "(3) Original requirements stand where nothing refines them. Simplification-mission records remain proposed except where a "
       + "clause adopts their resolved content. Nothing here authorizes implementation (no application code during this mission) "
       + "or production activation (see each clause's activation gate)."
       "## Counts\n\n"
       + table
           [ "Measure"; "Value" ]
           ((cnt |> List.map (fun (k, v) -> [ k; string v ]))
            @ (st |> List.map (fun (k, v) -> [ "status: " + k; string v ]))
            @ (cls |> List.map (fun (k, v) -> [ "classification: " + k; string v ])))
       "## Convergence clauses\n\n"
       + table [ "Clause"; "Title"; "Domain"; "Release scope"; "Class"; "Approval basis"; "Questions resolved"; "Amends" ] clauseRows
       "## Requirements governed by convergence clauses\n\n" + table [ "Requirement"; "Defined at"; "Clauses" ] converged ]
     |> String.concat "\n\n")
    + "\n"

let private jsonLine (sortKeys: bool) (j: Json) = dumpsWith { defaults with EnsureAscii = false; SortKeys = sortKeys } j + "\n"

let main (io: Io.Console) (argv: string list) : int =
    let root = argv.[1]
    let clauses = loadClauses (readClauseData root)
    let inv = Io.readCsvDicts (Io.path root "requirements/reconciliation/2026-11-23/data/inventory.csv")
    let recordsByFile = RECORD_FILES |> List.map (fun f -> f, Io.readJsonl (Io.path root f))
    let records = recordsByFile |> List.collect (fun (f, rs) -> rs |> List.map (fun r -> rid r, (r, f))) |> toDict
    let register = QL.loadCsv (Io.path root "requirements/open-questions.csv") |> List.map (itemStr "question_id") |> Set.ofList
    let controlIds = Io.readCsvDicts (Io.path root CONTROL_MATRIX) |> List.map (itemStr "control_id") |> Set.ofList
    match validate clauses inv (records |> List.map fst |> Set.ofList) register controlIds with
    | _ :: _ as problems ->
        io.Err(String.concat "\n" problems + "\n")
        2
    | [] ->
        // 1. clause data
        Io.writeText (Io.path root (MISSION + "/data/convergence-clauses.jsonl")) (clauses |> List.map (jsonLine true) |> String.concat "")
        // 2. domain sections
        let targets = clauses |> List.map (itemStr "target_file") |> dedupe
        for t in targets do
            let p = Io.path root t
            Io.writeText p (putSection (Io.readText p) (section (clauses |> List.filter (fun c -> itemStr "target_file" c = t)) t))
        // 3. annotations on Markdown definitions (first definition site per id)
        let conv = convergedBy clauses
        let sites =
            inv
            |> List.filter (fun r -> conv.ContainsKey(itemStr "identifier" r) && itemStr "kind" r = "requirement")
            |> List.map (fun r -> itemStr "identifier" r, (itemStr "path" r, int (itemStr "line" r)))
            |> List.fold (fun acc (i, site) -> if acc |> List.exists (fst >> (=) i) then acc else acc @ [ i, site ]) []
        let byFile =
            sites
            |> List.fold
                (fun (acc: (string * (int * string) list) list) (ident, (path, line)) ->
                    if acc |> List.exists (fst >> (=) path) then acc |> List.map (fun (k, v) -> if k = path then k, v @ [ line, ident ] else k, v)
                    else acc @ [ path, [ line, ident ] ])
                []
        for path, its in byFile do
            let p = Io.path root path
            let lines = splitOn "\n" (Io.readText p)
            let apply (ls: string list) (lineNo: int, ident: string) =
                let arr = List.toArray ls
                let idx = seq { max 0 (lineNo - 1) .. min arr.Length (lineNo + 400) - 1 } |> Seq.tryFind (fun i -> contains ident arr.[i])
                match idx with
                | None -> ls
                | Some idx ->
                    let cs = conv.[ident] |> List.distinct |> sortStrs |> List.map (fun c -> $"`{c}`") |> String.concat ", "
                    let note = $"> {NOTE}: governed by {cs} (section 'Converged requirements: master mission' in the domain file named by each clause)."
                    annotate ls (idx + 1) note
            let ordered = its |> List.sortWith (fun (l1, i1) (l2, i2) -> match compare l2 l1 with 0 -> cmpStr i2 i1 | c -> c)
            Io.writeText p (ordered |> List.fold apply lines |> String.concat "\n")
        // 4. JSONL records
        for f, rs in recordUpdates clauses recordsByFile do
            Io.writeText (Io.path root f) (rs |> List.map (jsonLine false) |> String.concat "")
        // 5. events
        let events = QL.loadEvents root
        let add = appliedEvents clauses (QL.fold events) events
        Io.appendText (Io.path root QL.EVENTS) (add |> List.map (jsonLine false) |> String.concat "")
        // 5b. clause-to-control crosswalk
        let crosswalk =
            [ [ "clause_id"; "control_id"; "control_operator"; "report_relevance"; "automated"; "cuec_id"; "evidence_query" ] ]
            @ (clauses
               |> List.collect (fun c ->
                   let ic = getOr "internal_control" JNull c
                   (ic |> orElse (JObj []) |> listOf "controls")
                   |> List.map (fun k ->
                       [ itemStr "id" c; str k; str (item "operator" ic); str (item "report" ic); (if truthy (item "automated" ic) then "yes" else "no")
                         str (item "cuec_id" ic); "to be defined at implementation (IPE definition per CNV-GOV-001)" ])))
        Io.writeText (Io.path root CROSSWALK) (csvQuoteAll "\n" crosswalk)
        // 6. baseline index
        let adoption = Io.readCsvDicts (Io.path root "requirements/reconciliation/2026-11-23/data/adoption.csv")
        let release =
            Io.readJsonl (Io.path root "requirements/discovery/2026-11-23-cross-industry/data/release1-scope.jsonl")
            |> List.map (fun l -> item "id" l, item "scope" l)
            |> Map.ofList
        let od = Io.path root (MISSION + "/data/owner-decisions.jsonl")
        let decisions = if Io.exists od then Io.readJsonl od else []
        let ratified =
            decisions
            |> List.filter (fun d -> truthyOpt (get "ratifies_clauses" d))
            |> List.collect (fun d -> getOr "clauses" (JArr []) d |> asList |> List.map (fun cid -> str cid, item "id" d))
            |> Map.ofList
        let rows = baseline clauses inv records adoption release (QL.fold (QL.loadEvents root)) ratified
        Io.mkdir (Io.path root "requirements/baseline")
        Io.writeText (Io.path root "requirements/baseline/baseline.jsonl") (rows |> List.map (jsonLine true) |> String.concat "")
        Io.writeText (Io.path root "requirements/AUTHORITATIVE-BASELINE.md") (renderBaseline rows clauses)
        io.Out($"clauses {clauses.Length}, files {targets.Length}, annotated {sites.Length}, events {add.Length}, baseline rows {rows.Length}\n")
        0

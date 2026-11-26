/// Integrate Phase 1 evidence dossiers into the evidence register and the resolution log
/// (port of tools/reqaudit/integrate_dossiers.py).
module Unbound.Convergence.IntegrateDossiers

open System.IO
open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat
module QL = Unbound.Convergence.QuestionsLib

let MISSION = "requirements/convergence/2026-11-24-master"
let REPORT = MISSION + "/02-EVIDENCE-DOSSIERS.md"
let ON = "2026-11-24"
let ACTOR = "Master mission agent (zero-consultation evidence research; agent-recommended, owner ratification pending)"

let BATCH_NAMES =
    Map.ofList
        [ "A", "Charitable tax law"; "B", "Accounting and financial reporting"; "C", "Banking and payments"
          "D", "Financial regulation"; "E", "Governance, succession and delegated authority"
          "F", "Privacy, communications and sanctions"; "G", "Assurance, internal control and board delegation" ]

/// sorted(glob(os.path.join(indir, prefix + "*.jsonl")))
let private globJsonl (indir: string) (prefix: string) : string list =
    if not (Directory.Exists indir) then []
    else
        Directory.EnumerateFileSystemEntries indir
        |> Seq.map Path.GetFileName
        |> Seq.filter (fun n -> startsWith prefix n && endsWith ".jsonl" n && n.Length >= prefix.Length + 6)
        |> Seq.map (fun n -> if endsWith "/" indir then indir + n else indir + "/" + n)
        |> sortStrs

let private batchOf (p: string) = (splitOn "." (splitOn "-" (Path.GetFileName p)).[1]).[0]

let loadBatches (indir: string) : Json list * Json list =
    let load prefix = globJsonl indir prefix |> List.collect (fun p -> Io.readJsonl p |> List.map (set "batch" (JStr(batchOf p))))
    load "dossiers-", load "evidence-"

/// sha256 of json.dumps(dossier without "batch", sort_keys=True), first 16 hex digits.
let dsha (d: Json) : string =
    let without = JObj(items d |> List.filter (fun (k, _) -> k <> "batch"))
    (sha256Hex (dumpsWith { defaults with SortKeys = true } without)).Substring(0, 16)

let asList (x: Json option) : Json list =
    match x with
    | Some(JArr xs) -> xs
    | Some x when truthy x -> [ x ]
    | _ -> []

let check (dossiers: Json list) (evidence: Json list) (states: QL.States) : string list =
    let evIds = evidence |> List.map (item "evidence_id") |> Set.ofList
    let problems =
        dossiers
        |> List.collect (fun d ->
            let q = str (item "question_id" d)
            (if not (match get "disposition" d with Some(JStr s) -> List.contains s QL.DISPOSITIONS | _ -> false) then
                 [ $"""{q}: unknown disposition {reprOpt (get "disposition" d)}""" ]
             else [])
            @ (if not (match item "question_id" d with JStr s -> states.ById.ContainsKey s | _ -> false) then [ $"{q}: not a registered question" ] else [])
            @ (asList (get "evidence_ids" d) |> List.filter (fun i -> not (evIds.Contains i)) |> List.map (fun i -> $"{q}: cites unknown evidence {str i}")))
    // Python iterates a set here (arbitrary order when several ids repeat); first-occurrence order is used.
    let dupEv =
        evidence |> List.map (item "evidence_id") |> counter |> List.filter (fun (_, n) -> n > 1) |> List.map (fun (i, _) -> $"duplicate evidence id {str i}")
    problems @ dupEv

let private s (x: string) = JStr x
let private g (k: string) (d: Json) = getOr k (s "") d

/// Events for the dossier's own question and for same-question duplicates it answers.
let primaryEvents (d: Json) (clusters: Json list) (states: QL.States) : (string * Json) list =
    let qid = itemStr "question_id" d
    let gate = getOr "external_authorization_required" JNull d |> orElse (s "")
    let empirical = get "disposition" d = Some(s "empirical-validation-required")
    let status = if get "disposition" d = Some(s "unresolved") then "proposed" else "resolved"
    let sha = dsha d
    let batch = item "batch" d
    let baseSet =
        JObj
            [ "answer", g "recommended_answer" d
              "disposition", g "disposition" d
              "confidence", g "confidence" d
              "evidence_refs", JArr(asList (get "evidence_ids" d))
              "conservative_fallback", g "conservative_fallback" d
              "remaining_uncertainty", g "remaining_uncertainty" d
              "acceptance_criteria", JArr(asList (get "required_acceptance_criteria" d))
              "reconsideration_triggers", JArr(asList (get "reconsideration_triggers" d))
              "external_authorization_required", gate
              "activation_gate", (gate |> orElse (if empirical then s ("Empirical validation: " + str (g "remaining_uncertainty" d)) else s ""))
              "alternative_design", g "alternative_design" d
              "resolution_status", s status
              "approval_status", s "agent-recommended"
              "dossier_sha", s sha
              "dossier_batch", batch ]
    let own = [ qid, baseSet ]
    let related =
        asList (get "related_question_answers" d)
        |> List.filter (fun r -> (match r with JObj _ -> true | _ -> false) && truthyOpt (get "id" r))
        |> List.map (fun r -> str (item "id" r), getOr "answer" (s "") r)
        |> toDict
    let relatedM = Map.ofList related
    let members = clusters |> List.filter (fun c -> itemStr "canonical_id" c = qid)
    let memberIds = members |> List.map (itemStr "member_id") |> Set.ofList
    let memberEvent (c: Json) =
        let mid = itemStr "member_id" c
        let same = List.contains (itemStr "relation" c) [ "same-question"; "sub-question" ]
        let ans = relatedM.TryFind mid |> Option.defaultValue (s "")
        let st =
            if same then
                JObj
                    [ "resolution_status", s "superseded-by-duplicate"
                      "canonical_id", s qid
                      "approval_status", s "agent-recommended"
                      "disposition", g "disposition" d
                      "answer", ans |> orElse (s ($"Answered by {qid}: " + str (g "recommended_answer" d))) ]
            else
                JObj([ "related_evidence", s $"See {qid} dossier" ] @ (if truthy ans then [ "related_answer", ans ] else []))
        mid, st |> set "dossier_sha" (s sha) |> set "dossier_batch" batch
    let extra =
        related
        |> List.filter (fun (q, _) -> states.ById.ContainsKey q && not (memberIds.Contains q) && q <> qid)
        |> List.map (fun (q, a) ->
            q, JObj [ "related_evidence", s $"See {qid} dossier"; "related_answer", a; "dossier_sha", s sha; "dossier_batch", batch ])
    own @ (members |> List.filter (fun c -> states.ById.ContainsKey(itemStr "member_id" c)) |> List.map memberEvent) @ extra

let eventNumber (e: Json) : int = int (splitOn "-" (itemStr "event_id" e)).[1]

let newEvents (dossiers: Json list) (clusters: Json list) (states: QL.States) (existing: Json list) : Json list =
    let seen =
        existing
        |> List.map (fun e -> itemStr "question_id" e, item "type" e, getOr "set" (JObj []) e |> get "dossier_sha")
        |> Set.ofList
    let fresh =
        dossiers
        |> List.collect (fun d -> primaryEvents d clusters states)
        |> List.filter (fun (q, st) -> not (seen.Contains(q, s "evidence-reviewed", get "dossier_sha" st)))
    let start = existing |> List.map eventNumber |> List.fold max 0
    fresh
    |> List.mapi (fun i (q, st) ->
        JObj
            [ "event_id", s ("QE-" + pad6 (start + i + 1)); "question_id", s q; "on", s ON; "type", s "evidence-reviewed"
              "artifact", s REPORT; "actor", s ACTOR; "set", st ])

// ---------------------------------------------------------------- rendering

let cell (n: int option) (t: Json option) : string =
    let x = joinWs (str (t |> Option.defaultValue JNull |> orElse (s ""))) |> fun x -> x.Replace("|", "\\|")
    match n with
    | Some n when cpLen x > n -> cpTake (n - 1) x + "…"
    | _ -> x

let private c0 (t: Json option) = cell None t

let bullets (xs: Json option) : string =
    match asList xs |> List.map (fun x -> "- " + c0 (Some x)) |> String.concat "\n" with
    | "" -> "- none recorded"
    | b -> b

let private bySortKey (d: Json) = [ str (item "batch" d); str (item "question_id" d) ]

let render (dossiers: Json list) (evidence: Json list) (states: QL.States) : string =
    let ev = evidence |> List.map (fun e -> item "evidence_id" e, e) |> Map.ofList
    ignore ev
    let byDisp = dossiers |> List.map (fun d -> itemStr "disposition" d) |> counter
    let ext59 = QL.stateItems states |> List.filter (fun (_, st) -> get "review_class" st = Some(s "External")) |> List.map fst
    let covered = dossiers |> List.map (fun d -> itemStr "question_id" d) |> Set.ofList
    let sortedDossiers = dossiers |> sortByStrs bySortKey
    let summary =
        [ "| Question | Batch | Disposition | Confidence | Recommended answer (short) |"; "|---|---|---|---|---|" ]
        @ (sortedDossiers
           |> List.map (fun d ->
               $"""| {str (item "question_id" d)} | {str (item "batch" d)} | {str (item "disposition" d)} | {getStr "confidence" d} | {cell (Some 220) (get "recommended_answer" d)} |"""))
        |> String.concat "\n"
    let finding (f: Json) =
        let pin = if truthyOpt (get "pinpoint" f) then ", " + c0 (get "pinpoint" f) else ""
        $"""- *{getStr "classification" f}* — {c0 (get "claim" f)} [{getStr "evidence_id" f}{pin}]"""
    let isDict (j: Json) = match j with JObj _ -> true | _ -> false
    let orNone (dflt: string) (x: string) = if x = "" then dflt else x
    let section (d: Json) =
        let rel =
            asList (get "related_question_answers" d) |> List.filter isDict
            |> List.map (fun r -> $"""- **{strOpt (get "id" r)}**: {c0 (get "answer" r)}""") |> String.concat "\n" |> orNone "- none"
        let corr =
            asList (get "corrections_to_prior_records" d) |> List.filter isDict
            |> List.map (fun c -> $"""- {c0 (get "record_or_artifact" c)}: {c0 (get "issue" c)} → {c0 (get "correction" c)}""")
            |> String.concat "\n" |> orNone "- none"
        let batch = str (item "batch" d)
        [ $"""### {str (item "question_id" d)} — {cell (Some 200) (get "exact_question" d)}"""
          $"""**Disposition:** {str (item "disposition" d)} · **Confidence:** {getStr "confidence" d} · **Jurisdiction:** {c0 (get "jurisdiction" d)} · **Batch:** {batch} ({BATCH_NAMES.TryFind batch |> Option.defaultValue ""})"""
          $"""**Original provisional answer.** {c0 (get "original_provisional_answer" d)}"""
          $"""**Relevant requirements.** {c0 (Some(s (asList (get "relevant_requirements" d) |> List.map str |> String.concat ", ")))}"""
          "**Authoritative findings.**\n\n" + (asList (get "authoritative_findings" d) |> List.filter isDict |> List.map finding |> String.concat "\n" |> orNone "- none")
          $"""**Supporting interpretation.** {c0 (get "supporting_interpretation" d)}"""
          $"""**Competing interpretation.** {c0 (get "competing_interpretation" d)}"""
          $"""**Applicability to Unbound.** {c0 (get "applicability_to_unbound" d)}"""
          "**Business assumptions.**\n\n" + bullets (get "business_assumptions" d)
          $"""**Recommended answer.** {c0 (get "recommended_answer" d)}"""
          $"""**Remaining uncertainty.** {c0 (get "remaining_uncertainty" d)}"""
          $"""**Consequences if wrong.** {c0 (get "consequences_if_wrong" d)}"""
          $"""**Conservative fallback.** {c0 (get "conservative_fallback" d)}"""
          $"""**Alternative business or architecture design.** {c0 (get "alternative_design" d)}"""
          $"""**External authorization required.** {c0 (get "external_authorization_required" d) |> orNone "none"}"""
          "**Acceptance criteria.**\n\n" + bullets (get "required_acceptance_criteria" d)
          "**Reconsideration triggers.**\n\n" + bullets (get "reconsideration_triggers" d)
          "**Related questions.**\n\n" + rel
          "**Corrections to prior records.**\n\n" + corr ]
        |> String.concat "\n\n"
    let evTable =
        [ "| ID | Level | Source | Status | Effective | Fetched | Pinpoint |"; "|---|---|---|---|---|---|---|" ]
        @ (evidence
           |> sortByStrs (fun e -> [ str (item "evidence_id" e) ])
           |> List.map (fun e ->
               $"""| {str (item "evidence_id" e)} | {getStr "hierarchy_level" e} | [{cell (Some 90) (get "title" e)}]({getStr "url" e}) ({cell (Some 40) (get "issuer" e)}) | {getStr "status" e} | {cell (Some 20) (get "effective_date" e)} | {(if truthyOpt (get "verified_by_fetch" e) then "yes" else "no")} | {cell (Some 60) (get "pinpoint" e)} |"""))
        |> String.concat "\n"
    let missing = ext59 |> List.filter (fun q -> not (covered.Contains q)) |> List.distinct |> sortStrs
    let byDispJson = dumpsWith { defaults with SortKeys = true } (JObj(byDisp |> List.map (fun (k, n) -> k, JInt(System.Numerics.BigInteger n))))
    let fetched = evidence |> List.filter (fun e -> truthyOpt (get "verified_by_fetch" e)) |> List.length
    ([ "# 02 — Evidence dossiers (Phase 1, zero-consultation evidence resolution)"
       "Generated by `tools/fs/Unbound.Convergence` (`integrate-dossiers`) from `data/dossiers.jsonl` and `data/evidence.jsonl`; do not edit by hand. "
       + "Research used only publicly available sources retrieved on 2026-11-24. **Research is not authorization**: a dossier never "
       + "substitutes for a board resolution, executed contract, bank or acquirer approval, licence, filing or assurance report. "
       + "Every answer is *agent-recommended* until the owner ratifies it; conservative fallbacks are adopted into the baseline in the meantime (report 03)."
       "## Method\n\nSeven domain batches researched the 59 externally gated confirmations and the further requirement questions and "
       + "decisions whose gates could be answered by research. Evidence hierarchy: statute, effective regulation, official guidance, "
       + "case law, accounting standards, official interpretations, public contracts and rules, audited statements, sponsor policies, "
       + "professional publications, industry research, commentary. Proposed regulations are recorded as proposed. Each source records "
       + "whether it was fetched; unfetched citations are marked. Dispositions: established-by-authoritative-evidence, reasoned-interpretation, "
       + "business-policy-decision, empirical-validation-required, external-authorization-required, unresolved."
       $"## Coverage\n\n- Dossiers: {dossiers.Length} (by disposition: {byDispJson})\n"
       + $"- Evidence sources: {evidence.Length} ({fetched} fetched)\n"
       + $"""- Externally gated confirmations without a dossier: {(if missing.IsEmpty then "none" else String.concat ", " missing)}"""
       "## Summary\n\n" + summary
       "## Dossiers\n\n" + (sortedDossiers |> List.map section |> String.concat "\n\n")
       "## Evidence register\n\n" + evTable ]
     |> String.concat "\n\n")
    + "\n"

let private jsonLine (sortKeys: bool) (j: Json) = dumpsWith { defaults with EnsureAscii = false; SortKeys = sortKeys } j + "\n"

let main (io: Io.Console) (argv: string list) : int =
    let root, indir = argv.[1], argv.[2]
    let dossiers, evidence = loadBatches indir
    let events = QL.loadEvents root
    let states = QL.fold events
    match check dossiers evidence states with
    | _ :: _ as problems ->
        io.Err(String.concat "\n" problems + "\n")
        2
    | [] ->
        let clusters = QL.loadCsv (Io.path root QL.CLUSTERS)
        let add = newEvents dossiers clusters states events
        Io.mkdirs (Io.path root (MISSION + "/data"))
        Io.writeText (Io.path root (MISSION + "/data/evidence.jsonl"))
            (evidence |> sortByStrs (fun e -> [ str (item "evidence_id" e) ]) |> List.map (jsonLine true) |> String.concat "")
        Io.writeText (Io.path root (MISSION + "/data/dossiers.jsonl")) (dossiers |> sortByStrs bySortKey |> List.map (jsonLine true) |> String.concat "")
        Io.appendText (Io.path root QL.EVENTS) (add |> List.map (jsonLine false) |> String.concat "")
        Io.writeText (Io.path root REPORT) (render dossiers evidence (QL.fold (QL.loadEvents root)))
        io.Out($"dossiers {dossiers.Length}, evidence {evidence.Length}, new events {add.Length}\n")
        0

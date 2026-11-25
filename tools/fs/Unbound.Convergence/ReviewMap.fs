/// Map every 2026-11-24 decision-review row to an exact register question (port of tools/reqaudit/review_map.py).
module Unbound.Convergence.ReviewMap

open System.Text.RegularExpressions
open Unbound.Py.PyJson
open Unbound.Convergence.PyCompat

let DISC = "requirements/discovery"

let PROPOSAL_ARTIFACTS =
    [ "2026-11-24-initial-38-question-resolutions.md"
      "2026-11-24-human-intervention-and-question-resolution.md"
      "2026-11-24-question-resolutions-accounting-banking-economics.md"
      "2026-11-24-final-55-question-resolutions.md" ]

let DISPOSITION_ARTIFACT = "2026-11-24-adversarial-approval-review.md"
let CORRECTION_ARTIFACT = "2026-11-24-approved-corrections-and-decision-gates.md"
let HOLD_ARTIFACT = "2026-11-24-three-evidence-hold-resolutions.md"
let DECISION_ARTIFACT = "2026-11-24-open-decision-resolution-review.md"
let CONFIRMATION_ARTIFACT = "2026-11-24-confirmation-question-review.md"
let EVIDENCE_ARTIFACT = "2026-11-24-zero-consultation-evidence-review-01.md"

let OUT_FIELDS =
    [ "question_id"; "kind"; "register_question"; "proposal_artifact"; "proposal_label"; "proposed_answer"; "proposal_gate"
      "disposition"; "disposition_note"; "correction"; "hold_resolution"; "decision_review_direction"; "decision_review_gate"
      "confirmation_class"; "confirmation_answer"; "evidence_review"; "quoted_question_matches"; "mapping_flags" ]

/// Python's \b next to a word character, with Python's notion of a word character (str.isalnum() or "_").
let private W = @"[\p{L}\p{N}_]"
let private idCore = @"(?<!" + W + @")(?:RR|UNB|SPD|XID|RCD)-[A-Z0-9-]*\d(?:-r\d+)?(?!" + W + ")"
/// ID_RE.findall
let private idRe = Regex(idCore)
/// ID_RE.match
let private idMatch = Regex(@"\A" + idCore)
/// ID_RE.fullmatch
let private idFull = Regex(@"\A(?:" + idCore + @")\z")
let private sp = @"[\s\x1c-\x1f]"
let private ruleRe = Regex(@"\A(?:\|?(" + sp + @"*:?-{3,}:?" + sp + @"*\|)+" + sp + @"*:?-*:?" + sp + @"*\|?)\z")
let private pipeSplit = Regex(@"(?<!\\)\|")
let private heading = Regex(@"^## ", RegexOptions.Multiline)
let private nonAlnum = Regex("[^a-z0-9]+")

/// Every data row of every pipe table, as stripped cells (header and rule rows dropped).
let tableRows (text: string) : string list list =
    let lines = splitLines text |> List.map strip
    let cells (l: string) = pipeSplit.Split(stripChars "|" (strip l)) |> Array.map strip |> List.ofArray
    let isRule (l: string) = ruleRe.IsMatch l
    let prevs = None :: (lines |> List.map Some) |> List.truncate lines.Length
    List.zip prevs lines
    |> List.choose (fun (prev, l) ->
        let inTable = startsWith "|" l && not (isRule l)
        let header = inTable && (match prev with None -> true | Some p -> not (startsWith "|" p))
        if inTable && not header then Some(cells l) else None)

let recordOf (qid: string) : string = (splitOn "#" qid).[0]

let private qnum (qid: string) = int (splitOn "#Q" qid).[1]

/// Assign review rows for one record to that record's questions in source order.
let orderedAssignment (reviewRows: 'p list) (questions: Json list) : (string option * 'p) list * string =
    let qids = questions |> List.map (itemStr "question_id") |> List.sortBy qnum
    let pairs = reviewRows |> List.mapi (fun i r -> (if i < qids.Length then Some qids.[i] else None), r)
    let flag = if reviewRows.Length = qids.Length then "" else $"count-mismatch:{reviewRows.Length}-reviews-vs-{qids.Length}-questions"
    pairs, flag

/// rows: (record, payload). Returns question_id -> payload, orphans, record -> flag.
let assignByRecord (rows: (string * 'p) list) (regRq: Json list) =
    let records = rows |> List.map fst |> dedupe
    let results =
        records
        |> List.map (fun rec' ->
            let byRecord = rows |> List.filter (fun (r, _) -> r = rec') |> List.map snd
            let qs = regRq |> List.filter (fun q -> recordOf (itemStr "question_id" q) = rec')
            let pairs, flag = orderedAssignment byRecord qs
            rec', pairs, flag)
    let mapping = results |> List.collect (fun (_, pairs, _) -> pairs |> List.choose (fun (q, p) -> q |> Option.map (fun q -> q, p))) |> Map.ofList
    let orphans = results |> List.collect (fun (rec', pairs, _) -> pairs |> List.filter (fun (q, _) -> q.IsNone) |> List.map (fun (_, p) -> rec', p))
    let flags = results |> List.filter (fun (_, _, f) -> f <> "") |> List.map (fun (r, _, f) -> r, f)
    mapping, orphans, flags

let norm (t: string) : string = strip (nonAlnum.Replace(lower t, " "))

let private head (p: string) = (splitOnce "\n" p).[0]
let private body (p: string) = (splitOnce "\n" p).[1]

/// Three evidence-hold sections: record -> text.
let holds (text: string) : (string * string) list =
    heading.Split text
    |> List.ofArray
    |> List.tail
    |> List.filter (fun p -> strip (head p) <> "Status")
    |> List.map (fun p -> strip (splitOn " / " (head p)).[0], joinWs (body p))
    |> toDict

let evidenceSections (text: string) : (string * string) list =
    heading.Split text
    |> List.ofArray
    |> List.tail
    |> List.map (fun p -> head p, (if contains "\n" p then joinWs (body p) else ""))
    |> List.collect (fun (h, b) -> idRe.Matches h |> Seq.map (fun m -> m.Value, b) |> List.ofSeq)
    |> toDict

type Proposal = { Artifact: string; Label: string; Answer: string; Gate: string }
type Disposition = { Disposition: string; Note: string; Label: string }

let private jint (n: int) = JInt(System.Numerics.BigInteger n)
let private countObj (xs: string list) = JObj(counter xs |> List.map (fun (k, n) -> k, jint n))

let build (read: string -> string) (reg: Json list) : (string * string) list list * Json =
    let regById = reg |> List.map (fun r -> itemStr "question_id" r, r) |> Map.ofList
    let regRq = reg |> List.filter (fun r -> itemStr "kind" r = "requirement question")
    let proposals =
        PROPOSAL_ARTIFACTS
        |> List.collect (fun name ->
            tableRows (read name)
            |> List.filter (fun c -> idMatch.IsMatch c.[0])
            |> List.map (fun c ->
                c.[0], { Artifact = name; Label = c.[1]; Answer = c.[2]; Gate = (if c.Length > 3 then c.[3] else "") }))
    let propMap, propOrphans, propFlags = assignByRecord proposals regRq
    let disp =
        tableRows (read DISPOSITION_ARTIFACT)
        |> List.filter (fun c -> idMatch.IsMatch c.[0])
        |> List.map (fun c -> c.[0], { Disposition = c.[2]; Note = c.[3]; Label = c.[1] })
    let dispMap, dispOrphans, dispFlags = assignByRecord disp regRq
    let corrRows = tableRows (read CORRECTION_ARTIFACT) |> List.filter (fun c -> idMatch.IsMatch c.[0] && c.Length = 4 && c.[2] = "revise")
    let corrByLabel = corrRows |> List.map (fun c -> (c.[0], c.[1]), c.[3]) |> Map.ofList
    let holdMap = holds (read HOLD_ARTIFACT) |> Map.ofList
    let dec = tableRows (read DECISION_ARTIFACT) |> List.filter (fun c -> idFull.IsMatch c.[0]) |> List.map (fun c -> c.[0], c) |> toDict
    let conf = tableRows (read CONFIRMATION_ARTIFACT) |> List.filter (fun c -> idFull.IsMatch c.[0]) |> List.map (fun c -> c.[0], c) |> toDict
    let decM, confM = Map.ofList dec, Map.ofList conf
    let ev = evidenceSections (read EVIDENCE_ARTIFACT)
    let evM = Map.ofList ev
    let propFlagM = Map.ofList propFlags
    let row (q: Json) : (string * string) list =
        let qid, kind = itemStr "question_id" q, itemStr "kind" q
        let p, d = propMap.TryFind qid, dispMap.TryFind qid
        let labelMismatch = match p, d with Some p, Some d -> norm p.Label <> norm d.Label | _ -> false
        let holdKey = if holdMap.ContainsKey(recordOf qid) then recordOf qid else ""
        let dr, cr = decM.TryFind qid, confM.TryFind qid
        let quoted = match dr, cr with Some dr, _ -> dr.[1] | None, Some cr -> cr.[1] | None, None -> ""
        let regQ = removePrefix "Confirm provisional decision: " (itemStr "question" q)
        let matches = if quoted = "" then "" elif norm quoted = norm regQ then "yes" else "no"
        let isRq = kind = "requirement question"
        let flags =
            [ (if isRq && p.IsNone then "no-proposal" else "")
              (if isRq && d.IsNone then "no-disposition" else "")
              (if labelMismatch then "proposal-disposition-label-differs" else "")
              (if isRq then propFlagM.TryFind(recordOf qid) |> Option.defaultValue "" else "")
              (if kind = "open decision" && dr.IsNone then "no-decision-review" else "")
              (if kind = "awaiting confirmation" && cr.IsNone then "no-confirmation-review" else "")
              (if matches = "no" then "quoted-question-differs" else "") ]
            |> List.filter ((<>) "")
        let dDisp = d |> Option.map (fun d -> d.Disposition) |> Option.defaultValue ""
        [ "question_id", qid
          "kind", kind
          "register_question", itemStr "question" q
          "proposal_artifact", p |> Option.map (fun p -> p.Artifact) |> Option.defaultValue ""
          "proposal_label", p |> Option.map (fun p -> p.Label) |> Option.defaultValue ""
          "proposed_answer", p |> Option.map (fun p -> p.Answer) |> Option.defaultValue ""
          "proposal_gate", p |> Option.map (fun p -> p.Gate) |> Option.defaultValue ""
          "disposition", dDisp
          "disposition_note", d |> Option.map (fun d -> d.Note) |> Option.defaultValue ""
          "correction",
          (if dDisp = "revise" then
               corrByLabel.TryFind((recordOf qid, d |> Option.map (fun d -> d.Label) |> Option.defaultValue "")) |> Option.defaultValue ""
           else "")
          "hold_resolution", (if dDisp = "hold" then holdMap.TryFind holdKey |> Option.defaultValue "" else "")
          "decision_review_direction", (match dr with Some dr -> dr.[3] | None -> "")
          "decision_review_gate", (match dr with Some dr -> dr.[4] | None -> "")
          "confirmation_class", (match cr with Some cr -> cr.[2] | None -> "")
          "confirmation_answer", (match cr with Some cr -> cr.[3] | None -> "")
          "evidence_review", evM.TryFind qid |> Option.defaultValue ""
          "quoted_question_matches", matches
          "mapping_flags", String.concat ";" flags ]
    let rows = reg |> List.map row
    let field k (r: (string * string) list) = r |> List.find (fst >> (=) k) |> snd
    let regIds = regById |> Map.keys |> Set.ofSeq
    let summary =
        JObj
            [ "register", countObj (reg |> List.map (itemStr "kind"))
              "register_total", jint reg.Length
              "proposal_rows", jint proposals.Length
              "proposal_rows_per_artifact", countObj (proposals |> List.map (fun (_, p) -> p.Artifact))
              "proposal_orphans", jstrs (propOrphans |> List.map fst)
              "proposal_count_mismatches", JObj(propFlags |> List.map (fun (r, f) -> r, JStr f))
              "disposition_rows", jint disp.Length
              "dispositions", countObj (disp |> List.map (fun (_, d) -> d.Disposition))
              "disposition_orphans", jstrs (dispOrphans |> List.map fst)
              "disposition_count_mismatches", JObj(dispFlags |> List.map (fun (r, f) -> r, JStr f))
              "correction_rows", jint corrRows.Length
              "corrections_joined", jint (rows |> List.filter (fun r -> field "correction" r <> "") |> List.length)
              "holds_joined", jint (rows |> List.filter (fun r -> field "hold_resolution" r <> "") |> List.length)
              "decision_review_rows", jint dec.Length
              "decision_review_ids_not_open_decisions",
              jstrs (dec |> List.map fst |> List.filter (fun i -> (regById.TryFind i |> Option.bind (get "kind")) <> Some(JStr "open decision")) |> sortStrs)
              "confirmation_review_rows", jint conf.Length
              "confirmation_classes", countObj (conf |> List.map (fun (_, c) -> c.[2]))
              "confirmation_review_ids_not_confirmations",
              jstrs (conf |> List.map fst |> List.filter (fun i -> (regById.TryFind i |> Option.bind (get "kind")) <> Some(JStr "awaiting confirmation")) |> sortStrs)
              "evidence_review_ids", jstrs (ev |> List.map fst |> sortStrs)
              "evidence_review_ids_unknown", jstrs (ev |> List.map fst |> List.filter (fun i -> not (regIds.Contains i)) |> sortStrs)
              "flag_counts", countObj (rows |> List.collect (fun r -> splitOn ";" (field "mapping_flags" r) |> List.filter ((<>) "")))
              "questions_with_flags", jstrs (rows |> List.filter (fun r -> field "mapping_flags" r <> "") |> List.map (field "question_id")) ]
    rows, summary

let toCsv (rows: (string * string) list list) : string = csvQuoteAll "\n" ([ OUT_FIELDS ] @ (rows |> List.map (List.map snd)))

let main (io: Io.Console) (argv: string list) : int =
    let root, out = argv.[1], argv.[2]
    let reg = Io.readCsvDicts (Io.path root "requirements/open-questions.csv")
    let rows, summary = build (fun name -> Io.readText (Io.path root (DISC + "/" + name))) reg
    Io.mkdirs out
    Io.writeText (Io.path out "review-map.csv") (toCsv rows)
    let text = dumpsWith { defaults with Indent = Some 2 } summary
    Io.writeText (Io.path out "review-map-summary.json") (text + "\n")
    io.Out(text + "\n")
    0

/// quality.py: deterministic requirement-quality heuristics over the inventory produced by inventory.
module Unbound.ReqAudit.Quality

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let RFC2119 = re @"\b(SHALL NOT|SHALL|MUST NOT|MUST|SHOULD NOT|SHOULD|MAY|REQUIRED)\b"
let NORMATIVE = re @"\b(SHALL|MUST|SHOULD|MAY|REQUIRED|never|cannot|must|shall|requires?|prohibit\w*|block\w*|reject\w*)\b"

let VAGUE =
    reI (@"\b(support\w*|flexible|appropriate(ly)?|where (relevant|applicable|needed|warranted|justified)|as needed|"
         + @"reasonabl\w*|robust|seamless\w*|user-friendly|comprehensive|etc\.?|various|adequate\w*|"
         + @"sufficient\w*|proportionate|optimi[sz]e\w*|enhanc\w*|intelligent\w*|easily)\b")

let ACCEPTANCE = reI @"\b(Given|acceptance|accept(ed)?|test(s|ed)?|verify|verified|assert|oracle|scenario|ADV-\d+)\b"
let TEST_FILE = reI @"(TESTS?|SCENARIOS?|VALIDATION|scenarios)\.csv$|ADVERSARIAL"
let QUESTION_FAMILIES = re "-Q$"
let private REQ_ID = re @"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-[A-Z]?\d{1,4}\b"
let private TEST_ID = re @"\b[A-Z][A-Z0-9]*(?:-[A-Z0-9]+)*-\d{1,4}\b"
let private CLAUSE = re @";|\band\b|,"

/// Prefer the requirements/ definition, then the longest text, as the canonical site (ids in sorted order).
let canonical (rows: Json list) : Json list =
    let rank (r: Json) = (if (s "path" r).StartsWith("requirements/", StringComparison.Ordinal) then 0 else 1), -(len (s "text" r))
    let byId = rows |> List.groupBy (s "identifier") |> Map.ofList
    byId |> Map.toList |> List.map fst |> sortStrings |> List.map (fun i -> byId.[i] |> List.sortBy rank |> List.head)

/// Requirement ids cited inside the text of any test or scenario record.
let referencedByTestRecords (all: Json list) : Set<string> =
    all |> List.filter (fun r -> List.contains (s "kind" r) [ "test"; "scenario" ]) |> List.collect (fun r -> findall REQ_ID (s "text" r)) |> Set.ofList

let filesWithTests (all: Json list) : Set<string> = all |> List.filter (fun r -> s "kind" r = "test") |> List.map (s "path") |> Set.ofList

/// Ids cited in the joined text of test-like files.
let referencedInTestsText (text: string) : Set<string> = findall TEST_ID text |> Set.ofList

let private pyBool (b: bool) = if b then "True" else "False"

/// The ordered output columns of requirements-quality.csv for one canonical record.
let signals (rec': Json) (tested: Set<string>) (testFiles: Set<string>) : (string * Json) list =
    let body = s "text" rec'
    let clauses = (finditer CLAUSE body).Length
    let vague = finditer VAGUE body |> List.map (fun m -> lower m.Value) |> Set.ofList |> sortStrings
    let ident = s "identifier" rec'
    let inlineAcc = isSearch ACCEPTANCE body
    [ "identifier", JStr ident
      "family", at "family" rec'
      "path", at "path" rec'
      "line", at "line" rec'
      "words", jint (split body).Length
      "has_normative_modal", JBool(isSearch NORMATIVE body)
      "has_rfc2119_keyword", JBool(isSearch RFC2119 body)
      "vague_terms", JStr(String.Join(";", vague))
      "clause_count", jint clauses
      "compound", JBool(clauses >= 8)
      "inline_acceptance", JBool inlineAcc
      "referenced_by_test_artifact", JBool(tested.Contains ident)
      "acceptance_signal", JBool(inlineAcc || tested.Contains ident)
      "file_has_test_records", JBool(testFiles.Contains(s "path" rec')) ]

let private csvCell (j: Json) = match j with JBool b -> pyBool b | JNull -> "" | j -> str j

let private flag (k: string) (o: (string * Json) list) = match List.find (fst >> (=) k) o |> snd with JBool b -> b | _ -> false
let private text (k: string) (o: (string * Json) list) = List.find (fst >> (=) k) o |> snd |> str

/// requirements-quality.csv text and the summary object.
let build (all: Json list) (testedText: Set<string>) : string * Json =
    let rows = all |> List.filter (fun r -> s "kind" r = "requirement" && not (isSearch QUESTION_FAMILIES (s "identifier" r)))
    let tested = Set.union testedText (referencedByTestRecords all)
    let testFiles = filesWithTests all
    let out = canonical rows |> List.map (fun r -> signals r tested testFiles)
    let header = out.Head |> List.map fst
    let csv = PyCsv.format PyCsv.QuoteMinimal (header :: (out |> List.map (List.map (snd >> csvCell))))
    let countIf f = jint (out |> List.filter f |> List.length)
    let terms = out |> List.collect (fun o -> (text "vague_terms" o).Split(';') |> List.ofArray |> List.filter (fun t -> t <> ""))
    let agg =
        JObj
            [ "requirements_canonical", jint out.Length
              "no_normative_modal", countIf (flag "has_normative_modal" >> not)
              "no_rfc2119_keyword", countIf (flag "has_rfc2119_keyword" >> not)
              "with_vague_terms", countIf (fun o -> text "vague_terms" o <> "")
              "compound_ge_8_clauses", countIf (flag "compound")
              "no_acceptance_signal", countIf (flag "acceptance_signal" >> not)
              "referenced_by_test_artifact", countIf (flag "referenced_by_test_artifact")
              "no_acceptance_signal_and_no_colocated_tests", countIf (fun o -> not (flag "acceptance_signal" o) && not (flag "file_has_test_records" o))
              "vague_term_frequency", JObj(byCountDesc (counter terms) |> List.truncate 15 |> List.map (fun (k, n) -> k, jint n))
              "by_family", JObj(counter (out |> List.map (text "family")) |> List.sortWith (fun (a, _) (b, _) -> cmpStr a b) |> List.map (fun (k, n) -> k, jint n)) ]
    csv, agg

/// Every file under the repository whose name looks like a test artifact (outside reconciliation), in rglob order.
let testFiles (repo: string) : string list =
    let rec walk (d: string) : string seq =
        seq {
            for e in IO.Directory.EnumerateFileSystemEntries d do
                let info = IO.FileInfo e
                if IO.File.Exists e then yield e
                elif IO.Directory.Exists e && isNull info.LinkTarget then yield! walk e
        }
    walk repo
    |> Seq.filter (fun p -> isSearch TEST_FILE (IO.Path.GetFileName p) && not (p.Contains "reconciliation"))
    |> List.ofSeq

let main (argv: string list) : int =
    let inv, repo = argv.[0], argv.[1]
    let all = dictRows (readText (combine inv "inventory.csv"))
    let joined = String.Join(" ", testFiles repo |> List.map readTextReplace)
    let csv, agg = build all (referencedInTestsText joined)
    writeText (combine inv "requirements-quality.csv") csv
    Io.writeJsonReport (combine inv "requirements-quality-summary.json") agg
    Io.print (Io.dumps2 (match agg with JObj kvs -> JObj(kvs |> List.filter (fun (k, _) -> k <> "by_family")) | j -> j))
    0

/// Regression tests for the source-linked question register (port of tools/reqaudit/test_question_register.py).
/// Each test copies the repository's requirement data into a temporary directory, perturbs it, and runs the generator.
/// The repository root is UNBOUND_REPO_ROOT when set, otherwise the nearest ancestor of the test assembly that holds
/// requirements/questions/resolution-events.jsonl.
module Unbound.Convergence.Tests.QuestionRegisterTests

open System
open System.IO
open System.Text
open Xunit
open Unbound.Py.PyJson
open Unbound.Convergence
open Unbound.Convergence.PyCompat

let private root =
    lazy
        (match Environment.GetEnvironmentVariable "UNBOUND_REPO_ROOT" with
         | null
         | "" ->
             let rec up (d: DirectoryInfo) =
                 if isNull d then failwith "repository root not found"
                 elif File.Exists(Path.Combine(d.FullName, "requirements/questions/resolution-events.jsonl")) then d.FullName
                 else up d.Parent
             up (DirectoryInfo AppContext.BaseDirectory)
         | r -> Path.GetFullPath r)

let rec private copyTree (src: string) (dst: string) =
    Directory.CreateDirectory dst |> ignore
    for f in Directory.GetFiles src do
        File.Copy(f, Path.Combine(dst, Path.GetFileName f))
    for d in Directory.GetDirectories src do
        copyTree d (Path.Combine(dst, Path.GetFileName d))

let private sandbox () =
    let d = Path.Combine(Path.GetTempPath(), "qreg-" + Guid.NewGuid().ToString("N"))
    copyTree (Path.Combine(root.Value, "requirements")) (Path.Combine(d, "requirements"))
    d

type private Run = { Code: int; Out: string; Err: string }

let private run (d: string) (extra: string list) : Run =
    let out, err = StringBuilder(), StringBuilder()
    let io: Io.Console = { Out = (fun s -> out.Append s |> ignore); Err = (fun s -> err.Append s |> ignore) }
    let code = Program.run io RenderQuestions.main ([ "render-questions"; d ] @ extra)
    { Code = code; Out = out.ToString(); Err = err.ToString() }

let private csvPath (d: string) = Path.Combine(d, "requirements/open-questions.csv")
let private readText (p: string) = Io.readText p
let private readCsv (d: string) = Io.readCsvDicts (csvPath d)

let private appendEvent (d: string) (event: Json) =
    let p = Path.Combine(d, "requirements/questions/resolution-events.jsonl")
    Io.writeText p (readText p + dumps event + "\n")

let private event (id: string) (q: string) (typ: string) (setFields: (string * Json) list) =
    JObj
        [ "event_id", JStr id; "question_id", JStr q; "on", JStr "2026-11-24"; "type", JStr typ; "artifact", JStr "test"; "actor", JStr "test"
          "set", JObj setFields ]

/// Rewrite a CSV through DictReader/DictWriter, as the Python tests do.
let private rewriteCsv (p: string) (quoteAll: bool) (f: Json list -> Json list) =
    let rows = f (Io.readCsvDicts p)
    let header = keys rows.Head
    let body = rows |> List.map (fun r -> header |> List.map (fun k -> str (getOr k (JStr "") r)))
    Io.writeText p ((if quoteAll then csvQuoteAll "\n" else csvQuoteMinimal "\n") (header :: body))

let private simplificationDecisions (d: string) =
    Path.Combine(d, "requirements/discovery/2026-11-24-simplification-economics/data/decisions.csv")

[<Fact>]
let ``regeneration is idempotent and keeps answers`` () =
    let d = sandbox ()
    let first = run d [], readText (csvPath d)
    let second = run d [], readText (csvPath d)
    Assert.True((fst first).Code = 0, (fst first).Err)
    Assert.Equal(snd first, snd second)
    let rows = readCsv d
    let isCno (r: Json) = (getStr "source" r).Contains "2026-11-26-charity-network-and-onboarding"
    let isCdd (r: Json) = (getStr "source" r).Contains "2026-11-26-competitive-displacement-decisions"
    let qid (r: Json) = getStr "question_id" r
    // Competitive-displacement questions the owner resolved on 2026-11-27 (PR #22 direction; OD-M-17..20).
    let cddResolved =
        Set.ofList [ "OD-CDD-01"; "OD-CDD-02"; "OD-CDD-03"; "OD-CDD-04"; "OD-CDD-05"; "UNB-CDD-CIV-008#Q1"; "UNB-CDD-CMF-007#Q1" ]
    let isOpenPackage (r: Json) = isCno r || (isCdd r && not (cddResolved.Contains(qid r)))
    // 316 up to 2026-11-25, 27 charity-network (open), 22 file-ingestion (answered or proposed),
    // 10 competitive-displacement decisions (OD-CDD-01..06 and four requirement questions)
    Assert.Equal(375, rows.Length)
    // The 316 questions registered up to 2026-11-25, the 22 file-ingestion questions and the seven owner-resolved
    // competitive-displacement questions are answered; the 27 charity-network questions and the three remaining
    // competitive-displacement questions (OD-CDD-06 and two counsel questions) are open.
    Assert.Equal(27, rows |> List.filter isCno |> List.length)
    Assert.Equal(10, rows |> List.filter isCdd |> List.length)
    Assert.True(rows |> List.filter (fun r -> cddResolved.Contains(qid r)) |> List.forall (fun r -> getStr "resolution_status" r = "resolved" && getStr "approval_status" r = "approved-owner"))
    Assert.True(rows |> List.filter (isOpenPackage >> not) |> List.forall (fun r -> strip (getStr "answer" r) <> ""))
    Assert.True(rows |> List.filter isOpenPackage |> List.forall (fun r -> getStr "resolution_status" r = "open"))

[<Fact>]
let ``hand-edited answer is not silently discarded`` () =
    let d = sandbox ()
    rewriteCsv (csvPath d) true (fun rows -> set "answer" (JStr "hand answer") rows.Head :: rows.Tail)
    let r = run d []
    Assert.True((r.Code = 3), r.Err)
    Assert.Contains("hand answer", readText (csvPath d))

[<Fact>]
let ``orphaned resolution fails`` () =
    let d = sandbox ()
    appendEvent d (event "QE-999999" "UNB-NOPE-001#Q1" "proposed" [ "answer", JStr "x" ])
    let r = run d []
    Assert.NotEqual(0, r.Code)
    Assert.Contains("unregistered-question", r.Err)

[<Fact>]
let ``duplicate event id fails`` () =
    let d = sandbox ()
    appendEvent d (event "QE-000001" "SPD-CFG-001" "proposed" [ "answer", JStr "x" ])
    let r = run d []
    Assert.NotEqual(0, r.Code)
    Assert.Contains("duplicate-event-id", r.Err)

[<Fact>]
let ``changed question text fails`` () =
    let d = sandbox ()
    rewriteCsv (simplificationDecisions d) false (List.map (fun r ->
        if getStr "decision_id" r = "SPD-CFG-001" then set "question" (JStr(getStr "question" r + " (edited)")) r else r))
    let r = run d []
    Assert.NotEqual(0, r.Code)
    Assert.Contains("changed-question-text", r.Err)

[<Fact>]
let ``invalid closure fails`` () =
    let d = sandbox ()
    appendEvent d (event "QE-999998" "SPD-CFG-001" "resolved" [ "resolution_status", JStr "resolved"; "disposition", JStr "external-authorization-required" ])
    let r = run d []
    Assert.NotEqual(0, r.Code)
    Assert.Contains("invalid-closure", r.Err)

[<Fact>]
let ``closed source decision stays registered`` () =
    let d = sandbox ()
    rewriteCsv (simplificationDecisions d) false (List.map (fun r ->
        if getStr "decision_id" r = "SPD-CFG-001" then set "status" (JStr "decided-provisional") r else r))
    let r = run d []
    Assert.True((r.Code = 0), r.Err)
    Assert.Contains("SPD-CFG-001", readCsv d |> List.map (getStr "question_id"))

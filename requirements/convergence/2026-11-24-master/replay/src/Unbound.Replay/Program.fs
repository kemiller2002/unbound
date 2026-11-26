/// Entry point. All file and network effects live here; everything it calls is pure.
///   fetch <dataDir> <returns.csv>                       download public inputs (idempotent)
///   run   <dataDir> <returns.csv> <out.json> <out.md>   compute the replay
///   synth <dataDir> <returns.csv> <parameters.json> <outDir> <out.md> [funds] [seed]   synthetic operating year
module Unbound.Replay.Program

open System
open System.IO
open System.IO.Compression
open System.Net.Http
open System.Security.Cryptography
open System.Xml.Linq
open Unbound.Replay.Domain
open Unbound.Replay.Analysis

/// Public inputs: local name, source URL, archive member to extract (if zipped).
let sources: (string * string * string option) list =
    [ "pub78.txt", "https://apps.irs.gov/pub/epostcard/data-download-pub78.zip", Some "data-download-pub78.txt"
      "revocation.txt", "https://apps.irs.gov/pub/epostcard/data-download-revocation.zip", Some "data-download-revocation.txt"
      "eo1.csv", "https://www.irs.gov/pub/irs-soi/eo1.csv", None
      "eo2.csv", "https://www.irs.gov/pub/irs-soi/eo2.csv", None
      "eo3.csv", "https://www.irs.gov/pub/irs-soi/eo3.csv", None
      "eo4.csv", "https://www.irs.gov/pub/irs-soi/eo4.csv", None
      "sdn.csv", "https://www.treasury.gov/ofac/downloads/sdn.csv", None
      "alt.csv", "https://www.treasury.gov/ofac/downloads/alt.csv", None ]

let xmlUrl (objectId: string) =
    $"https://gt990datalake-rawdata.s3.amazonaws.com/EfileData/XmlFiles/{objectId}_public.xml"

let xmlPath dataDir (objectId: string) = Path.Combine(dataDir, "xml", objectId + "_public.xml")

let private download (http: HttpClient) (url: string) (target: string) (member': string option) =
    if File.Exists target then
        printfn $"present  {target}"
    else
        printfn $"fetch    {url}"
        let bytes = http.GetByteArrayAsync(url).GetAwaiter().GetResult()

        match member' with
        | None -> File.WriteAllBytes(target, bytes)
        | Some m ->
            use zip = new ZipArchive(new MemoryStream(bytes))
            zip.GetEntry(m).ExtractToFile(target)

let fetch (dataDir: string) (returnsCsv: string) =
    Directory.CreateDirectory(Path.Combine(dataDir, "xml")) |> ignore
    use http = new HttpClient(Timeout = TimeSpan.FromMinutes 10.0)
    http.DefaultRequestHeaders.UserAgent.ParseAdd "unbound-public-data-replay/1.0"
    sources |> List.iter (fun (name, url, m) -> download http url (Path.Combine(dataDir, name)) m)

    File.ReadLines returnsCsv
    |> Sources.parseReturnList
    |> List.iter (fun (_, _, oid) -> download http (xmlUrl oid) (xmlPath dataDir oid) None)

let sha256 (path: string) =
    use s = File.OpenRead path
    SHA256.HashData(s) |> Convert.ToHexStringLower

let private lines dataDir name = File.ReadLines(Path.Combine(dataDir, name))
let private bmfLines dataDir = [ "eo1.csv"; "eo2.csv"; "eo3.csv"; "eo4.csv" ] |> Seq.collect (lines dataDir)

/// Deterministic samples for the safety test, taken in file order.
let private safetySamples dataDir =
    let pub78With code n =
        lines dataDir "pub78.txt" |> Seq.choose Sources.parsePub78 |> Seq.filter (fun e -> e.Codes = [ code ]) |> Seq.truncate n |> Seq.map (fun e -> e.Ein) |> List.ofSeq

    let nonC3 =
        bmfLines dataDir |> Seq.choose Sources.parseBmf |> Seq.filter (fun b -> b.Subsection <> "03" && b.Subsection <> "") |> Seq.truncate 400 |> Seq.map (fun b -> b.Ein) |> List.ofSeq

    let revoked =
        lines dataDir "revocation.txt"
        |> Seq.choose Sources.parseRevocation
        |> Seq.filter (fun r -> r.Reinstated.IsNone && r.Revoked.Year >= 2018)
        |> Seq.truncate 200
        |> Seq.map (fun r -> r.Ein)
        |> List.ofSeq

    let fabricated = [ for i in 1..200 -> sprintf "07%07d" (i * 4999) ]

    {| PF = pub78With "PF" 200; Sonfi = pub78With "SONFI" 200; PC = pub78With "PC" 400; NonC3 = nonC3; Revoked = revoked; Fabricated = fabricated |}

let private loadRegistry dataDir (needed: Set<string>) : Registry =
    let keep (ein: string) = needed.Contains ein

    { Pub78 = lines dataDir "pub78.txt" |> Seq.choose Sources.parsePub78 |> Seq.filter (fun e -> keep e.Ein) |> Seq.map (fun e -> e.Ein, e) |> Map.ofSeq
      Bmf =
        bmfLines dataDir
        |> Seq.filter (fun l -> l.Length > 9 && keep (l.Substring(0, 9)))
        |> Seq.choose Sources.parseBmf
        |> Seq.map (fun e -> e.Ein, e)
        |> Map.ofSeq
      Revocations =
        lines dataDir "revocation.txt"
        |> Seq.choose Sources.parseRevocation
        |> Seq.filter (fun e -> keep e.Ein)
        |> Seq.groupBy (fun e -> e.Ein)
        |> Seq.map (fun (k, es) -> k, List.ofSeq es)
        |> Map.ofSeq }

let private loadSdn dataDir =
    let primary = lines dataDir "sdn.csv" |> Seq.choose Sources.parseSdn |> List.ofSeq
    let kinds = primary |> List.map (fun n -> n.EntNum, n.Kind) |> Map.ofList
    let aliases = lines dataDir "alt.csv" |> Seq.choose (Sources.parseAlt kinds.TryFind) |> List.ofSeq
    primary, aliases

let private neutral (ein: string) = $"SAFETY CASE RECIPIENT {ein}"

let private case category expect ein name =
    { Category = category
      Expect = expect
      Recipient =
        { Name = name
          Ein = ein
          State = Some "OH"
          Foreign = false
          SectionDesc = ""
          Cash = 10000m
          NonCash = 0m } }

let run (dataDir: string) (returnsCsv: string) (outJson: string) (outMd: string) =
    let threshold = 0.92
    let thresholds = [ 0.85; 0.90; 0.92; 0.95; 0.98 ]

    let returns =
        File.ReadLines returnsCsv
        |> Sources.parseReturnList
        |> List.map (fun (_, _, oid) -> Sources.parseReturn oid (XDocument.Load(xmlPath dataDir oid)))

    returns |> List.iter (function Error e -> eprintfn $"skip {e}" | Ok _ -> ())
    let parsed = returns |> List.choose (function Ok r -> Some r | Error _ -> None)
    printfn $"returns  {parsed.Length} parsed"

    let samples = safetySamples dataDir
    let recipientEins = parsed |> List.collect (fun r -> r.Recipients |> List.choose (fun x -> x.Ein))

    let needed =
        Set.ofList (recipientEins @ samples.PF @ samples.Sonfi @ samples.PC @ samples.NonC3 @ samples.Revoked @ samples.Fabricated)

    let registry = loadRegistry dataDir needed
    printfn $"registry pub78={registry.Pub78.Count} bmf={registry.Bmf.Count} revoked={registry.Revocations.Count}"

    let primary, aliases = loadSdn dataDir
    let index = NameMatch.buildIndex 400 (primary @ aliases)
    printfn $"sdn      {primary.Length} primary, {aliases.Length} aliases, {index.Keys.Length} keys"

    let injected =
        primary
        |> List.filter (fun n -> n.Kind = Entity)
        |> List.indexed
        |> List.filter (fun (i, _) -> i % 15 = 0)
        |> List.truncate 500
        |> List.mapi (fun i (_, n) -> i, n.Name)

    let asOf =
        registry.Revocations |> Map.toSeq |> Seq.collect snd |> Seq.choose (fun r -> r.Posted) |> Seq.max

    let cases =
        List.concat
            [ samples.Revoked |> List.filter (fun e -> not (registry.Pub78.ContainsKey e)) |> List.truncate 100 |> List.map (fun e -> case "revoked_in_force" ExpectBlock (Some e) (neutral e))
              samples.PF |> List.filter (fun e -> not (registry.Revocations.ContainsKey e)) |> List.truncate 100 |> List.map (fun e -> case "private_foundation" ExpectPerson (Some e) (neutral e))
              samples.Sonfi |> List.filter (fun e -> not (registry.Revocations.ContainsKey e)) |> List.truncate 100 |> List.map (fun e -> case "type3_non_functionally_integrated" ExpectPerson (Some e) (neutral e))
              samples.NonC3
              |> List.filter (fun e -> not (registry.Pub78.ContainsKey e) && not (registry.Revocations.ContainsKey e))
              |> List.truncate 100
              |> List.map (fun e -> case "not_501c3" ExpectPerson (Some e) (neutral e))
              samples.Fabricated
              |> List.filter (fun e -> not (registry.Pub78.ContainsKey e || registry.Bmf.ContainsKey e))
              |> List.truncate 100
              |> List.map (fun e -> case "ein_not_in_irs_lists" ExpectPerson (Some e) (neutral e))
              [ for i in 1..100 -> case "no_ein" ExpectPerson None $"UNREGISTERED RECIPIENT {i}" ]
              injected
              |> List.truncate 100
              |> List.mapi (fun i (v, n) ->
                  let pc = samples.PC.[i]
                  case "listed_name_with_clean_public_charity_ein" ExpectPerson (Some pc) (NameMatch.mutate (1 + v % 4) n))
              samples.PC
              |> List.skip 100
              |> List.filter (fun e -> not (registry.Revocations.ContainsKey e))
              |> List.truncate 100
              |> List.map (fun e -> case "control_public_charity" ExpectStraightThrough (Some e) (neutral e)) ]

    let allNames =
        List.concat
            [ parsed |> List.collect (fun r -> r.Recipients |> List.map (fun x -> x.Name))
              cases |> List.map (fun c -> c.Recipient.Name)
              injected |> List.map (fun (v, n) -> NameMatch.mutate v n) ]
        |> List.distinct

    printfn $"scoring  {allNames.Length} names"
    let scorerOf (scorer: NameMatch.Scorer) =
        let scores = allNames |> Array.ofList |> Array.Parallel.map (fun n -> n, fst (NameMatch.best scorer index n)) |> Map.ofArray
        fun (n: string) -> scores.TryFind n |> Option.defaultWith (fun () -> fst (NameMatch.best scorer index n))

    let score = scorerOf NameMatch.weighted
    let naiveScore = scorerOf NameMatch.naive
    let currentNames = pairs parsed |> List.filter (fst >> itemises) |> List.collect (fun (r, _) -> r.Recipients |> List.map (fun x -> x.Name))

    let sponsors = sponsorResults registry threshold score parsed

    let inputs =
        (sources |> List.map (fun (n, u, _) -> n, u, sha256 (Path.Combine(dataDir, n))))
        @ (parsed |> List.sortBy (fun r -> r.ObjectId) |> List.map (fun r -> $"xml/{r.ObjectId}", xmlUrl r.ObjectId, sha256 (xmlPath dataDir r.ObjectId)))

    let results: Report.Results =
        { AsOf = asOf.ToString("yyyy-MM-dd")
          Threshold = threshold
          Inputs = inputs
          Returns = parsed.Length
          Sponsors = sponsors
          Pooled = pooled sponsors
          Classes = classBreakdown registry parsed
          RevocationLag = lines dataDir "revocation.txt" |> Seq.choose Sources.parseRevocation |> revocationLag (DateOnly(2020, 1, 1))
          Sanctions =
            [ sanctions "token_weighted" thresholds threshold score currentNames injected
              sanctions "naive_whole_string" thresholds threshold naiveScore currentNames injected ]
          Safety = safety registry threshold asOf score cases
          Calibration = calibration parsed }

    File.WriteAllText(outJson, Json.render (Report.toJson results))
    File.WriteAllText(outMd, Report.toMarkdown results)
    printfn $"wrote    {outJson}, {outMd}"

let private loadReturns dataDir returnsCsv =
    File.ReadLines returnsCsv
    |> Sources.parseReturnList
    |> List.choose (fun (_, _, oid) ->
        match Sources.parseReturn oid (XDocument.Load(xmlPath dataDir oid)) with
        | Ok r -> Some r
        | Error e -> eprintfn $"skip {e}"; None)

let synth (dataDir: string) (returnsCsv: string) (paramsPath: string) (outDir: string) (outMd: string) (funds: int) (seed: uint64) =
    let config = { Synthetic.defaultConfig with Funds = funds; Seed = seed }
    let p = Synthetic.parseParams "market" (File.ReadAllText paramsPath)
    let parsed = loadReturns dataDir returnsCsv

    // Recipient pool: every recipient of the itemising sponsors' latest returns, weighted by how many rows name it.
    let pool =
        pairs parsed
        |> List.filter (fst >> itemises)
        |> List.collect (fun (r, _) -> r.Recipients)
        |> List.groupBy recipientKey
        |> List.sortBy fst
        |> List.map (fun (_, rs) -> List.head rs, float rs.Length)
        |> Array.ofList

    let recipients = pool |> Array.map fst
    let cumulative = pool |> Array.map snd |> Synthetic.cumulate
    let samples = safetySamples dataDir
    let needed = Set.ofSeq (Seq.append (recipients |> Seq.choose (fun r -> r.Ein)) (samples.PF @ samples.PC @ samples.Revoked))
    let registry = loadRegistry dataDir needed
    let primary, aliases = loadSdn dataDir
    let index = NameMatch.buildIndex 400 (primary @ aliases)
    let neutralRecipient category ein name = (case category ExpectPerson ein name).Recipient

    let injections =
        [ "revoked", samples.Revoked |> List.filter (registry.Pub78.ContainsKey >> not) |> List.map (fun e -> neutralRecipient "" (Some e) (neutral e))
          "private_foundation", samples.PF |> List.map (fun e -> neutralRecipient "" (Some e) (neutral e))
          "sanctioned_name",
          primary
          |> List.filter (fun n -> n.Kind = Entity)
          |> List.indexed
          |> List.filter (fun (i, _) -> i % 37 = 0)
          |> List.truncate 100
          |> List.mapi (fun i (_, n) -> neutralRecipient "" (Some samples.PC.[i]) (NameMatch.mutate (1 + i % 4) n.Name)) ]
        |> List.map (fun (k, rs) -> k, Array.ofList rs)

    let fs = Synthetic.funds p config cumulative
    let evs = Synthetic.events p config recipients cumulative injections fs
    printfn $"synth    {fs.Length} funds, {evs.Length} events"

    let names =
        evs |> List.choose (fun e -> match e.Txn with Synthetic.Grant(r, _, _) -> Some r.Name | _ -> None) |> List.distinct |> Array.ofList

    let scores = names |> Array.Parallel.map (fun n -> n, fst (NameMatch.best NameMatch.weighted index n)) |> Map.ofArray
    let asOf = DateOnly(config.Year, 12, 31)
    let screenGrant = Screening.screen Standard 0.92 registry asOf (fun n -> scores.TryFind n |> Option.defaultValue 0.0)
    let items = Synthetic.workItems p config screenGrant fs evs
    let summary = Synthetic.summarize config evs items
    let sample = Synthetic.timeStudySample config 40 30 items
    Directory.CreateDirectory outDir |> ignore
    let write name (ls: string seq) = File.WriteAllLines(Path.Combine(outDir, name), ls)
    write "events.jsonl" (evs |> Seq.map (SyntheticReport.eventLine config.Year))
    write "work-items.jsonl" (items |> Seq.map (SyntheticReport.itemLine config.Year))
    write "time-study-sample.jsonl" (sample |> Seq.map (SyntheticReport.itemLine config.Year))
    write "time-study-answer-key.jsonl" (sample |> Seq.map SyntheticReport.answerKeyLine)
    let inputs = [ "parameters.json", sha256 paramsPath ] @ (sources |> List.map (fun (n, _, _) -> n, sha256 (Path.Combine(dataDir, n))))
    File.WriteAllText(Path.Combine(outDir, "summary.json"), Json.render (SyntheticReport.summaryJson config inputs summary))
    File.WriteAllText(outMd, SyntheticReport.toMarkdown config summary (sample |> List.countBy (fun w -> w.Kind)))
    printfn $"wrote    {outDir}, {outMd}"

[<EntryPoint>]
let main argv =
    match List.ofArray argv with
    | [ "fetch"; dataDir; returnsCsv ] -> fetch dataDir returnsCsv; 0
    | [ "run"; dataDir; returnsCsv; outJson; outMd ] -> run dataDir returnsCsv outJson outMd; 0
    | [ "synth"; dataDir; returnsCsv; paramsPath; outDir; outMd ] -> synth dataDir returnsCsv paramsPath outDir outMd 10000 20261125UL; 0
    | [ "synth"; dataDir; returnsCsv; paramsPath; outDir; outMd; funds; seed ] -> synth dataDir returnsCsv paramsPath outDir outMd (int funds) (uint64 seed); 0
    | _ ->
        eprintfn "usage: fetch <dataDir> <returns.csv> | run <dataDir> <returns.csv> <out.json> <out.md> | synth <dataDir> <returns.csv> <parameters.json> <outDir> <out.md> [funds seed]"
        2

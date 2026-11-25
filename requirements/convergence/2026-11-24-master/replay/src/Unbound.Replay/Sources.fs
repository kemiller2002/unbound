/// Pure parsers for the public sources. Each takes text already read by the caller and returns domain values.
module Unbound.Replay.Sources

open System
open System.Globalization
open System.Xml.Linq
open Unbound.Replay.Domain

let private digits (s: string) = String(s.Trim() |> Seq.filter Char.IsDigit |> Array.ofSeq)

/// Normalises an EIN to nine digits; zero or malformed EINs become None.
let normalizeEin (s: string) : string option =
    match digits s with
    | d when d.Length = 0 || d.Length > 9 -> None
    | d ->
        let padded = d.PadLeft(9, '0')
        if padded = "000000000" then None else Some padded

/// "15-NOV-2017" (revocation list format).
let parseIrsDate (s: string) : DateOnly option =
    match DateOnly.TryParseExact(s.Trim(), "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None) with
    | true, d -> Some d
    | _ -> None

/// Pub 78 line: EIN|Name|City|State|Country|Codes
let parsePub78 (line: string) : Pub78Entry option =
    match line.Split '|' with
    | [| ein; _; _; _; _; codes |] ->
        normalizeEin ein
        |> Option.map (fun e ->
            { Ein = e
              Codes = codes.Split(',', StringSplitOptions.RemoveEmptyEntries ||| StringSplitOptions.TrimEntries) |> List.ofArray })
    | _ -> None

/// Revocation line: EIN|Name|DBA|Street|City|State|ZIP|Country|ExemptionType|RevocationDate|PostingDate|ReinstatementDate
let parseRevocation (line: string) : RevocationEntry option =
    match line.Split '|' with
    | f when f.Length >= 12 ->
        match normalizeEin f.[0], parseIrsDate f.[9] with
        | Some e, Some revoked ->
            Some
                { Ein = e
                  Revoked = revoked
                  Posted = parseIrsDate f.[10]
                  Reinstated = parseIrsDate f.[11] }
        | _ -> None
    | _ -> None

/// EO BMF CSV row (header EIN,NAME,ICO,STREET,CITY,STATE,ZIP,GROUP,SUBSECTION,AFFILIATION,CLASSIFICATION,RULING,
/// DEDUCTIBILITY,FOUNDATION,...).
let parseBmf (line: string) : BmfEntry option =
    match Csv.split ',' line with
    | ein :: _ :: _ :: _ :: _ :: _ :: _ :: _ :: subsection :: _ :: _ :: _ :: deductibility :: foundation :: _ when ein <> "EIN" ->
        normalizeEin ein
        |> Option.map (fun e ->
            { Ein = e
              Subsection = subsection.Trim()
              Foundation = foundation.Trim()
              Deductibility = deductibility.Trim() })
    | _ -> None

let private sdnKind (t: string option) =
    match t |> Option.map (fun s -> s.ToLowerInvariant()) with
    | Some "individual" -> Individual
    | Some "vessel"
    | Some "aircraft" -> Conveyance
    | _ -> Entity

/// sdn.csv row: ent_num,"name","type",... ; returns the primary name.
let parseSdn (line: string) : SdnName option =
    match Csv.split ',' line with
    | num :: name :: kind :: _ ->
        match Int32.TryParse(num.Trim()), Csv.field name with
        | (true, n), Some nm ->
            Some
                { EntNum = n
                  Name = nm
                  Kind = sdnKind (Csv.field kind)
                  IsAlias = false }
        | _ -> None
    | _ -> None

/// alt.csv row: ent_num,alt_num,"alt_type","alt_name",remarks. Kind is resolved from the primary entry.
let parseAlt (kindOf: int -> SdnKind option) (line: string) : SdnName option =
    match Csv.split ',' line with
    | num :: _ :: _ :: name :: _ ->
        match Int32.TryParse(num.Trim()), Csv.field name with
        | (true, n), Some nm ->
            kindOf n
            |> Option.map (fun k ->
                { EntNum = n
                  Name = nm
                  Kind = k
                  IsAlias = true })
        | _ -> None
    | _ -> None

// ---- Form 990 XML ----

let private local (name: string) (e: XElement) = e.Name.LocalName = name

let private descendants name (e: XElement) = e.Descendants() |> Seq.filter (local name)

let private firstText name (e: XElement) =
    descendants name e |> Seq.tryHead |> Option.map (fun d -> d.Value.Trim())

let private firstDecimal name e =
    firstText name e
    |> Option.bind (fun s ->
        match Decimal.TryParse(s, NumberStyles.Number ||| NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture) with
        | true, v -> Some v
        | _ -> None)

let private firstInt name e =
    firstDecimal name e |> Option.map int

let private date name e =
    firstText name e
    |> Option.bind (fun s ->
        match DateOnly.TryParseExact(s, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None) with
        | true, d -> Some d
        | _ -> None)

let private parseRecipient (t: XElement) : Recipient =
    let name =
        [ "BusinessNameLine1Txt"; "BusinessNameLine2Txt" ]
        |> List.choose (fun n -> firstText n t)
        |> String.concat " "

    { Name = name
      Ein = firstText "RecipientEIN" t |> Option.bind normalizeEin
      State = firstText "StateAbbreviationCd" t
      Foreign = descendants "ForeignAddress" t |> Seq.isEmpty |> not
      SectionDesc = firstText "IRCSectionDesc" t |> Option.defaultValue ""
      Cash = firstDecimal "CashGrantAmt" t |> Option.defaultValue 0m
      NonCash = firstDecimal "NonCashAssistanceAmt" t |> Option.defaultValue 0m }

/// Parses a Form 990 e-file document. Returns Error with a reason when required header fields are absent.
let parseReturn (objectId: string) (doc: XDocument) : Result<SponsorReturn, string> =
    let root = doc.Root
    let header = descendants "ReturnHeader" root |> Seq.tryHead
    let irs990 = descendants "IRS990" root |> Seq.tryHead
    let schedD = descendants "IRS990ScheduleD" root |> Seq.tryHead
    let schedI = descendants "IRS990ScheduleI" root |> Seq.tryHead
    let from (scope: XElement option) f = scope |> Option.bind f

    match header with
    | None -> Error $"{objectId}: no ReturnHeader"
    | Some h ->
        let filer = descendants "Filer" h |> Seq.tryHead

        match from filer (firstText "EIN"), firstInt "TaxYr" h, date "TaxPeriodBeginDt" h, date "TaxPeriodEndDt" h with
        | Some ein, Some year, Some b, Some e ->
            Ok
                { ObjectId = objectId
                  Ein = ein
                  Name = from filer (firstText "BusinessNameLine1Txt") |> Option.defaultValue ""
                  TaxYear = year
                  PeriodBegin = b
                  PeriodEnd = e
                  Employees = from irs990 (firstInt "TotalEmployeeCnt")
                  DafCount = from schedD (firstInt "DonorAdvisedFundsHeldCnt")
                  DafGrants = from schedD (firstDecimal "DonorAdvisedFundsGrantsAmt")
                  DafContributions = from schedD (firstDecimal "DonorAdvisedFundsContriAmt")
                  DafAssets = from schedD (firstDecimal "DonorAdvisedFundsVlEOYAmt")
                  TotalExpenses = from irs990 (firstDecimal "CYTotalExpensesAmt")
                  GrantsPaid = from irs990 (firstDecimal "CYGrantsAndSimilarPaidAmt")
                  Salaries = from irs990 (firstDecimal "CYSalariesCompEmpBnftPaidAmt")
                  Recipients =
                    schedI
                    |> Option.map (descendants "RecipientTable" >> Seq.map parseRecipient >> List.ofSeq)
                    |> Option.defaultValue [] }
        | _ -> Error $"{objectId}: header lacks EIN, tax year or period"

/// returns.csv: ein,tax_year,object_id
let parseReturnList (lines: string seq) : (string * int * string) list =
    lines
    |> Seq.skip 1
    |> Seq.choose (fun l ->
        match l.Split ',' with
        | [| ein; year; oid |] ->
            match Int32.TryParse year with
            | true, y -> Some(ein.Trim(), y, oid.Trim())
            | _ -> None
        | _ -> None)
    |> List.ofSeq

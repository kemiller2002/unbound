/// Reference screening policy: a design artifact for measuring exception incidence, not legal advice. Class rules follow
/// IRC §4966(c)-(d) as summarised in the evidence dossiers; counsel confirms them before production (OD-P-LEG).
module Unbound.Replay.Screening

open System
open Unbound.Replay.Domain

let private has code (e: Pub78Entry) = List.contains code e.Codes

/// Class from Pub 78 codes; private-foundation and supporting-organization codes take precedence over other codes.
let private fromPub78 (e: Pub78Entry) : RecipientClass =
    [ "PF", PrivateFoundation
      "POF", PrivateOperatingFoundation
      "SONFI", SupportingOrgType3Nfi
      "SOUNK", SupportingOrgUnknown
      "SO", SupportingOrg
      "PC", PublicCharityPub78
      "GROUP", GroupCentral
      "FORGN", ForeignAddressedCharity
      "EO", Non501c3
      "LODGE", Non501c3 ]
    |> List.tryPick (fun (code, cls) -> if has code e then Some cls else None)
    |> Option.defaultValue UnknownStatus

/// Class from BMF subsection and foundation codes.
let private fromBmf (b: BmfEntry) : RecipientClass =
    match b.Subsection, b.Foundation with
    | s, _ when s <> "03" -> Non501c3
    | _, ("02" | "03") -> PrivateOperatingFoundation
    | _, "04" -> PrivateFoundation
    | _, ("21" | "22" | "23") -> SupportingOrg
    | _, "24" -> SupportingOrgType3Nfi
    | _, "17" -> SupportingOrgUnknown
    | _, ("00" | "09" | "") -> UnknownStatus
    | _ -> PublicCharityBmf

let private mentions (words: string list) (s: string) =
    let u = s.ToUpperInvariant()
    words |> List.exists u.Contains

let private hasToken (words: Set<string>) (name: string) =
    NameMatch.tokens name |> List.exists words.Contains

/// Names of houses of worship, which need not apply for recognition and are often absent from IRS lists.
let religiousName =
    hasToken (set [ "CHURCH"; "PARISH"; "CONGREGATION"; "SYNAGOGUE"; "TEMPLE"; "MOSQUE"; "MASJID"; "DIOCESE"; "CHAPEL"; "MINISTRIES"; "IGLESIA"; "BAPTIST"; "LUTHERAN"; "METHODIST"; "PRESBYTERIAN"; "EPISCOPAL"; "CATHOLIC" ])

/// Names of government units (IRC §170(c)(1)), which are not on IRS exempt-organization lists.
let governmentName (name: string) =
    let ts =
        name.ToUpperInvariant()
        |> String.map (fun c -> if Char.IsLetterOrDigit c then c else ' ')
        |> fun t -> t.Split(' ', StringSplitOptions.RemoveEmptyEntries)
        |> List.ofArray

    (ts |> List.exists (fun t -> t = "CITY" || t = "TOWN" || t = "COUNTY" || t = "VILLAGE" || t = "TOWNSHIP" || t = "BOROUGH"))
    && (ts |> List.exists (fun t -> t = "OF" || t = "SCHOOLS" || t = "DISTRICT"))
    || (ts |> List.contains "SCHOOL" && ts |> List.contains "DISTRICT")
    || (ts |> List.contains "PUBLIC" && ts |> List.contains "SCHOOLS")
    || (ts |> List.contains "CITY" && ts |> List.contains "SCHOOLS")

/// Eligibility class of a recipient, from IRS public data first and the sponsor's own label only when the EIN is absent
/// from every IRS list.
let classify (registry: Registry) (r: Recipient) : RecipientClass =
    match r.Ein with
    | None -> NoEin
    | Some ein ->
        match registry.Pub78.TryFind ein, registry.Bmf.TryFind ein with
        | Some p, _ -> fromPub78 p
        | None, Some b -> fromBmf b
        | None, None when mentions [ "GOVERN"; "170(C)(1)" ] r.SectionDesc || governmentName r.Name -> GovernmentUnverified
        | None, None when mentions [ "RELIGIOUS"; "CHURCH" ] r.SectionDesc || religiousName r.Name -> ReligiousUnlisted
        | None, None -> UnknownEin

/// Classes that pass without a person under each policy.
let straightThrough (policy: Policy) (c: RecipientClass) : bool =
    match policy, c with
    | _, (PublicCharityPub78 | PublicCharityBmf) -> true
    | Standard, (GroupCentral | ForeignAddressedCharity | PrivateOperatingFoundation) -> true
    | _ -> false

/// A revocation in force on `asOf`: effective on or before it and not reinstated by it.
let revocationInForce (asOf: DateOnly) (entries: RevocationEntry list) : RevocationEntry option =
    entries
    |> List.filter (fun r -> r.Revoked <= asOf && (r.Reinstated |> Option.forall (fun d -> d > asOf)))
    |> List.sortByDescending (fun r -> r.Revoked)
    |> List.tryHead

/// Revocation in force on `asOf`, unless the EIN is listed in Pub 78 today. The revocation file often omits
/// reinstatement dates; current Pub 78 listing is evidence of reinstatement (data limitation recorded in report 14).
let revokedAt (registry: Registry) (asOf: DateOnly) (ein: string) : RevocationEntry option =
    if registry.Pub78.ContainsKey ein then
        None
    else
        registry.Revocations.TryFind ein |> Option.bind (revocationInForce asOf)

let classReason (c: RecipientClass) : string =
    match c with
    | PrivateFoundation -> "private_foundation_expenditure_responsibility"
    | PrivateOperatingFoundation -> "private_operating_foundation"
    | SupportingOrg -> "supporting_org_control_check"
    | SupportingOrgType3Nfi -> "type3_non_functionally_integrated"
    | SupportingOrgUnknown -> "supporting_org_type_unknown"
    | Non501c3 -> "not_501c3"
    | UnknownStatus -> "irs_status_unknown"
    | GovernmentUnverified -> "government_unit_unverified"
    | ReligiousUnlisted -> "religious_org_not_listed"
    | UnknownEin -> "ein_not_in_irs_lists"
    | NoEin -> "no_ein"
    | GroupCentral -> "group_central_org"
    | ForeignAddressedCharity -> "foreign_addressed_charity"
    | PublicCharityPub78
    | PublicCharityBmf -> "public_charity"

/// Routes one recipient: revocation in force blocks; a sanctions score at or above `threshold` is an exception;
/// otherwise the class decides.
let screen
    (policy: Policy)
    (threshold: float)
    (registry: Registry)
    (asOf: DateOnly)
    (sanctionsScore: string -> float)
    (r: Recipient)
    : Screened =
    let cls = classify registry r

    let revoked = r.Ein |> Option.bind (revokedAt registry asOf)

    let score = sanctionsScore r.Name

    let route =
        match revoked with
        | Some _ -> Block "revoked"
        | None when score >= threshold -> Exception "sanctions_alert"
        | None when straightThrough policy cls -> StraightThrough
        | None -> Exception(classReason cls)

    { Recipient = r
      Class = cls
      Revoked = revoked
      SanctionsScore = score
      Route = route }

let isStraightThrough (s: Screened) =
    match s.Route with
    | StraightThrough -> true
    | _ -> false

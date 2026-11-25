/// Immutable domain types for the public-data replay. No behaviour lives here.
module Unbound.Replay.Domain

open System

/// IRS Publication 78 entry: EIN and deductibility status codes (PC, PF, POF, SO, SONFI, SOUNK, EO, LODGE, GROUP, FORGN, UNKWN).
type Pub78Entry = { Ein: string; Codes: string list }

/// IRS Exempt Organizations Business Master File entry (the fields the screening policy reads).
type BmfEntry = { Ein: string; Subsection: string; Foundation: string; Deductibility: string }

/// IRS automatic-revocation list entry.
type RevocationEntry =
    { Ein: string
      Revoked: DateOnly
      Posted: DateOnly option
      Reinstated: DateOnly option }

type SdnKind =
    | Individual
    | Entity
    | Conveyance

/// One OFAC SDN name: a primary name or an alias of entry EntNum.
type SdnName = { EntNum: int; Name: string; Kind: SdnKind; IsAlias: bool }

/// One Schedule I recipient row (grants over $5,000, aggregated per recipient for the tax year).
type Recipient =
    { Name: string
      Ein: string option
      State: string option
      Foreign: bool
      SectionDesc: string
      Cash: decimal
      NonCash: decimal }

/// The fields of one sponsor Form 990 that the replay uses.
type SponsorReturn =
    { ObjectId: string
      Ein: string
      Name: string
      TaxYear: int
      PeriodBegin: DateOnly
      PeriodEnd: DateOnly
      Employees: int option
      DafCount: int option
      DafGrants: decimal option
      DafContributions: decimal option
      DafAssets: decimal option
      TotalExpenses: decimal option
      GrantsPaid: decimal option
      Salaries: decimal option
      Recipients: Recipient list }

/// Eligibility class of a recipient as determined from IRS public data (not from the sponsor's own label).
type RecipientClass =
    | PublicCharityPub78
    | PublicCharityBmf
    | GroupCentral
    | ForeignAddressedCharity
    | PrivateOperatingFoundation
    | PrivateFoundation
    | SupportingOrg
    | SupportingOrgType3Nfi
    | SupportingOrgUnknown
    | Non501c3
    | UnknownStatus
    | GovernmentUnverified
    | ReligiousUnlisted
    | UnknownEin
    | NoEin

type Route =
    | StraightThrough
    | Exception of reason: string
    | Block of reason: string

/// Standard: statutory reading used by the economics model. Conservative: also routes classes that a cautious sponsor
/// would review (private operating foundations, group central organizations, foreign-addressed charities).
type Policy =
    | Standard
    | Conservative

/// Public registry restricted to the EINs the replay needs.
type Registry =
    { Pub78: Map<string, Pub78Entry>
      Bmf: Map<string, BmfEntry>
      Revocations: Map<string, RevocationEntry list> }

/// A recipient row after screening.
type Screened =
    { Recipient: Recipient
      Class: RecipientClass
      Revoked: RevocationEntry option
      SanctionsScore: float
      Route: Route }

/// ia.py: information architecture. Maps every derived surface to a workspace and panel and verifies complete coverage.
/// Run from the repository root (no arguments).
module Unbound.ReqAudit.Ia

open System
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat

let D = "requirements/reconciliation/2026-11-23/data"

/// workspace id: (name, audience, responsive class)
let WS =
    [
      "WS-01", ("Giving home and fund", "Donor, advisor, successor", "phone-complete")
      "WS-02", ("Grant recommendation and grants", "Donor, advisor", "phone-complete")
      "WS-03", ("Contributions", "Donor, advisor", "phone-complete")
      "WS-04", ("People, access and succession", "Donor, representatives", "phone-complete")
      "WS-05", ("Documents and notices", "Donor, advisor, charity", "phone-complete")
      "WS-06", ("Fund opening and lifecycle requests", "Applicant, donor", "phone-complete")
      "WS-07", ("Charity portal", "Charity finance user", "phone-complete")
      "WS-08", ("Help and support status", "Donor, advisor, charity", "phone-complete")
      "WS-09", ("Exception inbox", "Sponsor operations, compliance, finance, treasury", "phone-review-and-approve")
      "WS-10", ("Case workspace with typed decision panels", "Reviewers and approvers", "desktop-first; approve/decline on phone")
      "WS-11", ("Grant approval", "Grants approver", "phone-review-and-approve")
      "WS-12", ("Treasury and payments", "Treasury", "desktop-first")
      "WS-13", ("Fund accounting and close", "Controller, fund accountant", "desktop-first")
      "WS-14", ("Regulatory reporting", "Controller, CFO, signer", "desktop-first")
      "WS-15", ("Records and retention", "Records owner, counsel", "desktop-first")
      "WS-16", ("Fund lifecycle operations", "Sponsor operations", "desktop-first")
      "WS-17", ("Sponsor implementation and activation", "Implementation lead, sponsor officer", "desktop-first")
      "WS-18", ("Migration", "Migration lead, controller", "desktop-first")
      "WS-19", ("Sponsor policy and configuration", "Sponsor administrator, approver, platform risk", "desktop-first")
      "WS-20", ("Support console", "Support agent", "desktop-first")
      "WS-21", ("Sponsor overview", "Sponsor executive", "phone-review-and-approve")
      "WS-22", ("Embedded status widget", "Partner host end users", "phone-complete")
    ]

let MAP =
    [
      "UNB-UI-S-001", ("WS-01", "Giving home")
      "UNB-UI-S-002", ("WS-02", "Charity search")
      "UNB-UI-S-003", ("WS-02", "Recommend a grant")
      "UNB-UI-S-004", ("WS-02", "Grant detail")
      "UNB-UI-S-005", ("WS-03", "Add money and contribution status")
      "UNB-UI-S-006", ("WS-04", "Participants and successors")
      "UNB-UI-S-007", ("WS-05", "Document center")
      "UNB-UI-S-013", ("WS-05", "Notification preferences")
      "UNB-UI-S-012", ("WS-07", "Grants received and destinations")
      "UNB-UI-S-008", ("WS-09", "Inbox")
      "UNB-UI-S-009", ("WS-10", "Case frame")
      "UNB-UI-S-010", ("WS-19", "Policy draft, diff and approval")
      "UNB-UI-S-011", ("WS-21", "Outcomes at risk")
      "UNB-UI-W-001", ("WS-22", "Grant status card")
      "NEW:Recurring schedule management", ("WS-02", "Recurring schedules")
      "NEW:Invitation acceptance", ("WS-04", "Accept invitation")
      "NEW:Representative evidence upload", ("WS-04", "Representative evidence")
      "NEW:Verified-channel confirmation", ("WS-04", "Confirm a change on your verified channel")
      "NEW:Fund application", ("WS-06", "Open a fund")
      "NEW:Fund closure request", ("WS-06", "Close a fund")
      "NEW:Outbound transfer request", ("WS-06", "Transfer to another sponsor")
      "NEW:Support case status", ("WS-08", "My requests and appeals")
      "NEW:Charity hold decision panel", ("WS-10", "Panel: charity hold")
      "NEW:Charity identity resolution", ("WS-10", "Panel: charity identity")
      "NEW:Charity alternate evidence review", ("WS-10", "Panel: alternate eligibility evidence")
      "NEW:Recipient class counsel review", ("WS-10", "Panel: recipient class (counsel)")
      "NEW:Supporting organization certification", ("WS-10", "Panel: supporting-organization certification")
      "NEW:Related-party hold decision", ("WS-10", "Panel: related-party payee")
      "NEW:Sanctions match decision panel", ("WS-10", "Panel: sanctions potential match")
      "NEW:Unknown payment resolution", ("WS-10", "Panel: unknown payment")
      "NEW:Positive pay exception decision", ("WS-10", "Panel: positive-pay exception")
      "NEW:Destination change verification", ("WS-10", "Panel: destination verification")
      "NEW:Gift acceptance decision", ("WS-10", "Panel: gift acceptance")
      "NEW:Price exception review", ("WS-10", "Panel: price exception")
      "NEW:Fee waiver decision", ("WS-10", "Panel: fee waiver")
      "NEW:Liquidity advance approval", ("WS-10", "Panel: liquidity advance")
      "NEW:Succession allocation review", ("WS-10", "Panel: succession allocation")
      "NEW:Charitable default disposition", ("WS-10", "Panel: charitable default")
      "NEW:Fund application review", ("WS-10", "Panel: fund application")
      "NEW:Platform policy safety review", ("WS-10", "Panel: platform-safety policy change")
      "NEW:Grant batch approval", ("WS-11", "Batch approval")
      "NEW:Payment release approval", ("WS-12", "Manual release approval")
      "NEW:Check void and reissue", ("WS-12", "Checks: void and reissue")
      "NEW:Unclaimed property disposition", ("WS-12", "Checks: unclaimed property")
      "NEW:OFAC blocked property report", ("WS-12", "OFAC reports")
      "NEW:NAV review and approval", ("WS-13", "Unit pricing")
      "NEW:Pool liquidity buffer", ("WS-13", "Pool liquidity")
      "NEW:Reconciliation attestation review", ("WS-13", "Reconciliation and attestation")
      "NEW:Month-end close certification", ("WS-13", "Close certification")
      "NEW:Form 8283 donee signature", ("WS-13", "Noncash forms: 8283")
      "NEW:Form 8282 obligation tracker", ("WS-13", "Noncash forms: 8282")
      "NEW:Annual DAF reporting workspace", ("WS-14", "Annual DAF package")
      "NEW:Account-type classification register", ("WS-14", "Account-type classification")
      "NEW:Legal hold management", ("WS-15", "Legal holds")
      "NEW:Retention disposition batch approval", ("WS-15", "Disposition batches")
      "NEW:Fund closure workspace", ("WS-16", "Closures and transfers")
      "NEW:Sponsor contract and activation record", ("WS-17", "Contract and activation record")
      "NEW:Board delegation register", ("WS-17", "Board delegations")
      "NEW:Tenant provisioning console", ("WS-17", "Tenant provisioning")
      "NEW:Sponsor implementation readiness checklist", ("WS-17", "Readiness checklist")
      "NEW:Opening balance tie-out workspace", ("WS-17", "Opening balance tie-out")
      "NEW:Sponsor activation gate", ("WS-17", "Activation gate")
      "NEW:Migration control console", ("WS-18", "Migration runs")
      "NEW:Migration mapping review", ("WS-18", "Mapping review")
      "NEW:Migration cutover authorization", ("WS-18", "Cutover authorization")
      "NEW:Imported payee verification queue", ("WS-18", "Imported payee verification")
      "NEW:Assisted change console", ("WS-20", "Assisted change")
    ]

let private ws id = WS |> List.find (fst >> (=) id) |> snd
let private mapped surface = MAP |> List.tryFind (fst >> (=) surface) |> Option.map snd

/// Rows of surface-ia.csv plus the missing and extra surface lists.
let plan (inv: Json list) : string list list * string list * string list =
    let surfaces = inv |> List.map (s "surface")
    let missing = surfaces |> List.filter (fun sfc -> (mapped sfc).IsNone)
    let extra = MAP |> List.map fst |> List.filter (fun k -> not (List.contains k surfaces))
    let firstOr (col: string) (dflt: string) (sfc: string) =
        inv |> List.tryFind (fun r -> s "surface" r = sfc) |> Option.map (s col) |> Option.defaultValue dflt
    let rows =
        MAP
        |> List.sortWith (fun (k1, (w1, _)) (k2, (w2, _)) -> match cmpStr w1 w2 with 0 -> cmpStr k1 k2 | c -> c)
        |> List.map (fun (sfc, (wid, panel)) ->
            let name, audience, responsive = ws wid
            [ sfc; wid; name; panel; audience; responsive; firstOr "workflows" "" sfc; firstOr "max_authority" "A0" sfc; firstOr "transition_count" "0" sfc ])
    rows, missing, extra

let main (_: string list) : int =
    let inv = dictRows (readText $"{D}/surface-inventory.csv")
    let rows, missing, extra = plan inv
    writeText $"{D}/surface-ia.csv"
        (PyCsv.format PyCsv.QuoteAll ([ "surface"; "workspace_id"; "workspace"; "panel_or_view"; "audience"; "responsive"; "workflows"; "max_authority"; "transitions" ] :: rows))
    let workspaces = rows |> List.map (fun r -> r.[1]) |> List.distinct |> List.length
    Io.print $"surfaces {inv.Length} mapped {rows.Length} workspaces {workspaces} missing {reprList missing} extra {reprList extra}"
    if missing.IsEmpty then 0 else 1

/// Pure rendering of replay results to JSON and Markdown.
module Unbound.Replay.Report

open System
open System.Globalization
open Unbound.Replay.Json
open Unbound.Replay.Analysis
open Unbound.Replay.Stats

type Results =
    { AsOf: string
      Threshold: float
      Inputs: (string * string * string) list // name, source URL, sha256
      Returns: int
      Sponsors: SponsorResult list
      Pooled: Pooled
      Classes: (string * int * decimal) list
      RevocationLag: Summary
      Sanctions: SanctionsResult list // operating method first
      Safety: SafetyResult list
      Calibration: Calibration list }

let private summaryJson (s: Summary) =
    JObj [ "n", JInt(int64 s.N); "min", JNum s.Min; "p50", JNum s.P50; "max", JNum s.Max; "mean", JNum s.Mean ]

let private mapJson (f: 'v -> Json) (m: Map<string, 'v>) =
    JObj(m |> Map.toList |> List.map (fun (k, v) -> k, f v))

let toJson (r: Results) : Json =
    JObj
        [ "as_of", JStr r.AsOf
          "sanctions_operating_threshold", JNum r.Threshold
          "inputs", JArr(r.Inputs |> List.map (fun (n, u, h) -> JObj [ "name", JStr n; "source", JStr u; "sha256", JStr h ]))
          "returns_parsed", JInt(int64 r.Returns)
          "pooled",
          JObj
              [ "sponsors", JInt(int64 r.Pooled.Sponsors)
                "schedule_i_rows", JInt(int64 r.Pooled.Rows)
                "schedule_i_dollars", JDec r.Pooled.Dollars
                "exception_share_rows", mapJson JNum r.Pooled.ExceptionShare
                "exception_share_dollars", mapJson JNum r.Pooled.ExceptionDollarShare
                "per_sponsor_exception_share_standard", summaryJson r.Pooled.PerSponsorExceptionShare
                "determination_reuse_within_sponsor", JNum r.Pooled.ReuseWithinSponsor
                "determination_reuse_within_sponsor_dollars", JNum r.Pooled.ReuseWithinSponsorDollars
                "determination_reuse_any_sponsor_in_sample", JNum r.Pooled.ReuseAnySponsor
                "recipient_overlap_with_prior_year", JNum r.Pooled.RecipientOverlapPrior
                "revoked_during_year_per_1000_rows", JNum r.Pooled.RevokedDuringYearPer1000
                "granted_while_revoked_rows", JInt(int64 r.Pooled.GrantedWhileRevoked)
                "granted_while_revoked_visible_in_year", JInt(int64 r.Pooled.GrantedWhileRevokedVisible)
                "sponsor_label_mismatch_rows", JInt(int64 r.Pooled.LabelMismatch)
                "sanctions_alerts_per_1000_rows", JNum r.Pooled.SanctionsAlertsPer1000Rows ]
          "classes", JArr(r.Classes |> List.map (fun (c, n, d) -> JObj [ "class", JStr c; "rows", JInt(int64 n); "dollars", JDec d ]))
          "revocation_posting_lag_days", summaryJson r.RevocationLag
          "sanctions",
          JArr(
              r.Sanctions
              |> List.map (fun x ->
                  JObj
                      [ "method", JStr x.Method
                        "distinct_recipient_names", JInt(int64 x.DistinctNames)
                        "alerts_by_threshold", JArr(x.AlertsByThreshold |> List.map (fun (t, n) -> JObj [ "threshold", JNum t; "alerts", JInt(int64 n) ]))
                        "injected_listed_names", JInt(int64 x.InjectedNames)
                        "recall_by_threshold", JArr(x.RecallByThreshold |> List.map (fun (t, v) -> JObj [ "threshold", JNum t; "recall", JNum v ]))
                        "recall_by_variant_at_operating_threshold", JObj(x.RecallByVariant |> List.map (fun (v, y) -> v, JNum y))
                        "subgroup_names", JInt(int64 x.SubgroupNames)
                        "subgroup_alert_rate", JNum x.SubgroupAlertRate
                        "other_alert_rate", JNum x.OtherAlertRate ])
          )
          "safety",
          JArr(
              r.Safety
              |> List.map (fun s ->
                  JObj [ "category", JStr s.Category; "expected", JStr s.Expected; "cases", JInt(int64 s.Cases); "caught", JInt(int64 s.Caught); "correct_route", JInt(int64 s.CorrectRoute) ])
          )
          "sponsors",
          JArr(
              r.Sponsors
              |> List.map (fun s ->
                  JObj
                      [ "ein", JStr s.Ein
                        "name", JStr s.Name
                        "tax_year", JInt(int64 s.TaxYear)
                        "prior_year", opt (int64 >> JInt) s.PriorYear
                        "rows", JInt(int64 s.Rows)
                        "rows_with_ein", JInt(int64 s.RowsWithEin)
                        "dollars", JDec s.Dollars
                        "exception_rows", mapJson (int64 >> JInt) s.ExceptionRows
                        "exception_dollars", mapJson JDec s.ExceptionDollars
                        "blocked", JInt(int64 s.Blocked)
                        "sanctions_alerts", JInt(int64 s.SanctionsAlerts)
                        "reuse_within_sponsor", opt JNum s.ReuseWithinSponsor
                        "reuse_within_sponsor_dollars", opt JNum s.ReuseWithinSponsorDollars
                        "reuse_any_sponsor", JNum s.ReuseAnySponsor
                        "recipient_overlap_prior", opt JNum s.RecipientOverlapPrior
                        "revoked_during_year", JInt(int64 s.RevokedDuringYear) ])
          )
          "calibration",
          JArr(
              r.Calibration
              |> List.map (fun c ->
                  JObj
                      [ "ein", JStr c.Ein
                        "name", JStr c.Name
                        "tax_year", JInt(int64 c.TaxYear)
                        "daf_count", opt (int64 >> JInt) c.DafCount
                        "employees", opt (int64 >> JInt) c.Employees
                        "total_expenses", opt JDec c.TotalExpenses
                        "grants_paid", opt JDec c.GrantsPaid
                        "salaries", opt JDec c.Salaries
                        "daf_grants", opt JDec c.DafGrants
                        "daf_assets", opt JDec c.DafAssets
                        "schedule_i_rows", JInt(int64 c.ScheduleIRows)
                        "funds_per_employee", opt JNum c.FundsPerEmployee
                        "non_grant_expense_per_fund", opt JNum c.NonGrantExpensePerFund
                        "salary_per_fund", opt JNum c.SalaryPerFund ])
          ) ]

// Per-sponsor counts of grants to revoked recipients and of label mismatches are deliberately not emitted: they are
// unverified (EIN typos, reinstatements absent from the data) and must not read as findings against a named sponsor.

// ---- Markdown ----

let private pct (x: float) = (x * 100.0).ToString("0.0", CultureInfo.InvariantCulture) + "%"
let private f1 (x: float) = x.ToString("0.0", CultureInfo.InvariantCulture)
let private n0 (x: float) = x.ToString("#,0", CultureInfo.InvariantCulture)
let private usd (x: decimal) = "$" + x.ToString("#,0", CultureInfo.InvariantCulture)
let private optS f o = o |> Option.map f |> Option.defaultValue "—"

let private table (header: string list) (rows: string list list) =
    let line cells = "| " + String.concat " | " cells + " |"
    [ line header; line (header |> List.map (fun _ -> "---")) ] @ (rows |> List.map line)

/// Generated tables (report 14a). Narrative and interpretation live in report 14, written by hand.
let toMarkdown (r: Results) : string =
    let p = r.Pooled

    [ [ "# 14a Public-data replay — generated tables"
        ""
        $"Generated by `replay/` (F#) as of {r.AsOf}. Do not edit by hand; rerun `dotnet run --project replay/src/Unbound.Replay -- run …`."
        "Interpretation and limits are in [report 14](14-PUBLIC-DATA-REPLAY.md)."
        ""
        "## 1. Pooled results"
        "" ]
      table
          [ "Measure"; "Value" ]
          [ [ "Sponsors with itemised Schedule I"; string p.Sponsors ]
            [ "Schedule I recipient rows (latest year)"; n0 (float p.Rows) ]
            [ "Schedule I dollars"; usd p.Dollars ]
            [ "Rows routed to a person, standard policy"; pct p.ExceptionShare.["standard"] ]
            [ "Rows routed to a person, conservative policy"; pct p.ExceptionShare.["conservative"] ]
            [ "Dollars routed to a person, standard / conservative"
              pct p.ExceptionDollarShare.["standard"] + " / " + pct p.ExceptionDollarShare.["conservative"] ]
            [ "Per-sponsor exception share (standard): min / median / max"
              $"{pct p.PerSponsorExceptionShare.Min} / {pct p.PerSponsorExceptionShare.P50} / {pct p.PerSponsorExceptionShare.Max}" ]
            [ "Exception rows whose recipient the same sponsor granted to in the prior year"; pct p.ReuseWithinSponsor ]
            [ "… dollar-weighted"; pct p.ReuseWithinSponsorDollars ]
            [ "Exception rows seen at this or any sampled sponsor (either year)"; pct p.ReuseAnySponsor ]
            [ "All rows: recipient also granted in prior year"; pct p.RecipientOverlapPrior ]
            [ "Recipients revoked during the grant year, per 1,000 rows"; f1 p.RevokedDuringYearPer1000 ]
            [ "Rows granted to a recipient already revoked at year start (as now known)"; string p.GrantedWhileRevoked ]
            [ "… of which the revocation was posted by year end"; string p.GrantedWhileRevokedVisible ]
            [ "Rows where the sponsor's label says public charity but IRS data says PF, Type III NFI or non-501(c)(3)"; string p.LabelMismatch ]
            [ $"Sanctions alerts per 1,000 rows at threshold {r.Threshold}"; f1 p.SanctionsAlertsPer1000Rows ] ]
      [ ""; "## 2. Recipient classes (pooled, IRS data)"; "" ]
      table [ "Class"; "Rows"; "Share"; "Dollars" ] (r.Classes |> List.map (fun (c, n, d) -> [ c; n0 (float n); pct (share (float n) (float p.Rows)); usd d ]))
      [ ""; "## 3. Per sponsor"; "" ]
      table
          [ "Sponsor"; "TY"; "Rows"; "Exc. std"; "Exc. cons."; "Reuse (own prior yr)"; "Reuse (any)"; "Overlap prior"; "Revoked in yr" ]
          (r.Sponsors
           |> List.map (fun s ->
               [ s.Name
                 string s.TaxYear
                 n0 (float s.Rows)
                 pct (share (float s.ExceptionRows.["standard"]) (float s.Rows))
                 pct (share (float s.ExceptionRows.["conservative"]) (float s.Rows))
                 optS pct s.ReuseWithinSponsor
                 pct s.ReuseAnySponsor
                 optS pct s.RecipientOverlapPrior
                 string s.RevokedDuringYear ]))
      [ ""
        "## 4. Revocation posting lag"
        ""
        $"Automatic revocations effective since 2020: n = {n0 (float r.RevocationLag.N)}, median {f1 r.RevocationLag.P50} days, "
        + $"mean {f1 r.RevocationLag.Mean} days, max {f1 r.RevocationLag.Max} days from effective date to IRS posting."
        ""
        "## 5. Sanctions name screening"
        ""
        $"{n0 (float r.Sanctions.Head.DistinctNames)} distinct recipient names screened against OFAC SDN primary names and aliases "
        + $"(vessels and aircraft excluded); {r.Sanctions.Head.InjectedNames} listed entity names injected with deterministic perturbations. "
        + $"Routing uses method `{r.Sanctions.Head.Method}` at threshold {r.Threshold}."
        "" ]
      r.Sanctions
      |> List.collect (fun x ->
          [ $"### Method `{x.Method}`"; "" ]
          @ table
              [ "Threshold"; "Alerts on real recipients"; "Alerts per 1,000 names"; "Recall on injected names" ]
              (List.zip x.AlertsByThreshold x.RecallByThreshold
               |> List.map (fun ((t, a), (_, rc)) ->
                   [ t.ToString("0.00", CultureInfo.InvariantCulture); string a; f1 (1000.0 * share (float a) (float x.DistinctNames)); pct rc ]))
          @ [ ""; $"Recall by perturbation at {r.Threshold}: " + (x.RecallByVariant |> List.map (fun (v, y) -> $"{v} {pct y}") |> String.concat ", ") + "."
              ""
              $"Alert rate at {r.Threshold} for the {n0 (float x.SubgroupNames)} names with Arabic-origin or Islamic tokens: {pct x.SubgroupAlertRate}; "
              + $"all other names: {pct x.OtherAlertRate}."
              "" ])
      [ ""; "## 6. Safety test (injected bad cases, standard policy)"; "" ]
      table
          [ "Category"; "Expected route"; "Cases"; "Left straight-through path"; "Correct route" ]
          (r.Safety |> List.map (fun s -> [ s.Category; s.Expected; string s.Cases; string s.Caught; string s.CorrectRoute ]))
      [ ""; "## 7. Incumbent calibration (whole-organisation Form 990 figures)"; "" ]
      table
          [ "Sponsor"; "TY"; "DAFs"; "Employees"; "DAFs per employee"; "Non-grant expense per DAF"; "Salary per DAF"; "Sched. I rows" ]
          (r.Calibration
           |> List.map (fun c ->
               [ c.Name
                 string c.TaxYear
                 optS (float >> n0) c.DafCount
                 optS (float >> n0) c.Employees
                 optS f1 c.FundsPerEmployee
                 optS (fun x -> "$" + n0 x) c.NonGrantExpensePerFund
                 optS (fun x -> "$" + n0 x) c.SalaryPerFund
                 n0 (float c.ScheduleIRows) ]))
      [ ""; "## 8. Inputs"; "" ]
      table [ "Input"; "Source"; "SHA-256" ] (r.Inputs |> List.map (fun (n, u, h) -> [ n; u; "`" + h.Substring(0, 16) + "…`" ])) ]
    |> List.concat
    |> String.concat "\n"
    |> fun s -> s + "\n"

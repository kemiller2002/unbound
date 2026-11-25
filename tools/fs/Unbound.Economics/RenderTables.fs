/// Render enterprise-results.json into Markdown tables (pure): port of
/// requirements/convergence/2026-11-24-master/model/render_tables.py.
module Unbound.Economics.RenderTables

open System
open System.Globalization
open System.Numerics
open Unbound.Py
open Unbound.Py.PyJson
open Unbound.Economics.PyCompat

let APPROACH_ORDER = [ "A"; "A8"; "AL"; "B"; "C0"; "C"; "D" ]

/// f"{x:,.0f}"
let money (x: Json) : string = PyFloat.fixedWith false true 0 (num x).V

/// f"{x:+,.0f}"
let signedMoney (x: Json) : string = PyFloat.fixedWith true true 0 (num x).V

/// f"{x:.Nf}"
let fixedN (n: int) (x: Json) : string = PyFloat.fixedPoint n (num x).V

let private groupDigits (digits: string) =
    digits |> Seq.rev |> Seq.chunkBySize 3 |> Seq.map (fun c -> String(Array.rev c)) |> Seq.rev |> String.concat ","

/// f"{x:,}"
let grouped (x: Json) : string =
    match x with
    | JInt i -> (if i.Sign < 0 then "-" else "") + groupDigits (BigInteger.Abs(i).ToString(CultureInfo.InvariantCulture))
    | JFloat f ->
        let r = PyFloat.repr f
        let sign, body = if r.StartsWith "-" then "-", r.Substring 1 else "", r
        match body.IndexOfAny [| 'e'; 'n'; 'i' |], body.IndexOf '.' with
        | -1, dot when dot > 0 -> sign + groupDigits (body.Substring(0, dot)) + body.Substring dot
        | _ -> r
    | other -> failwith $"cannot format {other} with ','"

/// str(x)
let str (x: Json) : string =
    match x with
    | JStr s -> s
    | JInt _
    | JFloat _ -> Num.str (num x)
    | JNull -> "None"
    | JBool b -> if b then "True" else "False"
    | other -> failwith $"unexpected value {other}"

let table (headers: string list) (rows: string list list) : string =
    [ "| " + String.Join(" | ", headers) + " |"; "|" + String.replicate headers.Length "---|" ]
    @ (rows |> List.map (fun r -> "| " + String.Join(" | ", r) + " |"))
    |> String.concat "\n"

let private getOr (key: string) (j: Json) : Json = PyJson.field key j |> Option.defaultValue (JFloat 0.0)

let comparison (res: Json) : string =
    let rows =
        [ for e, by in items res?comparison do
              for a in APPROACH_ORDER do
                  let s = by?(a)
                  yield
                      [ e
                        text res?meta?enterprises?(e)?name
                        a
                        money s?total_economic_admin_per_account
                        money s?enterprise_admin_per_account
                        money s?fund_borne_admin_per_account
                        money s?acquisition_per_account
                        $"{money s?range?favourable_p10} / {money s?range?base_p50} / {money s?range?adverse_p90}"
                        money s?human_interventions_per_1000 ] ]
    table
        [ "Ent."; "Enterprise"; "Approach"; "Total economic admin $/fund-yr (expected)"; "Borne by enterprise"
          "Borne by funds"; "Acquisition (separate)"; "MC P10 / P50 / P90"; "Human interventions /1,000 funds" ]
        rows

let activities (res: Json) (e: string) : string =
    let by = res?comparison?(e)
    let acts =
        [ for _, s in items by do
              for a, v in items s?activities do
                  if (num v).V >= 0.5 then yield a ]
        |> Set.ofList
        |> PyText.sortStrings
    let rows = acts |> List.map (fun a -> a.Replace("_", " ") :: (APPROACH_ORDER |> List.map (fun x -> money (getOr a by?(x)?activities))))
    table ("Activity ($/fund-yr)" :: APPROACH_ORDER) rows

let bearers (res: Json) (e: string) : string =
    let by = res?comparison?(e)
    let bs = [ for _, s in items by do yield! keys s?bearers ] |> Set.ofList |> PyText.sortStrings
    let rows = bs |> List.map (fun b -> b :: (APPROACH_ORDER |> List.map (fun x -> money (getOr b by?(x)?bearers))))
    table ("Bearer ($/fund-yr, all lines incl. investment management and acquisition)" :: APPROACH_ORDER) rows

let scales (res: Json) : string =
    let rows =
        [ for e, by in items res?scales do
              for n, d in items by |> List.sortWith (fun (a, _) (b, _) -> compare (BigInteger.Parse a) (BigInteger.Parse b)) do
                  yield e :: n :: (APPROACH_ORDER |> List.map (fun a -> money d?(a))) ]
    table ("Ent." :: "Funds" :: APPROACH_ORDER) rows

let revenue (res: Json) : string =
    let rows =
        items res?revenue
        |> List.map (fun (e, r) ->
            [ e
              text r?model
              money r?blended_revenue_per_account
              money r?revenue_after_30pct_market_decline
              money r?margin_C_per_account
              money r?margin_C_after_decline ])
    table
        [ "Ent."; "Revenue model"; "Revenue $/fund"; "After 30% market decline"; "Margin over enterprise admin (C)"
          "Margin after decline" ]
        rows

let sensitivity (res: Json) : string =
    let rows =
        [ for _, rs in items res?sensitivity do
              let rs = PyJson.list rs
              for r in List.truncate 6 rs @ (rs |> List.filter (fun x -> (text x?parameter).StartsWith "licence")) do
                  yield
                      [ text r?enterprise
                        text r?parameter
                        str r?low_value
                        str r?high_value
                        text r?label
                        money r?cost_at_low
                        money r?cost_at_high
                        money r?``base`` ] ]
    table [ "Ent."; "Parameter"; "Low"; "High"; "Label"; "Cost at low"; "Cost at high"; "Base" ] rows

let stress (res: Json) : string =
    let rows =
        [ for _, rs in items res?stress do
              for r in PyJson.list rs do
                  yield
                      [ text r?enterprise
                        text r?scenario
                        money r?total_economic_admin_per_account
                        signedMoney r?delta_vs_normal
                        fixedN 2 r?sla_attainment_exceptions
                        fixedN 1 r?p90_wait_days_exceptions
                        money r?uncontained_exposure_usd ] ]
    table
        [ "Ent."; "Scenario (approach C)"; "$/fund-yr"; "Δ vs normal"; "Exception SLA attainment"; "P90 wait (days)"
          "Uncontained exposure $" ]
        rows

let licence (res: Json) : string =
    let rows =
        [ for e, by in items res?licence_sensitivity do
              for k, d in items by do
                  yield [ e; k; items d |> List.map (fun (w, v) -> $"{w}: {money v}") |> String.concat " / " ] ]
    table [ "Ent."; "Approach"; "Total economic admin $/fund-yr by licence scale" ] rows

let vendor (res: Json) : string =
    let rows =
        PyJson.list res?vendor_pnl
        |> List.map (fun v ->
            [ grouped v?platform_funds
              str v?sponsors
              money v?price_per_fund
              money v?revenue
              money v?total_cost
              signedMoney v?operating_result
              money v?break_even_price_per_fund ])
    table
        [ "Platform funds"; "Sponsors (5k avg)"; "Licence $/fund"; "Revenue"; "Total vendor cost"; "Operating result"
          "Break-even $/fund" ]
        rows

let surge (res: Json) : string =
    let rows =
        [ for _, rs in items res?december_surge do
              for r in PyJson.list rs do
                  yield
                      [ text r?enterprise
                        fixedN 1 r?seasonal_frac
                        money r?total_economic_admin_per_account
                        signedMoney r?delta_vs_normal_C
                        fixedN 2 r?sla_attainment_exceptions
                        fixedN 1 r?p90_wait_days_exceptions ] ]
    table
        [ "Ent."; "December extra staff (x base)"; "$/fund-yr"; "Δ vs normal C"; "Exception SLA attainment"; "P90 wait (days)" ]
        rows

let staffed (res: Json) : string =
    let rows =
        PyJson.list res?staffed_offer
        |> List.map (fun r ->
            [ grouped r?admin_funds
              text r?licence_scale
              money r?unbound_admin_cost_per_fund
              money r?sponsor_retained_decision_cost_per_fund
              (if truthy r?team_floor_binding then "yes" else "no")
              signedMoney r?margin_vs_ren_low ])
    table
        [ "Funds administered"; "Platform cost scale"; "Unbound admin cost $/fund-yr"; "Sponsor-retained decision cost $/fund-yr"
          "Team floor binds"; "Headroom vs Ren low ($413)" ]
        rows

/// The whole of 05a-ECONOMIC-TABLES.md.
let render (res: Json) : string =
    let lic = res?meta?licence_per_account
    let parts =
        [ "# 05a — Economic tables (generated)"
          "Generated by `model/render_tables.py` from `model/enterprise-results.json`; do not edit. **Theoretical model**: "
          + "every figure follows from labelled assumptions (parameters.json labels S/A/D and the [A] values in "
          + "`enterprise_economics.py`). *Total economic admin* = all administration cost lines regardless of who bears them "
          + "(enterprise and funds), excluding investment management fees and customer acquisition; this is the measure on "
          + "which approaches are compared, so moving a cost to funds is never counted as a saving."
          "Unbound licence per fund-year used for approaches AL, C and D (vendor cost at whole-platform scale plus 30% markup): "
          + $"favourable (250k platform funds) {money lic?favourable}, base (100k) {money lic?``base``}, adverse (25k) {money lic?adverse}; "
          + $"market-exceeding reference (1M) {money lic?market_exceeding_reference}. Approaches: A traditional (30%% per-item review), "
          + "A8 traditional with the same 8% exception definition, AL traditional on licensed software, B AI-enhanced self-built, "
          + "C0 Unbound without unmeasured redesign levers, C AI-native redesign, D C plus elimination."
          "## 1. Approach comparison at modelled size\n\n" + comparison res
          "## 2. Scale\n\n" + scales res
          "## 2a. Licence scale sensitivity (adversarial correction ADV-B-001)\n\n" + licence res
          "## 2b. Unbound vendor P&L (ADV-B-002)\n\nAssumptions [A]: " + text res?vendor_assumptions?basis + "\n\n" + vendor res
          "## 3. Activity costs by approach\n\n"
          + (keys res?comparison
             |> List.map (fun e ->
                 $"### {e} — {text res?meta?enterprises?(e)?name}\n\n" + activities res e + "\n\n" + bearers res e)
             |> String.concat "\n\n")
          "## 4. Revenue and margin\n\n" + revenue res
          "## 5. One-way sensitivity (approach C; top six plus licence scale)\n\n" + sensitivity res
          "## 6. Stress scenarios (approach C)\n\n" + stress res
          "## 7. December surge staffing (TEV-001; pooled queue simulation)\n\n" + surge res
          "## 8. Staffed administration offer vs Ren (OD-M-08)\n\nAssumptions: " + text res?staffed_assumptions?basis + "\n\n" + staffed res ]
    String.concat "\n\n" parts + "\n"

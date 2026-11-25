#!/usr/bin/env python3
"""Render enterprise-results.json into Markdown tables (pure; writes one file).

Run: python3 -I -B render_tables.py   -> ../05-ECONOMIC-TABLES.md
"""
import json
import os

HERE = os.path.dirname(os.path.abspath(__file__))
APPROACH_ORDER = ("A", "A8", "AL", "B", "C0", "C", "D")


def money(x):
    return f"{x:,.0f}"


def table(headers, rows):
    return "\n".join(["| " + " | ".join(headers) + " |", "|" + "---|" * len(headers)] + ["| " + " | ".join(r) + " |" for r in rows])


def comparison(res):
    rows = [(e, res["meta"]["enterprises"][e]["name"], a,
             money(s["total_economic_admin_per_account"]), money(s["enterprise_admin_per_account"]),
             money(s["fund_borne_admin_per_account"]), money(s["acquisition_per_account"]),
             f"{money(s['range']['favourable_p10'])} / {money(s['range']['base_p50'])} / {money(s['range']['adverse_p90'])}",
             money(s["human_interventions_per_1000"]))
            for e, by in res["comparison"].items() for a in APPROACH_ORDER for s in (by[a],)]
    return table(("Ent.", "Enterprise", "Approach", "Total economic admin $/fund-yr (expected)", "Borne by enterprise",
                  "Borne by funds", "Acquisition (separate)", "MC P10 / P50 / P90", "Human interventions /1,000 funds"), rows)


def activities(res, e):
    by = res["comparison"][e]
    acts = sorted({a for s in by.values() for a, v in s["activities"].items() if v >= 0.5})
    rows = [(a.replace("_", " "),) + tuple(money(by[x]["activities"].get(a, 0.0)) for x in APPROACH_ORDER) for a in acts]
    return table(("Activity ($/fund-yr)",) + APPROACH_ORDER, rows)


def bearers(res, e):
    by = res["comparison"][e]
    bs = sorted({b for s in by.values() for b in s["bearers"]})
    rows = [(b,) + tuple(money(by[x]["bearers"].get(b, 0.0)) for x in APPROACH_ORDER) for b in bs]
    return table(("Bearer ($/fund-yr, all lines incl. investment management and acquisition)",) + APPROACH_ORDER, rows)


def scales(res):
    rows = [(e, n) + tuple(money(d[a]) for a in APPROACH_ORDER) for e, by in res["scales"].items()
            for n, d in sorted(by.items(), key=lambda kv: int(kv[0]))]
    return table(("Ent.", "Funds") + APPROACH_ORDER, rows)


def revenue(res):
    rows = [(e, r["model"], money(r["blended_revenue_per_account"]), money(r["revenue_after_30pct_market_decline"]),
             money(r["margin_C_per_account"]), money(r["margin_C_after_decline"])) for e, r in res["revenue"].items()]
    return table(("Ent.", "Revenue model", "Revenue $/fund", "After 30% market decline", "Margin over enterprise admin (C)", "Margin after decline"), rows)


def sensitivity(res):
    rows = [(r["enterprise"], r["parameter"], str(r["low_value"]), str(r["high_value"]), r["label"], money(r["cost_at_low"]),
             money(r["cost_at_high"]), money(r["base"]))
            for e, rs in res["sensitivity"].items() for r in rs[:6] + [x for x in rs if x["parameter"].startswith("licence")]]
    return table(("Ent.", "Parameter", "Low", "High", "Label", "Cost at low", "Cost at high", "Base"), rows)


def stress(res):
    rows = [(r["enterprise"], r["scenario"], money(r["total_economic_admin_per_account"]), f"{r['delta_vs_normal']:+,.0f}",
             f"{r['sla_attainment_exceptions']:.2f}", f"{r['p90_wait_days_exceptions']:.1f}", money(r["uncontained_exposure_usd"]))
            for e, rs in res["stress"].items() for r in rs]
    return table(("Ent.", "Scenario (approach C)", "$/fund-yr", "Δ vs normal", "Exception SLA attainment", "P90 wait (days)", "Uncontained exposure $"), rows)


def licence(res):
    rows = [(e, k, " / ".join(f"{w}: {money(v)}" for w, v in d.items())) for e, by in res["licence_sensitivity"].items() for k, d in by.items()]
    return table(("Ent.", "Approach", "Total economic admin $/fund-yr by licence scale"), rows)


def vendor(res):
    rows = [(f"{v['platform_funds']:,}", str(v["sponsors"]), money(v["price_per_fund"]), money(v["revenue"]), money(v["total_cost"]),
             f"{v['operating_result']:+,.0f}", money(v["break_even_price_per_fund"])) for v in res["vendor_pnl"]]
    return table(("Platform funds", "Sponsors (5k avg)", "Licence $/fund", "Revenue", "Total vendor cost", "Operating result", "Break-even $/fund"), rows)


def surge(res):
    rows = [(r["enterprise"], f"{r['seasonal_frac']:.1f}", money(r["total_economic_admin_per_account"]), f"{r['delta_vs_normal_C']:+,.0f}",
             f"{r['sla_attainment_exceptions']:.2f}", f"{r['p90_wait_days_exceptions']:.1f}") for e, rs in res["december_surge"].items() for r in rs]
    return table(("Ent.", "December extra staff (x base)", "$/fund-yr", "Δ vs normal C", "Exception SLA attainment", "P90 wait (days)"), rows)


def staffed(res):
    rows = [(f"{r['admin_funds']:,}", r["licence_scale"], money(r["unbound_admin_cost_per_fund"]), money(r["sponsor_retained_decision_cost_per_fund"]),
             "yes" if r["team_floor_binding"] else "no", f"{r['margin_vs_ren_low']:+,.0f}") for r in res["staffed_offer"]]
    return table(("Funds administered", "Platform cost scale", "Unbound admin cost $/fund-yr", "Sponsor-retained decision cost $/fund-yr", "Team floor binds", "Headroom vs Ren low ($413)"), rows)


def main():
    res = json.load(open(os.path.join(HERE, "enterprise-results.json")))
    lic = res["meta"]["licence_per_account"]
    parts = ["# 05a — Economic tables (generated)",
             "Generated by `model/render_tables.py` from `model/enterprise-results.json`; do not edit. **Theoretical model**: "
             "every figure follows from labelled assumptions (parameters.json labels S/A/D and the [A] values in "
             "`enterprise_economics.py`). *Total economic admin* = all administration cost lines regardless of who bears them "
             "(enterprise and funds), excluding investment management fees and customer acquisition; this is the measure on "
             "which approaches are compared, so moving a cost to funds is never counted as a saving.",
             f"Unbound licence per fund-year used for approaches AL, C and D (vendor cost at whole-platform scale plus 30% markup): "
             f"favourable (250k platform funds) {money(lic['favourable'])}, base (100k) {money(lic['base'])}, adverse (25k) {money(lic['adverse'])}; "
             f"market-exceeding reference (1M) {money(lic['market_exceeding_reference'])}. Approaches: A traditional (30% per-item review), "
             "A8 traditional with the same 8% exception definition, AL traditional on licensed software, B AI-enhanced self-built, "
             "C0 Unbound without unmeasured redesign levers, C AI-native redesign, D C plus elimination.",
             "## 1. Approach comparison at modelled size\n\n" + comparison(res),
             "## 2. Scale\n\n" + scales(res),
             "## 2a. Licence scale sensitivity (adversarial correction ADV-B-001)\n\n" + licence(res),
             "## 2b. Unbound vendor P&L (ADV-B-002)\n\nAssumptions [A]: " + res["vendor_assumptions"]["basis"] + "\n\n" + vendor(res),
             "## 3. Activity costs by approach\n\n" + "\n\n".join(f"### {e} — {res['meta']['enterprises'][e]['name']}\n\n" + activities(res, e)
                                                                 + "\n\n" + bearers(res, e) for e in res["comparison"]),
             "## 4. Revenue and margin\n\n" + revenue(res),
             "## 5. One-way sensitivity (approach C; top six plus licence scale)\n\n" + sensitivity(res),
             "## 6. Stress scenarios (approach C)\n\n" + stress(res),
             "## 7. December surge staffing (TEV-001; pooled queue simulation)\n\n" + surge(res),
             "## 8. Staffed administration offer vs Ren (OD-M-08)\n\nAssumptions: " + res["staffed_assumptions"]["basis"] + "\n\n" + staffed(res)]
    with open(os.path.join(HERE, "..", "05a-ECONOMIC-TABLES.md"), "w") as fh:
        fh.write("\n\n".join(parts) + "\n")


if __name__ == "__main__":
    main()

#!/usr/bin/env python3
"""SPD-ADV-001 decision applied: one decision team per sponsor (never shared).

Owner decision (2026-11-24, product owner): no single team may exercise
legally reserved (A3+) authority for more than one sponsor. Every sponsor has
its own decision team. A sponsor may staff that team itself ("self-staffed");
otherwise the operator supplies a team dedicated to that sponsor alone
("operator-dedicated"). Pricing must reflect which option the sponsor picks.

This script re-runs the executed model (model.py, unchanged) and restructures
only the cost lines that the decision affects. It is pure: each function maps
inputs to outputs; nothing is mutated after construction.

Run:  python3 -I -B decision_teams.py [parameters.json] [output_dir]
Writes decision-teams-results.json and decision-teams-results.csv.

THEORETICAL MODEL: every output follows from labelled assumptions. The new
assumptions are in DT_PARAMS below and are all [A].
"""
import csv
import json
import math
import os
import random
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import model as M  # noqa: E402  (the unchanged executed model)

# Queues whose decisions are legally reserved to the sponsor (A3+): exception
# grants and complex-gift acceptance. Service and ops work is not reserved and
# stays pooled. Conservative [A]: the whole queue is treated as reserved.
RESERVED = ("exceptions", "complex")

DT_PARAMS = {
    "team_floor_fte": {"value": 2.5, "low": 2.0, "high": 3.5, "label": "A",
                       "source": "two-person approval for A3+ decisions plus absence cover; same floor as adversarial variant V1 (SPF-ADV-001)"},
    "team_granularity_fte": {"value": 0.5, "label": "A",
                             "source": "a team is hired in half-FTE steps (part-time staff may serve one sponsor only)"},
    "dedicated_mgmt_overhead": {"value": 0.10, "low": 0.05, "high": 0.20, "label": "A",
                                "source": "operator supervision, appointment paperwork and conflict attestation per dedicated team"},
    "self_staffed_loaded_cost_mult": {"value": 1.0, "low": 0.8, "high": 1.3, "label": "A",
                                      "source": "sponsor's own loaded cost per FTE relative to the operator's"},
    "self_staffed_support_cost": {"value": 15000, "low": 5000, "high": 30000, "label": "A",
                                  "source": "operator-borne per self-staffed sponsor: authority-register set-up, training, attestation evidence, audit support (USD/yr)"},
}
MODES = ("pooled_v1_baseline", "operator_dedicated", "self_staffed")
OMS = ("OM1", "OM2", "OM3", "OM4", "OM5")
SPONSOR_SIZES = (500, 1000, 2500, 5000, 10000, 25000, 100000)


def dt_expected():
    return {k: s["value"] for k, s in DT_PARAMS.items()}


def dt_sample(rng):
    def draw(s):
        return rng.triangular(s["low"], s["high"], s["value"]) if "low" in s else s["value"]
    return {k: draw(s) for k, s in DT_PARAMS.items()}


def round_up(x, step):
    return math.ceil(x / step - 1e-9) * step


def reserved_workload_fte(v, vol):
    """Unrounded FTE of reserved decision work for the whole book."""
    handle = M.handle_minutes(v, vol)
    return sum(vol[q] * handle[q] / 60.0 / v["productive_hours"] * v["staffing_headroom"] for q in RESERVED)


def team_fte(workload_fte, dt):
    return max(dt["team_floor_fte"], round_up(workload_fte, dt["team_granularity_fte"]))


def sponsors_of(om, accounts, v):
    return max(1, math.ceil(accounts / v["accounts_per_sponsor"])) if om["platform"] else 1


def decision_team_lines(om, accounts, v, vol, dt, mode):
    """Cost lines for per-sponsor decision teams under one mode."""
    n = sponsors_of(om, accounts, v)
    per_team = team_fte(reserved_workload_fte(v, vol) / n, dt)
    loaded = v["loaded_cost_per_fte"]
    if mode == "operator_dedicated" or not om["platform"]:
        bearer = "platform_operator" if om["platform"] else "sponsor"
        staff = n * per_team * loaded
        overhead = staff * dt["dedicated_mgmt_overhead"] if om["platform"] else 0.0
        return ([M.line("decision_team:staff", staff, "step_fixed", bearer, "sponsor"),
                 M.line("decision_team:management", overhead, "fixed", bearer, "sponsor")], n, per_team)
    staff = n * per_team * loaded * dt["self_staffed_loaded_cost_mult"]
    support = n * dt["self_staffed_support_cost"]
    return ([M.line("decision_team:staff", staff, "step_fixed", "sponsor", "sponsor"),
             M.line("decision_team:operator_support", support, "fixed", "platform_operator", "sponsor")], n, per_team)


def restructure(run, om, accounts, dt, mode):
    """Replace the pooled reserved-queue staff lines with per-sponsor teams."""
    if mode == "pooled_v1_baseline":
        return run["lines"], sponsors_of(om, accounts, run["values"]), None
    pooled = {"staff:queue:" + q for q in RESERVED}
    kept = tuple(l for l in run["lines"] if l["line"] not in pooled)
    added, n, per_team = decision_team_lines(om, accounts, run["values"], run["volumes"], dt, mode)
    return kept + tuple(added), n, per_team


def opex(l):
    """Same basis as model.py cost_per_account: operating cost excl. customer acquisition, incl. identity checks."""
    return l["type"] != "acquisition" or l["line"] == "identity_new_accounts"


def bearer_split(lines, acct_years):
    def total(pred):
        return sum(l["usd"] for l in lines if pred(l))
    enterprise = total(lambda l: l["bearer"] in ("sponsor", "platform_operator") and opex(l))
    return {
        "cost_per_account": enterprise / acct_years,
        "operator_per_account": total(lambda l: l["bearer"] == "platform_operator" and opex(l)) / acct_years,
        "sponsor_per_account": total(lambda l: l["bearer"] == "sponsor" and opex(l)) / acct_years,
        "decision_team_per_account": total(lambda l: l["line"].startswith("decision_team:")) / acct_years,
        "pooled_reserved_staff_per_account": total(lambda l: l["line"] in ("staff:queue:exceptions", "staff:queue:complex")) / acct_years,
    }


def evaluate(cfg, om_key, accounts, rng, dt_rng, noise=True):
    om = cfg["operating_models"][om_key]
    run = M.run_once(cfg, om_key, accounts, "normal", rng, noise=noise)
    dt = dt_sample(dt_rng) if noise else dt_expected()
    ay = run["metrics"]["accounts_year_avg"]

    def one(mode):
        lines, n, per_team = restructure(run, om, accounts, dt, mode)
        return dict(bearer_split(lines, ay), sponsors=n, team_fte_per_sponsor=per_team)
    out = {mode: one(mode) for mode in MODES}
    # reproduction check: the pooled view must equal model.py's own figure
    assert abs(out["pooled_v1_baseline"]["cost_per_account"] - run["metrics"]["cost_per_account"]) < 1e-6
    return out


def mc(cfg, om_key, accounts, n):
    master = cfg["meta"]["seed"]
    return [evaluate(cfg, om_key, accounts,
                     random.Random(M.seed_for(master, om_key, accounts, i)),
                     random.Random(M.seed_for(master, "DT", om_key, accounts, i))) for i in range(n)]


def summarise(runs, mode, metric):
    xs = [r[mode][metric] for r in runs if r[mode][metric] is not None]
    return {p: M.pct(xs, q) for p, q in (("p10", 0.1), ("p50", 0.5), ("p90", 0.9))}


def sponsor_size_table(cfg, om_key, dt):
    """Deterministic: decision-team cost for ONE sponsor of a given size, by mode.

    Reserved workload per account is taken from the expected-value run of the
    operating model at 1M accounts (rates per account are scale-free in model.py).
    """
    run = M.run_once(cfg, om_key, 1000000, "normal", random.Random(0), noise=False)
    v, vol = run["values"], run["volumes"]
    per_acct_fte = reserved_workload_fte(v, vol) / run["metrics"]["accounts_year_avg"]
    loaded = v["loaded_cost_per_fte"]

    def row(size):
        fte = team_fte(per_acct_fte * size, dt)
        dedicated = fte * loaded * (1 + dt["dedicated_mgmt_overhead"])
        own = fte * loaded * dt["self_staffed_loaded_cost_mult"]
        return {"sponsor_accounts": size, "team_fte": fte, "floor_binds": fte <= dt["team_floor_fte"] + 1e-9,
                "operator_dedicated_usd_per_sponsor": dedicated,
                "operator_dedicated_usd_per_account": dedicated / size,
                "self_staffed_sponsor_staff_usd": own,
                "self_staffed_operator_support_usd": dt["self_staffed_support_cost"],
                "self_staffed_operator_support_per_account": dt["self_staffed_support_cost"] / size,
                # lower bound: a sponsor that already employs two authorised principals pays only the time used
                "self_staffed_existing_staff_marginal_usd": per_acct_fte * size * loaded * dt["self_staffed_loaded_cost_mult"]}
    return {"om": om_key, "reserved_fte_per_1000_accounts": per_acct_fte * 1000,
            "workload_fills_floor_at_accounts": dt["team_floor_fte"] / per_acct_fte if per_acct_fte else None,
            "rows": [row(s) for s in SPONSOR_SIZES]}


def price_structure(cfg, om_key, accounts, dt):
    """Cost-recovery floors (not prices) for the two-part tariff, deterministic."""
    run = M.run_once(cfg, om_key, accounts, "normal", random.Random(0), noise=False)
    om = cfg["operating_models"][om_key]
    ay = run["metrics"]["accounts_year_avg"]
    pooled = {"staff:queue:" + q for q in RESERVED}
    shared = [l for l in run["lines"] if l["line"] not in pooled and l["bearer"] == "platform_operator"
              and opex(l) and l["level"] != "sponsor"]
    per_sponsor_platform = [l for l in run["lines"] if l["line"] == "sponsor_level:platform_service"]
    n = sponsors_of(om, accounts, run["values"])
    ded, _, fte = decision_team_lines(om, accounts, run["values"], run["volumes"], dt, "operator_dedicated")
    own, _, _ = decision_team_lines(om, accounts, run["values"], run["volumes"], dt, "self_staffed")
    return {
        "om": om_key, "accounts": accounts, "sponsors": n, "team_fte_per_sponsor": fte,
        "A_platform_fee_per_account_floor": sum(l["usd"] for l in shared) / ay,
        "B_sponsor_service_fee_per_sponsor_floor": sum(l["usd"] for l in per_sponsor_platform) / n,
        "C1_dedicated_team_fee_per_sponsor_floor": sum(l["usd"] for l in ded) / n,
        "C2_self_staffed_support_fee_per_sponsor_floor": sum(l["usd"] for l in own if l["bearer"] == "platform_operator") / n,
        "self_staffed_sponsor_own_staff_cost_per_sponsor": sum(l["usd"] for l in own if l["bearer"] == "sponsor") / n,
        "note": "Floors recover modelled operator cost only; margin, acquisition and fund-borne costs excluded.",
    }


def main(argv):
    pfile = argv[1] if len(argv) > 1 else os.path.join(HERE, "parameters.json")
    outdir = argv[2] if len(argv) > 2 else HERE
    cfg = M.load(pfile)
    n_runs = cfg["meta"]["mc_runs_normal"]
    metrics = ("cost_per_account", "operator_per_account", "sponsor_per_account", "decision_team_per_account")
    mc_out = {}
    for om in OMS:
        for a in cfg["scales"]:
            runs = mc(cfg, om, a, n_runs)
            mc_out[f"{om}|{a}"] = {mode: {m: summarise(runs, mode, m) for m in metrics} for mode in MODES}
            mc_out[f"{om}|{a}"]["sponsors_p50"] = summarise(runs, "operator_dedicated", "sponsors")["p50"]
    dt = dt_expected()
    grid = [1000, 3000, 10000, 30000, 100000, 300000, 1000000, 2000000, 5000000, 10000000]

    def det(om, a, mode):
        return evaluate(cfg, om, a, random.Random(0), random.Random(0), noise=False)[mode]["cost_per_account"]

    targets = {str(t): {om: {mode: next((a for a in grid if det(om, a, mode) <= t), None) for mode in MODES}
                        for om in OMS} for t in cfg["targets"]}
    results = {
        "meta": {"decision": "SPD-ADV-001 decided-provisional 2026-11-24 by product owner: one decision team per sponsor, never shared; sponsor may self-staff",
                 "command": "python3 -I -B decision_teams.py", "seed": cfg["meta"]["seed"], "mc_runs": n_runs,
                 "base_model": "model.py (unchanged)", "dt_params": DT_PARAMS, "reserved_queues": list(RESERVED),
                 "disclaimer": "THEORETICAL MODEL OUTPUTS from labelled assumptions; not observations."},
        "normal_mc": mc_out,
        "targets_min_scale_deterministic": targets,
        "sponsor_size": [sponsor_size_table(cfg, om, dt) for om in ("OM3", "OM4", "OM5")],
        "price_floors": [price_structure(cfg, om, a, dt) for om, a in
                         (("OM3", 100000), ("OM3", 1000000), ("OM4", 1000000), ("OM5", 5000000))],
    }
    with open(os.path.join(outdir, "decision-teams-results.json"), "w") as fh:
        json.dump(results, fh, indent=1, sort_keys=True, default=float)
    with open(os.path.join(outdir, "decision-teams-results.csv"), "w", newline="") as fh:
        w = csv.writer(fh)
        w.writerow(["operating_model", "accounts", "mode", "metric", "p10", "p50", "p90"])
        for k, modes in mc_out.items():
            om, a = k.split("|")
            for mode in MODES:
                for m, q in modes[mode].items():
                    w.writerow([om, a, mode, m, round(q["p10"], 6), round(q["p50"], 6), round(q["p90"], 6)])
    for k, modes in mc_out.items():
        print(k, " | ".join(f"{mode} P50 {modes[mode]['cost_per_account']['p50']:.2f}" for mode in MODES),
              f"| sponsors {modes['sponsors_p50']:.0f}")
    print("targets", json.dumps(targets))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

#!/usr/bin/env python3
"""Owner decisions SPD-ADV-001 and SPD-ADV-004 applied to the executed model.

SPD-ADV-001 (2026-11-24): no team may exercise legally reserved (A3+) authority
for more than one sponsor; every sponsor has its own decision team.
SPD-ADV-004 (2026-11-24): Unbound is a software platform only. Unbound builds
and runs the software; the organizations that use it (a sponsor directly, or a
third-party administrator acting for several sponsors) employ every person who
does human work. Operator-supplied staff are a future option kept as a hook.

This script re-runs the executed model (model.py, unchanged) and re-assigns
cost lines by who bears them. It is pure: functions map inputs to outputs and
nothing is mutated after construction.

Run:  python3 -I -B decision_teams.py [parameters.json] [output_dir]
Writes decision-teams-results.json and decision-teams-results.csv.

THEORETICAL MODEL: every output follows from labelled assumptions. The new
assumptions are in DT_PARAMS and VENDOR_LINES below and are all [A].
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

# Queues whose decisions are reserved to the sponsor (A3+): exception grants and
# complex-gift acceptance. Conservative [A]: the whole queue is treated as reserved.
RESERVED = ("exceptions", "complex")
STAFFED = ("exceptions", "complex", "service", "ops")

DT_PARAMS = {
    "team_floor_fte": {"value": 2.5, "low": 2.0, "high": 3.5, "label": "A",
                       "source": "five named authority holders x 0.5 minimum time share (report 10, S1 floor); covers two-person approval and absence"},
    "team_granularity_fte": {"value": 0.5, "label": "A",
                             "source": "staff hired in half-FTE steps; a part-time person serves one sponsor only"},
    "dedicated_mgmt_overhead": {"value": 0.10, "low": 0.05, "high": 0.20, "label": "A",
                                "source": "future option only: operator supervision and appointment paperwork per dedicated team"},
    "customer_loaded_cost_mult": {"value": 1.0, "low": 0.8, "high": 1.3, "label": "A",
                                  "source": "customer's loaded cost per FTE relative to model.py loaded_cost_per_fte"},
    "tenant_authority_setup_cost": {"value": 15000, "low": 5000, "high": 30000, "label": "A",
                                    "source": "vendor-borne per sponsor tenant: authority-register set-up, training material, attestation evidence, audit support (USD/yr)"},
}

# Lines Unbound bears as the software vendor [A]. Everything else that is
# operating cost falls on the customer (sponsor or administrator).
VENDOR_LINES = frozenset({
    "staff:leadership", "staff:engineering", "staff:agent_ownership", "staff:security",
    "staff:philanthropic_intelligence", "ai_inference", "infrastructure_base", "infrastructure_per_account",
    "data_providers", "eligibility_screening", "identity_new_accounts", "insurance",
    "sponsor_level:platform_service", "tenant:authority_setup",
})

# Active modes for release 1, plus the reference and the future hook.
MODES = ("pooled_v1_baseline", "sponsor_direct", "via_administrator", "future_operator_dedicated")
ACTIVE = ("sponsor_direct", "via_administrator")
OMS = ("OM3", "OM4", "OM5")
SPONSOR_SIZES = (500, 1000, 2500, 5000, 10000, 25000, 100000)


def dt_expected():
    return {k: s["value"] for k, s in DT_PARAMS.items()}


def dt_sample(rng):
    def draw(s):
        return rng.triangular(s["low"], s["high"], s["value"]) if "low" in s else s["value"]
    return {k: draw(s) for k, s in DT_PARAMS.items()}


def round_up(x, step):
    return math.ceil(x / step - 1e-9) * step


def workload_fte(v, vol, queues):
    """Unrounded FTE of human work in the given queues for the whole book."""
    handle = M.handle_minutes(v, vol)
    arrivals = M.queue_annual_arrivals(vol)
    return sum(arrivals[q] * handle[q] / 60.0 / v["productive_hours"] * v["staffing_headroom"] for q in queues)


def team_fte(fte, dt):
    return max(dt["team_floor_fte"], round_up(fte, dt["team_granularity_fte"]))


def sponsors_of(om, accounts, v):
    return max(1, math.ceil(accounts / v["accounts_per_sponsor"])) if om["platform"] else 1


def bearer_of(line_name):
    return "vendor" if line_name in VENDOR_LINES else "customer"


def rebear(lines):
    return tuple(dict(l, bearer=bearer_of(l["line"])) if l["bearer"] in ("platform_operator", "sponsor") else l
                 for l in lines)


def staff_lines(om, accounts, v, vol, dt, mode):
    """Per-sponsor staffing lines that replace model.py's pooled queue staff."""
    n = sponsors_of(om, accounts, v)
    loaded = v["loaded_cost_per_fte"] * dt["customer_loaded_cost_mult"]
    setup = M.line("tenant:authority_setup", n * dt["tenant_authority_setup_cost"], "fixed", "vendor", "sponsor")
    if mode == "sponsor_direct":
        # each sponsor employs everyone: decisions, service and operations, one combined floor
        per = team_fte(workload_fte(v, vol, STAFFED) / n, dt)
        return (M.line("sponsor_staff:all_queues", n * per * loaded, "step_fixed", "customer", "sponsor"), setup), n, per, STAFFED
    per = team_fte(workload_fte(v, vol, RESERVED) / n, dt)
    if mode == "via_administrator":
        # an administrator pools service and operations; decisions stay with each sponsor's own team
        return (M.line("sponsor_staff:decision_team", n * per * loaded, "step_fixed", "customer", "sponsor"), setup), n, per, RESERVED
    staff = n * per * v["loaded_cost_per_fte"]
    return ((M.line("operator_staff:decision_team", staff, "step_fixed", "operator", "sponsor"),
             M.line("operator_staff:management", staff * dt["dedicated_mgmt_overhead"], "fixed", "operator", "sponsor")),
            n, per, RESERVED)


def restructure(run, om, accounts, dt, mode):
    if mode == "pooled_v1_baseline":
        return tuple(run["lines"]), sponsors_of(om, accounts, run["values"]), None
    added, n, per, replaced = staff_lines(om, accounts, run["values"], run["volumes"], dt, mode)
    dropped = {"staff:queue:" + q for q in replaced}
    kept = tuple(l for l in run["lines"] if l["line"] not in dropped)
    return rebear(kept) + added, n, per


def opex(l):
    """Same basis as model.py cost_per_account: operating cost excl. customer acquisition, incl. identity checks."""
    return l["type"] != "acquisition" or l["line"] == "identity_new_accounts"


def bearer_split(lines, acct_years):
    def per_acct(pred):
        return sum(l["usd"] for l in lines if opex(l) and pred(l)) / acct_years
    return {
        "cost_per_account": per_acct(lambda l: l["bearer"] in ("sponsor", "platform_operator", "vendor", "customer", "operator")),
        "vendor_per_account": per_acct(lambda l: l["bearer"] == "vendor"),
        "customer_per_account": per_acct(lambda l: l["bearer"] == "customer"),
        "operator_staff_per_account": per_acct(lambda l: l["bearer"] == "operator"),
    }


def evaluate(cfg, om_key, accounts, rng, dt_rng, noise=True):
    om = cfg["operating_models"][om_key]
    run = M.run_once(cfg, om_key, accounts, "normal", rng, noise=noise)
    dt = dt_sample(dt_rng) if noise else dt_expected()
    ay = run["metrics"]["accounts_year_avg"]

    def one(mode):
        lines, n, per = restructure(run, om, accounts, dt, mode)
        return dict(bearer_split(lines, ay), sponsors=n, staff_fte_per_sponsor=per)
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
    """Deterministic: what ONE sponsor of a given size spends on its own people, by route.

    Workload per account comes from the expected-value run at 1M accounts
    (per-account rates are scale-free in model.py).
    """
    run = M.run_once(cfg, om_key, 1000000, "normal", random.Random(0), noise=False)
    v, vol, ay = run["values"], run["volumes"], run["metrics"]["accounts_year_avg"]
    all_fte, res_fte = workload_fte(v, vol, STAFFED) / ay, workload_fte(v, vol, RESERVED) / ay
    loaded = v["loaded_cost_per_fte"] * dt["customer_loaded_cost_mult"]

    def row(size):
        direct, decide = team_fte(all_fte * size, dt), team_fte(res_fte * size, dt)
        return {"sponsor_accounts": size,
                "sponsor_direct_fte": direct, "sponsor_direct_usd": direct * loaded,
                "sponsor_direct_per_account": direct * loaded / size,
                "via_administrator_decision_fte": decide, "via_administrator_decision_usd": decide * loaded,
                "via_administrator_decision_per_account": decide * loaded / size,
                "existing_staff_time_used_usd": res_fte * size * loaded}
    return {"om": om_key, "all_work_fte_per_1000_accounts": all_fte * 1000,
            "reserved_fte_per_1000_accounts": res_fte * 1000,
            "floor_binds_below_accounts_direct": dt["team_floor_fte"] / all_fte if all_fte else None,
            "floor_binds_below_accounts_decisions": dt["team_floor_fte"] / res_fte if res_fte else None,
            "rows": [row(s) for s in SPONSOR_SIZES]}


def license_floors(cfg, om_key, accounts, dt):
    """Cost-recovery floors (not prices) for Unbound's software licence, deterministic."""
    run = M.run_once(cfg, om_key, accounts, "normal", random.Random(0), noise=False)
    om = cfg["operating_models"][om_key]
    ay = run["metrics"]["accounts_year_avg"]
    lines, n, _ = restructure(run, om, accounts, dt, "sponsor_direct")
    vendor = [l for l in lines if l["bearer"] == "vendor" and opex(l)]
    per_tenant = [l for l in vendor if l["level"] == "sponsor"]
    per_account = [l for l in vendor if l["level"] != "sponsor"]
    return {"om": om_key, "accounts": accounts, "sponsors": n,
            "A_licence_fee_per_account_floor": sum(l["usd"] for l in per_account) / ay,
            "B_tenant_fee_per_sponsor_floor": sum(l["usd"] for l in per_tenant) / n,
            "vendor_cost_total_usd": sum(l["usd"] for l in vendor),
            "note": "Floors recover modelled vendor cost only; margin, sales and customer acquisition excluded."}


def main(argv):
    pfile = argv[1] if len(argv) > 1 else os.path.join(HERE, "parameters.json")
    outdir = argv[2] if len(argv) > 2 else HERE
    cfg = M.load(pfile)
    n_runs = cfg["meta"]["mc_runs_normal"]
    metrics = ("cost_per_account", "vendor_per_account", "customer_per_account", "operator_staff_per_account")
    mc_out = {}
    for om in OMS:
        for a in cfg["scales"]:
            runs = mc(cfg, om, a, n_runs)
            mc_out[f"{om}|{a}"] = dict({mode: {m: summarise(runs, mode, m) for m in metrics} for mode in MODES},
                                       sponsors_p50=summarise(runs, "sponsor_direct", "sponsors")["p50"])
    dt = dt_expected()
    grid = [1000, 3000, 10000, 30000, 100000, 300000, 1000000, 2000000, 5000000, 10000000]

    def det(om, a, mode):
        return evaluate(cfg, om, a, random.Random(0), random.Random(0), noise=False)[mode]["cost_per_account"]

    targets = {str(t): {om: {mode: next((a for a in grid if det(om, a, mode) <= t), None) for mode in MODES}
                        for om in OMS} for t in cfg["targets"]}
    results = {
        "meta": {"decisions": {"SPD-ADV-001": "one decision team per sponsor, never shared",
                               "SPD-ADV-004": "Unbound is a software platform only; customers employ all staff; operator services are a future hook"},
                 "command": "python3 -I -B decision_teams.py", "seed": cfg["meta"]["seed"], "mc_runs": n_runs,
                 "base_model": "model.py (unchanged)", "dt_params": DT_PARAMS, "vendor_lines": sorted(VENDOR_LINES),
                 "reserved_queues": list(RESERVED), "active_modes": list(ACTIVE),
                 "disclaimer": "THEORETICAL MODEL OUTPUTS from labelled assumptions; not observations."},
        "normal_mc": mc_out,
        "targets_min_scale_deterministic": targets,
        "sponsor_size": [sponsor_size_table(cfg, om, dt) for om in OMS],
        "licence_floors": [license_floors(cfg, om, a, dt) for om, a in
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
              f"| vendor P50 {modes['sponsor_direct']['vendor_per_account']['p50']:.2f} | sponsors {modes['sponsors_p50']:.0f}")
    print("targets", json.dumps(targets))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

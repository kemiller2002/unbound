#!/usr/bin/env python3
"""ECN theoretical DAF enterprise simulation (Phase Five).

Pure-functional, standard-library-only model. Deterministic seeded Monte Carlo
(random.Random(seed) per run; per-cell seeds derived with zlib.crc32, never hash()).

Run:  python3 -I -B model.py [parameters.json] [output_dir]
Writes results.json and results.csv next to this file by default.

THEORETICAL MODEL: every output is a consequence of labelled assumptions
(parameters.json). Nothing here is an observation of a real sponsor.
"""
import csv
import json
import math
import os
import random
import sys
import zlib

HERE = os.path.dirname(os.path.abspath(__file__))
QUEUES = ("exceptions", "complex", "service", "ops")
EXT_DAYS = 60  # simulate into the following January-March to measure clearance


# ----------------------------------------------------------------------------
# Parameters
# ----------------------------------------------------------------------------

def load(path):
    with open(path) as fh:
        return json.load(fh)


def expected_values(cfg):
    return {k: s["value"] for k, s in cfg["params"].items()}


def sample_values(cfg, rng):
    """Triangular(low, mode=value, high) for mc params; fixed otherwise."""
    def draw(spec):
        lo, hi, mode = spec["low"], spec["high"], spec["value"]
        if spec.get("mc") and hi > lo:
            return rng.triangular(lo, hi, min(max(mode, lo), hi))
        return mode
    return {k: draw(s) for k, s in cfg["params"].items()}


def apply_overrides(values, overrides):
    def one(k, v):
        o = overrides.get(k)
        if o is None:
            return v
        if "set" in o:
            return o["set"]
        return v * o["mult"]
    return {k: one(k, v) for k, v in values.items()}


def seed_for(master, *parts):
    key = "|".join(str(p) for p in parts).encode()
    return (master * 1000003 + zlib.crc32(key)) & 0xFFFFFFFF


# ----------------------------------------------------------------------------
# Volumes (annual, by segment)
# ----------------------------------------------------------------------------

RAIL_COST_KEYS = {"ach": "ach_debit_cost", "check": "lockbox_check_cost", "wire": "wire_cost",
                  "dtc": "dtc_receipt_cost", "payroll": "payroll_item_cost"}


def segment_volumes(v, seg, n, act, outsource_complex):
    grants = n * seg["grants"] * v["grants_mult"] * act
    exc = grants * min(1.0, v["exception_rate"] * seg["exc_mult"]) * (1.0 - v["determination_reuse"])
    contribs = n * seg["contribs"] * act
    complex_n = contribs * min(1.0, seg["complex_share"] * v["complex_share_mult"])
    simple = contribs - complex_n
    contacts = n * seg["contacts"] * v["contacts_mult"]
    mix = seg["mix"]
    ach_debits = simple * mix.get("ach", 0.0)
    returns = ach_debits * v["ach_return_rate"]
    payroll_items = simple * mix.get("payroll", 0.0)
    reconcilable = grants + contribs - payroll_items + payroll_items / v["payroll_batch_size"]
    breaks = reconcilable * v["recon_break_rate"]
    check_share = min(1.0, seg["check_share"] * v["check_share_mult"])
    grant_rail = grants * ((1 - check_share) * v["ach_grant_cost"] + check_share * v["check_grant_cost"])
    contrib_rail = sum(simple * share * v[RAIL_COST_KEYS[r]] for r, share in mix.items() if r in RAIL_COST_KEYS)
    avg_grant = seg["avg_grant"] * v["avg_grant_scale"]
    card_fees = simple * mix.get("card", 0.0) * seg["avg_contrib"] * v["card_fee_pct"]
    return {
        "accounts": n, "grants": grants, "exceptions": exc, "contribs": contribs, "complex": complex_n,
        "contacts": contacts, "contact_minutes": contacts * v["contact_minutes"] * seg["contact_min_mult"],
        "returns": returns, "breaks": breaks, "grant_rail": grant_rail, "contrib_rail": contrib_rail,
        "card_fees_donor": card_fees, "aum": n * seg["balance"],
        "grant_dollars": grants * avg_grant, "contrib_dollars": contribs * seg["avg_contrib"],
        "ach_debit_dollars": ach_debits * seg["avg_contrib"],
        "complex_vendor_fund": complex_n * v["complex_vendor_fee"] if outsource_complex else 0.0,
        "advisor_accounts": n if seg.get("advisor") else 0.0,
    }


def volumes(cfg, v, om, accounts, act_mults):
    mix = cfg["mixes"][om["mix"]]
    per_seg = {s: segment_volumes(v, cfg["segments"][s], accounts * share, act_mults.get(s, 1.0),
                                  om["outsource_complex"]) for s, share in mix.items()}
    keys = next(iter(per_seg.values())).keys()
    total = {k: sum(d[k] for d in per_seg.values()) for k in keys}
    return total, per_seg


# ----------------------------------------------------------------------------
# Staffing
# ----------------------------------------------------------------------------

def handle_minutes(v, vol):
    return {
        "exceptions": v["minutes_per_exception"],
        "complex": v["complex_hours"] * 60.0,
        "service": (vol["contact_minutes"] / vol["contacts"]) if vol["contacts"] > 0 else v["contact_minutes"],
        "ops": ((vol["breaks"] * v["break_minutes"] + vol["returns"] * v["return_minutes"]) /
                (vol["breaks"] + vol["returns"])) if (vol["breaks"] + vol["returns"]) > 0 else v["break_minutes"],
    }


def queue_annual_arrivals(vol):
    return {"exceptions": vol["exceptions"], "complex": vol["complex"],
            "service": vol["contacts"], "ops": vol["breaks"] + vol["returns"]}


QUEUE_FLOORS = {"exceptions": 1, "complex": 1, "service": 1, "ops": 2}


def base_queue_fte(v, arrivals, handle):
    return {q: max(QUEUE_FLOORS[q], math.ceil(arrivals[q] * handle[q] / 60.0 / v["productive_hours"]
                                              * v["staffing_headroom"] - 1e-9)) for q in QUEUES}


def fixed_roles(accounts, platform):
    """Step-fixed roles reproducing the 09 baseline staffing table (FSM section 3)."""
    lead = 5 if accounts < 5000 else (8 if accounts < 50000 else 10)
    eng = min(10, 4 + 2 * max(0, round(math.log10(max(accounts, 1000) / 1000))))
    sec = 1 if accounts < 100000 else 3
    phil = 1 if accounts < 100000 else 3
    roles = {"leadership": lead, "engineering": eng, "agent_ownership": 4, "security": sec,
             "philanthropic_intelligence": phil}
    if not platform:
        roles["partnerships"] = max(1, math.ceil(accounts / 20000))
    return roles


# ----------------------------------------------------------------------------
# Scenario helpers
# ----------------------------------------------------------------------------

def accounts_multiplier(effects, day):
    m = 1.0
    for e in effects:
        if e["type"] == "accounts_step" and day >= e["day"]:
            m += e["frac"]
    return max(m, 0.0)


def account_years(effects, accounts, days):
    return accounts * sum(accounts_multiplier(effects, d) for d in range(days)) / days


def window_factor(effects, etype, queue, day):
    f = 1.0
    for e in effects:
        if e["type"] == etype and (e["queue"] == queue or e["queue"] == "all") and e["start"] <= day < e["end"]:
            f *= e["value"]
    return f


def one_off_daily(effects, queue, day, accounts, annual_grants, v):
    total = 0.0
    for e in effects:
        if e.get("queue") != queue or not (e.get("start", 0) <= day < e.get("end", -1)):
            continue
        span = e["end"] - e["start"]
        if e["type"] == "one_off":
            total += e["per_account"] * accounts / span
        elif e["type"] == "one_off_new":
            frac = sum(x["frac"] for x in effects if x["type"] == "accounts_step" and x["frac"] > 0)
            total += e["per_account"] * accounts * frac / span
        elif e["type"] == "one_off_departing":
            frac = -sum(x["frac"] for x in effects if x["type"] == "accounts_step" and x["frac"] < 0)
            total += e["per_account"] * accounts * frac / span
        elif e["type"] == "one_off_from_grants":
            cases = annual_grants / v["working_days"] * e["approved_days"] * e["f"]
            total += cases / span
    return total


def scenario_k(v, effects):
    ks = [e["value"] for e in effects if e["type"] == "december_k"]
    return ks[-1] if ks else v["december_k"]


def poisson(rng, lam):
    if lam <= 0:
        return 0.0
    if lam > 30:
        return max(0.0, rng.gauss(lam, math.sqrt(lam)))
    limit, k, p = math.exp(-lam), 0, 1.0
    while True:
        p *= rng.random()
        if p <= limit:
            return float(k)
        k += 1


# ----------------------------------------------------------------------------
# Daily queue simulation (backlog recursion B(d+1) = max(0, B(d) + a(d) - c(d)))
# ----------------------------------------------------------------------------

def simulate_queue(q, v, om, annual, handle, base_fte, sla, effects, win, accounts, annual_grants, rng, noise):
    days = int(v["working_days"])
    k = scenario_k(v, effects)
    dec_start = days - int(v["december_days"])
    base_rate = annual / (dec_start + v["december_days"] * k)
    per_fte_hours = v["productive_hours"] / days
    seasonal = v["seasonal_frac"] * base_fte if q in ("exceptions", "complex", "service") else 0.0
    backlog, served_total, arrivals_total = 0.0, 0.0, 0.0
    peak, ok_arr, late_arr = 0.0, 0.0, 0.0
    waits = []
    contractor_days, contractors, pending_at = 0.0, 0.0, None
    busy_min, avail_min = 0.0, 0.0
    exceeded_after_window, recover_day, ye_backlog = False, None, 0.0
    for d in range(days + EXT_DAYS):
        in_year = d < days
        w = k if (dec_start <= d < days) else 1.0
        lam = base_rate * w * accounts_multiplier(effects, d) * window_factor(effects, "arrival_mult", q, d)
        lam += one_off_daily(effects, q, d, accounts, annual_grants, v)
        a = poisson(rng, lam) if noise else lam
        h = handle * window_factor(effects, "handle_mult", q, d)
        avail = window_factor(effects, "capacity_mult", q, d)
        staff = base_fte + (seasonal if dec_start <= d < days else 0.0)
        per_fte_cap = per_fte_hours * 60.0 / h
        # surge response: contractors after lead time, capped
        cap_now = (staff + contractors) * per_fte_cap * avail
        wait_now = backlog / cap_now if cap_now > 0 else float("inf")
        if om["contractors"]:
            if wait_now > sla and pending_at is None and contractors == 0:
                pending_at = d + v["contractor_lead_days"]
            if pending_at is not None and d >= pending_at:
                need = math.ceil(backlog / (sla * per_fte_cap)) if backlog > 0 else 0
                contractors = float(min(v["contractor_max_frac"] * base_fte + 1, need))
                if contractors <= 0:
                    pending_at = None
        cap = (staff + contractors) * per_fte_cap * avail
        work = backlog + a
        served = min(work, cap)
        backlog = work - served
        wait = backlog / cap if cap > 0 else float("inf")
        if contractors > 0 and backlog <= 0.5 * cap:
            contractors, pending_at = 0.0, None
        if in_year:
            arrivals_total += a
            served_total += served
            peak = max(peak, backlog)
            waits.append((min(wait, 999.0), a))
            if wait <= sla:
                ok_arr += a
            else:
                late_arr += a
            contractor_days += contractors
            busy_min += served * h
            avail_min += cap * h
        if d == days - 1:
            ye_backlog = backlog
        waits_day = wait
        if d >= win[1] and waits_day > sla:
            exceeded_after_window = True
        if d >= win[1] and recover_day is None and waits_day <= 1.0:
            recover_day = d
    total_w = sum(a for _, a in waits) or 1.0
    return {
        "arrivals": arrivals_total, "served": served_total, "peak_backlog": peak,
        "year_end_backlog": ye_backlog, "sla_attainment": (ok_arr / total_w) if arrivals_total > 0 else 1.0,
        "late_arrivals": late_arr, "p90_wait_days": weighted_pct(waits, 0.9),
        "mean_wait_days": sum(w * a for w, a in waits) / total_w,
        "contractor_fte_days": contractor_days, "utilisation": busy_min / avail_min if avail_min else 0.0,
        "base_fte": base_fte, "seasonal_fte_days": seasonal * v["december_days"],
        "recovery_days": (recover_day - win[1]) if recover_day is not None else EXT_DAYS + days - win[1],
        "recovered": recover_day is not None, "exceeded_after_window": exceeded_after_window,
        "end_backlog": backlog,
    }


def weighted_pct(pairs, p):
    tot = sum(a for _, a in pairs)
    if tot <= 0:
        return 0.0
    acc = 0.0
    for w, a in sorted(pairs):
        acc += a
        if acc >= p * tot:
            return w
    return sorted(pairs)[-1][0]


# ----------------------------------------------------------------------------
# Exposure model
# ----------------------------------------------------------------------------

def exposures(v, vol, spec_list, rng, noise, accounts):
    days = v["working_days"]
    daily_grant_dollars = vol["grant_dollars"] / days
    rows = []
    # baseline exposures present in every scenario
    attempts = accounts * v["fraud_attempts_per_1000"] / 1000.0
    rows.append(("fraud_baseline", attempts * v["fraud_avg_attempt"],
                 attempts * v["fraud_avg_attempt"] * v["fraud_detection"]))
    returned = vol["ach_debit_dollars"] * v["ach_return_rate"]
    rows.append(("contribution_returns_baseline", returned * v["provisional_credit_exposure"], 0.0))
    for s in spec_list:
        kind = s["kind"]
        if kind == "overcommit":
            gross = vol["aum"] * s["share"] * s["drawdown"]
            rows.append((kind, gross, gross * 0.9))
        elif kind == "revocation":
            pipeline = daily_grant_dollars * s["pipeline_days"] * s["revoked_share"]
            paid_before_hold = daily_grant_dollars * s["lag_days"] * s["revoked_share"]
            gross = pipeline + paid_before_hold
            rows.append((kind, gross, pipeline * s["registry_coverage"]))
        elif kind == "payment_failure":
            at_risk = daily_grant_dollars * s["days"] * s["fail_share"]
            rows.append((kind, at_risk, at_risk * (1 - s["duplicate_risk"])))
        elif kind == "contribution_reversal":
            extra = vol["ach_debit_dollars"] * v["ach_return_rate"] * (s["return_mult"] - 1) * s["days"] / days
            rows.append((kind, extra, extra * (1 - v["provisional_credit_exposure"])))
        elif kind == "fraud":
            att = attempts * (s["mult"]) * s["days"] / days
            rows.append((kind, att * v["fraud_avg_attempt"], att * v["fraud_avg_attempt"] * v["fraud_detection"]))
        elif kind == "ai_error":
            lo, mode, hi = s["detect_days"]
            dd = rng.triangular(lo, hi, mode) if noise else mode
            gross = daily_grant_dollars * s["f"] * dd
            # post-payment: nothing caught before payment; recovery reduces loss
            rows.append((kind, gross, gross * v["post_payment_recovery"]))
            rows.append(("ai_error_excise", gross * v["taxable_share"] * v["excise_rate"], 0.0))
        elif kind == "recon":
            unrec = (vol["grant_dollars"] + vol["contrib_dollars"]) / days * s["days"] * v["recon_break_rate"] * s["mult"]
            rows.append((kind, unrec, unrec * 0.98))
        elif kind == "cyber":
            gross = accounts * s["affected_share"] * v["cyber_cost_per_record"]
            rows.append((kind, gross, 0.0))
        elif kind == "data_corruption":
            gross = (vol["grant_dollars"] + vol["contrib_dollars"]) / days * s["days"] * s["share"]
            rows.append((kind, gross, gross * 0.99))
    gross = sum(r[1] for r in rows)
    caught = sum(r[2] for r in rows)
    return {"rows": [{"kind": r[0], "gross": r[1], "contained": r[2]} for r in rows],
            "gross": gross, "uncontained": gross - caught,
            "control_effectiveness": caught / gross if gross > 0 else 1.0}


# ----------------------------------------------------------------------------
# Cost assembly
# ----------------------------------------------------------------------------

def line(name, usd, ctype, bearer, level):
    return {"line": name, "usd": usd, "type": ctype, "bearer": bearer, "level": level}


def assemble_costs(v, om, accounts, acct_years, vol, sims, base_fte, effects, aum_mult):
    platform = om["platform"]
    op_bearer = "platform_operator" if platform else "sponsor"
    loaded = v["loaded_cost_per_fte"]
    roles = fixed_roles(accounts, platform)
    lines = []
    for r, n in roles.items():
        lines.append(line("staff:" + r, n * loaded, "step_fixed", op_bearer, "platform" if platform else "sponsor"))
    for q in QUEUES:
        lines.append(line("staff:queue:" + q, base_fte[q] * loaded, "step_fixed(volume)", op_bearer, "account"))
        s = sims[q]
        extra = (s["seasonal_fte_days"] * v["december_seasonal_premium"] + s["contractor_fte_days"] * v["contractor_premium"]) * loaded / v["working_days"]
        lines.append(line("staff:surge:" + q, extra, "variable(surge)", op_bearer, "account"))
    ai = (sims["exceptions"]["arrivals"] * v["ai_cost_per_dossier"]
          + acct_years * v["ai_interactions_per_account"] * v["ai_cost_per_interaction"]
          + vol["grants"] * v["ai_cost_per_grant_screen"])
    lines.append(line("ai_inference", ai, "variable", op_bearer, "account"))
    lines.append(line("infrastructure_base", v["infra_base"], "fixed", op_bearer, "platform"))
    lines.append(line("infrastructure_per_account", acct_years * v["infra_per_account"], "variable", op_bearer, "account"))
    lines.append(line("payments:grants", vol["grant_rail"], "variable", op_bearer, "account"))
    lines.append(line("payments:contributions", vol["contrib_rail"], "variable", op_bearer, "account"))
    lines.append(line("eligibility_screening", vol["grants"] * v["routine_eligibility_per_grant"], "variable", op_bearer, "account"))
    lines.append(line("recurring_obligations", acct_years * v["recurring_obligations_per_account"], "variable", op_bearer, "account"))
    scale = 1 + math.log10(max(accounts, 1000) / 1000.0)
    lines.append(line("insurance", v["insurance_base"] * scale, "step_fixed", op_bearer, "platform" if platform else "sponsor"))
    lines.append(line("external_audit", v["audit_base"] + v["audit_bps"] / 1e4 * vol["aum"] * aum_mult, "fixed", op_bearer, "platform" if platform else "sponsor"))
    lines.append(line("legal_filings", v["legal_base"] * scale, "step_fixed", op_bearer, "platform" if platform else "sponsor"))
    lines.append(line("data_providers", v["data_base"], "fixed", op_bearer, "platform"))
    new_accts = accounts * v["new_account_share"] + accounts * sum(e["frac"] for e in effects if e["type"] == "accounts_step" and e["frac"] > 0)
    lines.append(line("identity_new_accounts", new_accts * v["identity_per_new"], "acquisition", op_bearer, "account"))
    cac_bearer = {"sponsor": "sponsor", "sponsor_distribution": "sponsor", "employer": "employer"}[om["cac_bearer"]]
    lines.append(line("customer_acquisition", new_accts * v["cac"], "acquisition", cac_bearer, "account"))
    # advisor relationships
    advisors = vol["advisor_accounts"] / v["accounts_per_advisor"]
    lines.append(line("advisor_relationships", advisors * (v["advisor_relationship_cost"] + v["advisor_onboarding_cost"] * 0.2),
                      "variable", op_bearer, "advisor"))
    # fund-borne
    lines.append(line("custody", v["custody_bps"] / 1e4 * vol["aum"] * aum_mult, "variable", "fund", "account"))
    lines.append(line("pool_investment_expense", v["pool_expense_bps"] / 1e4 * vol["aum"] * aum_mult, "variable", "fund", "account"))
    lines.append(line("complex_asset_vendor", vol["complex_vendor_fund"], "variable", "fund", "account"))
    lines.append(line("card_fees", vol["card_fees_donor"], "variable", "donor_or_fund", "account"))
    # sponsor-level (platform model only)
    if platform:
        sponsors = max(1, math.ceil(accounts / v["accounts_per_sponsor"]))
        lines.append(line("sponsor_level:platform_service", sponsors * (v["sponsor_platform_cost"] + v["sponsor_onboarding_cost"] / 5.0),
                          "step_fixed", "platform_operator", "sponsor"))
        lines.append(line("sponsor_level:entity_costs", sponsors * v["sponsor_entity_cost"], "fixed", "sponsor", "sponsor"))
    for e in effects:
        if e["type"] == "extra_cost":
            lines.append(line("exceptional:" + e["label"], e["usd"], "exceptional", op_bearer, "platform"))
    return lines


def summarise_costs(lines, acct_years, txns, platform, accounts, v):
    def total(pred):
        return sum(l["usd"] for l in lines if pred(l))
    operator = lambda l: l["bearer"] in ("sponsor", "platform_operator")
    opex_ex_acq = total(lambda l: operator(l) and l["type"] != "acquisition")
    acq = total(lambda l: l["type"] == "acquisition" and l["line"] == "customer_acquisition")
    ident = total(lambda l: l["line"] == "identity_new_accounts")
    fund = total(lambda l: l["bearer"] in ("fund", "donor_or_fund"))
    sponsor_level = total(lambda l: l["level"] == "sponsor" and operator(l))
    account_level = total(lambda l: l["level"] in ("account", "advisor") and operator(l) and l["type"] != "acquisition")
    custody = total(lambda l: l["line"] == "custody")
    sponsors = max(1, math.ceil(accounts / v["accounts_per_sponsor"])) if platform else 1
    return {
        "opex_ex_acq": opex_ex_acq + ident,
        "operator_only_per_account": (total(lambda l: l["bearer"] == ("platform_operator" if platform else "sponsor") and l["type"] != "acquisition") + ident) / acct_years,
        "cost_per_account": (opex_ex_acq + ident) / acct_years,
        "cost_per_account_incl_acq": (opex_ex_acq + ident + acq) / acct_years,
        "baseline09_comparable_per_account": (opex_ex_acq + ident + acq + custody) / acct_years,
        "account_level_per_account": account_level / acct_years,
        "platform_fixed_per_account": total(lambda l: l["level"] == "platform" and operator(l)) / acct_years,
        "sponsor_level_per_sponsor": sponsor_level / sponsors,
        "sponsor_level_per_account": sponsor_level / acct_years,
        "fund_borne_per_account": fund / acct_years,
        "acquisition_per_account": acq / acct_years,
        "cost_per_transaction": (opex_ex_acq + ident) / txns if txns else 0.0,
        "sponsors": sponsors,
        "exceptional_usd": total(lambda l: l["type"] == "exceptional"),
        "surge_staff_usd": total(lambda l: l["line"].startswith("staff:surge")),
    }


# ----------------------------------------------------------------------------
# One run
# ----------------------------------------------------------------------------

def run_once(cfg, om_key, accounts, scen_key, rng, noise=True, value_override=None):
    om = cfg["operating_models"][om_key]
    scen = cfg["scenarios"][scen_key]
    base_v = sample_values(cfg, rng) if noise else expected_values(cfg)
    if value_override:
        base_v = dict(base_v, **value_override)
    v = apply_overrides(base_v, om["overrides"])
    effects = scen["effects"]
    mix = cfg["mixes"][om["mix"]]
    act = {s: (math.exp(rng.gauss(0, 0.15)) if noise else 1.0) for s in mix}
    vol, per_seg = volumes(cfg, v, om, accounts, act)
    aum_mult = 1.0
    for e in effects:
        if e["type"] == "aum_mult":
            aum_mult *= e["value"]
    handle = handle_minutes(v, vol)
    arrivals = queue_annual_arrivals(vol)
    base_fte = base_queue_fte(v, arrivals, handle)
    sla = {"exceptions": v["sla_exceptions_days"], "complex": v["sla_complex_days"],
           "service": v["sla_service_days"], "ops": v["sla_ops_days"]}
    sims = {q: simulate_queue(q, v, om, arrivals[q], handle[q], base_fte[q], sla[q], effects, tuple(scen["window"]), accounts,
                              vol["grants"], rng, noise) for q in QUEUES}
    days = int(v["working_days"])
    ay = account_years(effects, accounts, days)
    scale_vol = ay / accounts
    vol_s = {k: (x * scale_vol if isinstance(x, float) else x) for k, x in vol.items()}
    lines = assemble_costs(v, om, accounts, ay, vol_s, sims, base_fte, effects, aum_mult)
    txns = vol_s["grants"] + vol_s["contribs"]
    cs = summarise_costs(lines, ay, txns, om["platform"], accounts, v)
    exp = exposures(v, vol_s, scen["exposure"], rng, noise, accounts)
    human = sum(s["arrivals"] for s in sims.values())
    staff_fte = sum(fixed_roles(accounts, om["platform"]).values()) + sum(base_fte.values())
    surge_fte = sum((s["contractor_fte_days"] + s["seasonal_fte_days"]) for s in sims.values()) / days
    late_exc = sims["exceptions"]["late_arrivals"] + sims["complex"]["late_arrivals"]
    friction = (sims["service"]["arrivals"] + late_exc + sims["ops"]["arrivals"] * 0.5) / ay * 1000.0
    win = scen["window"]
    recovery = max(s["recovery_days"] for s in sims.values())
    unresolved_items = sum(s["end_backlog"] for s in sims.values())
    sla_overall = (sum(s["sla_attainment"] * s["arrivals"] for s in sims.values()) / human) if human else 1.0
    exc_rate = sims["exceptions"]["arrivals"] / vol_s["grants"] if vol_s["grants"] else 0.0
    return {
        "values": v, "volumes": vol_s, "per_segment": per_seg, "lines": lines,
        "metrics": dict(cs, **{
            "human_interventions": human,
            "human_interventions_per_1000_accounts": human / ay * 1000.0,
            "exception_rate_per_grant": exc_rate,
            "peak_backlog_exceptions": sims["exceptions"]["peak_backlog"],
            "peak_backlog_total": sum(s["peak_backlog"] for s in sims.values()),
            "p90_wait_days_exceptions": sims["exceptions"]["p90_wait_days"],
            "p90_wait_days_service": sims["service"]["p90_wait_days"],
            "sla_attainment_overall": sla_overall,
            "sla_attainment_exceptions": sims["exceptions"]["sla_attainment"],
            "recovery_days_max_queue": recovery,
            "financial_exposure_gross": exp["gross"],
            "financial_exposure_uncontained": exp["uncontained"],
            "control_effectiveness_proxy": exp["control_effectiveness"],
            "staff_fte_base": staff_fte,
            "surge_fte_equivalent": surge_fte,
            "utilisation_exceptions": sims["exceptions"]["utilisation"],
            "customer_friction_per_1000": friction,
            "unresolved_backlog_items_after_ext": unresolved_items,
            "unresolved_risk_usd": exp["uncontained"],
            "accounts_year_avg": ay,
            "grants": vol_s["grants"], "contributions": vol_s["contribs"],
            "aum": vol_s["aum"] * aum_mult,
        }),
        "sims": sims, "exposure": exp, "base_fte": base_fte,
    }


# ----------------------------------------------------------------------------
# Aggregation helpers
# ----------------------------------------------------------------------------

def pct(xs, p):
    s = sorted(xs)
    if not s:
        return 0.0
    i = p * (len(s) - 1)
    lo, hi = int(math.floor(i)), int(math.ceil(i))
    return s[lo] + (s[hi] - s[lo]) * (i - lo)


def summarise_runs(runs):
    keys = runs[0]["metrics"].keys()
    return {k: {"p10": pct([r["metrics"][k] for r in runs], 0.1),
                "p50": pct([r["metrics"][k] for r in runs], 0.5),
                "p90": pct([r["metrics"][k] for r in runs], 0.9)} for k in keys}


def mc(cfg, om_key, accounts, scen_key, n):
    master = cfg["meta"]["seed"]
    return [run_once(cfg, om_key, accounts, scen_key,
                     random.Random(seed_for(master, om_key, accounts, i))) for i in range(n)]
    # common random numbers: run i uses the same seed in every scenario, so scenario deltas are paired


def ranks(xs):
    order = sorted(range(len(xs)), key=lambda i: xs[i])
    r = [0.0] * len(xs)
    for pos, i in enumerate(order):
        r[i] = float(pos)
    return r


def spearman(xs, ys):
    rx, ry = ranks(xs), ranks(ys)
    n = len(xs)
    mx, my = sum(rx) / n, sum(ry) / n
    cov = sum((a - mx) * (b - my) for a, b in zip(rx, ry))
    vx = math.sqrt(sum((a - mx) ** 2 for a in rx))
    vy = math.sqrt(sum((b - my) ** 2 for b in ry))
    return cov / (vx * vy) if vx and vy else 0.0


def tornado(cfg, om_key, accounts, metric="cost_per_account"):
    rng = random.Random(0)
    base = run_once(cfg, om_key, accounts, "normal", rng, noise=False)["metrics"][metric]
    rows = []
    for k, s in cfg["params"].items():
        if s["low"] == s["high"]:
            continue
        lo = run_once(cfg, om_key, accounts, "normal", rng, noise=False, value_override={k: s["low"]})["metrics"][metric]
        hi = run_once(cfg, om_key, accounts, "normal", rng, noise=False, value_override={k: s["high"]})["metrics"][metric]
        rows.append({"param": k, "label": s["label"], "low_input": s["low"], "high_input": s["high"],
                     "metric_at_low": lo, "metric_at_high": hi, "swing": abs(hi - lo),
                     "swing_pct_of_base": abs(hi - lo) / base * 100 if base else 0.0})
    rows.sort(key=lambda r: -r["swing"])
    return {"om": om_key, "accounts": accounts, "metric": metric, "base": base, "rows": rows}


# ----------------------------------------------------------------------------
# Revenue models
# ----------------------------------------------------------------------------

def tiered_fee(balance, tiers):
    fee, rem = 0.0, balance
    for width, bps in tiers:
        take = min(rem, width)
        fee += take * bps / 1e4
        rem -= take
        if rem <= 0:
            break
    return fee


def revenue_per_account(cfg, model_key, seg_key):
    m = cfg["revenue_models"][model_key]
    seg = cfg["segments"][seg_key]
    b = seg["balance"]
    if "tiers_bps" in m:
        return max(m["minimum"], tiered_fee(b, m["tiers_bps"]))
    if model_key == "asset_flat_no_min":
        return b * m["flat_bps"] / 1e4
    if model_key == "subscription":
        return m["per_segment_annual"].get(seg_key, 60)
    if model_key == "transaction":
        return seg["grants"] * m["per_grant"] + seg["contribs"] * min(m["contrib_cap"], seg["avg_contrib"] * m["contrib_pct"])
    if model_key == "employer_sponsored":
        return m["employer_per_workplace_account"] if seg_key == "workplace" else revenue_per_account(cfg, m["else"], seg_key)
    if model_key == "advisor_sponsored":
        if seg.get("advisor"):
            return max(250, tiered_fee(b, m["advisor_tiers_bps"])) * m["platform_share"]
        return revenue_per_account(cfg, m["else"], seg_key)
    if model_key == "institutional_licensing":
        return m["per_account"]
    if model_key == "hybrid":
        return m["base"] + b * m["flat_bps"] / 1e4
    if model_key == "transparent_cross_subsidy":
        return b * m["flat_bps"] / 1e4 if b >= m["threshold"] else m["below_threshold_fee"]
    raise KeyError(model_key)


def segment_cost_to_serve(cfg, om_key, accounts):
    """Deterministic marginal (variable) cost per account by segment, plus fixed share."""
    r = run_once(cfg, om_key, accounts, "normal", random.Random(0), noise=False)
    v = r["values"]
    loaded_min = v["loaded_cost_per_fte"] / (v["productive_hours"] * 60.0)
    fixed_total = sum(l["usd"] for l in r["lines"]
                      if l["bearer"] in ("sponsor", "platform_operator") and l["level"] in ("platform", "sponsor")
                      and l["type"] != "acquisition")
    fixed_pa = fixed_total / accounts
    out = {}
    for s, d in r["per_segment"].items():
        n = d["accounts"]
        if n <= 0:
            continue
        labour_min = (d["exceptions"] * v["minutes_per_exception"] + d["complex"] * v["complex_hours"] * 60
                      + d["contact_minutes"] + d["breaks"] * v["break_minutes"] + d["returns"] * v["return_minutes"])
        labour = labour_min * loaded_min * v["staffing_headroom"]
        nonlab = (d["grant_rail"] + d["contrib_rail"] + d["grants"] * (v["routine_eligibility_per_grant"] + v["ai_cost_per_grant_screen"])
                  + d["exceptions"] * v["ai_cost_per_dossier"]
                  + n * (v["infra_per_account"] + v["recurring_obligations_per_account"] + v["ai_interactions_per_account"] * v["ai_cost_per_interaction"]))
        marginal = (labour + nonlab) / n
        out[s] = {"marginal_variable_per_account": marginal, "labour_per_account": labour / n,
                  "nonlabour_per_account": nonlab / n, "fully_allocated_per_account": marginal + fixed_pa,
                  "fund_borne_per_account": (d["aum"] * (v["custody_bps"] + v["pool_expense_bps"]) / 1e4 + d["complex_vendor_fund"]) / n,
                  "balance": cfg["segments"][s]["balance"],
                  "revenue": {m: revenue_per_account(cfg, m, s) for m in cfg["revenue_models"]}}
    return {"om": om_key, "accounts": accounts, "fixed_per_account": fixed_pa, "segments": out}


# ----------------------------------------------------------------------------
# Marginal cost floor
# ----------------------------------------------------------------------------

def marginal_floor(cfg, om_key):
    v = apply_overrides(expected_values(cfg), cfg["operating_models"][om_key]["overrides"])
    lm = v["loaded_cost_per_fte"] / (v["productive_hours"] * 60.0)
    e_eff = v["exception_rate"] * (1 - v["determination_reuse"])
    rows = {
        "additional_account_per_year": {
            "theoretical": v["infra_per_account"] * 0.1 + 0.0,
            "practical": (v["infra_per_account"] + v["recurring_obligations_per_account"]
                          + v["ai_interactions_per_account"] * v["ai_cost_per_interaction"]
                          + 0.3 * v["contacts_mult"] * v["contact_minutes"] * lm),
            "one_off_onboarding": v["identity_per_new"],
            "basis": "theoretical = 10% of infra_per_account (storage/compute only; BENCH shows compute not binding) [A]; practical adds recurring obligations, AI service and 0.3 x contacts_mult human contacts"},
        "contribution_ach": {"theoretical": v["ach_debit_cost"],
                             "practical": v["ach_debit_cost"] + v["ach_return_rate"] * (3.5 + v["return_minutes"] * lm) + v["recon_break_rate"] * v["break_minutes"] * lm,
                             "basis": "ACH entry + expected return handling ($3.5 midpoint of BNK $2-5) + expected break"},
        "contribution_payroll": {"theoretical": v["payroll_item_cost"], "practical": v["payroll_item_cost"] + v["recon_break_rate"] * 0.1 * v["break_minutes"] * lm,
                                 "basis": "batched employer file; breaks reconciled per batch [A]"},
        "contribution_check_lockbox": {"theoretical": v["lockbox_check_cost"], "practical": v["lockbox_check_cost"] + 3 * lm + v["recon_break_rate"] * v["break_minutes"] * lm,
                                       "basis": "Sunwest $0.65 [S] + 3 min manual exception [A]"},
        "grant_ach": {"theoretical": v["ach_grant_cost"] + v["routine_eligibility_per_grant"] + v["ai_cost_per_grant_screen"],
                      "practical": (v["ach_grant_cost"] + v["routine_eligibility_per_grant"] + v["ai_cost_per_grant_screen"]
                                    + e_eff * (v["minutes_per_exception"] * lm * v["staffing_headroom"] + v["ai_cost_per_dossier"])
                                    + v["recon_break_rate"] * v["break_minutes"] * lm),
                      "basis": "rail + routine eligibility ($0.14 [S] GRT:206) + AI screen + expected exception labour"},
        "grant_check": {"theoretical": v["check_grant_cost"], "practical": v["check_grant_cost"] + 0.02 * 25,
                        "basis": "all-in check + 2% reissue at $25 [A] (BNK:123)"},
        "charity_verification_routine": {"theoretical": v["routine_eligibility_per_grant"], "practical": v["routine_eligibility_per_grant"],
                                         "basis": "$0.14 per listed domestic public charity check [S]; reusable across all grants to that EIN within validity"},
        "charity_verification_nonstandard": {"theoretical": 4000 / 2.0 / 12.5, "practical": 4000 / 2.0,
                                             "basis": "RCF typical international ~$4k [S] GRT:208; ED validity ~2 yrs [S]; theoretical spreads over ~12.5 grants per recipient (FSM:91) [S]"},
        "sponsor_per_year": {"theoretical": v["sponsor_platform_cost"] * 0.2, "practical": v["sponsor_platform_cost"] + v["sponsor_onboarding_cost"] / 5.0,
                             "sponsor_entity_cost_borne_by_sponsor": v["sponsor_entity_cost"],
                             "basis": "theoretical = default policy pack with no sponsor-specific configuration [A]"},
        "advisor_relationship_per_year": {"theoretical": v["advisor_relationship_cost"] * 0.2, "practical": v["advisor_relationship_cost"] + v["advisor_onboarding_cost"] / 5.0,
                                          "basis": "[A] firm-level federation and fee feed vs per-advisor manual entitlement"},
        "payment_failure_case": {"theoretical": v["return_minutes"] * lm * 0.2, "practical": v["return_minutes"] * lm + 3.5,
                                 "basis": "[A] auto-retry with typed return codes vs manual"},
        "service_case_ai_resolved": {"theoretical": v["ai_cost_per_interaction"], "practical": 0.50,
                                     "basis": "SVC:172 <$0.50 plus corpus governance (illustrative)"},
        "service_case_human_async": {"theoretical": 7.0, "practical": v["contact_minutes"] * lm,
                                     "basis": "SVC:170-171 async ~$7 at $1/min (illustrative); practical at modelled minutes x loaded rate"},
        "exception_human": {"theoretical": v["minutes_per_exception"] * lm, "practical": v["minutes_per_exception"] * lm * v["staffing_headroom"] + v["ai_cost_per_dossier"],
                            "basis": "minutes x loaded $/min; SVC:173 ~$46 illustrative"},
    }
    return {"om": om_key, "loaded_cost_per_minute": lm, "rows": rows}


# ----------------------------------------------------------------------------
# Target engineering and capital
# ----------------------------------------------------------------------------

def det_cost(cfg, om_key, accounts, override=None, metric="cost_per_account"):
    return run_once(cfg, om_key, accounts, "normal", random.Random(0), noise=False, value_override=override)["metrics"][metric]


def min_scale_for(cfg, om_key, target, grid):
    for a in grid:
        if det_cost(cfg, om_key, a) <= target:
            return a
    return None


def max_exception_minutes_per_grant(cfg, om_key, accounts, target):
    """Bisection on minutes_per_exception (others expected) to hit target cost/account."""
    om = cfg["operating_models"][om_key]
    base = expected_values(cfg)
    mult = om["overrides"].get("minutes_per_exception", {}).get("mult", 1.0)
    lo, hi = 0.0, 2000.0
    if det_cost(cfg, om_key, accounts, {"minutes_per_exception": 1e-6}) > target:
        return None
    for _ in range(40):
        mid = (lo + hi) / 2
        if det_cost(cfg, om_key, accounts, {"minutes_per_exception": mid}) <= target:
            lo = mid
        else:
            hi = mid
    v = apply_overrides(dict(base, minutes_per_exception=lo), om["overrides"])
    return {"minutes_per_exception_input": lo, "effective_minutes": lo * mult,
            "effective_exception_rate": v["exception_rate"] * (1 - v["determination_reuse"]),
            "exception_minutes_per_grant": v["exception_rate"] * (1 - v["determination_reuse"]) * lo * mult}


def ramp_capital(cfg, om_key, target_accounts, price_per_account, years=5):
    """Deterministic 5-year ramp, accounts_y = target*(y/years)^1.5 [A]. Operator view:
    self-operated sponsors (OM1/OM2) bear CAC on max(growth, 15% churn-replacement);
    platform models bear only operator costs (sponsor entity costs and CAC sit with sponsors/employers)."""
    v = expected_values(cfg)
    om = cfg["operating_models"][om_key]
    cum, peak, rows, prev = -v["build_capex"], v["build_capex"], [], 0
    for y in range(1, years + 1):
        a = max(1000, int(target_accounts * (y / years) ** 1.5))
        r = run_once(cfg, om_key, a, "normal", random.Random(0), noise=False)
        cost = r["metrics"]["operator_only_per_account"] * a
        if not om["platform"]:
            cost += v["cac"] * max(a - prev, v["new_account_share"] * a)
        rev = price_per_account * a
        prev = a
        cum += rev - cost
        peak = max(peak, -cum)
        rows.append({"year": y, "accounts": a, "revenue": rev, "cost": cost, "cumulative": cum})
    return {"om": om_key, "target_accounts": target_accounts, "peak_capital": peak, "years": rows}


def blended_asset_revenue(cfg, mix_key, model="asset_tiered_min100"):
    mix = cfg["mixes"][mix_key]
    return sum(share * revenue_per_account(cfg, model, s) for s, share in mix.items())


# ----------------------------------------------------------------------------
# Main
# ----------------------------------------------------------------------------

HEAD_METRICS = ["cost_per_account", "operator_only_per_account", "cost_per_account_incl_acq", "baseline09_comparable_per_account",
                "account_level_per_account", "platform_fixed_per_account", "sponsor_level_per_sponsor",
                "fund_borne_per_account", "cost_per_transaction", "human_interventions_per_1000_accounts",
                "exception_rate_per_grant", "peak_backlog_exceptions", "p90_wait_days_exceptions",
                "sla_attainment_overall", "sla_attainment_exceptions", "recovery_days_max_queue",
                "financial_exposure_gross", "financial_exposure_uncontained", "control_effectiveness_proxy",
                "staff_fte_base", "surge_fte_equivalent", "utilisation_exceptions", "customer_friction_per_1000",
                "unresolved_backlog_items_after_ext", "unresolved_risk_usd"]


def main(argv):
    pfile = argv[1] if len(argv) > 1 else os.path.join(HERE, "parameters.json")
    outdir = argv[2] if len(argv) > 2 else HERE
    cfg = load(pfile)
    scales = cfg["scales"]
    n_norm, n_scen = cfg["meta"]["mc_runs_normal"], cfg["meta"]["mc_runs_scenarios"]
    results = {"meta": {"seed": cfg["meta"]["seed"], "command": "python3 -I -B model.py",
                        "mc_runs_normal": n_norm, "mc_runs_scenarios": n_scen,
                        "disclaimer": "THEORETICAL MODEL OUTPUTS from labelled assumptions; not observations."}}
    # 0. replication of the 09 baseline
    results["replication_09"] = {str(a): det_cost(cfg, "OM0_REPLICA", a, metric="baseline09_comparable_per_account")
                                 for a in (1000, 10000, 100000, 1000000)}
    # 1. normal-scenario MC per operating model and scale
    normal, sampled = {}, {}
    for om in ("OM1", "OM2", "OM3", "OM4", "OM5"):
        for a in scales:
            runs = mc(cfg, om, a, "normal", n_norm)
            normal[f"{om}|{a}"] = summarise_runs(runs)
            if (om, a) in (("OM1", 100000), ("OM3", 1000000), ("OM4", 1000000)):
                sampled[f"{om}|{a}"] = [(r["values"], r["metrics"]["cost_per_account"]) for r in runs]
    results["normal_mc"] = {k: {m: s[m] for m in HEAD_METRICS} for k, s in normal.items()}
    # 2. scenarios: OM3 at every scale, OM1 at 100k
    scen, deltas, base_runs = {}, {}, {}
    dkeys = ("cost_per_account", "sla_attainment_overall", "financial_exposure_uncontained",
             "human_interventions_per_1000_accounts", "customer_friction_per_1000")
    for sk in cfg["scenarios"]:
        for om, a in [("OM3", s) for s in scales] + [("OM1", 100000)]:
            runs = mc(cfg, om, a, sk, n_scen)
            summ = summarise_runs(runs)
            scen[f"{om}|{a}|{sk}"] = {m: summ[m] for m in HEAD_METRICS}
            if sk == "normal":
                base_runs[(om, a)] = [r["metrics"] for r in runs]
            deltas[f"{om}|{a}|{sk}"] = {m: {p: pct([r["metrics"][m] - b[m] for r, b in zip(runs, base_runs[(om, a)])], q)
                                            for p, q in (("p10", 0.1), ("p50", 0.5), ("p90", 0.9))} for m in dkeys}
    results["scenario_mc"] = scen
    results["scenario_paired_delta_vs_normal"] = deltas
    # 3. sensitivity
    results["tornado"] = [tornado(cfg, "OM1", 100000), tornado(cfg, "OM3", 1000000), tornado(cfg, "OM4", 1000000)]
    rank_corr = {}
    for key, pairs in sampled.items():
        names = [k for k, s in cfg["params"].items() if s.get("mc") and s["high"] > s["low"]]
        ys = [c for _, c in pairs]
        rank_corr[key] = sorted(({"param": n, "spearman": spearman([vals[n] for vals, _ in pairs], ys)} for n in names),
                                key=lambda r: -abs(r["spearman"]))
    results["rank_correlation"] = rank_corr
    # 4. targets
    grid = [1000, 3000, 10000, 30000, 100000, 300000, 1000000, 2000000, 5000000, 10000000]
    tgt = {}
    for t in cfg["targets"]:
        row = {}
        for om in ("OM1", "OM2", "OM3", "OM4", "OM5"):
            row[om] = {"min_scale_deterministic": min_scale_for(cfg, om, t, grid),
                       "p50_by_scale": {str(a): normal[f"{om}|{a}"]["cost_per_account"]["p50"] for a in scales},
                       "p90_by_scale": {str(a): normal[f"{om}|{a}"]["cost_per_account"]["p90"] for a in scales}}
        row["OM3_1M_max_exception_minutes_per_grant"] = max_exception_minutes_per_grant(cfg, "OM3", 1000000, t)
        row["OM4_1M_max_exception_minutes_per_grant"] = max_exception_minutes_per_grant(cfg, "OM4", 1000000, t)
        row["break_even_balance_at_60bps"] = t / 0.006
        tgt[str(t)] = row
    results["targets"] = tgt
    # 5. cost floor, segment economics, revenue, capital
    results["marginal_floor"] = {om: marginal_floor(cfg, om) for om in ("OM1", "OM2", "OM3", "OM4", "OM5")}
    results["segment_economics"] = [segment_cost_to_serve(cfg, "OM1", 100000), segment_cost_to_serve(cfg, "OM3", 1000000),
                                    segment_cost_to_serve(cfg, "OM4", 1000000)]
    results["blended_revenue_per_account"] = {mk: {m: blended_asset_revenue(cfg, mk, m) for m in cfg["revenue_models"]}
                                              for mk in ("market", "digital_minimal")}
    mkt_rev = blended_asset_revenue(cfg, "market")
    small_rev = max(100.0, tiered_fee(100000, cfg["revenue_models"]["asset_tiered_min100"]["tiers_bps"]))
    dm_bal = sum(sh * cfg["segments"][s]["balance"] for s, sh in cfg["mixes"]["digital_minimal"].items())
    results["capital"] = [
        dict(ramp_capital(cfg, "OM1", 100000, mkt_rev), price_basis="market-mix blended Fidelity tiers ($100 min) [S tiers, A mix]"),
        dict(ramp_capital(cfg, "OM1", 100000, small_rev), price_basis="calibrated: blended tiers on a $100k-average book [A] (FSM:150 calibrated path)"),
        dict(ramp_capital(cfg, "OM2", 100000, mkt_rev), price_basis="market-mix blended Fidelity tiers [S tiers, A mix]"),
        dict(ramp_capital(cfg, "OM3", 1000000, 100.0), price_basis="platform fee $100/account [A] (below FSM:140 $200-300)"),
        dict(ramp_capital(cfg, "OM3", 1000000, 150.0), price_basis="platform fee $150/account [S FSM:140 low]"),
        dict(ramp_capital(cfg, "OM4", 1000000, 12.0), price_basis="employer fee $12/account [A] only"),
        dict(ramp_capital(cfg, "OM4", 1000000, 12.0 + 0.003 * dm_bal), price_basis="employer $12 + 30 bps of balance [A]"),
    ]
    # 6. worked examples (deterministic expected-value runs)
    we = {}
    for om, a, sk in [("OM1", 100000, "normal"), ("OM3", 1000000, "normal"), ("OM4", 1000000, "normal"),
                      ("OM3", 100000, "simultaneous_failures"), ("OM3", 100000, "incorrect_ai_outputs"),
                      ("OM3", 10000, "december_peak")]:
        r = run_once(cfg, om, a, sk, random.Random(0), noise=False)
        we[f"{om}|{a}|{sk}"] = {"metrics": r["metrics"], "base_fte": r["base_fte"],
                                "lines": [dict(l, usd=round(l["usd"], 2)) for l in r["lines"]],
                                "queues": {q: {k: r["sims"][q][k] for k in ("arrivals", "peak_backlog", "sla_attainment", "p90_wait_days", "contractor_fte_days", "recovery_days", "recovered")} for q in QUEUES},
                                "exposure": r["exposure"]}
    results["worked_examples"] = we
    with open(os.path.join(outdir, "results.json"), "w") as fh:
        json.dump(results, fh, indent=1, sort_keys=True, default=float)
    with open(os.path.join(outdir, "results.csv"), "w", newline="") as fh:
        w = csv.writer(fh)
        w.writerow(["table", "operating_model", "accounts", "scenario", "metric", "p10", "p50", "p90"])
        for k, s in results["normal_mc"].items():
            om, a = k.split("|")
            for m, q in s.items():
                w.writerow(["normal_mc", om, a, "normal", m, round(q["p10"], 6), round(q["p50"], 6), round(q["p90"], 6)])
        for k, s in results["scenario_mc"].items():
            om, a, sk = k.split("|")
            for m, q in s.items():
                w.writerow(["scenario_mc", om, a, sk, m, round(q["p10"], 6), round(q["p50"], 6), round(q["p90"], 6)])
        for k, s in results["scenario_paired_delta_vs_normal"].items():
            om, a, sk = k.split("|")
            for m, q in s.items():
                w.writerow(["scenario_delta_vs_normal", om, a, sk, m, round(q["p10"], 6), round(q["p50"], 6), round(q["p90"], 6)])
        for t in results["tornado"]:
            for r in t["rows"]:
                w.writerow(["tornado", t["om"], t["accounts"], "normal", r["param"], round(r["metric_at_low"], 6), round(t["base"], 6), round(r["metric_at_high"], 6)])
    print("replication_09", {k: round(x, 2) for k, x in results["replication_09"].items()})
    for k, s in results["normal_mc"].items():
        print(k, "cost/acct P10/P50/P90", round(s["cost_per_account"]["p10"], 2), round(s["cost_per_account"]["p50"], 2), round(s["cost_per_account"]["p90"], 2))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

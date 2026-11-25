#!/usr/bin/env python3
"""Phase 4: activity-based economics of five target DAF enterprises built on Unbound.

Imports the executed simplification model (model.py) and the decision-team restructuring
(decision_teams.py) UNCHANGED and adds:

  * five target enterprises (E1-E5) with their own account mixes, scales, routes and revenue models;
  * four operating approaches per enterprise:
        A traditional human-centred, self-built systems
        AL A's workflows on licensed software (isolates workflow from technology sourcing)
        B existing workflow enhanced with AI, self-built systems
        C AI-native redesign on Unbound (per-sponsor decision teams, licence fee)
        D C plus elimination of convention-only activities
  * an activity map that assigns every cost line to a business activity and a bearer, so that
    transfers between bearers are reported separately and never counted as savings;
  * base / favourable / adverse ranges (Monte Carlo P50 / P10 / P90 of total economic cost);
  * one-way sensitivity, stress scenarios and revenue / margin by enterprise.

THEORETICAL MODEL. Every number follows from labelled assumptions: model.py's parameters.json (labels
S = sourced, A = assumption, D = derived) and the [A] values in ENTERPRISES, APPROACHES and LICENCE
below. Nothing is an observation of a real organization.

Run: python3 -I -B enterprise_economics.py [output_dir]
Writes enterprise-results.json, activity-costs.csv, sensitivity.csv, run.log.
"""
import csv
import io
import json
import math
import os
import random
import sys
import zlib
from functools import reduce

HERE = os.path.dirname(os.path.abspath(__file__))
SIMP = os.path.normpath(os.path.join(HERE, "../../../discovery/2026-11-24-simplification-economics/model"))
sys.path.insert(0, SIMP)
import model as M  # noqa: E402
import decision_teams as DT  # noqa: E402

SEED = 20261124
MC_RUNS = 300

# ---------------------------------------------------------------------------- assumptions [A]

MIXES = {
    # [A] community foundation DAF book: fewer workplace and tiny accounts, more advisor-referred and HNW funds
    "community_foundation": {"inactive": 0.20, "small_recurring": 0.08, "typical": 0.32, "highly_active": 0.05,
                             "hnw": 0.14, "institutional": 0.03, "advisor_managed": 0.18},
    # [A] wealth-management distributed book
    "advisor_distributed": {"advisor_managed": 0.62, "hnw": 0.18, "typical": 0.15, "inactive": 0.05},
}

ENTERPRISES = {
    "E1": {"name": "Independent national DAF sponsor (direct to donor)", "mix": "market", "route": "sponsor_direct",
           "accounts": 50000, "scales": (10000, 50000, 250000), "accounts_per_sponsor": None,
           "revenue": "asset_tiered_min100", "cac_bearer": "sponsor", "complex_share_mult": 1.0,
           "basis": "daf-landscape business model D2C; market mix from parameters.json"},
    "E2": {"name": "Community foundation (DAF program inside a broader foundation)", "mix": "community_foundation",
           "route": "sponsor_direct", "accounts": 128, "scales": (46, 128, 313, 1500), "accounts_per_sponsor": None,
           "revenue": "asset_tiered_min250", "cac_bearer": "sponsor", "complex_share_mult": 1.5,
           "basis": "[S] DAFRC FY2024 panel, 802 community foundations: median 46, p75 128, p90 313 DAFs; only 7 hold >=1,500 "
                    "(daf-landscape data/mission-2026-11-14/dafrc-sponsor-schedule-d-fy2019-2024.csv); non-DAF programs excluded"},
    "E3": {"name": "Wealth-management and advisor-distributed DAF provider", "mix": "advisor_distributed",
           "route": "sponsor_direct", "accounts": 40000, "scales": (10000, 40000, 200000), "accounts_per_sponsor": None,
           "revenue": "asset_tiered_min250", "cac_bearer": "sponsor", "complex_share_mult": 1.3,
           "revenue_later": "advisor_sponsored",
           "basis": "daf-landscape advisor-distribution model; release 1 has no advisor-managed program and no fund-paid advisor "
                    "compensation (CNV-GRT-009), so release-1 revenue is the sponsor admin fee only; advisor_sponsored is a later option"},
    "E4": {"name": "High-volume workplace giving provider", "mix": "digital_minimal", "route": "sponsor_direct",
           "accounts": 500000, "scales": (100000, 500000, 2000000), "accounts_per_sponsor": None,
           "revenue": "employer_sponsored", "cac_bearer": "employer", "complex_share_mult": 1.0,
           "basis": "daf-landscape high-volume-low-balance and embedded models; employer pays acquisition"},
    "E5": {"name": "Enterprise outsourced DAF administrator serving many sponsors", "mix": "market",
           "route": "via_administrator", "accounts": 250000, "scales": (50000, 250000, 1000000), "accounts_per_sponsor": 5000,
           "revenue": "institutional_licensing", "cac_bearer": "sponsor", "complex_share_mult": 1.0,
           "basis": "daf-landscape outsourced-admin/white-label; decisions stay with each sponsor (SPD-ADV-001)"},
}

LEVERS_C = {"determination_reuse": {"set": 0.5}, "minutes_per_exception": {"mult": 0.667}, "contacts_mult": {"mult": 0.6},
            "recon_break_rate": {"mult": 0.3}, "complex_hours": {"set": 2.0}, "seasonal_frac": {"set": 0.5},
            "recurring_obligations_per_account": {"mult": 0.5}, "check_share_mult": {"set": 0.2}}
APPROACHES = {
    "A": {"name": "Traditional human-centred operation", "platform": False, "outsource_complex": False,
          "overrides": {"exception_rate": {"set": 0.30}, "minutes_per_exception": {"mult": 1.0}, "contacts_mult": {"mult": 1.3},
                        "ai_cost_per_dossier": {"set": 0.0}, "ai_interactions_per_account": {"set": 0.0},
                        "ai_cost_per_grant_screen": {"set": 0.0}, "recurring_obligations_per_account": {"mult": 1.5},
                        "check_share_mult": {"set": 1.5}},
          "basis": "[A] per-item human review: 30% of grants touched by a person; no AI; more paper and contacts"},
    "AL": {"name": "Traditional human-centred operation on licensed software", "platform": True, "outsource_complex": False,
           "overrides": {"exception_rate": {"set": 0.30}, "contacts_mult": {"mult": 1.3},
                         "ai_cost_per_dossier": {"set": 0.0}, "ai_interactions_per_account": {"set": 0.0},
                         "ai_cost_per_grant_screen": {"set": 0.0}, "recurring_obligations_per_account": {"mult": 1.5},
                         "check_share_mult": {"set": 1.5}},
           "basis": "[A] A's human workflows, but technology bought at the same licence as C; isolates the workflow effect "
                    "from the build-versus-buy effect"},
    "A8": {"name": "Traditional operation, same exception definition as B-D (8%)", "platform": False, "outsource_complex": False,
           "overrides": {"contacts_mult": {"mult": 1.3}, "ai_cost_per_dossier": {"set": 0.0}, "ai_interactions_per_account": {"set": 0.0},
                         "ai_cost_per_grant_screen": {"set": 0.0}, "recurring_obligations_per_account": {"mult": 1.5},
                         "check_share_mult": {"set": 1.5}},
           "basis": "Adversarial correction ADV-B-005: A with the same exception incidence as the other approaches; the 30% "
                    "per-item-review assumption in A is a separate, unmeasured lever"},
    "C0": {"name": "Unbound without unmeasured redesign levers", "platform": True, "outsource_complex": False, "overrides": {},
           "basis": "Adversarial correction ADV-B-006: C's platform, licence and per-sponsor teams with B's workflow parameters "
                    "(no determination reuse, no minute/contact/reconciliation/complex-hour reductions)"},
    "B": {"name": "Existing workflow enhanced with AI", "platform": False, "outsource_complex": False, "overrides": {},
          "basis": "OM1 levers (parameters.json): exception-based review, AI assistance, same processes"},
    "C": {"name": "AI-native redesign on Unbound", "platform": True, "outsource_complex": True, "overrides": LEVERS_C,
          "basis": "OM2/OM3 levers; Unbound licence; per-sponsor decision teams (SPD-ADV-001, SPD-ADV-004)"},
    "D": {"name": "AI-native redesign plus elimination of convention-only activities", "platform": True, "outsource_complex": True,
          "overrides": dict(LEVERS_C, **{"recurring_obligations_per_account": {"mult": 0.25}, "check_share_mult": {"set": 0.05},
                                         "contacts_mult": {"mult": 0.45}, "determination_reuse": {"set": 0.6}}),
          "basis": "[A] removes convention-only recurring obligations, default-ACH, proactive notices; legal obligations retained"},
}

LICENCE = {"platform_accounts": {"favourable": 250000, "base": 100000, "adverse": 25000, "market_exceeding_reference": 1000000},
           "markup": {"value": 0.30, "low": 0.10, "high": 0.60},
           "basis": "[A] Unbound's vendor cost per account (decision_teams.license_floors) at the stated whole-platform scale, plus markup. "
                    "Scales corrected after adversarial review ADV-B-001: the US market holds at most ~737k traditional invested DAF "
                    "accounts and only 20 sponsors hold >=5,000 (DAFRC FY2024); 1M is kept only as a labelled reference."}
VENDOR = {"build_capex_amortisation_years": 5, "sales_cost_per_new_sponsor": 40000, "new_sponsor_share": 0.2,
          "basis": "[A] build_capex from parameters.json amortised over 5 years; sales and implementation per new sponsor; 20% of "
                   "sponsors new each year. Sponsor onboarding cost is already in the vendor floor (sponsor_level:platform_service)."}
PLATFORM_SCALES = (25000, 50000, 100000, 250000, 500000, 1000000)

ACTIVITY = {
    "customer_acquisition": "acquisition", "identity_new_accounts": "onboarding_identity",
    "payments:contributions": "contributions", "staff:queue:complex": "contributions_complex_assets",
    "complex_asset_vendor": "contributions_complex_assets", "card_fees": "contributions",
    "custody": "investment_administration", "pool_investment_expense": "investment_management",
    "eligibility_screening": "charity_vetting_and_grant_decisions", "staff:queue:exceptions": "charity_vetting_and_grant_decisions",
    "data_providers": "charity_vetting_and_grant_decisions", "staff:philanthropic_intelligence": "charity_vetting_and_grant_decisions",
    "payments:grants": "payment_processing", "staff:queue:service": "customer_service",
    "staff:queue:ops": "reconciliation_accounting_exceptions", "recurring_obligations": "statements_and_recurring_obligations",
    "external_audit": "compliance_and_assurance", "legal_filings": "compliance_and_assurance", "insurance": "compliance_and_assurance",
    "infrastructure_base": "technology", "infrastructure_per_account": "technology", "staff:engineering": "technology",
    "staff:security": "technology", "staff:agent_ownership": "technology", "unbound_licence": "technology",
    "ai_inference": "ai_usage", "staff:leadership": "management_overhead", "staff:partnerships": "management_overhead",
    "sponsor_level:entity_costs": "management_overhead", "advisor_relationships": "advisor_service",
    "sponsor_level:platform_service": "sponsor_onboarding_and_exit", "tenant:authority_setup": "sponsor_onboarding_and_exit",
}
QUEUE_ACTIVITY = {"exceptions": "charity_vetting_and_grant_decisions", "complex": "contributions_complex_assets",
                  "service": "customer_service", "ops": "reconciliation_accounting_exceptions"}


def activity_of(line):
    if line.startswith("staff:surge:"):
        return QUEUE_ACTIVITY[line.split(":")[2]]
    return ACTIVITY.get(line, "other")


# ---------------------------------------------------------------------------- configuration

COMPLEX_POLICY = {"complex_hours": 1.0, "share_factor": 0.25,
                  "basis": "OD-M-09 (owner decision): complex assets are rare and are sold on receipt by a contracted intermediary, "
                           "so internal handling is intake checks and proceeds reconciliation (1 hour [A]) and the complex share is a "
                           "quarter of the earlier mix assumption [A]. Applied to every approach, so it is a business policy, not an "
                           "Unbound-only lever; the intermediary fee stays fund-borne."}


def config(base_cfg):
    """parameters.json plus the enterprise x approach operating models (pure)."""
    oms = {f"{e}_{a}": {"name": f"{E['name']} / {A['name']}", "mix": E["mix"], "platform": A["platform"],
                        "cac_bearer": E["cac_bearer"] if E["cac_bearer"] != "employer" else "employer",
                        "contractors": True, "outsource_complex": True,
                        "overrides": dict(A["overrides"], complex_share_mult={"set": E["complex_share_mult"] * COMPLEX_POLICY["share_factor"]},
                                          complex_hours={"set": COMPLEX_POLICY["complex_hours"]},
                                          **({"accounts_per_sponsor": {"set": E["accounts_per_sponsor"]}} if E["accounts_per_sponsor"] else {}))}
           for e, E in ENTERPRISES.items() for a, A in APPROACHES.items()}
    return dict(base_cfg, mixes=dict(base_cfg["mixes"], **MIXES), operating_models=dict(base_cfg["operating_models"], **oms))


def sponsors_for(e, accounts):
    E = ENTERPRISES[e]
    return max(1, math.ceil(accounts / E["accounts_per_sponsor"])) if E["accounts_per_sponsor"] else 1


def licence_per_account(cfg, rng_dt, which="base", markup=None, noise=False):
    """Vendor cost per account at whole-platform scale (OM3 licence floor) plus markup."""
    dt = DT.dt_sample(rng_dt) if noise else DT.dt_expected()
    scale = which if isinstance(which, int) else LICENCE["platform_accounts"][which]
    floors = DT.license_floors(cfg, "OM3", scale, dt)
    per_tenant_amortised = floors["B_tenant_fee_per_sponsor_floor"] / 5000.0
    m = LICENCE["markup"]["value"] if markup is None else markup
    return (floors["A_licence_fee_per_account_floor"] + per_tenant_amortised) * (1 + m)


# ---------------------------------------------------------------------------- one evaluation

def restructure_for(run, om, accounts, e, approach, dt):
    """Lines as borne by the enterprise. A/B: own systems, own staff (model.py lines as is).
    C/D: per-sponsor decision teams; vendor lines replaced by a licence charge."""
    if not APPROACHES[approach]["platform"]:
        return tuple(dict(l, bearer=("enterprise" if l["bearer"] in ("sponsor", "platform_operator") else l["bearer"]))
                     for l in run["lines"])
    n = sponsors_for(e, accounts)
    v = dict(run["values"], accounts_per_sponsor=accounts / n)
    added, _, _, replaced = DT.staff_lines(om, accounts, v, run["volumes"], dt, ENTERPRISES[e]["route"])
    dropped = {"staff:queue:" + q for q in replaced}
    kept = tuple(l for l in DT.rebear(run["lines"]) if l["line"] not in dropped and l["bearer"] != "vendor")
    return tuple(dict(l, bearer=("enterprise" if l["bearer"] == "customer" else l["bearer"])) for l in kept + added)


def split_direct_team(lines, run):
    """Allocate combined sponsor teams to activities pro rata to modelled queue workload."""
    fte = run["base_fte"]
    tot = sum(fte.values()) or 1.0
    def explode(l):
        if l["line"] == "sponsor_staff:all_queues":
            return tuple(dict(l, line=f"staff:queue:{q}", usd=l["usd"] * fte[q] / tot) for q in fte)
        if l["line"] == "sponsor_staff:decision_team":
            res = {q: fte[q] for q in DT.RESERVED}
            rt = sum(res.values()) or 1.0
            return tuple(dict(l, line=f"staff:queue:{q}", usd=l["usd"] * res[q] / rt) for q in res)
        return (l,)
    return tuple(x for l in lines for x in explode(l))


def evaluate(cfg, e, approach, accounts, rng, dt_rng, noise, scen="normal", licence_which="base", markup=None, override=None):
    om_key = f"{e}_{approach}"
    om = cfg["operating_models"][om_key]
    run = M.run_once(cfg, om_key, accounts, scen, rng, noise=noise, value_override=override)
    dt = DT.dt_sample(dt_rng) if noise else DT.dt_expected()
    lines = split_direct_team(restructure_for(run, om, accounts, e, approach, dt), run)
    ay = run["metrics"]["accounts_year_avg"]
    lic = (licence_per_account(cfg, dt_rng, licence_which, markup, noise=False) * ay,) if APPROACHES[approach]["platform"] else ()
    lines = lines + tuple(M.line("unbound_licence", u, "variable", "enterprise", "account") for u in lic)
    return {"lines": lines, "accounts_year": ay, "metrics": run["metrics"], "values": run["values"]}


def summarise(ev):
    ay = ev["accounts_year"]
    by = lambda pred: sum(l["usd"] for l in ev["lines"] if pred(l)) / ay
    acq = lambda l: l["line"] == "customer_acquisition"
    enterprise_admin = by(lambda l: l["bearer"] == "enterprise" and not acq(l))
    fund_admin = by(lambda l: l["bearer"] in ("fund", "donor_or_fund") and l["line"] != "pool_investment_expense")
    return {
        "enterprise_admin_per_account": enterprise_admin,
        "fund_borne_admin_per_account": fund_admin,
        "total_economic_admin_per_account": enterprise_admin + fund_admin,
        "acquisition_per_account": by(acq),
        "investment_management_per_account": by(lambda l: l["line"] == "pool_investment_expense"),
        "human_interventions_per_1000": ev["metrics"]["human_interventions_per_1000_accounts"],
        "staff_fte_queue_equivalent": sum(l["usd"] for l in ev["lines"] if l["line"].startswith("staff:queue")) / ev["values"]["loaded_cost_per_fte"],
        "activities": {a: by(lambda l, a=a: activity_of(l["line"]) == a and l["line"] != "pool_investment_expense")
                       for a in sorted(set(ACTIVITY.values()) | {"other"})},
        "bearers": {b: by(lambda l, b=b: l["bearer"] == b) for b in sorted({l["bearer"] for l in ev["lines"]})},
        "fixed_per_account": by(lambda l: l["type"] in ("fixed", "step_fixed") and l["bearer"] == "enterprise"),
        "variable_per_account": by(lambda l: l["type"].startswith("variable") and l["bearer"] == "enterprise"),
    }


def seeds(*parts):
    return zlib.crc32("|".join(map(str, (SEED,) + parts)).encode())


def mc(cfg, e, approach, accounts, n=MC_RUNS):
    runs = tuple(summarise(evaluate(cfg, e, approach, accounts, random.Random(seeds(e, approach, accounts, i)),
                                    random.Random(seeds("dt", e, approach, accounts, i)), True))
                 for i in range(n))
    key = "total_economic_admin_per_account"
    xs = sorted(r[key] for r in runs)
    return {"favourable_p10": M.pct(xs, 0.1), "base_p50": M.pct(xs, 0.5), "adverse_p90": M.pct(xs, 0.9),
            "enterprise_admin_p50": M.pct(sorted(r["enterprise_admin_per_account"] for r in runs), 0.5)}


def det(cfg, e, approach, accounts, **kw):
    return summarise(evaluate(cfg, e, approach, accounts, random.Random(0), random.Random(0), False, **kw))


# ---------------------------------------------------------------------------- revenue

def blended_revenue(cfg, e):
    mix = cfg["mixes"][ENTERPRISES[e]["mix"]]
    return sum(share * M.revenue_per_account(cfg, ENTERPRISES[e]["revenue"], s) for s, share in mix.items())


def revenue_under_market(cfg, e, aum_mult):
    """Asset-based revenue scales with balances; minimums and per-account fees do not."""
    mix = cfg["mixes"][ENTERPRISES[e]["mix"]]
    segs = {s: dict(cfg["segments"][s], balance=cfg["segments"][s]["balance"] * aum_mult) for s in mix}
    c2 = dict(cfg, segments=dict(cfg["segments"], **segs))
    return sum(share * M.revenue_per_account(c2, ENTERPRISES[e]["revenue"], s) for s, share in mix.items())


# ---------------------------------------------------------------------------- sensitivity

SENS_PARAMS = ("ai_cost_per_dossier", "ai_cost_per_interaction", "exception_rate", "minutes_per_exception",
               "ach_grant_cost", "routine_eligibility_per_grant", "cac", "loaded_cost_per_fte",
               "contacts_mult", "recon_break_rate", "infra_per_account", "complex_hours")


def sensitivity(cfg, e, accounts):
    p = cfg["params"]
    base = det(cfg, e, "C", accounts)["total_economic_admin_per_account"]
    def one(k):
        lo = det(cfg, e, "C", accounts, override={k: p[k]["low"]})["total_economic_admin_per_account"]
        hi = det(cfg, e, "C", accounts, override={k: p[k]["high"]})["total_economic_admin_per_account"]
        return {"enterprise": e, "parameter": k, "low_value": p[k]["low"], "high_value": p[k]["high"], "label": p[k]["label"],
                "cost_at_low": lo, "cost_at_high": hi, "base": base, "swing": abs(hi - lo)}
    rows = tuple(one(k) for k in SENS_PARAMS)
    licence = tuple({"enterprise": e, "parameter": f"licence:{w}", "low_value": "", "high_value": "", "label": "A",
                     "cost_at_low": det(cfg, e, "C", accounts, licence_which=w)["total_economic_admin_per_account"],
                     "cost_at_high": det(cfg, e, "C", accounts, licence_which=w, markup=LICENCE["markup"]["high"])["total_economic_admin_per_account"],
                     "base": base, "swing": 0.0} for w in ("favourable", "adverse"))
    return tuple(sorted(rows, key=lambda r: -r["swing"])) + licence


STRESS = ("december_peak", "market_downturn", "payment_failures", "fraud_attempts", "staff_absence", "vendor_outage",
          "ai_outage", "incorrect_ai_outputs", "regulatory_change", "sponsor_termination", "simultaneous_failures")


def stress(cfg, e, accounts):
    base = det(cfg, e, "C", accounts)
    def one(s):
        ev = evaluate(cfg, e, "C", accounts, random.Random(0), random.Random(0), False, scen=s)
        sm = summarise(ev)
        return {"enterprise": e, "scenario": s, "total_economic_admin_per_account": sm["total_economic_admin_per_account"],
                "delta_vs_normal": sm["total_economic_admin_per_account"] - base["total_economic_admin_per_account"],
                "sla_attainment_exceptions": ev["metrics"]["sla_attainment_exceptions"],
                "p90_wait_days_exceptions": ev["metrics"]["p90_wait_days_exceptions"],
                "uncontained_exposure_usd": ev["metrics"]["financial_exposure_uncontained"],
                "recovery_days_max_queue": ev["metrics"]["recovery_days_max_queue"]}
    return tuple(one(s) for s in STRESS)


# ---------------------------------------------------------------------------- December surge (TEV-001, ADV-R2-021)

SURGE_LEVELS = (0.5, 1.0, 2.0, 3.0)


def december_surge(cfg, e):
    """Cost and exception-SLA attainment of approach C in the december_peak scenario at several seasonal staffing levels
    (seasonal_frac = extra December staff as a multiple of base queue staff). Note: the queue simulation is the base
    model's pooled queue, so per-sponsor team rounding is not reflected in waiting times."""
    n = ENTERPRISES[e]["accounts"]
    base = det(cfg, e, "C", n)["total_economic_admin_per_account"]
    def one(sf):
        om = dict(cfg["operating_models"][f"{e}_C"], overrides=dict(cfg["operating_models"][f"{e}_C"]["overrides"], seasonal_frac={"set": sf}))
        c2 = dict(cfg, operating_models=dict(cfg["operating_models"], **{f"{e}_C": om}))
        ev = evaluate(c2, e, "C", n, random.Random(0), random.Random(0), False, scen="december_peak")
        return {"enterprise": e, "seasonal_frac": sf, "total_economic_admin_per_account": summarise(ev)["total_economic_admin_per_account"],
                "delta_vs_normal_C": summarise(ev)["total_economic_admin_per_account"] - base,
                "sla_attainment_exceptions": ev["metrics"]["sla_attainment_exceptions"], "p90_wait_days_exceptions": ev["metrics"]["p90_wait_days_exceptions"]}
    return tuple(one(sf) for sf in SURGE_LEVELS)


# ---------------------------------------------------------------------------- staffed administration offer (OD-M-08)

STAFFED = {"team_floor_fte": 4.0, "supervision_share": 0.15, "prep_share": 0.5,
           "ren_price_per_daf": (413.0, 745.0), "ipHi_software_per_daf": (32.0, 129.0),
           "basis": "[A] floor of 4 FTE for coverage and segregation of duties; 15% supervision overhead; Unbound staff prepare "
                    "half of exception and complex-gift work while sponsors' own appointed people decide (SPD-ADV-001). "
                    "[market-data] Ren administration $413-745 per DAF-year and iPhi software $32-129 per DAF-year from Form 990 "
                    "contractor payments (report 13; floors, may bundle services not modelled such as gift consulting)."}
ADMIN_LINES = ("staff:queue:service", "staff:queue:ops", "staff:surge:service", "staff:surge:ops", "payments:grants",
               "payments:contributions", "recurring_obligations", "advisor_relationships", "identity_new_accounts")


def staffed_offer(cfg, admin_funds, licence_which):
    """Unbound's cost per fund to run Ren-class outsourced administration for sponsors of 5,000 funds each (E5 mix),
    with Unbound's own platform at cost (no licence markup)."""
    ev = evaluate(cfg, "E5", "C", admin_funds, random.Random(0), random.Random(0), False, licence_which=licence_which, markup=0.0)
    ay, v = ev["accounts_year"], ev["values"]
    usd = lambda pred: sum(l["usd"] for l in ev["lines"] if pred(l))
    admin_ops = usd(lambda l: l["line"] in ADMIN_LINES or l["line"] == "unbound_licence")
    reserved = usd(lambda l: l["line"] in ("staff:queue:exceptions", "staff:queue:complex", "staff:surge:exceptions", "staff:surge:complex"))
    prep = reserved * STAFFED["prep_share"]
    staff_usd = usd(lambda l: l["line"] in ADMIN_LINES[:4]) + prep
    floor_usd = STAFFED["team_floor_fte"] * v["loaded_cost_per_fte"]
    staff_usd_floored = max(staff_usd, floor_usd)
    cost = admin_ops - usd(lambda l: l["line"] in ADMIN_LINES[:4]) + staff_usd_floored * (1 + STAFFED["supervision_share"]) + prep * 0
    return {"admin_funds": admin_funds, "licence_scale": licence_which, "unbound_admin_cost_per_fund": cost / ay,
            "sponsor_retained_decision_cost_per_fund": (reserved - prep) / ay,
            "team_floor_binding": staff_usd < floor_usd,
            "margin_vs_ren_low": STAFFED["ren_price_per_daf"][0] - cost / ay}


def staffed_table(cfg):
    return tuple(staffed_offer(cfg, n, w) for n in (10000, 25000, 50000, 100000) for w in ("adverse", "base"))


# ---------------------------------------------------------------------------- vendor P&L (ADV-B-002)

def vendor_pnl(cfg, platform_funds, price_per_fund):
    """Unbound's own annual P&L at a platform scale and a licence price (deterministic)."""
    dt = DT.dt_expected()
    floors = DT.license_floors(cfg, "OM3", platform_funds, dt)
    sponsors = floors["sponsors"]
    run_cost = floors["vendor_cost_total_usd"]
    capex = cfg["params"]["build_capex"]["value"] / VENDOR["build_capex_amortisation_years"]
    sales = sponsors * VENDOR["new_sponsor_share"] * VENDOR["sales_cost_per_new_sponsor"]
    cost = run_cost + capex + sales
    revenue = platform_funds * price_per_fund
    return {"platform_funds": platform_funds, "sponsors": sponsors, "price_per_fund": price_per_fund,
            "revenue": revenue, "run_cost": run_cost, "capex_amortisation": capex, "sales_and_implementation": sales,
            "total_cost": cost, "operating_result": revenue - cost, "break_even_price_per_fund": cost / platform_funds}


def vendor_table(cfg):
    return tuple(vendor_pnl(cfg, n, p) for n in PLATFORM_SCALES for p in (28.0, 50.0, 100.0))


# ---------------------------------------------------------------------------- orchestration

def results(base_cfg):
    cfg = config(base_cfg)
    ents = tuple(ENTERPRISES)
    comparison = {e: {a: dict(det(cfg, e, a, ENTERPRISES[e]["accounts"]), range=mc(cfg, e, a, ENTERPRISES[e]["accounts"]))
                      for a in APPROACHES} for e in ents}
    scales = {e: {str(n): {a: det(cfg, e, a, n)["total_economic_admin_per_account"] for a in APPROACHES}
                  for n in ENTERPRISES[e]["scales"]} for e in ents}
    revenue = {e: {"model": ENTERPRISES[e]["revenue"], "blended_revenue_per_account": blended_revenue(cfg, e),
                   "revenue_after_30pct_market_decline": revenue_under_market(cfg, e, 0.7),
                   "margin_C_per_account": blended_revenue(cfg, e) - comparison[e]["C"]["enterprise_admin_per_account"],
                   "margin_C_after_decline": revenue_under_market(cfg, e, 0.7) - comparison[e]["C"]["enterprise_admin_per_account"]}
               for e in ents}
    segment = {e: M.segment_cost_to_serve(cfg, f"{e}_C", ENTERPRISES[e]["accounts"]) for e in ents}
    corrected = {e: {"C_at_licence": {w: det(cfg, e, "C", ENTERPRISES[e]["accounts"], licence_which=w)["total_economic_admin_per_account"]
                                      for w in ("favourable", "base", "adverse", "market_exceeding_reference")},
                     "C0_at_licence": {w: det(cfg, e, "C0", ENTERPRISES[e]["accounts"], licence_which=w)["total_economic_admin_per_account"]
                                       for w in ("favourable", "base", "adverse")}}
                 for e in ents}
    return {"meta": {"seed": SEED, "mc_runs": MC_RUNS, "theoretical": True,
                     "basis": "simplification model.py + decision_teams.py unchanged; assumptions in this file are [A]",
                     "enterprises": ENTERPRISES, "approaches": {k: {kk: vv for kk, vv in v.items() if kk != "overrides"} for k, v in APPROACHES.items()},
                     "licence": LICENCE, "complex_policy": COMPLEX_POLICY,
                     "licence_per_account": {w: licence_per_account(cfg, random.Random(0), w) for w in ("favourable", "base", "adverse", "market_exceeding_reference")}},
            "comparison": comparison, "scales": scales, "revenue": revenue, "licence_sensitivity": corrected,
            "vendor_pnl": vendor_table(cfg), "vendor_assumptions": VENDOR,
            "december_surge": {e: december_surge(cfg, e) for e in ents},
            "staffed_offer": staffed_table(cfg), "staffed_assumptions": {k: v for k, v in STAFFED.items()},
            "sensitivity": {e: sensitivity(cfg, e, ENTERPRISES[e]["accounts"]) for e in ents},
            "stress": {e: stress(cfg, e, ENTERPRISES[e]["accounts"]) for e in ents},
            "segment_cost_to_serve": segment}


def activity_csv(res):
    rows = tuple({"enterprise": e, "approach": a, "activity": act, "usd_per_account": round(v, 4)}
                 for e, by_a in res["comparison"].items() for a, s in by_a.items() for act, v in s["activities"].items())
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=("enterprise", "approach", "activity", "usd_per_account"), lineterminator="\n")
    w.writeheader(); w.writerows(rows)
    return buf.getvalue()


def sens_csv(res):
    rows = tuple(r for e in res["sensitivity"] for r in res["sensitivity"][e])
    buf = io.StringIO()
    w = csv.DictWriter(buf, fieldnames=tuple(rows[0].keys()), lineterminator="\n")
    w.writeheader(); w.writerows(rows)
    return buf.getvalue()


def rounded(x):
    if isinstance(x, float):
        return round(x, 4)
    if isinstance(x, dict):
        return {k: rounded(v) for k, v in x.items()}
    if isinstance(x, (list, tuple)):
        return [rounded(v) for v in x]
    return x


def main(argv):
    out = argv[1] if len(argv) > 1 else HERE
    res = rounded(results(M.load(os.path.join(SIMP, "parameters.json"))))
    os.makedirs(out, exist_ok=True)
    with open(os.path.join(out, "enterprise-results.json"), "w") as fh:
        json.dump(res, fh, indent=1, sort_keys=True)
    with open(os.path.join(out, "activity-costs.csv"), "w") as fh:
        fh.write(activity_csv(res))
    with open(os.path.join(out, "sensitivity.csv"), "w") as fh:
        fh.write(sens_csv(res))
    summary = {e: {a: (s["total_economic_admin_per_account"], s["range"]["favourable_p10"], s["range"]["adverse_p90"])
                   for a, s in by.items()} for e, by in res["comparison"].items()}
    with open(os.path.join(out, "run.log"), "w") as fh:
        fh.write(json.dumps(summary, indent=1, sort_keys=True) + "\n")
    print(json.dumps(summary, indent=1, sort_keys=True))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

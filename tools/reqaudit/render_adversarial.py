#!/usr/bin/env python3
"""Render report 08 (adversarial enterprise review) with a disposition for every finding (pure render).

Dispositions come from (1) amendments in clauses/c7_amendments.py that cite the finding, (2) clause titles citing it,
(3) the BUSINESS_MAP below for business findings, and (4) explicit owner decisions. Fails if any finding lacks one.
Usage: python3 -I -B tools/reqaudit/render_adversarial.py <repo-root>
"""
import json
import re
import sys
from collections import Counter
from pathlib import Path

MISSION = Path("requirements/convergence/2026-11-24-master")
BUSINESS_MAP = {
    "CNV-ECO-003": ("001", "002", "005", "006", "007", "008", "009", "013", "030", "033"),
    "CNV-ECO-004": ("003", "004", "010", "027", "029"),
    "CNV-ECO-005": ("012", "016", "019", "021", "022", "025", "032"),
    "CNV-GOV-008": ("017", "020", "028"),
    "CNV-GOV-009": ("026",),
    "CNV-EXP-002": ("011", "023", "024"),
    "CNV-EVB-002": ("018", "031"),
    "report 05 §5 revised (durability rated with evidence)": ("014",),
    "owner decision OD-M-04 (report 10): trigger for reopening staffed services (SPR-ADV-009)": ("015",),
}


ROUND2_OTHER = {
    "ADV-R2-030": "addressed with ADV-R2-075 (CNV-GOV-010 amended: one level set)",
    "ADV-R2-032": "tooling: baseline rows now carry full source text and hash (adopt_convergence.py)",
    "ADV-R2-054": "tooling: textual clause citations are now derived as declared dependencies and cycle-checked",
    "ADV-R2-021": "RESIDUAL: December surge cost not yet re-run in the model (report 10 residuals)",
    "ADV-R2-033": "RESIDUAL: clauses not yet tagged internal-control and mapped to the control register (report 10 residuals)",
    "ADV-R2-063": "RESIDUAL: numeric basis tags not yet applied to every clause (report 10 residuals)",
}


def round2(root, cell):
    import re as _re
    from clauses.c10_round2_amendments import AMENDMENTS as A10
    from clauses.c11_round2_partial_closures import AMENDMENTS as A11
    disp = {}
    for mod, am in (("c10", A10), ("c11", A11)):
        for cid, a in am.items():
            for f in _re.findall(r"ADV-R2-\d{3}", a["finding"]):
                disp.setdefault(f, []).append(f"amended {cid}")
    rows = [json.loads(l) for l in (root / MISSION / "data/adversarial-round2.jsonl").read_text().splitlines() if l.strip()]
    def d(r):
        if r["finding_id"] in disp:
            return "; ".join(dict.fromkeys(disp[r["finding_id"]]))
        if r["finding_id"] in ROUND2_OTHER:
            return ROUND2_OTHER[r["finding_id"]]
        return "closed in round 2 (no action)" if r["verdict"] == "closed" else "UNDISPOSITIONED"
    missing = [r["finding_id"] for r in rows if d(r) == "UNDISPOSITIONED"]
    if missing:
        raise SystemExit(f"round-2 items without disposition: {missing}")
    verdicts = Counter((r["kind"], r["severity"], r["verdict"]) for r in rows)
    table = "\n".join(f"| {r['finding_id']} | {r['kind']} | {r['refers_to']} | {r['severity']} | {r['verdict']} | {cell(r['claim'], 220)} | {d(r)} |" for r in rows)
    summary = "\n".join(f"| {k[0]} | {k[1]} | {k[2]} | {n} |" for k, n in sorted(verdicts.items()))
    return f"""
## Round 2 (verification of the amended baseline)

A second pass, run by a different model tier and given the amended clauses rather than the first-round conclusions,
checked whether each critical and a sample of major first-round findings was actually closed, and searched for defects
the amendments introduced. It reported no finding 'not closed' or 'worse'. Its 22 new defects and the partially closed
items were answered by the amendments in `clauses/c10_round2_amendments.py` and `clauses/c11_round2_partial_closures.py`
(original wording kept in each clause's revision history) or by tooling changes. Three items remain as named residuals.
The same caveat applies: an AI pass is not independent evidence.

| Kind | Severity | Verdict | Count |
|---|---|---|---|
{summary}

| Item | Kind | Refers to | Severity | Verdict | Claim | Disposition |
|---|---|---|---|---|---|---|
{table}
"""


def main(argv):
    root = Path(argv[1])
    sys.path.insert(0, str(root / MISSION))
    from clauses.c7_amendments import AMENDMENTS
    read = lambda n: [json.loads(l) for l in (root / MISSION / "data" / n).read_text().splitlines() if l.strip()]
    findings = read("adversarial-business.jsonl") + read("adversarial-technical.jsonl")
    clauses = [json.loads(l) for l in (root / MISSION / "data/convergence-clauses.jsonl").read_text().splitlines() if l.strip()]
    disp = {}
    for cid, a in AMENDMENTS.items():
        for f in re.findall(r"ADV-T-\d{3}", a["finding"]):
            disp.setdefault(f, []).append(f"amended {cid}")
    for c in clauses:
        for f in re.findall(r"ADV-T-\d{3}", c["title"]):
            disp.setdefault(f, []).append(f"new {c['id']}")
    for target, nums in BUSINESS_MAP.items():
        for n in nums:
            disp.setdefault(f"ADV-B-{n}", []).append(target if target.startswith(("report", "owner")) else f"new {target}")
    missing = [f["finding_id"] for f in findings if f["finding_id"] not in disp]
    if missing:
        print("findings without disposition:", missing, file=sys.stderr)
        return 2
    cell = lambda t, n: (" ".join(str(t).split()).replace("|", "\\|"))[:n]
    rows = "\n".join(f"| {f['finding_id']} | {f['severity']} | {cell(f['perspective'], 30)} | {cell(f['claim'], 260)} | {'; '.join(dict.fromkeys(disp[f['finding_id']]))} |"
                     for f in findings)
    sev = Counter(f["severity"] for f in findings)
    text = f"""# 08 — Adversarial enterprise review (Phase 6)

Generated by `tools/reqaudit/render_adversarial.py` from `data/adversarial-business.jsonl`, `data/adversarial-technical.jsonl`,
the amendments in `clauses/c7_amendments.py` and the convergence clauses; do not edit by hand.

## Method and its limits

Two independent passes were run by separate agents that were given the artifacts, not this mission's conclusions:
a **business pass** (competitor, CFO, regulator, sponsor board member, donor, financial advisor, charity, investor,
distribution partner) and an **architecture and controls pass** (auditor, security attacker, software architect,
customer service employee, operations employee), which also applied the Echelon Diagnostic Framework v0.3 nine-step
sequence as a structuring method (the framework's own documentation says its value is not yet established, so it was
used as structure, not evidence). Clarity is a private repository and was not available. **Both passes used the same
model family as the mission; their agreement or disagreement with the mission is not independent evidence.** Their
factual claims that changed conclusions were re-verified against primary data before acting (report 05 revision note).

## Results

{len(findings)} findings: {sev.get('critical', 0)} critical, {sev.get('major', 0)} major, {sev.get('minor', 0)} minor. Every finding
has a disposition below: an amendment to an existing clause (original wording preserved in clause revision history),
a new clause, a revised report, or a named owner decision. None was dismissed.

### What changed because of the review

- **Economics (business pass).** The licence scale exceeded the addressable market; the traditional comparator used an
  inflated review rate; community foundations were modelled at an unrepresentative size; E3 revenue relied on a structure
  the baseline forbids. Corrected model and conclusions in report 05: Unbound's vendor case is **not demonstrated**;
  community foundations are not the entry segment; workplace and administrator models are not offered at realistic
  licence scale; commercial commitments are gated on a shadow replay and time study (CNV-ECO-003, CNV-ECO-004).
- **Payment destination control (critical).** The convergence clause had weakened the independent callback and
  cooling-off controls; restored and strengthened (CNV-PAY-002 amended).
- **Regulatory perimeter vs protective holds (critical).** Platform holds are now sponsor-adopted, time-bounded and
  bypassable through the sponsor's own bank (CNV-PLT-007; CNV-LEG-001 amended).
- **Hold fencing with database-per-sponsor (critical).** Replicated, versioned hold publications with a signed freshness
  beacon (CNV-ARC-003 amended).
- **Tenancy, authority and AI.** Administrator principal and service delegations, shared-store register, one authority
  matrix, injection-resistant decision dossiers, server-side authority, engineering insider controls (CNV-TEN-001,
  CNV-GOV-010, CNV-AIO-002, CNV-PLT-010).
- **Baseline integrity.** Source-text hashes, adoption-derived statuses, approval basis per clause and an acyclic
  dependency check are now generated (CNV-GOV-011; `tools/reqaudit/adopt_convergence.py`); the check found and removed
  a real dependency cycle.

### Answers to 6.1 and 6.2 in brief

| Question | Answer after review |
|---|---|
| Why would customers switch? | Control and evidence by design, transparency, lower fees made possible by lower cost — only if the redesign levers are measured. |
| Why would they refuse? | High asset-fee margins remove cost pressure; incumbent platforms cover all fund types and offer staffed services; switching cost and activation gates. |
| Durable incumbent advantages | Installed base, staffed administration, full community-foundation fund coverage, distribution through wealth platforms. |
| Do economics survive adverse conditions? | Sponsor margins do; Unbound's vendor P&L does not below ~100k platform funds at ~$81/fund. |
| Concentration risk | High: about 20 US sponsors hold ≥5,000 accounts (CNV-ECO-004 concentration cap). |
| Unrealistic automation rates? | The case depends on unmeasured policy levers, not AI capability (EV-01/EV-02). |
| Mandatory human work underestimated? | For small sponsors, yes: fixed control time per sponsor now a model parameter (CNV-ECO-005). |
| Configuration complexity | Reduced but real; fail-safe on unadopted mandatory changes; static policy-conflict checks (CNV-EXP-003). |
| Regulatory exposure understated? | California platform registration is real; a 50-state survey is an evidence task (CNV-EVB-002). |
| Integration dependencies | Integration register with lead times and fallbacks (CNV-ECO-005); rail-partner resilience (CNV-PLT-012). |
| Modular monolith appropriate? | Yes, with workload isolation of batch and reporting from the payment path (CNV-ARC-001 amended). |
| Financial authority boundaries coherent? | Now, after CNV-PLT-007, CNV-GOV-010 and the amended perimeter. |
| Tenant isolation sufficient? | With CNV-TEN-001 and CNV-PLT-010; not yet tested. |
| Recovery objectives realistic? | Declared per data class; cross-region RPO nonzero by design (CNV-ARC-001 amended). |
| Audit evidence producible? | Designed (IPE support, control register linkage, service-organization evidence retention); unproven until built and examined. |
| Has complexity merely moved? | Partly: a database fleet replaces orchestration; fleet operations now specified (CNV-ARC-005). |

## Findings and dispositions

| Finding | Severity | Perspective | Claim | Disposition |
|---|---|---|---|---|
{rows}
"""
    text += round2(root, cell)
    (root / MISSION / "08-ADVERSARIAL-ENTERPRISE-REVIEW.md").write_text(text, encoding="utf-8")
    print(len(findings), "findings rendered")
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))

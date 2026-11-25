# Ledger: cross-industry discovery, 2026-11-23

**Branch:** `claude/daf-economics-event-architecture-v1zdti` in kemiller2002/unbound.

**Baseline.** Originals are pinned at `777a70d`, the revision every RR record quotes. The reconciliation at `940b067` is read-only.

**Original requirement files: additions only (adoption, 2026-11-24).** At the owner's direction, the 46 decision-translated records were adopted into the domain files (RCD-001 step):
- 38 records as dated `<!-- BEGIN ADOPTED: cross-industry-discovery -->` sections;
- 8 as inline "Refined … by RR-…" annotations under the originals they refine.

No original line was changed or removed (`git diff --numstat 777a70d` shows zero deletions in those files), and gate G4 still quotes every original at `777a70d`. Files touched: `requirements/FULL-ACCOUNTING-SYSTEM.md`, `requirements/DEFERRED-PAYMENTS-AND-REMAINING-GAPS.md`, `requirements/investments/ACCOUNT-STRUCTURES-AND-PORTFOLIO-MANAGEMENT.md`, `requirements/grants/END-TO-END-GRANT-LIFECYCLE-AND-OPERATING-MODEL.md`, `requirements/administration/DAF-ADMINISTRATION-AND-SPONSOR-OPERATIONS.md`, `requirements/MULTI-PARTICIPANT-AND-SUCCESSOR-ACCOUNTS.md`, `requirements/PARTNER-SEGREGATION-AND-TENANCY.md`, `requirements/DAF-LANDSCAPE-CONVERSION-WAVE-03.md` and `research/…/19-questions/PROPOSED-CANONICAL-REQUIREMENTS-AND-ACCEPTANCE-2026-11-22.md`.

**Files not modified:**
- `requirements/reconciliation/`;
- `daf-landscape` (gate D8 checks its working tree is clean);
- other Echelon repositories.

## 1. How to resume

1. Read the [README](README.md), then [10](10-EXECUTIVE-FINDINGS.md) §6–§7 (uncertainties and deferred work).
2. Records live in `data/`. Narrative reports render their tables from the data. Never hand-edit between `<!-- BEGIN GENERATED -->` markers.
3. To add a record, append to the matching data file using the schema of the existing rows. Then run:

```bash
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- graph requirements/discovery/2026-11-23-cross-industry/data
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- validate-discovery .
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- render-discovery .
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- validate .        # reconciliation gates must still pass
```

4. Do not reuse IDs. Do not change `priority_as_proposed`, which preserves the research pass's original priority.

## 2. Passes completed

| Pass | Kind | Scope | Output |
|---|---|---|---|
| 0 | Integrator | Repository discovery; baseline reuse; shared brief; validator | `tools/reqaudit/validate_discovery.py` |
| 1 | Research, role-separated | A Banking, I Payments, M custody | industry/bnk.md |
| 2 | Research, role-separated | B Investment and wealth, C Accounting, M fund administration | industry/inv.md |
| 3 | Research, role-separated | D Call centers, E Claims, F Healthcare administration, M legal/BPO/fraud | industry/svc.md |
| 4 | Research, role-separated | G ERP, H CRM, K Trust/estate, M subscription billing | industry/erp.md |
| 5 | Research, role-separated | J Grantmaking, §9 Regulatory, M tax/benefits/regtech | industry/grt.md |
| 6 | Research, role-separated | L SaaS, M IAM/records/workflow/logistics/other industries, §5 business models | industry/sas.md |
| 7 | Research, role-separated | §18 Operating economics | industry/eco.md |
| 8 | Research, role-separated | §19 Event-architecture hypothesis | industry/eva.md |
| 9 | Integrator | Merge; refinement renumbering (no collisions found); duplicate screen (token-similarity over all cross-pass pairs, then topical review); XIF-INT, XIC-INT, XID-INT | data/*, reports 00–09, 11–13, 15 |
| 10 | Adversarial, independent brief | §16: assume failure; priorities; contradictions; legal spot-checks; arithmetic | XIF-ADV, XIC-ADV, XID-ADV, XRJ-ADV, UNB-XIN-ADV, priority-challenges.jsonl |
| 11 | Integrator | Applied the adversarial revisions; wrote 10, 14 and this ledger | §4 below |
| 12 | Integrator, at the owner's direction (2026-11-24) | Decided the 41 legal and accounting questions after reading the accounting rules; verified §4966(c)(2), §4962, 47 CFR 64.1200, FINRA 2165 and Rev. Proc. 2017-53 | `data/decision-outcomes.jsonl`, report 16, gate D10 |
| 16 | Integrator, at the owner's direction (2026-11-24) | PostgreSQL 16 benchmark of ADR-EVA-001 (seven pgbench scenarios, fee batch, period-close race); criteria 1, 2 and 5 not falsified; chunking requirement added to UNB-XIN-ERP-001 (XIF-INT-010) | report 18, `benchmark/` |
| 15 | Integrator, at the owner's direction (2026-11-24) | First-pass release-1 scope: 182 records classified; proposed answers to eight scope decisions (status proposed) | report 17 |
| 14 | Integrator, at the owner's direction (2026-11-24) | Adopted the 46 decision-translated records into 9 domain files (additions only); records carry `adoption_status`, `adopted_into` and `adopted_on` | domain files listed in the header |
| 13 | Integrator, at the owner's direction (2026-11-24) | Translated the 41 decisions into requirement text: 45 records revised (`statement_as_proposed`, `decisions_applied`, `AC-D*` criteria); RR-REQ-X-069-r1 added; scenarios XDS-DEC-001..006 | `gap-requirements.jsonl`, `refinements.jsonl`, `scenarios.csv` |

## 3. Integrator fixups applied to research-pass output

The integrator applied these fixups with session scripts in the order fixups, merge, graph, validate. Those scripts and the raw per-pass output lived in the session scratch area and are **not** in the repository. The merged files in `data/` are the canonical record, and every change below can be seen in them, for example through `priority_as_proposed` and `adversarial_disposition`.

| Fixup | Reason |
|---|---|
| `daf_equivalent: "missing"` changed to `"no"` on 2 atlas rows | Enumeration (gate D1) |
| XIF-INV-004 `affected_ids` set to UNB-CUST-011 and UNB-INV-021 | The finding had no affected IDs. These two requirements mention corporate actions. |
| Necessity vocabulary extended with `fiduciary` and `contractual` | PRINCIPLES.md step 3 distinguishes them. Three passes used them. |
| G4 is CSV-aware (`validate_discovery.py`) | inventory.py renders a CSV-defined original as its cells joined with ` \| `. RR-TVR-CON-001-r1 quotes such a row. |
| Rev. Proc. 2017-53 citation changed from §4.02(1) to §3.03(1) in every record and in industry/grt.md | XIF-ADV-009. The integrator verified this against the IRS PDF text (§3.03(1) "Written advice that is current"). |
| XRA-020: Colorado SB 26-189 domain list gains financial services; confidence lowered to low | XIF-ADV-010. Based on secondary commentary; bill text not read. |
| 48 priority challenges applied; `priority_as_proposed` and `priority_revision` retained | XIF-ADV-003 |
| 7 `adversarial_disposition` markers added (reject or merge recommended) | XRJ-ADV-001..007. The records are kept in place until the owner decides. |
| Reports 11 and 12 corrected; integrator notes added to industry/eco.md and industry/eva.md | XIF-ADV-006, -007, -008 |
| Research-pass narrative reports saved by the integrator from the passes' final messages | Subagents could not write `.md` files. Content preserved; light formatting only. |

## 4. Validation (last run)

<!-- BEGIN GENERATED: validation -->
| Measure | Value |
|---|---|
| Errors |  |
| Warnings |  |
| Trace edges | 2499 |
| Records: refinements | 32 |
| Records: gaps | 150 |
| Records: findings | 218 |
| Records: conflicts | 38 |
| Records: scenarios | 211 |
| Records: decisions | 86 |
| Records: atlas | 368 |
| Records: rejected | 93 |
| Records: failures | 148 |
| Records: events | 96 |
| Records: regulatory | 20 |
<!-- END GENERATED: validation -->

The reconciliation validator (`validate.py`) still reports 0 errors and 30 warnings. These are the reconciliation's own pre-existing warnings and are unchanged.

## 5. Agent provenance and telemetry

| Pass | Sub-agent tokens (harness-reported) | Tool uses |
|---|---:|---:|
| 1 Banking and payments | 266,930 | 59 |
| 2 Investment and accounting | 291,247 | 62 |
| 3 Service, claims, healthcare | 299,097 | 65 |
| 4 ERP, CRM, trust and estate | 268,807 | 41 |
| 5 Grantmaking and regulatory | 305,786 | 72 |
| 6 SaaS, IAM, records, workflow, other | 367,593 | 64 |
| 7 Operating economics | 279,538 | 44 |
| 8 Event architecture | 340,870 | 63 |
| 10 Adversarial review | 269,445 | 55 |
| **Sub-agent total** | **2,689,313** | **525** |

The harness does not report the integrating session's own token use, and it is not estimated here. No human intervention occurred during the run. Every pass is the same model family. The adversarial pass was independent only in its brief: it received records, not conclusions. This is relevant to the EF capability ledger required by PRINCIPLES.md.

## 6. Limitations and blockers

- **Web evidence.**
  - Several sources were read only as search summaries and are marked as such: Nacha rule pages, Visa dispute FAQ, NIST 800-63B-4, MSRB G-45, Sedona, USPS IV-MTR, Colorado SB 26-189, and California AB 488 text.
  - eCFR redirected from this environment, so Cornell LII and govinfo were used instead.
  - FASB Codification sections were not read directly. ASU 2018-08 was read.
- **No measured operating data.** Every cost and volume figure without a source is Illustrative or `[A]`.
- **Nothing executed.** There is no implementation, the scenarios have not been run, and no PostgreSQL benchmark exists.
- **No review by counsel or a CPA.** The 41 legal (29) and accounting (12) decisions are `decided-provisional`: owner-directed defaults with recorded confirmations, not opinions. Since report 16, `data/` is maintained directly; the merge is not re-run.
- **Prior blockers unchanged.** The Conditor bootstrap (RCD-002) is still not done. Only the 46 decision-translated records have been adopted into the domain files. The other new requirements and refinements await adoption (RCD-001).
- **Not reviewed by the adversarial pass.** Five industry reports (bnk, inv, svc, erp, grt) were assessed only through their data files.

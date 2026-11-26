# File ingestion, information exchange and straight-through processing — requirements discovery (2026-11-26)

Owner sources: [unbound#18](https://github.com/kemiller2002/unbound/issues/18) (secure file ingestion, quarantine and
financial import), [echelon-organization-administration#20](https://github.com/kemiller2002/echelon-organization-administration/issues/20)
(reusable controlled file ingestion), with [unbound#15](https://github.com/kemiller2002/unbound/issues/15) (PostgreSQL-first
durable command queues), [#16](https://github.com/kemiller2002/unbound/issues/16) (network restrictions) and
[#17](https://github.com/kemiller2002/unbound/issues/17) (federated SSO) incorporated where they touch ingestion.

Source revision assessed: `e2f6385` (main, 2026-11-25). Status: **proposed**. Nothing here amends the authoritative
baseline, a convergence clause or the generated `AUTHORITATIVE-BASELINE.md`, and nothing is implemented.

Industry research (no Unbound requirements) is in daf-landscape `research/file-ingestion-and-straight-through-processing/`;
reusable Echelon requirements (EFI-*) are in echelon-organization-administration `application-governance/file-ingestion/`
(introduced by a pull request in that repository; the EFI references resolve once it merges).

## Key findings

1. **The baseline had strong source-specific rules but no general ingestion capability**: no upload channel, file
   limits, malware inspection, inbound document store, bulk-import template, dry-run report or transport standard.
   Of 52 ingestion topics (weakest class counted), 9 were covered, 26 insufficiently specific, 12 missing,
   1 conflicting and 4 deferred ([13](13-REQUIREMENTS-RECONCILIATION.md)).
2. **115 distinct ingestion workflows** across 12 domains ([01](01-WORKFLOW-COVERAGE-MATRIX.md)): 61 needed in release 1,
   21 conditional on the launch sponsor's partners, 16 later, 13 deferred by governing clauses, 4 eliminated. 15 are
   replaced by structured capture or authoritative sources (no upload path at all).
3. **One boundary, five artifact classes, no file privilege.** A file produces the same commands as the interface, or
   append-only observations, snapshots, evidence or configuration changes — never ledger writes ([03](03-INGESTION-BOUNDARY-AND-PROFILES.md), ADR-ING-001).
4. **The 21-state lifecycle becomes six small machines** (Expectation, Artifact, Run, Reconciliation, Retention, Record)
   with every state a decision or fact ([04](04-SECURE-INGESTION-LIFECYCLE.md)).
5. **Bytes in object storage, everything else in PostgreSQL** — justified by native malware scanning, write-once
   retention and restore objectives, not convention ([07](07-STORAGE-ARCHITECTURE-EVALUATION.md), ADR-ING-002, owner decision FIL-OD-02).
6. **Exception prevention beats exception processing**: contribution intents carry a unique reference to defeat the
   industry's pooled-receipt attribution problem (UNB-ING-DAF-001).
7. **AI assists onboarding and migration mapping; evidence extraction is optional and off by default; AI is absent from
   the structured financial path** ([06](06-AI-PROCESSING.md)).
8. **Independent adversarial review found 29 material problems in the first version** — including a hold release that
   skipped dual approval, replay that could double-post, auto-matching that could credit the wrong fund, an override that
   could release unscanned bytes and a parser holding cross-tenant credentials. All 82 findings are dispositioned and the
   design was revised ([16](16-ADVERSARIAL-REVIEW.md)).

## Contents

| File | Content |
|---|---|
| [01-WORKFLOW-COVERAGE-MATRIX.md](01-WORKFLOW-COVERAGE-MATRIX.md) | 115 workflows (generated from `data/workflow-matrix.json`) |
| [02-REQUIREMENTS.md](02-REQUIREMENTS.md) | 94 proposed UNB-ING requirements with Given/When/Then acceptance and revision history (generated) |
| [03-INGESTION-BOUNDARY-AND-PROFILES.md](03-INGESTION-BOUNDARY-AND-PROFILES.md) | Boundary, profile specification, delivery mechanisms, operation rows OPC-035..046, format decisions |
| [04-SECURE-INGESTION-LIFECYCLE.md](04-SECURE-INGESTION-LIFECYCLE.md) | Six machines, transitions, mapping of the 21 proposed states (generated) |
| [05-FINANCIAL-IMPORT-AND-RECONCILIATION.md](05-FINANCIAL-IMPORT-AND-RECONCILIATION.md) | Two-level acceptance, atomicity, identity, replay, reconciliation |
| [06-AI-PROCESSING.md](06-AI-PROCESSING.md) | Where AI adds value; controls; thresholds |
| [07-STORAGE-ARCHITECTURE-EVALUATION.md](07-STORAGE-ARCHITECTURE-EVALUATION.md) | PostgreSQL vs object vs file storage vs hybrid; consistency protocol |
| [08-SECURITY-THREAT-MODEL.md](08-SECURITY-THREAT-MODEL.md) | 23 threats → invariants → controls → tests |
| [09-ADMINISTRATIVE-UI-AND-DASHBOARDS.md](09-ADMINISTRATIVE-UI-AND-DASHBOARDS.md) | Surfaces, widgets, reports |
| [10-PARTNER-ONBOARDING.md](10-PARTNER-ONBOARDING.md) | 11-step pipeline; marginal cost |
| [11-SYNTHETIC-TEST-CORPUS.md](11-SYNTHETIC-TEST-CORPUS.md) | 109 synthetic cases with expected results (generated) |
| [12-ARCHITECTURE-DECISIONS.md](12-ARCHITECTURE-DECISIONS.md) | ADR-ING-001..007; framework ownership |
| [13-REQUIREMENTS-RECONCILIATION.md](13-REQUIREMENTS-RECONCILIATION.md) | 52 topics, conflicts, what was not changed (generated register) |
| [14-QUESTIONS-AND-OWNER-DECISIONS.md](14-QUESTIONS-AND-OWNER-DECISIONS.md) | FIL-OD-01..14 (generated) |
| [15-ROADMAP.md](15-ROADMAP.md) | Dependency-ordered steps |
| [16-ADVERSARIAL-REVIEW.md](16-ADVERSARIAL-REVIEW.md) | Three independent review passes, 82 findings with dispositions (generated), disagreements |
| `data/` | `workflow-matrix.json/.csv`, `gap-requirements.jsonl`, `lifecycle.json`, `operation-classification.jsonl`, `test-corpus.jsonl`, `decisions.csv`, `format-decisions.csv`, `reconciliation.csv`, `review-findings.csv`, `catalogs.json`, `ingestion-profile.schema.json`, `profiles/*.json` (9) |

## Validation

```bash
dotnet run --project tools/fs/Unbound.Ingestion -c Release -- validate . --check   # gates I1-I8 + generated sections
dotnet test tools/fs/Unbound.Ingestion.Tests -c Release
dotnet run --project tools/fs/Unbound.Convergence -c Release -- render-questions .  # question register (338 rows)
dotnet test tools/fs/Unbound.Convergence.Tests -c Release
dotnet run --project tools/fs/Unbound.ReqAudit -c Release -- validate .
```

Without `--check`, the first command rewrites the generated sections. The gates: I1 identifiers; I2 requirement fields,
Given/When/Then, priority, resolvable parents and citations; I3 every mission §7 matrix field; I4 lifecycle consistency
and reachability; I5 profile cross-field rules and closed-catalog references; I6 operation classification; I7 every P0 requirement covered by a synthetic case unless
its only verification is architectural review; I8 decisions, formats, reconciliation and review-finding references.

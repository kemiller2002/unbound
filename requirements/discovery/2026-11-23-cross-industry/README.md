# Cross-industry DAF requirements discovery and refinement — 2026-11-23

**Scope.** This mission compares the Unbound DAF platform requirements with thirteen overlapping industries, and with several more found during the work. The question is what the platform does not yet know it needs. The mission also covers two addenda:
- §18, enterprise operating economics and cost-to-serve;
- §19, testing the event-driven enterprise architecture hypothesis.

This is requirements engineering only. No application code was written. All data is synthetic.

**Where this sits.** `kemiller2002/unbound` is the canonical inbound requirements repository. The product and enterprise are named **Unbound** (`UNB-EXP-002`). The mission builds on the baseline revision `777a70d` and on the reconciliation in [`../../reconciliation/2026-11-23/`](../../reconciliation/2026-11-23/README.md). Neither is modified.
- Every refinement quotes its original verbatim at `777a70d` and links to it by ID, path and line.
- New requirements carry discovery provenance: industry, source and finding.
- `daf-landscape` was read only, and the validator checks it has no working-tree changes (gate D8).
- Adopting these records into the domain files is an owner decision ([RCD-001](../../reconciliation/2026-11-23/14-OPEN-DECISIONS.md)).

## Start here

1. [10-EXECUTIVE-FINDINGS.md](10-EXECUTIVE-FINDINGS.md): the most consequential discoveries, contradictions and uncertainties.
2. [14-ADVERSARIAL-REVIEW.md](14-ADVERSARIAL-REVIEW.md): the independent "assume it fails" challenge, and the revisions it forced.
3. [11-OPERATING-ECONOMICS.md](11-OPERATING-ECONOMICS.md) and [12-EVENT-ARCHITECTURE-DECISION.md](12-EVENT-ARCHITECTURE-DECISION.md): the two addenda.

## Deliverables

| # | Mission deliverable | File | Machine-readable data (`data/`) |
|---|---|---|---|
| §2 | Baseline and traceability matrix | [00-BASELINE-AND-TRACEABILITY.md](00-BASELINE-AND-TRACEABILITY.md) | `capability-atlas.jsonl` (`existing_ids`, `coverage`), `trace-edges.csv` |
| 1 | Cross-Industry Capability Atlas | [01-CAPABILITY-ATLAS.md](01-CAPABILITY-ATLAS.md) | `capability-atlas.jsonl` |
| 2 | Coverage and Gap Matrix | [02-COVERAGE-AND-GAP-MATRIX.md](02-COVERAGE-AND-GAP-MATRIX.md) | `capability-atlas.jsonl`, `findings.csv` |
| 3 | Missing Requirements Register | [03-MISSING-REQUIREMENTS-REGISTER.md](03-MISSING-REQUIREMENTS-REGISTER.md) | `gap-requirements.jsonl` |
| 4 | Requirements Refinement Records | [04-REFINEMENT-RECORDS.md](04-REFINEMENT-RECORDS.md) | `refinements.jsonl` |
| 5 | Cross-Industry Workflow Redesigns | [05-WORKFLOW-REDESIGNS.md](05-WORKFLOW-REDESIGNS.md) | `industry/*.md` |
| 6 | Shared Enterprise Capability Architecture | [06-SHARED-ENTERPRISE-CAPABILITY-ARCHITECTURE.md](06-SHARED-ENTERPRISE-CAPABILITY-ARCHITECTURE.md) | `findings.csv` (`XIF-INT-*`), `conflicts.csv` |
| 7 | Regulatory Applicability Matrix | [07-REGULATORY-APPLICABILITY-MATRIX.md](07-REGULATORY-APPLICABILITY-MATRIX.md) | `regulatory-applicability.jsonl` |
| 8 | Failure and Exception Catalog | [08-FAILURE-AND-EXCEPTION-CATALOG.md](08-FAILURE-AND-EXCEPTION-CATALOG.md) | `failure-catalog.jsonl` |
| 9 | Rejected Recommendations Register | [09-REJECTED-RECOMMENDATIONS.md](09-REJECTED-RECOMMENDATIONS.md) | `rejected.jsonl` |
| 10 | Executive Findings Report | [10-EXECUTIVE-FINDINGS.md](10-EXECUTIVE-FINDINGS.md) | — |
| §18 | Operating economics requirements, cost attribution and profitability model, efficiency dashboard requirements | [11-OPERATING-ECONOMICS.md](11-OPERATING-ECONOMICS.md) | `gap-requirements.jsonl` (`UNB-XIN-ECO-*`) |
| §19 | Architectural decision record, event ownership and authority matrix, consistency and failure-recovery requirements, alternative architecture comparison | [12-EVENT-ARCHITECTURE-DECISION.md](12-EVENT-ARCHITECTURE-DECISION.md) | `event-matrix.jsonl`, `UNB-XIN-EVA-*` |
| §5 | Business-model analysis | [13-BUSINESS-MODEL-ANALYSIS.md](13-BUSINESS-MODEL-ANALYSIS.md) | `business-models.jsonl` |
| §16 | Final adversarial review | [14-ADVERSARIAL-REVIEW.md](14-ADVERSARIAL-REVIEW.md) | `XIF-ADV-*`, `priority-challenges.jsonl` |
| — | Open decisions and acceptance scenarios | [15-OPEN-DECISIONS-AND-SCENARIOS.md](15-OPEN-DECISIONS-AND-SCENARIOS.md) | `decisions.csv`, `scenarios.csv` |
| — | Decisions on the 41 legal and accounting questions (provisional; confirmations listed) | [16-LEGAL-AND-ACCOUNTING-DECISIONS.md](16-LEGAL-AND-ACCOUNTING-DECISIONS.md) | `decision-outcomes.jsonl` |
| — | Independent industry assessments (eight research passes) | [`industry/`](industry/) | — |
| — | Checkpoint ledger (resume here) | [LEDGER.md](LEDGER.md) | `validation-report.json` |

## Identifier schemes

None of these schemes overlaps a baseline family or a reconciliation family. Gate G2 checks this.

| Scheme | Meaning |
|---|---|
| `UNB-XIN-<DOM>-<nnn>` | New requirement (`origin: gap-analysis`, plus `discovery_source`) |
| `RR-<original-id>-r<n>` | Refinement of an original requirement. `n` continues after the reconciliation's `r1` where one exists. |
| `XCAP-<DOM>-<nnn>` | Capability atlas row |
| `XIF-<DOM>-<nnn>` | Finding: gap, defect, duplicate or over-complication |
| `XIC-<DOM>-<nnn>` | Conflict |
| `XID-<DOM>-<nnn>` | Open decision that needs human, legal or accounting authority |
| `XDS-<DOM>-<nnn>` | Acceptance or adversarial scenario |
| `XRJ-<DOM>-<nnn>` | Rejected recommendation |
| `XFC-<DOM>-<nnn>` | Failure and exception catalog row |
| `XEV-<nnn>` | Event authority matrix row |
| `XRA-<nnn>` | Regulatory applicability row (supplements `OB-*`) |

Domain codes:
- **Banking and payments:** `BNK` banking, `PAY` payments.
- **Investments and finance:** `INV` investment, `ACC` accounting, `ERP` enterprise resource planning.
- **Customer and relationships:** `SVC` customer service, `CLM` claims/healthcare/legal case patterns, `CRM` relationship management, `TRS` trust/estate.
- **Grantmaking and regulation:** `NPG` nonprofit grantmaking, `REG` regulatory, `GOV` government/regtech.
- **Platform:** `SAS` SaaS platform, `IAM` identity and access, `REC` records management, `WFA` workflow automation, `OTH` other industries.
- **Addenda:** `ECO` operating economics, `EVA` event architecture.
- **Review and integration:** `ADV` adversarial review, `INT` integrator reconciliation.

Every record field from mission §11 is present in `gap-requirements.jsonl`. Gate D2 enforces this. The fields are:
- requirement ID, parent requirement and parent capability;
- discovery source, industry analogy, business problem;
- statement, applicability, priority and its justification;
- Given/When/Then acceptance criteria, failure conditions, dependencies;
- security and compliance, AI and automation;
- UI, API, reporting and testing implications;
- the for/against challenge, the simplest alternative, decision rationale, confidence and open questions.

## Quality gates

```bash
python3 -I -B tools/reqaudit/graph.py requirements/discovery/2026-11-23-cross-industry/data   # regenerate trace edges
python3 -I -B tools/reqaudit/validate_discovery.py .                                       # G1-G9 + D1-D9; non-zero exit on error
python3 -I -B tools/reqaudit/render_discovery.py .                                         # refresh generated tables
python3 -I -B tools/reqaudit/validate.py .                                                 # reconciliation gates still pass
```

`validate_discovery.py` reuses the reconciliation gates and adds the following.

**Reused reconciliation gates:**
- G1: no duplicate IDs.
- G2: no ID collisions with baseline or reconciliation families.
- G3/G4: originals exist and are quoted verbatim. For originals defined in CSV, the quote is checked against the row's cells joined with ` | `.
- G5: required fields are present.
- G6: acceptance criteria use Given/When/Then.
- G7: gap-analysis provenance is present.
- G8/G9: references and edge endpoints resolve.

**New discovery gates:**
- D1: atlas enumerations are valid.
- D2: every mission §11 field is present.
- D3: priorities are P0–P4.
- D4: event-matrix rules hold. An AI observation is never authoritative or synchronous, and every event has an owning module.
- D5/D6: rejection and failure records are complete.
- D7: every new requirement and refinement is tested by a scenario. This is an ERROR, not a warning.
- D8: `daf-landscape` is unchanged.
- D9: every identifier cited in the reports resolves.
- D10: every decision outcome references an existing decision, carries its rationale, authority and required confirmation, and agrees with the decision's status.

## Evidence discipline

- **Web sources.** Every web source records the URL and the access date (2026-11-23). Sources read only as search summaries are marked as such and given lower confidence.
- **Illustrative figures.** Every cost and volume figure without a source is tagged *Illustrative* or `[A]`.
- **Legal statements.** Each carries an authority type: statute, regulation, guidance, proposed, practice or proposed control. Anything uncertain is an open `XID-*` decision for counsel, not a requirement.
- **Review independence.** The work was done in eight role-separated research passes, one integration pass and one independent adversarial pass. All passes used the same model family, so this is **not** independent human review. See [LEDGER.md](LEDGER.md).

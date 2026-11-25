# Availability, durable execution and messaging requirements discovery (2026-11-25)

Source issue: [unbound#12](https://github.com/kemiller2002/unbound/issues/12) — *requirements discovery only; reconcile
with existing canonical requirements and architecture before implementation.*
Portfolio initiative: INIT-012 in echelon-organization-administration ([index](https://github.com/kemiller2002/echelon-organization-administration/blob/claude/echelon-availability-architecture-ezhitq/application-governance/availability-architecture/00-INDEX.md); issues
[#12](https://github.com/kemiller2002/echelon-organization-administration/issues/12) and
[#14](https://github.com/kemiller2002/echelon-organization-administration/issues/14)).

Source revision assessed: `9b38de47437d2be1e86ded3db856aaf0eefc9fed` (main, 2026-11-24). Status: **proposed**. Nothing here
amends the authoritative baseline, a convergence clause, or the generated `AUTHORITATIVE-BASELINE.md`. No implementation.

## Key reconciliation finding

Most of unbound#12's availability asks are **already decided** by owner-ratified convergence clauses:

| unbound#12 ask | Already governed by |
|---|---|
| Managed durable storage, outbox, no broker unless evidence | CNV-ARC-001, ADR-EVA-001 (benchmark 2,100–3,900 tx/s vs ~200 needed) |
| Expand/contract migrations, N/N-1, rollback per tenant | CNV-ARC-005 |
| Revalidate at execution | CNV-PLT-010, CNV-ARC-003 (fencing) |
| Unknown outcomes, no automatic payment retry | CNV-PAY-004, RR-REQ-X-004-r1, CNV-PLT-012 |
| RPO/RTO by data class | RR-UNB-RES-003-r2 (proposed), CNV-ARC-001 |
| SLIs, error budgets | UNB-XIN-SAS-008 |
| Status labels | CNV-EXP-003 StatusProjection |

What was **missing**: a per-operation execution-model classification; receipt semantics at deadlines; queue
quotas/fairness/poison isolation as normative requirements; operational pause/drain controls; honest database-outage
behaviour; consistency classes and cache readiness; and **all platform messaging and mandatory-acknowledgment
requirements** (no predecessor existed in the baseline).

## Contents

| File | Content |
|---|---|
| [01-OPERATION-CLASSIFICATION-MATRIX.md](01-OPERATION-CLASSIFICATION-MATRIX.md) | 34 operations classified (generated from `data/operation-classification.jsonl`) |
| [02-REQUIREMENTS.md](02-REQUIREMENTS.md) | 27 proposed UNB-AVL requirements with parents, sources and Given/When/Then acceptance (generated) |
| [03-DEADLINES-AND-FAILURE-SCENARIOS.md](03-DEADLINES-AND-FAILURE-SCENARIOS.md) | Year-end and deadline analysis; realistic financial failure scenarios |
| [04-DECISIONS-AND-OPEN-QUESTIONS.md](04-DECISIONS-AND-OPEN-QUESTIONS.md) | Unbound ADR-AVL-001, threat-model deltas, open questions |

## Validation

```bash
python3 -I -B tools/reqaudit/validate_availability.py . --check   # gates V1-V6; exits 1 on any error
python3 -I -B tools/reqaudit/validate.py . && python3 -I -B tools/reqaudit/validate_discovery.py . && python3 -I -B tools/reqaudit/validate_simplification.py .
```

Market research (deadline behaviour, December volumes) stays in daf-landscape and is cited, not copied.

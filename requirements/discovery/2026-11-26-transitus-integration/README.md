# Transitus integration requirements discovery (2026-11-26)

Source: the Transitus research, architecture and requirements initiative ([kemiller2002/transitus](https://github.com/kemiller2002/transitus), branch `claude/transitus-research-architecture-42a96z`). Transitus is a proposed embeddable, domain-independent F# data-integration component. Unbound is its first reference consumer.

Source revision assessed: `e2f6385f3c8901b9a8d3362c48429e64525f3edc` (main, 2026-11-26). Status: **proposed**. Nothing here amends the authoritative baseline, a convergence clause, the owner-decision register or the generated `AUTHORITATIVE-BASELINE.md`. There is no implementation.

## What this adds

| File | Content |
|---|---|
| [02-REQUIREMENTS.md](02-REQUIREMENTS.md) | UNB-TRN-001..011: where Unbound uses Transitus for file ingestion, raw evidence, reconciliation gates, loading, quotas, bulk import, exceptions and migration reconciliation. Generated from `data/gap-requirements.jsonl` |
| [04-DECISIONS-AND-OPEN-QUESTIONS.md](04-DECISIONS-AND-OPEN-QUESTIONS.md) | Boundary decisions, owner questions and proposed changes to Echelon.Execution |

## Boundary

Unbound uses Transitus for applicable file ingestion, data migration, charity data ingestion, recurring integrations, bulk processing, transformation, validation, reconciliation and onboarding.

**What stays in Unbound:**
- financial and business rules: tolerances, matching policy, posting and reversal consequences, eligibility, variance power;
- the domain operations that ingestion loads through;
- the MigrationRun state machine (RR-REQ-X-061-r1), including fencing and cutover;
- the charity knowledge model from ObservedClaim onwards (UNB-KNOW-*).

**What Transitus supplies:**
- parsing with resource limits;
- deterministic idempotency keys;
- raw-evidence records aligned to UNB-KNOW-023;
- generic validation and reconciliation primitives;
- lineage and evidence packages.

## Key reconciliation findings

1. **Parser ownership.** `requirements/convergence/2026-11-24-master/replay/src/Unbound.Replay/Sources.fs` already parses Pub 78, the revocation list, EO BMF, 990 XML and OFAC SDN. Transitus phase 1 proposes *moving* these parsers into `Transitus.Formats` rather than duplicating them (TRN-REQ-ARC-006). The replay tool would then consume them.
2. **Migration scope conflict.** The adopted clause CNV-EXP-002 (`initial-implementation`) requires full fund-history import from the predecessor platform. The simplification discovery (`2026-11-24-simplification-economics/01-REVERSE-REQUIREMENTS-ANALYSIS.md:571`) marks REQ-X-061 "Out (carrier deferred: no migration in R1)". The owner must decide which governs. UNB-TRN-006 and UNB-TRN-011 are conditional on that decision.
3. **Missing domain operations.** No domain API exists yet to load into. UNB-TRN-005 asks Unbound to define the ingestion operations.
4. **Bulk-load fairness.** The UNB-AVL-006 per-sponsor quota would let a bulk import starve live operations. UNB-TRN-007 requires a quota share and a separate worker role.

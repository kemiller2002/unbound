# Machine-readable workflow traceability consistency checkpoint
Date: 2026-11-23. **Actual static cross-artifact checks performed on committed GitHub files. No workflow software or financial simulation executed.**

## Inputs and method
Parsed `TRANSACTION-STATE-TRANSITION-MATRIX-2026-11-23.csv`, `END-TO-END-DAF-ENTERPRISE-WORKFLOW-COMPLETENESS-AUDIT-2026-11-23.md`, `CROSS-WORKFLOW-FINANCIAL-AUTHORITY-AND-IRREVERSIBILITY-INVARIANTS-2026-11-23.md`, and `ADVERSARIAL-TRANSACTION-SIMULATIONS-AND-RECOVERY-CONTRACTS-2026-11-23.md`. Extracted exact IDs and expanded shorthand references such as `TX-03/05` to `TX-03` and `TX-05`. Exported `WORKFLOW-TRANSITION-TRACEABILITY-CATALOG-2026-11-23.json` as a machine-readable snapshot.

## Findings from executed static checks
| Check | Result | Interpretation |
|---|---|---|
| Workflow identifiers | 18 | Present in workflow audit |
| Invariant identifiers | 18 | Present in invariant register |
| Transaction transition identifiers | 28 | Present in CSV |
| Adversarial simulation identifiers | 28 | Present in simulation register |
| Invalid simulation-to-transition references | 0 | All referenced TX IDs resolve |
| Transitions without explicit simulation references | **2: TX-01, TX-02** | Direct scenario traceability gap; does not prove these transitions are untested by future implementation |
| Workflow-to-transition machine links | Missing | Cannot verify full end-to-end edge coverage |
| Invariant-to-transition machine links | Missing | Cannot prove each invariant is enforced at its relevant boundary |
| Exact ledger journal expectations | Missing | Prose financial effects insufficient for numerical reconciliation |
| Executable guards and authority expressions | Missing | No deterministic permission/policy consistency verification |
| Executed financial simulations | 0 | No runtime/ledger test evidence |

## Architectural contradictions to investigate, not yet proven defects
1. TX-03 records receipt, TX-04 acceptance, TX-05 capacity; need precise ledger debit/credit, settlement reversal and negative capacity rules.
2. TX-07 legal acceptance may occur before custody; gift receipt, tax evidence and financial recognition cannot be assumed identical.
3. TX-11 sponsor approval and fund reservation are local-atomic, but charity holds and authority revocation may arrive between approval and TX-12 dispatch; specify recheck and reservation release.
4. TX-13 'ProviderUnknownOrAccepted' combines distinct states; split Unknown, Acknowledged and Confirmed, with provider-specific settlement proofs.
5. TX-15 'ReturnedOrReversed' must distinguish return after settlement from pre-settlement void and partial return.
6. TX-21 'ClosedAndIssued' may conflate financial close, artifact generation and successful delivery; require distinct evidence.
7. TX-22 'SubmittedThenAccepted' combines an externally uncertain submission with government acceptance; separate states and durable evidence.
8. TX-23 migration cutover needs a defined fencing epoch and post-first-live-posting no-rollback boundary.
9. TX-28 sponsor activation must not imply every future donor fund is automatically authorized.

## Proposed corrections and required schema additions
- Every transition: `workflow_ids`, `invariant_ids`, `preconditions`, `authority_contract`, `policy_version_rule`, `input_schema`, `ledger_journal`, `outbox_events`, `external_idempotency`, `compensation_transition`, `proof_of_completion`, `ux_status`, `test_fixture_ids`.
- Every workflow: `entry_transition`, `terminal_states`, `allowed_transition_ids`, `exception_edges`, `irreversibility_points`, `legal_signoff_required`.
- Every simulation: explicit `initial_ledger`, `commands`, `injected_fault`, `expected_ledger`, `expected_statuses`, `expected_audit`, `invariant_assertions`, `review_status`.
- CI eventually validates IDs, referential integrity, required fields, graph reachability, no orphan transition, no unreachable terminal, policy/authorization coverage and invariant/fixture mapping. **Do not claim these CI checks exist yet.**

## Prioritization
**P0:** Split external Unknown/Accepted/Settled states; define legal acceptance vs capacity; grant reservation and release; exact ledger entries and reversals; migration writer fencing.
**P1:** Explicit cross-artifact links, proof of filing/receipt, role revocation, bank-change effective time and closed-period correction.
**P2:** Generate deterministic executable fixture specifications from validated typed contracts once implementation is authorized.

## Decision
Retain one Unbound component monolith with distinct authoritative aggregate owners. A single deployment does not make bank/custodian/government operations atomic. The traceability catalog is a *first static baseline*, not a proof of requirements consistency or production readiness.

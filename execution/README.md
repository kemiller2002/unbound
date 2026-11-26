# Echelon.Execution (incubating in Unbound)

Durable command execution for Echelon transactional applications: INIT-012 roadmap item RM-07,
incubated here under ADR-EAR-004 / UNB-AVL-019. The module references nothing in Unbound's domain, so it
can be extracted to a shared repository when a second transactional application needs it.

**Status: first implementation slice.** It is tested against PostgreSQL 16 locally. It has not been benchmarked
on a managed instance (RM-05) and is not deployed anywhere.

## What an application writes

One `Worker.Operation` value per durable operation. It has four parts:
- an upcaster for older contract versions;
- an execution-time `Authorize` re-check;
- an `Execute` function that does business writes through the transaction handle and returns a `HandlerResult`;
- the operation's name and current version.

Intake, idempotency, ordering, leases, retries, quarantine, unknown outcomes, lifecycle history, status and
the outbox all come from the module.

```fsharp
let recommendGrant : Worker.Operation<NpgsqlTransaction> =
    { Name = OperationName "grant.recommend"; CurrentVersion = 1
      Upcast = fun _ payload -> Ok payload
      Authorize = fun ctx -> Authority.recheck ctx.Tx ctx.Command        // CNV-PLT-010
      Execute = fun ctx -> Grants.decide ctx.Tx ctx.Payload }            // returns Done / Transient / Ambiguous ...
```

## Layout

| Path | Contents |
|---|---|
| `src/Echelon.Execution` | Pure core: `Types`, `Lifecycle` (exhaustive transition function), `Policy` (fingerprint, backoff, fair per-key claim selection, expiry), `Store` (the effect boundary as a record of functions), `Intake`, `Worker`, `InMemoryStore` |
| `src/Echelon.Execution.Postgres` | PostgreSQL adapter. It uses `FOR UPDATE SKIP LOCKED` claiming, transactional completion with the outbox, and fixed parameterized SQL (SEC-DB-001). `Schema.sql` is the DDL source, to be governed by Strata in deployments. |
| `tests/Echelon.Execution.Tests` | 25 unit and in-memory tests |
| `tests/Echelon.Execution.Postgres.Tests` | 10 integration tests against a real PostgreSQL |

## Running the tests

```bash
export ECHELON_EXECUTION_TEST_PG="$(scripts/test-postgres.sh start)"   # disposable local PostgreSQL 16
dotnet test Echelon.Execution.slnx -c Release
```

Without `ECHELON_EXECUTION_TEST_PG` the integration tests fail with an explanation. They are never skipped.

## Requirement coverage (INIT-012 EAR-*)

| Requirement | State | Evidence |
|---|---|---|
| EAR-API-002 static classes; EAR-API-003 controls never queued | Implemented (intake refuses non-durable classes) | `intake: immediate classes never reach the command store` |
| EAR-API-005 / EAR-CMD-004 idempotency with fingerprint; 50 concurrent submissions → 1 command | Implemented | in-memory and PostgreSQL concurrency tests |
| EAR-API-007 / EAR-AVL-010 receipt only after commit; honest refusal when the database is down | Implemented | outage tests (simulated, and an unreachable PostgreSQL) |
| EAR-API-008 receipts never mean completion | Implemented by type (`ReceiptMeaning` has no completed case) | receipt tests |
| EAR-API-009 / EAR-CMD-020 per-tenant queue limit; fair claiming across tenants | Implemented | throttle and fairness tests |
| EAR-CMD-001 / -006 atomic completion, business writes, history and outbox | Implemented | commit and rollback tests (PostgreSQL) |
| EAR-CMD-005 versioning and upcasting; unsupported versions are quarantined | Implemented | upcast test |
| EAR-CMD-008 stable effect keys; at-least-once relay | Implemented | relay tests |
| EAR-CMD-009 per-key ordering, cross-key parallelism | Implemented | 4-worker PostgreSQL test. A mutation that removes the ordering rule makes it fail. |
| EAR-CMD-012 / -013 retry classification and backoff; crash → quarantine of that key only | Implemented | retry and quarantine tests |
| EAR-CMD-014 / -019 OutcomeUnknown is never retried; replay is refused; reconciliation transitions | Implemented | unknown-outcome tests |
| EAR-CMD-016 expiry of unstarted work past its deadline | Implemented | expiry tests |
| EAR-CMD-017 execution-time authorization hook | Implemented (the application supplies the check) | revocation test |
| EAR-CMD-018 lease expiry, reclaim, and fencing of a stale owner | Implemented | lease tests |
| EAR-CMD-021 append-only lifecycle history | Implemented (append-only by code path) | history tests |
| EAR-AVL-004 durable pause and resume per tenant, per operation, or everything | Implemented (drain = pause plus lease expiry) | pause tests |
| EAR-CMD-003 tenant isolation | **Partial.** Tenant is on every row and every read is tenant-scoped. Schema-per-sponsor and row-level security are application schema concerns (CNV-ARC-001) and are not in this module. | history tenant test |
| EAR-SEC-002 distinct database roles for intake and worker; no UPDATE/DELETE on history | **Not implemented** (deployment grants) | — |
| EAR-CMD-010 dependency resolution (resuming a waiting command when its dependency completes) | **Partial.** The states and transitions exist; there is no automatic resume yet. | — |
| EAR-CMD-022 payload protection and retention purge | **Not implemented** | — |
| Lease renewal for long executions | **Not implemented** (choose the lease duration above the maximum handler time) | — |
| EAR-OBS-002 metrics | **Not implemented** (blocked on decision U-07) | — |
| HTTP intake layer, status endpoint, user-facing labels | **Not implemented** (Unbound API layer; the labels are CNV-EXP-003) | — |

## Known limitations

- **Idempotent retry at the queue limit:** an idempotent retry from a tenant that is exactly at its queue limit is throttled instead of
  receiving the original receipt, because the quota check runs before the duplicate lookup. Retrying after
  the backlog drains returns the original receipt.
- **Benchmark:** throughput figures come from Unbound ADR-EVA-001's benchmark of the outbox pattern, not of this code.
  RM-05 must benchmark this adapter on a managed instance before production use.

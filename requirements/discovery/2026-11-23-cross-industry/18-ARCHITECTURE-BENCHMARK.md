# 18 — Benchmark of the event-architecture decision (ADR-EVA-001)

**Date:** 2026-11-24. **Status:** measured, in a single-VM test environment, not production.

**Purpose.** This benchmark runs the first real test of ADR-EVA-001's falsification criteria (report 12). The ADR said database capacity was "unbenchmarked general knowledge". It now has a measurement, with the caveats in §5.

Everything needed to reproduce it is in [`benchmark/`](benchmark/):
- schema;
- seed;
- pgbench transaction scripts;
- runners;
- analysis;
- saved results.

## 1. What was measured

| Item | Setup |
|---|---|
| Database | PostgreSQL 16.15. `synchronous_commit=on`, `fsync=on`, `shared_buffers=2GB` |
| Host | 4 vCPU, 15 GiB RAM. The pgbench client ran on the same host and competed for the same CPUs. `pg_test_fsync` measured fdatasync at about 8,100 ops/s, which is fast local storage. |
| Data (one sponsor database) | 100,000 funds; 50,000 charities in the hold registry; 2,000,000 historical grants; 4,000,000 journal lines (1.1 GB) |
| Grant approval transaction | Hold-registry read, then conditional fund-row update (capacity check plus reservation, under a row lock), grant insert, reservation insert, journal batch insert, two journal lines, two outbox rows. The period-lock trigger takes a FOR SHARE lock on the open period. |
| Payment release transaction | Hold row read FOR SHARE (version check), payment insert, journal batch insert, two lines, one outbox row |
| Hold publication | Version bump on a charity hold row, which conflicts with releases |
| Outbox dispatcher | Two clients using claim-and-mark with `FOR UPDATE SKIP LOCKED`, 200 rows per claim |
| Mix | 70% approvals, 25% releases, 5% hold publications, plus the dispatcher running concurrently |
| Scenarios | **Spread:** random over all funds and charities. **Hot rows:** all traffic on 10 funds and 20 charities, the worst case for criterion 5. **RTT:** 1 ms of simulated app-to-database round trip after every statement inside the transaction, so locks are held about 7 ms longer. This approximates a managed database across a network. |

The target load comes from report 12. At Fidelity-scale volume, the 30× burst needs about **200 authoritative transactions per second**, and the year-end spike about 85.

## 2. Results

| Scenario | Total tx/s | Approval p50 / p99 / max (ms) | Release p99 (ms) | Failed | Outbox lag p99 (ms) |
|---|---:|---|---:|---:|---:|
| S1 capacity, spread | **3,944** | 8.2 / 18.5 / 230 | 16.5 | 0 | 9 |
| S2 200 tx/s, spread | 201 | 2.1 / 21.2 / 160 | 15.1 | 0 | 20 |
| S3 capacity, hot rows | **2,621** | 10.8 / 76.4 / 680 | 11.3 | 0 | 8 |
| S4 200 tx/s, hot rows | 202 | 2.2 / 7.7 / 74 | 7.2 | 0 | 4 |
| S5 200 tx/s, spread + RTT | 202 | 18.0 / 23.6 / 39 | 17.2 | 0 | 5 |
| S6 200 tx/s, hot rows + RTT | 202 | 18.3 / 41.2 / 89 | 18.0 | 0 | 5 |
| S7 capacity, spread + RTT | **2,127** | 32.6 / 52.6 / 139 | 42.0 | 0 | 15 |

The full per-script table is in `benchmark/results/scenarios-summary.csv`. Every outbox row was delivered (0 undelivered after drain).

### Fee batch (daily fee computation, UNB-XIN-ERP-001)

| Test | Funds | Mode | Batch time | Approval latency during batch |
|---|---:|---|---:|---|
| B1 | 100,000 | One transaction | 0.90 s | **p99 244 ms, max 347 ms**: approvals wait on fund-row locks |
| B2 | 100,000 | 1,000 funds per transaction | 1.50 s | p99 5.6 ms, max 10 ms |
| B3 | 1,420,000 | One transaction, no load | 11.8 s | By extrapolation, approvals would stall for about 12 s |
| B4 | 1,420,000 | 5,000 funds per transaction, no load | 16.2 s | Locks held only per chunk |

### Period close racing live postings (UNB-XIN-EVA-002)

Postings ran at 200/s, and the period was hard-closed after 10 s.

- Postings recorded in the period at the moment of close: **605,991**. The same count 10 s later: **605,991**. Nothing was posted into the closed period.
- All 32 posting clients received `PERIOD_CLOSED`.
- The FOR SHARE (posting) versus FOR UPDATE (close) ordering worked as specified.

## 3. Verdict against the ADR falsification criteria

| # | Criterion (report 12) | Result | Verdict |
|---|---|---|---|
| 1 | The largest sponsor's modelled peak needs more than 50% of sustainable commit capacity, or p99 reservation latency exceeds 500 ms | Peak need is about 200 tx/s against measured capacity of 2,100–3,900 tx/s, which is **5–10% of capacity**. p99 approval latency at 200 tx/s was 8–41 ms. | **Not falsified.** **Exception:** a single-transaction fee batch violates the latency bound (347 ms at 100k funds, about 12 s at 1.42M). Chunking is required (§4). |
| 2 | p99 outbox lag exceeds 60 s on peak days | p99 lag was 4–20 ms, and every row was delivered | **Not falsified** |
| 5 | A C0 interleaving cannot be serialized without unacceptable contention (e.g. hold publication vs release at year end) | Hot-row worst case: 10 funds and 20 charities, with hold publications conflicting with releases. Even unthrottled, approval p99 was 76 ms. At 200 tx/s with RTT, p99 was 41 ms. The period-close race serialized correctly. | **Not falsified**, provided batches are chunked |
| 3, 4, 6 | Contract, auditor and workflow-count criteria | Not testable by benchmark | Open |

**Conclusion.** No measured result supports adding a message broker, a separate event store or distributed services. The real risk was not throughput but a long batch transaction holding hot rows, as the adversarial review suspected (XIF-ADV-006). That risk is now a requirement rather than an assumption.

## 4. Requirement change that follows

UNB-XIN-ERP-001 gains a clause and acceptance criterion AC-D3. The daily fee computation MUST run in chunks so that no transaction holds more than 5,000 fund rows. While the computation runs, the p99 latency of concurrent grant approvals MUST stay within the approval SLO.

The same rule applies to any batch that updates fund rows: income attribution, NAV unit allocation and statement runs (UNB-XIN-EVA-011 already forbids batches from carrying authority). Finding: XIF-INT-010.

## 5. Limits of this evidence

**Where the environment is optimistic:**
- **Storage is faster than a managed database.** It is a single VM with fast local storage, about 8,100 fsync/s. Managed cloud databases commit slower over network storage. The RTT scenarios add 7 ms of lock hold per transaction to approximate this, and capacity still stayed above 2,100 tx/s. But commit fsync latency itself was not slowed.
- **Data is synthetic and uniform.** Real data has more indexes, bigger payloads and skew.
- **Runs were short.** Each run lasted 60 s, so autovacuum, bloat and checkpoint behaviour over a December week were not observed.

**Where the setup departs from the production design:**
- **The client competed for the same 4 CPUs.** Capacity numbers are therefore conservative in that one respect.
- **The hold registry was in the same database.** Production puts it in a separate platform database, which adds a cross-database read at release. Its availability is the single point of failure noted in XIF-ADV-007, and it was not tested here.
- **No concurrent workload besides the outbox.** No reporting queries or snapshot sealing ran alongside the load.

**Next measurement.** Repeat S5–S7 and B2 on the chosen managed PostgreSQL service before production. That would close criterion 1 with production-like storage.

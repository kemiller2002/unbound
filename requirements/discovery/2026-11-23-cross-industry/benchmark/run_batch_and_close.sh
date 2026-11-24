#!/usr/bin/env bash
# Criterion 5 tests: (a) daily fee batch for all funds concurrent with grant approvals, single-transaction vs chunked;
# (b) period close racing live postings. Usage: ./run_batch_and_close.sh <results-dir> [port]
set -euo pipefail
OUT=$(realpath -m "$1"); PORT=${2:-5499}; mkdir -p "$OUT"; S=$(cd "$(dirname "$0")" && pwd)
PG="-h localhost -p $PORT -U postgres"
V="-D fund_lo=1 -D fund_hi=100000 -D charity_lo=1 -D charity_hi=50000"

load () {  # name seconds: grant approvals at 200/s (Fidelity-scale burst) logged for latency
  ( cd "$OUT" && pgbench $PG -n -c 32 -j 4 -T "$2" -R 200 $V -f "$S/grant_approve.sql" -l --log-prefix="$1" sponsor > "$OUT/$1.txt" 2>&1 || true )
}

# (a1) single-transaction batch at t=10s under load
load B1-feebatch-single 40 & L=$!
sleep 10; t0=$(date +%s.%N); psql $PG -d sponsor -q -v day=2026-12-01 -f "$S/fee_batch_single.sql"; echo "$(date +%s.%N) - $t0" | bc > "$OUT/B1-batch-seconds.txt"
wait $L
# (a2) chunked batch (1,000 funds/tx) at t=10s under load
load B2-feebatch-chunked 40 & L=$!
sleep 10; t0=$(date +%s.%N); "$S/fee_batch_chunked.sh" 2026-12-02 100000 "$PORT"; echo "$(date +%s.%N) - $t0" | bc > "$OUT/B2-batch-seconds.txt"
wait $L

# (b) period close race: postings into 202612 at 200/s; close at t=10s; count postings 10s later.
load C1-period-close 25 & L=$!
sleep 10
psql $PG -d sponsor -qc "UPDATE unb.accounting_period SET status = 'hard_closed' WHERE period = 202612"
n1=$(psql $PG -d sponsor -tAc "select count(*) from unb.journal_batch where period = 202612")
sleep 10
n2=$(psql $PG -d sponsor -tAc "select count(*) from unb.journal_batch where period = 202612")
wait $L
aborted=$(grep -c "PERIOD_CLOSED" "$OUT/C1-period-close.txt" || true)
echo "postings_in_period_at_close=$n1,postings_in_period_10s_later=$n2,client_aborts_with_PERIOD_CLOSED=$aborted" > "$OUT/C1-result.txt"
psql $PG -d sponsor -qc "UPDATE unb.accounting_period SET status = 'open' WHERE period = 202612"   # restore for re-runs

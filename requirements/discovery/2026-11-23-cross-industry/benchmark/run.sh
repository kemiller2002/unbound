#!/usr/bin/env bash
# Runs the ADR-EVA-001 benchmark scenarios against a seeded 'sponsor' database. Usage: ./run.sh <results-dir> [port]
set -euo pipefail
OUT=$(realpath -m "$1"); PORT=${2:-5499}; mkdir -p "$OUT"; S=$(cd "$(dirname "$0")" && pwd)
PG="-h localhost -p $PORT -U postgres"
BASEV="-D fund_lo=1 -D fund_hi=100000 -D charity_lo=1 -D charity_hi=50000 -D hist=2000000"
HOTV="-D fund_lo=1 -D fund_hi=10 -D charity_lo=1 -D charity_hi=20 -D hist=2000000"
DUR=${DUR:-60}

scenario () {  # name clients rate(0=unthrottled) vars approve_script release_script
  local name=$1 clients=$2 rate=$3 vars=$4 approve=$5 release=$6
  local start; start=$(psql $PG -d sponsor -tAc "select clock_timestamp()")
  local ropt=""; [ "$rate" != 0 ] && ropt="-R $rate"
  pgbench $PG -n -c 2 -j 1 -T "$DUR" -f "$S/outbox_dispatch.sql" sponsor > "$OUT/$name.dispatch.txt" 2>&1 &
  local dpid=$!
  ( cd "$OUT" && pgbench $PG -n -c "$clients" -j 4 -T "$DUR" $ropt $vars \
      -f "$S/$approve"@70 -f "$S/$release"@25 -f "$S/hold_publish.sql"@5 \
      -l --log-prefix="$name" -P 15 sponsor ) > "$OUT/$name.txt" 2>&1 || true
  wait $dpid || true
  pgbench $PG -n -c 1 -t 50 -f "$S/outbox_dispatch.sql" sponsor > /dev/null 2>&1 || true   # drain
  psql $PG -d sponsor -tA -F, -c "select '$name',
      count(*), count(*) filter (where delivered_at is null),
      round(1000*percentile_cont(0.5) within group (order by extract(epoch from delivered_at - created_at)))::int,
      round(1000*percentile_cont(0.99) within group (order by extract(epoch from delivered_at - created_at)))::int,
      round(1000*max(extract(epoch from delivered_at - created_at)))::int
    from unb.outbox where created_at >= '$start'" >> "$OUT/outbox-lag.csv"
}

echo "scenario,outbox_rows,undelivered_after_drain,lag_p50_ms,lag_p99_ms,lag_max_ms" > "$OUT/outbox-lag.csv"
scenario S1-capacity-spread        32 0   "$BASEV" grant_approve.sql     payment_release.sql
scenario S2-target200-spread       32 200 "$BASEV" grant_approve.sql     payment_release.sql
scenario S3-capacity-hotrows       32 0   "$HOTV"  grant_approve.sql     payment_release.sql
scenario S4-target200-hotrows      32 200 "$HOTV"  grant_approve.sql     payment_release.sql
scenario S5-target200-spread-rtt   32 200 "$BASEV" grant_approve_rtt.sql payment_release_rtt.sql
scenario S6-target200-hotrows-rtt  64 200 "$HOTV"  grant_approve_rtt.sql payment_release_rtt.sql
scenario S7-capacity-spread-rtt    64 0   "$BASEV" grant_approve_rtt.sql payment_release_rtt.sql

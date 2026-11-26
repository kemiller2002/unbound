#!/usr/bin/env sh
# Starts a disposable local PostgreSQL 16 cluster for the Echelon.Execution integration tests and prints the
# connection string to export as ECHELON_EXECUTION_TEST_PG. Usage: scripts/test-postgres.sh [start|stop]
set -eu
PGBIN="${PGBIN:-/usr/lib/postgresql/16/bin}"
DATA="${ECHELON_PG_DATA:-/tmp/echelon-execution-pg}"
PORT="${ECHELON_PG_PORT:-54329}"
RUN_AS="${ECHELON_PG_USER:-postgres}"

as_pg() { if [ "$(id -u)" = "0" ]; then su "$RUN_AS" -s /bin/sh -c "$*"; else sh -c "$*"; fi; }

case "${1:-start}" in
  start)
    if [ ! -f "$DATA/PG_VERSION" ]; then
      mkdir -p "$DATA" && chown "$RUN_AS" "$DATA" 2>/dev/null || true
      as_pg "$PGBIN/initdb -D $DATA -A trust -U postgres >/dev/null"
    fi
    as_pg "$PGBIN/pg_ctl -D $DATA -o '-p $PORT -k /tmp -c listen_addresses=127.0.0.1' -l $DATA/log -w start >/dev/null" || true
    as_pg "$PGBIN/psql -h 127.0.0.1 -p $PORT -U postgres -tAc \"SELECT 1 FROM pg_database WHERE datname='execution_test'\"" | grep -q 1 \
      || as_pg "$PGBIN/createdb -h 127.0.0.1 -p $PORT -U postgres execution_test"
    echo "Host=127.0.0.1;Port=$PORT;Username=postgres;Database=execution_test"
    ;;
  stop)
    as_pg "$PGBIN/pg_ctl -D $DATA -m fast stop"
    ;;
esac

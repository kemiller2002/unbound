#!/usr/bin/env bash
# Daily fee computation in chunks of $CHUNK funds per transaction (short row locks). Usage: fee_batch_chunked.sh <day> <nfunds> [port]
set -euo pipefail
DAY=$1; N=$2; PORT=${3:-5499}; CHUNK=${CHUNK:-1000}
for lo in $(seq 1 "$CHUNK" "$N"); do
  hi=$((lo + CHUNK - 1))
  echo "BEGIN;
INSERT INTO unb.fee_daily (fund_id, day, fee_cents) SELECT fund_id, '$DAY'::date, round(journal_balance_cents * 0.006 / 365) FROM unb.fund WHERE fund_id BETWEEN $lo AND $hi ON CONFLICT DO NOTHING;
UPDATE unb.fund f SET accrued_unposted_fee_cents = accrued_unposted_fee_cents + d.fee_cents FROM unb.fee_daily d WHERE d.fund_id = f.fund_id AND d.day = '$DAY'::date AND f.fund_id BETWEEN $lo AND $hi;
COMMIT;"
done | psql -h localhost -p "$PORT" -U postgres -d sponsor -q -v ON_ERROR_STOP=1

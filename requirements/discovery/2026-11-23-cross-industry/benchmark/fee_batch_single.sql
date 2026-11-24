-- Daily fee computation for every fund in ONE transaction (locks every fund row until commit).
BEGIN;
INSERT INTO unb.fee_daily (fund_id, day, fee_cents)
  SELECT fund_id, :'day'::date, round(journal_balance_cents * 0.006 / 365) FROM unb.fund
  ON CONFLICT DO NOTHING;
UPDATE unb.fund f SET accrued_unposted_fee_cents = accrued_unposted_fee_cents + d.fee_cents
  FROM unb.fee_daily d WHERE d.fund_id = f.fund_id AND d.day = :'day'::date;
COMMIT;

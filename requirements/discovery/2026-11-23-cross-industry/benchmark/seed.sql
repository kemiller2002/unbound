-- :funds, :charities, :history_grants are psql variables.
SET search_path = unb;
INSERT INTO accounting_period VALUES (202611,'open'),(202612,'open');
INSERT INTO fund (fund_id, journal_balance_cents) SELECT g, 5000000 + (g % 97) * 100000 FROM generate_series(1, :funds) g;
INSERT INTO charity_hold (charity_id, held) SELECT g, (g % 1000 = 0) FROM generate_series(1, :charities) g;
-- History so indexes and heap are not trivially small.
INSERT INTO grant_rec (fund_id, charity_id, amount_cents, status, hold_version_seen, idem_key)
  SELECT 1 + (g % :funds), 1 + (g % :charities), 25000, 'paid', 0, 'hist-' || g FROM generate_series(1, :history_grants) g;
INSERT INTO journal_batch (event_id, book, period) SELECT 'hist-' || g, 'gaap', 202611 FROM generate_series(1, :history_grants) g;
INSERT INTO journal_line (batch_id, line_no, account, fund_id, debit_cents, credit_cents)
  SELECT b, l, CASE l WHEN 1 THEN 5010 ELSE 2010 END, 1 + (b % :funds), CASE l WHEN 1 THEN 25000 ELSE 0 END, CASE l WHEN 2 THEN 25000 ELSE 0 END
  FROM generate_series(1, :history_grants) b, generate_series(1,2) l;
UPDATE accounting_period SET status = 'hard_closed' WHERE period = 202611;
ANALYZE;

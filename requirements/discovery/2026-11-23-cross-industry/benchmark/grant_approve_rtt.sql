-- Approval + reservation + recognition journal (ApprovalCommunicatedToGrantee) + outbox, one transaction (TX-11, UNB-RCN-FIN-009).
\set fund random(:fund_lo, :fund_hi)
\set charity random(:charity_lo, :charity_hi)
\set amt random(10000, 500000)
BEGIN;
SELECT held::int AS held, hold_version FROM unb.charity_hold WHERE charity_id = :charity \gset
SELECT pg_sleep(0.001);
UPDATE unb.fund SET reserved_cents = reserved_cents + :amt, version = version + 1
  WHERE fund_id = :fund AND journal_balance_cents - reserved_cents - accrued_unposted_fee_cents >= :amt;
SELECT pg_sleep(0.001);
INSERT INTO unb.grant_rec (fund_id, charity_id, amount_cents, status, hold_version_seen, idem_key)
  VALUES (:fund, :charity, :amt, CASE WHEN :held = 1 THEN 'held' ELSE 'approved' END, :hold_version, gen_random_uuid()::text) RETURNING grant_id \gset
SELECT pg_sleep(0.001);
INSERT INTO unb.reservation VALUES (:grant_id, :fund, :amt);
SELECT pg_sleep(0.001);
INSERT INTO unb.journal_batch (event_id, book, period) VALUES ('grant-' || :grant_id, 'gaap', 202612) RETURNING batch_id \gset
SELECT pg_sleep(0.001);
INSERT INTO unb.journal_line VALUES (:batch_id, 1, 5010, :fund, :amt, 0), (:batch_id, 2, 2010, :fund, 0, :amt);
SELECT pg_sleep(0.001);
INSERT INTO unb.outbox (event_type, payload) VALUES
  ('GrantApproved', json_build_object('grant', :grant_id, 'fund', :fund)::jsonb),
  ('GranteeNotified', json_build_object('grant', :grant_id, 'charity', :charity)::jsonb);
SELECT pg_sleep(0.001);
COMMIT;

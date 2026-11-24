-- Release: hold re-check against the hold registry version at commit (RR-REQ-X-020-r1), payment + journal + outbox.
\set g random(1, :hist)
BEGIN;
SELECT c.held::int AS held, c.hold_version FROM unb.grant_rec g JOIN unb.charity_hold c USING (charity_id) WHERE g.grant_id = :g FOR SHARE OF c \gset
SELECT pg_sleep(0.001);
INSERT INTO unb.payment (grant_id, hold_version_seen, status) VALUES ((random() * 1e15)::bigint, :hold_version,
  CASE WHEN :held = 1 THEN 'blocked' ELSE 'released' END);
SELECT pg_sleep(0.001);
INSERT INTO unb.journal_batch (event_id, book, period) VALUES ('pay-' || gen_random_uuid(), 'gaap', 202612) RETURNING batch_id \gset
SELECT pg_sleep(0.001);
INSERT INTO unb.journal_line VALUES (:batch_id, 1, 2010, NULL, 25000, 0), (:batch_id, 2, 1040, NULL, 0, 25000);
SELECT pg_sleep(0.001);
INSERT INTO unb.outbox (event_type, payload) VALUES ('PaymentReleased', json_build_object('grant', :g)::jsonb);
SELECT pg_sleep(0.001);
COMMIT;

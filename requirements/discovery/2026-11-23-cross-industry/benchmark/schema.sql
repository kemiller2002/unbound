-- Benchmark schema for ADR-EVA-001 falsification criteria 1, 2 and 5 (report 12).
-- One sponsor database. Authority tables are insert-only except fund/period/hold version rows.
DROP SCHEMA IF EXISTS unb CASCADE;
CREATE SCHEMA unb;
SET search_path = unb;

CREATE TABLE accounting_period (period int PRIMARY KEY, status text NOT NULL CHECK (status IN ('open','soft_closed','hard_closed')));
CREATE TABLE fund (
  fund_id int PRIMARY KEY,
  journal_balance_cents bigint NOT NULL,   -- maintained in the same transaction as journal lines
  reserved_cents bigint NOT NULL DEFAULT 0,
  accrued_unposted_fee_cents bigint NOT NULL DEFAULT 0,
  version bigint NOT NULL DEFAULT 0);
-- Platform hold registry (in production a separate database; here same instance, see README).
CREATE TABLE charity_hold (charity_id int PRIMARY KEY, held boolean NOT NULL DEFAULT false, hold_version bigint NOT NULL DEFAULT 0);
CREATE TABLE grant_rec (grant_id bigserial PRIMARY KEY, fund_id int NOT NULL, charity_id int NOT NULL, amount_cents bigint NOT NULL,
  status text NOT NULL, hold_version_seen bigint NOT NULL, idem_key text NOT NULL UNIQUE, created_at timestamptz NOT NULL DEFAULT now());
CREATE TABLE reservation (grant_id bigint PRIMARY KEY, fund_id int NOT NULL, amount_cents bigint NOT NULL);
CREATE TABLE journal_batch (batch_id bigserial PRIMARY KEY, event_id text NOT NULL, book text NOT NULL, period int NOT NULL,
  posted_at timestamptz NOT NULL DEFAULT now(), UNIQUE (event_id, book));
CREATE TABLE journal_line (batch_id bigint NOT NULL, line_no int NOT NULL, account int NOT NULL, fund_id int,
  debit_cents bigint NOT NULL DEFAULT 0, credit_cents bigint NOT NULL DEFAULT 0, PRIMARY KEY (batch_id, line_no));
CREATE INDEX journal_line_fund ON journal_line (fund_id);
CREATE TABLE payment (payment_id bigserial PRIMARY KEY, grant_id bigint NOT NULL UNIQUE, hold_version_seen bigint NOT NULL, status text NOT NULL,
  released_at timestamptz NOT NULL DEFAULT now());
CREATE TABLE outbox (id bigserial PRIMARY KEY, event_type text NOT NULL, payload jsonb NOT NULL, created_at timestamptz NOT NULL DEFAULT clock_timestamp(),
  claimed_by text, delivered_at timestamptz);
CREATE INDEX outbox_undelivered ON outbox (id) WHERE delivered_at IS NULL;
CREATE TABLE fee_daily (fund_id int NOT NULL, day date NOT NULL, fee_cents bigint NOT NULL, PRIMARY KEY (fund_id, day));

-- Period lock enforced by the database: a posting into a non-open period fails (UNB-XIN-EVA-002).
CREATE FUNCTION assert_period_open() RETURNS trigger LANGUAGE plpgsql SET search_path = unb AS $$
BEGIN
  PERFORM 1 FROM accounting_period WHERE period = NEW.period AND status = 'open' FOR SHARE;
  IF NOT FOUND THEN RAISE EXCEPTION 'PERIOD_CLOSED %', NEW.period USING ERRCODE = 'P0001'; END IF;
  RETURN NEW;
END $$;
CREATE TRIGGER journal_batch_period_open BEFORE INSERT ON journal_batch FOR EACH ROW EXECUTE FUNCTION assert_period_open();

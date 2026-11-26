-- Echelon.Execution durable command store (INIT-012 EAR-CMD). {schema} is replaced by a validated identifier.
-- Governed declaratively by Strata in the application's schema set (CNV-ARC-004); this file is the source.
CREATE SCHEMA IF NOT EXISTS {schema};

CREATE TABLE IF NOT EXISTS {schema}.command (
    id              uuid        PRIMARY KEY,
    tenant_id       text        NOT NULL,
    principal_id    text        NOT NULL,
    operation       text        NOT NULL,
    version         integer     NOT NULL,
    request_class   text        NOT NULL,
    idempotency_key text        NOT NULL,
    fingerprint     text        NOT NULL,
    payload         text        NOT NULL,
    ordering_key    text        NULL,
    correlation_id  text        NOT NULL,
    causation       text        NULL,
    accepted_at     timestamptz NOT NULL,
    deadline        timestamptz NULL,
    priority        integer     NOT NULL DEFAULT 0,
    attempts        integer     NOT NULL DEFAULT 0,
    status          text        NOT NULL,
    not_before      timestamptz NULL,
    lease_owner     text        NULL,
    lease_expires   timestamptz NULL,
    outcome_kind    text        NULL,
    outcome_detail  text        NULL,
    fault_code      text        NULL,
    fault_detail    text        NULL,
    await_ref       text        NULL,
    await_ids       uuid[]      NULL,
    CONSTRAINT command_identity UNIQUE (tenant_id, principal_id, operation, idempotency_key),
    CONSTRAINT command_status CHECK (status IN ('Accepted','Scheduled','Processing','AwaitingDependency','AwaitingApproval',
        'RetryScheduled','Completed','Failed','Quarantined','Cancelled','Expired','OutcomeUnknown'))
);

CREATE INDEX IF NOT EXISTS command_open_by_key ON {schema}.command (tenant_id, ordering_key, accepted_at, id)
    WHERE status NOT IN ('Completed','Failed','Cancelled','Expired');
CREATE INDEX IF NOT EXISTS command_open ON {schema}.command (status, not_before)
    WHERE status NOT IN ('Completed','Failed','Cancelled','Expired');

CREATE TABLE IF NOT EXISTS {schema}.command_history (
    seq         bigint      GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    command_id  uuid        NOT NULL REFERENCES {schema}.command (id),
    tenant_id   text        NOT NULL,
    from_status text        NOT NULL,
    to_status   text        NOT NULL,
    at          timestamptz NOT NULL,
    actor       text        NOT NULL,
    reason      text        NOT NULL
);
CREATE INDEX IF NOT EXISTS command_history_by_command ON {schema}.command_history (command_id, seq);

CREATE TABLE IF NOT EXISTS {schema}.outbox (
    effect_key   text        PRIMARY KEY,
    command_id   uuid        NOT NULL REFERENCES {schema}.command (id),
    tenant_id    text        NOT NULL,
    topic        text        NOT NULL,
    payload      text        NOT NULL,
    created_at   timestamptz NOT NULL,
    attempts     integer     NOT NULL DEFAULT 0,
    delivered_at timestamptz NULL
);
CREATE INDEX IF NOT EXISTS outbox_pending ON {schema}.outbox (created_at) WHERE delivered_at IS NULL;

CREATE TABLE IF NOT EXISTS {schema}.pause (
    scope_kind  text NOT NULL CHECK (scope_kind IN ('everything','tenant','operation')),
    scope_value text NOT NULL,
    PRIMARY KEY (scope_kind, scope_value)
);

/// PostgreSQL adapter for Echelon.Execution (CNV-ARC-001: one managed PostgreSQL database, no broker).
/// Every statement is a fixed text with bound parameters (SEC-DB-001); the only interpolated value is the
/// schema name, validated once as an identifier at construction.
module Echelon.Execution.Postgres.PostgresStore

open System
open System.IO
open System.Reflection
open System.Text.RegularExpressions
open System.Threading.Tasks
open Npgsql
open NpgsqlTypes
open Echelon.Execution

let private terminal = "('Completed','Failed','Cancelled','Expired')"

// ---- pure state codec -----------------------------------------------------------------------------------

type StateColumns =
    { Status: string
      NotBefore: Instant option
      LeaseOwner: string option
      LeaseExpires: Instant option
      OutcomeKind: string option
      OutcomeDetail: string option
      FaultCode: string option
      FaultDetail: string option
      AwaitRef: string option
      AwaitIds: Guid list }

let private columns status =
    { Status = status
      NotBefore = None
      LeaseOwner = None
      LeaseExpires = None
      OutcomeKind = None
      OutcomeDetail = None
      FaultCode = None
      FaultDetail = None
      AwaitRef = None
      AwaitIds = [] }

let encode (state: LifecycleState) : StateColumns =
    let c = columns (Lifecycle.label state)

    let fault (f: Fault) =
        { c with
            FaultCode = Some f.Code
            FaultDetail = Some f.Detail }

    match state with
    | Accepted
    | Expired -> c
    | Scheduled nb
    | RetryScheduled nb -> { c with NotBefore = Some nb }
    | Processing(owner, expires) ->
        { c with
            LeaseOwner = Some owner
            LeaseExpires = Some expires }
    | AwaitingDependency ids ->
        { c with
            AwaitIds = ids |> List.map (fun (CommandId g) -> g) }
    | AwaitingApproval r
    | OutcomeUnknown r
    | Cancelled r -> { c with AwaitRef = Some r }
    | Completed(Succeeded d) ->
        { c with
            OutcomeKind = Some "Succeeded"
            OutcomeDetail = Some d }
    | Completed(Declined d) ->
        { c with
            OutcomeKind = Some "Declined"
            OutcomeDetail = Some d }
    | Failed f
    | Quarantined f -> fault f

let decode (c: StateColumns) : Result<LifecycleState, string> =
    let fault () =
        { Code = defaultArg c.FaultCode ""
          Detail = defaultArg c.FaultDetail "" }

    let text = defaultArg c.AwaitRef ""

    match c.Status, c.NotBefore, c.LeaseOwner, c.LeaseExpires with
    | "Accepted", _, _, _ -> Ok Accepted
    | "Expired", _, _, _ -> Ok Expired
    | "Scheduled", Some nb, _, _ -> Ok(Scheduled nb)
    | "RetryScheduled", Some nb, _, _ -> Ok(RetryScheduled nb)
    | "Processing", _, Some owner, Some expires -> Ok(Processing(owner, expires))
    | "AwaitingDependency", _, _, _ -> Ok(AwaitingDependency(c.AwaitIds |> List.map CommandId))
    | "AwaitingApproval", _, _, _ -> Ok(AwaitingApproval text)
    | "OutcomeUnknown", _, _, _ -> Ok(OutcomeUnknown text)
    | "Cancelled", _, _, _ -> Ok(Cancelled text)
    | "Completed", _, _, _ ->
        match c.OutcomeKind with
        | Some "Succeeded" -> Ok(Completed(Succeeded(defaultArg c.OutcomeDetail "")))
        | Some "Declined" -> Ok(Completed(Declined(defaultArg c.OutcomeDetail "")))
        | other -> Error(sprintf "Completed with outcome kind %A" other)
    | "Failed", _, _, _ -> Ok(Failed(fault ()))
    | "Quarantined", _, _, _ -> Ok(Quarantined(fault ()))
    | status, _, _, _ -> Error(sprintf "undecodable status %s" status)

// ---- SQL ------------------------------------------------------------------------------------------------

let private identifier = Regex("^[a-z_][a-z0-9_]{0,62}$", RegexOptions.Compiled)

let validateSchema (schema: string) =
    if identifier.IsMatch schema then Ok schema else Error(sprintf "invalid schema identifier '%s'" schema)

let schemaDdl (schema: string) =
    use stream =
        Assembly.GetExecutingAssembly().GetManifestResourceStream("Echelon.Execution.Postgres.Schema.sql")

    use reader = new StreamReader(stream)
    reader.ReadToEnd().Replace("{schema}", schema)

let private commandColumns =
    "id, tenant_id, principal_id, operation, version, request_class, idempotency_key, fingerprint, payload, ordering_key, \
     correlation_id, causation, accepted_at, deadline, priority, attempts, status, not_before, lease_owner, lease_expires, \
     outcome_kind, outcome_detail, fault_code, fault_detail, await_ref, await_ids"

type private Sql =
    { Insert: string
      SelectByIdentity: string
      CountOpen: string
      Claim: string
      LockForExecute: string
      UpdateState: string
      InsertHistory: string
      InsertOutbox: string
      LockForApply: string
      SelectById: string
      History: string
      DueForExpiry: string
      PauseInsert: string
      PauseDelete: string
      Pending: string
      Mark: string }

let private sql (s: string) : Sql =
    let returning = commandColumns.Split(',') |> Array.map (fun c -> "c." + c.Trim()) |> String.concat ", "

    { Insert =
        $"INSERT INTO {s}.command ({commandColumns}) VALUES (@id,@tenant,@principal,@operation,@version,@class,@key,@fingerprint,@payload,@ordering,@correlation,@causation,@accepted,@deadline,@priority,0,@status,@not_before,NULL,NULL,NULL,NULL,NULL,NULL,NULL,NULL) ON CONFLICT ON CONSTRAINT command_identity DO NOTHING RETURNING id"
      SelectByIdentity =
        $"SELECT {commandColumns} FROM {s}.command WHERE tenant_id=@tenant AND principal_id=@principal AND operation=@operation AND idempotency_key=@key"
      CountOpen = $"SELECT count(*) FROM {s}.command WHERE tenant_id=@tenant AND status NOT IN {terminal}"
      // Heads of ordering keys (oldest open command per key), claimable now, not paused, round-robin across
      // tenants by priority then age; rows locked by another claimer are skipped.
      Claim =
        $"""
WITH open_cmd AS (
  SELECT c.* FROM {s}.command c WHERE c.status NOT IN {terminal}
), heads AS (
  SELECT o.* FROM open_cmd o
  WHERE o.ordering_key IS NULL OR NOT EXISTS (
    SELECT 1 FROM open_cmd e WHERE e.tenant_id = o.tenant_id AND e.ordering_key = o.ordering_key
      AND (e.accepted_at, e.id) < (o.accepted_at, o.id))
), eligible AS (
  SELECT h.id, h.tenant_id, h.priority, h.accepted_at FROM heads h
  WHERE (h.status = 'Accepted'
      OR (h.status IN ('Scheduled','RetryScheduled') AND h.not_before <= @now)
      OR (h.status = 'Processing' AND h.lease_expires <= @now))
    AND NOT EXISTS (SELECT 1 FROM {s}.pause p WHERE p.scope_kind = 'everything'
      OR (p.scope_kind = 'tenant' AND p.scope_value = h.tenant_id)
      OR (p.scope_kind = 'operation' AND p.scope_value = (SELECT operation FROM {s}.command x WHERE x.id = h.id)))
), ranked AS (
  SELECT id, tenant_id, row_number() OVER (PARTITION BY tenant_id ORDER BY priority DESC, accepted_at, id) AS rn FROM eligible
), chosen AS (
  SELECT c.id, c.status AS prior_status FROM {s}.command c JOIN ranked r ON r.id = c.id
  WHERE c.status IN ('Accepted','Scheduled','RetryScheduled','Processing')
    AND (c.status <> 'Processing' OR c.lease_expires <= @now)
  ORDER BY r.rn, r.tenant_id, c.id
  LIMIT @limit
  FOR UPDATE OF c SKIP LOCKED
)
UPDATE {s}.command c SET status = 'Processing', lease_owner = @owner, lease_expires = @expires,
  not_before = NULL
FROM chosen WHERE c.id = chosen.id
RETURNING {returning}, chosen.prior_status"""
      LockForExecute = $"SELECT {commandColumns} FROM {s}.command WHERE id=@id FOR UPDATE"
      UpdateState =
        $"UPDATE {s}.command SET status=@status, attempts=@attempts, not_before=@not_before, lease_owner=@lease_owner, lease_expires=@lease_expires, outcome_kind=@outcome_kind, outcome_detail=@outcome_detail, fault_code=@fault_code, fault_detail=@fault_detail, await_ref=@await_ref, await_ids=@await_ids WHERE id=@id"
      InsertHistory =
        $"INSERT INTO {s}.command_history (command_id, tenant_id, from_status, to_status, at, actor, reason) VALUES (@id,@tenant,@from,@to,@at,@actor,@reason)"
      InsertOutbox =
        $"INSERT INTO {s}.outbox (effect_key, command_id, tenant_id, topic, payload, created_at) VALUES (@key,@id,@tenant,@topic,@payload,@at) ON CONFLICT (effect_key) DO NOTHING"
      LockForApply = $"SELECT {commandColumns} FROM {s}.command WHERE id=@id AND tenant_id=@tenant FOR UPDATE"
      SelectById = $"SELECT {commandColumns} FROM {s}.command WHERE id=@id AND tenant_id=@tenant"
      History =
        $"SELECT h.command_id, h.from_status, h.to_status, h.at, h.actor, h.reason FROM {s}.command_history h WHERE h.command_id=@id AND h.tenant_id=@tenant ORDER BY h.seq"
      DueForExpiry =
        $"SELECT {commandColumns} FROM {s}.command WHERE deadline < @now AND status IN ('Accepted','Scheduled','RetryScheduled','AwaitingDependency') FOR UPDATE SKIP LOCKED"
      PauseInsert = $"INSERT INTO {s}.pause (scope_kind, scope_value) VALUES (@kind,@value) ON CONFLICT DO NOTHING"
      PauseDelete = $"DELETE FROM {s}.pause WHERE scope_kind=@kind AND scope_value=@value"
      Pending =
        $"SELECT command_id, tenant_id, effect_key, topic, payload, attempts FROM {s}.outbox WHERE delivered_at IS NULL ORDER BY created_at, effect_key LIMIT @limit"
      Mark =
        $"UPDATE {s}.outbox SET attempts = attempts + 1, delivered_at = CASE WHEN @delivered THEN @at ELSE NULL END WHERE effect_key=@key" }

// ---- parameter and row helpers (edge) -------------------------------------------------------------------

let private utc (i: Instant) = i.ToUniversalTime()

let private param (cmd: NpgsqlCommand) (name: string) (value: obj) =
    cmd.Parameters.AddWithValue(name, (if isNull value then box DBNull.Value else value)) |> ignore

let private opt (v: 'a option) : obj =
    match v with
    | Some x -> box x
    | None -> null

let private optInstant (v: Instant option) = v |> Option.map utc |> opt

let private command (conn: NpgsqlConnection) (tx: NpgsqlTransaction) (text: string) =
    new NpgsqlCommand(text, conn, tx)

let private readOpt<'a> (r: NpgsqlDataReader) (name: string) : 'a option =
    let i = r.GetOrdinal name
    if r.IsDBNull i then None else Some(r.GetFieldValue<'a> i)

let private readCommand (r: NpgsqlDataReader) : Result<Command, string> =
    let state =
        decode
            { Status = r.GetString(r.GetOrdinal "status")
              NotBefore = readOpt<DateTimeOffset> r "not_before"
              LeaseOwner = readOpt<string> r "lease_owner"
              LeaseExpires = readOpt<DateTimeOffset> r "lease_expires"
              OutcomeKind = readOpt<string> r "outcome_kind"
              OutcomeDetail = readOpt<string> r "outcome_detail"
              FaultCode = readOpt<string> r "fault_code"
              FaultDetail = readOpt<string> r "fault_detail"
              AwaitRef = readOpt<string> r "await_ref"
              AwaitIds = readOpt<Guid[]> r "await_ids" |> Option.map List.ofArray |> Option.defaultValue [] }

    let requestClass = r.GetString(r.GetOrdinal "request_class")

    match state, RequestClass.parse requestClass with
    | Error e, _ -> Error e
    | _, None -> Error(sprintf "unknown request class %s" requestClass)
    | Ok state, Some cls ->
        Ok
            { Id = CommandId(r.GetGuid(r.GetOrdinal "id"))
              Tenant = TenantId(r.GetString(r.GetOrdinal "tenant_id"))
              Principal = PrincipalId(r.GetString(r.GetOrdinal "principal_id"))
              Operation = OperationName(r.GetString(r.GetOrdinal "operation"))
              Version = r.GetInt32(r.GetOrdinal "version")
              Class = cls
              IdempotencyKey = IdempotencyKey(r.GetString(r.GetOrdinal "idempotency_key"))
              Fingerprint = r.GetString(r.GetOrdinal "fingerprint")
              Payload = r.GetString(r.GetOrdinal "payload")
              OrderingKey = readOpt<string> r "ordering_key" |> Option.map OrderingKey
              Correlation = CorrelationId(r.GetString(r.GetOrdinal "correlation_id"))
              Causation = readOpt<string> r "causation"
              AcceptedAt = r.GetFieldValue<DateTimeOffset>(r.GetOrdinal "accepted_at")
              Deadline = readOpt<DateTimeOffset> r "deadline"
              Priority = r.GetInt32(r.GetOrdinal "priority")
              Attempts = r.GetInt32(r.GetOrdinal "attempts")
              State = state }

let private readAll (cmd: NpgsqlCommand) (read: NpgsqlDataReader -> 'a) : Task<'a list> =
    task {
        use! reader = cmd.ExecuteReaderAsync()
        let r = reader :?> NpgsqlDataReader
        let rows = ResizeArray<'a>() // accumulation at the I/O edge; returned as an immutable list

        let! first = r.ReadAsync()
        let mutable more = first

        while more do
            rows.Add(read r)
            let! next = r.ReadAsync()
            more <- next

        return List.ofSeq rows
    }

let private commands (cmd: NpgsqlCommand) : Task<Result<Command list, StoreFault>> =
    task {
        let! rows = readAll cmd readCommand

        return
            rows
            |> List.fold
                (fun acc row ->
                    match acc, row with
                    | Ok xs, Ok c -> Ok(xs @ [ c ])
                    | Error e, _ -> Error e
                    | _, Error e -> Error(Unavailable("corrupt row: " + e)))
                (Ok [])
    }

let private guard (f: unit -> Task<Result<'a, StoreFault>>) : Task<Result<'a, StoreFault>> =
    task {
        try
            return! f ()
        with
        | :? NpgsqlException as e -> return Error(Unavailable e.Message)
        | :? TimeoutException as e -> return Error(Unavailable e.Message)
        | :? System.Net.Sockets.SocketException as e -> return Error(Unavailable e.Message)
    }

let private writeState (q: Sql) conn tx (id: CommandId) (state: LifecycleState) (attempts: int) =
    task {
        let (CommandId g) = id
        let c = encode state
        use cmd = command conn tx q.UpdateState
        param cmd "id" g
        param cmd "status" c.Status
        param cmd "attempts" attempts
        param cmd "not_before" (optInstant c.NotBefore)
        param cmd "lease_owner" (opt c.LeaseOwner)
        param cmd "lease_expires" (optInstant c.LeaseExpires)
        param cmd "outcome_kind" (opt c.OutcomeKind)
        param cmd "outcome_detail" (opt c.OutcomeDetail)
        param cmd "fault_code" (opt c.FaultCode)
        param cmd "fault_detail" (opt c.FaultDetail)
        param cmd "await_ref" (opt c.AwaitRef)

        cmd.Parameters.Add(
            NpgsqlParameter("await_ids", NpgsqlDbType.Array ||| NpgsqlDbType.Uuid,
                            Value = (if c.AwaitIds.IsEmpty then box DBNull.Value else box (Array.ofList c.AwaitIds)))
        )
        |> ignore

        let! _ = cmd.ExecuteNonQueryAsync()
        return ()
    }

let private writeHistory (q: Sql) conn tx (c: Command) (next: LifecycleState) (at: Instant) (actor: string) (reason: string) =
    task {
        let (CommandId g) = c.Id
        let (TenantId t) = c.Tenant
        use cmd = command conn tx q.InsertHistory
        param cmd "id" g
        param cmd "tenant" t
        param cmd "from" (Lifecycle.label c.State)
        param cmd "to" (Lifecycle.label next)
        param cmd "at" (utc at)
        param cmd "actor" actor
        param cmd "reason" reason
        let! _ = cmd.ExecuteNonQueryAsync()
        return ()
    }

// ---- store --------------------------------------------------------------------------------------------

/// Create the schema if absent (idempotent). Production deployments apply the DDL through Strata instead.
let ensureSchema (dataSource: NpgsqlDataSource) (schema: string) : Task<Result<unit, string>> =
    task {
        match validateSchema schema with
        | Error e -> return Error e
        | Ok s ->
            use cmd = dataSource.CreateCommand(schemaDdl s)
            let! _ = cmd.ExecuteNonQueryAsync()
            return Ok()
    }

let create (dataSource: NpgsqlDataSource) (schema: string) (clock: unit -> Instant) : Result<Store<NpgsqlTransaction>, string> =
    validateSchema schema
    |> Result.map (fun s ->
        let q = sql s

        let inTransaction (body: NpgsqlConnection -> NpgsqlTransaction -> Task<Result<'a, StoreFault>>) =
            guard (fun () ->
                task {
                    use! conn = dataSource.OpenConnectionAsync()
                    use! tx = conn.BeginTransactionAsync()
                    let! result = body conn tx

                    match result with
                    | Ok _ -> do! tx.CommitAsync()
                    | Error _ -> do! tx.RollbackAsync()

                    return result
                })

        let lockOne (conn: NpgsqlConnection) tx (id: CommandId) =
            task {
                let (CommandId g) = id
                use cmd = command conn tx q.LockForExecute
                param cmd "id" g
                let! rows = commands cmd
                return rows |> Result.map List.tryHead
            }

        let persist conn tx (c: Command) (st: Settlement) (owner: string) =
            task {
                let at = clock ()
                do! writeState q conn tx c.Id st.NewState st.Attempts
                do! writeHistory q conn tx c st.NewState at owner st.Reason
                let (CommandId g) = c.Id
                let (TenantId t) = c.Tenant

                for m in st.Outbox do
                    use cmd = command conn tx q.InsertOutbox
                    param cmd "key" m.EffectKey
                    param cmd "id" g
                    param cmd "tenant" t
                    param cmd "topic" m.Topic
                    param cmd "payload" m.Payload
                    param cmd "at" (utc at)
                    let! _ = cmd.ExecuteNonQueryAsync()
                    ()
            }

        let holds owner (c: Command) =
            match c.State with
            | Processing(o, _) -> o = owner
            | _ -> false

        let applyEvent conn tx (c: Command) (event: Lifecycle.Event) (actor: string) (reason: string) =
            task {
                match Lifecycle.transition c.State event with
                | Error illegal -> return Error(Illegal illegal)
                | Ok next ->
                    do! writeState q conn tx c.Id next c.Attempts
                    do! writeHistory q conn tx c next (clock ()) actor reason
                    return Ok next
            }

        { Insert =
            fun c ->
                inTransaction (fun conn tx ->
                    task {
                        let (CommandId g) = c.Id
                        let (TenantId t) = c.Tenant
                        let (PrincipalId p) = c.Principal
                        let (OperationName o) = c.Operation
                        let (IdempotencyKey k) = c.IdempotencyKey
                        let (CorrelationId corr) = c.Correlation
                        let state = encode c.State
                        use cmd = command conn tx q.Insert
                        param cmd "id" g
                        param cmd "tenant" t
                        param cmd "principal" p
                        param cmd "operation" o
                        param cmd "version" c.Version
                        param cmd "class" (RequestClass.name c.Class)
                        param cmd "key" k
                        param cmd "fingerprint" c.Fingerprint
                        param cmd "payload" c.Payload
                        param cmd "ordering" (c.OrderingKey |> Option.map (fun (OrderingKey x) -> x) |> opt)
                        param cmd "correlation" corr
                        param cmd "causation" (opt c.Causation)
                        param cmd "accepted" (utc c.AcceptedAt)
                        param cmd "deadline" (optInstant c.Deadline)
                        param cmd "priority" c.Priority
                        param cmd "status" state.Status
                        param cmd "not_before" (optInstant state.NotBefore)
                        let! inserted = cmd.ExecuteScalarAsync()

                        if isNull inserted then
                            use existing = command conn tx q.SelectByIdentity
                            param existing "tenant" t
                            param existing "principal" p
                            param existing "operation" o
                            param existing "key" k
                            let! rows = commands existing

                            return
                                rows
                                |> Result.bind (function
                                    | [ found ] -> Ok(Existing found)
                                    | _ -> Error(Unavailable "identity conflict without a visible row"))
                        else
                            do! writeHistory q conn tx { c with State = Accepted } c.State c.AcceptedAt "intake" "accepted"
                            return Ok Inserted
                    })
          CountNonTerminal =
            fun (TenantId t) ->
                guard (fun () ->
                    task {
                        use cmd = dataSource.CreateCommand(q.CountOpen)
                        param cmd "tenant" t
                        let! n = cmd.ExecuteScalarAsync()
                        return Ok(int (unbox<int64> n))
                    })
          Claim =
            fun r ->
                inTransaction (fun conn tx ->
                    task {
                        use cmd = command conn tx q.Claim
                        param cmd "now" (utc r.Now)
                        param cmd "owner" r.Owner
                        param cmd "expires" (utc r.LeaseExpires)
                        param cmd "limit" r.Limit
                        let! rows = readAll cmd (fun rd -> readCommand rd, rd.GetString(rd.GetOrdinal "prior_status"))

                        match rows |> List.tryPick (function Error e, _ -> Some e | _ -> None) with
                        | Some corrupt -> return Error(Unavailable("corrupt row: " + corrupt))
                        | None ->
                            let claimed = rows |> List.choose (function Ok c, prior -> Some(c, prior) | _ -> None)

                            for c, prior in claimed do
                                use h = command conn tx q.InsertHistory
                                let (CommandId g) = c.Id
                                let (TenantId t) = c.Tenant
                                param h "id" g
                                param h "tenant" t
                                param h "from" prior
                                param h "to" (Lifecycle.label c.State)
                                param h "at" (utc r.Now)
                                param h "actor" r.Owner
                                param h "reason" "claimed"
                                let! _ = h.ExecuteNonQueryAsync()
                                ()

                            return Ok(claimed |> List.map fst)
                    })
          Execute =
            fun id owner run crash ->
                task {
                    // First transaction: lock, verify the lease, run the handler, persist. A handler exception
                    // rolls this transaction back (business writes included) and is settled in a second one.
                    let! first =
                        guard (fun () ->
                            task {
                                use! conn = dataSource.OpenConnectionAsync()
                                use! tx = conn.BeginTransactionAsync()

                                match! lockOne conn tx id with
                                | Error e -> return Error e
                                | Ok None -> return Error NotFound
                                | Ok(Some c) when not (holds owner c) -> return Error LeaseNotHeld
                                | Ok(Some c) ->
                                    let! outcome =
                                        task {
                                            try
                                                let! st = run tx c
                                                return Choice1Of2 st
                                            with
                                            | :? NpgsqlException as e -> return Choice2Of2(Error(Unavailable e.Message))
                                            | error -> return Choice2Of2(Ok error)
                                        }

                                    match outcome with
                                    | Choice1Of2 st ->
                                        do! persist conn tx c st owner
                                        do! tx.CommitAsync()
                                        return Ok(Choice1Of2 st)
                                    | Choice2Of2(Ok error) ->
                                        do! tx.RollbackAsync()
                                        return Ok(Choice2Of2 error)
                                    | Choice2Of2(Error fault) ->
                                        do! tx.RollbackAsync()
                                        return Error fault
                            })

                    match first with
                    | Error e -> return Error e
                    | Ok(Choice1Of2 st) -> return Ok st
                    | Ok(Choice2Of2 error) ->
                        return!
                            inTransaction (fun conn tx ->
                                task {
                                    match! lockOne conn tx id with
                                    | Error e -> return Error e
                                    | Ok None -> return Error NotFound
                                    | Ok(Some c) when not (holds owner c) -> return Error LeaseNotHeld
                                    | Ok(Some c) ->
                                        let st = crash c error
                                        do! persist conn tx c st owner
                                        return Ok st
                                })
                }
          Apply =
            fun (TenantId t) (CommandId g) event actor reason ->
                inTransaction (fun conn tx ->
                    task {
                        use cmd = command conn tx q.LockForApply
                        param cmd "id" g
                        param cmd "tenant" t

                        match! commands cmd with
                        | Error e -> return Error e
                        | Ok [ c ] -> return! applyEvent conn tx c event actor reason
                        | Ok _ -> return Error NotFound
                    })
          Get =
            fun (TenantId t) (CommandId g) ->
                guard (fun () ->
                    task {
                        use cmd = dataSource.CreateCommand(q.SelectById)
                        param cmd "id" g
                        param cmd "tenant" t
                        let! rows = commands cmd
                        return rows |> Result.map List.tryHead
                    })
          History =
            fun (TenantId t) (CommandId g) ->
                guard (fun () ->
                    task {
                        use cmd = dataSource.CreateCommand(q.History)
                        param cmd "id" g
                        param cmd "tenant" t

                        let! rows =
                            readAll cmd (fun r ->
                                { Command = CommandId(r.GetGuid 0)
                                  From = r.GetString 1
                                  To = r.GetString 2
                                  At = r.GetFieldValue<DateTimeOffset> 3
                                  Actor = r.GetString 4
                                  Reason = r.GetString 5 })

                        return Ok rows
                    })
          ExpireDue =
            fun now ->
                inTransaction (fun conn tx ->
                    task {
                        use cmd = command conn tx q.DueForExpiry
                        param cmd "now" (utc now)

                        match! commands cmd with
                        | Error e -> return Error e
                        | Ok due ->
                            for c in due do
                                let! _ = applyEvent conn tx c Lifecycle.Expire "system" "processing deadline passed"
                                ()

                            return Ok(due |> List.map (fun c -> c.Id))
                    })
          SetPause =
            fun scope paused ->
                guard (fun () ->
                    task {
                        let kind, value =
                            match scope with
                            | Everything -> "everything", "*"
                            | Tenant(TenantId t) -> "tenant", t
                            | Operation(OperationName o) -> "operation", o

                        use cmd = dataSource.CreateCommand(if paused then q.PauseInsert else q.PauseDelete)
                        param cmd "kind" kind
                        param cmd "value" value
                        let! _ = cmd.ExecuteNonQueryAsync()
                        return Ok()
                    })
          PendingOutbox =
            fun limit ->
                guard (fun () ->
                    task {
                        use cmd = dataSource.CreateCommand(q.Pending)
                        param cmd "limit" limit

                        let! rows =
                            readAll cmd (fun r ->
                                { Command = CommandId(r.GetGuid 0)
                                  Tenant = TenantId(r.GetString 1)
                                  Message =
                                    { EffectKey = r.GetString 2
                                      Topic = r.GetString 3
                                      Payload = r.GetString 4 }
                                  Attempts = r.GetInt32 5 })

                        return Ok rows
                    })
          MarkOutbox =
            fun key delivered ->
                guard (fun () ->
                    task {
                        use cmd = dataSource.CreateCommand(q.Mark)
                        param cmd "key" key
                        param cmd "delivered" delivered
                        param cmd "at" (utc (clock ()))
                        let! _ = cmd.ExecuteNonQueryAsync()
                        return Ok()
                    }) })

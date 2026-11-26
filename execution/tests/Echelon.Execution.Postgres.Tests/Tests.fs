module Echelon.Execution.Postgres.Tests.Tests

open System
open System.Threading.Tasks
open Npgsql
open Xunit
open Echelon.Execution
open Echelon.Execution.Postgres

/// Integration tests need a real PostgreSQL. Start one with scripts/test-postgres.sh and export its output as
/// ECHELON_EXECUTION_TEST_PG. Without it the tests fail (they are never silently skipped).
let connectionString () =
    match Environment.GetEnvironmentVariable "ECHELON_EXECUTION_TEST_PG" with
    | null
    | "" -> failwith "ECHELON_EXECUTION_TEST_PG is not set; run execution/scripts/test-postgres.sh and export its output"
    | cs -> cs

let dataSource = lazy (NpgsqlDataSource.Create(connectionString ()))

let t0 = DateTimeOffset(2026, 12, 31, 23, 58, 0, TimeSpan.Zero)

type Clock() =
    let mutable now = t0
    member _.Now() = now
    member _.Advance(span: TimeSpan) = now <- now.Add span

let tenantA = TenantId "sponsor-a"
let op = OperationName "grant.recommend"

/// A fresh schema per test, plus a business table the handler writes to inside the command transaction.
let fresh (clock: Clock) =
    task {
        let schema = "t_" + Guid.NewGuid().ToString("N").Substring(0, 12)
        let! ensured = PostgresStore.ensureSchema dataSource.Value schema
        Assert.Equal(Ok(), ensured)
        use cmd = dataSource.Value.CreateCommand($"CREATE TABLE {schema}.business (payload text NOT NULL)")
        let! _ = cmd.ExecuteNonQueryAsync()

        match PostgresStore.create dataSource.Value schema clock.Now with
        | Ok store -> return schema, store
        | Error e -> return failwith e
    }

let business (schema: string) =
    task {
        use cmd = dataSource.Value.CreateCommand($"SELECT payload FROM {schema}.business ORDER BY payload")
        use! r = cmd.ExecuteReaderAsync()
        let rows = ResizeArray<string>()
        let! first = r.ReadAsync()
        let mutable more = first

        while more do
            rows.Add(r.GetString 0)
            let! next = r.ReadAsync()
            more <- next

        return List.ofSeq rows
    }

let submission key payload orderingKey : Intake.Submission =
    { Tenant = tenantA
      Principal = PrincipalId "donor-1"
      Operation = op
      Version = 1
      Class = DurableCommand
      IdempotencyKey = IdempotencyKey key
      Payload = payload
      OrderingKey = orderingKey |> Option.map OrderingKey
      Correlation = CorrelationId("corr-" + key)
      Causation = None
      NotBefore = None
      Deadline = None
      Priority = 0 }

let accept store (clock: Clock) s =
    Intake.accept store Intake.defaultLimits Guid.NewGuid clock.Now s

let acceptedId =
    function
    | Ok(Intake.NewlyAccepted r)
    | Ok(Intake.Duplicate r) -> r.Command
    | Error e -> failwithf "expected acceptance, got %A" e

/// Handler that records its payload in the business table through the command transaction.
let writing (schema: string) (crashOn: string option) : Worker.Operation<NpgsqlTransaction> =
    { Name = op
      CurrentVersion = 1
      Upcast = fun _ p -> Ok p
      Authorize = fun _ -> Task.FromResult(Ok())
      Execute =
        fun ctx ->
            task {
                use cmd = new NpgsqlCommand($"INSERT INTO {schema}.business (payload) VALUES (@p)", ctx.Tx.Connection, ctx.Tx)
                cmd.Parameters.AddWithValue("p", ctx.Payload) |> ignore
                let! _ = cmd.ExecuteNonQueryAsync()

                if Some ctx.Payload = crashOn then
                    return failwith "handler crashed after writing"
                else
                    return Worker.Done(Succeeded "ok", [ "payments.instruction", ctx.Payload ])
            } }

let settings owner =
    { Worker.Owner = owner
      Worker.LeaseDuration = TimeSpan.FromMinutes 1.0
      Worker.Retry = Policy.defaultRetryPolicy
      Worker.BatchSize = 10 }

let cycle store operation owner (clock: Clock) =
    Worker.cycle store (Map.ofList [ op, operation ]) (settings owner) clock.Now (fun () -> 0.5)

let stateOf (store: Store<_>) id =
    task {
        let! c = store.Get tenantA id

        match c with
        | Ok(Some c) -> return c.State
        | other -> return failwithf "missing command %A" other
    }

// ---- pure codec --------------------------------------------------------------------------------------

[<Fact>]
let ``codec: every lifecycle state round-trips through its columns`` () =
    let states =
        [ Accepted
          Scheduled t0
          Processing("w", t0)
          AwaitingDependency [ CommandId(Guid.NewGuid()); CommandId(Guid.NewGuid()) ]
          AwaitingApproval "tier-2"
          RetryScheduled t0
          Completed(Succeeded "ok")
          Completed(Declined "insufficient funds")
          Failed { Code = "X"; Detail = "y" }
          Quarantined { Code = "P"; Detail = "q" }
          Cancelled "donor withdrew"
          Expired
          OutcomeUnknown "trace pending" ]

    for s in states do
        Assert.Equal(Ok s, PostgresStore.decode (PostgresStore.encode s))

[<Fact>]
let ``schema: rejects a non-identifier schema name and is idempotent`` () =
    task {
        Assert.True(Result.isError (PostgresStore.validateSchema "x; DROP TABLE y"))
        let clock = Clock()
        let! schema, _ = fresh clock
        let! again = PostgresStore.ensureSchema dataSource.Value schema
        Assert.Equal(Ok(), again)
    }

// ---- intake ---------------------------------------------------------------------------------------------

[<Fact>]
let ``intake: duplicate keys resolve to one row under concurrency; changed payload conflicts`` () =
    task {
        let clock = Clock()
        let! _, store = fresh clock

        let! results =
            Array.init 50 (fun _ ->
                Task.Run<Result<Intake.Response, Intake.Rejection>>(
                    Func<Task<Result<Intake.Response, Intake.Rejection>>>(fun () -> accept store clock (submission "same" "p" None))
                ))
            |> Task.WhenAll

        let ids = results |> Array.map acceptedId |> Array.distinct
        Assert.Equal(1, ids.Length)
        Assert.Equal(1, results |> Array.filter (function Ok(Intake.NewlyAccepted _) -> true | _ -> false) |> Array.length)
        let! conflict = accept store clock (submission "same" "different" None)
        Assert.Equal(Error(Intake.IdempotencyConflict ids.[0]), conflict)
    }

[<Fact>]
let ``intake: an unreachable database is reported as 'not accepted' (EAR-AVL-010)`` () =
    task {
        let clock = Clock()
        use unreachable = NpgsqlDataSource.Create("Host=127.0.0.1;Port=1;Username=x;Database=x;Timeout=2")

        match PostgresStore.create unreachable "nowhere" clock.Now with
        | Error e -> failwith e
        | Ok store ->
            let! r = accept store clock (submission "k" "p" None)
            Assert.Equal(Error(Intake.AcceptanceUnavailable(IdempotencyKey "k")), r)
    }

// ---- execution ----------------------------------------------------------------------------------------------

[<Fact>]
let ``execute: business write, completion, history and outbox commit together`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock
        let! r = accept store clock (submission "k" "grant-1" (Some "fund-1"))
        let id = acceptedId r
        let! report = cycle store (writing schema None) "w1" clock
        Assert.Equal(1, report.Claimed)
        let! state = stateOf store id
        Assert.Equal(Completed(Succeeded "ok"), state)
        let! rows = business schema
        Assert.Equal<string list>([ "grant-1" ], rows)
        let! pending = store.PendingOutbox 10
        Assert.Equal<string list>([ Worker.effectKey id 0 ], pending |> Result.defaultValue [] |> List.map (fun e -> e.Message.EffectKey))
        let! history = store.History tenantA id
        Assert.Equal<string list>([ "Accepted"; "Processing"; "Completed" ], history |> Result.defaultValue [] |> List.map (fun h -> h.To))
    }

[<Fact>]
let ``execute: a crashing handler's business write is rolled back and a retry is scheduled`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock
        let! r = accept store clock (submission "k" "boom" (Some "fund-1"))
        let! _ = cycle store (writing schema (Some "boom")) "w1" clock
        let! rows = business schema
        Assert.Empty(rows)

        match! stateOf store (acceptedId r) with
        | RetryScheduled _ -> ()
        | other -> failwithf "expected RetryScheduled, got %A" other

        let! pending = store.PendingOutbox 10
        Assert.Equal(Ok [], pending)
    }

[<Fact>]
let ``claim: concurrent workers execute each command exactly once and preserve per-key order`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock

        for i in 1..10 do
            for key in [ "fund-1"; "fund-2"; "fund-3"; "fund-4" ] do
                let! _ = accept store clock (submission (sprintf "%s-%02d" key i) (sprintf "%s-%02d" key i) (Some key))
                clock.Advance(TimeSpan.FromMilliseconds 1.0)

        let worker name =
            task {
                let mutable claimedAny = true

                while claimedAny do
                    let! report = cycle store (writing schema None) name clock
                    claimedAny <- report.Claimed > 0 || not report.Faults.IsEmpty
            }

        let! _ = Task.WhenAll [| worker "w1"; worker "w2"; worker "w3"; worker "w4" |]
        let! rows = business schema
        Assert.Equal(40, rows.Length)
        Assert.Equal(40, rows |> List.distinct |> List.length)

        // business rows carry no insertion order; verify order from lifecycle history instead
        use cmd =
            dataSource.Value.CreateCommand(
                $"SELECT c.payload FROM {schema}.command_history h JOIN {schema}.command c ON c.id = h.command_id WHERE h.to_status = 'Completed' ORDER BY h.seq"
            )

        use! rd = cmd.ExecuteReaderAsync()
        let completed = ResizeArray<string>()
        let! first = rd.ReadAsync()
        let mutable more = first

        while more do
            completed.Add(rd.GetString 0)
            let! next = rd.ReadAsync()
            more <- next

        for key in [ "fund-1"; "fund-2"; "fund-3"; "fund-4" ] do
            let order = completed |> Seq.filter (fun (p: string) -> p.StartsWith key) |> List.ofSeq
            Assert.Equal<string list>([ for i in 1..10 -> sprintf "%s-%02d" key i ], order)
    }

[<Fact>]
let ``lease: an expired lease is reclaimed and the stale owner cannot settle`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock
        let! r = accept store clock (submission "k" "grant" (Some "fund-1"))
        let id = acceptedId r

        let! byA =
            store.Claim
                { Owner = "worker-a"
                  Now = clock.Now()
                  LeaseExpires = clock.Now().AddMinutes 1.0
                  Limit = 5 }

        Assert.Equal(1, byA |> Result.defaultValue [] |> List.length)
        let! blocked = cycle store (writing schema None) "worker-b" clock
        Assert.Equal(0, blocked.Claimed)
        clock.Advance(TimeSpan.FromMinutes 2.0)
        let! _ = cycle store (writing schema None) "worker-b" clock
        let! state = stateOf store id
        Assert.Equal(Completed(Succeeded "ok"), state)

        let stale (_: NpgsqlTransaction) (_: Command) =
            Task.FromResult
                { NewState = Completed(Succeeded "stale")
                  Attempts = 9
                  Outbox = []
                  Reason = "stale" }

        let! late = store.Execute id "worker-a" stale (fun _ _ -> failwith "unused")
        Assert.Equal(Error LeaseNotHeld, late)
    }

[<Fact>]
let ``apply: unknown outcomes refuse replay and accept reconciliation; pause and expiry are durable`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock
        let! r = accept store clock (submission "k" "pay" (Some "chain-1"))
        let id = acceptedId r

        let ambiguous =
            { writing schema None with
                Execute = fun _ -> Task.FromResult(Worker.Ambiguous "rail timeout") }

        let! _ = cycle store ambiguous "w1" clock
        let! state = stateOf store id
        Assert.Equal(OutcomeUnknown "rail timeout", state)
        let! replay = store.Apply tenantA id Lifecycle.Replay "operator" "retry"
        Assert.True(Result.isError replay)
        let! reconciled = store.Apply tenantA id (Lifecycle.ReconciledExecuted(Succeeded "bank confirmed")) "treasury" "trace"
        Assert.Equal(Ok(Completed(Succeeded "bank confirmed")), reconciled)

        let! pausedCmd = accept store clock { submission "late" "x" None with Deadline = Some(t0.AddMinutes 5.0) }
        let! _ = store.SetPause (Tenant tenantA) true
        let! whilePaused = cycle store (writing schema None) "w1" clock
        Assert.Equal(0, whilePaused.Claimed)
        clock.Advance(TimeSpan.FromMinutes 10.0)
        let! _ = cycle store (writing schema None) "w1" clock
        let! expired = stateOf store (acceptedId pausedCmd)
        Assert.Equal(Expired, expired)
    }

[<Fact>]
let ``relay: delivery marks effects; failures stay pending with their key`` () =
    task {
        let clock = Clock()
        let! schema, store = fresh clock
        let! _ = accept store clock (submission "k1" "one" (Some "a"))
        let! _ = accept store clock (submission "k2" "two" (Some "b"))
        let! _ = cycle store (writing schema None) "w1" clock

        let send (e: PendingEffect) =
            Task.FromResult(if e.Message.Payload = "one" then Worker.Delivered else Worker.NotDelivered "503")

        let! delivered = Worker.relay store 10 send
        Assert.Equal(Ok 1, delivered)
        let! pending = store.PendingOutbox 10
        Assert.Equal<string list>([ "two" ], pending |> Result.defaultValue [] |> List.map (fun e -> e.Message.Payload))
    }

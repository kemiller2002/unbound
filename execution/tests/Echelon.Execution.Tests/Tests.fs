module Echelon.Execution.Tests.Tests

open System
open System.Threading.Tasks
open Xunit
open Echelon.Execution
open Echelon.Execution.Tests.Support

/// Run `step` until it returns false (or `limit` steps); returns the number of steps that returned true.
let rec repeatWhile (limit: int) (step: unit -> Task<bool>) : Task<int> =
    task {
        if limit <= 0 then
            return 0
        else
            let! more = step ()
            if more then
                let! rest = repeatWhile (limit - 1) step
                return 1 + rest
            else
                return 0
    }

// ---- Lifecycle (EAR-CMD §2) ------------------------------------------------------------------------------

[<Fact>]
let ``lifecycle: cancellation and expiry are illegal once execution has started`` () =
    let processing = Processing("w", t0)
    Assert.True(Result.isError (Lifecycle.transition processing (Lifecycle.Cancel "late")))
    Assert.True(Result.isError (Lifecycle.transition processing Lifecycle.Expire))
    Assert.Equal(Ok(Cancelled "user"), Lifecycle.transition Accepted (Lifecycle.Cancel "user"))

[<Fact>]
let ``lifecycle: an unknown external outcome cannot be replayed, only reconciled (EAR-CMD-014, -019)`` () =
    let unknown = OutcomeUnknown "trace with rail"
    Assert.True(Result.isError (Lifecycle.transition unknown Lifecycle.Replay))
    Assert.Equal(Ok Accepted, Lifecycle.transition unknown Lifecycle.ReconciledNotExecuted)
    Assert.Equal(Ok(Completed(Succeeded "paid")), Lifecycle.transition unknown (Lifecycle.ReconciledExecuted(Succeeded "paid")))

[<Fact>]
let ``lifecycle: completed is terminal and accepts no event`` () =
    let completed = Completed(Succeeded "ok")

    [ Lifecycle.Claim("w", t0); Lifecycle.Replay; Lifecycle.Cancel "x"; Lifecycle.Expire; Lifecycle.Resume ]
    |> List.iter (fun e -> Assert.True(Result.isError (Lifecycle.transition completed e)))

// ---- Policy ------------------------------------------------------------------------------------------------

[<Fact>]
let ``policy: fingerprint is stable and sensitive to payload, version and operation`` () =
    let fp = Policy.fingerprint grantOp 1 "{\"amount\":100}"
    Assert.Equal(fp, Policy.fingerprint grantOp 1 "{\"amount\":100}")
    Assert.NotEqual<string>(fp, Policy.fingerprint grantOp 1 "{\"amount\":101}")
    Assert.NotEqual<string>(fp, Policy.fingerprint grantOp 2 "{\"amount\":100}")
    Assert.NotEqual<string>(fp, Policy.fingerprint (OperationName "other") 1 "{\"amount\":100}")

[<Fact>]
let ``policy: backoff grows, is capped and stops at the attempt budget (EAR-CMD-012)`` () =
    let p = Policy.defaultRetryPolicy
    let delay n = (Policy.nextAttempt p n t0 0.999).Value - t0
    Assert.True(delay 2 > delay 1)
    Assert.True(delay 4 > delay 3)
    Assert.True(delay 4 <= p.MaxDelay)
    Assert.Equal(None, Policy.nextAttempt p p.MaxAttempts t0 0.5)

// ---- Intake (EAR-API-005/007/008/009, EAR-CMD-001/004) -------------------------------------------------------

[<Fact>]
let ``intake: immediate classes never reach the command store`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now

        let! r = accept store clock (newIds ()) { submission tenantA "k1" "p" None with Class = ImmediateControl }

        Assert.Equal(Error(Intake.NotDurableClass ImmediateControl), r)
        Assert.Empty((handle.Snapshot()).Commands)
    }

[<Fact>]
let ``intake: a receipt is issued only after commit and states acceptance, not completion`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now

        match! accept store clock (newIds ()) (submission tenantA "k1" "p" (Some "fund-1")) with
        | Ok(Intake.NewlyAccepted receipt) ->
            Assert.Equal(AcceptedForProcessing, receipt.Meaning)
            Assert.Equal("Accepted", receipt.State)
            Assert.Equal(t0, receipt.AcceptedAt)
            Assert.True((handle.Snapshot()).Commands.ContainsKey receipt.Command)
        | other -> failwithf "unexpected %A" other
    }

[<Fact>]
let ``intake: a database outage yields 'not accepted, retry with the same key' and stores nothing`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        handle.SetUnavailable true

        let! r = accept store clock (newIds ()) (submission tenantA "k1" "p" None)
        Assert.Equal(Error(Intake.AcceptanceUnavailable(IdempotencyKey "k1")), r)

        handle.SetUnavailable false
        let! retried = accept store clock (newIds ()) (submission tenantA "k1" "p" None)
        Assert.True(Result.isOk retried)
        Assert.Equal(1, (handle.Snapshot()).Commands.Count)
    }

[<Fact>]
let ``intake: same key and payload returns the original receipt; same key with a different payload is a conflict`` () =
    task {
        let clock = Clock()
        let store, _ = InMemoryStore.create clock.Now
        let ids = newIds ()
        let! first = accept store clock ids (submission tenantA "k1" "amount=100" None)
        clock.Advance(TimeSpan.FromSeconds 5.0)
        let! again = accept store clock ids (submission tenantA "k1" "amount=100" None)
        let! changed = accept store clock ids (submission tenantA "k1" "amount=999" None)

        match again with
        | Ok(Intake.Duplicate receipt) ->
            Assert.Equal(acceptedId first, receipt.Command)
            Assert.Equal(t0, receipt.AcceptedAt) // original acceptance time, never the retry time
        | other -> failwithf "unexpected %A" other

        Assert.Equal(Error(Intake.IdempotencyConflict(acceptedId first)), changed)
    }

[<Fact>]
let ``intake: fifty concurrent submissions with one key create exactly one command`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let ids = (fun () -> Guid.NewGuid())

        let! results =
            Array.init 50 (fun _ -> Task.Run<Result<Intake.Response, Intake.Rejection>>(Func<Task<Result<Intake.Response, Intake.Rejection>>>(fun () -> accept store clock ids (submission tenantA "same" "p" None))))
            |> Task.WhenAll

        Assert.Equal(1, (handle.Snapshot()).Commands.Count)
        Assert.Equal(1, results |> Array.filter (function Ok(Intake.NewlyAccepted _) -> true | _ -> false) |> Array.length)
        Assert.Equal(49, results |> Array.filter (function Ok(Intake.Duplicate _) -> true | _ -> false) |> Array.length)
    }

[<Fact>]
let ``intake: per-tenant queue limit throttles without accepting`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let limits = { Intake.defaultLimits with QueuedPerTenant = 2 }
        let ids = newIds ()
        let go key = Intake.accept store limits ids clock.Now (submission tenantA key "p" None)
        let! _ = go "a"
        let! _ = go "b"
        let! third = go "c"
        Assert.Equal(Error(Intake.Throttled limits.RetryAfter), third)
        Assert.Equal(2, (handle.Snapshot()).Commands.Count)
        let! otherTenant = Intake.accept store limits ids clock.Now (submission tenantB "c" "p" None)
        Assert.True(Result.isOk otherTenant)
    }

[<Fact>]
let ``intake: a future not-before produces a scheduled receipt`` () =
    task {
        let clock = Clock()
        let store, _ = InMemoryStore.create clock.Now
        let later = t0.AddDays 3.0
        let! r = accept store clock (newIds ()) { submission tenantA "k" "p" None with NotBefore = Some later }

        match r with
        | Ok(Intake.NewlyAccepted receipt) -> Assert.Equal(ScheduledForProcessing later, receipt.Meaning)
        | other -> failwithf "unexpected %A" other
    }

// ---- Worker (EAR-CMD-006/008/009/011/012/013/014/017/018/020) ---------------------------------------------------

[<Fact>]
let ``worker: completion, business writes and outbox commit together with stable effect keys`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "grant-1" (Some "fund-1"))
        let id = acceptedId r
        let! report = runCycle store (operation succeedWriting) "w1" clock

        Assert.Equal(1, report.Claimed)
        Assert.Equal(Completed(Succeeded "ok"), stateOf handle id)
        let snap = handle.Snapshot()
        Assert.Equal<string list>([ "grant-1" ], snap.BusinessLog)
        Assert.Equal<string list>([ Worker.effectKey id 0 ], snap.Outbox |> Map.toList |> List.map fst)
    }

[<Fact>]
let ``worker: a crashing handler rolls back its writes and schedules a retry; repeated crashes quarantine only that key`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let ids = newIds ()
        let! bad = accept store clock ids (submission tenantA "bad" "poison" (Some "fund-1"))
        let! later = accept store clock ids (submission tenantA "later" "ok" (Some "fund-1"))
        let! other = accept store clock ids (submission tenantA "other" "ok" (Some "fund-2"))

        let op =
            operation (fun ctx ->
                if ctx.Payload = "poison" then
                    ctx.Tx.Writes <- [ "should-not-commit" ] // written inside the transaction, then the handler crashes
                    failwith "boom"
                else
                    succeedWriting ctx)

        let crashCycle () =
            task {
                let! _ = runCycle store op "w1" clock
                clock.Advance(TimeSpan.FromMinutes 10.0)

                match stateOf handle (acceptedId bad) with
                | Quarantined _ -> return false
                | _ -> return true
            }

        let! retried = repeatWhile 10 crashCycle
        Assert.Equal(Policy.defaultRetryPolicy.QuarantineAfterCrashes - 1, retried)
        Assert.DoesNotContain("should-not-commit", (handle.Snapshot()).BusinessLog)
        // fund-2 progressed; fund-1's later command is held behind the quarantined one (ordering preserved)
        Assert.Equal(Completed(Succeeded "ok"), stateOf handle (acceptedId other))
        Assert.Equal(Accepted, stateOf handle (acceptedId later))
    }

[<Fact>]
let ``worker: an ambiguous external effect becomes OutcomeUnknown and is never retried automatically`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "pay" (Some "chain-1"))
        let id = acceptedId r
        let calls = ref 0

        let op =
            operation (fun _ ->
                calls.Value <- calls.Value + 1
                Task.FromResult(Worker.Ambiguous "rail timeout after submission"))

        let! _ = runCycle store op "w1" clock
        clock.Advance(TimeSpan.FromHours 2.0)
        let! second = runCycle store op "w1" clock

        Assert.Equal(1, calls.Value)
        Assert.Equal(0, second.Claimed)
        Assert.Equal(OutcomeUnknown "rail timeout after submission", stateOf handle id)

        let! replay = store.Apply tenantA id Lifecycle.Replay "operator" "retry please"
        Assert.True(Result.isError replay)

        let! reconciled = store.Apply tenantA id Lifecycle.ReconciledNotExecuted "treasury" "bank trace: not received"
        Assert.Equal(Ok Accepted, reconciled)
    }

[<Fact>]
let ``worker: authority revoked while queued completes as a decline with no business effect (EAR-CMD-017)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "grant" (Some "fund-1"))
        let revoked = { operation succeedWriting with Authorize = fun _ -> Task.FromResult(Error "delegation revoked") }
        let! _ = runCycle store revoked "w1" clock

        Assert.Equal(Completed(Declined "NotAuthorized: delegation revoked"), stateOf handle (acceptedId r))
        Assert.Empty((handle.Snapshot()).BusinessLog)
        Assert.Empty((handle.Snapshot()).Outbox)
    }

[<Fact>]
let ``worker: per-key order is acceptance order while different keys progress (EAR-CMD-009)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let ids = newIds ()

        for i in 1..5 do
            for key in [ "fund-1"; "fund-2" ] do
                let! _ = accept store clock ids (submission tenantA (sprintf "%s-%d" key i) (sprintf "%s-%d" key i) (Some key))
                clock.Advance(TimeSpan.FromMilliseconds 1.0)

        let drainCycle () =
            task {
                let! report = runCycle store (operation succeedWriting) "w1" clock
                return report.Claimed > 0
            }

        let! cycles = repeatWhile 20 drainCycle
        let log = (handle.Snapshot()).BusinessLog
        let forKey (key: string) = log |> List.filter (fun (w: string) -> w.StartsWith key)
        Assert.Equal<string list>([ for i in 1..5 -> sprintf "fund-1-%d" i ], forKey "fund-1")
        Assert.Equal<string list>([ for i in 1..5 -> sprintf "fund-2-%d" i ], forKey "fund-2")
        Assert.Equal(5, cycles) // two keys in parallel: five cycles, not ten
    }

[<Fact>]
let ``worker: a crashed worker's lease expires and another worker completes the command once (EAR-CMD-018)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "grant" (Some "fund-1"))
        let id = acceptedId r

        let! claimedByA =
            store.Claim
                { Owner = "worker-a"
                  Now = clock.Now()
                  LeaseExpires = clock.Now().AddMinutes 1.0
                  Limit = 10 }

        Assert.Equal(1, (Result.defaultValue [] claimedByA).Length)
        let! whileLeased = runCycle store (operation succeedWriting) "worker-b" clock
        Assert.Equal(0, whileLeased.Claimed)

        clock.Advance(TimeSpan.FromMinutes 2.0)
        let! _ = runCycle store (operation succeedWriting) "worker-b" clock
        Assert.Equal(Completed(Succeeded "ok"), stateOf handle id)

        // worker A wakes up and tries to finish: refused, nothing written twice
        let lateRun (tx: InMemoryStore.Tx) (_: Command) =
            tx.Writes <- [ "dup" ]
            Task.FromResult { NewState = Completed(Succeeded "dup"); Attempts = 1; Outbox = []; Reason = "late" }

        let! late = store.Execute id "worker-a" lateRun (fun _ _ -> failwith "unused")
        Assert.Equal(Error LeaseNotHeld, late)
        Assert.Equal<string list>([ "grant" ], (handle.Snapshot()).BusinessLog)
    }

[<Fact>]
let ``worker: transient failures retry with backoff and exhaust to Failed`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "x" None)
        let id = acceptedId r
        let op = operation (fun _ -> Task.FromResult(Worker.Transient { Code = "ProviderBusy"; Detail = "503" }))

        for _ in 1 .. Policy.defaultRetryPolicy.MaxAttempts + 1 do
            let! _ = runCycle store op "w1" clock
            clock.Advance(TimeSpan.FromHours 1.0)

        match stateOf handle id with
        | Failed fault -> Assert.Equal("ProviderBusy", fault.Code)
        | other -> failwithf "expected Failed, got %A" other

        Assert.Equal(Policy.defaultRetryPolicy.MaxAttempts, (handle.Snapshot()).Commands.[id].Attempts)
    }

[<Fact>]
let ``worker: older contract versions are upcast; unsupported versions are quarantined (EAR-CMD-005)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let ids = newIds ()
        let! v1 = accept store clock ids (submission tenantA "v1" "amount=1" (Some "a"))
        let! v9 = accept store clock ids { submission tenantA "v9" "amount=1" (Some "b") with Version = 9 }

        let op =
            { operation succeedWriting with
                CurrentVersion = 2
                Upcast =
                    fun v payload ->
                        match v with
                        | 1 -> Ok(payload + ";currency=USD")
                        | 2 -> Ok payload
                        | other -> Error(sprintf "v%d" other) }

        let! _ = runCycle store op "w1" clock
        Assert.Equal<string list>([ "amount=1;currency=USD" ], (handle.Snapshot()).BusinessLog)

        match stateOf handle (acceptedId v9) with
        | Quarantined fault -> Assert.Equal("UnsupportedVersion", fault.Code)
        | other -> failwithf "expected Quarantined, got %A" other

        Assert.Equal(Completed(Succeeded "ok"), stateOf handle (acceptedId v1))
    }

[<Fact>]
let ``worker: one tenant's backlog does not starve another (EAR-CMD-020)`` () =
    task {
        let clock = Clock()
        let store, _ = InMemoryStore.create clock.Now
        let ids = newIds ()

        for i in 1..50 do
            let! _ = accept store clock ids (submission tenantA (sprintf "a%d" i) "x" None)
            ()

        let! _ = accept store clock ids (submission tenantB "b1" "x" None)

        let! claimed =
            store.Claim
                { Owner = "w"
                  Now = clock.Now()
                  LeaseExpires = clock.Now().AddMinutes 1.0
                  Limit = 2 }

        let tenants = claimed |> Result.defaultValue [] |> List.map (fun c -> c.Tenant) |> Set.ofList
        Assert.True(tenants.Contains tenantB)
    }

[<Fact>]
let ``operations: pause holds work durably until resume (EAR-AVL-004)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "x" None)
        let! _ = store.SetPause (Tenant tenantA) true
        let! paused = runCycle store (operation succeedWriting) "w" clock
        Assert.Equal(0, paused.Claimed)
        Assert.Equal(Accepted, stateOf handle (acceptedId r))
        let! _ = store.SetPause (Tenant tenantA) false
        let! resumed = runCycle store (operation succeedWriting) "w" clock
        Assert.Equal(1, resumed.Claimed)
    }

[<Fact>]
let ``operations: unstarted commands past their deadline expire (EAR-CMD-016)`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) { submission tenantA "k" "x" None with Deadline = Some(t0.AddMinutes 5.0) }
        let! _ = store.SetPause Everything true
        clock.Advance(TimeSpan.FromMinutes 10.0)
        let! _ = runCycle store (operation succeedWriting) "w" clock
        Assert.Equal(Expired, stateOf handle (acceptedId r))
    }

[<Fact>]
let ``operations: lifecycle history reconstructs every transition (EAR-CMD-021)`` () =
    task {
        let clock = Clock()
        let store, _ = InMemoryStore.create clock.Now
        let! r = accept store clock (newIds ()) (submission tenantA "k" "x" None)
        let! _ = runCycle store (operation succeedWriting) "w" clock
        let! history = store.History tenantA (acceptedId r)
        let path = history |> Result.defaultValue [] |> List.map (fun h -> h.To)
        Assert.Equal<string list>([ "Accepted"; "Processing"; "Completed" ], path)
        let! hidden = store.History tenantB (acceptedId r)
        Assert.Equal(Ok [], hidden) // another tenant sees nothing (EAR-CMD-003)
    }

[<Fact>]
let ``relay: delivered effects leave the outbox; undelivered ones remain for redelivery with the same key`` () =
    task {
        let clock = Clock()
        let store, handle = InMemoryStore.create clock.Now
        let ids = newIds ()
        let! _ = accept store clock ids (submission tenantA "k1" "one" (Some "a"))
        let! _ = accept store clock ids (submission tenantA "k2" "two" (Some "b"))
        let! _ = runCycle store (operation succeedWriting) "w" clock

        let send (e: PendingEffect) =
            Task.FromResult(if e.Message.Payload = "one" then Worker.Delivered else Worker.NotDelivered "503")

        let! delivered = Worker.relay store 10 send
        Assert.Equal(Ok 1, delivered)
        let! pending = store.PendingOutbox 10
        Assert.Equal<string list>([ "two" ], pending |> Result.defaultValue [] |> List.map (fun e -> e.Message.Payload))
        Assert.Equal(1, (handle.Snapshot()).Outbox.Count - 1)
    }

module Echelon.Execution.Tests.Support

open System
open System.Threading.Tasks
open Echelon.Execution

let t0 = DateTimeOffset(2026, 12, 31, 23, 58, 0, TimeSpan.Zero)

/// A test clock: the only mutable value in the tests, standing in for wall time.
type Clock() =
    let mutable now = t0
    member _.Now() = now
    member _.Advance(span: TimeSpan) = now <- now.Add span

let tenantA = TenantId "sponsor-a"
let tenantB = TenantId "sponsor-b"
let donor = PrincipalId "donor-1"
let grantOp = OperationName "grant.recommend"

let submission tenant key payload orderingKey : Intake.Submission =
    { Tenant = tenant
      Principal = donor
      Operation = grantOp
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

let settings owner =
    { Worker.Owner = owner
      Worker.LeaseDuration = TimeSpan.FromMinutes 1.0
      Worker.Retry = Policy.defaultRetryPolicy
      Worker.BatchSize = 10 }

let operation (execute: Worker.ExecutionContext<InMemoryStore.Tx> -> Task<Worker.HandlerResult>) : Worker.Operation<InMemoryStore.Tx> =
    { Name = grantOp
      CurrentVersion = 1
      Upcast = fun v payload -> if v = 1 then Ok payload else Error(sprintf "version %d unsupported" v)
      Authorize = fun _ -> Task.FromResult(Ok())
      Execute = execute }

let ops (op: Worker.Operation<InMemoryStore.Tx>) = Map.ofList [ op.Name, op ]

let succeedWriting (ctx: Worker.ExecutionContext<InMemoryStore.Tx>) =
    ctx.Tx.Writes <- ctx.Tx.Writes @ [ ctx.Payload ]
    Task.FromResult(Worker.Done(Succeeded "ok", [ "payments.instruction", ctx.Payload ]))

let newIds () =
    let mutable n = 0

    fun () ->
        n <- n + 1
        Guid(n, 0s, 0s, Array.zeroCreate 8)

let accept store (clock: Clock) ids s =
    Intake.accept store Intake.defaultLimits ids clock.Now s

let acceptedId (r: Result<Intake.Response, Intake.Rejection>) =
    match r with
    | Ok(Intake.NewlyAccepted receipt)
    | Ok(Intake.Duplicate receipt) -> receipt.Command
    | Error e -> failwithf "expected acceptance, got %A" e

let stateOf (handle: InMemoryStore.Handle) id = (handle.Snapshot()).Commands.[id].State

let runCycle store op owner (clock: Clock) =
    Worker.cycle store (ops op) (settings owner) clock.Now (fun () -> 0.5)

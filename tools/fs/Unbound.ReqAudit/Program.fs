/// Entry point: one subcommand per former Python script, taking the same positional arguments.
module Unbound.ReqAudit.Program

open Unbound.ReqAudit.PyCompat

let private commands : (string * (string list -> int)) list =
    [ "graph", Graph.main
      "validate", Validate.main
      "validate-discovery", ValidateDiscovery.main
      "validate-simplification", ValidateSimplification.main
      "render", Render.main
      "render-discovery", RenderDiscovery.main
      "assemble-simplification", Assemble.main
      "inventory", Inventory.main
      "quality", Quality.main
      "narrative", Narrative.main
      "workflows", Workflows.main
      "ia", Ia.main
      "uiclass", UiClass.main
      "adopt", Adopt.main
      "benchmark-analyse", Benchmark.main ]

/// A Python exception reaching the top prints a traceback and exits 1. The traceback text itself is not reproduced.
let private crash (msg: string) =
    Io.flush ()
    Io.eprint ("Traceback (most recent call last):\n" + msg)
    1

[<EntryPoint>]
let main argv =
    let code =
        match List.ofArray argv with
        | cmd :: rest when commands |> List.exists (fst >> (=) cmd) ->
            try
                (commands |> List.find (fst >> (=) cmd) |> snd) rest
            with
            | PyError msg -> crash msg
            | e -> crash (string e)
        | _ ->
            Io.eprint ("usage: Unbound.ReqAudit <" + (commands |> List.map fst |> String.concat "|") + "> args...")
            2
    Io.flush ()
    code

/// Entry point: one subcommand per former authoring script (tools/reqaudit/authoring/*.py), each taking the repository
/// root in place of the absolute paths the scripts hard-coded.
module Unbound.Authoring.Program

open Unbound.Authoring.PyCompat

let private commands : (string * (Io.Paths -> int)) list =
    [ "ui-quality", UiQuality.main
      "merge-specs", MergeSpecs.main
      "ui-trace", UiTrace.main ]

/// A Python exception reaching the top prints a traceback and exits 1. The traceback text itself is not reproduced.
let private crash (msg: string) =
    Io.flush ()
    Io.eprint ("Traceback (most recent call last):\n" + msg)
    1

[<EntryPoint>]
let main argv =
    let code =
        match List.ofArray argv with
        | [ cmd; repo ] when commands |> List.exists (fst >> (=) cmd) ->
            try
                (commands |> List.find (fst >> (=) cmd) |> snd) (Io.paths repo)
            with
            | PyError msg -> crash msg
            | e -> crash (string e)
        | _ ->
            Io.eprint ("usage: Unbound.Authoring <" + (commands |> List.map fst |> String.concat "|") + "> <repo>")
            2
    Io.flush ()
    code

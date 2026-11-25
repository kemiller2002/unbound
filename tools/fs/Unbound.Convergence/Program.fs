/// Command-line edge of the convergence pipeline: one subcommand per former Python script, same positional arguments.
module Unbound.Convergence.Program

open System
open System.IO
open System.Text
open Unbound.Convergence.PyCompat

let commands : (string * (Io.Console -> string list -> int)) list =
    [ "render-questions", RenderQuestions.main
      "review-map", ReviewMap.main
      "integrate-dossiers", IntegrateDossiers.main
      "adopt-convergence", AdoptConvergence.main
      "render-decision-register", RenderDecisionRegister.main
      "render-adversarial", RenderAdversarial.main
      "render-ratification-packets", RenderRatificationPackets.main ]

/// Runs a command with Python's top-level behaviour: an uncaught exception prints "<Type>: <message>" (the last line
/// of a Python traceback) and exits 1.
let run (io: Io.Console) (command: Io.Console -> string list -> int) (argv: string list) : int =
    try
        command io argv
    with
    | PyError(kind, msg) ->
        io.Err($"{kind}: {msg}\n")
        1
    | :? ArgumentException
    | :? IndexOutOfRangeException ->
        io.Err("IndexError: list index out of range\n")
        1
    | :? Collections.Generic.KeyNotFoundException as e ->
        io.Err($"KeyError: {e.Message}\n")
        1
    | :? FileNotFoundException
    | :? DirectoryNotFoundException as e ->
        io.Err($"FileNotFoundError: {e.Message}\n")
        1
    | e ->
        io.Err($"{e.GetType().Name}: {e.Message}\n")
        1

[<EntryPoint>]
let main argv =
    let utf8 = UTF8Encoding(false)
    use out = new StreamWriter(Console.OpenStandardOutput(), utf8)
    use err = new StreamWriter(Console.OpenStandardError(), utf8)
    let io: Io.Console = { Out = out.Write; Err = err.Write }
    match List.ofArray argv with
    | name :: rest when commands |> List.exists (fst >> (=) name) -> run io (commands |> List.find (fst >> (=) name) |> snd) (name :: rest)
    | _ ->
        io.Err("usage: Unbound.Convergence <" + (commands |> List.map fst |> String.concat "|") + "> <args>\n")
        2

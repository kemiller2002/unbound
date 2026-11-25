/// Pure delimited-text parsing.
module Unbound.Replay.Csv

open System

/// Splits one line of RFC 4180 text (no embedded newlines) on `sep`, honouring double quotes and "" escapes.
let splitQuoted (sep: char) (line: string) : string list =
    let finish (field: char list) = String(field |> List.rev |> Array.ofList)

    let rec go i inQuotes field acc =
        if i >= line.Length then
            List.rev (finish field :: acc)
        else
            let c = line.[i]

            match inQuotes, c with
            | true, '"' when i + 1 < line.Length && line.[i + 1] = '"' -> go (i + 2) true ('"' :: field) acc
            | true, '"' -> go (i + 1) false field acc
            | true, _ -> go (i + 1) true (c :: field) acc
            | false, '"' -> go (i + 1) true field acc
            | false, _ when c = sep -> go (i + 1) false [] (finish field :: acc)
            | false, _ -> go (i + 1) false (c :: field) acc

    go 0 false [] []

/// Fast path: plain split when the line holds no quote character.
let split (sep: char) (line: string) : string list =
    if line.IndexOf '"' < 0 then
        line.Split sep |> List.ofArray
    else
        splitQuoted sep line

/// Trims a field and maps OFAC's "-0-" null marker and blanks to None.
let field (s: string) : string option =
    match s.Trim() with
    | ""
    | "-0-" -> None
    | t -> Some t

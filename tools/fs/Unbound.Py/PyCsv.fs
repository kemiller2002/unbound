/// Python csv module compatibility (default "excel" dialect): reader with quotes, doubled quotes and embedded newlines;
/// writer with QUOTE_MINIMAL or QUOTE_ALL and the "\r\n" line terminator.
module Unbound.Py.PyCsv

open System
open System.Text

/// Python's universal-newline text mode: "\r\n" and lone "\r" become "\n". Files opened with open(p) or
/// p.read_text() pass through this before csv sees them; files opened with newline="" do not.
let universalNewlines (text: string) : string = text.Replace("\r\n", "\n").Replace("\r", "\n")

/// Parses CSV text into rows of fields (csv.reader). A trailing newline does not produce an empty row.
let parse (text: string) : string list list =
    let finish (field: StringBuilder) = field.ToString()
    // State: position, in-quotes flag, current field, current row (reversed), rows (reversed), whether the row has content.
    let rec go i inQ (field: StringBuilder) (row: string list) (rows: string list list) (started: bool) =
        if i >= text.Length then
            if started || field.Length > 0 || not row.IsEmpty then List.rev (List.rev (finish field :: row) :: rows)
            else List.rev rows
        else
            let c = text.[i]
            if inQ then
                if c = '"' && i + 1 < text.Length && text.[i + 1] = '"' then go (i + 2) true (field.Append('"')) row rows true
                elif c = '"' then go (i + 1) false field row rows true
                else go (i + 1) true (field.Append(c)) row rows true
            else
                match c with
                | '"' when field.Length = 0 -> go (i + 1) true field row rows true
                | ',' -> go (i + 1) false (StringBuilder()) (finish field :: row) rows true
                | '\r' when i + 1 < text.Length && text.[i + 1] = '\n' ->
                    go (i + 2) false (StringBuilder()) [] (List.rev (finish field :: row) :: rows) false
                | '\n'
                | '\r' -> go (i + 1) false (StringBuilder()) [] (List.rev (finish field :: row) :: rows) false
                | _ -> go (i + 1) false (field.Append(c)) row rows true
    go 0 false (StringBuilder()) [] [] false

/// csv.DictReader: the first row is the header; short rows are padded with None (here: absent keys map to None);
/// extra fields are dropped. Rows are returned as ordered (header, value) lists; blank rows are skipped.
let parseDicts (text: string) : (string * string option) list list =
    match parse text |> List.filter (fun r -> r <> [] && r <> [ "" ]) with
    | [] -> []
    | header :: rows ->
        rows
        |> List.map (fun r ->
            header |> List.mapi (fun i h -> h, (if i < r.Length then Some r.[i] else None)))

type Quoting =
    | QuoteMinimal
    | QuoteAll

let private quote (s: string) = "\"" + s.Replace("\"", "\"\"") + "\""

/// One row as csv.writer writes it, including the "\r\n" terminator. Fields are already Python str() values.
let formatRow (q: Quoting) (fields: string list) : string =
    let cell (s: string) =
        match q with
        | QuoteAll -> quote s
        | QuoteMinimal when s.IndexOfAny [| ','; '"'; '\r'; '\n' |] >= 0 -> quote s
        | QuoteMinimal -> s
    let line =
        match q, fields with
        | QuoteMinimal, [ "" ] -> "\"\""
        | _ -> fields |> List.map cell |> String.concat ","
    line + "\r\n"

let format (q: Quoting) (rows: string list list) : string = rows |> List.map (formatRow q) |> String.concat ""

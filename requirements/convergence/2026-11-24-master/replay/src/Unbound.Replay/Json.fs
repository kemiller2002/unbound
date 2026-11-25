/// Minimal deterministic JSON: an immutable value type and a pure renderer (keys keep the order given).
module Unbound.Replay.Json

open System
open System.Globalization
open System.Text

type Json =
    | JNull
    | JBool of bool
    | JInt of int64
    | JNum of float
    | JDec of decimal
    | JStr of string
    | JArr of Json list
    | JObj of (string * Json) list

let private escape (s: string) =
    s
    |> Seq.map (fun c ->
        match c with
        | '"' -> "\\\""
        | '\\' -> "\\\\"
        | '\n' -> "\\n"
        | '\r' -> "\\r"
        | '\t' -> "\\t"
        | c when c < ' ' -> sprintf "\\u%04x" (int c)
        | c -> string c)
    |> String.concat ""

let private num (x: float) =
    Math.Round(x, 4, MidpointRounding.AwayFromZero).ToString("0.####", CultureInfo.InvariantCulture)

let rec private renderAt (indent: int) (j: Json) : string =
    let pad n = String(' ', n)

    let block (opening: string) (closing: string) (items: string list) =
        match items with
        | [] -> opening + closing
        | _ -> opening + "\n" + (items |> List.map (fun i -> pad (indent + 2) + i) |> String.concat ",\n") + "\n" + pad indent + closing

    match j with
    | JNull -> "null"
    | JBool b -> if b then "true" else "false"
    | JInt i -> i.ToString CultureInfo.InvariantCulture
    | JNum x -> num x
    | JDec d -> d.ToString("0.##", CultureInfo.InvariantCulture)
    | JStr s -> "\"" + escape s + "\""
    | JArr xs -> xs |> List.map (renderAt (indent + 2)) |> block "[" "]"
    | JObj kvs -> kvs |> List.map (fun (k, v) -> "\"" + escape k + "\": " + renderAt (indent + 2) v) |> block "{" "}"

let render (j: Json) : string = renderAt 0 j + "\n"

let opt (f: 'a -> Json) (o: 'a option) = o |> Option.map f |> Option.defaultValue JNull

/// Minimal immutable JSON view over System.Text.Json (parse once, then pure accessors).
module Unbound.CompetitiveDecisions.Json

open System.Text.Json

type J =
    | JNull
    | JBool of bool
    | JNum of decimal
    | JStr of string
    | JArr of J list
    | JObj of (string * J) list

let rec ofElement (e: JsonElement) : J =
    match e.ValueKind with
    | JsonValueKind.Null
    | JsonValueKind.Undefined -> JNull
    | JsonValueKind.True -> JBool true
    | JsonValueKind.False -> JBool false
    | JsonValueKind.Number -> JNum(e.GetDecimal())
    | JsonValueKind.String -> JStr(e.GetString())
    | JsonValueKind.Array -> JArr [ for x in e.EnumerateArray() -> ofElement x ]
    | _ -> JObj [ for p in e.EnumerateObject() -> p.Name, ofElement p.Value ]

let parse (text: string) : J =
    use d = JsonDocument.Parse(text)
    ofElement d.RootElement

let parseLines (text: string) : J list =
    text.Split('\n') |> Array.filter (fun l -> l.Trim() <> "") |> Array.map parse |> List.ofArray

let field (name: string) (j: J) : J =
    match j with
    | JObj ps -> ps |> List.tryFind (fst >> (=) name) |> Option.map snd |> Option.defaultValue JNull
    | _ -> JNull

let has (name: string) (j: J) : bool =
    match j with
    | JObj ps -> ps |> List.exists (fst >> (=) name)
    | _ -> false

let str (j: J) : string =
    match j with
    | JStr s -> s
    | JNum n -> string n
    | JBool b -> (if b then "true" else "false")
    | _ -> ""

let strs (j: J) : string list =
    match j with
    | JArr xs -> xs |> List.map str
    | JStr s when s <> "" -> [ s ]
    | _ -> []

let items (j: J) : J list =
    match j with
    | JArr xs -> xs
    | _ -> []

let fs (name: string) (j: J) = str (field name j)

/// A field counts as present when it is not null, not an empty string and not an empty array/object.
let nonEmpty (j: J) : bool =
    match j with
    | JNull -> false
    | JStr s -> s.Trim() <> ""
    | JArr xs -> not xs.IsEmpty
    | JObj ps -> not ps.IsEmpty
    | _ -> true

/// Every expectation here was produced by CPython 3.13 (golden.json); the F# library must reproduce it exactly.
module Unbound.Py.Tests.GoldenTests

open System
open System.IO
open System.Numerics
open Xunit
open Unbound.Py
open Unbound.Py.PyJson

let private golden = lazy (loads (File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "golden.json"))))
let private g name = (field name golden.Value).Value
let private strs name = g name |> list |> List.map (function JStr s -> s | JNull -> null | j -> failwith $"{j}")

let private floats =
    lazy (g "float_bits" |> list |> List.map (function JInt b -> BitConverter.Int64BitsToDouble(int64 (uint64 b)) | j -> failwith $"{j}"))

let private check (expected: string list) (actual: string list) =
    List.zip expected actual |> List.iteri (fun i (e, a) -> if e <> null then Assert.True((e = a), $"case {i}: expected {e}, got {a}"))

[<Fact>]
let ``repr matches CPython`` () = check (strs "repr") (floats.Value |> List.map PyFloat.repr)

[<Fact>]
let ``fixed formats match CPython`` () =
    check (strs "f0") (floats.Value |> List.map (PyFloat.fixedPoint 0))
    check (strs "f1") (floats.Value |> List.map (PyFloat.fixedPoint 1))
    check (strs "f2") (floats.Value |> List.map (PyFloat.fixedPoint 2))
    check (strs "plus_comma_f0") (floats.Value |> List.map (PyFloat.fixedWith true true 0))
    check (strs "comma_f0") (floats.Value |> List.map (PyFloat.fixedWith false true 0))

[<Fact>]
let ``round matches CPython`` () =
    check (strs "round2") (floats.Value |> List.map (PyFloat.roundTo 2 >> PyFloat.repr))
    check (strs "round0int") (floats.Value |> List.map (fun x -> if abs x < 1e300 then (PyFloat.roundInt x).ToString() else null))

[<Fact>]
let ``sum uses CPython compensated summation`` () =
    g "sums" |> list |> List.iteri (fun i case ->
        let items = (field "items_bits" case).Value |> list |> List.map (function JInt b -> BitConverter.Int64BitsToDouble(int64 (uint64 b)) | j -> failwith $"{j}")
        let actual = if items.IsEmpty then "0" else PyFloat.repr (PySum.sum items)
        Assert.True((str (field "sum" case).Value = actual), $"sum case {i}"))

[<Fact>]
let ``Mersenne Twister, gauss and triangular match CPython`` () =
    match g "mt" with
    | JObj seeds ->
        seeds |> List.iter (fun (seedText, case) ->
            let s0 = PyRandom.seed (BigInteger.Parse seedText)
            let draw f n s = List.mapFold (fun st _ -> f st) s [ 1..n ]
            let rs, s1 = draw PyRandom.random 700 s0
            let gs, s2 = draw (PyRandom.gauss 10.0 2.0) 9 s1
            let ts, s3 = draw (PyRandom.triangular 1.0 5.0 (Some 2.0)) 5 s2
            let t6, s4 = PyRandom.triangular 3.0 3.0 (Some 3.0) s3
            let t7, s5 = PyRandom.triangular 0.0 1.0 None s4
            let after, _ = PyRandom.random s5
            let nums name = (field name case).Value |> list |> List.map num
            Assert.Equal<float list>(nums "random", rs)
            Assert.Equal<float list>(nums "gauss", gs)
            Assert.Equal<float list>(nums "triangular", ts @ [ t6; t7 ])
            Assert.Equal(num (field "after" case).Value, after))
    | j -> failwith $"{j}"

[<Fact>]
let ``crc32 matches zlib`` () =
    match g "crc32" with
    | JObj kvs -> kvs |> List.iter (fun (s, v) -> Assert.Equal(uint32 (num v), Crc32.ofString s))
    | j -> failwith $"{j}"

[<Fact>]
let ``json dumps matches CPython for every option set used`` () =
    let input = loads (str (g "dumps_input"))
    let expect name = (field name (g "dumps")).Value |> str
    Assert.Equal(expect "default", dumps input)
    Assert.Equal(expect "indent2", dumpsWith { defaults with Indent = Some 2 } input)
    Assert.Equal(expect "ascii_false", dumpsWith { defaults with EnsureAscii = false } input)
    Assert.Equal(expect "ascii_false_sorted", dumpsWith { defaults with EnsureAscii = false; SortKeys = true } input)
    Assert.Equal(expect "indent1_sorted", dumpsWith { defaults with Indent = Some 1; SortKeys = true } input)
    Assert.Equal(expect "sorted", dumpsWith { defaults with SortKeys = true } input)

[<Fact>]
let ``csv writer matches CPython`` () =
    let rows = g "csv_rows" |> list |> List.map (list >> List.map str)
    Assert.Equal(str (g "csv_minimal"), PyCsv.format PyCsv.QuoteMinimal rows)
    Assert.Equal(str (g "csv_all"), PyCsv.format PyCsv.QuoteAll rows)

[<Fact>]
let ``csv DictReader matches CPython with and without universal newlines`` () =
    let expected name =
        g name |> list |> List.map (function JObj kvs -> kvs |> List.map (fun (k, v) -> k, (match v with JNull -> None | v -> Some(str v))) | j -> failwith $"{j}")
    let text = str (g "csv_text")
    Assert.Equal<(string * string option) list list>(expected "csv_dict_rows", PyCsv.parseDicts text)
    Assert.Equal<(string * string option) list list>(expected "csv_dict_rows_universal", PyCsv.parseDicts (PyCsv.universalNewlines text))

[<Fact>]
let ``libm functions agree with CPython`` () =
    let m = g "math"
    let get k = (field k m).Value |> list |> List.map str
    check (get "log") ([ 0.5; 2.0; 10.0; 1e-300; 123.456 ] |> List.map (Math.Log >> PyFloat.repr))
    check (get "exp") ([ -700.0; -1.0; 0.5; 3.0; 700.0 ] |> List.map (Math.Exp >> PyFloat.repr))
    check (get "sqrt") ([ 2.0; 3.0; 1e-300; 123.456 ] |> List.map (Math.Sqrt >> PyFloat.repr))
    check (get "cos") ([ 0.1; 1.0; 3.0; 6.2; 100.0 ] |> List.map (Math.Cos >> PyFloat.repr))
    check (get "sin") ([ 0.1; 1.0; 3.0; 6.2; 100.0 ] |> List.map (Math.Sin >> PyFloat.repr))

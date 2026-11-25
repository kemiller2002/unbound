/// Python numeric semantics the economics models depend on, on top of Unbound.Py.
///
/// The Python models mix int and float freely and the distinction is visible in their JSON output (100 vs 100.0), so
/// every quantity that can come from parameters.json is a Num: a float value plus a flag saying whether Python would hold
/// it as an int. Int values are kept exactly in the float (the operations fail loudly beyond 2^53, where a float could no
/// longer hold the Python int exactly), and arithmetic follows Python's rules: int op int stays int for + - *, true
/// division always gives a float, and anything involving a float is a float computed on the same IEEE values.
module Unbound.Economics.PyCompat

open System
open System.Globalization
open System.Numerics
open Unbound.Py
open Unbound.Py.PyJson

let private maxExact = 9007199254740992.0 // 2^53

[<Struct; CustomEquality; CustomComparison>]
type Num =
    { V: float
      IsInt: bool }

    /// Python int (exact).
    static member Int(v: float) : Num =
        if abs v > maxExact || Math.Floor v <> v then failwith $"int value {v} is outside the exactly representable range"
        { V = v; IsInt = true }

    /// Python float.
    static member Float(v: float) : Num = { V = v; IsInt = false }

    static member private Arith (a: Num) (b: Num) (op: float -> float -> float) : Num =
        if a.IsInt && b.IsInt then Num.Int(op a.V b.V) else Num.Float(op a.V b.V)

    static member (+)(a: Num, b: Num) = Num.Arith a b (+)
    static member (-)(a: Num, b: Num) = Num.Arith a b (-)
    static member (*)(a: Num, b: Num) = Num.Arith a b (*)

    /// Python true division (`/`): always a float; ZeroDivisionError is raised as an exception.
    static member (/)(a: Num, b: Num) =
        if b.V = 0.0 then raise (DivideByZeroException "division by zero")
        Num.Float(a.V / b.V)

    static member (~-)(a: Num) = if a.IsInt then Num.Int(-a.V) else Num.Float(-a.V)

    // Mixed operands: a bare float literal is a Python float, a bare int literal is a Python int.
    static member (+)(a: Num, b: float) = a + Num.Float b
    static member (+)(a: float, b: Num) = Num.Float a + b
    static member (+)(a: Num, b: int) = a + Num.Int(float b)
    static member (+)(a: int, b: Num) = Num.Int(float a) + b
    static member (-)(a: Num, b: float) = a - Num.Float b
    static member (-)(a: float, b: Num) = Num.Float a - b
    static member (-)(a: Num, b: int) = a - Num.Int(float b)
    static member (-)(a: int, b: Num) = Num.Int(float a) - b
    static member (*)(a: Num, b: float) = a * Num.Float b
    static member (*)(a: float, b: Num) = Num.Float a * b
    static member (*)(a: Num, b: int) = a * Num.Int(float b)
    static member (*)(a: int, b: Num) = Num.Int(float a) * b
    static member (/)(a: Num, b: float) = a / Num.Float b
    static member (/)(a: float, b: Num) = Num.Float a / b
    static member (/)(a: Num, b: int) = a / Num.Int(float b)
    static member (/)(a: int, b: Num) = Num.Int(float a) / b

    /// Python compares int and float by exact value, which is the float value here.
    override this.Equals(o: obj) =
        match o with
        | :? Num as n -> this.V = n.V
        | _ -> false

    override this.GetHashCode() = this.V.GetHashCode()

    interface IComparable with
        member this.CompareTo(o: obj) =
            match o with
            | :? Num as n -> compare this.V n.V
            | _ -> invalidArg "o" "not a Num"

let I (n: int) : Num = Num.Int(float n)
let F (x: float) : Num = Num.Float x

module Num =
    let ofInt64 (n: int64) : Num = Num.Int(float n)

    /// Python truthiness of a number.
    let truthy (n: Num) = n.V <> 0.0

    /// Python min(a, b): the first argument unless the second is strictly smaller.
    let min (a: Num) (b: Num) : Num = if b.V < a.V then b else a

    /// Python max(a, b): the first argument unless the second is strictly greater.
    let max (a: Num) (b: Num) : Num = if b.V > a.V then b else a

    /// Python max(iterable): the first maximal element.
    let maxOf (xs: Num seq) : Num = Seq.reduce max xs

    let abs (a: Num) : Num = if a.IsInt then Num.Int(Math.Abs a.V) else Num.Float(Math.Abs a.V)

    /// math.ceil / math.floor / int(): Python ints.
    let ceil (a: Num) : Num = Num.Int(Math.Ceiling a.V)
    let floor (a: Num) : Num = Num.Int(Math.Floor a.V)
    let toInt (a: Num) : Num = Num.Int(Math.Truncate a.V)

    /// float(x).
    let toFloat (a: Num) : Num = Num.Float a.V

    /// round(x) with no ndigits: int, ties to even.
    let roundInt (a: Num) : Num = if a.IsInt then a else Num.Int(float (PyFloat.roundInt a.V))

    /// round(x, n): an int stays the same int; a float is correctly rounded (half-even on the exact binary value).
    let round (n: int) (a: Num) : Num = if a.IsInt then a else Num.Float(PyFloat.roundTo n a.V)

    /// x ** y for floats (C pow, as CPython's float_pow calls).
    let pow (a: Num) (b: Num) : Num =
        if a.IsInt && b.IsInt then failwith "int ** int is not used by the models"
        Num.Float(Math.Pow(a.V, b.V))

    /// str(x) / f"{x}".
    let str (a: Num) : string =
        if a.IsInt then (BigInteger a.V).ToString(CultureInfo.InvariantCulture) else PyFloat.repr a.V

    let toJson (a: Num) : Json = if a.IsInt then JInt(BigInteger a.V) else JFloat a.V

    let ofJson (j: Json) : Num =
        match j with
        | JInt i ->
            if BigInteger.Abs i > BigInteger maxExact then failwith $"int {i} is outside the supported range"
            Num.Int(float i)
        | JFloat x -> Num.Float x
        | other -> failwith $"expected a number, got {other}"

    /// Python 3.12/3.13 builtin sum(iterable) with start 0: exact int accumulation while items are ints; once a float
    /// appears, Neumaier-compensated float accumulation in which later int items are added uncompensated (as CPython
    /// does); the empty sum is the int 0.
    let sum (xs: Num seq) : Num =
        use e = xs.GetEnumerator()
        let rec floats (f: float) (c: float) =
            if e.MoveNext() then
                let x = e.Current
                if x.IsInt then floats (f + x.V) c
                else
                    let t = f + x.V
                    let c' = if Math.Abs f >= Math.Abs x.V then c + ((f - t) + x.V) else c + ((x.V - t) + f)
                    floats t c'
            else if c <> 0.0 && Double.IsFinite c then Num.Float(f + c)
            else Num.Float f
        let rec ints (acc: float) =
            if e.MoveNext() then
                let x = e.Current
                if x.IsInt then ints (Num.Int(acc + x.V)).V
                else floats (acc + x.V) 0.0
            else Num.Int acc
        ints 0.0

    let sumBy (f: 'a -> Num) (xs: 'a seq) : Num = xs |> Seq.map f |> sum

/// Python sum() over values that are always floats (the result of the float path above).
let fsum (xs: float seq) : float = (xs |> Seq.map Num.Float |> Num.sum).V

/// sorted(xs, key=f) / list.sort(key=f) for numeric keys: stable, ascending.
let sortByNum (f: 'a -> Num) (xs: 'a list) : 'a list = xs |> List.sortWith (fun a b -> compare (f a).V (f b).V)

/// random.Random.triangular(low, high, mode) with Python's int/float typing: when high == low it returns low itself.
let triangular (low: Num) (high: Num) (mode: Num) (rng: PyRandom.State) : Num * PyRandom.State =
    let u, rng' = PyRandom.random rng
    let span = high - low
    if span.V = 0.0 then low, rng'
    else
        let c = (mode - low) / span
        let u, c, low, high = if u > c.V then 1.0 - u, 1.0 - c, high, low else u, c, low, high
        low + (high - low) * Math.Sqrt(u * c.V), rng'

/// List.init over independent pure computations (one seeded Monte Carlo run each), evaluated in parallel; the result
/// order is the index order, so output is identical to a sequential evaluation.
let parallelInit (n: int) (f: int -> 'a) : 'a list = Array.init n id |> Array.Parallel.map f |> List.ofArray

/// random.Random(n) for a Python int seed.
let seeded (n: BigInteger) : PyRandom.State = PyRandom.seed n

// ---- Json access (Python dict/list reads on parsed JSON) ----

/// d[key]; a missing key raises like Python's KeyError.
let (?) (j: Json) (key: string) : Json =
    match PyJson.field key j with
    | Some v -> v
    | None -> raise (Collections.Generic.KeyNotFoundException $"KeyError: '{key}'")

/// d.get(key) (None when missing; JSON null also reads as None).
let tryKey (key: string) (j: Json) : Json option =
    match PyJson.field key j with
    | Some JNull
    | None -> None
    | some -> some

/// d.items() of a JSON object, in insertion order.
let items (j: Json) : (string * Json) list =
    match j with
    | JObj kvs -> kvs
    | other -> failwith $"expected an object, got {other}"

let keys (j: Json) : string list = items j |> List.map fst

let num (j: Json) : Num = Num.ofJson j
let text (j: Json) : string = PyJson.str j

/// Python truthiness of a JSON value.
let truthy (j: Json) : bool =
    match j with
    | JNull -> false
    | JBool b -> b
    | JInt i -> not i.IsZero
    | JFloat x -> x <> 0.0
    | JStr s -> s <> ""
    | JArr xs -> not xs.IsEmpty
    | JObj kvs -> not kvs.IsEmpty

/// dict(d, k=v): replaces k in place, or appends it.
let withKey (key: string) (value: Json) (j: Json) : Json =
    let kvs = items j
    if kvs |> List.exists (fst >> (=) key) then JObj(kvs |> List.map (fun (k, v) -> if k = key then k, value else k, v))
    else JObj(kvs @ [ key, value ])

/// dict(a, **b).
let merge (b: (string * Json) list) (a: Json) : Json = b |> List.fold (fun acc (k, v) -> withKey k v acc) a

// ---- Json construction ----

let jo (kvs: (string * Json) list) : Json = JObj kvs
let js (s: string) : Json = JStr s
let jn (n: Num) : Json = Num.toJson n
let ji (n: int) : Json = JInt(BigInteger n)
let jf (x: float) : Json = JFloat x
let jl (xs: Json list) : Json = JArr xs
let jopt (f: 'a -> Json) (x: 'a option) : Json = x |> Option.map f |> Option.defaultValue JNull

/// json.dump(obj, fh, indent=1, sort_keys=True) (no trailing newline).
let dumpSorted (j: Json) : string = PyJson.dumpsWith { PyJson.defaults with Indent = Some 1; SortKeys = true } j

// ---- text ----

/// csv.writer row with QUOTE_MINIMAL and the given line terminator.
let csvRow (terminator: string) (fields: string list) : string =
    let row = PyCsv.formatRow PyCsv.QuoteMinimal fields
    row.Substring(0, row.Length - 2) + terminator

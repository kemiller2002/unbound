/// CPython random.Random: MT19937 seeded from an int, random(), gauss() and triangular(), as a pure state-passing API.
/// The 624-word state array is never mutated after construction: each twist builds a new array.
module Unbound.Py.PyRandom

open System
open System.Numerics

let private N = 624
let private M = 397

type State =
    { Mt: uint32[]
      Index: int
      /// gauss() caches its second normal deviate (Random.gauss_next).
      GaussNext: float option }

let private initGenrand (s: uint32) : uint32[] =
    Array.unfold
        (fun (i, prev) ->
            if i >= N then None
            else
                let v = if i = 0 then s else 1812433253u * (prev ^^^ (prev >>> 30)) + uint32 i
                Some(v, (i + 1, v)))
        (0, 0u)

/// init_by_array, as in CPython's _randommodule.c. Pure: builds and returns a fresh array (local buffer only).
let private initByArray (key: uint32[]) : uint32[] =
    let mt = initGenrand 19650218u
    let klen = key.Length
    let mutable i = 1
    let mutable j = 0
    for _ in 1 .. max N klen do
        mt.[i] <- (mt.[i] ^^^ ((mt.[i - 1] ^^^ (mt.[i - 1] >>> 30)) * 1664525u)) + key.[j] + uint32 j
        i <- i + 1
        j <- j + 1
        if i >= N then
            mt.[0] <- mt.[N - 1]
            i <- 1
        if j >= klen then j <- 0
    for _ in 1 .. N - 1 do
        mt.[i] <- (mt.[i] ^^^ ((mt.[i - 1] ^^^ (mt.[i - 1] >>> 30)) * 1566083941u)) - uint32 i
        i <- i + 1
        if i >= N then
            mt.[0] <- mt.[N - 1]
            i <- 1
    mt.[0] <- 0x80000000u
    mt

/// random.Random(seed) for an integer seed (the absolute value's 32-bit little-endian words are the key).
let seed (n: BigInteger) : State =
    let a = BigInteger.Abs n
    let key =
        if a.IsZero then [| 0u |]
        else
            Array.unfold (fun (v: BigInteger) -> if v.IsZero then None else Some(uint32 (v &&& BigInteger 0xFFFFFFFFu), v >>> 32)) a
    { Mt = initByArray key; Index = N; GaussNext = None }

let seedInt (n: int64) : State = seed (BigInteger n)

/// One MT19937 generation step. Pure: the input array is not modified; a new array is returned.
let private twist (input: uint32[]) : uint32[] =
    let mt = Array.copy input
    let mag (y: uint32) = if y &&& 1u = 0u then 0u else 0x9908b0dfu
    for k in 0 .. N - 1 do
        let y = (mt.[k] &&& 0x80000000u) ||| (mt.[(k + 1) % N] &&& 0x7fffffffu)
        mt.[k] <- mt.[(k + M) % N] ^^^ (y >>> 1) ^^^ mag y
    mt

/// genrand_uint32.
let nextUInt32 (s: State) : uint32 * State =
    let mt, idx = if s.Index >= N then twist s.Mt, 0 else s.Mt, s.Index
    let y0 = mt.[idx]
    let y1 = y0 ^^^ (y0 >>> 11)
    let y2 = y1 ^^^ ((y1 <<< 7) &&& 0x9d2c5680u)
    let y3 = y2 ^^^ ((y2 <<< 15) &&& 0xefc60000u)
    y3 ^^^ (y3 >>> 18), { s with Mt = mt; Index = idx + 1 }

/// random.random(): 53-bit float in [0, 1).
let random (s: State) : float * State =
    let a, s1 = nextUInt32 s
    let b, s2 = nextUInt32 s1
    (float (a >>> 5) * 67108864.0 + float (b >>> 6)) * (1.0 / 9007199254740992.0), s2

/// random.gauss(mu, sigma), including the cached second deviate.
let gauss (mu: float) (sigma: float) (s: State) : float * State =
    match s.GaussNext with
    | Some z -> mu + z * sigma, { s with GaussNext = None }
    | None ->
        let u1, s1 = random s
        let u2, s2 = random s1
        let x2pi = u1 * 2.0 * Math.PI
        let g2rad = Math.Sqrt(-2.0 * Math.Log(1.0 - u2))
        let z = Math.Cos x2pi * g2rad
        mu + z * sigma, { s2 with GaussNext = Some(Math.Sin x2pi * g2rad) }

/// random.triangular(low, high, mode); mode None means the midpoint.
let triangular (low: float) (high: float) (mode: float option) (s: State) : float * State =
    let u, s1 = random s
    if high - low = 0.0 && mode.IsSome then low, s1
    else
        let c = match mode with None -> 0.5 | Some m -> (m - low) / (high - low)
        let u, c, low, high = if u > c then 1.0 - u, 1.0 - c, high, low else u, c, low, high
        low + (high - low) * Math.Sqrt(u * c), s1

/// Python-compatible float text: repr, fixed-point format specs and round(), all correctly rounded on the exact binary
/// value with round-half-even ties, as CPython does.
module Unbound.Py.PyFloat

open System
open System.Globalization
open System.Numerics

/// Shortest round-trip digits and decimal-point position: x = 0.DIGITS × 10^decpt (x finite, non-zero, positive).
let private shortest (x: float) : string * int =
    let s = x.ToString("R", CultureInfo.InvariantCulture)
    let mant, exp =
        match s.IndexOfAny [| 'E'; 'e' |] with
        | -1 -> s, 0
        | i -> s.Substring(0, i), int (s.Substring(i + 1))
    let intPart, frac =
        match mant.IndexOf '.' with
        | -1 -> mant, ""
        | i -> mant.Substring(0, i), mant.Substring(i + 1)
    let raw = intPart + frac
    let lead = raw.Length - raw.TrimStart('0').Length
    let digits = raw.Trim('0')
    let decpt = intPart.Length + exp - lead
    (if digits = "" then "0" else digits), decpt

/// Python repr(float).
let repr (x: float) : string =
    if Double.IsNaN x then "nan"
    elif Double.IsPositiveInfinity x then "inf"
    elif Double.IsNegativeInfinity x then "-inf"
    elif x = 0.0 then (if BitConverter.DoubleToInt64Bits x < 0L then "-0.0" else "0.0")
    else
        let sign = if x < 0.0 then "-" else ""
        let digits, decpt = shortest (abs x)
        let body =
            if decpt <= -4 || decpt > 16 then
                let e = decpt - 1
                let m = if digits.Length = 1 then digits else digits.Substring(0, 1) + "." + digits.Substring 1
                m + (if e < 0 then "e-" else "e+") + (abs e).ToString("00", CultureInfo.InvariantCulture)
            elif decpt <= 0 then "0." + String('0', -decpt) + digits
            elif decpt >= digits.Length then digits + String('0', decpt - digits.Length) + ".0"
            else digits.Substring(0, decpt) + "." + digits.Substring decpt
        sign + body

/// Exact binary decomposition x = m × 2^e (x finite).
let private decompose (x: float) : BigInteger * int =
    let bits = BitConverter.DoubleToInt64Bits x
    let exp = int ((bits >>> 52) &&& 0x7FFL)
    let frac = bits &&& 0xFFFFFFFFFFFFFL
    if exp = 0 then BigInteger frac, -1074 else BigInteger(frac ||| 0x10000000000000L), exp - 1075

/// round-half-even of |x| × 10^p to an integer, computed exactly.
let private scaledHalfEven (x: float) (p: int) : BigInteger =
    let m, e = decompose (abs x)
    let num = m * BigInteger.Pow(BigInteger 10, p) * (if e > 0 then BigInteger.Pow(BigInteger 2, e) else BigInteger.One)
    let den = if e < 0 then BigInteger.Pow(BigInteger 2, -e) else BigInteger.One
    let q = BigInteger.Divide(num, den)
    let r = num - q * den
    let twice = r * BigInteger 2
    if twice > den || (twice = den && not q.IsEven) then q + BigInteger.One else q

let private group (intDigits: string) =
    intDigits
    |> Seq.rev
    |> Seq.chunkBySize 3
    |> Seq.map (fun c -> String(Array.rev c))
    |> Seq.rev
    |> String.concat ","

/// Python format(x, "[+][,].Nf").
let fixedWith (plus: bool) (comma: bool) (p: int) (x: float) : string =
    if Double.IsNaN x then "nan"
    elif Double.IsInfinity x then (if x > 0.0 then (if plus then "+inf" else "inf") else "-inf")
    else
        let q = (scaledHalfEven x p).ToString(CultureInfo.InvariantCulture).PadLeft(p + 1, '0')
        let ip, fp = q.Substring(0, q.Length - p), q.Substring(q.Length - p)
        let ip = if comma then group ip else ip
        let negative = x < 0.0 || (x = 0.0 && BitConverter.DoubleToInt64Bits x < 0L)
        let sign = if negative then "-" elif plus then "+" else ""
        sign + ip + (if p > 0 then "." + fp else "")

/// Python format(x, ".Nf").
let fixedPoint (p: int) (x: float) : string = fixedWith false false p x

/// Python round(x, n) for n >= 0 (returns a float).
let roundTo (n: int) (x: float) : float =
    if Double.IsNaN x || Double.IsInfinity x then x
    else Double.Parse(fixedPoint n x, CultureInfo.InvariantCulture)

/// Python round(x) with no ndigits: nearest integer, ties to even.
let roundInt (x: float) : BigInteger =
    let q = scaledHalfEven x 0
    if x < 0.0 then -q else q

/// Python str(int) with format "0Nd".
let padInt (width: int) (n: int64) : string =
    if n < 0L then "-" + (-n).ToString(CultureInfo.InvariantCulture).PadLeft(width - 1, '0')
    else n.ToString(CultureInfo.InvariantCulture).PadLeft(width, '0')

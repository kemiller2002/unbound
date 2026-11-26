/// CPython 3.12+ builtin sum() over floats: Neumaier compensated summation (Python/bltinmodule.c).
module Unbound.Py.PySum

/// sum(xs) for a sequence of floats, starting from integer 0 as Python does. Note: Python's sum of an EMPTY sequence is
/// the int 0 (printed "0", not "0.0"); callers that print the result must handle that case.
let sum (xs: float seq) : float =
    let total, c =
        xs
        |> Seq.fold
            (fun (f: float, c: float) x ->
                let t = f + x
                let c' = if abs f >= abs x then c + ((f - t) + x) else c + ((x - t) + f)
                t, c')
            (0.0, 0.0)
    if c <> 0.0 && System.Double.IsFinite c then total + c else total

/// sum(f(x) for x in xs).
let sumBy (f: 'a -> float) (xs: 'a seq) : float = xs |> Seq.map f |> sum

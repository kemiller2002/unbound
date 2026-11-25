/// Pure descriptive statistics.
module Unbound.Replay.Stats

/// Share with a zero-denominator guard.
let share (num: float) (den: float) : float = if den = 0.0 then 0.0 else num / den

/// Nearest-rank percentile (p in [0,1]) of a non-empty sample; 0 for an empty sample.
let percentile (p: float) (xs: float seq) : float =
    let sorted = xs |> Seq.sort |> Array.ofSeq

    if sorted.Length = 0 then
        0.0
    else
        let rank = int (ceil (p * float sorted.Length)) |> max 1 |> min sorted.Length
        sorted.[rank - 1]

type Summary = { N: int; Min: float; P50: float; Max: float; Mean: float }

let summarize (xs: float seq) : Summary =
    let a = Array.ofSeq xs

    if a.Length = 0 then
        { N = 0; Min = 0.0; P50 = 0.0; Max = 0.0; Mean = 0.0 }
    else
        { N = a.Length
          Min = Array.min a
          P50 = percentile 0.5 a
          Max = Array.max a
          Mean = Array.average a }

let round4 (x: float) = System.Math.Round(x, 4, System.MidpointRounding.AwayFromZero)

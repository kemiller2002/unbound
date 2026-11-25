/// Pure fuzzy name matching for sanctions screening: normalisation, Jaro-Winkler, and a blocked index.
module Unbound.Replay.NameMatch

open System
open Unbound.Replay.Domain

/// Legal-form and article tokens removed before comparison. FOUNDATION, CHURCH and similar words are kept on purpose:
/// removing them would let "SMITH FOUNDATION" collide with an individual named SMITH.
let stopwords =
    set [ "INC"; "INCORPORATED"; "LLC"; "LTD"; "LIMITED"; "CO"; "CORP"; "CORPORATION"; "COMPANY"; "THE"; "OF"; "AND"
          "SA"; "CV"; "DE"; "LA"; "EL"; "A"; "AN" ]

let tokens (s: string) : string list =
    s.ToUpperInvariant()
    |> String.map (fun c -> if c < '\u0080' && Char.IsLetterOrDigit c then c else ' ')
    |> fun t -> t.Split(' ', StringSplitOptions.RemoveEmptyEntries)
    |> Array.filter (stopwords.Contains >> not)
    |> List.ofArray

/// Order-insensitive comparison key: sorted tokens joined by a space.
let key (s: string) : string = tokens s |> List.sort |> String.concat " "

/// Jaro similarity, written as folds over immutable state.
let jaro (a: string) (b: string) : float =
    if a = b then 1.0
    elif a.Length = 0 || b.Length = 0 then 0.0
    else
        let window = max 0 (max a.Length b.Length / 2 - 1)

        let matchedA, matchedB =
            seq { 0 .. a.Length - 1 }
            |> Seq.fold
                (fun (ma: int list, mb: Set<int>) i ->
                    seq { max 0 (i - window) .. min (b.Length - 1) (i + window) }
                    |> Seq.tryFind (fun j -> not (mb.Contains j) && b.[j] = a.[i])
                    |> function
                        | Some j -> (i :: ma, mb.Add j)
                        | None -> (ma, mb))
                ([], Set.empty)

        let m = float (List.length matchedA)

        if m = 0.0 then
            0.0
        else
            let aChars = matchedA |> List.rev |> List.map (fun i -> a.[i])
            let bChars = matchedB |> Set.toList |> List.map (fun j -> b.[j])
            let half = List.zip aChars bChars |> List.filter (fun (x, y) -> x <> y) |> List.length |> float
            let t = half / 2.0
            (m / float a.Length + m / float b.Length + (m - t) / m) / 3.0

/// Jaro-Winkler with the standard 0.1 prefix scale over at most four characters.
let jaroWinkler (a: string) (b: string) : float =
    let j = jaro a b

    let prefix =
        Seq.zip a b |> Seq.truncate 4 |> Seq.takeWhile (fun (x, y) -> x = y) |> Seq.length |> float

    j + prefix * 0.1 * (1.0 - j)

let private prefixes (k: string) =
    k.Split ' ' |> Array.filter (fun t -> t.Length >= 3) |> Array.map (fun t -> t.Substring(0, 3)) |> Set.ofArray

/// Screening index: every listed name keyed, with a 3-character token-prefix block map and token rarity weights.
/// Prefix buckets larger than `bucketCap` are dropped as non-selective; the full-key bucket always applies.
type Index =
    { Keys: (string * SdnName) array
      Blocks: Map<string, int list>
      Exact: Map<string, int list>
      Weights: Map<string, float>
      MaxWeight: float }

let buildIndex (bucketCap: int) (names: SdnName seq) : Index =
    let keys =
        names
        |> Seq.filter (fun n -> n.Kind <> Conveyance)
        |> Seq.map (fun n -> key n.Name, n)
        |> Seq.filter (fun (k, _) -> k.Length > 0)
        |> Seq.distinctBy (fun (k, n) -> k, n.EntNum)
        |> Array.ofSeq

    let group (f: string -> string seq) =
        keys
        |> Seq.mapi (fun i (k, _) -> f k |> Seq.map (fun p -> p, i))
        |> Seq.concat
        |> Seq.groupBy fst
        |> Seq.map (fun (p, xs) -> p, xs |> Seq.map snd |> List.ofSeq)

    let n = float keys.Length
    let idf df = log (1.0 + n / float df)

    { Keys = keys
      Blocks = group (prefixes >> Set.toSeq) |> Seq.filter (fun (_, xs) -> List.length xs <= bucketCap) |> Map.ofSeq
      Exact = group Seq.singleton |> Map.ofSeq
      Weights = group (fun k -> k.Split ' ' |> Seq.distinct) |> Seq.map (fun (t, xs) -> t, idf (List.length xs)) |> Map.ofSeq
      MaxWeight = idf 1 }

/// A scorer compares a query key with a listed key.
type Scorer = Index -> string -> string -> float

/// Whole-string Jaro-Winkler over sorted-token keys: shared generic words raise the score.
let naive: Scorer = fun _ q l -> jaroWinkler q l

let private weight (idx: Index) (t: string) =
    idx.Weights.TryFind t |> Option.defaultValue idx.MaxWeight

/// Rarity-weighted share of tokens in `a` that have a close (Jaro-Winkler >= 0.85) counterpart in `b`.
let coverage (idx: Index) (a: string list) (b: string list) : float =
    let total = a |> List.sumBy (weight idx)

    if total = 0.0 then
        0.0
    else
        let closest t =
            b |> List.fold (fun m u -> max m (jaroWinkler t u)) 0.0 |> fun s -> if s >= 0.85 then s else 0.0

        (a |> List.sumBy (fun t -> weight idx t * closest t)) / total

/// Token-weighted score: mostly "are the listed name's distinctive parts all present", partly the reverse.
let weighted: Scorer =
    fun idx q l ->
        let qt = q.Split(' ', StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
        let lt = l.Split(' ', StringSplitOptions.RemoveEmptyEntries) |> List.ofArray
        0.8 * coverage idx lt qt + 0.2 * coverage idx qt lt

/// Best score of `name` against the index under `scorer`, with the matched listed name. 0 when no candidate shares a block.
let best (scorer: Scorer) (index: Index) (name: string) : float * SdnName option =
    let k = key name

    let candidates =
        prefixes k
        |> Seq.choose index.Blocks.TryFind
        |> Seq.append (index.Exact.TryFind k |> Option.toList)
        |> Seq.concat
        |> Seq.distinct

    candidates
    |> Seq.map (fun i ->
        let ck, n = index.Keys.[i]
        scorer index k ck, Some n)
    |> Seq.fold (fun (bs, bn) (s, n) -> if s > bs then (s, n) else (bs, bn)) (0.0, None)

/// Deterministic perturbations used to measure recall: identity, punctuation/case noise, dropped character,
/// token reorder, appended legal suffix.
let mutate (variant: int) (name: string) : string =
    let ts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries) |> List.ofArray

    match variant % 5 with
    | 0 -> name
    | 1 -> name.ToLowerInvariant().Replace(" ", " - ").Replace(".", "")
    | 2 when name.Length > 6 -> name.Remove(name.Length / 2, 1)
    | 3 when List.length ts > 1 -> (List.last ts :: List.take (List.length ts - 1) ts) |> String.concat " "
    | _ -> name + " INC"

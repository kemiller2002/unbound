/// zlib.crc32 (IEEE 802.3 polynomial, reflected), as an unsigned value.
module Unbound.Py.Crc32

open System.Text

let private table : uint32[] =
    Array.init 256 (fun n ->
        seq { 1..8 } |> Seq.fold (fun (c: uint32) _ -> if c &&& 1u <> 0u then 0xEDB88320u ^^^ (c >>> 1) else c >>> 1) (uint32 n))

let ofBytes (bytes: byte seq) : uint32 =
    bytes
    |> Seq.fold (fun (c: uint32) b -> table.[int ((c ^^^ uint32 b) &&& 0xFFu)] ^^^ (c >>> 8)) 0xFFFFFFFFu
    |> fun c -> c ^^^ 0xFFFFFFFFu

/// zlib.crc32(s.encode()) (UTF-8).
let ofString (s: string) : uint32 = ofBytes (Encoding.UTF8.GetBytes s)

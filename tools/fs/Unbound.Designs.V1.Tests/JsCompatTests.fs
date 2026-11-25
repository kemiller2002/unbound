/// JavaScript-compatibility helpers against values printed by Node 22.
module Unbound.Designs.V1.Tests.JsCompatTests

open Xunit
open Unbound.Designs.V1.JsCompat

[<Fact>]
let ``mulberry32 matches the JavaScript generator`` () =
    // node: prng(482) -> 0.2112622514832765 0.014763706363737583 0.25666302535682917
    let s1, a = mulberry32 482
    let s2, b = mulberry32 s1
    let _, c = mulberry32 s2
    Assert.Equal(0.2112622514832765, a)
    Assert.Equal(0.014763706363737583, b)
    Assert.Equal(0.25666302535682917, c)
    // node: prng(-7) -> 0.43306733411736786 0.32539576734416187
    let t1, d = mulberry32 -7
    let _, e = mulberry32 t1
    Assert.Equal(0.43306733411736786, d)
    Assert.Equal(0.32539576734416187, e)

[<Fact>]
let ``JSON.stringify with indent 2`` () =
    let v =
        JsArray
            [ JsString "a\"b\\c\n\u0001é<>"
              JsArray []
              JsObject []
              JsObject [ "k", JsArray [ JsNumber 1.0; JsBool true; JsNull ] ] ]
    let expected =
        String.concat
            "\n"
            [ "["
              "  \"a\\\"b\\\\c\\n\\u0001é<>\","
              "  [],"
              "  {},"
              "  {"
              "    \"k\": ["
              "      1,"
              "      true,"
              "      null"
              "    ]"
              "  }"
              "]" ]
    Assert.Equal(expected, stringify (Some 2) v)
    Assert.Equal("[[\"x\",\"<code>y</code>\"],[]]", stringify None (JsArray [ JsArray [ JsString "x"; JsString "<code>y</code>" ]; JsArray [] ]))

[<Fact>]
let ``JSON.parse round-trips the manifest shape`` () =
    let text = "[\n  {\n    \"page\": \"S-001.html\",\n    \"ok\": true,\n    \"no\": false,\n    \"n\": null,\n    \"s\": \"a\\\"é\\n\",\n    \"xs\": []\n  }\n]"
    let v = parse text
    Assert.Equal(text, stringify (Some 2) v)
    Assert.Equal(Some(JsString "S-001.html"), (match v with JsArray [ o ] -> prop "page" o | _ -> None))
    Assert.Equal(JsString "é\t/", parse "\"\\u00e9\\t\\/\"")

[<Fact>]
let ``toLocaleString en-US USD from integer cents`` () =
    // node: (123456789/100).toLocaleString(...) -> $1,234,567.89; (5/100) -> $0.05
    Assert.Equal("$1,234,567.89", usdOfCents 123456789L)
    Assert.Equal("$0.05", usdOfCents 5L)
    Assert.Equal("$0.00", usdOfCents 0L)
    Assert.Equal("$412,907,551.06", usdOfCents 41290755106L)
    Assert.Equal("-$4,210.33", usdOfCents -421033L)

[<Fact>]
let ``Math.round rounds halves toward positive infinity`` () =
    // node: Math.round(-2.5) -> -2, Math.round(2.5) -> 3, Math.round(1750.5*100) -> 175050, Math.round(-4210.33*100) -> -421033
    Assert.Equal(-2.0, mathRound -2.5)
    Assert.Equal(3.0, mathRound 2.5)
    Assert.Equal(175050.0, mathRound (1750.5 * 100.0))
    Assert.Equal(-421033.0, mathRound (-4210.33 * 100.0))

[<Fact>]
let ``Date.UTC day overflow and padStart`` () =
    // node: new Date(Date.UTC(2025, 9, 92)).toISOString().slice(0, 10) -> 2025-12-31
    Assert.Equal("2025-12-31", utcIsoDate 2025 9 92)
    Assert.Equal("2026-07-01", utcIsoDate 2026 6 1)
    Assert.Equal("0042", padStart 4 '0' "42")
    Assert.Equal("4913", padStart 4 '0' "4913")

[<Fact>]
let ``String.replace with a string pattern replaces the first match only`` () =
    Assert.Equal("s001", replaceFirst "-" "" "s-001")
    Assert.Equal("a-b-c", replaceFirst "x" "" "a-b-c")
    Assert.Equal("2026-1 Grant ", replaceFirst "Grant " "" "Grant 2026-1 Grant ")

[<Fact>]
let ``sha256 hex of a UTF-8 string`` () =
    Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", sha256Hex "")
    Assert.Equal("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", sha256Hex "abc")

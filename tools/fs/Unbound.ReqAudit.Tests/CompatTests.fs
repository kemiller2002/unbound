/// Python-compatibility primitives used by the port; expectations are CPython 3.13 results.
module Unbound.ReqAudit.Tests.CompatTests

open Xunit
open Unbound.Py.PyJson
open Unbound.ReqAudit.PyCompat
open Unbound.ReqAudit

[<Fact>]
let ``repr of str chooses quotes and escapes like CPython`` () =
    Assert.Equal("'abc'", reprStr "abc")
    Assert.Equal("\"it's\"", reprStr "it's")
    Assert.Equal("'both \\' and \"'", reprStr "both ' and \"")
    Assert.Equal("'tab\\tnl\\n\\x07\\x7f'", reprStr "tab\tnl\n\a\u007f")
    Assert.Equal("'é—\\xa0\\u2028'", reprStr "é—  ")

[<Fact>]
let ``repr of loaded values matches dict and list repr`` () =
    Assert.Equal("{'a': [1, 2.5, None, True], 'b': 'x'}", repr (JObj [ "a", JArr [ jint 1; JFloat 2.5; JNull; JBool true ]; "b", JStr "x" ]))
    Assert.Equal("['a', 'b']", reprList [ "a"; "b" ])

[<Fact>]
let ``str methods use Python whitespace and line boundaries`` () =
    Assert.Equal("x", strip "\u001c x  ")
    Assert.Equal<string list>([ "a"; "b"; "c" ], split " a\tb\u001f c ")
    Assert.Equal<string list>([ "a"; "b"; ""; "c" ], splitLines "a\r\nb \nc")
    Assert.Equal<string list>([], splitLines "")

[<Fact>]
let ``lower applies final sigma and dotted capital I`` () =
    Assert.Equal("aς b", lower "AΣ B")
    Assert.Equal("σ", lower "Σ")
    Assert.Equal("i̇", lower "İ")

[<Fact>]
let ``translated word boundaries follow Python's Unicode word class`` () =
    // '¼' is a word character for Python (No category) but not for .NET's \w
    Assert.False(isSearch (re @"\bAB-12\b") "¼AB-12")
    Assert.True(isSearch (re @"\bAB-12\b") "_ AB-12.")
    Assert.True(isFull (re "a|b") "b")
    Assert.False(isFull (re "a|b") "ab")

[<Fact>]
let ``code point order puts astral characters after the BMP`` () =
    Assert.True(cmpStr "�" "\U0001F600" < 0)
    Assert.True(cmpStr "a" "ab" < 0)

[<Fact>]
let ``counter keeps insertion order and most_common is stable`` () =
    let c = counter [ "b"; "a"; "b"; "c"; "a" ]
    Assert.Equal<(string * int) list>([ "b", 2; "a", 2; "c", 1 ], c)
    Assert.Equal<(string * int) list>([ "b", 2; "a", 2; "c", 1 ], byCountDesc c)

[<Fact>]
let ``ranges expand with zero padding`` () =
    Assert.Equal("UNB-SUCC-008 UNB-SUCC-009 UNB-SUCC-010 and X", Graph.expandRanges "UNB-SUCC-008..010 and X")
    Assert.Equal<string list>([ "A-001"; "A-002"; "B-7" ], Graph.idsIn "A-001..002, B-7, A-001")

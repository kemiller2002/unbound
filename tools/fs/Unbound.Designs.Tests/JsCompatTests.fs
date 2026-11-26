/// Golden values printed by Node 22 for the JavaScript behaviour JsCompat reproduces.
module Unbound.Designs.Tests.JsCompatTests

open Xunit
open Unbound.Designs.JsCompat

[<Theory>]
[<InlineData(0.125, "0.1", "0.13")>] // exact tie: JavaScript rounds up, .NET "F2" would give 0.12
[<InlineData(1.005, "1.0", "1.00")>] // exact value is below the tie
[<InlineData(8.345, "8.3", "8.35")>]
[<InlineData(83.33333333333334, "83.3", "83.33")>]
[<InlineData(4.166666666666666, "4.2", "4.17")>]
[<InlineData(-0.001, "-0.0", "-0.00")>]
[<InlineData(-0.0, "0.0", "0.00")>]
[<InlineData(1.45, "1.4", "1.45")>]
let ``toFixed matches Number.prototype.toFixed`` (x: float, one: string, two: string) =
    Assert.Equal(one, toFixed 1 x)
    Assert.Equal(two, toFixed 2 x)

[<Theory>]
[<InlineData(0.8, "0.8")>]
[<InlineData(1e21, "1e+21")>]
[<InlineData(1e-7, "1e-7")>]
[<InlineData(123e-9, "1.23e-7")>]
[<InlineData(0.000001, "0.000001")>]
[<InlineData(1.5e300, "1.5e+300")>]
[<InlineData(-2.5, "-2.5")>]
[<InlineData(100.0, "100")>]
[<InlineData(5e-324, "5e-324")>]
[<InlineData(123456789012345680000.0, "123456789012345680000")>]
let ``numberToString matches String(n)`` (x: float, expected: string) = Assert.Equal(expected, numberToString x)

[<Theory>]
[<InlineData(18240.55, "18,240.55")>]
[<InlineData(1204330.12, "1,204,330.12")>]
[<InlineData(1.005, "1.01")>] // ICU rounds the shortest decimal, not the binary value
[<InlineData(2.675, "2.68")>]
[<InlineData(0.005, "0.01")>]
[<InlineData(1e9, "1,000,000,000.00")>]
[<InlineData(-0.0, "-0.00")>]
let ``formatEnUs2 matches Intl.NumberFormat en-US with two decimals`` (x: float, expected: string) =
    Assert.Equal(expected, formatEnUs2 x)

[<Fact>]
let ``localeCompare sorts like Node's ICU root collation`` () =
    let input = [ "b"; "a-b"; "a/b"; "a.b"; "A"; "a"; "ab"; "Ab"; "a_b"; "a1"; "aB" ]
    let expected = [ "a"; "A"; "a_b"; "a-b"; "a.b"; "a/b"; "a1"; "ab"; "aB"; "Ab"; "b" ]
    Assert.Equal<string list>(expected, List.sortWith localeCompare input)

[<Fact>]
let ``stringify matches JSON.stringify with two-space indent, escaping included`` () =
    let v =
        JObj
            [ "a", JStr("q\"\\\n\u0001" + string (char 0xD800) + "é")
              "b", JArr []
              "c", JObj []
              "d", JArr [ JNum 1.0; JObj [ "e", JNull ] ] ]
    let expected =
        "{\n  \"a\": \"q\\\"\\\\\\n\\u0001\\ud800é\",\n  \"b\": [],\n  \"c\": {},\n  \"d\": [\n    1,\n    {\n      \"e\": null\n    }\n  ]\n}"
    Assert.Equal(expected, stringify v)

[<Fact>]
let ``parseJson keeps key order and rejects what JSON.parse rejects`` () =
    Assert.Equal(Some(JObj [ "z", JNum 1.0; "a", JArr [ JStr "x"; JBool true; JNull ] ]), parseJson """{"z":1,"a":["x",true,null]}""")
    Assert.Equal(None, parseJson "[1,]")
    Assert.Equal(None, parseJson "﻿[]")

[<Fact>]
let ``interpolate follows template-literal conversion`` () =
    Assert.Equal("undefined", interpolate None)
    Assert.Equal("null", interpolate (Some JNull))
    Assert.Equal("0.8", interpolate (Some(JNum 0.8)))
    Assert.Equal("a,,1", interpolate (Some(JArr [ JStr "a"; JNull; JNum 1.0 ])))
    Assert.Equal("[object Object]", interpolate (Some(JObj [])))

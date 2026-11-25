/// Unit tests of the pure check functions on fixed measurement inputs.
module Unbound.Designs.Verify.Tests.CheckTests

open Xunit
open Unbound.Designs.Verify
open Unbound.Designs.Verify.JsCompat

// ---------- JsCompat ----------

[<Theory>]
[<InlineData(0.0, "0")>]
[<InlineData(1280.0, "1280")>]
[<InlineData(23.5, "23.5")>]
[<InlineData(0.1, "0.1")>]
[<InlineData(-4.25, "-4.25")>]
[<InlineData(1e21, "1e+21")>]
[<InlineData(1.5e-7, "1.5e-7")>]
[<InlineData(0.000001, "0.000001")>]
[<InlineData(123456789012345680000.0, "123456789012345680000")>]
let ``numberToString matches Number.prototype.toString`` (d: float, expected: string) =
    Assert.Equal(expected, numberToString d)

[<Fact>]
let ``stringify matches JSON.stringify with indentation and JS escapes`` () =
    let v = JObj [ "a", JArr [ JNum 1.0; JStr "x\"\n\u0001" ]; "b", JObj []; "c", JArr [] ]
    Assert.Equal("{\n  \"a\": [\n    1,\n    \"x\\\"\\n\\u0001\"\n  ],\n  \"b\": {},\n  \"c\": []\n}", stringify 2 v)
    Assert.Equal("{\"a\":[1,\"x\\\"\\n\\u0001\"],\"b\":{},\"c\":[]}", stringify 0 v)

[<Fact>]
let ``obj puts integer-like keys first, ascending, like a JS object`` () =
    match obj [ "f", JNull; "1280", JNull; "320", JNull; "1280z200", JNull ] with
    | JObj kvs -> Assert.Equal<string list>([ "320"; "1280"; "f"; "1280z200" ], List.map fst kvs)
    | _ -> failwith "not an object"

[<Fact>]
let ``localeCompare orders punctuation before digits before letters, case-insensitively first`` () =
    let sorted = [ "b"; "B"; "a-c"; "ab"; "A"; "a/b"; "1" ] |> List.sortWith localeCompare
    Assert.Equal<string list>([ "1"; "A"; "a-c"; "a/b"; "ab"; "b"; "B" ], sorted)

// ---------- verify-v2 ----------

let private focus name tag visible obscured by =
    JObj [ "name", JStr name; "tag", JStr tag; "visibleIndicator", JBool visible; "obscured", JBool obscured; "obscuredBy", by ]

let private cleanBase =
    JObj [
        "overflowX", JBool false; "scrollWidth", JNum 390.0; "wrappedMoney", JArr []; "smallTargets", JArr []
        "h1Count", JNum 1.0; "skippedHeadingLevels", JNum 0.0; "duplicateNavLabels", JBool false; "mainCount", JNum 1.0
    ]

let private cleanSpacing = JObj [ "clipped", JArr []; "overflowX", JBool false; "wrappedMoney", JArr [] ]

[<Fact>]
let ``v2 failures are empty for clean measurements`` () =
    Assert.Empty(V2.failures [] cleanBase [ focus "Home" "a" true false JNull ] cleanSpacing [])

[<Fact>]
let ``v2 failures list every finding in the original order`` () =
    let pageBase =
        JObj [
            "overflowX", JBool true; "wrappedMoney", JArr [ JStr "$1,000.00" ]; "smallTargets", JArr [ JStr "a \"x\" 10x10" ]
            "h1Count", JNum 2.0; "skippedHeadingLevels", JNum 1.0; "duplicateNavLabels", JBool true
        ]
    let axe = V2.axeFindings (JObj [ "violations", JArr [ JObj [ "id", JStr "image-alt" ] ]; "contrastIncomplete", JArr [ JStr "#x" ] ])
    let f = [ focus "Menu" "button" false true (JStr "unb-bar") ]
    let spacing = JObj [ "clipped", JArr [ JStr "card" ]; "overflowX", JBool true; "wrappedMoney", JArr [] ]
    let expected =
        [ "axe:image-alt"; "axe:color-contrast-incomplete"; "overflow-x"; "money-wraps:$1,000.00"; "target<24:a \"x\" 10x10"
          "h1-count:2"; "heading-skip:1"; "duplicate-nav-label"; "focus-not-visible:Menu"; "focus-obscured:Menu (by unb-bar)"
          "text-spacing-clipped:card"; "text-spacing-overflow-x"; "console:failed: http://x/y.css" ]
    Assert.Equal<string list>(expected, V2.failures axe pageBase f spacing [ "failed: http://x/y.css" ])

[<Fact>]
let ``v2 keyboard walk stops at no focus and when focus wraps after more than three stops`` () =
    let a, b, c, d = focus "A" "a" true false JNull, focus "B" "a" true false JNull, focus "C" "a" true false JNull, focus "D" "a" true false JNull
    Assert.Equal(None, V2.walkStep [ a ] JNull)
    Assert.Equal(Some [ a; b; c; a ], V2.walkStep [ a; b; c ] a) // only three so far: keep going
    Assert.Equal(None, V2.walkStep [ a; b; c; d ] a)

[<Fact>]
let ``v2 merge keeps unchecked prior pages and orders by file`` () =
    let page f = JObj [ "file", JStr f ]
    let merged = V2.merge [ page "screens/S-003/a.html"; page "screens/S-001/b.html" ] [ page "screens/S-001/b.html"; page "screens/S-001-x/a.html" ]
    // ICU root order, as Node's localeCompare: "-" sorts before "/".
    Assert.Equal<Json list>([ page "screens/S-001-x/a.html"; page "screens/S-001/b.html"; page "screens/S-003/a.html" ], merged)

// ---------- verify-screens ----------

[<Fact>]
let ``screens componentsUsed maps tags, class roots and aliases onto catalog ids`` () =
    let html = """<forma-alert class="forma-component-tag"></forma-alert><div class="forma-field__label forma-status-lozenge--ok  forma-site"></div><button>x</button>"""
    let ids = [ "alert"; "text-field"; "status-lozenge"; "marketing-shell"; "button"; "unused" ]
    Assert.Equal<string list>([ "alert"; "button"; "marketing-shell"; "status-lozenge"; "text-field" ], Screens.componentsUsed html ids)

[<Fact>]
let ``screens static rules flag style elements, style attributes and scripts`` () =
    Assert.Empty(Screens.staticRuleViolations "<p class=\"x\">fine</p>")
    Assert.Equal<string list>(
        [ "page contains a <style> element"; "page contains a style attribute"; "page contains a <script> element" ],
        Screens.staticRuleViolations "<STYLE>p{}</STYLE><p style=\"x\"></p><script>1</script>"
    )

[<Fact>]
let ``screens totals count checks and violations`` () =
    let width hs nodes dialogs =
        JObj [ "h_scroll", JBool hs; "axe_violations", JArr [ for n in nodes -> JObj [ "nodes", JNum n ] ]; "dialogs", JArr dialogs ]
    let dialog = JObj [ "axe_violations", JArr [ JObj [ "nodes", JNum 1.0 ] ] ]
    let page = JObj [ "widths", JObj [ "320", width true [ 2.0; 3.0 ] [ dialog ]; "1280", width false [] [ dialog ] ]; "static_rule_violations", JArr [ JStr "x" ] ]
    Assert.Equal(
        """{"pages":1,"page_width_checks":2,"h_scroll_failures":1,"axe_violation_rules_page":2,"axe_violation_nodes_page":5,"dialog_checks":2,"axe_violation_rules_dialogs":2,"static_rule_violations":1}""",
        stringify 0 (Screens.totals [ page ])
    )

// ---------- verify-documents ----------

[<Fact>]
let ``documents page numbering needs Page n of N on every page`` () =
    Assert.True(Documents.pageNumberOk [ "x Page 1 of 2 y"; "Page 2  of 2" ])
    Assert.False(Documents.pageNumberOk [ "Page 1 of 2"; "Page 2 of 20" ])

[<Fact>]
let ``documents money format needs two decimals and thousands separators`` () =
    Assert.True(Documents.moneyOk "paid $25,000.00 and $3.10")
    Assert.False(Documents.moneyOk "paid $25000.00")
    Assert.False(Documents.moneyOk "paid $25,000.5")

[<Fact>]
let ``documents check text joins hyphenated line ends and squashes whitespace`` () =
    Assert.Equal("EIN 00-0000000 holds all", Documents.checkPage "  EIN 00-\n0000000\n holds  all ")

[<Fact>]
let ``documents pdfinfo lines become an ordered key list`` () =
    let info = Documents.parsePdfinfo "Producer:       Skia/PDF m158\nTagged:         no\nPages:          8\nPage size:      612 x 792 pts (letter)\n"
    Assert.Equal(Some "8", Documents.infoGet info "Pages")
    Assert.Equal(Some "612 x 792 pts (letter)", Documents.infoGet info "Page size")

[<Fact>]
let ``documents cash acknowledgment checks read the PDF text`` () =
    let doc = Documents.docs |> List.find (fun d -> d.File = "D-001-acknowledgment-cash")
    let text =
        $"{Documents.exclusiveControl} {Documents.noGoods} Example Charitable Foundation EIN 00-0000000 Elena Rivera Date of gift 16 Nov 2026 $25,000.00 ACH 2026-101942"
    let inputs: Documents.Inputs = { Doc = doc; Html = ""; Pages = [ text ]; Info = []; Raw = ""; FolioElements = Set.empty }
    let checks = Documents.specificChecks inputs
    Assert.All(checks, fun (_, v) -> Assert.Equal(JBool true, v))
    let short: Documents.Inputs = { inputs with Pages = [ text; "second page" ] }
    Assert.Equal(Some(JBool false), Documents.specificChecks short |> List.tryFind (fun (k, _) -> k = "single_page_letter") |> Option.map snd)

// ---------- audit-v1 ----------

let private style outline shadow =
    JObj [ "outlineStyle", JStr outline; "outlineWidth", JStr "2px"; "boxShadow", JStr shadow; "backgroundColor", JStr "white"; "borderColor", JStr "black"; "textDecorationLine", JStr "none"; "color", JStr "black" ]

[<Fact>]
let ``audit focusChanged needs a visible change other than removing the outline`` () =
    Assert.True(AuditV1.focusChanged (style "none" "none") (style "solid" "none"))
    Assert.False(AuditV1.focusChanged (style "solid" "none") (style "none" "none"))
    Assert.False(AuditV1.focusChanged JNull (style "solid" "none"))

[<Fact>]
let ``audit analyseFocus finds invisible, backwards and obscured stops`` () =
    let stop idx name y hits st =
        JObj [
            "idx", JStr idx; "tag", JStr "a"; "name", JStr name; "state", JStr "s1"
            "box", JObj [ "x", JNum 0.0; "y", JNum y; "w", JNum 40.0; "h", JNum 40.0 ]
            "visible", JBool true; "obscured", JObj [ "hitCount", JNum hits; "by", JNull ]; "offscreen", JBool false; "style", st
        ]
    let baseline = [ style "none" "none"; style "none" "none" ]
    let r = AuditV1.analyseFocus [ stop "0" "First" 500.0 5.0 (style "solid" "none"); stop "1" "Second" 100.0 0.0 (style "none" "none") ] true baseline
    let names key = asList (r ? (key: string)) |> List.map (fun s -> s ? name)
    Assert.Equal<Json list>([ JStr "Second" ], names "notVisible")
    Assert.Equal<Json list>([ JStr "Second" ], names "obscured")
    Assert.Equal("""[{"from":"First","to":"Second","state":"s1","dy":-400}]""", stringify 0 (r ? backwards))
    Assert.Equal(JNum 2.0, r ? stops)
    Assert.Equal(JBool false, r ? trap)
    Assert.DoesNotContain("style", stringify 0 (r ? sequence))

[<Fact>]
let ``audit summariseAxe keeps WCAG tags, six targets and a 200-character summary`` () =
    let node t = JObj [ "target", JArr [ JStr t ]; "failureSummary", JStr(String.replicate 250 "x") ]
    let v = JObj [ "id", JStr "color-contrast"; "impact", JStr "serious"; "tags", JArr [ JStr "cat.color"; JStr "wcag2aa"; JStr "wcag143" ]; "help", JStr "h"; "nodes", JArr [ for i in 1..8 -> node $"#n{i}" ] ]
    let s = AuditV1.summariseAxe [ v ] |> List.head
    Assert.Equal("""["wcag2aa","wcag143"]""", stringify 0 (s ? tags))
    Assert.Equal(JNum 8.0, s ? nodes)
    Assert.Equal(6, (asList (s ? targets)).Length)
    Assert.Equal(200, (asStr (s ? summary)).Length)

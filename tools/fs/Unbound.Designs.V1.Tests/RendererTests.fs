/// Unit tests of the pure renderers: escaping, money and date formatting, and the markup helpers' optional parts.
module Unbound.Designs.V1.Tests.RendererTests

open Xunit
open Unbound.Designs.V1
open Unbound.Designs.V1.Markup
open Unbound.Designs.V1.DocumentKit

[<Fact>]
let ``esc escapes ampersand, angle brackets and double quotes only`` () =
    Assert.Equal("Tom &amp; &quot;Jerry&quot; &lt;b&gt; it's", esc "Tom & \"Jerry\" <b> it's")
    Assert.Equal("&amp;amp;", esc "&amp;")

[<Fact>]
let ``money helpers`` () =
    Assert.Equal(250000L, cents 2500.0)
    Assert.Equal(125075L, cents 1250.75)
    Assert.Equal(-421033L, cents -4210.33)
    Assert.Equal("$2,500.00", usd (cents 2500.0))
    Assert.Equal("−$4,210.33 (decrease)", signedUsd -421033L)
    Assert.Equal("$10.00 (increase)", signedUsd 1000L)

[<Fact>]
let ``absolute dates drop leading zeros`` () =
    Assert.Equal("1 Jul 2026", absDate "2026-07-01")
    Assert.Equal("31 Dec 2025", absDate "2025-12-31")

[<Fact>]
let ``lozenge state is omitted when empty`` () =
    Assert.Equal("<span class=\"forma-status-lozenge\">Draft</span>", loz "" "Draft")
    Assert.Equal("<span class=\"forma-status-lozenge\" data-state=\"ok\">Paid</span>", loz "ok" "Paid")

[<Fact>]
let ``field renders description, error and attributes like the JavaScript`` () =
    let html = Ui.field(id = "x", label = "L", desc = "D", value = "5", attrs = "required", error = "E")
    Assert.Contains("<input id=\"x\" name=\"x\" type=\"text\" value=\"5\" aria-describedby=\"x-description x-error\" aria-invalid=\"true\" required>", html)
    Assert.Contains("<p class=\"forma-validation-message\" id=\"x-error\">", html)
    let plain = Ui.field(id = "y", label = "L")
    Assert.Equal("<div class=\"forma-field\">\n  <label class=\"forma-field__label\" for=\"y\">L</label>\n  \n  <input id=\"y\" name=\"y\" type=\"text\">\n  \n</div>", plain)

[<Fact>]
let ``scope trail marks the last crumb current`` () =
    Assert.Equal(
        "<forma-scope-trail class=\"forma-component-tag\"><nav class=\"forma-scope-trail\" aria-label=\"Current scope\"><ol><li><a href=\"#scope-0\">A</a></li><li aria-current=\"page\">B</li></ol></nav></forma-scope-trail>",
        scopeTrail [ "A"; "B" ]
    )

[<Fact>]
let ``state ids are lower-cased with the first hyphen removed`` () =
    Assert.Equal("s001-1", BuildScreens.prefixFor "S-001" 0)
    Assert.Equal("w001-3", BuildScreens.prefixFor "W-001" 2)

[<Fact>]
let ``unknown gap ids are undocumented`` () =
    Assert.Equal("undocumented", Gaps.describe "FORMA-GAP-99")
    Assert.StartsWith("Existing.", Gaps.describe "FORMA-GAP-01")

[<Fact>]
let ``partial builds merge into the previous manifest and sort by page`` () =
    let prev = JsCompat.parse "[{\"page\":\"Z.html\"},{\"page\":\"S-001-giving-home.html\",\"stale\":true}]"
    let previous = match prev with JsCompat.JsArray xs -> xs | _ -> []
    let merged = BuildScreens.mergeManifest previous [ BuildScreens.manifestEntry Screens.S001.screen ]
    let pages = merged |> List.map (JsCompat.prop "page")
    Assert.Equal<JsCompat.JsValue option list>([ Some(JsCompat.JsString "S-001-giving-home.html"); Some(JsCompat.JsString "Z.html") ], pages)
    Assert.Equal(None, JsCompat.prop "stale" merged.Head)

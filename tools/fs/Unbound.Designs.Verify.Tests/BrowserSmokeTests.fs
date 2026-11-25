/// One end-to-end smoke test of the DevTools client on a tiny local HTML file.
module Unbound.Designs.Verify.Tests.BrowserSmokeTests

open System
open System.IO
open Xunit
open Unbound.Designs.Verify
open Unbound.Designs.Verify.JsCompat

let private html =
    """<!doctype html><html lang="en"><head><meta charset="utf-8"><title>Smoke</title></head>
<body><main><h1>Smoke</h1><a href="#one">One</a> <button>Two</button><p style="height:2000px">tall</p></main></body></html>"""

[<Fact>]
let ``launches the browser, loads a page, evaluates, tabs, screenshots and prints`` () =
    let dir = Directory.CreateTempSubdirectory("unbound-smoke-").FullName
    let file = Path.Combine(dir, "smoke.html")
    File.WriteAllText(file, html)
    let browser = (Cdp.launch ()).Result
    try
        let page = (Cdp.newPage browser { Width = 320; Height = 480; DeviceScaleFactor = 1.0; ReducedMotion = Some "reduce" }).Result
        (Cdp.goto page (Uri file).AbsoluteUri).Wait()
        Assert.Equal(JStr "Smoke", (Cdp.call page "(sel) => document.querySelector(sel).textContent" [ JStr "h1" ]).Result)
        Assert.Equal(JBool true, (Cdp.evaluate page "matchMedia('(prefers-reduced-motion: reduce)').matches").Result)
        (Cdp.press page "Tab").Wait()
        Assert.Equal(JStr "One", (Cdp.evaluate page "document.activeElement.textContent").Result)
        (Cdp.press page "Tab").Wait()
        Assert.Equal(JStr "BUTTON", (Cdp.evaluate page "document.activeElement.tagName").Result)
        let png = (Cdp.screenshot page true).Result
        // PNG signature, then IHDR width and height (big-endian) at bytes 16..23: a full-page shot is taller than the viewport.
        Assert.Equal<byte[]>([| 0x89uy; 0x50uy; 0x4Euy; 0x47uy |], png.[0..3])
        let be (i: int) = (int png.[i] <<< 24) ||| (int png.[i + 1] <<< 16) ||| (int png.[i + 2] <<< 8) ||| int png.[i + 3]
        Assert.Equal(320, be 16)
        Assert.True(be 20 > 2000)
        let pdf = (Cdp.pdf page false).Result
        Assert.Equal("%PDF", Text.Encoding.ASCII.GetString(pdf, 0, 4))
        (Cdp.closePage page).Wait()
        Assert.Empty(Cdp.events page |> List.filter (function Cdp.Console("error", _) | Cdp.PageError _ -> true | _ -> false))
        Assert.Matches(@"^\d+\.\d+\.\d+\.\d+$", browser.Version)
    finally
        (Cdp.close browser).Wait()
        Directory.Delete(dir, true)

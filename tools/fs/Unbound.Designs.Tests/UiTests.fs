/// Renderer tests. golden/small-page.html was printed by v2/tools/lib/ui.mjs (Node 22) for the same configuration,
/// with the icon registry pointed at golden/registry.json.
module Unbound.Designs.Tests.UiTests

open System.IO
open Xunit
open Unbound.Designs
open Unbound.Designs.Ui

let private golden (name: string) = Path.Combine(System.AppContext.BaseDirectory, "golden", name)

let private registry () =
    File.ReadAllText(golden "registry.json") |> JsCompat.parseJson |> Option.get |> iconRegistry

[<Fact>]
let ``money: true minus sign, explicit plus, grouped hero cents`` () =
    Assert.Equal("""<span class="unb-money" data-sign="negative">−$2,500.00</span>""", money -2500.0)
    Assert.Equal("""<span class="unb-money">+$5,000.00</span>""", moneySigned 5000.0)
    Assert.Equal("""<span class="unb-money">$0.00</span>""", moneySigned 0.0)
    Assert.Equal("""<span class="unb-money">$1,234,567<span class="unb-money__cents">.89</span></span>""", moneyHero 1234567.891)
    Assert.Equal("""<span class="unb-money">$18,240.55</span>""", money 18240.55)

[<Fact>]
let ``icons expand from the registry, attribute values as JavaScript prints them`` () =
    let html = resolveIcons (registry ()) (icon "bell" + icon "inbox")
    Assert.Contains("""<path d="M6 8a6 6 0 0 1 12 0"/><line x1="10" y1="21" x2="14" y2="21" hidden/>""", html)
    Assert.Contains("""<rect x="0.5" y="1e-7" width="3"/>""", html)
    Assert.Throws<exn>(fun () -> resolveIcons (registry ()) (icon "no-such-icon") |> ignore) |> ignore

[<Fact>]
let ``panel without a title is labelled, with a title is labelled by its heading`` () =
    Assert.Equal("<section class=\"unb-panel\" aria-label=\"L\">\n\n<div class=\"unb-panel__body\">B</div>\n</section>", panel { P with Id = "x"; Label = "L"; Body = "B" })
    Assert.Equal(
        "<section class=\"unb-panel\" data-tone=\"attention\" aria-labelledby=\"x-t\">\n<header class=\"unb-panel__head\"><h3 id=\"x-t\">T</h3>M</header>\n<div class=\"unb-panel__body\">B</div><footer class=\"unb-panel__foot\">F</footer>\n</section>",
        panel { P with Id = "x"; Title = "T"; TitleTag = "h3"; Meta = "M"; Body = "B"; Foot = "F"; Tone = "attention" }
    )

[<Fact>]
let ``a small application page matches ui.mjs byte for byte`` () =
    let page =
        appPage
            { page with
                Title = "Grants received"
                DocTitle = Some "Charity view: Small page"
                Brand = "example-harbor"
                Audience = "charity"
                Nav = "received"
                User = { Name = "Jamie Cole"; Initials = "JC"; Role = Some "Riverside Food Bank" }
                Notifications = 1
                Counts = [ "received", 3 ]
                ExtraCss = Some [ "proposed/ops-charity-widget.css" ]
                Pagehead =
                    pagehead
                        { H with
                            Context = [ """<a href="#received">Grants received</a>"""; "Detail" ]
                            Title = "Grant"
                            Sub = "Riverside Food Bank"
                            Actions = [ primary "Acknowledge" ] }
                Body =
                    notice { N with State = "attention"; Title = "Heads up"; Body = "<p>Text</p>"; Actions = [ quiet "More" "#more" "chevron-down" ] }
                    + "\n"
                    + panel
                        { P with
                            Id = "sum"
                            Title = "Summary"
                            Tone = "attention"
                            Body =
                                chip "ok" "Paid"
                                + money -2500.0
                                + moneySigned 5000.0
                                + moneyHero 1234567.891
                                + steps [ "Approved", "done"; "Paid", "failed"; "Ack", "current" ]
                            Foot = "F" } }
    Assert.Equal(File.ReadAllText(golden "small-page.html"), resolveIcons (registry ()) page)

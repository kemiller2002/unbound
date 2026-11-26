/// verify-v2: verifies every v2 prototype page (pages.json) at five viewports (port of v2/tools/verify.mjs).
///
/// Viewports: phone 390x844, reflow 320x800 (= 1280 px at 400%), zoom200 640x450 (= 1280x900 at 200%),
///            tablet 768x1024, desktop 1280x900.
/// Checks:    axe-core (wcag2a, wcag2aa, wcag21a, wcag21aa, wcag22aa); page horizontal overflow;
///            money/number wrapping (.unb-money, .unb-num must render on one line);
///            target size (WCAG 2.5.8, 24x24 CSS px; inline links in sentences exempt);
///            keyboard walk: focus visible (2.4.7), focus not obscured by fixed bars (2.4.11);
///            heading outline (one h1, no skipped level); text spacing (1.4.12): clipped text after the WCAG
///            spacing overrides.
/// Screenshots: 390 and 1280 are committed reference images (rendered/); others go to OUT_EXTRA.
/// The measurement scripts run inside the page; the checks over their results are the pure functions below.
module Unbound.Designs.Verify.V2

open System
open System.IO
open Unbound.Designs.Verify.JsCompat

type Viewport = { Id: string; Width: int; Height: int; Reference: bool }

let viewports =
    [ { Id = "phone"; Width = 390; Height = 844; Reference = true }
      { Id = "reflow320"; Width = 320; Height = 800; Reference = false }
      { Id = "zoom200"; Width = 640; Height = 450; Reference = false }
      { Id = "tablet"; Width = 768; Height = 1024; Reference = false }
      { Id = "desktop"; Width = 1280; Height = 900; Reference = true } ]

let axeTags = [ "wcag2a"; "wcag2aa"; "wcag21a"; "wcag21aa"; "wcag22aa" ]

let textSpacingCss =
    "*{line-height:1.5!important;letter-spacing:.12em!important;word-spacing:.16em!important}p{margin-block-end:2em!important}"

let viewportJson (v: Viewport) =
    JObj(
        [ "id", str v.Id; "width", jint v.Width; "height", jint v.Height ]
        @ (if v.Reference then [ "reference", JBool true ] else [])
    )

// ---------- in-page measurements (JavaScript evaluated in the page) ----------

let pageChecks = """() => {
  const visible = (el) => { const r = el.getBoundingClientRect(); const cs = getComputedStyle(el); return r.width > 0 && r.height > 0 && cs.visibility !== "hidden" && cs.display !== "none"; };
  const lines = (el) => new Set([...el.getClientRects()].map((r) => Math.round(r.top))).size;
  const wrappedMoney = [...document.querySelectorAll(".unb-money, .unb-num")].filter(visible).filter((el) => lines(el) > 1).map((el) => el.textContent.trim());
  const interactive = [...document.querySelectorAll("a[href], button, input, select, textarea, [tabindex]:not([tabindex='-1']), summary")].filter(visible);
  const inlineInSentence = (el) => el.tagName === "A" && el.closest("p, li") && !el.className && (el.closest("p, li").textContent.trim().length > el.textContent.trim().length + 10);
  const smallTargets = interactive.filter((el) => !inlineInSentence(el)).filter((el) => { const r = el.getBoundingClientRect(); return r.width < 24 || r.height < 24; }).map((el) => `${el.tagName.toLowerCase()} "${(el.getAttribute("aria-label") || el.textContent).trim().slice(0, 40)}" ${Math.round(el.getBoundingClientRect().width)}x${Math.round(el.getBoundingClientRect().height)}`);
  const headings = [...document.querySelectorAll("h1,h2,h3,h4,h5,h6")].filter(visible).map((h) => +h.tagName[1]);
  const skipped = headings.filter((lvl, i) => i > 0 && lvl > headings[i - 1] + 1).length;
  const navLabels = [...document.querySelectorAll("nav")].filter(visible).map((n) => n.getAttribute("aria-label"));
  return {
    overflowX: document.documentElement.scrollWidth > window.innerWidth + 1,
    scrollWidth: document.documentElement.scrollWidth,
    wrappedMoney,
    smallTargets,
    h1Count: headings.filter((l) => l === 1).length,
    skippedHeadingLevels: skipped,
    duplicateNavLabels: navLabels.length !== new Set(navLabels).size,
    mainCount: document.querySelectorAll("main").length,
  };
}"""

let focusProbe = """() => {
  const el = document.activeElement;
  if (!el || el === document.body) return null;
  const r = el.getBoundingClientRect();
  const cs = getComputedStyle(el);
  const visibleIndicator = (cs.outlineStyle !== "none" && parseFloat(cs.outlineWidth) >= 2) || (cs.boxShadow && cs.boxShadow !== "none");
  const cx = Math.min(Math.max(r.left + r.width / 2, 0), window.innerWidth - 1);
  const cy = Math.min(Math.max(r.top + r.height / 2, 0), window.innerHeight - 1);
  const top = document.elementFromPoint(cx, cy);
  const obscured = top && !(top === el || el.contains(top) || top.contains(el));
  return { name: (el.getAttribute("aria-label") || el.textContent || "").trim().replace(/\s+/g, " ").slice(0, 50), tag: el.tagName.toLowerCase(), y: Math.round(r.top + window.scrollY), x: Math.round(r.left), visibleIndicator, obscured: !!obscured, obscuredBy: obscured ? top.className || top.tagName : null };
}"""

let clippedAfterSpacing = """() =>
  [...document.querySelectorAll("body *")].filter((el) => el.getBoundingClientRect().width > 2 && el.getBoundingClientRect().height > 2).filter((el) => { const cs = getComputedStyle(el); return /(hidden|clip)/.test(cs.overflow + cs.overflowY + cs.overflowX) && (el.scrollHeight > el.clientHeight + 2 || el.scrollWidth > el.clientWidth + 2) && el.textContent.trim(); }).map((el) => el.className || el.tagName).slice(0, 10)"""

// axe marks some contrast cases "incomplete" (e.g. single characters, v1 audit UXD-002);
// those are treated as failures unless their measured ratio passes.
let axeRun = """async (tags) => {
    const r = await window.axe.run(document, { runOnly: { type: "tag", values: tags } });
    return {
      violations: r.violations.map((v) => ({ id: v.id, impact: v.impact, nodes: v.nodes.length, target: v.nodes[0]?.target?.join(" ") })),
      contrastIncomplete: r.incomplete.filter((v) => v.id === "color-contrast").flatMap((v) => v.nodes).filter((n) => { const d = n.any?.[0]?.data; return d && d.contrastRatio && d.contrastRatio < (parseFloat(d.expectedContrastRatio) || 4.5); }).map((n) => n.target.join(" ")),
    };
  }"""

// ---------- pure checks over the measurements ----------

/// One keyboard-walk step: Some(next focus list) to continue, None to stop (no focus, or focus wrapped to the start).
let walkStep (acc: Json list) (probe: Json) : Json list option =
    match probe, acc with
    | JNull, _ -> None
    | f, first :: _ when first ? name = f ? name && first ? tag = f ? tag && acc.Length > 3 -> None
    | f, _ -> Some(acc @ [ f ])

/// axe findings: violations plus the incomplete contrast nodes whose measured ratio fails.
let axeFindings (run: Json) : Json list =
    asList (run ? violations)
    @ (asList (run ? contrastIncomplete)
       |> List.map (fun t -> JObj [ "id", str "color-contrast-incomplete"; "impact", str "serious"; "nodes", jint 1; "target", t ]))

let private s (j: Json) = match j with JStr x -> x | JNull -> "null" | JNum n -> numberToString n | JBool b -> (if b then "true" else "false") | other -> stringify 0 other

/// Text of each error event as the original records it (console errors and failed requests).
let errorTexts (events: Cdp.PageEvent list) =
    events
    |> List.choose (function
        | Cdp.Console("error", t) -> Some t
        | Cdp.RequestFailed url -> Some("failed: " + url)
        | _ -> None)

/// The failure list for one page at one viewport.
let failures (axe: Json list) (pageBase: Json) (focus: Json list) (spacing: Json) (errors: string list) : string list =
    let h1 = asNum (pageBase ? h1Count)
    let skipped = asNum (pageBase ? skippedHeadingLevels)
    [ yield! axe |> List.map (fun a -> "axe:" + s (a ? id))
      if truthy (pageBase ? overflowX) then "overflow-x"
      yield! asList (pageBase ? wrappedMoney) |> List.map (fun m -> "money-wraps:" + s m)
      yield! asList (pageBase ? smallTargets) |> List.map (fun t -> "target<24:" + s t)
      if h1 <> 1.0 then "h1-count:" + numberToString h1
      if skipped <> 0.0 then "heading-skip:" + numberToString skipped
      if truthy (pageBase ? duplicateNavLabels) then "duplicate-nav-label"
      yield! focus |> List.filter (fun f -> not (truthy (f ? visibleIndicator))) |> List.map (fun f -> "focus-not-visible:" + s (f ? name))
      yield! focus |> List.filter (fun f -> truthy (f ? obscured)) |> List.map (fun f -> $"focus-obscured:{s (f ? name)} (by {s (f ? obscuredBy)})")
      yield! asList (spacing ? clipped) |> List.map (fun c -> "text-spacing-clipped:" + s c)
      if truthy (spacing ? overflowX) then "text-spacing-overflow-x"
      yield! errors |> List.map (fun e -> "console:" + e) ]

let viewportResult (vp: Viewport) (axe: Json list) (pageBase: Json) (focus: Json list) (spacing: Json) (errors: string list) : Json =
    JObj [
        "viewport", str vp.Id
        "width", jint vp.Width
        "failures", strs (failures axe pageBase focus spacing errors)
        "axe", JArr axe
        "base", pageBase
        "focusOrder", JArr(focus |> List.map (fun f -> f ? name))
        "spacing", spacing
    ]

let private failuresOf (r: Json) = asList (r ? failures) |> List.map asStr

/// { ...page, results, pass }
let pageResult (page: Json) (perVp: Json list) : Json =
    let entries = match page with JObj kvs -> kvs | _ -> []
    obj (entries @ [ "results", JArr perVp; "pass", JBool(perVp |> List.forall (failuresOf >> List.isEmpty)) ])

/// The console line for one page.
let pageLine (page: Json) (perVp: Json list) : string =
    let ok = perVp |> List.forall (failuresOf >> List.isEmpty)
    let shown = perVp |> List.collect (fun r -> failuresOf r |> List.map (fun f -> $"[{asStr (r ? viewport)}] {f}")) |> List.truncate 6
    $"""{(if ok then "PASS" else "FAIL")} {s (page ? screen)}/{s (page ? state)} {String.Join(" | ", shown)}"""

/// Prior pages not re-checked this run, plus this run's, ordered by file (localeCompare, stable).
let merge (prior: Json list) (results: Json list) : Json list =
    let fresh = results |> List.map (fun r -> r ? file) |> Set.ofList
    (prior |> List.filter (fun x -> not (fresh.Contains(x ? file)))) @ results
    |> List.sortWith (fun a b -> localeCompare (s (a ? file)) (s (b ? file)))

let report (checkedAt: string) (merged: Json list) : Json =
    JObj [
        "checked_at", str checkedAt
        "viewports", JArr(viewports |> List.map viewportJson)
        "axe_tags", strs axeTags
        "pages", JArr merged
        "passing", jint (merged |> List.filter (fun r -> truthy (r ? pass)) |> List.length)
    ]

/// Content types of the original static server.
let contentType (ext: string) =
    Map [ ".html", "text/html"; ".css", "text/css"; ".woff2", "font/woff2"; ".js", "text/javascript"; ".json", "application/json"; ".png", "image/png"; ".svg", "image/svg+xml" ]
    |> Map.tryFind ext
    |> Option.orElse (Some "application/octet-stream")

// ---------- orchestration (I/O) ----------

type Settings = { Root: string; Axe: string; OutExtra: string; Only: string list option }

let private walkKeyboard (page: Cdp.Page) =
    task {
        let mutable acc = []
        let mutable go = true
        let mutable i = 0
        while go && i < 120 do
            do! Cdp.press page "Tab"
            let! f = Cdp.call page focusProbe []
            match walkStep acc f with
            | Some next -> acc <- next; i <- i + 1
            | None -> go <- false
        return acc
    }

let private checkPage (browser: Cdp.Browser) (settings: Settings) (baseUrl: string) (axeSrc: string) (p: Json) (vp: Viewport) =
    task {
        let! page = Cdp.newPage browser { Width = vp.Width; Height = vp.Height; DeviceScaleFactor = 1.0; ReducedMotion = Some "reduce" }
        do! Cdp.goto page $"{baseUrl}/v2/{asStr (p ? file)}"
        do! Cdp.evaluate page "document.fonts.ready.then(() => undefined)" :> Threading.Tasks.Task
        let! pageBase = Cdp.call page pageChecks []
        do! Cdp.addScriptTag page axeSrc
        let! run = Cdp.call page axeRun [ strs axeTags ]
        let! focus = walkKeyboard page
        let shotDir = if vp.Reference then Path.Combine(settings.Root, "rendered", asStr (p ? screen)) else Path.Combine(settings.OutExtra, asStr (p ? screen))
        Directory.CreateDirectory shotDir |> ignore
        do! Cdp.evaluate page "window.scrollTo(0, 0)" :> Threading.Tasks.Task
        let! png = Cdp.screenshot page true
        Cdp.save (Path.Combine(shotDir, $"{asStr (p ? state)}-{vp.Width}.png")) png
        do! Cdp.addStyleTag page textSpacingCss
        let! clipped = Cdp.call page clippedAfterSpacing []
        let! after1 = Cdp.call page pageChecks []
        let! after2 = Cdp.call page pageChecks []
        let spacing = JObj [ "clipped", clipped; "overflowX", after1 ? overflowX; "wrappedMoney", after2 ? wrappedMoney ]
        do! Cdp.closePage page
        return viewportResult vp (axeFindings run) pageBase focus spacing (errorTexts (Cdp.events page))
    }

let run (settings: Settings) : int =
    let pages =
        asList (parse (File.ReadAllText(Path.Combine(settings.Root, "pages.json"))))
        |> List.filter (fun p -> match settings.Only with None -> true | Some only -> List.contains (asStr (p ? screen)) only)
    let axeSrc = File.ReadAllText settings.Axe
    let designsRoot = Path.GetFullPath(Path.Combine(settings.Root, ".."))
    use server = StaticServer.start (StaticServer.fileUnder designsRoot contentType)
    let baseUrl = $"http://127.0.0.1:{server.Port}"
    let browser = (Cdp.launch ()).Result
    let results =
        pages
        |> List.map (fun p ->
            let perVp = viewports |> List.map (fun vp -> (checkPage browser settings baseUrl axeSrc p vp).Result)
            Console.WriteLine(pageLine p perVp)
            pageResult p perVp)
    (Cdp.close browser).Wait()
    let verification = Path.Combine(settings.Root, "verification.json")
    let prior = try asList ((parse (File.ReadAllText verification)) ? pages) with _ -> []
    let merged = merge prior results
    let out = report (isoString DateTime.UtcNow) merged
    File.WriteAllText(verification, stringify 2 out + "\n")
    Console.WriteLine $"{numberToString (asNum (out ? passing))}/{merged.Length} pages pass all checks at all viewports"
    0

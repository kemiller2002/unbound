/// A small Chrome DevTools Protocol client: launches the headless shell that Playwright's chromium.launch() uses,
/// with Playwright's default switches, and offers the page operations the verifiers need, each issuing the same
/// protocol calls Playwright 1.63 issues for page.goto / evaluate / addScriptTag / addStyleTag / keyboard.press /
/// screenshot / pdf. This module is the I/O edge; everything else in the tool is pure.
module Unbound.Designs.Verify.Cdp

open System
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Net.WebSockets
open System.Text
open System.Text.Json
open System.Threading
open System.Threading.Tasks
open Unbound.Designs.Verify.JsCompat

/// The binary Playwright's chromium.launch() (headless) starts in this environment.
let defaultExecutable =
    "/opt/pw-browsers/chromium_headless_shell-1243/chrome-headless-shell-linux64/chrome-headless-shell"

/// Playwright 1.63's default Chromium switches for a headless launch (chromiumSwitches plus the headless additions),
/// in the same order. Playwright's --remote-debugging-pipe is replaced by --remote-debugging-port=0.
let launchArgs (userDataDir: string) =
    [ "--disable-field-trial-config"
      "--disable-background-networking"
      "--disable-background-timer-throttling"
      "--disable-backgrounding-occluded-windows"
      "--disable-back-forward-cache"
      "--disable-breakpad"
      "--disable-client-side-phishing-detection"
      "--disable-component-extensions-with-background-pages"
      "--disable-component-update"
      "--no-default-browser-check"
      "--disable-default-apps"
      "--disable-dev-shm-usage"
      "--disable-edgeupdater"
      "--disable-extensions"
      "--disable-features=AvoidUnnecessaryBeforeUnloadCheckSync,DestroyProfileOnBrowserClose,DialMediaRouteProvider,GlobalMediaControls,HttpsUpgrades,LensOverlay,MediaRouter,PaintHolding,ThirdPartyStoragePartitioning,BlockOriginHeaderModificationOnRedirect,Translate,AutoDeElevate,OptimizationHints,msForceBrowserSignIn,msEdgeUpdateLaunchServicesPreferredVersion"
      "--enable-features=CDPScreenshotNewSurface"
      "--allow-pre-commit-input"
      "--disable-hang-monitor"
      "--disable-ipc-flooding-protection"
      "--disable-popup-blocking"
      "--disable-prompt-on-repost"
      "--disable-renderer-backgrounding"
      "--disable-updater-scheduler"
      "--force-color-profile=srgb"
      "--metrics-recording-only"
      "--no-first-run"
      "--password-store=basic"
      "--use-mock-keychain"
      "--no-service-autorun"
      "--export-tagged-pdf"
      "--disable-search-engine-choice-screen"
      "--unsafely-disable-devtools-self-xss-warnings"
      "--edge-skip-compat-layer-relaunch"
      "--disable-infobars"
      "--disable-search-engine-choice-screen"
      "--disable-sync"
      "--enable-unsafe-swiftshader"
      "--headless"
      "--hide-scrollbars"
      "--mute-audio"
      "--blink-settings=primaryHoverType=2,availableHoverTypes=2,primaryPointerType=4,availablePointerTypes=4"
      "--no-sandbox"
      $"--user-data-dir={userDataDir}"
      "--remote-debugging-port=0"
      "--no-startup-window" ]

exception CdpError of string

// ---------- connection ----------

/// A task completed once per key, whichever of producer and consumer comes first.
type Latch<'T>() =
    let slots = ConcurrentDictionary<string, TaskCompletionSource<'T>>()
    member _.Slot(key: string) = slots.GetOrAdd(key, fun _ -> TaskCompletionSource<'T>(TaskCreationOptions.RunContinuationsAsynchronously))
    member this.Set(key: string, value: 'T) = this.Slot(key).TrySetResult value |> ignore
    member this.Wait(key: string) = this.Slot(key).Task

/// One WebSocket to the browser endpoint, multiplexing flattened target sessions.
type Connection(ws: ClientWebSocket) =
    let pending = ConcurrentDictionary<int, TaskCompletionSource<Json>>()
    let listeners = ConcurrentDictionary<string, string -> Json -> unit>()
    let attached = Latch<string>()
    let sendLock = new SemaphoreSlim(1, 1)
    let mutable nextId = 0
    let mutable closed = false

    let dispatch (root: JsonElement) =
        let mutable idEl = Unchecked.defaultof<JsonElement>
        if root.TryGetProperty("id", &idEl) then
            match pending.TryRemove(idEl.GetInt32()) with
            | true, tcs ->
                let mutable err = Unchecked.defaultof<JsonElement>
                if root.TryGetProperty("error", &err) then
                    tcs.TrySetException(CdpError(err.GetProperty("message").GetString())) |> ignore
                else
                    let mutable res = Unchecked.defaultof<JsonElement>
                    tcs.TrySetResult(if root.TryGetProperty("result", &res) then ofElement res else JObj []) |> ignore
            | _ -> ()
        else
            let methodName = root.GetProperty("method").GetString()
            let mutable p = Unchecked.defaultof<JsonElement>
            let pars = if root.TryGetProperty("params", &p) then ofElement p else JObj []
            let mutable s = Unchecked.defaultof<JsonElement>
            let sid = if root.TryGetProperty("sessionId", &s) then s.GetString() else ""
            if sid = "" && methodName = "Target.attachedToTarget" then
                attached.Set(asStr (pars ? targetInfo ? targetId), asStr (pars ? sessionId))
            match listeners.TryGetValue sid with
            | true, f -> f methodName pars
            | _ -> ()

    let receiveLoop () =
        task {
            let buffer = Array.zeroCreate<byte> (1 <<< 20)
            use ms = new MemoryStream()
            try
                while ws.State = WebSocketState.Open do
                    let! r = ws.ReceiveAsync(ArraySegment buffer, CancellationToken.None)
                    if r.MessageType = WebSocketMessageType.Close then
                        closed <- true
                    else
                        ms.Write(buffer, 0, r.Count)
                        if r.EndOfMessage then
                            let bytes = ms.ToArray()
                            ms.SetLength 0L
                            use doc = JsonDocument.Parse(ReadOnlyMemory bytes, JsonDocumentOptions(MaxDepth = 1024))
                            dispatch doc.RootElement
            with _ -> ()
            closed <- true
            for KeyValue(_, tcs) in pending do
                tcs.TrySetException(CdpError "connection closed") |> ignore
        }

    member _.Start() = receiveLoop () |> ignore

    member _.Send(methodName: string, pars: (string * Json) list, ?sessionId: string) : Task<Json> =
        task {
            if closed then raise (CdpError "connection closed")
            let id = Interlocked.Increment(&nextId)
            let tcs = TaskCompletionSource<Json>(TaskCreationOptions.RunContinuationsAsynchronously)
            pending.[id] <- tcs
            let msg =
                JObj(
                    [ "id", JNum(float id); "method", JStr methodName; "params", JObj pars ]
                    @ (match sessionId with Some s -> [ "sessionId", JStr s ] | None -> [])
                )
                |> stringify 0
            do! sendLock.WaitAsync()
            try
                do! ws.SendAsync(ArraySegment(Encoding.UTF8.GetBytes msg), WebSocketMessageType.Text, true, CancellationToken.None)
            finally
                sendLock.Release() |> ignore
            return! tcs.Task
        }

    /// The flattened session id of an auto-attached target.
    member _.Attached(targetId: string) : Task<string> = attached.Wait targetId

    member _.Listen(sessionId: string, f: string -> Json -> unit) = listeners.[sessionId] <- f
    member _.Unlisten(sessionId: string) = listeners.TryRemove sessionId |> ignore
    member _.Socket = ws

// ---------- browser ----------

let private tryKill (p: Process) = try p.Kill true with _ -> ()
let private tryDispose (d: IDisposable) = try d.Dispose() with _ -> ()

type Browser =
    { Process: Process
      Conn: Connection
      UserDataDir: string
      /// browser.version(), e.g. "158.0.8127.36"
      Version: string }

/// chromium.launch(): starts the headless shell and connects to its DevTools endpoint.
let launch () : Task<Browser> =
    task {
        let exe = Environment.GetEnvironmentVariable "UNBOUND_CHROMIUM" |> Option.ofObj |> Option.defaultValue defaultExecutable
        let userDataDir = Path.Combine(Path.GetTempPath(), "unbound-cdp-profile-" + Guid.NewGuid().ToString("N").Substring(0, 12))
        Directory.CreateDirectory userDataDir |> ignore
        let psi = ProcessStartInfo(exe, launchArgs userDataDir)
        psi.RedirectStandardError <- true
        psi.RedirectStandardOutput <- true
        psi.RedirectStandardInput <- true
        psi.UseShellExecute <- false
        let proc = Process.Start psi
        let endpoint = TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously)
        proc.ErrorDataReceived.Add(fun e ->
            match e.Data with
            | null -> endpoint.TrySetException(CdpError "browser exited before its DevTools endpoint was ready") |> ignore
            | line when line.StartsWith "DevTools listening on " -> endpoint.TrySetResult(line.Substring("DevTools listening on ".Length).Trim()) |> ignore
            | _ -> ())
        proc.OutputDataReceived.Add ignore
        proc.BeginErrorReadLine()
        proc.BeginOutputReadLine()
        let! first = Task.WhenAny(endpoint.Task :> Task, Task.Delay 30000)
        if not (Object.ReferenceEquals(first, endpoint.Task)) then
            tryKill proc
            raise (CdpError "timed out waiting for the browser's DevTools endpoint")
        let! url = endpoint.Task
        let ws = new ClientWebSocket()
        ws.Options.Proxy <- null
        ws.Options.KeepAliveInterval <- TimeSpan.Zero
        do! ws.ConnectAsync(Uri url, CancellationToken.None)
        let conn = Connection ws
        conn.Start()
        let! v = conn.Send("Browser.getVersion", [])
        let product = asStr (v ? product)
        do! conn.Send("Target.setAutoAttach", [ "autoAttach", JBool true; "waitForDebuggerOnStart", JBool true; "flatten", JBool true ]) :> Task
        return { Process = proc; Conn = conn; UserDataDir = userDataDir; Version = product.Substring(product.IndexOf '/' + 1) }
    }

/// browser.close()
let close (b: Browser) : Task =
    task {
        try
            let! _ = Task.WhenAny(b.Conn.Send("Browser.close", []) :> Task, Task.Delay 5000)
            ()
        with _ -> ()
        let! _ = Task.WhenAny(b.Process.WaitForExitAsync(), Task.Delay 10000)
        if not b.Process.HasExited then tryKill b.Process
        tryDispose b.Conn.Socket
        for _ in 1..5 do
            if Directory.Exists b.UserDataDir then
                try Directory.Delete(b.UserDataDir, true) with _ -> Thread.Sleep 200
    }

// ---------- pages ----------

type ContextOptions =
    { Width: int
      Height: int
      DeviceScaleFactor: float
      /// Some "reduce" for reducedMotion: "reduce"; None for Playwright's default ("no-preference").
      ReducedMotion: string option }

/// What a page reported while it was open, in arrival order.
type PageEvent =
    /// page.on("console"): message type and text().
    | Console of kind: string * text: string
    /// page.on("requestfailed"): request url.
    | RequestFailed of url: string
    /// page.on("pageerror"): String(error).
    | PageError of text: string

type Page =
    { Conn: Connection
      SessionId: string
      BrowserContextId: string
      FrameId: string
      Options: ContextOptions
      Events: ConcurrentQueue<PageEvent>
      Loads: Latch<unit> }

let private send (p: Page) m pars = p.Conn.Send(m, pars, p.SessionId)

let private remotePreview (o: Json) =
    match o ? ``type``, prop "value" o, prop "unserializableValue" o with
    | _, _, Some(JStr u) -> u
    | JStr "undefined", _, _ -> "undefined"
    | _, Some(JStr s), _ -> s
    | _, Some(JNum n), _ -> numberToString n
    | _, Some(JBool b), _ -> (if b then "true" else "false")
    | _, Some JNull, _ -> "null"
    | _ -> (match o ? description with JStr d -> d | _ -> "JSHandle@" + (match o ? ``type`` with JStr t -> t | _ -> "object"))

/// String(error) for an error Playwright builds from Runtime.exceptionThrown (exceptionToError).
let exceptionText (details: Json) =
    let withStack =
        match prop "exception" details with
        | Some ex -> (match ex ? description with JStr d -> d | _ -> remotePreview ex)
        | None ->
            let frames = match details ? stackTrace with JObj _ as st -> asList (st ? callFrames) | _ -> []
            (match details ? text with JStr t -> t | _ -> "")
            + (frames
               |> List.map (fun f ->
                   let fn = match f ? functionName with JStr "" | JNull -> "<anonymous>" | j -> asStr j
                   $"\n    at {fn} ({asStr (f ? url)}:{numberToString (asNum (f ? lineNumber))}:{numberToString (asNum (f ? columnNumber))})")
               |> String.concat "")
    let lines = withStack.Split '\n'
    let first = lines |> Array.tryFindIndex (fun l -> l.StartsWith "    at")
    let messageWithName = match first with Some i -> String.Join("\n", lines.[.. i - 1]) | None -> withStack
    let sep = messageWithName.IndexOf ':'
    let name0 = if sep <> -1 then messageWithName.Substring(0, sep) else ""
    let message = if sep <> -1 && sep + 2 <= messageWithName.Length then messageWithName.Substring(sep + 2) else messageWithName
    let nameOverride =
        match (prop "exception" details |> Option.defaultValue JNull) ? preview ? properties with
        | JArr ps -> ps |> List.tryFind (fun p -> p ? name = JStr "name") |> Option.map (fun p -> match p ? value with JStr v -> v | _ -> "Error")
        | _ -> None
    let name = nameOverride |> Option.defaultValue name0
    match name, message with
    | "", m -> m
    | n, "" -> n
    | n, m -> n + ": " + m

/// browser.newContext(options) + context.newPage(), with the protocol set-up Playwright performs.
let newPage (b: Browser) (o: ContextOptions) : Task<Page> =
    task {
        let! ctx = b.Conn.Send("Target.createBrowserContext", [ "disposeOnDetach", JBool true ])
        let contextId = asStr (ctx ? browserContextId)
        let! target = b.Conn.Send("Target.createTarget", [ "url", JStr "about:blank"; "browserContextId", JStr contextId ])
        let targetId = asStr (target ? targetId)
        let! sid = b.Conn.Attached targetId
        let events = ConcurrentQueue<PageEvent>()
        let loads = Latch<unit>()
        let requests = ConcurrentDictionary<string, string>()
        b.Conn.Listen(
            sid,
            fun m p ->
                match m with
                | "Runtime.consoleAPICalled" when asNum (p ? executionContextId) <> 0.0 ->
                    let kind = match asStr (p ? ``type``) with "warning" -> "warning" | t -> t
                    events.Enqueue(Console(kind, asList (p ? args) |> List.map remotePreview |> String.concat " "))
                | "Log.entryAdded" when p ? entry ? source <> JStr "worker" ->
                    events.Enqueue(Console(asStr (p ? entry ? level), asStr (p ? entry ? text)))
                | "Page.lifecycleEvent" when p ? name = JStr "load" -> loads.Set(asStr (p ? loaderId) + "|" + asStr (p ? frameId), ())
                | "Runtime.exceptionThrown" -> events.Enqueue(PageError(exceptionText (p ? exceptionDetails)))
                | "Network.requestWillBeSent" -> requests.[asStr (p ? requestId)] <- asStr (p ? request ? url)
                | "Network.loadingFailed" ->
                    match requests.TryGetValue(asStr (p ? requestId)) with
                    | true, url -> events.Enqueue(RequestFailed url)
                    | _ -> ()
                | _ -> ()
        )
        let s m pars = b.Conn.Send(m, pars, sid)
        let! win = s "Browser.getWindowForTarget" []
        do! s "Page.enable" [] :> Task
        let! tree = s "Page.getFrameTree" []
        let frameId = asStr (tree ? frameTree ? frame ? id)
        do! s "Log.enable" [] :> Task
        do! s "Page.setLifecycleEventsEnabled" [ "enabled", JBool true ] :> Task
        do! s "Runtime.enable" [] :> Task
        do! s "Network.enable" [] :> Task
        do! s "Emulation.setFocusEmulationEnabled" [ "enabled", JBool true ] :> Task
        do! s "Browser.setWindowBounds" [ "windowId", win ? windowId; "bounds", JObj [ "width", jint o.Width; "height", jint o.Height ] ] :> Task
        do!
            s "Emulation.setDeviceMetricsOverride" [
                "mobile", JBool false
                "width", jint o.Width
                "height", jint o.Height
                "screenWidth", jint o.Width
                "screenHeight", jint o.Height
                "deviceScaleFactor", JNum o.DeviceScaleFactor
                "screenOrientation", JObj [ "angle", JNum 0.0; "type", JStr "landscapePrimary" ]
            ]
            :> Task
        do!
            s "Page.setFontFamilies" [
                "fontFamilies",
                JObj [
                    "standard", JStr "Times New Roman"
                    "fixed", JStr "Monospace"
                    "serif", JStr "Times New Roman"
                    "sansSerif", JStr "Arial"
                    "cursive", JStr "Comic Sans MS"
                    "fantasy", JStr "Impact"
                ]
            ]
            :> Task
        do!
            s "Emulation.setEmulatedMedia" [
                "media", JStr ""
                "features",
                JArr [
                    JObj [ "name", JStr "prefers-color-scheme"; "value", JStr "light" ]
                    JObj [ "name", JStr "prefers-reduced-motion"; "value", JStr(defaultArg o.ReducedMotion "no-preference") ]
                    JObj [ "name", JStr "forced-colors"; "value", JStr "none" ]
                    JObj [ "name", JStr "prefers-contrast"; "value", JStr "no-preference" ]
                ]
            ]
            :> Task
        do! s "Runtime.runIfWaitingForDebugger" [] :> Task
        return { Conn = b.Conn; SessionId = sid; BrowserContextId = contextId; FrameId = frameId; Options = o; Events = events; Loads = loads }
    }

/// context.close()
let closePage (p: Page) : Task =
    task {
        do! p.Conn.Send("Target.disposeBrowserContext", [ "browserContextId", JStr p.BrowserContextId ]) :> Task
        p.Conn.Unlisten p.SessionId
    }

/// Events reported so far, in order.
let events (p: Page) : PageEvent list = p.Events.ToArray() |> List.ofArray

/// page.goto(url) with waitUntil "load".
let goto (p: Page) (url: string) : Task =
    task {
        let! nav = send p "Page.navigate" [ "url", JStr url; "frameId", JStr p.FrameId; "referrerPolicy", JStr "unsafeUrl" ]
        match nav ? errorText with
        | JStr e when e <> "" -> raise (CdpError $"page.goto: {e} at {url}")
        | _ -> ()
        do! p.Loads.Wait(asStr (nav ? loaderId) + "|" + p.FrameId)
    }

/// Runtime.evaluate of an expression in the page's main world, awaiting promises and returning the value as JSON
/// (page.evaluate semantics; undefined reads as null).
let evaluate (p: Page) (expression: string) : Task<Json> =
    task {
        let! r = send p "Runtime.evaluate" [ "expression", JStr expression; "returnByValue", JBool true; "awaitPromise", JBool true; "userGesture", JBool true ]
        match prop "exceptionDetails" r with
        | Some d -> return raise (CdpError("page.evaluate: " + exceptionText d))
        | None -> return (r ? result ? value)
    }

/// page.evaluate(fn, arg): calls a function given as JavaScript source with JSON arguments.
let call (p: Page) (fnSource: string) (args: Json list) : Task<Json> =
    evaluate p ("(" + fnSource + ")(" + (args |> List.map (stringify 0) |> String.concat ", ") + ")")

/// page.evaluate(source) for a script given as a string (evaluated as a global script by indirect eval, as Playwright does).
let evaluateScript (p: Page) (source: string) : Task =
    task {
        let! r = send p "Runtime.evaluate" [ "expression", JStr("void globalThis.eval(" + stringify 0 (JStr source) + ")"); "awaitPromise", JBool true; "userGesture", JBool true ]
        match prop "exceptionDetails" r with
        | Some d -> raise (CdpError("page.evaluate: " + exceptionText d))
        | None -> ()
    }

/// page.addScriptTag({ content })
let addScriptTag (p: Page) (content: string) : Task =
    call p """(function addScriptContent(params3) {
          const script = document.createElement("script");
          script.type = params3.type || "text/javascript";
          script.text = params3.content;
          let error = null;
          script.onerror = (e) => error = e;
          document.head.appendChild(script);
          if (error)
            throw error;
          return true;
        })""" [ JObj [ "content", JStr content; "type", JStr "" ] ]
    :> Task

/// page.addStyleTag({ content })
let addStyleTag (p: Page) (content: string) : Task =
    call p """(async function addStyleContent(content2) {
          const style = document.createElement("style");
          style.type = "text/css";
          style.appendChild(document.createTextNode(content2));
          const promise = new Promise((res, rej) => {
            style.onload = res;
            style.onerror = rej;
          });
          document.head.appendChild(style);
          await promise;
          return true;
        })""" [ JStr content ]
    :> Task

let private keyCodes = Map [ "Tab", 9; "Escape", 27 ]

/// page.keyboard.press(key) for the keys the verifiers use (Tab, Escape).
let press (p: Page) (key: string) : Task =
    task {
        let code = keyCodes.[key]
        do!
            send p "Input.dispatchKeyEvent" [
                "type", JStr "rawKeyDown"; "modifiers", JNum 0.0; "windowsVirtualKeyCode", jint code; "code", JStr key; "commands", JArr []
                "key", JStr key; "text", JStr ""; "unmodifiedText", JStr ""; "autoRepeat", JBool false; "location", JNum 0.0; "isKeypad", JBool false
            ]
            :> Task
        do! send p "Input.dispatchKeyEvent" [ "type", JStr "keyUp"; "modifiers", JNum 0.0; "key", JStr key; "windowsVirtualKeyCode", jint code; "code", JStr key; "location", JNum 0.0 ] :> Task
    }

/// page.waitForTimeout(ms)
let waitForTimeout (ms: int) : Task = Task.Delay ms

// Playwright's inPagePrepareForScreenshots(undefined, hideCaret = true, false, false), applied to the document and to
// every same-origin frame (Playwright runs it in each frame).
let private prepareForScreenshot = """(() => {
  const cleanups = [];
  const prep = (doc) => {
    const collectRoots = (root, roots2 = []) => {
      roots2.push(root);
      const walker = doc.createTreeWalker(root, NodeFilter.SHOW_ELEMENT);
      do {
        const node = walker.currentNode;
        const shadowRoot = node.nodeType === 1 ? node.shadowRoot : null;
        if (shadowRoot)
          collectRoots(shadowRoot, roots2);
      } while (walker.nextNode());
      return roots2;
    };
    const roots = collectRoots(doc);
    const elements = new Map();
    for (const root of roots) {
      root.querySelectorAll("input,textarea,[contenteditable]").forEach((element3) => {
        elements.set(element3, { value: element3.style.getPropertyValue("caret-color"), priority: element3.style.getPropertyPriority("caret-color") });
        element3.style.setProperty("caret-color", "transparent", "important");
      });
    }
    cleanups.push(() => { for (const [element3, value2] of elements) element3.style.setProperty("caret-color", value2.value, value2.priority); });
    for (const f of doc.querySelectorAll("iframe,frame")) { try { if (f.contentDocument) prep(f.contentDocument); } catch (e) {} }
  };
  prep(document);
  window.__unboundCleanupScreenshot = () => { for (const c of cleanups) c(); delete window.__unboundCleanupScreenshot; };
})()"""

let private fullPageSize = """(() => {
  if (!document.body || !document.documentElement) return null;
  return {
    width: Math.max(document.body.scrollWidth, document.documentElement.scrollWidth, document.body.offsetWidth, document.documentElement.offsetWidth, document.body.clientWidth, document.documentElement.clientWidth),
    height: Math.max(document.body.scrollHeight, document.documentElement.scrollHeight, document.body.offsetHeight, document.documentElement.offsetHeight, document.body.clientHeight, document.documentElement.clientHeight)
  };
})()"""

/// page.screenshot({ fullPage }) as PNG bytes (Playwright's Screenshotter.screenshotPage + CRPage.takeScreenshot).
let screenshot (p: Page) (fullPage: bool) : Task<byte[]> =
    task {
        let vw, vh = float p.Options.Width, float p.Options.Height
        do! evaluate p prepareForScreenshot :> Task
        try
            do! evaluate p "document.fonts.ready.then(() => undefined)" :> Task
            let! clip, beyond =
                task {
                    if fullPage then
                        let! size = evaluate p fullPageSize
                        let w, h = asNum (size ? width), asNum (size ? height)
                        let! _ = send p "Page.getLayoutMetrics" []
                        return JObj [ "x", JNum 0.0; "y", JNum 0.0; "width", JNum w; "height", JNum h; "scale", JNum 1.0 ], not (w <= vw && h <= vh)
                    else
                        let! m = send p "Page.getLayoutMetrics" []
                        let vv = m ? visualViewport
                        let scale = asNum (vv ? scale)
                        return
                            JObj [
                                "x", vv ? pageX
                                "y", vv ? pageY
                                "width", JNum(floor (vw / scale + 1e-3))
                                "height", JNum(floor (vh / scale + 1e-3))
                                "scale", JNum scale
                            ],
                            false
                }
            let! r = send p "Page.captureScreenshot" [ "format", JStr "png"; "clip", clip; "captureBeyondViewport", JBool beyond; "optimizeForSpeed", JBool true ]
            return Convert.FromBase64String(asStr (r ? data))
        finally
            (evaluate p "window.__unboundCleanupScreenshot && window.__unboundCleanupScreenshot()").Wait()
    }

/// page.pdf({ format: "Letter", printBackground: true, preferCSSPageSize: true, tagged }) as bytes.
let pdf (p: Page) (tagged: bool) : Task<byte[]> =
    task {
        let! r =
            send p "Page.printToPDF" [
                "transferMode", JStr "ReturnAsStream"; "landscape", JBool false; "displayHeaderFooter", JBool false
                "headerTemplate", JStr ""; "footerTemplate", JStr ""; "printBackground", JBool true; "scale", JNum 1.0
                "paperWidth", JNum 8.5; "paperHeight", JNum 11.0; "marginTop", JNum 0.0; "marginBottom", JNum 0.0
                "marginLeft", JNum 0.0; "marginRight", JNum 0.0; "pageRanges", JStr ""; "preferCSSPageSize", JBool true
                "generateTaggedPDF", JBool tagged; "generateDocumentOutline", JBool false
            ]
        let handle = r ? stream
        use ms = new MemoryStream()
        let mutable eof = false
        while not eof do
            let! chunk = send p "IO.read" [ "handle", handle ]
            eof <- asBool (chunk ? eof)
            let data = asStr (chunk ? data)
            let bytes = if chunk ? base64Encoded = JBool true then Convert.FromBase64String data else Encoding.UTF8.GetBytes data
            ms.Write(bytes, 0, bytes.Length)
        do! send p "IO.close" [ "handle", handle ] :> Task
        return ms.ToArray()
    }

/// Writes bytes to a path, creating its directory (Playwright's `path` option).
let save (path: string) (bytes: byte[]) =
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath path)) |> ignore
    File.WriteAllBytes(path, bytes)

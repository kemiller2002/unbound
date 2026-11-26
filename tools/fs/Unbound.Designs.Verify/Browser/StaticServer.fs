/// A minimal HTTP/1.1 file server on 127.0.0.1 (an ephemeral port), so pages load over http as in the originals.
/// The response for a path is a pure function supplied by the caller; this module only does the socket I/O.
module Unbound.Designs.Verify.StaticServer

open System
open System.IO
open System.Net
open System.Net.Sockets
open System.Text
open System.Threading.Tasks

type Response =
    { Status: int
      ContentType: string option
      Body: byte[] }

let notFound = { Status = 404; ContentType = None; Body = [||] }

/// Resolves a request path under a root the way `path.join(root, decodeURIComponent(pathname))` does, refusing
/// anything outside the root and anything that is not a file.
let fileUnder (root: string) (contentType: string -> string option) (requestTarget: string) : Response =
    let pathname =
        let t = match requestTarget.IndexOfAny [| '?'; '#' |] with -1 -> requestTarget | i -> requestTarget.Substring(0, i)
        Uri.UnescapeDataString t
    let full = Path.GetFullPath(root.TrimEnd('/') + "/" + pathname.TrimStart('/'))
    if not (full.StartsWith root) || not (File.Exists full) then notFound
    else { Status = 200; ContentType = contentType (Path.GetExtension full); Body = File.ReadAllBytes full }

type Server(listener: TcpListener) =
    member _.Port = (listener.LocalEndpoint :?> IPEndPoint).Port
    interface IDisposable with
        member _.Dispose() = listener.Stop()

let private reason status = match status with 200 -> "OK" | 404 -> "Not Found" | _ -> "Error"

let private serveConnection (handler: string -> Response) (client: TcpClient) : Task =
    task {
        use client = client
        use stream = client.GetStream()
        let buf = Array.zeroCreate<byte> 65536
        let mutable acc = ""
        let mutable fin = false
        while not fin && not (acc.Contains "\r\n\r\n") do
            let! n = stream.ReadAsync(buf, 0, buf.Length)
            if n = 0 then fin <- true else acc <- acc + Encoding.Latin1.GetString(buf, 0, n)
        if acc.Contains "\r\n\r\n" then
            let requestLine = acc.Substring(0, acc.IndexOf "\r\n").Split(' ')
            let target = if requestLine.Length > 1 then requestLine.[1] else "/"
            let r = try handler target with _ -> notFound
            let head =
                $"HTTP/1.1 {r.Status} {reason r.Status}\r\n"
                + (match r.ContentType with Some ct -> $"content-type: {ct}\r\n" | None -> "")
                + $"Content-Length: {r.Body.Length}\r\nConnection: close\r\n\r\n"
            let hb = Encoding.ASCII.GetBytes head
            do! stream.WriteAsync(hb, 0, hb.Length)
            if requestLine.[0] <> "HEAD" then do! stream.WriteAsync(r.Body, 0, r.Body.Length)
            do! stream.FlushAsync()
    }

/// Starts serving; dispose to stop.
let start (handler: string -> Response) : Server =
    let listener = new TcpListener(IPAddress.Loopback, 0)
    listener.Start()
    let loop () : Task =
        task {
            while true do
                let! client = listener.AcceptTcpClientAsync()
                serveConnection handler client |> ignore
        }
    (task { try do! loop () with _ -> () }) |> ignore
    new Server(listener)

using System;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading;
using livemap.util;
using MimeTypes;

namespace livemap.httpd;

public partial class WebServer {
    [GeneratedRegex(@"^(.*\/)?(.+)\/([+-]?\d+)\/([+-]?\d+)\/([+-]?\d+)(\/.*)?")]
    private static partial Regex FriendlyUrlRegex();

    private readonly LiveMap _server;

    private HttpListener? _listener;
    private Thread? _thread;
    private bool _running;
    private bool _stopped;
    private bool _reload;

    public WebServer(LiveMap server) {
        _server = server;
    }

    public void Reload() {
        _reload = true;
        Dispose();
        _stopped = false;
    }

    public void Run() {
        if (!_server.Config.Httpd.Enabled) {
            return;
        }

        if (!_stopped && !_running) {
            RunAsyncInfiniteLoop();
        }
    }

    private void RunAsyncInfiniteLoop() {
        _running = true;

        (_thread = new Thread(_ => {
            int port = _server.Config.Httpd.Port;
            string host = _server.Config.Httpd.Host;

            if (_listener == null) {
                Logger.Info($"Internal webserver starting on port {port}");
            }

            try {
                (_listener = new HttpListener { Prefixes = { $"http://{host}:{port}/" } }).Start();
            } catch (Exception e) {
                Logger.Error("Internal webserver has failed to start");
                if (e is HttpListenerException { ErrorCode: 5 }) {
                    Logger.Error("Check LiveMap Wiki for possible fixes: https://vs.pl3x.net/livemap-access-denied");
                }
                Logger.Error(e.ToString());
                _running = false;
                _stopped = true;
                Thread.CurrentThread.Interrupt();
                return;
            }

            while (_running) {
                try {
                    HandleRequest(_listener!.GetContext());
                } catch (Exception) {
                    if (_stopped) {
                        Logger.Info("Internal webserver has stopped");
                        if (_reload) {
                            _reload = false;
                            _stopped = false;
                        }
                    }

                    try {
                        _listener?.Stop();
                    } catch (Exception) {
                        // ignore
                    }

                    _running = false;
                    Thread.CurrentThread.Interrupt();
                }
            }
        })).Start();
    }

    private static void HandleRequest(HttpListenerContext context) {
        string urlLoc = context.Request.Url?.LocalPath[1..] ?? "";

        try {
            // friendly urls
            MatchCollection matches = FriendlyUrlRegex().Matches(urlLoc);
            if (matches.Count > 0) {
                string group6 = matches[0].Groups[6].Value;
                if (group6.Length == 0 && !matches[0].Value.EndsWith('/')) {
                    context.Response.Redirect($"{context.Request.Url?.OriginalString}/");
                    context.Response.Close();
                    return;
                }
                urlLoc = group6[1..];
            }
        } catch (Exception) {
            // ignore
        }

        if (string.IsNullOrEmpty(urlLoc)) {
            urlLoc = "index.html";
        }

        using HttpListenerResponse response = context.Response;
        string filePath = Path.Combine(Files.WebDir, urlLoc);

        byte[] buffer;
        if (File.Exists(filePath)) {
            response.ContentType = MimeTypeMap.GetMimeType(new FileInfo(filePath).Extension) ?? MediaTypeNames.Text.Plain;
            buffer = File.ReadAllBytes(filePath);
            response.StatusCode = 200;
        } else {
            response.ContentType = MediaTypeNames.Text.Html;
            buffer = File.ReadAllBytes(Path.Combine(Files.WebDir, "404.html"));
            response.StatusCode = 404;
        }

        response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept, X-Requested-With");
        response.AddHeader("Access-Control-Allow-Methods", "GET,POST");
        response.AddHeader("Access-Control-Allow-Origin", "*");

        try {
            TimeSpan time = File.GetLastWriteTimeUtc(filePath) - DateTime.UnixEpoch;
            response.AddHeader("ETag", ((long)time.TotalMilliseconds).ToString());
        } catch (Exception) {
            // ignore
        }

        response.ContentLength64 = buffer.Length;
        response.OutputStream.Write(buffer, 0, buffer.Length);
        response.OutputStream.Close();
    }

    public void Dispose() {
        _stopped = true;

        try {
            _listener?.Stop();
        } catch (ObjectDisposedException) { }
        _listener = null;

        _thread?.Interrupt();
        _thread = null;
    }
}

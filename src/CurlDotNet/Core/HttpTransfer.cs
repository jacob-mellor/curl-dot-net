/***************************************************************************
 * HttpTransfer - the state of one curl transfer (first request, auth retries, redirects)
 *
 * Holds what curl keeps per transfer in its "easy handle": the request options of the current
 * hop, the wire transcript, cookies, the proxy decision, counters for -w (num_redirects,
 * num_connects) and the redirect target that %{redirect_url} reports.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Per-transfer state shared by <see cref="HttpHandler"/> and the redirect logic.
    /// </summary>
    internal sealed class HttpTransfer
    {
        private readonly Func<CurlOptions, HttpRequestMessage> _createRequest;
        private readonly Action<StringBuilder?, HttpRequestMessage> _verboseRequest;
        private readonly Action<StringBuilder?, HttpResponseMessage> _verboseResponse;
        private readonly Func<HttpResponseMessage, string> _headerBlock;
        private readonly List<string> _fallbackBlocks = new List<string>();

        public HttpTransfer(
            CurlOptions options,
            HttpClient client,
            CurlProxy? proxy,
            CookieEngine? cookies,
            StringBuilder? verboseLog,
            Func<CurlOptions, HttpRequestMessage> createRequest,
            Action<StringBuilder?, HttpRequestMessage> verboseRequest,
            Action<StringBuilder?, HttpResponseMessage> verboseResponse,
            Func<HttpResponseMessage, string> headerBlock)
        {
            Options = options;
            HopOptions = options;
            Client = client;
            Proxy = proxy;
            Cookies = cookies;
            VerboseLog = verboseLog;
            _createRequest = createRequest;
            _verboseRequest = verboseRequest;
            _verboseResponse = verboseResponse;
            _headerBlock = headerBlock;
            OriginalUrl = options.Url;
            FirstUri = new Uri(options.Url);
        }

        /// <summary>The command's options (never modified).</summary>
        public CurlOptions Options { get; }

        /// <summary>The options of the hop being sent (a copy once a redirect changed something).</summary>
        public CurlOptions HopOptions { get; set; }

        /// <summary>The client every request of this transfer is sent with.</summary>
        public HttpClient Client { get; }

        /// <summary>The proxy decision, or null for direct connections.</summary>
        public CurlProxy? Proxy { get; }

        /// <summary>The cookie engine, when -b FILE, -c or -j engaged it.</summary>
        public CookieEngine? Cookies { get; }

        /// <summary>The -v log being built, if any.</summary>
        public StringBuilder? VerboseLog { get; }

        /// <summary>The URL as given on the command line (for %{url} and -O).</summary>
        public string OriginalUrl { get; }

        /// <summary>The first request's URL; credentials only go to this origin unless --location-trusted.</summary>
        public Uri FirstUri { get; }

        /// <summary>Bytes on the wire, per exchange.</summary>
        public WireTranscript Transcript { get; } = new WireTranscript();

        /// <summary>The exchange of the last request sent.</summary>
        public WireExchange? LastExchange { get; private set; }

        /// <summary>Timings in milliseconds, as CurlResult reports them.</summary>
        public CurlTimings Timings { get; } = new CurlTimings();

        /// <summary>When the transfer started.</summary>
        public DateTime StartTime { get; } = DateTime.UtcNow;

        /// <summary>Redirects followed so far.</summary>
        public int RedirectCount { get; set; }

        /// <summary>Where the last response redirected to, when it was not followed (%{redirect_url}).</summary>
        public string? RedirectUrl { get; set; }

        /// <summary>The request currently in flight (for error messages).</summary>
        public HttpRequestMessage? CurrentRequest { get; private set; }

        /// <summary>True when the current request goes through <see cref="Proxy"/>.</summary>
        public bool CurrentViaProxy { get; private set; }

        /// <summary>Requests sent (excluding HttpClient's own auth retries).</summary>
        public int RequestsSent { get; private set; }

        /// <summary>True when --referer ended in ";auto": redirect hops send the previous URL as Referer.</summary>
        public bool AutoReferer { get; private set; }

        /// <summary>The byte offset -C/--continue-at resumes from (0 = not resuming).</summary>
        public long ResumeOffset { get; private set; }

        /// <summary>Builds the request for <paramref name="hop"/> and adds curl's transport headers.</summary>
        public HttpRequestMessage PrepareRequest(CurlOptions hop)
        {
            // curl sends -e/--referer verbatim; ";auto" (alone or after a value) turns on auto-referer for
            // redirects. A relative or empty value is not a URI, so it is added as a raw header here.
            var effective = hop;
            string? rawReferer = null;
            if (hop.Referer != null)
            {
                var referer = hop.Referer;
                if (referer.EndsWith(";auto", StringComparison.Ordinal))
                {
                    AutoReferer = true;
                    referer = referer.Substring(0, referer.Length - 5);
                }
                if (referer.Length == 0 || !Uri.TryCreate(referer, UriKind.Absolute, out _))
                {
                    effective = hop.ShallowCopy();
                    effective.Referer = null;
                    rawReferer = referer.Length > 0 ? referer : null;
                }
                else if (!ReferenceEquals(referer, hop.Referer))
                {
                    effective = hop.ShallowCopy();
                    effective.Referer = referer;
                }
            }

            var request = _createRequest(effective);
            if (rawReferer != null)
            {
                request.Headers.TryAddWithoutValidation("Referer", rawReferer);
            }
            var uri = request.RequestUri!;
            var proxyUri = Proxy?.ProxyFor(uri);
            var tunnel = proxyUri != null && (Options.ProxyTunnel || !string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase));
            var plainProxy = proxyUri != null && !tunnel && !proxyUri.Scheme.StartsWith("socks", StringComparison.OrdinalIgnoreCase);
            CurrentViaProxy = proxyUri != null;

            if (plainProxy)
            {
                // curl adds this for every request sent to an HTTP proxy without a tunnel
                if (!hop.Headers.ContainsKey("Proxy-Connection"))
                {
                    request.Headers.TryAddWithoutValidation("Proxy-Connection", "Keep-Alive");
                }

                var auth = Proxy!.PreemptiveAuthorization;
                if (auth != null && !hop.Headers.ContainsKey("Proxy-Authorization"))
                {
                    request.Headers.TryAddWithoutValidation("Proxy-Authorization", auth);
                }
            }

            Cookies?.ApplyTo(request, hop);
            ApplyExpect(request, hop);
            ApplyEtag(request, hop);
            ApplyResume(request, hop);

            if (hop.TransferEncoding)
            {
                // --tr-encoding: ask for a compressed transfer-encoding (hop-by-hop, hence Connection: TE)
                request.Headers.TryAddWithoutValidation("TE", "gzip");
                request.Headers.Connection.Add("TE");
            }

            var settings = new TransportRequestSettings
            {
                ConnectTimeoutSeconds = hop.ConnectTimeout ?? 0,
                Resolve = hop.Resolve.Count > 0 ? hop.Resolve : null,
                ViaProxy = proxyUri != null,
                Transcript = Transcript,
            };
            if (Options.ProxyTunnel && proxyUri != null)
            {
                settings.TunnelProxy = proxyUri;
                settings.TunnelAuthorization = Proxy!.PreemptiveAuthorization;
                settings.TunnelVersion = Options.ProxyHttp10 ? "1.0" : "1.1";
                settings.TunnelUserAgent = hop.UserAgent ?? Curl.DefaultUserAgent;
            }
            CurlTransport.Attach(request, settings);
            CurrentRequest = request;
            return request;
        }

        /// <summary>
        /// curl 8 sends <c>Expect: 100-continue</c> only for bodies over 1 MB; HttpClient must not add
        /// it otherwise.
        /// </summary>
        private static void ApplyExpect(HttpRequestMessage request, CurlOptions hop)
        {
            if (hop.Headers.ContainsKey("Expect"))
            {
                return; // -H "Expect: ..." or -H "Expect:" decides
            }

            var length = request.Content?.Headers.ContentLength;
            if (request.Content != null && (length == null || length > 1024 * 1024)
                && request.Version.Major == 1 && request.Version.Minor >= 1)
            {
                request.Headers.ExpectContinue = true;
            }
            else
            {
                request.Headers.ExpectContinue = false;
            }
        }

        /// <summary>
        /// -C/--continue-at: "-C -" resumes from the size of the existing output file; "-C 0" sends no
        /// Range at all, like curl.
        /// </summary>
        private void ApplyResume(HttpRequestMessage request, CurlOptions hop)
        {
            if (!hop.ResumeFrom.HasValue)
            {
                ResumeOffset = 0;
                return;
            }

            var offset = hop.ResumeFrom.Value;
            if (offset < 0)
            {
                offset = 0;
                var target = hop.OutputFile;
                if (!string.IsNullOrEmpty(target) && target != "-")
                {
                    if (!string.IsNullOrEmpty(hop.OutputDir) && !System.IO.Path.IsPathRooted(target))
                    {
                        target = System.IO.Path.Combine(hop.OutputDir!, target);
                    }
                    try
                    {
                        if (System.IO.File.Exists(target)) offset = new System.IO.FileInfo(target).Length;
                    }
                    catch (System.IO.IOException)
                    {
                        offset = 0;
                    }
                }
            }

            ResumeOffset = offset;
            request.Headers.Range = offset > 0 ? new System.Net.Http.Headers.RangeHeaderValue(offset, null) : null;
        }

        /// <summary>--etag-compare: send the stored ETag in If-None-Match.</summary>
        private static void ApplyEtag(HttpRequestMessage request, CurlOptions hop)
        {
            if (string.IsNullOrEmpty(hop.EtagCompare) || request.Headers.Contains("If-None-Match"))
            {
                return;
            }

            var etag = "\"\"";
            try
            {
                if (System.IO.File.Exists(hop.EtagCompare))
                {
                    var line = System.IO.File.ReadAllLines(hop.EtagCompare!).FirstOrDefault(l => l.Trim().Length > 0);
                    if (line != null)
                    {
                        etag = line.Trim();
                    }
                }
            }
            catch (System.IO.IOException)
            {
                // unreadable: curl sends the blank ETag
            }
            request.Headers.TryAddWithoutValidation("If-None-Match", etag);
        }

        /// <summary>Sends a request while recording its bytes, and stores any cookies it sets.</summary>
        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completion, CancellationToken cancellationToken)
        {
            var exchange = Transcript.Begin(request.Method == HttpMethod.Head);
            LastExchange = exchange;
            RequestsSent++;
            WireExchange.Current = exchange;
            var response = await Client.SendAsync(request, completion, cancellationToken).ConfigureAwait(false);

            TransportErrors.ValidateHeads(exchange, this);

            if (Cookies != null)
            {
                var heads = exchange.Heads;
                if (heads.Count > 0)
                {
                    foreach (var head in heads)
                    {
                        Cookies.StoreFromResponse(request.RequestUri!, HopOptions, head.GetHeaders("Set-Cookie"));
                    }
                }
                else if (response.Headers.TryGetValues("Set-Cookie", out var values))
                {
                    Cookies.StoreFromResponse(request.RequestUri!, HopOptions, values);
                }
            }

            return response;
        }

        /// <summary>The raw header block of the final response of the last exchange, if recorded.</summary>
        public RawResponseHead? FinalHead => LastExchange?.FinalHead;

        /// <summary>
        /// The Location value exactly as the server sent it (raw, untrimmed of inner spaces).
        /// </summary>
        public string? GetLocation(HttpResponseMessage response)
        {
            var head = FinalHead;
            if (head != null)
            {
                return head.GetHeader("Location");
            }

#if NET6_0_OR_GREATER
            if (response.Headers.NonValidated.TryGetValues("Location", out var raw))
            {
                foreach (var value in raw)
                {
                    return value;
                }
            }
            return null;
#else
            return response.Headers.Location?.OriginalString;
#endif
        }

        /// <summary>Remembers a redirect hop's header block when no raw recording exists.</summary>
        public void RecordFallbackHeaders(HttpResponseMessage response)
        {
            if (FinalHead == null)
            {
                _fallbackBlocks.Add(_headerBlock(response));
            }
        }

        /// <summary>
        /// Every header block of the transfer as curl prints it for -i, -I and -D: raw bytes when
        /// recorded (interim 1xx, auth challenges and redirect hops included), otherwise rebuilt
        /// from HttpClient's parsed headers.
        /// </summary>
        public string BuildHeaderText(HttpResponseMessage finalResponse)
        {
            if (Transcript.HasRecording && FinalHead != null)
            {
                return string.Concat(Transcript.AllHeads.Select(h => h.Text));
            }

            var builder = new StringBuilder();
            foreach (var block in _fallbackBlocks)
            {
                builder.Append(block).Append("\r\n\r\n");
            }
            builder.Append(_headerBlock(finalResponse)).Append("\r\n\r\n");
            return builder.ToString();
        }

        /// <summary>Adds a request to the -v log.</summary>
        public void AppendVerboseRequest(HttpRequestMessage request) => _verboseRequest(VerboseLog, request);

        /// <summary>Adds a response to the -v log.</summary>
        public void AppendVerboseResponse(HttpResponseMessage response) => _verboseResponse(VerboseLog, response);
    }
}

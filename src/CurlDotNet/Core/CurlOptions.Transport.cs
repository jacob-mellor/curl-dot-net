/***************************************************************************
 * CurlOptions.Transport - transport-side settings
 *
 * Partial-class file for options that configure how a request travels (proxy details,
 * cookie files, redirect policy, output file handling, write-out format, retries, TLS).
 * Request-shaping options stay in CurlOptions.cs.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System.Collections.Generic;

namespace CurlDotNet.Core
{
    public partial class CurlOptions
    {
        /// <summary>
        /// Hosts that are reached directly even though a proxy is configured, like curl's <c>--noproxy LIST</c>; null
        /// falls back to the <c>no_proxy</c>/<c>NO_PROXY</c> environment variable.
        /// </summary>
        /// <value>A comma-separated list of host names, domain suffixes, IP addresses and CIDR ranges; <c>*</c> bypasses the proxy
        /// for every host. Default: null.</value>
        /// <remarks>
        /// <para><c>example.com</c> also matches <c>www.example.com</c> (a leading dot is allowed); <c>10.0.0.0/8</c> matches the
        /// range. An empty string overrides the environment variable and proxies everything. Applies to <see cref="Proxy"/>,
        /// <see cref="Socks5Proxy"/> and proxies taken from the environment.</para>
        /// <para>Example: <c>curl -x http://proxy:3128 --noproxy localhost,.internal https://api.internal/</c> is
        /// <c>new CurlOptions { Url = "https://api.internal/", Proxy = "http://proxy:3128", NoProxy = "localhost,.internal" }</c>,
        /// which connects to <c>api.internal</c> directly while every other host goes through the proxy.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="Socks5Proxy"/>
        public string? NoProxy { get; set; }

        /// <summary>
        /// How <see cref="ProxyCredentials"/> are presented to the proxy: <c>basic</c>, <c>digest</c>, <c>ntlm</c>,
        /// <c>negotiate</c> or <c>anyauth</c>, set by curl's <c>--proxy-basic</c>, <c>--proxy-digest</c>, <c>--proxy-ntlm</c>,
        /// <c>--proxy-negotiate</c> and <c>--proxy-anyauth</c>.
        /// </summary>
        /// <value>A lower-case scheme name. Default: null, which means Basic.</value>
        /// <remarks>
        /// <para>With Basic (or null) the <c>Proxy-Authorization</c> header is sent on the first request, exactly like curl; the
        /// other schemes wait for the proxy's <c>407</c> challenge and answer it. A challenge that cannot be processed fails with
        /// <see cref="CurlDotNet.Exceptions.CurlAuthErrorException"/> (exit 94).</para>
        /// <para>Example: <c>curl -x proxy:3128 -U alice:secret --proxy-digest http://example.com/</c> is
        /// <c>new CurlOptions { Url = url, Proxy = "proxy:3128", ProxyCredentials = new NetworkCredential("alice", "secret"), ProxyAuthScheme = "digest" }</c>.</para>
        /// </remarks>
        /// <seealso cref="ProxyCredentials"/>
        /// <seealso cref="AuthScheme"/>
        /// <seealso cref="Proxy"/>
        public string? ProxyAuthScheme { get; set; }

        /// <summary>
        /// Tunnels through the HTTP proxy with <c>CONNECT</c> even for plain <c>http://</c> URLs, like curl's
        /// <c>-p</c>/<c>--proxytunnel</c>; <c>https://</c> URLs are always tunnelled.
        /// </summary>
        /// <value>True to tunnel every request. Default: false.</value>
        /// <remarks>
        /// <para>Without it, <c>http://</c> requests are sent to the proxy with the absolute URL in the request line
        /// (<c>GET http://host/path HTTP/1.1</c>). Some corporate proxies only allow <c>CONNECT</c>. Supported on .NET 5 and later.</para>
        /// <para>Example: <c>curl -p -x proxy:3128 http://example.com/</c> is
        /// <c>new CurlOptions { Url = "http://example.com/", Proxy = "proxy:3128", ProxyTunnel = true }</c>; the proxy first sees
        /// <c>CONNECT example.com:80 HTTP/1.1</c>.</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="ProxyHttp10"/>
        public bool ProxyTunnel { get; set; }

        /// <summary>
        /// Talks HTTP/1.0 to the proxy, so a <c>CONNECT</c> tunnel request is sent as <c>HTTP/1.0</c>, like curl's
        /// <c>--proxy1.0 HOST[:PORT]</c> for old proxies that reject HTTP/1.1.
        /// </summary>
        /// <value>True for an HTTP/1.0 proxy. Default: false.</value>
        /// <remarks>
        /// <para>The parser sets this together with <see cref="Proxy"/>. Example: <c>curl --proxy1.0 oldproxy:8080 https://example.com/</c>
        /// is <c>new CurlOptions { Url = url, Proxy = "oldproxy:8080", ProxyHttp10 = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="ProxyTunnel"/>
        public bool ProxyHttp10 { get; set; }

        /// <summary>
        /// Keeps the POST method and its body when a <c>301 Moved Permanently</c> redirect is followed, instead of switching
        /// to GET as curl and browsers do; curl's <c>--post301</c>.
        /// </summary>
        /// <value>True to keep POST on 301. Default: false.</value>
        /// <remarks>
        /// <para>Only matters with <see cref="FollowLocation"/>. Example: <c>curl -L --post301 -d x=1 https://example.com/old</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post301 = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Post302"/>
        /// <seealso cref="Post303"/>
        /// <seealso cref="FollowLocation"/>
        public bool Post301 { get; set; }

        /// <summary>
        /// Keeps the POST method and its body when a <c>302 Found</c> redirect is followed, instead of switching to GET;
        /// curl's <c>--post302</c>.
        /// </summary>
        /// <value>True to keep POST on 302. Default: false.</value>
        /// <remarks>
        /// <para>Only matters with <see cref="FollowLocation"/>. Example: <c>curl -L --post302 -d x=1 URL</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post302 = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Post301"/>
        /// <seealso cref="Post303"/>
        public bool Post302 { get; set; }

        /// <summary>
        /// Keeps the POST method and its body when a <c>303 See Other</c> redirect is followed; without it a 303 turns any
        /// method except GET and HEAD into GET. curl's <c>--post303</c>.
        /// </summary>
        /// <value>True to keep POST on 303. Default: false.</value>
        /// <remarks>
        /// <para>Only matters with <see cref="FollowLocation"/>. Example: <c>curl -L --post303 -d x=1 URL</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", Data = "x=1", FollowLocation = true, Post303 = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Post301"/>
        /// <seealso cref="Post302"/>
        public bool Post303 { get; set; }

        /// <summary>
        /// Follows redirects and lets 301/302/303 change a custom <c>-X</c> method to GET, as the HTTP specification
        /// describes; curl's <c>--follow</c>, a stricter alternative to <c>-L</c>.
        /// </summary>
        /// <value>True to apply the specification's method rules to <see cref="CustomMethod"/>. Default: false.</value>
        /// <remarks>
        /// <para>The parser also sets <see cref="FollowLocation"/>. With plain <c>-L</c> a method given with <c>-X</c> is used for every hop,
        /// which can repeat a DELETE on the redirect target.</para>
        /// <para>Example: <c>curl --follow -X POST -d x=1 URL</c> is
        /// <c>new CurlOptions { Url = url, CustomMethod = "POST", Data = "x=1", FollowLocation = true, FollowObeysMethod = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="FollowLocation"/>
        /// <seealso cref="CustomMethod"/>
        public bool FollowObeysMethod { get; set; }

        /// <summary>
        /// Like <see cref="FailOnError"/> (exit code 22 for HTTP status 400 and above) but the response body is still
        /// written to the output and kept on the exception; curl's <c>--fail-with-body</c>.
        /// </summary>
        /// <value>True to fail on HTTP errors and keep the body. Default: false.</value>
        /// <remarks>
        /// <para>The thrown <see cref="CurlDotNet.Exceptions.CurlHttpReturnedErrorException"/> carries the body in <c>ResponseBody</c>
        /// (handy for API error JSON), and an <see cref="OutputFile"/> is written before it is thrown.</para>
        /// <para>Example: <c>curl --fail-with-body https://api.example.com/orders/9</c> is
        /// <c>new CurlOptions { Url = url, FailWithBody = true }</c>; in the catch block read <c>ex.ResponseBody</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="FailOnError"/>
        /// <seealso cref="CurlDotNet.Exceptions.CurlHttpReturnedErrorException"/>
        public bool FailWithBody { get; set; }

        /// <summary>
        /// Stops a multi-URL command at the first failed transfer instead of running the rest, like curl's
        /// <c>--fail-early</c>; without it a failure only ends the command when it is the last transfer.
        /// </summary>
        /// <value>True to stop at the first failure. Default: false.</value>
        /// <remarks>
        /// <para>Only matters when the command has <see cref="AdditionalTransfers"/> (several URLs, globs or <c>--next</c>). Fatal errors
        /// (unknown option, bad argument, out of memory) stop the command regardless.</para>
        /// <para>Example: <c>curl --fail-early -f https://a.example/ https://b.example/</c> does not request <c>b</c> when <c>a</c> returns 404.</para>
        /// </remarks>
        /// <seealso cref="AdditionalTransfers"/>
        /// <seealso cref="FailOnError"/>
        public bool FailEarly { get; set; }

        /// <summary>
        /// Writes every response header block of the transfer to this file, like curl's <c>-D</c>/<c>--dump-header FILE</c>;
        /// <c>"-"</c> puts them in <see cref="CurlResult.Body"/> instead.
        /// </summary>
        /// <value>A file path, or <c>"-"</c>. Default: null.</value>
        /// <remarks>
        /// <para>Every header block is written (interim <c>1xx</c> responses, authentication challenges and each redirect hop), exactly as
        /// received, plus chunked trailers. The folder is checked before the request is sent: a folder that does not exist fails with
        /// <see cref="CurlDotNet.Exceptions.CurlWriteErrorException"/> (exit 23) unless <see cref="CreateDirs"/> is set. Several
        /// transfers of one command append to the same file.</para>
        /// <para>Example: <c>curl -D headers.txt -o page.html https://example.com/</c> is
        /// <c>new CurlOptions { Url = "https://example.com/", DumpHeaderFile = "headers.txt", OutputFile = "page.html" }</c>.</para>
        /// </remarks>
        /// <seealso cref="IncludeHeaders"/>
        /// <seealso cref="CreateDirs"/>
        /// <seealso cref="CurlResult.Headers"/>
        public string? DumpHeaderFile { get; set; }

        /// <summary>
        /// Names the <c>-O</c> output file after the server's <c>Content-Disposition: attachment; filename=...</c> header,
        /// like curl's <c>-J</c>/<c>--remote-header-name</c>.
        /// </summary>
        /// <value>True to use the server's file name. Default: false. Needs <see cref="UseRemoteFileName"/>.</value>
        /// <remarks>
        /// <para>Directory parts of the suggested name are stripped. When the header is missing, the name comes from the (final) URL as
        /// with plain <c>-O</c>. An existing file is never overwritten: the transfer fails with
        /// <see cref="CurlDotNet.Exceptions.CurlWriteErrorException"/> (exit 23) instead.</para>
        /// <para>Example: <c>curl -OJ https://example.com/download?id=42</c> is
        /// <c>new CurlOptions { Url = url, UseRemoteFileName = true, RemoteHeaderName = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="UseRemoteFileName"/>
        /// <seealso cref="OutputDir"/>
        public bool RemoteHeaderName { get; set; }

        /// <summary>
        /// Folder that relative <c>-o</c> names and <c>-O</c> file names are written into, like curl's
        /// <c>--output-dir DIR</c>; absolute <see cref="OutputFile"/> paths ignore it.
        /// </summary>
        /// <value>A folder path. Default: null (the current directory).</value>
        /// <remarks>
        /// <para>The folder must exist unless <see cref="CreateDirs"/> is also set (exit code 23 otherwise).</para>
        /// <para>Example: <c>curl --output-dir downloads -O https://example.com/a.zip</c> is
        /// <c>new CurlOptions { Url = url, OutputDir = "downloads", UseRemoteFileName = true }</c>, which writes <c>downloads/a.zip</c>.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="UseRemoteFileName"/>
        /// <seealso cref="CreateDirs"/>
        public string? OutputDir { get; set; }

        /// <summary>
        /// Gives the downloaded file the server's <c>Last-Modified</c> time instead of the current time, like curl's
        /// <c>-R</c>/<c>--remote-time</c>; useful for mirroring.
        /// </summary>
        /// <value>True to copy the server time. Default: false. Needs an output file.</value>
        /// <remarks>
        /// <para>Nothing happens when the response has no usable <c>Last-Modified</c> header or the time cannot be set.
        /// Example: <c>curl -R -O https://example.com/data.csv</c> is <c>new CurlOptions { Url = url, UseRemoteFileName = true, RemoteTime = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="TimeCondition"/>
        public bool RemoteTime { get; set; }

        /// <summary>
        /// Never overwrites an existing output file; the download is saved under a numbered name instead
        /// (<c>report-1.txt</c>, <c>report-2.txt</c>, or <c>name.1</c> without an extension), like curl's <c>--no-clobber</c>.
        /// </summary>
        /// <value>True to keep existing files. Default: false (existing files are replaced).</value>
        /// <remarks>
        /// <para>Up to 100 numbered names are tried; when all are taken the transfer fails with
        /// <see cref="CurlDotNet.Exceptions.CurlWriteErrorException"/> (exit 23). The name actually written is in
        /// <see cref="CurlResult.OutputFiles"/>.</para>
        /// <para>Example: <c>curl --no-clobber -o report.txt URL</c> is <c>new CurlOptions { Url = url, OutputFile = "report.txt", NoClobber = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="SkipExisting"/>
        public bool NoClobber { get; set; }

        /// <summary>
        /// Leaves no output file behind when the transfer fails (for example a connection cut mid-download), like curl's
        /// <c>--remove-on-error</c>, so a partial file is never mistaken for a complete one.
        /// </summary>
        /// <value>True to drop the output of a failed transfer. Default: false (whatever arrived is written).</value>
        /// <remarks>
        /// <para>Example: <c>curl --remove-on-error -o big.iso URL</c> is <c>new CurlOptions { Url = url, OutputFile = "big.iso", RemoveOnError = true }</c>.
        /// Without it, resume an interrupted download with <see cref="ResumeFrom"/> = -1.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="ResumeFrom"/>
        public bool RemoveOnError { get; set; }

        /// <summary>
        /// File that receives what curl would print on standard error, mainly the <c>%{stderr}</c> part of
        /// <see cref="WriteOut"/> and warnings, like curl's <c>--stderr FILE</c>.
        /// </summary>
        /// <value>A file path (appended to), or null/<c>"-"</c> (default) for the process's standard error (<c>Console.Error</c>).</value>
        /// <remarks>
        /// <para>Errors themselves are thrown as exceptions, not written here. If the file cannot be written, the text goes to
        /// <c>Console.Error</c> instead, like curl.</para>
        /// <para>Example: <c>curl --stderr errors.log -w "%{stderr}%{http_code}\n" URL</c> is
        /// <c>new CurlOptions { Url = url, StderrFile = "errors.log", WriteOut = "%{stderr}%{http_code}\\n" }</c>.</para>
        /// </remarks>
        /// <seealso cref="WriteOut"/>
        /// <seealso cref="Silent"/>
        public string? StderrFile { get; set; }

        /// <summary>
        /// Ignores session cookies (those without an expiry date) when reading cookie files given with <c>-b</c>, as if
        /// starting a new browser session; curl's <c>-j</c>/<c>--junk-session-cookies</c>.
        /// </summary>
        /// <value>True to discard session cookies on load. Default: false.</value>
        /// <remarks>
        /// <para>Applies to cookie files named in <see cref="Cookie"/>, not to a literal <c>name=value</c> cookie string.
        /// Example: <c>curl -j -b cookies.txt URL</c> is <c>new CurlOptions { Url = url, Cookie = "cookies.txt", JunkSessionCookies = true }</c>.
        /// Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <seealso cref="Cookie"/>
        /// <seealso cref="CookieJar"/>
        public bool JunkSessionCookies { get; set; }

        /// <summary>
        /// The parts of a <c>multipart/form-data</c> body in command-line order, with curl's full <c>-F</c> syntax
        /// (file uploads, file contents, custom types, file names and part headers).
        /// </summary>
        /// <value>A list of <see cref="CurlFormPart"/>. Default: empty, in which case <see cref="FormData"/> and <see cref="Files"/> are used.</value>
        /// <remarks>
        /// <para>Each <c>-F name=value</c>, <c>-F name=@file</c>, <c>-F name=&lt;file</c> or <c>--form-string name=value</c> adds one part.
        /// When the list is not empty the request body is <c>multipart/form-data</c> formatted exactly like curl's (boundary, part headers
        /// and file content types) and <see cref="FormData"/>/<see cref="Files"/> are ignored for the body (the parser still fills them
        /// for code that reads them). A file that cannot be read fails with exit code 26.</para>
        /// <para>Example: <c>curl -F "avatar=@me.png;type=image/png" URL</c> is
        /// <c>options.FormParts.Add(new CurlFormPart { Name = "avatar", FilePath = "me.png", IsFileUpload = true, ContentType = "image/png" });</c>
        /// with <c>Method = "POST"</c>. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="CurlFormPart"/>
        /// <seealso cref="FormData"/>
        /// <seealso cref="Files"/>
        /// <seealso cref="FormEscape"/>
        public List<CurlFormPart> FormParts { get; set; } = new List<CurlFormPart>();

        /// <summary>
        /// Escapes form field and file names with backslashes instead of percent-encoding, for older servers; curl's
        /// <c>--form-escape</c>.
        /// </summary>
        /// <value>True for backslash escaping. Default: false.</value>
        /// <remarks>
        /// <para>By default curl writes a double quote in a <c>-F</c> name or file name as <c>%22</c> (and CR/LF as <c>%0D</c>/<c>%0A</c>);
        /// with this option it writes <c>\"</c> and doubles backslashes instead.</para>
        /// <para>Example: <c>curl --form-escape -F "file=@\"odd\".txt" URL</c>.</para>
        /// </remarks>
        /// <seealso cref="FormParts"/>
        public bool FormEscape { get; set; }

        /// <summary>
        /// Seconds to wait for the server's <c>100 Continue</c> before sending a request body anyway, like curl's
        /// <c>--expect100-timeout SECONDS</c> (decimals allowed).
        /// </summary>
        /// <value>Seconds, or null (default), which keeps HttpClient's default of 1 second.</value>
        /// <remarks>
        /// <para>Only matters for requests that send <c>Expect: 100-continue</c> (large uploads). Setting it makes the request use a
        /// dedicated connection. Example: <c>curl --expect100-timeout 0.5 -T big.bin URL</c> is
        /// <c>new CurlOptions { Url = url, UploadFile = "big.bin", BinaryData = bytes, Method = "PUT", Expect100TimeoutSeconds = 0.5 }</c>.</para>
        /// </remarks>
        /// <seealso cref="UploadFile"/>
        /// <seealso cref="MaxTime"/>
        public double? Expect100TimeoutSeconds { get; set; }

        /// <summary>
        /// Saves the response's <c>ETag</c> to this file (one line), like curl's <c>--etag-save FILE</c>, so a later request can
        /// ask "only if changed" with <see cref="EtagCompare"/>; <c>"-"</c> appends it to <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <value>A file path, or <c>"-"</c>. Default: null.</value>
        /// <remarks>
        /// <para>A response without an ETag writes an empty file. When the folder does not exist (and <see cref="CreateDirs"/> is not
        /// set) curl skips the whole transfer with a warning, and so does CurlDotNet: nothing is sent and the result is empty.</para>
        /// <para>Example: <c>curl --etag-save etag.txt -o feed.xml URL</c> is <c>new CurlOptions { Url = url, OutputFile = "feed.xml", EtagSave = "etag.txt" }</c>.</para>
        /// </remarks>
        /// <seealso cref="EtagCompare"/>
        public string? EtagSave { get; set; }

        /// <summary>
        /// Sends the ETag stored in this file as <c>If-None-Match</c>, like curl's <c>--etag-compare FILE</c>, so the server
        /// answers <c>304 Not Modified</c> with no body when the document has not changed.
        /// </summary>
        /// <value>A file path (first non-empty line is the ETag). Default: null.</value>
        /// <remarks>
        /// <para>A missing file sends an empty <c>If-None-Match: ""</c>, which forces a download. A 304 leaves an existing output file untouched.
        /// An explicit <c>If-None-Match</c> header wins.</para>
        /// <para>Example: <c>curl --etag-compare etag.txt --etag-save etag.txt -o feed.xml URL</c> is
        /// <c>new CurlOptions { Url = url, OutputFile = "feed.xml", EtagCompare = "etag.txt", EtagSave = "etag.txt" }</c>.</para>
        /// </remarks>
        /// <seealso cref="EtagSave"/>
        /// <seealso cref="TimeCondition"/>
        public string? EtagCompare { get; set; }

        /// <summary>
        /// Reads the body until the server closes the connection and accepts it even when it is shorter than the announced
        /// <c>Content-Length</c>, like curl's <c>--ignore-content-length</c>.
        /// </summary>
        /// <value>True to ignore a wrong Content-Length. Default: false: a short body fails with
        /// <see cref="CurlDotNet.Exceptions.CurlPartialFileException"/> (exit 18).</value>
        /// <remarks>
        /// <para>Use it for broken servers that announce the wrong length. Example: <c>curl --ignore-content-length URL</c> is
        /// <c>new CurlOptions { Url = url, IgnoreContentLength = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Raw"/>
        public bool IgnoreContentLength { get; set; }

        /// <summary>
        /// Skips the transfer entirely when its output file already exists, like curl's <c>--skip-existing</c>; nothing is sent
        /// and the result is empty with status 0.
        /// </summary>
        /// <value>True to skip existing downloads. Default: false.</value>
        /// <remarks>
        /// <para>Applies to <see cref="OutputFile"/> and <see cref="UseRemoteFileName"/>. Useful when re-running a batch of downloads.
        /// Example: <c>curl --skip-existing -O URL</c> is <c>new CurlOptions { Url = url, UseRemoteFileName = true, SkipExisting = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="NoClobber"/>
        /// <seealso cref="OutputFile"/>
        public bool SkipExisting { get; set; }

        /// <summary>
        /// Accepts an HTTP/0.9 reply (a bare body with no status line or headers) as the response, like curl's
        /// <c>--http0.9</c>; needed for some very old devices and raw TCP services.
        /// </summary>
        /// <value>True to accept HTTP/0.9. Default: false: such a reply fails with
        /// <see cref="CurlDotNet.Exceptions.CurlUnsupportedProtocolException"/> (exit 1), like curl 7.66 and later.</value>
        /// <remarks>
        /// <para>Cleared again by <c>--no-http0.9</c>. Example: <c>curl --http0.9 http://192.168.1.20:9000/</c> is
        /// <c>new CurlOptions { Url = url, AllowHttp09 = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="HttpVersion"/>
        public bool AllowHttp09 { get; set; }

        /// <summary>
        /// Asks for a compressed transfer-encoding (<c>TE: gzip</c>) and decodes a <c>Transfer-Encoding: gzip</c> or
        /// <c>deflate</c> body, like curl's <c>--tr-encoding</c>.
        /// </summary>
        /// <value>True to request and decode transfer compression. Default: false: such a response fails with
        /// <see cref="CurlDotNet.Exceptions.CurlBadContentEncodingException"/> (exit 61), like curl.</value>
        /// <remarks>
        /// <para>Sends <c>TE: gzip</c> with <c>Connection: TE</c>. For the common <c>Content-Encoding</c> compression use
        /// <see cref="Compressed"/>. Example: <c>curl --tr-encoding URL</c> is <c>new CurlOptions { Url = url, TransferEncoding = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Compressed"/>
        public bool TransferEncoding { get; set; }

        /// <summary>
        /// Passes transfer-codings through instead of interpreting them, like curl's <c>--raw</c>: a chunked body keeps its
        /// chunk framing and unknown <c>Transfer-Encoding</c> values are accepted.
        /// </summary>
        /// <value>True for raw mode. Default: false.</value>
        /// <remarks>
        /// <para>The chunk sizes are written to the output when the wire bytes are available (.NET 5 and later); on .NET Standard 2.0
        /// and .NET Framework HttpClient removes the framing first. With an unknown transfer-coding the body is read until the server
        /// closes. Used for debugging servers and proxies.</para>
        /// <para>Example: <c>curl --raw URL</c> is <c>new CurlOptions { Url = url, Raw = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="TransferEncoding"/>
        /// <seealso cref="IgnoreContentLength"/>
        public bool Raw { get; set; }

        /// <summary>
        /// Creates a member-wise copy so a redirect hop can change its URL, method or headers without
        /// touching the caller's options. Collections are shared; replace them before modifying.
        /// </summary>
        internal CurlOptions ShallowCopy() => (CurlOptions)MemberwiseClone();
    }

    /// <summary>
    /// One part of a <c>multipart/form-data</c> body, as given with curl's <c>-F</c> or <c>--form-string</c>: a text field,
    /// a file upload, or a field whose value is read from a file.
    /// </summary>
    /// <remarks>
    /// <para>Accepted forms: <c>name=value</c> (text field), <c>name=@path</c> (file upload; sends <c>filename="basename"</c> and a
    /// Content-Type guessed from the extension), <c>name=&lt;path</c> (the file's content becomes the field value) and the modifiers
    /// <c>;type=mime/type</c>, <c>;filename=name</c> and <c>;headers=Header: value</c>. Values may be double-quoted to protect
    /// <c>;</c> and <c>,</c>.</para>
    /// <para>Example: <c>curl -F "avatar=@me.png;type=image/png" -F name=Alice https://example.com/upload</c> is two parts:
    /// <c>new CurlFormPart { Name = "avatar", FilePath = "me.png", IsFileUpload = true, ContentType = "image/png" }</c> and
    /// <c>new CurlFormPart { Name = "name", Value = "Alice" }</c>, added to <see cref="CurlOptions.FormParts"/>.
    /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlOptions.FormParts"/>
    /// <seealso cref="CurlOptions.FormData"/>
    /// <seealso cref="CurlOptions.Files"/>
    public class CurlFormPart
    {
        /// <summary>
        /// The form field name sent in the part's <c>Content-Disposition: form-data; name="..."</c> header; the part
        /// before <c>=</c> in curl's <c>-F name=value</c>.
        /// </summary>
        /// <value>The field name the server reads, such as <c>"avatar"</c>. Default: empty string.</value>
        /// <remarks><para>Quotes and line breaks are percent-encoded (or backslash-escaped with <see cref="CurlOptions.FormEscape"/>).
        /// Example: <c>-F user=ada</c> is <c>new CurlFormPart { Name = "user", Value = "ada" }</c>.</para></remarks>
        /// <seealso cref="Value"/>
        /// <seealso cref="FileName"/>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The literal text of a text field, the part after <c>=</c> in <c>-F name=value</c> or <c>--form-string name=value</c>;
        /// null for file parts.
        /// </summary>
        /// <value>The field text, sent as is. Default: null.</value>
        /// <remarks><para><c>--form-string</c> never treats a leading <c>@</c> or <c>&lt;</c> as a file. Example: <c>-F "note=hello world"</c> is
        /// <c>new CurlFormPart { Name = "note", Value = "hello world" }</c>.</para></remarks>
        /// <seealso cref="Name"/>
        /// <seealso cref="FilePath"/>
        public string? Value { get; set; }

        /// <summary>
        /// Path of the local file to upload (for <c>name=@path</c>, with <see cref="IsFileUpload"/>) or to read as the field
        /// value (for <c>name=&lt;path</c>, with <see cref="IsFileContent"/>).
        /// </summary>
        /// <value>A file path, absolute or relative to the current directory. Default: null.</value>
        /// <remarks><para>A file that cannot be read fails with <see cref="CurlDotNet.Exceptions.CurlReadErrorException"/> (exit 26) when
        /// the body is built. Example: <c>-F report=@./q3.pdf</c> is <c>new CurlFormPart { Name = "report", FilePath = "./q3.pdf", IsFileUpload = true }</c>.</para></remarks>
        /// <seealso cref="IsFileUpload"/>
        /// <seealso cref="IsFileContent"/>
        public string? FilePath { get; set; }

        /// <summary>
        /// True for <c>name=@path</c>: the file at <see cref="FilePath"/> is sent as an attachment with a
        /// <c>filename="..."</c> and its own Content-Type.
        /// </summary>
        /// <value>Default: false.</value>
        /// <remarks><para>The Content-Type is guessed from the extension unless <see cref="ContentType"/> is set; the file name is the base
        /// name unless <see cref="FileName"/> is set. Example: <c>-F "doc=@a.pdf"</c> sets this to true.</para></remarks>
        /// <seealso cref="FilePath"/>
        /// <seealso cref="IsFileContent"/>
        public bool IsFileUpload { get; set; }

        /// <summary>
        /// True for <c>name=&lt;path</c>: the content of the file at <see cref="FilePath"/> becomes the text value of the field,
        /// with no <c>filename</c> parameter.
        /// </summary>
        /// <value>Default: false.</value>
        /// <remarks><para>Example: <c>-F "description=&lt;notes.txt"</c> sends the text of <c>notes.txt</c> as the <c>description</c> field.</para></remarks>
        /// <seealso cref="FilePath"/>
        /// <seealso cref="IsFileUpload"/>
        public bool IsFileContent { get; set; }

        /// <summary>
        /// The <c>filename</c> parameter of a file part, from curl's <c>;filename=name</c> modifier; lets you upload
        /// <c>tmp123.bin</c> while the server sees <c>report.pdf</c>.
        /// </summary>
        /// <value>A file name, or null (default) for the base name of <see cref="FilePath"/>.</value>
        /// <remarks><para>Example: <c>-F "file=@tmp123.bin;filename=report.pdf"</c> is
        /// <c>new CurlFormPart { Name = "file", FilePath = "tmp123.bin", IsFileUpload = true, FileName = "report.pdf" }</c>.</para></remarks>
        /// <seealso cref="FilePath"/>
        /// <seealso cref="ContentType"/>
        public string? FileName { get; set; }

        /// <summary>
        /// The part's Content-Type, from curl's <c>;type=mime/type</c> modifier, such as <c>image/png</c> or
        /// <c>application/json</c>; overrides the type guessed from the file extension.
        /// </summary>
        /// <value>A MIME type, or null (default) for curl's default: guessed from the extension for files, none for text fields.</value>
        /// <remarks><para>Example: <c>-F "meta={\"a\":1};type=application/json"</c> is
        /// <c>new CurlFormPart { Name = "meta", Value = "{\"a\":1}", ContentType = "application/json" }</c>.</para></remarks>
        /// <seealso cref="FileName"/>
        /// <seealso cref="Headers"/>
        public string? ContentType { get; set; }

        /// <summary>
        /// Extra headers written inside this part, from curl's <c>;headers=Header: value</c> modifier (for example
        /// <c>X-Checksum: abc</c>), one <c>"Name: value"</c> string per entry.
        /// </summary>
        /// <value>Header lines. Default: empty.</value>
        /// <remarks><para>Example: <c>-F "file=@a.bin;headers=X-Checksum: abc"</c> adds <c>"X-Checksum: abc"</c> here.</para></remarks>
        /// <seealso cref="ContentType"/>
        public List<string> Headers { get; set; } = new List<string>();

        /// <summary>
        /// The files of <c>-F "name=@a.txt,b.txt"</c>: when more than one file is named, each one is an entry here and they
        /// are sent together as a nested <c>multipart/mixed</c> part, like curl.
        /// </summary>
        /// <value>File parts; empty for single-file and text parts.</value>
        /// <remarks><para>Example: <c>curl -F "docs=@a.txt,b.txt" URL</c> produces one part named <c>docs</c> with two entries here.</para></remarks>
        /// <seealso cref="FilePath"/>
        public List<CurlFormPart> MixedFiles { get; set; } = new List<CurlFormPart>();
    }
}

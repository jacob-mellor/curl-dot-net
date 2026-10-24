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
        /// Hosts that must be reached directly even when a proxy is configured.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--noproxy LIST</c>. The value is a comma-separated list of host names, domain
        /// suffixes (<c>example.com</c> also matches <c>www.example.com</c>; a leading dot is allowed),
        /// IP addresses and CIDR ranges (<c>10.0.0.0/8</c>). <c>*</c> bypasses the proxy for every host.
        /// An empty string overrides the <c>NO_PROXY</c> environment variable and proxies everything.
        /// Default: null, which means the <c>no_proxy</c>/<c>NO_PROXY</c> environment variable applies.</para>
        /// <para>Example: <c>curl -x http://proxy:3128 --noproxy localhost,.internal https://api.internal/</c>
        /// connects to <c>api.internal</c> directly while every other host goes through the proxy.</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        public string? NoProxy { get; set; }

        /// <summary>
        /// Authentication scheme used with the proxy.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--proxy-basic</c> (<c>basic</c>, the default), <c>--proxy-digest</c> (<c>digest</c>),
        /// <c>--proxy-ntlm</c> (<c>ntlm</c>), <c>--proxy-negotiate</c> (<c>negotiate</c>) and
        /// <c>--proxy-anyauth</c> (<c>anyauth</c>). With Basic (or null) the
        /// <c>Proxy-Authorization</c> header is sent on the first request, exactly like curl; the other
        /// schemes wait for the proxy's <c>407</c> challenge and answer it.</para>
        /// <para>Example: <c>curl -x proxy:3128 -U alice:secret --proxy-digest http://example.com/</c>.</para>
        /// </remarks>
        /// <seealso cref="ProxyCredentials"/>
        public string? ProxyAuthScheme { get; set; }

        /// <summary>
        /// Tunnel through the HTTP proxy with <c>CONNECT</c> even for plain <c>http://</c> URLs.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>-p</c> / <c>--proxytunnel</c>. Without it, <c>http://</c> requests are sent to the
        /// proxy with the absolute URL in the request line (<c>GET http://host/path HTTP/1.1</c>) and
        /// <c>https://</c> requests are always tunnelled. Supported on .NET 5 and later.</para>
        /// <para>Example: <c>curl -p -x proxy:3128 http://example.com/</c> sends
        /// <c>CONNECT example.com:80 HTTP/1.1</c> first.</para>
        /// </remarks>
        public bool ProxyTunnel { get; set; }

        /// <summary>
        /// The proxy speaks HTTP/1.0, so a <c>CONNECT</c> request uses <c>HTTP/1.0</c>.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--proxy1.0 HOST[:PORT]</c>, which also sets <see cref="Proxy"/>.</para>
        /// </remarks>
        public bool ProxyHttp10 { get; set; }

        /// <summary>
        /// Keep POST (and its body) when following a 301 redirect instead of switching to GET.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--post301</c>. curl, like browsers, turns a POST into a GET on 301 unless asked
        /// not to. Example: <c>curl -L --post301 -d x=1 https://example.com/old</c>.</para>
        /// </remarks>
        /// <seealso cref="Post302"/>
        /// <seealso cref="Post303"/>
        public bool Post301 { get; set; }

        /// <summary>
        /// Keep POST (and its body) when following a 302 redirect instead of switching to GET.
        /// </summary>
        /// <remarks><para>Set by <c>--post302</c>.</para></remarks>
        /// <seealso cref="Post301"/>
        public bool Post302 { get; set; }

        /// <summary>
        /// Keep POST (and its body) when following a 303 redirect instead of switching to GET.
        /// </summary>
        /// <remarks><para>Set by <c>--post303</c>.</para></remarks>
        /// <seealso cref="Post301"/>
        public bool Post303 { get; set; }

        /// <summary>
        /// Follow redirects and let 301/302/303 change a custom <c>-X</c> method to GET, as the HTTP
        /// specification describes.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--follow</c>, which also sets <see cref="FollowLocation"/>. With plain <c>-L</c>
        /// a method given with <c>-X</c> is used for every hop.</para>
        /// </remarks>
        public bool FollowObeysMethod { get; set; }

        /// <summary>
        /// Like <see cref="FailOnError"/> (exit code 22 for HTTP status 400 and above) but the response
        /// body is still written to the output.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--fail-with-body</c>. The thrown <see cref="CurlDotNet.Exceptions.CurlHttpReturnedErrorException"/>
        /// carries the body in <c>ResponseBody</c>, and an <c>-o</c> file is written before it is thrown.</para>
        /// </remarks>
        public bool FailWithBody { get; set; }

        /// <summary>
        /// <c>--fail-early</c>: stop at the first failed transfer. Accepted for compatibility; a single
        /// command always stops at its first failure in CurlDotNet.
        /// </summary>
        public bool FailEarly { get; set; }

        /// <summary>
        /// File that receives the response header blocks.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>-D FILE</c> / <c>--dump-header FILE</c>. Every header block of the transfer is
        /// written (interim <c>1xx</c> responses, authentication challenges and each redirect hop),
        /// exactly as received. <c>-</c> writes them to the standard output, which is
        /// <see cref="CurlResult.Body"/>. A missing directory is created only with <c>--create-dirs</c>;
        /// otherwise the command fails with exit code 23.</para>
        /// <para>Example: <c>curl -D headers.txt -o page.html https://example.com/</c>.</para>
        /// </remarks>
        public string? DumpHeaderFile { get; set; }

        /// <summary>
        /// Use the file name from the server's <c>Content-Disposition</c> header for <c>-O</c>.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>-J</c> / <c>--remote-header-name</c> together with <c>-O</c>. Directory parts of the
        /// suggested name are stripped. When the header is missing, the name comes from the URL as with
        /// plain <c>-O</c>. An existing file is never overwritten (exit code 23).</para>
        /// </remarks>
        public bool RemoteHeaderName { get; set; }

        /// <summary>
        /// Directory that <c>-o</c> and <c>-O</c> file names are relative to.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--output-dir DIR</c>. The directory must exist unless <c>--create-dirs</c> is
        /// also given (exit code 23 otherwise). Example: <c>curl --output-dir downloads -O https://example.com/a.zip</c>.</para>
        /// </remarks>
        public string? OutputDir { get; set; }

        /// <summary>
        /// Give the downloaded file the server's <c>Last-Modified</c> time.
        /// </summary>
        /// <remarks><para>Set by <c>-R</c> / <c>--remote-time</c>.</para></remarks>
        public bool RemoteTime { get; set; }

        /// <summary>
        /// Never overwrite an existing output file; write to <c>name.1</c>, <c>name.2</c> ... instead.
        /// </summary>
        /// <remarks><para>Set by <c>--no-clobber</c>. Up to 100 numbered names are tried before exit code 23.</para></remarks>
        public bool NoClobber { get; set; }

        /// <summary>
        /// Delete a partially written output file when the transfer fails.
        /// </summary>
        /// <remarks><para>Set by <c>--remove-on-error</c>.</para></remarks>
        public bool RemoveOnError { get; set; }

        /// <summary>
        /// File that receives what curl would print on standard error (the <c>%{stderr}</c> part of
        /// <c>-w</c> output and error messages).
        /// </summary>
        /// <remarks><para>Set by <c>--stderr FILE</c>; <c>-</c> means standard output.</para></remarks>
        public string? StderrFile { get; set; }

        /// <summary>
        /// Discard session cookies (cookies without an expiry) read from cookie files.
        /// </summary>
        /// <remarks><para>Set by <c>-j</c> / <c>--junk-session-cookies</c>; applies to files given with <c>-b</c>.</para></remarks>
        /// <seealso cref="Cookie"/>
        /// <seealso cref="CookieJar"/>
        public bool JunkSessionCookies { get; set; }

        /// <summary>
        /// Multipart form parts in command-line order, built from <c>-F</c> and <c>--form-string</c>.
        /// </summary>
        /// <remarks>
        /// <para>Each <c>-F name=value</c>, <c>-F name=@file</c>, <c>-F name=&lt;file</c> or
        /// <c>--form-string name=value</c> adds one <see cref="CurlFormPart"/>. When the list is not
        /// empty the request body is <c>multipart/form-data</c> formatted exactly like curl's (boundary,
        /// part headers and file content types). <see cref="FormData"/> and <see cref="Files"/> are
        /// still filled for code that reads them.</para>
        /// </remarks>
        public List<CurlFormPart> FormParts { get; set; } = new List<CurlFormPart>();

        /// <summary>
        /// Escape form field and file names with backslashes instead of percent-encoding.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--form-escape</c>. By default curl writes a double quote in a <c>-F</c> name or
        /// file name as <c>%22</c> (and CR/LF as <c>%0D</c>/<c>%0A</c>); with this option it writes
        /// <c>\"</c> and doubles backslashes instead, which some older servers expect.</para>
        /// </remarks>
        public bool FormEscape { get; set; }

        /// <summary>
        /// Seconds to wait for a <c>100 Continue</c> before sending the body anyway.
        /// </summary>
        /// <remarks><para>Set by <c>--expect100-timeout SECONDS</c> (decimals allowed). Default: 1 second.</para></remarks>
        public double? Expect100TimeoutSeconds { get; set; }

        /// <summary>
        /// Largest download allowed, in bytes.
        /// </summary>
        /// <remarks>
        /// <para>Set by <c>--max-filesize BYTES</c> (suffixes k, M and G allowed). A larger announced
        /// <c>Content-Length</c> or body ends the transfer with exit code 63.</para>
        /// </remarks>
        public long? MaxFileSize { get; set; }

        /// <summary>
        /// File that receives the response's <c>ETag</c>.
        /// </summary>
        /// <remarks><para>Set by <c>--etag-save FILE</c>.</para></remarks>
        /// <seealso cref="EtagCompare"/>
        public string? EtagSave { get; set; }

        /// <summary>
        /// File holding an <c>ETag</c> to send in <c>If-None-Match</c>.
        /// </summary>
        /// <remarks><para>Set by <c>--etag-compare FILE</c>. A missing file sends an empty <c>If-None-Match: ""</c>.</para></remarks>
        /// <seealso cref="EtagSave"/>
        public string? EtagCompare { get; set; }

        /// <summary>
        /// Read the body until the server closes the connection, ignoring a wrong Content-Length.
        /// </summary>
        /// <remarks><para>Set by <c>--ignore-content-length</c>. A body shorter than the announced length
        /// is then accepted instead of failing with exit code 18.</para></remarks>
        public bool IgnoreContentLength { get; set; }

        /// <summary>
        /// Skip the transfer when the output file already exists.
        /// </summary>
        /// <remarks><para>Set by <c>--skip-existing</c>; applies to <c>-o FILE</c> and <c>-O</c>. Nothing is
        /// sent and the result is empty.</para></remarks>
        public bool SkipExisting { get; set; }

        /// <summary>
        /// Accept an HTTP/0.9 reply (a body with no status line or headers).
        /// </summary>
        /// <remarks><para>Set by <c>--http0.9</c> (and cleared by <c>--no-http0.9</c>). Without it such a reply
        /// fails with exit code 1, like curl 7.66 and later.</para></remarks>
        public bool AllowHttp09 { get; set; }

        /// <summary>
        /// Ask for a compressed transfer-encoding and decode it.
        /// </summary>
        /// <remarks><para>Set by <c>--tr-encoding</c>: sends <c>TE: gzip</c> with <c>Connection: TE</c> and
        /// decodes a <c>Transfer-Encoding: gzip</c> (or deflate) body. Without it such a response is
        /// refused with exit code 61, like curl.</para></remarks>
        public bool TransferEncoding { get; set; }

        /// <summary>
        /// Pass transfer-codings through instead of interpreting them.
        /// </summary>
        /// <remarks><para>Set by <c>--raw</c>. Unknown <c>Transfer-Encoding</c> values are then accepted and the
        /// body is read until the server closes. HttpClient always removes chunked framing, so chunk
        /// sizes are not written to the output as curl would.</para></remarks>
        public bool Raw { get; set; }

        /// <summary>
        /// Creates a member-wise copy so a redirect hop can change its URL, method or headers without
        /// touching the caller's options. Collections are shared; replace them before modifying.
        /// </summary>
        internal CurlOptions ShallowCopy() => (CurlOptions)MemberwiseClone();
    }

    /// <summary>
    /// One part of a <c>multipart/form-data</c> body, as given with <c>-F</c> or <c>--form-string</c>.
    /// </summary>
    /// <remarks>
    /// <para>Accepted forms: <c>name=value</c> (text field), <c>name=@path</c> (file upload; sends
    /// <c>filename="basename"</c> and a Content-Type guessed from the extension), <c>name=&lt;path</c>
    /// (the file's content becomes the field value) and the modifiers <c>;type=mime/type</c>,
    /// <c>;filename=name</c> and <c>;headers=Header: value</c>. Values may be double-quoted to protect
    /// <c>;</c> and <c>,</c>.</para>
    /// <para>Example: <c>curl -F "avatar=@me.png;type=image/png" -F name=Alice https://example.com/upload</c>.</para>
    /// </remarks>
    public class CurlFormPart
    {
        /// <summary>The field name sent in <c>Content-Disposition: form-data; name="..."</c>.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>The literal text value for text fields; null for file parts.</summary>
        public string? Value { get; set; }

        /// <summary>Path of the file to send (for <c>@path</c>) or to read as the value (for <c>&lt;path</c>).</summary>
        public string? FilePath { get; set; }

        /// <summary>True for <c>name=@path</c>: the file is uploaded as an attachment with a file name.</summary>
        public bool IsFileUpload { get; set; }

        /// <summary>True for <c>name=&lt;path</c>: the file's content is sent as the field value.</summary>
        public bool IsFileContent { get; set; }

        /// <summary>The <c>filename</c> parameter; defaults to the base name of <see cref="FilePath"/>.</summary>
        public string? FileName { get; set; }

        /// <summary>Explicit part Content-Type from <c>;type=</c>; null means curl's default for the part kind.</summary>
        public string? ContentType { get; set; }

        /// <summary>Extra part headers from <c>;headers=</c>.</summary>
        public List<string> Headers { get; set; } = new List<string>();

        /// <summary>
        /// The files of <c>-F "name=@a.txt,b.txt"</c>: when more than one file is named, each one is an
        /// entry here and they are sent together as a nested <c>multipart/mixed</c> part, like curl.
        /// Empty for single-file and text parts.
        /// </summary>
        public List<CurlFormPart> MixedFiles { get; set; } = new List<CurlFormPart>();
    }
}

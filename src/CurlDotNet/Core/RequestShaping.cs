/***************************************************************************
 * RequestShaping - turns parsed curl options into the exact request curl would send
 *
 * Helpers shared by CommandParser (assembling -d/--data-binary/--json bodies, -T uploads,
 * sizes) and HttpHandler.CreateRequest (headers, authorization, HTTP version, body content).
 * Behaviour follows curl's src/tool_getparam.c, src/tool_operate.c and lib/http.c.
 *
 * Copyright (C) 2024-2026 Jacob Mellor and IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>How a <c>-d</c>-family argument is turned into body bytes.</summary>
    internal enum RequestDataFlavor
    {
        /// <summary><c>-d</c> / <c>--data</c> / <c>--data-ascii</c>: <c>@file</c> is read with CR, LF and NUL removed.</summary>
        Data,

        /// <summary><c>--data-raw</c> and the already-encoded <c>--data-urlencode</c>: always literal text.</summary>
        Raw,

        /// <summary><c>--data-binary</c>: <c>@file</c> is read byte for byte.</summary>
        Binary,

        /// <summary><c>--json</c>: like binary, and joined to the previous item without a <c>&amp;</c>.</summary>
        Json,
    }

    /// <summary>The kind of HTTP request an option selects; curl refuses commands that mix two kinds.</summary>
    internal enum RequestKind
    {
        None,
        Post,
        MimePost,
        Put,
        Head,
    }

    /// <summary>One <c>-d</c>-family argument as given on the command line.</summary>
    internal sealed class RequestDataPart
    {
        public RequestDataPart(RequestDataFlavor flavor, string value, bool isFile)
        {
            Flavor = flavor;
            Value = value;
            IsFile = isFile;
        }

        /// <summary>Which option produced the item.</summary>
        public RequestDataFlavor Flavor { get; }

        /// <summary>The literal text, or the file name when <see cref="IsFile"/> is true.</summary>
        public string Value { get; }

        /// <summary>True for <c>@file</c> items.</summary>
        public bool IsFile { get; }
    }

    /// <summary>
    /// Request-shaping helpers: body assembly for the parser and header/body construction for
    /// <see cref="HttpHandler"/>.
    /// </summary>
    internal static class RequestShaping
    {
        /// <summary>
        /// Parses a curl size: a byte count with an optional <c>k</c>, <c>M</c>, <c>G</c>, <c>T</c> or
        /// <c>P</c> suffix (case-insensitive, powers of 1024). Decimals such as <c>1.5M</c> are accepted.
        /// </summary>
        internal static bool TryParseSize(string? text, out long size)
        {
            size = 0;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var value = text!.Trim();
            double multiplier = 1;
            switch (char.ToLowerInvariant(value[value.Length - 1]))
            {
                case 'k': multiplier = 1024d; break;
                case 'm': multiplier = 1024d * 1024; break;
                case 'g': multiplier = 1024d * 1024 * 1024; break;
                case 't': multiplier = 1024d * 1024 * 1024 * 1024; break;
                case 'p': multiplier = 1024d * 1024 * 1024 * 1024 * 1024; break;
                case 'b': multiplier = 1; break;
            }

            if (multiplier != 1 || char.ToLowerInvariant(value[value.Length - 1]) == 'b')
            {
                value = value.Substring(0, value.Length - 1);
            }

            if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) || number < 0)
            {
                return false;
            }

            var bytes = number * multiplier;
            if (bytes > long.MaxValue)
            {
                return false;
            }

            size = (long)bytes;
            return true;
        }

        /// <summary>
        /// Joins the <c>-d</c>-family items of a transfer into the body curl would send and stores it on
        /// <paramref name="options"/>: <see cref="CurlOptions.Data"/> gets a readable copy and the exact
        /// bytes are kept for the request. A single <c>@file</c> item keeps the <c>@file</c> spelling in
        /// <see cref="CurlOptions.Data"/>; when that one file does not exist it is left for the request to
        /// report (<see cref="CurlFileCouldntReadException"/>), as before.
        /// </summary>
        /// <exception cref="CurlReadErrorException">Exit code 26 when one of several <c>@file</c> items cannot be read.</exception>
        internal static void BuildRequestBody(CurlOptions options)
        {
            var parts = options.DataParts;
            if (parts.Count == 0)
            {
                return;
            }

            if (parts.Count == 1 && parts[0].IsFile && parts[0].Value != "-" && !File.Exists(parts[0].Value))
            {
                options.Data = "@" + parts[0].Value;
                options.ParsedDataBytes = null;
                options.ParsedDataText = null;
                return;
            }

            var body = new MemoryStream();
            foreach (var part in parts)
            {
                // curl adds '&' between items, except in front of --json items
                if (body.Length > 0 && part.Flavor != RequestDataFlavor.Json)
                {
                    body.WriteByte((byte)'&');
                }

                var bytes = part.IsFile
                    ? ReadDataFile(part.Value, part.Flavor == RequestDataFlavor.Data, options.OriginalCommand)
                    : Encoding.UTF8.GetBytes(part.Value);
                body.Write(bytes, 0, bytes.Length);
            }

            var all = body.ToArray();
            var text = parts.Count == 1 && parts[0].IsFile ? "@" + parts[0].Value : Encoding.UTF8.GetString(all);
            options.Data = text;
            options.ParsedDataText = text;
            options.ParsedDataBytes = all;
        }

        /// <summary>
        /// Reads an <c>@file</c> data item. <c>-d @file</c> drops CR, LF and NUL bytes like curl;
        /// <c>--data-binary @file</c> and <c>--json @file</c> keep every byte.
        /// </summary>
        /// <exception cref="CurlReadErrorException">Exit code 26 when the file cannot be read or is standard input.</exception>
        internal static byte[] ReadDataFile(string path, bool stripNewlines, string? command)
        {
            var bytes = ReadFileForOption(path, "-d @file", command);
            if (!stripNewlines)
            {
                return bytes;
            }

            return bytes.Where(b => b != (byte)'\r' && b != (byte)'\n' && b != 0).ToArray();
        }

        /// <summary>
        /// Reads a file named on the command line, turning every I/O failure into curl's read error
        /// (exit code 26) with a message that says which option needed it and how to fix it.
        /// </summary>
        /// <exception cref="CurlReadErrorException">Exit code 26 when the file is missing, unreadable or is <c>-</c> (standard input).</exception>
        internal static byte[] ReadFileForOption(string path, string option, string? command)
        {
            if (path == "-")
            {
                throw new CurlReadErrorException(
                    path,
                    CurlException.Compose(
                        $"{option} was given \"-\", which in curl means \"read standard input\"; CurlDotNet runs inside your process and has no standard input to read.",
                        "Save the content to a file and pass its name instead (for example -d @body.json or -T upload.bin), or pass the text inline (-d '{\"name\":\"Ada\"}').",
                        DocLinks.HowToUpload),
                    command);
            }

            try
            {
                return File.ReadAllBytes(path);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException || ex is NotSupportedException)
            {
                var (reason, fix) = DescribeReadFailure(ex);
                throw new CurlReadErrorException(
                    path,
                    CurlException.Compose($"Failed to open/read local data from file/application: {option} could not read \"{path}\": {reason}.", fix, DocLinks.HowToUpload),
                    command, ex);
            }
        }

        /// <summary>
        /// Why reading a file named on the command line failed, and how to fix it, for each kind of I/O failure.
        /// </summary>
        internal static (string Reason, string Fix) DescribeReadFailure(Exception ex)
        {
            if (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                return ("the file does not exist",
                    $"Check the spelling of the path; a relative path is resolved against the current directory, {Directory.GetCurrentDirectory()}. " +
                    "Pass a full path (for example @/home/me/body.json or @C:\\data\\body.json) to be sure.");
            }
            if (ex is UnauthorizedAccessException)
            {
                return ($"access is denied ({ex.Message.TrimEnd('.')})",
                    "Give the current user read permission on the file, or copy it somewhere readable; naming a folder instead of a file also gives this error.");
            }
            if (ex is ArgumentException || ex is NotSupportedException || ex is PathTooLongException)
            {
                return ($"it is not a valid file name ({ex.Message.TrimEnd('.')})",
                    "Remove characters that are not allowed in file names (such as < > | or a stray quote) and check that the path is not too long.");
            }
            return ($"the read failed ({ex.GetType().Name}: {ex.Message.TrimEnd('.')})",
                "Check that no other program has the file locked and that the disk or network share is available, then run the command again.");
        }

        /// <summary>
        /// The URL curl would request: --path-as-is keeps dot segments (on .NET 6 and later, where the URI
        /// can be created without canonicalisation).
        /// </summary>
        internal static Uri BuildRequestUri(CurlOptions options)
        {
            try
            {
                return CreateRequestUri(options);
            }
            catch (UriFormatException ex)
            {
                throw new CurlMalformedUrlException(options.Url, CurlException.Compose(
                    $"\"{ErrorText.SafeUrl(options.Url)}\" cannot be turned into a request URI ({ex.Message.TrimEnd('.')}).",
                    "Check the scheme (https://), the host name and the port, and percent-encode spaces and other special characters (a space is %20).",
                    DocLinks.Url), options.OriginalCommand);
            }
        }

        private static Uri CreateRequestUri(CurlOptions options)
        {
#if NET6_0_OR_GREATER
            if (options.PathAsIs || NeedsVerbatimPath(options.Url))
            {
                // The fragment is never sent; without canonicalisation .NET would keep it in the request target
                var url = options.Url;
                var hash = url.IndexOf('#');
                if (hash >= 0)
                {
                    url = url.Substring(0, hash);
                }

                return new Uri(url, new UriCreationOptions { DangerousDisablePathAndQueryCanonicalization = true });
            }
#endif
            return new Uri(options.Url);
        }

        /// <summary>
        /// True when the URL's path or query holds characters that curl sends as they are but .NET would
        /// percent-encode or rewrite (<c>" { } | \ ^ ` &lt; &gt;</c>, curl tests 64 and 214), and nothing that
        /// needs .NET's canonicalisation (dot segments, spaces, control or non-ASCII characters).
        /// </summary>
        internal static bool NeedsVerbatimPath(string url)
        {
            var schemeEnd = url.IndexOf("://", StringComparison.Ordinal);
            if (schemeEnd < 0)
            {
                return false;
            }
            var pathStart = url.IndexOf('/', schemeEnd + 3);
            if (pathStart < 0)
            {
                return false;
            }
            var hash = url.IndexOf('#', pathStart);
            var target = hash >= 0 ? url.Substring(pathStart, hash - pathStart) : url.Substring(pathStart);

            var special = false;
            foreach (var c in target)
            {
                if (c <= ' ' || c >= 0x7F)
                {
                    return false;
                }
                if ("\"{}|\\^`<>".IndexOf(c) >= 0)
                {
                    special = true;
                }
            }
            if (!special)
            {
                return false;
            }

            var path = target.Split('?')[0].Replace('\\', '/');
            foreach (var segment in path.Split('/'))
            {
                if (segment == "." || segment == "..")
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// The credentials for the Authorization header: <c>-u</c> wins, then <c>user:password@</c> in the
        /// URL being requested (percent-decoded), matching curl.
        /// </summary>
        internal static NetworkCredential? GetCredentials(CurlOptions options, Uri uri)
        {
            var fromUrl = GetUrlCredentials(uri);
            if (options.Credentials != null && !options.CredentialsFromUrl)
            {
                return options.Credentials; // -u wins over the URL and over .netrc
            }

            // -n / --netrc-optional: the .netrc entry for this host (looked up again on every redirect hop)
            return Netrc.Resolve(options, uri, fromUrl ?? options.Credentials);
        }

        /// <summary>Decodes <c>user[:password]</c> from a URL's user-info part, or returns null when there is none.</summary>
        internal static NetworkCredential? GetUrlCredentials(Uri uri)
        {
            if (!uri.IsAbsoluteUri || string.IsNullOrEmpty(uri.UserInfo))
            {
                return null;
            }

            var userInfo = uri.UserInfo;
            var colon = userInfo.IndexOf(':');
            var user = colon < 0 ? userInfo : userInfo.Substring(0, colon);
            var password = colon < 0 ? string.Empty : userInfo.Substring(colon + 1);
            return new NetworkCredential(Uri.UnescapeDataString(user), Uri.UnescapeDataString(password));
        }

        /// <summary>
        /// Sets the HTTP version curl's <c>--http1.0</c>, <c>--http1.1</c>, <c>--http2</c>,
        /// <c>--http2-prior-knowledge</c>, <c>--http3</c> and <c>--http3-only</c> ask for.
        /// </summary>
        internal static void ApplyHttpVersion(HttpRequestMessage request, CurlOptions options)
        {
            Version? version;
            switch ((options.HttpVersion ?? string.Empty).Trim())
            {
                case "1.0":
                case "1":
                    version = new Version(1, 0);
                    break;
                case "1.1":
                    version = new Version(1, 1);
                    break;
                case "2":
                case "2.0":
                    version = new Version(2, 0);
                    break;
                case "3":
                case "3.0":
                    version = new Version(3, 0);
                    break;
                default:
                    version = null;
                    break;
            }

            if (version == null)
            {
                return;
            }

            request.Version = version;
#if NET5_0_OR_GREATER
            request.VersionPolicy = options.HttpVersionExact || version.Major < 2
                ? HttpVersionPolicy.RequestVersionExact
                : HttpVersionPolicy.RequestVersionOrLower;
#endif
        }

        /// <summary>
        /// Adds what curl puts on a request that travels through a plain HTTP proxy (an <c>http://</c> URL,
        /// no <c>-p</c> tunnel): <c>Proxy-Connection: Keep-Alive</c>, preemptive
        /// <c>Proxy-Authorization: Basic</c> for <c>-U</c> or <c>user:password@</c> in the proxy URL
        /// (unless <c>--proxy-digest</c> and friends ask for a challenge), and <c>--proxy-header</c> lines.
        /// </summary>
        internal static void ApplyProxyHeaders(HttpRequestMessage request, CurlOptions options, Uri uri, ISet<string> suppressed)
        {
            var proxy = GetHttpProxyUri(options, uri);
            if (proxy == null)
            {
                return;
            }

            foreach (var line in options.ProxyHeaderLines)
            {
                var colon = line.IndexOf(':');
                if (colon <= 0)
                {
                    continue;
                }

                var name = line.Substring(0, colon).Trim();
                var value = line.Substring(colon + 1).Trim();
                if (value.Length == 0)
                {
                    suppressed.Add(name);
                    continue;
                }

                request.Headers.Remove(name);
                request.Headers.TryAddWithoutValidation(name, value);
            }

            var credentials = options.ProxyCredentials ?? GetUrlCredentials(proxy);
            var scheme = options.ProxyAuthScheme ?? options.ProxyAuthMethod;
            var basic = scheme == null || string.Equals(scheme, "basic", StringComparison.OrdinalIgnoreCase);
            if (credentials != null && basic && !suppressed.Contains("Proxy-Authorization") && !request.Headers.Contains("Proxy-Authorization"))
            {
                var token = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.UserName}:{credentials.Password}"));
                request.Headers.ProxyAuthorization = new AuthenticationHeaderValue("Basic", token);
            }

            if (request.Version.Major < 2 && !suppressed.Contains("Proxy-Connection") && !request.Headers.Contains("Proxy-Connection"))
            {
                request.Headers.TryAddWithoutValidation("Proxy-Connection", "Keep-Alive");
            }
        }

        /// <summary>
        /// The HTTP proxy a request to <paramref name="uri"/> goes through, or null when it is sent directly,
        /// tunnelled, or through a SOCKS proxy. <c>--noproxy</c> (or <c>NO_PROXY</c>) exclusions are honoured.
        /// </summary>
        internal static Uri? GetHttpProxyUri(CurlOptions options, Uri uri)
        {
            if (string.IsNullOrEmpty(options.Proxy) || options.ProxyTunnelRequested ||
                !string.Equals(uri.Scheme, "http", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var text = options.Proxy!.Trim();
            if (text.IndexOf("://", StringComparison.Ordinal) < 0)
            {
                text = "http://" + text;
            }

            if (!Uri.TryCreate(text, UriKind.Absolute, out var proxy) ||
                !(proxy.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || proxy.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            var noProxy = options.NoProxyHosts ?? Environment.GetEnvironmentVariable("NO_PROXY") ?? Environment.GetEnvironmentVariable("no_proxy");
            return IsNoProxyMatch(noProxy, uri.Host) ? null : proxy;
        }

        /// <summary>curl's <c>--noproxy</c> matching: <c>*</c>, exact names, and domain suffixes (<c>example.com</c> covers <c>www.example.com</c>).</summary>
        internal static bool IsNoProxyMatch(string? noProxy, string host)
        {
            if (string.IsNullOrWhiteSpace(noProxy))
            {
                return false;
            }

            host = host.Trim('[', ']').TrimEnd('.');
            foreach (var raw in noProxy!.Split(','))
            {
                var entry = raw.Trim().TrimEnd('.');
                if (entry.Length == 0)
                {
                    continue;
                }

                if (entry == "*")
                {
                    return true;
                }

                entry = entry.TrimStart('.').Trim('[', ']');
                if (string.Equals(host, entry, StringComparison.OrdinalIgnoreCase) ||
                    host.EndsWith("." + entry, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>Formats a date the way HTTP conditional headers need it: <c>Sun, 12 Dec 1999 12:00:00 GMT</c>.</summary>
        internal static string FormatHttpDate(DateTimeOffset date)
        {
            return date.UtcDateTime.ToString("r", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// The body for <c>-d</c>/<c>--json</c> data: the exact bytes with curl's default
        /// <c>Content-Type: application/x-www-form-urlencoded</c>, a <c>-H "Content-Type: ..."</c> value
        /// instead, or no Content-Type at all after <c>-H "Content-Type:"</c>.
        /// </summary>
        internal static HttpContent CreateDataContent(byte[] body, CurlOptions options)
        {
            var content = new ByteArrayContent(body);
            ApplyContentType(content, options, "application/x-www-form-urlencoded");
            return content;
        }

        /// <summary>
        /// The body for <c>-T</c>: the file bytes, sent without a Content-Type unless <c>-H</c> gives one.
        /// </summary>
        internal static HttpContent CreateUploadContent(byte[] body, CurlOptions options)
        {
            var content = new ByteArrayContent(body);
            ApplyContentType(content, options, null);
            return content;
        }

        private static void ApplyContentType(HttpContent content, CurlOptions options, string? defaultType)
        {
            if (options.Headers.TryGetValue("Content-Type", out var contentType))
            {
                if (!string.IsNullOrEmpty(contentType))
                {
                    content.Headers.TryAddWithoutValidation("Content-Type", contentType);
                }

                return; // -H "Content-Type:" suppresses the default
            }

            if (options.EmptyHeaders.Contains("Content-Type"))
            {
                content.Headers.TryAddWithoutValidation("Content-Type", string.Empty);
                return;
            }

            if (defaultType != null)
            {
                content.Headers.TryAddWithoutValidation("Content-Type", defaultType);
            }
        }

        /// <summary>
        /// True when a body may be attached to a request with this method on the running framework:
        /// curl sends <c>-X GET -d x</c> as a GET with a body, which .NET Framework's handler refuses.
        /// </summary>
        internal static bool CanSendBody(HttpMethod method)
        {
            if (method == HttpMethod.Head)
            {
                return false;
            }

            if (method == HttpMethod.Get &&
                RuntimeInformation.FrameworkDescription.StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        /// <summary>True when <paramref name="headerName"/> belongs on HttpContent rather than on the request.</summary>
        internal static bool IsContentHeader(string headerName)
        {
            switch (headerName.ToLowerInvariant())
            {
                case "content-type":
                case "content-length":
                case "content-encoding":
                case "content-language":
                case "content-location":
                case "content-disposition":
                case "content-range":
                case "content-md5":
                case "expires":
                case "last-modified":
                case "allow":
                    return true;
                default:
                    return false;
            }
        }
    }
}

/***************************************************************************
 * CurlOptions - Parsed curl command options
 *
 * Represents all curl options after parsing
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Net;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Represents parsed curl command options.
    /// </summary>
    /// <remarks>
    /// <para>This class contains all options extracted from a curl command string.</para>
    /// <para>AI-Usage: This is the intermediate representation between curl syntax and HTTP operations.</para>
    /// </remarks>
    public partial class CurlOptions
    {
        /// <summary>
        /// The target URL.
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Options on the command line that CurlDotNet did not recognise. Execution ignores them
        /// (curl itself would exit with code 2), and <see cref="CurlEngine.Validate(string)"/> reports
        /// them so typos such as <c>--hedaer</c> are caught before a request is made.
        /// </summary>
        public List<string> UnknownOptions { get; } = new List<string>();

        /// <summary>
        /// HTTP method (GET, POST, PUT, DELETE, etc.).
        /// </summary>
        public string? Method { get; set; } = "GET";

        /// <summary>
        /// Request headers.
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Request body data.
        /// </summary>
        public string? Data { get; set; }

        /// <summary>
        /// Whether to URL encode the data (--data-urlencode flag).
        /// </summary>
        public bool DataUrlEncode { get; set; }

        /// <summary>
        /// Whether the data was provided via --data-raw, which prevents @file expansion.
        /// When true, a leading @ in <see cref="Data"/> is treated as a literal character
        /// rather than a file reference. This matches curl's --data-raw behavior.
        /// </summary>
        /// <remarks>
        /// <para>In curl, --data-raw is identical to --data except that @ has no special meaning.
        /// This is useful when the data literally starts with @ and should not be interpreted
        /// as a file reference.</para>
        /// <para>See: https://curl.se/docs/manpage.html#--data-raw</para>
        /// </remarks>
        public bool DataRaw { get; set; }

        /// <summary>
        /// Binary data for upload.
        /// </summary>
        public byte[]? BinaryData { get; set; }

        /// <summary>
        /// Form data for multipart/form-data.
        /// </summary>
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Files to upload (for -F flag with @file).
        /// </summary>
        public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Output file path (-o flag).
        /// </summary>
        public string? OutputFile { get; set; }

        /// <summary>
        /// Whether to use remote filename for output (-O flag).
        /// </summary>
        public bool UseRemoteFileName { get; set; }

        /// <summary>
        /// Whether to include headers in output (-i flag).
        /// </summary>
        public bool IncludeHeaders { get; set; }

        /// <summary>
        /// Whether to show only headers (-I or --head flag).
        /// </summary>
        public bool HeadOnly { get; set; }

        /// <summary>
        /// Whether to follow redirects (-L flag).
        /// </summary>
        public bool FollowLocation { get; set; }

        /// <summary>
        /// Alias for FollowLocation for test compatibility.
        /// </summary>
        public bool FollowRedirects
        {
            get => FollowLocation;
            set => FollowLocation = value;
        }

        /// <summary>
        /// Maximum number of redirects to follow (--max-redirs).
        /// </summary>
        public int MaxRedirects { get; set; } = 50;

        /// <summary>
        /// Whether to ignore SSL errors (-k flag).
        /// </summary>
        public bool Insecure { get; set; }

        /// <summary>
        /// Verbose output (-v flag).
        /// </summary>
        public bool Verbose { get; set; }

        /// <summary>
        /// Path to write a full diagnostic trace (request/response headers and bodies
        /// dumped as hex + ASCII), set by the <c>--trace</c> option. Use <c>"-"</c> to
        /// write the trace to standard output.
        /// </summary>
        /// <remarks>
        /// <para>The trace captures connection and proxy selection, TLS verification,
        /// the exact bytes sent and received, transfer timings, and the precise error
        /// when a request is blocked or fails. This is the recommended option for
        /// diagnosing why requests are being rejected behind a proxy.</para>
        /// <para>See: <see href="https://curl.se/docs/manpage.html#--trace"/></para>
        /// </remarks>
        public string? TraceFile { get; set; }

        /// <summary>
        /// Path to write an ASCII-only diagnostic trace (no hex column), set by the
        /// <c>--trace-ascii</c> option. Use <c>"-"</c> to write the trace to standard output.
        /// </summary>
        /// <remarks>
        /// <para>Identical to <see cref="TraceFile"/> except the payload dumps omit the
        /// hexadecimal column, producing a more compact, readable log. When both options
        /// are supplied, the ASCII format wins.</para>
        /// <para>See: <see href="https://curl.se/docs/manpage.html#--trace-ascii"/></para>
        /// </remarks>
        public string? TraceAsciiFile { get; set; }

        /// <summary>
        /// Whether to prefix every trace line with a high-resolution timestamp
        /// (<c>--trace-time</c>). Useful for spotting where time is spent or where a
        /// request stalls.
        /// </summary>
        /// <remarks>
        /// <para>See: <see href="https://curl.se/docs/manpage.html#--trace-time"/></para>
        /// </remarks>
        public bool TraceTime { get; set; }

        /// <summary>
        /// Silent mode (-s flag).
        /// </summary>
        public bool Silent { get; set; }

        /// <summary>
        /// Show error even in silent mode (-S flag).
        /// </summary>
        public bool ShowError { get; set; }

        /// <summary>
        /// Fail silently on HTTP errors (-f flag).
        /// </summary>
        public bool FailOnError { get; set; }

        /// <summary>
        /// User agent string (-A or --user-agent).
        /// </summary>
        public string? UserAgent { get; set; }

        /// <summary>
        /// Referer header (-e or --referer).
        /// </summary>
        public string? Referer { get; set; }

        /// <summary>
        /// Cookie string (-b or --cookie).
        /// </summary>
        public string? Cookie { get; set; }

        /// <summary>
        /// Cookie jar file path (-c or --cookie-jar).
        /// </summary>
        public string? CookieJar { get; set; }

        /// <summary>
        /// Basic authentication (-u or --user).
        /// </summary>
        public NetworkCredential? Credentials { get; set; }

        /// <summary>
        /// HTTP authentication scheme selected on the command line
        /// (--basic, --digest, --ntlm, --negotiate, --anyauth).
        /// Null means curl's default: preemptive Basic when credentials are present.
        /// Challenge-based schemes (digest/ntlm/negotiate/anyauth) are performed via the
        /// HTTP 401 challenge-response flow instead of a preemptive Authorization header.
        /// </summary>
        public string? AuthScheme { get; set; }

        /// <summary>
        /// Alias for Credentials for test compatibility.
        /// </summary>
        public NetworkCredential UserAuth
        {
            get => Credentials;
            set => Credentials = value;
        }

        /// <summary>
        /// Proxy URL (-x or --proxy).
        /// </summary>
        public string Proxy { get; set; }

        /// <summary>
        /// Proxy authentication (--proxy-user).
        /// </summary>
        public NetworkCredential? ProxyCredentials { get; set; }

        /// <summary>
        /// Maximum time in seconds (--max-time).
        /// </summary>
        public double? MaxTime { get; set; }

        /// <summary>
        /// Connection timeout in seconds (--connect-timeout).
        /// </summary>
        public double? ConnectTimeout { get; set; }

        /// <summary>
        /// Speed limit in bytes per second (--limit-rate).
        /// </summary>
        public long SpeedLimit { get; set; }

        /// <summary>
        /// Speed time period for limit (--speed-time).
        /// </summary>
        public int SpeedTime { get; set; }

        /// <summary>
        /// Resume from byte offset (-C or --continue-at).
        /// </summary>
        public long? ResumeFrom { get; set; }

        /// <summary>
        /// Range of bytes to request (-r or --range).
        /// </summary>
        public string? Range { get; set; }

        /// <summary>
        /// Whether to use compressed encoding (--compressed).
        /// </summary>
        public bool Compressed { get; set; }

        /// <summary>
        /// Certificate file for client authentication (--cert).
        /// </summary>
        public string? CertFile { get; set; }

        /// <summary>
        /// Certificate key file (--key).
        /// </summary>
        public string? KeyFile { get; set; }

        /// <summary>
        /// CA certificate file (--cacert).
        /// </summary>
        public string? CaCertFile { get; set; }

        /// <summary>
        /// Alias for CaCertFile for test compatibility.
        /// </summary>
        public string CaCert
        {
            get => CaCertFile;
            set => CaCertFile = value;
        }

        /// <summary>
        /// Interface to use for outgoing connections (--interface).
        /// </summary>
        public string? Interface { get; set; }

        /// <summary>
        /// HTTP version to use (--http1.0, --http1.1, --http2).
        /// </summary>
        public string? HttpVersion { get; set; }

        /// <summary>
        /// Request method override (-X flag value).
        /// </summary>
        public string? CustomMethod { get; set; }

        /// <summary>
        /// Write-out format string (-w or --write-out).
        /// </summary>
        public string? WriteOut { get; set; }

        /// <summary>
        /// Progress callback handler.
        /// </summary>
        public Action<double, long, long>? ProgressHandler { get; set; }

        /// <summary>
        /// The original command string.
        /// </summary>
        public string? OriginalCommand { get; set; }

        /// <summary>
        /// Whether to show progress bar (--progress-bar).
        /// </summary>
        public bool ProgressBar { get; set; }

        /// <summary>
        /// Whether to use TCP keepalive (--keepalive-time).
        /// </summary>
        public int? KeepAliveTime { get; set; }

        /// <summary>
        /// DNS servers to use (--dns-servers).
        /// </summary>
        public string? DnsServers { get; set; }

        /// <summary>
        /// Resolve host to IP (--resolve).
        /// </summary>
        public Dictionary<string, string> Resolve { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// FTP/SFTP commands to execute after transfer (--quote).
        /// </summary>
        public List<string> Quote { get; set; } = new List<string>();

        /// <summary>
        /// Whether to create missing directories for output (--create-dirs).
        /// </summary>
        public bool CreateDirs { get; set; }

        /// <summary>
        /// Whether to use passive mode for FTP (--ftp-pasv).
        /// </summary>
        public bool FtpPassive { get; set; } = true;

        /// <summary>
        /// Whether to use SSL/TLS for FTP (--ftp-ssl).
        /// </summary>
        public bool FtpSsl { get; set; }

        /// <summary>
        /// SOCKS proxy (--socks5).
        /// </summary>
        public string? Socks5Proxy { get; set; }

        /// <summary>
        /// Retry count (--retry).
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// Retry delay in seconds (--retry-delay).
        /// </summary>
        public double RetryDelay { get; set; }

        /// <summary>
        /// <c>--retry-all-errors</c>: retry on every failure, not only on the transient ones curl
        /// retries by default (timeouts and HTTP 408/429/500/502/503/504).
        /// </summary>
        public bool RetryAllErrors { get; set; }

        /// <summary>
        /// <c>--retry-connrefused</c>: also treat "connection refused" as a transient error worth retrying.
        /// </summary>
        public bool RetryConnRefused { get; set; }

        /// <summary>
        /// Maximum retry time (--retry-max-time).
        /// </summary>
        public int RetryMaxTime { get; set; }

        /// <summary>
        /// Location trusted for automatic authentication (--location-trusted).
        /// </summary>
        public bool LocationTrusted { get; set; }

        /// <summary>
        /// Whether to disable EPSV for FTP (--disable-epsv).
        /// </summary>
        public bool DisableEpsv { get; set; }

        /// <summary>
        /// Whether to disable EPRT for FTP (--disable-eprt).
        /// </summary>
        public bool DisableEprt { get; set; }

        /// <summary>
        /// AWS Signature Version 4 configuration for authenticating requests to AWS services.
        /// Set by the <c>--aws-sigv4</c> flag.
        /// </summary>
        /// <remarks>
        /// <para>When set, the request will be signed using the AWS Signature Version 4 algorithm.
        /// Credentials must be provided via the <see cref="Credentials"/> property (<c>--user ACCESS_KEY:SECRET_KEY</c>).</para>
        /// <para>The config format is <c>provider1[:provider2[:region[:service]]]</c>.</para>
        /// <para>Common values:</para>
        /// <list type="bullet">
        /// <item><c>"aws:amz:us-east-1:s3"</c> — AWS S3 in us-east-1</item>
        /// <item><c>"aws:amz:eu-west-1:execute-api"</c> — AWS API Gateway in eu-west-1</item>
        /// <item><c>"gcp:goog"</c> — Google Cloud Platform</item>
        /// </list>
        /// <para>See: <see href="https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html"/></para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Equivalent to: curl --aws-sigv4 "aws:amz:us-east-1:s3" --user "AKID:SECRET" https://s3.amazonaws.com/bucket
        /// var result = await Curl.RunAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket");
        /// </code>
        /// </example>
        public AwsSigV4Config? AwsSigV4 { get; set; }

        /// <summary>
        /// Forces all response content to be treated as binary data, regardless of Content-Type header.
        /// </summary>
        /// <remarks>
        /// <para>When set to <c>true</c>, the response body will always be read as a byte array
        /// using ReadAsByteArrayAsync. This prevents
        /// corruption of binary files that may have incorrect Content-Type headers from the server.</para>
        /// <para>Use this when downloading files from servers that serve binary content with
        /// text-like Content-Type headers (e.g., a server returning .xlsx files as text/plain).</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Using the fluent builder:
        /// var result = await CurlRequestBuilder
        ///     .Get("https://example.com/report.xlsx")
        ///     .AsBinary()
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public bool ForceBinary { get; set; }

        /// <summary>
        /// Additional MIME types that should be treated as binary data.
        /// </summary>
        /// <remarks>
        /// <para>These MIME types take priority over the built-in text content type detection.
        /// Use this to override the default behavior for specific MIME types that your server
        /// may serve with unexpected Content-Type headers.</para>
        /// <para>The library already recognizes common binary types including Office documents
        /// (.xlsx, .docx, .pptx), PDFs, archives, images, audio, video, and more.</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Using the fluent builder:
        /// var result = await CurlRequestBuilder
        ///     .Get("https://example.com/data")
        ///     .WithBinaryContentType("application/x-custom-binary")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public List<string> BinaryContentTypes { get; set; } = new List<string>();

        /// <summary>
        /// Additional MIME types that should be treated as text content.
        /// </summary>
        /// <remarks>
        /// <para>These MIME types take priority over the built-in binary detection (but not
        /// <see cref="BinaryContentTypes"/> which has highest priority). Use this to override
        /// the default behavior for MIME types that you know are text but that the library
        /// treats as binary by default.</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Using the fluent builder:
        /// var result = await CurlRequestBuilder
        ///     .Get("https://example.com/data")
        ///     .WithTextContentType("application/x-custom-text")
        ///     .ExecuteAsync();
        /// </code>
        /// </example>
        public List<string> TextContentTypes { get; set; } = new List<string>();

        /// <summary>
        /// Clone this options object.
        /// </summary>
        public CurlOptions Clone()
        {
            var clone = new CurlOptions
            {
                Url = Url,
                Method = Method,
                Headers = new Dictionary<string, string>(Headers),
                Data = Data,
                DataUrlEncode = DataUrlEncode,
                DataRaw = DataRaw,
                BinaryData = BinaryData,
                FormData = new Dictionary<string, string>(FormData),
                Files = new Dictionary<string, string>(Files),
                OutputFile = OutputFile,
                UseRemoteFileName = UseRemoteFileName,
                IncludeHeaders = IncludeHeaders,
                HeadOnly = HeadOnly,
                FollowLocation = FollowLocation,
                MaxRedirects = MaxRedirects,
                Insecure = Insecure,
                Verbose = Verbose,
                TraceFile = TraceFile,
                TraceAsciiFile = TraceAsciiFile,
                TraceTime = TraceTime,
                Silent = Silent,
                ShowError = ShowError,
                FailOnError = FailOnError,
                UserAgent = UserAgent,
                Referer = Referer,
                Cookie = Cookie,
                CookieJar = CookieJar,
                Credentials = Credentials,
                AuthScheme = AuthScheme,
                Proxy = Proxy,
                ProxyCredentials = ProxyCredentials,
                MaxTime = MaxTime,
                ConnectTimeout = ConnectTimeout,
                SpeedLimit = SpeedLimit,
                SpeedTime = SpeedTime,
                ResumeFrom = ResumeFrom,
                Range = Range,
                Compressed = Compressed,
                CertFile = CertFile,
                KeyFile = KeyFile,
                CaCertFile = CaCertFile,
                Interface = Interface,
                HttpVersion = HttpVersion,
                CustomMethod = CustomMethod,
                WriteOut = WriteOut,
                ProgressHandler = ProgressHandler,
                OriginalCommand = OriginalCommand,
                ProgressBar = ProgressBar,
                KeepAliveTime = KeepAliveTime,
                DnsServers = DnsServers,
                Resolve = new Dictionary<string, string>(Resolve),
                Quote = new List<string>(Quote),
                CreateDirs = CreateDirs,
                FtpPassive = FtpPassive,
                FtpSsl = FtpSsl,
                Socks5Proxy = Socks5Proxy,
                Retry = Retry,
                RetryDelay = RetryDelay,
                RetryMaxTime = RetryMaxTime,
                LocationTrusted = LocationTrusted,
                DisableEpsv = DisableEpsv,
                DisableEprt = DisableEprt,
                ForceBinary = ForceBinary,
                BinaryContentTypes = new List<string>(BinaryContentTypes),
                TextContentTypes = new List<string>(TextContentTypes),
                AwsSigV4 = AwsSigV4
            };

            CopyRequestShapingTo(clone);
            return clone;
        }

        #region Request shaping options

        /// <summary>
        /// Real curl options that appeared on the command line but that CurlDotNet does not act on yet,
        /// in the order they were given (for example <c>--tcp-nodelay</c> or <c>--ciphers</c>).
        /// </summary>
        /// <remarks>
        /// <para>The parser knows every option of curl 8.7.1 (and the newer ones used by curl's own test
        /// suite), so a recognised-but-unimplemented option still consumes its argument correctly and the
        /// URL is never mistaken for it: <c>curl --ciphers ALL https://example.com</c> requests
        /// <c>https://example.com</c>. Such options land here instead of in <see cref="UnknownOptions"/>,
        /// which only holds names that curl itself would reject with exit code 2.</para>
        /// <para>Each entry is the long form as typed, for example <c>--tcp-nodelay</c> or
        /// <c>--no-sessionid</c>; short forms are recorded by their long name (<c>-q</c> becomes
        /// <c>--disable</c>).</para>
        /// <para>Usage: after <c>var o = parser.Parse(cmd);</c> check <c>o.UnsupportedOptions.Count == 0</c>
        /// to know the request will be sent exactly as curl would send it.</para>
        /// </remarks>
        /// <seealso cref="UnknownOptions"/>
        public List<string> UnsupportedOptions { get; } = new List<string>();

        /// <summary>
        /// Send the <c>-d</c>/<c>--data*</c> data as a query string on a GET request instead of a POST
        /// body (curl <c>-G</c> / <c>--get</c>).
        /// </summary>
        /// <remarks>
        /// <para>The parser applies this itself: the joined data is appended to <see cref="Url"/> after a
        /// <c>?</c> (or <c>&amp;</c> when the URL already has a query), <see cref="Data"/> is cleared and the
        /// method stays GET (HEAD when <c>-I</c> is also given). Setting the property on hand-built options
        /// has the same effect at send time.</para>
        /// <para>Usage: <c>curl -G -d q=dotnet -d page=2 https://example.com/search</c> requests
        /// <c>GET /search?q=dotnet&amp;page=2</c>.</para>
        /// </remarks>
        /// <seealso cref="Data"/>
        public bool UseGet { get; set; }

        /// <summary>
        /// Local file uploaded as the request body with PUT (curl <c>-T</c> / <c>--upload-file</c>).
        /// </summary>
        /// <remarks>
        /// <para>The parser reads the file into <see cref="BinaryData"/> and selects PUT unless
        /// <c>-X</c> names another method. No Content-Type is sent unless a <c>-H "Content-Type: ..."</c>
        /// header is given, exactly like curl. When the URL has no path or its path ends with <c>/</c> (and
        /// there is no query string) the file name is appended to it, percent-encoded:
        /// <c>-T report.pdf https://example.com/files/</c> uploads to <c>/files/report.pdf</c>.</para>
        /// <para>A missing file fails with exit code 26 (<see cref="CurlDotNet.Exceptions.CurlReadErrorException"/>).
        /// Reading the body from standard input (<c>-T -</c> or <c>-T .</c>) is not supported.</para>
        /// <para>Usage: <c>curl -T ./backup.zip https://example.com/upload/</c>.</para>
        /// </remarks>
        /// <seealso cref="BinaryData"/>
        public string? UploadFile { get; set; }

        /// <summary>
        /// OAuth 2.0 bearer token sent as <c>Authorization: Bearer TOKEN</c> (curl <c>--oauth2-bearer</c>).
        /// </summary>
        /// <remarks>
        /// <para>The token is sent verbatim and takes precedence over Basic credentials from <c>-u</c> or
        /// the URL, as in curl. An explicit <c>-H "Authorization: ..."</c> header still wins over it.</para>
        /// <para>Usage: <c>curl --oauth2-bearer eyJhbGciOi... https://api.example.com/me</c>.</para>
        /// </remarks>
        /// <seealso cref="Credentials"/>
        public string? OAuth2Bearer { get; set; }

        /// <summary>
        /// Whether a Referer header should be set automatically when a redirect is followed
        /// (the <c>;auto</c> suffix of curl <c>-e</c> / <c>--referer</c>).
        /// </summary>
        /// <remarks>
        /// <para><c>-e ";auto"</c> sends no Referer on the first request and the previous URL on every
        /// redirect; <c>-e "https://from.example/;auto"</c> sends <c>https://from.example/</c> first.
        /// The parser strips the suffix, so <see cref="Referer"/> only ever holds the literal value.</para>
        /// <para>Usage: <c>curl -L -e ";auto" https://example.com/old-link</c>.</para>
        /// </remarks>
        /// <seealso cref="Referer"/>
        public bool AutoReferer { get; set; }

        /// <summary>
        /// The date used for a conditional request (curl <c>-z</c> / <c>--time-cond</c>), or null for none.
        /// </summary>
        /// <remarks>
        /// <para>Sent as <c>If-Modified-Since</c> or <c>If-Unmodified-Since</c> depending on
        /// <see cref="TimeConditionType"/>, always formatted as an RFC 1123 GMT date. The parser accepts the
        /// date formats curl's <c>curl_getdate</c> does (<c>"Sun, 12 Dec 1999 12:00:00 GMT"</c>,
        /// <c>"dec 12 12:00:00 1999 GMT"</c>, <c>"19991212"</c>, <c>"12 Dec 1999"</c>, ...) or the name of a
        /// local file, in which case the file's last-modified time is used. A leading <c>-</c> asks for
        /// "unmodified since", a leading <c>=</c> for an exact Last-Modified match.</para>
        /// <para>Usage: <c>curl -z "Sun, 12 Dec 1999 12:00:00 GMT" https://example.com/feed.xml</c> or
        /// <c>curl -z local.html -o local.html https://example.com/page.html</c>.</para>
        /// </remarks>
        /// <seealso cref="TimeConditionType"/>
        public DateTimeOffset? TimeCondition { get; set; }

        /// <summary>
        /// Which comparison <see cref="TimeCondition"/> requests (curl <c>-z DATE</c>, <c>-z -DATE</c>, <c>-z =DATE</c>).
        /// </summary>
        /// <remarks>
        /// <para>Defaults to <see cref="CurlTimeCondition.IfModifiedSince"/>; it is ignored while
        /// <see cref="TimeCondition"/> is null.</para>
        /// <para>Usage: <c>curl -z "-Jan 1 2024" https://example.com/</c> sends
        /// <c>If-Unmodified-Since: Mon, 01 Jan 2024 00:00:00 GMT</c>.</para>
        /// </remarks>
        /// <seealso cref="TimeCondition"/>
        public CurlTimeCondition TimeConditionType { get; set; } = CurlTimeCondition.IfModifiedSince;

        /// <summary>
        /// Largest response body, in bytes, the transfer accepts (curl <c>--max-filesize</c>), or null for no limit.
        /// </summary>
        /// <remarks>
        /// <para>The parser accepts a plain byte count or one with a <c>k</c>, <c>M</c>, <c>G</c>, <c>T</c> or
        /// <c>P</c> suffix (powers of 1024), for example <c>--max-filesize 10M</c>. curl stops with exit code
        /// 63 when the announced or received size exceeds the limit.</para>
        /// <para>Usage: <c>curl --max-filesize 500k https://example.com/avatar.png</c>.</para>
        /// </remarks>
        public long? MaxFileSize { get; set; }

        /// <summary>
        /// Require HTTP/2 without negotiation (curl <c>--http2-prior-knowledge</c>), or HTTP/3 only
        /// (<c>--http3-only</c>), instead of letting the connection fall back to an older protocol.
        /// </summary>
        /// <remarks>
        /// <para>Works together with <see cref="HttpVersion"/>, which holds <c>"1.0"</c>, <c>"1.1"</c>,
        /// <c>"2.0"</c> or <c>"3.0"</c>. When this is true the request uses
        /// <c>HttpVersionPolicy.RequestVersionExact</c> (on .NET 5 and later), so a server that cannot speak
        /// the requested version fails the request rather than silently downgrading.</para>
        /// <para>Usage: <c>curl --http2-prior-knowledge http://localhost:5000/grpc-health</c>.</para>
        /// </remarks>
        /// <seealso cref="HttpVersion"/>
        public bool HttpVersionExact { get; set; }

        /// <summary>
        /// Send the URL path exactly as written, without removing <c>/./</c> and <c>/../</c> segments
        /// (curl <c>--path-as-is</c>).
        /// </summary>
        /// <remarks>
        /// <para>Honoured on .NET 6 and later, where the request URI is created without path
        /// canonicalisation; older frameworks always normalise the path.</para>
        /// <para>Usage: <c>curl --path-as-is http://localhost:8080/../../etc/passwd</c>.</para>
        /// </remarks>
        public bool PathAsIs { get; set; }

        /// <summary>
        /// Turn off URL globbing, so <c>{}</c> and <c>[]</c> in URLs are sent literally (curl <c>-g</c> / <c>--globoff</c>).
        /// </summary>
        /// <remarks>
        /// <para>Without it, <c>curl "https://example.com/{a,b}"</c> and <c>"https://example.com/[1-3]"</c>
        /// expand into several URLs (see <see cref="AdditionalTransfers"/>); with it, the brackets are
        /// percent-encoded and sent as one request.</para>
        /// <para>Usage: <c>curl -g "http://[::1]:8080/"</c> for IPv6 literals.</para>
        /// </remarks>
        public bool GlobOff { get; set; }

        /// <summary>
        /// Header names given with curl's <c>-H "Name;"</c> syntax, which sends the header with an empty value.
        /// </summary>
        /// <remarks>
        /// <para>curl distinguishes <c>-H "X-Empty;"</c> (send <c>X-Empty:</c> with no value) from
        /// <c>-H "X-Empty:"</c> (do not send the header at all, even one curl would add itself). The
        /// latter is stored in <see cref="Headers"/> with an empty value; the former is stored here.
        /// Names compare case-insensitively.</para>
        /// <para>Usage: <c>curl -H "X-Requested-With;" https://example.com/</c>.</para>
        /// </remarks>
        /// <seealso cref="Headers"/>
        public HashSet<string> EmptyHeaders { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Further transfers described by the same command line: extra URLs, URLs produced by globbing
        /// (<c>{a,b}</c>, <c>[1-3]</c>) and groups separated by <c>--next</c> / <c>-:</c>.
        /// </summary>
        /// <remarks>
        /// <para>The options object returned by the parser describes the first transfer; every entry here
        /// is a complete, independent <see cref="CurlOptions"/> for the next one, in command-line order.
        /// The options of a <c>--next</c> group apply to every URL of that group wherever they appear, and
        /// the nth <c>-o</c>/<c>-O</c>/<c>-T</c> belongs to the nth URL, exactly as in curl:
        /// <c>curl -H "X: 1" https://a.example/ https://b.example/ --next -d x=1 https://c.example/</c>
        /// yields a first transfer for <c>a</c> and two additional ones, <c>b</c> (with the header) and
        /// <c>c</c> (a POST without it).</para>
        /// <para>An empty list means the command describes a single transfer. Executing a command string
        /// runs the first transfer; to run the others, pass each entry to
        /// <c>Curl.ExecuteAsync(CurlOptions)</c> in order.</para>
        /// </remarks>
        public List<CurlOptions> AdditionalTransfers { get; } = new List<CurlOptions>();

        /// <summary>
        /// Values from <c>#1</c>, <c>#2</c>... placeholders: the text each URL glob matched for this transfer.
        /// </summary>
        /// <remarks>
        /// <para>For <c>curl "https://example.com/{one,two}/[1-2].txt" -o "#1-#2.txt"</c> the transfer for
        /// <c>/two/1.txt</c> holds <c>["two", "1"]</c>, and the parser has already rewritten
        /// <see cref="OutputFile"/> to <c>two-1.txt</c>. Empty when the URL has no globs.</para>
        /// </remarks>
        public List<string> GlobMatches { get; } = new List<string>();

        /// <summary>
        /// Raw <c>-d</c> family items gathered while parsing, before they are joined into <see cref="Data"/>.
        /// </summary>
        internal List<RequestDataPart> DataParts { get; } = new List<RequestDataPart>();

        /// <summary>Header names in <see cref="Headers"/> that an option such as <c>--json</c> added, which a later <c>-H</c> replaces.</summary>
        internal HashSet<string> ImplicitHeaders { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary><c>-p</c> / <c>--proxytunnel</c>: requests go through a CONNECT tunnel, so no proxy headers are added to them.</summary>
        internal bool ProxyTunnelRequested { get; set; }

        /// <summary><c>--noproxy</c> host list (comma separated, <c>*</c> for all); null means "use NO_PROXY from the environment".</summary>
        internal string? NoProxyHosts { get; set; }

        /// <summary><c>--proxy-header</c> lines, sent on requests that go through a plain HTTP proxy.</summary>
        internal List<string> ProxyHeaderLines { get; } = new List<string>();

        /// <summary>The proxy authentication method (<c>--proxy-basic</c>, <c>--proxy-digest</c>...); null means Basic.</summary>
        internal string? ProxyAuthMethod { get; set; }


        /// <summary>The scheme <c>--proto-default</c> gives URLs written without one.</summary>
        internal string? DefaultProtocol { get; set; }

        /// <summary>
        /// Cookie files from earlier <c>-b FILE</c> options; <see cref="Cookie"/> holds the last file or the
        /// joined <c>-b name=value</c> strings. curl reads every file and sends every string.
        /// </summary>
        internal List<string> CookieFiles { get; set; } = new List<string>();

        /// <summary>
        /// State shared by every transfer of one command line (curl's share handle): the cookie engine.
        /// Null when the options run on their own.
        /// </summary>
        internal TransferSession? Session { get; set; }

        /// <summary>
        /// The error curl reports when it sets this transfer up (after the earlier transfers ran), such as
        /// a missing <c>-T</c> file of a later URL; the transfer is not sent and the command ends with it.
        /// </summary>
        internal CurlDotNet.Exceptions.CurlException? SetupError { get; set; }

        /// <summary>
        /// <c>-H</c> lines whose name HttpClient cannot write as given (white space around or inside the
        /// name), keyed by the trimmed name; the request head carries them verbatim like curl.
        /// </summary>
        internal Dictionary<string, string> RawHeaderLines { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary><c>--request-target</c>: the target written on the request line instead of the URL's path (for example <c>*</c>).</summary>
        internal string? RequestTarget { get; set; }

        /// <summary>The 0-based <c>--next</c> group this transfer belongs to.</summary>
        internal int GroupNumber { get; set; }

        /// <summary>The 0-based position of this transfer among all URLs of the command line (<c>%{urlnum}</c>).</summary>
        internal int TransferNumber { get; set; }

        /// <summary><c>--out-null</c>: the response headers (<c>-i</c>) and body are read and discarded.</summary>
        internal bool DiscardOutput { get; set; }

        /// <summary><c>--connect-to HOST1:PORT1:HOST2:PORT2</c> entries, in command-line order (the first match wins).</summary>
        internal List<string> ConnectTo { get; set; } = new List<string>();

        /// <summary><c>-n</c>/<c>--netrc</c>, <c>--netrc-optional</c>: whether credentials come from a .netrc file.</summary>
        internal NetrcMode NetrcMode { get; set; }

        /// <summary><c>--netrc-file FILE</c>: the .netrc file to read instead of <c>$NETRC</c> or <c>~/.netrc</c>.</summary>
        internal string? NetrcFile { get; set; }

        /// <summary>
        /// Set on a redirect hop to another host, port or scheme without <c>--location-trusted</c>: the
        /// <c>--oauth2-bearer</c> token is not sent there (and does not fall back to Basic).
        /// </summary>
        internal bool CrossOriginHop { get; set; }

        /// <summary><c>--url-query</c> arguments, applied to <see cref="Url"/> when the transfer is finished.</summary>
        internal List<string> UrlQueryParts { get; } = new List<string>();

        /// <summary>
        /// True when <see cref="Credentials"/> came from <c>user:password@</c> in the URL rather than from
        /// <c>-u</c>; each request (redirects included) then uses the user-info of the URL it targets.
        /// </summary>
        internal bool CredentialsFromUrl { get; set; }

        /// <summary>
        /// The kinds of request the options asked for (POST, multipart, PUT, HEAD) with the option that asked,
        /// checked for conflicts when the transfer is finished.
        /// </summary>
        internal List<KeyValuePair<RequestKind, string>> RequestKinds { get; } = new List<KeyValuePair<RequestKind, string>>();

        /// <summary>
        /// Exact body bytes the parser assembled from <see cref="DataParts"/>; only used while <see cref="Data"/>
        /// still equals <see cref="ParsedDataText"/> (so hand edits to <see cref="Data"/> win).
        /// </summary>
        internal byte[]? ParsedDataBytes { get; set; }

        /// <summary>The value <see cref="Data"/> had when <see cref="ParsedDataBytes"/> was assembled.</summary>
        internal string? ParsedDataText { get; set; }

        /// <summary>
        /// Returns the request body the parser assembled for this transfer, or null when the body should be
        /// built from <see cref="Data"/> as set by hand.
        /// </summary>
        internal byte[]? GetParsedBody()
        {
            return ParsedDataBytes != null && string.Equals(Data, ParsedDataText, StringComparison.Ordinal)
                ? ParsedDataBytes
                : null;
        }

        private void CopyRequestShapingTo(CurlOptions clone)
        {
            clone.UnsupportedOptions.AddRange(UnsupportedOptions);
            clone.UnknownOptions.AddRange(UnknownOptions);
            clone.UseGet = UseGet;
            clone.UploadFile = UploadFile;
            clone.OAuth2Bearer = OAuth2Bearer;
            clone.AutoReferer = AutoReferer;
            clone.TimeCondition = TimeCondition;
            clone.TimeConditionType = TimeConditionType;
            clone.MaxFileSize = MaxFileSize;
            clone.HttpVersionExact = HttpVersionExact;
            clone.PathAsIs = PathAsIs;
            clone.GlobOff = GlobOff;
            clone.EmptyHeaders = new HashSet<string>(EmptyHeaders, StringComparer.OrdinalIgnoreCase);
            clone.GlobMatches.AddRange(GlobMatches);
            clone.DataParts.AddRange(DataParts);
            clone.UrlQueryParts.AddRange(UrlQueryParts);
            clone.DefaultProtocol = DefaultProtocol;
            clone.CookieFiles = new List<string>(CookieFiles);
            clone.DiscardOutput = DiscardOutput;
            clone.NetrcMode = NetrcMode;
            clone.RequestTarget = RequestTarget;
            clone.RawHeaderLines = new Dictionary<string, string>(RawHeaderLines, StringComparer.OrdinalIgnoreCase);
            clone.ConnectTo = new List<string>(ConnectTo);
            clone.NetrcFile = NetrcFile;
            clone.ProxyTunnelRequested = ProxyTunnelRequested;
            clone.NoProxyHosts = NoProxyHosts;
            clone.ProxyHeaderLines.AddRange(ProxyHeaderLines);
            clone.ProxyAuthMethod = ProxyAuthMethod;
            clone.ProxyHttp10 = ProxyHttp10;
            clone.ImplicitHeaders.UnionWith(ImplicitHeaders);
            clone.CredentialsFromUrl = CredentialsFromUrl;
            clone.RequestKinds.AddRange(RequestKinds);
            clone.ParsedDataBytes = ParsedDataBytes;
            clone.ParsedDataText = ParsedDataText;
        }

        #endregion
    }

    /// <summary>
    /// The comparison a conditional request makes against the server's Last-Modified time
    /// (curl <c>-z</c> / <c>--time-cond</c>).
    /// </summary>
    /// <remarks>
    /// <para>Usage: set <see cref="CurlOptions.TimeCondition"/> to the date and this to the comparison, or
    /// write it curl's way: <c>-z DATE</c>, <c>-z -DATE</c> or <c>-z =DATE</c>.</para>
    /// </remarks>
    /// <seealso cref="CurlOptions.TimeCondition"/>
    public enum CurlTimeCondition
    {
        /// <summary>Only fetch the document if it changed after the date: sends <c>If-Modified-Since</c> (<c>-z DATE</c>).</summary>
        IfModifiedSince = 0,

        /// <summary>Only fetch the document if it did not change after the date: sends <c>If-Unmodified-Since</c> (<c>-z -DATE</c>).</summary>
        IfUnmodifiedSince = 1,

        /// <summary>Only accept a document whose Last-Modified equals the date; no request header is sent (<c>-z =DATE</c>).</summary>
        LastModified = 2,
    }
}
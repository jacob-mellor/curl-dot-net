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
using System.Linq;
using System.Net;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Every setting of one curl transfer as a typed object: the URL, method, headers, body, authentication,
    /// timeouts, proxy, TLS, output files and diagnostics that a curl command line can express.
    /// </summary>
    /// <remarks>
    /// <para>There are three ways to get one. (1) Parse a command: <c>Curl.Validate("curl -X POST -d a=1 https://api.example.com").ParsedOptions</c>
    /// returns the options exactly as the parser read them, which is the quickest way to see what a pasted command
    /// will send. (2) Use the fluent builder: <c>CurlRequestBuilder.Post(url).WithJson(body).GetOptions()</c>.
    /// (3) Set the properties yourself and run them with <see cref="Curl.ExecuteAsync(CurlOptions)"/>; nothing is
    /// parsed or shell-quoted then, so values containing quotes, <c>$</c> or spaces need no escaping.</para>
    /// <para>Every property names the curl flag it corresponds to. Example: <c>curl -L -m 30 -H "Accept: application/json" https://api.example.com/items</c>
    /// is <c>new CurlOptions { Url = "https://api.example.com/items", FollowLocation = true, MaxTime = 30, Headers = { ["Accept"] = "application/json" } }</c>,
    /// and <c>await Curl.ExecuteAsync(options)</c> returns the same <see cref="CurlResult"/> the command string would.</para>
    /// <para>Hand-built options follow the classic rules: set <see cref="Method"/> to <c>"POST"</c> (or another
    /// method with a body) when you set <see cref="Data"/>, because a GET or HEAD built this way sends no body,
    /// whereas the parser picks POST for <c>-d</c> automatically.</para>
    /// <para>Thread safety: an instance is not thread-safe. Executing it normalises <see cref="Url"/> and applies
    /// the process-wide <c>Curl.Default*</c> settings to it, so use one instance per request (or call
    /// <see cref="Clone"/>) instead of sharing one object between concurrent calls.</para>
    /// <para>A few properties are recorded for compatibility but cannot be acted on (their remarks say so):
    /// <see cref="DnsServers"/>, <see cref="Interface"/>, <see cref="Quote"/>, <see cref="DisableEpsv"/> and
    /// <see cref="DisableEprt"/>, which the parser also lists in <see cref="UnsupportedOptions"/>, and the terminal-only
    /// <see cref="ProgressBar"/> and <see cref="ShowError"/>. The TLS options (<see cref="CaCertFile"/>, <see cref="CaPath"/>,
    /// <see cref="CertFile"/>, <see cref="KeyFile"/>, <see cref="PinnedPublicKey"/>, <see cref="Ciphers"/>) need .NET 5 or
    /// later; on .NET Standard 2.0 and .NET Framework they are listed in <see cref="UnsupportedOptions"/> as well.</para>
    /// <para>How a command becomes a request: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/how-curldotnet-works/README.md
    /// and turning curl commands into C#: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/convert-curl-to-csharp/README.md</para>
    /// </remarks>
    /// <example>
    /// <code language="csharp">
    /// // curl -X POST -H "Content-Type: application/json" -d '{"name":"Ada"}' -m 10 https://api.example.com/users
    /// var options = new CurlOptions
    /// {
    ///     Url = "https://api.example.com/users",
    ///     Method = "POST",
    ///     Data = "{\"name\":\"Ada\"}",
    ///     MaxTime = 10
    /// };
    /// options.Headers["Content-Type"] = "application/json";
    /// var result = await Curl.ExecuteAsync(options);
    /// </code>
    /// </example>
    /// <seealso cref="Curl.ExecuteAsync(CurlOptions)"/>
    /// <seealso cref="Curl.ExecuteAsync(CurlOptions, System.Threading.CancellationToken)"/>
    /// <seealso cref="Curl.Validate(string)"/>
    /// <seealso cref="CurlRequestBuilder.GetOptions"/>
    /// <seealso cref="CurlSettings"/>
    public partial class CurlOptions
    {
        /// <summary>
        /// The URL to request, such as <c>https://api.example.com/v1/items?page=2</c>; the one required setting,
        /// equivalent to the URL argument (or <c>--url</c>) of a curl command.
        /// </summary>
        /// <value>An absolute URL with scheme <c>http</c>, <c>https</c>, <c>ftp</c>, <c>ftps</c> or <c>file</c>.
        /// Default: empty string, which fails with exit code 2 ("No URL specified") when executed.</value>
        /// <remarks>
        /// <para>A URL without a scheme (<c>example.com/path</c>) gets <c>http://</c>, like curl. User info in the URL
        /// (<c>https://alice:secret@host/</c>) becomes Basic credentials for that host. Spaces and other characters that
        /// are not valid in a URL must be percent-encoded (<c>%20</c>); with a command string, curl globbing
        /// (<c>{a,b}</c>, <c>[1-3]</c>) is expanded by the parser before this property is set (see <see cref="GlobOff"/>).</para>
        /// <para>Executing the options normalises this property in place (for example adding the missing scheme).</para>
        /// <para>Example: <c>curl "https://api.example.com/search?q=dotnet"</c> is <c>new CurlOptions { Url = "https://api.example.com/search?q=dotnet" }</c>.
        /// An unsupported scheme fails with <see cref="CurlDotNet.Exceptions.CurlUnsupportedProtocolException"/> (exit 1),
        /// a malformed URL with <see cref="CurlDotNet.Exceptions.CurlMalformedUrlException"/> (exit 3); see
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md</para>
        /// </remarks>
        /// <seealso cref="Method"/>
        /// <seealso cref="UseGet"/>
        /// <seealso cref="GlobOff"/>
        /// <seealso cref="CurlRequestBuilder.Get(string)"/>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Options in a parsed command that are not curl options at all (typos such as <c>--hedaer</c>); they are
        /// ignored when the request runs, so check this list to catch mistakes before sending.
        /// </summary>
        /// <value>The options exactly as typed, in command-line order; empty for hand-built options.</value>
        /// <remarks>
        /// <para>curl itself would stop with exit code 2 on these. CurlDotNet still sends the request, and
        /// <see cref="Curl.Validate(string)"/> reports them so that a pasted command with a typo is caught early.
        /// Real curl options that CurlDotNet merely does not implement go to <see cref="UnsupportedOptions"/> instead.</para>
        /// <para>Example: <c>var v = Curl.Validate("curl --hedaer 'X: 1' https://example.com");</c> gives
        /// <c>v.IsValid == false</c> and <c>v.ParsedOptions.UnknownOptions</c> contains <c>--hedaer</c>.</para>
        /// </remarks>
        /// <seealso cref="UnsupportedOptions"/>
        /// <seealso cref="Curl.Validate(string)"/>
        public List<string> UnknownOptions { get; private set; } = new List<string>();

        /// <summary>
        /// The HTTP method sent on the request line (<c>GET</c>, <c>POST</c>, <c>PUT</c>, <c>DELETE</c>, <c>PATCH</c>,
        /// <c>HEAD</c>, <c>OPTIONS</c>); curl chooses it from <c>-X</c>, <c>-d</c>, <c>-F</c>, <c>-T</c> and <c>-I</c>.
        /// </summary>
        /// <value>An upper-case method name. Default: <c>"GET"</c>. Null is treated as GET.</value>
        /// <remarks>
        /// <para>Only the seven names above are recognised here; any other value is sent as GET. For WebDAV and other
        /// custom verbs (<c>PROPFIND</c>, <c>MKCOL</c>, <c>PURGE</c>) set <see cref="CustomMethod"/>, which is what
        /// <c>-X</c> does and which is sent exactly as written.</para>
        /// <para>A hand-built GET or HEAD sends no body even when <see cref="Data"/> is set; set <c>Method = "POST"</c>
        /// (or PUT/PATCH) for a body. The parser does this for you: <c>-d</c> selects POST, <c>-T</c> PUT, <c>-I</c> HEAD.</para>
        /// <para>Example: <c>curl -X DELETE https://api.example.com/items/7</c> is
        /// <c>new CurlOptions { Url = "https://api.example.com/items/7", Method = "DELETE" }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="CustomMethod"/>
        /// <seealso cref="HeadOnly"/>
        /// <seealso cref="Data"/>
        /// <seealso cref="CurlRequestBuilder.Request(string, string)"/>
        public string? Method { get; set; } = "GET";

        /// <summary>
        /// Request headers to send, keyed by header name (<c>Accept</c>, <c>Authorization</c>, <c>X-Api-Key</c>...),
        /// exactly what curl's <c>-H "Name: value"</c> adds; names compare case-insensitively.
        /// </summary>
        /// <value>A dictionary of name to value (not <c>"Name: value"</c> lines). Default: empty, which sends only
        /// curl's own headers (<c>Host</c>, <c>User-Agent</c>, <c>Accept: */*</c>, plus body headers).</value>
        /// <remarks>
        /// <para>An empty value removes a header curl would send itself: <c>Headers["Accept"] = ""</c> is
        /// <c>-H "Accept:"</c>. To send a header with an empty value use <see cref="EmptyHeaders"/> (<c>-H "Name;"</c>).
        /// A second <c>-H</c> with the same name is joined to the first with <c>", "</c> by the parser. <c>Content-Type</c>
        /// and other content headers are applied to the body; <c>Host</c> replaces the Host header; <c>User-Agent</c>
        /// is overridden by <see cref="UserAgent"/> when that is set. Values must not contain CR or LF.</para>
        /// <para>Example: <c>curl -H "Authorization: Bearer abc123" -H "Accept: application/json" URL</c> is
        /// <c>options.Headers["Authorization"] = "Bearer abc123"; options.Headers["Accept"] = "application/json";</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="EmptyHeaders"/>
        /// <seealso cref="UserAgent"/>
        /// <seealso cref="Referer"/>
        /// <seealso cref="CurlRequestBuilder.WithHeader(string, string)"/>
        /// <seealso cref="CurlSettings.Headers"/>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// The request body text, as given with curl's <c>-d</c>/<c>--data</c>; a leading <c>@</c> names a file whose
        /// bytes are sent instead, unless <see cref="DataRaw"/> is true.
        /// </summary>
        /// <value>The body text, sent as UTF-8. Default: null (no body).</value>
        /// <remarks>
        /// <para>Without a <c>Content-Type</c> entry in <see cref="Headers"/> the body is labelled
        /// <c>application/x-www-form-urlencoded</c>, like curl's <c>-d</c>; add <c>Headers["Content-Type"] = "application/json"</c>
        /// for JSON. For hand-built options, set <see cref="Method"/> to POST/PUT/PATCH: a GET or HEAD sends no body.
        /// <c>Data = "@payload.json"</c> reads that file as raw bytes; a missing file fails with
        /// <see cref="CurlDotNet.Exceptions.CurlFileCouldntReadException"/> (exit 37).</para>
        /// <para>For parsed commands, several <c>-d</c> values are joined with <c>&amp;</c> and the exact bytes curl would send
        /// (including <c>--data-binary</c> and <c>--json</c> parts) are kept internally; editing this property afterwards
        /// replaces them with your text.</para>
        /// <para>Example: <c>curl -d '{"name":"Ada"}' -H "Content-Type: application/json" https://api.example.com/users</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", Data = "{\"name\":\"Ada\"}", Headers = { ["Content-Type"] = "application/json" } }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="DataRaw"/>
        /// <seealso cref="BinaryData"/>
        /// <seealso cref="UseGet"/>
        /// <seealso cref="CurlRequestBuilder.WithData(string)"/>
        /// <seealso cref="CurlRequestBuilder.WithJson(object)"/>
        public string? Data { get; set; }

        /// <summary>
        /// Records that the body came from curl's <c>--data-urlencode</c>; the parser has already percent-encoded the
        /// value into <see cref="Data"/>, so setting this yourself changes nothing on the wire.
        /// </summary>
        /// <value>True after parsing a command that used <c>--data-urlencode</c>. Default: false.</value>
        /// <remarks>
        /// <para>curl's forms are <c>content</c>, <c>=content</c>, <c>name=content</c>, <c>@file</c> and <c>name@file</c>;
        /// the content is encoded with <c>%XX</c> escapes and joined to other <c>-d</c> parts with <c>&amp;</c>.
        /// For hand-built options encode the value yourself, for example
        /// <c>Data = "q=" + Uri.EscapeDataString("C# &amp; .NET")</c>, which is what
        /// <c>curl --data-urlencode "q=C# &amp; .NET" URL</c> sends.</para>
        /// </remarks>
        /// <seealso cref="Data"/>
        /// <seealso cref="UseGet"/>
        public bool DataUrlEncode { get; set; }

        /// <summary>
        /// Sends <see cref="Data"/> literally even when it starts with <c>@</c>, instead of reading a file;
        /// curl's <c>--data-raw</c>, which is <c>-d</c> without the <c>@file</c> feature.
        /// </summary>
        /// <value>True to send a leading <c>@</c> as a character. Default: false.</value>
        /// <remarks>
        /// <para>Use it when the body really begins with <c>@</c>, such as a mention or a decorator name.
        /// Example: <c>curl --data-raw "@channel deploy done" URL</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", Data = "@channel deploy done", DataRaw = true }</c>; without
        /// <c>DataRaw</c> CurlDotNet would try to read a file called <c>channel deploy done</c>.</para>
        /// <para>curl manual: https://curl.se/docs/manpage.html#--data-raw</para>
        /// </remarks>
        /// <seealso cref="Data"/>
        public bool DataRaw { get; set; }

        /// <summary>
        /// Raw bytes sent as the request body without any text encoding, for images, archives and protobuf;
        /// also where the bytes of a <c>-T</c>/<c>--upload-file</c> file end up after parsing.
        /// </summary>
        /// <value>The exact body bytes. Default: null.</value>
        /// <remarks>
        /// <para>Used when <see cref="Data"/> is not the body source: with <see cref="UploadFile"/> it is the uploaded file,
        /// otherwise it is sent as <c>ByteArrayContent</c> on a POST/PUT/PATCH (set <see cref="Method"/>; GET and HEAD send
        /// no body). No Content-Type is added: set <c>Headers["Content-Type"]</c> (for example <c>application/octet-stream</c>).
        /// Multipart fields (<see cref="FormData"/>, <see cref="Files"/>, <see cref="FormParts"/>) take precedence over it.</para>
        /// <para>Example: <c>curl --data-binary @photo.jpg -H "Content-Type: image/jpeg" URL</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", BinaryData = File.ReadAllBytes("photo.jpg"), Headers = { ["Content-Type"] = "image/jpeg" } }</c>.</para>
        /// </remarks>
        /// <seealso cref="Data"/>
        /// <seealso cref="UploadFile"/>
        /// <seealso cref="CurlRequestBuilder.WithBinaryData(byte[])"/>
        public byte[]? BinaryData { get; set; }

        /// <summary>
        /// Text fields of a <c>multipart/form-data</c> body, field name to value, as curl's <c>-F name=value</c> sends
        /// them; filling it (or <see cref="Files"/>) turns the request into a multipart POST.
        /// </summary>
        /// <value>Field name to text value. Default: empty.</value>
        /// <remarks>
        /// <para>When <see cref="FormParts"/> is empty, each entry becomes one text part and each <see cref="Files"/> entry one
        /// file part (fields first), with curl's boundary and part headers. When the parser filled <see cref="FormParts"/>
        /// (from <c>-F</c>), those parts are sent and this dictionary is only a summary; a field read from a file
        /// (<c>-F name=&lt;file</c>) appears here as <c>"&lt;file"</c>. Set <see cref="Method"/> to POST for hand-built options.</para>
        /// <para>Example: <c>curl -F name=Ada -F avatar=@me.png URL</c> is
        /// <c>new CurlOptions { Url = url, Method = "POST", FormData = { ["name"] = "Ada" }, Files = { ["avatar"] = "me.png" } }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Files"/>
        /// <seealso cref="FormParts"/>
        /// <seealso cref="CurlRequestBuilder.WithMultipartForm(Dictionary{string, string}, Dictionary{string, string})"/>
        public Dictionary<string, string> FormData { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Files uploaded as parts of a <c>multipart/form-data</c> body, field name to local file path, as curl's
        /// <c>-F name=@path</c> sends them with <c>filename="basename"</c> and a guessed Content-Type.
        /// </summary>
        /// <value>Field name to file path (absolute, or relative to the current directory). Default: empty.</value>
        /// <remarks>
        /// <para>The Content-Type of each file part is guessed from its extension (<c>.png</c> is <c>image/png</c>,
        /// unknown is <c>application/octet-stream</c>). A file that cannot be read fails with
        /// <see cref="CurlDotNet.Exceptions.CurlReadErrorException"/> (exit 26) when the request is built. For custom part
        /// types or file names use <see cref="FormParts"/> (<c>-F "file=@a.bin;type=application/x-thing;filename=b.bin"</c>).</para>
        /// <para>Example: <c>curl -F "report=@./q3.pdf" https://example.com/upload</c> is
        /// <c>new CurlOptions { Url = "https://example.com/upload", Method = "POST", Files = { ["report"] = "./q3.pdf" } }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="FormData"/>
        /// <seealso cref="FormParts"/>
        /// <seealso cref="CurlRequestBuilder.WithFile(string, string)"/>
        /// <seealso cref="CurlRequestBuilder.WithUploadFile(string, string)"/>
        public Dictionary<string, string> Files { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// Writes the response body to this file instead of <see cref="CurlResult.Body"/>, like curl's <c>-o FILE</c>;
        /// <c>"-"</c> means standard output, which is <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <value>A file path, absolute or relative to <see cref="OutputDir"/> (or the current directory). Default: null.</value>
        /// <remarks>
        /// <para>The file is created or replaced; its path is added to <see cref="CurlResult.OutputFiles"/>. With
        /// <see cref="IncludeHeaders"/> the headers are written to the file too. The folder must exist unless
        /// <see cref="CreateDirs"/> is true; otherwise, or when the file cannot be written, the transfer fails with
        /// <see cref="CurlDotNet.Exceptions.CurlWriteErrorException"/> (exit 23). <see cref="NoClobber"/>,
        /// <see cref="SkipExisting"/>, <see cref="RemoveOnError"/> and <see cref="ResumeFrom"/> (<c>-C -</c>) change how
        /// an existing file is treated.</para>
        /// <para>For <c>file://</c> URLs the file handler is simpler: it always creates the output folder (no
        /// <see cref="CreateDirs"/> needed), returns status 404 for a missing source file instead of failing (so
        /// <see cref="FailOnError"/> does not apply), and ignores <see cref="NoClobber"/>, <see cref="OutputDir"/>,
        /// <see cref="DumpHeaderFile"/> and <see cref="WriteOut"/>.</para>
        /// <para>Example: <c>curl -o report.pdf https://example.com/report.pdf</c> is
        /// <c>new CurlOptions { Url = "https://example.com/report.pdf", OutputFile = "report.pdf" }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="UseRemoteFileName"/>
        /// <seealso cref="OutputDir"/>
        /// <seealso cref="CreateDirs"/>
        /// <seealso cref="CurlResult.OutputFiles"/>
        /// <seealso cref="CurlRequestBuilder.WithOutput(string)"/>
        public string? OutputFile { get; set; }

        /// <summary>
        /// Saves the body under the file name taken from the URL's last path segment (<c>/files/a.zip</c> becomes
        /// <c>a.zip</c>), like curl's <c>-O</c>/<c>--remote-name</c>.
        /// </summary>
        /// <value>True to name the output file after the URL. Default: false.</value>
        /// <remarks>
        /// <para>A URL without a file name in its path is saved as <c>curl_response</c>. The file goes into
        /// <see cref="OutputDir"/> when set, otherwise the current directory. With <see cref="RemoteHeaderName"/>
        /// (<c>-J</c>) the server's <c>Content-Disposition</c> file name is used instead. Ignored when
        /// <see cref="OutputFile"/> is set.</para>
        /// <para>Example: <c>curl -O https://example.com/downloads/tool-1.2.zip</c> is
        /// <c>new CurlOptions { Url = "https://example.com/downloads/tool-1.2.zip", UseRemoteFileName = true }</c>, which writes
        /// <c>tool-1.2.zip</c>.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="RemoteHeaderName"/>
        /// <seealso cref="OutputDir"/>
        /// <seealso cref="CurlRequestBuilder.SaveWithRemoteName"/>
        public bool UseRemoteFileName { get; set; }

        /// <summary>
        /// Puts the response status line and headers in front of the body in <see cref="CurlResult.Body"/> (or the
        /// <see cref="OutputFile"/>), exactly as curl's <c>-i</c>/<c>--include</c> prints them.
        /// </summary>
        /// <value>True to include the header block. Default: false (the headers are still in <see cref="CurlResult.Headers"/>).</value>
        /// <remarks>
        /// <para>Every header block of the transfer is included (redirect hops with <see cref="FollowLocation"/>, interim
        /// <c>100 Continue</c>), each ending with an empty line. To keep the body clean and write the headers elsewhere use
        /// <see cref="DumpHeaderFile"/> (<c>-D</c>).</para>
        /// <para>Example: <c>curl -i https://example.com/</c> is <c>new CurlOptions { Url = "https://example.com/", IncludeHeaders = true }</c>;
        /// <c>result.Body</c> then starts with <c>HTTP/1.1 200 OK</c>.</para>
        /// </remarks>
        /// <seealso cref="HeadOnly"/>
        /// <seealso cref="DumpHeaderFile"/>
        /// <seealso cref="CurlResult.Headers"/>
        /// <seealso cref="CurlRequestBuilder.IncludeHeaders(bool)"/>
        public bool IncludeHeaders { get; set; }

        /// <summary>
        /// Sends a HEAD request and returns only the response headers, no body, like curl's <c>-I</c>/<c>--head</c>;
        /// for parsed commands the headers also appear in <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <value>True for a headers-only request. Default: false.</value>
        /// <remarks>
        /// <para>The parser sets this together with <c>Method = "HEAD"</c>. On a parsed command (one with
        /// <see cref="OriginalCommand"/>) the header block is written to the output like curl does; on hand-built options
        /// the headers are only in <see cref="CurlResult.Headers"/>. Redirects followed with <see cref="FollowLocation"/>
        /// stay HEAD requests.</para>
        /// <para>Example: <c>curl -I https://example.com/big.iso</c> is
        /// <c>new CurlOptions { Url = "https://example.com/big.iso", HeadOnly = true, Method = "HEAD" }</c>; read
        /// <c>result.Headers["Content-Length"]</c>. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Method"/>
        /// <seealso cref="IncludeHeaders"/>
        /// <seealso cref="CurlRequestBuilder.Head(string)"/>
        public bool HeadOnly { get; set; }

        /// <summary>
        /// Follows <c>3xx</c> redirects (the <c>Location</c> header) until a final response, like curl's
        /// <c>-L</c>/<c>--location</c>; without it a redirect is returned as the result.
        /// </summary>
        /// <value>True to follow redirects. Default: false, unless <see cref="Curl.DefaultFollowRedirects"/> is true.</value>
        /// <remarks>
        /// <para>At most <see cref="MaxRedirects"/> hops are followed (50 by default), then the transfer fails with
        /// <see cref="CurlDotNet.Exceptions.CurlTooManyRedirectsException"/> (exit 47). A 301/302/303 after a POST continues
        /// as GET unless <see cref="Post301"/>/<see cref="Post302"/>/<see cref="Post303"/> are set. Credentials are only
        /// sent to the original host unless <see cref="LocationTrusted"/> is true. Only <c>http</c> and <c>https</c>
        /// targets are followed.</para>
        /// <para>Example: <c>curl -L https://bit.ly/abc</c> is <c>new CurlOptions { Url = "https://bit.ly/abc", FollowLocation = true }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-follow-redirects-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="MaxRedirects"/>
        /// <seealso cref="LocationTrusted"/>
        /// <seealso cref="FollowObeysMethod"/>
        /// <seealso cref="Curl.DefaultFollowRedirects"/>
        /// <seealso cref="CurlRequestBuilder.FollowRedirects(bool)"/>
        /// <seealso cref="CurlSettings.FollowRedirects"/>
        public bool FollowLocation { get; set; }

        /// <summary>
        /// Same setting as <see cref="FollowLocation"/> (curl's <c>-L</c>) under the name .NET developers usually
        /// look for; reading or writing either property changes both.
        /// </summary>
        /// <value>True to follow <c>3xx</c> redirects. Default: false.</value>
        /// <remarks>
        /// <para>Example: <c>new CurlOptions { Url = url, FollowRedirects = true }</c> is the same as <c>curl -L URL</c>.
        /// See <see cref="FollowLocation"/> for the redirect rules.</para>
        /// </remarks>
        /// <seealso cref="FollowLocation"/>
        /// <seealso cref="MaxRedirects"/>
        public bool FollowRedirects
        {
            get => FollowLocation;
            set => FollowLocation = value;
        }

        /// <summary>
        /// The largest number of redirects followed before the transfer fails with exit code 47, like curl's
        /// <c>--max-redirs N</c>; only used when <see cref="FollowLocation"/> is on.
        /// </summary>
        /// <value>A count of hops. Default: 50 (curl's default). <c>-1</c> means no limit; <c>0</c> refuses every redirect.</value>
        /// <remarks>
        /// <para>Exceeding it throws <see cref="CurlDotNet.Exceptions.CurlTooManyRedirectsException"/> with the number of hops
        /// followed; a redirect loop usually shows up this way. See
        /// https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/redirect-errors.md</para>
        /// <para>Example: <c>curl -L --max-redirs 5 URL</c> is <c>new CurlOptions { Url = url, FollowLocation = true, MaxRedirects = 5 }</c>.</para>
        /// </remarks>
        /// <seealso cref="FollowLocation"/>
        /// <seealso cref="CurlRequestBuilder.WithMaxRedirects(int)"/>
        public int MaxRedirects { get; set; } = 50;

        /// <summary>
        /// Skips TLS certificate verification so self-signed, expired or wrong-host certificates are accepted,
        /// like curl's <c>-k</c>/<c>--insecure</c>; use for local testing only.
        /// </summary>
        /// <value>True to accept any server certificate. Default: false, unless <see cref="Curl.DefaultInsecure"/> is true.</value>
        /// <remarks>
        /// <para>Without it an untrusted certificate fails with <see cref="CurlDotNet.Exceptions.CurlSslException"/>
        /// (exit 60). The traffic is still encrypted, but anyone in the middle can read it, so never enable it in
        /// production; trust the issuing CA in the operating system instead. The <c>--trace</c> output notes when
        /// verification is disabled.</para>
        /// <para>Example: <c>curl -k https://localhost:5001/health</c> is
        /// <c>new CurlOptions { Url = "https://localhost:5001/health", Insecure = true }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-ignore-ssl-certificate-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Curl.DefaultInsecure"/>
        /// <seealso cref="CaCertFile"/>
        /// <seealso cref="CurlRequestBuilder.Insecure(bool)"/>
        /// <seealso cref="CurlSettings.Insecure"/>
        public bool Insecure { get; set; }

        /// <summary>
        /// Records the request and response headers (curl's <c>&gt;</c>/<c>&lt;</c> lines) and connection notes,
        /// like curl's <c>-v</c>/<c>--verbose</c>, and puts that log at the start of <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <value>True to capture the verbose log. Default: false.</value>
        /// <remarks>
        /// <para>curl prints this to standard error; CurlDotNet has no terminal, so the log is placed in front of the
        /// output text, followed by the headers (<see cref="IncludeHeaders"/>) and body. For a complete wire log
        /// (bytes, TLS, timings) written to a file use <see cref="TraceFile"/> or <see cref="TraceAsciiFile"/>.</para>
        /// <para>Example: <c>curl -v https://example.com/</c> is <c>new CurlOptions { Url = "https://example.com/", Verbose = true }</c>.
        /// Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <seealso cref="TraceFile"/>
        /// <seealso cref="TraceAsciiFile"/>
        /// <seealso cref="CurlRequestBuilder.Verbose(bool)"/>
        public bool Verbose { get; set; }

        /// <summary>
        /// Writes a full diagnostic trace of the transfer (headers and bodies dumped as hex plus ASCII) to this file,
        /// like curl's <c>--trace FILE</c>; <c>"-"</c> writes the trace to standard output.
        /// </summary>
        /// <value>A file path, or <c>"-"</c>. Default: null (no trace).</value>
        /// <remarks>
        /// <para>The trace captures connection and proxy selection, TLS verification, the exact bytes sent and received,
        /// transfer timings, and the precise error when a request is blocked or fails. This is the recommended option for
        /// diagnosing why requests are rejected behind a proxy. When <see cref="TraceAsciiFile"/> is also set, the ASCII
        /// format wins. <see cref="TraceTime"/> adds timestamps.</para>
        /// <para>Example: <c>curl --trace trace.log https://example.com/</c> is
        /// <c>new CurlOptions { Url = "https://example.com/", TraceFile = "trace.log" }</c>. curl manual:
        /// https://curl.se/docs/manpage.html#--trace</para>
        /// </remarks>
        /// <seealso cref="TraceAsciiFile"/>
        /// <seealso cref="TraceTime"/>
        /// <seealso cref="Verbose"/>
        /// <seealso cref="CurlRequestBuilder.WithTrace(string, bool)"/>
        public string? TraceFile { get; set; }

        /// <summary>
        /// Writes a readable diagnostic trace of the transfer (payloads as text, no hex column) to this file,
        /// like curl's <c>--trace-ascii FILE</c>; <c>"-"</c> writes the trace to standard output.
        /// </summary>
        /// <value>A file path, or <c>"-"</c>. Default: null (no trace).</value>
        /// <remarks>
        /// <para>Identical to <see cref="TraceFile"/> except the payload dumps omit the hexadecimal column, producing a more
        /// compact log. When both options are supplied, the ASCII format wins.</para>
        /// <para>Example: <c>curl --trace-ascii - https://example.com/</c> is
        /// <c>new CurlOptions { Url = "https://example.com/", TraceAsciiFile = "-" }</c>. curl manual:
        /// https://curl.se/docs/manpage.html#--trace-ascii</para>
        /// </remarks>
        /// <seealso cref="TraceFile"/>
        /// <seealso cref="TraceTime"/>
        /// <seealso cref="CurlRequestBuilder.WithTraceAscii(string, bool)"/>
        public string? TraceAsciiFile { get; set; }

        /// <summary>
        /// Prefixes every line of the <see cref="TraceFile"/>/<see cref="TraceAsciiFile"/> output with a high-resolution
        /// timestamp, like curl's <c>--trace-time</c>, to show where a request stalls.
        /// </summary>
        /// <value>True to add timestamps. Default: false. Has no effect without a trace file.</value>
        /// <remarks>
        /// <para>Example: <c>curl --trace-ascii t.log --trace-time URL</c> is
        /// <c>new CurlOptions { Url = url, TraceAsciiFile = "t.log", TraceTime = true }</c>. curl manual:
        /// https://curl.se/docs/manpage.html#--trace-time</para>
        /// </remarks>
        /// <seealso cref="TraceFile"/>
        /// <seealso cref="TraceAsciiFile"/>
        public bool TraceTime { get; set; }

        /// <summary>
        /// Suppresses the warnings curl would print on standard error, like curl's <c>-s</c>/<c>--silent</c>; errors
        /// are still thrown as exceptions, so this never hides a failure.
        /// </summary>
        /// <value>True for silent mode. Default: false.</value>
        /// <remarks>
        /// <para>CurlDotNet draws no progress meter, so the only visible effect is that warnings (for example
        /// <c>--etag-save</c> folder missing, skipping the transfer) are not written to standard error.</para>
        /// <para>Example: <c>curl -s https://example.com/</c> is <c>new CurlOptions { Url = "https://example.com/", Silent = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="ShowError"/>
        /// <seealso cref="CurlRequestBuilder.Silent(bool)"/>
        public bool Silent { get; set; }

        /// <summary>
        /// curl's <c>-S</c>/<c>--show-error</c> (print errors even with <c>-s</c>); recorded for compatibility only,
        /// because CurlDotNet reports every error as an exception anyway.
        /// </summary>
        /// <value>True when <c>-S</c> was given. Default: false. Not acted on.</value>
        /// <remarks>
        /// <para>Example: <c>curl -sS URL</c> parses to <c>Silent = true, ShowError = true</c>; catch
        /// <see cref="CurlDotNet.Exceptions.CurlException"/> to see the error.</para>
        /// </remarks>
        /// <seealso cref="Silent"/>
        public bool ShowError { get; set; }

        /// <summary>
        /// Turns an HTTP status of 400 or above into a <see cref="CurlDotNet.Exceptions.CurlHttpReturnedErrorException"/>
        /// (exit 22) and discards the error body, like curl's <c>-f</c>/<c>--fail</c>.
        /// </summary>
        /// <value>True to fail on HTTP errors. Default: false, which returns 4xx/5xx responses as a normal
        /// <see cref="CurlResult"/> with <see cref="CurlResult.StatusCode"/> set.</value>
        /// <remarks>
        /// <para>The exception carries <c>StatusCode</c>, the URL and the response headers; no <see cref="OutputFile"/> body is
        /// written. To keep the body (for API error details) use <see cref="FailWithBody"/> instead. A <c>416</c> while
        /// resuming (<see cref="ResumeFrom"/>) is not treated as a failure.</para>
        /// <para>Example: <c>curl -f https://api.example.com/missing</c> is <c>new CurlOptions { Url = url, FailOnError = true }</c>;
        /// catch <c>CurlHttpReturnedErrorException ex</c> and read <c>ex.StatusCode</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="FailWithBody"/>
        /// <seealso cref="CurlResult.StatusCode"/>
        /// <seealso cref="CurlRequestBuilder.FailOnError(bool)"/>
        public bool FailOnError { get; set; }

        /// <summary>
        /// The <c>User-Agent</c> header sent with the request, like curl's <c>-A</c>/<c>--user-agent</c>; an empty
        /// string sends no User-Agent at all.
        /// </summary>
        /// <value>The header value, such as <c>"MyApp/2.1 (+https://example.com)"</c>. Default: null, which sends
        /// <see cref="Curl.DefaultUserAgent"/> (<c>curl/8.x</c>).</value>
        /// <remarks>
        /// <para>Takes precedence over a <c>User-Agent</c> entry in <see cref="Headers"/>; the parser also copies a
        /// <c>-H "User-Agent: ..."</c> value here. Some APIs (GitHub, for example) reject requests without one.</para>
        /// <para>Example: <c>curl -A "MyApp/2.1" URL</c> is <c>new CurlOptions { Url = url, UserAgent = "MyApp/2.1" }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Curl.DefaultUserAgent"/>
        /// <seealso cref="Headers"/>
        /// <seealso cref="CurlRequestBuilder.WithUserAgent(string)"/>
        public string? UserAgent { get; set; }

        /// <summary>
        /// The <c>Referer</c> header sent with the request, verbatim, like curl's <c>-e</c>/<c>--referer</c>; it need
        /// not be an absolute URL.
        /// </summary>
        /// <value>The header value, such as <c>"https://example.com/start"</c>. Default: null (no Referer).</value>
        /// <remarks>
        /// <para>The parser strips curl's <c>;auto</c> suffix and sets <see cref="AutoReferer"/> instead, so this property only
        /// holds the literal value. An explicit <c>Referer</c> in <see cref="Headers"/> wins.</para>
        /// <para>Example: <c>curl -e https://example.com/start URL</c> is <c>new CurlOptions { Url = url, Referer = "https://example.com/start" }</c>.</para>
        /// </remarks>
        /// <seealso cref="AutoReferer"/>
        /// <seealso cref="CurlRequestBuilder.WithReferer(string)"/>
        public string? Referer { get; set; }

        /// <summary>
        /// Cookies to send, like curl's <c>-b</c>/<c>--cookie</c>: a cookie string such as <c>name=value; other=2</c>,
        /// or (when there is no <c>=</c>) the path of a Netscape cookie file to read.
        /// </summary>
        /// <value>A <c>Cookie</c> header value, or a file path. Default: null.</value>
        /// <remarks>
        /// <para>A value containing <c>=</c> is sent as the <c>Cookie</c> header exactly as given. A value without <c>=</c>
        /// names a cookie file (Netscape format, as written by <see cref="CookieJar"/>, or a file of <c>Set-Cookie:</c> lines);
        /// its cookies are sent to matching hosts and paths, and a missing file is ignored like in curl. Several
        /// <c>-b name=value</c> options are joined with <c>"; "</c>. Combine with <see cref="CookieJar"/> to keep a session.</para>
        /// <para>Example: <c>curl -b "session=abc123; theme=dark" URL</c> is <c>new CurlOptions { Url = url, Cookie = "session=abc123; theme=dark" }</c>,
        /// and <c>curl -b cookies.txt -c cookies.txt URL</c> is <c>Cookie = "cookies.txt", CookieJar = "cookies.txt"</c>.
        /// Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <seealso cref="CookieJar"/>
        /// <seealso cref="JunkSessionCookies"/>
        /// <seealso cref="CurlRequestBuilder.WithCookie(string)"/>
        public string? Cookie { get; set; }

        /// <summary>
        /// File where every cookie known at the end of the transfer is saved in Netscape format, like curl's
        /// <c>-c</c>/<c>--cookie-jar FILE</c>; <c>"-"</c> appends the jar to <see cref="CurlResult.Body"/>.
        /// </summary>
        /// <value>A file path, or <c>"-"</c>. Default: null (cookies are not saved).</value>
        /// <remarks>
        /// <para>Setting a jar also turns on the cookie engine, so cookies set by one redirect hop are sent to the next.
        /// The file is written even when the server set no cookies. A folder that does not exist or a file that cannot be
        /// written fails with <see cref="CurlDotNet.Exceptions.CurlException"/> exit code 23. Read it back on the next
        /// request with <see cref="Cookie"/>.</para>
        /// <para>Example: <c>curl -c cookies.txt -d "user=ada&amp;pass=x" https://example.com/login</c> is
        /// <c>new CurlOptions { Url = "https://example.com/login", Method = "POST", Data = "user=ada&amp;pass=x", CookieJar = "cookies.txt" }</c>.
        /// Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/cookies.md</para>
        /// </remarks>
        /// <seealso cref="Cookie"/>
        /// <seealso cref="JunkSessionCookies"/>
        /// <seealso cref="CurlRequestBuilder.WithCookieJar(string)"/>
        public string? CookieJar { get; set; }

        /// <summary>
        /// User name and password for server authentication, like curl's <c>-u user:password</c>; sent as Basic
        /// authentication on the first request unless <see cref="AuthScheme"/> picks a challenge scheme.
        /// </summary>
        /// <value>A <see cref="NetworkCredential"/>. Default: null. The parser splits <c>-u user:password</c> at the first
        /// colon; <c>-u user</c> gives an empty password (curl would prompt for one).</value>
        /// <remarks>
        /// <para>With no <see cref="AuthScheme"/> (or <c>basic</c>) an <c>Authorization: Basic</c> header is sent preemptively.
        /// <c>digest</c>, <c>ntlm</c>, <c>negotiate</c> and <c>anyauth</c> answer the server's <c>401</c> challenge instead.
        /// Credentials are not sent to another host after a redirect unless <see cref="LocationTrusted"/> is true. An explicit
        /// <c>Authorization</c> header or <see cref="OAuth2Bearer"/> wins over them. With <see cref="AwsSigV4"/> they are the
        /// access key and secret key. User info in <see cref="Url"/> (<c>https://user:pass@host/</c>) has the same effect.</para>
        /// <para>Example: <c>curl -u ada:s3cret https://api.example.com/me</c> is
        /// <c>new CurlOptions { Url = "https://api.example.com/me", Credentials = new NetworkCredential("ada", "s3cret") }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-basic-auth-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="AuthScheme"/>
        /// <seealso cref="OAuth2Bearer"/>
        /// <seealso cref="ProxyCredentials"/>
        /// <seealso cref="CurlRequestBuilder.WithBasicAuth(string, string)"/>
        public NetworkCredential? Credentials { get; set; }

        /// <summary>
        /// Which HTTP authentication scheme <see cref="Credentials"/> use: <c>basic</c>, <c>digest</c>, <c>ntlm</c>,
        /// <c>negotiate</c> or <c>anyauth</c>, set by curl's <c>--basic</c>, <c>--digest</c>, <c>--ntlm</c>, <c>--negotiate</c>, <c>--anyauth</c>.
        /// </summary>
        /// <value>A lower-case scheme name. Default: null, which means curl's default: preemptive Basic when credentials are present.</value>
        /// <remarks>
        /// <para>Challenge-based schemes (digest, ntlm, negotiate, anyauth) send the first request without credentials and answer
        /// the server's <c>401 WWW-Authenticate</c> challenge, so the password never travels in Basic form. <c>anyauth</c> picks
        /// the most secure scheme the server offers. A challenge the scheme cannot process fails with
        /// <see cref="CurlDotNet.Exceptions.CurlAuthErrorException"/> (exit 94); a wrong password simply returns 401.</para>
        /// <para>Example: <c>curl --digest -u ada:s3cret URL</c> is
        /// <c>new CurlOptions { Url = url, Credentials = new NetworkCredential("ada", "s3cret"), AuthScheme = "digest" }</c>.
        /// Cookbook: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/cookbook/auth-schemes.md</para>
        /// </remarks>
        /// <seealso cref="Credentials"/>
        /// <seealso cref="ProxyAuthScheme"/>
        public string? AuthScheme { get; set; }

        /// <summary>
        /// Same setting as <see cref="Credentials"/> (curl's <c>-u user:password</c>) under an alternative name;
        /// reading or writing either property changes both.
        /// </summary>
        /// <value>A <see cref="NetworkCredential"/>, or null.</value>
        /// <remarks>
        /// <para>Example: <c>options.UserAuth = new NetworkCredential("ada", "s3cret");</c> is <c>curl -u ada:s3cret</c>.
        /// See <see cref="Credentials"/> for how the credentials are sent.</para>
        /// </remarks>
        /// <seealso cref="Credentials"/>
        public NetworkCredential UserAuth
        {
            get => Credentials;
            set => Credentials = value;
        }

        /// <summary>
        /// The proxy every request goes through, like curl's <c>-x</c>/<c>--proxy [scheme://][user:pass@]host[:port]</c>;
        /// null uses the <c>http_proxy</c>/<c>https_proxy</c>/<c>all_proxy</c> environment variables.
        /// </summary>
        /// <value>A proxy URL such as <c>http://proxy.corp:3128</c>, <c>socks5h://127.0.0.1:1080</c> or just
        /// <c>proxy.corp:3128</c> (HTTP, port 1080 when omitted, as in curl). Default: null.</value>
        /// <remarks>
        /// <para>Plain <c>http://</c> URLs are sent to the proxy with the absolute URL in the request line; <c>https://</c> URLs use a
        /// <c>CONNECT</c> tunnel (see <see cref="ProxyTunnel"/>). User info in the proxy URL, or <see cref="ProxyCredentials"/>,
        /// authenticates with the proxy (<see cref="ProxyAuthScheme"/>). Hosts in <see cref="NoProxy"/> bypass it. An empty
        /// string disables the environment proxy. An unreachable proxy fails with
        /// <see cref="CurlDotNet.Exceptions.CurlCouldntConnectException"/> (exit 7) naming the proxy; an unknown proxy host
        /// with <see cref="CurlDotNet.Exceptions.CurlCouldntResolveProxyException"/> (exit 5).</para>
        /// <para>Example: <c>curl -x http://proxy.corp:3128 https://api.example.com/</c> is
        /// <c>new CurlOptions { Url = "https://api.example.com/", Proxy = "http://proxy.corp:3128" }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="ProxyCredentials"/>
        /// <seealso cref="NoProxy"/>
        /// <seealso cref="ProxyTunnel"/>
        /// <seealso cref="Socks5Proxy"/>
        /// <seealso cref="CurlRequestBuilder.WithProxy(string)"/>
        /// <seealso cref="CurlRequestBuilder.WithProxy(string, string, string)"/>
        public string Proxy { get; set; }

        /// <summary>
        /// User name and password for the proxy, like curl's <c>-U</c>/<c>--proxy-user user:password</c>; sent as
        /// <c>Proxy-Authorization: Basic</c> unless <see cref="ProxyAuthScheme"/> picks another scheme.
        /// </summary>
        /// <value>A <see cref="NetworkCredential"/>. Default: null; <c>user:pass@</c> in <see cref="Proxy"/> has the same effect.</value>
        /// <remarks>
        /// <para>Example: <c>curl -x proxy.corp:3128 -U ada:s3cret URL</c> is
        /// <c>new CurlOptions { Url = url, Proxy = "proxy.corp:3128", ProxyCredentials = new NetworkCredential("ada", "s3cret") }</c>.
        /// A <c>407</c> answer means the proxy rejected them. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="ProxyAuthScheme"/>
        /// <seealso cref="Credentials"/>
        /// <seealso cref="CurlRequestBuilder.WithProxy(string, string, string)"/>
        public NetworkCredential? ProxyCredentials { get; set; }

        /// <summary>
        /// The longest the whole transfer may take, in seconds (decimals allowed), like curl's <c>-m</c>/<c>--max-time</c>;
        /// when it expires the transfer fails with exit code 28.
        /// </summary>
        /// <value>Seconds, for example <c>30</c> or <c>2.5</c>. Default: null, which uses <see cref="Curl.DefaultMaxTimeSeconds"/>
        /// (0 = no limit, as in curl). <c>0</c> or a negative value also means no limit.</value>
        /// <remarks>
        /// <para>The limit covers DNS, connecting, TLS, sending, waiting and downloading the body. Expiry throws
        /// <see cref="CurlDotNet.Exceptions.CurlOperationTimeoutException"/> with the limit and the bytes received; your own
        /// <c>CancellationToken</c> cancels independently of it. For the connection phase alone use <see cref="ConnectTimeout"/>.</para>
        /// <para>Example: <c>curl -m 30 https://slow.example.com/report</c> is
        /// <c>new CurlOptions { Url = "https://slow.example.com/report", MaxTime = 30 }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="ConnectTimeout"/>
        /// <seealso cref="Curl.DefaultMaxTimeSeconds"/>
        /// <seealso cref="CurlRequestBuilder.WithTimeout(TimeSpan)"/>
        /// <seealso cref="CurlSettings.MaxTimeSeconds"/>
        public double? MaxTime { get; set; }

        /// <summary>
        /// The longest the connection phase (DNS lookup, TCP connect and TLS handshake) may take, in seconds with
        /// decimals allowed, like curl's <c>--connect-timeout</c>; expiry fails with exit code 28.
        /// </summary>
        /// <value>Seconds, for example <c>5</c> or <c>0.5</c>. Default: null, which uses
        /// <see cref="Curl.DefaultConnectTimeoutSeconds"/> when that is above 0, otherwise no separate connect limit.</value>
        /// <remarks>
        /// <para>Once connected, only <see cref="MaxTime"/> limits the transfer. A short connect timeout with a longer
        /// <see cref="MaxTime"/> fails fast on a dead host while still allowing large downloads.</para>
        /// <para>Example: <c>curl --connect-timeout 5 -m 300 https://example.com/big.iso</c> is
        /// <c>new CurlOptions { Url = url, ConnectTimeout = 5, MaxTime = 300 }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="MaxTime"/>
        /// <seealso cref="Curl.DefaultConnectTimeoutSeconds"/>
        /// <seealso cref="CurlRequestBuilder.WithConnectTimeout(TimeSpan)"/>
        /// <seealso cref="CurlSettings.ConnectTimeoutSeconds"/>
        public double? ConnectTimeout { get; set; }

        /// <summary>
        /// Aborts a transfer that stays slower than this many bytes per second for <see cref="SpeedTime"/> seconds, like
        /// curl's <c>-Y</c>/<c>--speed-limit</c>; the transfer then fails with exit code 28.
        /// </summary>
        /// <value>Bytes per second; 0 (the default) turns the check off. When <see cref="SpeedTime"/> is 0, curl's default
        /// of 30 seconds is used.</value>
        /// <remarks>
        /// <para>The rate is measured while the response body downloads (http and https), over a sliding window of
        /// <see cref="SpeedTime"/> seconds. When it stays below the limit for the whole window the transfer is aborted with
        /// <see cref="CurlDotNet.Exceptions.CurlOperationTimeoutException"/> (exit 28) and curl's message
        /// <c>Operation too slow. Less than N bytes/sec transferred the last S seconds</c>. Use it to give up on stalled
        /// downloads without capping a healthy one with <see cref="MaxTime"/>. A non-numeric value on the command line fails
        /// with exit code 2.</para>
        /// <para>Example: <c>curl --speed-limit 1000 --speed-time 30 URL</c> is
        /// <c>new CurlOptions { Url = url, SpeedLimit = 1000, SpeedTime = 30 }</c>.</para>
        /// </remarks>
        /// <seealso cref="SpeedTime"/>
        /// <seealso cref="LimitRate"/>
        /// <seealso cref="MaxTime"/>
        public long SpeedLimit { get; set; }

        /// <summary>
        /// How long, in seconds, a transfer may stay below <see cref="SpeedLimit"/> before it is aborted, like curl's
        /// <c>-y</c>/<c>--speed-time</c>.
        /// </summary>
        /// <value>Whole seconds. Default: 0, which means curl's 30 seconds when <see cref="SpeedLimit"/> is set (and nothing
        /// otherwise).</value>
        /// <remarks>
        /// <para>curl also applies <c>--speed-time</c> alone with a limit of 1 byte per second; CurlDotNet does the same, so
        /// <c>--speed-time 20</c> aborts a transfer that receives nothing for 20 seconds (exit 28).</para>
        /// <para>Example: <c>curl -Y 500 -y 10 URL</c> is <c>new CurlOptions { Url = url, SpeedLimit = 500, SpeedTime = 10 }</c>.</para>
        /// </remarks>
        /// <seealso cref="SpeedLimit"/>
        /// <seealso cref="MaxTime"/>
        public int SpeedTime { get; set; }

        /// <summary>
        /// Caps the download speed at this many bytes per second, like curl's <c>--limit-rate</c>; the body is read more
        /// slowly so the average rate stays at or below the cap.
        /// </summary>
        /// <value>Bytes per second; the parser accepts suffixes <c>k</c>, <c>M</c>, <c>G</c>, <c>T</c>, <c>P</c> (powers of
        /// 1024), so <c>100k</c> is 102400. Default: 0 (no cap).</value>
        /// <remarks>
        /// <para>Applies to http and https response bodies: after each chunk the reader waits until the average rate since
        /// the body started is back under the cap (curl averages the same way, so short bursts are possible). Uploads are
        /// not throttled. A malformed value on the command line fails with exit code 2.</para>
        /// <para>Example: <c>curl --limit-rate 100k -O https://example.com/big.iso</c> is
        /// <c>new CurlOptions { Url = url, UseRemoteFileName = true, LimitRate = 100 * 1024 }</c>.</para>
        /// </remarks>
        /// <seealso cref="SpeedLimit"/>
        /// <seealso cref="MaxTime"/>
        public long LimitRate { get; set; }

        /// <summary>
        /// Resumes a download at a byte offset by sending <c>Range: bytes=N-</c>, like curl's <c>-C</c>/<c>--continue-at N</c>;
        /// <c>-1</c> (curl's <c>-C -</c>) continues from the size of the existing <see cref="OutputFile"/>.
        /// </summary>
        /// <value>A byte offset, <c>-1</c> for "from the existing file", or null. Default: null (no resume). <c>0</c> sends no Range.</value>
        /// <remarks>
        /// <para>A resumed download is appended to <see cref="OutputFile"/>. When the server ignores the range and sends the whole
        /// document the transfer fails with <see cref="CurlDotNet.Exceptions.CurlRangeErrorException"/> (exit 33) rather than
        /// corrupt the file; a <c>416</c> means the file is already complete and is not an error. Resuming an upload
        /// (with <see cref="Data"/>, <see cref="FormData"/> or <see cref="UploadFile"/>) is refused with exit code 43.
        /// The parser also sets <see cref="Range"/> to <c>N-</c>.</para>
        /// <para>Example: <c>curl -C - -o big.iso https://example.com/big.iso</c> is
        /// <c>new CurlOptions { Url = url, OutputFile = "big.iso", ResumeFrom = -1 }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Range"/>
        /// <seealso cref="OutputFile"/>
        public long? ResumeFrom { get; set; }

        /// <summary>
        /// Asks for part of the document with <c>Range: bytes=RANGE</c>, like curl's <c>-r</c>/<c>--range</c>; useful
        /// for reading the start of a large file or downloading it in pieces.
        /// </summary>
        /// <value>A byte range without the <c>bytes=</c> prefix: <c>0-499</c>, <c>500-</c>, <c>-500</c> (last 500 bytes) or
        /// <c>0-99,200-299</c>. Default: null.</value>
        /// <remarks>
        /// <para>A server that supports ranges answers <c>206 Partial Content</c>; one that does not sends the whole document
        /// with 200. With <see cref="UploadFile"/> (<c>-T</c>) the range is sent as <c>Content-Range</c> instead.</para>
        /// <para>Example: <c>curl -r 0-1023 https://example.com/big.bin</c> is <c>new CurlOptions { Url = url, Range = "0-1023" }</c>.</para>
        /// </remarks>
        /// <seealso cref="ResumeFrom"/>
        /// <seealso cref="CurlRequestBuilder.WithRange(string)"/>
        public string? Range { get; set; }

        /// <summary>
        /// Asks for a compressed response (<c>Accept-Encoding: gzip, deflate, br</c>) and decompresses the body, like curl's
        /// <c>--compressed</c>; the headers keep the server's <c>Content-Encoding</c>.
        /// </summary>
        /// <value>True to request and decode compression. Default: false (the body is returned exactly as sent).</value>
        /// <remarks>
        /// <para>Brotli (<c>br</c>) is offered on .NET 8/9/10; on .NET Standard 2.0 and .NET Framework only gzip and deflate.
        /// A body that cannot be decoded fails with <see cref="CurlDotNet.Exceptions.CurlBadContentEncodingException"/> (exit 61).
        /// For <c>Transfer-Encoding: gzip</c> see <see cref="TransferEncoding"/>.</para>
        /// <para>Example: <c>curl --compressed https://api.example.com/large.json</c> is
        /// <c>new CurlOptions { Url = url, Compressed = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="TransferEncoding"/>
        /// <seealso cref="CurlRequestBuilder.Compressed(bool)"/>
        public bool Compressed { get; set; }

        /// <summary>
        /// Client certificate for mutual TLS, from curl's <c>-E</c>/<c>--cert FILE[:PASSWORD]</c>: the certificate (and,
        /// for PEM, usually its private key) presented to servers that ask for one.
        /// </summary>
        /// <value>A file path. PEM (default), DER or PKCS#12 as chosen by <see cref="CertType"/> (a <c>.p12</c>/<c>.pfx</c>
        /// file is read as PKCS#12 without it). Default: null (no client certificate).</value>
        /// <remarks>
        /// <para>Applied on .NET 5 and later. PEM: the key is read from <see cref="KeyFile"/>, or from the same file when
        /// <see cref="KeyFile"/> is not set; an encrypted key needs <see cref="KeyPassword"/> (<c>--pass</c>) or
        /// <see cref="CertPassword"/>. DER: needs <see cref="KeyFile"/> (a PEM RSA or ECDSA key). P12: the password is
        /// <see cref="CertPassword"/> (the part after the colon in <c>--cert file.p12:secret</c>). A file that cannot be
        /// read, a wrong password or a key that does not match fails before connecting with
        /// <see cref="CurlDotNet.Exceptions.CurlSslCertificateProblemException"/> (exit 58); a server that rejects the
        /// certificate fails the handshake with <see cref="CurlDotNet.Exceptions.CurlSslConnectErrorException"/> (exit 35).
        /// On .NET Standard 2.0 and .NET Framework the option is recorded in <see cref="UnsupportedOptions"/> and no
        /// certificate is sent. It is not applied when you pass your own <c>HttpClient</c> to the engine (its handler decides).</para>
        /// <para>Example: <c>curl --cert client.pem --key client.key https://mtls.example.com/</c> is
        /// <c>new CurlOptions { Url = url, CertFile = "client.pem", KeyFile = "client.key" }</c>; with PKCS#12,
        /// <c>curl --cert client.p12:secret --cert-type P12 URL</c>.</para>
        /// </remarks>
        /// <seealso cref="KeyFile"/>
        /// <seealso cref="CertType"/>
        /// <seealso cref="CertPassword"/>
        /// <seealso cref="CaCertFile"/>
        public string? CertFile { get; set; }

        /// <summary>
        /// Private key file for <see cref="CertFile"/>, from curl's <c>--key FILE</c>; needed when the certificate file does
        /// not contain the key.
        /// </summary>
        /// <value>A PEM key file path (<c>-----BEGIN PRIVATE KEY-----</c>, <c>RSA PRIVATE KEY</c>, <c>EC PRIVATE KEY</c> or
        /// <c>ENCRYPTED PRIVATE KEY</c>). Default: null (the key is in <see cref="CertFile"/>).</value>
        /// <remarks>
        /// <para>Applied on .NET 5 and later, together with <see cref="CertFile"/>. An encrypted key needs
        /// <see cref="KeyPassword"/> (<c>--pass</c>).</para>
        /// <para>Example: <c>curl --cert client.pem --key client.key URL</c> parses to <c>KeyFile = "client.key"</c>.</para>
        /// </remarks>
        /// <seealso cref="CertFile"/>
        /// <seealso cref="KeyPassword"/>
        public string? KeyFile { get; set; }

        /// <summary>
        /// The format of <see cref="CertFile"/>, from curl's <c>--cert-type</c>: <c>PEM</c>, <c>DER</c> or <c>P12</c>.
        /// </summary>
        /// <value><c>PEM</c>, <c>DER</c>, <c>P12</c> (also <c>PKCS12</c>, <c>PFX</c>), case-insensitive. Default: null, meaning
        /// PEM, or P12 for a file ending in <c>.p12</c> or <c>.pfx</c>.</value>
        /// <remarks>
        /// <para>Another value fails with <see cref="CurlDotNet.Exceptions.CurlSslCertificateProblemException"/> (exit 58)
        /// when the transfer starts. Example: <c>curl --cert id.p12:pw --cert-type P12 URL</c> parses to <c>CertType = "P12"</c>.</para>
        /// </remarks>
        /// <seealso cref="CertFile"/>
        public string? CertType { get; set; }

        /// <summary>
        /// The format of <see cref="KeyFile"/>, from curl's <c>--key-type</c>; CurlDotNet reads PEM keys.
        /// </summary>
        /// <value><c>PEM</c> (the default) or another curl value. Default: null.</value>
        /// <remarks>
        /// <para>Only PEM keys are read: <c>--key-type DER</c> or <c>ENG</c> is recorded in <see cref="UnsupportedOptions"/>.
        /// Example: <c>curl --key k.pem --key-type PEM URL</c> parses to <c>KeyType = "PEM"</c>.</para>
        /// </remarks>
        /// <seealso cref="KeyFile"/>
        public string? KeyType { get; set; }

        /// <summary>
        /// The password of <see cref="CertFile"/>, from the part after the colon in curl's <c>--cert FILE:PASSWORD</c>; used
        /// for PKCS#12 files and encrypted PEM keys.
        /// </summary>
        /// <value>The password, or null. Default: null.</value>
        /// <remarks>
        /// <para>Like curl, a colon inside the file name can be escaped as <c>\:</c>, and a Windows drive letter
        /// (<c>C:\certs\id.p12</c>) is not mistaken for the separator. The value is a secret: CurlDotNet never prints it in
        /// messages or logs. Example: <c>curl --cert id.p12:s3cret --cert-type P12 URL</c> parses to
        /// <c>CertFile = "id.p12", CertPassword = "s3cret"</c>.</para>
        /// </remarks>
        /// <seealso cref="CertFile"/>
        /// <seealso cref="KeyPassword"/>
        public string? CertPassword { get; set; }

        /// <summary>
        /// The passphrase of an encrypted private key, from curl's <c>--pass PHRASE</c>.
        /// </summary>
        /// <value>The passphrase, or null. Default: null.</value>
        /// <remarks>
        /// <para>Used for <see cref="KeyFile"/> (or a PEM <see cref="CertFile"/> holding an encrypted key), and as the
        /// PKCS#12 password when <see cref="CertPassword"/> is not given. Never printed in messages or logs.
        /// Example: <c>curl --cert c.pem --key k.pem --pass phrase URL</c> parses to <c>KeyPassword = "phrase"</c>.</para>
        /// </remarks>
        /// <seealso cref="KeyFile"/>
        /// <seealso cref="CertPassword"/>
        public string? KeyPassword { get; set; }

        /// <summary>
        /// A PEM bundle of CA certificates to trust instead of the operating system's store, from curl's <c>--cacert FILE</c>;
        /// the way to call servers whose certificate comes from a private or corporate CA.
        /// </summary>
        /// <value>A path to a PEM file with one or more <c>-----BEGIN CERTIFICATE-----</c> blocks (a single DER certificate
        /// also works). Default: null (the system store decides).</value>
        /// <remarks>
        /// <para>Applied on .NET 5 and later: like curl, only the CAs in the bundle (plus <see cref="CaPath"/>) are trusted,
        /// the server's intermediates are used to build the chain, the host name must still match, and revocation is not
        /// checked. An untrusted server fails with <see cref="CurlDotNet.Exceptions.CurlPeerFailedVerificationException"/>
        /// (exit 60); a missing or empty bundle fails before connecting with
        /// <see cref="CurlDotNet.Exceptions.CurlSslCaCertBadFileException"/> (exit 77). On .NET Standard 2.0 and .NET
        /// Framework the option is recorded in <see cref="UnsupportedOptions"/> and the system store is used: install the CA
        /// there instead. <see cref="Insecure"/> skips verification altogether (testing only).</para>
        /// <para>Example: <c>curl --cacert corp-ca.pem https://intranet.example/</c> is
        /// <c>new CurlOptions { Url = url, CaCertFile = "corp-ca.pem" }</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/ssl-errors.md</para>
        /// </remarks>
        /// <seealso cref="CaPath"/>
        /// <seealso cref="PinnedPublicKey"/>
        /// <seealso cref="Insecure"/>
        /// <seealso cref="CaCert"/>
        public string? CaCertFile { get; set; }

        /// <summary>
        /// Same setting as <see cref="CaCertFile"/> (curl's <c>--cacert</c>) under a shorter name; reading or writing either
        /// property changes both.
        /// </summary>
        /// <value>A PEM file path, or null.</value>
        /// <remarks>
        /// <para>Example: <c>options.CaCert = "corp-ca.pem";</c> equals <c>options.CaCertFile = "corp-ca.pem";</c>.</para>
        /// </remarks>
        /// <seealso cref="CaCertFile"/>
        public string CaCert
        {
            get => CaCertFile;
            set => CaCertFile = value;
        }

        /// <summary>
        /// A folder of CA certificates to trust instead of the operating system's store, from curl's <c>--capath DIR</c>.
        /// </summary>
        /// <value>A folder path. Every file in it that holds PEM or DER certificates is loaded (OpenSSL's hashed names such as
        /// <c>5ed36f99.0</c> included); other files are skipped. Default: null.</value>
        /// <remarks>
        /// <para>Combined with <see cref="CaCertFile"/> into one trust store, applied on .NET 5 and later exactly like
        /// <see cref="CaCertFile"/>; a folder that does not exist or holds no certificate fails with
        /// <see cref="CurlDotNet.Exceptions.CurlSslCaCertBadFileException"/> (exit 77). Recorded in
        /// <see cref="UnsupportedOptions"/> on .NET Standard 2.0 and .NET Framework.</para>
        /// <para>Example: <c>curl --capath /etc/ssl/private-cas https://intranet.example/</c>.</para>
        /// </remarks>
        /// <seealso cref="CaCertFile"/>
        public string? CaPath { get; set; }

        /// <summary>
        /// Public key pins the server must match, from curl's <c>--pinnedpubkey</c>: <c>sha256//BASE64</c> hashes of the
        /// server's SubjectPublicKeyInfo (several separated by <c>;</c>), or the path of a PEM/DER public key file.
        /// </summary>
        /// <value>The pin text or file path. Default: null (no pinning).</value>
        /// <remarks>
        /// <para>Applied on .NET 5 and later, after certificate verification and also with <see cref="Insecure"/>, like curl.
        /// A server key that matches no pin, or a pin that cannot be read, fails with
        /// <see cref="CurlDotNet.Exceptions.CurlSslPinnedPublicKeyMismatchException"/> (exit 90); the message shows the pin of
        /// the key that was presented. Where pinning cannot be applied - on .NET Standard 2.0 and .NET Framework (where the
        /// parser also records the option in <see cref="UnsupportedOptions"/>), or when you passed your own <c>HttpClient</c> to
        /// the engine - an https transfer fails with <see cref="CurlDotNet.Exceptions.CurlNotBuiltInException"/> (exit 4) instead
        /// of silently skipping the check, like curl built without pinning support.</para>
        /// <para>Example: <c>curl --pinnedpubkey "sha256//YhKJKSzoTt2b5FP18fvpHo7fJYqQCjAa3HWY3tvRMwE=" https://api.example.com/</c>.</para>
        /// </remarks>
        /// <seealso cref="CaCertFile"/>
        public string? PinnedPublicKey { get; set; }

        /// <summary>
        /// The lowest TLS version to use, from curl's <c>-1</c>/<c>--tlsv1</c>, <c>--tlsv1.0</c>, <c>--tlsv1.1</c>,
        /// <c>--tlsv1.2</c> and <c>--tlsv1.3</c>.
        /// </summary>
        /// <value><c>"1.0"</c>, <c>"1.1"</c>, <c>"1.2"</c> or <c>"1.3"</c>. Default: null (the operating system's default,
        /// TLS 1.2 or later on current systems).</value>
        /// <remarks>
        /// <para>With <c>"1.2"</c> or <c>"1.3"</c> only that version and newer ones are offered (up to <see cref="TlsMaxVersion"/>);
        /// <c>"1.0"</c> and <c>"1.1"</c> leave the system default in place, which already excludes the old versions on most
        /// systems. A server that cannot meet the range fails the handshake with
        /// <see cref="CurlDotNet.Exceptions.CurlSslConnectErrorException"/> (exit 35). Whether TLS 1.3 is available depends on
        /// the operating system.</para>
        /// <para>Example: <c>curl --tlsv1.3 https://example.com/</c> is <c>new CurlOptions { Url = url, TlsMinVersion = "1.3" }</c>.</para>
        /// </remarks>
        /// <seealso cref="TlsMaxVersion"/>
        public string? TlsMinVersion { get; set; }

        /// <summary>
        /// The highest TLS version to use, from curl's <c>--tls-max VERSION</c>.
        /// </summary>
        /// <value><c>"1.0"</c>, <c>"1.1"</c>, <c>"1.2"</c> or <c>"1.3"</c>; <c>--tls-max default</c> leaves it null.
        /// Default: null (no maximum).</value>
        /// <remarks>
        /// <para>Without <see cref="TlsMinVersion"/> the minimum is TLS 1.2, like curl. A maximum below the minimum fails with
        /// <see cref="CurlDotNet.Exceptions.CurlSslConnectErrorException"/> (exit 35); another value fails parsing with exit
        /// code 2. Example: <c>curl --tls-max 1.2 https://example.com/</c> makes the handshake use TLS 1.2.</para>
        /// </remarks>
        /// <seealso cref="TlsMinVersion"/>
        public string? TlsMaxVersion { get; set; }

        /// <summary>
        /// The TLS cipher suites to offer, from curl's <c>--ciphers</c> and <c>--tls13-ciphers</c>, as IANA names
        /// separated by <c>:</c>.
        /// </summary>
        /// <value>For example <c>TLS_AES_256_GCM_SHA384:TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256</c>. Default: null (the system's list).</value>
        /// <remarks>
        /// <para>Like curl, the two versions are kept apart: a list that names only TLS 1.2 suites (<c>..._WITH_...</c>) keeps
        /// the usual TLS 1.3 suites, and a list of only TLS 1.3 suites keeps the usual ECDHE AES-GCM/ChaCha20 TLS 1.2 suites, so a
        /// server that speaks only the other version still connects.</para>
        /// <para>.NET can only restrict cipher suites where <c>CipherSuitesPolicy</c> is supported (Linux with OpenSSL, .NET 5
        /// and later). Elsewhere, or when a name is not an IANA suite name (OpenSSL-style names such as
        /// <c>ECDHE-RSA-AES128-GCM-SHA256</c> are not translated), the parser leaves this null and records the option in
        /// <see cref="UnsupportedOptions"/>. Setting it by hand on such a platform has no effect.</para>
        /// <para>Example: <c>curl --tls13-ciphers TLS_AES_256_GCM_SHA384 https://example.com/</c>.</para>
        /// </remarks>
        /// <seealso cref="TlsMinVersion"/>
        public string? Ciphers { get; set; }

        /// <summary>
        /// Network interface or source address from curl's <c>--interface</c>; not applied (connections use the operating
        /// system's default route), so the parser also records the option in <see cref="UnsupportedOptions"/>.
        /// </summary>
        /// <value>An interface name (<c>eth0</c>) or address. Default: null. Not acted on.</value>
        /// <remarks>
        /// <para>Binding outgoing connections to an interface is not available through HttpClient's connection layer.
        /// Example: <c>curl --interface eth1 URL</c> parses to <c>Interface = "eth1"</c> and
        /// <c>UnsupportedOptions</c> containing <c>--interface</c>.</para>
        /// </remarks>
        /// <seealso cref="Resolve"/>
        /// <seealso cref="UnsupportedOptions"/>
        public string? Interface { get; set; }

        /// <summary>
        /// The HTTP version to request: <c>"1.0"</c>, <c>"1.1"</c>, <c>"2.0"</c> or <c>"3.0"</c>, set by curl's
        /// <c>--http1.0</c>, <c>--http1.1</c>, <c>--http2</c> and <c>--http3</c>.
        /// </summary>
        /// <value>One of the strings above (<c>"1"</c>, <c>"2"</c> and <c>"3"</c> are accepted too). Default: null, which
        /// uses HttpClient's default (HTTP/1.1, upgraded where the platform negotiates it).</value>
        /// <remarks>
        /// <para>The connection may fall back to an older version unless <see cref="HttpVersionExact"/> is true
        /// (<c>--http2-prior-knowledge</c>, <c>--http3-only</c>). HTTP/2 and HTTP/3 need .NET 5+ (HTTP/3 also needs OS
        /// QUIC support); .NET Standard 2.0 and .NET Framework speak HTTP/1.x only. An unknown value is ignored.</para>
        /// <para>Example: <c>curl --http2 https://example.com/</c> is <c>new CurlOptions { Url = url, HttpVersion = "2.0" }</c>.</para>
        /// </remarks>
        /// <seealso cref="HttpVersionExact"/>
        /// <seealso cref="CurlRequestBuilder.WithHttpVersion(string)"/>
        public string? HttpVersion { get; set; }

        /// <summary>
        /// The method sent on the request line exactly as written (any token, such as <c>PROPFIND</c> or lower-case
        /// <c>purge</c>), like curl's <c>-X</c>/<c>--request</c>; overrides <see cref="Method"/>.
        /// </summary>
        /// <value>A method token. Default: null (use <see cref="Method"/>).</value>
        /// <remarks>
        /// <para>Like curl, <c>-X</c> only changes the word on the request line: it does not add a body or remove one.
        /// With <see cref="FollowLocation"/>, the custom method is kept on every redirect hop unless
        /// <see cref="FollowObeysMethod"/> is true. Prefer <see cref="Method"/> for the standard verbs; <c>-X HEAD</c> is
        /// better written as <see cref="HeadOnly"/>.</para>
        /// <para>Example: <c>curl -X PROPFIND https://dav.example.com/files/</c> is
        /// <c>new CurlOptions { Url = url, CustomMethod = "PROPFIND" }</c>.</para>
        /// </remarks>
        /// <seealso cref="Method"/>
        /// <seealso cref="FollowObeysMethod"/>
        public string? CustomMethod { get; set; }

        /// <summary>
        /// A curl <c>-w</c>/<c>--write-out</c> format whose expansion is appended to <see cref="CurlResult.Body"/> after the
        /// transfer, with variables such as <c>%{http_code}</c>, <c>%{time_total}</c> and <c>%{url_effective}</c>.
        /// </summary>
        /// <value>A format string; <c>\n</c> and <c>\t</c> are escapes, <c>%{json}</c> prints every variable as JSON,
        /// <c>%{stderr}</c> switches the following text to <see cref="StderrFile"/>. Default: null.</value>
        /// <remarks>
        /// <para>The text is expanded for failed transfers too (with <c>%{exitcode}</c> and <c>%{errormsg}</c>); it then travels on
        /// the exception under <see cref="CurlDotNet.Exceptions.CurlException.WriteOutDataKey"/>. In C# the same values are
        /// usually easier to read from <see cref="CurlResult.StatusCode"/>, <see cref="CurlResult.Timings"/> and
        /// <see cref="CurlResult.Headers"/>.</para>
        /// <para>Example: <c>curl -s -o /dev/null -w "%{http_code} %{time_total}\n" URL</c> is
        /// <c>new CurlOptions { Url = url, Silent = true, OutputFile = "/dev/null", WriteOut = "%{http_code} %{time_total}\\n" }</c>.</para>
        /// </remarks>
        /// <seealso cref="CurlResult.Timings"/>
        /// <seealso cref="StderrFile"/>
        /// <seealso cref="CurlDotNet.Exceptions.CurlException.WriteOutDataKey"/>
        public string? WriteOut { get; set; }

        /// <summary>
        /// Called for every chunk of the response body as it downloads with (percent, bytes received, total bytes),
        /// so you can drive a progress bar; curl's progress callback.
        /// </summary>
        /// <value>A callback, or null (default) for no progress reports.</value>
        /// <remarks>
        /// <para>Arguments: <c>percent</c> is 0 to 100, or <c>-1</c> when the server sent no <c>Content-Length</c> (chunked
        /// responses); <c>bytesReceived</c> is the running total of body bytes; <c>totalBytes</c> is the announced length, or
        /// <c>-1</c> when unknown. It is invoked on the thread reading the response, once per chunk (up to 80 KB), for
        /// http and https bodies only (not HEAD, FTP or file://). Keep it fast and marshal to your UI thread yourself.</para>
        /// <para>A handler that throws aborts the transfer with <see cref="CurlDotNet.Exceptions.CurlAbortedByCallbackException"/>
        /// (exit 42, like a curl progress callback returning non-zero); your exception is its <c>InnerException</c>.</para>
        /// <para>Example: <c>options.ProgressHandler = (pct, got, total) =&gt; Console.Write($"\r{(pct &lt; 0 ? got + " bytes" : pct.ToString("F0") + "%")}");</c>.
        /// With a command string use <c>new CurlSettings().WithProgress(...)</c>.
        /// Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/10-files-and-downloads.md</para>
        /// </remarks>
        /// <seealso cref="CurlSettings.WithProgress(Action{double, long, long})"/>
        /// <seealso cref="CurlSettings.OnProgress"/>
        /// <seealso cref="CurlDotNet.Exceptions.CurlAbortedByCallbackException"/>
        public Action<double, long, long>? ProgressHandler { get; set; }

        /// <summary>
        /// The curl command text these options were parsed from (or an equivalent command built by the fluent builder),
        /// kept for logging, error messages and <see cref="CurlResult.Command"/>.
        /// </summary>
        /// <value>The command string, or null for options you built yourself.</value>
        /// <remarks>
        /// <para>It is copied into every exception's <see cref="CurlDotNet.Exceptions.CurlException.Command"/>. It may contain
        /// secrets (<c>-u user:password</c>, tokens), so treat it like the credentials themselves when logging. A non-null
        /// value also marks the options as "parsed", which makes <see cref="HeadOnly"/> print headers like the curl tool.</para>
        /// <para>Example: <c>Curl.Validate("curl -I https://example.com").ParsedOptions.OriginalCommand</c> is <c>"curl -I https://example.com"</c>.</para>
        /// </remarks>
        /// <seealso cref="Flavor"/>
        /// <seealso cref="CurlResult.Command"/>
        /// <seealso cref="CurlRequestBuilder.ToCurlCommand"/>
        public string? OriginalCommand { get; set; }

        /// <summary>
        /// The shell flavor the command text was read with: which quoting, escaping, line-continuation and
        /// variable rules turned <see cref="OriginalCommand"/> into curl arguments.
        /// </summary>
        /// <value>
        /// <see cref="CurlFlavor.Auto"/> (the default) on options you build yourself. After a command string
        /// has been parsed it is the flavor actually used: the one you asked for, or, when you asked for
        /// <see cref="CurlFlavor.Auto"/>, the one Auto detected (<see cref="CurlFlavor.Posix"/>,
        /// <see cref="CurlFlavor.WindowsCmd"/>, <see cref="CurlFlavor.PowerShell"/> or
        /// <see cref="CurlFlavor.InvokeWebRequest"/>). Every transfer of a multi-URL command carries the same value.
        /// </value>
        /// <remarks>
        /// <para>Setting it on a <see cref="CurlOptions"/> object you pass to <see cref="Curl.ExecuteAsync(CurlOptions)"/>
        /// has no effect, because nothing is parsed there; choose the flavor when you pass a command string, with
        /// <see cref="Curl.ExecuteAsync(string, CurlFlavor)"/>, <see cref="CurlSettings.WithFlavor(CurlFlavor)"/>
        /// or <see cref="Curl.DefaultFlavor"/>.</para>
        /// <para>Example: after <c>var v = Curl.Validate("curl.exe -d '{\"a\":1}' https://x.example", CurlFlavor.PowerShell);</c>
        /// <c>v.ParsedOptions.Flavor</c> is <c>CurlFlavor.PowerShell</c>; log it next to the command when a pasted
        /// request does not behave the way it did in the terminal.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md</para>
        /// </remarks>
        /// <seealso cref="Curl.DetectFlavor(string)"/>
        /// <seealso cref="Curl.DefaultFlavor"/>
        /// <seealso cref="CurlSettings.WithFlavor(CurlFlavor)"/>
        public CurlFlavor Flavor { get; set; }

        /// <summary>
        /// curl's <c>-#</c>/<c>--progress-bar</c> (draw a bar instead of the meter); recorded for compatibility only,
        /// because CurlDotNet never draws progress on the terminal.
        /// </summary>
        /// <value>True when the option was given. Default: false. Not acted on.</value>
        /// <remarks>
        /// <para>Use <see cref="ProgressHandler"/> (or <c>CurlSettings.WithProgress</c>) to show your own progress.</para>
        /// </remarks>
        /// <seealso cref="ProgressHandler"/>
        public bool ProgressBar { get; set; }

        /// <summary>
        /// Seconds from curl's <c>--keepalive-time</c> (idle time before TCP keep-alive probes); only used by FTP
        /// transfers, where any value turns keep-alive on.
        /// </summary>
        /// <value>Whole seconds, or null. Default: null.</value>
        /// <remarks>
        /// <para>HTTP transfers use the pooled connection settings of HttpClient regardless of this value.
        /// Example: <c>curl --keepalive-time 60 ftp://example.com/file</c> parses to <c>KeepAliveTime = 60</c>.</para>
        /// </remarks>
        /// <seealso cref="FtpPassive"/>
        public int? KeepAliveTime { get; set; }

        /// <summary>
        /// DNS server list from curl's <c>--dns-servers</c>; not applied (names are resolved by the operating system), so
        /// the parser also records the option in <see cref="UnsupportedOptions"/>. Use <see cref="Resolve"/> to pin a name
        /// to an address.
        /// </summary>
        /// <value>A comma-separated list such as <c>8.8.8.8,1.1.1.1</c>. Default: null. Not acted on.</value>
        /// <remarks>
        /// <para>Example: <c>curl --dns-servers 1.1.1.1 URL</c> parses to <c>DnsServers = "1.1.1.1"</c> and
        /// <c>UnsupportedOptions</c> containing <c>--dns-servers</c>.</para>
        /// </remarks>
        /// <seealso cref="Resolve"/>
        /// <seealso cref="UnsupportedOptions"/>
        public string? DnsServers { get; set; }

        /// <summary>
        /// Pins host names to addresses without DNS, like curl's <c>--resolve HOST:PORT:ADDRESS</c>; the URL keeps its
        /// host (TLS name and <c>Host</c> header) while the connection goes to the given address.
        /// </summary>
        /// <value>A dictionary of <c>"host:port"</c> to <c>"address[,address...]"</c>, for example
        /// <c>["example.com:443"] = "127.0.0.1"</c>; <c>"*:443"</c> matches every host on that port. Default: empty.</value>
        /// <remarks>
        /// <para>Only entries whose port matches the URL's port apply; IPv6 addresses may be written in brackets. The parser
        /// rejects an entry without a numeric port with <see cref="CurlDotNet.Exceptions.CurlOptionSyntaxException"/> (exit 49).
        /// Typical uses: testing a new server before DNS changes, or sending traffic to a local container.</para>
        /// <para>Example: <c>curl --resolve example.com:443:10.0.0.5 https://example.com/</c> is
        /// <c>options.Resolve["example.com:443"] = "10.0.0.5";</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md</para>
        /// </remarks>
        /// <seealso cref="DnsServers"/>
        /// <seealso cref="Url"/>
        public Dictionary<string, string> Resolve { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// FTP commands from curl's <c>-Q</c>/<c>--quote</c> (such as <c>DELE file</c>); not sent to the server, so the
        /// parser also records the option in <see cref="UnsupportedOptions"/>.
        /// </summary>
        /// <value>The commands in command-line order. Default: empty. Not acted on.</value>
        /// <remarks>
        /// <para>The .NET FTP client CurlDotNet uses cannot send raw commands. Example: <c>curl -Q "DELE old.txt" ftp://example.com/</c>
        /// parses to <c>Quote = ["DELE old.txt"]</c> and <c>UnsupportedOptions</c> containing <c>--quote</c>.</para>
        /// </remarks>
        /// <seealso cref="FtpPassive"/>
        /// <seealso cref="UnsupportedOptions"/>
        public List<string> Quote { get; set; } = new List<string>();

        /// <summary>
        /// Creates missing folders of the output paths (<c>-o</c>, <c>-O</c> with <c>--output-dir</c>, <c>-D</c>,
        /// <c>--etag-save</c>) before writing, like curl's <c>--create-dirs</c>.
        /// </summary>
        /// <value>True to create folders. Default: false: a missing folder fails with exit code 23
        /// (<see cref="CurlDotNet.Exceptions.CurlWriteErrorException"/>).</value>
        /// <remarks>
        /// <para>Example: <c>curl --create-dirs -o downloads/2024/report.pdf URL</c> is
        /// <c>new CurlOptions { Url = url, OutputFile = "downloads/2024/report.pdf", CreateDirs = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="OutputFile"/>
        /// <seealso cref="OutputDir"/>
        /// <seealso cref="DumpHeaderFile"/>
        public bool CreateDirs { get; set; }

        /// <summary>
        /// Uses passive mode for FTP data connections (the client connects to the server), which works through most
        /// firewalls and NAT; curl's <c>--ftp-pasv</c>, on by default.
        /// </summary>
        /// <value>True for passive mode. Default: true. Set false for active mode.</value>
        /// <remarks>
        /// <para>Only affects <c>ftp://</c> and <c>ftps://</c> URLs. Example: <c>curl --ftp-pasv ftp://example.com/file.txt</c>
        /// is <c>new CurlOptions { Url = "ftp://example.com/file.txt", FtpPassive = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="FtpSsl"/>
        public bool FtpPassive { get; set; } = true;

        /// <summary>
        /// Encrypts an <c>ftp://</c> transfer with TLS (explicit FTPS), like curl's <c>--ftp-ssl</c>/<c>--ssl</c>;
        /// <c>ftps://</c> URLs are always encrypted.
        /// </summary>
        /// <value>True to require TLS. Default: false.</value>
        /// <remarks>
        /// <para>Example: <c>curl --ftp-ssl -u user:pass ftp://example.com/report.csv</c> is
        /// <c>new CurlOptions { Url = "ftp://example.com/report.csv", FtpSsl = true, Credentials = new NetworkCredential("user", "pass") }</c>.</para>
        /// </remarks>
        /// <seealso cref="FtpPassive"/>
        public bool FtpSsl { get; set; }

        /// <summary>
        /// A SOCKS proxy for every connection, like curl's <c>--socks5 host:port</c> (or <c>--socks4</c>,
        /// <c>--socks4a</c>, <c>--socks5-hostname</c>, which arrive here as <c>socks4://</c>, <c>socks4a://</c>, <c>socks5h://</c> URLs).
        /// </summary>
        /// <value>A proxy URL like <c>socks5://127.0.0.1:1080</c>, or <c>host:port</c> (treated as <c>socks5://</c>).
        /// Default: null. <see cref="Proxy"/> wins when both are set.</value>
        /// <remarks>
        /// <para>SOCKS proxies need .NET 6 or later. <c>socks5h</c> lets the proxy resolve host names (needed for <c>.onion</c>
        /// and internal names). Example: <c>curl --socks5-hostname 127.0.0.1:9050 https://example.com/</c> is
        /// <c>new CurlOptions { Url = url, Socks5Proxy = "socks5h://127.0.0.1:9050" }</c>.</para>
        /// </remarks>
        /// <seealso cref="Proxy"/>
        /// <seealso cref="NoProxy"/>
        public string? Socks5Proxy { get; set; }

        /// <summary>
        /// How many times a transfer that failed with a transient error is retried, like curl's <c>--retry N</c>;
        /// transient means a timeout, a host or proxy name that did not resolve, or an HTTP 408, 429, 500, 502, 503 or 504.
        /// </summary>
        /// <value>A retry count. Default: 0 (no retries).</value>
        /// <remarks>
        /// <para>Between attempts CurlDotNet waits <see cref="RetryDelay"/> seconds when set; otherwise curl's backoff (1, 2, 4...
        /// seconds, at most 10 minutes), or the server's <c>Retry-After</c> (whole seconds) when that is longer.
        /// <see cref="RetryMaxTime"/> bounds the total time. <see cref="RetryAllErrors"/> and <see cref="RetryConnRefused"/>
        /// widen what counts as transient. When the last attempt fails, its error is thrown. Each attempt's output is kept
        /// in <see cref="CurlResult.Body"/>; the <see cref="WriteOut"/> text is written once, for the last attempt, with
        /// <c>%{num_retries}</c> set to the number of retries made.</para>
        /// <para>Example: <c>curl --retry 3 --retry-delay 2 URL</c> is <c>new CurlOptions { Url = url, Retry = 3, RetryDelay = 2 }</c>.</para>
        /// </remarks>
        /// <seealso cref="RetryDelay"/>
        /// <seealso cref="RetryMaxTime"/>
        /// <seealso cref="RetryAllErrors"/>
        /// <seealso cref="RetryConnRefused"/>
        /// <seealso cref="CurlSettings.RetryCount"/>
        public int Retry { get; set; }

        /// <summary>
        /// Fixed number of seconds to wait between retries (decimals allowed), like curl's <c>--retry-delay</c>; without it
        /// curl's exponential backoff (1, 2, 4... seconds) is used.
        /// </summary>
        /// <value>Seconds. Default: 0 (use the server's <c>Retry-After</c> or exponential backoff).</value>
        /// <remarks>
        /// <para>A fixed delay is not overridden by the server's <c>Retry-After</c> header; leave it at 0 to honour
        /// <c>Retry-After</c>. A negative or non-numeric value on the command line fails with exit code 2.
        /// Example: <c>curl --retry 5 --retry-delay 0.5 URL</c> is <c>new CurlOptions { Url = url, Retry = 5, RetryDelay = 0.5 }</c>.</para>
        /// </remarks>
        /// <seealso cref="Retry"/>
        /// <seealso cref="CurlSettings.RetryDelayMs"/>
        public double RetryDelay { get; set; }

        /// <summary>
        /// Retries on every failure (any HTTP error with <c>-f</c>, refused connections, DNS errors...), not only the
        /// transient ones curl retries by default; curl's <c>--retry-all-errors</c>.
        /// </summary>
        /// <value>True to retry everything. Default: false. Only used when <see cref="Retry"/> is above 0.</value>
        /// <remarks>
        /// <para><c>CurlSettings.WithRetries</c> turns this on. Example: <c>curl --retry 3 --retry-all-errors URL</c> is
        /// <c>new CurlOptions { Url = url, Retry = 3, RetryAllErrors = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Retry"/>
        /// <seealso cref="RetryConnRefused"/>
        public bool RetryAllErrors { get; set; }

        /// <summary>
        /// Also treats "connection refused" (exit 7) as a transient error worth retrying, for services that are still
        /// starting up; curl's <c>--retry-connrefused</c>.
        /// </summary>
        /// <value>True to retry refused connections. Default: false. Only used when <see cref="Retry"/> is above 0.</value>
        /// <remarks>
        /// <para>Example: waiting for a container: <c>curl --retry 10 --retry-connrefused http://localhost:8080/health</c> is
        /// <c>new CurlOptions { Url = url, Retry = 10, RetryConnRefused = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Retry"/>
        /// <seealso cref="RetryAllErrors"/>
        public bool RetryConnRefused { get; set; }

        /// <summary>
        /// Stops retrying once this many seconds have passed since the first attempt (or when the server's
        /// <c>Retry-After</c> wait would pass it), like curl's <c>--retry-max-time</c>; the last attempt's outcome then
        /// stands: its result is returned, or its error thrown.
        /// </summary>
        /// <value>Whole seconds. Default: 0 (no limit beyond <see cref="Retry"/>).</value>
        /// <remarks>
        /// <para>Example: <c>curl --retry 100 --retry-max-time 60 URL</c> is <c>new CurlOptions { Url = url, Retry = 100, RetryMaxTime = 60 }</c>.</para>
        /// </remarks>
        /// <seealso cref="Retry"/>
        /// <seealso cref="MaxTime"/>
        public int RetryMaxTime { get; set; }

        /// <summary>
        /// Follows redirects and keeps sending <see cref="Credentials"/> (and bearer tokens) when a redirect goes to another
        /// host, like curl's <c>--location-trusted</c>; only use it with hosts you trust.
        /// </summary>
        /// <value>True to trust redirect targets. Default: false: credentials are only sent to the original host.</value>
        /// <remarks>
        /// <para>The parser also sets <see cref="FollowLocation"/>. Example: <c>curl --location-trusted -u ada:s3cret https://example.com/api</c> is
        /// <c>new CurlOptions { Url = url, FollowLocation = true, LocationTrusted = true, Credentials = new NetworkCredential("ada", "s3cret") }</c>.</para>
        /// </remarks>
        /// <seealso cref="FollowLocation"/>
        /// <seealso cref="Credentials"/>
        public bool LocationTrusted { get; set; }

        /// <summary>
        /// curl's <c>--disable-epsv</c> (do not use the FTP EPSV command); recorded for compatibility but not applied:
        /// the .NET FTP client decides the passive command itself.
        /// </summary>
        /// <value>True when given. Default: false. Not acted on.</value>
        /// <remarks><para>Example: <c>curl --disable-epsv ftp://example.com/</c> parses to <c>DisableEpsv = true</c>.</para></remarks>
        /// <seealso cref="FtpPassive"/>
        /// <seealso cref="DisableEprt"/>
        public bool DisableEpsv { get; set; }

        /// <summary>
        /// curl's <c>--disable-eprt</c> (do not use the FTP EPRT command in active mode); recorded for compatibility but
        /// not applied by the .NET FTP client.
        /// </summary>
        /// <value>True when given. Default: false. Not acted on.</value>
        /// <remarks><para>Example: <c>curl --disable-eprt ftp://example.com/</c> parses to <c>DisableEprt = true</c>.</para></remarks>
        /// <seealso cref="FtpPassive"/>
        /// <seealso cref="DisableEpsv"/>
        public bool DisableEprt { get; set; }

        /// <summary>
        /// Signs the request with AWS Signature Version 4 (S3, API Gateway, other AWS services), like curl's
        /// <c>--aws-sigv4 provider1[:provider2[:region[:service]]]</c>.
        /// </summary>
        /// <value>The signing configuration, or null (default) for no signing.</value>
        /// <remarks>
        /// <para>Credentials come from <see cref="Credentials"/> (<c>-u ACCESS_KEY:SECRET_KEY</c>); without them nothing is signed.
        /// The signature covers the method, path, query, signed headers and a hash of the body, so it is computed after the
        /// body is set. <see cref="OAuth2Bearer"/> takes precedence. The signature is dropped on a redirect to another host.</para>
        /// <para>Common values: <c>"aws:amz:us-east-1:s3"</c> (S3 in us-east-1), <c>"aws:amz:eu-west-1:execute-api"</c>
        /// (API Gateway), <c>"gcp:goog"</c> (Google Cloud). curl manual: https://curl.se/libcurl/c/CURLOPT_AWS_SIGV4.html</para>
        /// <para>Example: <c>curl --aws-sigv4 "aws:amz:us-east-1:s3" -u AKID:SECRET https://my-bucket.s3.amazonaws.com/key.txt</c>
        /// as a command string: <c>await Curl.ExecuteAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u AKID:SECRET https://my-bucket.s3.amazonaws.com/key.txt")</c>.</para>
        /// </remarks>
        /// <example>
        /// <code language="csharp">
        /// // Equivalent to: curl --aws-sigv4 "aws:amz:us-east-1:s3" --user "AKID:SECRET" https://s3.amazonaws.com/bucket
        /// var result = await Curl.ExecuteAsync("curl --aws-sigv4 \"aws:amz:us-east-1:s3\" -u \"AKID:SECRET\" https://s3.amazonaws.com/bucket");
        /// </code>
        /// </example>
        /// <seealso cref="Credentials"/>
        /// <seealso cref="AwsSigV4Config"/>
        public AwsSigV4Config? AwsSigV4 { get; set; }

        /// <summary>
        /// Treats the response as binary whatever its Content-Type, so the body bytes land unchanged in
        /// <see cref="CurlResult.BinaryData"/> instead of being decoded as text.
        /// </summary>
        /// <value>True to force binary. Default: false (CurlDotNet decides from Content-Type and by sniffing the bytes).</value>
        /// <remarks>
        /// <para>Use it for servers that label files wrongly (an <c>.xlsx</c> served as <c>text/plain</c>), which would otherwise be
        /// corrupted by text decoding. A CurlDotNet setting with no curl flag (curl never decodes bodies).</para>
        /// <para>Example: <c>new CurlOptions { Url = "https://example.com/report.xlsx", ForceBinary = true }</c>, then
        /// <c>File.WriteAllBytes("report.xlsx", result.BinaryData)</c>.</para>
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
        /// <seealso cref="BinaryContentTypes"/>
        /// <seealso cref="TextContentTypes"/>
        /// <seealso cref="CurlResult.BinaryData"/>
        /// <seealso cref="CurlRequestBuilder.AsBinary"/>
        public bool ForceBinary { get; set; }

        /// <summary>
        /// Extra MIME types that are always read as binary (into <see cref="CurlResult.BinaryData"/>), for custom types
        /// your server uses; checked before every other text/binary rule.
        /// </summary>
        /// <value>MIME types such as <c>"application/x-custom-binary"</c>, compared case-insensitively without parameters.
        /// Default: empty.</value>
        /// <remarks>
        /// <para>The library already recognises common binary types including Office documents (.xlsx, .docx, .pptx), PDFs,
        /// archives, images, audio, video and fonts. This list has the highest priority, above <see cref="TextContentTypes"/>.</para>
        /// <para>Example: <c>options.BinaryContentTypes.Add("application/x-protobuf");</c>.</para>
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
        /// <seealso cref="TextContentTypes"/>
        /// <seealso cref="ForceBinary"/>
        /// <seealso cref="CurlRequestBuilder.WithBinaryContentType(string)"/>
        public List<string> BinaryContentTypes { get; set; } = new List<string>();

        /// <summary>
        /// Extra MIME types that are always read as text (into <see cref="CurlResult.Body"/>), for custom types the
        /// library would otherwise treat as binary.
        /// </summary>
        /// <value>MIME types such as <c>"application/x-ndjson"</c>. Default: empty.</value>
        /// <remarks>
        /// <para>These take priority over the built-in binary detection, but not over <see cref="BinaryContentTypes"/> or
        /// <see cref="ForceBinary"/>.</para>
        /// <para>Example: <c>options.TextContentTypes.Add("application/x-custom-config");</c> then read <c>result.Body</c>.</para>
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
        /// <seealso cref="BinaryContentTypes"/>
        /// <seealso cref="ForceBinary"/>
        /// <seealso cref="CurlRequestBuilder.WithTextContentType(string)"/>
        public List<string> TextContentTypes { get; set; } = new List<string>();

        /// <summary>
        /// Returns a new options object with the same request settings, so you can derive several similar requests
        /// from one template without the requests affecting each other.
        /// </summary>
        /// <returns>A new <see cref="CurlOptions"/> with every property copied. Dictionaries, lists and sets
        /// (<see cref="Headers"/>, <see cref="FormData"/>, <see cref="Files"/>, <see cref="Resolve"/>, <see cref="Quote"/>,
        /// <see cref="EmptyHeaders"/>, <see cref="FormParts"/>, the content-type lists, <see cref="UnknownOptions"/>,
        /// <see cref="UnsupportedOptions"/>, <see cref="GlobMatches"/>) are new collections with the same entries and the same
        /// key comparer; <see cref="BinaryData"/> is a copy of the array; each of <see cref="AdditionalTransfers"/> is itself
        /// cloned. <see cref="Credentials"/>, <see cref="ProxyCredentials"/>, <see cref="ProgressHandler"/>,
        /// <see cref="AwsSigV4"/> and the <see cref="CurlFormPart"/> objects inside <see cref="FormParts"/> are shared
        /// references (change them by assigning new ones).</returns>
        /// <remarks>
        /// <para>The clone does not belong to a running command line: the state CurlDotNet attaches while a command runs
        /// (the cookie store shared by its transfers) is not carried over.</para>
        /// <para>Example: <c>var template = new CurlOptions { Url = "https://api.example.com", MaxTime = 10 };</c> then
        /// <c>var a = template.Clone(); a.Url += "/users";</c> and <c>var b = template.Clone(); b.Url += "/orders";</c>.</para>
        /// </remarks>
        /// <seealso cref="CurlRequestBuilder.GetOptions"/>
        public CurlOptions Clone()
        {
            // Every property, public and internal, is copied by MemberwiseClone; then each mutable collection is
            // replaced by a copy so the clone and the original never share one (ReleaseFixesTests checks both).
            var clone = (CurlOptions)MemberwiseClone();
            clone.Headers = Headers == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(Headers, Headers.Comparer);
            clone.FormData = FormData == null ? new Dictionary<string, string>() : new Dictionary<string, string>(FormData, FormData.Comparer);
            clone.Files = Files == null ? new Dictionary<string, string>() : new Dictionary<string, string>(Files, Files.Comparer);
            clone.BinaryData = BinaryData == null ? null : (byte[])BinaryData.Clone();
            clone.Resolve = Resolve == null ? new Dictionary<string, string>() : new Dictionary<string, string>(Resolve, Resolve.Comparer);
            clone.Quote = Quote == null ? new List<string>() : new List<string>(Quote);
            clone.BinaryContentTypes = BinaryContentTypes == null ? new List<string>() : new List<string>(BinaryContentTypes);
            clone.TextContentTypes = TextContentTypes == null ? new List<string>() : new List<string>(TextContentTypes);
            clone.FormParts = FormParts == null ? new List<CurlFormPart>() : new List<CurlFormPart>(FormParts);
            clone.EmptyHeaders = EmptyHeaders == null
                ? new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                : new HashSet<string>(EmptyHeaders, EmptyHeaders.Comparer);
            clone.UnknownOptions = new List<string>(UnknownOptions);
            clone.UnsupportedOptions = new List<string>(UnsupportedOptions);
            clone.AdditionalTransfers = AdditionalTransfers.Select(t => t.Clone()).ToList();
            clone.GlobMatches = new List<string>(GlobMatches);
            clone.DataParts = new List<RequestDataPart>(DataParts);
            clone.ImplicitHeaders = new HashSet<string>(ImplicitHeaders, ImplicitHeaders.Comparer);
            clone.ProxyHeaderLines = new List<string>(ProxyHeaderLines);
            clone.CookieFiles = CookieFiles == null ? new List<string>() : new List<string>(CookieFiles);
            clone.RawHeaderLines = RawHeaderLines == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(RawHeaderLines, RawHeaderLines.Comparer);
            clone.ConnectTo = ConnectTo == null ? new List<string>() : new List<string>(ConnectTo);
            clone.UrlQueryParts = new List<string>(UrlQueryParts);
            clone.RequestKinds = new List<KeyValuePair<RequestKind, string>>(RequestKinds);
            clone.ParsedDataBytes = ParsedDataBytes == null ? null : (byte[])ParsedDataBytes.Clone();
            clone.Session = null; // the cookie store of the command line the original ran in
            clone.TlsVerification = null; // what the original's last certificate check saw
            clone.RetryNumber = 0;
            clone.DeferWriteOut = false;
            return clone;
        }

        #region Request shaping options

        /// <summary>
        /// Real curl options that appeared on the command line but that CurlDotNet does not act on yet, in the order
        /// they were given (for example <c>--tcp-nodelay</c> or <c>--ciphers</c>).
        /// </summary>
        /// <value>The long option names; empty when every option was understood (and for hand-built options).</value>
        /// <remarks>
        /// <para>The parser knows every option of curl 8.7.1 (and the newer ones used by curl's own test suite), so a
        /// recognised-but-unimplemented option still consumes its argument correctly and the URL is never mistaken for it:
        /// <c>curl --ciphers ALL https://example.com</c> requests <c>https://example.com</c>. Such options land here instead of
        /// in <see cref="UnknownOptions"/>, which only holds names that curl itself would reject with exit code 2.</para>
        /// <para>Each entry is the long form as typed, for example <c>--tcp-nodelay</c> or <c>--no-sessionid</c>; short forms are
        /// recorded by their long name (<c>-q</c> becomes <c>--disable</c>). Options that are parsed into a property which
        /// cannot be applied are listed here too: <c>--interface</c>, <c>--dns-servers</c>, <c>--quote</c>,
        /// <c>--disable-epsv</c>, <c>--disable-eprt</c>, <c>--key-type</c> other than PEM, <c>--ciphers</c>/<c>--tls13-ciphers</c>
        /// where the platform cannot choose cipher suites, and on .NET Standard 2.0/.NET Framework <c>--cacert</c>,
        /// <c>--capath</c>, <c>--cert</c>, <c>--key</c> and <c>--pinnedpubkey</c>.</para>
        /// <para>Example: after <c>var o = Curl.Validate(cmd).ParsedOptions;</c> check <c>o.UnsupportedOptions.Count == 0</c>
        /// to know the request will be sent exactly as curl would send it.</para>
        /// </remarks>
        /// <seealso cref="UnknownOptions"/>
        /// <seealso cref="Curl.Validate(string)"/>
        public List<string> UnsupportedOptions { get; private set; } = new List<string>();

        /// <summary>
        /// Sends the <c>-d</c>/<c>--data*</c> data as a query string on a GET request instead of a POST body, like
        /// curl's <c>-G</c>/<c>--get</c>; handy for search forms and APIs with many query parameters.
        /// </summary>
        /// <value>True to move the data into the URL. Default: false.</value>
        /// <remarks>
        /// <para>The parser applies this itself: the joined data is appended to <see cref="Url"/> after a <c>?</c> (or <c>&amp;</c>
        /// when the URL already has a query), <see cref="Data"/> is cleared and the method stays GET (HEAD when <c>-I</c> is also
        /// given). Setting the property on hand-built options has the same effect at send time.</para>
        /// <para>Example: <c>curl -G -d q=dotnet -d page=2 https://example.com/search</c> requests
        /// <c>GET /search?q=dotnet&amp;page=2</c>; in C#, <c>new CurlOptions { Url = "https://example.com/search", Data = "q=dotnet&amp;page=2", UseGet = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Data"/>
        /// <seealso cref="Url"/>
        public bool UseGet { get; set; }

        /// <summary>
        /// Local file uploaded as the request body with PUT, like curl's <c>-T</c>/<c>--upload-file FILE</c>; the URL gets
        /// the file name appended when it ends with <c>/</c>.
        /// </summary>
        /// <value>The path as given on the command line. Default: null.</value>
        /// <remarks>
        /// <para>The parser reads the file into <see cref="BinaryData"/> and selects PUT unless <c>-X</c> names another method; for
        /// hand-built options set <see cref="BinaryData"/> as well. No Content-Type is sent unless a <c>-H "Content-Type: ..."</c>
        /// header is given, exactly like curl. When the URL has no path or its path ends with <c>/</c> (and there is no query
        /// string) the file name is appended to it, percent-encoded: <c>-T report.pdf https://example.com/files/</c> uploads to
        /// <c>/files/report.pdf</c>.</para>
        /// <para>A missing file fails with exit code 26 (<see cref="CurlDotNet.Exceptions.CurlReadErrorException"/>). Reading the
        /// body from standard input (<c>-T -</c> or <c>-T .</c>) is not supported.</para>
        /// <para>Example: <c>curl -T ./backup.zip https://example.com/upload/</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="BinaryData"/>
        /// <seealso cref="Range"/>
        public string? UploadFile { get; set; }

        /// <summary>
        /// OAuth 2.0 bearer token sent as <c>Authorization: Bearer TOKEN</c>, like curl's <c>--oauth2-bearer TOKEN</c>;
        /// the token is only sent to the original host, not to redirect targets elsewhere.
        /// </summary>
        /// <value>The token text without the <c>Bearer </c> prefix. Default: null.</value>
        /// <remarks>
        /// <para>The token is sent verbatim and takes precedence over Basic credentials from <c>-u</c> or the URL, and over
        /// <see cref="AwsSigV4"/>, as in curl. An explicit <c>-H "Authorization: ..."</c> header still wins over it. After a
        /// redirect to another host, port or scheme it is dropped unless <see cref="LocationTrusted"/> is true.</para>
        /// <para>Example: <c>curl --oauth2-bearer eyJhbGciOi... https://api.example.com/me</c> is
        /// <c>new CurlOptions { Url = "https://api.example.com/me", OAuth2Bearer = token }</c>.
        /// Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="Credentials"/>
        /// <seealso cref="Headers"/>
        /// <seealso cref="CurlRequestBuilder.WithBearerToken(string)"/>
        public string? OAuth2Bearer { get; set; }

        /// <summary>
        /// Sets the <c>Referer</c> header automatically to the previous URL when a redirect is followed, like the
        /// <c>;auto</c> suffix of curl's <c>-e</c>/<c>--referer</c>.
        /// </summary>
        /// <value>True to send the previous URL as Referer on each redirect hop. Default: false.</value>
        /// <remarks>
        /// <para><c>-e ";auto"</c> sends no Referer on the first request and the previous URL on every redirect;
        /// <c>-e "https://from.example/;auto"</c> sends <c>https://from.example/</c> first. The parser strips the suffix, so
        /// <see cref="Referer"/> only ever holds the literal value. Needs <see cref="FollowLocation"/>.</para>
        /// <para>Example: <c>curl -L -e ";auto" https://example.com/old-link</c> is
        /// <c>new CurlOptions { Url = url, FollowLocation = true, AutoReferer = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Referer"/>
        /// <seealso cref="FollowLocation"/>
        public bool AutoReferer { get; set; }

        /// <summary>
        /// The date for a conditional request, like curl's <c>-z</c>/<c>--time-cond</c>: the document is only downloaded
        /// when it changed after (or before) this date; null for no condition.
        /// </summary>
        /// <value>A date and time (sent as RFC 1123 GMT). Default: null.</value>
        /// <remarks>
        /// <para>Sent as <c>If-Modified-Since</c> or <c>If-Unmodified-Since</c> depending on <see cref="TimeConditionType"/>. The
        /// parser accepts the date formats curl's <c>curl_getdate</c> does (<c>"Sun, 12 Dec 1999 12:00:00 GMT"</c>,
        /// <c>"dec 12 12:00:00 1999 GMT"</c>, <c>"19991212"</c>, <c>"12 Dec 1999"</c>, ...) or the name of a local file, in which case
        /// the file's last-modified time is used. A leading <c>-</c> asks for "unmodified since", a leading <c>=</c> for an exact
        /// Last-Modified match. When the condition is not met the body is dropped and <see cref="CurlResult.StatusCode"/> is 304.</para>
        /// <para>Example: <c>curl -z "Sun, 12 Dec 1999 12:00:00 GMT" https://example.com/feed.xml</c> is
        /// <c>new CurlOptions { Url = url, TimeCondition = new DateTimeOffset(1999, 12, 12, 12, 0, 0, TimeSpan.Zero) }</c>.</para>
        /// </remarks>
        /// <seealso cref="TimeConditionType"/>
        /// <seealso cref="CurlTimeCondition"/>
        public DateTimeOffset? TimeCondition { get; set; }

        /// <summary>
        /// Which comparison <see cref="TimeCondition"/> requests, like curl's <c>-z DATE</c> (modified since),
        /// <c>-z -DATE</c> (unmodified since) and <c>-z =DATE</c> (Last-Modified match).
        /// </summary>
        /// <value>A <see cref="CurlTimeCondition"/>. Default: <see cref="CurlTimeCondition.IfModifiedSince"/>; ignored while
        /// <see cref="TimeCondition"/> is null.</value>
        /// <remarks>
        /// <para>Example: <c>curl -z "-Jan 1 2024" https://example.com/</c> sends <c>If-Unmodified-Since: Mon, 01 Jan 2024 00:00:00 GMT</c>;
        /// in C#, <c>TimeCondition = new DateTimeOffset(2024, 1, 1, 0, 0, 0, TimeSpan.Zero), TimeConditionType = CurlTimeCondition.IfUnmodifiedSince</c>.</para>
        /// </remarks>
        /// <seealso cref="TimeCondition"/>
        public CurlTimeCondition TimeConditionType { get; set; } = CurlTimeCondition.IfModifiedSince;

        /// <summary>
        /// Largest response body, in bytes, the transfer accepts, like curl's <c>--max-filesize</c>; a larger announced
        /// or received body fails with exit code 63 instead of filling memory or disk.
        /// </summary>
        /// <value>Bytes, or null (default) for no limit.</value>
        /// <remarks>
        /// <para>The parser accepts a plain byte count or one with a <c>k</c>, <c>M</c>, <c>G</c>, <c>T</c> or <c>P</c> suffix (powers of
        /// 1024), for example <c>--max-filesize 10M</c>. An announced <c>Content-Length</c> over the limit fails before the body is
        /// read; otherwise the transfer stops once the limit is passed. The error is
        /// <see cref="CurlDotNet.Exceptions.CurlFileSizeExceededException"/>.</para>
        /// <para>Example: <c>curl --max-filesize 500k https://example.com/avatar.png</c> is <c>new CurlOptions { Url = url, MaxFileSize = 500 * 1024 }</c>.</para>
        /// </remarks>
        /// <seealso cref="MaxTime"/>
        public long? MaxFileSize { get; set; }

        /// <summary>
        /// Requires exactly the <see cref="HttpVersion"/> instead of allowing a fallback, like curl's
        /// <c>--http2-prior-knowledge</c> (HTTP/2 without negotiation) and <c>--http3-only</c>.
        /// </summary>
        /// <value>True to forbid downgrading. Default: false.</value>
        /// <remarks>
        /// <para>Works together with <see cref="HttpVersion"/>, which holds <c>"1.0"</c>, <c>"1.1"</c>, <c>"2.0"</c> or <c>"3.0"</c>. When this
        /// is true the request uses <c>HttpVersionPolicy.RequestVersionExact</c> (on .NET 5 and later), so a server that cannot
        /// speak the requested version fails the request rather than silently downgrading.</para>
        /// <para>Example: <c>curl --http2-prior-knowledge http://localhost:5000/grpc-health</c> is
        /// <c>new CurlOptions { Url = url, HttpVersion = "2.0", HttpVersionExact = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="HttpVersion"/>
        public bool HttpVersionExact { get; set; }

        /// <summary>
        /// Sends the URL path exactly as written, without removing <c>/./</c> and <c>/../</c> segments, like curl's
        /// <c>--path-as-is</c>; used to test path-traversal handling on servers.
        /// </summary>
        /// <value>True to keep dot segments. Default: false (dot segments are resolved, as browsers do).</value>
        /// <remarks>
        /// <para>Honoured on .NET 6 and later, where the request URI is created without path canonicalisation; .NET Standard
        /// 2.0 and .NET Framework always normalise the path.</para>
        /// <para>Example: <c>curl --path-as-is http://localhost:8080/../../etc/passwd</c> is <c>new CurlOptions { Url = url, PathAsIs = true }</c>.</para>
        /// </remarks>
        /// <seealso cref="Url"/>
        public bool PathAsIs { get; set; }

        /// <summary>
        /// Turns off URL globbing so <c>{}</c> and <c>[]</c> in a parsed URL are sent literally instead of expanding into
        /// several requests, like curl's <c>-g</c>/<c>--globoff</c>.
        /// </summary>
        /// <value>True to disable globbing. Default: false.</value>
        /// <remarks>
        /// <para>Without it, <c>curl "https://example.com/{a,b}"</c> and <c>"https://example.com/[1-3]"</c> expand into several URLs
        /// (see <see cref="AdditionalTransfers"/>); with it, the brackets are percent-encoded and sent as one request. Globbing
        /// only happens when a command string is parsed; hand-built options are never globbed.</para>
        /// <para>Example: <c>curl -g "http://[::1]:8080/"</c> for IPv6 literals, or for APIs with <c>filter[name]=x</c> queries.</para>
        /// </remarks>
        /// <seealso cref="AdditionalTransfers"/>
        /// <seealso cref="GlobMatches"/>
        public bool GlobOff { get; set; }

        /// <summary>
        /// Header names sent with an empty value, from curl's <c>-H "Name;"</c> syntax (as opposed to <c>-H "Name:"</c>,
        /// which removes the header).
        /// </summary>
        /// <value>Header names, compared case-insensitively. Default: empty.</value>
        /// <remarks>
        /// <para>curl distinguishes <c>-H "X-Empty;"</c> (send <c>X-Empty:</c> with no value) from <c>-H "X-Empty:"</c> (do not send the
        /// header at all, even one curl would add itself). The latter is stored in <see cref="Headers"/> with an empty value; the
        /// former is stored here. A non-empty value for the same name in <see cref="Headers"/> wins.</para>
        /// <para>Example: <c>curl -H "X-Requested-With;" https://example.com/</c> is <c>options.EmptyHeaders.Add("X-Requested-With");</c>.</para>
        /// </remarks>
        /// <seealso cref="Headers"/>
        public HashSet<string> EmptyHeaders { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Further transfers described by the same command line: extra URLs, URLs produced by globbing (<c>{a,b}</c>,
        /// <c>[1-3]</c>) and groups separated by <c>--next</c>/<c>-:</c>; executing the options runs them all in order.
        /// </summary>
        /// <value>Complete, independent <see cref="CurlOptions"/> for the second and later transfers; empty for a single transfer.</value>
        /// <remarks>
        /// <para>The options object returned by the parser describes the first transfer; every entry here is the next one, in
        /// command-line order. The options of a <c>--next</c> group apply to every URL of that group wherever they appear, and the
        /// nth <c>-o</c>/<c>-O</c>/<c>-T</c> belongs to the nth URL, exactly as in curl:
        /// <c>curl -H "X: 1" https://a.example/ https://b.example/ --next -d x=1 https://c.example/</c> yields a first transfer
        /// for <c>a</c> and two additional ones, <c>b</c> (with the header) and <c>c</c> (a POST without it).</para>
        /// <para>Executing the first options (or the command string) runs every transfer serially and shares cookies between
        /// them, like curl. <see cref="CurlResult.Body"/> is the concatenated output, the status and headers are those of the
        /// last transfer, and <see cref="CurlResult.OutputFiles"/> lists every file. As in curl, a failed transfer does not stop
        /// the rest (unless <see cref="FailEarly"/>, or a fatal error); the command fails only when the last transfer failed.
        /// To run one transfer on its own, pass that entry to <c>Curl.ExecuteAsync(CurlOptions)</c> after clearing its own list.</para>
        /// </remarks>
        /// <seealso cref="GlobOff"/>
        /// <seealso cref="FailEarly"/>
        /// <seealso cref="Curl.ExecuteAsync(CurlOptions)"/>
        public List<CurlOptions> AdditionalTransfers { get; private set; } = new List<CurlOptions>();

        /// <summary>
        /// The text each URL glob matched for this transfer, which curl's <c>#1</c>, <c>#2</c>... placeholders in <c>-o</c>
        /// file names refer to.
        /// </summary>
        /// <value>One string per glob in the URL, in order; empty when the URL has no globs.</value>
        /// <remarks>
        /// <para>For <c>curl "https://example.com/{one,two}/[1-2].txt" -o "#1-#2.txt"</c> the transfer for <c>/two/1.txt</c> holds
        /// <c>["two", "1"]</c>, and the parser has already rewritten <see cref="OutputFile"/> to <c>two-1.txt</c>.</para>
        /// </remarks>
        /// <seealso cref="AdditionalTransfers"/>
        /// <seealso cref="OutputFile"/>
        public List<string> GlobMatches { get; private set; } = new List<string>();

        /// <summary>
        /// Raw <c>-d</c> family items gathered while parsing, before they are joined into <see cref="Data"/>.
        /// </summary>
        internal List<RequestDataPart> DataParts { get; private set; } = new List<RequestDataPart>();

        /// <summary>Header names in <see cref="Headers"/> that an option such as <c>--json</c> added, which a later <c>-H</c> replaces.</summary>
        internal HashSet<string> ImplicitHeaders { get; private set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary><c>-p</c> / <c>--proxytunnel</c>: requests go through a CONNECT tunnel, so no proxy headers are added to them.</summary>
        internal bool ProxyTunnelRequested { get; set; }

        /// <summary><c>--noproxy</c> host list (comma separated, <c>*</c> for all); null means "use NO_PROXY from the environment".</summary>
        internal string? NoProxyHosts { get; set; }

        /// <summary><c>--proxy-header</c> lines, sent on requests that go through a plain HTTP proxy.</summary>
        internal List<string> ProxyHeaderLines { get; private set; } = new List<string>();

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
        internal List<string> UrlQueryParts { get; private set; } = new List<string>();

        /// <summary>
        /// True when <see cref="Credentials"/> came from <c>user:password@</c> in the URL rather than from
        /// <c>-u</c>; each request (redirects included) then uses the user-info of the URL it targets.
        /// </summary>
        internal bool CredentialsFromUrl { get; set; }

        /// <summary>
        /// The kinds of request the options asked for (POST, multipart, PUT, HEAD) with the option that asked,
        /// checked for conflicts when the transfer is finished.
        /// </summary>
        internal List<KeyValuePair<RequestKind, string>> RequestKinds { get; private set; } = new List<KeyValuePair<RequestKind, string>>();

        /// <summary>
        /// Exact body bytes the parser assembled from <see cref="DataParts"/>; only used while <see cref="Data"/>
        /// still equals <see cref="ParsedDataText"/> (so hand edits to <see cref="Data"/> win).
        /// </summary>
        internal byte[]? ParsedDataBytes { get; set; }

        /// <summary>The value <see cref="Data"/> had when <see cref="ParsedDataBytes"/> was assembled.</summary>
        internal string? ParsedDataText { get; set; }

        /// <summary>What the certificate check of the handler built for these options saw (set when TLS options are applied).</summary>
        internal TlsVerificationState? TlsVerification { get; set; }

        /// <summary><see cref="CurlSettings.OnRedirect"/>: called with the absolute URL of each redirect that is followed.</summary>
        internal Action<string>? RedirectCallback { get; set; }

        /// <summary><see cref="CurlSettings.Cookies"/>: a .NET cookie container that supplies and receives cookies on every hop.</summary>
        internal CookieContainer? CookieContainer { get; set; }

        /// <summary>Chunk size, in bytes, for reading response bodies (from <see cref="CurlSettings.BufferSize"/>); 0 means 80 KB.</summary>
        internal int ReadBufferSize { get; set; }

        /// <summary>The 0-based <c>--retry</c> attempt being run (<c>%{num_retries}</c>); 0 outside a retry loop.</summary>
        internal int RetryNumber { get; set; }

        /// <summary>
        /// Set by the <c>--retry</c> loop while another attempt may follow: the attempt keeps its <c>-w</c> text
        /// (and <c>-c -</c> jar) back so it is written once, for the last attempt.
        /// </summary>
        internal bool DeferWriteOut { get; set; }

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

        #endregion
    }

    /// <summary>
    /// The comparison a conditional request makes against the server's Last-Modified time, set with
    /// <see cref="CurlOptions.TimeConditionType"/> or curl's <c>-z DATE</c>, <c>-z -DATE</c> and <c>-z =DATE</c>.
    /// </summary>
    /// <remarks>
    /// <para>Example: set <see cref="CurlOptions.TimeCondition"/> to the date and <see cref="CurlOptions.TimeConditionType"/> to
    /// <c>CurlTimeCondition.IfUnmodifiedSince</c>, or write it curl's way: <c>curl -z -DATE URL</c>. A condition that is not met
    /// gives <see cref="CurlResult.StatusCode"/> 304 and an empty body.</para>
    /// </remarks>
    /// <seealso cref="CurlOptions.TimeCondition"/>
    /// <seealso cref="CurlOptions.TimeConditionType"/>
    public enum CurlTimeCondition
    {
        /// <summary>
        /// Only fetch the document if it changed after the date: sends <c>If-Modified-Since</c> (curl's <c>-z DATE</c>);
        /// the default, used to refresh a cached copy.
        /// </summary>
        /// <remarks><para>Example: <c>TimeConditionType = CurlTimeCondition.IfModifiedSince</c> with yesterday's date downloads
        /// only a newer file; otherwise the result is a 304 with no body.</para></remarks>
        /// <seealso cref="CurlOptions.TimeCondition"/>
        IfModifiedSince = 0,

        /// <summary>
        /// Only fetch the document if it did not change after the date: sends <c>If-Unmodified-Since</c> (curl's
        /// <c>-z -DATE</c>); useful to make sure a resource is still the version you saw.
        /// </summary>
        /// <remarks><para>Example: <c>curl -z "-Jan 1 2024" URL</c> is <c>TimeConditionType = CurlTimeCondition.IfUnmodifiedSince</c>.</para></remarks>
        /// <seealso cref="CurlOptions.TimeCondition"/>
        IfUnmodifiedSince = 1,

        /// <summary>
        /// curl's <c>-z =DATE</c>: sends <c>Last-Modified: DATE</c> and, like curl, only keeps a document whose Last-Modified
        /// is newer than the date (curl applies the "newer" test to this mode too).
        /// </summary>
        /// <remarks><para>Example: <c>curl -z "=Jan 1 2024" URL</c> is <c>TimeConditionType = CurlTimeCondition.LastModified</c>.</para></remarks>
        /// <seealso cref="CurlOptions.TimeCondition"/>
        LastModified = 2,
    }
}
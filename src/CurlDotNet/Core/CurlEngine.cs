/***************************************************************************
 * CurlEngine - Core engine for executing curl commands
 *
 * Transpiled from curl's src/tool_operate.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * TRANSPILATION APPROACH:
 * This engine implements curl's command processing logic by transpiling behavior
 * from curl's C source. The command parsing, option handling, protocol selection,
 * and execution flow match curl's implementation. We use .NET's HttpClient as
 * the transport layer, but all protocol behavior is transpiled from curl's C.
 *
 * This solves the real problem: .NET's HttpClient API is verbose and awkward.
 * By transpiling curl's logic, we get curl's simplicity with .NET's runtime.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 * Sponsored by IronSoftware
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Core engine that processes and executes curl commands.
    /// </summary>
    /// <remarks>
    /// <para>This is the heart of CurlDotNet - translates curl commands to HTTP operations.</para>
    /// <para>AI-Usage: This class handles the actual curl command execution.</para>
    /// </remarks>
    internal class CurlEngine : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ICommandParser _parser;
        private readonly Dictionary<string, IProtocolHandler> _handlers;
        private bool _disposed;

        /// <summary>
        /// Create a new CurlEngine with default HttpClient.
        /// </summary>
        public CurlEngine() : this(CreateDefaultHttpClient(), ownsHttpClient: true)
        {
        }

        /// <summary>
        /// Create a new CurlEngine with custom HttpClient. The client is used exactly as given: its
        /// handler decides TLS validation and proxy behaviour, so <c>-k</c> / <c>-x</c> do not replace it.
        /// </summary>
        public CurlEngine(HttpClient httpClient) : this(httpClient, ownsHttpClient: false)
        {
        }

        private CurlEngine(HttpClient httpClient, bool ownsHttpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient), CurlException.Compose(
                "The HttpClient passed to CurlDotNet is null.",
                "Pass an HttpClient instance (for example from IHttpClientFactory), or use the overload without an HttpClient so CurlDotNet creates and pools its own.",
                DocLinks.HowToAspNetCore));
            _parser = new CommandParser();
            // A caller-supplied HttpClient is used as-is (its handler decides TLS/proxy policy); only
            // the engine's own default client is swapped for a configured one when -k/-x/--digest need it.
            var http = new HttpHandler(_httpClient, ownsHttpClient);
            _handlers = new Dictionary<string, IProtocolHandler>
            {
                ["http"] = http,
                ["https"] = http,
                ["ftp"] = new FtpHandler(),
                ["ftps"] = new FtpHandler(),
                ["file"] = new FileHandler()
            };
        }

        /// <summary>
        /// Execute a curl command string.
        /// </summary>
        /// <param name="command">The curl command to execute</param>
        /// <returns>Result of the curl operation</returns>
        public async Task<CurlResult> ExecuteAsync(string command)
        {
            return await ExecuteAsync(command, CancellationToken.None);
        }

        /// <summary>
        /// Execute a curl command with cancellation support.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(string command, CancellationToken cancellationToken)
        {
            // Validate command
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException(EmptyCommandMessage(command), nameof(command));
            }

            var options = _parser.Parse(command);
            return await ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Execute a curl command written for the given shell flavor (see <see cref="CurlFlavor"/>).
        /// </summary>
        /// <param name="command">The command text.</param>
        /// <param name="flavor">The shell the text was written for; <see cref="CurlFlavor.Auto"/> detects it.</param>
        /// <param name="cancellationToken">Token used to cancel the request.</param>
        public async Task<CurlResult> ExecuteAsync(string command, CurlFlavor flavor, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException(EmptyCommandMessage(command), nameof(command));
            }

            var options = _parser.Parse(command, flavor);
            return await ExecuteAsync(options, cancellationToken);
        }

        /// <summary>The message for a null, empty or whitespace-only command string.</summary>
        private static string EmptyCommandMessage(string? command) => CurlException.Compose(
            $"The curl command to execute is {(command == null ? "null" : command.Length == 0 ? "an empty string" : "only whitespace")}, so there is nothing to run.",
            "Pass a command such as \"curl https://api.example.com/users\" (the leading 'curl' is optional); when it is built from variables, check that they are set.",
            DocLinks.HowToRunCommand);

        /// <summary>
        /// Execute a curl command with custom settings. The command is read with
        /// <see cref="CurlSettings.Flavor"/>, or <see cref="Curl.DefaultFlavor"/> when that is not set.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(string command, CurlSettings settings)
        {
            var options = _parser.Parse(command, settings?.Flavor ?? Curl.DefaultFlavor);

            // Apply settings to options
            if (settings != null)
            {
                ApplySettings(options, settings);
            }

            return await ExecuteAsync(options, settings?.CancellationToken ?? CancellationToken.None);
        }

        /// <summary>
        /// Execute with parsed options.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(CurlOptions options)
        {
            return await ExecuteAsync(options, CancellationToken.None);
        }

        /// <summary>
        /// Execute with parsed options and cancellation.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options), CurlException.Compose(
                    "The CurlOptions passed to ExecuteAsync is null.",
                    "Create the options with new CurlOptions { Url = \"https://...\" }, CurlRequestBuilder.Get(url).GetOptions(), or pass a command string instead.",
                    DocLinks.HowToRunCommand));

            if (_disposed)
                throw new ObjectDisposedException(nameof(CurlEngine), CurlException.Compose(
                    "This CurlEngine (or the DotNetCurl/LibCurl instance that owns it) was disposed and cannot run more transfers.",
                    "Create a new instance, or keep the instance alive (for example as a singleton) for as long as you send requests.",
                    DocLinks.HowItWorks));

            // hand-built options may null out the collections; treat that as "none" instead of a NullReferenceException
            options.Headers ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            options.FormData ??= new Dictionary<string, string>();
            options.Files ??= new Dictionary<string, string>();

            if (options.AdditionalTransfers == null || options.AdditionalTransfers.Count == 0)
            {
                return await ExecuteTransferAsync(options, cancellationToken);
            }

            return await ExecuteTransfersAsync(options, cancellationToken);
        }

        /// <summary>
        /// Runs every transfer of a command line in order, like curl's serial transfer loop: several
        /// URLs, glob matches and <c>--next</c> groups. Standard output is the concatenation of every
        /// transfer's output; status, headers and binary data come from the last transfer that
        /// succeeded; output files of all transfers are listed.
        /// </summary>
        /// <remarks>
        /// A failed transfer does not stop the others (unless <c>--fail-early</c> was given or the error is
        /// one curl treats as fatal). As in curl's serial transfer loop, the outcome is the one of the last
        /// transfer that ran (curl tests 655 and 1328: <c>curl http://failthis/ http://good/</c> exits 0):
        /// when it failed, its exception is thrown with the complete standard output of every transfer in
        /// <see cref="CurlException.StandardOutputDataKey"/>; when it succeeded, the combined result is returned.
        /// </remarks>
        private async Task<CurlResult> ExecuteTransfersAsync(CurlOptions first, CancellationToken cancellationToken)
        {
            var transfers = new List<CurlOptions> { first };
            transfers.AddRange(first.AdditionalTransfers);

            var session = new TransferSession();
            var stdout = new StringBuilder();
            var outputFiles = new List<string>();
            CurlResult? last = null;
            CurlException? lastError = null;

            foreach (var transfer in transfers)
            {
                if (transfer.Session == null)
                {
                    transfer.Session = session;
                }

                if (transfer.SetupError != null)
                {
                    lastError = transfer.SetupError; // like curl: the command stops here with this error
                    break;
                }

                try
                {
                    var result = await ExecuteTransferAsync(transfer, cancellationToken);
                    stdout.Append(result.Body);
                    outputFiles.AddRange(result.OutputFiles);
                    last = result;
                    lastError = null; // like curl, the exit code is the one of the last transfer
                }
                catch (CurlException ex)
                {
                    stdout.Append(TransferStdout.Get(ex));
                    lastError = ex;
                    if (transfer.FailEarly || IsFatalError(ex.CurlErrorCode))
                    {
                        break;
                    }
                }
            }

            if (lastError != null)
            {
                TransferStdout.Set(lastError, stdout.ToString());
                throw lastError;
            }

            var combined = last!;
            combined.Body = stdout.ToString();
            combined.OutputFiles = outputFiles;
            combined.Command = first.OriginalCommand;
            return combined;
        }

        /// <summary>
        /// curl's is_fatal_error(): failures that stop the remaining transfers of a command line
        /// (failed init, out of memory, unknown option, function not found, bad function argument).
        /// </summary>
        private static bool IsFatalError(int? code) =>
            code == 2 || code == 27 || code == 41 || code == 43 || code == 48;

        /// <summary>Runs one transfer through the handler of its URL scheme.</summary>
        private async Task<CurlResult> ExecuteTransferAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            try
            {
                return await RunHandlerAsync(options, cancellationToken);
            }
            catch (CurlException ex)
            {
                // -w is expanded for transfers that failed before any handler ran (bad URL, unknown
                // scheme) too; handlers attach their own, richer output first (this call is then a no-op)
                HttpHandler.AttachFailureOutput(ex, null, null, null, null, options);
                throw;
            }
        }

        /// <summary>Picks the handler for the URL scheme and runs the transfer, translating framework exceptions.</summary>
        private async Task<CurlResult> RunHandlerAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(options.Url))
                throw new CurlException(CurlException.Compose(
                    "No URL specified: the request has no URL to fetch.",
                    "Add the URL to the command (\"curl https://api.example.com/items\") or set CurlOptions.Url; when the URL comes from a variable, check that it is not empty.",
                    DocLinks.Url), 2, options.OriginalCommand);

            options.Url = CommandParser.NormalizeUrl(options.Url);

            // Process-wide opt-ins from Curl.Default* apply to every request that did not set them itself
            if (Curl.DefaultFollowRedirects) options.FollowLocation = true;
            if (Curl.DefaultInsecure) options.Insecure = true;
            if (!options.ConnectTimeout.HasValue && Curl.DefaultConnectTimeoutSeconds > 0) options.ConnectTimeout = Curl.DefaultConnectTimeoutSeconds;

            try
            {
                // Parse URL to determine protocol
                var uri = new Uri(options.Url);
                var protocol = uri.Scheme.ToLower();

                if (!_handlers.TryGetValue(protocol, out var handler))
                {
                    throw new CurlUnsupportedProtocolException(protocol, CurlException.Compose(
                        $"Protocol \"{protocol}\" not supported: {ErrorText.SafeUrl(options.Url)} uses a scheme CurlDotNet does not implement",
                        "Use an http://, https://, ftp://, ftps:// or file:// URL; check for a typo in the scheme (for example htps://).",
                        DocLinks.SupportedProtocols), options.OriginalCommand);
                }

                // Execute with appropriate handler
                var result = await handler.ExecuteAsync(options, cancellationToken);
                result.Command = options.OriginalCommand;

                return result;
            }
            catch (UriFormatException ex)
            {
                // the parser adds http:// to a URL without a scheme (like curl); name the text the user typed
                var typed = options.Url ?? string.Empty;
                var added = typed.StartsWith("http://", StringComparison.Ordinal)
                            && !string.IsNullOrEmpty(options.OriginalCommand)
                            && options.OriginalCommand!.IndexOf(typed, StringComparison.Ordinal) < 0
                            && options.OriginalCommand.IndexOf(typed.Substring(7), StringComparison.Ordinal) >= 0;
                var shown = added
                    ? $"'{ErrorText.SafeUrl(typed.Substring(7))}' (read as '{ErrorText.SafeUrl(typed)}' after adding http://, because it has no scheme curl recognises)"
                    : $"'{ErrorText.SafeUrl(typed)}'";
                throw new CurlMalformedUrlException(options.Url, CurlException.Compose(
                    $"{shown} could not be parsed as a URL ({ex.Message})",
                    "Write it as scheme://host[:port]/path, percent-encode spaces (%20) and other special characters, and quote the URL in the command when it contains & ? or spaces.",
                    DocLinks.Url), options.OriginalCommand);
            }
            catch (CurlAbortedByCallbackException ex) when (cancellationToken.IsCancellationRequested)
            {
                // Standard .NET cancellation contract at the public boundary: catch (OperationCanceledException) just works
                throw new OperationCanceledException(CancelledMessage(options), ex, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException(CancelledMessage(options), ex, cancellationToken);
                }
                var timeoutSeconds = GetTimeoutSeconds(options);
                throw new CurlTimeoutException(CurlException.Compose(
                    $"Operation timed out after {timeoutSeconds.ToString(System.Globalization.CultureInfo.InvariantCulture)} seconds while fetching {ErrorText.SafeUrl(options.Url)}",
                    "Raise -m/--max-time (or CurlSettings.WithTimeout) for slow servers or large downloads; if you passed your own HttpClient, its Timeout also applies.",
                    DocLinks.Timeout),
                    options.OriginalCommand,
                    TimeSpan.FromSeconds(timeoutSeconds));
            }
            catch (HttpRequestException ex)
            {
                var uri = new Uri(options.Url);
                var port = uri.Port > 0 ? uri.Port : (uri.Scheme == "https" ? 443 : 80);
                throw new CurlCouldntConnectException(uri.Host, port, InnermostMessage(ex), viaProxy: false, options.OriginalCommand);
            }
            catch (IOException ex)
            {
                // last line of defence: no raw I/O exception leaves the engine
                throw new CurlReceiveErrorException(CurlException.Compose(
                    $"The transfer of {ErrorText.SafeUrl(options.Url)} failed with an I/O error: {ex.GetType().Name}: {ex.Message}",
                    "The connection or a local file broke during the transfer; retry with --retry 3, and run with --trace-ascii - to see where it stopped.",
                    DocLinks.ErrorReference), options.OriginalCommand, ex);
            }
        }

        private static string CancelledMessage(CurlOptions options) => CurlException.Compose(
            $"The curl request to {ErrorText.SafeUrl(options.Url)} was cancelled by the CancellationToken passed to the call.",
            "This is expected when you cancel on purpose: catch OperationCanceledException. Otherwise check what cancels the token (CancelAfter, an ASP.NET Core RequestAborted token).",
            DocLinks.TutorialCancellation);

        private static string InnermostMessage(Exception ex)
        {
            var e = ex;
            while (e.InnerException != null)
            {
                e = e.InnerException;
            }
            return e.Message.TrimEnd('.');
        }

        /// <summary>
        /// Validate a curl command without executing it, reading it with <see cref="Curl.DefaultFlavor"/>.
        /// </summary>
        public ValidationResult Validate(string command)
        {
            return Validate(command, Curl.DefaultFlavor);
        }

        /// <summary>
        /// Validate a curl command written for the given shell flavor without executing it.
        /// </summary>
        public ValidationResult Validate(string command, CurlFlavor flavor)
        {
            try
            {
                var options = _parser.Parse(command, flavor);

                if (string.IsNullOrEmpty(options.Url))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = CurlException.Compose("No URL specified: the command has options but nothing that looks like a URL.",
                            "Add the URL, for example curl -H 'Accept: application/json' https://api.example.com/items, and quote it when it contains & or ?.",
                            DocLinks.Url)
                    };
                }

                if (options.UnknownOptions.Count > 0)
                {
                    // curl exits with code 2 here; Validate() is the place to surface the typo
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = CurlException.Compose($"Unknown option: {string.Join(", ", options.UnknownOptions)} (the curl tool would stop with exit code 2; Execute ignores it).",
                            "Check the spelling against 'curl --help all' (options are case-sensitive: -o and -O differ), or remove the option if it belongs to a newer curl.",
                            DocLinks.CliCompatibility),
                        ParsedOptions = options
                    };
                }

                try
                {
                    var uri = new Uri(options.Url);
                    if (!_handlers.ContainsKey(uri.Scheme.ToLower()))
                    {
                        return new ValidationResult
                        {
                            IsValid = false,
                            Error = CurlException.Compose($"Unsupported protocol: {uri.Scheme} (in {ErrorText.SafeUrl(options.Url)}).",
                                "Use an http://, https://, ftp://, ftps:// or file:// URL; other schemes are not implemented by CurlDotNet.",
                                DocLinks.SupportedProtocols)
                        };
                    }
                }
                catch (UriFormatException)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = CurlException.Compose($"Invalid URL: {ErrorText.SafeUrl(options.Url)} is not a valid URL.",
                            "Write it as scheme://host/path (for example https://example.com/a%20b), percent-encode spaces and quote the URL in the command.",
                            DocLinks.Url)
                    };
                }

                return new ValidationResult
                {
                    IsValid = true,
                    ParsedOptions = options
                };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Convert curl command to HttpClient code.
        /// </summary>
        public string ToHttpClientCode(string command)
        {
            var options = _parser.Parse(command);
            var sb = new StringBuilder();
            var method = (options.Method ?? "GET").ToUpperInvariant();

            sb.AppendLine($"// Generated by CurlDotNet from: {command.Trim()}");

            var handlerLines = new List<string>();
            if (options.Credentials != null)
            {
                handlerLines.Add($"    Credentials = new NetworkCredential({CsString(options.Credentials.UserName)}, {CsString(options.Credentials.Password)}),");
            }
            if (!string.IsNullOrEmpty(options.Proxy))
            {
                handlerLines.Add($"    Proxy = new WebProxy({CsString(options.Proxy)}),");
            }
            if (options.Insecure)
            {
                handlerLines.Add("    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator, // curl -k");
            }
            if (options.FollowLocation)
            {
                handlerLines.Add("    AllowAutoRedirect = true, // curl -L");
                handlerLines.Add($"    MaxAutomaticRedirections = {options.MaxRedirects},");
            }

            if (handlerLines.Count > 0)
            {
                sb.AppendLine("var handler = new HttpClientHandler");
                sb.AppendLine("{");
                foreach (var line in handlerLines) sb.AppendLine(line);
                sb.AppendLine("};");
                sb.AppendLine("using var client = new HttpClient(handler);");
            }
            else
            {
                sb.AppendLine("using var client = new HttpClient();");
            }

            if (options.MaxTime.HasValue && options.MaxTime.Value > 0)
            {
                sb.AppendLine($"client.Timeout = TimeSpan.FromSeconds({options.MaxTime.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)});");
            }

            // Request headers go on the client; entity headers (Content-*) belong to the content below
            foreach (var header in options.Headers)
            {
                if (IsContentHeaderName(header.Key) || string.IsNullOrEmpty(header.Value)) continue;
                sb.AppendLine($"client.DefaultRequestHeaders.TryAddWithoutValidation({CsString(header.Key)}, {CsString(header.Value)});");
            }
            if (!string.IsNullOrEmpty(options.UserAgent) && !options.Headers.ContainsKey("User-Agent"))
            {
                sb.AppendLine($"client.DefaultRequestHeaders.TryAddWithoutValidation(\"User-Agent\", {CsString(options.UserAgent)});");
            }
            if (!string.IsNullOrEmpty(options.Cookie))
            {
                sb.AppendLine($"client.DefaultRequestHeaders.TryAddWithoutValidation(\"Cookie\", {CsString(options.Cookie)});");
            }

            var hasContent = false;
            if (options.FormData.Count > 0 || options.Files.Count > 0)
            {
                hasContent = true;
                sb.AppendLine("var content = new MultipartFormDataContent();");
                foreach (var field in options.FormData)
                    sb.AppendLine($"content.Add(new StringContent({CsString(field.Value)}), {CsString(field.Key)});");
                foreach (var file in options.Files)
                    sb.AppendLine($"content.Add(new ByteArrayContent(File.ReadAllBytes({CsString(file.Value)})), {CsString(file.Key)}, {CsString(Path.GetFileName(file.Value))});");
            }
            else if (!string.IsNullOrEmpty(options.Data))
            {
                hasContent = true;
                var contentType = options.Headers.TryGetValue("Content-Type", out var ct) ? ct : "application/x-www-form-urlencoded";
                var mediaType = contentType.Split(';')[0].Trim();
                sb.AppendLine($"var content = new StringContent({CsString(options.Data)}, Encoding.UTF8, {CsString(mediaType)}); // Content-Type: {contentType}");
            }

            var url = CsString(options.Url);
            switch (method)
            {
                case "GET" when !hasContent:
                    sb.AppendLine($"var response = await client.GetAsync({url});");
                    break;
                case "DELETE" when !hasContent:
                    sb.AppendLine($"var response = await client.DeleteAsync({url});");
                    break;
                case "POST":
                    sb.AppendLine($"var response = await client.PostAsync({url}, {(hasContent ? "content" : "null")});");
                    break;
                case "PUT":
                    sb.AppendLine($"var response = await client.PutAsync({url}, {(hasContent ? "content" : "null")});");
                    break;
                case "PATCH":
                    sb.AppendLine($"var response = await client.PatchAsync({url}, {(hasContent ? "content" : "null")});");
                    break;
                default:
                    sb.AppendLine($"var request = new HttpRequestMessage(new HttpMethod(\"{method}\"), {url});");
                    if (hasContent) sb.AppendLine("request.Content = content;");
                    sb.AppendLine("var response = await client.SendAsync(request);");
                    break;
            }

            sb.AppendLine("var body = await response.Content.ReadAsStringAsync();");

            return sb.ToString();
        }

        /// <summary>C# string literal for generated code (verbatim form keeps JSON readable).</summary>
        private static string CsString(string? value)
        {
            value ??= string.Empty;
            if (value.IndexOfAny(new[] { '"', '\\', '\n', '\r', '\t' }) < 0)
            {
                return "\"" + value + "\"";
            }
            return "@\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static bool IsContentHeaderName(string name) =>
            name.StartsWith("Content-", StringComparison.OrdinalIgnoreCase);

        public string ToFetchCode(string command)
        {
            var options = _parser.Parse(command);
            var sb = new StringBuilder();

            sb.AppendLine($"const response = await fetch('{options.Url}', {{");

            if (!string.IsNullOrEmpty(options.Method) && options.Method != "GET")
            {
                sb.AppendLine($"  method: '{options.Method}',");
            }

            if (options.Headers.Any())
            {
                sb.AppendLine("  headers: {");
                foreach (var header in options.Headers)
                {
                    sb.AppendLine($"    '{header.Key}': '{header.Value}',");
                }
                sb.AppendLine("  },");
            }

            if (!string.IsNullOrEmpty(options.Data))
            {
                sb.AppendLine($"  body: JSON.stringify({options.Data}),");
            }

            sb.AppendLine("});");
            sb.AppendLine("const data = await response.json();");

            return sb.ToString();
        }

        /// <summary>
        /// Convert curl command to Python requests.
        /// </summary>
        public string ToPythonCode(string command)
        {
            var options = _parser.Parse(command);
            var sb = new StringBuilder();

            sb.AppendLine("import requests");
            sb.AppendLine();

            var method = (options.Method ?? "GET").ToLowerInvariant();
            var knownMethod = method == "get" || method == "post" || method == "put" || method == "patch" || method == "delete" || method == "head" || method == "options";

            var headers = options.Headers.Where(h => !string.IsNullOrEmpty(h.Value)).ToList();
            if (!string.IsNullOrEmpty(options.UserAgent) && !options.Headers.ContainsKey("User-Agent"))
            {
                headers.Add(new KeyValuePair<string, string>("User-Agent", options.UserAgent!));
            }
            if (!string.IsNullOrEmpty(options.Cookie))
            {
                headers.Add(new KeyValuePair<string, string>("Cookie", options.Cookie!));
            }

            if (headers.Count > 0)
            {
                sb.AppendLine("headers = {");
                foreach (var header in headers)
                {
                    sb.AppendLine($"    {PyString(header.Key)}: {PyString(header.Value)},");
                }
                sb.AppendLine("}");
                sb.AppendLine();
            }

            if (!string.IsNullOrEmpty(options.Proxy))
            {
                sb.AppendLine("proxies = {");
                sb.AppendLine($"    'http': {PyString(options.Proxy)},");
                sb.AppendLine($"    'https': {PyString(options.Proxy)},");
                sb.AppendLine("}");
                sb.AppendLine();
            }

            var args = new List<string> { PyString(options.Url) };
            if (!knownMethod) args.Insert(0, PyString(method.ToUpperInvariant()));
            if (headers.Count > 0) args.Add("headers=headers");
            if (options.Credentials != null)
            {
                args.Add($"auth=({PyString(options.Credentials.UserName)}, {PyString(options.Credentials.Password)})");
            }
            if (!string.IsNullOrEmpty(options.Proxy)) args.Add("proxies=proxies");
            if (options.Insecure) args.Add("verify=False");
            if (options.FollowLocation) args.Add("allow_redirects=True");
            if (options.MaxTime.HasValue && options.MaxTime.Value > 0)
            {
                args.Add($"timeout={options.MaxTime.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}");
            }

            if (options.FormData.Count > 0 || options.Files.Count > 0)
            {
                if (options.FormData.Count > 0)
                {
                    args.Add("data={" + string.Join(", ", options.FormData.Select(f => $"{PyString(f.Key)}: {PyString(f.Value)}")) + "}");
                }
                if (options.Files.Count > 0)
                {
                    args.Add("files={" + string.Join(", ", options.Files.Select(f => $"{PyString(f.Key)}: open({PyString(f.Value)}, 'rb')")) + "}");
                }
            }
            else if (!string.IsNullOrEmpty(options.Data))
            {
                var isJson = options.Headers.TryGetValue("Content-Type", out var ct) && ct.IndexOf("json", StringComparison.OrdinalIgnoreCase) >= 0;
                args.Add(isJson ? $"json={options.Data}" : $"data={PyString(options.Data)}");
            }

            sb.AppendLine($"response = requests.{(knownMethod ? method : "request")}({string.Join(", ", args)})");
            sb.AppendLine("print(response.status_code)");
            sb.AppendLine("print(response.text)");

            return sb.ToString();
        }

        /// <summary>Python string literal for generated code.</summary>
        private static string PyString(string? value)
        {
            value ??= string.Empty;
            return "'" + value.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n") + "'";
        }

        public string ToPowershellCode(string command)
        {
            var options = _parser.Parse(command);
            var sb = new StringBuilder();

            var method = (options.Method ?? "GET").ToUpper();
            sb.Append($"Invoke-RestMethod -Uri \"{options.Url}\" -Method {method}");

            if (options.Headers.Any())
            {
                sb.Append(" -Headers @{");
                var first = true;
                foreach (var header in options.Headers)
                {
                    if (!first) sb.Append("; ");
                    sb.Append($"\"{header.Key}\"=\"{header.Value}\"");
                    first = false;
                }
                sb.Append("}");
            }

            if (!string.IsNullOrEmpty(options.Data))
            {
                // Escape double quotes in data
                var data = options.Data.Replace("\"", "`\"");
                sb.Append($" -Body \"{data}\"");

                // If it looks like JSON, add ContentType
                if (options.Data.TrimStart().StartsWith("{"))
                {
                    sb.Append(" -ContentType \"application/json\"");
                }
            }

            return sb.ToString();
        }

        internal static void ApplySettings(CurlOptions options, CurlSettings settings)
        {
            if (settings.MaxTimeSeconds.HasValue)
            {
                options.MaxTime = settings.MaxTimeSeconds.Value;
            }

            if (settings.ConnectTimeoutSeconds.HasValue)
            {
                options.ConnectTimeout = settings.ConnectTimeoutSeconds.Value;
            }

            if (settings.FollowRedirects.HasValue)
            {
                options.FollowLocation = settings.FollowRedirects.Value;
            }

            if (settings.Insecure.HasValue)
            {
                options.Insecure = settings.Insecure.Value;
            }

            if (settings.Headers != null)
            {
                foreach (var header in settings.Headers)
                {
                    options.Headers[header.Key] = header.Value;
                }
            }

            if (settings.OnProgress != null)
            {
                options.ProgressHandler = settings.OnProgress;
            }
            if (settings.RetryCount > 0)
            {
                // WithRetries() is the .NET convenience: retry any failure, with a fixed delay
                options.Retry = settings.RetryCount;
                options.RetryDelay = settings.RetryDelayMs / 1000.0;
                options.RetryAllErrors = true;
            }
        }

        private CurlResult CreateTimeoutResult(CurlOptions options)
        {
            return new CurlResult
            {
                StatusCode = 408,
                Body = $"Operation timed out after {GetTimeoutSeconds(options)} seconds.",
                Command = options.OriginalCommand
            };
        }

        private CurlResult CreateCancelledResult(CurlOptions options)
        {
            return new CurlResult
            {
                StatusCode = 499,
                Body = "Operation cancelled by caller.",
                Command = options.OriginalCommand
            };
        }

        private static double GetTimeoutSeconds(CurlOptions options)
        {
            // Like curl, no --max-time means no transfer timeout: the HttpClients CurlDotNet creates use Timeout.InfiniteTimeSpan
            var timeout = options.MaxTime ?? Curl.DefaultMaxTimeSeconds;
            return timeout > 0 ? timeout : 0;
        }

        /// <summary>
        /// Creates an HttpClient configured for curl-like behavior.
        /// </summary>
        private static HttpClient CreateDefaultHttpClient()
        {
            // No automatic redirects (RedirectHandler follows them like curl), no automatic
            // decompression (--compressed is decoded by HttpHandler), no cookie container (curl only
            // engages its cookie engine for -b FILE / -c, issue #41), no implicit timeout
            // (--max-time / Curl.DefaultMaxTimeSeconds implement it), and on .NET 5+ a recording
            // connection layer so headers and exit codes match curl byte for byte.
            return CurlTransport.CreateSharedClient();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Don't dispose the HttpClient if it was provided externally
                    // Only dispose handlers
                    foreach (var handler in _handlers.Values)
                    {
                        if (handler is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                    }
                }
                _disposed = true;
            }
        }
    }

    /// <summary>
    /// What <see cref="Curl.Validate(string)"/> found when it parsed a curl command without sending it:
    /// whether it would run (<see cref="IsValid"/>), why not (<see cref="Error"/>) and the parsed options.
    /// </summary>
    /// <remarks>
    /// <para>Validation parses the command exactly as <see cref="Curl.ExecuteAsync(string)"/> would (same shell
    /// flavor rules), then checks that there is a URL, that every option is known and that the URL's scheme is
    /// one CurlDotNet implements (http, https, ftp, ftps, file). Nothing is sent over the network, so a
    /// valid command can still fail at run time (unknown host, refused connection, HTTP errors).</para>
    /// <para>Example: <c>var check = Curl.Validate(userInput); if (!check.IsValid) { ShowError(check.Error); return; }</c>
    /// then <c>var result = await Curl.ExecuteAsync(userInput);</c>.</para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="Curl.Validate(string)"/>
    /// <seealso cref="Curl.Validate(string, CurlFlavor)"/>
    /// <seealso cref="CurlOptions"/>
    public class ValidationResult
    {
        /// <summary>
        /// True when the command parsed, has a URL, uses only known options and a supported scheme, so
        /// executing it would at least start a transfer.
        /// </summary>
        /// <value><c>true</c> for a runnable command; <c>false</c> with <see cref="Error"/> set otherwise.</value>
        /// <remarks>
        /// <para>Example: <c>Curl.Validate("curl https://example.com").IsValid</c> is <c>true</c>;
        /// <c>Curl.Validate("curl --no-such-flag https://example.com").IsValid</c> is <c>false</c>.</para>
        /// </remarks>
        /// <seealso cref="Error"/>
        public bool IsValid { get; set; }

        /// <summary>
        /// Why the command is not valid, worded for the person who wrote it; null when <see cref="IsValid"/> is true.
        /// </summary>
        /// <value>One of: <c>No URL specified</c>; <c>Unknown option: --a, --b</c> (every unknown option, comma
        /// separated); <c>Unsupported protocol: gopher</c>; <c>Invalid URL: ...</c>; or, when parsing itself
        /// failed (an unbalanced quote, an option without its value), the parser's exception message, which
        /// spans several lines: what is wrong, how to fix it and a <c>See https://...</c> documentation link.</value>
        /// <remarks>
        /// <para>Show it as-is to the user, or take the first line for a short status bar.</para>
        /// <para>Example: <c>Curl.Validate("curl -H").Error</c> explains that <c>-H</c> needs a header value.</para>
        /// </remarks>
        /// <seealso cref="IsValid"/>
        public string? Error { get; set; }

        /// <summary>
        /// The options the command parsed into (URL, method, headers, data, flags), for inspection or for
        /// running later with <see cref="Curl.ExecuteAsync(CurlOptions)"/>.
        /// </summary>
        /// <value>Set when the command is valid and also when its only problem is unknown options; null when
        /// there is no URL, the scheme is unsupported, the URL is malformed or parsing failed.</value>
        /// <remarks>
        /// <para>Example: <c>var opts = Curl.Validate("curl -X POST -d a=1 https://example.com").ParsedOptions;</c>
        /// gives <c>opts.Method == "POST"</c> and <c>opts.Data == "a=1"</c>.</para>
        /// </remarks>
        /// <seealso cref="CurlOptions"/>
        /// <seealso cref="Curl.ExecuteAsync(CurlOptions)"/>
        public CurlOptions? ParsedOptions { get; set; }
    }
}
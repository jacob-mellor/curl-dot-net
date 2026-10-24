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
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
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
                throw new ArgumentException("Command cannot be null or empty", nameof(command));
            }

            var options = _parser.Parse(command);
            return await ExecuteAsync(options, cancellationToken);
        }

        /// <summary>
        /// Execute a curl command with custom settings.
        /// </summary>
        public async Task<CurlResult> ExecuteAsync(string command, CurlSettings settings)
        {
            var options = _parser.Parse(command);

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
                throw new ArgumentNullException(nameof(options));

            if (_disposed)
                throw new ObjectDisposedException(nameof(CurlEngine));

            if (string.IsNullOrEmpty(options.Url))
                throw new CurlException("No URL specified", 3);

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
                    throw new CurlUnsupportedProtocolException($"Unsupported protocol: {protocol}");
                }

                // Execute with appropriate handler
                var result = await handler.ExecuteAsync(options, cancellationToken);
                result.Command = options.OriginalCommand;

                return result;
            }
            catch (UriFormatException)
            {
                throw new CurlMalformedUrlException($"Invalid URL: {options.Url}");
            }
            catch (CurlAbortedByCallbackException ex) when (cancellationToken.IsCancellationRequested)
            {
                // Standard .NET cancellation contract at the public boundary: catch (OperationCanceledException) just works
                throw new OperationCanceledException("The curl request was cancelled.", ex, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new OperationCanceledException("The curl request was cancelled.", cancellationToken);
                }
                var timeoutSeconds = GetTimeoutSeconds(options);
                throw new CurlTimeoutException(
                    $"Operation timed out after {timeoutSeconds} seconds",
                    options.OriginalCommand,
                    TimeSpan.FromSeconds(timeoutSeconds));
            }
            catch (HttpRequestException)
            {
                var uri = new Uri(options.Url);
                throw new CurlCouldntConnectException(uri.Host, uri.Port > 0 ? uri.Port : (uri.Scheme == "https" ? 443 : 80), options.OriginalCommand);
            }
        }

        /// <summary>
        /// Validate a curl command without executing it.
        /// </summary>
        public ValidationResult Validate(string command)
        {
            try
            {
                var options = _parser.Parse(command);

                if (string.IsNullOrEmpty(options.Url))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = "No URL specified"
                    };
                }

                if (options.UnknownOptions.Count > 0)
                {
                    // curl exits with code 2 here; Validate() is the place to surface the typo
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = $"Unknown option: {string.Join(", ", options.UnknownOptions)}",
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
                            Error = $"Unsupported protocol: {uri.Scheme}"
                        };
                    }
                }
                catch (UriFormatException)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Error = $"Invalid URL: {options.Url}"
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

        private void ApplySettings(CurlOptions options, CurlSettings settings)
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
            // Like curl, no --max-time means no transfer timeout (HttpClient's own 100 s cap still applies)
            var timeout = options.MaxTime ?? Curl.DefaultMaxTimeSeconds;
            return timeout > 0 ? timeout : 0;
        }

        /// <summary>
        /// Creates an HttpClient configured for curl-like behavior.
        /// </summary>
        private static HttpClient CreateDefaultHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false, // We handle redirects manually like curl
                // curl only sends Accept-Encoding with --compressed; HttpHandler decodes the body itself
                AutomaticDecompression = System.Net.DecompressionMethods.None,
                // curl only engages a cookie engine when -b/-c is given; with UseCookies=true
                // the shared container would swallow manual Cookie headers and leak session
                // cookies across unrelated requests (issue #41)
                UseCookies = false
            };
            // curl has no implicit transfer timeout; --max-time / Curl.DefaultMaxTimeSeconds implement it
            return new HttpClient(handler) { Timeout = Timeout.InfiniteTimeSpan };
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
    /// Result of command validation.
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Whether the validation succeeded.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Error message if validation failed, null if valid.
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Parsed options if validation succeeded, null if invalid.
        /// </summary>
        public CurlOptions? ParsedOptions { get; set; }
    }
}
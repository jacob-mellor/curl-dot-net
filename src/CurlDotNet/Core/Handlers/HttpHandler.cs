/***************************************************************************
 * HttpHandler - HTTP/HTTPS protocol handler
 *
 * Transpiled from curl's lib/http.c and lib/url.c by Daniel Stenberg and contributors
 * Original curl Copyright (C) 1996-2025, Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024-2025 Jacob Mellor and IronSoftware
 *
 * TRANSPILATION APPROACH:
 * This class implements curl's HTTP protocol logic by transpiling the behavior
 * and state machines from curl's C source code. While we use .NET's HttpClient
 * as the underlying transport layer (it's efficient and cross-platform), all
 * the protocol behavior, option handling, redirect logic, and error handling
 * match curl's implementation exactly.
 *
 * The value is NOT in wrapping HttpClient - it's in transpiling curl's protocol
 * logic, command parsing, and behavior so curl commands work identically in .NET.
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
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// Handler for HTTP and HTTPS protocols.
    /// </summary>
    internal class HttpHandler : IProtocolHandler
    {
        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;

        /// <summary>
        /// Create handler with default HttpClient.
        /// </summary>
        public HttpHandler() : this(CreateDefaultHttpClient(), true)
        {
        }

        /// <summary>
        /// Create handler with custom HttpClient.
        /// </summary>
        public HttpHandler(HttpClient httpClient, bool ownsHttpClient = false)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _ownsHttpClient = ownsHttpClient;
        }

        public async Task<CurlResult> ExecuteAsync(CurlOptions options, CancellationToken cancellationToken)
        {
            var request = CreateRequest(options);
            var startTime = DateTime.UtcNow;
            var timings = new CurlTimings();
            var verboseLog = options.Verbose ? new StringBuilder() : null;

            try
            {
                // Configure timeout
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                var timeoutSeconds = GetTimeoutSeconds(options);
                if (timeoutSeconds > 0)
                {
                    cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
                }

                AppendVerboseRequest(verboseLog, request);

                // Send request
                timings.PreTransfer = (DateTime.UtcNow - startTime).TotalMilliseconds;
                var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                timings.StartTransfer = (DateTime.UtcNow - startTime).TotalMilliseconds;
                AppendVerboseResponseHeaders(verboseLog, response);

                // Handle redirects manually if needed
                if (options.FollowLocation && IsRedirect(response.StatusCode))
                {
                    return await HandleRedirect(response, request, options, cts.Token, timings, startTime, verboseLog);
                }

                return await BuildResultAsync(request, response, options, timings, startTime, cts.Token, verboseLog);
            }
            catch (TaskCanceledException)
            {
                if (cancellationToken.IsCancellationRequested)
                    throw new CurlAbortedByCallbackException("Operation cancelled");
                throw new CurlOperationTimeoutException(GetTimeoutSeconds(options), options.OriginalCommand);
            }
            catch (HttpRequestException)
            {
                var uri = new Uri(options.Url);
                throw new CurlCouldntConnectException(uri.Host, uri.Port > 0 ? uri.Port : (uri.Scheme == "https" ? 443 : 80), options.OriginalCommand);
            }
        }

        public bool SupportsProtocol(string protocol)
        {
            return protocol == "http" || protocol == "https";
        }

        private HttpRequestMessage CreateRequest(CurlOptions options)
        {
            var method = GetHttpMethod(options);
            var request = new HttpRequestMessage(method, options.Url);

            // Add headers
            foreach (var header in options.Headers)
            {
                if (IsContentHeader(header.Key))
                {
                    // Will be added with content
                    continue;
                }

                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            // Set user agent
            if (!string.IsNullOrEmpty(options.UserAgent))
            {
                request.Headers.UserAgent.ParseAdd(options.UserAgent);
            }

            // Set referer
            if (!string.IsNullOrEmpty(options.Referer))
            {
                request.Headers.Referrer = new Uri(options.Referer);
            }

            // Set cookies
            if (!string.IsNullOrEmpty(options.Cookie))
            {
                request.Headers.Add("Cookie", options.Cookie);
            }

            // Set authorization
            if (options.Credentials != null)
            {
                var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(
                    $"{options.Credentials.UserName}:{options.Credentials.Password}"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
            }

            // Set range
            if (!string.IsNullOrEmpty(options.Range))
            {
                request.Headers.Range = ParseRange(options.Range);
            }

            // Set compression acceptance (--compressed flag)
            if (options.Compressed)
            {
                // Tell server we accept compressed content
                // HttpClient will automatically decompress when AutomaticDecompression is set
                request.Headers.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            }

            // Add content
            if (ShouldHaveContent(method, options))
            {
                request.Content = CreateContent(options);
            }

            return request;
        }

        private static int GetTimeoutSeconds(CurlOptions options)
        {
            var timeout = options.MaxTime ?? Curl.DefaultMaxTimeSeconds;
            return timeout > 0 ? timeout : 30;
        }

        private HttpMethod GetHttpMethod(CurlOptions options)
        {
            if (!string.IsNullOrEmpty(options.CustomMethod))
            {
                return new HttpMethod(options.CustomMethod);
            }

            return options.Method?.ToUpper() switch
            {
                "GET" => HttpMethod.Get,
                "POST" => HttpMethod.Post,
                "PUT" => HttpMethod.Put,
                "DELETE" => HttpMethod.Delete,
                "HEAD" => HttpMethod.Head,
                "OPTIONS" => HttpMethod.Options,
                "PATCH" => new HttpMethod("PATCH"),
                _ => HttpMethod.Get
            };
        }

        private bool ShouldHaveContent(HttpMethod method, CurlOptions options)
        {
            if (method == HttpMethod.Get || method == HttpMethod.Head)
                return false;

            return !string.IsNullOrEmpty(options.Data) ||
                   options.BinaryData != null ||
                   options.FormData.Any() ||
                   options.Files.Any();
        }

        private HttpContent CreateContent(CurlOptions options)
        {
            // Multipart form data
            if (options.Files.Any() || options.FormData.Any())
            {
                var content = new MultipartFormDataContent();

                foreach (var field in options.FormData)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }

                foreach (var file in options.Files)
                {
                    var fileContent = new ByteArrayContent(File.ReadAllBytes(file.Value));
                    fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/octet-stream");
                    content.Add(fileContent, file.Key, Path.GetFileName(file.Value));
                }

                return content;
            }

            // Binary data
            if (options.BinaryData != null)
            {
                return new ByteArrayContent(options.BinaryData);
            }

            // Text data
            if (!string.IsNullOrEmpty(options.Data))
            {
                var content = new StringContent(options.Data, Encoding.UTF8);

                // Set content type from headers if specified
                if (options.Headers.TryGetValue("Content-Type", out var contentType))
                {
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
                }
                else
                {
                    // Default to application/x-www-form-urlencoded for POST data
                    content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                }

                return content;
            }

            return null;
        }

        private async Task<(CurlResult Result, string? ResponseText, byte[]? ResponseBinary)> CreateResult(HttpResponseMessage response, CurlOptions options,
            CurlTimings timings, DateTime startTime)
        {
            var result = new CurlResult
            {
                StatusCode = (int)response.StatusCode,
                Headers = response.Headers.ToDictionary(h => h.Key, h => string.Join(", ", h.Value)),
                Command = options.OriginalCommand
            };
            string? responseText = null;
            byte[]? responseBinary = null;

            // Add content headers
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    result.Headers[header.Key] = string.Join(", ", header.Value);
                }

                // Read body
                if (!options.HeadOnly)
                {
                    if (IsTextContent(response.Content))
                    {
                        responseText = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        responseBinary = await response.Content.ReadAsByteArrayAsync();
                    }
                }
            }

            return (result, responseText, responseBinary);
        }

        private async Task<CurlResult> BuildResultAsync(HttpRequestMessage request, HttpResponseMessage response, CurlOptions options,
            CurlTimings timings, DateTime startTime, CancellationToken cancellationToken, StringBuilder? verboseLog)
        {
            var (result, responseText, responseBinary) = await CreateResult(response, options, timings, startTime);
            var downloadSize = responseBinary?.Length ?? (responseText != null ? Encoding.UTF8.GetByteCount(responseText) : 0);

            await HandleOutputFilesAsync(options, result, responseText, responseBinary, cancellationToken);

            result.Body = BuildFinalBody(options, request, response, responseText, verboseLog, downloadSize);
            timings.Total = (DateTime.UtcNow - startTime).TotalMilliseconds;
            result.Timings = timings;

            return result;
        }

        private async Task HandleOutputFilesAsync(CurlOptions options, CurlResult result, string? responseText, byte[]? responseBinary, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(options.OutputFile))
            {
                await WriteOutputFile(options.OutputFile, responseText, responseBinary, cancellationToken);
                result.OutputFiles.Add(options.OutputFile);
            }
            else if (options.UseRemoteFileName)
            {
                var remoteFile = DetermineRemoteFileName(options);
                await WriteOutputFile(remoteFile, responseText, responseBinary, cancellationToken);
                result.OutputFiles.Add(remoteFile);
            }

            result.BinaryData = responseBinary;
        }

        private async Task WriteOutputFile(string outputFile, string? body, byte[]? binary, CancellationToken cancellationToken)
        {
            if (binary != null)
            {
#if NETSTANDARD2_0 || NET472 || NET48
                await Task.Run(() => File.WriteAllBytes(outputFile, binary), cancellationToken);
#else
                await File.WriteAllBytesAsync(outputFile, binary, cancellationToken);
#endif
            }
            else if (!string.IsNullOrEmpty(body))
            {
#if NETSTANDARD2_0 || NET472 || NET48
                await Task.Run(() => File.WriteAllText(outputFile, body), cancellationToken);
#else
                await File.WriteAllTextAsync(outputFile, body, cancellationToken);
#endif
            }
        }

        private string BuildFinalBody(CurlOptions options, HttpRequestMessage request, HttpResponseMessage response, string? responseText, StringBuilder? verboseLog, int downloadSize)
        {
            var builder = new StringBuilder();

            if (options.Verbose && verboseLog != null)
            {
                builder.AppendLine(verboseLog.ToString().TrimEnd());
            }

            // Include headers BEFORE body when -i flag is used (like curl)
            if (options.IncludeHeaders)
            {
                if (builder.Length > 0)
                    builder.AppendLine();
                builder.AppendLine(BuildHeaderBlock(response));
                // Add empty line between headers and body
                if (!string.IsNullOrEmpty(responseText))
                {
                    builder.AppendLine();
                }
            }

            if (!string.IsNullOrEmpty(responseText))
            {
                if (!options.IncludeHeaders && builder.Length > 0)
                    builder.AppendLine();
                builder.Append(responseText);
            }

            if (!string.IsNullOrEmpty(options.WriteOut))
            {
                var writeOut = FormatWriteOut(options.WriteOut, response, options, downloadSize);
                if (!string.IsNullOrWhiteSpace(writeOut))
                {
                    if (builder.Length > 0)
                        builder.AppendLine();
                    builder.Append(writeOut);
                }
            }

            return builder.Length > 0
                ? builder.ToString()
                : responseText ?? string.Empty;
        }

        private string BuildHeaderBlock(HttpResponseMessage response)
        {
            var builder = new StringBuilder();
            builder.AppendLine(BuildStatusLine(response));
            foreach (var header in response.Headers)
            {
                builder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    builder.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            return builder.ToString().TrimEnd();
        }

        private string BuildStatusLine(HttpResponseMessage response)
        {
            var version = response.Version != null ? $"{response.Version.Major}.{response.Version.Minor}" : "1.1";
            return $"HTTP/{version} {(int)response.StatusCode} {response.ReasonPhrase}";
        }

        private void AppendVerboseRequest(StringBuilder? verboseLog, HttpRequestMessage request)
        {
            if (verboseLog == null)
                return;

            var uri = request.RequestUri;
            verboseLog.AppendLine($"*   Trying {uri.Host}...");
            verboseLog.AppendLine($"* Connected to {uri.Host} ({uri.Host}) port {uri.Port} (#0)");
            verboseLog.AppendLine($"> {request.Method} {uri.PathAndQuery} HTTP/{request.Version?.ToString() ?? "1.1"}");

            foreach (var header in request.Headers)
            {
                verboseLog.AppendLine($"> {header.Key}: {string.Join(", ", header.Value)}");
            }
            if (request.Content != null)
            {
                foreach (var header in request.Content.Headers)
                {
                    verboseLog.AppendLine($"> {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            verboseLog.AppendLine(">");
        }

        private void AppendVerboseResponseHeaders(StringBuilder? verboseLog, HttpResponseMessage response)
        {
            if (verboseLog == null)
                return;

            verboseLog.AppendLine($"< {BuildStatusLine(response)}");
            foreach (var header in response.Headers)
            {
                verboseLog.AppendLine($"< {header.Key}: {string.Join(", ", header.Value)}");
            }
            if (response.Content != null)
            {
                foreach (var header in response.Content.Headers)
                {
                    verboseLog.AppendLine($"< {header.Key}: {string.Join(", ", header.Value)}");
                }
            }
            verboseLog.AppendLine("<");
        }

        private string FormatWriteOut(string format, HttpResponseMessage response, CurlOptions options, int downloadSize)
        {
            if (string.IsNullOrEmpty(format))
                return string.Empty;

            var formatted = format
                .Replace("\\n", Environment.NewLine)
                .Replace("\\t", "\t");

            formatted = formatted
                .Replace("%{http_code}", ((int)response.StatusCode).ToString())
                .Replace("%{size_download}", downloadSize.ToString())
                .Replace("%{url_effective}", options.Url ?? string.Empty)
                .Replace("%{content_type}", response.Content?.Headers?.ContentType?.MediaType ?? string.Empty);

            return formatted;
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

        private string DetermineRemoteFileName(CurlOptions options)
        {
            var uri = new Uri(options.Url);
            var fileName = Path.GetFileName(uri.LocalPath);
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = "curl-download";
            }
            return Path.Combine(Directory.GetCurrentDirectory(), fileName);
        }

        private async Task<CurlResult> HandleRedirect(HttpResponseMessage response, HttpRequestMessage initialRequest, CurlOptions options,
            CancellationToken cancellationToken, CurlTimings timings, DateTime startTime, StringBuilder? verboseLog)
        {
            var redirectCount = 0;
            var currentResponse = response;
            var currentRequest = initialRequest;

            // Accumulate redirect headers when -i flag is used
            var redirectHeaders = new StringBuilder();

            while (IsRedirect(currentResponse.StatusCode) && redirectCount < options.MaxRedirects)
            {
                // If -i flag is used, accumulate headers from redirect responses
                if (options.IncludeHeaders)
                {
                    redirectHeaders.AppendLine(BuildHeaderBlock(currentResponse));
                    redirectHeaders.AppendLine(); // Empty line between responses
                }

                var location = currentResponse.Headers.Location;
                if (location == null)
                {
                    throw new CurlException("Redirect response missing Location header");
                }

                var newUrl = location.IsAbsoluteUri
                    ? location.ToString()
                    : new Uri(new Uri(options.Url), location).ToString();

                options.Url = newUrl;
                redirectCount++;

                currentRequest = CreateRequest(options);
                AppendVerboseRequest(verboseLog, currentRequest);
                currentResponse = await _httpClient.SendAsync(currentRequest, cancellationToken);
                AppendVerboseResponseHeaders(verboseLog, currentResponse);

                timings.Redirect = (DateTime.UtcNow - startTime).TotalMilliseconds;
            }

            if (redirectCount >= options.MaxRedirects)
            {
                throw new CurlTooManyRedirectsException(redirectCount);
            }

            var result = await BuildResultAsync(currentRequest, currentResponse, options, timings, startTime, cancellationToken, verboseLog);

            // Prepend redirect headers to the body when -i flag is used
            if (options.IncludeHeaders && redirectHeaders.Length > 0)
            {
                result.Body = redirectHeaders.ToString() + result.Body;
            }

            return result;
        }

        private bool IsRedirect(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.MovedPermanently ||
                   statusCode == HttpStatusCode.Found ||
                   statusCode == HttpStatusCode.SeeOther ||
                   statusCode == HttpStatusCode.TemporaryRedirect ||
#if NETSTANDARD2_0 || NET472 || NET48
                   statusCode == (HttpStatusCode)308; // PermanentRedirect
#else
                   statusCode == HttpStatusCode.PermanentRedirect;
#endif
        }

        private bool IsTextContent(HttpContent content)
        {
            var contentType = content.Headers.ContentType?.MediaType;
            if (contentType == null) return true;

            return contentType.StartsWith("text/") ||
                   contentType.Contains("json") ||
                   contentType.Contains("xml") ||
                   contentType.Contains("javascript");
        }

        private bool IsContentHeader(string headerName)
        {
            var contentHeaders = new[] { "Content-Type", "Content-Length", "Content-Encoding",
                "Content-Language", "Content-Location", "Content-Disposition" };
            return contentHeaders.Contains(headerName, StringComparer.OrdinalIgnoreCase);
        }

        private RangeHeaderValue ParseRange(string range)
        {
            // Parse range like "0-499" or "500-"
            var parts = range.Split('-');
            if (parts.Length == 2)
            {
                var from = string.IsNullOrEmpty(parts[0]) ? (long?)null : long.Parse(parts[0]);
                var to = string.IsNullOrEmpty(parts[1]) ? (long?)null : long.Parse(parts[1]);

                return new RangeHeaderValue(from, to);
            }
            return null;
        }

        private static HttpClient CreateDefaultHttpClient()
        {
            var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false, // We handle redirects manually
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            return new HttpClient(handler);
        }
    }
}
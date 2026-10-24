using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using CurlDotNet.Core;
using CurlDotNet.Exceptions;

namespace CurlDotNet
{
    /// <summary>
    /// Extra <see cref="CurlResult"/> helpers in the root <c>CurlDotNet</c> namespace: JSON parsing with your own
    /// <see cref="JsonSerializerOptions"/>, a non-throwing <c>TryParseJson</c>, <c>EnsureSuccessStatusCode</c> and a
    /// (success, body, error) tuple.
    /// </summary>
    /// <remarks>
    /// <para>These are extension methods, so they appear on any <see cref="CurlResult"/> once <c>using CurlDotNet;</c> is in
    /// scope. Where <see cref="CurlResult"/> has an instance method of the same name and parameters (<c>ParseJson&lt;T&gt;()</c>,
    /// <c>SaveToFile(path)</c>, <c>GetHeader(name)</c>), C# calls the instance method; the extension version runs only when you
    /// pass the extra argument (<c>ParseJson&lt;T&gt;(options)</c>) or call it statically
    /// (<c>ErgonomicExtensions.SaveToFile(result, path)</c>).</para>
    /// <para>The JSON helpers here always use System.Text.Json, on every target framework (including .NET Standard 2.0, where
    /// <see cref="CurlResult.ParseJson{T}"/> uses Newtonsoft.Json).</para>
    /// <para>Example: <c>var user = result.ParseJson&lt;User&gt;(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });</c></para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlResult"/>
    /// <seealso cref="CurlApiClient"/>
    public static class ErgonomicExtensions
    {
        /// <summary>Case-insensitive property matching: camelCase JSON binds to PascalCase C# properties.</summary>
        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private static void RequireResult(CurlResult result, string method)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result), CurlException.Compose(
                    $"{method}() was called on a null CurlResult.",
                    "Make sure the request ran and its result was assigned (await Curl.ExecuteAsync(...)) before calling " + method + "().",
                    DocLinks.HowToRunCommand));
            }
        }

        private static string Where(CurlResult result) => ErrorText.UrlFromCommand(result.Command) ?? "the request";

        /// <summary>
        /// Deserializes <see cref="CurlResult.Body"/> into a <typeparamref name="T"/> with System.Text.Json and the
        /// <paramref name="options"/> you choose, refusing non-2xx responses and empty bodies.
        /// </summary>
        /// <remarks>
        /// <para>Called as <c>result.ParseJson&lt;T&gt;(options)</c>. Without an argument, <c>result.ParseJson&lt;T&gt;()</c> binds
        /// to <see cref="CurlResult.ParseJson{T}"/> instead, which does not check the status. When <paramref name="options"/>
        /// is null, <c>PropertyNameCaseInsensitive = true</c> is used.</para>
        /// <para>Example: <c>var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } };</c>
        /// then <c>var order = result.ParseJson&lt;Order&gt;(opts);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <typeparam name="T">A class, struct or collection that System.Text.Json can create, such as <c>User</c> or <c>List&lt;User&gt;</c>.</typeparam>
        /// <param name="result">The response to parse; must not be null.</param>
        /// <param name="options">System.Text.Json options (naming policy, converters, case sensitivity), or null for
        /// case-insensitive defaults.</param>
        /// <returns>The deserialized <typeparamref name="T"/>; never null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <exception cref="CurlParsingException">The status is not 2xx, the body is empty, the body is not valid JSON for
        /// <typeparamref name="T"/>, or the JSON is the literal <c>null</c>. The message names the URL, status and the start of
        /// the body; for invalid JSON the <see cref="JsonException"/> is the InnerException. Fix: check
        /// <see cref="CurlResult.StatusCode"/> and print <see cref="CurlResult.Body"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</exception>
        /// <seealso cref="TryParseJson{T}(CurlResult, out T, JsonSerializerOptions)"/>
        /// <seealso cref="CurlResult.ParseJson{T}"/>
        /// <seealso cref="CurlResult.AsJsonDynamic"/>
        public static T ParseJson<T>(this CurlResult result, JsonSerializerOptions? options = null)
        {
            RequireResult(result, nameof(ParseJson));
            var contentType = result.GetHeader("Content-Type");
            if (!result.IsSuccess)
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{typeof(T).Name}>(options) refused to parse the response of {Where(result)}: it answered HTTP {result.StatusCode}, not 2xx (body starts with \"{ErrorText.Preview(result.Body, 60)}\").",
                    result.StatusFix(result.StatusCode),
                    DocLinks.HowToErrorHandling),
                    contentType, typeof(T), result.Command);
            }

            if (string.IsNullOrWhiteSpace(result.Body))
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{typeof(T).Name}>(options) found no JSON to parse: the response body of {Where(result)} is empty (HTTP {result.StatusCode}).",
                    result.IsBinary
                        ? $"The response is binary ({result.BinaryData.Length} bytes in BinaryData), not JSON; request the JSON endpoint or save the bytes with SaveToFile."
                        : "Check that the endpoint returns a body (204 and HEAD do not) and that the command did not send it to a file with -o.",
                    DocLinks.HowToParseJson),
                    contentType, typeof(T), result.Command);
            }

            T value;
            try
            {
                value = JsonSerializer.Deserialize<T>(result.Body, options ?? DefaultJsonOptions);
            }
            catch (JsonException ex)
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{typeof(T).Name}>(options) could not read the response body of {Where(result)} as {typeof(T).Name}: {ex.Message} (Content-Type: {contentType ?? "none"}, body starts with \"{ErrorText.Preview(result.Body, 60)}\").",
                    $"Print result.Body to see what arrived; make sure {typeof(T).Name} matches the JSON (an array needs List<{typeof(T).Name}>) and that the options' naming policy matches the property names.",
                    DocLinks.HowToParseJson),
                    contentType, typeof(T), result.Command, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{typeof(T).Name}>(options) cannot create a {typeof(T).FullName} from JSON: {ex.Message}",
                    "Use a concrete class with a public parameterless constructor and settable properties, or add a JsonConverter for the type to the options.",
                    DocLinks.HowToParseJson),
                    contentType, typeof(T), result.Command, ex);
            }

            if (value == null)
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{typeof(T).Name}>(options): the response body of {Where(result)} is the JSON literal null, so there is no {typeof(T).Name} to return.",
                    "Treat a null body as \"no data\" in your code (use TryParseJson, which returns false), or ask the API why it returned null.",
                    DocLinks.HowToParseJson),
                    contentType, typeof(T), result.Command);
            }
            return value;
        }

        /// <summary>
        /// Tries to deserialize <see cref="CurlResult.Body"/> into a <typeparamref name="T"/>; returns false instead of throwing
        /// when the status is not 2xx, the body is empty, or the JSON does not fit.
        /// </summary>
        /// <remarks>
        /// <para>Uses System.Text.Json with <paramref name="options"/> (or case-insensitive defaults). Parsing errors of any
        /// kind are swallowed and reported as false; the JSON literal <c>null</c> also returns false. Use
        /// <see cref="ParseJson{T}(CurlResult, JsonSerializerOptions)"/> when you want to know why parsing failed.</para>
        /// <para>Example: <c>if (result.TryParseJson&lt;ErrorBody&gt;(out var err)) Console.WriteLine(err.Message); else Console.WriteLine(result.Body);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <typeparam name="T">A reference type System.Text.Json can create, such as <c>User</c> or <c>List&lt;User&gt;</c>.</typeparam>
        /// <param name="result">The response to parse; must not be null.</param>
        /// <param name="value">The parsed object when the method returns true; null otherwise.</param>
        /// <param name="options">System.Text.Json options, or null for case-insensitive defaults.</param>
        /// <returns>true when the status is 2xx, the body is non-empty and parsed into a non-null <typeparamref name="T"/>; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <seealso cref="ParseJson{T}(CurlResult, JsonSerializerOptions)"/>
        /// <seealso cref="CurlResult.ParseJson{T}"/>
        public static bool TryParseJson<T>(this CurlResult result, out T value, JsonSerializerOptions? options = null) where T : class
        {
            RequireResult(result, nameof(TryParseJson));
            value = default(T);

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.Body))
            {
                return false;
            }

            try
            {
                value = JsonSerializer.Deserialize<T>(result.Body, options ?? DefaultJsonOptions);
                return value != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the response (bytes of <see cref="CurlResult.BinaryData"/>, else the text of <see cref="CurlResult.Body"/>) to
        /// <paramref name="filePath"/> after checking it is a 2xx response with content, and returns how much was written.
        /// </summary>
        /// <remarks>
        /// <para>Normal call syntax <c>result.SaveToFile(path)</c> binds to the instance method
        /// <see cref="CurlResult.SaveToFile(string)"/> (no status check, returns the result). Call this one statically:
        /// <c>long written = ErgonomicExtensions.SaveToFile(result, "out.bin");</c></para>
        /// <para>Overwrites an existing file; text is written as UTF-8; the folder must exist.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="result">The response to save; must not be null.</param>
        /// <param name="filePath">Relative or absolute path of the file to create or overwrite, such as <c>"report.pdf"</c>.</param>
        /// <returns>For binary data, the number of bytes written; for text, the number of characters in
        /// <see cref="CurlResult.Body"/> (the UTF-8 file can be larger for non-ASCII text).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <exception cref="InvalidOperationException">The status is not 2xx, or there is no body and no binary data. The
        /// message names the URL and status and says how to fix it.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (folder missing, access denied, file locked,
        /// disk full, invalid name); exit code 23. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="CurlResult.SaveToFile(string)"/>
        /// <seealso cref="CurlResult.SaveToFileAsync(string)"/>
        public static long SaveToFile(this CurlResult result, string filePath)
        {
            RequireResult(result, nameof(SaveToFile));
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(CurlException.Compose(
                    $"Cannot save failed response: {Where(result)} answered HTTP {result.StatusCode}, and ErgonomicExtensions.SaveToFile only saves 2xx responses.",
                    "Fix the request first (" + result.StatusFix(result.StatusCode) + ") or, to keep the error page anyway, call result.SaveToFile(path) (the instance method), which saves any status.",
                    DocLinks.HowToDownload));
            }

            try
            {
                if (result.BinaryData != null && result.BinaryData.Length > 0)
                {
                    File.WriteAllBytes(filePath, result.BinaryData);
                    return result.BinaryData.Length;
                }
                else if (!string.IsNullOrEmpty(result.Body))
                {
                    File.WriteAllText(filePath, result.Body);
                    return result.Body.Length;
                }
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException
                                       || ex is NotSupportedException || ex is System.Security.SecurityException)
            {
                throw new CurlWriteErrorException(filePath ?? "(null)", CurlException.Compose(
                    $"ErgonomicExtensions.SaveToFile could not write the response of {Where(result)} ({ex.GetType().Name}: {ex.Message}).",
                    ex is ArgumentException
                        ? "Pass a valid, non-empty file path without characters such as : * ? \" < > | (on Windows)."
                        : "Check that the folder exists and is writable, that the file is not open in another program, and that the disk has space.",
                    DocLinks.HowToDownload), result.Command);
            }

            throw new InvalidOperationException(CurlException.Compose(
                $"No content to save: the response of {Where(result)} (HTTP {result.StatusCode}) has neither a body nor binary data.",
                result.OutputFiles.Count > 0
                    ? $"The command already wrote the body to '{result.OutputFiles[0]}' with -o/-O; use that file."
                    : "A 204 No Content or HEAD response has nothing to save; check the endpoint, or remove -o from the command so the body stays in the result.",
                DocLinks.HowToDownload));
        }

        /// <summary>
        /// Returns a response header value by name, ignoring case, or null - the same as <see cref="CurlResult.GetHeader(string)"/>.
        /// </summary>
        /// <remarks>
        /// <para><c>result.GetHeader(name)</c> binds to the instance method; this static form exists for
        /// <c>ErgonomicExtensions.GetHeader(result, name)</c> and method-group use. A null <see cref="CurlResult.Headers"/>
        /// returns null.</para>
        /// <para>Example: <c>var etag = ErgonomicExtensions.GetHeader(result, "ETag");</c></para>
        /// </remarks>
        /// <param name="result">The response whose headers to search; must not be null.</param>
        /// <param name="headerName">The header name in any case, such as <c>content-type</c>.</param>
        /// <returns>The header value, or null when absent.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <seealso cref="CurlResult.GetHeader(string)"/>
        /// <seealso cref="HasContentType(CurlResult, string)"/>
        public static string? GetHeader(this CurlResult result, string headerName)
        {
            RequireResult(result, nameof(GetHeader));
            if (result.Headers == null || !result.Headers.ContainsKey(headerName))
            {
                // Try case-insensitive lookup
                foreach (var header in result.Headers ?? new System.Collections.Generic.Dictionary<string, string>())
                {
                    if (string.Equals(header.Key, headerName, StringComparison.OrdinalIgnoreCase))
                    {
                        return header.Value;
                    }
                }
                return null;
            }

            return result.Headers[headerName];
        }

        /// <summary>
        /// True when the response's <c>Content-Type</c> header contains <paramref name="contentType"/>, ignoring case - so
        /// <c>HasContentType("json")</c> matches <c>application/json; charset=utf-8</c>.
        /// </summary>
        /// <remarks>
        /// <para>It is a substring test, not a media-type parse: <c>"json"</c> also matches <c>application/problem+json</c>.
        /// No <c>Content-Type</c> header returns false.</para>
        /// <para>Example: <c>if (result.HasContentType("application/json")) data = result.ParseJson&lt;Data&gt;(); else Console.WriteLine(result.Body);</c></para>
        /// </remarks>
        /// <param name="result">The response to inspect; must not be null.</param>
        /// <param name="contentType">The text to look for, such as <c>application/json</c>, <c>json</c> or <c>text/html</c>.</param>
        /// <returns>true when the header exists and contains the text; otherwise false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <seealso cref="GetHeader(CurlResult, string)"/>
        /// <seealso cref="CurlResult.Headers"/>
        public static bool HasContentType(this CurlResult result, string contentType)
        {
            RequireResult(result, nameof(HasContentType));
            var actualContentType = result.GetHeader("Content-Type");
            if (actualContentType == null)
            {
                return false;
            }

            return actualContentType.IndexOf(contentType, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Throws <see cref="CurlDotNet.Exceptions.CurlHttpException"/> unless the status is 200-299, otherwise returns the
        /// result - named like <c>HttpResponseMessage.EnsureSuccessStatusCode()</c> for code moved from HttpClient.
        /// </summary>
        /// <remarks>
        /// <para>Differs from <see cref="CurlResult.EnsureSuccess"/> only in the exception type: this one throws the
        /// <see cref="CurlException"/>-based <see cref="CurlDotNet.Exceptions.CurlHttpException"/> (so a single
        /// <c>catch (CurlException)</c> covers transport and HTTP failures), with <c>StatusCode</c>, <c>ResponseBody</c>,
        /// <c>IsNotFound</c>, <c>IsRateLimited</c> and status-specific <c>Suggestions</c>.</para>
        /// <para>Example: <c>var data = (await Curl.ExecuteAsync("curl https://api.example.com/items")).EnsureSuccessStatusCode().ParseJson&lt;List&lt;Item&gt;&gt;();</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <param name="result">The response to check; must not be null.</param>
        /// <returns>The same result when the status is 2xx, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <exception cref="CurlDotNet.Exceptions.CurlHttpException">The status is not 200-299. The message names the URL, the
        /// status and how to fix it for that status.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="CurlResult.EnsureSuccess"/>
        /// <seealso cref="CurlResult.IsSuccess"/>
        public static CurlResult EnsureSuccessStatusCode(this CurlResult result)
        {
            RequireResult(result, nameof(EnsureSuccessStatusCode));
            if (!result.IsSuccess)
            {
                var message = CurlException.Compose(
                    $"Response status code does not indicate success: {Where(result)} answered HTTP {result.StatusCode}" +
                    (string.IsNullOrEmpty(result.Body) ? "." : $" (body starts with \"{ErrorText.Preview(result.Body, 80)}\")."),
                    result.StatusFix(result.StatusCode),
                    DocLinks.HowToErrorHandling);
                throw new Exceptions.CurlHttpException(
                    message,
                    result.StatusCode,
                    statusText: null, // StatusText doesn't exist in CurlResult
                    responseBody: result.Body,
                    command: result.Command
                ).WithHeaders(result.Headers);
            }

            return result;
        }

        /// <summary>
        /// Collapses the result into <c>(Success, Body, Error)</c> for code that just wants "did it work, what came back,
        /// and why not".
        /// </summary>
        /// <remarks>
        /// <para><c>Success</c> is <see cref="CurlResult.IsSuccess"/> (2xx). On failure <c>Error</c> is the message of
        /// <see cref="CurlResult.Exception"/> (set by the non-throwing LibCurl API) or <c>"Request failed with status N"</c>;
        /// <c>Body</c> is returned either way, since error pages often explain the problem.</para>
        /// <para>Example: <c>var (ok, body, error) = (await Curl.ExecuteAsync("curl https://api.example.com/ping")).ToSimple(); if (!ok) Console.WriteLine(error);</c></para>
        /// </remarks>
        /// <param name="result">The response to summarise; must not be null.</param>
        /// <returns>A tuple: <c>Success</c> true for 2xx; <c>Body</c> the response text; <c>Error</c> null on success, else a description.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is null.</exception>
        /// <seealso cref="CurlResult.IsSuccess"/>
        /// <seealso cref="CurlResult.Exception"/>
        public static (bool Success, string? Body, string? Error) ToSimple(this CurlResult result)
        {
            RequireResult(result, nameof(ToSimple));
            if (result.IsSuccess)
            {
                return (true, result.Body, null);
            }

            // Check if there's an exception stored
            var error = result.Exception?.Message ?? $"Request failed with status {result.StatusCode}";
            return (false, result.Body, error);
        }
    }

    /// <summary>
    /// A tiny REST client bound to one base URL: <c>GetAsync("users/1")</c>, <c>PostJsonAsync("users", obj)</c>, PUT, PATCH and
    /// DELETE, each with the same time limit - for scripts that call one API many times.
    /// </summary>
    /// <remarks>
    /// <para>Each call builds a <see cref="CurlRequestBuilder"/> for <c>baseUrl + "/" + path</c> and runs it with a
    /// <c>--max-time</c> equal to the client's timeout. Responses with any HTTP status are returned (check
    /// <see cref="CurlResult.IsSuccess"/>); transport failures throw <see cref="CurlException"/> subclasses, such as
    /// <see cref="CurlCouldntResolveHostException"/> or <see cref="CurlOperationTimeoutException"/>. The client holds no
    /// connection or state and is safe to share between threads.</para>
    /// <para>Bodies are serialized with System.Text.Json (Newtonsoft.Json on .NET Standard 2.0), keeping C# property names;
    /// <c>Content-Type: application/json</c> is set for you. There is no built-in authentication: for a token, build requests
    /// with <see cref="CurlRequestBuilder.WithBearerToken(string)"/> instead.</para>
    /// <para>Example: <c>var api = new CurlApiClient("https://jsonplaceholder.typicode.com", TimeSpan.FromSeconds(10));</c>
    /// then <c>var post = (await api.GetAsync("posts/1")).EnsureSuccess().ParseJson&lt;Post&gt;();</c> and
    /// <c>await api.PostJsonAsync("posts", new { title = "hi", body = "...", userId = 1 });</c></para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlRequestBuilder"/>
    /// <seealso cref="Curl.GetAsync(string)"/>
    public class CurlApiClient
    {
        private readonly string _baseUrl;
        private readonly TimeSpan _defaultTimeout;

        /// <summary>
        /// Creates a client for <paramref name="baseUrl"/> whose requests all use <paramref name="defaultTimeout"/> (30 seconds
        /// when null) as their total time limit.
        /// </summary>
        /// <remarks>
        /// <para>A trailing <c>/</c> on <paramref name="baseUrl"/> is removed, so <c>"https://api.example.com/v1/"</c> and
        /// <c>"https://api.example.com/v1"</c> behave the same. The URL is not validated here; a malformed one fails on the first
        /// request with <see cref="CurlMalformedUrlException"/>.</para>
        /// <para>Example: <c>var api = new CurlApiClient("https://api.example.com/v1", TimeSpan.FromSeconds(15));</c></para>
        /// </remarks>
        /// <param name="baseUrl">Absolute base URL including scheme, such as <c>https://api.example.com/v1</c>; must not be null.</param>
        /// <param name="defaultTimeout">Total time limit per request (curl <c>--max-time</c>); null means 30 seconds.</param>
        /// <exception cref="ArgumentNullException"><paramref name="baseUrl"/> is null; pass the API's root URL.</exception>
        /// <seealso cref="GetAsync(string)"/>
        public CurlApiClient(string baseUrl, TimeSpan? defaultTimeout = null)
        {
            if (baseUrl == null)
            {
                throw new ArgumentNullException(nameof(baseUrl), CurlException.Compose(
                    "CurlApiClient was created with a null base URL.",
                    "Pass the API's root URL with its scheme, for example new CurlApiClient(\"https://api.example.com/v1\").",
                    DocLinks.HowToRunCommand));
            }
            _baseUrl = baseUrl.TrimEnd('/');
            _defaultTimeout = defaultTimeout ?? TimeSpan.FromSeconds(30);
        }

        private string Combine(string path) => string.IsNullOrEmpty(path) ? _baseUrl : $"{_baseUrl}/{path.TrimStart('/')}";

        /// <summary>
        /// Sends <c>GET baseUrl/path</c> and returns the response, whatever its status - like <c>curl -m T baseUrl/path</c>.
        /// </summary>
        /// <remarks>
        /// <para>A leading <c>/</c> on <paramref name="path"/> is ignored; a query string can be included
        /// (<c>"users?page=2"</c>). Redirects are not followed.</para>
        /// <para>Example: <c>var users = (await api.GetAsync("users?page=2")).EnsureSuccess().ParseJson&lt;List&lt;User&gt;&gt;();</c></para>
        /// </remarks>
        /// <param name="path">Path relative to the base URL, such as <c>users/42</c>; null or empty requests the base URL itself.</param>
        /// <returns>A task with the <see cref="CurlResult"/> (status, body, headers).</returns>
        /// <exception cref="CurlException">The transfer failed (DNS, connection, timeout after the client's limit, TLS). The
        /// message says what happened and how to fix it. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="PostJsonAsync(string, object)"/>
        /// <seealso cref="Curl.GetAsync(string)"/>
        public Task<CurlResult> GetAsync(string path)
        {
            return CurlRequestBuilder.Get(Combine(path))
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Sends <c>POST baseUrl/path</c> with <paramref name="data"/> serialized as JSON and returns the response - like
        /// <c>curl --json '{...}' baseUrl/path</c>.
        /// </summary>
        /// <remarks>
        /// <para>Sets <c>Content-Type: application/json</c>. Property names are kept as written in C#.</para>
        /// <para>Example: <c>var created = await api.PostJsonAsync("users", new { name = "Ada", email = "ada@example.com" }); created.EnsureStatus(201);</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md</para>
        /// </remarks>
        /// <param name="path">Path relative to the base URL, such as <c>users</c>.</param>
        /// <param name="data">Any serializable object (class, anonymous object, dictionary); null sends the JSON literal <c>null</c>.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="CurlException">The transfer failed (DNS, connection, timeout, TLS); see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="PutJsonAsync(string, object)"/>
        /// <seealso cref="PatchJsonAsync(string, object)"/>
        /// <seealso cref="CurlRequestBuilder.WithJson(object)"/>
        public Task<CurlResult> PostJsonAsync(string path, object data)
        {
            return CurlRequestBuilder.Post(Combine(path))
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Sends <c>PUT baseUrl/path</c> with <paramref name="data"/> serialized as JSON (replace a resource) and returns the
        /// response - like <c>curl -X PUT --json '{...}' baseUrl/path</c>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>await api.PutJsonAsync("users/42", new { name = "Ada Lovelace" });</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="path">Path of the resource relative to the base URL, such as <c>users/42</c>.</param>
        /// <param name="data">The full new representation as any serializable object.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="CurlException">The transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="PatchJsonAsync(string, object)"/>
        /// <seealso cref="PostJsonAsync(string, object)"/>
        public Task<CurlResult> PutJsonAsync(string path, object data)
        {
            return CurlRequestBuilder.Put(Combine(path))
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Sends <c>DELETE baseUrl/path</c> and returns the response (often 204 No Content) - like
        /// <c>curl -X DELETE baseUrl/path</c>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>(await api.DeleteAsync("users/42")).EnsureSuccess();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="path">Path of the resource relative to the base URL, such as <c>users/42</c>.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="CurlException">The transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="GetAsync(string)"/>
        public Task<CurlResult> DeleteAsync(string path)
        {
            return CurlRequestBuilder.Delete(Combine(path))
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }

        /// <summary>
        /// Sends <c>PATCH baseUrl/path</c> with <paramref name="data"/> serialized as JSON (change some fields) and returns
        /// the response - like <c>curl -X PATCH --json '{...}' baseUrl/path</c>.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>await api.PatchJsonAsync("users/42", new { email = "new@example.com" });</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md</para>
        /// </remarks>
        /// <param name="path">Path of the resource relative to the base URL, such as <c>users/42</c>.</param>
        /// <param name="data">An object with only the fields to change, as any serializable object.</param>
        /// <returns>A task with the <see cref="CurlResult"/>.</returns>
        /// <exception cref="CurlException">The transfer failed; see the message for the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="PutJsonAsync(string, object)"/>
        /// <seealso cref="PostJsonAsync(string, object)"/>
        public Task<CurlResult> PatchJsonAsync(string path, object data)
        {
            return CurlRequestBuilder.Patch(Combine(path))
                .WithJson(data)
                .WithTimeout(_defaultTimeout)
                .ExecuteAsync();
        }
    }
}

/***************************************************************************
 * CurlResult - The response object from every curl command
 *
 * Inspired by curl's callback system in src/tool_cb_*.c
 * Original curl Copyright (C) Daniel Stenberg, <daniel@haxx.se>, et al.
 *
 * This .NET implementation:
 * Copyright (C) 2024 IronSoftware
 *
 * This class is designed to be so intuitive that you can guess every method.
 * If you want to save to a file, just type .Save and IntelliSense shows you
 * SaveToFile(), SaveAsJson(), SaveAsCsv(). It just flows naturally.
 *
 * By Jacob Mellor
 * GitHub: https://github.com/jacob-mellor
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurlDotNet.Exceptions;

namespace CurlDotNet.Core
{
    /// <summary>
    /// What a curl command returned: the HTTP status, the body text (or bytes), the response headers, the files
    /// written, the timings - plus helpers to parse JSON, save, validate and retry.
    /// </summary>
    /// <remarks>
    /// <para>Every <c>Curl.ExecuteAsync</c>/<c>Curl.Execute</c> call, <see cref="CurlRequestBuilder"/> and
    /// <see cref="CurlDotNet.Lib.LibCurl"/> request returns one. Transport failures (DNS, connection, timeout, TLS, <c>-f</c>
    /// with an HTTP error) are thrown as <see cref="CurlException"/> subclasses instead, so a CurlResult in your hand means
    /// the server answered - possibly with 404 or 500, which you check with <see cref="IsSuccess"/>,
    /// <see cref="StatusCode"/> or <see cref="EnsureSuccess"/>. (The non-throwing <see cref="CurlDotNet.Lib.LibCurl"/> API
    /// returns a result with <see cref="StatusCode"/> 0 and the failure in <see cref="Exception"/> instead.)</para>
    /// <para>Where the data is: text responses are in <see cref="Body"/> (exactly what the curl tool would print on standard
    /// output, so with <c>-i</c> it starts with the headers and with <c>-w</c> it ends with the write-out text); binary
    /// responses (images, PDFs, zip, Office files) are in <see cref="BinaryData"/>; responses written with <c>-o</c>/<c>-O</c>
    /// are on disk and listed in <see cref="OutputFiles"/>; headers of the final response are in <see cref="Headers"/>.</para>
    /// <para>Most helpers return the same instance, so calls chain:
    /// <c>var user = (await Curl.ExecuteAsync("curl https://api.example.com/users/42")).EnsureSuccess().SaveToFile("user.json").ParseJson&lt;User&gt;();</c></para>
    /// <para>A CurlResult is a plain object with settable properties; it is not thread-safe while being modified
    /// (<see cref="FilterLines"/> changes <see cref="Body"/>).</para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/05-understanding-results.md</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");
    ///
    /// if (result.IsSuccess)  // 200-299
    /// {
    ///     var user = result.ParseJson&lt;User&gt;();  // Parse JSON to your type
    ///     result.SaveToFile("user.json");          // Save for later
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="Curl.ExecuteAsync(string)"/>
    /// <seealso cref="CurlTimings"/>
    /// <seealso cref="CurlException"/>
    public class CurlResult
    {
        #region Core Properties - The basics everyone needs

        /// <summary>
        /// The HTTP status code of the final response (after any redirects followed with <c>-L</c>), such as 200, 201,
        /// 304, 404 or 500 - curl's <c>%{http_code}</c>.
        /// </summary>
        /// <value>100-599 for HTTP; the FTP reply code (such as 226) for <c>ftp://</c>; 200, or 404 for a missing file, for
        /// <c>file://</c>; 0 when there was no response (<c>--skip-existing</c> skipped the request, or a
        /// <see cref="CurlDotNet.Lib.LibCurl"/> call failed - see <see cref="Exception"/>); 304 when a <c>-z</c> time
        /// condition was not met, like curl.</value>
        /// <remarks>
        /// <para>An HTTP error status is not an exception: 404 and 500 come back here unless the command used
        /// <c>-f</c>/<c>--fail</c> (then <see cref="CurlHttpReturnedErrorException"/>, exit code 22, is thrown). Use
        /// <see cref="IsSuccess"/> for "2xx", or <see cref="EnsureSuccess"/>/<see cref="EnsureStatus"/> to throw.</para>
        /// <para>Example: <c>switch (result.StatusCode) { case 200: Use(result.Body); break; case 404: return null; case 401: await RefreshTokenAsync(); break; }</c>.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="IsSuccess"/>
        /// <seealso cref="EnsureSuccess"/>
        /// <seealso cref="EnsureStatus(int)"/>
        public int StatusCode { get; set; }

        /// <summary>
        /// The response as text - exactly what the curl tool would print to standard output (the body, plus the
        /// headers with <c>-i</c>, the <c>-v</c> log, and the <c>-w</c> output).
        /// </summary>
        /// <value>The decoded text (charset from <c>Content-Type</c>, UTF-8 when none). Empty (not null) when the body was
        /// written to a file with <c>-o</c>/<c>-O</c>, when the response is binary (the bytes are in <see cref="BinaryData"/>),
        /// for HEAD requests without <c>-i</c>, and for 204 responses. Null only on a CurlResult you created yourself.</value>
        /// <remarks>
        /// <para>With a plain <c>curl URL</c> this is just the response body, ready for <see cref="ParseJson{T}"/>. Flags
        /// that make curl print more also change Body: <c>-i</c> puts the status line and headers first, <c>-w '%{http_code}'</c>
        /// appends the expanded text, <c>-v</c> prepends the request/response log. Use <see cref="Headers"/> and
        /// <see cref="StatusCode"/> rather than parsing them out of Body.</para>
        /// <para>Example: <c>var html = (await Curl.ExecuteAsync("curl -L https://example.com")).Body;</c></para>
        /// </remarks>
        /// <seealso cref="BinaryData"/>
        /// <seealso cref="OutputFiles"/>
        /// <seealso cref="ParseJson{T}"/>
        /// <seealso cref="SaveToFile(string)"/>
        public string Body { get; set; }

        /// <summary>
        /// The headers of the final response, by name, case-insensitive (<c>Headers["content-type"]</c> works); repeated
        /// headers such as <c>Set-Cookie</c> are joined with <c>", "</c>.
        /// </summary>
        /// <value>A dictionary with <see cref="StringComparer.OrdinalIgnoreCase"/>; values are exactly as the server sent them
        /// (after the colon, trimmed). Empty when there was no HTTP response. After <c>-L</c> only the last hop's headers are
        /// here; use <c>-i</c> and <see cref="Body"/> to see every hop.</value>
        /// <remarks>
        /// <para>Indexing a missing name throws <see cref="KeyNotFoundException"/>; <see cref="GetHeader(string)"/> returns
        /// null instead and also works on a dictionary you assigned yourself with a case-sensitive comparer.</para>
        /// <para>Example: <c>if (result.Headers.TryGetValue("X-RateLimit-Remaining", out var left) &amp;&amp; int.Parse(left) &lt; 10) await Task.Delay(60_000);</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/08-headers-explained.md</para>
        /// </remarks>
        /// <seealso cref="GetHeader(string)"/>
        /// <seealso cref="HasHeader(string)"/>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// True when <see cref="StatusCode"/> is 200-299 (OK, Created, No Content...); false for 1xx, 3xx, 4xx, 5xx and 0.
        /// </summary>
        /// <value>Computed from <see cref="StatusCode"/> each time; never throws.</value>
        /// <remarks>
        /// <para>A 304 Not Modified or an unfollowed 301/302 is not "success" here. To accept other codes, compare
        /// <see cref="StatusCode"/> yourself or call <see cref="EnsureStatus(int)"/>.</para>
        /// <para>Example: <c>if (!result.IsSuccess) logger.LogWarning("{Status} from {Command}", result.StatusCode, result.Command);</c></para>
        /// </remarks>
        /// <seealso cref="StatusCode"/>
        /// <seealso cref="EnsureSuccess"/>
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        /// <summary>
        /// The response body as raw bytes when it is binary (image, PDF, zip, Office file, or anything that is not
        /// text); null for text responses, whose content is in <see cref="Body"/>.
        /// </summary>
        /// <value>The bytes exactly as received (after <c>--compressed</c> decoding) for a binary response, or null for a
        /// text response. A binary body written to a file with <c>-o</c> is kept here too; a text body written with
        /// <c>-o</c> is in neither <see cref="Body"/> nor this property, only in the file.</value>
        /// <remarks>
        /// <para>A response is binary when its <c>Content-Type</c> is not a text type, or when the bytes look binary even
        /// under a text type (a server sending an .xlsx as <c>text/plain</c>). Force it with
        /// <see cref="CurlRequestBuilder.AsBinary"/>, or register types with <see cref="CurlRequestBuilder.WithBinaryContentType(string)"/>.</para>
        /// <para>Example: <c>var r = await Curl.ExecuteAsync("curl https://example.com/logo.png"); if (r.IsBinary) File.WriteAllBytes("logo.png", r.BinaryData);</c>
        /// - or simply <c>r.SaveToFile("logo.png")</c>, which picks bytes or text for you.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="IsBinary"/>
        /// <seealso cref="Body"/>
        /// <seealso cref="SaveToFile(string)"/>
        /// <seealso cref="ToStream"/>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// True when the response body is in <see cref="BinaryData"/> (non-empty) rather than in <see cref="Body"/>.
        /// </summary>
        /// <value>Computed: <c>BinaryData != null &amp;&amp; BinaryData.Length &gt; 0</c>.</value>
        /// <remarks>
        /// <para>Example: <c>if (result.IsBinary) File.WriteAllBytes(path, result.BinaryData); else File.WriteAllText(path, result.Body);</c>
        /// (which is what <see cref="SaveToFile(string)"/> does).</para>
        /// </remarks>
        /// <seealso cref="BinaryData"/>
        /// <seealso cref="Body"/>
        public bool IsBinary => BinaryData != null && BinaryData.Length > 0;

        /// <summary>
        /// The curl command that produced this result, as you passed it (or as <see cref="CurlRequestBuilder.ToCurlCommand"/>
        /// rendered it); used by <see cref="Retry"/> and in error messages.
        /// </summary>
        /// <value>The command text, or null for a result you created yourself. It can contain secrets you put in the
        /// command (<c>-u user:password</c>, <c>-H "Authorization: Bearer ..."</c>), so redact it before logging; passing
        /// secrets through <see cref="CurlSettings.Headers"/> keeps them out of this string.</value>
        /// <remarks>
        /// <para>Example: <c>logger.LogInformation("{Status} for {Command}", result.StatusCode, result.Command);</c></para>
        /// </remarks>
        /// <seealso cref="Retry"/>
        /// <seealso cref="RetryWith(Action{CurlSettings})"/>
        public string Command { get; set; }

        /// <summary>
        /// How long each phase of the transfer took (DNS, connect, TLS, first byte, total) in milliseconds, like curl's
        /// <c>-w '%{time_total}'</c> family.
        /// </summary>
        /// <value>Filled for HTTP(S) transfers; null or all zeros for other protocols and results you created yourself.
        /// See <see cref="CurlTimings"/> for what each value means and when it is 0.</value>
        /// <remarks>
        /// <para>Example: <c>if (result.Timings.Total &gt; 2000) Console.WriteLine($"slow: DNS {result.Timings.NameLookup:F0} ms, connect {result.Timings.Connect:F0} ms, first byte {result.Timings.StartTransfer:F0} ms");</c></para>
        /// </remarks>
        /// <seealso cref="CurlTimings"/>
        /// <seealso cref="PrintVerbose"/>
        public CurlTimings Timings { get; set; }

        /// <summary>
        /// Paths of the files this transfer wrote: the <c>-o</c>/<c>-O</c> output (with <c>--output-dir</c> and
        /// <c>--no-clobber</c> renaming applied), plus any files saved later with <see cref="SaveToFile(string)"/> and friends.
        /// </summary>
        /// <value>Paths as written (relative paths stay relative to the current directory); empty when the body went to
        /// <see cref="Body"/>/<see cref="BinaryData"/> only. <c>-D</c>, <c>-c</c> and <c>--trace</c> files are not listed.</value>
        /// <remarks>
        /// <para>Example: <c>var r = await Curl.ExecuteAsync("curl -O https://example.com/files/report.pdf"); Console.WriteLine(r.OutputFiles[0]); // report.pdf</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <seealso cref="SaveToFile(string)"/>
        /// <seealso cref="Body"/>
        /// <seealso cref="BinaryData"/>
        public List<string> OutputFiles { get; set; } = new List<string>();

        /// <summary>
        /// The failure behind a result that has no HTTP response, set only by the non-throwing
        /// <see cref="CurlDotNet.Lib.LibCurl"/> API; null for results from <c>Curl.ExecuteAsync</c>, which throws instead.
        /// </summary>
        /// <value>A <see cref="CurlException"/> subclass (DNS, connect, timeout, TLS...) with <see cref="StatusCode"/> 0, or
        /// null. HTTP errors such as 404 never set it - check <see cref="StatusCode"/> for those.</value>
        /// <remarks>
        /// <para>Example: <c>var r = await libCurl.GetAsync(url); if (r.Exception is CurlException ce) Console.WriteLine($"{ce.WhatHappened} - {ce.DocumentationUrl}");</c></para>
        /// </remarks>
        /// <seealso cref="CurlException"/>
        /// <seealso cref="IsSuccess"/>
        public Exception Exception { get; set; }

        #endregion

        #region JSON Operations - Working with JSON responses

        /// <summary>
        /// JSON APIs are overwhelmingly camelCase while C# properties are PascalCase, so property
        /// matching is case-insensitive by default (the same as Newtonsoft on netstandard2.0).
        /// </summary>
        private static readonly System.Text.Json.JsonSerializerOptions JsonOptionsForParsing = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        /// <summary>
        /// Deserializes the JSON in <see cref="Body"/> into a <typeparamref name="T"/> - your class, a
        /// <c>List&lt;T&gt;</c>, a <c>Dictionary&lt;string, object&gt;</c> - matching property names case-insensitively.
        /// </summary>
        /// <remarks>
        /// <para>On .NET 8/9/10 and .NET Framework 4.7.2/4.8 this uses System.Text.Json with
        /// <c>PropertyNameCaseInsensitive = true</c> (so <c>"userName"</c> fills <c>UserName</c>); on the .NET Standard 2.0
        /// build it uses Newtonsoft.Json, which is case-insensitive by default. JSON properties without a matching C#
        /// property are ignored, and C# properties without a JSON value keep their default. JSON <c>null</c> returns
        /// <c>null</c>/<c>default</c>.</para>
        /// <para>The status code is not checked: a 404 with a JSON error body parses happily into your type with empty
        /// properties. Call <see cref="EnsureSuccess"/> first when only 2xx bodies are valid.</para>
        /// <para>Example: <c>var user = (await Curl.ExecuteAsync("curl https://api.github.com/users/octocat")).EnsureSuccess().ParseJson&lt;GitHubUser&gt;();</c>
        /// then <c>Console.WriteLine(user.Login);</c>. For arrays: <c>result.ParseJson&lt;List&lt;Repo&gt;&gt;()</c>. To explore
        /// JSON without a class, use <see cref="AsJsonDynamic"/>; to choose your own serializer options, use the
        /// <see cref="ErgonomicExtensions.ParseJson{T}(CurlResult, System.Text.Json.JsonSerializerOptions)"/> extension.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <typeparam name="T">A class or struct with a public parameterless constructor and settable properties, a
        /// collection such as <see cref="List{T}"/>, or <see cref="Dictionary{TKey, TValue}"/>. Interfaces and abstract types
        /// cannot be created.</typeparam>
        /// <returns>A new <typeparamref name="T"/> filled from <see cref="Body"/>; <c>null</c>/<c>default</c> when the body is
        /// the JSON literal <c>null</c>.</returns>
        /// <exception cref="CurlParsingException">The body is empty (a 204, a HEAD request, output sent to a file with
        /// <c>-o</c>, or binary data), the body is not valid JSON (often an HTML error page or login redirect), or the JSON
        /// does not fit <typeparamref name="T"/> (an array into a single object, text into a number). The message names the
        /// URL, status, Content-Type and the start of the body; the parser's exception is the InnerException. Fix: check
        /// <see cref="StatusCode"/> first and print <see cref="Body"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</exception>
        /// <example>
        /// <code language="csharp">
        /// public class User { public string Name { get; set; } public string Email { get; set; } public int Id { get; set; } }
        ///
        /// var response = await Curl.ExecuteAsync("curl https://api.example.com/users/42");
        /// var user = response.EnsureSuccess().ParseJson&lt;User&gt;();
        /// Console.WriteLine($"Hello {user.Name}!");
        ///
        /// var users = (await Curl.ExecuteAsync("curl https://api.example.com/users")).ParseJson&lt;List&lt;User&gt;&gt;();
        /// </code>
        /// </example>
        /// <seealso cref="AsJson{T}"/>
        /// <seealso cref="AsJsonDynamic"/>
        /// <seealso cref="Body"/>
        /// <seealso cref="SaveAsJson(string, bool)"/>
        public T ParseJson<T>()
        {
            if (string.IsNullOrEmpty(Body))
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{TypeName(typeof(T))}>() found no JSON to parse: the response body of {DescribeRequest()} " +
                    (IsBinary ? $"is binary ({BinaryData.Length} bytes in BinaryData, HTTP {StatusCode})." : $"is empty (HTTP {StatusCode})."),
                    EmptyBodyFix(), DocLinks.HowToParseJson),
                    GetHeader("Content-Type"), typeof(T), Command);
            }

            try
            {
#if NETSTANDARD2_0
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Body);
#else
                return System.Text.Json.JsonSerializer.Deserialize<T>(Body, JsonOptionsForParsing);
#endif
            }
#if NETSTANDARD2_0
            catch (Newtonsoft.Json.JsonException ex)
#else
            catch (System.Text.Json.JsonException ex)
#endif
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{TypeName(typeof(T))}>() could not read the response body of {DescribeRequest()} as {TypeName(typeof(T))}: {ex.Message} " +
                    $"(HTTP {StatusCode}, Content-Type: {GetHeader("Content-Type") ?? "none"}, body starts with \"{ErrorText.Preview(Body, 60)}\").",
                    BadJsonFix(typeof(T)), DocLinks.HowToParseJson),
                    GetHeader("Content-Type"), typeof(T), Command, ex);
            }
            catch (Exception ex) when (ex is NotSupportedException || ex is InvalidOperationException || ex is ArgumentException || ex is MissingMethodException)
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"ParseJson<{TypeName(typeof(T))}>() cannot create a {TypeName(typeof(T))} from JSON ({ex.GetType().Name}: {ex.Message}).",
                    "Use a concrete class with a public parameterless constructor and settable properties (not an interface or abstract type), or read the data with AsJsonDynamic().",
                    DocLinks.HowToParseJson),
                    GetHeader("Content-Type"), typeof(T), Command, ex);
            }
        }

        /// <summary>A C#-style type name for messages: <c>Dictionary&lt;String, Object&gt;</c> instead of <c>Dictionary`2</c>.</summary>
        private static string TypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            var name = type.Name;
            var tick = name.IndexOf('`');
            if (tick >= 0)
            {
                name = name.Substring(0, tick);
            }
            return name + "<" + string.Join(", ", type.GetGenericArguments().Select(TypeName)) + ">";
        }

        /// <summary>The fix for an empty body, depending on where the data went.</summary>
        private string EmptyBodyFix()
        {
            if (IsBinary)
            {
                return $"The response is binary ({BinaryData.Length} bytes in BinaryData, Content-Type: {GetHeader("Content-Type") ?? "none"}), not JSON; save it with SaveToFile(path) or request the JSON endpoint.";
            }
            if (OutputFiles != null && OutputFiles.Count > 0)
            {
                return $"The body was written to '{OutputFiles[0]}' by -o/-O, so Body is empty; read that file, or drop -o to keep the JSON in Body.";
            }
            return "Check StatusCode before parsing (204 No Content and HEAD requests have no body), and make sure the request asks for JSON, for example with -H 'Accept: application/json'.";
        }

        /// <summary>The fix for a body that did not parse, depending on the status code.</summary>
        private string BadJsonFix(Type target)
        {
            if (!IsSuccess && StatusCode != 0)
            {
                return $"The server answered HTTP {StatusCode}, so the body is probably an error message rather than your data; check StatusCode (or call EnsureSuccess()) before parsing.";
            }
            return $"Print result.Body to see what arrived: an HTML page usually means a wrong URL, method or missing credentials; a JSON array needs List<{target.Name}> rather than {target.Name}.";
        }

        /// <summary>
        /// Deserializes the JSON in <see cref="Body"/> into a <typeparamref name="T"/>; identical to
        /// <see cref="ParseJson{T}"/>, for those who prefer the name.
        /// </summary>
        /// <remarks>
        /// <para>Same serializer, case-insensitive matching and errors as <see cref="ParseJson{T}"/>.</para>
        /// <para>Example: <c>var order = result.AsJson&lt;Order&gt;();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <typeparam name="T">The type to create; see <see cref="ParseJson{T}"/>.</typeparam>
        /// <returns>A new <typeparamref name="T"/> filled from <see cref="Body"/>.</returns>
        /// <exception cref="CurlParsingException">The body is empty (a 204, a HEAD request, output sent to a file with
        /// <c>-o</c>, or binary data), the body is not valid JSON (often an HTML error page or login redirect), or the JSON
        /// does not fit <typeparamref name="T"/> (an array into a single object, text into a number). The message names the
        /// URL, status, Content-Type and the start of the body; the parser's exception is the InnerException. Fix: check
        /// <see cref="StatusCode"/> first and print <see cref="Body"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</exception>
        /// <seealso cref="ParseJson{T}"/>
        /// <seealso cref="AsJsonDynamic"/>
        public T AsJson<T>() => ParseJson<T>();

        /// <summary>
        /// Parses <see cref="Body"/> as JSON and returns it as a <c>dynamic</c> object, so you can read
        /// <c>json.rate.remaining</c> or <c>json.items[0].title</c> without writing a class first.
        /// </summary>
        /// <remarks>
        /// <para>What you get back, on .NET 8/9/10 and .NET Framework 4.7.2/4.8 (System.Text.Json builds):
        /// a JSON object becomes a dynamic object that supports <c>json.name</c>, <c>json["name"]</c>,
        /// <c>foreach (var property in json)</c> (key/value pairs) and a cast to
        /// <c>Dictionary&lt;string, object&gt;</c>; a JSON array becomes a <c>List&lt;object&gt;</c> (use
        /// <c>json.items[0]</c>, <c>json.items.Count</c>, <c>foreach</c>); numbers become <c>int</c> when they fit,
        /// otherwise <c>long</c>, otherwise <c>double</c>; strings become <c>string</c>, <c>true</c>/<c>false</c>
        /// become <c>bool</c> and <c>null</c> becomes <c>null</c>.</para>
        /// <para>A property that does not exist reads as <c>null</c> instead of throwing. Property names are
        /// matched exactly first; when that fails a single case-insensitive match is used, so <c>json.Name</c>
        /// also finds <c>"name"</c>. <c>json.ToString()</c> on an object returns its original JSON text.</para>
        /// <para>On the .NET Standard 2.0 build (Unity, Xamarin, older runtimes) the result is Newtonsoft's
        /// <c>JObject</c>/<c>JArray</c>, which supports the same <c>json.name</c>, <c>json["name"]</c> and
        /// <c>foreach</c> syntax; numbers are <c>JValue</c>s that convert implicitly to <c>int</c>, <c>long</c> or
        /// <c>double</c>.</para>
        /// <para>Example, reading GitHub's rate limit without a class: <c>var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");</c>
        /// then <c>dynamic json = result.AsJsonDynamic();</c> and <c>int remaining = json.rate.remaining;</c>
        /// and <c>Console.WriteLine($"{remaining} of {json.rate.limit} calls left");</c>.</para>
        /// <para>Example, iterating an array: <c>foreach (var repo in result.AsJsonDynamic()) Console.WriteLine(repo.full_name);</c>
        /// for a response such as <c>[{"full_name":"a/b"}, ...]</c>.</para>
        /// <para>There is no compile-time checking with <c>dynamic</c>: a typo in a property name gives
        /// <c>null</c>, not a compiler error. For code you keep, prefer <see cref="ParseJson{T}"/> with a typed
        /// class. Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <returns>The root of the JSON document as described in the remarks: a dynamic object for
        /// <c>{...}</c>, a <c>List&lt;object&gt;</c> for <c>[...]</c>, or a plain value (string, number, bool)
        /// for a document that is just a value. Never a <see cref="System.Text.Json.JsonDocument"/>.</returns>
        /// <exception cref="CurlParsingException">The body is empty (for example a 204 response or a HEAD
        /// request) or is not valid JSON (for example an HTML error page). The message names the URL, the
        /// Content-Type and the first characters of the body; the parser's own exception is the
        /// <see cref="Exception.InnerException"/>. Fix: check <see cref="StatusCode"/> and print
        /// <see cref="Body"/>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</exception>
        /// <example>
        /// <code language="csharp">
        /// var result = await Curl.ExecuteAsync("curl https://api.github.com/rate_limit");
        /// dynamic json = result.AsJsonDynamic();
        /// int remaining = json.rate.remaining;
        /// Console.WriteLine($"{remaining} of {json.rate.limit} calls left");
        ///
        /// var repos = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat/repos");
        /// foreach (var repo in repos.AsJsonDynamic())   // a JSON array
        ///     Console.WriteLine(repo.full_name);
        /// </code>
        /// </example>
        /// <seealso cref="ParseJson{T}">Parse JSON into a typed class (recommended for code you keep)</seealso>
        /// <seealso cref="AsJson{T}">Same as ParseJson</seealso>
        /// <seealso cref="Body">The JSON text being parsed</seealso>
        public dynamic AsJsonDynamic()
        {
            if (string.IsNullOrWhiteSpace(Body))
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"AsJsonDynamic() found no JSON to parse: the response body of {DescribeRequest()} " +
                    (IsBinary ? $"is binary ({BinaryData.Length} bytes in BinaryData, HTTP {StatusCode})." : $"is empty (HTTP {StatusCode})."),
                    EmptyBodyFix(),
                    DocLinks.HowToParseJson),
                    GetHeader("Content-Type"), typeof(object), Command);
            }

            try
            {
#if NETSTANDARD2_0
                return Newtonsoft.Json.JsonConvert.DeserializeObject(Body);
#else
                return DynamicJson.Parse(Body)!;
#endif
            }
#if NETSTANDARD2_0
            catch (Newtonsoft.Json.JsonException ex)
#else
            catch (System.Text.Json.JsonException ex)
#endif
            {
                throw new CurlParsingException(CurlException.Compose(
                    $"AsJsonDynamic() could not parse the response body of {DescribeRequest()} as JSON: {ex.Message} (Content-Type: {GetHeader("Content-Type") ?? "none"}; body starts with \"{ErrorText.Preview(Body, 60)}\").",
                    "Print result.Body to see what the server sent; an HTML error page or a login redirect usually means the URL, the method or the credentials are wrong.",
                    DocLinks.HowToParseJson),
                    GetHeader("Content-Type"), typeof(object), Command, ex);
            }
        }

        /// <summary>The request URL for messages (without credentials or query string), or "the request".</summary>
        private string DescribeRequest() => ErrorText.UrlFromCommand(Command) ?? "the request";

        #endregion

        #region Save Operations - Save responses to files

        /// <summary>
        /// Writes the response to <paramref name="filePath"/> - the bytes of <see cref="BinaryData"/> for binary responses,
        /// otherwise the text of <see cref="Body"/> - overwriting any existing file, and returns this result.
        /// </summary>
        /// <remarks>
        /// <para>Text is written as UTF-8 (without a byte-order mark); binary data byte-for-byte. The path is added to
        /// <see cref="OutputFiles"/>. The folder must already exist. Note that <see cref="Body"/> is what curl prints, so a
        /// command with <c>-i</c> or <c>-w</c> saves those extras too.</para>
        /// <para>For large downloads prefer curl's own <c>-o file</c> (or <c>-O</c>), which writes as the transfer runs:
        /// <c>await Curl.ExecuteAsync("curl -o report.pdf https://example.com/report.pdf");</c></para>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl https://example.com/logo.png")).SaveToFile("logo.png");</c> or chained:
        /// <c>var data = result.EnsureSuccess().SaveToFile("backup.json").ParseJson&lt;Data&gt;();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Relative (to the current directory) or absolute path, such as <c>"data.json"</c>,
        /// <c>"downloads/report.pdf"</c> or <c>@"C:\Temp\out.txt"</c>.</param>
        /// <returns>This same result, for chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty or whitespace; pass a path such as
        /// <c>"out.json"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23): the folder does not
        /// exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
        /// characters. The message names the path, the cause and the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="SaveToFileAsync(string)"/>
        /// <seealso cref="AppendToFile(string)"/>
        /// <seealso cref="SaveAsJson(string, bool)"/>
        /// <seealso cref="OutputFiles"/>
        /// <seealso cref="BinaryData"/>
        public CurlResult SaveToFile(string filePath)
        {
            WriteOrThrow(filePath, nameof(SaveToFile), () =>
            {
                if (BinaryData != null)
                    File.WriteAllBytes(filePath, BinaryData);
                else
                    File.WriteAllText(filePath, Body ?? "");
            });

            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>Size of what SaveToFile/AppendToFile write, for messages.</summary>
        private long PayloadSize => BinaryData?.LongLength ?? (Body == null ? 0 : Encoding.UTF8.GetByteCount(Body));

        /// <summary>
        /// Runs a file write and turns path and I/O failures into exceptions that name the file and the fix.
        /// </summary>
        private void WriteOrThrow(string filePath, string method, Action write)
        {
            ValidatePath(filePath, method);
            try
            {
                write();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException
                                       || ex is NotSupportedException || ex is System.Security.SecurityException)
            {
                throw WriteError(filePath, method, ex);
            }
        }

        private static void ValidatePath(string filePath, string method)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                var message = CurlException.Compose(
                    $"{method}() needs a file path, but was given {(filePath == null ? "null" : "an empty string")}.",
                    "Pass a relative path such as \"out.json\" or an absolute one such as \"/tmp/out.json\".",
                    DocLinks.HowToDownload);
                throw filePath == null ? new ArgumentNullException(nameof(filePath), message) : new ArgumentException(message, nameof(filePath));
            }
        }

        private CurlWriteErrorException WriteError(string filePath, string method, Exception ex)
        {
            string fix;
            if (ex is DirectoryNotFoundException)
            {
                string folder;
                try { folder = Path.GetDirectoryName(Path.GetFullPath(filePath)) ?? filePath; } catch (Exception) { folder = filePath; }
                fix = $"The folder '{folder}' does not exist; create it first with Directory.CreateDirectory, or save to an existing folder.";
            }
            else if (ex is UnauthorizedAccessException || ex is System.Security.SecurityException)
            {
                fix = "This process may not write there, or the file is read-only; choose a folder you can write to (for example Path.GetTempPath()) or fix the permissions.";
            }
            else if (ex is PathTooLongException)
            {
                fix = "The path is longer than this platform allows; use a shorter folder or file name.";
            }
            else if (ex is ArgumentException || ex is NotSupportedException)
            {
                fix = "The path contains characters that are not allowed in a file name (such as : * ? \" < > | on Windows); remove them.";
            }
            else
            {
                fix = "The file may be open in another program, or the disk may be full; close the file or free some space, then try again.";
            }
            return new CurlWriteErrorException(filePath, CurlException.Compose(
                $"{method}() could not write {PayloadSize} bytes of the response of {DescribeRequest()} ({ex.GetType().Name}: {ex.Message}).",
                fix, DocLinks.HowToDownload), Command, ex);
        }

        /// <summary>
        /// Writes the response to <paramref name="filePath"/> without blocking the calling thread - bytes for binary
        /// responses, text for the rest - and returns this result when the file is complete.
        /// </summary>
        /// <remarks>
        /// <para>Same rules as <see cref="SaveToFile(string)"/>: overwrites the file, writes UTF-8 text or raw bytes, adds the
        /// path to <see cref="OutputFiles"/>, and needs an existing folder. On .NET 8+ it uses the asynchronous file APIs; on
        /// .NET Standard 2.0 and .NET Framework, which lack them, the write runs on a thread-pool thread.</para>
        /// <para>Example: <c>var result = await Curl.ExecuteAsync("curl https://example.com/data.json"); await result.SaveToFileAsync("data.json");</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Relative or absolute path of the file to create or overwrite, such as <c>"exports/data.json"</c>.</param>
        /// <returns>A task that completes with this same result once the file is written.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty or whitespace; pass a path such as
        /// <c>"out.json"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23): the folder does not
        /// exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
        /// characters. The message names the path, the cause and the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="SaveToFile(string)"/>
        /// <seealso cref="OutputFiles"/>
        public async Task<CurlResult> SaveToFileAsync(string filePath)
        {
            ValidatePath(filePath, nameof(SaveToFileAsync));
            try
            {
#if NETSTANDARD2_0 || NET472 || NET48
                // .NET Framework and .NET Standard 2.0 don't have async file methods
                await Task.Run(() =>
                {
                    if (BinaryData != null)
                        File.WriteAllBytes(filePath, BinaryData);
                    else
                        File.WriteAllText(filePath, Body ?? "");
                }).ConfigureAwait(false);
#else
                if (BinaryData != null)
                    await File.WriteAllBytesAsync(filePath, BinaryData).ConfigureAwait(false);
                else
                    await File.WriteAllTextAsync(filePath, Body ?? "").ConfigureAwait(false);
#endif
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is ArgumentException
                                       || ex is NotSupportedException || ex is System.Security.SecurityException)
            {
                throw WriteError(filePath, nameof(SaveToFileAsync), ex);
            }

            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// Writes <see cref="Body"/> to <paramref name="filePath"/> as re-formatted JSON (indented by default, or
        /// minified), and returns this result; a body that is not JSON is written unchanged.
        /// </summary>
        /// <remarks>
        /// <para>The JSON is parsed and written again (System.Text.Json on .NET 8+/.NET Framework, Newtonsoft.Json on
        /// .NET Standard 2.0), so the content is the same but the whitespace changes. Invalid JSON does not throw: the
        /// text is saved as-is. Binary responses are not handled here; use <see cref="SaveToFile(string)"/>. The path is
        /// added to <see cref="OutputFiles"/>.</para>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl https://api.github.com/repos/dotnet/runtime")).SaveAsJson("runtime.json");</c>
        /// turns <c>{"id":1,"name":"x"}</c> into a readable, indented file.</para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md</para>
        /// </remarks>
        /// <param name="filePath">Relative or absolute path of the file to create or overwrite, such as <c>"data.json"</c>.</param>
        /// <param name="indented">true (default) for two-space indented output, false for minified single-line JSON.</param>
        /// <returns>This same result, for chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty or whitespace; pass a path such as
        /// <c>"out.json"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23): the folder does not
        /// exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
        /// characters. The message names the path, the cause and the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="SaveToFile(string)"/>
        /// <seealso cref="SaveAsCsv(string)"/>
        /// <seealso cref="ParseJson{T}"/>
        public CurlResult SaveAsJson(string filePath, bool indented = true)
        {
            string formatted;

            try
            {
#if NETSTANDARD2_0
                var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(Body);
                formatted = Newtonsoft.Json.JsonConvert.SerializeObject(obj,
                    indented ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None);
#else
                using var doc = System.Text.Json.JsonDocument.Parse(Body);
                var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = indented };
                formatted = System.Text.Json.JsonSerializer.Serialize(doc.RootElement, options);
#endif
            }
            catch
            {
                // If not valid JSON, save as-is
                formatted = Body;
            }

            WriteOrThrow(filePath, nameof(SaveAsJson), () => File.WriteAllText(filePath, formatted));
            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// Converts a JSON array of objects in <see cref="Body"/> to CSV (one column per property of the first object)
        /// and writes it to <paramref name="filePath"/>; a body that is not such an array is written unchanged.
        /// </summary>
        /// <remarks>
        /// <para>The header row comes from the first object's property names; each later object contributes the values
        /// of those properties (missing ones are empty, extra ones are ignored). Values containing a comma or a quote are
        /// quoted with doubled quotes; nested objects and arrays are written as their JSON text. An empty array writes an
        /// empty file. The path is added to <see cref="OutputFiles"/>.</para>
        /// <para>Example: for <c>[{"name":"John","age":30},{"name":"Jane","age":25}]</c>,
        /// <c>result.SaveAsCsv("users.csv")</c> writes <c>name,age</c>, <c>John,30</c>, <c>Jane,25</c> - ready for Excel.</para>
        /// </remarks>
        /// <param name="filePath">Relative or absolute path of the CSV file to create or overwrite, such as <c>"users.csv"</c>.</param>
        /// <returns>This same result, for chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty or whitespace; pass a path such as
        /// <c>"out.json"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23): the folder does not
        /// exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
        /// characters. The message names the path, the cause and the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="SaveAsJson(string, bool)"/>
        /// <seealso cref="SaveToFile(string)"/>
        public CurlResult SaveAsCsv(string filePath)
        {
            var csv = ConvertJsonToCsv(Body);
            WriteOrThrow(filePath, nameof(SaveAsCsv), () => File.WriteAllText(filePath, csv));
            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// Appends the response (bytes of <see cref="BinaryData"/>, or the text of <see cref="Body"/>) to the end of
        /// <paramref name="filePath"/>, creating the file if needed, and returns this result.
        /// </summary>
        /// <remarks>
        /// <para>Nothing is added between responses - append your own newline if you need one. Unlike
        /// <see cref="SaveToFile(string)"/>, the path is not added to <see cref="OutputFiles"/>. Text is written as UTF-8.</para>
        /// <para>Example: <c>foreach (var url in urls) (await Curl.ExecuteAsync($"curl {url}")).AppendToFile("combined.txt");</c></para>
        /// </remarks>
        /// <param name="filePath">Relative or absolute path of the file to append to, such as <c>"api-log.txt"</c>.</param>
        /// <returns>This same result, for chaining.</returns>
        /// <exception cref="ArgumentException"><paramref name="filePath"/> is null, empty or whitespace; pass a path such as
        /// <c>"out.json"</c>. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <exception cref="CurlWriteErrorException">The file could not be written (exit code 23): the folder does not
        /// exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
        /// characters. The message names the path, the cause and the fix.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md</exception>
        /// <seealso cref="SaveToFile(string)"/>
        /// <seealso cref="SaveToFileAsync(string)"/>
        public CurlResult AppendToFile(string filePath)
        {
            WriteOrThrow(filePath, nameof(AppendToFile), () =>
            {
                if (BinaryData != null)
                {
                    using var stream = new FileStream(filePath, FileMode.Append);
                    stream.Write(BinaryData, 0, BinaryData.Length);
                }
                else
                {
                    File.AppendAllText(filePath, Body ?? "");
                }
            });
            return this;
        }

        #endregion

        #region Header Operations - Working with HTTP headers

        /// <summary>
        /// Returns the value of one response header by name, ignoring case, or null when the response did not have it -
        /// safer than indexing <see cref="Headers"/>, which throws for a missing name.
        /// </summary>
        /// <remarks>
        /// <para>Repeated headers come back joined with <c>", "</c> (for example two <c>Set-Cookie</c> lines). The lookup is
        /// case-insensitive even when you replaced <see cref="Headers"/> with a case-sensitive dictionary.</para>
        /// <para>Example: <c>var left = result.GetHeader("X-RateLimit-Remaining"); if (left != null &amp;&amp; int.Parse(left) &lt; 10) await Task.Delay(60_000);</c>
        /// or <c>if (result.GetHeader("Content-Type")?.Contains("json") == true) data = result.ParseJson&lt;Data&gt;();</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/08-headers-explained.md</para>
        /// </remarks>
        /// <param name="headerName">The header name without the colon, in any case, such as <c>Content-Type</c>,
        /// <c>location</c> or <c>ETAG</c>. Null or empty returns null.</param>
        /// <returns>The header value as sent by the server (text after the colon, trimmed), or null when absent.</returns>
        /// <seealso cref="HasHeader(string)"/>
        /// <seealso cref="Headers"/>
        public string GetHeader(string headerName)
        {
            if (Headers == null || string.IsNullOrEmpty(headerName))
                return null;

            if (Headers.TryGetValue(headerName, out var value))
                return value;

            // Headers assigned by callers may use a case-sensitive dictionary; HTTP header names are not
            foreach (var header in Headers)
            {
                if (string.Equals(header.Key, headerName, StringComparison.OrdinalIgnoreCase))
                    return header.Value;
            }

            return null;
        }

        /// <summary>
        /// True when the response contains the header <paramref name="headerName"/> (ignoring case), so you can test before
        /// reading it.
        /// </summary>
        /// <remarks>
        /// <para>Uses the same case-insensitive lookup as <see cref="GetHeader(string)"/>; <c>HasHeader(x)</c> is equivalent to
        /// <c>GetHeader(x) != null</c>.</para>
        /// <para>Example: <c>if (result.HasHeader("WWW-Authenticate")) Console.WriteLine("Server wants credentials: " + result.GetHeader("WWW-Authenticate"));</c></para>
        /// </remarks>
        /// <param name="headerName">The header name without the colon, in any case, such as <c>Set-Cookie</c>. Null or empty
        /// returns false.</param>
        /// <returns>true when the header is present (even with an empty value); otherwise false.</returns>
        /// <seealso cref="GetHeader(string)"/>
        /// <seealso cref="Headers"/>
        public bool HasHeader(string headerName)
        {
            if (Headers == null || string.IsNullOrEmpty(headerName))
                return false;

            return Headers.ContainsKey(headerName)
                || Headers.Keys.Any(k => string.Equals(k, headerName, StringComparison.OrdinalIgnoreCase));
        }

        #endregion

        #region Validation Operations - Ensure everything worked

        /// <summary>
        /// Throws <see cref="CurlHttpException"/> unless <see cref="StatusCode"/> is 200-299, otherwise returns this result -
        /// the CurlDotNet version of <c>HttpResponseMessage.EnsureSuccessStatusCode()</c>.
        /// </summary>
        /// <remarks>
        /// <para>Use it to fail fast before parsing: <c>var user = (await Curl.ExecuteAsync("curl https://api.example.com/users/42")).EnsureSuccess().ParseJson&lt;User&gt;();</c></para>
        /// <para>It is the after-the-fact equivalent of curl's <c>-f</c>/<c>--fail</c>. With <c>-f</c> in the command the
        /// transfer itself throws <see cref="CurlHttpReturnedErrorException"/> (exit code 22) and you never get a result; with
        /// EnsureSuccess you get the result first and decide. The thrown exception is <see cref="CurlDotNet.Core.CurlHttpException"/>
        /// (namespace <c>CurlDotNet.Core</c>), which carries the body and headers.</para>
        /// <para>Example: <c>try { result.EnsureSuccess(); } catch (CurlHttpException ex) when (ex.StatusCode == 404) { return null; }</c></para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <returns>This same result when the status is 2xx, for chaining.</returns>
        /// <exception cref="CurlHttpException">(status not 200-299) The message names the URL, the status and a fix for that status (credentials
        /// for 401, permissions for 403, the path for 404, waiting for 429, retrying for 5xx, <c>-L</c> for 3xx);
        /// <see cref="CurlHttpException.ResponseBody"/> and <see cref="CurlHttpException.ResponseHeaders"/> carry the response.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <example>
        /// <code language="csharp">
        /// try
        /// {
        ///     var data = result.EnsureSuccess().ParseJson&lt;Data&gt;();
        /// }
        /// catch (CurlHttpException ex) when (ex.StatusCode >= 500)
        /// {
        ///     Console.WriteLine($"Server error {ex.StatusCode} - retry later");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="IsSuccess"/>
        /// <seealso cref="EnsureStatus(int)"/>
        /// <seealso cref="EnsureContains(string)"/>
        /// <seealso cref="ErgonomicExtensions.EnsureSuccessStatusCode(CurlResult)"/>
        public CurlResult EnsureSuccess()
        {
            if (!IsSuccess)
            {
                throw new CurlHttpException(CurlException.Compose(
                    $"EnsureSuccess() failed: {DescribeRequest()} answered HTTP {StatusCode}, and only 200-299 counts as success." + BodyHint(),
                    StatusFix(StatusCode), DocLinks.HowToErrorHandling), StatusCode)
                {
                    ResponseBody = Body,
                    ResponseHeaders = Headers
                };
            }
            return this;
        }

        /// <summary>" Response body starts with ..." for messages, or empty.</summary>
        private string BodyHint() =>
            string.IsNullOrEmpty(Body) ? string.Empty : $" Response body starts with \"{ErrorText.Preview(Body, 80)}\".";

        /// <summary>How to fix a request that ended with <paramref name="status"/>.</summary>
        internal string StatusFix(int status)
        {
            if (status == 0)
                return "There was no HTTP response (status 0): check result.Exception (LibCurl API) or whether the URL is http(s); for ftp:// and file:// use StatusCode directly.";
            if (status >= 300 && status < 400)
            {
                var location = GetHeader("Location");
                return $"The server redirected{(location == null ? "" : $" to {ErrorText.SafeUrl(location)}")}; add -L to the command (or FollowRedirects()) to follow it.";
            }
            switch (status)
            {
                case 400: return "The server rejected the request as malformed: check the body (-d/--json), the Content-Type header and required fields; the response body usually says which field.";
                case 401: return "Authentication is missing or wrong: check the credentials (-u user:password, or -H \"Authorization: Bearer <token>\") and whether the token has expired.";
                case 403: return "The credentials were accepted but are not allowed to do this: check the account's permissions or the API key's scopes.";
                case 404: return "Check the URL path, the resource id and the HTTP method (-X); the resource may not exist or may have moved.";
                case 405: return "The URL does not accept this HTTP method; check -X (GET, POST, PUT, DELETE...) against the API documentation.";
                case 408: return "The server timed out waiting for the request; retry, and check that a large upload is not stalling.";
                case 409: return "The request conflicts with the current state of the resource (for example a duplicate); fetch the latest version and retry.";
                case 413: return "The request body is too large for the server; send less data or split it.";
                case 415: return "The server does not accept this Content-Type; set the right one, for example -H \"Content-Type: application/json\" or use --json.";
                case 422: return "The server understood the request but the data failed validation; read the response body for the field errors.";
                case 429:
                    var retryAfter = GetHeader("Retry-After");
                    return $"Too many requests: wait{(retryAfter == null ? "" : $" {retryAfter} second(s), as Retry-After says,")} before retrying, and slow down (use --retry, which honours Retry-After).";
            }
            if (status >= 500)
                return $"The server failed ({status}); this is not caused by your request syntax. Retry later or with --retry, and contact the API owner if it persists.";
            return $"Check the API documentation for what HTTP {status} means for this endpoint; the response body may explain it.";
        }

        /// <summary>
        /// Throws <see cref="CurlHttpException"/> unless <see cref="StatusCode"/> equals <paramref name="expectedStatus"/> exactly,
        /// otherwise returns this result - for APIs where only one code is right (201 Created, 204 No Content).
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl -X POST --json '{\"name\":\"x\"}' https://api.example.com/items")).EnsureStatus(201);</c>
        /// throws if the API answered 200 or 409 instead of 201.</para>
        /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
        /// </remarks>
        /// <param name="expectedStatus">The exact status you require, such as 200, 201 or 204.</param>
        /// <returns>This same result when the status matches, for chaining.</returns>
        /// <exception cref="CurlHttpException">(status differs from <paramref name="expectedStatus"/>) The message names the URL, the status and a fix for that status (credentials
        /// for 401, permissions for 403, the path for 404, waiting for 429, retrying for 5xx, <c>-L</c> for 3xx);
        /// <see cref="CurlHttpException.ResponseBody"/> and <see cref="CurlHttpException.ResponseHeaders"/> carry the response.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="EnsureSuccess"/>
        /// <seealso cref="StatusCode"/>
        public CurlResult EnsureStatus(int expectedStatus)
        {
            if (StatusCode != expectedStatus)
            {
                var fix = IsSuccess
                    ? $"The request succeeded with {StatusCode}, which is not the {expectedStatus} you required; if {StatusCode} is also acceptable, use EnsureSuccess() or compare StatusCode yourself."
                    : StatusFix(StatusCode);
                throw new CurlHttpException(CurlException.Compose(
                    $"EnsureStatus({expectedStatus}) failed: {DescribeRequest()} answered HTTP {StatusCode}." + BodyHint(),
                    fix, DocLinks.HowToErrorHandling), StatusCode)
                {
                    ResponseBody = Body,
                    ResponseHeaders = Headers
                };
            }
            return this;
        }

        /// <summary>
        /// Throws <see cref="InvalidOperationException"/> unless <see cref="Body"/> contains <paramref name="expectedText"/>
        /// (exact, case-sensitive), otherwise returns this result - a quick content check for smoke tests and health checks.
        /// </summary>
        /// <remarks>
        /// <para>The comparison is ordinal: <c>"OK"</c> does not match <c>"ok"</c>. Binary responses have an empty
        /// <see cref="Body"/> and never match.</para>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl https://status.example.com/health")).EnsureSuccess().EnsureContains("\"status\":\"up\"");</c></para>
        /// </remarks>
        /// <param name="expectedText">The text that must appear somewhere in <see cref="Body"/>, such as <c>"success"</c>.</param>
        /// <returns>This same result when the text is found, for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="expectedText"/> is null; pass the text to look for.</exception>
        /// <exception cref="InvalidOperationException">The body does not contain the text. The message names the URL, the
        /// status, the body length and its first characters. Fix: print <see cref="Body"/>, check case and spelling.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</exception>
        /// <seealso cref="EnsureSuccess"/>
        /// <seealso cref="Body"/>
        public CurlResult EnsureContains(string expectedText)
        {
            if (expectedText == null)
            {
                throw new ArgumentNullException(nameof(expectedText), CurlException.Compose(
                    "EnsureContains() was called with null.",
                    "Pass the text the response body must contain, for example EnsureContains(\"success\").",
                    DocLinks.HowToErrorHandling));
            }
            if (Body?.Contains(expectedText) != true)
            {
                var what = string.IsNullOrEmpty(Body)
                    ? $"EnsureContains(\"{ErrorText.Preview(expectedText, 60)}\") failed: the response body of {DescribeRequest()} is empty (HTTP {StatusCode}{(IsBinary ? $", {BinaryData.Length} bytes of binary data" : "")})."
                    : $"EnsureContains(\"{ErrorText.Preview(expectedText, 60)}\") failed: the response body of {DescribeRequest()} (HTTP {StatusCode}, {Body.Length} characters) does not contain it." + BodyHint();
                var fix = !string.IsNullOrEmpty(Body) && Body.IndexOf(expectedText, StringComparison.OrdinalIgnoreCase) >= 0
                    ? "The text is there with different upper/lower case; the check is case-sensitive, so match the case exactly."
                    : "Print result.Body to see what the server actually returned; check the URL and status first, and remember the check is exact and case-sensitive.";
                throw new InvalidOperationException(CurlException.Compose(what, fix, DocLinks.HowToErrorHandling));
            }
            return this;
        }

        #endregion

        #region Retry Operations - Try again if something went wrong

        /// <summary>
        /// Runs <see cref="Command"/> again with <see cref="Curl.ExecuteAsync(string)"/> and returns the new result - a
        /// one-line manual retry after a 429, 503 or flaky network.
        /// </summary>
        /// <remarks>
        /// <para>No delay is added and nothing about the command changes; this result is not modified. For automatic
        /// retries with back-off use curl's <c>--retry 3</c> in the command or <see cref="CurlSettings.WithRetries(int, int)"/>.
        /// Settings passed to the original call (timeouts, headers, progress) are not repeated; use
        /// <see cref="RetryWith(Action{CurlSettings})"/> to supply them again.</para>
        /// <para>Example: <c>if (result.StatusCode == 429) { await Task.Delay(5000); result = await result.Retry(); }</c></para>
        /// </remarks>
        /// <returns>The result of the new transfer.</returns>
        /// <exception cref="InvalidOperationException"><see cref="Command"/> is null or empty (the result was created by hand),
        /// so there is nothing to run again. Fix: call <c>Curl.ExecuteAsync(command)</c> yourself, or set <see cref="Command"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</exception>
        /// <exception cref="CurlException">The repeated transfer failed (DNS, connection, timeout, TLS, <c>-f</c>...), exactly as
        /// <see cref="Curl.ExecuteAsync(string)"/> would throw; see the exception's message for the fix.</exception>
        /// <seealso cref="RetryWith(Action{CurlSettings})"/>
        /// <seealso cref="Command"/>
        /// <seealso cref="CurlSettings.WithRetries(int, int)"/>
        public async Task<CurlResult> Retry()
        {
            if (string.IsNullOrEmpty(Command))
                throw new InvalidOperationException(NoCommandMessage(nameof(Retry)));

            return await Curl.ExecuteAsync(Command).ConfigureAwait(false);
        }

        private string NoCommandMessage(string method) => CurlException.Compose(
            $"Cannot retry: {method}() needs the original command, but this CurlResult (HTTP {StatusCode}) has no Command - it was created by hand or copied without it.",
            "Run the request again with Curl.ExecuteAsync(command), or set result.Command to the curl command before calling " + method + "().",
            DocLinks.HowToRunCommand);

        /// <summary>
        /// Runs <see cref="Command"/> again with <see cref="CurlSettings"/> you configure (longer timeout, extra header,
        /// retries, progress) and returns the new result.
        /// </summary>
        /// <remarks>
        /// <para><paramref name="configure"/> receives a fresh <see cref="CurlSettings"/>; whatever it sets overrides the same
        /// option in the command (for example <see cref="CurlSettings.MaxTimeSeconds"/> wins over <c>-m</c>).</para>
        /// <para>Example: <c>result = await result.RetryWith(s =&gt; s.WithTimeout(60).WithHeader("Authorization", "Bearer " + newToken));</c></para>
        /// </remarks>
        /// <param name="configure">Callback that fills in the settings for the new attempt, such as
        /// <c>s =&gt; s.WithTimeout(60)</c>; must not be null.</param>
        /// <returns>The result of the new transfer.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="configure"/> is null; pass <c>s =&gt; { }</c> to change nothing,
        /// or call <see cref="Retry"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Command"/> is null or empty (the result was created by hand),
        /// so there is nothing to run again. Fix: call <c>Curl.ExecuteAsync(command)</c> yourself, or set <see cref="Command"/>.
        /// See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md</exception>
        /// <exception cref="CurlException">The repeated transfer failed (DNS, connection, timeout, TLS, <c>-f</c>...), exactly as
        /// <see cref="Curl.ExecuteAsync(string)"/> would throw; see the exception's message for the fix.</exception>
        /// <seealso cref="Retry"/>
        /// <seealso cref="CurlSettings"/>
        public async Task<CurlResult> RetryWith(Action<CurlSettings> configure)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure), CurlException.Compose(
                    "RetryWith() was called with a null configure callback.",
                    "Pass a callback such as s => s.WithTimeout(60), or call Retry() to run the command unchanged.",
                    DocLinks.HowToRunCommand));
            }
            if (string.IsNullOrEmpty(Command))
                throw new InvalidOperationException(NoCommandMessage(nameof(RetryWith)));

            var settings = new CurlSettings();
            configure(settings);
            return await Curl.ExecuteAsync(Command, settings).ConfigureAwait(false);
        }

        #endregion

        #region Display Operations - Show results in console

        /// <summary>
        /// Writes <see cref="Body"/> to the console (standard output) followed by a newline, and returns this result -
        /// handy in scripts and quick experiments.
        /// </summary>
        /// <remarks>
        /// <para>Prints exactly <see cref="Body"/>, so binary responses print an empty line. Not for production logging; use
        /// your logger with <see cref="Body"/> instead.</para>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl https://api.github.com/zen")).PrintBody();</c></para>
        /// </remarks>
        /// <returns>This same result, for chaining.</returns>
        /// <seealso cref="Print"/>
        /// <seealso cref="PrintVerbose"/>
        public CurlResult PrintBody()
        {
            Console.WriteLine(Body);
            return this;
        }

        /// <summary>
        /// Writes <c>Status: {StatusCode}</c> and then <see cref="Body"/> to the console, and returns this result.
        /// </summary>
        /// <remarks>
        /// <para>Example: <c>result.Print();</c> prints <c>Status: 200</c> followed by the body text.</para>
        /// </remarks>
        /// <returns>This same result, for chaining.</returns>
        /// <seealso cref="PrintBody"/>
        /// <seealso cref="PrintVerbose"/>
        public CurlResult Print()
        {
            Console.WriteLine($"Status: {StatusCode}");
            Console.WriteLine(Body);
            return this;
        }

        /// <summary>
        /// Writes the status, every response header, the body and the main timings (DNS, connect, total) to the console,
        /// and returns this result - a quick look at everything that came back.
        /// </summary>
        /// <remarks>
        /// <para>This prints the parsed result; for the raw request and response exchange as curl shows it, run the command
        /// with <c>-v</c> (the log is in <see cref="Body"/>) or <c>--trace-ascii trace.txt</c>.</para>
        /// <para>Example: <c>(await Curl.ExecuteAsync("curl https://example.com")).PrintVerbose();</c></para>
        /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
        /// </remarks>
        /// <returns>This same result, for chaining.</returns>
        /// <seealso cref="Print"/>
        /// <seealso cref="Timings"/>
        public CurlResult PrintVerbose()
        {
            Console.WriteLine($"Status: {StatusCode}");
            Console.WriteLine("Headers:");
            foreach (var header in Headers)
            {
                Console.WriteLine($"  {header.Key}: {header.Value}");
            }
            Console.WriteLine("Body:");
            Console.WriteLine(Body);

            if (Timings != null)
            {
                Console.WriteLine("Timings:");
                Console.WriteLine($"  DNS: {Timings.NameLookup}ms");
                Console.WriteLine($"  Connect: {Timings.Connect}ms");
                Console.WriteLine($"  Total: {Timings.Total}ms");
            }

            return this;
        }

        #endregion

        #region Transformation Operations - Change or extract data

        /// <summary>
        /// Passes this result to <paramref name="transformer"/> and returns whatever it returns - a way to end a fluent chain
        /// with your own projection.
        /// </summary>
        /// <remarks>
        /// <para>Exceptions thrown by <paramref name="transformer"/> propagate unchanged.</para>
        /// <para>Example: <c>var summary = result.Transform(r =&gt; new { r.StatusCode, Size = r.Body?.Length ?? 0, Type = r.GetHeader("Content-Type") });</c></para>
        /// </remarks>
        /// <typeparam name="T">The type your function returns.</typeparam>
        /// <param name="transformer">A function of this result, such as <c>r =&gt; r.ParseJson&lt;User&gt;().Name</c>; must not be null.</param>
        /// <returns>The value returned by <paramref name="transformer"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="transformer"/> is null; pass a function.</exception>
        /// <seealso cref="ParseJson{T}"/>
        public T Transform<T>(Func<CurlResult, T> transformer)
        {
            if (transformer == null)
            {
                throw new ArgumentNullException(nameof(transformer), CurlException.Compose(
                    $"Transform<{TypeName(typeof(T))}>() was called with a null function.",
                    "Pass a function of the result, for example r => r.StatusCode.",
                    DocLinks.HowToParseJson));
            }
            return transformer(this);
        }

        /// <summary>
        /// Returns the response as a new readable <see cref="MemoryStream"/>: the bytes of <see cref="BinaryData"/>, or
        /// <see cref="Body"/> encoded as UTF-8, or an empty stream.
        /// </summary>
        /// <remarks>
        /// <para>The data is already in memory; this does not stream from the network. Each call returns a new stream
        /// positioned at 0; dispose it when done. Useful for APIs that take a <see cref="Stream"/>, such as
        /// <c>XDocument.Load</c>, <c>ZipArchive</c> or an image library.</para>
        /// <para>Example: <c>using var zip = new System.IO.Compression.ZipArchive((await Curl.ExecuteAsync("curl -L https://example.com/files.zip")).ToStream());</c></para>
        /// </remarks>
        /// <returns>A new <see cref="MemoryStream"/> over the response data (never null).</returns>
        /// <seealso cref="BinaryData"/>
        /// <seealso cref="Body"/>
        public Stream ToStream()
        {
            if (BinaryData != null)
            {
                return new MemoryStream(BinaryData);
            }
            else if (Body != null)
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(Body);
                return new MemoryStream(bytes);
            }
            else
            {
                return new MemoryStream();
            }
        }

        /// <summary>
        /// Keeps only the lines of <see cref="Body"/> for which <paramref name="predicate"/> returns true (this changes
        /// <see cref="Body"/> in place) and returns this result.
        /// </summary>
        /// <remarks>
        /// <para>Lines are split on <c>\n</c> and joined back with <c>\n</c>; a <c>\r</c> before the newline stays part of the
        /// line. Does nothing when <see cref="Body"/> is null. <see cref="BinaryData"/> is untouched.</para>
        /// <para>Example: <c>result.FilterLines(line =&gt; line.StartsWith("data:"))</c> keeps the data lines of a server-sent-events response.</para>
        /// </remarks>
        /// <param name="predicate">Called once per line; return true to keep it. Must not be null.</param>
        /// <returns>This same result (with the filtered <see cref="Body"/>), for chaining.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="predicate"/> is null; pass a function such as <c>l =&gt; l.Length &gt; 0</c>.</exception>
        /// <seealso cref="Body"/>
        public CurlResult FilterLines(Func<string, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate), CurlException.Compose(
                    "FilterLines() was called with a null predicate.",
                    "Pass a function that returns true for lines to keep, for example line => !string.IsNullOrWhiteSpace(line).",
                    DocLinks.HowToParseJson));
            }
            if (Body != null)
            {
                var lines = Body.Split('\n').Where(predicate);
                Body = string.Join("\n", lines);
            }
            return this;
        }

        #endregion

        #region Private Helper Methods

        private string ConvertJsonToCsv(string json)
        {
            try
            {
                var sb = new StringBuilder();

#if NETSTANDARD2_0
                var array = Newtonsoft.Json.Linq.JArray.Parse(json);
                if (array.Count == 0) return "";

                var first = array[0] as Newtonsoft.Json.Linq.JObject;
                if (first != null)
                {
                    // Headers
                    var headers = first.Properties().Select(p => p.Name).ToList();
                    sb.AppendLine(string.Join(",", headers));

                    // Rows
                    foreach (Newtonsoft.Json.Linq.JObject obj in array)
                    {
                        var values = headers.Select(h =>
                        {
                            var val = obj[h]?.ToString() ?? "";
                            if (val.Contains(",") || val.Contains("\""))
                                val = "\"" + val.Replace("\"", "\"\"") + "\"";
                            return val;
                        });
                        sb.AppendLine(string.Join(",", values));
                    }
                }
#else
                using var doc = System.Text.Json.JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind != System.Text.Json.JsonValueKind.Array)
                    return Body;

                var array = doc.RootElement.EnumerateArray().ToList();
                if (array.Count == 0) return "";

                var first = array[0];
                if (first.ValueKind == System.Text.Json.JsonValueKind.Object)
                {
                    // Headers
                    var headers = first.EnumerateObject().Select(p => p.Name).ToList();
                    sb.AppendLine(string.Join(",", headers));

                    // Rows
                    foreach (var obj in array)
                    {
                        var values = headers.Select(h =>
                        {
                            if (obj.TryGetProperty(h, out var prop))
                            {
                                var val = prop.ToString();
                                if (val.Contains(",") || val.Contains("\""))
                                    val = "\"" + val.Replace("\"", "\"\"") + "\"";
                                return val;
                            }
                            return "";
                        });
                        sb.AppendLine(string.Join(",", values));
                    }
                }
#endif

                return sb.ToString();
            }
            catch
            {
                return Body; // Return as-is if not valid JSON array
            }
        }

        #endregion
    }

    /// <summary>
    /// Where the time of a transfer went - DNS, TCP connect, TLS handshake, first byte, total - in milliseconds,
    /// matching curl's <c>-w '%{time_namelookup}'</c>, <c>%{time_connect}</c> ... <c>%{time_total}</c> (which are seconds).
    /// </summary>
    /// <remarks>
    /// <para>All values are cumulative from the start of the transfer, as in curl: <see cref="NameLookup"/> &lt;=
    /// <see cref="Connect"/> &lt;= <see cref="AppConnect"/> (HTTPS) &lt;= <see cref="StartTransfer"/> &lt;= <see cref="Total"/>.
    /// Subtract neighbours to get the length of one phase. Connection-level values (NameLookup, Connect, AppConnect) are
    /// recorded on .NET 5 and later for new connections; on .NET Standard 2.0 / .NET Framework and for reused pooled
    /// connections they are 0.</para>
    /// <para>Example: <c>var t = result.Timings; Console.WriteLine($"DNS {t.NameLookup:F0} ms, TCP {t.Connect - t.NameLookup:F0} ms, TLS {t.AppConnect - t.Connect:F0} ms, server {t.StartTransfer - Math.Max(t.AppConnect, t.Connect):F0} ms, download {t.Total - t.StartTransfer:F0} ms");</c></para>
    /// <para>Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/14-debugging-requests.md</para>
    /// </remarks>
    /// <seealso cref="CurlResult.Timings"/>
    public class CurlTimings
    {
        /// <summary>
        /// Milliseconds from the start of the transfer until the host name was resolved to an address;
        /// curl's <c>-w '%{time_namelookup}'</c> (which prints seconds).
        /// </summary>
        /// <value>Milliseconds since the transfer started; 0 when the request reused a pooled connection
        /// (no lookup happened) or on .NET Standard 2.0 / .NET Framework, where the connection is not observable.</value>
        /// <remarks>
        /// <para>Recorded by the CurlDotNet transport on .NET 5 and later for every new connection, for
        /// <c>http://</c> and <c>https://</c> alike. A literal IP address still gets a (near-zero) value.</para>
        /// <para>Example: <c>if (result.Timings.NameLookup &gt; 500) Console.WriteLine("Slow DNS - try --resolve host:443:1.2.3.4");</c>.</para>
        /// </remarks>
        /// <seealso cref="Connect"/>
        /// <seealso cref="Total"/>
        public double NameLookup { get; set; }

        /// <summary>
        /// Milliseconds from the start until the TCP connection to the server (or proxy) was established;
        /// curl's <c>-w '%{time_connect}'</c>.
        /// </summary>
        /// <value>Cumulative milliseconds since the start, so it is always at least <see cref="NameLookup"/>;
        /// 0 for a reused connection or on .NET Standard 2.0 / .NET Framework.</value>
        /// <remarks>
        /// <para>The TCP handshake alone took <c>Connect - NameLookup</c> milliseconds. A large value usually
        /// means network latency or a firewall dropping packets; see <c>--connect-timeout</c>.</para>
        /// </remarks>
        /// <seealso cref="NameLookup"/>
        /// <seealso cref="AppConnect"/>
        public double Connect { get; set; }

        /// <summary>
        /// Milliseconds from the start until the TLS handshake finished; curl's <c>-w '%{time_appconnect}'</c>.
        /// Always 0 for plain <c>http://</c>, exactly like curl.
        /// </summary>
        /// <value>Cumulative milliseconds since the start (at least <see cref="Connect"/>) for <c>https://</c>
        /// on a new connection; 0 for <c>http://</c>, reused connections and .NET Standard 2.0 / .NET Framework.</value>
        /// <remarks>
        /// <para>The TLS handshake alone took <c>AppConnect - Connect</c> milliseconds.</para>
        /// </remarks>
        /// <seealso cref="Connect"/>
        /// <seealso cref="PreTransfer"/>
        public double AppConnect { get; set; }

        /// <summary>
        /// Milliseconds from the start until the request was about to be sent; curl's <c>-w '%{time_pretransfer}'</c>.
        /// </summary>
        /// <value>Milliseconds since the start of the transfer, measured just before the request is handed
        /// to the connection.</value>
        /// <remarks>
        /// <para>On CurlDotNet the connection is opened while the request is being sent, so this value can be
        /// smaller than <see cref="Connect"/>; use <see cref="StartTransfer"/> for "time to first byte".</para>
        /// </remarks>
        /// <seealso cref="StartTransfer"/>
        public double PreTransfer { get; set; }

        /// <summary>
        /// Milliseconds from the start until the last redirect was followed (with <c>-L</c>); curl's
        /// <c>-w '%{time_redirect}'</c>.
        /// </summary>
        /// <value>0 when no redirect was followed.</value>
        /// <remarks>
        /// <para>Example: <c>curl -L https://bit.ly/x</c> then <c>result.Timings.Redirect</c> tells how long the
        /// redirect hops took before the final request started.</para>
        /// </remarks>
        /// <seealso cref="Total"/>
        public double Redirect { get; set; }

        /// <summary>
        /// Milliseconds from the start until the response headers arrived ("time to first byte");
        /// curl's <c>-w '%{time_starttransfer}'</c>.
        /// </summary>
        /// <value>Milliseconds since the start; includes DNS, connect, TLS, sending the request and the
        /// server's think time.</value>
        /// <remarks>
        /// <para><c>StartTransfer - AppConnect</c> (or <c>- Connect</c> for http) is roughly how long the server
        /// took to answer. The body download then takes <c>Total - StartTransfer</c>.</para>
        /// </remarks>
        /// <seealso cref="Total"/>
        /// <seealso cref="PreTransfer"/>
        public double StartTransfer { get; set; }

        /// <summary>
        /// Milliseconds the whole transfer took, body included; curl's <c>-w '%{time_total}'</c>.
        /// </summary>
        /// <value>Milliseconds since the start of the transfer, measured after the body was read.</value>
        /// <remarks>
        /// <para>Example: <c>Console.WriteLine($"{result.Timings.Total:F0} ms (DNS {result.Timings.NameLookup:F0}, connect {result.Timings.Connect:F0}, TLS {result.Timings.AppConnect:F0}, first byte {result.Timings.StartTransfer:F0})");</c></para>
        /// <para>To get curl's seconds instead, divide by 1000 or run the command with
        /// <c>-w '%{time_total}'</c> and read it from <see cref="CurlResult.Body"/>.</para>
        /// </remarks>
        /// <seealso cref="StartTransfer"/>
        /// <seealso cref="NameLookup"/>
        public double Total { get; set; }
    }

    /// <summary>
    /// Thrown by <see cref="CurlResult.EnsureSuccess"/> and <see cref="CurlResult.EnsureStatus(int)"/> when the server answered
    /// with a status you did not accept; carries the status, the response body and the headers so you can react.
    /// </summary>
    /// <remarks>
    /// <para>This is <c>CurlDotNet.Core.CurlHttpException</c>, a plain <see cref="Exception"/>. It is a different type from
    /// <see cref="CurlDotNet.Exceptions.CurlHttpException"/>, which is a <see cref="CurlException"/> thrown during the transfer
    /// (for example <see cref="CurlHttpReturnedErrorException"/> when the command uses <c>-f</c>). With both namespaces
    /// imported, write the namespace you mean.</para>
    /// <para>Its message says what happened (URL and status), how to fix it for that status, and links the error-handling guide.</para>
    /// <para>Example: <c>try { result.EnsureSuccess(); } catch (CurlDotNet.Core.CurlHttpException ex) when (ex.StatusCode == 401) { await RefreshTokenAsync(); }</c></para>
    /// <para>Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md</para>
    /// </remarks>
    /// <seealso cref="CurlResult.EnsureSuccess"/>
    /// <seealso cref="CurlResult.EnsureStatus(int)"/>
    /// <seealso cref="CurlDotNet.Exceptions.CurlHttpException"/>
    public class CurlHttpException : Exception
    {
        /// <summary>
        /// The HTTP status the server answered with (for example 404 or 503), copied from <see cref="CurlResult.StatusCode"/>.
        /// </summary>
        /// <value>The status code; 0 when the request had no HTTP response.</value>
        /// <remarks>
        /// <para>Example: <c>catch (CurlHttpException ex) when (ex.StatusCode &gt;= 500) { /* retry later */ }</c></para>
        /// </remarks>
        /// <seealso cref="CurlResult.StatusCode"/>
        /// <seealso cref="ResponseBody"/>
        public int StatusCode { get; }

        /// <summary>
        /// The response text (<see cref="CurlResult.Body"/>) of the failed request - APIs usually explain the error here.
        /// </summary>
        /// <value>The body text, empty for binary or empty responses, or null when the exception was created without it.</value>
        /// <remarks>
        /// <para>Example: <c>catch (CurlHttpException ex) { logger.LogError("{Status}: {Body}", ex.StatusCode, ex.ResponseBody); }</c></para>
        /// </remarks>
        /// <seealso cref="CurlResult.Body"/>
        /// <seealso cref="ResponseHeaders"/>
        public string ResponseBody { get; set; }

        /// <summary>
        /// The response headers (<see cref="CurlResult.Headers"/>) of the failed request, such as <c>Retry-After</c> or
        /// <c>WWW-Authenticate</c>.
        /// </summary>
        /// <value>The result's case-insensitive header dictionary, or null when the exception was created without it.</value>
        /// <remarks>
        /// <para>Example: <c>if (ex.ResponseHeaders?.TryGetValue("Retry-After", out var wait) == true) await Task.Delay(TimeSpan.FromSeconds(int.Parse(wait)));</c></para>
        /// </remarks>
        /// <seealso cref="CurlResult.Headers"/>
        /// <seealso cref="ResponseBody"/>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Creates the exception with your message and status; <see cref="ResponseBody"/> and <see cref="ResponseHeaders"/>
        /// can be set with an object initializer.
        /// </summary>
        /// <remarks>
        /// <para>The message is used exactly as given. Write it as what happened, how to fix it and a link, like the
        /// library's own messages.</para>
        /// <para>Example: <c>throw new CurlHttpException($"Order API answered {r.StatusCode}.", r.StatusCode) { ResponseBody = r.Body, ResponseHeaders = r.Headers };</c></para>
        /// </remarks>
        /// <param name="message">The text returned by <see cref="Exception.Message"/>.</param>
        /// <param name="statusCode">The HTTP status code, such as 404, stored in <see cref="StatusCode"/>.</param>
        /// <seealso cref="CurlResult.EnsureSuccess"/>
        public CurlHttpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

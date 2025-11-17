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
    /// <para><b>🎯 The response from your curl command - everything you need is here!</b></para>
    ///
    /// <para>After running any curl command, you get this object back. It has the status code,
    /// response body, headers, and helpful methods to work with the data.</para>
    ///
    /// <para><b>The API is designed to be intuitive - just type what you want to do:</b></para>
    /// <list type="bullet">
    /// <item>Want the body? → <c>result.Body</c></item>
    /// <item>Want JSON? → <c>result.ParseJson&lt;T&gt;()</c> or <c>result.AsJson&lt;T&gt;()</c></item>
    /// <item>Want to save? → <c>result.SaveToFile("path")</c></item>
    /// <item>Want headers? → <c>result.Headers["Content-Type"]</c></item>
    /// <item>Check success? → <c>result.IsSuccess</c> or <c>result.EnsureSuccess()</c></item>
    /// </list>
    ///
    /// <para><b>Quick Example:</b></para>
    /// <code>
    /// var result = await Curl.Execute("curl https://api.github.com/users/octocat");
    ///
    /// if (result.IsSuccess)  // Was it 200-299?
    /// {
    ///     var user = result.ParseJson&lt;User&gt;();  // Parse JSON to your type
    ///     result.SaveToFile("user.json");       // Save for later
    /// }
    /// </code>
    /// </summary>
    /// <remarks>
    /// <para><b>Design Philosophy:</b> Every method name tells you exactly what it does.
    /// No surprises. If you guess a method name, it probably exists and does what you expect.</para>
    ///
    /// <para><b>Fluent API:</b> Most methods return 'this' so you can chain operations:</para>
    /// <code>
    /// result
    ///     .EnsureSuccess()           // Throw if not 200-299
    ///     .SaveToFile("backup.json") // Save a copy
    ///     .ParseJson&lt;Data&gt;()        // Parse and return data
    /// </code>
    /// </remarks>
    public class CurlResult
    {
        #region Core Properties - The basics everyone needs

        /// <summary>
        /// <para><b>The HTTP status code - tells you what happened.</b></para>
        ///
        /// <para>Common codes you'll see:</para>
        /// <code>
        /// 200 = OK, it worked!
        /// 201 = Created something new
        /// 204 = Success, but no content returned
        /// 400 = Bad request (you sent something wrong)
        /// 401 = Unauthorized (need to login)
        /// 403 = Forbidden (not allowed)
        /// 404 = Not found
        /// 429 = Too many requests (slow down!)
        /// 500 = Server error (their fault, not yours)
        /// 503 = Service unavailable (try again later)
        /// </code>
        ///
        /// <para><b>Example - Handle different statuses:</b></para>
        /// <code>
        /// switch (result.StatusCode)
        /// {
        ///     case 200: ProcessData(result.Body); break;
        ///     case 404: Console.WriteLine("Not found"); break;
        ///     case 401: RedirectToLogin(); break;
        ///     case >= 500: Console.WriteLine("Server error, retry later"); break;
        /// }
        /// </code>
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// <para><b>The response body as a string - this is your data!</b></para>
        ///
        /// <para>Contains whatever the server sent back: JSON, HTML, XML, plain text, etc.</para>
        ///
        /// <para><b>Common patterns:</b></para>
        /// <code>
        /// // JSON API response (most common)
        /// if (result.Body.StartsWith("{"))
        /// {
        ///     var data = result.ParseJson&lt;MyClass&gt;();
        /// }
        ///
        /// // HTML webpage
        /// if (result.Body.Contains("&lt;html"))
        /// {
        ///     result.SaveToFile("page.html");
        /// }
        ///
        /// // Plain text
        /// Console.WriteLine(result.Body);
        /// </code>
        ///
        /// <para><b>Note:</b> For binary data (images, PDFs), use <see cref="BinaryData"/> instead.</para>
        /// <para><b>Note:</b> Can be null for 204 No Content or binary responses.</para>
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// <para><b>All HTTP headers from the response - contains metadata about the response.</b></para>
        ///
        /// <para>Headers tell you things like content type, cache rules, rate limits, etc.
        /// Access them like a dictionary (case-insensitive keys).</para>
        ///
        /// <para><b>Get a specific header:</b></para>
        /// <code>
        /// // These all work (case-insensitive):
        /// var type = result.Headers["Content-Type"];
        /// var type = result.Headers["content-type"];
        /// var type = result.Headers["CONTENT-TYPE"];
        ///
        /// // Or use the helper:
        /// var type = result.GetHeader("Content-Type");
        /// </code>
        ///
        /// <para><b>Check rate limits (common in APIs):</b></para>
        /// <code>
        /// if (result.Headers.ContainsKey("X-RateLimit-Remaining"))
        /// {
        ///     var remaining = int.Parse(result.Headers["X-RateLimit-Remaining"]);
        ///     if (remaining &lt; 10)
        ///         Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
        /// }
        /// </code>
        ///
        /// <para><b>Common headers:</b></para>
        /// <list type="bullet">
        /// <item><b>Content-Type</b> - Format of the data (application/json, text/html)</item>
        /// <item><b>Content-Length</b> - Size in bytes</item>
        /// <item><b>Location</b> - Where you got redirected to</item>
        /// <item><b>Set-Cookie</b> - Cookies to store</item>
        /// <item><b>Cache-Control</b> - How long to cache</item>
        /// </list>
        /// </summary>
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// <para><b>Quick success check - true if status is 200-299.</b></para>
        ///
        /// <para>The easiest way to check if your request worked:</para>
        /// <code>
        /// if (result.IsSuccess)
        /// {
        ///     // It worked! Do something with result.Body
        /// }
        /// else
        /// {
        ///     // Something went wrong, check result.StatusCode
        /// }
        /// </code>
        ///
        /// <para>What's considered success: 200 OK, 201 Created, 204 No Content, etc.</para>
        /// <para>What's NOT success: 404 Not Found, 500 Server Error, etc.</para>
        /// </summary>
        public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

        /// <summary>
        /// <para><b>Binary data for files like images, PDFs, downloads.</b></para>
        ///
        /// <para>When you download non-text files, the bytes are here:</para>
        /// <code>
        /// // Download an image
        /// var result = await Curl.Execute("curl https://example.com/logo.png");
        ///
        /// if (result.IsBinary)
        /// {
        ///     File.WriteAllBytes("logo.png", result.BinaryData);
        ///     Console.WriteLine($"Saved {result.BinaryData.Length} bytes");
        /// }
        /// </code>
        /// </summary>
        public byte[] BinaryData { get; set; }

        /// <summary>
        /// <para><b>Is this binary data? (images, PDFs, etc.)</b></para>
        ///
        /// <para>Quick check before accessing BinaryData:</para>
        /// <code>
        /// if (result.IsBinary)
        ///     File.WriteAllBytes("file.bin", result.BinaryData);
        /// else
        ///     File.WriteAllText("file.txt", result.Body);
        /// </code>
        /// </summary>
        public bool IsBinary => BinaryData != null && BinaryData.Length > 0;

        /// <summary>
        /// <para><b>The original curl command that was executed.</b></para>
        ///
        /// <para>Useful for debugging or retrying:</para>
        /// <code>
        /// Console.WriteLine($"Executed: {result.Command}");
        ///
        /// // Retry the same command
        /// var retry = await Curl.Execute(result.Command);
        /// </code>
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// <para><b>Detailed timing information (like curl -w).</b></para>
        ///
        /// <para>See how long each phase took:</para>
        /// <code>
        /// Console.WriteLine($"DNS lookup: {result.Timings.NameLookup}ms");
        /// Console.WriteLine($"Connect: {result.Timings.Connect}ms");
        /// Console.WriteLine($"Total: {result.Timings.Total}ms");
        /// </code>
        /// </summary>
        public CurlTimings Timings { get; set; }

        /// <summary>
        /// <para><b>Files that were saved (if using -o flag).</b></para>
        ///
        /// <para>Track what files were created:</para>
        /// <code>
        /// foreach (var file in result.OutputFiles)
        /// {
        ///     Console.WriteLine($"Saved: {file}");
        /// }
        /// </code>
        /// </summary>
        public List<string> OutputFiles { get; set; } = new List<string>();

        /// <summary>
        /// <para><b>Any exception if the request failed completely.</b></para>
        ///
        /// <para>Only set for network failures, not HTTP errors:</para>
        /// <code>
        /// if (result.Exception != null)
        /// {
        ///     // Network/DNS/Timeout failure
        ///     Console.WriteLine($"Failed: {result.Exception.Message}");
        /// }
        /// else if (!result.IsSuccess)
        /// {
        ///     // HTTP error (404, 500, etc)
        ///     Console.WriteLine($"HTTP {result.StatusCode}");
        /// }
        /// </code>
        /// </summary>
        public Exception Exception { get; set; }

        #endregion

        #region JSON Operations - Working with JSON responses

        /// <summary>
        /// <para><b>Parse the JSON response into your C# class.</b></para>
        ///
        /// <para>The most common operation - turning JSON into objects. This method uses <see cref="System.Text.Json.JsonSerializer"/> 
        /// in .NET 6+ and <see cref="Newtonsoft.Json.JsonConvert"/> in .NET Standard 2.0 for maximum compatibility.</para>
        ///
        /// <para><b>Example:</b></para>
        /// <code language="csharp">
        /// // Define your class matching the JSON structure
        /// public class User
        /// {
        ///     public string Name { get; set; }
        ///     public string Email { get; set; }
        ///     public int Id { get; set; }
        /// }
        ///
        /// // Parse the response
        /// var user = result.ParseJson&lt;User&gt;();
        /// Console.WriteLine($"Hello {user.Name}!");
        ///
        /// // Or parse arrays
        /// var users = result.ParseJson&lt;List&lt;User&gt;&gt;();
        /// Console.WriteLine($"Found {users.Count} users");
        /// </code>
        ///
        /// <para><b>Tip:</b> Use https://json2csharp.com to generate C# classes from JSON!</para>
        /// </summary>
        /// <typeparam name="T">The type to deserialize to. Must match the JSON structure. Can be a class, struct, or collection type like <see cref="List{T}"/> or <see cref="Dictionary{TKey, TValue}"/>.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/> with data from the JSON <see cref="Body"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="Body"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when JSON deserialization fails or JSON doesn't match type <typeparamref name="T"/>.</exception>
        /// <exception cref="JsonException">Thrown when the JSON syntax is invalid. See <see cref="Exception.InnerException"/> for details.</exception>
        /// <remarks>
        /// <para>This method automatically detects whether to use System.Text.Json or Newtonsoft.Json based on the target framework.</para>
        /// <para>For complex JSON structures, consider using <see cref="AsJsonDynamic"/> for exploration, then creating a typed class.</para>
        /// <para>If <paramref name="T"/> doesn't match the JSON structure, properties that don't match will be left at their default values.</para>
        /// </remarks>
        /// <seealso cref="AsJson{T}">Alternative method name for parsing JSON</seealso>
        /// <seealso cref="AsJsonDynamic">Parse JSON as dynamic object without a class</seealso>
        /// <seealso cref="Body">The JSON string being parsed</seealso>
        public T ParseJson<T>()
        {
            if (string.IsNullOrEmpty(Body))
                throw new InvalidOperationException("Cannot parse JSON: Body is empty");

            try
            {
                #if NETSTANDARD2_0
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(Body);
                #else
                return System.Text.Json.JsonSerializer.Deserialize<T>(Body);
                #endif
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse JSON as {typeof(T).Name}: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// <para><b>Parse JSON response (alternative name for <see cref="ParseJson{T}"/>).</b></para>
        ///
        /// <para>Some people prefer <c>AsJson</c>, some prefer <c>ParseJson</c>. Both methods are identical and produce the same result.</para>
        /// <code language="csharp">
        /// var data = result.AsJson&lt;MyData&gt;();
        /// // Exactly the same as: result.ParseJson&lt;MyData&gt;()
        /// </code>
        /// </summary>
        /// <typeparam name="T">The type to deserialize to. See <see cref="ParseJson{T}"/> for details.</typeparam>
        /// <returns>An instance of <typeparamref name="T"/> with data from the JSON <see cref="Body"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="Body"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when JSON deserialization fails.</exception>
        /// <exception cref="JsonException">Thrown when the JSON syntax is invalid.</exception>
        /// <remarks>
        /// <para>This is simply an alias for <see cref="ParseJson{T}"/>. Use whichever method name you prefer.</para>
        /// </remarks>
        /// <seealso cref="ParseJson{T}">Primary method for parsing JSON</seealso>
        /// <seealso cref="AsJsonDynamic">Parse JSON as dynamic without a class</seealso>
        public T AsJson<T>() => ParseJson<T>();

        /// <summary>
        /// <para><b>Parse JSON as dynamic object (when you don't have a class).</b></para>
        ///
        /// <para>Useful for quick exploration or simple JSON structures. This method returns a dynamic object that allows 
        /// you to access JSON properties without defining a C# class. However, there's no compile-time checking, so prefer 
        /// <see cref="ParseJson{T}"/> with typed classes when possible.</para>
        ///
        /// <para><b>Example:</b></para>
        /// <code language="csharp">
        /// dynamic json = result.AsJsonDynamic();
        /// Console.WriteLine(json.name);           // Access properties directly
        /// Console.WriteLine(json.users[0].email); // Navigate arrays
        ///
        /// // Iterate dynamic arrays
        /// foreach (var item in json.items)
        /// {
        ///     Console.WriteLine(item.title);
        /// }
        /// </code>
        /// </summary>
        /// <returns>
        /// <para>A dynamic object representing the JSON. In .NET 6+, this is a <see cref="System.Text.Json.JsonDocument"/>.
        /// In .NET Standard 2.0, this is a <c>JObject</c> from Newtonsoft.Json.</para>
        /// <para>Access properties like: <c>dynamicObj.propertyName</c> or <c>dynamicObj["propertyName"]</c></para>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="Body"/> is null or empty.</exception>
        /// <exception cref="JsonException">Thrown when the JSON syntax is invalid.</exception>
        /// <remarks>
        /// <para><b>⚠️ Warning:</b> No compile-time checking! If a property doesn't exist, you'll get a runtime exception.</para>
        /// <para>For production code, prefer <see cref="ParseJson{T}"/> with typed classes for better safety and IntelliSense support.</para>
        /// <para>This method is useful for:</para>
        /// <list type="bullet">
        /// <item>Quick prototyping and exploration</item>
        /// <item>Working with highly dynamic JSON structures</item>
        /// <item>One-off scripts and tools</item>
        /// </list>
        /// </remarks>
        /// <seealso cref="ParseJson{T}">Parse JSON into typed classes (recommended)</seealso>
        /// <seealso cref="AsJson{T}">Alternative method name for typed parsing</seealso>
        /// <seealso cref="Body">The JSON string being parsed</seealso>
        public dynamic AsJsonDynamic()
        {
            #if NETSTANDARD2_0
            return Newtonsoft.Json.JsonConvert.DeserializeObject(Body);
            #else
            return System.Text.Json.JsonDocument.Parse(Body);
            #endif
        }

        #endregion

        #region Save Operations - Save responses to files

        /// <summary>
        /// <para><b>Save the response to a file - works for both text and binary!</b></para>
        ///
        /// <para>Smart saving - automatically handles text vs binary:</para>
        /// <code>
        /// // Save any response
        /// result.SaveToFile("output.txt");     // Text saved as text
        /// result.SaveToFile("image.png");      // Binary saved as binary
        ///
        /// // Chain operations (returns this)
        /// result
        ///     .SaveToFile("backup.json")       // Save a backup
        ///     .ParseJson&lt;Data&gt;();              // Then parse it
        /// </code>
        ///
        /// <para><b>Path examples:</b></para>
        /// <code>
        /// result.SaveToFile("file.txt");              // Current directory
        /// result.SaveToFile("data/file.txt");         // Relative path
        /// result.SaveToFile(@"C:\temp\file.txt");     // Absolute path
        /// result.SaveToFile("/home/user/file.txt");   // Linux/Mac path
        /// </code>
        /// </summary>
        /// <param name="filePath">Where to save the file</param>
        /// <returns>This result (for chaining)</returns>
        /// <example>
        /// <code language="csharp">
        /// // Download and save JSON response
        /// var result = await Curl.ExecuteAsync("curl https://api.example.com/data.json");
        /// result.SaveToFile("data.json");
        /// // File is now saved to disk AND still available in result.Body
        ///
        /// // Download image and save
        /// var result = await Curl.ExecuteAsync("curl https://example.com/logo.png");
        /// result.SaveToFile("logo.png");
        /// Console.WriteLine($"Saved {result.BinaryData.Length} bytes");
        ///
        /// // Chain with parsing
        /// var result = await Curl.ExecuteAsync("curl https://api.example.com/users");
        /// var users = result
        ///     .SaveToFile("backup-users.json")  // Save backup
        ///     .ParseJson&lt;List&lt;User&gt;&gt;();    // Then parse
        ///
        /// // Save with relative path
        /// result.SaveToFile("downloads/report.pdf");
        ///
        /// // Save with absolute path
        /// result.SaveToFile(@"C:\Temp\output.txt");  // Windows
        /// result.SaveToFile("/tmp/output.txt");       // Linux/Mac
        /// </code>
        /// </example>
        /// <seealso cref="SaveToFileAsync">Async version that doesn't block</seealso>
        /// <seealso cref="SaveAsJson">Save JSON with formatting</seealso>
        /// <seealso cref="AppendToFile">Append to existing file instead of overwriting</seealso>
        public CurlResult SaveToFile(string filePath)
        {
            if (BinaryData != null)
                File.WriteAllBytes(filePath, BinaryData);
            else
                File.WriteAllText(filePath, Body ?? "");

            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// <para><b>Save the response to a file asynchronously.</b></para>
        ///
        /// <para>Same as SaveToFile but doesn't block:</para>
        /// <code>
        /// await result.SaveToFileAsync("large-file.json");
        ///
        /// // Or chain async operations
        /// await result
        ///     .SaveToFileAsync("backup.json")
        ///     .ContinueWith(_ => Console.WriteLine("Saved!"));
        /// </code>
        /// </summary>
        public async Task<CurlResult> SaveToFileAsync(string filePath)
        {
#if NETSTANDARD2_0
            // .NET Standard 2.0 doesn't have async file methods
            await Task.Run(() =>
            {
                if (BinaryData != null)
                    File.WriteAllBytes(filePath, BinaryData);
                else
                    File.WriteAllText(filePath, Body ?? "");
            });
#else
            if (BinaryData != null)
                await File.WriteAllBytesAsync(filePath, BinaryData);
            else
                await File.WriteAllTextAsync(filePath, Body ?? "");
#endif

            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// <para><b>Save as formatted JSON file (pretty-printed).</b></para>
        ///
        /// <para>Makes JSON human-readable with indentation:</para>
        /// <code>
        /// // Save with nice formatting
        /// result.SaveAsJson("data.json");           // Pretty-printed
        /// result.SaveAsJson("data.json", false);    // Minified
        ///
        /// // Before: {"name":"John","age":30}
        /// // After:  {
        /// //           "name": "John",
        /// //           "age": 30
        /// //         }
        /// </code>
        /// </summary>
        /// <param name="filePath">Where to save the JSON file</param>
        /// <param name="indented">true for pretty formatting (default), false for minified</param>
        /// <returns>This result (for chaining)</returns>
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

            File.WriteAllText(filePath, formatted);
            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// <para><b>Save JSON response as CSV file (for JSON arrays).</b></para>
        ///
        /// <para>Converts JSON arrays to CSV for Excel:</para>
        /// <code>
        /// // JSON: [{"name":"John","age":30}, {"name":"Jane","age":25}]
        /// result.SaveAsCsv("users.csv");
        ///
        /// // Creates CSV:
        /// // name,age
        /// // John,30
        /// // Jane,25
        ///
        /// // Open in Excel
        /// Process.Start("users.csv");
        /// </code>
        ///
        /// <para><b>Note:</b> Only works with JSON arrays of objects.</para>
        /// </summary>
        public CurlResult SaveAsCsv(string filePath)
        {
            var csv = ConvertJsonToCsv(Body);
            File.WriteAllText(filePath, csv);
            OutputFiles.Add(filePath);
            return this;
        }

        /// <summary>
        /// <para><b>Append response to an existing file.</b></para>
        ///
        /// <para>Add to a file without overwriting:</para>
        /// <code>
        /// // Log all responses
        /// result.AppendToFile("api-log.txt");
        ///
        /// // Build up a file over time
        /// foreach (var url in urls)
        /// {
        ///     var r = await Curl.Execute($"curl {url}");
        ///     r.AppendToFile("combined.txt");
        /// }
        /// </code>
        /// </summary>
        public CurlResult AppendToFile(string filePath)
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
            return this;
        }

        #endregion

        #region Header Operations - Working with HTTP headers

        /// <summary>
        /// <para><b>Get a specific header value (case-insensitive).</b></para>
        ///
        /// <para>Easy header access with null safety. This matches curl's header behavior exactly.</para>
        /// </summary>
        /// <param name="headerName">Name of the header (case doesn't matter)</param>
        /// <returns>Header value or null if not found</returns>
        /// <example>
        /// <code language="csharp">
        /// // Get content type
        /// var contentType = result.GetHeader("Content-Type");
        /// if (contentType?.Contains("json") == true)
        /// {
        ///     var data = result.ParseJson&lt;MyData&gt;();
        /// }
        ///
        /// // Check rate limits (common in APIs)
        /// var remaining = result.GetHeader("X-RateLimit-Remaining");
        /// if (remaining != null &amp;&amp; int.Parse(remaining) &lt; 10)
        /// {
        ///     Console.WriteLine("⚠️ Only {0} API calls left!", remaining);
        /// }
        ///
        /// // Check cache control
        /// var cacheControl = result.GetHeader("Cache-Control");
        /// if (cacheControl?.Contains("no-cache") == true)
        /// {
        ///     Console.WriteLine("Response should not be cached");
        /// }
        ///
        /// // Get redirect location
        /// var location = result.GetHeader("Location");
        /// if (location != null)
        /// {
        ///     Console.WriteLine($"Redirected to: {location}");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="HasHeader">Check if header exists</seealso>
        /// <seealso cref="Headers">Access all headers as dictionary</seealso>
        public string GetHeader(string headerName)
        {
            return Headers.TryGetValue(headerName, out var value) ? value : null;
        }

        /// <summary>
        /// <para><b>Check if a header exists.</b></para>
        ///
        /// <para>Test for header presence before accessing. This is case-insensitive, matching curl's behavior.</para>
        /// </summary>
        /// <param name="headerName">Name of the header to check (case-insensitive)</param>
        /// <returns>true if the header exists, false otherwise</returns>
        /// <example>
        /// <code language="csharp">
        /// // Check for cookies
        /// if (result.HasHeader("Set-Cookie"))
        /// {
        ///     var cookie = result.GetHeader("Set-Cookie");
        ///     Console.WriteLine($"Cookie received: {cookie}");
        /// }
        ///
        /// // Check for authentication requirements
        /// if (result.HasHeader("WWW-Authenticate"))
        /// {
        ///     Console.WriteLine("Authentication required");
        /// }
        ///
        /// // Check for custom headers
        /// if (result.HasHeader("X-Custom-Header"))
        /// {
        ///     var value = result.GetHeader("X-Custom-Header");
        ///     ProcessCustomValue(value);
        /// }
        ///
        /// // Conditional logic based on headers
        /// if (result.HasHeader("Content-Encoding") &amp;&amp; 
        ///     result.GetHeader("Content-Encoding").Contains("gzip"))
        /// {
        ///     Console.WriteLine("Response is gzip compressed");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="GetHeader">Get header value</seealso>
        /// <seealso cref="Headers">Access all headers</seealso>
        public bool HasHeader(string headerName)
        {
            return Headers.ContainsKey(headerName);
        }

        #endregion

        #region Validation Operations - Ensure everything worked

        /// <summary>
        /// <para><b>Throw an exception if the request wasn't successful (not 200-299).</b></para>
        ///
        /// <para>Use this when you expect success and want to fail fast. This matches curl's <c>-f</c> (fail) flag behavior.</para>
        /// </summary>
        /// <returns>This result if successful (for chaining)</returns>
        /// <exception cref="CurlHttpException">Thrown if status is not 200-299. The exception contains <see cref="CurlHttpException.StatusCode"/> and <see cref="CurlHttpException.ResponseBody"/>.</exception>
        /// <example>
        /// <code language="csharp">
        /// // Fail fast pattern
        /// try
        /// {
        ///     var data = result
        ///         .EnsureSuccess()      // Throws if not 200-299
        ///         .ParseJson&lt;Data&gt;();  // Only runs if successful
        /// }
        /// catch (CurlHttpException ex)
        /// {
        ///     Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
        ///     Console.WriteLine($"Response body: {ex.ResponseBody}");
        /// }
        ///
        /// // Common API pattern - get user data
        /// var user = (await Curl.ExecuteAsync("curl https://api.example.com/user/123"))
        ///     .EnsureSuccess()        // Throws on 404, 500, etc.
        ///     .ParseJson&lt;User&gt;();    // Safe to parse, we know it's 200
        ///
        /// // Chain multiple operations
        /// var response = await Curl.ExecuteAsync("curl https://api.example.com/data");
        /// var processed = response
        ///     .EnsureSuccess()           // Ensure 200-299
        ///     .SaveToFile("backup.json") // Save backup
        ///     .ParseJson&lt;DataModel&gt;(); // Then parse
        ///
        /// // Different handling for different status codes
        /// try
        /// {
        ///     result.EnsureSuccess();
        ///     ProcessData(result.Body);
        /// }
        /// catch (CurlHttpException ex) when (ex.StatusCode == 404)
        /// {
        ///     Console.WriteLine("Resource not found");
        /// }
        /// catch (CurlHttpException ex) when (ex.StatusCode >= 500)
        /// {
        ///     Console.WriteLine("Server error - retry later");
        /// }
        /// </code>
        /// </example>
        /// <seealso cref="EnsureStatus">Ensure specific status code</seealso>
        /// <seealso cref="EnsureContains">Ensure response contains text</seealso>
        /// <seealso cref="IsSuccess">Check success without throwing</seealso>
        public CurlResult EnsureSuccess()
        {
            if (!IsSuccess)
            {
                throw new CurlHttpException($"HTTP request failed with status {StatusCode}", StatusCode)
                {
                    ResponseBody = Body,
                    ResponseHeaders = Headers
                };
            }
            return this;
        }

        /// <summary>
        /// <para><b>Throw if status doesn't match what you expect.</b></para>
        ///
        /// <para>Validate specific status codes:</para>
        /// <code>
        /// // Expect 201 Created
        /// result.EnsureStatus(201);
        ///
        /// // Expect 204 No Content
        /// result.EnsureStatus(204);
        /// </code>
        /// </summary>
        /// <param name="expectedStatus">The status code you expect</param>
        /// <returns>This result if status matches (for chaining)</returns>
        /// <exception cref="CurlHttpException">Thrown if status doesn't match</exception>
        public CurlResult EnsureStatus(int expectedStatus)
        {
            if (StatusCode != expectedStatus)
            {
                throw new CurlHttpException(
                    $"Expected status {expectedStatus} but got {StatusCode}",
                    StatusCode);
            }
            return this;
        }

        /// <summary>
        /// <para><b>Throw if response body doesn't contain expected text.</b></para>
        ///
        /// <para>Validate response content:</para>
        /// <code>
        /// // Make sure we got the right response
        /// result.EnsureContains("success");
        ///
        /// // Check for error messages
        /// if (result.Body.Contains("error"))
        /// {
        ///     result.EnsureContains("recoverable");  // Make sure it's recoverable
        /// }
        /// </code>
        /// </summary>
        public CurlResult EnsureContains(string expectedText)
        {
            if (Body?.Contains(expectedText) != true)
            {
                throw new InvalidOperationException($"Response does not contain '{expectedText}'");
            }
            return this;
        }

        #endregion

        #region Retry Operations - Try again if something went wrong

        /// <summary>
        /// <para><b>Retry the same curl command again.</b></para>
        ///
        /// <para>Simple retry for transient failures:</para>
        /// <code>
        /// // First attempt
        /// var result = await Curl.Execute("curl https://flaky-api.example.com");
        ///
        /// // Retry if it failed
        /// if (!result.IsSuccess)
        /// {
        ///     result = await result.Retry();
        /// }
        ///
        /// // Retry with delay
        /// if (result.StatusCode == 429)  // Too many requests
        /// {
        ///     await Task.Delay(5000);
        ///     result = await result.Retry();
        /// }
        /// </code>
        /// </summary>
        /// <returns>New result from retrying the command</returns>
        public async Task<CurlResult> Retry()
        {
            if (string.IsNullOrEmpty(Command))
                throw new InvalidOperationException("Cannot retry: Original command not available");

            return await Curl.ExecuteAsync(Command);
        }

        /// <summary>
        /// <para><b>Retry with modifications to the original command.</b></para>
        ///
        /// <para>Retry with different settings:</para>
        /// <code>
        /// // Retry with longer timeout
        /// var result = await result.RetryWith(settings =>
        /// {
        ///     settings.Timeout = TimeSpan.FromSeconds(60);
        /// });
        ///
        /// // Retry with authentication
        /// var result = await result.RetryWith(settings =>
        /// {
        ///     settings.AddHeader("Authorization", "Bearer " + token);
        /// });
        /// </code>
        /// </summary>
        public async Task<CurlResult> RetryWith(Action<CurlSettings> configure)
        {
            if (string.IsNullOrEmpty(Command))
                throw new InvalidOperationException("Cannot retry: Original command not available");

            var settings = new CurlSettings();
            configure(settings);
            return await Curl.ExecuteAsync(Command, settings);
        }

        #endregion

        #region Display Operations - Show results in console

        /// <summary>
        /// <para><b>Print the response body to console.</b></para>
        ///
        /// <para>Quick debugging output:</para>
        /// <code>
        /// result.PrintBody();  // Just prints the body
        ///
        /// // Chain with other operations
        /// result
        ///     .PrintBody()           // Debug output
        ///     .SaveToFile("out.txt") // Also save it
        ///     .ParseJson&lt;Data&gt;();   // Then parse
        /// </code>
        /// </summary>
        /// <returns>This result (for chaining)</returns>
        public CurlResult PrintBody()
        {
            Console.WriteLine(Body);
            return this;
        }

        /// <summary>
        /// <para><b>Print status code and body to console.</b></para>
        ///
        /// <para>More detailed debug output:</para>
        /// <code>
        /// result.Print();
        /// // Output:
        /// // Status: 200
        /// // {"name":"John","age":30}
        /// </code>
        /// </summary>
        public CurlResult Print()
        {
            Console.WriteLine($"Status: {StatusCode}");
            Console.WriteLine(Body);
            return this;
        }

        /// <summary>
        /// <para><b>Print everything - status, headers, and body (like curl -v).</b></para>
        ///
        /// <para>Full debug output:</para>
        /// <code>
        /// result.PrintVerbose();
        /// // Output:
        /// // Status: 200
        /// // Headers:
        /// //   Content-Type: application/json
        /// //   Content-Length: 123
        /// // Body:
        /// // {"name":"John"}
        /// </code>
        /// </summary>
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
        /// <para><b>Transform the result using your own function.</b></para>
        ///
        /// <para>Extract or convert data however you need:</para>
        /// <code>
        /// // Extract just what you need
        /// var name = result.Transform(r =>
        /// {
        ///     var user = r.ParseJson&lt;User&gt;();
        ///     return user.Name;
        /// });
        ///
        /// // Convert to your own type
        /// var summary = result.Transform(r => new
        /// {
        ///     Success = r.IsSuccess,
        ///     Size = r.Body?.Length ?? 0,
        ///     Type = r.GetHeader("Content-Type")
        /// });
        /// </code>
        /// </summary>
        public T Transform<T>(Func<CurlResult, T> transformer)
        {
            return transformer(this);
        }

        /// <summary>
        /// <para><b>Convert the response to a Stream for reading.</b></para>
        ///
        /// <para>Useful for streaming or processing data:</para>
        /// <code>
        /// using var stream = result.ToStream();
        /// using var reader = new StreamReader(stream);
        /// var line = await reader.ReadLineAsync();
        ///
        /// // Or for binary data
        /// using var stream = result.ToStream();
        /// var buffer = new byte[1024];
        /// var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
        /// </code>
        /// </summary>
        /// <returns>A MemoryStream containing the response data</returns>
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
        /// <para><b>Extract lines that match a condition.</b></para>
        ///
        /// <para>Filter text responses:</para>
        /// <code>
        /// // Keep only error lines
        /// result.FilterLines(line => line.Contains("ERROR"));
        ///
        /// // Remove empty lines
        /// result.FilterLines(line => !string.IsNullOrWhiteSpace(line));
        ///
        /// // Keep lines starting with data
        /// result.FilterLines(line => line.StartsWith("data:"));
        /// </code>
        /// </summary>
        public CurlResult FilterLines(Func<string, bool> predicate)
        {
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
    /// <para><b>Detailed timing breakdown of the curl operation.</b></para>
    ///
    /// <para>See where time was spent (like curl -w):</para>
    /// <code>
    /// if (result.Timings.Total > 2000)
    /// {
    ///     Console.WriteLine("Slow request! Let's see why:");
    ///     Console.WriteLine($"DNS: {result.Timings.NameLookup}ms");
    ///     Console.WriteLine($"Connect: {result.Timings.Connect}ms");
    ///     Console.WriteLine($"SSL: {result.Timings.AppConnect}ms");
    ///     Console.WriteLine($"Wait: {result.Timings.StartTransfer}ms");
    /// }
    /// </code>
    /// </summary>
    public class CurlTimings
    {
        /// <summary>DNS resolution time in milliseconds</summary>
        public double NameLookup { get; set; }

        /// <summary>TCP connection time in milliseconds</summary>
        public double Connect { get; set; }

        /// <summary>SSL/TLS handshake time in milliseconds</summary>
        public double AppConnect { get; set; }

        /// <summary>Time until request was sent in milliseconds</summary>
        public double PreTransfer { get; set; }

        /// <summary>Time spent on redirects in milliseconds</summary>
        public double Redirect { get; set; }

        /// <summary>Time until first byte received in milliseconds</summary>
        public double StartTransfer { get; set; }

        /// <summary>Total time in milliseconds</summary>
        public double Total { get; set; }
    }

    /// <summary>
    /// <para><b>Exception for HTTP errors (4xx, 5xx status codes).</b></para>
    ///
    /// <para>Thrown by EnsureSuccess() when request fails:</para>
    /// <code>
    /// try
    /// {
    ///     result.EnsureSuccess();
    /// }
    /// catch (CurlHttpException ex)
    /// {
    ///     Console.WriteLine($"HTTP {ex.StatusCode}: {ex.Message}");
    ///     Console.WriteLine($"Response was: {ex.ResponseBody}");
    /// }
    /// </code>
    /// </summary>
    public class CurlHttpException : Exception
    {
        /// <summary>The HTTP status code that caused the error</summary>
        public int StatusCode { get; }

        /// <summary>The response body (may contain error details)</summary>
        public string ResponseBody { get; set; }

        /// <summary>The response headers</summary>
        public Dictionary<string, string> ResponseHeaders { get; set; }

        /// <summary>
        /// Initializes a new instance of the CurlHttpException class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        public CurlHttpException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## Curl Class


<b>🚀 THE MAIN CURLDOTNET CLASS - Start here!</b>

This class lets you run ANY curl command in C# by just copying and pasting it as a string.
             No translation needed. No learning curve. If it works in curl, it works here.

<b>Quick Start:</b>

```csharp
// Just paste any curl command as a string:
var response = await Curl.Execute("curl https://api.github.com");
Console.WriteLine(response.Body);  // That's it! You're done!
```

<b>What is curl?</b> curl is the universal tool for making HTTP requests from the command line.
             Every API documentation uses it. Now you can use those exact same commands in your C# code.

<b>Learn more:</b>
- 📖 curl documentation: [https://curl\.se/docs/](https://curl.se/docs/ 'https://curl\.se/docs/')
- 📚 curl tutorial: [https://curl\.se/docs/tutorial\.html](https://curl.se/docs/tutorial.html 'https://curl\.se/docs/tutorial\.html')
- ⌨️ curl command generator: [https://curlbuilder\.com/](https://curlbuilder.com/ 'https://curlbuilder\.com/')

```csharp
public static class Curl
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; Curl

### Remarks

<b>Why use CurlDotNet instead of HttpClient?</b>
1. ✂️ <b>Copy &amp; Paste</b> - Use commands directly from API docs without translation
2. 🎓 <b>No Learning Curve</b> - If you know curl (everyone does), you know this
3. 🔄 <b>Easy Migration</b> - Move from bash scripts to C# without rewriting
4. 📦 <b>All Features</b> - Supports all 300+ curl options out of the box

<b>Thread Safety:</b> All methods are thread-safe. You can call them from multiple threads simultaneously.

<b>Memory Efficiency:</b> Responses are streamed, not loaded into memory all at once. Perfect for large files.

<b>Sponsored by</b>[IronSoftware](https://ironsoftware.com 'https://ironsoftware\.com') - creators of IronPDF, IronOCR, IronXL, and IronBarcode.

<b>Two Ways to Use CurlDotNet:</b>
1. <b>Paste curl commands:</b> Just copy/paste any curl command string - it works!
               
  
  ```csharp
  var result = await Curl.ExecuteAsync("curl -X POST https://api.example.com/data -H 'Content-Type: application/json' -d '{\"key\":\"value\"}'");
  ```
2. <b>Use fluent builder:</b> For programmatic API with IntelliSense
               
  
  ```csharp
  var result = await CurlRequestBuilder.Post("https://api.example.com/data").WithHeader("Content-Type", "application/json").WithJson(new { key = "value" }).ExecuteAsync();
  ```

| Properties | |
| :--- | :--- |
| [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds') |   <b>Sets how long to wait for a connection to be established (in seconds).</b>  This is different from the total timeout - it only applies to making the initial connection.              Like adding `--connect-timeout` to every curl command.  <b>Example:</b>\`\.\.\.\`  <b>Tip:</b> Set this lower than DefaultMaxTimeSeconds to fail fast on dead servers.  <b>Learn more:</b>[curl \-\-connect\-timeout documentation](https://curl.se/docs/manpage.html#--connect-timeout 'https://curl\.se/docs/manpage\.html\#\-\-connect\-timeout') |
| [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects') |   <b>Controls whether curl automatically follows HTTP redirects (301, 302, etc).</b>  When true, acts like adding `-L` or `--location` to every command.              Many APIs use redirects, so you often want this enabled.  <b>Example:</b>\`\.\.\.\`  <b>Security note:</b> Be careful following redirects to untrusted sources.  <b>Learn more:</b>[curl \-L documentation](https://curl.se/docs/manpage.html#-L 'https://curl\.se/docs/manpage\.html\#\-L') |
| [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure') |   <b>⚠️ WARNING: Disables SSL certificate validation - ONLY use for development/testing!</b>  When true, acts like adding `-k` or `--insecure` to every command.              This accepts any SSL certificate, even self-signed or expired ones.  <b>Example (DEVELOPMENT ONLY):</b>\`\.\.\.\`  <b>🔴 NEVER use this in production!</b> It makes you vulnerable to man-in-the-middle attacks.  <b>Learn more:</b>[curl \-k documentation](https://curl.se/docs/manpage.html#-k 'https://curl\.se/docs/manpage\.html\#\-k') |
| [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') |   <b>Sets a global timeout for all curl operations (in seconds).</b>  This is like adding `--max-time` to every curl command automatically.              Set to 0 (default) for no timeout. Individual commands can still override this.  <b>Example:</b>\`\.\.\.\`  <b>Learn more:</b>[curl \-\-max\-time documentation](https://curl.se/docs/manpage.html#-m 'https://curl\.se/docs/manpage\.html\#\-m') |

| Methods | |
| :--- | :--- |
| [Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)') |   <b>⚠️ SYNCHRONOUS file download (blocks thread).</b> |
| [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') |   <b>Download a file from a URL and save it to disk.</b>  Downloads any file and saves it to the specified path. Shows progress if the file is large.  <b>Example:</b>\`\.\.\.\`  <b>With Error Handling:</b>\`\.\.\.\` |
| [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') |   <b>⚠️ SYNCHRONOUS - Execute curl command and WAIT for it to complete (BLOCKS thread).</b>  This method BLOCKS your thread until the HTTP request completes. Your application              will FREEZE during this time. Only use when async is not possible.  <b>When to use SYNC (this method):</b>\.\.\.  <b>Example - When sync is OK:</b>\`\.\.\.\`  <b>⚠️ WARNING:</b> This blocks your thread. The application cannot do anything else              while waiting for the HTTP response. Use ExecuteAsync instead whenever possible! |
| [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') |   <b>⚠️ SYNCHRONOUS with settings - Blocks thread with advanced options.</b> |
| [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)') |   <b>⚠️ SYNCHRONOUS with cancellation - Blocks thread but can be cancelled.</b>  Still BLOCKS your thread, but can be cancelled. Prefer ExecuteAsync with cancellation.\`\.\.\.\` |
| [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') |   <b>🎯 THE MAIN METHOD - Executes any curl command and returns the response.</b>  Just paste ANY curl command as a string. It works exactly like running curl from the command line,              but returns the result as a nice C# object you can work with.  <b>Simple Example:</b>\`\.\.\.\`  <b>Real-World Example from Stripe Docs:</b>\`\.\.\.\`  <b>All HTTP Methods Supported:</b>\`\.\.\.\`  <b>Common Options:</b>\`\.\.\.\` |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') |   <b>Execute with advanced settings - for when you need more control.</b>  Use this overload when you need features beyond what curl command strings provide,              like progress callbacks, custom HTTP handlers, or retry policies.  <b>Example with Progress Reporting:</b>\`\.\.\.\`  <b>Example with Custom Retry Policy:</b>\`\.\.\.\` |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') |   <b>Execute a curl command with cancellation support - perfect for long-running operations.</b>  This lets you cancel the HTTP request if it's taking too long or if the user cancels.              Essential for good user experience in desktop and mobile apps.  <b>Basic Example:</b>\`\.\.\.\`  <b>User-Cancellable Download:</b>\`\.\.\.\`  <b>Web API with Request Timeout:</b>\`\.\.\.\` |
| [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)') |   <b>Execute multiple curl commands in parallel - great for performance!</b>  Runs multiple HTTP requests at the same time, which is much faster than running them one by one.              Perfect for fetching data from multiple APIs or endpoints simultaneously.  <b>Example - Fetch Multiple APIs:</b>\`\.\.\.\`  <b>Example - Aggregate Data:</b>\`\.\.\.\`  <b>Error Handling - Some May Fail:</b>\`\.\.\.\` |
| [Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)') |   <b>⚠️ SYNCHRONOUS GET request (blocks thread).</b>\`\.\.\.\` |
| [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') |   <b>Quick GET request - simpler syntax for basic GET operations.</b>  When you just need to GET a URL without any options, use this shortcut method.  <b>Example:</b>\`\.\.\.\` |
| [Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)') |   <b>⚠️ SYNCHRONOUS POST request (blocks thread).</b> |
| [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') |   <b>Quick POST request - simpler syntax for posting data.</b>  Convenient method for simple POST requests with string data.  <b>Example:</b>\`\.\.\.\` |
| [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)') |   <b>⚠️ SYNCHRONOUS POST with JSON (blocks thread).</b> |
| [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)') |   <b>POST with JSON data - automatically serializes objects to JSON.</b>  The easiest way to POST JSON data. Pass any object and it's automatically              serialized to JSON with the correct Content-Type header.  <b>Example:</b>\`\.\.\.\`  <b>Works with any object:</b>\`\.\.\.\` |
| [ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)') |   <b>Convert curl command to JavaScript fetch() code.</b>  Generates JavaScript code that does the same thing as your curl command.              Useful for web developers who need the same request in JavaScript.  <b>Example:</b>\`\.\.\.\` |
| [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)') |   <b>Convert curl command to C# HttpClient code - great for learning!</b>  Shows you exactly how to write the same request using HttpClient.              Perfect for understanding what curl is doing or migrating to pure HttpClient.  <b>Example:</b>\`\.\.\.\` |
| [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)') |   <b>Convert curl command to Python requests code.</b>  Generates Python code using the popular 'requests' library.              Great for Python developers or data scientists.  <b>Example:</b>\`\.\.\.\` |
| [Validate\(string\)](CurlDotNet.Curl.Validate(string).md 'CurlDotNet\.Curl\.Validate\(string\)') |   <b>Check if a curl command is valid without executing it.</b>  Useful for validating user input or checking commands before running them.              This only checks syntax, not whether the URL actually exists.  <b>Example:</b>\`\.\.\.\`  <b>Validate User Input:</b>\`\.\.\.\` |

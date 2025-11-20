#### [CurlDotNet](index.md 'index')
### [CurlDotNet](index.md#CurlDotNet 'CurlDotNet')

## Curl Class


<b>🚀 THE MAIN CURLDOTNET CLASS - Start here!</b>

This class lets you run ANY curl command in C# by just copying and pasting it as a string.
             No translation needed. No learning curve. If it works in curl, it works here.\<example\>
  \<code language="csharp"\>
             // Just paste any curl command as a string:
             var response = await Curl\.ExecuteAsync\("curl https://api\.github\.com"\);
             Console\.WriteLine\(response\.Body\);  // That's it\! You're done\!
            
             // Apply CurlSettings for retries/timeouts
             var settings = new CurlSettings\(\)
                 \.WithTimeout\(seconds: 10\)
                 \.WithRetries\(count: 2, delayMs: 500\);
             var json = await Curl\.ExecuteAsync\("curl https://httpbin\.org/json", settings\);
             json\.EnsureSuccess\(\);
             \</code\>
\</example\>

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
### Properties

<a name='CurlDotNet.Curl.DefaultConnectTimeoutSeconds'></a>

## Curl\.DefaultConnectTimeoutSeconds Property


<b>Sets how long to wait for a connection to be established (in seconds).</b>

This is different from the total timeout - it only applies to making the initial connection.
             Like adding `--connect-timeout` to every curl command.

<b>Example:</b>

```csharp
// Give servers 10 seconds to accept connection
Curl.DefaultConnectTimeoutSeconds = 10;

// If server doesn't respond in 10 seconds, fails fast
await Curl.Execute("curl https://overloaded-server.example.com");
```

<b>Tip:</b> Set this lower than DefaultMaxTimeSeconds to fail fast on dead servers.

<b>Learn more:</b>[curl \-\-connect\-timeout documentation](https://curl.se/docs/manpage.html#--connect-timeout 'https://curl\.se/docs/manpage\.html\#\-\-connect\-timeout')

```csharp
public static int DefaultConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Connection timeout in seconds\. 0 = no timeout\. Default is 0\.

<a name='CurlDotNet.Curl.DefaultFollowRedirects'></a>

## Curl\.DefaultFollowRedirects Property


<b>Controls whether curl automatically follows HTTP redirects (301, 302, etc).</b>

When true, acts like adding `-L` or `--location` to every command.
             Many APIs use redirects, so you often want this enabled.

<b>Example:</b>

```csharp
// Enable redirect following globally
Curl.DefaultFollowRedirects = true;

// Now shortened URLs work automatically
var response = await Curl.Execute("curl https://bit.ly/example");  // Follows to final destination

// Or use -L flag per command
var response = await Curl.Execute("curl -L https://bit.ly/example");
```

<b>Security note:</b> Be careful following redirects to untrusted sources.

<b>Learn more:</b>[curl \-L documentation](https://curl.se/docs/manpage.html#-L 'https://curl\.se/docs/manpage\.html\#\-L')

```csharp
public static bool DefaultFollowRedirects { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to follow redirects, false to stop at first response\. Default is false\.

<a name='CurlDotNet.Curl.DefaultInsecure'></a>

## Curl\.DefaultInsecure Property


<b>⚠️ WARNING: Disables SSL certificate validation - ONLY use for development/testing!</b>

When true, acts like adding `-k` or `--insecure` to every command.
             This accepts any SSL certificate, even self-signed or expired ones.

<b>Example (DEVELOPMENT ONLY):</b>

```csharp
#if DEBUG
// Only in debug builds for local testing
Curl.DefaultInsecure = true;

// Now works with self-signed certificates
await Curl.Execute("curl https://localhost:5001");  // Works even with invalid cert
#endif
```

<b>🔴 NEVER use this in production!</b> It makes you vulnerable to man-in-the-middle attacks.

<b>Learn more:</b>[curl \-k documentation](https://curl.se/docs/manpage.html#-k 'https://curl\.se/docs/manpage\.html\#\-k')

```csharp
public static bool DefaultInsecure { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
true to skip SSL validation \(DANGEROUS\), false to validate \(safe\)\. Default is false\.

<a name='CurlDotNet.Curl.DefaultMaxTimeSeconds'></a>

## Curl\.DefaultMaxTimeSeconds Property


<b>Sets a global timeout for all curl operations (in seconds).</b>

This is like adding `--max-time` to every curl command automatically.
             Set to 0 (default) for no timeout. Individual commands can still override this.

<b>Example:</b>

```csharp
// Set 30 second timeout for all operations
Curl.DefaultMaxTimeSeconds = 30;

// Now all commands timeout after 30 seconds
await Curl.Execute("curl https://slow-api.example.com");  // Times out after 30s

// Override for specific command
await Curl.Execute("curl --max-time 60 https://very-slow-api.example.com");  // 60s timeout
```

<b>Learn more:</b>[curl \-\-max\-time documentation](https://curl.se/docs/manpage.html#-m 'https://curl\.se/docs/manpage\.html\#\-m')

```csharp
public static int DefaultMaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Timeout in seconds\. 0 = no timeout \(wait forever\)\. Default is 0\.
### Methods

<a name='CurlDotNet.Curl.Download(string,string)'></a>

## Curl\.Download\(string, string\) Method


<b>⚠️ SYNCHRONOUS file download (blocks thread).</b>

```csharp
public static CurlDotNet.Core.CurlResult Download(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.Curl.Download(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Download(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.DownloadAsync(string,string)'></a>

## Curl\.DownloadAsync\(string, string\) Method


<b>Download a file from a URL and save it to disk.</b>

Downloads any file and saves it to the specified path. Shows progress if the file is large.

<b>Example:</b>

```csharp
// Download a PDF
await Curl.Download(
    "https://example.com/manual.pdf",
    @"C:\Downloads\manual.pdf"
);

// Download with original filename
await Curl.Download(
    "https://example.com/installer.exe",
    @"C:\Downloads\installer.exe"
);

Console.WriteLine("Download complete!");
```

<b>With Error Handling:</b>

```csharp
try
{
    await Curl.Download(url, "output.zip");
    Console.WriteLine("✅ Download successful");
}
catch (CurlHttpException ex) when (ex.StatusCode == 404)
{
    Console.WriteLine("❌ File not found");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Download failed: {ex.Message}");
}
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> DownloadAsync(string url, string outputPath);
```
#### Parameters

<a name='CurlDotNet.Curl.DownloadAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL of the file to download\.

<a name='CurlDotNet.Curl.DownloadAsync(string,string).outputPath'></a>

`outputPath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Where to save the file\. Can be:
- Full path: `@"C:\Downloads\file.pdf"`
- Relative path: `"downloads/file.pdf"`
- Just filename: `"file.pdf"` (saves to current directory)

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with download information\.

### Remarks

This is equivalent to: `Curl.Execute($"curl -o {outputPath} {url}")`

For large files with progress, use [Execute\(string, CurlSettings\)](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') with OnProgress callback.

<a name='CurlDotNet.Curl.Execute(string)'></a>

## Curl\.Execute\(string\) Method


<b>⚠️ SYNCHRONOUS - Execute curl command and WAIT for it to complete (BLOCKS thread).</b>

This method BLOCKS your thread until the HTTP request completes. Your application
             will FREEZE during this time. Only use when async is not possible.

<b>When to use SYNC (this method):</b>
- ⚠️ Console applications with simple flow
- ⚠️ Legacy code that can't use async
- ⚠️ Unit tests (sometimes)
- ⚠️ Quick scripts or tools
- ❌ NEVER in UI applications (will freeze)
- ❌ NEVER in web applications (reduces throughput)

<b>Example - When sync is OK:</b>

```csharp
// ✅ OK - Simple console app
static void Main()
{
    var result = Curl.Execute("curl https://api.example.com");
    Console.WriteLine(result.Body);
}

// ✅ OK - Unit test
[Test]
public void TestApi()
{
    var result = Curl.Execute("curl https://api.example.com");
    Assert.AreEqual(200, result.StatusCode);
}

// ❌ BAD - Will freeze UI!
private void Button_Click(object sender, EventArgs e)
{
    var result = Curl.Execute("curl https://api.example.com"); // FREEZES UI!
    textBox.Text = result.Body;
}
```

<b>⚠️ WARNING:</b> This blocks your thread. The application cannot do anything else
             while waiting for the HTTP response. Use ExecuteAsync instead whenever possible!

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to execute

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The result \(blocks until complete\)

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings)'></a>

## Curl\.Execute\(string, CurlSettings\) Method


<b>⚠️ SYNCHRONOUS with settings - Blocks thread with advanced options.</b>

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken)'></a>

## Curl\.Execute\(string, CancellationToken\) Method


<b>⚠️ SYNCHRONOUS with cancellation - Blocks thread but can be cancelled.</b>

Still BLOCKS your thread, but can be cancelled. Prefer ExecuteAsync with cancellation.

```csharp
// Blocks thread but can timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var result = Curl.Execute("curl https://api.example.com", cts.Token);
```

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.ExecuteAsync(string)'></a>

## Curl\.ExecuteAsync\(string\) Method


<b>🎯 THE MAIN METHOD - Executes any curl command and returns the response.</b>

Just paste ANY curl command as a string. It works exactly like running curl from the command line,
             but returns the result as a nice C# object you can work with.\<example\>
  \<code language="csharp"\>
             // Copy any curl command from documentation and paste it here:
             var response = await Curl\.ExecuteAsync\("curl https://api\.github\.com/users/octocat"\);
            
             // Work with the response:
             Console\.WriteLine\($"Status: \{response\.StatusCode\}"\);  // 200
             Console\.WriteLine\($"Body: \{response\.Body\}"\);          // JSON data
             var user = response\.ParseJson&lt;GitHubUser&gt;\(\);        // Parse to object
            
             // Use CurlSettings for retries/timeouts:
             var settings = new CurlSettings\(\)
                 \.WithTimeout\(seconds: 10\)
                 \.WithRetries\(count: 2, delayMs: 250\);
             var resilient = await Curl\.ExecuteAsync\("curl \-\-max\-time 5 https://httpbin\.org/json", settings\);
             resilient\.EnsureSuccess\(\);
             \</code\>
\</example\>

<b>Real-World Example from Stripe Docs:</b>

```csharp
// Paste the exact command from Stripe's documentation:
var response = await Curl.Execute(@"
    curl https://api.stripe.com/v1/charges \
      -u sk_test_4eC39HqLyjWDarjtT1zdp7dc: \
      -d amount=2000 \
      -d currency=usd \
      -d source=tok_mastercard \
      -d description='My First Test Charge'
");

if (response.IsSuccess)
{
    var charge = response.ParseJson<StripeCharge>();
    Console.WriteLine($"Payment successful! ID: {charge.Id}");
}
```

<b>All HTTP Methods Supported:</b>

```csharp
await Curl.Execute("curl -X GET https://api.example.com/users");     // GET (default)
await Curl.Execute("curl -X POST https://api.example.com/users");    // POST
await Curl.Execute("curl -X PUT https://api.example.com/users/123"); // PUT
await Curl.Execute("curl -X DELETE https://api.example.com/users/123"); // DELETE
await Curl.Execute("curl -X PATCH https://api.example.com/users/123"); // PATCH
```

<b>Common Options:</b>

```csharp
// Headers
await Curl.Execute("curl -H 'Authorization: Bearer token123' https://api.example.com");

// POST data
await Curl.Execute("curl -d '{\"name\":\"John\"}' https://api.example.com");

// Save to file
await Curl.Execute("curl -o download.pdf https://example.com/file.pdf");

// Follow redirects
await Curl.Execute("curl -L https://short.link/abc");

// Basic auth
await Curl.Execute("curl -u username:password https://api.example.com");

// Timeout
await Curl.Execute("curl --max-time 30 https://slow-api.example.com");
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')


Any valid curl command as a string. You can literally copy and paste from:
- 📖 API documentation (Stripe, Twilio, GitHub, etc.)
- 💬 Stack Overflow answers
- 📝 Blog posts and tutorials
- 🖥️ Your terminal history
- 🔧 Postman's "Code" export feature
- 🌐 Browser DevTools "Copy as cURL"

The "curl" prefix is optional - both work:

```csharp
await Curl.Execute("curl https://api.example.com");  // With "curl"
await Curl.Execute("https://api.example.com");       // Without "curl"
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  

A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') object containing everything from the HTTP response:
- <b>StatusCode</b> - HTTP status (200, 404, 500, etc.)
- <b>Body</b> - Response body as string
- <b>Headers</b> - All response headers as dictionary
- <b>IsSuccess</b> - True if status is 200-299
- <b>ParseJson&lt;T&gt;()</b> - Parse JSON response to your class
- <b>SaveToFile()</b> - Save response to disk

See [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') for all available properties and methods.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  

Thrown when command is null or empty.

```csharp
// ❌ These will throw:
await Curl.Execute(null);
await Curl.Execute("");
await Curl.Execute("   ");
```

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  

Thrown when the curl command can't be understood. Usually means typo or unsupported option.

```csharp
// ❌ This will throw CurlParsingException:
await Curl.Execute("curl --invalid-option https://example.com");
```

Error codes: CURLE_URL_MALFORMAT (3), CURLE_UNSUPPORTED_PROTOCOL (1)

See: [curl error codes](https://curl.se/libcurl/c/libcurl-errors.html 'https://curl\.se/libcurl/c/libcurl\-errors\.html')

[CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException')  

Thrown when the hostname cannot be resolved (DNS failure).

```csharp
try
{
    await Curl.Execute("curl https://this-domain-does-not-exist.com");
}
catch (Exceptions.CurlCouldntResolveHostException ex)
{
    Console.WriteLine($"Could not find server: {ex.Hostname}");
}
```

Error code: CURLE_COULDNT_RESOLVE_HOST (6)

[CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')  

Thrown when the operation takes longer than the timeout specified by [command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string).command 'CurlDotNet\.Curl\.ExecuteAsync\(string\)\.command') (via `--max-time`) 
             or [DefaultMaxTimeSeconds](CurlDotNet.Curl.md#CurlDotNet.Curl.DefaultMaxTimeSeconds 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds').

```csharp
try
{
    await Curl.ExecuteAsync("curl --max-time 5 https://very-slow-api.com");
}
catch (Exceptions.CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout} seconds");
}
```

Error code: `CURLE_OPERATION_TIMEDOUT` (28)

To cancel operations without waiting for timeout, use the [overload with ](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)').

[CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException')  

Thrown for SSL/TLS certificate problems. This can occur when certificates are self-signed, expired,
             or don't match the domain. Check [CertificateError](CurlDotNet.Exceptions.CurlSslException.md#CurlDotNet.Exceptions.CurlSslException.CertificateError 'CurlDotNet\.Exceptions\.CurlSslException\.CertificateError') for details.

```csharp
try
{
    await Curl.ExecuteAsync("curl https://self-signed-cert.example.com");
}
catch (Exceptions.CurlSslException ex)
{
    Console.WriteLine($"SSL problem: {ex.Message}");
    if (ex.CertificateError != null)
        Console.WriteLine($"Certificate error: {ex.CertificateError}");
    // In development only, you could use: curl -k (insecure)
}
```

Error codes: `CURLE_SSL_CONNECT_ERROR` (35), `CURLE_PEER_FAILED_VERIFICATION` (60)

<b>⚠️ WARNING:</b> Using `-k` or `--insecure` disables certificate validation and makes you vulnerable to man-in-the-middle attacks. Only use in development!

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  

Base exception for all curl-related errors. All specific exceptions inherit from this.
             See [CurlErrorCode](CurlDotNet.Exceptions.CurlException.md#CurlDotNet.Exceptions.CurlException.CurlErrorCode 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') for the specific curl error code.

Common error codes:

|Error Code|
|-|
|The URL uses an unsupported protocol (not http, https, ftp, or file)|
|The URL is malformed (missing protocol, invalid characters, etc.)|
|DNS lookup failed - hostname doesn't exist|
|Operation timed out (see [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException'))|
|SSL certificate verification failed (see [CurlSslException](CurlDotNet.Exceptions.CurlSslException.md 'CurlDotNet\.Exceptions\.CurlSslException'))|

For a complete list of all curl error codes, see [curl error codes](https://curl.se/libcurl/c/libcurl-errors.html 'https://curl\.se/libcurl/c/libcurl\-errors\.html').

### See Also
- [Execute with cancellation support](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')
- [Execute multiple commands in parallel](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.curl.executeasync#curldotnet-curl-executeasync(system-string[]) 'CurlDotNet\.Curl\.ExecuteAsync\(System\.String\[\]\)')
- [The response object returned containing , , , etc\.](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')
- [Fluent API alternative for building requests programmatically](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [Set a default timeout for all operations](CurlDotNet.Curl.md#CurlDotNet.Curl.DefaultMaxTimeSeconds 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
- [Enable redirect following globally](CurlDotNet.Curl.md#CurlDotNet.Curl.DefaultFollowRedirects 'CurlDotNet\.Curl\.DefaultFollowRedirects')
- [Complete curl documentation](https://curl.se/docs/manpage.html 'https://curl\.se/docs/manpage\.html')
- [All curl options](https://curl.se/docs/manpage.html#OPTIONS 'https://curl\.se/docs/manpage\.html\#OPTIONS')

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings)'></a>

## Curl\.ExecuteAsync\(string, CurlSettings\) Method


<b>Execute with advanced settings - for when you need more control.</b>

Use this overload when you need features beyond what curl command strings provide,
             like progress callbacks, custom HTTP handlers, or retry policies.

<b>Example with Progress Reporting:</b>

```csharp
var settings = new CurlSettings
{
    OnProgress = (bytes, total) =>
    {
        var percent = (bytes * 100.0) / total;
        Console.WriteLine($"Downloaded: {percent:F1}%");
    }
};

await Curl.Execute("curl -O https://example.com/large-file.zip", settings);
```

<b>Example with Custom Retry Policy:</b>

```csharp
var settings = new CurlSettings
{
    RetryCount = 3,
    RetryDelay = TimeSpan.FromSeconds(2),
    RetryOn = new[] { 500, 502, 503, 504 }  // Retry on server errors
};

await Curl.Execute("curl https://unstable-api.example.com", settings);
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command string\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')


Advanced settings including:
- <b>OnProgress</b> - Callback for download/upload progress
- <b>RetryCount</b> - Number of retry attempts
- <b>RetryDelay</b> - Delay between retries
- <b>CustomHttpMessageHandler</b> - Use your own HttpMessageHandler
- <b>Middleware</b> - Add custom processing pipeline

See [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') for all options.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken)'></a>

## Curl\.ExecuteAsync\(string, CancellationToken\) Method


<b>Execute a curl command with cancellation support - perfect for long-running operations.</b>

This lets you cancel the HTTP request if it's taking too long or if the user cancels.
             Essential for good user experience in desktop and mobile apps.

<b>Basic Example:</b>

```csharp
// Create a cancellation token that times out after 30 seconds
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var response = await Curl.Execute(
        "curl https://slow-api.example.com/large-file",
        cts.Token
    );
    Console.WriteLine("Download complete!");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Download cancelled or timed out after 30 seconds");
}
```

<b>User-Cancellable Download:</b>

```csharp
private CancellationTokenSource _downloadCts;

// Start download
async Task StartDownload()
{
    _downloadCts = new CancellationTokenSource();

    try
    {
        var result = await Curl.Execute(
            "curl -o large-file.zip https://example.com/huge-file.zip",
            _downloadCts.Token
        );
        MessageBox.Show("Download complete!");
    }
    catch (OperationCanceledException)
    {
        MessageBox.Show("Download cancelled by user");
    }
}

// Cancel button click
void CancelButton_Click()
{
    _downloadCts?.Cancel();  // This stops the download
}
```

<b>Web API with Request Timeout:</b>

```csharp
[HttpGet]
public async Task<IActionResult> ProxyRequest(CancellationToken cancellationToken)
{
    // ASP.NET Core passes cancellation token that triggers when:
    // - Client disconnects
    // - Request timeout is reached
    // - Server is shutting down

    var result = await Curl.Execute(
        "curl https://external-api.example.com/data",
        cancellationToken  // Pass it through!
    );

    return Ok(result.Body);
}
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Any curl command string\. See [Execute\(string\)](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') for full documentation\.

<a name='CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')


Token to cancel the operation. Get this from:
- ⏱️ `new CancellationTokenSource(TimeSpan.FromSeconds(30))` - Timeout
- 🔘 `CancellationTokenSource` linked to Cancel button - User cancellation
- 🌐 ASP.NET Core action parameter - Web request cancellation
- 🔗 `CancellationTokenSource.CreateLinkedTokenSource()` - Multiple conditions

Learn more: [Cancellation in \.NET](https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads 'https://docs\.microsoft\.com/en\-us/dotnet/standard/threading/cancellation\-in\-managed\-threads')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Same as [Execute\(string\)](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') \- a [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  

Thrown when the operation is cancelled via the token.

```csharp
try
{
    await Curl.Execute("curl https://api.example.com", cancelToken);
}
catch (OperationCanceledException)
{
    // Handle cancellation gracefully
    Console.WriteLine("Request was cancelled");
}
```

### Remarks

<b>Best Practices:</b>
1. Always dispose CancellationTokenSource when done: `using var cts = new...`
2. Check `token.IsCancellationRequested` before starting expensive operations
3. Pass tokens through your entire async call chain
4. Combine multiple tokens with CreateLinkedTokenSource for complex scenarios

<a name='CurlDotNet.Curl.ExecuteManyAsync(string[])'></a>

## Curl\.ExecuteManyAsync\(string\[\]\) Method


<b>Execute multiple curl commands in parallel - great for performance!</b>

Runs multiple HTTP requests at the same time, which is much faster than running them one by one.
             Perfect for fetching data from multiple APIs or endpoints simultaneously.

<b>Example - Fetch Multiple APIs:</b>

```csharp
// These all run at the same time (parallel), not one after another
var results = await Curl.ExecuteMany(
    "curl https://api.github.com/users/microsoft",
    "curl https://api.github.com/users/dotnet",
    "curl https://api.github.com/users/azure"
);

// Process results - array order matches command order
Console.WriteLine($"Microsoft: {results[0].Body}");
Console.WriteLine($"DotNet: {results[1].Body}");
Console.WriteLine($"Azure: {results[2].Body}");
```

<b>Example - Aggregate Data:</b>

```csharp
// Fetch from multiple services simultaneously
var results = await Curl.ExecuteMany(
    "curl https://api.weather.com/temperature",
    "curl https://api.weather.com/humidity",
    "curl https://api.weather.com/forecast"
);

// Check if all succeeded
if (results.All(r => r.IsSuccess))
{
    var temp = results[0].ParseJson<Temperature>();
    var humidity = results[1].ParseJson<Humidity>();
    var forecast = results[2].ParseJson<Forecast>();

    DisplayWeatherDashboard(temp, humidity, forecast);
}
```

<b>Error Handling - Some May Fail:</b>

```csharp
var results = await Curl.ExecuteMany(commands);

for (int i = 0; i < results.Length; i++)
{
    if (results[i].IsSuccess)
    {
        Console.WriteLine($"✅ Command {i} succeeded");
    }
    else
    {
        Console.WriteLine($"❌ Command {i} failed: {results[i].StatusCode}");
    }
}
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> ExecuteManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Array of curl command strings to execute\. Can pass as:
- Multiple parameters: `ExecuteMany(cmd1, cmd2, cmd3)`
- Array: `ExecuteMany(commandArray)`
- List: `ExecuteMany(commandList.ToArray())`

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Array of [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') objects in the same order as the commands\.
Even if some fail, you still get results for all commands\.

### Remarks

<b>Performance Note:</b> If you have 10 commands that each take 1 second,
             running them in parallel takes ~1 second total instead of 10 seconds sequentially!

<b>Limit:</b> Be respectful of APIs - don't send hundreds of parallel requests.

<a name='CurlDotNet.Curl.Get(string)'></a>

## Curl\.Get\(string\) Method


<b>⚠️ SYNCHRONOUS GET request (blocks thread).</b>

```csharp
var result = Curl.Get("https://api.example.com"); // Blocks!
```

```csharp
public static CurlDotNet.Core.CurlResult Get(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.GetAsync(string)'></a>

## Curl\.GetAsync\(string\) Method


<b>Quick GET request - simpler syntax for basic GET operations.</b>

When you just need to GET a URL without any options, use this shortcut method.

<b>Example:</b>

```csharp
// Instead of:
await Curl.Execute("curl https://api.github.com/users/octocat");

// You can use:
var response = await Curl.Get("https://api.github.com/users/octocat");

// Work with response
var user = response.ParseJson<GitHubUser>();
Console.WriteLine($"Followers: {user.Followers}");
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.GetAsync(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to GET\. Can be HTTP or HTTPS\. Query parameters can be included\.

```csharp
await Curl.Get("https://api.example.com/users?page=1&limit=10");
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

This is equivalent to: `Curl.Execute($"curl {url}")`

For GET requests with headers or auth, use the full [Execute\(string\)](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') method.

<a name='CurlDotNet.Curl.Post(string,string)'></a>

## Curl\.Post\(string, string\) Method


<b>⚠️ SYNCHRONOUS POST request (blocks thread).</b>

```csharp
public static CurlDotNet.Core.CurlResult Post(string url, string data);
```
#### Parameters

<a name='CurlDotNet.Curl.Post(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.Post(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.PostAsync(string,string)'></a>

## Curl\.PostAsync\(string, string\) Method


<b>Quick POST request - simpler syntax for posting data.</b>

Convenient method for simple POST requests with string data.

<b>Example:</b>

```csharp
// Post form data
var response = await Curl.Post(
    "https://api.example.com/login",
    "username=john&password=secret123"
);

// Post JSON data
var json = "{\"name\":\"John\",\"age\":30}";
var result = await Curl.Post("https://api.example.com/users", json);
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostAsync(string url, string data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostAsync(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to\.

<a name='CurlDotNet.Curl.PostAsync(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The data to send in the POST body\. Can be:
- JSON string: `"{\"key\":\"value\"}"`
- Form data: `"key1=value1&key2=value2"`
- XML or any other string content

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

This is equivalent to: `Curl.Execute($"curl -X POST -d '{data}' {url}")`

For POST with headers, use [PostJson\(string, object\)](CurlDotNet.Curl.md#CurlDotNet.Curl.PostJson(string,object) 'CurlDotNet\.Curl\.PostJson\(string, object\)') or the full [Execute\(string\)](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') method.

<a name='CurlDotNet.Curl.PostJson(string,object)'></a>

## Curl\.PostJson\(string, object\) Method


<b>⚠️ SYNCHRONOUS POST with JSON (blocks thread).</b>

```csharp
public static CurlDotNet.Core.CurlResult PostJson(string url, object data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostJson(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='CurlDotNet.Curl.PostJson(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

<a name='CurlDotNet.Curl.PostJsonAsync(string,object)'></a>

## Curl\.PostJsonAsync\(string, object\) Method


<b>POST with JSON data - automatically serializes objects to JSON.</b>

The easiest way to POST JSON data. Pass any object and it's automatically
             serialized to JSON with the correct Content-Type header.

<b>Example:</b>

```csharp
// Create your data object
var newUser = new
{
    name = "John Smith",
    email = "john@example.com",
    age = 30
};

// Post it as JSON automatically
var response = await Curl.PostJson("https://api.example.com/users", newUser);

// Check if successful
if (response.IsSuccess)
{
    var created = response.ParseJson<User>();
    Console.WriteLine($"User created with ID: {created.Id}");
}
```

<b>Works with any object:</b>

```csharp
// Anonymous objects
await Curl.PostJson(url, new { key = "value" });

// Your classes
await Curl.PostJson(url, myUserObject);

// Collections
await Curl.PostJson(url, new[] { item1, item2, item3 });

// Dictionaries
await Curl.PostJson(url, new Dictionary<string, object> { ["key"] = "value" });
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> PostJsonAsync(string url, object data);
```
#### Parameters

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The URL to POST to\.

<a name='CurlDotNet.Curl.PostJsonAsync(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

Any object to serialize as JSON\. Can be:
- Anonymous objects: `new { name = "John" }`
- Your classes: `new User { Name = "John" }`
- Collections: `new[] { 1, 2, 3 }`
- Dictionaries: `Dictionary<string, object>`

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

### Remarks

Automatically adds: `Content-Type: application/json` header

Uses System.Text.Json on .NET 6+ or Newtonsoft.Json on older frameworks

<a name='CurlDotNet.Curl.ToFetch(string)'></a>

## Curl\.ToFetch\(string\) Method


<b>Convert curl command to JavaScript fetch() code.</b>

Generates JavaScript code that does the same thing as your curl command.
             Useful for web developers who need the same request in JavaScript.

<b>Example:</b>

```javascript
var curlCommand = "curl -X GET https://api.example.com/data -H 'Authorization: Bearer token'";

string jsCode = Curl.ToFetch(curlCommand);
Console.WriteLine(jsCode);

// Output:
// fetch('https://api.example.com/data', {
//     method: 'GET',
//     headers: {
//         'Authorization': 'Bearer token'
//     }
// })
// .then(response => response.json())
// .then(data => console.log(data));
```

```csharp
public static string ToFetch(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToFetch(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
JavaScript fetch\(\) code that does the same thing\.

<a name='CurlDotNet.Curl.ToHttpClient(string)'></a>

## Curl\.ToHttpClient\(string\) Method


<b>Convert curl command to C# HttpClient code - great for learning!</b>

Shows you exactly how to write the same request using HttpClient.
             Perfect for understanding what curl is doing or migrating to pure HttpClient.

<b>Example:</b>

```csharp
var curlCommand = @"
    curl -X POST https://api.example.com/users \
      -H 'Content-Type: application/json' \
      -H 'Authorization: Bearer token123' \
      -d '{""name"":""John"",""age"":30}'
";

string code = Curl.ToHttpClient(curlCommand);
Console.WriteLine(code);

// Output:
// using var client = new HttpClient();
// var request = new HttpRequestMessage(HttpMethod.Post, "https://api.example.com/users");
// request.Headers.Add("Authorization", "Bearer token123");
// request.Content = new StringContent("{\"name\":\"John\",\"age\":30}",
//     Encoding.UTF8, "application/json");
// var response = await client.SendAsync(request);
```

```csharp
public static string ToHttpClient(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToHttpClient(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
C\# code using HttpClient that does the same thing\.

### Remarks

Great for:
- Learning how HttpClient works
- Migrating from CurlDotNet to pure HttpClient
- Understanding what curl commands actually do
- Code generation for your projects

<a name='CurlDotNet.Curl.ToPowershellCode(string)'></a>

## Curl\.ToPowershellCode\(string\) Method


<b>Convert a curl command to PowerShell code.</b>

Generates PowerShell code using Invoke-RestMethod that performs the same request.

<b>Example:</b>

```powershell
var curlCommand = "curl -X POST https://api.example.com/data -d '{\"key\":\"value\"}'";

string psCode = Curl.ToPowershellCode(curlCommand);
Console.WriteLine(psCode);

// Output:
// Invoke-RestMethod -Uri "https://api.example.com/data" -Method POST -Body "{\"key\":\"value\"}" -ContentType "application/json"
```

```csharp
public static string ToPowershellCode(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPowershellCode(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
PowerShell code using Invoke\-RestMethod\.

<a name='CurlDotNet.Curl.ToPythonRequests(string)'></a>

## Curl\.ToPythonRequests\(string\) Method


<b>Convert curl command to Python requests code.</b>

Generates Python code using the popular 'requests' library.
             Great for Python developers or data scientists.

<b>Example:</b>

```python
var curlCommand = "curl -u user:pass https://api.example.com/data";

string pythonCode = Curl.ToPythonRequests(curlCommand);
Console.WriteLine(pythonCode);

// Output:
// import requests
//
// response = requests.get(
//     'https://api.example.com/data',
//     auth=('user', 'pass')
// )
// print(response.json())
```

```csharp
public static string ToPythonRequests(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.ToPythonRequests(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command to convert\.

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
Python code using requests library\.

<a name='CurlDotNet.Curl.Validate(string)'></a>

## Curl\.Validate\(string\) Method


<b>Check if a curl command is valid without executing it.</b>

Useful for validating user input or checking commands before running them.
             This only checks syntax, not whether the URL actually exists.

<b>Example:</b>

```csharp
// Check if command is valid
var validation = Curl.Validate("curl -X POST https://api.example.com");

if (validation.IsValid)
{
    Console.WriteLine("✅ Command is valid!");
    // Safe to execute
    var result = await Curl.Execute(command);
}
else
{
    Console.WriteLine($"❌ Invalid command: {validation.ErrorMessage}");
    Console.WriteLine($"Problem at position {validation.ErrorPosition}");
}
```

<b>Validate User Input:</b>

```csharp
Console.Write("Enter curl command: ");
var userCommand = Console.ReadLine();

var validation = Curl.Validate(userCommand);
if (!validation.IsValid)
{
    Console.WriteLine($"Error: {validation.ErrorMessage}");
    if (validation.Suggestions.Any())
    {
        Console.WriteLine("Did you mean:");
        foreach (var suggestion in validation.Suggestions)
        {
            Console.WriteLine($"  - {suggestion}");
        }
    }
}
```

```csharp
public static CurlDotNet.Core.ValidationResult Validate(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Validate(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command string to validate\.

#### Returns
[ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult')  
A [ValidationResult](CurlDotNet.Core.ValidationResult.md 'CurlDotNet\.Core\.ValidationResult') containing:
- <b>IsValid</b> - true if command is valid
- <b>ErrorMessage</b> - Description of what's wrong (if invalid)
- <b>ErrorPosition</b> - Character position of error
- <b>Suggestions</b> - Possible fixes for common mistakes
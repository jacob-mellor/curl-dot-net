#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ExecuteAsync Method

| Overloads | |
| :--- | :--- |
| [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') |   <b>🎯 THE MAIN METHOD - Executes any curl command and returns the response.</b>  Just paste ANY curl command as a string. It works exactly like running curl from the command line,              but returns the result as a nice C# object you can work with.  <b>Simple Example:</b>\`\.\.\.\`  <b>Real-World Example from Stripe Docs:</b>\`\.\.\.\`  <b>All HTTP Methods Supported:</b>\`\.\.\.\`  <b>Common Options:</b>\`\.\.\.\` |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') |   <b>Execute with advanced settings - for when you need more control.</b>  Use this overload when you need features beyond what curl command strings provide,              like progress callbacks, custom HTTP handlers, or retry policies.  <b>Example with Progress Reporting:</b>\`\.\.\.\`  <b>Example with Custom Retry Policy:</b>\`\.\.\.\` |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') |   <b>Execute a curl command with cancellation support - perfect for long-running operations.</b>  This lets you cancel the HTTP request if it's taking too long or if the user cancels.              Essential for good user experience in desktop and mobile apps.  <b>Basic Example:</b>\`\.\.\.\`  <b>User-Cancellable Download:</b>\`\.\.\.\`  <b>Web API with Request Timeout:</b>\`\.\.\.\` |

<a name='CurlDotNet.Curl.ExecuteAsync(string)'></a>

## Curl\.ExecuteAsync\(string\) Method


<b>🎯 THE MAIN METHOD - Executes any curl command and returns the response.</b>

Just paste ANY curl command as a string. It works exactly like running curl from the command line,
             but returns the result as a nice C# object you can work with.

<b>Simple Example:</b>

```csharp
// Copy any curl command from documentation and paste it here:
var response = await Curl.Execute("curl https://api.github.com/users/octocat");

// Work with the response:
Console.WriteLine($"Status: {response.StatusCode}");  // 200
Console.WriteLine($"Body: {response.Body}");          // JSON data
var user = response.ParseJson<GitHubUser>();        // Parse to object
```

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

[CurlParseException](https://learn.microsoft.com/en-us/dotnet/api/curlparseexception 'CurlParseException')  

Thrown when the curl command can't be understood. Usually means typo or unsupported option.

```csharp
// ❌ This will throw CurlParseException:
await Curl.Execute("curl --invalid-option https://example.com");
```

Error codes: CURLE_URL_MALFORMAT (3), CURLE_UNSUPPORTED_PROTOCOL (1)

See: [curl error codes](https://curl.se/libcurl/c/libcurl-errors.html 'https://curl\.se/libcurl/c/libcurl\-errors\.html')

[CurlDnsException](https://learn.microsoft.com/en-us/dotnet/api/curldnsexception 'CurlDnsException')  

Thrown when the hostname cannot be resolved (DNS failure).

```csharp
try
{
    await Curl.Execute("curl https://this-domain-does-not-exist.com");
}
catch (CurlDnsException ex)
{
    Console.WriteLine($"Could not find server: {ex.Hostname}");
}
```

Error code: CURLE_COULDNT_RESOLVE_HOST (6)

[CurlTimeoutException](https://learn.microsoft.com/en-us/dotnet/api/curltimeoutexception 'CurlTimeoutException')  

Thrown when the operation takes longer than the timeout specified by [command](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string).command 'CurlDotNet\.Curl\.ExecuteAsync\(string\)\.command') (via `--max-time`) 
             or [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds').

```csharp
try
{
    await Curl.ExecuteAsync("curl --max-time 5 https://very-slow-api.com");
}
catch (CurlTimeoutException ex)
{
    Console.WriteLine($"Timed out after {ex.Timeout} seconds");
}
```

Error code: `CURLE_OPERATION_TIMEDOUT` (28)

To cancel operations without waiting for timeout, use the [overload with ](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)').

[CurlSslException](https://learn.microsoft.com/en-us/dotnet/api/curlsslexception 'CurlSslException')  

Thrown for SSL/TLS certificate problems. This can occur when certificates are self-signed, expired, 
             or don't match the domain. Check [CurlSslException\.CertificateError](https://learn.microsoft.com/en-us/dotnet/api/curlsslexception.certificateerror 'CurlSslException\.CertificateError') for details.

```csharp
try
{
    await Curl.ExecuteAsync("curl https://self-signed-cert.example.com");
}
catch (CurlSslException ex)
{
    Console.WriteLine($"SSL problem: {ex.Message}");
    if (ex.CertificateError != null)
        Console.WriteLine($"Certificate error: {ex.CertificateError}");
    // In development only, you could use: curl -k (insecure)
}
```

Error codes: `CURLE_SSL_CONNECT_ERROR` (35), `CURLE_PEER_FAILED_VERIFICATION` (60)

<b>⚠️ WARNING:</b> Using `-k` or `--insecure` disables certificate validation and makes you vulnerable to man-in-the-middle attacks. Only use in development!

[CurlException](https://learn.microsoft.com/en-us/dotnet/api/curlexception 'CurlException')  

Base exception for all curl-related errors. All specific exceptions inherit from this. 
             See [CurlException\.ErrorCode](https://learn.microsoft.com/en-us/dotnet/api/curlexception.errorcode 'CurlException\.ErrorCode') for the specific curl error code.

Common error codes:

|Error Code|
|-|
|The URL uses an unsupported protocol (not http, https, ftp, or file)|
|The URL is malformed (missing protocol, invalid characters, etc.)|
|DNS lookup failed - hostname doesn't exist|
|Operation timed out (see [CurlTimeoutException](https://learn.microsoft.com/en-us/dotnet/api/curltimeoutexception 'CurlTimeoutException'))|
|SSL certificate verification failed (see [CurlSslException](https://learn.microsoft.com/en-us/dotnet/api/curlsslexception 'CurlSslException'))|

For a complete list of all curl error codes, see [curl error codes](https://curl.se/libcurl/c/libcurl-errors.html 'https://curl\.se/libcurl/c/libcurl\-errors\.html').

### See Also
- [Execute with cancellation support](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')
- [Execute multiple commands in parallel](https://learn.microsoft.com/en-us/dotnet/api/curldotnet.curl.executeasync#curldotnet-curl-executeasync(system-string[]) 'CurlDotNet\.Curl\.ExecuteAsync\(System\.String\[\]\)')
- [The response object returned containing , , , etc\.](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')
- [Fluent API alternative for building requests programmatically](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [Set a default timeout for all operations](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
- [Enable redirect following globally](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')
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

Any curl command string\. See [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') for full documentation\.

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
Same as [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') \- a [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the response\.

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
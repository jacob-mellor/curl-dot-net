#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## Curl Class

Runs curl commands from C\#: pass the command text you would type in a terminal \(for example
`curl -H 'Accept: application/json' https://api.example.com`\) and get a [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') back\.

```csharp
public static class Curl
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; Curl

### Example

```csharp
// Just paste any curl command as a string:
var response = await Curl.ExecuteAsync("curl https://api.github.com");
Console.WriteLine(response.Body);

// Apply CurlSettings for retries/timeouts
var settings = new CurlSettings()
    .WithTimeout(seconds: 10)
    .WithRetries(count: 2, delayMs: 500);
var json = await Curl.ExecuteAsync("curl https://httpbin.org/json", settings);
json.EnsureSuccess();

// Or build the request in code
var result = await CurlRequestBuilder.Post("https://api.example.com/data")
    .WithJson(new { key = "value" })
    .ExecuteAsync();
```

### Remarks

<b>Start here.</b>[ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') is the main method: copy a command from API
             documentation, Stack Overflow, Postman ("Code > cURL") or a browser's "Copy as cURL" and pass it as a string.
             The leading `curl` is optional. Shell quoting, line continuations (`\`, `^`, backtick) and
             environment variables are read the way the shell it came from would read them (see [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')).

<b>Four ways to make a request</b>, all producing the same [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'):
             a command string with [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'); a typed [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object with
             [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)') (nothing is parsed, so data needs no quoting); the fluent
             [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder') (`CurlRequestBuilder.Get(url).WithHeader(...).ExecuteAsync()`); and the
             libcurl-style [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl') instance API with default headers and timeouts. The quick helpers
             [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)'), [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)'), [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)') and [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')
             cover one-line cases.

<b>What counts as a failure.</b> Like the curl tool, an HTTP error status (404, 500...) is not an
             exception: you get a result with [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode') set, and can call
             [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)'). Add `-f`/`--fail` to the command to turn HTTP errors into a
             [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') (exit code 22). Everything curl reports with a non-zero exit code
             (DNS failure 6, connection refused 7, timeout 28, TLS problem 35/60, write error 23...) throws a
             [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subtype whose [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode') is curl's exit code and whose
             message says what happened, how to fix it and where to read more.

<b>Sync and async.</b> Every `...Async` method has a blocking twin without the suffix
             ([Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)'), [Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)'), [Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)'), [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)'),
             [Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)')). Use the async ones in UI, ASP.NET and any code that is already async.

<b>Thread safety.</b> All static methods are safe to call from many threads at once; they share one
             pooled connection engine. The `Default*` properties are process-wide settings read at the start of every
             request, so set them once at start-up.

<b>Memory.</b> The response body is read into memory ([Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') or
             [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')) even when `-o` also writes it to a file, so for multi-gigabyte
             downloads use a streaming `HttpClient` instead.

Example: `var result = await Curl.ExecuteAsync("curl https://api.github.com/users/octocat");` then
             `Console.WriteLine($"{result.StatusCode}: {result.Body}");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md
             - error handling: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

Sponsored by [IronSoftware](https://ironsoftware.com 'https://ironsoftware\.com') - creators of IronPDF, IronOCR, IronXL, and IronBarcode.

### See Also
- [DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')
- [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')
- [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')

| Fields | |
| :--- | :--- |
| [CurlCompatVersion](CurlDotNet.Curl.CurlCompatVersion.md 'CurlDotNet\.Curl\.CurlCompatVersion') | The curl release \(`8.7.1`\) whose command\-line behaviour CurlDotNet reproduces; it is also the version in the default `User-Agent: curl/8.7.1` header\. |

| Properties | |
| :--- | :--- |
| [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds') | A process\-wide limit, in whole seconds, on how long establishing a connection may take, as if `--connect-timeout` were added to every command that does not set its own\. 0 \(the default\) means no limit\. |
| [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') | The shell \(bash, cmd\.exe, PowerShell, Invoke\-WebRequest\) whose quoting rules are used to read command strings when a call does not name one; [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') by default, which detects it\. |
| [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects') | When true, every request follows HTTP redirects \(301, 302, 303, 307, 308\) as if `-L`/`--location` were in every command\. False \(the default\) returns the redirect response itself, like curl\. |
| [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure') | When true, TLS certificates are not verified for any request, as if `-k`/`--insecure` were in every command\. Development and testing only: it accepts self\-signed, expired and wrong\-host certificates\. |
| [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') | A process\-wide limit, in whole seconds, on how long any HTTP transfer may take, as if `-m`/`--max-time` were added to every command that does not set its own\. 0 \(the default\) means no limit\. |
| [DefaultReadConfigFile](CurlDotNet.Curl.DefaultReadConfigFile.md 'CurlDotNet\.Curl\.DefaultReadConfigFile') | Opt\-in: when true, every command first reads curl's default config file \(`.curlrc`\) from the user's home directory exactly like the curl tool does, unless the command starts with `-q`/`--disable`\. False by default\. |
| [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent') | The `User-Agent` header sent when a command does not set one; `curl/8.7.1` unless you change it\. Set it to null or an empty string to send no User\-Agent at all\. |

| Methods | |
| :--- | :--- |
| [DetectFlavor\(string\)](CurlDotNet.Curl.DetectFlavor(string).md 'CurlDotNet\.Curl\.DetectFlavor\(string\)') | Returns the shell flavor [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') would read [command](CurlDotNet.Curl.DetectFlavor(string).md#CurlDotNet.Curl.DetectFlavor(string).command 'CurlDotNet\.Curl\.DetectFlavor\(string\)\.command') with, without parsing or sending anything\. |
| [Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)') | Downloads [url](CurlDotNet.Curl.Download(string,string).md#CurlDotNet.Curl.Download(string,string).url 'CurlDotNet\.Curl\.Download\(string, string\)\.url') to [outputPath](CurlDotNet.Curl.Download(string,string).md#CurlDotNet.Curl.Download(string,string).outputPath 'CurlDotNet\.Curl\.Download\(string, string\)\.outputPath') and blocks until the file is written; the synchronous twin of [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') \(like `curl -o PATH URL`\)\. |
| [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') | Downloads [url](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).url 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.url') and saves the response body to [outputPath](CurlDotNet.Curl.DownloadAsync(string,string).md#CurlDotNet.Curl.DownloadAsync(string,string).outputPath 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)\.outputPath') \(like `curl -o PATH URL`\); paths with spaces or quotes need no escaping\. |
| [Execute\(CurlOptions\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.Execute\(CurlDotNet\.Core\.CurlOptions\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object and blocks the calling thread until the response arrives; nothing is parsed, so data with quotes or spaces needs no escaping\. |
| [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') | Runs a curl command and blocks the calling thread until the response arrives; the synchronous twin of [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') for console tools, scripts and code that cannot be async\. |
| [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command with timeouts, retries, extra headers or a progress callback from a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object, blocking the calling thread until the response arrives\. |
| [Execute\(string, CurlFlavor\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)') | Executes a curl command copied from a specific shell and blocks the calling thread until the response arrives\. |
| [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)') | Runs a curl command, blocking the calling thread until the response arrives or [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; the synchronous twin of [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |
| [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object instead of a command string, so values from variables are sent exactly as given with no shell quoting to get wrong\. |
| [ExecuteAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object, like [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)'), and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\. |
| [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') | Runs a curl command \(the same text you would type in a terminal\) and returns the response; this is the main method of CurlDotNet\. |
| [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command with timeouts, retries, extra headers, a progress callback or a cancellation token set in code through a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object instead of in the command text\. |
| [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') | Runs a curl command that was copied from a specific shell \(bash, cmd\.exe, PowerShell or Invoke\-WebRequest\), reading its quotes, escapes, line continuations and variables the way that shell would\. |
| [ExecuteAsync\(string, CurlFlavor, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)') | Runs a curl command copied from a specific shell, like [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)'), and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled\. |
| [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)') | Runs a curl command like [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') and stops it when [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled, for user "Cancel" buttons, request aborts in ASP\.NET Core and your own deadlines\. |
| [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)') | Runs several curl commands at the same time and returns their results in the same order, so N requests take about as long as the slowest one instead of the sum\. |
| [Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)') | Sends a plain GET request to [url](CurlDotNet.Curl.Get(string).md#CurlDotNet.Curl.Get(string).url 'CurlDotNet\.Curl\.Get\(string\)\.url') and blocks until the response arrives; the synchronous twin of [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') \(like `curl URL`\)\. |
| [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') | Sends a plain GET request to [url](CurlDotNet.Curl.GetAsync(string).md#CurlDotNet.Curl.GetAsync(string).url 'CurlDotNet\.Curl\.GetAsync\(string\)\.url') \(like `curl URL`\) and returns the response; use it when you need no headers, auth or other options\. |
| [Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)') | Sends a POST request with [data](CurlDotNet.Curl.Post(string,string).md#CurlDotNet.Curl.Post(string,string).data 'CurlDotNet\.Curl\.Post\(string, string\)\.data') as the body and blocks until the response arrives; the synchronous twin of [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') \(like `curl -d DATA URL`\)\. |
| [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') | Sends a POST request with [data](CurlDotNet.Curl.PostAsync(string,string).md#CurlDotNet.Curl.PostAsync(string,string).data 'CurlDotNet\.Curl\.PostAsync\(string, string\)\.data') as the body \(like `curl -d DATA URL`\), with no quoting or escaping needed for the data\. |
| [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)') | Serializes [data](CurlDotNet.Curl.PostJson(string,object).md#CurlDotNet.Curl.PostJson(string,object).data 'CurlDotNet\.Curl\.PostJson\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json`, blocking until the response arrives; the synchronous twin of [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')\. |
| [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)') | Serializes [data](CurlDotNet.Curl.PostJsonAsync(string,object).md#CurlDotNet.Curl.PostJsonAsync(string,object).data 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json` \(like `curl -H 'Content-Type: application/json' -d '{...}' URL`\)\. |
| [ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)') | Translates a curl command into JavaScript `fetch()` code that sends the same request, for web front ends and Node\.js\. |
| [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)') | Translates a curl command into equivalent C\# `HttpClient` code, for learning what the command does or for moving a request to plain `HttpClient`\. |
| [ToPowershellCode\(string\)](CurlDotNet.Curl.ToPowershellCode(string).md 'CurlDotNet\.Curl\.ToPowershellCode\(string\)') | Translates a curl command into a PowerShell `Invoke-RestMethod` call that sends the same request, for Windows admins and build scripts\. |
| [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)') | Translates a curl command into Python code using the `requests` library, for sharing a request with Python scripts and notebooks\. |
| [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)') | Checks a curl command without sending anything and reports whether it can run, what is wrong with it, and exactly which URL, method, headers and body it would send\. |
| [Validate\(string, CurlFlavor\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Validate\(string, CurlDotNet\.CurlFlavor\)') | Checks a curl command copied from a specific shell without executing it, reading it with that shell's quoting, escaping, line\-continuation and variable rules\. |

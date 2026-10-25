#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet')

## DotNetCurl Class

The same API as [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl') under different names \(`DotNetCurl.Curl(command)`,
`DotNetCurl.CurlAsync(command)`\), for code that prefers to read "curl this command"\.

```csharp
public static class DotNetCurl
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; DotNetCurl

### Example

```csharp
// Synchronous execution
var response = DotNetCurl.Curl("curl https://api.github.com/users/octocat");
Console.WriteLine(response.Body);

// Async execution
var response = await DotNetCurl.CurlAsync("curl https://api.github.com/users/octocat");

// POST request
var result = DotNetCurl.Curl(@"
    curl -X POST https://api.example.com/users \
    -H 'Content-Type: application/json' \
    -d '{""name"":""John"",""email"":""john@example.com""}'
");

// Multiple commands in parallel
var results = await DotNetCurl.CurlManyAsync(
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts");
```

### Remarks

Every member forwards to the matching [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl') member, so behaviour, results,
             exceptions and the process-wide `Default*` settings are shared: setting
             [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds') here is the same as setting [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds').
             Pick one class and use it consistently.

Name mapping: [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)') = [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)'),
             [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') = [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'),
             [CurlManyAsync\(string\[\]\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)') = [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)'),
             [ToPython\(string\)](CurlDotNet.DotNetCurl.ToPython(string).md 'CurlDotNet\.DotNetCurl\.ToPython\(string\)') = [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)'); the others have
             the same names.

All members are static and thread-safe.

Example: `var response = DotNetCurl.Curl("curl https://api.github.com/users/octocat");` (blocking) or
             `var response = await DotNetCurl.CurlAsync("curl https://api.github.com/users/octocat");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

Sponsored by IronSoftware - creators of IronPDF, IronOCR, IronXL, and more.

### See Also
- [Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')
- [CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')
- [LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

| Properties | |
| :--- | :--- |
| [DefaultConnectTimeoutSeconds](CurlDotNet.DotNetCurl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultConnectTimeoutSeconds') | Process\-wide limit in whole seconds for establishing a connection \(DNS, TCP, TLS\), like `--connect-timeout`; 0 means no limit\. The same setting as [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')\. |
| [DefaultFollowRedirects](CurlDotNet.DotNetCurl.DefaultFollowRedirects.md 'CurlDotNet\.DotNetCurl\.DefaultFollowRedirects') | When true, every request follows HTTP redirects as if `-L` were in every command; the same setting as [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects')\. |
| [DefaultInsecure](CurlDotNet.DotNetCurl.DefaultInsecure.md 'CurlDotNet\.DotNetCurl\.DefaultInsecure') | When true, TLS certificates are not verified for any request, as if `-k` were in every command; the same setting as [DefaultInsecure](CurlDotNet.Curl.DefaultInsecure.md 'CurlDotNet\.Curl\.DefaultInsecure')\. Development and testing only\. |
| [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds') | Process\-wide limit in whole seconds for every HTTP transfer, like adding `-m`/`--max-time` to each command; 0 means no limit\. The same setting as [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')\. |

| Methods | |
| :--- | :--- |
| [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)') | Runs a curl command and blocks the calling thread until the response arrives; same as [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')\. |
| [Curl\(string, int\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)') | Runs a curl command, blocking the calling thread, and cancels it if it has not finished after [timeoutSeconds](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.timeoutSeconds') seconds\. |
| [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') | Runs a curl command asynchronously and returns the response; same as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. |
| [CurlAsync\(string, CurlSettings\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command asynchronously with timeouts, retries, headers or a progress callback from a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object; same as [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')\. |
| [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)') | Runs a curl command asynchronously and stops it when [cancellationToken](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; same as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |
| [CurlMany\(string\[\]\)](CurlDotNet.DotNetCurl.CurlMany(string[]).md 'CurlDotNet\.DotNetCurl\.CurlMany\(string\[\]\)') | Runs several curl commands in parallel and blocks until all have finished; results come back in the same order as the commands\. |
| [CurlManyAsync\(string\[\]\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)') | Runs several curl commands in parallel and returns their results in the same order; same as [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')\. |
| [Download\(string, string\)](CurlDotNet.DotNetCurl.Download(string,string).md 'CurlDotNet\.DotNetCurl\.Download\(string, string\)') | Downloads [url](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).url 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.url') to [outputPath](CurlDotNet.DotNetCurl.Download(string,string).md#CurlDotNet.DotNetCurl.Download(string,string).outputPath 'CurlDotNet\.DotNetCurl\.Download\(string, string\)\.outputPath') and blocks until the file is written; same as [Download\(string, string\)](CurlDotNet.Curl.Download(string,string).md 'CurlDotNet\.Curl\.Download\(string, string\)') \(like `curl -o PATH URL`\)\. |
| [DownloadAsync\(string, string\)](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)') | Downloads [url](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).url 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.url') to [outputPath](CurlDotNet.DotNetCurl.DownloadAsync(string,string).md#CurlDotNet.DotNetCurl.DownloadAsync(string,string).outputPath 'CurlDotNet\.DotNetCurl\.DownloadAsync\(string, string\)\.outputPath') asynchronously; same as [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)') \(like `curl -o PATH URL`\)\. |
| [Get\(string\)](CurlDotNet.DotNetCurl.Get(string).md 'CurlDotNet\.DotNetCurl\.Get\(string\)') | Sends a plain GET request to [url](CurlDotNet.DotNetCurl.Get(string).md#CurlDotNet.DotNetCurl.Get(string).url 'CurlDotNet\.DotNetCurl\.Get\(string\)\.url') and blocks until the response arrives; same as [Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)') \(like `curl URL`\)\. |
| [GetAsync\(string\)](CurlDotNet.DotNetCurl.GetAsync(string).md 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)') | Sends a plain GET request to [url](CurlDotNet.DotNetCurl.GetAsync(string).md#CurlDotNet.DotNetCurl.GetAsync(string).url 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)\.url') asynchronously; same as [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') \(like `curl URL`\)\. |
| [Post\(string, string\)](CurlDotNet.DotNetCurl.Post(string,string).md 'CurlDotNet\.DotNetCurl\.Post\(string, string\)') | Sends a POST request with [data](CurlDotNet.DotNetCurl.Post(string,string).md#CurlDotNet.DotNetCurl.Post(string,string).data 'CurlDotNet\.DotNetCurl\.Post\(string, string\)\.data') as the body and blocks until the response arrives; same as [Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)') \(like `curl -d DATA URL`\)\. |
| [PostAsync\(string, string\)](CurlDotNet.DotNetCurl.PostAsync(string,string).md 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)') | Sends a POST request with [data](CurlDotNet.DotNetCurl.PostAsync(string,string).md#CurlDotNet.DotNetCurl.PostAsync(string,string).data 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)\.data') as the body asynchronously; same as [PostAsync\(string, string\)](CurlDotNet.Curl.PostAsync(string,string).md 'CurlDotNet\.Curl\.PostAsync\(string, string\)') \(like `curl -d DATA URL`\)\. |
| [PostJson\(string, object\)](CurlDotNet.DotNetCurl.PostJson(string,object).md 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)') | Serializes [data](CurlDotNet.DotNetCurl.PostJson(string,object).md#CurlDotNet.DotNetCurl.PostJson(string,object).data 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json`, blocking until the response arrives; same as [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)')\. |
| [PostJsonAsync\(string, object\)](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)') | Serializes [data](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md#CurlDotNet.DotNetCurl.PostJsonAsync(string,object).data 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json` asynchronously; same as [PostJsonAsync\(string, object\)](CurlDotNet.Curl.PostJsonAsync(string,object).md 'CurlDotNet\.Curl\.PostJsonAsync\(string, object\)')\. |
| [ToFetch\(string\)](CurlDotNet.DotNetCurl.ToFetch(string).md 'CurlDotNet\.DotNetCurl\.ToFetch\(string\)') | Translates a curl command into JavaScript `fetch()` code without sending it; same as [ToFetch\(string\)](CurlDotNet.Curl.ToFetch(string).md 'CurlDotNet\.Curl\.ToFetch\(string\)')\. |
| [ToHttpClient\(string\)](CurlDotNet.DotNetCurl.ToHttpClient(string).md 'CurlDotNet\.DotNetCurl\.ToHttpClient\(string\)') | Translates a curl command into equivalent C\# `HttpClient` code without sending it; same as [ToHttpClient\(string\)](CurlDotNet.Curl.ToHttpClient(string).md 'CurlDotNet\.Curl\.ToHttpClient\(string\)')\. |
| [ToPython\(string\)](CurlDotNet.DotNetCurl.ToPython(string).md 'CurlDotNet\.DotNetCurl\.ToPython\(string\)') | Translates a curl command into Python `requests` code without sending it; same as [ToPythonRequests\(string\)](CurlDotNet.Curl.ToPythonRequests(string).md 'CurlDotNet\.Curl\.ToPythonRequests\(string\)')\. |
| [Validate\(string\)](CurlDotNet.DotNetCurl.Validate(string).md 'CurlDotNet\.DotNetCurl\.Validate\(string\)') | Checks a curl command without sending it and reports what is wrong \(no URL, unknown option, bad URL, unsupported scheme\) or what it would send; same as [Validate\(string\)](CurlDotNet.Curl.Validate.md#CurlDotNet.Curl.Validate(string) 'CurlDotNet\.Curl\.Validate\(string\)')\. |

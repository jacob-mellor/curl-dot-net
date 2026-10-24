#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.GetAsync\(string\) Method

Sends a plain GET request to [url](CurlDotNet.Curl.GetAsync(string).md#CurlDotNet.Curl.GetAsync(string).url 'CurlDotNet\.Curl\.GetAsync\(string\)\.url') \(like `curl URL`\) and returns the response; use it
when you need no headers, auth or other options\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> GetAsync(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.GetAsync(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL including the scheme, for example
            `https://api.example.com/users?page=1&limit=10`; http, https, ftp, ftps and file URLs are supported\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode'), [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
            \(the response text\), [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers') and, for binary content, [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.Curl.GetAsync(string).md#CurlDotNet.Curl.GetAsync(string).url 'CurlDotNet\.Curl\.GetAsync\(string\)\.url') is null, empty or whitespace\. Fix: pass an absolute URL\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

[CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException')  
The URL cannot be parsed \(exit code 3\)\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed \(DNS, connect, timeout, TLS\.\.\.\); see
            [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') for the subtypes\.

### Example

```csharp
var response = await Curl.GetAsync("https://api.github.com/users/octocat");
var user = response.ParseJson<GitHubUser>();
Console.WriteLine($"Followers: {user.Followers}");
```

### Remarks

The URL is sent as given; nothing is parsed or shell-quoted, so `&`, `?` and quotes in it
            need no escaping. The `Default*` properties (timeouts, [DefaultFollowRedirects](CurlDotNet.Curl.DefaultFollowRedirects.md 'CurlDotNet\.Curl\.DefaultFollowRedirects'),
            [DefaultUserAgent](CurlDotNet.Curl.DefaultUserAgent.md 'CurlDotNet\.Curl\.DefaultUserAgent')) apply. HTTP errors are returned, not thrown: check
            [IsSuccess](CurlDotNet.Core.CurlResult.IsSuccess.md 'CurlDotNet\.Core\.CurlResult\.IsSuccess') or call [EnsureSuccess\(\)](CurlDotNet.Core.CurlResult.EnsureSuccess().md 'CurlDotNet\.Core\.CurlResult\.EnsureSuccess\(\)').

For headers or authentication use [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
            (`curl -H 'Authorization: Bearer TOKEN' URL`) or [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)').

Example: `var user = (await Curl.GetAsync("https://api.github.com/users/octocat")).ParseJson<GitHubUser>();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)')
- [Builder equivalent](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')
- [libcurl\-style equivalent](CurlDotNet.Lib.LibCurl.GetAsync(string,System.Action_CurlDotNet.Core.CurlOptions_,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.GetAsync\(string, System\.Action\<CurlDotNet\.Core\.CurlOptions\>, System\.Threading\.CancellationToken\)')
- [GetAsync\(string\)](CurlDotNet.DotNetCurl.GetAsync(string).md 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)')
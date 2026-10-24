#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.Post\(string, string\) Method

Sends a POST request with [data](CurlDotNet.DotNetCurl.Post(string,string).md#CurlDotNet.DotNetCurl.Post(string,string).data 'CurlDotNet\.DotNetCurl\.Post\(string, string\)\.data') as the body and blocks until the response arrives; same as
[Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)') \(like `curl -d DATA URL`\)\.

```csharp
public static CurlDotNet.Core.CurlResult Post(string url, string data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Post(string,string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.DotNetCurl.Post(string,string).data'></a>

`data` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The request body as text \(form fields `a=1&b=2`, JSON, XML\.\.\.\); null sends an empty body\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.Post(string,string).md#CurlDotNet.DotNetCurl.Post(string,string).url 'CurlDotNet\.DotNetCurl\.Post\(string, string\)\.url') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

The data is sent exactly as given, with `Content-Type: application/x-www-form-urlencoded`; use
            [PostJson\(string, object\)](CurlDotNet.DotNetCurl.PostJson(string,object).md 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)') for JSON.

Example: `var result = DotNetCurl.Post("https://api.example.com/login", "user=john&pass=secret");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.PostAsync(string,string).md 'CurlDotNet\.DotNetCurl\.PostAsync\(string, string\)')
- [Post\(string, string\)](CurlDotNet.Curl.Post(string,string).md 'CurlDotNet\.Curl\.Post\(string, string\)')
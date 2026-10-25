#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.Get\(string\) Method

Sends a plain GET request to [url](CurlDotNet.DotNetCurl.Get(string).md#CurlDotNet.DotNetCurl.Get(string).url 'CurlDotNet\.DotNetCurl\.Get\(string\)\.url') and blocks until the response arrives; same as
[Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)') \(like `curl URL`\)\.

```csharp
public static CurlDotNet.Core.CurlResult Get(string url);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL including the scheme, for example `https://api.example.com/users?page=1`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with status code, body and headers\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.Get(string).md#CurlDotNet.DotNetCurl.Get(string).url 'CurlDotNet\.DotNetCurl\.Get\(string\)\.url') is null, empty or whitespace\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

No headers or options; the `Default*` settings apply. HTTP errors are returned, not thrown.

Example: `var result = DotNetCurl.Get("https://api.example.com/health");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.GetAsync(string).md 'CurlDotNet\.DotNetCurl\.GetAsync\(string\)')
- [Get\(string\)](CurlDotNet.Curl.Get(string).md 'CurlDotNet\.Curl\.Get\(string\)')
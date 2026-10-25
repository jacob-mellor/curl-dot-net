#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Get\(string\) Method

Sends a plain GET request to [url](CurlDotNet.Curl.Get(string).md#CurlDotNet.Curl.Get(string).url 'CurlDotNet\.Curl\.Get\(string\)\.url') and blocks until the response arrives; the synchronous
twin of [GetAsync\(string\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)') \(like `curl URL`\)\.

```csharp
public static CurlDotNet.Core.CurlResult Get(string url);
```
#### Parameters

<a name='CurlDotNet.Curl.Get(string).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL including the scheme, for example `https://api.example.com/users?page=1`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the status code, body and headers\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.Curl.Get(string).md#CurlDotNet.Curl.Get(string).url 'CurlDotNet\.Curl\.Get\(string\)\.url') is null, empty or whitespace\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/exceptions/url\-errors\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
var result = Curl.Get("https://api.example.com"); // blocks until the response arrives
```

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

No headers or options; the `Default*` properties apply. HTTP errors are returned, not thrown.
            Do not call it on a UI thread.

Example: `var result = Curl.Get("https://api.example.com/health");` then
            `Console.WriteLine(result.IsSuccess ? "up" : $"down ({result.StatusCode})");`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.Curl.GetAsync(string).md 'CurlDotNet\.Curl\.GetAsync\(string\)')
- [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')
- [Get\(string\)](CurlDotNet.DotNetCurl.Get(string).md 'CurlDotNet\.DotNetCurl\.Get\(string\)')
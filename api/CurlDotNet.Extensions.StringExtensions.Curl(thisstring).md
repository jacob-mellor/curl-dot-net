#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Extensions](CurlDotNet.Extensions.md 'CurlDotNet\.Extensions').[StringExtensions](CurlDotNet.Extensions.StringExtensions.md 'CurlDotNet\.Extensions\.StringExtensions')

## StringExtensions\.Curl\(this string\) Method

Runs this string as a curl command line and blocks until it finishes \- the synchronous form of
[CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)'), for console tools and scripts\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(this string command);
```
#### Parameters

<a name='CurlDotNet.Extensions.StringExtensions.Curl(thisstring).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

A curl command line such as `curl -I https://example.com`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command is invalid or the transfer failed; see the message for the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

Blocks the calling thread; in ASP.NET, UI or other async code use [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)') to avoid
            thread-pool starvation.

Example: `var result = "curl -s https://api.github.com/zen".Curl(); Console.WriteLine(result.Body);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [CurlAsync\(this string\)](CurlDotNet.Extensions.StringExtensions.CurlAsync.md#CurlDotNet.Extensions.StringExtensions.CurlAsync(thisstring) 'CurlDotNet\.Extensions\.StringExtensions\.CurlAsync\(this string\)')
- [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')
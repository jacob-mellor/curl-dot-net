#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.PostJson\(string, object\) Method

Serializes [data](CurlDotNet.DotNetCurl.PostJson(string,object).md#CurlDotNet.DotNetCurl.PostJson(string,object).data 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)\.data') to JSON and POSTs it with `Content-Type: application/json`, blocking
until the response arrives; same as [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)')\.

```csharp
public static CurlDotNet.Core.CurlResult PostJson(string url, object data);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).url'></a>

`url` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

An absolute URL to post to, for example `https://api.example.com/users`\.

<a name='CurlDotNet.DotNetCurl.PostJson(string,object).data'></a>

`data` [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object')

The object to send as JSON: anonymous object, your class, array/list or dictionary\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') with the server's status, headers and body\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[url](CurlDotNet.DotNetCurl.PostJson(string,object).md#CurlDotNet.DotNetCurl.PostJson(string,object).url 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)\.url') is null, empty or whitespace\.

[CurlParsingException](CurlDotNet.Exceptions.CurlParsingException.md 'CurlDotNet\.Exceptions\.CurlParsingException')  
[data](CurlDotNet.DotNetCurl.PostJson(string,object).md#CurlDotNet.DotNetCurl.PostJson(string,object).data 'CurlDotNet\.DotNetCurl\.PostJson\(string, object\)\.data') cannot be serialized to JSON\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

System.Text.Json is used (Newtonsoft.Json on the .NET Standard 2.0 build).

Example: `var result = DotNetCurl.PostJson("https://api.example.com/users", new { name = "John" });`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-post-json-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.PostJsonAsync(string,object).md 'CurlDotNet\.DotNetCurl\.PostJsonAsync\(string, object\)')
- [PostJson\(string, object\)](CurlDotNet.Curl.PostJson(string,object).md 'CurlDotNet\.Curl\.PostJson\(string, object\)')
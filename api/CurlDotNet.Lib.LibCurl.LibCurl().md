#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\(\) Constructor

Creates a client with no defaults: curl's behaviour \(no redirects followed, no time limit, TLS
certificates verified, proxy from the `http_proxy`/`https_proxy` environment variables\)\.

```csharp
public LibCurl();
```

### Example

```csharp
using (var curl = new LibCurl())
{
    var result = await curl.GetAsync("https://api.example.com/data");
}
```

### Remarks

The instance owns a connection pool, so create one per API (or per application) and reuse it rather
            than creating one per request; dispose it when you are done (`using var curl = new LibCurl();`).
            In ASP.NET Core register it as a singleton.

Example: `using var curl = new LibCurl().WithUserAgent("MyApp/1.0").WithFollowRedirects();`
            then `var page = await curl.GetAsync("http://example.com");`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-in-aspnet-core/README.md

### See Also
- [Dispose\(\)](CurlDotNet.Lib.LibCurl.Dispose().md 'CurlDotNet\.Lib\.LibCurl\.Dispose\(\)')
- [Configure\(Action&lt;CurlOptions&gt;\)](CurlDotNet.Lib.LibCurl.Configure(System.Action_CurlDotNet.Core.CurlOptions_).md 'CurlDotNet\.Lib\.LibCurl\.Configure\(System\.Action\<CurlDotNet\.Core\.CurlOptions\>\)')
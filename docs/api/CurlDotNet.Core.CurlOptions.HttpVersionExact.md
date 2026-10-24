#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.HttpVersionExact Property

Requires exactly the [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion') instead of allowing a fallback, like curl's
`--http2-prior-knowledge` \(HTTP/2 without negotiation\) and `--http3-only`\.

```csharp
public bool HttpVersionExact { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to forbid downgrading\. Default: false\.

### Remarks

Works together with [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion'), which holds `"1.0"`, `"1.1"`, `"2.0"` or `"3.0"`. When this
            is true the request uses `HttpVersionPolicy.RequestVersionExact` (on .NET 5 and later), so a server that cannot
            speak the requested version fails the request rather than silently downgrading.

Example: `curl --http2-prior-knowledge http://localhost:5000/grpc-health` is
            `new CurlOptions { Url = url, HttpVersion = "2.0", HttpVersionExact = true }`.

### See Also
- [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion')
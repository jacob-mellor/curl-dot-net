#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.HttpVersion Property

The HTTP version to request: `"1.0"`, `"1.1"`, `"2.0"` or `"3.0"`, set by curl's
`--http1.0`, `--http1.1`, `--http2` and `--http3`\.

```csharp
public string? HttpVersion { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
One of the strings above \(`"1"`, `"2"` and `"3"` are accepted too\)\. Default: null, which
            uses HttpClient's default \(HTTP/1\.1, upgraded where the platform negotiates it\)\.

### Remarks

The connection may fall back to an older version unless [HttpVersionExact](CurlDotNet.Core.CurlOptions.HttpVersionExact.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersionExact') is true
            (`--http2-prior-knowledge`, `--http3-only`). HTTP/2 and HTTP/3 need .NET 5+ (HTTP/3 also needs OS
            QUIC support); .NET Standard 2.0 and .NET Framework speak HTTP/1.x only. An unknown value is ignored.

Example: `curl --http2 https://example.com/` is `new CurlOptions { Url = url, HttpVersion = "2.0" }`.

### See Also
- [HttpVersionExact](CurlDotNet.Core.CurlOptions.HttpVersionExact.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersionExact')
- [WithHttpVersion\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithHttpVersion(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHttpVersion\(string\)')
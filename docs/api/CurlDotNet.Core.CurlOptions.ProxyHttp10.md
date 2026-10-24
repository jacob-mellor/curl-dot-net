#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProxyHttp10 Property

Talks HTTP/1\.0 to the proxy, so a `CONNECT` tunnel request is sent as `HTTP/1.0`, like curl's
`--proxy1.0 HOST[:PORT]` for old proxies that reject HTTP/1\.1\.

```csharp
public bool ProxyHttp10 { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for an HTTP/1\.0 proxy\. Default: false\.

### Remarks

The parser sets this together with [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy'). Example: `curl --proxy1.0 oldproxy:8080 https://example.com/`
            is `new CurlOptions { Url = url, Proxy = "oldproxy:8080", ProxyHttp10 = true }`.

### See Also
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [ProxyTunnel](CurlDotNet.Core.CurlOptions.ProxyTunnel.md 'CurlDotNet\.Core\.CurlOptions\.ProxyTunnel')
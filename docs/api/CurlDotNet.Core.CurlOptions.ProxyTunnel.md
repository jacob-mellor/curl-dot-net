#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProxyTunnel Property

Tunnels through the HTTP proxy with `CONNECT` even for plain `http://` URLs, like curl's
`-p`/`--proxytunnel`; `https://` URLs are always tunnelled\.

```csharp
public bool ProxyTunnel { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to tunnel every request\. Default: false\.

### Remarks

Without it, `http://` requests are sent to the proxy with the absolute URL in the request line
            (`GET http://host/path HTTP/1.1`). Some corporate proxies only allow `CONNECT`. Supported on .NET 5 and later.

Example: `curl -p -x proxy:3128 http://example.com/` is
            `new CurlOptions { Url = "http://example.com/", Proxy = "proxy:3128", ProxyTunnel = true }`; the proxy first sees
            `CONNECT example.com:80 HTTP/1.1`.

### See Also
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [ProxyHttp10](CurlDotNet.Core.CurlOptions.ProxyHttp10.md 'CurlDotNet\.Core\.CurlOptions\.ProxyHttp10')
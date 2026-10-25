#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Proxy Property

The proxy every request goes through, like curl's `-x`/`--proxy [scheme://][user:pass@]host[:port]`;
null uses the `http_proxy`/`https_proxy`/`all_proxy` environment variables\.

```csharp
public string Proxy { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A proxy URL such as `http://proxy.corp:3128`, `socks5h://127.0.0.1:1080` or just
            `proxy.corp:3128` \(HTTP, port 1080 when omitted, as in curl\)\. Default: null\.

### Remarks

Plain `http://` URLs are sent to the proxy with the absolute URL in the request line; `https://` URLs use a
            `CONNECT` tunnel (see [ProxyTunnel](CurlDotNet.Core.CurlOptions.ProxyTunnel.md 'CurlDotNet\.Core\.CurlOptions\.ProxyTunnel')). User info in the proxy URL, or [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials'),
            authenticates with the proxy ([ProxyAuthScheme](CurlDotNet.Core.CurlOptions.ProxyAuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.ProxyAuthScheme')). Hosts in [NoProxy](CurlDotNet.Core.CurlOptions.NoProxy.md 'CurlDotNet\.Core\.CurlOptions\.NoProxy') bypass it. An empty
            string disables the environment proxy. An unreachable proxy fails with
            [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') (exit 7) naming the proxy; an unknown proxy host
            with [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') (exit 5).

Example: `curl -x http://proxy.corp:3128 https://api.example.com/` is
            `new CurlOptions { Url = "https://api.example.com/", Proxy = "http://proxy.corp:3128" }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')
- [NoProxy](CurlDotNet.Core.CurlOptions.NoProxy.md 'CurlDotNet\.Core\.CurlOptions\.NoProxy')
- [ProxyTunnel](CurlDotNet.Core.CurlOptions.ProxyTunnel.md 'CurlDotNet\.Core\.CurlOptions\.ProxyTunnel')
- [Socks5Proxy](CurlDotNet.Core.CurlOptions.Socks5Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Socks5Proxy')
- [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)')
- [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)')
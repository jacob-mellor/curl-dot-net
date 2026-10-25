#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.Proxy Property

A \.NET proxy to send the request through, like curl's `-x`/`--proxy` \(with `-U` for its
credentials\); it replaces a proxy given in the command\.

```csharp
public System.Net.IWebProxy Proxy { get; set; }
```

#### Property Value
[System\.Net\.IWebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.iwebproxy 'System\.Net\.IWebProxy')  
Null by default \(the command's `-x` or the `http_proxy`/`https_proxy` environment variables
            decide\); [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)') sets a [System\.Net\.WebProxy](https://learn.microsoft.com/en-us/dotnet/api/system.net.webproxy 'System\.Net\.WebProxy')\.

### Remarks

When the transfer starts, the proxy is asked for the request URL ([System\.Net\.IWebProxy\.GetProxy\(System\.Uri\)](https://learn.microsoft.com/en-us/dotnet/api/system.net.iwebproxy.getproxy#system-net-iwebproxy-getproxy(system-uri) 'System\.Net\.IWebProxy\.GetProxy\(System\.Uri\)')):
            its answer becomes [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy'), and [System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')s from
            [System\.Net\.IWebProxy\.Credentials](https://learn.microsoft.com/en-us/dotnet/api/system.net.iwebproxy.credentials 'System\.Net\.IWebProxy\.Credentials') become [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials') (sent as Basic, like
            `-U`). A URL the proxy bypasses ([System\.Net\.IWebProxy\.IsBypassed\(System\.Uri\)](https://learn.microsoft.com/en-us/dotnet/api/system.net.iwebproxy.isbypassed#system-net-iwebproxy-isbypassed(system-uri) 'System\.Net\.IWebProxy\.IsBypassed\(System\.Uri\)')) is fetched directly. The URL is
            resolved once per call, so a `-L` redirect to another host uses the same proxy.

Example: `new CurlSettings().WithProxy("http://proxy.local:3128", "user", secret)` is
            `curl -x http://proxy.local:3128 -U user:SECRET`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [WithProxy\(string\)](CurlDotNet.Core.CurlSettings.WithProxy.md#CurlDotNet.Core.CurlSettings.WithProxy(string) 'CurlDotNet\.Core\.CurlSettings\.WithProxy\(string\)')
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
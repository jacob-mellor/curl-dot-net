#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithProxy Method

| Overloads | |
| :--- | :--- |
| [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)') | Sends the request through a proxy server \(curl's `-x` / `--proxy`\), for example a corporate proxy, Fiddler or a SOCKS tunnel\. |
| [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)') | Sends the request through a proxy that requires a user name and password \(curl's `-x URL --proxy-user user:password`\)\. |

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string)'></a>

## CurlRequestBuilder\.WithProxy\(string\) Method

Sends the request through a proxy server \(curl's `-x` / `--proxy`\), for example a corporate
proxy, Fiddler or a SOCKS tunnel\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy URL such as `"http://proxy.corp:8080"` or `"socks5://127.0.0.1:1080"`; null or empty means no explicit proxy\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Accepted forms: `http://proxy.corp:8080`, `https://proxy.corp:443`,
            `socks5://127.0.0.1:1080`, `socks5h://...` (DNS through the proxy), `socks4://...`, or just
            `proxy.corp:8080` (treated as http). Credentials can be embedded (`http://user:pass@proxy:8080`)
            or given with [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)'). Without this option the
            `http_proxy`/`https_proxy`/`all_proxy` environment variables apply, like curl.

An unreachable proxy fails with [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') (exit code 7) naming the
            proxy; an unknown proxy host with [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') (exit code 5).

Example: `CurlRequestBuilder.Get("https://example.com").WithProxy("http://127.0.0.1:8888")`
            routes through Fiddler, like `curl -x http://127.0.0.1:8888 https://example.com`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)')
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [WithProxy\(string, string, string\)](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)')

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string)'></a>

## CurlRequestBuilder\.WithProxy\(string, string, string\) Method

Sends the request through a proxy that requires a user name and password \(curl's
`-x URL --proxy-user user:password`\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithProxy(string proxyUrl, string username, string password);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy URL such as `"http://proxy.corp:8080"`\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy user name, for example `"ada"` or `"DOMAIN\\ada"`\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy password; may be empty\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The proxy URL accepts the same forms as [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)'). The credentials are sent
            to the proxy as Basic authentication (`Proxy-Authorization`); they are never sent to the target
            server. [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') prints them as `--proxy-user 'user:password'` in clear text.

A rejected login gives a result with `StatusCode == 407` (or, through HTTPS tunnels, a
            [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') describing the 407).

Example: `.WithProxy("http://proxy.corp:8080", "DOMAIN\\ada", password)` is
            `curl -x http://proxy.corp:8080 --proxy-user 'DOMAIN\ada:...' URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [WithProxy\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string\)')
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')
- [WithProxy\(string, string, string\)](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)')
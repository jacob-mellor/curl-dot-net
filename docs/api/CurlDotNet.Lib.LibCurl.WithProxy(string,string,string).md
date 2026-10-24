#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithProxy\(string, string, string\) Method

Sends every request through a proxy \(curl's `-x URL`, with `--proxy-user user:password` when
credentials are given\), for corporate proxies, Fiddler or SOCKS tunnels\.

```csharp
public CurlDotNet.Lib.LibCurl WithProxy(string proxyUrl, string username=null, string password=null);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).proxyUrl'></a>

`proxyUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy URL such as `"http://proxy.example.com:8080"` or `"socks5://127.0.0.1:1080"`\.

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).username'></a>

`username` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy user name, or null for none\.

<a name='CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).password'></a>

`password` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Proxy password, or null \(treated as empty\)\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Accepted proxy forms: `http://proxy.corp:8080`, `https://proxy.corp:443`,
            `socks5://127.0.0.1:1080`, `socks5h://...`, `socks4://...`, or `proxy.corp:8080`
            (http). When [username](CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).md#CurlDotNet.Lib.LibCurl.WithProxy(string,string,string).username 'CurlDotNet\.Lib\.LibCurl\.WithProxy\(string, string, string\)\.username') is null or empty no proxy credentials are sent (credentials embedded
            in the URL, `http://user:pass@proxy:8080`, still work). A request that sets its own
            [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy') keeps it.

An unreachable proxy gives [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') (exit code 7) and an unknown proxy
            host [CurlCouldntResolveProxyException](CurlDotNet.Exceptions.CurlCouldntResolveProxyException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveProxyException') (exit code 5) in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception').

Example: `using var curl = new LibCurl().WithProxy("http://proxy.corp:8080", "ada", password);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)')
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [ProxyCredentials](CurlDotNet.Core.CurlOptions.ProxyCredentials.md 'CurlDotNet\.Core\.CurlOptions\.ProxyCredentials')
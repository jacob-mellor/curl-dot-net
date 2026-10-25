#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProxyCredentials Property

User name and password for the proxy, like curl's `-U`/`--proxy-user user:password`; sent as
`Proxy-Authorization: Basic` unless [ProxyAuthScheme](CurlDotNet.Core.CurlOptions.ProxyAuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.ProxyAuthScheme') picks another scheme\.

```csharp
public System.Net.NetworkCredential? ProxyCredentials { get; set; }
```

#### Property Value
[System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')  
A [System\.Net\.NetworkCredential](https://learn.microsoft.com/en-us/dotnet/api/system.net.networkcredential 'System\.Net\.NetworkCredential')\. Default: null; `user:pass@` in [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy') has the same effect\.

### Remarks

Example: `curl -x proxy.corp:3128 -U ada:s3cret URL` is
            `new CurlOptions { Url = url, Proxy = "proxy.corp:3128", ProxyCredentials = new NetworkCredential("ada", "s3cret") }`.
            A `407` answer means the proxy rejected them. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [ProxyAuthScheme](CurlDotNet.Core.CurlOptions.ProxyAuthScheme.md 'CurlDotNet\.Core\.CurlOptions\.ProxyAuthScheme')
- [Credentials](CurlDotNet.Core.CurlOptions.Credentials.md 'CurlDotNet\.Core\.CurlOptions\.Credentials')
- [WithProxy\(string, string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithProxy.md#CurlDotNet.Core.CurlRequestBuilder.WithProxy(string,string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithProxy\(string, string, string\)')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.NoProxy Property

Hosts that are reached directly even though a proxy is configured, like curl's `--noproxy LIST`; null
falls back to the `no_proxy`/`NO_PROXY` environment variable\.

```csharp
public string? NoProxy { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A comma\-separated list of host names, domain suffixes, IP addresses and CIDR ranges; `*` bypasses the proxy
            for every host\. Default: null\.

### Remarks

`example.com` also matches `www.example.com` (a leading dot is allowed); `10.0.0.0/8` matches the
            range. An empty string overrides the environment variable and proxies everything. Applies to [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy'),
            [Socks5Proxy](CurlDotNet.Core.CurlOptions.Socks5Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Socks5Proxy') and proxies taken from the environment.

Example: `curl -x http://proxy:3128 --noproxy localhost,.internal https://api.internal/` is
            `new CurlOptions { Url = "https://api.internal/", Proxy = "http://proxy:3128", NoProxy = "localhost,.internal" }`,
            which connects to `api.internal` directly while every other host goes through the proxy.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-proxy-csharp/README.md

### See Also
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [Socks5Proxy](CurlDotNet.Core.CurlOptions.Socks5Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Socks5Proxy')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Socks5Proxy Property

A SOCKS proxy for every connection, like curl's `--socks5 host:port` \(or `--socks4`,
`--socks4a`, `--socks5-hostname`, which arrive here as `socks4://`, `socks4a://`, `socks5h://` URLs\)\.

```csharp
public string? Socks5Proxy { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A proxy URL like `socks5://127.0.0.1:1080`, or `host:port` \(treated as `socks5://`\)\.
            Default: null\. [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy') wins when both are set\.

### Remarks

SOCKS proxies need .NET 6 or later. `socks5h` lets the proxy resolve host names (needed for `.onion`
            and internal names). Example: `curl --socks5-hostname 127.0.0.1:9050 https://example.com/` is
            `new CurlOptions { Url = url, Socks5Proxy = "socks5h://127.0.0.1:9050" }`.

### See Also
- [Proxy](CurlDotNet.Core.CurlOptions.Proxy.md 'CurlDotNet\.Core\.CurlOptions\.Proxy')
- [NoProxy](CurlDotNet.Core.CurlOptions.NoProxy.md 'CurlDotNet\.Core\.CurlOptions\.NoProxy')
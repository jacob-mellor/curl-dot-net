#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.HaproxyProtocol Property

Sends a HAProxy PROXY protocol v1 header as the first bytes of every new connection, curl's
`--haproxy-protocol`: `PROXY TCP4 SOURCE-IP DEST-IP SOURCE-PORT DEST-PORT\r\n` \(`TCP6` for IPv6\),
telling a load balancer or server behind HAProxy which client address the connection comes from\.

```csharp
public bool HaproxyProtocol { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to send the header\. Default: false\. Also on when [HaproxyClientIp](CurlDotNet.Core.CurlOptions.HaproxyClientIp.md 'CurlDotNet\.Core\.CurlOptions\.HaproxyClientIp') is set\.

### Remarks

The addresses and ports are the local and remote ends of the TCP connection, exactly as curl reports them
            (lib/cf-haproxy.c). The header goes out before TLS for `https://` URLs, and through a `-p` tunnel after
            the CONNECT. Only send it to servers that expect it: any other server sees a broken request line.

The PROXY header is written by CurlDotNet's own connection code, which needs .NET 5 or later and an HttpClient
            created by CurlDotNet; on the .NET Standard 2.0 / .NET Framework build, or with an HttpClient you supplied, the transfer
            fails with [CurlNotBuiltInException](CurlDotNet.Exceptions.CurlNotBuiltInException.md 'CurlDotNet\.Exceptions\.CurlNotBuiltInException') (exit 4) instead of silently leaving the header out.

Example: `curl --haproxy-protocol http://backend:8080/` is
            `new CurlOptions { Url = "http://backend:8080/", HaproxyProtocol = true }`.

### See Also
- [HaproxyClientIp](CurlDotNet.Core.CurlOptions.HaproxyClientIp.md 'CurlDotNet\.Core\.CurlOptions\.HaproxyClientIp')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.HaproxyClientIp Property

The client address to claim in the HAProxy PROXY protocol header, curl's `--haproxy-clientip`; setting it
also turns on [HaproxyProtocol](CurlDotNet.Core.CurlOptions.HaproxyProtocol.md 'CurlDotNet\.Core\.CurlOptions\.HaproxyProtocol')\.

```csharp
public string? HaproxyClientIp { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An IPv4 or IPv6 address as text \(sent verbatim\), or null \(default\) to report the real local address\.

### Remarks

Like curl, the value is used as both the source and the destination address of the PROXY line, the ports stay
            those of the real connection, and the protocol is `TCP4` for a dotted IPv4 address and `TCP6` for anything
            else: `--haproxy-clientip 192.168.1.1` sends `PROXY TCP4 192.168.1.1 192.168.1.1 50123 80`. curl does not
            check the value, so neither does CurlDotNet.

Example: `curl --haproxy-clientip 2a04:4e42::347 http://backend/` is
            `new CurlOptions { Url = "http://backend/", HaproxyClientIp = "2a04:4e42::347" }`.

### See Also
- [HaproxyProtocol](CurlDotNet.Core.CurlOptions.HaproxyProtocol.md 'CurlDotNet\.Core\.CurlOptions\.HaproxyProtocol')
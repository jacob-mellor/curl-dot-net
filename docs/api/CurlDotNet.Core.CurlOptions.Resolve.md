#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Resolve Property

Pins host names to addresses without DNS, like curl's `--resolve HOST:PORT:ADDRESS`; the URL keeps its
host \(TLS name and `Host` header\) while the connection goes to the given address\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Resolve { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A dictionary of `"host:port"` to `"address[,address...]"`, for example
            `["example.com:443"] = "127.0.0.1"`; `"*:443"` matches every host on that port\. Default: empty\.

### Remarks

Only entries whose port matches the URL's port apply; IPv6 addresses may be written in brackets. The parser
            rejects an entry without a numeric port with [CurlOptionSyntaxException](CurlDotNet.Exceptions.CurlOptionSyntaxException.md 'CurlDotNet\.Exceptions\.CurlOptionSyntaxException') (exit 49).
            Typical uses: testing a new server before DNS changes, or sending traffic to a local container.

Example: `curl --resolve example.com:443:10.0.0.5 https://example.com/` is
            `options.Resolve["example.com:443"] = "10.0.0.5";`. See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/dns-errors.md

### See Also
- [DnsServers](CurlDotNet.Core.CurlOptions.DnsServers.md 'CurlDotNet\.Core\.CurlOptions\.DnsServers')
- [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url')
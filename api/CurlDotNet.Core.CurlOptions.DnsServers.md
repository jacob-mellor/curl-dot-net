#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DnsServers Property

DNS server list from curl's `--dns-servers`; not applied \(names are resolved by the operating system\), so
the parser also records the option in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')\. Use [Resolve](CurlDotNet.Core.CurlOptions.Resolve.md 'CurlDotNet\.Core\.CurlOptions\.Resolve') to pin a name
to an address\.

```csharp
public string? DnsServers { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A comma\-separated list such as `8.8.8.8,1.1.1.1`\. Default: null\. Not acted on\.

### Remarks

Example: `curl --dns-servers 1.1.1.1 URL` parses to `DnsServers = "1.1.1.1"` and
            `UnsupportedOptions` containing `--dns-servers`.

### See Also
- [Resolve](CurlDotNet.Core.CurlOptions.Resolve.md 'CurlDotNet\.Core\.CurlOptions\.Resolve')
- [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')
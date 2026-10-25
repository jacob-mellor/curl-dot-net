#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Interface Property

Network interface or source address from curl's `--interface`; not applied \(connections use the operating
system's default route\), so the parser also records the option in [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')\.

```csharp
public string? Interface { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An interface name \(`eth0`\) or address\. Default: null\. Not acted on\.

### Remarks

Binding outgoing connections to an interface is not available through HttpClient's connection layer.
            Example: `curl --interface eth1 URL` parses to `Interface = "eth1"` and
            `UnsupportedOptions` containing `--interface`.

### See Also
- [Resolve](CurlDotNet.Core.CurlOptions.Resolve.md 'CurlDotNet\.Core\.CurlOptions\.Resolve')
- [UnsupportedOptions](CurlDotNet.Core.CurlOptions.UnsupportedOptions.md 'CurlDotNet\.Core\.CurlOptions\.UnsupportedOptions')
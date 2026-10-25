#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.GlobOff Property

Turns off URL globbing so `{}` and `[]` in a parsed URL are sent literally instead of expanding into
several requests, like curl's `-g`/`--globoff`\.

```csharp
public bool GlobOff { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to disable globbing\. Default: false\.

### Remarks

Without it, `curl "https://example.com/{a,b}"` and `"https://example.com/[1-3]"` expand into several URLs
            (see [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers')); with it, the brackets are percent-encoded and sent as one request. Globbing
            only happens when a command string is parsed; hand-built options are never globbed.

Example: `curl -g "http://[::1]:8080/"` for IPv6 literals, or for APIs with `filter[name]=x` queries.

### See Also
- [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers')
- [GlobMatches](CurlDotNet.Core.CurlOptions.GlobMatches.md 'CurlDotNet\.Core\.CurlOptions\.GlobMatches')
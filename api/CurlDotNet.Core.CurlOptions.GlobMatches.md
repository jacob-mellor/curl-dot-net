#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.GlobMatches Property

The text each URL glob matched for this transfer, which curl's `#1`, `#2`\.\.\. placeholders in `-o`
file names refer to\.

```csharp
public System.Collections.Generic.List<string> GlobMatches { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
One string per glob in the URL, in order; empty when the URL has no globs\.

### Remarks

For `curl "https://example.com/{one,two}/[1-2].txt" -o "#1-#2.txt"` the transfer for `/two/1.txt` holds
            `["two", "1"]`, and the parser has already rewritten [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') to `two-1.txt`.

### See Also
- [AdditionalTransfers](CurlDotNet.Core.CurlOptions.AdditionalTransfers.md 'CurlDotNet\.Core\.CurlOptions\.AdditionalTransfers')
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
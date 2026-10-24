#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TextContentTypes Property

Extra MIME types that are always read as text \(into [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\), for custom types the
library would otherwise treat as binary\.

```csharp
public System.Collections.Generic.List<string> TextContentTypes { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
MIME types such as `"application/x-ndjson"`\. Default: empty\.

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithTextContentType("application/x-custom-text")
    .ExecuteAsync();
```

### Remarks

These take priority over the built-in binary detection, but not over [BinaryContentTypes](CurlDotNet.Core.CurlOptions.BinaryContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.BinaryContentTypes') or
            [ForceBinary](CurlDotNet.Core.CurlOptions.ForceBinary.md 'CurlDotNet\.Core\.CurlOptions\.ForceBinary').

Example: `options.TextContentTypes.Add("application/x-custom-config");` then read `result.Body`.

### See Also
- [BinaryContentTypes](CurlDotNet.Core.CurlOptions.BinaryContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.BinaryContentTypes')
- [ForceBinary](CurlDotNet.Core.CurlOptions.ForceBinary.md 'CurlDotNet\.Core\.CurlOptions\.ForceBinary')
- [WithTextContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTextContentType\(string\)')
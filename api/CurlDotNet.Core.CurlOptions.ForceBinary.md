#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ForceBinary Property

Treats the response as binary whatever its Content\-Type, so the body bytes land unchanged in
[BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') instead of being decoded as text\.

```csharp
public bool ForceBinary { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to force binary\. Default: false \(CurlDotNet decides from Content\-Type and by sniffing the bytes\)\.

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/report.xlsx")
    .AsBinary()
    .ExecuteAsync();
```

### Remarks

Use it for servers that label files wrongly (an `.xlsx` served as `text/plain`), which would otherwise be
            corrupted by text decoding. A CurlDotNet setting with no curl flag (curl never decodes bodies).

Example: `new CurlOptions { Url = "https://example.com/report.xlsx", ForceBinary = true }`, then
            `File.WriteAllBytes("report.xlsx", result.BinaryData)`.

### See Also
- [BinaryContentTypes](CurlDotNet.Core.CurlOptions.BinaryContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.BinaryContentTypes')
- [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes')
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')
- [AsBinary\(\)](CurlDotNet.Core.CurlRequestBuilder.AsBinary().md 'CurlDotNet\.Core\.CurlRequestBuilder\.AsBinary\(\)')
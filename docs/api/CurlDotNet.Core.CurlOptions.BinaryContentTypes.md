#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.BinaryContentTypes Property

Extra MIME types that are always read as binary \(into [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')\), for custom types
your server uses; checked before every other text/binary rule\.

```csharp
public System.Collections.Generic.List<string> BinaryContentTypes { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')  
MIME types such as `"application/x-custom-binary"`, compared case\-insensitively without parameters\.
            Default: empty\.

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithBinaryContentType("application/x-custom-binary")
    .ExecuteAsync();
```

### Remarks

The library already recognises common binary types including Office documents (.xlsx, .docx, .pptx), PDFs,
            archives, images, audio, video and fonts. This list has the highest priority, above [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes').

Example: `options.BinaryContentTypes.Add("application/x-protobuf");`.

### See Also
- [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes')
- [ForceBinary](CurlDotNet.Core.CurlOptions.ForceBinary.md 'CurlDotNet\.Core\.CurlOptions\.ForceBinary')
- [WithBinaryContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryContentType\(string\)')
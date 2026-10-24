#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.BinaryContentTypes Property

Additional MIME types that should be treated as binary data\.

```csharp
public System.Collections.Generic.List<string> BinaryContentTypes { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithBinaryContentType("application/x-custom-binary")
    .ExecuteAsync();
```

### Remarks

These MIME types take priority over the built-in text content type detection.
            Use this to override the default behavior for specific MIME types that your server
            may serve with unexpected Content-Type headers.

The library already recognizes common binary types including Office documents
            (.xlsx, .docx, .pptx), PDFs, archives, images, audio, video, and more.
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithBinaryContentType\(string\) Method

Registers a MIME type \(such as `application/x-custom-binary`\) whose responses are always kept as
bytes in [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') rather than decoded as text\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBinaryContentType(string mimeType);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).mimeType'></a>

`mimeType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The MIME type to treat as binary \(e\.g\., "application/x\-custom\-binary"\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithBinaryContentType("application/x-custom-format")
    .WithOutput("data.bin")
    .ExecuteAsync();
```

### Remarks

The library already recognizes common binary types (Office documents, PDFs, archives, images,
            audio, video, fonts). Use this for a custom or unusual type your server uses for binary content; the
            match is on the media type without parameters, case-insensitively. Call it once per type.

This is a CurlDotNet option with no curl flag, so [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') does not print it.

Example: `CurlRequestBuilder.Get(url).WithBinaryContentType("application/x-protobuf")`.

### See Also
- [AsBinary\(\)](CurlDotNet.Core.CurlRequestBuilder.AsBinary().md 'CurlDotNet\.Core\.CurlRequestBuilder\.AsBinary\(\)')
- [WithTextContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTextContentType\(string\)')
- [BinaryContentTypes](CurlDotNet.Core.CurlOptions.BinaryContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.BinaryContentTypes')
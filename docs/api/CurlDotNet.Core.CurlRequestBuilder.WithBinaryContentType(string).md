#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithBinaryContentType\(string\) Method

Register a MIME type that should always be treated as binary data\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithBinaryContentType(string mimeType);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).mimeType'></a>

`mimeType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The MIME type to treat as binary \(e\.g\., "application/x\-custom\-binary"\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining\.

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithBinaryContentType("application/x-custom-format")
    .WithOutput("data.bin")
    .ExecuteAsync();
```

### Remarks

The library already recognizes common binary types (Office documents, PDFs,
            archives, images, audio, video, fonts, etc.). Use this for custom or unusual
            MIME types that your server uses for binary content.
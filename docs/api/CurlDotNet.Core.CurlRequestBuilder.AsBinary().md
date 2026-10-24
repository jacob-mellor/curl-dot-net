#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.AsBinary\(\) Method

Treats the response as binary whatever its Content\-Type says, so the bytes land in
[BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') untouched instead of being decoded into [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder AsBinary();
```

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://example.com/report.xlsx")
    .AsBinary()
    .WithOutput("report.xlsx")
    .ExecuteAsync();
```

### Remarks

Use this for servers that label files wrongly (for example an .xlsx served as `text/plain`),
            which would otherwise be decoded as text and corrupted. Common binary types (Office documents, PDF,
            archives, images, audio, video, fonts) are already detected without it. Files written with
            [WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)') are always byte-exact; this only changes what the result holds.

This is a CurlDotNet option with no curl flag, so [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') does not print it.

Example: `var r = await CurlRequestBuilder.Get("https://example.com/report.xlsx").AsBinary().ExecuteAsync();`
            then `File.WriteAllBytes("report.xlsx", r.BinaryData);`.

### See Also
- [WithBinaryContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryContentType\(string\)')
- [WithTextContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTextContentType\(string\)')
- [ForceBinary](CurlDotNet.Core.CurlOptions.ForceBinary.md 'CurlDotNet\.Core\.CurlOptions\.ForceBinary')
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')
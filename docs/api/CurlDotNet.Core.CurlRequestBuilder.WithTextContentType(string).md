#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTextContentType\(string\) Method

Registers a MIME type \(such as `application/x-custom-config`\) whose responses are always decoded as
text into [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), even though the library would treat it as binary\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTextContentType(string mimeType);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).mimeType'></a>

`mimeType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The MIME type to treat as text \(e\.g\., "application/x\-custom\-text"\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Use this for custom types you know contain text; the body is decoded with the response's charset
            (UTF-8 when none is given). Call it once per type.

This is a CurlDotNet option with no curl flag, so [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') does not print it.

Example: `var r = await CurlRequestBuilder.Get(url).WithTextContentType("application/x-custom-config").ExecuteAsync();`
            then `Console.WriteLine(r.Body);`.

### See Also
- [WithBinaryContentType\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBinaryContentType(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBinaryContentType\(string\)')
- [AsBinary\(\)](CurlDotNet.Core.CurlRequestBuilder.AsBinary().md 'CurlDotNet\.Core\.CurlRequestBuilder\.AsBinary\(\)')
- [TextContentTypes](CurlDotNet.Core.CurlOptions.TextContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.TextContentTypes')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTextContentType\(string\) Method

Register a MIME type that should always be treated as text content\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTextContentType(string mimeType);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTextContentType(string).mimeType'></a>

`mimeType` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The MIME type to treat as text \(e\.g\., "application/x\-custom\-text"\)\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining\.

### Example

```csharp
var result = await CurlRequestBuilder
    .Get("https://example.com/config")
    .WithTextContentType("application/x-custom-config")
    .ExecuteAsync();
Console.WriteLine(result.Body); // Treated as text
```

### Remarks

Use this for custom MIME types that the library treats as binary by default
            but that you know contain text data.
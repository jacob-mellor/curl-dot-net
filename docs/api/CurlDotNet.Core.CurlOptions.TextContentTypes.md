#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TextContentTypes Property

Additional MIME types that should be treated as text content\.

```csharp
public System.Collections.Generic.List<string> TextContentTypes { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/data")
    .WithTextContentType("application/x-custom-text")
    .ExecuteAsync();
```

### Remarks

These MIME types take priority over the built-in binary detection (but not
            [BinaryContentTypes](CurlDotNet.Core.CurlOptions.BinaryContentTypes.md 'CurlDotNet\.Core\.CurlOptions\.BinaryContentTypes') which has highest priority). Use this to override
            the default behavior for MIME types that you know are text but that the library
            treats as binary by default.
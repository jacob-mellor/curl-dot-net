#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.AsBinary\(\) Method

Force the response to be treated as binary data, regardless of Content\-Type header\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder AsBinary();
```

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
Builder for method chaining\.

### Example

```csharp
// Download an .xlsx file that a server incorrectly serves as text/plain
var result = await CurlRequestBuilder
    .Get("https://example.com/report.xlsx")
    .AsBinary()
    .WithOutput("report.xlsx")
    .ExecuteAsync();

// The file is now saved correctly as binary data
```

### Remarks

Use this when downloading files from servers that serve binary content with
             incorrect or misleading Content-Type headers. When enabled, the response body
             is always read as a byte array, preventing corruption of binary data.
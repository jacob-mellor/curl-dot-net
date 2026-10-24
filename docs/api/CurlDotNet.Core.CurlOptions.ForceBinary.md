#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ForceBinary Property

Forces all response content to be treated as binary data, regardless of Content\-Type header\.

```csharp
public bool ForceBinary { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

### Example

```csharp
// Using the fluent builder:
var result = await CurlRequestBuilder
    .Get("https://example.com/report.xlsx")
    .AsBinary()
    .ExecuteAsync();
```

### Remarks

When set to `true`, the response body will always be read as a byte array
            using ReadAsByteArrayAsync. This prevents
            corruption of binary files that may have incorrect Content-Type headers from the server.

Use this when downloading files from servers that serve binary content with
            text-like Content-Type headers (e.g., a server returning .xlsx files as text/plain).
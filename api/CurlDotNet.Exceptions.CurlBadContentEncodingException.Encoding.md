#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException')

## CurlBadContentEncodingException\.Encoding Property

The encoding header value that could not be decoded, such as `gzip`, `br` or `chunked, gzip`\.

```csharp
public string Encoding { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The Content\-Encoding or Transfer\-Encoding value the server sent\.

### Remarks

Compare it with what the server documents; a mislabelled body is a server bug.

### See Also
- [Command](CurlDotNet.Exceptions.CurlException.Command.md 'CurlDotNet\.Exceptions\.CurlException\.Command')
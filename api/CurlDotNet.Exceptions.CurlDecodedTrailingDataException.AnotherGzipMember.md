#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlDecodedTrailingDataException](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException')

## CurlDecodedTrailingDataException\.AnotherGzipMember Property

True when the bytes after the first gzip member are a second gzip member \(the server concatenated several
compressed pieces\), false when they are something else\.

```csharp
public bool AnotherGzipMember { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for curl's `Multi-member gzip response not supported` case\.

### Remarks

A multi-member body is valid gzip that curl chooses not to decode; other trailing bytes are a
            server bug.

### See Also
- [TrailingBytes](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.TrailingBytes.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.TrailingBytes')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlDecodedTrailingDataException](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException')

## CurlDecodedTrailingDataException\.TrailingBytes Property

How many body bytes followed the end of the first gzip member\.

```csharp
public long TrailingBytes { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte count, 1 or more\.

### Remarks

Example: `Console.WriteLine($"{ex.TrailingBytes} bytes after the gzip stream");`

### See Also
- [AnotherGzipMember](CurlDotNet.Exceptions.CurlDecodedTrailingDataException.AnotherGzipMember.md 'CurlDotNet\.Exceptions\.CurlDecodedTrailingDataException\.AnotherGzipMember')
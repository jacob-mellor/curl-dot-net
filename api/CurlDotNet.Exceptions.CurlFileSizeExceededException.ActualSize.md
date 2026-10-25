#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')

## CurlFileSizeExceededException\.ActualSize Property

The size that exceeded the limit, in bytes: the announced Content\-Length, or the bytes read so far\.

```csharp
public long ActualSize { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte count larger than [MaxSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.MaxSize') \(`long.MaxValue` for an absurdly large announced length\)\.

### Remarks

Example: `Console.WriteLine($"{ex.ActualSize / 1048576} MB");`

### See Also
- [MaxSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.MaxSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.MaxSize')
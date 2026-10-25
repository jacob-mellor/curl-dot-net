#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException')

## CurlFileSizeExceededException\.MaxSize Property

The size limit set with `--max-filesize`, in bytes, that the response was not allowed to exceed\.

```csharp
public long MaxSize { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte count\.

### Remarks

Compare with [ActualSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.ActualSize') to decide on a new limit.

### See Also
- [ActualSize](CurlDotNet.Exceptions.CurlFileSizeExceededException.ActualSize.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException\.ActualSize')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')

## CurlRangeErrorException\.Offset Property

The resume offset that was requested, in bytes \(the size of the partial file for `-C -`\)\.

```csharp
public long Offset { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte offset greater than 0\.

### Remarks

The partial file is left untouched so you can decide whether to delete it.

### See Also
- [StatusCode](CurlDotNet.Exceptions.CurlRangeErrorException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.StatusCode')
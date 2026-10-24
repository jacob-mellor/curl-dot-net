#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRangeErrorException](CurlDotNet.Exceptions.CurlRangeErrorException.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException')

## CurlRangeErrorException\.StatusCode Property

The HTTP status of the response that did not honour the range, usually 200 \(whole document\) instead of 206\.

```csharp
public int StatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
An HTTP status code\.

### Remarks

A 4xx/5xx status here means the resource itself failed, not just the range.

### See Also
- [Offset](CurlDotNet.Exceptions.CurlRangeErrorException.Offset.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.Offset')
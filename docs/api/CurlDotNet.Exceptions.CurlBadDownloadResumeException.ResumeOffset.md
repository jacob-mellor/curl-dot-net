#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlBadDownloadResumeException](CurlDotNet.Exceptions.CurlBadDownloadResumeException.md 'CurlDotNet\.Exceptions\.CurlBadDownloadResumeException')

## CurlBadDownloadResumeException\.ResumeOffset Property

The byte offset the download tried to resume from \(the size of the partial local file for `-C -`\)\.

```csharp
public long ResumeOffset { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
A byte offset, 0 or more\.

### Remarks

Compare it with the remote file's Content-Length from a HEAD request (`curl -I`).

### See Also
- [Offset](CurlDotNet.Exceptions.CurlRangeErrorException.Offset.md 'CurlDotNet\.Exceptions\.CurlRangeErrorException\.Offset')
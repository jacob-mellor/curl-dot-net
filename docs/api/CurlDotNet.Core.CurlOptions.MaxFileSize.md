#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.MaxFileSize Property

Largest response body, in bytes, the transfer accepts, like curl's `--max-filesize`; a larger announced
or received body fails with exit code 63 instead of filling memory or disk\.

```csharp
public System.Nullable<long> MaxFileSize { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
Bytes, or null \(default\) for no limit\.

### Remarks

The parser accepts a plain byte count or one with a `k`, `M`, `G`, `T` or `P` suffix (powers of
            1024), for example `--max-filesize 10M`. An announced `Content-Length` over the limit fails before the body is
            read; otherwise the transfer stops once the limit is passed. The error is
            [CurlFileSizeExceededException](CurlDotNet.Exceptions.CurlFileSizeExceededException.md 'CurlDotNet\.Exceptions\.CurlFileSizeExceededException').

Example: `curl --max-filesize 500k https://example.com/avatar.png` is `new CurlOptions { Url = url, MaxFileSize = 500 * 1024 }`.

### See Also
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
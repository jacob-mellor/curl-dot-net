#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.LimitRate Property

Caps the download speed at this many bytes per second, like curl's `--limit-rate`; the body is read more
slowly so the average rate stays at or below the cap\.

```csharp
public long LimitRate { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
Bytes per second; the parser accepts suffixes `k`, `M`, `G`, `T`, `P` \(powers of
            1024\), so `100k` is 102400\. Default: 0 \(no cap\)\.

### Remarks

Applies to http and https response bodies: after each chunk the reader waits until the average rate since
            the body started is back under the cap (curl averages the same way, so short bursts are possible). Uploads are
            not throttled. A malformed value on the command line fails with exit code 2.

Example: `curl --limit-rate 100k -O https://example.com/big.iso` is
            `new CurlOptions { Url = url, UseRemoteFileName = true, LimitRate = 100 * 1024 }`.

### See Also
- [SpeedLimit](CurlDotNet.Core.CurlOptions.SpeedLimit.md 'CurlDotNet\.Core\.CurlOptions\.SpeedLimit')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
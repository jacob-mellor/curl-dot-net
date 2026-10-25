#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.SpeedLimit Property

Aborts a transfer that stays slower than this many bytes per second for [SpeedTime](CurlDotNet.Core.CurlOptions.SpeedTime.md 'CurlDotNet\.Core\.CurlOptions\.SpeedTime') seconds, like
curl's `-Y`/`--speed-limit`; the transfer then fails with exit code 28\.

```csharp
public long SpeedLimit { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
Bytes per second; 0 \(the default\) turns the check off\. When [SpeedTime](CurlDotNet.Core.CurlOptions.SpeedTime.md 'CurlDotNet\.Core\.CurlOptions\.SpeedTime') is 0, curl's default
            of 30 seconds is used\.

### Remarks

The rate is measured while the response body downloads (http and https), over a sliding window of
            [SpeedTime](CurlDotNet.Core.CurlOptions.SpeedTime.md 'CurlDotNet\.Core\.CurlOptions\.SpeedTime') seconds. When it stays below the limit for the whole window the transfer is aborted with
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit 28) and curl's message
            `Operation too slow. Less than N bytes/sec transferred the last S seconds`. Use it to give up on stalled
            downloads without capping a healthy one with [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime'). A non-numeric value on the command line fails
            with exit code 2.

Example: `curl --speed-limit 1000 --speed-time 30 URL` is
            `new CurlOptions { Url = url, SpeedLimit = 1000, SpeedTime = 30 }`.

### See Also
- [SpeedTime](CurlDotNet.Core.CurlOptions.SpeedTime.md 'CurlDotNet\.Core\.CurlOptions\.SpeedTime')
- [LimitRate](CurlDotNet.Core.CurlOptions.LimitRate.md 'CurlDotNet\.Core\.CurlOptions\.LimitRate')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
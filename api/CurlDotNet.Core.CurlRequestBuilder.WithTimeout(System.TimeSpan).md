#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithTimeout\(TimeSpan\) Method

Limits the whole transfer \(connect, send, wait and download\) to [timeout](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md#CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).timeout 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)\.timeout')
\(curl's `-m` / `--max-time`\); without it there is no limit, like curl\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Maximum total time; [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means unlimited\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The TimeSpan is converted to seconds with decimals, so `TimeSpan.FromMilliseconds(1500)` is
            `--max-time 1.5`. [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means no limit. When the limit expires the request
            fails with [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28). [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
            sets a process-wide default for requests that do not set one.

Example: `CurlRequestBuilder.Get(url).WithTimeout(TimeSpan.FromSeconds(30)).WithConnectTimeout(TimeSpan.FromSeconds(5))`
            is `curl --max-time 30 --connect-timeout 5 URL`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
- [WithTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)')
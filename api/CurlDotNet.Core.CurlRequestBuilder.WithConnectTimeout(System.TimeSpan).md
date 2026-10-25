#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithConnectTimeout\(TimeSpan\) Method

Limits only the connection phase \(DNS lookup, TCP connect and TLS handshake\) to
[timeout](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md#CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).timeout 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)\.timeout') \(curl's `--connect-timeout`\); the download itself is not limited\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithConnectTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Maximum time to establish the connection; [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means no limit\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Converted to seconds with decimals (`TimeSpan.FromMilliseconds(500)` is
            `--connect-timeout 0.5`). [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means the default (no extra limit). An unreachable
            host then fails fast with [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) instead of waiting
            for the operating system's TCP timeout.

Example: `.WithConnectTimeout(TimeSpan.FromSeconds(3))` for a health check that should give up
            quickly on a dead server.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)')
- [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout')
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithConnectTimeout\(System\.TimeSpan\)')
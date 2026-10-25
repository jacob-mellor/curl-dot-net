#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithConnectTimeout\(TimeSpan\) Method

Limits only the connection phase \(DNS, TCP connect, TLS handshake\) of every request, like curl's
`--connect-timeout`, so a dead server fails fast while slow downloads may continue\.

```csharp
public CurlDotNet.Lib.LibCurl WithConnectTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Maximum time to establish each connection; [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means no limit\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Converted to seconds with decimals. When it expires the result carries a
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception').

Example: `new LibCurl().WithConnectTimeout(TimeSpan.FromSeconds(3))` for health checks.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [WithTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)')
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)')
- [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout')
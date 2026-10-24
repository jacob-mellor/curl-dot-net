#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithTimeout\(TimeSpan\) Method

Limits every request's total time \(connect, send, wait, download\) to [timeout](CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).md#CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).timeout 'CurlDotNet\.Lib\.LibCurl\.WithTimeout\(System\.TimeSpan\)\.timeout'), like
curl's `-m` / `--max-time`; without it there is no limit\.

```csharp
public CurlDotNet.Lib.LibCurl WithTimeout(System.TimeSpan timeout);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithTimeout(System.TimeSpan).timeout'></a>

`timeout` [System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')

Maximum total time per request; [System\.TimeSpan\.Zero](https://learn.microsoft.com/en-us/dotnet/api/system.timespan.zero 'System\.TimeSpan\.Zero') means unlimited\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Converted to seconds with decimals (`TimeSpan.FromMilliseconds(2500)` is 2.5 s). When it expires the
            result carries a [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) in [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception').
            A request can override it with `opts => opts.MaxTime = 300`.

Example: `new LibCurl().WithTimeout(TimeSpan.FromSeconds(30)).WithConnectTimeout(TimeSpan.FromSeconds(5))`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Lib.LibCurl.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Lib\.LibCurl\.WithConnectTimeout\(System\.TimeSpan\)')
- [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
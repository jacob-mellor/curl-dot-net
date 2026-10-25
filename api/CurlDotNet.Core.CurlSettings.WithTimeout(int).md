#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithTimeout\(int\) Method

Limits the whole transfer to [seconds](CurlDotNet.Core.CurlSettings.WithTimeout(int).md#CurlDotNet.Core.CurlSettings.WithTimeout(int).seconds 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)\.seconds') \(curl `-m`/`--max-time`\) and returns these
settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithTimeout(int seconds);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithTimeout(int).seconds'></a>

`seconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Whole seconds, for example 30; 0 or negative means no limit\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds'), which overrides any `-m` in the command. A transfer that runs
            longer throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28).

Example: `await Curl.ExecuteAsync("curl https://slow.example.com", new CurlSettings().WithTimeout(10));`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds')
- [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)')
- [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)')
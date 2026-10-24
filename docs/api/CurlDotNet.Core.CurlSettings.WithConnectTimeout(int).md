#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithConnectTimeout\(int\) Method

Limits only the connection phase to [seconds](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md#CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).seconds 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)\.seconds') \(curl `--connect-timeout`\) and returns
these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithConnectTimeout(int seconds);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).seconds'></a>

`seconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Whole seconds, for example 5\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds'). A host that does not accept the connection in time throws
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28).

Example: `new CurlSettings().WithConnectTimeout(3).WithTimeout(120)` - fail fast when the server is
            down, but allow two minutes for a large download.

### See Also
- [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds')
- [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)')
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)')
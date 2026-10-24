#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.ConnectTimeoutSeconds Property

Limit for establishing the connection \(DNS, TCP and TLS\) in whole seconds, like curl's
`--connect-timeout`; exceeding it throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') \(exit code 28\)\.

```csharp
public System.Nullable<int> ConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
Seconds; null \(the default\) keeps the command's `--connect-timeout` or
            [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')\.

### Remarks

Copied to [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout'). It does not limit the download itself; combine it
            with [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') for that.

Example: `new CurlSettings { ConnectTimeoutSeconds = 5, MaxTimeSeconds = 60 }` fails fast when the
            host is down but allows a slow download a full minute.

### See Also
- [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)')
- [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds')
- [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.KeepAliveTime Property

Seconds from curl's `--keepalive-time` \(idle time before TCP keep\-alive probes\); only used by FTP
transfers, where any value turns keep\-alive on\.

```csharp
public System.Nullable<int> KeepAliveTime { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
Whole seconds, or null\. Default: null\.

### Remarks

HTTP transfers use the pooled connection settings of HttpClient regardless of this value.
            Example: `curl --keepalive-time 60 ftp://example.com/file` parses to `KeepAliveTime = 60`.

### See Also
- [FtpPassive](CurlDotNet.Core.CurlOptions.FtpPassive.md 'CurlDotNet\.Core\.CurlOptions\.FtpPassive')
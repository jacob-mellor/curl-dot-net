#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ConnectTimeout Property

The longest the connection phase \(DNS lookup, TCP connect and TLS handshake\) may take, in seconds with
decimals allowed, like curl's `--connect-timeout`; expiry fails with exit code 28\.

```csharp
public System.Nullable<double> ConnectTimeout { get; set; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
Seconds, for example `5` or `0.5`\. Default: null, which uses
            [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds') when that is above 0, otherwise no separate connect limit\.

### Remarks

Once connected, only [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime') limits the transfer. A short connect timeout with a longer
            [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime') fails fast on a dead host while still allowing large downloads.

Example: `curl --connect-timeout 5 -m 300 https://example.com/big.iso` is
            `new CurlOptions { Url = url, ConnectTimeout = 5, MaxTime = 300 }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
- [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)')
- [ConnectTimeoutSeconds](CurlDotNet.Core.CurlSettings.ConnectTimeoutSeconds.md 'CurlDotNet\.Core\.CurlSettings\.ConnectTimeoutSeconds')
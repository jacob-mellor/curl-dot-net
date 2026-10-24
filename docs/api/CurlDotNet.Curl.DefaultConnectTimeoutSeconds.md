#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultConnectTimeoutSeconds Property

A process\-wide limit, in whole seconds, on how long establishing a connection may take, as if
`--connect-timeout` were added to every command that does not set its own\. 0 \(the default\) means no limit\.

```csharp
public static int DefaultConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Seconds; 0 \(default\) or less means no connect limit\.

### Remarks

This covers the DNS lookup, the TCP connect and the TLS handshake only, not the response. Values: 0 or
            less means no connect limit; a positive number is seconds. A command's own `--connect-timeout 5`
            (decimals allowed, such as `--connect-timeout 2.5`) or [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)')
            wins over this default.

When it expires the call throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) with a
            message such as `Connection timed out after 5000 milliseconds`.

Tip: set it well below [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds') so that dead servers fail fast while slow
            downloads still get time. Process-wide and thread-safe; applies to every protocol.

Example: `Curl.DefaultConnectTimeoutSeconds = 10;` then a request to a server that never accepts
            the connection fails after 10 seconds instead of the operating system's default (often over a minute).

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
- [ConnectTimeout](CurlDotNet.Core.CurlOptions.ConnectTimeout.md 'CurlDotNet\.Core\.CurlOptions\.ConnectTimeout')
- [WithConnectTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithConnectTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithConnectTimeout\(int\)')
- [WithConnectTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithConnectTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithConnectTimeout\(System\.TimeSpan\)')
- [DefaultConnectTimeoutSeconds](CurlDotNet.DotNetCurl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultConnectTimeoutSeconds')
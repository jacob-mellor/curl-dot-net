#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.DefaultConnectTimeoutSeconds Property

Process\-wide limit in whole seconds for establishing a connection \(DNS, TCP, TLS\), like
`--connect-timeout`; 0 means no limit\. The same setting as
[DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')\.

```csharp
public static int DefaultConnectTimeoutSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Seconds; default 0 \(no limit\)\.

### Remarks

0 or less means no limit; a command's own `--connect-timeout` wins. Thread-safe.

Example: `DotNetCurl.DefaultConnectTimeoutSeconds = 10;` so dead servers fail after 10 seconds.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')
- [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds')
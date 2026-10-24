#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.DefaultMaxTimeSeconds Property

Process\-wide limit in whole seconds for every HTTP transfer, like adding `-m`/`--max-time` to each
command; 0 means no limit\. The same setting as [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')\.

```csharp
public static int DefaultMaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Seconds; default 0 \(no limit\)\.

### Remarks

0 or less means no limit; a command's own `-m` wins. When it expires the call throws
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28). Thread-safe.

Example: `DotNetCurl.DefaultMaxTimeSeconds = 30;`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [DefaultMaxTimeSeconds](CurlDotNet.Curl.DefaultMaxTimeSeconds.md 'CurlDotNet\.Curl\.DefaultMaxTimeSeconds')
- [DefaultConnectTimeoutSeconds](CurlDotNet.DotNetCurl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultConnectTimeoutSeconds')
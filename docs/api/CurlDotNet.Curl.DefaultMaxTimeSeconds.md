#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.DefaultMaxTimeSeconds Property

A process\-wide limit, in whole seconds, on how long any HTTP transfer may take, as if `-m`/`--max-time`
were added to every command that does not set its own\. 0 \(the default\) means no limit\.

```csharp
public static int DefaultMaxTimeSeconds { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
Seconds; 0 \(default\) or less means no limit\.

### Example

```csharp
Curl.DefaultMaxTimeSeconds = 30;
await Curl.ExecuteAsync("curl https://slow-api.example.com");                 // times out after 30 s
await Curl.ExecuteAsync("curl --max-time 60 https://very-slow-api.example.com"); // 60 s for this one
```

### Remarks

Values: 0 or a negative number means no limit (like curl without `--max-time`); any positive
            number is the total time allowed for connect, request, response headers and body together. A command's own
            `-m 60` / `--max-time 60` (decimals allowed there, such as `-m 2.5`), or
            [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)'), wins over this default.

When the limit expires the call throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (a
            [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException'), exit code 28) whose message says how many bytes had arrived. Your
            own [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken') is separate: cancelling it throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException').

Process-wide and thread-safe; read at the start of every transfer. Applies to http:// and https://.

Example: `Curl.DefaultMaxTimeSeconds = 30;` then `await Curl.ExecuteAsync("curl https://slow.example.com")`
            fails after 30 seconds, while `await Curl.ExecuteAsync("curl -m 120 https://slow.example.com/big.zip -o big.zip")`
            gets 120.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [DefaultConnectTimeoutSeconds](CurlDotNet.Curl.DefaultConnectTimeoutSeconds.md 'CurlDotNet\.Curl\.DefaultConnectTimeoutSeconds')
- [MaxTime](CurlDotNet.Core.CurlOptions.MaxTime.md 'CurlDotNet\.Core\.CurlOptions\.MaxTime')
- [WithTimeout\(int\)](CurlDotNet.Core.CurlSettings.WithTimeout(int).md 'CurlDotNet\.Core\.CurlSettings\.WithTimeout\(int\)')
- [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)')
- [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.RetryDelay Property

Fixed number of seconds to wait between retries \(decimals allowed\), like curl's `--retry-delay`; without it
curl's exponential backoff \(1, 2, 4\.\.\. seconds\) is used\.

```csharp
public double RetryDelay { get; set; }
```

#### Property Value
[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')  
Seconds\. Default: 0 \(use the server's `Retry-After` or exponential backoff\)\.

### Remarks

Like curl, the wait before a retry is the larger of this delay (or the backoff) and the server's
            `Retry-After` header, capped at 600 seconds. A negative or non-numeric value on the command line fails with exit code 2.
            Example: `curl --retry 5 --retry-delay 0.5 URL` is `new CurlOptions { Url = url, Retry = 5, RetryDelay = 0.5 }`.

### See Also
- [Retry](CurlDotNet.Core.CurlOptions.Retry.md 'CurlDotNet\.Core\.CurlOptions\.Retry')
- [RetryDelayMs](CurlDotNet.Core.CurlSettings.RetryDelayMs.md 'CurlDotNet\.Core\.CurlSettings\.RetryDelayMs')
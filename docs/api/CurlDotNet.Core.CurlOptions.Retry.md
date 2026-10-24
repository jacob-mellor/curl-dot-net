#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Retry Property

How many times a transfer that failed with a transient error is retried, like curl's `--retry N`;
transient means a timeout, a host or proxy name that did not resolve, or an HTTP 408, 429, 500, 502, 503 or 504\.

```csharp
public int Retry { get; set; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
A retry count\. Default: 0 \(no retries\)\.

### Remarks

Between attempts CurlDotNet waits [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay') seconds when set; otherwise curl's backoff (1, 2, 4...
            seconds, at most 10 minutes), or the server's `Retry-After` (whole seconds) when that is longer.
            [RetryMaxTime](CurlDotNet.Core.CurlOptions.RetryMaxTime.md 'CurlDotNet\.Core\.CurlOptions\.RetryMaxTime') bounds the total time. [RetryAllErrors](CurlDotNet.Core.CurlOptions.RetryAllErrors.md 'CurlDotNet\.Core\.CurlOptions\.RetryAllErrors') and [RetryConnRefused](CurlDotNet.Core.CurlOptions.RetryConnRefused.md 'CurlDotNet\.Core\.CurlOptions\.RetryConnRefused')
            widen what counts as transient. When the last attempt fails, its error is thrown. Each attempt's output is kept
            in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'); the [WriteOut](CurlDotNet.Core.CurlOptions.WriteOut.md 'CurlDotNet\.Core\.CurlOptions\.WriteOut') text is written once, for the last attempt, with
            `%{num_retries}` set to the number of retries made.

Example: `curl --retry 3 --retry-delay 2 URL` is `new CurlOptions { Url = url, Retry = 3, RetryDelay = 2 }`.

### See Also
- [RetryDelay](CurlDotNet.Core.CurlOptions.RetryDelay.md 'CurlDotNet\.Core\.CurlOptions\.RetryDelay')
- [RetryMaxTime](CurlDotNet.Core.CurlOptions.RetryMaxTime.md 'CurlDotNet\.Core\.CurlOptions\.RetryMaxTime')
- [RetryAllErrors](CurlDotNet.Core.CurlOptions.RetryAllErrors.md 'CurlDotNet\.Core\.CurlOptions\.RetryAllErrors')
- [RetryConnRefused](CurlDotNet.Core.CurlOptions.RetryConnRefused.md 'CurlDotNet\.Core\.CurlOptions\.RetryConnRefused')
- [RetryCount](CurlDotNet.Core.CurlSettings.RetryCount.md 'CurlDotNet\.Core\.CurlSettings\.RetryCount')
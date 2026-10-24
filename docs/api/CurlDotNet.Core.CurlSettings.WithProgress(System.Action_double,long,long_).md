#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithProgress\(Action\<double,long,long\>\) Method

Registers a callback that receives `(percent, bytesReceived, totalBytes)` for every chunk of the response
body, and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithProgress(System.Action<double,long,long> callback);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).callback'></a>

`callback` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')

Called on a thread\-pool thread once per network read; null removes the callback\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Sets [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress'). `percent` is 0-100, or -1 when the server sent no
            `Content-Length`; `bytesReceived` counts body bytes so far; `totalBytes` is the announced size or
            -1. Throwing from the callback aborts the transfer with [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') (exit code 42).

Example: `await Curl.ExecuteAsync("curl -o big.zip https://example.com/big.zip", new CurlSettings().WithProgress((pct, got, total) => Console.Write($"\r{pct:F0}% ({got:N0}/{total:N0})")));`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress')
- [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')
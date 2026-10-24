#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.ProgressHandler Property

Called for every chunk of the response body as it downloads with \(percent, bytes received, total bytes\),
so you can drive a progress bar; curl's progress callback\.

```csharp
public System.Action<double,long,long>? ProgressHandler { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')  
A callback, or null \(default\) for no progress reports\.

### Remarks

Arguments: `percent` is 0 to 100, or `-1` when the server sent no `Content-Length` (chunked
            responses); `bytesReceived` is the running total of body bytes; `totalBytes` is the announced length, or
            `-1` when unknown. It is invoked on the thread reading the response, once per chunk (up to 80 KB), for
            http and https bodies only (not HEAD, FTP or file://). Keep it fast and marshal to your UI thread yourself.

A handler that throws aborts the transfer with [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')
            (exit 42, like a curl progress callback returning non-zero); your exception is its `InnerException`.

Example: `options.ProgressHandler = (pct, got, total) => Console.Write($"\r{(pct < 0 ? got + " bytes" : pct.ToString("F0") + "%")}");`.
            With a command string use `new CurlSettings().WithProgress(...)`.
            Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/10-files-and-downloads.md

### See Also
- [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)')
- [OnProgress](CurlDotNet.Core.CurlSettings.OnProgress.md 'CurlDotNet\.Core\.CurlSettings\.OnProgress')
- [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')
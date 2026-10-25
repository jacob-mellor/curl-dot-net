#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.OnProgress Property

Callback invoked for every chunk of the response body as it downloads, with
`(percent, bytesReceived, totalBytes)`\.

```csharp
public System.Action<double,long,long> OnProgress { get; set; }
```

#### Property Value
[System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Double](https://learn.microsoft.com/en-us/dotnet/api/system.double 'System\.Double')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[,](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-3 'System\.Action\`3')  
Null \(the default\) for no callback\. Copied to [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')\.

### Remarks

Arguments: `percent` is 0 to 100, or -1 when the server did not send a `Content-Length`
            (chunked responses); `bytesReceived` is the body bytes received so far; `totalBytes` is the announced
            length, or -1 when unknown. The last call has `bytesReceived` equal to the body size.

It runs on the thread pool, once per network read (up to 80 KB), for HTTP and HTTPS response bodies;
            it is not called for HEAD requests, uploads or FTP/file transfers. Keep it fast and marshal to the UI thread
            yourself. If it throws, the transfer stops with [CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException') (exit code 42)
            carrying your exception as [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException') - throwing is the way to abort a download.

Example: `OnProgress = (pct, got, total) => Console.Write(pct < 0 ? $"\r{got:N0} bytes" : $"\r{pct:F1}% of {total:N0}")`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [WithProgress\(Action&lt;double,long,long&gt;\)](CurlDotNet.Core.CurlSettings.WithProgress(System.Action_double,long,long_).md 'CurlDotNet\.Core\.CurlSettings\.WithProgress\(System\.Action\<double,long,long\>\)')
- [ProgressHandler](CurlDotNet.Core.CurlOptions.ProgressHandler.md 'CurlDotNet\.Core\.CurlOptions\.ProgressHandler')
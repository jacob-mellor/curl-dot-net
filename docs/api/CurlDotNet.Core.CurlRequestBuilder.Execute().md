#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Execute\(\) Method

Sends the request and blocks the calling thread until the response is complete; the synchronous
form of [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)') for console apps, scripts and tests\.

```csharp
public CurlDotNet.Core.CurlResult Execute();
```

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode'), [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'),
            [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \(text\) or [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') \(binary\), saved paths in
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles') and [Timings](CurlDotNet.Core.CurlResult.Timings.md 'CurlDotNet\.Core\.CurlResult\.Timings')\. HTTP error statuses are returned
            as results unless [FailOnError\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FailOnError\(bool\)') was used\.

#### Exceptions

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed\. The subtype and [CurlErrorCode](CurlDotNet.Exceptions.CurlException.CurlErrorCode.md 'CurlDotNet\.Exceptions\.CurlException\.CurlErrorCode')
            say why, for example [CurlCouldntResolveHostException](CurlDotNet.Exceptions.CurlCouldntResolveHostException.md 'CurlDotNet\.Exceptions\.CurlCouldntResolveHostException') \(6, wrong host name\),
            [CurlCouldntConnectException](CurlDotNet.Exceptions.CurlCouldntConnectException.md 'CurlDotNet\.Exceptions\.CurlCouldntConnectException') \(7, server down or port closed\),
            [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') \(28, raise [WithTimeout\(TimeSpan\)](CurlDotNet.Core.CurlRequestBuilder.WithTimeout(System.TimeSpan).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithTimeout\(System\.TimeSpan\)')\),
            [CurlPeerFailedVerificationException](CurlDotNet.Exceptions.CurlPeerFailedVerificationException.md 'CurlDotNet\.Exceptions\.CurlPeerFailedVerificationException') \(60, untrusted certificate\) or
            [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') \(22, with [FailOnError\(bool\)](CurlDotNet.Core.CurlRequestBuilder.FailOnError(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.FailOnError\(bool\)')\)\. Each message says what
            happened, how to fix it and links the page; see
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-error\-handling\-csharp/README\.md

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

Prefer [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)') in ASP.NET, UI and other async code: blocking
            a thread pool or UI thread on network I/O wastes threads and can freeze the UI.

Example: `var result = CurlRequestBuilder.Get("https://api.example.com/health").Execute(); Console.WriteLine(result.StatusCode);`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [ExecuteAsync\(CancellationToken\)](CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync.md#CurlDotNet.Core.CurlRequestBuilder.ExecuteAsync(System.Threading.CancellationToken) 'CurlDotNet\.Core\.CurlRequestBuilder\.ExecuteAsync\(System\.Threading\.CancellationToken\)')
- [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')
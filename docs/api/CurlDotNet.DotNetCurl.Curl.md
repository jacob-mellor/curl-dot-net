#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.Curl Method

| Overloads | |
| :--- | :--- |
| [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)') | Runs a curl command and blocks the calling thread until the response arrives; same as [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')\. |
| [Curl\(string, int\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)') | Runs a curl command, blocking the calling thread, and cancels it if it has not finished after [timeoutSeconds](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.timeoutSeconds') seconds\. |

<a name='CurlDotNet.DotNetCurl.Curl(string)'></a>

## DotNetCurl\.Curl\(string\) Method

Runs a curl command and blocks the calling thread until the response arrives; same as
[Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Curl(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, for example `curl -H 'Accept: application/json' https://api.example.com`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): status code, body \(standard output text\), headers, binary data, written
             files and timings\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string).command 'CurlDotNet\.DotNetCurl\.Curl\(string\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command could not be read or the transfer failed \(DNS 6, connect 7,
             timeout 28, TLS 35/60, `-f` HTTP error 22\.\.\.\); see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
             See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var response = DotNetCurl.Curl("curl https://api.example.com/data");
Console.WriteLine(response.Body);

// Works without "curl" prefix
var data = DotNetCurl.Curl("https://api.example.com/data");
```

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
             or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
             thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
             progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
             can still deadlock. Prefer the async method wherever the caller can await.

Paste any curl command, with or without the leading `curl`; it is read with
             [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor'). HTTP error statuses are returned in the result, not
             thrown (add `-f` to throw). Blocks the thread: do not call it on a UI thread; use
             [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') in async code. Exceptions are thrown directly, not wrapped in
             [System\.AggregateException](https://learn.microsoft.com/en-us/dotnet/api/system.aggregateexception 'System\.AggregateException').

Example: `var data = DotNetCurl.Curl("https://api.example.com/data");` then `Console.WriteLine(data.Body);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)')
- [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')

<a name='CurlDotNet.DotNetCurl.Curl(string,int)'></a>

## DotNetCurl\.Curl\(string, int\) Method

Runs a curl command, blocking the calling thread, and cancels it if it has not finished after
[timeoutSeconds](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.timeoutSeconds') seconds\.

```csharp
public static CurlDotNet.Core.CurlResult Curl(string command, int timeoutSeconds);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.Curl(string,int).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)')\.

<a name='CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds'></a>

`timeoutSeconds` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

Seconds to wait before cancelling, for example `30`; must be 0 or more\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') when the request finished in time\.

#### Exceptions

[System\.ArgumentOutOfRangeException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentoutofrangeexception 'System\.ArgumentOutOfRangeException')  
[timeoutSeconds](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.timeoutSeconds') is negative\. Fix: pass the
            number of seconds to wait\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-timeout\-csharp/README\.md

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The request did not finish within [timeoutSeconds](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).timeoutSeconds 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.timeoutSeconds')\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.Curl(string,int).command 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed before the deadline; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

The deadline is a cancellation, not curl's `--max-time`: when it passes, the call throws
            [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException') (not [CurlTimeoutException](CurlDotNet.Exceptions.CurlTimeoutException.md 'CurlDotNet\.Exceptions\.CurlTimeoutException')). If you want curl's exit code
            28 and message instead, put `-m 30` in the command or use [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds').

Values: a positive number of seconds; 0 cancels immediately. Negative values are rejected.

Example: `var result = DotNetCurl.Curl("curl https://slow.example.com/report", 30);` gives up after 30 seconds.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)')
- [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)')
- [DefaultMaxTimeSeconds](CurlDotNet.DotNetCurl.DefaultMaxTimeSeconds.md 'CurlDotNet\.DotNetCurl\.DefaultMaxTimeSeconds')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.Execute Method

| Overloads | |
| :--- | :--- |
| [Execute\(CurlOptions\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.Execute\(CurlDotNet\.Core\.CurlOptions\)') | Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object and blocks the calling thread until the response arrives; nothing is parsed, so data with quotes or spaces needs no escaping\. |
| [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)') | Runs a curl command and blocks the calling thread until the response arrives; the synchronous twin of [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') for console tools, scripts and code that cannot be async\. |
| [Execute\(string, CurlSettings\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command with timeouts, retries, extra headers or a progress callback from a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object, blocking the calling thread until the response arrives\. |
| [Execute\(string, CurlFlavor\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)') | Executes a curl command copied from a specific shell and blocks the calling thread until the response arrives\. |
| [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)') | Runs a curl command, blocking the calling thread until the response arrives or [cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; the synchronous twin of [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |

<a name='CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions)'></a>

## Curl\.Execute\(CurlOptions\) Method

Sends a request described by a [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions') object and blocks the calling thread until the
response arrives; nothing is parsed, so data with quotes or spaces needs no escaping\.

```csharp
public static CurlDotNet.Core.CurlResult Execute(CurlDotNet.Core.CurlOptions options);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions).options'></a>

`options` [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

The request to send; [Url](CurlDotNet.Core.CurlOptions.Url.md 'CurlDotNet\.Core\.CurlOptions\.Url') must be set to an absolute URL\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[options](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(CurlDotNet.Core.CurlOptions).options 'CurlDotNet\.Curl\.Execute\(CurlDotNet\.Core\.CurlOptions\)\.options') is null\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
No URL \(exit code 2, like curl's "no URL specified"\) or the transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var result = Curl.Execute(new CurlOptions { Url = "https://api.example.com", Method = "DELETE" });
```

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

The synchronous twin of [ExecuteAsync\(CurlOptions\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)'); see it for how options map to curl flags.

Example: `var result = Curl.Execute(new CurlOptions { Url = "https://api.example.com/items/7", Method = "DELETE" });`
            then `Console.WriteLine(result.StatusCode);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(CurlDotNet.Core.CurlOptions) 'CurlDotNet\.Curl\.ExecuteAsync\(CurlDotNet\.Core\.CurlOptions\)')
- [PerformAsync\(CurlOptions, CancellationToken\)](CurlDotNet.Lib.LibCurl.PerformAsync(CurlDotNet.Core.CurlOptions,System.Threading.CancellationToken).md 'CurlDotNet\.Lib\.LibCurl\.PerformAsync\(CurlDotNet\.Core\.CurlOptions, System\.Threading\.CancellationToken\)')

<a name='CurlDotNet.Curl.Execute(string)'></a>

## Curl\.Execute\(string\) Method

Runs a curl command and blocks the calling thread until the response arrives; the synchronous twin of
[ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') for console tools, scripts and code that cannot be async\.

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, for example `curl -H 'Accept: application/json' https://api.example.com`;
            the leading `curl` is optional\. Read with [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): status code, body \(standard output text\), headers, binary data and
            written files, as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string).command 'CurlDotNet\.Curl\.Execute\(string\)\.command') is null, empty or whitespace\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/run\-curl\-command\-in\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command could not be read or the transfer failed; the subtypes \(DNS 6,
            connect 7, timeout 28, TLS 35/60, `-f` HTTP error 22\.\.\.\) are listed on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
static void Main()
{
    var result = Curl.Execute("curl https://api.example.com");
    Console.WriteLine(result.Body);
}
```

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

Behaves exactly like [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') (same command format, same result, same
            exceptions - thrown directly, not wrapped in [System\.AggregateException](https://learn.microsoft.com/en-us/dotnet/api/system.aggregateexception 'System\.AggregateException')) but waits for the result. The
            thread does nothing else meanwhile, so on a UI thread the window freezes and in ASP.NET request code it reduces
            throughput; use the async method there.

Example: `var result = Curl.Execute("curl -s https://api.example.com/status");` then
            `Console.WriteLine(result.Body);` in a `static void Main()`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [Execute\(string, CancellationToken\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)')
- [Execute\(\)](CurlDotNet.Core.CurlRequestBuilder.Execute().md 'CurlDotNet\.Core\.CurlRequestBuilder\.Execute\(\)')
- [Curl\(string\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)')

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings)'></a>

## Curl\.Execute\(string, CurlSettings\) Method

Runs a curl command with timeouts, retries, extra headers or a progress callback from a
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object, blocking the calling thread until the response arrives\.

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

Settings applied on top of the command, built with `new CurlSettings().With...()`;
            null runs the command unchanged\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings).command 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)\.command') is null, empty or whitespace\.

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The settings' cancellation token was cancelled\.

[CurlAbortedByCallbackException](CurlDotNet.Exceptions.CurlAbortedByCallbackException.md 'CurlDotNet\.Exceptions\.CurlAbortedByCallbackException')  
The progress callback threw \(exit code 42\)\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed after all retries; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

Applies the same settings as [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)'): `MaxTimeSeconds`
            (`-m`), `ConnectTimeoutSeconds` (`--connect-timeout`), `FollowRedirects` (`-L`),
            `Insecure` (`-k`), `Headers` (`-H`), `OnProgress`, `RetryCount`/`RetryDelayMs`,
            `Flavor` and `CancellationToken`. The progress callback runs on a thread-pool thread, not the
            blocked caller.

Example: `var result = Curl.Execute("curl https://flaky.example.com/api", new CurlSettings().WithRetries(3, 1000).WithTimeout(20));`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-synchronously-in-csharp/README.md

### See Also
- [Async version \(preferred\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')
- [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor)'></a>

## Curl\.Execute\(string, CurlFlavor\) Method

Executes a curl command copied from a specific shell and blocks the calling thread until the response
arrives\.

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, CurlDotNet.CurlFlavor flavor);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The command exactly as it was pasted, with or without the leading `curl` /
            `curl.exe` / `Invoke-WebRequest`\.

<a name='CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor).flavor'></a>

`flavor` [CurlFlavor](CurlDotNet.CurlFlavor.md 'CurlDotNet\.CurlFlavor')

Where it came from: [Posix](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Posix 'CurlDotNet\.CurlFlavor\.Posix') \(bash/zsh\),
            [WindowsCmd](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.WindowsCmd 'CurlDotNet\.CurlFlavor\.WindowsCmd') \(curl\.exe in cmd\.exe\), [PowerShell](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.PowerShell 'CurlDotNet\.CurlFlavor\.PowerShell') \(curl\.exe in
            PowerShell\), [InvokeWebRequest](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.InvokeWebRequest 'CurlDotNet\.CurlFlavor\.InvokeWebRequest') \(Invoke\-WebRequest / Invoke\-RestMethod syntax\) or
            [Auto](CurlDotNet.CurlFlavor.md#CurlDotNet.CurlFlavor.Auto 'CurlDotNet\.CurlFlavor\.Auto') to detect it\. Overrides [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor') for this call\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,CurlDotNet.CurlFlavor).command 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.CurlFlavor\)\.command') is null, empty or whitespace\.

[CurlInvalidCommandException](CurlDotNet.Exceptions.CurlInvalidCommandException.md 'CurlDotNet\.Exceptions\.CurlInvalidCommandException')  
The text breaks the flavor's rules or uses an
            untranslatable `Invoke-WebRequest` parameter; the message says how to fix it\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-flavors\-bash\-cmd\-powershell/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The request fails; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

Blocks the thread; prefer [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)') in async code. See that method
            for what changes between flavors.

Example: `var result = Curl.Execute("curl -H \"Accept: application/json\" -d \"{\\\"a\\\":1}\" https://api.example.com", CurlFlavor.WindowsCmd);`
            sends the body `{"a":1}`.

Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-flavors-bash-cmd-powershell/README.md

### See Also
- [ExecuteAsync\(string, CurlFlavor\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.CurlFlavor) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.CurlFlavor\)')
- [DefaultFlavor](CurlDotNet.Curl.DefaultFlavor.md 'CurlDotNet\.Curl\.DefaultFlavor')

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken)'></a>

## Curl\.Execute\(string, CancellationToken\) Method

Runs a curl command, blocking the calling thread until the response arrives or
[cancellationToken](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; the synchronous twin of
[ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\.

```csharp
public static CurlDotNet.Core.CurlResult Execute(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

<a name='CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancels the request when signalled, for example from a
            `CancellationTokenSource` with a timeout; `CancellationToken.None` for no cancellation\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'), as described on [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The token was cancelled before the transfer finished\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.Curl.md#CurlDotNet.Curl.Execute(string,System.Threading.CancellationToken).command 'CurlDotNet\.Curl\.Execute\(string, System\.Threading\.CancellationToken\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

The token lets another thread (or a timer) stop the request; the call still blocks until then. Cancelling
            throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException'). Prefer the async overload whenever the caller can await.

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));` then
            `var result = Curl.Execute("curl https://api.example.com", cts.Token);` gives up after 30 seconds.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [Async version \(preferred\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')
- [Execute\(string\)](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string) 'CurlDotNet\.Curl\.Execute\(string\)')
- [Curl\(string, int\)](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)')
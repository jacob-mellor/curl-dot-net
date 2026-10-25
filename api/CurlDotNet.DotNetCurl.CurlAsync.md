#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.CurlAsync Method

| Overloads | |
| :--- | :--- |
| [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)') | Runs a curl command asynchronously and returns the response; same as [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. |
| [CurlAsync\(string, CurlSettings\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)') | Runs a curl command asynchronously with timeouts, retries, headers or a progress callback from a [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object; same as [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')\. |
| [CurlAsync\(string, CancellationToken\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)') | Runs a curl command asynchronously and stops it when [cancellationToken](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; same as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\. |

<a name='CurlDotNet.DotNetCurl.CurlAsync(string)'></a>

## DotNetCurl\.CurlAsync\(string\) Method

Runs a curl command asynchronously and returns the response; same as
[ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, for example `curl -s https://api.example.com/users`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult'): status code, body, headers, binary data, written files and timings\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string).command 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The command could not be read or the transfer failed; see
            [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var response = await DotNetCurl.CurlAsync("curl https://api.example.com/data");
Console.WriteLine(response.Body);
```

### Remarks

Paste any curl command, with or without the leading `curl`. Multi-line commands with `\`,
            `^` or backtick continuations work. HTTP error statuses are returned, not thrown (add `-f` to throw).

Example: `var result = await DotNetCurl.CurlAsync("curl -X POST -H 'Content-Type: application/json' -d '{\"name\":\"John\"}' https://api.example.com/users");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/run-curl-command-in-csharp/README.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string) 'CurlDotNet\.DotNetCurl\.Curl\(string\)')
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings)'></a>

## DotNetCurl\.CurlAsync\(string, CurlSettings\) Method

Runs a curl command asynchronously with timeouts, retries, headers or a progress callback from a
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') object; same as [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command, CurlDotNet.Core.CurlSettings settings);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)')\.

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings).settings'></a>

`settings` [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

Settings applied on top of the command; null runs the command unchanged\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string,CurlDotNet.Core.CurlSettings).command 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, CurlDotNet\.Core\.CurlSettings\)\.command') is null, empty or whitespace\.

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The settings' cancellation token was cancelled\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed after all retries; see
            [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Remarks

Applied settings: `MaxTimeSeconds` (`-m`), `ConnectTimeoutSeconds`
            (`--connect-timeout`), `FollowRedirects` (`-L`), `Insecure` (`-k`), `Headers`
            (`-H`), `OnProgress`, `RetryCount`/`RetryDelayMs`, `Flavor` and
            `CancellationToken`; see the `Curl` method for details.

Example: `var result = await DotNetCurl.CurlAsync("curl https://api.example.com", new CurlSettings().WithTimeout(10).WithRetries(2));`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-timeout-csharp/README.md

### See Also
- [ExecuteAsync\(string, CurlSettings\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.ExecuteAsync\(string, CurlDotNet\.Core\.CurlSettings\)')
- [Blocking version](CurlDotNet.Curl.Execute.md#CurlDotNet.Curl.Execute(string,CurlDotNet.Core.CurlSettings) 'CurlDotNet\.Curl\.Execute\(string, CurlDotNet\.Core\.CurlSettings\)')
- [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken)'></a>

## DotNetCurl\.CurlAsync\(string, CancellationToken\) Method

Runs a curl command asynchronously and stops it when [cancellationToken](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).cancellationToken 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)\.cancellationToken') is cancelled; same
as [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> CurlAsync(string command, System.Threading.CancellationToken cancellationToken);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The curl command line as text, as for [CurlAsync\(string\)](CurlDotNet.DotNetCurl.CurlAsync.md#CurlDotNet.DotNetCurl.CurlAsync(string) 'CurlDotNet\.DotNetCurl\.CurlAsync\(string\)')\.

<a name='CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).cancellationToken'></a>

`cancellationToken` [System\.Threading\.CancellationToken](https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken 'System\.Threading\.CancellationToken')

Cancels the request when signalled; `CancellationToken.None` for no cancellation\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')\.

#### Exceptions

[System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException')  
The token was cancelled before the transfer finished\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[command](CurlDotNet.DotNetCurl.md#CurlDotNet.DotNetCurl.CurlAsync(string,System.Threading.CancellationToken).command 'CurlDotNet\.DotNetCurl\.CurlAsync\(string, System\.Threading\.CancellationToken\)\.command') is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The transfer failed; see [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\.

### Example

```csharp
var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var response = await DotNetCurl.CurlAsync("curl https://slow-api.com", cts.Token);
```

### Remarks

Cancelling throws [System\.OperationCanceledException](https://learn.microsoft.com/en-us/dotnet/api/system.operationcanceledexception 'System\.OperationCanceledException'); a `-m`/`--max-time` limit in the
            command throws [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException') (exit code 28) instead.

Example: `using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));` then
            `var response = await DotNetCurl.CurlAsync("curl https://slow-api.example.com", cts.Token);`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/12-cancellation-tokens.md

### See Also
- [Blocking version with a deadline](CurlDotNet.DotNetCurl.Curl.md#CurlDotNet.DotNetCurl.Curl(string,int) 'CurlDotNet\.DotNetCurl\.Curl\(string, int\)')
- [ExecuteAsync\(string, CancellationToken\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string,System.Threading.CancellationToken) 'CurlDotNet\.Curl\.ExecuteAsync\(string, System\.Threading\.CancellationToken\)')
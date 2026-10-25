#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Retry\(\) Method

Runs [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') again with [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') and returns the new result \- a
one\-line manual retry after a 429, 503 or flaky network\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> Retry();
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the new transfer\.

#### Exceptions

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
[Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') is null or empty \(the result was created by hand\),
            so there is nothing to run again\. Fix: call `Curl.ExecuteAsync(command)` yourself, or set [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/run\-curl\-command\-in\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The repeated transfer failed \(DNS, connection, timeout, TLS, `-f`\.\.\.\), exactly as
            [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') would throw; see the exception's message for the fix\.

### Remarks

No delay is added and nothing about the command changes; this result is not modified. For automatic
            retries with back-off use curl's `--retry 3` in the command or [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)').
            Settings passed to the original call (timeouts, headers, progress) are not repeated; use
            [RetryWith\(Action&lt;CurlSettings&gt;\)](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)') to supply them again.

Example: `if (result.StatusCode == 429) { await Task.Delay(5000); result = await result.Retry(); }`

### See Also
- [RetryWith\(Action&lt;CurlSettings&gt;\)](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)')
- [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')
- [WithRetries\(int, int\)](CurlDotNet.Core.CurlSettings.WithRetries(int,int).md 'CurlDotNet\.Core\.CurlSettings\.WithRetries\(int, int\)')
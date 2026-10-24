#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.RetryWith\(Action\<CurlSettings\>\) Method

Runs [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') again with [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') you configure \(longer timeout, extra header,
retries, progress\) and returns the new result\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> RetryWith(System.Action<CurlDotNet.Core.CurlSettings> configure);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Callback that fills in the settings for the new attempt, such as
            `s => s.WithTimeout(60)`; must not be null\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The result of the new transfer\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[configure](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md#CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).configure 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)\.configure') is null; pass `s => { }` to change nothing,
            or call [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)')\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
[Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command') is null or empty \(the result was created by hand\),
            so there is nothing to run again\. Fix: call `Curl.ExecuteAsync(command)` yourself, or set [Command](CurlDotNet.Core.CurlResult.Command.md 'CurlDotNet\.Core\.CurlResult\.Command')\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/run\-curl\-command\-in\-csharp/README\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
The repeated transfer failed \(DNS, connection, timeout, TLS, `-f`\.\.\.\), exactly as
            [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)') would throw; see the exception's message for the fix\.

### Remarks

[configure](CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).md#CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).configure 'CurlDotNet\.Core\.CurlResult\.RetryWith\(System\.Action\<CurlDotNet\.Core\.CurlSettings\>\)\.configure') receives a fresh [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings'); whatever it sets overrides the same
            option in the command (for example [MaxTimeSeconds](CurlDotNet.Core.CurlSettings.MaxTimeSeconds.md 'CurlDotNet\.Core\.CurlSettings\.MaxTimeSeconds') wins over `-m`).

Example: `result = await result.RetryWith(s => s.WithTimeout(60).WithHeader("Authorization", "Bearer " + newToken));`

### See Also
- [Retry\(\)](CurlDotNet.Core.CurlResult.Retry().md 'CurlDotNet\.Core\.CurlResult\.Retry\(\)')
- [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')
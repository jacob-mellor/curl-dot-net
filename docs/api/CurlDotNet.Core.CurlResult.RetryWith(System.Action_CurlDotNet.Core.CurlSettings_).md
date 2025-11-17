#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.RetryWith\(Action\<CurlSettings\>\) Method


<b>Retry with modifications to the original command.</b>

Retry with different settings:

```csharp
// Retry with longer timeout
var result = await result.RetryWith(settings =>
{
    settings.Timeout = TimeSpan.FromSeconds(60);
});

// Retry with authentication
var result = await result.RetryWith(settings =>
{
    settings.AddHeader("Authorization", "Bearer " + token);
});
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> RetryWith(System.Action<CurlDotNet.Core.CurlSettings> configure);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.RetryWith(System.Action_CurlDotNet.Core.CurlSettings_).configure'></a>

`configure` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')
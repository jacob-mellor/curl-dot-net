#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.Retry\(\) Method


<b>Retry the same curl command again.</b>

Simple retry for transient failures:

```csharp
// First attempt
var result = await Curl.Execute("curl https://flaky-api.example.com");

// Retry if it failed
if (!result.IsSuccess)
{
    result = await result.Retry();
}

// Retry with delay
if (result.StatusCode == 429)  // Too many requests
{
    await Task.Delay(5000);
    result = await result.Retry();
}
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> Retry();
```

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
New result from retrying the command
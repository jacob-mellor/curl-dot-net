#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ExecuteManyAsync\(string\[\]\) Method


<b>Execute multiple curl commands in parallel - great for performance!</b>

Runs multiple HTTP requests at the same time, which is much faster than running them one by one.
             Perfect for fetching data from multiple APIs or endpoints simultaneously.

<b>Example - Fetch Multiple APIs:</b>

```csharp
// These all run at the same time (parallel), not one after another
var results = await Curl.ExecuteMany(
    "curl https://api.github.com/users/microsoft",
    "curl https://api.github.com/users/dotnet",
    "curl https://api.github.com/users/azure"
);

// Process results - array order matches command order
Console.WriteLine($"Microsoft: {results[0].Body}");
Console.WriteLine($"DotNet: {results[1].Body}");
Console.WriteLine($"Azure: {results[2].Body}");
```

<b>Example - Aggregate Data:</b>

```csharp
// Fetch from multiple services simultaneously
var results = await Curl.ExecuteMany(
    "curl https://api.weather.com/temperature",
    "curl https://api.weather.com/humidity",
    "curl https://api.weather.com/forecast"
);

// Check if all succeeded
if (results.All(r => r.IsSuccess))
{
    var temp = results[0].ParseJson<Temperature>();
    var humidity = results[1].ParseJson<Humidity>();
    var forecast = results[2].ParseJson<Forecast>();

    DisplayWeatherDashboard(temp, humidity, forecast);
}
```

<b>Error Handling - Some May Fail:</b>

```csharp
var results = await Curl.ExecuteMany(commands);

for (int i = 0; i < results.Length; i++)
{
    if (results[i].IsSuccess)
    {
        Console.WriteLine($"✅ Command {i} succeeded");
    }
    else
    {
        Console.WriteLine($"❌ Command {i} failed: {results[i].StatusCode}");
    }
}
```

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> ExecuteManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Array of curl command strings to execute\. Can pass as:
- Multiple parameters: `ExecuteMany(cmd1, cmd2, cmd3)`
- Array: `ExecuteMany(commandArray)`
- List: `ExecuteMany(commandList.ToArray())`

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Array of [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') objects in the same order as the commands\.
Even if some fail, you still get results for all commands\.

### Remarks

<b>Performance Note:</b> If you have 10 commands that each take 1 second,
             running them in parallel takes ~1 second total instead of 10 seconds sequentially!

<b>Limit:</b> Be respectful of APIs - don't send hundreds of parallel requests.
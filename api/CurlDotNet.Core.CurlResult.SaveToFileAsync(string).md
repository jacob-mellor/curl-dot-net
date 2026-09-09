#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveToFileAsync\(string\) Method


<b>Save the response to a file asynchronously.</b>

Same as SaveToFile but doesn't block:

```csharp
await result.SaveToFileAsync("large-file.json");

// Or chain async operations
await result
    .SaveToFileAsync("backup.json")
    .ContinueWith(_ => Console.WriteLine("Saved!"));
```

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> SaveToFileAsync(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')
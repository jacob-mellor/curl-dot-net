#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.CurlManyAsync\(string\[\]\) Method

Execute multiple curl commands in parallel asynchronously\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> CurlManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

Array of curl commands

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
Task with array of results

### Example

```csharp
var results = await DotNetCurl.CurlManyAsync(new[] {
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts",
    "curl https://api.example.com/comments"
});
```
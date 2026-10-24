#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.CurlManyAsync\(string\[\]\) Method

Runs several curl commands in parallel and returns their results in the same order; same as
[ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> CurlManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The curl command lines to run, as separate arguments or an array\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
One [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') per command, in the same order\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[commands](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md#CurlDotNet.DotNetCurl.CurlManyAsync(string[]).commands 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)\.commands') is null\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
One of the commands is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
At least one command failed\. See
            https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var results = await DotNetCurl.CurlManyAsync(
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts",
    "curl https://api.example.com/comments");
```

### Remarks

All commands start at once with no concurrency limit; batch large lists to avoid HTTP 429 rate limits.
            If any command throws, that exception is thrown after all have finished and no results are returned.

Example: `var results = await DotNetCurl.CurlManyAsync("curl https://a.example.com", "curl https://b.example.com");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md

### See Also
- [Blocking version](CurlDotNet.DotNetCurl.CurlMany(string[]).md 'CurlDotNet\.DotNetCurl\.CurlMany\(string\[\]\)')
- [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')
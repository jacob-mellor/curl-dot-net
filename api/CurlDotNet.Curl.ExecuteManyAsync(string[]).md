#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[Curl](CurlDotNet.Curl.md 'CurlDotNet\.Curl')

## Curl\.ExecuteManyAsync\(string\[\]\) Method

Runs several curl commands at the same time and returns their results in the same order, so N requests take
about as long as the slowest one instead of the sum\.

```csharp
public static System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult[]> ExecuteManyAsync(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.Curl.ExecuteManyAsync(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The curl command lines to run, each as for [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)'); pass them
             as separate arguments \(`ExecuteManyAsync(cmd1, cmd2)`\) or as an array\. An empty array returns an empty
             array\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
One [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') per command, in the same order as [commands](CurlDotNet.Curl.ExecuteManyAsync(string[]).md#CurlDotNet.Curl.ExecuteManyAsync(string[]).commands 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)\.commands')\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[commands](CurlDotNet.Curl.ExecuteManyAsync(string[]).md#CurlDotNet.Curl.ExecuteManyAsync(string[]).commands 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)\.commands') is null\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
One of the commands is null, empty or whitespace \(the message gives its index\)\.
             See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/tutorials/13\-parallel\-requests\.md

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
At least one command failed; the exception of the first failed command \(in
             array order\) is thrown\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/troubleshooting/error\-reference\.md

### Example

```csharp
var results = await Curl.ExecuteManyAsync(
    "curl https://api.github.com/users/microsoft",
    "curl https://api.github.com/users/dotnet",
    "curl https://api.github.com/users/azure");

for (int i = 0; i < results.Length; i++)
    Console.WriteLine($"{i}: HTTP {results[i].StatusCode}");
```

### Remarks

All commands start immediately (there is no concurrency limit), share the connection pool, and run in
             parallel. The returned array has one [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') per command, in the order given.

<b>Failures:</b> an HTTP error status is not a failure (that result just has its status code). If any
             command throws (DNS failure, timeout, `-f` with a 404...), this method throws that exception after all
             commands have finished, and the results of the successful ones are not returned. To keep every outcome, run
             the tasks yourself: `var tasks = commands.Select(c => Curl.ExecuteAsync(c)).ToList(); try { await Task.WhenAll(tasks); } catch { }`
             and then inspect each task's `Status`, `Result` or `Exception`.

Be considerate to servers: hundreds of simultaneous requests to one API can trigger rate limits
             (HTTP 429); batch them with `Chunk` or a `SemaphoreSlim`.

Example: `var results = await Curl.ExecuteManyAsync("curl https://api.github.com/users/microsoft", "curl https://api.github.com/users/dotnet");`
             then `results[1].Body` is the dotnet user.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md

### See Also
- [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')
- [CurlManyAsync\(string\[\]\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)')
- [Blocking version](CurlDotNet.DotNetCurl.CurlMany(string[]).md 'CurlDotNet\.DotNetCurl\.CurlMany\(string\[\]\)')
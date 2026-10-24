#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[DotNetCurl](CurlDotNet.DotNetCurl.md 'CurlDotNet\.DotNetCurl')

## DotNetCurl\.CurlMany\(string\[\]\) Method

Runs several curl commands in parallel and blocks until all have finished; results come back in the same
order as the commands\.

```csharp
public static CurlDotNet.Core.CurlResult[] CurlMany(params string[] commands);
```
#### Parameters

<a name='CurlDotNet.DotNetCurl.CurlMany(string[]).commands'></a>

`commands` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The curl command lines to run, as separate arguments or an array; an empty array
            returns an empty array\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')  
One [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult') per command, in the same order\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[commands](CurlDotNet.DotNetCurl.CurlMany(string[]).md#CurlDotNet.DotNetCurl.CurlMany(string[]).commands 'CurlDotNet\.DotNetCurl\.CurlMany\(string\[\]\)\.commands') is null\.

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
One of the commands is null, empty or whitespace\.

[CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')  
At least one command failed; see [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')\.

### Example

```csharp
var results = DotNetCurl.CurlMany(
    "curl https://api.example.com/users",
    "curl https://api.example.com/posts");
foreach (var result in results)
    Console.WriteLine(result.Body);
```

### Remarks

Blocking: CurlDotNet awaits with `ConfigureAwait(false)` throughout, so this call does not deadlock on a UI
            or classic ASP.NET [System\.Threading\.SynchronizationContext](https://learn.microsoft.com/en-us/dotnet/api/system.threading.synchronizationcontext 'System\.Threading\.SynchronizationContext') by itself. It still occupies the calling
            thread until the transfer ends (a frozen window, fewer free threads under load), and a callback you supply - a
            progress or redirect handler, middleware, or your own `HttpMessageHandler` - that waits for that same context
            can still deadlock. Prefer the async method wherever the caller can await.

All commands start at once and share the connection pool. If any command throws (DNS failure, timeout,
            `-f` with an error status...), that exception is thrown after all have finished and no results are
            returned; HTTP error statuses without `-f` are not failures. Blocks the calling thread; use
            [CurlManyAsync\(string\[\]\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)') in async code.

Example: `var results = DotNetCurl.CurlMany("curl https://api.example.com/users", "curl https://api.example.com/posts");`
            then `results[0].Body` and `results[1].Body`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/tutorials/13-parallel-requests.md

### See Also
- [Async version \(preferred\)](CurlDotNet.DotNetCurl.CurlManyAsync(string[]).md 'CurlDotNet\.DotNetCurl\.CurlManyAsync\(string\[\]\)')
- [ExecuteManyAsync\(string\[\]\)](CurlDotNet.Curl.ExecuteManyAsync(string[]).md 'CurlDotNet\.Curl\.ExecuteManyAsync\(string\[\]\)')
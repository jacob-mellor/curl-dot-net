#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware')

## AuthenticationMiddleware\(Func\<CurlContext,Task\<string\>\>\) Constructor

Creates the middleware with the function that supplies a bearer token for each request\.

```csharp
public AuthenticationMiddleware(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<string>> tokenProvider);
```
#### Parameters

<a name='CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider'></a>

`tokenProvider` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Returns the token \(without the `Bearer ` prefix\) for the given request; return
            null or empty to send no header\. Must not be null\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[tokenProvider](CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md#CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider 'CurlDotNet\.Middleware\.AuthenticationMiddleware\.AuthenticationMiddleware\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)\.tokenProvider') is null\. Fix: pass a function that returns
            the token\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-bearer\-token\-csharp/README\.md

### Remarks

Example: `new AuthenticationMiddleware(ctx => Task.FromResult(Environment.GetEnvironmentVariable("API_TOKEN")))`

### See Also
- [UseAuthentication\(Func&lt;CurlContext,Task&lt;string&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseAuthentication\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseAuthentication\(Func\<CurlContext,Task\<string\>\>\) Method

Adds an [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware') that asks [tokenProvider](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseAuthentication\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)\.tokenProvider') for a token before
every request and sends it as `Authorization: Bearer <token>`\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseAuthentication(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<string>> tokenProvider);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider'></a>

`tokenProvider` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

Returns the token for the request \(null or empty sends no header\); must not be null\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[tokenProvider](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md#CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseAuthentication\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)\.tokenProvider') is null\. Fix: pass a function that
            returns the token\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-bearer\-token\-csharp/README\.md

### Remarks

Example: `builder.UseAuthentication(async ctx => await tokenCache.GetAccessTokenAsync());`.
            Bearer token tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware')

## AuthenticationMiddleware\(Func\<CurlContext,Task\<string\>\>\) Constructor

```csharp
public AuthenticationMiddleware(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<string>> tokenProvider);
```
#### Parameters

<a name='CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider'></a>

`tokenProvider` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')
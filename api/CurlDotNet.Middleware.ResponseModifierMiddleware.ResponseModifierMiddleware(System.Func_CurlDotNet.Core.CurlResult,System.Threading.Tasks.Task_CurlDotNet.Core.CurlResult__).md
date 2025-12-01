#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware')

## ResponseModifierMiddleware\(Func\<CurlResult,Task\<CurlResult\>\>\) Constructor

```csharp
public ResponseModifierMiddleware(System.Func<CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> modifier);
```
#### Parameters

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).modifier'></a>

`modifier` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')
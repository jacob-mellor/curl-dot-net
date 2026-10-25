#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[RequestModifierMiddleware](CurlDotNet.Middleware.RequestModifierMiddleware.md 'CurlDotNet\.Middleware\.RequestModifierMiddleware')

## RequestModifierMiddleware\(Action\<CurlContext\>\) Constructor

Creates the middleware with the action that changes each request's [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext') before it runs\.

```csharp
public RequestModifierMiddleware(System.Action<CurlDotNet.Middleware.CurlContext> modifier);
```
#### Parameters

<a name='CurlDotNet.Middleware.RequestModifierMiddleware.RequestModifierMiddleware(System.Action_CurlDotNet.Middleware.CurlContext_).modifier'></a>

`modifier` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Changes the context in place; must not be null\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[modifier](CurlDotNet.Middleware.RequestModifierMiddleware.RequestModifierMiddleware(System.Action_CurlDotNet.Middleware.CurlContext_).md#CurlDotNet.Middleware.RequestModifierMiddleware.RequestModifierMiddleware(System.Action_CurlDotNet.Middleware.CurlContext_).modifier 'CurlDotNet\.Middleware\.RequestModifierMiddleware\.RequestModifierMiddleware\(System\.Action\<CurlDotNet\.Middleware\.CurlContext\>\)\.modifier') is null\. Fix: pass an action such as
            `ctx => ctx.Command += " -s"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/guides/logging\-observability\.md

### Remarks

Example: `new RequestModifierMiddleware(ctx => ctx.Command += " --compressed")`

### See Also
- [ResponseModifierMiddleware\(Func&lt;CurlResult,Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware\.ResponseModifierMiddleware\(System\.Func\<CurlDotNet\.Core\.CurlResult,System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')
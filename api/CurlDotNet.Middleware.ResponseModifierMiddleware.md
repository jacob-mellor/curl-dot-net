#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](index.md#CurlDotNet.Middleware 'CurlDotNet\.Middleware')

## ResponseModifierMiddleware Class

Middleware for modifying responses\.

```csharp
public class ResponseModifierMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; ResponseModifierMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')
### Constructors

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## ResponseModifierMiddleware\(Func\<CurlResult,Task\<CurlResult\>\>\) Constructor

Initializes a new instance of the [ResponseModifierMiddleware](CurlDotNet.Middleware.ResponseModifierMiddleware.md 'CurlDotNet\.Middleware\.ResponseModifierMiddleware') class\.

```csharp
public ResponseModifierMiddleware(System.Func<CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> modifier);
```
#### Parameters

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ResponseModifierMiddleware(System.Func_CurlDotNet.Core.CurlResult,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).modifier'></a>

`modifier` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

A function that modifies the response after execution\.
### Methods

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## ResponseModifierMiddleware\.ExecuteAsync\(CurlContext, Func\<Task\<CurlResult\>\>\) Method

Executes the middleware, modifying the response after the next middleware completes\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Middleware.CurlContext context, System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> next);
```
#### Parameters

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).context'></a>

`context` [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

The curl execution context\.

<a name='CurlDotNet.Middleware.ResponseModifierMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).next'></a>

`next` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')

The next middleware in the pipeline\.

Implements [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.ICurlMiddleware.md#CurlDotNet.Middleware.ICurlMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__) 'CurlDotNet\.Middleware\.ICurlMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
The modified curl result\.
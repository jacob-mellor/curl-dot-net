#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](index.md#CurlDotNet.Middleware 'CurlDotNet\.Middleware')

## CurlMiddlewarePipeline Class

Manages the middleware pipeline for curl operations\.

```csharp
public class CurlMiddlewarePipeline
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlMiddlewarePipeline
### Constructors

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipeline\(Func\<CurlContext,Task\<CurlResult\>\>\) Constructor

Initialize a new middleware pipeline\.

```csharp
public CurlMiddlewarePipeline(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> finalHandler);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.CurlMiddlewarePipeline(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).finalHandler'></a>

`finalHandler` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

The final handler that executes the curl command
### Properties

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Count'></a>

## CurlMiddlewarePipeline\.Count Property

Get the count of middleware in the pipeline\.

```csharp
public int Count { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')
### Methods

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Clear()'></a>

## CurlMiddlewarePipeline\.Clear\(\) Method

Clear all middleware from the pipeline\.

```csharp
public void Clear();
```

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.CreateBuilder()'></a>

## CurlMiddlewarePipeline\.CreateBuilder\(\) Method

Create a new pipeline builder\.

```csharp
public static CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder CreateBuilder();
```

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext)'></a>

## CurlMiddlewarePipeline\.ExecuteAsync\(CurlContext\) Method

Execute the pipeline\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> ExecuteAsync(CurlDotNet.Middleware.CurlContext context);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).context'></a>

`context` [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware)'></a>

## CurlMiddlewarePipeline\.Use\(ICurlMiddleware\) Method

Add middleware to the pipeline \(fluent\)\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Use(CurlDotNet.Middleware.ICurlMiddleware middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware'></a>

`middleware` [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipeline\.Use\(Func\<CurlContext,Func\<Task\<CurlResult\>\>,Task\<CurlResult\>\>\) Method

Add middleware using a delegate \(fluent\)\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Use(System.Func<CurlDotNet.Middleware.CurlContext,System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>>,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipeline.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware'></a>

`middleware` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](index.md#CurlDotNet.Middleware 'CurlDotNet\.Middleware')

## CurlMiddlewarePipelineBuilder Class

Fluent builder for creating middleware pipelines\.

```csharp
public class CurlMiddlewarePipelineBuilder
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlMiddlewarePipelineBuilder
### Methods

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Build()'></a>

## CurlMiddlewarePipelineBuilder\.Build\(\) Method

Build the pipeline\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipeline Build();
```

#### Returns
[CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware)'></a>

## CurlMiddlewarePipelineBuilder\.Use\(ICurlMiddleware\) Method

Add middleware to the pipeline\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder Use(CurlDotNet.Middleware.ICurlMiddleware middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(CurlDotNet.Middleware.ICurlMiddleware).middleware'></a>

`middleware` [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipelineBuilder\.Use\(Func\<CurlContext,Func\<Task\<CurlResult\>\>,Task\<CurlResult\>\>\) Method

Add middleware using a delegate\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder Use(System.Func<CurlDotNet.Middleware.CurlContext,System.Func<System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>>,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> middleware);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.Use(System.Func_CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).middleware'></a>

`middleware` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-1 'System\.Func\`1')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-3 'System\.Func\`3')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__)'></a>

## CurlMiddlewarePipelineBuilder\.UseAuthentication\(Func\<CurlContext,Task\<string\>\>\) Method

Add authentication middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseAuthentication(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<string>> tokenProvider);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).tokenProvider'></a>

`tokenProvider` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_)'></a>

## CurlMiddlewarePipelineBuilder\.UseCaching\(Nullable\<TimeSpan\>\) Method

Add caching middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseCaching(System.Nullable<System.TimeSpan> ttl=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseCaching(System.Nullable_System.TimeSpan_).ttl'></a>

`ttl` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_)'></a>

## CurlMiddlewarePipelineBuilder\.UseLogging\(Action\<string\>\) Method

Add logging middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseLogging(System.Action<string> logger=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_).logger'></a>

`logger` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int)'></a>

## CurlMiddlewarePipelineBuilder\.UseRateLimit\(int\) Method

Add rate limiting middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseRateLimit(int requestsPerSecond);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRateLimit(int).requestsPerSecond'></a>

`requestsPerSecond` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_)'></a>

## CurlMiddlewarePipelineBuilder\.UseRetry\(int, Nullable\<TimeSpan\>\) Method

Add retry middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseRetry(int maxRetries=3, System.Nullable<System.TimeSpan> delay=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).maxRetries'></a>

`maxRetries` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseRetry(int,System.Nullable_System.TimeSpan_).delay'></a>

`delay` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseTiming()'></a>

## CurlMiddlewarePipelineBuilder\.UseTiming\(\) Method

Add timing middleware\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseTiming();
```

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__)'></a>

## CurlMiddlewarePipelineBuilder\.WithHandler\(Func\<CurlContext,Task\<CurlResult\>\>\) Method

Set the final handler\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder WithHandler(System.Func<CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult>> handler);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.WithHandler(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).handler'></a>

`handler` [System\.Func&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')[,](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.func-2 'System\.Func\`2')

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')
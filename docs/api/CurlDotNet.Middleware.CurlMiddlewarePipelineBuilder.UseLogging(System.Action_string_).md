#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')

## CurlMiddlewarePipelineBuilder\.UseLogging\(Action\<string\>\) Method

Adds a [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware') that writes one line before each request \(the command\) and one after
\(status and milliseconds, or the error message\)\.

```csharp
public CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder UseLogging(System.Action<string> logger=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_).logger'></a>

`logger` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Receives each log line; null \(the default\) writes to [System\.Console\.WriteLine\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/system.console.writeline#system-console-writeline(system-string) 'System\.Console\.WriteLine\(System\.String\)')\.

#### Returns
[CurlMiddlewarePipelineBuilder](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder')  
This builder\.

### Remarks

Lines look like `[CURL] Executing: curl https://...`, `[CURL] Success: Status=200, Time=123.4ms`
            and `[CURL] Failed: <message>, Time=...ms`. The command text can contain credentials; prefer your own
            middleware that logs `context.Options.Url` in production.

Example: `builder.UseLogging(msg => logger.LogInformation(msg));`

### See Also
- [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')
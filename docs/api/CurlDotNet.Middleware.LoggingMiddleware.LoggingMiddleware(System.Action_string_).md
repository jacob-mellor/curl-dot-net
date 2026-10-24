#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')

## LoggingMiddleware\(Action\<string\>\) Constructor

Creates a logging middleware that sends every line to [logger](CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).md#CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).logger 'CurlDotNet\.Middleware\.LoggingMiddleware\.LoggingMiddleware\(System\.Action\<string\>\)\.logger'), or to
[System\.Console\.WriteLine\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/system.console.writeline#system-console-writeline(system-string) 'System\.Console\.WriteLine\(System\.String\)') when no logger is given\.

```csharp
public LoggingMiddleware(System.Action<string> logger=null);
```
#### Parameters

<a name='CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).logger'></a>

`logger` [System\.Action&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.action-1 'System\.Action\`1')

Receives each complete line, such as `[CURL] Success: Status=200, Time=87.2ms`;
            null \(the default\) uses the console\.

### Remarks

Example: `new LoggingMiddleware(line => Debug.WriteLine(line))` logs to the debugger output.

### See Also
- [UseLogging\(Action&lt;string&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseLogging\(System\.Action\<string\>\)')
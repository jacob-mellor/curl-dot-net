#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## LoggingMiddleware Class

Writes one log line when a request starts \(its command\) and one when it ends \(HTTP status and elapsed
milliseconds, or the error message\), to the console or your own logger\.

```csharp
public class LoggingMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; LoggingMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

Lines written: `[CURL] Executing: {Command}` before the request, then either
            `[CURL] Success: Status={StatusCode}, Time={ms}ms` or `[CURL] Failed: {exception message}, Time={ms}ms`.
            The time covers everything registered after this middleware (retries included). Failures are logged and
            rethrown unchanged.

Credentials in the command are masked before it is logged: the values of `-u`/`-U`,
            `--oauth2-bearer` and `--pass`, the password of `--cert FILE:PASSWORD`, `Authorization`,
            `Proxy-Authorization`, `Cookie` and API-key header values (including the bearer token
            [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware') adds) and URL passwords all appear as `***`. Other values (for example
            a password inside `-d` form data) are logged as written. The instance holds no per-request state and is
            thread-safe as long as your logger is.

Example: `CurlMiddlewarePipeline.CreateBuilder().Use(new LoggingMiddleware(msg => logger.LogInformation(msg))).WithHandler(ctx => Curl.ExecuteAsync(ctx.Command)).Build();`

Structured logging (Serilog, Microsoft.Extensions.Logging) guide:
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/guides/logging-observability.md

### See Also
- [UseLogging\(Action&lt;string&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseLogging(System.Action_string_).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseLogging\(System\.Action\<string\>\)')
- [TimingMiddleware](CurlDotNet.Middleware.TimingMiddleware.md 'CurlDotNet\.Middleware\.TimingMiddleware')

| Constructors | |
| :--- | :--- |
| [LoggingMiddleware\(Action&lt;string&gt;\)](CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).md 'CurlDotNet\.Middleware\.LoggingMiddleware\.LoggingMiddleware\(System\.Action\<string\>\)') | Creates a logging middleware that sends every line to [logger](CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).md#CurlDotNet.Middleware.LoggingMiddleware.LoggingMiddleware(System.Action_string_).logger 'CurlDotNet\.Middleware\.LoggingMiddleware\.LoggingMiddleware\(System\.Action\<string\>\)\.logger'), or to [System\.Console\.WriteLine\(System\.String\)](https://learn.microsoft.com/en-us/dotnet/api/system.console.writeline#system-console-writeline(system-string) 'System\.Console\.WriteLine\(System\.String\)') when no logger is given\. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.LoggingMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.LoggingMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Logs the command, runs the rest of the pipeline, then logs the status and elapsed time \(or the failure, which is rethrown unchanged\)\. |

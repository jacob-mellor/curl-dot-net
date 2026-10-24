#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## AuthenticationMiddleware Class

Adds `Authorization: Bearer <token>` to every request, asking your token provider for the token each
time \(so a provider that refreshes expired tokens keeps requests authorised\)\.

```csharp
public class AuthenticationMiddleware : CurlDotNet.Middleware.ICurlMiddleware
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; AuthenticationMiddleware

Implements [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')

### Remarks

Before each request the provider is awaited with the [CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext'). A non-empty token is written
            to `context.Options.Headers["Authorization"]` (creating [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') when it is null)
            and, when the command text does not already mention `Authorization`, appended to
            [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') as `-H 'Authorization: Bearer <token>'`, so it works with handlers that
            send either the options or the command. A null or empty token sends the request without the header.

Exceptions thrown by your provider propagate unchanged and the request is not sent. The token ends up in the
            command text so that handlers which run [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') send it; [LoggingMiddleware](CurlDotNet.Middleware.LoggingMiddleware.md 'CurlDotNet\.Middleware\.LoggingMiddleware')
            and the exception log helpers (`ToDetailedString`, `ToJson`, `GetDiagnosticInfo`) print it as
            `Authorization: ***`, wherever the logging middleware sits in the pipeline. If your own code logs
            [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command'), mask it the same way. The middleware itself is stateless and thread-safe if your
            provider is.

Example: `builder.UseAuthentication(async ctx => (await credential.GetTokenAsync(scopes, ctx.CancellationToken)).Token);`

Bearer token tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [UseAuthentication\(Func&lt;CurlContext,Task&lt;string&gt;&gt;\)](CurlDotNet.Middleware.CurlMiddlewarePipelineBuilder.UseAuthentication(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipelineBuilder\.UseAuthentication\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)')
- [WithBearerToken\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithBearerToken(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithBearerToken\(string\)')

| Constructors | |
| :--- | :--- |
| [AuthenticationMiddleware\(Func&lt;CurlContext,Task&lt;string&gt;&gt;\)](CurlDotNet.Middleware.AuthenticationMiddleware.AuthenticationMiddleware(System.Func_CurlDotNet.Middleware.CurlContext,System.Threading.Tasks.Task_string__).md 'CurlDotNet\.Middleware\.AuthenticationMiddleware\.AuthenticationMiddleware\(System\.Func\<CurlDotNet\.Middleware\.CurlContext,System\.Threading\.Tasks\.Task\<string\>\>\)') | Creates the middleware with the function that supplies a bearer token for each request\. |

| Methods | |
| :--- | :--- |
| [ExecuteAsync\(CurlContext, Func&lt;Task&lt;CurlResult&gt;&gt;\)](CurlDotNet.Middleware.AuthenticationMiddleware.ExecuteAsync(CurlDotNet.Middleware.CurlContext,System.Func_System.Threading.Tasks.Task_CurlDotNet.Core.CurlResult__).md 'CurlDotNet\.Middleware\.AuthenticationMiddleware\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext, System\.Func\<System\.Threading\.Tasks\.Task\<CurlDotNet\.Core\.CurlResult\>\>\)') | Gets a token from the provider, adds it as a bearer `Authorization` header to the options and the command, then runs the rest of the pipeline\. |

#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware').[CurlContext](CurlDotNet.Middleware.CurlContext.md 'CurlDotNet\.Middleware\.CurlContext')

## CurlContext\.Options Property

The parsed request options \(URL, method, headers, timeouts\.\.\.\) that middleware can read or change,
and that a handler such as `ctx => Curl.ExecuteAsync(ctx.Options)` sends\.

```csharp
public CurlDotNet.Core.CurlOptions Options { get; set; }
```

#### Property Value
[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')  
A [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions'), or null when the caller only set [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command')\. Default null\.

### Remarks

Changes here only reach the wire when the final handler executes the options rather than the
            command text. [CachingMiddleware](CurlDotNet.Middleware.CachingMiddleware.md 'CurlDotNet\.Middleware\.CachingMiddleware') reads [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method') (it caches only GET) and
            [AuthenticationMiddleware](CurlDotNet.Middleware.AuthenticationMiddleware.md 'CurlDotNet\.Middleware\.AuthenticationMiddleware') sets the `Authorization` header here (creating the options when
            they are null).

Example: `context.Options.MaxTime = 10;` gives the request a 10-second `--max-time`.

### See Also
- [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command')
- [CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')
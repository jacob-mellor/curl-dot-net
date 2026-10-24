#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Middleware](CurlDotNet.Middleware.md 'CurlDotNet\.Middleware')

## CurlContext Class

Everything middleware needs to know about one request: the curl command, the parsed options, a
property bag shared by all middleware, the cancellation token and the start time\.

```csharp
public class CurlContext
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; CurlContext

### Remarks

Create one per request and pass it to [ExecuteAsync\(CurlContext\)](CurlDotNet.Middleware.CurlMiddlewarePipeline.ExecuteAsync(CurlDotNet.Middleware.CurlContext).md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline\.ExecuteAsync\(CurlDotNet\.Middleware\.CurlContext\)').
            Middleware may read and change it on the way in; the final handler decides whether it sends
            [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') or [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options'). A context is not thread-safe and should not be shared
            between concurrent requests.

Example: `var ctx = new CurlContext { Command = "curl https://api.example.com/users", Options = new CurlOptions { Url = "https://api.example.com/users" }, CancellationToken = ct };`
            then `var result = await pipeline.ExecuteAsync(ctx);`.

### See Also
- [ICurlMiddleware](CurlDotNet.Middleware.ICurlMiddleware.md 'CurlDotNet\.Middleware\.ICurlMiddleware')
- [CurlMiddlewarePipeline](CurlDotNet.Middleware.CurlMiddlewarePipeline.md 'CurlDotNet\.Middleware\.CurlMiddlewarePipeline')

| Properties | |
| :--- | :--- |
| [CancellationToken](CurlDotNet.Middleware.CurlContext.CancellationToken.md 'CurlDotNet\.Middleware\.CurlContext\.CancellationToken') | The token that cancels this request; built\-in middleware pass it to their waits \(retry back\-off, rate limiting\) and your handler should pass it to `Curl.ExecuteAsync`\. |
| [Command](CurlDotNet.Middleware.CurlContext.Command.md 'CurlDotNet\.Middleware\.CurlContext\.Command') | The curl command text for this request, such as `curl -H 'Accept: application/json' https://api.example.com/users`, usually what the final handler passes to [ExecuteAsync\(string\)](CurlDotNet.Curl.ExecuteAsync.md#CurlDotNet.Curl.ExecuteAsync(string) 'CurlDotNet\.Curl\.ExecuteAsync\(string\)')\. |
| [Options](CurlDotNet.Middleware.CurlContext.Options.md 'CurlDotNet\.Middleware\.CurlContext\.Options') | The parsed request options \(URL, method, headers, timeouts\.\.\.\) that middleware can read or change, and that a handler such as `ctx => Curl.ExecuteAsync(ctx.Options)` sends\. |
| [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') | A per\-request bag for passing values between middleware and the final handler, such as a correlation id, a stopwatch or the user the request is made for\. |
| [StartTime](CurlDotNet.Middleware.CurlContext.StartTime.md 'CurlDotNet\.Middleware\.CurlContext\.StartTime') | When this context was created \(UTC\), useful for measuring how long the whole pipeline took, including retries and rate\-limit waits\. |

| Methods | |
| :--- | :--- |
| [GetProperty&lt;T&gt;\(string\)](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)') | Reads a value from [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') as [T](CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).md#CurlDotNet.Middleware.CurlContext.GetProperty_T_(string).T 'CurlDotNet\.Middleware\.CurlContext\.GetProperty\<T\>\(string\)\.T'), returning `default(T)` when the key is missing or the value is of another type \(it never throws for a bad cast\)\. |
| [WithProperty\(string, object\)](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)') | Stores [value](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md#CurlDotNet.Middleware.CurlContext.WithProperty(string,object).value 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)\.value') in [Properties](CurlDotNet.Middleware.CurlContext.Properties.md 'CurlDotNet\.Middleware\.CurlContext\.Properties') under [key](CurlDotNet.Middleware.CurlContext.WithProperty(string,object).md#CurlDotNet.Middleware.CurlContext.WithProperty(string,object).key 'CurlDotNet\.Middleware\.CurlContext\.WithProperty\(string, object\)\.key') \(replacing any earlier value\) and returns this context, so calls can be chained\. |

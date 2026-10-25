#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRateLimitException Class

The server rate limited the request \(HTTP 429 Too Many Requests\); [RetryAfter](CurlDotNet.Exceptions.CurlRateLimitException.RetryAfter.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RetryAfter') says how long to
wait and [RemainingLimit](CurlDotNet.Exceptions.CurlRateLimitException.RemainingLimit.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RemainingLimit') how many calls are left, when the server said so\.

```csharp
public class CurlRateLimitException : CurlDotNet.Exceptions.CurlHttpException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') &#129106; CurlRateLimitException

### Remarks

A [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') with [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') 429 and no curl exit code.
            CurlDotNet does not throw it on its own (a 429 comes back as a `CurlResult`, or as
            [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') with `-f`); create it in your API client from the response's
            `Retry-After` and `X-RateLimit-Remaining` headers.

Example: `catch (CurlRateLimitException ex) { await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(60)); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [IsRateLimited](CurlDotNet.Exceptions.CurlHttpException.IsRateLimited.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRateLimited')
- [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)')

| Constructors | |
| :--- | :--- |
| [CurlRateLimitException\(string, Nullable&lt;TimeSpan&gt;, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.CurlRateLimitException\(string, System\.Nullable\<System\.TimeSpan\>, System\.Nullable\<int\>, string\)') | Creates a 429 rate\-limit error carrying the server's wait time and remaining quota, which the advice in the message uses\. |

| Properties | |
| :--- | :--- |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlRateLimitException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [RemainingLimit](CurlDotNet.Exceptions.CurlRateLimitException.RemainingLimit.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RemainingLimit') | How many requests the server still allows in the current window \(for example from `X-RateLimit-Remaining`\), or null when unknown\. |
| [RetryAfter](CurlDotNet.Exceptions.CurlRateLimitException.RetryAfter.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RetryAfter') | How long the server asked you to wait before the next request \(its `Retry-After` header\), or null when it did not say\. |

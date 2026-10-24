#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRetryException Class

Every attempt of a retried request failed; [RetryCount](CurlDotNet.Exceptions.CurlRetryException.RetryCount.md 'CurlDotNet\.Exceptions\.CurlRetryException\.RetryCount') says how many were made and
[LastAttemptException](CurlDotNet.Exceptions.CurlRetryException.LastAttemptException.md 'CurlDotNet\.Exceptions\.CurlRetryException\.LastAttemptException') holds the error of the final one\.

```csharp
public class CurlRetryException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlRetryException

### Remarks

Thrown by the retry middleware (`RetryMiddleware` in `CurlDotNet.Middleware`) after its last attempt;
            curl's own `--retry N` instead rethrows the last error with its exit code. The real cause is the inner
            exception: fix that (DNS, timeout, 5xx), then decide whether more attempts or a longer delay make sense.

Example: `catch (CurlRetryException ex) { Console.WriteLine($"Gave up after {ex.RetryCount} tries: {ex.LastAttemptException?.Message}"); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [IsRetryable\(\)](CurlDotNet.Exceptions.CurlException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlException\.IsRetryable\(\)')

| Constructors | |
| :--- | :--- |
| [CurlRetryException\(string, string, int, Exception\)](CurlDotNet.Exceptions.CurlRetryException.CurlRetryException(string,string,int,System.Exception).md 'CurlDotNet\.Exceptions\.CurlRetryException\.CurlRetryException\(string, string, int, System\.Exception\)') | Creates the "retries exhausted" error with the number of attempts and the last attempt's exception, which becomes the inner exception and is summarised in the first line\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlRetryException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlRetryException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlRetryException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlRetryException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [LastAttemptException](CurlDotNet.Exceptions.CurlRetryException.LastAttemptException.md 'CurlDotNet\.Exceptions\.CurlRetryException\.LastAttemptException') | The exception thrown by the final attempt \(also [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\), which tells you why the request keeps failing\. |
| [RetryCount](CurlDotNet.Exceptions.CurlRetryException.RetryCount.md 'CurlDotNet\.Exceptions\.CurlRetryException\.RetryCount') | How many retry attempts were made before giving up \(not counting the first try, when the middleware is configured that way\), useful for logs and alerts\. |

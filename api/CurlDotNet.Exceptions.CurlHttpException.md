#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlHttpException Class

An HTTP error response turned into an exception: holds the status code, reason phrase, response body and
headers, plus status\-specific advice \(401: check the token, 429: back off, 5xx: retry later\)\.

```csharp
public class CurlHttpException : CurlDotNet.Exceptions.CurlException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; CurlHttpException

Derived  
&#8627; [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')  
&#8627; [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

### Remarks

CurlDotNet returns 4xx/5xx responses as a normal `CurlResult`. It throws the derived
            [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException') (exit code 22) when the command uses `-f`/`--fail` or
            `--fail-with-body`; [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException') is the 429 variant. Construct it yourself to turn a
            result into an exception: `if (result.StatusCode >= 400) throw new CurlHttpException($"GET {url} failed", result.StatusCode, null, result.Body);`

Use [IsClientError](CurlDotNet.Exceptions.CurlHttpException.IsClientError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsClientError')/[IsServerError](CurlDotNet.Exceptions.CurlHttpException.IsServerError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsServerError') and the `Is*` shortcuts to branch, and
            [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)') to honour the server's back-off. Example:
            `catch (CurlHttpException ex) when (ex.IsUnauthorized) { await RefreshTokenAsync(); }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md
            tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-error-handling-csharp/README.md

### See Also
- [CurlHttpReturnedErrorException](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.md 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException')
- [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

| Constructors | |
| :--- | :--- |
| [CurlHttpException\(string, int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpException.#ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string) 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string\)') | Creates an HTTP error from a status code and the response, for code that turns a `CurlResult` with a 4xx/5xx status into an exception\. |
| [CurlHttpException\(string, int, string, string, string, int\)](CurlDotNet.Exceptions.CurlHttpException.#ctor.md#CurlDotNet.Exceptions.CurlHttpException.CurlHttpException(string,int,string,string,string,int) 'CurlDotNet\.Exceptions\.CurlHttpException\.CurlHttpException\(string, int, string, string, string, int\)') | Creates an HTTP error that also carries a curl exit code, for HTTP failures that end a command \(such as `-f`/`--fail`, exit code 22\) rather than being returned as a result\. |

| Properties | |
| :--- | :--- |
| [DefaultDocumentationUrl](CurlDotNet.Exceptions.CurlHttpException.DefaultDocumentationUrl.md 'CurlDotNet\.Exceptions\.CurlHttpException\.DefaultDocumentationUrl') | The documentation page for this exception type, used when the message does not end with its own `See https://...` link\. |
| [DefaultSuggestion](CurlDotNet.Exceptions.CurlHttpException.DefaultSuggestion.md 'CurlDotNet\.Exceptions\.CurlHttpException\.DefaultSuggestion') | How to fix this kind of error in one sentence, used as [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') \(and as the second line of [Message](CurlDotNet.Exceptions.CurlException.Message.md 'CurlDotNet\.Exceptions\.CurlException\.Message')\) when the message itself does not say how to fix it\. |
| [ExtraSuggestions](CurlDotNet.Exceptions.CurlHttpException.ExtraSuggestions.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ExtraSuggestions') | Further fixes appended to [Suggestions](CurlDotNet.Exceptions.CurlException.Suggestions.md 'CurlDotNet\.Exceptions\.CurlException\.Suggestions') after the ones in the message, for errors where several remedies apply \(for example the status\-specific advice of [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')\)\. |
| [IsClientError](CurlDotNet.Exceptions.CurlHttpException.IsClientError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsClientError') | True for 4xx statuses \(400 to 499\): the request itself is wrong \(URL, credentials, body\) and repeating it unchanged will fail again, except for 408 and 429\. |
| [IsForbidden](CurlDotNet.Exceptions.CurlHttpException.IsForbidden.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsForbidden') | True for 403 Forbidden: the server knows who you are but this account or token may not do this \(missing scope, role or IP allow\-list\)\. |
| [IsNotFound](CurlDotNet.Exceptions.CurlHttpException.IsNotFound.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsNotFound') | True for 404 Not Found: the URL path \(or the resource id in it\) does not exist on this server\. |
| [IsRateLimited](CurlDotNet.Exceptions.CurlHttpException.IsRateLimited.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRateLimited') | True for 429 Too Many Requests: you are being rate limited; wait \(see [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)')\) before sending more requests\. |
| [IsServerError](CurlDotNet.Exceptions.CurlHttpException.IsServerError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsServerError') | True for 5xx statuses \(500 to 599\): the server failed; the same request may work later, so it is a candidate for a retry with backoff\. |
| [IsUnauthorized](CurlDotNet.Exceptions.CurlHttpException.IsUnauthorized.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsUnauthorized') | True for 401 Unauthorized: the credentials are missing, wrong or expired \(refresh the token or check `-u`/`-H 'Authorization: ...'`\)\. |
| [ResponseBody](CurlDotNet.Exceptions.CurlHttpException.ResponseBody.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseBody') | The body of the error response, which usually holds the server's own explanation \(a JSON error object or an HTML error page\)\. |
| [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders') | The response headers of the error response \(for example `Retry-After`, `WWW-Authenticate`, `X-Request-Id`\), keyed by header name\. |
| [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') | The HTTP status code the server answered with, such as 404 or 503, so you can branch on it without parsing the message\. |
| [StatusText](CurlDotNet.Exceptions.CurlHttpException.StatusText.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusText') | The reason phrase from the response's status line, such as `Not Found` or `Service Unavailable`, when the server sent one\. |

| Methods | |
| :--- | :--- |
| [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)') | Reads the server's `Retry-After` header \(in seconds\) so you know how long to wait before the next request after a 429 or 503\. |
| [IsRetryable\(\)](CurlDotNet.Exceptions.CurlHttpException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRetryable\(\)') | Tells whether the same request may succeed later: true for 429 \(rate limited\) and every 5xx server error, false for other 4xx errors, which need the request to change first\. |
| [IsStatus\(int\)](CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsStatus\(int\)') | Checks for one exact status code, for `when` filters such as `catch (CurlHttpException ex) when (ex.IsStatus(409))`\. |
| [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlHttpException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlHttpException\.ToDetailedString\(\)') | Returns the base detailed report plus the HTTP status line, client/server classification, response body and response headers, for logs\. |
| [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Exceptions\.CurlHttpException\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)') | Attaches the response headers to [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders') and returns the same exception, for code that builds the exception from a result\. |

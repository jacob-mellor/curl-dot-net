#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlHttpReturnedErrorException Class

The server answered with an HTTP error status \(400 or above\) and the command used `-f`/`--fail` or
`--fail-with-body` \(exit code 22, CURLE\_HTTP\_RETURNED\_ERROR\)\.

```csharp
public class CurlHttpReturnedErrorException : CurlDotNet.Exceptions.CurlHttpException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') &#129106; CurlHttpReturnedErrorException

### Remarks

Without `-f` CurlDotNet returns the 4xx/5xx response as a normal `CurlResult`; with it, curl's rule
            "fail on HTTP errors" applies and this exception is thrown after the headers arrive. With `-f` the error body is
            discarded; with `--fail-with-body` it is written to the output and kept in
            [ResponseBody](CurlDotNet.Exceptions.CurlHttpException.ResponseBody.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseBody'). The message's advice depends on the status (401: credentials, 404:
            URL, 429: back off, 5xx: retry).

Example: `try { await Curl.ExecuteAsync($"curl -f {url}"); } catch (CurlHttpReturnedErrorException ex) when (ex.StatusCode == 404) { return null; }`

Docs: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/http-errors.md

### See Also
- [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')
- [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

| Constructors | |
| :--- | :--- |
| [CurlHttpReturnedErrorException\(int, string, string, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string\)') | Creates the error \(exit code 22\) from a status code, reason phrase and body, with status\-specific advice in the message\. |
| [CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)](CurlDotNet.Exceptions.CurlHttpReturnedErrorException.#ctor.md#CurlDotNet.Exceptions.CurlHttpReturnedErrorException.CurlHttpReturnedErrorException(int,string,string,string,bool,string) 'CurlDotNet\.Exceptions\.CurlHttpReturnedErrorException\.CurlHttpReturnedErrorException\(int, string, string, string, bool, string\)') | Creates the error for `-f`/`--fail` or `--fail-with-body` with curl's wording `The requested URL returned error: 404` and the URL \(without credentials or query string\)\. |

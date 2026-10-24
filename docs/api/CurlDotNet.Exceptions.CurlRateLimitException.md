#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions')

## CurlRateLimitException Class

Thrown when rate limiting is encountered

```csharp
public class CurlRateLimitException : CurlDotNet.Exceptions.CurlHttpException
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') &#129106; [System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception') &#129106; [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') &#129106; [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException') &#129106; CurlRateLimitException

| Constructors | |
| :--- | :--- |
| [CurlRateLimitException\(string, Nullable&lt;TimeSpan&gt;, Nullable&lt;int&gt;, string\)](CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.CurlRateLimitException\(string, System\.Nullable\<System\.TimeSpan\>, System\.Nullable\<int\>, string\)') | Initializes a new instance of the [CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException') class |

| Properties | |
| :--- | :--- |
| [RemainingLimit](CurlDotNet.Exceptions.CurlRateLimitException.RemainingLimit.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RemainingLimit') | Gets the number of remaining requests allowed, if provided |
| [RetryAfter](CurlDotNet.Exceptions.CurlRateLimitException.RetryAfter.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RetryAfter') | Gets the time to wait before retrying, if provided by the server |

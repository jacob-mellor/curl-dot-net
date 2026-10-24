#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

## CurlRateLimitException\(string, Nullable\<TimeSpan\>, Nullable\<int\>, string\) Constructor

Creates a 429 rate\-limit error carrying the server's wait time and remaining quota, which the advice in the
message uses\.

```csharp
public CurlRateLimitException(string message, System.Nullable<System.TimeSpan> retryAfter=null, System.Nullable<int> remainingLimit=null, string command=null);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).message'></a>

`message` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

What was rate limited, naming the API; optionally followed by fix lines\.

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).retryAfter'></a>

`retryAfter` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The time to wait before retrying \(the Retry\-After header\), or null\.

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).remainingLimit'></a>

`remainingLimit` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The number of requests remaining in the window, or null\.

<a name='CurlDotNet.Exceptions.CurlRateLimitException.CurlRateLimitException(string,System.Nullable_System.TimeSpan_,System.Nullable_int_,string).command'></a>

`command` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The original curl command that was executed, or null\.

### Remarks

Example: `throw new CurlRateLimitException("api.github.com rate limit reached", TimeSpan.FromSeconds(60), 0, command);`
            gives the advice to wait 60 s.

### See Also
- [CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.GetRetryAfter\(\) Method

Reads the server's `Retry-After` header \(in seconds\) so you know how long to wait before the next
request after a 429 or 503\.

```csharp
public System.Nullable<System.TimeSpan> GetRetryAfter();
```

#### Returns
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The delay, or null when the header is missing or not a number of seconds\.

### Remarks

Only the delay-seconds form (`Retry-After: 120`) is understood; an HTTP-date value returns null.
            Example: `await Task.Delay(ex.GetRetryAfter() ?? TimeSpan.FromSeconds(10));`

### See Also
- [IsRateLimited](CurlDotNet.Exceptions.CurlHttpException.IsRateLimited.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRateLimited')
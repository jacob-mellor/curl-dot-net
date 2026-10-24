#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

## CurlRateLimitException\.RetryAfter Property

How long the server asked you to wait before the next request \(its `Retry-After` header\), or null when it
did not say\.

```csharp
public System.Nullable<System.TimeSpan> RetryAfter { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.TimeSpan](https://learn.microsoft.com/en-us/dotnet/api/system.timespan 'System\.TimeSpan')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
The wait time, or null\.

### Remarks

Example: `await Task.Delay(ex.RetryAfter ?? TimeSpan.FromSeconds(30));`

### See Also
- [RemainingLimit](CurlDotNet.Exceptions.CurlRateLimitException.RemainingLimit.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RemainingLimit')
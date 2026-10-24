#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRateLimitException](CurlDotNet.Exceptions.CurlRateLimitException.md 'CurlDotNet\.Exceptions\.CurlRateLimitException')

## CurlRateLimitException\.RemainingLimit Property

How many requests the server still allows in the current window \(for example from `X-RateLimit-Remaining`\),
or null when unknown\.

```csharp
public System.Nullable<int> RemainingLimit { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')  
A non\-negative count, usually 0 for a 429, or null\.

### Remarks

Log it to see how close callers run to the limit.

### See Also
- [RetryAfter](CurlDotNet.Exceptions.CurlRateLimitException.RetryAfter.md 'CurlDotNet\.Exceptions\.CurlRateLimitException\.RetryAfter')
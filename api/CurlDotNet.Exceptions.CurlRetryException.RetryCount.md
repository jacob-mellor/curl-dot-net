#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')

## CurlRetryException\.RetryCount Property

How many retry attempts were made before giving up \(not counting the first try, when the middleware is
configured that way\), useful for logs and alerts\.

```csharp
public int RetryCount { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The configured maximum number of retries that were all used up\.

### Remarks

Example: `metrics.Increment("curl.retries_exhausted", ex.RetryCount);`

### See Also
- [LastAttemptException](CurlDotNet.Exceptions.CurlRetryException.LastAttemptException.md 'CurlDotNet\.Exceptions\.CurlRetryException\.LastAttemptException')
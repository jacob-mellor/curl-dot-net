#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlRetryException](CurlDotNet.Exceptions.CurlRetryException.md 'CurlDotNet\.Exceptions\.CurlRetryException')

## CurlRetryException\.LastAttemptException Property

The exception thrown by the final attempt \(also [System\.Exception\.InnerException](https://learn.microsoft.com/en-us/dotnet/api/system.exception.innerexception 'System\.Exception\.InnerException')\), which tells you why
the request keeps failing\.

```csharp
public System.Exception LastAttemptException { get; }
```

#### Property Value
[System\.Exception](https://learn.microsoft.com/en-us/dotnet/api/system.exception 'System\.Exception')  
Usually a [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException') subclass such as [CurlOperationTimeoutException](CurlDotNet.Exceptions.CurlOperationTimeoutException.md 'CurlDotNet\.Exceptions\.CurlOperationTimeoutException'); null
            when the last attempt returned a failing result instead of throwing\.

### Remarks

Example: `if (ex.LastAttemptException is CurlCouldntResolveHostException) /* no point retrying */;`

### See Also
- [RetryCount](CurlDotNet.Exceptions.CurlRetryException.RetryCount.md 'CurlDotNet\.Exceptions\.CurlRetryException\.RetryCount')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsServerError Property

True for 5xx statuses \(500 to 599\): the server failed; the same request may work later, so it is a
candidate for a retry with backoff\.

```csharp
public bool IsServerError { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') is between 500 and 599\.

### Remarks

Example: `catch (CurlHttpException ex) when (ex.IsServerError) { await Task.Delay(2000); /* retry */ }`

### See Also
- [IsClientError](CurlDotNet.Exceptions.CurlHttpException.IsClientError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsClientError')
- [IsRetryable\(\)](CurlDotNet.Exceptions.CurlHttpException.IsRetryable().md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsRetryable\(\)')
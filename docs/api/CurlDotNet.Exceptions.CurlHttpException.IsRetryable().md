#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsRetryable\(\) Method

Tells whether the same request may succeed later: true for 429 \(rate limited\) and every 5xx server error,
false for other 4xx errors, which need the request to change first\.

```csharp
public override bool IsRetryable();
```

#### Returns
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for status 429 or 500 and above; otherwise false\.

### Remarks

Example: `catch (CurlHttpException ex) when (ex.IsRetryable()) { await Task.Delay(ex.GetRetryAfter() ?? TimeSpan.FromSeconds(5)); }`

### See Also
- [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)')
- [IsServerError](CurlDotNet.Exceptions.CurlHttpException.IsServerError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsServerError')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsForbidden Property

True for 403 Forbidden: the server knows who you are but this account or token may not do this \(missing
scope, role or IP allow\-list\)\.

```csharp
public bool IsForbidden { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') is 403\.

### Remarks

Re-sending the same credentials will not help; request the permission or use another account.

### See Also
- [IsUnauthorized](CurlDotNet.Exceptions.CurlHttpException.IsUnauthorized.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsUnauthorized')
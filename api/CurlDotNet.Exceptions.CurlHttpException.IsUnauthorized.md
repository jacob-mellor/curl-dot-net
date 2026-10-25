#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsUnauthorized Property

True for 401 Unauthorized: the credentials are missing, wrong or expired \(refresh the token or check
`-u`/`-H 'Authorization: ...'`\)\.

```csharp
public bool IsUnauthorized { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') is 401\.

### Remarks

See https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-bearer-token-csharp/README.md

### See Also
- [IsForbidden](CurlDotNet.Exceptions.CurlHttpException.IsForbidden.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsForbidden')
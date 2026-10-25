#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.IsClientError Property

True for 4xx statuses \(400 to 499\): the request itself is wrong \(URL, credentials, body\) and repeating it
unchanged will fail again, except for 408 and 429\.

```csharp
public bool IsClientError { get; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True when [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode') is between 400 and 499\.

### Remarks

Example: `if (ex.IsClientError) log.LogWarning("Fix the request: {Body}", ex.ResponseBody);`

### See Also
- [IsServerError](CurlDotNet.Exceptions.CurlHttpException.IsServerError.md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsServerError')
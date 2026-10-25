#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.StatusCode Property

The HTTP status code the server answered with, such as 404 or 503, so you can branch on it without
parsing the message\.

```csharp
public int StatusCode { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')  
The numeric status from the status line, normally 400 to 599\.

### Remarks

Example: `switch (ex.StatusCode) { case 404: return null; case 409: return await MergeAsync(); default: throw; }`

### See Also
- [IsStatus\(int\)](CurlDotNet.Exceptions.CurlHttpException.IsStatus(int).md 'CurlDotNet\.Exceptions\.CurlHttpException\.IsStatus\(int\)')
- [StatusText](CurlDotNet.Exceptions.CurlHttpException.StatusText.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusText')
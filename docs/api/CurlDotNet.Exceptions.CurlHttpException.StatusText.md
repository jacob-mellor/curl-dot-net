#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.StatusText Property

The reason phrase from the response's status line, such as `Not Found` or `Service Unavailable`,
when the server sent one\.

```csharp
public string StatusText { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The text after the status code; null or empty when the server sent none \(HTTP/2 never does\)\.

### Remarks

For display only; branch on [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode'), never on this text.

### See Also
- [StatusCode](CurlDotNet.Exceptions.CurlHttpException.StatusCode.md 'CurlDotNet\.Exceptions\.CurlHttpException\.StatusCode')
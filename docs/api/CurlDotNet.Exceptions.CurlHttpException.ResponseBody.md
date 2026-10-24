#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.ResponseBody Property

The body of the error response, which usually holds the server's own explanation \(a JSON error object or
an HTML error page\)\.

```csharp
public string ResponseBody { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The decoded response text; null when the body was not read \(`-f` without `--fail-with-body`
            discards it\) or empty\.

### Remarks

Example: `var problem = JsonSerializer.Deserialize<ProblemDetails>(ex.ResponseBody ?? "{}");`.
            Use `--fail-with-body` instead of `-f` to keep the body.

### See Also
- [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders')
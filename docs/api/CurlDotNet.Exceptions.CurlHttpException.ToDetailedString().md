#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.ToDetailedString\(\) Method

Returns the base detailed report plus the HTTP status line, client/server classification, response body
and response headers, for logs\.

```csharp
public override string ToDetailedString();
```

#### Returns
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The text of [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)') followed by `HTTP Status: 404 Not Found`
            and the body and header lines\.

### Remarks

The body is printed in full; trim it before logging very large error pages. Example:
            `logger.LogError(ex.ToDetailedString());`

### See Also
- [ToDetailedString\(\)](CurlDotNet.Exceptions.CurlException.ToDetailedString().md 'CurlDotNet\.Exceptions\.CurlException\.ToDetailedString\(\)')
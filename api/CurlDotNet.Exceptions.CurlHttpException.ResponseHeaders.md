#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.ResponseHeaders Property

The response headers of the error response \(for example `Retry-After`, `WWW-Authenticate`,
`X-Request-Id`\), keyed by header name\.

```csharp
public System.Collections.Generic.Dictionary<string,string> ResponseHeaders { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A dictionary of header name to value; empty when headers were not attached\.

### Remarks

Example: `ex.ResponseHeaders.TryGetValue("X-Request-Id", out var id)` gives the id to quote to the API
            provider's support. [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)') reads `Retry-After` for you.

### See Also
- [GetRetryAfter\(\)](CurlDotNet.Exceptions.CurlHttpException.GetRetryAfter().md 'CurlDotNet\.Exceptions\.CurlHttpException\.GetRetryAfter\(\)')
- [WithHeaders\(Dictionary&lt;string,string&gt;\)](CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_).md 'CurlDotNet\.Exceptions\.CurlHttpException\.WithHeaders\(System\.Collections\.Generic\.Dictionary\<string,string\>\)')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Exceptions](CurlDotNet.Exceptions.md 'CurlDotNet\.Exceptions').[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')

## CurlHttpException\.WithHeaders\(Dictionary\<string,string\>\) Method

Attaches the response headers to [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders') and returns the same exception, for code that
builds the exception from a result\.

```csharp
public CurlDotNet.Exceptions.CurlHttpException WithHeaders(System.Collections.Generic.Dictionary<string,string> headers);
```
#### Parameters

<a name='CurlDotNet.Exceptions.CurlHttpException.WithHeaders(System.Collections.Generic.Dictionary_string,string_).headers'></a>

`headers` [System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')

The response headers, header name to value; replaces the current dictionary\.

#### Returns
[CurlHttpException](CurlDotNet.Exceptions.CurlHttpException.md 'CurlDotNet\.Exceptions\.CurlHttpException')  
This same exception instance, for chaining\.

### Remarks

Example: `throw new CurlHttpException("GET failed", r.StatusCode, null, r.Body).WithHeaders(r.Headers);`

### See Also
- [ResponseHeaders](CurlDotNet.Exceptions.CurlHttpException.ResponseHeaders.md 'CurlDotNet\.Exceptions\.CurlHttpException\.ResponseHeaders')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.IncludeHeaders\(bool\) Method

Puts the response status line and headers in front of the body in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')
\(and in the `-o` file\), like curl's `-i` / `--include`\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder IncludeHeaders(bool include=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.IncludeHeaders(bool).include'></a>

`include` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) to include headers; false for the body only\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Headers are always available in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'); use this only when you want the
            raw text curl would print, for example to log the full response.

Example: `(await CurlRequestBuilder.Get(url).IncludeHeaders().ExecuteAsync()).Body` starts with
            `HTTP/1.1 200 OK`, like `curl -i URL`.

### See Also
- [IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders')
- [Head\(string\)](CurlDotNet.Core.CurlRequestBuilder.Head(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)')
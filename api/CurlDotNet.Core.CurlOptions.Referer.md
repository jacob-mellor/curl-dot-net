#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Referer Property

The `Referer` header sent with the request, verbatim, like curl's `-e`/`--referer`; it need
not be an absolute URL\.

```csharp
public string? Referer { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The header value, such as `"https://example.com/start"`\. Default: null \(no Referer\)\.

### Remarks

The parser strips curl's `;auto` suffix and sets [AutoReferer](CurlDotNet.Core.CurlOptions.AutoReferer.md 'CurlDotNet\.Core\.CurlOptions\.AutoReferer') instead, so this property only
            holds the literal value. An explicit `Referer` in [Headers](CurlDotNet.Core.CurlOptions.Headers.md 'CurlDotNet\.Core\.CurlOptions\.Headers') wins.

Example: `curl -e https://example.com/start URL` is `new CurlOptions { Url = url, Referer = "https://example.com/start" }`.

### See Also
- [AutoReferer](CurlDotNet.Core.CurlOptions.AutoReferer.md 'CurlDotNet\.Core\.CurlOptions\.AutoReferer')
- [WithReferer\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithReferer(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithReferer\(string\)')
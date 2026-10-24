#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Method Property

The HTTP method sent on the request line \(`GET`, `POST`, `PUT`, `DELETE`, `PATCH`,
`HEAD`, `OPTIONS`\); curl chooses it from `-X`, `-d`, `-F`, `-T` and `-I`\.

```csharp
public string? Method { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An upper\-case method name\. Default: `"GET"`\. Null is treated as GET\.

### Remarks

Only the seven names above are recognised here; any other value is sent as GET. For WebDAV and other
            custom verbs (`PROPFIND`, `MKCOL`, `PURGE`) set [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod'), which is what
            `-X` does and which is sent exactly as written.

A hand-built GET or HEAD sends no body even when [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data') is set; set `Method = "POST"`
            (or PUT/PATCH) for a body. The parser does this for you: `-d` selects POST, `-T` PUT, `-I` HEAD.

Example: `curl -X DELETE https://api.example.com/items/7` is
            `new CurlOptions { Url = "https://api.example.com/items/7", Method = "DELETE" }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-put-delete-patch-csharp/README.md

### See Also
- [CustomMethod](CurlDotNet.Core.CurlOptions.CustomMethod.md 'CurlDotNet\.Core\.CurlOptions\.CustomMethod')
- [HeadOnly](CurlDotNet.Core.CurlOptions.HeadOnly.md 'CurlDotNet\.Core\.CurlOptions\.HeadOnly')
- [Data](CurlDotNet.Core.CurlOptions.Data.md 'CurlDotNet\.Core\.CurlOptions\.Data')
- [Request\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.Request(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Request\(string, string\)')
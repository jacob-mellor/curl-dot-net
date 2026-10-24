#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Headers Property

Request headers to send, keyed by header name \(`Accept`, `Authorization`, `X-Api-Key`\.\.\.\),
exactly what curl's `-H "Name: value"` adds; names compare case\-insensitively\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Headers { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
A dictionary of name to value \(not `"Name: value"` lines\)\. Default: empty, which sends only
            curl's own headers \(`Host`, `User-Agent`, `Accept: */*`, plus body headers\)\.

### Remarks

An empty value removes a header curl would send itself: `Headers["Accept"] = ""` is
            `-H "Accept:"`. To send a header with an empty value use [EmptyHeaders](CurlDotNet.Core.CurlOptions.EmptyHeaders.md 'CurlDotNet\.Core\.CurlOptions\.EmptyHeaders') (`-H "Name;"`).
            A second `-H` with the same name is joined to the first with `", "` by the parser. `Content-Type`
            and other content headers are applied to the body; `Host` replaces the Host header; `User-Agent`
            is overridden by [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent') when that is set. Values must not contain CR or LF.

Example: `curl -H "Authorization: Bearer abc123" -H "Accept: application/json" URL` is
            `options.Headers["Authorization"] = "Bearer abc123"; options.Headers["Accept"] = "application/json";`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-custom-headers-user-agent-csharp/README.md

### See Also
- [EmptyHeaders](CurlDotNet.Core.CurlOptions.EmptyHeaders.md 'CurlDotNet\.Core\.CurlOptions\.EmptyHeaders')
- [UserAgent](CurlDotNet.Core.CurlOptions.UserAgent.md 'CurlDotNet\.Core\.CurlOptions\.UserAgent')
- [Referer](CurlDotNet.Core.CurlOptions.Referer.md 'CurlDotNet\.Core\.CurlOptions\.Referer')
- [WithHeader\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithHeader(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithHeader\(string, string\)')
- [Headers](CurlDotNet.Core.CurlSettings.Headers.md 'CurlDotNet\.Core\.CurlSettings\.Headers')
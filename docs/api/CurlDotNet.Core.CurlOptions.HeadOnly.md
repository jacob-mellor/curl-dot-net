#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.HeadOnly Property

Sends a HEAD request and returns only the response headers, no body, like curl's `-I`/`--head`;
for parsed commands the headers also appear in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public bool HeadOnly { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for a headers\-only request\. Default: false\.

### Remarks

The parser sets this together with `Method = "HEAD"`. On a parsed command (one with
            [OriginalCommand](CurlDotNet.Core.CurlOptions.OriginalCommand.md 'CurlDotNet\.Core\.CurlOptions\.OriginalCommand')) the header block is written to the output like curl does; on hand-built options
            the headers are only in [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers'). Redirects followed with [FollowLocation](CurlDotNet.Core.CurlOptions.FollowLocation.md 'CurlDotNet\.Core\.CurlOptions\.FollowLocation')
            stay HEAD requests.

Example: `curl -I https://example.com/big.iso` is
            `new CurlOptions { Url = "https://example.com/big.iso", HeadOnly = true, Method = "HEAD" }`; read
            `result.Headers["Content-Length"]`. Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-head-request-csharp/README.md

### See Also
- [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method')
- [IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders')
- [Head\(string\)](CurlDotNet.Core.CurlRequestBuilder.Head(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Head\(string\)')
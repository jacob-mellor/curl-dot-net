#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Url Property

The URL to request, such as `https://api.example.com/v1/items?page=2`; the one required setting,
equivalent to the URL argument \(or `--url`\) of a curl command\.

```csharp
public string Url { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
An absolute URL with scheme `http`, `https`, `ftp`, `ftps` or `file`\.
            Default: empty string, which fails with exit code 2 \("No URL specified"\) when executed\.

### Remarks

A URL without a scheme (`example.com/path`) gets `http://`, like curl. User info in the URL
            (`https://alice:secret@host/`) becomes Basic credentials for that host. Spaces and other characters that
            are not valid in a URL must be percent-encoded (`%20`); with a command string, curl globbing
            (`{a,b}`, `[1-3]`) is expanded by the parser before this property is set (see [GlobOff](CurlDotNet.Core.CurlOptions.GlobOff.md 'CurlDotNet\.Core\.CurlOptions\.GlobOff')).

Executing the options normalises this property in place (for example adding the missing scheme).

Example: `curl "https://api.example.com/search?q=dotnet"` is `new CurlOptions { Url = "https://api.example.com/search?q=dotnet" }`.
            An unsupported scheme fails with [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException') (exit 1),
            a malformed URL with [CurlMalformedUrlException](CurlDotNet.Exceptions.CurlMalformedUrlException.md 'CurlDotNet\.Exceptions\.CurlMalformedUrlException') (exit 3); see
            https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/exceptions/url-errors.md

### See Also
- [Method](CurlDotNet.Core.CurlOptions.Method.md 'CurlDotNet\.Core\.CurlOptions\.Method')
- [UseGet](CurlDotNet.Core.CurlOptions.UseGet.md 'CurlDotNet\.Core\.CurlOptions\.UseGet')
- [GlobOff](CurlDotNet.Core.CurlOptions.GlobOff.md 'CurlDotNet\.Core\.CurlOptions\.GlobOff')
- [Get\(string\)](CurlDotNet.Core.CurlRequestBuilder.Get(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Get\(string\)')
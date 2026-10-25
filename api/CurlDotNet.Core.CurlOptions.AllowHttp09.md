#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.AllowHttp09 Property

Accepts an HTTP/0\.9 reply \(a bare body with no status line or headers\) as the response, like curl's
`--http0.9`; needed for some very old devices and raw TCP services\.

```csharp
public bool AllowHttp09 { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to accept HTTP/0\.9\. Default: false: such a reply fails with
            [CurlUnsupportedProtocolException](CurlDotNet.Exceptions.CurlUnsupportedProtocolException.md 'CurlDotNet\.Exceptions\.CurlUnsupportedProtocolException') \(exit 1\), like curl 7\.66 and later\.

### Remarks

Cleared again by `--no-http0.9`. Example: `curl --http0.9 http://192.168.1.20:9000/` is
            `new CurlOptions { Url = url, AllowHttp09 = true }`.

### See Also
- [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion')
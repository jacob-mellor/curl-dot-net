#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.TransferEncoding Property

Asks for a compressed transfer\-encoding \(`TE: gzip`\) and decodes a `Transfer-Encoding: gzip` or
`deflate` body, like curl's `--tr-encoding`\.

```csharp
public bool TransferEncoding { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to request and decode transfer compression\. Default: false: such a response fails with
            [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException') \(exit 61\), like curl\.

### Remarks

Sends `TE: gzip` with `Connection: TE`. For the common `Content-Encoding` compression use
            [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed'). Example: `curl --tr-encoding URL` is `new CurlOptions { Url = url, TransferEncoding = true }`.

### See Also
- [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Compressed Property

Asks for a compressed response \(`Accept-Encoding: gzip, deflate, br`\) and decompresses the body, like curl's
`--compressed`; the headers keep the server's `Content-Encoding`\.

```csharp
public bool Compressed { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to request and decode compression\. Default: false \(the body is returned exactly as sent\)\.

### Remarks

Brotli (`br`) is offered on .NET 8/9/10; on .NET Standard 2.0 and .NET Framework only gzip and deflate.
            A body that cannot be decoded fails with [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException') (exit 61).
            For `Transfer-Encoding: gzip` see [TransferEncoding](CurlDotNet.Core.CurlOptions.TransferEncoding.md 'CurlDotNet\.Core\.CurlOptions\.TransferEncoding').

Example: `curl --compressed https://api.example.com/large.json` is
            `new CurlOptions { Url = url, Compressed = true }`.

### See Also
- [TransferEncoding](CurlDotNet.Core.CurlOptions.TransferEncoding.md 'CurlDotNet\.Core\.CurlOptions\.TransferEncoding')
- [Compressed\(bool\)](CurlDotNet.Core.CurlRequestBuilder.Compressed(bool).md 'CurlDotNet\.Core\.CurlRequestBuilder\.Compressed\(bool\)')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.Compressed\(bool\) Method

Asks for a compressed response \(`Accept-Encoding: gzip, deflate, br`\) and decompresses it for you,
like curl's `--compressed`; saves bandwidth on large JSON or HTML\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder Compressed(bool compressed=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.Compressed(bool).compressed'></a>

`compressed` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

True \(the default\) to request and decode compression\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Brotli (`br`) is offered on .NET 8/9/10; the .NET Standard 2.0 and .NET Framework builds offer
            `gzip, deflate`. [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') and the `-o` file contain the decompressed data. Without this option
            nothing is requested and a server that compresses anyway delivers the compressed bytes, exactly like curl.
            A body that cannot be decoded fails with [CurlBadContentEncodingException](CurlDotNet.Exceptions.CurlBadContentEncodingException.md 'CurlDotNet\.Exceptions\.CurlBadContentEncodingException') (exit code 61).

Example: `CurlRequestBuilder.Get("https://api.example.com/large-list").Compressed()` is
            `curl --compressed https://api.example.com/large-list`.

### See Also
- [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed')
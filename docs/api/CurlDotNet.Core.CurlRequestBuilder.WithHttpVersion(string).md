#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithHttpVersion\(string\) Method

Chooses the HTTP protocol version to request: `"1.0"`, `"1.1"`, `"2"`/`"2.0"` or
`"3"`/`"3.0"` \(curl's `--http1.0`, `--http1.1`, `--http2`, `--http3`\)\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithHttpVersion(string version);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithHttpVersion(string).version'></a>

`version` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

One of `"1.0"`, `"1.1"`, `"2"`, `"2.0"`, `"3"`, `"3.0"`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Without it .NET negotiates (normally HTTP/1.1, or HTTP/2 over TLS when the server offers it).
            `"2"` and `"3"` ask for that version and fall back to a lower one when the server or platform
            cannot do it, like curl; HTTP/3 also needs QUIC support in the operating system (msquic/libmsquic).
            HTTP/2 and HTTP/3 are available on the .NET 8/9/10 builds; on the .NET Standard 2.0 and .NET Framework
            builds use 1.0 or 1.1. `"1"` is accepted as 1.0. Unknown strings are ignored (no flag is printed by
            [ToCurlCommand\(\)](CurlDotNet.Core.CurlRequestBuilder.ToCurlCommand().md 'CurlDotNet\.Core\.CurlRequestBuilder\.ToCurlCommand\(\)') either).

Example: `CurlRequestBuilder.Get("https://www.google.com").WithHttpVersion("2")` is
            `curl --http2 https://www.google.com`; the version used is in the response's status line.

### See Also
- [HttpVersion](CurlDotNet.Core.CurlOptions.HttpVersion.md 'CurlDotNet\.Core\.CurlOptions\.HttpVersion')
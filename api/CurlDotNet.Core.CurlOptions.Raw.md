#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Raw Property

Passes transfer\-codings through instead of interpreting them, like curl's `--raw`: a chunked body keeps its
chunk framing and unknown `Transfer-Encoding` values are accepted\.

```csharp
public bool Raw { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True for raw mode\. Default: false\.

### Remarks

The chunk sizes are written to the output when the wire bytes are available (.NET 5 and later); on .NET Standard 2.0
            and .NET Framework HttpClient removes the framing first. With an unknown transfer-coding the body is read until the server
            closes. Used for debugging servers and proxies.

Example: `curl --raw URL` is `new CurlOptions { Url = url, Raw = true }`.

### See Also
- [TransferEncoding](CurlDotNet.Core.CurlOptions.TransferEncoding.md 'CurlDotNet\.Core\.CurlOptions\.TransferEncoding')
- [IgnoreContentLength](CurlDotNet.Core.CurlOptions.IgnoreContentLength.md 'CurlDotNet\.Core\.CurlOptions\.IgnoreContentLength')
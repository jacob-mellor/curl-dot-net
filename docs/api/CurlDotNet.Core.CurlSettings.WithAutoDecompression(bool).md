#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')

## CurlSettings\.WithAutoDecompression\(bool\) Method

Requests and decodes compressed responses \([enable](CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).md#CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).enable 'CurlDotNet\.Core\.CurlSettings\.WithAutoDecompression\(bool\)\.enable') true, like `--compressed`\) or turns
that off \(false, like `--no-compressed`\), and returns these settings for chaining\.

```csharp
public CurlDotNet.Core.CurlSettings WithAutoDecompression(bool enable=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlSettings.WithAutoDecompression(bool).enable'></a>

`enable` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true \(default\) to ask for and decode gzip/deflate/brotli; false to send no
            `Accept-Encoding` and keep the body as received\.

#### Returns
[CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings')  
This same [CurlSettings](CurlDotNet.Core.CurlSettings.md 'CurlDotNet\.Core\.CurlSettings') instance\.

### Remarks

Example: `await Curl.ExecuteAsync("curl https://api.example.com/data", new CurlSettings().WithAutoDecompression());`
            receives a gzip or brotli body and returns it decoded.

### See Also
- [AutomaticDecompression](CurlDotNet.Core.CurlSettings.AutomaticDecompression.md 'CurlDotNet\.Core\.CurlSettings\.AutomaticDecompression')
- [Compressed](CurlDotNet.Core.CurlOptions.Compressed.md 'CurlDotNet\.Core\.CurlOptions\.Compressed')
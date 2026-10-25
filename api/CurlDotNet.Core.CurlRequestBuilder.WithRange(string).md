#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithRange\(string\) Method

Downloads only part of the resource \(curl's `-r` / `--range`\), sent as
`Range: bytes=RANGE`; useful for resuming or sampling large files\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithRange(string range);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithRange(string).range'></a>

`range` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Byte range such as `"0-499"`, `"500-"`, `"-500"` or `"0-99,200-299"`, without the `bytes=` prefix\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

Formats: `"0-499"` (first 500 bytes), `"500-999"`, `"500-"` (from byte 500 to the end),
            `"-500"` (last 500 bytes), or several ranges `"0-99,200-299"` (the server may answer multipart).
            A server that supports ranges answers `206 Partial Content`; one that does not sends the whole file
            with 200, which you can detect from [StatusCode](CurlDotNet.Core.CurlResult.StatusCode.md 'CurlDotNet\.Core\.CurlResult\.StatusCode').

Example: `CurlRequestBuilder.Get("https://example.com/video.mp4").WithRange("0-1048575").SaveToFile("first-mb.mp4")`
            is `curl -r 0-1048575 -o first-mb.mp4 https://example.com/video.mp4`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [Range](CurlDotNet.Core.CurlOptions.Range.md 'CurlDotNet\.Core\.CurlOptions\.Range')
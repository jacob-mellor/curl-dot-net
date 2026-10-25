#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Range Property

Asks for part of the document with `Range: bytes=RANGE`, like curl's `-r`/`--range`; useful
for reading the start of a large file or downloading it in pieces\.

```csharp
public string? Range { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A byte range without the `bytes=` prefix: `0-499`, `500-`, `-500` \(last 500 bytes\) or
            `0-99,200-299`\. Default: null\.

### Remarks

A server that supports ranges answers `206 Partial Content`; one that does not sends the whole document
            with 200. With [UploadFile](CurlDotNet.Core.CurlOptions.UploadFile.md 'CurlDotNet\.Core\.CurlOptions\.UploadFile') (`-T`) the range is sent as `Content-Range` instead.

Example: `curl -r 0-1023 https://example.com/big.bin` is `new CurlOptions { Url = url, Range = "0-1023" }`.

### See Also
- [ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom')
- [WithRange\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithRange(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithRange\(string\)')
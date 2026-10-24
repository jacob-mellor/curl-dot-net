#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.EtagCompare Property

Sends the ETag stored in this file as `If-None-Match`, like curl's `--etag-compare FILE`, so the server
answers `304 Not Modified` with no body when the document has not changed\.

```csharp
public string? EtagCompare { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path \(first non\-empty line is the ETag\)\. Default: null\.

### Remarks

A missing file sends an empty `If-None-Match: ""`, which forces a download. A 304 leaves an existing output file untouched.
            An explicit `If-None-Match` header wins.

Example: `curl --etag-compare etag.txt --etag-save etag.txt -o feed.xml URL` is
            `new CurlOptions { Url = url, OutputFile = "feed.xml", EtagCompare = "etag.txt", EtagSave = "etag.txt" }`.

### See Also
- [EtagSave](CurlDotNet.Core.CurlOptions.EtagSave.md 'CurlDotNet\.Core\.CurlOptions\.EtagSave')
- [TimeCondition](CurlDotNet.Core.CurlOptions.TimeCondition.md 'CurlDotNet\.Core\.CurlOptions\.TimeCondition')
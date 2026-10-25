#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.UploadFile Property

Local file uploaded as the request body with PUT, like curl's `-T`/`--upload-file FILE`; the URL gets
the file name appended when it ends with `/`\.

```csharp
public string? UploadFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
The path as given on the command line\. Default: null\.

### Remarks

The parser reads the file into [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData') and selects PUT unless `-X` names another method; for
            hand-built options set [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData') as well. No Content-Type is sent unless a `-H "Content-Type: ..."`
            header is given, exactly like curl. When the URL has no path or its path ends with `/` (and there is no query
            string) the file name is appended to it, percent-encoded: `-T report.pdf https://example.com/files/` uploads to
            `/files/report.pdf`.

A missing file fails with exit code 26 ([CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException')). Reading the
            body from standard input (`-T -` or `-T .`) is not supported.

Example: `curl -T ./backup.zip https://example.com/upload/`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [BinaryData](CurlDotNet.Core.CurlOptions.BinaryData.md 'CurlDotNet\.Core\.CurlOptions\.BinaryData')
- [Range](CurlDotNet.Core.CurlOptions.Range.md 'CurlDotNet\.Core\.CurlOptions\.Range')
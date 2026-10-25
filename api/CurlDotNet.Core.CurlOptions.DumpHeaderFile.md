#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.DumpHeaderFile Property

Writes every response header block of the transfer to this file, like curl's `-D`/`--dump-header FILE`;
`"-"` puts them in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') instead\.

```csharp
public string? DumpHeaderFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, or `"-"`\. Default: null\.

### Remarks

Every header block is written (interim `1xx` responses, authentication challenges and each redirect hop), exactly as
            received, plus chunked trailers. The folder is checked before the request is sent: a folder that does not exist fails with
            [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit 23) unless [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') is set. Several
            transfers of one command append to the same file.

Example: `curl -D headers.txt -o page.html https://example.com/` is
            `new CurlOptions { Url = "https://example.com/", DumpHeaderFile = "headers.txt", OutputFile = "page.html" }`.

### See Also
- [IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders')
- [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs')
- [Headers](CurlDotNet.Core.CurlResult.Headers.md 'CurlDotNet\.Core\.CurlResult\.Headers')
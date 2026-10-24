#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.Files Property

Files uploaded as parts of a `multipart/form-data` body, field name to local file path, as curl's
`-F name=@path` sends them with `filename="basename"` and a guessed Content\-Type\.

```csharp
public System.Collections.Generic.Dictionary<string,string> Files { get; set; }
```

#### Property Value
[System\.Collections\.Generic\.Dictionary&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[,](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2 'System\.Collections\.Generic\.Dictionary\`2')  
Field name to file path \(absolute, or relative to the current directory\)\. Default: empty\.

### Remarks

The Content-Type of each file part is guessed from its extension (`.png` is `image/png`,
            unknown is `application/octet-stream`). A file that cannot be read fails with
            [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException') (exit 26) when the request is built. For custom part
            types or file names use [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts') (`-F "file=@a.bin;type=application/x-thing;filename=b.bin"`).

Example: `curl -F "report=@./q3.pdf" https://example.com/upload` is
            `new CurlOptions { Url = "https://example.com/upload", Method = "POST", Files = { ["report"] = "./q3.pdf" } }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [FormData](CurlDotNet.Core.CurlOptions.FormData.md 'CurlDotNet\.Core\.CurlOptions\.FormData')
- [FormParts](CurlDotNet.Core.CurlOptions.FormParts.md 'CurlDotNet\.Core\.CurlOptions\.FormParts')
- [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)')
- [WithUploadFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithOutput\(string\) Method

Writes the response body to a file instead of keeping it only in memory \(curl's `-o` /
`--output`\); binary downloads are saved byte for byte\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithOutput(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Destination file path, such as `"data/out.json"`\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

### Remarks

The path may be absolute or relative to the current directory; an existing file is overwritten.
            The folder must exist: a missing folder fails with [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit code 23)
            naming the path (curl's `--create-dirs` in a command string creates it). `"-"` means no file
            (standard output, i.e. [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')). The written path is listed in
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: `await CurlRequestBuilder.Get("https://example.com/report.xlsx").WithOutput(@"C:\downloads\report.xlsx").ExecuteAsync();`
            is `curl -o 'C:\downloads\report.xlsx' https://example.com/report.xlsx`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlRequestBuilder.SaveToFile(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveToFile\(string\)')
- [SaveWithRemoteName\(\)](CurlDotNet.Core.CurlRequestBuilder.SaveWithRemoteName().md 'CurlDotNet\.Core\.CurlRequestBuilder\.SaveWithRemoteName\(\)')
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [WithOutputFile\(string\)](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)')
- [DownloadAsync\(string, string\)](CurlDotNet.Curl.DownloadAsync(string,string).md 'CurlDotNet\.Curl\.DownloadAsync\(string, string\)')
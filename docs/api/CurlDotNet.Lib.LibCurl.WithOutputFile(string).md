#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Lib](CurlDotNet.Lib.md 'CurlDotNet\.Lib').[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')

## LibCurl\.WithOutputFile\(string\) Method

Writes the body of every request to [filePath](CurlDotNet.Lib.LibCurl.WithOutputFile(string).md#CurlDotNet.Lib.LibCurl.WithOutputFile(string).filePath 'CurlDotNet\.Lib\.LibCurl\.WithOutputFile\(string\)\.filePath') \(curl's `-o`\), overwriting it each
time, unless a request sets its own [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')\.

```csharp
public CurlDotNet.Lib.LibCurl WithOutputFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Lib.LibCurl.WithOutputFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Destination file path, absolute or relative to the current directory\.

#### Returns
[LibCurl](CurlDotNet.Lib.LibCurl.md 'CurlDotNet\.Lib\.LibCurl')  
This instance, for chaining\.

### Remarks

Because every request overwrites the same file, this is mostly useful for a client dedicated to one
            download; per request prefer `opts => opts.OutputFile = "report.pdf"` in the `configure`
            callback. A folder that does not exist gives [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit code 23) in
            [Exception](CurlDotNet.Core.CurlResult.Exception.md 'CurlDotNet\.Core\.CurlResult\.Exception').

Example: `await curl.GetAsync("https://example.com/report.pdf", opts => opts.OutputFile = @"C:\downloads\report.pdf");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)')
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
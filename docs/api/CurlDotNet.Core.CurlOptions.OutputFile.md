#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.OutputFile Property

Writes the response body to this file instead of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body'), like curl's `-o FILE`;
`"-"` means standard output, which is [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\.

```csharp
public string? OutputFile { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, absolute or relative to [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir') \(or the current directory\)\. Default: null\.

### Remarks

The file is created or replaced; its path is added to [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'). With
            [IncludeHeaders](CurlDotNet.Core.CurlOptions.IncludeHeaders.md 'CurlDotNet\.Core\.CurlOptions\.IncludeHeaders') the headers are written to the file too. The folder must exist unless
            [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') is true; otherwise, or when the file cannot be written, the transfer fails with
            [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit 23). [NoClobber](CurlDotNet.Core.CurlOptions.NoClobber.md 'CurlDotNet\.Core\.CurlOptions\.NoClobber'),
            [SkipExisting](CurlDotNet.Core.CurlOptions.SkipExisting.md 'CurlDotNet\.Core\.CurlOptions\.SkipExisting'), [RemoveOnError](CurlDotNet.Core.CurlOptions.RemoveOnError.md 'CurlDotNet\.Core\.CurlOptions\.RemoveOnError') and [ResumeFrom](CurlDotNet.Core.CurlOptions.ResumeFrom.md 'CurlDotNet\.Core\.CurlOptions\.ResumeFrom') (`-C -`) change how
            an existing file is treated.

For `file://` URLs the file handler is simpler: it always creates the output folder (no
            [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') needed), returns status 404 for a missing source file instead of failing (so
            [FailOnError](CurlDotNet.Core.CurlOptions.FailOnError.md 'CurlDotNet\.Core\.CurlOptions\.FailOnError') does not apply), and ignores [NoClobber](CurlDotNet.Core.CurlOptions.NoClobber.md 'CurlDotNet\.Core\.CurlOptions\.NoClobber'), [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir'),
            [DumpHeaderFile](CurlDotNet.Core.CurlOptions.DumpHeaderFile.md 'CurlDotNet\.Core\.CurlOptions\.DumpHeaderFile') and [WriteOut](CurlDotNet.Core.CurlOptions.WriteOut.md 'CurlDotNet\.Core\.CurlOptions\.WriteOut').

Example: `curl -o report.pdf https://example.com/report.pdf` is
            `new CurlOptions { Url = "https://example.com/report.pdf", OutputFile = "report.pdf" }`.
            Guide: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName')
- [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir')
- [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs')
- [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles')
- [WithOutput\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithOutput(string).md 'CurlDotNet\.Core\.CurlRequestBuilder\.WithOutput\(string\)')
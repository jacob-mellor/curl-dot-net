#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.CreateDirs Property

Creates missing folders of the output paths \(`-o`, `-O` with `--output-dir`, `-D`,
`--etag-save`\) before writing, like curl's `--create-dirs`\.

```csharp
public bool CreateDirs { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to create folders\. Default: false: a missing folder fails with exit code 23
            \([CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')\)\.

### Remarks

Example: `curl --create-dirs -o downloads/2024/report.pdf URL` is
            `new CurlOptions { Url = url, OutputFile = "downloads/2024/report.pdf", CreateDirs = true }`.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [OutputDir](CurlDotNet.Core.CurlOptions.OutputDir.md 'CurlDotNet\.Core\.CurlOptions\.OutputDir')
- [DumpHeaderFile](CurlDotNet.Core.CurlOptions.DumpHeaderFile.md 'CurlDotNet\.Core\.CurlOptions\.DumpHeaderFile')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.NoClobber Property

Never overwrites an existing output file; the download is saved under a numbered name instead
\(`report-1.txt`, `report-2.txt`, or `name.1` without an extension\), like curl's `--no-clobber`\.

```csharp
public bool NoClobber { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to keep existing files\. Default: false \(existing files are replaced\)\.

### Remarks

Up to 100 numbered names are tried; when all are taken the transfer fails with
            [CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException') (exit 23). The name actually written is in
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: `curl --no-clobber -o report.txt URL` is `new CurlOptions { Url = url, OutputFile = "report.txt", NoClobber = true }`.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [SkipExisting](CurlDotNet.Core.CurlOptions.SkipExisting.md 'CurlDotNet\.Core\.CurlOptions\.SkipExisting')
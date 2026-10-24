#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.SkipExisting Property

Skips the transfer entirely when its output file already exists, like curl's `--skip-existing`; nothing is sent
and the result is empty with status 0\.

```csharp
public bool SkipExisting { get; set; }
```

#### Property Value
[System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')  
True to skip existing downloads\. Default: false\.

### Remarks

Applies to [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') and [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName'). Useful when re-running a batch of downloads.
            Example: `curl --skip-existing -O URL` is `new CurlOptions { Url = url, UseRemoteFileName = true, SkipExisting = true }`.

### See Also
- [NoClobber](CurlDotNet.Core.CurlOptions.NoClobber.md 'CurlDotNet\.Core\.CurlOptions\.NoClobber')
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlOptions](CurlDotNet.Core.CurlOptions.md 'CurlDotNet\.Core\.CurlOptions')

## CurlOptions\.OutputDir Property

Folder that relative `-o` names and `-O` file names are written into, like curl's
`--output-dir DIR`; absolute [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile') paths ignore it\.

```csharp
public string? OutputDir { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A folder path\. Default: null \(the current directory\)\.

### Remarks

The folder must exist unless [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs') is also set (exit code 23 otherwise).

Example: `curl --output-dir downloads -O https://example.com/a.zip` is
            `new CurlOptions { Url = url, OutputDir = "downloads", UseRemoteFileName = true }`, which writes `downloads/a.zip`.

### See Also
- [OutputFile](CurlDotNet.Core.CurlOptions.OutputFile.md 'CurlDotNet\.Core\.CurlOptions\.OutputFile')
- [UseRemoteFileName](CurlDotNet.Core.CurlOptions.UseRemoteFileName.md 'CurlDotNet\.Core\.CurlOptions\.UseRemoteFileName')
- [CreateDirs](CurlDotNet.Core.CurlOptions.CreateDirs.md 'CurlDotNet\.Core\.CurlOptions\.CreateDirs')
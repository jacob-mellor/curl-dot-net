#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlFormPart](CurlDotNet.Core.CurlFormPart.md 'CurlDotNet\.Core\.CurlFormPart')

## CurlFormPart\.FilePath Property

Path of the local file to upload \(for `name=@path`, with [IsFileUpload](CurlDotNet.Core.CurlFormPart.IsFileUpload.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileUpload')\) or to read as the field
value \(for `name=<path`, with [IsFileContent](CurlDotNet.Core.CurlFormPart.IsFileContent.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileContent')\)\.

```csharp
public string? FilePath { get; set; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')  
A file path, absolute or relative to the current directory\. Default: null\.

### Remarks

A file that cannot be read fails with [CurlReadErrorException](CurlDotNet.Exceptions.CurlReadErrorException.md 'CurlDotNet\.Exceptions\.CurlReadErrorException') (exit 26) when
            the body is built. Example: `-F report=@./q3.pdf` is `new CurlFormPart { Name = "report", FilePath = "./q3.pdf", IsFileUpload = true }`.

### See Also
- [IsFileUpload](CurlDotNet.Core.CurlFormPart.IsFileUpload.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileUpload')
- [IsFileContent](CurlDotNet.Core.CurlFormPart.IsFileContent.md 'CurlDotNet\.Core\.CurlFormPart\.IsFileContent')
#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')

## CurlRequestBuilder\.WithUploadFile\(string, string\) Method

Adds a file upload field to a `multipart/form-data` body \(curl's `-F 'name=@path'`\) without
checking the file now; switches a GET to POST\.

```csharp
public CurlDotNet.Core.CurlRequestBuilder WithUploadFile(string parameterName, string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).parameterName'></a>

`parameterName` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The form field name the server expects, such as `"file"`\.

<a name='CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Path of the file to upload, absolute or relative to the current directory\.

#### Returns
[CurlRequestBuilder](CurlDotNet.Core.CurlRequestBuilder.md 'CurlDotNet\.Core\.CurlRequestBuilder')  
This builder, for chaining\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[parameterName](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md#CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).parameterName 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)\.parameterName') is null or empty\. Fix: pass the form field
            name the server expects, such as `"file"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-file\-upload\-csharp/README\.md

### Remarks

The file is read when the request runs; a missing file then fails with a [CurlException](CurlDotNet.Exceptions.CurlException.md 'CurlDotNet\.Exceptions\.CurlException')
            (exit code 26) naming the path. Use [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)') to check the path immediately.
            Calling it again with the same [parameterName](CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).md#CurlDotNet.Core.CurlRequestBuilder.WithUploadFile(string,string).parameterName 'CurlDotNet\.Core\.CurlRequestBuilder\.WithUploadFile\(string, string\)\.parameterName') replaces the earlier path.

Example: `CurlRequestBuilder.Post("https://api.example.com/import").WithUploadFile("csv", "export.csv")`
            is `curl -F 'csv=@export.csv' https://api.example.com/import`.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-file-upload-csharp/README.md

### See Also
- [WithFile\(string, string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string,string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string, string\)')
- [WithFile\(string\)](CurlDotNet.Core.CurlRequestBuilder.WithFile.md#CurlDotNet.Core.CurlRequestBuilder.WithFile(string) 'CurlDotNet\.Core\.CurlRequestBuilder\.WithFile\(string\)')
- [Files](CurlDotNet.Core.CurlOptions.Files.md 'CurlDotNet\.Core\.CurlOptions\.Files')
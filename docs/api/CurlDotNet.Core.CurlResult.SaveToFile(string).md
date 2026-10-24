#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveToFile\(string\) Method

Writes the response to [filePath](CurlDotNet.Core.CurlResult.SaveToFile(string).md#CurlDotNet.Core.CurlResult.SaveToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)\.filePath') \- the bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData') for binary responses,
otherwise the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \- overwriting any existing file, and returns this result\.

```csharp
public CurlDotNet.Core.CurlResult SaveToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative \(to the current directory\) or absolute path, such as `"data.json"`,
            `"downloads/report.pdf"` or `@"C:\Temp\out.txt"`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[filePath](CurlDotNet.Core.CurlResult.SaveToFile(string).md#CurlDotNet.Core.CurlResult.SaveToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)\.filePath') is null, empty or whitespace; pass a path such as
            `"out.json"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\): the folder does not
            exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
            characters\. The message names the path, the cause and the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

Text is written as UTF-8 (without a byte-order mark); binary data byte-for-byte. The path is added to
            [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'). The folder must already exist. Note that [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') is what curl prints, so a
            command with `-i` or `-w` saves those extras too.

For large downloads prefer curl's own `-o file` (or `-O`), which writes as the transfer runs:
            `await Curl.ExecuteAsync("curl -o report.pdf https://example.com/report.pdf");`

Example: `(await Curl.ExecuteAsync("curl https://example.com/logo.png")).SaveToFile("logo.png");` or chained:
            `var data = result.EnsureSuccess().SaveToFile("backup.json").ParseJson<Data>();`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [SaveToFileAsync\(string\)](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)')
- [AppendToFile\(string\)](CurlDotNet.Core.CurlResult.AppendToFile(string).md 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)')
- [SaveAsJson\(string, bool\)](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)')
- [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles')
- [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData')
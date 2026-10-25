#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.AppendToFile\(string\) Method

Appends the response \(bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), or the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) to the end of
[filePath](CurlDotNet.Core.CurlResult.AppendToFile(string).md#CurlDotNet.Core.CurlResult.AppendToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)\.filePath'), creating the file if needed, and returns this result\.

```csharp
public CurlDotNet.Core.CurlResult AppendToFile(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.AppendToFile(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative or absolute path of the file to append to, such as `"api-log.txt"`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[filePath](CurlDotNet.Core.CurlResult.AppendToFile(string).md#CurlDotNet.Core.CurlResult.AppendToFile(string).filePath 'CurlDotNet\.Core\.CurlResult\.AppendToFile\(string\)\.filePath') is null, empty or whitespace; pass a path such as
            `"out.json"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\): the folder does not
            exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
            characters\. The message names the path, the cause and the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

Nothing is added between responses - append your own newline if you need one. Unlike
            [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)'), the path is not added to [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'). Text is written as UTF-8.

Example: `foreach (var url in urls) (await Curl.ExecuteAsync($"curl {url}")).AppendToFile("combined.txt");`

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [SaveToFileAsync\(string\)](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)')
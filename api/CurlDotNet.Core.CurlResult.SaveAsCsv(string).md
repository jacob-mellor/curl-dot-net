#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveAsCsv\(string\) Method

Converts a JSON array of objects in [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to CSV \(one column per property of the first object\)
and writes it to [filePath](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md#CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)\.filePath'); a body that is not such an array is written unchanged\.

```csharp
public CurlDotNet.Core.CurlResult SaveAsCsv(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative or absolute path of the CSV file to create or overwrite, such as `"users.csv"`\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[filePath](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md#CurlDotNet.Core.CurlResult.SaveAsCsv(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)\.filePath') is null, empty or whitespace; pass a path such as
            `"out.json"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\): the folder does not
            exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
            characters\. The message names the path, the cause and the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

The header row comes from the first object's property names; each later object contributes the values
            of those properties (missing ones are empty, extra ones are ignored). Values containing a comma or a quote are
            quoted with doubled quotes; nested objects and arrays are written as their JSON text. An empty array writes an
            empty file. The path is added to [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: for `[{"name":"John","age":30},{"name":"Jane","age":25}]`,
            `result.SaveAsCsv("users.csv")` writes `name,age`, `John,30`, `Jane,25` - ready for Excel.

### See Also
- [SaveAsJson\(string, bool\)](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)')
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
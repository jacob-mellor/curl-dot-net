#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveAsJson\(string, bool\) Method

Writes [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') to [filePath](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md#CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)\.filePath') as re\-formatted JSON \(indented by default, or
minified\), and returns this result; a body that is not JSON is written unchanged\.

```csharp
public CurlDotNet.Core.CurlResult SaveAsJson(string filePath, bool indented=true);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative or absolute path of the file to create or overwrite, such as `"data.json"`\.

<a name='CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).indented'></a>

`indented` [System\.Boolean](https://learn.microsoft.com/en-us/dotnet/api/system.boolean 'System\.Boolean')

true \(default\) for two\-space indented output, false for minified single\-line JSON\.

#### Returns
[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')  
This same result, for chaining\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[filePath](CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).md#CurlDotNet.Core.CurlResult.SaveAsJson(string,bool).filePath 'CurlDotNet\.Core\.CurlResult\.SaveAsJson\(string, bool\)\.filePath') is null, empty or whitespace; pass a path such as
            `"out.json"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\): the folder does not
            exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
            characters\. The message names the path, the cause and the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

The JSON is parsed and written again (System.Text.Json on .NET 8+/.NET Framework, Newtonsoft.Json on
            .NET Standard 2.0), so the content is the same but the whitespace changes. Invalid JSON does not throw: the
            text is saved as-is. Binary responses are not handled here; use [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)'). The path is
            added to [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles').

Example: `(await Curl.ExecuteAsync("curl https://api.github.com/repos/dotnet/runtime")).SaveAsJson("runtime.json");`
            turns `{"id":1,"name":"x"}` into a readable, indented file.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/parse-json-from-curl-in-csharp/README.md

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [SaveAsCsv\(string\)](CurlDotNet.Core.CurlResult.SaveAsCsv(string).md 'CurlDotNet\.Core\.CurlResult\.SaveAsCsv\(string\)')
- [ParseJson&lt;T&gt;\(\)](CurlDotNet.Core.CurlResult.ParseJson_T_().md 'CurlDotNet\.Core\.CurlResult\.ParseJson\<T\>\(\)')
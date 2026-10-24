#### [CurlDotNet](index.md 'index')
### [CurlDotNet\.Core](CurlDotNet.Core.md 'CurlDotNet\.Core').[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

## CurlResult\.SaveToFileAsync\(string\) Method

Writes the response to [filePath](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md#CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)\.filePath') without blocking the calling thread \- bytes for binary
responses, text for the rest \- and returns this result when the file is complete\.

```csharp
public System.Threading.Tasks.Task<CurlDotNet.Core.CurlResult> SaveToFileAsync(string filePath);
```
#### Parameters

<a name='CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative or absolute path of the file to create or overwrite, such as `"exports/data.json"`\.

#### Returns
[System\.Threading\.Tasks\.Task&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')[CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1 'System\.Threading\.Tasks\.Task\`1')  
A task that completes with this same result once the file is written\.

#### Exceptions

[System\.ArgumentException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentexception 'System\.ArgumentException')  
[filePath](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md#CurlDotNet.Core.CurlResult.SaveToFileAsync(string).filePath 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)\.filePath') is null, empty or whitespace; pass a path such as
            `"out.json"`\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(exit code 23\): the folder does not
            exist, access is denied, the file is locked by another program, the disk is full, or the name contains invalid
            characters\. The message names the path, the cause and the fix\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

Same rules as [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)'): overwrites the file, writes UTF-8 text or raw bytes, adds the
            path to [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles'), and needs an existing folder. On .NET 8+ it uses the asynchronous file APIs; on
            .NET Standard 2.0 and .NET Framework, which lack them, the write runs on a thread-pool thread.

Example: `var result = await Curl.ExecuteAsync("curl https://example.com/data.json"); await result.SaveToFileAsync("data.json");`

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [OutputFiles](CurlDotNet.Core.CurlResult.OutputFiles.md 'CurlDotNet\.Core\.CurlResult\.OutputFiles')
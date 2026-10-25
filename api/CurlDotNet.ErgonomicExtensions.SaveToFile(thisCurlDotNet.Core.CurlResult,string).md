#### [CurlDotNet](index.md 'index')
### [CurlDotNet](CurlDotNet.md 'CurlDotNet').[ErgonomicExtensions](CurlDotNet.ErgonomicExtensions.md 'CurlDotNet\.ErgonomicExtensions')

## ErgonomicExtensions\.SaveToFile\(this CurlResult, string\) Method

Writes the response \(bytes of [BinaryData](CurlDotNet.Core.CurlResult.BinaryData.md 'CurlDotNet\.Core\.CurlResult\.BinaryData'), else the text of [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body')\) to
[filePath](CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).filePath 'CurlDotNet\.ErgonomicExtensions\.SaveToFile\(this CurlDotNet\.Core\.CurlResult, string\)\.filePath') after checking it is a 2xx response with content, and returns how much was written\.

```csharp
public static long SaveToFile(this CurlDotNet.Core.CurlResult result, string filePath);
```
#### Parameters

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).result'></a>

`result` [CurlResult](CurlDotNet.Core.CurlResult.md 'CurlDotNet\.Core\.CurlResult')

The response to save; must not be null\.

<a name='CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).filePath'></a>

`filePath` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

Relative or absolute path of the file to create or overwrite, such as `"report.pdf"`\.

#### Returns
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')  
For binary data, the number of bytes written; for text, the number of characters in
            [Body](CurlDotNet.Core.CurlResult.Body.md 'CurlDotNet\.Core\.CurlResult\.Body') \(the UTF\-8 file can be larger for non\-ASCII text\)\.

#### Exceptions

[System\.ArgumentNullException](https://learn.microsoft.com/en-us/dotnet/api/system.argumentnullexception 'System\.ArgumentNullException')  
[result](CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).md#CurlDotNet.ErgonomicExtensions.SaveToFile(thisCurlDotNet.Core.CurlResult,string).result 'CurlDotNet\.ErgonomicExtensions\.SaveToFile\(this CurlDotNet\.Core\.CurlResult, string\)\.result') is null\.

[System\.InvalidOperationException](https://learn.microsoft.com/en-us/dotnet/api/system.invalidoperationexception 'System\.InvalidOperationException')  
The status is not 2xx, or there is no body and no binary data\. The
            message names the URL and status and says how to fix it\.
            See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

[CurlWriteErrorException](CurlDotNet.Exceptions.CurlWriteErrorException.md 'CurlDotNet\.Exceptions\.CurlWriteErrorException')  
The file could not be written \(folder missing, access denied, file locked,
            disk full, invalid name\); exit code 23\. See https://github\.com/jacob\-mellor/curl\-dot\-net/blob/master/docs/how\-to/curl\-download\-file\-csharp/README\.md

### Remarks

Normal call syntax `result.SaveToFile(path)` binds to the instance method
            [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)') (no status check, returns the result). Call this one statically:
            `long written = ErgonomicExtensions.SaveToFile(result, "out.bin");`

Overwrites an existing file; text is written as UTF-8; the folder must exist.

Tutorial: https://github.com/jacob-mellor/curl-dot-net/blob/master/docs/how-to/curl-download-file-csharp/README.md

### See Also
- [SaveToFile\(string\)](CurlDotNet.Core.CurlResult.SaveToFile(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFile\(string\)')
- [SaveToFileAsync\(string\)](CurlDotNet.Core.CurlResult.SaveToFileAsync(string).md 'CurlDotNet\.Core\.CurlResult\.SaveToFileAsync\(string\)')